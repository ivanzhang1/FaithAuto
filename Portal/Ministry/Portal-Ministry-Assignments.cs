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
    public class Portal_Ministry_Assignments : FixtureBaseWebDriver
    {
        #region Assignments

        #region View All
        [Test, RepeatOnFailure, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Stuart Platt")]
        [Description("F1-3759: Verify Page loads")]
        public void Portal_Assignments_ViewAll()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            //Navigate to Assigments View all page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

            // Verify popover message appears
            test.GeneralMethods.VerifyElementPresentWebDriver(By.ClassName("popover-inner"));

            //Verify show more Toggle is functioning properly
            Assert.AreEqual("HIDE SIDEBAR", test.Driver.FindElementByCssSelector("[style=''][data-hide-on-expand='yes']").Text, "Hide Sidebar not displayed");

            //Verify that Popoever is present
            //<div class="popover fade left in" style="top: 214.5px; left: 845.5px; display: block;">
            //<div class="arrow"></div>
            //<div class="popover-inner"><h3 class="popover-title">Select a Ministry &amp; Activity</h3>
            //<div class="popover-content"><p>A Ministry filter and an Activity filter are required to view a list of Assignments. All other filters are optional.</p></div></div></div>


            //Verfiy Action items 
            //Disabled for now (Export, View Parent / Children, Send Email, Mass Action, Add to Group and More actions)
            //Assert.IsTrue(test.Driver.FindElementByLinkText("Export").Enabled, "Export is not disabled");
            //Assert.IsTrue(test.Driver.FindElementByLinkText("View Parents/Children").Enabled, "View Parents / Children is not disabled");
            //Assert.IsTrue(test.Driver.FindElementByLinkText("Send Email").Enabled, "Send Email is not disabled");
            //Assert.IsTrue(test.Driver.FindElementByLinkText("Mass Action").Enabled, "Mass Action is not disabled");
            Assert.IsTrue(test.Driver.FindElementById("addToGroupDropdown").Enabled, "Add to Group is not disabled");
            //Assert.IsTrue(test.Driver.FindElementByLinkText("More Actions").Enabled, "More Actions is not disabled");

            // Verify Grid Headers
            IWebElement assignmentsTable = test.Driver.FindElementByXPath(TableIds.Ministry_Assignments_ViewAll);
            Assert.AreEqual("Name", assignmentsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[1].Text, "Assignments Header Mismatch");
            //Assert.AreEqual("Activity\r\nMinistry", assignmentsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[2].Text, "Assignments Header Mismatch");
            Assert.AreEqual("Roster (RLC) / Folder\r\nBreakout Group", assignmentsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[3].Text, "Assignments Header Mismatch");
            Assert.AreEqual("Schedule / Time\r\nVol / Staff Schedule", assignmentsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[4].Text, "Assignments Header Mismatch");
            //Assert.AreEqual("…", assignmentsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[7].FindElements(By.TagName("div"))[0].FindElements(By.TagName("a"))[0].Text, "Assignments Header Mismatch");
            
            //Verify Add Assignments Link
            //Button disabled for now. 
            test.GeneralMethods.VerifyTextPresentWebDriver("Actions");
            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath(GeneralMinistry.Assignments.Add_Assignment));
            
            //Verify Reset and Apply buttons
            test.GeneralMethods.VerifyTextPresentWebDriver("Filters");
            Assert.IsTrue(test.Driver.FindElementByLinkText(GeneralMinistry.Assignments.Reset_Link).Displayed, "Reset link is not displayed");
            Assert.IsTrue(test.Driver.FindElementByXPath(GeneralMinistry.Assignments.Apply_Button).Displayed, "General button is not displayed");

            //Verify Checkboxes
            Assert.IsTrue(test.Driver.FindElementById(GeneralMinistry.Assignments.Participant_Checkbox).Selected, "Participant Check Box should be selected by default");
            Assert.IsTrue(test.Driver.FindElementById(GeneralMinistry.Assignments.Volunteer_Checkbox).Selected, "Volunteer / Staff Check Box should be selected by default");
            Assert.IsFalse(test.Driver.FindElementById(GeneralMinistry.Assignments.Inactive_Staff_Checkbox).Selected, "Inactive Staff Check Box should not be selected by default");

            //Verify Ministry
            test.GeneralMethods.VerifyTextPresentWebDriver("Ministry");
            test.Driver.FindElementById(GeneralMinistry.Assignments.Ministry).Click();
            Assert.IsTrue(test.Driver.FindElementById(string.Format("{0}-1", GeneralMinistry.Assignments.Ministry_DropDown)).Displayed, "Ministry input is not displayed");
            Assert.AreEqual("---", new SelectElement(test.Driver.FindElementById(string.Format("{0}-1", GeneralMinistry.Assignments.Ministry_DropDown))).SelectedOption.Text.Trim(), "Default --- not selected option");

            //Verify Activity
            test.GeneralMethods.VerifyTextPresentWebDriver("Activity");
            test.Driver.FindElementById(GeneralMinistry.Assignments.Activity).Click();
            Assert.IsTrue(test.Driver.FindElementById(string.Format("{0}-1", GeneralMinistry.Assignments.Activity_Dropdown)).Displayed, "Activity input is not displayed");
            Assert.AreEqual("---", new SelectElement(test.Driver.FindElementById(string.Format("{0}-1", GeneralMinistry.Assignments.Activity_Dropdown))).SelectedOption.Text.Trim(), "Default --- not selected option");

            //Verify Rosters / Breakout Groups
            test.GeneralMethods.VerifyTextPresentWebDriver("Rosters / Breakout Groups");
            test.Driver.FindElementByXPath(GeneralMinistry.Assignments.Rosters).Click();
            test.GeneralMethods.WaitForElementDisplayed(By.Id(string.Format("{0}-1", GeneralMinistry.Assignments.Roster_Folders_Dropdown)), 30, "Roster Folder input is not displayed");
            //Assert.IsTrue(test.Driver.FindElementById(string.Format("{0}-1", GeneralMinistry.Assignments.Roster_Folders_Dropdown)).Displayed, "Roster Folder input is not displayed");
            Assert.AreEqual("---", new SelectElement(test.Driver.FindElementById(string.Format("{0}-1", GeneralMinistry.Assignments.Roster_Folders_Dropdown))).SelectedOption.Text.Trim(), "Default --- not selected option");
            Assert.IsTrue(test.Driver.FindElementById(string.Format("{0}-1", GeneralMinistry.Assignments.Roster_DropDown)).Displayed, "Roster input is not displayed");
            Assert.AreEqual("---", new SelectElement(test.Driver.FindElementById(string.Format("{0}-1", GeneralMinistry.Assignments.Roster_DropDown))).SelectedOption.Text.Trim(), "Default --- not selected option");
            Assert.IsTrue(test.Driver.FindElementById(string.Format("{0}-1", GeneralMinistry.Assignments.Breakout_Group_DropDown)).Displayed, "Breakout input is not displayed");
            Assert.AreEqual("---", new SelectElement(test.Driver.FindElementById(string.Format("{0}-1", GeneralMinistry.Assignments.Breakout_Group_DropDown))).SelectedOption.Text.Trim(), "Default --- not selected option");

            //Verify Activity Schedules
            test.GeneralMethods.VerifyTextPresentWebDriver("Activity Schedules");
            test.Driver.FindElementByXPath(GeneralMinistry.Assignments.Activity_Schedules).Click();
            Assert.IsTrue(test.Driver.FindElementById(string.Format("{0}-1", GeneralMinistry.Assignments.Activity_Schedules_DropDown)).Displayed, "Activity Schedule input is not displayed");
            Assert.AreEqual("---", new SelectElement(test.Driver.FindElementById(string.Format("{0}-1", GeneralMinistry.Assignments.Activity_Schedules_DropDown))).SelectedOption.Text.Trim(), "Default --- not selected option");

            //Verify Dates & Times
            //test.GeneralMethods.VerifyTextPresentWebDriver("Dates & Times");
            //test.Driver.FindElementByXPath(GeneralMinistry.Assignments.Date_Time).Click();
            ////Assert.IsTrue(test.Driver.FindElementById(GeneralMinistry.Assignments.Date_Range_DropDown).Displayed, "Date Range input is not displayed");
            ////Assert.AreEqual("Choose a date range...", new SelectElement(test.Driver.FindElementById(GeneralMinistry.Assignments.Date_Range_DropDown)).SelectedOption.Text.Trim(), "Default --- not selected option");
            //test.GeneralMethods.WaitForElementDisplayed(By.Id(GeneralMinistry.Assignments.Date_From), 30, "Date From input is not displayed");
            ////Assert.IsTrue(test.Driver.FindElementById(GeneralMinistry.Assignments.Date_From).Displayed, "Date From input is not displayed");
            //Assert.AreEqual("", test.Driver.FindElementById(GeneralMinistry.Assignments.Date_From).Text, "Default text is not set to empty");
            //Assert.IsTrue(test.Driver.FindElementById(GeneralMinistry.Assignments.Date_To).Displayed, "Date To input is not displayed");
            //Assert.AreEqual("", test.Driver.FindElementById(GeneralMinistry.Assignments.Date_To).Text, "Default text is not set to empty");
            //Assert.IsTrue(test.Driver.FindElementById(string.Format("{0}-1", GeneralMinistry.Assignments.DateTime_DropDown)).Displayed, "Occurrence input is not displayed");
            //Assert.AreEqual("---", new SelectElement(test.Driver.FindElementById(string.Format("{0}-1", GeneralMinistry.Assignments.DateTime_DropDown))).SelectedOption.Text.Trim(), "Default --- not selected option");
     
            //Verify more button
            Assert.IsTrue(test.Driver.FindElementById(GeneralMinistry.Assignments.More_Button).Displayed, "More button is not displayed");
            
            //Verify Footer
            Assert.AreEqual("Viewing 0 − 0 of 0 assignments", test.Driver.FindElementByXPath("//div[@class='grid_record_count float_right']").Text);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies you toggle the side bar on the new Assignments/View All page. ")]
        public void Portal_Assignments_ViewAll_SideBarCollapse()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to the Activities/View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

            //Verify show more Toggle is functioning properly
            Assert.AreEqual("HIDE SIDEBAR", test.Driver.FindElementByCssSelector("[style=''][data-hide-on-expand='yes']").Text, "Hide Sidebar not displayed");
            test.Driver.FindElementById("expand_collapse").Click();
            Assert.AreEqual("SHOW SIDEBAR", test.Driver.FindElementByCssSelector("[style='display: inline;'][data-show-on-expand='yes']").Text, "Show Sidebar not displayed");

            //Verify table is expanded
            IWebElement assignmentsTable = test.Driver.FindElementByXPath(TableIds.Ministry_Assignments_ViewAll);
            Assert.AreEqual("Name", assignmentsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[1].Text, "Assignments Header Mismatch");
            Assert.AreEqual("Activity\r\nMinistry", assignmentsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[2].Text, "Assignments Header Mismatch");
            Assert.AreEqual("Roster (RLC) / Folder\r\nBreakout Group", assignmentsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[3].Text, "Assignments Header Mismatch");
            Assert.AreEqual("Schedule / Time\r\nVol / Staff Schedule", assignmentsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[4].Text, "Assignments Header Mismatch");
            Assert.AreEqual("Job\r\nVol / Staff Type", assignmentsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[5].Text, "Assignments Header Mismatch");
            //Assert.AreEqual("Requirements\r\nWebForm", assignmentsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[6].Text, "Assignments Header Mismatch");
            

            //Verify Actions side bar not visible.

            //Verify reToggle gets back to default view
            test.Driver.FindElementById("expand_collapse").Click();
            Assert.AreEqual("Name", assignmentsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[1].Text, "Assignments Header Mismatch");
            //Assert.AreEqual("Activity\r\nMinistry", assignmentsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[2].Text, "Assignments Header Mismatch");
            Assert.AreEqual("Roster (RLC) / Folder\r\nBreakout Group", assignmentsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[3].Text, "Assignments Header Mismatch");
            Assert.AreEqual("Schedule / Time\r\nVol / Staff Schedule", assignmentsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[4].Text, "Assignments Header Mismatch");
            //Assert.AreEqual("…", assignmentsTable.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[5].Text, "Assignments Header Mismatch");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("F1-3760: Verifies that when you search for just Participant assignments no Staff assignments appear")]
        public void Portal_Assignments_ViewAll_Search_ParticipantOnly()
        {
            // Data
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";
            //string participantName = "FT NoEmail";
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            //Navigate to View All Assignment page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

            //Deselect Staff assignments
            Assert.IsTrue(test.Driver.FindElementById(GeneralMinistry.Assignments.Participant_Checkbox).Selected, "Participant Check Box should be selected by default");
            test.Driver.FindElementById(GeneralMinistry.Assignments.Volunteer_Checkbox).Click();
            //test.GeneralMethods.WaitForElementEnabled(By.XPath(GeneralMinistry.Assignments.Apply_Button));
            Assert.IsFalse(test.Driver.FindElementById(GeneralMinistry.Assignments.Volunteer_Checkbox).Selected, "Volunteer Check Box should not be selected by default");
            
            // Search for participants
            //Select Ministry
            test.Portal.Ministry_Assignments_View_All_Filters_Ministry(ministryName, false);

            //Select Activity Filter
            test.Portal.Ministry_Assignments_View_All_Filters_Activity(new string[] { activityName });
           
            //Verify Results
            test.GeneralMethods.WaitForElementDisplayed(By.XPath(TableIds.Ministry_Assignments_ViewAll));

            //Verify that no staff assignments were found
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.ClassName("attendance_staff"));
            //Verify participant assignments were found
            test.GeneralMethods.VerifyElementPresentWebDriver(By.ClassName("attendance_participant"));
            test.Driver.FindElementById(GeneralMinistry.Assignments.More_Button).Click();
            
            // Logout 
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that you can get to the create search screen from the View All page")]
        public void Portal_Assignments_ViewAll_AddNewAssignmentLink()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            //Navigate to View All Assignment page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

            // Click Add new Assignments
            test.Driver.FindElementByLinkText("Add assignment").Click();

            //Verify Add assignment  page
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("search_name"), 30);
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("Advanced Search"));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Name("btn"));
            test.GeneralMethods.VerifyTextPresentWebDriver("Add New Assignment");

            //Logout
            test.Portal.LogoutWebDriver();


        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that when you click on the individual's name it takes you to their individual view page")]
        public void Portal_Assignments_ViewAll_IndividualNameLink()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            //Navigate to View All Assignment page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";

            //Wait for Search button
            //test.GeneralMethods.WaitForElementEnabled(By.XPath(GeneralMinistry.Assignments.Apply_Button));

            // Search for participants
            Assert.IsTrue(test.Driver.FindElementById(GeneralMinistry.Assignments.Participant_Checkbox).Selected, "Participant Check Box should be selected by default");
            //test.Driver.FindElementById(GeneralMinistry.Assignments.Activity).Click();
            new SelectElement(test.Driver.FindElementById(String.Format("{0}-1", GeneralMinistry.Assignments.Ministry_DropDown))).SelectByText(ministryName);
            test.GeneralMethods.WaitForElementEnabled(By.Id(String.Format("{0}-1", GeneralMinistry.Assignments.Activity_Dropdown)));
            new SelectElement(test.Driver.FindElementById(String.Format("{0}-1", GeneralMinistry.Assignments.Activity_Dropdown))).SelectByText(activityName);
            test.GeneralMethods.SelectCheckbox(By.Id(GeneralMinistry.Assignments.Volunteer_Checkbox), false);
            test.GeneralMethods.WaitForElementEnabled(By.XPath(GeneralMinistry.Assignments.Apply_Button));
            test.Driver.FindElementByXPath(GeneralMinistry.Assignments.Apply_Button).Click();
            test.GeneralMethods.WaitForElementDisplayed(By.XPath(TableIds.Ministry_Assignments_ViewAll));
            
            //Click on Participant name
            string participantName = test.Driver.FindElementByXPath("//tbody[@id='all_assign_wrapper']/tr/td[2]/span/a/strong").Text.Trim();
            test.Driver.FindElementByXPath("//tbody[@id='all_assign_wrapper']/tr/td[2]/span/a/strong").Click();
            
            //Verify Individual Page
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("household_individual_name"), 30);
            test.GeneralMethods.VerifyTextPresentWebDriver(participantName);
            test.GeneralMethods.VerifyTextPresentWebDriver("Individual Detail");
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("View the household"));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("Add an individual"));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("Edit privacy settings"));

            //Logout
            test.Portal.LogoutWebDriver();

        }

        #endregion View All

        #region Filters

        //[Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the search filter for Ministry is functioning")]
        public void Portal_Assignments_ViewAll_SearchBy_Ministry()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to the Activities/View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("load_button"));

            //Select Ministy options
            test.Portal.Ministry_Assignments_View_All_Filters_Ministry("A Test Ministry");
            
            // Search for participants
            test.GeneralMethods.VerifyElementDisplayedWebDriver(By.LinkText("Matthew Sneeden"));
            //test.GeneralMethods.VerifyTextPresentWebDriver("A Test Ministry");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Stuart Platt")]
        [Description("Verifies that the Activity filter is functioning as designed")]
        public void Portal_Assignments_ViewAll_SearchBy_Activity()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to the Activities/View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);
            test.GeneralMethods.WaitForElement(test.Driver, By.Id(GeneralMinistry.Assignments.More_Button));

            //Select Ministry
            test.Portal.Ministry_Assignments_View_All_Filters_Ministry("A Test Ministry", false);
            
            //Select Activity Filter
            test.Portal.Ministry_Assignments_View_All_Filters_Activity(new string[] { "A Test Activity" });
            
            // Search for participants
            test.GeneralMethods.VerifyElementDisplayedWebDriver(By.LinkText("FT Tester"));
            
            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Roster filters are working as designed")]
        public void Portal_Assignments_ViewAll_SearchBy_Rosters()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            string activityName = "A Test Activity";
            string ministryName = "A Test Ministry";
            string rosterName = "A Test RLC";
            string folderName = "All Rosters";
            string individualName = "FT Tester";

            //Create Assignment
            test.Portal.Ministry_ParticipantAssignment_Individual_Create(individualName, ministryName, activityName, rosterName, null, null, "View assignments list", false);

            // Navigate to the Activities/View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(GeneralMinistry.Assignments.Add_Assignment));

            //Select Ministry
            test.Portal.Ministry_Assignments_View_All_Filters_Ministry(ministryName, false);

            //Select Activity Filter
            test.Portal.Ministry_Assignments_View_All_Filters_Activity(new string[] { activityName }, false);

            //Select roster options 
            test.Portal.Ministry_Assignments_View_All_Filters_Rosters(new string[] { folderName }, new string[] { rosterName }, null);

            //Verify results
            test.GeneralMethods.VerifyTextPresentWebDriver(individualName);

            //Delete Assignment
            test.Portal.People_ParticipantAssignment_Delete(individualName);

            //Logout
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies the Activity Schedule filters on the Assignment Listing page.")]
        public void Portal_Assignments_ViewAll_SearchBy_ActivitySchedules()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            string activityName = "Assign Schedule Level";
            string ministryName = "A Test Ministry";
            string activitySchedule = "Assign Schedule Level";
            string individualName = "FT Tester";
            string rosterName = "Assign Schedule Level";

            //Create Assignment
            test.Portal.Ministry_ParticipantAssignment_Individual_Create(individualName, ministryName, activityName, rosterName, null, activitySchedule, "View assignments list", false);

            // Navigate to the Activities/View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(GeneralMinistry.Assignments.Add_Assignment));

            //Select Ministry
            test.Portal.Ministry_Assignments_View_All_Filters_Ministry(ministryName, false);

            //Select Activity Filter
            test.Portal.Ministry_Assignments_View_All_Filters_Activity(new string[] { activityName }, false);

            //Select roster options 
            test.Portal.Ministry_Assignments_View_All_Filters_Schedules(new string[] { activitySchedule });

            //Verify results
            test.GeneralMethods.VerifyTextPresentWebDriver(individualName);

            //Delete Assignment
            test.Portal.People_ParticipantAssignment_Delete(individualName);

            //Logout
            test.Portal.LogoutWebDriver();
        }

        //[Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies the Date/Time Filters")]
        public void Portal_Assignments_ViewAll_SearchBy_DateTime()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            string activityName = "Assign Date/Time Level";
            string ministryName = "A Test Ministry";
            string dateTime = string.Format("{0} 6:00:00 PM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1).ToString("M/d/yyyy"));
            string dateTime2 = string.Format("{0} 6:00 PM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1).ToString("ddd, MMM d, yyyy"));
            string individualName = "FT Tester";
            string rosterName = "Assign Date/Time Level";
            string activitySchedule = "Assign Date/Time Level";

            //Create Assignment
            test.Portal.Ministry_ParticipantAssignment_Individual_Create(individualName, ministryName, activityName, rosterName, null, dateTime, "View assignments list", false);

            // Navigate to the Activities/View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(GeneralMinistry.Assignments.Add_Assignment));

            //Select Ministry
            test.Portal.Ministry_Assignments_View_All_Filters_Ministry(ministryName, false);

            //Select Activity Filter
            test.Portal.Ministry_Assignments_View_All_Filters_Activity(new string[] { activityName }, false);

            //Select Roster
            test.Portal.Ministry_Assignments_View_All_Filters_Rosters(null, new string[] { rosterName }, null, false);

            //Select Schedule options 
            test.Portal.Ministry_Assignments_View_All_Filters_Schedules(new string[] { activitySchedule }, false);

            //Select Date Time
            test.Portal.Ministry_Assignments_View_All_Filters_Date_Time(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("M/d/yyyy"), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(2).ToString("M/d/yyyy"), new string[] { dateTime2 });
            
            //Verify results
            test.GeneralMethods.VerifyTextPresentWebDriver(individualName);

            //Delete Assignment
            test.Portal.People_ParticipantAssignment_Delete(individualName);

            //Logout
            test.Portal.LogoutWebDriver();
        }

        //[Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies Date of birth filter")]
        public void Portal_Assignments_ViewAll_SearchBy_DOB()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            string activityName = "A Test Activity";
            string ministryName = "A Test Ministry";
            string rosterName = "A Test RLC";
            string individualName = "DOB Test";
            string startDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("M/d/yyyy");
            string endDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("M/d/yyyy");

            //Create individual 
            test.Portal.People_AddHousehold("DOB", "Test", HouseholdPositionConstants.Head, "Attendee", null, null);
            
            //Create Assignment
            test.Portal.Ministry_ParticipantAssignment_Individual_Create(individualName, ministryName, activityName, rosterName, null, null, "View assignments list", false);

            // Navigate to the Activities/View All page
            //test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(GeneralMinistry.Assignments.Add_Assignment));

            //Select Ministry
            test.Portal.Ministry_Assignments_View_All_Filters_Ministry(ministryName, false);

            //Select Activity Filter
            test.Portal.Ministry_Assignments_View_All_Filters_Activity(new string[] { activityName }, false);

            //Select roster options 
            test.Portal.Ministry_Assignments_View_All_Filters_Rosters(null, new string[] { rosterName }, null, false);

            //Select DOB filter
            test.Portal.Ministry_Assignments_View_All_Filters_DOB(startDate, endDate, GeneralMinistry.Assignments.DateOfBirth_Date_Radio);
            
            //Verify results
            test.GeneralMethods.VerifyTextPresentWebDriver(individualName);

            //Delete Assignment
            test.Portal.People_ParticipantAssignment_Delete(individualName);

            //Logout
            test.Portal.LogoutWebDriver();
        }

        #endregion Filters

        #region Advanced Search
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Uses the Advanced Search to find an individual by their Address")]
        public void Portal_Assignments_AdvancedSearch_Address()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform search using an address
            test.Portal.Ministry_Assignments_AdvancedSearch("FT ParticipantMale", "123 Participant Ln", string.Empty, "---", string.Empty);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }


        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Uses the Advanced Search to find an individual by their Phone number")]
        public void Portal_Assignments_AdvancedSearch_Phone()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform search using a phone number
            test.Portal.Ministry_Assignments_AdvancedSearch("FT ParticipantMale", string.Empty, "555-555-9876", "---", string.Empty);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Uses the Advanced Search to find an individual by their Email address")]
        public void Portal_Assignments_AdvancedSearch_Email()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform search using an email address
            test.Portal.Ministry_Assignments_AdvancedSearch("FT ParticipantMale", string.Empty, "ft.participantmale@email.com", "---", string.Empty);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Uses the Advanced Search to find an individual by their Web address")]
        public void Portal_Assignments_AdvancedSearch_Web()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform search using a web address
            test.Portal.Ministry_Assignments_AdvancedSearch("FT ParticipantMale", string.Empty, "www.participantmale.com", "---", string.Empty);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Uses the Advanced Search to find an individual by their Attribute")]
        public void Portal_Assignments_AdvancedSearch_Attribute()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform search using an attribute
            test.Portal.Ministry_Assignments_AdvancedSearch("FT ParticipantMale", string.Empty, string.Empty, "Automation", string.Empty);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Uses the Advanced Search to find an individual by their Member/Env number")]
        public void Portal_Assignments_AdvancedSearch_MemberEnvelopeNumber()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform search using a member/envelope number
            test.Portal.Ministry_Assignments_AdvancedSearch("FT ParticipantMale", string.Empty, string.Empty, "---", "5559876");

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }
        #endregion Advanced Search

        #region Create Participant Assignment
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Creates a Partcipant Assignment for an individual at the Activity level and then goes back to the View All page")]
        public void Portal_Ministry_Assignments_CreateAssignment_Participant_ActivityLevel_ViewAllPage()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Create participant assignment at activity level for individual and then go to View All page
            test.Portal.Ministry_ParticipantAssignment_Individual_Create("FT ParticipantMale", "A Test Ministry", "Assign Activity Level", "Assign Activity Level", null, null, "View assignments list", false);

            // Delete participant assignment
            test.Portal.People_ParticipantAssignment_Delete("FT ParticipantMale");

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Creates a Partcipant Assignment for an individual at the Activity Schedule level and then goes back to the add assignment search page")]
        public void Portal_Ministry_Assignments_CreateAssignment_Participant_ScheduleLevel_SearchPage()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Create participant assignment at schedule level for individual and then go to Search screen
            test.Portal.Ministry_ParticipantAssignment_Individual_Create("FT ParticipantMale", "A Test Ministry", "Assign Schedule Level", "Assign Schedule Level", null, "Assign Schedule Level", "Create a new assignment", false);

            // Delete participant assignment
            test.Portal.People_ParticipantAssignment_Delete("FT ParticipantMale");

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Creates a Partcipant Assignment for an individual at the Activity Date/Time level and then goes back to the View All page to add another person")]
        public void Portal_Ministry_Assignments_CreateAssignment_Participant_DateTimeLevel_AddAnother()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Create participant assignment at date/time level for individual and then add another to same assignment
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Portal.Ministry_ParticipantAssignment_Individual_Create("FT ParticipantMale", "A Test Ministry", "Assign Date/Time Level", "Assign Date/Time Level", null, string.Format("{0} 6:00:00 PM", now.AddDays(1).ToString("M/d/yyyy")), "Add another person to this assignment", false);

            // Delete participant assignment
            test.Portal.People_ParticipantAssignment_Delete("FT ParticipantMale");

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies that you can fill out all the assigment fields and then Start Over")]
        public void Portal_Ministry_Assignments_CreateAssignment_Participant_StartOver()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Enter assignment information and then click Start Over to search for a new individual
            test.Portal.Ministry_ParticipantAssignment_Individual_Create("FT ParticipantMale", "A Test Ministry", "Assign Activity Level", "Assign Activity Level", null, null, "View assignments list", true);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }
        [Test, RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("bug FO-4091:Creates a Partcipant Assignment for an individual without roster and then goes back to the View All page")]
        public void Portal_Ministry_Assignments_CreateAssignment_Participant_NoSelectRoster()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Create participant assignment at activity level for individual and then go to View All page
            test.Portal.Ministry_ParticipantAssignment_Individual_Create("FT ParticipantMale", "A Test Ministry", "Assign Activity Level", null, null, null, "View assignments list", false);

            // Delete participant assignment
            test.Portal.People_ParticipantAssignment_DeleteForce("FT ParticipantMale");

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }
        #endregion Create Participant Assignment

        #region Create Staff Assignment
        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Creates a Staffing Assignment for an individual at the Activity level with no job and then goes back to the View All page")]
        public void Ministry_Assignments_CreateAssignment_Staff_ActivityLevel_NoJob_ViewAllPage()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Create staff assignment at activity level with no job and then go to View All page
            test.Portal.Ministry_StaffAssignment_Individual_Create("FT ParticipantMale", "Staff", "Active", "A Test Ministry", "Assign Activity Level", "- Assign Activity Level", null, null, null, "View assignments list", false);

            // Delete staff assignment
            test.Portal.People_StaffAssignment_Delete("FT ParticipantMale");

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Creates a Staffing Assignment at the schedule level with a job where requirements are met and then goes to the search page")]
        public void Ministry_Assignments_CreateAssignment_Staff_ScheduleLevel_Job_RequirementsMet_SearchPage()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Create staff assignment at the schedule level with a job where all requirements are met
            test.Portal.Ministry_StaffAssignment_Individual_Create("FT ParticipantMale", "Staff", "Active", "A Test Ministry", "Assign Schedule Level", "- Assign Schedule Level", "Assign Schedule Level", "Base Schedule", "Dryer", "Create a new assignment", false);

            // Delete staff assignment
            test.Portal.People_StaffAssignment_Delete("FT ParticipantMale");

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Creates a Staffing Assignment at the Date/Time level with a job where optional requirements are not met and then goes to the search page")]
        public void Ministry_Assignments_CreateAssignment_Staff_DateTimeLevel_Job_RequirementsNotMet_Optional_AddAnother()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Create staff assignment at date/time level with a job where optional requirements are not met
            test.Portal.Ministry_StaffAssignment_Individual_Create("FT ParticipantMale", "Staff", "Active", "A Test Ministry", "Assign Date/Time Level", "- Assign Date/Time Level", string.Format("{0} 6:00:00 PM", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1).ToShortDateString()), "Base Schedule", "Washer", "Add another person to this assignment", false);

            // Delete staff assignment
            test.Portal.People_StaffAssignment_Delete("FT ParticipantMale");

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Creates a Staffing Assignment for an individual at the Activity level with a job where required requirements are not met and then takes you take to the search page")]
        public void Ministry_Assignments_CreateAssignment_Staff_ActivityLevel_Job_RequirementsNotMet_Required()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Create staff assignment for individual with a job where required requirements are not met
            test.Portal.Ministry_StaffAssignment_Individual_Create("FT ParticipantFemale", "Staff", "Active", "A Test Ministry", "Assign Activity Level", "- Assign Activity Level", null, "Base Schedule", "Dryer", "View assignments list", false);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies that you can fill out all the staff assigment fields and then Start Over")]
        public void Ministry_Assignments_CreateAssignment_Staff_StartOver()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Fill out all staff assignment fields and then Start Over
            test.Portal.Ministry_StaffAssignment_Individual_Create("FT ParticipantMale", "Staff", "Active", "A Test Ministry", "Assign Activity Level", "- Assign Activity Level", null, "Base Schedule", "Dryer", "View assignments list", true);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }
        #endregion Create Staff Assignment

        #region Participant Requirements

        #endregion Participant Requirements

        #region Actions

        #region Add To Group
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to group Modal can create a new temp group from assigned individuals")]
        public void Portal_Assignments_ViewAll_AddToGroup_AssignedIndividuals_TempGroup_New()
        {
            //Group Name Randomizer
            string groupName = string.Format("TempGroup-{0}", Guid.NewGuid().ToString().Substring(0, 4));

            // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Create Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup("A Test Ministry", new string[] { "A Test Activity" }, groupName, "Temporary Group", GeneralMinistry.Assignments.Add_To_Assigned, true, true);

                //Logout 
                test.Portal.LogoutWebDriver();

                //Delete Group
                base.SQL.Groups_Temporary_Delete(15, groupName);
            

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to Group action can create a new people list from assigned individuals")]
        public void Portal_Assignments_ViewAll_AddToGroup_AssignedIndividuals_PeopleList_New()
        {
            //Group Name Randomizer
            string peopleListName = string.Format("PeopleList-{0}", Guid.NewGuid().ToString().Substring(0, 4));

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


                //Login
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Create Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup("A Test Ministry", new string[] { "A Test Activity" }, peopleListName, "People List", GeneralMinistry.Assignments.Add_To_Assigned, true, true);

                //Delete Group
                //test.Portal.Groups_PeopleList_Delete_WebDriver(peopleListName);
                test.SQL.Groups_PeopleLists_Delete(15, peopleListName);

                test.Portal.LogoutWebDriver();
            
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to Group action can create a new group from assigned individuals")]
        public void Portal_Assignments_ViewAll_AddToGroup_AssignedIndividuals_Group_New()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Group Name Randomizer
            string groupName = string.Format("Group-{0}", Guid.NewGuid().ToString().Substring(0, 4));

                // Login to Portal
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Create Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup("A Test Ministry", new string[] { "A Test Activity" }, groupName, "Automation Tests", GeneralMinistry.Assignments.Add_To_Assigned, true, true);

                //Delete Group
                //test.Portal.Groups_Group_Delete_WebDriver(groupName);
                test.SQL.Groups_Group_Delete(15, groupName);

                //Logout 
                test.Portal.LogoutWebDriver();

        }
        
        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to Group Action is able to create a new temp group for the parents of assigned individuals")]
        public void Portal_Assignments_ViewAll_AddToGroup_ParentsOfIndividuals_TempGroup_New()
        {
            //Group Name Randomizer
            string tempName = string.Format("TempGroup-{0}", Guid.NewGuid().ToString().Substring(0, 4));

                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Create Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup("A Test Ministry", new string[] { "A Test Activity" }, tempName, "Temporary Group", GeneralMinistry.Assignments.Add_To_Parents, true, true);

                //Logout 
                test.Portal.LogoutWebDriver();

               //Delete Group
                base.SQL.Groups_Temporary_Delete(15, tempName);
            
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to Group Action is able to create a new people list for the parents of assigned individuals")]
        public void Portal_Assignments_ViewAll_AddToGroup_ParentsOfIndividuals_PeopleList_New()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            
            //Group Name Randomizer
            string peopleListName = string.Format("1 PeopleList-{0}", Guid.NewGuid().ToString().Substring(0, 4));
            

                // Login to Portal  
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Create Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup("A Test Ministry", new string[] { "A Test Activity" }, peopleListName, "People List", GeneralMinistry.Assignments.Add_To_Parents, true, true);

            //Delete Group
            //test.Portal.Groups_PeopleList_Delete_WebDriver(peopleListName);
                test.SQL.Groups_PeopleLists_Delete(15, peopleListName);

            //Logout 
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to Group Action is able to create a new group for the parents of assigned individuals")]
        public void Portal_Assignments_ViewAll_AddToGroup_ParentsOfIndividuals_Group_New()
        {
            
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Group Name Randomizer
            string groupName = string.Format("Group-{0}", Guid.NewGuid().ToString().Substring(0, 4));


                // Login to Portal
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Create Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup("A Test Ministry", new string[] { "A Test Activity" }, groupName, "Automation Tests", GeneralMinistry.Assignments.Add_To_Parents, true, true);

                //Delete Group
                //test.Portal.Groups_Group_Delete_WebDriver(groupName);
                test.SQL.Groups_Group_Delete(15, groupName);

                //Logout 
                test.Portal.LogoutWebDriver();
            
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to group action can add a household to a new temp group from a list of assigned individuals.")]
        public void Portal_Assignments_ViewAll_AddToGroup_Household_TempGroup_New()
        {

            //Group Name Randomizer
            string tempName = string.Format("TempGroup-{0}", Guid.NewGuid().ToString().Substring(0, 4));

                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Create Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup("A Test Ministry", new string[] { "A Test Activity" }, tempName, "Temporary Group", GeneralMinistry.Assignments.Add_To_FamilyHousehold, true, true);

                //Logout 
                test.Portal.LogoutWebDriver();

                //Delete Group
                base.SQL.Groups_Temporary_Delete(15, tempName);
            
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to group action can add a household to a new people list from a list of assigned individuals.")]
        public void Portal_Assignments_ViewAll_AddToGroup_Household_PeopleList_New()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Group Name Randomizer
            string peopleListName = string.Format("1 PeopleList-{0}", Guid.NewGuid().ToString().Substring(0, 4));

                // Login to Portal
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Create Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup("A Test Ministry", new string[] { "A Test Activity" }, peopleListName, "People List", GeneralMinistry.Assignments.Add_To_FamilyHousehold, true, true);

                //Delete Group
                //test.Portal.Groups_PeopleList_Delete_WebDriver(peopleListName);
                test.SQL.Groups_PeopleLists_Delete(15, peopleListName);

                //Logout 
                test.Portal.LogoutWebDriver();
            
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to group action can add a household to a new group from a list of assigned individuals.")]
        public void Portal_Assignments_ViewAll_AddToGroup_Household_Group_New()
        {
            
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Group Name Randomizer
            string groupName = string.Format("Group-{0}", Guid.NewGuid().ToString().Substring(0, 4));


                // Login to Portal
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Create Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup("A Test Ministry", new string[] { "A Test Activity" }, groupName, "Automation Tests", GeneralMinistry.Assignments.Add_To_FamilyHousehold, true, true);

                //Delete Group
                //test.Portal.Groups_Group_Delete_WebDriver(groupName);
                test.SQL.Groups_Group_Delete(15, groupName);

                //Logout 
                test.Portal.LogoutWebDriver();
            
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to Group action can add the children of assigned individuals to a new Temporary Group")]
        public void Portal_Assignments_ViewAll_AddToGroup_ChildrenOfIndividuals_TempGroup_New()
        {
            //Group Name Randomizer
            string tempName = string.Format("TempGroup-{0}", Guid.NewGuid().ToString().Substring(0, 4));


                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Create Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup("A Test Ministry", new string[] { "A Test Activity" }, tempName, "Temporary Group", GeneralMinistry.Assignments.Add_To_Children, true, true);

                //Logout 
                test.Portal.LogoutWebDriver();

                //Delete Group
                base.SQL.Groups_Temporary_Delete(15, tempName);
            
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to Group action can add the children of assigned individuals to a new People List")]
        public void Portal_Assignments_ViewAll_AddToGroup_ChildrenOfIndividuals_PeopleList_New()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Group Name Randomizer
            string peopleListName = string.Format("1 PeopleList-{0}", Guid.NewGuid().ToString().Substring(0, 4));


                // Login to Portal
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Create Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup("A Test Ministry", new string[] { "A Test Activity" }, peopleListName, "People List", GeneralMinistry.Assignments.Add_To_Children, true, true);

                //Delete Group
                //test.Portal.Groups_PeopleList_Delete_WebDriver(peopleListName);
                test.SQL.Groups_PeopleLists_Delete(15, peopleListName);

                //Logout 
                test.Portal.LogoutWebDriver();
            
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to Group action can add the children of assigned individuals to a new Group")]
        public void Portal_Assignments_ViewAll_AddToGroup_ChildrenOfIndividuals_Group_New()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Group Name Randomizer
            string groupName = string.Format("Group-{0}", Guid.NewGuid().ToString().Substring(0, 4));


                // Login to Portal
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Create Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup("A Test Ministry", new string[] { "A Test Activity" }, groupName, "Automation Tests", GeneralMinistry.Assignments.Add_To_Children, true, true);

                //Delete Group
                //test.Portal.Groups_Group_Delete_WebDriver(groupName);
                test.SQL.Groups_Group_Delete(15, groupName);

                //Logout 
                test.Portal.LogoutWebDriver();
            
        }

        [Test, RepeatOnFailure, Timeout(1000)]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to Group action can add assigned individuals into an existing temporary group")]
        public void Portal_Assignments_ViewAll_AddToGroup_AssignedIndividuals_TempGroup()
        {

            //Group Name Randomizer
            string tempName = string.Format("TempGroup-{0}", Guid.NewGuid().ToString().Substring(0, 4));
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";
            

                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Create Group
                string memberCountAdded = test.Portal.Ministry_Assignments_View_All_AddToGroup(ministryName, new string[] { activityName }, tempName, "Temporary Group", GeneralMinistry.Assignments.Add_To_Assigned, true, true, null, false);

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Add to Existing Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup(ministryName, new string[] { activityName }, tempName, "Temporary Group", GeneralMinistry.Assignments.Add_To_Assigned, false, true, memberCountAdded);

                //Logout 
                test.Portal.LogoutWebDriver();

                //Delete Group
                base.SQL.Groups_Temporary_Delete(15, tempName);
            
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to Group action can add assigned individuals into an existing people list")]
        public void Portal_Assignments_ViewAll_AddToGroup_AssignedIndividuals_PeopleList()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Group Name Randomizer
            string peopleListName = string.Format("1 PeopleList-{0}", Guid.NewGuid().ToString().Substring(0, 4));
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";


                // Login to Portal
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Create Group
                string memberCountAdded = test.Portal.Ministry_Assignments_View_All_AddToGroup(ministryName, new string[] { activityName }, peopleListName, "People List", GeneralMinistry.Assignments.Add_To_Assigned, true, true, null, false);

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Add to Existing Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup(ministryName, new string[] { activityName }, peopleListName, "People List", GeneralMinistry.Assignments.Add_To_Assigned, false, true, memberCountAdded);
            
                //Delete Group
                //test.Portal.Groups_PeopleList_Delete_WebDriver(peopleListName);
                test.SQL.Groups_PeopleLists_Delete(15, peopleListName);

                //Logout 
                test.Portal.LogoutWebDriver();
            
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to Group action can add assigned individuals into an existing group")]
        public void Portal_Assignments_ViewAll_AddToGroup_AssignedIndividuals_Group()
        {
            
            //Group Name Randomizer
            string groupName = string.Format("Group-{0}", Guid.NewGuid().ToString().Substring(0, 4));
            string groupType = "Automation Tests";

            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

                // Login to Portal
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Create Group
                test.SQL.Groups_Group_Create(15, groupType, "FT Tester", groupName, "", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Add to Existing Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup("A Test Ministry", new string[] { "A Test Activity" }, groupName, groupType, GeneralMinistry.Assignments.Add_To_Assigned, false, true);

                //Logout 
                test.Portal.LogoutWebDriver();

                //Delete Group
                //test.Portal.Groups_Group_Delete_WebDriver(groupName);                
                test.SQL.Groups_Group_Delete(15, groupName);
                
            
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to Group action can add the parents of assigned individuals to an existing temp group")]
        public void Portal_Assignments_ViewAll_AddToGroup_ParentsOfIndividuals_TempGroup()
        {

            //Group Name Randomizer
            string tempName = string.Format("TempGroup-{0}", Guid.NewGuid().ToString().Substring(0, 4));
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";

            
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Create Group
                string memberCountAdded = test.Portal.Ministry_Assignments_View_All_AddToGroup(ministryName, new string[] { activityName }, tempName, "Temporary Group", GeneralMinistry.Assignments.Add_To_Parents, true, true, null, false);

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Add to Existing Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup(ministryName, new string[] { activityName }, tempName, "Temporary Group", GeneralMinistry.Assignments.Add_To_Parents, false, true, memberCountAdded);

                //Logout 
                test.Portal.LogoutWebDriver();
            
                //Delete Group
                base.SQL.Groups_Temporary_Delete(15, tempName);
            
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to Group action can add the parents of assigned individuals to an existing people list")]
        public void Portal_Assignments_ViewAll_AddToGroup_ParentsOfIndividuals_PeopleList()
        {
            
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            
            //Group Name Randomizer
            string peopleListName = string.Format("1 PeopleList-{0}", Guid.NewGuid().ToString().Substring(0, 4));
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";

                // Login to Portal
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Create Group
                string memberCountAdded = test.Portal.Ministry_Assignments_View_All_AddToGroup(ministryName, new string[] { activityName }, peopleListName, "People List", GeneralMinistry.Assignments.Add_To_Parents, true, true, null, false);

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Add to Existing Group
                string memberCountAll = test.Portal.Ministry_Assignments_View_All_AddToGroup(ministryName, new string[] { activityName }, peopleListName, "People List", GeneralMinistry.Assignments.Add_To_Parents, false, true, memberCountAdded);

                //Logout 
                test.Portal.LogoutWebDriver();

            //Delete Group
            test.SQL.Groups_PeopleLists_Delete(15, peopleListName);
            //test.Portal.Groups_PeopleList_Delete_WebDriver(peopleListName);

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to Group action can add the parents of assigned individuals to an existing group")]
        public void Portal_Assignments_ViewAll_AddToGroup_ParentsOfIndividuals_Group()
        {
            
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Group Name Randomizer
            string groupName = string.Format("Group-{0}", Guid.NewGuid().ToString().Substring(0, 4));
            string groupType = "Automation Tests";


                // Login to Portal
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Create Group
                test.SQL.Groups_Group_Create(15, groupType, "FT Tester", groupName, "", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("M/d/yyyy"));

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Add to Existing Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup("A Test Ministry", new string[] { "A Test Activity" }, groupName, groupType, GeneralMinistry.Assignments.Add_To_Parents, false, true);
            
                //Logout 
                test.Portal.LogoutWebDriver();

                //Delete Group
                test.SQL.Groups_Group_Delete(15, groupName);

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to group action can add the households of assigned individuals to an existing Temporary Group")]
        public void Portal_Assignments_ViewAll_AddToGroup_Household_TempGroup()
        {

            //Group Name Randomizer
            string tempName = string.Format("TempGroup-{0}", Guid.NewGuid().ToString().Substring(0, 4));
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";

                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Create Group
                string memberCountAdded = test.Portal.Ministry_Assignments_View_All_AddToGroup(ministryName, new string[] { activityName }, tempName, "Temporary Group", GeneralMinistry.Assignments.Add_To_FamilyHousehold, true, true, null, false);

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Add to Existing Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup(ministryName, new string[] { activityName }, tempName, "Temporary Group", GeneralMinistry.Assignments.Add_To_FamilyHousehold, false, true, memberCountAdded);

                //Logout 
                test.Portal.LogoutWebDriver();

                //Delete Group
                base.SQL.Groups_Temporary_Delete(15, tempName);
            
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to group action can add the households of assigned individuals to an existing People List")]
        public void Portal_Assignments_ViewAll_AddToGroup_Household_PeopleList()
        {
            
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Group Name Randomizer
            string peopleListName = string.Format("1 PeopleList-{0}", Guid.NewGuid().ToString().Substring(0, 4));
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";

                // Login to Portal
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Create Group
                string memberCountAdded = test.Portal.Ministry_Assignments_View_All_AddToGroup(ministryName, new string[] { activityName }, peopleListName, "People List", GeneralMinistry.Assignments.Add_To_FamilyHousehold, true, true, null, false);

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Add to Existing Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup(ministryName, new string[] { activityName }, peopleListName, "People List", GeneralMinistry.Assignments.Add_To_FamilyHousehold, false, true, memberCountAdded);

                //Delete Group
                //test.Portal.Groups_PeopleList_Delete_WebDriver(peopleListName);
                test.SQL.Groups_PeopleLists_Delete(15, peopleListName);

                //Logout 
                test.Portal.LogoutWebDriver();
            
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to group action can add the households of assigned individuals to an existing Group")]
        public void Portal_Assignments_ViewAll_AddToGroup_Household_Group()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Group Name Randomizer
            string groupName = string.Format("Group-{0}", Guid.NewGuid().ToString().Substring(0, 4));
            string groupType = "Automation Tests";

                // Login to Portal
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Create Group
                test.SQL.Groups_Group_Create(15, groupType, "FT Tester", groupName, "", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("M/d/yyyy"));

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Add to Existing Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup("A Test Ministry", new string[] { "A Test Activity" }, groupName, groupType, GeneralMinistry.Assignments.Add_To_FamilyHousehold, false, true);

                //Delete Group
                //test.Portal.Groups_Group_Delete_WebDriver(groupName);
                test.SQL.Groups_Group_Delete(15, groupName);

                //Logout 
                test.Portal.LogoutWebDriver();
            
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to Group action can add the children of assigned individuals to an existing Temporary Group")]
        public void Portal_Assignments_ViewAll_AddToGroup_ChildrenOfIndividuals_TempGroup()
        {
            
            //Group Name Randomizer
            string tempName = string.Format("TempGroup-{0}", Guid.NewGuid().ToString().Substring(0, 4));
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";


                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Create Group
                string memberCountAdded = test.Portal.Ministry_Assignments_View_All_AddToGroup(ministryName, new string[] { activityName }, tempName, "Temporary Group", GeneralMinistry.Assignments.Add_To_Children, true, true, null, false);

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Add to Existing Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup(ministryName, new string[] { activityName }, tempName, "Temporary Group", GeneralMinistry.Assignments.Add_To_Children, false, true, memberCountAdded);

                //Logout 
                test.Portal.LogoutWebDriver();

                //Delete Group
                base.SQL.Groups_Temporary_Delete(15, tempName);
            
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to Group action can add the children of assigned individuals to an existing People List")]
        public void Portal_Assignments_ViewAll_AddToGroup_ChildrenOfIndividuals_PeopleList()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            //Group Name Randomizer
            string peopleListName = string.Format("1 PeopleList-{0}", Guid.NewGuid().ToString().Substring(0, 4));
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";

                // Login to Portal
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Create Group
                string memberCountAdded = test.Portal.Ministry_Assignments_View_All_AddToGroup(ministryName, new string[] { activityName }, peopleListName, "People List", GeneralMinistry.Assignments.Add_To_Children, true, true, null, false);

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Add to Existing Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup(ministryName, new string[] { activityName }, peopleListName, "People List", GeneralMinistry.Assignments.Add_To_Children, false, true, memberCountAdded);

                //Delete Group
                //test.Portal.Groups_PeopleList_Delete_WebDriver(peopleListName);
                test.SQL.Groups_PeopleLists_Delete(15, peopleListName);

                //Logout 
                test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the Add to Group action can add the children of assigned individuals to an existing Group")]
        public void Portal_Assignments_ViewAll_AddToGroup_ChildrenOfIndividuals_Group()
        {
            
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            //Group Name Randomizer
            string groupName = string.Format("Group-{0}", Guid.NewGuid().ToString().Substring(0, 4));
            string groupType = "Automation Tests";

                // Login to Portal
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                //Create Group
                test.SQL.Groups_Group_Create(15, groupType, "FT Tester", groupName, "", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("M/d/yyyy"));

                //Navigate to Assigments View all page
                test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

                //Add to Existing Group
                test.Portal.Ministry_Assignments_View_All_AddToGroup("A Test Ministry", new string[] { "A Test Activity" }, groupName, groupType, GeneralMinistry.Assignments.Add_To_Children, false, true);

                //Delete Group
                //test.Portal.Groups_Group_Delete_WebDriver(groupName);
                test.SQL.Groups_Group_Delete(15, groupName);

                //Logout 
                test.Portal.LogoutWebDriver();
            
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies Error message when members were not added to a group")]
        public void Portal_Assignments_ViewAll_AddToGroup_NoMembersAdded()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            //Navigate to the Assignments View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);

            //Group Name Randomizer
            string groupName = string.Format("Group-{0}", Guid.NewGuid().ToString().Substring(0, 4));
            string groupType = "Automation Tests";
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";

            //Create a group
            test.Portal.Ministry_Assignments_View_All_AddToGroup(ministryName, new string[] { activityName }, groupName, groupType, GeneralMinistry.Assignments.Add_To_Assigned, true, true);

            //Navigate to the Assignments View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);
                
            //Attempt to add same individuals to same group
            test.Portal.Ministry_Assignments_View_All_AddToGroup(ministryName, new string[] { activityName }, groupName, groupType, GeneralMinistry.Assignments.Add_To_Assigned, false, false);

            //Verify Error message
            test.GeneralMethods.WaitForElementDisplayed(By.XPath(GeneralMinistry.Assignments.Add_To_Group_Error_Ok));
            string headerText = test.Driver.FindElementByXPath("//div[@class='icon_alert_large']/h2/strong").Text.Trim();
            string errorText = test.Driver.FindElementByXPath("//div[@class='modal_no_scroll modal_content']/div[2]").Text.Trim();
            string errorText2 = test.Driver.FindElementByXPath("//div[@class='modal_no_scroll modal_content']/div[3]").Text.Trim();
            Assert.AreEqual("Group Was Not Created/Updated", headerText);
            Assert.AreEqual("Using the selected assignments in combination with the WHO selection in the Add to Group window, no one was qualified to be placed in the group. No group was created or updated as a result.", errorText);
            Assert.AreEqual("Please choose a different WHO selection or assignments to Add to Group.", errorText2);
            
            //Click Ok
            test.Driver.FindElementByXPath(GeneralMinistry.Assignments.Add_To_Group_Error_Ok).Click();

            //Delete Group
            System.Threading.Thread.Sleep(5000);
            test.Portal.Groups_Group_Delete_WebDriver(groupName);

            //Logout
            test.Portal.LogoutWebDriver();

        }
        #endregion Add To Group

        #region Email
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("F1-3916: Verifies a user can select a number of assignments and send them an email")]
        public void Ministry_Assignments_ViewAll_Email()
        {
            // Data
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to the assignments view all page
            test.Portal.Ministry_Assignments_View_All_WebDriver();

            // Select the Ministry/Activity combo
            test.Portal.Ministry_Assignments_View_All_Filters_Ministry(ministryName, false);
            System.Threading.Thread.Sleep(2000);
            test.Portal.Ministry_Assignments_View_All_Filters_Activity(new string[] {activityName}, false);

            // Click apply
            test.GeneralMethods.WaitForElementEnabled(By.XPath(GeneralMinistry.Assignments.Apply_Button));
            test.Driver.FindElement(By.XPath(GeneralMinistry.Assignments.Apply_Button)).Click();
            test.GeneralMethods.WaitForElement(By.Id(GeneralMinistry.Assignments.Table_Results));
            test.GeneralMethods.WaitForElement(By.XPath(TableIds.Ministry_Assignments_ViewAll));

            // Check all assignments
            System.Threading.Thread.Sleep(5000);
            test.Driver.FindElementByXPath(GeneralMinistry.Assignments.Select_All_CheckBox).Click();
            test.GeneralMethods.WaitForElementEnabled(By.Id("massEmail"));

            // Click on Email
            test.Driver.FindElementById(GeneralMinistry.Assignments.Email).Click();
            test.GeneralMethods.WaitForElement(By.Id("ctl00_ctl00_MainContent_content_txtSubject"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }
        #endregion Email

        #region Delete Assignment
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("F1-3469: Deletes multiple assignments from the View All page containing no web form assignments")]
        public void Ministry_Assignments_ViewAll_DeleteMultiple_NOWebForms()
        {
            // Data
            string ministryName = "A Test Ministry";
            string activityName = "Multi-Assignment Delete";
            string rosterName = "Delete Attendees";
            int individualId = base.SQL.People_Individuals_FetchID(15, "David Martin");
            int individual2Id = base.SQL.People_Individuals_FetchID(15, "Kevin Spacey");
            int ministryId = base.SQL.Ministry_Ministries_FetchID(15, ministryName);
            base.SQL.Ministry_Activities_Delete_Activity(15, activityName);
            base.SQL.Ministry_Activities_Create(15, ministryId, activityName, rosterName);
            base.SQL.Ministry_ParticipantAssignments_Create(15, individualId, activityName, rosterName);
            base.SQL.Ministry_ParticipantAssignments_Create(15, individual2Id, activityName, rosterName);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // Delete multiple assignments containing no web forms
                test.Portal.Ministry_Assignments_ViewAll_Delete_All(ministryName, new string[] { activityName });
            }
            finally
            {
                // Clean up
                base.SQL.Ministry_Activities_Delete_Activity(15, activityName);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }
            

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3469: Deletes multiple assignments from the View All page containing web form assignments")]
        public void Ministry_Assignments_ViewAll_DeleteMultiple_WITHWebForms()
        {
            // Data
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Delete multiple assignments containing web forms
            test.Portal.Ministry_Assignments_ViewAll_Delete_All(ministryName, new string[] { activityName }, true);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3469: Attempts to delete multiple assignments with a user without Ministry Write permission")]
        public void Ministry_Assignments_ViewAll_DeleteMultiple_NOWritePermission()
        {
            // Data
            string ministryName = "A Test Ministry";
            string activityName = "A Test Activity";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.nominwrite", "FT4life!", "dc");

            // Delete multiple assignments using a portal user with no "Write" permissions
            test.Portal.Ministry_Assignments_ViewAll_Delete_All(ministryName, new string[] { activityName }, false, false);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-4087: Attempts to delete a single assignment from the View All page that is not from a web form")]
        public void Ministry_Assignments_ViewALl_DeleteSingle_NoWebForms()
        {

        }

        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-4087: Attempts to delete a single assignment from the View All page that IS tied to a web form")]
        public void Ministry_Assignments_ViewAll_DeleteSingle_WITHWebForms()
        {

        }
        #endregion Delete Assignment

        #region Edit Assignment
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("F1-3970: Attempts to edit an assignment from the View All page")]
        public void Ministry_Assignments_ViewAll_Edit()
        {
            // Data
            string ministryName = "A Test Ministry";
            string activityName = "Assignment Edit";
            string rosterName = "Edit Attendees";
            string individualName = "David Martin";
            string newMinistryName = "Auto Test Ministry";
            string newActivityName = "Edit Attendees - New";
            string rosterGroup = "Other Rosters";
            string newRosterName = "New Attendees";
            int individualId = base.SQL.People_Individuals_FetchID(15, individualName);
            int ministryId = base.SQL.Ministry_Ministries_FetchID(15, ministryName);
            int ministry2Id = base.SQL.Ministry_Ministries_FetchID(15, newMinistryName);


            // Clean up
            base.SQL.Ministry_ParticipantAssignments_Delete(15, individualId, activityName, rosterName);
            base.SQL.Ministry_ParticipantAssignments_Delete(15, individualId, newActivityName, newRosterName);
            base.SQL.Ministry_Activities_Delete_Activity(15, activityName);
            base.SQL.Ministry_Activities_Delete_Activity(15, newActivityName);

            // Setup
            base.SQL.Ministry_Activities_Create(15, ministryId, activityName, rosterName);
            base.SQL.Ministry_Activities_Create(15, ministry2Id, newActivityName, newRosterName);
            base.SQL.Ministry_ActivityRLCGroups_Create(15, newActivityName, rosterGroup);
            base.SQL.Ministry_ParticipantAssignments_Create(15, individualId, activityName, rosterName);

            try
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // Attempt to edit an assignment from the View All page
                test.Portal.Ministry_Assignments_ViewAll_Edit_Participant(individualName, ministryName, new string[] { activityName }, true, false, newMinistryName, newActivityName, newRosterName);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }
            finally
            {
                // Clean up
                base.SQL.Ministry_ParticipantAssignments_Delete(15, individualId, activityName, rosterName);
                base.SQL.Ministry_ParticipantAssignments_Delete(15, individualId, newActivityName, newRosterName);
                base.SQL.Ministry_Activities_Delete_Activity(15, activityName);
                base.SQL.Ministry_Activities_Delete_Activity(15, newActivityName);
            }
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3970: Attempts to edit an assignment from the View All page with a user with NO ministry write permissions")]
        public void Ministry_Assignments_ViewAll_Edit_NOWritePermissions()
        {
            // Data
            string ministryName = "A Test Ministry";
            string activityName = "Assignment Edit - NoRights";
            string rosterName = "NoRights Attendees";
            string individualName = "David Martin";
            int individualId = base.SQL.People_Individuals_FetchID(15, individualName);
            int ministryId = base.SQL.Ministry_Ministries_FetchID(15, ministryName);
            // Clean up
            base.SQL.Ministry_ParticipantAssignments_Delete(15, individualId, activityName, rosterName);
            base.SQL.Ministry_Activities_Delete_Activity(15, activityName);

            // Setup
            base.SQL.Ministry_Activities_Create(15, ministryId, activityName, rosterName);
            base.SQL.Ministry_ParticipantAssignments_Create(15, individualId, activityName, rosterName);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.nominwrite", "FT4life!", "dc");

            // Attempt to delete a single assignment with a user without "write" permissions
            test.Portal.Ministry_Assignments_ViewAll_Edit_Participant(individualName, ministryName, new string[] { activityName }, false);

            // Logout of portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Ministry_ParticipantAssignments_Delete(15, individualId, activityName, rosterName);
            base.SQL.Ministry_Activities_Delete_Activity(15, activityName);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-3970: Attempts to edit an assignment from the View All page but clicks Cancel instead")]
        public void Ministry_Assignments_ViewAll_Edit_Cancel()
        {
            // Data
            string ministryName = "A Test Ministry";
            string activityName = "Assignment Edit - Cancel";
            string rosterName = "Cancel Attendees";
            string individualName = "David Martin";
            int individualId = base.SQL.People_Individuals_FetchID(15, individualName);
            int ministryId = base.SQL.Ministry_Ministries_FetchID(15, ministryName);

            // Clean up
            base.SQL.Ministry_ParticipantAssignments_Delete(15, individualId, activityName, rosterName);
            base.SQL.Ministry_Activities_Delete_Activity(15, activityName);

            // Setup
            base.SQL.Ministry_Activities_Create(15, ministryId, activityName, rosterName);
            base.SQL.Ministry_ParticipantAssignments_Create(15, individualId, activityName, rosterName);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Attempt to edit an assignment from the View All page and click Cancel
            test.Portal.Ministry_Assignments_ViewAll_Edit_Participant(individualName, ministryName, new string[] { activityName }, true, true);

            // Logout of portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Ministry_ParticipantAssignments_Delete(15, individualId, activityName, rosterName);
            base.SQL.Ministry_Activities_Delete_Activity(15, activityName);

        }
        #endregion Edit Assignment

        #endregion Actions

        #region Swooshy Back Arrow
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("F1-4322: Verifies the back arrow on the Assignments View All page navigates back to the Activities View All page.")]
        public void Ministry_Assignments_ViewAll_BackArrow()
        {
            // Data
            #region Data
            int churchId = 15;
            string individualName = "FT ParticipantMale";
            string ministryName = "A Test Ministry";
            string activityName = "Assign Activity Level";
            int activityId = this.SQL.Ministry_Activities_FetchID(churchId, activityName);
            string rosterGroupName = "Roster Grouping";
            int rosterGroupId = this.SQL.Ministry_RosterFolders_FetchID(churchId, rosterGroupName, activityId);
            string rosterName = "Assign Activity Level";
            #endregion Data

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to the Assignments View All page
            test.Portal.Ministry_Assignments_View_All_WebDriver();

            // Click on the back arrow
            test.Driver.FindElementById(GeneralMinistry.Assignments.Tab_Back_Arrow).Click();
            test.GeneralMethods.WaitForElement(By.Id(GeneralMinistry.Activities.Add_Activity));

            // Click on assignment pill
            test.Portal.Ministry_Activities_View_All_AssignmentPill(churchId, activityName, true);

            // Click on the back arrow
            test.Driver.FindElementById(GeneralMinistry.Assignments.Tab_Back_Arrow).Click();
            test.GeneralMethods.WaitForElement(By.Id(GeneralMinistry.Activities.Add_Activity));

            // Click on an activity
            test.Portal.Ministry_Activities_Search(ministryName, activityName);
            test.Driver.FindElementByLinkText(activityName).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Manage Staffing Needs"));

            // Click on the Rosters tab
            test.Driver.FindElementByLinkText("Rosters").Click();
            System.Threading.Thread.Sleep(5000);
            test.GeneralMethods.WaitForElementDisplayed(By.LinkText(GeneralMinistry.Activities.RosterFolder_Add));

            // Click on assignment pill
            test.Portal.Ministry_Activities_Rosters_AssignmentPill(churchId, activityName, rosterGroupName, rosterName, false, true);

            // Click on the back arrow
            test.Driver.FindElementById(GeneralMinistry.Assignments.Tab_Back_Arrow).Click();
            test.GeneralMethods.WaitForElement(By.LinkText(GeneralMinistry.Activities.RosterFolder_Add));

            // Click on assignment pill
            test.Driver.FindElementByXPath(string.Format("//div[@id='block{0}']/table/tbody/tr[2]/td[7]/a/span", rosterGroupId)).Click();
            
            // Click on gear for assignment and select Edit Assignment
            System.Threading.Thread.Sleep(5000);
            decimal itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Ministry_Assignments_ViewAll, individualName, "Name", null);
            test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[8]/div/a", TableIds.Ministry_Assignments_ViewAll, itemRow)).Click();
            System.Threading.Thread.Sleep(1000);
            test.Driver.FindElementByLinkText("Edit assignment").Click();

            // Click Cancel
            test.Driver.FindElementByLinkText("Cancel").Click();
            test.GeneralMethods.WaitForElement(By.LinkText("Add assignment"));

            // Click on the back arrow
            test.Driver.FindElementById(GeneralMinistry.Assignments.Tab_Back_Arrow).Click();
            test.GeneralMethods.WaitForElement(By.Id(GeneralMinistry.Activities.Add_Activity));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }
        #endregion Swooshy Back Arrow

        #region Activity Requirement

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("FO-2919 Verifies that the selected ministry value is kept when create an activity requirement")]
        public void Portal_Assignments_ViewAll_CreateRequirement_MinistryDisplay()
        {
            string Test_Ministry = "A Test Ministry";
            string Test_Activity = "A Test Activity";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to the Activities/View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);
            test.GeneralMethods.WaitForElement(test.Driver, By.Id(GeneralMinistry.Assignments.More_Button));

            SelectElement ministryDropdown = new SelectElement(test.Driver.FindElementById(string.Format("{0}-1", GeneralMinistry.Assignments.Ministry_DropDown)));
            if (Test_Ministry.Equals(ministryDropdown.Options.ElementAt(1).Text.ToString()))
            {
                Assert.Fail("We should create some ministry display above " + Test_Ministry);
            }

            //Select Ministry
            test.Portal.Ministry_Assignments_View_All_Filters_Ministry(Test_Ministry, false);
            
            //Select Activity Filter
            test.Portal.Ministry_Assignments_View_All_Filters_Activity(new string[] { Test_Activity });
            
            // Wait for and Click to open Add activity requirement
            test.GeneralMethods.WaitForElementVisible(By.Id("hideWhenNoRequirementsLink"));
            test.Driver.FindElementById("hideWhenNoRequirementsLink").Click();
            // Wait for correct page is opened
            test.GeneralMethods.WaitForElementVisible(By.Id("active_ministry_name"));
            Assert.IsTrue(Test_Ministry.Equals(test.Driver.FindElementById("active_ministry_name").Text.ToString()), "Ministry value in add activity requirement page is wrong");
            
            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure, Timeout(10000)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("FO-2919 Verifies that create an activity requirement can be saved successfully")]
        public void Portal_Assignments_ViewAll_CreateRequirement_Success()
        {
            string Test_Ministry = "A Test Ministry";
            string Test_Activity = "A Test Activity";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to the Activities/View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);
            test.GeneralMethods.WaitForElement(test.Driver, By.Id(GeneralMinistry.Assignments.More_Button));

            //Select Ministry
            test.Portal.Ministry_Assignments_View_All_Filters_Ministry(Test_Ministry, false);

            //Select Activity Filter
            test.Portal.Ministry_Assignments_View_All_Filters_Activity(new string[] { Test_Activity });

            // Wait for and Click to open Add activity requirement
            test.GeneralMethods.WaitForElementVisible(By.Id("hideWhenNoRequirementsLink"));
            test.Driver.FindElementById("hideWhenNoRequirementsLink").Click();
            // Wait for correct page is opened
            test.GeneralMethods.WaitForElementVisible(By.Id("active_ministry_name"));
            test.GeneralMethods.WaitForElementEnabled(By.Id("ctl00_ctl00_MainContent_content_btnGo"));

            test.GeneralMethods.WaitForPageIsLoaded(60);
            var options = test.Driver.FindElementById("ctl00_ctl00_MainContent_content_lbParticipants").FindElements(By.TagName("option"));
            if (options.Count == 0)
            {
                //Select schedule
                test.GeneralMethods.waitForElementIsEnabled("document.getElementById('ctl00_ctl00_MainContent_content_ddlSchedule_dropDownList')");
                new SelectElement(test.GeneralMethods.WaitAndGetElement(By.Id(GeneralMinistry.PostAttendance.Schedule_DropDown))).SelectByIndex(0);
                test.Driver.FindElementById("btnSearch").Click();
                while (options.Count == 0)
                {
                    System.Threading.Thread.Sleep(500);
                    options = test.Driver.FindElementById("ctl00_ctl00_MainContent_content_lbParticipants").FindElements(By.TagName("option"));
                }
            }

            // Click on Select button
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnGo").Click();
            test.GeneralMethods.WaitForElementVisible(By.Id("ctl00_ctl00_MainContent_content_Fieldset11"));

            test.GeneralMethods.WaitForElementVisible(By.Id("ctl00_ctl00_MainContent_content_dgActivityRequirements_ctl02_cbkSaveRequirement"));
            IWebElement rquirementCheckbox = test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgActivityRequirements_ctl02_cbkSaveRequirement");

            string comment_is_disabled = ((IJavaScriptExecutor)test.Driver).ExecuteScript("return document.getElementById(\"ctl00_ctl00_MainContent_content_dgActivityRequirements_ctl02_txtComment\").disabled;").ToString();

            if (comment_is_disabled.ToLower().Equals("true"))
            {
                // Activate one requirement if it's unchecked
                rquirementCheckbox.Click();
            }
            IWebElement comment = test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgActivityRequirements_ctl02_txtComment");
            comment.Clear();
            comment.SendKeys("Comment test");
            IWebElement date = test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgActivityRequirements_ctl02_dtbRequirementDate");
            date.Clear();
            date.SendKeys("6/1/2015");

            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSave").Click();

            test.GeneralMethods.WaitForElementVisible(By.Id("ctl00_ctl00_MainContent_content_dgActivityRequirements_ctl02_lblRequirementDate"));

            string color = test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgActivityRequirements_ctl02_lblRequirementDate").GetCssValue("color");

            Assert.IsFalse(color.Equals("rgba(0,0,0,1)"), "Error should not show up on page");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("FO-2919 Verifies that create an activity requirement cann't be saved when missing date")]
        public void Portal_Assignments_ViewAll_CreateRequirement_MissingDateError()
        {
            string Test_Ministry = "A Test Ministry";
            string Test_Activity = "A Test Activity";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to the Activities/View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);
            test.GeneralMethods.WaitForElement(test.Driver, By.Id(GeneralMinistry.Assignments.More_Button));

            //Select Ministry
            test.Portal.Ministry_Assignments_View_All_Filters_Ministry(Test_Ministry, false);

            //Select Activity Filter
            test.Portal.Ministry_Assignments_View_All_Filters_Activity(new string[] { Test_Activity });

            // Wait for and Click to open Add activity requirement
            test.GeneralMethods.WaitForElementVisible(By.Id("hideWhenNoRequirementsLink"));
            test.Driver.FindElementById("hideWhenNoRequirementsLink").Click();
            // Wait for correct page is opened
            test.GeneralMethods.WaitForElementVisible(By.Id("active_ministry_name"));

            test.GeneralMethods.WaitForPageIsLoaded(60);
            var options = test.Driver.FindElementById("ctl00_ctl00_MainContent_content_lbParticipants").FindElements(By.TagName("option"));
            if (options.Count == 0)
            {
                //Select schedule
                test.GeneralMethods.waitForElementIsEnabled("document.getElementById('ctl00_ctl00_MainContent_content_ddlSchedule_dropDownList')");
                new SelectElement(test.GeneralMethods.WaitAndGetElement(By.Id(GeneralMinistry.PostAttendance.Schedule_DropDown))).SelectByIndex(0);
                test.Driver.FindElementById("btnSearch").Click();
                while (options.Count == 0)
                {
                    System.Threading.Thread.Sleep(500);
                    options = test.Driver.FindElementById("ctl00_ctl00_MainContent_content_lbParticipants").FindElements(By.TagName("option"));
                }
            }

            // Click on Select button
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnGo").Click();
            test.GeneralMethods.WaitForElementVisible(By.Id("ctl00_ctl00_MainContent_content_Fieldset11"));

            test.GeneralMethods.WaitForElementVisible(By.Id("ctl00_ctl00_MainContent_content_dgActivityRequirements_ctl02_cbkSaveRequirement"));
            IWebElement rquirementCheckbox = test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgActivityRequirements_ctl02_cbkSaveRequirement");

            string comment_is_disabled = ((IJavaScriptExecutor)test.Driver).ExecuteScript("return document.getElementById(\"ctl00_ctl00_MainContent_content_dgActivityRequirements_ctl02_txtComment\").disabled;").ToString();

            if (comment_is_disabled.ToLower().Equals("true"))
            {
                // Activate one requirement if it's unchecked
                rquirementCheckbox.Click();
            }
            IWebElement comment = test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgActivityRequirements_ctl02_txtComment");
            comment.Clear();
            comment.SendKeys("Comment test");
            IWebElement date = test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgActivityRequirements_ctl02_dtbRequirementDate");
            date.Clear();

            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSave").Click();
            test.GeneralMethods.WaitForElementDisplayed(By.Id("ctl00_ctl00_MainContent_content_dgActivityRequirements_ctl02_lblRequirementDate"));

            string color = test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgActivityRequirements_ctl02_lblRequirementDate").GetCssValue("color");
            Assert.IsFalse(color.Equals("rgba(255,0,0,1)"), "Error isn't show up on page");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("FO-2919 Verifies that create an activity requirement page can be opened twice")]
        public void Portal_Assignments_ViewAll_CreateRequirement_OpenSecondTime()
        {
            string Test_Ministry = "A Test Ministry";
            string Test_Activity = "A Test Activity";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to the Activities/View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);
            test.GeneralMethods.WaitForElement(test.Driver, By.Id(GeneralMinistry.Assignments.More_Button));

            //Select Ministry
            test.Portal.Ministry_Assignments_View_All_Filters_Ministry(Test_Ministry, false);

            //Select Activity Filter
            test.Portal.Ministry_Assignments_View_All_Filters_Activity(new string[] { Test_Activity });

            // Wait for and Click to open Add activity requirement
            test.GeneralMethods.WaitForElementVisible(By.Id("hideWhenNoRequirementsLink"));
            test.Driver.FindElementById("hideWhenNoRequirementsLink").Click();
            // Wait for correct page is opened
            test.GeneralMethods.WaitForElementVisible(By.Id("active_ministry_name"));

            test.GeneralMethods.WaitForPageIsLoaded(60);
            var options = test.Driver.FindElementById("ctl00_ctl00_MainContent_content_lbParticipants").FindElements(By.TagName("option"));
            if (options.Count == 0)
            {
                //Select schedule
                test.GeneralMethods.waitForElementIsEnabled("document.getElementById('ctl00_ctl00_MainContent_content_ddlSchedule_dropDownList')");
                new SelectElement(test.GeneralMethods.WaitAndGetElement(By.Id(GeneralMinistry.PostAttendance.Schedule_DropDown))).SelectByIndex(0);
                test.Driver.FindElementById("btnSearch").Click();
                while (options.Count == 0)
                {
                    System.Threading.Thread.Sleep(500);
                    options = test.Driver.FindElementById("ctl00_ctl00_MainContent_content_lbParticipants").FindElements(By.TagName("option"));
                }
            }

            // Click on Select button
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnGo").Click();
            test.GeneralMethods.WaitForElementVisible(By.Id("ctl00_ctl00_MainContent_content_Fieldset11"));
            
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSave").Click();

            test.GeneralMethods.WaitForElementVisible(By.Id("ctl00_ctl00_MainContent_content_btnCancel"));
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnCancel").Click();

            test.GeneralMethods.WaitForElementVisible(By.Id("hideWhenNoRequirementsLink"));
            test.Driver.FindElementById("hideWhenNoRequirementsLink").Click();
            // Wait for correct page is opened
            test.GeneralMethods.WaitForElementVisible(By.Id("active_ministry_name"));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("FO-2919 Verifies that create an activity requirement page can be returned by clicking on return")]
        public void Portal_Assignments_ViewAll_CreateRequirement_ReturnBack()
        {
            string Test_Ministry = "A Test Ministry";
            string Test_Activity = "A Test Activity";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Navigate to the Activities/View All page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.View_All);
            test.GeneralMethods.WaitForElement(test.Driver, By.Id(GeneralMinistry.Assignments.More_Button));

            //Select Ministry
            test.Portal.Ministry_Assignments_View_All_Filters_Ministry(Test_Ministry, false);

            //Select Activity Filter
            test.Portal.Ministry_Assignments_View_All_Filters_Activity(new string[] { Test_Activity });

            // Wait for and Click to open Add activity requirement
            test.GeneralMethods.WaitForElementVisible(By.Id("hideWhenNoRequirementsLink"));
            test.Driver.FindElementById("hideWhenNoRequirementsLink").Click();
            // Wait for correct page is opened
            test.GeneralMethods.WaitForElementVisible(By.Id("active_ministry_name"));


            test.Driver.FindElementById("tab_back").Click();

            test.GeneralMethods.WaitForElementVisible(By.Id("hideWhenNoRequirementsLink"));
            test.Driver.FindElementById("hideWhenNoRequirementsLink").Click();
            // Wait for correct page is opened
            test.GeneralMethods.WaitForElementVisible(By.Id("active_ministry_name"));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        #endregion Activity Requirement

        #endregion Assignments
    }
}
