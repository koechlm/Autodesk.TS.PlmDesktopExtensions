using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Autodesk.TS.VltPlmAddIn
{
    public partial class CefControlItem : UserControl
    {
        public CefControlItem()
        {
            InitializeComponent();

            //Initialize the CefSharp browser
            InitializeItemBrowser();
        }

        private void InitializeItemBrowser()
        {
            // Create a new instance of the CefSharp browser
            CefSharp.WinForms.ChromiumWebBrowser browser = new CefSharp.WinForms.ChromiumWebBrowser("https://www.plm.tools:9600/addins/item?dmsId=14849&wsId=57&theme=light");
            _ = browser.WaitForInitialLoadAsync();

            // Make the browser fill the form
            browser.Dock = DockStyle.Fill;
            browser.Show();
            // Add the browser to the form
            this.Controls.Add(browser);

        }

        public void SetSelectedObject(object o)
        {
            //mPropertyGrid.SelectedObject = o;
        }
    }
}
