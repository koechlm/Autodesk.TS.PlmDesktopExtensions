using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACET = Autodesk.Connectivity.Explorer.ExtensibilityTools;
using ACW = Autodesk.Connectivity.WebServices;
using VDF = Autodesk.DataManagement.Client.Framework;
using VltBase = Connectivity.Application.VaultBase;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Properties;
using InvPlmAddIn.Model;
using System.ServiceModel.Syndication;
using Autodesk.Connectivity.WebServices;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using Autodesk.Connectivity.WebServicesTools;

namespace InvPlmAddIn.Utils
{
    /// <summary>
    /// Vault utility class, wrapping multiple API calls to commonly used methods.
    /// Files: Download, upload, check in, check out, etc.
    /// Items: Download primary linked files, etc.
    /// Search: Search for files, items, etc.
    /// Note - This class derived and extended the tech preview of iLogic-Vault integration.
    /// </summary>
    public class VaultUtils : IDisposable
    {
        public void Dispose()
        {
            mFilePropDefs = null;
            mItemPropDefs = null;
            mWsMgr = null;
            conn = null;
            mSettings = null;
        }

        //read addin settings
        private static Settings mSettings = Settings.Load();
        private static List<string> mAddOpenSpprtdExts = mSettings.AddOpenSpprtdExts.ToString().ToLower().Split(",").ToList<string>();

        private static VDF.Vault.Currency.Connections.Connection conn = VltBase.ConnectionManager.Instance.Connection;
        private static Autodesk.Connectivity.WebServicesTools.WebServiceManager mWsMgr = conn.WebServiceManager;

        /// <summary>
        /// Some methods are not applicable to Vault Basic; we need to know the environment
        /// </summary>
        private static readonly ACW.Product[] mProducts = VltBase.ConnectionManager.Instance.Connection.WebServiceManager.InformationService.GetSupportedProducts();

        private readonly bool IsVaultPro = mIsVaultPro;

        private readonly bool IsVaultBasic = mIsVaultBasic;

        /// <summary>
        /// Indicates Vault Basic environment
        /// </summary>
        private static bool mIsVaultBasic
        {
            get
            {
                //Vault Basic Servers return only a single product, whereas Vault Pro returns 4
                if (mProducts.Length == 1)
                {
                    return true;
                }
                return false;
            }
        }

        private static bool mIsVaultPro
        {
            get
            {
                //Vault Pro return 4 products
                if (mProducts.Length == 4)
                {
                    return true;
                }
                return false;
            }
        }

        //avoid multiple server calls for the iLogic-Vault session
        private static ACW.PropDef[] mFilePropDefs = mWsMgr.PropertyService.GetPropertyDefinitionsByEntityClassId("FILE");
        private static ACW.PropDef[] mItemPropDefs = mWsMgr.PropertyService.GetPropertyDefinitionsByEntityClassId("ITEM");

        internal static bool LoggedIn
        {
            get
            {
                if (conn != null)
                {
                    return true;
                }
                return false;
            }
        }

        internal static WebServiceManager GetConnection()
        {
            WebServiceManager webServiceManager = null;
            conn = VltBase.ConnectionManager.Instance.Connection;
            if (conn != null)
            {
                webServiceManager = conn.WebServiceManager;
                return webServiceManager;
            }
            else
            {
                AdskTsVaultUtils.Messages.ShowWarning("The AddIn could get a Vault connection. \n Are you logged in to Vault?", InvPlmAddinSrv.AddInName);
                return null;
            }
        }

        private static void mRenewConnection()
        {
            if (conn == null || conn?.Ticket == "" || mWsMgr == null)
            {
                mWsMgr = GetConnection();
                if (mWsMgr == null)
                {
                    AdskTsVaultUtils.Messages.ShowWarning("The AddIn could not reuse the Vault connection. \n Unload/Load the Inventor Vault plm Addin and try again.", InvPlmAddinSrv.AddInName);
                }
            }
        }

        /// <summary>
        /// extract files to be downloaded and return list of successfully downloaded files
        /// </summary>
        /// <param name="mVaultEntities"></param>
        /// <returns></returns>
        internal static List<string> mDownloadRelatedFiles(Dictionary<string, HostObject.mVaultEntity> mVaultEntities)
        {
            mRenewConnection();

            List<VDF.Vault.Currency.Entities.FileIteration> mVaultFiles = new List<VDF.Vault.Currency.Entities.FileIteration>();
            List<string> mDownloadedFiles = new List<string>();
            ACW.File mFile = null;
            long mFileId = -1, mItemId;
            ACW.Item mItem = null;

            //get files from Vault entities
            foreach (HostObject.mVaultEntity item in mVaultEntities.Values)
            {
                //get file full name from file id
                if (item.entityType == "file")
                {
                    mFileId = mParse(item.id);
                    if (mFileId != -1)
                    {
                        mFile = mWsMgr.DocumentService.GetFileById(mFileId);
                    }
                    if (mFile != null && mIsFileTypeSupported(mFile))
                    {
                        //get the latest version of the file
                        mFile = mWsMgr.DocumentService.GetLatestFileByMasterId(mFile.MasterId);
                        VDF.Vault.Currency.Entities.FileIteration mFileIt = new VDF.Vault.Currency.Entities.FileIteration(conn, mFile);
                        mVaultFiles.Add(mFileIt);
                    }
                }
                //get files from item => get file by ItemRevision's primary linked file iteration'
                if (item.entityType == "item")
                {
                    mItemId = mParse(item.id);
                    if (mItemId != -1)
                    {
                        mItem = mWsMgr.ItemService.GetItemsByIds(new long[] { mItemId }).FirstOrDefault();
                        if (mItem != null)
                        {
                            ACW.ItemFileAssoc itemFileAssoc = mWsMgr.ItemService.GetItemFileAssociationsByItemIds(new long[] { mItem.Id }, ItemFileLnkTypOpt.Primary).FirstOrDefault();
                            if (itemFileAssoc?.Cloaked == false)
                            {
                                mFile = mWsMgr.DocumentService.GetFileById(itemFileAssoc.CldFileId);
                                if (mFile != null && mIsFileTypeSupported(mFile))
                                {
                                    //get the latest version of the file
                                    mFile = mWsMgr.DocumentService.GetLatestFileByMasterId(mFile.MasterId);
                                    VDF.Vault.Currency.Entities.FileIteration mFileIt = new VDF.Vault.Currency.Entities.FileIteration(conn, mFile);
                                    mVaultFiles.Add(mFileIt);
                                }
                            }
                        }
                    }
                }
                //get files from plm-item => get file by full file name (Vault path + file name)
                if (item.entityType == "plm-item")
                {
                    mFile = mWsMgr.DocumentService.FindLatestFilesByPaths(new string[] { item.parentFolder + "/" + item.name }).FirstOrDefault();
                    if (mFile != null && mIsFileTypeSupported(mFile))
                    {
                        VDF.Vault.Currency.Entities.FileIteration mFileIt = new VDF.Vault.Currency.Entities.FileIteration(conn, mFile);
                        mVaultFiles.Add(mFileIt);
                    }
                }
            }

            //download files
            mDownloadedFiles = DownloadFiles(mVaultFiles);

            return mDownloadedFiles;
        }

        internal static List<string> mGetPartNumbers(Dictionary<string, HostObject.mVaultEntity> mVaultEntities)
        {
            mRenewConnection();

            List<string> mPartNumbers = new List<string>();
            string mPartNumber = "";
            ACW.PropDef mPropDef = mFilePropDefs.FirstOrDefault(p => p.SysName == "PartNumber");
            ACW.File mFile = null;
            long mFileId = -1, mItemId;
            ACW.Item mItem = null;

            //get Part Number values of Vault entities
            foreach (HostObject.mVaultEntity item in mVaultEntities.Values)
            {
                //get from file
                if (item.entityType == "file")
                {
                    mFileId = mParse(item.id);
                    if (mFileId != -1)
                    {
                        mPartNumber = mWsMgr.PropertyService.GetProperties("FILE", new long[] { mFileId }, new long[] { mPropDef.Id }).FirstOrDefault().Val.ToString();
                    }
                    if (mPartNumber.IsNullOrEmpty() == false)
                    {
                        mPartNumbers.Add(mPartNumber);
                    }
                }
                //get from file by ItemRevision's primary linked file iteration'
                if (item.entityType == "item")
                {
                    mItemId = mParse(item.id);
                    if (mItemId != -1)
                    {
                        mItem = mWsMgr.ItemService.GetItemsByIds(new long[] { mItemId }).FirstOrDefault();
                        if (mItem != null)
                        {
                            ACW.ItemFileAssoc itemFileAssoc = mWsMgr.ItemService.GetItemFileAssociationsByItemIds(new long[] { mItem.Id }, ItemFileLnkTypOpt.Primary).FirstOrDefault();
                            if (itemFileAssoc?.Cloaked == false)
                            {
                                mFile = mWsMgr.DocumentService.GetFileById(itemFileAssoc.CldFileId);
                                if (mFile != null)
                                {
                                    mPartNumber = mWsMgr.PropertyService.GetProperties("FILE", new long[] { mFile.Id }, new long[] { mPropDef.Id }).FirstOrDefault().Val.ToString();
                                    if (mPartNumber.IsNullOrEmpty() == false)
                                    {
                                        mPartNumbers.Add(mPartNumber);
                                    }
                                }
                            }
                        }
                    }
                }
                //get file by full file name (Vault location + file name are PLM fields)
                if (item.entityType == "plm-item")
                {
                    mFile = mWsMgr.DocumentService.FindLatestFilesByPaths(new string[] { item.parentFolder + "/" + item.name }).FirstOrDefault();
                    if (mFile != null)
                    {
                        mPartNumber = mWsMgr.PropertyService.GetProperties("FILE", new long[] { mFile.Id }, new long[] { mPropDef.Id }).FirstOrDefault().Val.ToString();
                        if (mPartNumber.IsNullOrEmpty() == false)
                        {
                            mPartNumbers.Add(mPartNumber);
                        }
                    }
                }
            }

            return mPartNumbers;
        }

        private static bool mIsFileTypeSupported(ACW.File mFile)
        {
            string mFileExt = mFile.Name.Split(".").LastOrDefault().ToLower().Trim();
            if (mAddOpenSpprtdExts.Any(s => s.Trim().Equals(mFileExt, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            return false;
        }

        private static List<string> DownloadFiles(List<VDF.Vault.Currency.Entities.FileIteration> mVaultFiles)
        {
            List<String> mFilesDownloaded = new List<string>();

            foreach (VDF.Vault.Currency.Entities.FileIteration mFileIt in mVaultFiles)
            {
                //create download settings and options
                VDF.Vault.Settings.AcquireFilesSettings settings = CreateAcquireSettings(false);
                settings.AddFileToAcquire(mFileIt, settings.DefaultAcquisitionOption);

                //download
                VDF.Vault.Results.AcquireFilesResults results = conn.FileManager.AcquireFiles(settings);
                
                //capture primary file name for return (download may include children and attachments)
                if (results.FileResults != null)
                {
                    if (results.FileResults.Any(n => n.File.EntityName == mFileIt.EntityName))
                    {
                        mFilesDownloaded.Add(conn.WorkingFoldersManager.GetPathOfFileInWorkingFolder(mFileIt).FullPath.ToString());
                    }
                }
                //the download cancelled if the file already exists in the working folder
                if (results.IsCancelled == true)
                {
                    PropertyDefinitionDictionary mProps = conn.PropertyManager.GetPropertyDefinitions(VDF.Vault.Currency.Entities.EntityClassIds.Files, null, PropertyDefinitionFilter.IncludeAll);

                    PropertyDefinition mVaultStatus = mProps[PropertyDefinitionIds.Client.VaultStatus];

                    EntityStatusImageInfo mStatus = conn.PropertyManager.GetPropertyValue(mFileIt, mVaultStatus, null) as EntityStatusImageInfo;
                    if (mStatus.Status.ConsumableState == EntityStatus.ConsumableStateEnum.LatestConsumable)
                    {
                        mFilesDownloaded.Add(conn.WorkingFoldersManager.GetPathOfFileInWorkingFolder(mFileIt).FullPath.ToString());
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
            VDF.Vault.Settings.AcquireFilesSettings settings = new VDF.Vault.Settings.AcquireFilesSettings(conn);
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


        private static long mParse(string id)
        {
            if (long.TryParse(id, out long iterationId))
            {
                return iterationId;
            }
            return -1;
        }

    }
}