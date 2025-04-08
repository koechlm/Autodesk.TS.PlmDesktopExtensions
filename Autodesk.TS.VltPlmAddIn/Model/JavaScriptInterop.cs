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
        //private readonly CefControlItem? _cefControlItem;
        //private readonly CefControlSearch? _cefControlSearch;
        //private readonly CefControlTasks? _cefControlTaks;

        private Navigation? _navigation;

        //public JavaScriptInterop(CefControlItem? cefControlItem)
        //{
        //    _cefControlItem = cefControlItem;
        //}

        //public JavaScriptInterop(CefControlSearch? cefControlSearch)
        //{
        //    _cefControlSearch = cefControlSearch;
        //}

        //public JavaScriptInterop(CefControlTasks? cefControlTaks)
        //{
        //    _cefControlTaks = cefControlTaks;
        //}

        public void handleJsMessage(string message)
        {
            String[]? mMessageArray = message?.ToString()?.Split(":");
            if (mMessageArray?.Length > 1)
            {
                String mCommand = mMessageArray[0];
                String mParameters = mMessageArray[1];
                String[] mParametersArray = mParameters.Split(";");

                switch (mCommand)
                {
                    case "addComponent":
                        _ = addComponent(mParametersArray);
                        break;
                    case "openComponents":
                        _ = openComponent(mParametersArray);
                        break;
                    case "gotoVaultFolder":
                        _ = gotoVaultFolder(mParametersArray);
                        break;
                    case "gotoVaultFile":
                        _ = gotoVaultFile(mParametersArray);
                        break;
                    case "gotoVaultItem":
                        _ = gotoVaultItem(mParametersArray);
                        break;
                    case "gotoVaultECO":
                        _ = gotoVaultECO(mParametersArray);
                        break;
                    default:
                        break;
                }
            }
        }

        // This method is called from JavaScript to add components to Inventor _application
        public async Task addComponent(string[] parameters)
        {
            await Task.Run(() =>
            {
                // Add components to Inventor _application
            });
        }

        // This method is called from JavaScript to open components from Inventor _application
        public async Task openComponent(string[] parameters)
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
                if (_navigation == null)
                    _navigation = new Navigation();
                _navigation?.GotoVaultFolder(parameters);
            });
        }

        // This method is called from JavaScript to navigate to the parameters in Vault
        public async Task gotoVaultFile(string[] parameters)
        {
            VaultExplorerExtension.mSender = Autodesk.TS.VltPlmAddIn.NavigationSender.FMExtension;
            await Task.Run(() =>
            {
                // Navigate to the related entity in Vault
                if (_navigation == null)
                    _navigation = new Navigation();
                
                _navigation?.GotoVaultFile(parameters);
            });

            //reset the sender
            VaultExplorerExtension.mSender = Autodesk.TS.VltPlmAddIn.NavigationSender.Host;
        }

        //This method is called from JavaScript to navigate to the parameters in Vault
        public async Task gotoVaultItem(string[] parameters)
        {
            await Task.Run(() =>
            {
                // Navigate to the related entity in Vault
                if (_navigation == null)
                    _navigation = new Navigation();
                _navigation?.GotoVaultItem(parameters);
            });
        }
        //public void gotoVaultItem(string[] parameters)
        //{
        //        // Navigate to the related entity in Vault
        //        if (_navigation == null)
        //            _navigation = new Navigation();
        //        _navigation?.GotoVaultItem(parameters);           
        //}

        // This method is called from JavaScript to navigate to the change order in Vault
        public async Task gotoVaultECO(string[] parameters)
        {
            await Task.Run(() =>
            {
                // Navigate to the related entity in Vault
                if (_navigation == null)
                    _navigation = new Navigation();
                _navigation?.GotoVaultChangeOrder(parameters);
            });
        }
    }
}
