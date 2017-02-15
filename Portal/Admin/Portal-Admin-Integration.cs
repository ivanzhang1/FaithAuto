using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace FTTests.Portal.Admin {

    [TestFixture]
    public class Portal_Admin_SSO_Applications : FixtureBaseWebDriver
    {

        #region Applications

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Felix Gaytan")]
        [Description("Verifies that all SSO API links are available")]
        public void Admin_Integration_Verify_SSO_API()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to admin->applications
            test.GeneralMethods.Navigate_Portal(Navigation.Admin.Integration.Applications);
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("SSO"), 20);

            //Are SSO applications there?
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("Fellowship One Portal API"));
            test.GeneralMethods.VerifyTextPresentWebDriver("Fellowship One Portal web application for use with the Fellowship One API Single Sign On.");

            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("Infellowship"));
            test.GeneralMethods.VerifyTextPresentWebDriver("Infellowship web application for use with the Fellowship One API Single Sign On.");

            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("Weblink"));
            test.GeneralMethods.VerifyTextPresentWebDriver("Weblink Congregation portal web application for use with the Fellowship One API Single Sign On.");

            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies SSO Applications cannot be disabled.")]
        public void Admin_Integration_Applications_SSO_cannot_revoke_access_Portal()
        {

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Navigate to admin->church contacts
            test.GeneralMethods.Navigate_Portal(Navigation.Admin.Integration.Applications);

            // View an application
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("Fellowship One Portal API"));
            test.Driver.FindElementByLinkText("Fellowship One Portal API").Click();

            // Verify that the Revoke access button is disabled.
            test.Driver.FindElement(By.Id("submitQuery")).GetAttribute("disabled=true");

            // Logout
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies SSO Applications cannot be disabled.")]
        public void Admin_Integration_Applications_SSO_cannot_revoke_access_Infellowship()
        {

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Navigate to admin->church contacts
            test.GeneralMethods.Navigate_Portal(Navigation.Admin.Integration.Applications);

            // View an application
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("Infellowship"));
            test.Driver.FindElementByLinkText("Infellowship").Click();

            // Verify that the Revoke access button is disabled.
            test.Driver.FindElement(By.Id("submitQuery")).GetAttribute("disabled=true");

            // Logout
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies SSO Applications cannot be disabled.")]
        public void Admin_Integration_Applications_SSO_cannot_revoke_access_Weblink()
        {

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Navigate to admin->church contacts
            test.GeneralMethods.Navigate_Portal(Navigation.Admin.Integration.Applications);

            // View an application
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("Weblink"));
            test.Driver.FindElementByLinkText("Weblink").Click();

            // Verify that the Revoke access button is disabled.
            test.Driver.FindElement(By.Id("submitQuery")).GetAttribute("disabled=true");

            // Logout
            test.Portal.LogoutWebDriver();

        }


        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies SSO Doc page.")]
        public void Admin_Integration_Applications_SSO_Link_Docs_Page()
        {

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to admin->applications
            test.GeneralMethods.Navigate_Portal(Navigation.Admin.Integration.Applications);

            //Store the current window handle
            string BaseWindow = test.Driver.CurrentWindowHandle;
            //TestLog.WriteLine("BaseWinHandle: " + BaseWindow);
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Fellowship One Portal API"));

            //Navigate to SSO docs
            int row = test.GeneralMethods.GetTableRowNumberWebDriver("//table[@class='grid']", "Fellowship One Portal API", "Name & Description", "contains");
            TestLog.WriteLine("Fellowship One Portal API found in row " + row);            
            test.Driver.FindElementByXPath(string.Format("//table[@class='grid']/tbody/tr[{0}]/td[2]/span/a", row + 1)).Click();
            //test.Driver.FindElement(By.LinkText("SSO")).Click();

            ReadOnlyCollection<string> handles = test.Driver.WindowHandles;

            //Should only have 2 windows
            Assert.AreEqual(2, handles.Count, "SSO Applications: More Than One Window Was Launched");

            foreach (string handle in handles)
            {

                //TestLog.WriteLine("WinHandle: " + handle);
                //TestLog.WriteLine(test.Driver.SwitchTo().Window(handle).Title.ToString());

                if (handle != BaseWindow)
                {
                    TestLog.WriteLine("SSO Docs Found");
                    Assert.AreEqual(test.Driver.SwitchTo().Window(handle).Title, "Fellowship One - Developer Community", "Did Not Go To SSO Doc Page");
                    break;
                }
            }

            // Verify the SSO docs page loads
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Login"), 20);

            test.GeneralMethods.VerifyTextPresentWebDriver("Single Sign On Client Implementation");

            //Close Browser
            test.Driver.Close();
        }

    }

    [TestFixture]
	public class Portal_Admin_F1Touch_Integration : FixtureBaseWebDriver
    {

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Felix Gaytan")]
        [Description("Revokes and then restores access to an F1Touch API application.")]
        public void Admin_Integration_F1Touch_RevokeRestoreAccess()
        {

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to admin->applications
            test.GeneralMethods.Navigate_Portal(Navigation.Admin.Integration.Applications);

            // View an application
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("F1Touch"));
            test.Driver.FindElementByLinkText("F1Touch").Click();

            // Revoke access, if not grant first then revoke
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("submitQuery"));
            TestLog.WriteLine("Button Access Text: " + test.Driver.FindElementById("submitQuery").GetAttribute("value"));

            test.Portal.GrantOrRevokeAccessWebDriver();

            // Verify back button remains and return to landing page                
            test.Driver.FindElementByLinkText("Back").Click();

            // Verify application is still present
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("F1Touch"));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("F1Touch"));

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        #endregion Applications

        #region Application Keys

        [Test, MultipleAsserts, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that Application Key options can be edited")]
        public void Admin_Integration_ApplicationKeys_Edit()
        {
            string appName = "Dynamic Church";
            string appNameEdit = "Dynamic Church Too";
            string version = "1.0";
            string versionEdit = "1.5";
            string description = "This is an application description";
            string email = "nfloyd@fellowshiptech.com";
            string emailEdit = "not_nfloyd@fellowshiptech.com";
            string homePageURI = "www.localhost.com";
            string homePageURIEdit = "www.localhost.com/edit/";
            string downloadURI = "https://downloads.fellowshipone.com";
            string callbackURI = "https://www.fellowshipone.com/";

            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            test.GeneralMethods.Navigate_Portal(Navigation.Admin.Integration.Application_Keys);

            //Edit Application Keys options
            test.Portal.EditApplicationKeys(appName, appNameEdit, versionEdit, description, emailEdit, homePageURIEdit, downloadURI, callbackURI);

            //Save application
            test.Driver.FindElementById(GeneralAdmin.ApplicationKeys.Edit_Save_Button).Click();
            test.GeneralMethods.WaitForElement(By.Id("tab_back"));

            //Verify Changes
            Assert.AreEqual(string.Format("{0} Edit", appNameEdit), test.Driver.FindElementByXPath("//table[@class='grid']/tbody/tr[1]/td").Text);
            Assert.AreEqual(versionEdit, test.Driver.FindElementByXPath("//table[@class='grid']/tbody/tr[2]/td").Text);
            Assert.AreEqual(homePageURIEdit, test.Driver.FindElementByXPath("//table[@class='grid']/tbody/tr[5]/td").Text);
            Assert.AreEqual(emailEdit, test.Driver.FindElementByXPath("//table[@class='grid']/tbody/tr[7]/td").Text);
            Assert.AreEqual(description, test.Driver.FindElementByXPath("//table[@class='grid']/tbody/tr[10]/td").Text);

            test.Driver.FindElementById(GeneralAdmin.ApplicationKeys.Back_Button).Click();
            test.GeneralMethods.WaitForElement(By.XPath(TableIds.Admin_ApplicationKeys));

            //Edit it all back
            test.Portal.EditApplicationKeys(appNameEdit, appName, version, null, email, homePageURI, null, null);

            test.Driver.FindElementById(GeneralAdmin.ApplicationKeys.Edit_Description).Clear();
            test.Driver.FindElementById(GeneralAdmin.ApplicationKeys.Edit_Download_URI).Clear();
            test.Driver.FindElementById(GeneralAdmin.ApplicationKeys.Edit_Callback_URI).Clear();

            //Save 
            test.Driver.FindElementById(GeneralAdmin.ApplicationKeys.Edit_Save_Button).Click();
            test.GeneralMethods.WaitForElement(By.Id("tab_back"));

            //Verify Changes
            Assert.AreEqual(string.Format("{0} Edit", appName), test.Driver.FindElementByXPath("//table[@class='grid']/tbody/tr[1]/td").Text);
            Assert.AreEqual(version, test.Driver.FindElementByXPath("//table[@class='grid']/tbody/tr[2]/td").Text);
            Assert.AreEqual(homePageURI, test.Driver.FindElementByXPath("//table[@class='grid']/tbody/tr[5]/td").Text);
            Assert.AreEqual(email, test.Driver.FindElementByXPath("//table[@class='grid']/tbody/tr[7]/td").Text);

            //Logout
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies error messages for required fields on Application keys edit page.")]
        public void Admin_Integration_ApplicationKeys_RequiredFields()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            string appName = "Dynamic Church";

            test.GeneralMethods.Navigate_Portal(Navigation.Admin.Integration.Application_Keys);

            //Click Edit
            int findRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Admin_ApplicationKeys, appName, "Application", "contains");
            string row = (findRow + 1).ToString();
            TestLog.WriteLine(string.Format("This is the name {0}, this is the row {1}", appName, row));
            test.Driver.FindElementByXPath(string.Format("//table[@id='douglasP']/tbody/tr[{0}]/td[5]/a", row)).Click();
            test.GeneralMethods.WaitForElement(By.Id(GeneralAdmin.ApplicationKeys.Edit_Application_Name));

            //Clear required fields 
            test.Driver.FindElementById(GeneralAdmin.ApplicationKeys.Edit_Application_Name).Clear();
            test.Driver.FindElementById(GeneralAdmin.ApplicationKeys.Edit_Contact_Email).Clear();

            //Attempt to Save
            test.Driver.FindElementById(GeneralAdmin.ApplicationKeys.Edit_Save_Button).Click();

            //Verify Error message
            Assert.AreEqual("Application Name is required.", test.Driver.FindElementByXPath("//dl[@id='error_message']/dd[1]").Text);
            Assert.AreEqual("Contact Email is required.", test.Driver.FindElementByXPath("//dl[@id='error_message']/dd[2]").Text);

            //Click Cancel
            test.Driver.FindElementByLinkText("Cancel");

            //Logout 
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("Verifies messages and link on Application keys page when it's disabled")]
        public void Admin_Integration_ApplicationKeys_DisabledContent_Check()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C1");

            test.GeneralMethods.Navigate_Portal(Navigation.Admin.Integration.Application_Keys);

            test.GeneralMethods.WaitForElement(By.XPath("//a[@href='http://developer.fellowshipone.com/']"));
            test.GeneralMethods.WaitForElement(By.LinkText("developer.fellowshipone.com"));

            //Logout 
            test.Portal.LogoutWebDriver();

        }


        #endregion Application Keys


    }

//Converted to WebDriver
//	[TestFixture]

	public class Portal_Admin_Integration : FixtureBase {
		#region Applications
		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Matthew Sneeden")]
		[Description("Revokes and then restores access to an API application.")]
		public void Admin_Integration_Applications_RevokeRestoreAccess() {


            // Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Navigate to admin->applications
			test.Selenium.Navigate(Navigation.Admin.Integration.Applications);

			// View an application
			test.Selenium.ClickAndWaitForPageToLoad("link=F1Touch");

			// Revoke access, if not grant first then revoke
            if (test.Selenium.IsElementPresent("//input[@value='Grant access']"))
            {

                test.Selenium.Click("//input[@value='Grant access']");
                test.Selenium.WaitForPageToLoad("30000");
                test.Selenium.VerifyTextPresent("This application is allowed to access your church's data.");

                // Verify back button remains and return to landing page
                test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Back);

                // Verify application is still present
                Assert.AreEqual("F1Touch", test.Selenium.GetText("link=F1Touch"));

                // View an application
                test.Selenium.ClickAndWaitForPageToLoad("link=F1Touch");

                test.Selenium.Click("//input[@value='Revoke access']");
                Assert.IsTrue(Regex.IsMatch(test.Selenium.GetConfirmation(), "^Revoking access will instantly disable the application for all users as well as revoking any valid tokens that exist for this application\\. Are you sure you want to do this[\\s\\S]$"));
                test.Selenium.WaitForPageToLoad("30000");
                test.Selenium.VerifyTextPresent("This application is not configured to access your data.");


            }
            else
            {

                test.Selenium.Click("//input[@value='Revoke access']");
                Assert.IsTrue(Regex.IsMatch(test.Selenium.GetConfirmation(), "^Revoking access will instantly disable the application for all users\\. Are you sure you want to do this[\\s\\S]$"));
                test.Selenium.WaitForPageToLoad("30000");
                test.Selenium.VerifyTextPresent("This application is not configured to access your data.");

                // Verify back button remains and return to landing page
                test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Back);

                // Verify application is still present
                Assert.AreEqual("F1Touch", test.Selenium.GetText("link=F1Touch"));

                // Restore access
                test.Selenium.ClickAndWaitForPageToLoad("link=F1Touch");
                test.Selenium.ClickAndWaitForPageToLoad("//input[@value='Grant access']");
                test.Selenium.VerifyTextPresent("This application is allowed to access your church's data.");
            }

			// Logout of portal
			test.Portal.Logout();
		}
		#endregion Applications

		#region Application Keys
		#endregion Application Keys

		#region Data Exchange Export
		#endregion Data Exchange Export

		#region Data Exchange Import
		#endregion Data Exchange Import
	}
}