﻿using System;
using System.Linq;
using System.Text;
using System.Web;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace FTTests.Portal.Giving {
	[TestFixture]
	public class Portal_Giving : FixtureBaseWebDriver {
		[Test, RepeatOnFailure, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("David Martin")]
		[Description("Verifies the links present under the giving menu.")]
		public void Giving() {
			// Login to portal
			TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

			// Verify the links present
            WebDriverWait wait = new WebDriverWait(test.Driver, TimeSpan.FromMilliseconds(3000));
            test.Driver.FindElementByLinkText("Giving").Click(); ////*[@id="nav_sub_5"]
            IWebElement menuHeader = wait.Until<IWebElement>((e) => { return e.FindElement(By.XPath("//div[@style='visibility: visible;' and @id='nav_sub_5']/dl[1]/dt")); });
            
            Assert.AreEqual("Contributions", menuHeader.Text);
            Assert.AreEqual("Search", test.Driver.FindElementByXPath("//div[@id='nav_sub_5']/dl[1]/dd[1]/a").Text);
            Assert.AreEqual("Contributor Details", test.Driver.FindElementByXPath("//div[@id='nav_sub_5']/dl[1]/dd[2]/a").Text);
            Assert.AreEqual("Batches", test.Driver.FindElementByXPath("//div[@id='nav_sub_5']/dl[1]/dd[3]/a").Text);
            Assert.AreEqual("Scanned Contributions", test.Driver.FindElementByXPath("//div[@id='nav_sub_5']/dl[1]/dd[4]/a").Text);
            Assert.AreEqual("Enter Contributions", test.Driver.FindElementByXPath("//div[@id='nav_sub_5']/dl[1]/dd[5]/a").Text);
            Assert.AreEqual("Unmatched", test.Driver.FindElementByXPath("//div[@id='nav_sub_5']/dl[1]/dd[6]/a").Text);

            Assert.AreEqual("Statements", test.Driver.FindElementByXPath("//div[@id='nav_sub_5']/dl[2]/dt").Text);
            Assert.AreEqual("Statement Builder", test.Driver.FindElementByXPath("//div[@id='nav_sub_5']/dl[2]/dd[1]/a").Text);
            Assert.AreEqual("Queue", test.Driver.FindElementByXPath("//div[@id='nav_sub_5']/dl[2]/dd[2]/a").Text);
            Assert.AreEqual("Custom Styles", test.Driver.FindElementByXPath("//div[@id='nav_sub_5']/dl[2]/dd[3]/a").Text);

            Assert.AreEqual("Setup", test.Driver.FindElementByXPath("//div[@id='nav_sub_5']/dl[3]/dt").Text);
            Assert.AreEqual("Funds", test.Driver.FindElementByXPath("//div[@id='nav_sub_5']/dl[3]/dd[1]/a").Text);
            Assert.AreEqual("Sub Funds", test.Driver.FindElementByXPath("//div[@id='nav_sub_5']/dl[3]/dd[2]/a").Text);
            Assert.AreEqual("Pledge Drives", test.Driver.FindElementByXPath("//div[@id='nav_sub_5']/dl[3]/dd[3]/a").Text);
            Assert.AreEqual("Contribution Attributes", test.Driver.FindElementByXPath("//div[@id='nav_sub_5']/dl[3]/dd[4]/a").Text);
            Assert.AreEqual("Sub Types", test.Driver.FindElementByXPath("//div[@id='nav_sub_5']/dl[3]/dd[5]/a").Text);
            Assert.AreEqual("Account References", test.Driver.FindElementByXPath("//div[@id='nav_sub_5']/dl[3]/dd[6]/a").Text);
            Assert.AreEqual("Organizations", test.Driver.FindElementByXPath("//div[@id='nav_sub_5']/dl[3]/dd[7]/a").Text);

			// Logout of portal
			test.Portal.LogoutWebDriver();
		}

        #region Contributions
        #region Search
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Navigates to the search page under giving.")]
        public void Giving_Contributions_Search() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Navigate to giving->search
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Search);

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Contribution Search", test.Driver.Title);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Contribution Search"));

            // Verify the date label is marked as required
            Assert.AreEqual("Date *", test.Driver.FindElementById("ctl00_ctl00_MainContent_content_lblSearchType").Text);

            // Verify the default date range type
            Assert.AreEqual("Received Date", new SelectElement(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlSearchType_dropDownList")).SelectedOption.Text);

            // Select a type
            new SelectElement(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlContributionType_dropDownList")).SelectByText("Cash");

            // Select a fund
            new SelectElement(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlFund_dropDownList")).SelectByIndex(1);


            // View the contribution attribute popup
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_lnkFindAttribute").Click();
            test.Driver.SwitchTo().Window("psuedoModal");

            // Verify the labels
            Assert.AreEqual("Attribute contains", test.Driver.FindElementById("ctl00_content_lblNameContains").Text);
            Assert.AreEqual("Created between *", test.Driver.FindElementById("ctl00_content_lblAttributeDateSearch").Text);

            // Close the popup
            test.Driver.FindElementByLinkText("Close window").Click();
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[0]);


            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Search

        #region Contributor Details
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the contributor details page.")]
        public void Giving_Contributions_ContributorDetails() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            // Navigate to giving->contributor details
            test.GeneralMethods.Navigate_Portal(Navigation.Giving.Contributions.Contributor_Details);
            
            // Verify title, text
            Assert.AreEqual("Fellowship One :: Contributor Details", test.Driver.Title);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Contributor Details"));

            // Verify the search controls are present
            test.Driver.FindElementByXPath("//form[@action='/bridge/Giving/ContributorDetails/SearchIndividuals/0']");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Navigates to the contributor details page under giving for an individual.")]
        public void Giving_Contributions_ContributorDetails_Individual() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            // Navigate and search for an individual
            test.GeneralMethods.Navigate_Portal(Navigation.Giving.Contributions.Contributor_Details);
            test.GeneralMethods.SelectPersonWebDriver("FT Tester");

            // Verify the individual was selected
            Assert.AreEqual("FT Tester", test.Driver.FindElementById("ctl00_ctl00_MainContent_content_lblName").Text);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Navigates to the contributor details page under giving for an organization.")]
        public void Giving_Contributions_ContributorDetails_Organization() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            // Navigate and search for an organization
            test.GeneralMethods.Navigate_Portal(Navigation.Giving.Contributions.Contributor_Details);
            test.GeneralMethods.SelectPersonWebDriver("Auto Test Organization", true);

            // Verify the organization was selected
            Assert.AreEqual("Auto Test Organization", test.Driver.FindElementById("ctl00_ctl00_MainContent_content_lblName").Text);

            // Verify the schedules tab is not present
            Assert.IsFalse(test.Driver.FindElementsById("ctl00_ctl00_MainContent_content_lbtnAutomatic").Count > 0);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Winnie Wang")]
        [Description("Search Organization with multiple addresses and check view on the search widget of Contributor Details.")]
        public void Giving_Contributions_ContributorDetails_Organization_MultipleAddresses()
        {
            // Setup data, if it is INT, INT2, INT3 or QA evn, check on DC church. If it is staging, product, check on LPCBWIMD church
            string userName, password, churchCode;
            int churchID;
            
            if (base.F1Environment == F1Environments.INT || base.F1Environment == F1Environments.INT2 || base.F1Environment == F1Environments.INT3 || base.F1Environment == F1Environments.LV_QA)           
            {               
                userName = "ft.tester";
                password = "FT4life!";
                churchCode= "dc";
                churchID = SQL.FetchChurchID(churchCode);
            }
            else if (base.F1Environment == F1Environments.STAGING || base.F1Environment == F1Environments.PRODUCTION)
            {
                userName = "mrodriguez";
                password = "Missy3-16";
                churchCode= "LPCBWIMD";
                churchID = SQL.FetchChurchID(churchCode);
            }
            else
            {
                TestLog .WriteLine ("The portal environment doesn't have proper church for test!"); 
                return;
            }

            // Search in DB to find an Organization with multiple addresses
            string OrgName = SQL.Giving_GetMultipleAddressesOrganizationForSearchInContributorDetails(churchID);

            if (OrgName != null)
            {
                TestLog.WriteLine("Find an Organization with multiple addresses on {0}: {1}", churchCode, OrgName);

                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver(userName, password, churchCode);

                // Navigate to giving->contributor details and search for an organization
                test.GeneralMethods.Navigate_Portal(Navigation.Giving.Contributions.Contributor_Details);
                test.GeneralMethods.SelectPersonWebDriver_Search(OrgName, true);
  
                // Verify if there is only one result
                Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.XPath(".//*[@id='search_results_inner']/table/tbody/tr[3]")), "Wrong: Multiple addresses are still displayed!");            
            
            }
            else
            {
                TestLog .WriteLine ("There's no Organization having multiple addresses.");
            }            
        }

        #endregion Contributor Details

        #region Batches

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies the URLs for the All and General batches pages.")]
        public void Giving_Contributions_Batches_Verify_URLs_General() {
            // Setup data
            string batchName = "URL Test Batch";
            base.SQL.Giving_Batches_Create(15, batchName, 10, 1);

            try {

                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // Navigate to Giving -> Batches
                test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Batches_All);

                // Verify URL and Title for the ALL page
                string url = test.GeneralMethods.GetUrl();
                TestLog.WriteLine("URL: " + url);
                TestLog.WriteLine("Verify URL: " + (string.Format("{0}/bridge/batches/batch/index", F1Environment)));
                Assert.Contains(url, "/bridge/batches/batch/index", "URL was incorrect");
                Assert.AreEqual("Fellowship One :: All Batches", test.Driver.Title, "Title did not match");

                // Click on the General link
                test.Driver.FindElementByLinkText("General").Click();

                // Verify URL for the General page
                string url2 = test.GeneralMethods.GetUrl();
                TestLog.WriteLine("URL: " + url2);
                TestLog.WriteLine("Verify URL: " + (string.Format("{0}/bridge/batches/batch/index", F1Environment)));
                Assert.Contains(url, "/bridge/batches/batch/index", "URL was incorrect");
                Assert.AreEqual("Fellowship One :: General Batches", test.Driver.Title, "Title did not match");

                // Click on the gear icon and select Edit Batch
                decimal itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName, "Name", null);
                test.GeneralMethods.SelectOptionFromGearWebDriver(Convert.ToInt16(itemRow), "Edit batch");
                //test.GeneralMethods.WaitForElement(test.Driver, By.Id("is_cc"));

               
                // Click on the Return link
                test.Driver.FindElementByClassName("minimal_return_arrow").Click();
                //test.Driver.FindElementByLinkText(GeneralLinks.RETURN).Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("All"));

                // Verify Return takes you back to correct URL
                string url3 = test.GeneralMethods.GetUrl();
                TestLog.WriteLine("URL: " + url3);
                TestLog.WriteLine("Verify URL: " + (string.Format("{0}/bridge/batches/batch/index", F1Environment)));
                Assert.Contains(url, "/bridge/batches/batch/index", "URL was incorrect");

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            catch (Exception e) {
                throw new WebDriverException(e.StackTrace);
            }
            finally {
                // Clean up
                base.SQL.Giving_Batches_Delete(15, batchName, 1);
            }
 
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the URLs for the Credit Card batches pages.")]
        public void Giving_Contributions_Batches_Verify_URLs_CreditCard() {
            // Setup data
            string batchName = "URL Test Batch CC";
            base.SQL.Giving_Batches_Create(15, batchName, 10, 2);

            try {

                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // Navigate to Giving -> Batches
                test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Batches_All);

                // Click on the Credit Card link
                test.Driver.FindElementByLinkText("Credit Card").Click();

                // Verify URL for the Credit Card page
                string url = test.GeneralMethods.GetUrl();
                TestLog.WriteLine("URL: " + url);
                TestLog.WriteLine("Verify URL: " + (string.Format("{0}/bridge/batches/batch/creditcard", F1Environment)));
                Assert.Contains(url, "/bridge/batches/batch/creditcard", "URL was incorrect");
                Assert.AreEqual("Fellowship One :: Credit Card Batches", test.Driver.Title, "Title did not match");

                // Click on the gear icon and select Resume Progress
                decimal itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName, "Name", null);
                test.GeneralMethods.SelectOptionFromGearWebDriver(Convert.ToInt16(itemRow), "Resume progress");
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("New individual"));

                // Click on the Return link
                test.Driver.FindElementByClassName("minimal_return_arrow").Click();
                // test.Driver.FindElementByLinkText(GeneralLinks.RETURN).Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("All"));

                // Verify Return takes you back to the current page
                string url2 = test.GeneralMethods.GetUrl();
                TestLog.WriteLine("URL: " + url);
                TestLog.WriteLine("Verify URL: " + (string.Format("{0}/bridge/batches/batch/creditcard", F1Environment)));
                Assert.Contains(url2, "/bridge/batches/batch/creditcard", "URL was incorrect");

                // Click on the gear icon and select Edit Batch
                decimal itemRow2 = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName, "Name", null);
                test.GeneralMethods.SelectOptionFromGearWebDriver(Convert.ToInt16(itemRow2), "Edit batch");
                test.GeneralMethods.WaitForElement(test.Driver, By.Id("is_cc"));

                // Click on the Return link
                test.Driver.FindElementByClassName("minimal_return_arrow").Click();
                // test.Driver.FindElementByLinkText(GeneralLinks.RETURN).Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("All"));

                // Verify Return takes you back to the current page
                string url3 = test.GeneralMethods.GetUrl();
                TestLog.WriteLine("URL: " + url);
                TestLog.WriteLine("Verify URL: " + (string.Format("{0}/bridge/batches/batch/creditcard", F1Environment)));
                Assert.Contains(url3, "/bridge/batches/batch/creditcard", "URL was incorrect");

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            catch (Exception e) {
                throw new WebDriverException(e.StackTrace);
            }
            finally {
                // Clean up
                base.SQL.Giving_Batches_Delete(15, batchName, 2);
            }

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the URLs for the Scanned batches pages.")]
        public void Giving_Contributions_Batches_Verify_URLs_Scanned() {
            // Setup data
            string batchName = "URL Test Batch Scanned - Pending";
            string batchName2 = "URL Test Batch Scanned - Saved";
            base.SQL.Giving_Batches_Create_Scanned(15, batchName, false, new System.Collections.Generic.List<double>() { 777.00 }, null,
                new System.Collections.Generic.List<string> { "685397101" },
                new System.Collections.Generic.List<string> { "111000025" }, false, "Auto Test Fund", false);
            base.SQL.Giving_Batches_Create_Scanned(15, batchName2, true, new System.Collections.Generic.List<double>() { 999.00 }, null,
                new System.Collections.Generic.List<string> { "673328191" },
                new System.Collections.Generic.List<string> { "111000025" }, false, "Auto Test Fund", false);

            try {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // Navigate to Giving -> Batches
                test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Batches_All);

                // Click on the Scanned link
                test.Driver.FindElementByLinkText("Scanned").Click();

                // Verify URL for the Scanned page
                string url = test.GeneralMethods.GetUrl();
                TestLog.WriteLine("URL: " + url);
                TestLog.WriteLine("Verify URL: " + (string.Format("{0}/bridge/batches/batch/scanned", F1Environment)));
                Assert.Contains(url.ToLower(), "/bridge/batches/batch/scanned", "URL was incorrect");
                Assert.AreEqual("Fellowship One :: Scanned Batches", test.Driver.Title, "Title did not match");

                // Click on the gear icon and select View/Edit batch for Pending batch
                decimal itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName, "Name", null);
                test.GeneralMethods.SelectOptionFromGearWebDriver(Convert.ToInt16(itemRow), "View/Edit batch");
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Match"));

                // Click on the Return link
                test.Driver.FindElementByClassName("minimal_return_arrow").Click();
                // test.Driver.FindElementByLinkText(GeneralLinks.RETURN).Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("All"));

                // Verify Return takes you back to the correct page
                string url2 = test.GeneralMethods.GetUrl();
                TestLog.WriteLine("URL: " + url);
                TestLog.WriteLine("Verify URL: " + (string.Format("{0}/bridge/batches/batch/scanned", F1Environment)));
                Assert.Contains(url2.ToLower(), "/bridge/batches/batch/scanned", "URL was incorrect");

                // Click on the gear icon and select View audit for Pending batch
                decimal itemRow2 = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName, "Name", null);
                test.GeneralMethods.SelectOptionFromGearWebDriver(Convert.ToInt16(itemRow2), "View audit");
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Download Excel"));

                // Click on the Return link
                test.Driver.FindElementByClassName("minimal_return_arrow").Click();
                // test.Driver.FindElementByLinkText(GeneralLinks.RETURN).Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("All"));

                // Verify Return takes you back to the correct page
                string url3 = test.GeneralMethods.GetUrl();
                TestLog.WriteLine("URL: " + url);
                TestLog.WriteLine("Verify URL: " + (string.Format("{0}/bridge/batches/batch/scanned", F1Environment)));
                Assert.Contains(url3.ToLower(), "/bridge/batches/batch/scanned", "URL was incorrect");

                // Click on the gear icon and select Edit batch name for Pending batch
                decimal itemRow3 = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName, "Name", null);
                test.GeneralMethods.SelectOptionFromGearWebDriver(Convert.ToInt16(itemRow3), "Edit batch name");
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Cancel"));

                // Click on the Return link
                test.Driver.FindElementByClassName("minimal_return_arrow").Click();
                // test.Driver.FindElementByLinkText(GeneralLinks.RETURN).Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("All"));

                // Verify Return takes you back to the correct page
                string url4 = test.GeneralMethods.GetUrl();
                TestLog.WriteLine("URL: " + url);
                TestLog.WriteLine("Verify URL: " + (string.Format("{0}/bridge/batches/batch/scanned", F1Environment)));
                Assert.Contains(url4.ToLower(), "/bridge/batches/batch/scanned", "URL was incorrect");

                // Click on the gear icon and select Edit batch name for Pending batch
                decimal itemRow7 = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName, "Name", null);
                test.GeneralMethods.SelectOptionFromGearWebDriver(Convert.ToInt16(itemRow7), "Edit batch name");
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Cancel"));

                // Click on the Cancel link
                test.Driver.FindElementByLinkText("Cancel").Click();

                // Verify Cancel takes you back to the correct page
                string url7 = test.GeneralMethods.GetUrl();
                TestLog.WriteLine("URL: " + url);
                TestLog.WriteLine("Verify URL: " + (string.Format("{0}/bridge/batches/batch/scanned", F1Environment)));
                Assert.Contains(url7.ToLower(), "/bridge/batches/batch/scanned", "URL was incorrect");

                // Click on the gear icon and select View batch for Saved batch
                decimal itemRow4 = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName2, "Name", null);
                test.GeneralMethods.SelectOptionFromGearWebDriver(Convert.ToInt16(itemRow4), "View batch");
                
                /* Behavior changed due to Story F1-4057. User is now taken to Search view.
                test.GeneralMethods.WaitForElement(test.Driver, By.Id("tab_back"));

                // Return to previous page
                test.Driver.FindElementById("tab_back").Click();
                 */

                //Verify that we are in Contribution Search
                //Story F1-4057 changes
                string urlScanned = test.GeneralMethods.GetUrl();
                TestLog.WriteLine("URL: " + urlScanned);
                TestLog.WriteLine("Verify URL: " + (string.Format("{0}/giving/contributionsearch.aspx", F1Environment)));
                Assert.Contains(urlScanned.ToLower(), "/giving/contributionsearch.aspx", "URL was incorrect");
                test.GeneralMethods.VerifyTextPresentWebDriver("Contribution Search");

                test.GeneralMethods.Navigate_Back();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("All"));

                // Verify Return takes you back to the current page
                string url5 = test.GeneralMethods.GetUrl();
                TestLog.WriteLine("URL: " + url);
                TestLog.WriteLine("Verify URL: " + (string.Format("{0}/bridge/batches/batch/scanned", F1Environment)));
                Assert.Contains(url5.ToLower(), "/bridge/batches/batch/scanned", "URL was incorrect");

                // Click on the gear icon and select View audit for Saved batch
                // TODO FIX AUDIT. Tested Manually on another saved batch.
                decimal itemRow5 = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName2, "Name", null);
                test.GeneralMethods.SelectOptionFromGearWebDriver(Convert.ToInt16(itemRow5), "View audit");
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Download Excel"));

                // Click on the Return link
                test.Driver.FindElementByClassName("minimal_return_arrow").Click();
                // test.Driver.FindElementByLinkText(GeneralLinks.RETURN).Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("All"));

                // Verify Return takes you back to the current page
                string url8 = test.GeneralMethods.GetUrl();
                TestLog.WriteLine("URL: " + url);
                TestLog.WriteLine("Verify URL: " + (string.Format("{0}/bridge/batches/batch/scanned", F1Environment)));
                Assert.Contains(url8.ToLower(), "/bridge/batches/batch/scanned", "URL was incorrect");

                // Click on the gear icon and select Edit batch name for Saved batch
                decimal itemRow6 = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName2, "Name", null);
                test.GeneralMethods.SelectOptionFromGearWebDriver(Convert.ToInt16(itemRow6), "Edit batch name");
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Cancel"));

                // Click on the Return link
                test.Driver.FindElementByClassName("minimal_return_arrow").Click();
                // test.Driver.FindElementByLinkText(GeneralLinks.RETURN).Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("All"));

                // Verify Return takes you back to the current page
                string url6 = test.GeneralMethods.GetUrl();
                TestLog.WriteLine("URL: " + url);
                TestLog.WriteLine("Verify URL: " + (string.Format("{0}/bridge/batches/batch/scanned", F1Environment)));
                Assert.Contains(url6.ToLower(), "/bridge/batches/batch/scanned", "URL was incorrect");

                // Logout of Portal
                test.Portal.LogoutWebDriver();

            }
            catch (Exception e) {
                throw new WebDriverException(e.Message + " " + e.StackTrace);
            }
            finally {
                // Delete batch
                base.SQL.Giving_Batches_Delete(15, batchName, 4);
                base.SQL.Giving_Batches_Delete(15, batchName2, 4);
            }
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the URLs for the RDC batches pages.")]
        public void Giving_Contributions_Batches_Verify_URLs_RDC() {
            // Setup data
            string batchName = "URL Test Batch for RDC - Pending";
            string batchName2 = "URL Test Batch for RDC - Saved";
            string acctNumber = "666333111";
            string routNumber = "111000025";
            base.SQL.Giving_Batches_Create_RDC(15, batchName, false, new System.Collections.Generic.List<double>() { 555.00 }, null,
                new System.Collections.Generic.List<string> { acctNumber },
                new System.Collections.Generic.List<string> { routNumber }, false, "Auto Test Fund", false);
            base.SQL.Giving_Batches_Create_RDC(15, batchName2, true, new System.Collections.Generic.List<double>() { 444.00 }, null,
                new System.Collections.Generic.List<string> { acctNumber },
                new System.Collections.Generic.List<string> { routNumber }, false, "Auto Test Fund", false);

            try {

                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // Navigate to Giving -> Batches
                test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Batches_All);

                // Click on theRemote Deposit Capture link
                test.Driver.FindElementByLinkText("Remote Deposit Capture").Click();

                // Verify URL for the RDC page
                string url = test.GeneralMethods.GetUrl();
                TestLog.WriteLine("URL: " + url);
                TestLog.WriteLine("Verify URL: " + (string.Format("{0}/bridge/batches/batch/remotedepositcapture", F1Environment)));
                Assert.Contains(url.ToLower(), "/bridge/batches/batch/remotedepositcapture", "URL was incorrect");
                Assert.AreEqual("Fellowship One :: Remote Deposit Capture Batches", test.Driver.Title, "Title did not match");

                // Click on the gear icon and select View/Edit batch for Pending batch
                decimal itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName, "Name", null);
                test.GeneralMethods.SelectOptionFromGearWebDriver(Convert.ToInt16(itemRow), "View/Edit batch");
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Match"));

                // Click on the Return link
                test.Driver.FindElementByClassName("minimal_return_arrow").Click();
                // test.Driver.FindElementByLinkText(GeneralLinks.RETURN).Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("All"));

                // Verify Return takes you back to the correct page
                string url2 = test.GeneralMethods.GetUrl();
                TestLog.WriteLine("URL: " + url);
                TestLog.WriteLine("Verify URL: " + (string.Format("{0}/bridge/batches/batch/remotedepositcapture", F1Environment)));
                Assert.Contains(url2.ToLower(), "/bridge/batches/batch/remotedepositcapture", "URL was incorrect");

                // Click on the gear icon and select View audit for Pending batch
                decimal itemRow2 = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName, "Name", null);
                test.GeneralMethods.SelectOptionFromGearWebDriver(Convert.ToInt16(itemRow2), "View audit");
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Download Excel"));

                // Click on the Return link
                test.Driver.FindElementByClassName("minimal_return_arrow").Click();
                // test.Driver.FindElementByLinkText(GeneralLinks.RETURN).Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("All"));

                // Verify Return takes you back to the correct page
                string url3 = test.GeneralMethods.GetUrl();
                TestLog.WriteLine("URL: " + url);
                TestLog.WriteLine("Verify URL: " + (string.Format("{0}/bridge/batches/batch/remotedepositcapture", F1Environment)));
                Assert.Contains(url3.ToLower(), "/bridge/batches/batch/remotedepositcapture", "URL was incorrect");

                // Click on the gear icon and select View batch for Saved batch
                decimal itemRow4 = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName2, "Name", null);
                test.GeneralMethods.SelectOptionFromGearWebDriver(Convert.ToInt16(itemRow4), "View batch");
                test.GeneralMethods.WaitForElement(test.Driver, By.Id("tab_back"));

                // Return to previous page
                test.Driver.FindElementById("tab_back").Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("All"));

                // Verify Return takes you back to the current page
                string url5 = test.GeneralMethods.GetUrl();
                TestLog.WriteLine("URL: " + url);
                TestLog.WriteLine("Verify URL: " + (string.Format("{0}/bridge/batches/batch/remotedepositcapture", F1Environment)));
                Assert.Contains(url5.ToLower(), "/bridge/batches/batch/remotedepositcapture", "URL was incorrect");

                // Click on the gear icon and select View audit for Saved batch
                decimal itemRow5 = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, batchName2, "Name", null);
                test.GeneralMethods.SelectOptionFromGearWebDriver(Convert.ToInt16(itemRow5), "View audit");
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Download Excel"));

                // Click on the Return link
                test.Driver.FindElementByClassName("minimal_return_arrow").Click();
                // test.Driver.FindElementByLinkText(GeneralLinks.RETURN).Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("All"));

                // Verify Return takes you back to the current page
                string url8 = test.GeneralMethods.GetUrl();
                TestLog.WriteLine("URL: " + url);
                TestLog.WriteLine("Verify URL: " + (string.Format("{0}/bridge/batches/batch/remotedepositcapture", F1Environment)));
                Assert.Contains(url8.ToLower(), "/bridge/batches/batch/remotedepositcapture", "URL was incorrect");

                // Logout of Portal
                test.Portal.LogoutWebDriver();

            }
            catch (Exception e) {
                throw new WebDriverException(e.StackTrace);
                
            }
            finally {
                // Delete batch
                base.SQL.Giving_Batches_Delete(15, batchName, 3);
                base.SQL.Giving_Batches_Delete_RDC(15, batchName, acctNumber, routNumber);
                base.SQL.Giving_Batches_Delete(15, batchName2, 3);
                base.SQL.Giving_Batches_Delete_RDC(15, batchName2, acctNumber, routNumber);
            }
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the Batches page (and subsequent pages) under giving.")]
        public void Giving_Contributions_Batches() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Navigate to giving->batches
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Batches_General);
            test.GeneralMethods.WaitForElement(test.Driver, By.PartialLinkText("Add a new batch"));

            // Verify title, text
            Assert.AreEqual("Fellowship One :: General Batches", test.Driver.Title, "Title Incorrect");
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("General Batches"));

            // Verify the gear options associated with a regular batch
            //                                             /html/body/div/div[5]/div/div/div/div[2]/form/table/tbody/tr[2]/td[6]/ul/li/a
            Assert.IsTrue(test.Driver.FindElementById(TableIds.Portal.Giving_Batches).FindElements(By.XPath("//tbody/tr[2]/td[position()=6 and count(ul/li)=2 and ul/li[1]/a/span/text()='View batch' and ul/li[2]/a/span/text()='Edit batch']")).Count > 0, "Gear Options Missing");


            // Click to create a new batch
            test.Driver.FindElementByLinkText("Add a new batch").Click();

            // Verify labels
            Assert.AreEqual("Batch name *", test.Driver.FindElementByXPath("//label[@for='batch_name']").Text);
            Assert.AreEqual("Batch amount *", test.Driver.FindElementByXPath("//label[@for='batch_amount']").Text);

            // Verify button text
            Assert.AreEqual("Save batch", test.Driver.FindElementByXPath("//input[@id='submitQuery']").GetAttribute("value"));

            // Check the box indicating this will be a credit card batch
            WebDriverWait wait = new WebDriverWait(test.Driver, TimeSpan.FromMilliseconds(2500));
            test.Driver.FindElementById("is_cc").Click();
            IWebElement receivedDate = wait.Until<IWebElement>((e) => { return e.FindElement(By.Id("received_date")); });

            // Verify the label for received date
            Assert.IsTrue(test.Driver.FindElementByXPath("//tr[@id='choose_a_received_date']/th").Text == "Received *");

            // Verify the received date control has autocomplete turned off
            Assert.IsTrue(test.Driver.FindElementsByXPath("//input[@id='received_date' and @autocomplete='off']").Count > 0);

            // Verify the formatting on the received date date control
            test.GeneralMethods.VerifyDateControlWebDriver("received_date");

            // Return to the batches landing page
            test.Driver.FindElementByLinkText("RETURN").Click();
            Assert.AreEqual("Fellowship One :: General Batches", test.Driver.Title);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("General Batches"));


            // Edit an existing batch
            test.GeneralMethods.SelectOptionFromGearWebDriver(1, "Edit batch");

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Edit Batch", test.Driver.Title);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Edit Batch"));


            // Return to the landing page
            test.Driver.FindElementByLinkText("RETURN").Click();

            // Verify title, text
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Add a new batch"), 30, "Timeout loading General Batches page");
            Assert.AreEqual("Fellowship One :: General Batches", test.Driver.Title, "Title Mismatch");
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("General Batches"));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        #region Credit Card Batches
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the Credit Card Batches page under giving.")]
        public void Giving_Contributions_Batches_CreditCardBatches() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Navigate to giving->batches->credit card batches
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Batches_CreditCardBatches);

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Credit Card Batches", test.Driver.Title);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Credit Card Batches"));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the audit batch page for a credit card batch.")]
        public void Giving_Contributions_Batches_CreditCardBatches_Audit() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Navigate to giving->batches->credit card batches
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Batches_CreditCardBatches);

            // View the audit page for a credit card batch
            string batchName = test.Driver.FindElementById(TableIds.Portal.Giving_Batches).FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[0].Text;
            string batchAmount = test.Driver.FindElementById(TableIds.Portal.Giving_Batches).FindElement(By.XPath(string.Format("//tbody/tr[*]/td[1]/a[normalize-space(text())='{0}']/ancestor::tr/td[4]", batchName))).Text;
            test.Driver.FindElementById(TableIds.Portal.Giving_Batches).FindElement(By.XPath(string.Format("//tbody/tr[*]/td[1]/a[normalize-space(text())='{0}']", batchName))).Click();
            Assert.AreEqual("Fellowship One :: Audit Batch", test.Driver.Title);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Audit Batch"));
            Assert.AreEqual(string.Format("{0} — {1}", batchName, batchAmount), test.Driver.FindElementByXPath("//h3[@class='float_left']").Text);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the Edit Batch page for credit card batch.")]
        public void Giving_Contributions_Batches_CreditCardBatches_Edit() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Navigate to giving->batches->credit card batches
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Batches_CreditCardBatches);

            // View the edit batch page for a credit card batch
            IWebElement table = test.Driver.FindElementById(TableIds.Portal.Giving_Batches);
            //string commonXPath = string.Format("{0}/tbody/tr[2]", TableIds.Portal.Giving_Batches);
            string batchName = table.FindElements(By.TagName("td"))[0].FindElement(By.TagName("a")).Text;
            string batchAmount = table.FindElements(By.TagName("td"))[3].Text;
            test.GeneralMethods.SelectOptionFromGearWebDriver(1, "Edit batch");

            // Verify the name, amount, and cc checkbox
            Assert.AreEqual(batchName, test.Driver.FindElementById("batch_name").GetAttribute("value"));
            Assert.AreEqual(string.Format("$ {0}", batchAmount.Replace("$", "").Replace(",", "")), test.Driver.FindElementByXPath("//span[@class='text_green']").Text);
            Assert.IsTrue(test.Driver.FindElementsByXPath("//input[@id='batch_amount' and @type='hidden']").Count > 0);
            Assert.IsTrue(test.Driver.FindElementsByXPath("//input[@id='is_cc' and @disabled='disabled']").Count > 0);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Credit Card Batches

        #region Remote Deposit Capture
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the Remote Deposit Capture landing page.")]
        public void Giving_Contributions_Batches_RemoteDepositCapture() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Navigate to giving->batches->remote deposit capture
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Batches_General);
            test.Driver.FindElementByLinkText("Remote Deposit Capture").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Refresh batch listing"), 20);

            // Verify title, text
            //Assert.AreEqual("Fellowship One :: Remote Deposit Capture", test.Driver.Title);
            //F1-3376 Consolidated Batches
            Assert.AreEqual("Fellowship One :: Remote Deposit Capture Batches", test.Driver.Title, "Title Incorrect");            
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Remote Deposit Capture Batches"));
            // Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Imported Batches"));

            // Verify the date tooltip
            // Assert.IsTrue(test.Driver.FindElementsByXPath("//th/abbr[@title='Date is when the batch was created via ProfitStars.' and text()='Date']").Count > 0);

            // Verify the time delta from the last 2 times the rdcchecker service ran
            StringBuilder script = new StringBuilder("DECLARE @time1 DATETIME, @time2 DATETIME, @applicationLogID INT ");
            script.Append("SELECT @time1 = CREATED_DATE, @applicationLogID = APPLICATION_LOG_ID FROM ChmPortal.dbo.APPLICATION_LOG WITH (NOLOCK) WHERE APPLICATION_LOG_ID = (SELECT MAX(APPLICATION_LOG_ID) FROM ChmPortal.dbo.APPLICATION_LOG WHERE APPLICATION_ID = 30) ");
            script.Append("SET @time2 = (SELECT CREATED_DATE FROM ChmPortal.dbo.APPLICATION_LOG WITH (NOLOCK) WHERE APPLICATION_LOG_ID = (SELECT MAX(APPLICATION_LOG_ID) FROM ChmPortal.dbo.APPLICATION_LOG WHERE APPLICATION_ID = 30 AND APPLICATION_LOG_ID < @applicationLogID)) ");
            script.Append("SELECT DATEDIFF(minute, @time2, @time1)");
            Assert.Between(base.SQL.Execute(script.ToString()).Rows[0][0], 20, 22, "RDC Checkeer Service did not run in last 20 minutes");
            //Assert.AreEqual(20, base.SQL.Execute(script.ToString()).Rows[0][0], "RDC Checkeer Service did not run in last 20 minutes");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Remote Deposit Capture

        #region Scanned
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Felix Gaytan")]
        [Description("Navigates to the Scanned Batches page under giving.")]
        public void Giving_Contributions_Batches_Scanned()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Navigate to giving->batches
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Batches_All);
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Scanned"));

            //Verify that All is current pill highlighed
            Assert.AreEqual("All", test.Driver.FindElement(By.XPath("//a[@class='current']")).Text, "All is not current highlighted pill");

            test.Driver.FindElementByLinkText("Scanned").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("View unmatched contributions"));

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Scanned Batches", test.Driver.Title, "Title Incorrect");
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Scanned Batches"));
            
            //Ivan: we should select the pending option and then click the filter btn
            test.Driver.FindElementById("bool_filter_false").Click();
            test.Driver.FindElementById("RDCFilterButton").Click();
            
            //waiting for page loading completed.
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(".//*[@id='batches_grid']/tbody/tr[2]/td[2]/span[normalize-space(text())='Pending']"));
             
            // Verify the gear options associated with a regular batch
            //                                             /html/body/div/div[5]/div/div/div/div[2]/form/table/tbody/tr[2]/td[5]/ul
            decimal itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Giving_Batches, "Pending", "Status", null);
            TestLog.WriteLine("Item row: {0}",itemRow);
            Assert.IsTrue(test.Driver.FindElementById(TableIds.Portal.Giving_Batches).FindElements(By.XPath(string.Format("//tbody/tr[{0}]/td[position()=5 and count(ul/li)=4 and ul/li[1]/a/span/text()='View/Edit batch' and ul/li[2]/a/span/text()='View audit' and ul/li[3]/a/span/text()='Edit batch name']", itemRow + 1))).Count > 0);


            // Click to create a new batch
            test.Driver.FindElementByLinkText("View unmatched contributions").Click();

            //Verify Batches and All Links
            test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Batches"));
            test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("All"));

            //Verify that All is current pill highlighed
            Assert.AreEqual("All", test.Driver.FindElement(By.XPath("//a[@class='current']")).Text, "All is not current highlighted pill");

            //Click on Batches

            //Click on All
            test.Driver.FindElementByLinkText("All").Click();

            //Verify Header columns
            IWebElement table = test.Driver.FindElementByXPath(TableIds.Giving_Unmatched_Batches);
            IWebElement row = table.FindElements(By.TagName("tr"))[0];
            Assert.AreEqual("Amount", row.FindElements(By.TagName("th"))[0].Text, "Amount column header missing");
            Assert.AreEqual("Type", row.FindElements(By.TagName("th"))[1].Text, "Type column header missing");
            Assert.AreEqual("Date", row.FindElements(By.TagName("th"))[2].Text, "Date column header missing");

            // Verify Date labels
            test.GeneralMethods.IsElementPresentWebDriver(By.Id("start_date"));
            test.GeneralMethods.IsElementPresentWebDriver(By.Id("end_date"));

            // Verify button text
            Assert.AreEqual("Search", test.Driver.FindElementByXPath("//input[@id='submitQuery']").GetAttribute("value"));

            // Can't return to previous so let's try this trick
            // Return to the batches landing page
            // Do this twice since you went forward already once by clicking all
            test.GeneralMethods.Navigate_Back();
            test.GeneralMethods.Navigate_Back();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("View unmatched contributions"), 20, "Was not able to go back using browser");

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Scanned Batches", test.Driver.Title, "Title Incorrect");
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Scanned Batches"));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        #endregion Scanned
        #endregion Batches

        #region Enter Contributions
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Navigates to the enter contributions page under giving.")]
        public void Giving_Contributions_EnterContributions() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Navigate to giving->enter contributions
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Enter_Contributions);

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Enter Contributions", test.Driver.Title);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Enter Contributions"));

            // Verify button text
            Assert.AreEqual("Add split", test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSaveSplit").GetAttribute("value"));
            Assert.AreEqual("Add contribution", test.Driver.FindElementById(GeneralButtons.Save).GetAttribute("value"));

            // Verify checkbox text
            Assert.AreEqual("Do not thank", test.Driver.FindElementByXPath("//label[@for='ctl00_ctl00_MainContent_content_chkBoxThank']").Text);

            // Verify column header
            Assert.AreEqual("Attributed To", test.Driver.FindElementByXPath("//table[@id='ctl00_ctl00_MainContent_content_dgSplitContributions']/tbody/tr/td[2]").Text);

            #region Popups
            // Store the main window's handle
            string mainWindowHandle = test.Driver.CurrentWindowHandle;

            // View the batches popup
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_lnkSelectBatch").Click();
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]);

            // Verify the labels
            Assert.AreEqual("Batch amount", test.Driver.FindElementById("ctl00_content_lblBatchAmtSearch").Text);
            Assert.AreEqual("Batch date", test.Driver.FindElementById("ctl00_content_lblBatchDateSearch").Text);

            // Close the popup
            test.Driver.FindElementByLinkText("Cancel").Click();
            test.Driver.SwitchTo().Window(mainWindowHandle);


            // View the find person popup
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlFindPerson_lnkFindPerson").Click();
            test.Driver.SwitchTo().Window("psuedoModal");

            // Verify the labels
            Assert.AreEqual("Attribute group", test.Driver.FindElementByXPath("//label[@for='ctl00_content_ddlAttributeGroup_dropDownList']").Text);

            // Close the popup
            test.Driver.FindElementByLinkText("Cancel").Click();
            test.Driver.SwitchTo().Window(mainWindowHandle);


            // View the activity date range popup
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_lnkSelectActivity").Click();
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]);

            // Verify the labels
            Assert.AreEqual("Start date", test.Driver.FindElementById("ctl00_content_lblStartDate").Text);
            Assert.AreEqual("End date", test.Driver.FindElementById("ctl00_content_lblEndDate").Text);
            Assert.AreEqual("Activity time *", test.Driver.FindElementById("ctl00_content_lblActivityInstance").Text);

            // Verify the ok button
            Assert.AreEqual("OK", test.Driver.FindElementById("ctl00_content_btnOk").GetAttribute("value"));

            // Close the popup
            test.Driver.FindElementByLinkText("Close window").Click();
            test.Driver.SwitchTo().Window(mainWindowHandle);
            #endregion Popups

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Enter Contributions

        #region Unmatched
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Navigates to the unmatched page under giving.")]
        public void Giving_Contributions_Unmatched() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to giving->unmatched
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Contributions.Unmatched);

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Unmatched Contributions", test.Driver.Title);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Unmatched Contributions"));

            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Selected Contributor"));

            // Verify "Match" button is not present by default
            Assert.IsFalse(test.Driver.FindElementsByXPath("//input[@value='Match']").Count > 0);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Unmatched
        #endregion Contributions

        #region Statements
        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Navigates to the statement builder page under giving.")]
        public void Giving_Statements_StatementBuilder() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Navigate to giving->statement builder
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Statements.Statement_Builder);

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Statement Builder", test.Driver.Title);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Statement Builder"));

            // Verify labels
            Assert.AreEqual("Include detail contributions made within the following time range", test.Driver.FindElementByXPath("//fieldset[@id='fsDateRange']/p[1]").Text);
            Assert.AreEqual("Include summarized contributions by funds within the following time range", test.Driver.FindElementByXPath("//fieldset[@id='fsDateRange']/p[2]").Text);

            Assert.AreEqual("Limit the results to an household/organization or postal code range", test.Driver.FindElementByXPath("//fieldset[@id='fsFilters']/p[1]").Text);
            Assert.AreEqual("Generate statements for the following Funds, Sub Funds and Pledge drives", test.Driver.FindElementByXPath("//fieldset[@id='fsFilters']/p[3]").Text);
            Assert.AreEqual("Include only individuals with the following Statuses and/or Attributes", test.Driver.FindElementByXPath("//fieldset[@id='fsFilters']/p[4]").Text);
            Assert.AreEqual("Include only the following Household Member Positions", test.Driver.FindElementByXPath("//fieldset[@id='fsFilters']/p[5]").Text);

            Assert.AreEqual("Enter email addresses to notify when statement generation is complete. Use a semi-colon (;) to separate addresses.", test.Driver.FindElementByXPath("//form[@id='aspnetForm']/div[10]/fieldset[4]/p").Text);
            Assert.AreEqual("When generating statements, include the following", test.Driver.FindElementByXPath("//form[@id='aspnetForm']/div[11]/fieldset/p[1]").Text);

            Assert.AreEqual("Additional files", test.Driver.FindElementByXPath("//form[@id='aspnetForm']/div[11]/fieldset/p[8]").Text);

            #region Popups
            // Store the main window's handle
            string mainWindowHandle = test.Driver.CurrentWindowHandle;

            // View the funds popup
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_lnkSelectFunds").Click();
            test.Driver.SwitchTo().Window("psuedoModal");

            // Verify the select funds button
            Assert.AreEqual("Select funds", test.Driver.FindElementById("ctl00_content_btnSelectFunds").GetAttribute("value"));

            // Close the popup
            test.Driver.FindElementById("ctl00_content_btnCancel").Click();
            test.Driver.SwitchTo().Window(mainWindowHandle);


            // View the sub funds popup
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_lnkSelectSubFunds").Click();
            test.Driver.SwitchTo().Window("psuedoModal");

            // Verify the select statuses button
            Assert.AreEqual("Select sub funds", test.Driver.FindElementById("ctl00_content_btnSelectSubFunds").GetAttribute("value"));

            // Close the popup
            test.Driver.FindElementById("ctl00_content_btnCancel").Click();
            test.Driver.SwitchTo().Window(mainWindowHandle);


            // View the pledge drives popup
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_lnkSelectPledgeDrives").Click();
            test.Driver.SwitchTo().Window("psuedoModal");

            // Verify the select pledge drives button
            Assert.AreEqual("Select pledge drives", test.Driver.FindElementById("ctl00_content_btnSelectPledgeDrives").GetAttribute("value"));

            // Close the popup
            test.Driver.FindElementById("ctl00_content_btnCancel").Click();
            test.Driver.SwitchTo().Window(mainWindowHandle);


            // View the statuses popup
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_lnkSelectIndividualStatus").Click();
            test.Driver.SwitchTo().Window("psuedoModal");

            // Verify the select statuses button
            Assert.AreEqual("Select statuses", test.Driver.FindElementById("ctl00_content_btnSelectStatuses").GetAttribute("value"));

            // Close the popup
            test.Driver.FindElementById("ctl00_content_btnCancel").Click();
            test.Driver.SwitchTo().Window(mainWindowHandle);


            // View the attributes popup
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_lnkSelectIndividualAttribute").Click();
            test.Driver.SwitchTo().Window("psuedoModal");

            // Verify the select attributes button
            Assert.AreEqual("Select attributes", test.Driver.FindElementById("ctl00_content_btnSelectAttributes").GetAttribute("value"));

            // Close the popup
            test.Driver.FindElementById("ctl00_content_btnCancel").Click();
            test.Driver.SwitchTo().Window(mainWindowHandle);


            // View the positions popup
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_lnkSelectHouseholdPosition").Click();
            test.Driver.SwitchTo().Window("psuedoModal");

            // Verify the select positions button
            Assert.AreEqual("Select positions", test.Driver.FindElementById("ctl00_content_btnSelectPositions").GetAttribute("value"));

            // Close the popup
            test.Driver.FindElementById("ctl00_content_btnCancel").Click();
            test.Driver.SwitchTo().Window(mainWindowHandle);


            // View the rollup individual contributions to household popup
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_chkRollupIndividuals").Click();
            test.Driver.SwitchTo().Window("MessageBoxWindow");

            // Verify the text on the popup
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("You previously selected to include Individual statements in the output. By choosing to roll up indivdual contributions you will no longer be able include Individual statements in the output."));

            // Close the popup
            test.Driver.FindElementByXPath("//input[@value='OK']").Click();
            test.Driver.SwitchTo().Window(mainWindowHandle);
            #endregion Popups

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Navigates to the queue page under giving.")]
        public void Giving_Statements_Queue() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Navigate to giving->queue
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Statements.Queue);

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Statement Queue", test.Driver.Title);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Statement Queue"));

            // Verify button text
            Assert.AreEqual("Refresh statement requests", test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnRefresh").GetAttribute("value"));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Navigates to the custom styles page under giving.")]
        public void Giving_Statements_CustomStyles() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Navigate to giving->custom styles
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Statements.Custom_Styles);

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Custom Styles", test.Driver.Title);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Custom Styles"));

            // Verify column header
            Assert.AreEqual("Style Name", test.Driver.FindElementByXPath("//table[@id='ctl00_ctl00_MainContent_content_dgFormats']/tbody/tr[1]/td[1]").Text);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Navigates to the online statement page under giving.")]
        public void Giving_Statements_OnlineStatement() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Navigate giving->online statement
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Statements.Online_Statement);

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Statement Details", test.Driver.Title);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Statement Details"));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Statements

        #region Setup
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Navigates to the funds page (and subsequent pages) under giving.")]
        public void Giving_Setup_Funds() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Navigate to giving->funds
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Setup.Funds);

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Funds", test.Driver.Title);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Funds"));

            // Verify button
            Assert.AreEqual("Add fund", test.Driver.FindElementById(GeneralButtons.Save).GetAttribute("value"));

            // Verify column headers
            Assert.AreEqual("Fund Name", test.Driver.FindElementByXPath("//table[@id='ctl00_ctl00_MainContent_content_dgFunds']/tbody/tr[1]/td[2]").Text);
            Assert.AreEqual("Fund Code", test.Driver.FindElementByXPath("//table[@id='ctl00_ctl00_MainContent_content_dgFunds']/tbody/tr[1]/td[3]").Text);
            Assert.AreEqual("Fund Type", test.Driver.FindElementByXPath("//table[@id='ctl00_ctl00_MainContent_content_dgFunds']/tbody/tr[1]/td[4]").Text);
            Assert.AreEqual("Online Giving", test.Driver.FindElementByXPath("//table[@id='ctl00_ctl00_MainContent_content_dgFunds']/tbody/tr[1]/td[5]").Text);
            Assert.AreEqual("Account Reference Desc.", test.Driver.FindElementByXPath("//table[@id='ctl00_ctl00_MainContent_content_dgFunds']/tbody/tr[1]/td[6]").Text);

            // Click to edit an existing fund
            test.Driver.FindElementByLinkText("Edit").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.Id(GeneralButtons.Save));

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Funds", test.Driver.Title);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Funds"));

            // Verify button
            Assert.AreEqual("Save fund", test.Driver.FindElementById(GeneralButtons.Save).GetAttribute("value"));

            // Verify cancel control
            Assert.IsTrue(test.Driver.FindElementsByLinkText("Cancel").Count > 0);

            // Navigate to giving->sub funds
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Setup.Sub_Funds);

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Sub Funds", test.Driver.Title);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Sub Funds"));

            // Verify column headers
            Assert.AreEqual("Sub Fund Name", test.Driver.FindElementByXPath("//table[@id='ctl00_ctl00_MainContent_content_dgSubFund']/tbody/tr[1]/td[2]").Text);
            Assert.AreEqual("Parent Fund", test.Driver.FindElementByXPath("//table[@id='ctl00_ctl00_MainContent_content_dgSubFund']/tbody/tr[1]/td[3]").Text);
            Assert.AreEqual("General Ledger", test.Driver.FindElementByXPath("//table[@id='ctl00_ctl00_MainContent_content_dgSubFund']/tbody/tr[1]/td[4]").Text);
            Assert.AreEqual("Sub Fund Code", test.Driver.FindElementByXPath("//table[@id='ctl00_ctl00_MainContent_content_dgSubFund']/tbody/tr[1]/td[5]").Text);
            Assert.AreEqual("Online Giving", test.Driver.FindElementByXPath("//table[@id='ctl00_ctl00_MainContent_content_dgSubFund']/tbody/tr[1]/td[6]").Text);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Navigates to the pledge drives page under giving.")]
        public void Giving_Setup_PledgeDrives() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Navigate to giving->pledge drives
            test.GeneralMethods.Navigate_Portal(Navigation.Giving.Setup.Pledge_Drives);

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Pledge Drives", test.Driver.Title);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Pledge Drives"));

            // Select a fund
            new SelectElement(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlFund_dropDownList")).SelectByIndex(0);

            // Verify label
            Assert.AreEqual("Active", test.Driver.FindElementByXPath("//span[@id='ctl00_ctl00_MainContent_content_Label1']").Text);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the contribution attributes page under giving.")]
        public void Giving_Setup_ContributionAttributes() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Navigate to giving->contribution attributes
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Setup.Contribution_Attributes);

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Contribution Attributes", test.Driver.Title);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Contribution Attributes"));

            // Search to display table
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctl02_DateTextBox").Clear();
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctl02_DateTextBox").SendKeys("9/18/2005");
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_chkCurrentUserOnly").Click();

            WebDriverWait wait = new WebDriverWait(test.Driver, TimeSpan.FromMilliseconds(5000));
            test.Driver.FindElementById(GeneralButtons.Search).Click();
            IWebElement menuHeader = wait.Until<IWebElement>((e) => { return e.FindElement(By.Id("ctl00_ctl00_MainContent_content_dgAttrs_ctl02_lnkEdit")); });

            // Verify column headers
            Assert.AreEqual("Contribution Attribute", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[1]/td[2]", TableIds.Giving_ContributionAttributes)).Text);
            Assert.AreEqual("Create Date", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[1]/td[3]", TableIds.Giving_ContributionAttributes)).Text);
            Assert.AreEqual("# of Records", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[1]/td[4]", TableIds.Giving_ContributionAttributes)).Text);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Navigates to the sub types page under giving.")]
        public void Giving_Setup_SubTypes() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Navigate to giving->sub types
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Setup.Sub_Types);
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath("//table[@id='ctl00_ctl00_MainContent_content_dgContributionType']/tbody/tr[1]/td[2]"));

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Sub Types", test.Driver.Title);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Sub Types"));

            // Verify column headers
            Assert.AreEqual("Contribution Sub Type", test.Driver.FindElementByXPath("//table[@id='ctl00_ctl00_MainContent_content_dgContributionType']/tbody/tr[1]/td[2]").Text);
            Assert.AreEqual("Contribution Type", test.Driver.FindElementByXPath("//table[@id='ctl00_ctl00_MainContent_content_dgContributionType']/tbody/tr[1]/td[3]").Text);
            Assert.IsFalse(test.Driver.FindElementByTagName("html").Text.Contains("Gift Aid"));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Navigates to the account references page under giving.")]
        public void Giving_Setup_AccountReferences() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Navigate to giving->account references
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Setup.Account_References);

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Account References", test.Driver.Title);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Account References"));

            // Verify label
            Assert.AreEqual("Account reference *", test.Driver.FindElementById("ctl00_ctl00_MainContent_content_lblAccountCode").Text);

            // Verify column headers
            Assert.AreEqual("Account Reference", test.Driver.FindElementByXPath("//table[@id='ctl00_ctl00_MainContent_content_dgAccountReference']/tbody/tr[1]/td[2]").Text);
            Assert.AreEqual("Major Account Code", test.Driver.FindElementByXPath("//table[@id='ctl00_ctl00_MainContent_content_dgAccountReference']/tbody/tr[1]/td[4]").Text);
            Assert.AreEqual("Payment Gateway ID", test.Driver.FindElementByXPath("//table[@id='ctl00_ctl00_MainContent_content_dgAccountReference']/tbody/tr[1]/td[5]").Text);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Navigates to the organizations page under giving.")]
        public void Giving_Setup_Organizations() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            // Navigate to giving->organizations
            test.GeneralMethods.Navigate_Portal(Navigation.Giving.Setup.Organizations);

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Organizations", test.Driver.Title);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Organizations"));

            // Verify labels
            Assert.AreEqual("Address 1", test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlManageOrganization_ctlAddress_lblAddress1").Text);
            Assert.AreEqual("Address 2", test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlManageOrganization_ctlAddress_lblAddress2").Text);

            // Verify button
            Assert.AreEqual("Add organization", test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlManageOrganization_btnSave").GetAttribute("value"));

            // Verify default option for 'Active' drop-down list
            Assert.AreEqual("Active", new SelectElement(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlFilterActive_dropDownList")).SelectedOption.Text);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Setup
	}
}