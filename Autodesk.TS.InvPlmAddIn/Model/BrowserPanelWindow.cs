using System;
using Inventor;

namespace InvPlmAddIn.Model
{
	public class BrowserPanelWindow
	{
		public BrowserPanelWindow(PanelOptions options, string clientId, Inventor.Application mInventorApp)
		{
			Options = options;
			Init(clientId, mInventorApp);
		}

		public PanelOptions Options { get; set; }

		private DockableWindow PlmDockableWindow { get; set; }

		//private Form.XtraUserControl UserControl { get; set; }
        private global::InvPlmAddIn.Forms.mDockWindowChild UserControl { get; set; }

        public void ExecutePlmSearchRawMaterial(string searchText)
		{
			UserControl.WebViewHandler.ExecutePlmSearchRawMaterial(searchText);
		}

		public void ExecutePlmSelectItem(string partNumbers)
		{
			UserControl.WebViewHandler.ExecutePlmSelectItem(partNumbers);
		}

		public WebViewHandler GetWebViewHandler()
		{
			return UserControl.WebViewHandler;
		}

		public void Remove(Application app)
		{
			var userInterfaceManager = app.UserInterfaceManager;

			foreach (DockableWindow window in userInterfaceManager.DockableWindows)
			{
				if (window.InternalName.Equals(Options.InternalName,StringComparison.InvariantCultureIgnoreCase))
				{
					window.Visible = false;
					window.ShowVisibilityCheckBox = false;
					window.Clear();
				}
			}

			PlmDockableWindow = null;

			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		public void SetWebViewHandler(WebViewHandler handler)
		{
			UserControl.SetWebView(handler);
		}

		private void Init(string clientId, Application app)
		{
			var userInterfaceManager = app.UserInterfaceManager;
			PlmDockableWindow = null;
			foreach (DockableWindow mWindow in userInterfaceManager.DockableWindows)
			{
				if (mWindow.InternalName.Equals(Options.InternalName,StringComparison.InvariantCultureIgnoreCase))
				{
					PlmDockableWindow = mWindow;
					PlmDockableWindow.ShowVisibilityCheckBox = true;
				}
			}

			if (PlmDockableWindow == null)
				PlmDockableWindow =
					userInterfaceManager.DockableWindows.Add(clientId, Options.InternalName, Options.WindowTitle);
			try
			{
				UserControl = new InvPlmAddIn.Forms.mDockWindowChild(app.ActiveColorScheme.Name.Replace("Theme", ""));

				PlmDockableWindow.AddChild(UserControl.Handle.ToInt64());

				var backgroundColor = app.ThemeManager.GetComponentThemeColor("BrowserPane_BackgroundColor");
				var color = System.Drawing.Color.FromArgb(backgroundColor.Red, backgroundColor.Green,
					backgroundColor.Blue);
				UserControl.BackColor = color;

				UserControl.Show();
			}
			catch (Exception e)
			{
				AdskTsVaultUtils.Messages.ShowError(string.Format("Adding a dockable window failed with unhandled exception: {0}", e.Message), InvPlmAddIn.InvPlmAddinSrv.AddInName);
			}
		}
	}
}
