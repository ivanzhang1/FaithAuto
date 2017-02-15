using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System.Text.RegularExpressions;

namespace FTTests.Portal.Giving {
	[TestFixture]
	public class Portal_Giving_Setup : FixtureBaseWebDriver {
		#region Funds
		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Matthew Sneeden")]
		[Description("Creates a Fund.")]
		public void Giving_Setup_Funds_Create() {
            // Set initial conditions
            string fundName = "0-Test Fund";
            base.SQL.Giving_Funds_Delete(15, fundName);

			// Login to portal
			TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                // Create a fund
                test.Portal.Giving_Funds_Create(fundName, "TF", "Contribution", false, "Auto Testing", true);

            }
            finally
            {
                base.SQL.Giving_Funds_Delete(15, fundName);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }

		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Updates a Fund.")]
		public void Giving_Setup_Funds_Update() {
            // Set initial conditions
            string fundName = "1Test Fund - Update";
            string fundCode = "TF";
            string accountReferenceDescription = "Auto Testing";
            string fundNameUpdated = "1Test Fund - Mod";
            base.SQL.Giving_Funds_Delete(15, fundName);
            base.SQL.Giving_Funds_Delete(15, fundNameUpdated);
            base.SQL.Giving_Funds_Create(15, fundName, true, fundCode, 1, false, accountReferenceDescription);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                // Update a fund
                test.Portal.Giving_Funds_Update(fundName, fundCode, "Contribution", false, accountReferenceDescription, true, fundNameUpdated, "TF-M", "Receipt", true, accountReferenceDescription, true);

            }
            finally
            {
                base.SQL.Giving_Funds_Delete(15, fundName);
                base.SQL.Giving_Funds_Delete(15, fundNameUpdated);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }
            
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Deletes a Fund.")]
		public void Giving_Setup_Funds_Delete() {
            // Set initial conditions
            string fundName = "0-Test Fund - Delete";
            base.SQL.Giving_Funds_Delete(15, fundName);
            base.SQL.Giving_Funds_Create(15, fundName, true, "TF-D", 2, true, "Auto Testing");

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                // Delete a fund
                test.Portal.Giving_Funds_Delete(fundName);
            }
            finally
            {
                base.SQL.Giving_Funds_Delete(15, fundName);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Creates a Web Enabled Fund.")]
		public void Giving_Setup_Funds_CreateWebEnabled() {
            // Set initial conditions
            string fundName = "0-Test Fund - Web Enabled2";
            base.SQL.Giving_Funds_Delete(15, fundName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                // Create a web enabled fund
                test.Portal.Giving_Funds_Create(fundName, "TF", "Contribution", true, "Auto Testing", true);
            }
            finally
            {
                base.SQL.Giving_Funds_Delete(15, fundName);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Creates an inactive Fund.")]
		public void Giving_Setup_Funds_CreateInactive() {
            // Set initial conditions
            string fundName = "0-Test Fund - Inactive";
            base.SQL.Giving_Funds_Delete(15, fundName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                // Create an inactive fund
                test.Portal.Giving_Funds_Create(fundName, "TF", "Contribution", false, "Auto Testing", false);
            }
            finally
            {
                base.SQL.Giving_Funds_Delete(15, fundName);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }

		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Creates a Receipt Fund.")]
		public void Giving_Setup_Funds_CreateReceipt() {
            // Set initial conditions
            string fundName = "0-Test Fund - Receipt";
            base.SQL.Giving_Funds_Delete(15, fundName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                // Create a receipt fund
                test.Portal.Giving_Funds_Create(fundName, "TF", "Receipt", true, "Auto Testing", true);
            }
            finally
            {
                base.SQL.Giving_Funds_Delete(15, fundName);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }

		}
		#endregion Funds

		#region Sub Funds
		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Matthew Sneeden")]
		[Description("Creates a Sub Fund.")]
		public void Giving_Setup_SubFunds_Create() {
            // Set initial conditions
            string fundName = "Test Fund - SF:C";
            string subFundName = "Test Sub Fund";
            base.SQL.Giving_SubFunds_Delete(15, fundName, subFundName);
            base.SQL.Giving_Funds_Delete(15, fundName);
            base.SQL.Giving_Funds_Create(15, fundName, true, "TS_TSF", 1, true, "DC");

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                // Create a sub fund
                test.Portal.Giving_SubFunds_Create(fundName, "Test Sub Fund", "GL", "TSF--12345678901234567890", true, true);
            }
            finally
            {
                base.SQL.Giving_SubFunds_Delete(15, fundName, subFundName);
                base.SQL.Giving_Funds_Delete(15, fundName);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }

		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Creates an inactive Sub Fund.")]
		public void Giving_Setup_SubFunds_CreateInactive() {
            // Set initial conditions
            string fundName = "Test Fund - SF:I";
            string subFundName = "Test Sub Fund - Inactive";
            base.SQL.Giving_SubFunds_Delete(15, fundName, subFundName);
            base.SQL.Giving_Funds_Delete(15, fundName);
            base.SQL.Giving_Funds_Create(15, fundName, true, "SF:I", 1, true, "DC");

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                // Create an inactive sub fund
                test.Portal.Giving_SubFunds_Create(fundName, subFundName, "GL", "TSFI01", true, false);
            }
            finally
            {
                base.SQL.Giving_SubFunds_Delete(15, fundName, subFundName);
                base.SQL.Giving_Funds_Delete(15, fundName);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }

		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Updates a Sub Fund.")]
		public void Giving_Setup_SubFunds_Update() {
            // Set initial conditions
            string fundName = "Test Fund - SF:U";
            string subFundName = "A Test Sub Fund - Update";
            string subFundNameUpdated = "A Test Sub Fund - Mod";
            base.SQL.Giving_SubFunds_Delete(15, fundName, subFundName);
            base.SQL.Giving_SubFunds_Delete(15, fundName, subFundNameUpdated);
            base.SQL.Giving_Funds_Delete(15, fundName);
            base.SQL.Giving_Funds_Create(15, fundName, true, "SF:U", 1, true, "DC");
            base.SQL.Giving_SubFunds_Create(15, fundName, subFundName, "TSF--12345678901234567890", true, "GL", true);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                // Update a sub fund
                test.Portal.Giving_SubFunds_Update(fundName, subFundName, "GL", "TSF--12345678901234567890", true, true, subFundNameUpdated, "GLM", "TSFM-123456789012345678901234567890123456789012345", false, false);
            }
            finally
            {
                base.SQL.Giving_SubFunds_Delete(15, fundName, subFundName);
                base.SQL.Giving_SubFunds_Delete(15, fundName, subFundNameUpdated);
                base.SQL.Giving_Funds_Delete(15, fundName);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }

		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Deletes a Sub Fund.")]
		public void Giving_Setup_SubFunds_Delete() {
            // Set initial conditions
            string fundName = "Test Fund - SF:D";
            string subFundName = "A Test Sub Fund - Delete";
            base.SQL.Giving_SubFunds_Delete(15, fundName, subFundName);
            base.SQL.Giving_Funds_Delete(15, fundName);
            base.SQL.Giving_Funds_Create(15, fundName, false, "SF:D", 1, true, "DC");
            base.SQL.Giving_SubFunds_Create(15, fundName, subFundName, "TSF-D", false, "GL", false);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                // Delete a sub fund
                test.Portal.Giving_SubFunds_Delete(subFundName);
            }
            finally
            {
                base.SQL.Giving_SubFunds_Delete(15, fundName, subFundName);
                base.SQL.Giving_Funds_Delete(15, fundName);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }

		}


        [Test, RepeatOnFailure, Timeout(1000)]
        [Author("Demi Zhang")]
        [Description("FO-4093 - Deletes a Sub Fund which is currently tied to one or more contribution schedules.")]
        public void Giving_Setup_SubFunds_NotAllow_Delete()
        {
            // Set initial conditions
            string fundName = "A Test Fund - SF:D";
            string subFundName = "A Test Sub Fund - Delete";
            base.SQL.Giving_SubFunds_Delete(15, fundName, subFundName);
            base.SQL.Giving_Funds_Delete(15, fundName);
            base.SQL.Giving_Funds_Create(15, fundName, true, "SF:U", 1, true, "DC");
            base.SQL.Giving_SubFunds_Create(15, fundName, subFundName, "TSF-D", true, "GL", true);

            string individulaName = "FT Tester";
            int individualId = base.SQL.People_Individuals_FetchID(15, individulaName);
            double amountCC = 10.01;

            //Updated by Jim
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            try
            {
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");
                
               
                // Store variables
                var startDay = now.Day.ToString();
                var startMonth = now.ToString("MMMM");

                
                // Create scheduled contribution
                test.Portal.Giving_ContributorDetails_Schedules_CreditCard_Monthly_WebDriver("FT Tester", fundName, subFundName, amountCC.ToString(), startDay, startMonth, now.AddYears(1).Year.ToString(), null, null,
                                                             "FT", "Tester", "Visa", "4111111111111111", "12 - December", now.AddYears(1).Year.ToString());
                
                // Navigate to giving->sub funds
                test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Setup.Sub_Funds);
                
                // Delete the sub fund
                int itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Portal.Giving_SubFunds, subFundName, "Sub Fund Name", null);
                test.Driver.FindElementById(TableIds.Portal.Giving_SubFunds).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[7].FindElement(By.TagName("a")).Click();

                // Click 'Yes' at the confirmation
                test.GeneralMethods.Popups_ConfirmationWebDriver("Yes");


                // Verify the dialog prompting the subfund cannot be deleted comes up
                var handler = test.Driver.CurrentWindowHandle;
                
                
                test.Driver.SwitchTo().Window("MessageBoxWindow");
                

               // string actualText = test.Driver.FindElementById("divMessage").Text.ToString();

                Assert.AreEqual(Regex.IsMatch(test.Driver.FindElementById("divMessage").Text.ToString(), "The Sub Fund " + "\"A Test Sub Fund - Delete\"" + " has one or more dependent items and therefore could not be deleted."), true);

                test.Driver.FindElementByXPath("*//body/*//input[@value='OK']").Click();
                                
                test.Driver.SwitchTo().Window(handler);
                // Verify sub fund was not deleted
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Portal.Giving_SubFunds, subFundName, "Sub Fund Name", null));

            }


            finally
            {
                base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(15, individualId, 10.01, 1);
                base.SQL.Giving_SubFunds_Delete(15, fundName, subFundName);
                base.SQL.Giving_Funds_Delete(15, fundName);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }

        }
		#endregion Sub Funds

		#region Pledge Drives
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Creates a pledge drive.")]
        public void Giving_Setup_PledgeDrives_Create() {
            // Set initial conditions
            string pledgeDriveName = "Test Pledge Drive";
            string fundName = "A Test Fund";
            string date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString(); //M/dd/yyyy
            int amount = 1000;
            base.SQL.Giving_PledgeDrives_Delete(15, pledgeDriveName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            try
            {
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                // Create a pledge drive
                test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Setup.Pledge_Drives);

                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtPledgeDriveName_textBox").SendKeys(pledgeDriveName);
                new SelectElement(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlFund_dropDownList")).SelectByText(fundName);
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctl01_DateTextBox").Clear();
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctl01_DateTextBox").SendKeys(date);
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctl02_DateTextBox").Clear();
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctl02_DateTextBox").SendKeys(date);
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_chkWebEnabled").Click();
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtPledgeDriveGoal_textBox").SendKeys(amount.ToString());

                test.Driver.FindElementById(GeneralButtons.Save).Click();

                //filter pledge name
                new SelectElement(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlFundFilter_dropDownList")).SelectByText(fundName);
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSearch").Click();
                test.GeneralMethods.WaitForPageIsLoaded(60);

                // Verify the pledge drive was created
                int itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Portal.Giving_PledgeDrives, pledgeDriveName, "Pledge Drive Name");
                test.GeneralMethods.VerifyTableDataWebDriver(TableIds.Portal.Giving_PledgeDrives, itemRow, new System.Collections.Generic.Dictionary<int, string>() { { 1, pledgeDriveName }, { 2, fundName }, { 3, "–" }, { 4, date }, { 5, date }, { 6, amount.ToString("c") } });

            }
            finally
            {
                base.SQL.Giving_PledgeDrives_Delete(15, pledgeDriveName);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }
        }
		#endregion Pledge Drives

		#region Contribution Attributes
		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Matthew Sneeden")]
		[Description("Creates a Contribution Attribute.")]
		public void Giving_Setup_ContributionAttributes_Create() {
            // Set initial conditions
            string contributionAttributeName = "Test Contribution Attribute";
            base.SQL.Giving_ContributionAttributes_Delete(15, contributionAttributeName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            try
            {
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                // Create a contribution attribute
                test.Portal.Giving_ContributionAttributes_Create(contributionAttributeName);
            }
            finally
            {
                base.SQL.Giving_ContributionAttributes_Delete(15, contributionAttributeName);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Updates a Contribution Attribute.")]
		public void Giving_Setup_ContributionAttributes_Update() {
            // Set initial conditions
            string contributionAttributeName = "Test Contribution Attribute - Update";
            string contributionAttributeNameUpdated = "Test Contribution Attribute - Mod";
            base.SQL.Giving_ContributionAttributes_Delete(15, contributionAttributeName);
            base.SQL.Giving_ContributionAttributes_Delete(15, contributionAttributeNameUpdated);
            base.SQL.Giving_ContributionAttributes_Create(15, contributionAttributeName, 65211);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            try
            {
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                // Update a contribution attribute
                test.Portal.Giving_ContributionAttributes_Update(contributionAttributeName, contributionAttributeNameUpdated);
            }
            finally
            {
                base.SQL.Giving_ContributionAttributes_Delete(15, contributionAttributeName);
                base.SQL.Giving_ContributionAttributes_Delete(15, contributionAttributeNameUpdated);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Deletes a Contribution Attribute.")]
		public void Giving_Setup_ContributionAttributes_Delete() {
            // Set initial conditions
            string contributionAttributeName = "Test Contribution Attribute - Delete";
            base.SQL.Giving_ContributionAttributes_Delete(15, contributionAttributeName);
            base.SQL.Giving_ContributionAttributes_Create(15, contributionAttributeName, 65211);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            try
            {
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                // Delete a contribution attribute
                test.Portal.Giving_ContributionAttributes_Delete(contributionAttributeName);
            }
            finally
            {
                base.SQL.Giving_ContributionAttributes_Delete(15, contributionAttributeName);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }
		}
		#endregion Contribution Attributes

        #region Sub Types
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Creates a Sub Type.")]
        public void Giving_Setup_SubTypes_Create() {
            // Set initial conditions
            string subTypeName = "Test Sub Type";
            base.SQL.Giving_SubTypes_Delete(15, subTypeName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            try
            {
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                // Create a sub type
                test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Setup.Sub_Types);
                new SelectElement(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlContributionType_dropDownList")).SelectByText("Cash");
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtContributionSubType_textBox").SendKeys(subTypeName);
                test.Driver.FindElementById(GeneralButtons.Save).Click();

                // Verify the sub type was created
                int itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Portal.Giving_SubTypes, subTypeName, "Contribution Sub Type");
                test.GeneralMethods.VerifyTableDataWebDriver(TableIds.Portal.Giving_SubTypes, itemRow, new System.Collections.Generic.Dictionary<int, string>() { { 1, subTypeName }, { 2, "Cash" } });
                Assert.AreEqual(true, test.Driver.FindElementById(TableIds.Portal.Giving_SubTypes).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[3].FindElements(By.TagName("img")).Count > 0);
            }
            finally
            {
                base.SQL.Giving_SubTypes_Delete(15, subTypeName);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }
        }
        #endregion Sub Types

        #region Account References
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Matthew Sneeden")]
		[Description("Creates an Account Reference.")]
		public void Giving_Setup_AccountReferences_Create() {
            // Set initial conditions
            string accountReferenceName = "Test Account Reference-New";
            base.SQL.Giving_AccountReferences_Delete(15, accountReferenceName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            try
            {
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                // Create an account reference
                test.Portal.Giving_AccountReferences_Create(accountReferenceName, "Test", null, "ftreseller", true);
                //test.Portal.Giving_AccountReferences_Create(accountReferenceName, "Test", null, "ftreseller-85", true);
            }
            finally
            {
                base.SQL.Giving_AccountReferences_Delete(15, accountReferenceName);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Updates an Account Reference.")]
		public void Giving_Setup_AccountReferences_Update() {
            // Set initial conditions
            string accountReferenceName = "Test Account Reference - U";
            string accountReferenceDescription = "Test - Update";
            string accountReferenceNameUpdated = "Test Account Reference - Mod";
            string accountReferenceDescriptionUpdated = "Test - Mod";
            base.SQL.Giving_AccountReferences_Delete(15, accountReferenceName);
            base.SQL.Giving_AccountReferences_Delete(15, accountReferenceNameUpdated);
            //base.SQL.Giving_AccountReferences_Create(15, accountReferenceName, accountReferenceDescription, null, "ftreseller-85", true);// "ftreseller-85" not exist in DB
            base.SQL.Giving_AccountReferences_Create(15, accountReferenceName, accountReferenceDescription, null, "ftreseller", true);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            try
            {
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                // Update an account reference
                test.Portal.Giving_AccountReferences_Update(accountReferenceName, accountReferenceDescription, null, "ftreseller", true, accountReferenceNameUpdated, accountReferenceDescriptionUpdated, "Mod", "ftone", false);

            }
            finally
            {
                base.SQL.Giving_AccountReferences_Delete(15, accountReferenceName);
                base.SQL.Giving_AccountReferences_Delete(15, accountReferenceNameUpdated);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }

		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Deletes an Account Reference.")]
		public void Giving_Setup_AccountReferences_Delete() {
            // Set initial conditions
            string accountReferenceName = "Test Account Reference - D";
            base.SQL.Giving_AccountReferences_Delete(15, accountReferenceName);
            base.SQL.Giving_AccountReferences_Create(15, accountReferenceName, "Test - Mod", "Mod", "ftone", false);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            try
            {
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                // Delete an account reference
                test.Portal.Giving_AccountReferences_Delete(accountReferenceName);
            }
            finally
            {
                base.SQL.Giving_AccountReferences_Delete(15, accountReferenceName);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }

		}
		#endregion Account References

		#region Organizations
		#endregion Organizations
    }
}