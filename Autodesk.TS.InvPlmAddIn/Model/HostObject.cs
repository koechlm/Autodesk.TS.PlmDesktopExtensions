using DevExpress.Data.Filtering.Helpers;
using DevExpress.XtraEditors;
using InvPlmAddIn.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InvPlmAddIn.Model
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class HostObject
    {
        public HostObject(BrowserPanelWindowManager panelManager)
        {
            ApplicationPanelSet = panelManager;
        }

        public HostObject(DocumentPanelSet documentPanelSet)
        {
            DocumentPanelSet = documentPanelSet;
        }

        public bool IsChanged { get; set; }

        private DocumentPanelSet DocumentPanelSet { get; set; }

        private BrowserPanelWindowManager ApplicationPanelSet { get; set; }


        public class mVaultEntity
        {
            // valid values: "file", "item", "plm-item"        
            public string entityType { get; set; }

            // values represent per type: file = iterationId, item = ItemRevision, plm-item = NUMBER
            public string id { get; set; }

            // values represent per type: file = fileName (includes extension), item = Number, plm-item = linked Vault file name (including extension)
            public string name { get; set; }

            // values represent per type: file = folderId, item = "", plm-item = LOCATION (folder path of linked file)
            public string parentFolder { get; set; }
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
            //dic = mCastToDicOfVaultEntities(numbers);

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

        public async Task addComponents(object[] numbers)
        {
            //show wait form and wait cursor
            Forms.WaitForm1 waitForm = new Forms.WaitForm1(InvPlmAddinSrv.mTheme, "Adding component...");
            waitForm.Show();
            Cursor.Current = Cursors.WaitCursor;        

            var EntityIds = new Dictionary<string, mVaultEntity>();
            EntityIds = mCastToDicOfVaultEntities(numbers);

            //download files from Vault
            List<string> mDownloadedFiles = VaultUtils.mDownloadRelatedFiles(EntityIds);

            //todo: check and feedback to user if the download was successful


            //hand over file list (successfully downloaded files) to iLogic for insertion
            var dic = new Dictionary<string, object>()
            {
                ["DownloadedFiles"] = mDownloadedFiles
            };

            CallILogic("AddComponents", ref dic);
            await Task.CompletedTask;

            Cursor.Current = Cursors.Default;
            waitForm.Close();
            waitForm.Dispose();
        }

        public async Task openComponents(object[] numbers)
        {
            //show wait form and wait cursor
            Forms.WaitForm1 waitForm = new Forms.WaitForm1(InvPlmAddinSrv.mTheme, "Opening component...");
            
            waitForm.Show();
            Cursor.Current = Cursors.WaitCursor;

            var EntityIds = new Dictionary<string, mVaultEntity>();
            EntityIds = mCastToDicOfVaultEntities(numbers);

            //download files from Vault
            List<string> mDownloadedFiles = VaultUtils.mDownloadRelatedFiles(EntityIds);

            //todo: check and feedback to user if the download was successful


            //hand over file list (successfully downloaded files) to iLogic for insertion
            var dic = new Dictionary<string, object>()
            {
                ["DownloadedFiles"] = mDownloadedFiles
            };

            CallILogic("OpenComponent", ref dic);
            await Task.CompletedTask;

            Cursor.Current = Cursors.Default;
            waitForm.Close();
            waitForm.Dispose();
        }

        public async Task selectComponents(object[] numbers)
        {
            BrowserPanelWindowManager.mSelectionSender = "PLM";

            var EntityIds = new Dictionary<string, mVaultEntity>();
            EntityIds = mCastToDicOfVaultEntities(numbers);

            //get instance names (=file names, not extension) from Vault using mVaultEntity.entityType
            List<string> mPartNumbers = VaultUtils.mGetPartNumbers(EntityIds);

            var dic = new Dictionary<string, object>
            {
                ["PartNumbers"] = mPartNumbers.ToArray()
            };

            CallILogic("SelectComponents", ref dic);
            await Task.CompletedTask;

            BrowserPanelWindowManager.mSelectionSender = "Inventor";
        }

        public async Task isolateComponents(object[] numbers)
        {
            BrowserPanelWindowManager.mSelectionSender = "PLM";

            var EntityIds = new Dictionary<string, mVaultEntity>();
            EntityIds = mCastToDicOfVaultEntities(numbers);

            //get instance names (=file names, not extension) from Vault using mVaultEntity.entityType
            List<string> mPartNumbers = VaultUtils.mGetPartNumbers(EntityIds);

            var dic = new Dictionary<string, object>
            {
                ["PartNumbers"] = mPartNumbers.ToArray()
            };

            CallILogic("IsolateComponents", ref dic);
            await Task.CompletedTask;

            BrowserPanelWindowManager.mSelectionSender = "Inventor";
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

        private void CallILogic(string rule, ref Dictionary<string, object> dic)
        {
            (DocumentPanelSet?.PanelManager ?? ApplicationPanelSet)?.CallILogic(rule, ref dic);
        }

        private string[] mCastObjectToStringArray(object obj)
            => (obj as object[])?.Select(x => x.ToString()).ToArray();


        private Dictionary<string, mVaultEntity> mCastToDicOfVaultEntities(object obj)
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
                            parentFolder = mItems[3]
                        };

                        // Ensure all properties are not null before adding to the list
                        if (number.entityType != null && number.id != null && number.name != null && number.parentFolder != null)
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

    }
}
