using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using System.Collections.Generic;
using OpenQA.Selenium;

namespace FTTests.Portal.People {
    public static class PeopleHeadingText {
        // Search
        public const string Search_FindAPerson = "Find a Person";
        public const string Search_PeopleQuery = "Query Builder";
        public const string Search_MySavedQueries = "My Saved Queries";
        public const string Search_AddHousehold = "Create a New Household";
        public const string Search_EditAnIndividual = "Edit an Individual";
        public const string Search_IndividualDetail = "Individual Detail";
        public const string Search_ViewIndividual_AddAnIndividual = "Add an Individual";
        public const string Search_ViewIndividual_ViewHousehold = "Household View";
        public const string Search_ViewIndividual_ViewHousehold_AddAnIndividual = "Add an Individual";
        public const string Search_ViewIndividual_ViewHousehold_EnterAContactForm = "Select a Contact Form";
        public const string Search_ViewIndividual_ViewHousehold_EditCommunications = "Household Communications";
        public const string Search_ViewIndividual_ViewHousehold_AddAnother = "Household - New Address";
        public const string Search_ViewIndividual_ViewHousehold_EditThisAddress = "Household - Edit Address";
        public const string Search_ViewIndividual_ViewHousehold_ContactItems = "Contacts";

        public const string Search_ViewIndividual_ViewHousehold_Note = "Notes";
        public const string Search_ViewIndividual_ViewHousehold_EditHousehold = "Edit a Household";

        // Volunteer Pipeline
        public const string VolunteerPipeline_SubmitApplication = "Submit Application";
        public const string VolunteerPipeline_ReviewApplications = "Review Applications";
        public const string VolunteerPipeline_MinistryReview = "Ministry Review";
        public const string VolunteerPipeline_VerifyRequirements = "Verify Requirements";
        public const string VolunteerPipeline_BackgroundChecks = "Background Checks";
        public const string VolunteerPipeline_BackgroundChecks_Add = "Submit Background Check Request";
        public const string VolunteerPipeline_ApprovedVolunteers = "Approved Volunteers";
        public const string VolunteerPipeline_RejectedVolunteers = "Rejected Volunteers";

        // Data Integrity
        public const string DataIntegrity_DuplicateFinder = "Duplicate Finder";
        public const string DataIntegrity_MergeIndividual = "Merge Individual";
        public const string DataIntegrity_MoveIndividual = "Move Individual to Household";
        public const string DataIntegrity_SplitHousehold = "Split Household";
        public const string DataIntegrity_DuplicateQueue = "Duplicate Queue";
        public const string DataIntegrity_MassActionQueue = "Mass Action Queue";

        public static string TitleFormat(string text) {
            return string.Format("Fellowship One :: {0}", text);
        }
    }

    [TestFixture]
    [Importance(Importance.Critical)]
    public class Portal_People_WebDriver : FixtureBaseWebDriver {

        #region Search
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies a user can access the saved query details for a saved query.")]
        public void People_Search_MySavedQueries_SavedQueriesDetails() {

            string savedQuery = "Test Saved Query";
            
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to the My Saved Queries page
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.People.Search.My_Saved_Queries);
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText(savedQuery));

            // Click on a saved query
            test.Driver.FindElementByLinkText(savedQuery).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Edit settings"));

            // Verify page loaded
            test.GeneralMethods.VerifyTextPresentWebDriver(savedQuery);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        #endregion Search
    }

    [TestFixture]
    [Importance(Importance.Critical)]
    public class Portal_People : FixtureBase {

        [Test, RepeatOnFailure, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Verifies the menu items, pages under the people section.")]
        public void People() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Verify the links present
            Assert.AreEqual("Search", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[1]/dt"));
            Assert.AreEqual("Find a Person", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[1]/dd[1]/a"));
            Assert.AreEqual("People Query", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[1]/dd[2]/a"));
            Assert.AreEqual("My Saved Queries", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[1]/dd[3]/a"));
            Assert.AreEqual("Add Household", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[1]/dd[4]/a"));

            Assert.AreEqual("Group Email", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[2]/dt"));
            Assert.AreEqual("Compose", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[2]/dd[1]/a"));
            Assert.AreEqual("Drafts", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[2]/dd[2]/a"));
            Assert.AreEqual("Sent Items", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[2]/dd[3]/a"));
            Assert.AreEqual("Templates", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[2]/dd[4]/a"));
            Assert.AreEqual("Delegates", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[2]/dd[5]/a"));

            Assert.AreEqual("Volunteer Pipeline", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[3]/dt"));
            Assert.AreEqual("Submit Application", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[3]/dd[1]/a"));
            Assert.AreEqual("Review Applications", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[3]/dd[2]/a"));
            Assert.AreEqual("Ministry Review", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[3]/dd[3]/a"));
            Assert.AreEqual("Verify Requirements", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[3]/dd[4]/a"));
            Assert.AreEqual("Background Checks", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[3]/dd[5]/a"));
            Assert.AreEqual("Approved Volunteers", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[3]/dd[6]/a"));
            Assert.AreEqual("Rejected Volunteers", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[3]/dd[7]/a"));

            Assert.AreEqual("Data Integrity", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[4]/dt"));
            Assert.AreEqual("Duplicate Finder", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[4]/dd[1]/a"));
            Assert.AreEqual("Merge Individual", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[4]/dd[2]/a"));
            Assert.AreEqual("Move Individual", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[4]/dd[3]/a"));
            Assert.AreEqual("Split Household", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[4]/dd[4]/a"));
            Assert.AreEqual("Duplicate Queue", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[4]/dd[5]/a"));
            Assert.AreEqual("Mass Action Queue", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[4]/dd[6]/a"));

            Assert.AreEqual("Counseling/Mentoring", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[5]/dt"));
            Assert.AreEqual("View Relationships", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[5]/dd[1]/a"));

            // Logout of portal
            test.Portal.Logout();
        }

        #region Search
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the find a person page under people.")]
        public void People_Search_FindAPerson() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->find a person
            test.Selenium.Navigate(Navigation.People.Search.Find_a_Person);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.Search_FindAPerson));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.Search_FindAPerson);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the people query page under people.")]
        public void People_Search_PeopleQuery() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->people query
            test.Selenium.Navigate(Navigation.Portal.People.Search.People_Query);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.Search_PeopleQuery));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.Search_PeopleQuery);

            // Verify button text
            Assert.AreEqual("Add to statement builder", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_btnAddToStmt"));

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the my saved queries page under people.")]
        public void People_Search_MySavedQuerires() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->my saved queries
            test.Selenium.Navigate(Navigation.Portal.People.Search.My_Saved_Queries);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.Search_MySavedQueries));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.Search_MySavedQueries);

            // Logout of portal
            test.Portal.Logout();
        }

        #region Add Household
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the people search page under people (via the Add Household link).")]
        public void People_Search_AddHousehold() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->people search
            test.Selenium.Navigate(Navigation.Portal.People.Search.Add_Household);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.Search_FindAPerson));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.Search_FindAPerson);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the add household page under people.")]
        public void People_Search_AddHousehold_AddHousehold() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->people search
            test.Selenium.Navigate(Navigation.Portal.People.Search.Add_Household);

            // View the add household page
            test.Selenium.ClickAndWaitForPageToLoad("link=Add a household");

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.Search_AddHousehold));
            test.Selenium.VerifyTextPresent("Add New Household");

            // Verify the available selections in the marital status drop down
            Assert.AreEqual(8, test.Selenium.GetXpathCount("//select[contains(@id, 'ctl00_ctl00_MainContent_content_ctl01_ddlMaritalStatus_marital_status')]/option"));
            Assert.AreEqual("", test.Selenium.GetValue("//select[contains(@id, 'ctl00_ctl00_MainContent_content_ctl01_ddlMaritalStatus_marital_status')]/option[1]"));
            Assert.AreEqual("Child/Yth", test.Selenium.GetValue("//select[contains(@id, 'ctl00_ctl00_MainContent_content_ctl01_ddlMaritalStatus_marital_status')]/option[2]"));
            Assert.AreEqual("Divorced", test.Selenium.GetValue("//select[contains(@id, 'ctl00_ctl00_MainContent_content_ctl01_ddlMaritalStatus_marital_status')]/option[3]"));
            Assert.AreEqual("Married", test.Selenium.GetValue("//select[contains(@id, 'ctl00_ctl00_MainContent_content_ctl01_ddlMaritalStatus_marital_status')]/option[4]"));
            Assert.AreEqual("Separated", test.Selenium.GetValue("//select[contains(@id, 'ctl00_ctl00_MainContent_content_ctl01_ddlMaritalStatus_marital_status')]/option[5]"));
            Assert.AreEqual("Single", test.Selenium.GetValue("//select[contains(@id, 'ctl00_ctl00_MainContent_content_ctl01_ddlMaritalStatus_marital_status')]/option[6]"));
            Assert.AreEqual("Widow", test.Selenium.GetValue("//select[contains(@id, 'ctl00_ctl00_MainContent_content_ctl01_ddlMaritalStatus_marital_status')]/option[7]"));
            Assert.AreEqual("Widower", test.Selenium.GetValue("//select[contains(@id, 'ctl00_ctl00_MainContent_content_ctl01_ddlMaritalStatus_marital_status')]/option[8]"));

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Add Household

        #endregion Search

        #region Group Email
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the compose page under people.")]
        public void People_GroupEmail_Compose() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->compose
            test.Selenium.Navigate(Navigation.People.GroupEmail.Compose);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Compose");
            test.Selenium.VerifyTextPresent("Compose");

            // Logout of portal
            test.Portal.Logout();
        }
        
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the drafts page under people.")]
        public void People_GroupEmail_Drafts() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->drafts
            test.Selenium.Navigate(Navigation.People.GroupEmail.Drafts);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Drafts");
            test.Selenium.VerifyTextPresent("Drafts");

            // Verify text is present alerting users when drafts are automatically deleted
            test.Selenium.VerifyTextPresent("Drafts are automatically deleted after 90 days.");


            // Navigate to people->drafts
            test.Selenium.Navigate(Navigation.People.GroupEmail.Drafts);

            // Create a new draft
            string target = test.Selenium.IsElementPresent(GeneralLinks.Add) ? GeneralLinks.Add : "link=Create your first email";
            test.Selenium.ClickAndWaitForPageToLoad(target);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Compose");
            test.Selenium.VerifyTextPresent("Compose");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the sent items page under people.")]
        public void People_GroupEmail_SentItems() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->sent items
            test.Selenium.Navigate(Navigation.People.GroupEmail.Sent_Items);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Sent Items");
            test.Selenium.VerifyTextPresent("Sent Items");

            // Logout of portal
            test.Portal.Logout();
        }
        
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the templates page under people.")]
        public void People_GroupEmail_Templates() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->templates
            test.Selenium.Navigate(Navigation.People.GroupEmail.Templates);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Templates");
            test.Selenium.VerifyTextPresent("Templates");


            // Navigate to people->templates
            test.Selenium.Navigate(Navigation.People.GroupEmail.Templates);

            // Add a new template
            string target = test.Selenium.IsElementPresent(GeneralLinks.Add) ? GeneralLinks.Add : "link=Create the first template";
            test.Selenium.ClickAndWaitForPageToLoad(target);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Create a new Email Template");
            test.Selenium.VerifyTextPresent("Email Template — New");

            // Logout of portal
            test.Portal.Logout();
        }
        
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the delegates page under people.")]
        public void People_GroupEmail_Delegates() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->delegates
            test.Selenium.Navigate(Navigation.People.GroupEmail.Delegates);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Delegates");
            test.Selenium.VerifyTextPresent("Delegates");


            // Click to create a delegate
            test.Selenium.Click(GeneralLinks.Add);

            // Verify title, text
            test.Selenium.VerifyTextPresent("Add a delegate");
            test.Selenium.Click(GeneralLinks.Cancel);

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Group Email
        
        #region Volunteer Pipeline
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the submit application page under people.")]
        public void People_VolunteerPipeline_SubmitApplication() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->submit application
            test.Selenium.Navigate(Navigation.People.Volunteer_Pipeline.Submit_Application);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.VolunteerPipeline_SubmitApplication));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.VolunteerPipeline_SubmitApplication);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the review applications page under people.")]
        public void People_VolunteerPipeline_ReviewApplications() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->review applications
            test.Selenium.Navigate(Navigation.People.Volunteer_Pipeline.Review_Applications);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.VolunteerPipeline_ReviewApplications));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.VolunteerPipeline_ReviewApplications);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the ministry review page under people.")]
        public void People_VolunteerPipeline_MinistryReview() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->ministry review
            test.Selenium.Navigate(Navigation.People.Volunteer_Pipeline.Ministry_Review);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.VolunteerPipeline_MinistryReview));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.VolunteerPipeline_MinistryReview);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the verify requirements page under people.")]
        public void People_VolunteerPipeline_VerifyRequirements() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->verify requirements
            test.Selenium.Navigate(Navigation.People.Volunteer_Pipeline.Verify_Requirements);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.VolunteerPipeline_VerifyRequirements));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.VolunteerPipeline_VerifyRequirements);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the background checks page under people.")]
        public void People_VolunteerPipeline_BackgroundChecks() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->background checks
            test.Selenium.Navigate(Navigation.People.Volunteer_Pipeline.Background_Checks);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.VolunteerPipeline_BackgroundChecks));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.VolunteerPipeline_BackgroundChecks);

            // Verify label
            Assert.AreEqual("Request status", test.Selenium.GetText("//label[@for='ctl00_ctl00_MainContent_content_ddlRequestStatus_dropDownList']"));


            // Navigate to people->background checks
            test.Selenium.Navigate(Navigation.People.Volunteer_Pipeline.Background_Checks);

            // Create a background check
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Add);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.VolunteerPipeline_BackgroundChecks_Add));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.VolunteerPipeline_BackgroundChecks_Add);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the approved volunteers page under people.")]
        public void People_VolunteerPipeline_ApprovedVolunteers() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->approved volunteers
            test.Selenium.Navigate(Navigation.People.Volunteer_Pipeline.Approved_Volunteers);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.VolunteerPipeline_ApprovedVolunteers));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.VolunteerPipeline_ApprovedVolunteers);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the rejected volunteers page under people.")]
        public void People_VolunteerPipeline_RejectedVolunteers() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->rejected volunteers
            test.Selenium.Navigate(Navigation.People.Volunteer_Pipeline.Rejected_Volunteers);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.VolunteerPipeline_RejectedVolunteers));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.VolunteerPipeline_RejectedVolunteers);

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Volunteer Pipeline
        
        #region Data Integrity
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the duplicate finder page under people.")]
        public void People_DataIntegrity_DuplicateFinder() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->duplicate finder
            test.Selenium.Navigate(Navigation.People.Data_Integrity.Duplicate_Finder);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.DataIntegrity_DuplicateFinder));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.DataIntegrity_DuplicateFinder);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the merge individual page under people.")]
        public void People_DataIntegrity_MergeIndividual() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->merge individual
            test.Selenium.Navigate(Navigation.People.Data_Integrity.Merge_Individual);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.DataIntegrity_MergeIndividual));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.DataIntegrity_MergeIndividual);

            // Verify 'Find person' links
            test.Selenium.VerifyElementPresent("link=Find person");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the move individual page under people.")]
        public void People_DataIntegrity_MoveIndividual() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->move individual
            test.Selenium.Navigate(Navigation.People.Data_Integrity.Move_Individual);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.DataIntegrity_MoveIndividual));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.DataIntegrity_MoveIndividual);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the split household page under people.")]
        public void People_DataIntegrity_SplitHousehold() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->split household
            test.Selenium.Navigate(Navigation.People.Data_Integrity.Split_Household);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.DataIntegrity_SplitHousehold));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.DataIntegrity_SplitHousehold);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the duplicate queue page under people.")]
        public void People_DataIntegrity_DuplicateQueue() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->duplicate queue
            test.Selenium.Navigate(Navigation.People.Data_Integrity.Duplicate_Queue);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.DataIntegrity_DuplicateQueue));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.DataIntegrity_DuplicateQueue);

            // Verify column headers
            test.Selenium.Click("ctl00_ctl00_MainContent_content_QDM1_TG");
            test.Selenium.Click("//table[@id='ctl00_ctl00_MainContent_content_QDM1_PN_MenuTable']/tbody/tr[6]/td[2]");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.Search);

            if (test.Selenium.IsElementPresent(TableIds.Portal.People_DuplicateQueue)) {
                Assert.AreEqual("Submitted Date", test.Selenium.GetText(string.Format("//table[@id='{0}']/tbody/tr[2]/td[2]", TableIds.Portal.People_DuplicateQueue)));
                Assert.AreEqual("Submitted By", test.Selenium.GetText(string.Format("//table[@id='{0}']/tbody/tr[2]/td[3]", TableIds.Portal.People_DuplicateQueue)));
            }
            else {
                test.Selenium.VerifyTextPresent("No records found");
            }

            // Logout of portal
            test.Portal.Logout();
        }

        #region Mass Action Queue
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the mass action queue page under people.")]
        public void People_DataIntegrity_MassActionQueue() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->mass action queue
            test.Selenium.Navigate(Navigation.People.Data_Integrity.Mass_Action_Queue);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.DataIntegrity_MassActionQueue));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.DataIntegrity_MassActionQueue);

            // Verify column header
            Assert.AreEqual("Submitted By", test.Selenium.GetText("//table[@id='mass_action_list']/tbody/tr[1]/th[4]"));

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views and verifies the prepare mass action page.")]
        public void People_DataIntegrity_MassActionQueue_PrepareMassAction() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Open the prepare mass action page
            test.Selenium.Open("/MassAction/Index.aspx?lid=1&count=1&contextType=1&contextTypeID=1&qb=1&isQueued=False");

            // Verify the available marital statuses
            test.Selenium.SelectAndWaitForCondition("MassActionType", "Change demographics", JavaScriptMethods.IsElementPresent("marital_status"), "10000");
            // Verify the marital status selections
            Assert.AreEqual(8, test.Selenium.GetXpathCount("//select[@id='marital_status']/option"));
            Assert.AreEqual("--", test.Selenium.GetText("//select[@id='marital_status']/option[1]"));
            Assert.AreEqual("Child/Youth", test.Selenium.GetText("//select[@id='marital_status']/option[2]"));
            Assert.AreEqual("Divorced", test.Selenium.GetText("//select[@id='marital_status']/option[3]"));
            Assert.AreEqual("Married", test.Selenium.GetText("//select[@id='marital_status']/option[4]"));
            Assert.AreEqual("Separated", test.Selenium.GetText("//select[@id='marital_status']/option[5]"));
            Assert.AreEqual("Single", test.Selenium.GetText("//select[@id='marital_status']/option[6]"));
            Assert.AreEqual("Widow", test.Selenium.GetText("//select[@id='marital_status']/option[7]"));
            Assert.AreEqual("Widower", test.Selenium.GetText("//select[@id='marital_status']/option[8]"));

            // Logout of portal
            test.Portal.Logout();
        }
        
        #endregion Mass Action Queue
        #endregion Data Integrity

        #region Counseling/Mentoring
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the view relationships page under people.")]
        public void People_CounselingMentoring_ViewRelationships() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->view relationships
            test.Selenium.Navigate(Navigation.People.CounselingMentoring.View_Relationships);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: View Relationships");
            test.Selenium.VerifyTextPresent("View Relationships");

            // ** Create a relationship
            // Create a new relationship
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Add);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Create New Relationship");
            test.Selenium.VerifyTextPresent("Create New Relationship");

            // Verify labels
            Assert.AreEqual("First person *", test.Selenium.GetText("ctl00_ctl00_MainContent_content_ucPrimaryRelationshipMember_lblRelation"));
            Assert.AreEqual("Second person *", test.Selenium.GetText("ctl00_ctl00_MainContent_content_ucSecondaryRelationshipMember_lblRelation"));

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Counseling/Mentoring
    }
}