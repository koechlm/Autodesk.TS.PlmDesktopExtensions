using Autodesk.Connectivity.Explorer.Extensibility;
using System.Reflection;

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
    public class VaultExplorerExtension : IExplorerExtension
    {
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
                                                "Fusion Manage Search", typeof(CefControlItem));          
            mDockPanels.Add(mPanelSearch);
       
            DockPanel? mPanelItemDetails = new DockPanel(Guid.Parse("31DB4F79-84D5-4D67-A109-5807563BE133"),
                                                "Fusion Manage Item Details", typeof(CefControlItem));
            // Add event handler for selection changed event; the content of the panel needs to update accordingly.
            mPanelItemDetails.SelectionChanged += mPanelItemDetails_SelectionChanged;
            mDockPanels.Add(mPanelItemDetails);

            DockPanel? mPanelTasks = new DockPanel(Guid.Parse("7B5E20B1-C3FD-42FB-8955-A8D57D2015B2"),
                                                "Fusion Manage Tasks", typeof(CefControlItem));
            //no event handler for now: the content is the personal tasks of the user and not related to the selected object in Vault
            mDockPanels.Add(mPanelTasks);

            // Returns panels
            return mDockPanels;
        }

        IEnumerable<string>? IExplorerExtension.HiddenCommands()
        {
            return null;
        }

        void IExplorerExtension.OnLogOff(IApplication application)
        {
        }

        void IExplorerExtension.OnLogOn(IApplication application)
        {
        }

        void IExplorerExtension.OnShutdown(IApplication application)
        {
        }

        void IExplorerExtension.OnStartup(IApplication application)
        {            
        }

        private void mPanelItemDetails_SelectionChanged(object? sender, DockPanelSelectionChangedEventArgs e)
        {
            try
            {
                // The event args has our custom panel object.  We need to cast it to our type.
                CefControlItem? mCefControl = e.Context.UserControl as CefControlItem;
                // Send selection to the panel so that it can display the object.
                mCefControl?.SetSelectedObject(e.Context.SelectedObject);
            }
            catch (Exception ex)
            {
                // If something goes wrong, we don't want the exception to bubble up to Vault Explorer.
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
