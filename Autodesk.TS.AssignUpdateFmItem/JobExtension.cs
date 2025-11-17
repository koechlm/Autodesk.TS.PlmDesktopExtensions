using Autodesk.Connectivity.Extensibility.Framework;
using Autodesk.Connectivity.JobProcessor.Extensibility;
using Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.WebServicesTools;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// *ComponentUpgradeEveryRelease-Client*
[assembly: ApiVersion("19.0")]
[assembly: ExtensionId("e47f92ba-e2c2-4c6c-91c3-9482b732d738")]


namespace adsk.ts.job.assignupdateitem
{
    /// <summary>
    /// Job handler to assign/update Vault and Fusion Manage items
    /// </summary>
    public class JobExtension : IJobHandler
    {
        private static string JOB_TYPE = "adsk.ts.job.assignupdateitem";

        #region custom variables
        private static readonly List<string> mExcludedCategories = new List<string>()
        {
            "Reference",
            "Phantom",
            "Substitute"
        };

        private static readonly List<FileClassification> mExcludedFileCls = new()
        {
            FileClassification.DesignVisualization,
            FileClassification.DesignRepresentation,
            FileClassification.ConfigurationFactory
        };

        private static Settings mSettings = Settings.Load();
        private static string mLogDir = JobExtension.mSettings.LogFileLocation;
        private static string mLogFile;
        private TextWriterTraceListener mTrace;
        Connection connection = null;
        WebServiceManager mWsMgr = null;
        Autodesk.Connectivity.WebServices.File mFile = null;


        #endregion custom variables

        #region IJobHandler Implementation
        public bool CanProcess(string jobType)
        {
            return jobType == JOB_TYPE;
        }

        public JobOutcome Execute(IJobProcessorServices context, IJob job)
        {
            try
            {
                //pick up this job's context
                connection = context.Connection;
                mWsMgr = connection.WebServiceManager;
                long mEntId = Convert.ToInt64(job.Params["EntityId"]);
                string mEntClsId = job.Params["EntityClassId"];

                // only run the job for files
                if (mEntClsId != "FILE")
                    return JobOutcome.Success;

                // get the file object for this job
                mFile = mWsMgr.DocumentService.GetFileById(mEntId);
                if (mFile == null)
                {
                    throw new Exception("The file version is no longer available!");
                }
                if (mFile.FileRev.MaxFileId != mFile.Id)
                {
                    // not the latest file version - get the latest
                    mFile = mWsMgr.DocumentService.GetFileById(mFile.FileRev.MaxFileId);
                }

                // prepare log file and initiate logging
                mLogFile = JOB_TYPE + "_" + mFile.Name + ".log";
                FileInfo mLogFileInfo = new FileInfo(System.IO.Path.Combine(
                    mLogDir, mLogFile));
                if (mLogFileInfo.Exists) mLogFileInfo.Delete();
                mTrace = new TextWriterTraceListener(System.IO.Path.Combine(mLogDir, mLogFile), "mJobTrace");
                mTrace.WriteLine("Starting Job...");

                // assign or update FM item for this file
                mAssignUpdateItem(context.Connection.WebServiceManager, mFile);

                mTrace.IndentLevel = 0;
                mTrace.WriteLine("... successfully ending Job.");
                mTrace.Flush();
                mTrace.Close();

                return JobOutcome.Success;
            }
            catch (Exception ex)
            {
                context.Log(ex, "Job " + JOB_TYPE + " failed: " + ex.ToString() + " ");
                mTrace.IndentLevel = 0;
                mTrace.WriteLine("... ending Job with failure.");
                return JobOutcome.Failure;
            }
            finally
            {
                // close the log file
                if (mTrace != null)
                {
                    mTrace.Flush();
                    mTrace.Close();
                }
            }

        }

        private void mAssignUpdateItem(object sender, Autodesk.Connectivity.WebServices.File file)
        {
            // exclude categories that must not get an item assigned and would fail
            if (mExcludedCategories.Contains(file.Cat.CatName))
            {
                return;
            }

            // exclude file classifications
            if (mExcludedFileCls.Contains(file.FileClass))
            {
                return;
            }

            // retrieve the primary referenced file for files of classification "Design Document"
            if (file.FileClass == FileClassification.DesignDocument)
            {
                WebServiceManager serviceManager = sender as WebServiceManager;

                Autodesk.Connectivity.WebServices.File parent = null;

                DocumentService docService = serviceManager.DocumentService;
                // get the associated references
                List<TreeNode> children = new List<TreeNode>();
                FileAssocArray[] fileAssociations = serviceManager.DocumentService.GetLatestFileAssociationsByMasterIds(
                    new long[] { file.MasterId },
                    FileAssociationTypeEnum.None,
                    false,
                    FileAssociationTypeEnum.Dependency,
                    false,
                    false,
                    false,
                    false);

                if (fileAssociations.FirstOrDefault()?.FileAssocs != null)
                {
                    foreach (var fileAssociation in fileAssociations.First().FileAssocs)
                    {
                        parent = fileAssociation.CldFile;
                    }
                }

                if (parent != null)
                {
                    // use the parent file for item assignment
                    file = parent;
                }
                else
                {
                    // no valid parent found - exit
                    return;
                }
            }

            // call promote file to assign or update item on FM
            mPromoteFileToItem(sender, file.Id);

        }

        private void mPromoteFileToItem(object sender, long mFileId)
        {
            using (WebServiceManager serviceManager = sender as WebServiceManager)
            {
                ItemService mItemSvc = serviceManager.ItemService;

                ItemsAndFiles promoteResult = null;
                Item[] updatedItems = null;
                bool mPromoteFailed = false;
                try
                {
                    // in this case - we enforce to create/update an item by checkin; with that we must not cause the item creation "twice" in case an assembly's subcomponent also requires an item creation
                    // with that we have to set ItemAssignAll = No
                    mItemSvc.AddFilesToPromote(new long[] { mFileId }, ItemAssignAll.No, true);
                    DateTime timestamp;
                    GetPromoteOrderResults promoteOrderResults = mItemSvc.GetPromoteComponentOrder(out timestamp);
                    if (promoteOrderResults.PrimaryArray != null && promoteOrderResults.PrimaryArray.Any())
                        try
                        {
                            mItemSvc.PromoteComponents(timestamp, promoteOrderResults.PrimaryArray);
                        }
                        catch
                        {
                            mPromoteFailed = true;
                            //create new restriction / message 
                        }
                    if (promoteOrderResults.NonPrimaryArray != null && promoteOrderResults.NonPrimaryArray.Any())
                        try
                        {
                            mItemSvc.PromoteComponentLinks(promoteOrderResults.NonPrimaryArray);
                        }
                        catch
                        {
                            mPromoteFailed = true;
                            //create new restriction / message indicating that the item (unknown number here) linked to file e is probably locked by an editor
                        }
                    try
                    {
                        if (mPromoteFailed != true)
                        {
                            promoteResult = mItemSvc.GetPromoteComponentsResults(timestamp);
                            //check the result for locked root item as we continue to update this
                            if (promoteResult.ItemRevArray[0].Locked != true)
                            {
                                updatedItems = promoteResult.ItemRevArray;
                                Item m_CurrentItem = promoteResult.ItemRevArray[0];
                                Item[] m_ItemToUpdateCommit = new Item[1];
                                m_ItemToUpdateCommit[0] = m_CurrentItem;
                                // commit the changes for the root element only; the reason is as stated before for ItemAssignAll = No
                                mItemSvc.UpdateAndCommitItems(m_ItemToUpdateCommit);
                            }
                            else
                            {
                                //create a restriction for file e and item promoteResult.ItemRevArray[0] Number / Title
                            }
                        }

                    }
                    catch
                    {
                        //still an unhandled situation?
                    }
                }
                catch
                {
                    if (updatedItems != null && updatedItems.Length > 0)
                    {
                        long[] itemIds = new long[updatedItems.Length];
                        for (int i = 0; i < updatedItems.Length; i++)
                        {
                            itemIds[i] = updatedItems[i].Id;
                        }
                        serviceManager.ItemService.UndoEditItems(itemIds);
                    }
                }
                finally
                {
                    if (promoteResult != null && mPromoteFailed == true)
                    {
                        // clear out the promoted item
                        serviceManager.ItemService.DeleteUnusedItemNumbers(new long[] { promoteResult.ItemRevArray[0].MasterId });
                        serviceManager.ItemService.UndoEditItems(new long[] { promoteResult.ItemRevArray[0].Id });
                    }
                }
            }

        }

        public void OnJobProcessorShutdown(IJobProcessorServices context)
        {
            //throw new NotImplementedException();
        }

        public void OnJobProcessorSleep(IJobProcessorServices context)
        {
            //throw new NotImplementedException();
        }

        public void OnJobProcessorStartup(IJobProcessorServices context)
        {
            //throw new NotImplementedException();
        }

        public void OnJobProcessorWake(IJobProcessorServices context)
        {
            //throw new NotImplementedException();
        }
        #endregion IJobHandler Implementation
    }
}
