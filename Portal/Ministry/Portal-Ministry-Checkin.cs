using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace FTTests.Portal.Ministry {

    [TestFixture]
    public class Portal_Ministry_Checkin_WebDriver : FixtureBaseWebDriver
    {
        //DataConstants.Activity
        //DataConstants.Ministry

        #region Super Check-ins
        [Test, RepeatOnFailure, Timeout(1000)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Creates a Super Check-in.")]
        public void Ministry_Checkin_SuperCheckins_Create()
        {
            // Set initial conditions
            string activityScheduleName = "Test Activity Schedule - SC";
            string superCheckinName = "Test Super Check-in";
            base.SQL.Ministry_SuperCheckins_Delete(15, superCheckinName);
            base.SQL.Ministry_ActivitySchedules_Create(15, DataConstants.Activity, activityScheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")));

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Create a super check-in
            test.Portal.Ministry_SuperCheckins_Create_WebDriver(superCheckinName, DataConstants.Ministry, DataConstants.Activity, new string[] { activityScheduleName });

            // Logout of portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Ministry_SuperCheckins_Delete(15, superCheckinName);
            base.SQL.Ministry_ActivitySchedules_Delete(15, activityScheduleName);

        }


        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Deletes a Super Check-in.")]
        public void Ministry_Checkin_SuperCheckins_Delete()
        {
            // Set initial conditions
            string activityScheduleName = "Test Activity Schedule - SC:D";
            string superCheckinName = "Test Super Check-in - Delete";
            base.SQL.Ministry_SuperCheckins_Delete(15, superCheckinName);
            base.SQL.Ministry_ActivitySchedules_Delete(15, activityScheduleName);
            base.SQL.Ministry_ActivitySchedules_Create(15, "A Test Activity", activityScheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")));
            base.SQL.Ministry_SuperCheckins_Create(15, superCheckinName, activityScheduleName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Delete a super check-in
            test.Portal.Ministry_SuperCheckins_Delete_WebDriver(superCheckinName);

            // Delete an activity schedule
            base.SQL.Ministry_ActivitySchedules_Delete(15, activityScheduleName);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        #endregion Super Check-ins

    }
	[TestFixture]
	public class Portal_Ministry_Checkin : FixtureBase {
        private string _ministryName = "A Test Ministry";
        private string _activityName = "A Test Activity";

		#region Live Check-ins
		#endregion Live Check-ins

		#region Super Check-ins

		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Matthew Sneeden")]
		[Description("Updates a Super Check-in.")]
		public void Ministry_Checkin_SuperCheckins_Update() {
            // Set initial conditions
            string activityName = "A Test Activity";
            string activityScheduleName = "Test Activity Schedule - SC:U";
            string superCheckinName = "Test Super Check-in - Update";
            string superCheckinNameUpdated = "Test Super Check-in - Updated";
            base.SQL.Ministry_SuperCheckins_Delete(15, superCheckinName);
            base.SQL.Ministry_SuperCheckins_Delete(15, superCheckinNameUpdated);
            base.SQL.Ministry_ActivitySchedules_Delete(15, activityScheduleName);
            base.SQL.Ministry_ActivitySchedules_Create(15, activityName, activityScheduleName, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")));
            base.SQL.Ministry_SuperCheckins_Create(15, superCheckinName, activityScheduleName);

			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Update a super check-in
            test.Portal.Ministry_SuperCheckins_Update(superCheckinName, _ministryName, _activityName, new string[] { activityScheduleName }, superCheckinNameUpdated);

			// Logout of portal
			test.Portal.Logout();

		}


		#endregion Super Check-in

		#region Theme Manager
		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
		public void Ministry_Checkin_ThemeManager_ManageTheme() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Manage an existing theme
            test.Portal.Ministry_ThemeNamager_ManageTheme(ThemeManagerConstants.ThemeName);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Theme Overview");
			test.Selenium.VerifyTextPresent("Theme Overview");

			// Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
		public void Ministry_Checkin_ThemeManager_ManageTheme_AddToActivity() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Manage an existing theme
            test.Portal.Ministry_ThemeNamager_ManageTheme(ThemeManagerConstants.ThemeName);

			// Add theme to an activity
            test.Selenium.ClickAndWaitForPageToLoad("link=Apply to activity");

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Apply Theme to Activity");
			test.Selenium.VerifyTextPresent("Apply Theme to Activity");

			// Verify column header
			Assert.AreEqual("Current Theme", test.Selenium.GetText("//table[@class='grid']/tbody/tr[1]/th[2]"));

			// Verify button text
			Assert.AreEqual("Apply theme", test.Selenium.GetValue(GeneralButtons.Save));

			// Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
		public void Ministry_Checkin_ThemeManager_ManageTheme_ApplyToSuperCheckin() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Manage an existing theme
            test.Portal.Ministry_ThemeNamager_ManageTheme(ThemeManagerConstants.ThemeName);

			// Apply to super check-in
			test.Selenium.ClickAndWaitForPageToLoad(ThemeManagerConstants.Apply_to_super_checkin);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Apply Theme to Super Check-in");
			test.Selenium.VerifyTextPresent("Apply Theme to Super Check-in");

			// Verify column headers
			Assert.AreEqual("Super Check-in", test.Selenium.GetText("//table[@id='globalActivities']/tbody/tr[1]/th[2]"));
			Assert.AreEqual("Current Theme", test.Selenium.GetText("//table[@id='globalActivities']/tbody/tr[1]/th[3]"));

			// Verify button text
			Assert.AreEqual("Apply theme", test.Selenium.GetValue(GeneralButtons.Save));

			// Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
		public void Ministry_Checkin_ThemeManager_ManageTheme_ApplyToSuperCheckin_Back() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Manage an existing theme
            test.Portal.Ministry_ThemeNamager_ManageTheme(ThemeManagerConstants.ThemeName);

			// Apply to super check-in
			test.Selenium.ClickAndWaitForPageToLoad(ThemeManagerConstants.Apply_to_super_checkin);

			// Click the back button
			test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Back);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Theme Overview");
			test.Selenium.VerifyTextPresent("Theme Overview");

			// Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
		public void Ministry_Checkin_ThemeManager_ManageTheme_EditTheme() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Manage an existing theme
            test.Portal.Ministry_ThemeNamager_ManageTheme(ThemeManagerConstants.ThemeName);

			// Apply to super check-in
            test.Selenium.ClickAndWaitForPageToLoad("link=Edit theme");

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Theme Overview");
			test.Selenium.VerifyTextPresent("Theme Overview");

			// Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
		public void Ministry_Checkin_ThemeManager_Preview_ReturnToThemeList() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Navigate to ministry->theme manager
			test.Selenium.Navigate(Navigation.Ministry.Checkin.Theme_Manager);

			// Preview a theme
			test.Selenium.ClickAndWaitForPageToLoad(ThemeManagerConstants.Preview);

			// Return to theme list
            test.Selenium.ClickAndWaitForPageToLoad("link=Return to theme list");

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Theme Manager");
			test.Selenium.VerifyTextPresent("Theme Manager");

			// Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
		public void Ministry_Checkin_ThemeManager_Preview_ManageTheme() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Navigate to ministry->theme manager
			test.Selenium.Navigate(Navigation.Ministry.Checkin.Theme_Manager);

			// Preview a theme
			test.Selenium.ClickAndWaitForPageToLoad(ThemeManagerConstants.Preview);

			// Return to theme list
            test.Selenium.Click("link=Manage theme");

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Theme Overview");
			test.Selenium.VerifyTextPresent("Theme Overview");

			// Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Creates a Theme.")]
		public void Ministry_CheckIn_ThemeManager_Create() {
            // Set initial conditions
            string themeName = "Test Theme";
            base.SQL.Ministry_Themes_Delete(15, themeName);

			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Create a theme
            test.Portal.Ministry_ThemeManager_Create(themeName, null, null);

			// Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Updates a Theme.")]
		public void Ministry_CheckIn_ThemeManager_Update() {
            // Set initial conditions
            string themeName = "Test Theme - Update";
            string themeNameUpdated = "Test Theme - Updated";
            base.SQL.Ministry_Themes_Delete(15, themeName);
            base.SQL.Ministry_Themes_Delete(15, themeNameUpdated);
            base.SQL.Ministry_Themes_Create(15, themeName);

			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Update a theme
            test.Portal.Ministry_ThemeManager_Update(themeName, null, null, themeNameUpdated);

			// Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Deletes a Theme.")]
		public void Ministry_CheckIn_ThemeManager_Delete() {
            // Set initial conditions
            string themeName = "Test Theme - Delete";
            base.SQL.Ministry_Themes_Delete(15, themeName);
            base.SQL.Ministry_Themes_Create(15, themeName);

			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Delete a theme
            test.Portal.Ministry_ThemeManager_Delete(themeName);

			// Logout of portal
			test.Portal.Logout();
		}
		#endregion Theme Manager
    }
}