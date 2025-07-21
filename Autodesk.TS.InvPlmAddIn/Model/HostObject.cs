using DevExpress.CodeParser;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Preview;
using Inventor;
using InvPlmAddIn.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ACW = Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.Explorer.Extensibility;
using Autodesk.Connectivity.Explorer.ExtensibilityTools;
using VDF = Autodesk.DataManagement.Client.Framework;
using VDFV = Autodesk.DataManagement.Client.Framework.Vault;
using VltBase = Connectivity.Application.VaultBase;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;


namespace InvPlmAddIn.Model
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class HostObject
    {

        private static VDF.Vault.Currency.Connections.Connection _conn = null;
        private static IExplorerUtil _explorerUtil = null;
        private static string _navigationSource = null;

        public HostObject(BrowserPanelWindowManager panelManager)
        {
            ApplicationPanelSet = panelManager;

            _conn = VltBase.ConnectionManager.Instance.Connection;
            _explorerUtil = ExplorerLoader.LoadExplorerUtil(_conn.Server, _conn.Vault, _conn.UserID, _conn.Ticket);
        }

        public HostObject(DocumentPanelSet documentPanelSet = null)
        {
            DocumentPanelSet = documentPanelSet;
        }

        public bool IsChanged { get; set; }

        private static DocumentPanelSet DocumentPanelSet { get; set; }

        private static BrowserPanelWindowManager ApplicationPanelSet { get; set; }

        private static InvPlmAddIn.Forms.WaitForm1 mWaitForm;

        public class mVaultEntity
        {
            // valid values: "file", "item", "plm-item"        
            public string entityType { get; set; }

            // values represent per type: file = File.Id (Iteration), item = ItemRev.Id, plm-item = NUMBER
            public string id { get; set; }

            // values represent per type: file = fileName (includes extension), item = Number, plm-item = item descriptor + [Rev]
            public string name { get; set; }

            // values represent per type: file = File.MasterId, item = "", plm-item = urn
            public string masterId { get; set; }
        }


        /// <summary>
        /// PLM call to query active document; optionally check against contextId
        /// </summary>
        /// <param name="contextId"></param>
        /// <returns>Part Number of Inventor.ActiveDocument</returns>
        public async Task<string> getActiveDocument(string contextId)
        {
            var result = new List<string>();
            var dic = new Dictionary<string, object>
            {
                ["contextId"] = contextId
            };

            CallILogic("getActiveDocument", ref dic);
            await Task.FromResult(dic);

            object iLogicResult;
            if (dic.TryGetValue("Result", out iLogicResult) == true)
            {
                return iLogicResult?.ToString();
            }
            else
            {
                return result.ToString();
            }
        }

        public async Task<string> getComponentsLocked(object[] numbers)
        {
            var result = new List<string>();

            var dic = new Dictionary<string, object>();
            //dic = mCastToDicOfVaultEntities(parameters);

            CallILogic("GetComponentsLocked", ref dic);
            await Task.FromResult(dic);

            object iLogicResult;
            if (dic.TryGetValue("Result", out iLogicResult) == true)
            {
                return System.Text.Json.JsonSerializer.Serialize((List<string>)iLogicResult).ToString();
            }
            else
            {
                return result.ToString();
            }
        }

        public static async Task addComponent(string[] parameters)
        {
            //show wait form and wait cursor
            mWaitForm = new Forms.WaitForm1(InvPlmAddinSrv.mTheme, "Downloading Component(s)...");
            mWaitForm.Show();
            Cursor.Current = Cursors.WaitCursor;

            var EntityIds = new Dictionary<string, mVaultEntity>();
            // legacy: parameters had been an array of Part Numbers //EntityIds = mCastToDicOfVaultEntities(parameters);
            EntityIds.Add("0", mCastToVaultEntity(parameters));

            //download files from Vault
            List<string> mDownloadedFiles = new List<string>();
            mDownloadedFiles = VaultUtils.mDownloadRelatedFiles(EntityIds);

            // check and feedback to user if the download was successful
            if (mDownloadedFiles.Count == 0)
            {
                AdskTsVaultUtils.Messages.ShowError("No files were downloaded.", InvPlmAddinSrv.AddInName);

                mWaitForm.Close();
                mWaitForm.Dispose();

                return;
            }

            //hand over file list (successfully downloaded files) to iLogic for insertion
            var dic = new Dictionary<string, object>()
            {
                ["DownloadedFiles"] = mDownloadedFiles
            };

            mWaitForm.progressPanel1.Description = "Adding Component...";
            CallILogic("AddComponents", ref dic);
            await Task.CompletedTask;

            Cursor.Current = Cursors.Default;
            mWaitForm.Close();
            mWaitForm.Dispose();
        }

        public static async Task openComponent(string[] parameters)
        {
            // Initialize the progress form
            mWaitForm = new Forms.WaitForm1(InvPlmAddinSrv.mTheme, "Downloading Component(s)...");
            mWaitForm.Show();
            Cursor.Current = Cursors.WaitCursor;

            var EntityIds = new Dictionary<string, mVaultEntity>();
            // legacy: parameters had been an array of Part Numbers //EntityIds = mCastToDicOfVaultEntities(parameters);
            EntityIds.Add("0", mCastToVaultEntity(parameters));

            // download files from Vault
            List<string> mDownloadedFiles = new List<string>();
            mDownloadedFiles = VaultUtils.mDownloadRelatedFiles(EntityIds);

            // check and feedback to user if the download was successful
            if (mDownloadedFiles.Count == 0)
            {
                AdskTsVaultUtils.Messages.ShowError("No files were downloaded.", InvPlmAddinSrv.AddInName);

                mWaitForm.Close();
                mWaitForm.Dispose();

                return;
            }

            //hand over file list (successfully downloaded files) to iLogic for insertion
            var dic = new Dictionary<string, object>()
            {
                ["DownloadedFiles"] = mDownloadedFiles
            };

            mWaitForm.progressPanel1.Description = "Opening...";
            CallILogic("OpenComponent", ref dic);
            await Task.CompletedTask;

            Cursor.Current = Cursors.Default;
            mWaitForm.Close();
            mWaitForm.Dispose();
        }

        public static async Task selectComponent(string[] parameters)
        {
            BrowserPanelWindowManager.mSelectionSender = "PLM";

            var EntityIds = new Dictionary<string, mVaultEntity>();
            EntityIds.Add("0", mCastToVaultEntity(parameters));

            //get instance names (=file names, not extension) from Vault using mVaultEntity.entityType
            List<string> mPartNumbers = new List<string>();
            mPartNumbers = VaultUtils.mGetPartNumbers(EntityIds);

            if (mPartNumbers.Count != 0)
            {
                var dic = new Dictionary<string, object>
                {
                    ["PartNumbers"] = mPartNumbers.ToArray()
                };

                CallILogic("SelectComponents", ref dic);
                await Task.CompletedTask;
            }

            BrowserPanelWindowManager.mSelectionSender = "Inventor";
        }

        public static async Task isolateComponent(string[] parameters)
        {
            BrowserPanelWindowManager.mSelectionSender = "PLM";

            var EntityIds = new Dictionary<string, mVaultEntity>();
            EntityIds.Add("0", mCastToVaultEntity(parameters));

            //get instance names (=file names, not extension) from Vault using mVaultEntity.entityType
            List<string> mPartNumbers = new List<string>();
            mPartNumbers = VaultUtils.mGetPartNumbers(EntityIds);

            if (mPartNumbers.Count != 0)
            {

                var dic = new Dictionary<string, object>
                {
                    ["PartNumbers"] = mPartNumbers.ToArray()
                };

                CallILogic("IsolateComponents", ref dic);
                await Task.CompletedTask;
            }

            BrowserPanelWindowManager.mSelectionSender = "Inventor";
        }

        public static async Task selectInstance(string[] parameters)
        {
            // reserved for future use
            // This method is currently not implemented in the iLogic rule.
            // It can be used to select a specific instance of a component in the assembly.
            // For now, it simply returns without doing anything.
            var dic = new Dictionary<string, object>
            {
                ["Parameters"] = parameters
            };
            //CallILogic("SelectInstance", ref dic);
            await Task.CompletedTask;
        }

        public static async Task isolateInstance(string[] parameters)
        {
            // reserved for future use
            // This method is currently not implemented in the iLogic rule.
            // It can be used to isolate a specific instance of a component in the assembly.
            // For now, it simply returns without doing anything.
            var dic = new Dictionary<string, object>
            {
                ["Parameters"] = parameters
            };
            //CallILogic("IsolateInstance", ref dic);
            await Task.CompletedTask;
        }

        public async Task setLifecycleState(string folderName, string targetStateName)
        {
            var dic = new Dictionary<string, object>
            {
                ["FolderName"] = folderName,
                ["TargetStateName"] = targetStateName
            };

            CallILogic("SetLifecycleState", ref dic);
            await Task.CompletedTask;
        }

        public async Task updateProperties(string updateProperties)
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<UpdateProperties[]>(updateProperties, new System.Text.Json.JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            var dic = new Dictionary<string, object>
            {
                ["UpdateProperties"] = data
            };

            CallILogic("UpdateProperties", ref dic);

            await Task.CompletedTask;
        }

        private static void CallILogic(string rule, ref Dictionary<string, object> dic)
        {
            (DocumentPanelSet?.PanelManager ?? ApplicationPanelSet)?.CallILogic(rule, ref dic);
        }

        private string[] mCastObjectToStringArray(object obj)
            => (obj as object[])?.Select(x => x.ToString()).ToArray();


        private static Dictionary<string, mVaultEntity> mCastToDicOfVaultEntities(object obj)
        {
            Dictionary<string, mVaultEntity> dic = new Dictionary<string, mVaultEntity>();

            if (obj is object[] objArray)
            {
                foreach (string item in objArray)
                {
                    string[] mItems = item.Split(";");
                    try
                    {
                        var number = new mVaultEntity
                        {
                            entityType = mItems[0],
                            id = mItems[1],
                            name = mItems[2],
                            masterId = mItems[3]
                        };

                        // Ensure all properties are not null before adding to the list
                        if (number.entityType != null && number.id != null && number.name != null && number.masterId != null)
                        {
                            dic.Add(number.id, number);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        // For example, you could log the error to a file or output window
                        System.Diagnostics.Debug.WriteLine($"Error processing item: {ex.Message}");
                    }
                }
            }

            return dic;
        }

        /// <summary>
        /// Cast parameters to mVaultEntity objects.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static mVaultEntity mCastToVaultEntity(string[] parameters)
        {
            if (parameters == null)
            {
                return null;
            }

            //parameters are expected to be in the format: "source(entitytype);id;name/number;masterId or URN"

            switch (parameters[0])
            {
                case "file":
                    return new mVaultEntity
                    {
                        entityType = "file",
                        id = parameters[1],
                        name = parameters[2],
                        masterId = parameters[3]
                    };
                case "item":
                    return new mVaultEntity
                    {
                        entityType = "item",
                        id = parameters[1],
                        name = parameters[2],
                        masterId = string.Empty // Item does not have a masterId
                    };
                case "plm-item":
                    return new mVaultEntity
                    {
                        entityType = "plm-item",
                        id = parameters[1],
                        name = parameters[2]
                    };
                default:
                    return null;
            }
        }

        /// <summary>
        /// Build assembly context path, e.g. "Assembly:1 | Subassembly1:5 | Part8:2"
        /// </summary>
        /// <returns>Serialized JSON string of iLogic RuleArgument's list object.</returns>
        public async Task<string> getSelectedComponentPaths(object contextId)
        {
            var result = new List<string>();
            var dic = new Dictionary<string, object>
            {
                ["contextId"] = mCastObjectToStringArray(contextId)
            };

            CallILogic("getSelectedComponentPaths", ref dic);
            await Task.FromResult(dic);

            object iLogicResult;
            if (dic.TryGetValue("Result", out iLogicResult) == true)
            {
                return System.Text.Json.JsonSerializer.Serialize((List<string>)iLogicResult).ToString();
            }
            else
            {
                return result.ToString();
            }
        }

        /// <summary>
        /// PLM call to assign raw material to multi-bodies, or apply Inventor part material
        /// </summary>
        /// <param name="designator"></param>
        /// <returns></returns>
        public async Task assignMaterial(string designator)
        {
            var dic = new Dictionary<string, object>
            {
                ["Designator"] = designator
            };

            CallILogic("assignMaterial", ref dic);
            await Task.CompletedTask;
        }

        internal static void HandleJsMessage(string message)
        {
            // call the task openComponent

            String[]? mMessageArray = message?.ToString()?.Split(":");
            if (mMessageArray?.Length > 1)
            {
                String mCommand = mMessageArray[0];
                String mParameters = mMessageArray[1];
                String[] mParametersArray = mParameters.Split(";");

                switch (mCommand)
                {
                    case "addComponent":
                        _ = addComponent(mParametersArray);
                        break;
                    case "openComponent":
                        _ = openComponent(mParametersArray);
                        break;
                    case "selectComponent":
                        _ = selectComponent(mParametersArray);
                        break;
                    case "isolateComponent":
                        _ = isolateComponent(mParametersArray);
                        break;
                    case "gotoVaultFile":
                        // GoToEntity() targets the Vault client context only
                        break;
                    case "gotoVaultItem":
                        // GoToEntity() targets the Vault client context only
                        break;
                    case "gotoVaultECO":
                        // GoToEntity() targets the Vault client context only
                        break;
                    default:
                        break;
                }
            }
        }

        internal static ACW.File GetFileByParameters(string[] parameters)
        {
            _navigationSource = parameters[0];
            long fileId = -1;
            long fileMasterId = -1;
            ACW.File mFile = null;

            // get the fileId from the parameters; search for the file, if the fileId is not valid
            // the FM Search panel may return Vault Items, Files or Fusion Manage Items
            if (_navigationSource == "item")
            {
                mFile = GetPrimaryFile(parameters[2]);
                mFile = _conn?.WebServiceManager.DocumentService.GetLatestFileByMasterId(mFile.MasterId);
            }
            if (_navigationSource == "plm-item")
            {
                // try the direct path if the itemrevision is defined
                if (parameters[1] != "undefined")
                {
                    ACW.Item mItem = _conn?.WebServiceManager.ItemService.GetLatestItemByItemNumber(parameters[1]);
                    mFile = GetPrimaryFile(mItem.ItemNum);
                    mFile = _conn?.WebServiceManager.DocumentService.GetLatestFileByMasterId(mFile.MasterId);
                }
                else
                {
                    ACW.Item mItem = _conn?.WebServiceManager.ItemService.GetLatestItemByItemNumber(parameters[2].Split(" - ")[0]);
                    mFile = GetPrimaryFile(mItem.ItemNum);
                    mFile = _conn?.WebServiceManager.DocumentService.GetLatestFileByMasterId(mFile.MasterId);
                }
            }
            if (_navigationSource == "file")
            {
                fileId = long.Parse(parameters[1]);
                fileMasterId = long.Parse(parameters[3]);
                if (fileId != -1)
                {
                    try
                    {
                        mFile = _conn?.WebServiceManager.DocumentService.GetFileById(fileId);
                        if (mFile == null)
                        {
                            // try to get the latest file by masterId
                            mFile = _conn?.WebServiceManager.DocumentService.GetLatestFileByMasterId(mFile.MasterId);
                        }
                        return mFile;
                    }
                    catch (Exception)
                    {
                        if (fileMasterId != -1)
                        {
                            try
                            {
                                mFile = _conn?.WebServiceManager.DocumentService.GetLatestFileByMasterId(fileMasterId);
                                return mFile;
                            }
                            catch (Exception)
                            {
                                return null;
                            }
                        }
                    }
                }
                return null;
            }
            return mFile;
        }

        internal static ACW.File GetPrimaryFile(string ItemNumber)
        {
            ACW.Item mItem = null;
            ACW.File mFile = null;
            mItem = _conn.WebServiceManager.ItemService.GetLatestItemByItemNumber(ItemNumber);
            if (mItem != null)
            {
                ACW.ItemFileAssoc[] itemFileAssocs = _conn?.WebServiceManager.ItemService.GetItemFileAssociationsByItemIds(new long[] { mItem.Id }, ACW.ItemFileLnkTypOpt.Primary);
                if (itemFileAssocs != null && itemFileAssocs.Any())
                {
                    long fileId = itemFileAssocs.First().CldFileId;
                    mFile = _conn?.WebServiceManager.DocumentService.GetFileById(fileId);
                    return mFile;
                }

                // todo: Vault error message no primary file
                return null;
            }
            else
            {
                // todo: Vault error message no item ;
                return null;
            }
        }
    }
}