using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace FTTests.FOOS {
    [TestFixture]
    public class FOOS : FixtureBase {
        private struct FOOSConstants {
            public const string Announcements = "link=Announcements";
            public const string Report_Manager = "link=Report Manager";
            public const string Group_Email = "link=Group Email";

            public const string Return = "btnGoBack";
            public const string Search = "btnSearch";
            public const string Add_New = "lnkAddNew";
        }

        #region Announcements
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the announcements page.")]
        public void Announcements() {
            // Login to foos
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.FOOS.Login();

            // View the announcements page
            test.Selenium.ClickAndWaitForPageToLoad(FOOSConstants.Announcements);

            // Verify user was taken to the announcements page
            Assert.AreEqual("Messages", test.Selenium.GetText("//span[@class='sectionTitle']"));

            // Logout of foos
            test.FOOS.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the add announcements page.")]
        public void Announcements_Add() {
            // Login to foos
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.FOOS.Login();

            // View the announcements page
            test.Selenium.ClickAndWaitForPageToLoad(FOOSConstants.Announcements);

            // View the new announcement page
            test.Selenium.ClickAndWaitForPageToLoad("btnAddNew");

            // Verify data on the page
            test.Selenium.VerifyTextPresent("Title Bar Text:");
            test.Selenium.VerifyTextPresent("Full Message:");
            test.Selenium.VerifyTextPresent("Short Message:");

            // Return to the announcements page
            test.Selenium.ClickAndWaitForPageToLoad(FOOSConstants.Return);

            // Logout of foos
            test.FOOS.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the edit announcements page.")]
        public void Announcements_Edit() {
            // Login to foos
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.FOOS.Login();

            // View the announcements page
            test.Selenium.ClickAndWaitForPageToLoad(FOOSConstants.Announcements);

            // View the edit announcement page
            test.Selenium.ClickAndWaitForPageToLoad("link=Edit");

            // Verify data on the page
            test.Selenium.VerifyTextPresent("Title Bar Text:");
            test.Selenium.VerifyTextPresent("Full Message:");
            test.Selenium.VerifyTextPresent("Short Message:");

            // Return to the announcements page
            test.Selenium.ClickAndWaitForPageToLoad(FOOSConstants.Return);

            // Logout of foos
            test.FOOS.Logout();
        }
        #endregion Announcements

        #region Report Manager
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the report manager page.")]
        public void ReportManager() {
            // Login to foos
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.FOOS.Login();

            // View the report manager page
            test.Selenium.ClickAndWaitForPageToLoad(FOOSConstants.Report_Manager);

            // Verify user was taken to the report manager page
            Assert.AreEqual("Report Search", test.Selenium.GetText("//span[@class='SectionTitle']"));

            // Logout of foos
            test.FOOS.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the group manager page.")]
        public void ReportManager_Groups() {
            // Login to foos
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.FOOS.Login();

            // View the report manager page
            test.Selenium.ClickAndWaitForPageToLoad(FOOSConstants.Report_Manager);

            // View the groups section
            test.Selenium.ClickAndWaitForPageToLoad("link=Groups");

            // Verify data on the page
            test.Selenium.VerifyTextPresent("Group Name: *");
            test.Selenium.VerifyTextPresent("Group Type:");
            test.Selenium.VerifyTextPresent("New Sub Group Name:");
            test.Selenium.VerifyTextPresent("Sub Group Type:");

            // Logout of foos
            test.FOOS.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Performs a report search.")]
        public void ReportManager_Search() {
            // Login to foos
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.FOOS.Login();

            // View the report manager page
            test.Selenium.ClickAndWaitForPageToLoad(FOOSConstants.Report_Manager);

            // Search for reports
            test.Selenium.Select("ddlFunctionalArea_dropDownList", "Administration");
            test.Selenium.ClickAndWaitForPageToLoad(FOOSConstants.Search);

            // Verify reports are listed
            Assert.AreEqual("Manage", test.Selenium.GetTable("dgSearch.2.0"));
            Assert.AreEqual("Administration", test.Selenium.GetTable("dgSearch.2.1"));

            // Logout of foos
            test.FOOS.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the add report page.")]
        public void ReportManager_Add() {
            // Login to foos
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.FOOS.Login();

            // View the report manager page
            test.Selenium.ClickAndWaitForPageToLoad(FOOSConstants.Report_Manager);

            // View the add report page
            test.Selenium.ClickAndWaitForPageToLoad(FOOSConstants.Add_New);

            // Verify data on the page
            Assert.AreEqual("Settings", test.Selenium.GetText("//span[@class='SectionTitle']"));
            test.Selenium.VerifyTextPresent("Functional Area:");
            test.Selenium.VerifyTextPresent("Report Code: *");
            test.Selenium.VerifyTextPresent("Name: *");
            test.Selenium.VerifyTextPresent("Path: *");
            test.Selenium.VerifyTextPresent("Version: *");
            test.Selenium.VerifyTextPresent("Keyword(s):");

            // Logout of foos
            test.FOOS.Logout();
        }
        #endregion Report Manager

        #region Group Email
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the group email page.")]
        public void GroupEmail() {
            // Login to foos
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.FOOS.Login();

            // View the group email page
            test.Selenium.ClickAndWaitForPageToLoad(FOOSConstants.Group_Email);

            // Verify user was taken to the group email page
            Assert.AreEqual("Email Recipients", test.Selenium.GetText("//span[@class='SectionTitle']"));

            // Verify user can search by church name or church code
            Assert.IsTrue(test.Selenium.IsElementPresent("//input[@id='txtSearch' and preceding-sibling::p[text()='Search by Name or Church Code:']]"));

            // Verify "Send to all users" is not checked
            Assert.IsFalse(test.Selenium.IsChecked("cbAll"));

            // Logout of foos
            test.FOOS.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user can search by church name when composing a group email.")]
        public void GroupEmail_SearchByChurchName() {
            // Login to foos
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.FOOS.Login();

            // View the group email page
            test.Selenium.ClickAndWaitForPageToLoad(FOOSConstants.Group_Email);

            // Verify the user can search by church name
            test.Selenium.Click("rbtnChurch");
            test.Selenium.TypeKeys("txtSearch", "Milestone");

            for (int i = 1; i <= test.Selenium.GetXpathCount("//select[@id='lstChurch']/option"); i++) {
                Assert.IsTrue(test.Selenium.GetText(string.Format("//select[@id='lstChurch']/option[{0}]/text()", i)).Contains("Milestone"));
            }

            // Logout of foos
            test.FOOS.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user can search by church code when composing a group email.")]
        public void GroupEmail_SearchByChurchCode() {
            // Login to foos
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.FOOS.Login();

            // View the group email page
            test.Selenium.ClickAndWaitForPageToLoad(FOOSConstants.Group_Email);

            // Verify the user can search by church code
            test.Selenium.Click("rbtnChurch");
            test.Selenium.TypeKeys("txtSearch", "MCDFWTX");

            for (int i = 1; i <= test.Selenium.GetXpathCount("//select[@id='lstChurch']/option"); i++) {
                Assert.IsTrue(test.Selenium.GetText(string.Format("//select[@id='lstChurch']/option[{0}]/text()", i)).Contains("MCDFWTX"));
            }

            // Logout of foos
            test.FOOS.Logout();
        }
        #endregion Group Email
    }
}
