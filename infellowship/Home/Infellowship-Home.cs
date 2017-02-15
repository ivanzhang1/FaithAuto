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

namespace FTTests.infellowship.Home
{
    [TestFixture]
    public class Infellowship_Home_WebDriver : FixtureBaseWebDriver
    {
        #region Remove link & texts
        [Test, RepeatOnFailure]
        [Author("Winnie Wang")]
        [Description("Verify original links and texts are removed from Infellowship footer")]
        public void FTAPI_Footer_OldFooterRemove_WebDriver()
        {
            // Open Infellowship homepage
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("ftapi");

            // Verify 'Church Privacy Policy' link does not exist
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Privacy Policy")), "'Church Privacy Policy' still exists.");
            ////Assert.IsTrue(this._generalMethods.IsElementPresentWebDriver(By.Id("username")));                        

            // Verify 'Updated 12/12/13' text does not exist
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.XPath(".//*[text()='Updated 12/12/13']")), "'Updated' info still exists");
            
            //Verify 'Copyright © 2015' text does not exit
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.XPath(String.Format(".//*[normalize-space(text())='Copyright © {0}']", DateTime.Now.Year.ToString()))), "'Copyright' info still exists");
            
            //Verify 'The ACTIVE Network, Inc' text does not exit 
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.XPath(".//*[normalize-space(text())='The ACTIVE Network, Inc']")), "'Inc' info still exists");
        }
        #endregion Remove link & texts

        #region Add links & texts
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Winnie Wang")]
        [Description("Verify new links and texts are added to Infellowship footer")]
        public void FTAPI_Footer_NewFooterAdded_WebDriver()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("ftapi");

            // Verify Active logo exists and has correct href
            IWebElement activeLogo = test.Driver.FindElementById("footer_active_logo");
            string activeLogo_href = activeLogo.GetAttribute("href").ToString();
            Assert.AreEqual(activeLogo_href, "http://www.fellowshipone.com/");

            // Verify 'Terms of Use' link exists
            IWebElement termLink = test.Driver.FindElementByLinkText("Terms of Use");
            string termLink_href = termLink.GetAttribute("href").ToString();
            Assert.AreEqual(termLink_href, "http://www.activenetwork.com/information/terms-of-use");

            // Verify 'Copyright Policy' link exists
            IWebElement copyrighPolicytLink = test.Driver.FindElementByLinkText("Copyright Policy");
            string copyrightPolicyLink_href = copyrighPolicytLink.GetAttribute("href").ToString();
            Assert.AreEqual(copyrightPolicyLink_href, "http://www.activenetwork.com/information/copyright-policy");

            // Verify 'Your Privacy Rights' link exists
            IWebElement privacyLink = test.Driver.FindElementByLinkText("Your Privacy Rights");
            string privacyLink_href = privacyLink.GetAttribute("href").ToString();
            Assert.AreEqual(privacyLink_href, "http://www.activenetwork.com/information/privacy-policy");

            // Verify 'Cookie Policy' link exists
            IWebElement cookieLink = test.Driver.FindElementByLinkText("Cookie Policy");
            string cookieLink_href = cookieLink.GetAttribute("href").ToString();
            Assert.AreEqual(cookieLink_href, "http://www.activenetwork.com/information/cookie-policy");

            // Verify 'Security' link exists
            IWebElement securityLink = test.Driver.FindElementByLinkText("Security");
            string securityLink_href = securityLink.GetAttribute("href").ToString();
            Assert.AreEqual(securityLink_href, "http://www.activenetwork.com/information/security");

            // Verify copyright link exists
            IWebElement copyrightLink = test.Driver.FindElementByLinkText(String.Format("© {0} Active Network, LLC", DateTime.Now.Year.ToString()));
            string copyrightLink_href = copyrightLink.GetAttribute("href").ToString();
            Assert.AreEqual(copyrightLink_href, "http://www.activenetwork.com/");
                        
            // Verify TRUSTe icon exists
            test.GeneralMethods.WaitForElementDisplayed(By.Id("footer_truste_badge"));

        }      
        #endregion Add links & texts

        #region other
        ////#region Giving_ftAPI
        ////[Test, RepeatOnFailure]
        ////[Category(TestCategories.Services.SmokeTest)]
        ////[Author("Winnie Wang")]
        ////[Description("Verify links in Infellowship footer on Giving")]
        ////public void Giving_footer_ftAPI()
        ////{

        ////  //  CreateAccountAndLogin_WebDriver();


        ////}

        ////#endregion Giving_ftAPI


        ////// create and login();
        ////[Test, RepeatOnFailure]
        ////[Category(TestCategories.Services.SmokeTest)]
        ////[Author("Winnie Wang")]
        ////[Description("Create new account and login")]
        ////public void CreateAccountAndLogin_WebDriver()
        ////{
        ////    string email = "winnie.wang@activenetwork.com";
        ////    string password = "111111";

        ////    // Deletes InFellowship account. For church ftapi, id = 500
        ////    base.SQL.People_InFellowshipAccount_Delete(500, email);

        ////    // Open InFellowship
        ////    TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        ////    test.Infellowship.OpenWebDriver("ftapi");

        ////    //Create Account
        ////    string activationCode = test.Infellowship.People_Accounts_Create_Webdriver("500", "Created", "Account", email, password, false, "10/28/1945", "Female", "United States", "717 N Harwood St.", null, "Dallas", "Texas", "75201", "", "877-318-5669", null, null);

        ////    //Login to Created Account
        ////    test.Infellowship.LoginWebDriver(email, password, base.SQL.FetchChurchCode(500));

        ////    Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
        ////    Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
        ////    Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
        ////    Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
        ////    Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
        ////    Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));
        ////}

        #endregion other

    }
}