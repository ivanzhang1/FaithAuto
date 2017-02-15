
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Selenium;

namespace FTTests.infellowship.People {
    [TestFixture]
    public class infellowship_People_ProfileEditor : FixtureBase {
        private string _groupTypeName = "Profile Editor Group Type";
        private string _groupName = "General Profile Editor Group";

        [FixtureSetUp]
        public void FixtureSetUp() {
            // Create a generic group and add a leader and member
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", _groupTypeName, new List<int> { 1, 2, 3, 4, 5, 9, 10, 11, 12 });
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Member", "Member");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "InFellowship User", "Member");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "InFellowship Privacy", "Member");
        }

        [FixtureTearDown]
        public void FixtureTearDown() {
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
        }


        #region Updating Information

        #region Personal Information
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Verifies the available marital statues on the profile editor page.")]
        public void ProfileEditor_View() {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the profile editor
            test.infellowship.People_ProfileEditor_View_UpdatePage();

            // Verify the selectable marital statuses
            Assert.AreEqual(8, test.Selenium.GetXpathCount("//select[@id='marital_status']/option"));
            Assert.AreEqual(string.Empty, test.Selenium.GetText("//select[@id='marital_status']/option[1]"));
            Assert.AreEqual("Child/Yth", test.Selenium.GetText("//select[@id='marital_status']/option[2]"));
            Assert.AreEqual("Divorced", test.Selenium.GetText("//select[@id='marital_status']/option[3]"));
            Assert.AreEqual("Married", test.Selenium.GetText("//select[@id='marital_status']/option[4]"));
            Assert.AreEqual("Separated", test.Selenium.GetText("//select[@id='marital_status']/option[5]"));
            Assert.AreEqual("Single", test.Selenium.GetText("//select[@id='marital_status']/option[6]"));
            Assert.AreEqual("Widow", test.Selenium.GetText("//select[@id='marital_status']/option[7]"));
            Assert.AreEqual("Widower", test.Selenium.GetText("//select[@id='marital_status']/option[8]"));

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies visitors are not present when you update your profile.")]
        public void ProfileEditor_View_Visitors_Not_Present() {
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

            // View the update page in Profile Editor
            test.infellowship.People_ProfileEditor_View_UpdatePage();

            // Verify Visitors are not present
            Assert.IsFalse(test.Selenium.IsTextPresent("Visitor Sneeden"));

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can update your first and last name.")]
        public void ProfileEditor_Update_First_And_Last_Name() {
            // Initial data
            var firstName = Guid.NewGuid().ToString().Substring(0, 8);
            var lastName = "UpdateProfile";
            var individualName = firstName + " " + lastName;
            var email = "profileupdate.name@fellowshiptech.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);           
            base.SQL.People_Individual_Create(254, firstName, lastName);
            base.SQL.People_InFellowshipAccount_Create(254, individualName, email, "BM.Admin09");

            try
            {
                // Login to infellowship
                TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

                // Update your First Name and Last Name
                test.infellowship.People_ProfileEditor_Update_Address("United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", null);
                test.infellowship.People_ProfileEditor_Update_PersonalInfo("Bob", "Weblink", null, GeneralEnumerations.HouseholdPositonWeb.Husband, "11/15/1970", GeneralEnumerations.MaritalStatus.NA, GeneralEnumerations.Gender.NA);

                // Opt in to the directory
                test.infellowship.People_PrivacySettings_Update_OptInToDirectory(true);

                // Verify your first and last name was updated
                // View page
                test.infellowship.People_ProfileEditor_View();
                test.Selenium.VerifyTextPresent("Bob Weblink");
                Assert.IsFalse(test.Selenium.IsTextPresent(individualName));

                // Update page
                test.infellowship.People_ProfileEditor_View_UpdatePage();
                Assert.AreEqual("Bob", test.Selenium.GetValue("first_name"));
                Assert.AreEqual("Weblink", test.Selenium.GetValue("last_name"));

                // Church Directory
                test.infellowship.People_ChurchDirectory_View_Individual("Bob Weblink");
                test.Selenium.VerifyTextPresent("Bob Weblink");
                Assert.IsFalse(test.Selenium.IsTextPresent(individualName));

                test.infellowship.People_ChurchDirectory_Search(individualName);
                test.Selenium.VerifyElementNotPresent(string.Format("link={0}", individualName));

                // Revert the chages
                test.infellowship.People_ProfileEditor_Update_PersonalInfo(firstName, lastName, null, GeneralEnumerations.HouseholdPositonWeb.NA, null, GeneralEnumerations.MaritalStatus.NA, GeneralEnumerations.Gender.NA);

                // Verify your first and last name was reverted
                // View page
                test.infellowship.People_ProfileEditor_View();
                Assert.IsFalse(test.Selenium.IsTextPresent("Bob Weblink"));
                test.Selenium.VerifyTextPresent(individualName);

                // Update page
                test.infellowship.People_ProfileEditor_View_UpdatePage();
                Assert.AreEqual(firstName, test.Selenium.GetValue("first_name"));
                Assert.AreEqual(lastName, test.Selenium.GetValue("last_name"));

                // Church Directory
                test.infellowship.People_ChurchDirectory_Search("ProfileUpdate Name");
                test.Selenium.VerifyElementPresent(string.Format("link={0}", individualName));
                test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", individualName));
                test.Selenium.VerifyTextPresent(individualName);
                Assert.IsFalse(test.Selenium.IsTextPresent("Bob Weblink"));

                test.infellowship.People_ChurchDirectory_Search("Bob Weblink");
                test.Selenium.VerifyElementNotPresent("link=Bob Weblink");

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Clean up
                base.SQL.People_InFellowshipAccount_Delete(254, email);
                base.SQL.People_MergeIndividual(254, individualName, "Merge Dump");
            }
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can update your goes by name.")]
        public void ProfileEditor_Update_GoesBy_Name() {
            // Initial data
            var firstName = Guid.NewGuid().ToString().Substring(0, 8);
            var lastName = "User";
            var individualName = firstName + " " + lastName;
            var email = "profileupdate.goesby@fellowshiptech.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);
            base.SQL.People_Individual_Create(254, firstName, lastName);
            base.SQL.People_InFellowshipAccount_Create(254, individualName, email, "BM.Admin09");

            try
            {
                // Login to infellowship
                TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

                // Update your Goes By Name
                test.infellowship.People_ProfileEditor_Update_Address("United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", null);
                test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, "Barney", GeneralEnumerations.HouseholdPositonWeb.NA, "11/15/1970", GeneralEnumerations.MaritalStatus.NA, GeneralEnumerations.Gender.NA);

                // Opt in to the directory
                test.infellowship.People_PrivacySettings_Update_OptInToDirectory(true);


                // Verify your goes by name was updated
                // View page
                test.infellowship.People_ProfileEditor_View();
                Assert.IsTrue(test.Selenium.IsTextPresent("Barney"));

                // Update page
                test.infellowship.People_ProfileEditor_View_UpdatePage();
                Assert.AreEqual("Barney", test.Selenium.GetValue("goes_by"));

                // Church Directory
                test.infellowship.People_ChurchDirectory_Search("Barney User");
                test.Selenium.VerifyElementPresent("link=Barney User");
                test.Selenium.VerifyElementNotPresent(string.Format("link={0}", individualName));
                test.Selenium.ClickAndWaitForPageToLoad("link=Barney User");
                test.Selenium.VerifyTextPresent("Barney User");
                Assert.IsFalse(test.Selenium.IsTextPresent(individualName));

                // Revert the chages
                test.infellowship.People_ProfileEditor_Delete_GoesByName();

                // Verify your goes by name was reverted
                // View page
                test.infellowship.People_ProfileEditor_View();
                Assert.IsFalse(test.Selenium.IsTextPresent("Barney"));

                // Update page
                test.infellowship.People_ProfileEditor_View_UpdatePage();
                Assert.AreEqual("", test.Selenium.GetValue("goes_by"));

                // Church Directory
                test.infellowship.People_ChurchDirectory_Search(individualName);
                test.Selenium.VerifyElementPresent(string.Format("link={0}", individualName));
                test.Selenium.VerifyElementNotPresent("link=Barney User");
                test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", individualName));
                test.Selenium.VerifyTextPresent(individualName);
                Assert.IsFalse(test.Selenium.IsTextPresent("Barney User"));

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Clean up
                base.SQL.People_InFellowshipAccount_Delete(254, email);
                base.SQL.People_MergeIndividual(254, individualName, "Merge Dump");
            }
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Mady Kou")]
        [Description("Verifies you can update your goes by name max to 30 characters")]
        public void ProfileEditor_Update_LongGoesBy_Name()
        {
            // Initial data
            var firstName = Guid.NewGuid().ToString().Substring(0, 8);
            var lastName = "User";
            var individualName = firstName + " " + lastName;
            var email = "profileupdate.goesbylong@fellowshiptech.com";
            string goesBy = "aaaaaaaaaabbbbbbbbbbccccccccccddddd";
            base.SQL.People_InFellowshipAccount_Delete(254, email);
            base.SQL.People_Individual_Create(254, firstName, lastName);
            base.SQL.People_InFellowshipAccount_Create(254, individualName, email, "BM.Admin09");

            try
            {
                // Login to infellowship
                TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

                // Update your Goes By Name
                test.infellowship.People_ProfileEditor_Update_Address("United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", null);
                test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, goesBy, GeneralEnumerations.HouseholdPositonWeb.NA, "11/15/1970", GeneralEnumerations.MaritalStatus.NA, GeneralEnumerations.Gender.NA);

                // Opt in to the directory
                test.infellowship.People_PrivacySettings_Update_OptInToDirectory(true);


                // Verify your goes by name was updated
                // View page
                test.infellowship.People_ProfileEditor_View();
                Assert.IsTrue(test.Selenium.IsTextPresent(goesBy.Substring(0, 30)));

                // Update page
                test.infellowship.People_ProfileEditor_View_UpdatePage();
                Assert.AreEqual(goesBy.Substring(0, 30), test.Selenium.GetValue("goes_by"));

                // Church Directory
                test.infellowship.People_ChurchDirectory_Search(goesBy.Substring(0, 30) + " User");
                test.Selenium.VerifyElementPresent("link=" + goesBy.Substring(0, 30) + " User");
                test.Selenium.VerifyElementNotPresent(string.Format("link={0}", individualName));
                test.Selenium.ClickAndWaitForPageToLoad("link=" + goesBy.Substring(0, 30) + " User");
                test.Selenium.VerifyTextPresent(goesBy.Substring(0, 30) + " User");
                Assert.IsFalse(test.Selenium.IsTextPresent(goesBy));
                Assert.IsFalse(test.Selenium.IsTextPresent(individualName));

                // Revert the chages
                test.infellowship.People_ProfileEditor_Delete_GoesByName();

                // Verify your goes by name was reverted
                // View page
                test.infellowship.People_ProfileEditor_View();
                Assert.IsFalse(test.Selenium.IsTextPresent(goesBy.Substring(0, 30)));
                Assert.IsFalse(test.Selenium.IsTextPresent(goesBy));

                // Update page
                test.infellowship.People_ProfileEditor_View_UpdatePage();
                Assert.AreEqual("", test.Selenium.GetValue("goes_by"));

                // Church Directory
                test.infellowship.People_ChurchDirectory_Search(individualName);
                test.Selenium.VerifyElementPresent(string.Format("link={0}", individualName));
                test.Selenium.VerifyElementNotPresent("link=" + goesBy.Substring(0, 30) + " User");
                test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", individualName));
                test.Selenium.VerifyTextPresent(individualName);
                Assert.IsFalse(test.Selenium.IsTextPresent(goesBy.Substring(0, 30) + " User"));

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Clean up
                base.SQL.People_InFellowshipAccount_Delete(254, email);
                base.SQL.People_MergeIndividual(254, individualName, "Merge Dump");
            }
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can update your household position.")]
        public void ProfileEditor_Update_Household_Position() {
            // Initial data
            var firstName = Guid.NewGuid().ToString().Substring(0, 8);
            var lastName = "Household";
            var individualName = firstName + " " + lastName;
            var email = "profileupdate.household@fellowshiptech.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);
            base.SQL.People_Individual_Create(254, firstName, lastName);
            base.SQL.People_InFellowshipAccount_Create(254, individualName, email, "BM.Admin09");

            try
            {
                // Login to infellowship
                TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

                // Update your Household Position
                test.infellowship.People_ProfileEditor_Update_Address("United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", null);
                test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.Other, null, GeneralEnumerations.MaritalStatus.NA, GeneralEnumerations.Gender.NA);

                // Verify your household position was updated
                // View page
                test.infellowship.People_ProfileEditor_View();
                Assert.IsTrue(test.Selenium.IsTextPresent("Other (extended family)"));

                // Update page
                test.infellowship.People_ProfileEditor_View_UpdatePage();
                Assert.AreEqual("Other (extended family)", test.Selenium.GetSelectedLabel(InFellowshipConstants.People.ProfileEditorConstants.DropDown_HouseholdPositon));

                // Revert the chages
                test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.SingleAdult, null, GeneralEnumerations.MaritalStatus.NA, GeneralEnumerations.Gender.NA);

                // Verify your household position was reverted
                // View page
                test.infellowship.People_ProfileEditor_View();
                Assert.IsFalse(test.Selenium.IsTextPresent("Friend of Family"));

                // Update page
                test.infellowship.People_ProfileEditor_View_UpdatePage();
                Assert.AreEqual("Single Adult", test.Selenium.GetSelectedLabel(InFellowshipConstants.People.ProfileEditorConstants.DropDown_HouseholdPositon));

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Clean up
                base.SQL.People_InFellowshipAccount_Delete(254, email);
                base.SQL.People_MergeIndividual(254, individualName, "Merge Dump");
            }
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can update your date of birth.")]
        public void ProfileEditor_Update_DateOfBirth() {
            // Initial data
            var firstName = Guid.NewGuid().ToString().Substring(0, 8);
            var lastName = "DOB";
            var individualName = firstName + " " + lastName;
            var email = "profileupdate.dob@fellowshiptech.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);
            base.SQL.People_Individual_Create(254, firstName, lastName);
            base.SQL.People_InFellowshipAccount_Create(254, individualName, email, "BM.Admin09");

            try
            {
                // Login to infellowship
                TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

                // Update your D.O.B.
                test.infellowship.People_ProfileEditor_Update_Address("United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", null);
                test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.NA, "11/1/1960", GeneralEnumerations.MaritalStatus.NA, GeneralEnumerations.Gender.NA);
                test.infellowship.People_PrivacySettings_Update_OptInToDirectory(true);

                // Verify your D.O.B. was updated
                // View page
                test.infellowship.People_ProfileEditor_View();
                Assert.IsTrue(test.Selenium.IsTextPresent("11/1/1960"));

                // Update page
                test.infellowship.People_ProfileEditor_View_UpdatePage();
                Assert.AreEqual("11/1/1960", test.Selenium.GetValue(InFellowshipConstants.People.ProfileEditorConstants.DateControl_DateOfBirth));

                // Church Directory
                test.infellowship.People_ChurchDirectory_View_Individual(individualName);
                test.Selenium.VerifyTextPresent("November 01");
                Assert.IsFalse(test.Selenium.IsTextPresent("November 15"));

                // Revert the chages
                test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.NA, "11/15/1985", GeneralEnumerations.MaritalStatus.NA, GeneralEnumerations.Gender.NA);

                // Verify your D.O.B. was reverted
                // View page
                test.infellowship.People_ProfileEditor_View();
                Assert.IsTrue(test.Selenium.IsTextPresent("11/15/1985"));

                // Update page
                test.infellowship.People_ProfileEditor_View_UpdatePage();
                Assert.AreEqual("11/15/1985", test.Selenium.GetValue(InFellowshipConstants.People.ProfileEditorConstants.DateControl_DateOfBirth));

                // Church Directory
                test.infellowship.People_ChurchDirectory_View_Individual(individualName);
                test.Selenium.VerifyTextPresent("November 15");
                Assert.IsFalse(test.Selenium.IsTextPresent("November 01"));

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Clean up
                base.SQL.People_InFellowshipAccount_Delete(254, email);
                base.SQL.People_MergeIndividual(254, individualName, "Merge Dump");
            }
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can update your marital status.")]
        public void ProfileEditor_Update_MaritalStatus() {
            // Initial data
            var firstName = Guid.NewGuid().ToString().Substring(0, 8);
            var lastName = "Marital";
            var individualName = firstName + " " + lastName;
            var email = "profileupdate.marital@fellowshiptech.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);
            base.SQL.People_Individual_Create(254, firstName, lastName);
            base.SQL.People_InFellowshipAccount_Create(254, individualName, email, "BM.Admin09");

            try
            {
                // Login to infellowship
                TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

                // Update your Maritial Status
                test.infellowship.People_ProfileEditor_Update_Address("United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", null);
                test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.NA, null, GeneralEnumerations.MaritalStatus.Married, GeneralEnumerations.Gender.NA);

                // Verify your marital status was updated
                // View page
                test.infellowship.People_ProfileEditor_View();
                Assert.IsTrue(test.Selenium.IsTextPresent("Married"));

                // Update page
                test.infellowship.People_ProfileEditor_View_UpdatePage();
                Assert.AreEqual("Married", test.Selenium.GetValue(InFellowshipConstants.People.ProfileEditorConstants.DropDown_MaritalStatus));

                // Revert the chages
                test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.NA, null, GeneralEnumerations.MaritalStatus.Single, GeneralEnumerations.Gender.NA);

                // Verify your maritial status was reverted
                // View page
                test.infellowship.People_ProfileEditor_View();
                Assert.IsTrue(test.Selenium.IsTextPresent("Single"));

                // Update page
                test.infellowship.People_ProfileEditor_View_UpdatePage();
                Assert.AreEqual("Single", test.Selenium.GetValue(InFellowshipConstants.People.ProfileEditorConstants.DropDown_MaritalStatus));

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Clean up
                base.SQL.People_InFellowshipAccount_Delete(254, email);
                base.SQL.People_MergeIndividual(254, individualName, "Merge Dump");
            }
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can update your gender.")]
        public void ProfileEditor_Update_Gender() {
            // Initial data
            var firstName = Guid.NewGuid().ToString().Substring(0, 8);
            var lastName = "Gender";
            var individualName = firstName + " " + lastName;
            var email = "profileupdate.gender@fellowshiptech.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);
            base.SQL.People_Individual_Create(254, firstName, lastName);
            base.SQL.People_InFellowshipAccount_Create(254, individualName, email, "BM.Admin09");

            try
            {
                // Login to infellowship
                TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

                // Update your Gender
                test.infellowship.People_ProfileEditor_Update_Address("United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", null);
                test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.NA, null, GeneralEnumerations.MaritalStatus.NA, GeneralEnumerations.Gender.Female);

                // Verify your gender was updated
                // View page
                test.infellowship.People_ProfileEditor_View();
                Assert.IsTrue(test.Selenium.IsTextPresent("Female"));

                // Update page
                test.infellowship.People_ProfileEditor_View_UpdatePage();
                Assert.AreEqual("Female", test.Selenium.GetSelectedValue(InFellowshipConstants.People.ProfileEditorConstants.DropDown_Gender));

                // Revert the chages
                test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.NA, null, GeneralEnumerations.MaritalStatus.NA, GeneralEnumerations.Gender.Male);

                // Verify your gender was reverted
                // View page
                test.infellowship.People_ProfileEditor_View();
                Assert.IsTrue(test.Selenium.IsTextPresent("Male"));

                // Update page
                test.infellowship.People_ProfileEditor_View_UpdatePage();
                Assert.AreEqual("Male", test.Selenium.GetSelectedValue(InFellowshipConstants.People.ProfileEditorConstants.DropDown_Gender));

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Clean up
                base.SQL.People_InFellowshipAccount_Delete(254, email);
                base.SQL.People_MergeIndividual(254, individualName, "Merge Dump");
            }
        }
        #endregion Personal Information

        #region Phone Communication
        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can add a mobile number.")]
        public void ProfileEditor_Update_Mobile_Number() {
            // Initial data
            var firstName = Guid.NewGuid().ToString().Substring(0, 8);
            var lastName = "UpdateMobile";
            var individualName = firstName + " " + lastName;
            var email = "profileupdate.mobile@fellowshiptech.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);
            base.SQL.People_Individual_Create(254, firstName, lastName);
            base.SQL.People_InFellowshipAccount_Create(254, individualName, email, "BM.Admin09");

            try
            {
                // Login to infellowship
                TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

                // Add a home number
                test.infellowship.People_ProfileEditor_Update_Address("United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", null);
                test.infellowship.People_ProfileEditor_Update_Phone(GeneralEnumerations.CommunicationTypes.Mobile, "214-532-7112");

                // Revert the changes
                test.infellowship.People_ProfileEditor_Delete_Phone(GeneralEnumerations.CommunicationTypes.Mobile);

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Clean up
                base.SQL.People_InFellowshipAccount_Delete(254, email);
                base.SQL.People_MergeIndividual(254, individualName, "Merge Dump");
            }
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can add a home number.")]
        public void ProfileEditor_Update_Home_Number() {
            // Initial data
            var firstName = Guid.NewGuid().ToString().Substring(0, 8);
            var lastName = "UpdateHome";
            var individualName = firstName + " " + lastName;
            var email = "profileupdate.home@fellowshiptech.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);
            base.SQL.People_Individual_Create(254, firstName, lastName);
            base.SQL.People_InFellowshipAccount_Create(254, individualName, email, "BM.Admin09");

            try
            {
                // Login to infellowship
                TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

                // Add a home number
                test.infellowship.People_ProfileEditor_Update_Address("United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", null);
                test.infellowship.People_ProfileEditor_Update_Phone(GeneralEnumerations.CommunicationTypes.Home, "214-513-1354");

                // Revert the changes
                test.infellowship.People_ProfileEditor_Delete_Phone(GeneralEnumerations.CommunicationTypes.Home);

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Clean up
                base.SQL.People_InFellowshipAccount_Delete(254, email);
                base.SQL.People_MergeIndividual(254, individualName, "Merge Dump");
            }
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can add a work number.")]
        public void ProfileEditor_Update_Work_Number() {
            // Initial data
            var firstName = Guid.NewGuid().ToString().Substring(0, 8);
            var lastName = "UpdateWork";
            var individualName = firstName + " " + lastName;
            var email = "profileupdate.work@fellowshiptech.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);
            base.SQL.People_Individual_Create(254, firstName, lastName);
            base.SQL.People_InFellowshipAccount_Create(254, individualName, email, "BM.Admin09");

            try
            {
                // Login to infellowship
                TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

                // Add a work number            
                test.infellowship.People_ProfileEditor_Update_Address("United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", null);
                test.infellowship.People_ProfileEditor_Update_Phone(GeneralEnumerations.CommunicationTypes.Work, "214-555-1354");

                // Revert the changes
                test.infellowship.People_ProfileEditor_Delete_Phone(GeneralEnumerations.CommunicationTypes.Work);

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Clean up
                base.SQL.People_InFellowshipAccount_Delete(254, email);
                base.SQL.People_MergeIndividual(254, individualName, "Merge Dump");
            }
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can provide an emergency phone number.")]
        public void ProfileEditor_Update_Emergency_Phone() {          
            // Initial data
            var firstName = Guid.NewGuid().ToString().Substring(0, 8);
            var lastName = "UpdateEmergency";
            var individualName = firstName + " " + lastName;
            var email = "profileupdate.emergency@fellowshiptech.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);
            base.SQL.People_Individual_Create(254, firstName, lastName);
            base.SQL.People_InFellowshipAccount_Create(254, individualName, email, "BM.Admin09");

            try
            {
                // Login to infellowship
                TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

                // Add an emergency number            
                test.infellowship.People_ProfileEditor_Update_Address("United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", null);
                test.infellowship.People_ProfileEditor_Update_EmergencyPhone("927-555-5555", "Hobbits can call me here.");

                // Delete the emergency number
                test.infellowship.People_ProfileEditor_Delete_EmergencyPhone();

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Clean up
                base.SQL.People_InFellowshipAccount_Delete(254, email);
                base.SQL.People_MergeIndividual(254, individualName, "Merge Dump");
            }
        }

        #endregion Phone Communication

        #region Address
        [Test, RepeatOnFailure, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can update your address.")]
        public void ProfileEditor_Update_Address() {
            // Initial data
            var firstName = Guid.NewGuid().ToString().Substring(0, 8);
            var lastName = "UpdateAddress";
            var individualName = firstName + " " + lastName;
            var email = "profileupdate.address@fellowshiptech.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);
            base.SQL.People_Individual_Create(254, firstName, lastName);
            base.SQL.People_InFellowshipAccount_Create(254, individualName, email, "BM.Admin09");

            try
            {
                // Login to infellowship
                TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

                // Update your address
                test.infellowship.People_ProfileEditor_Update_Address(null, "1817 Foxfield Way", null, "Justin", "Texas", "76247", "Denton");
                test.infellowship.People_PrivacySettings_Update_OptInToDirectory(true);

                // Verify the address was updated
                // View Page
                test.Selenium.VerifyTextPresent("1817 Foxfield Way");
                test.Selenium.VerifyTextPresent("Justin, TX 76247");
                test.Selenium.VerifyTextPresent("Denton");

                // Update page
                test.infellowship.People_ProfileEditor_View_UpdatePage();
                Assert.AreEqual("1817 Foxfield Way", test.Selenium.GetValue(InFellowshipConstants.People.ProfileEditorConstants.TextField_AddressOne));
                Assert.AreEqual("Justin", test.Selenium.GetValue(InFellowshipConstants.People.ProfileEditorConstants.TextField_City));
                Assert.AreEqual("TX", test.Selenium.GetValue(InFellowshipConstants.People.ProfileEditorConstants.DropDown_State));
                Assert.AreEqual("76247", test.Selenium.GetValue(InFellowshipConstants.People.ProfileEditorConstants.TextField_ZipCode));
                Assert.AreEqual("Denton", test.Selenium.GetValue(InFellowshipConstants.People.ProfileEditorConstants.TextField_County));

                // Church Directory
                test.infellowship.People_ChurchDirectory_Search(individualName);
                test.Selenium.VerifyTextPresent("Justin, TX");
                Assert.IsFalse(test.Selenium.IsTextPresent("Flower Mound, TX"));

                test.infellowship.People_ChurchDirectory_View_Individual(individualName);
                test.Selenium.VerifyTextPresent("1817 Foxfield Way");
                test.Selenium.VerifyTextPresent("Justin, TX 76247");
                Assert.IsFalse(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"));
                Assert.IsFalse(test.Selenium.IsTextPresent("Flower Mound, TX 75022"));

                // Revert the changes
                test.infellowship.People_ProfileEditor_Update_Address(null, "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", "Denton");

                // Verify the address was updated
                // View Page
                Assert.IsTrue(test.Selenium.IsTextPresent("2812 Meadow Wood Drive"));
                Assert.IsTrue(test.Selenium.IsTextPresent("Flower Mound, TX 75022"));
                Assert.IsTrue(test.Selenium.IsTextPresent("Denton"));

                // Update page
                test.infellowship.People_ProfileEditor_View_UpdatePage();
                Assert.AreEqual("2812 Meadow Wood Drive", test.Selenium.GetValue(InFellowshipConstants.People.ProfileEditorConstants.TextField_AddressOne));
                Assert.AreEqual("Flower Mound", test.Selenium.GetValue(InFellowshipConstants.People.ProfileEditorConstants.TextField_City));
                Assert.AreEqual("TX", test.Selenium.GetValue(InFellowshipConstants.People.ProfileEditorConstants.DropDown_State));
                Assert.AreEqual("75022", test.Selenium.GetValue(InFellowshipConstants.People.ProfileEditorConstants.TextField_ZipCode));
                Assert.AreEqual("Denton", test.Selenium.GetValue(InFellowshipConstants.People.ProfileEditorConstants.TextField_County));

                // Church Directory
                test.infellowship.People_ChurchDirectory_Search(individualName);
                test.Selenium.VerifyTextPresent("Flower Mound, TX");
                Assert.IsFalse(test.Selenium.IsTextPresent("Justin, TX"));

                test.infellowship.People_ChurchDirectory_View_Individual(individualName);
                test.Selenium.VerifyTextPresent("2812 Meadow Wood Drive");
                test.Selenium.VerifyTextPresent("Flower Mound, TX 75022");
                Assert.IsFalse(test.Selenium.IsTextPresent("1817 Foxfield Way"));
                Assert.IsFalse(test.Selenium.IsTextPresent("Justin, TX 76247"));

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Clean up
                base.SQL.People_InFellowshipAccount_Delete(254, email);
                base.SQL.People_MergeIndividual(254, individualName, "Merge Dump");
            }

        }
        #endregion Address

        #region Websites
        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can add a website.")]
        public void ProfileEditor_Update_Add_Website() {
            // Initial data
            var firstName = Guid.NewGuid().ToString().Substring(0, 8);
            var lastName = "AddWebsite";
            var individualName = firstName + " " + lastName;
            var email = "profileupdate.websiteadd@fellowshiptech.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);
            base.SQL.People_Individual_Create(254, firstName, lastName);
            base.SQL.People_InFellowshipAccount_Create(254, individualName, email, "BM.Admin09");

            try
            {
                // Login to infellowship
                TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

                // Add a website 
                test.infellowship.People_ProfileEditor_Update_Address("United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", null);
                test.infellowship.People_ProfileEditor_Update_Add_Website("http://www.google.com");

                // Verify the website was added
                test.Selenium.VerifyElementPresent("link=http://www.google.com");

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Clean up
                base.SQL.People_InFellowshipAccount_Delete(254, email);
                base.SQL.People_MergeIndividual(254, individualName, "Merge Dump");
            }
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can update a website.")]
        public void ProfileEditor_Update_Update_Website() {
            // Initial data
            var firstName = Guid.NewGuid().ToString().Substring(0, 8);
            var lastName = "UpdateWebsite";
            var individualName = firstName + " " + lastName;
            var email = "profileupdate.websiteup@fellowshiptech.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);
            base.SQL.People_Individual_Create(254, firstName, lastName);
            base.SQL.People_InFellowshipAccount_Create(254, individualName, email, "BM.Admin09");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            try
            {
                test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

                // Add a website 
                test.infellowship.People_ProfileEditor_Update_Address("United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", null);
                test.infellowship.People_ProfileEditor_Update_Add_Website("http://www.google.com");

                // Update the website
                test.infellowship.People_ProfileEditor_Update_Website("http://www.google.com", "http://www.apple.com");

                // Verify the website was updated
                test.Selenium.VerifyElementNotPresent("link=http://www.google.com");
                test.Selenium.VerifyElementPresent("link=http://www.apple.com");

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Clean up
                base.SQL.People_InFellowshipAccount_Delete(254, email);
                base.SQL.People_MergeIndividual(254, individualName, "Merge Dump");
            }
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can delete a website.")]
        public void ProfileEditor_Update_Delete_Website() {
            // Initial data
            var firstName = Guid.NewGuid().ToString().Substring(0, 8);
            var lastName = "DelWebsite";
            var individualName = firstName + " " + lastName;
            var email = "profileupdate.websitedel@fellowshiptech.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);
            base.SQL.People_Individual_Create(254, firstName, lastName);
            base.SQL.People_InFellowshipAccount_Create(254, individualName, email, "BM.Admin09");

            TestLog.WriteLine("SQL AccountDel/IndividualCreated/AccountCreate: [" + email + "/" + firstName + "/" + lastName + "]");

            try
            {
                // Login to infellowship
                TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                TestLog.WriteLine("Login");
                test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

                // Add a website 
                TestLog.WriteLine("Update Address");
                test.infellowship.People_ProfileEditor_Update_Address("United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", null);
                TestLog.WriteLine("Add Website http://www.apple.com");
                test.infellowship.People_ProfileEditor_Update_Add_Website("http://www.apple.com");


                // Delete a website
                TestLog.WriteLine("Delete Website http://www.apple.com");
                test.infellowship.People_ProfileEditor_Delete_Website("http://www.apple.com");

                // Verify the website was deleted
                TestLog.WriteLine("Verify Delete Website");
                test.Selenium.VerifyElementNotPresent("link=http://www.apple.com");

                // Logout
                TestLog.WriteLine("Logout");
                test.infellowship.Logout();
            }
            finally
            {
                // Clean up
                TestLog.WriteLine("Delete Infellowship Account " + email);
                base.SQL.People_InFellowshipAccount_Delete(254, email);
                base.SQL.People_MergeIndividual(254, individualName, "Merge Dump");
            }
        }

        #endregion Websites

        #region Social Media
        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can update your facebook information.")]
        public void ProfileEditor_Update_Facebook() {
            // Initial data
            var firstName = Guid.NewGuid().ToString().Substring(0, 8);
            var lastName = "UpdateFacebook";
            var individualName = firstName + " " + lastName;
            var email = "profileupdate.facebook@fellowshiptech.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);
            base.SQL.People_Individual_Create(254, firstName, lastName);
            base.SQL.People_InFellowshipAccount_Create(254, individualName, email, "BM.Admin09");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

                // Update your Facebook
                test.infellowship.People_ProfileEditor_Update_Address("United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", null);
                test.infellowship.People_ProfileEditor_Update_SocialMedia(GeneralEnumerations.SocialMediaTypes.Facebook, "http://www.facebook.com/bryanmikaelian");
                test.infellowship.People_PrivacySettings_Update_OptInToDirectory(true);

                // Verify changes in the Church Directory
                test.infellowship.People_ChurchDirectory_Search(individualName);
                test.Selenium.VerifyElementPresent("//a[1]/img[@alt='Facebook']");

                test.infellowship.People_ChurchDirectory_View_Individual(individualName);
                test.Selenium.VerifyElementPresent("//a[1]/img[@alt='Facebook']");

                // Revert the changes
                test.infellowship.People_ProfileEditor_Delete_SocialMedia(GeneralEnumerations.SocialMediaTypes.Facebook);

                // Verify changes in the Church Directory
                test.infellowship.People_ChurchDirectory_Search(individualName);
                test.Selenium.VerifyElementNotPresent("//a[1]/img[@alt='Facebook']");

                test.infellowship.People_ChurchDirectory_View_Individual(individualName);
                test.Selenium.VerifyElementNotPresent("//a[1]/img[@alt='Facebook']");

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Clean up
                base.SQL.People_InFellowshipAccount_Delete(254, email);
                base.SQL.People_MergeIndividual(254, individualName, "Merge Dump");
            }
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can update your twitter information.")]
        public void ProfileEditor_Update_Twitter() {
            // Initial data
            var firstName = Guid.NewGuid().ToString().Substring(0, 8);
            var lastName = "UpdateTwitter";
            var individualName = firstName + " " + lastName;
            var email = "profileupdate.twitter@fellowshiptech.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);
            base.SQL.People_Individual_Create(254, firstName, lastName);
            base.SQL.People_InFellowshipAccount_Create(254, individualName, email, "BM.Admin09");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

                // Update your Twitter
                test.infellowship.People_ProfileEditor_Update_Address("United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", null);
                test.infellowship.People_ProfileEditor_Update_SocialMedia(GeneralEnumerations.SocialMediaTypes.Twitter, "http://www.twitter.com/bryanmikaelian");
                test.infellowship.People_PrivacySettings_Update_OptInToDirectory(true);

                // Verify changes in the Church Directory
                test.infellowship.People_ChurchDirectory_Search(individualName);
                test.Selenium.VerifyElementPresent("//a[1]/img[@alt='Twitter']");

                test.infellowship.People_ChurchDirectory_View_Individual(individualName);
                test.Selenium.VerifyElementPresent("//a[1]/img[@alt='Twitter']");

                // Revert the changes
                test.infellowship.People_ProfileEditor_Delete_SocialMedia(GeneralEnumerations.SocialMediaTypes.Twitter);

                // Verify changes in the Church Directory
                test.infellowship.People_ChurchDirectory_Search(individualName);
                test.Selenium.VerifyElementNotPresent("//a[1]/img[@alt='Twitter']");

                test.infellowship.People_ChurchDirectory_View_Individual(individualName);
                test.Selenium.VerifyElementNotPresent("//a[1]/img[@alt='Twitter']");

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Clean up
                base.SQL.People_InFellowshipAccount_Delete(254, email);
                base.SQL.People_MergeIndividual(254, individualName, "Merge Dump");
            }
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can update your linkedin information.")]
        public void ProfileEditor_Update_LinkedIn() {
            // Initial data
            var firstName = Guid.NewGuid().ToString().Substring(0, 8);
            var lastName = "UpdateLinkedin";
            var individualName = firstName + " " + lastName;
            var email = "profileupdate.linkedin@fellowshiptech.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);
            base.SQL.People_Individual_Create(254, firstName, lastName);
            base.SQL.People_InFellowshipAccount_Create(254, individualName, email, "BM.Admin09");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

                // Update your Facebook
                test.infellowship.People_ProfileEditor_Update_Address("United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", null);
                test.infellowship.People_ProfileEditor_Update_SocialMedia(GeneralEnumerations.SocialMediaTypes.LinkedIn, "http://www.linkedin.com/in/bryanmikaelian");
                test.infellowship.People_PrivacySettings_Update_OptInToDirectory(true);

                // Verify changes in the Church Directory
                test.infellowship.People_ChurchDirectory_Search(individualName);
                test.Selenium.VerifyElementPresent("//a[1]/img[@alt='LinkedIn']");

                test.infellowship.People_ChurchDirectory_View_Individual(individualName);
                test.Selenium.VerifyElementPresent("//a[1]/img[@alt='LinkedIn']");

                // Revert the changes
                test.infellowship.People_ProfileEditor_Delete_SocialMedia(GeneralEnumerations.SocialMediaTypes.LinkedIn);

                // Verify changes in the Church Directory
                test.infellowship.People_ChurchDirectory_Search(individualName);
                test.Selenium.VerifyElementNotPresent("//a[1]/img[@alt='LinkedIn']");

                test.infellowship.People_ChurchDirectory_View_Individual(individualName);
                test.Selenium.VerifyElementNotPresent("//a[1]/img[@alt='LinkedIn']");

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Clean up
                base.SQL.People_InFellowshipAccount_Delete(254, email);
                base.SQL.People_MergeIndividual(254, individualName, "Merge Dump");
            }
        }
        #endregion Social Media

        #region Email
        [Test, RepeatOnFailure, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can update your personal email information.")]
        public void ProfileEditor_Update_Email_Personal() {
            // Initial data
            var firstName = Guid.NewGuid().ToString().Substring(0, 8);
            var lastName = "UpdateEmail";
            var individualName = firstName + " " + lastName;
            var email = "profileupdate.email@fellowshiptech.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);
            base.SQL.People_Individual_Create(254, firstName, lastName);
            base.SQL.People_InFellowshipAccount_Create(254, individualName, email, "BM.Admin09");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

                // Update your email
                test.infellowship.People_ProfileEditor_Update_Address("United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", null);
                test.infellowship.People_ProfileEditor_Update_Email(GeneralEnumerations.CommunicationTypes.Personal, "inf.personal@gmail.com");

                // Revert the changes
                test.infellowship.People_ProfileEditor_Delete_Email(GeneralEnumerations.CommunicationTypes.Personal);

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Clean up
                base.SQL.People_InFellowshipAccount_Delete(254, email);
                base.SQL.People_MergeIndividual(254, individualName, "Merge Dump");
            }
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can update your home email information.")]
        public void ProfileEditor_Update_Email_Home() {
            // Initial data
            var firstName = Guid.NewGuid().ToString().Substring(0, 8);
            var lastName = "UpdateHomeEmail";
            var individualName = firstName + " " + lastName;
            var email = "profileupdate.homeemail@fellowshiptech.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);
            base.SQL.People_Individual_Create(254, firstName, lastName);
            base.SQL.People_InFellowshipAccount_Create(254, individualName, email, "BM.Admin09");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

                // Update your email
                test.infellowship.People_ProfileEditor_Update_Address("United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", null);
                test.infellowship.People_ProfileEditor_Update_Email(GeneralEnumerations.CommunicationTypes.Home, "inf.home@gmail.com");

                // Revert the changes
                test.infellowship.People_ProfileEditor_Delete_Email(GeneralEnumerations.CommunicationTypes.Home);

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Clean up
                base.SQL.People_InFellowshipAccount_Delete(254, email);
                base.SQL.People_MergeIndividual(254, individualName, "Merge Dump");
            }
        }


        #endregion Email

        #endregion Updating Information

        #region Preferred Information
        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the rules for preferred phone numbers.")]
        public void ProfileEditor_Update_Preferred_Phone() {
            // Initial data
            var mobileNumber = "817-532-7112";
            var homeNumber = "817-513-1354";
            var workNumber = "817-533-7112";
            var firstName = Guid.NewGuid().ToString().Substring(0, 8);
            var lastName = "PrefPhone";
            var individualName = firstName + " " + lastName;
            var email = "preferred.phone@fellowshiptech.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);
            base.SQL.People_Individual_Create(254, firstName, lastName);
            base.SQL.People_InFellowshipAccount_Create(254, individualName, email, "BM.Admin09");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, individualName, "Member");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

                // Add three phone numbers       
                test.infellowship.People_ProfileEditor_Update_Address("United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", null);
                test.infellowship.People_ProfileEditor_Update_Phone(GeneralEnumerations.CommunicationTypes.Mobile, mobileNumber);
                test.infellowship.People_ProfileEditor_Update_Phone(GeneralEnumerations.CommunicationTypes.Home, homeNumber);
                test.infellowship.People_ProfileEditor_Update_Phone(GeneralEnumerations.CommunicationTypes.Work, workNumber);
                test.infellowship.People_PrivacySettings_Update_OptInToDirectory(true);

                // Mark the mobile number as preferred
                test.infellowship.People_ProfileEditor_Update_Preferred_Phone(GeneralEnumerations.CommunicationTypes.Mobile);

                // Verify the mobile number shows up and the others do not
                // Church Directory
                test.infellowship.People_ChurchDirectory_Search(individualName);
                test.Selenium.VerifyTextPresent(mobileNumber);
                Assert.IsFalse(test.Selenium.IsTextPresent(homeNumber), "Non preferred number was present in the church directory!");
                Assert.IsFalse(test.Selenium.IsTextPresent(workNumber), "Non preferred number was present in the church directory!");

                test.infellowship.People_ChurchDirectory_View_Individual(individualName);
                test.Selenium.VerifyTextPresent(mobileNumber);
                Assert.IsFalse(test.Selenium.IsTextPresent(homeNumber), "Non preferred number was present in the church directory!");
                Assert.IsFalse(test.Selenium.IsTextPresent(workNumber), "Non preferred number was present in the church directory!");

                // Group Roster
                test.infellowship.Groups_Group_View_Roster(_groupName);
                test.Selenium.VerifyTextPresent(mobileNumber);
                Assert.IsFalse(test.Selenium.IsTextPresent(homeNumber), "Non preferred number was present on the group roster!");
                Assert.IsFalse(test.Selenium.IsTextPresent(workNumber), "Non preferred number was present on the group roster!");

                // Mark the home number as preferred
                test.infellowship.People_ProfileEditor_Update_Preferred_Phone(GeneralEnumerations.CommunicationTypes.Home);

                // Verify the home number shows up and the others do not
                // Church Directory
                test.infellowship.People_ChurchDirectory_Search(individualName);
                test.Selenium.VerifyTextPresent(homeNumber);
                Assert.IsFalse(test.Selenium.IsTextPresent(mobileNumber), "Non preferred number was present in the church directory!");
                Assert.IsFalse(test.Selenium.IsTextPresent(workNumber), "Non preferred number was present in the church directory!");

                test.infellowship.People_ChurchDirectory_View_Individual(individualName);
                test.Selenium.VerifyTextPresent(homeNumber);
                Assert.IsFalse(test.Selenium.IsTextPresent(mobileNumber), "Non preferred number was present in the church directory!");
                Assert.IsFalse(test.Selenium.IsTextPresent(workNumber), "Non preferred number was present in the church directory!");

                // Group Roster
                test.infellowship.Groups_Group_View_Roster(_groupName);
                test.Selenium.VerifyTextPresent(homeNumber);
                Assert.IsFalse(test.Selenium.IsTextPresent(mobileNumber), "Non preferred number was present on the group roster!");
                Assert.IsFalse(test.Selenium.IsTextPresent(workNumber), "Non preferred number was present on the group roster!");

                // Mark the work number as preferred
                test.infellowship.People_ProfileEditor_Update_Preferred_Phone(GeneralEnumerations.CommunicationTypes.Work);

                // Verify the work number shows up and the others do not
                // Church Directory
                test.infellowship.People_ChurchDirectory_Search(individualName);
                test.Selenium.VerifyTextPresent(workNumber);
                Assert.IsFalse(test.Selenium.IsTextPresent(mobileNumber), "Non preferred number was present in the church directory!");
                Assert.IsFalse(test.Selenium.IsTextPresent(homeNumber), "Non preferred number was present in the church directory!");

                test.infellowship.People_ChurchDirectory_View_Individual(individualName);
                test.Selenium.VerifyTextPresent(workNumber);
                Assert.IsFalse(test.Selenium.IsTextPresent(mobileNumber), "Non preferred number was present in the church directory!");
                Assert.IsFalse(test.Selenium.IsTextPresent(homeNumber), "Non preferred number was present in the church directory!");

                // Group Roster
                test.infellowship.Groups_Group_View_Roster(_groupName);
                test.Selenium.VerifyTextPresent(workNumber);
                Assert.IsFalse(test.Selenium.IsTextPresent(mobileNumber), "Non preferred number was present on the group roster!");
                Assert.IsFalse(test.Selenium.IsTextPresent(homeNumber), "Non preferred number was present on the group roster!");

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Clean up
                base.SQL.People_InFellowshipAccount_Delete(254, email);
                base.SQL.People_MergeIndividual(254, individualName, "Merge Dump");
            }
 
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the rules for preferred email.")]
        public void ProfileEditor_Update_Preferred_Email() {
            var personalEmail = "personal@gmail.com";
            var homeEmail = "home@gmail.com";
            var infellowshipId = "preferred.email@fellowshiptech.com";
            var firstName = Guid.NewGuid().ToString().Substring(0, 8);
            var lastName = "PrefEmail";
            var individualName = firstName + " " + lastName;
            var email = "preferred.email@fellowshiptech.com";

            base.SQL.People_InFellowshipAccount_Delete(254, email);
            base.SQL.People_Individual_Create(254, firstName, lastName);
            base.SQL.People_InFellowshipAccount_Create(254, individualName, email, "BM.Admin09");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, individualName, "Member");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.infellowship.Login(email, "BM.Admin09", "QAEUNLX0C2");

                // Add two emails          
                test.infellowship.People_ProfileEditor_Update_Address("United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", null);
                test.infellowship.People_ProfileEditor_Update_Email(GeneralEnumerations.CommunicationTypes.Personal, personalEmail);
                test.infellowship.People_ProfileEditor_Update_Email(GeneralEnumerations.CommunicationTypes.Home, homeEmail);
                test.infellowship.People_PrivacySettings_Update_OptInToDirectory(true);

                // Mark the personal email as preferred
                test.infellowship.People_ProfileEditor_Update_Preferred_Email(GeneralEnumerations.CommunicationTypes.Personal);

                // Verify the personal email shows up and the others do not
                // Church Directory
                test.infellowship.People_ChurchDirectory_Search(individualName);
                test.Selenium.VerifyTextPresent(personalEmail);
                Assert.IsFalse(test.Selenium.IsTextPresent(homeEmail), "Non preferred email was present in the church directory!");
                Assert.IsFalse(test.Selenium.IsTextPresent(infellowshipId), "Non preferred email was present in the church directory!");

                test.infellowship.People_ChurchDirectory_View_Individual(individualName);
                test.Selenium.VerifyTextPresent(personalEmail);
                Assert.IsFalse(test.Selenium.IsTextPresent(homeEmail), "Non preferred email was present in the church directory!");
                Assert.IsFalse(test.Selenium.IsTextPresent(infellowshipId), "Non preferred email was present in the church directory!");

                // Group Roster
                test.infellowship.Groups_Group_View_Roster(_groupName);
                test.Selenium.VerifyTextPresent(personalEmail);
                Assert.IsFalse(test.Selenium.IsTextPresent(homeEmail), "Non preferred email was present on the group roster!");
                Assert.IsFalse(test.Selenium.IsTextPresent(infellowshipId), "Non preferred email was present on the group roster!");

                // Mark the home email as preferred
                test.infellowship.People_ProfileEditor_Update_Preferred_Email(GeneralEnumerations.CommunicationTypes.Home);

                // Verify the home email shows up and the others do not
                // Church Directory
                test.infellowship.People_ChurchDirectory_Search(individualName);
                test.Selenium.VerifyTextPresent(homeEmail);
                Assert.IsFalse(test.Selenium.IsTextPresent(personalEmail), "Non preferred email was present in the church directory!");
                Assert.IsFalse(test.Selenium.IsTextPresent(infellowshipId), "Non preferred email was present in the church directory!");

                test.infellowship.People_ChurchDirectory_View_Individual(individualName);
                test.Selenium.VerifyTextPresent(homeEmail);
                Assert.IsFalse(test.Selenium.IsTextPresent(personalEmail), "Non preferred email was present in the church directory!");
                Assert.IsFalse(test.Selenium.IsTextPresent(infellowshipId), "Non preferred email was present in the church directory!");

                // Group Roster
                test.infellowship.Groups_Group_View_Roster(_groupName);
                test.Selenium.VerifyTextPresent(homeEmail);
                Assert.IsFalse(test.Selenium.IsTextPresent(personalEmail), "Non preferred email was present on the group roster!");
                Assert.IsFalse(test.Selenium.IsTextPresent(infellowshipId), "Non preferred email was present on the group roster!");

                // Mark the infellowship id as preferred
                test.infellowship.People_ProfileEditor_Update_Preferred_Email(GeneralEnumerations.CommunicationTypes.InFellowship);

                // Verify the work email shows up and the others do not
                // Church Directory
                test.infellowship.People_ChurchDirectory_Search(individualName);
                test.Selenium.VerifyElementPresent(string.Format("//a[@href='mailto:{0}']", infellowshipId));
                Assert.IsFalse(test.Selenium.IsTextPresent(personalEmail), "Non preferred email was present in the church directory!");
                Assert.IsFalse(test.Selenium.IsTextPresent(homeEmail), "Non preferred email was present in the church directory!");

                test.infellowship.People_ChurchDirectory_View_Individual(individualName);
                test.Selenium.VerifyTextPresent(infellowshipId);
                Assert.IsFalse(test.Selenium.IsTextPresent(personalEmail), "Non preferred email was present in the church directory!");
                Assert.IsFalse(test.Selenium.IsTextPresent(homeEmail), "Non preferred email was present in the church directory!");

                // Group Roster
                test.infellowship.Groups_Group_View_Roster(_groupName);
                test.Selenium.VerifyElementPresent(string.Format("//a[@href='mailto:{0}']", infellowshipId));
                Assert.IsFalse(test.Selenium.IsTextPresent(personalEmail), "Non preferred email was present on the group roster!");
                Assert.IsFalse(test.Selenium.IsTextPresent(homeEmail), "Non preferred email was present on the group roster!");

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Clean up
                base.SQL.People_InFellowshipAccount_Delete(254, email);
                base.SQL.People_MergeIndividual(254, individualName, "Merge Dump");
            }

            
        }

        #endregion Preferred Information

        #region Invalid Email
        [Test, RepeatOnFailure(Order = 1)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that if an infellowship email is no longe valid, Profile Editor will display a notification message to the user about the invalid email.")]
        public void ProfileEditor_Update_Invalid_InFellowship_Email_Notification_Present() {
            // Mark an Email as invalid
            base.SQL.People_UpdateCommunicationValidationStatus(254, "Bryan Mikaelian", "bmikaelian@fellowshiptech.com", 0);

            // Login to InFellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.infellowship.Login("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

                // View the update profile page
                test.infellowship.People_ProfileEditor_View_UpdatePage();

                // Verify the text box for the email is red and messaging is present for the invalid email
                test.Selenium.VerifyTextPresent("bmikaelian@fellowshiptech.com isn't receiving email messages");
                test.Selenium.VerifyElementPresent("//small[@class='red']");
                test.Selenium.VerifyTextPresent("This address is either invalid or not receiving messages.");

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Revert the change
                base.SQL.People_UpdateCommunicationValidationStatus(254, "Bryan Mikaelian", "bmikaelian@fellowshiptech.com", null);
            }
        }

        [Test, RepeatOnFailure(Order = 2)]
        [Author("Bryan Mikaelian")]
        [Description("Makes an invalid infellowship email valid by clicking accept on the modal popup that appears for an invalid email.")]
        public void ProfileEditor_Update_Invalid_InFellowship_Email_Make_Valid_Modal() {
            // Mark an Email as invalid
            base.SQL.People_UpdateCommunicationValidationStatus(254, "Bryan Mikaelian", "bmikaelian@fellowshiptech.com", 0);

            // Login to InFellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // View the update profile page
            test.infellowship.People_ProfileEditor_View_UpdatePage();

            // Make the infellowship email active
            test.infellowship.People_ProfileEditor_Update_Email_Make_Valid(GeneralEnumerations.CommunicationTypes.InFellowship, "bmikaelian@fellowshiptech.com", "Bryan Mikaelian", 254);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure(Order=3)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that if an alternate email is no longe valid, Profile Editor will display a notification message to the user about the invalid email.")]
        public void ProfileEditor_Update_Invalid_Alternate_Email_Notification_Present() {
            // Mark an Email as invalid
            base.SQL.People_UpdateCommunicationValidationStatus(254, "Bryan Mikaelian", "bryan.mikaelian@gmail.com", 0);

            // Login to InFellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.infellowship.Login("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

                // View the update profile page
                test.infellowship.People_ProfileEditor_View_UpdatePage();

                // Verify the text box for the email is red and messaging is present for the invalid email
                test.Selenium.VerifyTextPresent("bryan.mikaelian@gmail.com isn't receiving email messages");
                test.Selenium.VerifyElementPresent("//small[@class='red']");
                test.Selenium.VerifyTextPresent("This address is either invalid or not receiving messages.");

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Revert the change
                base.SQL.People_UpdateCommunicationValidationStatus(254, "Bryan Mikaelian", "bryan.mikaelian@gmail.com", null);
            }
        }

        [Test, RepeatOnFailure(Order = 4)]
        [Author("Bryan Mikaelian")]
        [Description("Makes an invalid alternate email valid by clicking accept on the modal popup that appears for an invalid email.")]
        public void ProfileEditor_Update_Invalid_Alternate_Email_Make_Valid_Modal() {
            // Mark an Email as invalid
            base.SQL.People_UpdateCommunicationValidationStatus(254, "Bryan Mikaelian", "bryan.mikaelian@gmail.com", 0);

            // Login to InFellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // View the update profile page
            test.infellowship.People_ProfileEditor_View_UpdatePage();

            // Make the alternate email active
            test.infellowship.People_ProfileEditor_Update_Email_Make_Valid(GeneralEnumerations.CommunicationTypes.Personal, "bryan.mikaelian@gmail.com", "Bryan Mikaelian", 254);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure(Order = 5)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that if a home email is no longe valid, Profile Editor will display a notification message to the user about the invalid email.")]
        public void ProfileEditor_Update_Invalid_Home_Email_Notification_Present() {
            // Mark an Email as invalid
            base.SQL.People_UpdateCommunicationValidationStatus(254, "Bryan Mikaelian", "bryan.mikaelian@activenetwork.com", 0);

            // Login to InFellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.infellowship.Login("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

                // View the update profile page
                test.infellowship.People_ProfileEditor_View_UpdatePage();

                // Verify the text box for the email is red and messaging is present for the invalid email
                test.Selenium.VerifyTextPresent("bryan.mikaelian@activenetwork.com isn't receiving email messages");
                test.Selenium.VerifyElementPresent("//small[@class='red']");
                test.Selenium.VerifyTextPresent("This address is either invalid or not receiving messages.");

                // Logout
                test.infellowship.Logout();
            }
            finally
            {
                // Revert the change
                base.SQL.People_UpdateCommunicationValidationStatus(254, "Bryan Mikaelian", "bryan.mikaelian@activenetwork.com", null);
            }
        }

        [Test, RepeatOnFailure(Order = 6)]
        [Author("Bryan Mikaelian")]
        [Description("Makes an invalid home email valid by clicking accept on the modal popup that appears for an invalid email.")]
        public void ProfileEditor_Update_Invalid_Home_Email_Make_Valid_Modal() {
            // Mark an Email as invalid
            base.SQL.People_UpdateCommunicationValidationStatus(254, "Bryan Mikaelian", "bryan.mikaelian@activenetwork.com", 0);

            // Login to InFellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // View the update profile page
            test.infellowship.People_ProfileEditor_View_UpdatePage();

            // Make the home email active
            test.infellowship.People_ProfileEditor_Update_Email_Make_Valid(GeneralEnumerations.CommunicationTypes.Home, "bryan.mikaelian@activenetwork.com", "Bryan Mikaelian", 254);

            // Logout
            test.infellowship.Logout();
        }

        #endregion Invalid Email

        #region Unsubscribe
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you can unsubscribe from church emails.")]
        public void ProfileEditor_Update_Unsubscribe_From_Emails() {
            // Login to InFellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Unsubscribe
            test.infellowship.People_ProfileEditor_Update_Email_SubscriptionStatus(true, "Group Leader", 254);

            // Resubscribe
            test.infellowship.People_ProfileEditor_Update_Email_SubscriptionStatus(false, "Group Leader", 254);

            // Logout
            test.infellowship.Logout();
        }

        #endregion Unsubscribe
    }
}