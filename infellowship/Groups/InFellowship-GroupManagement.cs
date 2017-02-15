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

namespace FTTests.infellowship.Groups {
    [TestFixture]
    public class infellowship_Groups_GroupManagement : FixtureBase {

        #region Fixture Setup and Teardown

        string _groupTypeName = "Group Management Group Type";
        string _groupName = "General Group";
        string _spanOfCareName = "InFellowship Group Management SOC";

        [FixtureSetUp]
        public void FixtureSetUp() {
            // Group Type
            base.SQL.Groups_SpanOfCare_Delete(254, _spanOfCareName);
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", _groupTypeName, new List<int> { 1, 2, 3, 4, 5, 9, 10, 11, 12 });

            // Create a generic group and add a leader and member
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "InFellowship User", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Member", "Member");
            base.SQL.Groups_SpanOfCare_Create(254, _spanOfCareName, "Bryan Mikaelian", "SOC Owner", new List<string>() { _groupTypeName });
        }

        [FixtureTearDown]
        public void FixtureTearDown() {
            //base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
            //base.SQL.Groups_SpanOfCare_Delete(254, _spanOfCareName);
        }

        #endregion Fixture Setup and Teardown


        #region Update Bulletin Board
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a leader can perform various updates to the bulletin board to display it on the dashboard and update the date/time.")]
        public void GroupManagement_UpdateBulletinBoard_Leader() {
            // Create the data / remove any left over data
            var groupTypeName = _groupTypeName;
            var groupName = "Bulletin Board Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Bryan Mikaelian", "Leader");

            // Login to infellowship - Groups
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // Update the bulletin board and publish
            test.infellowship.Groups_Group_Update_BulletinBoard(groupName, "I made an update to the bulletin board ", "Bryan Mikaelian", true);

            // Verify the bulletin board update is published to the dashboard
            test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
            test.Selenium.VerifyTextPresent("I made an update to the bulletin board");

            // Unpublish the bulletin board
            test.infellowship.Groups_Group_Update_BulletinBoard(groupName, "I made another update to the bulletin board", "Bryan Mikaelian", false);

            // Verify it is not present on the dashboard
            test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
            Assert.IsFalse(test.Selenium.IsTextPresent("I made another update to the bulletin board"), "Unpublished bulletin board message was published!");

            // Logout
            test.infellowship.Logout();

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a leader can update the bulletin board via the wrench on the dashboard.")]
        public void GroupManagement_UpdateBulletinBoard_Wrench_Leader() {
            // Create the data / remove any left over data
            var groupTypeName = _groupTypeName;
            var groupName = "Wrench Bulletin Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Bryan Mikaelian", "Leader");

            // Login to infellowship - Groups
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // Update the bulletin board and publish
            test.infellowship.Groups_Group_Update_BulletinBoard_Wrench(groupName, "I made an update to the bulletin board ", "Bryan Mikaelian", true);

            // Verify the bulletin board update is published to the dashboard
            test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
            test.Selenium.VerifyTextPresent("I made an update to the bulletin board");

            // Unpublish the bulletin board
            test.infellowship.Groups_Group_Update_BulletinBoard_Wrench(groupName, "I made another update to the bulletin board", "Bryan Mikaelian", false);

            // Verify it is not present on the dashboard
            test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
            Assert.IsFalse(test.Selenium.IsTextPresent("I made another update to the bulletin board"), "Unpublished bulletin board message was published!");

            // Logout
            test.infellowship.Logout();

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you are taken to the Edit Bulletin Board page from the Settings page after clicking on the link, while logged in")]
        public void GroupManagement_UpdateBulletinBoard_View_Page_From_Settings_Leader() {
            // Login to infellowship - Groups
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.infellowship.Groups_Group_View_Settings(_groupName);

            // Click on the Edit Bulletin Board link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard);

            // Verify elements are present
            Assert.IsTrue(test.Selenium.IsTextPresent("Bulletin Board"));
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.SettingsPage.UpdateBulletinBoardPage.TextField_BulletinBoardPost));
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.SettingsPage.UpdateBulletinBoardPage.CheckBox_Publish));

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you are taken to the Settings page after clicking on the Cancel link for Update Bulletin Board.")]
        public void GroupManagement_UpdateBulletinBoard_Cancel_To_Settings_Leader() {
            // Login to infellowship - Groups
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.infellowship.Groups_Group_View_Settings(_groupName);

            // Click on the Edit Bulletin Board link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard);

            // Click the Cancel Link 
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.UpdateBulletinBoardPage.Link_Cancel_BulletinBoard);

            // Verify you are on the Settings page
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Roster);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);

            test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName));

            // sidebar links
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.Link_EditDetails);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditSchedule);

            // Headers
            test.Selenium.VerifyTextPresent("Settings");
            test.Selenium.VerifyTextPresent("Bulletin board");
            test.Selenium.VerifyTextPresent("Details");
            test.Selenium.VerifyTextPresent("Schedule");
            test.Selenium.VerifyTextPresent("Location");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a span of care owner can perform various updates to the bulletin board to display it on the dashboard and update the date/time.")]
        public void GroupManagement_UpdateBulletinBoard_Owner() {
            // Create the data / remove any left over data
            var socName = _spanOfCareName;
            var groupTypeName = _groupTypeName;
            var groupName = "Bulletin Board Owner Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Bryan Mikaelian", "Leader");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Update the bulletin board and publish
            test.infellowship.Groups_Group_Update_BulletinBoard(socName, groupName, "I made an update to the bulletin board ", "SOC Owner", true, "QAEUNLX0C2");

            // Verify the bulletin board update is published to the dashboard
            test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
            test.Selenium.VerifyTextPresent("I made an update to the bulletin board");

            test.infellowship.Logout();

            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            // Unpublish the bulletin board
            //modify by grace zhang:add timezone for church
            test.infellowship.Groups_Group_Update_BulletinBoard(socName, groupName, "I made another update to the bulletin board", "SOC Owner", false, "QAEUNLX0C2");
            
            // Verify it is not present on the dashboard
            test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
            Assert.IsFalse(test.Selenium.IsTextPresent("I made another update to the bulletin board"), "Unpublished bulletin board message was published!");

            // Logout
            test.infellowship.Logout();

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a span of care owner can update the bulletin board via the wrench on the dashboard.")]
        public void GroupManagement_UpdateBulletinBoard_Wrench_Owner() {
            // Create the data / remove any left over data
            var socName = _spanOfCareName;
            var groupTypeName = _groupTypeName;
            var groupName = "Wrench Bulletin Owner Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Bryan Mikaelian", "Leader");

            // Login to infellowship - Groups
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Update the bulletin board and publish
            test.infellowship.Groups_Group_Update_BulletinBoard(socName, groupName, "I made an update to the bulletin board ", "SOC Owner", true);

            // Verify the bulletin board update is published to the dashboard
            test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
            test.Selenium.VerifyTextPresent("I made an update to the bulletin board");

            // Unpublish the bulletin board
            test.infellowship.Groups_Group_Update_BulletinBoard_Wrench(socName, groupName, "I made another update to the bulletin board", "SOC Owner", false);
            
            // Verify it is not present on the dashboard
            test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
            Assert.IsFalse(test.Selenium.IsTextPresent("I made another update to the bulletin board"), "Unpublished bulletin board message was published!");

            // Logout
            test.infellowship.Logout();

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);    
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you, a span of care owner, are taken to the Edit Bulletin Board page from the Settings page after clicking on the link, while logged in")]
        public void GroupManagement_UpdateBulletinBoard_View_Page_From_Settings_Owner() {
            // Login to infellowship - Groups
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.infellowship.Groups_Group_View_Settings(_spanOfCareName, _groupName);

            // Click on the Edit Bulletin Board link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard);

            // Verify elements are present
            Assert.IsTrue(test.Selenium.IsTextPresent("Bulletin Board"));
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.SettingsPage.UpdateBulletinBoardPage.TextField_BulletinBoardPost));
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.SettingsPage.UpdateBulletinBoardPage.CheckBox_Publish));

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you, a span of care owner, are taken to the Settings page after clicking on the Cancel link for Update Bulletin Board.")]
        public void GroupManagement_UpdateBulletinBoard_Cancel_To_Settings_Owner() {
            // Login to infellowship - Groups
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.infellowship.Groups_Group_View_Settings(_spanOfCareName, _groupName);

            // Click on the Edit Bulletin Board link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard);

            // Click the Cancel Link 
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.UpdateBulletinBoardPage.Link_Cancel_BulletinBoard);

            // Verify you are on the Settings page
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Roster);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);

            test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName));

            // sidebar links
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.Link_EditDetails);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditSchedule);

            // Headers
            test.Selenium.VerifyTextPresent("Settings");
            test.Selenium.VerifyTextPresent("Bulletin board");
            test.Selenium.VerifyTextPresent("Details");
            test.Selenium.VerifyTextPresent("Schedule");
            test.Selenium.VerifyTextPresent("Location");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("Bug FO-3435:Verifies  When using < > markdown language in bulletin board ,can update.")]
        public void GroupManagement_UpdateBulletinBoard_UsingMarkdown()
        {
            // Create the data / remove any left over data
            var groupTypeName = _groupTypeName;
            var groupName = "Bulletin Board Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Bryan Mikaelian", "Leader");

            // Login to infellowship - Groups
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // Unpublish the bulletin board
            test.infellowship.Groups_Group_Update_BulletinBoard_UsingMarkdown(groupName, "Email pastor Dan at <pastordan@ourchurch.org>", "Email pastor Dan at pastordan@ourchurch.org", "Bryan Mikaelian", false);
                       
            // Logout
            test.infellowship.Logout();

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);
        }
        #endregion Update Bulletin Board

        #region Schedule Management

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a leader can create a one time event and delete it.")]
        public void GroupManagement_Create_Schedule_OneTimeEvent_Leader() {
            // Create the data / remove any left over data
            var groupTypeName = _groupTypeName;
            var groupName = "New One Time Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Create a schedule
            test.infellowship.Groups_Group_Create_Schedule_OneTimeEvent(groupName, "10/31/2050", "10:30 PM", null, false);;

            // Delete the schedule
            test.infellowship.Groups_Group_Delete_Schedule(groupName, true);

            // Logout
            test.infellowship.Logout();

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);

        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a group can be searched if its one time event schedule is deleted.")]
        public void GroupManagement_Create_Schedule_OneTimeEvent_Group_Searchable_After_Delete() {
            // Create the data / remove any left over data
            var groupTypeName = _groupTypeName;
            var groupName = "New One Time Search Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Create a schedule that occured in the past
            test.infellowship.Groups_Group_Create_Schedule_OneTimeEvent(groupName, "10/31/2009", "10:30 PM", null, false);

            // Delete the schedule
            test.infellowship.Groups_Group_Delete_Schedule(groupName, true);

            // Find a group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup);

            // Search for all groups     
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify the group is present
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));

            // Logout of infellowship
            test.infellowship.Logout();

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the required fields for creating a schedule")]
        public void GroupManagement_Create_Schedule_Required_Fields_Leader() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.infellowship.Groups_Group_View_Settings(_groupName);

            // Click on the Create Schedule sidebar Link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditSchedule);

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.CreateEditSchedulePage.Button_CreateSchedule);

            // Verify error message
            Assert.IsTrue(test.Selenium.IsTextPresent("The start date is in an invalid format."));
            Assert.IsTrue(test.Selenium.IsTextPresent("The start time is in an invalid format."));

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you are taken to the Create Schedule page after clicking on the sidebar link on the Settings Page.")]
        public void GroupManagement_Create_Schedule_View_From_Settings_Sidebar_Link_Leader() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.infellowship.Groups_Group_View_Settings(_groupName);

            // Click on the Create Schedule sidebar Link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditSchedule);

            // Verify you are on the Create Schedule page
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditSchedulePage.DateControl_StartDate);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditSchedulePage.TextField_StartMeetingTime);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditSchedulePage.RadioButton_OneTimeEvent);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditSchedulePage.RadioButton_WeeklyEvent);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditSchedulePage.RadioButton_MonthlyEvent);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditSchedulePage.CheckBox_NotifyMembers);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you are taken back to the Settings page after clicking on the cancel link on the Create Schedule Page.")]
        public void GroupManagement_Create_Schedule_Cancel_Leader() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.infellowship.Groups_Group_View_Settings(_groupName);

            // Click on the Create Schedule sidebar Link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditSchedule);

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.CreateEditSchedulePage.Link_Cancel_CreateSchedule);

            // Verify you are on the Settings page
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Roster);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);

            test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName));

            // sidebar links
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.Link_EditDetails);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditSchedule);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditLocation);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a span of care owner can create a one time event and delete it.")]
        public void GroupManagement_Create_Schedule_OneTimeEvent_Owner() {
            // Create the data / remove any left over data
            var socName = _spanOfCareName;
            var groupTypeName = _groupTypeName;
            var groupName = "New One Time Owner Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Create a schedule
            test.infellowship.Groups_Group_Create_Schedule_OneTimeEvent(socName, groupName, "10/31/2050", "10:30 PM", null, false);

            // Delete the schedule
            test.infellowship.Groups_Group_Delete_Schedule(socName, groupName, false);

            // Logout
            test.infellowship.Logout();

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the required fields for creating a schedule as an owner.")]
        public void GroupManagement_Create_Schedule_Required_Fields_Owner() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.infellowship.Groups_Group_View_Settings(_spanOfCareName, _groupName);

            // Click on the Create Schedule sidebar Link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditSchedule);

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.CreateEditSchedulePage.Button_CreateSchedule);

            // Verify error message
            Assert.IsTrue(test.Selenium.IsTextPresent("The start date is in an invalid format."));
            Assert.IsTrue(test.Selenium.IsTextPresent("The start time is in an invalid format."));

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you, a span of care owner, are taken to the Create Schedule page after clicking on the sidebar link on the Settings Page.")]
        public void GroupManagement_Create_Schedule_View_From_Settings_Sidebar_Link_Owner() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.infellowship.Groups_Group_View_Settings(_spanOfCareName, _groupName);

            // Click on the Create Schedule sidebar Link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditSchedule);

            // Verify you are on the Create Schedule page
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditSchedulePage.DateControl_StartDate);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditSchedulePage.TextField_StartMeetingTime);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditSchedulePage.RadioButton_OneTimeEvent);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditSchedulePage.RadioButton_WeeklyEvent);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditSchedulePage.RadioButton_MonthlyEvent);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditSchedulePage.CheckBox_NotifyMembers);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you, a span of care owner, are taken back to the Settings page after clicking on the cancel link on the Create Schedule Page.")]
        public void GroupManagement_Create_Schedule_Cancel_Owner() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.infellowship.Groups_Group_View_Settings(_spanOfCareName, _groupName);

            // Click on the Create Schedule sidebar Link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditSchedule);

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.CreateEditSchedulePage.Link_Cancel_CreateSchedule);

            // Verify you are on the Settings page
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Roster);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);

            test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName));

            // sidebar links
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.Link_EditDetails);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditSchedule);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditLocation);

            // Logout
            test.infellowship.Logout();
        }
        [Test, RepeatOnFailure, Timeout(1000)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("FO-7074:Cannot edit group monthly schedule in Infellowship.")]
        public void GroupManagement_Edit_GroupMonthlySchedule()
        {
            // Create the data / remove any left over data
            var groupTypeName = _groupTypeName;
            var groupName = "New One Time Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Create a schedule
            test.infellowship.Groups_Group_Create_Schedule_OneTimeEvent(groupName, "10/31/2050", "10:30 PM", null, false); ;

            //Edit Schedule: change to a monthly schedule
            test.infellowship.Groups_Group_View_Settings(groupName);
            test.Selenium.ClickAndWaitForPageToLoad("link=Edit schedule");

            test.Selenium.Click("recurrence_monthly");

            test.Selenium.Click("recurrence_monthly_first");
            test.Selenium.Click("recurrence_monthly_3rd");
            test.Selenium.Select("recurrence_monthly_weekday", "Monday");
            test.Selenium.Select("recurrence_monthly_nth_month", "month .");

            //Verify: modify successfully
            test.Selenium.ClickAndWaitForPageToLoad("submitQuery");
            test.Selenium.Click("success_close");

            // Delete the schedule
            test.infellowship.Groups_Group_Delete_Schedule(groupName, true);

            // Logout
            test.infellowship.Logout();

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);

        }
        #endregion Schedule Management

        #region Location Management
        [Test, MultipleAsserts, RepeatOnFailure, DoesNotRunOutsideOfStaging]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a physical private location can be created for a group from the InFellowship side and this group will appear in the search results after doing a proximity search in InFellowship.")]
        public void GroupManagement_Create_Location_Physical_Leader_Private() {
            // Create the data / remove any left over data
            var groupTypeName = _groupTypeName;
            var groupName = "Leader Private Physical Loc Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Bryan Mikaelian", "Leader");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // Create a location for the group. Make it private.
            test.infellowship.Groups_Group_Create_Location_Physical(groupName, "Test Location", null, "2812 Meadow Wood Drive", "Flower Mound", "Texas", "75022", true);

            // Find a group by zip code
            test.infellowship.Groups_FindAGroup_Search(null, null, null, false);

            // Verify your group is returned
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));

            // View the group
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", groupName));

            // Verify the address is not present since it is private
            Assert.IsFalse(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"), "Private location was present on the group's public page!");

            // Logout
            test.infellowship.Logout();

            // Find a group in InFellowship while not logged in
            test.infellowship.Groups_FindAGroup_Search(null, null, null, false);

            // Verify your group is returned
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));

            // View the group
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", groupName));

            // Verify the address is not present since it is private
            Assert.IsFalse(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"), "Private location was present on the group's public page!");

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        [Test, MultipleAsserts, RepeatOnFailure, DoesNotRunOutsideOfStaging]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a physical location can be created for a group from the InFellowship side and this group will appear in the search results after doing a proximity search in InFellowship.")]
        public void GroupManagement_Create_Location_Physical_Leader() {
            // Create the data / remove any left over data
            var groupTypeName = _groupTypeName;
            var groupName = "Leader Physical Loc Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Bryan Mikaelian", "Leader");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // Create a location for the group
            test.infellowship.Groups_Group_Create_Location_Physical(groupName, "Test Location", null, "2812 Meadpw Wood Drive", "Flower Mound", "Texas", "75022", false);

            // Find a group by zip code
            test.infellowship.Groups_FindAGroup_Search("75287", null, null, false);

            // Verify your group is returned
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));

            // Logout
            test.infellowship.Logout();

            // Find a group in InFellowship while not logged in.
            test.infellowship.Groups_FindAGroup_Search("75287", null, null, false);

            // Verify your group is returned
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        [Test, MultipleAsserts, RepeatOnFailure, DoesNotRunOutsideOfStaging]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that an online location can be created for a group from the InFellowship side and verifies it is not in the search results if a zip code is provided.")]
        public void GroupManagement_Create_Location_Online_Leader() {
            // Create the data / remove any left over data
            var groupTypeName = _groupTypeName;
            var groupName = "Leade Online Loc Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Bryan Mikaelian", "Leader");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // Create an online location for the group
            test.infellowship.Groups_Group_Create_Location_Online(groupName, "Test Location", null, "http://www.google.com");

            // Find a group in InFellowship while not logged in.
            TestLog.WriteLine("Find Group Search using ZIP Code");
            test.infellowship.Groups_FindAGroup_Search("78570", null, null, false);

            // Verify your group is not returned
            TestLog.WriteLine(string.Format("{0} should not be found", _groupName));
            Assert.IsFalse(test.Selenium.IsElementPresent(string.Format("link={0}", groupName)), "A group with an online location was returned when searching by zip code!");


            TestLog.WriteLine("Find Group Search NO ZIP Code");
            // Find a group again without a zip code.
            test.infellowship.Groups_FindAGroup_Search(null, null, null, false);

            // Verify your group is returned
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));

            // Logout
            test.infellowship.Logout();

            // Search for all groups by a zipcode 
            test.infellowship.Groups_FindAGroup_Search("78570", null, null, false);

            // Verify your group is not returned
            Assert.IsFalse(test.Selenium.IsElementPresent(string.Format("link={0}", groupName)), "A group with an online location was returned when searching by zip code!");

            // Find a group again without a zip code.
            test.infellowship.Groups_FindAGroup_Search(null, null, null, false);

            // Verify your group is returned
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);

        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the controls are present if you switch from an online location to a physical location")]
        public void GroupManagement_Create_Location_Controls_Present_After_Switching_Between_Online_And_Physical_For_Leader() {
            // Login to infellowship - Groups
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.infellowship.Groups_Group_View_Settings(_groupName);

            // Click on the Create Location sidebar Link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditLocation);

            // Select online
            test.Selenium.Click(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.RadioButton_MeetsOnline);

            // Select meets in person
            test.Selenium.Click(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.RadioButton_MeetsInPerson);

            // Verify the controls are present for a physical location
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.DropDown_Country));
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.TextField_AddressOne));
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.TextField_AddressTwo));
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.TextField_City));
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.DropDown_State));
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.TextField_ZipCode));
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.TextField_County));

            // Log out
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the required fields for a 'Meets in Person' location.")]
        public void GroupManagement_Create_Location_Physical_Required_Fields_Leader() {
            // Login to infellowship - Groups
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.infellowship.Groups_Group_View_Settings(_groupName);

            // Click on the Create Location sidebar Link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditLocation);

            // Select meets in person
            test.Selenium.Click(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.RadioButton_MeetsInPerson);

            // Clear out the name
            test.Selenium.Type(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.TextField_LocationName, "");

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.Button_CreateLocation);

            // Verify text
            test.Selenium.VerifyTextPresent("Name is required.");

            // Enter a name
            test.Selenium.Type(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.TextField_LocationName, "My Location");

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.Button_CreateLocation);

            // Verify text
            test.Selenium.VerifyTextPresent("Address 1 is required");
            test.Selenium.VerifyTextPresent("State is required.");
            test.Selenium.VerifyTextPresent("Postal Code is required.");
            test.Selenium.VerifyTextPresent("City is required.");

            // Log out
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the required fields for an 'Online' location.")]
        public void GroupManagement_Create_Location_Online_Required_Fields_Leader() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.infellowship.Groups_Group_View_Settings(_groupName);

            // Click on the Create Location sidebar Link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditLocation);

            // Select online
            test.Selenium.Click(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.RadioButton_MeetsOnline);

            // Clear out the name
            test.Selenium.Type(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.TextField_LocationName, "");

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.Button_CreateLocation);

            // Verify text
            test.Selenium.VerifyTextPresent("Name is required.");

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you are taken to the Create Location page after clicking on the sidebar link on the Settings Page.")]
        public void GroupManagement_Create_Location_View_From_Settings_Leader() {
            // Login to infellowship - Groups
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.infellowship.Groups_Group_View_Settings(_groupName);

            // Click on the Create Location sidebar Link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditLocation);

            // Verify you are on the Create Location page
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.TextField_LocationName);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.TextField_Description);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.RadioButton_MeetsInPerson);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.RadioButton_MeetsOnline);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you are taken back to the Settings page after clicking on the cancel link on the Create Location Page")]
        public void GroupManagement_Create_Location_Cancel_Leader() {
            // Login to infellowship - Groups
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.infellowship.Groups_Group_View_Settings(_groupName);

            // Click on the Create Location sidebar Link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditLocation);

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.Link_Cancel_CreateLocation);

            // Verify you are on the Settings page
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Roster);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);

            test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName));

            // sidebar links
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.Link_EditDetails);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditSchedule);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditLocation);

            // Logout
            test.infellowship.Logout();
        }

        [Test, MultipleAsserts, RepeatOnFailure, DoesNotRunOutsideOfStaging]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that span of care owner can create a physical location for a group from the InFellowship side and this group will appear in the search results after doing a proximity search in InFellowship.")]
        public void GroupManagement_Create_Location_Physical_Owner() {
            // Create the data / remove any left over data
            var socName = _spanOfCareName;
            var groupTypeName = _groupTypeName;
            var groupName = "Owner Physical Loc Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Bryan Mikaelian", "Leader");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Create a location for the group
            test.infellowship.Groups_Group_Create_Location_Physical(socName, groupName, "Test Location", null, "2812 Meadow Wood Drive", "Flower Mound", "Texas", "75022", false);

            // Find a group by zip code
            test.infellowship.Groups_FindAGroup_Search("75287", null, null, false);

            // Verify your group is returned
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));

            // Logout
            test.infellowship.Logout();

            // Find a group in InFellowship while not logged in. Verify this group is present
            test.infellowship.Groups_FindAGroup_Search("75287", null, null, false);

            // Verify your group is returned
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        [Test, MultipleAsserts, RepeatOnFailure, DoesNotRunOutsideOfStaging]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a span of care owner can create an online location for a group from the InFellowship side and verifies it is not in the search results if a zip code is provided.")]
        public void GroupManagement_Create_Location_Online_Owner() {
            // Create the data / remove any left over data
            var socName = _spanOfCareName;
            var groupTypeName = _groupTypeName;
            var groupName = "Owner Online Loc Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Bryan Mikaelian", "Leader");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Create an online location for the group
            test.infellowship.Groups_Group_Create_Location_Online(socName, groupName, "Test Location", null, "http://www.google.com");

            // Find a group in InFellowship while not logged in.
            test.infellowship.Groups_FindAGroup_Search("75287", null, null, false);

            // Verify your group is not returned
            Assert.IsFalse(test.Selenium.IsElementPresent(string.Format("link={0}", groupName)), "A group with an online location was returned when searching by zip code!");

            // Find a group again without a zip code.
            test.infellowship.Groups_FindAGroup_Search(null, null, null, false);

            // Verify your group is returned
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));

            // Logout
            test.infellowship.Logout();

            // Search for all groups by a zipcode
            TestLog.WriteLine("Search for all groups in ZIP 75287");
            test.infellowship.Groups_FindAGroup_Search("75287", null, null, false);

            // Verify your group is not returned
            TestLog.WriteLine("Verify Group is Not Returned");
            Assert.IsFalse(test.Selenium.IsElementPresent(string.Format("link={0}", groupName)), "A group with an online location was returned when searching by zip code!");

            // Find a group again without a zip code.
            TestLog.WriteLine("Find a Group NO ZIP");
            test.infellowship.Groups_FindAGroup_Search(null, null, null, false);

            // Verify your group is returned
            TestLog.WriteLine("Verify Group Returned: " + groupName);
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);

        }

        [Test, MultipleAsserts, RepeatOnFailure, DoesNotRunOutsideOfStaging]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a physical private location can be created for a group from the InFellowship side and this group will appear in the search results after doing a proximity search in InFellowship.")]
        public void GroupManagement_Create_Location_Physical_Owner_Private() {
            // Create the data / remove any left over data
            var socName = _spanOfCareName;
            var groupTypeName = _groupTypeName;
            var groupName = "Owner Physical Private Loc Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Bryan Mikaelian", "Leader");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Create a location for the group as an owner. Make it private.
            test.infellowship.Groups_Group_Create_Location_Physical(socName, groupName, "Test Location", null, "2812 Meadow Wood Drive", "Flower Mound", "Texas", "75022", true);

            // Find a group in InFellowship while logged in
            test.infellowship.Groups_FindAGroup_Search(null, null, null, false);

            // Verify your group is returned
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));

            // View the group
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", groupName));

            // Verify the address is not present since it is private
            Assert.IsFalse(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"), "Private location was present on the group's public page!");
            
            // Logout
            test.infellowship.Logout();

            // Find a group in InFellowship while not logged in. Verify this group is present
            test.infellowship.Groups_FindAGroup_Search(null, null, null, false);

            // Verify your group is returned
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));

            // View the group
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", groupName));

            // Verify the address is not present since it is private
            Assert.IsFalse(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"), "Private location was present on the group's public page!");

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the controls are present if you, a span of care owner, switch from an online location to a physical location")]
        public void GroupManagement_Create_Location_Controls_Present_After_Switching_Between_Online_And_Physical_For_Owner() {
            // Login to infellowship - Groups
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.infellowship.Groups_Group_View_Settings(_spanOfCareName, _groupName);

            // Click on the Create Location sidebar Link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditLocation);

            // Select online
            test.Selenium.Click(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.RadioButton_MeetsOnline);

            // Select meets in person
            test.Selenium.Click(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.RadioButton_MeetsInPerson);

            // Verify the controls are present for a physical location
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.DropDown_Country));
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.TextField_AddressOne));
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.TextField_AddressTwo));
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.TextField_City));
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.DropDown_State));
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.TextField_ZipCode));
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.TextField_County));

            // Log out
            test.infellowship.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the required fields for a 'Meets in Person' location for a span of care owner.")]
        public void GroupManagement_Create_Location_Physical_Required_Fields_Owner() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.infellowship.Groups_Group_View_Settings(_spanOfCareName, _groupName);

            // Click on the Create Location sidebar Link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditLocation);

            // Select meets in person
            test.Selenium.Click(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.RadioButton_MeetsInPerson);

            // Clear out the name
            test.Selenium.Type(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.TextField_LocationName, "");

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.Button_CreateLocation);

            // Verify text
            test.Selenium.IsTextPresent("Name is required.");

            // Enter a name
            test.Selenium.Type(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.TextField_LocationName, "My Location");

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.Button_CreateLocation);

            // Verify text
            Assert.IsTrue(test.Selenium.IsTextPresent("Address 1 is required"));
            Assert.IsTrue(test.Selenium.IsTextPresent("State is required."));
            Assert.IsTrue(test.Selenium.IsTextPresent("Postal Code is required."));
            Assert.IsTrue(test.Selenium.IsTextPresent("City is required."));

            // Log out
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the required fields for an 'Online' location for a span of care owner")]
        public void GroupManagement_Create_Location_Online_Required_Fields_Owner() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.infellowship.Groups_Group_View_Settings(_spanOfCareName, _groupName);

            // Click on the Create Location sidebar Link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditLocation);

            // Select online
            test.Selenium.Click(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.RadioButton_MeetsOnline);

            // Clear out the name
            test.Selenium.Type(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.TextField_LocationName, "");

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.Button_CreateLocation);

            // Verify text
            test.Selenium.VerifyTextPresent("Name is required.");

            // Log out
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you, a span of care owner, are taken to the Create Location page after clicking on the sidebar link on the Settings Page.")]
        public void GroupManagement_Create_Location_View_From_Settings_Owner() {
            // Login to infellowship - Groups
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.infellowship.Groups_Group_View_Settings(_spanOfCareName, _groupName);

            // Click on the Create Location sidebar Link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditLocation);

            // Verify you are on the Create Location page
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.TextField_LocationName);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.TextField_Description);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.RadioButton_MeetsInPerson);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.RadioButton_MeetsOnline);

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you, a span of care owner, are taken back to the Settings page after clicking on the cancel link on the Create Location Page")]
        public void GroupManagement_Create_Location_Cancel_Owner() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.infellowship.Groups_Group_View_Settings(_spanOfCareName, _groupName);

            // Click on the Create Location sidebar Link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditLocation);

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.SettingsPage.CreateEditLocationPage.Link_Cancel_CreateLocation);

            // Verify you are on the Settings page
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Roster);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);

            test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName));

            // sidebar links
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.Link_EditDetails);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditSchedule);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditLocation);

            // Logout
            test.infellowship.Logout();

        }

        #endregion Location Management

        #region Send an Email
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you can send an email to the group when you are a leader of a group and have the correct permissions.")]
        public void GroupManagement_Send_Email_Leader() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Send an email to everyone
            test.infellowship.Groups_Group_SendEmail_All(_groupName, string.Format("[{0}] Test Email to Group from Leader", base.F1Environment), "This is a test email from a leader!");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelain")]
        [Description("Verifies that a Group Leader can send an email to other group leaders.")]
        public void GroupManagement_Send_Email_Leader_To_Leader() {
            // Recipients
            List<string> recipientNames = new List<string>();
            recipientNames.Add("InFellowship User");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Email certain recipients
            test.infellowship.Groups_Group_SendEmail(_groupName, "Email Specific Users", "This is only to certain users", recipientNames);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a Group Member can send an email to other group leaders.")]
        public void GroupManagement_Send_Email_Member_To_Leader() {
            // Recipients
            List<string> recipientNames = new List<string>();
            recipientNames.Add("InFellowship User");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Email certain recipients
            test.infellowship.Groups_Group_SendEmail(_groupName, "Email Specific Users", "This is only to certain users", recipientNames);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the control that appears when you, a group leader, attempt to add an attachment to an email shows up.")]
        public void GroupManagement_Send_Email_Attachment_Controls_Present_Leader() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a group 
            test.infellowship.Groups_Group_View(_groupName);

            // Send an email
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.RosterPage.Link_SendAnEmail);

            // Add an attachment
            test.Selenium.Click(GroupsConstants.GroupManagement.SendAnEmailPage.Link_AttachAFile);

            // Verify controls are present
            Assert.IsTrue(test.Selenium.IsElementPresent("attachment"));
            Assert.IsTrue(test.Selenium.IsTextPresent("Max file size of 512 KB."));

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the required fields for sending a group email as a leader.")]
        public void GroupManagement_Send_Email_Required_Fields_Leader() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Send an email without filling out any information
            test.infellowship.Groups_Group_SendEmail_All(_groupName, null, null);

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you are taken back to the Roster Page if you hit the cancel link on the Send email page")]
        public void GroupManagement_Send_Email_Cancel_Leader() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View Send an Email
            test.infellowship.Groups_Group_View_SendEmail(_groupName);

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.NavigationLinks.Link_CancelToRoster);

            // Verify you are on the roster page
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Roster);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Prospects);

            // side bar
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_ViewProspects);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_InviteSomeone);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_SendAnEmail);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you are taken to the Send Email Page if you hit the link on the Gear Menu")]
        public void GroupManagement_Send_Email_View_Page_From_Gear_Leader() {
            // Login to infellowship - Groups
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Open the Gear Menu
            test.Selenium.Click(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);

            // Select Send an Email
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.NavigationLinks.Gear_SendAnEmail);

            // Verify you are on the send an email page
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SendAnEmailPage.RadioButton_SendToEveryone);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SendAnEmailPage.RadioButton_LetMeChoose);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SendAnEmailPage.TextField_Subject);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SendAnEmailPage.TextField_Message);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.SendAnEmailPage.Link_AttachAFile);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you, a span of care owner, can send an email to the group when you are a leader of a group and have the correct permissions.")]
        public void GroupManagement_Send_Email_Owner() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Send an email to everyone
            test.infellowship.Groups_Group_SendEmail_All(_spanOfCareName, _groupName, string.Format("[{0}] Test Email to Group from Owner", base.F1Environment), "This is a test email from an owner!");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the control that appears when you, a span of care owner, attempt to add an attachment to an email shows up.")]
        public void GroupManagement_Send_Email_Attachment_Controls_Present_Owner() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View Send an Email
            test.infellowship.Groups_Group_View_SendEmail(_spanOfCareName, _groupName);

            // Add an attachment
            test.Selenium.Click(GroupsConstants.GroupManagement.SendAnEmailPage.Link_AttachAFile);

            // Verify controls are present
            Assert.IsTrue(test.Selenium.IsElementPresent("attachment"));
            Assert.IsTrue(test.Selenium.IsTextPresent("Max file size of 512 KB."));

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the required fields for sending a group email as an owner.")]
        public void GroupManagement_Send_Email_Required_Fields_Owner() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Send an email without filling out any information
            test.infellowship.Groups_Group_SendEmail_All(_spanOfCareName, _groupName, null, null);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you, a span of care owner, are taken back to the Roster Page if you hit the cancel link on the Send email page")]
        public void GroupManagement_Send_Email_Cancel_Owner() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View Send an Email
            test.infellowship.Groups_Group_View_SendEmail(_spanOfCareName, _groupName);

            // Cancel arrow
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.NavigationLinks.Link_CancelToRoster);

            // Verify you are on the roster page
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Roster);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Prospects);

            // side bar
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_ViewProspects);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_InviteSomeone);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_SendAnEmail);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you cannot select someone to email if they are unsubscribed from church communication.")]
        public void GroupManagement_Send_Email_Cannot_Select_Unsubscribed() {
            // Unsubscribe an individual
            var individualName = "Group Member";
            base.SQL.Execute(string.Format("UPDATE ChmPeople.dbo.Individual SET UnsubscribeAllChurchEmail = 1 where church_id = 254 and individual_name = '{0}'", individualName));

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the email page for the group
            test.infellowship.Groups_Group_View_SendEmail(_groupName);

            // Verify the checkbox is not present for the individual since they are unsubscribed.
            test.Selenium.VerifyElementNotPresent(string.Format("//input[@name='send_to' and normalize-space(ancestor::td/following-sibling::td/label/text())='{0}']", individualName));

            // Logout
            test.infellowship.Logout();

            // Clean up
            base.SQL.Execute(string.Format("UPDATE ChmPeople.dbo.Individual SET UnsubscribeAllChurchEmail = 0 where church_id = 254 and individual_name = '{0}'", individualName));

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you cannot select someone to email from InFellowship if they have an invalid email.")]
        public void GroupManagement_Send_Email_Cannot_Select_Invalid() {
            // Unsubscribe an individual
            var individualName = "Group Leader";
            var groupName = "Invalid Email group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.People_UpdateCommunicationValidationStatus(254, individualName, "group.leader.ft@gmail.com", 0);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Bryan Mikaelian", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Member");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the email page for the group
            test.infellowship.Groups_Group_View_SendEmail(groupName);

            // Verify the checkbox is not present for the individual since they are unsubscribed.
            test.Selenium.VerifyElementNotPresent(string.Format("//input[@name='send_to' and normalize-space(ancestor::td/following-sibling::td/label/text())='{0}']", individualName));

            // Logout
            test.infellowship.Logout();

            // Clean up
            base.SQL.People_UpdateCommunicationValidationStatus(254, individualName, "group.leader.ft@gmail.com", null);
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        #endregion Send an Email

        #region Roster Page
        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you are taken to the group roster page after clicking on the sidebar link on the Dashboard Page")]
        public void GroupManagement_View_Roster_Page_From_Dashboard_Sidebar_Link() {
            // Login to infellowship - Groups
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Click on the Dashboard Link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard);

            // Click on the Roster sidebar Link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.DashboardPage.Link_ViewRoster);

            // Verify you are on the roster page
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Roster);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);

            test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName));

            // side bar
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_ViewProspects);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_InviteSomeone);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_SendAnEmail);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you are taken to the group roster page after clicking on the link")]
        public void GroupManagement_View_Roster_Page() {
            // Login to infellowship - Groups
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Verify you are on the roster page
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Roster);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);

            test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName));

            // side bar
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_ViewProspects);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_InviteSomeone);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_SendAnEmail);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you are taken to the group roster page after clicking on the link from the Dashboard Page")]
        public void GroupManagement_View_Roster_Page_From_Dashboard() {
            // Login to infellowship - Groups
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Click on the Dashboard Link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard);

            // Click on the Roster Link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.NavigationLinks.Link_Roster);

            // Verify you are on the roster page
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Roster);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);

            test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName));

            // side bar
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_ViewProspects);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_InviteSomeone);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_SendAnEmail);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you are taken to the group roster page after clicking on the link from the Settings Page")]
        public void GroupManagement_View_Roster_Page_From_Settings() {
            // Login to infellowship - Groups
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings
            test.infellowship.Groups_Group_View_Settings(_groupName);

            // Click on the Roster Link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.NavigationLinks.Link_Roster);

            // Verify you are on the roster page
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Roster);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);

            test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName));

            // side bar
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_ViewProspects);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_InviteSomeone);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_SendAnEmail);

            // Logout
            test.infellowship.Logout();
        }

        #endregion Roster Page

        #region Dashboard Page
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you are taken to the dashboard page from the settings page after clicking on the link, while logged in")]
        public void GroupManagement_View_Dashboard_From_Settings_Page() {
            // Login to infellowship - Groups
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View Settings
            test.infellowship.Groups_Group_View_Settings(_groupName);

            // Click on the Dashboard Link
            test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard);

            // Verify you are on the Dashboard page
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Roster);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);

            test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName));

            // Side links
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.DashboardPage.Link_ViewRoster);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.DashboardPage.Link_ViewSettings);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_InviteSomeone);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_SendAnEmail);

            test.Selenium.VerifyTextPresent("Information");

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you are taken to the dashboard page if you click on the Group Name link, while viewing the group's roster")]
        public void GroupManagement_View_Dashboard_Page_From_Roster() {
            // Login to infellowship - Groups
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a group
            test.infellowship.Groups_Group_View(_groupName);

            // Click on the Group Name link at the top
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupName));

            // Verify you are on the Dashboard page
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Roster);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);

            test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName));

            // Side links
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.DashboardPage.Link_ViewRoster);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.DashboardPage.Link_ViewSettings);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_InviteSomeone);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_SendAnEmail);

            test.Selenium.VerifyTextPresent("Information");

            // Logout of infellowship
            test.infellowship.Logout();
        }
        #endregion Dashboard Page

        #region Contact Us
        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can contact the church.")]
        public void GroupManagement_Contact_Us() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // Contact us
            test.Selenium.Click("link=Contact us");

            // Enter a message
            test.Selenium.Type("contact_us_message", string.Format("[{0}] Contact message.", base.F1Environment.ToString()));

            // Submit
            test.Selenium.Click("//input[@value='Submit']");

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the required fields for Contact Us")]
        public void GroupManagement_Contact_Us_Required_Fields() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // Contact us
            test.Selenium.Click("link=Contact us");

            // Submit
            test.Selenium.Click("//input[@value='Submit']");

            // Verify text
            Assert.IsTrue(test.Selenium.IsTextPresent("Required: Name, message, and valid email address."));

            // Logout
            test.infellowship.Logout();
        }
        #endregion Contact Us
    }

    [TestFixture]
    public class infellowship_Groups_GroupManagementWebDriver : FixtureBaseWebDriver
    {
        #region Fixture Setup and Teardown
        string _groupTypeName = "Group Management Group Type WebDriver";
        string _groupName = "General Group WebDriver";
        string _spanOfCareName = "InFellowship Group Management SOC WebDriver";

        [FixtureSetUp]
        public void FixtureSetUp()
        {
            // Group Type
            base.SQL.Groups_SpanOfCare_Delete(254, _spanOfCareName);
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", _groupTypeName, new List<int> { 1, 2, 3, 4, 5, 9, 10, 11, 12 });

            // Create a generic group and add a leader and member
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "InFellowship User", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Member", "Member");
            base.SQL.Groups_SpanOfCare_Create(254, _spanOfCareName, "Bryan Mikaelian", "SOC Owner", new List<string>() { _groupTypeName });
        }

        [FixtureTearDown]
        public void FixtureTearDown()
        {
            //base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
            //base.SQL.Groups_SpanOfCare_Delete(254, _spanOfCareName);
        }
        #endregion Fixture Setup and Teardown


        #region Group Member - Edit
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies the correct marital statuses show up when editing a group member.")]
        public void GroupManagement_Group_Member_Edit_Correct_Marital_Statuses_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

            // Navigate to the roster page for a group
            test.Infellowship.Groups_Group_View_Roster_WebDriver("A Test Group");
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("View prospects"));

            // Navigate to the member page
            test.Driver.FindElementByXPath("//table[contains(@class,'hidden-xs')]//a[contains(@href, '/Members/')]").Click();
            //test.Driver.FindElementByXPath("//a[contains(@href, '/Members/')]").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Edit this person"));

            // Edit the member
            test.Driver.FindElementByLinkText(GroupsConstants.GroupManagement.IndividualMemberPage.Link_EditMemberWebDriver).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Cancel"));

            // Verify the selectable marital statuses
            Assert.AreEqual(8, test.Driver.FindElementsByXPath("//select[@id='marital_status']/option").Count);
            Assert.AreEqual(string.Empty, test.Driver.FindElementByXPath("//select[@id='marital_status']/option[1]").Text);
            Assert.AreEqual("Child/Yth", test.Driver.FindElementByXPath("//select[@id='marital_status']/option[2]").Text);
            Assert.AreEqual("Divorced", test.Driver.FindElementByXPath("//select[@id='marital_status']/option[3]").Text);
            Assert.AreEqual("Married", test.Driver.FindElementByXPath("//select[@id='marital_status']/option[4]").Text);
            Assert.AreEqual("Separated", test.Driver.FindElementByXPath("//select[@id='marital_status']/option[5]").Text);
            Assert.AreEqual("Single", test.Driver.FindElementByXPath("//select[@id='marital_status']/option[6]").Text);
            Assert.AreEqual("Widow", test.Driver.FindElementByXPath("//select[@id='marital_status']/option[7]").Text);
            Assert.AreEqual("Widower", test.Driver.FindElementByXPath("//select[@id='marital_status']/option[8]").Text);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3474: Verifies a group leader has the ability to edit a member's joined date when they have rights to do so.")]
        public void GroupManagement_Group_Member_Joined_Date_Edit_Present_Leader()
        {
            // Data
            string group = "Test Group Member Joined Date";
            string individual = "David Martin";

            // Login to Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!");

            // Navigate to Your Groups
            test.Driver.FindElementByXPath(Navigation.InFellowship.Your_Groups).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText(group));

            // Click on the desired group name
            test.Driver.FindElementByLinkText(group).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("View roster"));

            // View the roster
            test.Driver.FindElementByLinkText("View roster").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText(individual));

            // Click on an indvidual
            test.Driver.FindElementByLinkText(individual).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Edit this person"));

            // Verify there is an Edit link next to their "Member since" date
            IWebElement table = test.Driver.FindElementByXPath(TableIds.InFellowship_Groups_Roster_Profile);
            
            //Commenting these lines and just verifying Edit text for now.
            //string editText = table.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[0].FindElements(By.TagName("a"))[0].Text;
           // Assert.AreEqual("Edit", editText);

            test.Driver.FindElementByLinkText("Edit");

            // Logout of Infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3785: Verifies a SOC owner has the ability to edit a member's joined date when they have rights to do so.")]
        public void GroupManagement_Group_Member_Joined_Date_Edit_Present_SOCOwner()
        {
            // Data
            string group = "Test Group Member Joined Date";
            string soc = "Auto Test SOC Edit";
            string individual = "David Martin";

            // Login to Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.billmurray@gmail.com", "FT4life!");

            // Navigate to Your Groups
            test.Driver.FindElementByXPath(Navigation.InFellowship.Your_Groups).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText(soc));

            // Click on the Span of Care
            test.Driver.FindElementByLinkText(soc).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText(group));

            // Click on the desired group name
            test.Driver.FindElementByLinkText(group).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("View roster"));

            // View the roster
            test.Driver.FindElementByLinkText("View roster").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText(individual));

            // Click on an indvidual
            test.Driver.FindElementByLinkText(individual).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Edit this person"));

            // Verify there is an Edit link next to their "Member since" date
            IWebElement table = test.Driver.FindElementByXPath(TableIds.InFellowship_Groups_Roster_Profile);
            //TestLog.WriteLine("Row #: {0}", test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster_Profile, "01/22/2014", "Member since", null));
            //decimal itemRow = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster_Profile, "01/22/2014", "Member since", null);
            /*          
            string editText = table.FindElements(By.TagName("tr"))[8].FindElements(By.TagName("td"))[0].FindElements(By.TagName("a"))[0].Text;

            
            Assert.AreEqual("Edit", editText); */

            //SP-  verifying text to make sure Edit is displayed in the page since We are gettig index out of bound for line above....
            test.GeneralMethods.VerifyTextPresentWebDriver("Edit");

            // Logout of Infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3474: Verifies a group leader does not have the ability to edit a member's joined date when they do not have rights to do so.")]
        public void GroupManagement_Group_Member_Joined_Date_Edit_NOTPresent()
        {
            // Data
            string group = "Joined Date Not Present";
            string individual = "David Martin";

            // Login to Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!");

            // Navigate to Your Groups
            test.Driver.FindElementByXPath(Navigation.InFellowship.Your_Groups).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText(group));

            // Click on the desired group name
            test.Driver.FindElementByLinkText(group).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("View roster"));

            // View the roster
            test.Driver.FindElementByLinkText("View roster").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText(individual));

            // Click on an indvidual
            test.Driver.FindElementByLinkText(individual).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Remove from group"));

            // Verify there is NOT an Edit link next to their "Member since" date
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.XPath("//a[@class='modal_link']"));
            
            // Logout of Infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3474: Verifies you can successfully edit a group member's joined date.")]
        public void GroupManagement_Group_Member_Joined_Date_Edit_Success() 
        {
            // Data
            string group = "Test Group Member Joined Date";
            string firstName = "Kevin";
            string lastName = "Spacey";

            // Login to Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!");

            // Add a new group member
            test.Infellowship.Groups_Group_Add_New_Member_WebDriver(group, firstName, lastName);

            try
            {
                // Edit the joined date
                test.Infellowship.Groups_Group_ViewMember_Edit_Joined_Date_WebDriver(group, string.Format("{0} {1}", firstName, lastName), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(-1));

            }
            finally
            {
                // Remove group member from group
                test.Infellowship.Groups_Group_Delete_Member_WebDriver(group, string.Format("{0} {1}", firstName, lastName));

                // Logout of Infellowship
                test.Infellowship.LogoutWebDriver();
            }
            

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3474: Verifies you cannot successfully edit a group member's joined date to a date before the group started.")]
        public void GroupManagement_Group_Member_Joined_Date_Edit_BeforeGroupStartDate()
        {
            // Data
            string group = "Test Group Member Joined Date";
            string firstName = "Dave";
            string lastName = "Matthews";
            string date = "09/10/2013";
            DateTime joinDate = Convert.ToDateTime(date);

            // Login to Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!");

            // Add a new group member
            test.Infellowship.Groups_Group_Add_New_Member_WebDriver(group, firstName, lastName);

            try
            {
                // Edit the joined date
                test.Infellowship.Groups_Group_ViewMember_Edit_Joined_Date_WebDriver(group, string.Format("{0} {1}", firstName, lastName), joinDate);
            }
            finally
            {
                // Remove group member from group
                test.Infellowship.Groups_Group_Delete_Member_WebDriver(group, string.Format("{0} {1}", firstName, lastName));

                // Logout of Infellowship
                test.Infellowship.LogoutWebDriver();
            }
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3474: Verifies you cannot successfully edit a group member's joined date to a date in the future.")]
        public void GroupManagement_Group_Member_Joined_Date_Edit_FutureDate()
        {
            // Data
            string group = "Test Group Member Joined Date";
            string firstName = "Amanda";
            string lastName = "Martin";
            DateTime joinDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1);

            // Login to Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!");

            // Add a new group member
            test.Infellowship.Groups_Group_Add_New_Member_WebDriver(group, firstName, lastName);

            // Edit the joined date
            test.Infellowship.Groups_Group_ViewMember_Edit_Joined_Date_WebDriver(group, string.Format("{0} {1}", firstName, lastName), joinDate);

            // Remove group member from group
            test.Infellowship.Groups_Group_Delete_Member_WebDriver(group, string.Format("{0} {1}", firstName, lastName));

            // Logout of Infellowship
            test.Infellowship.LogoutWebDriver();

        }

        #endregion Group Member - Edit

        #region Edit Details
        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("David Martin")]
        [Description("Verifies that a leader can update the name and description of a group")]
        public void GroupManagement_Edit_Details_Update_Name_And_Description_Leader_WebDriver()
        {
            // Create the data / remove any left over data
            var groupTypeName = _groupTypeName;
            var groupName = "Edit Details Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Bryan Mikaelian", "Leader");

            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // Update the details of a group
            test.Infellowship.Groups_Group_Update_Details_WebDriver(groupName, "Leader UPDATED Group", "Test updated description");

            // Revert the changes
            test.Infellowship.Groups_Group_Update_Details_WebDriver("Leader UPDATED Group", groupName, string.Empty);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies you are taken to the Edit Details page after clicking on the sidebar link on the Settings Page, while logged in")]
        public void GroupManagement_Edit_Details_View_From_Settings_Sidebar_Link_Leader_WebDriver()
        {
            // Login to infellowship - Groups
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.Infellowship.Groups_Group_View_Settings_WebDriver(_groupName);

            // Click on the Edit Details sidebar Link
            test.Driver.FindElementByLinkText(GroupsConstants.GroupManagement.SettingsPage.Link_EditDetails_WebDriver).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Cancel"));

            // Verify you are on the Edit details page
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id(GroupsConstants.GroupManagement.SettingsPage.EditDetailsPage.TextField_GroupName));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id(GroupsConstants.GroupManagement.SettingsPage.EditDetailsPage.DropDown_Timezone));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id(GroupsConstants.GroupManagement.SettingsPage.EditDetailsPage.TextField_Description));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id(GroupsConstants.GroupManagement.SettingsPage.EditDetailsPage.DropDown_MaritalStatus));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id(GroupsConstants.GroupManagement.SettingsPage.EditDetailsPage.DropDown_Gender));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id(GroupsConstants.GroupManagement.SettingsPage.EditDetailsPage.TextField_LowerAgeRange));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id(GroupsConstants.GroupManagement.SettingsPage.EditDetailsPage.TextField_UpperAgeRange));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id(GroupsConstants.GroupManagement.SettingsPage.EditDetailsPage.Checkbox_Childcare));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id(GroupsConstants.GroupManagement.SettingsPage.EditDetailsPage.Checkbox_Searchable));

            // Logout
            test.Infellowship.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies you are taken back to the Settings page after clicking on the cancel link on the Edit Details Page")]
        public void GroupManagement_Edit_Details_Cancel_Leader_WebDriver()
        {
            // Login to infellowship - Groups
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.Infellowship.Groups_Group_View_Settings_WebDriver(_groupName);

            // Click on the Edit Details sidebar Link
            test.Driver.FindElementByLinkText(GroupsConstants.GroupManagement.SettingsPage.Link_EditDetails_WebDriver).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Cancel"));

            // Cancel
            test.Driver.FindElementByLinkText(GroupsConstants.GroupManagement.SettingsPage.EditDetailsPage.Link_Cancel_EditDetails).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText(GroupsConstants.GroupManagement.SettingsPage.Link_EditDetails_WebDriver));

            // Verify you are on the Settings page
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard_WebDriver));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(GroupsConstants.GroupManagement.NavigationLinks.Link_Roster_WebDriver));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.CssSelector(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu.Split('=')[1]));
      

            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(_groupName));

            // sidebar links
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard_WebDriver));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(GroupsConstants.GroupManagement.SettingsPage.Link_EditDetails_WebDriver));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditSchedule));

            // Logout
            test.Infellowship.LogoutWebDriver();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("David Martin")]
        [Description("Verifies that a span of care owner can update the name and description of a group")]
        public void GroupManagement_Edit_Details_Update_Name_And_Description_Owner_WebDriver()
        {
            // Create the data / remove any left over data
            var spanOfCareName = _spanOfCareName;
            var groupTypeName = _groupTypeName;
            var groupName = "Edit Details Owner Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Bryan Mikaelian", "Leader");

            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Update the details of a group
            test.Infellowship.Groups_Group_Update_Details_WebDriver(spanOfCareName, groupName, "Owner UPDATED Group", "Test updated description");

            // Revert the changes
            test.Infellowship.Groups_Group_Update_Details_WebDriver("Owner UPDATED Group", groupName, string.Empty);

            // Logout
            test.Infellowship.LogoutWebDriver();

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies you, a Span of Care Owner, are taken to the Edit Details page after clicking on the sidebar link on the Settings Page, while logged in")]
        public void GroupManagement_Edit_Details_View_From_Settings_Sidebar_Link_Owner_WebDriver()
        {
            // Login to infellowship - Groups
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.Infellowship.Groups_Group_View_Settings_WebDriver(_spanOfCareName, _groupName);

            // Click on the Edit Details sidebar Link
            test.Driver.FindElementByLinkText(GroupsConstants.GroupManagement.SettingsPage.Link_EditDetails_WebDriver).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Cancel"));

            // Verify you are on the Edit details page
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id(GroupsConstants.GroupManagement.SettingsPage.EditDetailsPage.TextField_GroupName));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id(GroupsConstants.GroupManagement.SettingsPage.EditDetailsPage.DropDown_Timezone));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id(GroupsConstants.GroupManagement.SettingsPage.EditDetailsPage.TextField_Description));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id(GroupsConstants.GroupManagement.SettingsPage.EditDetailsPage.DropDown_MaritalStatus));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id(GroupsConstants.GroupManagement.SettingsPage.EditDetailsPage.DropDown_Gender));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id(GroupsConstants.GroupManagement.SettingsPage.EditDetailsPage.TextField_LowerAgeRange));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id(GroupsConstants.GroupManagement.SettingsPage.EditDetailsPage.TextField_UpperAgeRange));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id(GroupsConstants.GroupManagement.SettingsPage.EditDetailsPage.Checkbox_Childcare));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id(GroupsConstants.GroupManagement.SettingsPage.EditDetailsPage.Checkbox_Searchable));

            // Logout
            test.Infellowship.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies you, a span of care owner, are taken back to the Settings page after clicking on the cancel link on the Edit Details Page")]
        public void GroupManagement_Edit_Details_Cancel_Owner_WebDriver()
        {
            // Login to infellowship - Groups
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the settings of a group
            test.Infellowship.Groups_Group_View_Settings_WebDriver(_spanOfCareName, _groupName);

            // Click on the Edit Details sidebar Link
            test.Driver.FindElementByLinkText(GroupsConstants.GroupManagement.SettingsPage.Link_EditDetails_WebDriver).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Cancel"));

            // Cancel
            test.Driver.FindElementByLinkText(GroupsConstants.GroupManagement.SettingsPage.EditDetailsPage.Link_Cancel_EditDetails).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText(GroupsConstants.GroupManagement.SettingsPage.Link_EditDetails_WebDriver));

            // Verify you are on the Settings page
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard_WebDriver));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(GroupsConstants.GroupManagement.NavigationLinks.Link_Roster_WebDriver));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.CssSelector(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu.Split('=')[1]));

            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(_groupName));

            // sidebar links
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard_WebDriver));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(GroupsConstants.GroupManagement.SettingsPage.Link_EditDetails_WebDriver));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath(GroupsConstants.GroupManagement.SettingsPage.Link_CreateEditSchedule));

            // Logout
            test.Infellowship.LogoutWebDriver();
        }
        #endregion Edit Details

        #region Update Bulletin Board
        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies that a leader can perform various updates to the bulletin board to display it on the dashboard and update the date/time.")]
        public void GroupManagement_UpdateBulletinBoard_Leader_WebDriver()
        {
            // Create the data / remove any left over data
            var groupTypeName = _groupTypeName;
            var groupName = "Bulletin Board Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Bryan Mikaelian", "Leader");

            // Login to infellowship - Groups
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // Update the bulletin board and publish
            test.Infellowship.Groups_Group_Update_BulletinBoard_WebDriver(groupName, "I made an update to the bulletin board ", "Bryan Mikaelian", true);

            // Verify the bulletin board update is published to the dashboard
            test.Driver.FindElementByLinkText("Dashboard").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("View settings"));
            test.GeneralMethods.VerifyTextPresentWebDriver("I made an update to the bulletin board");

            // Unpublish the bulletin board
            test.Infellowship.Groups_Group_Update_BulletinBoard_WebDriver(groupName, "I made another update to the bulletin board", "Bryan Mikaelian", false);

            // Verify it is not present on the dashboard
            test.Driver.FindElementByLinkText("Dashboard").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("View settings"));
            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver("I made another update to the bulletin board"), "Unpublished bulletin board message was published!");

            // Logout
            test.Infellowship.LogoutWebDriver();

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);
        }
        #endregion Update Bulletin Board

        #region Schedule Management
        #endregion Schedule Management

        #region Location Management
        #endregion Location Management

        #region Send an Email
        [Test, RepeatOnFailure]
        [Author("Jim Jin")]
        [Description("FO-2746 Infellowship: Can't send group e-mails w/ group contains individuals with too long e-mails")]
        public void GroupManagement_SendEmail_VeryLong_WebDriver()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            #region Test data parameters
            var churchId = 15;
            var groupTypeName = "Automation Tests";
            var groupName = utility.GetUniqueName("TestGroup");
            var firstName1 = "Auto";
            var lastName1 = utility.GetUniqueName("TestInd1");
            var email_1 = utility.GetUniqueName("TestEmailAdd") + "@qp1.org";
            var firstName2 = "Auto";
            var lastName2 = utility.GetUniqueName("TestInd2");
            var email_2_long = "TestEmailAdd01234567890123456789012345678901234567890123456789012345678901234567890"
                                 +"12345678901234567890123456789987654321001234567890123456789012345678901234567890"
                                 +"12345678901234567890123456789@qp1.org";
            var firstName3 = "Auto";
            var lastName3 = utility.GetUniqueName("TestInd3");
            var email_3 = utility.GetUniqueName("TestEmail") + "@qp1.org";
            #endregion

            try
            {
                #region Test data SQL Preparation
                base.SQL.People_Individual_Create(churchId, firstName1, lastName1, 1);
                base.SQL.People_CommunicationValue_Add(churchId, string.Format("{0} {1}", firstName1, lastName1), GeneralEnumerations.CommunicationTypes.Personal, email_1, true);
                base.SQL.People_InFellowshipAccount_Create(churchId, string.Format("{0} {1}", firstName1, lastName1), email_1, "FT4life!");

                base.SQL.People_Individual_Create(churchId, firstName2, lastName2, 1);
                base.SQL.People_CommunicationValue_Add(churchId, string.Format("{0} {1}", firstName2, lastName2), GeneralEnumerations.CommunicationTypes.Personal, email_2_long, true);

                base.SQL.People_Individual_Create(churchId, firstName3, lastName3, 1);
                base.SQL.People_CommunicationValue_Add(churchId, string.Format("{0} {1}", firstName3, lastName3), GeneralEnumerations.CommunicationTypes.Personal, email_3, true);

                base.SQL.Groups_Group_Delete(churchId, groupName);
                base.SQL.Groups_Group_Create(churchId, groupTypeName, string.Format("{0} {1}", firstName1, lastName1), groupName, null, DateTime.Now.AddDays(-1).ToString("M/d/yyyy"));
                base.SQL.Groups_Group_AddLeaderOrMember(churchId, groupName, string.Format("{0} {1}", firstName1, lastName1), "Leader");
                base.SQL.Groups_Group_AddLeaderOrMember(churchId, groupName, string.Format("{0} {1}", firstName3, lastName3), "Member");
                #endregion

                #region Test Operations and Assertions
                // Login to infellowship
                test.Infellowship.LoginWebDriver(email_1, "FT4life!");

                // Go to Group Infomation page and click link "Send an email"
                utility.WaitAndGetElement(By.LinkText("Your Groups")).Click();
                utility.WaitAndGetElement(By.LinkText(groupName)).Click();
                utility.WaitAndGetElement(By.LinkText("Send an email")).Click();

                //Compose and send email page should be shown
                utility.waitForElementAppear("$('#subject')", 100);
                Assert.IsTrue(test.Driver.PageSource.Contains("Compose and send email"));

                //Enter subject and body of email
                utility.WaitAndGetElement(By.Id("subject")).SendKeys("Auto test subject");
                utility.WaitAndGetElement(By.Id("email_body")).SendKeys("Just for test, please don't reply!");

                //send email
                utility.WaitAndGetElement(By.Id("submitQuery")).Click();

                //Message "Email sent" should be shown
                utility.waitForElementAppear("$(\"span:contains('Email sent')\")", 100);
                Assert.IsTrue(test.Driver.FindElement(By.XPath("//span[contains(text(),'Email sent')]")).Text.Contains("Email sent"));

                //Add the individual whose email address length equals to 200 into group
                base.SQL.Groups_Group_AddLeaderOrMember(churchId, groupName, string.Format("{0} {1}", firstName2, lastName2), "Member");

                // Go to Group Infomation page and click link "Send an email"
                utility.WaitAndGetElement(By.LinkText("HOME")).Click();
                utility.WaitAndGetElement(By.LinkText("Your Groups")).Click();
                utility.WaitAndGetElement(By.LinkText(groupName)).Click();
                utility.WaitAndGetElement(By.LinkText("Send an email")).Click();

                //Compose and send email page should be shown
                utility.waitForElementAppear("$('#subject')", 100);
                Assert.IsTrue(test.Driver.PageSource.Contains("Compose and send email"));

                //Enter subject and body of email
                utility.WaitAndGetElement(By.Id("subject")).SendKeys("Auto test subject");
                utility.WaitAndGetElement(By.Id("email_body")).SendKeys("Just for test long email, please don't reply!");

                //send email
                utility.WaitAndGetElement(By.Id("submitQuery")).Click();

                //Message "Email sent" should be shown
                utility.waitForElementAppear("$(\"span:contains('Email sent')\")", 100);
                Assert.IsTrue(test.Driver.FindElement(By.XPath("//span[contains(text(),'Email sent')]")).Text.Contains("Email sent"));

                // Logout
                test.Infellowship.LogoutWebDriver();
                #endregion
            }
            finally
            {
                // Test data clean up
                base.SQL.Groups_Group_Delete(churchId, groupName);
                base.SQL.People_Individual_Delete(churchId, firstName1, lastName1);
                base.SQL.People_Individual_Delete(churchId, firstName2, lastName2);
                base.SQL.People_Individual_Delete(churchId, firstName3, lastName3);
            }
        }
        #endregion Send an Email

        #region Roster Page
        #endregion Roster Page

        #region Dashboard Page
        #endregion Dashboard Page

        #region Contact Us
        #endregion Contact Us
    }

}