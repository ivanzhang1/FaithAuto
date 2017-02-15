using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Selenium;
using ActiveUp.Net.Mail;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;


namespace FTTests.infellowship.Groups {
    // All email will route to infellowship.group.tests@gmail.com / password: BM.Admin09
	[TestFixture]
	public class infellowship_Groups_ProspectWorkflow : FixtureBase {

        #region Private Members
        private string _socName = "Prospect Span of Care";
        private string _groupTypeName = "Prospect Group Type";
        private string _groupTypeName2 = "Prospect Group Type 2";
        private string _groupName = "Prospect Group";
        private string _groupName2 = "Prospect Group 2";
        private string _groupName3 = "Prospect Group 3";
        private string _socAddPersonGroup = "Prospect Group Add Owner";
        #endregion Private Members

        #region Fixture Setup

        [FixtureSetUp]
        public void FixtureSetUp() {
            // Groups
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName2);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", _groupTypeName, new List<int> { 1, 2, 3, 4, 5, 9, 10, 11, 12 });
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", _groupTypeName2, new List<int> { 1, 2, 3, 4, 5, 9, 10, 11, 12 });
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(), GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, false, true, null, true);
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Member", "Member");
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _socAddPersonGroup, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, _socAddPersonGroup, "Group Leader", "Leader");
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _groupName2, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName2, "Group Leader", "Leader");
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", _groupTypeName, new List<int> { 1, 2, 3, 4, 5, 9, 10, 11, 12 });
            base.SQL.Groups_Group_Create(254, _groupTypeName2, "Bryan Mikaelian", _groupName3, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(), GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, false, true, null, true);
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName3, "Group Leader", "Leader");

            // Span of Care
            base.SQL.Groups_SpanOfCare_Create(254, _socName, "Bryan Mikaelian", "SOC Owner", new List<string>() { _groupTypeName });
        }

        #endregion Fixture Setup

        #region Fixture Teardown

        [FixtureTearDown]
        public void FixtureTearDown() {
            //base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName2);
            base.SQL.Groups_SpanOfCare_Delete(254, _socName);
        }

        #endregion Fixture Teardown

		#region Invite / Add 

        #region Invite
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Bryan Mikaelian")]
		[Description("Verifies the required fields for inviting an individual to a group as a leader.")]
		public void ProspectWorkflow_Invite_Individual_Required_Fields_Leader() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Invite someone to a group without any of the required fields
            test.infellowship.Groups_Prospect_Invite(_groupName, null, null, null, null, null, false, false);

			// Logout of infellowship
			test.infellowship.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies the required fields for inviting an individual to a group as a Span of Care Owner.")]
		public void ProspectWorkflow_Invite_Individual_Required_Fields_Owner() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Invite someone to a group, unde your span of care, without any of the required fields
            test.infellowship.Groups_Prospect_Invite(_socName, _groupName, null, null, null, null, null, false, false); ;

			// Logout
			test.infellowship.Logout();
		}

        [Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Verifies a leader can invite an individual to a group.")]
		public void ProspectWorkflow_Invite_Individual_Leader() {
            // Set initial conditions
            int churchId = 254;
            string firstName = "Test";
            string lastName = "Invite";
            string email = "infellowship.group.tests@gmail.com";
            base.SQL.People_DeleteProspectInfo(churchId, _groupName, firstName, lastName, email);

			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// Invite a prospect to a group
            test.infellowship.Groups_Prospect_Invite(_groupName, firstName, lastName, email, null, null, false, false);

            // Logout of infellowship
            test.infellowship.Logout();

            // Attempt to view the resulting page
            test.Selenium.Open(string.Format("{0}/GroupOptIn/Index/{1}", test.infellowship.URL, base.SQL.Groups_GetProspectActivationCode(churchId, email, _groupName)));
            test.Selenium.VerifyTextPresent("You have been approved to join:");
            test.Selenium.VerifyTextPresent(_groupName);
            test.Selenium.VerifyTextPresent("To officially join, please click \"Join this group\" below. If you've changed your mind, click \"Decline.\"");

			// Login to Portal
			test.Portal.Login("groupadmin", "BM.Admin09", "QAEUNLX0C2");

			// View the prospects of the Group 
            test.Portal.Groups_Group_View_Prospects(_groupName);

			// Verify prospect exists
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_Group_ActiveProspects, "Test Invite", "Name"));
            decimal itemRow = test.GeneralMethods.GetTableRowNumber(TableIds.Groups_Group_ActiveProspects, "Test Invite", "Name");
            Assert.AreEqual("Invited", test.Selenium.GetTable(string.Format("{0}.{1}.4", TableIds.Groups_Group_ActiveProspects, itemRow)), "Prospect status was not correct.");

			// Logout of Portal
			test.Portal.Logout();
		}

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a leader cannot invite to the group that is already in the group.")]
        public void ProspectWorkflow_Invite_Cannot_Invite_Individuals_In_Group_Leader() {
            var firstName = "Group";
            var lastName = "Leader";
            var email = "group.leader.ft@gmail.com";

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Invite a prospect to a group that already is in the group
            test.infellowship.Groups_Prospect_Invite(_groupName, firstName, lastName, email, null, null, false, false);

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a span of care owner cannot invite to the group that is already in the group.")]
        public void ProspectWorkflow_Invite_Cannot_Invite_Individuals_In_Group_Owner() {
            var firstName = "Group";
            var lastName = "Leader";
            var email = "group.leader.ft@gmail.com";

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Invite someone to the group, under your span of care, that is already in the group.
            test.infellowship.Groups_Prospect_Invite(_socName, _groupName, firstName, lastName, email, null, null, false, false);

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Verifies a span of care owner can invite an individual to a group.")]
		public void ProspectWorkflow_Invite_Individual_Owner() {
            // Set initial conditions
            int churchId = 254;
            string firstName = "Owner";
            string lastName = "Invite";
            string email = "infellowship.group.tests@gmail.com";
            base.SQL.People_DeleteProspectInfo(churchId, _groupName, firstName, lastName, email);

			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// Invite a prospect to a group
            test.infellowship.Groups_Prospect_Invite(_socName, _groupName, firstName, lastName, email, null, null, false, false);

            // Logout of infellowship
            test.infellowship.Logout();

            // Attempt to view the resulting page
            test.Selenium.Open(string.Format("{0}/GroupOptIn/Index/{1}", test.infellowship.URL, base.SQL.Groups_GetProspectActivationCode(churchId, "infellowship.group.tests@gmail.com", _groupName)));
            test.Selenium.VerifyTextPresent("You have been approved to join:");
            test.Selenium.VerifyTextPresent(_groupName);
            test.Selenium.VerifyTextPresent("To officially join, please click \"Join this group\" below. If you've changed your mind, click \"Decline.\"");

			// Login to portal
			test.Portal.Login("groupadmin", "BM.Admin09", "QAEUNLX0C2");

			// View the prospects of the Group 
            test.Portal.Groups_Group_View_Prospects(_groupName);

			// Verify prospect exists
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_Group_ActiveProspects, "Owner Invite", "Name"));
            decimal itemRow = test.GeneralMethods.GetTableRowNumber(TableIds.Groups_Group_ActiveProspects, "Owner Invite", "Name");
            Assert.AreEqual("Invited", test.Selenium.GetTable(string.Format("{0}.{1}.4", TableIds.Groups_Group_ActiveProspects, itemRow)), "Prospect status was not correct.");

			// Logout of Portal
			test.Portal.Logout();
		}

        [Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you are taken to the Invite Someone Page if you navigate to it via the gear menu.")]
		public void ProspectWorkflow_Invite_View_Page_From_Gear_Menu_Leader() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View a Group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Open the Gear Menu
			test.Selenium.Click(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);

			// Select Invite Someone
			test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.NavigationLinks.Gear_InviteSomeone);

			// Verify you are on the invite page
			test.Selenium.IsTextPresent("Invite someone");

			test.Selenium.VerifyElementPresent("FirstName");
            test.Selenium.VerifyElementPresent("LastName");
			test.Selenium.VerifyElementPresent("Email");
			test.Selenium.VerifyElementPresent("Phone");

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("link=Cancel");

			// Logout of infellowship
			test.infellowship.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies, as a span of care owner, you are taken to the Invite Someone Page if you navigate to it via the gear menu.")]
		public void ProspectWorkflow_Invite_View_Page_From_Gear_Menu_Owner() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View your Span of Care
            test.infellowship.Groups_Group_View_Roster(_socName, _groupName);

			// Open the Gear Menu
			test.Selenium.Click(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);

			// Select Invite Someone
			test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.NavigationLinks.Gear_InviteSomeone);

			// Verify you are on the invite page
			test.Selenium.IsTextPresent("Invite someone");

			test.Selenium.VerifyElementPresent("FirstName");
            test.Selenium.VerifyElementPresent("LastName");
			test.Selenium.VerifyElementPresent("Email");
			test.Selenium.VerifyElementPresent("Phone");

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("link=Cancel");

			// Logout of infellowship
			test.infellowship.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you are taken to the Invite Someone Page after clicking on the sidebar link on the Dashboard Page.")]
		public void ProspectWorkflow_Invite_View_Page_From_Dashboard_Invite_Someone_Link_Leader() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a Group's dashboard
            test.infellowship.Groups_Group_View_Dashboard(_groupName);

			// Click on the Invite Someone sidebar Link
			test.Selenium.ClickAndWaitForPageToLoad("link=Add or Invite someone");

			// Verify you are on the Invite Someone page
			test.Selenium.IsTextPresent("Invite someone");

			test.Selenium.VerifyElementPresent("FirstName");
            test.Selenium.VerifyElementPresent("LastName");
			test.Selenium.VerifyElementPresent("Email");
			test.Selenium.VerifyElementPresent("Phone");

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("link=Cancel");

			// Logout
			test.infellowship.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies, as a span of care owner, you are taken to the Invite Someone Page after clicking on the sidebar link on the Dashboard Page.")]
		public void ProspectWorkflow_Invite_View_Page_From_Dashboard_Invite_Someone_Link_Owner() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a Group's dashboard
            test.infellowship.Groups_Group_View_Dashboard(_socName, _groupName);

			// Click on the Invite Someone sidebar Link
			test.Selenium.ClickAndWaitForPageToLoad("link=Add or Invite someone");

			// Verify you are on the Invite Someone page
			test.Selenium.IsTextPresent("Invite someone");

			test.Selenium.VerifyElementPresent("FirstName");
            test.Selenium.VerifyElementPresent("LastName");
			test.Selenium.VerifyElementPresent("Email");
			test.Selenium.VerifyElementPresent("Phone");

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("link=Cancel");

			// Logout
			test.infellowship.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you are taken to the Invite Prospect Page if you hit the invite link on the Roster page.")]
		public void ProspectWorkflow_Invite_View_Page_From_Roster_Leader() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a Group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Click on the Invite Someone Link
			test.Selenium.ClickAndWaitForPageToLoad("link=Add or Invite someone");

			// Verify you are on the invite page
			test.Selenium.IsTextPresent("Invite someone");

			test.Selenium.VerifyElementPresent("FirstName");
            test.Selenium.VerifyElementPresent("LastName");
			test.Selenium.VerifyElementPresent("Email");
			test.Selenium.VerifyElementPresent("Phone");

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("link=Cancel");

			// Logout
			test.infellowship.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies, as a span of care owner, you are taken to the Invite Prospect Page if you hit the invite link on the Roster page.")]
		public void ProspectWorkflow_Invite_View_Page_From_Roster_Owner() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a Group in your Span of Care
            test.infellowship.Groups_Group_View_Roster(_socName, _groupName);

			// Click on the Invite Someone Link
			test.Selenium.ClickAndWaitForPageToLoad("link=Add or Invite someone");

			// Verify you are on the invite page
			test.Selenium.IsTextPresent("Invite someone");

			test.Selenium.VerifyElementPresent("FirstName");
            test.Selenium.VerifyElementPresent("LastName");
			test.Selenium.VerifyElementPresent("Email");
			test.Selenium.VerifyElementPresent("Phone");

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("link=Cancel");

			// Logout
			test.infellowship.Logout();
		}

        //commented by ivan.zhang, This case is rewritten with webdriver as below.
		//[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you are taken back to the Roster Page if you hit the Cancel link on the Invite Someone page.")]
		public void ProspectWorkflow_Invite_Page_Cancel_to_Roster_Leader() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View a Group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Click on the Invite Someone Link
			test.Selenium.ClickAndWaitForPageToLoad("link=Add or Invite someone");

			// Select the Cancel Link
            test.Selenium.ClickAndWaitForPageToLoad(".//a[@class='btn btn-danger btn-lg']");

			// Verify you are on the roster page
			test.Selenium.VerifyElementPresent("link=Dashboard");
			test.Selenium.VerifyElementPresent("link=Roster");
			test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);

			// side bar
			test.Selenium.VerifyElementPresent("link=View prospects");
			test.Selenium.VerifyElementPresent("link=Add or Invite someone");
			test.Selenium.VerifyElementPresent("link=Send an email");

			// Logout
			test.infellowship.Logout();
		}

        //commented by ivan.zhang, This case is rewritten with webdriver as below.
		//[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies, as span of care owner, you are taken back to the Roster Page if you hit the Cancel link on the Invite Someone page.")]
		public void ProspectWorkflow_Invite_Page_Cancel_to_Roster_Owner() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a Group in your Span of Care
            test.infellowship.Groups_Group_View_Roster(_socName, _groupName);

			// Click on the Invite Someone Link
			test.Selenium.ClickAndWaitForPageToLoad("link=Add or Invite someone");

			// Select the Cancel Link
			test.Selenium.ClickAndWaitForPageToLoad("css=p > a.cancel");

			// Verify you are on the roster page
			test.Selenium.VerifyElementPresent("link=Dashboard");
			test.Selenium.VerifyElementPresent("link=Roster");
			test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);

			// side bar
			test.Selenium.VerifyElementPresent("link=View prospects");
			test.Selenium.VerifyElementPresent("link=Add or Invite someone");
			test.Selenium.VerifyElementPresent("link=Send an email");

			// Logout of infellowship
			test.infellowship.Logout();
		}

        #region Matching Rules

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that an individual, with an InFellowship account, can be invited to join a group and they are able to go through the workflow to join the group")]
        public void ProspectWorkflow_Invite_Individual_Has_InFellowship_Account_Has_Individual_Record() {
            // Initial Conditions
            var firstName = "Span";
            var lastName = "Owner";
            var email = "socowner@gmail.com";

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Invite a prospect to a group
            test.infellowship.Groups_Prospect_Invite(_groupName3, firstName, lastName, email, null, null, false, false);

            // Logout of infellowship
            test.infellowship.Logout();

            // As the prospect, confirm that you want to join the group.  Finish up the regisration process to join.
            test.Selenium.Open(string.Format("{0}/GroupOptIn/Index/{1}", test.infellowship.URL, base.SQL.Groups_GetProspectActivationCode(254, email, _groupName3)));
            test.Selenium.ClickAndWaitForPageToLoad("//input[@value='Join this group']");

            // Verify we just have to login
            // Full registration is not present.
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.DateControl_DateOfBirth);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.RadioButton_Gender_Male);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.RadioButton_Gender_Female);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.DropDown_Country);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_StreetOne);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_StreetTwo);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_City);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.DropDown_State);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_ZipCode);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_HomePhone);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_MobilePhone);

            // Account creation is not present.
            test.Selenium.VerifyElementNotPresent("password");
            test.Selenium.VerifyElementNotPresent("confirmpassword");
            Assert.IsFalse(test.Selenium.IsTextPresent("Please create an account to complete the join the process."));

            // Logging in is present.
            test.Selenium.VerifyElementPresent("login_password");
            test.Selenium.VerifyTextPresent("Please login to complete the join process");

            // Log in to finish joining
            test.Selenium.Type("login_password", "BM.Admin09");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify your group is present
            test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName3));

            // View the group
            test.infellowship.Groups_Group_View_Dashboard(_groupName3);

            // Verify you are a member of the group
            test.Selenium.VerifyElementPresent("link=Roster");
            test.Selenium.VerifyElementPresent("link=Dashboard");
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Prospects);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_ViewSettings);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard_Wrench);

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that if an InFellowship user is invited to a group and they don't finish the joining process after accepting the invitation, they can come back at a later time and finish up the prospect workflow.")]
        public void ProspectWorkflow_Invite_Individual_Has_InFellowship_Account_Has_Individual_Record_Finish_Later() {
            // Initial Conditions
            var firstName = "Group";
            var lastName = "Member";
            var email = "group.member.ft@gmail.com";
            
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Invite a prospect to a group
            test.infellowship.Groups_Prospect_Invite(_groupName2, firstName, lastName, email, null, null, false, false);

            // Logout of infellowship
            test.infellowship.Logout();

            // As the prospect, confirm that you want to join the group.  Finish up the regisration process to join.
            test.Selenium.Open(string.Format("{0}/GroupOptIn/Index/{1}", test.infellowship.URL, base.SQL.Groups_GetProspectActivationCode(254, email, _groupName2)));
            test.Selenium.ClickAndWaitForPageToLoad("//input[@value='Join this group']");

            // As the prospect, get distracted and go do something else.
            test.Selenium.Open("http://www.google.com");

            // Attempt to finish the registration process
            test.Selenium.Open(string.Format("{0}/GroupOptIn/Index/{1}", test.infellowship.URL, base.SQL.Groups_GetProspectActivationCode(254, email, _groupName2)));

            // Verify we just have to login
            // Full registration is not present.
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.DateControl_DateOfBirth);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.RadioButton_Gender_Male);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.RadioButton_Gender_Female);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.DropDown_Country);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_StreetOne);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_StreetTwo);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_City);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.DropDown_State);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_ZipCode);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_HomePhone);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_MobilePhone);

            // Account creation is not present.
            test.Selenium.VerifyElementNotPresent("password");
            test.Selenium.VerifyElementNotPresent("confirmpassword");
            Assert.IsFalse(test.Selenium.IsTextPresent("Please create an account to complete the join the process."));

            // Logging in is present.
            test.Selenium.VerifyElementPresent("login_password");
            test.Selenium.VerifyTextPresent("Please login to complete the join process");

            // Login to finish joining
            test.Selenium.Type("login_password", "BM.Admin09");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify your group is present
            test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName2));

            // View the group
            test.infellowship.Groups_Group_View_Dashboard(_groupName2);

            // Verify you are a member of the group
            test.Selenium.VerifyElementPresent("link=Roster");
            test.Selenium.VerifyElementPresent("link=Dashboard");
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Prospects);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_ViewSettings);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard_Wrench);

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that an individual with an Individual Record but no InFellowship Account can be invited to join a group and they are able to go through the workflow to join the group")]
        public void ProspectWorkflow_Invite_Individual_No_InFellowship_Account_Has_Individual_Record() {
            // Initial Conditions
            var firstName = "InFellowship";
            var lastName = "Prospect";
            var emailAddress = "infellowship.prospect.ft@gmail.com";
            base.SQL.People_InFellowshipAccount_Delete(254, emailAddress);

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Invite a prospect to a group that has an record individual record but no InFellowship Account
            test.infellowship.Groups_Prospect_Invite(_groupName, firstName, lastName, emailAddress, null, null, false, false);

            //Logout
            test.infellowship.Logout();

            // As the prospect, confirm that you want to join the group.  Finish up the regisration process to join.
            test.Selenium.Open(string.Format("{0}/GroupOptIn/Index/{1}", test.infellowship.URL, base.SQL.Groups_GetProspectActivationCode(254, emailAddress, _groupName)));
            test.Selenium.ClickAndWaitForPageToLoad("//input[@value='Join this group']");

            // Verify we just have to just create an InFellowship Account
            // Full Registration is not present.
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.DateControl_DateOfBirth);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.RadioButton_Gender_Male);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.RadioButton_Gender_Female);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.DropDown_Country);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_StreetOne);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_StreetTwo);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_City);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.DropDown_State);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_ZipCode);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_HomePhone);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_MobilePhone);

            // Logging In is not present since don't have an account
            test.Selenium.VerifyElementNotPresent("login_password");
            Assert.IsFalse(test.Selenium.IsTextPresent("Please login to complete the join process"));

            // Creating an account is present.
            test.Selenium.VerifyElementPresent("password");
            test.Selenium.VerifyElementPresent("confirmpassword");
            test.Selenium.VerifyTextPresent("In order to join this group, you will need to create an account.");

            // Create an account to finish joining
            test.Selenium.Type("password", "BM.Admin09");
            test.Selenium.Type("confirmpassword", "BM.Admin09");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify banner text
            TestLog.WriteLine(string.Format("Banner Text: {0}", test.Selenium.GetText("success_message").Trim().Replace("\r", string.Empty).Replace("\n", string.Empty)));
            Assert.AreEqual("Close You have been successfully added to the group", test.Selenium.GetText("success_message").Trim().Replace("\r", string.Empty).Replace("\n", string.Empty));
            test.Selenium.Click("success_close");

            // Login
            test.infellowship.ReLoginInFellowship(emailAddress, "BM.Admin09", "QAEUNLX0C2");

            // Go to Group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourGroups);

            // Verify your group is present
            test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName));

            // View the group
            test.infellowship.Groups_Group_View_Dashboard(_groupName);

            // Verify you are a member of the group
            test.Selenium.VerifyElementPresent("link=Roster");
            test.Selenium.VerifyElementPresent("link=Dashboard");
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Prospects);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_ViewSettings);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard_Wrench);

            // Logout of infellowship
            test.infellowship.Logout();

            // Login again, verifying your new account works
            test.infellowship.Login(emailAddress, "BM.Admin09");

            // View the group
            test.infellowship.Groups_Group_View_Dashboard(_groupName);

            // Verify you are a member of the group
            test.Selenium.VerifyElementPresent("link=Roster");
            test.Selenium.VerifyElementPresent("link=Dashboard");
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Prospects);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_ViewSettings);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard_Wrench);

            // Logout of infellowship
            test.infellowship.Logout();

            // Clean up
            base.SQL.People_InFellowshipAccount_Delete(254, emailAddress);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that if an individual with an Individual Record but no InFellowship Account is invited to join a group and they don't finish the joining process after accepting the invitation, they are able to go through the workflow to join the group at a later date.")]
        public void ProspectWorkflow_Invite_Individual_No_InFellowship_Account_Has_Individual_Record_Finish_Later() {
            // Initial Conditions
            var firstName = "Some";
            var lastName = "Prospect";
            var emailAddress = "some.prospect.ft@gmail.com";
            base.SQL.People_InFellowshipAccount_Delete(254, emailAddress);

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            //Invite a prospect to a group that has an record individual record but no InFellowship Account
            test.infellowship.Groups_Prospect_Invite(_groupName, firstName, lastName, emailAddress, null, null, false, false);

            //Logout
            test.infellowship.Logout();

            // As the prospect, confirm that you want to join the group.  Finish up the regisration process to join.
            test.Selenium.Open(string.Format("{0}/GroupOptIn/Index/{1}", test.infellowship.URL, base.SQL.Groups_GetProspectActivationCode(254, emailAddress, _groupName)));
            test.Selenium.ClickAndWaitForPageToLoad("//input[@value='Join this group']");

            // Get distracted and go do something else.
            test.Selenium.Open("http://www.google.com");

            // Attempt to finish the registration process
            test.Selenium.Open(string.Format("{0}/GroupOptIn/Index/{1}", test.infellowship.URL, base.SQL.Groups_GetProspectActivationCode(254, emailAddress, _groupName)));

            // Verify we just have to just create an InFellowship Account
            // Full Registration is not present.
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.DateControl_DateOfBirth);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.RadioButton_Gender_Male);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.RadioButton_Gender_Female);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.DropDown_Country);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_StreetOne);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_StreetTwo);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_City);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.DropDown_State);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_ZipCode);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_HomePhone);
            test.Selenium.VerifyElementNotPresent(People.AccountConstants.AdditionalInformationPage.TextField_MobilePhone);

            // Logging In is not present since you don't have an account            
            test.Selenium.VerifyElementNotPresent("login_password");
            Assert.IsFalse(test.Selenium.IsTextPresent("Please login to complete the join process"));

            // Creating an account is present.
            test.Selenium.VerifyElementPresent("password");
            test.Selenium.VerifyElementPresent("confirmpassword");
            test.Selenium.VerifyTextPresent("In order to join this group, you will need to create an account.");

            // Create an account to finish joining
            test.Selenium.Type("password", "BM.Admin09");
            test.Selenium.Type("confirmpassword", "BM.Admin09");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify banner text
            TestLog.WriteLine(string.Format("Banner Text: {0}", test.Selenium.GetText("success_message").Trim().Replace("\r", string.Empty).Replace("\n", string.Empty)));
            Assert.AreEqual("Close You have been successfully added to the group", test.Selenium.GetText("success_message").Trim().Replace("\r", string.Empty).Replace("\n", string.Empty));
            test.Selenium.Click("success_close");

            // Login
            test.infellowship.ReLoginInFellowship(emailAddress, "BM.Admin09", "QAEUNLX0C2");
            
            // Go to Group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourGroups);

            // Verify your group is present
            test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName));

            // View the group
            test.infellowship.Groups_Group_View_Dashboard(_groupName);

            // Verify you are a member of the group
            test.Selenium.VerifyElementPresent("link=Roster");
            test.Selenium.VerifyElementPresent("link=Dashboard");
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Prospects);

            test.infellowship.Groups_Group_View_Dashboard(_groupName);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_ViewSettings);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard_Wrench);

            // Logout of infellowship
            test.infellowship.Logout();

            // Attempt to registration process
            test.Selenium.Open(string.Format("{0}/GroupOptIn/Index/{1}", test.infellowship.URL, base.SQL.Groups_GetProspectActivationCode(254, emailAddress, _groupName)));
            
             // Verify banner text 
             TestLog.WriteLine(string.Format("Banner Text: {0}", test.Selenium.GetText("success_message").Trim().Replace("\r", string.Empty).Replace("\n", string.Empty)));
             Assert.AreEqual("Close You've already used this invitation code.", test.Selenium.GetText("success_message").Trim().Replace("\r", string.Empty).Replace("\n", string.Empty));
             test.Selenium.Click("success_close");

            // Clean up
            base.SQL.People_InFellowshipAccount_Delete(254, emailAddress);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that an individual with no Individual Record and no InFellowship Account can be invited to join a group and they are able to go through the workflow to join the group")]
        public void ProspectWorkflow_Invite_Individual_No_InFellowship_Account_No_Individual_Record() {
            // Prepare the information
            string firstName = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Millisecond.ToString();
            string lastName = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Second.ToString();
            string emailAddress = string.Format("{0}.{1}.{2}@gmail.com", firstName, lastName, firstName);

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Invite a prospect to a group that does not have an individual record and does not have an InFellowship Account
            test.infellowship.Groups_Prospect_Invite(_groupName, firstName, lastName, emailAddress, null, null, false, false);

            // Logout of infellowship
            test.infellowship.Logout();

            // As the prospect, confirm that you want to join the group.  Finish up the regisration process to join.
            test.Selenium.Open(string.Format("{0}/GroupOptIn/Index/{1}", test.infellowship.URL, base.SQL.Groups_GetProspectActivationCode(254, emailAddress, _groupName)));
            test.Selenium.ClickAndWaitForPageToLoad("//input[@value='Join this group']");

            // Register to finish joining
            test.Selenium.Type("password", "BM.Admin09");
            test.Selenium.Type("confirmpassword", "BM.Admin09");

            test.Selenium.Type(People.AccountConstants.AdditionalInformationPage.DateControl_DateOfBirth, "11/15/1985");
            test.Selenium.Click(People.AccountConstants.AdditionalInformationPage.RadioButton_Gender_Male);
            test.Selenium.Type(People.AccountConstants.AdditionalInformationPage.TextField_StreetOne, "2812 Meadow Wood Drive");
            test.Selenium.Type(People.AccountConstants.AdditionalInformationPage.TextField_City, "Flower Mound");
            test.Selenium.Select(People.AccountConstants.AdditionalInformationPage.DropDown_State, "Texas");
            test.Selenium.Type(People.AccountConstants.AdditionalInformationPage.TextField_ZipCode, "75022");
            test.Selenium.Type(People.AccountConstants.AdditionalInformationPage.TextField_HomePhone, "214-513-1354");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify banner text
            TestLog.WriteLine(string.Format("Banner Text: {0}", test.Selenium.GetText("success_message").Trim().Replace("\r", string.Empty).Replace("\n", string.Empty)));
            Assert.AreEqual("Close Your account has been successfully activated.You have been successfully added to the group", test.Selenium.GetText("success_message").Trim().Replace("\r", string.Empty).Replace("\n", string.Empty));
            test.Selenium.Click("success_close");

            // Login
            test.infellowship.Login(emailAddress, "BM.Admin09","QAEUNLX0C2");

            // Go to Group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourGroups);

            // Verify your group is present
            test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName));

            // View the group
            test.infellowship.Groups_Group_View_Dashboard(_groupName);

            // Verify you are a member of the group
            test.Selenium.VerifyElementPresent("link=Roster");
            test.Selenium.VerifyElementPresent("link=Dashboard");
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Prospects);

            test.infellowship.Groups_Group_View_Dashboard(_groupName);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_ViewSettings);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard_Wrench);

            // Logout of infellowship
            test.infellowship.Logout();

            // Merge the created individual
            base.SQL.People_MergeIndividual(254, string.Format("{0} {1}", firstName, lastName), "Merge Dump");
            base.SQL.People_InFellowshipAccount_Delete(254, emailAddress);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that an individual with no Individual Record and no InFellowship Account can be invited to join a group and they are able to go through the workflow to join the group.  Also specifies the home number as preferred at the final account creation.")]
        public void ProspectWorkflow_Invite_Individual_No_InFellowship_Account_No_Individual_Record_Home_Preferred() {
            // Prepare the information
            string firstName = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Millisecond.ToString();
            string lastName = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Second.ToString();
            string emailAddress = string.Format("{0}.{1}.{2}@gmail.com", firstName, lastName, firstName);
            base.SQL.People_InFellowshipAccount_Delete(254, emailAddress);

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Invite a prospect to a group that does not have an individual record and does not have an InFellowship Account
            test.infellowship.Groups_Prospect_Invite(_groupName, firstName, lastName, emailAddress, null, null, false, false);

            // Logout of infellowship
            test.infellowship.Logout();

            // As the prospect, confirm that you want to join the group.  Finish up the regisration process to join.
            test.Selenium.Open(string.Format("{0}/GroupOptIn/Index/{1}", test.infellowship.URL, base.SQL.Groups_GetProspectActivationCode(254, emailAddress, _groupName)));
            test.Selenium.ClickAndWaitForPageToLoad("//input[@value='Join this group']");

            // Register to finish joining
            test.Selenium.Type("password", "BM.Admin09");
            test.Selenium.Type("confirmpassword", "BM.Admin09");

            test.Selenium.Type(People.AccountConstants.AdditionalInformationPage.DateControl_DateOfBirth, "11/15/1985");
            test.Selenium.Click(People.AccountConstants.AdditionalInformationPage.RadioButton_Gender_Male);
            test.Selenium.Type(People.AccountConstants.AdditionalInformationPage.TextField_StreetOne, "2812 Meadow Wood Drive");
            test.Selenium.Type(People.AccountConstants.AdditionalInformationPage.TextField_City, "Flower Mound");
            test.Selenium.Select(People.AccountConstants.AdditionalInformationPage.DropDown_State, "Texas");
            test.Selenium.Type(People.AccountConstants.AdditionalInformationPage.TextField_ZipCode, "75022");
            test.Selenium.Type(People.AccountConstants.AdditionalInformationPage.TextField_HomePhone, "214-513-1354");

            // Specify the home as preferred
            test.Selenium.Click("//a[@class='select_primary' and text()='home']");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify banner text
            TestLog.WriteLine(string.Format("Banner Text: {0}", test.Selenium.GetText("success_message").Trim().Replace("\r", string.Empty).Replace("\n", string.Empty)));
            Assert.AreEqual("Close Your account has been successfully activated.You have been successfully added to the group", test.Selenium.GetText("success_message").Trim().Replace("\r", string.Empty).Replace("\n", string.Empty));
            test.Selenium.Click("success_close");

            // Login
            test.infellowship.Login(emailAddress, "BM.Admin09", "QAEUNLX0C2");

            // Verify the home number is preferred.
            test.infellowship.People_ProfileEditor_View_UpdatePage();
            Assert.IsTrue(test.Selenium.IsChecked("//fieldset[2]/table/tbody/tr[4]/td[3]/input"), "Home number was marked as preferred but a different value was saved or no value was saved.");

            // Logout of infellowship
            test.infellowship.Logout();

            // Merge the created individual
            base.SQL.People_MergeIndividual(254, string.Format("{0} {1}", firstName, lastName), "Merge Dump");
            base.SQL.People_InFellowshipAccount_Delete(254, emailAddress);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that an individual with no Individual Record and no InFellowship Account can be invited to join a group and they are able to go through the workflow to join the group.  Also specifies the mobile number as preferred at the final account creation.")]
        public void ProspectWorkflow_Invite_Individual_No_InFellowship_Account_No_Individual_Record_Mobile_Preferred() {
            // Prepare the information
            string firstName = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Millisecond.ToString();
            string lastName = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Second.ToString();
            string emailAddress = string.Format("{0}.{1}.{2}@gmail.com", firstName, lastName, firstName);
            base.SQL.People_InFellowshipAccount_Delete(254, emailAddress);

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Invite a prospect to a group that does not have an individual record and does not have an InFellowship Account
            test.infellowship.Groups_Prospect_Invite(_groupName, firstName, lastName, emailAddress, null, null, false, false);

            // Logout of infellowship
            test.infellowship.Logout();

            // As the prospect, confirm that you want to join the group.  Finish up the regisration process to join.
            test.Selenium.Open(string.Format("{0}/GroupOptIn/Index/{1}", test.infellowship.URL, base.SQL.Groups_GetProspectActivationCode(254, emailAddress, _groupName)));
            test.Selenium.ClickAndWaitForPageToLoad("//input[@value='Join this group']");

            // Register to finish joining
            test.Selenium.Type("password", "BM.Admin09");
            test.Selenium.Type("confirmpassword", "BM.Admin09");

            test.Selenium.Type(People.AccountConstants.AdditionalInformationPage.DateControl_DateOfBirth, "11/15/1985");
            test.Selenium.Click(People.AccountConstants.AdditionalInformationPage.RadioButton_Gender_Male);
            test.Selenium.Type(People.AccountConstants.AdditionalInformationPage.TextField_StreetOne, "2812 Meadow Wood Drive");
            test.Selenium.Type(People.AccountConstants.AdditionalInformationPage.TextField_City, "Flower Mound");
            test.Selenium.Select(People.AccountConstants.AdditionalInformationPage.DropDown_State, "Texas");
            test.Selenium.Type(People.AccountConstants.AdditionalInformationPage.TextField_ZipCode, "75022");
            test.Selenium.Type(People.AccountConstants.AdditionalInformationPage.TextField_MobilePhone, "214-513-1354");

            // Specify the mobile as preferred
            test.Selenium.Click("//a[@class='select_primary' and text()='mobile']");

            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify banner text
            TestLog.WriteLine(string.Format("Banner Text: {0}", test.Selenium.GetText("success_message").Trim().Replace("\r", string.Empty).Replace("\n", string.Empty)));
            Assert.AreEqual("Close Your account has been successfully activated.You have been successfully added to the group", test.Selenium.GetText("success_message").Trim().Replace("\r", string.Empty).Replace("\n", string.Empty));
            test.Selenium.Click("success_close");

            // Login
            test.infellowship.Login(emailAddress, "BM.Admin09", "QAEUNLX0C2");

            // Verify the mobile number is marked as preferred
            test.infellowship.People_ProfileEditor_View_UpdatePage();
            Assert.IsTrue(test.Selenium.IsChecked("//fieldset[2]/table/tbody/tr[2]/td[3]/input"), "Mobile number was marked as preferred but a different value was saved or no value was saved.");

            // Logout of infellowship
            test.infellowship.Logout();

            // Merge the created individual
            base.SQL.People_MergeIndividual(254, string.Format("{0} {1}", firstName, lastName), "Merge Dump");
            base.SQL.People_InFellowshipAccount_Delete(254, emailAddress);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that if an individual with no Individual Record and no InFellowship Account is invited to join a group and they don't finish the joining process after accepting the invitation, they are able to go through the workflow to join the group at a later date.")]
        public void ProspectWorkflow_Invite_Individual_No_InFellowship_Account_No_Individual_Record_Finish_Later() {
            // Intialize test data
            string firstName = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Millisecond.ToString();
            string lastName = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Second.ToString();
            string emailAddress = string.Format("{0}.{1}.{2}@gmail.com", firstName, lastName, firstName);

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Invite a prospect to a group that does not have an individual record and does not have an InFellowship Account
            test.infellowship.Groups_Prospect_Invite(_groupName, firstName, lastName, emailAddress, null, null, false, false);

            // Logout of infellowship
            test.infellowship.Logout();


            // As the prospect, confirm that you want to join the group.  Finish up the regisration process to join.
            test.Selenium.Open(string.Format("{0}/GroupOptIn/Index/{1}", test.infellowship.URL, base.SQL.Groups_GetProspectActivationCode(254, emailAddress, _groupName)));
            test.Selenium.ClickAndWaitForPageToLoad("//input[@value='Join this group']");

            // Get distracted and go do something else.
            test.Selenium.Open("http://www.google.com");

            // Attempt to finish the registration process
            test.Selenium.Open(string.Format("{0}/GroupOptIn/Index/{1}", test.infellowship.URL, base.SQL.Groups_GetProspectActivationCode(254, emailAddress, _groupName)));

            // Register to finish joining
            test.Selenium.Type("password", "BM.Admin09");
            test.Selenium.Type("confirmpassword", "BM.Admin09");

            test.Selenium.Type(People.AccountConstants.AdditionalInformationPage.DateControl_DateOfBirth, "11/15/1985");
            test.Selenium.Click(People.AccountConstants.AdditionalInformationPage.RadioButton_Gender_Male);
            test.Selenium.Type(People.AccountConstants.AdditionalInformationPage.TextField_StreetOne, "2812 Meadow Wood Drive");
            test.Selenium.Type(People.AccountConstants.AdditionalInformationPage.TextField_City, "Flower Mound");
            test.Selenium.Select(People.AccountConstants.AdditionalInformationPage.DropDown_State, "Texas");
            test.Selenium.Type(People.AccountConstants.AdditionalInformationPage.TextField_ZipCode, "75022");
            test.Selenium.Type(People.AccountConstants.AdditionalInformationPage.TextField_HomePhone, "214-513-1354");

            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify banner text
            TestLog.WriteLine(string.Format("Banner Text: {0}", test.Selenium.GetText("success_message").Trim().Replace("\r", string.Empty).Replace("\n", string.Empty)));
            Assert.AreEqual("Close Your account has been successfully activated.You have been successfully added to the group", test.Selenium.GetText("success_message").Trim().Replace("\r", string.Empty).Replace("\n", string.Empty));
            test.Selenium.Click("success_close");

            // Login
            test.infellowship.Login(emailAddress, "BM.Admin09", "QAEUNLX0C2");

            // Go to Group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourGroups);

            // Verify your group is present
            test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName));

            // View the group
            test.infellowship.Groups_Group_View_Dashboard(_groupName);

            // Verify you are a member of the group
            test.Selenium.VerifyElementPresent("link=Roster");
            test.Selenium.VerifyElementPresent("link=Dashboard");
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Prospects);

            test.infellowship.Groups_Group_View_Dashboard(_groupName);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_ViewSettings);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard_Wrench);

            // Logout of infellowship
            test.infellowship.Logout();

            // Merge the created individual
            base.SQL.People_MergeIndividual(254, string.Format("{0} {1}", firstName, lastName), "Merge Dump");
            base.SQL.People_InFellowshipAccount_Delete(254, emailAddress);
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the text displayed when a prospect is Invited but has not created an InFellowship account.")]
        public void ProspectWorkflow_Invite_Individual_No_InFellowship_Account_Finish_Later_Text_Correct() {
            // Set initial conditions
            string groupName = "A Test Group";
            string firstName = "InFellowship";
            string lastName = "Prospect";
            string email = "InFellowshipProspect@gmail.com";
            base.SQL.People_DeleteProspectInfo(15, groupName, firstName, lastName, email);

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

            // Invite someone to join the group
            test.infellowship.Groups_Prospect_Invite(groupName, firstName, lastName, email, null, null, false, false);

            // Logout of infellowship
            test.infellowship.Logout();

            // Open the page to simulate the user clicking the link sent in the email
            test.Selenium.Open(string.Format("{0}/GroupOptIn/Index/{1}", test.infellowship.URL, base.SQL.Groups_GetProspectActivationCode(15, email, groupName)));

            // Click to join the group
            test.Selenium.ClickAndWaitForPageToLoad("//input[@value='Join this group']");

            // Login to infellowship
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

            // Navigate to the prospects tab
            test.infellowship.Groups_Group_View_Prospects(groupName);

            // Click the prospect name
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Prospect");

            // Verify text message exists
            test.Selenium.VerifyTextPresent("InFellowship has accepted the invitation to join, pending account creation.");

            // Logout of infellowship
            test.infellowship.Logout();
        }

        #endregion Matching Rules

        #endregion Invite

        #region Add Someone

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a leader can directly add someone to a group")]
        public void ProspectWorkflow_Invite_Add_Individual_Leader() {

            var firstName = "Group";
            var lastName = "Member";
            var email = "group.member.ft@gmail.com";

            // Log in to InFellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Add someone to the group
            test.infellowship.Groups_Prospect_Invite(_groupName, firstName, lastName, email, null, null, true, false);

            // Verify they are in the group
            test.infellowship.Groups_Group_View_Roster(_groupName);
            test.Selenium.VerifyElementPresent("link=Group Member");

            // Verify they are not a prospect
            test.infellowship.Groups_Group_View_Prospects(_groupName);

            if (test.Selenium.IsElementPresent(TableIds.Groups_Group_ActiveProspects)) {
                Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_Group_ActiveProspects, "Group Member", "Name"), "Added individual was also shown as an open prospect!");
            }

            // Logout
            test.infellowship.Logout();

            // Login as that member
            test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

            // Verify you are in the group as a member
            // View the group
            test.infellowship.Groups_Group_View(_groupName);

            // Verify you are a member of the group
            test.Selenium.VerifyElementPresent("link=Dashboard");
            test.Selenium.VerifyElementNotPresent("link=Prospects");

            test.infellowship.Groups_Group_View_Dashboard(_groupName);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_ViewSettings);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard_Wrench);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a Span of Care owner can directly add someone to a group")]
        public void ProspectWorkflow_Invite_Add_Individual_Owner() {

            var firstName = "Group";
            var lastName = "Member";
            var email = "group.member.ft@gmail.com";

            // Log in to InFellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Add someone to the group
            test.infellowship.Groups_Prospect_Invite(_socName, _socAddPersonGroup, firstName, lastName, email, null, null, true, false);

            // Verify they are in the group
            test.infellowship.Groups_Group_View_Roster(_groupName);
            test.Selenium.VerifyElementPresent("link=Group Member");

            // Verify they are not a prospect
            test.infellowship.Groups_Group_View_Prospects(_socName, _socAddPersonGroup);

            if (test.Selenium.IsElementPresent(TableIds.Groups_Group_ActiveProspects)) {
                Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_Group_ActiveProspects, "Group Member", "Name"), "Added individual was also shown as an open prospect!");
            }

            // Logout
            test.infellowship.Logout();

            // Login as that member
            test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

            // View the group
            test.infellowship.Groups_Group_View_Dashboard(_socAddPersonGroup);

            // Verify you are a member of the group
            test.Selenium.VerifyElementPresent("link=Dashboard");
            test.Selenium.VerifyElementNotPresent("link=Prospects");

            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_ViewSettings);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard);
            test.Selenium.VerifyElementNotPresent(GroupsConstants.GroupManagement.DashboardPage.Link_UpdateBulletinBoard_Wrench);

            // Logout
            test.infellowship.Logout();
        }

        #endregion Add

        #endregion Invite / Add

        #region Tell a Friend
        [Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Sends a 'Tell a friend' to an individual, as a Leader of the group")]
		public void ProspectWorkflow_TellAFriend_As_Leader() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a group you lead
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Tell a friend
			test.infellowship.Groups_Prospect_TellAFriend("Test", "Person", "infellowship.group.tests@gmail.com", "You should join my group! I am a leader");

			// Logout of infellowship
			test.infellowship.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Sends a 'Tell a friend' to an individual, as an owner.")]
		public void ProspectWorkflow_TellAFriend_As_Owner() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View a group in your Span of Care
            test.infellowship.Groups_Group_View_Roster(_socName, _groupName);

			// Tell a friend
            test.infellowship.Groups_Prospect_TellAFriend("Test", "Person", "infellowship.group.tests@gmail.com", "You should join my group! I am a Span of Care owner.");


			// Logout of infellowship
			test.infellowship.Logout();
		}

		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Bryan Mikaelian")]
		[Description("Sends a 'Tell a friend' to an individual as a Member")]
		public void ProspectWorkflow_TellAFriend_As_Member() {
            // Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View a group you are a member of
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Tell a friend
			test.infellowship.Groups_Prospect_TellAFriend("Test", "Person", "infellowship.group.tests@gmail.com", "You should join my group! I am a member");

			// Logout
			test.infellowship.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies an email address must be in the correct format when sending a 'Tell a Friend'.")]
		public void ProspectWorkflow_TellAFriend_Invalid_Email_Format() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View a group 
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Tell a friend
			test.Selenium.Click(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);
			test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.NavigationLinks.Gear_TellAFriend);

			// Fill out the information with an invalid email address
			test.Selenium.Type(GroupsConstants.GroupManagement.TellAFriendPage.TextField_FirstName, "Test");
			test.Selenium.Type(GroupsConstants.GroupManagement.TellAFriendPage.TextField_LastName, "Person");
			test.Selenium.Type(GroupsConstants.GroupManagement.TellAFriendPage.TextField_Email, "bmikaelian");

			// Submit
			test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

			// Verify error message
			Assert.IsTrue(test.Selenium.IsTextPresent("Email is not a valid email address."));

			// Logout
			test.infellowship.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies the required fields for Tell a Friend.")]
		public void ProspectWorkflow_TellAFriend_Required_Fields() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a group 
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Tell a friend
			test.Selenium.Click(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);
			test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.NavigationLinks.Gear_TellAFriend);

			// Submit
			test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

			// Verify text
			Assert.IsTrue(test.Selenium.IsTextPresent("First Name is required."));
			Assert.IsTrue(test.Selenium.IsTextPresent("Last Name is required."));
			Assert.IsTrue(test.Selenium.IsTextPresent("Email is required."));

			// Logout
			test.infellowship.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies the public URL for Tell a Friend goes to the group's public page.")]
		public void ProspectWorkflow_TellAFriend_Public_URL_Correct() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a group 
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Tell a friend
			test.Selenium.Click(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);
			test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.NavigationLinks.Gear_TellAFriend);

			// Verify the public URL
            test.Selenium.VerifyElementPresent("//a[contains(@href, '/GroupTypes')]");

			// Logout
			test.infellowship.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you are taken to the Tell a Friend Page if you hit the link on the Gear Menu")]
		public void ProspectWorkflow_TellAFriend_View_Page_From_Gear() {
			// Login to infellowship - Groups
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a group 
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Open the Gear Menu
			test.Selenium.Click(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);

			// Select Tell a friend
			test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.NavigationLinks.Gear_TellAFriend);

			// Verify you are on the Tell a friend page
			Assert.IsTrue(test.Selenium.IsTextPresent("Tell a friend"));

			Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.TellAFriendPage.TextField_FirstName));
			Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.TellAFriendPage.TextField_LastName));
			Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.TellAFriendPage.TextField_Email));
			Assert.IsTrue(test.Selenium.IsElementPresent(GroupsConstants.GroupManagement.TellAFriendPage.TextField_PersonalNote));

			// Logout of infellowship
			test.infellowship.Logout();
		}
		#endregion Tell a Friend

		#region View Prospects

		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you are taken to the Prospects Page after clicking on the sidebar number link on the Dashboard Page.")]
		public void ProspectWorkflow_View_Prospects_Page_From_Dashboard_Prospect_Number_Link() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a group 
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Click on the Dashboard Link
			test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");

			// Click on the Prospect Number Link
			test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.DashboardPage.Link_ProspectsNumLink);

			// Verify you are on prospects page
			test.Selenium.VerifyElementPresent("link=Dashboard");
			test.Selenium.VerifyElementPresent("link=Roster");
			test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);

			test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName));
			Assert.IsTrue(test.Selenium.IsTextPresent("Prospects"));

			// Logout of infellowship
			test.infellowship.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you are taken to the View Prospects page if you click on the View prospects link from the roster page.")]
		public void ProspectWorkflow_View_Prospects_Page_From_Roster_View_Prospects_Link() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a group 
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Click on the View Prospects Link
			test.Selenium.ClickAndWaitForPageToLoad("link=View prospects");

			// Verify you are on the prospect page
			test.Selenium.VerifyElementPresent("link=Dashboard");
			test.Selenium.VerifyElementPresent("link=Roster");
			test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);

			test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName));
			Assert.IsTrue(test.Selenium.IsTextPresent("Prospects"));

			// Logout of infellowship
			test.infellowship.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you are taken to the Dashboard if you hit the Dashboard link on View Prospects page, while viewing a specific group")]
		public void ProspectWorkflow_View_Prospects_Page_From_Dashboard() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a Group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Click on the View Prospects Link
			test.Selenium.ClickAndWaitForPageToLoad("link=View prospects");

			// Click on the Dashboard link
			test.Selenium.ClickAndWaitForPageToLoad("link=Dashboard");

			// Verify you are on the Dashboard page
			test.Selenium.VerifyElementPresent("link=Dashboard");
			test.Selenium.VerifyElementPresent("link=Roster");
			test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);

			test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName));

			// Side links
			test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.DashboardPage.Link_ViewRoster);
			test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.DashboardPage.Link_ViewSettings);
			test.Selenium.VerifyElementPresent("link=Add or Invite someone");
			test.Selenium.VerifyElementPresent("link=Send an email");

			// Logout of infellowship
			test.infellowship.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you are taken to the Invite Prospect Page if you hit the invite link on the Prospects page that has no prospects.  This link is only present if the group has 0 prospects")]
		public void ProspectWorkflow_View_Prospects_View_Invite_Page() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a group 
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Click on the View Prospects Link
			test.Selenium.ClickAndWaitForPageToLoad("link=View prospects");

			// Invite someone using the link that is present when a group has no prospects
			test.Selenium.Click(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);
			test.Selenium.ClickAndWaitForPageToLoad(GroupsConstants.GroupManagement.NavigationLinks.Gear_InviteSomeone);

			// Verify you are on the invite page
			test.Selenium.VerifyElementPresent("FirstName");
            test.Selenium.VerifyElementPresent("LastName");
			test.Selenium.VerifyElementPresent("Email");
			test.Selenium.VerifyElementPresent("Phone");

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("link=Cancel");

			// Logout of infellowship
			test.infellowship.Logout();
		}

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies a prospect can be deleted.")]
        public void ProspectWorkflow_Delete_Prospect() {
            // Set initial conditions
            int churchId = 15;
            string groupName = "A Test Group";
            string firstName = "Prospect";
            string lastName = "Delete";
            string email = "prospect.delete@gmail.com";
            base.SQL.People_DeleteProspectInfo(churchId, groupName, firstName, lastName, email);

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

            // Invite a prospect to a group
            test.infellowship.Groups_Prospect_Invite(groupName, firstName, lastName, email, null, null, false, false);

            // Logout of infellowship
            test.infellowship.Logout();

            // Login to portal
            test.Portal.Login();

            // View the prospects of the Group 
            test.Portal.Groups_Group_View_Prospects(groupName);

            // Verify prospect exists
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_Group_ActiveProspects, string.Format("{0} {1}", firstName, lastName), "Name"));
            decimal itemRow = test.GeneralMethods.GetTableRowNumber(TableIds.Groups_Group_ActiveProspects, string.Format("{0} {1}", firstName, lastName), "Name");
            Assert.AreEqual("Invited", test.Selenium.GetTable(string.Format("{0}.{1}.4", TableIds.Groups_Group_ActiveProspects, itemRow)), "Prospect status was not correct.");

            // Delete this prospect to the test can be re-ran           
            test.Portal.Groups_Group_Delete_Prospect(groupName, string.Format("{0} {1}", firstName, lastName));

            // Logout of Portal
            test.Portal.Logout();
        }
		#endregion View Prospects

	}


    //Add the webdriver class by ivan.zhang
    public class infellowship_Groups_ProspectWorkFlow_Webdriver : FixtureBaseWebDriver
    {
        #region Private Members
        private string _socName = "Prospect Span of Care";
        private string _groupTypeName = "Prospect Group Type";
        private string _groupTypeName2 = "Prospect Group Type 2";
        private string _groupName = "Prospect Group";
        private string _groupName2 = "Prospect Group 2";
        private string _groupName3 = "Prospect Group 3";
        private string _socAddPersonGroup = "Prospect Group Add Owner";
        #endregion Private Members

        #region Fixture Setup

        [FixtureSetUp]
        public void FixtureSetUp()
        {
            // Groups
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName2);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", _groupTypeName, new List<int> { 1, 2, 3, 4, 5, 9, 10, 11, 12 });
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", _groupTypeName2, new List<int> { 1, 2, 3, 4, 5, 9, 10, 11, 12 });
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(), GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, false, true, null, true);
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Member", "Member");
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _socAddPersonGroup, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, _socAddPersonGroup, "Group Leader", "Leader");
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _groupName2, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName2, "Group Leader", "Leader");
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", _groupTypeName, new List<int> { 1, 2, 3, 4, 5, 9, 10, 11, 12 });
            base.SQL.Groups_Group_Create(254, _groupTypeName2, "Bryan Mikaelian", _groupName3, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(), GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, false, true, null, true);
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName3, "Group Leader", "Leader");

            // Span of Care
            base.SQL.Groups_SpanOfCare_Create(254, _socName, "Bryan Mikaelian", "SOC Owner", new List<string>() { _groupTypeName });
        }

        #endregion Fixture Setup

        #region Fixture Teardown

        [FixtureTearDown]
        public void FixtureTearDown()
        {
            //base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName2);
            base.SQL.Groups_SpanOfCare_Delete(254, _socName);
        }

        #endregion Fixture Teardown

        #region invite
        //rewrite by ivan.zhang 1/25/2016
        [Test, RepeatOnFailure]
        [Author("Ivan Zhang")]
        [Description("Verifies you are taken back to the Roster Page if you hit the Cancel link on the Invite Someone page.")]
        public void ProspectWorkflow_Invite_Page_Cancel_to_Roster_Leader_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a Group
            test.Infellowship.Groups_Group_View_Roster_WebDriver(_groupName);

            // Click on the Invite Someone Link
            test.Driver.FindElementByLinkText("Add or Invite someone").Click();
           
            // Select the Cancel Link
            test.GeneralMethods.WaitForElementDisplayed(By.XPath(".//a[@class='btn btn-danger btn-lg']"));
            test.Driver.FindElementByXPath(".//a[@class='btn btn-danger btn-lg']").Click();

            // Verify you are on the roster page
            test.GeneralMethods.WaitForElementVisible(By.LinkText("Roster"),30,"roster is not displayed!");
            test.GeneralMethods.VerifyElementDisplayedWebDriver(By.LinkText("Roster"));
            test.GeneralMethods.VerifyElementDisplayedWebDriver(By.LinkText("Dashboard"));
            test.GeneralMethods.VerifyElementDisplayedWebDriver(By.XPath(".//*[@id='main']/div[5]/div/div/span"));

            // side bar
            test.GeneralMethods.VerifyElementDisplayedWebDriver(By.LinkText("View prospects"));
            test.GeneralMethods.VerifyElementDisplayedWebDriver(By.LinkText("Add or Invite someone"));
            test.GeneralMethods.VerifyElementDisplayedWebDriver(By.LinkText("Send an email"));

            // Logout
            test.Infellowship.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Ivan Zhang")]
        [Description("Verifies, as span of care owner, you are taken back to the Roster Page if you hit the Cancel link on the Invite Someone page.")]
        public void ProspectWorkflow_Invite_Page_Cancel_to_Roster_Owner_WebDriver()
        {
            //// Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            //test.infellowship.Login

            //// View a Group in your Span of Care
            test.Infellowship.Groups_Group_View_Roster_WebDriver(_socName,_groupName);

            //// Click on the Invite Someone Link
            test.Driver.FindElementByLinkText("Add or Invite someone").Click();

            //// Select the Cancel Link
            test.GeneralMethods.WaitForElementDisplayed(By.XPath(".//a[@class='btn btn-danger btn-lg']"));
            test.Driver.FindElementByXPath(".//a[@class='btn btn-danger btn-lg']").Click();

            //// Verify you are on the roster page
            test.GeneralMethods.WaitForElementVisible(By.LinkText("Roster"), 30, "roster is not displayed!");
            test.GeneralMethods.VerifyElementDisplayedWebDriver(By.LinkText("Roster"));
            test.GeneralMethods.VerifyElementDisplayedWebDriver(By.LinkText("Dashboard"));
            test.GeneralMethods.VerifyElementDisplayedWebDriver(By.XPath(".//*[@id='main']/div[5]/div/div/span"));

            //// side bar
            test.GeneralMethods.VerifyElementDisplayedWebDriver(By.LinkText("View prospects"));
            test.GeneralMethods.VerifyElementDisplayedWebDriver(By.LinkText("Add or Invite someone"));
            test.GeneralMethods.VerifyElementDisplayedWebDriver(By.LinkText("Send an email"));

            //// Logout of infellowship
            test.Infellowship.LogoutWebDriver();
        }
        #endregion invite

    }
}