using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace FTTests.Portal.Ministry {
    public struct MinistryAttendanceConstants {
        public struct PostAttendance {
            public const string DropDown_AttendeeFilter = "ctl00_ctl00_MainContent_content_ddlType_dropDownList";
            public const string DropDown_AttendanceType = "ctl00_ctl00_MainContent_content_ddlDetailType_dropDownList";
            public const string Button_PostAttendanceForSelectedIndividuals = "post_attendance";
            public const string Button_FilterAttendees = "ctl00_ctl00_MainContent_content_btnFilter";
            public const string Button_ChangeGo = "ctl00_ctl00_MainContent_content_btnChangeDetails";
            public const string TextField_NameSearch = "ctl00_ctl00_MainContent_content_basicsearch";
        }
    }

    public struct MinistryVolunteersConstants {
        public struct StaffingAssignments {
            public const string DropDown_Activity = "ctl00_ctl00_MainContent_content_Addeditstaff1_activityDropDown";
            public const string DropDown_VolunteerType = "ctl00_ctl00_MainContent_content_Addeditstaff1_indTypeDropDown"; 
            public const string DropDown_RLC = "ctl00_ctl00_MainContent_content_Addeditstaff1_activityDetailDropDown";
            public const string DropDown_Status = "ctl00_ctl00_MainContent_content_Addeditstaff1_statusDropDown";
            public const string DropDown_ActivityRLCGrouping = "ctl00_ctl00_MainContent_content_Addeditstaff1_ddlArea";
            public const string DropDown_Job = "ctl00_ctl00_MainContent_content_Addeditstaff1_jobDropDown";
            public const string DropDown_StaffSchedule = "ctl00_ctl00_MainContent_content_Addeditstaff1_scheduleDropDown";
            public const string DropDown_BreakoutGroup = "ctl00_ctl00_MainContent_content_Addeditstaff1_individualGroupDropDown";
            public const string DropDown_ActivitySchedule = "ctl00_ctl00_MainContent_content_Addeditstaff1_actTimeInstanceDropDown";
        }
    }

    public struct ThemeManagerConstants {
        public static string ThemeName = "A Test Theme";
        public static string Preview = "link=Preview";
        public static string Apply_to_super_checkin = "link=Apply to super check-in";
    }

    public struct MinistryEnumerations {
        public enum AssignmentRole {
            Staff,
            Participant
        }
    }

    //All of the legacy features are removed by FO-4800. 09/24/2015. commented by ivan.zhang.
	[TestFixture]
	public class Portal_Ministry : FixtureBase {

        [Test, RepeatOnFailure, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Verifies the links present under the ministry menu.")]
        public void Ministry() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Verify the links present
			Assert.AreEqual("Contacts", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[4]/dt"));
			Assert.AreEqual("My Contacts", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[4]/dd[1]/a"));
			Assert.AreEqual("Edit Contact Forms", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[4]/dd[2]/a"));
			Assert.AreEqual("Monitor Efficiency", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[4]/dd[3]/a"));
			Assert.AreEqual("Monitor Statistics", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[4]/dd[4]/a"));

			//Assert.AreEqual("Activities (Legacy)", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[3]/dt"));
			//Assert.AreEqual("Activities", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[3]/dd[1]/a"));
			//Assert.AreEqual("Activity Schedules", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[3]/dd[2]/a"));
			//Assert.AreEqual("Activity RLC Groups", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[3]/dd[3]/a"));
			//Assert.AreEqual("Rooms, Locations & Classes", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[3]/dd[4]/a"));
			//Assert.AreEqual("Breakout Groups", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[3]/dd[5]/a"));
			//Assert.AreEqual("Activity Requirements", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[3]/dd[6]/a"));
			//Assert.AreEqual("Group Finder Properties", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[3]/dd[7]/a"));

            Assert.AreEqual("Activities", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[1]/dt"));
            Assert.AreEqual("View All", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[1]/dd[1]/a"));
            Assert.AreEqual("Add Activity", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[1]/dd[2]/a"));
            Assert.AreEqual("Requirements", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[1]/dd[3]/a"));

			Assert.AreEqual("Assignments", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[2]/dt"));
			Assert.AreEqual("View All", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[2]/dd[1]/a"));
            Assert.AreEqual("Add Assignment", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[2]/dd[2]/a"));
            Assert.AreEqual("Volunteer / Staff Jobs", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[2]/dd[3]/a"));
            Assert.AreEqual("Rotation Schedules", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[2]/dd[4]/a"));
            Assert.AreEqual("Mass Change", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[2]/dd[5]/a"));

			//Assert.AreEqual("Participants (Legacy)", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[4]/dt"));
			//Assert.AreEqual("Assignments", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[4]/dd[1]/a"));
			//Assert.AreEqual("Manage Assignments", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[4]/dd[2]/a"));

			//Assert.AreEqual("Volunteers (Legacy)", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[5]/dt"));
			//Assert.AreEqual("Staffing Assignment", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[5]/dd[1]/a"));
			//Assert.AreEqual("Jobs", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[5]/dd[2]/a"));
			//Assert.AreEqual("Schedules", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[5]/dd[3]/a"));
            //Assert.AreEqual("VSO Funnel Report", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[5]/dd[4]/a"));

            Assert.AreEqual("Attendance", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[3]/dt"));
            Assert.AreEqual("Post Attendance", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[3]/dd[1]/a"));
            Assert.AreEqual("Head Count", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[3]/dd[2]/a"));
            Assert.AreEqual("Automated Attendance Rules", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[3]/dd[3]/a"));

			Assert.AreEqual("Check-in", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[5]/dt"));
			Assert.AreEqual("Live Check-ins", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[5]/dd[1]/a"));
			Assert.AreEqual("Super Check-ins", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[5]/dd[2]/a"));
			Assert.AreEqual("Theme Manager", test.Selenium.GetText("//div[@id='nav_sub_3']/dl[5]/dd[3]/a"));

            // Logout of portal
			test.Portal.Logout();
		}
        
        #region Contacts
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Matthew Sneeden")]
		[Description("Navigates to the my contacts page under ministry.")]
        public void Ministry_Contacts_MyContacts() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to ministry->my contacts
			test.Selenium.Navigate(Navigation.Ministry.Contacts.My_Contacts);

            // Change ministry
            test.Portal.Ministry_ChangeMinistry(DataConstants.Ministry);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: My Contacts");
			test.Selenium.VerifyTextPresent("My Contacts");

			// Verify label
			Assert.AreEqual("Assigned to", test.Selenium.GetText("ctl00_ctl00_MainContent_content_Label1"));

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the edit contact forms page under ministry.")]
        public void Ministry_Contacts_EditContactForms() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to ministry->edit contact forms
			test.Selenium.Navigate(Navigation.Ministry.Contacts.Edit_Contact_Forms);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Edit Contact Forms");
			test.Selenium.VerifyTextPresent("Edit Contact Forms");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the Edit Contact Forms page.")]
        public void Ministry_Contacts_EditContactForms_Edit() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to ministry->edit contact forms
            test.Selenium.Navigate(Navigation.Ministry.Contacts.Edit_Contact_Forms);

            // Edit a contact form
            test.Selenium.ClickAndWaitForPageToLoad("ctl00_ctl00_MainContent_content_grdList_ctl02_lnkContact");

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Contact Form");
            test.Selenium.VerifyTextPresent("Contact Form");

            // Verify back button present
            test.Selenium.VerifyElementPresent(GeneralLinks.Back);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the monitor efficiency page under ministry.")]
        public void Ministry_Contacts_MonitorEfficiency() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to ministry->monitor efficiency
			test.Selenium.Navigate(Navigation.Ministry.Contacts.Monitor_Efficiency);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Monitor Efficiency");
			test.Selenium.VerifyTextPresent("Monitor Efficiency");


			// Search
			test.Selenium.Type("ctl00_ctl00_MainContent_content_ctl02_DateTextBox", "01/01/2005");
			test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.Search);

			// Select an individual
			test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/admin/security/portalusers.aspx')]");

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Add/Edit Portal Users");
            test.Selenium.VerifyTextPresent("Add/Edit Portal Users");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the monitor statistics page under ministry.")]
        public void Ministry_Contacts_MonitorStatistics() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to ministry->monitor statistics
			test.Selenium.Navigate(Navigation.Ministry.Contacts.Monitor_Statistics);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Monitor Statistics");
			test.Selenium.VerifyTextPresent("Monitor Statistics");

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Contacts
        
        #region Activity/Room Setup
        //[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the activities page under ministry.")]
        public void Ministry_ActivityRoomSetup_Activities() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to ministry->activities
			test.Selenium.Navigate(Navigation.Ministry.ActivityRoom_Setup.Activities);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Activities");
			test.Selenium.VerifyTextPresent("Activities");

			// Verify labels
			Assert.AreEqual("Age Attributes", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_pnlEditActivity']/div[1]/fieldset[2]/legend"));

			Assert.AreEqual("Age range", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_pnlEditActivity']/div[1]/fieldset[2]/table/tbody/tr[1]/th"));
			Assert.AreEqual("Extended age range", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_pnlEditActivity']/div[1]/fieldset[2]/table/tbody/tr[2]/th"));

			Assert.AreEqual("Check-in enabled", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_pnlEditActivity']/div[2]/fieldset[1]/table/tbody/tr[1]/td"));
			Assert.AreEqual("Assignments override closed room", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_pnlEditActivity']/div[2]/fieldset[1]/table/tbody/tr[4]/td[2]"));
			Assert.AreEqual("Auto assignments", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_pnlEditActivity']/div[2]/fieldset[1]/table[2]/tbody/tr[1]/td[1]"));

			Assert.AreEqual("Participant assignments", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_pnlEditActivity']/div[2]/fieldset[2]/table/tbody/tr[1]/th"));
			Assert.AreEqual("Vol/Staff assignments", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_pnlEditActivity']/div[2]/fieldset[2]/table/tbody/tr[2]/th"));

			Assert.AreEqual("Include this activity with small group finder", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_pnlEditActivity']/div[1]/fieldset[3]/table/tbody/tr[1]/td"));
			Assert.AreEqual("Contact item *", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_pnlEditActivity']/div[1]/fieldset[3]/table/tbody/tr[2]/th"));

            // Logout of portal
            test.Portal.Logout();
        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the edit activity page.")]
        public void Ministry_ActivityRoomSetup_Activities_Edit() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to ministry->activities
            test.Selenium.Navigate(Navigation.Ministry.ActivityRoom_Setup.Activities);

            // View the edit page for an activity
            test.Selenium.ClickAndWaitForPageToLoad("//table[@id='ctl00_ctl00_MainContent_content_dgActivity']/tbody/tr[*]/td[12]/ul/li[*]/a/span[text()='Edit activity' and normalize-space(ancestor::tr/td[1]/text())='A Test Activity']");

            // Verify user can edit the activity
            Assert.AreEqual("A Test Activity", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_txtActivityName_textBox"));

            // Logout of portal
            test.Portal.Logout();
        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the activity schedules page under ministry.")]
        public void Ministry_ActivityRoomSetup_ActivitySchedules() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Change ministry
			test.Portal.Ministry_ChangeMinistry(DataConstants.Ministry);

			// Navigate to ministry->activity schedules
			test.Selenium.Navigate(Navigation.Ministry.ActivityRoom_Setup.Activity_Schedules);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Activity Schedules");
			test.Selenium.VerifyTextPresent("Activity Schedules");

			// Verify labels, buttons
			Assert.AreEqual("Activity schedule name *", test.Selenium.GetText("//div[@class='box']/table/tbody/tr[2]/th"));
			Assert.AreEqual("Start time *", test.Selenium.GetText("//div[@class='box']/table/tbody/tr[3]/th"));
			Assert.AreEqual("End time *", test.Selenium.GetText("//div[@class='box']/table/tbody/tr[4]/th"));

			Assert.AreEqual("Create new activity schedule name", test.Selenium.GetValue("btnSave"));

			Assert.AreEqual("Show activity", test.Selenium.GetText("//div[@class='data_grid_filters']/table/tbody/tr/td[1]"));

            // Logout of portal
            test.Portal.Logout();
        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the activity rlc groups page under ministry.")]
        public void Ministry_ActivityRoomSetup_ActivityRLCGroups() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Change ministry
			test.Portal.Ministry_ChangeMinistry(DataConstants.Ministry);

			// Navigate to ministry->activity rlc groups
			test.Selenium.Navigate(Navigation.Ministry.ActivityRoom_Setup.Activity_RLC_Groups);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Activity RLC Groups");
			test.Selenium.VerifyTextPresent("Activity RLC Groups");

			// Verify labels
			Assert.AreEqual("Activity grouping name *", test.Selenium.GetText("//div[@class='box']/table/tbody/tr[1]/th"));
			Assert.AreEqual("Balance type", test.Selenium.GetText("//div[@class='box']/table/tbody/tr[2]/th"));
			//Assert.AreEqual("Super group", test.Selenium.GetText("//div[@class='box']/table/tbody/tr[3]/th"));

			Assert.AreEqual("Show activity", test.Selenium.GetText("//form[@id='aspnetForm']/div[5]/div[2]/div[2]/table/tbody/tr/td[1]"));

            // Logout of portal
            test.Portal.Logout();
        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the rooms locations classes page under ministry.")]
        public void Ministry_ActivityRoomSetup_RoomsLocationClasses() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Change ministry
            test.Portal.Ministry_ChangeMinistry(DataConstants.Ministry);

			// Navigate to ministry->rooms, locations & classes
			test.Selenium.Navigate(Navigation.Ministry.ActivityRoom_Setup.Rooms_Locations_Classes);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Rooms, Locations & Classes");
			test.Selenium.VerifyTextPresent("Rooms, Locations & Classes");

			// Verify labels
			Assert.AreEqual("Group/Sub-group", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_tblOuterShell']/div[1]/fieldset[1]/table/tbody/tr[3]/th"));
			Assert.AreEqual("Building/Room", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_tblOuterShell']/div[1]/fieldset[1]/table/tbody/tr[4]/th"));

			Assert.AreEqual("Share contacts with groups", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_tblOuterShell']/div[2]/fieldset[2]/table/tbody/tr[3]/td"));

			Assert.AreEqual("Check-in Best Fit Attributes", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_tblOuterShell']/div[1]/fieldset[2]/legend"));
			Assert.AreEqual("No age restriction", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_tblOuterShell']/div[1]/fieldset[2]/table/tbody/tr[1]/td"));
			Assert.AreEqual("Birth date range", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_tblOuterShell']/div[1]/fieldset[2]/table/tbody/tr[4]/td"));

			// Verify button
			Assert.AreEqual("Add room/location", test.Selenium.GetValue(GeneralButtons.Save));

			// Verify column headers
			Assert.AreEqual("Group", test.Selenium.GetText("//table[@id='ctl00_ctl00_MainContent_content_dgDetails']/tbody/tr/td[2]"));
			Assert.AreEqual("RLC", test.Selenium.GetText("//table[@id='ctl00_ctl00_MainContent_content_dgDetails']/tbody/tr/td[3]"));
			Assert.AreEqual("Room", test.Selenium.GetText("//table[@id='ctl00_ctl00_MainContent_content_dgDetails']/tbody/tr/td[4]"));
			Assert.AreEqual("Cap", test.Selenium.GetText("//table[@id='ctl00_ctl00_MainContent_content_dgDetails']/tbody/tr/td[5]"));
			Assert.AreEqual("Check-in Best Fit", test.Selenium.GetText("//table[@id='ctl00_ctl00_MainContent_content_dgDetails']/tbody/tr/td[6]"));
			Assert.AreEqual("Visible", test.Selenium.GetText("//table[@id='ctl00_ctl00_MainContent_content_dgDetails']/tbody/tr/td[7]"));
			Assert.AreEqual("Override", test.Selenium.GetText("//table[@id='ctl00_ctl00_MainContent_content_dgDetails']/tbody/tr/td[8]"));
			Assert.AreEqual("Auto Open", test.Selenium.GetText("//table[@id='ctl00_ctl00_MainContent_content_dgDetails']/tbody/tr/td[9]"));
			Assert.AreEqual("Active", test.Selenium.GetText("//table[@id='ctl00_ctl00_MainContent_content_dgDetails']/tbody/tr/td[10]"));

            // Logout of portal
            test.Portal.Logout();
        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the breakout groups page under ministry.")]
        public void Ministry_ActivityRoomSetup_BreakoutGroups() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to ministry->breakout groups
			test.Selenium.Navigate(Navigation.Ministry.ActivityRoom_Setup.Breakout_Groups);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Breakout Groups");
			test.Selenium.VerifyTextPresent("Breakout Groups");

			// Verify labels, buttons
			Assert.AreEqual("Breakout group name *", test.Selenium.GetText("//div[@class='grid_16']/div[2]/div/table/tbody/tr[2]/th"));
			Assert.AreEqual("Tag code", test.Selenium.GetText("//div[@class='grid_16']/div[2]/div/table/tbody/tr[4]/th"));

			Assert.AreEqual("Add breakout group", test.Selenium.GetValue(GeneralButtons.Save));

            // Logout of portal
            test.Portal.Logout();
        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the activity requirements page under ministry.")]
        public void Ministry_ActivityRoomSetup_ActivityRequirements() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to ministry->activity requirements
			test.Selenium.Navigate(Navigation.Ministry.ActivityRoom_Setup.Activity_Requirements);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Activity Requirements");
			test.Selenium.VerifyTextPresent("Activity Requirements");

			// Verify buttons
			Assert.AreEqual("Add requirement", test.Selenium.GetValue(GeneralButtons.Save));

            // Logout of portal
            test.Portal.Logout();
        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the group finder properties page under ministry.")]
        public void Ministry_ActivityRoomSetup_GroupFinderProperties() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to ministry->group finder properties
			test.Selenium.Navigate(Navigation.Ministry.ActivityRoom_Setup.Group_Finder_Properties);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Group Finder Properties");
			test.Selenium.VerifyTextPresent("Group Finder Properties");

			// Verify navigation links
			test.Selenium.VerifyElementPresent("link=Properties");
			test.Selenium.VerifyElementPresent("link=Choices");
			test.Selenium.VerifyElementPresent("link=Associations");

			int navigationLinksPositionTop = (int)test.Selenium.GetElementPositionTop("link=Properties");
			Assert.AreEqual(navigationLinksPositionTop, (int)test.Selenium.GetElementPositionTop("link=Choices"));
			Assert.AreEqual(navigationLinksPositionTop, (int)test.Selenium.GetElementPositionTop("link=Associations"));

			// Verify labels
			Assert.AreEqual("Property name *", test.Selenium.GetText("ctl00_ctl00_MainContent_content_lblProperty"));
			Assert.AreEqual("WebLink activity finder", test.Selenium.GetText("ctl00_ctl00_MainContent_content_lblWLActivityFinder"));
			Assert.AreEqual("Include as filter choice", test.Selenium.GetText("//label[@for='ctl00_ctl00_MainContent_content_chkIsFilterChoice']"));
			Assert.AreEqual("Show in results", test.Selenium.GetText("//label[@for='ctl00_ctl00_MainContent_content_chkShowResults']"));
			Assert.AreEqual("Create new property", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_btnCreateNewProperty"));


            // Switch to choices
            test.Selenium.ClickAndWaitForPageToLoad("link=Choices");

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Group Finder Properties");
            test.Selenium.VerifyTextPresent("Group Finder Properties");

            // Verify labels
            Assert.AreEqual("Activity", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_pnlChoices']/div/table/tbody/tr[1]/th/label"));
            Assert.AreEqual("Property choice *", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_pnlChoices']/div/table/tbody/tr[2]/th/label"));

            Assert.AreEqual("Activity property", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_pnlChoices']/table/tbody/tr/th"));

            // Logout of portal
            test.Portal.Logout();
        }
		#endregion Activity/Room Setup
        
        #region Participants
        //Participants hyperlink was removed by FO-4800. 09/24/2015. commented by ivan.zhang.
        //[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the assignments page under ministry.")]
        public void Ministry_Participants_Assignments() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to ministry->assignments
			test.Selenium.Navigate(Navigation.Ministry.Participants.Assignments);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Participant Assignments");
			test.Selenium.VerifyTextPresent("Participant Assignments");

			// Verify link
			test.Selenium.VerifyElementPresent("link=Find person");

			// Check the text for the breakout group label
			Assert.AreEqual("Breakout group", test.Selenium.GetText("//span[@id='ctl00_ctl00_MainContent_content_ctl01_smallGroupLabel']"));

			// Check the text on the button for adding an assignment
			Assert.AreEqual("Add assignment", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_btnSubmit"));

            // Logout of portal
			test.Portal.Logout();
		}

        //Participants hyperlink was removed by FO-4800. 09/24/2015. commented by ivan.zhang.
        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the manage assignments page under ministry.")]
        public void Ministry_Participants_ManageAssignments() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Select ministry->manage assignments
			test.Selenium.Navigate(Navigation.Ministry.Participants.Manage_Assignments);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Manage Assignments");
			test.Selenium.VerifyTextPresent("Manage Assignments");

			// Verify labels
			Assert.AreEqual("Choose an Assignment", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_pnlSelection']/div[1]/h3"));

			Assert.AreEqual("Activity schedule", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_pnlSelection']/div[1]/div/table/tbody/tr[3]/th"));
			Assert.AreEqual("Activity schedule", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_pnlSelection']/div[2]/div/table/tbody/tr[3]/th"));

			Assert.AreEqual("Breakout group", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_pnlSelection']/div[1]/div/table/tbody/tr[5]/th"));
			Assert.AreEqual("Breakout group", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_pnlSelection']/div[2]/div/table/tbody/tr[5]/th"));

			Assert.AreEqual("Last move", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_pnlSelection']/div[1]/div/table/tbody/tr[6]/th"));

			Assert.AreEqual((int)test.Selenium.GetElementPositionTop("ctl00_ctl00_MainContent_content_ddlLastMove"), (int)test.Selenium.GetElementPositionTop("ctl00_ctl00_MainContent_content_textLastMoveDate"));

			Assert.AreEqual("Found Assignments: 0", test.Selenium.GetText("//div[@id='ctl00_ctl00_MainContent_content_pnlSelection']/div[2]/h3"));

			// Verify button text
			Assert.AreEqual("Review", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_btnSubmit"));

            // Logout of portal
			test.Portal.Logout();
		}
        #endregion Participants

        #region Volunteers
        //Volunteers Legacy hyperlink was removed by FO-4800. 09/24/2015. commented by ivan.zhang.
        //[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the manage assignments page under ministry.")]
        public void Ministry_Volunteers_StaffingAssignment() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to ministry->staffing assignment
			test.Selenium.Navigate(Navigation.Portal.Ministry.Volunteers.Staffing_Assignment);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Staffing Assignment");
			test.Selenium.VerifyTextPresent("Staffing Assignment");

			// Verify labels
			Assert.AreEqual("Volunteer type", test.Selenium.GetText("ctl00_ctl00_MainContent_content_Addeditstaff1_lblStaffType"));
			Assert.AreEqual("Activity RLC grouping", test.Selenium.GetText("ctl00_ctl00_MainContent_content_Addeditstaff1_lblArea"));

            // Logout of portal
			test.Portal.Logout();
		}

        // Volunteers hyperlink was removed by FO-4800. 09/24/2015. commented by ivan.zhang
		//[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the jobs page under ministry.")]
        public void Ministry_Volunteers_Jobs() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to ministry->jobs
			test.Selenium.Navigate(Navigation.Ministry.Volunteers.Jobs);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Jobs");
			test.Selenium.VerifyTextPresent("Jobs");


			// Click to create a new job
			test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Add);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Manage Jobs");
			test.Selenium.VerifyTextPresent("Manage Jobs");

			// Verify back button present
			test.Selenium.VerifyElementPresent(GeneralLinks.Back);

			// Verify labels
			Assert.AreEqual("Contact person", test.Selenium.GetText("//form[@id='aspnetForm']/div[3]/fieldset/table/tbody/tr[2]/th"));
			Assert.AreEqual("Reports to", test.Selenium.GetText("//form[@id='aspnetForm']/div[3]/fieldset/table/tbody/tr[3]/th"));
			Assert.AreEqual("Default time served", test.Selenium.GetText("//form[@id='aspnetForm']/div[3]/fieldset/table/tbody/tr[4]/th"));

			// Verify button
			Assert.AreEqual("Add job", test.Selenium.GetValue(GeneralButtons.Save));


			// View the summary of an existing job
			// Navigate to ministry->jobs
			test.Selenium.Navigate(Navigation.Ministry.Volunteers.Jobs);

			test.Selenium.ClickAndWaitForPageToLoad("//table[@id='ctl00_ctl00_MainContent_content_dgJobs']/tbody/tr[2]/td[1]/a");

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Job Summary");
			test.Selenium.VerifyTextPresent("Job Summary");
			test.Selenium.VerifyTextPresent("Job Information");
			test.Selenium.VerifyTextPresent("Job Attributes");
			test.Selenium.VerifyTextPresent("Job Restrictions");
			
            // Logout of portal
			test.Portal.Logout();
		}

        // Volunteers hyperlink was removed by FO-4800. 09/24/2015. commented by ivan.zhang
        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the schedules page under ministry.")]
        public void Ministry_Volunteers_Schedules() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to ministry->schedules
			test.Selenium.Navigate(Navigation.Ministry.Volunteers.Schedules);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Staff Schedules");
			test.Selenium.VerifyTextPresent("Staff Schedules");

			// Verify labels
			Assert.AreEqual("Schedule name *", test.Selenium.GetText("ctl00_ctl00_MainContent_content_AddEditSchedule1_lblSchedule"));
			Assert.AreEqual("Create new schedule", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_AddEditSchedule1_btnSave"));

			Assert.AreEqual("Show activity", test.Selenium.GetText("//form[@id='aspnetForm']/div[5]/div[2]/fieldset/table[1]/tbody/tr/td[1]"));

            // Logout of portal
			test.Portal.Logout();
		}

        // Volunteers hyperlink was removed by FO-4800. 09/24/2015. commented by ivan.zhang
		//[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the vso funnel report page under ministry.")]
        public void Ministry_Volunteers_VSOFunnelReport() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to ministry->vso funnel report
			test.Selenium.Navigate(Navigation.Ministry.Volunteers.VSO_Funnel_Report);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: VSO Funnel Report");
			test.Selenium.VerifyTextPresent("VSO Funnel Report");

			// Verify labels
			Assert.AreEqual("Ministry Selection", test.Selenium.GetText("//form[@id='aspnetForm']/fieldset[1]/legend"));
			Assert.AreEqual("Ministry *", test.Selenium.GetText("//form[@id='aspnetForm']/fieldset[1]/table/tbody/tr[1]/td/label"));
			Assert.AreEqual("Date range *", test.Selenium.GetText("//label[@for='ctl00_ctl00_MainContent_content_dtbStartDate']"));

			Assert.AreEqual("Job Attribute Selection *", test.Selenium.GetText("//form[@id='aspnetForm']/fieldset[2]/legend"));

			// Verify button
			Assert.AreEqual("View report", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_btnView"));


			// Click to view a report
			test.Selenium.ClickAndWaitForPageToLoad("ctl00_ctl00_MainContent_content_btnView");

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: VSO Funnel Report");
			test.Selenium.VerifyTextPresent("VSO Funnel Report");

			// Verify error text
			test.Selenium.VerifyTextPresent(TextConstants.ErrorHeadingPlural);
			test.Selenium.VerifyTextPresent("Please select at least one ministry");
			test.Selenium.VerifyTextPresent("Please select at least one job attribute");
			
            // Logout of portal
			test.Portal.Logout();
		}
        #endregion Volunteers

		#region Attendance
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the post attendance page under ministry.")]
        public void Ministry_Attendance_PostAttendance() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to ministry->post attendance
			test.Selenium.Navigate(Navigation.Ministry.Attendance.Post_Attendance);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Post Attendance");
			test.Selenium.VerifyTextPresent("Post Attendance");

			// Verify labels
			Assert.AreEqual("Activity *", test.Selenium.GetText("//label[@for='ctl00_ctl00_MainContent_content_ddlActivity_dropDownList']"));
			Assert.AreEqual("Schedule *", test.Selenium.GetText("//label[@for='ctl00_ctl00_MainContent_content_ddlSchedule_dropDownList']"));
			Assert.AreEqual("Date range", test.Selenium.GetText("//label[@for='ctl00_ctl00_MainContent_content_dtMYRange']"));
			Assert.AreEqual("Time *", test.Selenium.GetText("//label[@for='ctl00_ctl00_MainContent_content_ddlActivityTime_dropDownList']"));

            // Logout of portal
			test.Portal.Logout();
		}

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the head count page under ministry.")]
        public void Ministry_Attendance_HeadCount() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to ministry->headcount
			test.Selenium.Navigate(Navigation.Ministry.Attendance.Head_Count);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Head Count");
			test.Selenium.VerifyTextPresent("Head Count");

			// Verify label
			Assert.AreEqual("Date/time", test.Selenium.GetText("//table[@class='horiz form_element']/tbody/tr/td[2]/label"));

			// Verify table column headers
			//Assert.AreEqual("Room/Location/Class", test.Selenium.GetText("//table[@class='grid']/tbody/tr/th[1]"));
            Assert.AreEqual("Roster", test.Selenium.GetText("//table[@class='grid']/tbody/tr/th[1]")); // Added by Winnie Wang
			Assert.AreEqual("Head Count", test.Selenium.GetText("//table[@class='grid']/tbody/tr/th[2]"));

            // Logout of portal
			test.Portal.Logout();
		}

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the automated attendance rules page under ministry.")]
        public void Ministry_Attendance_AutomatedAttendanceRules() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to ministry->automated attendance rules
			test.Selenium.Navigate(Navigation.Ministry.Attendance.Automated_Attendance_Rules);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Automated Attendance Rules");
			test.Selenium.VerifyTextPresent("Automated Attendance Rules");


			// ** Add
			// Navigate to ministry->automated attendance rules
			test.Selenium.Navigate(Navigation.Ministry.Attendance.Automated_Attendance_Rules);

			// Add a new attendance rule
			test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Add);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Add/Edit Automatic Attendance Rule");
			test.Selenium.VerifyTextPresent("Add/Edit Automatic Attendance Rule");

			// Verify back button present
			test.Selenium.VerifyElementPresent(GeneralLinks.Back);

			// Verify column header
			Assert.Contains(test.Selenium.GetText("//table[@class='grid']/tbody/tr[1]/th[2]"), "Generate Attendance For");


			// ** Edit
			// Navigate to ministry->automated attendance rules
			test.Selenium.Navigate(Navigation.Ministry.Attendance.Automated_Attendance_Rules);

			// Edit an existing rule
			test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Edit);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Add/Edit Automatic Attendance Rule");
			test.Selenium.VerifyTextPresent("Add/Edit Automatic Attendance Rule");

			// Verify back button present
			test.Selenium.VerifyElementPresent(GeneralLinks.Back);
			
            // Logout of portal
			test.Portal.Logout();
		}
        #endregion Attendance

		#region Check-In
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the live check-ins page under ministry.")]
        public void Ministry_Checkin_LiveCheckins() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to ministry->live check-ins
			test.Selenium.Navigate(Navigation.Ministry.Checkin.Live_Checkins);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Live Check-ins");
			test.Selenium.VerifyTextPresent("Live Check-ins");

            // Logout of portal
			test.Portal.Logout();
		}

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the super check-ins page under ministry.")]
        public void Ministry_Checkin_SuperCheckins() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to ministry->super checkins
			test.Selenium.Navigate(Navigation.Ministry.Checkin.Super_Checkins);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Super Check-ins");
			test.Selenium.VerifyTextPresent("Super Check-ins");

			// Verify add link
			Assert.AreEqual("Add", test.Selenium.GetText("//div[@class='grid_16']/div[2]/div/a"));

			// Verify table headers
			Assert.AreEqual("Super Check-in Name", test.Selenium.GetText("//table[@class='grid']/tbody/tr/th[2]"));

			// Verify location of edit, add activity links
            // Changed by Mady, modified the Xpath to find the first table line value rather than the 2nd line
			Assert.AreEqual("Edit", test.Selenium.GetText("//table[@class='grid']/tbody/tr[2]/td[1]/a"));
			Assert.AreEqual("Add activity", test.Selenium.GetText("//table[@class='grid']/tbody/tr[2]/td[4]/a"));
            
			// ** Create new super check-in
			// Create a new super checkin
			test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Add);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Super Check-in");
			test.Selenium.VerifyTextPresent("Super Check-in");

			// Verify text on add, remove buttons
			Assert.AreEqual("Add >>", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_MakeGlobalAssociations1_btnAdd"));
			Assert.AreEqual("<< Remove", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_MakeGlobalAssociations1_btnRemove"));

            // Logout of portal
			test.Portal.Logout();
		}

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the theme manager page under ministry.")]
        public void Ministry_Checkin_ThemeManager() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to ministry->theme manager
			test.Selenium.Navigate(Navigation.Ministry.Checkin.Theme_Manager);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Theme Manager");
			test.Selenium.VerifyTextPresent("Theme Manager");


			// ** Create a new theme
			// Create a new theme
			test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Add);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Theme Overview");
			test.Selenium.VerifyTextPresent("Theme Overview");


			// ** Back
			// Click the back button
			test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Back);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Theme Manager");
			test.Selenium.VerifyTextPresent("Theme Manager");


			// ** Preview
			// Navigate to ministry->theme manager
			test.Selenium.Navigate(Navigation.Ministry.Checkin.Theme_Manager);

			// Preview a theme
			test.Selenium.ClickAndWaitForPageToLoad(ThemeManagerConstants.Preview);

			// Verify title, links
			test.Selenium.VerifyTitle("Fellowship One");
			test.Selenium.VerifyElementPresent("link=Background image");
			test.Selenium.VerifyElementPresent("link=Welcome screen");
			test.Selenium.VerifyElementPresent("link=Confirmation screen");

			// Return to the theme list page
			test.Selenium.ClickAndWaitForPageToLoad("//a[@id='ctl00_lnkBackto']/span[2]");


			// ** Welcome screen
			// Preview a theme
			test.Selenium.ClickAndWaitForPageToLoad(ThemeManagerConstants.Preview);

			// View the welcome screen
            test.Selenium.Click("link=Welcome screen");

			// Verify title, links
			test.Selenium.VerifyTitle("Fellowship One");
			test.Selenium.VerifyElementPresent("link=Background image");
			test.Selenium.VerifyElementPresent("link=Welcome screen");
			test.Selenium.VerifyElementPresent("link=Confirmation screen");

			// Return to the theme list page
			test.Selenium.ClickAndWaitForPageToLoad("//a[@id='ctl00_lnkBackto']/span[2]");


			// ** Confirmation screen
			// Preview a theme
			test.Selenium.ClickAndWaitForPageToLoad(ThemeManagerConstants.Preview);

			// View the welcome screen
            test.Selenium.Click("link=Confirmation screen");

			// Verify title, links
			test.Selenium.VerifyTitle("Fellowship One");
			test.Selenium.VerifyElementPresent("link=Background image");
			test.Selenium.VerifyElementPresent("link=Welcome screen");
			test.Selenium.VerifyElementPresent("link=Confirmation screen");

			// Return to the theme list page
			test.Selenium.ClickAndWaitForPageToLoad("//a[@id='ctl00_lnkBackto']/span[2]");

            // Logout of portal
            test.Portal.Logout();
        }
		#endregion Check-In
	}
}