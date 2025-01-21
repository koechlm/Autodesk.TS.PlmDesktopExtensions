using Autodesk.Connectivity.Explorer.Extensibility;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using Autodesk.TS.VltPlmAddIn.Forms;
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
        // Capture the current theme on startup
        private string mCurrentTheme = "light";

        internal static Connection? conn { get; set; }

        internal static IApplication? mApplication { get; set; }

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
                                                "FM Search", typeof(CefControlSearch));          
            mDockPanels.Add(mPanelSearch);
       
            DockPanel? mPanelItemDetails = new DockPanel(Guid.Parse("31DB4F79-84D5-4D67-A109-5807563BE133"),
                                                "FM Item Details", typeof(CefControlItem));
            // Add event handler for selection changed event; the content of the panel needs to update accordingly.
            mPanelItemDetails.SelectionChanged += mPanelItemDetails_SelectionChanged;
            mDockPanels.Add(mPanelItemDetails);

            DockPanel? mPanelTasks = new DockPanel(Guid.Parse("7B5E20B1-C3FD-42FB-8955-A8D57D2015B2"),
                                                "FM Tasks", typeof(CefControlTasks));
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
            mApplication = application;
            conn = application.Connection;
        }

        void IExplorerExtension.OnShutdown(IApplication application)
        {
        }

        void IExplorerExtension.OnStartup(IApplication application)
        {  

        }

        private void mPanelItemDetails_SelectionChanged(object? sender, DockPanelSelectionChangedEventArgs e)
        {
            //todo: check that the sender is relevant

            // filter the selected entities for candidates to navigate to in the Item Details panel
            // for now we allow Files (Category = "Part", "Assembly", "Drawing") and Items
            // todo: implement filter

            try
            {
                // The event args has our custom panel object.  We need to cast it to our type.
                CefControlItem? mCefControl = e.Context.UserControl as CefControlItem;

                // build the Item's URL and navigate to the selected item in the CEF browser
                string mUrl = "https://www.plm.tools:9600/addins/item?number=9410-000&theme=light";
                mCefControl?.NavigateToUrl(mUrl);
            }
            catch (Exception ex)
            {
                // If something goes wrong, we don't want the exception to bubble up to Vault Explorer.
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
