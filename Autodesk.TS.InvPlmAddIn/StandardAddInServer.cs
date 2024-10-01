using Inventor;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;

namespace Autodesk.TS.InvPlmAddIn
{
    /// <summary>
    /// This is the primary AddIn Server class that implements the ApplicationAddInServer interface
    /// that all Inventor AddIns are required to implement. The communication between Inventor and
    /// the AddIn is via the methods on this interface.
    /// </summary>
    [GuidAttribute("08b0dcaa-5757-4a40-a785-c79fae87efea")]
    public class StandardAddInServer : Inventor.ApplicationAddInServer
    {
        /// <summary>
        /// Display name of the addin to be used for Dialog caption.
        /// </summary>
        public static readonly string AddInName = "Fusion Manage Inventor";
        public static readonly string mClsId = "{08b0dcaa-5757-4a40-a785-c79fae87efea}";
        private Inventor.Application mInventorApplication;
        private UserInterfaceManager mUserInterfaceManager;
        private global::InvPlmAddIn.Forms.mDockWindowChild mDockWinChild;
        private DockableWindow mPlmNavigatorWindow = null, mPlmTasksWindow = null, mPlmSearchWindow = null;
        public const string mSearchWinName = "plmSearchWindow";
        public const string mTasksWinName = "plmTasksWindow";
        public const string mNavigatorWinName = "plmNavigatorWindow";


        public StandardAddInServer()
        {
        }

        #region ApplicationAddInServer Members

        public void Activate(Inventor.ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            // This method is called by Inventor when it loads the addin.
            // The AddInSiteObject provides access to the Inventor Application object.
            // The FirstTime flag indicates if the addin is loaded for the first time.

            // Initialize AddIn members.
            mInventorApplication = addInSiteObject.Application;
            mUserInterfaceManager = mInventorApplication.UserInterfaceManager;

            foreach (DockableWindow mWindow in mUserInterfaceManager.DockableWindows)
            {
                if (mWindow.InternalName.Equals(mSearchWinName, StringComparison.InvariantCultureIgnoreCase))
                {                   
                    mPlmSearchWindow = mWindow;
                }
                if (mWindow.InternalName == mTasksWinName)
                {
                    mPlmTasksWindow = mWindow;
                }
                if (mWindow.InternalName == mNavigatorWinName)
                {
                    mPlmNavigatorWindow = mWindow;
                }
            }

            if (mPlmSearchWindow == null)
            {
                try
                {
                    mPlmSearchWindow = mUserInterfaceManager.DockableWindows.Add(mClsId, mSearchWinName, "PLM Search");
                    //add the child form to the dockable window
                    mDockWinChild = new global::InvPlmAddIn.Forms.mDockWindowChild(mInventorApplication.ActiveColorScheme.Name, mSearchWinName);
                    mPlmSearchWindow.AddChild(mDockWinChild.Handle.ToInt64());
                    mDockWinChild.Show();
                }
                catch (Exception)
                {
                    //todo: add Autodesk TS Vault Utils' error message                    
                }
            }

            if (mPlmTasksWindow == null)
            {
                mPlmTasksWindow = mUserInterfaceManager.DockableWindows.Add(mClsId, mTasksWinName, "PLM Tasks");
                //add the child form to the dockable window
                try
                {
                    mDockWinChild = new global::InvPlmAddIn.Forms.mDockWindowChild(mInventorApplication.ActiveColorScheme.Name, mTasksWinName);
                    mPlmTasksWindow.AddChild(mDockWinChild.Handle.ToInt64());
                    mDockWinChild.Show();
                }
                catch (Exception)
                {
                    //todo: add Autodesk TS Vault Utils' error message                    
                }
            }

            if (mPlmNavigatorWindow == null)
            {
                try
                {
                    mPlmNavigatorWindow = mUserInterfaceManager.DockableWindows.Add(mClsId, mNavigatorWinName, "PLM Navigator");
                    //add the child form to the dockable window
                    mDockWinChild = new global::InvPlmAddIn.Forms.mDockWindowChild(mInventorApplication.ActiveColorScheme.Name, mNavigatorWinName);
                    mPlmNavigatorWindow.AddChild(mDockWinChild.Handle.ToInt64());
                    mDockWinChild.Show();
                }
                catch (Exception)
                {
                    //todo: add Autodesk TS Vault Utils' error message
                }
            }

            //add the windows to the user interface View menu
            mPlmSearchWindow.ShowVisibilityCheckBox = true;
            mPlmTasksWindow.ShowVisibilityCheckBox = true;
            mPlmNavigatorWindow.ShowVisibilityCheckBox = true;
        }

        public void Deactivate()
        {
            // This method is called by Inventor when the AddIn is unloaded.
            // The AddIn will be unloaded either manually by the user or
            // when the Inventor session is terminated
            // Release objects.

            foreach (DockableWindow mWindow in mUserInterfaceManager.DockableWindows)
            {

                if (mWindow.InternalName == "plmSearchWindow")
                {
                    mWindow.Visible = false;
                    mWindow.ShowVisibilityCheckBox = false;
                    mWindow.Clear();
                    mPlmSearchWindow = null;
                }
                if (mWindow.InternalName == "plmTasksWindow")
                {
                    mWindow.Visible = false;
                    mWindow.ShowVisibilityCheckBox = false;
                    mWindow.Clear();
                    mPlmTasksWindow = null;
                }
                if (mWindow.InternalName == "plmNavigatorWindow")
                {
                    mWindow.Visible = false;
                    mWindow.ShowVisibilityCheckBox = false;
                    mWindow.Clear();
                    mPlmNavigatorWindow = null;
                }
            }

            mInventorApplication = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void ExecuteCommand(int commandID)
        {
            // Note:this method is now obsolete, you should use the 
            // ControlDefinition functionality for implementing commands.
        }

        public object Automation
        {
            // This property is provided to allow the AddIn to expose an API 
            // of its own to other programs. Typically, this  would be done by
            // implementing the AddIn's API interface in a class and returning 
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
