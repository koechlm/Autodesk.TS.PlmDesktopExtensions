using Autodesk.Connectivity.Extensibility.Framework;
using Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.WebServicesTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

// *ComponentUpgradeEveryRelease-Client*
[assembly: ApiVersion("19.0")]
[assembly: ExtensionId("0b11c526-2bfe-45ab-8614-aa22c8d861b4")]

namespace Autodesk.TS.PlmEventHandler
{
    public class EventHandlers : IWebServiceExtension
    {
        #region custom variables
        private static readonly List<string> mExcludedCategories = new List<string>()
        {
            "Reference",
            "Phantom",
            "Substitute",
            "Inventor Drawing"
        };

        private static readonly List<string> mExcludedFileCls = new List<string>()
        {
            "Design Visualization",
            "Design Representation",
            "Configuration Factory",
            "Design Document"
        };

        private const string mFMConfigName = "Adsk.Vault.ExternalSyncTask.FusionManage";

        #endregion custom variables

        #region IWebServiceExtension Members

        public void OnLoad()
        {
            #region register events
            // register for events here
            // comment out events not being used

            // File Events
            //DocumentService.AddFileEvents.GetRestrictions += new EventHandler<AddFileCommandEventArgs>(AddFileEvents_GetRestrictions);
            DocumentService.AddFileEvents.Post += new EventHandler<AddFileCommandEventArgs>(AddFileEvents_Post);
            //DocumentService.AddFileEvents.Pre += new EventHandler<AddFileCommandEventArgs>(AddFileEvents_Pre);
            //DocumentService.CheckinFileEvents.GetRestrictions += new EventHandler<CheckinFileCommandEventArgs>(CheckinFileEvents_GetRestrictions);
            //DocumentService.CheckinFileEvents.Pre += new EventHandler<CheckinFileCommandEventArgs>(CheckInFileEvents_Pre);
            //DocumentService.CheckinFileEvents.Post += new EventHandler<CheckinFileCommandEventArgs>(CheckInFileEvents_Post);
            //DocumentService.CheckoutFileEvents.GetRestrictions += new EventHandler<CheckoutFileCommandEventArgs>(CheckoutFileEvents_GetRestrictions);
            //DocumentService.CheckoutFileEvents.Pre += new EventHandler<CheckoutFileCommandEventArgs>(CheckoutFileEvents_Pre);
            //DocumentService.CheckoutFileEvents.Post += new EventHandler<CheckoutFileCommandEventArgs>(CheckoutFileEvents_Post);
            //DocumentService.DeleteFileEvents.GetRestrictions += new EventHandler<DeleteFileCommandEventArgs>(DeleteFileEvents_GetRestrictions);
            //DocumentService.DeleteFileEvents.Pre += new EventHandler<DeleteFileCommandEventArgs>(DeleteFileEvents_Pre);
            //DocumentService.DeleteFileEvents.Post += new EventHandler<DeleteFileCommandEventArgs>(DeleteFileEvents_Post);
            //DocumentService.DownloadFileEvents.GetRestrictions += new EventHandler<DownloadFileCommandEventArgs>(DownloadFileEvents_GetRestrictions);
            //DocumentService.DownloadFileEvents.Pre += new EventHandler<DownloadFileCommandEventArgs>(DownloadFileEvents_Pre);
            //DocumentService.DownloadFileEvents.Post += new EventHandler<DownloadFileCommandEventArgs>(DownloadFileEvents_Post);

            //DocumentServiceExtensions.UpdateFileLifecycleStateEvents.GetRestrictions += new EventHandler<UpdateFileLifeCycleStateCommandEventArgs>(UpdateFileLifecycleStateEvents_GetRestrictions);
            //DocumentServiceExtensions.UpdateFileLifecycleStateEvents.Pre += new EventHandler<UpdateFileLifeCycleStateCommandEventArgs>(UpdateFileLifecycleStateEvents_Pre);
            DocumentServiceExtensions.UpdateFileLifecycleStateEvents.Post += new EventHandler<UpdateFileLifeCycleStateCommandEventArgs>(UpdateFileLifecycleStateEvents_Post);

            // Folder Events
            //DocumentService.AddFolderEvents.GetRestrictions += new EventHandler<AddFolderCommandEventArgs>(AddFolderEvents_GetRestrictions);
            //DocumentService.AddFolderEvents.Pre += new EventHandler<AddFolderCommandEventArgs>(AddFolderEvents_Pre);
            //DocumentService.AddFolderEvents.Post += new EventHandler<AddFolderCommandEventArgs>(AddFolderEvents_Post);
            //DocumentService.DeleteFolderEvents.GetRestrictions += new EventHandler<DeleteFolderCommandEventArgs>(DeleteFolderEvents_GetRestrictions);
            //DocumentService.DeleteFolderEvents.Pre += new EventHandler<DeleteFolderCommandEventArgs>(DeleteFolderEvents_Pre);
            //DocumentService.DeleteFolderEvents.Post += new EventHandler<DeleteFolderCommandEventArgs>(DeleteFolderEvents_Post);
            //DocumentService.MoveFolderEvents.GetRestrictions += new EventHandler<MoveFolderCommandEventArgs>(MoveFolderEvent_GetRestrictions);
            //DocumentService.MoveFolderEvents.Pre += new EventHandler<MoveFolderCommandEventArgs>(MoveFolderEvent_Pre);
            //DocumentService.MoveFolderEvents.Post += new EventHandler<MoveFolderCommandEventArgs>(MoveFolderEvent_Post);

            //DocumentServiceExtensions.UpdateFolderLifecycleStateEvents.GetRestrictions += new EventHandler<UpdateFolderLifeCycleStateCommandEventArgs>(UpdateFolderLifecycleStateEvents_GetRestrictions);
            //DocumentServiceExtensions.UpdateFolderLifecycleStateEvents.Pre += new EventHandler<UpdateFolderLifeCycleStateCommandEventArgs>(UpdateFolderLifecycleStateEvents_Pre);
            //DocumentServiceExtensions.UpdateFolderLifecycleStateEvents.Post += new EventHandler<UpdateFolderLifeCycleStateCommandEventArgs>(UpdateFolderLifecycleStateEvents_Post);

            // Item Events
            //ItemService.AddItemEvents.GetRestrictions += new EventHandler<AddItemCommandEventArgs>(AddItemEvents_GetRestrictions);
            //ItemService.AddItemEvents.Pre += new EventHandler<AddItemCommandEventArgs>(AddItemEvents_Post);
            //ItemService.AddItemEvents.Post += new EventHandler<AddItemCommandEventArgs>(AddItemEvents_Post);
            //ItemService.CommitItemEvents.GetRestrictions += new EventHandler<CommitItemCommandEventArgs>(CommitItemEvents_GetRestrictions);
            //ItemService.CommitItemEvents.Pre += new EventHandler<CommitItemCommandEventArgs>(CommitItemEvents_Pre);
            ItemService.CommitItemEvents.Post += new EventHandler<CommitItemCommandEventArgs>(CommitItemEvents_Post);
            //ItemService.ItemRollbackLifeCycleStatesEvents.GetRestrictions += new EventHandler<ItemRollbackLifeCycleStateCommandEventArgs>(ItemRollbackLifeCycleStatesEvents_GetRestrictions);
            //ItemService.ItemRollbackLifeCycleStatesEvents.Pre += new EventHandler<ItemRollbackLifeCycleStateCommandEventArgs>(ItemRollbackLifeCycleStatesEvents_Pre);
            //ItemService.ItemRollbackLifeCycleStatesEvents.Post += new EventHandler<ItemRollbackLifeCycleStateCommandEventArgs>(ItemRollbackLifeCycleStatesEvents_Pos);
            //ItemService.DeleteItemEvents.GetRestrictions += new EventHandler<DeleteItemCommandEventArgs>(DeleteItemEvents_GetRestrictions);
            //ItemService.DeleteItemEvents.Pre += new EventHandler<DeleteItemCommandEventArgs>(DeleteItemEvents_Pre);
            //ItemService.DeleteItemEvents.Post += new EventHandler<DeleteItemCommandEventArgs>(DeleteItemEvents_Post);
            //ItemService.EditItemEvents.GetRestrictions += new EventHandler<EditItemCommandEventArgs>(EditItemEvents_GetRestrictions);
            //ItemService.EditItemEvents.Pre += new EventHandler<EditItemCommandEventArgs>(EditItemEvents_Pre);
            //ItemService.EditItemEvents.Post += new EventHandler<EditItemCommandEventArgs>(EditItemEvents_Post);
            //ItemService.PromoteItemEvents.GetRestrictions += new EventHandler<PromoteItemCommandEventArgs>(PromoteItemEvents_GetRestrictions);
            //ItemService.PromoteItemEvents.Pre += new EventHandler<PromoteItemCommandEventArgs>(PromoteItemEvents_Pre);
            //ItemService.PromoteItemEvents.Post += new EventHandler<PromoteItemCommandEventArgs>(PromoteItemEvents_Post);
            //ItemService.UpdateItemLifecycleStateEvents.GetRestrictions += new EventHandler<UpdateItemLifeCycleStateCommandEventArgs>(UpdateItemLifecycleStateEvents_GetRestrictions);
            //ItemService.UpdateItemLifecycleStateEvents.Pre += new EventHandler<UpdateItemLifeCycleStateCommandEventArgs>(UpdateItemLifecycleStateEvents_Pre);
            //ItemService.UpdateItemLifecycleStateEvents.Post += new EventHandler<UpdateItemLifeCycleStateCommandEventArgs>(UpdateItemLifecycleStateEvents_Post);

            // Change Order Events
            //ChangeOrderService.AddChangeOrderEvents.GetRestrictions += new EventHandler<AddChangeOrderCommandEventArgs>(AddChangeOrderEvents_GetRestrictions);
            //ChangeOrderService.AddChangeOrderEvents.Pre += new EventHandler<AddChangeOrderCommandEventArgs>(AddChangeOrderEvents_Pre);
            //ChangeOrderService.AddChangeOrderEvents.Post += new EventHandler<AddChangeOrderCommandEventArgs>(AddChangeOrderEvents_Post);
            //ChangeOrderService.CommitChangeOrderEvents.GetRestrictions += new EventHandler<CommitChangeOrderCommandEventArgs>(CommitChangeOrderEvents_GetRestrictions);
            //ChangeOrderService.CommitChangeOrderEvents.Pre += new EventHandler<CommitChangeOrderCommandEventArgs>(CommitChangeOrderEvents_Pre);
            //ChangeOrderService.CommitChangeOrderEvents.Post += new EventHandler<CommitChangeOrderCommandEventArgs>(CommitChangeOrderEvents_Post);
            //ChangeOrderService.DeleteChangeOrderEvents.GetRestrictions += new EventHandler<DeleteChangeOrderCommandEventArgs>(DeleteChangeOrderEvents_GetRestrictions);
            //ChangeOrderService.DeleteChangeOrderEvents.Pre += new EventHandler<DeleteChangeOrderCommandEventArgs>(DeleteChangeOrderEvents_Pre);
            //ChangeOrderService.DeleteChangeOrderEvents.Post += new EventHandler<DeleteChangeOrderCommandEventArgs>(DeleteChangeOrderEvents_Post);
            //ChangeOrderService.EditChangeOrderEvents.GetRestrictions += new EventHandler<EditChangeOrderCommandEventArgs>(EditChangeOrderEvents_GetRestrictions);
            //ChangeOrderService.EditChangeOrderEvents.Pre += new EventHandler<EditChangeOrderCommandEventArgs>(EditChangeOrderEvents_Pre);
            //ChangeOrderService.EditChangeOrderEvents.Post += new EventHandler<EditChangeOrderCommandEventArgs>(EditChangeOrderEvents_Post);
            //ChangeOrderService.UpdateChangeOrderLifecycleStateEvents.GetRestrictions += new EventHandler<UpdateChangeOrderLifeCycleStateCommandEventArgs>(UpdateChangeOrderLifecycleStateEvents_GetRestrictions);
            //ChangeOrderService.UpdateChangeOrderLifecycleStateEvents.Pre += new EventHandler<UpdateChangeOrderLifeCycleStateCommandEventArgs>(UpdateChangeOrderLifecycleStateEvents_Pre);
            //ChangeOrderService.UpdateChangeOrderLifecycleStateEvents.Post += new EventHandler<UpdateChangeOrderLifeCycleStateCommandEventArgs>(UpdateChangeOrderLifecycleStateEvents_Post);

            // Custom Entity Events
            //CustomEntityService.UpdateCustomEntityLifecycleStateEvents.GetRestrictions += new EventHandler<UpdateCustomEntityLifeCycleStateCommandEventArgs>(UpdateCustomEntityLifecycleStateEvents_GetRestrictions);
            //CustomEntityService.UpdateCustomEntityLifecycleStateEvents.Pre += new EventHandler<UpdateCustomEntityLifeCycleStateCommandEventArgs>(UpdateCustomEntityLifecycleStateEvents_Pre);
            //CustomEntityService.UpdateCustomEntityLifecycleStateEvents.Post += new EventHandler<UpdateCustomEntityLifeCycleStateCommandEventArgs>(UpdateCustomEntityLifecycleStateEvents_Post);
            #endregion register events
        }

        #region CustomEntity Events
        private void UpdateCustomEntityLifecycleStateEvents_Post(object sender, UpdateCustomEntityLifeCycleStateCommandEventArgs e)
        {
            //add code here;
        }

        private void UpdateCustomEntityLifecycleStateEvents_Pre(object sender, UpdateCustomEntityLifeCycleStateCommandEventArgs e)
        {
            //add code here;
        }

        private void UpdateCustomEntityLifecycleStateEvents_GetRestrictions(object sender, UpdateCustomEntityLifeCycleStateCommandEventArgs e)
        {
            //add code here;
        }
        #endregion CustomEntity Events

        #region ChangeOrder Events
        private void UpdateChangeOrderLifecycleStateEvents_Post(object sender, UpdateChangeOrderLifeCycleStateCommandEventArgs e)
        {
            //add code here;
        }

        private void UpdateChangeOrderLifecycleStateEvents_Pre(object sender, UpdateChangeOrderLifeCycleStateCommandEventArgs e)
        {
            //add code here;
        }

        private void UpdateChangeOrderLifecycleStateEvents_GetRestrictions(object sender, UpdateChangeOrderLifeCycleStateCommandEventArgs e)
        {
            //add code here;
        }

        private void EditChangeOrderEvents_Post(object sender, EditChangeOrderCommandEventArgs e)
        {
            //add code here;
        }

        private void EditChangeOrderEvents_Pre(object sender, EditChangeOrderCommandEventArgs e)
        {
            //add code here;
        }

        private void EditChangeOrderEvents_GetRestrictions(object sender, EditChangeOrderCommandEventArgs e)
        {
            //add code here;
        }

        private void DeleteChangeOrderEvents_Post(object sender, DeleteChangeOrderCommandEventArgs e)
        {
            //add code here;
        }

        private void DeleteChangeOrderEvents_Pre(object sender, DeleteChangeOrderCommandEventArgs e)
        {
            //add code here;
        }

        private void DeleteChangeOrderEvents_GetRestrictions(object sender, DeleteChangeOrderCommandEventArgs e)
        {
            //add code here;
        }

        private void CommitChangeOrderEvents_Post(object sender, CommitChangeOrderCommandEventArgs e)
        {
            //add code here;
        }

        private void CommitChangeOrderEvents_Pre(object sender, CommitChangeOrderCommandEventArgs e)
        {
            //add code here;
        }

        private void CommitChangeOrderEvents_GetRestrictions(object sender, CommitChangeOrderCommandEventArgs e)
        {
            //add code here;
        }

        private void AddChangeOrderEvents_Post(object sender, AddChangeOrderCommandEventArgs e)
        {
            //add code here;
        }

        private void AddChangeOrderEvents_Pre(object sender, AddChangeOrderCommandEventArgs e)
        {
            //add code here;
        }

        private void AddChangeOrderEvents_GetRestrictions(object sender, AddChangeOrderCommandEventArgs e)
        {
            //add code here;
        }
        #endregion ChangeOrder Events

        #region Item Events
        private void UpdateItemLifecycleStateEvents_Post(object sender, UpdateItemLifeCycleStateCommandEventArgs e)
        {
            //add code here;
        }

        private void UpdateItemLifecycleStateEvents_Pre(object sender, UpdateItemLifeCycleStateCommandEventArgs e)
        {
            //add code here;
        }

        private void UpdateItemLifecycleStateEvents_GetRestrictions(object sender, UpdateItemLifeCycleStateCommandEventArgs e)
        {
            //add code here;
        }

        private void PromoteItemEvents_Post(object sender, PromoteItemCommandEventArgs e)
        {
            //add code here;
        }

        private void PromoteItemEvents_Pre(object sender, PromoteItemCommandEventArgs e)
        {
            //add code here;
        }

        private void PromoteItemEvents_GetRestrictions(object sender, PromoteItemCommandEventArgs e)
        {
            //add code here;
        }

        private void EditItemEvents_Post(object sender, EditItemCommandEventArgs e)
        {
            //add code here;
        }

        private void EditItemEvents_Pre(object sender, EditItemCommandEventArgs e)
        {
            //add code here;
        }

        private void EditItemEvents_GetRestrictions(object sender, EditItemCommandEventArgs e)
        {
            //add code here;
        }

        private void DeleteItemEvents_Post(object sender, DeleteItemCommandEventArgs e)
        {
            //add code here;
        }

        private void DeleteItemEvents_Pre(object sender, DeleteItemCommandEventArgs e)
        {
            //add code here;
        }

        private void DeleteItemEvents_GetRestrictions(object sender, DeleteItemCommandEventArgs e)
        {
            //add code here;
        }

        private void ItemRollbackLifeCycleStatesEvents_Pos(object sender, ItemRollbackLifeCycleStateCommandEventArgs e)
        {
            //add code here;
        }

        private void ItemRollbackLifeCycleStatesEvents_Pre(object sender, ItemRollbackLifeCycleStateCommandEventArgs e)
        {
            //add code here;
        }

        private void ItemRollbackLifeCycleStatesEvents_GetRestrictions(object sender, ItemRollbackLifeCycleStateCommandEventArgs e)
        {
            //add code here;
        }

        private void CommitItemEvents_Post(object sender, CommitItemCommandEventArgs e)
        {
            if (e.Status == EventStatus.FAIL)
            {
                return;
            }

            IWebService service = sender as IWebService;
            if (service == null)
                return;

            WebServiceCredentials cred = new WebServiceCredentials(service);
            using (WebServiceManager serviceManager = new WebServiceManager(cred))
            {
                var mExternalSyncService = serviceManager.ExternalSyncService;

                // submit the task to FM for the created/modified item
                long mId = serviceManager.ItemService.GetItemsByRevisionIds(new long[] { e.ItemRevisionIds.FirstOrDefault() }, true).FirstOrDefault().MasterId;
                mExternalSyncService.AddExtSyncTask(mId, "ITEM", mFMConfigName, null);
            }
        }

        private void CommitItemEvents_Pre(object sender, CommitItemCommandEventArgs e)
        {
            //add code here;
        }

        private void CommitItemEvents_GetRestrictions(object sender, CommitItemCommandEventArgs e)
        {
            //add code here;
        }
        private void AddItemEvents_Post(object sender, AddItemCommandEventArgs e)
        {
            //add code here;
        }

        private void AddItemEvents_GetRestrictions(object sender, AddItemCommandEventArgs e)
        {
            //add code here;
        }
        #endregion Item Events

        #region Folder Events
        private void UpdateFolderLifecycleStateEvents_Post(object sender, UpdateFolderLifeCycleStateCommandEventArgs e)
        {
            //add code here;
        }

        private void UpdateFolderLifecycleStateEvents_Pre(object sender, UpdateFolderLifeCycleStateCommandEventArgs e)
        {
            //add code here;
        }

        private void UpdateFolderLifecycleStateEvents_GetRestrictions(object sender, UpdateFolderLifeCycleStateCommandEventArgs e)
        {
            //add code here;
        }

        private void MoveFolderEvent_Post(object sender, MoveFolderCommandEventArgs e)
        {
            //add code here;
        }

        private void MoveFolderEvent_Pre(object sender, MoveFolderCommandEventArgs e)
        {
            //add code here;

        }

        private void MoveFolderEvent_GetRestrictions(object sender, MoveFolderCommandEventArgs e)
        {
            //add code here;
        }

        private void DeleteFolderEvents_Post(object sender, DeleteFolderCommandEventArgs e)
        {
            //add code here;
        }

        private void DeleteFolderEvents_Pre(object sender, DeleteFolderCommandEventArgs e)
        {
            //add code here;
        }

        private void DeleteFolderEvents_GetRestrictions(object sender, DeleteFolderCommandEventArgs e)
        {
            //add code here;
        }

        private void AddFolderEvents_Post(object sender, AddFolderCommandEventArgs e)
        {
            //add code here;
        }

        private void AddFolderEvents_Pre(object sender, AddFolderCommandEventArgs e)
        {
            //add code here;
        }

        private void AddFolderEvents_GetRestrictions(object sender, AddFolderCommandEventArgs e)
        {
            //add code here;
        }
        #endregion Folder Events

        #region File Events
        private void UpdateFileLifecycleStateEvents_Post(object sender, UpdateFileLifeCycleStateCommandEventArgs e)
        {
            if (e.Status == EventStatus.FAIL)
            {
                return;
            }

            // on success only: call the assign/update item method to initiate an item creation on FM;
            mAssignUpdateItem(sender, e.ReturnValue.FirstOrDefault());

        }

        private void UpdateFileLifecycleStateEvents_Pre(object sender, UpdateFileLifeCycleStateCommandEventArgs e)
        {
            //add code here;
        }

        private void UpdateFileLifecycleStateEvents_GetRestrictions(object sender, UpdateFileLifeCycleStateCommandEventArgs e)
        {
            //add code here;
        }

        private void DownloadFileEvents_Post(object sender, DownloadFileCommandEventArgs e)
        {
            //add code here;
        }

        private void DownloadFileEvents_Pre(object sender, DownloadFileCommandEventArgs e)
        {
            //add code here;
        }

        private void DownloadFileEvents_GetRestrictions(object sender, DownloadFileCommandEventArgs e)
        {
            //add code here;
        }

        private void DeleteFileEvents_Post(object sender, DeleteFileCommandEventArgs e)
        {
            //add code here;
        }

        private void DeleteFileEvents_Pre(object sender, DeleteFileCommandEventArgs e)
        {
            //add code here;
        }

        private void DeleteFileEvents_GetRestrictions(object sender, DeleteFileCommandEventArgs e)
        {
            //add code here;
        }

        private void CheckoutFileEvents_Post(object sender, CheckoutFileCommandEventArgs e)
        {
            //add code here;
        }

        private void CheckoutFileEvents_Pre(object sender, CheckoutFileCommandEventArgs e)
        {
            //add code here;
        }

        private void CheckoutFileEvents_GetRestrictions(object sender, CheckoutFileCommandEventArgs e)
        {
            //add code here;
        }

        private void CheckInFileEvents_Post(object sender, CheckinFileCommandEventArgs e)
        {
            //add code here;
        }

        private void CheckInFileEvents_Pre(object sender, CheckinFileCommandEventArgs e)
        {
            //add code here;
        }

        private void CheckinFileEvents_GetRestrictions(object sender, CheckinFileCommandEventArgs e)
        {
            //add code here;
        }

        private void AddFileEvents_Pre(object sender, AddFileCommandEventArgs e)
        {
            //add code here;
        }

        private void AddFileEvents_Post(object sender, AddFileCommandEventArgs e)
        {
            if (e.Status == EventStatus.FAIL)
            {
                return;
            }

            // on success only: call the assign/update item method to initiate an item creation on FM;
            mAssignUpdateItem(sender, e.ReturnValue);
        }

        private void AddFileEvents_GetRestrictions(object sender, AddFileCommandEventArgs e)
        {
            //add code here;
        }
        #endregion File Events
        #endregion IWebServiceExtension Members

        #region execution methods

        private void mAssignUpdateItem(object sender, File file)
        {
            // exclude categories that must not get an item assigned and would fail
            if (mExcludedCategories.Contains(file.Cat.CatName))
            {
                return;
            }

            // exclude file classifications
            if (mExcludedFileCls.Contains(file.FileClass.ToString()))
            {
                return;
            }

            // call promote file to assign or update item on FM
            mPromoteFileToItem(sender, file.Id);

        }

        private void mPromoteFileToItem(object sender, long mFileId)
        {
            IWebService service = sender as IWebService;
            if (service == null)
                return;
            WebServiceCredentials cred = new WebServiceCredentials(service);
            using (WebServiceManager serviceManager = new WebServiceManager(cred))
            {
                long currentUserId = serviceManager.SecurityService.Session.User.Id;
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
                    if (promoteResult == null && mPromoteFailed != true)
                    {
                        // clear out the promoted item
                        serviceManager.ItemService.DeleteUnusedItemNumbers(new long[] { promoteResult.ItemRevArray[0].MasterId });
                        serviceManager.ItemService.UndoEditItems(new long[] { promoteResult.ItemRevArray[0].Id });
                    }
                }
            }

        }

        #endregion execution methods
    }
}
