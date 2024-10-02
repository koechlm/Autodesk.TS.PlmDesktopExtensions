using DevExpress.XtraEditors;
using DevExpress.XtraSpellChecker.Rules;
using InvPlmAddIn.Model;
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
    /// <summary>
    /// Login dialog for the Fusion Manage Extension.
    /// A successful login will close the dialog and continue with the Addin activation, or cancel the Addin activation if the login fails or the user cancels the dialog.
    /// </summary>
    public partial class PlmExtensionLogin : DevExpress.XtraEditors.XtraForm
    {
        private static WebView2 webView;

        public PlmExtensionLogin(string currentTheme)
        {
            InitializeComponent();

            ApplyThemes(currentTheme);

            // Set the webview handler
            webView = new WebView2();
            this.panelControl1.Controls.Add(webView);
            webView.Dock = DockStyle.Fill;

            mWebViewInit(webView);
        }

        /// <summary>
        /// Initialize the webview control; it will share the Autodesk Account login with all other webviews of the Addin
        /// </summary>
        /// <param name="webView21"></param>
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
        }

        /// <summary>
        /// Push the Inventor theme to the login dialog
        /// </summary>
        /// <param name="currentTheme"></param>
        private void ApplyThemes(string currentTheme)
        {
            if (string.IsNullOrEmpty(currentTheme))
                return;

            if (currentTheme == VDF.Forms.SkinUtils.Theme.Light.ToString())
                LookAndFeel.SetSkinStyle(VDF.Forms.SkinUtils.CustomThemeSkins.LightThemeName);
            else if (currentTheme == VDF.Forms.SkinUtils.Theme.Dark.ToString())
                LookAndFeel.SetSkinStyle(VDF.Forms.SkinUtils.CustomThemeSkins.DarkThemeName);
        }

        /// <summary>
        /// The dialog is shown, navigate to the Autodesk Account login page
        /// It is a re-direction from the Fusion Manage Extension login page to the Autodesk Account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlmExtensionLogin_Shown(object sender, EventArgs e)
        {
            Uri uriRel = new Uri("/addins/login", UriKind.Relative);
            Uri uri = new Uri(InvPlmAddinSrv.mBaseUri, uriRel);

            webView.CoreWebView2.Navigate(uri.AbsoluteUri);

            //add event handler to get feedback from the webview as the login succeeded
            webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
        }

        /// <summary>
        /// The event handler to auto-close the login dialog when the login is successful
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
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