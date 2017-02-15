
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Selenium;

namespace FTTests.infellowship.Groups {
	[TestFixture]
    public class infellowship_Groups_SpanOfCare : FixtureBase {

        #region Private Members
        private string _groupName = "General SOC Group";
        private string _groupTypeName = "Attendance SOC Data";
        private string _socName = "General SOC";
        #endregion Private Members

        #region Fixture Setup
        [FixtureSetUp]
        public void FixtureSetUp() {
            List<string> groupTypes = new List<string>();
            groupTypes.Add(_groupTypeName);
            groupTypes.Add("Non Public Group Type");      
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
            base.SQL.Groups_GroupType_Delete(254, "Non Public Group Type");
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", _groupTypeName, new List<int> { 1, 2, 3, 4, 5, 9, 10, 11, 12 });
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", "Non Public Group Type", new List<int> { 4, 5, 10 }, false);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _groupName, null, "11/15/2009");
            base.SQL.Groups_Group_Create(254, "Non Public Group Type", "Bryan Mikaelian", "Non Public Group", null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, false, true, null, false);
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Leader", "Leader");
            base.SQL.Groups_SpanOfCare_Create(254, _socName, "Bryan Mikaelian", "SOC Owner", groupTypes);
        }

        #endregion Fixture Setup

        #region Fixture Teardown
        [FixtureTearDown]
        public void FixtureTearDown() {
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
            base.SQL.Groups_GroupType_Delete(254, "Non Public Group Type");
            base.SQL.Groups_SpanOfCare_Delete(254, _socName);
        }
        #endregion Fixture Teardown

		#region View Groups
		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Bryan Mikaelian")]
		[Description("Verifies viewing a group under a Span of Care takes you to the Dashboard for the group.")]
		public void SpanOfCare_View_Specific_Group() {
            // Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View a Span of Care's group
            test.infellowship.Groups_Group_View_Dashboard(_socName, _groupName);

			// Verify you are on the dashboard page for this group
			test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard);
			test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Roster);
			test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Gear_Menu);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.NavigationLinks.Link_Prospects);
            test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.DashboardPage.Link_ViewSettings);

			test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName));

			// Side links
			test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.DashboardPage.Link_ViewRoster);
			test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.DashboardPage.Link_ViewSettings);
			test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_InviteSomeone);
			test.Selenium.VerifyElementPresent(GroupsConstants.GroupManagement.RosterPage.Link_SendAnEmail);

			// Logout of infellowship
			test.infellowship.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies a Span of Care you do not own is not present in InFellowship.")]
		public void SpanOfCare_View_Non_Owned_Not_Present() {
            List<string> groupTypes = new List<string>();
            groupTypes.Add(_groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, "Unowned SOC");
            base.SQL.Groups_SpanOfCare_Create(254, "Unowned SOC", "Bryan Mikaelian", "Group Leader", groupTypes);

            // Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// Verifiy a Span of Care you do not own is not present in InFellowship
			test.Selenium.VerifyElementNotPresent("link=Unowned SOC");

            // Logout of infellowship
			test.infellowship.Logout();

            // Clean up
            base.SQL.Groups_SpanOfCare_Delete(254, "Unowned SOC");
		}

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies non public groups are not present in InFellowship when viewing a Span of Care.")]
        public void SpanOfCare_View_Non_Public_Not_Present() {

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            
            // View a Span of Care
            test.infellowship.Groups_SpanOfCare_View(_socName);

            // Verify the non public group is not present
            test.Selenium.VerifyElementNotPresent("link=Non Public Group");

            // Logout of infellowship
            test.infellowship.Logout();
        }

		#endregion View Groups

        #region Email All Leaders
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the email all leaders page loads for a Span of Care Owner.")]
        public void SpanOfCare_View_EmailAllLeaders() {
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the email all leaders page
            test.infellowship.Groups_SpanOfCare_View_EmailLeaders(_socName);

            // Verify the elements on the page are present
            Assert.IsTrue(test.Selenium.IsTextPresent("Compose and send email"));
            Assert.IsTrue(test.Selenium.IsElementPresent("subject"));
            Assert.IsTrue(test.Selenium.IsElementPresent("email_body"));

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the required fields for emailing all leaders.")]
        public void SpanOfCare_Email_All_Leaders_Required_Fields() {
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the email all leaders page
            test.infellowship.Groups_SpanOfCare_View_EmailLeaders(_socName);

            // Send an email without a subject
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify validation message
            Assert.IsTrue(test.Selenium.IsTextPresent("Subject is required and cannot exceed 200 characters."));

            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the subject of an email to all leaders cannot exceed 200 characters.")]
        public void SpanOfCare_Email_All_Leaders_Subject_Exceeds_200_Characters() {
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");


            // View the email all leaders page
            test.infellowship.Groups_SpanOfCare_View_EmailLeaders(_socName);

            // Send an email with a 200+ character subject
            test.Selenium.Type("subject", "This is a really really long subject This is a really really long subject This is a really really long subject This is a really really long subjectThis is a really really long subject This is a really really long subjectThis is a really really long subject This is a really really long subject.");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify validation message
            Assert.IsTrue(test.Selenium.IsTextPresent("Subject is required and cannot exceed 200 characters."));

            test.infellowship.Logout();
        }

        #endregion Email All Leaders

        #region Prospects
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that the prospects page loads when you click on the prospects link under a Span of Care.")]
        public void SpanOfCare_Prospects_View_Prospects_Page() {

            // Login to infellowship as a Span of Care owner 
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the prospect data of a Span of Care you own
            test.infellowship.Groups_SpanOfCare_View_ProspectsData(_socName);

            // Verify page loads
            Assert.IsTrue(test.Selenium.IsTextPresent("Prospect Analysis"));

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that the numerical prospect link is not a link if the group has no prospects.")]
        public void SpanOfCare_Prospects_Prospect_Link_Not_Linked_For_With_No_Prospects() {
            // Login to infellowship as a Span of Care owner 
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the prospect data of a Span of Care you own
            test.infellowship.Groups_SpanOfCare_View_ProspectsData(_socName);

            // Verify the Prospect link for a group is not linked in the group does not have any prospects.
            // Table does not have header text, so we need to use XPaths.
            int numberOfRows = (Int32)test.Selenium.GetXpathCount(TableIds.InFellowship_SpanOfCare_Prospects_GroupList);

            // Determine if the prospect link is linked.
            for (int rowPosition = 1; rowPosition <= numberOfRows; rowPosition++) {
                if (test.Selenium.GetText(TableIds.InFellowship_SpanOfCare_Prospects_GroupList + "/tbody/tr[" + rowPosition + "]/td[1]") == _groupName) {
                    if (test.Selenium.IsElementPresent(TableIds.InFellowship_SpanOfCare_Prospects_GroupList + "/tbody/tr[" + rowPosition + "]/td[2]/a")) {
                        test.Selenium.VerifyElementNotPresent(TableIds.InFellowship_SpanOfCare_Prospects_GroupList + "/tbody/tr[" + rowPosition + "]/td[2]/a");
                    }
                }
            }


            // Logout of infellowship
            test.infellowship.Logout();
        }

	    #endregion

        #region Attendance

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that the attendance tab is not present for a Span of Care if no attendance data exists")]
        public void SpanOfCare_Attendance_Not_Present_If_No_Data_Exists() {
            // Set up the data
            string groupTypeName = "SOC No Data";
            string groupName = "SOC No Data";
            var socName = "SOC No Attendance Data";
            string individual = "Bryan Mikaelian";
            List<string> groupTypes = new List<string>();
            groupTypes.Add(groupTypeName);

            // Clean up anything hanging around
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_SpanOfCare_Delete(254, socName);

            // Create the data
            base.SQL.Groups_GroupType_Create(254, individual, groupTypeName, new List<int>(10));
            base.SQL.Groups_Group_Create(254, groupTypeName, individual, groupName, null, "11/15/2009");
            base.SQL.Groups_SpanOfCare_Create(254, socName, individual, "SOC Owner", groupTypes);

            // Login in to InFellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View a the Span of Care
            test.infellowship.Groups_SpanOfCare_View(socName);

            // Verify the attendance tab is not present
            test.Selenium.VerifyElementNotPresent("link=Attendance");

            // Logout
            test.infellowship.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_SpanOfCare_Delete(254, socName);
        }

        #endregion Attendance

    }
}