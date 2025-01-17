using Autodesk.TS.VltPlmAddIn.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodesk.TS.VltPlmAddIn.Model
{
    internal class JavaScriptInterop
    {
        private readonly CefControlItem? _cefControlItem;
        private readonly CefControlSearch? _cefControlSearch;
        private readonly CefControlTasks? _cefControlTaks;

        public JavaScriptInterop(CefControlItem? cefControlItem)
        {
            _cefControlItem = cefControlItem;
        }

        public JavaScriptInterop(CefControlSearch? cefControlSearch)
        {
            _cefControlSearch = cefControlSearch;
        }

        public JavaScriptInterop(CefControlTasks? cefControlTaks)
        {
            _cefControlTaks = cefControlTaks;
        }

        // This method is called from JavaScript to add components to Inventor application
        public async Task addComponents(object[] numbers)
        {
            await Task.Run(() =>
            {
                // Add components to Inventor application
            });
        }

        // This method is called from JavaScript to open components from Inventor application
        public async Task openComponents(object[] numbers)
        {
            await Task.Run(() =>
            {
                // Open components from Inventor application
            });
        }

        // This method is called from JavaScript to navigate to the folder in Vault
        public async Task gotoVaultFolder(object[] folder)
        {
            await Task.Run(() =>
            {
                // Navigate to the related entity in Vault
            });
        }

        // This method is called from JavaScript to navigate to the item in Vault
        public async Task gotoVaultItem(object[] item)
        {
            await Task.Run(() =>
            {
                // Navigate to the related entity in Vault
            });
        }

        // This method is called from JavaScript to navigate to the file in Vault
        public async Task gotoVaultFile(object[] file)
        {
            await Task.Run(() =>
            {
                // Navigate to the related entity in Vault
            });
        }

        // This method is called from JavaScript to navigate to the change order in Vault
        public async Task gotoVaultChangeOrder(object[] changeOrder)
        {
            await Task.Run(() =>
            {
                // Navigate to the related entity in Vault
            });
        }
    }
}
