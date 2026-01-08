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

namespace Autodesk.TS.VltPlmAddIn.Utils
{
    internal partial class VaultUtils
    {

        private static VDF.Vault.Currency.Connections.Connection _conn = null;

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
            _conn = VltPlmAddIn.VaultExplorerExtension.conn;

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
