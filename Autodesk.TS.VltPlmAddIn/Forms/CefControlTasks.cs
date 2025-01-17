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

namespace Autodesk.TS.VltPlmAddIn.Forms
{
    public partial class CefControlTasks : UserControl
    {
        CefSharp.WinForms.ChromiumWebBrowser? mBrowser;

        //register the JavaScript interoperability class
        internal JavaScriptInterop? JavaScriptInterop { get; set; }

        public CefControlTasks()
        {
            InitializeComponent();

            // Initialize the CefSharp mBrowser
            InitializeTasksBrowser();
        }

        private void InitializeTasksBrowser()
        {
            // Create a new instance of the CefSharp mBrowser
            mBrowser = new CefSharp.WinForms.ChromiumWebBrowser("https://www.plm.tools:9600/addins/tasks?&theme=light");
            _ = mBrowser.WaitForInitialLoadAsync();

            // Make the mBrowser fill the form
            mBrowser.Dock = DockStyle.Fill;
            mBrowser.Show();
            // Add the mBrowser to the form
            this.Controls.Add(mBrowser);

            //register the JavaScript interoperability class
            JavaScriptInterop = new JavaScriptInterop(this);
            mBrowser.JavascriptObjectRepository.Register("JavaScriptInterop", JavaScriptInterop);

        }
    }
}
