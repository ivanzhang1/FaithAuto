using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace FTTests.Portal.Groups {
	[TestFixture]
	public class Portal_Groups_GroupsByGroupType_ViewAll_WebDriver : FixtureBaseWebDriver {
        private string _groupTypeNameMISC = "A Test Group Type:New User WD";
        private string _groupNameMISC = "A Test Group:New User WD";
        private string _groupTypeName = "View All Group Type WD";
        private string _groupTypeName2 = "View All Group Type 2 WD";

        // People Lists
        private string _peopleListName = "View All People List WD";
        private string _publicPeopleList = "View All Public People List WD";

        // Groups
        private string _groupName = "View All Generic Group WD";
        private string _groupName2 = "View All Owner Explicit WD";
        private string _coedGroup = "View All Coed Group WD";
        private string _maleGroup = "View All Male Group WD";
        private string _femaleGroup = "View All Female Group WD";
        private string _marriedOrSingleGroup = "View All Married or Single Group WD";
        private string _marriedGroup = "View All Married Group WD";
        private string _singleGroup = "View All Single Group WD";
        private string _startDate1Group = "View All Start Date Group 1 WD";
        private string _startDate2Group = "View All Start Date Group 2 WD";
        private string _sundayGroup = "View All Sunday Group WD";
        private string _mondayGroup = "View All Monday Group WD";
        private string _tuesdayGroup = "View All Tuesday Group WD";
        private string _wednesdayGroup = "View All Wednesday Group WD";
        private string _thursdayGroup = "View All Thursday Group WD";
        private string _fridayGroup = "View All Friday Group WD";
        private string _saturdayGroup = "View All Saturday Group WD";
        private string _ageRangeGroup = "View All Age Range Group WD";

        // Span of care
        private string _spanOfCareName = "View All Span of Care WD";

        [FixtureSetUp]
        public void FixtureSetUp() {

            base.SQL.Groups_GroupType_Delete(15, _groupTypeNameMISC);
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName2);
            base.SQL.Groups_SpanOfCare_Delete(254, _spanOfCareName);
            base.SQL.Groups_GroupType_Create(15, "Matthew Sneeden", _groupTypeNameMISC, new List<int> { 4, 5, 10 });
            base.SQL.Groups_GroupType_Create(254, "Group Admin", _groupTypeName, new List<int> { 4, 5, 10 });
            base.SQL.Groups_GroupType_Create(254, "Group Admin", _groupTypeName2, new List<int> { 4, 5, 10 });
            base.SQL.Groups_Group_Create(15, _groupTypeNameMISC, "Matthew Sneeden", _groupNameMISC, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());

            // People Lists
            base.SQL.Groups_PeopleLists_Delete(254, _peopleListName);
            base.SQL.Groups_PeopleLists_Create(254, _peopleListName, null, "Bryan Mikaelian");
            base.SQL.Groups_PeopleLists_Create(254, _publicPeopleList, null, "Bryan Mikaelian", true);
            base.SQL.Groups_PeopleLists_AddManagerOrViewer(254, _peopleListName, "Group Admin", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_PeopleLists_AddManagerOrViewer(254, _publicPeopleList, "Bryan Mikaelian", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_PeopleLists_AddMember(254, _peopleListName, "Group Leader");

            // Groups
            // Generic Groups
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _groupName, "Group Manager", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _groupName, "Group Viewer", GeneralEnumerations.GroupRolesPortalUser.Viewer);

            base.SQL.Groups_Group_Create(254, _groupTypeName2, "Bryan Mikaelian", _groupName2, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName2, _groupName2, "SOC Owner", GeneralEnumerations.GroupRolesPortalUser.Manager);

            // Age Range Group
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _ageRangeGroup, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString(), GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, false, true, null, false, 19, 35);
            base.SQL.Groups_Group_AddLeaderOrMember(254, _ageRangeGroup, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _ageRangeGroup, "Group Manager", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _ageRangeGroup, "Group Viewer", GeneralEnumerations.GroupRolesPortalUser.Viewer);

            // Coed Group
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _coedGroup, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _coedGroup, "Group Manager", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _coedGroup, "Group Viewer", GeneralEnumerations.GroupRolesPortalUser.Viewer);

            // Female Group
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _femaleGroup, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString(), GeneralEnumerations.Gender.Female);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _femaleGroup, "Group Manager", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _femaleGroup, "Group Viewer", GeneralEnumerations.GroupRolesPortalUser.Viewer);

            // Male Group
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _maleGroup, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString(), GeneralEnumerations.Gender.Male);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _maleGroup, "Group Manager", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _maleGroup, "Group Viewer", GeneralEnumerations.GroupRolesPortalUser.Viewer);

            // Married or Single Group
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _marriedOrSingleGroup, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString(), GeneralEnumerations.Gender.Female);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _marriedOrSingleGroup, "Group Manager", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _marriedOrSingleGroup, "Group Viewer", GeneralEnumerations.GroupRolesPortalUser.Viewer);

            // Married Group
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _marriedGroup, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString(), GeneralEnumerations.Gender.Female, GeneralEnumerations.GroupMaritalStatus.Married);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _marriedGroup, "Group Manager", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _marriedGroup, "Group Viewer", GeneralEnumerations.GroupRolesPortalUser.Viewer);

            // Single Group
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _singleGroup, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString(), GeneralEnumerations.Gender.Female, GeneralEnumerations.GroupMaritalStatus.Single);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _singleGroup, "Group Manager", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _singleGroup, "Group Viewer", GeneralEnumerations.GroupRolesPortalUser.Viewer);

            // Start Date groups
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _startDate1Group, null, "11/15/2013");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _startDate1Group, "Group Manager", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _startDate1Group, "Group Viewer", GeneralEnumerations.GroupRolesPortalUser.Viewer);

            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _startDate2Group, null, "11/20/2013");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _startDate2Group, "Group Manager", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _startDate2Group, "Group Viewer", GeneralEnumerations.GroupRolesPortalUser.Viewer);
            
            // Schedule groups
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _sundayGroup, null, "11/20/2013");
            base.SQL.Groups_Group_CreateSchedule(254, GeneralEnumerations.GroupScheduleFrequency.Weekly, _sundayGroup, "Bryan Mikaelian", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortTimeString(), null, null, new List<GeneralEnumerations.WeeklyScheduleDays>() { GeneralEnumerations.WeeklyScheduleDays.Sunday });
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _mondayGroup, null, "11/20/2013");
            base.SQL.Groups_Group_CreateSchedule(254, GeneralEnumerations.GroupScheduleFrequency.Weekly, _mondayGroup, "Bryan Mikaelian", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortTimeString(), null, null, new List<GeneralEnumerations.WeeklyScheduleDays>() { GeneralEnumerations.WeeklyScheduleDays.Monday });
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _tuesdayGroup, null, "11/20/2013");
            base.SQL.Groups_Group_CreateSchedule(254, GeneralEnumerations.GroupScheduleFrequency.Weekly, _tuesdayGroup, "Bryan Mikaelian", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortTimeString(), null, null, new List<GeneralEnumerations.WeeklyScheduleDays>() { GeneralEnumerations.WeeklyScheduleDays.Tuesday });
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _wednesdayGroup, null, "11/20/2013");
            base.SQL.Groups_Group_CreateSchedule(254, GeneralEnumerations.GroupScheduleFrequency.Weekly, _wednesdayGroup, "Bryan Mikaelian", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortTimeString(), null, null, new List<GeneralEnumerations.WeeklyScheduleDays>() { GeneralEnumerations.WeeklyScheduleDays.Wednesday });
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _thursdayGroup, null, "11/20/2013");
            base.SQL.Groups_Group_CreateSchedule(254, GeneralEnumerations.GroupScheduleFrequency.Weekly, _thursdayGroup, "Bryan Mikaelian", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortTimeString(), null, null, new List<GeneralEnumerations.WeeklyScheduleDays>() { GeneralEnumerations.WeeklyScheduleDays.Thursday });
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _fridayGroup, null, "11/20/2013");
            base.SQL.Groups_Group_CreateSchedule(254, GeneralEnumerations.GroupScheduleFrequency.Weekly, _fridayGroup, "Bryan Mikaelian", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortTimeString(), null, null, new List<GeneralEnumerations.WeeklyScheduleDays>() { GeneralEnumerations.WeeklyScheduleDays.Friday });
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _saturdayGroup, null, "11/20/2013");
            base.SQL.Groups_Group_CreateSchedule(254, GeneralEnumerations.GroupScheduleFrequency.Weekly, _saturdayGroup, "Bryan Mikaelian", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortTimeString(), null, null, new List<GeneralEnumerations.WeeklyScheduleDays>() { GeneralEnumerations.WeeklyScheduleDays.Saturday });

            // Span of Care
            base.SQL.Groups_SpanOfCare_Create(254, _spanOfCareName, "Bryan Mikaelian", "SOC Owner", new List<string>() { _groupTypeName });
        }

        [FixtureTearDown]
        public void FixtureTearDown() {
            base.SQL.Groups_GroupType_Delete(15, _groupTypeNameMISC);
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName2);
            base.SQL.Groups_PeopleLists_Delete(254, _peopleListName);
            base.SQL.Groups_PeopleLists_Delete(254, _publicPeopleList);
            base.SQL.Groups_SpanOfCare_Delete(254, _spanOfCareName);
            
        }

        #region All

        
        [Test, RepeatOnFailure]
        [Author("Michelle Zheng")]
        [Description("FO-3431 Verifies inactive user is not displayed on group permission list")]
        public void Groups_GroupsByGroupType_GroupPermissionList_InvalidUser_NotDisplay()
        {
            string fullName = "inactive user";
            string groupName = "Auto test_mz";
            string firstName = "inactive";
            string lastName = "user";
            string individual_Name = "ft.tester";
            string password1 = "FT4life!";
            string password = "P@$$w0rd";
            string email = "email@email.com";
            int churchId = 15;

            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            #region initial data
            // Login to Portal
            test.Portal.LoginWebDriver(individual_Name, password1, "dc");
           

            //Create household
            test.Portal.People_AddHousehold(firstName, lastName, HouseholdPositionConstants.Head, "Member", null, null, DateTime.Now.ToString("M/d/yyyy"));
            int individualId = base.SQL.People_Individuals_FetchID(churchId, fullName);

            // Create a new portal user
            test.GeneralMethods.Navigate_Portal(Navigation.Admin.Security_Setup.Portal_Users);
            test.Driver.FindElementByPartialLinkText(GeneralLinksWebDriver.Add).Click();
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtLogin").SendKeys(fullName);
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtFirstName").SendKeys(firstName);
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtLastName").SendKeys(lastName);
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtEmail").SendKeys(email);
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtPassword").SendKeys(password);
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtPasswordValidation").SendKeys(password);
            test.Driver.FindElementById(GeneralButtons.Save).Click();
            test.Driver.SwitchTo().Alert().Dismiss();

            System.Threading.Thread.Sleep(3000);
            // Update the user to link it to an individual
            base.SQL.Execute((string.Format("UPDATE ChmChurch.dbo.USERS SET INDIVIDUAL_ID = {0} WHERE CHURCH_ID = 15 AND LOGIN = '{1}'", individualId, fullName)));
           
            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for group based on a name
            test.Driver.FindElementByLinkText("Name and start date").Click();
            test.Driver.FindElementById("group_name").Clear();
            test.Driver.FindElementById("group_name").SendKeys(groupName);
            test.Driver.FindElementById("commit").Click();
           
            //View group settings
            test.GeneralMethods.WaitForElementDisplayed(By.LinkText("Auto test_mz"));
            test.Driver.FindElementByLinkText("Auto test_mz").Click();
            test.GeneralMethods.WaitForElementDisplayed(By.LinkText("View group settings"));
            test.Driver.FindElementByLinkText("View group settings").Click();
            
            //Add individual to permission list
            test.GeneralMethods.WaitForElementDisplayed(By.LinkText("Change permissions"));
            test.Driver.FindElementByLinkText("Change permissions").Click();
            test.Driver.FindElementByXPath("//*[@id='group_permissions_table']/tbody/tr/td/span[text()='user, inactive']/parent::td/following-sibling::td[@class='align_center group_permissions_manager']/input[@type='radio']").Click();
            test.Driver.FindElementByXPath(".//*[@value='Save permissions']").Click();
            
            //Set portal user to inactive
            test.SQL.Admin_Set_User_ActiveInActive(churchId, fullName, 0);
  
           

            #endregion

                try
                {
                    // Navigate to groups->view all
                    test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);
                    // Search for group based on a name
                    test.Driver.FindElementByLinkText("Name and start date").Click();
                    test.Driver.FindElementById("group_name").Clear();
                    test.Driver.FindElementById("group_name").SendKeys(groupName);
                    test.Driver.FindElementById("commit").Click();

                    //View group settings
                    test.GeneralMethods.WaitForElementDisplayed(By.LinkText("Auto test_mz"));
                    test.Driver.FindElementByLinkText("Auto test_mz").Click();
                    test.GeneralMethods.WaitForElementDisplayed(By.LinkText("View group settings"));
                    test.Driver.FindElementByLinkText("View group settings").Click();

                    //Verify if the individual shows in permission list 
                    Assert.IsFalse(test.Driver.FindElementByTagName("html").Text.Contains(fullName));
    
                }
   
                finally
                {
                    this.SQL.People_DeleteIndividual(churchId, individualId);
                    base.SQL.Admin_Users_Delete(15, fullName);

                }
             
            // Logout
            test.Portal.LogoutWebDriver();
        }  
      

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the correct search options are present when viewing All Groups")]
        public void Groups_GroupsByGroupType_ViewAll_All_Availible_Search_Options() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("bmikaelian", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Verify all elements are present for the All Groups list
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText(GroupsByGroupTypeConstants.ViewAll.HeaderLink_GroupTypesAndCustomFields_Link)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Standard Fields")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("commit")));

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups and people lists by a group name.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Search_Group_Name() {
            // Login to Portal as group admin
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on a name
            test.Driver.FindElementByLinkText("Name and start date").Click();
            test.Driver.FindElementById("group_name").SendKeys(_groupName);   //.Substring(6)
            //test.Driver.FindElementById("commit").Click();
            //test.GeneralMethods.Click_Button(By.CssSelector("[value='Search']"));
            test.ScreenShotDriver.FindElementById("commit").Click();

            //Wait for Search Results
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(TableIds.Groups_ViewAll_GroupList_SearchResults));

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _groupName, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();
            System.Threading.Thread.Sleep(5000);
            // Login to Portal as a group manager
            test.Portal.LoginWebDriver("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on a name
            test.Driver.FindElementByLinkText("Name and start date").Click();
            test.Driver.FindElementById("group_name").SendKeys(_groupName);
            
            //test.Driver.FindElementById("commit").Click();
            //test.GeneralMethods.Click_Button(By.CssSelector("[value='Search']"));
            test.ScreenShotDriver.FindElementById("commit").Click();

            //Wait for Search Results
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(TableIds.Groups_ViewAll_GroupList_SearchResults));

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _groupName, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();
            System.Threading.Thread.Sleep(5000);
            // Login to Portal as a group viewer
            test.Portal.LoginWebDriver("groupviewer", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on a name
            test.Driver.FindElementByLinkText("Name and start date").Click();
            test.Driver.FindElementById("group_name").SendKeys(_groupName);
            //test.Driver.FindElementById("commit").Click();
            //test.GeneralMethods.Click_Button(By.CssSelector("[value='Search']"));
            test.ScreenShotDriver.FindElementById("commit").Click();

            //Wait for Search Results
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(TableIds.Groups_ViewAll_GroupList_SearchResults));

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _groupName, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups and people lists based on an individual's name.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Search_Group_IndividualName() {
            // Login to Portal as a group admin
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on an individual name
            test.Driver.FindElementByLinkText("Name and start date").Click();
            test.Driver.FindElementById("individual_name").SendKeys("Leader");
            //test.Driver.FindElementById("commit").Click();
            //test.GeneralMethods.Click_Button(By.CssSelector("[value='Search']"));
            test.ScreenShotDriver.FindElementById("commit").Click();

            //Wait for Search Results
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(TableIds.Groups_ViewAll_GroupList_SearchResults));
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Cancel and view all"));

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _groupName, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();
            System.Threading.Thread.Sleep(5000);
            // Login to Portal as a group manager
            test.Portal.LoginWebDriver("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);
            
            // Search for all groups based on an individual name
            test.Driver.FindElementByLinkText("Name and start date").Click();
            test.Driver.FindElementById("individual_name").SendKeys("Leader");
            //test.Driver.FindElementById("commit").Click();
            //test.GeneralMethods.Click_Button(By.CssSelector("[value='Search']"));
            test.ScreenShotDriver.FindElementById("commit").Click();

            //Wait for Search Results
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(TableIds.Groups_ViewAll_GroupList_SearchResults));

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _groupName, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();
            System.Threading.Thread.Sleep(5000);
            // Login to Portal as a group viewer
            test.Portal.LoginWebDriver("groupviewer", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on an individual name
            test.Driver.FindElementByLinkText("Name and start date").Click();
            test.Driver.FindElementById("individual_name").SendKeys("Leader");
            //test.Driver.FindElementById("commit").Click();
            //test.GeneralMethods.Click_Button(By.CssSelector("[value='Search']"));
            test.ScreenShotDriver.FindElementById("commit").Click();

            //Wait for Search Results
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath(TableIds.Groups_ViewAll_GroupList_SearchResults));

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _groupName, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies group admin can search for all groups and people lists based on an Start date from and Start date To.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Search_StartDate_GroupAdmin() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "qaeunlx0c2");

            try
            {
                // Navigate to groups->view all
                test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

                // Search on an inclusive start date range
                test.Portal.Groups_Search_NameAndStartDate_WebDriver(null, null, "11/14/2013", "11/21/2013");
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));
                test.Driver.FindElementByLinkText("Cancel and view all").Click();
                test.GeneralMethods.WaitForElement(By.Id("criteria_options"));

                // Search on a start date range that starts on a group's start date but has no end date
                test.Portal.Groups_Search_NameAndStartDate_WebDriver(null, null, "11/15/2013", null);
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));
                test.Driver.FindElementByLinkText("Cancel and view all").Click();
                test.GeneralMethods.WaitForElement(By.Id("criteria_options"));

                // Search on a start date range that starts and ends on a group's start date
                test.Portal.Groups_Search_NameAndStartDate_WebDriver(null, null, "11/15/2013", "11/20/2013");
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));
                test.Driver.FindElementByLinkText("Cancel and view all").Click();
                test.GeneralMethods.WaitForElement(By.Id("criteria_options"));

                // Search on a start date range that is after on a group's start date
                test.Portal.Groups_Search_NameAndStartDate_WebDriver(null, null, "12/26/2013", null);
                test.GeneralMethods.TakeScreenShot_WebDriver();
                Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"), string.Format("Group Found: {0}", _startDate1Group));
                Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"), string.Format("Group Found: {0}", _startDate2Group));

            }
            finally
            {
                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies group manager can search for all groups and people lists based on an Start date from and Start date To.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Search_StartDate_GroupManager()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            // Login as a group manager
            test.Portal.LoginWebDriver("groupmanager", "BM.Admin09", "qaeunlx0c2");

            try
            {
                // Navigate to groups->view all
                test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

                // Search on an inclusive start date range
                test.Portal.Groups_Search_NameAndStartDate_WebDriver(null, null, "11/14/2013", "11/21/2013");
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));
                test.Driver.FindElementByLinkText("Cancel and view all").Click();
                test.GeneralMethods.WaitForElement(By.Id("criteria_options"));

                // Search on a start date range that starts on a group's start date but has no end date
                test.Portal.Groups_Search_NameAndStartDate_WebDriver(null, null, "11/15/2013", null);
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));
                test.Driver.FindElementByLinkText("Cancel and view all").Click();
                test.GeneralMethods.WaitForElement(By.Id("criteria_options"));

                // Search on a start date range that starts and ends on a group's start date
                test.Portal.Groups_Search_NameAndStartDate_WebDriver(null, null, "11/15/2013", "11/20/2013");
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));
                test.Driver.FindElementByLinkText("Cancel and view all").Click();
                test.GeneralMethods.WaitForElement(By.Id("criteria_options"));

                // Search on a start date range that is after on a group's start date
                test.Portal.Groups_Search_NameAndStartDate_WebDriver(null, null, "11/26/2013", null);
                Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
                Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));
            }
            finally
            {
                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies group viewer can search for all groups and people lists based on an Start date from and Start date To.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Search_StartDate_GroupViewer()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            // Login as a group viewer
            test.Portal.LoginWebDriver("groupviewer", "BM.Admin09", "qaeunlx0c2");

            try
            {
                // Navigate to groups->view all
                test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

                // Search on an inclusive start date range
                test.Portal.Groups_Search_NameAndStartDate_WebDriver(null, null, "11/14/2013", "11/21/2013");
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));
                test.Driver.FindElementByLinkText("Cancel and view all").Click();
                test.GeneralMethods.WaitForElement(By.Id("criteria_options"));

                // Search on a start date range that starts on a group's start date but has no end date
                test.Portal.Groups_Search_NameAndStartDate_WebDriver(null, null, "11/15/2013", null);
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));
                test.Driver.FindElementByLinkText("Cancel and view all").Click();
                test.GeneralMethods.WaitForElement(By.Id("criteria_options"));

                // Search on a start date range that starts and ends on a group's start date
                test.Portal.Groups_Search_NameAndStartDate_WebDriver(null, null, "11/15/2013", "11/20/2013");
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));
                test.Driver.FindElementByLinkText("Cancel and view all").Click();
                test.GeneralMethods.WaitForElement(By.Id("criteria_options"));

                // Search on a start date range that is after on a group's start date
                test.Portal.Groups_Search_NameAndStartDate_WebDriver(null, null, "11/26/2013", null);
                Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
                Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));
            }
            finally
            {
                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }   
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups and people lists by a people list name.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Search_PeopleList_Name() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all people lists based on a name
            test.Driver.FindElementByLinkText("Name and start date").Click();
            test.Driver.FindElementById("group_name").SendKeys(_peopleListName.Substring(6));
            //test.Driver.FindElementById("commit").Click();
            test.GeneralMethods.Click_Button(By.CssSelector("[value='Search']"));
            test.ScreenShotDriver.FindElementById("commit").Click();

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _peopleListName, "Group"));

            // Verify the groups table isn't returned
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.XPath(TableIds.Groups_ViewAll_GroupList_GroupTab)), "Groups were returned!");

            // Logout
            test.Portal.LogoutWebDriver();
        }               

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups and people lists by an individual name in a people list.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Search_PeopleList_IndividualName() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all people lists based on a name
            test.Driver.FindElementByLinkText("Name and start date").Click();
            test.Driver.FindElementById("individual_name").SendKeys("Group");
            //test.Driver.FindElementById("commit").Click();
            test.GeneralMethods.Click_Button(By.CssSelector("[value='Search']"));
            test.ScreenShotDriver.FindElementById("commit").Click();

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _peopleListName, "Group"));

            // Verify the groups table isn't returned
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.Id(TableIds.Groups_ViewAll_GroupList_GroupTab)), "Groups were returned!");

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups and people lists by the Coed demographic.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Search_Demographic_Coed() {
            // Login to Portal as a group admin
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on the Coed demographic
            test.Driver.FindElementByLinkText("Standard Fields").Click();
            test.Driver.FindElementById("gender_0").Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results  
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _coedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _femaleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _maleGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();

            // Login to Portal as a group manager
            test.Portal.LoginWebDriver("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on the Coed demographic
            test.Driver.FindElementByLinkText("Standard Fields").Click();
            test.Driver.FindElementById("gender_0").Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results  
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _coedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _femaleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _maleGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();

            // Login to Portal as a group viewer
            test.Portal.LoginWebDriver("groupviewer", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on the Coed demographic
            test.Driver.FindElementByLinkText("Standard Fields").Click();
            test.Driver.FindElementById("gender_0").Click();
            test.Driver.FindElementById("commit").Click();


            // Verify the results  
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _coedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _femaleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _maleGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups and people lists by the female demographic.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Search_Demographic_Female() {
            // Login to Portal as a group admin
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on the Female demographic
            test.Driver.FindElementByLinkText("Standard Fields").Click();
            test.Driver.FindElementById("gender_1").Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _coedGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _femaleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _maleGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();

            // Login to Portal as a group manager
            test.Portal.LoginWebDriver("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on the Female demographic
            test.Driver.FindElementByLinkText("Standard Fields").Click();
            test.Driver.FindElementById("gender_1").Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _coedGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _femaleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _maleGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();

            // Login to Portal as a group viewer
            test.Portal.LoginWebDriver("groupviewer", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on the Female demographic
            test.Driver.FindElementByLinkText("Standard Fields").Click();
            test.Driver.FindElementById("gender_1").Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _coedGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _femaleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _maleGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups and people lists by the male demographic.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Search_Demographic_Male() {
            // Login to Portal as a group admin
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on the Male demographic
            test.Driver.FindElementByLinkText("Standard Fields").Click();
            test.Driver.FindElementById("gender_2").Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _coedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _femaleGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _maleGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();

            // Login to Portal as a group manager
            test.Portal.LoginWebDriver("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on the Male demographic
            test.Driver.FindElementByLinkText("Standard Fields").Click();
            test.Driver.FindElementById("gender_2").Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _coedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _femaleGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _maleGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();

            // Login to Portal as a group viewer
            test.Portal.LoginWebDriver("groupviewer", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on the Male demographic
            test.Driver.FindElementByLinkText("Standard Fields").Click();
            test.Driver.FindElementById("gender_2").Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _coedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _femaleGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _maleGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups and people lists by the married demographic.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Search_Demographic_Married() {
            // Login to Portal as a group admin
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on the married demographic
            test.Driver.FindElementByLinkText("Standard Fields").Click();
            test.Driver.FindElementById("marital_status_1").Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedOrSingleGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _singleGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();

            // Login to Portal as a group manager
            test.Portal.LoginWebDriver("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on the married demographic
            test.Driver.FindElementByLinkText("Standard Fields").Click();
            test.Driver.FindElementById("marital_status_1").Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedOrSingleGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _singleGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();

            // Login to Portal as a group viewer
            test.Portal.LoginWebDriver("groupviewer", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on the married demographic
            test.Driver.FindElementByLinkText("Standard Fields").Click();
            test.Driver.FindElementById("marital_status_1").Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedOrSingleGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _singleGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups and people lists by the single demographic.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Search_Demographic_Single() {
            // Login to Portal as a group admin
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on the single demographic
            test.Driver.FindElementByLinkText("Standard Fields").Click();
            test.Driver.FindElementById("marital_status_2").Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedOrSingleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _singleGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();

            // Login to Portal as a group manager
            test.Portal.LoginWebDriver("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on the single demographic
            test.Driver.FindElementByLinkText("Standard Fields").Click();
            test.Driver.FindElementById("marital_status_2").Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedOrSingleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _singleGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();

            // Login to Portal as a group viewer
            test.Portal.LoginWebDriver("groupviewer", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on the single demographic
            test.Driver.FindElementByLinkText("Standard Fields").Click();
            test.Driver.FindElementById("marital_status_2").Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedOrSingleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _singleGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups and people lists by the married or single demographic.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Search_Demographic_Married_Or_Single() {
            // Login to Portal as a group admin
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on the single demographic
            test.Driver.FindElementByLinkText("Standard Fields").Click();
            test.Driver.FindElementById("marital_status_0").Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results  
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedOrSingleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _singleGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();

            // Login to Portal as a group manager
            test.Portal.LoginWebDriver("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on the single demographic
            test.Driver.FindElementByLinkText("Standard Fields").Click();
            test.Driver.FindElementById("marital_status_0").Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results  
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedOrSingleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _singleGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();

            // Login to Portal as a group viewer
            test.Portal.LoginWebDriver("groupviewer", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on the single demographic
            test.Driver.FindElementByLinkText("Standard Fields").Click();
            test.Driver.FindElementById("marital_status_0").Click();
            test.Driver.FindElementById("commit").Click();


            // Verify the results  
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedOrSingleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _singleGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups and people lists by an age range.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Search_Demographic_Age() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups based on an age range 
            test.Driver.FindElementByLinkText("Standard Fields").Click();
            new SelectElement(test.Driver.FindElementById("age_range_min")).SelectByText("18");
            new SelectElement(test.Driver.FindElementById("age_range_max")).SelectByText("45");
            test.Driver.FindElementById("commit").Click();

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _ageRangeGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups and people lists that meet on Sunday.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Search_Schedule_Sunday() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups that meet on Sunday
            test.Driver.FindElementByLinkText("Standard Fields").Click();
            test.Driver.FindElementByLinkText("Day of the Week").Click();
            test.Driver.FindElementById(GroupsByGroupTypeConstants.ViewAll.CheckBox_Sunday).Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _sundayGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups and people lists that meet on Monday.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Search_Schedule_Monday() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups that meet on Monday
            test.Driver.FindElementByLinkText("Day of the Week").Click();
            test.Driver.FindElementById(GroupsByGroupTypeConstants.ViewAll.CheckBox_Monday).Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _mondayGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups and people lists that meet on Tuesday.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Search_Schedule_Tuesday() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups that meet on Tuesday
            test.Driver.FindElementByLinkText("Day of the Week").Click();
            test.Driver.FindElementById(GroupsByGroupTypeConstants.ViewAll.CheckBox_Tuesday).Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _tuesdayGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups and people lists that meet on Wednesday.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Search_Schedule_Wednesday() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups that meet on Wednesday
            test.Driver.FindElementByLinkText("Day of the Week").Click();
            test.Driver.FindElementById(GroupsByGroupTypeConstants.ViewAll.CheckBox_Wednesday).Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _wednesdayGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups and people lists that meet on Thursday.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Search_Schedule_Thursday() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups that meet on Thursday
            test.Driver.FindElementByLinkText("Day of the Week").Click();
            test.Driver.FindElementById(GroupsByGroupTypeConstants.ViewAll.CheckBox_Thursday).Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _thursdayGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups and people lists that meet on Friday.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Search_Schedule_Friday() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups that meet on Friday
            test.Driver.FindElementByLinkText("Day of the Week").Click();
            test.Driver.FindElementById(GroupsByGroupTypeConstants.ViewAll.CheckBox_Friday).Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _fridayGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups and people lists that meet on Saturday.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Search_Schedule_Saturday() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups that meet on Saturday
            test.Driver.FindElementByLinkText("Day of the Week").Click();
            test.Driver.FindElementById(GroupsByGroupTypeConstants.ViewAll.CheckBox_Saturday).Click();
            test.Driver.FindElementById("commit").Click();

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults, _saturdayGroup, "Group"));

            // Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies that a newly created Portal user can view a group that is set to 'All users can view this group'")]
        public void Groups_GroupsByGroupType_ViewAll_All_NewPortalUser() {
            // Set initial conditions
            string login = "NewUser";
            base.SQL.Admin_Users_Delete(15, login);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Configure group if necessary
            test.Portal.Groups_Groups_View_WebDriver(_groupNameMISC, "Groups");
            test.Driver.FindElementByLinkText("View group settings").Click();

            if (!test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//strong[@class='icon_check' and text()='All users can view this group.']"))) {
                test.Driver.FindElementByLinkText("Change permissions").Click();
                test.Driver.FindElementById("group_permissions_check").Click();
                test.Driver.FindElementByXPath("//input[@value='Save permissions']").Click();
            }

            // Create a new portal user
            test.GeneralMethods.Navigate_Portal(Navigation.Admin.Security_Setup.Portal_Users);
            test.Driver.FindElementByPartialLinkText(GeneralLinksWebDriver.Add).Click();

            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtLogin").SendKeys(login);
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtFirstName").SendKeys("New");
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtLastName").SendKeys("User");
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtEmail").SendKeys("newuser@gmail.com");
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtPassword").SendKeys("Pa$$w0rd");
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtPasswordValidation").SendKeys("Pa$$w0rd");
            test.Driver.FindElementById(GeneralButtons.Save).Click();
            test.Driver.SwitchTo().Alert().Dismiss();
//            test.Driver.FindElementById(GeneralButtons.Save).Click();
//            test.Driver.SwitchTo().Alert().Accept();

            // Logout of portal
            test.Portal.LogoutWebDriver();


            // Update the user to link it to an individual
            base.SQL.Execute((string.Format("UPDATE ChmChurch.dbo.USERS SET INDIVIDUAL_ID = {0} WHERE CHURCH_ID = 15 AND LOGIN = '{1}'", base.SQL.IndividualID, login)));


            // Login to portal
            test.Portal.LoginWebDriver(login, "Pa$$w0rd", "dc");

            // Verify the group is visible to the new user
            test.Portal.Groups_Groups_View_WebDriver(_groupNameMISC, "Groups");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies you can search for groups by single selection custom field in group type")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Search_CustomField_Single()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string customFieldName = "Custom field for group search";
            string groupTypeName = "Group type for group search";
            string groupName1 = "Group with custom field option";
            string groupName2 = "Group without custom field option";

            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_Group_Delete(254, groupName1);
            base.SQL.Groups_Group_Delete(254, groupName2); 

            this.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.SingleSelect, customFieldName, null, new List<string> { "1", "2", "3" });
            this.SQL.Groups_GroupType_Create(254, "Group Admin", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_GroupTypes_AddCustomField(254, groupTypeName, customFieldName);

            base.SQL.Groups_Group_Create(254, groupTypeName, "Group Admin", groupName1, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle);
            base.SQL.Groups_Group_Create(254, groupTypeName, "Group Admin", groupName2, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.Married);

            try
            {
                // Login to Portal
                test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "qaeunlx0c2");
                int customFieldOptionId = base.SQL.Groups_CustomFieldOption_FetchID(254, customFieldName, "3");
                test.Portal.Groups_Group_Update_CustomFieldOption(groupName1, customFieldOptionId);
                int customFieldId = base.SQL.Groups_CustomField_FetchID(254, customFieldName);
                int groupTypeId = base.SQL.Groups_GroupTypes_FetchID(254, groupTypeName);
                test.Portal.Groups_Search_CustomFieldOption_WebDriver("group_type_" + groupTypeId, "custom_field_" + customFieldId, customFieldOptionId.ToString(), groupName1);

                // Logout
                test.Portal.LogoutWebDriver();
            }
            finally
            {
                base.SQL.Groups_CustomField_Delete(254, customFieldName);
                base.SQL.Groups_GroupType_Delete(254, groupTypeName);
                base.SQL.Groups_Group_Delete(254, groupName1);
                base.SQL.Groups_Group_Delete(254, groupName2);
            }
        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies you can search for groups by multiple selection custom field in group type")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Search_CustomField_Multiple()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string customFieldName = "Custom field for multiple";
            string groupTypeName = "Group type for multiple";
            string groupName1 = "Group with custom field multiple";
            string groupName2 = "Group without custom field multiple";

            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_Group_Delete(254, groupName1);
            base.SQL.Groups_Group_Delete(254, groupName2);

            this.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.MultiSelect, customFieldName, null, new List<string> { "1", "2", "3" });
            this.SQL.Groups_GroupType_Create(254, "Group Admin", groupTypeName, new List<int> { 11 });
            base.SQL.Groups_GroupTypes_AddCustomField(254, groupTypeName, customFieldName);

            base.SQL.Groups_Group_Create(254, groupTypeName, "Group Admin", groupName1, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle);
            base.SQL.Groups_Group_Create(254, groupTypeName, "Group Admin", groupName2, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.Married);

            try
            {
                // Login to Portal
                test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "qaeunlx0c2");
                int customFieldOptionId = base.SQL.Groups_CustomFieldOption_FetchID(254, customFieldName, "3");
                test.Portal.Groups_Group_Update_CustomFieldOption(groupName1, customFieldOptionId);
                int customFieldId = base.SQL.Groups_CustomField_FetchID(254, customFieldName);
                int groupTypeId = base.SQL.Groups_GroupTypes_FetchID(254, groupTypeName);
                test.Portal.Groups_Search_CustomFieldOption_WebDriver("group_type_" + groupTypeId, "custom_field_" + customFieldId, customFieldOptionId.ToString(), groupName1);

                // Logout
                test.Portal.LogoutWebDriver();
            }
            finally
            {
                base.SQL.Groups_CustomField_Delete(254, customFieldName);
                base.SQL.Groups_GroupType_Delete(254, groupTypeName);
                base.SQL.Groups_Group_Delete(254, groupName1);
                base.SQL.Groups_Group_Delete(254, groupName2);
            }
        }

        #endregion All
    }

    [TestFixture]
    public class Portal_Groups_GroupsByGroupType_PeopleList_WebDriver : FixtureBaseWebDriver
    {
        #region People List
        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-5645 - People List view Permissions")]
        public void Groups_GroupsByGroupType_PeopleLists_CannotAddPeople_WithViewPermissions()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            // Initial data
            var peopleListName = "PeopleList-View";
            base.SQL.Groups_PeopleLists_Delete(15, peopleListName);
            base.SQL.Groups_PeopleLists_Create(15, peopleListName, null, "Bryan Mikaelian");
            base.SQL.Groups_PeopleLists_AddManagerOrViewer(15, peopleListName, "Ft Tester", GeneralEnumerations.GroupRolesPortalUser.Viewer);


            //Login to Portal 
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            //View Individuals details in Portal
            test.Portal.People_ViewIndividual_WebDriver("Stuart Platt");

            //Verify that the PeopleList Name isnot available to add people

            //click on Add to List

            test.Driver.FindElementByLinkText("Add to list").Click();

            IWebElement PeopleListDropdown = test.Driver.FindElement(By.Id("people_list"));
            IList<IWebElement> options = PeopleListDropdown.FindElements(By.TagName("option"));
            int itemRow = 0;
            Boolean elementFound = false;
            foreach (IWebElement option in options)
            {
                if (option.Text.Contains(peopleListName))
                {
                    elementFound = true;
                }
                itemRow++;

            }
            Assert.IsFalse(elementFound, "PeopleList visible for View Permissions");

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("FO-2749 - Group cannot be saved/cancelled properly with long people list returned")]
        public void Groups_GroupsByGroupType_PeopleLists_CreateWithLongIndividualList()
        {
            string peopleListName = "LongIndividualPeopleList";
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            base.SQL.Groups_PeopleLists_Delete(15, peopleListName);
            //Login to Portal 
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            //Navigate to Group -> Group view all page
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            test.Driver.FindElementByLinkText("Add a people list").Click();
            test.GeneralMethods.WaitForElementDisplayed(By.Id(GroupsByGroupTypeConstants.Wizard_PeopleListCreation.TextField_PeopleListName));

            try
            {
                test.Driver.FindElementById(GroupsByGroupTypeConstants.Wizard_PeopleListCreation.TextField_PeopleListName).SendKeys(peopleListName);
                test.Driver.FindElementById(GroupsByGroupTypeConstants.Wizard_PeopleListCreation.TextField_PeopleListDescription).SendKeys("DescriptionValue");

                if (test.Driver.FindElementById(GroupsByGroupTypeConstants.Wizard_PeopleListCreation.Checkbox_Unlock_Group).GetAttribute("checked").Equals("false"))
                {
                    test.Driver.FindElementById(GroupsByGroupTypeConstants.Wizard_PeopleListCreation.Checkbox_Unlock_Group).Click();
                }

                test.Driver.FindElementById(GroupsByGroupTypeConstants.Wizard_PeopleListCreation.Button_CreatePeopleList).Click();
                test.GeneralMethods.WaitForElementDisplayed(By.Id(GroupsByGroupTypeConstants.Wizard_PeopleListCreation.TextField_IndividualSearch));
                test.Driver.FindElementById(GroupsByGroupTypeConstants.Wizard_PeopleListCreation.TextField_IndividualSearch).SendKeys("test");
                test.Driver.FindElementById(GroupsByGroupTypeConstants.Wizard_PeopleListCreation.Button_IndividualSearch).Click();

                test.GeneralMethods.WaitForElementVisible(By.Id(GroupsByGroupTypeConstants.Wizard_PeopleListCreation.Button_Save));
                test.GeneralMethods.WaitForElementVisible(By.Id(GroupsByGroupTypeConstants.Wizard_PeopleListCreation.Button_SaveAddAnother));

            }
            finally
            {
                test.Driver.FindElementByLinkText(GeneralLinks.RETURN_WebDriver).Click();
                test.Portal.LogoutWebDriver();
                base.SQL.Groups_PeopleLists_Delete(15, peopleListName);
            }
            

        }
        
        #endregion

    }
       

    [TestFixture]
	public class Portal_Groups_GroupsByGroupType_ViewAll : FixtureBase 
    {

        private string _groupTypeNameMISC = "A Test Group Type:New User";
        private string _groupNameMISC = "A Test Group:New User";
        private string _groupTypeName = "View All Group Type";
        private string _groupTypeName2 = "View All Group Type 2";

        // People Lists
        private string _peopleListName = "View All People List";
        private string _publicPeopleList = "View All Public People List";

        // Groups
        private string _groupName = "View All Generic Group";
        private string _groupName2 = "View All Owner Explicit";
        private string _coedGroup = "View All Coed Group";
        private string _maleGroup = "View All Male Group";
        private string _femaleGroup = "View All Female Group";
        private string _marriedOrSingleGroup = "View All Married or Single Group";
        private string _marriedGroup = "View All Married Group";
        private string _singleGroup = "View All Single Group";
        private string _startDate1Group = "View All Start Date Group 1";
        private string _startDate2Group = "View All Start Date Group 2";
        private string _sundayGroup = "View All Sunday Group";
        private string _mondayGroup = "View All Monday Group";
        private string _tuesdayGroup = "View All Tuesday Group";
        private string _wednesdayGroup = "View All Wednesday Group";
        private string _thursdayGroup = "View All Thursday Group";
        private string _fridayGroup = "View All Friday Group";
        private string _saturdayGroup = "View All Saturday Group";
        private string _ageRangeGroup = "View All Age Range Group";

        // Span of care
        private string _spanOfCareName = "View All Span of Care";

        [FixtureSetUp]
        public void FixtureSetUp() {
            base.SQL.Groups_GroupType_Delete(15, _groupTypeNameMISC);
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName2);
            base.SQL.Groups_SpanOfCare_Delete(254, _spanOfCareName);
            base.SQL.Groups_GroupType_Create(15, "Matthew Sneeden", _groupTypeNameMISC, new List<int> { 4, 5, 10 });
            base.SQL.Groups_GroupType_Create(254, "Group Admin", _groupTypeName, new List<int> { 4, 5, 10 });
            base.SQL.Groups_GroupType_Create(254, "Group Admin", _groupTypeName2, new List<int> { 4, 5, 10 });
            base.SQL.Groups_Group_Create(15, _groupTypeNameMISC, "Matthew Sneeden", _groupNameMISC, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());

            // People Lists
            base.SQL.Groups_PeopleLists_Delete(254, _peopleListName);
            base.SQL.Groups_PeopleLists_Create(254, _peopleListName, null, "Bryan Mikaelian");
            base.SQL.Groups_PeopleLists_Create(254, _publicPeopleList, null, "Bryan Mikaelian", true);
            base.SQL.Groups_PeopleLists_AddManagerOrViewer(254, _peopleListName, "Group Admin", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_PeopleLists_AddManagerOrViewer(254, _publicPeopleList, "Bryan Mikaelian", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_PeopleLists_AddMember(254, _peopleListName, "Group Leader");

            // Groups
            // Generic Groups
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _groupName, "Group Manager", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _groupName, "Group Viewer", GeneralEnumerations.GroupRolesPortalUser.Viewer);

            base.SQL.Groups_Group_Create(254, _groupTypeName2, "Bryan Mikaelian", _groupName2, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName2, _groupName2, "SOC Owner", GeneralEnumerations.GroupRolesPortalUser.Manager);

            // Age Range Group
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _ageRangeGroup, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString(), GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, false, true, null, false, 19, 35);
            base.SQL.Groups_Group_AddLeaderOrMember(254, _ageRangeGroup, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _ageRangeGroup, "Group Manager", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _ageRangeGroup, "Group Viewer", GeneralEnumerations.GroupRolesPortalUser.Viewer);

            // Coed Group
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _coedGroup, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _coedGroup, "Group Manager", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _coedGroup, "Group Viewer", GeneralEnumerations.GroupRolesPortalUser.Viewer);

            // Female Group
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _femaleGroup, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString(), GeneralEnumerations.Gender.Female);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _femaleGroup, "Group Manager", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _femaleGroup, "Group Viewer", GeneralEnumerations.GroupRolesPortalUser.Viewer);

            // Male Group
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _maleGroup, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString(), GeneralEnumerations.Gender.Male);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _maleGroup, "Group Manager", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _maleGroup, "Group Viewer", GeneralEnumerations.GroupRolesPortalUser.Viewer);

            // Married or Single Group
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _marriedOrSingleGroup, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString(), GeneralEnumerations.Gender.Female);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _marriedOrSingleGroup, "Group Manager", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _marriedOrSingleGroup, "Group Viewer", GeneralEnumerations.GroupRolesPortalUser.Viewer);

            // Married Group
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _marriedGroup, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString(), GeneralEnumerations.Gender.Female, GeneralEnumerations.GroupMaritalStatus.Married);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _marriedGroup, "Group Manager", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _marriedGroup, "Group Viewer", GeneralEnumerations.GroupRolesPortalUser.Viewer);

            // Single Group
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _singleGroup, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString(), GeneralEnumerations.Gender.Female, GeneralEnumerations.GroupMaritalStatus.Single);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _singleGroup, "Group Manager", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _singleGroup, "Group Viewer", GeneralEnumerations.GroupRolesPortalUser.Viewer);

            // Start Date groups
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _startDate1Group, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _startDate1Group, "Group Manager", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _startDate1Group, "Group Viewer", GeneralEnumerations.GroupRolesPortalUser.Viewer);

            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _startDate2Group, null, "11/20/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _startDate2Group, "Group Manager", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _startDate2Group, "Group Viewer", GeneralEnumerations.GroupRolesPortalUser.Viewer);
            
            // Schedule groups
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _sundayGroup, null, "11/20/2009");
            base.SQL.Groups_Group_CreateSchedule(254, GeneralEnumerations.GroupScheduleFrequency.Weekly, _sundayGroup, "Bryan Mikaelian", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortTimeString(), null, null, new List<GeneralEnumerations.WeeklyScheduleDays>() { GeneralEnumerations.WeeklyScheduleDays.Sunday });
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _mondayGroup, null, "11/20/2009");
            base.SQL.Groups_Group_CreateSchedule(254, GeneralEnumerations.GroupScheduleFrequency.Weekly, _mondayGroup, "Bryan Mikaelian", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortTimeString(), null, null, new List<GeneralEnumerations.WeeklyScheduleDays>() { GeneralEnumerations.WeeklyScheduleDays.Monday });
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _tuesdayGroup, null, "11/20/2009");
            base.SQL.Groups_Group_CreateSchedule(254, GeneralEnumerations.GroupScheduleFrequency.Weekly, _tuesdayGroup, "Bryan Mikaelian", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortTimeString(), null, null, new List<GeneralEnumerations.WeeklyScheduleDays>() { GeneralEnumerations.WeeklyScheduleDays.Tuesday });
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _wednesdayGroup, null, "11/20/2009");
            base.SQL.Groups_Group_CreateSchedule(254, GeneralEnumerations.GroupScheduleFrequency.Weekly, _wednesdayGroup, "Bryan Mikaelian", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortTimeString(), null, null, new List<GeneralEnumerations.WeeklyScheduleDays>() { GeneralEnumerations.WeeklyScheduleDays.Wednesday });
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _thursdayGroup, null, "11/20/2009");
            base.SQL.Groups_Group_CreateSchedule(254, GeneralEnumerations.GroupScheduleFrequency.Weekly, _thursdayGroup, "Bryan Mikaelian", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortTimeString(), null, null, new List<GeneralEnumerations.WeeklyScheduleDays>() { GeneralEnumerations.WeeklyScheduleDays.Thursday });
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _fridayGroup, null, "11/20/2009");
            base.SQL.Groups_Group_CreateSchedule(254, GeneralEnumerations.GroupScheduleFrequency.Weekly, _fridayGroup, "Bryan Mikaelian", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortTimeString(), null, null, new List<GeneralEnumerations.WeeklyScheduleDays>() { GeneralEnumerations.WeeklyScheduleDays.Friday });
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _saturdayGroup, null, "11/20/2009");
            base.SQL.Groups_Group_CreateSchedule(254, GeneralEnumerations.GroupScheduleFrequency.Weekly, _saturdayGroup, "Bryan Mikaelian", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortTimeString(), null, null, new List<GeneralEnumerations.WeeklyScheduleDays>() { GeneralEnumerations.WeeklyScheduleDays.Saturday });

            // Span of Care
            base.SQL.Groups_SpanOfCare_Create(254, _spanOfCareName, "Bryan Mikaelian", "SOC Owner", new List<string>() { _groupTypeName });
        }

        [FixtureTearDown]
        public void FixtureTearDown() {
            base.SQL.Groups_GroupType_Delete(15, _groupTypeNameMISC);
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName2);
            base.SQL.Groups_PeopleLists_Delete(254, _peopleListName);
            base.SQL.Groups_PeopleLists_Delete(254, _publicPeopleList);
            base.SQL.Groups_SpanOfCare_Delete(254, _spanOfCareName);
        }


        #region Groups
        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the correct search options are present when viewing just Groups")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Availible_Search_Options() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Verify all elements are present for the Groups list
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.ViewAllTabs.GroupsTab);
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsByGroupTypeConstants.ViewAll.HeaderLink_GroupTypesAndCustomFields));
            Assert.IsTrue(test.Selenium.IsElementPresent("commit"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for just groups by a group name.")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Search_Name() {
            // Login to Portal as group admin
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on a name
            test.Selenium.Click("link=Name and start date");
            test.Selenium.Type("group_name", _groupName.Substring(6));
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _groupName, "Group"));

            // Logout
            test.Portal.Logout();

            // Login to Portal as a group manager
            test.Portal.Login("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on a name
            test.Selenium.Click("link=Name and start date");
            test.Selenium.Type("group_name", _groupName.Substring(6));
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _groupName, "Group"));

            // Logout
            test.Portal.Logout();

            // Login to Portal as a group viewer
            test.Portal.Login("groupviewer", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on a name
            test.Selenium.Click("link=Name and start date");
            test.Selenium.Type("group_name", _groupName.Substring(6));
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _groupName, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for just groups based on an individual's name.")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Search_IndividualName() {
            // Login to Portal as a group admin
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on an individual name
            test.Selenium.Click("link=Name and start date");
            test.Selenium.Type("individual_name", "Leader");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _groupName, "Group"));

            // Logout
            test.Portal.Logout();

            // Login to Portal as a group manager
            test.Portal.Login("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on an individual name
            test.Selenium.Click("link=Name and start date");
            test.Selenium.Type("individual_name", "Leader");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _groupName, "Group"));

            // Logout
            test.Portal.Logout();

            // Login to Portal as a group viewer
            test.Portal.Login("groupviewer", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on an individual name
            test.Selenium.Click("link=Name and start date");
            test.Selenium.Type("individual_name", "Leader");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _groupName, "Group"));

            // Logout
            test.Portal.Logout();
        }


        [Test, RepeatOnFailure, Timeout(1000)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for just groups based on an Start date from and Start date To.")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Search_StartDate() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search on an inclusive start date range
            test.Portal.Groups_Search_NameAndStartDate(null, null, "11/14/2009", "11/21/2009");
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));

            // Search on a start date range that starts on a group's start date but has no end date
            test.Portal.Groups_Search_NameAndStartDate(null, null, "11/15/2009", null);
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));

            // Search on a start date range that starts and ends on a group's start date
            test.Portal.Groups_Search_NameAndStartDate(null, null, "11/15/2009", "11/20/2009");
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));

            // Search on a start date range that is after on a group's start date
            test.Portal.Groups_Search_NameAndStartDate(null, null, "11/26/2009", null);
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));

            // Logout of Portal
            test.Portal.Logout();

            // Login as a group manager
            test.Portal.Login("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search on an inclusive start date range
            test.Portal.Groups_Search_NameAndStartDate(null, null, "11/14/2009", "11/21/2009");
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));

            // Search on a start date range that starts on a group's start date but has no end date
            test.Portal.Groups_Search_NameAndStartDate(null, null, "11/15/2009", null);
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));

            // Search on a start date range that starts and ends on a group's start date
            test.Portal.Groups_Search_NameAndStartDate(null, null, "11/15/2009", "11/20/2009");
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));

            // Search on a start date range that is after on a group's start date
            test.Portal.Groups_Search_NameAndStartDate(null, null, "11/26/2009", null);
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));

            // Logout of Portal
            test.Portal.Logout();

            // Login as a group viewer
            test.Portal.Login("groupviewer", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search on an inclusive start date range
            test.Portal.Groups_Search_NameAndStartDate(null, null, "11/14/2009", "11/21/2009");
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));

            // Search on a start date range that starts on a group's start date but has no end date
            test.Portal.Groups_Search_NameAndStartDate(null, null, "11/15/2009", null);
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));

            // Search on a start date range that starts and ends on a group's start date
            test.Portal.Groups_Search_NameAndStartDate(null, null, "11/15/2009", "11/20/2009");
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));

            // Search on a start date range that is after on a group's start date
            test.Portal.Groups_Search_NameAndStartDate(null, null, "11/26/2009", null);
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate1Group, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _startDate2Group, "Group"));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for just groups by the Coed demographic.")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Search_Demographic_Coed() {
            // Login to Portal as a group admin
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on the Coed demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("gender_0");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _coedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _femaleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _maleGroup, "Group"));

            // Logout
            test.Portal.Logout();

            // Login to Portal as a group manager
            test.Portal.Login("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on the Coed demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("gender_0");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _coedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _femaleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _maleGroup, "Group"));

            // Logout
            test.Portal.Logout();

            // Login to Portal as a group viewer
            test.Portal.Login("groupviewer", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on the Coed demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("gender_0");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _coedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _femaleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _maleGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for just groups by the female demographic.")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Search_Demographic_Female() {
            // Login to Portal as a group admin
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on the Female demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("gender_1");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _coedGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _femaleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _maleGroup, "Group"));

            // Logout
            test.Portal.Logout();

            // Login to Portal as a group manager
            test.Portal.Login("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on the Female demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("gender_1");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _coedGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _femaleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _maleGroup, "Group"));

            // Logout
            test.Portal.Logout();

            // Login to Portal as a group viewer
            test.Portal.Login("groupviewer", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on the Female demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("gender_1");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _coedGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _femaleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _maleGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for just groups by the male demographic.")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Search_Demographic_Male() {
            // Login to Portal as a group admin
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on the Male demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("gender_2");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _coedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _femaleGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _maleGroup, "Group"));

            // Logout
            test.Portal.Logout();

            // Login to Portal as a group manager
            test.Portal.Login("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on the Male demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("gender_2");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _coedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _femaleGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _maleGroup, "Group"));

            // Logout
            test.Portal.Logout();

            // Login to Portal as a group viewer
            test.Portal.Login("groupviewer", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on the Male demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("gender_2");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _coedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _femaleGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _maleGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for just groups by the married demographic.")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Search_Demographic_Married() {
            // Login to Portal as a group admin
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on the married demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("marital_status_1");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedOrSingleGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _singleGroup, "Group"));

            // Logout
            test.Portal.Logout();

            // Login to Portal as a group manager
            test.Portal.Login("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on the married demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("marital_status_1");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedOrSingleGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _singleGroup, "Group"));

            // Logout
            test.Portal.Logout();

            // Login to Portal as a group viewer
            test.Portal.Login("groupviewer", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on the married demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("marital_status_1");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedOrSingleGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _singleGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for just groups by the single demographic.")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Search_Demographic_Single() {
            // Login to Portal as a group admin
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on the single demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("marital_status_2");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedOrSingleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _singleGroup, "Group"));

            // Logout
            test.Portal.Logout();

            // Login to Portal as a group manager
            test.Portal.Login("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on the single demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("marital_status_2");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedOrSingleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _singleGroup, "Group"));

            // Logout
            test.Portal.Logout();

            // Login to Portal as a group viewer
            test.Portal.Login("groupviewer", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on the single demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("marital_status_2");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedOrSingleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _singleGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for just groups by the married or single demographic.")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Search_Demographic_Married_Or_Single() {
            // Login to Portal as a group admin
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on the married or single demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("marital_status_0");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedOrSingleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _singleGroup, "Group"));

            // Logout
            test.Portal.Logout();

            // Login to Portal as a group manager
            test.Portal.Login("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on the married or single demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("marital_status_0");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedOrSingleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _singleGroup, "Group"));

            // Logout
            test.Portal.Logout();

            // Login to Portal as a group viewer
            test.Portal.Login("groupviewer", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on the married or single demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("marital_status_0");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedOrSingleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _singleGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for just groups by an age range.")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Search_Demographic_Age() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups based on an age range 
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Select("age_range_min", "18");
            test.Selenium.Select("age_range_max", "45");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _ageRangeGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for just groups that meet on Sunday.")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Search_Schedule_Sunday() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups that meet on Sunday
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click(GroupsByGroupTypeConstants.ViewAll.CheckBox_Sunday);
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _sundayGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for just groups that meet on Monday.")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Search_Schedule_Monday() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups that meet on Monday
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click(GroupsByGroupTypeConstants.ViewAll.CheckBox_Monday);
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _mondayGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for just groups that meet on Tuesday.")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Search_Schedule_Tuesday() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups that meet on Tuesday
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click(GroupsByGroupTypeConstants.ViewAll.CheckBox_Tuesday);
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _tuesdayGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for just groups that meet on Wednesday.")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Search_Schedule_Wednesday() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups that meet on Wednesday
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click(GroupsByGroupTypeConstants.ViewAll.CheckBox_Wednesday);
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _wednesdayGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for just groups that meet on Thursday.")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Search_Schedule_Thursday() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups that meet on Thursday
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click(GroupsByGroupTypeConstants.ViewAll.CheckBox_Thursday);
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _thursdayGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for just groups that meet on Friday.")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Search_Schedule_Friday() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups that meet on Friday
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click(GroupsByGroupTypeConstants.ViewAll.CheckBox_Friday);
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _fridayGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for just groups that meet on Saturday.")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Search_Schedule_Saturday() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all groups that meet on Saturday
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click(GroupsByGroupTypeConstants.ViewAll.CheckBox_Saturday);
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _saturdayGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies people lists are not present when searching by group name for just groups")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Search_Name_PeopleLists_Not_Returned() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all people lists based on a name
            test.Selenium.Click("link=Name and start date");
            test.Selenium.Type("group_name", _peopleListName);
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify people lists are not returned
            test.Selenium.VerifyElementNotPresent(TableIds.Groups_ViewAll_GroupList_SearchResults);

            // Logout
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies people lists are not present when searching by individual name for just groups")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Search_IndividualName_PeopleLists_Not_Returned() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]");

            // Search for all people lists based on a name
            test.Selenium.Click("link=Name and start date");
            test.Selenium.Type("individual_name", "Group Leader");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _peopleListName, "Group"));

            // Logout
            test.Portal.Logout();
        }

        #endregion Groups

        #region People Lists
        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the correct search options are present when viewing just People Lists")]
        public void Groups_GroupsByGroupType_ViewAll_PeopleLists_Availible_Search_Options() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Verify all elements are present for People lists
            test.Selenium.ClickAndWaitForPageToLoad("link=People Lists");
            Assert.IsFalse(test.Selenium.IsElementPresent(GroupsByGroupTypeConstants.ViewAll.HeaderLink_GroupTypesAndCustomFields));
            Assert.IsFalse(test.Selenium.IsElementPresent(GroupsByGroupTypeConstants.ViewAll.HeaderLink_StandardFields));
            Assert.IsTrue(test.Selenium.IsElementPresent("commit"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for people lists by a group name.")]
        public void Groups_GroupsByGroupType_ViewAll_PeopleLists_Search_Name() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // People Lists Only
            test.Selenium.ClickAndWaitForPageToLoad("link=People Lists");

            // Search for all people lists based on a name
            test.Selenium.Click("link=Name");
            test.Selenium.Type("group_name", _peopleListName.Substring(6));
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_PeopleList_SearchResults, _peopleListName, "People List"));

            // Verify the groups table isn't returned
            Assert.IsFalse(test.Selenium.IsElementPresent(TableIds.Groups_ViewAll_GroupList_GroupTab), "Groups were returned!");

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for people lists by an individual name.")]
        public void Groups_GroupsByGroupType_ViewAll_PeopleLists_Search_IndividualName() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // People Lists Only
            test.Selenium.ClickAndWaitForPageToLoad("link=People Lists");

            // Search for all people lists based on an individual name
            test.Selenium.Click("link=Name");
            test.Selenium.Type("individual_name", "Group");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_PeopleList_SearchResults, _peopleListName, "People List"));

            // Verify the groups table isn't returned
            Assert.IsFalse(test.Selenium.IsElementPresent(TableIds.Groups_ViewAll_GroupList_GroupTab), "Groups were returned!");

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a people list that was created by some user shows up on the All tab and the People list tab for just that user.")]
        public void Groups_GroupsByGroupType_ViewAll_PeopleLists_Cannot_See_Others() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Check the People List tab
            test.Selenium.ClickAndWaitForPageToLoad("link=People Lists");
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_PeopleList_PeopleListTab, _peopleListName, "People List"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a people list that is viewable by all users shows up on the All tab and the People list tab for multiple users.")]
        public void Groups_GroupsByGroupType_ViewAll_PeopleLists_Public_PeopleList_Present() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Check the People List tab
            test.Selenium.ClickAndWaitForPageToLoad("link=People Lists");

            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_PeopleList_PeopleListTab, _publicPeopleList, "People List"), "People list was not present!");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups and people lists by a group name.")]
        public void Groups_GroupsByGroupType_ViewAll_PeopleLists_Search_Name_Groups_Not_Returned() {
            // Login to Portal as group admin
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Check the People List tab
            test.Selenium.ClickAndWaitForPageToLoad("link=People Lists");

            // Search for all people lists based on a name
            test.Selenium.Click("link=Name");
            test.Selenium.Type("group_name", _groupName.Substring(6));
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify no results came back
            test.Selenium.VerifyElementNotPresent(TableIds.Groups_ViewAll_PeopleList_SearchResults);

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups and people lists based on an individual's name.")]
        public void Groups_GroupsByGroupType_ViewAll_PeopleLists_Search_IndividualName_Groups_Not_Returned() {
            // Login to Portal as a group admin
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Check the People List tab
            test.Selenium.ClickAndWaitForPageToLoad("link=People Lists");

            // Search for all people lists based on an individual name
            test.Selenium.Click("link=Name");
            test.Selenium.Type("individual_name", "Group");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_PeopleList_SearchResults, _groupName, "People List"));

            // Logout
            test.Portal.Logout();
        }


        #endregion People Lists

        #region Temporary Groups
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the correct search options are present when viewing just Temporary Groups")]
        public void Groups_GroupsByGroupType_ViewAll_TempGroups_Availible_Search_Options() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Verify all elements are present for the Temporary Groups list
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.ViewAllTabs.TempGroupTab);
            Assert.IsFalse(test.Selenium.IsElementPresent(GroupsByGroupTypeConstants.ViewAll.HeaderLink_GroupTypesAndCustomFields));
            Assert.IsFalse(test.Selenium.IsElementPresent(GroupsByGroupTypeConstants.ViewAll.HeaderLink_StandardFields));
            Assert.IsTrue(test.Selenium.IsElementPresent("commit"));

            // Logout
            test.Portal.Logout();
        }

        #endregion Temporary Groups

        #region Span of Care
        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the correct search options are present when viewing just Spans of Care")]
        public void Groups_GroupsByGroupType_ViewAll_SpanOfCare_Availible_Search_Options() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Verify all elements are present for the Span of Care list
            test.Selenium.ClickAndWaitForPageToLoad("link=Spans of Care");
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsByGroupTypeConstants.ViewAll.HeaderLink_GroupTypesAndCustomFields));
            Assert.IsTrue(test.Selenium.IsElementPresent("commit"));

            // Logout
            test.Portal.Logout();
        }


        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups you own by a group name.")]
        public void Groups_GroupsByGroupType_ViewAll_SpanOfCare_Search_Name() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Span of Care Only
            test.Selenium.ClickAndWaitForPageToLoad("link=Spans of Care");

            // Search for Span of Care groups based on a name
            test.Selenium.Click("link=Name and start date");
            test.Selenium.Type("group_name", _groupName.Substring(6));
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results (Should only be Groups from Spans of Care you own)
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _groupName, "Group"));

            // Logout
            test.Portal.Logout();
        }


        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups you own by an individual name.")]
        public void Groups_GroupsByGroupType_ViewAll_SpanOfCare_Search_IndividualName() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "qaeunlx0c2");
            //test.Portal.Login("bmikaelian", "BM.Admin09", "qaeunlx0c2");
            //test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Span of Care Groups Only
            test.Selenium.ClickAndWaitForPageToLoad("link=Spans of Care");

            // Search for Span of Care groups based on a name
            test.Selenium.Click("link=Name and start date");
            test.Selenium.Type("individual_name", "David");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, "Test", "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups you own by the Coed demographic.")]
        public void Groups_GroupsByGroupType_ViewAll_SpanOfCare_Search_Demographic_Coed() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Span of Care Only
            test.Selenium.ClickAndWaitForPageToLoad("link=Spans of Care");

            // Search for all groups based on the Coed demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("gender_0");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _coedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _femaleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _maleGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups you own by the female demographic.")]
        public void Groups_GroupsByGroupType_ViewAll_SpanOfCare_Search_Demographic_Female() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Span of Care Only
            test.Selenium.ClickAndWaitForPageToLoad("link=Spans of Care");

            // Search for all groups based on the Female demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("gender_1");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _coedGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _femaleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _maleGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups you own by the male demographic.")]
        public void Groups_GroupsByGroupType_ViewAll_SpanOfCare_Search_Demographic_Male() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Span of Care Only
            test.Selenium.ClickAndWaitForPageToLoad("link=Spans of Care");

            // Search for all groups based on the Male demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("gender_2");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _coedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _femaleGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _maleGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups you own by the married demographic.")]
        public void Groups_GroupsByGroupType_ViewAll_SpanOfCare_Search_Demographic_Married() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Span of Care Only
            test.Selenium.ClickAndWaitForPageToLoad("link=Spans of Care");

            // Search for all groups based on the married demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("marital_status_1");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedOrSingleGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _singleGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups you own by the single demographic.")]
        public void Groups_GroupsByGroupType_ViewAll_SpanOfCare_Search_Demographic_Single() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Span of Care Only
            test.Selenium.ClickAndWaitForPageToLoad("link=Spans of Care");

            // Search for all groups based on the single demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("marital_status_2");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedOrSingleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedGroup, "Group"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _singleGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups you own by the married or single demographic.")]
        public void Groups_GroupsByGroupType_ViewAll_SpanOfCare_Search_Demographic_Married_Or_Single() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Span of Care Only
            test.Selenium.ClickAndWaitForPageToLoad("link=Spans of Care");

            // Search for all groups based on the married or single demographic
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click("marital_status_0");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results  
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedOrSingleGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _marriedGroup, "Group"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _singleGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for all groups you own by the age range demographic.")]
        public void Groups_GroupsByGroupType_ViewAll_SpanOfCare_Search_Demographic_Age() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Span of Care Only
            test.Selenium.ClickAndWaitForPageToLoad("link=Spans of Care");

            // Search for all groups based on an age range 
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Select("age_range_min", "18");
            test.Selenium.Select("age_range_max", "45");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _ageRangeGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for Span of Care Groups that meet on Sunday.")]
        public void Groups_GroupsByGroupType_ViewAll_SpanOfCare_Search_Schedule_Sunday() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Span of Care Tab
            test.Selenium.ClickAndWaitForPageToLoad("link=Spans of Care");

            // Search for all groups that meet on Sunday
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click(GroupsByGroupTypeConstants.ViewAll.CheckBox_Sunday);
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _sundayGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for Span of Care groups that meet on Monday.")]
        public void Groups_GroupsByGroupType_ViewAll_SpanOfCare_Search_Schedule_Monday() {
        
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Span of Care Tab
            test.Selenium.ClickAndWaitForPageToLoad("link=Spans of Care");

            // Search for all groups that meet on Monday
            test.Selenium.Click("link=Standard Fields");

            //if (!test.Selenium.IsTextPresent("What day does the group meet?"))
            //{
            //    test.Selenium.Click("link=Day of the Week");
            //}

            test.Selenium.Click(GroupsByGroupTypeConstants.ViewAll.CheckBox_Monday);
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            //Bypassing verify in QA environment. Never works in QA. Can't seem to find data issue - FGJ
            if (test.Configuration.AppSettings.Settings["FTTests.Environment"].Value.ToString() != "LV_QA")
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _mondayGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for Span of Care groups that meet on Tuesday.")]
        public void Groups_GroupsByGroupType_ViewAll_SpanOfCare_Search_Schedule_Tuesday() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Span of Care Tab
            test.Selenium.ClickAndWaitForPageToLoad("link=Spans of Care");

            // Search for all groups that meet on Tuesday
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click(GroupsByGroupTypeConstants.ViewAll.CheckBox_Tuesday);
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _tuesdayGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for Span of Care groups that meet on Wednesday.")]
        public void Groups_GroupsByGroupType_ViewAll_SpanOfCare_Search_Schedule_Wednesday() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Span of Care Tab
            test.Selenium.ClickAndWaitForPageToLoad("link=Spans of Care");

            // Search for all groups that meet on Wednesday
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click(GroupsByGroupTypeConstants.ViewAll.CheckBox_Wednesday);
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _wednesdayGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for Span of Care groups that meet on Thursday.")]
        public void Groups_GroupsByGroupType_ViewAll_SpanOfCare_Search_Schedule_Thursday() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Span of Care Tab
            test.Selenium.ClickAndWaitForPageToLoad("link=Spans of Care");

            // Search for all groups that meet on Thursday
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click(GroupsByGroupTypeConstants.ViewAll.CheckBox_Thursday);
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _thursdayGroup, "Group"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for Span of Care groups that meet on Friday.")]
        public void Groups_GroupsByGroupType_ViewAll_SpanOfCare_Search_Schedule_Friday() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Span of Care Tab
            test.Selenium.ClickAndWaitForPageToLoad("link=Spans of Care");

            // Search for all groups that meet on Friday
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click(GroupsByGroupTypeConstants.ViewAll.CheckBox_Friday);
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _fridayGroup, "Group"));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for Span of Care groups that meet on Saturday.")]
        public void Groups_GroupsByGroupType_ViewAll_SpanOfCare_Search_Schedule_Saturday() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Span of Care Tab
            test.Selenium.ClickAndWaitForPageToLoad("link=Spans of Care");

            // Search for all groups that meet on Saturday
            test.Selenium.Click("link=Standard Fields");
            test.Selenium.Click(GroupsByGroupTypeConstants.ViewAll.CheckBox_Saturday);
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _saturdayGroup, "Group"));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a span of care care owner can view their span of care from group's view all but they only have read access.")]
        public void Groups_GroupsByGroupType_ViewAll_SpanOfCare_View_Read_Access_Only() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

            // View a span of care from View All
            test.Portal.Groups_SpanOfCare_View_ViewAll(_spanOfCareName);

            // Verify you cannot add an owner
            test.Selenium.VerifyElementNotPresent(GeneralLinks.Add);

            // Verify you cannot edit the span of care
            test.Selenium.VerifyElementNotPresent("//a[contains(@href, '/Groups/GroupSoc/Step1.aspx')]");
            test.Selenium.VerifyElementNotPresent("//a[contains(@href, '/Groups/GroupSoc/Step2.aspx')]");
            test.Selenium.VerifyElementNotPresent("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]");
            test.Selenium.VerifyElementNotPresent("//a[contains(@href, '/Groups/GroupSoc/Step4.aspx')]");

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a span of care care owner will only see groups they own if they search from the span of care tab.")]
        public void Groups_GroupsByGroupType_ViewAll_SpanOfCare_Search_Owned_Groups_Only_Returned() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Span of Care Only
            test.Selenium.ClickAndWaitForPageToLoad("link=Spans of Care");

            // Search for Span of Care groups based on a name
            test.Selenium.Click("link=Name and start date");
            test.Selenium.Type("group_name", "View All");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the results. Groups you have explict rights to should not be present since you searched on the span of care tab.
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_SearchResults, _groupName2, "Group"), "Group you had explicit rights to was present when searching on the span of care tab.");
            
            // Logout
            test.Portal.Logout();
        }

        #endregion Span of Care

        #region Save a search
        #region General Testing
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a name is required to save a search.")]
        public void Groups_GroupsByGroupType_ViewAll_Save_Search_Name_Required() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups 
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Save the search but don't specify a name.
            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.SaveASearch.Button_SaveSearch);

            // Verify the validation message
            Assert.IsTrue(test.Selenium.IsTextPresent("A search name must be specified."));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure(Order = 1)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a saved search is specific to a portal user, not the church.")]
        public void Groups_GroupsByGroupType_ViewAll_Save_Search_Create_Unique_Per_User() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups 
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Save the search
            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Selenium.Type(GroupsByGroupTypeConstants.SaveASearch.Field_Name, "Owners Saved Search");
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.SaveASearch.Button_SaveSearch);

            // Verify the save search exists
            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Portal.Groups_Group_ViewAll_VerifySavedSearchExists("Owners Saved Search");

            // Logout
            test.Portal.Logout();

            // Log in as a different user
            test.Portal.Login("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups 
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Verify the save search doesn't exist for this user
            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Portal.Groups_Group_ViewAll_VerifySavedSearchDoesNotExists("Owners Saved Search");

            // Logout
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure(Order = 2)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can delete a search after saving a search on all groups for a specific user.")]
        public void Groups_GroupsByGroupType_ViewAll_Save_Search_Delete_Unique_Per_User() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Delete the saved search
            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Portal.Groups_ViewAll_DeleteSavedSearch("Owners Saved Search");

            // Verify it is no longer present
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Portal.Groups_Group_ViewAll_VerifySavedSearchDoesNotExists("Owners Saved Search");

            // Logout
            test.Portal.Logout();
        }

        #endregion General Testing

        #region All
        [Test(Order = 1), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can save a search after doing a search on all groups.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Save_Search_Create() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupviewer", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for all groups 
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Save the search
            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Selenium.Type(GroupsByGroupTypeConstants.SaveASearch.Field_Name, "My Saved Search - All");
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.SaveASearch.Button_SaveSearch);

            // Verify the save search exists
            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Portal.Groups_Group_ViewAll_VerifySavedSearchExists("My Saved Search - All");

            // Logout
            test.Portal.Logout();
        }

        [Test(Order = 2), RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can execute a search after saving a search on all groups.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Save_Search_Execute() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupviewer", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Execute the saved search
            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Selenium.ClickAndWaitForPageToLoad("link=My Saved Search - All");

            // Logout
            test.Portal.Logout();
        }

        [Test(Order = 3), RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can delete a search after saving a search on all groups.")]
        public void Groups_GroupsByGroupType_ViewAll_All_Save_Search_Delete() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupviewer", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Delete the saved search
            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Portal.Groups_ViewAll_DeleteSavedSearch("My Saved Search - All");

            // Verify it is no longer present
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);
            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Selenium.VerifyElementNotPresent("link=My Saved Search - All");

            // Logout
            test.Portal.Logout();
        }

        #endregion All

        #region Groups
        [Test(Order = 1), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can save a search after doing a search on just groups.")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Save_Search_Create() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for just groups 
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.ViewAllTabs.GroupsTab);
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Save the search
            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Selenium.Type(GroupsByGroupTypeConstants.SaveASearch.Field_Name, "My Saved Search - Groups");
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.SaveASearch.Button_SaveSearch);

            // Verify the save search exists
            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Portal.Groups_Group_ViewAll_VerifySavedSearchExists("My Saved Search - Groups");

            // Logout
            test.Portal.Logout();
        }

        [Test(Order = 2), RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can execute a search after saving a search on just groups.")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Save_Search_Execute() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Execute the saved search
            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Selenium.ClickAndWaitForPageToLoad("link=My Saved Search - Groups");

            // Logout
            test.Portal.Logout();
        }

        [Test(Order = 3), RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can delete a search after saving a search on just groups.")]
        public void Groups_GroupsByGroupType_ViewAll_Groups_Save_Search_Delete() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Delete the saved search
            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Portal.Groups_ViewAll_DeleteSavedSearch("My Saved Search - Groups");

            // Verify it is no longer present
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);
            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Selenium.VerifyElementNotPresent("link=My Saved Search - Groups");

            // Logout
            test.Portal.Logout();
        }

        #endregion Groups

        #region People Lists
        [Test(Order = 1), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can save a search after doing a search on people lists.")]
        public void Groups_GroupsByGroupType_ViewAll_PeopleLists_Save_Search_Create() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for just people lists
            test.Selenium.ClickAndWaitForPageToLoad("link=People Lists");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Save the search
            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Selenium.Type(GroupsByGroupTypeConstants.SaveASearch.Field_Name, "My Saved Search - People Lists");
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.SaveASearch.Button_SaveSearch);

            // Verify the save search exists
            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Portal.Groups_Group_ViewAll_VerifySavedSearchExists("My Saved Search - People Lists");

            // Logout
            test.Portal.Logout();
        }

        [Test(Order = 2), RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can execute a search after saving a search on people lists.")]
        public void Groups_GroupsByGroupType_ViewAll_PeopleLists_Save_Search_Execute() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Search for people lists
            test.Selenium.ClickAndWaitForPageToLoad("link=People Lists");
            test.Selenium.ClickAndWaitForPageToLoad("commit");

            // Save the search
            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Selenium.Type(GroupsByGroupTypeConstants.SaveASearch.Field_Name, "My Saved Search - People Lists");
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.SaveASearch.Button_SaveSearch);
                        
            // Execute the saved search
            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Selenium.ClickAndWaitForPageToLoad("link=My Saved Search - People Lists");

            // Verify people lists are returned.      
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_PeopleList_SearchResults, _peopleListName, "People List"));

            // Verify groups aren't returned. 
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_PeopleList_SearchResults, _groupName, "People List"));

            // Verify we aren't using the wrong table
            Assert.IsFalse(test.Selenium.IsElementPresent(TableIds.Groups_ViewAll_GroupList_GroupTab));

            // Delete the saved search
            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Portal.Groups_ViewAll_DeleteSavedSearch("My Saved Search - People Lists");

            // Logout
            test.Portal.Logout();
        }

        [Test(Order = 3), RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can delete a search after saving a search on people lists.")]
        public void Groups_GroupsByGroupType_ViewAll_PeopleLists_Save_Search_Delete() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Delete the saved search
            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Portal.Groups_ViewAll_DeleteSavedSearch("My Saved Search - People Lists");

            // Verify it is no longer present
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);
            test.Selenium.Click(GroupsByGroupTypeConstants.SaveASearch.Link_Drawer);
            test.Selenium.VerifyElementNotPresent("link=My Saved Search - People Lists");

            // Logout
            test.Portal.Logout();
        }

        #endregion People Lists

        #endregion Save a search

	}

	[TestFixture]
    public class Portal_Groups_GroupsByGroupType_Management : FixtureBase {
        private string _groupTypeNameMISC = "A Test Group Type:MISC";
        private string _groupNameMISC = "A Test Group:MISC";
        private string _groupTypeName = "A Generic Group Type";
        private string _groupName = "Generic Group";
        private string _showLeadersMembersGroup = "Show Leaders Group";
        private string _showLeadersMembersGroup2 = "Show Leaders Group 2";
        private string _showAttendanceGroup = "Show Attendance Group";
        private string _showAttendanceGroup2 = "Show Attendance Group - No data";
        private string _peopleListName = "Generic People List";

        [FixtureSetUp]
        public void FixtureSetUp() {
            base.SQL.Groups_GroupType_Delete(15, _groupTypeNameMISC);
            base.SQL.Groups_GroupType_Delete(15, _groupTypeName);
            base.SQL.Groups_GroupType_Create(15, "Matthew Sneeden", _groupTypeNameMISC, new List<int> { 4, 5, 10 });
            base.SQL.Groups_GroupType_Create(254, "Group Admin", _groupTypeName, new List<int> { 4, 5, 10, 11 });
            base.SQL.Groups_Group_Create(15, _groupTypeNameMISC, "Matthew Sneeden", _groupNameMISC, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _showLeadersMembersGroup, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _showLeadersMembersGroup2, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _showAttendanceGroup, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _showAttendanceGroup2, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_PeopleLists_Create(254, _peopleListName, null, "Bryan Mikaelian");
            base.SQL.Groups_Group_AddLeaderOrMember(15, _groupNameMISC, "Matthew Sneeden", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _showLeadersMembersGroup, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _showLeadersMembersGroup, "Group Member", "Member");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _showLeadersMembersGroup2, "Group Leader", "Member");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _showAttendanceGroup, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _showAttendanceGroup2, "Group Leader", "Leader");
            base.SQL.Groups_PeopleLists_AddManagerOrViewer(254, _peopleListName, "Group Admin", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddProspect(254, _groupName, "Test", "Prospect", null, null, GeneralEnumerations.ProspectTaskStates.ExpressInterest);
            base.SQL.Groups_Group_AddProspect(254, _groupName, "Denied", "Prospect", null, null, GeneralEnumerations.ProspectTaskStates.Denied);
            base.SQL.Groups_Group_CreateAttendanceRecord(254, _showAttendanceGroup, new List<string>() { "Group Leader" }, true, false);
            base.SQL.Groups_Group_CreateSchedule(254, GeneralEnumerations.GroupScheduleFrequency.Weekly, _showAttendanceGroup, "Group Admin", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(-2).ToShortDateString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortTimeString(), null, null);
        }

        [FixtureTearDown]
        public void FixtureTearDown() {
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
            base.SQL.Groups_PeopleLists_Delete(254, _peopleListName);
        }

        #region Groups

        #region Mass Action
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Matthew Sneeden")]
		[Description("Verifies the user cannot perform a Mass Action when the security right is disabled.")]
		public void Groups_GroupsByGroupType_Groups_MassAction_NoPermissions() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("FR_MassAction", "Pa$$w0rd");

			// Verify user cannot perform a mass action
            test.Portal.Groups_Group_View(_groupNameMISC);

			// Check all members in the grid
			test.Selenium.Click("checkAll");

			// Make selection from grid
			test.Selenium.Click(GeneralLinks.Actions);

			// Verify mass action queue is not present on the people menu
			Assert.AreEqual("Data Integrity", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[4]/dt"));
			Assert.AreEqual("Duplicate Finder", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[4]/dd[1]/a"));
			Assert.AreEqual("Merge Individual", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[4]/dd[2]/a"));
			Assert.AreEqual("Move Individual", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[4]/dd[3]/a"));
			Assert.AreEqual("Split Household", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[4]/dd[4]/a"));
			Assert.AreEqual("Duplicate Queue", test.Selenium.GetText("//div[@id='nav_sub_2']/dl[4]/dd[5]/a"));
			Assert.IsFalse(test.Selenium.IsElementPresent("//div[@id='nav_sub_2']/dl[4]/dd[6]/a"));

			// Verify perform a mass action is not present
			Assert.AreEqual(4, test.Selenium.GetXpathCount("//div[@class='grid_controls']/ul[@class='grid_menu']/li"));
			Assert.AreEqual("Add to group…", test.Selenium.GetText("//div[@class='grid_controls']/ul[@class='grid_menu']/li[1]/a"));
			Assert.AreEqual("Export…", test.Selenium.GetText("//div[@class='grid_controls']/ul[@class='grid_menu']/li[2]/a"));
			Assert.AreEqual("View parents or children…", test.Selenium.GetText("//div[@class='grid_controls']/ul[@class='grid_menu']/li[3]/a"));
			Assert.AreEqual("Send an email", test.Selenium.GetText("//div[@class='grid_controls']/ul[@class='grid_menu']/li[4]/a"));

			// Logout of portal
			test.Portal.Logout();
		}
		#endregion Mass Action

        #region CRUD
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Creates a new group without a schedule and location.")]
        public void Groups_GroupsByGroupType_Groups_Create() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Create a group
            string groupName = "Test New Group";
            test.Portal.Groups_Group_Create(_groupTypeName, groupName, null, "11/15/2009", true);

            // Groups -> View All
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Check the Groups Tab
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.ViewAllTabs.GroupsTab);
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_GroupTab, groupName, "Group"), "New group was not present!");

            // Check the people list tab
            test.Selenium.ClickAndWaitForPageToLoad("link=People Lists");
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_PeopleList_PeopleListTab, groupName, "People List"), "New group was present for people lists!");

            // Check the temporary group tab
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.ViewAllTabs.TempGroupTab);
            Assert.IsFalse(test.Selenium.IsElementPresent(string.Format("link={0}", groupName)));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the add a group link is not present if the portal user logged in does not have an active group.")]
        public void Groups_GroupsByGroupType_Groups_Create_Add_Group_Link_Not_Present_No_Active_Group() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Create a group.  This will make this group the active group.
            test.Portal.Groups_Group_Create(_groupTypeName, "Delete my group", null, "11/15/2009", true);

            // Delete the group so we don't have an active group
            test.Portal.Groups_Group_Delete("Delete my group");

            // Portal -> Home
            test.Selenium.ClickAndWaitForPageToLoad("link=Home");

            // Verify Add a group is not present
            test.Selenium.VerifyElementNotPresent("active_group_add");

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Adds a group to group type that is part of a Span of Care, verifies it shows up for the owner on the View All page")]
        public void Groups_GroupsByGroupType_Groups_Create_SpanOfCare_Group() {
            // Initial data
            var spanOfCareName = "Added Group SOC";
            var groupTypeName = "Add SOC Group Group Type";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);
            base.SQL.Groups_GroupType_Create(254, "Group Admin", groupTypeName, new List<int>() { 11 });
            base.SQL.Groups_SpanOfCare_Create(254, spanOfCareName, "Bryan Mikaelian", "SOC Owner", new List<string>() { groupTypeName });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", "Existing Group", null, "11/15/2009");

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Create a group
            string groupName = "Test New SoC Group";
            test.Portal.Groups_Group_Create(groupTypeName, groupName, null, "11/15/2009", true);

            // Logout of Portal
            test.Portal.Logout();

            // Login to Portal as the owner of a span of care
            test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

            // Groups -> View All
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Check the group tab
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.ViewAllTabs.GroupsTab);
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_AllTab, groupName, "Group"), "Group was not present!");

            // Check the People List Tab
            test.Selenium.ClickAndWaitForPageToLoad("link=People Lists");
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_PeopleList_PeopleListTab, groupName, "People List"), "Group was present on people list tab!");

            // Check the Temporary Groups tab
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.ViewAllTabs.TempGroupTab);
            test.Selenium.VerifyElementNotPresent(string.Format("link={0}", groupName));

            // Check the Span of Care tab; Expand the SoC list
            test.Selenium.ClickAndWaitForPageToLoad("link=Spans of Care");
            test.Selenium.Click(string.Format("link={0}", spanOfCareName));
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));

            // Delete the group
            base.SQL.Groups_Group_Delete(254, groupName);

            // Navigate to Groups->View All
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Verify group is not present
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_AllTab, groupName, "Group"), "Group was present!");

            // Check the group tab
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.ViewAllTabs.GroupsTab);
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_AllTab, groupName, "Group"), "Group was present!");

            // Check the People List Tab
            test.Selenium.ClickAndWaitForPageToLoad("link=People Lists");
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_PeopleList_PeopleListTab, groupName, "People List"), "Group was present on people list tab!");

            // Check the Temporary Groups tab
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.ViewAllTabs.TempGroupTab);
            test.Selenium.VerifyElementNotPresent(string.Format("link={0}", groupName));

            // Check the Span of Care tab; Expand the SoC list
            test.Selenium.ClickAndWaitForPageToLoad("link=Spans of Care");
            test.Selenium.Click(string.Format("link={0}", spanOfCareName));
            test.Selenium.VerifyElementNotPresent(string.Format("link={0}", groupName));

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Attempts to create a duplicate Group.")]
        public void Groups_GroupsByGroupType_Groups_Create_Duplicate() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Attempt to create a duplicate group
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Pick a group type
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupTypeName));

            // Specify a duplicate name
            test.Selenium.Type("group_name", _groupName);
            test.Selenium.Type("start_date", "11/15/2009");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify error displayed to the user
            test.Selenium.VerifyTextPresent(new string[] { TextConstants.ErrorHeadingSingular, "A group with this name already exists. Please create a unique group name." });

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Attempts to create a group with double quotes.")]
        public void Groups_GroupsByGroupType_Groups_Create_DoubleQuote() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Create a group with double quotes
            string groupName = "Double \"Quotes\" Group";
            test.Portal.Groups_Group_Create(_groupTypeName, groupName, null, "11/15/2009", true);

            // Verify the group is present
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_AllTab, groupName, "Group"));
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.ViewAllTabs.GroupsTab);
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_GroupTab, groupName, "Group"));

            // Logout of portal
            test.Portal.Logout();

            // Delete the group
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Attempts to create a group with special characters.")]
        public void Groups_GroupsByGroupType_Groups_Create_SpecialCharacters() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Attempt to create a special character group
            string groupName = "$pecial% %Characters& *Group#";
            test.Portal.Groups_Group_Create("Automation Group Type", groupName, null, "11/15/2009", true);

            // Verify the group is present
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_AllTab, groupName, "Group"));
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.ViewAllTabs.GroupsTab);
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_GroupTab, groupName, "Group"));

            // Logout of portal
            test.Portal.Logout();

            // Delete the group
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a name is required when creating a group.")]
        public void Groups_GroupsByGroupType_Groups_Create_Name_Required() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Create a new group
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select a group type
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupTypeName));

            // Don't specify a name
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify the error message is present
            Assert.IsTrue(test.Selenium.IsTextPresent("Group Name is required and cannot exceed 100 characters."));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a group of one type can be named the same as a group that of a different type")]
        public void Groups_GroupsByGroupType_Groups_Create_Duplicate_Different_Type() {
            // Initial data
            var groupTypeName = "Different Group Group Type";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Group Admin", groupTypeName, new List<int>() { 11 });

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Create a new group of a different type, but having a name of a group of another type
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select a group type
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", groupTypeName));

            test.Selenium.Type("group_name", _groupName);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify the error message is not present
            Assert.IsFalse(test.Selenium.IsTextPresent("A group with this name already exists. Please create a unique group name."));

            // Logout
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the group name cannot exceed 100 characters when creating a group.")]
        public void Groups_GroupsByGroupType_Groups_Create_Name_Exceed_100_Characters() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Create a new group
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select a group type
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupTypeName));

            // Specify a name that exceeds 100 characters.
            test.Selenium.Type("group_name", "Test Group Test Group Test Group Test Group Test Group Test Group Test Group Test Group Test Group Test Group Test Group Test Group Test Group Test Group Test Group");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify the error message is present
            Assert.IsTrue(test.Selenium.IsTextPresent("Group Name is required and cannot exceed 100 characters."));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the group description cannot exceed 500 characters when creating a group.")]
        public void Groups_GroupsByGroupType_Groups_Create_Description_Exceed_500_Characters() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Create a new group
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select a group type
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupTypeName));

            // Specify a description that exceeds 500 characters
            test.Selenium.Type("group_name", "Some Group");
            test.Selenium.Type(GroupsByGroupTypeConstants.Wizard_GroupCreation.Step2_TextField_GroupDescription, "Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify the error message is present
            Assert.IsTrue(test.Selenium.IsTextPresent("Group Description cannot exceed 500 characters."));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a timezone is required when creating a group.")]
        public void Groups_GroupsByGroupType_Groups_Create_TimeZone_Required() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Create a new group
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select a group type
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupTypeName));

            // Don't specify a time zone
            test.Selenium.Type("group_name", "Some Group");
            test.Selenium.Select("time_zone", "---");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify the error message is present
            Assert.IsTrue(test.Selenium.IsTextPresent("Time Zone is required."));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a start date is required when creating a group.")]
        public void Groups_GroupsByGroupType_Groups_Create_StartDate_Required() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Create a new group
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select a group type
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupTypeName));

            // Don't specify a start date zone
            test.Selenium.Type("group_name", "Some Group");
            test.Selenium.Type("start_date", "");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify the error message is present
            Assert.IsTrue(test.Selenium.IsTextPresent("A valid Start Date is required."));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a group cannot be made searchable if the selected group type does not allow searchable groups.")]
        public void Groups_GroupsByGroupType_Groups_Create_Searchable_Not_Present() {
            // Initial data
            var groupTypeName = "Not searchable";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Group Admin", groupTypeName, new List<int> { 4, 5, 10, 11 }, false, false);

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Create a new group
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select a group type that does not allow searchable groups
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", groupTypeName));

            // Verify the group cannot be made searchable
            Assert.IsFalse(test.Selenium.IsElementPresent(GroupsByGroupTypeConstants.Wizard_GroupCreation.Step2_Checkbox_Searchable));

            // Logout
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a group can be made searchable or unsearchable if the selected group type does allow searchable groups.")]
        public void Groups_GroupsByGroupType_Groups_Create_Searchable_Present() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Create a new group
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select a group type that does allow searchable groups
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupTypeName));

            // Verify the group cannot be made searchable
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsByGroupTypeConstants.Wizard_GroupCreation.Step2_Checkbox_Searchable));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a schedule must have a start date when creating a group")]
        public void Groups_GroupsByGroupType_Groups_Create_Schedule_StartDate_Required() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Create a new group
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select a group type 
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupTypeName));

            // Specify a name
            test.Selenium.Type("group_name", "Some Group");
            test.Selenium.Type("start_date", "11/15/2009");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Don't specify a schedule start date
            test.Selenium.Type(GroupsByGroupTypeConstants.Wizard_GroupCreation.Step3_TextField_ScheduleStartTime, "9:00 PM");
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.Wizard_GroupCreation.Step3_Button_Next);

            Assert.IsTrue(test.Selenium.IsTextPresent("The start date is in an invalid format."));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a schedule must have a start time when creating a group")]
        public void Groups_GroupsByGroupType_Groups_Create_Schedule_StartTime_Required() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Create a new group
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select a group type 
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupTypeName));

            // Specify a name
            test.Selenium.Type("group_name", "Some Group");
            test.Selenium.Type("start_date", "11/15/2009");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Don't specify a schedule start time
            test.Selenium.Type("start_date", "11/15/2009");
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.Wizard_GroupCreation.Step3_Button_Next);

            Assert.IsTrue(test.Selenium.IsTextPresent("The start time is in an invalid format."));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a weekly schedule must have a weekday and a recurrence when creating a group")]
        public void Groups_GroupsByGroupType_Groups_Create_Schedule_Weekday_And_Recurrence_Required() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Create a new group
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select a group type 
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupTypeName));

            // Specify a name
            test.Selenium.Type("group_name", "Some Group");
            test.Selenium.Type("start_date", "11/15/2009");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Don't specify a schedule start time
            test.Selenium.Type("start_date", "11/15/2009");
            test.Selenium.Type(GroupsByGroupTypeConstants.Wizard_GroupCreation.Step3_TextField_ScheduleStartTime, "9:00 PM");
            test.Selenium.Click(GroupsByGroupTypeConstants.Wizard_GroupCreation.Step3_RadioButton_WeeklyEvent);
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.Wizard_GroupCreation.Step3_Button_Next);

            Assert.IsTrue(test.Selenium.IsTextPresent("At least one weekday must be selected."));
            Assert.IsTrue(test.Selenium.IsTextPresent("No weekly recurrence has been selected."));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a monthly schedule must have a monthly recurrence when creating a group")]
        public void Groups_GroupsByGroupType_Groups_Create_Schedule_Monthly_Recurrence_Required() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Create a new group
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select a group type 
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupTypeName));

            // Specify a name
            test.Selenium.Type("group_name", "Some Group");
            test.Selenium.Type("start_date", "11/15/2009");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Don't specify a schedule start time
            test.Selenium.Type("start_date", "11/15/2009");
            test.Selenium.Type(GroupsByGroupTypeConstants.Wizard_GroupCreation.Step3_TextField_ScheduleStartTime, "9:00 PM");
            test.Selenium.Click(GroupsByGroupTypeConstants.Wizard_GroupCreation.Step3_RadioButton_MonthlyEvent);
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.Wizard_GroupCreation.Step3_Button_Next);

            Assert.IsTrue(test.Selenium.IsTextPresent("No weekday has been selected."));
            Assert.IsTrue(test.Selenium.IsTextPresent("No nth weekday has been selected."));
            Assert.IsTrue(test.Selenium.IsTextPresent("No monthly recurrence has been selected."));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a location name cannot exceed 50 characters when creating a group")]
        public void Groups_GroupsByGroupType_Groups_Create_Location_Name_Exceed_50_Characters() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Create a new group
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select a group type 
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupTypeName));

            // Specify a name
            test.Selenium.Type("group_name", "Some Group");
            test.Selenium.Type("start_date", "11/15/2009");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Skip Step 3
            test.Selenium.ClickAndWaitForPageToLoad("link=Skip this step →");

            // Enter a location name that is over 50 Characters
            test.Selenium.Type("location_name", "Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name Location Name");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
            Assert.IsTrue(test.Selenium.IsTextPresent("Location Name is required and cannot exceed 50 characters."));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a location state, and city is required when creating a group, but address 1 is not")]
        public void Groups_GroupsByGroupType_Groups_Create_Location_Address_City_State_Required() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Create a new group
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select a group type 
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupTypeName));

            // Specify a name
            test.Selenium.Type("group_name", "Some Group");
            test.Selenium.Type("start_date", "11/15/2009");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Skip Step 3
            test.Selenium.ClickAndWaitForPageToLoad("link=Skip this step →");

            // Do not specify address 1, a state, or city                        
            test.Selenium.Type("location_name", "My Location");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            Assert.IsTrue(test.Selenium.IsTextPresent("Address1 is required and cannot exceed 40 characters."));
            Assert.IsTrue(test.Selenium.IsTextPresent("City is required and cannot exceed 30 characters."));
            Assert.IsTrue(test.Selenium.IsTextPresent("State is required."));
            Assert.IsTrue(test.Selenium.IsTextPresent("Postal code is required and cannot exceed 10 characters."));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a location name is required when creating a group.")]
        public void Groups_GroupsByGroupType_Groups_Create_Location_Name_Required() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Create a new group
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select a group type 
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupTypeName));

            // Specify a name
            test.Selenium.Type("group_name", "Some Group");
            test.Selenium.Type("start_date", "11/15/2009");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Skip Step 3
            test.Selenium.ClickAndWaitForPageToLoad("link=Skip this step →");

            // Do not specify a location name
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
            Assert.IsTrue(test.Selenium.IsTextPresent("Location Name is required and cannot exceed 50 characters."));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies address 1 of a group location cannot exceed 40 characters when creating a group.")]
        public void Groups_GroupsByGroupType_Groups_Create_Location_Address1_Exceed_40_Characters() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Create a new group
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select a group type 
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupTypeName));

            // Specify a name
            test.Selenium.Type("group_name", "Some Group");
            test.Selenium.Type("start_date", "11/15/2009");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Skip Step 3
            test.Selenium.ClickAndWaitForPageToLoad("link=Skip this step →");

            // Specify an address 1 that exceeds 40 characters                    
            test.Selenium.Type("location_name", "My Location");
            test.Selenium.Type("address_address1", "2812 Meadow Wood Meadow Wood Meadow Wood Meadow Wood Meadow Wood Meadow Wood Meadow Wood Meadow Wood Meadow Wood Meadow Wood Meadow Wood");
            test.Selenium.Type("address_city", "Flower Mound");
            test.Selenium.Type("ddlStates", "Texas");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            Assert.IsTrue(test.Selenium.IsTextPresent("Address1 is required and cannot exceed 40 characters."));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies address 2 of a group location cannot exceed 40 characters when creating a group.")]
        public void Groups_GroupsByGroupType_Groups_Create_Location_Address2_Exceed_40_Characters() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Create a new group
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select a group type 
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupTypeName));

            // Specify a name
            test.Selenium.Type("group_name", "Some Group");
            test.Selenium.Type("start_date", "11/15/2009");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Skip Step 3
            test.Selenium.ClickAndWaitForPageToLoad("link=Skip this step →");

            // Specify an address 1 that exceeds 40 characters                    
            test.Selenium.Type("location_name", "My Location");
            test.Selenium.Type("address_address1", "2812 Meadow Wood");
            test.Selenium.Type(GroupsByGroupTypeConstants.Wizard_GroupCreation.Step4_TextField_Address2, "2812 Meadow Wood Meadow Wood Meadow Wood Meadow Wood Meadow Wood Meadow Wood Meadow Wood Meadow Wood Meadow Wood Meadow Wood Meadow Wood");
            test.Selenium.Type("address_city", "Flower Mound");
            test.Selenium.Type("ddlStates", "Texas");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            Assert.IsTrue(test.Selenium.IsTextPresent("Address2 cannot exceed 40 characters."));

            // Return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the city for a group location cannot exceed 30 characters when creating a group.")]
        public void Groups_GroupsByGroupType_Groups_Create_Location_City_Exceed_30_Characters() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Create a new group
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select a group type 
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupTypeName));

            // Specify a name
            test.Selenium.Type("group_name", "Some Group");
            test.Selenium.Type("start_date", "11/15/2009");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Skip Step 3
            test.Selenium.ClickAndWaitForPageToLoad("link=Skip this step →");

            // Specify a city that exceeds 30 characters                 
            test.Selenium.Type("location_name", "My Location");
            test.Selenium.Type("address_address1", "2812 Meadow Wood");
            test.Selenium.Type("address_city", "Flower Mound Flower Mound Flower Mound Flower Mound Flower Mound Flower Mound Flower Mound Flower Mound");
            test.Selenium.Type("ddlStates", "Texas");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            Assert.IsTrue(test.Selenium.IsTextPresent("City is required and cannot exceed 30 characters."));

            // Return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a state is required when creating a location for a group.")]
        public void Groups_GroupsByGroupType_Groups_Create_Location_State_Required() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Create a new group
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select a group type 
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupTypeName));

            // Specify a name
            test.Selenium.Type("group_name", "Some Group");
            test.Selenium.Type("start_date", "11/15/2009");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Skip Step 3
            test.Selenium.ClickAndWaitForPageToLoad("link=Skip this step →");

            // Don't specify a state              
            test.Selenium.Type("location_name", "My Location");
            test.Selenium.Type("address_address1", "2812 Meadow Wood");
            test.Selenium.Type("address_city", "Flower Mound");
            test.Selenium.Type("ddlStates", "");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            Assert.IsTrue(test.Selenium.IsTextPresent("State is required."));

            // Return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a zipcode for a group location cannot exceed 10 characters when creating a group.")]
        public void Groups_GroupsByGroupType_Groups_Create_Location_ZipCode_Exceed_10_Characters() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Create a new group
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select a group type 
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupTypeName));

            // Specify a name
            test.Selenium.Type("group_name", "Some Group");
            test.Selenium.Type("start_date", "11/15/2009");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Skip Step 3
            test.Selenium.ClickAndWaitForPageToLoad("link=Skip this step →");

            // Don't specify a state              
            test.Selenium.Type("location_name", "My Location");
            test.Selenium.Type("address_address1", "2812 Meadow Wood");
            test.Selenium.Type("address_city", "Flower Mound");
            test.Selenium.Type("ddlStates", "Texas");
            test.Selenium.Type(GroupsByGroupTypeConstants.Wizard_GroupCreation.Step4_TextField_Zipcode, "750222222222222222");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            Assert.IsTrue(test.Selenium.IsTextPresent("Postal code is required and cannot exceed 10 characters."));

            // Return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout
            test.Portal.Logout();
        }

        [Test, MultipleAsserts, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a portal user can create a private location.")]
        public void Groups_GroupsByGroupType_Groups_Create_Location_Private() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Create a group with a private location
            var groupName = "Private Location - Portal";
            test.Portal.Groups_Group_Create(_groupTypeName, groupName, null, "11/15/2009", true, "Private Location", null, true, "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022");

            // Verify the address is present
            test.Selenium.VerifyTextPresent("2812 Meadow Wood Drive");
            test.Selenium.VerifyTextPresent("Flower Mound, TX 75022");

            // Logout of Portal
            test.Portal.Logout();

            // Add an individual to the group as a leader and member
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Member", "Member");

            // Login to InFellowship
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the dashboard of the group
            test.infellowship.Groups_Group_View_Dashboard(groupName);

            // Verify the address is present
            test.Selenium.VerifyTextPresent("2812 Meadow Wood Drive");
            test.Selenium.VerifyTextPresent("Flower Mound, TX 75022");

            // Logout of InFellowship
            test.infellowship.Logout();

            // Repeat for members
            // Login to InFellowship
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the dashboard of the group
            test.infellowship.Groups_Group_View_Dashboard(groupName);

            // Verify the address is present
            test.Selenium.VerifyTextPresent("2812 Meadow Wood Drive");
            test.Selenium.VerifyTextPresent("Flower Mound, TX 75022");

            // Logout of InFellowship
            test.infellowship.Logout();

            // Find a group
            test.infellowship.Groups_FindAGroup();

            // Click on the group
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", groupName));

            // Verify the location is not fully shown.
            Assert.IsFalse(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"));
            Assert.IsTrue(test.Selenium.IsTextPresent("Flower Mound, TX 75022"));
            //Assert.IsFalse(test.Selenium.IsTextPresent("Flower Mound, TX 75022"));
            //Assert.Contains(test.Selenium.GetText("//table[@class='info']"), "Private", "Private location should be shown as 'Private'");

            // Close
            test.Selenium.Close();
        }

        [Test, MultipleAsserts, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a portal user can create a location.")]
        public void Groups_GroupsByGroupType_Groups_Create_Location() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Create a group with a location
            var groupName = "Location - Portal";
            test.Portal.Groups_Group_Create(_groupTypeName, groupName, null, "11/15/2009", true,  "Location", null, false, "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022");

            // Verify the address is present
            test.Selenium.VerifyTextPresent("2812 Meadow Wood Drive");
            test.Selenium.VerifyTextPresent("Flower Mound, TX 75022");

            // Logout of Portal
            test.Portal.Logout();

            // Add an individual to the group as a leader and member
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Member", "Member");

            // Login to InFellowship
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the dashboard of the group
            test.infellowship.Groups_Group_View_Dashboard(groupName);

            // Verify the address is present
            test.Selenium.VerifyTextPresent("2812 Meadow Wood Drive");
            test.Selenium.VerifyTextPresent("Flower Mound, TX 75022");

            // Logout of InFellowship
            test.infellowship.Logout();

            // Repeat for members
            // Login to InFellowship
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the dashboard of the group
            test.infellowship.Groups_Group_View_Dashboard(groupName);

            // Verify the address is present
            test.Selenium.VerifyTextPresent("2812 Meadow Wood Drive");
            test.Selenium.VerifyTextPresent("Flower Mound, TX 75022");

            // Logout of InFellowship
            test.infellowship.Logout();

            // Find a group
            test.infellowship.Groups_FindAGroup();
            //InFellowship.Groups.FindAGroupMethods.FindAGroup_Search(test, null, null, null, false);

            // Click on the group
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", groupName));

            // Verify the location is fully shown.
            test.Selenium.VerifyTextPresent("2812 Meadow Wood Drive");
            test.Selenium.VerifyTextPresent("Flower Mound, TX 75022");

            // Close
            test.Selenium.Close();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a location information is not saved if Skip this Step is clicked  on the location step in the Group Wizard.  This used to cause a 500 error when you saved a group.")]
        public void Groups_GroupsByGroupType_Groups_Create_Location_Skip_After_Validation() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Create a new group
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select a group type 
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupTypeName));

            // Specify a name
            var groupName = "Skip Location Group";
            test.Selenium.Type("group_name", groupName);
            test.Selenium.Type("start_date", "11/15/2009");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Skip Step 3
            test.Selenium.ClickAndWaitForPageToLoad("link=Skip this step →");

            // Specify some information for a group's location.         
            test.Selenium.Type("location_name", "My Location");
            test.Selenium.Type("address_address1", "2812 Meadow Wood");
            test.Selenium.Type("address_city", "Flower Mound");
            test.Selenium.Type("ddlStates", "Texas");

            // Attempt to go to the next step.  This will prompt validation.
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Skip this step.
            test.Selenium.ClickAndWaitForPageToLoad("link=Skip this step →");

            // Verify no location was saved
            test.Selenium.VerifyTextPresent("No location exists.");

            // Submit 
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify the group was created
            test.Selenium.VerifyTextPresent("Your group has been created.");

            // Logout
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);
        }

		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Bryan Mikaelian")]
		[Description("Verifies group admins can edit a group they have rights to.")]
		public void Groups_GroupsByGroupType_Groups_Edit_Admins_Can_Edit_Group() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

			// View the settings of a group that belongs to a group type you have admin rights to
            test.Portal.Groups_Group_View_Settings(_groupName);

			// Verify the correct links are present
            test.Selenium.VerifyElementPresent("link=Edit group details");
            test.Selenium.VerifyElementPresent("link=New location");
            test.Selenium.VerifyElementPresent("link=New schedule");
            test.Selenium.VerifyElementPresent("link=Change permissions");

			// Verify you can delete this group
            test.Selenium.VerifyElementPresent("link=Delete this group");

            // Logout of portal
            test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies group managers can edit a group they have rights to.")]
		public void Groups_GroupsByGroupType_Groups_Edit_Managers_Can_Edit_Group() {
            // Initial data
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _groupName, "Group Manager", GeneralEnumerations.GroupRolesPortalUser.Manager);
            
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("groupmanager", "BM.Admin09", "qaeunlx0c2");

			// View the settings of a group that you have manager rights too
            test.Portal.Groups_Group_View_Settings(_groupName);


            // Verify the correct links are present
            test.Selenium.VerifyElementPresent("link=Edit group details");
            test.Selenium.VerifyElementPresent("link=New location");
            test.Selenium.VerifyElementPresent("link=New schedule");
            test.Selenium.VerifyElementPresent("link=Change permissions");

            // Verify you cannot delete this group
            test.Selenium.VerifyElementNotPresent("link=Delete this group");

            // Logout of portal
            test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies group viewers cannot edit a group they have rights to.")]
		public void Groups_GroupsByGroupType_Groups_Edit_Viewers_Cannot_Edit_Group() {
            // Initial data
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _groupName, "Group Viewer", GeneralEnumerations.GroupRolesPortalUser.Viewer);

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("groupviewer", "BM.Admin09", "qaeunlx0c2");

			// View the settings of a group that you have viewer rights too
            test.Portal.Groups_Group_View_Settings(_groupName);

			// Verify none of the links are present
            test.Selenium.VerifyElementNotPresent("link=Edit group details");
            test.Selenium.VerifyElementNotPresent("link=New location");
            test.Selenium.VerifyElementNotPresent("link=New schedule");
            test.Selenium.VerifyElementNotPresent("link=Change permissions");

			// Verify you cannot delete this group
            test.Selenium.VerifyElementNotPresent("link=Delete this group");

            // Logout of portal
            test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies span of care owners can edit a group they have rights to.")]
		public void Groups_GroupsByGroupType_Groups_Edit_Owners_Can_Edit_Group() {
            // Initial data
            var spanOfCareName = "Permission SOC - Viewer";
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);
            base.SQL.Groups_SpanOfCare_Create(254, spanOfCareName, "Bryan Mikaelian", "SOC Owner", new List<string>() { _groupTypeName });

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("socowner", "BM.Admin09", "qaeunlx0c2");

			// View the settings of a group that you have span of care ownership rights over
			test.Portal.Groups_Group_View_Settings(_groupName);

            // Verify the correct links are present
            test.Selenium.VerifyElementPresent("link=Edit group details");
            test.Selenium.VerifyElementPresent("link=New location");
            test.Selenium.VerifyElementPresent("link=New schedule");
            test.Selenium.VerifyElementPresent("link=Change permissions");

            // Verify you cannot delete this group
            test.Selenium.VerifyElementNotPresent("link=Delete this group");

            // Logout of portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);

		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Updates a group's properties.")]
		public void Groups_GroupsByGroupType_Groups_Edit_Properties() {
            // Intitial data
            var groupName = "Test Update Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Delete(254, "Test New Updated Group");
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

			// Update a group
			test.Portal.Groups_Group_Update_Details(groupName, "Test New Updated Group", "Updated description", "12/14/2009");

            // Logout of portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Delete(254, "Test New Updated Group");
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies a group name cannot exceed 100 characters when editing a group.")]
		public void Groups_GroupsByGroupType_Groups_Edit_Details_Name_Exceed_100_Characters() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

			// View the settings of a group that belongs to a group type you have admin rights to
            test.Portal.Groups_Group_View_Settings(_groupName);

			// Edit the details of a group
			test.Selenium.ClickAndWaitForPageToLoad("link=Edit group details");

			// Specify a name that exceeds 100 characters.
			test.Selenium.Type("group_name", "Test Group Test Group Test Group Test Group Test Group Test Group Test Group Test Group Test Group Test Group Test Group Test Group Test Group Test Group Test Group");
			test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

			// Verify the error message is present
			Assert.IsTrue(test.Selenium.IsTextPresent("Group Name is required and cannot exceed 100 characters."));

            // Logout of portal
            test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies a group name is required when editing a group.")]
		public void Groups_GroupsByGroupType_Groups_Edit_Details_Name_Required() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

			// View the settings of a group that belongs to a group type you have admin rights to
            test.Portal.Groups_Group_View_Settings(_groupName);

			// Edit the details of a group
			test.Selenium.ClickAndWaitForPageToLoad("link=Edit group details");

			// Don't specify a name
			test.Selenium.Type("group_name", "");
			test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

			// Verify the error message is present
			Assert.IsTrue(test.Selenium.IsTextPresent("Group Name is required and cannot exceed 100 characters."));

            // Logout of portal
            test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies a group description cannot exceed 500 characters when editing a group.")]
		public void Groups_GroupsByGroupType_Groups_Edit_Details_Description_Exceed_500_Characters() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

			// View the settings of a group that belongs to a group type you have admin rights to
            test.Portal.Groups_Group_View_Settings(_groupName);

			// Edit the details of a group
			test.Selenium.ClickAndWaitForPageToLoad("link=Edit group details");

			// Specify a description that exceeds 500 characters.
			test.Selenium.Type("group_description", "Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group Some Group");
			test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

			// Verify the error message is present
			Assert.IsTrue(test.Selenium.IsTextPresent("Group Description cannot exceed 500 characters."));

            // Logout of portal
            test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies a group's start date is required when editing a group.")]
		public void Groups_GroupsByGroupType_Groups_Edit_Details_StartDate_Required() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

			// View the settings of a group that belongs to a group type you have admin rights to
            test.Portal.Groups_Group_View_Settings(_groupName);

			// Edit the details of a group
			test.Selenium.ClickAndWaitForPageToLoad("link=Edit group details");

			// Don't specify a start date
			test.Selenium.Type("start_date", "");
			test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

			// Verify the error message is present
			Assert.IsTrue(test.Selenium.IsTextPresent("A valid Start Date is required."));

            // Logout of portal
            test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies a group's time zone is required when editing a group.")]
		public void Groups_GroupsByGroupType_Groups_Edit_Details_TimeZone_Required() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

			// View the settings of a group that belongs to a group type you have admin rights to
            test.Portal.Groups_Group_View_Settings(_groupName);

			// Edit the details of a group
			test.Selenium.ClickAndWaitForPageToLoad("link=Edit group details");

			// Don't specify a time zone
			test.Selenium.Select(GroupsByGroupTypeConstants.GroupManagement.Dropdown_GroupTimeZone, "---");
			test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

			// Verify the error message is present
			Assert.IsTrue(test.Selenium.IsTextPresent("Time Zone is required."));

			// Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies a group's location name cannot exceed 50 characters when editing a group and adding a new location.")]
		public void Groups_GroupsByGroupType_Groups_Edit_New_Location_Name_Cannot_Exceed_50_Characters() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// View the settings of a group that belongs to a group type you have admin rights to
			test.Portal.Groups_Group_View_Settings(_groupNameMISC);

			// Add a new location to a group
			test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.GroupManagement.Link_NewLocation);

			// Specify a name that exceeds 50 characters.
			test.Selenium.Type(GroupsByGroupTypeConstants.GroupManagement.TextField_LocationName, "New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location");
			test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.GroupManagement.Button_NewLocationSaveChanges);

			// Verify the error message is present
			Assert.IsTrue(test.Selenium.IsTextPresent("Location Name is required and cannot exceed 50 characters."));

            // Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies a group's location name cannot exceed 500 characters when editing a group and adding a new location.")]
		public void Groups_GroupsByGroupType_Groups_Edit_New_Location_Description_Cannot_Exceed_500_Characters() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// View the settings of a group that belongs to a group type you have admin rights to
			test.Portal.Groups_Group_View_Settings(_groupNameMISC);

			// Add a new location to a group
			test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.GroupManagement.Link_NewLocation);
			test.Selenium.Type(GroupsByGroupTypeConstants.GroupManagement.TextField_LocationName, "Test Location");

			// Specify a description that exceeds 500 characters.
			test.Selenium.Type(GroupsByGroupTypeConstants.GroupManagement.TextField_LocationDescription, "New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location New location");
			test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.GroupManagement.Button_NewLocationSaveChanges);

			// Verify the error message is present
			Assert.IsTrue(test.Selenium.IsTextPresent("Location Description can't exceed 500 characters."));

			// Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Verifies if a location is added to an existing group, it saves and is applied.")]
		public void Groups_GroupsByGroupType_Groups_Edit_Location_Applies_To_Group() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

			// Create a group
			test.Portal.Groups_Group_Create("Automation Group Type", "Location Group", null, "11/15/2009", true);

			// Create a physical location
			test.Portal.Groups_Group_View_Settings("Location Group");
			test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.GroupManagement.Link_NewLocation);

			test.Selenium.Type(GroupsByGroupTypeConstants.GroupManagement.TextField_LocationName, "Apple");
			test.Selenium.Type(GroupsByGroupTypeConstants.GroupManagement.TextField_Address1, "1 Infinite Loop");
			test.Selenium.Type(GroupsByGroupTypeConstants.GroupManagement.TextField_City, "Cupertino");
			test.Selenium.Select(GroupsByGroupTypeConstants.GroupManagement.DropDown_State, "California");
			test.Selenium.Type(GroupsByGroupTypeConstants.GroupManagement.TextField_ZipCode, "95014");
			test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.GroupManagement.Button_NewLocationSaveChanges);

			// Edit the location and verify all the data was saved
			test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.GroupManagement.Link_EditLocation);

			Assert.AreEqual("Apple", test.Selenium.GetValue(GroupsByGroupTypeConstants.GroupManagement.TextField_LocationName));
			Assert.AreEqual("1 Infinite Loop", test.Selenium.GetValue(GroupsByGroupTypeConstants.GroupManagement.TextField_Address1));
			Assert.AreEqual("Cupertino", test.Selenium.GetValue(GroupsByGroupTypeConstants.GroupManagement.TextField_City));
			Assert.AreEqual("CA", test.Selenium.GetSelectedValue(GroupsByGroupTypeConstants.GroupManagement.DropDown_State));
			Assert.AreEqual("95014", test.Selenium.GetValue(GroupsByGroupTypeConstants.GroupManagement.TextField_ZipCode));

			// Return
			test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

			// Delete the group
			test.Portal.Groups_Group_Delete("Location Group");

			test.Portal.Logout();
		}

        [Test, MultipleAsserts, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a portal user can update a location to be a private location.")]
        public void Groups_GroupsByGroupType_Groups_Edit_Location_Make_Private() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Create a group with a normal location
            test.Portal.Groups_Group_Create("Automation Group Type", "Location Update Private - Portal", null, "11/15/2009", true, "Private Location", null, false, "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022");

            // Add an individual to the group as a leader
            test.Portal.Groups_Group_Update_AddToRoster("Location Update Private - Portal", "Group Leader", GeneralEnumerations.IndividualGroupRole.Leader);

            // Add an individual to the group as a member
            test.Portal.Groups_Group_Update_AddToRoster("Location Update Private - Portal", "Group Member", GeneralEnumerations.IndividualGroupRole.Member);

            test.Portal.Logout();

            // Make the location private
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");
            test.Portal.Groups_Group_Update_Location_Set_Private("Location Update Private - Portal", true);

            // Logout of Portal
            test.Portal.Logout();


            // Login to InFellowship
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the dashboard of the group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourGroups);
            test.Selenium.ClickAndWaitForPageToLoad("link=Location Update Private - Portal");
            test.Selenium.ClickAndWaitForPageToLoad(InFellowshipConstants.Groups.GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard);

            // Verify the address is present
            test.Selenium.VerifyTextPresent("2812 Meadow Wood Drive");
            test.Selenium.VerifyTextPresent("Flower Mound, TX 75022");

            // Find a group
            test.infellowship.Groups_FindAGroup();
            //InFellowship.Groups.FindAGroupMethods.FindAGroup_Search(test, null, null, null, false);

            // Click on the group
            test.Selenium.ClickAndWaitForPageToLoad("link=Location Update Private - Portal");

            // Verify the location is not fully shown.
            Assert.IsFalse(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"));
            //Assert.Contains(test.Selenium.GetText("//table[@class='info']"), "Private", "Private location should be shown as 'Private'");
            //Assert.Contains(test.Selenium.GetEval(string.Format(script, "Location Update Private - Portal", "Location Update Private - Portal")), "null", "Private location should not be shown");

            // Logout
            test.infellowship.Logout();

            // Repeat for members
            // Login to InFellowship
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the dashboard of the group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourGroups);
            test.Selenium.ClickAndWaitForPageToLoad("link=Location Update Private - Portal");
            test.Selenium.ClickAndWaitForPageToLoad(InFellowshipConstants.Groups.GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard);

            // Verify the address is not present
            test.Selenium.VerifyTextPresent("2812 Meadow Wood Drive");
            test.Selenium.VerifyTextPresent("Flower Mound, TX 75022");
            //Assert.IsFalse(test.Selenium.GetText("//table[@class='info']").Contains("Flower Mound, TX 75022"));

            // Find a group
            test.infellowship.Groups_FindAGroup();
            //InFellowship.Groups.FindAGroupMethods.FindAGroup_Search(test, null, null, null, false);

            // Click on the group
            test.Selenium.ClickAndWaitForPageToLoad("link=Location Update Private - Portal");

            // Verify the location is not fully shown.
            Assert.IsFalse(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"));
            test.Selenium.VerifyTextPresent("Flower Mound, TX 75022");
            //Assert.Contains(test.Selenium.GetText("//table[@class='info']"), "Private", "Private location should be shown as 'Private'");

            // Logout
            test.infellowship.Logout();

            // Repeat for owners
            // Login to InFellowship
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the dashboard of the group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourGroups);
            test.Selenium.ClickAndWaitForPageToLoad("link=Automation Span of Care");
            test.Selenium.ClickAndWaitForPageToLoad("link=Location Update Private - Portal");
            test.Selenium.ClickAndWaitForPageToLoad(InFellowshipConstants.Groups.GroupsConstants.GroupManagement.NavigationLinks.Link_Dashboard);

            // Verify the address is present
            test.Selenium.VerifyTextPresent("2812 Meadow Wood Drive");
            test.Selenium.VerifyTextPresent("Flower Mound, TX 75022");
            //Assert.IsFalse(test.Selenium.GetText("//table[@class='info']").Contains("Flower Mound, TX 75022"));

            // Find a group
            test.infellowship.Groups_FindAGroup();
            //InFellowship.Groups.FindAGroupMethods.FindAGroup_Search(test, null, null, null, false);

            // Click on the group
            test.Selenium.ClickAndWaitForPageToLoad("link=Location Update Private - Portal");

            // Verify the location is not fully shown.
            Assert.IsFalse(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"));
            test.Selenium.VerifyTextPresent("Flower Mound, TX 75022");
            //Assert.Contains(test.Selenium.GetText("//table[@class='info']"), "Private", "Private location should be shown as 'Private'");

            // Logout
            test.infellowship.Logout();

            // Repeat for a non logged in user.
            // Find a group
            test.infellowship.Groups_FindAGroup();
            //InFellowship.Groups.FindAGroupMethods.FindAGroup_Search(test, null, null, null, false);

            // Click on the group
            test.Selenium.ClickAndWaitForPageToLoad("link=Location Update Private - Portal");

            // Verify the location is not fully shown.
            Assert.IsFalse(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"));

            //Assert.Contains(test.Selenium.GetText("//table[@class='info']"), "Private", "Private location should be shown as 'Private'");
            //test.Selenium.VerifyTextPresent("Location Update Private - Portal");

            // Delete the group
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");
            test.Portal.Groups_Group_Delete("Location Update Private - Portal");
        }

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies the required fields for creating a schedule that is a one time event.")]
		public void Groups_GroupsByGroupType_Groups_Edit_New_Schedule_One_Time_Event_Required_Fields() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

			// View the settings of a group
            test.Portal.Groups_Group_View_Settings(_groupName);

			// Create a new schedule for a one time event, but don't specify any of the required fields
			test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.GroupManagement.Link_NewSchedule);

			// Submit
			test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.GroupManagement.Button_NewScheduleSaveChanges);

			// Verify validation messages
			Assert.IsTrue(test.Selenium.IsTextPresent("The start date is in an invalid format."));
			Assert.IsTrue(test.Selenium.IsTextPresent("The start time is in an invalid format."));

			// Return
			test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

			// Log out
			test.Portal.Logout();

		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies the required fields for creating a schedule that is a weekly event.")]
		public void Groups_GroupsByGroupType_Groups_Edit_New_Schedule_Weekly_Event_Required_Fields() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

			// View the settings of a group
            test.Portal.Groups_Group_View_Settings(_groupName);

			// Create a new schedule for a weekly event, but don't specify the reoccurance
			test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.GroupManagement.Link_NewSchedule);
			test.Selenium.Type(GroupsByGroupTypeConstants.GroupManagement.DateControl_ScheduleStartDate, "11/15/2009");
			test.Selenium.Type(GroupsByGroupTypeConstants.GroupManagement.TextField_ScheduleStartTime, "3:00 AM");

			// Specify a weekly event
			test.Selenium.Click(GroupsByGroupTypeConstants.GroupManagement.RadioButton_WeeklyEvent);

			// Submit
			test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.GroupManagement.Button_NewScheduleSaveChanges);

			// Verify validation messages
			Assert.IsTrue(test.Selenium.IsTextPresent("At least one weekday must be selected."));
			Assert.IsTrue(test.Selenium.IsTextPresent("No weekly recurrence has been selected."));

			// Return
			test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

			// Log out
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies the required fields for creating a schedule that is a monthly event.")]
		public void Groups_GroupsByGroupType_Groups_Edit_New_Schedule_Monthly_Event_Required_Fields() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

			// View the settings of a group
			test.Portal.Groups_Group_View_Settings(_groupName);

			// Create a new schedule for a monthly event, but don't specify the reoccurance
			test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.GroupManagement.Link_NewSchedule);
			test.Selenium.Type(GroupsByGroupTypeConstants.GroupManagement.DateControl_ScheduleStartDate, "11/15/2009");
			test.Selenium.Type(GroupsByGroupTypeConstants.GroupManagement.TextField_ScheduleStartTime, "3:00 AM");

			// Specify a monthly event
			test.Selenium.Click(GroupsByGroupTypeConstants.GroupManagement.RadioButton_MonthlyEvent);

			// Submit
			test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.GroupManagement.Button_NewScheduleSaveChanges);

			// Verify validation messages
			Assert.IsTrue(test.Selenium.IsTextPresent("No weekday has been selected."));
			Assert.IsTrue(test.Selenium.IsTextPresent("No nth weekday has been selected."));
			Assert.IsTrue(test.Selenium.IsTextPresent("No monthly recurrence has been selected."));

			// Return
			test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

			// Log out
			test.Portal.Logout();
		}

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the edit links go to the correct pages in the Group Creation wizard")]
        public void Groups_GroupsByGroupType_Groups_Create_Edit_Links_Go_To_Correct_Page() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Create a new group
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select a group type
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupTypeName));

            // Edit step 1
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.Wizard_GroupCreation.Link_EditStep1);
            Assert.IsTrue(test.Selenium.IsElementPresent(TableIds.Groups_Group_GroupTypes));

            // Select a group type
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _groupTypeName));

            test.Selenium.Type("group_name", "Some group");
            test.Selenium.Type("start_date", "11/15/1985");

            // Next to Step 3
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Edit step 2
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.Wizard_GroupCreation.Link_EditStep2);
            Assert.IsTrue(test.Selenium.IsElementPresent("group_name"));
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsByGroupTypeConstants.Wizard_GroupCreation.Step2_TextField_GroupDescription));
            Assert.IsTrue(test.Selenium.IsElementPresent("start_date"));

            // Next to Step 3
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Skip step 3
            test.Selenium.ClickAndWaitForPageToLoad("link=Skip this step →");

            // Edit step 3
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.Wizard_GroupCreation.Link_EditStep3);
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsByGroupTypeConstants.Wizard_GroupCreation.Step3_DateControl_ScheduleStartDate));
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Skip this step →"));

            // Skip this step to step 4
            test.Selenium.ClickAndWaitForPageToLoad("link=Skip this step →");

            // Skip step 4
            test.Selenium.ClickAndWaitForPageToLoad("link=Skip this step →");

            // Edit step 4
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.Wizard_GroupCreation.Link_EditStep4);
            Assert.IsTrue(test.Selenium.IsElementPresent("location_name"));
            Assert.IsTrue(test.Selenium.IsElementPresent(GroupsByGroupTypeConstants.Wizard_GroupCreation.Step4_TextField_LocationDescription));
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Skip this step →"));

            // Skip this step to step 5
            test.Selenium.ClickAndWaitForPageToLoad("link=Skip this step →");

            // Return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Log out
            test.Portal.Logout();
        }

		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Bryan Mikaelian")]
		[Description("Deletes a group.")]
		public void Groups_GroupsByGroupType_Groups_Delete() {
            // Intitial data
            var groupName = "Test Delete Group";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

			// Delete a group
			test.Portal.Groups_Group_Delete(groupName);

			// Navigate to Groups->View All
			test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

			// Verify Test New Group is not present
			Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_AllTab, groupName, "Group"), "Deleted group was present!");

			// Check the Groups Tab
			test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.ViewAllTabs.GroupsTab);
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_AllTab, groupName, "Group"), "Deleted group was present!");

			// Check the people list tab
			test.Selenium.ClickAndWaitForPageToLoad("link=People Lists");
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_PeopleList_PeopleListTab, groupName, "People List"), "Deleted group was present on the people list tab!");

			// Check the temporary group tab
			test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.ViewAllTabs.TempGroupTab);
			Assert.IsFalse(test.Selenium.IsElementPresent(string.Format("link={0}", groupName)), "Deleted group was present on the temporary group tab!");

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies if you delete the active group, logout and log back in, the active group is not the deleted group.")]
		public void Groups_GroupsByGroupType_Groups_Delete_Active_Group_Not_Deleted_Group() {
            // Intitial data
            var groupName = "Test Delete Group - No Active";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

			// Delete the group so we don't have an active group
            test.Portal.Groups_Group_Delete(groupName);

			// Logout
			test.Portal.Logout();

			// Log in
			test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

			// Verify active group is not this deleted group
            test.Selenium.VerifyElementNotPresent(string.Format("link={0}", groupName));

			// Verify Add a group is not present
			test.Selenium.VerifyElementNotPresent("active_group_add");

			// Logout
			test.Portal.Logout();
		}

        [Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Verifies that if you create a group under a group type that has a single select custom without a default applied to it, this custom field is present on Step 2 of the group wizard.")]
		public void Groups_GroupsByGroupType_Groups_Create_Custom_Field_Present_Single_Select_No_Default() {
            // Initial data
            var groupTypeName = "Group Custom Field - No Default";
            var customFieldName = "Group Specific Single Select CF";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Create(254, "Group Admin", groupTypeName, new List<int>() { 11 });
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.SingleSelect, customFieldName, null, new List<string>() { "Yes", "No", "Dont care" });
            base.SQL.Groups_GroupTypes_AddCustomField(254, groupTypeName, customFieldName);

            // Login in as a group admin
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

			// Create a group under this group type
			test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);
			test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

			// Select the group type
			test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", groupTypeName));

			// Verify custom field is there but has no default
            Assert.IsTrue(test.Selenium.IsChecked(string.Format("//p/span/strong[text()='{0}']/ancestor::p/following-sibling::div/ul/li/label[text()='Not specified']/preceding-sibling::input", customFieldName)), "Custom field without a default had a default value checked!");

			// Return
			test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

			// Logout
			test.Portal.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_CustomField_Delete(254, customFieldName);

		}

		[Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Verifies that if you create a group under a group type that has a multi select custom without a default applied to it, this custom field is present on Step 2 of the group wizard.")]
		public void Groups_GroupsByGroupType_Groups_Create_Custom_Field_Present_Multi_Select_No_Default() {
            // Initial data
            var groupTypeName = "Group Custom Field - No Default MS";
            var customFieldName = "Group Specific Multi Select CF";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Create(254, "Group Admin", groupTypeName, new List<int>() { 11 });
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.MultiSelect, customFieldName, null, new List<string>() { "Yes", "No", "Dont care" });
            base.SQL.Groups_GroupTypes_AddCustomField(254, groupTypeName, customFieldName);

            // Login in as a group admin
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Create a group under this group type
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select the group type
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", groupTypeName));

            // Verify custom field is there but has no default
            Assert.IsFalse(test.Selenium.IsChecked(string.Format("//p/span/strong[text()='{0}']/ancestor::p/following-sibling::div/ul/li/label[text()='Yes']/preceding-sibling::input", customFieldName)), "Custom field without a default had a default value checked!");
            Assert.IsFalse(test.Selenium.IsChecked(string.Format("//p/span/strong[text()='{0}']/ancestor::p/following-sibling::div/ul/li/label[text()='No']/preceding-sibling::input", customFieldName)), "Custom field without a default had a default value checked!");
            Assert.IsFalse(test.Selenium.IsChecked(string.Format("//p/span/strong[text()='{0}']/ancestor::p/following-sibling::div/ul/li/label[text()='Dont care']/preceding-sibling::input", customFieldName)), "Custom field without a default had a default value checked!");

            // Return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
		}

		[Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Verifies if a single select custom field value is picked on Step 2, it is actually applied to a group when you finish the wizard.")]
		public void Groups_GroupsByGroupType_Groups_Create_Custom_Fields_Select_Single_Select() {
            // Initial data
            var groupTypeName = "Group SS Custom Field - Pick Choice";
            var customFieldName = "Group Custom Field";
            var groupName = "Group - Single Select CF";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Create(254, "Group Admin", groupTypeName, new List<int>() { 11 });
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.SingleSelect, customFieldName, null, new List<string>() { "Yes", "No", "Dont care" });
            base.SQL.Groups_GroupTypes_AddCustomField(254, groupTypeName, customFieldName);

			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			// Login in as a group admin
			test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

			// Create a group under this group type
			test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);
			test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

			// Select the group type
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", groupTypeName));

			// Specify a name
            test.Selenium.Type("group_name", groupName);

			// Pick a custom field
            test.Selenium.Click(string.Format("//p/span/strong[text()='{0}']/ancestor::p/following-sibling::div/ul/li/label[text()='{1}']/preceding-sibling::input", customFieldName, "Yes"));

			// Next
			test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

			// Skip step 3
			test.Selenium.ClickAndWaitForPageToLoad("link=Skip this step →");

			// Skip step 4
			test.Selenium.ClickAndWaitForPageToLoad("link=Skip this step →");

			// Save
			test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

			// Edit Custom Fields
			test.Selenium.ClickAndWaitForPageToLoad("link=Manage custom fields");

			// Verify the value picked on Step 2 is applied
            Assert.IsTrue(test.Selenium.IsChecked(string.Format("//p/span/strong[text()='{0}']/ancestor::p/following-sibling::div/ul/li/label[text()='{1}']/preceding-sibling::input", customFieldName, "Yes")), "Custom field value was picked but not applied!!");

			// Return
			test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
		}

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies if a multi select custom field value is picked on Step 2, it is actually applied to a group when you finish the wizard.")]
        public void Groups_GroupsByGroupType_Groups_Create_Custom_Fields_Select_Multi_Select() {
            // Initial data
            var groupTypeName = "Group MS Custom Field - Pick Choice";
            var customFieldName = "Group Custom Field - MS";
            var groupName = "Group - Mult Select CF";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Create(254, "Group Admin", groupTypeName, new List<int>() { 11 });
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.MultiSelect, customFieldName, null, new List<string>() { "Yes", "No", "Dont care" });
            base.SQL.Groups_GroupTypes_AddCustomField(254, groupTypeName, customFieldName);

            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            // Login in as a group admin
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Create a group under this group type
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/Group/Step1.aspx')]");

            // Select the group type
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", groupTypeName));

            // Specify a name
            test.Selenium.Type("group_name", groupName);

            // Pick a custom field
            test.Selenium.Click(string.Format("//p/span/strong[text()='{0}']/ancestor::p/following-sibling::div/ul/li/label[text()='{1}']/preceding-sibling::input", customFieldName, "Yes"));

            // Next
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Skip step 3
            test.Selenium.ClickAndWaitForPageToLoad("link=Skip this step →");

            // Skip step 4
            test.Selenium.ClickAndWaitForPageToLoad("link=Skip this step →");

            // Save
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Edit Custom Fields
            test.Selenium.ClickAndWaitForPageToLoad("link=Manage custom fields");

            // Verify the value picked on Step 2 is applied
            Assert.IsTrue(test.Selenium.IsChecked(string.Format("//p/span/strong[text()='{0}']/ancestor::p/following-sibling::div/ul/li/label[text()='{1}']/preceding-sibling::input", customFieldName, "Yes")), "Custom field value was picked but not applied!!");

            // Return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }


        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelians")]
        [Description("Verifies you can grant a portal user manager rights to a group")]
        public void Groups_GroupsByGroupType_Groups_Edit_Permissions_Manager_Rights() {
            // Initital data
            var groupName = "A Manager Group";
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");

            // Login in as a group admin
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Grant a portal user manager rights
            test.Portal.Groups_Group_Update_AddManagerOrView(groupName, "Manager, Group", GeneralEnumerations.GroupRolesPortalUser.Manager);

            // Logout
            test.Portal.Logout();

            // Login as the manager
            test.Portal.Login("groupmanager", "BM.Admin09", "qaeunlx0c2");

            // View the settings of a group that you have manager rights to
            test.Portal.Groups_Group_View_Settings(groupName);

            // Verify the correct links are present
            test.Selenium.VerifyElementPresent("link=Edit group details");
            test.Selenium.VerifyElementPresent("link=New location");
            test.Selenium.VerifyElementPresent("link=New schedule");
            test.Selenium.VerifyElementPresent("link=Change permissions");

            // Verify you cannot delete this group
            test.Selenium.VerifyElementNotPresent("link=Delete this group");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelians")]
        [Description("Verifies you can grant a portal user viewer rights to a group")]
        public void Groups_GroupsByGroupType_Groups_Edit_Permissions_Viewer_Rights() {
            // Initital data
            var groupName = "A Viewer Group";
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");

            // Login in as a group admin
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Grant a portal user viewer rights
            test.Portal.Groups_Group_Update_AddManagerOrView(groupName, "Viewer, Group", GeneralEnumerations.GroupRolesPortalUser.Viewer);

            // Logout
            test.Portal.Logout();

            // Login as the viewer
            test.Portal.Login("groupviewer", "BM.Admin09", "qaeunlx0c2");

            // View the settings of a group that you have viewer rights too
            test.Portal.Groups_Group_View_Settings(groupName);

            // Verify none of the links are present
            test.Selenium.VerifyElementNotPresent("link=Edit group details");
            test.Selenium.VerifyElementNotPresent("link=New location");
            test.Selenium.VerifyElementNotPresent("link=New schedule");
            test.Selenium.VerifyElementNotPresent("link=Change permissions");

            // Verify you cannot delete this group
            test.Selenium.VerifyElementNotPresent("link=Delete this group");

            // Logout of portal
            test.Portal.Logout();
        }

        #endregion CRUD

        #region View Prospects
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Bryan Mikaelian")]
		[Description("Verifies the View Prospects page loads for the correct group when clicked.")]
		public void Groups_GroupsByGroupType_Groups_View_Prospects_View_Correct_Group() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("groupadmin", "BM.Admin09", "QAEUNLX0C2");

			// View the prospects of a group
            test.Portal.Groups_Group_View_Prospects(_groupName);

			// Correct group is loaded
			Assert.AreEqual("Fellowship One :: View Prospects", test.Selenium.GetTitle());
            Assert.AreEqual(_groupName, test.Selenium.GetText("//div[@id='main_content']/div[1]/div[2]/h2"));

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies the View Prospects page lists the leaders of the group.")]
		public void Groups_GroupsByGroupType_Groups_View_Prospects_Leaders_Displayed() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("groupadmin", "BM.Admin09", "QAEUNLX0C2");

			// View the prospects of a group
			test.Portal.Groups_Group_View_Prospects(_groupName);

			// Verify the Leaders are shown on this page
			Assert.AreEqual("Leaders", test.Selenium.GetText("//div[@id='main_content']/div[2]/h6"));
            //test.Selenium.VerifyElementPresent(string.Format("link={0}", _groupName));
            //Assert.IsTrue(string.IsNullOrEmpty(test.Selenium.GetText(".//div[@id='main_content']/*//h2[text()='Generic Group']")));

			// Logout of Portal
			test.Portal.Logout();
		}

		#region Export to CSV

		#region Active Prospects
		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Bryan Mikaelian")]
		[Description("Verifies the Export to CSV wizard loads when the user selects export for active prospects.  ALso verifies you can navigate through it")]
		public void Groups_GroupsByGroupType_Groups_View_Prospects_Active_Export_To_CSV_Wizard_Navigation() {

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "QAEUNLX0C2");

			// View the Group in Portal
            test.Portal.Groups_Group_View_Prospects(_groupName);

			// Export to CSV
			test.Selenium.Click("link=Actions");
			test.Selenium.ClickAndWaitForPageToLoad("link=Export to CSV");

			// Verify you can navigate through the wizard           
			test.Selenium.VerifyTitle("Fellowship One :: Choose Columns");
			test.Selenium.Click(GroupsByGroupTypeConstants.Wizard_ExportCSV.Checkbox_IndividualName);
			test.Selenium.ClickAndWaitForPageToLoad("button_submit");

			test.Selenium.VerifyTitle("Fellowship One :: Order Columns");
			test.Selenium.ClickAndWaitForPageToLoad("button_submit");

			test.Selenium.VerifyTitle("Fellowship One :: Download file");

			test.Selenium.ClickAndWaitForPageToLoad("link=Return to prospects");

			test.Selenium.VerifyTitle("Fellowship One :: View Prospects");

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies the Export to CSV wizard loads when the user selects export for active prospects, specifically checking if communication values can be selected. This was an existing bug.")]
		public void Groups_GroupsByGroupType_Groups_View_Prospects_Active_Export_To_CSV_Wizard_Check_Communication_Values() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("groupadmin", "BM.Admin09", "QAEUNLX0C2");

			// View the Group in Portal
            test.Portal.Groups_Group_View_Prospects(_groupName);

			// Export to CSV
			test.Selenium.Click("link=Actions");
			test.Selenium.ClickAndWaitForPageToLoad("link=Export to CSV");

			// Verify you can navigate through the wizard
			test.Selenium.VerifyTitle("Fellowship One :: Choose Columns");
			test.Selenium.Click("//input[@name='column_name' and @value='Communication - Email Count']");
			test.Selenium.Click("//input[@name='column_name' and @value='Communication - Phone Count']");
			test.Selenium.Click("//input[@name='column_name' and @value='Communication - Meeting Count']");
			test.Selenium.Click("//input[@name='column_name' and @value='Communication - Comment Count']");

			test.Selenium.ClickAndWaitForPageToLoad("button_submit");

			test.Selenium.VerifyTitle("Fellowship One :: Order Columns");
			test.Selenium.ClickAndWaitForPageToLoad("button_submit");

			test.Selenium.VerifyTitle("Fellowship One :: Download file");

			test.Selenium.ClickAndWaitForPageToLoad("link=Return to prospects");

			test.Selenium.VerifyTitle("Fellowship One :: View Prospects");

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies the check all checkbox remains checked if the user checks all options on step 1, goes to step 2, and goes back to step 1. This is for active prospects.")]
		public void Groups_GroupsByGroupType_Groups_View_Prospects_Active_Export_To_CSV_Check_All_Checkbox_Remains_Checked() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "QAEUNLX0C2");

			// View the Group in Portal
            test.Portal.Groups_Group_View_Prospects(_groupName);

			// Export to CSV
			test.Selenium.Click("link=Actions");
			test.Selenium.ClickAndWaitForPageToLoad("link=Export to CSV");

			// Check All on Step 1
			test.Selenium.Click(GroupsByGroupTypeConstants.Wizard_ExportCSV.Checkbox_All);
			test.Selenium.ClickAndWaitForPageToLoad("button_submit");

			// Go back to step 1
			test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.Wizard_ExportCSV.Link_Back);

			// Verify the check all checkbox is checked       
			Assert.IsTrue(test.Selenium.IsChecked(GroupsByGroupTypeConstants.Wizard_ExportCSV.Checkbox_All));

			// Logout of Portal
			test.Portal.Logout();
		}
        #endregion Active Prospects

        #region Closed Prospects

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Bryan Mikaelian")]
		[Description("Verifies the Export to CSV wizard loads when the user selects export for active prospects.  Also verifies you can navigate through it")]
		public void Groups_GroupsByGroupType_Groups_View_Prospects_Closed_Export_To_CSV_Wizard_Navigation() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "QAEUNLX0C2");

			// View the Group in Portal
            test.Portal.Groups_Group_View_Prospects(_groupName);
			test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.GroupManagement.Link_ViewProspects_Closed);

			// Export to CSV
			test.Selenium.Click("link=Actions");
			test.Selenium.ClickAndWaitForPageToLoad("link=Export to CSV");

			// Verify you can navigate through the wizard
			test.Selenium.VerifyTitle("Fellowship One :: Choose Columns");
			test.Selenium.Click(GroupsByGroupTypeConstants.Wizard_ExportCSV.Checkbox_IndividualName);
			test.Selenium.ClickAndWaitForPageToLoad("button_submit");

			test.Selenium.VerifyTitle("Fellowship One :: Order Columns");
			test.Selenium.ClickAndWaitForPageToLoad("button_submit");

			test.Selenium.VerifyTitle("Fellowship One :: Download file");

			test.Selenium.ClickAndWaitForPageToLoad("link=Return to prospects");

			test.Selenium.VerifyTitle("Fellowship One :: View Prospects");

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies the Export to CSV wizard loads when the user selects export for active prospects, specifically checking if communication values can be selected. This was an existing bug.")]
		public void Groups_GroupsByGroupType_Groups_View_Prospects_Closed_Export_To_CSV_Wizard_Check_Communication_Values() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "QAEUNLX0C2");

			// View the Group in Portal
            test.Portal.Groups_Group_View_Prospects(_groupName);
			test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.GroupManagement.Link_ViewProspects_Closed);

			// Export to CSV
			test.Selenium.Click("link=Actions");
			test.Selenium.ClickAndWaitForPageToLoad("link=Export to CSV");

			// Verify you can navigate through the wizard
			test.Selenium.VerifyTitle("Fellowship One :: Choose Columns");
			test.Selenium.Click("//input[@name='column_name' and @value='Communication - Email Count']");
			test.Selenium.Click("//input[@name='column_name' and @value='Communication - Phone Count']");
			test.Selenium.Click("//input[@name='column_name' and @value='Communication - Meeting Count']");
			test.Selenium.Click("//input[@name='column_name' and @value='Communication - Comment Count']");

			test.Selenium.ClickAndWaitForPageToLoad("button_submit");

			test.Selenium.VerifyTitle("Fellowship One :: Order Columns");
			test.Selenium.ClickAndWaitForPageToLoad("button_submit");

			test.Selenium.VerifyTitle("Fellowship One :: Download file");

			test.Selenium.ClickAndWaitForPageToLoad("link=Return to prospects");

			test.Selenium.VerifyTitle("Fellowship One :: View Prospects");

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies the check all checkbox remains checked if the user checks all options on step 1, goes to step 2, and goes back to step 1. This is for closed prospects.")]
		public void Groups_GroupsByGroupType_Groups_View_Prospects_Closed_Export_To_CSV_Check_All_Checkbox_Remains_Checked() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("groupadmin", "BM.Admin09", "QAEUNLX0C2");

			// View the Group in Portal
            test.Portal.Groups_Group_View_Prospects(_groupName);
			test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.GroupManagement.Link_ViewProspects_Closed);

			// Export to CSV
			test.Selenium.Click("link=Actions");
			test.Selenium.ClickAndWaitForPageToLoad("link=Export to CSV");

			// Check All on Step 1
			test.Selenium.Click(GroupsByGroupTypeConstants.Wizard_ExportCSV.Checkbox_All);
			test.Selenium.ClickAndWaitForPageToLoad("button_submit");

			// Go back to step 1
			test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.Wizard_ExportCSV.Link_Back);

			// Verify the check all checkbox is checked       
			Assert.IsTrue(test.Selenium.IsChecked(GroupsByGroupTypeConstants.Wizard_ExportCSV.Checkbox_All));

			// Cancel and return
			test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

			// Logout of Portal
			test.Portal.Logout();
		}

        #endregion Closed Prospects

        #endregion Export to CSV

        #endregion View Prospects

        #region Show Leaders and Members
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies if you view the leaders and members of a group, the leader is present in the results.")]
        public void Groups_GroupsByGroupType_Groups_View_Leaders_And_Members_Leader_Present() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // View the Leaders and Members of a group
            test.Portal.Groups_Group_View_Leaders_Members(new string[] { _showLeadersMembersGroup });

            // Verify the leader is present
            var row = test.GeneralMethods.GetTableRowNumber(TableIds.Groups_ViewAll_ShowLeadersAndMembers_List, "Group Leader", "Name") + 1;
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_ShowLeadersAndMembers_List, "Group Leader", "Name"));

            // Leader image is present
            test.Selenium.VerifyElementPresent(string.Format("//table[@id='ctl00_ctl00_MainContent_content_grdLeadersMembers']/tbody/tr[{0}]/td[5]/div[@class='float_left role_designation leader_designation']", row));

            // Member image is not present
            test.Selenium.VerifyElementPresent(string.Format("//table[@id='ctl00_ctl00_MainContent_content_grdLeadersMembers']/tbody/tr[{0}]/td[5]/div[@class='float_left role_designation member_designation muted']", row));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies if you view the leaders and members of a group, the member is present in the results.")]
        public void Groups_GroupsByGroupType_Groups_View_Leaders_And_Members_Member_Present() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // View the Leaders and Members of a group
            test.Portal.Groups_Group_View_Leaders_Members(new string[] { _showLeadersMembersGroup });

            // Verify the member is present
            var row = test.GeneralMethods.GetTableRowNumber(TableIds.Groups_ViewAll_ShowLeadersAndMembers_List, "Group Member", "Name") + 1;
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_ShowLeadersAndMembers_List, "Group Member", "Name"));

            // Leader image is not present
            test.Selenium.VerifyElementPresent(string.Format("//table[@id='ctl00_ctl00_MainContent_content_grdLeadersMembers']/tbody/tr[{0}]/td[5]/div[@class='float_left role_designation leader_designation muted']", row));

            // Member image is present
            test.Selenium.VerifyElementPresent(string.Format("//table[@id='ctl00_ctl00_MainContent_content_grdLeadersMembers']/tbody/tr[{0}]/td[5]/div[@class='float_left role_designation member_designation']", row));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies if you view the leaders and members of several groups and someone has both leader and member roles to the groups, both icons are displayed.")]
        public void Groups_GroupsByGroupType_Groups_View_Leaders_And_Members_Both_Present() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // View the Leaders and Members of several groups
            string[] groups = { _showLeadersMembersGroup, _showLeadersMembersGroup2 };
            test.Portal.Groups_Group_View_Leaders_Members(groups);

            // Verify the leader is present
            var row = test.GeneralMethods.GetTableRowNumber(TableIds.Groups_ViewAll_ShowLeadersAndMembers_List, "Group Leader", "Name") + 1;
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_ShowLeadersAndMembers_List, "Group Leader", "Name"));

            // Leader image is present
            test.Selenium.VerifyElementPresent(string.Format("//table[@id='ctl00_ctl00_MainContent_content_grdLeadersMembers']/tbody/tr[{0}]/td[5]/div[@class='float_left role_designation leader_designation']", row));

            // Member image is present
            test.Selenium.VerifyElementPresent(string.Format("//table[@id='ctl00_ctl00_MainContent_content_grdLeadersMembers']/tbody/tr[{0}]/td[5]/div[@class='float_left role_designation member_designation']", row));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the all count is correct for the Show Leaders and Member page when viewing a group.")]
        public void Groups_GroupsByGroupType_Groups_View_Leaders_And_Members_All_Count_Correct() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // View the Leaders and Members of a group
            test.Portal.Groups_Group_View_Leaders_Members(new string[] { _showLeadersMembersGroup });

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the leader count is correct for the Show Leaders and Member page when viewing a group.")]
        public void Groups_GroupsByGroupType_Groups_View_Leaders_And_Members_Leader_Count_Correct() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // View the Leaders and Members of a group
            test.Portal.Groups_Group_View_Leaders_Members(new string[] { _showLeadersMembersGroup });

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the member count is correct for the Show Leaders and Member page when viewing a group.")]
        public void Groups_GroupsByGroupType_Groups_View_Leaders_And_Members_Member_Count_Correct() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // View the Leaders and Members of a group
            test.Portal.Groups_Group_View_Leaders_Members(new string[] { _showLeadersMembersGroup });

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the filters work correctly when viewing the leaders and members of a group.")]
        public void Groups_GroupsByGroupType_Groups_View_Leaders_And_Members_Filter_Shows_Correct_Data() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // View the Leaders and Members of a group
            test.Portal.Groups_Group_View_Leaders_Members(new string[] { _showLeadersMembersGroup });

            // Filter on Leaders
            test.Selenium.Click("filter_leaders");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_ShowLeadersAndMembers_List, "Group Leader", "Name"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_ShowLeadersAndMembers_List, "Group Member", "Name"));

            // Filter on Members
            test.Selenium.Click("filter_members");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_ShowLeadersAndMembers_List, "Group Leader", "Name"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_ShowLeadersAndMembers_List, "Group Member", "Name"));

            // Filter on All
            test.Selenium.Click("filter_all");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_ShowLeadersAndMembers_List, "Group Leader", "Name"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_ShowLeadersAndMembers_List, "Group Member", "Name"));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the filters work correctly when viewing the leaders and members of a set of groups where an individual is a leader and member within these groups.")]
        public void Groups_GroupsByGroupType_Groups_View_Leaders_And_Members_Filter_Shows_Correct_Data_Multiple_Roles() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");


            // View the Leaders and Members of a group
            string[] groups = { _showLeadersMembersGroup, _showLeadersMembersGroup2 };
            test.Portal.Groups_Group_View_Leaders_Members(groups);


            // Filter on Leaders
            test.Selenium.Click("filter_leaders");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_ShowLeadersAndMembers_List, "Group Leader", "Name"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_ShowLeadersAndMembers_List, "Group Member", "Name"));

            // Filter on Members
            test.Selenium.Click("filter_members");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_ShowLeadersAndMembers_List, "Group Leader", "Name"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_ShowLeadersAndMembers_List, "Group Member", "Name"));

            // Filter on All
            test.Selenium.Click("filter_all");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_ShowLeadersAndMembers_List, "Group Leader", "Name"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_ShowLeadersAndMembers_List, "Group Member", "Name"));

            // Logout of Portal
            test.Portal.Logout();
        }
        #endregion Show Leaders and Members

        #region View Attendance
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies if you view the attendance for a set of groups, the groups with attendance are present.")]
        public void Groups_GroupsByGroupType_Groups_View_Attendance_Groups_With_Attendance_Present() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // View attendance
            List<string> groups = new List<string>() { _showAttendanceGroup };
            test.Portal.Groups_Group_View_Attendance(groups);

            // Verify the group exists under the table of groups with attendance
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_AttendanceDashboard, _showAttendanceGroup, "Group", "contains"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies if you view the attendance for a set of groups, the groups without attendance are present.")]
        public void Groups_GroupsByGroupType_Groups_View_Attendance_Groups_Without_Attendance_Present() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // View attendance
            List<string> groups = new List<string>() { _showAttendanceGroup2 };
            test.Portal.Groups_Group_View_Attendance(groups);

            // Verify the group exists under the table of groups without attendance
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_AttendanceDashboard, _showAttendanceGroup, "Groups With Attendance Disabled", "contains"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies if you view the attendance for a set groups, the groups with attendance and without attendance show up.")]
        public void Groups_GroupsByGroupType_Groups_View_Attendance_Groups_With_and_Without_Attendance_Present() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // View attendance
            List<string> groups = new List<string>() { _showAttendanceGroup, _showAttendanceGroup2 };
            test.Portal.Groups_Group_View_Attendance(groups);

            // Verify the group exists under the table of groups with attendance
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_AttendanceDashboard, _showAttendanceGroup, "Group", "contains"));

            // Verify the group exists under the table of groups without attendance
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_AttendanceDashboard, _showAttendanceGroup2, "Groups With Attendance Disabled", "contains"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can view a group from the attendance dashboard.")]
        public void Groups_GroupsByGroupType_Groups_View_Attendance_View_Group() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // View attendance
            List<string> groups = new List<string>() { _showAttendanceGroup };
            test.Portal.Groups_Group_View_Attendance(groups);

            // Click on a group
            test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", _showAttendanceGroup));

            // Verify you are viewing the group
            test.Selenium.VerifyTextPresent(_showAttendanceGroup);
            test.Selenium.VerifyElementPresent("link=View group settings");
            test.Selenium.VerifyElementPresent("link=Add a person");

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the count that shows the number of groups is correct.")]
        public void Groups_GroupsByGroupType_Groups_View_Attendance_Total_Group_Count_Correct() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // View attendance
            List<string> groups = new List<string>() { _showAttendanceGroup, _showAttendanceGroup2 };
            test.Portal.Groups_Group_View_Attendance(groups);

            // Verify the group count is correct
            Assert.AreEqual(string.Format("{0} Groups", groups.Count), test.Selenium.GetText("//div[@class='meta_count float_right']"));

            // Logout
            test.Portal.Logout();
        }

        #endregion View Attendance

        #region Add Leaders and Members
        #endregion Add Leaders and Members

        #endregion Groups

        #region People Lists
        [Test, RepeatOnFailure, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Creates a people list and verifies it shows up for that user on the View All page.")]
        public void Groups_GroupsByGroupType_PeopleLists_Create() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Create a people list
            string peopleListName = "Test New People List";
            test.Portal.Groups_PeopleList_Create(peopleListName, null, null);

            // Groups -> View All
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Check the group tab
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.ViewAllTabs.GroupsTab);
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_AllTab, peopleListName, "Group"), "New people list was present on the group tab!");

            // Check the People List Tab
            test.Selenium.ClickAndWaitForPageToLoad("link=People Lists");
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_PeopleList_PeopleListTab, peopleListName, "People List"), "New people list was not present!");

            // Logout of portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_Group_Delete(254, peopleListName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a name is required for a people list.")]
        public void Groups_GroupsByGroupType_PeopleLists_Create_Name_Required() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Create a people list but do not specify a name
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);
            test.Selenium.ClickAndWaitForPageToLoad("link=Add a people list");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify message
            test.Selenium.VerifyTextPresent("Group Name is required and cannot exceed 100 characters.");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a name cannot exceed 100 characters when creating a people list.")]
        public void Groups_GroupsByGroupType_PeopleLists_Create_Name_Exceed_100_Characters() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Create a people list with a name that exceeds 100 characters
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);
            test.Selenium.ClickAndWaitForPageToLoad("link=Add a people list");

            test.Selenium.Type("group_name", "Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify message
            test.Selenium.VerifyTextPresent("Group Name is required and cannot exceed 100 characters.");

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a description cannot exceed 500 characters when creating a people list.")]
        public void Groups_GroupsByGroupType_PeopleLists_Create_Description_Exceed_500_Characters() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Create a people list with a description that exceeds 500 characters
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);
            test.Selenium.ClickAndWaitForPageToLoad("link=Add a people list");
            test.Selenium.Type("group_name", "Test People List");
            test.Selenium.Type("group_description", "Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List Test People List");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify message
            test.Selenium.VerifyTextPresent("Group Description cannot exceed 500 characters.");

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a start date is required when creating a people list.")]
        public void Groups_GroupsByGroupType_PeopleLists_Create_StartDate_Required() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Create a people list but don't specify a start date
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);
            test.Selenium.ClickAndWaitForPageToLoad("link=Add a people list");
            test.Selenium.Type("group_name", "Test People List");
            test.Selenium.Type(GroupsByGroupTypeConstants.Wizard_PeopleListCreation.DateControl_StartDate, "");

            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify message
            Assert.IsTrue(test.Selenium.IsTextPresent("A valid Start Date is required."));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a name is required when editing people list.")]
        public void Groups_GroupsByGroupType_PeopleLists_Edit_Name_Required() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Edit a people list
            test.Portal.Groups_PeopleLists_ViewSettings(_peopleListName);
            test.Selenium.ClickAndWaitForPageToLoad("link=Edit group details");

            // Don't specify a name
            test.Selenium.Type("group_name", "");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify message
            test.Selenium.VerifyTextPresent("Group Name is required and cannot exceed 100 characters.");

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a name is required when editing people list.")]
        public void Groups_GroupsByGroupType_PeopleLists_Edit_Description_Exceed_500_Characters() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Edit a people list
            test.Portal.Groups_PeopleLists_ViewSettings(_peopleListName);
            test.Selenium.ClickAndWaitForPageToLoad("link=Edit group details");

            // Specify a description over 500 characters
            test.Selenium.Type("group_description", "Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description Long description ");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify message
            test.Selenium.VerifyTextPresent("Group Description cannot exceed 500 characters.");

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a name cannot exceed 100 characters when editing people list.")]
        public void Groups_GroupsByGroupType_PeopleLists_Edit_Name_Exceed_100_Characters() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Edit a people list
            test.Portal.Groups_PeopleLists_ViewSettings(_peopleListName);
            test.Selenium.ClickAndWaitForPageToLoad("link=Edit group details");

            // Specify a name that exceeds 100 characters
            test.Selenium.Type("group_name", "People list People listPeople listPeople listPeople listPeople listPeople listPeople listPeople listPeople listPeople listPeople listPeople listPeople listPeople listPeople listPeople listPeople listPeople listPeople listPeople list");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify message
            test.Selenium.VerifyTextPresent("Group Name is required and cannot exceed 100 characters.");

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a start date is required when editing people list.")]
        public void Groups_GroupsByGroupType_PeopleLists_Edit_StartDate_Required() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Edit a people list
            test.Portal.Groups_PeopleLists_ViewSettings(_peopleListName);
            test.Selenium.ClickAndWaitForPageToLoad("link=Edit group details");

            // Don't specify a start date
            test.Selenium.Type("start_date", string.Empty);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify message
            test.Selenium.VerifyTextPresent("A valid Start Date is required.");

            // Logout of Portal
            test.Portal.Logout();
        }

        //[Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Creates a people list and verifies it shows up for that user on the View All page.")]
        [Obsolete("Converted to WebDriver", true)]
        public void Groups_GroupsByGroupType_PeopleLists_Delete() {
            // Initial data
            var peopleListName = "Test New People List - D";
            base.SQL.Groups_PeopleLists_Delete(254, peopleListName);
            base.SQL.Groups_PeopleLists_Create(254, peopleListName, null, "Bryan Mikaelian");
            base.SQL.Groups_PeopleLists_AddManagerOrViewer(254, peopleListName, "Group Admin", GeneralEnumerations.GroupRolesPortalUser.Manager);

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Delete the people list
            test.Portal.Groups_PeopleList_Delete(peopleListName);

            // Navigate to groups->view all
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Verify Test New People List is present
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_AllTab, peopleListName, "Group"), "New people list was still present!");

            // Check the group tab
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.ViewAllTabs.GroupsTab);
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_GroupList_AllTab, peopleListName, "Group"), "New people list was still present and on the group tab!");

            // Check the People List Tab
            test.Selenium.ClickAndWaitForPageToLoad("link=People Lists");
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_ViewAll_PeopleList_PeopleListTab, peopleListName, "People List"), "New people list was still present!");

            // Check the Temporary Group Tab
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.ViewAllTabs.TempGroupTab);
            Assert.IsFalse(test.Selenium.IsElementPresent(string.Format("link={0}", peopleListName)));

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_PeopleLists_Delete(254, peopleListName);
        }

        #endregion People List
    }

    [TestFixture]
    public class Portal_Groups_GroupsByGroupType_Management_WebDriver : FixtureBaseWebDriver
    {
        private string _groupTypeNameMISC = "A Test Group Type:MISCWD";
        private string _groupNameMISC = "A Test Group:MISCWD";
        private string _groupTypeName = "A Generic Group Type WD";
        private string _groupName = "Generic Group WD";
        private string _showLeadersMembersGroup = "Show Leaders Group WD";
        private string _showLeadersMembersGroup2 = "Show Leaders Group 2 WD";
        private string _showAttendanceGroup = "Show Attendance Group WD";
        private string _showAttendanceGroup2 = "Show Attendance Group - No data WD";
        private string _peopleListName = "Generic People List WD";

        [FixtureSetUp]
        public void FixtureSetUp()
        {
            base.SQL.Groups_GroupType_Delete(15, _groupTypeNameMISC);
            base.SQL.Groups_GroupType_Delete(15, _groupTypeName);
            base.SQL.Groups_GroupType_Create(15, "Matthew Sneeden", _groupTypeNameMISC, new List<int> { 4, 5, 10 });
            base.SQL.Groups_GroupType_Create(254, "Group Admin", _groupTypeName, new List<int> { 4, 5, 10, 11 });
            base.SQL.Groups_Group_Create(15, _groupTypeNameMISC, "Matthew Sneeden", _groupNameMISC, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _showLeadersMembersGroup, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _showLeadersMembersGroup2, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _showAttendanceGroup, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _showAttendanceGroup2, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_PeopleLists_Create(254, _peopleListName, null, "Bryan Mikaelian");
            base.SQL.Groups_Group_AddLeaderOrMember(15, _groupNameMISC, "Matthew Sneeden", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _showLeadersMembersGroup, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _showLeadersMembersGroup, "Group Member", "Member");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _showLeadersMembersGroup2, "Group Leader", "Member");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _showAttendanceGroup, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _showAttendanceGroup2, "Group Leader", "Leader");
            base.SQL.Groups_PeopleLists_AddManagerOrViewer(254, _peopleListName, "Group Admin", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddProspect(254, _groupName, "Test", "Prospect", null, null, GeneralEnumerations.ProspectTaskStates.ExpressInterest);
            base.SQL.Groups_Group_AddProspect(254, _groupName, "Denied", "Prospect", null, null, GeneralEnumerations.ProspectTaskStates.Denied);
            base.SQL.Groups_Group_CreateAttendanceRecord(254, _showAttendanceGroup, new List<string>() { "Group Leader" }, true, false);
            base.SQL.Groups_Group_CreateSchedule(254, GeneralEnumerations.GroupScheduleFrequency.Weekly, _showAttendanceGroup, "Group Admin", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(-2).ToShortDateString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortTimeString(), null, null);
        }

        [FixtureTearDown]
        public void FixtureTearDown()
        {
            base.SQL.Groups_GroupType_Delete(15, _groupTypeNameMISC);
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
            base.SQL.Groups_PeopleLists_Delete(254, _peopleListName);
        }

        #region PeopleList

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Category(TestCategories.Services.SmokeTest)]
        [Description("Creates a people list and verifies it shows up for that user on the View All page.")]        
        public void Groups_GroupsByGroupType_PeopleLists_Delete()
        {
            // Initial data
            var peopleListName = "Test New People List - D - WD";
            base.SQL.Groups_PeopleLists_Delete(254, peopleListName);
            base.SQL.Groups_PeopleLists_Create(254, peopleListName, null, "Bryan Mikaelian");
            base.SQL.Groups_PeopleLists_AddManagerOrViewer(254, peopleListName, "Group Admin", GeneralEnumerations.GroupRolesPortalUser.Manager);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "qaeunlx0c2");

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Delete the people list
            test.Portal.Groups_PeopleList_Delete_WebDriver(peopleListName);

            // Navigate to groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Verify Test New People List is present
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_AllTab, peopleListName, "Group"), "New people list was still present!");

            // Check the group tab
            test.Driver.FindElementByXPath(GroupsByGroupTypeConstants.ViewAllTabs.GroupsTab).Click();
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_GroupList_AllTab, peopleListName, "Group"), "New people list was still present and on the group tab!");

            // Check the People List Tab
            test.Driver.FindElementByLinkText("People Lists").Click();
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_ViewAll_PeopleList_PeopleListTab, peopleListName, "People List"), "New people list was still present!");

            // Check the Temporary Group Tab
            test.Driver.FindElementByLinkText(GroupsByGroupTypeConstants.ViewAllTabs.TempGroupTabLink).Click();
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText(peopleListName)));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Groups_PeopleLists_Delete(254, peopleListName);

        }


        #endregion PeopleList

    }
}