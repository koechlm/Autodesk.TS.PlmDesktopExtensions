using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace InvPlmAddIn.Model
{
	public class WebViewHandler : IDisposable
	{
		public WebViewHandler(string url, HostObject hostObject)
		{
			Url = url;

			WebView = new WebView2();
			InitializeWebViewAsync(hostObject);
		}

		public string Url { get; set; }

		public WebView2 WebView { get; set; }

		public static async Task<CoreWebView2Environment> GetWebViewEnvironment()
		{
            string userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Adsk.TS.Inventor-FM-Panels");            
            return await CoreWebView2Environment.CreateAsync(null, userDataFolder, null);
		}

		public void Dispose()
			=> WebView?.Dispose();

		public void ExecutePlmSearchRawMaterial(string searchText)
			=> WebView.ExecuteScriptAsync($"addinSelectRawMaterial({searchText})");

		public void ExecutePlmSelectItem(string partNumbers)
			=> WebView.CoreWebView2.ExecuteScriptAsync($"addinSelect({partNumbers})");

		public void LoadUrl()
		{
			WebView.CoreWebView2.Navigate(Url);
		}

		private async void InitializeWebViewAsync(HostObject hostObject)
		{
			await WebView.EnsureCoreWebView2Async(await GetWebViewEnvironment());

			WebView.CoreWebView2.AddHostObjectToScript("plmAddin", hostObject);

            // register the message handler
            WebView.CoreWebView2.WebMessageReceived += WebView_MessageReceived;

			LoadUrl();
		}

        private void WebView_MessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            // Handle the message received from the web view
            string message = e.TryGetWebMessageAsString();
            if (!String.IsNullOrEmpty(message))
            {
                HostObject.HandleJsMessage(message);
            }
        }
	}
}
