using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACW = Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.Explorer.Extensibility;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using VDFV = Autodesk.DataManagement.Client.Framework.Vault;
using Autodesk.Connectivity.Explorer.ExtensibilityTools;
using System.Windows.Forms;

namespace Autodesk.TS.VltPlmAddIn.Model
{
    internal class Navigation
    {
        private Connection? _conn;
        private IApplication? _application;
        private IExplorerUtil? _explorerUtil;

        private string _navigationSource = null;

        public Navigation()
        {
            _conn = VaultExplorerExtension.mConnection;
            _application = VaultExplorerExtension.mApplication;
            _explorerUtil = Autodesk.Connectivity.Explorer.ExtensibilityTools.ExplorerLoader.GetExplorerUtil(_application);
        }

        internal void GotoVaultFolder(string[] parameters)
        {
            if (long.TryParse(parameters[1], out long folderId))
            {
                ACW.Folder? folder = _conn?.WebServiceManager.DocumentService.GetFolderById(folderId);
                _explorerUtil?.GoToEntity(new VDFV.Currency.Entities.Folder(_conn, folder));
            }
            else
            {
                //todo: Vault error message;
            }
        }

        internal void GotoVaultFile(string[] parameters)
        {
            _navigationSource = parameters[0];
            long fileId = -1;

            if (_navigationSource == "item")
            {
                // get the primary file of the item
                ACW.Item? item = _conn?.WebServiceManager.ItemService.GetLatestItemByItemMasterId(long.Parse(parameters[1]));
                ACW.ItemFileAssoc[]? itemFileAssocs = _conn?.WebServiceManager.ItemService.GetItemFileAssociationsByItemIds(new long[] { item.Id }, ACW.ItemFileLnkTypOpt.Primary);
                if (itemFileAssocs != null && itemFileAssocs.Any())
                {
                    fileId = itemFileAssocs.First().CldFileId;
                }
                else
                {
                    // todo: Vault error message;
                    return;
                }
            }
            else if (_navigationSource == "file")
            {
                fileId = long.Parse(parameters[1]);
            }

            try
            {
                ACW.File? file = _conn?.WebServiceManager.DocumentService.GetFileById(fileId);
                _explorerUtil?.GoToEntity(new VDFV.Currency.Entities.FileIteration(_conn, file));
            }
            catch (Exception)
            {
                //todo: Vault error message;
            }
        }

        internal void GotoVaultItem(string[] parameters)
        {
            _navigationSource = parameters[0];
            ACW.Item? item = null;

            if (_navigationSource == "file")
            {
                //get the item of the file
                try
                {
                    item = _conn?.WebServiceManager.ItemService.GetItemsByFileIdAndLinkTypeOptions(long.Parse(parameters[1]), ACW.ItemFileLnkTypOpt.Primary).FirstOrDefault();
                    _explorerUtil?.GoToEntity(new VDFV.Currency.Entities.ItemRevision(_conn, item));
                    return;
                }
                catch (Exception)
                {
                    // todo: Vault error message;
                }
            }

            // navigate to the item using itemrevision
            if (_navigationSource == "item")
            {
                try
                {
                    item = _conn?.WebServiceManager.ItemService.GetItemsByRevisionIds(new long[] { long.Parse(parameters[1]) }, true).FirstOrDefault();
                    _explorerUtil?.GoToEntity(new VDFV.Currency.Entities.ItemRevision(_conn, item));
                    return;
                }
                catch (Exception)
                {
                    // todo: Vault error message;
                }
            }

            // navigate to the item using plm item number
            if (_navigationSource == "plm-item")
            {
                try
                {
                    item = _conn?.WebServiceManager.ItemService.GetLatestItemByItemNumber(parameters[1]);
                    _explorerUtil?.GoToEntity(new VDFV.Currency.Entities.ItemRevision(_conn, item));
                    return;
                }
                catch (Exception)
                {
                }
            }
        }

        internal void GotoVaultChangeOrder(string[] parameters)
        {
            var explorerUtil = Autodesk.Connectivity.Explorer.ExtensibilityTools.ExplorerLoader.GetExplorerUtil(_application);
            if (!string.IsNullOrEmpty(parameters[1]))
            {
                string changeOrderId = parameters[1];
                ACW.ChangeOrder? changeOrder = _conn?.WebServiceManager.ChangeOrderService.GetChangeOrderByNumber(changeOrderId);
                explorerUtil.GoToEntity(new VDFV.Currency.Entities.ChangeOrder(_conn, changeOrder));
            }
            else
            {
            }
        }
    }
}
