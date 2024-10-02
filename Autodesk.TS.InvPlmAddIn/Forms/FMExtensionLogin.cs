using DevExpress.XtraEditors;
using DevExpress.XtraSpellChecker.Rules;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;
using VDF = Autodesk.DataManagement.Client.Framework;

namespace InvPlmAddIn.Forms
{
    public partial class FMExtensionLogin : DevExpress.XtraEditors.XtraForm
    {


        public FMExtensionLogin(string currentTheme)
        {
            InitializeComponent();

            ApplyThemes(currentTheme);

            // Set the webview handler
            WebView2 webView = new WebView2();
            this.panelControl1.Controls.Add(webView);
            webView.Dock = DockStyle.Fill;

            mWebViewInit(webView);

        }

        private void mWebViewInit(WebView2 webView21)
        {
            var frame = new DispatcherFrame();
            var env = CoreWebView2Environment.CreateAsync(null, System.Environment.GetEnvironmentVariable("TEMP"), null);

            using (var task = webView21.EnsureCoreWebView2Async(env.Result))
            {
                task.ContinueWith((dummy) => frame.Continue = false);
                frame.Continue = true;
                Dispatcher.PushFrame(frame);
            }

            Uri uriRel = new Uri("/addins/tasks", UriKind.Relative);
            
            Uri uri = new Uri(InvPlmAddinSrv.mBaseUri, uriRel);

            webView21.CoreWebView2.Navigate(uri.AbsoluteUri);
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
    }
}