using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Media;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.Versioning;
using System.Windows.Media.Imaging;
using Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.WebServicesTools;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Entities;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.PersistentId;
using VDF = Autodesk.DataManagement.Client.Framework;
using ACET = Autodesk.Connectivity.Explorer.ExtensibilityTools;
using Inventor;
using AcInterop = Autodesk.AutoCAD.Interop;
using AcInteropCom = Autodesk.AutoCAD.Interop.Common;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Properties;


namespace Autodesk.TS.VltPlmAddIn.Utils
{
    /// <summary>
    /// Class representing a node in the tree structure of file dependencies.
    /// </summary>

    public class TreeNode
    {
        private Connection _con;
        private Autodesk.Connectivity.WebServices.File _file;
        private WebServiceManager _svc => _con?.WebServiceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNode"/> class.
        /// </summary>
        /// <param name="file">The file to be represented by this node.</param>
        /// <param name="con">The connection to the Vault.</param>
        public TreeNode(Autodesk.Connectivity.WebServices.File file, Connection con)
        {
            _con = con;
            _file = file;
        }

        /// <summary>
        /// Gets the name of the file represented by this node.
        /// </summary>
        public string Name => _file?.Name;

        /// <summary>
        /// Retrieve the children of the current file. The children are the files that are dependent on this file.
        /// </summary>
        public List<TreeNode> Children
        {
            get
            {
                if (_svc == null || _file == null)
                {
                    throw new InvalidOperationException("WebServiceManager or File is null.");
                }

                List<TreeNode> children = new List<TreeNode>();
                FileAssocArray[] fileAssociations = _svc.DocumentService.GetLatestFileAssociationsByMasterIds(
                    new long[] { _file.MasterId },
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
                        children.Add(new TreeNode(fileAssociation.CldFile, _con));
                    }
                }
                return children;
            }
        }

        /// <summary>
        /// Retrieve the parent of the current file. The parent is the file that is dependent on this file.
        /// </summary>
        public List<TreeNode> Parents
        {
            get
            {
                if (_svc == null || _file == null)
                {
                    throw new InvalidOperationException("WebServiceManager or File is null.");
                }

                List<TreeNode> parents = new List<TreeNode>();
                FileAssocArray[] fileAssociations = _svc.DocumentService.GetLatestFileAssociationsByMasterIds(
                    new long[] { _file.MasterId },
                    FileAssociationTypeEnum.Dependency,
                    false,
                    FileAssociationTypeEnum.None,
                    false,
                    false,
                    false,
                    false);

                if (fileAssociations.FirstOrDefault()?.FileAssocs != null)
                {
                    foreach (var fileAssociation in fileAssociations.First().FileAssocs)
                    {
                        parents.Add(new TreeNode(fileAssociation.ParFile, _con));
                    }
                }
                return parents;
            }
        }

        /// <summary>
        /// Get the file type's icon as a bitmap image.
        /// </summary>
        public BitmapImage Icon
        {
            get
            {
                PropertyDefinitionDictionary props = _con.PropertyManager.GetPropertyDefinitions("FILE", null, PropertyDefinitionFilter.IncludeAll);
                var def = props["EntityIcon"];
                object fileIter = new FileIteration(_con, _file);
                ImageInfo prop = _con.PropertyManager.GetPropertyValue((IEntity)fileIter, def, null) as ImageInfo;
                if (prop != null)
                {
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    // Save the memory stream as a PNG file
                    prop.GetImage().Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    prop.Dispose();
                    System.Windows.Media.Imaging.BitmapImage bImg = new System.Windows.Media.Imaging.BitmapImage();
                    bImg.BeginInit();
                    bImg.StreamSource = ms;// new System.IO.MemoryStream(ms.ToArray());
                    bImg.EndInit();
                    return bImg;
                }
                return null;
            }
        }
    }

    /// <summary>
    /// Class extending VDS Vault scripts
    /// </summary>
    public class VltHelpers
    {
        /// <summary>
        /// Creates user credentials for connecting to a Vault server.
        /// </summary>
        /// <param name="server">The IP address or DNS name of the ADMS server.</param>
        /// <param name="vault">The name of the Vault to connect to.</param>
        /// <param name="user">The username for authentication.</param>
        /// <param name="pw">The password for authentication.</param>
        /// <returns>A <see cref="Autodesk.Connectivity.WebServicesTools.UserPasswordCredentials"/> object for the specified server and Vault.</returns>
        public static Autodesk.Connectivity.WebServicesTools.UserPasswordCredentials UserCredentials1(string server, string vault, string user, string pw)
        {
            // Simplify object initialization and ensure platform compatibility
            var mServer = new ServerIdentities
            {
                DataServer = server,
                FileServer = server
            };

            return new Autodesk.Connectivity.WebServicesTools.UserPasswordCredentials(mServer, vault, user, pw);
        }

        /// <summary>
        /// UserCredentials1 and UserCredentials2 differentiate overloads as powershell can't handle
        /// UserCredentials2 returns readonly loginuser object
        /// </summary>
        /// <param name="server">IP Address or DNS Name of ADMS Server</param>
        /// <param name="vault">Name of vault to connect to</param>
        /// <param name="user">User name</param>
        /// <param name="pw">Password</param>
        /// <param name="rw">Set to "True" to allow Read/Write access</param>
        /// <returns></returns>
        public Autodesk.Connectivity.WebServicesTools.UserPasswordCredentials UserCredentials2(string server, string vault, string user, string pw, bool rw = true)
        {
            // Simplify object initialization and ensure platform compatibility
            var mServer = new ServerIdentities
            {
                DataServer = server,
                FileServer = server
            };

            return new Autodesk.Connectivity.WebServicesTools.UserPasswordCredentials(mServer, vault, user, pw, rw);
        }

        /// <summary>
        /// Deprecated - no longer required, as the overload is removed in 2017 API
        /// </summary>
        /// <param name="svc"></param>
        /// <param name="FldIds"></param>
        /// <param name="m_PropArray"></param>
        /// <returns></returns>
        public Boolean UpdateFolderProp2(WebServiceManager svc, long[] FldIds, PropInstParamArray[] m_PropArray)
        {
            try
            {
                svc.DocumentServiceExtensions.UpdateFolderProperties(FldIds, m_PropArray);
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// LinkManager.GetLinkedChildren has an override list; the input is of type IEntity. 
        /// This wrapper allows to input commonly known object types, like Ids and entity names instead.
        /// </summary>
        /// <param name="con">The utility dll is not connected to Vault; 
        /// we need to leverage the established connection to call LinkManager methods</param>
        /// <param name="mId">The parent entity's id to get linked children of</param>
        /// <param name="mClsId">The parent entity's class name; allowed values are FILE FLDR and CUSTENT. 
        /// CO and ITEM cannot have linked children, as they use specific links to related child objects.</param>
        /// <param name="mFilter">Limit the search on links to a particular class; providing an empty value "" will result in a search on all types</param>
        /// <returns>List of entity Ids</returns>
        public List<long> mGetLinkedChildren1(Connection con, long mId, string mClsId, string mFilter)
        {
            IEnumerable<PersistableIdEntInfo> mEntInfo = new PersistableIdEntInfo[] { new PersistableIdEntInfo(mClsId, mId, true, false) };
            IDictionary<PersistableIdEntInfo, IEntity> mIEnts = con.EntityOperations.ConvertEntInfosToIEntities(mEntInfo);
            IEntity mIEnt = null;
            try
            {
                foreach (var item in mIEnts)
                {
                    mIEnt = item.Value;
                }
                IEnumerable<IEntity> mLinkedChldrn = con.LinkManager.GetLinkedChildren(mIEnt, mFilter);
                //return mLinkedChldrn;
                List<long> mLinkedIds = new List<long>();
                foreach (var item in mLinkedChldrn)
                {
                    mLinkedIds.Add(item.EntityIterationId);
                }
                return mLinkedIds;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Evaluation of overload 2; see mGetLinkedchildren1 for detailed description
        /// </summary>
        /// <param name="con"></param>
        /// <param name="mParEntIds"></param>
        /// <param name="mClsIds"></param>
        /// <returns></returns>
        private static IEnumerable<IEntity> GetLinkedChildren2(Connection con, long[] mParEntIds, string[] mClsIds)
        {
            List<PersistableIdEntInfo> mEntInfo = new List<PersistableIdEntInfo>();
            for (int i = 0; i < mParEntIds.Length; i++)
            {
                mEntInfo.Add(new PersistableIdEntInfo("CUSTENT", mParEntIds[i], true, false));
            }

            IDictionary<PersistableIdEntInfo, IEntity> mIEnts = con.EntityOperations.ConvertEntInfosToIEntities(mEntInfo.AsEnumerable());
            List<IEntity> mIEnt = new List<IEntity>();
            try
            {
                foreach (var item in mIEnts)
                {
                    mIEnt.Add(item.Value);
                }
                IEnumerable<IEntity> mLinkedChldrn = con.LinkManager.GetLinkedChildren(mIEnt.AsEnumerable(), mClsIds.AsEnumerable());
                return mLinkedChldrn;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Update file properties
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="mFile"></param>
        /// <param name="mPropDictonary"></param>
        /// <returns>True if updated successfully</returns>
        public bool mUpdateFileProperties(VDF.Vault.Currency.Connections.Connection conn,
            Autodesk.Connectivity.WebServices.File mFile, Dictionary<Autodesk.Connectivity.WebServices.PropDef, object> mPropDictonary)
        {
            try
            {
                ACET.IExplorerUtil mExplUtil = Autodesk.Connectivity.Explorer.ExtensibilityTools.ExplorerLoader.LoadExplorerUtil(
                                            conn.Server, conn.Vault, conn.UserID, conn.Ticket);

                mExplUtil.UpdateFileProperties(mFile, mPropDictonary);
                return true;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Downloads Vault file using full file path, e.g. "$/Designs/Base.ipt". Returns full file name in local working folder (download enforces override, if local file exists),
        /// returns "FileNotFound if file does not exist at indicated location.
        /// Preset Options: Download Children (recursively) = Enabled, Enforce Overwrite = True
        /// </summary>
        /// <param name="conn">Current Vault Connection</param>
        /// <param name="VaultFullFileName">FullFilePath</param>
        /// <param name="CheckOut">Optional. File downloaded does NOT check-out as default.</param>
        /// <returns>Local path/filename or error statement "FileNotFound"</returns>
        public string mGetFileByFullFileName(VDF.Vault.Currency.Connections.Connection conn, string VaultFullFileName, bool CheckOut = false)
        {
            List<string> mFiles = new List<string>();
            mFiles.Add(VaultFullFileName);
            Autodesk.Connectivity.WebServices.File[] wsFiles = conn.WebServiceManager.DocumentService.FindLatestFilesByPaths(mFiles.ToArray());
            VDF.Vault.Currency.Entities.FileIteration mFileIt = new VDF.Vault.Currency.Entities.FileIteration(conn, (wsFiles[0]));

            VDF.Vault.Settings.AcquireFilesSettings settings = new VDF.Vault.Settings.AcquireFilesSettings(conn);
            if (CheckOut)
            {
                settings.DefaultAcquisitionOption = VDF.Vault.Settings.AcquireFilesSettings.AcquisitionOption.Checkout;
            }
            else
            {
                settings.DefaultAcquisitionOption = VDF.Vault.Settings.AcquireFilesSettings.AcquisitionOption.Download;
            }
            settings.OptionsRelationshipGathering.FileRelationshipSettings.IncludeChildren = true;
            settings.OptionsRelationshipGathering.FileRelationshipSettings.RecurseChildren = true;
            settings.OptionsRelationshipGathering.FileRelationshipSettings.VersionGatheringOption = VDF.Vault.Currency.VersionGatheringOption.Latest;
            settings.OptionsRelationshipGathering.IncludeLinksSettings.IncludeLinks = false;
            VDF.Vault.Settings.AcquireFilesSettings.AcquireFileResolutionOptions mResOpt = new VDF.Vault.Settings.AcquireFilesSettings.AcquireFileResolutionOptions();
            mResOpt.OverwriteOption = VDF.Vault.Settings.AcquireFilesSettings.AcquireFileResolutionOptions.OverwriteOptions.ForceOverwriteAll;
            mResOpt.SyncWithRemoteSiteSetting = VDF.Vault.Settings.AcquireFilesSettings.SyncWithRemoteSite.Always;
            settings.AddFileToAcquire(mFileIt, settings.DefaultAcquisitionOption);
            VDF.Vault.Results.AcquireFilesResults results = conn.FileManager.AcquireFiles(settings);
            if (results != null)
            {
                try
                {
                    VDF.Vault.Results.FileAcquisitionResult mFilesDownloaded = results.FileResults.Last();
                    return mFilesDownloaded.LocalPath.FullPath.ToString();
                }
                catch (Exception)
                {
                    return "FileFoundButDownloadFailed";
                }
            }
            return "FileNotFound";
        }


        /// <summary>
        /// Get the file iteration's properties with Display Names and Values
        /// </summary>
        /// <param name="conn">Current Vault connection ($VaultConnection)</param>
        /// <param name="FileId">File iteration Id</param>
        /// <param name="FileProperties">Name-Value map of Display Name and Values. All Values return as text.</param>
        public void GetFileProps(Connection conn, long FileId, ref Dictionary<string, string> FileProperties)
        {
            PropDef[] mPropDefs = conn.WebServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId("FILE");
            PropInst[] mSourcePropInsts = conn.WebServiceManager.PropertyService.GetPropertiesByEntityIds("FILE", new long[] { FileId });
            string mPropDispName;
            string mPropVal;
            string mThumbnailDispName = mPropDefs.FirstOrDefault(n => n.SysName == "Thumbnail").DispName;
            foreach (PropInst mFilePropInst in mSourcePropInsts)
            {
                mPropDispName = mPropDefs.FirstOrDefault(n => n.Id == mFilePropInst.PropDefId).DispName;
                //filter thumbnail property
                if (mPropDispName != mThumbnailDispName)
                {
                    if (mFilePropInst.Val == null)
                    {
                        mPropVal = "";
                    }
                    else
                    {
                        mPropVal = mFilePropInst.Val.ToString();
                    }
                    FileProperties.Add(mPropDispName, mPropVal);
                }
            }
        }

        /// <summary>
        /// Get Folder properties with Display Names and Values
        /// </summary>
        /// <param name="conn">Current Vault connection ($VaultConnection)</param>
        /// <param name="FolderId">Folder Id</param>
        /// <param name="FolderProperties">Name-Value map of Display Name and Values. All Values return as text.</param>        
        public void GetFolderProps(Connection conn, long FolderId, ref Dictionary<string, string> FolderProperties)
        {
            PropDef[] mPropDefs = conn.WebServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId("FLDR");
            PropInst[] mSourcePropInsts = conn.WebServiceManager.PropertyService.GetPropertiesByEntityIds("FLDR", new long[] { FolderId });
            string mPropDispName;
            string mPropVal;

            foreach (PropInst mFilePropInst in mSourcePropInsts)
            {
                mPropDispName = mPropDefs.Where(n => n.Id == mFilePropInst.PropDefId).FirstOrDefault().DispName;

                if (mFilePropInst.Val == null)
                {
                    mPropVal = "";
                }
                else
                {
                    mPropVal = mFilePropInst.Val.ToString();
                }
                FolderProperties.Add(mPropDispName, mPropVal);
            }
        }


        /// <summary>
        /// Get Item properties with Display Names and Values
        /// </summary>
        /// <param name="conn">Current Vault connection ($VaultConnection)</param>
        /// <param name="ItemId">Item Id</param>
        /// <param name="ItemProperties">Name-Value map of Display Name and Values. All Values return as text.</param>
        public void GetItemProps(Connection conn, long ItemId, ref Dictionary<string, string> ItemProperties)
        {
            PropDef[] mPropDefs = conn.WebServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId("ITEM");
            PropInst[] mSourcePropInsts = conn.WebServiceManager.PropertyService.GetPropertiesByEntityIds("ITEM", new long[] { ItemId });
            string mPropDispName;
            string mPropVal;
            string mThumbnailDispName = mPropDefs.Where(n => n.SysName == "Thumbnail").FirstOrDefault().DispName;
            foreach (PropInst mFilePropInst in mSourcePropInsts)
            {
                mPropDispName = mPropDefs.Where(n => n.Id == mFilePropInst.PropDefId).FirstOrDefault().DispName;
                //filter thumbnail property
                if (mPropDispName != mThumbnailDispName)
                {
                    if (mFilePropInst.Val == null)
                    {
                        mPropVal = "";
                    }
                    else
                    {
                        mPropVal = mFilePropInst.Val.ToString();
                    }
                    ItemProperties.Add(mPropDispName, mPropVal);
                }
            }
        }

        /// <summary>
        /// Get Custom Object properties with Display Names and Values
        /// </summary>
        /// <param name="conn">Current Vault connection ($VaultConnection)</param>
        /// <param name="CustentId">Custom Object Id</param>
        /// <param name="CustentProperties">Name-Value map of Display Name and Values. All Values return as text.</param>

        public void GetCustentProps(Connection conn, long CustentId, ref Dictionary<string, string> CustentProperties)
        {
            PropDef[] mPropDefs = conn.WebServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId("CUSTENT");
            PropInst[] mSourcePropInsts = conn.WebServiceManager.PropertyService.GetPropertiesByEntityIds("CUSTENT", new long[] { CustentId });
            string mPropDispName;
            string mPropVal;
            string mThumbnailDispName = mPropDefs.Where(n => n.SysName == "Thumbnail").FirstOrDefault().DispName;
            foreach (PropInst mFilePropInst in mSourcePropInsts)
            {
                mPropDispName = mPropDefs.Where(n => n.Id == mFilePropInst.PropDefId).FirstOrDefault().DispName;
                //filter thumbnail property, as iLogic RuleArguments will fail reading it.
                if (mPropDispName != mThumbnailDispName)
                {
                    if (mFilePropInst.Val == null)
                    {
                        mPropVal = "";
                    }
                    else
                    {
                        mPropVal = mFilePropInst.Val.ToString();
                    }
                    CustentProperties.Add(mPropDispName, mPropVal);
                }
            }
        }

    }

    /// <summary>
    /// Class sharing options to interact with hosting Inventor session
    /// </summary>
    public class InvHelpers
    {
        static Inventor.Application m_Inv = null;
        Inventor.Document m_Doc = null;
        Inventor.DrawingDocument m_DrawDoc = null;
        Inventor.PresentationDocument m_IpnDoc = null;
        String m_ModelPath = null;
        static Inventor.CommandManager m_InvCmdMgr = null;

        [System.Runtime.InteropServices.DllImport("User32.dll", SetLastError = true)]
        internal static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        /// <summary>
        /// Retrieve property value of main view referenced model
        /// </summary>
        /// <param name="m_InvApp">Connect to the hosting instance of the VDS dialog</param>
        /// <param name="m_ViewModelFullName"></param>
        /// <param name="m_PropName">Display Name</param>
        /// <returns></returns>
        public object m_GetMainViewModelPropValue(object m_InvApp, String m_ViewModelFullName, String m_PropName)
        {
            try
            {
                m_Inv = (Inventor.Application)m_InvApp;
                m_Doc = m_Inv.Documents.Open(m_ViewModelFullName, false);
                foreach (PropertySet m_PropSet in m_Doc.PropertySets)
                {
                    foreach (Property m_Prop in m_PropSet)
                    {
                        if (m_Prop.Name == m_PropName)
                        {
                            return m_Prop.Value;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        /// <summary>
        /// Gets the 3D model (ipt/iam/ipn) linked to the main view of the current (active) drawing.
        /// Gets the 3D model (iam) linked to the main view of the current (active) presentation.
        /// </summary>
        /// <param name="m_InvApp">Running host (instance of Inventor) of calling VDS Dialog.</param>
        /// <returns>Returns the fullfilename (path and filename incl. extension) of the referenced model as string.</returns>
        public String m_GetMainViewModelPath(object m_InvApp)
        {
            try
            {
                m_Inv = (Inventor.Application)m_InvApp;

                if (m_Inv.ActiveDocumentType == DocumentTypeEnum.kDrawingDocumentObject)
                {
                    m_DrawDoc = (DrawingDocument)m_Inv.ActiveDocument;
                    Sheet m_Sheet = m_DrawDoc.ActiveSheet;
                    DrawingView m_DrwView = m_Sheet.DrawingViews[1];
                    if (!(m_DrwView is null))
                    {
                        m_ModelPath = m_DrwView.ReferencedFile.FullFileName;
                        return m_ModelPath;
                    }
                }

                if (m_Inv.ActiveDocumentType == DocumentTypeEnum.kPresentationDocumentObject)
                {
                    m_IpnDoc = (PresentationDocument)m_Inv.ActiveDocument;
                    if (m_IpnDoc.ReferencedDocuments.Count >= 1)
                    {
                        m_ModelPath = m_IpnDoc.ReferencedDocuments[1].FullDocumentName;
                        return m_ModelPath;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Delete orphaned drawing sheets. Sheet format consuming workflows likely cause an unused sheet1
        /// </summary>
        /// <param name="m_InvApp">Inventor Application ($Application)</param>
        /// <returns>false on unhandled errors, else true</returns>
        public bool m_RemoveOrphanedSheets(object m_InvApp)
        {
            try
            {
                m_Inv = (Inventor.Application)m_InvApp;

                if (m_Inv.ActiveDocumentType == DocumentTypeEnum.kDrawingDocumentObject)
                {
                    m_DrawDoc = (DrawingDocument)m_Inv.ActiveDocument;
                    foreach (Sheet sheet in m_DrawDoc.Sheets)
                    {
                        if (sheet.DrawingViews.Count == 0 && sheet != m_DrawDoc.ActiveSheet)
                        {
                            sheet.Delete(false);
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Return running Inventor application
        /// </summary>
        /// <returns></returns>
        public Inventor.Application m_InventorApplication()
        {
            // Try to get an active instance of Inventor
            try
            {
                return MarshalCore.GetActiveObject("Inventor.Application") as Inventor.Application;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// Return active Inventor document
        /// </summary>
        /// <param name="m_InvApp">Inventor Application ($Application)</param>
        /// <returns></returns>
        public string m_ActiveDocFullFileName(object m_InvApp)
        {
            m_Inv = (Inventor.Application)m_InvApp;
            if (m_Inv.ActiveDocument != null)
            {
                return m_Inv.ActiveDocument.FullFileName;
            }
            else
            {
                return null;
            }

        }


        /// <summary>
        /// Place component in active Inventor assembly document;
        /// </summary>
        /// <param name="m_InvApp"></param>
        /// <param name="m_CompFullFileName"></param>
        public static void m_PlaceComponent(object m_InvApp, String m_CompFullFileName)
        {
            m_Inv = (Inventor.Application)m_InvApp;
            Inventor.ControlDefinition m_InvCtrlDef = null;

            if (m_Inv != null)
            {
                m_InvCmdMgr = m_Inv.CommandManager;
                m_InvCmdMgr.PostPrivateEvent(PrivateEventTypeEnum.kFileNameEvent, m_CompFullFileName);

                // insert components IPT or IAM into active assembly document
                if (m_Inv.ActiveDocumentType == DocumentTypeEnum.kAssemblyDocumentObject)
                {
                    m_InvCtrlDef = (ControlDefinition)m_InvCmdMgr.ControlDefinitions["AssemblyPlaceComponentCmd"];
                }

                // insert components to part environments as derived components
                if (m_Inv.ActiveDocumentType == DocumentTypeEnum.kPartDocumentObject)
                {
                    m_InvCtrlDef = (ControlDefinition)m_InvCmdMgr.ControlDefinitions["PartDerivedComponentCmd"];
                }

                // insert components to presentation environments scene model
                if (m_Inv.ActiveDocumentType == DocumentTypeEnum.kPresentationDocumentObject)
                {
                    Inventor.PresentationDocument mPresDoc = (Inventor.PresentationDocument)m_Inv.ActiveDocument;
                    Inventor.PresentationComponent mSceneComp;
                    bool mSceneCompExists = false;

                    // check that the current scene model is not empty
                    try
                    {
                        mSceneComp = mPresDoc.ActiveScene.TopSceneComponent;
                        mSceneCompExists = true;
                    }
                    catch (Exception)
                    {
                        mSceneCompExists = false;
                    }
                    if (mSceneCompExists == false)
                    {
                        // insert the model into the current scene
                        m_InvCtrlDef = (ControlDefinition)m_InvCmdMgr.ControlDefinitions["PublisherInsertModelCmd"];
                    }
                    else
                    {
                        // insert the model into a new scene
                        m_InvCtrlDef = (ControlDefinition)m_InvCmdMgr.ControlDefinitions["PublisherCreatePublicationCmd"];
                    }                        
                }

                // insert components as new base view for drawing documents
                if (m_Inv.ActiveDocumentType == DocumentTypeEnum.kDrawingDocumentObject)
                {
                    m_InvCtrlDef = (ControlDefinition)m_InvCmdMgr.ControlDefinitions["DrawingBaseViewCmd"];
                }

                //bring Inventor to front and run the user command
                if (m_InvCtrlDef == null)
                {
                    try
                    {
                        IntPtr mWinPt = (IntPtr)m_Inv.MainFrameHWND;
                        SwitchToThisWindow(mWinPt, true);
                        m_InvCtrlDef.Execute();

                        // zoom all
                        m_InvCtrlDef = (ControlDefinition)m_InvCmdMgr.ControlDefinitions["AppSteeringWheelFitWindowCmd"];
                        m_InvCtrlDef.Execute();
                    }
                    catch (Exception ex)
                    {
                        // Vault AddIn will catch the exception and display it in the VDF Error dialog                        
                    }
                }
            }
        }

        /// <summary>
        /// validate active Factory Design Utility AddIn
        /// </summary>
        /// <param name="mInvApp">Inventor Application ($Application)</param>
        /// <returns></returns>
        public bool m_FDUActive(object mInvApp)
        {
            m_Inv = (Inventor.Application)mInvApp;
            try
            {
                ApplicationAddIn mFDUAddIn = m_Inv.ApplicationAddIns.get_ItemById("{031C8B05-13C0-4C6C-B8FD-5A19DACCB64F}");
                if (mFDUAddIn != null)
                {
                    if (mFDUAddIn.Activated)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Return FDU key/value pairs to identify Factory Layout or Factory Asset files
        /// </summary>
        /// <param name="m_InvApp">Inventor Application ($Application)</param>
        /// <param name="mFdsKeys">empty dictonary</param>
        /// <returns></returns>
        public Dictionary<string, string> m_GetFdsKeys(object m_InvApp, Dictionary<string, string> mFdsKeys)
        {
            try
            {
                m_Inv = (Inventor.Application)m_InvApp;
                m_Doc = m_Inv.ActiveDocument;
                if (m_Doc != null)
                {
                    if (m_Doc.DocumentInterests.HasInterest("factory.filetype.factory_layout_template"))
                    {
                        //FDS Type
                        mFdsKeys.Add("FdsType", "FDS-Layout");

                        //FDS Property Set exists for syncronized layouts
                        foreach (PropertySet m_PropSet in m_Doc.PropertySets)
                        {
                            if (m_PropSet.Name == "autodesk.factory.inventor.DwgInv")
                            {
                                foreach (Property m_Prop in m_PropSet)
                                {
                                    mFdsKeys.Add(m_Prop.Name, (string)m_Prop.Value);
                                }
                                //Get Fullname set by synchronization, to avoid save to other location
                                mFdsKeys.Add("FdsNewFullFileName", m_Doc.File.FullFileName);
                                System.IO.FileInfo mFdsFileInfo = new System.IO.FileInfo(m_Doc.File.FullFileName);
                                string mFdsPath = mFdsFileInfo.Directory.FullName;
                                mFdsKeys.Add("FdsNewPath", mFdsPath);
                            }
                        }
                    }
                    if (m_Doc.DocumentInterests.HasInterest("factory.filetype.factory_asset"))
                    {
                        mFdsKeys.Add("FdsType", "FDS-Asset");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mFdsKeys;
        }

        /// <summary>
        /// Return custom iPropertyset for AutoCAD files handled by Inventor FDU
        /// </summary>
        /// <param name="m_InvApp">Inventor Application ($Application)</param>
        /// <param name="mFdsKeys">empty Dictonary of String, String</param>
        /// <returns></returns>
        public Dictionary<string, string> m_GetFdsAcadProps(object m_InvApp, Dictionary<string, string> mFdsKeys)
        {
            Inventor.Document mDwgSource = null;
            DefaultNonInventorDWGFileOpenBehaviorEnum mUserOpenOpt = DefaultNonInventorDWGFileOpenBehaviorEnum.kRegularOpenNonInventorDWGFile;

            try
            {
                m_Inv = (Inventor.Application)m_InvApp;
                m_Doc = m_Inv.ActiveDocument;
                if (m_Doc.DocumentInterests.HasInterest("factory.filetype.factory_layout_template"))
                {
                    //FDS Type
                    mFdsKeys.Add("FdsType", "FDS-Layout");

                    //FDS Property Set exists for syncronized layouts
                    foreach (PropertySet m_PropSet in m_Doc.PropertySets)
                    {
                        if (m_PropSet.Name == "autodesk.factory.inventor.DwgInv")
                        {
                            foreach (Property m_Prop in m_PropSet)
                            {
                                mFdsKeys.Add(m_Prop.Name, (string)m_Prop.Value);
                            }

                            //Get Fullname set by synchronization, to avoid save to other location
                            mFdsKeys.Add("FdsNewFullFileName", m_Doc.File.FullFileName);
                            System.IO.FileInfo mFdsFileInfo = new System.IO.FileInfo(m_Doc.File.FullFileName);
                            string mFdsPath = mFdsFileInfo.Directory.FullName;
                            mFdsKeys.Add("FdsNewPath", mFdsPath);

                            if (m_Doc.FileSaveCounter >= 0) //if save counter = 0, the file is currently in the sync process; we must not open the sync source then.
                            {
                                //Open the source DWG to read properties;
                                try
                                {
                                    string mFdsSourceFullFileName = mFdsPath + "\\" + mFdsKeys["DwgFileName"];
                                    //read inventor application option to reset later
                                    mUserOpenOpt = m_Inv.DrawingOptions.DefaultNonInventorDWGFileOpenBehavior;
                                    m_Inv.DrawingOptions.DefaultNonInventorDWGFileOpenBehavior = DefaultNonInventorDWGFileOpenBehaviorEnum.kRegularOpenNonInventorDWGFile;
                                    mDwgSource = m_Inv.Documents.Open(mFdsSourceFullFileName, false);
                                    //Read the properties and add to dictionary if a value exists
                                    foreach (PropertySet m_TempPropSet in mDwgSource.PropertySets)
                                    {
                                        if (m_TempPropSet.DisplayName.Contains("Summary") || m_TempPropSet.DisplayName == "User Defined Properties")
                                        {
                                            foreach (Property m_TempProp in m_TempPropSet)
                                            {
                                                if (!string.IsNullOrEmpty((string)m_TempProp.Value))
                                                {
                                                    mFdsKeys.Add(m_TempProp.Name, (string)m_TempProp.Value);
                                                }
                                            }
                                        }
                                    }

                                }
                                catch (Exception)
                                {
                                    //throw;
                                }
                                finally
                                {
                                    mDwgSource.Close(true);
                                    //reset application option
                                    m_Inv.DrawingOptions.DefaultNonInventorDWGFileOpenBehavior = mUserOpenOpt;
                                }
                            }
                            else
                            {
                                mFdsKeys.Add("FdsAcadProps", "We can't retrieve properties before the calling file is saved.");
                            }
                        }
                    }
                }
                if (m_Doc.DocumentInterests.HasInterest("factory.filetype.factory_asset"))
                {
                    mFdsKeys.Add("FdsType", "FDS-Asset");
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mFdsKeys;
        }

    }

    /// <summary>
    /// /// Class sharing options to interact with hosting AutoCAD session
    /// </summary>
    public class AcadHelpers
    {
        AcInterop.AcadApplication mAcad = null;
        private const string progID = "AutoCAD.Application";
        AcInterop.AcadDocument mAcDoc = null;

        [System.Runtime.InteropServices.DllImport("User32.dll", SetLastError = true)]
        static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        /// <summary>
        /// Get AutoCAD session hosting; deprecated as VDS >2017 dialogs share the hosting application object
        /// </summary>
        /// <returns></returns>
        private Boolean m_ConnectAcad()
        {
            try
            {
                mAcad = MarshalCore.GetActiveObject("AutoCAD.Application") as AcInterop.AcadApplication;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check for FDS Blocks in AutoCAD drawings
        /// </summary>
        /// <param name="m_AcadApp">AutoCAD Application ($Application)</param>
        /// <returns>True for Blocknames containing "FDS"</returns>
        public Boolean mFdsDrawing(object m_AcadApp)
        {
            mAcad = (AcInterop.AcadApplication)m_AcadApp;
            mAcDoc = mAcad.ActiveDocument;
            AcInteropCom.AcadDatabase m_AcDB = (dynamic)mAcDoc.Database;
            AcInteropCom.AcadSummaryInfo m_AcSummInfo = m_AcDB.SummaryInfo;
            foreach (AcInteropCom.AcadBlock mBlock in mAcDoc.Blocks)
            {
                if (mBlock.Name.Contains("FDS"))
                {
                    return true;
                }
                ;
            }
            return false;
        }

        private Boolean mFdsDict(object m_AcadApp)
        {
            mAcad = (AcInterop.AcadApplication)m_AcadApp;
            mAcDoc = mAcad.ActiveDocument;
            AcInteropCom.AcadDatabase m_AcDB = mAcDoc.Database;

            return false;
        }


        /// <summary>
        /// Switch running AutoCAD application
        /// </summary>
        /// <param name="m_AcadApp">AutoCAD Application ($Application)</param>
        private void m_GoToAcad(object m_AcadApp)
        {
            try
            {
                mAcad = (AcInterop.AcadApplication)m_AcadApp;
                mAcDoc = mAcad.ActiveDocument;
                IntPtr mWinPt = (IntPtr)mAcad.HWND;
                SwitchToThisWindow(mWinPt, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

        }
    }
}

