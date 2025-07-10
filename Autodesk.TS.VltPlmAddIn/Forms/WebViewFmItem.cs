using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;
using Microsoft.Web.WebView2.Core;
using System.Text.Json;
using Autodesk.TS.VltPlmAddIn.Model;

namespace Autodesk.TS.VltPlmAddIn.Forms
{
    public partial class WebViewFmItem : UserControl
    {
        // Register the JavaScript interoperability class
        internal JavaScriptInterop JavaScriptInterop { get; set; } = null!; // Use null-forgiving operator to suppress CS8618

        public WebViewFmItem()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private void InitializeWebView()
        {
            var frame = new DispatcherFrame();
            string userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Adsk.TS.Vault-FM-Panels");
            var env = CoreWebView2Environment.CreateAsync(null, userDataFolder, null);

            using (var task = FmItem.EnsureCoreWebView2Async(env.Result))
            {
                task.ContinueWith((dummy) => frame.Continue = false);
                frame.Continue = true;
                Dispatcher.PushFrame(frame);
            }

            // Register the JavaScript interoperability class
            JavaScriptInterop = new JavaScriptInterop(this);

            FmItem.CoreWebView2.WebMessageReceived += FmItem_WebMessageReceived;
        }

        public void Navigate(string mUrl)
        {
            Uri uri = new Uri(mUrl, System.UriKind.Absolute);
            FmItem.Source = uri;
        }

        private void FmItem_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
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
