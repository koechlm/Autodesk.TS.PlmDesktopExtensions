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
            _conn = VaultExplorerExtension.conn;
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
            ACW.Item? mItem = null;

            // get the fileId from the parameters
            // the FM Search panel may return Vault Items, Files or Fusion Manage Items

            if (_navigationSource == "Item")
            {
                // get the primary file of the Item
                mItem = _conn?.WebServiceManager.ItemService.GetLatestItemByItemMasterId(long.Parse(parameters[1]));
                ACW.ItemFileAssoc[]? itemFileAssocs = _conn?.WebServiceManager.ItemService.GetItemFileAssociationsByItemIds(new long[] { mItem.Id }, ACW.ItemFileLnkTypOpt.Primary);
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

            if (_navigationSource == "plm-item")
            {
                // try the direct path if the itemrevision is defined
                if (parameters[1] != "undefined")
                {
                    mItem = _conn?.WebServiceManager.ItemService.GetLatestItemByItemNumber(parameters[1]);
                }
                else
                {
                    mItem = _conn?.WebServiceManager.ItemService.GetLatestItemByItemNumber(parameters[2].Split(" - ")[0]);
                }
                // get the primary file of the Item
                if (mItem != null)
                {
                    ACW.ItemFileAssoc[]? itemFileAssocs = _conn?.WebServiceManager.ItemService.GetItemFileAssociationsByItemIds(new long[] { mItem.Id }, ACW.ItemFileLnkTypOpt.Primary);
                    if (itemFileAssocs != null && itemFileAssocs.Any())
                    {
                        fileId = itemFileAssocs.First().CldFileId;
                    }
                }
            }

            if (_navigationSource == "file")
            {
                fileId = long.Parse(parameters[1]);
            }

            // finally navigate to the file
            if (fileId != -1)
            {
                ACW.File? file = _conn?.WebServiceManager.DocumentService.GetFileById(fileId);
                _explorerUtil?.GoToEntity(new VDFV.Currency.Entities.FileIteration(_conn, file));
            }
        }

        internal void GotoVaultItem(string[] parameters)
        {
            _navigationSource = parameters[0];
            ACW.Item? item = null;

            if (_navigationSource == "file")
            {
                //get the mItem of the file
                try
                {
                    item = _conn?.WebServiceManager.ItemService.GetItemsByFileIdAndLinkTypeOptions(long.Parse(parameters[1]), ACW.ItemFileLnkTypOpt.Primary).FirstOrDefault();
                    item = _conn?.WebServiceManager.ItemService.GetItemsByFileId(long.Parse(parameters[1])).FirstOrDefault();
                    _explorerUtil?.GoToEntity(new VDFV.Currency.Entities.ItemRevision(_conn, item));
                    return;
                }
                catch (Exception)
                {
                    // todo: Vault error message;
                }
            }

            // navigate to the mItem using itemrevision
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

            // navigate to the mItem using plm mItem number
            if (_navigationSource == "plm-item")
            {
                // try the direct path if the itemrevision is defined
                if (parameters[1] != "undefined")
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
                else
                {
                    try
                    {
                        item = _conn?.WebServiceManager.ItemService.GetLatestItemByItemNumber(parameters[2].Split(" - ")[0]);
                        _explorerUtil?.GoToEntity(new VDFV.Currency.Entities.ItemRevision(_conn, item));
                        return;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        internal void GotoVaultChangeOrder(string[] parameters)
        {            
            if (!string.IsNullOrEmpty(parameters[1]))
            {
                string changeOrderId = parameters[1];
                ACW.ChangeOrder? changeOrder = _conn?.WebServiceManager.ChangeOrderService.GetChangeOrderByNumber(changeOrderId);
                _explorerUtil?.GoToEntity(new VDFV.Currency.Entities.ChangeOrder(_conn, changeOrder));
            }
            else
            {
            }
        }
    }
}
