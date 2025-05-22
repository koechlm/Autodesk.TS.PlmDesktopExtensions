using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Windows.Threading;
using Autodesk.TS.VltPlmAddIn.Model;

namespace Autodesk.TS.VltPlmAddIn.Forms
{
    public partial class XtraFormFmLogin : DevExpress.XtraEditors.XtraForm
    {
        string mCurrentTheme = "light";

        public XtraFormFmLogin(string currentTheme)
        {
            InitializeComponent();

            ApplyThemes(currentTheme);

            InitializeWebView();
        }

        private void ApplyThemes(string currentTheme)
        {
            // Apply the theme to the form and its controls
            mCurrentTheme = currentTheme;
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(currentTheme);
            this.LookAndFeel.SetSkinStyle(currentTheme);
        }


        private void InitializeWebView()
        {
            var frame = new DispatcherFrame();
            string userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Adsk.TS.Vault-FM-Panels");
            var env = CoreWebView2Environment.CreateAsync(null, userDataFolder, null);
            using (var task = WebViewFmLogin.EnsureCoreWebView2Async(env.Result))
            {
                task.ContinueWith((dummy) => frame.Continue = false);
                frame.Continue = true;
                Dispatcher.PushFrame(frame);
            }
        }

        private void XtraFormFmLogin_Shown(object sender, EventArgs e)
        {
            string mUrl = VaultExplorerExtension.mFmExtensionUrl + "/login?" + "&theme=" + mCurrentTheme.ToLower();

            Uri uri = new Uri(mUrl, System.UriKind.Absolute);

            WebViewFmLogin.Source = uri;

            WebViewFmLogin.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;

            // To get the focus, simulate a click on the WebView control by invoking a JavaScript click event
            // WebViewFmLogin.CoreWebView2.ExecuteScriptAsync("document.body.click();");
        }

        // The event handler to auto-close the login dialog when the login is successful
        private void CoreWebView2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string message = e.TryGetWebMessageAsString();
            if (message == "Login successful")
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}