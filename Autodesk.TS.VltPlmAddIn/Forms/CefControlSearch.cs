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
using Newtonsoft.Json;

namespace Autodesk.TS.VltPlmAddIn.Forms
{
    public partial class CefControlSearch : UserControl
    {
        CefSharp.WinForms.ChromiumWebBrowser? mBrowser;

        //register the JavaScript interoperability class
        internal JavaScriptInterop? JavaScriptInterop { get; set; }

        public CefControlSearch()
        {
            InitializeComponent();
            //Initialize the CefSharp mBrowser

            InitializeItemSearchBrowser();
        }

        private void InitializeItemSearchBrowser()
        {
            // Create a new instance of the CefSharp mBrowser
            mBrowser = new CefSharp.WinForms.ChromiumWebBrowser("https://www.plm.tools:9600/addins/search?&theme=light");
            _ = mBrowser.WaitForInitialLoadAsync();

            mBrowser.JavascriptMessageReceived += (sender, e) =>
            {

            };                    

            // Make the mBrowser fill the form
            mBrowser.Dock = DockStyle.Fill;
            mBrowser.Show();

            //register the JavaScript interoperability class
            JavaScriptInterop = new JavaScriptInterop(this);
            mBrowser.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
            mBrowser.JavascriptObjectRepository.Register("JavaScriptInterop", JavaScriptInterop, options:BindingOptions.DefaultBinder );

            mBrowser.JavascriptMessageReceived += ChromeBrowser_JavascriptMessageReceived;
        


            // Add the mBrowser to the form
            this.Controls.Add(mBrowser);

        }


        private void ChromeBrowser_JavascriptMessageReceived(object? sender, JavascriptMessageReceivedEventArgs e)
        {
            MessageBox.Show($"JavascriptMessageReceived event: {e.Message}");
        }
    }
}
