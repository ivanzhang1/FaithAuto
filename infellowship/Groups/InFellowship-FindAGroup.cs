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
    public class infellowship_Groups_FindAGroup : FixtureBase {
        #region Private Members
        string _groupTypeName = "Group Finder Group Type";
        #endregion Private Members

        #region Fixture Setup and Teardown

        [FixtureSetUp]
        public void FixtureSetUp() {
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", _groupTypeName, new List<int>() { 1, 2, 3, 4, 5, 9, 10, 11, 12 });
        }

        [FixtureTearDown]
        public void FixtureTearDown() {
            //base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
        }

        #endregion Fixture Setup and Teardown

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you can view the Find a Group page.")]
        public void FindAGroup_View() {
            // Open InFellowship          
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Open("DC");

            // Find a group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LoginPage.Link_FindAGroupHere);

            // Verify all elements are present
            Assert.IsTrue(test.Selenium.IsElementPresent(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.TextField_ZipCode));
            Assert.IsTrue(test.Selenium.IsElementPresent(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Dropdown_Campus));
            Assert.IsTrue(test.Selenium.IsElementPresent(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Dropdown_Category));
            Assert.IsTrue(test.Selenium.IsElementPresent(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Checkbox_ChildcareProvided));
            Assert.IsTrue(test.Selenium.IsElementPresent(GeneralButtons.submitQuery));

            // Login to infellowship
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "DC");

            // Find a group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup);

            // Verify all elements are present
            Assert.IsTrue(test.Selenium.IsElementPresent(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.TextField_ZipCode));
            Assert.IsTrue(test.Selenium.IsElementPresent(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Dropdown_Campus));
            Assert.IsTrue(test.Selenium.IsElementPresent(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Dropdown_Category));
            Assert.IsTrue(test.Selenium.IsElementPresent(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Checkbox_ChildcareProvided));
            Assert.IsTrue(test.Selenium.IsElementPresent(GeneralButtons.submitQuery));

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you can view the show page for Find a Group.")]
        public void FindAGroup_View_Show_Page() {
            // Open InFellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Open("DC");

            // Find a group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LoginPage.Link_FindAGroupHere);

            // Search for all groups     
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify all elements are present
            Assert.IsTrue(test.Selenium.IsElementPresent(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.TextField_ZipCode));
            Assert.IsTrue(test.Selenium.IsElementPresent(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Dropdown_Campus));
            Assert.IsTrue(test.Selenium.IsElementPresent(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Dropdown_Category));
            Assert.IsTrue(test.Selenium.IsElementPresent(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Checkbox_ChildcareProvided));
            test.Selenium.VerifyElementPresent(GeneralButtons.submitQuery);


            // Login to infellowship
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "DC");

            // Find a group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup);

            // Search for all groups     
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify all elements are present 
            Assert.IsTrue(test.Selenium.IsElementPresent(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.TextField_ZipCode));
            Assert.IsTrue(test.Selenium.IsElementPresent(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Dropdown_Campus));
            Assert.IsTrue(test.Selenium.IsElementPresent(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Dropdown_Category));
            Assert.IsTrue(test.Selenium.IsElementPresent(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Dropdown_Weekday));
            Assert.IsTrue(test.Selenium.IsElementPresent(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Dropdown_StartTime));
            Assert.IsTrue(test.Selenium.IsElementPresent(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Checkbox_ChildcareProvided));
            test.Selenium.VerifyElementPresent(GeneralButtons.submitQuery);

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you are taken back to the Find a group page if you hit the link on the Gear Menu")]
        public void FindAGroup_View_Gear_Menu() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "DC");

            // Find a group via the gear inside a gear
            test.infellowship.Groups_FindAGroup_View_Gear("A Test Group");

            // Verify you are on the find a group page
            test.Selenium.VerifyElementPresent(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Dropdown_Category);
            test.Selenium.VerifyElementPresent(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Checkbox_ChildcareProvided);
            test.Selenium.VerifyElementPresent(GeneralButtons.submitQuery);

            // Logout of infellowship
            test.infellowship.Logout();
        }

        #region Search
        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you can search for all groups that have childcare provided.")]
        public void FindAGroup_Childcare() {
            // Initial Values
            string groupName = "HasChildCare Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, true);

            // InFellowship -> Find a Group             
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Open("QAEUNLX0C2");

            // Find a group
            test.infellowship.Groups_FindAGroup_Search(null, null, null, true);

            // Verify the results
            Assert.IsTrue(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));

            // Login to infellowship
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Find a group
            test.infellowship.Groups_FindAGroup_Search(null, null, null, true);

            // Verify the results
            Assert.IsTrue(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));

            // Logout of infellowship
            test.infellowship.Logout();

            // Clean up 
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you can search for all groups that meet on a certain day.")]
        public void FindAGroup_MeetingDay() {
            // Initial Values
            string groupName = "MeetingDay Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");

            // Log in to InFellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Create a schedule for the group
            test.infellowship.Groups_Group_Create_Schedule_Weekly(groupName, "11/15/2010", "9:00 PM", null, new GeneralEnumerations.WeeklyScheduleDays[] { GeneralEnumerations.WeeklyScheduleDays.Friday }, "week .", false);

            // Find a group
            test.infellowship.Groups_FindAGroup_Search(null, null, null, false, GeneralEnumerations.WeeklyScheduleDays.Friday, GeneralEnumerations.StartTimes.Any);

            // Verify the results
            Assert.IsTrue(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));

            // Find a group again but with different criteria
            test.infellowship.Groups_FindAGroup_Search(null, null, null, false, GeneralEnumerations.WeeklyScheduleDays.Tuesday, GeneralEnumerations.StartTimes.Any);

            // Verify the results
            Assert.IsFalse(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));

            // Logout of infellowship
            test.infellowship.Logout();

            // Find a group
            test.infellowship.Groups_FindAGroup_Search(null, null, null, false, GeneralEnumerations.WeeklyScheduleDays.Friday, GeneralEnumerations.StartTimes.Any);

            // Verify the results
            Assert.IsTrue(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));

            // Find a group again but with different criteria
            test.infellowship.Groups_FindAGroup_Search(null, null, null, false, GeneralEnumerations.WeeklyScheduleDays.Tuesday, GeneralEnumerations.StartTimes.Any);

            // Verify the results
            Assert.IsFalse(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));

            // Close
            test.Selenium.Close();

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you can search for all groups that meet at a certain start time.")]
        public void FindAGroup_StartTime() {
            // Initial Values
            string groupName = "Morning Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");

            // Log in to InFellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Create a schedule for the group
            test.infellowship.Groups_Group_Create_Schedule_Weekly(groupName, "11/15/2010", "8:00 AM", null, new GeneralEnumerations.WeeklyScheduleDays[] { GeneralEnumerations.WeeklyScheduleDays.Friday }, "week .", false);

            // Find a group
            test.infellowship.Groups_FindAGroup_Search(null, null, null, false, GeneralEnumerations.WeeklyScheduleDays.Any, GeneralEnumerations.StartTimes.Morning);

            // Verify the results
            Assert.IsTrue(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));

            // Find a group again but with different criteria
            test.infellowship.Groups_FindAGroup_Search(null, null, null, false, GeneralEnumerations.WeeklyScheduleDays.Any, GeneralEnumerations.StartTimes.Midday);

            // Verify the results
            Assert.IsFalse(test.Selenium.IsElementPresent(TableIds.InFellowship_FindAGroup_Results));

            // Logout of infellowship
            test.infellowship.Logout();

            // Find a group
            test.infellowship.Groups_FindAGroup_Search(null, null, null, false, GeneralEnumerations.WeeklyScheduleDays.Any, GeneralEnumerations.StartTimes.Morning);

            // Verify the results
            Assert.IsTrue(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));

            // Find a group again but with different criteria
            test.infellowship.Groups_FindAGroup_Search(null, null, null, false, GeneralEnumerations.WeeklyScheduleDays.Any, GeneralEnumerations.StartTimes.Midday);

            // Verify the results
            Assert.IsFalse(test.Selenium.IsElementPresent(TableIds.InFellowship_FindAGroup_Results));

            // Close
            test.Selenium.Close();

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for a group by campus.")]
        public void FindAGroup_Campus() {
            // Initial Values
            string groupName = "Campus Search Group";
            string campusName = "Search Campus";
            base.SQL.Admin_CampusDelete(254, campusName);
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Admin_CampusCreate(254, campusName);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, false, true, campusName); ;

            // InFellowship -> Find a Group             
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Open("QAEUNLX0C2");

            // Find a group
            test.infellowship.Groups_FindAGroup_Search(null, campusName, null, false);

            // Verify results
            Assert.IsTrue(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));

            // Login to infellowship
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Find a group
            test.infellowship.Groups_FindAGroup_Search(null, campusName, null, false);

            // Verify results
            Assert.IsTrue(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));

            // Logout of infellowship
            test.infellowship.Logout();

            // Clean up
            base.SQL.Admin_CampusDelete(254, campusName);
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        [Test, RepeatOnFailure, DoesNotRunOutsideOfStaging]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for a group by zipcode.")]
        public void FindAGroup_Zipcode() {
            // Initial Values
            string groupName = "Zipcode Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");

            // Log in to InFellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Create a location for the group
            test.infellowship.Groups_Group_Create_Location_Physical(groupName, "Test Location", null, "2812 Meadow Wood Drive", "Flower Mound", "Texas", "75022", false);

            // Find a group
            test.infellowship.Groups_FindAGroup_Search("75287", null, null, false);

            // Verify the List and Map view elements are present
            Assert.IsTrue(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_ListView));
            Assert.IsTrue(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_MapView));

            // Verify your group is returned
            Assert.IsTrue(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));

            // Logout
            test.infellowship.Logout();

            // Find a group
            test.infellowship.Groups_FindAGroup_Search("75287", null, null, false);

            // Verify your group is returned
            Assert.IsTrue(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));

            // Close
            test.Selenium.Close();

            // Clean up 
            base.SQL.Groups_Group_Delete(254, groupName);

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("FO-4987 Find a Group by zip get error")]
        public void FindAGroup_WithoutLogIn_Zipcode()
        {
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            // Initial Values
            string groupName = "Zipcode Group Mady";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2015");

            try
            {
                base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");

                // Log in to InFellowship
                test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

                // Create a location for the group
                test.infellowship.Groups_Group_Create_Location_Physical(groupName, "Test Location", null, "2812 Meadow Wood Drive", "Flower Mound", "Texas", "75201", false);

                // Logout
                test.infellowship.Logout();

                // Find a group
                test.infellowship.Groups_FindAGroup_Search("75201", null, null, false);

                // Verify the List and Map view elements are present
                Assert.IsTrue(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_ListView));
                Assert.IsTrue(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_MapView));

                // Verify your group is returned
                Assert.IsTrue(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));
            }
            finally
            {
                // Clean up 
                base.SQL.Groups_Group_Delete(254, groupName);

                // Close
                test.Selenium.Close();
            }

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("FO-7305 P3: In the Group Name Field, Apostrophes and Ampersands do not display correctly")]
        public void FindAGroup_GroupNameDisplay_SpecialCharacters()
        {
            // Initial Values
            string groupName = "~!@#$%^&*()_+-=[]:;/?><,.";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");

            // Log in to InFellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

                // Create a location for the group
                test.infellowship.Groups_Group_Create_Location_Physical(groupName, "Test Location", null, "2812 Meadow Wood Drive", "Flower Mound", "Texas", "75022", false);

                // Find a group
                test.infellowship.Groups_FindAGroup_Search("75287", null, null, false);

                // Verify the List and Map view elements are present
                Assert.IsTrue(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_ListView));
                Assert.IsTrue(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_MapView));

                // Verify your group is returned
                Assert.IsTrue(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));
                Assert.IsTrue(test.Selenium.IsElementPresent("link=" + groupName), "Group name is not display correctly in group detail page");
                test.Selenium.ClickAndWaitForPageToLoad("link=" + groupName);
                Assert.AreEqual(groupName, test.Selenium.GetText("//div[@id='page_header']").Trim(), "Group name is not display correctly in group detail page");
                // Logout
                test.infellowship.Logout();

                // Find a group
                test.infellowship.Groups_FindAGroup_Search("75287", null, null, false);

                // Verify your group is returned
                Assert.IsTrue(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));
                Assert.IsTrue(test.Selenium.IsElementPresent("link=" + groupName), "Group name is not display correctly in group detail page");
                test.Selenium.ClickAndWaitForPageToLoad("link=" + groupName);
                Assert.AreEqual(groupName, test.Selenium.GetText("//div[@id='page_header']").Trim(), "Group name is not display correctly in group detail page");
            }
            finally
            {
                // Clean up 
                base.SQL.Groups_Group_Delete(254, groupName);

                // Close
                test.Selenium.Close();
            }

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for a group by campus and if childcare is provided.")]
        public void FindAGroup_Campus_And_Childcare() {
            // Initial Values
            string groupName = "Campus Childcare Group";
            string campusName = "Childcare Campus";
            base.SQL.Admin_CampusDelete(254, campusName);
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Admin_CampusCreate(254, campusName);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, true, true, campusName); ;


            // InFellowship -> Find a Group             
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Open();

            // Find a group
            test.infellowship.Groups_FindAGroup_Search(null, campusName, null, true);

            // Verify results
            Assert.IsTrue(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));

            // Login to infellowship
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Find a group
            test.infellowship.Groups_FindAGroup_Search(null, campusName, null, true);

            // Verify results
            Assert.IsTrue(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));

            // Logout
            test.infellowship.Logout();

            // Clean up 
            base.SQL.Admin_CampusDelete(254, campusName);
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        [Test, RepeatOnFailure, DoesNotRunOutsideOfStaging]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for a group by zipcode and a campus.")]
        public void FindAGroup_Zipcode_And_Campus() {
            // Initial Values
            string groupName = "ZipCode Campus Group";
            string campusName = "ZipCode Campus";
            base.SQL.Admin_CampusDelete(254, campusName);
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Admin_CampusCreate(254, campusName);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, false, true, campusName); ;
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");

            // Log in to InFellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Create a location for the group
            test.infellowship.Groups_Group_Create_Location_Physical(groupName, "Test Location", null, "2812 Meadow Wood Drive", "Flower Mound", "Texas", "75022", false);

            // Find a group
            test.infellowship.Groups_FindAGroup_Search("75287", campusName, null, false);

            // Verify the List and Map view elements are present
            Assert.IsTrue(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_ListView));
            Assert.IsTrue(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_MapView));

            // Verify your group is returned
            Assert.IsTrue(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));

            // Logout
            test.infellowship.Logout();

            // Find a group
            test.infellowship.Groups_FindAGroup_Search("75287", campusName, null, false);

            // Verify the List and Map view elements are present
            Assert.IsTrue(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_ListView));
            Assert.IsTrue(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_MapView));

            // Verify your group is returned
            Assert.IsTrue(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));

            // Close
            test.Selenium.Close();

            // Clean up
            base.SQL.Admin_CampusDelete(254, campusName);
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        [Test, RepeatOnFailure, DoesNotRunOutsideOfStaging]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for a group by zipcode and if childcare is provided.")]
        public void FindAGroup_Zipcode_And_Childcare() {
            // Initial Values
            string groupName = "Zipcode Childcare Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, true, true, null);
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");

            // Log in to InFellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Create a location for the group
            test.infellowship.Groups_Group_Create_Location_Physical(groupName, "Test Location", null, "2812 Meadow Wood Drive", "Flower Mound", "Texas", "75022", false);

            // Find a group
            test.infellowship.Groups_FindAGroup_Search("75287", null, null, true);

            // Verify the List and Map view elements are present
            Assert.IsTrue(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_ListView));
            Assert.IsTrue(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_MapView));

            // Verify your group is returned
            Assert.IsTrue(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));

            // Logout
            test.infellowship.Logout();

            // Find a group
            test.infellowship.Groups_FindAGroup_Search("75287", null, null, true);

            // Verify the List and Map view elements are present
            Assert.IsTrue(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_ListView));
            Assert.IsTrue(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_MapView));

            // Verify your group is returned
            Assert.IsTrue(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));

            // Close
            test.Selenium.Close();

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies unsearchable groups are not present in Find a Group.")]
        public void FindAGroup_Unsearchable_Not_Present() {
            // Initial Values
            string groupName = "Unsearchable Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, false, false, null);

            // Open Infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Open("QAEUNLX0C2");

            // Find a group
            test.infellowship.Groups_FindAGroup_Search(null, null, null, false);

            // Verify the unsearchable group is not returned
            Assert.IsFalse(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));

            // Login to infellowship
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Find a group
            test.infellowship.Groups_FindAGroup_Search(null, null, null, false);

            // Verify the unsearchable group is not returned
            Assert.IsFalse(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));

            // Logout
            test.infellowship.Logout();

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        [Test, RepeatOnFailure, DoesNotRunOutsideOfStaging]
        [Author("Bryan Mikaelian")]
        [Description("Verifies online groups are not present in Find a Group with a zipcode is used.")]
        public void FindAGroup_Zipcode_Online_Not_Returned() {
            // Initial Values
            string groupName = "Online Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, true, true, null);
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Create online location
            test.infellowship.Groups_Group_Create_Location_Online(groupName, "Google", null, "https://www.google.com");

            // Find a group
            test.infellowship.Groups_FindAGroup_Search(null, null, null, false);

            // Verify the online group is returned
            Assert.IsTrue(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));

            // Logout
            test.infellowship.Logout();

            // Find a group
            test.infellowship.Groups_FindAGroup_Search("75022", null, null, false);

            // Verify the online group is not returned
            Assert.IsFalse(test.infellowship.Groups_FindAGroup_GroupExistsInResults(groupName));

            // Close
            test.Selenium.Close();

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        #endregion Search

        #region Groups at the Same Location
        [Test, MultipleAsserts, RepeatOnFailure, DoesNotRunOutsideOfStaging]
        [Author("Bryan Mikaelian")]
        [Description("Verifies if two groups meet at the same location, you can view the both those groups when you click on the pin on the map")]
        public void FindAGroup_Same_Location() {
            
            TestLog.WriteLine("SQL Setup");
            string groupName = "Location 1 Group";
            string groupName2 = "Location 2 Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Delete(254, groupName2);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, true, true, null);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName2, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, true, true, null);
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName2, "Group Leader", "Leader");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login .. ");
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Create locations for two groups but make the location the same spot
            TestLog.WriteLine("Create location: " + groupName);
            test.infellowship.Groups_Group_Create_Location_Physical(groupName, "Bryan's House", null, "2812 Meadow Wood Drive", "Flower Mound", "Texas", "75022", false);
            TestLog.WriteLine("Create location: " + groupName2);
            test.infellowship.Groups_Group_Create_Location_Physical(groupName2, "Bryan's House", null, "2812 Meadow Wood Drive", "Flower Mound", "Texas", "75022", false);

            // Find a group while logged in
            TestLog.WriteLine("Find A group Search ZIP:  75022"); 
            test.infellowship.Groups_FindAGroup_Search("75022", null, null, false);

            // We should have two groups that meet at the same location. Attempt to view them
            TestLog.WriteLine("Find Groups Same Location");
            test.infellowship.Groups_FindAGroup_View_GroupsAtSameLocation("2812 Meadow Wood Drive", "Flower Mound, TX 75022");

            // Verify all the data is there
            TestLog.WriteLine("Veridy Data for " + groupName + " and " + groupName2);
            //test.Selenium.VerifyTextPresent("Bryan's House");
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName2));

            // Logout of infellowship
            TestLog.WriteLine("Logout");
            test.infellowship.Logout();

            // Find a group while logged out
            TestLog.WriteLine("Find a group while logged out ZIP 75022");
            test.infellowship.Groups_FindAGroup_Search("75022", null, null, false);

            // We should have two groups that meet at the same location. Attempt to view them
            TestLog.WriteLine("Find Groups Same Location");
            test.infellowship.Groups_FindAGroup_View_GroupsAtSameLocation("2812 Meadow Wood Drive", "Flower Mound, TX 75022");

            // Verify all the data is there
            TestLog.WriteLine("Verify Data for " + groupName + " and " + groupName2);
            //test.Selenium.VerifyTextPresent("Bryan's House");
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName2));
            
            /*
            // Close
            test.Selenium.Close();
            */

            // Clean up
            //TestLog.WriteLine("Clean up by deleteing " + groupName + " and " + groupName2);
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Delete(254, groupName2);
            
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("FO-2755 Verifies if three groups meet at the same location and one of the group's location is private, the private group will not show up when you click the marker.")]
        public void FindAGroup_Same_Location_Private_Not_Present() {
            // Initial Values
            string groupName = "Location A Group test";
            string groupName2 = "Location B Group test";
            string groupName3 = "Really Private Location test";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Delete(254, groupName2);
            base.SQL.Groups_Group_Delete(254, groupName3);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, true, true, null);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName2, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, true, true, null);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName3, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, true, true, null);
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName2, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName3, "Group Leader", "Leader");

            System.Threading.Thread.Sleep(5000);

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Create locations for three groups but make the location the same spot. Make one location private
            test.infellowship.Groups_Group_Create_Location_Physical(groupName, "Flower Mound High School", null, "3411 Peters Colony", "Flower Mound", "Texas", "75022", false);
            test.infellowship.Groups_Group_Create_Location_Physical(groupName2, "Flower Mound High School", null, "3411 Peters Colony", "Flower Mound", "Texas", "75022", false);
            test.infellowship.Groups_Group_Create_Location_Physical(groupName3, "Flower Mound High School", null, "3411 Peters Colony", "Flower Mound", "Texas", "75022", true);

            System.Threading.Thread.Sleep(5000);

            // Find a group while logged in
            test.infellowship.Groups_FindAGroup_Search("75022", null, null, false);

            System.Threading.Thread.Sleep(5000);

            // We should have two groups that meet at the same location. Attempt to view them
            TestLog.WriteLine("We should have two groups that meet at the same location. Attempt to view them while logged in.");
            test.infellowship.Groups_FindAGroup_View_GroupsAtSameLocation("3411 Peters Colony", "Flower Mound, TX 75022");

            System.Threading.Thread.Sleep(5000);

            // Verify all the data is there
            //test.Selenium.VerifyTextPresent("Flower Mound High School");
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName2));

            // Verify the private group is not there
            #region OneTimeJavaScript
            string script = "function getNextTdText(){"
                               + "var objs = window.document.getElementsByTagName('td');"
                                  + "for(var i=0;i<objs.length;i++){"
                                    + "if(navigator.userAgent.indexOf('Firefox') != -1 ){"
                                      + "if(objs[i].textContent.indexOf('" + groupName3 + "')!=-1){ return objs[i+1].textContent; }"
                                    + "}"
                                    + "else{"
                                      + "if(objs[i].innerText.indexOf('" + groupName3 + "')!=-1){return objs[i+1].innerText; }"
                                    + "}"
                                  + "}"
                            + "}"
                            + "getNextTdText();";
            #endregion
            Assert.Contains(test.Selenium.GetEval(script), "Flower Mound, TX 75022");

            // Logout of infellowship
            TestLog.WriteLine("Logout");
            test.infellowship.Logout();

            // Find a group while logged out
            TestLog.WriteLine("Find a group while logged out");
            test.infellowship.Groups_FindAGroup_Search("75022", null, null, false);

            System.Threading.Thread.Sleep(5000);
            // We should have two groups that meet at the same location. Attempt to view them
            TestLog.WriteLine("We should have two groups that meet at the same location. Attempt to view them while logged out.");
            test.infellowship.Groups_FindAGroup_View_GroupsAtSameLocation("3411 Peters Colony", "Flower Mound, TX 75022");

            System.Threading.Thread.Sleep(5000);

            // Verify all the data is there
            test.Selenium.VerifyTextPresent("Flower Mound");
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName2));

            // Verify the private group is there but location is private
            Assert.Contains(test.Selenium.GetEval(script), "Flower Mound, TX 75022");

            // Close
            test.Selenium.Close();
        }

        [Test, RepeatOnFailure, MultipleAsserts, Timeout(600)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("FO-2755 Verifies if two groups meet at the same location and one of the group's location is private...")]
        public void FindAGroup_Same_Location_Private_ShouldNot_Show()
        {
            // Initial Values
            string groupName = "Public Location A Group";
            string groupName2 = "Private Location B Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Delete(254, groupName2);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, true, true, "SOC Campus", true);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName2, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, true, true, "SOC Campus", true);
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName2, "Group Leader", "Leader");

            System.Threading.Thread.Sleep(5000);
            
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Create locations for three groups but make the location the same spot. Make one location private
            test.infellowship.Groups_Group_Create_Location_Physical(groupName, "Flower Mound High School", null, "3411 Peters Colony", "Flower Mound", "Texas", "75022", false);
            test.infellowship.Groups_Group_Create_Location_Physical(groupName2, "Flower Mound High School", null, "3411 Peters Colony", "Flower Mound", "Texas", "75022", true);

            System.Threading.Thread.Sleep(5000);

            // Find a group while logged in
            test.infellowship.Groups_FindAGroup_Search(null, "SOC Campus", null, true);
            //test.Selenium.WaitForPageToLoad("10000");
            System.Threading.Thread.Sleep(5000);

            // Verify all the data is there
            test.Selenium.VerifyTextPresent("Flower Mound");
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName2));

            #region OneTimeJavaScript
            string script = "function getNextTdText(){"
                               +"var objs = window.document.getElementsByTagName('td');"
                                  +"for(var i=0;i<objs.length;i++){"
                                    +"if(navigator.userAgent.indexOf('Firefox') != -1 ){"
                                      +"if(objs[i].textContent.indexOf('"+groupName2+"')!=-1){ return objs[i+1].textContent; }"
                                    +"}"
                                    +"else{"
                                      + "if(objs[i].innerText.indexOf('" + groupName2 + "')!=-1){return objs[i+1].innerText; }"
                                    +"}"
                                  +"}"
                            +"}"
                            +"getNextTdText();";
            #endregion
            Assert.Contains(test.Selenium.GetEval(script), "Flower Mound, TX 75022");

            // Logout of infellowship
            TestLog.WriteLine("Logout");
            test.infellowship.Logout();

            // Find a group while logged out
            TestLog.WriteLine("Find a group while logged out");
            test.infellowship.Groups_FindAGroup_Search(null, "SOC Campus", null, true);

            System.Threading.Thread.Sleep(5000);

            // Verify all the data is there
            test.Selenium.VerifyTextPresent("Flower Mound");
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName2));

            Assert.Contains(test.Selenium.GetEval(script.ToString()), "Flower Mound, TX 75022");

            // Close
            test.Selenium.Close();
        }

        [Test, MultipleAsserts, RepeatOnFailure, DoesNotRunOutsideOfStaging]
        [Author("Bryan Mikaelian")]
        [Description("Verifies if two private groups meet at the same location, you can view the both those groups when you click on the pin on the map but no address information is present")]
        public void FindAGroup_Same_Location_Private_Groups() {
            // Initial Values
            string groupName = "Private A Group";
            string groupName2 = "Private B Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Delete(254, groupName2);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, true, true, null);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName2, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, true, true, null);
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName2, "Group Leader", "Leader");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            
            
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Create locations for two groups but make the location the same spot and private 
            test.infellowship.Groups_Group_Create_Location_Physical(groupName, "Starbucks", null, "2646 Flower Mound Road", "Flower Mound", "Texas", "75028", true);
            test.infellowship.Groups_Group_Create_Location_Physical(groupName2, "Starbucks", null, "2646 Flower Mound Road", "Flower Mound", "Texas", "75028", true);
            
            
            // Find a group while logged in
            test.infellowship.Groups_FindAGroup_Search("75022", null, null, false);

            // We should have two groups that meet at the same location. Attempt to view them
            test.infellowship.Groups_FindAGroup_View_GroupsAtSameLocation(null, "Flower Mound, TX 75028");
            /*
            #region OneTimeJavaScript
            string script = "function getNextTdText(){{"
                               + "var objs = window.document.getElementsByTagName('td');"
                                  + "for(var i=0;i<objs.length;i++){{"
                                    + "if(navigator.userAgent.indexOf('Firefox') != -1 ){{"
                                      + "if(objs[i].textContent.indexOf('{0}')!=-1){{ return objs[i+1].textContent; }}"
                                    + "}}"
                                    + "else{{"
                                      + "if(objs[i].innerText.indexOf('{1}')!=-1){{return objs[i+1].innerText; }}"
                                    + "}}"
                                  + "}}"
                            + "}}"
                            + "getNextTdText();";
            #endregion
            Assert.Contains(test.Selenium.GetEval(string.Format(script, groupName, groupName)), "null", "Private location should not be shown");
            Assert.Contains(test.Selenium.GetEval(string.Format(script, groupName2, groupName2)), "null", "Private location should not be shown");
            */
            // Verify all the data is there
            //test.Selenium.VerifyTextPresent("Starbucks");
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName2));

            // Verify the location is not present
            Assert.IsFalse(test.Selenium.IsTextPresent("2646 Flower Mound Road"), "Private location was present when viewing groups that meet at the same location!");

            // Logout of infellowship
            test.infellowship.Logout();

            // Find a group while logged out
            test.infellowship.Groups_FindAGroup_Search("75022", null, null, false);

            // We should have two groups that meet at the same location. Attempt to view them
            test.infellowship.Groups_FindAGroup_View_GroupsAtSameLocation(null, "Flower Mound, TX 75028");

            //Assert.Contains(test.Selenium.GetEval(string.Format(script, groupName, groupName)), "null", "Private location should not be shown");
            //Assert.Contains(test.Selenium.GetEval(string.Format(script, groupName2, groupName2)), "null", "Private location should not be shown");

            // Verify all the data is there
            //test.Selenium.VerifyTextPresent("Starbucks");
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName2));

            // Verify the location is not present
            Assert.IsFalse(test.Selenium.IsTextPresent("2646 Flower Mound Road"), "Private location was present when viewing groups that meet at the same location!");

            // Close
            test.Selenium.Close();

            // Clean up
            //base.SQL.Groups_Group_Delete(254, groupName);
            //base.SQL.Groups_Group_Delete(254, groupName2);
            
        }
        #endregion Groups at the Same Location

    }
}