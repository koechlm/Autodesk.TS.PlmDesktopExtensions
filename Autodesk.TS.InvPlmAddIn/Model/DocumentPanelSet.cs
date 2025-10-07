using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InvPlmAddIn.Model
{
	public class DocumentPanelSet : IDisposable
	{
        private static Utils.Settings mAddinSettings = new Utils.Settings();
        private static Uri mBaseUri = null;

        public const string PartNumberParameter = "{PartNumber}";
		public PanelOptions[] Options;
		public DocumentEvents DocumentsEvents { get; set; }

		public DocumentPanelSet(BrowserPanelWindowManager panelManager, IEnumerable<PanelOptions> options, string partNumber)
		{
            mAddinSettings = Utils.Settings.Load();
            mBaseUri = new Uri(mAddinSettings.FmExtensionUrl);
			PanelManager = panelManager;
			HostObject = new HostObject(this);
			Options = options?.ToArray() ?? Array.Empty<PanelOptions>();
			PartNumber = partNumber;
			InitViews();
		}

		public bool ArePagesRefreshedAfterLogin { get; set; }

		public HostObject HostObject { get; set; }

		public BrowserPanelWindowManager PanelManager { get; private set; }

		public string PartNumber { get; set; }

		public Dictionary<string, WebViewHandler> WebViewHandlers { get; set; } = new Dictionary<string, WebViewHandler>();

		public void Dispose()
		{
			foreach (var keyValue in WebViewHandlers)
				keyValue.Value.Dispose();
			WebViewHandlers.Clear();
		}

		public void InitViews()
		{
			foreach (var option in Options)
				WebViewHandlers.Add(option.InternalName, new WebViewHandler(ReplaceUrlParameter(option.Url), HostObject));
		}

		public void RefreshWebViewsAfterLoginIfNeeded()
		{
			if (!ArePagesRefreshedAfterLogin && PanelManager.IsLoggedIn)
			{
				ArePagesRefreshedAfterLogin = true;
				RefreshPages();
			}
		}

		private bool DoesUrlContainsPartNumber(string url)
			=> url.Contains(PartNumberParameter);


		private void RefreshPages()
		{
			foreach (var panel in WebViewHandlers.Values)
				panel.LoadUrl();
		}

		private string ReplaceUrlParameter(string url)
			=> url
				.Replace("{BaseUrl}", mBaseUri.ToString())
				.Replace(PartNumberParameter, PartNumber)
				.Replace("{Theme}", InvPlmAddIn.InvPlmAddinSrv.mInventorApplication.ThemeManager.ActiveTheme.Name.Replace("Theme", ""));

        public void mSendMessage(string message)
        {
            foreach (var panel in WebViewHandlers.Values)
                panel.WebView.CoreWebView2.PostWebMessageAsString(message);
        }
    }
}
