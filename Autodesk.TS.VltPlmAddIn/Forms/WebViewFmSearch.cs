using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.Windows.Threading;
using Autodesk.TS.VltPlmAddIn.Model;

namespace Autodesk.TS.VltPlmAddIn.Forms
{
    public partial class WebViewFmSearch: UserControl
    {
        private static string mRelURL = "/pdm-search?&theme=";

        //register the JavaScript interoperability class
        internal JavaScriptInterop JavaScriptInterop { get; set; } = null;

        public WebViewFmSearch()
        {
            InitializeComponent();

            // Initialize the WebView2 control
            InitializeWebView();

            // Set the URL to navigate to and navigate
            String mURL = VaultExplorerExtension.mFmExtensionUrl + mRelURL + VaultExplorerExtension.mCurrentTheme.ToLower();
            Navigate(mURL);
        }

        private void InitializeWebView()
        {
            var frame = new DispatcherFrame();
            string userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Adsk.TS.Vault-FM-Panels");
            var env = CoreWebView2Environment.CreateAsync(null, userDataFolder, null);

            using (var task = FmSearch.EnsureCoreWebView2Async(env.Result))
            {
                task.ContinueWith((dummy) => frame.Continue = false);
                frame.Continue = true;
                Dispatcher.PushFrame(frame);
            }

            //register the JavaScript interoperability class
            JavaScriptInterop = new JavaScriptInterop(this);

            FmSearch.GotFocus += FmSearch_GotFocus;

            FmSearch.CoreWebView2.WebMessageReceived += FmSearch_WebMessageReceived;
        }

        private void FmSearch_GotFocus(object sender, EventArgs e)
        {
            String mURL = VaultExplorerExtension.mFmExtensionUrl + mRelURL + VaultExplorerExtension.mCurrentTheme.ToLower() + "&host=Vault";
            Navigate(mURL);
        }

        public void Navigate(string mUrl)
        {
            Uri uri = new Uri(mUrl, System.UriKind.Absolute);
            FmSearch.Source = uri;
        }

        private void FmSearch_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            // Handle the message received from the web view
            string message = e.TryGetWebMessageAsString();
            if (!String.IsNullOrEmpty(message))
            {
                JavaScriptInterop?.handleJsMessage(message);
            }
        }
    }
}
