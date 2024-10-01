using DevExpress.XtraEditors;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using VDF = Autodesk.DataManagement.Client.Framework;
using DevExpress.Utils;
using System.Diagnostics;
using Inventor;

namespace InvPlmAddIn.Forms
{
    public partial class mDockWindowChild : DevExpress.XtraEditors.XtraUserControl, IDisposable
    {
        private static Utils.Settings mAddinSettings = new Utils.Settings();
        private static Uri mBaseUri = null;

        public mDockWindowChild(string currentTheme, string internalName)
        {
            InitializeComponent();

            mAddinSettings = Utils.Settings.Load();
            mBaseUri = new Uri(mAddinSettings.FmExtensionUrl);
            Debug.Print("mBaseUri: " + mBaseUri.ToString());
            ApplyThemes(currentTheme);

            mWebViewInit(internalName);
        }

        private void ApplyThemes(string currentTheme)
        {
            if (string.IsNullOrEmpty(currentTheme))
                return;

            if (currentTheme == VDF.Forms.SkinUtils.Theme.Light.ToString())
                LookAndFeel.SetSkinStyle(VDF.Forms.SkinUtils.CustomThemeSkins.LightThemeName);
            else if (currentTheme == VDF.Forms.SkinUtils.Theme.Dark.ToString())
                LookAndFeel.SetSkinStyle(VDF.Forms.SkinUtils.CustomThemeSkins.DarkThemeName);
        }

        void mWebViewInit(string internalName)
        {
            //ensure the webview2 is loaded completely
            var frame = new DispatcherFrame();
            var env = CoreWebView2Environment.CreateAsync(null, System.Environment.GetEnvironmentVariable("TEMP"), null);

            using (var task = webView21.EnsureCoreWebView2Async(env.Result))
            {
                task.ContinueWith((dummy) => frame.Continue = false);
                frame.Continue = true;
                Dispatcher.PushFrame(frame);
            }

            String mUrl = null;
            Uri uri = null;
            Uri uriRel = null;
            switch (internalName)
            {
                case Autodesk.TS.InvPlmAddIn.StandardAddInServer.mSearchWinName:
                    uriRel = new Uri("/addins/search", UriKind.Relative);
                    Debug.Print("mUrl: " + mUrl);
                    uri = new Uri(mBaseUri, uriRel);
                    Debug.Print("uri: " + uri.ToString());
                    break;
                case Autodesk.TS.InvPlmAddIn.StandardAddInServer.mTasksWinName:
                    uriRel = new Uri("/addins/tasks", UriKind.Relative);
                    Debug.Print("mUrl: " + mUrl);
                    uri = new Uri(mBaseUri, uriRel);
                    Debug.Print("uri: " + uri.ToString());
                    break;
                case Autodesk.TS.InvPlmAddIn.StandardAddInServer.mNavigatorWinName:
                    uriRel = new Uri("/addins/navigate", UriKind.Relative);
                    Debug.Print("mUrl: " + mUrl);
                    uri = new Uri(mBaseUri, uriRel);
                    Debug.Print("uri: " + uri.ToString());
                    break;
                default:
                    break;
            }

            if (uri != null)
            {
                webView21.Source = uri;
            }
            else
            {
                mUrl = System.IO.Path.Combine(Utils.Util.GetAssemblyPath(), "/Forms/plmExtensionAddInError.html");
                webView21.CoreWebView2.NavigateToString(System.IO.File.ReadAllText(mUrl));
            }
        }
    }
}
