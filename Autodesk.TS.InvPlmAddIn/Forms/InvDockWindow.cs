using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvPlmAddIn.Forms
{
    internal class InvDockWindow
    {
        public static bool mDockWinExists { get; set; }

        public static DockableWindow mDokableWindow { get; set; }

        public static void CreateDockWindow(UserInterfaceManager mUserInterfaceManager, string mClsId)
        {
            mDockWinExists = false;
            foreach (DockableWindow mWindow in mUserInterfaceManager.DockableWindows)
            {
                if (mWindow.InternalName == "plmSearchWindow" || mWindow.InternalName == "plmTasksWindow" || mWindow.InternalName == "plmNavigatorWindow")
                {
                    mDockWinExists = true;
                    mDokableWindow = mWindow;
                    mDokableWindow.ShowVisibilityCheckBox = true;
                }
            }
            if (mDockWinExists == false)
            {
                mDokableWindow = mUserInterfaceManager.DockableWindows.Add(mClsId, "plmNavigatorWindow", "PLM Navigator");
                mDokableWindow = mUserInterfaceManager.DockableWindows.Add(mClsId, "plmTasksWindow", "PLM Tasks");
                mDokableWindow = mUserInterfaceManager.DockableWindows.Add(mClsId, "plmSearchWindow", "PLM Search");
            }
        }
    }
}
