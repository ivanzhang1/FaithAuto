using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace FTTests.Portal.People {
    [TestFixture]
    [Importance(Importance.Critical)]
    public class Portal_People_CounselingMentoring : FixtureBase {
        #region View Relationships
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Views the find person popup on the view relationships page.")]
        public void People_CounselingMentoring_ViewRelationships_FindPerson() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->view relationships
            test.Selenium.Navigate(Navigation.People.CounselingMentoring.View_Relationships);

            // View the find person popup
            test.Selenium.ClickAndSelectPopUp(GeneralLinks.Find_person, "psuedoModal", "30000");

            // Close the popup
            test.Selenium.Click(GeneralLinks.Cancel);
            test.Selenium.SelectWindow("");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the create new relationship page.")]
        public void People_CounselingMentoring_ViewRelationships_Add() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->view relationships
            test.Selenium.Navigate(Navigation.People.CounselingMentoring.View_Relationships);

            // View the create page
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Add);

            // Verify the text on the find person links
            Assert.AreEqual("Find person", test.Selenium.GetText("ctl00_ctl00_MainContent_content_ucPrimaryRelationshipMember_ctlFindPerson_lnkFindPerson"));
            Assert.AreEqual("Find person", test.Selenium.GetText("ctl00_ctl00_MainContent_content_ucSecondaryRelationshipMember_ctlFindPerson_lnkFindPerson"));

            // View the find person popup
            test.Selenium.ClickAndSelectPopUp(GeneralLinks.Find_person, "psuedoModal", "30000");

            // Close the popup
            test.Selenium.Click(GeneralLinks.Cancel);
            test.Selenium.SelectWindow("");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the find person popup for the first person off of the create new relationship page.")]
        public void People_CounselingMentoring_ViewRelationships_Add_FindPerson_FirstPerson() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->view relationships
            test.Selenium.Navigate(Navigation.People.CounselingMentoring.View_Relationships);

            // View the create page
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Add);

            // View the find person popup (first person)
            test.Selenium.ClickAndSelectPopUp("ctl00_ctl00_MainContent_content_ucPrimaryRelationshipMember_ctlFindPerson_lnkFindPerson", "psuedoModal", "30000");

            // Close the popup
            test.Selenium.Click(GeneralLinks.Cancel);
            test.Selenium.SelectWindow("");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the find person popup for the second person off of the create new relationship page.")]
        public void People_CounselingMentoring_ViewRelationships_Add_SecondPerson_FirstPerson() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->view relationships
            test.Selenium.Navigate(Navigation.People.CounselingMentoring.View_Relationships);

            // View the create page
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Add);

            // View the find person popup (second person)
            test.Selenium.ClickAndSelectPopUp("ctl00_ctl00_MainContent_content_ucSecondaryRelationshipMember_ctlFindPerson_lnkFindPerson", "psuedoModal", "30000");

            // Close the popup
            test.Selenium.Click(GeneralLinks.Cancel);
            test.Selenium.SelectWindow("");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the find person for the manage relation page")]
        public void People_CounselingMentoring_ViewRelationships_ViewSummary_ManageRelation() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->view relationships
            test.Selenium.Navigate(Navigation.People.CounselingMentoring.View_Relationships);

            // View the summary page for a relationship
            test.Selenium.ClickAndWaitForPageToLoad("link=View summary");

            // View the manage relation page
            test.Selenium.ClickAndWaitForPageToLoad("link=Add/Edit members");

            // View the find person popup
            test.Selenium.ClickAndSelectPopUp(GeneralLinks.Find_person, "psuedoModal", "30000");

            // Close the popup
            test.Selenium.Click(GeneralLinks.Cancel);
            test.Selenium.SelectWindow("");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Joyce Li")]
        [Description("Add/Edit note for the manage relation page")]
        public void People_CounselingMentoring_ViewRelationships_ViewSummary_RelationshipNote()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->view relationships
            test.Selenium.Navigate(Navigation.People.CounselingMentoring.View_Relationships);

            // View the summary page for a relationship
            test.Selenium.ClickAndWaitForPageToLoad("link=View summary");

            // View the Relationship Note page
            test.Selenium.ClickAndWaitForPageToLoad("link=Add/Edit");

            // Verify title, text, label
            test.Selenium.VerifyTitle("Fellowship One :: Relationship Note");
            test.Selenium.VerifyTextPresent("Relationship Note");
            Assert.AreEqual("Note *", test.Selenium.GetText("ctl00_ctl00_MainContent_content_lblNote"));

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Joyce Li")]
        [Description("Add/Edit note for inactive person on the manage relation page")]
        public void People_CounselingMentoring_ViewRelationships_ViewSummary_RelationshipNote_InactivePerson()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->view relationships
            test.Selenium.Navigate(Navigation.People.CounselingMentoring.View_Relationships);

            // Select Inactive option in the Show dropdown list
            test.Selenium.Select("ctl00_ctl00_MainContent_content_ddlActiveFilter_dropDownList", "Inactive");
            
            // Click on Search button
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.Search);

            // View the summary page for a relationship
            test.Selenium.ClickAndWaitForPageToLoad("link=View summary");

            // View the Relationship Note page
            test.Selenium.ClickAndWaitForPageToLoad("link=Add/Edit");

            // Verify title, text, label
            test.Selenium.VerifyTitle("Fellowship One :: Relationship Note");
            test.Selenium.VerifyTextPresent("Relationship Note");
            Assert.AreEqual("Note *", test.Selenium.GetText("ctl00_ctl00_MainContent_content_lblNote"));

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion View Relationships
    }
}