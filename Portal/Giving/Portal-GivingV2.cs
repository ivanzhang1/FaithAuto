﻿using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace FTTests.Portal.GivingV2 {
	// [TestFixture]
	public class Portal_GivingV2 : FixtureBaseWebDriver {
	
        #region V2

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Daniel Lee")]
        [Description("FO-2579: CUI Shortcut Links in AUI")]
        public void V2_WebLink_OnlineGiving_Links_Reroute()
        {
            // Login portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "qaeunlx0v2");

            // Navigate to Features page
            test.GeneralMethods.Navigate_Portal(Navigation.WebLink.InFellowship.Features);

            // Click on the Online Giving section
            test.Driver.FindElementByXPath(GeneralWebLink.InFellowship.Features.Online_Giving).Click();

            string url = test.Driver.FindElementByXPath("//div[@id='church_directory']/div[2]/div[1]/h3[1]/a[1]").GetAttribute("href").ToString();

            // Get the urls
            string url1 = test.Driver.FindElementByXPath("//div[@id='online_giving']/div[2]/div[1]/form[1]/table[1]/tbody/tr[1]/td[1]/label[1]/a[1]").GetAttribute("href").ToString();
            string url2 = test.Driver.FindElementByXPath("//div[@id='online_giving']/div[2]/div[1]/form[1]/table[1]/tbody/tr[2]/td[1]/label[1]/a[1]").GetAttribute("href").ToString();
            string url3 = test.Driver.FindElementByXPath("//div[@id='online_giving']/div[2]/div[1]/form[1]/table[1]/tbody/tr[3]/td[1]/label[1]/a[1]").GetAttribute("href").ToString();
            string[] actualURL = new string[] {url1, url2, url3};
            string[] expectURL = new string[] { Regex.Replace(url, "directory", "OnlineGiving/GiveNow/V2Giving"), Regex.Replace(url, "directory", "OnlineGiving/ScheduledGiving/ScheduledGivingV2"), Regex.Replace(url, "directory", "OnlineGiving/GiveNow/NoAccountV2Giving") };
            
            //Verify urls
            for (int i = 1; i <= 3; i++)
            {
                Assert.AreEqual(expectURL[i - 1], actualURL[i-1]);
            }


            // Logout of portal
            test.Portal.LogoutWebDriver();


        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Daniel Lee")]
        [Description("FO-1783: Re-route Portal Links")]
        public void V2_Giving_Reroute_Portal_Links()
        {
            // Login portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "qaeunlx0v2");

            // Open Giving menu and get urls
            test.Driver.FindElementByLinkText("Giving").Click();
            string search = test.Driver.FindElementByXPath("//div[@id='nav_sub_5']/dl[1]/*//a[text()='Search']").GetAttribute("href").ToString();
            string contributorDetail = test.Driver.FindElementByXPath("//div[@id='nav_sub_5']/dl[1]/*//a[text()='Contributor Details']").GetAttribute("href").ToString();
            string enterContributions = test.Driver.FindElementByXPath("//div[@id='nav_sub_5']/dl[1]/*//a[text()='Enter Contributions']").GetAttribute("href").ToString();
            string onlineStatement = test.Driver.FindElementByXPath("//div[@id='nav_sub_5']/dl[2]/*//a[text()='Online Statement']").GetAttribute("href").ToString();

            //Verify urls are re-routed
            Assert.Contains(search, "/Finance/Payment");
            Assert.Contains(contributorDetail, "People/Individual");
            Assert.Contains(enterContributions, "Giving/Gift/Create");
            Assert.Contains(onlineStatement, "Finance/StatementConfiguration");
            Assert.IsFalse(test.GeneralMethods.IsElementVisibleWebDriver(By.LinkText("Unmatched")));

            // Logout of portal
            test.Portal.LogoutWebDriver();


        }

        [Test, RepeatOnFailure]
        [Author("Daniel Lee")]
        [Description("FO-3253 Disable Creation and Editing of Pledge Drives in Portal for V2 Churches Only")]
        public void V2_Giving_Disable_Creation_And_Editing_Of_Pledge_Drives()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "qaeunlx0v2");

            // Navigate to  pledge drive page
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Setup.Pledge_Drives);

            //Verify Creation and Editing are hidden, indication is displayed.
            Assert.IsFalse(test.GeneralMethods.IsElementVisibleWebDriver(By.Id("ctl00_ctl00_MainContent_content_txtPledgeDriveName_textBox")));
            Assert.IsFalse(test.GeneralMethods.IsElementVisibleWebDriver(By.Id("ctl00_ctl00_MainContent_content_ddlFund_dropDownList")));
            Assert.IsFalse(test.GeneralMethods.IsElementVisibleWebDriver(By.Id("ctl00_ctl00_MainContent_content_ctl01_DateTextBox")));
            Assert.IsFalse(test.GeneralMethods.IsElementVisibleWebDriver(By.Id("ctl00_ctl00_MainContent_content_ctl02_DateTextBox")));
            Assert.IsFalse(test.GeneralMethods.IsElementVisibleWebDriver(By.Id("ctl00_ctl00_MainContent_content_chkWebEnabled")));
            Assert.IsFalse(test.GeneralMethods.IsElementVisibleWebDriver(By.Id("ctl00_ctl00_MainContent_content_txtPledgeDriveGoal_textBox")));
            Assert.IsFalse(test.GeneralMethods.IsElementVisibleWebDriver(By.Id("ctl00_ctl00_MainContent_content_btnSave")));
            Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver("Note: Pledge Drives are managed by Vision2 Systems"));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        #endregion V2
    }
}