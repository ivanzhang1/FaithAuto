using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using System.Collections.Generic;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace FTTests.Portal.Ministry
{
    [TestFixture]
    public class Portal_Ministry_Activities_WebDriver : FixtureBaseWebDriver
    {
        #region Data
        // Data
        string ministryName = "A Test Ministry";
        string activityName = "A Test Activity";
        #endregion Data

        #region Fixture Setup

        [FixtureSetUp]
        public void FixtureSetUp()
        {
            // Delete all activities under the Auto Test Ministry
            base.SQL.Ministry_Activities_DeleteAll(15, "Auto Test Ministry"); 
        }

        #endregion Fixture Setup

        #region Fixture Tear Down
        [FixtureTearDown]
        public void FixtureTearDown()
        {
            // Delete all activities under the Filter Test Ministry
            //base.SQL.Ministry_Activities_DeleteAll(15, "Filter Test Ministry");
        }

        #endregion Fixture Tear Down 

        #region ActivitiesNew

        #region View All

        [Test, RepeatOnFailure, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Stuart Platt")]
        [Description("F1-2156:Verifies you can hit the new Activities/View All page.")]
        public void Ministry_Activities_ViewAll()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Activities/View All page
            test.Portal.Ministry_Activities_View_All_WebDriver();

            //Verify show more Toggle is functioning properly
            Assert.AreEqual("HIDE SIDEBAR", test.Driver.FindElementByCssSelector("[style=''][data-hide-on-expand='yes']").Text, "Hide Sidebar not displayed");

            //Verfiy Move Activities and Clone Activities
            //Disabled for now
            //Assert.IsTrue(test.Driver.FindElementByLinkText("Move Activities").Enabled, "Move Activities is not disabled");
            //Assert.IsTrue(test.Driver.FindElementByLinkText("Clone Activities").Enabled, "Clone Activities is not disabled");
            

            //Verify Grid Headers
            IWebElement activitiesTable = test.Driver.FindElementByXPath(TableIds.Ministry_Activities_ViewAll);
            Assert.AreEqual("Name", activitiesTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[0].Text, "Activities Header Mismatch");
            Assert.AreEqual("Ministry", activitiesTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[1].Text, "Activities Header Mismatch");
            //Assert.AreEqual("…", activitiesTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[6].Text, "Activities Header Mismatch");
            Assert.AreEqual("Assigned", activitiesTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[2].Text, "Activities Header Mismatch");
            Assert.AreEqual("Last Att", activitiesTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[3].Text, "Activities Header Mismatch");
            //Assert.AreEqual("Occurs Next", activitiesTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[4].Text, "Activities Header Mismatch");

            //Verify Add an activity link
            test.GeneralMethods.VerifyTextPresentWebDriver("Actions");
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id(GeneralMinistry.Activities.Add_Activity));
            //test.GeneralMethods.VerifyTextPresentWebDriver("Add activity");
            //Assert.IsTrue(test.Driver.FindElementByLinkText(GeneralMinistry.Activities.Add_Link).Displayed, "Add an activity link missing");
            
            //Verify Reset and Apply buttons
            test.GeneralMethods.VerifyTextPresentWebDriver("Filters");
            Assert.IsTrue(test.Driver.FindElementByLinkText(GeneralMinistry.Activities.Reset_Link).Displayed, "Reset link is not displayed");
            Assert.IsTrue(test.Driver.FindElementByXPath(GeneralMinistry.Activities.Apply_Button).Displayed, "General button is not displayed");

            //Verify Activity Name
            test.GeneralMethods.VerifyTextPresentWebDriver("Activity name");
            Assert.IsTrue(test.Driver.FindElementById(GeneralMinistry.Activities.Activity_Name).Displayed, "Activity input is not displayed");

            //Verify Ministry
            test.GeneralMethods.VerifyTextPresentWebDriver("Ministry");
            Assert.IsTrue(test.Driver.FindElementById(GeneralMinistry.Activities.Ministry_DropDown).Displayed, "Activity input is not displayed");
            Assert.AreEqual("---", new SelectElement(test.Driver.FindElementById(GeneralMinistry.Activities.Ministry_DropDown)).SelectedOption.Text.Trim(), "Default --- not selected option");

            //Verify Activity Type label & input
            Assert.IsTrue(test.Driver.FindElementByXPath(GeneralMinistry.Activities.Activity_Type_Filter).GetAttribute("class").Equals(GeneralMinistry.Activities.UI_Collapsed), "Activity Type is expanded");
            test.Driver.FindElementByXPath(GeneralMinistry.Activities.Activity_Type_Filter).Click();
            Assert.IsTrue(test.Driver.FindElementByXPath(GeneralMinistry.Activities.Activity_Type_Filter).GetAttribute("class").Contains(GeneralMinistry.Activities.UI_Expanded), "Activity Type is not expanded");
            Assert.AreEqual("---", new SelectElement(test.Driver.FindElementByXPath(GeneralMinistry.Activities.Activity_Type_DropDown)).SelectedOption.Text.Trim(), "Default --- not selected option");

            //Commenting out Filters that were removed in the Activities-Beta

            //Verify Occurance 
            //Assert.IsTrue(test.Driver.FindElementByXPath(GeneralMinistry.Activities.Occurence).GetAttribute("class").Equals(GeneralMinistry.Activities.UI_Collapsed), "Occurance is expanded");
            //test.Driver.FindElementByXPath(GeneralMinistry.Activities.Occurence).Click();
            //Assert.IsTrue(test.Driver.FindElementByXPath(GeneralMinistry.Activities.Occurence).GetAttribute("class").Contains(GeneralMinistry.Activities.UI_Expanded), "Occurance is not expanded");
            //Assert.IsFalse(test.Driver.FindElementById(GeneralMinistry.Activities.Sun_CheckBox).Selected, "Sunday is enabled");
            //Assert.IsFalse(test.Driver.FindElementById(GeneralMinistry.Activities.Mon_CheckBox).Selected, "Monday is enabled");
            //Assert.IsFalse(test.Driver.FindElementById(GeneralMinistry.Activities.Tue_CheckBox).Selected, "Tuesday is enabled");
            //Assert.IsFalse(test.Driver.FindElementById(GeneralMinistry.Activities.Wed_CheckBox).Selected, "Wednesday is enabled");
            //Assert.IsFalse(test.Driver.FindElementById(GeneralMinistry.Activities.Thu_CheckBox).Selected, "Thursday is enabled");
            //Assert.IsFalse(test.Driver.FindElementById(GeneralMinistry.Activities.Fri_CheckBox).Selected, "Friday is enabled");
            //Assert.IsFalse(test.Driver.FindElementById(GeneralMinistry.Activities.Sat_CheckBox).Selected, "Saturday is enabled");
            ////Assert.AreEqual("Choose a date range...", new SelectElement(test.Driver.FindElementByXPath(GeneralMinistry.Activities.DateRange_DropDown)).SelectedOption.Text.Trim());
            //Assert.IsTrue(test.Driver.FindElementById(GeneralMinistry.Activities.DateFrom).Displayed, "Date From not displayed");
            //Assert.AreEqual("", test.Driver.FindElementById(GeneralMinistry.Activities.DateFrom).Text, "Default not set to empty");
            //Assert.IsTrue(test.Driver.FindElementById(GeneralMinistry.Activities.DateTo).Displayed, "Date To not displayed");
            //Assert.AreEqual("", test.Driver.FindElementById(GeneralMinistry.Activities.DateTo).Text, "Default not set to empty");

            ////Verify Age Range
            //Assert.IsTrue(test.Driver.FindElementByXPath(GeneralMinistry.Activities.Age_Range).GetAttribute("class").Equals(GeneralMinistry.Activities.UI_Collapsed), "Age Range is expanded");
            //test.Driver.FindElementByXPath(GeneralMinistry.Activities.Age_Range).Click();
            //Assert.IsTrue(test.Driver.FindElementByXPath(GeneralMinistry.Activities.Age_Range).GetAttribute("class").Equals(GeneralMinistry.Activities.UI_Expanded), "Age Range is not expanded");
            //Assert.AreEqual("---", new SelectElement(test.Driver.FindElementById(GeneralMinistry.Activities.Age_Range_Min_DropDown)).SelectedOption.Text.Trim(), "Age Range Min Default --- not selected option");
            //Assert.AreEqual("---", new SelectElement(test.Driver.FindElementById(GeneralMinistry.Activities.Age_Range_Max_DropDown)).SelectedOption.Text.Trim(), "Age Range Max Default --- not selected option");

            ////Verify Location
            //Assert.IsTrue(test.Driver.FindElementByXPath(GeneralMinistry.Activities.Location).GetAttribute("class").Equals(GeneralMinistry.Activities.UI_Collapsed), "Location is expanded");
            //test.Driver.FindElementByXPath(GeneralMinistry.Activities.Location).Click();
            //Assert.IsTrue(test.Driver.FindElementByXPath(GeneralMinistry.Activities.Location).GetAttribute("class").Equals(GeneralMinistry.Activities.UI_Expanded), "Location is not expanded");
            //Assert.AreEqual("---", new SelectElement(test.Driver.FindElementByXPath(GeneralMinistry.Activities.Buildings_DropDown)).SelectedOption.Text.Trim(), "Buildings Default --- not selected option");
            //Assert.AreEqual("---", new SelectElement(test.Driver.FindElementByXPath(GeneralMinistry.Activities.Rooms_DropDown)).SelectedOption.Text.Trim(), "Rooms Default --- not selected option");

            ////Verify Confidential
            //Assert.IsTrue(test.Driver.FindElementByXPath(GeneralMinistry.Activities.Confidential).GetAttribute("class").Equals(GeneralMinistry.Activities.UI_Collapsed), "Confidential is expanded");
            //test.Driver.FindElementByXPath(GeneralMinistry.Activities.Confidential).Click();
            //Assert.IsTrue(test.Driver.FindElementByXPath(GeneralMinistry.Activities.Confidential).GetAttribute("class").Equals(GeneralMinistry.Activities.UI_Expanded), "Confidential is not expanded");
            //Assert.IsFalse(test.Driver.FindElementById(GeneralMinistry.Activities.Confidential_Checkbox).Selected, "Confidential Checkbox is enabled");
            //Assert.IsFalse(test.Driver.FindElementById(GeneralMinistry.Activities.Normal_Checkbox).Selected, "Normal Checkbox is enabled");

            //Verify Check-in Enabled
            //Felix approved the Sleeps
            System.Threading.Thread.Sleep(3000);
            Assert.IsTrue(test.Driver.FindElementByXPath(GeneralMinistry.Activities.Check_In_Enabled).GetAttribute("class").Equals(GeneralMinistry.Activities.UI_Collapsed), "Check In Enabled is expanded");
            test.Driver.FindElementByXPath(GeneralMinistry.Activities.Check_In_Enabled).Click();
            System.Threading.Thread.Sleep(3000);
            Assert.IsTrue(test.Driver.FindElementByXPath(GeneralMinistry.Activities.Check_In_Enabled).GetAttribute("class").Equals(GeneralMinistry.Activities.UI_Expanded), "Check In Enabled is not expanded");
            Assert.IsFalse(test.Driver.FindElementById(GeneralMinistry.Activities.Check_In_Enabled_CheckBox).Selected, "Check In Checkbox is enabled");
            Assert.IsFalse(test.Driver.FindElementById(GeneralMinistry.Activities.Check_In_NotEnabled_CheckBox).Selected, "Not Check In Checkbox is enabled");

            //Verify Active is checkbox default
            Assert.IsTrue(test.Driver.FindElementByXPath(GeneralMinistry.Activities.Active).GetAttribute("class").Equals(GeneralMinistry.Activities.UI_Expanded), "Active UI is not expanded");
            Assert.IsTrue(test.Driver.FindElementById(GeneralMinistry.Activities.Active_CheckBox).Selected, "Active Check Box should be selected by default");
            Assert.IsFalse(test.Driver.FindElementById(GeneralMinistry.Activities.Inactive_CheckBox).Selected, "Inactive Check Box should not be selected by default");
            test.Driver.FindElementByXPath(GeneralMinistry.Activities.Active).Click();
            Assert.IsTrue(test.Driver.FindElementByXPath(GeneralMinistry.Activities.Active).GetAttribute("class").Equals(GeneralMinistry.Activities.UI_Collapsed), "Active UI is expanded");

            //Verify more button
            Assert.IsTrue(test.Driver.FindElementById(GeneralMinistry.Activities.More_Button).Displayed, "More button is not displayed");

            //Verify that default rows are only 20
            //Autoscroll changed this behavior therefore we are getting the count and what shows up shows up
            //Commenting this and having the check below
            /*if (test.Configuration.AppSettings.Settings["FTTests.Host"].Value == "localhost")
            {                
                Assert.AreEqual(20, tblRows, "Default Displayed rows of 20 does not match");
            }
            else
            {                
                Assert.AreEqual(21, tblRows, "Default Displayed rows of 21 does not match");
            }*/

            //Verify grid record count default 1-20
            //Autoscroll changed behavior, so verify what rows are displayed that the text matches
            //We subtract one to accomodate for header.
            test.GeneralMethods.TakeScreenShot_WebDriver();
            int tblRows = tblRows = activitiesTable.FindElements(By.TagName("tr")).Count - 1;

            //If loading indicator is visible then rows will be higher so subtract one again
            if (test.GeneralMethods.IsElementVisibleWebDriver(By.Id("load_bar")))
            {  tblRows--; }
            Assert.Contains(test.Driver.FindElementByXPath("//div[@class='grid_record_count float_right']").Text, string.Format("Viewing 1 − {0} of ", tblRows), string.Format("Displayed rows of {0} do not match", tblRows));


            // Logout of portal
            test.Portal.LogoutWebDriver();
         
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("F1-2156:Verifies you toggle the side bar on the new Activities/View All page.")]
        public void Ministry_Activities_ViewAll_SideBar_Collapse()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to the Activities/View All page
            test.Portal.Ministry_Activities_View_All_WebDriver();

            //Verify show more Toggle is functioning properly
            Assert.AreEqual("HIDE SIDEBAR", test.Driver.FindElementByCssSelector("[style=''][data-hide-on-expand='yes']").Text, "Hide Sidebar not displayed");
            test.Driver.FindElementById("expand_collapse").Click();
            Assert.AreEqual("SHOW SIDEBAR", test.Driver.FindElementByCssSelector("[style='display: inline;'][data-show-on-expand='yes']").Text, "Show Sidebar not displayed");

            //Verify table is expanded
            IWebElement activitiesTable = test.Driver.FindElementByXPath(TableIds.Ministry_Activities_ViewAll);
            Assert.AreEqual("Name", activitiesTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[0].Text, "Activities Header Mismatch");
            Assert.AreEqual("Ministry", activitiesTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[1].Text, "Activities Header Mismatch");
            Assert.AreEqual("Assigned", activitiesTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[2].Text, "Activities Header Mismatch");
            //Assert.AreEqual("Last Att", activitiesTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[3].Text, "Activities Header Mismatch");
            Assert.AreEqual("Last Attendance", activitiesTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[4].Text, "Activities Header Mismatch");
            Assert.AreEqual("Occurs Next", activitiesTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[5].Text, "Activities Header Mismatch");

            //Verify Actions side bar not visible.

            //Verify reToggle gets back to default view
            test.Driver.FindElementById("expand_collapse").Click();
            activitiesTable = test.Driver.FindElementByXPath(TableIds.Ministry_Activities_ViewAll);
            Assert.AreEqual("Name", activitiesTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[0].Text, "Activities Header Mismatch");
            Assert.AreEqual("Ministry", activitiesTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[1].Text, "Activities Header Mismatch");
            //Assert.AreEqual("…", activitiesTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[1].Text, "Activities Header Mismatch");
            Assert.AreEqual("Assigned", activitiesTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[2].Text, "Activities Header Mismatch");
            Assert.AreEqual("Last Att", activitiesTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[3].Text, "Activities Header Mismatch");
            Assert.AreEqual("Occurs Next", activitiesTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[5].Text, "Activities Header Mismatch");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verify Activity Assignment/Attendance Counts")]
        public void Ministry_Activities_ViewAll_VerifyCounts()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Setup
            //int churchID = 15;
            string activityName = "A Test Activity";
            //int individualID = 0;
            //test.SQL.Ministry_ActivityDetails_Create(churchID, activityName, "Activity Room");
            //test.SQL.Ministry_StaffingAssignments_Create(churchID, individualID, 0, activityName, "Base Schedule");

            // Navigate to the Activities/View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Ministry.Activities.View_All);

            //Get count 
            int activityID = test.SQL.Ministry_Activities_FetchID(15, activityName);
            string[] participantStaffCount = test.SQL.Ministry_Assignments_GetAssignmentCountForActivities(15, activityID);
            string attendanceCount = test.SQL.Ministry_Assignments_GetAttendanceForLastActivityDay(15, activityID);

            TestLog.WriteLine("Participants: {0}", participantStaffCount[0]);
            TestLog.WriteLine("Staff: {0}", participantStaffCount[1]);
            TestLog.WriteLine("Attendance: {0}", attendanceCount);

            //Find the Activity Row
            int itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Ministry_Activities_ViewAll, activityName, "Name", null);
            IWebElement table = test.Driver.FindElementByXPath(TableIds.Ministry_Activities_ViewAll);
            TestLog.WriteLine("Name: {0}", table.FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[0].Text);

            //Verify Staff Count
            TestLog.WriteLine("Staff: {0}", table.FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[2].Text);
            Assert.AreEqual(participantStaffCount[1], table.FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[2].Text, "Staff count does not match");

            //Verify Participant count
            TestLog.WriteLine("Participants: {0}", table.FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[3].Text);
            Assert.AreEqual(participantStaffCount[0], table.FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[3].Text, "Participant count does not match");

            //Verify Attendance count
            TestLog.WriteLine("Attendance: {0}", table.FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[4].Text);
            Assert.AreEqual(attendanceCount, table.FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[4].Text, "Attendance does not match");

            //Data Cleanup
            //test.SQL.Ministry_StaffingAssignments_Delete(churchID, individualID, activityName, "Base Schedule");
            //test.SQL.Ministry_ActivityDetails_Delete(churchID, activityName, "Activity Room");

            //Logout
            test.Portal.LogoutWebDriver();
        }

        #region Filter

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Stuart Platt")]
        [Description("F1-3619:Tests the Name search filter.")]
        public void Ministry_Activities_ViewAll_FilterBy_Name()
        {

            //string activityName = "A Test Activity";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to the Activities/View All page
            test.Portal.Ministry_Activities_View_All_WebDriver();

            //Enter Search filter functions properly
            test.Portal.Ministry_Activities_Filter_Activity_Name(activityName);

            //Verify Search returned activity name and that it's a link
            IWebElement activitiesTable = test.Driver.FindElementByXPath(TableIds.Ministry_Activities_ViewAll);
            int tblRows = activitiesTable.FindElements(By.TagName("tr")).Count - 1;
            TestLog.WriteLine("Rows Count: {0}", tblRows);
            int row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Ministry_Activities_ViewAll, activityName, "Name");
            Assert.AreEqual(activityName, activitiesTable.FindElements(By.TagName("tr"))[row].FindElements(By.TagName("td"))[0].Text, "Activities Name Not Found");
            Assert.IsTrue(activitiesTable.FindElements(By.TagName("tr"))[row].FindElements(By.TagName("td"))[0].FindElement(By.LinkText(activityName)).Displayed, "Activity Name is not a link");

            //Verify that Rows count displayed correctly
            Assert.Contains(test.Driver.FindElementByXPath("//div[@class='grid_record_count float_right']").Text, string.Format("Viewing 1 − {0} of {0}", tblRows));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies the Ministry Filter functions properly")]
        public void Ministry_Activities_ViewAll_FilterBy_Ministry()
        {

            //string activityName = "A Test Activity";
            //string ministry = "A Test Ministry";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to the Activities/View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Ministry.Activities.View_All);

            //Search Filter functions properly
            List<string> ministries = new List<string>();
            ministries.Add(ministryName);
            test.Portal.Ministry_Activities_Filter_Ministry(ministries);

            //Verify Search Results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Ministry_Activities_ViewAll, activityName, "Name"), "A Test Activity was not returned");

            //Verify that all Search Results only returned "A Test Ministry" activities
            test.Driver.FindElementById("expand_collapse").Click();
            int rows = test.GeneralMethods.GetTableRowCountWebDriver(TableIds.Ministry_Activities_ViewAll);
            IWebElement activitiesTable = test.Driver.FindElementByXPath(TableIds.Ministry_Activities_ViewAll);
            for (int r = 2; r < rows; r++)
            {
                Assert.AreEqual(ministryName, activitiesTable.FindElements(By.TagName("tr"))[r].FindElements(By.TagName("td"))[1].Text, string.Format("Ministry Mismatch in row [{0}]", r));

            }

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }
        [Test, RepeatOnFailure]
        [Author("Daniel Lee")]
        [Description("F1-5682: Verifies Staffing Assignments Are Appearing Under Ministry > Activities (New) without Filtering By Ministry")]
        public void Ministry_Activities_ViewAll_Activity_StaffAssignments_Appear_WithNoFilterApplied()
        {

            //string activityName = "A Test Activity";
            //string ministry = "A Test Ministry";
            
            string activity = "";
            string staffingCount = "";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to the Activities/View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Ministry.Activities.View_All);
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("load_button"));
            test.Driver.FindElementById("load_button").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("load_button"));

            //Activity List to check
            int rows = test.GeneralMethods.GetTableRowCountWebDriver(TableIds.Ministry_Activities_ViewAll);
            IWebElement activitiesTable = test.Driver.FindElementByXPath(TableIds.Ministry_Activities_ViewAll);
            try
            {
                for (int r = 1; r < rows; r++)
                {
                    activity = activitiesTable.FindElements(By.TagName("tr"))[r].FindElements(By.TagName("td"))[0].Text.ToString();
                    staffingCount = activitiesTable.FindElements(By.TagName("tr"))[r].FindElements(By.TagName("td"))[2].Text.ToString();
                    //test.Portal.Ministry_Activities_View_All_AssignmentPill(15, activity, false);
                    int activityID = test.SQL.Ministry_Activities_FetchID(15, activity);
                    string[] assignmentCount = test.SQL.Ministry_Assignments_GetAssignmentCountForActivities(15, activityID);
                    Assert.AreEqual(assignmentCount[1], staffingCount);

                }
            }
            catch (Exception e)
            {
                TestLog.WriteLine("Error occurred!");
            }

        }

        //[Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("F1-3619:Tests the Occurence search filter.")]
        public void Ministry_Activities_ViewAll_FilterBy_Occurence()
        {

            DateTime currentTime;

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Activities/View All page
            test.Portal.Ministry_Activities_View_All_WebDriver();

            currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

            //Set Search filter functions properly
            //test.Portal.Ministry_Activities_Filter_Occurence(null, null, "01/01/2013", currentTime.ToString("MM/dd/yyyy"));

            //Verify Filter Results
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Ministry_Activities_ViewAll, "Assign Activity Level", "Name"), "Assign Activity Level not found");
            //test.Portal.Ministry_Activities_Filter_Verify_Occurance_Date("01/01/2013", currentTime.ToString("MM/dd/yyyy"));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        //[Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("F1-3619:Tests the Occurence search filter for Day of the week.")]
        public void Ministry_Activities_ViewAll_FilterBy_Occurence_DayoftheWeek()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Activities/View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Ministry.Activities.View_All);

            //Enter Search filter functions properly
            //test.Portal.Ministry_Activities_Filter_Occurence(new GeneralEnumerations.WeeklyScheduleDays[] { GeneralEnumerations.WeeklyScheduleDays.Friday }, "This Year", "", "", false);

            //Verify Date Range is updated to this year in dateFrom and dateTo input fields
            DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string year = currentTime.ToString("yyyy");
            //test.Driver.FindElementById(GeneralMinistry.Activities.DateFrom).SendKeys(Keys.Tab);
            //Assert.AreEqual(string.Format("01/01/{0}", year), test.Driver.FindElementById(GeneralMinistry.Activities.DateFrom).GetAttribute("value"), "Date From was not set correctly");
            //Assert.AreEqual(string.Format("12/31/{0}", year), test.Driver.FindElementById(GeneralMinistry.Activities.DateTo).GetAttribute("value"), "Date From was not set correctly");

            //Apply Filter
            test.Driver.FindElementByXPath(GeneralMinistry.Activities.Apply_Button).Click();

            //Verify that Filter Results
            //test.GeneralMethods.VerifyTextPresentWebDriver("Assign Activity Level");
            //test.Portal.Ministry_Activities_Filter_Verify_Occurance_Day_Of_Week(new GeneralEnumerations.WeeklyScheduleDays[] { GeneralEnumerations.WeeklyScheduleDays.Friday });
            //test.Portal.Ministry_Activities_Filter_Verify_Occurance_Date(string.Format("1/01/{0}", year), string.Format("12/31/{0}", year));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("F1-3619:Tests the ActivityType search filter.")]
        public void Ministry_Activities_ViewAll_FilterBy_ActivityType()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to the Activities/View All page
            test.Portal.Ministry_Activities_View_All_WebDriver();

            //Enter filter info
            List<string> activityTypes = new List<string>();
            activityTypes.Add("Activity Type Test");
            test.Portal.Ministry_Activities_Filter_Activity_Type(activityTypes);

            //Verify Filter results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Ministry_Activities_ViewAll, "Activity Type - Test", "Name"), "[Activity Type - Test] was not found within filter");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        //[Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("F1-3619:Tests the Age range filter for the Activity View all page")]
        public void Ministry_Activities_ViewAll_FilterBy_Age()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            //Navigate to the Activities/View All page 
            test.Portal.Ministry_Activities_View_All_WebDriver();

            //Enter search filter
            //test.Portal.Ministry_Activities_Filter_Age_Range("0", "99");

            //Verify Filter results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Ministry_Activities_ViewAll, "A Test Activity", "Name"), "[A Test Activity] was not found within filter");

            //Log out 
            test.Portal.LogoutWebDriver();
        }

        //[Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("F1-3619:Tests the Location filter building only")]
        public void Ministry_Activities_ViewAll_FilterBy_Location_Building_Only()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            //Navigate to the Activities/View All page 
            test.Portal.Ministry_Activities_View_All_WebDriver();

            //Verify search filter functions properly
            List<string> buildings = new List<string>();

            buildings.Add("Building I");

            //Filter By Location
            //test.Portal.Ministry_Activities_Filter_Location(buildings);

            TestLog.WriteLine("Submit Query");
            test.Driver.FindElementByXPath(GeneralMinistry.Activities.Apply_Button).Click();

            //Verify if result is there
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(TableIds.Ministry_Activities_ViewAll));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Ministry_Activities_ViewAll, "Activity 1", "Name"), "Activity 1 not found in results");
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Ministry_Activities_ViewAll, "Activity 2", "Name"), "Activity 2 not found in results");

            //Log out 
            test.Portal.LogoutWebDriver();
        }


        //[Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("F1-3619:Tests the Location filter building and room")]
        public void Ministry_Activities_ViewAll_FilterBy_Location()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            //Navigate to the Activities/View All page 
            test.Portal.Ministry_Activities_View_All_WebDriver();

            //Verify search filter functions properly
            List<string> buildings = new List<string>();
            List<string> rooms = new List<string>();

            buildings.Add("Building I");
            rooms.Add("Building I - Room 001");

            //Filter By Location
            //test.Portal.Ministry_Activities_Filter_Location(buildings, rooms);

            TestLog.WriteLine("Submit Query");
            test.Driver.FindElementByXPath(GeneralMinistry.Activities.Apply_Button).Click();

            //Verify if result is there
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(TableIds.Ministry_Activities_ViewAll));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Ministry_Activities_ViewAll, "Activity 1", "Name"), "Activity 1 not found in results");
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Ministry_Activities_ViewAll, "Activity 2", "Name"), "Activity 2 was found in results");

            //Log out 
            test.Portal.LogoutWebDriver();
        }

        //[Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("F1-3619: Verifies that the Confidnetial search feature is functioning properly.")]
        public void Ministry_Activities_ViewAll_FilterBy_Confidential()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to the Activities/View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Ministry.Activities.View_All);

            //Verify search filter functions properly
            //test.Portal.Ministry_Activities_Filter_Confidential(true);

            //Verify Filter results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Ministry_Activities_ViewAll, "Confidential Test", "Name"), "[Confidential Test] was not found within filter");

            //Log out 
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("F1-3619: Verifies that the Check_in Enabled search feature working properly.")]
        public void Ministry_Activities_ViewAll_FilterBy_CheckInEnabled()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to the Activities/View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Ministry.Activities.View_All);

            //Verify search filter functions properly
            test.Portal.Ministry_Activities_Filter_CheckIn_Enabled(true);

            //Verify Filter results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Ministry_Activities_ViewAll, "A Test Activity", "Name"), "[A Test Activity] was not found within filter");

            //Log out 
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the CheckIn Enabled Check-in code search function works properly")]
        public void Ministry_Activities_ViewAll_FilterBy_CheckInCode()
        {

            string code = string.Empty;

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to the Activities/View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Ministry.Activities.View_All);

            //Enter search filter info
            switch (this.F1Environment.ToString())
            {
                case "INT3":
                    code = "5944";
                    break;
                case "LV_QA":
                    code = "2761";
                    break;
                case "STAGING":
                    code = "1145";
                    break;
                case "LV_UAT":
                    code = "1145";
                    break;
                default:
                    code = "5111";
                    break;
            }

            test.Portal.Ministry_Activities_Filter_CheckIn_Enabled(true, code);
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Ministry_Activities_ViewAll, "A Test Activity", "Name"), "[A Test Activity] was not found within filter");

            //Log out 
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Check-in Enabled search feature also will limit activities without check-in")]
        public void Ministry_Activities_ViewAll_FilterBy_CheckInDisabled()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to the Activities/View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Ministry.Activities.View_All);

            //Enter search filter info
            test.Portal.Ministry_Activities_Filter_CheckIn_Enabled(false, "", true);

            //Verify Results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Ministry_Activities_ViewAll, "**no checkin", "Name"), "[**no checkin] was not found within filter");

            //Log out 
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Inactive search function works properly")]
        public void Ministry_Activities_ViewAll_FilterBy_Inactive()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to the Activities/View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Ministry.Activities.View_All);

            //Enter search filter info
            test.Portal.Ministry_Activities_Filter_Active(false, true);

            //Verify Results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Ministry_Activities_ViewAll, "*Inactive Test", "Name"), "[*Inactive Test] was not found within filter");

            //Log out 
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-4183: Verifies filters are saved when navigating away from the View All page and coming back")]
        public void Ministry_Activities_ViewAll_SavedFilters()
        {
            // Data
            string activityName = "A Test Activity";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to the Activities/View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Ministry.Activities.View_All);

            // Filter by activity
            test.Portal.Ministry_Activities_Filter_Activity_Name(activityName);

            // Click on an activity
            test.Driver.FindElementByLinkText(activityName).Click();
            test.GeneralMethods.WaitForElement(By.LinkText("View assignments"));

            // Click on the return arrow to go back to View All page
            test.Driver.FindElementById("tab_back").Click();
            test.GeneralMethods.WaitForElement(By.LinkText("Add activity"));

            // Verify activity filter retained its setting
            Assert.AreEqual(activityName, test.Driver.FindElementById(GeneralMinistry.Activities.Activity).GetAttribute("value"));

            // Click on the participant pills to navigate to Assignments View All page
            test.Driver.FindElementById(GeneralMinistry.Activities.Activity).Clear();
            test.Portal.Ministry_Activities_View_All_AssignmentPill(15, activityName, true);

            // Navigate back to Activies View All page
            test.Portal.Ministry_Activities_View_All_WebDriver();

            // Verify activity filter retained its setting
            Assert.AreEqual(activityName, test.Driver.FindElementById(GeneralMinistry.Activities.Activity).GetAttribute("value"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        #endregion Filter

        #region Assignment Pills

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Stuart Platt")]
        [Description("Verifies that the Assignment Pill on the Activities page navigates to the Assignments page")]
        public void Ministry_Activities_ViewAll_Assignment_Pill_Staff()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            //Navigate to the Activities Page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Activities.View_All);

            //Click and Verify Asignment Pill
            test.Portal.Ministry_Activities_View_All_AssignmentPill(15, "Assign Schedule Level", false);

            //Logout
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Assignment Pill on the Activities page navigates to the Assignments page")]
        public void Ministry_Activities_ViewAll_Assignment_Pill_Participant()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            //Navigate to the Activities Page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Activities.View_All);

            //Click and Verify Asignment Pill
            test.Portal.Ministry_Activities_View_All_AssignmentPill(15, "Assign Schedule Level", true);

            //Logout
            test.Portal.LogoutWebDriver();
        }

        #endregion Assignment Pills

        #endregion View All

        #region Add an Activity
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Creates a one time activity without check-in through the wizard.")]
        public void Ministry_Activities_ViewAll_AddAnActivity_Once_NoCheckin()
        {
            // Data
            string ministryName = "Auto Test Ministry";
            string activityName = "*One Time Activity";
            string scheduleName = "One Time Schedule";
            string rosterName = "One Time Roster";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Setup roster names
            System.Collections.Generic.List<string> roster = new System.Collections.Generic.List<string>();
            roster.Add(rosterName);

            // Create a one time activity without checkin
            test.Portal.Ministry_Activities_Create_Step1_WebDriver(ministryName, activityName);
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Ministry_Activities_Create_Step2_OneTime_WebDriver(15, scheduleName, "6:00 PM", "8:00 PM", today.AddDays(1).ToShortDateString());
            test.Portal.Ministry_Activities_Create_Step3_WebDriver(false, roster, string.Empty, string.Empty);

            // Verify activity was created
            test.Portal.Ministry_Activities_View_All_WebDriver();
            test.Portal.Ministry_Activities_Filter_Activity_Name(activityName, true);
            test.GeneralMethods.VerifyTextPresentWebDriver(activityName);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Creates a daily activity without check-in through the wizard.")]
        public void Ministry_Activities_ViewAll_AddAnActivity_Daily_NoCheckin()
        {
            // Data
            string ministryName = "Auto Test Ministry";
            string activityName = "*Daily Activity";
            string scheduleName = "Daily Schedule";
            string rosterName = "Daily Roster";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Setup roster names
            System.Collections.Generic.List<string> roster = new System.Collections.Generic.List<string>();
            roster.Add(rosterName);

            // Create a daily activity without checkin
            test.Portal.Ministry_Activities_Create_Step1_WebDriver(ministryName, activityName);
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Ministry_Activities_Create_Step2_Daily_WebDriver(15, scheduleName, "6:00 PM", "8:00 PM", "1", today.AddDays(1).ToShortDateString());
            test.Portal.Ministry_Activities_Create_Step3_WebDriver(false, roster, string.Empty, string.Empty);

            // Verify activity was created
            test.Portal.Ministry_Activities_View_All_WebDriver();
            test.Portal.Ministry_Activities_Filter_Activity_Name(activityName, true);
            test.GeneralMethods.VerifyTextPresentWebDriver(activityName);
            
            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Creates a weekly activity without check-in through the wizard.")]
        public void Ministry_Activities_ViewAll_AddAnActivity_Weekly_NoCheckin()
        {
            // Data
            string ministryName = "Auto Test Ministry";
            string activityName = "*Weekly Activity";
            string scheduleName = "Weekly Schedule";
            string rosterName = "Weekly Roster";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Setup days to meet
            System.Collections.Generic.List<GeneralEnumerations.WeeklyScheduleDays> daysOfWeek = new System.Collections.Generic.List<GeneralEnumerations.WeeklyScheduleDays>();
            daysOfWeek.Add(GeneralEnumerations.WeeklyScheduleDays.Sunday);
            // daysOfWeek.Add(GeneralEnumerations.WeeklyScheduleDays.Wednesday);

            // Setup roster names
            System.Collections.Generic.List<string> roster = new System.Collections.Generic.List<string>();
            roster.Add(rosterName);

            // Create a weekly activity without checkin
            test.Portal.Ministry_Activities_Create_Step1_WebDriver(ministryName, activityName);
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Ministry_Activities_Create_Step2_Weekly_WebDriver(15, scheduleName, "6:00 PM", "8:00 PM", "1", daysOfWeek, today.AddDays(1).ToShortDateString());
            test.Portal.Ministry_Activities_Create_Step3_WebDriver(false, roster, string.Empty, string.Empty);

            // Verify activity was created
            test.Portal.Ministry_Activities_View_All_WebDriver();
            test.Portal.Ministry_Activities_Filter_Activity_Name(activityName, true);
            test.GeneralMethods.VerifyTextPresentWebDriver(activityName);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Creates a monthly activity without check-in through the wizard.")]
        public void Ministry_Activities_ViewAll_AddAnActivity_Monthly_NoCheckin()
        {
            // Data
            string ministryName = "Auto Test Ministry";
            string activityName = "*Monthly Activity";
            string scheduleName = "Monthly Activity";
            string rosterName = "Monthly Roster";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Setup roster names
            System.Collections.Generic.List<string> roster = new System.Collections.Generic.List<string>();
            roster.Add(rosterName);

            // Create a monthly activity through the new activity wizard
            test.Portal.Ministry_Activities_Create_Step1_WebDriver(ministryName, activityName);
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Ministry_Activities_Create_Step2_Monthly_WebDriver(15, scheduleName, "6:00 PM", "8:00 PM", "1", "15th", today.AddDays(1).ToShortDateString());
            test.Portal.Ministry_Activities_Create_Step3_WebDriver(false, roster, string.Empty, string.Empty);

            // Verify activity was created
            test.Portal.Ministry_Activities_View_All_WebDriver();
            test.Portal.Ministry_Activities_Filter_Activity_Name(activityName, true);
            test.GeneralMethods.VerifyTextPresentWebDriver(activityName);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Creates a yearly activity without check-in through the wizard.")]
        public void Ministry_Activities_ViewAll_AddAnActivity_Yearly_NoCheckin()
        {
            // Data
            string ministryName = "Auto Test Ministry";
            string activityName = "*Yearly Activity";
            string scheduleName = "Yearly Activity";
            string rosterName = "Yearly Roster";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Setup roster names
            System.Collections.Generic.List<string> roster = new System.Collections.Generic.List<string>();
            roster.Add(rosterName);

            // Create a yearly activity through the new activity wizard
            test.Portal.Ministry_Activities_Create_Step1_WebDriver(ministryName, activityName);
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Ministry_Activities_Create_Step2_Yearly_WebDriver(15, scheduleName, "6:00 PM", "8:00 PM", today.AddDays(1).ToShortDateString());
            test.Portal.Ministry_Activities_Create_Step3_WebDriver(false, roster, string.Empty, string.Empty);

            // Verify activity was created
            test.Portal.Ministry_Activities_View_All_WebDriver();
            test.Portal.Ministry_Activities_Filter_Activity_Name(activityName, true);
            test.GeneralMethods.VerifyTextPresentWebDriver(activityName);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Creates a one time activity with check-in through the wizard.")]
        public void Ministry_Activities_ViewAll_AddAnActivity_Once_WithCheckin()
        {
            // Data
            string ministryName = "Auto Test Ministry";
            string activityName = "*One Time w/ Checkin";
            string scheduleName = "One Time w/ Checkin";
            string rosterName = "One Time w/ Check-in";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Setup roster names
            System.Collections.Generic.List<string> roster = new System.Collections.Generic.List<string>();
            roster.Add(rosterName);

            // Create a one time activity with check-in through the activity wizard
            test.Portal.Ministry_Activities_Create_Step1_WebDriver(ministryName, activityName, string.Empty, false, "---", true, true, true);
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Ministry_Activities_Create_Step2_OneTime_WebDriver(15, scheduleName, "6:00 PM", "8:00 PM", today.AddDays(1).ToShortDateString());
            test.Portal.Ministry_Activities_Create_Step3_WebDriver(true, roster, string.Empty, string.Empty, true, true);

            // Verify activity was created
            test.Portal.Ministry_Activities_View_All_WebDriver();
            test.Portal.Ministry_Activities_Filter_Activity_Name(activityName, true);
            test.GeneralMethods.VerifyTextPresentWebDriver(activityName);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Creates an activity with check-in and an age range through the wizard.")]
        public void Ministry_Activities_ViewAll_AddAnActivity_Once_WithCheckin_AgeRange()
        {
            // Data
            string ministryName = "Auto Test Ministry";
            string activityName = "*One Time w/ Age";
            string scheduleName = "One Time w/ Age";
            string rosterName = "One Time w/ Age";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Setup roster names
            System.Collections.Generic.List<string> roster = new System.Collections.Generic.List<string>();
            roster.Add(rosterName);

            // Create a one time activity with check-in through the activity wizard
            test.Portal.Ministry_Activities_Create_Step1_WebDriver(ministryName, activityName, string.Empty, false, "---", true, true, true, false, false, GeneralEnumerations.ActivitiesRequireAssignment.None, true, "18", "40");
            test.GeneralMethods.WaitForElement(By.XPath("//table[@class='normalize']/tbody/tr[3]/td"));
            Assert.AreEqual("18 to 40 years", test.Driver.FindElement(By.XPath("//table[@class='normalize']/tbody/tr[3]/td")).Text, "The age restriction display error");

            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Ministry_Activities_Create_Step2_OneTime_WebDriver(15, scheduleName, "6:00 PM", "8:00 PM", today.AddDays(1).ToShortDateString());
            test.GeneralMethods.WaitForElement(By.XPath("//table[@class='normalize']/tbody/tr[3]/td"));
            Assert.AreEqual("18 to 40 years", test.Driver.FindElement(By.XPath("//table[@class='normalize']/tbody/tr[3]/td")).Text, "The age restriction display error");

            test.Portal.Ministry_Activities_Create_Step3_WebDriver(true, roster, string.Empty, string.Empty, true, true, false, true, GeneralEnumerations.ActivityCreationCheckinBestFit.AgeRange, "18", "30", "Years");

            // Verify activity was created
            test.Portal.Ministry_Activities_View_All_WebDriver();
            test.Portal.Ministry_Activities_Filter_Activity_Name(activityName, true);
            test.GeneralMethods.VerifyTextPresentWebDriver(activityName);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("FO-6459 Creates an activity without check-in but an age range through the wizard.")]
        public void Ministry_Activities_ViewAll_AddAnActivity_Once_NoCheckin_AgeRange()
        {
            // Data
            string ministryName = "Auto Test Ministry";
            string activityName = "*One Time Age no CheckIn";
            string scheduleName = activityName;
            string rosterName = activityName;

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Setup roster names
            System.Collections.Generic.List<string> roster = new System.Collections.Generic.List<string>();
            roster.Add(rosterName);

            // Create a one time activity without check-in but age through the activity wizard
            test.Portal.Ministry_Activities_Create_Step1_WebDriver(ministryName, activityName, string.Empty, false, "---", false, true, true, false, false, GeneralEnumerations.ActivitiesRequireAssignment.None, true, "10", "14");
            test.GeneralMethods.WaitForElement(By.XPath("//table[@class='normalize']/tbody/tr[3]/td"));
            Assert.AreEqual("10 to 14 years", test.Driver.FindElement(By.XPath("//table[@class='normalize']/tbody/tr[3]/td")).Text, "The age restriction display error");

            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Ministry_Activities_Create_Step2_OneTime_WebDriver(15, scheduleName, "6:00 PM", "8:00 PM", today.AddDays(1).ToShortDateString());

            test.GeneralMethods.WaitForElement(By.XPath("//table[@class='normalize']/tbody/tr[3]/td"));
            Assert.AreEqual("10 to 14 years", test.Driver.FindElement(By.XPath("//table[@class='normalize']/tbody/tr[3]/td")).Text, "The age restriction display error");
            test.Portal.Ministry_Activities_Create_Step3_WebDriver(false, roster, string.Empty, string.Empty, true, true, false, true, GeneralEnumerations.ActivityCreationCheckinBestFit.AgeRange, "10", "12", "Years");

            // Verify activity was created
            test.Portal.Ministry_Activities_View_All_WebDriver();
            test.Portal.Ministry_Activities_Filter_Activity_Name(activityName, true);
            test.GeneralMethods.VerifyTextPresentWebDriver(activityName);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        #region Validation

        #region Step 1
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies the text in the Remember box on Step 1 of the activity creation wizard")]
        public void Ministry_Activities_ViewAll_AddAnActivity_Step1_RememberBox()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Activities/View All page
            test.Portal.Ministry_Activities_View_All_WebDriver();

            // Click to create a new activity
            test.Driver.FindElementByLinkText("Add activity").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Cancel"));

            // Verify the text in the Remember box on Step 1
            test.GeneralMethods.VerifyTextPresentWebDriver("Remember: You must complete all steps in the workflow to create the Activity.");

            // Cancel out of activity wizard
            test.Driver.FindElementByLinkText("Cancel");

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies that a ministry is not selected by default and is required")]
        public void Ministry_Activities_ViewAll_AddAnActivity_Step1_NoMinistrySelected()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Perform Step 1 of the activity creation wizard
            test.Portal.Ministry_Activities_Create_Step1_WebDriver("Please select a ministry....", "No Ministry Activity");

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies an activity name is required.")]
        public void Ministry_Activities_ViewAll_AddAnActivity_Step1_NoActivityName()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Perform Step 1 of the activity creation wizard
            test.Portal.Ministry_Activities_Create_Step1_WebDriver("Auto Test Ministry", string.Empty);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the name of the activity cannot exceed 30 characters")]
        public void Ministry_Activities_ViewAll_AddAnActivity_Step1_NameCannotExceed30Characters()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Activities/View All page
            test.Portal.Ministry_Activities_View_All_WebDriver();

            // Click to create a new activity
            test.Driver.FindElementById("add_activity_button").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Cancel"));

            // Enter an activity name greater than 30 characters
            test.Driver.FindElementById("activity_name").SendKeys("A Really Really Long Activity Monster");

            // Verify only 30 characters were entered
            test.GeneralMethods.VerifyTextNotPresentWebDriver("Monster");

            // Cancel out of activity wizard
            test.Driver.FindElementByLinkText("Cancel").Click();

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }
        #endregion Step 1

        #region Step 2
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies a schedule name is required.")]
        public void Ministry_Activities_ViewAll_AddAnActivity_Step2_NoScheduleName()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to activity creation wizard past Step 1
            test.Portal.Ministry_Activities_Create_Step1_WebDriver("Auto Test Ministry", "No Schedule Name");

            // Perform Step 2 with no schedule name
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Ministry_Activities_Create_Step2_OneTime_WebDriver(15, string.Empty, "6:00PM", "8:00PM", today.AddDays(1).ToShortDateString());

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies a Start Time is required for an activity schedule.")]
        public void Ministry_Activities_ViewAll_AddAnActivity_Step2_NoScheduleStartTime()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to activity creation wizard past Step 1
            test.Portal.Ministry_Activities_Create_Step1_WebDriver("Auto Test Ministry", "No Schedule Start Time");

            // Perform Step 2 with no schedule start time
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Ministry_Activities_Create_Step2_OneTime_WebDriver(15, "One Time", string.Empty, "8:00PM", today.AddDays(1).ToShortDateString());

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies an End Time is required for an activity schedule.")]
        public void Ministry_Activities_ViewAll_AddAnActivity_Step2_NoScheduleEndTime()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to activity creation wizard past Step 1
            test.Portal.Ministry_Activities_Create_Step1_WebDriver("Auto Test Ministry", "No Schedule End Time");

            // Perform Step 2 with no schedule end time
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Ministry_Activities_Create_Step2_OneTime_WebDriver(15, "One Time", "6:00PM", string.Empty, today.AddDays(1).ToShortDateString());

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies a schedule date is required for a one time activity schedule.")]
        public void Ministry_Activities_ViewAll_AddAnActivity_Step2_Once_NoScheduledate()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to activity creation wizard past Step 1
            test.Portal.Ministry_Activities_Create_Step1_WebDriver("Auto Test Ministry", "Once - No Date");

            // Perform Step 2 with no schedule date
            test.Portal.Ministry_Activities_Create_Step2_OneTime_WebDriver(15, "One Time", "6:00PM", "8:00PM", string.Empty);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies a selection for start date is required for a daily activity schedule")]
        public void Ministry_Activities_ViewAll_AddAnActivity_Step2_Daily_NoScheduleStartDate()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to activity creation wizard past Step 1
            test.Portal.Ministry_Activities_Create_Step1_WebDriver("Auto Test Ministry", "Daily - No Start Date");

            // Perform Step 2 with no schedule start date
            test.Portal.Ministry_Activities_Create_Step2_Daily_WebDriver(15, "Daily", "6:00PM", "8:00PM", "1", string.Empty);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies a start date is required for a weekly activity schedule")]
        public void Ministry_Activities_ViewAll_AddAnActivity_Step2_Weekly_NoScheduleStartDate()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Setup days to meet
            System.Collections.Generic.List<GeneralEnumerations.WeeklyScheduleDays> daysOfWeek = new System.Collections.Generic.List<GeneralEnumerations.WeeklyScheduleDays>();
            daysOfWeek.Add(GeneralEnumerations.WeeklyScheduleDays.Sunday);

            // Navigate to activity creation wizard past Step 1
            test.Portal.Ministry_Activities_Create_Step1_WebDriver("Auto Test Ministry", "Weekly - No Start Date");

            // Perform Step 2 with no start date for weekly
            test.Portal.Ministry_Activities_Create_Step2_Weekly_WebDriver(15, "Weekly", "6:00PM", "8:00PM", "1", daysOfWeek, string.Empty);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies a selection for days the activity repeats on is required for a weekly activity schedule")]
        public void Ministry_Activities_ViewAll_AddAnActivity_Step2_Weekly_NoRepeatsOn()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Setup days to meet
            System.Collections.Generic.List<GeneralEnumerations.WeeklyScheduleDays> daysOfWeek = new System.Collections.Generic.List<GeneralEnumerations.WeeklyScheduleDays>();
            
            // Navigate to activity creation wizard past Step 1
            test.Portal.Ministry_Activities_Create_Step1_WebDriver("Auto Test Ministry", "Weekly - No Repeat On Days");

            // Perform Step 2 with no repeat on days for weekly
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Ministry_Activities_Create_Step2_Weekly_WebDriver(15, "Weekly", "6:00PM", "8:00PM", "1", daysOfWeek, today.AddDays(1).ToShortDateString());

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }
        #endregion Step 2

        #endregion Validation

        #endregion Add an Activity

        #region Dashboard
        #endregion Dashboard

        #region Schedules

        #region Validation
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("F1-2161: Verifies a schedule name is required when adding a new schedule")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Add_NoName()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName2 = "One Time Schedule";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add a Once schedule with no date in the recurrence
            test.Portal.Ministry_Activities_Schedules_Add_OneTime(ministryName2, activityName2, string.Empty, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1).ToShortDateString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-2161: Verifies a start time is required when adding a new schedule")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Add_NoStartTime()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName2 = "One Time Schedule";
            string scheduleName = "Schedule No Start Time";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add a Once schedule with no date in the recurrence
            test.Portal.Ministry_Activities_Schedules_Add_OneTime(ministryName2, activityName2, scheduleName, string.Empty, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1).ToShortDateString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-2161: Verifies an end time is required when adding a new schedule")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Add_NoEndTime()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName2 = "One Time Schedule";
            string scheduleName = "Schedule No End Time";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add a Once schedule with no date in the recurrence
            test.Portal.Ministry_Activities_Schedules_Add_OneTime(ministryName2, activityName2, scheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2).ToShortTimeString(), string.Empty, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1).ToShortDateString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }
        #endregion Validation

        #region Once
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("F1-2161: Adds a Once schedule to an activity")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Add_Once()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName2 = "One Time Schedule";
            string scheduleName = "One Time Schedule";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add a Once schedule to an activity
            test.Portal.Ministry_Activities_Schedules_Add_OneTime(ministryName2, activityName2, scheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1).ToShortDateString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Delete schedule
            base.SQL.Ministry_ActivitySchedules_Delete(15, scheduleName);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-2161: Verifies a date is required when adding a Once recurrence")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Add_Once_NoDate()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName2 = "One Time Schedule";
            string scheduleName = "One Time Schedule No Date";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add a Once schedule with no date in the recurrence
            test.Portal.Ministry_Activities_Schedules_Add_OneTime(ministryName2, activityName2, scheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3).ToShortTimeString(), string.Empty);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }
        #endregion Once

        #region Daily
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("F1-2161: Adds a Daily schedule to an activity.")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Add_Daily()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName = "Daily Schedule";
            string scheduleName = "Daily Schedule Auto Test";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add a daily recurrence schedule
            test.Portal.Ministry_Activities_Schedules_Add_Daily(ministryName2, activityName, scheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1).ToShortDateString(), "1");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Delete schedule
            base.SQL.Ministry_ActivitySchedules_Delete(15, scheduleName);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-2161: Verifies a start date is required for a Daily recurrence schedule")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Add_Daily_NoStartDate()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName = "Daily Schedule";
            string scheduleName = "Daily Schedule No Start Date";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add a daily recurrence schedule
            test.Portal.Ministry_Activities_Schedules_Add_Daily(ministryName2, activityName, scheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3).ToShortTimeString(), string.Empty, "1");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-2161: Verifies a daily interval is required for a Daily recurrence schedule")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Add_Daily_NoDailyInterval()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName = "Daily Schedule";
            string scheduleName = "Daily Schedule No Interval";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add a daily recurrence schedule
            test.Portal.Ministry_Activities_Schedules_Add_Daily(ministryName2, activityName, scheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1).ToShortDateString(), "---");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }
        #endregion Daily

        #region Weekly
        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-2161: Adds a single day Weekly schedule to an activity")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Add_Weekly_SingleDay()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName = "Weekly Schedule";
            string scheduleName = "Weekly Schedule";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add a weekly recurrence schedule
            test.Portal.Ministry_Activities_Schedules_Add_Weekly(ministryName2, activityName, scheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1).ToShortDateString(), "1", new GeneralEnumerations.WeeklyScheduleDays[] { GeneralEnumerations.WeeklyScheduleDays.Tuesday });

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Delete schedule
            base.SQL.Ministry_ActivitySchedules_Delete(15, scheduleName);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-2161: Adds a multi day Weekly schedule to an activity")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Add_Weekly_MultiDay()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName = "Weekly Schedule";
            string scheduleName = "Weekly Schedule";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add a weekly recurrence schedule
            test.Portal.Ministry_Activities_Schedules_Add_Weekly(ministryName2, activityName, scheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1).ToShortDateString(), "1", new GeneralEnumerations.WeeklyScheduleDays[] { GeneralEnumerations.WeeklyScheduleDays.Tuesday, GeneralEnumerations.WeeklyScheduleDays.Saturday });

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Delete schedule
            base.SQL.Ministry_ActivitySchedules_Delete(15, scheduleName);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-2161: Verifies a start date is required for a Weekly recurrence schedule")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Add_Weekly_NoStartDate()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName = "Weekly Schedule";
            string scheduleName = "Weekly Schedule No Date";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add a weekly recurrence schedule
            test.Portal.Ministry_Activities_Schedules_Add_Weekly(ministryName2, activityName, scheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3).ToShortTimeString(), string.Empty, "1", new GeneralEnumerations.WeeklyScheduleDays[] { GeneralEnumerations.WeeklyScheduleDays.Tuesday });

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-2161: Verifies a weekly interval is required for a Weekly recurrence schedule")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Add_Weekly_NoWeeklyInterval()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName = "Weekly Schedule";
            string scheduleName = "Weekly Schedule No Interval";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add a weekly recurrence schedule
            test.Portal.Ministry_Activities_Schedules_Add_Weekly(ministryName2, activityName, scheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1).ToShortDateString(), "---", new GeneralEnumerations.WeeklyScheduleDays[] { GeneralEnumerations.WeeklyScheduleDays.Tuesday });

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-2161: Verifies a week day is required for a Weekly recurrence schedule")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Add_Weekly_NoWeekday()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName = "Weekly Schedule";
            string scheduleName = "Weekly Schedule No Week Day";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add a weekly recurrence schedule
            test.Portal.Ministry_Activities_Schedules_Add_Weekly(ministryName2, activityName, scheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1).ToShortDateString(), "1", null);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }
        #endregion Weekly

        #region Monthly
        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-2161: Adds a Monthly schedule to an activity")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Add_Monthly()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName = "Monthly Schedule";
            string scheduleName = "Monthly Schedule";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add a monthly schedule
            test.Portal.Ministry_Activities_Schedules_Add_Monthly(ministryName2, activityName, scheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1).ToShortDateString(), "1st", "1");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Delete schedule
            base.SQL.Ministry_ActivitySchedules_Delete(15, scheduleName);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-2161: Verifies a start date is required for a monthly schedule")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Add_Monthly_NoStartDate()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName = "Monthly Schedule";
            string scheduleName = "Monthly Schedule No Date";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add a monthly schedule
            test.Portal.Ministry_Activities_Schedules_Add_Monthly(ministryName2, activityName, scheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3).ToShortTimeString(), string.Empty, "1st", "1");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-2161: Verifies a day of the month is required for a monthly schedule")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Add_Monthly_NoMonthDay()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName = "Monthly Schedule";
            string scheduleName = "Monthly Schedule No Day";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add a monthly schedule
            test.Portal.Ministry_Activities_Schedules_Add_Monthly(ministryName2, activityName, scheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1).ToShortDateString(), "---", "1");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-2161: Verifies a month interval is required for a monthly schedule")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Add_Monthly_NoMonthInterval()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName = "Monthly Schedule";
            string scheduleName = "Monthly Schedule No Interval";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add a monthly schedule
            test.Portal.Ministry_Activities_Schedules_Add_Monthly(ministryName2, activityName, scheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1).ToShortDateString(), "1st", "---");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }
        #endregion Monthly

        #region Yearly
        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-2161: Adds a Yearly schedule to an activity")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Add_Yearly_SameDate()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName = "Yearly Schedule";
            string scheduleName = "Yearly Schedule";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add a yearly schedule
            test.Portal.Ministry_Activities_Schedules_Add_Yearly(ministryName2, activityName, scheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1).ToShortDateString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Delete schedule
            base.SQL.Ministry_ActivitySchedules_Delete(15, scheduleName);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-2161: Adds a Yearly schedule to an activity")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Add_Yearly_SpecificWeekDay()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName = "Yearly Schedule";
            string scheduleName = "Yearly Schedule 2";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add a yearly schedule
            test.Portal.Ministry_Activities_Schedules_Add_Yearly(ministryName2, activityName, scheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1).ToShortDateString(), true, "2nd", "Wed", "March");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Delete schedule
            base.SQL.Ministry_ActivitySchedules_Delete(15, scheduleName);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-2161: Verifies a start date is required for a yearly schedule")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Add_Yearly_NoStartDate()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName = "Yearly Schedule";
            string scheduleName = "Yearly Schedule No Date";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add a yearly schedule
            test.Portal.Ministry_Activities_Schedules_Add_Yearly(ministryName2, activityName, scheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3).ToShortTimeString(), string.Empty);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-2161: Verifies a day, week, and month is required for a yearly schedule")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Add_Yearly_NoDayWeekMonth()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName = "Yearly Schedule";
            string scheduleName = "Yearly Schedule No Date 2";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add a yearly schedule
            test.Portal.Ministry_Activities_Schedules_Add_Yearly(ministryName2, activityName, scheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1).ToShortDateString(), true);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }
        #endregion Yearly

        #region Edit/Delete
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("F1-2161: Verifies you can delete an activity schedule")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Delete_Schedule()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName = "Daily Schedule";
            string scheduleName = "Delete Schedule Test";
            base.SQL.Ministry_ActivitySchedules_Create(15, activityName, scheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3));

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Delete Recurrence from activity schedule
            test.Portal.Ministry_Activities_Schedules_Delete_Schedule(ministryName2, activityName, scheduleName);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-2161: Verifies you can delete a recurrence from an activity schedule")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Delete_Recurrence()
        {
            // Setup
            string ministryName2 = "Auto Test Schedule Ministry";
            string activityName = "Daily Schedule";
            string scheduleName = "Delete Recurrence Schedule";
            string recurrenceName = "Every 3 days";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add a schedule and recurrence to an activity
            test.Portal.Ministry_Activities_Schedules_Add_Daily(ministryName2, activityName, scheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1).ToShortDateString(), "3");

            // Delete recurrence from the activity schedule
            test.Portal.Ministry_Activities_Schedules_Delete_Recurrence(ministryName2, activityName, scheduleName, recurrenceName);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }
        #endregion Edit/Delete

        #region Assignment Pills

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Stuart Platt")]
        [Description("Verifies that the Volunteer assignment pill on the Activity Schedule tab navigate to the Assignments page")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Assignments_Pill_Staff()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            string activityName = "Assign Schedule Level";
            string scheduleName = "Assign Schedule Level";
            
            //Navigate to Schedule page
            test.Portal.Ministry_Activities_Schedules_View(ministryName, activityName);

            //CLick on the Assignment pill
            test.Portal.Ministry_Activities_Schedules_AssignmentPill(15, activityName, scheduleName, "Staff");

            //Logout
            test.Portal.LogoutWebDriver();
            
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Participant assignment pill on the Activity Schedule tab navigate to the Assignments page")]
        public void Ministry_Activities_ViewAll_Activity_Schedules_Assignmnets_Pill_Participant()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            string activityName = "Assign Schedule Level";

            //Navigate to Schedule page
            test.Portal.Ministry_Activities_Schedules_View(ministryName, activityName);

            //CLick on the Assignment pill
            test.Portal.Ministry_Activities_Schedules_AssignmentPill(15, activityName, "Assign Schedule Level", "Participant");

            //Logout
            test.Portal.LogoutWebDriver();
        }

        #endregion Assignment Pills

        #endregion Schedules

        #region Rosters
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies you can access the Roster listing page")]
        public void Ministry_Activities_ViewAll_Activity_Rosters()
        {
            // Data
            #region Data
            //string ministryName = "A Test Ministry";
            string activityName = "*Rosters";
            string rosterFolderName1 = "First Level";
            string rosterFolderName2 = "Second Level";
            string rosterFolderName3 = "Third Level";
            string rosterFolderName4 = "No Rosters";
            string rosterFolderName5 = "All Rosters";
            int activityId = base.SQL.Ministry_Activities_FetchID(15, activityName);
            int rosterFolderId1 = base.SQL.Ministry_RosterFolders_FetchID(15, rosterFolderName1, activityId);
            int rosterFolderId2 = base.SQL.Ministry_RosterFolders_FetchID(15, rosterFolderName2, activityId);
            int rosterFolderId3 = base.SQL.Ministry_RosterFolders_FetchID(15, rosterFolderName3, activityId);
            int rosterFolderId4 = base.SQL.Ministry_RosterFolders_FetchID(15, rosterFolderName4, activityId);
            int rosterFolderId5 = base.SQL.Ministry_RosterFolders_FetchID(15, rosterFolderName5, activityId);
            #endregion Data

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Rosters page
            test.Portal.Ministry_Activities_Rosters_View(ministryName, activityName);

            // Verify gear options for each roster folder
            // First Level
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(string.Format("//div[@id='{0}_header']/span[@class='relativize float_right normal']/a", rosterFolderId1)));
            bool subRosterExist = test.GeneralMethods.IsElementExist(By.Id("block" + rosterFolderId3));

            test.Driver.FindElementByXPath(string.Format("//div[@id='{0}_header']/span[@class='relativize float_right normal']/a", rosterFolderId1)).Click();
            Assert.IsFalse(test.Driver.FindElementByXPath("//div[@id='" + rosterFolderId1 + "_header']/span[@class='relativize float_right normal uiMenuVisible']/ul/li/a[@class='add_roster_link']").Displayed, "Add Roster was clickable when it shouldn't be");
            Assert.IsTrue(test.Driver.FindElementByXPath("//div[@id='" + rosterFolderId1 + "_header']/span[@class='relativize float_right normal uiMenuVisible']/ul/li[2]/a[@class='add_folder_link']").Displayed, "Add Subfolder was not clickable when it should be");
            Assert.IsTrue(test.Driver.FindElementByLinkText("Edit").Displayed, "Edit was not displaying as an option");
            if (!subRosterExist)
            {
                Assert.IsTrue(test.Driver.FindElementByXPath("//div[@id='" + rosterFolderId1 + "_header']/span[@class='relativize float_right normal uiMenuVisible']/ul/li[4]/a[@class='delete_group_link']").Displayed, "Delete was not clickable when it should be");
            }
            else
            {
                Assert.IsFalse(test.Driver.FindElementByXPath("//div[@id='" + rosterFolderId1 + "_header']/span[@class='relativize float_right normal uiMenuVisible']/ul/li[4]/a[@class='delete_group_link']").Displayed, "Delete was clickable when it should not be");
            }
            
            // Second Level
            test.Driver.FindElementByXPath(string.Format("//div[@id='{0}_header']/span[@class='relativize float_right normal']/a", rosterFolderId2)).Click();
            Assert.IsFalse(test.Driver.FindElementByXPath("//div[@id='" + rosterFolderId2 + "_header']/span[@class='relativize float_right normal uiMenuVisible']/ul/li/a[@class='add_roster_link']").Displayed, "Add Roster was clickable when it shouldn't be");
            Assert.IsTrue(test.Driver.FindElementByXPath("//div[@id='" + rosterFolderId2 + "_header']/span[@class='relativize float_right normal uiMenuVisible']/ul/li[2]/a[@class='add_folder_link']").Displayed, "Add Subfolder was not clickable when it should be");
            Assert.IsTrue(test.Driver.FindElementByLinkText("Edit").Displayed, "Edit was not displaying as an option");
            if (!subRosterExist)
            {
                Assert.IsTrue(test.Driver.FindElementByXPath("//div[@id='" + rosterFolderId2 + "_header']/span[@class='relativize float_right normal uiMenuVisible']/ul/li[4]/a[@class='delete_group_link']").Displayed, "Delete was not clickable when it should be");
            }
            else
            {
                Assert.IsFalse(test.Driver.FindElementByXPath("//div[@id='" + rosterFolderId2 + "_header']/span[@class='relativize float_right normal uiMenuVisible']/ul/li[4]/a[@class='delete_group_link']").Displayed, "Delete was clickable when it should not be");
            }

            // Third Level
            test.Driver.FindElementByXPath(string.Format("//div[@id='{0}_header']/span[@class='relativize float_right normal']/a", rosterFolderId3)).Click();
            Assert.IsTrue(test.Driver.FindElementByXPath("//div[@id='" + rosterFolderId3 + "_header']/span[@class='relativize float_right normal uiMenuVisible']/ul/li/a[@class='add_roster_link']").Displayed, "Add Roster was not clickable when it should be");
            Assert.IsFalse(test.Driver.FindElementByXPath("//div[@id='" + rosterFolderId3 + "_header']/span[@class='relativize float_right normal uiMenuVisible']/ul/li/a[@class='add_folder_link']").Displayed, "Add Subfolder was clickable when it shouldn't be");
            Assert.IsTrue(test.Driver.FindElementByLinkText("Edit").Displayed, "Edit was not displaying as an option");
            if (!subRosterExist)
            {
                 Assert.IsTrue(test.Driver.FindElementByXPath("//div[@id='" + rosterFolderId3 + "_header']/span[@class='relativize float_right normal uiMenuVisible']/ul/li[4]/a[@class='delete_group_link']").Displayed, "Delete was not clickable when it should be");
            }
            else
            {
                 Assert.IsFalse(test.Driver.FindElementByXPath("//div[@id='" + rosterFolderId3 + "_header']/span[@class='relativize float_right normal uiMenuVisible']/ul/li[4]/a[@class='delete_group_link']").Displayed, "Delete was clickable when it should not be");
            }

            // No Rosters
            test.Driver.FindElementByXPath(string.Format("//div[@id='{0}_header']/span[@class='relativize float_right normal']/a", rosterFolderId4)).Click();
            Assert.IsTrue(test.Driver.FindElementByXPath("//div[@id='" + rosterFolderId4 + "_header']/span[@class='relativize float_right normal uiMenuVisible']/ul/li/a[@class='add_roster_link']").Displayed, "Add Roster was not clickable when it should be");
            Assert.IsTrue(test.Driver.FindElementByXPath("//div[@id='" + rosterFolderId4 + "_header']/span[@class='relativize float_right normal uiMenuVisible']/ul/li[2]/a[@class='add_folder_link']").Displayed, "Add Subfolder was not clickable when it should be");
            Assert.IsTrue(test.Driver.FindElementByLinkText("Edit").Displayed, "Edit was not displaying as an option");
            if (!test.GeneralMethods.IsElementExist(By.Id("block" + rosterFolderId4)))
            {
                Assert.IsTrue(test.Driver.FindElementByXPath("//div[@id='" + rosterFolderId4 + "_header']/span[@class='relativize float_right normal uiMenuVisible']/ul/li[4]/a[@class='delete_group_link']").Displayed, "Delete was not clickable when it should be");
            }
            else
            {
                Assert.IsFalse(test.Driver.FindElementByXPath("//div[@id='" + rosterFolderId4 + "_header']/span[@class='relativize float_right normal uiMenuVisible']/ul/li[4]/a[@class='delete_group_link']").Displayed, "Delete was clickable when it should not be");
            }

            // All Rosters
            test.Driver.FindElementByXPath(string.Format("//div[@id='{0}_header']/span[@class='relativize float_right normal']/a", rosterFolderId5)).Click();
            Assert.IsTrue(test.Driver.FindElementByXPath("//div[@id='" + rosterFolderId5 + "_header']/span[@class='relativize float_right normal uiMenuVisible']/ul/li/a[@class='add_roster_link']").Displayed, "Add Roster was not clickable when it should be");
            Assert.IsFalse(test.Driver.FindElementByXPath("//div[@id='" + rosterFolderId5 + "_header']/span[@class='relativize float_right normal uiMenuVisible']/ul/li/a[@class='add_folder_link']").Displayed, "Add Subfolder was clickable when it shouldn't be");
            Assert.IsTrue(test.Driver.FindElementByLinkText("Edit").Displayed, "Edit was not displaying as an option");
            if (!test.GeneralMethods.IsElementExist(By.Id("block" + rosterFolderId5)))
            {
                Assert.IsTrue(test.Driver.FindElementByXPath("//div[@id='" + rosterFolderId5 + "_header']/span[@class='relativize float_right normal uiMenuVisible']/ul/li/a[@class='delete_group_link']").Displayed, "Delete was not clickable when it should be");
            }
            else
            {
                Assert.IsFalse(test.Driver.FindElementByXPath("//div[@id='" + rosterFolderId5 + "_header']/span[@class='relativize float_right normal uiMenuVisible']/ul/li/a[@class='delete_group_link']").Displayed, "Delete was clickable when it shouldn't be");
            }

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies you can add a new roster folder")]
        public void Ministry_Activities_ViewAll_Activity_Rosters_AddRosterFolder()
        {
            // Data
            //string ministryName = "A Test Ministry";
            string activityName = "*Rosters";
            string rosterFolderName = "Automation Folder";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");
            base.SQL.Ministry_ActivityRLCGroups_Delete(15, activityName, rosterFolderName);
            // Navigate to the Rosters page
            test.Portal.Ministry_Activities_Rosters_View(ministryName, activityName);

            // Add a new roster folder
            test.Portal.Ministry_Activities_RosterFolder_Add(rosterFolderName);

            // Wait a minute
            System.Threading.Thread.Sleep(5000);

            try
            {
                // Verify folder was created
                test.GeneralMethods.VerifyTextPresentWebDriver(rosterFolderName);

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            finally
            {
                // Clean up
                base.SQL.Ministry_ActivityRLCGroups_Delete(15, activityName, rosterFolderName);
            }


        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies you can edit an existing roster folder")]
        public void Ministry_Activities_ViewAll_Activity_Rosters_EditRosterFolder()
        {
            // Data
            //string ministryName = "A Test Ministry";
            string activityName = "*Rosters";
            string rosterFolderName = "Automation - Edit - Folder";
            string editedRosterFolderName = "Edited Automation Folder";
            base.SQL.Ministry_ActivityRLCGroups_Delete(15, activityName, rosterFolderName);
            base.SQL.Ministry_ActivityRLCGroups_Delete(15, activityName, editedRosterFolderName);
            base.SQL.Ministry_ActivityRLCGroups_Create(15, activityName, rosterFolderName);
            int activityId = base.SQL.Ministry_Activities_FetchID(15, activityName);
            int rosterFolderId = base.SQL.Ministry_RosterFolders_FetchID(15, rosterFolderName, activityId);

            try
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

                // Navigate to the Rosters page
                test.Portal.Ministry_Activities_Rosters_View(ministryName, activityName);

                // Edit the roster folder
                test.Portal.Ministry_Activities_RosterFolder_Edit(activityName, rosterFolderName, editedRosterFolderName);

                // Wait a minute
                System.Threading.Thread.Sleep(5000);

                // Verify roster folder was edited
                test.GeneralMethods.VerifyTextPresentWebDriver(editedRosterFolderName);

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            finally
            {
                // Clean up
                base.SQL.Ministry_ActivityRLCGroups_Delete(15, activityName, rosterFolderName);
                base.SQL.Ministry_ActivityRLCGroups_Delete(15, activityName, editedRosterFolderName);
            }

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies you can delete an existing roster folder")]
        public void Ministry_Activities_ViewAll_Activity_Rosters_DeleteRosterFolder()
        {
            // Data
            //string ministryName = "A Test Ministry";
            string activityName = "*Rosters";
            string rosterFolderName = "Delete Automation Folder";
            base.SQL.Ministry_ActivityRLCGroups_Delete(15, activityName, rosterFolderName);
            base.SQL.Ministry_ActivityRLCGroups_Create(15, activityName, rosterFolderName);
            int activityId = base.SQL.Ministry_Activities_FetchID(15, activityName);
            int rosterFolderId = base.SQL.Ministry_RosterFolders_FetchID(15, rosterFolderName, activityId);

            try
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

                // Navigate to the Rosters page
                test.Portal.Ministry_Activities_Rosters_View(ministryName, activityName);

                // Click to delete roster folder
                System.Threading.Thread.Sleep(5000);
                test.Portal.Ministry_Activities_RosterFolder_Delete(activityName, rosterFolderName);

                // Confirm roster folder was deleted
                test.GeneralMethods.VerifyTextNotPresentWebDriver(rosterFolderName);

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            finally
            {
                base.SQL.Ministry_ActivityRLCGroups_Delete(15, activityName, rosterFolderName);
            }

        }

        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies you can drag and drop roster folders on the Rosters page")]
        public void Ministry_Activities_ViewAll_Activity_Rosters_DragAndDropRosterFolders()
        {

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies you can add a new roster to a roster group")]
        public void Ministry_Activities_ViewAll_Activity_Rosters_AddRoster()
        {
            // Data
            //string ministryName = "A Test Ministry";
            string activityName = "*Rosters";
            string rosterFolderName = "All Rosters";
            string rosterName = "Add New Roster";
            int activityId = base.SQL.Ministry_Activities_FetchID(15, activityName);
            int rosterFolderId = base.SQL.Ministry_RosterFolders_FetchID(15, rosterFolderName, activityId);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Rosters page
            test.Portal.Ministry_Activities_Rosters_View(ministryName, activityName);

            // Add new roster to roster folder
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText(GeneralMinistry.Activities.RosterFolder_Add));
            test.Portal.Ministry_Activities_Rosters_AddEdit(ministryName, activityName, true, rosterFolderName, rosterName, true, rosterFolderName, string.Empty, string.Empty, false, false, false, false, false, false, string.Empty, string.Empty, null, string.Empty, string.Empty, false, false);

            // Wait a minute
            System.Threading.Thread.Sleep(10000);
            //test.GeneralMethods.WaitForElementNotDisplayed(By.ClassName("loading_animation_overlay"));

            // Verify roster was created
            test.GeneralMethods.VerifyTextPresentWebDriver(rosterName);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Ministry_ActivityDetails_Delete(15, activityName, rosterName);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies you can edit an existing roster")]
        public void Ministry_Activities_ViewAll_Activity_Rosters_EditRoster()
        {
            // Data
            //string ministryName = "A Test Ministry";
            string activityName = "*Rosters";
            string rosterFolderName = "All Rosters";
            string rosterName = "Attendees";
            string newRosterName = "Attendees YAY";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Rosters page
            test.Portal.Ministry_Activities_Rosters_View(ministryName, activityName);

            // Edit a roster
            test.Portal.Ministry_Activities_Rosters_AddEdit(ministryName, activityName, false, rosterFolderName, newRosterName, true, rosterFolderName, string.Empty, string.Empty, false, false, false, false, false, false, string.Empty, string.Empty, null, string.Empty, string.Empty, false, false);

            // Wait a minute
            System.Threading.Thread.Sleep(5000);
            //test.GeneralMethods.WaitForElementNotDisplayed(By.ClassName("loading_animation_overlay"));

            // Verify roster was edited
            test.GeneralMethods.VerifyTextPresentWebDriver(newRosterName);

            // Edit roster back 
            test.Portal.Ministry_Activities_Rosters_AddEdit(ministryName, activityName, false, rosterFolderName, rosterName, true, rosterFolderName, string.Empty, string.Empty, false, false, false, false, false, false, string.Empty, string.Empty, null, string.Empty, string.Empty, false, false);

            // Verify roster was edited back
            System.Threading.Thread.Sleep(5000);
            test.GeneralMethods.VerifyTextPresentWebDriver(rosterName);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        #region Assignment Pills

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies the assignment pills from the Rosters tab navigates to the Assignments page")]
        public void Ministry_Activities_ViewAll_Activity_RosterFolder_Assignments_Pill_Staff()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            string activityName = "Assign Activity Level";
            string rosterGroupName = "Roster Grouping";
            string rosterName = "Assign Activity Level";

            // Navigate to Roster page
            test.Portal.Ministry_Activities_Rosters_View(ministryName, activityName);

            // Click on the Staff assignment pill for Roster Group
            test.Portal.Ministry_Activities_Rosters_AssignmentPill(15, activityName, rosterGroupName, rosterName, true, false);

            // Logout
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the assignment pills from the Rosters tab navigates to the Assignments page")]
        public void Ministry_Activities_ViewAll_Activity_RosterFolder_Assignments_Pill_Participant()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            string activityName = "Assign Activity Level";
            string rosterGroupName = "Roster Grouping";
            string rosterName = "Assign Activity Level";

            // Navigate to Roster page
            test.Portal.Ministry_Activities_Rosters_View(ministryName, activityName);

            // Click on the Staff assignment pill for Roster
            test.Portal.Ministry_Activities_Rosters_AssignmentPill(15, activityName, rosterGroupName, rosterName, false, true);

            // Logout
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the assignment pills from the Rosters tab navigates to the Assignments page")]
        public void Ministry_Activities_ViewAll_Activity_Rosters_Assignments_Pill_Staff()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            string activityName = "Assign Activity Level";
            string rosterGroupName = "Roster Grouping";
            string rosterName = "Assign Activity Level";

            // Navigate to Roster page
            test.Portal.Ministry_Activities_Rosters_View(ministryName, activityName);

            // Click on the Staff assignment pill for Roster
            test.Portal.Ministry_Activities_Rosters_AssignmentPill(15, activityName, rosterGroupName, rosterName, false, false);

            // Logout
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the assignment pills from the Rosters tab navigates to the Assignments page")]
        public void Ministry_Activities_ViewAll_Activity_Rosters_Assignments_Pill_Participant()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            string activityName = "Assign Activity Level";
            string rosterGroupName = "Roster Grouping";
            string rosterName = "Assign Activity Level";

            // Navigate to Roster page
            test.Portal.Ministry_Activities_Rosters_View(ministryName, activityName);

            // Click on the Staff assignment pill for Roster
            test.Portal.Ministry_Activities_Rosters_AssignmentPill(15, activityName, rosterGroupName, rosterName, false, true);

            // Logout
            test.Portal.LogoutWebDriver();

        }

        #endregion Assignment Pills

        #endregion Rosters

        #region Requirements

        //[Test, RepeatOnFailure, MultipleAsserts]
        [Author("Stuart Platt")]
        [Description("Verify Activity Requirements Page")]
        public void Ministry_Activities_ViewAll_Activity_Requirements()
        {
            //Log in to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("splatt", "Romans10:9", "DC");

            // Setup
            string activityName = "A Test Activity";
            string ministryName = "A Test Ministry";

            // Navigate to Activity View All Requirments Tab 
            test.Portal.Ministry_Activities_View_All_Requirements(ministryName, activityName);

            //Verify Page elements
            Assert.IsTrue(test.Driver.FindElementById(GeneralMinistry.Activities.Active_RequirementsCheckBox).Selected, "Active Check Box should be selected by default");
            Assert.IsFalse(test.Driver.FindElementById(GeneralMinistry.Activities.Inactive_Requirements_CheckBox).Selected, "Inactive Check Box should not be selected by default");
            Assert.IsTrue(test.Driver.FindElementByLinkText(GeneralMinistry.Activities.Add_Requirement).Displayed, "Add Requirement button is not displayed");
            Assert.AreEqual("-All schedules", new SelectElement(test.Driver.FindElementById(GeneralMinistry.Activities.Requirements_Schedule_DropDown)).SelectedOption.Text.Trim());

            //Verify Table Elements 
            IWebElement requirementsTable = test.Driver.FindElementByXPath(TableIds.Ministry_Activities_ViewAll_Requirements);
            Assert.AreEqual("Name", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[0].Text, "Requirements Header Mismatch");
            Assert.AreEqual("Miss", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[1].Text, "Requirements Header Mismatch");
            Assert.AreEqual("Comp", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[2].Text, "Requirements Header Mismatch");
            Assert.AreEqual("Appr", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[3].Text, "Requirements Header Mismatch");
            Assert.AreEqual("Cond", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[4].Text, "Requirements Header Mismatch");
            Assert.AreEqual("Pend", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[5].Text, "Requirements Header Mismatch");
            Assert.AreEqual("Expr", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[6].Text, "Requirements Header Mismatch");
            Assert.AreEqual("Not Appr", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[7].Text, "Requirements Header Mismatch");

            //Verify Footer
            Assert.Contains(test.Driver.FindElementByXPath("//div[@class='grid_pagination_details']").Text, "requirements");

            //Logout
            test.Portal.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Side bar can be collapsed and expanded")]
        public void Ministry_Activities_ViewAll_Activity_Requirements_SideBar_Collapse()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Setup
            string activityName = "A Test Activity";
            string ministryName = "A Test Ministry";

            // Navigate to Activity View All Requirments Tab 
            test.Portal.Ministry_Activities_View_All_Requirements(ministryName, activityName);

            //Verify show more Toggle is functioning properly
            Assert.AreEqual("HIDE SIDEBAR", test.Driver.FindElementByCssSelector("[style=''][data-hide-on-expand='yes']").Text, "Hide Sidebar not displayed");
            test.Driver.FindElementById("expand_collapse").Click();
            Assert.AreEqual("SHOW SIDEBAR", test.Driver.FindElementByCssSelector("[style='display: inline;'][data-show-on-expand='yes']").Text, "Show Sidebar not displayed");

            //Verify Table Expanded 
            IWebElement requirementsTable = test.Driver.FindElementByXPath(TableIds.Ministry_Activities_ViewAll_Requirements);
            Assert.AreEqual("Name", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[0].Text, "Requirements Header Mismatch");
            Assert.AreEqual("Missing", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[1].Text, "Requirements Header Mismatch");
            Assert.AreEqual("Completed", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[2].Text, "Requirements Header Mismatch");
            Assert.AreEqual("Approved", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[3].Text, "Requirements Header Mismatch");
            Assert.AreEqual("Conditional", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[4].Text, "Requirements Header Mismatch");
            Assert.AreEqual("Pending", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[5].Text, "Requirements Header Mismatch");
            Assert.AreEqual("Expired", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[6].Text, "Requirements Header Mismatch");
            Assert.AreEqual("Not Approved", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[7].Text, "Requirements Header Mismatch");

            //Verify page back to normal  
            test.Driver.FindElementById("expand_collapse").Click();
            Assert.AreEqual("Name", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[0].Text, "Requirements Header Mismatch");
            Assert.AreEqual("Miss", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[1].Text, "Requirements Header Mismatch");
            Assert.AreEqual("Comp", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[2].Text, "Requirements Header Mismatch");
            Assert.AreEqual("Appr", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[3].Text, "Requirements Header Mismatch");
            Assert.AreEqual("Cond", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[4].Text, "Requirements Header Mismatch");
            Assert.AreEqual("Pend", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[5].Text, "Requirements Header Mismatch");
            Assert.AreEqual("Expr", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[6].Text, "Requirements Header Mismatch");
            Assert.AreEqual("Not Appr", requirementsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[7].Text, "Requirements Header Mismatch");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        //[Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies the Requirements Counts")]
        public void Ministry_Activities_ViewAll_Activity_Requirements_VerifyCounts()
        {
            //Login 
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Setup
            string activityName = "A Test Activity";
            string ministryName = "A Test Ministry";

            // Navigate to Activity View All Requirments Tab 
            test.Portal.Ministry_Activities_View_All_Requirements(ministryName, activityName);

            Dictionary<string, string> requirementsCount = test.SQL.Ministry_Activities_Activity_Requirements(15, "A Test Activity", "A Test Requirement");

            int row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Ministry_Activities_ViewAll_Requirements, "A Test Requirement", "Name");

            IWebElement requirementTable = test.Driver.FindElementByXPath(TableIds.Ministry_Activities_ViewAll_Requirements);
            Assert.AreEqual("A Test Requirement", requirementTable.FindElements(By.TagName("tr"))[row].FindElements(By.TagName("td"))[0].Text);
            Assert.AreEqual(requirementsCount["Missing"], requirementTable.FindElements(By.TagName("tr"))[row].FindElements(By.TagName("td"))[1].Text);
            Assert.AreEqual(requirementsCount["Approved"], requirementTable.FindElements(By.TagName("tr"))[row].FindElements(By.TagName("td"))[3].Text);
            Assert.AreEqual(requirementsCount["Pending"], requirementTable.FindElements(By.TagName("tr"))[row].FindElements(By.TagName("td"))[5].Text);
            Assert.AreEqual(requirementsCount["NotApproved"], requirementTable.FindElements(By.TagName("tr"))[row].FindElements(By.TagName("td"))[7].Text);
            Assert.AreEqual(requirementsCount["Expired"], requirementTable.FindElements(By.TagName("tr"))[row].FindElements(By.TagName("td"))[6].Text);
            Assert.AreEqual(requirementsCount["Completed"], requirementTable.FindElements(By.TagName("tr"))[row].FindElements(By.TagName("td"))[2].Text);
            Assert.AreEqual(requirementsCount["Conditional"], requirementTable.FindElements(By.TagName("tr"))[row].FindElements(By.TagName("td"))[4].Text);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies the Add Requirment button in functioning properly")]
        public void Ministry_Activities_ViewAll_Activity_Requirements_Add()
        {
            //Login 
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Setup
            string activityName = "A Test Activity";
            string ministryName = "A Test Ministry";

            // Navigate to Activity View All Requirments Tab 
            test.Portal.Ministry_Activities_View_All_Requirements(ministryName, activityName);

            //Check for Requirement
            string requirementName = "Newly Created Requirement";
            if (test.GeneralMethods.IsElementVisibleWebDriver(By.LinkText(requirementName)))
            {
                test.Portal.Ministry_Activities_View_All_Requirements_Delete(requirementName);
            }

            // Add requirement
            test.Portal.Ministry_Activities_View_All_Requirements_Add(requirementName);
            
            //Verify Requirement Present
            Assert.IsTrue(test.Driver.FindElementByLinkText(requirementName).Displayed, "Requirements link is not displayed");

            // Delete Requirement
            test.Portal.Ministry_Activities_View_All_Requirements_Delete(requirementName);

            //Logout
            test.Portal.LogoutWebDriver();
        }

        //[Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies the Add and Save Another button")]
        public void Ministry_Activities_ViewAll_Activity_Requirements_AddAnother()
        {
            //Login 
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Setup
            string activityName = "A Test Activity";
            string ministryName = "A Test Ministry";

            // Navigate to Activity View All Requirments Tab 
            test.Portal.Ministry_Activities_View_All_Requirements(ministryName, activityName);

            //Check for Requirement
            string requirementName1 = "Newly Created Requirement -- 1";
            string requirementName2 = "Newly Created Requirement -- 2";

            if (test.GeneralMethods.IsElementVisibleWebDriver(By.LinkText(requirementName1)))
            {
                test.Portal.Ministry_Activities_View_All_Requirements_Delete(requirementName1);
            }

            if (test.GeneralMethods.IsElementVisibleWebDriver(By.LinkText(requirementName2)))
            {
                test.Portal.Ministry_Activities_View_All_Requirements_Delete(requirementName2);
            }

            //Click Add
            test.Driver.FindElementByLinkText(GeneralMinistry.Activities.Add_Requirement).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.Id(GeneralMinistry.Activities.Requirement_Name_Input));

            //Enter Requirement Name
            test.Driver.FindElementById(GeneralMinistry.Activities.Requirement_Name_Input).SendKeys(requirementName1);

            //Click Save and Add another
            test.Driver.FindElementByXPath(GeneralMinistry.Activities.Requirement_Save_And_Add_Button).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.Id(GeneralMinistry.Activities.Requirement_Name_Input));

            //Enter second Requirement Name
            test.Driver.FindElementById(GeneralMinistry.Activities.Requirement_Name_Input).SendKeys(requirementName2);

            //Save second requirement
            test.Driver.FindElementByXPath(GeneralMinistry.Activities.Requirement_Save_Button).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.Id(GeneralMinistry.Activities.Requirements_Schedule_DropDown));

            //Verify Requirement Present
            Assert.IsTrue(test.Driver.FindElementByLinkText(requirementName1).Displayed, "Requirements link is not displayed");
            Assert.IsTrue(test.Driver.FindElementByLinkText(requirementName2).Displayed, "Requirements link is not displayed");

            // Delete Requirement
            test.Portal.Ministry_Activities_View_All_Requirements_Delete(requirementName1);
            test.Portal.Ministry_Activities_View_All_Requirements_Delete(requirementName2);

            //Logout
            test.Portal.LogoutWebDriver();
        }

        //[Test, RepeatOnFailure, MultipleAsserts]
        [Author("Stuart Platt")]
        [Description("Verify that the requirements checkboxes are functioning properly")]
        public void Ministry_Activities_ViewAll_Activity_Requirements_Inactive()
        {
            //Login 
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Setup
            string activityName = "*Requirements Active/Inactive";
            string ministryName = "A Test Ministry";
            string requirement = "Active Requirement";
            string inactiveRequirement = "Inactive Requirement";
            string footer = "//div[@class='grid_pagination_details']";
            
            // Navigate to Activity View All Requirments Tab 
            test.Portal.Ministry_Activities_View_All_Requirements(ministryName, activityName);

            //Verify that the active requirement is displayed
            Assert.IsTrue(test.Driver.FindElementByLinkText(requirement).Displayed, "Active Requirements link is not displayed");
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.LinkText(inactiveRequirement));
            //Assert.Contains(test.Driver.FindElementByXPath(footer).Text, "1 requirements");

            //Check inactive check box 
            test.Driver.FindElementById(GeneralMinistry.Activities.Inactive_Requirements_CheckBox).Click();

            //Verify that both active and inactive requirements are displayed
            Assert.IsTrue(test.Driver.FindElementByLinkText(inactiveRequirement).Displayed, "Inactive Requirements link is not displayed");
            Assert.IsTrue(test.Driver.FindElementByLinkText(requirement).Displayed, "Active Requirements link is not displayed");
            //Assert.Contains(test.Driver.FindElementByXPath(footer).Text, "2 requirements");
            
            //Deselect active check box 
            test.Driver.FindElementById(GeneralMinistry.Activities.Active_RequirementsCheckBox).Click();

            //Verify only inactive requirements displayed
            Assert.IsTrue(test.Driver.FindElementByLinkText(inactiveRequirement).Displayed, "Inactive Requirements link is not displayed");
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.LinkText(requirement));
            //Assert.Contains(test.Driver.FindElementByXPath(footer).Text, "1 requirements");

            //Deslect the Inactive Check box
            test.Driver.FindElementById(GeneralMinistry.Activities.Inactive_Requirements_CheckBox).Click();

            //Verify both active and inactive are displayed
            Assert.IsTrue(test.Driver.FindElementByLinkText(inactiveRequirement).Displayed, "Incactive Requirements link is not displayed");
            Assert.IsTrue(test.Driver.FindElementByLinkText(requirement).Displayed, "Active Requirements link is not displayed");
            //Assert.Contains(test.Driver.FindElementByXPath(footer).Text, "2 requirements");

            //Logout 
            test.Portal.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verify that the Inactivate action option is functioning as designed")]
        public void Ministry_Activities_ViewAll_Activity_Requirements_Inactivate()
        {
            //Setup
            string requirementName = "Requirement to make inactive";
            string activityName = "A Test Activity";
            string ministryName = "A Test Ministry";
                        
            //Login 
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            //Navigate to Requirements tab
            test.Portal.Ministry_Activities_View_All_Requirements(ministryName, activityName);

            if (test.GeneralMethods.IsElementVisibleWebDriver(By.LinkText(requirementName)))
            {
                test.Portal.Ministry_Activities_View_All_Requirements_Delete(requirementName);
            }
            
            //Add a new requirement
            test.Portal.Ministry_Activities_View_All_Requirements_Add(requirementName);

            //Verify Activity was created
            Assert.IsTrue(test.Driver.FindElementByLinkText(requirementName).Displayed, "Active Requirements link is not displayed");

            //Click Inactivate option from Action Gear
            int row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Ministry_Activities_ViewAll_Requirements, requirementName, "Name");
            test.GeneralMethods.SelectOptionFromGearWebDriver(row -1, "Inactivate");

            //Verfiy that the Requirement is listed as inactive
            test.Driver.FindElementById(GeneralMinistry.Activities.Inactive_Requirements_CheckBox).Click();
            Assert.IsTrue(test.Driver.FindElementByLinkText(requirementName).Displayed, "Active Requirements link is not displayed");

            //Clean Up
            test.Portal.Ministry_Activities_View_All_Requirements_Delete(requirementName);

            //Logout
            test.Portal.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Edit Action option is functioning")]
        public void Ministry_Activities_ViewAll_Activity_Requirements_Edit()
        {
            string requirementName = "This Requirement Needs Editing";
            string requirementEdit = "This Requirement was Edited";
            string activityName = "A Test Activity";
            string ministryName = "A Test Ministry";

            //Login 
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            //Navigate to Requirements tab
            test.Portal.Ministry_Activities_View_All_Requirements(ministryName, activityName);

            if (test.GeneralMethods.IsElementVisibleWebDriver(By.LinkText(requirementName)))
            {
                test.Portal.Ministry_Activities_View_All_Requirements_Delete(requirementName);
            }
            
            if (test.GeneralMethods.IsElementVisibleWebDriver(By.LinkText(requirementEdit)))
            {
                test.Portal.Ministry_Activities_View_All_Requirements_Delete(requirementEdit);
            }

            //Add a new requirement
            test.Portal.Ministry_Activities_View_All_Requirements_Add(requirementName);

            //Verify Activity was created
            Assert.IsTrue(test.Driver.FindElementByLinkText(requirementName).Displayed, "Active Requirements link is not displayed");

            //Click Inactivate option from Action Gear
            int row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Ministry_Activities_ViewAll_Requirements, requirementName, "Name");
            test.GeneralMethods.SelectOptionFromGearWebDriver(row - 1, "Edit");
            test.GeneralMethods.WaitForElement(test.Driver, By.Id(GeneralMinistry.Activities.Requirement_Name_Input));

            //Edit Name
            test.Driver.FindElementById(GeneralMinistry.Activities.Requirement_Name_Input).Clear();
            test.Driver.FindElementById(GeneralMinistry.Activities.Requirement_Name_Input).SendKeys(requirementEdit);

            //Save
            test.Driver.FindElementByXPath(GeneralMinistry.Activities.Requirement_Save_Button).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.Id(GeneralMinistry.Activities.Requirements_Schedule_DropDown));

            //Verify Requirement was edited. 
            Assert.IsTrue(test.Driver.FindElementByLinkText(requirementEdit).Displayed, "Active Requirements link is not displayed");

            //Cleanup
            test.Portal.Ministry_Activities_View_All_Requirements_Delete(requirementEdit);

            //Logout
            test.Portal.LogoutWebDriver();
            
        }

        // [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies the View Assignments action from the Requirements Tab")]
        public void Ministry_Activities_ViewAll_Activity_Requirements_ViewAssignments()
        {
            string activityName = "A Test Activity";
            string ministryName = "A Test Ministry";

            //Login 
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            //Navigate to Requirements tab
            test.Portal.Ministry_Activities_View_All_Requirements(ministryName, activityName);

            //Click on the View Assignments Action option
            int row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Ministry_Activities_ViewAll_Requirements, "A Test Requirement", "Name");
            test.GeneralMethods.SelectOptionFromGearWebDriver(row - 1, "View assignments");
            
            //Did we make it to the Assignments View all page?
            test.GeneralMethods.WaitForElement(test.Driver, By.Id(string.Format("{0}0", GeneralMinistry.Assignments.Activity_Dropdown)));
            Assert.IsTrue(test.Driver.FindElementByLinkText(GeneralMinistry.Assignments.Reset_Link).Displayed, "Reset link is not displayed");
            Assert.IsTrue(test.Driver.FindElementByXPath(GeneralMinistry.Assignments.Apply_Button).Displayed, "General button is not displayed");
            Assert.IsTrue(test.Driver.FindElementById(GeneralMinistry.Assignments.Participant_Checkbox).Selected, "Participant Check Box should be selected by default");
            Assert.IsTrue(test.Driver.FindElementById(GeneralMinistry.Assignments.Volunteer_Checkbox).Selected, "Volunteer / Staff Check Box should be selected by default");
            Assert.IsFalse(test.Driver.FindElementById(GeneralMinistry.Assignments.Inactive_Staff_Checkbox).Selected, "Inactive Staff Check Box should not be selected by default");

            //Verify that the correct Activity was imported to the assignment page. 
            Assert.AreEqual(activityName, new SelectElement(test.Driver.FindElementById(string.Format("{0}0", GeneralMinistry.Assignments.Activity_Dropdown))).SelectedOption.Text.Trim(), "The proper activity was not selected");
            
            //Logout 
            test.Portal.LogoutWebDriver();

        }
        
        #endregion Requirements

        #region Breakout Groups
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("F1-2170: Verifies you can access the Breakout Groups listing page")]
        public void Ministry_Activities_ViewAll_Activity_BreakoutGroups()
        {
            
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Breakout Groups tab for an activity
            test.Portal.Ministry_Activities_BreakoutGroups_View(ministryName, activityName);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies you can Add a new Breakout Group")]
        public void Ministry_Activities_ViewAll_Activity_BreakoutGroups_Add()
        {
            // Data
            string ministryName = "Auto Test Ministry";
            string activityName = "Breakout Group - Add";
            string breakoutGroupName = "BG - Add";
            string rosterName = "BG - Attendees";
            int ministryId = base.SQL.Ministry_Ministries_FetchID(15, ministryName);
            base.SQL.Ministry_Activities_Create(15, ministryId, activityName, rosterName);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Breakout Groups tab for an activity
            test.Portal.Ministry_Activities_BreakoutGroups_View(ministryName, activityName);

            // Add a breakout group to an activity
            test.Portal.Ministry_Activities_BreakoutGroups_AddEdit(true, breakoutGroupName);

            // Logout of portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Ministry_Activities_BreakoutGroups_Delete(15, activityName, breakoutGroupName);
            base.SQL.Ministry_Activities_Delete_Activity(15, activityName);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies you can Edit an existing Breakout Group")]
        public void Ministry_Activities_ViewAll_Activity_BreakoutGroups_Edit()
        {
            // Data
            string ministryName = "Auto Test Ministry";
            string activityName = "Breakout Group - Edit";
            string breakoutGroupName = "BG - Edit";
            string newBreakoutGroupName = "Edited BG";
            string rosterName = "BG - Attendees";
            int ministryId = base.SQL.Ministry_Ministries_FetchID(15, ministryName);
            base.SQL.Ministry_Activities_Create(15, ministryId, activityName, rosterName);
            base.SQL.Ministry_Activities_BreakoutGroups_Create(15, activityName, breakoutGroupName);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Breakout Groups tab for an activity
            test.Portal.Ministry_Activities_BreakoutGroups_View(ministryName, activityName);

            // Edit a breakout group
            test.Portal.Ministry_Activities_BreakoutGroups_Edit(breakoutGroupName, newBreakoutGroupName, "Edited: Yay");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Ministry_Activities_BreakoutGroups_Delete(15, activityName, newBreakoutGroupName);
            base.SQL.Ministry_Activities_Delete_Activity(15, activityName);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies you can Delete a Breakout Group")]
        public void Ministry_Activities_ViewAll_Activity_BreakoutGroups_Delete_NO_Assignments()
        {
            // Data
            string ministryName = "Auto Test Ministry";
            string activityName = "BG - Delete - No Assign";
            string breakoutGroupName = "BG-Delete-No Ass.";
            string rosterName = "BG - Attendees";
            int ministryId = base.SQL.Ministry_Ministries_FetchID(15, ministryName);
            base.SQL.Ministry_Activities_Create(15, ministryId, activityName, rosterName);
            base.SQL.Ministry_Activities_BreakoutGroups_Create(15, activityName, breakoutGroupName);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Breakout Groups tab for an activity
            test.Portal.Ministry_Activities_BreakoutGroups_View(ministryName, activityName);

            // Delete breakout group
            test.Portal.Ministry_Activities_BreakoutGroups_Delete(breakoutGroupName, false);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Ministry_Activities_BreakoutGroups_Delete(15, activityName, breakoutGroupName);
            base.SQL.Ministry_Activities_Delete_Activity(15, activityName);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies you cannot delete a Breakout Group when assignments are tied to it")]
        public void Ministry_Activities_ViewAll_Activity_BreakoutGroups_Delete_WITH_Assignments()
        {
            // Data
            string ministryName = "Auto Test Ministry";
            string activityName = "BG - Delete - WITH Assign";
            string breakoutGroupName = "BG-Delete-WITH Ass.";
            string rosterName = "BG - Attendees";
            string individualName = "FT Tester";
            int individualId = base.SQL.People_Individuals_FetchID(15, individualName);
            int ministryId = base.SQL.Ministry_Ministries_FetchID(15, ministryName);
            base.SQL.Ministry_Activities_Create(15, ministryId, activityName, rosterName);
            base.SQL.Ministry_Activities_BreakoutGroups_Create(15, activityName, breakoutGroupName);
            base.SQL.Ministry_ParticipantAssignments_Create(15, individualId, activityName, rosterName, breakoutGroupName);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Breakout Groups tab for an activity
            test.Portal.Ministry_Activities_BreakoutGroups_View(ministryName, activityName);

            // Delete breakout group
            test.Portal.Ministry_Activities_BreakoutGroups_Delete(breakoutGroupName, true);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Ministry_ParticipantAssignments_Delete(15, individualId, activityName, rosterName);
            base.SQL.Ministry_Activities_BreakoutGroups_Delete(15, activityName, breakoutGroupName);
            base.SQL.Ministry_Activities_Delete_Activity(15, activityName);

        }

        #region Assignments
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("F1-2170: Verifies you can click on the Breakout Group name and get transported to the edit page")]
        public void Ministry_Activities_ViewAll_Activity_BreakoutGroups_Assignments_NameLink()
        {
            // Data
            string breakoutGroup = "A Test Breakout";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Breakout Groups tab for an activity
            test.Portal.Ministry_Activities_BreakoutGroups_View(ministryName, activityName);

            // Click on the name of a breakout group
            test.Driver.FindElementByLinkText(breakoutGroup).Click();
            test.GeneralMethods.WaitForElement(By.LinkText("Cancel"));

            // Verify you are on the Edit page
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id("breakout_group_name"));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id("breakout_group_description"));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id("breakout_group_tag_code"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-2170: Verifies you can access the Assignments page from the gear icon")]
        public void Ministry_Activities_ViewAll_Activity_BreakoutGroups_Assignments_GearIcon()
        {
            // Data
            string breakoutGroup = "A Test Breakout";
            string activityName = "Assign Schedule Level";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Breakout Groups tab for an activity
            test.Portal.Ministry_Activities_BreakoutGroups_View(ministryName, activityName);

            // Click on the gear of a breakout group and select View Assignments
            int itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Ministry_Activities_BreakoutGroups, breakoutGroup, "Name");
            string gearOption = "View Assignments";
            //test.GeneralMethods.SelectOptionFromGearWebDriver(itemRow, "View Assignments");
            test.Driver.FindElementByXPath(string.Format("//table[*]/tbody/tr[{0}]/td[*]/span[@class='']/a|//table[*]/tbody/tr[{0}]/td[*]/div[@data-menu-type='gear']/a[text()='Options']", itemRow + 1)).Click();
            test.Driver.FindElementByXPath(string.Format("//table[*]/tbody/tr[{0}]/td[*]/span/ul/li[*]/a[text()='{1}']|//table[*]/tbody/tr[{0}]/td[*]/ul/li[*]/a[text()='{1}']|//table/tbody/tr[{0}]/td[*]/div/ul/li[*]/a[contains(text(), '{1}')]", itemRow + 1, gearOption)).Click();
            test.GeneralMethods.WaitForElement(By.CssSelector("[style=''][data-show-on-expand='yes']"));

            // Verify you are on the Assignments page
            test.Portal.Ministry_Assignments_VerifyPageLoad();
            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath(string.Format("//span[text()='{0}']", breakoutGroup)));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//span[@class='attendance_participant']"));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//span[@class='attendance_staff']"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-2170: Verifies the blue pill transports you to the Assignments page for just Participants")]
        public void Ministry_Activities_ViewAll_Activity_BreakoutGroups_Assignments_Pill_Participant()
        {
            // Data
            string breakoutGroup = "A Test Breakout";
            string activityName = "Assign Schedule Level";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Breakout Groups tab for an activity
            test.Portal.Ministry_Activities_BreakoutGroups_View(ministryName, activityName);

            // Click on the blue assignment pill to view participant assignments
            int itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Ministry_Activities_BreakoutGroups, breakoutGroup, "Name");
            test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[4]/a/span", TableIds.Ministry_Activities_BreakoutGroups, itemRow + 1)).Click();
            test.GeneralMethods.WaitForElement(By.CssSelector("[style=''][data-show-on-expand='yes']"));

            // Verify you are on the Assignments page
            test.Portal.Ministry_Assignments_VerifyPageLoad();
            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath(string.Format("//span[text()='{0}']", breakoutGroup)));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//span[@class='attendance_participant']"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-2170: Verifies the red pill transports you to the Assignments page for just Staff")]
        public void Ministry_Activities_ViewAll_Activity_BreakoutGroups_Assignments_Pill_Staff()
        {
            // Data
            string breakoutGroup = "A Test Breakout";
            string activityName = "Assign Schedule Level";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Breakout Groups tab for an activity
            test.Portal.Ministry_Activities_BreakoutGroups_View(ministryName, activityName);

            // Click on the blue assignment pill to view participant assignments
            int itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Ministry_Activities_BreakoutGroups, breakoutGroup, "Name");
            test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[3]/a/span", TableIds.Ministry_Activities_BreakoutGroups, itemRow + 1)).Click();
            test.GeneralMethods.WaitForElement(By.CssSelector("[style=''][data-show-on-expand='yes']"));

            // Verify you are on the Assignments page
            test.Portal.Ministry_Assignments_VerifyPageLoad();
            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath(string.Format("//span[text()='{0}']", breakoutGroup)));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//span[@class='attendance_staff']"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Assignments

        #endregion Breakout Groups

        #region Staff/Volunteer Schedules
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("F1-3867: Verifies a user can access the staff/Volunteer Schedules tab")]
        public void Ministry_Activities_ViewAll_Activity_StaffSchedules()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Staff & Volunteer Schedules tab for an activity
            test.Portal.Ministry_Activities_StaffSchedules_View(ministryName, activityName);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3868: Verifies a user can add a new Staff/Volunteer schedule")]
        public void Ministry_Activities_ViewAll_Activity_StaffSchedules_Add_Once()
        {

            // Data
            string scheduleName = "Test Staff Schedule - Once";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Staff & Volunteer Schedules tab for an activity
            test.Portal.Ministry_Activities_StaffSchedules_View(ministryName, activityName);

            // Add a new staff schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Ministry_Activities_StaffSchedules_Add_Once(15, scheduleName, today.AddDays(1).ToShortDateString());

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Ministry_StaffingSchedules_Delete(15, activityName, scheduleName);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3868: Verifies a user can add a new Staff/Volunteer schedule")]
        public void Ministry_Activities_ViewAll_Activity_StaffSchedules_Add_Daily()
        {

            // Data
            string scheduleName = "Test Staff Schedule - Daily";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Staff & Volunteer Schedules tab for an activity
            test.Portal.Ministry_Activities_StaffSchedules_View(ministryName, activityName);

            // Add a new staff schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Ministry_Activities_StaffSchedules_Add_Daily(15, scheduleName, today.AddDays(1).ToShortDateString(), "1");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Ministry_StaffingSchedules_Delete(15, activityName, scheduleName);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3868: Verifies a user can add a new Staff/Volunteer schedule")]
        public void Ministry_Activities_ViewAll_Activity_StaffSchedules_Add_Weekly()
        {

            // Data
            string scheduleName = "Test Staff Schedule - Weekly";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Staff & Volunteer Schedules tab for an activity
            test.Portal.Ministry_Activities_StaffSchedules_View(ministryName, activityName);

            // Add a new staff schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Ministry_Activities_StaffSchedules_Add_Weekly(15, scheduleName, today.AddDays(1).ToShortDateString(), "1", new GeneralEnumerations.WeeklyScheduleDays[] { GeneralEnumerations.WeeklyScheduleDays.Tuesday });

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Ministry_StaffingSchedules_Delete(15, activityName, scheduleName);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3868: Verifies a user can add a new Staff/Volunteer schedule")]
        public void Ministry_Activities_ViewAll_Activity_StaffSchedules_Add_Monthly()
        {

            // Data
            string scheduleName = "Test Staff Schedule - Monthly";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Staff & Volunteer Schedules tab for an activity
            test.Portal.Ministry_Activities_StaffSchedules_View(ministryName, activityName);

            // Add a new staff schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Ministry_Activities_StaffSchedules_Add_Monthly(15, scheduleName, today.AddDays(1).ToShortDateString(), "1", "1st");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Ministry_StaffingSchedules_Delete(15, activityName, scheduleName);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3868: Verifies a user can add a new Staff/Volunteer schedule")]
        public void Ministry_Activities_ViewAll_Activity_StaffSchedules_Add_Yearly()
        {

            // Data
            string scheduleName = "Test Staff Schedule - Yearly";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Staff & Volunteer Schedules tab for an activity
            test.Portal.Ministry_Activities_StaffSchedules_View(ministryName, activityName);

            // Add a new staff schedule
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Ministry_Activities_StaffSchedules_Add_Yearly(15, scheduleName, today.AddDays(1).ToShortDateString(), false);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Ministry_StaffingSchedules_Delete(15, activityName, scheduleName);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3868: Verifies a user can edit a Staff/Volunteer schedule")]
        public void Ministry_Activities_ViewAll_Activity_StaffSchedules_Edit()
        {
            // Data
            string scheduleName = "Test Staff Schedule - Edit";
            string newScheduleName = "EDITED - Staff Schedule";
            base.SQL.Ministry_StaffingSchedules_Delete(15, activityName, scheduleName);
            base.SQL.Ministry_StaffingSchedules_Delete(15, activityName, newScheduleName);
            base.SQL.Ministry_StaffingSchedules_Create(15, activityName, scheduleName);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            try
            {
                test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

                // Navigate to the Staff & Volunteer Schedules tab for an activity
                test.Portal.Ministry_Activities_StaffSchedules_View(ministryName, activityName);

                // Edit staff schedule
                test.Portal.Ministry_Activities_StaffSchedules_Edit(15, activityName, scheduleName, newScheduleName, true, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(2).ToShortDateString());

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            finally
            {
                // Clean up
                base.SQL.Ministry_StaffingSchedules_Delete(15, activityName, scheduleName);
                base.SQL.Ministry_StaffingSchedules_Delete(15, activityName, newScheduleName);
            }
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-4114: Verifies a user can delete a Staff/Volunteer schedule")]
        public void Ministry_Activities_ViewAll_Activity_StaffSchedules_Delete_WITH_Assignment()
        {
            // Data
            string individualName = "FT Tester";
            string staffingScheduleName = "Staff Schedule - DWA";
            int individualId = base.SQL.People_Individuals_FetchID(15, individualName);
            base.SQL.Ministry_StaffingSchedules_Delete(15, activityName, staffingScheduleName);
            base.SQL.Ministry_StaffingSchedules_Create(15, activityName, staffingScheduleName);
            base.SQL.Ministry_StaffingAssignments_Create(15, individualId, 2, activityName, staffingScheduleName);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            try
            {
                test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

                // Navigate to the Staff & Volunteer Schedules tab for an activity
                test.Portal.Ministry_Activities_StaffSchedules_View(ministryName, activityName);

                // Delete Staff Schedule
                test.Portal.Ministry_Activities_StaffSchedules_Delete(ministryName, activityName, staffingScheduleName, true, individualId);

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            finally
            {
                base.SQL.Ministry_StaffingSchedules_Delete(15, activityName, staffingScheduleName);
            }
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-4114: Verifies a user can delete a Staff/Volunteer schedule")]
        public void Ministry_Activities_ViewAll_Activity_StaffSchedules_Delete_NO_Assignment()
        {
            // Data
            string individualName = "FT Tester";
            string staffingScheduleName = "Staff Schedule - DHA";
            int individualId = base.SQL.People_Individuals_FetchID(15, individualName);
            base.SQL.Ministry_StaffingSchedules_Delete(15, activityName, staffingScheduleName);
            base.SQL.Ministry_StaffingSchedules_Create(15, activityName, staffingScheduleName);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Staff & Volunteer Schedules tab for an activity
            test.Portal.Ministry_Activities_StaffSchedules_View(ministryName, activityName);

            // Delete Staff Schedule
            test.Portal.Ministry_Activities_StaffSchedules_Delete(ministryName, activityName, staffingScheduleName);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3867: Verifies a user can click on the staff assignment pill and be transported to the Assignments page")]
        public void Ministry_Activities_ViewAll_Activity_StaffSchedules_AssignmentPill_Staff()
        {
            // Data
            string staffScheduleName = "Base Schedule";
            string activityName = "**no checkin";
            int activityId = base.SQL.Ministry_Activities_FetchID(15, activityName);
            int staffScheduleId = base.SQL.Ministry_StaffingSchedules_FetchID(15, activityId, staffScheduleName);
            int staffCount = base.SQL.Ministry_Assignments_GetStaffAssignmentForStaffScheduleCount(activityId, staffScheduleId);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Staff & Volunteer Schedules tab for an activity
            test.Portal.Ministry_Activities_StaffSchedules_View(ministryName, activityName);

            // Click on the assignment pill for a staff schedule
            test.GeneralMethods.WaitForElementDisplayed(By.XPath(string.Format("//div[@id='schedule_{0}']/span[@class='float_right gutter_right']/a/span", staffScheduleId)));
            Assert.AreEqual(staffCount.ToString(), test.Driver.FindElementByXPath(string.Format("//div[@id='schedule_{0}']/span[@class='float_right gutter_right']/a/span", staffScheduleId)).Text);
            test.Driver.FindElementByXPath(string.Format("//div[@id='schedule_{0}']/span[@class='float_right gutter_right']/a", staffScheduleId)).Click();

            // Verify you are on the Assignments page
            test.GeneralMethods.WaitForElement(By.XPath(GeneralMinistry.Assignments.Select_All_CheckBox));
            Assert.AreEqual("SHOW SIDEBAR", test.Driver.FindElementByCssSelector("[style=''][data-show-on-expand='yes']").Text, "Show Sidebar not displayed");
            test.Driver.FindElementById("expand_collapse").Click();
            test.GeneralMethods.WaitForElementEnabled(By.XPath(GeneralMinistry.Assignments.Apply_Button));
            Assert.AreEqual("HIDE SIDEBAR", test.Driver.FindElementByCssSelector("[style='display: inline;'][data-hide-on-expand='yes']").Text, "Hide Sidebar not displayed");
            Assert.AreEqual(activityName, new SelectElement(test.Driver.FindElementById(string.Format("{0}-1", GeneralMinistry.Assignments.Activity_Dropdown))).SelectedOption.Text.Trim(), "Activity Name does not match");
            Assert.AreEqual(staffScheduleName, new SelectElement(test.Driver.FindElementById(string.Format("{0}-1", GeneralMinistry.Assignments.Staff_Schedule_DropDown))).SelectedOption.Text.Trim(), "Staff Schedule does not match");
            
            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        #endregion Staff/Volunteer Schedules

        #region Properties
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("F1-3882: Verifies you can edit an activity and save with all properties checked")]
        public void Ministry_Activities_ViewAll_Activity_Properties_Edit_AddEverything()
        {
            // Data
            int churchId = 15;
            //string ministryName = "A Test Ministry";
            string activityName = "*Properties Edit Test";
            string activityType = "Automation Activity";
            int checkinCode = base.SQL.Ministry_CheckIn_FetchCheckInCode(15, activityName);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Properties Edit page
            test.Portal.Ministry_Activities_Properties_EditPage(churchId, ministryName, activityName);

            // Edit the basic information
            test.Portal.Ministry_Activities_AddEditSettings_BasicInformation(activityName, null, true, null, true, activityType);

            // Edit the Check-in settings
            test.Portal.Ministry_Activities_AddEditSettings_EnableCheckin(true, true, true, true, true, false);

            // Edit the age restrictions
            test.Portal.Ministry_Activities_AddEditSettings_EnableAgeRestrictions(true, "1", "99");

            // Edit enforce assignment creation rules
            test.Portal.Ministry_Activities_AddEditSettings_EnforceAssignmentCreationRules(true, true, GeneralEnumerations.ActivityEnforceAssignmentCreationType.Schedule, true, GeneralEnumerations.ActivityEnforceAssignmentCreationType.Schedule, true, GeneralEnumerations.ActivityEnforceAssignmentAutoCreate.FirstAttendance);

            // Edit use as weblink groups
            test.Portal.Ministry_Activities_AddEditSettings_UseAsWebLinkGroups(true, "18-24");

            // Save Changes
            test.Driver.FindElementById(GeneralMinistry.Activities.Im_Done).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(GeneralMinistry.Activities.Manage_Staffing_Needs));
            //test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Delete this activity"));

            // Verify Properties Sidebar            
            Dictionary<string, string> properties = new Dictionary<string, string> { 
              {GeneralMinistry.Activity.Property.Ministry, ministryName},
              {GeneralMinistry.Activity.Property.Activity_Type, activityType},
              {GeneralMinistry.Activity.Property.Age, "1 to 99"},
              {GeneralMinistry.Activity.Property.Confidential, GeneralMinistry.Activity.Property.Yes},
              {GeneralMinistry.Activity.Property.Volunteers_Staff, "Activity Schedule"},
              {GeneralMinistry.Activity.Property.Participants, "Activity Schedule"},
              {GeneralMinistry.Activity.Property.Auto_Assignment, "First Attendance"},
              {GeneralMinistry.Activity.Property.Weblink_Groups, "18-24"},
              {GeneralMinistry.Activity.Property.Active, GeneralMinistry.Activity.Property.Yes} 
            };

            test.Portal.Ministry_Activities_Activity_Property_Verify(properties);

            // Verify Check-in Sidebar
            IWebElement checkinTable = test.Driver.FindElementByXPath(TableIds.Ministry_Activity_CheckInTable);
            Assert.AreEqual("Code", checkinTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[0].Text);
            Assert.AreEqual(checkinCode, Convert.ToInt32(checkinTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("td"))[0].FindElements(By.TagName("span"))[0].Text));
            Assert.AreEqual("Name Tag", checkinTable.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Parent Receipt", checkinTable.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Closed Room Override", checkinTable.FindElements(By.TagName("tr"))[3].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[3].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Assignment Required", checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Alert", checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("td"))[0].FindElements(By.TagName("span"))[0].Text);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [DependsOn("Ministry_Activities_ViewAll_Activity_Properties_Edit_AddEverything")]
        [Author("David Martin")]
        [Description("F1-3882: Verifies you can edit an activity description")]
        public void Ministry_Activities_ViewAll_Activity_Properties_Edit_Description()
        {
            // Data
            int churchId = 15;
            //string ministryName = "A Test Ministry";
            string activityName = "*Properties Edit Test";
            string description = "This is an activity description";
            string activityType = "Automation Activity";
            int checkinCode = base.SQL.Ministry_CheckIn_FetchCheckInCode(15, activityName);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Properties Edit page
            test.Portal.Ministry_Activities_Properties_EditPage(churchId, ministryName, activityName);

            // Edit the basic information
            test.Portal.Ministry_Activities_AddEditSettings_BasicInformation(activityName, null, true, description, true, activityType);

            // Edit the Check-in settings
            test.Portal.Ministry_Activities_AddEditSettings_EnableCheckin(true, true, true, true, true, false);

            // Edit the age restrictions
            test.Portal.Ministry_Activities_AddEditSettings_EnableAgeRestrictions(true, "1", "99");

            // Edit enforce assignment creation rules
            test.Portal.Ministry_Activities_AddEditSettings_EnforceAssignmentCreationRules(true, true, GeneralEnumerations.ActivityEnforceAssignmentCreationType.Schedule, true, GeneralEnumerations.ActivityEnforceAssignmentCreationType.Schedule, true, GeneralEnumerations.ActivityEnforceAssignmentAutoCreate.FirstAttendance);

            // Edit use as weblink groups
            test.Portal.Ministry_Activities_AddEditSettings_UseAsWebLinkGroups(true, "18-24");

            // Save Changes
            test.Driver.FindElementById(GeneralMinistry.Activities.Im_Done).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(GeneralMinistry.Activities.Manage_Staffing_Needs));
            //test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Delete this activity"));

            // Verify updated description
            test.GeneralMethods.VerifyTextPresentWebDriver(description);

            // Verify Properties Sidebar            
            Dictionary<string, string> properties = new Dictionary<string, string> { 
              {GeneralMinistry.Activity.Property.Ministry, ministryName},
              {GeneralMinistry.Activity.Property.Activity_Type, activityType},
              {GeneralMinistry.Activity.Property.Age, "1 to 99"},
              {GeneralMinistry.Activity.Property.Confidential, GeneralMinistry.Activity.Property.Yes},
              {GeneralMinistry.Activity.Property.Volunteers_Staff, "Activity Schedule"},
              {GeneralMinistry.Activity.Property.Participants, "Activity Schedule"},
              {GeneralMinistry.Activity.Property.Auto_Assignment, "First Attendance"},
              {GeneralMinistry.Activity.Property.Weblink_Groups, "18-24"},
              {GeneralMinistry.Activity.Property.Active, GeneralMinistry.Activity.Property.Yes} 
            };

            test.Portal.Ministry_Activities_Activity_Property_Verify(properties);
            
            // Verify Check-in Sidebar
            IWebElement checkinTable = test.Driver.FindElementByXPath(TableIds.Ministry_Activity_CheckInTable);
            Assert.AreEqual("Code", checkinTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[0].Text);
            Assert.AreEqual(checkinCode, Convert.ToInt32(checkinTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("td"))[0].FindElements(By.TagName("span"))[0].Text));
            Assert.AreEqual("Name Tag", checkinTable.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Parent Receipt", checkinTable.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Closed Room Override", checkinTable.FindElements(By.TagName("tr"))[3].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[3].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Assignment Required", checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Alert", checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("td"))[0].FindElements(By.TagName("span"))[0].Text);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [DependsOn("Ministry_Activities_ViewAll_Activity_Properties_Edit_Description")]
        [Author("David Martin")]
        [Description("F1-3882: Verifies you can edit an activity's confidentiality")]
        public void Ministry_Activities_ViewAll_Activity_Properties_Edit_Confidential()
        {
            // Data
            int churchId = 15;
            //string ministryName = "A Test Ministry";
            string activityName = "*Properties Edit Test";
            string description = "This is an activity description";
            string activityType = "Automation Activity";
            int checkinCode = base.SQL.Ministry_CheckIn_FetchCheckInCode(15, activityName);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Properties Edit page
            test.Portal.Ministry_Activities_Properties_EditPage(churchId, ministryName, activityName);

            // Edit the basic information
            test.Portal.Ministry_Activities_AddEditSettings_BasicInformation(activityName, null, true, description, false, activityType);

            // Edit the Check-in settings
            test.Portal.Ministry_Activities_AddEditSettings_EnableCheckin(true, true, true, true, true, false);

            // Edit the age restrictions
            test.Portal.Ministry_Activities_AddEditSettings_EnableAgeRestrictions(true, "1", "99");

            // Edit enforce assignment creation rules
            test.Portal.Ministry_Activities_AddEditSettings_EnforceAssignmentCreationRules(true, true, GeneralEnumerations.ActivityEnforceAssignmentCreationType.Schedule, true, GeneralEnumerations.ActivityEnforceAssignmentCreationType.Schedule, true, GeneralEnumerations.ActivityEnforceAssignmentAutoCreate.FirstAttendance);

            // Edit use as weblink groups
            test.Portal.Ministry_Activities_AddEditSettings_UseAsWebLinkGroups(true, "18-24");

            // Save Changes
            test.Driver.FindElementById(GeneralMinistry.Activities.Im_Done).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(GeneralMinistry.Activities.Manage_Staffing_Needs));
            //test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Delete this activity"));

            // Verify updated description
            test.GeneralMethods.VerifyTextPresentWebDriver(description);

            // Verify Properties Sidebar            
            Dictionary<string, string> properties = new Dictionary<string, string> { 
              {GeneralMinistry.Activity.Property.Ministry, ministryName},
              {GeneralMinistry.Activity.Property.Activity_Type, activityType},
              {GeneralMinistry.Activity.Property.Age, "1 to 99"},
              {GeneralMinistry.Activity.Property.Volunteers_Staff, "Activity Schedule"},
              {GeneralMinistry.Activity.Property.Participants, "Activity Schedule"},
              {GeneralMinistry.Activity.Property.Auto_Assignment, "First Attendance"},
              {GeneralMinistry.Activity.Property.Weblink_Groups, "18-24"},
              {GeneralMinistry.Activity.Property.Active, GeneralMinistry.Activity.Property.Yes} 
            };

            test.Portal.Ministry_Activities_Activity_Property_Verify(properties);

            // Verify Check-in Sidebar
            IWebElement checkinTable = test.Driver.FindElementByXPath(TableIds.Ministry_Activity_CheckInTable);
            Assert.AreEqual("Code", checkinTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[0].Text);
            Assert.AreEqual(checkinCode, Convert.ToInt32(checkinTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("td"))[0].FindElements(By.TagName("span"))[0].Text));
            Assert.AreEqual("Name Tag", checkinTable.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Parent Receipt", checkinTable.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Closed Room Override", checkinTable.FindElements(By.TagName("tr"))[3].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[3].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Assignment Required", checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Alert", checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("td"))[0].FindElements(By.TagName("span"))[0].Text);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [DependsOn("Ministry_Activities_ViewAll_Activity_Properties_Edit_Confidential")]
        [Author("David Martin")]
        [Description("F1-3882: Verifies you can edit the Activity Type for an activity")]
        public void Ministry_Activities_ViewAll_Activity_Properties_Edit_ActivityType()
        {
            // Data
            int churchId = 15;
            //string ministryName = "A Test Ministry";
            string activityName = "*Properties Edit Test";
            string description = "This is an activity description";
            int checkinCode = base.SQL.Ministry_CheckIn_FetchCheckInCode(15, activityName);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Properties Edit page
            test.Portal.Ministry_Activities_Properties_EditPage(churchId, ministryName, activityName);

            // Edit the basic information
            test.Portal.Ministry_Activities_AddEditSettings_BasicInformation(activityName, null, true, description, false, "---");

            // Edit the Check-in settings
            test.Portal.Ministry_Activities_AddEditSettings_EnableCheckin(true, true, true, true, true, false);

            // Edit the age restrictions
            test.Portal.Ministry_Activities_AddEditSettings_EnableAgeRestrictions(true, "1", "99");

            // Edit enforce assignment creation rules
            test.Portal.Ministry_Activities_AddEditSettings_EnforceAssignmentCreationRules(true, true, GeneralEnumerations.ActivityEnforceAssignmentCreationType.Schedule, true, GeneralEnumerations.ActivityEnforceAssignmentCreationType.Schedule, true, GeneralEnumerations.ActivityEnforceAssignmentAutoCreate.FirstAttendance);

            // Edit use as weblink groups
            test.Portal.Ministry_Activities_AddEditSettings_UseAsWebLinkGroups(true, "18-24");

            // Save Changes
            test.Driver.FindElementById(GeneralMinistry.Activities.Im_Done).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(GeneralMinistry.Activities.Manage_Staffing_Needs));
            //test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Delete this activity"));

            // Verify updated description
            test.GeneralMethods.VerifyTextPresentWebDriver(description);

            // Verify Properties Sidebar            
            Dictionary<string, string> properties = new Dictionary<string, string> { 
              {GeneralMinistry.Activity.Property.Ministry, ministryName},
              {GeneralMinistry.Activity.Property.Age, "1 to 99"},
              {GeneralMinistry.Activity.Property.Volunteers_Staff, "Activity Schedule"},
              {GeneralMinistry.Activity.Property.Participants, "Activity Schedule"},
              {GeneralMinistry.Activity.Property.Auto_Assignment, "First Attendance"},
              {GeneralMinistry.Activity.Property.Weblink_Groups, "18-24"},
              {GeneralMinistry.Activity.Property.Active, GeneralMinistry.Activity.Property.Yes} 
            };

            test.Portal.Ministry_Activities_Activity_Property_Verify(properties);

            // Verify Check-in Sidebar
            IWebElement checkinTable = test.Driver.FindElementByXPath(TableIds.Ministry_Activity_CheckInTable);
            Assert.AreEqual("Code", checkinTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[0].Text, "Title: Code was not found");
            Assert.AreEqual(checkinCode, Convert.ToInt32(checkinTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("td"))[0].FindElements(By.TagName("span"))[0].Text));
            Assert.AreEqual("Name Tag", checkinTable.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Parent Receipt", checkinTable.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Closed Room Override", checkinTable.FindElements(By.TagName("tr"))[3].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[3].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Assignment Required", checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Alert", checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("td"))[0].FindElements(By.TagName("span"))[0].Text);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [DependsOn("Ministry_Activities_ViewAll_Activity_Properties_Edit_ActivityType")]
        [Author("David Martin")]
        [Description("F1-3882: Verifies you can edit the Activity Name for an activity")]
        public void Ministry_Activities_ViewAll_Activity_Properties_Edit_ActivityName()
        {
            // Data
            int churchId = 15;
            //string ministryName = "A Test Ministry";
            string activityName = "*Properties Edit Test";
            string updatedActivityName = "*Properties: Your Mom";
            string activityType = "---";
            int checkinCode = base.SQL.Ministry_CheckIn_FetchCheckInCode(15, activityName);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Properties Edit page
            test.Portal.Ministry_Activities_Properties_EditPage(churchId, ministryName, activityName);

            // Edit the basic information
            test.Portal.Ministry_Activities_AddEditSettings_BasicInformation(activityName, updatedActivityName, true, null, false, activityType);

            // Edit the Check-in settings
            test.Portal.Ministry_Activities_AddEditSettings_EnableCheckin(true, true, true, true, true, false);

            // Edit the age restrictions
            test.Portal.Ministry_Activities_AddEditSettings_EnableAgeRestrictions(true, "1", "99");

            // Edit enforce assignment creation rules
            test.Portal.Ministry_Activities_AddEditSettings_EnforceAssignmentCreationRules(true, true, GeneralEnumerations.ActivityEnforceAssignmentCreationType.Schedule, true, GeneralEnumerations.ActivityEnforceAssignmentCreationType.Schedule, true, GeneralEnumerations.ActivityEnforceAssignmentAutoCreate.FirstAttendance);

            // Edit use as weblink groups
            test.Portal.Ministry_Activities_AddEditSettings_UseAsWebLinkGroups(true, "18-24");

            // Save Changes
            test.Driver.FindElementById(GeneralMinistry.Activities.Im_Done).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(GeneralMinistry.Activities.Manage_Staffing_Needs));
            //test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Delete this activity"));

            // Verify updated activity name
            test.GeneralMethods.VerifyTextPresentWebDriver(updatedActivityName);

            // Verify Properties Sidebar            
            Dictionary<string, string> properties = new Dictionary<string, string> { 
              {GeneralMinistry.Activity.Property.Ministry, ministryName},
              {GeneralMinistry.Activity.Property.Age, "1 to 99"},
              {GeneralMinistry.Activity.Property.Volunteers_Staff, "Activity Schedule"},
              {GeneralMinistry.Activity.Property.Participants, "Activity Schedule"},
              {GeneralMinistry.Activity.Property.Auto_Assignment, "First Attendance"},
              {GeneralMinistry.Activity.Property.Weblink_Groups, "18-24"},
              {GeneralMinistry.Activity.Property.Active, GeneralMinistry.Activity.Property.Yes} 
            };

            test.Portal.Ministry_Activities_Activity_Property_Verify(properties);

            // Verify Check-in Sidebar
            IWebElement checkinTable = test.Driver.FindElementByXPath(TableIds.Ministry_Activity_CheckInTable);
            Assert.AreEqual("Code", checkinTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[0].Text);
            Assert.AreEqual(checkinCode, Convert.ToInt32(checkinTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("td"))[0].FindElements(By.TagName("span"))[0].Text));
            Assert.AreEqual("Name Tag", checkinTable.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Parent Receipt", checkinTable.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Closed Room Override", checkinTable.FindElements(By.TagName("tr"))[3].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[3].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Assignment Required", checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Alert", checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("td"))[0].FindElements(By.TagName("span"))[0].Text);

            // Change activity name back
            test.Portal.Ministry_Activities_Properties_EditPage(churchId, ministryName, updatedActivityName);

            // Edit the basic information
            test.Portal.Ministry_Activities_AddEditSettings_BasicInformation(updatedActivityName, activityName, true, null, false, activityType);

            // Save Changes
            test.Driver.FindElementById(GeneralMinistry.Activities.Im_Done).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(GeneralMinistry.Activities.Manage_Staffing_Needs));
            //test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Delete this activity"));

            // Verify updated activity name
            test.GeneralMethods.VerifyTextPresentWebDriver(activityName);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [DependsOn("Ministry_Activities_ViewAll_Activity_Properties_Edit_ActivityName")]
        [Author("David Martin")]
        [Description("F1-3882: Verifies you can make an activity inactive")]
        public void Ministry_Activities_ViewAll_Activity_Properties_Edit_InactiveActivity()
        {
            // Data
            int churchId = 15;
            //string ministryName = "A Test Ministry";
            string activityName = "*Properties Edit Test";
            int checkinCode = base.SQL.Ministry_CheckIn_FetchCheckInCode(15, activityName);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Properties Edit page
            test.Portal.Ministry_Activities_Properties_EditPage(churchId, ministryName, activityName);

            // Edit the basic information
            test.Portal.Ministry_Activities_AddEditSettings_BasicInformation(activityName, null, false, null, false, "---");

            // Edit the Check-in settings
            test.Portal.Ministry_Activities_AddEditSettings_EnableCheckin(true, true, true, true, true, false);

            // Edit the age restrictions
            test.Portal.Ministry_Activities_AddEditSettings_EnableAgeRestrictions(true, "1", "99");

            // Edit enforce assignment creation rules
            test.Portal.Ministry_Activities_AddEditSettings_EnforceAssignmentCreationRules(true, true, GeneralEnumerations.ActivityEnforceAssignmentCreationType.Schedule, true, GeneralEnumerations.ActivityEnforceAssignmentCreationType.Schedule, true, GeneralEnumerations.ActivityEnforceAssignmentAutoCreate.FirstAttendance);

            // Edit use as weblink groups
            test.Portal.Ministry_Activities_AddEditSettings_UseAsWebLinkGroups(true, "18-24");

            // Save Changes
            test.Driver.FindElementById(GeneralMinistry.Activities.Im_Done).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(GeneralMinistry.Activities.Manage_Staffing_Needs));
            //test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Delete this activity"));

            // Verify Properties Sidebar            
            Dictionary<string, string> properties = new Dictionary<string, string> { 
              {GeneralMinistry.Activity.Property.Ministry, ministryName},
              {GeneralMinistry.Activity.Property.Age, "1 to 99"},
              {GeneralMinistry.Activity.Property.Volunteers_Staff, "Activity Schedule"},
              {GeneralMinistry.Activity.Property.Participants, "Activity Schedule"},
              {GeneralMinistry.Activity.Property.Auto_Assignment, "First Attendance"},
              {GeneralMinistry.Activity.Property.Weblink_Groups, "18-24"},
              {GeneralMinistry.Activity.Property.Active, GeneralMinistry.Activity.Property.No} 
            };

            test.Portal.Ministry_Activities_Activity_Property_Verify(properties);

            // Verify Check-in Sidebar
            IWebElement checkinTable = test.Driver.FindElementByXPath(TableIds.Ministry_Activity_CheckInTable);
            Assert.AreEqual("Code", checkinTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[0].Text);
            Assert.AreEqual(checkinCode, Convert.ToInt32(checkinTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("td"))[0].FindElements(By.TagName("span"))[0].Text));
            Assert.AreEqual("Name Tag", checkinTable.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Parent Receipt", checkinTable.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Closed Room Override", checkinTable.FindElements(By.TagName("tr"))[3].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[3].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Assignment Required", checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Alert", checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("td"))[0].FindElements(By.TagName("span"))[0].Text);

            // Make Activity active again
            test.Driver.FindElementByXPath(GeneralMinistry.Activities.Properties_Edit).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Cancel"));
            test.Driver.FindElementById(GeneralMinistry.Activities.Activity_Active).Click();
            test.Driver.FindElementById(GeneralMinistry.Activities.Im_Done).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(GeneralMinistry.Activities.Properties_Edit));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [DependsOn("Ministry_Activities_ViewAll_Activity_Properties_Edit_InactiveActivity")]
        [Author("David Martin")]
        [Description("F1-3882: Verifies you can edit the checkin settings to not print name tags")]
        public void Ministry_Activities_ViewAll_Activity_Properties_Edit_CheckinNoNameTag()
        {
            // Data
            int churchId = 15;
            //string ministryName = "A Test Ministry";
            string activityName = "*Properties Edit Test";
            int checkinCode = base.SQL.Ministry_CheckIn_FetchCheckInCode(15, activityName);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Properties Edit page
            test.Portal.Ministry_Activities_Properties_EditPage(churchId, ministryName, activityName);

            // Edit the basic information
            test.Portal.Ministry_Activities_AddEditSettings_BasicInformation(activityName, null, true, null, false, "---");

            // Edit the Check-in settings
            test.Portal.Ministry_Activities_AddEditSettings_EnableCheckin(true, false, true, true, true, false);

            // Edit the age restrictions
            test.Portal.Ministry_Activities_AddEditSettings_EnableAgeRestrictions(true, "1", "99");

            // Edit enforce assignment creation rules
            test.Portal.Ministry_Activities_AddEditSettings_EnforceAssignmentCreationRules(true, true, GeneralEnumerations.ActivityEnforceAssignmentCreationType.Schedule, true, GeneralEnumerations.ActivityEnforceAssignmentCreationType.Schedule, true, GeneralEnumerations.ActivityEnforceAssignmentAutoCreate.FirstAttendance);

            // Edit use as weblink groups
            test.Portal.Ministry_Activities_AddEditSettings_UseAsWebLinkGroups(true, "18-24");

            // Save Changes
            test.Driver.FindElementById(GeneralMinistry.Activities.Im_Done).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(GeneralMinistry.Activities.Manage_Staffing_Needs));
            //test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Delete this activity"));

            // Verify Properties Sidebar            
            Dictionary<string, string> properties = new Dictionary<string, string> { 
              {GeneralMinistry.Activity.Property.Ministry, ministryName},
              {GeneralMinistry.Activity.Property.Age, "1 to 99"},
              {GeneralMinistry.Activity.Property.Volunteers_Staff, "Activity Schedule"},
              {GeneralMinistry.Activity.Property.Participants, "Activity Schedule"},
              {GeneralMinistry.Activity.Property.Auto_Assignment, "First Attendance"},
              {GeneralMinistry.Activity.Property.Weblink_Groups, "18-24"},
              {GeneralMinistry.Activity.Property.Active, GeneralMinistry.Activity.Property.Yes} 
            };

            test.Portal.Ministry_Activities_Activity_Property_Verify(properties);

            // Verify Check-in Sidebar
            IWebElement checkinTable = test.Driver.FindElementByXPath(TableIds.Ministry_Activity_CheckInTable);
            Assert.AreEqual("Code", checkinTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[0].Text);
            Assert.AreEqual(checkinCode, Convert.ToInt32(checkinTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("td"))[0].FindElements(By.TagName("span"))[0].Text));
            Assert.AreEqual("Name Tag", checkinTable.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/close.gif");
            Assert.AreEqual("Parent Receipt", checkinTable.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Closed Room Override", checkinTable.FindElements(By.TagName("tr"))[3].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[3].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Assignment Required", checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Alert", checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("td"))[0].FindElements(By.TagName("span"))[0].Text);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [DependsOn("Ministry_Activities_ViewAll_Activity_Properties_Edit_CheckinNoNameTag")]
        [Author("David Martin")]
        [Description("F1-3882: Verifies you can edit the checkin settings to not print parent receipts")]
        public void Ministry_Activities_ViewAll_Activity_Properties_Edit_CheckinNoParentReceipt()
        {
            // Data
            int churchId = 15;
            //string ministryName = "A Test Ministry";
            string activityName = "*Properties Edit Test";
            int checkinCode = base.SQL.Ministry_CheckIn_FetchCheckInCode(15, activityName);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Properties Edit page
            test.Portal.Ministry_Activities_Properties_EditPage(churchId, ministryName, activityName);

            // Edit the basic information
            test.Portal.Ministry_Activities_AddEditSettings_BasicInformation(activityName, null, true, null, false, "---");

            // Edit the Check-in settings
            test.Portal.Ministry_Activities_AddEditSettings_EnableCheckin(true, false, false, true, true, false);

            // Edit the age restrictions
            test.Portal.Ministry_Activities_AddEditSettings_EnableAgeRestrictions(true, "1", "99");

            // Edit enforce assignment creation rules
            test.Portal.Ministry_Activities_AddEditSettings_EnforceAssignmentCreationRules(true, true, GeneralEnumerations.ActivityEnforceAssignmentCreationType.Schedule, true, GeneralEnumerations.ActivityEnforceAssignmentCreationType.Schedule, true, GeneralEnumerations.ActivityEnforceAssignmentAutoCreate.FirstAttendance);

            // Edit use as weblink groups
            test.Portal.Ministry_Activities_AddEditSettings_UseAsWebLinkGroups(true, "18-24");

            // Save Changes
            test.Driver.FindElementById(GeneralMinistry.Activities.Im_Done).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(GeneralMinistry.Activities.Manage_Staffing_Needs));
            //test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Delete this activity"));

            // Verify Properties Sidebar            
            Dictionary<string, string> properties = new Dictionary<string, string> { 
              {GeneralMinistry.Activity.Property.Ministry, ministryName},
              {GeneralMinistry.Activity.Property.Age, "1 to 99"},
              {GeneralMinistry.Activity.Property.Volunteers_Staff, "Activity Schedule"},
              {GeneralMinistry.Activity.Property.Participants, "Activity Schedule"},
              {GeneralMinistry.Activity.Property.Auto_Assignment, "First Attendance"},
              {GeneralMinistry.Activity.Property.Weblink_Groups, "18-24"},
              {GeneralMinistry.Activity.Property.Active, GeneralMinistry.Activity.Property.Yes} 
            };

            test.Portal.Ministry_Activities_Activity_Property_Verify(properties);

            // Verify Check-in Sidebar
            IWebElement checkinTable = test.Driver.FindElementByXPath(TableIds.Ministry_Activity_CheckInTable);
            Assert.AreEqual("Code", checkinTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[0].Text);
            Assert.AreEqual(checkinCode, Convert.ToInt32(checkinTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("td"))[0].FindElements(By.TagName("span"))[0].Text));
            Assert.AreEqual("Name Tag", checkinTable.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/close.gif");
            Assert.AreEqual("Parent Receipt", checkinTable.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/close.gif");
            Assert.AreEqual("Closed Room Override", checkinTable.FindElements(By.TagName("tr"))[3].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[3].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Assignment Required", checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Alert", checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("td"))[0].FindElements(By.TagName("span"))[0].Text);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [DependsOn("Ministry_Activities_ViewAll_Activity_Properties_Edit_CheckinNoParentReceipt")]
        [Author("David Martin")]
        [Description("F1-3882: Verifies you can edit the checkin settings to not allow assignments to override closed rooms")]
        public void Ministry_Activities_ViewAll_Activity_Properties_Edit_CheckinNoOverrideClosedRooms()
        {
            // Data
            int churchId = 15;
            //string ministryName = "A Test Ministry";
            string activityName = "*Properties Edit Test";
            int checkinCode = base.SQL.Ministry_CheckIn_FetchCheckInCode(15, activityName);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Properties Edit page
            test.Portal.Ministry_Activities_Properties_EditPage(churchId, ministryName, activityName);

            // Edit the basic information
            test.Portal.Ministry_Activities_AddEditSettings_BasicInformation(activityName, null, true, null, false, "---");

            // Edit the Check-in settings
            test.Portal.Ministry_Activities_AddEditSettings_EnableCheckin(true, false, false, false, true, false);

            // Edit the age restrictions
            test.Portal.Ministry_Activities_AddEditSettings_EnableAgeRestrictions(true, "1", "99");

            // Edit enforce assignment creation rules
            test.Portal.Ministry_Activities_AddEditSettings_EnforceAssignmentCreationRules(true, true, GeneralEnumerations.ActivityEnforceAssignmentCreationType.Schedule, true, GeneralEnumerations.ActivityEnforceAssignmentCreationType.Schedule, true, GeneralEnumerations.ActivityEnforceAssignmentAutoCreate.FirstAttendance);

            // Edit use as weblink groups
            test.Portal.Ministry_Activities_AddEditSettings_UseAsWebLinkGroups(true, "18-24");

            // Save Changes
            test.Driver.FindElementById(GeneralMinistry.Activities.Im_Done).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(GeneralMinistry.Activities.Manage_Staffing_Needs));
            //test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Delete this activity"));

            // Verify Properties Sidebar            
            Dictionary<string, string> properties = new Dictionary<string, string> { 
              {GeneralMinistry.Activity.Property.Ministry, ministryName},
              {GeneralMinistry.Activity.Property.Age, "1 to 99"},
              {GeneralMinistry.Activity.Property.Volunteers_Staff, "Activity Schedule"},
              {GeneralMinistry.Activity.Property.Participants, "Activity Schedule"},
              {GeneralMinistry.Activity.Property.Auto_Assignment, "First Attendance"},
              {GeneralMinistry.Activity.Property.Weblink_Groups, "18-24"},
              {GeneralMinistry.Activity.Property.Active, GeneralMinistry.Activity.Property.Yes} 
            };

            test.Portal.Ministry_Activities_Activity_Property_Verify(properties);

            // Verify Check-in Sidebar
            IWebElement checkinTable = test.Driver.FindElementByXPath(TableIds.Ministry_Activity_CheckInTable);
            Assert.AreEqual("Code", checkinTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[0].Text);
            Assert.AreEqual(checkinCode, Convert.ToInt32(checkinTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("td"))[0].FindElements(By.TagName("span"))[0].Text));
            Assert.AreEqual("Name Tag", checkinTable.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/close.gif");
            Assert.AreEqual("Parent Receipt", checkinTable.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/close.gif");
            Assert.AreEqual("Closed Room Override", checkinTable.FindElements(By.TagName("tr"))[3].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[3].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/close.gif");
            Assert.AreEqual("Assignment Required", checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/check.gif");
            Assert.AreEqual("Alert", checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("td"))[0].FindElements(By.TagName("span"))[0].Text);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [DependsOn("Ministry_Activities_ViewAll_Activity_Properties_Edit_CheckinNoOverrideClosedRooms")]
        [Author("David Martin")]
        [Description("F1-3882: Verifies you can edit the checkin settings to not require assignments")]
        public void Ministry_Activities_ViewAll_Activity_Properties_Edit_CheckinNoAssignmentRequired()
        {
            // Data
            int churchId = 15;
            //string ministryName = "A Test Ministry";
            string activityName = "*Properties Edit Test";
            int checkinCode = base.SQL.Ministry_CheckIn_FetchCheckInCode(15, activityName);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Properties Edit page
            test.Portal.Ministry_Activities_Properties_EditPage(churchId, ministryName, activityName);

            // Edit the basic information
            test.Portal.Ministry_Activities_AddEditSettings_BasicInformation(activityName, null, true, null, false, "---");

            // Edit the Check-in settings
            test.Portal.Ministry_Activities_AddEditSettings_EnableCheckin(true, false, false, false, false, false);

            // Edit the age restrictions
            test.Portal.Ministry_Activities_AddEditSettings_EnableAgeRestrictions(true, "1", "99");

            // Edit enforce assignment creation rules
            test.Portal.Ministry_Activities_AddEditSettings_EnforceAssignmentCreationRules(true, true, GeneralEnumerations.ActivityEnforceAssignmentCreationType.Schedule, true, GeneralEnumerations.ActivityEnforceAssignmentCreationType.Schedule, true, GeneralEnumerations.ActivityEnforceAssignmentAutoCreate.FirstAttendance);

            // Edit use as weblink groups
            test.Portal.Ministry_Activities_AddEditSettings_UseAsWebLinkGroups(true, "18-24");

            // Save Changes
            test.Driver.FindElementById(GeneralMinistry.Activities.Im_Done).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(GeneralMinistry.Activities.Manage_Staffing_Needs));
            //test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Delete this activity"));

            // Verify Properties Sidebar            
            Dictionary<string, string> properties = new Dictionary<string, string> { 
              {GeneralMinistry.Activity.Property.Ministry, ministryName},
              {GeneralMinistry.Activity.Property.Age, "1 to 99"},
              {GeneralMinistry.Activity.Property.Volunteers_Staff, "Activity Schedule"},
              {GeneralMinistry.Activity.Property.Participants, "Activity Schedule"},
              {GeneralMinistry.Activity.Property.Auto_Assignment, "First Attendance"},
              {GeneralMinistry.Activity.Property.Weblink_Groups, "18-24"},
              {GeneralMinistry.Activity.Property.Active, GeneralMinistry.Activity.Property.Yes} 
            };

            test.Portal.Ministry_Activities_Activity_Property_Verify(properties);

            // Verify Check-in Sidebar
            IWebElement checkinTable = test.Driver.FindElementByXPath(TableIds.Ministry_Activity_CheckInTable);
            Assert.AreEqual("Code", checkinTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[0].Text);
            Assert.AreEqual(checkinCode, Convert.ToInt32(checkinTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("td"))[0].FindElements(By.TagName("span"))[0].Text));
            Assert.AreEqual("Name Tag", checkinTable.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/close.gif");
            Assert.AreEqual("Parent Receipt", checkinTable.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/close.gif");
            Assert.AreEqual("Closed Room Override", checkinTable.FindElements(By.TagName("tr"))[3].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[3].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/close.gif");
            Assert.AreEqual("Assignment Required", checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("th"))[0].Text);
            Assert.Contains(checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("td"))[0].FindElements(By.TagName("img"))[0].GetAttribute("src"), "/portal/images/close.gif");
            //Assert.AreEqual("Alert", checkinTable.FindElements(By.TagName("tr"))[4].FindElements(By.TagName("td"))[0].FindElements(By.TagName("span"))[0].Text);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]      
        [Author("Winnie Wang")]
        [Description("FO-1573: New Activity - Require an Assignment Option Does not Stick")]
        public void Ministry_Activities_ViewAll_Activity_Properties_Edit_CheckinWithAssignment()        
        {
            // Data           
            string ministryName = "Auto Test Ministry";
            string activityName = "*Daily Test Activity";
            string scheduleName = "Daily Schedule";
            string rosterName = "Daily Roster";

            try
            {
                // Login to portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

                // Setup roster names
                System.Collections.Generic.List<string> roster = new System.Collections.Generic.List<string>();
                roster.Add(rosterName);

                // Create a daily activity with checkin and Require an Assignment_Alert
                test.Portal.Ministry_Activities_Create_Step1_WebDriver(ministryName, activityName, string.Empty, false, "---", true, false, false, false, true, GeneralEnumerations.ActivitiesRequireAssignment.Alert);
                DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
                test.Portal.Ministry_Activities_Create_Step2_Daily_WebDriver(15, scheduleName, "6:00 PM", "8:00 PM", "1", today.AddDays(1).ToShortDateString());
                test.Portal.Ministry_Activities_Create_Step3_WebDriver(false, roster, string.Empty, string.Empty);

                Assert.IsTrue(test.Driver.FindElementByXPath(".//*[@id='sidebar']/div/table[2]/tbody/tr[5]/td/span").Text.Equals("Alert"), "Checkin Alert isn't saved!");

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            
            finally
            {
                base.SQL.Ministry_Activities_Delete_Activity(15, activityName);
            }
        }

        [Test, RepeatOnFailure]
        [DependsOn("Ministry_Activities_ViewAll_Activity_Properties_Edit_CheckinNoAssignmentRequired")]
        [Author("David Martin")]
        [Description("F1-3882: Verifies you can edit an activity and save with checkin settings disabled")]
        public void Ministry_Activities_ViewAll_Activity_Properties_Edit_DisableCheckin()
        {
            // Data
            int churchId = 15;
            //string ministryName = "A Test Ministry";
            string activityName = "*Properties Edit Test";
            int checkinCode = base.SQL.Ministry_CheckIn_FetchCheckInCode(15, activityName);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Navigate to the Properties Edit page
            test.Portal.Ministry_Activities_Properties_EditPage(churchId, ministryName, activityName);

            // Edit the basic information
            test.Portal.Ministry_Activities_AddEditSettings_BasicInformation(activityName, null, true, null, false, "---");

            // Edit the Check-in settings
            test.Portal.Ministry_Activities_AddEditSettings_EnableCheckin(false, false, false, false, false, false);

            // Edit the age restrictions
            test.Portal.Ministry_Activities_AddEditSettings_EnableAgeRestrictions(true, "1", "99");

            // Edit enforce assignment creation rules
            test.Portal.Ministry_Activities_AddEditSettings_EnforceAssignmentCreationRules(true, true, GeneralEnumerations.ActivityEnforceAssignmentCreationType.Schedule, true, GeneralEnumerations.ActivityEnforceAssignmentCreationType.Schedule, true, GeneralEnumerations.ActivityEnforceAssignmentAutoCreate.FirstAttendance);

            // Edit use as weblink groups
            test.Portal.Ministry_Activities_AddEditSettings_UseAsWebLinkGroups(true, "18-24");

            // Save Changes
            test.Driver.FindElementById(GeneralMinistry.Activities.Im_Done).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(GeneralMinistry.Activities.Manage_Staffing_Needs));
            //test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Delete this activity"));

            // Verify Properties Sidebar            
            Dictionary<string, string> properties = new Dictionary<string, string> { 
              {GeneralMinistry.Activity.Property.Ministry, ministryName},
              {GeneralMinistry.Activity.Property.Age, "1 to 99"},
              {GeneralMinistry.Activity.Property.Volunteers_Staff, "Activity Schedule"},
              {GeneralMinistry.Activity.Property.Participants, "Activity Schedule"},
              {GeneralMinistry.Activity.Property.Auto_Assignment, "First Attendance"},
              {GeneralMinistry.Activity.Property.Weblink_Groups, "18-24"},
              {GeneralMinistry.Activity.Property.Active, GeneralMinistry.Activity.Property.Yes} 
            };

            test.Portal.Ministry_Activities_Activity_Property_Verify(properties);

            // Verify Check-in Sidebar is not present
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.XPath(TableIds.Ministry_Activity_CheckInTable));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }
        #endregion Properties

        #region Manage Staffing Needs
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("F1-3836: Verifies you add staffing needs to a job for an activity schedule")]
        public void Ministry_Activities_ViewAll_Activity_ManageStaffingNeeds_AddRemove()
        {
            // Setup
            string activityName = "Assign Activity Level";
            string activitySchedule = "Assign Activity Level";
            string jobTitle = "Dryer";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add staffing needs to an activity schedule
            test.Portal.Ministry_Activities_ManageStaffingNeeds(15, activityName, activitySchedule, jobTitle, "37", false, false);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3836: Verifies you can cancel out of the staffing needs page without changes being saved")]
        public void Ministry_Activities_ViewAll_Activity_ManageStaffingNeeds_Cancel()
        {
            // Setup
            string activityName = "Assign Activity Level";
            string activitySchedule = "Assign Activity Level";
            string jobTitle = "Dryer";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add staffing needs to an activity schedule
            test.Portal.Ministry_Activities_ManageStaffingNeeds(15, activityName, activitySchedule, jobTitle, "3737", true, false);

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3836: Verifies only digits will be saved for the staffing needs amount")]
        public void Ministry_Activities_ViewAll_Activity_ManageStaffingNeeds_NonNumeric()
        {
            // Setup
            string activityName = "Assign Activity Level";
            string activitySchedule = "Assign Activity Level";
            string jobTitle = "Dryer";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.ministrytest", "FT4life!", "dc");

            // Add staffing needs to an activity schedule
            test.Portal.Ministry_Activities_ManageStaffingNeeds(15, activityName, activitySchedule, jobTitle, "49ers", false, true);

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Manage Staffing Needs

        #endregion ActivitiesNew

    }
}
