using Inventor;
using InvPlmAddIn.Model;
using InvPlmAddIn.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InvPlmAddIn
{
    /// <summary>
    /// This is the primary mAddIn Server class that implements the ApplicationAddInServer interface
    /// that all Inventor AddIns are required to implement. The communication between Inventor and
    /// the mAddIn is via the methods on this interface.
    /// </summary>
    [GuidAttribute("08b0dcaa-5757-4a40-a785-c79fae87efea")]
    public class InvPlmAddinSrv : Inventor.ApplicationAddInServer
    {
        /// <summary>
        /// Display name of the addin to be used for Dialog caption.
        /// </summary>
        public static readonly string AddInName = "Vault plm Inventor";
        public static string ClientId = "{08b0dcaa-5757-4a40-a785-c79fae87efea}";
        public static Inventor.Application mInventorApplication { get; set; }
        private UserInterfaceManager mUserInterfaceManager;

        //get the base uri of the Fusion Manage Extension
        private static Utils.Settings mAddinSettings = Utils.Settings.Load();
        public static Uri mBaseUri = new Uri(mAddinSettings.FmExtensionUrl);

        private global::InvPlmAddIn.Forms.PlmExtensionLogin mLoginDialog;           


        public BrowserPanelWindowManager WindowManager { get; set; }

        private static string PathToiLogicRules
            => System.IO.Path.Combine(System.IO.Path.GetDirectoryName(MethodBase.GetCurrentMethod().ReflectedType.Assembly.Location), "InvPlmAddiniLogicRules");

        public static string ILogicFullRulePath(string rulename)
            => System.IO.Path.Combine(PathToiLogicRules, $"{rulename}.iLogicVb");

        public InvPlmAddinSrv()
        {
        }

        #region ApplicationAddInServer Members

        /// <summary>
        /// This method is called by Inventor when it loads the addin.
        /// The AddInSiteObject provides access to the Inventor mInventorApplication object.
        /// The FirstTime flag indicates if the addin is loaded for the first time.
        /// </summary>
        /// <param name="addInSiteObject"></param>
        /// <param name="firstTime"></param>
        /// <exception cref="OperationCanceledException"></exception>
        private CancellationTokenSource _cancellationTokenSource;

        public void Activate(Inventor.ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            // Initialize mAddIn members.
            mInventorApplication = addInSiteObject.Application;

            _cancellationTokenSource = new CancellationTokenSource();
            ProgressFrm progressFrm = new ProgressFrm(mInventorApplication.ActiveColorScheme.Name, _cancellationTokenSource.Token);
            progressFrm.Text = AddInName;            
            System.Threading.Thread thread = new System.Threading.Thread(() => RunProgressFrm(progressFrm, _cancellationTokenSource.Token));
            thread.Start();

            UpdateProgressFrm(progressFrm, "Autodesk Account Login ...");

            // loading the panels requires an active document
            if (mInventorApplication.ActiveDocument == null)
            {
                mInventorApplication.Documents.Add(Inventor.DocumentTypeEnum.kAssemblyDocumentObject, "", true);
            }
            mUserInterfaceManager = mInventorApplication.UserInterfaceManager;

            // the add-in requires iLogic to be enabled

            //we need the iLogic Addin to run the external rules
            try
            {
                Inventor.ApplicationAddIns mInvSrvAddIns = mInventorApplication.ApplicationAddIns;
                Inventor.ApplicationAddIn iLogicAddIn = mInvSrvAddIns.ItemById["{3BDD8D79-2179-4B11-8A5A-257B1C0263AC}"];

                if (iLogicAddIn != null && iLogicAddIn.Activated != true)
                {
                    iLogicAddIn.Activate();
                }
            }
            catch (Exception)
            {
                string mMessage = AddInName + " requires a running Inventor instance with the iLogic Addin available.\nPlease load iLogic first.";
                AdskTsVaultUtils.Messages.ShowError(mMessage, InvPlmAddinSrv.AddInName);
                throw new OperationCanceledException("Add-in loading canceled due to missing iLogic Add-in");
            }

            // Show the FMExtension Login dialog and continue only if the user has logged in and confirmed the dialog with OK
            mLoginDialog = new InvPlmAddIn.Forms.PlmExtensionLogin(mInventorApplication.ActiveColorScheme.Name);
            if (mLoginDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                // User canceled the dialog, exit the add-in
                _cancellationTokenSource.Cancel();
                thread.Join();
                throw new OperationCanceledException("Add-in loading canceled by user.");
            }

            UpdateProgressFrm(progressFrm, "Initialize Panels...");

            WindowManager = new BrowserPanelWindowManager(this);

            // Update the progress form
            UpdateProgressFrm(progressFrm, "Registering events...");
            ConnectEvents();

            UpdateProgressFrm(progressFrm, "Loading dockable windows...");
            AddBrowserWindow();

            // Request cancellation and wait for the thread to finish
            _cancellationTokenSource.Cancel();
            thread.Join();
        }

        private void RunProgressFrm(ProgressFrm progressFrm, CancellationToken token)
        {
            try
            {
                System.Windows.Forms.Application.Run(progressFrm);
            }
            catch (OperationCanceledException)
            {
                // Handle the cancellation if needed
            }
            finally
            {
                if (progressFrm.InvokeRequired)
                {
                    progressFrm.Invoke(new Action(() => progressFrm.Close()));
                }
                else
                {
                    progressFrm.Close();
                }
            }
        }

        private void UpdateProgressFrm(ProgressFrm progressFrm, string message)
        {
            if (progressFrm.InvokeRequired)
            {
                progressFrm.Invoke(new Action(() => progressFrm.lblProgress.Text = message));
            }
            else
            {
                progressFrm.lblProgress.Text = message;
            }
        }


        public void Deactivate()
        {
            // This method is called by Inventor when the mAddIn is unloaded.

            DisconnectEvents();
            RemoveBrowserWindow();

            mInventorApplication = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void CallILogic(string rulename)
        {
            global::InvPlmAddIn.Utils.iLogicUtil.ExecuteILogicRule(
                mInventorApplication,
                mInventorApplication.ActiveDocument,
                ILogicFullRulePath(rulename));
        }

        public void CallILogic(string rulename, ref Dictionary<string, object> args)
        {
            global::InvPlmAddIn.Utils.iLogicUtil.ExecuteILogicRule(
                mInventorApplication,
                mInventorApplication.ActiveDocument,
                ILogicFullRulePath(rulename), ref args);
        }


        private void AddBrowserWindow()
        {
            try
            {
                WindowManager.Init();
            }
            catch (Exception e)
            {
                AdskTsVaultUtils.Messages.ShowError(string.Format("The initialization of dockable windows for Vault plm failed with unhandled exception: {0}", e.Message), AddInName);
            }
        }

        private void ApplicationEventsOnActivateDocument(_Document documentObject, EventTimingEnum beforeOrAfter,
            NameValueMap context, out HandlingCodeEnum handlingCode)
        {
            handlingCode = HandlingCodeEnum.kEventNotHandled;

            if (beforeOrAfter == EventTimingEnum.kAfter)
                WindowManager.OnChangeDocument(documentObject);
        }

        private void ApplicationEventsOnCloseView(Inventor.View viewObject, EventTimingEnum beforeOrAfter, NameValueMap context,
            out HandlingCodeEnum handlingCode)
        {
            handlingCode = HandlingCodeEnum.kEventNotHandled;
        }

        private void ApplicationEventsOnDeactivateView(Inventor.View viewObject, EventTimingEnum beforeOrAfter,
            NameValueMap context, out HandlingCodeEnum handlingCode)
        {
            handlingCode = HandlingCodeEnum.kEventNotHandled;

            if (beforeOrAfter == EventTimingEnum.kBefore)
                WindowManager.OnChangeDocument(null);
        }

        private void ApplicationEventsOnOnCloseDocument(_Document documentObject, string fullDocumentName, EventTimingEnum beforeOrAfter, NameValueMap context, out HandlingCodeEnum handlingCode)
        {
            handlingCode = HandlingCodeEnum.kEventNotHandled;

            if (mInventorApplication.ActiveView?.Document == documentObject)
            {
                WindowManager.OnCloseDocument(documentObject);
            }
        }

        private void ConnectEvents()
        {
            mInventorApplication.ApplicationEvents.OnCloseView += ApplicationEventsOnCloseView;
            mInventorApplication.ApplicationEvents.OnActivateDocument += ApplicationEventsOnActivateDocument;
            mInventorApplication.ApplicationEvents.OnDeactivateView += ApplicationEventsOnDeactivateView;
            mInventorApplication.ApplicationEvents.OnCloseDocument += ApplicationEventsOnOnCloseDocument;
        }

        private void DisconnectEvents()
        {
            mInventorApplication.ApplicationEvents.OnCloseView -= ApplicationEventsOnCloseView;
            mInventorApplication.ApplicationEvents.OnActivateDocument -= ApplicationEventsOnActivateDocument;
            mInventorApplication.ApplicationEvents.OnDeactivateView -= ApplicationEventsOnDeactivateView;
            mInventorApplication.ApplicationEvents.OnCloseDocument -= ApplicationEventsOnOnCloseDocument;

            WindowManager?.DeactivateByInventorAddIn();
        }

        private void RemoveBrowserWindow()
        {
            WindowManager?.Remove();
        }

        public void ExecuteCommand(int commandID)
        {
            // Note:this method is now obsolete, you should use the 
            // ControlDefinition functionality for implementing commands.
        }

        public object Automation
        {
            // This property is provided to allow the mAddIn to expose an API 
            // of its own to other programs. Typically, this  would be done by
            // implementing the mAddIn's API interface in a class and returning 
            // that class object through this property.

            get
            {
                // TODO: Add ApplicationAddInServer.Automation getter implementation
                return null;
            }
        }

        #endregion

    }
}
