using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace FTTests.Portal.Ministry {
	[TestFixture]
	public class Portal_Ministry_Attendance : FixtureBase {

		#region Post Attendance
		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you can post attendance for an individual after searching for an individual using Search All.")]
		public void Ministry_Attendance_PostAttendance_Search_All_Records_Individual() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");
            
            // Post Attendance for an individual
            test.Portal.Ministry_PostAttendance_Update_RLC_Add_Attendee("Automation Ministry", "Test Activity", "Test Schedule", string.Format("{0}/1/{1} 9:20 AM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, DateTime.Now.Year), "Test RLC", "Bryan Mikaelian");

            // Verify person was added and you can see them under ALL and the resepective letter for their last name.
            test.Selenium.VerifyTextPresent("Attendance posted for 1 person.");
            test.Selenium.ClickAndWaitForPageToLoad("link=View all attendees");
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Ministry_PostAttendance_Attendees, "Bryan Mikaelian Bryan Mikaelian household", "Attendees"));
            test.Selenium.ClickAndWaitForPageToLoad("link=M");
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Ministry_PostAttendance_Attendees, "Bryan Mikaelian Bryan Mikaelian household", "Attendees"));
            
            // Remove all attendees
            test.Portal.Ministry_PostAttendance_Remove_RLC_Attendees("Automation Ministry", "Test Activity", "Test Schedule", string.Format("{0}/1/{1} 9:20 AM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, DateTime.Now.Year), "Test RLC");

			// Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies that searching for someone that doesn't exist returns proper validation.")]
		public void Ministry_Attendance_PostAttendance_Search_All_Records_No_Results() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // View the Post Attendance page for a particular RLC
            test.Portal.Ministry_PostAttendance_View_RLC_Post_Attendace_Page("Automation Ministry", "Test Activity", "Test Schedule", string.Format("{0}/1/{1} 9:20 AM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, DateTime.Now.Year), "Test RLC");

            // Search for an individual that would return 0 results.
            test.Selenium.Type(MinistryAttendanceConstants.PostAttendance.TextField_NameSearch, "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.Search);

			// Verify search results returned nothing
			test.Selenium.VerifyTextPresent("Sorry, no results found.");

			// Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you can post attendance for an individual after searching for an individual using the advanced search tool.")]
        [DependsOn("Ministry_Attendance_PostAttendance_Search_All_Records_Individual")]
		public void Ministry_Attendance_PostAttendance_Search_All_Records_Advanced_Search() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Post Attendance for an individual, searching for an individual using advanced search.
            test.Portal.Ministry_PostAttendance_Update_RLC_Add_Attendee("Automation Ministry", "Test Activity", "Test Schedule", string.Format("{0}/1/{1} 9:20 AM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, DateTime.Now.Year), "Test RLC", "Bryan Mikaelian", "2812 Meadow Wood Drive", null, null, null, null);

            // Verify person was added and you can see them under ALL and the resepective letter for their last name.
            test.Selenium.VerifyTextPresent("Attendance posted for 1 person.");
            test.Selenium.ClickAndWaitForPageToLoad("link=View all attendees");
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Ministry_PostAttendance_Attendees, "Bryan Mikaelian Bryan Mikaelian household", "Attendees"));
            test.Selenium.ClickAndWaitForPageToLoad("link=M");
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Ministry_PostAttendance_Attendees, "Bryan Mikaelian Bryan Mikaelian household", "Attendees"));

            // Remove all attendees
            test.Portal.Ministry_PostAttendance_Remove_RLC_Attendees("Automation Ministry", "Test Activity", "Test Schedule", string.Format("{0}/1/{1} 9:20 AM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, DateTime.Now.Year), "Test RLC");

			// Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies that searching for someone that doesn't exist returns an error message when you use advanced search.")]
		public void Ministry_Attendance_PostAttendance_Search_All_Records_Advanced_Search_No_Results() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // View the Post Attendance page for a particular RLC
            test.Portal.Ministry_PostAttendance_View_RLC_Post_Attendace_Page("Automation Ministry", "Test Activity", "Test Schedule", string.Format("{0}/1/{1} 9:20 AM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, DateTime.Now.Year), "Test RLC");

			// Select advanced search
            test.Selenium.Click("link=Advanced search");

			// Perform an advanced search with no results expected
            test.Selenium.Type("search", "zzzzzzzzzzzzzzzzzzz");
            test.Selenium.Type("address", "zzzzzzzzzzzzzzzzzzz");
			test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.Search);

			// Verify search results returned nothing
			test.Selenium.VerifyTextPresent("Sorry, no results found.");

			// Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you can search all staff assignments records for someone who has a staff assignment, post attendance, and they show up")]
        [DependsOn("Ministry_Attendance_PostAttendance_Search_All_Records_Advanced_Search")]
		public void Ministry_Attendance_PostAttendance_Search_Staff_Assignments() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Post Attendance for an individual with a staffing assignment
            test.Portal.Ministry_PostAttendance_Update_RLC_Add_Staff("Automation Ministry", "Test Activity", "Test Schedule", string.Format("{0}/1/{1} 9:20 AM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, DateTime.Now.Year), "Test RLC", "Bryan Mikaelian");

            // Verify person was added and you can see them under ALL and the resepective letter for their last name.
            test.Selenium.VerifyTextPresent("Attendance posted for 1 person.");
            test.Selenium.ClickAndWaitForPageToLoad("link=View all attendees");

            // All
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Ministry_PostAttendance_Attendees, "Bryan Mikaelian Bryan Mikaelian household", "Attendees"));
            test.Selenium.ClickAndWaitForPageToLoad("link=M");
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Ministry_PostAttendance_Attendees, "Bryan Mikaelian Bryan Mikaelian household", "Attendees"));

            // Filter on Staff Assignments
            test.Selenium.Select(MinistryAttendanceConstants.PostAttendance.DropDown_AttendeeFilter, "Staff");
            test.Selenium.ClickAndWaitForPageToLoad(MinistryAttendanceConstants.PostAttendance.Button_FilterAttendees);

            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Ministry_PostAttendance_Attendees, "Bryan Mikaelian Bryan Mikaelian household", "Attendees"));
            test.Selenium.ClickAndWaitForPageToLoad("link=M");
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Ministry_PostAttendance_Attendees, "Bryan Mikaelian Bryan Mikaelian household", "Attendees"));

            // Filter on Participant Assignments
            test.Selenium.Select(MinistryAttendanceConstants.PostAttendance.DropDown_AttendeeFilter, "Participant");
            test.Selenium.ClickAndWaitForPageToLoad(MinistryAttendanceConstants.PostAttendance.Button_FilterAttendees);

            Assert.IsFalse(test.Selenium.IsElementPresent(TableIds.Ministry_PostAttendance_Attendees));

			// Clean up so this test and future tests can be re-ran
            test.Portal.Ministry_PostAttendance_Remove_RLC_Attendees("Automation Ministry", "Test Activity", "Test Schedule", string.Format("{0}/1/{1} 9:20 AM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, DateTime.Now.Year), "Test RLC");

			// Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you can change someone from Staff to a participant.")]
        [DependsOn("Ministry_Attendance_PostAttendance_Search_Staff_Assignments")]
		public void Ministry_Attendance_PostAttendance_Update_Attendance_Details_Staff_To_Participant() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Post attendance for an individual
            test.Portal.Ministry_PostAttendance_Update_RLC_Add_Staff("Automation Ministry", "Test Activity", "Test Schedule", string.Format("{0}/1/{1} 9:20 AM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, DateTime.Now.Year), "Test RLC", "Bryan Mikaelian");

            // Update their attendance details
            test.Portal.Ministry_PostAttendance_Update_Attendance_Details("Automation Ministry", "Test Activity", "Test Schedule", string.Format("{0}/1/{1} 9:20 AM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, DateTime.Now.Year), "Test RLC", "Bryan Mikaelian", GeneralEnumerations.AssignmentRole.Participant);

            // Verify change occured
            test.Portal.Ministry_PostAttendance_View_RLC_Attendees("Automation Ministry", "Test Activity", "Test Schedule", string.Format("{0}/1/{1} 9:20 AM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, DateTime.Now.Year), "Test RLC");

            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Ministry_PostAttendance_Attendees, "Bryan Mikaelian Bryan Mikaelian household", "Attendees"));
            test.Selenium.ClickAndWaitForPageToLoad("link=M");
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Ministry_PostAttendance_Attendees, "Bryan Mikaelian Bryan Mikaelian household", "Attendees"));

            // Filter on Participant Assignments
            test.Selenium.Select(MinistryAttendanceConstants.PostAttendance.DropDown_AttendeeFilter, "Participant");
            test.Selenium.ClickAndWaitForPageToLoad(MinistryAttendanceConstants.PostAttendance.Button_FilterAttendees);

            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Ministry_PostAttendance_Attendees, "Bryan Mikaelian Bryan Mikaelian household", "Attendees"));
            test.Selenium.ClickAndWaitForPageToLoad("link=M");
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Ministry_PostAttendance_Attendees, "Bryan Mikaelian Bryan Mikaelian household", "Attendees"));

            // Filter on Staff Assignments
            test.Selenium.Select(MinistryAttendanceConstants.PostAttendance.DropDown_AttendeeFilter, "Staff");
            test.Selenium.ClickAndWaitForPageToLoad(MinistryAttendanceConstants.PostAttendance.Button_FilterAttendees);
            Assert.IsFalse(test.Selenium.IsElementPresent(TableIds.Ministry_PostAttendance_Attendees));

            // Clean up so this test and future tests can be re-ran
            test.Portal.Ministry_PostAttendance_Remove_RLC_Attendees("Automation Ministry", "Test Activity", "Test Schedule", string.Format("{0}/1/{1} 9:20 AM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, DateTime.Now.Year), "Test RLC");

			// Logout of portal
			test.Portal.Logout();
		}

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can change someone from a participant assignment to a staffing assignment.")]
        [DependsOn("Ministry_Attendance_PostAttendance_Update_Attendance_Details_Staff_To_Participant")]
        public void Ministry_Attendance_PostAttendance_Update_Attendance_Details_Participant_To_Staff() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Post attendance for an individual
            test.Portal.Ministry_PostAttendance_Update_RLC_Add_Attendee("Automation Ministry", "Test Activity", "Test Schedule", string.Format("{0}/1/{1} 9:20 AM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, DateTime.Now.Year), "Test RLC", "Bryan Mikaelian");

            // Update their attendance details
            test.Portal.Ministry_PostAttendance_Update_Attendance_Details("Automation Ministry", "Test Activity", "Test Schedule", string.Format("{0}/1/{1} 9:20 AM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, DateTime.Now.Year), "Test RLC", "Bryan Mikaelian", GeneralEnumerations.AssignmentRole.Staff);

            // Verify change occured
            test.Portal.Ministry_PostAttendance_View_RLC_Attendees("Automation Ministry", "Test Activity", "Test Schedule", string.Format("{0}/1/{1} 9:20 AM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, DateTime.Now.Year), "Test RLC");

            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Ministry_PostAttendance_Attendees, "Bryan Mikaelian Bryan Mikaelian household", "Attendees"));
            test.Selenium.ClickAndWaitForPageToLoad("link=M");
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Ministry_PostAttendance_Attendees, "Bryan Mikaelian Bryan Mikaelian household", "Attendees"));

            // Filter on Staff Assignments
            test.Selenium.Select(MinistryAttendanceConstants.PostAttendance.DropDown_AttendeeFilter, "Staff");
            test.Selenium.ClickAndWaitForPageToLoad(MinistryAttendanceConstants.PostAttendance.Button_FilterAttendees);

            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Ministry_PostAttendance_Attendees, "Bryan Mikaelian Bryan Mikaelian household", "Attendees"));
            test.Selenium.ClickAndWaitForPageToLoad("link=M");
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Ministry_PostAttendance_Attendees, "Bryan Mikaelian Bryan Mikaelian household", "Attendees"));

            // Filter on Participant Assignments
            test.Selenium.Select(MinistryAttendanceConstants.PostAttendance.DropDown_AttendeeFilter, "Participant");
            test.Selenium.ClickAndWaitForPageToLoad(MinistryAttendanceConstants.PostAttendance.Button_FilterAttendees);

            Assert.IsFalse(test.Selenium.IsElementPresent(TableIds.Ministry_PostAttendance_Attendees));

            // Clean up so this test and future tests can be re-ran
            test.Portal.Ministry_PostAttendance_Remove_RLC_Attendees("Automation Ministry", "Test Activity", "Test Schedule", string.Format("{0}/1/{1} 9:20 AM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, DateTime.Now.Year), "Test RLC");

            // Logout of portal
            test.Portal.Logout();
        }
 
        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can specify a job and a time served for an attendee.")]
        [DependsOn("Ministry_Attendance_PostAttendance_Update_Attendance_Details_Participant_To_Staff")]
        public void Ministry_Attendance_PostAttendance_Update_Attendance_Details_Add_Job_And_Time_Served() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Post attendance for an individual
            test.Portal.Ministry_PostAttendance_Update_RLC_Add_Attendee("Automation Ministry", "Test Activity", "Test Schedule", string.Format("{0}/1/{1} 9:20 AM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, DateTime.Now.Year), "Test RLC", "Bryan Mikaelian");

            // Update their attendance details
            test.Portal.Ministry_PostAttendance_Update_Attendance_Details("Automation Ministry", "Test Activity", "Test Schedule", string.Format("{0}/1/{1} 9:20 AM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, DateTime.Now.Year), "Test RLC", "Bryan Mikaelian", GeneralEnumerations.AssignmentRole.Staff, "Test Job", "1:30");
            
            // Verify the Job and Time Served was applied.
            decimal itemRow = test.GeneralMethods.GetTableRowNumber(TableIds.Ministry_PostAttendance_Attendees, "Bryan Mikaelian", "Attendees", "contains");
            Assert.AreEqual("Test Job 1:30", test.Selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[3]/small[2]", TableIds.Ministry_PostAttendance_Attendees, itemRow + 1)));

            // Clean up so this test and future tests can be re-ran
            test.Portal.Ministry_PostAttendance_Remove_RLC_Attendees("Automation Ministry", "Test Activity", "Test Schedule", string.Format("{0}/1/{1} 9:20 AM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, DateTime.Now.Year), "Test RLC");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies deceased individuals are not returned in the search")]
        public void Ministry_Attendance_PostAttendance_Search_All_Records_Decease_Individuals_Not_Returned() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // View the Post Attendance page for a particular RLC
            test.Portal.Ministry_PostAttendance_View_RLC_Post_Attendace_Page("Automation Ministry", "Test Activity", "Test Schedule", string.Format("{0}/1/{1} 9:20 AM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, DateTime.Now.Year), "Test RLC");

            // Search for a deceased person
            test.Selenium.Type(MinistryAttendanceConstants.PostAttendance.TextField_NameSearch, "Dead Individual");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.Search);

            // Verify validation message
            test.Selenium.VerifyTextPresent("Sorry, no results found.");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you need to provide at least 2 characters to search.")]
        public void Ministry_Attendance_PostAttendance_Search_All_Records_Two_Character_Minimum() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // View the Post Attendance page for a particular RLC
            test.Portal.Ministry_PostAttendance_View_RLC_Post_Attendace_Page("Automation Ministry", "Test Activity", "Test Schedule", string.Format("{0}/1/{1} 9:20 AM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, DateTime.Now.Year), "Test RLC");

            // Search for a deceased person
            test.Selenium.Type(MinistryAttendanceConstants.PostAttendance.TextField_NameSearch, "a");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.Search);

            // Verify validation message
            test.Selenium.VerifyTextPresent(" You must enter at least 2 characters to perform search ");

            // Logout of portal
            test.Portal.Logout();
        }
		#endregion Post Attendance

		#region Head Count
		#endregion Head Count

		#region Automated Attendance Rules
		//[Test, RepeatOnFailure(Order=1)]
		[Author("Matthew Sneeden")]
		[Description("Creates an Automated Attendance Rule.")]
		public void Ministry_Attendance_AutomatedAttendanceRules_Create() {
            // Set initial conditions
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";
            string rlcName = "A Test RLC";

            TestLog.WriteLine("SQL Ministry_AutomatedAttendanceRules_Delete");
            base.SQL.Ministry_AutomatedAttendanceRules_Delete(15, ministryName, activityName, rlcName);
            //base.SQL.Ministry_ActivityDetails_Create(15, activityName, rlcName);

			// Login to portal
            TestLog.WriteLine("Login to Portal");
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Create an automated attendance rule
            TestLog.WriteLine("Create an automated attendance rule");
			test.Portal.Ministry_AutomatedAttendanceRules_Create(ministryName, activityName, rlcName, new string[] { "ctl00_ctl00_MainContent_content_chkBoxHouseholdHead" });

			// Logout of portal
            TestLog.WriteLine("Logout Portal");
			test.Portal.Logout();

            TestLog.WriteLine("SQL Ministry_AutomatedAttendanceRules_Delete");
            //base.SQL.Ministry_ActivityDetails_Delete(15, activityName, activityName);
            base.SQL.Ministry_AutomatedAttendanceRules_Delete(15, ministryName, activityName, rlcName);

		}

		//[Test, RepeatOnFailure(Order=2)]
		[Author("Matthew Sneeden")]
		[Description("Updates an Automated Attendance Rule.")]
		public void Ministry_Attendance_AutomatedAttendanceRules_Delete() {

            // Set initial conditions
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";
            string rlcName = "A Test RLC Delete";

            TestLog.WriteLine("SQL Create " + ministryName + "/" + activityName + "/" + rlcName);
            //base.SQL.Ministry_ActivityDetails_Create(15, activityName, rlcName);

//            System.Threading.Thread.Sleep(5000);

			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
			//test.Portal.Login("ft.tester", "FT4life!", "dc");
            test.Portal.Login();

            // Create an automated attendance rule
            TestLog.WriteLine("Ministry_AutomatedAttendanceRules_Create");
            test.Portal.Ministry_AutomatedAttendanceRules_Create(ministryName, activityName, rlcName, new string[] { "ctl00_ctl00_MainContent_content_chkBoxHouseholdHead" });

			// Delete an automated attendance rule
            TestLog.WriteLine("Ministry_AutomatedAttendanceRules_Delete");
			test.Portal.Ministry_AutomatedAttendanceRules_Delete(string.Format("{0} > {1} > {2}", ministryName, activityName, rlcName));

			// Logout of portal
            TestLog.WriteLine("Logout Portal");
			test.Portal.Logout();

            //Clean up
            //base.SQL.Ministry_ActivityDetails_Delete(15, activityName, rlcName);


        }

		//[Test, RepeatOnFailure(Order=3)]
		[Author("Matthew Sneeden")]
		[Description("Creates an Automated Attendance Rule with all options.")]
		public void Ministry_Attendance_AutomatedAttendanceRules_CreateAllOptions() {
            // Set initial conditions
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";
            string rlcGroupName = "All Rosters";
            string rlc = "A Test RLC: All";
            string rlcName = "All Rosters > A Test RLC: All";

            TestLog.WriteLine("SQL Delete/Create");
            base.SQL.Ministry_AutomatedAttendanceRules_Delete(15, ministryName, activityName, rlc);
            //base.SQL.Ministry_ActivityDetails_Delete(15, activityName, rlcName);
            //base.SQL.Ministry_ActivityDetails_Create(15, activityName, rlcName);

			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login Portal");
            test.Portal.Login();

			// Create an automated attendance rule
            TestLog.WriteLine("Create automate attendance rule");
			test.Portal.Ministry_AutomatedAttendanceRules_Create(ministryName, activityName, rlc, new string[] { "ctl00_ctl00_MainContent_content_chkBoxHouseholdHead", 
                "ctl00_ctl00_MainContent_content_chkBoxHouseholdSpouse", "ctl00_ctl00_MainContent_content_chkBoxHouseholdChild", "ctl00_ctl00_MainContent_content_chkBoxContributorSpouse", 
                "ctl00_ctl00_MainContent_content_chkBoxContributorChildren", "ctl00_ctl00_MainContent_content_chkBoxChildParents", "ctl00_ctl00_MainContent_content_chkBoxSiblings" });

			// Logout of portal
            TestLog.WriteLine("Logout Portal");
			test.Portal.Logout();

            // Clean up
            base.SQL.Ministry_AutomatedAttendanceRules_Delete(15, ministryName, activityName, rlcName);

        }

		//[Test, RepeatOnFailure(Order=4)]
		[Author("Matthew Sneeden")]
		[Description("Creates an Automated Attendance Rule with no options.")]
		public void Ministry_Attendance_AutomatedAttendanceRules_NoOptions() {
            // Set initial conditions
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";
            string rlcName = "A Test RLC: None";
            base.SQL.Ministry_AutomatedAttendanceRules_Delete(15, ministryName, activityName, rlcName);
            //base.SQL.Ministry_ActivityDetails_Delete(15, activityName, rlcName);
            //base.SQL.Ministry_ActivityDetails_Create(15, activityName, rlcName);

			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Create an automated attendance rule
			test.Portal.Ministry_AutomatedAttendanceRules_Create(ministryName, activityName, rlcName, null);

			// Logout of portal
			test.Portal.Logout();

            // Clean up
            //base.SQL.Ministry_AutomatedAttendanceRules_Delete(15, ministryName, activityName, rlcName);
//            base.SQL.Ministry_ActivityDetails_Delete(15, activityName, rlcName);

        }
		#endregion Automated Attendance Rules
	}

    [TestFixture]
    public class Portal_Ministry_Attendance_WebDriver : FixtureBaseWebDriver
    {
        #region Automated Attendance Rules
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Creates an Automated Attendance Rule.")]
        public void Ministry_Attendance_AutomatedAttendanceRules_Create_WebDriver()
        {
            // Set initial conditions
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";
            string rlcName = "A Test RLC";

            TestLog.WriteLine("SQL Ministry_AutomatedAttendanceRules_Delete");
            base.SQL.Ministry_AutomatedAttendanceRules_Delete(15, ministryName, activityName, rlcName);
            //base.SQL.Ministry_ActivityDetails_Create(15, activityName, rlcName);

            // Login to portal
            TestLog.WriteLine("Login to Portal");
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Create an automated attendance rule
            TestLog.WriteLine("Create an automated attendance rule");
            test.Portal.Ministry_AutomatedAttendanceRules_Create_WebDriver(ministryName, activityName, rlcName, new string[] { GeneralMinistry.AutoAttendanceRules.HH_Head });

            // Logout of portal
            TestLog.WriteLine("Logout Portal");
            test.Portal.LogoutWebDriver();

            TestLog.WriteLine("SQL Ministry_AutomatedAttendanceRules_Delete");
            //base.SQL.Ministry_ActivityDetails_Delete(15, activityName, activityName);
            base.SQL.Ministry_AutomatedAttendanceRules_Delete(15, ministryName, activityName, rlcName);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [DependsOn("Ministry_Attendance_AutomatedAttendanceRules_Create_WebDriver")]
        [Description("Updates an Automated Attendance Rule.")]
        public void Ministry_Attendance_AutomatedAttendanceRules_Delete_WebDriver()
        {

            // Set initial conditions
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";
            string rlcName = "A Test RLC Delete";

            TestLog.WriteLine("SQL Create " + ministryName + "/" + activityName + "/" + rlcName);
            //base.SQL.Ministry_ActivityDetails_Create(15, activityName, rlcName);

            //            System.Threading.Thread.Sleep(5000);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Create an automated attendance rule
            TestLog.WriteLine("Ministry_AutomatedAttendanceRules_Create");
            test.Portal.Ministry_AutomatedAttendanceRules_Create_WebDriver(ministryName, activityName, rlcName, new string[] { GeneralMinistry.AutoAttendanceRules.HH_Head });

            // Delete an automated attendance rule
            TestLog.WriteLine("Ministry_AutomatedAttendanceRules_Delete");
            test.Portal.Ministry_AutomatedAttendanceRules_Delete_WebDriver(string.Format("{0} > {1} > {2}", ministryName, activityName, rlcName));

            // Logout of portal
            TestLog.WriteLine("Logout Portal");
            test.Portal.LogoutWebDriver();

            //Clean up
            //base.SQL.Ministry_ActivityDetails_Delete(15, activityName, rlcName);


        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [DependsOn("Ministry_Attendance_AutomatedAttendanceRules_Delete_WebDriver")]
        [Description("Creates an Automated Attendance Rule with all options.")]
        public void Ministry_Attendance_AutomatedAttendanceRules_CreateAllOptions_WebDriver()
        {
            // Set initial conditions
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";
            string rlcGroupName = "All Rosters";
            string rlc = "A Test RLC: All";
            string rlcName = "All Rosters > A Test RLC: All";

            TestLog.WriteLine("SQL Delete/Create");
            base.SQL.Ministry_AutomatedAttendanceRules_Delete(15, ministryName, activityName, rlc);
            //base.SQL.Ministry_ActivityDetails_Delete(15, activityName, rlcName);
            //base.SQL.Ministry_ActivityDetails_Create(15, activityName, rlcName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login Portal");
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Create an automated attendance rule
            TestLog.WriteLine("Create automate attendance rule");
            test.Portal.Ministry_AutomatedAttendanceRules_Create_WebDriver(ministryName, activityName, rlc, new string[] { GeneralMinistry.AutoAttendanceRules.HH_Head, GeneralMinistry.AutoAttendanceRules.HH_Spouse, 
                GeneralMinistry.AutoAttendanceRules.HH_Child, GeneralMinistry.AutoAttendanceRules.Contributor_Spouse, GeneralMinistry.AutoAttendanceRules.Contributor_Children, 
                GeneralMinistry.AutoAttendanceRules.Child_Parents, GeneralMinistry.AutoAttendanceRules.Child_Siblings });

            // Logout of portal
            TestLog.WriteLine("Logout Portal");
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Ministry_AutomatedAttendanceRules_Delete(15, ministryName, activityName, rlcName);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [DependsOn("Ministry_Attendance_AutomatedAttendanceRules_CreateAllOptions_WebDriver")]
        [Description("Creates an Automated Attendance Rule with no options.")]
        public void Ministry_Attendance_AutomatedAttendanceRules_NoOptions_WebDriver()
        {
            // Set initial conditions
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";
            string rlcName = "A Test RLC: None";
            base.SQL.Ministry_AutomatedAttendanceRules_Delete(15, ministryName, activityName, rlcName);
            //base.SQL.Ministry_ActivityDetails_Delete(15, activityName, rlcName);
            //base.SQL.Ministry_ActivityDetails_Create(15, activityName, rlcName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Create an automated attendance rule
            test.Portal.Ministry_AutomatedAttendanceRules_Create_WebDriver(ministryName, activityName, rlcName, null);

            // Logout of portal
            test.Portal.LogoutWebDriver();

            // Clean up
            //base.SQL.Ministry_AutomatedAttendanceRules_Delete(15, ministryName, activityName, rlcName);
            //            base.SQL.Ministry_ActivityDetails_Delete(15, activityName, rlcName);

        }
        #endregion Automated Attendance Rules

        #region Post Attendance

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Stuart PLatt")]
        [Description("Verifies that Staff attendance is posted properly for Assignmees")]
        public void Ministry_Attendance_PostAttendance_Staff_AllAssigned()
        {
            string ministry = "A Test Ministry";
            string activity = "Staff Assignment - Automation";
            string roster = "Staff roster";
            string schedule = "Daily Schedule";
            string scheduleDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(-1).ToString("M/d/yyyy");
            string selectDate = scheduleDate + " " + "8:00 AM";

            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to the Post Attendance page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Attendance.Post_Attendance);
            
            //Post Attendance
            test.Portal.Ministry_PostAttendance_RLC_Add_Attendance_All_Assigned_WebDriver(ministry, activity, null, schedule, null, selectDate, roster, false);

            //Navigate to Attendees page
            test.Driver.FindElementByLinkText("View all attendees").Click();
            test.GeneralMethods.WaitForElement(By.Id(GeneralMinistry.PostAttendance.Action_DropDown));

            //Verify that the staff attendance matches the assignments
            Assert.AreEqual("Angela Platt", test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgResults_ctl04_lblName").Text);
            Assert.AreEqual("Job A", test.Driver.FindElementByXPath("//table[@id='ctl00_ctl00_MainContent_content_dgResults']/tbody/tr[4]/td[3]/small[2]").Text);
            Assert.AreEqual("Lindsley Platt", test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgResults_ctl05_lblName").Text);
            Assert.AreEqual("Job B", test.Driver.FindElementByXPath("//table[@id='ctl00_ctl00_MainContent_content_dgResults']/tbody/tr[5]/td[3]/small[2]").Text);
            Assert.AreEqual("Miles Platt", test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgResults_ctl06_lblName").Text);
            //TestLog.WriteLine(string.Format("The Tag name is  '{0}'", test.Driver.FindElementByXPath("table[@id='ctl00_ctl00_MainContent_content_dgResults']/tbody/tr[6]/td[2]/span").TagName));
            //Assert.AreEqual("staff_dot", test.Driver.FindElementByXPath("table[@id='ctl00_ctl00_MainContent_content_dgResults']/tbody/tr[6]/td[2]/span").TagName.ToString());
            
            //Remove Attendance
            test.Portal.Ministry_PostAttendance_Remove_All_Attendees_WebDriver();

            //Logout
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Stuart Platt")]
        [Description("Posts attendance for all participants assigned to an activity roster")]
        public void Ministry_Attendance_PostAttendance_Participants_AllAssigned()
        {
            string ministry = "A Test Ministry";
            string activity = "Participants - Automation";
            string roster = "Participant roster";
            string schedule = "Daily Schedule";
            string scheduleDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(-1).ToString("M/d/yyyy");
            string selectDate = scheduleDate + " " + "8:00 AM";

            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to the Post Attendance page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Attendance.Post_Attendance);

            //Post Attendance
            test.Portal.Ministry_PostAttendance_RLC_Add_Attendance_All_Assigned_WebDriver(ministry, activity, null, schedule, null, selectDate, roster);

            //Navigate to Attendees page
            test.Driver.FindElementByLinkText("View all attendees").Click();
            test.GeneralMethods.WaitForElement(By.Id(GeneralMinistry.PostAttendance.Action_DropDown));

            //Verify that the staff attendance matches the assignments
            Assert.AreEqual("Angela Platt", test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgResults_ctl04_lblName").Text);
            Assert.AreEqual("Lindsley Platt", test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgResults_ctl05_lblName").Text);
            Assert.AreEqual("Miles Platt", test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgResults_ctl06_lblName").Text);
            

            //Remove Attendance
            test.Portal.Ministry_PostAttendance_Remove_All_Attendees_WebDriver();

            //Logout
            test.Portal.LogoutWebDriver();
        }

        #endregion Post Attendance

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies search out long result list when post attendance works well")]
        public void Ministry_Attendance_PostAttendance_FindPeople_LongResultList()
        {
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";
            string scheduleName = "wSchedule Long Individual";
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string start_Time = Convert.ToString(now.AddHours(2));
            string end_Time = Convert.ToString(now.AddHours(3));
            string start_Date = Convert.ToString(now.Date.AddDays(1).ToShortDateString());
            string postTime = Convert.ToString(now.AddHours(-3).AddMinutes(-3).ToShortTimeString());
           
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                // Login to Portal
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");
                test.Portal.Ministry_Activities_Schedules_Add_OneTime(ministryName, activityName, scheduleName, start_Time, end_Time, start_Date);

                #region go to the post attendance page.
                // Post Attendance for an individual, searching for an individual using advanced search.
                TestLog.WriteLine("Step 2: go to the post attendance page.");
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Attendance.Post_Attendance);

                //Select Ministry to post attendance for
                string selectMinistry = test.Driver.FindElementById(GeneralMinistry.PostAttendance.MinistryName).Text;
                if (selectMinistry != ministryName)
                {
                    test.Driver.FindElementByLinkText("Change").Click();
                    new SelectElement(test.Driver.FindElementById(GeneralMinistry.PostAttendance.Ministry_DropDown)).SelectByText(ministryName);
                    test.GeneralMethods.WaitForElementEnabled(By.Id(GeneralMinistry.PostAttendance.Activity_DropDown));
                }
                //Select Activity 
                new SelectElement(test.Driver.FindElementById(GeneralMinistry.PostAttendance.Activity_DropDown)).SelectByText(activityName);
                test.GeneralMethods.WaitForElementEnabled(By.Id(GeneralMinistry.PostAttendance.Schedule_DropDown));
                //Select Schedule
                new SelectElement(test.Driver.FindElementById(GeneralMinistry.PostAttendance.Schedule_DropDown)).SelectByText(scheduleName);
                test.GeneralMethods.WaitForElementEnabled(By.Id(GeneralMinistry.PostAttendance.Time_DropDown));
                //Select DateTime
                new SelectElement(test.Driver.FindElementById(GeneralMinistry.PostAttendance.Time_DropDown)).SelectByIndex(1);
                //Search
                test.Driver.FindElementById(GeneralMinistry.PostAttendance.Search_Button).Click();
                test.GeneralMethods.WaitForElement(By.LinkText("Post attendance"));

                //Go to Post Attendance screen
                test.Driver.FindElementByLinkText("Post attendance").Click();
                test.GeneralMethods.WaitForElement(By.Id(GeneralMinistry.PostAttendance.SearchAll_Radio));
                #endregion go to the post attendance page.

                #region check data.
                TestLog.WriteLine("Step 3: Check data xss attack.");
                bool isXssAttack;
                //find element & active.[Check xss for first name].
                isXssAttack = this.Ministry_Attendance_PostAttendance_FindPeople_ByName_CheckData_SecurityXSS(test, "LN", "test");
                Assert.IsFalse(isXssAttack, "'FirstName' is vulnerable to xss attack!");

                test.GeneralMethods.WaitForElementVisible(By.Id("post_attendance"));

                #endregion check data.
            }
            finally
            {

                //Clear add information.
                TestLog.WriteLine("Step 4: Clear add information.");
                //Clear basic information.
                test.Portal.Ministry_Activities_Schedules_Delete_Schedule_ByName(ministryName, activityName, scheduleName);            

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }
        }

        [Test, RepeatOnFailure]
        [Author("Alan Luo")]
        [Description("Verifies Security XSS when searching for an individual.")]
        public void Ministry_Attendance_PostAttendance_FindPeople_ByName_SecurityXSS()
        {
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";
            string scheduleName = string.Format("XSS{0}", DateTime.Now.Ticks.ToString());
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string start_Time = Convert.ToString(now.AddHours(2));
            string end_Time = Convert.ToString(now.AddHours(3));
            string start_Date = Convert.ToString(now.Date.AddDays(1).ToShortDateString());
            string postTime = Convert.ToString(now.AddHours(-3).AddMinutes(-3).ToShortTimeString());
            
            int churchId = 15;
            string security_FirstName = "FirstNameSecurity";
            string security_LastName = "LastNameSecurity";
            string security_Name = string.Format("{0} {1}", security_FirstName, security_LastName);

            int individualId = 0;
            int houseHoldId = 0;

            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                #region Init Data
                //Create basic people.
                TestLog.WriteLine("Step 1: Init Data");
                test.SQL.People_MergeIndividual(churchId, security_Name, "Merge Dump");
                test.SQL.People_Individual_Create(churchId, security_FirstName, security_LastName);
                houseHoldId = test.SQL.People_Households_FetchID(churchId, this.SQL.People_Individuals_FetchID(churchId, security_Name));
                TestLog.WriteLine(string.Format("Add individual Name:{0}.", security_Name));

                individualId = test.SQL.People_Individual_Only_Create_Param(churchId, "xssFirstName", "xssLastName");
                test.SQL.People_Individual_Join_Household(churchId, houseHoldId, individualId);
                TestLog.WriteLine(string.Format("Add individual {0} to given household {1}", individualId, houseHoldId));
                #endregion Init Data

                // Login to Portal
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");
                test.Portal.Ministry_Activities_Schedules_Add_OneTime(ministryName, activityName, scheduleName, start_Time, end_Time, start_Date);

                #region go to the post attendance page.
                // Post Attendance for an individual, searching for an individual using advanced search.
                TestLog.WriteLine("Step 2: go to the post attendance page.");
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Attendance.Post_Attendance);

                //Select Ministry to post attendance for
                string selectMinistry = test.Driver.FindElementById(GeneralMinistry.PostAttendance.MinistryName).Text;
                if (selectMinistry != ministryName)
                {
                    test.Driver.FindElementByLinkText("Change").Click();
                    new SelectElement(test.Driver.FindElementById(GeneralMinistry.PostAttendance.Ministry_DropDown)).SelectByText(ministryName);
                    test.GeneralMethods.WaitForElementEnabled(By.Id(GeneralMinistry.PostAttendance.Activity_DropDown));
                }
                //Select Activity 
                new SelectElement(test.Driver.FindElementById(GeneralMinistry.PostAttendance.Activity_DropDown)).SelectByText(activityName);
                test.GeneralMethods.WaitForElementEnabled(By.Id(GeneralMinistry.PostAttendance.Schedule_DropDown));
                //Select Schedule
                new SelectElement(test.Driver.FindElementById(GeneralMinistry.PostAttendance.Schedule_DropDown)).SelectByText(scheduleName);
                test.GeneralMethods.WaitForElementEnabled(By.Id(GeneralMinistry.PostAttendance.Time_DropDown));
                //Select DateTime
                new SelectElement(test.Driver.FindElementById(GeneralMinistry.PostAttendance.Time_DropDown)).SelectByIndex(1);
                //Search
                test.Driver.FindElementById(GeneralMinistry.PostAttendance.Search_Button).Click();
                test.GeneralMethods.WaitForElement(By.LinkText("Post attendance"));

                //Go to Post Attendance screen
                test.Driver.FindElementByLinkText("Post attendance").Click();
                test.GeneralMethods.WaitForElement(By.Id(GeneralMinistry.PostAttendance.SearchAll_Radio));
                #endregion go to the post attendance page.

                #region check data.
                TestLog.WriteLine("Step 3: Check data xss attack.");
                bool isXssAttack;
                //find element & active.[Check xss for first name].
                test.SQL.People_Individual_UpdateNameByID(churchId, individualId, "XSSFirstName", "<script>alert('LN');</script>");
                isXssAttack = this.Ministry_Attendance_PostAttendance_FindPeople_ByName_CheckData_SecurityXSS(test, "LN", "XSSFirstName");
                Assert.IsFalse(isXssAttack, "'FirstName' is vulnerable to xss attack!");

                test.SQL.People_Individual_UpdateNameByID(churchId, individualId, "<script>alert('FN');</script>", "XSSLastName");
                isXssAttack = this.Ministry_Attendance_PostAttendance_FindPeople_ByName_CheckData_SecurityXSS(test, "FN", "XSSLastName");
                Assert.IsFalse(isXssAttack, "'LastName' is vulnerable to xss attack!");
                #endregion check data.

            }
            finally
            {
                
                //Clear add information.
                TestLog.WriteLine("Step 4: Clear add information.");
                TestLog.WriteLine(String.Format("Clean up to remove individual {0} from household {1}", individualId, houseHoldId));
                this.SQL.People_DeleteIndividualFromHousehold(churchId, individualId, houseHoldId);

                TestLog.WriteLine("Clean up to remove individual from DB, id: " + individualId);
                this.SQL.People_DeleteIndividual(churchId, individualId);
                
                //Clear basic information.
                test.SQL.Ministry_ActivitySchedules_Delete(churchId, scheduleName);
                //test.Portal.Ministry_Activities_Schedules_Delete_Schedule_ByName(ministryName, activityName, scheduleName);
                test.SQL.People_MergeIndividual(churchId, security_Name, "Merge Dump");
            }

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Jim Jin")]
        [Description("FO-1561 INC2270245 - Advanced Search in Post Attendance error")]
        public void Ministry_Attendance_PostAttendance_AdvancedSearch_MoreThanOnce()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

            #region Test data parameters
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";
            string scheduleName = utility.GetUniqueName("TestSch");
            #endregion

            try
            {
                #region Test data SQL Preparation
                test.SQL.Ministry_ActivitySchedules_Create(15, activityName, scheduleName, now.AddDays(-30), now.AddDays(30));
                int activityId = test.SQL.Ministry_Activities_FetchID(15, activityName);
                int scheduleId = test.SQL.Ministry_ActivitySchedules_FetchID(15, scheduleName);
                #endregion

                #region Add a test Recurrence
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");
                test.Portal.Ministry_Activities_Schedules_Add_Daily_Recurrence(activityId, scheduleId, now.AddDays(-30));
                #endregion

                #region Test operations and assertions
                
                utility.Navigate_Portal(Navigation.Ministry.Attendance.Post_Attendance);
                
                //Select Ministry
                string selectMinistry = utility.WaitAndGetElement(By.Id(GeneralMinistry.PostAttendance.MinistryName)).Text;
                if (selectMinistry != ministryName)
                {
                    utility.WaitAndGetElement(By.Id("active_ministry_toggle")).Click();
                    new SelectElement(utility.WaitAndGetElement(By.Id(GeneralMinistry.PostAttendance.Ministry_DropDown))).SelectByText(ministryName);
                }

                //select activity
                new SelectElement(utility.WaitAndGetElement(By.Id(GeneralMinistry.PostAttendance.Activity_DropDown))).SelectByText(activityName);

                //select schedule
                utility.waitForElementIsEnabled("document.getElementById('ctl00_ctl00_MainContent_content_ddlSchedule_dropDownList')");
                new SelectElement(utility.WaitAndGetElement(By.Id(GeneralMinistry.PostAttendance.Schedule_DropDown))).SelectByText(scheduleName);

                //select time
                utility.waitForElementIsEnabled("document.getElementById('ctl00_ctl00_MainContent_content_ddlActivityTime_dropDownList')");
                new SelectElement(utility.WaitAndGetElement(By.Id("ctl00_ctl00_MainContent_content_ddlActivityTime_dropDownList"))).SelectByIndex(1);

                //click search
                utility.WaitAndGetElement(By.Id("ctl00_ctl00_MainContent_content_btnSearch")).Click();
                
                //post attendance for first record
                utility.WaitAndGetElement(By.Id("ctl00_ctl00_MainContent_content_grdAttendanceHistory_ctl02_lnkPostAttendance")).Click();
                
                //advanced search with name="jack"
                utility.WaitAndGetElement(By.XPath("//a[@href='#advanced_search']")).Click();
                utility.WaitAndGetElement(By.Id("search")).SendKeys("jack");
                utility.WaitAndGetElement(By.Id("ctl00_ctl00_MainContent_content_btnSearch")).Click();

                //post attendance for all records of search result
                utility.WaitAndGetElement(By.Id("search_results_table")).FindElement(By.XPath("//input[@type='checkbox' and @value='none']")).Click();
                utility.WaitAndGetElement(By.Id("post_attendance")).Click();

                //advanced search with name="rose"
                utility.WaitAndGetElement(By.Id("search")).Clear();
                utility.WaitAndGetElement(By.Id("search")).SendKeys("rose");
                utility.WaitAndGetElement(By.Id("ctl00_ctl00_MainContent_content_btnSearch")).Click();

                //search result shoule be correct, instead of always shown "no result found"
                Assert.IsFalse(utility.WaitAndGetElement(By.Id("data_handler")).Text.Contains("Sorry, no results found."));

                //logout
                test.Portal.LogoutWebDriver();
                #endregion
            }
            finally
            {
                // Clear data and 
                test.SQL.Ministry_ActivitySchedules_Delete(15, scheduleName);
            }       
        }

        private bool Ministry_Attendance_PostAttendance_FindPeople_ByName_CheckData_SecurityXSS(TestBaseWebDriver test, string alertText, string sendKey)
        {
            bool isXssAttack = false;
            var txtBasicSearch = test.Driver.FindElement(By.Id("ctl00_ctl00_MainContent_content_basicsearch"));
            var btnBasicSearch = test.Driver.FindElement(By.Id("ctl00_ctl00_MainContent_content_btnSearch"));
            txtBasicSearch.Clear();
            txtBasicSearch.SendKeys(sendKey);
            btnBasicSearch.Click();

            //check data.
            try
            {
                IAlert alert = test.Driver.SwitchTo().Alert();
                if (alert != null && alert.Text == alertText)
                {   
                    //XSS Attack.
                    isXssAttack = true;
                    alert.Accept();
                }
            }
            catch (NoAlertPresentException)
            {
                isXssAttack = false;
                TestLog.WriteLine("No Alert here!");
            }
            
            return isXssAttack;
        }
    }
}