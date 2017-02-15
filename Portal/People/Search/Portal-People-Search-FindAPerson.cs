using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Globalization;


namespace FTTests.Portal.People
{
    public struct SearchConstants
    {
        public const string Add_another = "link=Add another";
        public const string Add_an_individual = "link=Add an individual";
        public const string Edit_address = "//a[contains(@href, '/people/Household/Address/Edit.aspx')]";
        public const string Edit_individual = "//a[contains(@href, '/People/Individual/Edit.aspx') and not(@class='edit_individual')]";
        public const string Edit_communications = "//a[contains(@href, 'communications/edit')]";
        public const string Enter_a_contact_form = "link=Enter a contact form";
    }


    [TestFixture]
    public class Portal_People_Search_FindAPerson : FixtureBase
    {
        private string _indID;
        private string _hsdID;
        private string _groupTypeName = "Individual Detail Group Type";
        private string _groupName = "Individual Detail Group";
        private string _spanOfCareName = "Individual Detail Span of Care";

        [FixtureSetUp]
        public void FixtureSetUp()
        {
            _indID = base.SQL.People_Individuals_FetchID(15, "Matthew Sneeden").ToString();
            _hsdID = base.SQL.People_Households_FetchID(15, Convert.ToInt32(_indID)).ToString();
            base.SQL.Groups_GroupType_Create(254, "Group Admin", _groupTypeName, new List<int>() { 11 });
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _groupName, null, "11/15/2009");
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _groupName, "Group Manager", GeneralEnumerations.GroupRolesPortalUser.Manager);
            base.SQL.Groups_Group_AddManagerOrViewer(254, _groupTypeName, _groupName, "Group Viewer", GeneralEnumerations.GroupRolesPortalUser.Viewer);
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Leader", "Leader");
            base.SQL.Groups_SpanOfCare_Create(254, _spanOfCareName, "Bryan Mikaelian", "SOC Owner", new List<string>() { _groupTypeName });
        }

        [FixtureTearDown]
        public void FixtureTearDown()
        {
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, _spanOfCareName);
        }

        #region Search
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Verifies functionality when logged in with a user lacking the people contact right performing a people search.")]
        public void People_Search_FindAPerson_NoPeopleContact()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("PR_PeopleContact", "Pa$$w0rd");

            // Search for an individual
            test.Portal.People_SearchIndividual(test.Portal.PortalUser);

            // Verify the 'Enter a contact form' gear option is not present
            test.Selenium.VerifyElementNotPresent(Portal.People.SearchConstants.Enter_a_contact_form);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies functionality when logged in with a user lacking the people edit right performing a people search.")]
        public void People_Search_NoPeopleEdit()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("PR_PeopleEdit", "Pa$$w0rd");

            // Search for an individual
            test.Portal.People_SearchIndividual(test.Portal.PortalUser);

            // Verify the 'Edit individual' gear option is not present
            test.Selenium.VerifyElementNotPresent(Portal.People.SearchConstants.Edit_individual);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Performs a people search using a status.")]
        public void People_Search_Status()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Search by status 'Child of Member'
            test.Portal.People_FindAPerson_Individual(null, null, null, false, "Child of Member", null, null, null, false);

            // Verify the search results
            test.Portal.People_VerifyPeopleSearchResults("Name", "Child of Member");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Performs a people search using an address.")]
        public void People_Search_Address()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Search by address '9616 Armour'
            test.Portal.People_FindAPerson_Individual(null, "9616 Armour", null, false, null, null, null, null, false);

            // Verify the search results
            test.Portal.People_VerifyPeopleSearchResults("Address", "9616 Armour");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Performs a people search using a communication value.")]
        public void People_Search_Communications_Phone()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Search by phone '6302172170'
            test.Portal.People_FindAPerson_Individual(null, null, "6302172170", false, null, null, null, null, false);

            // Verify the search results
            test.Portal.People_VerifyPeopleSearchResults("Phone", "6302172170");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Performs a people search 'lastname, firstname'")]
        public void People_Search_LastnameFirstname()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Search in the format "lastname, firstname"
            test.Portal.People_Search(SearchByConstants.Individual, "Sneeden, Matthew");

            // Verify the search results
            Assert.AreEqual("Matthew Sneeden", test.Selenium.GetText(TableIds.People_Individuals + "/tbody/tr[2]/td[2]/a"));
            Assert.AreEqual(2, test.Selenium.GetXpathCount(TableIds.People_Individuals + "/tbody/tr"));

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Performs a people search using 'date of birth' in an international church.")]
        public void People_Search_FindAPerson_DateOfBirth_International()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("msneeden", "Pa$$w0rd", "QAEUNLX0C6");

            // Search by date of birth
            test.Portal.People_Search(SearchByConstants.Date_of_birth, "02/09/1982");

            // Verify the search results
            Assert.AreEqual("Matthew Sneeden", test.Selenium.GetText(TableIds.People_Individuals + "/tbody/tr[2]/td[2]/a"));
            Assert.IsTrue(test.Selenium.GetText(TableIds.People_Individuals + "/tbody/tr[2]/td[3]").Contains("02/09/1982"), "Date of birth was not in the correct format.");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to perform a search by date of birth without specifying a date.")]
        public void People_Search_FindAPerson_DateOfBirth_NoData()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Attempt to search for an individual by date of birth with no date of birth provided
            test.Portal.People_Search(SearchByConstants.Date_of_birth, null);

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Search

        #region Gear Commands
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Selects 'View household' off of the people search gear.")]
        public void People_Search_FindAPerson_Gear_ViewHousehold()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Search for an individual
            test.Portal.People_SearchIndividual("Matthew Sneeden");

            // Select "View household" from the gear
            test.GeneralMethods.SelectOptionFromGear(1, "View household");

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.Search_ViewIndividual_ViewHousehold));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.Search_ViewIndividual_ViewHousehold);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Selects 'View individual' off of the people search gear.")]
        public void People_Search_FindAPerson_Gear_ViewIndividual()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Search for an individual
            test.Portal.People_SearchIndividual("Matthew Sneeden");

            // Select "View individual" from the gear
            test.GeneralMethods.SelectOptionFromGear(1, "View individual");

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.Search_IndividualDetail));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.Search_IndividualDetail);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Selects 'Edit individual' off of the people search gear.")]
        public void People_Search_FindAPerson_Gear_EditIndividual()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Search for an individual
            test.Portal.People_SearchIndividual("Matthew Sneeden");

            // Select "Edit individual" from the gear
            test.GeneralMethods.SelectOptionFromGear(1, "Edit individual");

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.Search_EditAnIndividual));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.Search_EditAnIndividual);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Selects 'Enter a contact form' off of the people search gear.")]
        public void People_Search_FindAPerson_Gear_EnterAContactForm()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Search for an individual
            test.Portal.People_SearchIndividual("Matthew Sneeden");

            // Select "Enter a contact form" from the gear
            test.GeneralMethods.SelectOptionFromGear(1, "Enter a contact form");

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.Search_ViewIndividual_ViewHousehold_EnterAContactForm));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.Search_ViewIndividual_ViewHousehold_EnterAContactForm);

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Gear Commands

        #region View Household
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Verifies functionality when logged in with a user lacking the people contact right views the household view.")]
        public void People_Search_FindAPerson_ViewHousehold_NoPeopleContact()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("PR_PeopleContact", "Pa$$w0rd");

            // Search for an individual and view the household
            test.Portal.People_ViewHousehold(test.Portal.PortalUser);

            // Verify the 'Contact form' gear link is not present
            test.Selenium.VerifyElementNotPresent("link=Contact form");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user cannot navigate directly to the add contact form page w/out the people contact right enabled.")]
        public void People_Search_FindAPerson_ViewHousehold_NoPeopleContact_NavigateDirectly()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("PR_PeopleContact", "Pa$$w0rd");

            // Attempt to open the page directly
            test.GeneralMethods.OpenURLExpecting403(string.Format("/people/ContactForm/Select.aspx?HouseholdID={0}&Source=HouseholdIndex&AllowRedirect=True", _hsdID));
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to add an email address that is missing an '@'.")]
        public void People_HH_Communications_CreateEmailMissingAtSymbol()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View a household
            test.Portal.People_ViewHousehold("Matthew Sneeden");

            // Edit the communications
            test.Selenium.ClickAndWaitForPageToLoad(Portal.People.SearchConstants.Edit_communications);

            // Attempt to save an improperly formatted email address
            test.Selenium.Type("home_email", "asdf.com");

            test.Selenium.ClickAndWaitForPageToLoad("btn");

            // Verify error(s) displayed to user
            test.Selenium.VerifyTextPresent(new string[] { TextConstants.ErrorHeadingSingular, "Please verify all e-mail address records are properly formatted." });

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to add an email address that is missing a '.'.")]
        public void People_HH_Communications_CreateEmailMissingPeriod()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View a household
            test.Portal.People_ViewHousehold("Matthew Sneeden");

            // Edit the communications
            test.Selenium.ClickAndWaitForPageToLoad(Portal.People.SearchConstants.Edit_communications);

            // Attempt to save an improperly formatted email address
            test.Selenium.Type("home_email", "asdf@asdf");

            test.Selenium.ClickAndWaitForPageToLoad("btn");

            // Verify error(s) displayed to user
            test.Selenium.VerifyTextPresent(new string[] { TextConstants.ErrorHeadingSingular, "Please verify all e-mail address records are properly formatted." });

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the household for an individual and verifies the saved image is present, not the avatar.")]
        public void People_Search_FindAPerson_ViewHousehold()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // View a household
            test.Portal.People_ViewHousehold(test.Portal.PortalUser);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.Search_ViewIndividual_ViewHousehold));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.Search_ViewIndividual_ViewHousehold);

            // Verify image is present for individual, not avatar
            Assert.IsTrue(test.Selenium.IsElementPresent(string.Format("//table[@class='grid']/tbody/tr[*]/td[2]/a[text()='{0}' and ancestor::tr/td[1]/a[contains(@href, '/ImageHandler.ashx?AppID=1&PKID=')]]", test.Portal.PortalUser)));
            Assert.IsFalse(test.Selenium.IsElementPresent(string.Format("//table[@class='grid']/tbody/tr[*]/td[2]/a[text()='{0}' and ancestor::tr/td[1]/img[contains(@src, 'avatar_')]]", test.Portal.PortalUser)));

            // Logout of portal
            test.Portal.Logout();
        }

        #region Actions
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        public void People_Search_FindAPerson_ViewHousehold_AddAnIndividual()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View a household
            test.Portal.People_ViewHousehold("Matthew Sneeden");

            // Add an individual
            test.Selenium.ClickAndWaitForPageToLoad(Portal.People.SearchConstants.Add_an_individual);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.Search_ViewIndividual_ViewHousehold_AddAnIndividual));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.Search_ViewIndividual_ViewHousehold_AddAnIndividual);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        public void People_Search_FindAPerson_ViewHousehold_EnterAContactForm()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View a household
            test.Portal.People_ViewHousehold("Matthew Sneeden");

            // Enter a contact form
            test.Selenium.ClickAndWaitForPageToLoad(Portal.People.SearchConstants.Enter_a_contact_form);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.Search_ViewIndividual_ViewHousehold_EnterAContactForm));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.Search_ViewIndividual_ViewHousehold_EnterAContactForm);

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Actions

        #region Phone, Email, Web
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        public void People_Search_FindAPerson_ViewHousehold_Edit()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View a household
            test.Portal.People_ViewHousehold("Matthew Sneeden");

            // Edit communications
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Edit);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.Search_ViewIndividual_ViewHousehold_EditCommunications));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.Search_ViewIndividual_ViewHousehold_EditCommunications);

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Phone, Email, Web

        #region Addresses
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        public void People_Search_FindAPerson_ViewHousehold_EditThisAddress()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View a household
            test.Portal.People_ViewHousehold(test.Portal.PortalUser);

            // Edit an address
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/people/Household/Address/Edit.aspx')]");

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.Search_ViewIndividual_ViewHousehold_EditThisAddress));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.Search_ViewIndividual_ViewHousehold_EditThisAddress);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        public void People_Search_FindAPerson_ViewHousehold_AddAnother()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View a household
            test.Portal.People_ViewHousehold(test.Portal.PortalUser);

            // Add an address
            test.Selenium.ClickAndWaitForPageToLoad(Portal.People.SearchConstants.Add_another);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.Search_ViewIndividual_ViewHousehold_AddAnother));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.Search_ViewIndividual_ViewHousehold_AddAnother);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        public void People_Search_FindAPerson_ViewHousehold_AddAddress_NoData()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View a household
            test.Portal.People_ViewHousehold(test.Portal.PortalUser);

            // Add an address
            test.Selenium.ClickAndWaitForPageToLoad(Portal.People.SearchConstants.Add_another);

            // Submit with no data
            test.Selenium.ClickAndWaitForPageToLoad("save_address_button");

            // Verify error messages
            test.Selenium.VerifyTextPresent(new string[] { TextConstants.ErrorHeadingPlural, "Address type is required.", "Household/Person is required" });

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Addresses

        #region Change Image
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        public void People_Search_FindAPerson_ViewHousehold_ChangeImage()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View a household
            test.Portal.People_ViewHousehold("Matthew Sneeden");

            // Change the image for the household
            test.Selenium.ClickAndWaitForPageToLoad("link=Change");

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.Search_ViewIndividual_ViewHousehold_EditHousehold));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.Search_ViewIndividual_ViewHousehold_EditHousehold);

            // Verify text
            test.Selenium.VerifyTextPresent("Upload a photo");

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Change Image

        #region Notes
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        public void People_Search_FindAPerson_ViewHousehold_Note()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View a household
            test.Portal.People_ViewHousehold("Matthew Sneeden");

            // Add a note
            test.Selenium.ClickAndWaitForPageToLoad("link=Note");

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.Search_ViewIndividual_ViewHousehold_Note));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.Search_ViewIndividual_ViewHousehold_Note);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        public void People_Search_FindAPerson_ViewHousehold_Notes()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View a household
            test.Portal.People_ViewHousehold("Matthew Sneeden");

            // Select 'Notes'
            test.Selenium.ClickAndWaitForPageToLoad("link=notes");

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.Search_ViewIndividual_ViewHousehold_Note));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.Search_ViewIndividual_ViewHousehold_Note);

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Notes

        #region Contact Form
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        public void People_Search_FindAPerson_ViewHousehold_ContactForm()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View a household
            test.Portal.People_ViewHousehold("Matthew Sneeden");

            // Select contact form
            test.Selenium.ClickAndWaitForPageToLoad("link=Contact form");

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.Search_ViewIndividual_ViewHousehold_EnterAContactForm));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.Search_ViewIndividual_ViewHousehold_EnterAContactForm);

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Contact Form

        #region Contact Items
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        public void People_Search_FindAPerson_ViewHousehold_ContactItems()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View a household
            test.Portal.People_ViewHousehold("Matthew Sneeden");

            // Select contact items
            test.Selenium.ClickAndWaitForPageToLoad("link=contact items");

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.Search_ViewIndividual_ViewHousehold_ContactItems));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.Search_ViewIndividual_ViewHousehold_ContactItems);

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Contact Items
        #endregion View Household

        #region Edit Household
        [Test, RepeatOnFailure(Order = 1)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Edits the Household/Individual values through the new Edit HH/IND page in Full View")]
        public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_FullView_AllFields()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.householdedit", "FT4life!", "dc");

            // Edit household through the new Edit HH/IND page
            TestLog.WriteLine("Edit the HH/IND values.");
            test.Portal.People_FindAPerson_Household_Edit_HouseholdIndividualValues_FullView(15, "FT HouseholdEdit", "111-111-1111", "email@Household.com", "222-222-2222", "333-333-3333", "444-444-4444", "email@Personal.com", "Active Member", "Auto Sub Status", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), "Comment", "Head", "Single", "Male");

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure(Order = 2)]
        [Author("David Martin")]
        [Description("Edits the Household/Individual values through the new Edit HH/IND page in Full View")]
        public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_FullView_NoHomePhone()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.householdedit", "FT4life!", "dc");

            // Edit household through the new Edit HH/IND page
            TestLog.WriteLine("Edit the HH/IND values.");
            test.Portal.People_FindAPerson_Household_Edit_HouseholdIndividualValues_FullView(15, "FT HouseholdEdit", null, "email@Household.com", "222-222-2222", "333-333-3333", "444-444-4444", "email@Personal.com", "Active Member", "Auto Sub Status", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), "Comment", "Head", "Single", "Male");

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure(Order = 3)]
        [Author("David Martin")]
        [Description("Edits the Household/Individual values through the new Edit HH/IND page in Full View")]
        public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_FullView_NoHomeEmail()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.householdedit", "FT4life!", "dc");

            // Edit household through the new Edit HH/IND page
            TestLog.WriteLine("Edit the HH/IND values.");
            test.Portal.People_FindAPerson_Household_Edit_HouseholdIndividualValues_FullView(15, "FT HouseholdEdit", "111-111-1111", null, "222-222-2222", "333-333-3333", "444-444-4444", "email@Personal.com", "Active Member", "Auto Sub Status", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), "Comment", "Head", "Single", "Female");

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure(Order = 4)]
        [Author("David Martin")]
        [Description("Edits the Household/Individual values through the new Edit HH/IND page in Full View")]
        public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_FullView_NoMobilePhone()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.householdedit", "FT4life!", "dc");

            // Edit household through the new Edit HH/IND page
            TestLog.WriteLine("Edit the HH/IND values.");
            test.Portal.People_FindAPerson_Household_Edit_HouseholdIndividualValues_FullView(15, "FT HouseholdEdit", "111-111-1111", "email@Household.com", null, "333-333-3333", "444-444-4444", "email@Personal.com", "Active Member", "Auto Sub Status", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), "Comment", "Head", "Single", "Female");

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure(Order = 5)]
        [Author("David Martin")]
        [Description("Edits the Household/Individual values through the new Edit HH/IND page in Full View")]
        public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_FullView_NoWorkPhone()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.householdedit", "FT4life!", "dc");

            // Edit household through the new Edit HH/IND page
            TestLog.WriteLine("Edit the HH/IND values.");
            test.Portal.People_FindAPerson_Household_Edit_HouseholdIndividualValues_FullView(15, "FT HouseholdEdit", "111-111-1111", "email@Household.com", "222-222-2222", null, "444-444-4444", "email@Personal.com", "Active Member", "Auto Sub Status", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), "Comment", "Head", "Single", "Male");

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure(Order = 6)]
        [Author("David Martin")]
        [Description("Edits the Household/Individual values through the new Edit HH/IND page in Full View")]
        public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_FullView_NoEmergencyPhone()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.householdedit", "FT4life!", "dc");

            // Edit household through the new Edit HH/IND page
            TestLog.WriteLine("Edit the HH/IND values.");
            test.Portal.People_FindAPerson_Household_Edit_HouseholdIndividualValues_FullView(15, "FT HouseholdEdit", "111-111-1111", "email@Household.com", "222-222-2222", "333-333-3333", null, "email@Personal.com", "Active Member", "Auto Sub Status", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), "Comment", "Head", "Single", "Male");

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure(Order = 7)]
        [Author("David Martin")]
        [Description("Edits the Household/Individual values through the new Edit HH/IND page in Full View")]
        public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_FullView_NoPersonalEmail()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.householdedit", "FT4life!", "dc");

            // Edit household through the new Edit HH/IND page
            TestLog.WriteLine("Edit the HH/IND values.");
            test.Portal.People_FindAPerson_Household_Edit_HouseholdIndividualValues_FullView(15, "FT HouseholdEdit", "111-111-1111", "email@Household.com", "222-222-2222", "333-333-3333", "444-444-4444", null, "Active Member", "Auto Sub Status", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), "Comment", "Head", "Single", "Male");

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure(Order = 8)]
        [Author("David Martin")]
        [Description("Edits the Household/Individual values through the new Edit HH/IND page in Full View")]
        public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_FullView_NoStatus()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.householdedit", "FT4life!", "dc");

            // Edit household through the new Edit HH/IND page
            TestLog.WriteLine("Edit the HH/IND values.");
            test.Portal.People_FindAPerson_Household_Edit_HouseholdIndividualValues_FullView(15, "FT HouseholdEdit", "111-111-1111", "email@Household.com", "222-222-2222", "333-333-3333", "444-444-4444", "email@Personal.com", null, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), "Comment", "Head", "Single", "Male");

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure(Order = 9)]
        [Author("David Martin")]
        [Description("Edits the Household/Individual values through the new Edit HH/IND page in Full View")]
        public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_FullView_NoSubStatus()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.householdedit", "FT4life!", "dc");

            // Edit household through the new Edit HH/IND page
            TestLog.WriteLine("Edit the HH/IND values.");
            test.Portal.People_FindAPerson_Household_Edit_HouseholdIndividualValues_FullView(15, "FT HouseholdEdit", "111-111-1111", "email@Household.com", "222-222-2222", "333-333-3333", "444-444-4444", "email@Personal.com", "Active Member", null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), "Comment", "Head", "Single", "Male");

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure(Order = 10)]
        [Author("David Martin")]
        [Description("Edits the Household/Individual values through the new Edit HH/IND page in Full View")]
        public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_FullView_NoStatusDate()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.householdedit", "FT4life!", "dc");

            // Edit household through the new Edit HH/IND page
            TestLog.WriteLine("Edit the HH/IND values.");
            test.Portal.People_FindAPerson_Household_Edit_HouseholdIndividualValues_FullView(15, "FT HouseholdEdit", "111-111-1111", "email@Household.com", "222-222-2222", "333-333-3333", "444-444-4444", "email@Personal.com", "Active Member", "Auto Sub Status", null, "Comment", "Head", "Single", "Female");

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure(Order = 11)]
        [Author("David Martin")]
        [Description("Edits the Household/Individual values through the new Edit HH/IND page in Full View")]
        public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_FullView_NoStatusComment()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.householdedit", "FT4life!", "dc");

            // Edit household through the new Edit HH/IND page
            TestLog.WriteLine("Edit the HH/IND values.");
            test.Portal.People_FindAPerson_Household_Edit_HouseholdIndividualValues_FullView(15, "FT HouseholdEdit", "111-111-1111", "email@Household.com", "222-222-2222", "333-333-3333", "444-444-4444", "email@Personal.com", "Active Member", "Auto Sub Status", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), string.Empty, "Head", "Single", "Male");

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure(Order = 12)]
        [Author("David Martin")]
        [Description("Edits the Household/Individual values through the new Edit HH/IND page in Full View")]
        public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_FullView_NoHouseholdPosition()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.householdedit", "FT4life!", "dc");

            // Edit household through the new Edit HH/IND page
            TestLog.WriteLine("Edit the HH/IND values.");
            test.Portal.People_FindAPerson_Household_Edit_HouseholdIndividualValues_FullView(15, "FT HouseholdEdit", "111-111-1111", "email@Household.com", "222-222-2222", "333-333-3333", "444-444-4444", "email@Personal.com", "Active Member", "Auto Sub Status", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), "Comment", null, "Single", "Male");

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure(Order = 13)]
        [Author("David Martin")]
        [Description("Edits the Household/Individual values through the new Edit HH/IND page in Full View")]
        public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_FullView_NoMaritalStatus()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.householdedit", "FT4life!", "dc");

            // Edit household through the new Edit HH/IND page
            TestLog.WriteLine("Edit the HH/IND values.");
            test.Portal.People_FindAPerson_Household_Edit_HouseholdIndividualValues_FullView(15, "FT HouseholdEdit", "111-111-1111", "email@Household.com", "222-222-2222", "333-333-3333", "444-444-4444", "email@Personal.com", "Active Member", "Auto Sub Status", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), "Comment", "Head", null, null);

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure(Order = 14)]
        [Author("David Martin")]
        [Description("Edits the Communication values for the Household/Individuals through the new Edit HH/IND page in Tabbed View")]
        public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_CommunicationsTab()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.householdedit", "FT4life!", "dc");

            // Edit the Communication values through the new Edit HH/IND page in Tabbed View
            TestLog.WriteLine("Edit Communications through HH/IND edit page.");
            test.Portal.People_FindAPerson_Household_Edit_HouseholdIndividualCommunications_TabbedView(15, "FT HouseholdEdit", "999-999-9999", "commtab@Household.com", "888-888-8888", "777-777-7777", "666-666-6666", "commtab@Personal.com");

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure(Order = 15)]
        [Author("David Martin")]
        [Description("Edits all of the Status/Sub Status values for the individuals through the new Edit HH/IND page in Tabbed View")]
        public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_StatusSubStatusTab_AllFields()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Edit the Status/Sub Status values through the new Edit HH/IND page in Tabbed View
            TestLog.WriteLine("Edit Status/Sub Status through HH/IND edit page.");
            test.Portal.People_FindAPerson_Household_Edit_IndividualStatus_TabbedView(15, "FT HouseholdEdit", "Attendee", "First Time Visitor", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(-1), "Nice Comment");

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure(Order = 16)]
        [Author("David Martin")]
        [Description("Edits the Status/Sub Status values for the individuals through the new Edit HH/IND page in Tabbed View without comment")]
        public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_StatusSubStatusTab_NoComment()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Edit the Status/Sub Status values through the new Edit HH/IND page in Tabbed View
            TestLog.WriteLine("Edit Status/Sub Status through HH/IND edit page.");
            test.Portal.People_FindAPerson_Household_Edit_IndividualStatus_TabbedView(15, "FT HouseholdEdit", "Attendee", "CAR", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), string.Empty);

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure(Order = 17)]
        [Author("David Martin")]
        [Description("Edits the Status/Sub Status values for the individuals through the new Edit HH/IND page in Tabbed View without status date or comment")]
        public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_StatusSubStatusTab_NoStatusDateOrComment()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Edit the Status/Sub Status values through the new Edit HH/IND page in Tabbed View
            TestLog.WriteLine("Edit Status/Sub Status through HH/IND edit page.");
            test.Portal.People_FindAPerson_Household_Edit_IndividualStatus_TabbedView(15, "FT HouseholdEdit", "Attendee", "COL", null, null);

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure(Order = 18)]
        [Author("David Martin")]
        [Description("Edits the Status only for the individuals through the new Edit HH/IND page in Tabbed View")]
        public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_StatusSubStatusTab_StatusOnly()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Edit the Status/Sub Status values through the new Edit HH/IND page in Tabbed View
            TestLog.WriteLine("Edit Status/Sub Status through HH/IND edit page.");
            test.Portal.People_FindAPerson_Household_Edit_IndividualStatus_TabbedView(15, "FT HouseholdEdit", "Member", null, null, null);

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure(Order = 19)]
        [Author("David Martin")]
        [Description("Edits the household position and arital status values for the individuals through the new Edit HH/IND page in Tabbed View")]
        public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_PositionMaritalStatusTab_NoMaritalStatus()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.householdedit", "FT4life!", "dc");

            // Edit the HH position and Marital status values through the new Edit HH/IND page in Tabbed View
            TestLog.WriteLine("Edit HH position and Marital status through HH/IND edit page.");
            test.Portal.People_FindAPerson_Household_Edit_PositionMaritalStatus_TabbedView(15, "FT HouseholdEdit", "Other", null, null);

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure(Order = 20)]
        [Author("David Martin")]
        [Description("Edits the household position and arital status values for the individuals through the new Edit HH/IND page in Tabbed View")]
        public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_PositionMaritalStatusTab_NoHouseholdPosition()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.householdedit", "FT4life!", "dc");

            // Edit the HH position and Marital status values through the new Edit HH/IND page in Tabbed View
            TestLog.WriteLine("Edit HH position and Marital status through HH/IND edit page.");
            test.Portal.People_FindAPerson_Household_Edit_PositionMaritalStatus_TabbedView(15, "FT HouseholdEdit", null, null, null);

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }
        // Suchitra Changes  
        [Test, RepeatOnFailure(Order = 21)]
        [Author("Suchitra Patnam")]
        [Description("Edits the Household/Individual values through the new Edit HH/IND page in Full View")]
        public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_FullView_NoGender()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.householdedit", "FT4life!", "dc");

            // Edit household through the new Edit HH/IND page
            TestLog.WriteLine("Edit the HH/IND values.");
            test.Portal.People_FindAPerson_Household_Edit_HouseholdIndividualValues_FullView(15, "FT HouseholdEdit", "111-111-1111", "email@Household.com", "222-222-2222", "333-333-3333", "444-444-4444", "email@Personal.com", "Active Member", "Auto Sub Status", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), "Comment", "Head", null, null);

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }

        // Suchitra Changes  
        [Test, RepeatOnFailure(Order = 22)]
        [Author("Suchitra Patnam")]
        [Description("Edits the household position and arital status values for the individuals through the new Edit HH/IND page in Tabbed View")]
        public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_PositionMaritalStatusTab_NoGender()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.householdedit", "FT4life!", "dc");

            // Edit the HH position and Marital status values through the new Edit HH/IND page in Tabbed View
            TestLog.WriteLine("Edit HH position and Marital status through HH/IND edit page.");
            test.Portal.People_FindAPerson_Household_Edit_PositionMaritalStatus_TabbedView(15, "FT HouseholdEdit", "Other", null, null);

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies you can access the HH/IND edit page through the Gear options on the individual profile page")]
        public void People_Search_FindAPerson_ViewIndividual_EditHouseholdIndividual_GearOptions()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login to Portal");
            test.Portal.Login("ft.householdedit", "FT4life!", "dc");

            // Navigate to the HH/IND edit page through the gear options on the individual profile page
            TestLog.WriteLine("Navigate to HH/IND page through gear options on individual profile page");
            test.Portal.People_FindAPerson_Household_Edit_HouseholdIndividualValues_GearOptions(15, "FT HouseholdEdit");

            // Logout of Portal
            TestLog.WriteLine("Logout of Portal");
            test.Portal.Logout();

        }


        #endregion Edit Household

        #region View Individual
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user can add an individual to a group from the view individual page.")]
        public void People_Search_FindAPerson_ViewIndividual_AddIndividualToGroup()
        {
            // Set initial conditions
            string groupTypeName = "Add Individual to Group - GT";
            string groupName = "Add Individual to Group";
            string individualName = "Matthew Sneeden";
            try
            {
                this.SQL.Groups_GroupType_Delete(15, groupTypeName);
                this.SQL.Groups_Group_Delete(15, groupName);
                this.SQL.Groups_GroupType_Create(15, individualName, groupTypeName, new List<int>(10));
                this.SQL.Groups_Group_Create(15, groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());

                // Login to Portal
                TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.Login();

                // View the individual
                test.Portal.People_ViewIndividual(individualName);

                // Attempt to add the individual to the group
                test.Selenium.Click("lnk_add_to_group");
                test.Selenium.SelectAndWaitForCondition("ddlGroupTypes", groupTypeName, JavaScriptMethods.OptionExistsInSelect("ddlGroups", groupName), "10000");
                test.Selenium.Select("ddlGroups", groupName);
                test.Selenium.ClickAndWaitForCondition("//input[@value='Add to group']", "selenium.isElementPresent(\"xpath=//div[@id='add_to_group' and contains(@style, 'display: none;')]\");", "10000");

                // Verify the individual was added to the group
                Assert.IsTrue(test.Selenium.IsElementPresent(string.Format("//table[@class='grid gutter_bottom_none']/tbody/tr[*]/td[2]/a[text()='{0}']", groupName)));

                // Logout of Portal
                test.Portal.Logout();
            }
            finally
            {
                this.SQL.Groups_GroupType_Delete(15, groupTypeName);
            }
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user cannot see 'People Lists' when logged in as a non-linked user.")]
        public void People_Search_FindAPerson_ViewIndividual_NonLinkedUser()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("msneedenunlinked", "Pa$$w0rd", "dc");

            // View the individual
            test.Portal.People_ViewIndividual("Matthew Sneeden");

            // Verify people list controls are not visible
            Assert.IsFalse(test.Selenium.IsElementPresent("//h6/a[text()='Add to list']"));
            Assert.IsFalse(test.Selenium.IsElementPresent("//table[@id='people_lists']"));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the add a assignment link is not a dead link.")]
        public void People_Search_FindAPerson_ViewIndividual_AddAssignment()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View an individual
            test.Portal.People_ViewIndividual("Laura Hause");

            // Click the assignments link
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/people/Individual/Assignment.aspx?indName=')]");

            // Click the link to add a staffing assignment
            test.Selenium.ClickAndWaitForPageToLoad("link=Add an assignment");
            Assert.AreEqual("Fellowship One :: Add New Assignment", test.Selenium.GetTitle());
            Assert.AreEqual("Laura Hause", test.Selenium.GetText(string.Format("//div[@id='new_assignment_add_individual']/form/div/table/tbody/tr/td/div/strong[text()='{0}']", "Laura Hause")));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies functionality when logged in with a user lacking the people edit right.")]
        public void People_Search_ViewIndividualNoPeopleEdit()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("PR_PeopleEdit", "Pa$$w0rd");

            // View an individual
            test.Portal.People_ViewIndividual(test.Portal.PortalUser);

            // Verify the edit links for the individual is not present
            test.Selenium.VerifyElementNotPresent(Portal.People.SearchConstants.Edit_individual);
            test.Selenium.VerifyElementNotPresent("link=Edit individual");

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies functionality when logged in with a user lacking the people contact right views an individual.")]
        public void People_Search_FindAPerson_ViewIndividual_NoPeopleContact()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("PR_PeopleContact", "Pa$$w0rd");

            // View an individual
            test.Portal.People_ViewIndividual(test.Portal.PortalUser);

            // Verify the 'Enter a contact form' gear option is not present
            test.Selenium.VerifyElementNotPresent(Portal.People.SearchConstants.Enter_a_contact_form);

            // Verify the 'add one' link is not present, contact item count is at zero
            Assert.IsFalse(test.Selenium.IsVisible("//a[@onclick='QuickContact.ToggleAddNew();return false;']"));
            Assert.AreEqual("0", test.Selenium.GetText("//div[@id='contact_item_count']/span"));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user cannot navigate directly to the add contact form page w/out the people contact right enabled.")]
        public void People_Search_FindAPerson_ViewIndividual_NoPeopleContact_NavigateDirectly()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("PR_PeopleContact", "Pa$$w0rd");

            // Attempt to open the page directly
            test.GeneralMethods.OpenURLExpecting403(string.Format("/people/ContactForm/Select.aspx?HouseholdID={0}&IndividualID={1}&Source=INDIVIDUALINDEXPROXY&AllowRedirect=True", _hsdID, _indID));
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Selects 'Cancel' after clicking 'Save and Add Another' when editing an address.")]
        public void People_Search_FindAPerson_ViewIndividual_EditAddress_CancelAfterSaveAndAddAnother()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View an individual
            test.Portal.People_ViewIndividual("Matthew Sneeden");

            // Edit an address
            test.Selenium.ClickAndWaitForPageToLoad(Portal.People.SearchConstants.Edit_address);
            test.Selenium.Select("address_type", "Vacation");
            test.Selenium.ClickAndWaitForPageToLoad("SaveAndAddAnother");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Cancel);

            // Verify page displayed to the user
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.Search_IndividualDetail));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.Search_IndividualDetail);

            // Reset the address data
            test.Selenium.ClickAndWaitForPageToLoad("link=View the household");
            test.Selenium.ClickAndWaitForPageToLoad(Portal.People.SearchConstants.Edit_address);
            test.Selenium.Select("address_type", "Primary");
            test.Selenium.ClickAndWaitForPageToLoad("save_address");

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to add an email address that is missing an '@'.")]
        public void People_IND_Communications_CreateEmailMissingAtSymbol()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View a household
            test.Portal.People_ViewIndividual(test.Portal.PortalUser);

            // Edit the communications
            test.Selenium.ClickAndWaitForPageToLoad(Portal.People.SearchConstants.Edit_communications);

            // Attempt to save an improperly formatted email address
            // Goofy business rules on the contextual email input
            if (test.Selenium.IsElementPresent("personal_email"))
            {
                test.Selenium.Type("personal_email", "asdf.com");
            }
            else
            {
                test.Selenium.Type("alternate_email", "asdf.com");
            }

            test.Selenium.ClickAndWaitForPageToLoad("btn");

            // Verify error(s) displayed to user
            test.Selenium.VerifyTextPresent(new string[] { TextConstants.ErrorHeadingSingular, "Please verify all e-mail address records are properly formatted." });

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to add an email address that is missing a '.'.")]
        public void People_IND_Communications_CreateEmailMissingPeriod()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View a household
            test.Portal.People_ViewIndividual("Matthew Sneeden");

            // Edit the communications
            test.Selenium.ClickAndWaitForPageToLoad(Portal.People.SearchConstants.Edit_communications);

            // Attempt to save an improperly formatted email address
            // Goofy business rules on the contextual email input
            if (test.Selenium.IsElementPresent("personal_email"))
            {
                test.Selenium.Type("personal_email", "asdf@asdf");
            }
            else
            {
                test.Selenium.Type("alternate_email", "asdf@asdf");
            }

            test.Selenium.ClickAndWaitForPageToLoad("btn");

            // Verify error(s) displayed to user
            test.Selenium.VerifyTextPresent(new string[] { TextConstants.ErrorHeadingSingular, "Please verify all e-mail address records are properly formatted." });

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        public void People_Search_FindAPerson_ViewIndividual_AddAnIndividual()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View a household
            test.Portal.People_ViewIndividual(test.Portal.PortalUser);

            // Select add an individual
            test.Selenium.ClickAndWaitForPageToLoad(Portal.People.SearchConstants.Add_an_individual);

            // Verify title, text
            test.Selenium.VerifyTitle(PeopleHeadingText.TitleFormat(PeopleHeadingText.Search_ViewIndividual_AddAnIndividual));
            test.Selenium.VerifyTextPresent(PeopleHeadingText.Search_ViewIndividual_AddAnIndividual);

            // Verify label
            Assert.AreEqual("Sub status", test.Selenium.GetText("//form[@id='aspnetForm']/div[3]/div[3]/table/tbody/tr[1]/th[2]"));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        public void People_Search_FindAPerson_ViewIndividual_AddARelationship()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View a household
            test.Portal.People_ViewIndividual(test.Portal.PortalUser);

            // Select add a relationship from the gear
            test.Selenium.Click(GeneralLinks.Options);
            test.Selenium.ClickAndWaitForPageToLoad("link=Add a relationship");

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Create New Relationship");
            test.Selenium.VerifyTextPresent("Create New Relationship");

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies that an unlinked portal user does not appear in the drop down list when adding a requirement")]
        public void People_Search_FindAPerson_ViewIndividual_AddRequirement()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Add requirement
            test.Portal.People_FindAPerson_Individual_Add_Requirement("FT Tester", "Background Check", "Approved", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString(), "Tester, FT", true);

            // Delete requirement
            test.Portal.People_FindAPerson_Individual_Delete_Requirement("FT Tester", "Background Check");

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies that an unlinked portal user does not appear in the drop down list when adding a requirement")]
        public void People_Search_FindAPerson_ViewIndividual_AddRequirement_UnlinkedUser()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.unlinkedtest", "FT4life!", "dc");

            // Add requirement
            test.Portal.People_FindAPerson_Individual_Add_Requirement("FT Tester", "Background Check", "Approved", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString(), "Unlinkedtester, FT", false);

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        public void People_Search_FindAPerson_ViewIndividual_SubmitAWebForm()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View a household
            test.Portal.People_ViewIndividual(test.Portal.PortalUser);

            // Select add a relationship from the gear
            test.Selenium.Click(GeneralLinks.Options);
            test.Selenium.ClickAndWaitForPageToLoad("link=Submit a web form");

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Add Submission");
            test.Selenium.VerifyTextPresent("Add Submission");

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        public void Add_Individual_PrefixFirstNameMiddleNameLastNameSuffixOccupationDescriptionFormerChurch()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View an individual
            test.Portal.People_ViewIndividual("Matthew Sneeden");

            // Edit the information
            test.Selenium.ClickAndWaitForPageToLoad("link=Add an individual");

            // Verify the max length of the prefix, first name, middle name, last night, suffix, goes by, former name, employer, occupation description, former church, bar code, tag comment fields
            Assert.AreEqual("10", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtPrefix@maxlength"));
            Assert.AreEqual("30", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFirstName@maxlength"));
            Assert.AreEqual("30", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtMiddleName@maxlength"));
            Assert.AreEqual("30", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtLastName@maxlength"));
            Assert.AreEqual("5", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtSuffix@maxlength"));

            Assert.AreEqual("30", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtGoesByName@maxlength"));
            Assert.AreEqual("30", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFormerName@maxlength"));
            Assert.AreEqual("30", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtEmployer@maxlength"));
            Assert.AreEqual("100", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtOccupationDesc@maxlength"));
            Assert.AreEqual("50", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFormerChurch@maxlength"));
            Assert.AreEqual("50", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtBarCode@maxlength"));
            Assert.AreEqual("50", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtTagComment@maxlength"));

            // Attempt to populate fields past their maxlength
            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtPrefix", "12345678901");
            Assert.AreEqual("1234567890", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtPrefix"));

            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFirstName", "1234567890123456789012345678901");
            Assert.AreEqual("123456789012345678901234567890", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFirstName"));

            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtMiddleName", "1234567890123456789012345678901");
            Assert.AreEqual("123456789012345678901234567890", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtMiddleName"));

            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtLastName", "1234567890123456789012345678901");
            Assert.AreEqual("123456789012345678901234567890", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtLastName"));

            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtSuffix", "123456");
            Assert.AreEqual("12345", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtSuffix"));

            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtGoesByName", "1234567890123456789012345678901");
            Assert.AreEqual("123456789012345678901234567890", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtGoesByName"));

            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFormerName", "1234567890123456789012345678901");
            Assert.AreEqual("123456789012345678901234567890", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFormerName"));

            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtEmployer", "1234567890123456789012345678901");
            Assert.AreEqual("123456789012345678901234567890", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtEmployer"));

            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtOccupationDesc", "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901");
            Assert.AreEqual("1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtOccupationDesc"));

            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFormerChurch", "123456789012345678901234567890123456789012345678901");
            Assert.AreEqual("12345678901234567890123456789012345678901234567890", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFormerChurch"));

            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtBarCode", "123456789012345678901234567890123456789012345678901");
            Assert.AreEqual("12345678901234567890123456789012345678901234567890", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtBarCode"));

            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtTagComment", "123456789012345678901234567890123456789012345678901");
            Assert.AreEqual("12345678901234567890123456789012345678901234567890", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtTagComment"));

            // Logout of Portal
            test.Portal.Logout();
        }

        #region Widgets

        #region Involvement
        #endregion Involvement

        #region Groups
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a group admin can see a group on the individual detail page.")]
        public void People_Search_FindAPerson_ViewIndividual_Groups_Group_Admin_Can_See_Group()
        {

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "QAEUNLX0C2");

            // View an individual
            test.Portal.People_ViewIndividual("Group Leader");

            // Verify the group shows up
            test.Selenium.VerifyElementPresent("//table[@class='grid gutter_bottom_none']");
            var row = test.GeneralMethods.GetTableRowNumber(TableIds.People_GroupsWidget, _groupName, "Group belongs to… these groups", "contains") + 1;
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.People_GroupsWidget, _groupName, "Group belongs to… these groups", "contains"), "Group was not present in the groups widget!");

            // The group should be linked because you have explicit rights
            test.Selenium.VerifyElementPresent(string.Format(TableIds.People_GroupsWidget + "/tbody/tr[{0}]/td[2]/a", row));

            // Logout
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a group manager can see a group on the individual detail page.")]
        public void People_Search_FindAPerson_ViewIndividual_Groups_Group_Manager_Can_See_Group()
        {

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupmanager", "BM.Admin09", "QAEUNLX0C2");

            // View an individual
            test.Portal.People_ViewIndividual("Group Leader");

            // Verify the group shows up
            test.Selenium.VerifyElementPresent("//table[@class='grid gutter_bottom_none']");
            var row = test.GeneralMethods.GetTableRowNumber(TableIds.People_GroupsWidget, _groupName, "Group belongs to… these groups", "contains") + 1;
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.People_GroupsWidget, _groupName, "Group belongs to… these groups", "contains"), "Group was not present in the groups widget!");

            // The group should be linked because you have explicit rights
            test.Selenium.VerifyElementPresent(string.Format(TableIds.People_GroupsWidget + "/tbody/tr[{0}]/td[2]/a", row));

            // Logout
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a group viewer can see a group on the individual detail page.")]
        public void People_Search_FindAPerson_ViewIndividual_Groups_Group_Viewer_Can_See_Group()
        {

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupviewer", "BM.Admin09", "QAEUNLX0C2");

            // View an individual
            test.Portal.People_ViewIndividual("Group Leader");

            // Verify the group shows up
            test.Selenium.VerifyElementPresent("//table[@class='grid gutter_bottom_none']");
            var row = test.GeneralMethods.GetTableRowNumber(TableIds.People_GroupsWidget, _groupName, "Group belongs to… these groups", "contains") + 1;
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.People_GroupsWidget, _groupName, "Group belongs to… these groups", "contains"), "Group was not present in the groups widget!");

            // The group should be linked because you have explicit rights
            test.Selenium.VerifyElementPresent(string.Format(TableIds.People_GroupsWidget + "/tbody/tr[{0}]/td[2]/a", row));

            // Logout
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a span of care owner can see a group on the individual detail page.")]
        public void People_Search_FindAPerson_ViewIndividual_Groups_Owner_Can_See_Group()
        {

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("socowner", "BM.Admin09", "QAEUNLX0C2");

            // View an individual
            test.Portal.People_ViewIndividual("Group Leader");

            // Verify the group shows up
            test.Selenium.VerifyElementPresent("//table[@class='grid gutter_bottom_none']");
            var row = test.GeneralMethods.GetTableRowNumber(TableIds.People_GroupsWidget, _groupName, "Group belongs to… these groups", "contains") + 1;
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.People_GroupsWidget, _groupName, "Group belongs to… these groups", "contains"), "Group was not present in the groups widget!");

            // The group should be linked because you have implicit rights
            test.Selenium.VerifyElementPresent(string.Format(TableIds.People_GroupsWidget + "/tbody/tr[{0}]/td[2]/a", row));

            // Logout
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a group type admin can see a group on the individual detail page but also verifies the group is not linked.")]
        public void People_Search_FindAPerson_ViewIndividual_Groups_GroupTypeAdmin_Can_See_Group()
        {

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // View an individual
            test.Portal.People_ViewIndividual("Group Leader");

            // Verify the group shows up
            test.Selenium.VerifyElementPresent("//table[@class='grid gutter_bottom_none']");
            var row = test.GeneralMethods.GetTableRowNumber(TableIds.People_GroupsWidget, _groupName, "Group belongs to… these groups", "contains") + 1;
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.People_GroupsWidget, _groupName, "Group belongs to… these groups", "contains"), "Group was not present in the groups widget!");

            // The group should not be linked because you do not have explicit rights
            test.Selenium.VerifyElementNotPresent(string.Format(TableIds.People_GroupsWidget + "/tbody/tr[{0}]/td[2]/a", row));

            // Logout
            test.Portal.Logout();

        }

        #endregion Groups

        #region Notes
        #endregion Notes

        #region Contact Items
        #endregion Contact Items

        #region Attributes
        #endregion Attributes

        #region Requiremens
        #endregion Requirements

        #endregion Widgets

        #endregion View Individual

        #region Edit Individual
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        public void People_Search_FindAPerson_EditIndividual_StatusDateBirthDate()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View an individual
            test.Portal.People_ViewIndividual(test.Portal.PortalUser);

            // Edit the information
            test.Selenium.ClickAndWaitForPageToLoad(Portal.People.SearchConstants.Edit_individual);

            // Clear the value (if one exists in the status date field)
            test.Selenium.GetEval("selenium.browserbot.getCurrentWindow().document.getElementById('_status_control_').value = \"\"");
            Assert.AreEqual("", test.Selenium.GetValue("_status_control_"));

            // Press the "t/T" key to populate the field with the current date
            test.Selenium.KeyDown("_status_control_", "\\84");

            // Verify today's date has been entered into the field
            string currentDateFormatted = string.Format("{0:MM/dd/yyyy}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")));
            Assert.AreEqual(currentDateFormatted, test.Selenium.GetValue("_status_control_"));


            // Clear the value (if one exists in the status date field)
            test.Selenium.GetEval("selenium.browserbot.getCurrentWindow().document.getElementById('_dob_control_').value = \"\"");
            Assert.AreEqual("", test.Selenium.GetValue("_dob_control_"));

            // Press the "t/T" key to populate the field with the current date
            test.Selenium.KeyDown("_dob_control_", "\\84");

            // Verify today's date has been entered into the field
            Assert.AreEqual(currentDateFormatted, test.Selenium.GetValue("_dob_control_"));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the fields displayed when the user selects 'More fields' when editing an individual.")]
        public void People_Search_FindAPerson_EditIndividual_MoreFields()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View an individual
            test.Portal.People_ViewIndividual("Matthew Sneeden");

            // Edit an individual
            test.Selenium.ClickAndWaitForPageToLoad(Portal.People.SearchConstants.Edit_individual);

            // Verify fields are not visible
            test.Selenium.VerifyIsNotVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtTitle");
            test.Selenium.VerifyIsNotVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtGoesByName");
            test.Selenium.VerifyIsNotVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFormerName");

            test.Selenium.VerifyIsNotVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtEmployer");
            test.Selenium.VerifyIsNotVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_ddlOccupation");
            test.Selenium.VerifyIsNotVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtOccupationDesc");

            test.Selenium.VerifyIsNotVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_ddlDenomination");
            test.Selenium.VerifyIsNotVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFormerChurch");
            test.Selenium.VerifyIsNotVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_ddlSchool");

            test.Selenium.VerifyIsNotVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtBarCode");
            test.Selenium.VerifyIsNotVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtTagComment");
            test.Selenium.VerifyIsNotVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtMemberEnvNo");

            test.Selenium.VerifyIsNotVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_chkCanAddIndividual");
            test.Selenium.VerifyIsNotVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_chkDoNotThank");

            // Click to view more fields
            test.Selenium.Click("link=More fields");

            // Verify fields are visible
            test.Selenium.VerifyIsVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtTitle");
            test.Selenium.VerifyIsVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtGoesByName");
            test.Selenium.VerifyIsVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFormerName");

            test.Selenium.VerifyIsVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtEmployer");
            test.Selenium.VerifyIsVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_ddlOccupation");
            test.Selenium.VerifyIsVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtOccupationDesc");

            test.Selenium.VerifyIsVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_ddlDenomination");
            test.Selenium.VerifyIsVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFormerChurch");
            test.Selenium.VerifyIsVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_ddlSchool");

            test.Selenium.VerifyIsVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtBarCode");
            test.Selenium.VerifyIsVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtTagComment");
            test.Selenium.VerifyIsVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtMemberEnvNo");

            test.Selenium.VerifyIsVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_chkCanAddIndividual");
            test.Selenium.VerifyIsVisible("ctl00_ctl00_MainContent_content_ctlIndividualFull_chkDoNotThank");

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("")]
        public void People_Search_FindAPerson_EditIndividual_CommentExceeding200Char()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View and edit an individual
            test.Portal.People_ViewIndividual(test.Portal.PortalUser);
            test.Selenium.ClickAndWaitForPageToLoad(Portal.People.SearchConstants.Edit_individual);

            // Attempt to enter a comment exceeding 200 characters
            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtStatusComment", "123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.Save);

            // Verify error message
            test.Selenium.VerifyTextPresent(new string[] { TextConstants.ErrorHeadingSingular, "Individuals status comment field can't exceed 200 characters." });

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies fields present when editing an individual cannot exceed their designation maximum lengths.")]
        public void People_Search_FindAPerson_EditIndividual_PrefixFirstNameMiddleNameLastNameSuffixOccupationDescriptionFormerChurch()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View and edit an individual
            test.Portal.People_ViewIndividual(test.Portal.PortalUser);
            test.Selenium.ClickAndWaitForPageToLoad(Portal.People.SearchConstants.Edit_individual);

            // Verify the max length of the prefix, first name, middle name, last night, suffix, goes by, former name, employer, occupation description, former church, bar code, tag comment fields
            Assert.AreEqual("10", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtPrefix@maxlength"));
            Assert.AreEqual("30", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFirstName@maxlength"));
            Assert.AreEqual("30", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtMiddleName@maxlength"));
            Assert.AreEqual("30", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtLastName@maxlength"));
            Assert.AreEqual("5", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtSuffix@maxlength"));

            Assert.AreEqual("30", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtGoesByName@maxlength"));
            Assert.AreEqual("30", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFormerName@maxlength"));
            Assert.AreEqual("30", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtEmployer@maxlength"));
            Assert.AreEqual("100", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtOccupationDesc@maxlength"));
            Assert.AreEqual("50", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFormerChurch@maxlength"));
            Assert.AreEqual("50", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtBarCode@maxlength"));
            Assert.AreEqual("50", test.Selenium.GetAttribute("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtTagComment@maxlength"));

            // Attempt to populate fields past their maxlength
            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtPrefix", "12345678901");
            Assert.AreEqual("1234567890", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtPrefix"));

            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFirstName", "1234567890123456789012345678901");
            Assert.AreEqual("123456789012345678901234567890", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFirstName"));

            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtMiddleName", "1234567890123456789012345678901");
            Assert.AreEqual("123456789012345678901234567890", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtMiddleName"));

            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtLastName", "1234567890123456789012345678901");
            Assert.AreEqual("123456789012345678901234567890", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtLastName"));

            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtSuffix", "123456");
            Assert.AreEqual("12345", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtSuffix"));

            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtGoesByName", "1234567890123456789012345678901");
            Assert.AreEqual("123456789012345678901234567890", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtGoesByName"));

            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFormerName", "1234567890123456789012345678901");
            Assert.AreEqual("123456789012345678901234567890", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFormerName"));

            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtEmployer", "1234567890123456789012345678901");
            Assert.AreEqual("123456789012345678901234567890", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtEmployer"));

            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtOccupationDesc", "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901");
            Assert.AreEqual("1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtOccupationDesc"));

            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFormerChurch", "123456789012345678901234567890123456789012345678901");
            Assert.AreEqual("12345678901234567890123456789012345678901234567890", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFormerChurch"));

            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtBarCode", "123456789012345678901234567890123456789012345678901");
            Assert.AreEqual("12345678901234567890123456789012345678901234567890", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtBarCode"));

            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtTagComment", "123456789012345678901234567890123456789012345678901");
            Assert.AreEqual("12345678901234567890123456789012345678901234567890", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtTagComment"));

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you can add and remove a mobile number for an individual.")]
        public void People_Search_FindAPerson_EditIndividual_Communications_Mobile_Phone()
        {
            // Variables
            var individualName = "Auto Tester";
            var mobilePhone = "214-555-5553";

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Add a mobile phone for an individual
            test.Portal.People_FindAPerson_Individual_Edit_Communications_Phone(individualName, GeneralEnumerations.CommunicationTypes.Mobile, mobilePhone, null, false);

            // Verify the mobile phone exists
            test.Selenium.VerifyTextPresent(mobilePhone);

            // Revert the change
            test.Portal.People_FindAPerson_Individual_Edit_Communications_Phone(individualName, GeneralEnumerations.CommunicationTypes.Mobile, string.Empty, null, false);

            // Verify the mobile phone does not exist
            Assert.IsFalse(test.Selenium.IsTextPresent(mobilePhone), "Deleted mobile phone was still present.");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you can add and remove a home number for an individual.")]
        public void People_Search_FindAPerson_EditIndividual_Communications_Home_Phone()
        {
            // Variables
            var individualName = "Auto Tester";
            var homePhone = "214-555-5523";

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Add a home phone for an individual
            test.Portal.People_FindAPerson_Individual_Edit_Communications_Phone(individualName, GeneralEnumerations.CommunicationTypes.Home, homePhone, null, false);

            // Verify the home phone exists
            test.Selenium.VerifyTextPresent(homePhone);

            // View the household
            test.Portal.People_ViewHousehold(individualName);

            // Verify the home phone exists
            test.Selenium.VerifyTextPresent(homePhone);

            // Revert the change
            test.Portal.People_FindAPerson_Individual_Edit_Communications_Phone(individualName, GeneralEnumerations.CommunicationTypes.Home, string.Empty, null, false);

            // Verify the home phone does not exist
            Assert.IsFalse(test.Selenium.IsTextPresent(homePhone), "Deleted home phone was still present.");

            // View the household
            test.Portal.People_ViewHousehold(individualName);

            // Verify the home phone does not exist
            Assert.IsFalse(test.Selenium.IsTextPresent(homePhone), "Deleted home phone was still present.");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you can add and remove a work number for an individual.")]
        public void People_Search_FindAPerson_EditIndividual_Communications_Work_Phone()
        {
            // Variables
            var individualName = "Auto Tester";
            var workPhone = "214-555-6523";

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Add a work phone for an individual
            test.Portal.People_FindAPerson_Individual_Edit_Communications_Phone(individualName, GeneralEnumerations.CommunicationTypes.Work, workPhone, null, false);

            // Verify the work phone exists
            test.Selenium.VerifyTextPresent(workPhone);

            // Revert the change
            test.Portal.People_FindAPerson_Individual_Edit_Communications_Phone(individualName, GeneralEnumerations.CommunicationTypes.Work, string.Empty, null, false);

            // Verify the work phone does not exist
            Assert.IsFalse(test.Selenium.IsTextPresent(workPhone), "Deleted work phone was still present.");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("FO-4887 P2 - When People record is modified, the Create Portal User changes to the Portal User that Modified the record.")]
        public void People_Search_FindAPerson_EditIndividual_WorkPhone_CheckCreatedByUserID()
        {
            // Variables
            var individualName = "Graceteacher4 Zhang";
            var workPhone = "214-555-6523";
            int churchId = 15;

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            int individualId = test.SQL.People_Individuals_FetchID(churchId, individualName);
            string updateData = test.SQL.People_Individual_FetchColumnValue(churchId, individualId, "Last_update");
            updateData += test.SQL.People_Individual_FetchColumnValue(churchId, individualId, "last_updated_by_login");
            updateData += test.SQL.People_Individual_FetchColumnValue(churchId, individualId, "last_updated_by_user_id");
            string createData = test.SQL.People_Individual_FetchColumnValue(churchId, individualId, "created_date");
            createData += test.SQL.People_Individual_FetchColumnValue(churchId, individualId, "created_by_login");
            createData += test.SQL.People_Individual_FetchColumnValue(churchId, individualId, "created_by_user_id");

            TestLog.WriteLine("Individual create data is: " + createData);
            TestLog.WriteLine("Individual update data is: " + updateData);

            string createDataNew;
            string updateDataNew;

            test.Portal.Login("madykou", "Active@123", "DC");

            // Add a work phone for an individual
            test.Portal.People_FindAPerson_Individual_Edit_Communications_Phone(individualName, GeneralEnumerations.CommunicationTypes.Work, workPhone, null, false);

            // Verify the work phone exists
            test.Selenium.VerifyTextPresent(workPhone);

            updateDataNew = test.SQL.People_Individual_FetchColumnValue(churchId, individualId, "Last_update");
            updateDataNew += test.SQL.People_Individual_FetchColumnValue(churchId, individualId, "last_updated_by_login");
            updateDataNew += test.SQL.People_Individual_FetchColumnValue(churchId, individualId, "last_updated_by_user_id");
            createDataNew = test.SQL.People_Individual_FetchColumnValue(churchId, individualId, "created_date");
            createDataNew += test.SQL.People_Individual_FetchColumnValue(churchId, individualId, "created_by_login");
            createDataNew += test.SQL.People_Individual_FetchColumnValue(churchId, individualId, "created_by_user_id");

            TestLog.WriteLine("Individual afterChange create data is: " + createDataNew);
            TestLog.WriteLine("Individual afterChange update data is: " + updateDataNew);

            Assert.AreEqual(createData, createDataNew, "Create data should not change after individual is updated");
            Assert.AreNotEqual(updateData, updateDataNew, "Update data should change after individual is updated");

            // Logout of portal
            test.Portal.Logout();

            test.Portal.Login("ft.householdedit", "FT4life!", "dc");

            // Revert the change
            test.Portal.People_FindAPerson_Individual_Edit_Communications_Phone(individualName, GeneralEnumerations.CommunicationTypes.Work, string.Empty, null, false);

            updateDataNew = test.SQL.People_Individual_FetchColumnValue(churchId, individualId, "Last_update");
            updateDataNew += test.SQL.People_Individual_FetchColumnValue(churchId, individualId, "last_updated_by_login");
            updateDataNew += test.SQL.People_Individual_FetchColumnValue(churchId, individualId, "last_updated_by_user_id");
            createDataNew = test.SQL.People_Individual_FetchColumnValue(churchId, individualId, "created_date");
            createDataNew += test.SQL.People_Individual_FetchColumnValue(churchId, individualId, "created_by_login");
            createDataNew += test.SQL.People_Individual_FetchColumnValue(churchId, individualId, "created_by_user_id");

            TestLog.WriteLine("Individual afterChange create data is: " + createDataNew);
            TestLog.WriteLine("Individual afterChange update data is: " + updateDataNew);

            Assert.AreEqual(createData, createDataNew, "Create data should not change after individual is updated");
            Assert.AreNotEqual(updateData, updateDataNew, "Update data should change after individual is updated");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you can add and remove an emergency number for an individual.")]
        public void People_Search_FindAPerson_EditIndividual_Communications_Emergency_Phone()
        {
            // Variables
            var individualName = "Auto Tester";
            var emergencyPhone = "214-555-9523";

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Add a emergency phone for an individual
            test.Portal.People_FindAPerson_Individual_Edit_Communications_Phone(individualName, GeneralEnumerations.CommunicationTypes.Emergency, emergencyPhone, null, false);

            // Verify the emergency phone exists
            test.Selenium.VerifyTextPresent(emergencyPhone);

            // Revert the change
            test.Portal.People_FindAPerson_Individual_Edit_Communications_Phone(individualName, GeneralEnumerations.CommunicationTypes.Emergency, string.Empty, null, false);

            // Verify the emergency phone does not exist
            Assert.IsFalse(test.Selenium.IsTextPresent(emergencyPhone), "Deleted work phone was still present.");

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you can add and remove a personal email for an individual.")]
        public void People_Search_FindAPerson_EditIndividual_Communications_Personal_Email()
        {
            // Variables
            var individualName = "Auto Tester";
            var personalEmail = "ft.autotester@gmail.com";

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Add a personal email for an individual
            test.Portal.People_FindAPerson_Individual_Edit_Communications_Email(individualName, GeneralEnumerations.CommunicationTypes.Personal, personalEmail, null, false);

            // Verify the personal email exists
            test.Selenium.VerifyTextPresent(personalEmail);

            // Revert the change
            test.Portal.People_FindAPerson_Individual_Edit_Communications_Email(individualName, GeneralEnumerations.CommunicationTypes.Personal, string.Empty, null, false);

            // Verify the personal email does not exist
            Assert.IsFalse(test.Selenium.IsTextPresent(personalEmail), "Deleted personal email was still present.");

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you can add and remove a homw email for an individual.")]
        public void People_Search_FindAPerson_EditIndividual_Communications_Home_Email()
        {
            // Variables
            var individualName = "Auto Tester";
            var homeEmail = "ft.autotesterhome@gmail.com";

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Add a home email for an individual
            test.Portal.People_FindAPerson_Individual_Edit_Communications_Email(individualName, GeneralEnumerations.CommunicationTypes.Home, homeEmail, null, false);

            // Verify the home email exists
            test.Selenium.VerifyTextPresent(homeEmail);

            // View the household
            test.Portal.People_ViewHousehold(individualName);

            // Verify the home email exists
            test.Selenium.VerifyTextPresent(homeEmail);

            // Revert the change
            test.Portal.People_FindAPerson_Individual_Edit_Communications_Email(individualName, GeneralEnumerations.CommunicationTypes.Home, string.Empty, null, false);

            // Verify the home email does not exist
            Assert.IsFalse(test.Selenium.IsTextPresent(homeEmail), "Deleted home email was still present.");

            // View the household
            test.Portal.People_ViewHousehold(individualName);

            // Verify the home email does not exist
            Assert.IsFalse(test.Selenium.IsTextPresent(homeEmail), "Deleted home email was still present.");

            // Logout
            test.Portal.Logout();
        }

        #region Invalid Email
        [Test, RepeatOnFailure(Order = 1)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that if an infellowship email is no longe valid, the Edit Communications page will display a notification message to the user about the invalid email.")]
        public void People_Search_FindAPerson_EditIndividual_Communication_Invalid_InFellowship_Email_Notification_Present()
        {
            // Mark an Email as invalid
            base.SQL.People_UpdateCommunicationValidationStatus(254, "Bryan Mikaelian", "bmikaelian@fellowshiptech.com", 0);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // View the edit communications page
            test.Portal.People_ViewIndividual("Bryan Mikaelian");
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, 'communications/edit')]");

            // Verify the row for the email is red and messaging is present for the invalid email
            test.Selenium.VerifyElementPresent("//tr[@class='error_row']");
            test.Selenium.VerifyTextPresent("There were errors with this email address.");

            // Logout of portal
            test.Portal.Logout();

            // Revert the change
            base.SQL.People_UpdateCommunicationValidationStatus(254, "Bryan Mikaelian", "bmikaelian@fellowshiptech.com", null);
        }

        [Test, RepeatOnFailure(Order = 2)]
        [Author("Bryan Mikaelian")]
        [Description("Makes an invalid infellowship email valid by clicking accept on the modal popup that appears for an invalid email.")]
        public void People_Search_FindAPerson_EditIndividual_Communication_Invalid_InFellowship_Email_Make_Valid_Modal()
        {
            // Mark an Email as invalid
            base.SQL.People_UpdateCommunicationValidationStatus(254, "Bryan Mikaelian", "bmikaelian@fellowshiptech.com", 0);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Make the infellowship email active for an individual
            test.Portal.People_FindAPerson_Individual_EditCommuncations_Make_Email_Valid("Bryan Mikaelian", GeneralEnumerations.CommunicationTypes.InFellowship, "bmikaelian@fellowshiptech.com", 254);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure(Order = 3)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that if an alternate email is no longer valid, the Edit Communications page will display a notification message to the user about the invalid email.")]
        public void People_Search_FindAPerson_EditIndividual_Communication_Invalid_Alternate_Email_Notification_Present()
        {
            // Mark an Email as invalid
            base.SQL.People_UpdateCommunicationValidationStatus(254, "Bryan Mikaelian", "bryan.mikaelian@gmail.com", 0);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // View the edit communications page
            test.Portal.People_ViewIndividual("Bryan Mikaelian");
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, 'communications/edit')]");

            // Verify the row for the email is red and messaging is present for the invalid email
            test.Selenium.VerifyElementPresent("//tr[contains(@class, 'error_row')]");
            test.Selenium.VerifyTextPresent("There were errors with this email address.");

            // Logout of portal
            test.Portal.Logout();

            // Revert the change
            base.SQL.People_UpdateCommunicationValidationStatus(254, "Bryan Mikaelian", "bryan.mikaelian@gmail.com", null);
        }

        [Test, RepeatOnFailure(Order = 4)]
        [Author("Bryan Mikaelian")]
        [Description("Makes an invalid alternate email valid by clicking accept on the modal popup that appears for an invalid email.")]
        public void People_Search_FindAPerson_EditIndividual_Communication_Invalid_Alternate_Email_Make_Valid_Modal()
        {
            // Mark an Email as invalid
            base.SQL.People_UpdateCommunicationValidationStatus(254, "Bryan Mikaelian", "bryan.mikaelian@gmail.com", 0);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Make the alternate email active
            test.Portal.People_FindAPerson_Individual_EditCommuncations_Make_Email_Valid("Bryan Mikaelian", GeneralEnumerations.CommunicationTypes.Personal, "bryan.mikaelian@gmail.com", 254);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure(Order = 5)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that if a home email is no longe valid, the Edit Communications page will display a notification message to the user about the invalid email.")]
        public void People_Search_FindAPerson_EditIndividual_Communication_Invalid_Home_Email_Notification_Present()
        {
            // Mark an Email as invalid
            base.SQL.People_UpdateCommunicationValidationStatus(254, "Bryan Mikaelian", "bryan.mikaelian@activenetwork.com", 0);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // View the edit communications page
            test.Portal.People_ViewIndividual("Bryan Mikaelian");
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, 'communications/edit')]");

            // Verify the row for the email is red and messaging is present for the invalid email
            test.Selenium.VerifyElementPresent("//tr[@class='error_row']");
            test.Selenium.VerifyTextPresent("There were errors with this email address.");

            // Logout of portal
            test.Portal.Logout();

            // Revert the change
            base.SQL.People_UpdateCommunicationValidationStatus(254, "Bryan Mikaelian", "bryan.mikaelian@activenetwork.com", null);
        }

        [Test, RepeatOnFailure(Order = 6)]
        [Author("Bryan Mikaelian")]
        [Description("Makes an invalid home email valid by clicking accept on the modal popup that appears for an invalid email.")]
        public void People_Search_FindAPerson_EditIndividual_Communication_Invalid_Home_Email_Make_Valid_Modal()
        {
            // Mark an Email as invalid
            base.SQL.People_UpdateCommunicationValidationStatus(254, "Bryan Mikaelian", "bryan.mikaelian@activenetwork.com", 0);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Make the home email active
            test.Portal.People_FindAPerson_Individual_EditCommuncations_Make_Email_Valid("Bryan Mikaelian", GeneralEnumerations.CommunicationTypes.Home, "bryan.mikaelian@activenetwork.com", 254);

            // Logout of portal
            test.Portal.Logout();
        }

        #endregion Invalid Email

        #endregion Edit Individual

        #region Edit Privacy Settings
        #region Address
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Updates the Address Privacy Setting so that only church staff can view their address.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdateAddress_ChurchStaff()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for Address
            test.Portal.People_FindAPerson_PrivacySettings_Update_AddressSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.ChurchStaff);

            // Logout of Portal
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the Address Privacy Setting so that church staff and group leaders can view their address.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdateAddress_GroupLeaders()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for Address
            test.Portal.People_FindAPerson_PrivacySettings_Update_AddressSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.Leaders);

            // Logout of Portal
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the Address Privacy Setting so that church staff, group leaders, and group members can view their address.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdateAddress_GroupMembers()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for Address
            test.Portal.People_FindAPerson_PrivacySettings_Update_AddressSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.Members);

            // Logout of Portal
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the Address Privacy Setting so that everyone can view their address.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdateAddress_Everyone()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for Address
            test.Portal.People_FindAPerson_PrivacySettings_Update_AddressSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.Everyone);

            // Logout of Portal
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies the preview when changing the address privacy policy")]
        public void People_Search_FindAPerson_PrivacySettings_VerifyPreview_Address()
        {
            string fullAddress = "6363 N State Highway 161  Ste 200  Irving , TX 75038-2226";
            string cityState = "Irving , TX";
            
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            //Navigate to the Privacy settings page
            test.Portal.People_FindAPerson_PrivacySettings_View("FT PrivacySettings");

            //Edit Address privacy to Church Staff
            test.Portal.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Address, GeneralEnumerations.PrivacyLevels.ChurchStaff);

            // Church Staff preview
            Assert.AreEqual(fullAddress, test.Selenium.GetText("//tbody/tr/td[1]/div/p[2]").Replace("\n", ""));
            ///html/body/div[2]/div[2]/div/div[3]/div/div[2]/div[2]/form/table[2]/tbody/tr/td/div/p[2]
            Assert.AreNotEqual(cityState, test.Selenium.GetText("//tbody/tr/td[1]/div/p[2]"));

            // Leaders preview
            Assert.AreNotEqual(fullAddress, test.Selenium.GetText("//tbody/tr/td[2]/div/p[2]").Replace("\n", ""));
            Assert.AreEqual(cityState, test.Selenium.GetText("//tbody/tr/td[2]/div/p[2]"));

            // Members preview
            Assert.AreNotEqual(fullAddress, test.Selenium.GetText("//tbody/tr/td[3]/div/p[2]").Replace("\n", ""));
            Assert.AreEqual(cityState, test.Selenium.GetText("//tbody/tr/td[3]/div/p[2]"));

            // Everyone preview
            Assert.AreNotEqual(fullAddress, test.Selenium.GetText("//tbody/tr/td[4]/div/p[2]").Replace("\n", ""));
            Assert.AreEqual(cityState, test.Selenium.GetText("//tbody/tr/td[4]/div/p[2]"));

            // Leaders
            test.Portal.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Address, GeneralEnumerations.PrivacyLevels.Leaders);

            // Church Staff preview
            Assert.AreEqual(fullAddress, test.Selenium.GetText("//tbody/tr/td[1]/div/p[2]").Replace("\n", ""));
            Assert.AreNotEqual(cityState, test.Selenium.GetText("//tbody/tr/td[1]/div/p[2]"));

            // Leaders preview
            Assert.AreEqual(fullAddress, test.Selenium.GetText("//tbody/tr/td[2]/div/p[2]").Replace("\n", ""));
            Assert.AreNotEqual(cityState, test.Selenium.GetText("//tbody/tr/td[2]/div/p[2]"));

            // Members preview
            Assert.AreNotEqual(fullAddress, test.Selenium.GetText("//tbody/tr/td[3]/div/p[2]").Replace("\n", ""));
            Assert.AreEqual(cityState, test.Selenium.GetText("//tbody/tr/td[3]/div/p[2]"));

            // Everyone preview
            Assert.AreNotEqual(fullAddress, test.Selenium.GetText("//tbody/tr/td[4]/div/p[2]").Replace("\n", ""));
            Assert.AreEqual(cityState, test.Selenium.GetText("//tbody/tr/td[4]/div/p[2]"));

            // Members
            test.Portal.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Address, GeneralEnumerations.PrivacyLevels.Members);

            // Church Staff preview
            Assert.AreEqual(fullAddress, test.Selenium.GetText("//tbody/tr/td[1]/div/p[2]").Replace("\n", ""));
            Assert.AreNotEqual(cityState, test.Selenium.GetText("//tbody/tr/td[1]/div/p[2]"));

            // Leaders preview
            Assert.AreEqual(fullAddress, test.Selenium.GetText("//tbody/tr/td[2]/div/p[2]").Replace("\n", ""));
            Assert.AreNotEqual(cityState, test.Selenium.GetText("//tbody/tr/td[2]/div/p[2]"));

            // Members preview
            Assert.AreEqual(fullAddress, test.Selenium.GetText("//tbody/tr/td[3]/div/p[2]").Replace("\n", ""));
            Assert.AreNotEqual(cityState, test.Selenium.GetText("//tbody/tr/td[3]/div/p[2]"));

            // Everyone preview
            Assert.AreNotEqual(fullAddress, test.Selenium.GetText("//tbody/tr/td[4]/div/p[2]").Replace("\n", ""));
            Assert.AreEqual(cityState, test.Selenium.GetText("//tbody/tr/td[4]/div/p[2]"));

            // Everyone
            test.Portal.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Address, GeneralEnumerations.PrivacyLevels.Everyone);

            // Church Staff preview
            Assert.AreEqual(fullAddress, test.Selenium.GetText("//tbody/tr/td[1]/div/p[2]").Replace("\n", ""));
            Assert.AreNotEqual(cityState, test.Selenium.GetText("//tbody/tr/td[1]/div/p[2]"));

            // Leaders preview
            Assert.AreEqual(fullAddress, test.Selenium.GetText("//tbody/tr/td[2]/div/p[2]").Replace("\n", ""));
            Assert.AreNotEqual(cityState, test.Selenium.GetText("//tbody/tr/td[2]/div/p[2]"));

            // Members preview
            Assert.AreEqual(fullAddress, test.Selenium.GetText("//tbody/tr/td[3]/div/p[2]").Replace("\n", ""));
            Assert.AreNotEqual(cityState, test.Selenium.GetText("//tbody/tr/td[3]/div/p[2]"));

            // Everyone preview
            Assert.AreEqual(fullAddress, test.Selenium.GetText("//tbody/tr/td[4]/div/p[2]").Replace("\n", ""));
            Assert.AreNotEqual(cityState, test.Selenium.GetText("//tbody/tr/td[4]/div/p[2]"));

            // Logout
            test.Portal.Logout();


        }

        #endregion Address

        #region Birthdate
        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the Birthdate Privacy Setting so that only church staff can view their birthday.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdateBirthdate_ChurchStaff()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for Birthdate
            test.Portal.People_FindAPerson_PrivacySettings_Update_BirthdateSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.ChurchStaff);

            // Logout of Portal
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the Birthdate Privacy Setting so that church staff and group leaders can view their birthday.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdateBirthdate_GroupLeaders()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for Birthdate
            test.Portal.People_FindAPerson_PrivacySettings_Update_BirthdateSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.Leaders);

            // Logout of Portal
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the Birthdate Privacy Setting so that church staff, group leaders, and group members can view their birthday.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdateBirthdate_GroupMembers()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for Birthdate
            test.Portal.People_FindAPerson_PrivacySettings_Update_BirthdateSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.Members);

            // Logout of Portal
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the Birthdate Privacy Setting so that everyone can view their birthday.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdateBirthdate_Everyone()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for Birthdate
            test.Portal.People_FindAPerson_PrivacySettings_Update_BirthdateSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.Everyone);

            // Logout of Portal
            test.Portal.Logout();

        }
        #endregion Birthdate

        #region Email

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Updates the Email Privacy Setting so that church staff can view their email address.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdateEmail_ChurchStaff()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for Email
            test.Portal.People_FindAPerson_PrivacySettings_Update_EmailSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.ChurchStaff);

            // Logout of Portal
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the Email Privacy Setting so that church staff and group leaders can view their email address.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdateEmail_GroupLeaders()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for Email
            test.Portal.People_FindAPerson_PrivacySettings_Update_EmailSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.Leaders);

            // Logout of Portal
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the Email Privacy Setting so that church staff, group leaders, and group members can view their email address.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdateEmail_GroupMembers()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for Email
            test.Portal.People_FindAPerson_PrivacySettings_Update_EmailSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.Members);

            // Logout of Portal
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the Email Privacy Setting so that everyone can view their email address.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdateEmail_Everyone()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for Email
            test.Portal.People_FindAPerson_PrivacySettings_Update_EmailSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.Everyone);

            // Logout of Portal
            test.Portal.Logout();

        }
        #endregion Email

        #region Phone

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the Phone Privacy Setting so that church staff can view their Phone number.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdatePhone_ChurchStaff()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for Phone
            test.Portal.People_FindAPerson_PrivacySettings_Update_PhoneSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.ChurchStaff);

            // Logout of Portal
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the Phone Privacy Setting so that church staff and group leaders can view their Phone number.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdatePhone_GroupLeaders()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for Phone
            test.Portal.People_FindAPerson_PrivacySettings_Update_PhoneSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.Leaders);

            // Logout of Portal
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the Phone Privacy Setting so that church staff, group leaders, and group members can view their Phone number.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdatePhone_GroupMembers()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for Phone
            test.Portal.People_FindAPerson_PrivacySettings_Update_PhoneSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.Members);

            // Logout of Portal
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the Phone Privacy Setting so that everyone can view their Phone number.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdatePhone_Everyone()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for Phone
            test.Portal.People_FindAPerson_PrivacySettings_Update_PhoneSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.Everyone);

            // Logout of Portal
            test.Portal.Logout();

        }
        #endregion Phone

        #region Websites

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the Websites Privacy Settings so that church staff can view their websites.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdateWebsites_ChurchStaff()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for Websites
            test.Portal.People_FindAPerson_PrivacySettings_Update_WebsitesSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.ChurchStaff);

            // Logout of Portal
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the Websites Privacy Settings so that church staff and group leaders can view their websites.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdateWebsites_GroupLeaders()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for Websites
            test.Portal.People_FindAPerson_PrivacySettings_Update_WebsitesSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.Leaders);

            // Logout of Portal
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the Websites Privacy Settings so that church staff, group leaders, and group members can view their websites.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdateWebsites_GroupMembers()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for Websites
            test.Portal.People_FindAPerson_PrivacySettings_Update_WebsitesSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.Members);

            // Logout of Portal
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the Websites Privacy Settings so that everyone can view their websites.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdateWebsites_Everyone()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for Websites
            test.Portal.People_FindAPerson_PrivacySettings_Update_WebsitesSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.Everyone);

            // Logout of Portal
            test.Portal.Logout();

        }
        #endregion Websites

        #region Social networks

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the Social Networks Privacy Settings so that church staff can view their social networks.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdateSocialNetworks_ChurchStaff()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for social networks
            test.Portal.People_FindAPerson_PrivacySettings_Update_SocialNetworksSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.ChurchStaff);

            // Logout of Portal
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the Social Networks Privacy Settings so that church staff and group leaders can view their social networks.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdateSocialNetworks_GroupLeaders()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for social networks
            test.Portal.People_FindAPerson_PrivacySettings_Update_SocialNetworksSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.Leaders);

            // Logout of Portal
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the Social Networks Privacy Settings so that church staff, group leaders, and group members can view their social networks.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdateSocialNetworks_GroupMembers()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for social networks
            test.Portal.People_FindAPerson_PrivacySettings_Update_SocialNetworksSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.Members);

            // Logout of Portal
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the Social Networks Privacy Settings so that everyone can view their social networks.")]
        public void People_Search_FindAPerson_PrivacySettings_UpdateSocialNetworks_Everyone()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Update the privacy setting for social networks
            test.Portal.People_FindAPerson_PrivacySettings_Update_SocialNetworksSettings("FT PrivacySettings", GeneralEnumerations.PrivacyLevels.Everyone);

            // Logout of Portal
            test.Portal.Logout();

            // Login to Infellowship
            test.infellowship.Login("ft.privacysettings@gmail.com", "FT4life!", "dc");

            // View the Privacy Settings page
            test.infellowship.People_PrivacySettings_View();

            // Verify the change was saved
            test.Selenium.VerifyElementNotPresent("//table[@id='slider_table']/tbody/tr[6]/td/div/a[contains(@style='left: 100%')]");

            // Logout of Infellowship
            test.infellowship.Logout();

        }
        #endregion Social networks

        #endregion Edit Privacy Settings
    }

    [TestFixture]
    public class Portal_People_Attendance_History : FixtureBaseWebDriver
    {

        #region Involvement

        [Test, RepeatOnFailure, Timeout(120000)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Suchitra")]
        [Description("F1-3762 Groups Enh: Validate Attendance History in Involvemnet Section")]
        public void People_Attendance_History_Involvement_GroupAttendance()
        {

            //Initializing intial test values for variables
            string individualName = "FT Tester";
            string groupTypeName = "Automation Tests";
            string groupName = "Attendance Group";
            int churchId = 15;
            List<string> individualsInGroup = new List<string>();
            individualsInGroup.Add(individualName);
            individualsInGroup.Add("Test Name");

            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string startDate = now.AddMonths(-2).ToShortDateString();
            string startTime = now.ToShortTimeString();
            TestLog.WriteLine("Start Date Time: {0} {1}", startDate, startTime);

            //Clean up the last test run values
            base.SQL.Groups_Group_Delete(churchId, groupName);
            base.SQL.Groups_Group_DeleteAttendanceSummary(churchId, groupName);

            //Create a new Test Group
            base.SQL.Groups_Group_Create(churchId, groupTypeName, individualName, groupName, null, startDate);

            //Add Individuals to Group
            base.SQL.Groups_Group_AddLeaderOrMember(churchId, groupName, individualName, "Leader", startDate);
            base.SQL.Groups_Group_AddLeaderOrMember(churchId, groupName, "Test Name", "Member", startDate);

            //Create  Group Attendance  Record for past dates 
           // string attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(15, groupName, individualsInGroup, false, false, startDate, startTime);
            var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(churchId, groupName, individualsInGroup, false, false, startDate, startTime);


            //Attendance Date and Time
            TestLog.WriteLine("Attendance Date: {0}", attendanceDate);
            /*string[] splitDelim = new string[] { " at " };
            string[] splitArray = attendanceDate.Split(splitDelim, StringSplitOptions.None);
            TestLog.WriteLine("Attendance Date: {0}", splitArray[0].Trim());
            TestLog.WriteLine("Attendance Time: {0}", splitArray[1].Trim());

            string shortDate = Convert.ToDateTime(splitArray[0]).ToString("M/d/yyyy");
            string shortTime = Convert.ToDateTime(splitArray[1]).ToString("h:mm tt");

            TestLog.WriteLine("Short Attendance Date: {0}", shortDate);
            TestLog.WriteLine("Short Attendance Time: {0}", shortTime);
            */

            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            //Login to infellowship
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!");

            //Enter Attendance for Individuals in Group
            test.Infellowship.Groups_Attendance_Enter_All_Webdriver(groupName, attendanceDate, true);

            //Log out of Infellowship
            test.Infellowship.LogoutWebDriver();

            //Login to Portal 
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");
            //test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "QAEUNLX0C2");

            //View Individuals details in Portal
            test.Portal.People_ViewIndividual_WebDriver(individualName);

            //View Attendance History
            test.Portal._People_Verify_Involvement_Graph_GroupDetails(startDate, startTime, groupName, groupTypeName);

           //Logout
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure, Timeout(120000)]
        [Author("Suchitra")]
        [Description("F1-3762 Groups Enh: Retain Attendance History")]
        public void People_Attendance_History_Inolvement_RetainAttendance()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            //Initializing intial test values for variables
            string individualName = "Felix Gaytan";
            string groupTypeName = "Automation Tests";
            string groupName = "Retain Group";
            int churchId = 15;
            List<string> individualsInGroup = new List<string>();
            individualsInGroup.Add(individualName);
            individualsInGroup.Add("Test Name");
            individualsInGroup.Add("FT Tester");

            
            //string startDate = DateTime.Parse(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(-2).ToShortDateString() + " 08:30 PM").ToString();
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string startDate = now.AddMonths(-2).ToShortDateString();
            string startTime = now.ToShortTimeString();
            TestLog.WriteLine("Start Date Time: {0} {1}", startDate, startTime);

            //Clean up the last test run values
            base.SQL.Groups_Group_Delete(churchId, groupName);
            base.SQL.Groups_Group_DeleteAttendanceSummary(churchId, groupName);

            //Create a new Test Group
            base.SQL.Groups_Group_Create(churchId, groupTypeName, individualName, groupName, null, startDate);

            //Add Individuals to Group
            base.SQL.Groups_Group_AddLeaderOrMember(churchId, groupName, individualName, "Leader", startDate);
            base.SQL.Groups_Group_AddLeaderOrMember(churchId, groupName, "Test Name", "Member", startDate);
            base.SQL.Groups_Group_AddLeaderOrMember(churchId, groupName, "FT Tester", "Leader", startDate);

            //Create  Group Attendance  Record for past dates 
           // string attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(15, groupName, individualsInGroup, false, false, startDate, startTime);
            var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(churchId, groupName, individualsInGroup, false, false, startDate, startTime);

            //Attendance Date and Time
            TestLog.WriteLine("Attendance Date: {0}", attendanceDate);
            /*string[] splitDelim = new string[] { " at " };
            string[] splitArray = attendanceDate.Split(splitDelim, StringSplitOptions.None);
            TestLog.WriteLine("Attendance Date: {0}", splitArray[0].Trim());
            TestLog.WriteLine("Attendance Time: {0}", splitArray[1].Trim());

            string shortDate = Convert.ToDateTime(splitArray[0]).ToString("M/d/yyyy");
            string shortTime = Convert.ToDateTime(splitArray[1]).ToString("h:mm tt");

            TestLog.WriteLine("Short Attendance Date: {0}", shortDate);
            TestLog.WriteLine("Short Attendance Time: {0}", shortTime);
            */

            //Login to infellowship
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!");

            //Enter Attendance for Individuals in Group
            test.Infellowship.Groups_Attendance_Enter_All_Webdriver(groupName, attendanceDate, true);

            //Log out of Infellowship
            test.Infellowship.LogoutWebDriver();            

            //Login to Portal 
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");
            //test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "QAEUNLX0C2");

            //View Individuals details in Portal
            test.Portal.People_ViewIndividual_WebDriver(individualName);

            //View Attendance History
            test.Portal._People_Verify_Involvement_Graph_GroupDetails(startDate, startTime, groupName, groupTypeName);

            // Login to infellowship to Remove Person from Group
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!");
            test.Infellowship.Groups_Group_Delete_Member_WebDriver(groupName, individualName);
            TestLog.WriteLine("Person removed from Group {0} :", groupName);
            test.Infellowship.LogoutWebDriver();

            //Search for Individual in Portal
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");
            test.Portal.People_ViewIndividual_WebDriver(individualName);

            //View Attendance History After Person Removed from Group
            TestLog.WriteLine("Attendance History after Person removed from Group");
            test.Portal._People_Verify_Involvement_Graph_GroupDetails(startDate, startTime, groupName, groupTypeName);

            //Logout
            test.Portal.LogoutWebDriver();

        }

        #endregion Involvement

    }



        [TestFixture]
        public class Portal_People_Search_FindAPerson_WebDriver : FixtureBaseWebDriver
        {
            [FixtureSetUp]
            public void FixtureSetUp()
            {

            }


            [FixtureTearDown]
            public void FixtureTearDown()
            {

            }



            #region Edit Household

            // Suchitra Changes  
            [Test, RepeatOnFailure]
            [Category(TestCategories.Services.SmokeTest)]
            [Author("Suchitra Patnam")]
            [Description("Edits the Gender to Male through the new Edit HH/IND page in Full View")]
            public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_FullView_GenderMale()
            {

                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                TestLog.WriteLine("Login to Portal");
                test.Portal.LoginWebDriver("ft.householdedit", "FT4life!", "dc");

                // Edit household through the new Edit HH/IND page
                TestLog.WriteLine("Edit the HH/IND values.");
                test.Portal.People_FindAPerson_Household_Edit_HouseholdIndividualValues_FullView_WebDriver(15, "FT HouseholdEdit", "111-111-1111", "email@Household.com", "222-222-2222", "333-333-3333", "444-444-4444", "email@Personal.com", "Active Member", "Auto Sub Status", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), "Comment", "Head", null, "Male");

                // Logout of Portal
                TestLog.WriteLine("Logout of Portal");
                test.Portal.LogoutWebDriver();

            }

            [Test, RepeatOnFailure]
            [Author("Suchitra Patnam")]
            [Description("Edits the Gender to Female through the new Edit HH/IND page in Full View")]
            public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_FullView_GenderFemale()
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                TestLog.WriteLine("Login to Portal");
                test.Portal.LoginWebDriver("ft.householdedit", "FT4life!", "dc");

                // Edit household through the new Edit HH/IND page
                TestLog.WriteLine("Edit the HH/IND values.");
                test.Portal.People_FindAPerson_Household_Edit_HouseholdIndividualValues_FullView_WebDriver(15, "FT HouseholdEdit", "111-111-1111", "email@Household.com", "222-222-2222", "333-333-3333", "444-444-4444", "email@Personal.com", "Active Member", "Auto Sub Status", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), "Comment", "Head", null, "Female");

                // Logout of Portal
                TestLog.WriteLine("Logout of Portal");
                test.Portal.LogoutWebDriver();

            }

            [Test, RepeatOnFailure]
            [Author("Suchitra Patnam")]
            [Description("Edits the Gender to No value through the new Edit HH/IND page in Full View")]
            public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_FullView_NoGender()
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                TestLog.WriteLine("Login to Portal");
                test.Portal.LoginWebDriver("ft.householdedit", "FT4life!", "dc");

                // Edit household through the new Edit HH/IND page
                TestLog.WriteLine("Edit the HH/IND values.");
                test.Portal.People_FindAPerson_Household_Edit_HouseholdIndividualValues_FullView_WebDriver(15, "FT HouseholdEdit", "111-111-1111", "email@Household.com", "222-222-2222", "333-333-3333", "444-444-4444", "email@Personal.com", "Active Member", "Auto Sub Status", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), "Comment", "Head", null, null);

                // Logout of Portal
                TestLog.WriteLine("Logout of Portal");
                test.Portal.LogoutWebDriver();

            }

            // Suchitra Changes  
            [Test, RepeatOnFailure]
            [Author("Suchitra Patnam")]
            [Description("Edits the household position and arital status values for the individuals through the new Edit HH/IND page in Tabbed View")]
            public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_PositionMaritalStatusTab_NoGender()
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                TestLog.WriteLine("Login to Portal");
                test.Portal.LoginWebDriver("ft.householdedit", "FT4life!", "dc");

                // Edit the HH position and Marital status values through the new Edit HH/IND page in Tabbed View
                TestLog.WriteLine("Edit HH position and Marital status through HH/IND edit page.");
                test.Portal.People_FindAPerson_Household_Edit_PositionMaritalStatus_TabbedView_WebDriver(15, "FT HouseholdEdit", "Other", null, null);

                // Logout of Portal
                TestLog.WriteLine("Logout of Portal");
                test.Portal.LogoutWebDriver();

            }

            [Test, RepeatOnFailure]
            [Author("Suchitra Patnam")]
            [Description("Edits the household position and arital status values for the individuals through the new Edit HH/IND page in Tabbed View")]
            public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_PositionMaritalStatusTab_GenderMale()
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                TestLog.WriteLine("Login to Portal");
                test.Portal.LoginWebDriver("ft.householdedit", "FT4life!", "dc");

                // Edit the HH position and Marital status values through the new Edit HH/IND page in Tabbed View
                TestLog.WriteLine("Edit HH position and Marital status through HH/IND edit page.");
                test.Portal.People_FindAPerson_Household_Edit_PositionMaritalStatus_TabbedView_WebDriver(15, "FT HouseholdEdit", "Other", null, "Male");

                // Logout of Portal
                TestLog.WriteLine("Logout of Portal");
                test.Portal.LogoutWebDriver();

            }
 
            [Test, RepeatOnFailure]
            [Author("Suchitra Patnam")]
            [Description("Edits the household position and arital status values for the individuals through the new Edit HH/IND page in Tabbed View")]
            public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_PositionMaritalStatusTab_GenderFemale()
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                TestLog.WriteLine("Login to Portal");
                test.Portal.LoginWebDriver("ft.householdedit", "FT4life!", "dc");

                // Edit the HH position and Marital status values through the new Edit HH/IND page in Tabbed View
                TestLog.WriteLine("Edit HH position and Marital status through HH/IND edit page.");
                test.Portal.People_FindAPerson_Household_Edit_PositionMaritalStatus_TabbedView_WebDriver(15, "FT HouseholdEdit", "Other", null, "Female");

                // Logout of Portal
                TestLog.WriteLine("Logout of Portal");
                test.Portal.LogoutWebDriver();

            }
            #endregion Edit Household

            #region Edit Individual

            [Test, RepeatOnFailure]
            [Author("Mady Kou")]
            [Description("Edits the Individual status date as blank through the new Edit HH/IND page in Full View, check no (New) label in status")]
            public void People_Search_FindAPerson_ViewHousehold_EditHouseholdIndividual_StatusNoDate_Display()
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                TestLog.WriteLine("Login to Portal");
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                // Edit household through the new Edit HH/IND page
                TestLog.WriteLine("Edit the IND values.");
                test.Portal.People_FindAPerson_Household_Edit_IndividualStatus_TabbedView_WebDriver(15, "FT HouseholdEdit", "Attendee", "COL", null, "comment");

                // Logout of Portal
                TestLog.WriteLine("Logout of Portal");
                test.Portal.LogoutWebDriver();

            }

            #endregion


            #region SocialMedia

            [Test, RepeatOnFailure]
            [Category(TestCategories.Services.SmokeTest)]
            [Author("Stuart Platt")]
            [Description("Verifies that the social media fields display on the individual record")]
            [Category("Portal_People_FindAPerson_SocialMedia")]
            public void Portal_People_Search_FindAPerson_SocialMedia_FieldsDisplay()
            {
                // Login to portal

                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

                TestLog.WriteLine("Login To Portal");
                test.Portal.LoginWebDriver("splatt", "Romans10:9", "DC");

                // Find individual
                TestLog.WriteLine("Find Individual");
                test.Portal.People_ViewIndividual_WebDriver("Stuart Platt");

                //Did we make it?
                test.GeneralMethods.VerifyTextPresentWebDriver("Individual Detail");

                //Click on Show additional info
                TestLog.WriteLine("Click on Show Additional info");
                test.Driver.FindElementByLinkText("Show additional info").Click();

                //Verify Social Media Image present
                TestLog.WriteLine("Verify Facebook");
                test.Driver.FindElementsByPartialLinkText("https://www.facebook.com");

                TestLog.WriteLine("Verify Twitter");
                test.Driver.FindElementsByPartialLinkText("http://www.twitter.com");

                TestLog.WriteLine("Verify LinkedIn");
                test.Driver.FindElementsByPartialLinkText("http://www.linkedin.com");

                //Logout
                TestLog.WriteLine("Logout");
                test.Portal.LogoutWebDriver();

            }

            [Test, RepeatOnFailure]
            [Author("Stuart Platt")]
            [Description("Verifies that the social media fields for Facebook can be added and removed")]
            [Category("Portal_People_FindAPerson_SocialMedia")]
            public void Portal_People_Search_FindAPerson_SocialMedia_AddandRemove_Facebook()
            {
                // Login to portal

                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

                TestLog.WriteLine("Login To Portal");
                test.Portal.LoginWebDriver("splatt", "Romans10:9", "DC");

                // Find individual
                TestLog.WriteLine("Find Individual");
                test.Portal.People_ViewIndividual_WebDriver("Social Media");

                //Did we make it?
                test.GeneralMethods.VerifyTextPresentWebDriver("Individual Detail");

                //Edit Individual
                TestLog.WriteLine("Edit Individual");
                test.Driver.FindElementByXPath(string.Format("//*[@id='main_content']/div[2]/div/h6[2]/a")).Click();

                //Remove Social Media
                TestLog.WriteLine("Remove Social Media");
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlIndividualFull_facebook").Clear();

                //Save changes
                TestLog.WriteLine("Save Changes");
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSave").Click();

                //Did we make it?
                test.GeneralMethods.VerifyTextPresentWebDriver("Individual Detail");

                //Click on Show additional info
                TestLog.WriteLine("Click on Show Additional info");
                test.Driver.FindElementByLinkText("Show additional info").Click();

                //Verify Link is not there
                TestLog.WriteLine("Verify Link is gone");
                test.GeneralMethods.VerifyElementNotPresentWebDriver(By.XPath("//img[contains(@src, '/images/icon_facebook.png?')]"));


                //Edit Individual
                TestLog.WriteLine("Edit Individual");
                test.Driver.FindElementByXPath(string.Format("//*[@id='main_content']/div[2]/div/h6[2]/a")).Click();

                //Add Social Media
                TestLog.WriteLine("Add Social Media");
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlIndividualFull_facebook").SendKeys("https://www.facebook.com");

                //Save changes
                TestLog.WriteLine("Save Changes");
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSave").Click();

                //Did we make it?
                test.GeneralMethods.VerifyTextPresentWebDriver("Individual Detail");

                //Click on Show additional info
                TestLog.WriteLine("Click on Show Additional info");
                test.Driver.FindElementByLinkText("Show additional info").Click();

                //Verify Social Media Image present
                TestLog.WriteLine("Verify Facebook");
                test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//img[contains(@src, '/images/icon_facebook.png?')]"));

                //Logout
                TestLog.WriteLine("Logout");
                test.Portal.LogoutWebDriver();

            }

            [Test, RepeatOnFailure]
            [Author("Stuart Platt")]
            [Description("Verifies that the social media fields for Twitter can be added and removed")]
            [Category("Portal_People_FindAPerson_SocialMedia")]
            public void Portal_People_Search_FindAPerson_SocialMedia_AddandRemove_Twitter()
            {
                // Login to portal

                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

                TestLog.WriteLine("Login To Portal");
                test.Portal.LoginWebDriver("splatt", "Romans10:9", "DC");

                // Find individual
                TestLog.WriteLine("Find Individual");
                test.Portal.People_ViewIndividual_WebDriver("Social Media");

                //Did we make it?
                test.GeneralMethods.VerifyTextPresentWebDriver("Individual Detail");

                //Edit Individual
                TestLog.WriteLine("Edit Individual");
                test.Driver.FindElementByXPath(string.Format("//*[@id='main_content']/div[2]/div/h6[2]/a")).Click();

                //Remove Social Media
                TestLog.WriteLine("Remove Social Media");
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlIndividualFull_twitter").Clear();

                //Save changes
                TestLog.WriteLine("Save Changes");
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSave").Click();

                //Did we make it?
                test.GeneralMethods.VerifyTextPresentWebDriver("Individual Detail");

                //Click on Show additional info
                TestLog.WriteLine("Click on Show Additional info");
                test.Driver.FindElementByLinkText("Show additional info").Click();

                //Verify Link is not there
                TestLog.WriteLine("Verify Link is gone");
                test.GeneralMethods.VerifyElementNotPresentWebDriver(By.XPath("//img[contains(@src, '/images/icon_twitter.png?')]"));

                //Edit Individual
                TestLog.WriteLine("Edit Individual");
                test.Driver.FindElementByXPath(string.Format("//*[@id='main_content']/div[2]/div/h6[2]/a")).Click();

                //Add Social Media
                TestLog.WriteLine("Add Social Media");
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlIndividualFull_twitter").SendKeys("http://www.twitter.com");

                //Save changes
                TestLog.WriteLine("Save Changes");
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSave").Click();

                //Did we make it?
                test.GeneralMethods.VerifyTextPresentWebDriver("Individual Detail");

                //Click on Show additional info
                TestLog.WriteLine("Click on Show Additional info");
                test.Driver.FindElementByLinkText("Show additional info").Click();

                //Verify Social Media Image present
                TestLog.WriteLine("Verify Twitter");
                test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//img[contains(@src, '/images/icon_twitter.png?')]"));

                //Logout
                TestLog.WriteLine("Logout");
                test.Portal.LogoutWebDriver();

            }

            [Test, RepeatOnFailure]
            [Author("Stuart Platt")]
            [Description("Verifies that the social media fields for LinkedIn can be added and removed")]
            [Category("Portal_People_FindAPerson_SocialMedia")]
            public void Portal_People_Search_FindAPerson_SocialMedia_AddandRemove_LinkedIn()
            {
                // Login to portal

                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

                TestLog.WriteLine("Login To Portal");
                test.Portal.LoginWebDriver("splatt", "Romans10:9", "DC");

                // Find individual
                TestLog.WriteLine("Find Individual");
                test.Portal.People_ViewIndividual_WebDriver("Social Media");

                //Did we make it?
                test.GeneralMethods.VerifyTextPresentWebDriver("Individual Detail");

                //Edit Individual
                TestLog.WriteLine("Edit Individual");
                test.Driver.FindElementByXPath(string.Format("//*[@id='main_content']/div[2]/div/h6[2]/a")).Click();

                //Remove Social Media
                TestLog.WriteLine("Remove Social Media");
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlIndividualFull_linkedin").Clear();

                //Save changes
                TestLog.WriteLine("Save Changes");
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSave").Click();

                //Did we make it?
                test.GeneralMethods.VerifyTextPresentWebDriver("Individual Detail");

                //Click on Show additional info
                TestLog.WriteLine("Click on Show Additional info");
                test.Driver.FindElementByLinkText("Show additional info").Click();

                //Verify Link is not there
                TestLog.WriteLine("Verify Link is gone");
                test.GeneralMethods.VerifyElementNotPresentWebDriver(By.XPath("//img[contains(@src, '/images/icon_linkedin.png?')]"));

                //Edit Individual
                TestLog.WriteLine("Edit Individual");
                test.Driver.FindElementByXPath(string.Format("//*[@id='main_content']/div[2]/div/h6[2]/a")).Click();

                //Add Social Media
                TestLog.WriteLine("Add Social Media");
                test.GeneralMethods.WaitForElement(test.Driver, By.Id("ctl00_ctl00_MainContent_content_ctlIndividualFull_linkedin"));
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlIndividualFull_linkedin").SendKeys("http://www.linkedin.com");

                //Save changes
                TestLog.WriteLine("Save Changes");
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSave").Click();

                //Did we make it?
                test.GeneralMethods.VerifyTextPresentWebDriver("Individual Detail");

                //Click on Show additional info
                TestLog.WriteLine("Click on Show Additional info");
                test.Driver.FindElementByLinkText("Show additional info").Click();

                //Verify Social Media Image present
                TestLog.WriteLine("Verify LinkedIn");
                test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//img[contains(@src, '/images/icon_linkedin.png?')]"));

                //Logout
                TestLog.WriteLine("Logout");
                test.Portal.LogoutWebDriver();

            }

            [Test, RepeatOnFailure]
            [Author("Stuart Platt")]
            [Description("Verifies that the social media fields only display when present")]
            [Category("Portal_People_FindAPerson_SocialMedia")]
            public void Portal_People_Search_FindAPerson_SocialMedia_Missing()
            {
                // Login to portal

                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

                TestLog.WriteLine("Login To Portal");
                test.Portal.LoginWebDriver("splatt", "Romans10:9", "DC");

                // Find individual
                TestLog.WriteLine("Find Individual");
                test.Portal.People_ViewIndividual_WebDriver("Missing SocialMedia");

                //Did we make it?
                test.GeneralMethods.VerifyTextPresentWebDriver("Individual Detail");

                //Click on Show additional info
                TestLog.WriteLine("Click on Show Additional info");
                test.Driver.FindElementByLinkText("Show additional info").Click();

                //Verify Facebook is not there
                TestLog.WriteLine("Verify Facebook not present");
                test.GeneralMethods.VerifyElementNotPresentWebDriver(By.XPath("//img[contains(@src, '/images/icon_linkedin.png?')]"));

                //Verify Twitter is not there
                TestLog.WriteLine("Verify Twitter not present");
                test.GeneralMethods.VerifyElementNotPresentWebDriver(By.XPath("//img[contains(@src, '/images/icon_twitter.png?')]"));

                //Verify Link is not there
                TestLog.WriteLine("Verify LinkedIn not present");
                test.GeneralMethods.VerifyElementNotPresentWebDriver(By.XPath("//img[contains(@src, '/images/icon_linkedin.png?')]"));

                //Logout
                TestLog.WriteLine("Logout");
                test.Portal.LogoutWebDriver();

            }

            [Test, RepeatOnFailure]
            [Author("Stuart Platt")]
            [Description("Verifies that the social media links work on the individual record")]
            [Category("Portal_People_FindAPerson_SocialMedia")]
            public void Portal_People_Search_FindAPerson_SocialMedia_NavigateToFacebook()
            {
                // Login to portal

                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

                TestLog.WriteLine("Login To Portal");
                test.Portal.LoginWebDriver("splatt", "Romans10:9", "DC");

                // Find individual
                TestLog.WriteLine("Find Individual");
                test.Portal.People_ViewIndividual_WebDriver("Stuart Platt");

                //Did we make it?
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Edit"));
                test.GeneralMethods.VerifyTextPresentWebDriver("Individual Detail");

                //Let's get the FB URL
                test.Driver.FindElementByXPath("//table[@class='info']/tbody/tr/td[3]/form/a[@class='gear_trigger gear_large']").Click();
                test.Driver.FindElementByXPath("//table[@class='info']/tbody/tr/td[3]/form/ul/li[3]/a/span[text()='Edit individual']").Click();
                //var Shane = test.Driver.FindElementByXPath("//a[contains(@href, '/People/Individual/Edit.aspx?IndI//D')]");
                //test.Driver.FindElementByXPath("//a[contains(@href, '/People/Individual/Edit.aspx?IndID')]").Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.Id("ctl00_ctl00_MainContent_content_ctlIndividualFull_facebook"));
                string fbURL = test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlIndividualFull_facebook").GetAttribute("value");
                test.Driver.FindElementByLinkText("Cancel").Click();

                //Click on Show additional info
                TestLog.WriteLine("Click on Show Additional info");
                test.Driver.FindElementByLinkText("Show additional info").Click();

                //Verify Social Media Image present
                TestLog.WriteLine("Verify Facebook");
                ///img[contains(@src, '/images/icon_facebook.png?')]
                test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath(string.Format("//a[contains(@href, '{0}')]", fbURL)));
                IWebElement fbElement = test.Driver.FindElementByXPath("//img[contains(@src, '/images/icon_facebook.png?')]");

                //Verify element is visible
                Assert.IsTrue(fbElement.Displayed, "FB Icon not visible");

                //Store the current window handle
                string BaseWindow = test.Driver.CurrentWindowHandle;

                //Click on Facebook link
                TestLog.WriteLine("Click on Link to Facebook");
                //test.Driver.FindElementByXPath(string.Format("//table[@class='info']/tbody/tr[4]/td/a[1]")).Click();
                fbElement.Click();


                ReadOnlyCollection<string> handles = test.Driver.WindowHandles;

                //Should only have 2 windows
                Assert.AreEqual(2, handles.Count, "Facebook: More Than One Window Was Launched");

                foreach (string handle in handles)
                {

                    if (handle != BaseWindow)
                    {
                        TestLog.WriteLine("Facebook Found");
                        Assert.AreEqual("Douglas Stuart Platt | Facebook", test.Driver.SwitchTo().Window(handle).Title, "Did Not Go To Facebook Page");
                        break;
                    }
                }

                // Verify the Facebook page loads
                TestLog.WriteLine("Find Facebook");
                test.GeneralMethods.WaitForElement(test.Driver, By.XPath("//*[@id='facebook']"));
                ////*[@id="blueBarNAXAnchor"]/div/div/div/div[1]/h1/a/i
                //*[@id="facebook"]
                
                

                TestLog.WriteLine("Verify correct Facebook page");
                test.GeneralMethods.VerifyTextPresentWebDriver("Douglas Stuart Platt");

                //Close Browser
                TestLog.WriteLine("Close Browser");
                test.Driver.Close();

            }

            [Test, RepeatOnFailure]
            [Author("Stuart Platt")]
            [Description("Verifies that the social media fields work on the individual record")]
            [Category("Portal_People_FindAPerson_SocialMedia")]
            public void Portal_People_Search_FindAPerson_SocialMedia_NavigateToTwitter()
            {
                // Login to portal

                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

                TestLog.WriteLine("Login To Portal");
                test.Portal.LoginWebDriver("splatt", "Romans10:9", "DC");

                // Find individual
                TestLog.WriteLine("Find Individual");
                test.Portal.People_ViewIndividual_WebDriver("Stuart Platt");

                //Did we make it?
                test.GeneralMethods.VerifyTextPresentWebDriver("Individual Detail");

                //Click on Show additional info
                TestLog.WriteLine("Click on Show Additional info");
                test.Driver.FindElementByLinkText("Show additional info").Click();

                //Verify Social Media Image present
                TestLog.WriteLine("Verify Twitter");
                
                IWebElement twitterElement = test.Driver.FindElementByXPath("//img[contains(@src, '/images/icon_twitter.png?')]");

                //Verify element is visible
                Assert.IsTrue(twitterElement.Displayed, "Twitter Icon not visible");

                //Store the current window handle
                string BaseWindow = test.Driver.CurrentWindowHandle;

                //Click on Twitter link
                TestLog.WriteLine("Click on Link to Twitter");
                twitterElement.Click();


                ReadOnlyCollection<string> handles = test.Driver.WindowHandles;

                //Should only have 2 windows
                Assert.AreEqual(2, handles.Count, "Twitter: More Than One Window Was Launched");

                foreach (string handle in handles)
                {

                    if (handle != BaseWindow)
                    {
                        TestLog.WriteLine("Twitter Found");
                        //Assert.AreEqual("stuplatt (stuplatt) on Twitter", test.Driver.SwitchTo().Window(handle).Title, "Did Not Go To Twitter Page");
                        Assert.Contains(test.Driver.SwitchTo().Window(handle).Title, "Twitter", "Did Not Go To Twitter Page");
                        Assert.Contains(test.Driver.SwitchTo().Window(handle).Title, "stuplatt", "Did Not Go To Stuart's Page");
                        break;
                    }
                }

                // Verify the Facebook page loads
                TestLog.WriteLine("Find Twitter Logo");
                test.GeneralMethods.WaitForElement(test.Driver, By.XPath(string.Format("//*[@id='doc']/div[1]/div/div/div/ul/li/a/span")), 20);
                ////*[@id="doc"]/div[1]/div/div/div/ul/li/a/span

                TestLog.WriteLine("Verify correct Twitter page");
                test.GeneralMethods.VerifyTextPresentWebDriver("stuplatt");

                //Close Browser
                TestLog.WriteLine("Close Browser");
                test.Driver.Close();

            }

            [Test, RepeatOnFailure]
            [Author("Stuart Platt")]
            [Description("Verifies that the social media fields work on the individual record")]
            [Category("Portal_People_FindAPerson_SocialMedia")]
            public void Portal_People_Search_FindAPerson_SocialMedia_NavigateToLinkedIn()
            {
                // Login to portal

                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

                TestLog.WriteLine("Login To Portal");
                test.Portal.LoginWebDriver("splatt", "Romans10:9", "DC");

                // Find individual
                TestLog.WriteLine("Find Individual");
                test.Portal.People_ViewIndividual_WebDriver("Stuart Platt");

                //Did we make it?
                test.GeneralMethods.VerifyTextPresentWebDriver("Individual Detail");

                //Click on Show additional info
                TestLog.WriteLine("Click on Show Additional info");
                test.Driver.FindElementByLinkText("Show additional info").Click();

                //Verify Social Media Image present
                TestLog.WriteLine("Verify LinkedIn");
                IWebElement linkInElement = test.Driver.FindElementByXPath("//img[contains(@src, '/images/icon_linkedin.png?')]");

                //Verify element is visible
                Assert.IsTrue(linkInElement.Displayed, "Linked In Icon not visible");

                //Store the current window handle
                string BaseWindow = test.Driver.CurrentWindowHandle;

                //Click on LinkedIn link
                TestLog.WriteLine("Click on Link to LinkedIn");
                linkInElement.Click();


                ReadOnlyCollection<string> handles = test.Driver.WindowHandles;

                //Should only have 2 windows
                Assert.AreEqual(2, handles.Count, "LinkedIn: More Than One Window Was Launched");

                TestLog.WriteLine("Title: " + test.Driver.SwitchTo().Window(handles[1]).Title);
                TestLog.WriteLine("URL: " + test.Driver.SwitchTo().Window(handles[1]).Url);

                WebDriverWait wait = new WebDriverWait(test.Driver, TimeSpan.FromSeconds(30));
                bool submitted = wait.Until<bool>((d) => { ExpectedConditions.TitleContains("LinkedIn").Equals(true); return true; });

                foreach (string handle in handles)
                {

                    if (handle != BaseWindow)
                    {
                        TestLog.WriteLine("LinkedIn Found");
                        Assert.AreEqual("Douglas \"Stuart\" Platt | LinkedIn", test.Driver.SwitchTo().Window(handle).Title, "Did Not Go To LinkedIn Page");
                        break;
                    }
                }

                // Verify the LinkedIn page loads
                TestLog.WriteLine("Find LinkedIn Logo");
                test.GeneralMethods.WaitForElement(test.Driver, By.Id(string.Format("li-logo")), 20);

                TestLog.WriteLine("Verify correct LinkedIn page");
                // test.GeneralMethods.VerifyTextPresentWebDriver("QA Engineer at The Active Network");
                Assert.IsTrue(test.Driver.Url.Contains("//www.linkedin.com"), "The opened page is not expected page in linked in");

                //Close Browser
                TestLog.WriteLine("Close Browser");
                test.Driver.Close();

            }

            #endregion SocialMedia

            #region AddAssignment

            [Test, RepeatOnFailure]
            [Author("Mady Kou")]
            [Description("FO-3274 Add assignment without name cause no system error but error message")]
            public void Portal_People_Search_FindAPerson_ViewIndividual_AddAssignment()
            {
                // Login to Portal
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C3");

                try
                {
                    // Navigate to the Add Assignment Page
                    test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Assignments.Add_Assignment);
                    test.Driver.FindElementById("btn_search").Click();
                    test.GeneralMethods.WaitForElementVisible(By.Id("new_assignment_search_results"));

                    Assert.IsTrue(test.Driver.FindElementById("new_assignment_search_results").Text.StartsWith("Your search returned 0 people."), "Fail to find out the 0 individual message on page");
                    Assert.IsTrue(test.GeneralMethods.IsElementVisibleWebDriver(By.Id("btn_search")), "Search button should still display");
                }
                finally
                {
                    test.Driver.FindElementById("return_to_app_inner").Click();

                    // Logout of Portal
                    test.Portal.LogoutWebDriver();
                }
                
            }

            #endregion AddAssignment

            
        }


    }
