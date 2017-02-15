using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using OpenQA.Selenium;

namespace FTTests.Portal.People {
    [TestFixture]
    public class Portal_People_Search_AddHousehold : FixtureBaseWebDriver {

        #region New Household
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Creates a household with an international address.")]
        public void People_Search_AddHousehold_InternationalAddress() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Add a household
            string household = "Automated Test-International";
            test.Portal.People_AddHousehold("Automated", "Test-International", HouseholdPositionConstants.Head, "Member", null, new PortalBase.AddressData { Country = "Canada", Address_1 = "#35 250 Satok Crescent", City = "Milton", Province = "Ontario", Postal_code = "L9T 3P4" });

            // Merge the created individual
            base.SQL.People_MergeIndividual(15, household, "Merge Dump");
          
            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Creates a household in an international church.")]
        public void People_Search_AddHousehold_InternationalChurch() {
            // Set initial conditions
            string household = "Automated Test-InternationalChurch";
            base.SQL.People_MergeIndividual(258, household, "Merge Dump");

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C6");

            // Add a household
            test.Portal.People_AddHousehold("Automated", "Test-InternationalChurch", HouseholdPositionConstants.Head, "Member", null, new PortalBase.AddressData { Address_1 = "123 Main", City = "London", Province = "England", Postal_code = "W11 2BQ" });

            // Merge the created individual
            test.Portal.People_MergeIndividual("Merge Dump", household);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Creates a household with communication values.")]
        public void People_Search_AddHousehold_Communications() {
            // Set initial conditions
            string household = "Automated Communications";
            base.SQL.People_MergeIndividual(15, household, "Merge Dump");

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            string numOfDuplicates = test.Portal.People_Duplicate_Finder_Merge(household);
            test.Portal.People_MergeIndividual("Merge Dump", household, "Matthew Sneeden", numOfDuplicates);

            // Prepare the communication data
            List<List<string>> commData = new List<List<string>>();
            commData.Add(new List<string>());
            commData[0].Add("Phone");
            commData[0].Add("Home");
            commData[0].Add("8479520499");
            commData.Add(new List<string>());
            commData[1].Add("Email");
            commData[1].Add("Email");
            commData[1].Add("msneeden@fellowshiptech.com");

            // Add a household
            test.Portal.People_AddHousehold("Automated", "Communications", HouseholdPositionConstants.Head, "Active Member", commData, null);

            // View the individual
            test.Portal.People_ViewIndividual_WebDriver(household);

            // View the communications
            test.Driver.FindElementByXPath(Portal.People.SearchConstants.Edit_communications).Click();

            // Verify the updated dates
            string timeFormatted = string.Format("{0:dd MMM yyyy}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")));
            Assert.AreEqual(timeFormatted, test.Driver.FindElementByXPath("//table[@class='grid full'][1]/tbody/tr[3]/td[6]").Text);
            Assert.AreEqual(timeFormatted, test.Driver.FindElementByXPath("//table[@class='grid full'][2]/tbody/tr[2]/td[6]").Text);

            // Merge the created individual
            base.SQL.People_MergeIndividual(15, household, "Merge Dump");
            numOfDuplicates = test.Portal.People_Duplicate_Finder_Merge(household);
            test.Portal.People_MergeIndividual("Merge Dump", household, "Matthew Sneeden", numOfDuplicates);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure, Timeout(8000)]
        [Author("Bryan Mikaelian")]
        [Description("Creates a household with communication values and specify the home phone to be preferred.")]
        public void People_Search_AddHousehold_Communications_PreferredPhone_Home() {
            // Set initial conditions
            string household = "Home Preferred";
            base.SQL.People_MergeIndividual(15, household, "Merge Dump");

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            //Merge what we have
            string numOfDuplicates = test.Portal.People_Duplicate_Finder_Merge(household);
            test.Portal.People_MergeIndividual("Merge Dump", household, "Matthew Sneeden", numOfDuplicates);


            // Prepare the communication data
            List<List<string>> commData = new List<List<string>>();
            commData.Add(new List<string>());
            commData[0].Add("Phone");
            commData[0].Add("Home");
            commData[0].Add("8479520499");
            commData.Add(new List<string>());
            commData[1].Add("Email");
            commData[1].Add("Email");
            commData[1].Add("msneeden@fellowshiptech.com");

            // Add a household
            test.Portal.People_AddHousehold("Home", "Preferred", HouseholdPositionConstants.Head, "Active Member", commData, null, null, null, GeneralEnumerations.CommunicationTypes.Home);

            // View the individual
            test.Portal.People_ViewIndividual_WebDriver(household);

            // View the communications
            test.Driver.FindElementByXPath(Portal.People.SearchConstants.Edit_communications).Click();

            // Verify the updated dates
            string timeFormatted = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("dd MMM yyyy");
            Assert.AreEqual(timeFormatted, test.Driver.FindElementByXPath("//table[@class='grid full'][1]/tbody/tr[3]/td[6]").Text);
            Assert.AreEqual(timeFormatted, test.Driver.FindElementByXPath("//table[@class='grid full'][2]/tbody/tr[2]/td[6]").Text);

            // Verify the home phone is marked as preferred.
            Assert.IsTrue(test.Driver.FindElementByXPath("//input[@name='phone_priority' and @value='home']").Selected, "Home phone was marked as preferred but value was not saved.");

            // Merge the created individual
            base.SQL.People_MergeIndividual(15, household, "Merge Dump");
            numOfDuplicates = test.Portal.People_Duplicate_Finder_Merge(household);
            test.Portal.People_MergeIndividual("Merge Dump", household, "Matthew Sneeden", numOfDuplicates);

            // Logout of portal
            test.Portal.LogoutWebDriver();


        }

        [Test, RepeatOnFailure, Timeout(8000)]
        [Author("Bryan Mikaelian")]
        [Description("Creates a household with communication values and specify the mobile phone to be preferred.")]
        public void People_Search_AddHousehold_Communications_PreferredPhone_Mobile() {
            // Set initial conditions
            string household = "Mobile Preferred";
            base.SQL.People_MergeIndividual(15, household, "Merge Dump");
            
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            string numOfDuplicates = test.Portal.People_Duplicate_Finder_Merge(household);
            test.Portal.People_MergeIndividual("Merge Dump", household, "Matthew Sneeden", numOfDuplicates);

            // Prepare the communication data
            List<List<string>> commData = new List<List<string>>();
            commData.Add(new List<string>());
            commData[0].Add("Phone");
            commData[0].Add("Mobile");
            commData[0].Add("8479520499");
            commData.Add(new List<string>());
            commData[1].Add("Email");
            commData[1].Add("Email");
            commData[1].Add("msneeden@fellowshiptech.com");

            // Add a household
            test.Portal.People_AddHousehold("Mobile", "Preferred", HouseholdPositionConstants.Head, "Active Member", commData, null, null, null, GeneralEnumerations.CommunicationTypes.Mobile);

            // View the individual
            test.Portal.People_ViewIndividual_WebDriver(household);

            // View the communications
            test.Driver.FindElementByXPath(Portal.People.SearchConstants.Edit_communications).Click();

            // Verify the updated dates
            string timeFormatted = string.Format("{0:dd MMM yyyy}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")));
            Assert.AreEqual(timeFormatted, test.Driver.FindElementByXPath("//table[@class='grid full'][1]/tbody/tr[2]/td[6]").Text);
            Assert.AreEqual(timeFormatted, test.Driver.FindElementByXPath("//table[@class='grid full'][2]/tbody/tr[2]/td[6]").Text);

            // Verify the mobile phone is marked as preferred.
            Assert.IsTrue(test.Driver.FindElementByXPath("//input[@name='phone_priority' and @value='mobile']").Selected, "Mobile phone was marked as preferred but value was not saved.");

            // Merge the created individual
            base.SQL.People_MergeIndividual(15, household, "Merge Dump");
            numOfDuplicates = test.Portal.People_Duplicate_Finder_Merge(household);
            test.Portal.People_MergeIndividual("Merge Dump", household, "Matthew Sneeden", numOfDuplicates);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure, Timeout(8000)]
        [Author("Bryan Mikaelian")]
        [Description("Creates a household with communication values and specify the work phone to be preferred.")]
        public void People_Search_AddHousehold_Communications_PreferredPhone_Work() {
            // Set initial conditions
            string household = "Work Preferred";
            base.SQL.People_MergeIndividual(15, household, "Merge Dump");
            
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            string numOfDuplicates = test.Portal.People_Duplicate_Finder_Merge(household);
            test.Portal.People_MergeIndividual("Merge Dump", household, "Matthew Sneeden", numOfDuplicates);

            // Prepare the communication data
            List<List<string>> commData = new List<List<string>>();
            commData.Add(new List<string>());
            commData[0].Add("Phone");
            commData[0].Add("Work");
            commData[0].Add("8479520499");
            commData.Add(new List<string>());
            commData[1].Add("Email");
            commData[1].Add("Email");
            commData[1].Add("msneeden@fellowshiptech.com");

            // Add a household
            test.Portal.People_AddHousehold("Work", "Preferred", HouseholdPositionConstants.Head, "Active Member", commData, null, null, null, GeneralEnumerations.CommunicationTypes.Work);

            // View the individual
            test.Portal.People_ViewIndividual_WebDriver(household);

            // View the communications
            test.Driver.FindElementByXPath(Portal.People.SearchConstants.Edit_communications).Click();

            // Verify the updated dates
            string timeFormatted = string.Format("{0:dd MMM yyyy}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")));
            Assert.AreEqual(timeFormatted, test.Driver.FindElementByXPath("//table[@class='grid full'][1]/tbody/tr[4]/td[6]").Text);
            Assert.AreEqual(timeFormatted, test.Driver.FindElementByXPath("//table[@class='grid full'][2]/tbody/tr[2]/td[6]").Text);

            // Verify the mobile phone is marked as preferred.
            Assert.IsTrue(test.Driver.FindElementByXPath("//input[@name='phone_priority' and @value='work']").Selected, "Work phone was marked as preferred but value was not saved.");

            // Merge the created individual
            base.SQL.People_MergeIndividual(15, household, "Merge Dump");
            numOfDuplicates = test.Portal.People_Duplicate_Finder_Merge(household);
            test.Portal.People_MergeIndividual("Merge Dump", household, "Matthew Sneeden", numOfDuplicates);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure, Timeout(1000)]
        [Author("Matthew Sneeden")]
        [Description("Creates a household without creating an address.")]
        public void People_Search_AddHousehold_Without_Address() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Add a household
            string household = "Automated Test-NoAddress";
            test.Portal.People_AddHousehold("Automated", "Test-NoAddress", HouseholdPositionConstants.Head, "Active Member", null, null);

            // Merge the created individual
            base.SQL.People_MergeIndividual(15, household, "Merge Dump");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure, Timeout(1000)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Creates a household with an address.")]
        public void People_Search_AddHousehold_With_Address() {
            // Set initial conditions
            string household = "Automated Test";
            base.SQL.People_MergeIndividual(15, household, "Merge Dump");

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Add a household
            test.Portal.People_AddHousehold("Automated", "Test", HouseholdPositionConstants.Head, "Active Member", null, new PortalBase.AddressData { Address_1 = "1704 S Milbrook Ln", Postal_code = "60005" });

            // Merge the created individual
            test.Portal.People_MergeIndividual("Merge Dump", household);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create a household without any of the required data.")]
        public void People_Search_AddHousehold_NoData() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Attempt to create the household
            test.Portal.People_AddHousehold(null, null, null, null, null, null);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create a household without providing a last name.")]
        public void People_Search_AddHousehold_MissingLastName() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Attempt to create the household
            test.Portal.People_AddHousehold("Matthew", null, HouseholdPositionConstants.Head, "Member", null, null);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create a household without providing a first name.")]
        public void People_Search_AddHousehold_MissingFirstName() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Attempt to create the household
            test.Portal.People_AddHousehold(null, "Sneeden", HouseholdPositionConstants.Head, "Member", null, null);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create a household without providing a household position.")]
        public void People_Search_AddHousehold_MissingHouseholdPosition() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Attempt to create the household
            test.Portal.People_AddHousehold("Matthew", "Sneeden", null, "Member", null, null);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create a household without providing a status.")]
        public void People_Search_AddHousehold_MissingStatus() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Attempt to create the household
            test.Portal.People_AddHousehold("Matthew", "Sneeden", HouseholdPositionConstants.Head, null, null, null);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion New Household
    }
}