using System;
using System.Linq;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Web;

namespace FTTests.Portal.Giving
{
    [TestFixture]
    public class Portal_Giving_Contributions_SearchWebDriver : FixtureBaseWebDriver
    {

        [Test, RepeatOnFailure]
        [Description("Modifies a contribution, clearing the individual.")]
        [Category(TestCategories.Services.SmokeTest)]
        public void Giving_Contributions_Search_EditContribution_Check()
        {

            // Set initial conditions
            string random = Guid.NewGuid().ToString().Substring(0, 5);
            string batchName = string.Format("Test Batch Contribution - {0}", random);
            // base.SQL.Giving_ContributionReceipts_DeleteByBatchName(15, batchName, 1);
            // base.SQL.Giving_Batches_Delete(15, batchName, 1);
            base.SQL.Giving_Batches_Create(15, batchName, 100, 1);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Add a contribution to a batch
            string referenceOrig = new Random().Next(1000).ToString();
            test.Portal.Giving_EnterContributions(batchName, "Matthew Sneeden", "Household", "Check", "1 - General Fund", new string[] { "100", referenceOrig }, false, false, null);

            // View the batch
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Search);
            new SelectElement(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlHousehold_dropDownList")).SelectByText("Household");
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtBatchName_textBox").SendKeys(batchName);
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSearch").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Matthew Sneeden and Laura Hause"));
            //test.GeneralMethods.Navigate_Portal(Navigation.Giving.Contributions.Batches);
            //test.Driver.FindElementByLinkText(batchName).Click();

            // Edit the contribution
            int itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Portal.Giving_Search, referenceOrig, "Ref.");
            test.GeneralMethods.SelectOptionFromGearWebDriver(itemRow, "Edit contribution");

            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_lnkClearHousehold").Click();
            string referenceMod = new Random().Next(1000).ToString();
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlCheck_txtReference_textBox").Clear();
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlCheck_txtReference_textBox").SendKeys(referenceMod);
            test.Driver.FindElementById(GeneralButtons.Save).Click();

            // Verify existing contribution has been removed from the Household
            test.GeneralMethods.VerifyTextPresentWebDriver("No records found");
            //Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Portal.Giving_Search, referenceOrig, "Ref."));

            // Search for all contributions within the batch
            new SelectElement(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlHousehold_dropDownList")).SelectByText("");
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSearch").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("All"));

            // Confirm that the contribution is now Unmatched
            int itemRowVerify = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Portal.Giving_Search, referenceMod, "Ref.");

            IWebElement table = test.Driver.FindElementById(TableIds.Portal.Giving_Search);
            Assert.AreEqual("Unmatched", table.FindElements(By.TagName("tr"))[itemRowVerify].FindElements(By.TagName("td"))[2].Text);
            Assert.AreEqual(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("M/d/yyyy"), table.FindElements(By.TagName("tr"))[itemRowVerify].FindElements(By.TagName("td"))[4].Text);
            Assert.AreEqual(referenceMod, table.FindElements(By.TagName("tr"))[itemRowVerify].FindElements(By.TagName("td"))[7].Text);
            Assert.AreEqual(string.Format("{0:c}", 100), table.FindElements(By.TagName("tr"))[itemRowVerify].FindElements(By.TagName("td"))[9].Text);
            Assert.AreEqual(4, table.FindElements(By.TagName("tr")).Count);

            // Logout of portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Giving_Batches_Delete(15, batchName, 1);

        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Deletes a split from a contribution.")]
        public void Giving_Contributions_Search_EditContribution_DeleteSplit()
        {
            // Set initial conditions
            string batchName = "Test Batch - Split Contribution";
            base.SQL.Giving_ContributionReceipts_DeleteByBatchName(15, batchName, 1);
            base.SQL.Giving_Batches_Delete(15, batchName, 1);
            base.SQL.Giving_Batches_Create(15, batchName, 100, 1);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Add a split contribution to a batch
            test.Portal.Giving_EnterContributions(batchName, "Matthew Sneeden", null, "Cash", null, null, false, true, new string[][] { new string[5] { "Matthew Sneeden", "50.00", "1 - General Fund", null, null }, new string[5] { "Matthew Sneeden", "50.00", "2 - Building Fund", null, null } });

            // Delete a split from the batch
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Search);
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtBatchName_textBox").SendKeys(batchName);
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSearch").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Matthew Sneeden"));
            test.GeneralMethods.SelectOptionFromGearWebDriver(3, "Edit contribution");
            test.Driver.FindElementByXPath("//table[@id='ctl00_ctl00_MainContent_content_dgSplitContributions']/tbody/tr[2]/td[7]/span/a").Click();
            test.Driver.FindElementById(GeneralButtons.Save).Click();

            // Verify records are correct
            IWebElement table = test.Driver.FindElementById(TableIds.Portal.Giving_Search);
            Assert.AreEqual("Matthew Sneeden", table.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[2].Text);
            Assert.AreEqual(batchName, table.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[3].Text);
            Assert.AreEqual(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString(), table.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[4].Text); //M/dd/yyyy
            Assert.AreEqual("2 - Building Fund", table.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[5].Text);
            Assert.IsFalse(test.Driver.FindElementsByXPath((string.Format("{0}/tbody/tr[3]/td[9]/img[contains(@src, '/portal/images/check.gif? ')]", TableIds.Giving_Search))).Count > 0);
            Assert.AreEqual("$50.00", table.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[9].Text);

            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Portal.Giving_Search, "1 - General Fund", "Fund"));
            Assert.AreEqual(4, test.Driver.FindElementsByXPath(string.Format("{0}/tbody/tr", TableIds.Giving_Search)).Count);
            Assert.IsTrue(test.Driver.FindElementsByXPath(string.Format("{0}/tbody/tr[4]/td[9]/span[@class='bold' and text()='Total' and ancestor::tr/td[10]/span/b/text()='$50.00']", TableIds.Giving_Search)).Count > 0);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user can split a contribution receipt that was created via credit card batches.")]
        public void Giving_Contributions_Search_EditContribution_SplitContributionFromSettledCreditCardBatch_WebDriver()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            // Set initial conditions
            string batchName = "Settled: Split";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, 100, 2);

            // Login to portal
            test.Portal.LoginWebDriver();

            //Add Credit card contribution to Batch
            if (base.SQL.IsAMSEnabled(15))
            {
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "12", string.Format("{0:yyyy}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))), "100", "1 - General Fund", null, null);
            }
            else
            {

                base.SQL.Giving_Batches_AddContributionToCreditCardBatch(15, batchName, "1 - General Fund", 100, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1));
            }

            //Authorize the batch
            base.SQL.Giving_Batches_Authorize(15, batchName, base.SQL.IndividualID);

            // Settle the batch
            test.Portal.Giving_Batches_CreditCardBatches_Settle_WebDriver(batchName);

            // View a settled batch
            test.Driver.FindElementByLinkText(GeneralLinks.RETURN_WebDriver).Click();
            //test.GeneralMethods.SelectOptionFromGear(Convert.ToInt16(test.GeneralMethods.GetTableRowNumber(TableIds.Giving_Batches, batchName, "Name")), "View batch");
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Search);
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtBatchName_textBox").SendKeys(batchName);
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSearch").Click();

            // Edit the contribution
            test.GeneralMethods.SelectOptionFromGearWebDriver(Convert.ToInt16(test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Search, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString(), "Received")), "Edit contribution");

            // Enter the information for the split
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_chkSplit").Click();
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtSplitAmount_textBox").SendKeys("50");
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSaveSplit").Click();

            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtSplitAmount_textBox").SendKeys("50");
            new SelectElement(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlSplitFund_dropDownList")).SelectByText("A Test Fund");
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSaveSplit").Click();
            test.GeneralMethods.WaitForElement(By.XPath("//span[@id='ctl00_ctl00_MainContent_content_lblSplitAmountTotal' and text()='100.00']"), 10000);

            // Save the contribution
            test.Driver.FindElementById(GeneralButtons.Save).Click();

            // Verify the split contributions, original contribution was removed
            Assert.AreEqual("Matthew Sneeden and Laura Hause", test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgContributions_ctl03_lnkHouseholdName").Text);
            Assert.AreEqual(batchName, test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgContributions_ctl03_lblBatchValue").Text);
            Assert.AreEqual(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString(), test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgContributions_ctl03_lblReceivedDate").Text);
            Assert.AreEqual("1 - General Fund", test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgContributions_ctl03_lblFundName").Text);
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//*[@id='ctl00_ctl00_MainContent_content_dgContributions_ctl03_imgIsSplit']")));
            // Assert.IsTrue(test.Selenium.IsElementPresent(string.Format("{0}/tbody/tr[3]/td[9]/img[contains(@src, '/assets/images/check.gif?')]", TableIds.Giving_Search)));
            // Assert.IsTrue(test.Selenium.IsElementPresent(string.Format("{0}/tbody/tr[3]/td[9]/img[contains(@src, '/portal/images/check.gif?')]", TableIds.Giving_Search)));
            Assert.AreEqual("$50.00", test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgContributions_ctl03_lblAmtValue").Text);

            Assert.AreEqual("Matthew Sneeden and Laura Hause", test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgContributions_ctl04_lnkHouseholdName").Text);
            Assert.AreEqual(batchName, test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgContributions_ctl04_lblBatchValue").Text);
            Assert.AreEqual(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString(), test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgContributions_ctl04_lblReceivedDate").Text);
            Assert.AreEqual("A Test Fund", test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgContributions_ctl04_lblFundName").Text);
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//*[@id='ctl00_ctl00_MainContent_content_dgContributions_ctl03_imgIsSplit']")));
            // Assert.IsTrue(test.Selenium.IsElementPresent(string.Format("{0}/tbody/tr[3]/td[9]/img[contains(@src, '/assets/images/check.gif?')]", TableIds.Giving_Search)));
            // Assert.IsTrue(test.Selenium.IsElementPresent(string.Format("{0}/tbody/tr[4]/td[9]/img[contains(@src, '/portal/images/check.gif?')]", TableIds.Giving_Search)));
            Assert.AreEqual("$50.00", test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgContributions_ctl04_lblAmtValue").Text);

            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Giving_Search, "Matthew Sneeden", "Attributed To"));
            //Assert.AreEqual(5, test.GeneralMethods.GetTableRowCountWebDriver(string.Format("{0}/tbody/tr", TableIds.Giving_Search)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath(string.Format("{0}/tbody/tr[5]/td[9]/span[@class='bold' and text()='Total' and ancestor::tr/td[10]/span/b/text()='$100.00']", TableIds.Giving_Search))));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
    }

    //FGJ Commenting DoesNotRunOutisdeOfStaging since we want these TC to execute against legacy (QA) and AMS (Staging) PP
    //[TestFixture, DoesNotRunOutsideOfStaging]
    [TestFixture]
    [Category(TestCategories.Services.PaymentProcessor)]
    public class Portal_Giving_Contributions_ContributorDetails_Contributions_WebDriver : FixtureBaseWebDriver
    {
        #region Private Members
        private string _individualNameFTAutoTester = "FT Tester";
        #endregion Private Members

        #region Fixture Teardown
        [FixtureTearDown]
        public void FixtureTearDown()
        {
            base.SQL.Giving_ScheduleGiving_DeleteAll(15, "ft.autotester@gmail.com");
            base.SQL.Giving_ScheduleGiving_DeleteAll(258, "ft.autotester@gmail.com");
            base.SQL.Giving_ScheduleGiving_DeleteAll(99005, "ft.autotester@gmail.com");

            try
            {
                base.SQL.Giving_PledgeDrives_Pledge_Delete(15, "FT Tester");
                base.SQL.Giving_PledgeDrives_Pledge_Delete(15, "Kevin Spacey");
            }
            catch (Exception e)
            {

                TestLog.WriteLine(e.StackTrace);
            }
        }
        #endregion Fixture Teardown

        #region Accounts
        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies a user can view the Accounts tab for an individual on the Contributor Details page.")]
        public void Giving_Contributions_ContributorDetails_Accounts_View()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to the Accounts tab on the Contributor Details page
            test.Portal.Giving_ContributorDetails_Accounts_View_WebDriver("FT Tester");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies a user can add an account for an individual through the Contributor Details page.")]
        public void Giving_Contributions_ContributorDetails_Accounts_AddAccount()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to Accounts tab and add account
            test.Portal.Giving_ContributorDetails_Accounts_AddAccountAndDelete_WebDriver("FT Tester3", "Household", "ACH", "111222334", "111000025", false);
            
            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("FO-4993 Add an account start with 0")]
        public void Giving_Contributions_ContributorDetails_Accounts_AddAccountZero()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to Accounts tab and add account
            test.Portal.Giving_ContributorDetails_Accounts_AddAccount_WebDriver("FT Tester3", "Household", "ACH", "1234565432", "044000037", false);
            test.Portal.Giving_ContributorDetails_Accounts_DeleteAccount_WebDriver("ACH", "1234565432", "044000037");
            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("FO-4993 Add an account and update to value start with 0")]
        public void Giving_Contributions_ContributorDetails_Accounts_AddAccountUpdateZero()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to Accounts tab and add account
            test.Portal.Giving_ContributorDetails_Accounts_AddAccount_WebDriver("FT Tester3", "Household", "ACH", "111222338", "111000025", false);
            try
            {
                test.Portal.Giving_ContributorDetails_Accounts_EditAccount_WebDriver("Cash", "044000037", "ACH", "111222338", "111000025");
            }
            finally
            {
                test.Portal.Giving_ContributorDetails_Accounts_DeleteAccount_WebDriver("Cash", "111222338", "044000037");

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies a user can delete a credit card account and that the credit card number is not fully displayed in the confirmation message.")]
        public void Giving_Contributions_ContributorDetails_Accounts_DeleteCCAccount_CardNumberNOTDisplayed() {

        }

        #endregion Accounts

        #region Pledges

        [Test, RepeatOnFailure(Order = 1)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Felix Gaytan")]
        [Description("Deletes all contribution Pledge Views")]
        public void Giving_Contributions_ContributorDetails_Pledges_Delete_All()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to the Pledges tab on the Contributor Details page
            test.Portal.Giving_ContributorDetails_Pledges_Delete_All_WebDriver("FT Tester");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies a user can view the Pledges tab for an individual on the Contributor Details page.")]
        public void Giving_Contributions_ContributorDetails_Pledges_View()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to the Pledges tab on the Contributor Details page
            test.Portal.Giving_ContributorDetails_Pledges_View_WebDriver("FT Tester");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }


        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies a user can add a pledge for an individual through the Contributor Details page.")]
        public void Giving_Contributions_ContributorDetails_Pledges_AddPledge()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to Pledges tab and add pledge
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Pledges_AddPledgeDrive_WebDriver("FT Tester", "Auto Test Pledge Drive", "Household", "109.00", "Monthly", today, today.AddYears(1));

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure, Timeout(1000)]
        [Author("David Martin")]
        [Description("Verifies a user can add a pledge to a second individual right after adding one to a different individual.")]
        public void Giving_Contributions_ContributorDetails_Pledges_AddPledge_SecondIndividual()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to Pledges tab and add pledges to TWO separate individuals
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Pledges_AddPledgeDrive_WebDriver("FT Tester", "Auto Test Pledge Drive", "Household", "299.00", "Monthly", today, today.AddYears(1));
            test.Portal.Giving_ContributorDetails_Pledges_AddPledgeDrive_WebDriver("Kevin Spacey", "Auto Test Pledge Drive", "Household", "199.00", "Monthly", today, today.AddYears(1));

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        #endregion Pledges

        #region Schedules - 1.0

        //[Test, RepeatOnFailure(Order = 3)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Creates a scheduled contribution via the contributor details page.")]
        public void Giving_Contributions_ContributorDetails_Schedules_1x_Create()
        {
            // Set initial conditions
            int individualId = base.SQL.People_Individuals_FetchID(256, _individualNameFTAutoTester);
            double amountCC = 10.01;
            double amountECheck = 10.02;
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(256, individualId, amountCC, 1);
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(256, individualId, amountECheck, 1);
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(99005, individualId, amountCC, 1);
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(99005, individualId, amountECheck, 1);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");
            test.Portal.LoginWebDriver("dmartin", "Amanda01!", "dcdmdfwtx");

            //Clean Things Up
            test.Portal.Giving_ContributorDetails_Scheduled_Delete_All_WebDriver(_individualNameFTAutoTester, amountCC, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1), "Credit Card");
            test.Portal.Giving_ContributorDetails_Scheduled_Delete_All_WebDriver(_individualNameFTAutoTester, amountECheck, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1), "eCheck");


            // Create a scheduled contribtion test.Portal.PortalUser
            test.Portal.Giving_ContributorDetails_Scheduled_Create_WebDriver(_individualNameFTAutoTester, amountCC, "One Time", null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1), "Auto Test Fund", null, null, "Visa", new string[] { _individualNameFTAutoTester, "4111111111111111", "8/2018" });
            System.Threading.Thread.Sleep(5000);
            test.Portal.Giving_ContributorDetails_Scheduled_Create_WebDriver(_individualNameFTAutoTester, amountECheck, "One Time", null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1), "Auto Test Fund", null, null, "eCheck", new string[] { "Bank of America", "111000025", "1234567890" });

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        //[Test, RepeatOnFailure(Order = 4)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Updates a scheduled contribution via the contributor details page")]
        public void Giving_Contributions_ContributorDetails_Schedules_1x_Update()
        {
            // Set initial conditions
            int individualId = base.SQL.People_Individuals_FetchID(256, _individualNameFTAutoTester);
            double amountCC = 10.03;
            double amountECheck = 10.04;
            double amountCCUpdated = 10.05;
            double amountECheckUpdated = 10.06;
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(256, individualId, amountCC, 1);
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(256, individualId, amountECheck, 1);
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(256, individualId, amountCCUpdated, 1);
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(256, individualId, amountECheckUpdated, 1);
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(99005, individualId, amountCC, 1);
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(99005, individualId, amountECheck, 1);
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(99005, individualId, amountCCUpdated, 1);
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(99005, individualId, amountECheckUpdated, 1);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");
            test.Portal.LoginWebDriver("dmartin", "Amanda01!", "dcdmdfwtx");

            //Clean Things Up
            test.Portal.Giving_ContributorDetails_Scheduled_Delete_All_WebDriver(_individualNameFTAutoTester, amountCC, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1), "Credit Card");
            test.Portal.Giving_ContributorDetails_Scheduled_Delete_All_WebDriver(_individualNameFTAutoTester, amountECheck, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1), "eCheck");
            test.Portal.Giving_ContributorDetails_Scheduled_Delete_All_WebDriver(_individualNameFTAutoTester, amountCCUpdated, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1), "Credit Card");
            test.Portal.Giving_ContributorDetails_Scheduled_Delete_All_WebDriver(_individualNameFTAutoTester, amountECheckUpdated, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1), "eCheck");

            // Create a scheduled contribution
            test.Portal.Giving_ContributorDetails_Scheduled_Create_WebDriver(_individualNameFTAutoTester, amountCC, "One Time", null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1), "Auto Test Fund", null, null, "Visa", new string[] { _individualNameFTAutoTester, "4111111111111111", "8/2019" });
            System.Threading.Thread.Sleep(5000);
            test.Portal.Giving_ContributorDetails_Scheduled_Create_WebDriver(_individualNameFTAutoTester, amountECheck, "One Time", null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1), "Auto Test Fund", null, null, "eCheck", new string[] { "Bank of America", "111000025", "1234567890" });

            // Update a scheduled contribtion
            test.Portal.Giving_ContributorDetails_Scheduled_Update_WebDriver(_individualNameFTAutoTester, amountCC, "One Time", null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1), "Auto Test Fund", null, null, "Visa", new string[] { _individualNameFTAutoTester, "4111111111111111", "8/2019" }, amountCCUpdated);
            System.Threading.Thread.Sleep(5000);
            test.Portal.Giving_ContributorDetails_Scheduled_Update_WebDriver(_individualNameFTAutoTester, amountECheck, "One Time", null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1), "Auto Test Fund", null, null, "eCheck", new string[] { "Bank of America", "111000025", "1234567890" }, amountECheckUpdated);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }


        //[Test, RepeatOnFailure(Order = 2)]
        [Author("David Martin")]
        [Category(TestCategories.Services.SmokeTest)]
        [Description("Deletes a scheduled contribution via the contributor details page.")]
        public void Giving_Contributions_ContributorDetails_Schedules_1x_Delete()
        {
            // Set initial conditions
            int individualId = base.SQL.People_Individuals_FetchID(256, _individualNameFTAutoTester);
            double amountCC = 10.07;
            double amountECheck = 10.08;
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(256, individualId, amountCC, 1);
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(256, individualId, amountECheck, 1);
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(99005, individualId, amountCC, 1);
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(99005, individualId, amountECheck, 1);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");
            test.Portal.LoginWebDriver("dmartin", "Amanda01!", "dcdmdfwtx");

            //Clean Things Up
            test.Portal.Giving_ContributorDetails_Scheduled_Delete_All_WebDriver(_individualNameFTAutoTester, amountCC, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1), "Credit Card");
            test.Portal.Giving_ContributorDetails_Scheduled_Delete_All_WebDriver(_individualNameFTAutoTester, amountECheck, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1), "eCheck");


            // Create a scheduled contribution
            test.Portal.Giving_ContributorDetails_Scheduled_Create_WebDriver(_individualNameFTAutoTester, amountCC, "One Time", null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1), "Auto Test Fund", null, null, "Visa", new string[] { _individualNameFTAutoTester, "4111111111111111", "8/2019" });
            System.Threading.Thread.Sleep(5000);
            test.Portal.Giving_ContributorDetails_Scheduled_Create_WebDriver(_individualNameFTAutoTester, amountECheck, "One Time", null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1), "Auto Test Fund", null, null, "eCheck", new string[] { "Bank of America", "111000025", "1234567890" });

            // Delete a scheduled contribtion
            test.Portal.Giving_ContributorDetails_Scheduled_Delete_WebDriver(_individualNameFTAutoTester, amountCC);
            System.Threading.Thread.Sleep(5000);
            test.Portal.Giving_ContributorDetails_Scheduled_Delete_WebDriver(_individualNameFTAutoTester, amountECheck);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        #endregion Schedules - 1.0


    }

    //FGJ Commenting DoesNotRunOutisdeOfStaging since we want these TC to execute against legacy (QA) and AMS (Staging) PP
    [TestFixture]
    [Category(TestCategories.Services.PaymentProcessor)]
    public class Portal_Giving_Contributions_ContributorDetails_Schedules_WebDriver : FixtureBaseWebDriver
    {

        #region Fixture Teardown
        [FixtureTearDown]
        public void FixtureTearDown()
        {
            try
            {
                base.SQL.Giving_ScheduleGiving_DeleteAll(15, "ft.autotester@gmail.com");
                base.SQL.Giving_ScheduleGiving_DeleteAll(258, "ft.autotester@gmail.com");
//                base.SQL.Giving_PledgeDrives_Pledge_Delete(15, "FT Tester");
            }
            catch (Exception e)
            {
                TestLog.WriteLine(e);
            }
        }

        #endregion Fixture Teardown

        #region Credit Cards

        #region One Time

        #region Success

        [Test, RepeatOnFailure, Timeout(1000)]
        [Author("David Martin")]
        [Description("Performs a One Time scheduled giving for a Visa card in US church. This is for Online Giving 2.0")]
        public void Giving_Contributions_ContributorDetails_Schedules_OneTime_CreditCard_Success_Today_15()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a one time scheduled giving for Visa
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Date;
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "39.99", today, "FT", "Tester",
                                                                                         "Visa", "4111111111111111", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure, Timeout(8000)]
        [Author("Stuart Platt")]
        [Description("Performs a One Time scheduled giving for a Visa card in US church where the expiration date is in the current month and year. This is for Online Giving 2.0")]
        public void Giving_Contributions_ContributorDetails_Schedules_OneTime_CreditCard_Success_Today_15_ExpirationDate_Current_Month()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a one time scheduled giving for Visa
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Date;
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "9.99", today, 
                                                      "FT", "Tester", "Visa", "4111111111111111", today.ToString("M" + " - " + "MMMM"), (today.Year).ToString()); 

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a One Time scheduled giving for a Visa card in INTERNATIONAL church. This is for Online Giving 2.0")]
        public void Giving_Contributions_ContributorDetails_Schedules_OneTime_CreditCard_Success_Today_258()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C6");

            // Perform a one time scheduled giving for Visa
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Date;
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "49.99", today,
                                                                                         "FT", "Tester", "Visa", "4111111111111111", "12 - December", today.AddYears(1).Year.ToString(),
                                                                                         null, null, "", "258");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Sifter 7736: Performs a One Time scheduled giving for a Visa card for user with no linked e-mail address")]
        public void Giving_Contributions_ContributorDetails_Schedules_OneTime_CreditCard_Sucesss_Individual_No_Linked_Email()
        {

            string indFirstName = "FT";
            string indLastName = "NoEmail";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // View an individual
            test.Portal.People_ViewIndividual_WebDriver(string.Format("{0} {1}", indFirstName, indLastName));

            // Perform a one time scheduled giving for Visa
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver(string.Format("{0} {1}", indFirstName, indLastName), "Auto Test Fund", null, "19.97", today.AddDays(1),
                                                                               indFirstName, indLastName, "Visa", "4111111111111111",
                                                                               "12 - December", today.AddYears(1).Year.ToString(), null, null, "", "15",
                                                                               "3025 Bryan St 3D", "", "Dallas", "Texas", "75204-6175", "Dallas");

            // Logout of Portal
            test.Portal.LogoutWebDriver();


        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Performs a One Time scheduled giving for a Visa card. This is for Online Giving 2.0")]
        public void Giving_Contributions_ContributorDetails_Schedules_OneTime_CreditCard_Success_Visa()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a one time scheduled giving for Visa
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "30.00", today.AddDays(1), 
                                                                                         "FT", "Tester", "Visa", "4111111111111111", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a One Time scheduled giving for a MasterCard. This is for Online Giving 2.0")]
        public void Giving_Contributions_ContributorDetails_Schedules_OneTime_CreditCard_Success_MasterCard()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a one time scheduled giving for MasterCard
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "30.01", today.AddDays(1), 
                                                                      "FT", "Tester", "Master Card", "5555555555554444", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a One Time scheduled giving for an American Express card. This is for Online Giving 2.0")]
        public void Giving_Contributions_ContributorDetails_Schedules_OneTime_CreditCard_Success_AmericanExpress()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a one time scheduled giving for American Express
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "30.02", today.AddDays(1), 
                                                               "FT", "Tester", "American Express", "378282246310005", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a One Time scheduled giving for a Discover card. This is for Online Giving 2.0")]
        public void Giving_Contributions_ContributorDetails_Schedules_OneTime_CreditCard_Success_Discover()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a one time scheduled giving for Discover
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "30.03", today.AddDays(1), 
                                                                  "FT", "Tester", "Discover", "6011111111111117", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a One Time scheduled giving for a JCB card. This is for Online Giving 2.0")]
        public void Giving_Contributions_ContributorDetails_Schedules_OneTime_CreditCard_Success_JCB()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C6");

            // Perform a one time scheduled giving for JCB
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "30.58", today.AddDays(1), 
                                                                                         "FT", "Tester", "JCB", "3566111111111113", "12 - December", today.AddYears(1).Year.ToString(), 
                                                                                         null, null, "", "258");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a One Time scheduled giving with Sub Fund selected. This is for Online Giving 2.0")]
        public void Giving_Contributions_ContributorDetails_Schedules_OneTime_CreditCard_Success_WithSubFund()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a one time scheduled giving for Visa
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver("FT Tester", "Auto Test Fund w Sub Fund", "Auto Test Sub Fund", "30.30", today.AddDays(1), 
                                                                       "FT", "Tester", "Visa", "4111111111111111", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure, Timeout(8000)]
        [Author("David Martin")]
        [Description("Performs a One Time scheduled giving for today with Sub Fund selected. This is for Online Giving 2.0")]
        public void Giving_Contributions_ContributorDetails_Schedules_OneTime_CreditCard_Success_Today_WithSubFund()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a one time scheduled giving for Visa
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Date;
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver("FT Tester", "Auto Test Fund w Sub Fund", "Auto Test Sub Fund", "45.99", today, 
                                                                          "FT", "Tester", "Visa", "4111111111111111", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }



        #endregion Success

        #region Validation

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the proper error message occurs when a schedule date is in the past")]
        public void Giving_Contributions_ContributorDetails_Schedules_CreditCard_Validation_ScheduleDateInPast()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule with a schedule date in the past
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "30.51", today.AddDays(-1), 
                                                                                         "FT", "Tester", "Visa", "4111111111111111", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }


        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the proper error message occurs when the Fund/Pledge Drive is not provided")]
        public void Giving_Contributions_ContributorDetails_Schedules_CreditCard_Validation_NoFundOrPledgeDrive()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform Step 1 without selecting a Fund/Pledge Drive
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver("FT Tester", "--", null, "30.50", today, 
                                                   "FT", "Tester", "Visa", "4111111111111111", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Verifies the proper error message occurs when no address is not provided")]
        public void Giving_Contributions_ContributorDetails_Schedules_CreditCard_Validation_NoAddress1()
        {

            string indFirstName = "FT";
            string indLastName = "NoAddress";
            base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", indFirstName, indLastName), "Merge Dump");
            base.SQL.People_Individual_Create(15, indFirstName, indLastName, 1);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a one time scheduled giving for Visa
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
                test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver(string.Format("{0} {1}", indFirstName, indLastName), "Auto Test Fund", null, "19.97", today.AddDays(1),
                                                                                   indFirstName, indLastName, "Visa", "4111111111111111",
                                                                                   "12 - December", today.AddYears(1).Year.ToString(),
                                                                                   null, null, "", "15",
                                                                                   "", "", "Mercedes", "Texas", "78570");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Cleanup
            base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", indFirstName, indLastName), "Merge Dump");

        }

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Verifies the proper error message occurs when no city is not provided")]
        public void Giving_Contributions_ContributorDetails_Schedules_CreditCard_Validation_NoCity()
        {

            string indFirstName = "FT";
            string indLastName = "NoCity";
            base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", indFirstName, indLastName), "Merge Dump");
            base.SQL.People_Individual_Create(15, indFirstName, indLastName, 1);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a one time scheduled giving for Visa
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver(string.Format("{0} {1}", indFirstName, indLastName), "Auto Test Fund", null, "19.97", today.AddDays(1),
                                                                               indFirstName, indLastName, "Visa", "4111111111111111",
                                                                               "12 - December", today.AddYears(1).Year.ToString(),
                                                                               null, null, "", "15",
                                                                               "347 Palm Ave", "", "", "Texas", "78570");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Cleanup
            base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", indFirstName, indLastName), "Merge Dump");

        }

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Verifies the proper error message occurs when no zip is not provided")]
        public void Giving_Contributions_ContributorDetails_Schedules_CreditCard_Validation_NoZip()
        {

            string indFirstName = "FT";
            string indLastName = "NoZip";
            base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", indFirstName, indLastName), "Merge Dump");
            base.SQL.People_Individual_Create(15, indFirstName, indLastName, 1);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a one time scheduled giving for Visa
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver(string.Format("{0} {1}", indFirstName, indLastName), "Auto Test Fund", null, "19.97", today.AddDays(1),
                                                                               indFirstName, indLastName, "Visa", "4111111111111111",
                                                                               "12 - December", today.AddYears(1).Year.ToString(),
                                                                               null, null, "", "15",
                                                                               "347 Palm Ave", "", "Mercedes", "Texas", "");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Cleanup
            base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", indFirstName, indLastName), "Merge Dump");

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the proper error message occurs when the amount is not provided")]
        public void Giving_Contributions_ContributorDetails_Schedules_CreditCard_Validation_NoAmount()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform Step 1 without entering an amount
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "", today, 
                                                                    "FT", "Tester", "Visa", "4111111111111111", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the proper error message occurs when zero amount is provided")]
        public void Giving_Contributions_ContributorDetails_Schedules_CreditCard_Validation_ZeroAmount()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform Step 1 with a Zero dollar amount entered
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "0.00", today, 
                                                           "FT", "Tester", "Visa", "4111111111111111", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the proper error message occurs when First Name is not provided")]
        public void Giving_Contributions_ContributorDetails_Schedules_CreditCard_Validation_NoFirstName()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule without first name provided
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "30.52", today, 
                                                        "", "Tester", "Visa", "4111111111111111", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the proper error message occurs when Last Name is not provided")]
        public void Giving_Contributions_ContributorDetails_Schedules_CreditCard_Validation_NoLastName()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule without last name provided
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "30.53", today, 
                                                 "FT", "", "Visa", "4111111111111111", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the proper error message occurs when credit card type is not provided")]
        public void Giving_Contributions_ContributorDetails_Schedules_CreditCard_Validation_NoCreditCardType()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule without credit card type provided
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "30.54", today, 
                                                  "FT", "Tester", "--", "4111111111111111", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies the proper error message occurs when credit card number is not provided")]
        public void Giving_Contributions_ContributorDetails_Schedules_CreditCard_Validation_NoCreditCardNumber()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule without credit card number provided
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "30.55", today, 
                                                      "FT", "Tester", "Visa", "", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the proper error message occurs when expiration month is not provided")]
        public void Giving_Contributions_ContributorDetails_Schedules_CreditCard_Validation_NoExpireMonth()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule without expiration month provided
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "30.56", today, 
                                                  "FT", "Tester", "Visa", "4111111111111111", "--", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the proper error message occurs when expiration year is not provided")]
        public void Giving_Contributions_ContributorDetails_Schedules_CreditCard_Validation_NoExpireYear()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule without expiration year provided
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "30.57", today, 
                                          "FT", "Tester", "Visa", "4111111111111111", "12 - December", "--");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        #endregion Validation

        #endregion One Time

        #region Monthly

        #region Success

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Performs a Monthly scheduled giving for a Visa card. This is for Online Giving 2.0")]
        public void Giving_Contributions_ContributorDetails_Schedules_Monthly_CreditCard_Success_Visa()
        {
            // Store variables
            var startDay = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Day.ToString();
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(2).ToString("MMMM");

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_Monthly_WebDriver("FT Tester", "Auto Test Fund", null, "30.04", startDay, startMonth, DateTime.Now.Year.ToString(), null, null, 
                                                         "FT", "Tester", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Monthly scheduled giving for a MasterCard. This is for Online Giving 2.0")]
        public void Giving_Contributions_ContributorDetails_Schedules_Monthly_CreditCard_Success_MasterCard()
        {
            // Store variables
            var startDay = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Day.ToString();
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(2).ToString("MMMM");

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_Monthly_WebDriver("FT Tester", "Auto Test Fund", null, "30.05", startDay, startMonth, DateTime.Now.Year.ToString(), 
                                                                            null, null, "FT", "Tester", "Master Card", "5555555555554444", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Monthly scheduled giving for an American Express card. This is for Online Giving 2.0")]
        public void Giving_Contributions_ContributorDetails_Schedules_Monthly_CreditCard_Success_AmericanExpress()
        {
            // Store variables
            var startDay = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Day.ToString();
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(2).ToString("MMMM");

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_Monthly_WebDriver("FT Tester", "Auto Test Fund", null, "30.06", startDay, startMonth, DateTime.Now.Year.ToString(), 
                                                  null, null, "FT", "Tester", "American Express", "378282246310005", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Monthly scheduled giving for a Discover card. This is for Online Giving 2.0")]
        public void Giving_Contributions_ContributorDetails_Schedules_Monthly_CreditCard_Success_Discover()
        {
            // Store variables
            string startDay = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Day.ToString();
            string startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(2).ToString("MMMM");

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_Monthly_WebDriver("FT Tester", "Auto Test Fund", null, "30.07", startDay, startMonth, DateTime.Now.Year.ToString(), 
                                                                  null, null, "FT", "Tester", "Discover", "6011111111111117", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        //Updated by Jim
        [Test, RepeatOnFailure, Timeout(1200)]
        [Author("David Martin")]
        [Description("Performs a Monthly scheduled giving for a JCB card. This is for Online Giving 2.0")]
        public void Giving_Contributions_ContributorDetails_Schedules_Monthly_CreditCard_Success_JCB()
        {
            // Store variables
            CultureInfo culture = new CultureInfo("en-GB");

            var amount = "30.61";
            var fundName = "Auto Test Fund";
            var churchCode = "QAEUNLX0C6";
            var individualName = "FT Tester";
            
            DateTime now = DateTime.UtcNow;
            var startDay = now.Day.ToString();
            var startMonth = now.ToString("MMMM");
            var startYear = now.Year.ToString();
            var formattedDateInContributions = Convert.ToDateTime(string.Format("{0} {1} {2}", startDay, startMonth, startYear));

            var schDay = now.Day.ToString();
            var schMonth = now.AddMonths(1).ToString("MMMM");
            var schYear = now.AddMonths(1).Year.ToString();
            var formattedDateInSchedules = Convert.ToDateTime(string.Format("{0} {1} {2}", schDay, schMonth, schYear));

            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            var churchID = SQL.FetchChurchID(churchCode);
            var individualId = test.SQL.People_Individuals_FetchID(churchID, individualName);

            try
            {
                // Login to Portal
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", churchCode);

                // Perform schedule
                test.Portal.Giving_ContributorDetails_Schedules_CreditCard_Monthly_WebDriver_Plus(individualName, fundName, null, amount, startDay, startMonth, startYear, null, null,
                                                      "FT", "Tester", "JCB", "3566111111111113", "12 - December", now.AddYears(1).Year.ToString(),
                                                      null, null, "", "258");

                // Verify schedule exists
                var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Schedules, string.Format(culture, "{0:c}", Convert.ToDecimal(amount)), "Amount") + 1;
                Assert.AreEqual(test.GeneralMethods.ConvertDateToNeutralFormat(formattedDateInSchedules), test.Driver.FindElementByXPath(string.Format("//table[@class='grid']/tbody/tr[{0}]/td[2]", row)).Text, "Next occurence was incorrect.");

                //Verify the first contribution has been created successfully
                test.Driver.FindElement(By.LinkText("Contributions")).Click();
                test.GeneralMethods.WaitForPageIsLoaded(120);

                Assert.AreEqual(string.Format(culture, "{0:c}", Convert.ToDecimal(amount)), test.Driver.FindElementByXPath(string.Format("//table[@class='grid']/tbody/tr[2]/td[5]", row)).Text, "The correct amount is not found in the contribution list.");
                Assert.IsTrue(test.Driver.FindElementByXPath(string.Format("//table[@class='grid']/tbody/tr[2]/td[4]", row)).Text.Contains(fundName), "The designation is not correct.");

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            finally
            {
                test.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(churchID, individualId, double.Parse(amount), 1);
            }
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Monthly scheduled giving with Sub Fund selected. This is for Online Giving 2.0")]
        public void Giving_Contributions_ContributorDetails_Schedules_Monthly_CreditCard_Success_WithSubFund()
        {
            // Store variables
            var startDay = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Day.ToString();
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(2).ToString("MMMM");

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_Monthly_WebDriver("FT Tester", "Auto Test Fund w Sub Fund", "Auto Test Sub Fund", "30.31", startDay, startMonth, DateTime.Now.Year.ToString(), null, null, 
                                                  "FT", "Tester", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Monthly scheduled with End Date selected. This is for Online Giving 2.0")]
        public void Giving_Contributions_ContributorDetails_Schedules_Monthly_CreditCard_Success_WithEndDate()
        {
            // Store variables
            var startDay = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Day.ToString();
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(2).ToString("MMMM");
            var endMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(2).ToString("MMMM");

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_Monthly_WebDriver("FT Tester", "Auto Test Fund", null, "30.32", startDay, startMonth, DateTime.Now.Year.ToString(), endMonth, (DateTime.Now.Year + 1).ToString(), 
                                                  "FT", "Tester", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }


        #endregion Success

        #region Validation

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies the proper error message occurs when the Start Day is not provided")]
        public void Giving_Contributions_ContributorDetails_Schedules_Monthly_CreditCard_Validation_NoStartDay()
        {
            // Store variables
            var startDay = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Day.ToString();
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var endMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule without selecting a Start Day
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_Monthly_WebDriver("FT Tester", "Auto Test Fund", null, "31.00", "--", startMonth, DateTime.Now.Year.ToString(), 
                                                        null, null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the proper error message occurs when the Start Month is not provided")]
        public void Giving_Contributions_ContributorDetails_Schedules_Monthly_CreditCard_Validation_NoStartMonth()
        {
            // Store variables
            var startDay = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Day.ToString();
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var endMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule without selecting a Start Month
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_Monthly_WebDriver("FT Tester", "Auto Test Fund", null, "31.00", startDay, "--", DateTime.Now.Year.ToString(), 
                                                                 null, null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the proper error message occurs when the Start Year is not provided")]
        public void Giving_Contributions_ContributorDetails_Schedules_Monthly_CreditCard_Validation_NoStartYear()
        {
            // Store variables
            var startDay = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Day.ToString();
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var endMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule without selecting a Start Year
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_Monthly_WebDriver("FT Tester", "Auto Test Fund", null, "31.00", startDay, startMonth, "--", 
                                                  null, null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }


        #endregion Validation

        #endregion Monthly

        #region Twice Monthly

        #region Success

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Twice Monthly scheduled giving for a Visa card starting on the 1st. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_1st_CreditCard_Success_Visa()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund", null, "30.10", "1", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Twice Monthly scheduled giving for a Visa card starting on the 16th. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_16th_CreditCard_Success_Visa()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund", null, "30.11", "16", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Twice Monthly scheduled giving for a Master Card starting on the 1st. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_1st_CreditCard_Success_MasterCard()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund", null, "30.12", "1", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "Master Card", "5555555555554444", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Twice Monthly scheduled giving for a Master Card starting on the 16th. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_16th_CreditCard_Success_MasterCard()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund", null, "30.13", "16", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "Master Card", "5555555555554444", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Twice Monthly scheduled giving for an American Express card starting on the 1st. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_1st_CreditCard_Success_AmericanExpress()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund", null, "30.14", "1", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "American Express", "378282246310005", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Twice Monthly scheduled giving for an American Express card starting on the 16th. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_16th_CreditCard_Success_AmericanExpress()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund", null, "30.15", "16", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "American Express", "378282246310005", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Twice Monthly scheduled giving for a Discover card starting on the 1st. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_1st_CreditCard_Success_Discover()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund", null, "30.16", "1", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "Discover", "6011111111111117", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Twice Monthly scheduled giving for a Discover card starting on the 16th. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_16th_CreditCard_Success_Discover()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund", null, "30.17", "16", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "Discover", "6011111111111117", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Twice Monthly scheduled giving for a JCB card starting on the 1st. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_1st_CreditCard_Success_JCB()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C6");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund", null, "32.00", "1", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "JCB", "3566111111111113", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString(), null, null, "", "258");

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Twice Monthly scheduled giving for a JCB card starting on the 16th. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_16th_CreditCard_Success_JCB()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C6");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund", null, "32.01", "16", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "JCB", "3566111111111113", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString(), null, null, "", "258");

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Twice Monthly scheduled giving on the 1st with Sub Fund selected. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_1st_CreditCard_Success_WithSubFund()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund w Sub Fund", "Auto Test Sub Fund", "30.33", "1", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Twice Monthly scheduled giving on the 16th with Sub Fund selected. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_16th_CreditCard_Success_WithSubFund()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund w Sub Fund", "Auto Test Sub Fund", "30.40", "16", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Twice Monthly scheduled starting on 1st with End Date selected. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_1st_CreditCard_Success_WithEndDate()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var endMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund", null, "30.34", "1", startMonth, DateTime.Now.Year.ToString(), "1", endMonth, (DateTime.Now.Year + 1).ToString(), "FT", "Tester", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Twice Monthly scheduled starting on 16th with End Date selected. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_16th_CreditCard_Success_WithEndDate()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var endMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund", null, "30.35", "16", startMonth, DateTime.Now.Year.ToString(), "16", endMonth, (DateTime.Now.Year + 1).ToString(), "FT", "Tester", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout
            test.Portal.LogoutWebDriver();
        }
        #endregion Success

        #region Validation

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the proper error message occurs when the Start Day is not provided.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_CreditCard_Validation_NoStartDay()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var endMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund", null, "31.00", "--", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the proper error message occurs when the Start Month is not provided.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_CreditCard_Validation_NoStartMonth()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var endMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund", null, "31.00", "1", "--", DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the proper error message occurs when the Start Year is not provided.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_CreditCard_Validation_NoStartYear()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var endMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund", null, "31.00", "1", startMonth, "--", null, null, null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        #endregion Validation

        #endregion Twice Monthly


        #region Weekly

        #region Success

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Weekly scheduled giving for a Visa card. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_Weekly_CreditCard_Success_Visa()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_Weekly_WebDriver("FT Tester", "Auto Test Fund", null, "30.20", today.AddDays(1).DayOfWeek, today.AddDays(1), null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Weekly scheduled giving for a Master Card. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_Weekly_CreditCard_Success_MasterCard()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_Weekly_WebDriver("FT Tester", "Auto Test Fund", null, "30.21", today.AddDays(1).DayOfWeek, today.AddDays(1), null, "FT", "Tester", "Master Card", "5555555555554444", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Weekly scheduled giving for an American Express card. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_Weekly_CreditCard_Success_AmericanExpress()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_Weekly_WebDriver("FT Tester", "Auto Test Fund", null, "30.22", today.AddDays(1).DayOfWeek, today.AddDays(1), null, "FT", "Tester", "American Express", "378282246310005", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Daniel Lee")]
        [Description("FO-3243 End Dates Are Not Appearing For Contribution Schedules")]
        public void Giving_Contributions_ContributorDetails_Schedules_Weekly_CreditCard_Success_Verify_End_Date_Appear()
        {
            base.SQL.Giving_InactivateScheduledGivingByName("daniel", "lee");
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_Weekly_WebDriver("daniel lee", "Auto Test Fund", null, "30.22", today.AddDays(1).DayOfWeek, today.AddDays(1), today.AddYears(1), "FT", "Tester", "American Express", "378282246310005", "12 - December", today.AddYears(1).Year.ToString());

            //Navigate to edit schedule page and verify Month and Year are enabled and correct
            test.Driver.FindElementByXPath("//div[@id = 'schedule_grid']/table[1]/tbody/tr[2]/td[1]/a[1]").Click();
            test.Driver.FindElementByXPath("//div[@id = 'main_content']/div[1]/div[2]/table[2]/tbody/tr[1]/th[1]/a[1]").Click();
            Assert.IsTrue(test.Driver.FindElementById("weekly_ends").Enabled);

            String format = "MM/dd/yyyy";
            string endDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddYears(1), TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(format, DateTimeFormatInfo.InvariantInfo);
            Assert.AreEqual(endDate, test.Driver.FindElementById("weekly-end-date").GetAttribute("value").ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Weekly scheduled giving for a Discover card. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_Weekly_CreditCard_Success_Discover()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_Weekly_WebDriver("FT Tester", "Auto Test Fund", null, "30.23", today.AddDays(1).DayOfWeek, today.AddDays(1), null, "FT", "Tester", "Discover", "6011111111111117", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Weekly scheduled giving for a JCB card. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_Weekly_CreditCard_Success_JCB()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C6");

            // Perform schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_Weekly_WebDriver("FT Tester", "Auto Test Fund", null, "33.00", today.AddDays(1).DayOfWeek, today.AddDays(1), null, "FT", "Tester", "JCB", "3566111111111113", "12 - December", today.AddYears(1).Year.ToString(), null, null, "", "258");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Weekly scheduled giving with Sub Fund selected. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_Weekly_CreditCard_Success_WithSubFund()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_Weekly_WebDriver("FT Tester", "Auto Test Fund w Sub Fund", "Auto Test Sub Fund", "30.36", today.AddDays(1).DayOfWeek, today.AddDays(1), null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Weekly scheduled giving with End Date selected. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_Weekly_CreditCard_Success_WithEndDate()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_Weekly_WebDriver("FT Tester", "Auto Test Fund", null, "30.37", today.AddDays(1).DayOfWeek, today.AddDays(1), today.AddYears(1), "FT", "Tester", "Visa", "4111111111111111", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Success

        #region Validation

        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the proper error message occurs when the Day of the Week is not provided for a weekly schedule.")]
        public void Giving_Contributions_ContributorDetails_Schedules_Weekly_CreditCard_Validation_NoStartDay()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            // DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            // test.Portal.Giving_ContributorDetails_Schedules_CreditCard_Weekly_WebDriver("FT Tester", "Auto Test Fund", null, "31.00", "--", today.AddDays(1), null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the proper error message occurs when the Start Date is not provided for a weekly schedule.")]
        public void Giving_Contributions_ContributorDetails_Schedules_Weekly_CreditCard_Validation_NoStartDate()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            // test.Portal.Giving_ContributorDetails_Schedules_CreditCard_Weekly_WebDriver("FT Tester", "Auto Test Fund", null, "31.00", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1).DayOfWeek, null, null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        #endregion Validation

        #endregion Weekly

        #region Every Two Weeks

        #region Success

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs an Every Two Weeks scheduled giving for a Visa card. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_EveryTwoWeeks_CreditCard_Success_Visa()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_EveryTwoWeeks_WebDriver("FT Tester", "Auto Test Fund", null, "30.25", today.AddDays(1).DayOfWeek, today.AddDays(1), null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs an Every Two Weeks scheduled giving for a Master Card card. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_EveryTwoWeeks_CreditCard_Success_MasterCard()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_EveryTwoWeeks_WebDriver("FT Tester", "Auto Test Fund", null, "30.26", today.AddDays(1).DayOfWeek, today.AddDays(1), null, "FT", "Tester", "Master Card", "5555555555554444", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs an Every Two Weeks scheduled giving for an American Express card. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_EveryTwoWeeks_CreditCard_Success_AmericanExpress()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_EveryTwoWeeks_WebDriver("FT Tester", "Auto Test Fund", null, "30.27", today.AddDays(1).DayOfWeek, today.AddDays(1), null, "FT", "Tester", "American Express", "378282246310005", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs an Every Two Weeks scheduled giving for a Discover card. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_EveryTwoWeeks_CreditCard_Success_Discover()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_EveryTwoWeeks_WebDriver("FT Tester", "Auto Test Fund", null, "30.28", today.AddDays(1).DayOfWeek, today.AddDays(1), null, "FT", "Tester", "Discover", "6011111111111117", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs an Every Two Weeks scheduled giving for a JCB card. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_EveryTwoWeeks_CreditCard_Success_JCB()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C6");

            // Perform schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_EveryTwoWeeks_WebDriver("FT Tester", "Auto Test Fund", null, "32.20", today.AddDays(1).DayOfWeek, today.AddDays(1), null, "FT", "Tester", "JCB", "3566111111111113", "12 - December", today.AddYears(1).Year.ToString(), null, null, "", "258");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs an Every Two Weeks scheduled giving with Sub Fund. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_EveryTwoWeeks_CreditCard_Success_WithSubFund()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_EveryTwoWeeks_WebDriver("FT Tester", "Auto Test Fund w Sub Fund", "Auto Test Sub Fund", "30.38", today.AddDays(1).DayOfWeek, today.AddDays(1), null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs an Every Two Weeks scheduled giving with End Date. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_EveryTwoWeeks_CreditCard_Success_WithEndDate()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_CreditCard_EveryTwoWeeks_WebDriver("FT Tester", "Auto Test Fund", null, "30.39", today.AddDays(1).DayOfWeek, today.AddDays(1), today.AddYears(1), "FT", "Tester", "Visa", "4111111111111111", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Success

        #region Validation

        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the proper error message occurs when StartDay is not provided for an Every Two Week schedule")]
        public void Giving_Contributions_ContributorDetails_Schedules_EveryTwoWeeks_CreditCard_Validation_NoStartDay()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            // DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            // test.Portal.Giving_ContributorDetails_Schedules_CreditCard_EveryTwoWeeks_WebDriver("FT Tester", "Auto Test Fund", null, "31.00", "--", today.AddDays(1), null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the proper error message occurs when StartDate is not provided for an Every Two Week schedule")]
        public void Giving_Contributions_ContributorDetails_Schedules_EveryTwoWeeks_CreditCard_Validation_NoStartDate()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            // DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            // test.Portal.Giving_ContributorDetails_Schedules_CreditCard_EveryTwoWeeks_WebDriver("FT Tester", "Auto Test Fund", null, "31.00", today.AddDays(1).DayOfWeek, null, null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", today.AddYears(1).Year.ToString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        #endregion Validation

        #endregion Every Two Weeks

        #endregion Credit Cards

        #region Personal Checks

        #region One Time

        #region Success

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Sifter 7736: Performs a One Time scheduled giving for a Visa card for user with no linked e-mail address or address or phone number")]
        public void Giving_Contributions_ContributorDetails_Schedules_OneTime_PersonalCheck_Sucesss_Individual_No_Address_PhoneNumber()
        {

            string indFirstName = "FT";
            string indLastName = "NoEmail";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // View an individual
            test.Portal.People_ViewIndividual_WebDriver(string.Format("{0} {1}", indFirstName, indLastName));

            // Perform a one time scheduled giving for Visa
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_OneTime_WebDriver(string.Format("{0} {1}", indFirstName, indLastName), "Auto Test Fund", null, "19.75",
                                                                                  today.AddDays(1),
                                                                                 "9728015973", "111000025", "918272635", "15");

            // Logout of Portal
            test.Portal.LogoutWebDriver();


        }


        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a One Time scheduled giving for a Personal Check. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_OneTime_PersonalCheck_Success()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a one time scheduled giving for Personal Check
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Date;
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "30.08",
                                                                                  today.AddDays(1),
                                                                                 "8176833453", "111000025", "111222333", "15");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure, Timeout(1000)]
        [Author("David Martin")]
        [Description("Performs a One Time scheduled giving for a Personal Check run today. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_OneTime_PersonalCheck_Success_Today()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a one time scheduled giving for Personal Check
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Date;
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "88.88", today, "8176833453", "111000025", "111222333", "15");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Performs a One Time scheduled giving for a Personal Check with Sub Fund selected. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_OneTime_PersonalCheck_Success_WithSubFund()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a one time scheduled giving for Personal Check
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Date;
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_OneTime_WebDriver("FT Tester", "Auto Test Fund w Sub Fund", "Auto Test Sub Fund", "30.47", today.AddDays(1), "8176833453", "111000025", "111222333", "15");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure, Timeout(1000)]
        [Author("David Martin")]
        [Description("Performs a One Time scheduled giving for a Personal Check with Sub Fund selected. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_OneTime_PersonalCheck_Success_Today_WithSubFund()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a one time scheduled giving for Personal Check
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Date;
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_OneTime_WebDriver("FT Tester", "Auto Test Fund w Sub Fund", "Auto Test Sub Fund", "337.37", today, "8176833453", "111000025", "111222333", "15");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Success

        #endregion One Time

        #region Monthly

        #region Success

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Performs a Monthly scheduled giving for a Personal Check. This is for Onlive Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_Monthly_PersonalCheck_Success()
        {
            // Store variables
            var startDay = "1";
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var startYear = string.Empty;

            if (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month == 12)
            {
                startYear = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).ToString("yyyy");
                TestLog.WriteLine("We are in DEC, Add Year too: " + startYear);
            }
            else
            {
                startYear = DateTime.Now.Year.ToString();
            }

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a monthly scheduled giving for Personal Check
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_Monthly_WebDriver("FT Tester", "Auto Test Fund", null, "30.09", startDay, startMonth, startYear, null, null, "8176833453", "111000025", "111222333", "15");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        //        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Performs a Monthly scheduled giving for a Personal Check. Runs the stored procedure to process the schedules. This is for Onlive Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_Monthly_PersonalCheck_Success_Populate()
        {
            // Store variables
            DateTime currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            var startDay = currentDateTime.Day.ToString();
            var startMonth = currentDateTime.AddMonths(2).ToString("MMMM");
            var startYear = string.Empty;

            if (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month == 12)
            {
                startYear = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).ToString();
                TestLog.WriteLine("We are in DEC, Add Year too: " + startYear);
            }
            else
            {
                startYear = DateTime.Now.Year.ToString();
            }


            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a monthly scheduled giving for Personal Check
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_Monthly_WebDriver("FT Tester", "Auto Test Fund", null, "30.09", startDay, startMonth, startYear, null, null, "8176833453", "111000025", "111222333", "15");

            // Logout of Portal
            TestLog.WriteLine("Logout");
            test.Portal.LogoutWebDriver();

            // Move the StartDate and NextPaymentDate back
            TestLog.WriteLine("Build SQL Stmt");
            base.SQL.Execute(string.Format("UPDATE ChmContribution.dbo.ScheduledGiving SET StartDate = '{0}', NextPaymentDate = '{0}' WHERE ChurchID = 15 AND IsActive = 1 AND CreatedDate > '{1}' AND CreatedByLogin = 'msneeden@fellowshiptech.com'", string.Format("{0}-{1}-{2}",
                currentDateTime.Year, startMonth, startDay), currentDateTime.ToString("yyyy-MM-dd HH:mm:ss")));

            // Execute the stored procedure to run the schedules
            TestLog.WriteLine("Exeucte SQL Stmt");
            base.SQL.Execute("exec ChmContribution.Pmt.ScheduledGiving_PopulateAll");

            //Launch Payment Processor


        }


        [Test, RepeatOnFailure, Timeout(1000)]
        [Author("David Martin")]
        [Description("Performs a Monthly scheduled giving for a Personal Check with Sub Fund selected. This is for Onlive Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_Monthly_PersonalCheck_Success_WithSubFund()
        {
            // Store variables
            var startDay = "1";
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var startYear = string.Empty;

            if (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month == 12)
            {
                startYear = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).ToString("yyyy");
                TestLog.WriteLine("We are in DEC, Add Year too: " + startYear);
            }
            else
            {
                startYear = DateTime.Now.Year.ToString();
            }


            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a monthly scheduled giving for Personal Check
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_Monthly_WebDriver("FT Tester", "Auto Test Fund w Sub Fund", "Auto Test Sub Fund", "30.42", startDay, startMonth, startYear, null, null, "8176833453", "111000025", "111222333", "15");

            //test for FO-3809,add by grace zhang
            test.Driver.FindElementById("schedule_grid").FindElement(By.TagName("table")).FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[0].FindElement(By.TagName("a")).Click();
            
            test.Driver.FindElementById("lnk_delete_schedule").Click();

            try
            {
                IAlert confirm = test.Driver.SwitchTo().Alert();

                confirm.Accept();
               
            }
            catch (NoAlertPresentException)
            {

                TestLog.WriteLine("No Confirm here!");
            }
          
            // Verify the schedule was deleted
            Assert.AreEqual("No records found", test.Driver.FindElementById("schedule_grid").FindElement(By.TagName("p")).Text.ToString());
           //test for FO-3809,add by grace zhang

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Monthly scheduled giving for a Personal Check with End Date selected. This is for Onlive Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_Monthly_PersonalCheck_Success_WithEndDate()
        {
            // Store variables
            var startDay = "1";
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var endMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(2).ToString("MMMM");
            var startYear = string.Empty;

            if (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month == 12)
            {
                startYear = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).ToString("yyyy");
                TestLog.WriteLine("We are in DEC, Add Year too: " + startYear);
            }
            else
            {
                startYear = DateTime.Now.Year.ToString();
            }


            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a monthly scheduled giving for Personal Check
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_Monthly_WebDriver("FT Tester", "Auto Test Fund", null, "30.41", startDay, startMonth, startYear, endMonth, (DateTime.Now.Year + 1).ToString(), "8176833453", "111000025", "111222333", "15");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Success

        #endregion Monthly

        #region Twice Monthly

        #region Success

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Twice Monthly scheduled giving for a Personal Check starting on the 1st. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_1st_PersonalCheck_Success()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var startYear = string.Empty;

            if (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month == 12)
            {
                startYear = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).ToString("yyyy");
                TestLog.WriteLine("We are in DEC, Add Year too: " + startYear);
            }
            else
            {
                startYear = DateTime.Now.Year.ToString();
            }

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a Twice Monthly scheduled giving for Personal Check starting on the 1st
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund", null, "30.18", "1", startMonth, startYear, null, null, null, "8176833453", "111000025", "111222333", "15");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Twice Monthly scheduled giving for a Personal Check starting on 1st with Sub Fund selected. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_1st_PersonalCheck_Success_WithSubFundANDEndDate()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var endMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var startYear = string.Empty;

            if (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month == 12)
            {
                startYear = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).ToString("yyyy");
                TestLog.WriteLine("We are in DEC, Add Year too: " + startYear);
            }
            else
            {
                startYear = DateTime.Now.Year.ToString();
            }

            // Login
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund w Sub Fund", "Auto Test Sub Fund", "30.44", "1", startMonth, startYear, "1", endMonth, (DateTime.Now.Year + 1).ToString(), "8176833453", "111000025", "111222333", "15");

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Twice Monthly scheduled giving for a Personal Check starting on 1st with End Date selected. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_1st_PersonalCheck_Success_WithEndDate()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var endMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var startYear = string.Empty;

            if (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month == 12)
            {
                startYear = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).ToString("yyyy");
                TestLog.WriteLine("We are in DEC, Add Year too: " + startYear);
            }
            else
            {
                startYear = DateTime.Now.Year.ToString();
            }

            // Login
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund", null, "30.43", "1", startMonth, startYear, "1", endMonth, (DateTime.Now.Year + 1).ToString(), "8176833453", "111000025", "111222333", "15");

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Twice Monthly scheduled giving for a Personal Check starting on the 16th. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_16th_PersonalCheck_Success()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var startYear = string.Empty;

            if (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month == 12)
            {
                startYear = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).ToString("yyyy");
                TestLog.WriteLine("We are in DEC, Add Year too: " + startYear);
            }
            else
            {
                startYear = DateTime.Now.Year.ToString();
            }

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a Twice Monthly scheduled giving for Personal Check starting on the 16th
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund", null, "30.19", "16", startMonth, startYear, null, null, null, "8176833453", "111000025", "111222333", "15");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Twice Monthly scheduled giving for a Personal Check starting on 16th with Sub Fund selected. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_16th_PersonalCheck_Success_WithSubFundANDEndDate()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var endMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var startYear = string.Empty;

            if (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month == 12)
            {
                startYear = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).ToString("yyyy");
                TestLog.WriteLine("We are in DEC, Add Year too: " + startYear);
            }
            else
            {
                startYear = DateTime.Now.Year.ToString();
            }

            // Login
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund w Sub Fund", "Auto Test Sub Fund", "30.45", "16", startMonth, startYear, "16", endMonth, (DateTime.Now.Year + 1).ToString(), "8176833453", "111000025", "111222333", "15");

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Twice Monthly scheduled giving for a Personal Check starting on 16th with End Date selected. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_TwiceMonthly_16th_PersonalCheck_Success_WithEndDate()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var endMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var startYear = string.Empty;

            if (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month == 12)
            {
                startYear = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).ToString("yyyy");
                TestLog.WriteLine("We are in DEC, Add Year too: " + startYear);
            }
            else
            {
                startYear = DateTime.Now.Year.ToString();
            }

            // Login
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_TwiceMonthly_WebDriver("FT Tester", "Auto Test Fund", null, "30.46", "16", startMonth, startYear, "16", endMonth, (DateTime.Now.Year + 1).ToString(), "8176833453", "111000025", "111222333", "15");

            // Logout
            test.Portal.LogoutWebDriver();
        }
        #endregion Success

        #endregion Twice Monthly

        #region Weekly

        #region Success

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Weekly scheduled giving for a Personal Check. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_Weekly_PersonalCheck_Success()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_Weekly_WebDriver("FT Tester", "Auto Test Fund", null, "30.24", today.AddDays(1).DayOfWeek, today.AddDays(1), null, "8176833453", "111000025", "111222333", "15");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Weekly scheduled giving for a Personal Check with Sub Fund selected. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_Weekly_PersonalCheck_Success_WithSubFund()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_Weekly_WebDriver("FT Tester", "Auto Test Fund w Sub Fund", "Auto Test Sub Fund", "33.47", today.AddDays(1).DayOfWeek, today.AddDays(1), null, "8176833453", "111000025", "111222333", "15");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a Weekly scheduled giving for a Personal Check with End Date selected. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_Weekly_PersonalCheck_Success_WithEndDate()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_Weekly_WebDriver("FT Tester", "Auto Test Fund", null, "30.48", today.AddDays(1).DayOfWeek, today.AddDays(1), today.AddYears(1), "8176833453", "111000025", "111222333", "15");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Success

        #endregion Weekly

        #region Every Two Weeks

        #region Success
        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs an Every Two Weeks scheduled giving for a Personal Check. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_EveryTwoWeeks_PersonalCheck_Success()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_EveryTwoWeeks_WebDriver("FT Tester", "Auto Test Fund", null, "30.29", today.AddDays(1).DayOfWeek, today.AddDays(1), null, "8176833453", "111000025", "111222333", "15");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs an Every Two Weeks scheduled giving for a Personal Check with Sub Fund selected. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_EveryTwoWeeks_PersonalCheck_Success_WithSubFund()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_EveryTwoWeeks_WebDriver("FT Tester", "Auto Test Fund w Sub Fund", "Auto Test Sub Fund", "30.49", today.AddDays(1).DayOfWeek, today.AddDays(1), null, "8176833453", "111000025", "111222333", "15");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs an Every Two Weeks scheduled giving for a Personal Check with End Date selected. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_EveryTwoWeeks_PersonalCheck_Success_WithEndDate()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_EveryTwoWeeks_WebDriver("FT Tester", "Auto Test Fund", null, "30.50", today.AddDays(1).DayOfWeek, today.AddDays(1), today.AddYears(1), "8176833453", "111000025", "111222333", "15");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Success

        #endregion Every Two Weeks

        #region Validation

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a One Time scheduled giving for a Personal Check without Phone Number provided. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_PersonalCheck_Validation_NoPhoneNumber()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a one time scheduled giving for Personal Check without Phone Number provided
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "30.61", today, "", "111000025", "111222333", "15");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a One Time scheduled giving for a Personal Check with Phone Number over 50 digits provided. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_PersonalCheck_Validation_PhoneNumberOver50()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a one time scheduled giving for Personal Check with Phone Number over 50 digits provided
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "30.61", today, "111111111122222222223333333333444444444455555555556", "111000025", "111222333", "15");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Performs a One Time scheduled giving for a Personal Check without Routing Number provided. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_PersonalCheck_Validation_NoRoutingNumber()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a one time scheduled giving for Personal Check without Routing Number provided
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "30.62", today, "8176833453", "", "111222333", "15");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a One Time scheduled giving for a Personal Check with a Routing Number over 9 digits provided. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_PersonalCheck_Validation_RoutingNumberOver9()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a one time scheduled giving for Personal Check with a Routing Number over 9 provided
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "30.63", today, "8176833453", "1111000025", "111222333", "15");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a One Time scheduled giving for a Personal Check with a Routing Number under 9 digits provided. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_PersonalCheck_Validation_RoutingNumberUnder9()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a one time scheduled giving for Personal Check with a Routing Number less than 9 provided
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "30.64", today, "8176833453", "11100025", "111222333", "15");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a One Time scheduled giving for a Personal Check without an Account Number provided. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_PersonalCheck_Validation_NoAccountNumber()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a one time scheduled giving for Personal Check without an Account Number provided
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "30.65", today, "8176833453", "111000025", "", "15");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a One Time scheduled giving for a Personal Check with an Account Number over 30 digits provided. This is for Online Giving 2.0.")]
        public void Giving_Contributions_ContributorDetails_Schedules_PersonalCheck_Validation_AccountNumberOver30()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a one time scheduled giving for Personal Check with an Account Number of 30 digits provided
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "30.65", today, "8176833453", "111000025", "1111111111222222222233333333334", "15");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure, DoesNotRunInStaging]
        [Author("David Martin")]
        [Description("Verifies that the proper error message is provided when a Velocity Dollar Amount error occurs for a One Time schedule.")]
        public void Giving_Contributions_ContributorDetails_Schedules_PersonalCheck_Validation_VelocityDollarAmount()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C2");

            // Perform a one time scheduled giving for Personal Check
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "1101.00", today, "8176833453", "111000025", "123123123", "254");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure, DoesNotRunInStaging]
        [Author("David Martin")]
        [Description("Verifies that the proper error message is provided when a Velocity Transaction Amount error occurs for a One Time schedule.")]
        public void Giving_Contributions_ContributorDetails_Schedules_PersonalCheck_Validation_VelocityTransactionAmount()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C2");

            // Perform a one time scheduled giving for Personal Check
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Giving_ContributorDetails_Schedules_PersonalCheck_OneTime_WebDriver("FT Tester", "Auto Test Fund", null, "1102.00", today, "8176833453", "111000025", "123123123", "254");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Validation

        #endregion Personal Checks

    }

    [TestFixture]
    public class Portal_Giving_Contributions_Batches_WebDriver : FixtureBaseWebDriver
    {

        #region Private Members
        private string _nonDCBatchID;

        private string _pledgeDriveStartDate;
        private string _fundID;
        private string _subFundID;
        private string _pledgeDriveName;
        private string _fundName;
        private string _subFundName;
        private DataTable _pledgeDrive;

        private string _ccBatchName = "Contribution: IND";

        #endregion Private Members

        [FixtureSetUp]
        public void FixtureSetUp()
        {

            base.SQL.Giving_Batches_Delete(15, _ccBatchName, 2);

            _nonDCBatchID = base.SQL.Execute("SELECT TOP 1 [BATCH_ID] FROM [ChmContribution].[dbo].[BATCH] WITH (NOLOCK) WHERE [CHURCH_ID] <> 15 AND [BatchTypeID] = 2").Rows[0][0].ToString();
            _pledgeDriveStartDate = base.SQL.Execute("DECLARE @startDate DATETIME SET @startDate = (SELECT TOP 1 [START_DATE] FROM [ChmContribution].[dbo].[PLEDGE_DRIVE] WITH (NOLOCK) WHERE CHURCH_ID = 15) SELECT CONVERT(VARCHAR, @startDate, 1)").Rows[0][0].ToString();
            _pledgeDrive = base.SQL.Execute("SELECT TOP 1 [PLEDGE_DRIVE_NAME], [FUND_ID], [SUB_FUND_ID] FROM [ChmContribution].[dbo].[PLEDGE_DRIVE] WITH (NOLOCK) WHERE CHURCH_ID = 15");
            _fundID = _pledgeDrive.Rows[0]["FUND_ID"].ToString();
            _subFundID = _pledgeDrive.Rows[0]["SUB_FUND_ID"].ToString();
            _pledgeDriveName = _pledgeDrive.Rows[0]["PLEDGE_DRIVE_NAME"].ToString();

            // Get the parent fund name
            _fundName = base.SQL.Execute(string.Format("SELECT TOP 1 [FUND_NAME] FROM [ChmContribution].[dbo].[FUND] WITH (NOLOCK) WHERE [FUND_ID] = {0}", _fundID)).Rows[0][0].ToString();

            // Get the sub fund name if present
            if (!string.IsNullOrEmpty(_subFundID))
            {
                _subFundName = base.SQL.Execute(string.Format("SELECT TOP 1 [SUB_FUND_NAME] FROM [ChmContribution].[dbo].[SUB_FUND] WITH (NOLOCK) WHERE [SUB_FUND_ID] = {1}", _subFundID)).Rows[0][0].ToString();
            }

            // Create a credit card batch for miscellaneous 'contribution' state tests
            base.SQL.Giving_Batches_Create(15, _ccBatchName, 100, 2);

        }

        [FixtureTearDown]
        public void FixtureTearDown()
        {
            base.SQL.Giving_Batches_Delete(15, _ccBatchName, 2);

        }

        #region All

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies that all batches links appear when all modules are turned on.")]
        public void Giving_Contributions_Batches_VerifyAllLinks_15()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Verify links
            test.Portal.Giving_Batches_View_WebDriver(15);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies that Credit Card Batches are not available when the module is turned off.")]
        public void Giving_Contributions_Batches_VerifyCreditCardNotPresent_255()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C3");

            // Verify links
            test.Portal.Giving_Batches_View_WebDriver(255);

            // Verify the controls for credit card batches are not present
            //test.Selenium.ClickAndWaitForPageToLoad("link=General Batches");
            test.Driver.FindElementByLinkText("General").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Add a new batch"));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//ul[@id='criteria_options']")), "criteria options is not present");

            // Verify credit card checkbox is not present on create batch page
            test.Driver.FindElementByLinkText("Add a new batch").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Cancel"));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//input[@id='is_cc']")));

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies that Credit Card Batches and RDC Batches are not available when the modules are turned off.")]
        public void Giving_Contributions_Batches_VerifyCreditCardAndRDCNotPresent_256()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Verify links
            test.Portal.Giving_Batches_View_WebDriver(256);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies that RDC Batches are not available when the module is turned off.")]
        public void Giving_Contributions_Batches_VerifyRDCNotPresent_258()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C6");

            // Verify links
            test.Portal.Giving_Batches_View_WebDriver(258);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        #region Create

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies error handling when the user attempts to create a Batch with a name greater than 45 characters.")]
        public void Giving_Contributions_Batches_CreateBatchNameGreaterThan45Characters()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Create a batch
            test.Portal.Giving_Batches_Create_WebDriver("1234567890123456789012345678901234567890123456", null, false, "t");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user is not allowed to enter non-numerical data in the batch amount field.")]
        public void Giving_Contributions_Batches_CreateNonNumericalBatchAmount()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            //Navigate to Batches
            test.GeneralMethods.Navigate_Portal(Navigation.Giving.Contributions.Batches);


            // Click to begin creating a new batch
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Add a new batch"), 20);
            test.Driver.FindElementByLinkText("Add a new batch").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("batch_name"), 20);

            // Attempt to create a batch by entering non-numerical data in the batch amount field
            test.Driver.FindElementById("batch_amount").Clear();
            test.Driver.FindElementById("batch_amount").SendKeys("\\65");
            System.Threading.Thread.Sleep(1000);

            test.Driver.FindElementById("batch_amount").Clear();
            test.Driver.FindElementById("batch_amount").SendKeys("\\83");
            System.Threading.Thread.Sleep(1000);

            test.Driver.FindElementById("batch_amount").Clear();
            test.Driver.FindElementById("batch_amount").SendKeys("\\68");
            System.Threading.Thread.Sleep(1000);

            test.Driver.FindElementById("batch_amount").Clear();
            test.Driver.FindElementById("batch_amount").SendKeys("\\70");
            System.Threading.Thread.Sleep(1000);

            test.Driver.FindElementById("batch_amount").Clear();
            test.Driver.FindElementById("batch_amount").SendKeys("\\65");
            System.Threading.Thread.Sleep(1000);

            // Verify the batch amount field is empty
            Assert.AreEqual(string.Empty, test.Driver.FindElementById("batch_amount").Text, "Batch Amount Field is Not Empty");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }


        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Creates a zero dollar batch.")]
        public void Giving_Contributions_Batches_CreateZeroDollar()
        {
            // Set initial conditions
            string batchName = "Test Batch - Zero Dollar";
            base.SQL.Giving_Batches_Delete(15, batchName, 1);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Create a zero dollar batch
            test.Portal.Giving_Batches_Create_WebDriver(batchName, 0, false, "t");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to view the create batch page using an unlinked user.")]
        public void Giving_Contributions_Batches_Create_Unlinked()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneedenunlinked", "Pa$$w0rd");

            // Attempt to view the create batch page
            test.GeneralMethods.OpenURLExpecting403_WebDriver(string.Format("{0}Payment/Batch/New.aspx", PortalBase.GetPortalURL(this.F1Environment).ToLower()));
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to view the batches landing page w/out the contribution write security right.")]
        public void Giving_Contributions_Batches_NoContributionWrite()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("CR_ContWrite", "Pa$$w0rd");

            // Attempt to open the batches landing page
            test.GeneralMethods.OpenURLExpecting403_WebDriver(string.Format("{0}Payment/Batch/Index.aspx", PortalBase.GetPortalURL(this.F1Environment).ToLower()));
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Creates a Batch.")]
        public void Giving_Contributions_Batches_Create()
        {
            // Set initial conditions
            string batchName = "Test Batch";
            base.SQL.Giving_Batches_Delete(15, batchName, 1);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Create a batch
            test.Portal.Giving_Batches_Create_WebDriver(batchName, 1000, false, "t");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Creates a Batch providing a name containing script tags.")]
        public void Giving_Contributions_Batches_CreateScriptTags()
        {
            // Set initial conditions
            string batchName = "<script>Alert('Hello!!')</script>";
            base.SQL.Giving_Batches_Delete(15, batchName, 1);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Create a batch
            test.Portal.Giving_Batches_Create_WebDriver(batchName, 1000, false, "t");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }



        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create a batch with no data.")]
        public void Giving_Contributions_Batches_CreateNoData()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Create a batch
            test.Portal.Giving_Batches_Create_WebDriver(null, null, false, null);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        #endregion Create

        #endregion All

        #region Credit Card

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to view the credit card batches landing page w/out the contribution write security right.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_NoContributionWrite()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("CR_ContWrite", "Pa$$w0rd");

            // Attempt to open the credit card batches landing page
            test.GeneralMethods.OpenURLExpecting403_WebDriver(string.Format("{0}Payment/Batch/CCBatchIndex.aspx", PortalBase.GetPortalURL(this.F1Environment).ToLower()));
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create a credit card batch with no data.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_CreateNoData()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Create a credit card batch
            test.Portal.Giving_Batches_Create_WebDriver(null, null, true, null);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Creates a credit card batch.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Create()
        {
            // Set initial conditions
            string batchName = "Test Credit Card Batch";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Create a batch
            test.Portal.Giving_Batches_Create_WebDriver(batchName, 1000, true, "t");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        #region New Individual
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the new individual page and verifies the date controls.")]
        public void Giving_Contributions_Batches_CreditCardBatches_Contribution_NewIndividual_DateControls()
        {

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Resume progress on a credit card batch
            test.Portal.Giving_Batches_CreditCardBatches_ResumeProgressWebDriver(this._ccBatchName);

            // Click to navigate to the add household page
            test.Driver.FindElementByLinkText("New individual").Click();

            // Verify the date controls
            test.GeneralMethods.VerifyDateControlWebDriver(test.Driver.FindElementByXPath("//div[@id='individuals']/div/div[2]/table[2]/tbody/tr[2]/td[@class='date_control shrink align_top']/div/input").GetAttribute("id"));
            test.GeneralMethods.VerifyDateControlWebDriver(test.Driver.FindElementByXPath("//div[@id='individuals']/div/div[3]/table/tbody/tr[2]/td[@class='date_control align_top']/div/input").GetAttribute("id"));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the new individual page.")]
        public void Giving_Contributions_Batches_CreditCardBatches_Contribution_NewIndividual()
        {

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Resume progress
            test.Portal.Giving_Batches_CreditCardBatches_ResumeProgressWebDriver(this._ccBatchName);

            // Attempt to create a new individual
            test.Portal.People_AddHousehold(null, null, null, null, null, null);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        #endregion New Individual

        #endregion Credit Card

        #region Batches
        #region Create
        #endregion Create



        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to view the edit batch page using an unlinked user.")]
        public void Giving_Contributions_Batches_Edit_Unlinked()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneedenunlinked", "Pa$$w0rd");

            // Attempt to view the create batch page
            test.GeneralMethods.OpenURLExpecting403_WebDriver(string.Format("{0}Payment/Batch/Edit.aspx", test.Portal.GetPortalURL().ToLower()));


        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to set a general ledger post date with no batches selected.")]
        public void Giving_Contributions_Batches_ApplyGLPostDateNoBatchesSelected()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to giving->batches
            test.GeneralMethods.Navigate_Portal(Navigation.Giving.Contributions.Batches);

            // Attempt to set a gl post date with no batches selected
            test.Driver.FindElementByLinkText("General").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Actions"));
            test.Driver.FindElementByLinkText("Actions").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Apply GL post date to contributions…"));
            //test.Selenium.ClickAndWaitForCondition("link=Apply GL post date to contributions…", JavaScriptMethods.IsElementPresent("gl_post_date"), "10000");
            test.Driver.FindElementByLinkText("Apply GL post date to contributions…").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("gl_post_date"));

            test.Driver.FindElementById("ApplyGLPostDate").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("error_message"));

            // Verify text displayed to user
            //IList<string> errorText = new List<string>();
            //errorText.Add("At least one batch is required to set a GL post date.");
            //test.GeneralMethods.VerifyErrorMessagesWebDriver(errorText);

            test.GeneralMethods.VerifyTextPresentWebDriver("At least one batch is required to set a GL post date.");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the batches page in a church that does not have the batch credit card module enabled.")]
        public void Giving_Contributions_Batches_NonCreditCardModuleChurch()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "QAEUNLX0C1");

            // Navigate to giving->batches
            test.GeneralMethods.Navigate_Portal(Navigation.Giving.Contributions.Batches);

            // Verify the controls for credit card batches are not present
            //test.Selenium.ClickAndWaitForPageToLoad("link=General Batches");
            test.Driver.FindElementByLinkText("General").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Add a new batch"));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//ul[@id='criteria_options']")), "criteria options is not present");

            // Verify credit card checkbox is not present on create batch page
            test.Driver.FindElementByLinkText("Add a new batch").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Cancel"));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//input[@id='is_cc']")));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }


        #endregion Batches

        #region Completed Batch Screen

        #region General
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("F1-3572: Verifies the Completed Batches screen updates the contributor name when the contribution has been updated.")]
        public void Giving_Contributions_Batches_General_CompletedPage_UpdatedContributorName()
        {
            // Setup batch
            string batchName = "General Batch Complete - Update Contributor";
            string individual = "Kevin Spacey";
            string updatedIndividual = "RDC Edit";
            string fund = "Auto Test Fund";
            int amount = 222;
            string reference = new Random().Next(1000).ToString();
            base.SQL.Giving_Batches_Create(15, batchName, amount, 1);

            try
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // Add contribution to batch
                test.Portal.Giving_EnterContributions(batchName, individual, individual, "Check", fund, new string[] { amount.ToString(), reference }, false, false, null);

                // View Completed batch screen
                test.Portal.Giving_Batches_CompletedBatch_View(GeneralEnumerations.BatchTypes.Standard, batchName);

                // Edit a contribution within the batch and change the contributor
                test.Portal.Giving_EditContribution(individual, string.Format("${0}.00", amount), null, updatedIndividual, updatedIndividual, null, null, null);

                // View Completed batch screen again
                test.Portal.Giving_Batches_CompletedBatch_View(GeneralEnumerations.BatchTypes.Standard, batchName);

                // Verify the contributor name was updated
                test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(updatedIndividual));

                // Logout of Portal
                test.Portal.LogoutWebDriver();

            }
            catch (Exception e)
            {
                throw new WebDriverException(e.StackTrace);
            }
            finally
            {
                // Clean up
                base.SQL.Giving_Batches_Delete(15, batchName, 1);
            }

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3572: Verifies the Completed Batches screen updates the fund name when the contribution has been updated.")]
        public void Giving_Contributions_Batches_General_CompletedPage_UpdatedFundName()
        {
            // Setup batch
            string batchName = "General Batch Complete - Update Fund Name";
            string individual = "RDC Edit";
            string fund = "Auto Test Fund";
            string updatedFund = "1 - General Fund";
            int amount = 225;
            string reference = new Random().Next(1000).ToString();
            base.SQL.Giving_Batches_Create(15, batchName, amount, 1);

            try
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // Add contribution to batch
                test.Portal.Giving_EnterContributions(batchName, individual, individual, "Check", fund, new string[] { amount.ToString(), reference }, false, false, null);

                // View Completed batch screen
                test.Portal.Giving_Batches_CompletedBatch_View(GeneralEnumerations.BatchTypes.Standard, batchName);

                // Edit a contribution within the batch and change the contributor
                test.Portal.Giving_EditContribution(individual, string.Format("${0}.00", amount), null, null, null, updatedFund, null, null);

                // View Completed batch screen again
                test.Portal.Giving_Batches_CompletedBatch_View(GeneralEnumerations.BatchTypes.Standard, batchName);

                // Verify the contributor name was updated
                test.GeneralMethods.VerifyTextPresentWebDriver(updatedFund);

                // Logout of Portal
                test.Portal.LogoutWebDriver();

            }
            catch (Exception e)
            {
                throw new WebDriverException(e.StackTrace);
            }
            finally
            {
                // Clean up
                base.SQL.Giving_Batches_Delete(15, batchName, 1);
            }

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3572: Verifies the Completed Batches screen updates the amount when the contribution has been updated.")]
        public void Giving_Contributions_Batches_General_CompletedPage_UpdatedAmount()
        {
            // Setup batch
            string batchName = "General Batch Complete - Update Amount";
            string individual = "RDC Edit";
            string fund = "Auto Test Fund";
            int amount = 525;
            int updatedAmount = 37;
            string reference = new Random().Next(1000).ToString();
            base.SQL.Giving_Batches_Create(15, batchName, amount, 1);

            try
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // Add contribution to batch
                test.Portal.Giving_EnterContributions(batchName, individual, individual, "Check", fund, new string[] { amount.ToString(), reference }, false, false, null);

                // View Completed batch screen
                test.Portal.Giving_Batches_CompletedBatch_View(GeneralEnumerations.BatchTypes.Standard, batchName);

                // Edit a contribution within the batch and change the contributor
                test.Portal.Giving_EditContribution(individual, string.Format("${0}.00", amount), null, null, null, null, null, updatedAmount.ToString());

                // View Completed batch screen again
                test.Portal.Giving_Batches_CompletedBatch_View(GeneralEnumerations.BatchTypes.Standard, batchName);

                // Verify the contributor name was updated
                test.GeneralMethods.VerifyTextPresentWebDriver(string.Format("${0}", updatedAmount));

                // Logout of Portal
                test.Portal.LogoutWebDriver();

            }
            catch (Exception e)
            {
                throw new WebDriverException(e.StackTrace);
            }
            finally
            {
                // Clean up
                base.SQL.Giving_Batches_Delete(15, batchName, 1);
            }

        }

        #endregion General

        #region Scanned
        [Test, RepeatOnFailure, Timeout(1000)]
        [Author("David Martin")]
        [Description("F1-3572: Verifies the Completed Batches screen updates the contributor name when the contribution has been updated.")]
        public void Giving_Contributions_Batches_Scanned_CompletedPage_UpdatedContributorName()
        {
            // Setup batch
            string batchName = "Scanned Batch Complete - Update Contributor";
            string individual = "Kevin Spacey";
            string updatedIndividual = "RDC Edit";
            string fund = "Auto Test Fund";
            int amount = 121;
            string account = "717828939";
            string routing = "111000025";
            string reference = new Random().Next(1000).ToString();
            base.SQL.Giving_Batches_Create_Scanned(15, batchName, true, new System.Collections.Generic.List<double>() { amount }, new System.Collections.Generic.List<string> { individual },
                new System.Collections.Generic.List<string> { account },
                new System.Collections.Generic.List<string> { routing }, false, fund, false);

            try
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // View Completed batch screen
                test.Portal.Giving_Batches_CompletedBatch_View(GeneralEnumerations.BatchTypes.Scanned, batchName);

                // Edit a contribution within the batch and change the contributor
                test.Portal.Giving_EditContribution(individual, string.Format("${0}.00", amount), null, updatedIndividual, updatedIndividual, null, null, null);

                // View Completed batch screen again
                test.Portal.Giving_Batches_CompletedBatch_View(GeneralEnumerations.BatchTypes.Scanned, batchName);

                // Verify the contributor name was updated
                test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(updatedIndividual));

                // Logout of Portal
                test.Portal.LogoutWebDriver();

            }
            catch (Exception e)
            {
                throw new WebDriverException(e.StackTrace);
            }
            finally
            {
                // Clean up
                base.SQL.Giving_Batches_Delete(15, batchName, 4);
            }

        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("F1-3572: Verifies the Completed Batches screen updates the fund name when the contribution has been updated.")]
        public void Giving_Contributions_Batches_Scanned_CompletedPage_UpdatedFundName()
        {
            // Setup batch
            string batchName = "Scanned Batch Complete - Update Fund Name";
            string individual = "RDC Edit";
            string fund = "Auto Test Fund";
            string updatedFund = "1 - General Fund";
            int amount = 131;
            string account = "676868747";
            string routing = "111000025";
            string reference = new Random().Next(1000).ToString();
            base.SQL.Giving_Batches_Create_Scanned(15, batchName, true, new System.Collections.Generic.List<double>() { amount }, new System.Collections.Generic.List<string> { individual },
                new System.Collections.Generic.List<string> { account },
                new System.Collections.Generic.List<string> { routing }, false, fund, false);

            try
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // View Completed batch screen
                test.Portal.Giving_Batches_CompletedBatch_View(GeneralEnumerations.BatchTypes.Scanned, batchName);

                // Edit a contribution within the batch and change the contributor
                test.Portal.Giving_EditContribution(individual, string.Format("${0}.00", amount), null, null, individual, updatedFund, null, null);

                // View Completed batch screen again
                test.Portal.Giving_Batches_CompletedBatch_View(GeneralEnumerations.BatchTypes.Scanned, batchName);

                // Verify the fund name was updated
                test.GeneralMethods.VerifyTextPresentWebDriver(updatedFund);

                // Logout of Portal
                test.Portal.LogoutWebDriver();

            }
            catch (Exception e)
            {
                throw new WebDriverException(e.StackTrace);
            }
            finally
            {
                // Clean up
                base.SQL.Giving_Batches_Delete(15, batchName, 4);
            }

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3572: Verifies the Completed Batches screen updates the amount when the contribution has been updated.")]
        public void Giving_Contributions_Batches_Scanned_CompletedPage_UpdatedAmount()
        {
            // Setup batch
            string batchName = "Scanned Batch Complete - Update Amount";
            string individual = "RDC Edit";
            string fund = "Auto Test Fund";
            int amount = 951;
            int updatedAmount = 78;
            string account = "131242353";
            string routing = "111000025";
            string reference = new Random().Next(1000).ToString();
            base.SQL.Giving_Batches_Create_Scanned(15, batchName, true, new System.Collections.Generic.List<double>() { amount }, new System.Collections.Generic.List<string> { individual },
                new System.Collections.Generic.List<string> { account },
                new System.Collections.Generic.List<string> { routing }, false, fund, false);

            try
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // View Completed batch screen
                test.Portal.Giving_Batches_CompletedBatch_View(GeneralEnumerations.BatchTypes.Scanned, batchName);

                // Edit a contribution within the batch and change the amount
                test.Portal.Giving_EditContribution(individual, string.Format("${0}.00", amount), null, null, individual, null, null, updatedAmount.ToString());

                // View Completed batch screen again
                test.Portal.Giving_Batches_CompletedBatch_View(GeneralEnumerations.BatchTypes.Scanned, batchName);

                // Verify the amount was updated
                test.GeneralMethods.VerifyTextPresentWebDriver(string.Format("${0}", updatedAmount));

                //Added for F1-4057
                //Verify Amount updated on batch screen
                //Navigate to batch screen
                test.GeneralMethods.Navigate_Portal(Navigation.Giving.Contributions.Batches);
                test.GeneralMethods.WaitForElement(By.LinkText("Add a new batch"));
                //Click on Scanned batch
                test.Driver.FindElementByLinkText("Scanned").Click();
                test.GeneralMethods.VerifyTextPresentWebDriver("Scanned Batches");
                //Find Batch and verify the amount has been updated
                IWebElement batchTable = test.Driver.FindElementByXPath(TableIds.Giving_Batches);
                int row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName, "Name");
                string batchTotal = String.Format("${0}.00", updatedAmount);
                Assert.AreEqual(batchName, batchTable.FindElements(By.TagName("tr"))[row].FindElements(By.TagName("td"))[0].Text, "Batch Name Not Found");
                Assert.AreEqual(batchTotal, batchTable.FindElements(By.TagName("tr"))[row].FindElements(By.TagName("td"))[3].Text, "Batch amount not updated");

                // Logout of Portal
                test.Portal.LogoutWebDriver();

            }
            catch (Exception e)
            {
                throw new WebDriverException(e.StackTrace);
            }
            finally
            {
                // Clean up
                base.SQL.Giving_Batches_Delete(15, batchName, 4);
            }

        }

        #endregion Scanned



        #endregion Completed Batch Screen

        #region Credit Card Batches


        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to view the audit batch page referencing a batch id from a different church.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_AuditDifferentChurch()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Attempt to view the audit batch page referencing a batch from another church           
            test.GeneralMethods.OpenURLWebDriver(string.Format("{0}Payment/Batch/Audit.aspx?ID={1}", test.Portal.GetPortalURL(), _nonDCBatchID));

            // Verify user is not taken to the edit page
            Assert.AreEqual("Audit Batch", test.Driver.FindElementByXPath("//span[@class='float_left']").Text);
            test.GeneralMethods.VerifyTextPresentWebDriver("You attempted to access an invalid batch.");
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.LinkText("Download PDF"));
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.LinkText("Download Excel"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to view the add contributions page referencing a batch id from a different church.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_AddContributionsDifferentChurch()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Attempt to view the add contributions page referencing a batch from another church            
            test.GeneralMethods.OpenURLExpecting500WebDriver(string.Format("{0}Payment/BatchContribution/New.aspx?batchID={1}", test.Portal.GetPortalURL(), _nonDCBatchID));

            // Verify user is not taken to the add contributions page
            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver("Add Contributions"), "User was taken to the add contributions page for a batch in a different church!");
        }

        #region Edit Batch Amount
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the edit batch amount page.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_EditBatchAmount()
        {
            // Set initial conditions
            int churchId = 15;
            int batchTypeId = 2;
            double batchAmount = 100.00;
            string batchName = "Edit Batch Amount";
            string fundName = "1 - General Fund";
            base.SQL.Giving_Batches_Delete(churchId, batchName, batchTypeId);
            base.SQL.Giving_Batches_Create(churchId, batchName, batchAmount, batchTypeId);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            //Add Contribution to Check batch

            if (base.SQL.IsAMSEnabled(15))
            {
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "12", string.Format("{0:yyyy}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))), "100", "1 - General Fund", null, null);

                //Suchitra Notes --Adding this return lik logic since the was not returning to Creditcard batches page
                // Navigate to giving->batches->credit card batches            
                test.Driver.FindElementByLinkText(GeneralLinksWebDriver.RETURN).Click();

            }
            else
            {

                base.SQL.Giving_Batches_AddContributionToCreditCardBatch(churchId, batchName, fundName, batchAmount, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1));

            }



            // Resume progress
            test.Portal.Giving_Batches_CreditCardBatches_ResumeProgressWebDriver(batchName);

            // View the edit batch amount page
            test.Driver.FindElementByXPath("//a[contains(@href, '/Payment/BatchContribution/EditBatchAmount.aspx?ID=')]").Click();

            // Verify page title, header
            Assert.IsTrue(test.Driver.Title.Equals("Fellowship One :: Edit Batch Amount"));
            test.GeneralMethods.VerifyTextPresentWebDriver("Edit Batch Amount");

            // Verify the text for the amount field
            Assert.AreEqual("Enter new amount *", test.Driver.FindElementByXPath("//label[@for='batch_amount_new']").Text);

            // Verify the text above the comment box
            Assert.AreEqual("Optional – Please explain why you are changing the amount.", test.Driver.FindElementByXPath("//label[@for='batch_amount_comment']").Text);

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to update the batch amount without providing a value.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_EditBatchAmount_NoData()
        {
            // Set initial conditions
            int churchId = 15;
            int batchTypeId = 2;
            double batchAmount = 100.00;
            string batchName = "Edit Batch Amount: No Data";
            string fundName = "1 - General Fund";
            base.SQL.Giving_Batches_Delete(churchId, batchName, batchTypeId);
            base.SQL.Giving_Batches_Create(churchId, batchName, batchAmount, batchTypeId);


            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            //Add Contribution to CRedit card Batch

            if (base.SQL.IsAMSEnabled(15))
            {

                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "12", string.Format("{0:yyyy}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))), "100", "1 - General Fund", null, null);

                //Suchitra Notes --Adding this return lik logic since the was not returning to Creditcard batches page
                // Navigate to giving->batches->credit card batches            
                //test.Driver.FindElementByLinkText(GeneralLinksWebDriver.RETURN).Click();

            }
            else
            {

                base.SQL.Giving_Batches_AddContributionToCreditCardBatch(churchId, batchName, fundName, batchAmount, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1));
                //Resume processing the batch created with SQL
                test.Portal.Giving_Batches_CreditCardBatches_ResumeProgressWebDriver(batchName);
            }





            // Attempt to edit the batch amount without a batch amount
            test.Portal.Giving_Batches_CreditCardBatches_EditBatchAmount_WebDriver(batchName, null, null);

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to update the batch amount, providing a comment greater than 300 characters.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_EditBatchAmount_CommentExceeding300Characters()
        {
            // Set initial conditions
            int churchId = 15;
            int batchTypeId = 2;
            double batchAmount = 100.00;
            string batchName = "Edit Batch Amount: Comment";
            string fundName = "1 - General Fund";
            base.SQL.Giving_Batches_Delete(churchId, batchName, batchTypeId);
            base.SQL.Giving_Batches_Create(churchId, batchName, batchAmount, batchTypeId);


            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            //Add Contribution to Credit card Batch
            if (base.SQL.IsAMSEnabled(15))
            {
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "12", string.Format("{0:yyyy}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))), "100", "1 - General Fund", null, null);

                //Suchitra Notes --Adding this return lik logic since the was not returning to Creditcard batches page
                // Navigate to giving->batches->credit card batches            
                // test.Driver.FindElementByLinkText(GeneralLinksWebDriver.RETURN).Click();

            }
            else
            {

                base.SQL.Giving_Batches_AddContributionToCreditCardBatch(churchId, batchName, fundName, batchAmount, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1));
                //Resume processing the batch created with SQL
                test.Portal.Giving_Batches_CreditCardBatches_ResumeProgressWebDriver(batchName);
            }




            // Attempt to edit the batch amount with a large comment
            test.Portal.Giving_Batches_CreditCardBatches_EditBatchAmount_WebDriver(batchName, null, "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to view the edit batch amount page referencing a batch id from a different church.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_EditBatchAmount_DifferentChurch()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Attempt to view the edit batch amount page referencing a batch from another church
            test.GeneralMethods.OpenURLExpecting500WebDriver(string.Format("{0}Payment/BatchContribution/EditBatchAmount.aspx?ID={1}", test.Portal.GetPortalURL(), _nonDCBatchID));

            // Verify user is not taken to the edit batch amount page
            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver("Edit Batch Amount"), "User was taken to the add contributions page for a batch in a different church!");
            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver("Enter new amount *"), "User was taken to the add contributions page for a batch in a different church!");
        }
        #endregion Edit Batch Amount

        #region Create


        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create a credit card batch with an invalid received date.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_CreateInvalidDate()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Attempt to create a credit card batch with an invalid received date
            test.Portal.Giving_Batches_Create_WebDriver("Test credit card batch - invalid date", 100, true, "151515");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Create

        #region Edit Batch
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Updates a credit card batch.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Update()
        {
            // Set initial conditions
            string batchName = "Test Credit Card Batch - U";
            string batchNameUpdated = "Test Credit Card Batch - Updated";
            string fundName = "1 - General Fund";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Delete(15, batchNameUpdated, 2);
            base.SQL.Giving_Batches_Create(15, batchName, 1000, 2);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Update a batch Name
            test.Portal.Giving_Batches_Update_WebDriver(batchName, 1000, true, batchNameUpdated);

            //Add Contribution to Credit card Batch
            if (base.SQL.IsAMSEnabled(15))
            {
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchNameUpdated, "Matthew Sneeden", "Visa", "4111111111111111", "12", string.Format("{0:yyyy}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))), "1000", "1 - General Fund", null, null);
            }
            else
            {

                base.SQL.Giving_Batches_AddContributionToCreditCardBatch(15, batchNameUpdated, fundName, 1000, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1));
                //Resume processing the batch created with SQL
                test.Portal.Giving_Batches_CreditCardBatches_ResumeProgressWebDriver(batchNameUpdated);
            }

            // Update a batch Amount
            test.Portal.Giving_Batches_CreditCardBatches_EditBatchAmount_WebDriver(batchNameUpdated, 900, null);


            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the edit batch page for a credit card batch then returns to the landing page.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Update_RETURN()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to giving->batches
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Batches_CreditCardBatches);

            // View the edit batch page
            test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[2]/td[5]/a", TableIds.Giving_Batches)).Click();
            test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[2]/td[5]/ul/li[*]/a/span[text()='Edit batch']", TableIds.Giving_Batches)).Click();

            // Return to the credit card batches landing page
            test.Driver.FindElementByLinkText(GeneralLinksWebDriver.RETURN).Click();
            Assert.IsTrue(test.Driver.Title.Equals("Fellowship One :: Credit Card Batches"));
            test.GeneralMethods.VerifyTextPresentWebDriver("Credit Card Batches");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to view the edit batch page referencing a batch id from a different church.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Edit_DifferentChurch()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Attempt to view the edit page referencing a batch from another church

            test.GeneralMethods.OpenURLExpecting500WebDriver(string.Format("{0}/Payment/Batch/Edit.aspx?ID={1}", test.Portal.GetPortalURL(), _nonDCBatchID));

            // Verify user is not taken to the edit page
            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver("Batch name *"), "User was taken to the edit page for a batch in a different church!");
            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver("Batch amount *"), "User was taken to the edit page for a batch in a different church!");
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the edit batch page for a settled credit card batch that does not have an activity instance associated to it.")]
        [Category(TestCategories.Services.PaymentProcessor), Category(TestCategories.Services.SmokeTest)]
        public void Giving_Contributions_Batches_CreditCardBatches_Update_SettledNoActivityInstance()
        {

            // Set initial conditions
            string batchName = "Settled: No Activity";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);


            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            //Add Contribution to Credit Card Batch
            if (base.SQL.IsAMSEnabled(15))
            {
                //batch creation
                base.SQL.Giving_Batches_Create(15, batchName, 100, 2);
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "12", string.Format("{0:yyyy}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))), "100", "1 - General Fund", null, null);
                // Authorize the batch
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "100");

            }
            else
            {

                //Batch creation
                base.SQL.Giving_Batches_Create(15, batchName, 100, 2);
                //Add Contribution to CC
                this.SQL.Giving_Batches_AddContributionToCreditCardBatch(15, batchName, "1 - General Fund", 100, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1));
                //Authorize the Batch
                this.SQL.Giving_Batches_Authorize(15, batchName, this.SQL.People_Individuals_FetchID(15, "Matthew Sneeden"));
            }

            // Settle the batch
            test.Portal.Giving_Batches_CreditCardBatches_Settle_WebDriver(batchName, false, true);

            // Navigate to giving->batches
            test.Driver.FindElementByLinkText(GeneralLinksWebDriver.RETURN).Click();

            // View the edit batch page
            test.GeneralMethods.SelectOptionFromGearWebDriver(Convert.ToInt16(test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName, "Name")), "Edit batch");
            //test.GeneralMethods.SelectOptionFromGear(Convert.ToInt16(test.GeneralMethods.GetTableRowNumber(TableIds.Portal.Giving_Batches, "Settled", "Name")), "Edit batch");
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Cancel"));

            // Verify the activity time label is not present
            Assert.IsFalse(test.Driver.FindElementById("choose_an_activity_time").Displayed, "Activity time was displayed");

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }
        #endregion Edit Batch


        #region Delete Batch
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Deletes a credit card batch.")]
        [Category(TestCategories.Services.PaymentProcessor), Category(TestCategories.Services.SmokeTest)]
        public void Giving_Contributions_Batches_CreditCardBatches_Delete()
        {

            // Set initial conditions
            string batchName = "Test Credit Card Batch - Delete";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, 900, 2);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();
            try
            {
                // Delete a credit card batch
                test.Portal.Giving_Batches_CreditCardBatches_Delete_WebDriver(batchName);
            }
            finally
            {
                base.SQL.Giving_Batches_Delete(15, batchName, 2);

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }

        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Deletes a credit card batch containing an ' in the name.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Delete_Aposthrophe()
        {
            // Set initial conditions
            string batchName = "'";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            try
            {
                // Create a credit card batch
                test.Portal.Giving_Batches_Create_WebDriver(batchName, 100, true, "t");
            }
            finally
            {
                // Delete the credit card batch
                test.Portal.Giving_Batches_CreditCardBatches_Delete_WebDriver(batchName);

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Deletes a credit card batch from a search subset.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Delete_Subset()
        {
            // Set initial conditions
            string batchName = "Delete Subset";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            try
            {
                // Create a credit card batch
                test.Portal.Giving_Batches_Create_WebDriver(batchName, 100, true, "t");

                // Search for the batch
                test.Portal.Giving_Batches_Search_WebDriver(batchName, null, null, null, null);
            }
            finally
            {
                // Delete the batch
                test.Portal.Giving_Batches_CreditCardBatches_Delete_WebDriver(batchName);

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
        }
        #endregion Delete Batch

        #region View Batch
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user is taken to the correct page via 'View batch' for a CC Batch")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_ViewBatch()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to giving->batches
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Batches_CreditCardBatches);

            // Click the gear associated with a non credit card batch

            //string batchName = test.Selenium.GetTable(string.Format("{0}.1.0", TableIds.Giving_Batches));
            test.GeneralMethods.WaitForElement(By.Id(TableIds.Portal.Giving_Batches));
            IWebElement table = test.Driver.FindElementById(TableIds.Portal.Giving_Batches);
            string batchName = table.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[0].Text;

            TestLog.WriteLine(string.Format("Go to {0}", batchName));
            test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[2]/td[5]/a", TableIds.Giving_Batches)).Click();
            test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[2]/td[5]/ul/li[2]/a/span[text()='View batch']", TableIds.Giving_Batches)).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("ctl00_ctl00_MainContent_content_txtBatchName_textBox"));

            // Verify title, text
            Assert.AreEqual(test.Driver.Title, "Fellowship One :: Contribution Search", "Title does not match");
            test.GeneralMethods.VerifyTextPresentWebDriver("Contribution Search");

            // Verify the batch name
            Assert.AreEqual(batchName, test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtBatchName_textBox").GetAttribute("value"), "Batch Name is incorrect");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion View Batch

        #region Resume Progress
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Resumes progress while at the contribution phase.")]
        [Category(TestCategories.Services.PaymentProcessor), Category(TestCategories.Services.SmokeTest)]
        public void Giving_Contributions_Batches_CreditCardBatches_ResumeProgress_Contribution()
        {
            // Set initial conditions
            string batchName = "Contribution";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, 100, 2);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Resume progress
            test.Portal.Giving_Batches_CreditCardBatches_ResumeProgressWebDriver(batchName);

            // Verify page title, header            
            Assert.AreEqual("Fellowship One :: Add Contributions", test.Driver.Title, "Tittle Verify Error");
            test.GeneralMethods.VerifyTextPresentWebDriver("Add Contributions");

            // Verify the wizard
            string commonXPath = "//table[@id='step_wizard']/tbody/tr/";
            Assert.AreEqual("Add Contributions", test.Driver.FindElementByXPath(string.Format("{0}td[1]/div/strong", commonXPath)).Text);
            Assert.AreEqual("Add or edit the contributions for this batch.", test.Driver.FindElementByXPath(string.Format("{0}td[1]/div/small", commonXPath)).Text);

            //For AMS we don't have this step in wizard any more
            if (!test.SQL.IsAMSEnabled(test.Portal.ChurchID))
            {
                Assert.AreEqual("Batch Authorization", test.Driver.FindElementByXPath(string.Format("{0}td[2]/div/strong", commonXPath)).Text);
                Assert.AreEqual("Verify availability of funds.", test.Driver.FindElementByXPath(string.Format("{0}td[2]/div/small", commonXPath)).Text);

                Assert.AreEqual("Batch Settlement", test.Driver.FindElementByXPath(string.Format("{0}td[3]/div/strong", commonXPath)).Text);
                Assert.AreEqual("Finalize the transaction.", test.Driver.FindElementByXPath(string.Format("{0}td[3]/div/small", commonXPath)).Text);
            }


            // Search for, select the individual
            test.Portal.Giving_SearchIndividual_WebDriver("Matthew Sneeden", null);


            // Verify the available funds are sorted
            DataTable funds = base.SQL.Giving_Funds_FetchNames(15, true, null, true);
            for (int i = 0; i < funds.Rows.Count; i++)
            {
                //TestLog.WriteLine(string.Format("FUND DB: {0}", funds.Rows[i]["FUND_NAME"].ToString()));
                //TestLog.WriteLine(string.Format("FUND   : {0}", new SelectElement(test.Driver.FindElementById("ddlFund_1")).Options.ElementAt(i + 1).Text));
                Assert.AreEqual(Regex.Replace(funds.Rows[i]["FUND_NAME"].ToString(), @"\s+", " "), new SelectElement(test.Driver.FindElementById("ddlFund_1")).Options.ElementAt(i + 1).Text);
                //Assert.AreEqual(Regex.Replace(funds.Rows[i]["FUND_NAME"].ToString(), @"\s+", " "), test.Driver.FindElementByXPath(string.Format("//select[@id='ddlFund_1']/option[{0}]", i + 2)).Text);
            }

            // Click the return button
            test.Driver.FindElementByLinkText(GeneralLinksWebDriver.RETURN).Click();

            // Verify title, text
            Assert.AreEqual(test.Driver.Title, "Fellowship One :: Credit Card Batches");
            test.GeneralMethods.VerifyTextPresentWebDriver("Credit Card Batches");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Resumes progress while at the authorization phase.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_ResumeProgress_Authorization()
        {
            // Set initial conditions
            string batchName = "Authorization";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Create a credit card batch
            test.Portal.Giving_Batches_Create_WebDriver(batchName, 100, true, "t");

            // Add a contribution
            test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "12", string.Format("{0:yyyy}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))), "100", "1 - General Fund", null, null);

            // Authorize the batch
            test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, null);

            // Return to the landing page
            test.Driver.FindElementByLinkText(GeneralLinksWebDriver.RETURN).Click();

            // Verify title, text
            Assert.AreEqual(test.Driver.Title, "Fellowship One :: Credit Card Batches");
            test.GeneralMethods.VerifyTextPresentWebDriver("Credit Card Batches");

            // Resume progress
            test.Portal.Giving_Batches_CreditCardBatches_ResumeProgressWebDriver(batchName);

            // Verify page title, header
            if (test.SQL.IsAMSEnabled(test.Portal.ChurchID))
            {
                Assert.AreEqual("Fellowship One :: Batch Settlement", test.Driver.Title, "Title Verify Error");
                test.GeneralMethods.VerifyTextPresentWebDriver("Batch Settlement");

            }
            else
            {
                Assert.AreEqual("Fellowship One :: Batch Authorization", test.Driver.Title, "Title Verify Error");
                test.GeneralMethods.VerifyTextPresentWebDriver("Batch Authorization");

            }

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Resumes progress while at the settled phase.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_ResumeProgress_Settled()
        {
            // Set initial conditions
            string batchName = "Settled";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Create a credit card batch
            test.Portal.Giving_Batches_Create_WebDriver(batchName, 100, true, "t");

            // Add a contribution
            test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "12", DateTime.Now.Year.ToString(), "100", "1 - General Fund", null, null);

            // Authorize the batch
            test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, null);

            // Settle the batch
            test.Portal.Giving_Batches_CreditCardBatches_Settle_WebDriver(batchName);

            // Return to the landing page
            test.Driver.FindElementByLinkText(GeneralLinksWebDriver.RETURN).Click();

            // Verify title, text
            Assert.AreEqual(test.Driver.Title, "Fellowship One :: Credit Card Batches");
            test.GeneralMethods.VerifyTextPresentWebDriver("Credit Card Batches");

            // Resume progress
            test.Portal.Giving_Batches_CreditCardBatches_ResumeProgressWebDriver(batchName);

            // Verify page title, header
            Assert.AreEqual("Fellowship One :: Batch Settlement", test.Driver.Title);
            test.GeneralMethods.VerifyTextPresentWebDriver("Batch Settlement");

            // Logout of portal
            test.Portal.LogoutWebDriver();


            // Ping the database to verify the data associated with the payment
            //SELECT TOP 1 MerchantReferenceCode, ClientApplication FROM Payment WHERE ChurchID = 15 AND ProcessedDate IS NOT NULL AND MerchantReferenceCode = 'Settled' AND ClientApplication = 'Batch Contribution' ORDER BY CreatedDate DESC
        }
        #endregion Resume Progress



        #region Authorization
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the available gear options for a credit card batch in the authorization state.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Authorization_Gear()
        {
            // Set initial conditions
            string batchName = "Gear: Authorization";
            this.SQL.Giving_Batches_Delete(15, batchName, 2);
            this.SQL.Giving_Batches_Create(15, batchName, 100, 2, 2);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to giving->batches->credit card batches
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Batches_CreditCardBatches);

            // Verify the gear options for a batch in the authorization state
            int itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName, "Name");

            test.Driver.FindElementByXPath(string.Format("//table[*]/tbody/tr[{0}]/td[*]/a[@class='gear_trigger']", itemRow + 1)).Click();

            Assert.AreEqual(3, test.Driver.FindElements(By.XPath(string.Format("{0}/tbody/tr[{1}]/td[5]/ul/li", TableIds.Giving_Batches, itemRow + 1))).Count);
            Assert.AreEqual("Resume progress", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]/ul/li[1]/a/span", TableIds.Giving_Batches, itemRow + 1)).Text);
            Assert.AreEqual("View batch", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]/ul/li[2]/a/span", TableIds.Giving_Batches, itemRow + 1)).Text);
            Assert.AreEqual("Edit batch", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]/ul/li[3]/a/span", TableIds.Giving_Batches, itemRow + 1)).Text);

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user can add, authorize a transaction using a valid credit card number containing dashes.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Authorization_NumberWithDashes()
        {
            // Set initial conditions
            string batchName = "Number with dashes";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, 100, 2);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Add a contribution to the batch
            test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111-1111-1111-1111", "01", "2020", "100", "1 - General Fund", null, null);

            // Authorize the batch
            test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, null);

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user can add, authorize a transaction using a valid credit card number containing spaces.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Authorization_NumberWithSpaces()
        {
            // Set initial conditions
            string batchName = "Number with spaces";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, 100, 2);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Add a contribution to the batch
            test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111 1111 1111 1111", "01", "2020", "100", "1 - General Fund", null, null);

            // Authorize the batch
            test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, null);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies designation information when the user authorizes a batch with a fund, sub fund, and pledge drive")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Authorization_Designation()
        {
            // Set initial conditions
            string batchName = "Designation";
            string fund = "Auto Test Fund w Sub Fund";
            string subFund = "Auto Test Sub Fund";
            string pledgeDrive = "Auto Test Pledge Drive 2";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, 2402, 2);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Add a contribution to the batch
            test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "01", "2020", "2402.00", fund, subFund, pledgeDrive);
            if (base.SQL.IsAMSEnabled(15))
            {

                // Authorize the batch
                //Changing Return to "233" to accommidate AMS changes
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "233");
            }
            else
            {
                // Authorize the batch
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "201");
            }
            // Verify the designation information
            IWebElement table = test.Driver.FindElementByXPath("//table[@class='extra_info']");
            Assert.AreEqual(fund, table.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[0].Text);
            Assert.AreEqual(subFund, table.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[1].Text);
            Assert.AreEqual(pledgeDrive, table.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[2].Text);

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Tests authorization response code 100 - Successful transaction")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Authorization_RC_100()
        {
            // Set initial conditions
            string batchName = "RC - 100";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Create a credit card batch
            test.Portal.Giving_Batches_Create_WebDriver(batchName, 100, true, "t");

            // Add a contribution to the batch
            test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "01", "2020", "100", "1 - General Fund", null, null);

            // Authorize the batch
            test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, null);

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        // Ignore("Invalid Test Case")
        // [Test]
        [Author("Matthew Sneeden")]
        [Description("Tests authorization response code 150 - General system failure")]
        public void Giving_Contributions_Batches_CreditCardBatches_Authorization_RC_150()
        {
            // Set initial conditions
            string batchName = "RC - 150";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, 2000, 2);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            //Add Contribution to Credit Card Batch

            if (base.SQL.IsAMSEnabled(15))
            {
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "01", "2020", "2000", "1 - General Fund", null, null);
                // Authorize the batch
                //RC code for AMS - 233
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "233");
            }
            else
            {
                base.SQL.Giving_Batches_AddContributionToCreditCardBatch(15, batchName, "1 - General Fund", 2000.00, new DateTime(2020, 12, 1));
                // Authorize the batch
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "150");
            }




            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Tests authorization response code 201 - The issuing bank has questions...")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Authorization_RC_201()
        {
            // Set initial conditions
            string batchName = "RC - 201";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, 2402, 2);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            //Add Contribution to Credit Card Batch
            if (base.SQL.IsAMSEnabled(15))
            {
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "01", "2020", "2402", "1 - General Fund", null, null);
                // Authorize the batch
                //RC code for AMS - 233
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "233");
            }
            else
            {
                base.SQL.Giving_Batches_AddContributionToCreditCardBatch(15, batchName, "1 - General Fund", 2402.00, new DateTime(2020, 12, 1));
                // Authorize the batch
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "201");
            }




            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        // Commenting this test case out until we figure out if this test case is still valid.....this always comes back as 150 error instead of 203.
        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Tests authorization response code 203 - General decline of the card")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Authorization_RC_203()
        {
            // Set initial conditions
            string batchName = "RC - 203";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, 2260, 2);


            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            //Add Contribution to Credit Card Batch
            if (base.SQL.IsAMSEnabled(15))
            {
                //  test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4012888888881881", "01", "2020", "2260", "1 - General Fund", null, null);
                // Authorize the batch
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "01", "2020", "2260", "1 - General Fund", null, null);
                // Authorize the batch
                //AMS Reason code passed as 231
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "231");

            }
            else
            {
                base.SQL.Giving_Batches_AddContributionToCreditCardBatch(15, batchName, "Auto Test Fund", 2260.00, new DateTime(2020, 12, 1));
                // Authorize the batch
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "203");
            }




            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Tests authorization response code 204 - Insufficient funds in the account.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Authorization_RC_204()
        {
            // Set initial conditions
            string batchName = "RC - 204";
            string fund = "AMS error test fund";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, 2521, 2);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            //Add Contribution to Credit Card Batch
            if (base.SQL.IsAMSEnabled(15))
            {
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "01", "2020", "2521", fund, null, null);
            }
            else
            {
                base.SQL.Giving_Batches_AddContributionToCreditCardBatch(15, batchName, fund, 2521.00, new DateTime(2020, 12, 1));
            }

            // Authorize the batch
            test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "204");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Tests authorization response code 205 - Stolen or lost card.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Authorization_RC_205()
        {
            // Set initial conditions
            string batchName = "RC - 205";
            string fund = "AMS error test fund";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, 2501, 2);


            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            //Add Contribution to Credit Card Batch
            if (base.SQL.IsAMSEnabled(15))
            {
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "01", "2020", "2501", fund, null, null);
                // Authorize the batch
                //AMS Reason code passed as 231
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "231");
            }
            else
            {
                base.SQL.Giving_Batches_AddContributionToCreditCardBatch(15, batchName, fund, 2501.00, new DateTime(2020, 12, 1));
                // Authorize the batch
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "205");

            }

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Tests authorization response code 208 - Inactive card or card not authorized...")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Authorization_RC_208()
        {
            // Set initial conditions
            string batchName = "RC - 208";
            string fund = "AMS error test fund";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, 2606, 2);


            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            //Add Contribution to Credit Card Batch
            if (base.SQL.IsAMSEnabled(15))
            {
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "01", "2020", "2606", fund, null, null);
                // Authorize the batch
                //AMS Reason code passed as 231
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "231");
            }
            else
            {
                base.SQL.Giving_Batches_AddContributionToCreditCardBatch(15, batchName, fund, 2606.00, new DateTime(2020, 12, 1));
                // Authorize the batch
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "208");
            }



            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Tests authorization response code 209 - American Express Card Identification...")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Authorization_RC_209()
        {
            // Set initial conditions
            string batchName = "RC - 209";
            string fund = "AMS error test fund";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, 2811, 2);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();
            if (base.SQL.IsAMSEnabled(15))
            {
                // Add a contribution to the batch
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "American Express", "378282246310005", "12", "2020", "2811", fund, null, null);
                // Authorize the batch
                //AMS Reason code passed as 231
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "231");

            }
            else
            {
                // Add a contribution to the batch
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "American Express", "378282246310005", "12", "2020", "2811", fund, null, null);
                // Authorize the batch
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "209");

            }

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Tests authorization response code 211 - Invalid card verification number.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Authorization_RC_211()
        {
            // Set initial conditions
            string batchName = "RC - 211";
            string amountString = " ";
            int amountInt = 0;
            double amountDbl = 0.00;
            string fundName = string.Empty;

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            //Add Contribution to Credit Card Batch

            if (base.SQL.IsAMSEnabled(15))
            {
                amountString = "2";
                amountInt = 2;
                amountDbl = 2.00;
                fundName = "Simulated Fund";
                base.SQL.Giving_Batches_Delete(15, batchName, 2);
                base.SQL.Giving_Batches_Create(15, batchName, amountInt, 2);

                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "01", "2020", amountString, "Simulated Fund", null, null);
            }
            else
            {
                amountString = "2531";
                amountInt = 2531;
                amountDbl = 2531.00;
                fundName = "1 - General Fund";
                base.SQL.Giving_Batches_Delete(15, batchName, 2);
                base.SQL.Giving_Batches_Create(15, batchName, amountInt, 2);

                base.SQL.Giving_Batches_AddContributionToCreditCardBatch(15, batchName, fundName, amountDbl, new DateTime(2020, 12, 1));

            }

            // Authorize the batch
            test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "211");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Tests authorization response code 233 - General Decline by the processor.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Authorization_RC_233()
        {
            // Set initial conditions
            string batchName = "RC - 233";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, 2204, 2);



            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            //Add Contribution to Credit Card Batch
            if (base.SQL.IsAMSEnabled(15))
            {
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "01", "2020", "2204", "1 - General Fund", null, null);
            }
            else
            {
                base.SQL.Giving_Batches_AddContributionToCreditCardBatch(15, batchName, "1 - General Fund", 2204.00, new DateTime(2020, 12, 1));
            }

            // Authorize the batch
            test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "233");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        #region AMS Error Return Codes

        //This test case is already covered in the test case above
        //  [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("Tests AMS response code 211 - Invalid card verification number.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_AMS_RC_211()
        {
            // Set initial conditions
            if (base.SQL.IsAMSEnabled(15))
            {
                string batchName = "AMS-RC - 211";
                base.SQL.Giving_Batches_Delete(15, batchName, 2);
                base.SQL.Giving_Batches_Create(15, batchName, 2, 2);
                //base.SQL.Giving_Batches_AddContributionToCreditCardBatch(15, batchName, "Simulated Fund", 2.00, new DateTime(2020, 12, 1));

                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver();

                // Authorize the batch
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "211");

                //TEST

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
        }

        //   [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("Tests AMS response code 0 - Approved not Processed.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_AMS_RC_ZERO()
        {
            // Set initial conditions
            if (base.SQL.IsAMSEnabled(15))
            {
                string batchName = "AMS-RC - ZERO";
                base.SQL.Giving_Batches_Delete(15, batchName, 2);
                base.SQL.Giving_Batches_Create(15, batchName, 3, 2);
                // base.SQL.Giving_Batches_AddContributionToCreditCardBatch(15, batchName, "Simulated Fund", 3.00, new DateTime(2020, 12, 1));

                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver();

                //Add Contribution to Credit Card batch
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "01", "2020", "2", "Simulated Fund", null, null);


                // Authorize the batch
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "0");

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
        }

        //  [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("Tests AMS response code 930144 - Approved not Processed.CVV2 Required")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_AMS_RC_930144()
        {
            // Set initial conditions
            if (base.SQL.IsAMSEnabled(15))
            {
                string batchName = "AMS-RC - 930144";
                base.SQL.Giving_Batches_Delete(15, batchName, 2);
                base.SQL.Giving_Batches_Create(15, batchName, 4, 2);
                // base.SQL.Giving_Batches_AddContributionToCreditCardBatch(15, batchName, "Simulated Fund", 4.00, new DateTime(2020, 12, 1));

                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver();

                //Add Contribution to Credit Card Batch
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "01", "2020", "4", "Simulated Fund", null, null);

                // Authorize the batch
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "930144");

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
        }

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("Tests AMS response code 230 - Declined by CyberSource")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_AMS_RC_230()
        {
            // Set initial conditions
            string batchName = "AMS-RC - 230";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, 5, 2);


            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            //Add Contribution to Credit Card Batch
            if (base.SQL.IsAMSEnabled(15))
            {
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "01", "2020", "5", "Simulated Fund", null, null);

                // Authorize the batch
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "230");

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
        }

        #endregion  AMS Error REturn Codes

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Deletes a failed authorization.")]
        [Category(TestCategories.Services.PaymentProcessor), Category(TestCategories.Services.SmokeTest)]
        public void Giving_Contributions_Batches_CreditCardBatches_Authorization_DeleteFailedAuthorization()
        {
            // Set initial conditions
            string batchName = "Delete Failed";
            double batchAmount = 2402;
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, batchAmount, 2);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            //Add Contribution to Credit Card Batch
            if (base.SQL.IsAMSEnabled(15))
            {
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "01", "2020", "2402", "1 - General Fund", null, null);
                test.Driver.FindElementById("submitQuery").Click();

                //Suchitra Notes --Adding this return lik logic since there was not returning to Creditcard batches page
                // Navigate to giving->batches->credit card batches                                 
                //test.Driver.FindElementByLinkText(GeneralLinksWebDriver.RETURN).Click();
            }
            else
            {
                base.SQL.Giving_Batches_AddContributionToCreditCardBatch(15, batchName, "1 - General Fund", batchAmount, new DateTime(2020, 12, 1));
            }

            //Authorize Batch
            base.SQL.Giving_Batches_Authorize(15, batchName, base.SQL.IndividualID);
            
            //// Resume progress
            //test.Portal.Giving_Batches_CreditCardBatches_ResumeProgressWebDriver(batchName);
            
            System.Threading.Thread.Sleep(20000);

            //Retry.WithPolling(500).WithTimeout(20000).WithFailureMessage("Delete link not found")
            //      .Until(() => test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Delete")));

            // Attempt to delete the failed authorization
            test.Driver.FindElementByLinkText("Delete").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath("//div[@id='main_content']/div[1]/div[1]/span[2]"));

            // Verify page information            
            Assert.IsTrue(test.Driver.Title.Equals("Fellowship One :: Delete Contribution"));
            Assert.IsTrue(test.Driver.FindElementByXPath("//div[@id='main_content']/div[1]/div[1]/span[2]").Text.Contains("Delete Contribution"));  
            Assert.IsTrue(test.Driver.FindElementByXPath("//div[@id='main_content']/div[1]/div[2]/form/h2").Text.Contains(batchName));
            test.GeneralMethods.VerifyTextPresentWebDriver(string.Format("Note: This will decrease the batch by {0:c}.", batchAmount));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(string.Format("Note: This will decrease the batch by {0:c}.", batchAmount)));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(GeneralLinksWebDriver.Cancel));
                
            // Delete the authorization
            test.Driver.FindElementByXPath("//input[@type='submit']").Click();

            // Delete the credit card batch
            test.Portal.Giving_Batches_CreditCardBatches_Delete_WebDriver(batchName);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to delete a failed authorization, providing a comment greater than 300 characters.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Authorization_DeleteFailedAuthorizationCommentExceeding300Characters()
        {
            // Set initial conditions
            string batchName = "Delete Failed: Comment";
            string deleteTextID = String.Empty;
            double batchAmount = 2402;
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, batchAmount, 2);


            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            //Add Contribution to Credit Card Batch
            if (base.SQL.IsAMSEnabled(15))
            {
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "01", "2020", "2402", "1 - General Fund", null, null);
                // test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, null);

                //passing Reason code to make this test case work for AMS Enabled
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "233");

                deleteTextID = "deleteNote";


            }
            else
            {
                base.SQL.Giving_Batches_AddContributionToCreditCardBatch(15, batchName, "1 - General Fund", batchAmount, new DateTime(2020, 12, 1));

                //Authorize Batch
                base.SQL.Giving_Batches_Authorize(15, batchName, base.SQL.IndividualID);

                // Resume progress
                test.Portal.Giving_Batches_CreditCardBatches_ResumeProgressWebDriver(batchName);

                deleteTextID = "delete_auth_explain";

            }


            Retry.WithPolling(500).WithTimeout(25000).WithFailureMessage("Delete link not found")
                    .Until(() => test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Delete")));

            // Attempt to delete the failed authorization
            test.Driver.FindElementByLinkText("Delete").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.Id(deleteTextID));


            // Delete the authorization, provide a comment greater than 300 characters
            test.Driver.FindElementById(deleteTextID).SendKeys("1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901");
            test.Driver.FindElementByXPath("//input[@type='submit']").Click();

            // Verify the error presented to the user
            //TODO FGJ: Error message handling not set for AMS
            //test.GeneralMethods.VerifyTextPresentWebDriver(TextConstants.ErrorHeadingPlural);
            test.GeneralMethods.VerifyTextPresentWebDriver(TextConstants.ErrorHeadingSingular);
            test.GeneralMethods.VerifyTextPresentWebDriver("Note cannot exceed 300 characters, it is currently 301 characters.");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        //Updated by Jim
        //[Test, RepeatOnFailure, Timeout(1000)]
        [Author("Matthew Sneeden")]
        [Description("Edits a failed authorization.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Authorization_EditFailedAuthorization()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            // Set initial conditions
            string batchName = "Edit Failed";
            double batchAmount = 2402;
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, batchAmount, 2);

            // Login to portal
            test.Portal.LoginWebDriver();

            //Add Contribution to Credit Card Batch
            if (base.SQL.IsAMSEnabled(15))
            {
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "01", "2020", "2402", "1 - General Fund", null, null);

                //Suchitra Notes --Adding this return lik logic since the was not returning to Creditcard batches page
                // Navigate to giving->batches->credit card batches            
                test.Driver.FindElementByLinkText(GeneralLinksWebDriver.RETURN).Click();

            }
            else
            {
                base.SQL.Giving_Batches_AddContributionToCreditCardBatch(15, batchName, "1 - General Fund", batchAmount, new DateTime(2020, 12, 1));
            }

            //Authorize Batch
            base.SQL.Giving_Batches_Authorize(15, batchName, base.SQL.IndividualID);



            // Resume progress
            test.Portal.Giving_Batches_CreditCardBatches_ResumeProgressWebDriver(batchName);

            Retry.WithPolling(500).WithTimeout(20000).WithFailureMessage("Edit link not found")
                    .Until(() => test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Edit")));

            // Attempt to delete the failed authorization
            test.Driver.FindElementByLinkText("Edit").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath("//div[@id='main_content']/div[1]/div[1]/span[2]"));

            // Verify page information            
            Assert.IsTrue(test.Driver.Title.Equals("Fellowship One :: Edit Authorization"));
            Assert.AreEqual("Edit Authorization", test.Driver.FindElementByXPath("//div[@id='main_content']/div[1]/div[1]/span[2]").Text);
            Assert.AreEqual(batchName, test.Driver.FindElementByXPath("//div[@id='main_content']/div[1]/div[2]/form/h2").Text);
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(GeneralLinksWebDriver.Cancel));

            // Cancel
            test.Driver.FindElementByLinkText(GeneralLinksWebDriver.Cancel).Click();

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Edits a failed authorization; attempts to save changes using an expired credit card.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Authorization_EditFailedAuthorizationExpiredCreditCard()
        {

            // Set initial conditions
            string batchName = "Edit Failed: Expired Credit Card";
            double batchAmount = 2402;
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, batchAmount, 2);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            if (base.SQL.IsAMSEnabled(15))
            {
                //Add Contribution to Credit Card Batch - FGJ commented this since we are already doing this twice
                //Uncommented this and added if condition for AMS enabled..
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "01", "2020", "2402", "1 - General Fund", null, null);
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "233");
            }
            else
            {
                base.SQL.Giving_Batches_AddContributionToCreditCardBatch(15, batchName, "1 - General Fund", batchAmount, new DateTime(2020, 12, 1));
                base.SQL.Giving_Batches_Authorize(15, batchName, base.SQL.IndividualID);

            }


            // Edit a failed authorization
            test.Portal.Giving_Batches_CreditCardBatches_EditFailedAuthorization_WebDriver(batchName, null, null, null, "4111111111111111", "01", DateTime.Now.Year.ToString(), null, null, null, null, null, null);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Edits a failed authorization; attempts to save changes using an invalid credit card.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Authorization_EditFailedAuthorizationInvalidCreditCard()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            // Set initial conditions
            string batchName = "Edit Failed: Invalid Credit Card";
            double batchAmount = 2402;
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, batchAmount, 2);

            // Login to portal
            test.Portal.LoginWebDriver();

            //Add Contribution to Credit Card Batch
            if (base.SQL.IsAMSEnabled(15))
            {
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "01", "2020", "2402", "1 - General Fund", null, null);
                //test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, null);
                //Passing in Reason code as 233 for AMS enabled batch
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "233");
            }
            else
            {
                base.SQL.Giving_Batches_AddContributionToCreditCardBatch(15, batchName, "1 - General Fund", batchAmount, new DateTime(2020, 12, 1));

                //Authorize the Batch
                base.SQL.Giving_Batches_Authorize(15, batchName, base.SQL.IndividualID);
            }



            //Suchitra Notes --Adding this return lik logic since the was not returning to Creditcard batches page
            //Navigate to giving->batches->credit card batches            
            //FGJ - Don't need this for non AMS stuff. Revisit AMS enabled later
            //test.Driver.FindElementByLinkText(GeneralLinksWebDriver.RETURN).Click();

            // Edit a failed authorization
            //test.Portal.Giving_Batches_CreditCardBatches_EditFailedAuthorization_WebDriver("Edit Failed", null, null, null, "4111", null, null, null, null, null, null, null, null);
            test.Portal.Giving_Batches_CreditCardBatches_EditFailedAuthorization_WebDriver(batchName, null, null, null, "4111", null, null, null, null, null, null, null, null);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Authorization

        #region Settlement
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the available gear options for a credit card batch in the settled state.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Settled_Gear()
        {
            // Set initial conditions
            string batchName = "Gear: Settlement";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, 100, 2, 3);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to giving->batches->credit card batches
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Batches_CreditCardBatches);

            // Verify the gear options for a batch in the authorization state
            int itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName, "Name");
            //Assert.AreEqual(3, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]/ul/li", TableIds.Giving_Batches, itemRow + 1)));            
            //Assert.AreEqual(3, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]/ul/", TableIds.Giving_Batches, itemRow + 1)).FindElements(By.TagName("li")).Count);
            IWebElement table = test.Driver.FindElementByXPath(TableIds.Giving_Batches);

            //Assert.AreEqual(3, table.FindElements(By.TagName("tr"))[(itemRow + 1)].FindElements(By.TagName("td"))[4].FindElements(By.TagName("ul"))[0].FindElements(By.TagName("li")).Count);
            Assert.AreEqual(3, test.Driver.FindElements(By.XPath(string.Format("{0}/tbody/tr[{1}]/td[5]/ul/li", TableIds.Giving_Batches, itemRow + 1))).Count);

            test.Driver.FindElementByXPath(string.Format("//table[*]/tbody/tr[{0}]/td[*]/a[@class='gear_trigger']", itemRow + 1)).Click();

            Assert.AreEqual("Resume progress", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]/ul/li[1]/a/span", TableIds.Giving_Batches, itemRow + 1)).Text);
            Assert.AreEqual("View batch", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]/ul/li[2]/a/span", TableIds.Giving_Batches, itemRow + 1)).Text);
            Assert.AreEqual("Edit batch", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]/ul/li[3]/a/span", TableIds.Giving_Batches, itemRow + 1)).Text);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the received date throughout the credit card batch process.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_ReceivedDate()
        {
            // Set initial conditions
            string batchName = "Received date";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Create a credit card batch
            test.Portal.Giving_Batches_Create_WebDriver(batchName, 100, true, "t");

            // Add a contribution
            test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "12", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).ToString("yyyy"), "100", "1 - General Fund", null, null);

            // Authorize the batch
            test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "100");

            // Settle the batch
            test.Portal.Giving_Batches_CreditCardBatches_Settle_WebDriver(batchName);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the reference field is '-' for a contribution receipt generated via credit card batches.")]
        [Category(TestCategories.Services.PaymentProcessor), Category(TestCategories.Services.SmokeTest)]
        public void Giving_Contributions_Batches_CreditCardBatches_Settled()
        {
            // Set initial conditions
            string batchName = "Settled: Reference Field";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, 100, 2);
            //base.SQL.Giving_Batches_AddContributionToCreditCardBatch(15, batchName, "1 - General Fund", 100, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1), true);


            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            if (base.SQL.IsAMSEnabled(15))
            {
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "12", string.Format("{0:yyyy}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))), "100", "1 - General Fund", null, null);
                //Authorize batch
                test.Portal.Giving_Batches_CreditCardBatches_Authorize_WebDriver(batchName, "100");
            }
            else
            {
                base.SQL.Giving_Batches_AddContributionToCreditCardBatch(15, batchName, "1 - General Fund", 100, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1));
            }

            //Authorize Batch

            if (!base.SQL.IsAMSEnabled(15))
            {
                base.SQL.Giving_Batches_Authorize(15, batchName, base.SQL.IndividualID);
            }

            // Settle the batch
            test.Portal.Giving_Batches_CreditCardBatches_Settle_WebDriver(batchName, true);

            // Navigate to giving->batches->credit card batches            
            test.Driver.FindElementByLinkText(GeneralLinksWebDriver.RETURN).Click();

            // View a credit card batch
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Search);
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtBatchName_textBox").SendKeys(batchName);
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSearch").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Matthew Sneeden"));
            //test.GeneralMethods.SelectOptionFromGearWebDriver(Convert.ToInt16(test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName, "Name")), "View batch");

            // Verify reference field is blank (-)
            //Assert.AreEqual(batchName, test.Selenium.GetTable(string.Format("{0}.2.3", TableIds.Giving_Search)));
            //Assert.AreEqual("–", test.Selenium.GetTable(string.Format("{0}.2.7", TableIds.Giving_Search)));

            //class="grid_no_records"

            IWebElement table = test.Driver.FindElementByXPath(TableIds.Giving_Search);
            Assert.AreEqual(batchName, table.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[3].Text);
            Assert.AreEqual("–", table.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[7].Text);


            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Settlement

        #endregion Credit Card Batches

        #region Scanned Contribution

        #region Edit Batch Name

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3512: Verifies a user can edit the name of a Scanned Contribution batch that is in a pending state.")]
        public void Giving_Contributions_Batches_ScannedContribution_Edit_Pending()
        {
            // Setup batch
            string batchName = "Scanned Batch Edit - Pending";
            base.SQL.Giving_Batches_Delete(15, batchName, 4);
            string updatedBatchName = "Scanned Batch Updated - Pending";
            base.SQL.Giving_Batches_Create_Scanned(15, batchName, false, new System.Collections.Generic.List<double>() { 1.02 }, null,
                new System.Collections.Generic.List<string> { "666333111" },
                new System.Collections.Generic.List<string> { "111000025" }, false, "Auto Test Fund", false);

            try
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // Edit a Scanned Contribution batch that is in a Pending state
                test.Portal.Giving_Batches_ScannedContribution_EditBatchName(batchName, updatedBatchName, true);

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            catch (Exception e)
            {
                throw new WebDriverException(e.StackTrace);
            }
            finally
            {
                // Clean up
                base.SQL.Giving_Batches_Delete(15, updatedBatchName, 4);
            }

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3512: Verifies a user can attempt to edit the name of a Scanned Contribution and then cancel out of it.")]
        public void Giving_Contributions_Batches_ScannedContribution_Edit_Cancel()
        {
            // Setup batch
            string batchName = "Scanned Batch Cancel - Pending";
            base.SQL.Giving_Batches_Create_Scanned(15, batchName, false, new System.Collections.Generic.List<double>() { 1.02 }, null,
                new System.Collections.Generic.List<string> { "666333111" },
                new System.Collections.Generic.List<string> { "111000025" }, false, "Auto Test Fund", false);

            try
            {
                // Login a Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // Attempt to edit the name of a Scanned Contribution batch and then cancel out of it
                test.Portal.Giving_Batches_ScannedContribution_EditBatchName(batchName, null, true);

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            catch (Exception e)
            {
                throw new WebDriverException(e.StackTrace);
            }
            finally
            {
                // Clean up
                base.SQL.Giving_Batches_Delete(15, batchName, 4);
            }

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Category(TestCategories.Services.SmokeTest)]
        [Description("F1-3512: Verifies a user can edit the name of a Scanned Contribution batch that is in a Saved state.")]
        public void Giving_Contributions_Batches_ScannedContribution_Edit_Saved()
        {           
            // Setup batch
            string batchName = "Scanned Batch Edit - Saved";
            base.SQL.Giving_Batches_Delete(15, batchName, 4);
            string updatedBatchName = "Scanned Batch Updated - Saved";
            base.SQL.Giving_Batches_Create_Scanned(15, batchName, true, new System.Collections.Generic.List<double>() { 1.03 }, null,
                new System.Collections.Generic.List<string> { "777444222" },
                new System.Collections.Generic.List<string> { "111000025" }, false, "Auto Test Fund", false);

            try
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // Edit a Scanned Contribution batch that is in a Saved state
                test.Portal.Giving_Batches_ScannedContribution_EditBatchName(batchName, updatedBatchName, true);

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            catch (Exception e)
            {
                throw new WebDriverException(e.StackTrace);
            }
            finally
            {
                // Clean up
                base.SQL.Giving_Batches_Delete(15, updatedBatchName, 4);
            }

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3512: Verifies a user can edit the name of a Scanned Contribution batch with HTML code and it will save it as text.")]
        public void Giving_Contributions_Batches_ScannedContribution_Edit_HTML()
        {
            // Setup batch
            string batchName = "Scanned Batch Edit - HTML";
            string updatedBatchName = "HTML - <b>Updated</b> - HTML";

            base.SQL.Giving_Batches_Delete(15, updatedBatchName, 4);
            base.SQL.Giving_Batches_Delete(15, batchName, 4);

            base.SQL.Giving_Batches_Create_Scanned(15, batchName, false, new System.Collections.Generic.List<double>() { 1.04 }, null,
                new System.Collections.Generic.List<string> { "888555333" },
                new System.Collections.Generic.List<string> { "111000025" }, false, "Auto Test Fund", false);

            try
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // Edit a Scanned Contribution batch with script code in the name
                test.Portal.Giving_Batches_ScannedContribution_EditBatchName(batchName, updatedBatchName, true);

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            finally
            {
                // Clean up
                base.SQL.Giving_Batches_Delete(15, updatedBatchName, 4);
                base.SQL.Giving_Batches_Delete(15, batchName, 4);
            }

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3512: Verifies a user cannot edit the name of an RDC batch.")]
        public void Giving_Contributions_Batches_RemoteDepositCapture_NoEditBatchName()
        {
            // Setup batch
            string batchName = "RDC Batch Edit - Pending";
            string updatedBatchName = "RDC Batch Updated - Pending";
            base.SQL.Giving_Batches_Create_RDC(15, batchName, false, new System.Collections.Generic.List<double>() { 1.05 }, null,
                new System.Collections.Generic.List<string> { "888555333" },
                new System.Collections.Generic.List<string> { "111000025" }, false, "Auto Test Fund", false);

            try
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // Attempt to edit the name of an RDC batch in a Pending state
                test.Portal.Giving_Batches_ScannedContribution_EditBatchName(batchName, updatedBatchName, false);

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            catch (Exception e)
            {
                throw new WebDriverException(e.StackTrace);
            }
            finally
            {
                // Clean up
                base.SQL.Giving_Batches_Delete(15, batchName, 3);
                base.SQL.Giving_Batches_Delete_RDC(15, batchName, "888555333", "111000025");
            }

        }
        #endregion Edit Batch Name

        #region Delete Batch

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3513: Verifies a user can delete a Scanned Contribution batch in a Pending state.")]
        public void Giving_Contributions_Batches_ScannedContribution_Delete_Pending()
        {
            // Setup batch
            string batchName = "Scanned Batch Delete - Pending";
            base.SQL.Giving_Batches_Create_Scanned(15, batchName, false, new System.Collections.Generic.List<double>() { 1.06 }, null,
                new System.Collections.Generic.List<string> { "888555333" },
                new System.Collections.Generic.List<string> { "111000025" }, false, "Auto Test Fund", false);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Delete a Pending Scanned batch
            test.Portal.Giving_Batches_ScannedContribution_DeleteBatch(true, batchName);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3513: Verifies a user can delete a Scanned Contribution batch in a Saved state")]
        public void Giving_Contributions_Batches_ScannedContribution_Delete_Saved()
        {
            // Setup batch
            string batchName = "Scanned Batch Delete - Saved";
            base.SQL.Giving_Batches_Create_Scanned(15, batchName, true, new System.Collections.Generic.List<double>() { 1.07 }, null,
                new System.Collections.Generic.List<string> { "555999222" },
                new System.Collections.Generic.List<string> { "111000025" }, false, "Auto Test Fund", false);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Delete a Saved Scanned batch
            test.Portal.Giving_Batches_ScannedContribution_DeleteBatch(true, batchName);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3513: Verifies a user without permissions cannot delete a scanned batch")]
        public void Giving_Contributions_Batches_ScannedContribution_Delete_NoAccess()
        {
            // Setup batch
            string batchName = "Scanned Batch Delete - NoAccess";
            base.SQL.Giving_Batches_Create_Scanned(15, batchName, false, new System.Collections.Generic.List<double>() { 1.08 }, null,
                new System.Collections.Generic.List<string> { "555999222" },
                new System.Collections.Generic.List<string> { "111000025" }, false, "Auto Test Fund", false);

            try
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.checkscanonly", "FT4life!", "dc");

                // Attempt to delete a batch without permission
                test.Portal.Giving_Batches_ScannedContribution_DeleteBatch(false, batchName);

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            catch (Exception e)
            {
                throw new WebDriverException(e.StackTrace);
            }
            finally
            {
                // Clean up
                base.SQL.Giving_Batches_Delete(15, batchName, 4);
            }

        }

        #endregion Delete Batch

        #endregion Scanned Contribution

        #region View Batch
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user is taken to the correct page via 'View batch' for a non CC Batch")]
        public void Giving_Contributions_Batches_ViewBatch_NonCreditCard()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to giving->batches
            test.GeneralMethods.Navigate_Portal(Navigation.Giving.Contributions.Batches);
            test.Driver.FindElementByLinkText("General").Click();

            // Click the gear associated with a non credit card batch            
            //string batchName = test.Selenium.GetTable(string.Format("{0}.1.1", TableIds.Giving_Batches));
            IWebElement table = test.Driver.FindElementById(TableIds.Portal.Giving_Batches);
            string batchName = table.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[1].Text;

            TestLog.WriteLine(string.Format("Batch Name: {0}", batchName));
            test.GeneralMethods.SelectOptionFromGearWebDriver(Convert.ToInt16(test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName, "Name")), "View batch");

            // Verify title, text
            Assert.AreEqual(test.Driver.Title, "Fellowship One :: Contribution Search", "Title does not match");
            test.GeneralMethods.VerifyTextPresentWebDriver("Contribution Search");

            // Verify the batch name
            test.GeneralMethods.VerifyTextPresentWebDriver(batchName);
            // Assert.AreEqual(batchName, test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtBatchName_textBox").GetAttribute("value"), "Batch Name is incorrect");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion View Batch

        #region Search
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Performs a batches search yielding no results.")]
        public void Giving_Contributions_Batches_SearchNoResults()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to giving->batches
            test.GeneralMethods.Navigate_Portal(Navigation.Giving.Contributions.Batches);

            // Perform a search that will yield no results
            test.Portal.Giving_Batches_Search_WebDriver("asdfop", null, null, null, null);

            // Verify text displayed to user
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Add a new batch"));
            //test.Selenium.VerifyTextPresent(new string[] { "No batches exist", "To add a new batch, click the link below." });
            test.GeneralMethods.VerifyTextPresentWebDriver("No batches exist");
            test.GeneralMethods.VerifyTextPresentWebDriver("To add a new batch, click the link below.");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Performs a credit card batches search yielding no results.")]
        public void Giving_Contributions_Batches_CreditCardBatches_SearchNoResults()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to giving->batches->credit card batches
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Batches_CreditCardBatches);

            // Perform a search that will yield no results
            test.Portal.Giving_Batches_Search_WebDriver("asdf", null, null, null, null);

            // Verify text displayed to user
            //test.Selenium.VerifyTextPresent(new string[] { "No batches exist", "No batches have been created.", "To add a new batch, click the link below." });
            test.GeneralMethods.VerifyTextPresentWebDriver("No batches exist");
            test.GeneralMethods.VerifyTextPresentWebDriver("No batches have been created.");
            test.GeneralMethods.VerifyTextPresentWebDriver("To add a new batch, click the link below.");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Search
        
        //These test cases for AMS Batch settle issues
        #region AMS Batch Settle JIRA-Issues
        [Test, RepeatOnFailure]
        [Author("Suchitra")]
        [Description("Verify that when one of the batch contribution fail then user can access batch from batch list to process again")]
        public void Giving_Contributions_Batches_CreditCardBatches_AMS_BatchSettle_MultipleContributions_BatchError()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            if (test.SQL.IsAMSEnabled(test.Portal.ChurchID))
            {
                // Set initial conditions
                string batchName = "AMS-Multi Contribution";
                base.SQL.Giving_Batches_Delete(15, batchName, 2);
                base.SQL.Giving_Batches_Create(15, batchName, 12, 2);

                // Login to Portal
                test.Portal.LoginWebDriver();

                // Resume progress
                test.Portal.Giving_Batches_CreditCardBatches_ResumeProgressWebDriver(batchName);

                // Verify page title, header            
                Assert.AreEqual("Fellowship One :: Add Contributions", test.Driver.Title, "Tittle Verify Error");
                test.GeneralMethods.VerifyTextPresentWebDriver("Add Contributions");                              

                // Search for, select the individual
              //  test.Portal.Giving_SearchIndividual_WebDriver("Matthew Sneeden", null);      
                
                //Add contribution
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "12", "2020","5.00","Simulated Fund", string.Empty, string.Empty);

                //Add contribution
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "12", "2020", "7.00", "Simulated Fund", string.Empty, string.Empty);
                
                //Settle the batch
                test.Driver.FindElementByXPath("//input[@value='Settle this batch']").Click();
                WebDriverWait wait = new WebDriverWait(test.Driver, TimeSpan.FromSeconds(600));
                
                IWebElement element = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("batchStateTitle")));

                //Verify Error message exists for one of the batch
             

                //Return to Batches List and verify batch exisits with inprogress status

                // Click the return button
                test.Driver.FindElementByLinkText(GeneralLinksWebDriver.RETURN).Click();


                // Navigate to giving->batches->credit card batches
                test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Batches_CreditCardBatches);

                // verify batch name availabel in list for processing with In progress status
                int itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Portal.Giving_Batches, batchName, "Name", null);
                TestLog.WriteLine("Row number : " + itemRow);
                Assert.AreEqual(batchName, test.Driver.FindElement(By.XPath(string.Format("//table[@id='batches_grid']/tbody/tr[{0}]/td[1]", itemRow+1))).Text, "Batch Name not found in batch list");
               // Assert.AreEqual(batchName, test.Driver.FindElement(By.XPath("//table[@id='batches_grid']")).FindElement(By.TagName("tbody").FindElement(By.TagName("tr[{0]/td[1]/a", itemRow + 1))).Text, "Batch Name not found in batch list");
                Assert.AreEqual("In Progress", test.Driver.FindElement(By.XPath(string.Format("//table[@id='batches_grid']/tbody/tr[{0}]/td[2]", itemRow+1))).Text, "Incorrect batch status");


                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
        }
        [Test, RepeatOnFailure]
        [Author("Suchitra")]
        [Description("Verify that when one of the batch contribution fail then user can access batch from batch list to process again")]
        public void Giving_Contributions_Batches_CreditCardBatches_AMS_BatchSettle_MultipleFunds_BatchError()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            if (test.SQL.IsAMSEnabled(test.Portal.ChurchID))
            {
                // Set initial conditions
                string batchName = "AMS-Multi Fund Contribution";
                base.SQL.Giving_Batches_Delete(15, batchName, 2);
                base.SQL.Giving_Batches_Create(15, batchName, 12, 2);

                // Login to Portal
                test.Portal.LoginWebDriver();

                // Resume progress
                test.Portal.Giving_Batches_CreditCardBatches_ResumeProgressWebDriver(batchName);

                // Verify page title, header            
                Assert.AreEqual("Fellowship One :: Add Contributions", test.Driver.Title, "Tittle Verify Error");
                test.GeneralMethods.VerifyTextPresentWebDriver("Add Contributions");

                // Search for, select the individual
                //  test.Portal.Giving_SearchIndividual_WebDriver("Matthew Sneeden", null);      

                //Add contribution
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "12", "2020", "5.00", "Simulated Fund", string.Empty, string.Empty);

                //Add contribution
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111111111111111", "12", "2020", "7.00", "1 - General Fund", string.Empty, string.Empty);

                //Settle the batch
                test.Driver.FindElementByXPath("//input[@value='Settle this batch']").Click();
                WebDriverWait wait = new WebDriverWait(test.Driver, TimeSpan.FromSeconds(600));

                IWebElement element = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("batchStateTitle")));

                //Verify Error message exists for one of the batch


                //Return to Batches List and verify batch exisits with inprogress status

                // Click the return button
                test.Driver.FindElementByLinkText(GeneralLinksWebDriver.RETURN).Click();


                // Navigate to giving->batches->credit card batches
                test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Batches_CreditCardBatches);

                // verify batch name availabel in list for processing with In progress status
                int itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Portal.Giving_Batches, batchName, "Name", null);
                TestLog.WriteLine("Row number : " + itemRow);
                Assert.AreEqual(batchName, test.Driver.FindElement(By.XPath(string.Format("//table[@id='batches_grid']/tbody/tr[{0}]/td[1]", itemRow + 1))).Text, "Batch Name not found in batch list");
                // Assert.AreEqual(batchName, test.Driver.FindElement(By.XPath("//table[@id='batches_grid']")).FindElement(By.TagName("tbody").FindElement(By.TagName("tr[{0]/td[1]/a", itemRow + 1))).Text, "Batch Name not found in batch list");
                Assert.AreEqual("In Progress", test.Driver.FindElement(By.XPath(string.Format("//table[@id='batches_grid']/tbody/tr[{0}]/td[2]", itemRow + 1))).Text, "Incorrect batch status");


                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
        }

        #endregion AMS Batch Settle issues

    }


    [TestFixture]
    [Category(TestCategories.Services.PaymentProcessor)]
    public class Portal_Giving_Contributions_Batches_Remote_Deposit_Capture_WebDriver : FixtureBaseWebDriver
    {

        #region Private Members

        private string _rdcBatchID;
        private string _rdcBatchName = string.Format("Test RDC Batch - {0:g}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")));

        private string _rdcBatchPending = "Test RDC Batch - PendingWD";
        private string _rdcBatchSaved = "Test RDC Batch - SavedWD";
        private string _rdcBatchPendingSplit = "Test RDC Batch - Pending - SplitsWD";
        private string _rdcBatchPendingMatched = "Test RDC Batch - Pending - MatchedWD";
        private string _rdcBatchPendingUnmatched = "Test RDC Batch - Pending - UnmatchedWD";

        private string _rdcBatchAccountNumberInd = Guid.NewGuid().ToString().Substring(0, 5);
        private string _rdcBatchAccountNumberShared = Guid.NewGuid().ToString().Substring(0, 5);
        private string _rdcBatchAccountNumberOrg = Guid.NewGuid().ToString().Substring(0, 5);
        private string _rdcBatchAccountNumberOrgShared = Guid.NewGuid().ToString().Substring(0, 5);

        private int _rdcBatchRoutingNumberInd = 12345678;
        private int _rdcBatchRoutingNumberShared = 56789;
        private int _rdcBatchRoutingNumberOrg = 87654321;
        private int _rdcBatchRoutingNumberSharedOrg = 98765;

        private string _organizationSingle;
        private string _organizationShared1;
        private string _organizationShared2;

        //Household
        private string _rdcBatchPendingHouseholdUnmatched = "Test RDC Batch - Pending - Household - Unmatched";
        private string _rdcBatchAccountNumberHousehold = Guid.NewGuid().ToString().Substring(0, 5);
        private int _rdcBatchRoutingNumberHousehold = 673820185;

        #endregion Private Members

        [FixtureSetUp]
        public void FixtureSetUp()
        {

            // Create an rdc batch in a pending state
            base.SQL.Giving_Batches_Delete(15, _rdcBatchPending, 3);
            base.SQL.Giving_Batches_Create_RDC(15, _rdcBatchPending, false, new System.Collections.Generic.List<double>() { 1.00, 2.00 });

            // Create an rdc batch in a saved state
            base.SQL.Giving_Batches_Delete(15, _rdcBatchSaved, 3);
            base.SQL.Giving_Batches_Create_RDC(15, _rdcBatchSaved, true, new System.Collections.Generic.List<double>() { 1.00, 4.00 });

            // Create an rdc batch in a pending state that can be used for splitting
            base.SQL.Giving_Batches_Delete(15, _rdcBatchPendingSplit, 3);
            base.SQL.Giving_Batches_Create_RDC(15, _rdcBatchPendingSplit, false, new System.Collections.Generic.List<double>() { 1.00, 2.00 });

            // Create an RDC Batch in a pending state with a matched person
            base.SQL.Giving_Batches_Delete(15, _rdcBatchPendingMatched, 3);
            base.SQL.Giving_Batches_Create_RDC(15, _rdcBatchPendingMatched, false, new System.Collections.Generic.List<double>() { 9.00 }, new System.Collections.Generic.List<string>() { "Bryan Mikaelian" });

            // Create an RDC Batch in a pending state without a matched person
            base.SQL.Giving_Batches_Delete(15, _rdcBatchPendingUnmatched, 3);
            base.SQL.Giving_Batches_Create_RDC(15, _rdcBatchPendingUnmatched, false, new System.Collections.Generic.List<double>() { 5.00 });

            _rdcBatchID = base.SQL.Execute("SELECT TOP 1 [BATCH_ID] FROM [ChmContribution].[dbo].[BATCH] WITH (NOLOCK) WHERE [CHURCH_ID] = 15 AND [BatchTypeID] = 3").Rows[0][0].ToString();

            // Create accounts

            // Single Individual
            var individualID = base.SQL.People_Individuals_FetchID(15, "Bryan Mikaelian");
            var householdId = base.SQL.People_Households_FetchID(15, individualID);
            base.SQL.Giving_Accounts_Create(15, 1, householdId, individualID, _rdcBatchAccountNumberInd, _rdcBatchRoutingNumberInd);



            // Shared Individual
            var individualID1 = base.SQL.People_Individuals_FetchID(15, "Matthew Sneeden");
            var individualID2 = base.SQL.People_Individuals_FetchID(15, "Mark Lindsley");
            var householdId1 = base.SQL.People_Households_FetchID(15, individualID1);
            var householdId2 = base.SQL.People_Households_FetchID(15, individualID2);
            base.SQL.Giving_Accounts_Create(15, 1, householdId1, individualID1, _rdcBatchAccountNumberShared, _rdcBatchRoutingNumberShared);
            base.SQL.Giving_Accounts_Create(15, 1, householdId2, individualID2, _rdcBatchAccountNumberShared, _rdcBatchRoutingNumberShared);

            // Single Organizaiton
            DataTable results = base.SQL.Execute("SELECT TOP 1 HOUSEHOLD_ID FROM ChmPeople.dbo.HOUSEHOLD WHERE CHURCH_ID = 15 AND PARTY_TYPE_ID IN (2,3) AND HOUSEHOLD_NAME = 'Auto Organization'");
            var organizationID = (int)results.Rows[0][0];
            _organizationSingle = "Auto Organization";
            base.SQL.Giving_Accounts_Create(15, 1, organizationID, null, _rdcBatchAccountNumberOrg, 98765);

            // Shared organization
            DataTable results2 = base.SQL.Execute("SELECT TOP 3 HOUSEHOLD_ID, HOUSEHOLD_NAME FROM ChmPeople.dbo.HOUSEHOLD WHERE CHURCH_ID = 15 AND PARTY_TYPE_ID IN (2,3)");
            var organizationID1 = (int)results2.Rows[1][0];
            _organizationShared1 = results2.Rows[1][1].ToString();
            var organizationID2 = (int)results2.Rows[2][0];
            _organizationShared2 = results2.Rows[2][1].ToString();
            base.SQL.Giving_Accounts_Create(15, 1, organizationID1, null, _rdcBatchAccountNumberOrgShared, _rdcBatchRoutingNumberSharedOrg);
            base.SQL.Giving_Accounts_Create(15, 1, organizationID2, null, _rdcBatchAccountNumberOrgShared, _rdcBatchRoutingNumberSharedOrg);

            //HOUSEHOLD

            // Create an RDC Batch in a pending state without a matched houshold
            base.SQL.Giving_Batches_Delete(15, _rdcBatchPendingHouseholdUnmatched, 3);
            base.SQL.Giving_Batches_Create_RDC(15, _rdcBatchPendingHouseholdUnmatched, false, new System.Collections.Generic.List<double>() { 19.97 }, null, new System.Collections.Generic.List<string> { _rdcBatchAccountNumberHousehold }, new System.Collections.Generic.List<string>() { _rdcBatchRoutingNumberHousehold.ToString() }, false, "1 - General Fund", true);

            // Create accounts

            // Household
            TestLog.WriteLine("Create Household Batch");
            var rdcHouseholdIndId = base.SQL.People_Individuals_FetchID(15, "Felix Gaytan");
            var rdcHouseholdId = base.SQL.People_Households_FetchID(15, rdcHouseholdIndId);
            base.SQL.Giving_Accounts_Create(15, 1, rdcHouseholdId, rdcHouseholdIndId, _rdcBatchAccountNumberHousehold, _rdcBatchRoutingNumberHousehold);



        }


        #region Remote Deposit Capture


        #region Search

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Performs a remote deposit capture search, by name, yielding no results.")]
        [Category(TestCategories.Services.RDCChecker), Category(TestCategories.Services.SmokeTest)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Search_Name_NoResults()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Perform a search that will yield no results
            test.Portal.Giving_Batches_RemoteDepositCapture_Search_WebDriver("asdf", 12345, null, null, null);

            // Verify text displayed to user
            test.GeneralMethods.VerifyTextPresentWebDriver("No batches were found that match your criteria.");

            // LogoutWebDriver of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Performs a remote deposit capture search, by amount, yielding no results.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Search_Amount_NoResults()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Perform a search that will yield no results
            test.Portal.Giving_Batches_RemoteDepositCapture_Search_WebDriver(null, 999998, 999999, null, null);

            // Verify text displayed to user
            test.GeneralMethods.VerifyTextPresentWebDriver("No batches were found that match your criteria.");

            // LogoutWebDriver of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Performs a remote deposit capture using special characters.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Search_SpecialCharacters()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Perform a search that will yield no results
            test.Portal.Giving_Batches_RemoteDepositCapture_Search_WebDriver(": ./", null, null, null, null);

            // Verify text displayed to user
            test.GeneralMethods.VerifyTextPresentWebDriver("No batches were found that match your criteria.");

            // LogoutWebDriver of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the pending filter only shows pending batches.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Search_Show_Pending()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Filter on Pending Batches
            test.Portal.Giving_Batches_RemoteDepositCapture_Filter_WebDriver(GeneralEnumerations.RemoteDepositeCaptureSearchFilters.Pending);

            // Verify
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Giving_RemoteDepositCapture, _rdcBatchPending, "Name"), "Pending batch was not present.");
            int row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RemoteDepositCapture, _rdcBatchPending, "Name") + 1;
            Assert.AreEqual(_rdcBatchPending, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[1]", TableIds.Giving_RemoteDepositCapture, row)).Text);

            //Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Giving_RemoteDepositCapture, _rdcBatchSaved, "Name"), "Saved batch was present.");
            row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RemoteDepositCapture, _rdcBatchSaved, "Name", null, false);
            Assert.AreEqual(0, row, string.Format("[{0}] was found. Only Pending Batches should be visible.", _rdcBatchSaved));

            // LogoutWebDriver of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the saved filter only shows saved batches.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Search_Show_Saved()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Filter on Pending Batches
            test.Portal.Giving_Batches_RemoteDepositCapture_Filter_WebDriver(GeneralEnumerations.RemoteDepositeCaptureSearchFilters.Saved);

            // Verify
            //Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Giving_RemoteDepositCapture, _rdcBatchPending, "Name"), "Pending batch was present.");
            int row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RemoteDepositCapture, _rdcBatchPending, "Name", null, false);
            Assert.AreEqual(0, row, string.Format("[{0}] was found. Only Saved Batches should be visible.", _rdcBatchPending));

            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Giving_RemoteDepositCapture, _rdcBatchSaved, "Name"), "Saved batch was not present.");
            row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RemoteDepositCapture, _rdcBatchSaved, "Name") + 1;
            Assert.AreEqual(_rdcBatchSaved, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[1]", TableIds.Giving_RemoteDepositCapture, row)).Text);

            // LogoutWebDriver of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the all filter shows alls batches.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Search_Show_All()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Filter on Pending Batches
            test.Portal.Giving_Batches_RemoteDepositCapture_Filter_WebDriver(GeneralEnumerations.RemoteDepositeCaptureSearchFilters.All);

            // Verify
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Giving_RemoteDepositCapture, _rdcBatchPending, "Name"), "Pending batch was not present.");
            int row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RemoteDepositCapture, _rdcBatchPending, "Name") + 1;
            Assert.AreEqual(_rdcBatchPending, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[1]", TableIds.Giving_RemoteDepositCapture, row)).Text);

            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Giving_RemoteDepositCapture, _rdcBatchSaved, "Name"), "Saved batch was not present.");
            row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RemoteDepositCapture, _rdcBatchSaved, "Name") + 1;
            Assert.AreEqual(_rdcBatchSaved, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[1]", TableIds.Giving_RemoteDepositCapture, row)).Text);

            // LogoutWebDriver of Portal
            test.Portal.LogoutWebDriver();
        }

        #endregion Search

        #region Pending

        #region Audit

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the remote deposit capture audit page.")]
        [Category(TestCategories.Services.RDCChecker), Category(TestCategories.Services.SmokeTest)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Audit()
        {

            // Create an RDC Batch in a pending state without a matched person
            string importBatchName = "Test RDC Batch - AuditLink - Pending";
            base.SQL.Giving_Batches_Delete(15, importBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, importBatchName, false, new System.Collections.Generic.List<double>() { 11.11 });


            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View the audit page for an rdc batch in the pending state
            test.Portal.Giving_Batches_ImportedBatches_View_Audit_WebDriver(importBatchName);

            // Verify user is taken to the audit page
            Assert.AreEqual("Fellowship One :: Batches Audit", test.Driver.Title, "Title mismatch");
            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//div[@id='return_to_app']"));
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.XPath("//div[@id='nav']"));
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.XPath("//p[@id='breadcrumb']"));

            // Verify download pdf link, gear is not available for pending batch
            //Assert.IsTrue(test.Driver.FindElementByLinkText("Download PDF").Displayed, "Download PDF Link is visible");
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.LinkText("Download PDF"));

            // Verify table data
            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//table[@class='bccp_audit']/tbody/tr[1]/td[3]/span[text()='Automated System Event']"));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//table[@class='bccp_audit']/tbody/tr[1]/td[position()=4 and normalize-space(text())='imported batch from remote deposit source']"));

            // Logout of portal
            test.Portal.LogoutWebDriver();

            base.SQL.Giving_Batches_Delete(15, importBatchName, 3);

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the snapshot section for a pending RDC batch that is unmatched.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Audit_Snapshot_Unmatched()
        {

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View a pending batch
            test.Portal.Giving_Batches_ImportedBatches_View_Audit_WebDriver(_rdcBatchPendingUnmatched);

            // Verify the snapshot section
            var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$5.00", "Amount") + 1;
            Assert.AreEqual("Unmatched", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[1]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("$5.00", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[4]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the snapshot section for a pending RDC batch that matched an individual.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Audit_Snapshot_Matched()
        {

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View a pending batch
            test.Portal.Giving_Batches_ImportedBatches_View_Audit_WebDriver(_rdcBatchPendingMatched);

            // Verify the snapshot section
            var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$9.00", "Amount") + 1;
            //Assert.AreEqual("Bryan Mikaelian", test.Selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[1]", TableIds.Giving_RDC_RDCBatchItemList, row)));
            Assert.AreEqual("$9.00", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[4]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            // Logout
            test.Portal.LogoutWebDriver();
        }


        #endregion Audit

        #region Save RDC Batch



        //[Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Views the remote deposit capture household pending automatched page.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Household_Test()
        {

            // Set initial conditions
            var account = Guid.NewGuid().ToString().Substring(0, 5);
            var rdcBatchName = "Test RDC Batch - Pending - Household - Automatch";
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 10.99 }, new System.Collections.Generic.List<string>() { "Felix Gaytan" }, new System.Collections.Generic.List<string> { _rdcBatchAccountNumberHousehold }, new System.Collections.Generic.List<string>() { _rdcBatchRoutingNumberHousehold.ToString() }, false, "1 - General Fund", true);

            //Shared
            var _rdcBatchAccountNumberHouseholdInd = Guid.NewGuid().ToString().Substring(0, 5);
            var _rdcBatchRoutingNumberHouseholdInd = 111000025;

            var individualID1 = base.SQL.People_Individuals_FetchID(15, "Felix Gaytan");
            var individualID2 = base.SQL.People_Individuals_FetchID(15, "Nina Segovia");
            var individualID3 = base.SQL.People_Individuals_FetchID(15, "Maya Gaytan");
            var householdId1 = base.SQL.People_Households_FetchID(15, individualID1);
            var householdId2 = base.SQL.People_Households_FetchID(15, individualID2);
            var householdId3 = base.SQL.People_Households_FetchID(15, individualID3);

            base.SQL.Giving_Accounts_Create(15, 1, householdId1, individualID1, _rdcBatchAccountNumberHouseholdInd, _rdcBatchRoutingNumberHouseholdInd);
            base.SQL.Giving_Accounts_Create(15, 1, householdId2, individualID2, _rdcBatchAccountNumberHouseholdInd, _rdcBatchRoutingNumberHouseholdInd);
            base.SQL.Giving_Accounts_Create(15, 1, householdId3, individualID3, _rdcBatchAccountNumberHouseholdInd, _rdcBatchRoutingNumberHouseholdInd);

            var rdcBatchNameInd = "Test RDC Batch - Pending - HH - Multi";
            base.SQL.Giving_Batches_Delete(15, rdcBatchNameInd, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchNameInd, false, new System.Collections.Generic.List<double>() { 99.99 }, null, new System.Collections.Generic.List<string> { _rdcBatchAccountNumberHouseholdInd }, new System.Collections.Generic.List<string>() { _rdcBatchRoutingNumberHouseholdInd.ToString() }, false, "1 - General Fund", true);

            // Set initial conditions
            var rdcBatchNameHH = "Test RDC Batch - Pending - HH - Automatch";
            base.SQL.Giving_Batches_Delete(15, rdcBatchNameHH, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchNameHH, false, new System.Collections.Generic.List<double>() { 100.55 }, new System.Collections.Generic.List<string>() { "Nina Segovia" }, new System.Collections.Generic.List<string> { _rdcBatchAccountNumberHouseholdInd }, new System.Collections.Generic.List<string>() { _rdcBatchRoutingNumberHouseholdInd.ToString() }, false, "1 - General Fund", true);

            // Set initial conditions
            var rdcBatchNameHHInd = "Test RDC Batch - Pending - HHInd - Automatch";
            base.SQL.Giving_Batches_Delete(15, rdcBatchNameHHInd, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchNameHHInd, false, new System.Collections.Generic.List<double>() { 123.45 }, new System.Collections.Generic.List<string>() { "Maya Gaytan" }, new System.Collections.Generic.List<string> { _rdcBatchAccountNumberHouseholdInd }, new System.Collections.Generic.List<string>() { _rdcBatchRoutingNumberHouseholdInd.ToString() }, false, "1 - General Fund", true);


            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View the save page for an rdc batch in the pending state
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(_rdcBatchPendingHouseholdUnmatched);

            // Verify user is taken to the pending page
            Assert.AreEqual("Fellowship One :: Remote Deposit Capture - Pending", test.Driver.Title);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }


        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Views the remote deposit capture pending page using link")]
        [Category(TestCategories.Services.RDCChecker), Category(TestCategories.Services.SmokeTest)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending()
        {

            // Create an RDC Batch in a pending state without a matched person
            string importBatchName = "Test RDC Batch - Link - Pending";
            base.SQL.Giving_Batches_Delete(15, importBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, importBatchName, false, new System.Collections.Generic.List<double>() { 33.33 });

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View the save page for an rdc batch in the pending state
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(importBatchName);

            // Verify user is taken to the pending page
            Assert.AreEqual("Fellowship One :: Batch - Pending", test.Driver.Title);

            // Logout of portal
            test.Portal.LogoutWebDriver();


            base.SQL.Giving_Batches_Delete(15, importBatchName, 3);

        }



        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Views the remote deposit capture pending page using gear option")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_GearOption()
        {

            // Create an RDC Batch in a pending state without a matched person
            string importBatchName = "Test RDC Batch - Gear - Pending";
            base.SQL.Giving_Batches_Delete(15, importBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, importBatchName, false, new System.Collections.Generic.List<double>() { 44.44 });


            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View the save page for an rdc batch in the pending state
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(importBatchName, true);

            // Verify user is taken to the pending page
            Assert.AreEqual("Fellowship One :: Batch - Pending", test.Driver.Title);

            // Logout of portal
            test.Portal.LogoutWebDriver();

            base.SQL.Giving_Batches_Delete(15, importBatchName, 3);

        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user cannot save a pending batch using an invalid received date.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_SaveInvalidDate()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to giving->batches->remote deposit capture
            test.GeneralMethods.Navigate_Portal(Navigation.Giving.Contributions.Batches_RemoteDepositCapture);

            // View a pending batch
            test.Portal.Giving_Batches_ImportedBatches_View_Pending(_rdcBatchPending);

            // Attempt to save the batch using an received date
            test.Driver.FindElementById("rdc_date").Clear();
            test.Driver.FindElementById("rdc_date").SendKeys("151515");
            test.Driver.FindElementById("rdc_date").SendKeys(Keys.ArrowDown);
            test.Driver.FindElementById("rdc_date").SendKeys(Keys.Tab);
            //test.Selenium.Focus("btn_submit");
            new SelectElement(test.Driver.FindElementById("ddlFund_0")).SelectByText("A Test Fund");
            test.Driver.FindElementById("btn_submit").Click();

            // Verify error displayed to the user
            test.GeneralMethods.VerifyTextPresentWebDriver(TextConstants.ErrorHeadingSingular);
            test.GeneralMethods.VerifyTextPresentWebDriver("Received date (1/5/1515) is not a valid date.");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the user has the option to split an RDC Batch.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Splits_Available()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View a pending batch
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(_rdcBatchPendingSplit, true);

            // Verify the correct text is present
            //test.GeneralMethods.VerifyTextPresentWebDriver("This batch is being processed as a single Fund/Subfund, or Pledge.");
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("This batch is being processed as a single Fund/Subfund, or Pledge."));

            // Verify the split option is present
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("Process this batch as splits"));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        //[Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a fund is required when saving an RDC batch.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Fund_Required()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to giving->batches->remote deposit capture
            test.GeneralMethods.Navigate_Portal(Navigation.Giving.Contributions.Batches_RemoteDepositCapture);

            // View a pending batch
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(_rdcBatchPendingSplit, true);

            // Submit
            test.Driver.FindElementById("btn_submit").Click();

            // Verify validation
            test.GeneralMethods.VerifyTextPresentWebDriver("You must specify a fund for this batch.");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        //[Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a received date is required when saving an RDC batch.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_ReceivedDate_Required()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to giving->batches->remote deposit capture
            test.GeneralMethods.Navigate_Portal(Navigation.Giving.Contributions.Batches_RemoteDepositCapture);

            // View a pending batch
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(_rdcBatchPendingSplit);

            // Enter no date for the received date
            test.Driver.FindElementById("rdc_date").Clear();

            // Submit
            test.Driver.FindElementById("btn_submit").Click();

            // Verify validation
            test.GeneralMethods.VerifyTextPresentWebDriver("You must specify a received date for this batch.");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        #endregion Save RDC Batch

        #region Matching - Single

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies that the option to Share an RDC Batch item is present when matching individuals")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Matching_Shared()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View a batch item and click match
            test.Portal.Giving_Batches_RemoteDepositCapture_Match_RDCBatchItem_View_WebDriver(_rdcBatchPending, "1.00");

            // Verify Skip this check is present
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id("shared_account"));
            test.Driver.FindElementByLinkText(GeneralLinksWebDriver.RETURN).Click();

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you are can Skip an RDC Batch item when matching individuals")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Matching_Skip()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View a batch item and click match
            test.Portal.Giving_Batches_RemoteDepositCapture_Match_RDCBatchItem_View_WebDriver(_rdcBatchPending, "1.00");

            // Verify Skip this check is present
            try
            {
                test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("Skip this check"));
            }
            catch (Exception e)
            {
                test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("Skip this item"));
            }


            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies you can search and select an Inactive individual and match them to an RDC Batch item.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Matching_Search_Inactive_IND()
        {
            // Set initial conditions
            var rdcBatchName = "Test RDC Batch - Pending - Inactive";
            var individualName = "Testing InactiveUser";
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 6.25 }, null, new System.Collections.Generic.List<string> { _rdcBatchAccountNumberInd }, new System.Collections.Generic.List<string>() { _rdcBatchRoutingNumberInd.ToString() });

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Match an RDC Batch item to an Inactive individual
            test.Portal.Giving_Batches_RemoteDepositCapture_Match_RDCBatchItem_WebDriver(rdcBatchName, "6.25", individualName, false, false, true);

            // Save Batch
            new SelectElement(test.Driver.FindElementById("ddlFundPledgeDrive_0")).SelectByText("1 - General Fund");
            test.Driver.FindElementById("btn_submit").Click();

            // Verify
            Assert.AreEqual(individualName, test.Driver.FindElementByXPath("//table[@id=ctl00_content_RDCBatchItemList_grdBatcheItems]/tbody/tr[2]/td[2]/a").Text, "Name does not match");
            // Assert.AreEqual("–", test.Selenium.GetText("//table[@class='grid select_row']/tbody/tr/td[3]"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies we do not show an age, if someone does not have a date of birth, when searching for someone to match to an RDC Batch Item.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Matching_Search_Age_Not_Present_When_No_DOB()
        {
            // Set initial conditions
            StringBuilder query = new StringBuilder("SELECT TOP 1 (IND.FIRST_NAME + ' ' + IND.LAST_NAME), STAT.STATUS_NAME FROM ChmPeople.dbo.INDIVIDUAL AS IND WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmPeople.dbo.STATUS AS STAT WITH (NOLOCK) ");
            query.Append("ON IND.STATUS_ID = STAT.STATUS_ID ");
            query.Append("WHERE IND.CHURCH_ID = 15 AND IND.DATE_OF_BIRTH IS NULL AND (IND.FIRST_NAME+IND.LAST_NAME) not like '% %'");
            DataTable results = base.SQL.Execute(query.ToString());
            string individualName = results.Rows[0][0].ToString();

            TestLog.WriteLine("Individual Name: " + individualName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Match an RDC Batch item and search for someone without a date of birth
            test.Portal.Giving_Batches_RemoteDepositCapture_Match_RDCBatchItem_WebDriver(_rdcBatchPending, "1.00", individualName, false, false, false);

            // Verify
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("find_individual_results"), 200);

            for (int r = 1; r < 4; r++)
            {
                TestLog.WriteLine(string.Format("[{0}]: {1}", r, test.Driver.FindElementByXPath(string.Format("//table[@class='grid select_row']/tbody/tr/td[{0}]", r)).Text));
            }

            #region verify individual in search result table
            //ivan.zhang modification.
            string tablexpath = "//table[@class='grid select_row']";
            int rowcounts = test.GeneralMethods.GetTableRowCountWebDriver(tablexpath);
            IWebElement table = test.Driver.FindElementByXPath(tablexpath);
            bool ExistingFlag = false;
            //IWebElement tdNameElement = test.Driver.FindElementById("find_individual_results").FindElement(By.TagName("table")).FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[1];
            //String actualValue = tdNameElement.Text;

            //traversal the return table
            for (int i = 1; i < rowcounts; i++)
            {
                string namevalue = table.FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"))[i].FindElements(By.TagName("td"))[1].Text;
                if (namevalue == string.Format("{0}\r\n{1}", individualName, results.Rows[0][1]))
                {
                    string DOBvalue = table.FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"))[i].FindElements(By.TagName("td"))[2].Text;
                    if (DOBvalue == "–")
                    {
                        ExistingFlag = true;
                        break;
                    }
                }
            }
            Assert.IsTrue(ExistingFlag, "The specify user doesn't exist in search result table.");

            #endregion 
            
            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the substatus is not present, if someone does not have a substatus, when searching for an individual to match to an RDC Batch item.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Matching_Search_No_Substatus()
        {
            // Set initial conditions
            StringBuilder query = new StringBuilder("SELECT TOP 1 (IND.FIRST_NAME + ' ' + IND.LAST_NAME), STAT.STATUS_NAME, DATEDIFF(day, IND.DATE_OF_BIRTH, CURRENT_TIMESTAMP)/365 AS DATE_OF_BIRTH FROM ChmPeople.dbo.INDIVIDUAL AS IND WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmPeople.dbo.STATUS AS STAT WITH (NOLOCK) ");
            query.Append("ON IND.STATUS_ID = STAT.STATUS_ID ");
            query.Append("WHERE IND.CHURCH_ID = 15 AND IND.DATE_OF_BIRTH IS NOT NULL AND IND.STATUS_SELECTLIST_ID IS NULL AND (IND.FIRST_NAME+IND.LAST_NAME) not like '% %'");
            DataTable results = base.SQL.Execute(query.ToString());
            string individualName = results.Rows[0][0].ToString();

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Match an RDC Batch item and search for someone without a substatus
            test.Portal.Giving_Batches_RemoteDepositCapture_Match_RDCBatchItem_WebDriver(_rdcBatchPending, "1.00", individualName, false, false, false);

            // Verify
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("find_individual_results"), 200);

            for (int r = 1; r < 4; r++)
            {
                TestLog.WriteLine(string.Format("[{0}]: {1}", r, test.Driver.FindElementByXPath(string.Format("//table[@class='grid select_row']/tbody/tr/td[{0}]", r)).Text));
            }

            Assert.AreEqual(string.Format("{0}\r\n{1}", individualName, results.Rows[0][1]), test.Driver.FindElementByXPath("//table[@class='grid select_row']/tbody/tr/td[2]").Text);
            Assert.AreNotEqual(string.Format("{0}\r\n{1} >", individualName, results.Rows[0][1]), test.Driver.FindElementByXPath("//table[@class='grid select_row']/tbody/tr/td[2]").Text);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the status, substatus, and age display when searching for someone to match to an RDC Batch Item.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Matching_Search_Status_SubStatus_And_Age_Present()
        {
            // Set initial conditions
            StringBuilder query = new StringBuilder("SELECT TOP 1 (IND.FIRST_NAME + '  ' + IND.LAST_NAME), STAT.STATUS_NAME, STATSEL.LIST_NAME, DATEDIFF(day, IND.DATE_OF_BIRTH, CURRENT_TIMESTAMP)/365 AS DATE_OF_BIRTH FROM ChmPeople.dbo.INDIVIDUAL AS IND WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmPeople.dbo.STATUS AS STAT WITH (NOLOCK) ");
            query.Append("ON IND.STATUS_ID = STAT.STATUS_ID ");
            query.Append("INNER JOIN ChmPeople.dbo.STATUS_SELECTLIST AS STATSEL WITH (NOLOCK) ");
            query.Append("ON IND.STATUS_SELECTLIST_ID = STATSEL.STATUS_SELECTLIST_ID ");
            query.Append("WHERE IND.CHURCH_ID = 15 AND IND.DATE_OF_BIRTH IS NOT NULL AND (IND.FIRST_NAME+IND.LAST_NAME) not like '% %'");
            DataTable results = base.SQL.Execute(query.ToString());
            string individualName = results.Rows[0][0].ToString();

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Match an RDC Batch item and search for someone with status, substatus and date of birth
            test.Portal.Giving_Batches_RemoteDepositCapture_Match_RDCBatchItem_WebDriver(_rdcBatchPending, "1.00", individualName, false, false, false);

            // Search and verify the results
            test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@id='find_individual_results']/table/tbody/tr[2]/td[1]/button"));
            TestLog.WriteLine(string.Format("This is the name text - ( {0} )", test.Driver.FindElementByXPath("//div[@id='find_individual_results']/table/tbody/tr/td[2]").Text));
            TestLog.WriteLine(string.Format("This is the age text - ( {0} )", test.Driver.FindElementByXPath("//div[@id='find_individual_results']/table/tbody/tr/td[3]").Text));
            //Assert.AreEqual(string.Format("{0}\r\n{1} > {2}", individualName, results.Rows[0][1], results.Rows[0][2]), test.Driver.FindElementByXPath("//div[@id='find_individual_results']/table/tbody/tr/td[2]").Text);
            //Assert.AreEqual(string.Format("{0} yrs.", results.Rows[0]["DATE_OF_BIRTH"]), test.Driver.FindElementByXPath("//div[@id='find_individual_results']/table/tbody/tr/td[3]").Text);
            
            //Updated By Jim: To adapt the stuation that more than one individules have same full name
            bool equal = false;
            string expectedResult = string.Format("{0}\r\n{1} > {2}", individualName, results.Rows[0][1], results.Rows[0][2]).Replace("  ", " ");
            foreach (IWebElement element in test.Driver.FindElementsByXPath(string.Format("//td[contains(text(),'{0}')]", individualName)))
            {
                equal = equal || (expectedResult == element.Text);
                if (expectedResult == element.Text)
                {
                    string ageText = test.Driver.FindElementByXPath(string.Format("//td[contains(text(),'{0}')]/following-sibling::td[1]", individualName)).Text;
                    Assert.AreEqual(string.Format("{0} yrs.", results.Rows[0]["DATE_OF_BIRTH"]), ageText);
                }
            }
            Assert.IsTrue(equal, "Specific individual is not found in the list!");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that an RDC Batch Item automatches to a person if they are the only person with the account number.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Automatch_Individual_Single_Account()
        {
            // Set initial conditions
            var account = Guid.NewGuid().ToString().Substring(0, 5);
            var rdcBatchName = "Test RDC Batch - Pending - Automatch";
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 6.25 }, new System.Collections.Generic.List<string>() { "Bryan Mikaelian" }, new System.Collections.Generic.List<string> { _rdcBatchAccountNumberInd }, new System.Collections.Generic.List<string>() { _rdcBatchRoutingNumberInd.ToString() });

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View an RDC Batch that has an RDC Batch Item that matched to only one person
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName, true);

            // Verify that the RDC batch item is auto matched to the individual
            var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$6.25", "Amount") + 1;
            Assert.AreEqual("Bryan Mikaelian", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreNotEqual("Unmatched", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("$6.25", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            // Verify the multiple matches UI is not present.
            test.Driver.FindElementByLinkText("Match").Click();
            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver("Multiple people and/or organizations have this account. Pick one to match:"));

            // Verify only one person has this account
            Assert.AreEqual(1, base.SQL.Execute(string.Format("SELECT COUNT(*) FROM ChmContribution.dbo.Account WHERE CHURCH_ID = 15 and Account = '{0}'", _rdcBatchAccountNumberInd)).Rows.Count);

            // Logout
            test.Portal.LogoutWebDriver();

            // Clean up
            //base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that an RDC Batch Item shows multiple match if they are there are more than one person with the account number.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Automatch_Individual_Multiple_Account()
        {
            // Set initial conditions
            string individualName1 = "Matthew Sneeden";
            string individualName2 = "Mark Lindsley";
            var rdcBatchName = "Test RDC Batch - Pending - Automatch - Multi";
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 6.25 }, null, new System.Collections.Generic.List<string> { _rdcBatchAccountNumberShared }, new System.Collections.Generic.List<string>() { _rdcBatchRoutingNumberShared.ToString() });

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View an RDC Batch that has an RDC Batch Item that matched to multiple people
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName, false);

            // Verify that the RDC batch item is auto matched to the individual
            var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$6.25", "Amount") + 1;
            Assert.AreNotEqual(individualName1, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreNotEqual(individualName2, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("Unmatched", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("$6.25", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            // Verify the multiple matches UI is present.
            test.Driver.FindElementByLinkText("Match").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Change"), 30, "Timed out waiting for AutoMatch list");
            test.GeneralMethods.VerifyTextPresentWebDriver("Multiple people and/or organizations have this account. Pick one to match:");

            // Verify the individuals that have this account can be picked
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Giving_RDC_PotentialMatches, individualName1, "Name", "contains"));
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Giving_RDC_PotentialMatches, individualName2, "Name", "contains"));
            var ind1Row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_IndividualMatches, individualName1, "Name", "contains") + 1;
            var ind2Row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_IndividualMatches, individualName2, "Name", "contains") + 1;
            Assert.Contains(test.Driver.FindElementByXPath(string.Format("//div[@id='find_individual_results']/table/tbody/tr[{0}]/td[2]", ind1Row)).Text, individualName1, individualName1 + " not present");
            Assert.Contains(test.Driver.FindElementByXPath(string.Format("//div[@id='find_individual_results']/table/tbody/tr[{0}]/td[2]", ind2Row)).Text, individualName2, individualName2 + " not present");

            // Verify more than one person has this account
            Assert.AreEqual(2, base.SQL.Execute(string.Format("SELECT COUNT(*) FROM ChmContribution.dbo.Account WHERE CHURCH_ID = 15 and ACCOUNT = '{0}'", _rdcBatchAccountNumberShared)).Rows[0][0]);

            // Logout
            test.Portal.LogoutWebDriver();

            // Clean up
            //base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that an RDC Batch Item shows no matches if there are no individuals with the account number or routing number.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Automatch_No_Match()
        {
            // Set initial conditions
            var account = Guid.NewGuid().ToString().Substring(0, 5);
            var rdcBatchName = "Test RDC Batch - Pending - Automatch - None";
            base.SQL.Execute(string.Format("DELETE FROM ChmContribution.dbo.ACCOUNT WHERE CHURCH_ID = 15 and ACCOUNT = '{0}'", account));
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 6.25 }, null, new System.Collections.Generic.List<string> { account }, new System.Collections.Generic.List<string>() { "10000345" });

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View an RDC Batch that has an RDC Batch Item that matched to multiple people
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName, true);

            // Verify that the RDC batch item is auto matched to the individual
            var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$6.25", "Amount") + 1;
            Assert.AreEqual("Unmatched", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("$6.25", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            // Verify the search UI is present.
            test.Driver.FindElementByLinkText("Match").Click();
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id("find_individual_name"));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id("find_individual_address"));

            // Verify the table for potential matches is not present
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.XPath(TableIds.Giving_RDC_PotentialMatches));

            // Verify no one has this account
            Assert.AreEqual(0, base.SQL.Execute(string.Format("SELECT COUNT(*) FROM ChmContribution.dbo.Account WHERE CHURCH_ID = 15 and ACCOUNT = '{0}'", account)).Rows[0][0]);

            // Logout
            test.Portal.LogoutWebDriver();

            // Clean up
            //base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that an RDC Batch Item automatches to an organization if they are the only organization with the account number.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Automatch_Organization_Single_Account()
        {
            // Set initial conditions
            var rdcBatchName = "Test RDC Batch - Pending - Automatch Org";
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 6.25 }, new System.Collections.Generic.List<string>() { _organizationSingle }, new System.Collections.Generic.List<string> { _rdcBatchAccountNumberOrg }, new System.Collections.Generic.List<string>() { _rdcBatchRoutingNumberOrg.ToString() }, true);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View an RDC Batch that has an RDC Batch Item that matched to only one person
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName);

            // Verify that the RDC batch item is auto matched to the individual
            var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$6.25", "Amount") + 1;
            Assert.AreEqual(_organizationSingle, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreNotEqual("Unmatched", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("$6.25", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            // Verify the multiple matches UI is not present.
            test.Driver.FindElementByLinkText("Match").Click();
            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver("Multiple people and/or organizations have this account. Pick one to match:"));

            // Verify the UI elements for an organization
            test.GeneralMethods.VerifyTextPresentWebDriver("Selected organization");
            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//a[@class='change_org']"));

            // Verify only one organization has this account
            Assert.AreEqual(1, base.SQL.Execute(string.Format("SELECT COUNT(*) FROM ChmContribution.dbo.Account WHERE CHURCH_ID = 15 and ACCOUNT = '{0}'", _rdcBatchAccountNumberOrg)).Rows.Count);

            // Logout
            test.Portal.LogoutWebDriver();

            // Clean up
            //base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that an RDC Batch Item shows multiple match if there is more than one organization with the account number.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Automatch_Organization_Multiple_Account()
        {
            // Set initial conditions
            var rdcBatchName = "Test RDC Batch - Pending - Automatch Org - Multi";
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 9.25 }, null, new System.Collections.Generic.List<string> { _rdcBatchAccountNumberOrgShared }, new System.Collections.Generic.List<string>() { _rdcBatchRoutingNumberSharedOrg.ToString() }, true);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View an RDC Batch that has an RDC Batch Item that matched to multiple people
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName);

            // Verify that the RDC batch item is auto matched to an organization
            var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$9.25", "Amount") + 1;
            Assert.AreNotEqual(_organizationShared1, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreNotEqual(_organizationShared2, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("Unmatched", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("$9.25", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            // Verify the multiple matches UI is present.
            test.Driver.FindElementByLinkText("Match").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Change"), 30, "Timed out waiting for AutoMatch list");
            test.GeneralMethods.VerifyTextPresentWebDriver("Multiple people and/or organizations have this account. Pick one to match:");

            // Verify the organizations that have this account can be picked
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Giving_RDC_PotentialMatches, _organizationShared1, "Name", "contains"));
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Giving_RDC_PotentialMatches, _organizationShared2, "Name", "contains"));
            var org1Row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_IndividualMatches, _organizationShared1, "Name", "contains") + 1;
            var org2Row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_IndividualMatches, _organizationShared2, "Name", "contains") + 1;
            Assert.Contains(test.Driver.FindElementByXPath(string.Format("//div[@id='find_individual_results']/table/tbody/tr[{0}]/td[2]", org1Row)).Text, _organizationShared1, _organizationShared1 + " not present");
            Assert.Contains(test.Driver.FindElementByXPath(string.Format("//div[@id='find_individual_results']/table/tbody/tr[{0}]/td[2]", org2Row)).Text, _organizationShared2, _organizationShared2 + " not present");

            // Verify more than one organization has this account
            Assert.AreEqual(2, base.SQL.Execute(string.Format("SELECT COUNT(*) FROM ChmContribution.dbo.Account WHERE CHURCH_ID = 15 and ACCOUNT = '{0}'", _rdcBatchAccountNumberOrgShared)).Rows[0][0]);

            // Logout
            test.Portal.LogoutWebDriver();

            // Clean up
            //base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);

        }

        #endregion Matching - Single

        #region Matching - Splits

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you are can go to the next check in an RDC Batch item, under a split RDC Batch, when matching individuals")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Splits_Matching_Next()
        {
            // Initial Data
            var rdcBatchPendingSplit = "Test RDC Batch - Pending - Split - Search 1";
            base.SQL.Giving_Batches_Delete(15, rdcBatchPendingSplit, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchPendingSplit, false, new System.Collections.Generic.List<double>() { 6.25 });

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View a batch
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchPendingSplit);

            // Process as splits
            test.Driver.FindElementByLinkText("Process this batch as splits").Click();

            // Verify Next check is present
            //changed from Next check to Next item
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("Next item"));

            // Logout of portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Giving_Batches_Delete(15, rdcBatchPendingSplit, 3);
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies we do not show an age, if someone does not have a date of birth, when searching for someone to match to an RDC Batch Item.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Splits_Matching_Search_Age_Not_Present_When_No_DOB()
        {
            // Set initial conditions
            StringBuilder query = new StringBuilder("SELECT TOP 1 (IND.FIRST_NAME + ' ' + IND.LAST_NAME), STAT.STATUS_NAME FROM ChmPeople.dbo.INDIVIDUAL AS IND WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmPeople.dbo.STATUS AS STAT WITH (NOLOCK) ");
            query.Append("ON IND.STATUS_ID = STAT.STATUS_ID ");
            query.Append("WHERE IND.CHURCH_ID = 15 AND IND.DATE_OF_BIRTH IS NULL AND (IND.FIRST_NAME+IND.LAST_NAME) not like '% %'");
            DataTable results = base.SQL.Execute(query.ToString());
            string individualName = results.Rows[0][0].ToString();
            var rdcBatchPendingSplit = "Test RDC Batch - Pending - Split - Search 2";
            base.SQL.Giving_Batches_Delete(15, rdcBatchPendingSplit, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchPendingSplit, false, new System.Collections.Generic.List<double>() { 6.25 });

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Match an RDC Batch item being processed as a split batch and search for someone without a date of birth
            test.Portal.Giving_Batches_RemoteDepositCapture_Match_RDCBatchItem_WebDriver(rdcBatchPendingSplit, "1.00", individualName, false, false, false, true);

            // Verify
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath("//table[@class='grid select_row']"), 90);
           
            #region verify individual in search result table
            //ivan.zhang modification.
            string tablexpath = "//table[@class='grid select_row']";
            int rowcounts = test.GeneralMethods.GetTableRowCountWebDriver(tablexpath);
            IWebElement table = test.Driver.FindElementByXPath(tablexpath);
            bool ExistingFlag = false;
            //IWebElement tdNameElement = test.Driver.FindElementById("find_individual_results").FindElement(By.TagName("table")).FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[1];
            //String actualValue = tdNameElement.Text;

            //traversal the return table
            for (int i = 1; i < rowcounts; i++)
            {
                string namevalue = table.FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"))[i].FindElements(By.TagName("td"))[1].Text;
                if (namevalue == string.Format("{0}\r\n{1}", individualName, results.Rows[0][1]))
                {
                    string DOBvalue = table.FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"))[i].FindElements(By.TagName("td"))[2].Text;
                    if (DOBvalue == "–")
                    {
                        ExistingFlag = true;
                        break;
                    }
                }
            }
            Assert.IsTrue(ExistingFlag, "The specify user doesn't exist in search result table.");

            #endregion 

            // Logout of portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Giving_Batches_Delete(15, rdcBatchPendingSplit, 3);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the substatus is not present, if someone does not have a substatus, when searching for an individual to match to an RDC Batch item being processed as a split.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Splits_Matching_Search_No_Substatus()
        {
            // Set initial conditions
            StringBuilder query = new StringBuilder("SELECT TOP 1 (IND.FIRST_NAME + ' ' + IND.LAST_NAME), STAT.STATUS_NAME, DATEDIFF(day, IND.DATE_OF_BIRTH, CURRENT_TIMESTAMP)/365 AS DATE_OF_BIRTH FROM ChmPeople.dbo.INDIVIDUAL AS IND WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmPeople.dbo.STATUS AS STAT WITH (NOLOCK) ");
            query.Append("ON IND.STATUS_ID = STAT.STATUS_ID ");
            query.Append("WHERE IND.CHURCH_ID = 15 AND IND.DATE_OF_BIRTH IS NOT NULL AND IND.STATUS_SELECTLIST_ID IS NULL AND (IND.FIRST_NAME+IND.LAST_NAME) not like '% %'");
            DataTable results = base.SQL.Execute(query.ToString());
            string individualName = results.Rows[0][0].ToString();
            var rdcBatchPendingSplit = "Test RDC Batch - Pending - Split - Search 3";
            base.SQL.Giving_Batches_Delete(15, rdcBatchPendingSplit, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchPendingSplit, false, new System.Collections.Generic.List<double>() { 6.25 });

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Match an RDC Batch item being processed as a split batch and search for someone without a substatus
            test.Portal.Giving_Batches_RemoteDepositCapture_Match_RDCBatchItem_WebDriver(rdcBatchPendingSplit, "1.00", individualName, false, false, false, true);

            // Verify:modify verify logic by grace zhang
            Thread.Sleep(1 * 60 * 1000);
            bool flagEqual = false;
            bool flagNotEqual = true;
            bool firstEqualMatch = false;
            bool firstNotEqualMatch = false;
            String expectEqualInfo = string.Format("{0}\r\n{1}", individualName, results.Rows[0][1]);
            String expectNotEqualInfo = string.Format("{0}\r\n{1} >", individualName, results.Rows[0][1]);
            TestLog.WriteLine("expectEqualInfo==" + expectEqualInfo);
            TestLog.WriteLine("expectNotEqualInfo==" + expectNotEqualInfo);
            int trCount = test.Driver.FindElementByXPath("//table[@class='grid select_row']/tbody").FindElements(By.TagName("tr")).Count;
            for (int i = 1; i < trCount; i++)
            {

                String actualInfo = test.Driver.FindElementByXPath("//table[@class='grid select_row']/tbody").FindElements(By.TagName("tr"))[i].FindElements(By.TagName("td"))[1].Text.Trim();
                if (actualInfo.Equals(expectEqualInfo) && !firstEqualMatch)
                {
                    flagEqual = true;
                    firstEqualMatch = true;
                }
                if (actualInfo.Equals(expectNotEqualInfo) && !firstNotEqualMatch)
                {
                    flagNotEqual = false;
                    firstNotEqualMatch = true;
                }
            }
            Assert.IsTrue(flagEqual);
            Assert.IsTrue(flagNotEqual);

            //Assert.AreEqual(string.Format("{0}\r\n{1}", individualName, results.Rows[0][1]), test.Driver.FindElementByXPath("//table[@class='grid select_row']/tbody/tr/td[2]").Text);
            //Assert.AreNotEqual(string.Format("{0}\r\n{1} >", individualName, results.Rows[0][1]), test.Driver.FindElementByXPath("//table[@class='grid select_row']/tbody/tr/td[2]").Text);

            // Logout of portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Giving_Batches_Delete(15, rdcBatchPendingSplit, 3);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies, while processing an RDC batch as splits, that an RDC Batch Item automatches to a person if they are the only person with the account number.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Splits_Automatch_Individual_Single_Account()
        {
            // Set initial conditions
            var rdcBatchName = "Test RDC Batch - Pending Split - Automatch";
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 7.25 }, new System.Collections.Generic.List<string>() { "Bryan Mikaelian" }, new System.Collections.Generic.List<string> { _rdcBatchAccountNumberInd }, new System.Collections.Generic.List<string>() { _rdcBatchRoutingNumberInd.ToString() });

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View an RDC Batch that has an RDC Batch Item that matched to only one person
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName, true);

            // Verify that the RDC batch item is auto matched to the individual
            var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$7.25", "Amount") + 1;
            Assert.AreEqual("Bryan Mikaelian", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreNotEqual("Unmatched", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("$7.25", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            // Split the RDC Batch
            test.Driver.FindElementByLinkText("Process this batch as splits").Click();

            // Verify the multiple matches UI is not present.
            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver("Multiple people and/or organizations have this account. Pick one to match:"));

            // Verify only one person has this account
            Assert.AreEqual(1, base.SQL.Execute(string.Format("SELECT COUNT(*) FROM ChmContribution.dbo.Account WHERE CHURCH_ID = 15 and Account = '{0}'", _rdcBatchAccountNumberInd)).Rows.Count);

            // Return
            //test.Driver.FindElementByLinkText(GeneralLinksWebDriver.RETURN);
            test.Driver.FindElementByCssSelector("[class='minimal_return_arrow']").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(TableIds.Giving_RDC_RDCBatchItemList));

            // Verify that the RDC batch item is auto matched to the individual
            row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$7.25", "Amount") + 1;
            Assert.AreEqual("Bryan Mikaelian", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreNotEqual("Unmatched", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("$7.25", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            // Logout
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies, while processing an RDC batch as splits, that an RDC Batch Item shows multiple match if they are there are more than one person with the account number.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Splits_Automatch_Individual_Multiple_Account()
        {
            // Set initial conditions
            string individualName1 = "Matthew Sneeden";
            string individualName2 = "Mark Lindsley";
            var rdcBatchName = "Test RDC Batch - Pending Splits - Automatch - Mult";
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 6.25 }, null, new System.Collections.Generic.List<string> { _rdcBatchAccountNumberShared }, new System.Collections.Generic.List<string>() { _rdcBatchRoutingNumberShared.ToString() });

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View an RDC Batch that has an RDC Batch Item that matched to multiple people
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName);

            // Verify that the RDC batch item is not auto matched to the individual
            var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$6.25", "Amount") + 1;
            Assert.AreNotEqual(individualName1, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreNotEqual(individualName2, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("Unmatched", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("$6.25", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            // Split the RDC Batch
            test.Driver.FindElementByLinkText("Process this batch as splits").Click();

            // Verify the multiple matches UI is present.
            test.GeneralMethods.VerifyTextPresentWebDriver("Multiple people and/or organizations have this account. Pick one to match:");

            // Verify the individuals that have this account can be picked
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Giving_RDC_PotentialMatches, individualName1, "Name", "contains"));
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Giving_RDC_PotentialMatches, individualName2, "Name", "contains"));
            var ind1Row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_IndividualMatches, individualName1, "Name", "contains") + 1;
            var ind2Row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_IndividualMatches, individualName2, "Name", "contains") + 1;
            Assert.Contains(test.Driver.FindElementByXPath(string.Format("//div[@id='find_individual_results']/table/tbody/tr[{0}]/td[2]", ind1Row)).Text, individualName1, individualName1 + " not present");
            Assert.Contains(test.Driver.FindElementByXPath(string.Format("//div[@id='find_individual_results']/table/tbody/tr[{0}]/td[2]", ind2Row)).Text, individualName2, individualName2 + " not present");


            // Verify more than one person has this account
            Assert.AreEqual(2, base.SQL.Execute(string.Format("SELECT COUNT(*) FROM ChmContribution.dbo.Account WHERE CHURCH_ID = 15 and ACCOUNT = '{0}'", _rdcBatchAccountNumberShared)).Rows[0][0]);

            // Logout
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies, while processing an RDC batch as splits, that an RDC Batch Item shows no matches if there are no individuals with the account number or routing number.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Splits_Automatch_No_Match()
        {
            // Set initial conditions
            var account = Guid.NewGuid().ToString().Substring(0, 5);
            var rdcBatchName = "Test RDC Batch - Pending Splits - Automatch - None";
            base.SQL.Execute(string.Format("DELETE FROM ChmContribution.dbo.ACCOUNT WHERE CHURCH_ID = 15 and ACCOUNT = '{0}'", account));
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 6.25 }, null, new System.Collections.Generic.List<string> { account }, new System.Collections.Generic.List<string>() { "10000345" });

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View an RDC Batch that has an RDC Batch Item that matched to multiple people
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName);

            // Verify that the RDC batch item is not auto matched to any individual
            var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$6.25", "Amount") + 1;
            Assert.AreEqual("Unmatched", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("$6.25", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            // Split the RDC Batch
            test.Driver.FindElementByLinkText("Process this batch as splits").Click();

            // Verify the search UI is present.
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id("find_individual_name"));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id("find_individual_address"));

            // Verify the table for potential matches is not present
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.XPath(TableIds.Giving_RDC_PotentialMatches));

            // Verify no one has this account
            Assert.AreEqual(0, base.SQL.Execute(string.Format("SELECT COUNT(*) FROM ChmContribution.dbo.Account WHERE CHURCH_ID = 15 and ACCOUNT = '{0}'", account)).Rows[0][0]);

            // Logout
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that an RDC Batch Item automatches to an organization if they are the only organization with the account number.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Splits_Automatch_Organization_Single_Account()
        {
            // Set initial conditions
            var rdcBatchName = "Test RDC Batch - Pending Splits - Auto Org";
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 6.25 }, new System.Collections.Generic.List<string>() { _organizationSingle }, new System.Collections.Generic.List<string> { _rdcBatchAccountNumberOrg }, new System.Collections.Generic.List<string>() { _rdcBatchRoutingNumberOrg.ToString() }, true);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View an RDC Batch that has an RDC Batch Item that matched to only one person
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName);

            // Verify that the RDC batch item is auto matched to the individual
            var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$6.25", "Amount") + 1;
            Assert.AreEqual(_organizationSingle, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreNotEqual("Unmatched", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("$6.25", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            // Split the RDC Batch
            test.Driver.FindElementByLinkText("Process this batch as splits").Click();

            // Verify the multiple matches UI is not present.
            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver("Multiple people and/or organizations have this account. Pick one to match:"));

            // Verify the UI elements for an organization
            test.GeneralMethods.VerifyTextPresentWebDriver("Selected organization");
            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//a[@class='change_org']"));

            // Verify only one organization has this account
            Assert.AreEqual(1, base.SQL.Execute(string.Format("SELECT COUNT(*) FROM ChmContribution.dbo.Account WHERE CHURCH_ID = 15 and ACCOUNT = '{0}'", _rdcBatchAccountNumberOrg)).Rows.Count);

            // Logout
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that an RDC Batch Item shows multiple match if there is more than one organization with the account number.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Splits_Automatch_Organization_Multiple_Account()
        {
            // Set initial conditions
            var rdcBatchName = "Test RDC Batch - Splits - Auto Mult Org";
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 9.25 }, null, new System.Collections.Generic.List<string> { _rdcBatchAccountNumberOrgShared }, new System.Collections.Generic.List<string>() { _rdcBatchRoutingNumberSharedOrg.ToString() }, true);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View an RDC Batch that has an RDC Batch Item that matched to multiple people
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName, true);

            // Verify that the RDC batch item is auto matched to an organization
            var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$9.25", "Amount") + 1;
            Assert.AreNotEqual(_organizationShared1, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreNotEqual(_organizationShared2, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("Unmatched", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("$9.25", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            // Split the RDC Batch
            test.Driver.FindElementByLinkText("Process this batch as splits").Click();

            // Verify the multiple matches UI is present.
            test.GeneralMethods.VerifyTextPresentWebDriver("Multiple people and/or organizations have this account. Pick one to match:");

            // Verify the organizations that have this account can be picked
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Giving_RDC_PotentialMatches, _organizationShared1, "Name", "contains"));
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Giving_RDC_PotentialMatches, _organizationShared2, "Name", "contains"));
            var org1Row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_IndividualMatches, _organizationShared1, "Name", "contains") + 1;
            var org2Row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_IndividualMatches, _organizationShared2, "Name", "contains") + 1;
            Assert.Contains(test.Driver.FindElementByXPath(string.Format("//div[@id='find_individual_results']/table/tbody/tr[{0}]/td[2]", org1Row)).Text, _organizationShared1, _organizationShared1 + " not present");
            Assert.Contains(test.Driver.FindElementByXPath(string.Format("//div[@id='find_individual_results']/table/tbody/tr[{0}]/td[2]", org2Row)).Text, _organizationShared2, _organizationShared2 + " not present");

            // Verify more than one organization has this account
            Assert.AreEqual(2, base.SQL.Execute(string.Format("SELECT COUNT(*) FROM ChmContribution.dbo.Account WHERE CHURCH_ID = 15 and ACCOUNT = '{0}'", _rdcBatchAccountNumberOrgShared)).Rows[0][0]);

            // Logout
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);

        }


        #endregion  Matching - Splits

        #endregion Pending


        #region Saved

        #region Audit

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("F1-3376 Consolidated Batches Screen: Verifies that imported batch is viewed using gear option")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Saved_Audit_GearOption()
        {

            // Create an RDC Batch in a pending state without a matched person
            string importBatchName = "Test RDC Batch - AuditGear - Pending";
            base.SQL.Giving_Batches_Delete(15, importBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, importBatchName, false, new System.Collections.Generic.List<double>() { 55.55 });

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View the audit page for an rdc batch in the saved state
            test.Portal.Giving_Batches_ImportedBatches_View_Audit_WebDriver(importBatchName);

            base.SQL.Giving_Batches_Delete(15, importBatchName, 3);


        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user cannot edit a batch item using an invalid received date.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Saved_Audit()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View the audit page for an rdc batch in the saved state
            test.Portal.Giving_Batches_ImportedBatches_View_Audit_WebDriver(_rdcBatchSaved);

            // Verify user is taken to the audit page
            Assert.AreEqual("Fellowship One :: Remote Deposit Capture Batches Audit", test.Driver.Title);
            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//div[@id='return_to_app']"));
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.XPath("//div[@id='nav']"));
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.XPath("//p[@id='breadcrumb']"));

            // Verify download pdf link, gear is not available for pending batch
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("Download PDF"));
            //Assert.IsTrue(test.Selenium.IsVisible("link=Download PDF"));

            // Verify table data
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//table[@class='bccp_audit']/tbody/tr[1]/td[3]/span[text()='Automated System Event']")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//table[@class='bccp_audit']/tbody/tr[1]/td[position()=4 and normalize-space(text())='imported batch from remote deposit source']")));
            //Assert.IsTrue(test.Selenium.IsElementPresent("//table[@class='bccp_audit']/tbody/tr[position() > 1]/td[4]/ul/li[position()=1 and text()='saved the batch...']"));  // Verify in a new test

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        #endregion Audit

        #region Edit

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user cannot edit a batch item using an invalid received date.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Saved_Edit_SaveInvalidDate()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View the save page for an rdc batch in the saved state
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(_rdcBatchSaved);

            // Edit a batch item
            test.Driver.FindElementByLinkText("Edit").Click();

            // Verify the date control
            test.Driver.FindElementById("rdc_date").Clear();
            test.Driver.FindElementById("rdc_date").SendKeys("151515");
            test.Driver.FindElementById("rdc_date").SendKeys(Keys.ArrowDown);
            test.Driver.FindElementById("rdc_date").SendKeys(Keys.Tab);
            //test.Selenium.Focus("btn_submit");
            new SelectElement(test.Driver.FindElementById("ddlFund_0")).SelectByText("1 - General Fund");
            test.Driver.FindElementById("btn_submit").Click();


            // Verify error displayed to the user
            test.GeneralMethods.VerifyTextPresentWebDriver(TextConstants.ErrorHeadingSingular);
            test.GeneralMethods.VerifyTextPresentWebDriver("Received date (1/5/1515) is not a valid date.");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }


        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user is not allowed to edit an rdc batch.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Edit()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Attempt to edit an rdc batch
            test.GeneralMethods.OpenURLExpecting403_WebDriver(string.Format("{0}Payment/Batch/Edit.aspx?ID={1}", test.Portal.GetPortalURL().ToLower(), _rdcBatchID));
        }

        #endregion Edit

        #endregion Saved


        #region Household

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Views the remote deposit capture pending page using web driver gear option")]
        [Category(TestCategories.Services.RDCChecker), Category(TestCategories.Services.SmokeTest)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_GearOption_WebDriver()
        {

            // Create an RDC Batch in a pending state without a matched person
            string importBatchName = "Test RDC Batch - WDGear - Pending";
            base.SQL.Giving_Batches_Delete(15, importBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, importBatchName, false, new System.Collections.Generic.List<double>() { 44.44 });


            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View the save page for an rdc batch in the pending state
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(importBatchName, true);

            // Verify user is taken to the pending page
            Assert.AreEqual("Fellowship One :: Batch - Pending", test.Driver.Title);

            // Logout of portal
            test.Portal.LogoutWebDriver();

            base.SQL.Giving_Batches_Delete(15, importBatchName, 3);

        }


        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Views the remote deposit capture pending page using web driver link option")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Link_WebDriver()
        {

            // Create an RDC Batch in a pending state without a matched person
            string importBatchName = "Test RDC Batch - WDLink - Pending";
            base.SQL.Giving_Batches_Delete(15, importBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, importBatchName, false, new System.Collections.Generic.List<double>() { 45.45 });


            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View the save page for an rdc batch in the pending state
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(importBatchName);

            // Verify user is taken to the pending page
            Assert.AreEqual("Fellowship One :: Batch - Pending", test.Driver.Title);

            // Logout of portal
            test.Portal.LogoutWebDriver();

            base.SQL.Giving_Batches_Delete(15, importBatchName, 3);

        }


        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Views the remote deposit capture household pending page.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Household_Unmatched()
        {

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            TestLog.WriteLine("Login to Portal");
            test.Portal.LoginWebDriver();

            // View the save page for an rdc batch in the pending state
            TestLog.WriteLine("Go to RDC");
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(_rdcBatchPendingHouseholdUnmatched);

            // Verify user is taken to the pending page
            Assert.AreEqual("Fellowship One :: Batch - Pending", test.Driver.Title);

            // Logout of portal
            TestLog.WriteLine("Logout Portal");
            test.Portal.LogoutWebDriver();



        }

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Views and verifies the remote deposit capture household pending automatched page.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Household_Automatch()
        {

            // Set initial conditions
            string householdName = "Unknown";

            var account = Guid.NewGuid().ToString().Substring(0, 5);
            var rdcBatchName = "Test RDC Batch - Pending - Household - Automatch";
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 10.39 },
                new System.Collections.Generic.List<string>() { "Felix Gaytan" },
                new System.Collections.Generic.List<string> { _rdcBatchAccountNumberHousehold },
                new System.Collections.Generic.List<string>() { _rdcBatchRoutingNumberHousehold.ToString() },
                false, "1 - General Fund", true);
            //Returns top 1 only
            DataTable householdDT = base.SQL.People_GetHouseholdInformation(15, "Felix Gaytan");

            TestLog.WriteLine("Household Count: " + householdDT.Rows.Count);

            if (householdDT.Rows.Count > 0)
            {
                DataRow dataRow = householdDT.Rows[0];
                householdName = dataRow["HOUSEHOLD_NAME"].ToString();

            }

            TestLog.WriteLine("Household Name: " + householdName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            test.Portal.LoginWebDriver();

            /** TEST Verify Matched **/
            // View an RDC Batch that has an RDC Batch Item that matched to only one person
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName);

            // Verify that the RDC batch item is auto matched to the individual
            var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$10.39", "Amount") + 1;
            Assert.AreEqual(householdName, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreNotEqual("Unmatched", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("$10.39", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            /** TEST Verify that Household is matched and comm values are correct **/
            // Click Match & Verify the multiple matches UI is not present.
            test.Driver.FindElementByLinkText("Match").Click();
            test.GeneralMethods.VerifyTextNotPresentWebDriver("Multiple people and/or organizations have this account. Pick one to match:");

            //Verify that Household present and is current pill highlighed
            test.GeneralMethods.VerifyTextPresentWebDriver("Household");
            Assert.IsTrue(test.Driver.FindElementByLinkText("Household").Displayed);
            IWebElement pill = test.Driver.FindElement(By.XPath("//a[@class='pill current']"));
            TestLog.WriteLine("Pill: " + pill.Text);

            Assert.AreEqual("Household", test.Driver.FindElement(By.XPath("//a[@class='pill current']")).Text, "Household is not current highlighted pill");

            //Verify Hosehold Name
            test.GeneralMethods.VerifyTextPresentWebDriver(householdName);

            // Logout of portal
            test.Portal.LogoutWebDriver();

            base.SQL.Giving_Batches_Delete_RDC(15, rdcBatchName, _rdcBatchAccountNumberHousehold, _rdcBatchRoutingNumberHousehold.ToString());

        }

        //[Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Views and verifies the remote deposit capture household multi automatched.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Household_Multi_Automatch()
        {

            // Set initial conditions
            string householdName = "Unknown";
            string householdName2 = "Unknown";

            var account = Guid.NewGuid().ToString().Substring(0, 5);
            var routingNumber = 111000025;
            var rdcBatchName = "Test RDC Batch - Pending - HH - Multimatch";
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);

            string individualName1 = "Felix Gaytan";
            string individualName2 = "Nina Segovia";
            string individualName3 = "Maya Gaytan";
            var individualID1 = base.SQL.People_Individuals_FetchID(15, individualName1);
            var individualID2 = base.SQL.People_Individuals_FetchID(15, individualName2);
            var individualID3 = base.SQL.People_Individuals_FetchID(15, individualName3);
            var householdId1 = base.SQL.People_Households_FetchID(15, individualID1);
            var householdId2 = base.SQL.People_Households_FetchID(15, individualID2);
            var householdId3 = base.SQL.People_Households_FetchID(15, individualID3);

            base.SQL.Giving_Accounts_Create(15, 1, householdId1, null, account, routingNumber);
            base.SQL.Giving_Accounts_Create(15, 1, householdId2, null, account, routingNumber);

            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 99.39 },
                new System.Collections.Generic.List<string>() { "Felix Gaytan", "Nina Segovia" },
                new System.Collections.Generic.List<string> { _rdcBatchAccountNumberHousehold },
                new System.Collections.Generic.List<string>() { _rdcBatchRoutingNumberHousehold.ToString() },
                false, "1 - General Fund", true);

            //Returns top 1 only
            DataTable householdDT = base.SQL.People_GetHouseholdInformation(15, "Felix Gaytan");
            DataTable householdDT2 = base.SQL.People_GetHouseholdInformation(15, "Nina Segovia");

            TestLog.WriteLine("Household 1 Count: " + householdDT.Rows.Count);
            TestLog.WriteLine("Household 2 Count: " + householdDT2.Rows.Count);

            if (householdDT.Rows.Count > 0)
            {
                DataRow dataRow = householdDT.Rows[0];
                householdName = dataRow["HOUSEHOLD_NAME"].ToString();

            }

            if (householdDT2.Rows.Count > 0)
            {
                DataRow dataRow = householdDT2.Rows[0];
                householdName2 = dataRow["HOUSEHOLD_NAME"].ToString();

            }

            TestLog.WriteLine("Household 1 Name: " + householdName);
            TestLog.WriteLine("Household 2 Name: " + householdName2);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            test.Portal.LoginWebDriver();

            /* BEGIN
                        // TEST Verify Matched
                        // View an RDC Batch that has an RDC Batch Item that matched to only one person
                        test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName);

                        // Verify that the RDC batch item is auto matched to the individual
                        var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$10.39", "Amount") + 1;
                        Assert.AreEqual(householdName, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
                        Assert.AreNotEqual("Unmatched", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
                        Assert.AreEqual("$10.39", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

                        // TEST Verify that Household is matched and comm values are correct
                        // Click Match & Verify the multiple matches UI is not present.
                        test.Driver.FindElementByLinkText("Match").Click();
                        Assert.IsFalse(test.GeneralMethods.VerifyTextPresentWebDriver("Multiple people and/or organizations have this account. Pick one to match:"));

                        //Verify that Household present and is current pill highlighed
                        Assert.IsTrue(test.GeneralMethods.VerifyTextPresentWebDriver("Household"));
                        Assert.IsTrue(test.Driver.FindElementByLinkText("Household").Displayed);
                        IWebElement pill = test.Driver.FindElement(By.XPath("//a[@class='pill current']"));
                        TestLog.WriteLine("Pill: " + pill.Text);

                        Assert.AreEqual("Household", test.Driver.FindElement(By.XPath("//a[@class='pill current']")).Text, "Household is not current highlighted pill");

                        //Verify Hosehold Name
                        Assert.IsTrue(test.GeneralMethods.VerifyTextPresentWebDriver(householdName));
            END */

            // Logout of portal
            test.Portal.LogoutWebDriver();

            base.SQL.Giving_Batches_Delete_RDC(15, rdcBatchName, account, routingNumber.ToString());

        }


        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Views and verifies the houshold can be selected for a remote deposit capture.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Household_Select()
        {

            // Set initial conditions
            string householdName = "Unknown";
            string individualName = "Felix Gaytan";
            string lastName = "Gaytan";
            string amount = "$167.39";

            var account = Guid.NewGuid().ToString().Substring(0, 5);
            var accountNumber = Guid.NewGuid().ToString().Substring(0, 5);
            var routingNumber = 111000025;

            var rdcBatchName = "Test RDC Batch - Pending - Household - Select";
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 167.39 }, null,
                new System.Collections.Generic.List<string> { accountNumber },
                new System.Collections.Generic.List<string>() { routingNumber.ToString() }, false, "1 - General Fund", true);

            //Returns top 1 only
            DataTable householdDT = base.SQL.People_GetHouseholdInformation(15, individualName);

            TestLog.WriteLine("Household Count: " + householdDT.Rows.Count);

            if (householdDT.Rows.Count > 0)
            {
                DataRow dataRow = householdDT.Rows[0];
                householdName = dataRow["HOUSEHOLD_NAME"].ToString();

            }

            TestLog.WriteLine("Household Name: " + householdName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            test.Portal.LoginWebDriver();

            /** TEST Verify Matched **/
            // View an RDC Batch that has an RDC Batch Item that matched to only one person
            TestLog.WriteLine("Go to Imported Batches");
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName);

            // Verify that the RDC batch item is auto matched to the individual
            var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, amount, "Amount") + 1;
            Assert.AreNotEqual(householdName, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("Unmatched", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual(amount, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            /** TEST Verify that Household is matched and comm values are correct **/
            // Click Match & Verify the multiple matches UI is not present.
            test.Driver.FindElementByLinkText("Match").Click();
            test.GeneralMethods.VerifyTextNotPresentWebDriver("Multiple people and/or organizations have this account. Pick one to match:");

            if (test.Driver.FindElements(By.LinkText("Change or unmatch")).Count > 0)
            {
                test.Driver.FindElementByLinkText("Change or unmatch").Click();
            }
            else
            {
                TestLog.WriteLine("Nothing has been matched");
            }

            test.GeneralMethods.WaitForElement(test.Driver, By.Id("find_individual_envelope_number"));

            //Check if you can type in the envelope member number
            test.Driver.FindElementById("find_individual_name");
            test.Driver.FindElementById("find_individual_name").SendKeys(lastName);
            test.Driver.FindElementByXPath("//input[@value='Search']").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(TableIds.Giving_RDC_PotentialMatches));


            //Select Individual
            var selectRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_PotentialMatches, individualName, "Name", "contains") + 1;
            test.Driver.FindElementByXPath(string.Format("//div[@id='find_individual_results']/table/tbody/tr[{0}]/td/button", selectRow)).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Change or unmatch"), 30);

            //Verify that Individual, Household present and you can make a change
            test.Driver.FindElementByLinkText("Individual");
            test.Driver.FindElementByLinkText("Household");
            test.Driver.FindElementByLinkText("Change or unmatch");
            Assert.AreEqual("Individual", test.Driver.FindElement(By.XPath("//a[@class='pill current form_modal_trigger']")).Text, "Individual is not current highlighted pill");

            //Verify that Individual was selected
            test.GeneralMethods.VerifyTextPresentWebDriver(individualName);


            //Select Household link
            test.Driver.FindElementByLinkText("Household").Click();

            //Verify that Household present and is current pill highlighed
            test.GeneralMethods.VerifyTextPresentWebDriver("Household");
            //           Assert.IsTrue(test.Driver.FindElementByLinkText("Household").Displayed);
            Assert.AreEqual("Household", test.Driver.FindElement(By.XPath("//a[@class='pill current']")).Text, "Household is not current highlighted pill");

            //Verify Hosehold Name
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText(""));
            test.GeneralMethods.VerifyTextPresentWebDriver(householdName);

            //Return
            test.Driver.FindElementByLinkText("RETURN").Click();

            //Verify that Selected is now showing up
            row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, amount, "Amount") + 1;
            Assert.AreEqual(householdName, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            // Logout of portal
            test.Portal.LogoutWebDriver();

            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Delete_RDC(15, rdcBatchName, accountNumber, routingNumber.ToString());

        }


        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Views and verifies the remote deposit capture comm values for household are correct")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Household_Automatch_Verify_Comm_Values()
        {

            // Set initial conditions
            string householdName = "Unknown";

            var account = Guid.NewGuid().ToString().Substring(0, 5);
            var rdcBatchName = "Test RDC Batch - Pending - Household - Comm Values";
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 10.99 },
                new System.Collections.Generic.List<string>() { "Felix Gaytan" }, new System.Collections.Generic.List<string> { account },
                new System.Collections.Generic.List<string>() { _rdcBatchRoutingNumberHousehold.ToString() }, false, "1 - General Fund", true);
            //Returns top 1 only
            DataTable householdDT = base.SQL.People_GetHouseholdInformation(15, "Felix Gaytan");
            DataTable householdAddrDT = base.SQL.People_GetHouseholdAddress(15, "Felix Gaytan", "1"); //Primary only
            DataTable householdCommDT = base.SQL.People_GetHouseholdCommunication(15, "Felix Gaytan");

            TestLog.WriteLine("Household Count: " + householdDT.Rows.Count);

            if (householdDT.Rows.Count > 0)
            {
                DataRow dataRow = householdDT.Rows[0];
                householdName = dataRow["HOUSEHOLD_NAME"].ToString();


            }

            TestLog.WriteLine("Household Name: " + householdName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            test.Portal.LoginWebDriver();

            /** TEST Verify Matched **/
            // View an RDC Batch that has an RDC Batch Item that matched to only one person
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName);

            // Verify that the RDC batch item is auto matched to the individual
            var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$10.99", "Amount") + 1;
            Assert.AreEqual(householdName, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreNotEqual("Unmatched", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("$10.99", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            /** TEST Verify that Household is matched and comm values are correct **/
            // Click Match & Verify the multiple matches UI is not present.
            test.Driver.FindElementByLinkText("Match").Click();
            test.GeneralMethods.VerifyTextNotPresentWebDriver("Multiple people and/or organizations have this account. Pick one to match:");

            //Verify that Household present and is current pill highlighed
            test.GeneralMethods.VerifyTextPresentWebDriver("Household");
            Assert.IsTrue(test.Driver.FindElementByLinkText("Household").Displayed);

            Assert.AreEqual("Household", test.Driver.FindElement(By.XPath("//a[@class='pill current']")).Text, "Household is not current highlighted pill");

            //Verify Hosehold Name
            test.GeneralMethods.VerifyTextPresentWebDriver(householdName);


            //Verify Address
            TestLog.WriteLine("Household Addr Count: " + householdAddrDT.Rows.Count);
            if (householdAddrDT.Rows.Count > 0)
            {
                TestLog.WriteLine("Verify Address");
                DataRow householdAddrDR = householdAddrDT.Rows[0];
                test.GeneralMethods.VerifyTextPresentWebDriver(householdAddrDR["ADDRESS_1"].ToString());

                if (!householdAddrDR["ADDRESS_2"].Equals(null))
                {
                    test.GeneralMethods.VerifyTextPresentWebDriver(householdAddrDR["ADDRESS_2"].ToString());
                }

                //City
                test.GeneralMethods.VerifyTextPresentWebDriver(householdAddrDR["CITY"].ToString());

                //State
                test.GeneralMethods.VerifyTextPresentWebDriver(householdAddrDR["ST_PROVINCE"].ToString());

                //ZIP
                test.GeneralMethods.VerifyTextPresentWebDriver(householdAddrDR["POSTAL_CODE"].ToString());

                //test.GeneralMethods.VerifyTextPresentWebDriver(string.Format("{0}, {1} {2}", householdAddrDR["CITY"].ToString(), householdAddrDR["ST_PROVINCE"].ToString(), householdAddrDR["POSTAL_CODE"].ToString()));

                //Verify map link
                Assert.IsTrue(test.Driver.FindElementByLinkText("map").Displayed, "Map Link not present");
                Assert.IsTrue(test.Driver.FindElement(By.XPath("//a[contains(@href, 'maps.google.com/maps?q=" + householdAddrDR["ADDRESS_1"].ToString() + "')]")).Displayed, "Google Map link incorrect");


            }
            else
            {
                Assert.IsFalse(test.Driver.FindElementByClassName("adr").Displayed, "An address is present");
            }

            //Verify Comm Values
            TestLog.WriteLine("Household Comm Value Count: " + householdCommDT.Rows.Count);
            foreach (DataRow dr in householdCommDT.Rows)
            {
                //Home Phone or Email
                TestLog.WriteLine("Communication Type ID: " + dr["COMMUNICATION_TYPE_ID"].ToString());
                if ((dr["COMMUNICATION_TYPE_ID"].ToString() == "1") || (dr["COMMUNICATION_TYPE_ID"].ToString() == "4"))
                {
                    TestLog.WriteLine("Verify Home Phone or Email");
                    test.GeneralMethods.VerifyTextPresentWebDriver(dr["COMMUNICATION_VALUE"].ToString());
                }
                else
                {
                    //Should not show up
                    test.GeneralMethods.VerifyTextNotPresentWebDriver(dr["COMMUNICATION_VALUE"].ToString());

                }

            }

            // Logout of portal
            test.Portal.LogoutWebDriver();

            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Delete_RDC(15, rdcBatchName, account, _rdcBatchRoutingNumberHousehold.ToString());

        }


        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Views and verifies the RDC modal for individual")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Household_IndividualModal()
        {

            // Set initial conditions
            string individualName = "Erica Gaytan";
            var account = Guid.NewGuid().ToString().Substring(0, 5);
            var rdcBatchName = "Test RDC Batch - Pending - Individual - Modal";
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 7.89 },
                new System.Collections.Generic.List<string>() { individualName }, new System.Collections.Generic.List<string> { account },
                new System.Collections.Generic.List<string>() { _rdcBatchRoutingNumberHousehold.ToString() }, false, "1 - General Fund", false);

            DataTable householdModalInfo = base.SQL.People_GetHouseholdIndividual_Modal_Information(15, individualName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            /** TEST Verify Matched **/
            // View an RDC Batch that has an RDC Batch Item that matched to only one person
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName);

            // Verify that the RDC batch item is auto matched to the individual
            var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$7.89", "Amount") + 1;
            Assert.AreEqual(individualName, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreNotEqual("Unmatched", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("$7.89", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            /** TEST Verify that Household is matched and comm values are correct **/
            // Click Match & Verify the multiple matches UI is not present.
            test.Driver.FindElementByLinkText("Match").Click();
            test.GeneralMethods.VerifyTextNotPresentWebDriver("Multiple people and/or organizations have this account. Pick one to match:");

            //Verify that individual is current pill highlighed
            Assert.AreEqual("Individual", test.Driver.FindElement(By.XPath("//a[@class='pill current form_modal_trigger']")).Text, "Individual is not current highlighted pill");

            //click on Individual to get to household individual list modal
            test.GeneralMethods.VerifyTextPresentWebDriver(individualName);
            test.Driver.FindElement(By.XPath("//a[@class='pill current form_modal_trigger']")).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath("//div[@id='modal_select_individual']"));

            //Verify we are in modal
            test.GeneralMethods.VerifyTextPresentWebDriver("Select a household member to attribute this contribution");
            Assert.IsTrue(test.Driver.FindElementByXPath("//div[@id='modal_select_individual']").Displayed);

            //Close Modal
            test.Driver.FindElement(By.XPath("//img[@id='form_modal_close']")).Click();

            // Logout of portal
            test.Portal.LogoutWebDriver();

            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Delete_RDC(15, rdcBatchName, account, _rdcBatchRoutingNumberHousehold.ToString());

        }

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Views and verifies the RDC modal for individual comm values")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Household_IndividualModal_Verify_Comm_Values()
        {

            // Set initial conditions
            string individualName = "Erica Gaytan";
            var account = Guid.NewGuid().ToString().Substring(0, 5);
            var rdcBatchName = "Test RDC Batch - Pending - Ind - ModalCommValue";
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 34.89 },
                new System.Collections.Generic.List<string>() { individualName }, new System.Collections.Generic.List<string> { account },
                new System.Collections.Generic.List<string>() { _rdcBatchRoutingNumberHousehold.ToString() }, false, "1 - General Fund", false);

            DataTable householdModalInfo = base.SQL.People_GetHouseholdIndividual_Modal_Information(15, individualName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            /** TEST Verify Matched **/
            // View an RDC Batch that has an RDC Batch Item that matched to only one person
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName);

            // Verify that the RDC batch item is auto matched to the individual
            var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$34.89", "Amount") + 1;
            Assert.AreEqual(individualName, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreNotEqual("Unmatched", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("$34.89", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            /** TEST Verify that Household is matched and comm values are correct **/
            // Click Match & Verify the multiple matches UI is not present.
            test.Driver.FindElementByLinkText("Match").Click();
            test.GeneralMethods.VerifyTextNotPresentWebDriver("Multiple people and/or organizations have this account. Pick one to match:");

            //Verify that individual is current pill highlighed
            Assert.AreEqual("Individual", test.Driver.FindElement(By.XPath("//a[@class='pill current form_modal_trigger']")).Text, "Individual is not current highlighted pill");

            //click on Individual to get to household individual list modal
            test.GeneralMethods.VerifyTextPresentWebDriver(individualName);
            test.Driver.FindElement(By.XPath("//a[@class='pill current form_modal_trigger']")).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath("//div[@id='modal_select_individual']"));

            //Verify we are in modal
            test.GeneralMethods.VerifyTextPresentWebDriver("Select a household member to attribute this contribution");
            Assert.IsTrue(test.Driver.FindElementByXPath("//div[@id='modal_select_individual']").Displayed);

            //Verity by sort through all information in select individual modal
            int count = 1;
            DateTime birth;
            string ageString;
            string birthString;

            foreach (DataRow peepRow in householdModalInfo.Rows)
            {
                //Verify Individual Name and Status
                TestLog.WriteLine("Name: " + peepRow["INDIVIDUAL_NAME"].ToString());
                if (peepRow["LIST_NAME"].ToString().Equals(""))
                {
                    Assert.AreEqual(peepRow["INDIVIDUAL_NAME"].ToString() + "\r\n" + peepRow["STATUS_NAME"].ToString(), test.Driver.FindElementByXPath(string.Format("//div[@id='modal_select_individual']/table/tbody/tr[{0}]/td[2]", count)).Text, "Individual Name & Status is not correct");
                }
                else
                {
                    Assert.AreEqual(peepRow["INDIVIDUAL_NAME"].ToString() + "\r\n" + peepRow["STATUS_NAME"].ToString() + " > " + peepRow["LIST_NAME"], test.Driver.FindElementByXPath(string.Format("//div[@id='modal_select_individual']/table/tbody/tr[{0}]/td[2]", count)).Text, "Individual Name & Status is not correct");
                }

                try {
                    //Verify DOB and Age
                    birth = DateTime.Parse(peepRow["DATE_OF_BIRTH"].ToString());
                    ageString = test.Portal.CalculateIndividualModalAge(birth);
                    birthString = birth.ToString("d MMM yyyy");
                    Assert.AreEqual(peepRow["HOUSEHOLD_MEMBER_TYPE_NAME"].ToString() + "\r\n" + birthString + " " + ageString, test.Driver.FindElementByXPath(string.Format("//div[@id='modal_select_individual']/table/tbody/tr[{0}]/td[3]", count)).Text, "DOB & year calc is not correct");
                }
                catch (System.FormatException e)
                {
                    TestLog.WriteHighlighted("Find an error configured data, no birthday set in the household: " + peepRow["INDIVIDUAL_NAME"].ToString());
                }

                count++;

            }

            //Return
            test.Driver.FindElementById("form_modal_close").Click();
            test.Driver.FindElementByLinkText("RETURN").Click();

            // Logout of portal
            test.Portal.LogoutWebDriver();

            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Delete_RDC(15, rdcBatchName, account, _rdcBatchRoutingNumberHousehold.ToString());
        }

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Views and verifies new selected individual")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Household_IndividualModal_Select()
        {

            // Set initial conditions
            string individualName = "Erica Gaytan";
            var account = Guid.NewGuid().ToString().Substring(0, 5);
            var rdcBatchName = "Test RDC Batch - Pending - Individual - Select";
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 94.89 },
                new System.Collections.Generic.List<string>() { individualName }, new System.Collections.Generic.List<string> { account },
                new System.Collections.Generic.List<string>() { _rdcBatchRoutingNumberHousehold.ToString() }, false, "1 - General Fund", false);

            DataTable householdModalInfo = base.SQL.People_GetHouseholdIndividual_Modal_Information(15, individualName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            /** TEST Verify Matched **/
            // View an RDC Batch that has an RDC Batch Item that matched to only one person
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName);

            // Verify that the RDC batch item is auto matched to the individual
            var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$94.89", "Amount") + 1;
            Assert.AreEqual(individualName, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreNotEqual("Unmatched", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("$94.89", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            /** TEST Verify that Household is matched and comm values are correct **/
            // Click Match & Verify the multiple matches UI is not present.
            test.Driver.FindElementByLinkText("Match").Click();
            test.GeneralMethods.VerifyTextNotPresentWebDriver("Multiple people and/or organizations have this account. Pick one to match:");

            //Verify that individual is current pill highlighed
            Assert.AreEqual("Individual", test.Driver.FindElement(By.XPath("//a[@class='pill current form_modal_trigger']")).Text, "Individual is not current highlighted pill");

            //click on Individual to get to household individual list modal
            test.GeneralMethods.VerifyTextPresentWebDriver(individualName);
            test.Driver.FindElement(By.XPath("//a[@class='pill current form_modal_trigger']")).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath("//div[@id='modal_select_individual']"));

            //Verify we are in modal
            test.GeneralMethods.VerifyTextPresentWebDriver("Select a household member to attribute this contribution");
            Assert.IsTrue(test.Driver.FindElementByXPath("//div[@id='modal_select_individual']").Displayed);


            //Verity by sort through all information in select individual modal
            int count = 1;
            DateTime birth;
            string ageString;
            string birthString;

            foreach (DataRow peepRow in householdModalInfo.Rows)
            {
                //Verify Individual Name and Status
                TestLog.WriteLine("Name: " + peepRow["INDIVIDUAL_NAME"].ToString());
                TestLog.WriteLine("List: " + peepRow["LIST_NAME"]);

                if (peepRow["LIST_NAME"].ToString().Equals(""))
                {
                    Assert.AreEqual(peepRow["INDIVIDUAL_NAME"].ToString() + "\r\n" + peepRow["STATUS_NAME"].ToString(), test.Driver.FindElementByXPath(string.Format("//div[@id='modal_select_individual']/table/tbody/tr[{0}]/td[2]", count)).Text, "Individual Name & Status is not correct");
                }
                else
                {
                    Assert.AreEqual(peepRow["INDIVIDUAL_NAME"].ToString() + "\r\n" + peepRow["STATUS_NAME"].ToString() + " > " + peepRow["LIST_NAME"], test.Driver.FindElementByXPath(string.Format("//div[@id='modal_select_individual']/table/tbody/tr[{0}]/td[2]", count)).Text, "Individual Name & Status is not correct");
                }

                try
                {
                    //Verify DOB and Age
                    birth = DateTime.Parse(peepRow["DATE_OF_BIRTH"].ToString());
                    ageString = test.Portal.CalculateIndividualModalAge(birth);
                    birthString = birth.ToString("d MMM yyyy");
                    TestLog.WriteLine("Checking borthday info of individual: " + peepRow["INDIVIDUAL_NAME"].ToString());
                    Assert.AreEqual(peepRow["HOUSEHOLD_MEMBER_TYPE_NAME"].ToString() + "\r\n" + birthString + " " + ageString, test.Driver.FindElementByXPath(string.Format("//div[@id='modal_select_individual']/table/tbody/tr[{0}]/td[3]", count)).Text, "DOB & year calc is not correct");
                }
                catch (System.FormatException e)
                {
                    TestLog.WriteHighlighted("Find an error configured data, no birthday set in the household: " + peepRow["INDIVIDUAL_NAME"].ToString());
                }
                
                count++;

            }


            //Select the last individual in modal
            int selectRow = test.GeneralMethods.GetTableRowCountWebDriver(TableIds.Giving_RDC_PotentialMatches);
            TestLog.WriteLine("Selected Row: " + selectRow);
            test.Driver.FindElementByXPath(string.Format("//div[@id='modal_select_individual']/table/tbody/tr[{0}]/td[2]", selectRow)).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath("//a[@class='pill current form_modal_trigger']"));

            //Verify that new individual is present
            //Verify that individual is current pill highlighed
            Assert.AreEqual("Individual", test.Driver.FindElementByXPath("//a[@class='pill current form_modal_trigger']").Text, "Selected Individual is not current highlighted pill");
            test.GeneralMethods.VerifyTextPresentWebDriver(individualName);


            //Return
            test.Driver.FindElementByLinkText("RETURN").Click();

            //Verify New Individual Name
            test.GeneralMethods.VerifyTextPresentWebDriver(householdModalInfo.Rows[selectRow - 1]["INDIVIDUAL_NAME"].ToString());

            // Logout of portal
            test.Portal.LogoutWebDriver();

            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Delete_RDC(15, rdcBatchName, account, _rdcBatchRoutingNumberHousehold.ToString());

        }

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Views and changes the automatch remote deposit capture household pending to unmatched.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Household_Automatch_Change_Unmatch()
        {

            // Set initial conditions
            var accountNumber = Guid.NewGuid().ToString().Substring(0, 5);
            var routingNumber = 111099025;
            string individualName = "Nina Segovia";
            var individualID = base.SQL.People_Individuals_FetchID(15, individualName);
            var householdId = base.SQL.People_Households_FetchID(15, individualID);
            base.SQL.Giving_Accounts_Create(15, 1, householdId, individualID, accountNumber, routingNumber);


            var rdcBatchName = "Test RDC Batch - Pending - Household - Change";
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 12.99 },
                new System.Collections.Generic.List<string>() { individualName }, new System.Collections.Generic.List<string> { accountNumber },
                new System.Collections.Generic.List<string>() { routingNumber.ToString() }, false, "1 - General Fund", true);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            /** TEST Verify Matched **/
            // View an RDC Batch that has an RDC Batch Item that matched to only one person
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName);

            // Verify that the RDC batch item is auto matched to the individual
            var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$12.99", "Amount") + 1;
            Assert.AreEqual(individualName, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreNotEqual("Unmatched", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("$12.99", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            /** TEST Verify that Household is matched and comm values are correct **/
            // Click Match & Verify the multiple matches UI is not present.
            test.Driver.FindElementByLinkText("Match").Click();
            test.GeneralMethods.VerifyTextNotPresentWebDriver("Multiple people and/or organizations have this account. Pick one to match:");

            //Verify that Household present and is current pill highlighed
            test.GeneralMethods.VerifyTextPresentWebDriver("Household");
            test.Driver.FindElementByLinkText("Household");
            Assert.AreEqual("Household", test.Driver.FindElement(By.XPath("//a[@class='pill current']")).Text, "Household is not current highlighted pill");

            //Change to unmatch
            //test.Driver.FindElementByLinkText("Change or unmatch");
            test.Driver.FindElementByLinkText("Change or unmatch").Click();

            test.GeneralMethods.WaitForElement(test.Driver, By.Id("find_individual_name"), 60);

            //Verify that we are in the search
            test.Driver.FindElementByLinkText("Individual");
            test.Driver.FindElementByLinkText("Organization");
            test.Driver.FindElementByLinkText("New individual");
            test.Driver.FindElementById("find_individual_name");
            test.Driver.FindElementById("find_individual_envelope_number");
            test.Driver.FindElementById("find_individual_address");


            // Return
            test.Driver.FindElementByLinkText("RETURN").Click();

            // Verify that the RDC batch item is auto unmatched
            var unmatchedrow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$12.99", "Amount") + 1;
            Assert.AreNotEqual(individualName, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, unmatchedrow)).Text);
            Assert.AreEqual("Unmatched", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, unmatchedrow)).Text);
            Assert.AreEqual("$12.99", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.Giving_RDC_RDCBatchItemList, unmatchedrow)).Text);

            // Return
            test.Driver.FindElementByLinkText("RETURN").Click();

            // Logout of portal
            test.Portal.LogoutWebDriver();

            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Delete_RDC(15, rdcBatchName, accountNumber, routingNumber.ToString());
            base.SQL.Giving_Accounts_Delete(15, 1, individualID, accountNumber);


        }


        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Views and verifies the remote deposit capture pending has multiple individuals to chose from.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Individual_Multi_Accounts_Select()
        {

            //Shared
            var _rdcBatchAccountNumberHouseholdInd = Guid.NewGuid().ToString().Substring(0, 5);
            var _rdcBatchRoutingNumberHouseholdInd = 111000025;

            string individualName1 = "Felix Gaytan";
            string individualName2 = "Nina Segovia";
            string individualName3 = "Maya Gaytan";
            var individualID1 = base.SQL.People_Individuals_FetchID(15, individualName1);
            var individualID2 = base.SQL.People_Individuals_FetchID(15, individualName2);
            var individualID3 = base.SQL.People_Individuals_FetchID(15, individualName3);
            var householdId1 = base.SQL.People_Households_FetchID(15, individualID1);
            var householdId2 = base.SQL.People_Households_FetchID(15, individualID2);
            var householdId3 = base.SQL.People_Households_FetchID(15, individualID3);

            base.SQL.Giving_Accounts_Create(15, 1, householdId1, individualID1, _rdcBatchAccountNumberHouseholdInd, _rdcBatchRoutingNumberHouseholdInd);
            base.SQL.Giving_Accounts_Create(15, 1, householdId2, individualID2, _rdcBatchAccountNumberHouseholdInd, _rdcBatchRoutingNumberHouseholdInd);
            base.SQL.Giving_Accounts_Create(15, 1, householdId3, individualID3, _rdcBatchAccountNumberHouseholdInd, _rdcBatchRoutingNumberHouseholdInd);

            var rdcBatchNameInd = "Test RDC Batch - Pending - HH - Multi";
            base.SQL.Giving_Batches_Delete(15, rdcBatchNameInd, 3);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchNameInd, false, new System.Collections.Generic.List<double>() { 99.99 }, null,
                new System.Collections.Generic.List<string> { _rdcBatchAccountNumberHouseholdInd },
                new System.Collections.Generic.List<string>() { _rdcBatchRoutingNumberHouseholdInd.ToString() }, false, "1 - General Fund", true);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            /** TEST Verify Matched **/
            // View an RDC Batch that has an RDC Batch Item that matched to only one person
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchNameInd);

            // Verify that the RDC batch item is auto matched to the individual
            var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$99.99", "Amount") + 1;
            Assert.AreNotEqual(individualName1, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreNotEqual(individualName2, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreNotEqual(individualName3, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("Unmatched", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);
            Assert.AreEqual("$99.99", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            /** TEST Verify that Household is matched and comm values are correct **/
            // Click Match & Verify the multiple matches UI is not present.
            test.Driver.FindElementByLinkText("Match").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Change"));
            test.GeneralMethods.VerifyTextPresentWebDriver("Multiple people and/or organizations have this account. Pick one to match:");

            //Verify that Household present and you can make a change
            Assert.IsFalse(test.Driver.FindElements(By.LinkText("Individual")).Equals(0), "Individual Link is not available");
            Assert.IsFalse(test.Driver.FindElements(By.LinkText("Household")).Equals(0), "Household Link is not available");
            Assert.IsTrue(test.Driver.FindElementByLinkText("Change").Displayed, "Change link is not displayed");

            // Verify the table for potential matches is present
            //Assert.IsTrue(test.Driver.FindElements(By.XPath("//div[@id='find_individual_results']")).Equals(1), "Find Individual Results not present.");


            // Verify the individuals that have this account can be picked
            //Hack for now
            var ind1Row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_IndividualMatches, individualName1, "Name", "contains") + 1;
            var ind2Row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_IndividualMatches, individualName2, "Name", "contains") + 1;
            var ind3Row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_IndividualMatches, individualName3, "Name", "contains") + 1;

            Assert.Contains(test.Driver.FindElementByXPath(string.Format("//div[@id='find_individual_results']/table/tbody/tr[{0}]/td[2]", ind1Row)).Text, individualName1, individualName1 + " not present");
            Assert.Contains(test.Driver.FindElementByXPath(string.Format("//div[@id='find_individual_results']/table/tbody/tr[{0}]/td[2]", ind2Row)).Text, individualName2, individualName2 + " not present");
            Assert.Contains(test.Driver.FindElementByXPath(string.Format("//div[@id='find_individual_results']/table/tbody/tr[{0}]/td[2]", ind3Row)).Text, individualName3, individualName3 + " not present");


            //TODO ItemExist Method does not work DEBUG
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Giving_RDC_IndividualMatches, individualName1, "Name", "conatins"), individualName1 + " not present");
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Giving_RDC_PotentialMatches, individualName2, "Name", "contains"), individualName2 + " not present");
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Giving_RDC_PotentialMatches, individualName3, "Name", "contains"), individualName3 + " not present");

            //Select individual
            ////div[@id='find_individual_results']/table/tbody/tr[4]/td/button
            var selectRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_IndividualMatches, individualName3, "Name", "contains") + 1;
            test.Driver.FindElementByXPath(string.Format("//div[@id='find_individual_results']/table/tbody/tr[{0}]/td/button", selectRow)).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Individual"));

            //Verify that Individual, Household present and you can make a change
            test.Driver.FindElementByLinkText("Individual");
            test.Driver.FindElementByLinkText("Household");
            test.Driver.FindElementByLinkText("Change or unmatch");
            Assert.AreEqual("Individual", test.Driver.FindElement(By.XPath("//a[@class='pill current form_modal_trigger']")).Text, "Individual is not current highlighted pill");

            test.GeneralMethods.VerifyTextPresentWebDriver(individualName3);
            //TODO Verify Comm Values


            // Return
            test.Driver.FindElementByLinkText("RETURN").Click();

            //Verify that Selected is now showing up
            row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_RDCBatchItemList, "$99.99", "Amount") + 1;
            Assert.AreEqual(individualName3, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.Giving_RDC_RDCBatchItemList, row)).Text);

            //Return
            test.Driver.FindElementByLinkText("RETURN").Click();


            // Logout of portal
            test.Portal.LogoutWebDriver();

            base.SQL.Giving_Batches_Delete(15, rdcBatchNameInd, 3);
            base.SQL.Giving_Batches_Delete_RDC(15, rdcBatchNameInd, _rdcBatchAccountNumberHouseholdInd, _rdcBatchRoutingNumberHouseholdInd.ToString());
            base.SQL.Giving_Accounts_Delete(15, 1, individualID1, _rdcBatchAccountNumberHouseholdInd);
            base.SQL.Giving_Accounts_Delete(15, 1, individualID2, _rdcBatchAccountNumberHouseholdInd);
            base.SQL.Giving_Accounts_Delete(15, 1, individualID2, _rdcBatchAccountNumberHouseholdInd);

        }


        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Views the remote deposit capture household pending page and search by member envelope number")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Household_Search_Member_Envelope_Number()
        {

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string individualName1 = "Felix Gaytan";
            string individualName2 = "Maya Gaytan";
            string memberEnvelopeNumber = Guid.NewGuid().ToString().Substring(0, 8);

            base.SQL.People_Update_Member_Envelope_Number(15, individualName1, memberEnvelopeNumber);
            base.SQL.People_Update_Member_Envelope_Number(15, individualName2, memberEnvelopeNumber);


            test.Portal.LoginWebDriver();

            // View the save page for an rdc batch in the pending state
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(_rdcBatchPendingHouseholdUnmatched);

            // Verify user is taken to the pending page
            Assert.AreEqual("Fellowship One :: Batch - Pending", test.Driver.Title);

            // Click Match & Verify the multiple matches UI is not present.
            test.Driver.FindElementByPartialLinkText("Match").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("View Batch"));

            if (test.Driver.FindElements(By.LinkText("Change or unmatch")).Count > 0)
            {
                test.Driver.FindElementByLinkText("Change or unmatch").Click();
            }
            else
            {
                TestLog.WriteLine("Nothing has been matched");
            }

            test.GeneralMethods.WaitForElement(test.Driver, By.Id("find_individual_envelope_number"));

            //Check if you can type in the envelope member number
            test.Driver.FindElementById("find_individual_envelope_number");
            test.Driver.FindElementById("find_individual_envelope_number").SendKeys(memberEnvelopeNumber);
            test.Driver.FindElementByXPath("//input[@value='Search']").Click();

            //test.GeneralMethods.WaitForElement(test.Driver, By.XPath(TableIds.Giving_RDC_PotentialMatches), 20);
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("find_individual_results"), 20);

            // Verify the individuals that have this account can be picked
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Giving_RDC_PotentialMatches, individualName1, "Name", "contains"), "Member Envelope Number: " + memberEnvelopeNumber + " for " + individualName1 + " not found");
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Giving_RDC_PotentialMatches, individualName2, "Name", "contains"), "Member Envelope Number: " + memberEnvelopeNumber + " for " + individualName2 + " not found");
            //Hack for now // //*[@id="find_individual_results"]/table
            //var ind1Row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_PotentialMatches, individualName1, "Name", "contains") + 1;
            //var ind2Row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_RDC_PotentialMatches, individualName2, "Name", "contains") + 1;
            var ind1Row = test.GeneralMethods.GetTableRowNumberWebDriver("//div[@id='find_individual_results']/table", individualName1, "Name", "contains") + 1;
            var ind2Row = test.GeneralMethods.GetTableRowNumberWebDriver("//div[@id='find_individual_results']/table", individualName2, "Name", "contains") + 1;


            Assert.Contains(test.Driver.FindElementByXPath(string.Format("//div[@id='find_individual_results']/table/tbody/tr[{0}]/td[2]", ind1Row)).Text, individualName1, individualName1 + " not present");
            Assert.Contains(test.Driver.FindElementByXPath(string.Format("//div[@id='find_individual_results']/table/tbody/tr[{0}]/td[2]", ind2Row)).Text, individualName2, individualName2 + " not present");


            // Return
            test.Driver.FindElementByLinkText("RETURN").Click();
            test.Driver.FindElementByLinkText("RETURN").Click();

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Views the remote deposit capture household pending page and search by member envelope number with no results")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Household_Search_Member_Envelope_Number_NoResults()
        {

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View the save page for an rdc batch in the pending state
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(_rdcBatchPendingHouseholdUnmatched);

            // Verify user is taken to the pending page
            Assert.AreEqual("Fellowship One :: Batch - Pending", test.Driver.Title);

            // Click Match & Verify the multiple matches UI is not present.
            test.Driver.FindElementByPartialLinkText("Match").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("View Batch"));

            if (test.Driver.FindElements(By.LinkText("Change or unmatch")).Count > 0)
            {
                test.Driver.FindElementByLinkText("Change or unmatch").Click();
            }
            else
            {
                TestLog.WriteLine("Nothing has been matched");
            }

            //Check if you can type in the envelope member number
            test.Driver.FindElementById("find_individual_envelope_number");
            test.Driver.FindElementById("find_individual_envelope_number").SendKeys("GigEmAg97");
            test.Driver.FindElementByXPath("//input[@value='Search']").Click();

            Thread.Sleep(5000);

            test.GeneralMethods.VerifyTextPresentWebDriver("Sorry, no results found. Please try searching on different criteria.");

            // Return
            test.Driver.FindElementByLinkText("RETURN").Click();
            test.Driver.FindElementByLinkText("RETURN").Click();

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }


        #endregion Household

        #region Misc


        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the date control on the remote deposit capture pending page.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_DateControl()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // View the save page for an rdc batch in the pending state
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(_rdcBatchPending, true);

            // Verify the date control
            test.GeneralMethods.VerifyDateControlWebDriver("rdc_date");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3465: Matches an RDC batch item to a newly created individual")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Matching_NewIndividual()
        {
            // Setup data
            var rdcBatchName = "Test RDC Batch - Match New Ind";
            var indFirstName = "FT";
            var indLastName = "NewIndividual";
            var acctNumber = "222333444";
            var routNumber = "111000025";
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Delete_RDC(15, rdcBatchName, acctNumber, routNumber);
            base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", indFirstName, indLastName), "Merge Dump");
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 1.11 }, null,
                new System.Collections.Generic.List<string> { acctNumber },
                new System.Collections.Generic.List<string> { routNumber }, false, "Auto Test Fund", false);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // View a Pending RDC batch
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName);
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Match"));

            // Click on Match for a batch item
            test.Driver.FindElementByLinkText("Match").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("View Batch"));

            // Match item to newly created individual
            if (test.Driver.FindElements(By.LinkText("Change or unmatch")).Count > 0)
            {
                test.Driver.FindElementByLinkText("Change or unmatch").Click();
            }
            test.Driver.FindElementByLinkText("New individual").Click();
            test.Driver.FindElementById("input_firstname_1").SendKeys(indFirstName);
            test.Driver.FindElementById("last_name1").SendKeys(indLastName);
            IWebElement reqTypeDropDown = test.Driver.FindElementById("individual_status1");
            SelectElement select = new SelectElement(reqTypeDropDown);
            select.SelectByText("Member");
            test.Driver.FindElementById("button_submit").Click();

            try
            {
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("View Batch"));

                // Verify indiviudal is matched to item
                test.GeneralMethods.VerifyTextPresentWebDriver(string.Format("{0} {1}", indFirstName, indLastName));

                // Cancel out of batch
                test.Driver.FindElementByLinkText("RETURN").Click();
                test.Driver.FindElementByLinkText("RETURN").Click();

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }
            finally
            {
                // Clean up
                base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
                base.SQL.Giving_Batches_Delete_RDC(15, rdcBatchName, acctNumber, routNumber);
                base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", indFirstName, indLastName), "Merge Dump");
            }

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the fund is retained while matching a split batch")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Matching_Retain_Fund()
        {
            // Setup data
            string rdcBatchName = "Test RDC Batch - Retain Fund split batch";
            string indFirstName = "FT";
            string indLastName = "Tester";
            string acctNumber = "222333744";
            string routNumber = "111000075";
            string acctNumber2 = "222337743";
            string routNumber2 = "111000724";
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Delete_RDC(15, rdcBatchName, acctNumber, routNumber);

            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 1.11, 1.12 }, null,
                new System.Collections.Generic.List<string> { acctNumber, acctNumber2 },
                new System.Collections.Generic.List<string> { routNumber, routNumber2 }, false, "Auto Test Fund", false);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // View a Pending RDC batch
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName);
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Match"));

            // Click Process this batch as splits
            test.Driver.FindElementByLinkText("Process this batch as splits").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("View Batch"));

            //// Select First Fund 
            IWebElement reqTypeDropDown = test.Driver.FindElementById("ddlFundPledgeDrive_0");
            SelectElement select = new SelectElement(reqTypeDropDown);
            select.SelectByText("1 - General Fund");
            test.Driver.FindElementById("add_line_item").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("ddlFundPledgeDrive_1"), 20);

            /// Select Second Fund
            IWebElement reqTypeDropDown2 = test.Driver.FindElementById("ddlFundPledgeDrive_1");
            SelectElement select2 = new SelectElement(reqTypeDropDown2);
            select2.SelectByText("2 - Building Fund");
            test.Driver.FindElementById("add_line_item").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("ddlFundPledgeDrive_2"), 20);

            /// Select Third Fund
            IWebElement reqTypeDropDown3 = test.Driver.FindElementById("ddlFundPledgeDrive_2");
            SelectElement select3 = new SelectElement(reqTypeDropDown3);
            select3.SelectByText("3 - Missions");

            /// Go to Next Item in Batch
            test.Driver.FindElementByLinkText("Next item").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("View Batch"));

            /// Verify Funds Retained
            SelectElement verifySelect = new SelectElement(test.Driver.FindElementById("ddlFundPledgeDrive_0"));
            SelectElement verifySelect2 = new SelectElement(test.Driver.FindElementById("ddlFundPledgeDrive_1"));
            SelectElement verifySelect3 = new SelectElement(test.Driver.FindElementById("ddlFundPledgeDrive_2"));
            Assert.AreEqual("1 - General Fund", verifySelect.SelectedOption.Text, "General Fund was not retained");
            Assert.AreEqual("2 - Building Fund", verifySelect2.SelectedOption.Text, "Building Fund was not retained");
            Assert.AreEqual("3 - Missions", verifySelect3.SelectedOption.Text, "Missions was not retained");
            //test.GeneralMethods.VerifyTextPresentWebDriver("1 - General Fund");
            //test.GeneralMethods.VerifyTextPresentWebDriver("2 - Building Fund");
            //test.GeneralMethods.VerifyTextPresentWebDriver("3 - Missions");

            // Cancel out of batch
            test.Driver.FindElementByLinkText("RETURN").Click();
            test.Driver.FindElementByLinkText("RETURN").Click();

            // Logout of portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Delete_RDC(15, rdcBatchName, acctNumber, routNumber);

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the fund is retained while matching a split batch even if it is deleted")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Matching_Delete_Fund()
        {
            // Setup data
            string rdcBatchName = "Test RDC Batch - Delete Fund split batch";
            string indFirstName = "FT";
            string indLastName = "Tester";
            string acctNumber = "278333490";
            string routNumber = "111000016";
            string acctNumber2 = "023456789";
            string routNumber2 = "111000012";
            string acctNumber3 = "123456780";
            string routNumber3 = "111000321";
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Delete_RDC(15, rdcBatchName, acctNumber, routNumber);
            //base.SQL.People_MergeIndividual(15, indFirstName + indLastName, "Merge Dump");
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 2.31, 3.12, 2.13 }, null,
                new System.Collections.Generic.List<string> { acctNumber, acctNumber2, acctNumber3 },
                new System.Collections.Generic.List<string> { routNumber, routNumber2, routNumber3 }, false, "Auto Test Fund", false);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // View a Pending RDC batch
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName);
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Match"));

            // Click Process this batch as splits
            test.Driver.FindElementByLinkText("Process this batch as splits").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("View Batch"));

            //// Select First Fund 
            IWebElement reqTypeDropDown = test.Driver.FindElementById("ddlFundPledgeDrive_0");
            SelectElement select = new SelectElement(reqTypeDropDown);
            select.SelectByText("1 - General Fund");
            test.Driver.FindElementById("add_line_item").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("ddlFundPledgeDrive_1"), 20);

            /// Select Second Fund
            IWebElement reqTypeDropDown2 = test.Driver.FindElementById("ddlFundPledgeDrive_1");
            SelectElement select2 = new SelectElement(reqTypeDropDown2);
            select2.SelectByText("2 - Building Fund");
            test.Driver.FindElementById("add_line_item").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("ddlFundPledgeDrive_2"), 20);


            /// Select Third Fund
            IWebElement reqTypeDropDown3 = test.Driver.FindElementById("ddlFundPledgeDrive_2");
            SelectElement select3 = new SelectElement(reqTypeDropDown3);
            select3.SelectByText("3 - Missions");

            /// Go to Next Item in Batch
            test.Driver.FindElementByLinkText("Next item").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("View Batch"));

            /// Verify Funds Retained
            SelectElement verifySelect = new SelectElement(test.Driver.FindElementById("ddlFundPledgeDrive_0"));
            SelectElement verifySelect2 = new SelectElement(test.Driver.FindElementById("ddlFundPledgeDrive_1"));
            SelectElement verifySelect3 = new SelectElement(test.Driver.FindElementById("ddlFundPledgeDrive_2"));
            Assert.AreEqual("1 - General Fund", verifySelect.SelectedOption.Text, "General Fund was not retained");
            Assert.AreEqual("2 - Building Fund", verifySelect2.SelectedOption.Text, "Building Fund was not retained");
            Assert.AreEqual("3 - Missions", verifySelect3.SelectedOption.Text, "Missions was not retained");

            //test.GeneralMethods.VerifyTextPresentWebDriver("1 - General Fund");
            //test.GeneralMethods.VerifyTextPresentWebDriver("2 - Building Fund");
            //test.GeneralMethods.VerifyTextPresentWebDriver("3 - Missions");

            /// Delete a fund
            int beforeRows = test.GeneralMethods.GetTableRowCountWebDriver("//table[@id='line_items']");
            test.Driver.FindElementById("Trash_can_anchor_2").Click();

            Assert.AreEqual(test.Driver.SwitchTo().Alert().Text, "Are you sure?", "Delete Alert Message Incorrect");
            test.Driver.SwitchTo().Alert().Accept();
            //test.GeneralMethods.VerifyTextPresentWebDriver("3 - Missions");
            //TODO Verify Delete Fund

            int afterRows = test.GeneralMethods.GetTableRowCountWebDriver("//table[@id='line_items']");
            Assert.AreEqual((beforeRows - 1), afterRows, "Row Not Deleted");

            try
            {
                test.Driver.FindElementById("ddlFundPledgeDrive_2");
                throw new WebDriverException("Pledge/Fund [3 - Missions] was not deleted");
            }
            catch (Exception e)
            {
                //Exception capture means we deleted it
                verifySelect = new SelectElement(test.Driver.FindElementById("ddlFundPledgeDrive_0"));
                verifySelect2 = new SelectElement(test.Driver.FindElementById("ddlFundPledgeDrive_1"));
                Assert.AreEqual("1 - General Fund", verifySelect.SelectedOption.Text, "General Fund was not retained after delete");
                Assert.AreEqual("2 - Building Fund", verifySelect2.SelectedOption.Text, "Building Fund was not retained after delete");
            }

            /// Go to Next Item in Batch
            test.Driver.FindElementByLinkText("Next item").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("View Batch"));

            /// Verify Funds Retained
            verifySelect = new SelectElement(test.Driver.FindElementById("ddlFundPledgeDrive_0"));
            verifySelect2 = new SelectElement(test.Driver.FindElementById("ddlFundPledgeDrive_1"));
            verifySelect3 = new SelectElement(test.Driver.FindElementById("ddlFundPledgeDrive_2"));
            Assert.AreEqual("1 - General Fund", verifySelect.SelectedOption.Text, "General Fund was not retained");
            Assert.AreEqual("2 - Building Fund", verifySelect2.SelectedOption.Text, "Building Fund was not retained");
            Assert.AreEqual("3 - Missions", verifySelect3.SelectedOption.Text, "Missions was not retained");
            //test.GeneralMethods.VerifyTextPresentWebDriver("1 - General Fund");
            //test.GeneralMethods.VerifyTextPresentWebDriver("2 - Building Fund");
            //test.GeneralMethods.VerifyTextPresentWebDriver("3 - Missions");

            // Cancel out of batch
            test.Driver.FindElementByLinkText("RETURN").Click();
            test.Driver.FindElementByLinkText("RETURN").Click();

            // Logout of portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Delete_RDC(15, rdcBatchName, acctNumber, routNumber);
            //base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", indFirstName, indLastName), "Merge Dump");
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that all funds are retained while matching a split batch even if it is changed")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Matching_Change_Fund()
        {
            // Setup data
            string rdcBatchName = "Test RDC Batch - Change Fund split batch";
            string indFirstName = "FT";
            string indLastName = "Tester";
            string acctNumber = "122333344";
            string routNumber = "111000069";
            string acctNumber2 = "122333456";
            string routNumber2 = "111000009";
            string acctNumber3 = "333444555";
            string routNumber3 = "111000058";
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Delete_RDC(15, rdcBatchName, acctNumber, routNumber);
            //base.SQL.People_MergeIndividual(15, indFirstName + indLastName, "Merge Dump");
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 1.23, 3.12, 2.31 }, null,
                new System.Collections.Generic.List<string> { acctNumber, acctNumber2, acctNumber3 },
                new System.Collections.Generic.List<string> { routNumber, routNumber2, routNumber3 }, false, "Auto Test Fund", false);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // View a Pending RDC batch
            test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName);
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Match"));

            // Click Process this batch as splits
            test.Driver.FindElementByLinkText("Process this batch as splits").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("View Batch"));

            //// Select First Fund 
            IWebElement reqTypeDropDown = test.Driver.FindElementById("ddlFundPledgeDrive_0");
            SelectElement select = new SelectElement(reqTypeDropDown);
            select.SelectByText("1 - General Fund");
            test.Driver.FindElementById("add_line_item").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("ddlFundPledgeDrive_1"), 20);

            /// Select Second Fund
            IWebElement reqTypeDropDown2 = test.Driver.FindElementById("ddlFundPledgeDrive_1");
            SelectElement select2 = new SelectElement(reqTypeDropDown2);
            select2.SelectByText("2 - Building Fund");
            test.Driver.FindElementById("add_line_item").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("ddlFundPledgeDrive_2"), 20);

            /// Go to Next Item in Batch
            test.Driver.FindElementByLinkText("Next item").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("View Batch"));

            /// Verify Funds Retained
            SelectElement verifySelect = new SelectElement(test.Driver.FindElementById("ddlFundPledgeDrive_0"));
            SelectElement verifySelect2 = new SelectElement(test.Driver.FindElementById("ddlFundPledgeDrive_1"));
            Assert.AreEqual("1 - General Fund", verifySelect.SelectedOption.Text, "General Fund was not retained");
            Assert.AreEqual("2 - Building Fund", verifySelect2.SelectedOption.Text, "Building Fund was not retained");
            //test.GeneralMethods.VerifyTextPresentWebDriver("1 - General Fund");
            //test.GeneralMethods.VerifyTextPresentWebDriver("2 - Building Fund");

            /// Change Second Fund
            IWebElement reqTypeDropDown3 = test.Driver.FindElementById("ddlFundPledgeDrive_1");
            SelectElement select3 = new SelectElement(reqTypeDropDown3);
            select3.SelectByText("3 - Missions");

            /// Go to Next Item in Batch
            test.Driver.FindElementByLinkText("Next item").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("View Batch"));

            /// Verify All Funds Were Retained
            verifySelect = new SelectElement(test.Driver.FindElementById("ddlFundPledgeDrive_0"));
            verifySelect2 = new SelectElement(test.Driver.FindElementById("ddlFundPledgeDrive_1"));
            SelectElement verifySelect3 = new SelectElement(test.Driver.FindElementById("ddlFundPledgeDrive_2"));
            Assert.AreEqual("1 - General Fund", verifySelect.SelectedOption.Text, "General Fund was not retained");
            Assert.AreEqual("2 - Building Fund", verifySelect2.SelectedOption.Text, "Building Fund was not retained");
            Assert.AreEqual("3 - Missions", verifySelect3.SelectedOption.Text, "Missions was not retained");
            //test.GeneralMethods.VerifyTextPresentWebDriver("1 - General Fund");
            //test.GeneralMethods.VerifyTextPresentWebDriver("2 - Building Fund");
            //test.GeneralMethods.VerifyTextPresentWebDriver("3 - Missions");


            // Cancel out of batch
            test.Driver.FindElementByLinkText("RETURN").Click();
            test.Driver.FindElementByLinkText("RETURN").Click();

            // Logout of portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
            base.SQL.Giving_Batches_Delete_RDC(15, rdcBatchName, acctNumber, routNumber);
            //base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", indFirstName, indLastName), "Merge Dump");
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3455: Matches an RDC batch item to an Inactive individual")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Pending_Matching_InactiveIndividual()
        {
            // Setup data
            var rdcBatchName = "Test RDC Batch - Match Inactive Ind";
            var indFirstName = "FT";
            var indLastName = "InactiveIndividual";
            var acctNumber = "333444555";
            var routNumber = "111000025";
            base.SQL.Giving_Batches_Delete_RDC(15, rdcBatchName, acctNumber, routNumber);
            base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", indFirstName, indLastName), "Merge Dump");
            base.SQL.People_Individual_Create(15, indFirstName, indLastName, 106);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 1.11 }, null,
                new System.Collections.Generic.List<string> { acctNumber },
                new System.Collections.Generic.List<string> { routNumber }, false, "Auto Test Fund", false);

            try
            {
                // Login to portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // View a Pending RDC batch
                test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName);
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Match"));

                // Click on Match for a batch item
                test.Driver.FindElementByLinkText("Match").Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("View Batch"));

                // Match item to Inactive individual
                if (test.Driver.FindElements(By.LinkText("Change or unmatch")).Count > 0)
                {
                    test.Driver.FindElementByLinkText("Change or unmatch").Click();
                }
                test.Driver.FindElementById("find_individual_name").SendKeys(string.Format("{0} {1}", indFirstName, indLastName));
                test.Driver.FindElementByXPath("//input[@value='Search']").Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.XPath("//button[@class='choose_row']"));
                test.Driver.FindElementByXPath("//button[@class='choose_row']").Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Change or unmatch"));
                test.GeneralMethods.VerifyTextPresentWebDriver(string.Format("{0} {1}", indFirstName, indLastName));

                // Cancel out of batch
                test.Driver.FindElementByLinkText("RETURN").Click();
                test.Driver.FindElementByLinkText("RETURN").Click();

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }
            catch (Exception e)
            {
                throw new WebDriverException(e.StackTrace);
            }
            finally
            {
                // Clean up
                base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
                base.SQL.Giving_Batches_Delete_RDC(15, rdcBatchName, acctNumber, routNumber);
                base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", indFirstName, indLastName), "Merge Dump");
            }
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Sifter 7627: Verifies that once you save an RDC batch you cannot go back and save it again.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_Save_BackButton_SavedPage()
        {
            // Setup data
            var rdcBatchName = "Test RDC Batch - Saved Back Button";
            var indFirstName = "FT";
            var indLastName = "SavedBackButton";
            var acctNumber = "666444222";
            var routNumber = "111000025";
            base.SQL.Giving_Batches_Delete_RDC(15, rdcBatchName, acctNumber, routNumber);
            base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", indFirstName, indLastName), "Merge Dump");
            base.SQL.People_Individual_Create(15, indFirstName, indLastName, 1);
            base.SQL.Giving_Batches_Create_RDC(15, rdcBatchName, false, new System.Collections.Generic.List<double>() { 1.11 }, null,
                new System.Collections.Generic.List<string> { acctNumber },
                new System.Collections.Generic.List<string> { routNumber }, false, "Auto Test Fund", false);

            try
            {
                // Login to portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // View a Pending RDC batch
                test.Portal.Giving_Batches_ImportedBatches_View_Pending_WebDriver(rdcBatchName);
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Match"));

                // Click on Match for a batch item
                test.Driver.FindElementByLinkText("Match").Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("View Batch"));

                // Match item to individual
                if (test.Driver.FindElements(By.LinkText("Change or unmatch")).Count > 0)
                {
                    test.Driver.FindElementByLinkText("Change or unmatch").Click();
                }
                test.Driver.FindElementById("find_individual_name").SendKeys(string.Format("{0} {1}", indFirstName, indLastName));
                test.Driver.FindElementByXPath("//input[@value='Search']").Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.XPath("//button[@class='choose_row']"));
                test.Driver.FindElementByXPath("//button[@class='choose_row']").Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Change or unmatch"));
                test.GeneralMethods.VerifyTextPresentWebDriver(string.Format("{0} {1}", indFirstName, indLastName));

                // Click Next item
                test.Driver.FindElementByLinkText("Next item").Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Match"));

                // Select Fund
                IWebElement fund = test.Driver.FindElementById("ddlFundPledgeDrive_0");
                SelectElement select = new SelectElement(fund);
                select.SelectByText("Auto Test Fund");

                // Save batch
                test.Driver.FindElementById("btn_submit").Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Refresh batch listing"));

                // Click the back button on web browser
                test.Driver.Navigate().Back();

                // Verify you are on the Saved batch page
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText(string.Format("{0} {1}", indFirstName, indLastName)));
                test.GeneralMethods.VerifyTextPresentWebDriver("Saved Batch");

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }
            catch (Exception e)
            {
                throw new WebDriverException(e.StackTrace);
            }
            finally
            {

                // Clean up
                base.SQL.Giving_Batches_Delete(15, rdcBatchName, 3);
                base.SQL.Giving_Batches_Delete_RDC(15, rdcBatchName, acctNumber, routNumber);
                base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", indFirstName, indLastName), "Merge Dump");
            }
        }


        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to view the remote deposit capture page w/out the contribution write security right.")]
        [Category(TestCategories.Services.RDCChecker)]
        public void Giving_Contributions_Batches_RemoteDepositCapture_NoContributionWrite()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("CR_ContWrite", "Pa$$w0rd");

            // Attempt to navigate to the remote deposit capture landing page
            test.GeneralMethods.OpenURLExpecting403_WebDriver(string.Format("{0}Payment/RDCBatch/Index.aspx?isSearch=False", PortalBase.GetPortalURL(this.F1Environment).ToLower()));

        }


        #endregion Misc

        #region Completed Batch Screen
        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3572: Verifies the Completed Batches screen updates the contributor name when the contribution has been updated.")]
        public void Giving_Contributions_Batches_RemoteDepositCapture_CompletedPage_UpdatedContributorName()
        {
            // Setup batch
            string batchName = "RDC Batch Complete - Update Contributor";
            string individual = "Kevin Spacey";
            string updatedIndividual = "RDC Edit";
            string fund = "Auto Test Fund";
            string account = "606303301";
            string routing = "111000025";
            int amount = 521;
            base.SQL.Giving_Batches_Delete(15, batchName, 3);
            base.SQL.Giving_Batches_Delete_RDC(15, batchName, account, routing);
            base.SQL.Giving_Batches_Create_RDC(15, batchName, true, new System.Collections.Generic.List<double>() { amount }, new System.Collections.Generic.List<string> { individual },
                new System.Collections.Generic.List<string> { account },
                new System.Collections.Generic.List<string> { routing }, false, fund, false);

            try
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // View Completed batch screen
                test.Portal.Giving_Batches_CompletedBatch_View(GeneralEnumerations.BatchTypes.RDC, batchName);

                // Edit a contribution within the batch and change the contributor
                test.Portal.Giving_EditContribution(individual, string.Format("${0}.00", amount), null, updatedIndividual, updatedIndividual, null, null, null);

                // View Completed batch screen again
                test.Portal.Giving_Batches_CompletedBatch_View(GeneralEnumerations.BatchTypes.RDC, batchName);

                // Verify the contributor name was updated
                test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(updatedIndividual));

                // Logout of Portal
                test.Portal.LogoutWebDriver();

            }
            catch (Exception e)
            {
                throw new WebDriverException(e.StackTrace);
            }
            finally
            {
                // Clean up
                base.SQL.Giving_Batches_Delete(15, batchName, 3);
                base.SQL.Giving_Batches_Delete_RDC(15, batchName, account, routing);
            }

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3572: Verifies the Completed Batches screen updates the fund name when the contribution has been updated.")]
        public void Giving_Contributions_Batches_RemoteDepositCapture_CompletedPage_UpdatedFundName()
        {
            // Setup batch
            string batchName = "RDC Batch Complete - Update Fund Name";
            string individual = "RDC Edit";
            string fund = "Auto Test Fund";
            string updatedFund = "1 - General Fund";
            int amount = 555;
            string account = "600300100";
            string routing = "111000025";
            base.SQL.Giving_Batches_Delete_RDC(15, batchName, account, routing);
            base.SQL.Giving_Batches_Create_RDC(15, batchName, true, new System.Collections.Generic.List<double>() { amount }, new System.Collections.Generic.List<string> { individual },
                new System.Collections.Generic.List<string> { account },
                new System.Collections.Generic.List<string> { routing }, false, fund, false);

            try
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // View Completed batch screen
                test.Portal.Giving_Batches_CompletedBatch_View(GeneralEnumerations.BatchTypes.RDC, batchName);

                // Edit a contribution within the batch and change the contributor
                test.Portal.Giving_EditContribution(individual, string.Format("${0}.00", amount), null, null, individual, updatedFund, null, null);

                // View Completed batch screen again
                test.Portal.Giving_Batches_CompletedBatch_View(GeneralEnumerations.BatchTypes.RDC, batchName);

                // Verify the contributor name was updated
                test.GeneralMethods.VerifyTextPresentWebDriver(updatedFund);

                // Logout of Portal
                test.Portal.LogoutWebDriver();

            }
            catch (Exception e)
            {
                throw new WebDriverException(e.StackTrace);
            }
            finally
            {
                // Clean up
                base.SQL.Giving_Batches_Delete(15, batchName, 3);
                base.SQL.Giving_Batches_Delete_RDC(15, batchName, account, routing);
            }

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3572: Verifies the Completed Batches screen updates the amount when the contribution has been updated.")]
        public void Giving_Contributions_Batches_RemoteDepositCapture_CompletedPage_UpdatedAmount()
        {
            // Setup batch
            string batchName = "RDC Batch Complete - Update Amount Name";
            string individual = "RDC Edit";
            string fund = "Auto Test Fund";
            int amount = 777;
            int updatedAmount = 96;
            string account = "101202303";
            string routing = "111000025";
            base.SQL.Giving_Batches_Delete(15, batchName, 3);
            base.SQL.Giving_Batches_Delete_RDC(15, batchName, account, routing);
            base.SQL.Giving_Batches_Create_RDC(15, batchName, true, new System.Collections.Generic.List<double>() { amount }, new System.Collections.Generic.List<string> { individual },
                new System.Collections.Generic.List<string> { account },
                new System.Collections.Generic.List<string> { routing }, false, fund, false);

            try
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // View Completed batch screen
                test.Portal.Giving_Batches_CompletedBatch_View(GeneralEnumerations.BatchTypes.RDC, batchName);

                // Edit a contribution within the batch and change the contributor
                test.Portal.Giving_EditContribution(individual, string.Format("${0}.00", amount), null, null, individual, null, null, updatedAmount.ToString());

                // View Completed batch screen again
                test.Portal.Giving_Batches_CompletedBatch_View(GeneralEnumerations.BatchTypes.RDC, batchName);

                // Verify the amount was updated
                test.GeneralMethods.VerifyTextPresentWebDriver(string.Format("${0}", updatedAmount));

                // Logout of Portal
                test.Portal.LogoutWebDriver();

            }
            catch (Exception e)
            {
                throw new WebDriverException(e.StackTrace);
            }
            finally
            {
                // Clean up
                base.SQL.Giving_Batches_Delete(15, batchName, 3);
                base.SQL.Giving_Batches_Delete_RDC(15, batchName, account, routing);
            }

        }

        #endregion Completed Batch Screen

        #endregion Remote Deposit Capture
    }

    [TestFixture]
    public class Portal_Giving_Contributions_Batches_Contributions_WebDriver : FixtureBaseWebDriver
    {

        #region Private Members

        private string _pledgeDriveStartDate;
        private string _fundID;
        private string _subFundID;
        private string _pledgeDriveName;
        private string _fundName;
        private string _subFundName;
        private DataTable _pledgeDrive;

        private string _ccBatchNameMISC = "Contribution: MISCWD";
        private string _ccBatchNameMISCZero = "Contribution: MISCWD-0";

        #endregion Private Members

        [FixtureSetUp]
        public void FixtureSetUp()
        {

            // Create a credit card batch for miscellaneous 'contribution' state tests
            base.SQL.Giving_Batches_Delete(15, _ccBatchNameMISC, 2);
            base.SQL.Giving_Batches_Delete(15, _ccBatchNameMISCZero, 2);
            
            _pledgeDriveStartDate = base.SQL.Execute("DECLARE @startDate DATETIME SET @startDate = (SELECT TOP 1 [START_DATE] FROM [ChmContribution].[dbo].[PLEDGE_DRIVE] WITH (NOLOCK) WHERE CHURCH_ID = 15) SELECT CONVERT(VARCHAR, @startDate, 1)").Rows[0][0].ToString();
            _pledgeDrive = base.SQL.Execute("SELECT TOP 1 [PLEDGE_DRIVE_NAME], [FUND_ID], [SUB_FUND_ID] FROM [ChmContribution].[dbo].[PLEDGE_DRIVE] WITH (NOLOCK) WHERE CHURCH_ID = 15");
            _fundID = _pledgeDrive.Rows[0]["FUND_ID"].ToString();
            _subFundID = _pledgeDrive.Rows[0]["SUB_FUND_ID"].ToString();
            _pledgeDriveName = _pledgeDrive.Rows[0]["PLEDGE_DRIVE_NAME"].ToString();

            // Get the parent fund name
            _fundName = base.SQL.Execute(string.Format("SELECT TOP 1 [FUND_NAME] FROM [ChmContribution].[dbo].[FUND] WITH (NOLOCK) WHERE [FUND_ID] = {0}", _fundID)).Rows[0][0].ToString();

            // Get the sub fund name if present
            if (!string.IsNullOrEmpty(_subFundID))
            {
                _subFundName = base.SQL.Execute(string.Format("SELECT TOP 1 [SUB_FUND_NAME] FROM [ChmContribution].[dbo].[SUB_FUND] WITH (NOLOCK) WHERE [SUB_FUND_ID] = {1}", _subFundID)).Rows[0][0].ToString();
            }

            base.SQL.Giving_Batches_Create(15, _ccBatchNameMISC, 100, 2);
            base.SQL.Giving_Batches_Create(15, _ccBatchNameMISCZero, 100, 2);


        }


        #region Contribution
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the available gear options for a credit card batch in the contribution state.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Gear_Contribution()
        {
            // Set initial conditions
            string batchName = "Gear: Contribution";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, 100, 2);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to giving->batches->credit card batches
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Batches_CreditCardBatches);

            // Verify the gear options for a batch in the contribution state
            int itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName, "Name");
            //Assert.AreEqual(4, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]/ul/li", TableIds.Giving_Batches, itemRow + 1)).Size);
            Assert.AreEqual(4, test.Driver.FindElements(By.XPath(string.Format("{0}/tbody/tr[{1}]/td[5]/ul/li", TableIds.Giving_Batches, itemRow + 1))).Count);

            test.Driver.FindElementByXPath(string.Format("//table[*]/tbody/tr[{0}]/td[*]/a[@class='gear_trigger']", itemRow + 1)).Click();

            Assert.AreEqual("Resume progress", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]/ul/li[1]/a", TableIds.Giving_Batches, itemRow + 1)).Text);
            Assert.AreEqual("View batch", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]/ul/li[2]/a", TableIds.Giving_Batches, itemRow + 1)).Text);
            Assert.AreEqual("Edit batch", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]/ul/li[3]/a", TableIds.Giving_Batches, itemRow + 1)).Text);
            Assert.AreEqual("Delete batch", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]/ul/li[4]/a", TableIds.Giving_Batches, itemRow + 1)).Text);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies that the user is unable to delete a credit card batch containing one or more contributions.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Gear_ContributionWithContribution()
        {
            // Set initial conditions
            string batchName = "Gear: Contribution w/Contribution";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, 100, 2);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Add a contribution to the batch
            test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(batchName, "Matthew Sneeden", "Visa", "4111-1111-1111-1111", "01", "2020", "100", "1 - General Fund", null, null);

            // Navigate to giving->batches->credit card batches
            test.Driver.FindElementByLinkText(GeneralLinksWebDriver.RETURN).Click();

            // Verify the gear options for a batch in the contribution state with one or more contributions
            int itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName, "Name");
            //Assert.AreEqual(3, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]/ul/li", TableIds.Giving_Batches, itemRow + 1)).Size);
            //Assert.AreEqual(3, test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]/ul/", TableIds.Giving_Batches, itemRow + 1)).FindElements(By.TagName("li")).Count);

            test.Driver.FindElementByXPath(string.Format("//table[*]/tbody/tr[{0}]/td[*]/a[@class='gear_trigger']", itemRow + 1)).Click();
            Assert.AreEqual(3, test.Driver.FindElements(By.XPath(string.Format("{0}/tbody/tr[{1}]/td[5]/ul/li", TableIds.Giving_Batches, itemRow + 1))).Count);
            Assert.AreEqual("Resume progress", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]/ul/li[1]/a", TableIds.Giving_Batches, itemRow + 1)).Text);
            Assert.AreEqual("View batch", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]/ul/li[2]/a", TableIds.Giving_Batches, itemRow + 1)).Text);
            Assert.AreEqual("Edit batch", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]/ul/li[3]/a", TableIds.Giving_Batches, itemRow + 1)).Text);

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies that correct pledge drives are displayed based on the received date.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Contribution_PledgeDrive()
        {
            // Set initial conditions
            string batchName = "Pledge Drive";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Create a credit card batch
            test.Portal.Giving_Batches_Create_WebDriver(batchName, 100, true, _pledgeDriveStartDate);

            // Resume progress
            test.Portal.Giving_Batches_CreditCardBatches_ResumeProgressWebDriver(batchName);

            // Search for an individual
            test.Portal.Giving_SearchIndividual_WebDriver("Matthew Sneeden", null);

            // Verify pledge drive is present
            new SelectElement(test.Driver.FindElementById("ddlFund_1")).SelectByText(_fundName);

            if (!string.IsNullOrEmpty(_subFundName))
            {
                new SelectElement(test.Driver.FindElementById("ddlSubfund_1")).SelectByText(_subFundName);
            }


            WebDriverWait wait = new WebDriverWait(test.Driver, TimeSpan.FromSeconds(60));
            IWebElement element = wait.Until(ExpectedConditions.ElementExists(By.Id("ddlPledgedrive_1")));
            new SelectElement(test.Driver.FindElementById("ddlPledgedrive_1")).SelectByText(_pledgeDriveName);

            //Comments below code, since no need to judge the environment now
            /*
            if (this.F1Environment == F1Environments.LV_QA)
            {
                // Type vida card number
                test.Driver.FindElementById("card_number").SendKeys("4111111111111111");

                // Type contribution amount
                test.Driver.FindElementById("amount_1").Clear();
                test.Driver.FindElementById("amount_1").SendKeys("100");

                new SelectElement(test.Driver.FindElementById("card_expiration_month")).SelectByText("10");
                new SelectElement(test.Driver.FindElementById("card_expiration_year")).SelectByIndex(6);
            }
            else
            {*/
                // Type vida card number
                test.Driver.FindElementById("account_number").Clear();
                test.Driver.FindElementById("account_number").SendKeys("4111111111111111");

                // Type contribution amount
                test.Driver.FindElementById("amount_1").Clear();
                test.Driver.FindElementById("amount_1").SendKeys("100");

                new SelectElement(test.Driver.FindElementById("expiration_month")).SelectByText("10");
                new SelectElement(test.Driver.FindElementById("expiration_year")).SelectByIndex(6);
            //}

            test.Driver.FindElementById("submitQuery").Click();
            test.Driver.FindElementByLinkText(GeneralLinksWebDriver.RETURN).Click();

            // Edit the batch and adjust the received date
            test.GeneralMethods.SelectOptionFromGearWebDriver(Convert.ToInt16(test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName, "Name")), "Edit batch");

            //Keeping these as reference
            //test.Selenium.Focus("received_date");
            //test.Selenium.Focus("//input[@id='submitQuery' or @id='ctl00_content_btnSave' or @id='btn_submit']");
            //test.Selenium.KeyDown("received_date", "\\40");
            //test.Selenium.KeyUp("received_date", "\\40");

            string rcvDate = test.Driver.FindElementById("received_date").GetAttribute("value");
            TestLog.WriteLine(string.Format("RCV DATE: {0}", rcvDate));
            string button = test.Driver.FindElementByXPath("//input[@id='submitQuery' or @id='ctl00_content_btnSave' or @id='btn_submit']").GetAttribute("value");
            TestLog.WriteLine(string.Format("  BUTTON: {0}", button));
            test.Driver.FindElementById("received_date").Clear();
            test.Driver.FindElementById("received_date").SendKeys(DateTime.Parse(rcvDate).AddDays(-1).ToString("MM/dd/yyyy"));

            //test.Driver.FindElementById("received_date").SendKeys(Keys.Down);
            //test.Driver.FindElementById("received_date").SendKeys(Keys.Up);


            //test.Selenium.ClickAndWaitForPageToLoad("//input[@id='submitQuery']");
            test.Driver.FindElementById("submitQuery").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(TableIds.Giving_Batches));

            test.GeneralMethods.SelectOptionFromGearWebDriver(Convert.ToInt16(test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName, "Name")), "Resume progress");

            // Search for an individual
            test.Portal.Giving_SearchIndividual_WebDriver("Matthew Sneeden", null);

            // Verify pledge drive is not present
            new SelectElement(test.Driver.FindElementById("ddlFund_1")).SelectByText(_fundName);
            if (!string.IsNullOrEmpty(_subFundName))
            {
                new SelectElement(test.Driver.FindElementById("ddlSubfund_1")).SelectByText(_subFundName);
            }
              
            IWebElement pledgeElement = wait.Until(ExpectedConditions.ElementExists(By.Id("ddlPledgedrive_1")));
            SelectElement selectElement = new SelectElement(test.Driver.FindElementById("ddlPledgedrive_1"));
            int selectCount = selectElement.Options.Count;
            bool driveFound = false;
            for (int i = 0; i < selectCount; i++)
            {
                //Assert.AreNotEqual(_pledgeDriveName, selectElement.Options.ElementAt(i).Text.Trim(), "Pledge Drive Found");
                if (_pledgeDriveName.Equals(selectElement.Options.ElementAt(i).Text.Trim()))
                    driveFound = true;

            }

            //Modify the checkpoint according the previous comments
            Assert.IsFalse(driveFound, "The Pledge drive was found in the list");
            //Assert.IsTrue(driveFound, "The Pledge drive was not found in the list");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the correct error message if the user attempts to add a zero dollar contribution.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Contribution_ZeroDollar()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Attempt to add a zero dollar contribution to a batch
            test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(this._ccBatchNameMISCZero, "Matthew Sneeden", "Visa", "4111111111111111", "12", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).ToString("yyyy"), "0", "1 - General Fund", null, null);

            // Logout of Portal 
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies that error messages do not remain on the screen if the user changes the selected individual.")]
        [Category(TestCategories.Services.PaymentProcessor), Category(TestCategories.Services.SmokeTest)]
        public void Giving_Contributions_Batches_CreditCardBatches_Contribution_ChangeAfterErrors()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Search for and select an individual
            test.Portal.Giving_Batches_CreditCardBatches_ResumeProgressWebDriver(this._ccBatchNameMISC);
            test.Portal.Giving_SearchIndividual_WebDriver("Matthew Sneeden", null);

            // Submit with missing information to display errors
            test.Driver.FindElementByXPath("//input[@value='Save to batch']").Click();

            // Change person
            test.Driver.FindElementByLinkText("Change").Click();

            // Verify error messages are no longer present
            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(TextConstants.ErrorHeadingSingular));
            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(TextConstants.ErrorHeadingPlural));

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to perform a search without providing any data.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Contribution_SearchNoData()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Resume progress
            test.Portal.Giving_Batches_CreditCardBatches_ResumeProgressWebDriver(this._ccBatchNameMISCZero);

            WebDriverWait wait = new WebDriverWait(test.Driver, TimeSpan.FromSeconds(60));

            // Search for an individual with no data provided
            if (test.SQL.IsAMSEnabled(test.Portal.ChurchID))
            {
                test.Driver.FindElementById("findIndividualSearch").Click();
                IWebElement element = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("findIndividualSearch")));
                test.GeneralMethods.VerifyTextPresentWebDriver("Sorry, no results found. Please try searching on different criteria.");
            }
            else
            {
                test.Driver.FindElementByXPath("//button[@type='submit' and text()='Search']").Click();
                IWebElement element = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[@type='submit' and text()='Search']")));

                TestLog.WriteLine(test.Driver.FindElementById("add_contributor_search_results").Text.Trim());
                // Verify the user is prompted regarding the search criteria
                Assert.AreEqual("Sorry, no results found. Please try searching on different criteria.", test.Driver.FindElementById("add_contributor_search_results").Text.Trim());
            }

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure, Timeout(1000)]
        [Author("Matthew Sneeden")]
        [Description("Attempts to perform a search for an individual with only one character.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Contribution_SearchNameOneCharacter()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Resume progress
            test.Portal.Giving_Batches_CreditCardBatches_ResumeProgressWebDriver(this._ccBatchNameMISCZero);

            WebDriverWait wait = new WebDriverWait(test.Driver, TimeSpan.FromSeconds(60));

            // Search for an individual with only one character
            if (test.SQL.IsAMSEnabled(test.Portal.ChurchID))
            {
                test.Driver.FindElementById("SearchName").SendKeys("m");
                test.Driver.FindElementById("findIndividualSearch").Click();
                IWebElement element = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("findIndividualSearch")));
                test.GeneralMethods.VerifyTextPresentWebDriver("Sorry, no results found. Please try searching on different criteria.");
            }
            else
            {
                test.Driver.FindElementById("find_individual_name").SendKeys("m");
                test.Driver.FindElementByXPath("//button[@type='submit' and text()='Search']").Click();
                IWebElement element = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[@type='submit' and text()='Search']")));
                TestLog.WriteLine(test.Driver.FindElementById("add_contributor_search_results").Text.Trim());
                // Verify the user is prompted regarding the search criteria
                Assert.AreEqual("Sorry, no results found. Please try searching on different criteria.", test.Driver.FindElementById("add_contributor_search_results").Text.Trim());
            }


            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to perform a search for an individual with special characters.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Contribution_SearchSpecialCharacter()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Resume progress
            test.Portal.Giving_Batches_CreditCardBatches_ResumeProgressWebDriver(this._ccBatchNameMISCZero);

            WebDriverWait wait = new WebDriverWait(test.Driver, TimeSpan.FromSeconds(60));

            // Search for an individual with only one character
            if (test.SQL.IsAMSEnabled(test.Portal.ChurchID))
            {
                test.Driver.FindElementById("SearchName").SendKeys("<b>");
                test.Driver.FindElementById("findIndividualSearch").Click();

                IWebElement element = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("findIndividualSearch")));

                test.GeneralMethods.VerifyTextPresentWebDriver("Sorry, no results found. Please try searching on different criteria.");
            }
            else
            {
                test.Driver.FindElementById("find_individual_name").SendKeys("<b>");
                //icon_alert
                test.Driver.FindElementByXPath("//button[@type='submit' and text()='Search']").Click();

                IWebElement element = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[@type='submit' and text()='Search']")));
                //test.GeneralMethods.WaitForElement(test.Driver, By.Id("add_contributor_search_results"));
                TestLog.WriteLine(test.Driver.FindElementById("add_contributor_search_results").Text.Trim());
                // Verify the user is prompted regarding the search criteria
                Assert.AreEqual("Sorry, no results found. Please try searching on different criteria.", test.Driver.FindElementById("add_contributor_search_results").Text.Trim());
            }

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the Add Contributions page in Giving and attempts to save a contribution with an expired credit card.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Contribution_ExpiredCreditCard()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Cannot do an expired credit card in January.
            if (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month.ToString("MM") == "01")
            {
                Assert.Inconclusive("This test cannot run in January because you cannot provide an expired credit card. The previous year is not an option to select for a credit card's expiration year.  January is the first month of the year, so there is no past months this year.");

            }
            else
            {
                // Attempt to add a contribution to a credit card batch using an expired credit card
                test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(this._ccBatchNameMISC, "Matthew Sneeden", "Visa", "4111111111111111", "01", DateTime.Now.Year.ToString(), "100", "1 - General Fund", null, null);
            }

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the Add Contributions page in Giving and attempts to save a contribution without any " +
            "of the required fields populated.  Verifies the error message appears below the wizard.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Contribution_MissingAllFields()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Resume progress
            test.Portal.Giving_Batches_CreditCardBatches_ResumeProgressWebDriver(this._ccBatchNameMISC);

            // Search for, select an individual
            test.Portal.Giving_SearchIndividual_WebDriver("Matthew Sneeden", null);

            if (test.SQL.IsAMSEnabled(test.Portal.ChurchID))
            {
                // Delete content from the payment fields
                test.Driver.FindElementById("FirstName").Clear();
                test.Driver.FindElementById("LastName").Clear();
                test.Driver.FindElementById("PostalCode").Clear();

            }
            else
            {
                // Delete content from the payment fields
                test.Driver.FindElementById("card_first_name").Clear();
                test.Driver.FindElementById("card_last_name").Clear();
                test.Driver.FindElementById("b_zip").Clear();
            }

            test.Driver.FindElementById("b_address1").Clear();
            test.Driver.FindElementById("b_city").Clear();
            new SelectElement(test.Driver.FindElementById("s_state")).SelectByText("--");

            // Create additional contribution rows
            test.Driver.FindElementByXPath("//a[@id='add_row']").Click();
            test.Driver.FindElementByXPath("//a[@id='add_row']").Click();
            test.Driver.FindElementByXPath("//a[@id='add_row']").Click();

            // Attempt to add the contribution
            test.Driver.FindElementByXPath("//input[@value='Save to batch']").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Change"));

            // Verify the errors present
            test.GeneralMethods.VerifyTextPresentWebDriver(TextConstants.ErrorHeadingPlural);
            test.GeneralMethods.VerifyTextPresentWebDriver("The 1st contribution is missing amount");
            test.GeneralMethods.VerifyTextPresentWebDriver("The 1st contribution is missing fund");
            test.GeneralMethods.VerifyTextPresentWebDriver("The 2nd contribution is missing amount");
            test.GeneralMethods.VerifyTextPresentWebDriver("The 2nd contribution is missing fund");
            test.GeneralMethods.VerifyTextPresentWebDriver("The 3rd contribution is missing amount");
            test.GeneralMethods.VerifyTextPresentWebDriver("The 3rd contribution is missing fund");
            test.GeneralMethods.VerifyTextPresentWebDriver("The 4th contribution is missing amount");
            test.GeneralMethods.VerifyTextPresentWebDriver("The 4th contribution is missing fund");
            test.GeneralMethods.VerifyTextPresentWebDriver("First name is required.");
            test.GeneralMethods.VerifyTextPresentWebDriver("Last name is required.");
            test.GeneralMethods.VerifyTextPresentWebDriver("Address1 is required.");
            test.GeneralMethods.VerifyTextPresentWebDriver("City is required.");
            test.GeneralMethods.VerifyTextPresentWebDriver("State/Province is required.");
            test.GeneralMethods.VerifyTextPresentWebDriver("Postal code is required.");
            test.GeneralMethods.VerifyTextPresentWebDriver("Card number is required");

            // Verify the error message(s) appear below the wizard            
            //Assert.IsTrue(test.Selenium.GetElementPositionTop("//table[@id='step_wizard']") < test.Selenium.GetElementPositionTop("//dl[@id='error_message']"));
            TestLog.WriteLine(string.Format("Step Wizard {0}, {1}", test.Driver.FindElementByXPath("//table[@id='step_wizard']").Location.X, test.Driver.FindElementByXPath("//table[@id='step_wizard']").Location.Y));
            TestLog.WriteLine(string.Format("Error Message: {0}, {1}", test.Driver.FindElementByXPath("//dl[@id='error_message']").Location.X, test.Driver.FindElementByXPath("//dl[@id='error_message']").Location.Y));
            Assert.IsTrue(test.Driver.FindElementByXPath("//table[@id='step_wizard']").Location.Y < test.Driver.FindElementByXPath("//dl[@id='error_message']").Location.Y);

            // Verify the received date is still present
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            Assert.AreEqual(now.ToString("M/d/yyyy"), test.Driver.FindElementByXPath("//table[@class='info']/tbody/tr/td").Text);

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies error handling when an invalid card number is provided using Visa.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Contribution_InvalidVisa()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Add a contribution to a batch
            test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(this._ccBatchNameMISC, "Matthew Sneeden", "Visa", "111", "12", "2020", "2000", "1 - General Fund", null, null);

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies error handling when an invalid card number is provided using MasterCard.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Contribution_InvalidMasterCard()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Add a contribution a batch
            test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(this._ccBatchNameMISC, "Matthew Sneeden", "MasterCard", "111", "12", "2020", "2000", "1 - General Fund", null, null);

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies error handling when an invalid card number is provided using American Express.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Contribution_InvalidAmericanExpress()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Add a contribution to the batch
            test.Portal.Giving_Batches_CreditCardBatches_AddContributions_WebDriver(this._ccBatchNameMISC, "Matthew Sneeden", "American Express", "111", "12", "2020", "2000", "1 - General Fund", null, null);

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the data returned to the user when searching for an individual in the contribution phase of credit card batches.")]
        [Category(TestCategories.Services.PaymentProcessor)]
        public void Giving_Contributions_Batches_CreditCardBatches_Contribution_IndividualSearch()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Add a contribution to the batch
            test.Portal.Giving_Batches_CreditCardBatches_ResumeProgressWebDriver(this._ccBatchNameMISC);

            // Search for an individual and verify the data returned
            StringBuilder query = new StringBuilder("SELECT TOP 1 (IND.FIRST_NAME + ' ' + IND.LAST_NAME), STAT.STATUS_NAME, STATSEL.LIST_NAME, DATEDIFF(day, IND.DATE_OF_BIRTH, CURRENT_TIMESTAMP)/365 AS DATE_OF_BIRTH FROM ChmPeople.dbo.INDIVIDUAL AS IND WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmPeople.dbo.STATUS AS STAT WITH (NOLOCK) ");
            query.Append("ON IND.STATUS_ID = STAT.STATUS_ID ");
            query.Append("INNER JOIN ChmPeople.dbo.STATUS_SELECTLIST AS STATSEL WITH (NOLOCK) ");
            query.Append("ON IND.STATUS_SELECTLIST_ID = STATSEL.STATUS_SELECTLIST_ID ");
            query.Append("WHERE IND.CHURCH_ID = 15 AND IND.DATE_OF_BIRTH IS NOT NULL AND (IND.FIRST_NAME+IND.LAST_NAME) not like '% %'");
            DataTable results = base.SQL.Execute(query.ToString());

            if (test.SQL.IsAMSEnabled(test.Portal.ChurchID))
            {
                test.Driver.FindElementById("SearchName").SendKeys(results.Rows[0][0].ToString());
                test.Driver.FindElementById("findIndividualSearch").Click();
            }
            else
            {
                test.Driver.FindElementById("find_individual_name").SendKeys(results.Rows[0][0].ToString());
                //test.Selenium.ClickAndWaitForCondition("//button[@type='submit']", JavaScriptMethods.IsElementPresent("find_individual_results"), "30000");
                test.Driver.FindElementByXPath("//button[@type='submit']").Click();
            }

            test.GeneralMethods.WaitForElement(test.Driver, By.Id("find_individual_results"), 30, "Did not find any individual results");
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath("//table[@class='grid select_row']/tbody/tr[2]/td[2]"), 30, "Did not find any individual results");

            TestLog.WriteLine(results.Rows[0][1].ToString().Trim());
            TestLog.WriteLine(test.Driver.FindElementByXPath("//table[@class='grid select_row']/tbody/tr[2]/td[2]").Text.Trim());
            //Assert.AreEqual(string.Format("{0}\r\n{1} > {2}", results.Rows[0][0].ToString().Trim(), results.Rows[0][1].ToString().Trim(), results.Rows[0][2].ToString().Trim()), test.Driver.FindElementByXPath("//table[@class='grid select_row']/tbody/tr[2]/td[2]").Text.Trim());
            //modify grace zhang:Loop to get value to determine whether there is a valid info on page
            IWebElement tboday = test.Driver.FindElementByCssSelector("table[class='grid select_row']").FindElement(By.TagName("tbody"));
            int trcount = tboday.FindElements(By.TagName("tr")).Count;
            String expectValue = results.Rows[0][0].ToString().Trim() +"\r\n" + results.Rows[0][1].ToString().Trim()+ " > " + results.Rows[0][2].ToString().Trim();
            bool isExist = false;
            int index = 0;
            for (int i = 0; i < trcount; i++)
            {
                if (i == 0)
                {
                    continue;
                }
               String actualValue = test.Driver.FindElementByXPath("//table[@class='grid select_row']/tbody/tr["+Convert.ToString(i+1)+"]/td[2]").Text.Trim();
               if(actualValue.Equals(expectValue))
               {
                   isExist = true;
                   index = i;
                   break;
               }
            }
            Assert.IsTrue(isExist);

            Assert.AreEqual(string.Format("{0} yrs.", results.Rows[0][3].ToString().Trim()), test.Driver.FindElementByXPath("//table[@class='grid select_row']/tbody/tr[" + Convert.ToString(index + 1) + "]/td[3]").Text);

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Contribution

       

    }

    [TestFixture]
    public class Portal_Giving_Contributions_Batches_UnmatchedWebDriver : FixtureBaseWebDriver {

        # region Login
        private string userName = "ft.tester";
        private string password = "FT4life!";
        private string churchCode = "dc";
        #endregion Login

        #region View

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3503: Verifies you can navigate to the new unmatched contributions screen.")]
        public void Giving_Contributions_Batches_ViewUnmatchedContributions_ModuleOn() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver(userName, password, churchCode);

            // Navigate to the new Unmatched Contributions screen
            test.Portal.Giving_Batches_ViewUnmatched_All(15, true);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3503: Verifies you cannot view the new unmatched contributions screen with the module turned off.")]
        public void Giving_Contributions_Batches_ViewUnmatchedContributions_ModuleOff() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver(userName, password, "QAEUNLX0C4");

            // Navigate to Batches screen and show View Unmatched Contributions link not available
            test.Portal.Giving_Batches_ViewUnmatched_All(256, true);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3503: Verifies a portal user without contribution write access cannot view unmatched contributions.")]
        public void Giving_Contributions_Batches_ViewUnmatchedContributions_NoWriteAccess() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.nocontwrite", password, churchCode);

            // Navigate to Scanned Contributions showing no access to unmatched
            test.Portal.Giving_Batches_ViewUnmatched_All(15, false);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3503: Verifies a portal user without contribution write access cannot view unmatched contribution through the direct URL.")]
        public void Giving_Contributions_Batches_ViewUnmatchedContributions_NoWriteAccess_URL() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.nocontwrite", password, churchCode);

            // Navigate to the Unmatched Contributions page through direct URL
            test.GeneralMethods.OpenURLWebDriver(string.Format("{0}/bridge/Batches/batch/UnmatchedContributions", PortalBase.GetPortalURL(this.F1Environment).ToLower()));
            
            // Verify 404 error
            test.GeneralMethods.VerifyTextPresentWebDriver("404");

            // End test session
            test.Driver.Quit();

        }
        #endregion View

        #region Search All

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3503: Verifies changing the Start/End Dates on the unmatched All page will automatically updates the other date field to 90 days out.")]
        public void Giving_Contributions_Batches_ViewUnmatchedContributions_Search_All_MoreThan90Days() {
            // Set Variables
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            var start = today.AddYears(-2);
            var end = today;
            
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver(userName, password, churchCode);

            // Navigate to the View Unmatched Contributions page
            test.Portal.Giving_Batches_ViewUnmatched_All(15, true);

            // Change the Start Date for date range search
            test.Portal.Giving_Batches_ViewUnmatched_All_Search(start, end);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3503: Verifies the proper message is displayed when there are no unmatched contributions within the date range search.")]
        public void Giving_Contributions_Batches_ViewUnmatchedContributions_Search_All_NoResultsFound() {
            // Set Variables
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            var start = today.AddDays(2);
            var end = today.AddYears(2);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver(userName, password, churchCode);

            // Navigate to the View Unmatched Contributions page
            test.Portal.Giving_Batches_ViewUnmatched_All(15, true);

            // Search for unmatched contributions resulting in zero results
            test.Portal.Giving_Batches_ViewUnmatched_All_Search(start, end);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        #endregion Search All

        #region Match All

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3505: Verifies a contribution can be matched to an existing individual through the unmatched contributions All page.")]
        public void Giving_Contributions_Batches_ViewUnmatchedContributions_All_Match_Individual() {
            // Setup data
            string batchName = "Scanned Contribution - Ind";
            string firstName = "FT";
            string lastName = "MatchIndividual";
            base.SQL.People_Individual_Create(15, firstName, lastName, 1); 
            base.SQL.Giving_Batches_Create_Scanned(15, batchName, true, new System.Collections.Generic.List<double>() { 3505.00 }, null,
                new System.Collections.Generic.List<string> { "666333111" },
                new System.Collections.Generic.List<string> { "111000025" }, false, "Auto Test Fund", false);

            try {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver(userName, password, churchCode);

                // Match contribution to existing individual
                test.Portal.Giving_Batches_ViewUnmatched_MatchContribution(false, batchName, "$3,505.00", string.Format("{0} {1}", firstName, lastName), true, null, null, null, false);

                // Delete the batch
                test.Portal.Giving_Batches_ScannedContribution_DeleteBatch(true, batchName);

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            catch (Exception e) {
                throw new WebDriverException(e.StackTrace);
            }
            finally {
                // Clean Up
                base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", firstName, lastName), "Merge Dump");
                base.SQL.Giving_Batches_Delete(15, batchName, 4);
            }

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3505: Verifies a contribution can be matched to an existing household through the unmatched contributions All page.")]
        public void Giving_Contributions_Batches_ViewUnmatchedContributions_All_Match_Household() {
            // Setup data
            string batchName = "Scanned Contribution - HH";
            string firstName = "FT";
            string lastName = "MatchHousehold";
            base.SQL.People_Individual_Create(15, firstName, lastName, 1);
            base.SQL.Giving_Batches_Create_Scanned(15, batchName, true, new System.Collections.Generic.List<double>() { 3606.00 }, null,
                new System.Collections.Generic.List<string> { "666333111" },
                new System.Collections.Generic.List<string> { "111000025" }, false, "Auto Test Fund", false);

            try {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver(userName, password, churchCode);

                // Match contribution to existing individual
                test.Portal.Giving_Batches_ViewUnmatched_MatchContribution(false, batchName, "$3,606.00", string.Format("{0} {1}", firstName, lastName), true, null, null, null, true);

                // Delete the batch
                test.Portal.Giving_Batches_ScannedContribution_DeleteBatch(true, batchName);

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            catch (Exception e) {
                throw new WebDriverException(e.StackTrace);
            }
            finally {
                // Clean Up
                base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", firstName, lastName), "Merge Dump");
                base.SQL.Giving_Batches_Delete(15, batchName, 4);
            }

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3505: Verifies a contribution can be matched to an existing organization through the unmatched contributions All page.")]
        public void Giving_Contributions_Batches_ViewUnmatchedContributions_All_Match_Organization() {
            // Setup data
            string batchName = "Scanned Contribution - Org";
            string orgName = "FT MatchOrganization";
            string orgContact = "FT MatchOrganization";
            string amount = "$3,707.00";
            base.SQL.Giving_Batches_Create_Scanned(15, batchName, true, new System.Collections.Generic.List<double>() { 3707.00 }, null,
                new System.Collections.Generic.List<string> { "666333111" },
                new System.Collections.Generic.List<string> { "111000025" }, false, "Auto Test Fund", false);

            try {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver(userName, password, churchCode);

                // Match contribution to existing individual
                test.Portal.Giving_Batches_ViewUnmatched_MatchContribution(false, batchName, amount, orgName, false, null, null, null, false, true, true, orgName, orgContact);

                // Confirm contribution was matched to organization
                test.Portal.Giving_ContributorDetails_View_WebDriver(orgName, true);

                var row = test.GeneralMethods.GetTableRowNumberWebDriver("//table[@id='repTable']", amount, "Amount", null) + 1;
                Assert.AreEqual(amount, test.Driver.FindElementByXPath(string.Format("//table[@id='repTable']/tbody/tr[{0}]/td[5]", row)).Text, string.Format("{0} contribution not found", amount));

                // Delete the batch
                test.Portal.Giving_Batches_ScannedContribution_DeleteBatch(true, batchName);

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            catch (Exception e) {
                throw new WebDriverException(e.StackTrace);
            }
            finally {
                base.SQL.Giving_Batches_Delete(15, batchName, 4);
            }

        }

        [Test, RepeatOnFailure, Timeout(100000)]
        [Author("David Martin")]
        [Description("F1-3505: Verifies a contribution can be matched to a newly created individual through the unmatched contributions All page.")]
        public void Giving_Contributions_Batches_ViewUnmatchedContributions_All_Match_Individual_New() {

            // Setup data
            string batchName = "Scanned Contribution - New Ind";
            string firstName = "FT";
            string lastName = "MatchNewIndividual";
            base.SQL.Giving_Batches_Delete(15, batchName, 4);
            base.SQL.Giving_Batches_Create_Scanned(15, batchName, true, new System.Collections.Generic.List<double>() { 3808.00 }, null,
                new System.Collections.Generic.List<string> { "666333111" },
                new System.Collections.Generic.List<string> { "111000025" }, false, "Auto Test Fund", false);

           
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver(userName, password, churchCode);

                string numOfDuplicates = test.Portal.People_Duplicate_Finder_Merge(string.Format("{0} {1}", firstName, lastName), "FT Tester");
                test.Portal.People_MergeIndividual("Merge Dump", string.Format("{0} {1}", firstName, lastName), "FT Tester", numOfDuplicates);

                // Match contribution to new individual
                test.Portal.Giving_Batches_ViewUnmatched_MatchContribution(false, batchName, "$3,808.00", string.Format("{0} {1}", firstName, lastName), false, firstName, lastName, "Active Member", false);

                // Delete the batch
                test.Portal.Giving_Batches_ScannedContribution_DeleteBatch(true, batchName);

                // Clean Up
                base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", firstName, lastName), "Merge Dump");
                base.SQL.Giving_Batches_Delete(15, batchName, 4);

                numOfDuplicates = test.Portal.People_Duplicate_Finder_Merge(string.Format("{0} {1}", firstName, lastName), "FT Tester");
                test.Portal.People_MergeIndividual("Merge Dump", string.Format("{0} {1}", firstName, lastName), "FT Tester", numOfDuplicates);

                // Logout of Portal
                test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure, Timeout(8000)]
        [Author("David Martin")]
        [Description("F1-3505: Verifies a contribution can be matched to a newly created household through the unmatched contributions All page.")]
        public void Giving_Contributions_Batches_ViewUnmatchedContributions_All_Match_Household_New() {
            // Setup Data
            string batchName = "Scanned Contribution - New HH";
            string firstName = "FT";
            string lastName = "MatchNewHousehold";
            base.SQL.Giving_Batches_Delete(15, batchName, 4);
            base.SQL.Giving_Batches_Create_Scanned(15, batchName, true, new System.Collections.Generic.List<double>() { 3909.00 }, null,
                new System.Collections.Generic.List<string> { "666333111" },
                new System.Collections.Generic.List<string> { "111000025" }, false, "Auto Test Fund", false);

            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            
                // Login to Portal
                //TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver(userName, password, churchCode);

                string numOfDuplicates = test.Portal.People_Duplicate_Finder_Merge(string.Format("{0} {1}", firstName, lastName), "FT Tester");
                test.Portal.People_MergeIndividual("Merge Dump", string.Format("{0} {1}", firstName, lastName), "FT Tester", numOfDuplicates);


                // Match contribution to the new Household
                test.Portal.Giving_Batches_ViewUnmatched_MatchContribution(false, batchName, "$3,909.00", string.Format("{0} {1}", firstName, lastName), false, firstName, lastName, "Active Member", true);

                // Delete the batch
                test.Portal.Giving_Batches_ScannedContribution_DeleteBatch(true, batchName);

                // Clean Up
                numOfDuplicates = test.Portal.People_Duplicate_Finder_Merge(string.Format("{0} {1}", firstName, lastName), "FT Tester");
                test.Portal.People_MergeIndividual("Merge Dump", string.Format("{0} {1}", firstName, lastName), "FT Tester", numOfDuplicates);

                // Logout of Portal
                test.Portal.LogoutWebDriver();
                
                base.SQL.Giving_Batches_Delete(15, batchName, 4);
            

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3505: Verifies a contribution can be matched to a newly created organization through the unmatched contributions All page.")]
        public void Giving_Contributions_Batches_ViewUnmatchedContributions_All_Match_Organization_New() {


            string random = Guid.NewGuid().ToString().Substring(0, 5); 
            // Setup Data
            string batchName = string.Format("Scanned Contribution - {0} Org", random);
            string orgName = string.Format("FT MatchOrganization - {0}", random);
            string orgContact = string.Format("FT MatchOrganization - {0}", random);
            string amount = "$4,101.00";
            base.SQL.Giving_Batches_Create_Scanned(15, batchName, true, new System.Collections.Generic.List<double>() { 4101.00 }, null,
                new System.Collections.Generic.List<string> { "666333111" },
                new System.Collections.Generic.List<string> { "111000025" }, false, "Auto Test Fund", false);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            try {

                test.Portal.LoginWebDriver(userName, password, churchCode);

                // Match contribution to existing individual
                test.Portal.Giving_Batches_ViewUnmatched_MatchContribution(false, batchName, amount, orgName, false, null, null, null, false, true, false, orgName, orgContact);

                // Confirm contribution was matched to organization
                test.Portal.Giving_ContributorDetails_View_WebDriver(orgName, true);
                ////*[@id="repTable"]/tbody/tr[2]/td[5]
                var row = test.GeneralMethods.GetTableRowNumberWebDriver("//table[@id='repTable']", amount, "Amount", null) + 1;
                Assert.AreEqual(amount, test.Driver.FindElementByXPath(string.Format("//table[@id='repTable']/tbody/tr[{0}]/td[5]", row)).Text, string.Format("{0} contribution not found", amount));

                // Delete the batch
                test.Portal.Giving_Batches_ScannedContribution_DeleteBatch(true, batchName);

            }
            catch (Exception e) {
                throw new WebDriverException(e.StackTrace + "\n" + e.Message);
            }
            finally {

                // Delete batch
                base.SQL.Giving_Batches_Delete(15, batchName, 4);

                // Inactivate all orgs
                test.Portal.Giving_Organizations_Inactive(orgName, true);
                test.Portal.LogoutWebDriver();
            }

        }

        #endregion Match All

        #region Search Batch

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3504: Verifies changing the Start/End Dates on the Unmatched by Batches page will automatically updates the other date field to 90 days out.")]
        public void Giving_Contributions_Batches_ViewUnmatchedContributions_Search_Batches_MoreThan90Days() {
            // Set Variables
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            var start = today.AddYears(-2);
            var end = today;

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver(userName, password, churchCode);

            // Navigate to the View Unmatched Contributions page
            test.Portal.Giving_Batches_ViewUnmatched_Batches(15, true);

            // Change the Start Date for date range search
            test.Portal.Giving_Batches_ViewUnmatched_Batches_Search(start, end);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3504: Verifies the proper message is displayed when there are no unmatched batches within the date range search.")]
        public void Giving_Contributions_Batches_ViewUnmatchedContributions_Search_Batches_NoResultsFound() {
            // Set Variables
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            var start = today.AddDays(2);
            var end = today.AddYears(2);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver(userName, password, churchCode);

            // Navigate to the View Unmatched Contributions page
            test.Portal.Giving_Batches_ViewUnmatched_Batches(15, true);

            // Search for unmatched contributions resulting in zero results
            test.Portal.Giving_Batches_ViewUnmatched_Batches_Search(start, end);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        #endregion Search Batch

        #region Match Batch

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3505: Verifies a contribution can be matched to an existing individual through the unmatched contributions All page.")]
        public void Giving_Contributions_Batches_ViewUnmatchedContributions_Batches_Match_Individual() {
            // Setup data
            string batchName = "Scanned Batch - Ind";
            string firstName = "FT";
            string lastName = "BatchIndividual";
            base.SQL.People_Individual_Create(15, firstName, lastName, 1);
            base.SQL.Giving_Batches_Create_Scanned(15, batchName, true, new System.Collections.Generic.List<double>() { 4505.00 }, null,
                new System.Collections.Generic.List<string> { "666333111" },
                new System.Collections.Generic.List<string> { "111000025" }, false, "Auto Test Fund", false);

            try {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver(userName, password, churchCode);

                // Match contribution to existing individual
                test.Portal.Giving_Batches_ViewUnmatched_MatchContribution(true, batchName, "$4,505.00", string.Format("{0} {1}", firstName, lastName), true, null, null, null, false);

                // Delete the batch
                test.Portal.Giving_Batches_ScannedContribution_DeleteBatch(true, batchName);

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            catch (Exception e) {
                throw new WebDriverException(e.StackTrace);
            }
            finally {
                // Clean Up
                base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", firstName, lastName), "Merge Dump");
                base.SQL.Giving_Batches_Delete(15, batchName, 4);
            }

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3505: Verifies a contribution can be matched to an existing household through the unmatched contributions Batch page.")]
        public void Giving_Contributions_Batches_ViewUnmatchedContributions_Batches_Match_Household() {
            // Setup data
            string batchName = "Scanned Batch - HH";
            string firstName = "FT";
            string lastName = "BatchHousehold";
            base.SQL.People_Individual_Create(15, firstName, lastName, 1);
            base.SQL.Giving_Batches_Create_Scanned(15, batchName, true, new System.Collections.Generic.List<double>() { 4606.00 }, null,
                new System.Collections.Generic.List<string> { "666333111" },
                new System.Collections.Generic.List<string> { "111000025" }, false, "Auto Test Fund", false);

            try {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver(userName, password, churchCode);

                // Match contribution to existing individual
                test.Portal.Giving_Batches_ViewUnmatched_MatchContribution(true, batchName, "$4,606.00", string.Format("{0} {1}", firstName, lastName), true, null, null, null, true);

                // Delete the batch
                test.Portal.Giving_Batches_ScannedContribution_DeleteBatch(true, batchName);

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            catch (Exception e) {
                throw new WebDriverException(e.StackTrace);
            }
            finally {
                // Clean Up
                base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", firstName, lastName), "Merge Dump");
                base.SQL.Giving_Batches_Delete(15, batchName, 4);
            }

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3505: Verifies a contribution can be matched to an existing organization through the unmatched contributions All page.")]
        public void Giving_Contributions_Batches_ViewUnmatchedContributions_Batches_Match_Organization() {
            
            string random = Guid.NewGuid().ToString().Substring(0, 5);
            // Setup Data
            string batchName = string.Format("Scanned Batch - {0} Org", random);
            string orgName = "FT BatchOrganization";
            string orgContact = "FT BatchOrganization";
            string amount = "$4,707.00";
            base.SQL.Giving_Batches_Create_Scanned(15, batchName, true, new System.Collections.Generic.List<double>() { 4707.00 }, null,
                new System.Collections.Generic.List<string> { "666333111" },
                new System.Collections.Generic.List<string> { "111000025" }, false, "Auto Test Fund", false);

            try {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver(userName, password, churchCode);

                // Match contribution to existing individual
                test.Portal.Giving_Batches_ViewUnmatched_MatchContribution(true, batchName, amount, orgName, false, null, null, null, false, true, true, orgName, orgContact);

                // Confirm contribution was matched to organization
                test.Portal.Giving_ContributorDetails_View_WebDriver(orgName, true);
                
                // decimal row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Contributions, amount, "Amount", null);
                // Assert.AreEqual(amount, test.Driver.FindElementById("ctl00_ctl00_MainContent_content_rptrContribution_ctl01_lblAmount1").Text, string.Format("{0} contribution not found", amount));
                var row = test.GeneralMethods.GetTableRowNumberWebDriver("//table[@id='repTable']", amount, "Amount", null) + 1;
                Assert.AreEqual(amount, test.Driver.FindElementByXPath(string.Format("//table[@id='repTable']/tbody/tr[{0}]/td[5]", row)).Text, string.Format("{0} contribution not found", amount));
                // var row = test.GeneralMethods.GetTableRowNumberWebDriver("//table[@id='repTable']", amount, "Amount", null) + 1;
                // Assert.AreEqual(amount, test.Driver.FindElementByXPath(string.Format("//table[@id='repTable']/tbody/tr[{0}]/td[5]", row)).Text, string.Format("{0} contribution not found", amount));

                // Delete the batch
                test.Portal.Giving_Batches_ScannedContribution_DeleteBatch(true, batchName);

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            catch (Exception e) {
                throw new WebDriverException(e.StackTrace);
            }
            finally {
                base.SQL.Giving_Batches_Delete(15, batchName, 4);
            }

        }

        [Test, RepeatOnFailure, Timeout(800000)]
        [Author("David Martin")]
        [Description("F1-3505: Verifies a contribution can be matched to a newly created individual through the unmatched contributions Batches page.")]
        public void Giving_Contributions_Batches_ViewUnmatchedContributions_Batches_Match_Individual_New() {
            // Setup data
            string batchName = "Scanned Batch - New Ind";
            string firstName = "FT";
            string lastName = "BatchNewIndividual";

            base.SQL.Giving_Batches_Delete(15, batchName, 4);
            base.SQL.Giving_Batches_Create_Scanned(15, batchName, true, new System.Collections.Generic.List<double>() { 4808.00 }, null,
                new System.Collections.Generic.List<string> { "666333111" },
                new System.Collections.Generic.List<string> { "111000025" }, false, "Auto Test Fund", false);

           
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver(userName, password, churchCode);

                string numOfDuplicates = test.Portal.People_Duplicate_Finder_Merge(string.Format("{0} {1}", firstName, lastName), "FT Tester");
                test.Portal.People_MergeIndividual("Merge Dump", string.Format("{0} {1}", firstName, lastName), "FT Tester", numOfDuplicates);

                // Match contribution to new individual
                test.Portal.Giving_Batches_ViewUnmatched_MatchContribution(true, batchName, "$4,808.00", string.Format("{0} {1}", firstName, lastName), false, firstName, lastName, "Active Member", false);

                // Delete the batch
                test.Portal.Giving_Batches_ScannedContribution_DeleteBatch(true, batchName);


                // Clean Up
                base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", firstName, lastName), "Merge Dump");
                base.SQL.Giving_Batches_Delete(15, batchName, 4);

                numOfDuplicates = test.Portal.People_Duplicate_Finder_Merge(string.Format("{0} {1}", firstName, lastName), "FT Tester");
                test.Portal.People_MergeIndividual("Merge Dump", string.Format("{0} {1}", firstName, lastName), "FT Tester", numOfDuplicates);

                // Logout of Portal
                test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure, Timeout(800000)]
        [Author("David Martin")]
        [Description("F1-3505: Verifies a contribution can be matched to a newly created household through the unmatched contributions Batches page.")]
        public void Giving_Contributions_Batches_ViewUnmatchedContributions_Batches_Match_Household_New() {
            // Setup Data
            string batchName = "Scanned Batch - New HH";
            string firstName = "FT";
            string lastName = "BatchNewHousehold";
            base.SQL.Giving_Batches_Delete(15, batchName, 4);
            base.SQL.Giving_Batches_Create_Scanned(15, batchName, true, new System.Collections.Generic.List<double>() { 4909.00 }, null,
                new System.Collections.Generic.List<string> { "666333111" },
                new System.Collections.Generic.List<string> { "111000025" }, false, "Auto Test Fund", false);

            
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver(userName, password, churchCode);

                string numOfDuplicates = test.Portal.People_Duplicate_Finder_Merge(string.Format("{0} {1}", firstName, lastName), "FT Tester");
                test.Portal.People_MergeIndividual("Merge Dump", string.Format("{0} {1}", firstName, lastName), "FT Tester", numOfDuplicates);

                // Match contribution to the new Household
                test.Portal.Giving_Batches_ViewUnmatched_MatchContribution(true, batchName, "$4,909.00", string.Format("{0} {1}", firstName, lastName), false, firstName, lastName, "Active Member", true);

                // Delete the batch
                test.Portal.Giving_Batches_ScannedContribution_DeleteBatch(true, batchName);

            
                // Clean Up
                base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", firstName, lastName), "Merge Dump");
                base.SQL.Giving_Batches_Delete(15, batchName, 4);

                numOfDuplicates = test.Portal.People_Duplicate_Finder_Merge(string.Format("{0} {1}", firstName, lastName), "FT Tester");
                test.Portal.People_MergeIndividual("Merge Dump", string.Format("{0} {1}", firstName, lastName), "FT Tester", numOfDuplicates);

                // Logout of Portal
                test.Portal.LogoutWebDriver();


        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3505: Verifies a contribution can be matched to a newly created organization through the unmatched contributions Batches page.")]
        public void Giving_Contributions_Batches_ViewUnmatchedContributions_Batches_Match_Organization_New() {


            string random = Guid.NewGuid().ToString().Substring(0, 5);
            // Setup Data
            string batchName = string.Format("Scanned Batch - {0} Org", random);
            string orgName = string.Format("FT BatchOrganization - {0}", random);
            string orgContact = string.Format("FT BatchOrganization - {0}", random);
            string amount = "$5,101.00";
            base.SQL.Giving_Batches_Create_Scanned(15, batchName, true, new System.Collections.Generic.List<double>() { 5101.00 }, null,
                new System.Collections.Generic.List<string> { "666333111" },
                new System.Collections.Generic.List<string> { "111000025" }, false, "Auto Test Fund", false);

            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try {
                // Login to Portal
                test.Portal.LoginWebDriver(userName, password, churchCode);

                // Match contribution to existing individual
                test.Portal.Giving_Batches_ViewUnmatched_MatchContribution(true, batchName, amount, orgName, false, null, null, null, false, true, false, orgName, orgContact);

                // Confirm contribution was matched to organization
                test.Portal.Giving_ContributorDetails_View_WebDriver(orgName, true);
                ////*[@id="repTable"]/tbody/tr[2]/td[5]
                var row = test.GeneralMethods.GetTableRowNumberWebDriver("//table[@id='repTable']", amount, "Amount", null) + 1;
                Assert.AreEqual(amount, test.Driver.FindElementByXPath(string.Format("//table[@id='repTable']/tbody/tr[{0}]/td[5]", row)).Text, string.Format("{0} contribution not found", amount));

                // Delete the batch
                test.Portal.Giving_Batches_ScannedContribution_DeleteBatch(true, batchName);

            }
            catch (Exception e) {
                throw new WebDriverException(e.StackTrace + "\n" + e.Message);
            }
            finally {

                // Delete batch
                base.SQL.Giving_Batches_Delete(15, batchName, 4);

                // Inactivate all orgs
                test.Portal.Giving_Organizations_Inactive(orgName, true);
                test.Portal.LogoutWebDriver();
            }

        }

        #endregion Match Batch

    }

    [TestFixture]
    [Category(TestCategories.Services.PaymentProcessor)]
    public class Portal_Giving_Contributions_EnterContributions : FixtureBaseWebDriver
    {
        [FixtureSetUp]
        public void FixtureSetUp()
        {
            //base.SQL.Giving_DeleteNonTransactionalContributionReceipts(256, null, base.SQL.People_Households_FetchID(256, base.SQL.People_Individuals_FetchID(256, _individualNameFTAutoTester)));
            base.SQL.Giving_DeleteNonTransactionalContributionReceipts(256, null, base.SQL.People_Households_FetchID(256, base.SQL.People_Individuals_FetchID(256, "FT Tester")));
        }




        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Creates an ach contribution attributed to an individual.")]
        public void Giving_Contributions_EnterContributions_IND_ACH()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Enter a contribution
            string fund = "Auto Test Fund";
            string individual = "FT Tester";
            int amount = 100;
            string reference = new Random().Next(1000).ToString();
            test.Portal.Giving_EnterContributions(null, individual, individual, "ACH", fund, new string[] { amount.ToString(), reference }, false, false, null);

            // Verify the contribution was created
            test.Portal.Giving_Search_VerifyContributionsPresent(individual, individual, fund, reference, amount, false);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        
        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Creates a cash contribution attributed to an individual.")]
        public void Giving_Contributions_EnterContributions_IND_Cash()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Enter a contribution
            string fund = "Auto Test Fund";
            string individual = "FT Tester";
            int amount = new Random().Next(1000);
            string reference = "–";
            test.Portal.Giving_EnterContributions(null, individual, individual, "Cash", fund, new string[] { amount.ToString(), reference }, false, false, null);

            // Verify the contribution was created
            test.Portal.Giving_Search_VerifyContributionsPresent(individual, individual, fund, reference, amount, false);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Enters a contribution at the individual leaving do not thank unchecked.")]
        public void Giving_Contributions_EnterContributions_IND_Check()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Enter a contribution
            string fund = "Auto Test Fund";
            string individual = "FT Tester";
            int amount = 102;
            string reference = new Random().Next(1000).ToString();
            test.Portal.Giving_EnterContributions(null, individual, individual, "Check", fund, new string[] { amount.ToString(), reference }, false, false, null);

            // Verify the contribution was created
            test.Portal.Giving_Search_VerifyContributionsPresent("Household", individual, fund, reference, amount, false);
            //test.Portal.Giving_Search_VerifyContributionsPresent("Household", individual, fund, reference, amount, true);
            test.Portal.Giving_Search_VerifyContributionsPresent(individual, individual, fund, reference, amount, false);
            //test.Portal.Giving_Search_VerifyContributionsPresent(individual, individual, fund, reference, amount, true);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Creates a credit card contribution attributed to an individual.")]
        public void Giving_Contributions_EnterContributions_IND_CreditCard()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Enter a contribution
            string fund = "Auto Test Fund";
            string individual = "FT Tester";
            int amount = 103;
            string reference = new Random().Next(1000).ToString();
            test.Portal.Giving_EnterContributions(null, individual, individual, "Credit Card", fund, new string[] { amount.ToString(), reference }, false, false, null);

            // Verify the contribution was created
            test.Portal.Giving_Search_VerifyContributionsPresent(individual, individual, fund, reference, amount, false);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Creates a voucher contribution attributed to an individual.")]
        public void Giving_Contributions_EnterContributions_IND_Voucher()
        {
            // Set initial conditions
            string voucher = "1234567890";
            int individualId = base.SQL.People_Individuals_FetchID(256, "FT Tester");
            base.SQL.Giving_Accounts_Delete(256, 16, individualId, voucher);
            base.SQL.Giving_Accounts_Create(256, 16, base.SQL.People_Households_FetchID(256, individualId), individualId, voucher);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Enter a contribution
            string fund = "Auto Test Fund";
            string individual = "FT Tester";
            int amount = 105;
            string reference = new Random().Next(1000).ToString();
            test.Portal.Giving_EnterContributions(null, individual, individual, "Voucher", fund, new string[] { amount.ToString(), voucher, reference }, false, false, null);

            // Verify the contribution was created
            test.Portal.Giving_Search_VerifyContributionsPresent(individual, individual, fund, reference, amount, false);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Creates an ach contribution attributed to a household.")]
        public void Giving_Contributions_EnterContributions_HH_ACH()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Enter a contribution
            string fund = "Auto Test Fund";
            string individual = "FT Tester";
            int amount = 100;
            string reference = new Random().Next(1000).ToString();
            test.Portal.Giving_EnterContributions(null, individual, "Household", "ACH", fund, new string[] { amount.ToString(), reference }, false, false, null);

            // Verify the contribution was created
            test.Portal.Giving_Search_VerifyContributionsPresent("Household", individual, fund, reference, amount, false);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Creates a cash contribution attributed to a household.")]
        public void Giving_Contributions_EnterContributions_HH_Cash()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Enter a contribution
            string fund = "Auto Test Fund";
            string individual = "FT Tester";
            int amount = new Random().Next(1000);
            string reference = "–";
            test.Portal.Giving_EnterContributions(null, individual, "Household", "Cash", fund, new string[] { amount.ToString(), reference }, false, false, null);

            // Verify the contribution was created
            test.Portal.Giving_Search_VerifyContributionsPresent("Household", individual, fund, reference, amount, false);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Enters a contribution at the household level leaving do not thank unchecked.")]
        public void Giving_Contributions_EnterContributions_HH_Check()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Enter a contribution
            string fund = "Auto Test Fund";
            string individual = "FT Tester";
            int amount = 102;
            string reference = new Random().Next(1000).ToString();
            test.Portal.Giving_EnterContributions(null, individual, "Household", "Check", fund, new string[] { amount.ToString(), reference }, false, false, null);

            // Verify the contribution was created
            test.Portal.Giving_Search_VerifyContributionsPresent("Household", individual, fund, reference, amount, false);
            test.Portal.Giving_Search_VerifyContributionsPresent("Household", individual, fund, reference, amount, true);
            test.Portal.Giving_Search_VerifyContributionsNotPresent(individual, individual, fund, reference, amount, false);
            test.Portal.Giving_Search_VerifyContributionsNotPresent(individual, individual, fund, reference, amount, true);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Creates a credit card contribution attributed to a household.")]
        public void Giving_Contributions_EnterContributions_HH_CreditCard()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Enter a contribution
            string fund = "Auto Test Fund";
            string individual = "FT Tester";
            int amount = 103;
            string reference = new Random().Next(1000).ToString();
            test.Portal.Giving_EnterContributions(null, individual, "Household", "Credit Card", fund, new string[] { amount.ToString(), reference }, false, false, null);

            // Verify the contribution was created
            test.Portal.Giving_Search_VerifyContributionsPresent("Household", individual, fund, reference, amount, false);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Creates a voucher contribution attributed to a household.")]
        public void Giving_Contributions_EnterContributions_HH_Voucher()
        {
            // Set initial conditions
            string individual = "FT Tester";
            string voucher = "0987654321";
            int individualId = base.SQL.People_Individuals_FetchID(256, individual);
            base.SQL.Giving_Accounts_Delete(256, 16, individualId, voucher);
            base.SQL.Giving_Accounts_Create(256, 16, base.SQL.People_Households_FetchID(256, individualId), individualId, voucher);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Enter a contribution
            string fund = "Auto Test Fund";
            int amount = 105;
            string reference = new Random().Next(1000).ToString();
            test.Portal.Giving_EnterContributions(null, individual, "Household", "Voucher", fund, new string[] { amount.ToString(), voucher, reference }, false, false, null);

            // Verify the contribution was created
            test.Portal.Giving_Search_VerifyContributionsPresent("Household", individual, fund, reference, amount, false);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Enters a contribution at the household level checking do not thank.")]
        public void Giving_Contributions_EnterContributions_HHDoNotThank_Check()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Enter a contribution
            string fund = "Auto Test Fund";
            string individual = "FT Tester";
            int amount = 101;
            string reference = new Random().Next(1000).ToString();
            test.Portal.Giving_EnterContributions(null, individual, "Household", "Check", fund, new string[] { amount.ToString(), reference }, true, false, null);

            // Verify the contribution was created
            test.Portal.Giving_Search_VerifyContributionsPresent("Household", individual, fund, reference, amount, false);
            test.Portal.Giving_Search_VerifyContributionsNotPresent("Household", individual, fund, reference, amount, true);
            test.Portal.Giving_Search_VerifyContributionsNotPresent(individual, individual, fund, reference, amount, false);
            test.Portal.Giving_Search_VerifyContributionsNotPresent(individual, individual, fund, reference, amount, true);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Enters a contribution at the individual checking do not thank.")]
        public void Giving_Contributions_EnterContributions_INDDoNotThank_Check()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Enter a contribution
            string fund = "Auto Test Fund";
            string individual = "FT Tester";
            int amount = 103;
            string reference = new Random().Next(1000).ToString();
            test.Portal.Giving_EnterContributions(null, individual, individual, "Check", fund, new string[] { amount.ToString(), reference }, true, false, null);

            // Verify the contribution was created
            test.Portal.Giving_Search_VerifyContributionsPresent("Household", individual, fund, reference, amount, false);
            test.Portal.Giving_Search_VerifyContributionsNotPresent("Household", individual, fund, reference, amount, true);
            test.Portal.Giving_Search_VerifyContributionsPresent(individual, individual, fund, reference, amount, false);
            test.Portal.Giving_Search_VerifyContributionsNotPresent(individual, individual, fund, reference, amount, true);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Creates a credit card batch and verifies it is not present when searching for batches on the enter contributions page.")]
        public void Giving_Contributions_EnterContributions_CreditCardBatchesNotPresent()
        {
            // Set initial conditions
            string batchName = "Enter Contributions";
            base.SQL.Giving_Batches_Delete(15, batchName, 2);
            base.SQL.Giving_Batches_Create(15, batchName, 100, 2);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Navigate to giving->enter contributions
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Enter_Contributions);

            // View the batches popup
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_lnkSelectBatch").Click();
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]);

            // Search for a credit card batch, verify not found
            test.Driver.FindElementById("ctl00_content_txtBatchNameSearch_textBox").SendKeys(batchName);
            test.Driver.FindElementById("ctl00_content_btnSearch").Click();
            //test.Selenium.KeyPressAndWaitForPageToLoad("ctl00_content_btnSearch", "\\13", "30000");
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("No records found"));
            Assert.IsFalse(test.Driver.FindElementsById("ctl00_content_dgBatches").Count > 0);
            //test.Selenium.KeyPress(GeneralLinks.Cancel, "\\13");
            test.Driver.FindElementByLinkText("Cancel").Click();
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[0]);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("FO-62: Remove Pledge or Sub Fund once a associated to a split by editing.")]
        public void Giving_Contributions_RemovePledgeandSubFundOfSplit()
        {
            // Set initial conditions
            string batchName = "Gracezhang Batch- Split Contribution";
            try {
                base.SQL.Giving_ContributionReceipts_DeleteByBatchName(15, batchName, 1);
            }
            catch(Exception e){
                e.ToString();

            }
           
            base.SQL.Giving_Batches_Create(15, batchName, 100, 1);

            try
            {


                // Login to portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                // Add a split contribution to a batch
                test.Portal.Giving_EnterContributions(batchName, "Matthew Sneeden", null, "Cash", null, null, false, true, new string[][] { new string[5] { "Matthew Sneeden", "5.00", "1 - General Fund", null, "Goins Test 3" }, new string[5] { "Matthew Sneeden", "6.00", "2 - Building Fund", "Jim fund", null } });


                test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Search);
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtBatchName_textBox").SendKeys(batchName);
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSearch").Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Matthew Sneeden"));

                test.GeneralMethods.SelectOptionFromGearWebDriver(2, "Edit contribution");

                //edit the first split:delete pledge drive
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgSplitContributions").FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[0].FindElement(By.TagName("span")).FindElement(By.TagName("a")).Click();
                new SelectElement(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlSplitPledgeDrive_dropDownList")).SelectByIndex(0);
                // Save the first split
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSaveSplit").Click();

                //edit the second split:delete sub fund
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgSplitContributions").FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[0].FindElement(By.TagName("span")).FindElement(By.TagName("a")).Click();
                new SelectElement(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlSplitSubFund_dropDownList")).SelectByIndex(0);
                // Save the second split
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSaveSplit").Click();
                //Click Update controbution
                test.Driver.FindElementById(GeneralButtons.Save).Click();

                //come in again,check 
                test.GeneralMethods.SelectOptionFromGearWebDriver(3, "Edit contribution");

                // Verify  Pledge drive of the first split is null,sub fund of the second split is null
                Assert.AreEqual(" ", test.Driver.FindElementByXPath("//table[@id='ctl00_ctl00_MainContent_content_dgSplitContributions']/tbody/tr[2]/td[4]").Text.ToString());
                Assert.AreEqual(" ", test.Driver.FindElementByXPath("//table[@id='ctl00_ctl00_MainContent_content_dgSplitContributions']/tbody/tr[3]/td[5]").Text.ToString());
                // Logout of portal
                test.Portal.LogoutWebDriver();
            }
            finally
            {
                base.SQL.Giving_ContributionReceipts_DeleteByBatchName(15, batchName, 1);
            }
            
            
        }
        [Test, RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("FO-6890: P3: Changes to contribution are not saving when changing split contribution from individual to household")]
        public void Giving_Contributions_SplitAttributedToHousehold()
        {
            // Set initial conditions
            string batchName = "Gracezhang Batch- Split Contribution To Household";
            try
            {
                base.SQL.Giving_ContributionReceipts_DeleteByBatchName(15, batchName, 1);
            }
            catch (Exception e)
            {
                e.ToString();

            }

            base.SQL.Giving_Batches_Create(15, batchName, 100, 1);

            try
            {


                // Login to portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                // Add a split contribution to a batch
                test.Portal.Giving_EnterContributions(batchName, "Matthew Sneeden", null, "Cash", null, null, false, true, new string[][] { new string[5] { "Matthew Sneeden", "5.00", "1 - General Fund", null, "Goins Test 3" }, new string[5] { "Matthew Sneeden", "6.00", "2 - Building Fund", "Jim fund", null } });


                test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Search);
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtBatchName_textBox").SendKeys(batchName);
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSearch").Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Matthew Sneeden"));

                test.GeneralMethods.SelectOptionFromGearWebDriver(2, "Edit contribution");

                //edit the first split:AttributedTo Household
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgSplitContributions").FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[0].FindElement(By.TagName("span")).FindElement(By.TagName("a")).Click();
                new SelectElement(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlSplitAttributedTo")).SelectByIndex(0);
                // Save the first split
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSaveSplit").Click();
                //edit the second split:AttributedTo Individual
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgSplitContributions").FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[0].FindElement(By.TagName("span")).FindElement(By.TagName("a")).Click();
                new SelectElement(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlSplitAttributedTo")).SelectByIndex(0);
                // Save the second split
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSaveSplit").Click();

                //Click Update controbution
                test.Driver.FindElementById(GeneralButtons.Save).Click();

                test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Search);
                new SelectElement(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlHousehold_dropDownList")).SelectByIndex(0);          
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtBatchName_textBox").SendKeys(batchName);
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSearch").Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Matthew Sneeden"));

                String acutalAttributedTo =test.Driver.FindElementByXPath("//table[@class='grid']/tbody/tr[3]/td[3]").Text.ToString().Trim();
                TestLog.WriteLine("-acutalAttributedTo = {0}", acutalAttributedTo);
                Assert.AreEqual("Matthew Sneeden and Laura Hause", acutalAttributedTo);
                // Logout of portal
                test.Portal.LogoutWebDriver();
            }
            finally
            {
                base.SQL.Giving_ContributionReceipts_DeleteByBatchName(15, batchName, 1);
            }


        }
    }

}