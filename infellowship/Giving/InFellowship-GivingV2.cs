using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Data;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;

namespace FTTests.infellowship.GivingV2
{
    [TestFixture]
    public class infellowship_GivingV2 : FixtureBaseWebDriver
    {
        #region Private Members
        private int _individualId;
        private int _householdId;
        private int _individual2Id;
        private int _household2Id;
        #endregion Private Members

        [FixtureSetUp]
        public void FixtureSetUp()
        {
            DataTable results = base.SQL.People_GetIndividualHouseholdIdFromEmail(15, "ft.autotester@gmail.com");
            _individualId = (int)results.Rows[0]["INDIVIDUAL_ID"];
            _householdId = (int)results.Rows[0]["HOUSEHOLD_ID"];

            DataTable results2 = base.SQL.People_GetIndividualHouseholdIdFromEmail(15, "ft.tester2@gmail.com");
            _individual2Id = (int)results2.Rows[0]["INDIVIDUAL_ID"];
            _household2Id = (int)results2.Rows[0]["HOUSEHOLD_ID"];

        }

        #region V2 Giving

        [Test, RepeatOnFailure]
        [Author("Daniel Lee")]
        [Category(TestCategories.Services.SmokeTest)]
        [Description("FO-1780: InFellowship Giving Links to V2")]
        public void Giving_V2_User_LoggedIn_WebDriver()
        {
            bool isPass = true;

            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "qaeunlx0v2");

            // Navigate to Schedule Giving page
            test.Driver.FindElementByXPath(Navigation.InFellowship.Your_Giving).Click();
            test.Driver.FindElementByXPath("//a[@href='/OnlineGiving/ScheduledGiving']").Click();

            try
            {

                //Verify Hide Donor Name button exists
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='col-md-12']/button[2]"));

                //Verify Scheduled Giving text exists
                Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver("Scheduled Giving"));

                //Verify Date of Gift exists
                Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver("Date of Gift"));
            }

            catch (WebDriverException)
            {
                isPass = false;
            }
            //Navigate to history tab
            test.Driver.FindElementByXPath("//a[@href='/OnlineGiving/History']").Click();

            try
            {

                //Verify Download button on the page
                test.GeneralMethods.WaitForElementDisplayed(By.LinkText("Download"));

                //verify Schedule Giving button does not exists
                Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Schedule Giving")));

                //Verify Contribution for filter does not exists
                Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.Id("family_member_filter_trigger")));

                //Verify View button does not exists
                Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.Id("submit")));
            }
            catch (WebDriverException)
            {
                isPass = false;
            }

            Assert.IsTrue(isPass, "Failed - V2 page can't be loaded properly");

            // Logout of Infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Demi Zhang")]
        [Category(TestCategories.Services.SmokeTest)]
        [Description("FO-1780: Verify the V2 Give Now link page appears properly without user logged in / FO-3407: V2 Schedule Giving when user not login/without account")]
        public void Giving_V2_GiveWithoutAccount_WebDriver()
        {
            bool isPass = true;

            // Navigate to infellowship            
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("qaeunlx0v2");

            // Click on the Give Now link
            test.Driver.FindElementByLinkText(GeneralInFellowship.Giving.GIVE_NOW).Click();

            try
            {
                test.GeneralMethods.WaitForElement(By.XPath(".//*[@id='projectBrowseid1View']//*/button[text()='View All Opportunities']"));
            }
            catch (WebDriverException)
            {
                isPass = false;
            }

            // Check if V2 Give Now page is loaded
            Assert.IsTrue(isPass, "the V2 Give Now page is not loaded properly");

            //Check if there are any available gifts to do Give Now
            Assert.IsTrue(test.GeneralMethods.IsElementExist(By.XPath(".//*[@id='projectBrowseid1View']/*//div/a")), "there is no gift available");

            //Click Give Now to add a gift to basket
            test.Driver.FindElementByXPath(".//*[@id='projectBrowseid1View']/*//div/a").Click();

            try
            {
                test.GeneralMethods.WaitForElement(By.XPath("html/body/*//div/a[2][text()='Continue to Checkout']"));
            }

            catch (Exception e)
            {
                TestLog.WriteLine("Gift cannot be selected correctly", e.ToString());
            }

            Assert.IsEmpty(test.Driver.FindElementsByXPath("html/body/*//div/p[@class='add-schedule-text']"));


            // Close page
            test.Driver.Close();

        }


        [Test, RepeatOnFailure]
        [Author("Demi Zhang")]
        [Description("FO-2578: Verify the V2 Check Out page without gifts In Basket appears properly")]
        public void Giving_V2_CheckOut_WithoutGiftsInBasket_WebDriver()
        {
            bool isPass = true;

            // Login to infellowship            
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "qaeunlx0v2");

            // Navigate to your giving page

            test.Driver.FindElementByXPath(Navigation.InFellowship.Your_Giving).Click();

            try
            {
                test.GeneralMethods.WaitForElement(By.XPath(".//*[@id='sidebar']//*/span[text()='Checkout']"));
                test.GeneralMethods.WaitForElement(By.XPath(".//*[@id='sidebar']//*/span[text()='Give Now']"));
            }
            catch (WebDriverException)
            {
                isPass = false;
            }

            // Check if Giving page is properly loaded
            Assert.IsTrue(isPass, "Giving page with Give Now and Check Out is not loaded properly");


            // Delete the existing gifts in basket
            test.Infellowship.Giving_V2_CheckOut_DeleteExistingGifts();


            //Go to Giving page and navigate to Check Out page
            test.Driver.FindElementByXPath(".//*[@id='tab_topper_giving']/a[text()='GIVING']").Click();

            test.Driver.FindElementByXPath(".//*[@id='sidebar']//*/span[text()='Checkout']").Click();

            // Check if V2 Check Out page is properly loaded with correct info
            Assert.AreEqual(test.Driver.FindElement(By.XPath(".//*[@id='items']//*/div[@class='form-group']/h4")).Text, "There is nothing in your basket!");


            // Close page
            test.Driver.Close();

        }

        [Test, RepeatOnFailure]
        [Author("Demi Zhang")]
        [Category(TestCategories.Services.SmokeTest)]
        [Description("FO-2578: Verify the V2 Check Out page with gifts In Basket appears properly")]
        public void Giving_V2_CheckOut_WithGiftsInBasket_WebDriver()
        {


            // Login to infellowship            
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "qaeunlx0v2");

            // Navigate to your giving page

            test.Driver.FindElementByXPath(Navigation.InFellowship.Your_Giving).Click();

            bool isPass = true;
            isPass = test.GeneralMethods.IsElementExist(By.XPath(".//*[@id='sidebar']//*/span[text()='Checkout']")) && isPass;
            isPass = test.GeneralMethods.IsElementExist(By.XPath(".//*[@id='sidebar']//*/span[text()='Give Now']")) && isPass;

            // Check if Giving page is properly loaded
            Assert.IsTrue(isPass, "Giving page with Give Now and Check Out is not loaded properly");

            // Delete the existing gifts in basket if there are
            test.Infellowship.Giving_V2_CheckOut_DeleteExistingGifts();

            //Go to Giving page and do Give Now to add gift to basket
            test.Driver.FindElementByXPath(".//*[@id='tab_topper_giving']/a[text()='GIVING']").Click();
            test.Infellowship.Giving_V2_GiveNow_AddGiftToBasket();

            //Nagivate to Giving and Check Out page
            test.Driver.FindElementByXPath(".//*[@id='tab_topper_giving']/a[text()='GIVING']").Click();
            test.Driver.FindElementByXPath(".//*[@id='sidebar']//*/span[text()='Checkout']").Click();

            // Check if the gift just added exist in basket
            Assert.AreEqual(test.Driver.FindElement(By.XPath(".//*[@id='items']//*/div[@class='form-group']/h4")).Text, "1 Item in your Basket");

            // Close page
            test.Driver.Close();

        }
        [Test, RepeatOnFailure]
        [Author("Daniel Lee")]
        [Category(TestCategories.Services.SmokeTest)]
        [Description("FO-2583: Page Stubs")]
        public void Giving_V2_Verify_Page_Stubs()
        {
            bool isPass = true;

            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "qaeunlx0v2");

            // Navigate to Giving History page
            test.Driver.FindElementByXPath(Navigation.InFellowship.Your_Giving).Click();


            // Click on the Give Now button
            test.Driver.FindElementByLinkText("Give Now").Click();
            try
            {
                //Verify V2 widget loads properly
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='project-search']/div[2]/div[1]/button[1]"));
                Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("projectBrowseid1ViewSearch")));
                Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("projectBrowseid1View")));


                //Click Read More for a fund detail and verify
                test.Driver.FindElementByXPath("//div[@id='projectBrowseid1View']/div[14]/div[2]/p[1]/a[1]").Click();
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='project-action-btn-wrapper']/a[1]"));

                //Click Give Now button
                test.Driver.FindElementByXPath("//div[@class='project-action-btn-wrapper']/a[1]").Click();

                //Click Continue to checkout
                test.Driver.FindElementByXPath("//div[@class = 'well well-sm']/div[1]/a[2]").Click();
                //modify by grace zhang
                test.GeneralMethods.VerifyTextPresentWebDriver("Schedule this Gift");
            }

            catch (WebDriverException)
            {
                isPass = false;
            }

            Assert.IsTrue(isPass, "Failed - V2 widget or checkout page can't be loaded properly");

            // Logout of Infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Demi Zhang")]
        [Category(TestCategories.Services.SmokeTest)]
        [Description("FO-3366: For V2 Churches: Redirect from legacy online giving links to new V2 giving links -> GiveNowLink")]
        public void Giving_V2_Redirect_F1GiveNowLink_ToV2GivingLinks()
        {
             
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "qaeunlx0v2");

            //try to access F1 Give Now page for V2 Church;
            test.Driver.Url = test.Driver.Url + "/onlinegiving/givenow/step1";

            // check if F1 Give Now link for V2 Church can be re-directed to V2 Give Now page - the url will be changed to V2 giving link and a prompt indicating user has been redirected to new giving page.
            bool isPass1 = test.GeneralMethods.IsElementExist(By.XPath(".//div[@id='success_message_inner']/span[@class='big' and normalize-space(text())='You have been re-directed to the new giving page.']"));
            Assert.AreEqual(true, isPass1);
            Assert.AreEqual(Regex.Match(test.Driver.Url, "https://qaeunlx0v2.[A-Z a-z 0-9]+.infellowship.com/").Value + "OnlineGiving/GiveNow/V2Giving", test.Driver.Url);

            
            //try to access F1 Scheduled Giving page for V2 Church;
            string url = Regex.Match(test.Driver.Url, "https://qaeunlx0v2.[A-Z a-z 0-9]+.infellowship.com/").Value + "onlinegiving/scheduledgiving/step1";
            test.Driver.Url = url;

            //check if F1 scheduled giving link for V2 Church can be redirected to V2 give now page - the url will be changed to V2 scheduled giving link and a prompt indicating user has been redirected to new giving page..
            bool isPass2 = test.GeneralMethods.IsElementExist(By.XPath(".//div[@id='success_message_inner']/span[@class='big' and normalize-space(text())='You have been re-directed to the new giving page.']"));
            Assert.AreEqual(true, isPass2);
            Assert.AreEqual(Regex.Match(test.Driver.Url, "https://qaeunlx0v2.[A-Z a-z 0-9]+.infellowship.com/").Value + "OnlineGiving/ScheduledGiving/ScheduledGivingV2", test.Driver.Url);

            // Logout of Infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Demi Zhang")]
        [Category(TestCategories.Services.SmokeTest)]
        [Description("FO-3366: For V2 Churches: Redirect from legacy online giving links to new V2 giving links -> GiveNowWithoutAccountLink")]
        public void Giving_V2_Redirect_F1GiveNowWithoutAccountLink_ToV2GivingLinks()
        {

            // Navigate to infellowship            
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("qaeunlx0v2");
                      

            //try to access F1 Give Now Without Account page for the V2 Church;

            string url = Regex.Match(test.Driver.Url, "https://qaeunlx0v2.[A-Z a-z 0-9]+.infellowship.com/").Value + "onlinegiving/givenow/noaccount";
            test.Driver.Url = url;
            
            //check if F1 GiveNow without account link for V2 Church can be redirected to V2 givenow without account page;
            bool isPass = test.GeneralMethods.IsElementExist(By.XPath(".//div[@id='success_message_inner']/span[@class='big' and normalize-space(text())='You have been re-directed to the new giving page.']"));
            Assert.AreEqual(true, isPass);
            Assert.AreEqual(Regex.Match(test.Driver.Url, "https://qaeunlx0v2.[A-Z a-z 0-9]+.infellowship.com/").Value + "OnlineGiving/GiveNow/NoAccountV2Giving", test.Driver.Url);

            // close page
            test.Driver.Close();

        }

        #endregion V2 Giving
    }
}
