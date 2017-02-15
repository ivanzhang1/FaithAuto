using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;

namespace FTTests.Portal.Admin {
    [TestFixture]
    public class Portal_Admin : FixtureBase {
        [Test, RepeatOnFailure, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Verifies the links present under the admin menu.")]
        public void Admin() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Verify the links present
            Assert.AreEqual("Security Setup", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[1]/dt"));
            Assert.AreEqual("Portal Users", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[1]/dd[1]/a"));
            Assert.AreEqual("Security Roles", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[1]/dd[2]/a"));

            Assert.AreEqual("People Setup", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[2]/dt"));
            Assert.AreEqual("Status", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[2]/dd[1]/a"));
            Assert.AreEqual("Individual Attributes", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[2]/dd[2]/a"));
            Assert.AreEqual("Individual Note Types", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[2]/dd[3]/a"));
            Assert.AreEqual("Individual Requirements", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[2]/dd[4]/a"));
            Assert.AreEqual("Relationship Types", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[2]/dd[5]/a"));
            Assert.AreEqual("Schools", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[2]/dd[6]/a"));

            Assert.AreEqual("Ministry Setup", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[3]/dt"));
            Assert.AreEqual("Ministries", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[3]/dd[1]/a"));
            Assert.AreEqual("Head Count Attributes", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[3]/dd[2]/a"));
            Assert.AreEqual("Activity Types", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[3]/dd[3]/a"));
            Assert.AreEqual("Job Attributes", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[3]/dd[4]/a"));
            Assert.AreEqual("Job Information", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[3]/dd[5]/a"));
            Assert.AreEqual("Volunteer Types", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[3]/dd[6]/a"));

            Assert.AreEqual("Contact Setup", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[4]/dt"));
            Assert.AreEqual("Form Names", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[4]/dd[1]/a"));
            Assert.AreEqual("Manage Items", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[4]/dd[2]/a"));
            Assert.AreEqual("Build Forms", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[4]/dd[3]/a"));
            Assert.AreEqual("Contact Dispositions", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[4]/dd[4]/a"));

            Assert.AreEqual("Church Setup", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[5]/dt"));
            Assert.AreEqual("Church Contacts", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[5]/dd[1]/a"));
            Assert.AreEqual("News & Announcements", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[5]/dd[2]/a"));
            Assert.AreEqual("Campuses", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[5]/dd[3]/a"));
            Assert.AreEqual("Buildings", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[5]/dd[4]/a"));
            Assert.AreEqual("Rooms", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[5]/dd[5]/a"));
            Assert.AreEqual("Departments", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[5]/dd[6]/a"));
            Assert.AreEqual("Mailing Address", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[5]/dd[7]/a"));
            Assert.AreEqual("Giftedness Programs", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[5]/dd[8]/a"));
            //Assert.AreEqual("Church Logo", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[5]/dd[9]/a"));

            Assert.AreEqual("Image Upload", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[6]/dt"));
            Assert.AreEqual("View Upload Status", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[6]/dd[1]/a"));

            Assert.AreEqual("Integration", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[7]/dt"));
            Assert.AreEqual("Applications", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[7]/dd[1]/a"));
            Assert.AreEqual("Application Keys", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[7]/dd[2]/a"));
            Assert.AreEqual("Data Exchange Export", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[7]/dd[3]/a"));
            Assert.AreEqual("Data Exchange Import", test.Selenium.GetText("//div[@id='nav_sub_6']/dl[7]/dd[4]/a"));

            // Logout of portal
            test.Portal.Logout();
        }

        #region Security Setup
        #region Portal Users
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the portal users page under admin.")]
        public void Admin_SecuritySetup_PortalUsers() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->portal users
            test.Selenium.Navigate(Navigation.Admin.Security_Setup.Portal_Users);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Portal Users");
            test.Selenium.VerifyTextPresent("Portal Users");

            // Verify the alpha bar
            Assert.IsTrue(test.Selenium.IsElementPresent("//ul[@class='grid_alpha_bar']"));


            // Click to create a new portal user
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Add);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Add/Edit Portal Users");
            test.Selenium.VerifyTextPresent("Add/Edit Portal Users");

            // Verify the save button
            Assert.AreEqual("Add new portal user", test.Selenium.GetValue(GeneralButtons.Save));


            // Navigate to admin->portal users
            test.Selenium.Navigate(Navigation.Admin.Security_Setup.Portal_Users);

            // Click to edit a user
            test.Selenium.Click(GeneralLinks.Options);
            test.Selenium.ClickAndWaitForPageToLoad("link=Edit user");

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Add/Edit Portal Users");
            test.Selenium.VerifyTextPresent("Add/Edit Portal Users");

            // Verify save button
            Assert.AreEqual("Save", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_btnMod"));


            // Replace photo
            test.Selenium.ClickAndSelectPopUp("ctl00_ctl00_MainContent_content_urlChangeImage", "win", "10000");
            test.Selenium.VerifyTextPresent(".jpg file must be no larger than 1048 kb in order to be uploaded.");

            // Close popup
            test.Selenium.Click("link=Close window");
            test.Selenium.SelectWindow("");


            // Navigate to admin->portal users
            test.Selenium.Navigate(Navigation.Admin.Security_Setup.Portal_Users);

            // Edit the rights for a user
            test.Selenium.ClickAndWaitForPageToLoad("link=Edit rights");

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Manage User Rights");
            test.Selenium.VerifyTextPresent("Manage User Rights");

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Portal Users

        #region Security Roles
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the security roles page under admin.")]
        public void Admin_SecuritySetup_SecurityRoles() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->security roles
            test.Selenium.Navigate(Navigation.Admin.Security_Setup.Security_Roles);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Security Roles");
            test.Selenium.VerifyTextPresent("Security Roles");


            // Navigate to admin->security roles
            test.Selenium.Navigate(Navigation.Admin.Security_Setup.Security_Roles);

            // Click to create a new security role
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Add);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Manage Security Role");
            test.Selenium.VerifyTextPresent("Manage Security Role");


            // Navigate to admin->security roles
            test.Selenium.Navigate(Navigation.Admin.Security_Setup.Security_Roles);

            // Edit an existing security role
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Edit);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Manage Security Role");
            test.Selenium.VerifyTextPresent("Manage Security Role");

            // Verify label
            Assert.AreEqual("Role name *", test.Selenium.GetText("ctl00_ctl00_MainContent_content_lblRoleName"));

            // Verify button
            Assert.AreEqual("Save role", test.Selenium.GetValue(GeneralButtons.Save));

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Security Roles
        #endregion Security Setup

        #region People Setup
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the status page under admin.")]
        public void Admin_PeopleSetup_Status() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->status
            test.Selenium.Navigate(Navigation.Admin.People_Setup.Status);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Status");
            test.Selenium.VerifyTextPresent("Status");

            // Verify labels
            Assert.AreEqual("Status group", test.Selenium.GetText("//table[@id='ctl00_ctl00_MainContent_content_tblStatus']/tbody/tr[1]/th"));
            Assert.AreEqual("Status name *", test.Selenium.GetText("ctl00_ctl00_MainContent_content_lblStatusNametxt"));
            Assert.AreEqual("Require a comment?", test.Selenium.GetText("//table[@id='ctl00_ctl00_MainContent_content_tblStatus']/tbody/tr[3]/th"));

            Assert.AreEqual("Add sub status", test.Selenium.GetText("ctl00_ctl00_MainContent_content_rptrStatus_ctl01_urlAddSSL"));

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the individual attributes page under admin.")]
        public void Admin_PeopleSetup_IndividualAttributes() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->individual attributes
            test.Selenium.Navigate(Navigation.Admin.People_Setup.Individual_Attributes);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Individual Attributes");
            test.Selenium.VerifyTextPresent("Individual Attributes");

            // Verify drop-down items and default item
            Assert.AreEqual("Active", test.Selenium.GetSelectedLabel("ctl00_ctl00_MainContent_content_ddlStatus"));
            //test.Selenium.VerifySelectedLabel("ctl00_ctl00_MainContent_content_ddlStatus", "Active");

            string[] selectOptions = test.Selenium.GetSelectOptions("ctl00_ctl00_MainContent_content_ddlStatus");
            Assert.AreEqual("All", selectOptions.GetValue(0));
            Assert.AreEqual("Active", selectOptions.GetValue(1));
            Assert.AreEqual("Inactive", selectOptions.GetValue(2));


            // Select the individual attributes link
            test.Selenium.ClickAndWaitForPageToLoad("ctl00_ctl00_MainContent_content_lnkIndividualAttributes");

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Individual Attributes");
            test.Selenium.VerifyTextPresent("Individual Attributes");

            // Verify labels
            Assert.AreEqual("Individual attribute name *", test.Selenium.GetText("//div[@id='add_attribute_ind']/div/table/tbody/tr[1]/th"));
            Assert.AreEqual("Record staff/pastor involved", test.Selenium.GetText("//div[@id='add_attribute_ind']/div/table/tbody/tr[2]/th"));
            Assert.AreEqual("Record start date", test.Selenium.GetText("//div[@id='add_attribute_ind']/div/table/tbody/tr[3]/th"));
            Assert.AreEqual("Record comment", test.Selenium.GetText("//div[@id='add_attribute_ind']/div/table/tbody/tr[4]/th"));
            Assert.AreEqual("Record end date", test.Selenium.GetText("//div[@id='add_attribute_ind']/div/table/tbody/tr[5]/th"));

            // Verify column header
            Assert.AreEqual("Staff/Pastor", test.Selenium.GetText("//table[@id='ctl00_ctl00_MainContent_content_dgIndividualAttributes']/tbody/tr[1]/td[3]"));

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the individual note types page under admin.")]
        public void Admin_PeopleSetup_IndividualNoteTypes() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->individual note types
            test.Selenium.Navigate(Navigation.Admin.People_Setup.Individual_Note_Types);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Individual Note Types");
            test.Selenium.VerifyTextPresent("Individual Note Types");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the individual requirements page under admin.")]
        public void Admin_PeopleSetup_IndividualRequirements() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->individual requirements
            test.Selenium.Navigate(Navigation.Admin.People_Setup.Individual_Requirements);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Individual Requirements");
            test.Selenium.VerifyTextPresent("Individual Requirements");

            // Verify column header
            Assert.AreEqual("Requirement Type", test.Selenium.GetText("//form[@id='aspnetForm']/table/tbody/tr[1]/th[4]"));

            // Verify deactivate link
            test.Selenium.VerifyElementPresent("link=Deactivate");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the relationship types page under admin.")]
        public void Admin_PeopleSetup_RelationshipTypes() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->relationship types
            test.Selenium.Navigate(Navigation.Admin.People_Setup.Relationship_Types);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Relationship Types");
            test.Selenium.VerifyTextPresent("Relationship Types");


            // Click to create a relationship type
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Add);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Add Relationship Type");
            test.Selenium.VerifyTextPresent("Add Relationship Type");

            // Verify label
            Assert.AreEqual("Relationship name *", test.Selenium.GetText("ctl00_ctl00_MainContent_content_ucManageRT_lblRelationshipName"));

            // Verify button
            Assert.AreEqual("Add relationship type", test.Selenium.GetValue(GeneralButtons.Save));

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the schools page under admin.")]
        public void Admin_PeopleSetup_Schools() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->schools
            test.Selenium.Navigate(Navigation.Admin.People_Setup.Schools);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Schools");
            test.Selenium.VerifyTextPresent("Schools");

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion People Setup

        #region Ministry Setup
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the ministries page under admin.")]
        public void Admin_MinistrySetup_Ministries() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->ministries
            test.Selenium.Navigate(Navigation.Admin.Ministry_Setup.Ministries);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Ministries");
            test.Selenium.VerifyTextPresent("Ministries");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the head count attributes page under admin.")]
        public void Admin_MinistrySetup_HeadCountAttributes() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->head count attributes
            test.Selenium.Navigate(Navigation.Admin.Ministry_Setup.Head_Count_Attributes);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Head Count Attributes");
            test.Selenium.VerifyTextPresent("Head Count Attributes");


            // Click to view the preferences for an attribute
            test.Selenium.ClickAndWaitForPageToLoad("link=Preferences");

            // Verify elements on page
            test.Selenium.VerifyTextPresent("Activity attribute preferences");
            test.Selenium.VerifyElementPresent("ctl00_ctl00_MainContent_content_Menu1_preferences_listActivityAttributeId");
            test.Selenium.VerifyElementPresent("ctl00_ctl00_MainContent_content_Menu1_preferences_listSelectedActivityAttributeId");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the activity types page under admin.")]
        public void Admin_MinistrySetup_ActivityTypes() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->activity types
            test.Selenium.Navigate(Navigation.Admin.Ministry_Setup.Activity_Types);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Activity Types");
            test.Selenium.VerifyTextPresent("Activity Types");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the job attributes page under admin.")]
        public void Admin_MinistrySetup_JobAttributes() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->job attributes
            test.Selenium.Navigate(Navigation.Admin.Ministry_Setup.Job_Attributes);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Job Attributes");
            test.Selenium.VerifyTextPresent("Job Attributes");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the job attributes page under admin.")]
        public void Admin_MinistrySetup_JobInformation() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->job information
            test.Selenium.Navigate(Navigation.Admin.Ministry_Setup.Job_Information);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Job Information");
            test.Selenium.VerifyTextPresent("Job Information");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the volunteer types page under admin.")]
        public void Admin_MinistrySetup_VolunteerTypes() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->volunteer types
            test.Selenium.Navigate(Navigation.Portal.Admin.Ministry_Setup.Volunteer_Types);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Volunteer Types");
            test.Selenium.VerifyTextPresent("Volunteer Types");

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Ministry Setup

        #region Contact Setup
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the form names page under admin.")]
        public void Admin_ContactSetup_FormNames() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->form names
            test.Selenium.Navigate(Navigation.Admin.Contact_Setup.Form_Names);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Contact Form Names");
            test.Selenium.VerifyTextPresent("Contact Form Names");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the manage items page under admin.")]
        public void Admin_ContactSetup_ManageItems() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->manage items
            test.Selenium.Navigate(Navigation.Admin.Contact_Setup.Manage_Items);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Manage Contact Items");
            test.Selenium.VerifyTextPresent("Manage Contact Items");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the build forms page under admin.")]
        public void Admin_ContactSetup_BuildForms() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Select admin->build forms
            test.Selenium.Navigate(Navigation.Admin.Contact_Setup.Build_Forms);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Build Forms");
            test.Selenium.VerifyTextPresent("Build Forms");

            // Verify label
            Assert.AreEqual("Form name", test.Selenium.GetText("//label[@for='ctl00_ctl00_MainContent_content_Contactinfo1_ddContactTypeId']"));

            // Verify link
            Assert.AreEqual("Individual attributes", test.Selenium.GetText("ctl00_ctl00_MainContent_content_Menu1_lnkAttrs"));

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the contact dispositions page under admin.")]
        public void Admin_ContactSetup_ContactDispositions() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->contact dispositions
            test.Selenium.Navigate(Navigation.Admin.Contact_Setup.Contact_Dispositions);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Contact Dispositions");
            test.Selenium.VerifyTextPresent("Contact Dispositions");

            // Verify required field label
            Assert.AreEqual("Name *", test.Selenium.GetText("ctl00_ctl00_MainContent_content_lblContactDispositionName"));

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Contact Setup

        #region Church Setup
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the church contacts page under admin.")]
        public void Admin_ChurchSetup_ChurchContacts() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->church contacts
            test.Selenium.Navigate(Navigation.Admin.Church_Setup.Church_Contacts);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Church Contacts");
            test.Selenium.VerifyTextPresent("Church Contacts");


            // Add a contact
            test.Selenium.ClickAndWaitForPageToLoad("link=Add contact");

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Church Contacts");
            test.Selenium.VerifyTextPresent("Church Contacts");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the news and announcements page under admin.")]
        public void Admin_ChurchSetup_NewsAnnouncements() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->news & announcements
            test.Selenium.Navigate(Navigation.Portal.Admin.Church_Setup.News_Announcements);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: News & Announcements");
            test.Selenium.VerifyTextPresent("Add/Edit News & Announcements");
            test.Selenium.VerifyTextPresent("News & Announcements");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the campuses page under admin.")]
        public void Admin_ChurchSetup_Campuses() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->campuses
            test.Selenium.Navigate(Navigation.Portal.Admin.Church_Setup.Campuses);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Campuses");
            test.Selenium.VerifyTextPresent("Campuses");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the buildings page under admin.")]
        public void Admin_ChurchSetup_Buildings() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->buildings
            test.Selenium.Navigate(Navigation.Admin.Church_Setup.Buildings);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Buildings");
            test.Selenium.VerifyTextPresent("Buildings");

            // Verify label
            Assert.AreEqual("Building name *", test.Selenium.GetText("//label[@for='ctl00_ctl00_MainContent_content_txtBuildingName']"));

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the rooms page under admin.")]
        public void Admin_ChurchSetup_Rooms() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->rooms
            test.Selenium.Navigate(Navigation.Admin.Church_Setup.Rooms);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Rooms");
            test.Selenium.VerifyTextPresent("Rooms");

            // Verify label
            Assert.AreEqual("Room name *", test.Selenium.GetText("ctl00_ctl00_MainContent_content_lblRoomName"));

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the departments page under admin.")]
        public void Admin_ChurchSetup_Departments() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->departments
            test.Selenium.Navigate(Navigation.Admin.Church_Setup.Departments);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Departments");
            test.Selenium.VerifyTextPresent("Departments");

            // Verify label
            Assert.AreEqual("Name *", test.Selenium.GetText("//form[@id='aspnetForm']/fieldset/table/tbody/tr[1]/th"));

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the mailing address page under admin.")]
        public void Admin_ChurchSetup_MailingAddress() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->mailing address
            test.Selenium.Navigate(Navigation.Admin.Church_Setup.Mailing_Address);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Mailing Address");
            test.Selenium.VerifyTextPresent("Mailing Address");

            // Verify labels, buttons
            Assert.AreEqual("Country", test.Selenium.GetText("ctl00_ctl00_MainContent_content_ctlAddress_lblCountry"));
            Assert.AreEqual("Address 1 *", test.Selenium.GetText("ctl00_ctl00_MainContent_content_ctlAddress_lblAddress1"));
            Assert.AreEqual("Address 2", test.Selenium.GetText("ctl00_ctl00_MainContent_content_ctlAddress_lblAddress2"));
            Assert.AreEqual("City *", test.Selenium.GetText("ctl00_ctl00_MainContent_content_ctlAddress_lblCity"));
            Assert.AreEqual("State", test.Selenium.GetText("ctl00_ctl00_MainContent_content_ctlAddress_lblStateDDL"));
            Assert.AreEqual("Postal code", test.Selenium.GetText("ctl00_ctl00_MainContent_content_ctlAddress_lblPostalCode"));

            Assert.AreEqual("Save address", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_btnSubmit"));

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the twa census page under admin.")]
        public void Admin_ChurchSetup_TWACensus() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

           /* if (base.F1Environment == F1Environments.DEV) {
                // Navigate to admin->twa census
                test.Selenium.Navigate(Navigation.Admin.Church_Setup.TWA_Census);

                // Verify title, text
                test.Selenium.VerifyTitle("Fellowship One :: TWA Census");
                test.Selenium.VerifyTextPresent("TWA Census");

                // Verify labels
                Assert.AreEqual("Edition", test.Selenium.GetText("//div[@id='main_content']/div[1]/div[2]/div/table/tbody/tr[1]/th[1]"));
                Assert.AreEqual("Contract duration", test.Selenium.GetText("//div[@id='main_content']/div[1]/div[2]/div/table/tbody/tr[2]/th[1]"));
                Assert.AreEqual("Pay frequency", test.Selenium.GetText("//div[@id='main_content']/div[1]/div[2]/div/table/tbody/tr[3]/th[1]"));

                Assert.AreEqual("Go live", test.Selenium.GetText("//div[@id='main_content']/div[1]/div[2]/div/table/tbody/tr[1]/th[2]"));
                Assert.AreEqual("Contract start", test.Selenium.GetText("//div[@id='main_content']/div[1]/div[2]/div/table/tbody/tr[2]/th[2]"));
                Assert.AreEqual("Expiration", test.Selenium.GetText("//div[@id='main_content']/div[1]/div[2]/div/table/tbody/tr[3]/th[2]"));

                // Verify column header
                Assert.AreEqual("Updated By", test.Selenium.GetText("//div[@id='main_content']/div[1]/div[2]/table/tbody/tr[1]/th[5]"));
            }*/

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the giftedness programs page under admin.")]
        public void Admin_ChurchSetup_GiftednessPrograms() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->giftedness programs
            test.Selenium.Navigate(Navigation.Admin.Church_Setup.Giftedness_Programs);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Giftedness Programs");
            test.Selenium.VerifyTextPresent("Giftedness Programs");

            // Verify labels
            Assert.AreEqual("Program name *", test.Selenium.GetText("//form[@id='aspnetForm']/div[3]/fieldset/table/tbody/tr[1]/th"));
            Assert.AreEqual("Abbreviated name *", test.Selenium.GetText("//form[@id='aspnetForm']/div[3]/fieldset/table/tbody/tr[2]/th"));


            // Navigate to admin->giftedness programs
            test.Selenium.Navigate(Navigation.Admin.Church_Setup.Giftedness_Programs);

            // Click to view the summary of a giftedness program
            test.Selenium.ClickAndWaitForPageToLoad("link=View Summary");

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Giftedness Program Summary");
            test.Selenium.VerifyTextPresent("Giftedness Programs");
            test.Selenium.VerifyTextPresent("Categories");
            test.Selenium.VerifyTextPresent("Attributes");

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Church Setup

        #region Image Upload
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the view upload status page under admin.")]
        public void Admin_ImageUpload_ViewUploadStatus() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->view upload status
            test.Selenium.Navigate(Navigation.Admin.Image_Upload.View_Upload_Status);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: View Upload Status");
            test.Selenium.VerifyTextPresent("View Upload Status");

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Image Upload

        #region Integration
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the applications page under admin.")]
        public void Admin_Integration_Applications() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->applications
            test.Selenium.Navigate(Navigation.Admin.Integration.Applications);

            // Verify title, test
            test.Selenium.VerifyTitle("Fellowship One :: Applications");
            test.Selenium.VerifyTextPresent("Applications");
            test.Selenium.VerifyTextPresent("1st party — Developed for all customers by Fellowship Technologies");
            test.Selenium.VerifyTextPresent("2nd party — Developed for your church, by your church (or by someone else)");
            test.Selenium.VerifyTextPresent("3rd party — Developed for all customers by members of the community (vendors, ministries, partners, etc.)");

            // ** View an application
            test.Selenium.ClickAndWaitForPageToLoad("link=F1Touch");

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Enable/Disable Application");
            test.Selenium.VerifyTextPresent("Enable/Disable Application");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the application keys page under admin.")]
        public void Admin_Integration_ApplicationKeys() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->application keys
            test.Selenium.Navigate(Navigation.Admin.Integration.Application_Keys);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Application Keys");
            test.Selenium.VerifyTextPresent("Application Keys");
            test.Selenium.VerifyElementPresent("link=Dynamic Church");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the data exchange export page under admin.")]
        public void Admin_Integration_DataExchangeExport() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->data exchange export
            test.Selenium.Navigate(Navigation.Admin.Integration.Data_Exchange_Export);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Data Exchange Export");
            test.Selenium.VerifyTextPresent("Data Exchange Export");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the data exchange import page under admin.")]
        public void Admin_Integration_DataExchangeImport() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->data exchange import
            test.Selenium.Navigate(Navigation.Admin.Integration.Data_Exchange_Import);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Data Exchange Import");
            test.Selenium.VerifyTextPresent("Data Exchange Import");

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Integration

    }

    [TestFixture]
    public class Portal_Admin_Tiers_WebDriver : FixtureBaseWebDriver
    {
        #region TIERS

        #region Volunteer Management

        #region PeopleTab
        [Test, MultipleAsserts, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies when a church does not have the Volunteer Management module no Volunteer/Staff functionality appears under the People Tab section")]
        public void Admin_TIERS_VolunteerManagement_PeopleTab()
        {
            // Make sure the module is turned off for church 255 (QA3)
            base.SQL.Portal_Modules_Delete(255, 84);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C3");

            // Click on the People Tab and verify the menu items are not appearing
            test.Driver.FindElementByLinkText("People").Click();

            // Verify the Volunteer Pipeline section is not appearing
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Volunteer Pipeline")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Submit Application")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Review Applications")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Ministry Review")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Verify Requirements")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Background Checks")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Approved Volunteers")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Rejected Volunteers")));

            // Close Drawer
            test.Driver.FindElementByLinkText("People").Click();

            //Logout 
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verfies when a church does not have the Volunteer management module turn on the individual record reflect that behavior correctly")]
        public void Admin_TIERS_VolunteerManagment_IndividualRecord()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C3");

            //Navigate to individual record 
            test.Portal.People_ViewIndividual_WebDriver("FT Tester");

            // Verify their involvement grid does not display Staff assignments
            TestLog.WriteLine(string.Format("Individual page text = {0}", test.Driver.FindElementByXPath("//span[@class='attendance_staff']").Text));
            Assert.AreNotEqual("Staffing", test.Driver.FindElementByXPath("//span[@class='attendance_staff']").Text);

            // Click on View Household
            test.Driver.FindElementByLinkText("View the household").Click();
            test.GeneralMethods.WaitForElement(By.LinkText("FT Tester"));

            // Verify that involvement grid does not display Staff assignments
            TestLog.WriteLine(string.Format("Household page text = {0}", test.Driver.FindElementByXPath("//span[@class='attendance_staff']").Text));
            Assert.AreNotEqual("Staffing", test.Driver.FindElementByXPath("//span[@class='attendance_staff']").Text);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        #endregion PeopleTab

        #region MinistryTab
        [Test, MultipleAsserts, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies when a church does not have the Volunteer Management module no Volunteer/Staff functionality appears under the Ministry Tab section")]
        public void Admin_TIERS_VolunteerManagement_MinistryTab()
        {
            // Make sure the module is turned off for church 255 (QA3)
            base.SQL.Portal_Modules_Delete(255, 84);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C3");

            // Click on the People Tab and verify the menu items are not appearing
            test.Driver.FindElementByLinkText("Ministry").Click();

            // Verify the Volunteer Pipeline section is not appearing
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Volunteer / Staff Jobs")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Rotation Schedules")));
            //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Staffing Assignment")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Jobs")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Schedules")));
            //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("VSO Funnel Report")));

            // Close Drawer
            test.Driver.FindElementByLinkText("Ministry").Click();

            //Logout 
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies when a church does not have the Volunteer Management module no Volunteer/Staff functionality appears under the Activities View All pages")]
        public void Admin_TIERS_VolunteerManagement_Activities_ViewAll()
        {
            // Make sure the module is turned off for church 255 (QAEUNLX0C3)
            base.SQL.Portal_Modules_Delete(255, 84);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C3");

            // Navigate to the Activities View All Page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Activities.View_All);

            //Verify Volunteer elements not found
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//span[@class='badge badge-staff']")));

            //Go to activity page
            test.Driver.FindElementByLinkText("A Test Activity").Click();
            test.GeneralMethods.WaitForElement(By.LinkText("View assignments"));

            //Verify Volunteer items not present on Activity page
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Staff & Volunteer Schedules")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Manage Staffing Needs")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//span[@class='badge badge-staff']")));

            //Click Roster tab
            test.Driver.FindElementByLinkText("Rosters").Click();
            test.GeneralMethods.WaitForElement(By.LinkText("Add Folder"));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//span[@class='badge badge-staff']")));

            //Click Breakout tab
            test.Driver.FindElementByLinkText("Breakout Groups").Click();
            test.GeneralMethods.WaitForElement(By.LinkText("Add Breakout Group"));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//span[@class='badge badge-staff']")));

            // Logout
            test.Portal.LogoutWebDriver();

        }


        /// Attendance
        [Test, MultipleAsserts, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies when a church does not have the Volunteer Management module no Volunteer/Staff functionality appears under the Post Assignments page")]
        public void Admin_TIERS_VolunteerManagement_PostAttendance()
        {
            // Make sure the module is turned off for church 255 (QA3)
            base.SQL.Portal_Modules_Delete(255, 84);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C3");

            // Navigate to the Post Attendance page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Attendance.Post_Attendance);

            //Select Ministry to post attendance for
            string selectMinistry = test.Driver.FindElementById(GeneralMinistry.PostAttendance.MinistryName).Text;
            if (selectMinistry != "A Test Ministry")
            {
                test.Driver.FindElementByLinkText("Change").Click();
                new SelectElement(test.Driver.FindElementById(GeneralMinistry.PostAttendance.Ministry_DropDown)).SelectByText("A Test Ministry");
                test.GeneralMethods.WaitForElementEnabled(By.Id(GeneralMinistry.PostAttendance.Activity_DropDown));
            }
            //Select Activity 
            new SelectElement(test.Driver.FindElementById(GeneralMinistry.PostAttendance.Activity_DropDown)).SelectByText("A Test Activity");
            test.GeneralMethods.WaitForElementEnabled(By.Id(GeneralMinistry.PostAttendance.Schedule_DropDown));
            //Select Schedule
            new SelectElement(test.Driver.FindElementById(GeneralMinistry.PostAttendance.Schedule_DropDown)).SelectByText("A Test Schedule");
            test.GeneralMethods.WaitForElementEnabled(By.Id(GeneralMinistry.PostAttendance.Time_DropDown));
            //Select DateTime
            string scheduleDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(-1).ToString("M/d/yyyy");
            string selectDate = scheduleDate + " " + "8:00 AM";
            new SelectElement(test.Driver.FindElementById(GeneralMinistry.PostAttendance.Time_DropDown)).SelectByText(selectDate);
            //Search
            test.Driver.FindElementById(GeneralMinistry.PostAttendance.Search_Button).Click();
            test.GeneralMethods.WaitForElement(By.LinkText("Post attendance"));
            //Go to Post Attendance screen
            test.Driver.FindElementByLinkText("Post attendance").Click();
            test.GeneralMethods.WaitForElement(By.Id(GeneralMinistry.PostAttendance.SearchAll_Radio));
            // Verify that the Volunteer staff option is not present
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GeneralMinistry.PostAttendance.StaffAssignment_Radio)));
            // Post attendance
            test.Driver.FindElementById(GeneralMinistry.PostAttendance.SearchAll_TextBox).SendKeys("FT Tester");
            test.Driver.FindElementById(GeneralMinistry.PostAttendance.Search_Button).Click();
            test.GeneralMethods.WaitForElement(By.Id("search_results_shell"));
            test.Driver.FindElementByXPath("//table[@id='search_results_table']/tbody/tr[1]/th[1]/input").Click();
            test.Driver.FindElementById(GeneralMinistry.PostAttendance.PostAttendance_Button).Click();
            test.GeneralMethods.WaitForElement(By.Id("post_results_message"));
            //Navigate to Attendees page
            test.Driver.FindElementByLinkText("View all attendees").Click();
            test.GeneralMethods.WaitForElement(By.Id(GeneralMinistry.PostAttendance.Action_DropDown));
            //Verify All Volunteer options are not present
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.Id("ctl00_ctl00_MainContent_content_ddlType_dropDownList")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.Id("ctl00_ctl00_MainContent_content_btnFilter")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//span[@class='icon_attendance_staff nudge_right']")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//select[@id='ctl00_ctl00_MainContent_content_ddlAction_dropDownList']/option[3]")));
            
            //Logout
            test.Portal.LogoutWebDriver();

        }
        /// Assignments
        [Test, MultipleAsserts, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies when a church does not have the Volunteer Management module no Volunteer/Staff functionality appears under the Assignments View All page")]
        public void Admin_TIERS_VolunteerManagment_Assignments_ViewAll()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C3");

            // Navigate to the Assignments View All Page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

            test.Portal.Ministry_Assignments_View_All_Filters_Ministry("A Test Ministry", false);
            test.Portal.Ministry_Assignments_View_All_Filters_Activity(new string[] {"A Test Activity"} );

            //Verify missing features
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.Id("showActiveStaff")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.Id("showInactiveStaff")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//span[@class='attendance_staff']")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.Id("staffingScheduleSection")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.Id("jobSection")));

            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure, Timeout(1000)]
        [Author("Stuart Platt")]
        [Description("Verifies when a church does not have the Volunteer Management module no Volunteer/Staff functionality appears under the Add Assignment Page")]
        public void Admin_TIERS_VolunteerManagement_AddAssignment()
        {
            // Make sure the module is turned off for church 255 (QA3)
            base.SQL.Portal_Modules_Delete(255, 84);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C3");

            // Navigate to the Add Assignment Page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.Add_Assignment);
            test.Portal.Ministry_Assignments_Search("FT Tester");
            test.Driver.FindElementByLinkText("Add").Click();
            test.GeneralMethods.WaitForElement(By.Id("show_ministry"));

            //Verify that Volunteer option not present
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.Id("staff_assign_individual")));

            //Logout
            test.Portal.LogoutWebDriver();

        }
        #endregion MinistryTab

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies when a church does not have the Volunteer Management module no Volunteer/Staff functionality appears under the WebLink Tab section")]
        public void Admin_TIERS_VolunteerManagement_WebLinkTab()
        {
            // Make sure the module is turned off for church 255 (QA3)
            base.SQL.Portal_Modules_Delete(255, 84);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C3");

            // Click on the People Tab and verify the menu items are not appearing
            test.Driver.FindElementByLinkText("WebLink").Click();

            // Verify the Volunteer Pipeline section is not appearing
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//a[@href='/admin/volunteer/volunteerForm.aspx']")));

            // Close Drawer
            test.Driver.FindElementByLinkText("WebLink").Click();

            //Logout 
            test.Portal.LogoutWebDriver();
        }

        [Test, MultipleAsserts, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies when a church does not have the Volunteer Management module no Volunteer/Staff functionality appears under the Admin Tab section")]
        public void Admin_TIERS_VolunteerManagement_AdminTab()
        {
            // Make sure the module is turned off for church 255 (QA3)
            base.SQL.Portal_Modules_Delete(255, 84);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C3");

            // Click on the People Tab and verify the menu items are not appearing
            test.Driver.FindElementByLinkText("Admin").Click();

            // Verify the Volunteer Pipeline section is not appearing
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Job Attributes")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Job Information")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Volunteer Types")));

            // Close Drawer
            test.Driver.FindElementByLinkText("Admin").Click();

            //Logout 
            test.Portal.LogoutWebDriver();
        }
        #endregion Volunteer Management

        #region Infellowship Groups

        /// TO DO
        /// Groups View All
        [Test, MultipleAsserts, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that when the Group Management module is turned off the Group features are not present on the groups view all page")]
        public void Admin_TIERS_GroupManagement_Groups_ViewAll()
        {
            // Make sure the module is turned off (for the church: QAEUNLX0C3)
            base.SQL.Portal_Modules_Delete(255, 29);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C3");

            // Navigate to the groups view all page
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            //Verify elements not present
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Add a group")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Group types and custom fields")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Campus")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Standard Fields")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Day of the Week")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Spans of Care")));
            //Verify Name search filter and settings
            Assert.AreEqual("Name", test.Driver.FindElementById("name_toggle").Text);
            test.Driver.FindElementById("name_toggle").Click();
            test.GeneralMethods.WaitForElementDisplayed(By.Id("group_name"));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.Id("start_date_from")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.Id("start_date_to")));

            //Log out
            test.Portal.LogoutWebDriver();
        }

        [Test, MultipleAsserts, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that when the Group Management module is turned off the Group features are not present on the groups tab")]
        public void Admin_TIERS_GroupManagement_GroupsTab()
        {
            // Make sure the module is turned off (for the church: QAEUNLX0C3)
            base.SQL.Portal_Modules_Delete(255, 29);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C3");

            // Click on the People Tab and verify the menu items are not appearing
            test.Driver.FindElementByLinkText("Groups").Click();

            // Verify the Volunteer Pipeline section is not appearing
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Group Types")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Custom Fields")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Search Categories")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Span of Care")));

            // Close Drawer
            test.Driver.FindElementByLinkText("Groups").Click();

            //Logout 
            test.Portal.LogoutWebDriver();
        }
        /// Admin rights
        /// Infellowship
        /// Add to Groups Gear box 
        /// - Assignments
        [Test, MultipleAsserts, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that when the Group Management module is turned off the Group features are not present on the Assignments View all - Add to Group Modal")]
        public void Admin_TIERS_GroupManagement_Assignments_ViewAll_AddToGroup()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C3");

            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";

            //Navigate to Assignments View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

            //Search for assignments
            test.Portal.Ministry_Assignments_View_All_Filters_Ministry(ministryName, false);
            test.Portal.Ministry_Assignments_View_All_Filters_Activity(new string[] { activityName });
            test.GeneralMethods.WaitForElementEnabled(By.XPath("//tr[@class='participant-row']"));

            //Select all assignees
            test.Driver.FindElementByXPath(GeneralMinistry.Assignments.Select_All_CheckBox).Click();

            // Open add to group modal
            Assert.IsTrue(test.Driver.FindElementById(GeneralMinistry.Assignments.Add_To_Group_DropDown).Enabled, "Add To Group DropDown was not enabled");
            test.Driver.FindElementById(GeneralMinistry.Assignments.Add_To_Group_DropDown).Click();
            test.Driver.FindElementByLinkText("Add assigned to group").Click();
            test.GeneralMethods.WaitForElement(By.Id(GeneralMinistry.Assignments.Add_To_Group_Modal));

            //Verify Group options not available for New Group condition
            Assert.IsTrue(test.Driver.FindElementById(GeneralMinistry.Assignments.Add_To_Group_Modal_New_Radio_Button).Selected);
            TestLog.WriteLine(string.Format("This is the first Group Type Option - {0}", test.Driver.FindElementByXPath("//select[@id='type']/option[1]").Text));
            TestLog.WriteLine(string.Format("This is the second Group Type Option - {0}", test.Driver.FindElementByXPath("//select[@id='type']/option[2]").Text));
            Assert.AreEqual(2, test.Driver.FindElement(By.Id("type")).FindElements(By.TagName("option")).Count);
            Assert.AreEqual("People List", test.Driver.FindElementByXPath("//select[@id='type']/option[1]").Text);
            Assert.AreEqual("Temporary Group", test.Driver.FindElementByXPath("//select[@id='type']/option[2]").Text);

            // Verify Group options not available for Existing Group condition
            test.Driver.FindElementById(GeneralMinistry.Assignments.Add_To_Group_Modal_Existing_Radio_Button).Click();
            Assert.IsTrue(test.Driver.FindElementById(GeneralMinistry.Assignments.Add_To_Group_Modal_Existing_Radio_Button).Selected);
            test.GeneralMethods.WaitForElementEnabled(By.Id(GeneralMinistry.Assignments.Add_To_Group_Modal_Type_DropDown), 30, "Group Type drop down did not load");
            TestLog.WriteLine(string.Format("This is the first Group Type Option - {0}", test.Driver.FindElementByXPath("//select[@id='type']/option[1]").Text));
            TestLog.WriteLine(string.Format("This is the second Group Type Option - {0}", test.Driver.FindElementByXPath("//select[@id='type']/option[2]").Text));
            Assert.AreEqual(2, test.Driver.FindElement(By.Id("type")).FindElements(By.TagName("option")).Count);
            Assert.AreEqual("People List", test.Driver.FindElementByXPath("//select[@id='type']/option[1]").Text);
            Assert.AreEqual("Temporary Group", test.Driver.FindElementByXPath("//select[@id='type']/option[2]").Text);

            //Exit Modal
            test.Driver.FindElementById("fancybox-close").Click();

            //Logout 
            test.Portal.LogoutWebDriver();

        }
        /// - People Query
        [Test, MultipleAsserts, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that when the Group Management module is turned off the Group features are not present on the People Query Add to Group Modal")]
        public void Admin_TIERS_GroupManagement_PeopleQuery_AddToGroup()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C3");

            // Run a People Query
            test.Portal.People_PeopleQuery(SearchByConstants.Individual_Information, FieldConstants.Last_Name, ComparisonConstants.Starts_With, new string[] { "Test" });
            test.GeneralMethods.WaitForElement(By.Id("ctl00_ctl00_MainContent_content_grdIndividuals"));
            
            //Select All
            test.Driver.FindElementById("checkAll").Click();

            //Open the Add to Group Modal
            test.Driver.FindElementByXPath("//form[@id='aspnetForm']/div[3]/div/a").Click();
            ///html/body/div[1]/div[4]/div/div/div[1]/div[2]/form/div[3]/div/a
            test.Driver.FindElementByLinkText("Add to group…").Click();
            test.GeneralMethods.WaitForElementDisplayed(By.Id("form_modal_content"));

            //Verify that no Group features are enabled in the Existing group type dropdown
            TestLog.WriteLine(string.Format("This is the Existing Group Type option text - {0}", test.Driver.FindElementByXPath("//select[@id='ddlAddToGroup']/optgroup[2]").GetAttribute("label").ToString()));
            Assert.AreEqual(2, test.Driver.FindElement(By.Id("ddlAddToGroup")).FindElements(By.TagName("optgroup")).Count);
            Assert.AreEqual("People List", test.Driver.FindElementByXPath("//select[@id='ddlAddToGroup']/optgroup[2]").GetAttribute("label").ToString());
            
            //Select Add to new group
            //updated by ivan, the text is updated. 
            new SelectElement(test.Driver.FindElementById("ddlAddToGroup")).SelectByText("Add to a new group...");
            
            //Verify that no group features are enable in the new group dropdown
            TestLog.WriteLine(string.Format("This is the New group type options text - {0}", test.Driver.FindElementByXPath("//select[@id='ddlNewGroupType']/option").Text));
            //updated by ivan, the element id is updated.
            Assert.AreEqual(1, test.Driver.FindElement(By.Id("ddlNewGroupType")).FindElements(By.TagName("option")).Count);
            Assert.AreEqual("People List", test.Driver.FindElementByXPath("//select[@id='ddlNewGroupType']/option").Text);

            //Close Modal
            test.Driver.FindElementById("form_modal_close").Click();

            //Logout
            test.Portal.LogoutWebDriver();

        }

        #endregion Infellowship Groups

        #region Relationship Manager

        //[Test, RepeatOnFailure, MultipleAsserts]
        [Author("Stuart Platt")]
        [Description("Verifies that the Releationship management features do not show on the People Tab when the module is turned off")]
        public void Admin_TIERS_RelationshipManager_MenuItems()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C3");

            // Click on the People Tab and verify the menu items are not appearing
            test.Driver.FindElementByLinkText("People").Click();

            // Verify the Volunteer Pipeline section is not appearing
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("View Relationships")));

            // Close Drawer
            test.Driver.FindElementByLinkText("People").Click();

            // Click on the People Tab and verify the menu items are not appearing
            test.Driver.FindElementByLinkText("Admin").Click();

            // Verify the Volunteer Pipeline section is not appearing
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Relationship Types")));

            // Close Drawer
            test.Driver.FindElementByLinkText("Admin").Click();

            //Logout 
            test.Portal.LogoutWebDriver();
        }

        //[Test, RepeatOnFailure, MultipleAsserts]
        [Author("Stuart Platt")]
        [Description("Verifies that Weblink access to the Relationship manager is disabled when module is turned off.")]
        public void Admin_TIERS_RelationshipManager_WeblinkAccess()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string userName = "ft.autotester@gmail.com";
            string password = "FT4life!";
            string encryptedChurchCode = "O5JJMNqcTeedP3XS+ev/fg==";
            
            //Navigate to Weblink Relationship manager
            test.Weblink.ViewRelationshipManager(userName, password, encryptedChurchCode);

            // Verify that Access has been denied
            Assert.Contains("Access has been denied", test.Driver.FindElementByXPath("//table[@id='tblConfirmation']/tbody/tr[2]/td").Text);
            Assert.Contains("Please close your browser tab or window", test.Driver.FindElementByXPath("//table[@id='tblConfirmation']/tbody/tr[2]/td").Text);

            test.Weblink.Logout(encryptedChurchCode);

        }

        //[Test, RepeatOnFailure, MultipleAsserts]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add a Relationship link is not on the individual record when the module is turned off.")]
        public void Admin_TIERS_RelationshipManager_IndividualEdit()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C3");

            //Navigate to Individual record 
            test.Portal.People_ViewIndividual_WebDriver("FT Tester");

            //Open the Gear
            test.Driver.FindElementByLinkText("Options").Click();

            // Verify the Volunteer Pipeline section is not appearing
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Add a relationship")));

            //Logout
            test.Portal.LogoutWebDriver();

        }
        #endregion Relationship Manager

        #endregion TIERS

    }
}