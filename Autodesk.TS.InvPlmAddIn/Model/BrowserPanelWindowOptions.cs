namespace InvPlmAddIn.Model
{
	public class BrowserPanelWindowOptions
	{
		public string InternalName { get; set; }

		public string WindowTitle { get; set; }

		public Inventor.Application Application { get; set; }

		public HostObject HostObject { get; set; }

		public string ClientId { get; set; }

		public string Url { get; set; }

		public string ThemeName { get; set; }

		public BrowserPanelWindowOptions() {}

		public BrowserPanelWindowOptions(BrowserPanelWindowOptions cloneBaseOptions)
		{
			Application = cloneBaseOptions.Application;
			HostObject = cloneBaseOptions.HostObject;
			ClientId = cloneBaseOptions.ClientId;
			ThemeName = cloneBaseOptions.ThemeName;
		}

		private const string PartNumberParameter = "{PartNumber}";

		public bool DoesUrlContainsPartNumber
			=> Url.Contains(PartNumberParameter);

		public string ReplaceUrlParameter(string connectionstring)
			=> Url
				.Replace("{BaseUrl}", "https://www.forge.tools:9150/addins")
				.Replace(PartNumberParameter, connectionstring)
				.Replace("{Theme}", Application.ThemeManager.ActiveTheme.Name.Replace("Theme", ""));

	}
}