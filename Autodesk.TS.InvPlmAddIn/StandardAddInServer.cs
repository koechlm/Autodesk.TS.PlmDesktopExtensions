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

        // Inventor application object.
        private const string mClsId = "{08b0dcaa-5757-4a40-a785-c79fae87efea}";
        private Inventor.Application mInventorApplication;
        private UserInterfaceManager mUserInterfaceManager;
        private global::InvPlmAddIn.Forms.mDockWindowChild mDockWinChild;
        private DockableWindow mPlmNavigatorWindow, mPlmTasksWindow, mPlmSearchWindow;
        public const string mSearchWinName = "plmSearchWindow";
        public const string mTasksWinName = "plmTasksWindow";
        public const string mNavigatorWinName = "plmNavigatorWindow";
        private bool mPlmNvgtrWinExists = false, mPlmTsksWinExists = false, mPlmSrchWinExists = false;


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
                if (mWindow.InternalName == "plmSearchWindow")
                {
                    if (mPlmSrchWinExists == false)
                    {
                        mPlmSearchWindow = mUserInterfaceManager.DockableWindows.Add(mClsId, mSearchWinName, "PLM Search");
                        mPlmSrchWinExists = true;
                        //add the child form to the dockable window
                        mDockWinChild = new global::InvPlmAddIn.Forms.mDockWindowChild(mInventorApplication.ActiveColorScheme.Name, mWindow.InternalName);
                        mPlmSearchWindow.AddChild(mDockWinChild);
                        mDockWinChild.Show();
                    }
                    else
                    {
                        mPlmSearchWindow = mWindow;
                        mPlmSearchWindow.ShowVisibilityCheckBox = true;
                    }
                }

                if (mWindow.InternalName == "plmTasksWindow" && mPlmTsksWinExists != false)
                {
                    if (mPlmTsksWinExists == false)
                    {
                        mPlmTasksWindow = mUserInterfaceManager.DockableWindows.Add(mClsId, "plmTasksWindow", "PLM Tasks");
                        mPlmTsksWinExists = true;
                        //add the child form to the dockable window
                        mDockWinChild = new global::InvPlmAddIn.Forms.mDockWindowChild(mInventorApplication.ActiveColorScheme.Name, mWindow.InternalName);
                        mPlmTasksWindow.AddChild(mDockWinChild);
                        mDockWinChild.Show();
                    }
                    else
                    {
                        mPlmTasksWindow = mWindow;
                        mPlmTasksWindow.ShowVisibilityCheckBox = true;
                    }
                }

                if (mWindow.InternalName == "plmNavigatorWindow" && mPlmNvgtrWinExists != false)
                {
                    if (mPlmNvgtrWinExists == false)
                    {
                        mPlmNavigatorWindow = mUserInterfaceManager.DockableWindows.Add(mClsId, "plmNavigatorWindow", "PLM Navigator");
                        mPlmNvgtrWinExists = true;
                        //add the child form to the dockable window
                        mDockWinChild = new global::InvPlmAddIn.Forms.mDockWindowChild(mInventorApplication.ActiveColorScheme.Name, mWindow.InternalName);
                        mPlmNavigatorWindow.AddChild(mDockWinChild);
                        mDockWinChild.Show();
                    }
                    else
                    {
                        mPlmNavigatorWindow = mWindow;
                        mPlmNavigatorWindow.ShowVisibilityCheckBox = true;
                    }
                }
            }
        }

        public void Deactivate()
        {
            // This method is called by Inventor when the AddIn is unloaded.
            // The AddIn will be unloaded either manually by the user or
            // when the Inventor session is terminated

            // TODO: Add ApplicationAddInServer.Deactivate implementation

            // Release objects.

            foreach (DockableWindow mWindow in mUserInterfaceManager.DockableWindows)
            {

                if (mWindow.InternalName == "plmSearchWindow" && mPlmSrchWinExists == true)
                {
                    mWindow.Visible = false;
                    mWindow.ShowVisibilityCheckBox = false;
                    mWindow.Clear();
                    mPlmSrchWinExists = false;
                    mPlmSearchWindow = null;
                }
                if (mWindow.InternalName == "plmTasksWindow" && mPlmTsksWinExists == true)
                {
                    mWindow.Visible = false;
                    mWindow.ShowVisibilityCheckBox = false;
                    mWindow.Clear();
                    mPlmTsksWinExists = false;
                    mPlmTasksWindow = null;
                }
                if (mWindow.InternalName == "plmNavigatorWindow" && mPlmTsksWinExists == true)
                {
                    mWindow.Visible = false;
                    mWindow.ShowVisibilityCheckBox = false;
                    mWindow.Clear();
                    mPlmNvgtrWinExists = false;
                    mPlmNavigatorWindow = null;
                }
            }

            // Release objects.
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
