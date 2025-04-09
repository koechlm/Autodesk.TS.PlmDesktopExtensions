using System;
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

namespace Autodesk.TS.VltPlmAddIn.Forms
{
    public partial class WebViewFmSearch: UserControl
    {
        public WebViewFmSearch()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private void InitializeWebView()
        {
            var frame = new DispatcherFrame(); // This now resolves correctly
            var env = CoreWebView2Environment.CreateAsync(null, Environment.GetEnvironmentVariable("TEMP"), null);

            using (var task = FmSearch.EnsureCoreWebView2Async(env.Result))
            {
                task.ContinueWith((dummy) => frame.Continue = false);
                frame.Continue = true;
                Dispatcher.PushFrame(frame);
            }

            FmSearch.CoreWebView2.WebMessageReceived += FmItem_WebMessageReceived;
        }

        public void Navigate(string mUrl)
        {
            Uri uri = new Uri(mUrl, System.UriKind.Absolute);
            FmSearch.Source = uri;
        }

        private void FmItem_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            // Handle the message received from the web view
            string message = e.TryGetWebMessageAsString();

            // Process the message as needed
        }
    }
}
