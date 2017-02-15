using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using System.Collections.ObjectModel;
using System.Data;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;


namespace FTTests.infellowship.People {

	[TestFixture]
	public class infellowship_People_PrivacySettings : FixtureBase {
        private string _groupTypeName = "Privacy Settings Group Type";
        private string _groupName = "General Privacy Group";

        [FixtureSetUp]
        public void FixtureSetUp() {
            // Create a generic group and add a leader and member
            TestLog.WriteLine("[FixtureSetup] Create generic group, leader, member");
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", _groupTypeName, new List<int> { 1, 2, 3, 4, 5, 9, 10, 11, 12 });
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Member", "Member");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "InFellowship User", "Member");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "InFellowship Privacy", "Member");
            TestLog.WriteLine("[FixtureSetup] Complete generic group, leader, member setup");
        }

        [FixtureTearDown]
        public void FixtureTearDown() {
            TestLog.WriteLine("[FixtureTearDown] Delete " + _groupTypeName);
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);

        }


		#region Address Privacy Settings
        [Test, RepeatOnFailure(Order = 1)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can allow leaders and church staff to view address information")]
        public void PrivacySettings_Update_Address_Leaders() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// Set the privacy settings to allow only church staff and leaders to view address information.
            test.infellowship.People_PrivacySettings_Update_AddressSetting(GeneralEnumerations.PrivacyLevels.Leaders);

			// Logout
			test.infellowship.Logout();


            // Login to infellowship as a Leader
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the address information is showing up
            Assert.IsTrue(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"));
            Assert.IsTrue(test.Selenium.IsTextPresent("Flower Mound, TX 75022"));

            // Logout
            test.infellowship.Logout();


            // Login to infellowship as a Member
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the address information is not showing up
            Assert.IsFalse(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"));
            Assert.IsFalse(test.Selenium.IsTextPresent("Flower Mound, TX 75022"));

            // Logout
            test.infellowship.Logout();


            // Login to infellowship
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Search for an individual in the directory
            infellowship_People_ChurchDirectory chDir = new infellowship_People_ChurchDirectory();
            test.infellowship.People_ChurchDirectory_Search("InFellowship Privacy");

            // Verify the City, State are present but the address and Zip are not present.
            Assert.IsFalse(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"));
            Assert.IsFalse(test.Selenium.IsTextPresent("75022"));
            Assert.IsTrue(test.Selenium.IsTextPresent("Flower Mound, TX"));

            // View the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the City, State are present but the address and Zip are not present.
            Assert.IsFalse(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"));
            Assert.IsFalse(test.Selenium.IsTextPresent("75022"));
            Assert.IsTrue(test.Selenium.IsTextPresent("Flower Mound, TX"));


            // Logout
            test.infellowship.Logout();
        }

		[Test, RepeatOnFailure(Order=2)]
		[MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you can allow members, leaders and church staff to view address information")]
		public void PrivacySettings_Update_Address_Members() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// Set the privacy settings to allow only church staff and leaders and members to view address information.
            test.infellowship.People_PrivacySettings_Update_AddressSetting(GeneralEnumerations.PrivacyLevels.Members);

			// Logout
			test.infellowship.Logout();


			// Login to infellowship as a Leader
			test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Click on the individual
			test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

			// Verify the address information is showing up
			Assert.IsTrue(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"));
			Assert.IsTrue(test.Selenium.IsTextPresent("Flower Mound, TX 75022"));

			// Logout
			test.infellowship.Logout();


			// Login to infellowship as a Member
			test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

			// Verify the address information is showing up
			Assert.IsTrue(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"));
			Assert.IsTrue(test.Selenium.IsTextPresent("Flower Mound, TX 75022"));

			// Logout
			test.infellowship.Logout();


            // Login to infellowship
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Search for an individual in the directory
            infellowship_People_ChurchDirectory chDir = new infellowship_People_ChurchDirectory();
            test.infellowship.People_ChurchDirectory_Search("InFellowship Privacy");

            // Verify the City, State are present but the address and Zip are not present.
            Assert.IsFalse(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"));
            Assert.IsFalse(test.Selenium.IsTextPresent("75022"));
            Assert.IsTrue(test.Selenium.IsTextPresent("Flower Mound, TX"));

            // View the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the City, State are present but the address and Zip are not present.
            Assert.IsFalse(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"));
            Assert.IsFalse(test.Selenium.IsTextPresent("75022"));
            Assert.IsTrue(test.Selenium.IsTextPresent("Flower Mound, TX"));

            // Logout
            test.infellowship.Logout();
		}

		[Test, RepeatOnFailure(Order=3)]
		[MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you can allow church staff to view address information")]
		public void PrivacySettings_Update_Address_ChurchStaff() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// Set the privacy settings to allow only church staff view address information.
            test.infellowship.People_PrivacySettings_Update_AddressSetting(GeneralEnumerations.PrivacyLevels.ChurchStaff);

			// Logout
			test.infellowship.Logout();

			// Login to infellowship as a Leader
			test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

			// Verify the address information is not showing up
			Assert.IsFalse(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"));
			Assert.IsFalse(test.Selenium.IsTextPresent("Flower Mound, TX 75022"));

			// Logout
			test.infellowship.Logout();

			// Login to infellowship as a Member
			test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

			// Verify the address information is not showing up
			Assert.IsFalse(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"));
			Assert.IsFalse(test.Selenium.IsTextPresent("Flower Mound, TX 75022"));

			// Logout
			test.infellowship.Logout();

            // Login to infellowship
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Search for an individual in the directory
            infellowship_People_ChurchDirectory chDir = new infellowship_People_ChurchDirectory();
            test.infellowship.People_ChurchDirectory_Search("InFellowship Privacy");

            // Verify the City, State are present but the address and Zip are not present.
            Assert.IsFalse(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"));
            Assert.IsFalse(test.Selenium.IsTextPresent("75022"));
            Assert.IsTrue(test.Selenium.IsTextPresent("Flower Mound, TX"));

            // View the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the City, State are present but the address and Zip are not present.
            Assert.IsFalse(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"));
            Assert.IsFalse(test.Selenium.IsTextPresent("75022"));
            Assert.IsTrue(test.Selenium.IsTextPresent("Flower Mound, TX"));

            // Logout
            test.infellowship.Logout();
		}

		[Test, RepeatOnFailure(Order=4)]
		[MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you can allow everyone to view address information")]
		public void PrivacySettings_Update_Address_Everyone() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login ..");
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// Set the privacy settings to allow everyone to view address information.
            TestLog.WriteLine("People_PrivacySettings_Update_AddressSetting - Everyone");
            test.infellowship.People_PrivacySettings_Update_AddressSetting(GeneralEnumerations.PrivacyLevels.Everyone);

			// Logout
            TestLog.WriteLine("Logout");
			test.infellowship.Logout();

            TestLog.WriteLine("Refresh...");
            test.infellowship.Refresh();

			// Login to infellowship as a Leader
            TestLog.WriteLine("Login ..");
			test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View the roster of a group
            TestLog.WriteLine("Groups Group View Roster - " + _groupName);
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Click on the individual
            TestLog.WriteLine("Click InFellowship Privacy");
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

			// Verify the address information is showing up
            TestLog.WriteLine("Verify Address Information ... ");
			Assert.IsTrue(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"));
			Assert.IsTrue(test.Selenium.IsTextPresent("Flower Mound, TX 75022"));

			// Logout
            TestLog.WriteLine("Logout");
			test.infellowship.Logout();

            test.infellowship.Refresh();


			// Login to infellowship as a Member
			test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View the roster of a group
            TestLog.WriteLine("Groups Group View Roster - " + _groupName);
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Click on the individual
            TestLog.WriteLine("Click Infellowship Privacy");
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

			// Verify the address information is showing up
            TestLog.WriteLine("Verify Address Information");
			Assert.IsTrue(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"));
			Assert.IsTrue(test.Selenium.IsTextPresent("Flower Mound, TX 75022"));

			// Logout
            TestLog.WriteLine("Logout");
            test.infellowship.Logout();

            test.infellowship.Refresh();

            // Login to infellowship
            TestLog.WriteLine("Login ");
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Search for an individual in the directory
            TestLog.WriteLine("People ChurchDirectory Search Infellowship Privacy");
            test.infellowship.People_ChurchDirectory_Search("InFellowship Privacy");

            // Verify the City, State, and Address is present
            TestLog.WriteLine("Verify City, State, ZIP");
            Assert.IsTrue(test.Selenium.IsTextPresent("Flower Mound, TX"));
            Assert.IsTrue(test.Selenium.IsTextPresent("75022"));

            // View the individual
            TestLog.WriteLine("Click Infellowship Privacy");
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the City, State, and Address is present.
            TestLog.WriteLine("Verify Address ...");
            Assert.IsTrue(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"));
            Assert.IsTrue(test.Selenium.IsTextPresent("Flower Mound, TX 75022"));

            // Logout
            TestLog.WriteLine("Logout");
            test.infellowship.Logout();
		}

		#endregion Address Privacy Settings

		#region Date of Birth Privacy Settings
		[Test, RepeatOnFailure(Order = 5)]
		[MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you can allow leaders and church staff to view date of birth information")]
		public void PrivacySettings_Update_DateOfBirth_Leaders() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// Set the privacy settings to allow only church staff and leaders to view date of birth information.
            test.infellowship.People_PrivacySettings_Update_BirthdateSetting(GeneralEnumerations.PrivacyLevels.Leaders);

			// Logout
			test.infellowship.Logout();

			// Login to infellowship as a Leader
			test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

			// Verify the date of birth information is showing up
			Assert.IsTrue(test.Selenium.IsTextPresent("November 15"));

			// Logout
			test.infellowship.Logout();

			// Login to infellowship as a Member
			test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

			// Verify the date of birth information is not showing up
			Assert.IsFalse(test.Selenium.IsTextPresent("November 15"));

			// Logout
			test.infellowship.Logout();

            // Login to infellowship
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View  an individual in the directory
            test.infellowship.People_ChurchDirectory_View_Individual("InFellowship Privacy");

            // Verify the date of birth information is not showing up
            Assert.IsFalse(test.Selenium.IsTextPresent("November 15"));

            // Logout
            test.infellowship.Logout();
		}

		[Test, RepeatOnFailure(Order = 6)]
		[MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you can allow leaders, members and church staff to view date of birth information")]
		public void PrivacySettings_Update_DateOfBirth_Members() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// Set the privacy settings to allow only church staff and leaders and members to view date of birth information.
            test.infellowship.People_PrivacySettings_Update_BirthdateSetting(GeneralEnumerations.PrivacyLevels.Members);

			// Logout
			test.infellowship.Logout();

			// Login to infellowship as a Leader
			test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Click on the individual
			test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

			// Verify the date of birth information is showing up
			Assert.IsTrue(test.Selenium.IsTextPresent("November 15"));

			// Logout
			test.infellowship.Logout();

			// Login to infellowship as a Member
			test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Click on the individual
			test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

			// Verify the date of birth information is showing up
			Assert.IsTrue(test.Selenium.IsTextPresent("November 15"));

			// Logout
			test.infellowship.Logout();

            // Login to infellowship
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View  an individual in the directory
            infellowship_People_ChurchDirectory chDir = new infellowship_People_ChurchDirectory();
            test.infellowship.People_ChurchDirectory_View_Individual("InFellowship Privacy");

            // Verify the date of birth information is not showing up
            Assert.IsFalse(test.Selenium.IsTextPresent("November 15"));

            // Logout
            test.infellowship.Logout();
		}

		[Test, RepeatOnFailure(Order = 7)]
		[MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you can allow church staff to view date of birth information")]
		public void PrivacySettings_Update_DateOfBirth_ChurchStaff() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// Set the privacy settings to allow only church staff view date of birth information.
            test.infellowship.People_PrivacySettings_Update_BirthdateSetting(GeneralEnumerations.PrivacyLevels.ChurchStaff);

			// Logout
			test.infellowship.Logout();

			// Login to infellowship as a Leader
			test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Click on the individual
			test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

			// Verify the date of birth information is not showing up
			Assert.IsFalse(test.Selenium.IsTextPresent("November 15"));

			// Logout
			test.infellowship.Logout();

			// Login to infellowship as a Member
			test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Click on the individual
			test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

			// Verify the date of birth information is not showing up
			Assert.IsFalse(test.Selenium.IsTextPresent("November 15"));

			// Logout
			test.infellowship.Logout();

            // Login to infellowship
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View  an individual in the directory
            infellowship_People_ChurchDirectory chDir = new infellowship_People_ChurchDirectory();
            test.infellowship.People_ChurchDirectory_View_Individual("InFellowship Privacy");

            // Verify the date of birth information is not showing up
            Assert.IsFalse(test.Selenium.IsTextPresent("November 15"));

            // Logout
            test.infellowship.Logout();

		}

		[Test, RepeatOnFailure(Order = 8)]
		[MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you can allow everyone to view date of birth information")]
		public void PrivacySettings_Update_DateOfBirth_Everyone() {
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// Set the privacy settings to allow everyone to view date of birth information.
            test.infellowship.People_PrivacySettings_Update_BirthdateSetting(GeneralEnumerations.PrivacyLevels.Everyone);

			// Logout
			test.infellowship.Logout();

			// Login to infellowship as a Leader
			test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Click on the individual
			test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

			// Verify the date of birth information is showing up
			Assert.IsTrue(test.Selenium.IsTextPresent("November 15"));

			// Logout
			test.infellowship.Logout();

			// Login to infellowship as a Member
			test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Click on the individual
			test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

			// Verify the date of birth information is showing up
			Assert.IsTrue(test.Selenium.IsTextPresent("November 15"));

			// Logout
			test.infellowship.Logout();

            // Login to infellowship
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View  an individual in the directory
            infellowship_People_ChurchDirectory chDir = new infellowship_People_ChurchDirectory();
            test.infellowship.People_ChurchDirectory_View_Individual("InFellowship Privacy");

            // Verify the date of birth information is showing up
            Assert.IsTrue(test.Selenium.IsTextPresent("November 15"));

            // Logout
            test.infellowship.Logout();
		}
		#endregion Data of Birth Privacy Settings

		#region Email Privacy Settings
		[Test, RepeatOnFailure(Order = 9)]
		[MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you can allow leaders and church staff to view email information")]
		public void PrivacySettings_Update_Email_Leaders() {    
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// Set the privacy settings to allow only church staff and leaders to view email information.
            test.infellowship.People_PrivacySettings_Update_EmailSetting(GeneralEnumerations.PrivacyLevels.Leaders);

			// Logout
			test.infellowship.Logout();


			// Login to infellowship as a Leader
			test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Verify the email address is present
            // Figure out what row the individual is on.
            var row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));

            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[3]/div/a", row + 1));

			// Logout
			test.infellowship.Logout();


			// Login to infellowship as a Member
			test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Verify the email address is not present
            // Figure out what row the individual is on.
            row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));

            test.Selenium.VerifyElementNotPresent(string.Format("//tr[{0}]/td[3]/div/a", row + 1));

			// Logout
			test.infellowship.Logout();

            // Login to infellowship
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Search for an individual in the directory
            infellowship_People_ChurchDirectory chDir = new infellowship_People_ChurchDirectory();
            test.infellowship.People_ChurchDirectory_Search("InFellowship Privacy");

            // Verify the email address is not present
            test.Selenium.VerifyElementNotPresent("link=infellowship.privacy@gmail.com");

            // View the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the email address is not present
            test.Selenium.VerifyElementNotPresent("link=infellowship.privacy@gmail.com");

            // Logout
            test.infellowship.Logout();
		}
		
		[Test, RepeatOnFailure(Order = 10)]
		[MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you can allow leaders, members and church staff to view email information")]
		public void PrivacySettings_Update_Email_Members() {    
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// Set the privacy settings to allow only church staff and leaders and members to view email information.
            test.infellowship.People_PrivacySettings_Update_EmailSetting(GeneralEnumerations.PrivacyLevels.Members);

			// Logout
			test.infellowship.Logout();


			// Login to infellowship as a Leader
			test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Verify the email address is present
            var row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));

            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[3]/div/a", row + 1));

			// Logout
			test.infellowship.Logout();


			// Login to infellowship as a Member
			test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Verify the email address is present
            row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));

            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[3]/div/a", row + 1));

			// Logout
			test.infellowship.Logout();

            // Login to infellowship
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Search for an individual in the directory
            infellowship_People_ChurchDirectory chDir = new infellowship_People_ChurchDirectory();
            test.infellowship.People_ChurchDirectory_Search("InFellowship Privacy");

            // Verify the email address is not present
            test.Selenium.VerifyElementNotPresent("link=infellowship.privacy@gmail.com");

            // View the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the email address is not present
            test.Selenium.VerifyElementNotPresent("link=infellowship.privacy@gmail.com");

            // Logout
            test.infellowship.Logout();
		}
		
		[Test, RepeatOnFailure(Order = 11)]
		[MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you can allow only church staff to view email information")]
		public void PrivacySettings_Update_Email_ChurchStaff() {    
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// Set the privacy settings to allow only church staff to view email information.
            test.infellowship.People_PrivacySettings_Update_EmailSetting(GeneralEnumerations.PrivacyLevels.ChurchStaff);

			// Logout
			test.infellowship.Logout();

			// Login to infellowship as a Leader
			test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Verify the email address is not present
            var row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));

            test.Selenium.VerifyElementNotPresent(string.Format("//tr[{0}]/td[3]/div/a", row + 1)); ;

			// Logout
			test.infellowship.Logout();


			// Login to infellowship as a Member
			test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Verify the email address is not present
            row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));

            test.Selenium.VerifyElementNotPresent(string.Format("//tr[{0}]/td[3]/div/a", row + 1));

			// Logout
			test.infellowship.Logout();


            // Login to infellowship
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Search for an individual in the directory
            infellowship_People_ChurchDirectory chDir = new infellowship_People_ChurchDirectory();
            test.infellowship.People_ChurchDirectory_Search("InFellowship Privacy");

            // Verify the email address is not present
            test.Selenium.VerifyElementNotPresent("link=infellowship.privacy@gmail.com");

            // View the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the email address is not present
            test.Selenium.VerifyElementNotPresent("link=infellowship.privacy@gmail.com");

            // Logout
            test.infellowship.Logout();
		}

		[Test, RepeatOnFailure(Order = 12)]
		[MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Verifies you can allow everyone to view email information")]
		public void PrivacySettings_Update_Email_Everyone() {    
			// Login to infellowship
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// Set the privacy settings to allow everyone to view email information.
            test.infellowship.People_PrivacySettings_Update_EmailSetting(GeneralEnumerations.PrivacyLevels.Everyone);

			// Logout
			test.infellowship.Logout();


			// Login to infellowship as a Leader
			test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Verify the email address is present
            var row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));

            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[3]/div/a", row + 1));

			// Logout
			test.infellowship.Logout();


			// Login to infellowship as a Member
			test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

			// View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

			// Verify the email address is present
            row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));

            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[3]/div/a", row + 1));

			// Logout
            test.infellowship.Logout();


            // Login to infellowship
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Search for an individual in the directory
            infellowship_People_ChurchDirectory chDir = new infellowship_People_ChurchDirectory();
            test.infellowship.People_ChurchDirectory_Search("InFellowship Privacy");

            // Verify the email address is present
            test.Selenium.VerifyElementPresent("link=infellowship.priv...");

            // View the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the email address is present
            test.Selenium.VerifyElementPresent("link=infellowship.privacy@gmail.com");

            // Logout
            test.infellowship.Logout();
		}
		#endregion Email Privacy Settings

        #region Phone Privacy Settings
        [Test, RepeatOnFailure(Order = 13)]
        [MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can allow leaders and church staff to view phone information")]
        public void PrivacySettings_Update_Phone_Leaders() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Set the privacy settings to allow only church staff and leaders to view phone information.
            test.infellowship.People_PrivacySettings_Update_PhoneSetting(GeneralEnumerations.PrivacyLevels.Leaders);

            // Logout
            test.infellowship.Logout();


            // Login to infellowship as a Leader
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Verify the phone information is showing up on the roster
            // Figure out what row the individual is on.
            var row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));

            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[4]/div/span[3][text()='555-555-5555']", row + 1));

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the phone information is showing up
            Assert.IsTrue(test.Selenium.IsTextPresent("555-555-5555"));

            // Logout
            test.infellowship.Logout();


            // Login to infellowship as a Member
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Verify the phone information is not showing up
            // Figure out what row the individual is on.
            row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));

            test.Selenium.VerifyElementNotPresent(string.Format("//tr[{0}]/td[4]/div/span[3][text()='555-555-5555']", row + 1));

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the phone information is not showing up
            Assert.IsFalse(test.Selenium.IsTextPresent("555-555-5555"));

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure(Order = 14)]
        [MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can allow members, leaders and church staff to view phone information")]
        public void PrivacySettings_Update_Phone_Members() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Set the privacy settings to allow only church staff and members and leaders to view phone information.
            test.infellowship.People_PrivacySettings_Update_PhoneSetting(GeneralEnumerations.PrivacyLevels.Members);

            // Logout
            test.infellowship.Logout();


            // Login to infellowship as a Leader
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Verify the phone information is showing up on the roster
            // Figure out what row the individual is on.
            var row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));

            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[4]/div/span[3][text()='555-555-5555']", row + 1));

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the phone information is showing up
            Assert.IsTrue(test.Selenium.IsTextPresent("555-555-5555"));

            // Logout
            test.infellowship.Logout();


            // Login to infellowship as a Member
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Verify the phone information is showing up
            // Figure out what row the individual is on.
            row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));

            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[4]/div/span[3][text()='555-555-5555']", row + 1));

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the phone information is showing up
            Assert.IsTrue(test.Selenium.IsTextPresent("555-555-5555"));

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure(Order = 15)]
        [MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can allow Church Staff to view phone information")]
        public void PrivacySettings_Update_Phone_ChurchStaff() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Set the privacy settings to allow only church staff to view phone information.
            test.infellowship.People_PrivacySettings_Update_PhoneSetting(GeneralEnumerations.PrivacyLevels.ChurchStaff);

            // Logout
            test.infellowship.Logout();


            // Login to infellowship as a Leader
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Verify the phone information is not showing up on the roster
            // Figure out what row the individual is on.
            var row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));

            test.Selenium.VerifyElementNotPresent(string.Format("//tr[{0}]/td[4]/div/span[3][text()='555-555-5555']", row + 1));

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the phone information is not showing up
            Assert.IsFalse(test.Selenium.IsTextPresent("555-555-5555"));

            // Logout
            test.infellowship.Logout();


            // Login to infellowship as a Member
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Verify the phone information is not showing up
            // Figure out what row the individual is on.
            row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));

            test.Selenium.VerifyElementNotPresent(string.Format("//tr[{0}]/td[4]/div/span[3][text()='555-555-5555']", row + 1));

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the phone information is not showing up
            Assert.IsFalse(test.Selenium.IsTextPresent("555-555-5555"));

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure(Order = 16)]
        [MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can allow Everyone to view phone information")]
        public void PrivacySettings_Update_Phone_Everyone() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Set the privacy settings to allow everyone to view phone information.
            test.infellowship.People_PrivacySettings_Update_PhoneSetting(GeneralEnumerations.PrivacyLevels.Everyone);

            // Logout
            test.infellowship.Logout();


            // Login to infellowship as a Leader
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);
            // Figure out what row the individual is on.
            var row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));

            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[4]/div/span[3][text()='555-555-5555']", row + 1));

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the phone information is showing up
            Assert.IsTrue(test.Selenium.IsTextPresent("555-555-5555"));

            // Logout
            test.infellowship.Logout();


            // Login to infellowship as a Member
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Verify the phone information is showing up
            // Figure out what row the individual is on.
            row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));

            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[4]/div/span[3][text()='555-555-5555']", row + 1));

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the phone information is showing up
            Assert.IsTrue(test.Selenium.IsTextPresent("555-555-5555"));

            // Logout
            test.infellowship.Logout();
        }
        #endregion Phone Privacy Settings

        #region Social Media Privacy Settings
        [Test, RepeatOnFailure(Order = 20)]
        [MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can allow Everyone to view social media information")]
        public void PrivacySettings_Update_SocialMedia_Everyone() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Set the privacy settings to allow everyone to view social media information.
            test.infellowship.People_PrivacySettings_Update_SocialSetting(GeneralEnumerations.PrivacyLevels.Everyone);

            // Logout
            test.infellowship.Logout();


            // Login to infellowship as a Leader
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Verify the social media information is showing up on the roster
            // Figure out what row the individual is on
            var row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));

            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[5]/a[1]/img[@alt='Facebook']", row + 1));
            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[5]/a[2]/img[@alt='Twitter']", row + 1));
            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[5]/a[3]/img[@alt='LinkedIn']", row + 1));

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the social media information is showing up
            test.Selenium.VerifyElementPresent("//a[1]/img[@alt='Facebook']");
            test.Selenium.VerifyElementPresent("//a[2]/img[@alt='Linkedin']");
            test.Selenium.VerifyElementPresent("//a[3]/img[@alt='Twitter']");

            // Logout
            test.infellowship.Logout();


            // Login to infellowship as a Member
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Verify the social media information is showing up on the roster
            row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));
            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[5]/a[1]/img[@alt='Facebook']", row + 1));
            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[5]/a[2]/img[@alt='Twitter']", row + 1));
            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[5]/a[3]/img[@alt='LinkedIn']", row + 1));

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the social media information is showing up
            test.Selenium.VerifyElementPresent("//a[1]/img[@alt='Facebook']");
            test.Selenium.VerifyElementPresent("//a[2]/img[@alt='Linkedin']");
            test.Selenium.VerifyElementPresent("//a[3]/img[@alt='Twitter']");

            // Logout
            test.infellowship.Logout();


            // Login to infellowship
            test.infellowship.Login("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // Verify social media is present on the church directory
            infellowship_People_ChurchDirectory chDir = new infellowship_People_ChurchDirectory();
            test.infellowship.People_ChurchDirectory_Search("InFellowship Privacy");
            test.Selenium.VerifyElementPresent("//a[1]/img[@alt='Facebook']");
            test.Selenium.VerifyElementPresent("//a[2]/img[@alt='Twitter']");
            test.Selenium.VerifyElementPresent("//a[3]/img[@alt='LinkedIn']");

            test.infellowship.People_ChurchDirectory_View_Individual("InFellowship Privacy");
            test.Selenium.VerifyElementPresent("//a[1]/img[@alt='Facebook']");
            test.Selenium.VerifyElementPresent("//a[2]/img[@alt='Twitter']");
            test.Selenium.VerifyElementPresent("//a[3]/img[@alt='LinkedIn']");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure(Order = 19)]
        [MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can allow Church Staff to view social media information")]
        public void PrivacySettings_Update_SocialMedia_ChurchStaff() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Set the privacy settings to allow church staff to view social media information.
            test.infellowship.People_PrivacySettings_Update_SocialSetting(GeneralEnumerations.PrivacyLevels.ChurchStaff);

            // Logout
            test.infellowship.Logout();


            // Login to infellowship as a Leader
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Verify the social media information is not showing up on the roster
            // Figure out what row the individual is on
            var row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));

            test.Selenium.VerifyElementNotPresent(string.Format("//tr[{0}]/td[5]/a[1]/img[@alt='Facebook']", row + 1));
            test.Selenium.VerifyElementNotPresent(string.Format("//tr[{0}]/td[5]/a[2]/img[@alt='Twitter']", row + 1));
            test.Selenium.VerifyElementNotPresent(string.Format("//tr[{0}]/td[5]/a[3]/img[@alt='LinkedIn']", row + 1));

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the social media information is not showing up
            test.Selenium.VerifyElementNotPresent("//a[1]/img[@alt='Facebook']");
            test.Selenium.VerifyElementNotPresent("//a[2]/img[@alt='Linkedin']");
            test.Selenium.VerifyElementNotPresent("//a[3]/img[@alt='Twitter']");

            // Logout
            test.infellowship.Logout();


            // Login to infellowship as a Member
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Verify the social media information is not showing up on the roster
            row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));
            test.Selenium.VerifyElementNotPresent(string.Format("//tr[{0}]/td[5]/a[1]/img[@alt='Facebook']", row + 1));
            test.Selenium.VerifyElementNotPresent(string.Format("//tr[{0}]/td[5]/a[2]/img[@alt='Twitter']", row + 1));
            test.Selenium.VerifyElementNotPresent(string.Format("//tr[{0}]/td[5]/a[3]/img[@alt='LinkedIn']", row + 1));

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the social media information is not showing up
            test.Selenium.VerifyElementNotPresent("//a[1]/img[@alt='Facebook']");
            test.Selenium.VerifyElementNotPresent("//a[2]/img[@alt='Linkedin']");
            test.Selenium.VerifyElementNotPresent("//a[3]/img[@alt='Twitter']");

            // Logout
            test.infellowship.Logout();


            // Login to infellowship
            test.infellowship.Login("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // Verify social media is not present on the church directory
            infellowship_People_ChurchDirectory chDir = new infellowship_People_ChurchDirectory();
            test.infellowship.People_ChurchDirectory_Search("InFellowship Privacy");
            test.Selenium.VerifyElementNotPresent("//a[1]/img[@alt='Facebook']");
            test.Selenium.VerifyElementNotPresent("//a[2]/img[@alt='Twitter']");
            test.Selenium.VerifyElementNotPresent("//a[3]/img[@alt='LinkedIn']");

            test.infellowship.People_ChurchDirectory_View_Individual("InFellowship Privacy");
            test.Selenium.VerifyElementNotPresent("//a[1]/img[@alt='Facebook']");
            test.Selenium.VerifyElementNotPresent("//a[2]/img[@alt='Twitter']");
            test.Selenium.VerifyElementNotPresent("//a[3]/img[@alt='LinkedIn']");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure(Order = 17)]
        [MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can allow Leaders to view social media information")]
        public void PrivacySettings_Update_SocialMedia_Leaders() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Set the privacy settings to allow leaders to view social media information.
            test.infellowship.People_PrivacySettings_Update_SocialSetting(GeneralEnumerations.PrivacyLevels.Leaders);

            // Logout
            test.infellowship.Logout();


            // Login to infellowship as a Leader
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Verify the social media information is showing up on the roster
            // Figure out what row the individual is on
            var row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));

            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[5]/a[1]/img[@alt='Facebook']", row + 1));
            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[5]/a[2]/img[@alt='Twitter']", row + 1));
            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[5]/a[3]/img[@alt='LinkedIn']", row + 1));

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the social media information is showing up
            test.Selenium.VerifyElementPresent("//a[1]/img[@alt='Facebook']");
            test.Selenium.VerifyElementPresent("//a[2]/img[@alt='Linkedin']");
            test.Selenium.VerifyElementPresent("//a[3]/img[@alt='Twitter']");

            // Logout
            test.infellowship.Logout();


            // Login to infellowship as a Member
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Verify the social media information is not showing up on the roster
            row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));
            test.Selenium.VerifyElementNotPresent(string.Format("//tr[{0}]/td[5]/a[1]/img[@alt='Facebook']", row + 1));
            test.Selenium.VerifyElementNotPresent(string.Format("//tr[{0}]/td[5]/a[2]/img[@alt='Twitter']", row + 1));
            test.Selenium.VerifyElementNotPresent(string.Format("//tr[{0}]/td[5]/a[3]/img[@alt='LinkedIn']", row + 1));

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the social media information is not showing up
            test.Selenium.VerifyElementNotPresent("//a[1]/img[@alt='Facebook']");
            test.Selenium.VerifyElementNotPresent("//a[2]/img[@alt='Linkedin']");
            test.Selenium.VerifyElementNotPresent("//a[3]/img[@alt='Twitter']");

            // Logout
            test.infellowship.Logout();


            // Login to infellowship
            test.infellowship.Login("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // Verify social media is not present on the church directory
            infellowship_People_ChurchDirectory chDir = new infellowship_People_ChurchDirectory();
            test.infellowship.People_ChurchDirectory_Search("InFellowship Privacy");
            test.Selenium.VerifyElementNotPresent("//a[1]/img[@alt='Facebook']");
            test.Selenium.VerifyElementNotPresent("//a[2]/img[@alt='Twitter']");
            test.Selenium.VerifyElementNotPresent("//a[3]/img[@alt='LinkedIn']");

            test.infellowship.People_ChurchDirectory_View_Individual("InFellowship Privacy");
            test.Selenium.VerifyElementNotPresent("//a[1]/img[@alt='Facebook']");
            test.Selenium.VerifyElementNotPresent("//a[2]/img[@alt='Twitter']");
            test.Selenium.VerifyElementNotPresent("//a[3]/img[@alt='LinkedIn']");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure(Order = 18)]
        [MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can allow Members to view social media information")]
        public void PrivacySettings_Update_SocialMedia_Members() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Set the privacy settings to allow members to view social media information.
            test.infellowship.People_PrivacySettings_Update_SocialSetting(GeneralEnumerations.PrivacyLevels.Members);

            // Logout
            test.infellowship.Logout();


            // Login to infellowship as a Leader
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Verify the social media information is showing up on the roster
            // Figure out what row the individual is on
            var row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));

            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[5]/a[1]/img[@alt='Facebook']", row + 1));
            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[5]/a[2]/img[@alt='Twitter']", row + 1));
            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[5]/a[3]/img[@alt='LinkedIn']", row + 1));

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the social media information is showing up
            test.Selenium.VerifyElementPresent("//a[1]/img[@alt='Facebook']");
            test.Selenium.VerifyElementPresent("//a[2]/img[@alt='Linkedin']");
            test.Selenium.VerifyElementPresent("//a[3]/img[@alt='Twitter']");

            // Logout
            test.infellowship.Logout();


            // Login to infellowship as a Member
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Verify the social media information is showing up on the roster
            row = test.GeneralMethods.GetTableRowNumber(TableIds.InFellowship_Groups_Roster, "InFellowship Privacy", string.Format("Members ({0})", test.Selenium.GetXpathCount(string.Format("{0}/tbody/tr", TableIds.InFellowship_Groups_Roster)) - 1));
            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[5]/a[1]/img[@alt='Facebook']", row + 1));
            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[5]/a[2]/img[@alt='Twitter']", row + 1));
            test.Selenium.VerifyElementPresent(string.Format("//tr[{0}]/td[5]/a[3]/img[@alt='LinkedIn']", row + 1));

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the social media information is showing up
            test.Selenium.VerifyElementPresent("//a[1]/img[@alt='Facebook']");
            test.Selenium.VerifyElementPresent("//a[2]/img[@alt='Linkedin']");
            test.Selenium.VerifyElementPresent("//a[3]/img[@alt='Twitter']");

            // Logout
            test.infellowship.Logout();


            // Login to infellowship
            test.infellowship.Login("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // Verify social media is not present on the church directory
            infellowship_People_ChurchDirectory chDir = new infellowship_People_ChurchDirectory();
            test.infellowship.People_ChurchDirectory_Search("InFellowship Privacy");
            test.Selenium.VerifyElementNotPresent("//a[1]/img[@alt='Facebook']");
            test.Selenium.VerifyElementNotPresent("//a[2]/img[@alt='Twitter']");
            test.Selenium.VerifyElementNotPresent("//a[3]/img[@alt='LinkedIn']");

            test.infellowship.People_ChurchDirectory_View_Individual("InFellowship Privacy");
            test.Selenium.VerifyElementNotPresent("//a[1]/img[@alt='Facebook']");
            test.Selenium.VerifyElementNotPresent("//a[2]/img[@alt='Twitter']");
            test.Selenium.VerifyElementNotPresent("//a[3]/img[@alt='LinkedIn']");

            // Logout
            test.infellowship.Logout();
        }
        #endregion Social Media Privacy Settings

        #region Website Privacy Settings
        
        [Test, RepeatOnFailure(Order=21)]
        [MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can allow leaders and church staff to view website information")]
        public void PrivacySettings_Update_Websites_Leaders() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Set the privacy settings to allow only church staff and leaders to view website information.
            test.infellowship.People_PrivacySettings_Update_WebsiteSetting(GeneralEnumerations.PrivacyLevels.Leaders);

            // Logout
            test.infellowship.Logout();

            // Login to infellowship as a Leader
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the webiste is showing up
            test.Selenium.VerifyElementPresent("link=http://www.google.com");

            // Logout
            test.infellowship.Logout();

            // Login to infellowship as a Member
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the webiste is not showing up
            test.Selenium.VerifyElementNotPresent("link=http://www.google.com");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure(Order = 22)]
        [MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can allow leaders, members and church staff to view website information")]
        public void PrivacySettings_Update_Websites_Members() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Set the privacy settings to allow only church staff, members, and leaders to view website information.
            test.infellowship.People_PrivacySettings_Update_WebsiteSetting(GeneralEnumerations.PrivacyLevels.Members);

            // Logout
            test.infellowship.Logout();

            // Login to infellowship as a Leader
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the webiste is showing up
            test.Selenium.VerifyElementPresent("link=http://www.google.com");

            // Logout
            test.infellowship.Logout();

            // Login to infellowship as a Member
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the webiste is showing up
            test.Selenium.VerifyElementPresent("link=http://www.google.com");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure(Order = 23)]
        [MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can allow church staff to view website information")]
        public void PrivacySettings_Update_Websites_ChurchStaff() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Set the privacy settings to allow only church staff, members, and leaders to view website information.
            test.infellowship.People_PrivacySettings_Update_WebsiteSetting(GeneralEnumerations.PrivacyLevels.ChurchStaff);

            // Logout
            test.infellowship.Logout();

            // Login to infellowship as a Leader
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the webiste is not showing up
            test.Selenium.VerifyElementNotPresent("link=http://www.google.com");

            // Logout
            test.infellowship.Logout();

            // Login to infellowship as a Member
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the webiste is not showing up
            test.Selenium.VerifyElementNotPresent("link=http://www.google.com");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure(Order = 24)]
        [MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can allow everyone to view website information")]
        public void PrivacySettings_Update_Websites_Everyone() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Set the privacy settings to allow only church staff, members, and leaders to view website information.
            test.infellowship.People_PrivacySettings_Update_WebsiteSetting(GeneralEnumerations.PrivacyLevels.Everyone);

            // Logout
            test.infellowship.Logout();

            // Login to infellowship as a Leader
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the webiste is showing up
            test.Selenium.VerifyElementPresent("link=http://www.google.com");

            // Logout
            test.infellowship.Logout();

            // Login to infellowship as a Member
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the roster of a group
            test.infellowship.Groups_Group_View_Roster(_groupName);

            // Click on the individual
            test.Selenium.ClickAndWaitForPageToLoad("link=InFellowship Privacy");

            // Verify the webiste is showing up
            test.Selenium.VerifyElementPresent("link=http://www.google.com");

            // Logout
            test.infellowship.Logout();
        }

        #endregion Website Privacy Settings

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that the preview section on the privacy settings page updates correctly based on the temporary set privacy settings for address.")]
        public void PrivacySettings_View_Preview_Updates_Address() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Privacy Settings
            test.infellowship.People_PrivacySettings_View();

            // Set your privacy setting level at each tick for address, verifying the preview section updates correctly.
            // Church Staff
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Address, GeneralEnumerations.PrivacyLevels.ChurchStaff);

            // Church Staff preview
            Assert.AreEqual("2812 Meadow Wood Drive Flower Mound , TX  75022", test.Selenium.GetText("//tbody/tr/td[1]/div/p[2]").Replace("\n", ""));
            Assert.AreNotEqual("Flower Mound , TX", test.Selenium.GetText("//tbody/tr/td[1]/div/p[2]").Replace("\n", "").Replace("-", ""));

            // Leaders preview
            Assert.AreNotEqual("2812 Meadow Wood Drive Flower Mound, TX  75022", test.Selenium.GetText("//tbody/tr/td[2]/div/p[2]").Replace("\n", ""));
            Assert.AreEqual("Flower Mound , TX", test.Selenium.GetText("//tbody/tr/td[2]/div/p[2]").Replace("\n", ""));

            // Members preview
            Assert.AreNotEqual("2812 Meadow Wood Drive Flower Mound, TX  75022", test.Selenium.GetText("//tbody/tr/td[3]/div/p[2]").Replace("\n", ""));
            Assert.AreEqual("Flower Mound , TX", test.Selenium.GetText("//tbody/tr/td[3]/div/p[2]").Replace("\n", ""));

            // Everyone preview
            Assert.AreNotEqual("2812 Meadow Wood Drive Flower Mound, TX  75022", test.Selenium.GetText("//tbody/tr/td[4]/div/p[2]").Replace("\n", ""));
            Assert.AreEqual("Flower Mound , TX", test.Selenium.GetText("//tbody/tr/td[4]/div/p[2]").Replace("\n", ""));
             
            // Leaders
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Address, GeneralEnumerations.PrivacyLevels.Leaders);

            // Church Staff preview
            Assert.AreEqual("2812 Meadow Wood Drive Flower Mound , TX  75022", test.Selenium.GetText("//tbody/tr/td[1]/div/p[2]").Replace("\n", ""));
            Assert.AreNotEqual("Flower Mound , TX", test.Selenium.GetText("//tbody/tr/td[1]/div/p[2]"));

            // Leaders preview
            Assert.AreEqual("2812 Meadow Wood Drive Flower Mound , TX  75022", test.Selenium.GetText("//tbody/tr/td[2]/div/p[2]").Replace("\n", ""));
            Assert.AreNotEqual("Flower Mound , TX", test.Selenium.GetText("//tbody/tr/td[2]/div/p[2]"));

            // Members preview
            Assert.AreNotEqual("2812 Meadow Wood Drive Flower Mound , TX  75022", test.Selenium.GetText("//tbody/tr/td[3]/div/p[2]").Replace("\n", ""));
            Assert.AreEqual("Flower Mound , TX", test.Selenium.GetText("//tbody/tr/td[3]/div/p[2]"));

            // Everyone preview
            Assert.AreNotEqual("2812 Meadow Wood Drive Flower Mound , TX  75022", test.Selenium.GetText("//tbody/tr/td[4]/div/p[2]").Replace("\n", ""));
            Assert.AreEqual("Flower Mound , TX", test.Selenium.GetText("//tbody/tr/td[4]/div/p[2]"));

            // Members
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Address, GeneralEnumerations.PrivacyLevels.Members);

            // Church Staff preview
            Assert.AreEqual("2812 Meadow Wood Drive Flower Mound , TX  75022", test.Selenium.GetText("//tbody/tr/td[1]/div/p[2]").Replace("\n", ""));
            Assert.AreNotEqual("Flower Mound , TX", test.Selenium.GetText("//tbody/tr/td[1]/div/p[2]"));

            // Leaders preview
            Assert.AreEqual("2812 Meadow Wood Drive Flower Mound , TX  75022", test.Selenium.GetText("//tbody/tr/td[2]/div/p[2]").Replace("\n", ""));
            Assert.AreNotEqual("Flower Mound , TX", test.Selenium.GetText("//tbody/tr/td[2]/div/p[2]"));

            // Members preview
            Assert.AreEqual("2812 Meadow Wood Drive Flower Mound , TX  75022", test.Selenium.GetText("//tbody/tr/td[3]/div/p[2]").Replace("\n", ""));
            Assert.AreNotEqual("Flower Mound , TX", test.Selenium.GetText("//tbody/tr/td[3]/div/p[2]"));

            // Everyone preview
            Assert.AreNotEqual("2812 Meadow Wood Drive Flower Mound , TX  75022", test.Selenium.GetText("//tbody/tr/td[4]/div/p[2]").Replace("\n", ""));
            Assert.AreEqual("Flower Mound , TX", test.Selenium.GetText("//tbody/tr/td[4]/div/p[2]"));

            // Everyone
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Address, GeneralEnumerations.PrivacyLevels.Everyone);

            // Church Staff preview
            Assert.AreEqual("2812 Meadow Wood Drive Flower Mound , TX  75022", test.Selenium.GetText("//tbody/tr/td[1]/div/p[2]").Replace("\n", ""));
            Assert.AreNotEqual("Flower Mound , TX", test.Selenium.GetText("//tbody/tr/td[1]/div/p[2]"));

            // Leaders preview
            Assert.AreEqual("2812 Meadow Wood Drive Flower Mound , TX  75022", test.Selenium.GetText("//tbody/tr/td[2]/div/p[2]").Replace("\n", ""));
            Assert.AreNotEqual("Flower Mound , TX", test.Selenium.GetText("//tbody/tr/td[2]/div/p[2]"));

            // Members preview
            Assert.AreEqual("2812 Meadow Wood Drive Flower Mound , TX  75022", test.Selenium.GetText("//tbody/tr/td[3]/div/p[2]").Replace("\n", ""));
            Assert.AreNotEqual("Flower Mound , TX", test.Selenium.GetText("//tbody/tr/td[3]/div/p[2]"));

            // Everyone preview
            Assert.AreEqual("2812 Meadow Wood Drive Flower Mound , TX  75022", test.Selenium.GetText("//tbody/tr/td[4]/div/p[2]").Replace("\n", ""));
            Assert.AreNotEqual("Flower Mound , TX", test.Selenium.GetText("//tbody/tr/td[4]/div/p[2]"));

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that the preview section on the privacy settings page updates correctly based on the temporary set privacy settings for date of birth.")]
        public void PrivacySettings_View_Preview_Updates_DateOfBirth() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login ...");
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Privacy Settings
            TestLog.WriteLine("View Privacy Settings");
            test.infellowship.People_PrivacySettings_View();

            // Set your privacy setting level at each tick for birthdate, verifying the preview section updates correctly.
            // Church Staff
            TestLog.WriteLine("People_PrivacySettings_SetPrivacyLevel - ChurchStaff");
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Birthdate, GeneralEnumerations.PrivacyLevels.ChurchStaff);

            // Church Staff preview
            TestLog.WriteLine("Verify Church Staff Preview");
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[1]/div/p[3]"), "Birthdate: November 15, 1985");
            Assert.AreNotEqual(test.Selenium.GetText("//tbody/tr/td[1]/div/p[3]"), "Birthdate: November 15");

            // Leaders preview
            TestLog.WriteLine("Verify Leaders Preview");
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[2]/div/p[3]");

            // Members preview
            TestLog.WriteLine("Verify Members Preview");
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[3]/div/p[3]");

            // Everyone preview
            TestLog.WriteLine("Verify Everyone Preview");
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[4]/div/p[3]");

            // Leaders
            TestLog.WriteLine("People_PrivacySettings_SetPrivacyLevel - Leaders");
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Birthdate, GeneralEnumerations.PrivacyLevels.Leaders);

            // Church Staff preview
            TestLog.WriteLine("Verify Church Staff Preview");
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[1]/div/p[3]"), "Birthdate: November 15, 1985");
            Assert.AreNotEqual(test.Selenium.GetText("//tbody/tr/td[1]/div/p[3]"), "Birthdate: November 15");

            // Leaders preview
            TestLog.WriteLine("Verify Leaders Preview");
            Assert.AreNotEqual(test.Selenium.GetText("//tbody/tr/td[2]/div/p[3]"), "Birthdate: November 15, 1985");
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[2]/div/p[3]"), "Birthdate: November 15");

            // Members preview
            TestLog.WriteLine("Verify Members Preview");
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[3]/div/p[3]");

            // Everyone preview
            TestLog.WriteLine("Verify Everyone Preview");
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[4]/div/p[3]");

            // Members
            TestLog.WriteLine("People_PrivacySettings_SetPrivacyLevel - Members");
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Birthdate, GeneralEnumerations.PrivacyLevels.Members);

            // Church Staff preview
            TestLog.WriteLine("Verify Church Staff Perview");
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[1]/div/p[3]"), "Birthdate: November 15, 1985");
            Assert.AreNotEqual(test.Selenium.GetText("//tbody/tr/td[1]/div/p[3]"), "Birthdate: November 15");

            // Leaders preview
            TestLog.WriteLine("Verify Leaders Preview");
            Assert.AreNotEqual(test.Selenium.GetText("//tbody/tr/td[2]/div/p[3]"), "Birthdate: November 15, 1985");
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[2]/div/p[3]"), "Birthdate: November 15");

            // Members preview
            TestLog.WriteLine("Verify Members Preview");
            Assert.AreNotEqual(test.Selenium.GetText("//tbody/tr/td[3]/div/p[3]"), "Birthdate: November 15, 1985");
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[3]/div/p[3]"), "Birthdate: November 15");

            // Everyone preview
            TestLog.WriteLine("Verify EVeryone Preview");
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[4]/div/p[3]");

            // Everyone
            TestLog.WriteLine("People_PrivacySettings_SetPrivacyLevel - Everyone");
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Birthdate, GeneralEnumerations.PrivacyLevels.Everyone);

            // Church Staff preview
            TestLog.WriteLine("Verify Church Staff Preview");
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[1]/div/p[3]"), "Birthdate: November 15, 1985");
            Assert.AreNotEqual(test.Selenium.GetText("//tbody/tr/td[1]/div/p[3]"), "Birthdate: November 15");

            // Leaders preview
            TestLog.WriteLine("Verify Leaders Preview");
            Assert.AreNotEqual(test.Selenium.GetText("//tbody/tr/td[2]/div/p[3]"), "Birthdate: November 15, 1985");
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[2]/div/p[3]"), "Birthdate: November 15");

            // Members preview
            TestLog.WriteLine("Verify Members Preview");
            Assert.AreNotEqual(test.Selenium.GetText("//tbody/tr/td[3]/div/p[3]"), "Birthdate: November 15, 1985");
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[3]/div/p[3]"), "Birthdate: November 15");

            // Everyone preview
            TestLog.WriteLine("Verify Everyone Preview");
            Assert.AreNotEqual(test.Selenium.GetText("//tbody/tr/td[4]/div/p[3]"), "Birthdate: November 15, 1985");
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[4]/div/p[3]"), "Birthdate: November 15"); ;

            // Logout
            TestLog.WriteLine("Logout ..");
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that the preview section on the privacy settings page updates correctly based on the temporary set privacy settings for email.")]
        public void PrivacySettings_View_Preview_Updates_Email() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Privacy Settings
            test.infellowship.People_PrivacySettings_View();

            // Set your privacy setting level at each tick for email, verifying the preview section updates correctly.
            // Church Staff
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Email, GeneralEnumerations.PrivacyLevels.ChurchStaff);

            // Church Staff preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[1]/div/p[4]"), "infellowship.privacy@gmail.com");

            // Leaders preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[2]/div/p[4]");

            // Members preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[3]/div/p[4]");

            // Everyone preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[4]/div/p[4]");

            // Leaders
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Email, GeneralEnumerations.PrivacyLevels.Leaders);

            // Church Staff preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[1]/div/p[4]"), "infellowship.privacy@gmail.com");

            // Leaders preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[2]/div/p[4]"), "infellowship.privacy@gmail.com");

            // Members preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[3]/div/p[4]");

            // Everyone preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[4]/div/p[4]");

            // Members
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Email, GeneralEnumerations.PrivacyLevels.Members);

            // Church Staff preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[1]/div/p[4]"), "infellowship.privacy@gmail.com");

            // Leaders preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[2]/div/p[4]"), "infellowship.privacy@gmail.com");

            // Members preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[3]/div/p[4]"), "infellowship.privacy@gmail.com");

            // Everyone preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[4]/div/p[4]");

            // Everyone
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Email, GeneralEnumerations.PrivacyLevels.Everyone);

            // Church Staff preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[1]/div/p[4]"), "infellowship.privacy@gmail.com");

            // Leaders preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[2]/div/p[4]"), "infellowship.privacy@gmail.com");

            // Members preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[3]/div/p[4]"), "infellowship.privacy@gmail.com");

            // Everyone preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[4]/div/p[4]"), "infellowship.privacy@gmail.com");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that the preview section on the privacy settings page updates correctly based on the temporary set privacy settings for phone.")]
        public void PrivacySettings_View_Preview_Updates_Phone() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Privacy Settings
            test.infellowship.People_PrivacySettings_View();

            // Set your privacy setting level at each tick for phone, verifying the preview section updates correctly.
            // Church Staff
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Phone, GeneralEnumerations.PrivacyLevels.ChurchStaff);

            // Church Staff preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[1]/div/p[5]"), "555-555-5555");

            // Leaders preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[2]/div/p[5]");

            // Members preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[3]/div/p[5]");

            // Everyone preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[4]/div/p[5]");

            // Leaders
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Phone, GeneralEnumerations.PrivacyLevels.Leaders);

            // Church Staff preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[1]/div/p[5]"), "555-555-5555");

            // Leaders preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[2]/div/p[5]"), "555-555-5555");

            // Members preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[3]/div/p[5]");

            // Everyone preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[4]/div/p[5]");

            // Members
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Phone, GeneralEnumerations.PrivacyLevels.Members);

            // Church Staff preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[1]/div/p[5]"), "555-555-5555");

            // Leaders preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[2]/div/p[5]"), "555-555-5555");

            // Members preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[3]/div/p[5]"), "555-555-5555");

            // Everyone preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[4]/div/p[5]");

            // Everyone
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Phone, GeneralEnumerations.PrivacyLevels.Everyone);

            // Church Staff preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[1]/div/p[5]"), "555-555-5555");

            // Leaders preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[2]/div/p[5]"), "555-555-5555");

            // Members preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[3]/div/p[5]"), "555-555-5555");

            // Everyone preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[4]/div/p[5]"), "555-555-5555");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that the preview section on the privacy settings page updates correctly based on the temporary set privacy settings for websites.")]
        public void PrivacySettings_View_Preview_Updates_Websites() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Privacy Settings
            test.infellowship.People_PrivacySettings_View();

            // Set your privacy setting level at each tick for websites, verifying the preview section updates correctly.
            // Church Staff
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Websites, GeneralEnumerations.PrivacyLevels.ChurchStaff);

            // Church Staff preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[1]/div/p[6]"), "http://www.google.com");

            // Leaders preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[2]/div/p[6]");

            // Members preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[3]/div/p[6]");

            // Everyone preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[4]/div/p[6]");

            // Leaders
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Websites, GeneralEnumerations.PrivacyLevels.Leaders);

            // Church Staff preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[1]/div/p[6]"), "http://www.google.com");

            // Leaders preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[2]/div/p[6]"), "http://www.google.com");

            // Members preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[3]/div/p[6]");

            // Everyone preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[4]/div/p[6]");

            // Members
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Websites, GeneralEnumerations.PrivacyLevels.Members);

            // Church Staff preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[1]/div/p[6]"), "http://www.google.com");

            // Leaders preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[2]/div/p[6]"), "http://www.google.com");

            // Members preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[3]/div/p[6]"), "http://www.google.com");

            // Everyone preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[4]/div/p[6]");

            // Everyone
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Websites, GeneralEnumerations.PrivacyLevels.Everyone);

            // Church Staff preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[1]/div/p[6]"), "http://www.google.com");

            // Leaders preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[2]/div/p[6]"), "http://www.google.com");

            // Members preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[3]/div/p[6]"), "http://www.google.com");

            // Everyone preview
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr/td[4]/div/p[6]"), "http://www.google.com");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that the preview section on the privacy settings page updates correctly based on the temporary set privacy settings for social media.")]
        public void PrivacySettings_View_Preview_Updates_SocialMedia() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Privacy Settings
            test.infellowship.People_PrivacySettings_View();

            // Set your privacy setting level at each tick for social media, verifying the preview section updates correctly.
            // Church Staff
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Social, GeneralEnumerations.PrivacyLevels.ChurchStaff);

            // Church Staff preview
            test.Selenium.VerifyElementPresent("//tbody/tr/td[1]/div/p[7]/a[1]"); // Twitter
            test.Selenium.VerifyElementPresent("//tbody/tr/td[1]/div/p[7]/a[2]"); // Facebook
            test.Selenium.VerifyElementPresent("//tbody/tr/td[1]/div/p[7]/a[3]"); // LinkedIn

            // Leaders preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[2]/div/p[7]/a[1]"); // Twitter
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[2]/div/p[7]/a[2]"); // Facebook
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[2]/div/p[7]/a[3]"); // LinkedIn

            // Members preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[3]/div/p[7]/a[1]"); // Twitter
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[3]/div/p[7]/a[2]"); // Facebook
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[3]/div/p[7]/a[3]"); // LinkedIn

            // Everyone preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[4]/div/p[7]/a[1]"); // Twitter
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[4]/div/p[7]/a[2]"); // Facebook
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[4]/div/p[7]/a[3]"); // LinkedIn

            // Leaders
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Social, GeneralEnumerations.PrivacyLevels.Leaders);

            // Church Staff preview
            test.Selenium.VerifyElementPresent("//tbody/tr/td[1]/div/p[7]/a[1]"); // Twitter
            test.Selenium.VerifyElementPresent("//tbody/tr/td[1]/div/p[7]/a[2]"); // Facebook
            test.Selenium.VerifyElementPresent("//tbody/tr/td[1]/div/p[7]/a[3]"); // LinkedIn

            // Leaders preview
            test.Selenium.VerifyElementPresent("//tbody/tr/td[2]/div/p[7]/a[1]"); // Twitter
            test.Selenium.VerifyElementPresent("//tbody/tr/td[2]/div/p[7]/a[2]"); // Facebook
            test.Selenium.VerifyElementPresent("//tbody/tr/td[2]/div/p[7]/a[3]"); // LinkedIn

            // Members preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[3]/div/p[7]/a[1]"); // Twitter
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[3]/div/p[7]/a[2]"); // Facebook
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[3]/div/p[7]/a[3]"); // LinkedIn

            // Everyone preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[4]/div/p[7]/a[1]"); // Twitter
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[4]/div/p[7]/a[2]"); // Facebook
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[4]/div/p[7]/a[3]"); // LinkedIn

            // Members
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Social, GeneralEnumerations.PrivacyLevels.Members);

            // Church Staff preview
            test.Selenium.VerifyElementPresent("//tbody/tr/td[1]/div/p[7]/a[1]"); // Twitter
            test.Selenium.VerifyElementPresent("//tbody/tr/td[1]/div/p[7]/a[2]"); // Facebook
            test.Selenium.VerifyElementPresent("//tbody/tr/td[1]/div/p[7]/a[3]"); // LinkedIn

            // Leaders preview
            test.Selenium.VerifyElementPresent("//tbody/tr/td[2]/div/p[7]/a[1]"); // Twitter
            test.Selenium.VerifyElementPresent("//tbody/tr/td[2]/div/p[7]/a[2]"); // Facebook
            test.Selenium.VerifyElementPresent("//tbody/tr/td[2]/div/p[7]/a[3]"); // LinkedIn

            // Members preview
            test.Selenium.VerifyElementPresent("//tbody/tr/td[3]/div/p[7]/a[1]"); // Twitter
            test.Selenium.VerifyElementPresent("//tbody/tr/td[3]/div/p[7]/a[2]"); // Facebook
            test.Selenium.VerifyElementPresent("//tbody/tr/td[3]/div/p[7]/a[3]"); // LinkedIn

            // Everyone preview
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[4]/div/p[7]/a[1]"); // Twitter
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[4]/div/p[7]/a[2]"); // Facebook
            test.Selenium.VerifyIsNotVisible("//tbody/tr/td[4]/div/p[7]/a[3]"); // LinkedIn

            // Everyone
            test.infellowship.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Social, GeneralEnumerations.PrivacyLevels.Everyone);

            // Church Staff preview
            test.Selenium.VerifyElementPresent("//tbody/tr/td[1]/div/p[7]/a[1]"); // Twitter
            test.Selenium.VerifyElementPresent("//tbody/tr/td[1]/div/p[7]/a[2]"); // Facebook
            test.Selenium.VerifyElementPresent("//tbody/tr/td[1]/div/p[7]/a[3]"); // LinkedIn

            // Leaders preview
            test.Selenium.VerifyElementPresent("//tbody/tr/td[2]/div/p[7]/a[1]"); // Twitter
            test.Selenium.VerifyElementPresent("//tbody/tr/td[2]/div/p[7]/a[2]"); // Facebook
            test.Selenium.VerifyElementPresent("//tbody/tr/td[2]/div/p[7]/a[3]"); // LinkedIn

            // Members preview
            test.Selenium.VerifyElementPresent("//tbody/tr/td[3]/div/p[7]/a[1]"); // Twitter
            test.Selenium.VerifyElementPresent("//tbody/tr/td[3]/div/p[7]/a[2]"); // Facebook
            test.Selenium.VerifyElementPresent("//tbody/tr/td[3]/div/p[7]/a[3]"); // LinkedIn

            // Everyone preview
            test.Selenium.VerifyElementPresent("//tbody/tr/td[4]/div/p[7]/a[1]"); // Twitter
            test.Selenium.VerifyElementPresent("//tbody/tr/td[4]/div/p[7]/a[2]"); // Facebook
            test.Selenium.VerifyElementPresent("//tbody/tr/td[4]/div/p[7]/a[3]"); // LinkedIn

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that the privacy settings page loads.")]
        [Importance(Importance.Critical)]
        public void PrivacySettings_View() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.privacy@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Privacy Settings page
            test.infellowship.People_PrivacySettings_View();

            // Verify the page loads

            // Header text
            test.Selenium.VerifyTextPresent("Privacy Settings");
            test.Selenium.VerifyTextPresent("Drag the sliders to select what info people will see. The further you move each slider, the more people will be able to see that info online.");

            // Sliders
            test.Selenium.VerifyElementPresent(InFellowshipConstants.People.PrivacySettingsConstants.Slider_Address);
            test.Selenium.VerifyElementPresent(InFellowshipConstants.People.PrivacySettingsConstants.Slider_Birthdate);
            test.Selenium.VerifyElementPresent(InFellowshipConstants.People.PrivacySettingsConstants.Slider_Email);
            test.Selenium.VerifyElementPresent(InFellowshipConstants.People.PrivacySettingsConstants.Slider_Websites);
            test.Selenium.VerifyElementPresent(InFellowshipConstants.People.PrivacySettingsConstants.Slider_Social);

            // Privacy Preview
            test.Selenium.VerifyElementPresent("//table[@id='privacy_preview']");

            // Note about leaders
            test.Selenium.VerifyTextPresent("Note: Leaders can view personal information if the church allows them to edit your profile.");

            // Checkbox to opt into or opt out of the Church Directory
            test.Selenium.VerifyElementPresent(InFellowshipConstants.People.PrivacySettingsConstants.Checkbox_IncludeInDirectory);

            // Submit button
            test.Selenium.VerifyElementPresent(InFellowshipConstants.People.PrivacySettingsConstants.Button_Submit);

            // Logout
            test.infellowship.Logout();
        }
    }
    [TestFixture]
    public class Infellowship_PrivacySettings_PrivacyPolicyWebDriver : FixtureBaseWebDriver
    {

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Stuart Platt")]
        [Description("Verify Privacy Policy")]
        public void Infellowship_PrivacyPolicy()
        {
            //Login to Weblink
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.Login();

            //Verify Privacy Policy
            Assert.IsTrue(test.Driver.FindElement(By.LinkText("Your Privacy Rights")).Displayed, "No Privacy Policy Found");
            IWebElement privacyElement = test.Driver.FindElementByLinkText("Your Privacy Rights");
            Assert.Contains(privacyElement.GetAttribute("href"), "http://www.activenetwork.com/information/privacy-policy.htm", "Privacy Policy Link Not Found");
            privacyElement.Click();

            TestLog.WriteLine("Privacy Policy Found");
            Assert.AreEqual(test.Driver.Title, "Your Privacy Rights", "Did Not Go To Privacy Policy Page");
            test.GeneralMethods.VerifyTextPresentWebDriver("Your Privacy Rights");

            // Close the Weblink Session
            test.Driver.Close();

        }
    }
}