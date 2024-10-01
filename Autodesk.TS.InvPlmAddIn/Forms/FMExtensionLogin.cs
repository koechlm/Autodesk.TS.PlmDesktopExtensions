using DevExpress.XtraEditors;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;
using VDF = Autodesk.DataManagement.Client.Framework;

namespace InvPlmAddIn.Forms
{
    public partial class FMExtensionLogin : DevExpress.XtraEditors.XtraForm
    {


        public FMExtensionLogin(string currentTheme)
        {
            InitializeComponent();

            ApplyThemes(currentTheme);

            // Set the webview handler
            WebView2 webView = new WebView2();
            this.panelControl1.Controls.Add(webView);
            webView.Dock = DockStyle.Fill;

            //InvPlmAddIn.Model.WebViewHandler mWebViewHandler = new global::InvPlmAddIn.Model.WebViewHandler("https://www.plm.tools:9600/addins/navigate", new Model.HostObject(this));
                   
        }

        private void mWebViewInit()
        {

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