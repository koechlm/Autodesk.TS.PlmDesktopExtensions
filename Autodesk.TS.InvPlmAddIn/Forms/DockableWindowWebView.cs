using DevExpress.XtraEditors;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using VDF = Autodesk.DataManagement.Client.Framework;
using DevExpress.Utils;
using System.Diagnostics;
using Inventor;
using InvPlmAddIn.Model;

namespace InvPlmAddIn.Forms
{
    public partial class mDockWindowChild : DevExpress.XtraEditors.XtraUserControl, IDisposable
    {
        private static Utils.Settings mAddinSettings = new Utils.Settings();
        private static Uri mBaseUri = null;

        public mDockWindowChild(string currentTheme)
        {
            InitializeComponent();

            mAddinSettings = Utils.Settings.Load();
            mBaseUri = new Uri(mAddinSettings.FmExtensionUrl);
            ApplyThemes(currentTheme);

        }

        public Action<bool> IsolateAction { get; set; }
        public WebViewHandler WebViewHandler { get; private set; }

        public void SetWebView(WebViewHandler webViewHandler)
        {
            if (WebViewHandler != null)
            {
                webViewPanel.Controls.Clear();
            }

            webViewPanel.Controls.Add(webViewHandler.WebView);
            //webViewHandler.WebView.Parent = webViewPanel;
            webViewHandler.WebView.Dock = DockStyle.Fill;
            webViewHandler.WebView.Visible = true;
            WebViewHandler = webViewHandler;
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
