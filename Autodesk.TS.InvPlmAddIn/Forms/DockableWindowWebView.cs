using DevExpress.XtraEditors;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using VDF = Autodesk.DataManagement.Client.Framework;

namespace InvPlmAddIn.Forms
{
    public partial class mDockWindowChild : DevExpress.XtraEditors.XtraUserControl
    {
        private static Utils.Settings mAddinSettings = new Utils.Settings();

        public mDockWindowChild(string currentTheme, string internalName)
        {
            InitializeComponent();

            mAddinSettings = Utils.Settings.Load();

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
            var env = CoreWebView2Environment.CreateAsync(null, Environment.GetEnvironmentVariable("TEMP"), null);

            using (var task = webView21.EnsureCoreWebView2Async(env.Result))
            {
                task.ContinueWith((dummy) => frame.Continue = false);
                frame.Continue = true;
                Dispatcher.PushFrame(frame);
            }

            String mUrl = null;
            switch (internalName)
            {
                case Autodesk.TS.InvPlmAddIn.StandardAddInServer.mSearchWinName:
                    mUrl = Path.Combine(mAddinSettings.FmExtensionUrl, "/search");
                    break;
                case Autodesk.TS.InvPlmAddIn.StandardAddInServer.mTasksWinName:
                    mUrl = Path.Combine(mAddinSettings.FmExtensionUrl, "/tasks");
                    break;
                case Autodesk.TS.InvPlmAddIn.StandardAddInServer.mNavigatorWinName:
                    mUrl = Path.Combine(mAddinSettings.FmExtensionUrl, "/navigator");
                    break;
                default:
                    break;
            }

            if (mUrl != null)
            {
                mNavigate(mUrl);
            }
            else
            {
                mUrl = Path.Combine(Utils.Util.GetAssemblyPath(), "/Forms/plmExtensionAddInError.html");
                mNavigate(mUrl);
            }            

        }
        

        private void mNavigate(string mUrl)
        {
            //read the property of the corresponding project
            Uri uri = new Uri(mUrl, System.UriKind.Absolute);
            webView21.Source = uri;
        }
    }
}
