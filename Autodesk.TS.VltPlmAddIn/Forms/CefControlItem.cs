using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.TS.VltPlmAddIn.Model;
using CefSharp;
using CefSharp.WinForms;

namespace Autodesk.TS.VltPlmAddIn
{
    public partial class CefControlItem : UserControl
    {
        CefSharp.WinForms.ChromiumWebBrowser? mBrowser;

        //register the JavaScript interoperability class
        internal JavaScriptInterop? JavaScriptInterop { get; set; }

        public CefControlItem()
        {
            InitializeComponent();

            //Initialize the CefSharp mBrowser

            InitializeItemBrowser();
        }

        private void InitializeItemBrowser()
        {
            // Create a new instance of the CefSharp mBrowser
            mBrowser = new CefSharp.WinForms.ChromiumWebBrowser("");
            _ = mBrowser.WaitForInitialLoadAsync();

            // Set the custom context menu handler
            mBrowser.MenuHandler = new CustomContextMenuHandler();

            //register the JavaScript interoperability class
            JavaScriptInterop = new JavaScriptInterop(this);
            mBrowser.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
            mBrowser.JavascriptObjectRepository.Register("JavaScriptInterop", JavaScriptInterop, options: BindingOptions.DefaultBinder);

            mBrowser.JavascriptMessageReceived += ItemBrowser_JavascriptMessageReceived;

            // Make the mBrowser fill the form
            mBrowser.Dock = DockStyle.Fill;
            mBrowser.Show();

            // Add the mBrowser to the form
            this.Controls.Add(mBrowser);
        }

        private void ItemBrowser_JavascriptMessageReceived(object? sender, JavascriptMessageReceivedEventArgs e)
        {
            //MessageBox.Show(e.Message?.ToString());

            var message = e.Message?.ToString();
            if (!String.IsNullOrEmpty(message))
            {
                JavaScriptInterop?.handleJsMessage(message);
            }
        }

        public void SetSelectedObject(object o)
        {
            //mPropertyGrid.SelectedObject = o;
        }

        //navigate to the specified URL
        public void NavigateToUrl(string url)
        {
            mBrowser?.LoadUrlAsync(url);
        }

        //navigate asynch to the specified URL
        public async Task NavigateToUrlAsync(string url)
        {
            if (mBrowser != null)
            {
                await mBrowser.LoadUrlAsync(url);
            }
        }

        public void ExecuteScript(string script)
        {
            mBrowser?.ExecuteScriptAsync(script);
        }

        
    }
}
