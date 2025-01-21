using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
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

        private Navigation? _navigation;

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

        // This method is called from JavaScript to add components to Inventor _application
        public async Task addComponents(object[] numbers)
        {
            await Task.Run(() =>
            {
                // Add components to Inventor _application
            });
        }

        // This method is called from JavaScript to open components from Inventor _application
        public async Task openComponents(object[] numbers)
        {
            await Task.Run(() =>
            {
                // Open components from Inventor _application
            });
        }

        // This method is called from JavaScript to navigate to the parameters in Vault
        public async Task gotoVaultFolder(string[] parameters)
        {
            await Task.Run(() =>
            {
                // Navigate to the related entity in Vault
                _navigation = new Navigation();
                _navigation?.GotoVaultFolder(parameters);
            });
        }

        // This method is called from JavaScript to navigate to the parameters in Vault
        public async Task gotoVaultFile(string[] parameters)
        {
            await Task.Run(() =>
            {
                // Navigate to the related entity in Vault
                _navigation = new Navigation();
                _navigation?.GotoVaultFile(parameters);
            });
        }

        // This method is called from JavaScript to navigate to the parameters in Vault
        public async Task gotoVaultItem(string[] parameters)
        {
            await Task.Run(() =>
            {
                // Navigate to the related entity in Vault
            });
        }

        // This method is called from JavaScript to navigate to the change order in Vault
        public async Task gotoVaultECO(object[] parameters)
        {
            await Task.Run(() =>
            {
                // Navigate to the related entity in Vault
            });
        }
    }
}
