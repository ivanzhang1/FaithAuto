using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace FTTests.infellowship.Groups {

	[TestFixture]
	public class infellowship_Groups_Security : FixtureBase {
        private string _groupTypeName = "Security Group Type";
        private string _groupName = "General Security Group";

        [FixtureSetUp]
        public void FixtureSetUp() {
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);

            // Create a generic group and add a leader and member
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", _groupTypeName, new List<int> { 1, 2, 3, 4, 5, 9, 10, 11, 12 });
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Member", "Member");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Mark Lindsley", "Member");
        }

        [FixtureTearDown]
        public void FixtureTearDown() {            
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
        }

		#region Members
		#region Admin Rights
		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Bryan Mikaelian")]
		[Description("Toggles the ability for members to email a group on and off.  Verifies the links are visible and aren't visisble.")]
		public void Security_Admin_Members_Email_Group() {
            // Set up the data
            var groupTypeName = "MemberEmail";
            var groupName = "MemberEmailGroup";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Member", "Member");

			// Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

			// Turn on the ability for members to email the group   
            test.Portal.Groups_GroupType_Update_AdminRight_Member(groupTypeName, GeneralEnumerations.GroupTypeMemberAdminRights.EmailGroup, true);

			// Logout
			test.Portal.Logout();

            // Login to infellowship
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a group you are a member of
            test.infellowship.Groups_Group_View_Roster(groupName);

			// Verify Send an email links are  present
			// Roster
			Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_SendAnEmail));

			// Dashboard
			test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
			Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_SendAnEmail));

			// Logout
			test.infellowship.Logout();


			// Revert the changes in portal
			test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Turn off the ability for members to email the group   
            test.Portal.Groups_GroupType_Update_AdminRight_Member(groupTypeName, GeneralEnumerations.GroupTypeMemberAdminRights.EmailGroup, false);

			// Logout
			test.Portal.Logout();


			// Login to infellowship
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View a group you are a member of
            test.infellowship.Groups_Group_View_Roster(groupName);

			// Verify Send an email links aren't present
			// Roster
			Assert.IsFalse(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_SendAnEmail));

			// Dashboard
			test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
			Assert.IsFalse(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_SendAnEmail));

			// Logout of infellowship
			test.infellowship.Logout();

            // Delete the group type
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
		}

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Toggles the ability for members to view the roster of the group on and off.  Verifies the links are visible and aren't visisble.")]
        public void Security_Admin_Members_View_Roster() {
            // Set up the data
            var groupTypeName = "MemberViewRoster";
            var groupName = "MemberViewRosterGroup";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Member", "Member");

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");


            // Turn off the ability for members to view the roster of a the group   
            test.Portal.Groups_GroupType_Update_AdminRight_Member(groupTypeName, GeneralEnumerations.GroupTypeMemberAdminRights.ViewRoster, false);

            // Logout
            test.Portal.Logout();


            // Login to infellowship
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a group you are a member of
            test.infellowship.Groups_Group_View_Dashboard(groupName);

            // Verify the roster links are present
            test.Selenium.VerifyElementNotPresent("link=Roster");

            // Dashboard
            test.infellowship.Groups_Group_View_Dashboard(groupName);
            test.Selenium.VerifyElementNotPresent("link=View roster");

            // Logout
            test.infellowship.Logout();


            // Revert the changes in portal
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Turn on the ability for members to email the group   
            test.Portal.Groups_GroupType_Update_AdminRight_Member(groupTypeName, GeneralEnumerations.GroupTypeMemberAdminRights.ViewRoster, true);

            // Logout
            test.Portal.Logout();

            // Login to infellowship
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a group you are a member of
            test.infellowship.Groups_Group_View_Roster(groupName);

            // Verify the roster links are not present
            test.Selenium.VerifyElementPresent("link=Roster");

            // Dashboard
            test.infellowship.Groups_Group_View_Dashboard(groupName);
            test.Selenium.VerifyElementPresent("link=View roster");

            // Logout of infellowship
            test.infellowship.Logout();

            // Delete the group type
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
        }

		[Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Verifies that a member cannot change the settings a group.")]
		public void Security_Admin_Members_Cannot_Change_Settings_Of_Group() {
			// Log in to InFellowship       
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// Select a group that you are a member of
            test.infellowship.Groups_Group_View(_groupName);

			// Verify Settings link is not present

			// Roster
			Assert.IsFalse(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Settings));

			// Dashboard
			test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
			Assert.IsFalse(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.DashboardPage.Link_ViewSettings));

			// Logout
			test.infellowship.Logout();
		}

		[Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Verifies that a member cannot invite members to the group or see the prospects.")]
		public void Security_Admin_Members_Cannot_Invite_or_View_Prospects() {
			// Log in to InFellowship       
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// Select a group that you are a member of
            test.infellowship.Groups_Group_View(_groupName);

			// Verify Invite someone link is not present

			// Roster
			Assert.IsFalse(test.Selenium.IsElementPresent("link=Add or Invite someone"));

			// Dashboard
			test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
			Assert.IsFalse(test.Selenium.IsElementPresent("link=Add or Invite someone"));

			// View prospects side bar
			Assert.IsFalse(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.DashboardPage.Link_ProspectsNumLink));

			// Gear
			//test.Selenium.Click("nav_gear");Modify by Clark.Peng
            test.Selenium.ClickAndWaitForPageToLoad("//div[contains(@class,'hidden-xs')]//span[contains(text(),'Quick Links')]");
			Assert.IsFalse(test.Selenium.IsElementPresent("nav_gear_menu_email"));

			// Logout
			test.infellowship.Logout();
		}
		#endregion Admin Rights

		#region View Rights
		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Bryan Mikaelian")]
		[Description("Toggles the ability for members to view limited information and basic information.  Verifies the correct amount of information is show.")]
		public void Security_View_Members() {
            // Set up the data
            var groupTypeName = "MemberLimitedView";
            var groupName = "MemberLimitedViewGroup";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Member", "Member");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Mark Lindsley", "Member");

			// Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

			// Grant member limited view rights
            test.Portal.Groups_GroupType_Update_ViewRights_Member(groupTypeName, GeneralEnumerations.GroupTypeMemberViewRights.Limited);

			// Logout
			test.Portal.Logout();


			// Navigate to InFellowship, view a group that you have member rights to
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a group you are a member of
            test.infellowship.Groups_Group_View_Roster(groupName);

			// View an individual of the group
			test.Selenium.ClickAndWaitForPageToLoad("link=Mark Lindsley");

			// Verify household information is not present and additional groups are not present
			Assert.IsFalse(test.Selenium.IsElementPresent("css=h6:contains('Household')"));
            Assert.IsFalse(test.Selenium.IsElementPresent("css=table.grid"));

			// Logout
			test.infellowship.Logout();


			// Revert the changes in portal
			test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

			// Grant members basic view rights
            test.Portal.Groups_GroupType_Update_ViewRights_Member(groupTypeName, GeneralEnumerations.GroupTypeMemberViewRights.Basic);
			
            // Logout
			test.Portal.Logout();


			// Navigate to InFellowship, view a group that you have member rights to
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            test.infellowship.Groups_Group_View_Roster(groupName);

			// View an individual of the group
            test.Selenium.ClickAndWaitForPageToLoad("link=Mark Lindsley");

            // Verify household information is not present and additional groups are not present
            Assert.IsTrue(test.Selenium.IsElementPresent("css=h6:contains('Household')"));
            Assert.IsFalse(test.Selenium.IsElementPresent("css=table.grid"));

			// Logout 
			test.infellowship.Logout();

            // Delete the group type
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
		}
		#endregion View Rights
		#endregion Members

		#region Leaders
		#region Admin Rights
		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Bryan Mikaelian")]
		[Description("Toggles the ability for leaders to email a group on and off.  Verifies the links are visible and aren't visisble.")]
		public void Security_Admin_Leaders_Email_Group() {
            // Set up the data
            var groupTypeName = "LeadersEmail";
            var groupName = "LeadersEmailGroup";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Member", "Member");

			// Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

			// Turn on the ability for leaders to email the group
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.EmailGroup, true);

			// Logout
			test.Portal.Logout();


			// Navigate to InFellowship, view a group that you have leader rights to
			test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            test.infellowship.Groups_Group_View_Roster(groupName);

			// Verify Email group links are present
			// Roster Page
			Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_SendAnEmail));

			// Dashboard
			test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_SendAnEmail));

			// Logout
			test.infellowship.Logout();


			// Revert the changes in portal
			test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

			// Turn off the ability for leaders to email the group
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.EmailGroup, false);
			
            // Logout
			test.Portal.Logout();


			// Navigate to InFellowship, view a group that you have leader rights to
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            test.infellowship.Groups_Group_View_Roster(groupName);

			// Verify Email group links are present
			// Roster Page
            Assert.IsFalse(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_SendAnEmail));

			// Dashboard
			test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
			Assert.IsFalse(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_SendAnEmail));

			// Logout of infellowship
			test.infellowship.Logout();

            // Delete the group type
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
		}

		[Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Toggles the ability for leaders to both add and invite someone to a group on and off.  Verifies the links are visible and aren't visisble.")]
		public void Security_Admin_Leaders_Edit_Add_Invite_Someone() {

            // Set up the data
            var groupTypeName = "AddInvite";
            var groupName = "AddInviteGroup";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Member", "Member");

			// Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

			// Turn on the ability for leaders to both invite and add someone
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.InviteSomeone, true);
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.AddSomeone, true);

			// Logout
			test.Portal.Logout();


			// Navigate to InFellowship, view a group that you have leader rights to
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            test.infellowship.Groups_Group_View_Roster(groupName);

			// Verify Invite someone links are present
			// Roster Page
			Assert.IsTrue(test.Selenium.IsElementPresent("link=Add or Invite someone"));

			// Dashboard
			test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Add or Invite someone"));

			// Gear 
            //test.Selenium.Click("nav_gear");  Modify by Clark.Peng
            test.Selenium.ClickAndWaitForPageToLoad("//div[contains(@class,'hidden-xs')]//span[contains(text(),'Quick Links')]");
            Assert.IsTrue(test.Selenium.IsElementPresent("//a[contains(@href, '/Group/Invite/')]"));

			// Verify remove individual link is present
			test.Selenium.ClickAndWaitForPageToLoad("link=Roster");
			test.Selenium.ClickAndWaitForPageToLoad("link=Group Member");
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Remove from group"));

			// Logout
			test.infellowship.Logout();

			// Revert the changes in portal
			test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

			// Turn off the ability for leaders to both invite and add someone to the group
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.InviteSomeone, false);
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.AddSomeone, false);

			// Logout
			test.Portal.Logout();

			// Navigate to InFellowship, view a group that you have leader rights to
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            test.infellowship.Groups_Group_View_Roster(groupName);

			// Verify Invite Smeone links aren't present
			// Roster Page
			Assert.IsFalse(test.Selenium.IsElementPresent("link=Add or Invite someone"));

			// Dashboard
			test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
            Assert.IsFalse(test.Selenium.IsElementPresent("link=Add or Invite someone"));

			// Gear 
			//test.Selenium.Click("nav_gear"); Modify by Clark.Peng
            test.Selenium.ClickAndWaitForPageToLoad("//div[contains(@class,'hidden-xs')]//span[contains(text(),'Quick Links')]");
            Assert.IsFalse(test.Selenium.IsElementPresent("//a[contains(@href, '/Group/Invite/')]"));

			// Verify remove individual link is not present
			test.Selenium.ClickAndWaitForPageToLoad("link=Roster");
			test.Selenium.ClickAndWaitForPageToLoad("link=Group Member");
            Assert.IsFalse(test.Selenium.IsElementPresent("link=Remove from group"));

			// Logout of infellowship
			test.infellowship.Logout();


            // Delete the Group Type
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
		}

		[Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Toggles the ability for leaders to edit records on and off.  Verifies the links are visible and aren't visisble.")]
        public void Security_Admin_Leaders_Edit_Records() {
            // Set up the data
            var groupTypeName = "EditRecords";
            var groupName = "EditRecordsGroup";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Member", "Member");

			// Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

			// Turn on the ability for leaders to edit records
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.EditRecords, true);

			// Logout
			test.Portal.Logout();


			// Navigate to InFellowship, view a group that you have leader rights to
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            test.infellowship.Groups_Group_View(groupName);

			// Verify edit individual link is present
			test.Selenium.ClickAndWaitForPageToLoad("link=Roster");
			test.Selenium.ClickAndWaitForPageToLoad("link=Group Member");
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.IndividualMemberPage.Link_EditMember);

			// Logout
			test.infellowship.Logout();


			// Revert the changes in portal
			test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

			// Turn off the ability for leaders to edit records
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.EditRecords, false);

			// Logout
			test.Portal.Logout();


			// Navigate to InFellowship, view a group that you have leader rights to
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            test.infellowship.Groups_Group_View(groupName);

			// Verify edit individual link is not present
			test.Selenium.ClickAndWaitForPageToLoad("link=Roster");
			test.Selenium.ClickAndWaitForPageToLoad("link=Group Member");
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.IndividualMemberPage.Link_EditMember);

			// Logout of infellowship
			test.infellowship.Logout();

            // Delete the Group Type
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
		}

		[Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Toggles the ability for leaders to edit the details of a group on and off.  Verifies the links are visible and aren't visisble.")]
		public void Security_Admin_Leaders_Edit_Details() {
            // Set up the data
            var groupTypeName = "EditDetails";
            var groupName = "EditDetailsGroup";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Member", "Member");

			// Login in to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

			// Turn on the ability for leaders to edit the details of a group
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.EditDetails, true);

			// Logout
			test.Portal.Logout();


			// Login to infellowship
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// Verify update bulletin board and edit details links are present for a group you lead.
            test.infellowship.Groups_Group_View_Settings(groupName);
			Assert.IsTrue(test.Selenium.IsElementPresent("link=Update bulletin board"));
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Edit details"));

			// Logout
			test.infellowship.Logout();


			// Revert the changes in portal
			test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

			// Turn off the ability for leaders to edit the details of a group
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.EditDetails, false);

			// Logout
			test.Portal.Logout();


            // Login to infellowship
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Verify update bulletin board and edit details links aren't present for a group you lead.
            test.infellowship.Groups_Group_View_Settings(groupName);
			Assert.IsFalse(test.Selenium.IsElementPresent("link=Update bulletin board"));
            Assert.IsFalse(test.Selenium.IsElementPresent("link=Edit details"));

			// Logout of infellowship
			test.infellowship.Logout();

            // Delete the Group Type
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
		}

		[Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Toggles the ability for leaders to edit the schedule and location of a group on and off.  Verifies the links are visible and aren't visisble.")]
		public void Security_Admin_Leaders_Edit_Schedule_And_Location() {
            // Set up the data
            var groupTypeName = "EditScheduleLocation";
            var groupName = "EditScheduleLocationGroup";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Member", "Member");
            
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

			// Turn on the ability for leaders to edit the schedule and location of a group
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.ChangeScheduleLocation, true);

			// Logout
			test.Portal.Logout();


			// Navigate to InFellowship
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");
           
            // View Settings of a group you lead
            test.infellowship.Groups_Group_View_Settings(groupName);

			// Verify create schedule and creation location links are present
			Assert.IsTrue(test.Selenium.IsElementPresent("//a[contains(@href, '/GroupSchedule/')]"));
            Assert.IsTrue(test.Selenium.IsElementPresent("//a[contains(@href, '/GroupLocation/')]"));

			// Logout
			test.infellowship.Logout();


			// Revert the changes in portal
			test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Turn off the ability for leaders to edit the schedule and location of a group
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.ChangeScheduleLocation, false);

			// Logout
			test.Portal.Logout();


            // Navigate to InFellowship
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View Settings of a group you lead
            test.infellowship.Groups_Group_View_Settings(groupName);

			// Verify create schedule and creation location links are not present
			Assert.IsFalse(test.Selenium.IsElementPresent("//a[contains(@href, '/GroupSchedule/')]"));
            Assert.IsFalse(test.Selenium.IsElementPresent("//a[contains(@href, '/GroupLocation/')]"));

			// Logout of infellowship
			test.infellowship.Logout();

            // Delete the Group Type
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
		}

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Toggles the ability for leaders to take attendance on and off.  Verifies the links are visible and aren't visisble.")]
        public void Security_Admin_Leaders_Edit_Take_Attendance() {

            // Set up the data
            var groupTypeName = "TakeAttendance";
            var groupName = "TakeAttendanceGroup";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Member", "Member");

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Turn on the ability for leaders to take attendance for a group
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.TakeAttendance, true);

            // Logout of Portal
            test.Portal.Logout();


            // Navigate to InFellowship
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a group
            test.infellowship.Groups_Group_View(groupName);

            // Verify the attendance tab is present
            test.Selenium.VerifyElementPresent("link=Attendance");
            
            // Logout of infellowship
            test.infellowship.Logout();


            // Revert the changes in portal
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Turn off the ability for leaders to take attendance for a group
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.TakeAttendance, false);

            // Logout of Portal
            test.Portal.Logout();


            // Navigate to InFellowship
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a group
            test.infellowship.Groups_Group_View(groupName);

            // Verify the attendance tab is not present
            test.Selenium.VerifyElementNotPresent("link=Attendance");

            // Logout of infellowship
            test.infellowship.Logout();

            // Delete the Group Type
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Toggles the ability for leaders to add someone to a group.  Verifies the links are visible and aren't visisble.")]
        public void Security_Admin_Leaders_Edit_Add_Someone() {

            // Set up the data
            var groupTypeName = "AddSomeone";
            var groupName = "AddSomeoneGroup";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Member", "Member");

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Turn on the ability for leaders to add people to a group for a group
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.AddSomeone, true);

            // Logout
            test.Portal.Logout();


            // Navigate to InFellowship, view a group that you have leader rights to
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            test.infellowship.Groups_Group_View_Roster(groupName);

            // Verify Invite someone links are present
            // Roster Page
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Add someone"));

            // Dashboard
            test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Add someone"));

            // Gear 
            //test.Selenium.Click("nav_gear");  Modify by Clark.Peng
            test.Selenium.ClickAndWaitForPageToLoad("//div[contains(@class,'hidden-xs')]//span[contains(text(),'Quick Links')]");
            Assert.IsTrue(test.Selenium.IsElementPresent("//a[contains(@href, '/Group/Invite/')]"));

            // Verify remove individual link is present
            test.Selenium.ClickAndWaitForPageToLoad("link=Roster");
            test.Selenium.ClickAndWaitForPageToLoad("link=Group Member");
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Remove from group"));

            // Verify the ability to invite the original prospect entered is not present
            test.infellowship.Groups_Group_View_Roster(groupName);
            test.Selenium.ClickAndWaitForPageToLoad("link=Add someone");
            test.Selenium.Type("FirstName", "Mark");
            test.Selenium.Type("LastName", "Lindsley");
            test.Selenium.ClickAndWaitForPageToLoad("btn_invite");

            test.Selenium.VerifyElementNotPresent("//div[@class='box_gutter_bottom']");
            test.Selenium.ClickAndWaitForPageToLoad("link=Start over");
            test.Selenium.ClickAndWaitForPageToLoad("link=Cancel");

            // Logout
            test.infellowship.Logout();

            // Revert the changes in portal
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Turn off the ability for leaders to add people to the group
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.AddSomeone, false);

            // Logout
            test.Portal.Logout();

            // Navigate to InFellowship, view a group that you have leader rights to
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            test.infellowship.Groups_Group_View_Roster(groupName);

            // Verify Invite Smeone links aren't present
            // Roster Page
            Assert.IsFalse(test.Selenium.IsElementPresent("link=Add someone"));

            // Dashboard
            test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
            Assert.IsFalse(test.Selenium.IsElementPresent("link=Add someone"));

            // Gear 
            //test.Selenium.Click("nav_gear"); Modify by Clark.Peng
            test.Selenium.ClickAndWaitForPageToLoad("//div[contains(@class,'hidden-xs')]//span[contains(text(),'Quick Links')]");
            Assert.IsFalse(test.Selenium.IsElementPresent("//a[contains(@href, '/Group/Invite/')]"));

            // Verify remove individual link is not present
            test.Selenium.ClickAndWaitForPageToLoad("link=Roster");
            test.Selenium.ClickAndWaitForPageToLoad("link=Group Member");
            Assert.IsFalse(test.Selenium.IsElementPresent("link=Remove from group"));

            // Logout of infellowship
            test.infellowship.Logout();

            // Delete the Group Type
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Toggles the ability for leaders to add someone to a group.  Verifies the links are visible and aren't visisble.")]
        public void Security_Admin_Leaders_Edit_Invite_Someone() {

            // Set up the data
            var groupTypeName = "InviteSomeone";
            var groupName = "InviteSomeoneToGroup";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Member", "Member");

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Turn on the ability for leaders to add people to a group for a group
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.InviteSomeone, true);

            // Logout
            test.Portal.Logout();


            // Navigate to InFellowship, view a group that you have leader rights to
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            test.infellowship.Groups_Group_View_Roster(groupName);

            // Verify Invite someone links are present
            // Roster Page
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Invite someone"));

            // Dashboard
            test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Invite someone"));

            // Gear 
            //test.Selenium.Click("nav_gear"); Modify by Clark.Peng
            test.Selenium.ClickAndWaitForPageToLoad("//div[contains(@class,'hidden-xs')]//span[contains(text(),'Quick Links')]");
            Assert.IsTrue(test.Selenium.IsElementPresent("//a[contains(@href, '/Group/Invite/')]"));

            // Verify remove individual link is present
            test.Selenium.ClickAndWaitForPageToLoad("link=Roster");
            test.Selenium.ClickAndWaitForPageToLoad("link=Group Member");
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Remove from group"));

            // Logout
            test.infellowship.Logout();

            // Revert the changes in portal
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Turn off the ability for leaders to add people to the group
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.InviteSomeone, false);

            // Logout
            test.Portal.Logout();

            // Navigate to InFellowship, view a group that you have leader rights to
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            test.infellowship.Groups_Group_View_Roster(groupName);

            // Verify Invite Smeone links aren't present
            // Roster Page
            Assert.IsFalse(test.Selenium.IsElementPresent("link=Invite someone"));

            // Dashboard
            test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
            Assert.IsFalse(test.Selenium.IsElementPresent("link=Invite someone"));

            // Gear 
            //test.Selenium.Click("nav_gear");  Modify by Clark.Peng
            test.Selenium.ClickAndWaitForPageToLoad("//div[contains(@class,'hidden-xs')]//span[contains(text(),'Quick Links')]");
            Assert.IsFalse(test.Selenium.IsElementPresent("//a[contains(@href, '/Group/Invite/')]"));

            // Verify remove individual link is not present
            test.Selenium.ClickAndWaitForPageToLoad("link=Roster");
            test.Selenium.ClickAndWaitForPageToLoad("link=Group Member");
            Assert.IsFalse(test.Selenium.IsElementPresent("link=Remove from group"));

            // Logout of infellowship
            test.infellowship.Logout();

            // Delete the Group Type
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
        }

        #endregion Admin Rights

		#region View Rights
		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Bryan Mikaelian")]
		[Description("Toggles the ability for leaders to view basic information.  Verifies the correct amount of information is show.")]
		public void Security_View_Leaders_Basic() {
            // Set up the data
            var groupTypeName = "LeaderBasic";
            var groupName = "LeaderBasicGroup";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Mark Lindsley", "Member");

			// Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

			// Grant leaders basic view rights
            test.Portal.Groups_GroupType_Update_ViewRights_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderViewRights.Basic);
			
            // Logout
			test.Portal.Logout();


			// Navigate to InFellowship, view a group that you have leader rights to
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            test.infellowship.Groups_Group_View_Roster(groupName);

			// View an individual of the group
			test.Selenium.ClickAndWaitForPageToLoad("link=Mark Lindsley");

			// Verify household information is present and additional groups are not present
            Assert.IsTrue(test.Selenium.IsElementPresent("css=h6:contains('Household')"));
            Assert.IsFalse(test.Selenium.IsElementPresent("css=table.grid"));

			// Logout of infellowship
			test.infellowship.Logout();

            // Delete the Group Type
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
		}

		[Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Toggles the ability for leaders to view limited information.  Verifies the correct amount of information is show.")]
		public void Security_View_Leaders_Limited() {
            // Set up the data
            var groupTypeName = "LeaderLimited";
            var groupName = "LeaderLimitedGroup";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Mark Lindsley", "Member");

            TestLog.WriteLine("SQL Added: " + _groupTypeName + " and " + groupName);
            TestLog.WriteLine("SQL Added Group Leader");
            TestLog.WriteLine("SQL Added Member Mark Lindsley");


			// Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Portal Login gta/BM.Admin09/QAEUNLX0C2");
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

			// Grant leaders limited view rights if they don't have it already
            TestLog.WriteLine("Grant leaders limited view rights if they don't have it already");
            test.Portal.Groups_GroupType_Update_ViewRights_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderViewRights.Limited);
			
			// Logout
            TestLog.WriteLine("Portal Logout");
			test.Portal.Logout();


			// Navigate to InFellowship, view a group that you have leader rights to
            TestLog.WriteLine("Infellowship Login");
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            TestLog.WriteLine("View Roser: " + groupName);
            test.infellowship.Groups_Group_View_Roster(groupName);

            // View an individual of the group
            TestLog.WriteLine("View Individual: Mark Lindsley");
            test.Selenium.ClickAndWaitForPageToLoad("link=Mark Lindsley");

            // Verify household information is not present and additional groups are not present
            TestLog.WriteLine("Verify household information is not present and additional groups are not present");
            Assert.IsFalse(test.Selenium.IsElementPresent("css=h6:contains('Household')"));
            Assert.IsFalse(test.Selenium.IsElementPresent("css=table.grid"));

			// Logout of infellowship
            TestLog.WriteLine("Infellowship Logout");
			test.infellowship.Logout();

            // Delete the Group Type
            TestLog.WriteLine("SQL Delete " + _groupTypeName);
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
		}

		[Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Toggles the ability for leaders to view full information.  Verifies the correct amount of information is show.")]
		public void Security_View_Leaders_Full() {
            // Set up the data
            var groupTypeName = "LeaderFull";
            var groupName = "LeaderFullGroup";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Mark Lindsley", "Member");

			// Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

			// Grant leaders full view rights
            test.Portal.Groups_GroupType_Update_ViewRights_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderViewRights.Full);
			
			// Logout
			test.Portal.Logout();

			// Navigate to InFellowship, view a group that you have leader rights to
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            test.infellowship.Groups_Group_View_Roster(groupName);

			// View an individual of the group
			test.Selenium.ClickAndWaitForPageToLoad("link=Mark Lindsley");

			// Verify household information is present and additional groups are present
            Assert.IsTrue(test.Selenium.IsElementPresent("css=h6:contains('Household')"));
            Assert.IsTrue(test.Selenium.IsElementPresent("css=table.grid"));

			// Logout of infellowship
			test.infellowship.Logout();

            // Delete the Group Type
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
		}
		#endregion View Rights
		#endregion Leaders

		#region Span of Care Owners
		#region Admin Rights
		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Bryan Mikaelian")]
		[Description("Toggles the ability for to email a group on and off.  Verifies the links are visible and aren't visisble for a Span of Care Owner.")]
		public void Security_Admin_Owners_Email_Group() {
            // Set up the data
            var groupTypeName = "LeadersEmail";
            var groupName = "Leaders Email Group";
            var socName = "Leaders Email Soc";
            List<string> groupTypes = new List<string>();
            groupTypes.Add(groupTypeName);

            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, socName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_SpanOfCare_Create(254, socName, "Bryan Mikaelian", "SOC Owner", groupTypes, null, GeneralEnumerations.Gender.NA, GeneralEnumerations.GroupMaritalStatus.NA, GeneralEnumerations.SpanOfCareChildCareSettings.NA); 


            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Turn on the ability for leaders to email the group
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.EmailGroup, true);

            // Logout
            test.Portal.Logout();


            // Navigate to InFellowship, view a group that you have owner rights to
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            test.infellowship.Groups_Group_View_Roster(socName, groupName);

            // Verify Email group links are present
            // Roster Page
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_SendAnEmail));

            // Dashboard
            test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_SendAnEmail));

            // Logout
            test.infellowship.Logout();

            // Revert the changes in portal
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Turn off the ability for leaders to email the group
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.EmailGroup, false);

            // Logout
            test.Portal.Logout();


            // Navigate to InFellowship, view a group that you have owner rights to
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            test.infellowship.Groups_Group_View_Roster(socName, groupName);

            // Verify Email group links are present
            // Roster Page
            Assert.IsFalse(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_SendAnEmail));

            // Dashboard
            test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
            Assert.IsFalse(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_SendAnEmail));

            // Logout of infellowship
            test.infellowship.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, socName);
		}

		[Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Toggles the ability for leaders to edit the schedule and location of a group on and off.  Verifies the links are visible and aren't visisble for a Span of Care Owner.")]
		public void Security_Admin_Owners_Edit_Schedule_And_Location() {
            // Set up the data
            var groupTypeName = "Owner Edit Schedule Location";
            var groupName = "Owner Edit Schedule Loc Group";
            var socName = "Edit Schedule Location Soc";
            List<string> groupTypes = new List<string>();
            groupTypes.Add(groupTypeName);

            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, socName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_SpanOfCare_Create(254, socName, "Bryan Mikaelian", "SOC Owner", groupTypes, null, GeneralEnumerations.Gender.NA, GeneralEnumerations.GroupMaritalStatus.NA, GeneralEnumerations.SpanOfCareChildCareSettings.NA); 

            // Login in to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Turn on the ability for leaders to edit the schedule and location of a group
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.ChangeScheduleLocation, true);

            // Logout
            test.Portal.Logout();


            // Navigate to InFellowship
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View Settings of a group you own
            test.infellowship.Groups_Group_View_Settings(socName, groupName);

            // Verify create schedule and creation location links are present
            Assert.IsTrue(test.Selenium.IsElementPresent("//a[contains(@href, '/GroupSchedule/')]"));
            Assert.IsTrue(test.Selenium.IsElementPresent("//a[contains(@href, '/GroupLocation/')]"));

            // Logout
            test.infellowship.Logout();


            // Revert the changes in portal
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Turn off the ability for leaders to edit the schedule and location of a group
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.ChangeScheduleLocation, false);

            // Logout
            test.Portal.Logout();


            // Navigate to InFellowship
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View Settings of a group you own
            test.infellowship.Groups_Group_View_Settings(socName, groupName);

            // Verify create schedule and creation location links are not present
            Assert.IsFalse(test.Selenium.IsElementPresent("//a[contains(@href, '/GroupSchedule/')]"));
            Assert.IsFalse(test.Selenium.IsElementPresent("//a[contains(@href, '/GroupLocation/')]"));

            // Logout of infellowship
            test.infellowship.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, socName);
        }

		[Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Toggles the ability for leaders to invite and add people to the group.  Verifies the links are visible and aren't visisble for Span of Care owners.")]
		public void Security_Admin_Owners_Invite_and_Add_Group() {
            // Set up the data
            var groupTypeName = "Owner Invite Add";
            var groupName = "Owner Invite Add Group";
            var socName = "Invite Add Soc";
            List<string> groupTypes = new List<string>();
            groupTypes.Add(groupTypeName);

            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, socName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_SpanOfCare_Create(254, socName, "Bryan Mikaelian", "SOC Owner", groupTypes, null, GeneralEnumerations.Gender.NA, GeneralEnumerations.GroupMaritalStatus.NA, GeneralEnumerations.SpanOfCareChildCareSettings.NA);
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Member");

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Turn on the ability for leaders to invite and add to the group.
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.InviteSomeone, true);
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.AddSomeone, true);

            // Logout
            test.Portal.Logout();


            // Navigate to InFellowship, view a group that you own
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            test.infellowship.Groups_Group_View_Roster(socName, groupName);

            // Verify Invite someone links are present
            // Roster Page
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Add or Invite someone"));

            // Dashboard
            test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Add or Invite someone"));

            // Gear 
            //test.Selenium.Click("nav_gear");Modify by Clark.Peng
            test.Selenium.ClickAndWaitForPageToLoad("//div[contains(@class,'hidden-xs')]//span[contains(text(),'Quick Links')]");
            Assert.IsTrue(test.Selenium.IsElementPresent("//a[contains(@href, '/Group/Invite/')]"));

            // Verify remove individual link is present
            test.Selenium.ClickAndWaitForPageToLoad("link=Roster");
            test.Selenium.ClickAndWaitForPageToLoad("link=Group Leader");
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Remove from group"));

            // Logout
            test.infellowship.Logout();


            // Revert the changes in portal
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Turn off the ability for leaders to invite and add to the group.
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.InviteSomeone, false);
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.AddSomeone, false);

            // Logout
            test.Portal.Logout();


            // Navigate to InFellowship, view a group that you own
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            test.infellowship.Groups_Group_View_Roster(socName, groupName);

            // Verify Invite Smeone links aren't present
            // Roster Page
            Assert.IsFalse(test.Selenium.IsElementPresent("link=Add or Invite someone"));

            // Dashboard
            test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");
            Assert.IsFalse(test.Selenium.IsElementPresent("link=Add or Invite someone"));

            // Gear 
            //test.Selenium.Click("nav_gear");Modify by Clark.Peng
            test.Selenium.ClickAndWaitForPageToLoad("//div[contains(@class,'hidden-xs')]//span[contains(text(),'Quick Links')]");
            Assert.IsFalse(test.Selenium.IsElementPresent("//a[contains(@href, '/Group/Invite/')]"));

            // Verify remove individual link is not present
            test.Selenium.ClickAndWaitForPageToLoad("link=Roster");
            test.Selenium.ClickAndWaitForPageToLoad("link=Group Leader");
            Assert.IsFalse(test.Selenium.IsElementPresent("link=Remove from group"));

            // Logout of infellowship
            test.infellowship.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, socName);

        }

		[Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Toggles the ability for leaders to edit the details of a group on and off.  Verifies the links are visible and aren't visisble for Span of Care owners.")]
		public void Security_Admin_Owners_Edit_Details() {
            // Set up the data
            var groupTypeName = "Owner Edit Details";
            var groupName = "Owner Edit Details Group";
            var socName = "Edit Details Soc";
            List<string> groupTypes = new List<string>();
            groupTypes.Add(groupTypeName);

            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, socName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_SpanOfCare_Create(254, socName, "Bryan Mikaelian", "SOC Owner", groupTypes, null, GeneralEnumerations.Gender.NA, GeneralEnumerations.GroupMaritalStatus.NA, GeneralEnumerations.SpanOfCareChildCareSettings.NA);

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Turn on the ability for leaders to edit the details of a group
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.EditDetails, true);

            // Logout
            test.Portal.Logout();

            // Login to infellowship
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Verify update bulletin board and edit details links are present for a group you own.
            test.infellowship.Groups_Group_View_Settings(socName, groupName);
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Update bulletin board"));
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Edit details"));

            // Logout
            test.infellowship.Logout();


            // Revert the changes in portal
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Turn off the ability for leaders to edit the details of a group
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.EditDetails, false);


            // Logout
            test.Portal.Logout();

            // Login to infellowship
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Verify update bulletin board and edit details links aren't present for a group you own.
            test.infellowship.Groups_Group_View_Settings(socName, groupName);
            Assert.IsFalse(test.Selenium.IsElementPresent("link=Update bulletin board"));
            Assert.IsFalse(test.Selenium.IsElementPresent("link=Edit details"));

            // Logout of infellowship
            test.infellowship.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, socName);
        }

		[Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Toggles the ability for leaders to edit records on and off.  Verifies the links are visible and aren't visisble for Span of Care owners.")]
		public void Security_Admin_Owners_Edit_Records() {
            // Set up the data
            var groupTypeName = "Owner Edit Records";
            var groupName = "Owner Edit Records Group";
            var socName = "Edit Records Soc";
            List<string> groupTypes = new List<string>();
            groupTypes.Add(groupTypeName);

            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, socName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_SpanOfCare_Create(254, socName, "Bryan Mikaelian", "SOC Owner", groupTypes, null, GeneralEnumerations.Gender.NA, GeneralEnumerations.GroupMaritalStatus.NA, GeneralEnumerations.SpanOfCareChildCareSettings.NA);
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Turn on the ability for leaders to edit records
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.EditRecords, true);

            // Logout
            test.Portal.Logout();


            // Navigate to InFellowship, view a group that you own
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            test.infellowship.Groups_Group_View(socName, groupName);

            // Verify edit individual link is present
            test.Selenium.ClickAndWaitForPageToLoad("link=Roster");
            test.Selenium.ClickAndWaitForPageToLoad("link=Group Leader");
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.IndividualMemberPage.Link_EditMember));

            // Logout
            test.infellowship.Logout();


            // Revert the changes in portal
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Turn off the ability for leaders to edit records
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.EditRecords, false);

            // Logout
            test.Portal.Logout();


            // Navigate to InFellowship, view a group that you own
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            test.infellowship.Groups_Group_View(socName, groupName);

            // Verify edit individual link is not present
            test.Selenium.ClickAndWaitForPageToLoad("link=Roster");
            test.Selenium.ClickAndWaitForPageToLoad("link=Group Leader");
            Assert.IsFalse(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.IndividualMemberPage.Link_EditMember));

            // Logout of infellowship
            test.infellowship.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, socName);
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Toggles the ability for leaders to take attendance on and off.  Verifies the links are visible and aren't visible for Span of Care owners.")]
        public void Security_Admin_Owners_Edit_Take_Attendance() {
            // Set up the data
            var groupTypeName = "Owner Attendance";
            var groupName = "Owner Attendance Group";
            var socName = "Attendance Soc";
            List<string> groupTypes = new List<string>();
            groupTypes.Add(groupTypeName);

            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, socName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_SpanOfCare_Create(254, socName, "Bryan Mikaelian", "SOC Owner", groupTypes, null, GeneralEnumerations.Gender.NA, GeneralEnumerations.GroupMaritalStatus.NA, GeneralEnumerations.SpanOfCareChildCareSettings.NA);

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Turn on the ability for leaders to take attendance for a group
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.TakeAttendance, true);

            // Logout of Portal
            test.Portal.Logout();


            // Navigate to InFellowship
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a group
            test.infellowship.Groups_Group_View(socName, groupName);

            // Verify the attendance tab is present
            test.Selenium.VerifyElementPresent("link=Attendance");

            // Logout of infellowship
            test.infellowship.Logout();


            // Revert the changes in portal
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Turn off the ability for leaders to take attendance for a group
            test.Portal.Groups_GroupType_Update_AdminRight_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderAdminRights.TakeAttendance, false);

            // Logout of Portal
            test.Portal.Logout();


            // Navigate to InFellowship
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a group
            test.infellowship.Groups_Group_View(socName, groupName);

            // Verify the attendance tab is not present
            test.Selenium.VerifyElementNotPresent("link=Attendance");

            // Logout of infellowship
            test.infellowship.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, socName);
        }
		#endregion Admin Rights

		#region View Rights
		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Bryan Mikaelian")]
		[Description("Toggles the ability for leaders to view limited information.  Verifies the correct amount of information is shown for a Span of Care Owner.")]
		public void Security_View_Owners_Limited() {
            // Set up the data
            var groupTypeName = "Owner Limited View";
            var groupName = "Owner Limited View Group";
            var socName = "Limited View Soc";
            List<string> groupTypes = new List<string>();
            groupTypes.Add(groupTypeName);

            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, socName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_SpanOfCare_Create(254, socName, "Bryan Mikaelian", "SOC Owner", groupTypes, null, GeneralEnumerations.Gender.NA, GeneralEnumerations.GroupMaritalStatus.NA, GeneralEnumerations.SpanOfCareChildCareSettings.NA);
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Mark Lindsley", "Member");

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Grant leaders limited view rights if they don't have it already
            test.Portal.Groups_GroupType_Update_ViewRights_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderViewRights.Limited);

            // Logout
            test.Portal.Logout();


            // Navigate to InFellowship, view a group that you own
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            test.infellowship.Groups_Group_View_Roster(socName, groupName);

            // View an individual of the group
            test.Selenium.ClickAndWaitForPageToLoad("link=Mark Lindsley");

            // Verify household information is not present and additional groups are not present
            Assert.IsFalse(test.Selenium.IsElementPresent("css=h6:contains('Household')"));
            Assert.IsFalse(test.Selenium.IsElementPresent("css=table.grid"));

            // Logout of infellowship
            test.infellowship.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, socName);
		}

		[Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Toggles the ability for leaders to view basic information.  Verifies the correct amount of information is shown for a Span of Care Owner.")]
		public void Security_View_Owners_Basic() {
            // Set up the data
            var groupTypeName = "Owner Basic View";
            var groupName = "Owner Basic View Group";
            var socName = "Basic View Soc";
            List<string> groupTypes = new List<string>();
            groupTypes.Add(groupTypeName);

            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, socName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_SpanOfCare_Create(254, socName, "Bryan Mikaelian", "SOC Owner", groupTypes, null, GeneralEnumerations.Gender.NA, GeneralEnumerations.GroupMaritalStatus.NA, GeneralEnumerations.SpanOfCareChildCareSettings.NA);
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Mark Lindsley", "Member");

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Grant leaders basic view rights
            test.Portal.Groups_GroupType_Update_ViewRights_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderViewRights.Basic);

            // Logout
            test.Portal.Logout();


            // Navigate to InFellowship, view a group that own
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            test.infellowship.Groups_Group_View_Roster(socName, groupName);

            // View an individual of the group
            test.Selenium.ClickAndWaitForPageToLoad("link=Mark Lindsley");

            // Verify household information is present and additional groups are not present
            Assert.IsTrue(test.Selenium.IsElementPresent("css=h6:contains('Household')"));
            Assert.IsFalse(test.Selenium.IsElementPresent("css=table.grid"));

            // Logout of infellowship
            test.infellowship.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, socName);
		}


		[Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Toggles the ability for leaders to view full information.  Verifies the correct amount of information is shown for Span of Care owners.")]
		public void Security_View_Owners_Full() {
            // Set up the data
            var groupTypeName = "Owner Full View";
            var groupName = "Owner Full View Group";
            var socName = "Full View Soc";
            List<string> groupTypes = new List<string>();
            groupTypes.Add(groupTypeName);

            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, socName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));
            base.SQL.Groups_SpanOfCare_Create(254, socName, "Bryan Mikaelian", "SOC Owner", groupTypes, null, GeneralEnumerations.Gender.NA, GeneralEnumerations.GroupMaritalStatus.NA, GeneralEnumerations.SpanOfCareChildCareSettings.NA);
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Mark Lindsley", "Member");

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Grant leaders full view rights
            test.Portal.Groups_GroupType_Update_ViewRights_Leader(groupTypeName, GeneralEnumerations.GroupTypeLeaderViewRights.Full);

            // Logout
            test.Portal.Logout();

            // Navigate to InFellowship, view a group that you own
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            test.infellowship.Groups_Group_View_Roster(socName, groupName);

            // View an individual of the group
            test.Selenium.ClickAndWaitForPageToLoad("link=Mark Lindsley");

            // Verify household information is present and additional groups are present
            Assert.IsTrue(test.Selenium.IsElementPresent("css=h6:contains('Household')"));
            Assert.IsTrue(test.Selenium.IsElementPresent("css=table.grid"));

            // Logout of infellowship
            test.infellowship.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, socName);
		}
		#endregion View Rights
		#endregion Span of Care Owners
	}
}