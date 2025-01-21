using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACW = Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.Explorer.Extensibility;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using VDFV = Autodesk.DataManagement.Client.Framework.Vault;

namespace Autodesk.TS.VltPlmAddIn.Model
{
    internal class Navigation
    {
        private Connection? _conn;
        private IApplication? _application;

        public Navigation()
        {
            _conn = VaultExplorerExtension.conn;
            _application = VaultExplorerExtension.mApplication;
        }

        internal void GotoVaultFolder(string[] parameters)
        {
            var explorerUtil = Autodesk.Connectivity.Explorer.ExtensibilityTools.ExplorerLoader.GetExplorerUtil(_application);
            if (long.TryParse(parameters[1], out long folderId))
            {
                ACW.Folder? folder = _conn?.WebServiceManager.DocumentService.GetFolderById(folderId);
                explorerUtil.GoToEntity(new VDFV.Currency.Entities.Folder(_conn, folder));
            }
            else
            {
                //todo: Vault error message;
            }
        }

        internal void GotoVaultFile(string[] parameters)
        {
            var explorerUtil = Autodesk.Connectivity.Explorer.ExtensibilityTools.ExplorerLoader.GetExplorerUtil(_application);
            if (long.TryParse(parameters[1], out long fileId))
            {
                ACW.File? file = _conn?.WebServiceManager.DocumentService.GetFileById(fileId);
                explorerUtil.GoToEntity(new VDFV.Currency.Entities.FileIteration(_conn, file));
            }
            else
            {
                //todo: Vault error message;
            }
        }

        internal void GotoVaultItem(string[] parameters)
        {
            var explorerUtil = Autodesk.Connectivity.Explorer.ExtensibilityTools.ExplorerLoader.GetExplorerUtil(_application);
            if (long.TryParse(parameters[1], out long itemId))
            {

            }
            else
            {
                // todo: Vault error message;
            }
        }
    }
}
