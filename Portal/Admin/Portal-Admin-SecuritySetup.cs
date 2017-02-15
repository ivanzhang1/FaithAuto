using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Selenium;
using OpenQA.Selenium;

namespace FTTests.Portal.Admin {
	[TestFixture]
	public class Portal_Admin_SecuritySetup_WebDriver : FixtureBaseWebDriver {
		#region Portal Users

        //[Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Create a portal user account and login")]
        public void Admin_Portal_User_Create()
        {
            // Setup
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string userName = "newUser";
            string firstName = "New";
            string lastName = "User";
            string fullName = string.Format("{0} {1}", firstName, lastName);
            string password = "P@$$w0rd";
            string email = "email@email.com";

            base.SQL.Admin_Users_Delete(15, userName);

            //Login to portal
            test.Portal.LoginWebDriver();

            //Navigate to Portal Users page
            test.GeneralMethods.Navigate_Portal(Navigation.Admin.Security_Setup.Portal_Users);
            test.GeneralMethods.WaitForElement(By.LinkText("Add"));

            //Click Add 
            test.Driver.FindElementByLinkText("Add").Click();
            test.GeneralMethods.WaitForElement(By.Id("ctl00_ctl00_MainContent_content_txtLogin"));
            
            //Fill out form
            test.Portal.Admin_SecuritySetup_User_Create(userName, firstName, lastName, email, password, password, true);

            //Save
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSave").Click();

            //Logout 
            test.Portal.LogoutWebDriver();
            
            // Login new user
            test.Portal.LoginWebDriver(userName, password, "DC");

            test.Portal.LogoutWebDriver();

            //Delete User
            base.SQL.Admin_Users_Delete(15, userName);

        }

        [Test, RepeatOnFailure]
        [Author("Daniel Lee")]
        [Description("FO-2589 Disable Security Rights")]
        public void Admin_Portal_User_V2_Disable_Security_Rights()
        {
            // Setup
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            //Login to portal
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "qaeunlx0v2");

            //Navigate to Portal Users page
            test.GeneralMethods.Navigate_Portal(Navigation.Admin.Security_Setup.Portal_Users);
            test.GeneralMethods.WaitForElement(By.LinkText("Add"));

            try
            {
                //Navigate to Manage User Rights
                test.Driver.FindElementByXPath("//table[@id = 'ctl00_ctl00_MainContent_content_dgUsers']/tbody/tr[3]/td[7]/a[1]").Click();
                //test.GeneralMethods.WaitForElement(By.Id("ctl00_ctl00_MainContent_content_dgUsers_ctl03_lnkEditRights"));
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgUsers_ctl03_lnkEditRights").Click();

                //Verify 
                test.GeneralMethods.WaitForElement(By.Id("ctl00_ctl00_MainContent_content_btnSave"));
                string text1 = test.Driver.FindElementByXPath("//div[@id='ctl00_ctl00_MainContent_content_ctlRolesTreeView_trvAccessRights_t0_t2']/span[1]").Text.ToString();
                string text2 = test.Driver.FindElementByXPath("//div[@id='ctl00_ctl00_MainContent_content_ctlRolesTreeView_trvAccessRights_t0_t3']/span[1]").Text.ToString();

                Assert.AreEqual(text1, "Contribution Write    Managed by Vision2 Systems");
                Assert.AreEqual(text2, "Move Contributions / Receipts    Managed by Vision2 Systems");
                Assert.IsFalse(test.Driver.FindElementByXPath("//div[@id='ctl00_ctl00_MainContent_content_ctlRolesTreeView_trvAccessRights_t0_t2']/input[1]").Enabled);
                Assert.IsFalse(test.Driver.FindElementByXPath("//div[@id='ctl00_ctl00_MainContent_content_ctlRolesTreeView_trvAccessRights_t0_t3']/input[1]").Enabled);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
            //Logout 
            test.Portal.LogoutWebDriver();




        }

        ///------------------------------------------------------------ TO DO --------------------------------------------------------------------------------------------------------
        /// Create 
        /// Edit
        /// Link / unlink
        /// Create unlinked
        /// Inactive
        /// Missing Required fields 
        /// Missmatched passwords
        /// 

		#endregion Portal Users

		#region Security Roles

        /// --------------------------------------------TO DO ------------------------------------------------------------------------------------------------------------------------
        /// Create
        /// Edit
        /// Delete
        /// Admin
        /// Giving
        /// ALL ON
        /// ALL OFF

		#endregion Security Roles
	}
}