using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Selenium;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using log4net;

namespace FTTests.infellowship.Groups {
    [TestFixture]
    public class infellowship_Groups_Attendance : FixtureBase {
        #region Private Members

        private string _groupTypeName = "Attendance Data";

        #endregion Private Members

        #region Fixture Setup
        [FixtureSetUp]
        public void FixtureSetUp() {
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", _groupTypeName, new List<int> { 4, 5, 10 });
        }
        #endregion Fixture Setup

        #region Fixture Teardown
        [FixtureTearDown]
        public void FixtureTearDown() {
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
        }
        #endregion Fixture Teardown

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the available controls for a group whose group type enables leaders to post attendance.")]
        public void Attendance_View_Group_With_Attendance() {
            // Set initial conditions
            string individualName = "Matthew Sneeden";            
            string groupTypeName = "Post Attendance Group Type";
            string groupName = "Post Attendance Group";

            // Clean up all the left over data that could be hanging around
            base.SQL.Groups_GroupType_Delete(15, groupTypeName);
            base.SQL.Groups_Group_Delete(15, groupName);
            base.SQL.Groups_Group_DeleteAttendanceSummary(15, groupName);

            // Create the group type
            //base.SQL.Groups_CreateGroupType(15, individualName, groupTypeName, new Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights[] { Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights.ChangeScheduleLocation, Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights.TakeAttendance });
            base.SQL.Groups_GroupType_Create(15, individualName, groupTypeName, new List<int> { 4, 5, 10 });

            // Spin up a group
            base.SQL.Groups_Group_Create(15, groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());

            // Add a leader
            base.SQL.Groups_Group_AddLeaderOrMember(15, groupName, individualName, "Leader");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

            // Verify the attendance tab is present
            test.infellowship.Groups_Group_View_Dashboard(groupName);
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Attendance"));

            // Logout of infellowship
            test.infellowship.Logout();
        }

        //[Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you cannot post attendance if the group has no attendance data.")]
        public void Attendance_View_Group_Without_Attendance() {
            // Set initial conditions
            string individualName = "Matthew Sneeden";
            string groupName = "No Post Attendance Group";

            // Clean up all the left over data that could be hanging around
            base.SQL.Groups_Group_Delete(15, groupName);
            base.SQL.Groups_Group_DeleteAttendanceSummary(15, groupName);

            // Create the group type
            //base.SQL.Groups_CreateGroupType(15, individualName, groupTypeName, new Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights[] { Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights.ChangeScheduleLocation, Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights.TakeAttendance });
            base.SQL.Groups_GroupType_Create(15, individualName, _groupTypeName, new List<int> { 4, 5, 10 });

            // Spin up a group
            base.SQL.Groups_Group_Create(15, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());

            // Add a leader
            base.SQL.Groups_Group_AddLeaderOrMember(15, groupName, individualName, "Leader");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

            // View the group
            test.infellowship.Groups_Group_View(groupName);

            // Verify the attendance tab is still present since you can post attendance by adding a new date
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Attendance"));

            // Logout of infellowship
            test.infellowship.Logout();
        }

        //[Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a group member cannot post attendance for a group if attendance is enabled for the group.")]
        public void Attendance_View_Member_Cannot_Post() {
            // Set initial conidiitions
            string individualName = "Bryan Mikaelian";
            string groupName = "Post Attendance Group";

            // Create a group
            base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());

            // Add a member
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Member", "Member");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09");

            // View the group
            test.infellowship.Groups_Group_View(groupName);

            // Verify the attendance tab is not present for the member
            Assert.IsFalse(test.Selenium.IsElementPresent("link=Attendance"));

            // Logout of infellowship
            test.infellowship.Logout();
        }

        #region New
        //[Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a you cannot post attendance twice for the same day.")]
        public void Attendance_Post_Did_Meet_Cannot_Post_Twice() {
            // Set initial conidiitions
            string individualName = "Group Leader";
            string groupName = "Post Attendance Group - Double Post";
            List<string> individualsInGroup = new List<string>();
            individualsInGroup.Add("Group Leader");
            // Clean up all the left over data that could be hanging around
            base.SQL.Groups_Group_Delete(254, groupName);
           
            base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false);

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09");

            // Enter attendance that did meet
            test.infellowship.Groups_Attendance_Enter_All(groupName, attendanceDate, true);

            // Select enter attendance
            test.Selenium.ClickAndWaitForPageToLoad("link=Enter attendance");

            // Verify that date is no longer in the drop down
            var dateOptions = test.Selenium.GetSelectOptions("//select[@id='post_attendance']");
            Assert.IsFalse(dateOptions.Contains(attendanceDate), "Attendance was posted for a date but that date was still present in the drop down for posting attendance!");

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("link=Cancel");

            // Logout
            test.infellowship.Logout();
        }

        //[Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a you can post attendance for a given date that a group did meet.")]
        public void Attendance_Post_Did_Meet() {
            // Set initial conidiitions
            string individualName = "Group Leader";
            string groupName = "Post Attendance Group - General Test";
            List<string> individualsInGroup = new List<string>();
            individualsInGroup.Add("Group Leader");
            // Clean up all the left over data that could be hanging around
           
            base.SQL.Groups_Group_Delete(254, groupName);
            
            base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false);

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09");

            // Enter attendance that did meet
            test.infellowship.Groups_Attendance_Enter_All(groupName, attendanceDate, true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a you can post attendance for a given date that a group did meet and has an end time.")]
        public void Attendance_Post_Did_Meet_With_End_Time() {
            // Set initial conidiitions
            string individualName = "Group Leader";
            string groupName = "Post Attendance Group - General Test End Time";
            List<string> individualsInGroup = new List<string>();
            individualsInGroup.Add("Group Leader");
            // Clean up all the left over data that could be hanging around
            
            base.SQL.Groups_Group_Delete(254, groupName);
            
            base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false, true);

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09");

            // Enter attendance that did meet
            test.infellowship.Groups_Attendance_Enter_All(groupName, attendanceDate, true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a you can post attendance for a given date that a group did not meet.")]
        public void Attendance_Post_Did_Not_Meet() {
            // Set initial conidiitions
            string individualName = "Group Leader";
            string groupName = "Post Attendance Group - Did Not Meet Test";
            List<string> individualsInGroup = new List<string>();
            individualsInGroup.Add("Group Leader");
            // Clean up all the left over data that could be hanging around
            
            base.SQL.Groups_Group_Delete(254, groupName);
            
            base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false, false);

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09");

            // Enter attendance that did not meet
            test.infellowship.Groups_Attendance_Enter_All(groupName, attendanceDate, false);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a you can post attendance for a given date that a group did not meet.")]
        public void Attendance_Post_Did_Not_Meet_With_End_Time() {
            // Set initial conidiitions
            string individualName = "Group Leader";
            string groupName = "Post Attendance Group - Did Not Meet Test End Time";
            List<string> individualsInGroup = new List<string>();
            individualsInGroup.Add("Group Leader");
            // Clean up all the left over data that could be hanging around
           
            base.SQL.Groups_Group_Delete(254, groupName);
           
            base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false, true);

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09");

            // Enter attendance that did not meet with an end time
            test.infellowship.Groups_Attendance_Enter_All(groupName, attendanceDate, false);

            // Logout
            test.infellowship.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a meeting date is required when posting attendance.")]
        public void Attendance_Post_Meeting_Date_Required() {
            // Set initial conidiitions
            string individualName = "Group Leader";
            string groupName = "Post Attendance Group - Validation";
            List<string> individualsInGroup = new List<string>();
            individualsInGroup.Add("Group Leader");
            // Clean up all the left over data that could be hanging around
            base.SQL.Groups_Group_Delete(111, groupName);
            
            base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false, false);

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09");

            // View the attendance page
            test.infellowship.Groups_Attendance_View(groupName);
            test.Selenium.ClickAndWaitForPageToLoad("link=Enter attendance");

            // Hit save
           // test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
            //SP - changed this since ID value is changed in UI for Attendance Save button....
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.Attendance_submitQuery);

            // Verify the validation message
            test.Selenium.VerifyTextPresent("You must select a group meeting to post attendance");

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("link=Cancel");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you must select individuals when posting attendance.")]
        public void Attendance_Post_Members_Required() {
            // Set initial conidiitions
            string individualName = "Group Leader";
            string groupName = "Post Attendance Group - Validation 3";
            List<string> individualsInGroup = new List<string>();
            individualsInGroup.Add("Group Leader");
            // Clean up all the left over data that could be hanging around
            base.SQL.Groups_Group_Delete(254, groupName);
            
            base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false, false);

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09");

            // View the attendance page
            test.infellowship.Groups_Attendance_View(groupName);
            test.Selenium.ClickAndWaitForPageToLoad("link=Enter attendance");

            // Select a date
            test.Selenium.SelectAndWaitForCondition("post_attendance", attendanceDate, test.JavaScript.IsElementPresent("attendance_roster"), "10000");

            // Hit save
           // test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
            //SP - changed this since ID value is changed in UI for Attendance Save button....
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.Attendance_submitQuery);

            // Verify the validation message
            test.Selenium.VerifyTextPresent("You must select at least one member");

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("link=Cancel");

            // Logout
            test.infellowship.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a reason for not meeting is required when posting attendance.")]
        public void Attendance_Post_Reason_For_Not_Meeting_Required() {
            // Set initial conidiitions
            string individualName = "Group Leader";
            string groupName = "Post Attendance Group - Validation 2";
            List<string> individualsInGroup = new List<string>();
            individualsInGroup.Add("Group Leader");
            // Clean up all the left over data that could be hanging around
            base.SQL.Groups_Group_Delete(254, groupName);
            
            base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false, false);

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09");

            // View the attendance page
            test.infellowship.Groups_Attendance_View(groupName);
            test.Selenium.ClickAndWaitForPageToLoad("link=Enter attendance");

            // Select a date
            test.Selenium.SelectAndWaitForCondition("post_attendance", attendanceDate, test.JavaScript.IsElementPresent("attendance_roster"), "10000");

            // Specify the group did not meet
            test.Selenium.Click("group_attendance_no");

            // Hit save
           // test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
            //SP - changed this since ID value is changed in UI for Attendance Save button....
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.Attendance_submitQuery);


            // Verify the validation message
            test.Selenium.VerifyTextPresent("Please provide a reason why your group did not meet");

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("link=Cancel");

            // Logout
            test.infellowship.Logout();

        }
        #endregion New

        #region Update

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a you can update an attendance date where the group met to did not meet.")]
        public void Attendance_Update_Meet_To_Did_Not_Meet() {
            // Set initial conidiitions
            string individualName = "Group Leader";
            string groupName = "Post Attendance Group - Update Meet";
            List<string> individualsInGroup = new List<string>();
            individualsInGroup.Add("Group Leader");
            // Clean up all the left over data that could be hanging around
           
            base.SQL.Groups_Group_Delete(254, groupName);
            
            base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false, false);

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09");

                // Enter attendance
                test.infellowship.Groups_Attendance_Enter_All(groupName, attendanceDate, true);

                // Update the attendance to specify that the group did not meet
                test.infellowship.Groups_Attendance_Update(groupName, attendanceDate, false);

                // Verify the message explaining why the group did not meet is present
                test.infellowship.Groups_Attendance_View_Date(groupName, attendanceDate);
                test.Selenium.VerifyTextPresent(string.Format("The group did not meet on {0}.", attendanceDate));

                // Cancel
                test.Selenium.ClickAndWaitForPageToLoad("link=Cancel");
            }
            finally
            {
                base.SQL.Groups_Group_Delete(254, groupName);

                // Logout
                test.infellowship.Logout();
            }

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a you can update an attendance date where the group did not meet to met.")]
        public void Attendance_Update_Did_Not_Meet_To_Meet() {
            // Set initial conidiitions
            string individualName = "Group Leader";
            string groupName = "Post Attendance Group - Update Did Not Meet";
            List<string> individualsInGroup = new List<string>();
            individualsInGroup.Add("Group Leader");
            // Clean up all the left over data that could be hanging around
            base.SQL.Groups_Group_Delete(254, groupName);
            
            base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false, false);

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09");

                // Enter attendance for that date
                test.infellowship.Groups_Attendance_Enter_All(groupName, attendanceDate, false);

                // Update the attendance to specify that the group did meet
                test.infellowship.Groups_Attendance_Update(groupName, attendanceDate, true);

                // Select enter attendance
                test.Selenium.ClickAndWaitForPageToLoad("link=Enter attendance");

                // Verify that date is no longer in the drop down
                var dateOptions = test.Selenium.GetSelectOptions("//select[@id='post_attendance']");
                Assert.IsFalse(dateOptions.Contains(attendanceDate));

                // Cancel
                test.Selenium.ClickAndWaitForPageToLoad("link=Cancel");
            }
            finally
            {
                base.SQL.Groups_Group_Delete(254, groupName);

                // Logout
                test.infellowship.Logout();
            }

        }

        #endregion Update

        #region New Member

        [Test, RepeatOnFailure]
        //[MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that if you are taking attendance and select a date to post for, members added on that day show up.")]
        public void Attendance_Post_New_Member_Present_Joined_On_Attendance_Date() {
            // Set initial conditions
            string groupName = "New Attendance - New Member";
            List<string> individualsInGroup = new List<string>();
            individualsInGroup.Add("Group Leader");
            // Clean up all the left over data that could be hanging around
            base.SQL.Groups_Group_Delete(254, groupName);
            
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false, false);

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the attendance tab
            test.infellowship.Groups_Attendance_View(groupName);

            // Enter attendance
            test.Selenium.ClickAndWaitForPageToLoad("link=Enter attendance");

            // Pick the only date to post attendance for
            test.Selenium.SelectAndWaitForCondition("//select[@id='post_attendance']", "index=1", test.JavaScript.IsElementPresentSelector("table.grid"), "20000");

            // Verify the member we are about to add is not present
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.InFellowship_Group_Attendance, "Coly Gutekunst", "Members"), "New member was already present!");
                      
            // Hit back//            
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@class,'hidden-xs') and contains(@class,'btn')]");
   
            // Add a new member
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Coly Gutekunst", "Member");

            // Enter attendance
            test.Selenium.ClickAndWaitForPageToLoad("link=Enter attendance");

            // Pick the only date to post attendance for
            test.Selenium.SelectAndWaitForCondition("//select[@id='post_attendance']", "index=1", test.JavaScript.IsElementPresentSelector("table.grid"), "10000");

            // Verify the member we just added is present since their created date falls on the same day of the attendance date
            string memberName = test.Selenium.GetText(string.Format("{0}/tbody/tr[*]/td[3]/a", TableIds.InFellowship_Group_Attendance));
            Assert.AreEqual("Coly Gutekunst", memberName);
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.InFellowship_Group_Attendance, "Coly Gutekunst", "Members"), "New member was not present!");

            // Hit back
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@class,'hidden-xs') and contains(@class,'btn')]");
            
            // Update the member so their join date is 5 days ago
            base.SQL.Groups_Group_UpdateMemberJoinDate(254, "Coly Gutekunst", groupName, "-", 5);

            // Enter attendance
            test.Selenium.ClickAndWaitForPageToLoad("link=Enter attendance");

            // Pick the only date to post attendance for
            test.Selenium.SelectAndWaitForCondition("//select[@id='post_attendance']", "index=1", test.JavaScript.IsElementPresentSelector("table.grid"), "10000");

            // Verify the member is present
            //string memberName = test.Selenium.GetText(string.Format("{0}/tbody/tr[*]/td[3]/a", TableIds.InFellowship_Group_Attendance));
            Assert.AreEqual("Coly Gutekunst", memberName);
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.InFellowship_Group_Attendance, "Coly Gutekunst", "Members"), "New member added in the past was not present.");

            // Hit back
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@class,'hidden-xs') and contains(@class,'btn')]");

            // Update the member so their join date is 5 days in the future
            //base.SQL.Groups_Group_UpdateMemberJoinDate(254, "Coly Gutekunst", groupName, "+", 5);

            // Enter attendance
            //test.Selenium.ClickAndWaitForPageToLoad("link=Enter attendance");

            // Pick the only date to post attendance for
            //test.Selenium.SelectAndWaitForCondition("//select[@id='post_attendance']", "index=1", test.JavaScript.IsElementPresentSelector("table.grid"), "10000");

            // Verify the member is not present
            //Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.InFellowship_Group_Attendance, "Coly Gutekunst", "Members"), "New member added after the attendance date was present.");

            // Hit back
            //test.Selenium.ClickAndWaitForPageToLoad("link=Back");

            // Logout
            test.infellowship.Logout();           

        }

        [Test, RepeatOnFailure]
        //[MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that if you update an attendance record, members added on that day show up.")]
        public void Attendance_Update_New_Member_Present_Joined_On_Attendance_Date() {
            // Set initial conditions
            string groupName = "Update Attendance - New Member";

            // Clean up all the left over data that could be hanging around
            base.SQL.Groups_Group_Delete(254, groupName);
            try
            {
                List<string> individualsInGroup = new List<string>();
                individualsInGroup.Add("Group Leader");

                base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
                base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
                var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false, false);

                // Login to infellowship
                TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

                // Post Attendance
                test.infellowship.Groups_Attendance_Enter_All(groupName, attendanceDate, true);

                // Add a new member
                base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Coly Gutekunst", "Member");
                System.Threading.Thread.Sleep(12000);
                // View the attendance date
                test.infellowship.Groups_Attendance_View_Date(groupName, attendanceDate);

                // Wait for the member we added is present in the table that displays members without attendance
                test.Selenium.WaitForCondition("selenium.isVisible(\"xpath=//table[@id='members_missing_attendance']\");", "30");
                // test.Selenium.VerifyElementPresent(TableIds.InFellowship_Groups_Members_Without_Attendance);
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.InFellowship_Groups_Members_Without_Attendance, "Coly Gutekunst", "Members Missing Attendance"), "New member was not present!");

                // Update the member so their join date is 4 days ago
                base.SQL.Groups_Group_UpdateMemberJoinDate(254, "Coly Gutekunst", groupName, "-", 5);
                System.Threading.Thread.Sleep(12000);

                // View the attendance date
                test.infellowship.Groups_Attendance_View_Date(groupName, attendanceDate);

                // Verify the member we added is not present in the table that displays members without attendance
                test.Selenium.WaitForCondition("selenium.isVisible(\"xpath=//table[@id='members_missing_attendance']\");", "30");
                // test.Selenium.VerifyElementPresent(TableIds.InFellowship_Groups_Members_Without_Attendance);
                // Hit back
                test.Selenium.ClickAndWaitForPageToLoad("link=Back");

                // Update the member so their join date is 5 days in the future
                //base.SQL.Groups_Group_UpdateMemberJoinDate(254, "Coly Gutekunst", groupName, "+", 5);

                // View the attendance date
                //test.infellowship.Groups_Attendance_View_Date(groupName, attendanceDate);

                // Verify the member we added is not present in the table that displays members without attendance
                //test.Selenium.VerifyElementNotPresent(TableIds.InFellowship_Groups_Members_Without_Attendance);

                // Hit back
                //test.Selenium.ClickAndWaitForPageToLoad("link=Back");

                // Update the member so their join date is today
                base.SQL.Groups_Group_UpdateMemberJoinDate(254, "Coly Gutekunst", groupName, "+", 0);

                // Update
                test.infellowship.Groups_Attendance_Update(groupName, attendanceDate, true);

                // View the attendance date
                test.infellowship.Groups_Attendance_View_Date(groupName, attendanceDate);

                // Verify the newely added member is there
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.InFellowship_Groups_Members_With_Attendance, "Coly Gutekunst", "Members"), "New member was not present!");

                // Hit back
                test.Selenium.ClickAndWaitForPageToLoad("link=Back");

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                base.SQL.Groups_Group_Delete(254, groupName);
            }
            

        }

        #endregion New Member

        #region Enter a date

        [Test, RepeatOnFailure]
        //[MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Creates a new attendance date instead of selecting an existing one.")]
        public void Attendance_Post_New_Attendance_Date() {
            // Set initial conditions
            string groupName = "Attendance Data - New Date";

            // Create the group
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");

            // Add someone as a leader
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09");

            // Create a new attendance date
            var attendanceDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy");
            test.infellowship.Groups_Attendance_Post_Enter_All_New_Date(groupName, attendanceDate, "1:00 AM");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        //[MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Creates a new attendance date instead of selecting an existing one then updates it.")]
        public void Attendance_Update_New_Attendance_Date() {
            // Set initial conditions
            string groupName = "Attendance Data - New Date Update";

            // Clean up all the left over data that could be hanging around
            base.SQL.Groups_Group_Delete(254, groupName);
            try
            {

           
                // Create the group
                base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");

                // Add someone as a leader
                base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");

                // Login to infellowship
                TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09");

                // Create a new attendance date
                var attendanceDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy");
                var attendanceDateFormal = string.Format("{0} {1}, {2} at 1:00 AM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MMMM"), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("dd"), DateTime.Now.Year);
                test.infellowship.Groups_Attendance_Post_Enter_All_New_Date(groupName, attendanceDate, "1:00 AM");

                // Update the date
                test.infellowship.Groups_Attendance_Update(groupName, attendanceDateFormal, false);

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                base.SQL.Groups_Group_Delete(254, groupName);
            }
        }


        //[Test, RepeatOnFailure]
        //[MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the required fields for adding a new attendance date.")]
        public void Attendance_Update_New_Attendance_Date_Required_Fields() {
            // Set initial conditions
            string groupName = "Attendance Data - New Date Validation";

            // Create the group
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");

            // Add someone as a leader
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Bryan Mikaelian", "Leader");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // View the attendance tab
            test.infellowship.Groups_Group_View_Dashboard(groupName);
            test.infellowship.Groups_Attendance_View(groupName);

            // Enter attendance
            test.Selenium.ClickAndWaitForPageToLoad("link=Enter attendance");

            // Create a new attendance date but don't specify anything
            // Select the enter new date option
            test.Selenium.SelectAndWaitForCondition("//select[@id='post_attendance']", "Add your new event...", test.JavaScript.IsElementPresent("pick_event_date"), "10000");

            // Submit
            //test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
            //SP - changed this since ID value is changed in UI for Attendance Save button....
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.Attendance_submitQuery);

            // Verify the validation
            test.Selenium.VerifyTextPresent("The date must be between today and the last 18 months.");
            // test.Selenium.VerifyTextPresent("Please select an attendance date.");
            test.Selenium.VerifyTextPresent("Please select an attendance time.");

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("link=Cancel");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        //[MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the attendance tab is always present for a group, even if they do not have a schedule but the add attendance date module is enabled.")]
        public void Attendance_Post_Tab_Present_Add_Attendance_Date_Module_Enabled() {
            // Set initial conditions
            string groupName = "Attendance Data - No Schedule";

            // Create the group
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");

            // Add someone as a leader
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Bryan Mikaelian", "Leader");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // View the attendance tab
            test.infellowship.Groups_Group_View_Dashboard(groupName);

            // Verify the attendance tab is there
            test.Selenium.VerifyElementPresent("link=Attendance");

            // Logout
            test.infellowship.Logout();
        }

        #endregion Enter a date
    }

    [TestFixture]
    public class infellowship_Groups_Attendance_WebDriver : FixtureBaseWebDriver
    {
        #region Private Members

        private string _groupTypeName = "Attendance Data";

        #endregion Private Members

        #region Fixture Setup
        [FixtureSetUp]
        public void FixtureSetUp()
        {
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", _groupTypeName, new List<int> { 4, 5, 10 });
        }
        #endregion Fixture Setup

        #region Fixture Teardown
        [FixtureTearDown]
        public void FixtureTearDown()
        {
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
        }
        #endregion Fixture Teardown

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Verifies the available controls for a group whose group type enables leaders to post attendance.")]
        public void Attendance_View_Group_With_Attendance_WebDriver()
        {
            // Set initial conditions
            string individualName = "Matthew Sneeden";
            string groupTypeName = "Post Attendance Group Type";
            string groupName = "Post Attendance Group";

            // Clean up all the left over data that could be hanging around
            base.SQL.Groups_GroupType_Delete(15, groupTypeName);
            base.SQL.Groups_Group_Delete(15, groupName);
            base.SQL.Groups_Group_DeleteAttendanceSummary(15, groupName);

            // Create the group type
            //base.SQL.Groups_CreateGroupType(15, individualName, groupTypeName, new Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights[] { Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights.ChangeScheduleLocation, Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights.TakeAttendance });
            base.SQL.Groups_GroupType_Create(15, individualName, groupTypeName, new List<int> { 4, 5, 10 });

            // Spin up a group
            base.SQL.Groups_Group_Create(15, groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());

            // Add a leader
            base.SQL.Groups_Group_AddLeaderOrMember(15, groupName, individualName, "Leader");

            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            test.Infellowship.LoginWebDriver("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

            // Verify the attendance tab is present
            test.Infellowship.Groups_Group_View_WebDriver(groupName);
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Attendance")));

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you cannot post attendance if the group has no attendance data.")]
        public void Attendance_View_Group_Without_Attendance_WebDriver()
        {
            // Set initial conditions
            string individualName = "Matthew Sneeden";
            string groupName = "No Post Attendance Group";

            // Clean up all the left over data that could be hanging around
            base.SQL.Groups_Group_Delete(15, groupName);
            base.SQL.Groups_Group_DeleteAttendanceSummary(15, groupName);

            // Create the group type
            //base.SQL.Groups_CreateGroupType(15, individualName, groupTypeName, new Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights[] { Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights.ChangeScheduleLocation, Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights.TakeAttendance });
            base.SQL.Groups_GroupType_Create(15, individualName, _groupTypeName, new List<int> { 4, 5, 10 });

            // Spin up a group
            base.SQL.Groups_Group_Create(15, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());

            // Add a leader
            base.SQL.Groups_Group_AddLeaderOrMember(15, groupName, individualName, "Leader");

            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

            // View the group
            test.Infellowship.Groups_Group_View_WebDriver(groupName);

            // Verify the attendance tab is still present since you can post attendance by adding a new date
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Attendance")));

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a group member cannot post attendance for a group if attendance is enabled for the group.")]
        public void Attendance_View_Member_Cannot_Post_WebDriver()
        {
            // Set initial conidiitions
            string individualName = "Bryan Mikaelian";
            string groupName = "Post Attendance Group";

            // Create a group
            base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());

            // Add a member
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Member", "Member");

            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the group
            test.Infellowship.Groups_Group_View_WebDriver(groupName);

            // Verify the attendance tab is not present for the member
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Attendance")));

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        #region New
        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a you cannot post attendance twice for the same day.")]
        public void Attendance_Post_Did_Meet_Cannot_Post_Twice_WebDriver()
        {
            // Set initial conidiitions
            string individualName = "Group Leader";
            string groupName = "Post Attendance Group - Double Post";
            List<string> individualsInGroup = new List<string>();
            individualsInGroup.Add("Group Leader");
            // Clean up all the left over data that could be hanging around
            base.SQL.Groups_Group_Delete(254, groupName);
            
            base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false);

            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Enter attendance that did meet
            test.Infellowship.Groups_Attendance_Enter_All_WebDriver(groupName, attendanceDate, true);

            // Select enter attendance
            test.Driver.FindElementByLinkText("Enter attendance").Click();

            // Verify that date is no longer in the drop down
            Assert.IsFalse(test.Driver.FindElementByTagName("html").Text.Contains(attendanceDate));

            // Cancel
            var btnCancel = test.Driver.FindElementsByXPath(".//a[contains(@class,'btn') and text()='Cancel']").FirstOrDefault(x => x.Displayed == true);
            btnCancel.Click();

            // Logout
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a you can post attendance for a given date that a group did meet.")]
        public void Attendance_Post_Did_Meet_WebDriver()
        {
            // Set initial conidiitions
            string individualName = "Group Leader";
            string groupName = "Post Attendance Group - General Test";
            List<string> individualsInGroup = new List<string>();
            individualsInGroup.Add("Group Leader");
            // Clean up all the left over data that could be hanging around
            base.SQL.Groups_Group_Delete(254, groupName);
            
            base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false);

            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Enter attendance that did meet
            test.Infellowship.Groups_Attendance_Enter_All_WebDriver(groupName, attendanceDate, true);

            // Logout
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Michelle Zheng")]
        [Description("Verifies that you cannot add an event in InFellowship group attendance if a member's join date is modified.")]
        public void Attendance_View_Group_Cannot_Add_Event()
        {
            string individualName = "michelle1 zheng";
            string groupName = "michelle group";

            #region initial data
            // Clean up all the left over data that could be hanging around
            base.SQL.Groups_Group_Delete(15, groupName);
            base.SQL.Groups_Group_DeleteAttendanceSummary(15, groupName);
            base.SQL.Groups_GroupType_Delete(15, _groupTypeName);

            // Create the group type
            base.SQL.Groups_GroupType_Create(15, individualName, _groupTypeName, new List<int> { 1, 2, 9, 4, 5, 10, 12 });

            // Spin up a group
            base.SQL.Groups_Group_Create(15, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(-1).ToShortDateString());

            // Add a leader
            base.SQL.Groups_Group_AddLeaderOrMember(15, groupName, individualName, "Leader");
            // Add a member
            base.SQL.Groups_Group_AddLeaderOrMember(15, groupName, "jonny liang", "Member");
            // Update the member so their join date is 30 days ago
            base.SQL.Groups_Group_UpdateMemberJoinDate(15, "jonny liang", groupName, "-", 31);
            //Add a member again.
            base.SQL.Groups_Group_AddLeaderOrMember(15, groupName, "jonny liang", "Member");


            #endregion

            try
            {
                // Login to infellowship
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

                test.Infellowship.LoginWebDriver("zhm_8343@163.com", "111111", "dc");
                // Click on Your Groups
                test.Driver.FindElementByLinkText(GeneralInFellowshipConstants.LandingPage.Link_YourGroups_WebDriver).Click();
                // Click on Group name
                test.Driver.FindElementByXPath(".//*[@id='main-content']/div/ul/li/div/a[text()='michelle group']").Click();
                // Click on Attendance tab
                test.Driver.FindElement(By.LinkText("Attendance")).Click();
                // Click enter attedance link
                test.Driver.FindElementById("lnk_enter_attendance").Click();
                //Click drop down list
                SelectElement select = new SelectElement(test.Driver.FindElementById("post_attendance"));
                select.SelectByValue("new_event_option");

                bool isHasGivenName = test.GeneralMethods.IsElementPresentWebDriver(By.XPath(".//*[@id='attendance_roster']/table/tbody/tr/td/a/span[text()='Jonny']"));
                bool isHasFamilyName = test.GeneralMethods.IsElementPresentWebDriver(By.XPath(".//*[@id='attendance_roster']/table/tbody/tr/td/a/span[text()='liang']"));
                Assert.IsTrue(isHasGivenName && isHasFamilyName, "Error fMember isn't searched out.");

                // Logout .
                test.Infellowship.LogoutWebDriver();
            }
            finally
            {
                #region clear data
                base.SQL.Groups_Group_Delete(15, groupName);
                base.SQL.Groups_Group_DeleteAttendanceSummary(15, groupName);
                base.SQL.Groups_GroupType_Delete(15, _groupTypeName);
                #endregion
            }



        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies that you can post attendance successfully when infellowship account password contains space.")]
        public void Attendance_Post_Space_In_Password_WebDriver()
        {
            // Set initial conidiitions
            string individualName = "SpaceIn Password";
            string groupName = "Post Attendance Group - Space Password Test";
            List<string> individualsInGroup = new List<string>();
            individualsInGroup.Add(individualName);
            // Clean up all the left over data that could be hanging around
            base.SQL.Groups_Group_Delete(254, groupName);
            
            base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, individualName, "Leader");
            var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false);

            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("group.spaceinpassword@gmail.com", "BM.Admin 09", "QAEUNLX0C2");

            // Enter attendance that did meet
            test.Infellowship.Groups_Attendance_Enter_All_WebDriver(groupName, attendanceDate, true);

            // Logout
            test.Infellowship.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Verifies that a you can post attendance for a given date that a group did meet and has an end time.")]
        //public void Attendance_Post_Did_Meet_With_End_Time()
        //{
        //    // Set initial conidiitions
        //    string individualName = "Group Leader";
        //    string groupName = "Post Attendance Group - General Test End Time";
        //    List<string> individualsInGroup = new List<string>();
        //    individualsInGroup.Add("Group Leader");
        //    base.SQL.Groups_Group_Delete(254, groupName);
        //    base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
        //    base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
        //    var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false, true);

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09");

        //    // Enter attendance that did meet
        //    test.infellowship.Groups_Attendance_Enter_All(groupName, attendanceDate, true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        //[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Verifies that a you can post attendance for a given date that a group did not meet.")]
        //public void Attendance_Post_Did_Not_Meet()
        //{
        //    // Set initial conidiitions
        //    string individualName = "Group Leader";
        //    string groupName = "Post Attendance Group - Did Not Meet Test";
        //    List<string> individualsInGroup = new List<string>();
        //    individualsInGroup.Add("Group Leader");
        //    base.SQL.Groups_Group_Delete(254, groupName);
        //    base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
        //    base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
        //    var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false, false);

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09");

        //    // Enter attendance that did not meet
        //    test.infellowship.Groups_Attendance_Enter_All(groupName, attendanceDate, false);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        //[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Verifies that a you can post attendance for a given date that a group did not meet.")]
        //public void Attendance_Post_Did_Not_Meet_With_End_Time()
        //{
        //    // Set initial conidiitions
        //    string individualName = "Group Leader";
        //    string groupName = "Post Attendance Group - Did Not Meet Test End Time";
        //    List<string> individualsInGroup = new List<string>();
        //    individualsInGroup.Add("Group Leader");
        //    base.SQL.Groups_Group_Delete(254, groupName);
        //    base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
        //    base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
        //    var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false, true);

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09");

        //    // Enter attendance that did not meet with an end time
        //    test.infellowship.Groups_Attendance_Enter_All(groupName, attendanceDate, false);

        //    // Logout
        //    test.infellowship.Logout();

        //}

        //[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Verifies that a meeting date is required when posting attendance.")]
        //public void Attendance_Post_Meeting_Date_Required()
        //{
        //    // Set initial conidiitions
        //    string individualName = "Group Leader";
        //    string groupName = "Post Attendance Group - Validation";
        //    List<string> individualsInGroup = new List<string>();
        //    individualsInGroup.Add("Group Leader");
        //    base.SQL.Groups_Group_Delete(254, groupName);
        //    base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
        //    base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
        //    base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false, false);

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09");

        //    // View the attendance page
        //    test.infellowship.Groups_Attendance_View(groupName);
        //    test.Selenium.ClickAndWaitForPageToLoad("link=Enter attendance");

        //    // Hit save
        //    // test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
        //    //SP - changed this since ID value is changed in UI for Attendance Save button....
        //    test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.Attendance_submitQuery);

        //    // Verify the validation message
        //    test.Selenium.VerifyTextPresent("You must select a group meeting to post attendance");

        //    // Cancel
        //    test.Selenium.ClickAndWaitForPageToLoad("link=Cancel");

        //    // Logout
        //    test.infellowship.Logout();
        //}

        //[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Verifies that you must select individuals when posting attendance.")]
        //public void Attendance_Post_Members_Required()
        //{
        //    // Set initial conidiitions
        //    string individualName = "Group Leader";
        //    string groupName = "Post Attendance Group - Validation 3";
        //    List<string> individualsInGroup = new List<string>();
        //    individualsInGroup.Add("Group Leader");
        //    base.SQL.Groups_Group_Delete(254, groupName);
        //    base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
        //    base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
        //    var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false, false);

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09");

        //    // View the attendance page
        //    test.infellowship.Groups_Attendance_View(groupName);
        //    test.Selenium.ClickAndWaitForPageToLoad("link=Enter attendance");

        //    // Select a date
        //    test.Selenium.SelectAndWaitForCondition("post_attendance", attendanceDate, test.JavaScript.IsElementPresent("attendance_roster"), "10000");

        //    // Hit save
        //    // test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
        //    //SP - changed this since ID value is changed in UI for Attendance Save button....
        //    test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.Attendance_submitQuery);

        //    // Verify the validation message
        //    test.Selenium.VerifyTextPresent("You must select at least one member");

        //    // Cancel
        //    test.Selenium.ClickAndWaitForPageToLoad("link=Cancel");

        //    // Logout
        //    test.infellowship.Logout();

        //}

        //[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Verifies that a reason for not meeting is required when posting attendance.")]
        //public void Attendance_Post_Reason_For_Not_Meeting_Required()
        //{
        //    // Set initial conidiitions
        //    string individualName = "Group Leader";
        //    string groupName = "Post Attendance Group - Validation 2";
        //    List<string> individualsInGroup = new List<string>();
        //    individualsInGroup.Add("Group Leader");
        //    base.SQL.Groups_Group_Delete(254, groupName);
        //    base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
        //    base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
        //    var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false, false);

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09");

        //    // View the attendance page
        //    test.infellowship.Groups_Attendance_View(groupName);
        //    test.Selenium.ClickAndWaitForPageToLoad("link=Enter attendance");

        //    // Select a date
        //    test.Selenium.SelectAndWaitForCondition("post_attendance", attendanceDate, test.JavaScript.IsElementPresent("attendance_roster"), "10000");

        //    // Specify the group did not meet
        //    test.Selenium.Click("group_attendance_no");

        //    // Hit save
        //    // test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
        //    //SP - changed this since ID value is changed in UI for Attendance Save button....
        //    test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.Attendance_submitQuery);


        //    // Verify the validation message
        //    test.Selenium.VerifyTextPresent("Please provide a reason why your group did not meet");

        //    // Cancel
        //    test.Selenium.ClickAndWaitForPageToLoad("link=Cancel");

        //    // Logout
        //    test.infellowship.Logout();

        //    // Clean up
        //    base.SQL.Groups_Group_Delete(254, groupName);

        //}

        #endregion New

        #region Responsive
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Verifies the available controls for a group whose group type enables leaders to post attendance.")]
        public void Attendance_View_Group_With_Attendance_WebDriver_RESPONSIVE()
        {
            // Set initial conditions
            string individualName = "Matthew Sneeden";
            string groupTypeName = "Post Attendance Group Type";
            string groupName = "Post Attendance Group Resp";

            // Clean up all the left over data that could be hanging around
            base.SQL.Groups_GroupType_Delete(15, groupTypeName);
            base.SQL.Groups_Group_Delete(15, groupName);
            base.SQL.Groups_Group_DeleteAttendanceSummary(15, groupName);

            // Create the group type
            //base.SQL.Groups_CreateGroupType(15, individualName, groupTypeName, new Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights[] { Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights.ChangeScheduleLocation, Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights.TakeAttendance });
            base.SQL.Groups_GroupType_Create(15, individualName, groupTypeName, new List<int> { 4, 5, 10 });

            // Spin up a group
            base.SQL.Groups_Group_Create(15, groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());

            // Add a leader
            base.SQL.Groups_Group_AddLeaderOrMember(15, groupName, individualName, "Leader");

            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

            // Set window size to simulate iPhone
            test.Infellowship.SetBrowserSizeTo_Mobile();

            // Verify the attendance tab is present
            test.Infellowship.Groups_Group_View_WebDriver(groupName);
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Attendance")));

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you cannot post attendance if the group has no attendance data.")]
        public void Attendance_View_Group_Without_Attendance_WebDriver_RESPONSIVE()
        {
            // Set initial conditions
            string individualName = "Matthew Sneeden";
            string groupName = "No Post Attendance Group Resp";

            // Clean up all the left over data that could be hanging around
            base.SQL.Groups_Group_Delete(15, groupName);
            base.SQL.Groups_Group_DeleteAttendanceSummary(15, groupName);

            // Create the group type
            //base.SQL.Groups_CreateGroupType(15, individualName, groupTypeName, new Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights[] { Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights.ChangeScheduleLocation, Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights.TakeAttendance });
            base.SQL.Groups_GroupType_Create(15, individualName, _groupTypeName, new List<int> { 4, 5, 10 });

            // Spin up a group
            base.SQL.Groups_Group_Create(15, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());

            // Add a leader
            base.SQL.Groups_Group_AddLeaderOrMember(15, groupName, individualName, "Leader");

            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

            // Set window size to simulate iPhone
            test.Infellowship.SetBrowserSizeTo_Mobile();

            // View the group
            test.Infellowship.Groups_Group_View_WebDriver(groupName);

            // Verify the attendance tab is still present since you can post attendance by adding a new date
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Attendance")));

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a group member cannot post attendance for a group if attendance is enabled for the group.")]
        public void Attendance_View_Member_Cannot_Post_WebDriver_RESPONSIVE()
        {
            // Set initial conidiitions
            string individualName = "Bryan Mikaelian";
            string groupName = "Post Attendance Group Resp";

            // Create a group
            base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());

            // Add a member
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Member", "Member");

            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Set window size to simulate iPhone
            test.Infellowship.SetBrowserSizeTo_Mobile();

            // Click on Your Groups
            test.Driver.FindElementByLinkText(GeneralInFellowshipConstants.LandingPage.Link_YourGroups_WebDriver).Click();

            //Verfiy the "Take Attendance" button is not present for the member
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//a[contains(@class, btn) and text()='Take Attendance']")));

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a you cannot post attendance twice for the same day.")]
        public void Attendance_Post_Did_Meet_Cannot_Post_Twice_WebDriver_RESPONSIVE()
        {
            // Set initial conidiitions
            string individualName = "Group Leader";
            string groupName = "Post Attendance Group - Double Post Resp";
            List<string> individualsInGroup = new List<string>();
            individualsInGroup.Add(individualName);
            // Clean up all the left over data that could be hanging around
            base.SQL.Groups_Group_Delete(254, groupName);
            
            base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, individualName, "Leader");
            string attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false);


            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Set window size to simulate iPhone
            test.Infellowship.SetBrowserSizeTo_Mobile();

            // Enter attendance that did meet
            test.Infellowship.Groups_Attendance_Enter_All_WebDriver(groupName, attendanceDate, true);

            // Select enter attendance
            test.Driver.FindElementByLinkText("Enter attendance").Click();

            // Verify that date is no longer in the drop down
            Assert.IsFalse(test.Driver.FindElementByTagName("html").Text.Contains(attendanceDate));

            // Cancel        
            var btnCancel = test.Driver.FindElementsByXPath(".//a[contains(@class,'btn') and text()='Cancel']").FirstOrDefault(x => x.Displayed == true);
            btnCancel.Click();
           
            // Logout
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a you can post attendance for a given date that a group did meet.")]
        public void Attendance_Post_Did_Meet_WebDriver_RESPONSIVE()
        {
            // Set initial conidiitions
            string individualName = "Group Leader";
            string groupName = "Post Attendance Group - Responsive";
            List<string> individualsInGroup = new List<string>();
            individualsInGroup.Add(individualName);
            // Clean up all the left over data that could be hanging around
            base.SQL.Groups_Group_Delete(254, groupName);
                        
            base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, individualName, "Leader");
            
            var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false);

            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");
            

            // Set window size to simulate iPhone
            test.Infellowship.SetBrowserSizeTo_Mobile();

            // Enter attendance that did meet            
            test.Infellowship.Groups_Attendance_Enter_All_WebDriver(groupName, attendanceDate, true);

            // Logout
            test.Infellowship.LogoutWebDriver();

        }

        #endregion Responsive
    }
}