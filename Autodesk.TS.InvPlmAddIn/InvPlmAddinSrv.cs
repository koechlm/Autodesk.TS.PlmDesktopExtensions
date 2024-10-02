using Inventor;
using InvPlmAddIn.Model;
using InvPlmAddIn.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

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
        public static readonly string AddInName = "Fusion Manage Inventor Extension";
        public static string ClientId = "{08b0dcaa-5757-4a40-a785-c79fae87efea}";
        public static Inventor.Application mInventorApplication { get; set; }
        private UserInterfaceManager mUserInterfaceManager;

        //get the base uri of the Fusion Manage Extension
        private static Utils.Settings mAddinSettings = Utils.Settings.Load();
        public static Uri mBaseUri = new Uri(mAddinSettings.FmExtensionUrl);

        private global::InvPlmAddIn.Forms.FMExtensionLogin mLoginDialog;             

        public BrowserPanelWindowManager WindowManager { get; set; }

        private static string PathToWebSite
            => System.IO.Path.Combine(System.IO.Path.GetDirectoryName(MethodBase.GetCurrentMethod().ReflectedType.Assembly.Location), "PLMInventorRules");

        public static string ILogicFullRulePath(string rulename)
            => System.IO.Path.Combine(PathToWebSite, $"{rulename}.iLogicVb");

        public InvPlmAddinSrv()
        {
        }

        #region ApplicationAddInServer Members

        public void Activate(Inventor.ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            // This method is called by Inventor when it loads the addin.
            // The AddInSiteObject provides access to the Inventor mInventorApplication object.
            // The FirstTime flag indicates if the addin is loaded for the first time.

            // Initialize mAddIn members.
            mInventorApplication = addInSiteObject.Application;
            mUserInterfaceManager = mInventorApplication.UserInterfaceManager;

            // Show the FMExtension Login dialog and continue only if the user has logged in and confirmed the dialog with OK
            mLoginDialog = new InvPlmAddIn.Forms.FMExtensionLogin(mInventorApplication.ActiveColorScheme.Name);
            if (mLoginDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            WindowManager = new BrowserPanelWindowManager(this);
            ConnectEvents();
            AddBrowserWindow();
            
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
                AdskTsVaultUtils.Messages.ShowError(string.Format("The initialization of the WindowManager failed with unhandled exception: {0}", e.Message), AddInName);
            }
        }

        private void ApplicationEventsOnActivateDocument(_Document documentObject, EventTimingEnum beforeOrAfter,
            NameValueMap context, out HandlingCodeEnum handlingCode)
        {
            handlingCode = HandlingCodeEnum.kEventNotHandled;

            if (beforeOrAfter == EventTimingEnum.kAfter)
                WindowManager.OnChangeDocument(documentObject);
        }

        private void ApplicationEventsOnCloseView(View viewObject, EventTimingEnum beforeOrAfter, NameValueMap context,
            out HandlingCodeEnum handlingCode)
        {
            handlingCode = HandlingCodeEnum.kEventNotHandled;
        }

        private void ApplicationEventsOnDeactivateView(View viewObject, EventTimingEnum beforeOrAfter,
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

            WindowManager.DeactivateByInventorAddIn();
        }

        private void RemoveBrowserWindow()
        {
            WindowManager.Remove();
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
