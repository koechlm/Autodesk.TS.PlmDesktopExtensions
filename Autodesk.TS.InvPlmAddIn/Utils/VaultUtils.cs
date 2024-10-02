using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACET = Autodesk.Connectivity.Explorer.ExtensibilityTools;
using ACW = Autodesk.Connectivity.WebServices;
using VDF = Autodesk.DataManagement.Client.Framework;
using VltBase = Connectivity.Application.VaultBase;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Properties;

namespace InvPlmAddIn.Utils
{
    /// <summary>
    /// Vault utility class, wrapping multiple API calls to commonly used methods.
    /// Files: Download, upload, check in, check out, etc.
    /// Items: Download primary linked files, etc.
    /// Search: Search for files, items, etc.
    /// Note - This class derived and extended the tech preview of iLogic-Vault integration.
    /// </summary>
    internal class VaultUtils
    {
        /// <summary>
        /// Any Vault interaction requires an active Client-Server connection.
        /// To avoid Vault API specific references, check connection state using the loggedIn property.
        /// </summary>
        private readonly static VDF.Vault.Currency.Connections.Connection conn = VltBase.ConnectionManager.Instance.Connection;
        private readonly static Autodesk.Connectivity.WebServicesTools.WebServiceManager mWsMgr = conn.WebServiceManager;

        /// <summary>
        /// Some methods are not applicable to Vault Basic; we need to know the environment
        /// </summary>
        private static readonly ACW.Product[] mProducts = VltBase.ConnectionManager.Instance.Connection.WebServiceManager.InformationService.GetSupportedProducts();

        private readonly bool IsVaultPro = mIsVaultPro;

        private readonly bool IsVaultBasic = mIsVaultBasic;

        /// <summary>
        /// Indicates Vault Basic environment
        /// </summary>
        private static bool mIsVaultBasic
        {
            get
            {
                //Vault Basic Servers return only a single product, whereas Vault Pro returns 4
                if (mProducts.Length == 1)
                {
                    return true;
                }
                return false;
            }
        }

        private static bool mIsVaultPro
        {
            get
            {
                //Vault Pro return 4 products
                if (mProducts.Length == 4)
                {
                    return true;
                }
                return false;
            }
        }

        //avoid multiple server calls for the iLogic-Vault session
        private ACW.PropDef[] mFilePropDefs = mWsMgr.PropertyService.GetPropertyDefinitionsByEntityClassId("FILE");
        private ACW.PropDef[] mItemPropDefs = mWsMgr.PropertyService.GetPropertyDefinitionsByEntityClassId("ITEM");

        /// <summary>
        /// Property representing the current user's Vault connection state; returns true, if current user is logged in.
        /// </summary>
        public bool LoggedIn
        {
            get
            {
                if (conn != null)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Deprecated. Returns current Vault connection. Leverage LoggedIn property whenever possible. 
        /// Null value returned if user is not logged in.
        /// </summary>
        /// <returns>Vault Connection</returns>
        public VDF.Vault.Currency.Connections.Connection GetVaultConnection()
        {
            if (conn != null)
            {
                return conn;
            }
            return null;
        }
    }
}
