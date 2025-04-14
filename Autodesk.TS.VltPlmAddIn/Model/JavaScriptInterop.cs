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
        private Navigation? _navigation;
        private WebViewFmItem? webViewFmItem;
        private WebViewFmSearch? webViewFmSearch;
        private WebViewFmTasks? webViewFmTasks;

        public JavaScriptInterop(WebViewFmItem webViewFmItem)
        {
            this.webViewFmItem = webViewFmItem;
        }

        public JavaScriptInterop(WebViewFmSearch webViewFmSearch)
        {
            this.webViewFmSearch = webViewFmSearch;
        }

        public JavaScriptInterop(WebViewFmTasks webViewFmTasks)
        {
            this.webViewFmTasks = webViewFmTasks;
        }

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
                        gotoVaultFolder(mParametersArray);
                        break;
                    case "gotoVaultFile":
                        gotoVaultFile(mParametersArray);
                        break;
                    case "gotoVaultItem":
                        gotoVaultItem(mParametersArray);
                        break;
                    case "gotoVaultECO":
                        gotoVaultECO(mParametersArray);
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
        public void gotoVaultFolder(string[] parameters)
        {
            VaultExplorerExtension.mSender = Autodesk.TS.VltPlmAddIn.NavigationSender.FMExtension;

            // Navigate to the related entity in Vault
            if (_navigation == null)
                _navigation = new Navigation();
            _navigation.GotoVaultFolder(parameters);

            //reset the sender
            VaultExplorerExtension.mSender = Autodesk.TS.VltPlmAddIn.NavigationSender.Host;
        }

        // This method is called from JavaScript to navigate to the parameters in Vault
        public void gotoVaultFile(string[] parameters)
        {
            VaultExplorerExtension.mSender = Autodesk.TS.VltPlmAddIn.NavigationSender.FMExtension;

            // Navigate to the related entity in Vault
            if (_navigation == null)
                _navigation = new Navigation();
            _navigation.GotoVaultFile(parameters);

            //reset the sender
            VaultExplorerExtension.mSender = Autodesk.TS.VltPlmAddIn.NavigationSender.Host;
        }

        // This method is called from JavaScript to navigate to the parameters in Vault
        public void gotoVaultItem(string[] parameters)
        {
            VaultExplorerExtension.mSender = Autodesk.TS.VltPlmAddIn.NavigationSender.FMExtension;

            // Navigate to the related entity in Vault
            if (_navigation == null)
                _navigation = new Navigation();
            _navigation.GotoVaultItem(parameters);

            //reset the sender
            VaultExplorerExtension.mSender = Autodesk.TS.VltPlmAddIn.NavigationSender.Host;
        }

        public void gotoVaultECO(string[] parameters)
        {
            VaultExplorerExtension.mSender = Autodesk.TS.VltPlmAddIn.NavigationSender.FMExtension;

            // Navigate to the related entity in Vault
            if (_navigation == null)
                _navigation = new Navigation();
            _navigation.GotoVaultChangeOrder(parameters);

            //reset the sender
            VaultExplorerExtension.mSender = Autodesk.TS.VltPlmAddIn.NavigationSender.Host;
        }
    }
}
