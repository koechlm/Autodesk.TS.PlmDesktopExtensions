using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACW = Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.Explorer.Extensibility;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using VDF = Autodesk.DataManagement.Client.Framework;
using VDFV = Autodesk.DataManagement.Client.Framework.Vault;
using Autodesk.Connectivity.Explorer.ExtensibilityTools;
using System.Windows.Forms;
using Autodesk.Connectivity.WebServices;
using DevExpress.XtraRichEdit.Model;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Properties;
using System.Threading;

namespace Autodesk.TS.VltPlmAddIn.Model
{
    internal class Navigation
    {
        private static VDF.Vault.Currency.Connections.Connection _conn;
        private IApplication _application;
        private IExplorerUtil _explorerUtil;

        private string _navigationSource = null;

        private static Inventor.Application mInv = null;

        public Navigation()
        {
            _conn = VaultExplorerExtension.conn;
            _application = VaultExplorerExtension.mApplication;
            _explorerUtil = Autodesk.Connectivity.Explorer.ExtensibilityTools.ExplorerLoader.GetExplorerUtil(_application);
        }

        internal void GotoVaultFile(string[] parameters)
        {
            ACW.File mFile = null;

            // get the fileId from the parameters; search for the file, if the fileId is not valid
            mFile = GetFileByParameters(parameters);

            // finally navigate to the file, goto navigation targets the main view, mFile should be the tip version
            if (mFile != null)
            {
                _explorerUtil?.GoToEntity(new VDFV.Currency.Entities.FileIteration(_conn, mFile));
            }
        }

        internal void GotoVaultItem(string[] parameters)
        {
            _navigationSource = parameters[0];
            ACW.Item item = null;

            if (_navigationSource == "file")
            {
                //get the mItem of the file
                try
                {
                    //item = _conn?.WebServiceManager.ItemService.GetItemsByFileIdAndLinkTypeOptions(long.Parse(parameters[1]), ACW.ItemFileLnkTypOpt.Primary).FirstOrDefault();
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
                    item = _conn?.WebServiceManager.ItemService.GetLatestItemByItemNumber(parameters[2]);
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
                try
                {
                    ACW.ChangeOrder changeOrder = _conn?.WebServiceManager.ChangeOrderService.GetChangeOrderByNumber(changeOrderId);
                    _explorerUtil?.GoToEntity(new VDFV.Currency.Entities.ChangeOrder(_conn, changeOrder));
                }
                catch (Exception)
                {
                    VDF.Forms.Library.ShowError("Unable to navigate to the Change Order. Please validate the Change Order ID (parameter).", "Vault PLM Extension");
                }

            }
            else
            {
                VDF.Forms.Library.ShowError("Unable to navigate to the Change Order. The Change Order ID (parameter) is empty.", "Vault PLM Extension");
            }
        }

        internal void addComponent(string[] parameters)
        {
            _navigationSource = parameters[0];
            string mFileName = null;

            // get the file object as identified by the parameters
            ACW.File mFile = GetFileByParameters(parameters);

            if (mFile == null)
            {
                VDF.Forms.Library.ShowError("Could not find the expected file.", "Vault PLM Extension");
                return;
            }

            // only Inventor file types are valid for this command
            mFileName = mFile.Name;
            string[] mValidExt = { "ipt", "iam" };
            if (!mValidExt.Contains(mFileName.Split(".").LastOrDefault().ToLower()))
            {
                VDF.Forms.Library.ShowWarning("This command applies to Inventor files only.", "Vault PLM Extension", VDF.Forms.Currency.ButtonConfiguration.Ok);
                return;
            }

            // try to get a running Inventor instance, exit with warning if none exists            
            if (mInv == null)
            {
                mInv = Utils.MarshalCore.GetActiveObject("Inventor.Application") as Inventor.Application;

                if (mInv == null)
                {
                    VDF.Forms.Library.ShowWarning("This command requires a running Inventor.", "Vault PLM Extension", VDF.Forms.Currency.ButtonConfiguration.Ok);
                    return;
                }
            }

            // show a progress dialog
            Autodesk.TS.VltPlmAddIn.Utils.ProgressForm mProgressForm = new Autodesk.TS.VltPlmAddIn.Utils.ProgressForm("Downloading file...");
            mProgressForm.Show();

            // download the file identified from the parameters
            string mFullCompFileName = null;
            mFullCompFileName = DownloadFiles(new List<VDF.Vault.Currency.Entities.FileIteration> { new VDF.Vault.Currency.Entities.FileIteration(_conn, mFile) })?.FirstOrDefault();

            // exit with warning if the download failed
            if (mFullCompFileName == null)
            {
                VDF.Forms.Library.ShowError("Could not download the file from Vault.", "Vault PLM Extension");
                return;
            }
            mProgressForm.CloseProgress();
            mProgressForm.Close();
            mProgressForm = null;

            // call the Inventor command - it handles the different parent file behaviors for assemblies, parts, presentation, and drawing
            if (mInv != null && mFullCompFileName != null)
            {
                Utils.InvHelpers.m_PlaceComponent(mInv, mFullCompFileName);
            }
        }

        internal void openComponent(string[] parameters)
        {
            _navigationSource = parameters[0];
            string mFileName = null;

            // get the file object as identified by the parameters
            ACW.File mFile = GetFileByParameters(parameters);

            if (mFile == null)
            {
                VDF.Forms.Library.ShowError("Could not find the expected file.", "Vault PLM Extension");
                return;
            }

            // only Inventor file types are valid for this command
            mFileName = mFile.Name;
            string[] mValidExt = { "ipt", "iam", "ipn", "idw", "dwg" };
            if (!mValidExt.Contains(mFileName.Split(".").LastOrDefault().ToLower()))
            {
                VDF.Forms.Library.ShowWarning("This command applies to Inventor files only.", "Vault PLM Extension", VDF.Forms.Currency.ButtonConfiguration.Ok);
                return;
            }

            // try to get a running Inventor instance, exit with warning if none exists            
            if (mInv == null)
            {
                mInv = Utils.MarshalCore.GetActiveObject("Inventor.Application") as Inventor.Application;

                if (mInv == null)
                {
                    VDF.Forms.Library.ShowWarning("This command requires a running Inventor.", "Vault PLM Extension", VDF.Forms.Currency.ButtonConfiguration.Ok);
                    return;
                }
            }

            // show a progress dialog
            Autodesk.TS.VltPlmAddIn.Utils.ProgressForm mProgressForm = new Autodesk.TS.VltPlmAddIn.Utils.ProgressForm("Downloading file...");
            mProgressForm.Show();

            // download the file identified from the parameters
            string mFullCompFileName = null;
            mFullCompFileName = DownloadFiles(new List<VDF.Vault.Currency.Entities.FileIteration> { new VDF.Vault.Currency.Entities.FileIteration(_conn, mFile) })?.FirstOrDefault();

            // exit with warning if the download failed
            if (mFullCompFileName == null)
            {
                VDF.Forms.Library.ShowError("Could not download the file from Vault.", "Vault PLM Extension");
                return;
            }
            mProgressForm.CloseProgress();
            mProgressForm.Close();
            mProgressForm = null;

            // open the file in Inventor
            mProgressForm = new Autodesk.TS.VltPlmAddIn.Utils.ProgressForm("Opening file in Inventor...");
            mProgressForm.Show();
            Inventor.Document mDoc = null; // reset the document to avoid issues with multiple open documents
            try
            {
                mDoc = mInv.Documents.Open(mFullCompFileName, true);
                if (mDoc == null)
                {
                    VDF.Forms.Library.ShowError("Could not open the file in Inventor.", "Vault PLM Extension");
                    return;
                }
                else
                {
                    // switch to Inventor
                    IntPtr mWinPt = (IntPtr)mInv.MainFrameHWND;
                    Utils.InvHelpers.SwitchToThisWindow(mWinPt, true);
                }
            }
            catch (Exception ex)
            {
                VDF.Forms.Library.ShowError("Inventor failed to open the dialog." + "-" + ex.Message, "Vault PLM Extension");
            }

            // close the progress dialog
            mProgressForm.CloseProgress();
            mProgressForm.Close();
            mProgressForm = null;
        }

        internal File GetFileByParameters(string[] parameters)
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
                        return null;
                    }
                }
                return null;
            }

            return mFile;
        }

        internal File GetPrimaryFile(string ItemNumber)
        {
            Item mItem = null;
            File mFile = null;
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

        internal static List<string> DownloadFiles(List<VDF.Vault.Currency.Entities.FileIteration> mVaultFiles)
        {
            List<String> mFilesDownloaded = new List<string>();

            foreach (VDF.Vault.Currency.Entities.FileIteration mFileIt in mVaultFiles)
            {
                //create download settings and options
                VDF.Vault.Settings.AcquireFilesSettings settings = CreateAcquireSettings(false);
                settings.AddFileToAcquire(mFileIt, settings.DefaultAcquisitionOption);

                //download
                VDF.Vault.Results.AcquireFilesResults results = _conn.FileManager.AcquireFiles(settings);

                //capture primary file name for return (download may include children and attachments)
                if (results.FileResults != null)
                {
                    if (results.FileResults.Any(n => n.File.EntityName == mFileIt.EntityName))
                    {
                        mFilesDownloaded.Add(_conn.WorkingFoldersManager.GetPathOfFileInWorkingFolder(mFileIt).FullPath.ToString());
                    }
                }
                //the download cancelled if the file already exists in the working folder
                if (results.IsCancelled == true)
                {
                    PropertyDefinitionDictionary mProps = _conn.PropertyManager.GetPropertyDefinitions(VDF.Vault.Currency.Entities.EntityClassIds.Files, null, PropertyDefinitionFilter.IncludeAll);

                    PropertyDefinition mVaultStatus = mProps[PropertyDefinitionIds.Client.VaultStatus];

                    EntityStatusImageInfo mStatus = _conn.PropertyManager.GetPropertyValue(mFileIt, mVaultStatus, null) as EntityStatusImageInfo;
                    if (mStatus.Status.ConsumableState == EntityStatus.ConsumableStateEnum.LatestConsumable)
                    {
                        mFilesDownloaded.Add(_conn.WorkingFoldersManager.GetPathOfFileInWorkingFolder(mFileIt).FullPath.ToString());
                    }
                }
            }

            //return the files
            if (mFilesDownloaded.Count > 0)
            {
                return mFilesDownloaded;
            }
            else
            {
                return null;
            }
        }

        private static VDF.Vault.Settings.AcquireFilesSettings CreateAcquireSettings(bool CheckOut = false)
        {
            VDF.Vault.Settings.AcquireFilesSettings settings = new VDF.Vault.Settings.AcquireFilesSettings(_conn);
            if (CheckOut)
            {
                settings.DefaultAcquisitionOption = VDF.Vault.Settings.AcquireFilesSettings.AcquisitionOption.Checkout;
            }
            else
            {
                settings.DefaultAcquisitionOption = VDF.Vault.Settings.AcquireFilesSettings.AcquisitionOption.Download;
                settings.OptionsRelationshipGathering.FileRelationshipSettings.IncludeChildren = true;
                settings.OptionsRelationshipGathering.FileRelationshipSettings.RecurseChildren = true;
                settings.OptionsRelationshipGathering.FileRelationshipSettings.IncludeAttachments = false;
                settings.OptionsRelationshipGathering.FileRelationshipSettings.IncludeLibraryContents = true;
                settings.OptionsRelationshipGathering.FileRelationshipSettings.ReleaseBiased = false;
                settings.OptionsRelationshipGathering.FileRelationshipSettings.VersionGatheringOption = VDF.Vault.Currency.VersionGatheringOption.Actual;
                settings.OptionsRelationshipGathering.IncludeLinksSettings.IncludeLinks = false;
                //download options => don't overwrite, sync with remote site
                VDF.Vault.Settings.AcquireFilesSettings.AcquireFileResolutionOptions mResOpt = new VDF.Vault.Settings.AcquireFilesSettings.AcquireFileResolutionOptions();
                mResOpt.OverwriteOption = VDF.Vault.Settings.AcquireFilesSettings.AcquireFileResolutionOptions.OverwriteOptions.NoOverwrite;
                mResOpt.SyncWithRemoteSiteSetting = VDF.Vault.Settings.AcquireFilesSettings.SyncWithRemoteSite.Always;
            }

            return settings;
        }
    }
}
