﻿using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace FTTests.Portal.Home
{
  
    [TestFixture]
    public class Portal_Home : FixtureBase
    {

        [Test, RepeatOnFailure, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        public void Home_Home()
        {
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Assert the title of the page.
            Assert.AreEqual("Fellowship One :: Welcome to Portal Home", test.Selenium.GetTitle());

			// Select home
			test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Home);

			// Verify there are no menu items under home
			Assert.AreEqual(0, test.Selenium.GetXpathCount("//div[@id='nav_sub_1']/dl"));

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Welcome to Portal Home");
			test.Selenium.VerifyTextPresent("My Tasks");
			test.Selenium.VerifyTextPresent("News & Announcements");
			test.Selenium.VerifyTextPresent("F1 Announcements");
			test.Selenium.VerifyTextPresent("Staff Birthdays");
			test.Selenium.VerifyTextPresent("Today");
			test.Selenium.VerifyTextPresent("New Staff");

			// Verify the number of main links present
            // Comment out the below check point, as we had a new feature added - Interactive Reports, and that will shown based on user's level (the new point will be covered in other TCs)
			//Assert.AreEqual(8, test.Selenium.GetXpathCount("id('nav')/div/ul/li"));

			// Verify the links present
			Assert.AreEqual("Home", test.Selenium.GetText("//div[@id='nav']/div/ul/li[1]"));
			Assert.AreEqual("People", test.Selenium.GetText("//div[@id='nav']/div/ul/li[2]"));
			Assert.AreEqual("Groups", test.Selenium.GetText("//div[@id='nav']/div/ul/li[3]"));
			Assert.AreEqual("Ministry", test.Selenium.GetText("//div[@id='nav']/div/ul/li[4]"));
			Assert.AreEqual("WebLink", test.Selenium.GetText("//div[@id='nav']/div/ul/li[5]"));
			Assert.AreEqual("Giving", test.Selenium.GetText("//div[@id='nav']/div/ul/li[6]"));
			Assert.AreEqual("Admin", test.Selenium.GetText("//div[@id='nav']/div/ul/li[7]"));
			Assert.AreEqual("Reports", test.Selenium.GetText("//div[@id='nav']/div/ul/li[8]"));

			// Verify the search button is present
			Assert.IsTrue(test.Selenium.IsElementPresent("header_button"));

			// Verify the "View all" link does not exist
			Assert.IsFalse(test.Selenium.IsElementPresent("link=View all"));

			// Verify the "Security" link text is present
			test.Selenium.VerifyElementPresent("link=Security");

            // Verify support link is present
            //test.Selenium.VerifyElementPresent("link=Support");
            test.Selenium.VerifyElementPresent("link=Help Center");
            // Verify copyright is present
            //test.Selenium.VerifyTextPresent(string.Format("© {0}", DateTime.Now.Year));

            //Updated by Jim: To adapt the change of copyright for each year
            test.Selenium.VerifyElementPresent(string.Format("link=© {0} Active Network, LLC", now.Year.ToString()));

            // Verify help image, link exists
            //LMS got rid of this
            //test.Selenium.VerifyElementPresent("//*[@id='icon_help']");
            test.Selenium.VerifyElementPresent("//*[@id='help_link']");

            // Select portal users link
            test.Selenium.ClickAndWaitForPageToLoad("link=Portal users");

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: View Portal Users");
            test.Selenium.VerifyTextPresent("View Portal Users");


            // Select home
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Home);

            // Select the edit link for the news and announcements
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Edit);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: News & Announcements");
            test.Selenium.VerifyTextPresent("Add/Edit News & Announcements");

            #region My Account
            // Select home
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Home);

            // View the account settings for the current portal user
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", test.Portal.PortalUser));

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: My Account");
            test.Selenium.VerifyTextPresent("My Account");

            // ** Cancel
            // Cancel to return to the home page
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Cancel);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Welcome to Portal Home");
            test.Selenium.VerifyTextPresent("My Tasks");
            #endregion My Account

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the Manage Applications page off of the My Account page.")]
        public void Home_MyAccount_ManageApplications()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View the account settings for the current portal user
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", test.Portal.PortalUser));

            // View the application access for the current portal user
            test.Selenium.ClickAndWaitForPageToLoad("link=Manage applications");

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Manage Applications");
            test.Selenium.VerifyTextPresent("Manage Applications");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to change the portal user password not following requirements.")]
        public void Home_MyAccount_ChangePasswordInvalid()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View the account settings for the current portal user
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", test.Portal.PortalUser));

            // Attempt to change the password not following the requirements
            test.Selenium.Type("ctl00_ctl00_MainContent_content_txtNewPassword", "m");
            test.Selenium.Type("ctl00_ctl00_MainContent_content_txtRptPassword", "m");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.Save);

            // Verify error text
            test.Selenium.VerifyTextPresent(new string[] { TextConstants.ErrorHeadingSingular, "The password entered is invalid." });

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Selects a user after viewing the View Portal Users page off of the Home page.")]
        public void Home_PortalUsers_SelectIndividual()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // View the portal users
            test.Selenium.ClickAndWaitForPageToLoad("link=Portal users");

            // View user summary for "Matthew Sneeden"
            test.Selenium.ClickAndWaitForPageToLoad("link=T");
            decimal itemRow = test.GeneralMethods.GetTableRowNumber(TableIds.Admin_PortalUsers, test.Portal.PortalUser, "Name", null);
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("{0}/tbody/tr[{1}]/td[2]/a", TableIds.Admin_PortalUsers, itemRow + 1));

            // Verify title, text
            test.Selenium.VerifyTitle(Portal.People.PeopleHeadingText.TitleFormat(Portal.People.PeopleHeadingText.Search_IndividualDetail));
            test.Selenium.VerifyTextPresent(Portal.People.PeopleHeadingText.Search_IndividualDetail);
            Assert.IsTrue(test.Selenium.GetText("//td[@id='household_individual_name']").Contains(test.Portal.PortalUser));

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views a user summary after viewing the View Portal Users page off of the Home page.")]
        public void Home_PortalUsers_ViewSummary()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // View the portal users
            test.Selenium.ClickAndWaitForPageToLoad("link=Portal users");

            // View the current user
            test.Selenium.ClickAndWaitForPageToLoad("link=T");
            decimal itemRow = test.GeneralMethods.GetTableRowNumber(TableIds.Admin_PortalUsers, test.Portal.PortalUser, "Name", null);
            test.Selenium.ClickAndWaitForCondition(string.Format("{0}/tbody/tr[{1}]/td[1]/a", TableIds.Admin_PortalUsers, itemRow + 1), JavaScriptMethods.IsElementPresent("lnkChangePassword"), "10000");

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: View Portal Users");
            test.Selenium.VerifyTextPresent("View Portal Users");
            Assert.IsTrue(test.Selenium.GetText("//strong[@id='lblName']") == test.Portal.PortalUser);

            // Verify the selected letter
            Assert.IsTrue(test.Selenium.GetText("//li[@class='current']/a") == "T");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the active group.")]
        public void Home_PortalUsers_ViewActiveGroup()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Set the active group by selecting a group
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);
            string groupName = test.Selenium.GetText("//table[@id='ctl00_ctl00_MainContent_content_ucGrpGrid_grdGroups']/tbody/tr[*]/td/a");
            //test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", groupName));

            // Return to the home page
            // test.Selenium.ClickAndWaitForPageToLoad("link=Home");

            // View the active group
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", groupName));

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: View Group");
            test.Selenium.VerifyTextPresent("View Group");
            Assert.IsTrue(test.Selenium.GetText("//p[@class='text_big_serif']") == groupName);

            // Return to the home page
            test.Selenium.ClickAndWaitForPageToLoad("link=Home");

            // Logout of portal
            test.Portal.Logout();
        }

        #region Help Link
        //[Test, RepeatOnFailure]
        //[Author("David Martin")]
        [Author("Suchitra Patnam")]
        [Description("Verifies the correct options appear under the new Help link.")]
        public void Home_HelpLink_VerifyLinksPresent()
        {

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            //Verify Help is there
            test.Selenium.VerifyElementPresent("id=help_menu");

            // Verify all the link options are there
            // Suchitra making changes for Help center changes for story 3976
            //test.Selenium.VerifyElementPresent("link=Help With This Feature");
            //test.Selenium.VerifyElementPresent("link=Fellowship One Learning Center");
            //test.Selenium.VerifyElementPresent("link=Customer Support Portal");
            //test.Selenium.VerifyElementPresent("link=Open a Support Request");
            //test.Selenium.VerifyElementPresent("link=Upgrades/Account Management Questions");


            /*
            //Verify if Help links are not visible
            Assert.IsFalse(test.Selenium.IsVisible("link=Help With This Feature"), "Link [Help With This Feature] should not be visible yet.");
            Assert.IsFalse(test.Selenium.IsVisible("link=Fellowship One Learning Center"), "Link [Fellowship One Learning Center] should not be visible yet.");
            Assert.IsFalse(test.Selenium.IsVisible("link=Customer Support Portal"), "Link [Customer Support Portal] should not be visible yet.");
            Assert.IsFalse(test.Selenium.IsVisible("link=Open a Support Request"), "Link [Open a Support Request] should not be visible yet.");
            Assert.IsFalse(test.Selenium.IsVisible("link=Upgrades/Account Management Questions"), "Link [Upgrades/Account Management Questions] should not be visible yet."); */


            test.Selenium.VerifyElementPresent("link=Help With This Feature");
            test.Selenium.VerifyElementPresent("link=Fellowship One Help Center");
            test.Selenium.VerifyElementPresent("link=Fellowship One Learning Center");
            test.Selenium.VerifyElementPresent("link=Manage Your Support Cases");
            test.Selenium.VerifyElementPresent("link=Upgrades/Account Management Questions");

            /*  test.Selenium.VerifyElementPresent("link=Help With This Feature");
              test.Selenium.VerifyElementPresent("link=Fellowship One Learning Center");
              test.Selenium.VerifyElementPresent("link=Customer Support Portal");
              test.Selenium.VerifyElementPresent("link=Open a Support Request");
              test.Selenium.VerifyElementPresent("link=Upgrades/Account Management Questions"); 
                    


              //Verify if Help links are not visible
              Assert.IsFalse(test.Selenium.IsVisible("link=Help With This Feature"), "Link [Help With This Feature] should not be visible yet.");
              Assert.IsFalse(test.Selenium.IsVisible("link=Fellowship One Learning Center"), "Link [Fellowship One Learning Center] should not be visible yet.");
              Assert.IsFalse(test.Selenium.IsVisible("link=Customer Support Portal"), "Link [Customer Support Portal] should not be visible yet.");
              Assert.IsFalse(test.Selenium.IsVisible("link=Open a Support Request"), "Link [Open a Support Request] should not be visible yet.");
              Assert.IsFalse(test.Selenium.IsVisible("link=Upgrades/Account Management Questions"), "Link [Upgrades/Account Management Questions] should not be visible yet."); */


            test.Selenium.VerifyElementPresent("link=Fellowship One Help Center");
            test.Selenium.ClickAndWaitForPageToLoad("link=Fellowship One Help Center");

            test.Selenium.SelectWindow("title=Fellowship One | Help Center");
            /*  string BaseWindow = test.Driver.CurrentWindowHandle;
              ReadOnlyCollection<string> handles = test.Driver.WindowHandles;
              test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]); */
            test.Selenium.VerifyTextPresent("FELLOWSHIP ONE | HELP CENTER");

            test.Selenium.VerifyElementPresent("link=Fellowship One Learning Center");
            test.Selenium.VerifyElementPresent("link=Manage Your Support Cases");
            test.Selenium.VerifyElementPresent("link=Upgrades/Account Management Questions");

            //Assert.IsFalse(test.Selenium.IsVisible("link=Help With This Feature"), "Link [Help With This Feature] should not be visible yet.");
            //Assert.IsFalse(test.Selenium.IsVisible("link=Fellowship One Help Center"), "Link [Fellowship One Help Center] should not be visible yet.");
            //Assert.IsFalse(test.Selenium.IsVisible("link=Fellowship One Learning Center"), "Link [Fellowship One Learning Center] should not be visible yet.");
            //Assert.IsFalse(test.Selenium.IsVisible("link=Manage Your Support Cases"), "Link [Manage Your Support Cases] should not be visible yet.");
            //Assert.IsFalse(test.Selenium.IsVisible("link=Upgrades/Account Management Questions"), "Link [Upgrades/Account Management Questions] should not be visible yet.");

            // Click on the Help Link
            // //*[@id="help_menu"]
            //<ul style="top: 103px; left: 1020px; display: none;" class="active_links_list" id="help_menu">

            //test.Selenium.Click("help_link");
            //Assert.IsTrue(test.Selenium.IsVisible("link=Help With This Feature"), "Link [Help With This Feature] should not be visible yet.");
            //Assert.IsTrue(test.Selenium.IsVisible("link=Fellowship One Help Center"), "Link [Fellowship One Help Center] should not be visible yet.");
            //Assert.IsTrue(test.Selenium.IsVisible("link=Fellowship One Learning Center"), "Link [Fellowship One Learning Center] should not be visible yet.");
            //Assert.IsTrue(test.Selenium.IsVisible("link=Manage Your Support Cases"), "Link [Manage Your Support Cases] should not be visible yet.");
            //Assert.IsTrue(test.Selenium.IsVisible("link=Upgrades/Account Management Questions"), "Link [Upgrades/Account Management Questions] should not be visible yet.");

            // Click on link to submit support request
            // test.Selenium.ClickAndWaitForPageToLoad("link=Open a Support Request");
            // test.Selenium.SelectWindow("title=Request Support | Fellowship One");
            // test.Selenium.VerifyTextPresent("This is the place where all Fellowship One customers can share ideas, learn, and get help.");
            // test.Selenium.Close();




            //   test.Selenium.ClickAndWaitForPageToLoad("link=Fellowship One Learning Center");
            test.Selenium.SelectWindow("title=Active Network");

            test.Selenium.ClickAndWaitForPageToLoad("link=Manage Your Support Cases");
            test.Selenium.SelectWindow("title=Active Customer Portal");



            // Logout of Portal
            test.Portal.Logout();
        }
        #endregion Help Link
    }
    
    [TestFixture]
    public class Portal_Home_WebDriver : FixtureBaseWebDriver
    {
        #region Home
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Suchitra Patnam")]
        [Description("Verifies that user is taken to Help Center")]
        public void Home_Help_Center()
        {

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            test.GeneralMethods.VerifyTextPresentWebDriver("Help Center");

            string BaseWindow = test.Driver.CurrentWindowHandle;
            test.Driver.FindElementByLinkText("Help Center").Click();

            Retry.WithPolling(500).WithTimeout(15000).Until(() => test.Driver.WindowHandles.Count > 1);
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]);

            Assert.AreEqual("http://activesupport.force.com/fellowshipone/", test.Driver.Url);
            //Assert.IsTrue(test.Driver.FindElementByXPath("/html/body/div[2]").Equals("ManageCaseButton"));
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]).Close();

        }

        /// <summary>
        ///  New Help Link changes
        /// </summary>
        //[Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4115 KA - Update Help Links")]
        public void Home_HelpLink_HelpWithThisFeature()
        {

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//*[@id='help_link']"));

            //Verify Help is there
            test.Driver.FindElementByXPath("//*[@id='help_link']").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Help With This Feature"));
            test.GeneralMethods.VerifyElementDisplayedWebDriver(By.LinkText("Help With This Feature"));

            //Click on "Help With This Feature
            //test.Driver.FindElementByXPath("//*[@id='help_menu']/li[1]/a").Click();
            test.Driver.FindElement(By.LinkText("Help With This Feature")).Click();

            Retry.WithPolling(500).WithTimeout(15000).Until(() => test.Driver.WindowHandles.Count > 1);
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]);
           // Assert.AreEqual("https://help.int.fellowshipone.com/fellowshipone.htm#cshid=3127", test.Driver.Url);
            Assert.AreEqual("Getting Started",test.Driver.Title);
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]).Close();
            
        }

        //Commented this test case since we revert back the changes in production for the story F1-4115..
        //[Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4115 KA - Update Help Links")]
        public void Home_HelpLink_AdditionalHelpResources()
        {

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//*[@id='help_link']"));

            //Verify Help is there
            test.Driver.FindElementByXPath("//*[@id='help_link']").Click();
            //test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Additional help resources"));

            //Click on "Help With This Feature
            test.Driver.FindElementByXPath("//*[@id='help_menu']/li[2]/a").Click();
            //test.Driver.FindElement(By.LinkText("Additional help resources")).Click();

            Retry.WithPolling(500).WithTimeout(15000).Until(() => test.Driver.WindowHandles.Count > 1);
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]);
            Assert.AreEqual("http://www.fellowshipone.com/help-resources", test.Driver.Url);
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]).Close();

        }
        
        // Uncommented these test cases since we reverted the changes in production..
        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("Help Center")]
        public void Home_HelpLink_HelpCenter()
        {

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//*[@id='help_link']"));

            //Verify Help is there
            test.Driver.FindElementByXPath("//*[@id='help_link']").Click();
            //test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Additional help resources"));

            //Click on "Help With This Feature
            test.Driver.FindElementByXPath("//*[@id='help_menu']/li[2]/a").Click();
            //test.Driver.FindElement(By.LinkText("Additional help resources")).Click();

            Retry.WithPolling(500).WithTimeout(15000).Until(() => test.Driver.WindowHandles.Count > 1);
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]);
            Assert.AreEqual("http://activesupport.force.com/fellowshipone/", test.Driver.Url);
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]).Close();

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        public void Home_HelpLink_LearningCenter()
        {

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("felixg", "FG.Admin12"); 

            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Learning Center")));

            test.Driver.FindElementById("help_link").Click();
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Fellowship One Learning Center")));
            
            test.Driver.Close();

        } 

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        public void Home_HelpLink_CustomerPortal()
        {

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//*[@id='help_link']"));

            //Verify Help is there
            test.Driver.FindElementByXPath("//*[@id='help_link']").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Manage Your Support Cases"));
            test.GeneralMethods.VerifyElementDisplayedWebDriver(By.LinkText("Manage Your Support Cases"));

            ///html/body/div[1]/ul/li[3]
            Assert.AreEqual("Manage Your Support Cases", test.Driver.FindElementByXPath("//*[@id='help_menu']/li[3]/a").Text);
            Assert.AreEqual("http://customerportal.activenetwork.com/", test.Driver.FindElementByXPath("//*[@id='help_menu']/li[3]/a").GetAttribute("href"));
            test.Driver.FindElementByXPath("//*[@id='help_menu']/li[3]/a").Click();


            Retry.WithPolling(500).WithTimeout(15000).Until(() => test.Driver.WindowHandles.Count > 1);
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]);
            
            //Issues with launching customer portal
            //Assert.AreEqual("http://customerportal.activenetwork.com", test.Driver.Url);
            //Assert.AreEqual("https://activesupport.secure.force.com/customerportal", test.Driver.Url);
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]).Close();

            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[0]);
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Suchitra Patnam")]
        [Description("F1-4049: Editing News and Announcements section from Home page")]
        public void Home_NewsAndAnnouncements()
        {

            //Default values
            string NewsText = "Testing updates to News & Announcements!!";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            //Edit News and Announcements section
            test.Driver.FindElementByLinkText("Edit").Click();

            //Adding text to News and Announcements section
            
            IWebElement tiny_mce_frame = test.Driver.FindElementById("tiny_mce_editor_ifr");
            test.Driver.SwitchTo().Frame(tiny_mce_frame);
            test.Driver.FindElement(By.CssSelector(".mceContentBody")).Clear();
            test.Driver.FindElement(By.CssSelector(".mceContentBody")).SendKeys(NewsText);
            test.Driver.SwitchTo().DefaultContent();
            test.Driver.FindElementByXPath("//*[@id='ctl00_ctl00_MainContent_content_cmdSave']").Click();
            
            //Return back to Home page to verify text
            test.Driver.FindElementByLinkText("Home").Click();

            //Verifying added text under home page
            IWebElement sandbox_frame = test.Driver.FindElementById("sandboxed_iframe");
            test.Driver.SwitchTo().Frame(sandbox_frame);
            Assert.AreEqual(NewsText, test.Driver.FindElement(By.CssSelector("body")).Text);
            test.Driver.SwitchTo().DefaultContent();                     

            //Logout from Portal
            test.Portal.LogoutWebDriver();

        }
        
        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verify Privacy Policy")]
        public void Portal_PrivacyPolicy()
        {
            //Login to Weblink
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            //Verify Privacy Policy
            Assert.IsTrue(test.Driver.FindElement(By.LinkText("Your Privacy Rights")).Displayed, "No Privacy Policy Found");
            IWebElement privacyElement = test.Driver.FindElementByLinkText("Your Privacy Rights");
            Assert.Contains(privacyElement.GetAttribute("href"), "http://www.activenetwork.com/information/privacy-policy", "Privacy Policy Link Not Found");
            privacyElement.Click();

            TestLog.WriteLine("Privacy Policy Found");
            Assert.AreEqual(test.Driver.Title, "Your Privacy Rights", "Did Not Go To Privacy Policy Page");
            test.GeneralMethods.VerifyTextPresentWebDriver("Your Privacy Rights");

            // Close the Weblink Session
            test.Driver.Close();

        }
        #endregion Home

        #region Login
        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Category(TestCategories.Services.SmokeTest)]
        [Description("Verifies that the user security feature is functioning properly")]
        public void Home_User_Login_Fail_MaxAttempts()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string username = "lockeduser";
            string password = "FT4life!";
            string wrongPassword = "WrongPassword";
            string churchCode = "dc";
            int churchId = 15;
            //Reset Count
            test.SQL.Admin_Set_User_RetryCount(churchId, username, 0);
            
            //Login eleven times
            test.Portal.LoginFailMaxAttemptsWebdriver(username, wrongPassword, churchCode);

            //Attempt to login with correct password
            test.Portal.LoginFailWebDriver(username, password, churchCode);
            string lastLogin = string.Format("{0}:00.000", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMinutes(-16).ToString("yyyy-MM-dd HH:mm"));
            TestLog.WriteLine(lastLogin);
            //Verify locked account
            Assert.AreEqual("Your account is currently locked. Please try again later.", test.Driver.FindElementByXPath("//form[@id='aspnetForm']/p[1]").Text);

            //Set Fail time to more than 15 minutes
            test.SQL.Admin_Set_User_LastFailLoginDate(churchId, username, lastLogin);

            //Login with valid password
            test.Portal.LoginWebDriver(username, password);

            //Logout
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure, Timeout(1200)]
        [Author("Stuart Platt")]
        [Description("Resets locked user password and verifies user can login")]
        public void Home_User_Login_Fail_AdminReset()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string username = "lockedusertwo";
            string fullName = "Locked Usertwo";
            string password = "FT4life!";
            string wrongPassword = "WrongPassword";
            string churchCode = "dc";
            string adminUser = "ft.tester";
            int churchId = 15;
            //Reset count  
            test.SQL.Admin_Set_User_RetryCount(churchId, username, 0);

            //Login eleven times
            test.Portal.LoginFailMaxAttemptsWebdriver(username, wrongPassword, churchCode);

            // Login as admin
            test.Portal.LoginWebDriver(adminUser, password, churchCode);

            //Reset Password as admin
            test.Portal.Admin_SecuritySetup_User_PasswordReset(fullName, password); 
            
            // Logout as Admin
            test.Portal.LogoutWebDriver();
            test.GeneralMethods.WaitForElement(By.Id("ctl00_content_userNameText"));

            //Login as user with valid password
            test.Portal.LoginWebDriver(username, password, churchCode);

            //Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies that the links on sign in page work well")]
        public void Home_User_Login_Links_Check()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            test.Portal.OpenLoginWebDriver();

            test.GeneralMethods.WaitForElement(By.XPath("//a[@href='https://www.youtube.com/watch?v=C5_cJij-J64&index=1&list=PLnxjkDdIeeqQZLAZ3lnU8qcUUD6Ro59EQ']"));
            test.GeneralMethods.WaitForElement(By.XPath("//a[@href='http://www.fellowshipone.com']"));
            test.GeneralMethods.WaitForElement(By.LinkText("Fellowship One"));

            test.GeneralMethods.WaitForElement(By.XPath("//a[@href='http://Activesupport.force.com/fellowshipone/']"));
            test.GeneralMethods.WaitForElement(By.LinkText("Customer Support Portal"));
            test.GeneralMethods.WaitForElement(By.XPath("//a[@href='mailto:F1Support@activenetwork.com']"));
            test.GeneralMethods.WaitForElement(By.LinkText("F1Support@activenetwork.com"));
        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        public void Home_Page_View_My_Account_Notifications_Check_Box()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("Alfred", "Alfred1@"); //modified by Clark.Peng, login with Alfred so that the checkbox can be check or uncheck. The default user(App.config) can't check or un check the checkbox.

            test.Driver.FindElementByXPath("//a[@href='/UserAccount/Index.aspx']").Click();

            bool expectStatus = true;
            if (test.Driver.FindElementById("chkUpdates").Selected)
            {
                test.Driver.FindElementById("chkUpdates").Click(); //Added by Clark.Peng, to restore the checkbox default status. 
                //expectStatus = false;
            }

            // Check the Account Notifications check box
            test.Driver.FindElementById("chkUpdates").Click();

            // Save account changes
            test.Driver.FindElementById(GeneralButtons.Save).Click();

            // Select the user name
            test.Driver.FindElementByXPath("//a[@href='/UserAccount/Index.aspx']").Click();

            // Verify the check box remained checked.
            Assert.IsTrue(expectStatus == test.Driver.FindElementById("chkUpdates").Selected);


            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        #endregion Login
    }
}
