using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Selenium;
using Gallio.Framework;
using MbUnit.Framework;
using Gallio.Framework.Pattern;

namespace FTTests {
    public class FOOSBase {
        private ISelenium _selenium;
        private string _url;

        #region Properties
        public string URL {
            get { return _url; }
            set { _url = value; }
        }
        #endregion Properties

        public FOOSBase(ISelenium selenium) {
            this._selenium = selenium;
        }


        #region Instance Methods
        /// <summary>
        /// Logs into FOOS.
        /// </summary>
        public void Login() {
            // Open the foos login page
            this._selenium.Open(this._url);

            // Login to foos
            this._selenium.Type("txtUserName_textBox", "tcoulson"); //pm
            this._selenium.Type("txtPassword:textBox", "Tara.Coulson1");    //God!1st
            this._selenium.ClickAndWaitForPageToLoad("btnLogin");
        }

        /// <summary>
        /// Logs out of FOOS.
        /// </summary>
        public void Logout() {
            this._selenium.ClickAndWaitForPageToLoad("link=Sign Out");
        }
        #endregion Instance Methods
    }
}
