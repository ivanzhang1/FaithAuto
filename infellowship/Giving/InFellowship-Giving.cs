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


namespace FTTests.infellowship.Giving {
    [TestFixture]
    public class infellowship_Giving_WebDriver : FixtureBaseWebDriver {
        #region Private Members
        private int _individualId;
        private int _householdId;
        private int _individual2Id;
        private int _household2Id;
        #endregion Private Members

        [FixtureSetUp]
        public void FixtureSetUp() {
            DataTable results = base.SQL.People_GetIndividualHouseholdIdFromEmail(15, "ft.autotester@gmail.com");
            _individualId = (int)results.Rows[0]["INDIVIDUAL_ID"];
            _householdId = (int)results.Rows[0]["HOUSEHOLD_ID"];

            DataTable results2 = base.SQL.People_GetIndividualHouseholdIdFromEmail(15, "ft.tester2@gmail.com");
            _individual2Id = (int)results2.Rows[0]["INDIVIDUAL_ID"];
            _household2Id = (int)results2.Rows[0]["HOUSEHOLD_ID"];

        }

        #region Giving History
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Views the Giving History page in the Online Giving section of InFellowship.")]
        public void Giving_GivingHistory_View_WebDriver() {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Navigate to the giving history page
            test.Driver.FindElementByXPath(Navigation.InFellowship.Your_Giving).Click();

            // Verify the current year is defaulted in the drop down list
            Assert.AreEqual(DateTime.Now.Year.ToString(), test.Driver.FindElementByXPath("//select[@id='year_select']/option[1]").Text);

            // Verify the user can view this year or last year
            Assert.AreEqual(2, test.Driver.FindElementsByXPath("//select[@id='year_select']/option").Count);
            Assert.AreEqual(DateTime.Now.Year.ToString(), test.Driver.FindElementByXPath("//select[@id='year_select']/option[1]").Text);
            Assert.AreEqual(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(-1).Year.ToString(), test.Driver.FindElementByXPath("//select[@id='year_select']/option[2]").Text);

            // Verify the filtering options
            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//div[@id='family_member_filter']/ul/li[1]/input[@id='people_filter_1']"));

            DataTable individualNames = SQL.People_FetchIndividualsInHouseholdByHouseholdID(15, _householdId);
            for (int i = 0; i < individualNames.Rows.Count; i++) {
                test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath(string.Format("//div[@id='family_member_filter']/ul/li[*]/label[@for='people_filter' and text()='{0}']", individualNames.Rows[i]["INDIVIDUAL_NAME"])));
            }

            // Verify the total amount of contributions displayed for the current year
            //double amount = SQL.Giving_GetContributionReceiptTotalForIndividualOrHousehold(15, _householdId, int.MinValue, DateTime.Now.Year.ToString());
            //Assert.AreEqual(string.Format("{0:c}", amount), test.Selenium.GetText("//td[@class='align_right bold shrink']"));
            string amount = SQL.Giving_GetContributionReceiptTotalForIndividualOrHousehold_Proc(15, _individualId, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), string.Format("0, {0}", _individualId));
            Assert.AreEqual(string.Format("{0:c}", Convert.ToDouble(amount)), test.Driver.FindElementByXPath("//td[@class='align_right bold shrink']").Text);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the filtering of online giving at the household level.")]
        public void Giving_GivingHistory_Filter_Household_WebDriver() {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Navigate to the giving history page
            test.Driver.FindElementByXPath(Navigation.InFellowship.Your_Giving).Click();

            // Deselect all filters except for household
            test.Driver.FindElementById("family_member_filter_trigger").Click();
            for (int i = 2; i < test.Driver.FindElementsByXPath("//div[@id='family_member_filter']/ul/li").Count + 1; i++) {
                test.Driver.FindElementByXPath(string.Format("//div[@id='family_member_filter']/ul/li[{0}]/input", i)).Click();
            }

            // View the online giving
            test.Driver.FindElementByXPath("//button[@type='submit' and @id='submit']").Click();

            // Verify the total amount of contributions displayed for the household level only
            //double amount = SQL.Giving_GetContributionReceiptTotalForIndividualOrHousehold(15, _householdId, null, DateTime.Now.Year.ToString());
            //Assert.AreEqual(string.Format("{0:c}", amount), test.Selenium.GetText("//td[@class='align_right bold shrink']"));            
            string amount = SQL.Giving_GetContributionReceiptTotalForIndividualOrHousehold_Proc(15, _individualId, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")));
            if (amount == "0.00")
            {
                TestLog.WriteLine("No Results Found is expected");
                test.GeneralMethods.VerifyTextPresentWebDriver("No results found");
            }
            else
            {
                Assert.AreEqual(string.Format("{0:c}", Convert.ToDouble(amount)), test.Driver.FindElementByXPath("//td[@class='align_right bold shrink']").Text);
            }

            // Verify all rows list household as the name
            for (int i = 2; i < test.Driver.FindElementsByXPath("//table[@class='grid']/tbody/tr").Count; i++) {

                if (test.Driver.FindElementByXPath(string.Format("//table[@class='grid']/tbody/tr[{0}]/td[4]", i)).Text == "Pending") {
                    Assert.AreEqual("FT Tester", test.Driver.FindElementByXPath(string.Format("//table[@class='grid']/tbody/tr[{0}]/td[2]", i)).Text);
                }
                else {
                    Assert.AreEqual("Household", test.Driver.FindElementByXPath(string.Format("//table[@class='grid']/tbody/tr[{0}]/td[2]", i)).Text);
                }
            }

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the filtering of online giving at the individual level.")]
        public void Giving_GivingHistory_Filter_Individual_WebDriver() {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.tester2@gmail.com", "FT4life!", "dc");

            // Navigate to the giving history page
            test.Driver.FindElementByXPath(Navigation.InFellowship.Your_Giving).Click();

            // Deselect all filters
            test.Driver.FindElementById("family_member_filter_trigger").Click();
            for (int i = 1; i < test.Driver.FindElementsByXPath("//div[@id='family_member_filter']/ul/li").Count + 1; i++) {
                test.Driver.FindElementByXPath(string.Format("//div[@id='family_member_filter']/ul/li[{0}]/input", i)).Click();
            }

            // Select an individual
            // test.Driver.FindElementById("family_member_filter_trigger").Click();
            test.Driver.FindElementByXPath(string.Format("//div[@id='family_member_filter']/ul/li[*]/input[@type='checkbox' and following-sibling::label/text()='FT Tester2']")).Click();

            // View the online giving
            test.Driver.FindElementByXPath("//button[@type='submit' and @id='submit']").Click();

            // Verify the total amount of contributions displayed for the individual only
            //double amount = SQL.Giving_GetContributionReceiptTotalForIndividualOrHousehold(15, _householdId, _individualId, DateTime.Now.Year.ToString());
            //Assert.AreEqual(string.Format("{0:c}", amount), test.Selenium.GetText("//td[@class='align_right bold shrink']"));
            string amount = SQL.Giving_GetContributionReceiptTotalForIndividualOrHousehold_Proc(15, _individual2Id, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), string.Format("{0}", _individual2Id));

            if (amount == "0.00")
            {
                TestLog.WriteLine("No Results Found is expected");
                test.GeneralMethods.VerifyTextPresentWebDriver("No results found");
            }
            else
            {
                Assert.AreEqual(string.Format("{0:c}", Convert.ToDouble(amount)), test.Driver.FindElementByXPath("//td[@class='align_right bold shrink']").Text);
            }

            // Verify all rows list the individual as the name
            for (int i = 2; i < test.Driver.FindElementsByXPath("//table[@class='grid']/tbody/tr").Count; i++) {
                Assert.AreEqual("FT Tester2", test.Driver.FindElementByXPath(string.Format("//table[@class='grid']/tbody/tr[{0}]/td[2]", i)).Text);
            }

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies functionality when the user deselects all filtering options on the Giving History page.")]
        public void Giving_GivingHistory_Filter_DeselectAll_WebDriver() {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

            // Navigate to the giving history page
            test.Driver.FindElementByXPath(Navigation.InFellowship.Your_Giving).Click();

            // Deselect all filters
            test.Driver.FindElementById("family_member_filter_trigger").Click();
            for (int i = 1; i < test.Driver.FindElementsByXPath("//div[@id='family_member_filter']/ul/li").Count + 1; i++) {
                test.Driver.FindElementByXPath(string.Format("//div[@id='family_member_filter']/ul/li[{0}]/input", i)).Click();
            }

            // View the online giving
            test.Driver.FindElementByXPath("//input[@type='submit' and @value='View']").Click();

            test.GeneralMethods.WaitForElement(test.Driver, By.Id("family_member_filter_trigger"), 30, "Wait for No results found");

            // Verify user is displayed 'No results found'
            Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver("No results found"));

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the filtering of online giving should be hidden when login as visitor or other position.")]
        public void Giving_GivingHistory_Filter_HiddenForVisitorOrOther_WebDriver() {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("rosemarry2018@yahoo.com", "chrdwhdhXT1!", "dc");

            // Navigate to the giving history page
            test.Driver.FindElementByXPath(Navigation.InFellowship.Your_Giving).Click();

            // Verify the 'Contributions for' label and drop down list are hidden
            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver("Contributions for"));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//div[@id='family_member_filter']")));

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Give now link is not present when the feature is turned off in responsive view.")]
        public void Giving_GiveNowEnabled_Responsive()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "qaeunlx0c3");

            // Set window size to simulate iPhone
            test.Infellowship.SetBrowserSizeTo_Mobile();

            // Navigate to the giving history page
            test.Driver.FindElementByXPath(Navigation.InFellowship.Your_Giving).Click();

            //Verify that Give Now button is present
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Give Now")));

            //Log out 
            test.Infellowship.LogoutWebDriver();

        }
        #endregion Giving History

        #region Giving Schedules
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("F1-4392: Verifies the Schedule Giving tab appears properly in full screen view")]
        public void Giving_GivingSchedules_View_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Navigate to the giving history page
            test.Driver.FindElementByXPath(Navigation.InFellowship.Your_Giving).Click();

            // Click on the Schedules tab
            test.Driver.FindElementByLinkText("Schedules").Click();

            // Verify user is on the Schedules page
            if (test.GeneralMethods.IsElementPresentWebDriver(By.XPath(TableIds.InFellowship_GivingSchedules)))
            {
                Assert.AreEqual("Name", test.Driver.FindElementByXPath(string.Format("{0}/thead/tr/th[1]", TableIds.InFellowship_GivingSchedules)).Text);
                Assert.AreEqual("Next Occurrence", test.Driver.FindElementByXPath(string.Format("{0}/thead/tr/th[2]", TableIds.InFellowship_GivingSchedules)).Text);
                Assert.AreEqual("Giving Method", test.Driver.FindElementByXPath(string.Format("{0}/thead/tr/th[3]", TableIds.InFellowship_GivingSchedules)).Text);
                Assert.AreEqual("Amount", test.Driver.FindElementByXPath(string.Format("{0}/thead/tr/th[4]", TableIds.InFellowship_GivingSchedules)).Text);
            }
            else
            {
                Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver("No giving schedules found"));
            }

            // Logout of Infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-4392: Verifies the Schedule Giving tab appears properly in RESPONSIVE view")]
        public void Giving_GivingSchedules_View_RESPONSIVE_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Set window size to simulate iPhone
            test.Infellowship.SetBrowserSizeTo_Mobile();

            // Navigate to the giving history page
            test.Driver.FindElementByXPath(Navigation.InFellowship.Your_Giving).Click();

            // Click on the dropdown and select Schedules
            test.Driver.FindElementByXPath("//div[@class='tabbed-nav']/ul/li/a/span[@class='caret']").Click();
            test.Driver.FindElementByLinkText("Schedules").Click();

            // Verify user is on the Schedules page
            if (test.GeneralMethods.IsElementPresentWebDriver(By.XPath(TableIds.InFellowship_GivingSchedules)))
            {
                // Verify headers are not displaying
                test.GeneralMethods.VerifyElementNotDisplayedWebDriver(By.XPath(string.Format("{0}/thead/tr/th[1]", TableIds.InFellowship_GivingSchedules)));
                test.GeneralMethods.VerifyElementNotDisplayedWebDriver(By.XPath(string.Format("{0}/thead/tr/th[2]", TableIds.InFellowship_GivingSchedules)));
                test.GeneralMethods.VerifyElementNotDisplayedWebDriver(By.XPath(string.Format("{0}/thead/tr/th[3]", TableIds.InFellowship_GivingSchedules)));
                test.GeneralMethods.VerifyElementNotDisplayedWebDriver(By.XPath(string.Format("{0}/thead/tr/th[4]", TableIds.InFellowship_GivingSchedules)));

                // Verify a giving schedule appears
                test.GeneralMethods.VerifyElementDisplayedWebDriver(By.XPath(string.Format("{0}/tbody/tr/td/span", TableIds.InFellowship_GivingSchedules)));
                Assert.Contains(test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr/td/span", TableIds.InFellowship_GivingSchedules)).Text, "$");
            }
            else
            {
                Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver("No giving schedules found"));
            }

            // Logout of Infellowship
            test.Infellowship.LogoutWebDriver();

        }
        #endregion Giving Schedules

        #region GiveWithoutAccount

        //[Test, RepeatOnFailure, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Stuart Platt")]
        [Description("Verifies Give Without Account page")]
        public void Giving_GiveWithoutAccount_View_PersonalCheck()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            test.Driver.FindElementByLinkText(GeneralInFellowship.Giving.GIVE_NOW).Click();
            test.GeneralMethods.WaitForElement(By.Id(GeneralInFellowship.Giving.Contributor_FirstName));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(string.Format("{0}0", GeneralInFellowship.Giving.Fund))));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.SubFund)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.Amount)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.Add_Another)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.Contributor_FirstName)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.Contributor_LastName)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.Contributor_Email)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.PersonalCheck_Bullet)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.CreditCard_Bullet)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath(GeneralInFellowship.Giving.PhoneNumber)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath(GeneralInFellowship.Giving.RoutingNumber)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath(GeneralInFellowship.Giving.AccountNumber)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("sample-check")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("VerificationCaptcha_CaptchaImage")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("CaptchaCode")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("VerificationCaptcha_ReloadIcon")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("VerificationCaptcha_SoundIcon")));

            test.Driver.Close();
        }

        //[Test, RepeatOnFailure, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Stuart Platt")]
        [Description("Verifies Give Without Account page")]
        public void Giving_GiveWithoutAccount_View_CreditCard()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            test.Driver.FindElementByLinkText(GeneralInFellowship.Giving.GIVE_NOW).Click();
            test.GeneralMethods.WaitForElement(By.Id(GeneralInFellowship.Giving.Contributor_FirstName));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(string.Format("{0}0", GeneralInFellowship.Giving.Fund))));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.SubFund)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.Amount)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.Add_Another)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.Contributor_FirstName)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.Contributor_LastName)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.Contributor_Email)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.PersonalCheck_Bullet)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.CreditCard_Bullet)));
            test.Driver.FindElementById(GeneralInFellowship.Giving.CreditCard_Bullet).Click();
            test.GeneralMethods.WaitForElement(By.Id(GeneralInFellowship.Giving.Cardholder_FirstName));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.Cardholder_FirstName)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.Cardholder_LastName)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.CreditCard_Type)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.CreditCard_Number)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.CreditCard_ExpireMonth)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.CreditCard_ExpireYear)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.CreditCard_SecurityCode)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.Country)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.AddressLine1)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.AddressLine2)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.City)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.State)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.ZipCode)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.County)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("VerificationCaptcha_CaptchaImage")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("CaptchaCode")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("VerificationCaptcha_ReloadIcon")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("VerificationCaptcha_SoundIcon")));

            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies the content appears properly in give now screen without log in")]
        public void Giving_GiveWithoutAccount_UK_View_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("QAEUNLX0C6");

            // Click on the Give Now link
            test.Driver.FindElementByLinkText(GeneralInFellowship.Giving.GIVE_NOW).Click();
            test.GeneralMethods.WaitForElement(By.Id(string.Format("{0}0", GeneralInFellowship.Giving.Fund)));

            // Click to add another row
            test.Driver.FindElementById("add_row").Click();

            // Verify user is on the Schedules page
            Assert.AreEqual("£", test.Driver.FindElementByXPath(string.Format("{0}/li[2]/div/div[3]/div/span", TableIds.InFellowship_GiveNowTable)).Text);
            Assert.AreEqual("£", test.Driver.FindElementByXPath(string.Format("{0}/li[3]/div/div[3]/div/span", TableIds.InFellowship_GiveNowTable)).Text);
            Assert.AreEqual("£0.00\r\nTotal", test.Driver.FindElementByXPath(string.Format("{0}/li[4]/div/div[2]", TableIds.InFellowship_GiveNowTable)).Text);

            // Close page
            test.Driver.Close();

        }

        #endregion GiveWithoutAccount
    }
}