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
using ACW = Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.WebServicesTools;
using VDF = Autodesk.DataManagement.Client.Framework;
using ACET = Autodesk.Connectivity.Explorer.ExtensibilityTools;
using Inventor;
using DevExpress.Data.Filtering.Helpers;


namespace Autodesk.TS.VltPlmAddIn.Utils
{
    /// <summary>
    /// Class sharing options to interact with hosting Inventor session
    /// </summary>
    internal partial class InventorInteraction
    {
        private static Inventor.Application InvApp = null;
        private static Inventor.Document InvDoc = null;
        private static Inventor.DrawingDocument InvDrwDoc = null;
        private static Inventor.PresentationDocument InvIpnDoc = null;
        private static string InvDocFileModelPath = null;
        private static Inventor.CommandManager InvCmdMgr = null;

        [System.Runtime.InteropServices.DllImport("User32.dll", SetLastError = true)]
        internal static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        /// <summary>
        /// Retrieve property value of main view referenced model
        /// </summary>
        /// <param name="InventorApplication">Connect to the hosting instance of the VDS dialog</param>
        /// <param name="ViewModelFullName"></param>
        /// <param name="PropName">Display Name</param>
        /// <returns></returns>
        internal static object GetMainViewModelPropValue(object InventorApplication, String ViewModelFullName, String PropName)
        {
            try
            {
                InvApp = (Inventor.Application)InventorApplication;
                InvDoc = InvApp.Documents.Open(ViewModelFullName, false);
                foreach (PropertySet m_PropSet in InvDoc.PropertySets)
                {
                    foreach (Property m_Prop in m_PropSet)
                    {
                        if (m_Prop.Name == PropName)
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
        /// <param name="InventorApplication">Running host (instance of Inventor) of calling VDS Dialog.</param>
        /// <returns>Returns the fullfilename (path and filename incl. extension) of the referenced model as string.</returns>
        internal static String GetMainViewModelPath(object InventorApplication)
        {
            try
            {
                InvApp = (Inventor.Application)InventorApplication;

                if (InvApp.ActiveDocumentType == DocumentTypeEnum.kDrawingDocumentObject)
                {
                    InvDrwDoc = (DrawingDocument)InvApp.ActiveDocument;
                    Sheet m_Sheet = InvDrwDoc.ActiveSheet;
                    DrawingView m_DrwView = m_Sheet.DrawingViews[1];
                    if (!(m_DrwView is null))
                    {
                        InvDocFileModelPath = m_DrwView.ReferencedFile.FullFileName;
                        return InvDocFileModelPath;
                    }
                }

                if (InvApp.ActiveDocumentType == DocumentTypeEnum.kPresentationDocumentObject)
                {
                    InvIpnDoc = (PresentationDocument)InvApp.ActiveDocument;
                    if (InvIpnDoc.ReferencedDocuments.Count >= 1)
                    {
                        InvDocFileModelPath = InvIpnDoc.ReferencedDocuments[1].FullDocumentName;
                        return InvDocFileModelPath;
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
        /// <param name="InventorApplication">Inventor Application ($Application)</param>
        /// <returns>false on unhandled errors, else true</returns>
        internal static bool RemoveOrphanedSheets(object InventorApplication)
        {
            try
            {
                InvApp = (Inventor.Application)InventorApplication;

                if (InvApp.ActiveDocumentType == DocumentTypeEnum.kDrawingDocumentObject)
                {
                    InvDrwDoc = (DrawingDocument)InvApp.ActiveDocument;
                    foreach (Sheet sheet in InvDrwDoc.Sheets)
                    {
                        if (sheet.DrawingViews.Count == 0 && sheet != InvDrwDoc.ActiveSheet)
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
        internal static Inventor.Application GetInventorApplication()
        {
            // Try to get an active instance of Inventor
            try
            {
                return MarshalCore.GetActiveObject("Inventor.Application") as Inventor.Application;
            }
            catch
            {
                VDF.Forms.Library.ShowWarning("Inventor is not running. Please start Inventor with target file open to insert the selected model state", "Model States Panel - Insert into CAD", VDF.Forms.Currency.ButtonConfiguration.Ok);
                return null;
            }
        }


        /// <summary>
        /// Return active Inventor document
        /// </summary>
        /// <param name="InventorApplication">Inventor Application ($Application)</param>
        /// <returns></returns>
        internal static string ActiveDocFullFileName(object InventorApplication)
        {
            InvApp = (Inventor.Application)InventorApplication;
            if (InvApp.ActiveDocument != null)
            {
                return InvApp.ActiveDocument.FullFileName;
            }
            else
            {
                return null;
            }

        }


        internal static void OpenComponentFile(ACW.File file, string modelState = "[Primary]")
        {
            InvApp = InventorInteraction.GetInventorApplication();
            string CompFullFileName = null;
            if (InvApp != null)
            {
                // switch to Inventor window
                IntPtr mWinPt = (IntPtr)InvApp.MainFrameHWND;
                InventorInteraction.SwitchToThisWindow(mWinPt, true);
                InvApp.StatusBarText = "Downloading component from Vault...";

                // download the file from Vault
                CompFullFileName = VaultUtils.DownloadFiles(new List<VDF.Vault.Currency.Entities.FileIteration> { new VDF.Vault.Currency.Entities.FileIteration(VaultExplorerExtension.conn, file) })?.FirstOrDefault();

                if (string.IsNullOrEmpty(CompFullFileName))
                {
                    VDF.Forms.Library.ShowError("Failed to download the selected file from Vault.", "Model States Panel | Inventor Action - Open Component");
                    return;
                }

                try
                {
                    // create name/value map for model state selection
                    Inventor.NameValueMap openOptions = InvApp.TransientObjects.CreateNameValueMap();
                    openOptions.Add("ModelState", modelState);
                    InvApp.StatusBarText = "Opening component in Inventor...";
                    InvApp.Documents.OpenWithOptions(CompFullFileName, openOptions, true);

                }
                catch (Exception ex)
                {
                    VDF.Forms.Library.ShowError(ex, "Model States Panel | Inventor Action - Open Component");
                }
                finally
                {
                    InvApp.StatusBarText = "Ready";
                }

            }
            else
            {
                VDF.Forms.Library.ShowWarning("Inventor is not running. Please start Inventor to open the selected model state.", "Model States Panel - Open Component", VDF.Forms.Currency.ButtonConfiguration.Ok);
            }
        }


        /// <summary>
        /// Place component in active Inventor assembly document;
        /// </summary>
        /// <param name="file">WebServices.File</param>
        /// <param name="modelState">default value = [Primary]</param>
        internal static void InsertComponentFile(ACW.File file, string modelState = "[Primary]")
        {
            InvApp = InventorInteraction.GetInventorApplication();

            // inserting a component requires a open document of type part, assembly, drawing or presentation
            List<DocumentTypeEnum> validDocTypes = new List<DocumentTypeEnum>
            {
                DocumentTypeEnum.kAssemblyDocumentObject,
                DocumentTypeEnum.kPartDocumentObject,
                DocumentTypeEnum.kDrawingDocumentObject,
                DocumentTypeEnum.kPresentationDocumentObject
            };
            if (InvApp.Documents.Count == 0 || !validDocTypes.Contains(InvApp.ActiveDocumentType))
            {
                VDF.Forms.Library.ShowWarning("Please open an assembly, part, drawing or presentation document to insert the selected model state.", "Model States Panel - Insert into CAD", VDF.Forms.Currency.ButtonConfiguration.Ok);
                return;
            }

            Inventor.ControlDefinition InvCtrlDef = null;
            string CompFullFileName = null;

            if (InvApp != null)
            {
                IntPtr mWinPt = (IntPtr)InvApp.MainFrameHWND;
                InventorInteraction.SwitchToThisWindow(mWinPt, true);
                InvApp.StatusBarText = "Downloading component from Vault...";

                // download the file from Vault
                CompFullFileName = VaultUtils.DownloadFiles(new List<VDF.Vault.Currency.Entities.FileIteration> { new VDF.Vault.Currency.Entities.FileIteration(VaultExplorerExtension.conn, file) })?.FirstOrDefault();

                if (string.IsNullOrEmpty(CompFullFileName))
                {
                    VDF.Forms.Library.ShowError("Failed to download the selected file from Vault.", "Model States Panel | Inventor Action - Open Component");
                    return;
                }

                InvCmdMgr = InvApp.CommandManager;
                InvCmdMgr.ClearPrivateEvents();
                InvCmdMgr.PostPrivateEvent(PrivateEventTypeEnum.kFileNameEvent, CompFullFileName);
                InvCmdMgr.PostPrivateEvent(PrivateEventTypeEnum.kStringEvent, "Model State|" + modelState);


                // insert components IPT or IAM into active assembly document
                if (InvApp.ActiveDocumentType == DocumentTypeEnum.kAssemblyDocumentObject)
                {
                    InvApp.StatusBarText = "Inserting component into assembly...";
                    InvCtrlDef = (ControlDefinition)InvCmdMgr.ControlDefinitions["AssemblyPlaceComponentCmd"];
                }
                else
                {
                    // inform the user that this command targets Inventor assemblies only
                    VDF.Forms.Library.ShowWarning("Inserting model states is only supported for active Inventor assembly documents. Please open an assembly document to insert the selected model state.", "Model States Panel - Insert into CAD", VDF.Forms.Currency.ButtonConfiguration.Ok);
                    return;
                }

                // insert components to part environments as derived components
                if (InvApp.ActiveDocumentType == DocumentTypeEnum.kPartDocumentObject)
                {
                    InvCtrlDef = (ControlDefinition)InvCmdMgr.ControlDefinitions["PartDerivedComponentCmd"];
                }

                //insert components to presentation environments scene model
                if (InvApp.ActiveDocumentType == DocumentTypeEnum.kPresentationDocumentObject)
                {
                    Inventor.PresentationDocument mPresDoc = (Inventor.PresentationDocument)InvApp.ActiveDocument;
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
                        // insert the model into the current scene; user interaction requires to select a component from local disk
                        InvCtrlDef = (ControlDefinition)InvCmdMgr.ControlDefinitions["PublisherInsertModelCmd"];
                    }
                    else
                    {
                        // insert the model into a new scene
                        InvCtrlDef = (ControlDefinition)InvCmdMgr.ControlDefinitions["PublisherCreatePublicationCmd"];
                    }
                }

                // insert components as new base view for drawing documents
                if (InvApp.ActiveDocumentType == DocumentTypeEnum.kDrawingDocumentObject)
                {
                    InvCtrlDef = (ControlDefinition)InvCmdMgr.ControlDefinitions["DrawingBaseViewCmd"];
                }

                // run the user command
                if (InvCtrlDef != null)
                {
                    try
                    {
                        InvCtrlDef.Execute2(true);

                        // zoom all
                        InvCtrlDef = (ControlDefinition)InvCmdMgr.ControlDefinitions["AppSteeringWheelFitWindowCmd"];
                        InvCtrlDef.Execute();
                    }
                    catch (Exception ex)
                    {
                        VDF.Forms.Library.ShowError(ex, "Model States Panel | Inventor Action - Unhandled Exception");
                    }
                }

                InvApp.StatusBarText = "Ready";
            }

        }


        /// <summary>
        /// validate active Factory Design Utility AddIn
        /// </summary>
        /// <param name="mInvApp">Inventor Application ($Application)</param>
        /// <returns></returns>
        internal static bool FduIsActive(object mInvApp)
        {
            InvApp = (Inventor.Application)mInvApp;
            try
            {
                ApplicationAddIn mFDUAddIn = InvApp.ApplicationAddIns.get_ItemById("{031C8B05-13C0-4C6C-B8FD-5A19DACCB64F}");
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
        /// <param name="InventorApplication">Inventor Application ($Application)</param>
        /// <param name="mFdsKeys">empty dictonary</param>
        /// <returns></returns>
        internal static Dictionary<string, string> GetFdsKeys(object InventorApplication, Dictionary<string, string> mFdsKeys)
        {
            try
            {
                InvApp = (Inventor.Application)InventorApplication;
                InvDoc = InvApp.ActiveDocument;
                if (InvDoc != null)
                {
                    if (InvDoc.DocumentInterests.HasInterest("factory.filetype.factory_layout_template"))
                    {
                        //FDS Type
                        mFdsKeys.Add("FdsType", "FDS-Layout");

                        //FDS Property Set exists for syncronized layouts
                        foreach (PropertySet m_PropSet in InvDoc.PropertySets)
                        {
                            if (m_PropSet.Name == "autodesk.factory.inventor.DwgInv")
                            {
                                foreach (Property m_Prop in m_PropSet)
                                {
                                    mFdsKeys.Add(m_Prop.Name, (string)m_Prop.Value);
                                }
                                //Get Fullname set by synchronization, to avoid save to other location
                                mFdsKeys.Add("FdsNewFullFileName", InvDoc.File.FullFileName);
                                System.IO.FileInfo mFdsFileInfo = new System.IO.FileInfo(InvDoc.File.FullFileName);
                                string mFdsPath = mFdsFileInfo.Directory.FullName;
                                mFdsKeys.Add("FdsNewPath", mFdsPath);
                            }
                        }
                    }
                    if (InvDoc.DocumentInterests.HasInterest("factory.filetype.factory_asset"))
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
        /// <param name="InventorApplication">Inventor Application ($Application)</param>
        /// <param name="mFdsKeys">empty Dictonary of String, String</param>
        /// <returns></returns>
        internal static Dictionary<string, string> GetFdsAcadProps(object InventorApplication, Dictionary<string, string> mFdsKeys)
        {
            Inventor.Document mDwgSource = null;
            DefaultNonInventorDWGFileOpenBehaviorEnum mUserOpenOpt = DefaultNonInventorDWGFileOpenBehaviorEnum.kRegularOpenNonInventorDWGFile;

            try
            {
                InvApp = (Inventor.Application)InventorApplication;
                InvDoc = InvApp.ActiveDocument;
                if (InvDoc.DocumentInterests.HasInterest("factory.filetype.factory_layout_template"))
                {
                    //FDS Type
                    mFdsKeys.Add("FdsType", "FDS-Layout");

                    //FDS Property Set exists for syncronized layouts
                    foreach (PropertySet m_PropSet in InvDoc.PropertySets)
                    {
                        if (m_PropSet.Name == "autodesk.factory.inventor.DwgInv")
                        {
                            foreach (Property m_Prop in m_PropSet)
                            {
                                mFdsKeys.Add(m_Prop.Name, (string)m_Prop.Value);
                            }

                            //Get Fullname set by synchronization, to avoid save to other location
                            mFdsKeys.Add("FdsNewFullFileName", InvDoc.File.FullFileName);
                            System.IO.FileInfo mFdsFileInfo = new System.IO.FileInfo(InvDoc.File.FullFileName);
                            string mFdsPath = mFdsFileInfo.Directory.FullName;
                            mFdsKeys.Add("FdsNewPath", mFdsPath);

                            if (InvDoc.FileSaveCounter >= 0) //if save counter = 0, the file is currently in the sync process; we must not open the sync source then.
                            {
                                //Open the source DWG to read properties;
                                try
                                {
                                    string mFdsSourceFullFileName = mFdsPath + "\\" + mFdsKeys["DwgFileName"];
                                    //read inventor application option to reset later
                                    mUserOpenOpt = InvApp.DrawingOptions.DefaultNonInventorDWGFileOpenBehavior;
                                    InvApp.DrawingOptions.DefaultNonInventorDWGFileOpenBehavior = DefaultNonInventorDWGFileOpenBehaviorEnum.kRegularOpenNonInventorDWGFile;
                                    mDwgSource = InvApp.Documents.Open(mFdsSourceFullFileName, false);
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
                                    InvApp.DrawingOptions.DefaultNonInventorDWGFileOpenBehavior = mUserOpenOpt;
                                }
                            }
                            else
                            {
                                mFdsKeys.Add("FdsAcadProps", "We can't retrieve properties before the calling file is saved.");
                            }
                        }
                    }
                }
                if (InvDoc.DocumentInterests.HasInterest("factory.filetype.factory_asset"))
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
}

