using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using System.Runtime.InteropServices;

using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Firefox;

using log4net;

namespace FTTests {
    public class AdHocReportingBase {
        private RemoteWebDriver _driver;
        private string _adhocReportingUserName;
        private string _adhocReportingPassword;
        private string _adhocReportingChurchCode;
        private string _url;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Properties
        public string AdHocReportingUserName {
            set { _adhocReportingUserName = value; }
            get { return _adhocReportingUserName; }
        }

        public string AdHocReportingPassword {
            set { _adhocReportingPassword = value; }
            get { return _adhocReportingPassword; }
        }

        public string AdHocReportingChurchCode {
            set { _adhocReportingChurchCode = value; }
            get { return _adhocReportingChurchCode; }
        }

        public string URL {
            set { _url = value; }
            get { return _url; }
        }
        #endregion Properties

        public AdHocReportingBase(RemoteWebDriver driver) {
            this._driver = driver;
        }

        #region Instance Methods
        /// <summary>
        /// Logs into Reporting Analytics.
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="churchCode">The church code.</param>
        /// </summary>
        public void Login([Optional] string username, [Optional] string password, [Optional] string churchCode) {

            log.Debug("Enter Login");

            // Open the login page for reporting analytics
            //this._selenium.Open(this._url);
            this._driver.Navigate().GoToUrl(this._url);

            // Set the username
            if (!string.IsNullOrEmpty(username)) {
                //this._selenium.Type("username", username);
                this._driver.FindElement(By.Id("username")).SendKeys(username);
            }
            
            // Set the password
            if (!string.IsNullOrEmpty(password)) {
                //this._selenium.Type("password", password);
                this._driver.FindElement(By.Id("password")).SendKeys(password);
            }

            // Set the church code
            if (!string.IsNullOrEmpty(churchCode)) {
                //this._selenium.Type("churchcode", churchCode);
                this._driver.FindElement(By.Id("churchcode")).SendKeys(churchCode);
            }
            
            // Attempt to login
            //this._selenium.ClickAndWaitForPageToLoad("btn_login");
            this._driver.FindElement(By.Id("btn_login")).Click();

            log.Debug("Exit Login");

        
        }

        /// <summary>
        /// Logs out of Reporting Analytics.
        /// </summary>
        public void Logout() {
            log.Debug("Enter Logout");
            //this._selenium.ClickAndWaitForPageToLoad("link=sign out");
            this._driver.FindElement(By.LinkText("sign out")).Click();
            log.Debug("Exit Logout");

        }

        public void Security_Verify_DataSource_Displayed_Correctly(string[] reportRights) {

            log.Debug("Enter Security Verify DataSource Displayed Correctly");

            //string[] allDataSource = this._selenium.GetSelectOptions("//select[@id='ctl00_MainContent_queryBuilder_ctl01_jtc_Table']");
            var allDataSource = this._driver.FindElement(By.Id("ctl00_MainContent_queryBuilder_ctl01_jtc_Table")).FindElements(By.TagName("option"));

            //Verify 'Contribution_' present when Only Contribution right
            if ((reportRights.Length == 1) && (reportRights[0] == "Contribution")) {
                if ((this.AdhocReporting_Verify_OneReportRight_Exist_In_DataSource(reportRights[0])) && (!this.AdhocReporting_Verify_OneReportRight_Exist_In_DataSource("Ministry"))) {
                    log.Fatal("AdhocReporting_Verify_OneReportRight_Exist_In_DataSource: Expecting Only Contribution Actual: " + reportRights[0]);
                    Assert.IsTrue(true);
                }
            }

            //Verify 'Ministry_' present when Only Ministry right
            if ((reportRights.Length == 1) && (reportRights[0] == "Ministry")) {
                if ((this.AdhocReporting_Verify_OneReportRight_Exist_In_DataSource(reportRights[0])) && (!this.AdhocReporting_Verify_OneReportRight_Exist_In_DataSource("Contribution"))) {
                    log.Fatal("AdhocReporting_Verify_OneReportRight_Exist_In_DataSource: Expecting Only Ministry Actual: " + reportRights[0]);
                    Assert.IsTrue(true);
                }
            }

            //Verify 'Date_' and 'Group_' and 'People_'  present when the user with any report right but without Contribution or  Ministry right
            if ((reportRights.Length == 1) && ((reportRights[0] != "Ministry") || (reportRights[0] != "Contribution"))) {
                if ((this.AdhocReporting_Verify_OneReportRight_Exist_In_DataSource(reportRights[0]))) {
                    log.Fatal("AdhocReporting_Verify_OneReportRight_Exist_In_DataSource: Expecting: Contribution Actual: " + reportRights[0] + "OR Expecting: Ministry Actual: " + reportRights[1]);
                    Assert.IsTrue(true);
                }
            }

            //Verify Both 'Contribution_' and 'Ministry_' present when the user has Contribution and  Ministry right
            if ((reportRights.Length == 2) && (reportRights[0] == "Contribution") && (reportRights[1] == "Ministry")) {
                if ((this.AdhocReporting_Verify_OneReportRight_Exist_In_DataSource(reportRights[0])) && (this.AdhocReporting_Verify_OneReportRight_Exist_In_DataSource(reportRights[1]))) {
                    log.Fatal("AdhocReporting_Verify_OneReportRight_Exist_In_DataSource: Expecting: Contribution Actual: " + reportRights[0] + "AND Expecting: Ministry Actual: " + reportRights[1]);
                    Assert.IsTrue(true);
                }
            }

            //Verify 'Date_' and 'Group_' and 'People_' present in Data source, but 'Contribution_' and 'Ministry_' not present whe the user has multiple rights but no Contribution and ministry right
            if (reportRights.Length > 2) {
                if ((this.AdhocReporting_Verify_OneReportRight_Exist_In_DataSource(reportRights[0]))) {
                    log.Fatal("AdhocReporting_Verify_OneReportRight_Exist_In_DataSource: " + reportRights[0]);
                    Assert.IsTrue(true);
                }
            }

            log.Debug("Exit Security Verify DataSource Displayed Correctly");

        }
        #endregion Instance Methods

        #region Private Methods
        private bool AdhocReporting_Verify_OneReportRight_Exist_In_DataSource(string reportRight) {

            log.Debug("Enter AdhocReporting_Verify_OneReportRight_Exist_In_DataSource");
            bool rtnTOrF = false;
            bool dateTorF = false;
            bool groupTorF = false;
            bool peopleTorF = false;
            //string[] allDataSource = this._selenium.GetSelectOptions("//select[@id='ctl00_MainContent_queryBuilder_ctl01_jtc_Table']");
            var allDataSource = this._driver.FindElement(By.Id("ctl00_MainContent_queryBuilder_ctl01_jtc_Table")).FindElements(By.TagName("option"));

            if ((reportRight == "Contribution") || (reportRight == "Ministry")) {
                for (int i = 0; i < allDataSource.Count; i++) {
                    if (allDataSource[i].Text.Contains(reportRight)) {
                        rtnTOrF = true;
                    }
                }
            }
            else {
                for (int i = 0; i < allDataSource.Count; i++) {

                    if (allDataSource[i].Text.Contains("Date_")) {
                        dateTorF = true;
                    }
                    if (allDataSource[i].Text.Contains("Group_")) {
                        groupTorF = true;
                    }
                    if (allDataSource[i].Text.Contains("People_")) {
                        peopleTorF = true;
                    }
                }
                rtnTOrF = dateTorF && groupTorF && peopleTorF;
            }

            log.Debug("Exit AdhocReporting_Verify_OneReportRight_Exist_In_DataSource " + rtnTOrF);

            return rtnTOrF;
        }
        #endregion Private Methods
    }
}