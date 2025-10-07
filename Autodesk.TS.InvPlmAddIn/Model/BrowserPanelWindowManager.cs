using DevExpress.XtraSpellChecker.Rules;
using Inventor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;

namespace InvPlmAddIn.Model
{
    public class BrowserPanelWindowManager
    {
        private static Uri mBaseUri = InvPlmAddIn.InvPlmAddinSrv.mBaseUri;

        private HostObject AppContextHostObject;
        private List<DocumentPanelSet> panelSets = new List<DocumentPanelSet>();

        private List<BrowserPanelWindow> windows = new List<BrowserPanelWindow>();

        private static List<BrowserPanelWindow> windowsAppContext = new List<BrowserPanelWindow>();

        public BrowserPanelWindowManager(InvPlmAddIn.InvPlmAddinSrv addIn) => mAddIn = addIn;

        public static string mSelectionSender { get; set; } = "Inventor";
        public static DocumentPanelSet ActiveDocumentPanelSet { get; set; }
        private InvPlmAddIn.InvPlmAddinSrv mAddIn { get; set; }
        public bool IsLoggedIn { get; private set; } = false;

        private PanelOptions[] Options { get; set; } =
        {
            new PanelOptions
            {
                InternalName = "Item",
                WindowTitle = "Vault PLM Item",
                Url = mBaseUri.ToString() + "/item?number={PartNumber}&theme={Theme}" + "&host=Inventor"
            },

            new PanelOptions
            {
                InternalName = "Instances",
                WindowTitle = "Vault PLM Instance",
                Url = mBaseUri.ToString() + "/../asset-editor?number={PartNumber}&theme={Theme}" + "&host=Inventor"
            }

            //new PanelOptions
            //{
            //    InternalName = "Context",
            //    WindowTitle = "Vault PLM Context",
            //    Url = mBaseUri.ToString() + "/context?number={PartNumber}&theme={Theme}" + "&host=Inventor"
            //},
        };

        private PanelOptions[] OptionsAppConnect { get; set; } =
        {
            new PanelOptions
            {
                InternalName = "plmTasksWindow",
                WindowTitle = "Vault PLM Tasks",
                Url = mBaseUri.ToString() + "/tasks?&theme={Theme}" + "&host=Inventor"
            },
            
            //new PanelOptions
            //{
            //    InternalName = "plmNavigatorWindow",
            //    WindowTitle = "Vault PLM Navigator",
            //    Url = mBaseUri.ToString() + "/navigate?&theme={Theme}"
            //},

            new PanelOptions
            {
                InternalName = "plmSearchWindow",
                WindowTitle = "Vault PLM Search",
                Url = mBaseUri.ToString() + "/pdm-search?&theme={Theme}" + "&host=Inventor"
            }
        };

        public void CallILogic(string rulename)
            => mAddIn.CallILogic(rulename);

        public void CallILogic(string rulename, ref Dictionary<string, object> args)
            => mAddIn.CallILogic(rulename, ref args);

        public string GetDocPartNumber(Document document)
        {
            var propSet = document?.PropertySets["Design Tracking Properties"];
            var property = propSet?["Part Number"];
            return property?.Value?.ToString();
        }

        public void Init()
        {
            AppContextHostObject = new HostObject(this);
            foreach (var a in OptionsAppConnect)
            {
                var browserPanelWindow = new BrowserPanelWindow(a, InvPlmAddIn.InvPlmAddinSrv.ClientId, InvPlmAddinSrv.mInventorApplication);
                browserPanelWindow.SetWebViewHandler(new WebViewHandler(ReplaceUrlParameter(a.Url, ""), AppContextHostObject));
                windowsAppContext.Add(browserPanelWindow);
            }

            foreach (var a in Options)
            {
                windows.Add(new BrowserPanelWindow(a, InvPlmAddinSrv.ClientId, InvPlmAddinSrv.mInventorApplication));
            }
        }

        public void OnChangeDocument(_Document document)
        {
            mAddIn.CallILogic("OnDocumentOpen");

            SetActiveDocumentSet(document);
        }

        public void OnCloseDocument(_Document document)
        {
            mAddIn.CallILogic("OnDocumentClose");

            SetActiveDocumentSet(null);
            RemoveDocumentPanelSetOfDocument(document);
        }

        public void Remove()
        {
            windows.ForEach(x => x.Remove(InvPlmAddinSrv.mInventorApplication));
            windowsAppContext.ForEach(x => x.Remove(InvPlmAddinSrv.mInventorApplication));
        }

        public void SetActiveDocumentSet(Document document)
        {
            if (document == null)
            {
                return;
            }
            var docSet = GetDocumentPanelSetForDocument(document);

            if (docSet != ActiveDocumentPanelSet)
            {
                foreach (var window in windows)
                {
                    var webViewHandler = docSet.WebViewHandlers[window.Options.InternalName];
                    window.SetWebViewHandler(webViewHandler);
                }

                ActiveDocumentPanelSet = docSet;

                docSet.RefreshWebViewsAfterLoginIfNeeded();
            }
        }

        public void UserLoggedIn()
        {
            if (!IsLoggedIn)
            {
                IsLoggedIn = true;

                UpdateAppContectBrowserWindows();
                ActiveDocumentPanelSet?.RefreshWebViewsAfterLoginIfNeeded();
            }
        }

        private void DocumentEvents_OnChangeSelectSet(EventTimingEnum BeforeOrAfter, NameValueMap Context, out HandlingCodeEnum handlingCode)
        {
            SelectSet mSelectSet = null;
            handlingCode = HandlingCodeEnum.kEventNotHandled;
            var application = InvPlmAddinSrv.mInventorApplication;

            if (BeforeOrAfter == EventTimingEnum.kAfter && application.ActiveDocument?.SelectSet != null)
            {
                mSelectSet = application.ActiveDocument.SelectSet;
                List<string> mSelectedPartNumbers = new List<string>();
                List<string> mSelectedBodyNames = new List<string>();

                //forward the selection to the PLM view if the event hasn't been initiated from there
                if (mSelectionSender == "Inventor")
                {
                    //relevant selections are occurrences for assemblies and bodies for parts
                    if (application.ActiveDocumentType == Inventor.DocumentTypeEnum.kAssemblyDocumentObject)
                    {
                        foreach (var item in mSelectSet)
                        {
                            if (item is Inventor.ComponentOccurrence)
                            {
                                ComponentOccurrence component = (ComponentOccurrence)item;
                                mSelectedPartNumbers.Add(GetDocPartNumber((Document)component.Definition.Document));
                            }
                        }
                        string mNumbers = System.Text.Json.JsonSerializer.Serialize((List<string>)mSelectedPartNumbers).ToString();
                        GetPlmItemWindow().ExecutePlmSelectItem(mNumbers);
                    }

                    //Body selection; we differentiate selection of 'material-assigned' bodies and others. 'material-assigned' body names start with a partnumber 'CAD_'
                    if (application.ActiveDocumentType == Inventor.DocumentTypeEnum.kPartDocumentObject)
                    {
                        foreach (var item in mSelectSet)
                        {
                            if (item is Inventor.SurfaceBody)
                            {
                                SurfaceBody surfaceBody = (SurfaceBody)item;
                                string bodyPartNum = GetBodyPartNumber(surfaceBody.Name);
                                if (bodyPartNum != null)
                                {
                                    mSelectedPartNumbers.Add(bodyPartNum);
                                }
                            }
                        }
                        if (mSelectedPartNumbers.Count > 0)
                        {
                            string mNumbers = System.Text.Json.JsonSerializer.Serialize((List<string>)mSelectedPartNumbers).ToString();
                            GetPlmItemWindow().ExecutePlmSelectItem(mNumbers);
                            return;
                        }
                    }
                }

            }
        }

        private string GetBodyPartNumber(string BodyName)
        {
            //the name convention is: Pos_Partnumber_Title_LxBxH
            string[] NameSubStrings = BodyName.Split('_');
            if (NameSubStrings.Length >= 3)
            {
                return NameSubStrings[1] + "_" + NameSubStrings[2];
            }
            return null;
        }

        private DocumentPanelSet GetDocumentPanelSetForDocument(Document document)
        {
            if (document == null)
                return null;

            var partNumber = GetDocPartNumber(document);
            var docSet = panelSets.FirstOrDefault(p => p.PartNumber == partNumber);
            if (docSet == null)
            {
                panelSets.Add(docSet = new DocumentPanelSet(this, Options, partNumber)
                {
                    ArePagesRefreshedAfterLogin = IsLoggedIn
                });
                if (document != null)
                {

                }

                //toDo: bind event registration to a future option/setting of the addin; for now, we decided to turn off and actively grab a selection
                //docSet.DocumentsEvents = document.DocumentEvents;
                //docSet.DocumentsEvents.OnChangeSelectSet += DocumentEvents_OnChangeSelectSet;
            }
            return docSet;
        }

        private BrowserPanelWindow GetPlmItemWindow() => windows.First(x => x.Options.InternalName == "Item");

        private void RemoveDocumentPanelSetOfDocument(Document document)
        {
            if (document == null)
                return;

            document.DocumentEvents.OnChangeSelectSet -= DocumentEvents_OnChangeSelectSet;

            var partNumber = GetDocPartNumber(document);
            var docSet = panelSets.FirstOrDefault(p => p.PartNumber == partNumber);
            if (docSet != null)
            {
                //todo: investigate if we need to remove the event handler, there is a risk that it does not exist anymore
                try
                {
                    docSet.DocumentsEvents.OnChangeSelectSet -= DocumentEvents_OnChangeSelectSet;
                }
                catch (Exception)
                { }

                panelSets.Remove(docSet);
                docSet.Dispose();
            }
        }

        private string ReplaceUrlParameter(string url, string partNumber)
                                        => url
            .Replace("{BaseUrl}", mBaseUri.ToString())
            .Replace(DocumentPanelSet.PartNumberParameter, partNumber)
            .Replace("{Theme}", InvPlmAddinSrv.mInventorApplication.ThemeManager.ActiveTheme.Name.Replace("Theme", ""));

        private void UpdateAppContectBrowserWindows()
        {
            foreach (var a in windowsAppContext)
                a.GetWebViewHandler().LoadUrl();
        }

        public void DeactivateByInventorAddIn()
        {
            foreach (var a in panelSets)
            {
                if (a.DocumentsEvents != null)
                    a.DocumentsEvents.OnChangeSelectSet -= DocumentEvents_OnChangeSelectSet;
            }
            panelSets.Clear();
        }

        public static void SendMessage(string message)
        {
            foreach (var a in windowsAppContext)
                a.GetWebViewHandler().WebView_SendMessage(message);
            ActiveDocumentPanelSet?.WebViewHandlers.Values.ToList().ForEach(x => x.WebView_SendMessage(message));
        }
    }
}
