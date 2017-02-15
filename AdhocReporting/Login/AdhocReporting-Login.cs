using System;
using System.Data;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace FTTests.AdhocReporting.Login {
    [TestFixture]
    public class AdhocReporting_Login : FixtureBaseWebDriver {
        #region Login
        #region Validation
        [Test, RepeatOnFailure]
        [Author("Luke Jiang")]
        [Description("Views and verifies the login page for Adhoc Reporting.")]
        public void AdhocReporting_Login_View() {
            // View the login page for adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Driver.Navigate().GoToUrl(test.AdHocReporting.URL);

            // Verify title
            Assert.AreEqual("Reporting Analytics-Login", test.Driver.Title);

            // Verify the header text
            Assert.AreEqual("Login", test.Driver.FindElementByXPath("//div[@id='main']/div/div[2]/div[1]/span").Text);

            // Verify the labels associated with the fields for login
            Assert.AreEqual("Username", test.Driver.FindElementByXPath("//label[@for='username']").Text);
            Assert.AreEqual("Password", test.Driver.FindElementByXPath("//label[@for='pwd']").Text);
            Assert.AreEqual("Church Code", test.Driver.FindElementByXPath("//label[@for='churchcode']").Text);

            // Verify the text on the login button
            Assert.AreEqual("Login", test.Driver.FindElementById("btn_login").GetAttribute("value"));

            // Verify the footer links
            Assert.IsTrue(test.Driver.FindElementByLinkText("Fellowship Technologies, LP").Displayed);
            Assert.IsTrue(test.Driver.FindElementByLinkText("Security tested daily").Displayed);

            // Close the browser
            test.Driver.Close();
        }

        [Test, RepeatOnFailure(Order = 7)]
        [Author("Luke Jiang")]
        [Description("Verify Local user login successfully with correct login crediential.")]
        public void AdhocReporting_Login_Validation_LocalUser_Login_Successfully_With_ReportRight() {
            // Set initial conditions
            string login = "lukejiang88";
            string[] securityRights = new string[] { "Contribution" };
            base.SQL.Admin_Users_DeleteSecurityRolesBySecurityGroup(15, login, 4);
            base.SQL.Admin_Users_CreateSecurityRoles(15, login, 4, securityRights);

            // Login to adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.AdHocReporting.Login(login, "chrdwhdhXT1!", "dc");

            // Verify the church name is present
            Assert.AreEqual("Dynamic Church", test.Driver.FindElementById("brand_name").Text);

            // Verify the page title
            Assert.AreEqual("Report List Page", test.Driver.Title);

            // Logout of adhoc reporting
            test.AdHocReporting.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Luke Jiang")]
        [Description("Verifies functionality when the user attempts to login to Adhoc Reporting without providing a username, password, or church code.")]
        public void AdhocReporting_Login_Validation_Missed_All_LoginCredentials() {
            // View the login page for adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Driver.Navigate().GoToUrl(test.AdHocReporting.URL);

            // Attempt to login without providing a username, password, or church code
            test.AdHocReporting.Login(null, null, null);

            // Verify the error messages
            Assert.IsTrue(test.Driver.PageSource.Contains("Login attempt failed."));
            Assert.IsTrue(test.Driver.PageSource.Contains("Username, password and church code are required."));

            // Close the browser
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Luke Jiang")]
        [Description("Verifies functionality when the user attempts to login to Adhoc Reporting without providing a username.")]
        public void AdhocReporting_Login_Validation_Missed_UserName() {
            // View the login page for adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Driver.Navigate().GoToUrl(test.AdHocReporting.URL);

            // Attempt to login without providing a username 
            test.AdHocReporting.Login(null, "cchrdwhdhXT1!", "dc");

            // Verify the error messages
            Assert.IsTrue(test.Driver.PageSource.Contains("Login attempt failed."));
            Assert.IsTrue(test.Driver.PageSource.Contains("Username, password and church code are required."));

            // Close the browser
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Luke Jiang")]
        [Description("Verifies functionality when the user attempts to login to Adhoc Reporting without providing a password.")]
        public void AdhocReporting_Login_Validation_Missed_Password() {
            // View the login page for adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Driver.Navigate().GoToUrl(test.AdHocReporting.URL);

            // Attempt to login without providing a password
            test.AdHocReporting.Login("lukejiang88", null, "dc");

            // Verify the error messages
            Assert.IsTrue(test.Driver.PageSource.Contains("Login attempt failed."));
            Assert.IsTrue(test.Driver.PageSource.Contains("Username, password and church code are required."));

            // Close the browser
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Luke Jiang")]
        [Description("Verifies functionality when the user attempts to login to Adhoc Reporting without providing a church code.")]
        public void AdhocReporting_Login_Validation_Missed_ChurchCode() {
            // View the login page for adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Driver.Navigate().GoToUrl(test.AdHocReporting.URL);

            // Attempt to login without providing a church code
            test.AdHocReporting.Login("lukejiang88", "chrdwhdhXT1", null);

            // Verify the error messages
            Assert.IsTrue(test.Driver.PageSource.Contains("Login attempt failed."));
            Assert.IsTrue(test.Driver.PageSource.Contains("Username, password and church code are required."));

            // Close the browser
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Luke Jiang")]
        [Description("Verifies functionality when the user attempts to login to Adhoc Reporting without providing a username and password.")]
        public void AdhocReporting_Login_Validation_Missed_UserName_Password() {
            // View the login page for adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Driver.Navigate().GoToUrl(test.AdHocReporting.URL);

            // Attempt to login without providing a username and password
            test.AdHocReporting.Login(null, null, "dc");

            // Verify the error messages
            Assert.IsTrue(test.Driver.PageSource.Contains("Login attempt failed."));
            Assert.IsTrue(test.Driver.PageSource.Contains("Username, password and church code are required."));

            // Close the browser
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Luke Jiang")]
        [Description("Verifies functionality when the user attempts to login to Adhoc Reporting without providing a username and church code.")]
        public void AdhocReporting_Login_Validation_Missed_UserName_ChurchCode() {
            // View the login page for adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Driver.Navigate().GoToUrl(test.AdHocReporting.URL);

            // Attempt to login without providing a username and church code
            test.AdHocReporting.Login(null, "chrdwhdhXT1!", null);

            // Verify the error messages
            Assert.IsTrue(test.Driver.PageSource.Contains("Login attempt failed."));
            Assert.IsTrue(test.Driver.PageSource.Contains("Username, password and church code are required."));

            // Close the browser
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Luke Jiang")]
        [Description("Verifies functionality when the user attempts to login to Adhoc Reporting without providing a password and church code.")]
        public void AdhocReporting_Login_Validation_Missed_Password_ChurchCode() {
            // View the login page for adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Driver.Navigate().GoToUrl(test.AdHocReporting.URL);

            // Attempt to login without providing a password and church code
            test.AdHocReporting.Login("lukejiang88", null, null);

            // Verify the error messages
            Assert.IsTrue(test.Driver.PageSource.Contains("Login attempt failed."));
            Assert.IsTrue(test.Driver.PageSource.Contains("Username, password and church code are required."));

            // Close the browser
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Luke Jiang")]
        [Description("Verifies functionality when the user attempts to login to Adhoc Reporting with incorrect credentials.")]
        public void AdhocReporting_Login_Validation_Incorrect_Crediential() {
            // View the login page for adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Driver.Navigate().GoToUrl(test.AdHocReporting.URL);

            // Attempt to login with incorrect credentials
            test.AdHocReporting.Login("lukejiang88", "???///$$", "dc");

            // Verify the error messages
            Assert.IsTrue(test.Driver.PageSource.Contains("Login attempt failed."));
            Assert.IsFalse(test.Driver.PageSource.Contains("Username, password and church code are required."));

            // Close the browser
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Luke Jiang")]
        [Description("Verifies functionality when the user attempts to login to Adhoc Reporting using a church code to which the user does not belong to.")]
        public void AdhocReporting_Login_Validation_Unmatched_ChurchCode() {
            // View the login page for adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Driver.Navigate().GoToUrl(test.AdHocReporting.URL);

            // Attempt to login with a church code to which the user does not belong to
            test.AdHocReporting.Login("lukejiang88", "chrdwhdhXT1!", "bevo");

            // Verify the error messages
            Assert.IsTrue(test.Driver.PageSource.Contains("Login attempt failed."));
            Assert.IsFalse(test.Driver.PageSource.Contains("Username, password and church code are required."));

            // Close the browser
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Luke Jiang")]
        [Description("Verify the error message when trying to login username is slash /")]
        public void AdhocReporting_Login_Validation_UserName_Is_Slash() {
            // View the login page for adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Driver.Navigate().GoToUrl(test.AdHocReporting.URL);

            // Attempt to login
            test.AdHocReporting.Login("/", "chrdwhdhXT1!", "dc");

            // Verify the error messages
            Assert.IsTrue(test.Driver.PageSource.Contains("Login attempt failed."));
            Assert.IsFalse(test.Driver.PageSource.Contains("Username, password and church code are required."));

            // Close the browser
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Luke Jiang")]
        [Description("Verify the error message when trying to login username and churchcode are 100 characters.")]
        public void AdhocReporting_Login_Validation_UserName_ChurchCode_100Characters() {
            // View the login page for adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Driver.Navigate().GoToUrl(test.AdHocReporting.URL);

            // Attempt to login
            test.AdHocReporting.Login("abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuv", "chrdwhdhXT1!", "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuv");

            // Verify the error messages
            Assert.IsTrue(test.Driver.PageSource.Contains("Login attempt failed."));
            Assert.IsFalse(test.Driver.PageSource.Contains("Username, password and church code are required."));

            // Close the browser
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Luke Jiang")]
        [Description("Verify the error message when trying to login username sql injection")]
        public void AdhocReporting_Login_Validation_UserName_Is_SQLInjection() {
            // View the login page for adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Driver.Navigate().GoToUrl(test.AdHocReporting.URL);

            //Attempt to login
            test.AdHocReporting.Login("' or '1'='1'", "chrdwhdhXT1!", "dc");

            // Verify the error messages
            Assert.IsTrue(test.Driver.PageSource.Contains("Login attempt failed."));
            Assert.IsFalse(test.Driver.PageSource.Contains("Username, password and church code are required."));

            // Close the browser
            test.Driver.Close();
        }
        #endregion Validation

        #region Local User
        [Test, RepeatOnFailure(Order = 1)]
        [Author("Luke Jiang")]
        [Description("Verify the error message when trying to login but without any report rights")]
        public void AdhocReporting_Login_LocalUser_Without_Any_ReportRights() {
            // Set initial conditions
            string login = "lukejiang88";
            base.SQL.Admin_Users_DeleteSecurityRolesBySecurityGroup(15, login, 4);

            // Login to adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.AdHocReporting.Login(login, "chrdwhdhXT1!", "dc");

            // Verify error message present
            Assert.IsFalse(test.Driver.PageSource.Contains("Login attempt failed."));
            Assert.IsTrue(test.Driver.PageSource.Contains("You do not have sufficient rights to access this application."));

            // Verify 'sign out' link not present
            Assert.IsFalse(test.Driver.FindElementsByLinkText("sign out").Count > 0);

            // Close the browser
            test.Driver.Close();
        }

        [Test, RepeatOnFailure(Order = 2)]
        [Author("Luke Jiang")]
        [Description("Verify the error message when trying to login with at least one report rights")]
        public void AdhocReporting_Login_LocalUser_With_One_ReportRight_Not_Contribution_Or_Ministry() {
            // Set initial conditions
            string login = "lukejiang88";
            string[] securityRights = new string[] { "Contact" };
            base.SQL.Admin_Users_DeleteSecurityRolesBySecurityGroup(15, login, 4);
            base.SQL.Admin_Users_CreateSecurityRoles(15, login, 4, securityRights);

            // Login to adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.AdHocReporting.Login(login, "chrdwhdhXT1!", "dc");

            // Click the link to design a new report
            test.Driver.FindElementByLinkText("Design a New Report").Click();

            // Verify 'Date_' and 'Group' and 'People_'  present in Data source
            test.AdHocReporting.Security_Verify_DataSource_Displayed_Correctly(securityRights);

            // Logout of adhoc reporting
            test.AdHocReporting.Logout();
        }

        [Test, RepeatOnFailure(Order = 3)]
        [Author("Luke Jiang")]
        [Description("Verify the error message when trying to login with local User's contribution report rights")]
        public void AdhocReporting_Login_LocalUser_With_Contribution_ReportRight_Only() {
            // Set initial conditions
            string login = "lukejiang88";
            string[] securityRights = new string[] { "Contribution" };
            base.SQL.Admin_Users_DeleteSecurityRolesBySecurityGroup(15, login, 4);
            base.SQL.Admin_Users_CreateSecurityRoles(15, login, 4, securityRights);

            // Login to adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.AdHocReporting.Login(login, "chrdwhdhXT1!", "dc");

            // Click the link to design a new report
            test.Driver.FindElementByLinkText("Design a New Report").Click();

            // Verify 'Contribution_' present in Data source
            test.AdHocReporting.Security_Verify_DataSource_Displayed_Correctly(securityRights);

            // Logout of adhoc reporting
            test.AdHocReporting.Logout();
        }

        [Test, RepeatOnFailure(Order = 4)]
        [Author("Luke Jiang")]
        [Description("Verify the error message when trying to login with local User's ministry report rights")]
        public void AdhocReporting_Login_LocalUser_With_Ministry_ReportRight_Only() {
            // Set initial conditions
            string login = "lukejiang88";
            string[] securityRights = new string[] { "Ministry" };
            base.SQL.Admin_Users_DeleteSecurityRolesBySecurityGroup(15, login, 4);
            base.SQL.Admin_Users_CreateSecurityRoles(15, login, 4, securityRights);

            // Login to adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.AdHocReporting.Login(login, "chrdwhdhXT1!", "dc");

            // Click the link to design a new report
            test.Driver.FindElementByLinkText("Design a New Report").Click();

            // Verify 'Ministry_' present in Data source
            test.AdHocReporting.Security_Verify_DataSource_Displayed_Correctly(securityRights);

            // Logout of adhoc reporting
            test.AdHocReporting.Logout();
        }

        [Test, RepeatOnFailure(Order = 5)]
        [Author("Luke Jiang")]
        [Description("Verify the error message when trying to login with local User's contribution/ministry report rights")]
        public void AdhocReporting_Login_LocalUser_With_Contribution_And_Ministry_ReportRight_Only() {
            // Set initial conditions
            string login = "lukejiang88";
            string[] securityRights = new string[] { "Contribution", "Ministry" };
            base.SQL.Admin_Users_DeleteSecurityRolesBySecurityGroup(15, login, 4);
            base.SQL.Admin_Users_CreateSecurityRoles(15, login, 4, securityRights);

            // Login to adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.AdHocReporting.Login(login, "chrdwhdhXT1!", "dc");

            // Click the link to design a new report
            test.Driver.FindElementByLinkText("Design a New Report").Click();

            // Verify 'contribution_' and Ministry_' present in Data source
            test.AdHocReporting.Security_Verify_DataSource_Displayed_Correctly(securityRights);

            // Logout of adhoc reporting
            test.AdHocReporting.Logout();
        }

        [Test, RepeatOnFailure(Order = 6)]
        [Author("Luke Jiang")]
        [Description("Verify the local user login with All specific report rights except Contribution and ministry right")]
        public void AdhocReporting_Login_LocalUser_With_All_ReportRights_Not_Contribution_And_Ministry() {
            // Set initial conditions
            string login = "lukejiang88";
            string[] securityRights = new string[] { "Administrator", "Contact", "Contributor Summaries", "Contributor Visibility", "Data Export", "Individual Note", "Medical Module", "Ministry Contribution Summary", "Relationship", "Volunteer Management" };
            base.SQL.Admin_Users_DeleteSecurityRolesBySecurityGroup(15, login, 4);
            base.SQL.Admin_Users_CreateSecurityRoles(15, login, 4, securityRights);

            // Login to adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.AdHocReporting.Login(login, "chrdwhdhXT1!", "dc");

            // Click the link to design a new report
            test.Driver.FindElementByLinkText("Design a New Report").Click();

            // Verify 'Date_' and 'Group_' and 'People_' present in Data source, but 'Contribution_' and 'Ministry_' not present
            test.AdHocReporting.Security_Verify_DataSource_Displayed_Correctly(securityRights);

            // Logout of adhoc reporting
            test.AdHocReporting.Logout();
        }
        #endregion Local User

        #region Super User
        [Test, RepeatOnFailure(Order = 1)]
        [Author("Luke Jiang")]
        [Description("Verify the error message when trying to login but without any report rights")]
        public void AdhocReporting_Login_SuperUser_Without_Any_ReportRights() {
            // Set initial conditions
            string login = "global_reports";
            base.SQL.Admin_Users_DeleteSecurityRolesBySecurityGroup(600, login, 4);

            // Login to adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.AdHocReporting.Login(login, "@FTAdmin0", "AdHoc");

            // Verify error message present
            Assert.IsFalse(test.Driver.PageSource.Contains("Login attempt failed."));
            Assert.IsTrue(test.Driver.PageSource.Contains("You do not have sufficient rights to access this application."));

            // Verify 'sign out' link not present
            Assert.IsFalse(test.Driver.FindElementsByLinkText("sign out").Count > 0);

            // Close the browser
            test.Driver.Close();
        }

        [Test, RepeatOnFailure(Order = 2)]
        [Author("Luke Jiang")]
        [Description("Verify the super user login with one specific report rights but not Contribution or Ministry right")]
        public void AdhocReporting_Login_SuperUser_With_One_ReportRights_Not_Contribution_OR_Ministry() {
            // Set initial conditions
            string login = "global_reports";
            string[] securityRights = new string[] { "Administrator" };
            base.SQL.Admin_Users_DeleteSecurityRolesBySecurityGroup(600, login, 4);
            base.SQL.Admin_Users_CreateSecurityRoles(600, login, 4, securityRights);

            // Login to adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.AdHocReporting.Login(login, "@FTAdmin0", "AdHoc");

            // Click the link to design a new report
            test.Driver.FindElementByLinkText("Design a New Report").Click();

            // Verify 'Dat_' and 'Group_' and 'People_' present in Data source
            test.AdHocReporting.Security_Verify_DataSource_Displayed_Correctly(securityRights);

            // Logout of adhoc reporting
            test.AdHocReporting.Logout();
        }

        [Test, RepeatOnFailure(Order = 3)]
        [Author("Luke Jiang")]
        [Description("Verify the super user login with Contribution right")]
        public void AdhocReporting_Login_SuperUser_With_Contribution_Reportright_Only() {
            // Set initial conditions
            string login = "global_reports";
            string[] securityRights = new string[] { "Contribution" };
            base.SQL.Admin_Users_DeleteSecurityRolesBySecurityGroup(600, login, 4);
            base.SQL.Admin_Users_CreateSecurityRoles(600, login, 4, securityRights);

            // Login to adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.AdHocReporting.Login(login, "@FTAdmin0", "AdHoc");

            // Click the link to design a new report
            test.Driver.FindElementByLinkText("Design a New Report").Click();

            // Verify 'Contribution_' Data source present in Data source
            test.AdHocReporting.Security_Verify_DataSource_Displayed_Correctly(securityRights);

            // Logout of adhoc reporting
            test.AdHocReporting.Logout();
        }

        [Test, RepeatOnFailure(Order = 4)]
        [Author("Luke Jiang")]
        [Description("Verify the super user login with ministry right")]
        public void AdhocReporting_Login_SuperUser_With_Ministry_Reportright_Only() {
            // Set initial conditions
            string login = "global_reports";
            string[] securityRights = new string[] { "Ministry" };
            base.SQL.Admin_Users_DeleteSecurityRolesBySecurityGroup(600, login, 4);
            base.SQL.Admin_Users_CreateSecurityRoles(600, login, 4, securityRights);

            // Login to adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.AdHocReporting.Login(login, "@FTAdmin0", "AdHoc");

            // Click the link to design a new report
            test.Driver.FindElementByLinkText("Design a New Report").Click();

            // Verify 'Ministry_' Data source present in Data source
            test.AdHocReporting.Security_Verify_DataSource_Displayed_Correctly(securityRights);

            // Logout of adhoc reporting
            test.AdHocReporting.Logout();
        }

        [Test, RepeatOnFailure(Order = 5)]
        [Author("Luke Jiang")]
        [Description("Verify the super user login with Contribution and ministry right")]
        public void AdhocReporting_Login_SuperUser_With_Contribution_And_Ministry_Reportright_Only() {
            // Set initial conditions
            string login = "global_reports";
            string[] securityRights = new string[] { "Contribution", "Ministry" };
            base.SQL.Admin_Users_DeleteSecurityRolesBySecurityGroup(600, login, 4);
            base.SQL.Admin_Users_CreateSecurityRoles(600, login, 4, securityRights);

            // Login to adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.AdHocReporting.Login(login, "@FTAdmin0", "AdHoc");

            // Click the link to design a new report
            test.Driver.FindElementByLinkText("Design a New Report").Click();

            // Verify 'Contribution_' and 'Ministry_' Data source present in Data source
            test.AdHocReporting.Security_Verify_DataSource_Displayed_Correctly(securityRights);

            // Logout of adhoc reporting
            test.AdHocReporting.Logout();
        }

        [Test, RepeatOnFailure(Order = 6)]
        [Author("Luke Jiang")]
        [Description("Verify the super user login with All specific report rights except Contribution and ministry right")]
        public void AdhocReporting_Login_SuperUser_With_All_ReportRights_Not_Contribution_And_Ministry() {
            // Set initial conditions
            string login = "global_reports";
            string[] securityRights = new string[] { "Administrator", "Contact", "Contributor Summaries", "Contributor Visibility", "Data Export", "Individual Note", "Ministry Contribution Summary", "Relationship", "Volunteer Management" };
            base.SQL.Admin_Users_DeleteSecurityRolesBySecurityGroup(600, login, 4);
            base.SQL.Admin_Users_CreateSecurityRoles(600, login, 4, securityRights);

            // Login to adhoc reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.AdHocReporting.Login(login, "@FTAdmin0", "AdHoc");

            // Click the link to design a new report
            test.Driver.FindElementByLinkText("Design a New Report").Click();

            // Verify 'Date_' and 'Group_' and 'People_' Data source present in Data source
            test.AdHocReporting.Security_Verify_DataSource_Displayed_Correctly(securityRights);

            // Logout of adhoc reporting
            test.AdHocReporting.Logout();
        }
        #endregion Super User
        #endregion Login

        #region Sign Out
        [Test, RepeatOnFailure(Order = 13)]
        [Author("Luke Jiang")]
        [Description("Verify sign out function ")]
        public void AdhocReporting_Login_SignOut_Successfully() {
            // Set initial conditions
            string login = "lukejiang88";
            string[] securityRights = new string[] { "Contribution" };
            base.SQL.Admin_Users_DeleteSecurityRolesBySecurityGroup(15, login, 4);
            base.SQL.Admin_Users_CreateSecurityRoles(15, login, 4, securityRights);

            // Login to Adhoc Reporting
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.AdHocReporting.Login(login, "chrdwhdhXT1!", "dc");

            // Verify the brand name present
            Assert.AreEqual("Dynamic Church", test.Driver.FindElementById("brand_name").Text);

            // Verify the Page title after login successfully
            Assert.AreEqual("Report List Page", test.Driver.Title);

            // Logout of adhoc reporting
            test.AdHocReporting.Logout();
        }
        #endregion Sign Out
    }
}