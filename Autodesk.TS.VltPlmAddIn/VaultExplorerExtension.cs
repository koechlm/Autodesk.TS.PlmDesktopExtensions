using Autodesk.Connectivity.Explorer.Extensibility;
using VDF = Autodesk.DataManagement.Client.Framework;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using Autodesk.TS.VltPlmAddIn.Forms;
using Autodesk.TS.VltPlmAddIn.Utils;
using System.Reflection;
using Autodesk.DataManagement.Client.Framework.Forms;
using ACW = Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.WebServices;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Entities;
using System.Drawing;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Windows.Forms;

// These 5 assembly attributes must be specified or your extension will not load. 
//[assembly: AssemblyCompany("Autodesk")]
//[assembly: AssemblyProduct("HelloWorldCommandExtension")]
[assembly: AssemblyDescription("FM-Vault Extension")]

// The extension ID needs to be unique for each extension.  
// Make sure to generate your own ID when writing your own extension. 
[assembly: Autodesk.Connectivity.Extensibility.Framework.ExtensionId("0AAB9A1B-B055-4ED0-B54D-08720E1F677F")]

// This number gets incremented for each Vault release.
// *ComponentUpgradeEveryRelease-Client*
[assembly: Autodesk.Connectivity.Extensibility.Framework.ApiVersion("19.0")]

namespace Autodesk.TS.VltPlmAddIn
{
    
        public enum NavigationSender
        {
            FMExtension,
            Host
        }
    

    public class VaultExplorerExtension : IExplorerExtension
    {
        // Capture the current theme on startup
        internal static string mCurrentTheme = "light";

        internal static Connection? conn { get; set; }

        internal static IApplication? mApplication { get; set; }

        internal static string? mFmExtensionUrl { get; set; }

        internal static NavigationSender? mSender { get; set; }

        ISelection? selection1 = null;
        ISelection? selection2 = null;


        #region IExplorerExtension Members

        IEnumerable<CommandSite>? IExplorerExtension.CommandSites()
        {
            return null;
        }

        IEnumerable<CustomEntityHandler>? IExplorerExtension.CustomEntityHandlers()
        {
            return null;
        }

        IEnumerable<DetailPaneTab>? IExplorerExtension.DetailTabs()
        {
            return null;
        }

        IEnumerable<DockPanel> IExplorerExtension.DockPanels()
        {
            // Create a DockPanel list to return from method
            List<DockPanel> mDockPanels = new List<DockPanel>();

            // Create a dock panels for Vault/Fusion Manage Search, ITem/BOM and Tasks
            DockPanel? mPanelSearch = new DockPanel(Guid.Parse("E2B3E9C6-80B2-4FED-8DF5-08E8C830E31E"),
                                                "FM Search", typeof(WebViewFmSearch));
            mDockPanels.Add(mPanelSearch);

            DockPanel? mPanelItemDetails = new DockPanel(Guid.Parse("31DB4F79-84D5-4D67-A109-5807563BE133"),
                                                "FM Item Details", typeof(WebViewFmItem));
            // Add event handler for selection changed event; the content of the panel needs to update accordingly.
            mPanelItemDetails.SelectionChanged += mPanelItemDetails_SelectionChanged;
            mDockPanels.Add(mPanelItemDetails);

            DockPanel? mPanelTasks = new DockPanel(Guid.Parse("7B5E20B1-C3FD-42FB-8955-A8D57D2015B2"),
                                                "FM Tasks", typeof(WebViewFmTasks));
            //no event handler for now: the content is the personal tasks of the user and not related to the selected object in Vault
            mDockPanels.Add(mPanelTasks);

            // Returns panels
            return mDockPanels;
        }

        IEnumerable<string> IExplorerExtension.HiddenCommands()
        {
            return null;
        }

        void IExplorerExtension.OnLogOff(IApplication application)
        {
        }

        void IExplorerExtension.OnLogOn(IApplication application)
        {
            conn = application.Connection;
        }

        void IExplorerExtension.OnShutdown(IApplication application)
        {
        }

        void IExplorerExtension.OnStartup(IApplication application)
        {
            mApplication = application;

            mCurrentTheme = VDF.Forms.Library.CurrentTheme.ToString();
            VDF.Forms.Library.ThemeChanged += ThemeChanged;

            Autodesk.TS.VltPlmAddIn.Utils.Settings mSettings = new Autodesk.TS.VltPlmAddIn.Utils.Settings();
            mSettings = Settings.Load();
            mFmExtensionUrl = mSettings.FmExtensionUrl;

            mSender = NavigationSender.Host;
        }

        #endregion

        #region custom methods
        
        private void ThemeChanged(object? sender, Library.UITheme e)
        {
            mCurrentTheme = VDF.Forms.Library.CurrentTheme.ToString().ToLower();
        }

        private void mPanelItemDetails_SelectionChanged(object? sender, DockPanelSelectionChangedEventArgs? e)
        {
            if (mSender as NavigationSender? == NavigationSender.FMExtension)
            {
                return;
            }
            ////mimic NavigationSelection changed handler, as the selection changed event is a general event and not specific to the navigation selection change
            //if (e?.Context?.NavSelectionSet.GetEnumerator().Current.Id == -1)
            //{
            //    return;
            //}
            //if (e?.Context?.SelectedObject == null)
            //{
            //    return;
            //}
            //if (e.Context.NavSelectionSet.Count() == 0)
            //{
            //    return;
            //}
            //if (e.Context.SelectedObject != null)
            //{
            //    selection2 = e.Context.SelectedObject;
            //}

            //selection changed
            //if (selection1?.Id != selection2?.Id)
            //{
                // filter the selected entities for candidates to navigate to in the Item Details panel
                if (selection2?.TypeId.EntityClassId is not "FILE" and not "ITEM")
                {
                    return;
                }

                string? mItemNumber = "";

                if (selection2.TypeId.EntityClassId == "FILE")
                {
                    // Look of the File object.  How we do this depends on what is selected.
                    Item? mItem = null;
                    if (selection2.TypeId == SelectionTypeId.File)
                    {
                        // our ISelection.Id is really a File.MasterId
                        var selectedFile = conn?.WebServiceManager.DocumentService.GetLatestFileByMasterId(selection2.Id);
                        if (selectedFile != null)
                        {
                            var items = conn?.WebServiceManager.ItemService.GetItemsByFileId(selectedFile.Id);
                            mItem = items?.FirstOrDefault();
                        }
                    }
                    else if (selection2.TypeId == SelectionTypeId.FileVersion)
                    {
                        // our ISelection.Id is really a File.Id
                        var items = conn?.WebServiceManager.ItemService.GetItemsByFileId(selection2.Id);
                        mItem = items?.FirstOrDefault();
                    }
                    mItemNumber = mItem?.ItemNum;
                }

                if (selection2.TypeId.EntityClassId == "ITEM")
                {
                    ACW.Item? mItem = conn?.WebServiceManager.ItemService.GetLatestItemByItemMasterId(selection2.Id);
                    mItemNumber = mItem?.ItemNum;
                }

                try
                {
                    // The event args has our custom panel object.  We need to cast it to our type.
                    //CefControlItem? mCefControl = e.Context.UserControl as CefControlItem;
                    WebViewFmItem mFmItemControl = e.Context.UserControl as WebViewFmItem;

                // build the Item's URL and navigate to the selected mItem in the CEF browser
                if (mFmItemControl != null)
                    {
                        string mUrl = mFmExtensionUrl + "/Item?number=" + mItemNumber + "&theme=" + mCurrentTheme.ToLower();
                    //await mCefControl.NavigateToUrlAsync(mUrl);
                    mFmItemControl.Navigate(mUrl);
                }
                }
                catch (Exception ex)
                {
                    // If something goes wrong, we don't want the exception to bubble up to Vault Explorer.
                    MessageBox.Show("Error: " + ex.Message);
                    
                }

                selection1 = selection2;

                // Set the sender to FMExtension to avoid infinite loop
                mSender = NavigationSender.Host;
            //}
        }

        public IEnumerable<CommandSite> CommandSites()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<DetailPaneTab> DetailTabs()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<DockPanel> DockPanels()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<CustomEntityHandler> CustomEntityHandlers()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> HiddenCommands()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
