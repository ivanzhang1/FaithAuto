using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace FTTests.infellowship.People {
    [TestFixture]
    public class infellowship_People_ChurchDirectory : FixtureBase {
        #region Search
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for individual in the directory using part of their first name and last name, i.e. Nic Floy")]
        public void ChurchDirectory_Search_Partial_FirstName_LastName() {
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Search for an individual in the directory using a partial first name and last name
            test.infellowship.People_ChurchDirectory_Search("Bry Mik");

            // Verify the individual is returned in the results
            test.Selenium.VerifyElementPresent("link=Bryan Mikaelian");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for individual in the directory using a first name i.e. Nick")]
        public void ChurchDirectory_Search_FirstName() {
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Search for an individual in the directory using only a first name
            test.infellowship.People_ChurchDirectory_Search("Bryan");

            // Verify the individual is returned in the results
            test.Selenium.VerifyElementPresent("link=Bryan Mikaelian");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for individual in the directory using no last name followed by a comma and a first name i.e. , Bryan")]
        public void ChurchDirectory_Search_Comma_FirstName() {
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Search for an individual in the directory using no last name followed by a comma and a first name
            test.infellowship.People_ChurchDirectory_Search(" , Bryan");

            // Verify the individual is returned in the results
            test.Selenium.VerifyElementPresent("link=Bryan Mikaelian");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for individual in the directory using last name, first name i.e. Mikaelian , Bryan")]
        public void ChurchDirectory_Search_LastName_Comma_FirstName() {
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Search for an individual in the directory using last name, first name 
            test.infellowship.People_ChurchDirectory_Search("Mikaelian, Bryan");

            // Verify the individual is returned in the results
            test.Selenium.VerifyElementPresent("link=Bryan Mikaelian");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for individual in the directory using partial last name, partial first name i.e. Mik , Bry")]
        public void ChurchDirectory_Search_Parital_LastName_Comma_Partial_FirstName() {
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Search for an individual in the directory using partial last name, partial first nam
            test.infellowship.People_ChurchDirectory_Search("Mik, Bry");

            // Verify the individual is returned in the results
            test.Selenium.VerifyElementPresent("link=Bryan Mikaelian");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can search for individual in the directory using a last name followed a comma and no first name  i.e. Mik , ")]
        public void ChurchDirectory_Search_Parital_LastName_Comma_No_FirstName() {
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Search for an individual in the directory using a last name followed a comma and no first name
            test.infellowship.People_ChurchDirectory_Search("Mikaelian, ");

            // Verify the individual is returned in the results
            test.Selenium.VerifyElementPresent("link=Bryan Mikaelian");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies searching for individuals that have not opted into the Church directory are not present and are not returned in results.")]
        public void ChurchDirectory_Search_Individual_Not_Opted_In() {
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Search for an individual in the directory.
            test.infellowship.People_ChurchDirectory_Search("Group Leader");

            // Verify the individual that did not opt in is not present.
            test.Selenium.VerifyElementNotPresent("link=Group Leader");

            // Logout
            test.infellowship.Logout();
        }
        #endregion Search

        #region Opting In
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can opt in and opt out of the Church Directory. ")]
        public void ChurchDirectory_OptIn() {
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Opt in to the Church Directory
            test.infellowship.People_PrivacySettings_Update_OptInToDirectory(true);

            // Search
            test.infellowship.People_ChurchDirectory_Search("Group Leader");

            // Verify the results include the individual
            test.Selenium.VerifyElementPresent("link=Group Leader");

            // Opt out of the directory
            test.infellowship.People_PrivacySettings_Update_OptInToDirectory(false);

            // Verify that you cannot view the directory
            test.infellowship.People_ChurchDirectory_View();
            test.Selenium.VerifyTextPresent("In order to view the church directory you must opt-in to the directory.");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the rules for opting into the directory for children.")]
        public void ChurchDirectory_OptIn_Rules_For_Children() {
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Update a birthdate so you are less than 18.
            test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.Child, "11/15/2009", GeneralEnumerations.MaritalStatus.NA, GeneralEnumerations.Gender.NA);

            // Attempt to opt into the directory, verifying you cannot.
            test.infellowship.People_PrivacySettings_View();
            test.Selenium.VerifyElementNotPresent(InFellowshipConstants.People.PrivacySettingsConstants.Checkbox_IncludeInDirectory);
            Assert.IsFalse(test.Selenium.IsTextPresent("Include me in the church directory – This includes your name, city, state, zip, and info marked Everyone."));

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("css=a.cancel");

            // Update a birthdate so you have no birthdate
            test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.Child, "", GeneralEnumerations.MaritalStatus.NA, GeneralEnumerations.Gender.NA);

            // Attempt to opt into the directory, verifying you cannot.
            test.infellowship.People_PrivacySettings_View();
            test.Selenium.VerifyElementNotPresent(InFellowshipConstants.People.PrivacySettingsConstants.Checkbox_IncludeInDirectory);
            Assert.IsFalse(test.Selenium.IsTextPresent("Include me in the church directory – This includes your name, city, state, zip, and info marked Everyone."));

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("css=a.cancel");

            // Update a birthdate so you are over 18
            test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.Child, "11/15/1985", GeneralEnumerations.MaritalStatus.NA, GeneralEnumerations.Gender.NA);

            // Attempt to opt into the directory, verifying you can.
            test.infellowship.People_PrivacySettings_View();
            test.Selenium.VerifyElementPresent(InFellowshipConstants.People.PrivacySettingsConstants.Checkbox_IncludeInDirectory);
            Assert.IsTrue(test.Selenium.IsTextPresent("Include me in the church directory – This includes your name, city, state, and info marked Everyone."));

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure, MultipleAsserts, Timeout(1000)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the rules for opting into the directory based on your birthdate being less than 18.")]
        public void ChurchDirectory_OptIn_Rules_Birthdate_Less_Than_18() {
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.member.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Update a birthdate so you are less than 18 and you are a husband and married
            test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.Husband, "11/15/2010", GeneralEnumerations.MaritalStatus.Married, GeneralEnumerations.Gender.Male);

            // Verify the directory links are not preset
            test.Selenium.VerifyElementNotPresent(GeneralInFellowshipConstants.GeneralLinks.Link_Directory);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
            test.Selenium.VerifyElementNotPresent(GeneralInFellowshipConstants.LandingPage.Link_ChurchDirectory);

            // Attempt to opt into the directory, verifying you cannot.
            test.infellowship.People_PrivacySettings_View();
            test.Selenium.VerifyElementNotPresent(InFellowshipConstants.People.PrivacySettingsConstants.Checkbox_IncludeInDirectory);
            Assert.IsFalse(test.Selenium.IsTextPresent("Include me in the church directory – This includes your name, city, state, zip, and info marked Everyone."));

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("css=a.cancel");

            // Update a birthdate so you are less than 18 and you are a wife and married
            test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.Wife, "11/15/2010", GeneralEnumerations.MaritalStatus.Married, GeneralEnumerations.Gender.Female);

            // Verify the directory links are not preset
            test.Selenium.VerifyElementNotPresent(GeneralInFellowshipConstants.GeneralLinks.Link_Directory);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
            test.Selenium.VerifyElementNotPresent(GeneralInFellowshipConstants.LandingPage.Link_ChurchDirectory); ;

            // Attempt to opt into the directory, verifying you cannot.
            test.infellowship.People_PrivacySettings_View();
            test.Selenium.VerifyElementNotPresent(InFellowshipConstants.People.PrivacySettingsConstants.Checkbox_IncludeInDirectory);
            Assert.IsFalse(test.Selenium.IsTextPresent("Include me in the church directory – This includes your name, city, state, zip, and info marked Everyone."));

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("css=a.cancel");

            // Update a birthdate so you are less than 18 and you are a single adult
            test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.SingleAdult, "11/15/2010", GeneralEnumerations.MaritalStatus.Single, GeneralEnumerations.Gender.NA);

            // Verify the directory links are not preset
            test.Selenium.VerifyElementNotPresent(GeneralInFellowshipConstants.GeneralLinks.Link_Directory);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
            test.Selenium.VerifyElementNotPresent(GeneralInFellowshipConstants.LandingPage.Link_ChurchDirectory);

            // Attempt to opt into the directory, verifying you cannot.
            test.infellowship.People_PrivacySettings_View();
            test.Selenium.VerifyElementNotPresent(InFellowshipConstants.People.PrivacySettingsConstants.Checkbox_IncludeInDirectory);
            Assert.IsFalse(test.Selenium.IsTextPresent("Include me in the church directory – This includes your name, city, state, zip, and info marked Everyone."));

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("css=a.cancel");

            // Update a birthdate so you are less than 18 and you are a child
            test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.Child, "11/15/2010", GeneralEnumerations.MaritalStatus.Single, GeneralEnumerations.Gender.NA);

            // Verify the directory links are not preset
            test.Selenium.VerifyElementNotPresent(GeneralInFellowshipConstants.GeneralLinks.Link_Directory);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
            test.Selenium.VerifyElementNotPresent(GeneralInFellowshipConstants.LandingPage.Link_ChurchDirectory); ;

            // Attempt to opt into the directory, verifying you cannot.
            test.infellowship.People_PrivacySettings_View();
            test.Selenium.VerifyElementNotPresent(InFellowshipConstants.People.PrivacySettingsConstants.Checkbox_IncludeInDirectory);
            Assert.IsFalse(test.Selenium.IsTextPresent("Include me in the church directory – This includes your name, city, state, zip, and info marked Everyone."));

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("css=a.cancel");

            // Update a birthdate so you are less than 18 and you are of the status of Other
            test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.Other, "11/15/2010", GeneralEnumerations.MaritalStatus.Single, GeneralEnumerations.Gender.NA);

            // Verify the directory links are not preset
            test.Selenium.VerifyElementNotPresent(GeneralInFellowshipConstants.GeneralLinks.Link_Directory);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
            test.Selenium.VerifyElementNotPresent(GeneralInFellowshipConstants.LandingPage.Link_ChurchDirectory); ;

            // Attempt to opt into the directory, verifying you cannot.
            test.infellowship.People_PrivacySettings_View();
            test.Selenium.VerifyElementNotPresent(InFellowshipConstants.People.PrivacySettingsConstants.Checkbox_IncludeInDirectory);
            Assert.IsFalse(test.Selenium.IsTextPresent("Include me in the church directory – This includes your name, city, state, zip, and info marked Everyone."));

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("css=a.cancel");

            // Update a birthdate so you are less than 18 and you are of the status of visitor
            test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.FriendOfFamily, "11/15/2010", GeneralEnumerations.MaritalStatus.Single, GeneralEnumerations.Gender.NA);

            // Verify the directory links are not preset
            test.Selenium.VerifyElementNotPresent(GeneralInFellowshipConstants.GeneralLinks.Link_Directory);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
            test.Selenium.VerifyElementNotPresent(GeneralInFellowshipConstants.LandingPage.Link_ChurchDirectory); ;

            // Attempt to opt into the directory, verifying you cannot.
            test.infellowship.People_PrivacySettings_View();
            test.Selenium.VerifyElementNotPresent(InFellowshipConstants.People.PrivacySettingsConstants.Checkbox_IncludeInDirectory);
            Assert.IsFalse(test.Selenium.IsTextPresent("Include me in the church directory – This includes your name, city, state, zip, and info marked Everyone."));

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("css=a.cancel");

            // Revert the changes
            test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.SingleAdult, "11/15/1985", GeneralEnumerations.MaritalStatus.Single, GeneralEnumerations.Gender.NA);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the rules for opting into the directory based on you not having a birthdate.")]
        public void ChurchDirectory_OptIn_Rules_No_Birthdate() {
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Update a birthdate so you have no birthdate and you are a husband and married
            test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.Husband, string.Empty, GeneralEnumerations.MaritalStatus.Married, GeneralEnumerations.Gender.Male);

            // Verify the directory links are preset
            test.Selenium.VerifyElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_Directory);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
            test.Selenium.VerifyElementPresent(GeneralInFellowshipConstants.LandingPage.Link_ChurchDirectory);

            // Attempt to opt into the directory, verifying you can.
            test.infellowship.People_PrivacySettings_View();
            test.Selenium.VerifyElementPresent(InFellowshipConstants.People.PrivacySettingsConstants.Checkbox_IncludeInDirectory);
            Assert.IsTrue(test.Selenium.IsTextPresent("Include me in the church directory – This includes your name, city, state, and info marked Everyone."));

            // Cancel
            //test.Selenium.ClickAndWaitForPageToLoad("css=a.cancel");

            // Update a birthdate so you have no birthdate and you are a wife and married
            test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.Wife, string.Empty, GeneralEnumerations.MaritalStatus.Married, GeneralEnumerations.Gender.Female);

            // Verify the directory links are preset
            test.Selenium.VerifyElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_Directory);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
            test.Selenium.VerifyElementPresent(GeneralInFellowshipConstants.LandingPage.Link_ChurchDirectory); ;

            // Attempt to opt into the directory, verifying you can.
            test.infellowship.People_PrivacySettings_View();
            test.Selenium.VerifyElementPresent(InFellowshipConstants.People.PrivacySettingsConstants.Checkbox_IncludeInDirectory);
            Assert.IsTrue(test.Selenium.IsTextPresent("Include me in the church directory – This includes your name, city, state, and info marked Everyone."));

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("css=a.cancel");

            // Update a birthdate so you are less than 18 and you are a single adult
            test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.SingleAdult, string.Empty, GeneralEnumerations.MaritalStatus.Single, GeneralEnumerations.Gender.NA);

            // Verify the directory links are not preset
            test.Selenium.VerifyElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_Directory);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
            test.Selenium.VerifyElementPresent(GeneralInFellowshipConstants.LandingPage.Link_ChurchDirectory);

            // Attempt to opt into the directory, verifying you cannot.
            test.infellowship.People_PrivacySettings_View();
            test.Selenium.VerifyElementPresent(InFellowshipConstants.People.PrivacySettingsConstants.Checkbox_IncludeInDirectory);
            Assert.IsTrue(test.Selenium.IsTextPresent("Include me in the church directory – This includes your name, city, state, and info marked Everyone."));

            test.infellowship.Logout();
            test.infellowship.ReLoginInFellowship("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // Update a birthdate so you have no birthdate and you are a child
            test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.Child, string.Empty, GeneralEnumerations.MaritalStatus.Single, GeneralEnumerations.Gender.NA);

            // Verify the directory links are not preset
            test.Selenium.VerifyElementNotPresent(GeneralInFellowshipConstants.GeneralLinks.Link_Directory);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
            test.Selenium.VerifyElementNotPresent(GeneralInFellowshipConstants.LandingPage.Link_ChurchDirectory); ;

            // Attempt to opt into the directory, verifying you cannot.
            test.infellowship.People_PrivacySettings_View();
            test.Selenium.VerifyElementNotPresent(InFellowshipConstants.People.PrivacySettingsConstants.Checkbox_IncludeInDirectory);
            Assert.IsFalse(test.Selenium.IsTextPresent("Include me in the church directory – This includes your name, city, state, and info marked Everyone."));

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("css=a.cancel");

            // Update a birthdate so you do not have one and you are of the status of Other
            test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.Other, string.Empty, GeneralEnumerations.MaritalStatus.Single, GeneralEnumerations.Gender.NA);

            // Verify the directory links are preset
            test.Selenium.VerifyElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_Directory);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
            test.Selenium.VerifyElementPresent(GeneralInFellowshipConstants.LandingPage.Link_ChurchDirectory); ;

            // Attempt to opt into the directory, verifying you cannot.
            test.infellowship.People_PrivacySettings_View();
            test.Selenium.VerifyElementPresent(InFellowshipConstants.People.PrivacySettingsConstants.Checkbox_IncludeInDirectory);
            Assert.IsTrue(test.Selenium.IsTextPresent("Include me in the church directory – This includes your name, city, state, and info marked Everyone."));

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("css=a.cancel");

            // Update a birthdate so you do not have one and you are of the status of visitor
            test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.FriendOfFamily, string.Empty, GeneralEnumerations.MaritalStatus.Single, GeneralEnumerations.Gender.NA);

            // Verify the directory links are not preset
            test.Selenium.VerifyElementNotPresent(GeneralInFellowshipConstants.GeneralLinks.Link_Directory);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
            test.Selenium.VerifyElementNotPresent(GeneralInFellowshipConstants.LandingPage.Link_ChurchDirectory); ;

            // Attempt to opt into the directory, verifying you cannot.
            test.infellowship.People_PrivacySettings_View();
            test.Selenium.VerifyElementNotPresent(InFellowshipConstants.People.PrivacySettingsConstants.Checkbox_IncludeInDirectory);
            Assert.IsFalse(test.Selenium.IsTextPresent("Include me in the church directory – This includes your name, city, state, and info marked Everyone."));

            // Cancel
            test.Selenium.ClickAndWaitForPageToLoad("css=a.cancel");

            // Revert the changes
            test.infellowship.People_ProfileEditor_Update_PersonalInfo(null, null, null, GeneralEnumerations.HouseholdPositonWeb.SingleAdult, "11/15/1985", GeneralEnumerations.MaritalStatus.Single, GeneralEnumerations.Gender.NA);

            // Logout
            test.infellowship.Logout();
        }
        #endregion Opting In

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can view the Church Directory.")]
        public void ChurchDirectory_View() {
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("infellowship.user@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Church Directory
            test.infellowship.People_ChurchDirectory_View();

            // Verify the page loads
            test.Selenium.VerifyElementPresent("search_query");
            test.Selenium.VerifyElementPresent(TableIds.InFellowship_ChurchDirectory);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies visitors are not present when viewing an individual in the church directory.")]
        public void ChurchDirectory_View_Visitors_Not_Present() {
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

            // View an individual in the church directory
            test.infellowship.People_ChurchDirectory_View_Individual("Matthew Sneeden");

            // Verify Visitors are not present
            Assert.IsFalse(test.Selenium.IsTextPresent("Visitor Sneeden"));

            // Logout
            test.infellowship.Logout();
        }
    }
}