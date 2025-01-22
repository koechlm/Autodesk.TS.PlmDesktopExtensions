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

            // Make the mBrowser fill the form
            mBrowser.Dock = DockStyle.Fill;
            mBrowser.Show();

            // Add the mBrowser to the form
            this.Controls.Add(mBrowser);
        }

        public void SetSelectedObject(object o)
        {
            //mPropertyGrid.SelectedObject = o;
        }

        //navigate to the specified URL
        public void NavigateToUrl(string url)
        {
            mBrowser?.LoadUrl(url);
        }

        public void ExecuteScript(string script)
        {
            mBrowser?.ExecuteScriptAsync(script);
        }

        
    }
}
