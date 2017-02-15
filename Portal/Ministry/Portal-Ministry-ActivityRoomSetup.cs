using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using System.Collections.Generic;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace FTTests.Portal.Ministry {

    //comment by ivan.zhang since we had removed all features related with legacy. 09/24/2015
    //[TestFixture]
    public class Portal_Ministry_ActivityRoomSetup_WebDriver : FixtureBaseWebDriver
    {

        #region Activity Schedules


        //[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Creates an Activity Schedule.")]
        public void Ministry_ActivityRoomSetup_ActivitySchedules_Create()
        {
            // Set initial conditions
            string activityScheduleName = "Test Activity Schedule - FG";
            base.SQL.Ministry_ActivitySchedules_Delete(15, activityScheduleName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Create an activity schedule
            test.Portal.Ministry_ActivitySchedules_Create_WebDriver(activityScheduleName, null, null, null);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to creates an Activity Schedule without a start date.")]
        public void Ministry_ActivityRoomSetup_ActivitySchedules_CreateNoStartDate()
        {

            IList<string> _errorText = new List<string>();

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to ministry->activity schedules
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.ActivityRoom_Setup.Activity_Schedules);

            // Provide a name
            test.GeneralMethods.WaitForElement(test.Driver, By.Name("txtActivityTimeName"));
            test.Driver.FindElementByXPath("//input[@class='large initial_focus']").SendKeys("Test Activity Time - No Start Time");

            // Remove the start date
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_AddEditTime1_txtStartDate").Clear();
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_AddEditTime1_txtStartTime").Clear();
            // Attempt to save the activity schedule
            test.Driver.FindElementByXPath("//input[@type='submit']").Click();

            // Verify the error displayed to the user
            _errorText.Add("Please enter a valid date and time for the start date");
            test.GeneralMethods.VerifyErrorMessagesWebDriver(_errorText);
            //test.GeneralMethods.VerifyTextPresentWebDriver(new string[] { TextConstants.ErrorHeadingSingular, "Please enter a valid date and time for the start date" });

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }


        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to creates an Activity Schedule without an end date.")]
        public void Ministry_ActivityRoomSetup_ActivitySchedules_CreateNoEndDate()
        {

            IList<string> _errorText = new List<string>();

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to ministry->activity schedules
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.ActivityRoom_Setup.Activity_Schedules);

            // Provide a name
            test.GeneralMethods.WaitForElement(test.Driver, By.Name("txtActivityTimeName"));
            test.Driver.FindElementByXPath("//input[@class='large initial_focus']").SendKeys("Test Activity Time - No End Time");

            // Remove the start date
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_AddEditTime1_txtEndDate").Clear();
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_AddEditTime1_txtEndTime").Clear();

            // Attempt to save the activity schedule
            test.Driver.FindElementByXPath("//input[@type='submit']").Click();

            // Verify the error displayed to the user
            _errorText.Add("Please enter a valid date and time for the end date");
            test.GeneralMethods.VerifyErrorMessagesWebDriver(_errorText);
            //test.GeneralMethods.VerifyTextPresentWebDriver(new string[] { TextConstants.ErrorHeadingSingular, "Please enter a valid date and time for the start date" });

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Deletes an Activity Schedule.")]
        public void Ministry_ActivityRoomSetup_ActivitySchedules_Delete()
        {
            // Set initial conditions
            string activityName = "A Test Activity";
            string[] activityScheduleName = {"Test Activity Schedule - D"};
            base.SQL.Ministry_ActivitySchedules_Delete(15, activityScheduleName[0]);
            base.SQL.Ministry_ActivitySchedules_Create(15, activityName, activityScheduleName[0], TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")));

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Delete an activity schedule
            test.Portal.Ministry_ActivitySchedules_Delete_WebDriver(activityScheduleName);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        #endregion Activity Schedules

    }

	[TestFixture]
	public class Portal_Ministry_ActivityRoomSetup : FixtureBase {

		#region Activities
		#endregion Activities

		#region Activity Schedules
        //[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Verifies start times and end times save correctly for a church set to 12 hour time format.")]
        public void Ministry_ActivityRoomSetup_ActivitySchedules_Create_12HourTimeFormat() {
            // Set initial conditions
            base.SQL.Ministry_ActivitySchedules_Delete(15, "Test AC-12hr-1");
            base.SQL.Ministry_ActivitySchedules_Delete(15, "Test AC-12hr-2");
            base.SQL.Ministry_ActivitySchedules_Delete(15, "Test AC-12hr-3");
            base.SQL.Ministry_ActivitySchedules_Delete(15, "Test AC-12hr-4");
            base.SQL.Ministry_ActivitySchedules_Delete(15, "Test AC-12hr-5");

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Create activity schedules using varying time values
            test.Portal.Ministry_ActivitySchedules_Create("Test AC-12hr-1", "0:00", "10:30", GeneralEnumerations.TimeSetting.TwelveHour);
            test.Portal.Ministry_ActivitySchedules_Create("Test AC-12hr-2", "8:00", "15:30", GeneralEnumerations.TimeSetting.TwelveHour);
            test.Portal.Ministry_ActivitySchedules_Create("Test AC-12hr-3", "12:00", "15:30", GeneralEnumerations.TimeSetting.TwelveHour);
            test.Portal.Ministry_ActivitySchedules_Create("Test AC-12hr-4", "2:00 PM", "15:30", GeneralEnumerations.TimeSetting.TwelveHour);
            test.Portal.Ministry_ActivitySchedules_Create("Test AC-12hr-5", "14:00", "15:30", GeneralEnumerations.TimeSetting.TwelveHour);

            // Logout of portal
            test.Portal.Logout();
        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies start times and end times save correctly for a church set to 24 hour time format.")]
        public void Ministry_ActivityRoomSetup_ActivitySchedules_Create_24HourTimeFormat() {
            // Set initial conditions
            base.SQL.Ministry_ActivitySchedules_Delete(258, "Test AC-24hr-1");
            base.SQL.Ministry_ActivitySchedules_Delete(258, "Test AC-24hr-2");
            base.SQL.Ministry_ActivitySchedules_Delete(258, "Test AC-24hr-3");
            base.SQL.Ministry_ActivitySchedules_Delete(258, "Test AC-24hr-4");
            base.SQL.Ministry_ActivitySchedules_Delete(258, "Test AC-24hr-5");

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("QAEUNLX0C6");

            // Create activity schedules using varying time values
            test.Portal.Ministry_ActivitySchedules_Create("Test AC-24hr-1", "00:00", "10:30", GeneralEnumerations.TimeSetting.TwentyFourHour);
            test.Portal.Ministry_ActivitySchedules_Create("Test AC-24hr-2", "08:00", "14:00", GeneralEnumerations.TimeSetting.TwentyFourHour);
            test.Portal.Ministry_ActivitySchedules_Create("Test AC-24hr-3", "12:00", "15:00", GeneralEnumerations.TimeSetting.TwentyFourHour);
            test.Portal.Ministry_ActivitySchedules_Create("Test AC-24hr-4", "02:00 PM", "18:00", GeneralEnumerations.TimeSetting.TwentyFourHour);
            test.Portal.Ministry_ActivitySchedules_Create("Test AC-24hr-5", "14:00", "18:00", GeneralEnumerations.TimeSetting.TwentyFourHour);

            // Logout of portal
            test.Portal.Logout();
        }
		#endregion Activity Schedules

		#region Activity RLC Groups
		//[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		public void Ministry_ActivityRoomSetup_ActivityRLCGroups_Edit() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Change ministry
			test.Portal.Ministry_ChangeMinistry(DataConstants.Ministry);

			// Navigate to ministry->activity rlc groups
			test.Selenium.Navigate(Navigation.Ministry.ActivityRoom_Setup.Activity_RLC_Groups);

			// Edit an existing group
			test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Edit);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Activity RLC Groups");
			test.Selenium.VerifyTextPresent("Activity RLC Groups");

			// Verify cancel link
			test.Selenium.VerifyElementPresent(GeneralLinks.Cancel);

			// Logout of portal
			test.Portal.Logout();
		}

		//[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Creates an Activity RLC Group.")]
		public void Ministry_ActivityRoomSetup_ActivityRLCGroups_Create() {
            // Set initial conditions
            string activityRLCGroupName = "Test Activity RLC Group";
            base.SQL.Ministry_ActivityRLCGroups_Delete(15, "A Test Activity", activityRLCGroupName);

			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Create an activity rlc group
			test.Portal.Ministry_ActivityRLCGroups_Create(activityRLCGroupName, "No Balance", null);

			// Logout of portal
			test.Portal.Logout();
		}

		//[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Deletes an Activity RLC Group.")]
		public void Ministry_ActivityRoomSetup_ActivityRLCGroups_Delete() {
            // Set initial conditions
            string activityName = "A Test Activity";
            string activityRLCGroupName = "Test Activity RLC Group - D";
            base.SQL.Ministry_ActivityRLCGroups_Delete(15, activityName, activityRLCGroupName);
            base.SQL.Ministry_ActivityRLCGroups_Create(15, activityName, activityRLCGroupName);

			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Delete an activity rlc group
			test.Portal.Ministry_ActivityRLCGroups_Delete(activityRLCGroupName);

			// Logout of portal
			test.Portal.Logout();
		}

		//[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Creates an Activity RLC Group as part of a Super Group.")]
		public void Ministry_ActivityRoomSetup_ActivityRLCGroups_CreateSuperGroup() {
            // Set initial conditions
            string activityName = "A Test Activity";
            string activityRLCGroupNameSG = "Test Activity RLC Group - SG";
            string activityRLCGroupName = "Test Activity RLC Group - SG C";
            base.SQL.Ministry_ActivityRLCGroups_Delete(15, activityName, activityRLCGroupNameSG);
            base.SQL.Ministry_ActivityRLCGroups_Delete(15, activityName, activityRLCGroupName);
            base.SQL.Ministry_ActivityRLCGroups_Create(15, activityName, activityRLCGroupNameSG);


            
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Create another activity rlc group
            test.Portal.Ministry_ActivityRLCGroups_Create(activityRLCGroupName, "No Balance", activityRLCGroupNameSG);

			// Delete the super group
			test.Portal.Ministry_ActivityRLCGroups_Delete(" Test Activity RLC Group - SG   - Test Activity RLC Group - SG C ");

			// Logout of portal
			test.Portal.Logout();
            
		}
		#endregion Activity RLC Groups

		#region Rooms & Locations
		//[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		public void Ministry_ActivityRoomSetup_RoomsLocations_Edit() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Change ministry
			test.Portal.Ministry_ChangeMinistry(DataConstants.Ministry);

			// Navigate to ministry->activity rlc groups
			test.Selenium.Navigate(Navigation.Ministry.ActivityRoom_Setup.Activity_RLC_Groups);

			// Edit an existing
			test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Edit);

			// Verify cancel control is a link
			test.Selenium.VerifyElementPresent(GeneralLinks.Cancel);

			// Logout of portal
			test.Portal.Logout();
		}
		#endregion Rooms & Locations

		#region Breakout Groups
		#endregion Breakout Groups

		#region Activity Requirements
		//[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		public void Ministry_ActivityRoomSetup_ActivityRequirements_Edit() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Change ministry
			test.Portal.Ministry_ChangeMinistry(DataConstants.Ministry);

			// Navigate to ministry->activity requirements
			test.Selenium.Navigate(Navigation.Ministry.ActivityRoom_Setup.Activity_Requirements);

			// Edit an existing requirement
			test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Edit);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Activity Requirements");
			test.Selenium.VerifyTextPresent("Activity Requirements");

			// Verify buttons
			Assert.AreEqual("Save requirement", test.Selenium.GetValue(GeneralButtons.Save));

			// Logout of portal
			test.Portal.Logout();
		}
		#endregion Activity Requirements

		#region Group Finder Properties
		//[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Matthew Sneeden")]
		[Description("Creates a Group Finder Property.")]
		public void Ministry_ActivityRoomSetup_GroupFinderProperties_Create() {
            // Set initial conditions
            string propertyName = "Test Property";
            base.SQL.Ministry_GroupFinderProperties_Delete(15, "A Test Activity", propertyName);

			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Create a group finder property
			test.Portal.Ministry_GroupFinderProperties_Create(propertyName, false, false);

			// Logout of portal
			test.Portal.Logout();
		}

		//[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Creates a Group Finder Property with the Include as Filter Choice set to True.")]
		public void Ministry_ActivityRoomSetup_GroupFinderProperties_Create_IncludeAsFilterChoice() {
            // Set initial conditions
            string propertyName = "Test Property - Include as Filter Choice";
            base.SQL.Ministry_GroupFinderProperties_Delete(15, "A Test Activity", propertyName);

			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Create a group finder property
            test.Portal.Ministry_GroupFinderProperties_Create(propertyName, true, false);

			// Logout of portal
			test.Portal.Logout();
		}

		//[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Creates a Group Finder Property with the Show in Results set to True.")]
		public void Ministry_ActivityRoomSetup_GroupFinderProperties_Create_ShowInResults() {
            // Set initial conditions
            string propertyName = "Test Property - Show in Results";
            base.SQL.Ministry_GroupFinderProperties_Delete(15, "A Test Activity", propertyName);

			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Create a group finder property
            test.Portal.Ministry_GroupFinderProperties_Create(propertyName, false, true);

			// Logout of portal
			test.Portal.Logout();
		}

		//[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Updates a Group Finder Property.")]
		public void Ministry_ActivityRoomSetup_GroupFinderProperties_Update() {
            // Set initial conditions
            string activityName = "A Test Activity";
            string propertyName = "Test Property - Update";
            string propertyNameUpdated = "Test Property - Updated";
            base.SQL.Ministry_GroupFinderProperties_Delete(15, activityName, propertyName);
            base.SQL.Ministry_GroupFinderProperties_Delete(15, activityName, propertyNameUpdated);
            base.SQL.Ministry_GroupFinderProperties_Create(15, activityName, propertyName, false, false);

			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Update the group finder property
            test.Portal.Ministry_GroupFinderProperties_Update(propertyName, false, false, propertyNameUpdated, true, true);

			// Logout of portal
			test.Portal.Logout();
		}

		//[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Deletes a Group Finder Property.")]
		public void Ministry_ActivityRoomSetup_GroupFinderProperties_Delete() {
            // Set initial conditions
            string activityName = "A Test Activity";
            string propertyName = "Test Property - Delete";
            base.SQL.Ministry_GroupFinderProperties_Delete(15, activityName, propertyName);
            base.SQL.Ministry_GroupFinderProperties_Create(15, activityName, propertyName, false, false);

			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Delete the group finder property
            test.Portal.Ministry_GroupFinderProperties_Delete(propertyName);

			// Logout of portal
			test.Portal.Logout();
		}

		//[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Creates a Group Finder Choice.")]
		public void Ministry_ActivityRoomSetup_GroupFinderProperties_Choices_Create() {
            // Set initial conditions
            string activityName = "A Test Activity";
            string propertyName = "Test Property - Choice:C";
            string choiceName = "Test Choice";
            base.SQL.Ministry_GroupFinderProperties_Choices_Delete(15, activityName, propertyName, choiceName);
            base.SQL.Ministry_GroupFinderProperties_Delete(15, activityName, propertyName);
            base.SQL.Ministry_GroupFinderProperties_Create(15, activityName, propertyName, false, false);

			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Create a group finder choice
            test.Portal.Ministry_GroupFinderProperties_Choices_Create(propertyName, choiceName);

			// Logout of portal
			test.Portal.Logout();
		}

		//[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Updates a Group Finder Choice.")]
		public void Ministry_ActivityRoomSetup_GroupFinderProperties_Choices_Update() {
            // Set initial conditions
            string activityName = "A Test Activity";
            string propertyName = "Test Property - Choice:U";
            string choiceName = "Test Choice - Update";
            string choiceNameUpdated = "Test Choice - Updated";
            base.SQL.Ministry_GroupFinderProperties_Choices_Delete(15, activityName, propertyName, choiceName);
            base.SQL.Ministry_GroupFinderProperties_Choices_Delete(15, activityName, propertyName, choiceNameUpdated);
            base.SQL.Ministry_GroupFinderProperties_Delete(15, activityName, propertyName);
            base.SQL.Ministry_GroupFinderProperties_Create(15, activityName, propertyName, false, false);
            base.SQL.Ministry_GroupFinderProperties_Choices_Create(15, activityName, propertyName, choiceName);

			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Update the group finder choice
            test.Portal.Ministry_GroupFinderProperties_Choices_Update(propertyName, choiceName, choiceNameUpdated);

			// Logout of portal
			test.Portal.Logout();
		}

		//[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Deletes a Group Finder Choice.")]
		public void Ministry_ActivityRoomSetup_GroupFinderProperties_Choices_Delete() {
            // Set initial conditions
            string activityName = "A Test Activity";
            string propertyName = "Test Property - Choice:D";
            string choiceName = "Test Choice - Delete";
            base.SQL.Ministry_GroupFinderProperties_Choices_Delete(15, activityName, propertyName, choiceName);
            base.SQL.Ministry_GroupFinderProperties_Delete(15, activityName, propertyName);
            base.SQL.Ministry_GroupFinderProperties_Create(15, activityName, propertyName, false, false);
            base.SQL.Ministry_GroupFinderProperties_Choices_Create(15, activityName, propertyName, choiceName);

			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Delete the group finder choice
            test.Portal.Ministry_GroupFinderProperties_Choices_Delete(propertyName, choiceName);

			// Logout of portal
			test.Portal.Logout();
		}
		#endregion Group Finder Properties
    }



}