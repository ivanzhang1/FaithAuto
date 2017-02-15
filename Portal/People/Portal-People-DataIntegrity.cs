using System;
using System.Collections;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;

namespace FTTests.Portal.People {
    [TestFixture]
    public class Portal_People_DataIntegrity : FixtureBaseWebDriver {

        #region Duplicate Finder

        [Test, RepeatOnFailure]
        [Author("Ivan Zhang")]
        [Description("Verifies the dropdown list works fine when the dropdown list's value has noncharacter.")]
        public void People_DataIntegrity_DuplicateFinder_StatusDropDownList()
        {

            #region variables
            string tempStatus = "d!@#$%^&*()\'+/.\\;\"[";
            string origin_Status;
            string changed_Status;
            #endregion

            #region Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("****Try to login the portal *****");
            test.Portal.LoginWebDriver("izhang2", "123qwe!@#QWE", "DC");
            #endregion

            #region Insert specific value to DB
            //we should create the new status and sub status, the value of status& substatus has the non-characters
            //key point, add the non-character.
            test.SQL.Admin_Status_Delete_SpecCharater(15, tempStatus);
            test.SQL.Admin_Status_Create_SpecCharater(15, "Attendee", tempStatus, true);
            #endregion

            try
            {
                #region Search with Status
                // Navigate to people->Duplicate finder
                if (!test.Driver.Url.Contains("DuplicateFinder"))
                {
                    test.GeneralMethods.Navigate_Portal(Navigation.People.Data_Integrity.Duplicate_Finder);
                    TestLog.WriteLine("Duplicate Finder is opened.");
                }

                //click the  status hyperlink
                test.GeneralMethods.WaitForElement(test.Driver, OpenQA.Selenium.By.LinkText("Status"));
                test.Driver.FindElementByLinkText("Status").Click();
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnFindPotentialDuplicates").Click();
                TestLog.WriteLine("click search btn");
                #endregion

                #region Check Dropdown List
                try
                {
                    //status value will be changed with status group's value

                    //find the status group, status, substatus
                    IWebElement tempObject_StatusGroup = test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlFilterStatusGroup");
                    IWebElement tempObject_Status = test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlFilterStatus");
                    IWebElement tempObject_SubStatus = test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlFilterSubStatus");

                    #region Get origin status value
                    SelectElement tempSelect_Status = new SelectElement(tempObject_Status);
                    origin_Status = tempSelect_Status.SelectedOption.Text;
                    TestLog.WriteLine("**origin_Status is " + origin_Status);
                    #endregion

                    #region Get changed status value
                    //change status group
                    SelectElement tempSelect_StatusGroup = new SelectElement(tempObject_StatusGroup);
                    tempSelect_StatusGroup.SelectByText("System");
                    //click search btn
                    test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnFindPotentialDuplicates").Click();
                    //waiting for the result return
                    test.GeneralMethods.WaitForElement(test.Driver, OpenQA.Selenium.By.Id("ctl00_ctl00_MainContent_content_dgIndividualResults"));

                    //Get changes status value from UI
                    tempObject_StatusGroup = test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlFilterStatusGroup");
                    tempObject_Status = test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlFilterStatus");
                    tempSelect_StatusGroup = new SelectElement(tempObject_StatusGroup);
                    tempSelect_Status = new SelectElement(tempObject_Status);

                    changed_Status = tempSelect_Status.SelectedOption.Text;
                    TestLog.WriteLine("**changed_Status is " + changed_Status);
                    #endregion

                    //Verify changes status value is diff with origin value when we changed the status group
                    Assert.AreNotEqual(origin_Status, changed_Status);

                }
                catch (Exception e)
                {
                    TestLog.WriteLine(e.Message.ToString());
                }
                #endregion

            }
            finally
            {
                test.SQL.Admin_Status_Delete_SpecCharater(15, tempStatus);
            }
        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("FO-4059 INC2297617: People › Data Integrity › Duplicate Finder is throwing errors (LCOKCOK)")]
        public void People_DataIntegrity_DuplicateFinder_SearchAttendeeWell()
        {

            #region Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login the portal");
            test.Portal.LoginWebDriver("madykou", "Active@123", "DC");
            #endregion

            // Navigate to people->Duplicate finder
            if (!test.Driver.Url.Contains("DuplicateFinder"))
            {
                TestLog.WriteLine("Open Duplicate Finder page");
                test.GeneralMethods.Navigate_Portal(Navigation.People.Data_Integrity.Duplicate_Finder);
            }

            //click the  status hyperlink
            test.GeneralMethods.WaitForElement(test.Driver, OpenQA.Selenium.By.LinkText("Status"));
            TestLog.WriteLine("Click Status option on the page");
            test.Driver.FindElementByLinkText("Status").Click();

            TestLog.WriteLine("Select Duplicate Finder Status group and Status value");
            new SelectElement(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlFilterStatusGroup")).SelectByText("Attendee");
            new SelectElement(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlFilterStatus")).SelectByText("Attendee");

            TestLog.WriteLine("click search btn");
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnFindPotentialDuplicates").Click();

            TestLog.WriteLine("Check search result");
            //waiting for the result return
            test.GeneralMethods.WaitForElement(test.Driver, OpenQA.Selenium.By.Id("ctl00_ctl00_MainContent_content_dgIndividualResults"));
            test.GeneralMethods.WaitForElement(test.Driver, OpenQA.Selenium.By.Id("ctl00_ctl00_MainContent_content_dgIndividualResults_ctl02_lnkMatch"));
        }

        #endregion Duplicate Finder

        #region Merge Individual
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Merges an individual through portal's UI")]
        public void People_DataIntegrity_MergeIndividual() {
            // Create a duplicate individual in the database
            base.SQL.People_Individual_Create(15, "FT", "Duplicate");

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a merge through Portal
            test.Portal.People_MergeIndividual("FT Merge", "FT Duplicate");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a merge where the Master record has no HH phone, the Duplicate does, and none is selected.")]
        public void People_DataIntegrity_MergeIndividual_HH_Phone_NotSelected() {
            // Create a duplicate individual in the database with HH phone
            base.SQL.People_Individual_Create(15, "FT", "Visitor2");
            base.SQL.People_CommunicationValue_Add(15, "FT Visitor2", GeneralEnumerations.CommunicationTypes.Home, "444-444-4444", false);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Merge individuals without retaining HH phone
            test.Portal.People_MergeIndividual("FT Visitor", "FT Visitor2", "FT Tester", "1", true);

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a merge where the Master record and Duplicate record have no HH phone, and one is manually added during merge.")]
        public void People_DataIntegrity_MergeIndividual_HH_Phone_Added() {
            // Create a duplicate individual in the database
            base.SQL.People_Individual_Create(15, "FT", "Visitor3");

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Merge individuals with a new HH phone added
            test.Portal.People_MergeIndividual("FT Visitor", "FT Visitor3", "FT Tester", "1", true, "123-456-7890");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Merge Individual

        #region Move Individual
        #endregion Move Individual

        #region Split Household
        #endregion Split Household

        #region Duplicate Queue
        #endregion Duplicate Queue

        #region Mass Action Queue
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Performs a Mass Action to add an attribute via a people query.")]
        public void People_DataIntegrity_MassActionAttribute_PeopleQuery() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // People query
            test.Portal.People_PeopleQuery(SearchByConstants.Household_Address, FieldConstants.Address, ComparisonConstants.Equal_To, new string[] { "9616 Armour Dr" });

            // Perform a mass action
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            Hashtable options = new Hashtable() { { "Attribute Group", "Allergies - Food" }, { "Attribute", "Chocolate" }, { "Staff", "Allen, Kingsley" }, { "Start Date", now.ToString("M/d/yyyy") }, { "Comment", "Comment" } };
            int daysDiffMatthew = SqlMethods.DateDiffDay(new DateTime(1982, 9, 2), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")));
            int daysDiffLaura = SqlMethods.DateDiffDay(new DateTime(1982, 1, 23), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")));

            test.Portal.People_MassAction(new string[] { string.Format("1/23/1982 ({0} yrs.)", (daysDiffLaura - ((daysDiffLaura / 365) / 4)) / 365), string.Format("9/2/1982 ({0} yrs.)", (daysDiffMatthew - ((daysDiffMatthew / 365) / 4)) / 365)}, "Add an attribute", "Test Mass Action", options);

            // Verify the attributes were added
            test.Portal.People_ViewIndividual_WebDriver("Matthew Sneeden");
            test.Driver.FindElementByLinkText("Allergies - Food").Click();
            System.Threading.Thread.Sleep(500);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Chocolate"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Comment"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(string.Format("Starts: {0}", now.ToString("MM/dd/yyyy"))));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Staff/Pastor: Kingsley Allen"));

            //Delete the attribute
            test.Driver.FindElementById("ind_attributes").FindElement(By.TagName("div")).FindElement(By.TagName("ul")).FindElement(By.TagName("li")).FindElement(By.TagName("form")).FindElement(By.TagName("a")).Click();
            //test.Driver.FindElementByClassName("Delete").Click();
            test.Driver.SwitchTo().Alert().Accept();
            

            test.Portal.People_ViewIndividual_WebDriver("Laura Hause");
            test.Driver.FindElementByLinkText("Allergies - Food").Click();
            System.Threading.Thread.Sleep(500);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Chocolate"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Comment"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(string.Format("Starts: {0}", now.ToString("MM/dd/yyyy"))));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Staff/Pastor: Kingsley Allen"));

            //Delete the attribute
            test.Driver.FindElementById("ind_attributes").FindElement(By.TagName("div")).FindElement(By.TagName("ul")).FindElement(By.TagName("li")).FindElement(By.TagName("form")).FindElement(By.TagName("a")).Click();
            //test.Driver.FindElementByClassName("Delete").Click();
            test.Driver.SwitchTo().Alert().Accept();

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Mass Action Queue


        [Test, RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("FO-3026:Duplicates will not merge in duplicate finder")]
        public void People_DataIntegrity_DuplicateFinderAbnormalIndividual()
        {
            String individualName = "Joseph Moore";
            String emailValue = "jmoore@fellowshiptech.com";
            #region Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("****Try to login the portal *****");
            test.Portal.LoginWebDriver("izhang2", "123qwe!@#QWE", "DC");
            #endregion
            // Navigate to people->Duplicate finder
            if (!test.Driver.Url.Contains("DuplicateFinder"))
            {
                test.GeneralMethods.Navigate_Portal(Navigation.People.Data_Integrity.Duplicate_Finder);
                TestLog.WriteLine("Duplicate Finder is opened.");
            }

            //update DB: "Test2 Testerson" to abnoamal individual DB
            test.SQL.InfellowShipIndividual_data_Update(15, emailValue, "2015-07-07 00:00:00.000");

            try
            {
                test.Driver.FindElementByName("ctl00$ctl00$MainContent$content$txtFindPerson").Clear();
                test.Driver.FindElementByName("ctl00$ctl00$MainContent$content$txtFindPerson").SendKeys(individualName);

                test.Driver.FindElementByName("ctl00$ctl00$MainContent$content$btnFindPerson").Click();


                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgIndividualResults").FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"))[1].FindElement(By.Id("ctl00_ctl00_MainContent_content_dgIndividualResults_ctl02_lnkMatch")).Click();

                //Verify Selected Individual's Name is not null
                Assert.AreEqual(individualName, test.Driver.FindElementById("ctl00_ctl00_MainContent_content_lblMasterRecordFullName").Text.ToString());
            }
            finally
            {

                //restore enviroment
                test.SQL.InfellowShipIndividual_data_Update(15, emailValue, null);
            }
            
        }

    }
}