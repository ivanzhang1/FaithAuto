using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Selenium;
using ActiveUp.Net.Mail;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;


namespace FTTests.infellowship.EmailsCaptcha {

    // All email will route to infellowship.group.tests@gmail.com / password: BM.Admin09

    [TestFixture, DoesNotRunInJenkins]
    public class infellowship_Groups_ProspectWorkflow_Captcha_WebDriver : FixtureBaseWebDriver
    {

        #region Private Members
        private string _socName = "Prospect SoC WD";
        private string _groupTypeName = "Prospect GroupType WD";
        private string _groupTypeName2 = "Prospect GroupType 2 WD";
        private string _groupName = "Prospect Group WD";
        private string _groupName2 = "Prospect Group 2 WD";
        private string _groupName3 = "Prospect Group 3 WD";
        private string _socAddPersonGroup = "Prospect Group Add Owner WD";
        #endregion Private Members

        #region Fixture Setup

        [FixtureSetUp]
        public void FixtureSetUp()
        {
            // Groups
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName2);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", _groupTypeName, new List<int> { 1, 2, 3, 4, 5, 9, 10, 11, 12 });
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", _groupTypeName2, new List<int> { 1, 2, 3, 4, 5, 9, 10, 11, 12 });
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(), GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, false, true, null, true);
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName, "Group Member", "Member");
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _socAddPersonGroup, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, _socAddPersonGroup, "Group Leader", "Leader");
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", _groupName2, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString());
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName2, "Group Leader", "Leader");
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", _groupTypeName, new List<int> { 1, 2, 3, 4, 5, 9, 10, 11, 12 });
            base.SQL.Groups_Group_Create(254, _groupTypeName2, "Bryan Mikaelian", _groupName3, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(), GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, false, true, null, true);
            base.SQL.Groups_Group_AddLeaderOrMember(254, _groupName3, "Group Leader", "Leader");

            // Span of Care
            base.SQL.Groups_SpanOfCare_Create(254, _socName, "Bryan Mikaelian", "SOC Owner WD", new List<string>() { _groupTypeName });
        }

        #endregion Fixture Setup

        #region Fixture Teardown

        [FixtureTearDown]
        public void FixtureTearDown()
        {
            //base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName2);
            base.SQL.Groups_SpanOfCare_Delete(254, _socName);
        }

        #endregion Fixture Teardown

        #region Express Interest
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the church contact gets an email if a person expresses interest to a group that has no leader.")]
        public void ProspectWorkflow_ExpressInterest_To_Group_No_Leader()
        {
            // Initial Data
            var groupName = "No Leader WD";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString());

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            // Express interest to a group without a leader  
            test.Infellowship.Groups_Prospect_ExpressInterest_WebDriver(groupName, "Test", "Prospect", "infellowship.group.tests@gmail.com");

            // Verify email comes through

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the church contact gets an email if a person expresses interest to a group that has a leader without an InFellowship account.")]
        public void ProspectWorkflow_ExpressInterest_To_Group_Leader_With_No_InFellowship_Account()
        {
            // Initial Data
            var groupName = "No Leader Account";
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Create(254, _groupTypeName, "Bryan Mikaelian", groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString());

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            // Express interest to a group with a leader without an InFellowship Account
            test.Infellowship.Groups_Prospect_ExpressInterest_WebDriver(groupName, "Test", "Prospect", "infellowship.group.tests@gmail.com");

            // Verify email comes through

            // Clean up
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a person can express interest to a group.")]
        public void ProspectWorkflow_ExpressInterest_To_Group()
        {
            // Set initial conditions
            int churchId = 254;
            string firstName = "Test";
            string lastName = "Prospect";
            string email = "infellowship.group.tests@gmail.com";
            base.SQL.People_DeleteProspectInfo(churchId, _groupName, firstName, lastName, email);

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            // Express interest to a group
            test.Infellowship.Groups_Prospect_ExpressInterest_WebDriver(_groupName, firstName, lastName, email);

            // Login to infellowship
            test.Infellowship.LoginWebDriver("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the group and its prospects
            test.Infellowship.Groups_Prospect_View_All_WebDriver(_groupName);

            // Verify the individual appears on the prospect list
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.InFellowship_ActiveProspects, "Test Prospect", "Name"));
            int itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.InFellowship_ActiveProspects, "Test Prospect", "Name");
            //Assert.AreEqual("Interested", test.Selenium.GetTable(string.Format("{0}.{1}.3", TableIds.InFellowship_ActiveProspects, itemRow + 1)));
            Assert.AreEqual("Test Prospect", test.Driver.FindElementByXPath(TableIds.InFellowship_ActiveProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[0].Text);
            Assert.AreEqual("Interested", test.Driver.FindElementByXPath(TableIds.InFellowship_ActiveProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[3].Text);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Verifies a person can express interest to a group.")]
        public void ProspectWorkflow_ExpressInterest_To_Group_Captcha()
        {
            // Set initial conditions
            int churchId = 254;
            string firstName = "Test";
            string lastName = "Prospect";
            string email = string.Format("{0}.active@gmail.com", Guid.NewGuid().ToString().Substring(0, 6));

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            // Express interest to a group
            test.Infellowship.Groups_Prospect_ExpressInterest_WebDriver(_groupName, firstName, lastName, email);

            // Delete Prospect Info
            base.SQL.People_DeleteProspectInfo(churchId, _groupName, firstName, lastName, email);


        }


        [Test, RepeatOnFailure, MultipleAsserts, Timeout(120000)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the statuses are correct for a prospect when they go from interested to invited by leader action.")]
        public void ProspectWorkflow_ExpressInterest_To_Invited_Status_Correct_Leader()
        {
            // Set initial conditions
            int churchId = 254;
            string firstName = "Expressed";
            string lastName = "Invited";
            string email = "infellowship.group.tests@gmail.com";
            base.SQL.People_DeleteProspectInfo(churchId, _groupName, firstName, lastName, email);

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            // Express interest to a group
            test.Infellowship.Groups_Prospect_ExpressInterest_WebDriver(_groupName, firstName, lastName, email);

            // Login to infellowship
            test.Infellowship.LoginWebDriver("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the group and its prospects
            test.Infellowship.Groups_Group_View_Prospects_WebDriver(_groupName);

            // Verify the individual appears on the prospect list
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.InFellowship_ActiveProspects, "Expressed Invited", "Name"));
            int itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.InFellowship_ActiveProspects, "Expressed Invited", "Name");
            Assert.AreEqual("Expressed Invited", test.Driver.FindElementByXPath(TableIds.InFellowship_ActiveProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[0].Text);
            Assert.AreEqual("Interested", test.Driver.FindElementByXPath(TableIds.InFellowship_ActiveProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[3].Text);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

            // Login to Portal
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "QAEUNLX0C2");

            // View the prospects of the Group 
            test.Portal.Groups_Group_View_Prospects_WebDriver(_groupName);

            // Verify individual shows up as an active prospect that is interested and active                         
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_Group_ActiveProspects, "Expressed Invited", "Name"));
            itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Groups_Group_ActiveProspects, "Expressed Invited", "Name");
            Assert.AreEqual("Expressed Invited", test.Driver.FindElementByXPath(TableIds.Groups_Group_ActiveProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[1].Text);
            //Assert.AreEqual("Interested", test.Selenium.GetTable(string.Format("{0}.{1}.4", TableIds.Groups_Group_ActiveProspects, itemRow)), "Prospect status was not correct.");
            Assert.AreEqual("Interested", test.Driver.FindElementByXPath(TableIds.Groups_Group_ActiveProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[4].Text, "Prospect status was not correct.");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to infellowship
            test.Infellowship.LoginWebDriver("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the group and its prospects
            test.Infellowship.Groups_Prospect_View_WebDriver(_groupName, "Expressed Invited");

            // Accept the interested prospect
            //test.Selenium.ClickAndWaitForPageToLoad("button_allow");
            test.Driver.FindElementById("button_allow").Click();

            // View the prospects of the group
            test.Infellowship.Groups_Group_View_Prospects_WebDriver(_groupName);

            // Verify individual shows up as an active prospect and is invited
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.InFellowship_ActiveProspects, "Expressed Invited", "Name"));
            itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.InFellowship_ActiveProspects, "Expressed Invited", "Name");
            Assert.AreEqual("Expressed Invited", test.Driver.FindElementByXPath(TableIds.InFellowship_ActiveProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[0].Text);
            //Assert.AreEqual("Invited", test.Selenium.GetTable(string.Format("{0}.{1}.3", TableIds.InFellowship_ActiveProspects, itemRow + 1)));
            Assert.AreEqual("Invited", test.Driver.FindElementByXPath(TableIds.InFellowship_ActiveProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[3].Text);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

            // Login to Portal
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "QAEUNLX0C2");

            // View the prospects of the group
            test.Portal.Groups_Group_View_Prospects_WebDriver(_groupName);

            // Verify individual shows up as an active prospect that is invited and active 
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_Group_ActiveProspects, "Expressed Invited", "Name"));
            itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Groups_Group_ActiveProspects, "Expressed Invited", "Name");
            Assert.AreEqual("Expressed Invited", test.Driver.FindElementByXPath(TableIds.Groups_Group_ActiveProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[1].Text);
            //Assert.AreEqual("Invited", test.Selenium.GetTable(string.Format("{0}.{1}.4", TableIds.Groups_Group_ActiveProspects, itemRow)), "Prospect status was not correct.");
            Assert.AreEqual("Invited", test.Driver.FindElementByXPath(TableIds.Groups_Group_ActiveProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[4].Text, "Prospect status was not correct.");

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure, MultipleAsserts, Timeout(120000)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the statuses are correct for a prospect when they go from interested to invited by owner action.")]
        public void ProspectWorkflow_ExpressInterest_To_Invited_Status_Correct_Owner()
        {
            // Set initial conditions
            int churchId = 254;
            string firstName = "Express";
            string lastName = "InvitedOwner";
            string email = "infellowship.group.tests@gmail.com";
            base.SQL.People_DeleteProspectInfo(churchId, _groupName, firstName, lastName, email);

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            // Express interest to a group
            test.Infellowship.Groups_Prospect_ExpressInterest_WebDriver(_groupName, firstName, lastName, email);

            // Login to infellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View your Span of Care and the prospects of a specific group
            test.Infellowship.Groups_Prospect_View_All_WebDriver(_socName, _groupName);

            // Verify the individual appears on the prospect list
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.InFellowship_ActiveProspects, "Express InvitedOwner", "Name"));
            int itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.InFellowship_ActiveProspects, "Express InvitedOwner", "Name");
            Assert.AreEqual("Express InvitedOwner", test.Driver.FindElementByXPath(TableIds.InFellowship_ActiveProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[0].Text);
            //Assert.AreEqual("Interested", test.Selenium.GetTable(string.Format("{0}.{1}.3", TableIds.InFellowship_ActiveProspects, itemRow + 1)));
            Assert.AreEqual("Interested", test.Driver.FindElementByXPath(TableIds.InFellowship_ActiveProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[3].Text);


            // View your Span of Care and a specific prospects of a specific group
            test.Infellowship.Groups_Prospect_View_WebDriver(_socName, _groupName, "Express InvitedOwner");

            // Accept the interested prospect
            test.Driver.FindElementById("button_allow").Click();

            // View the prospects of the group
            test.Infellowship.Groups_Group_View_Prospects_WebDriver(_socName, _groupName);

            // Verify individual shows up as an active prospect and is invited      
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.InFellowship_ActiveProspects, "Express InvitedOwner", "Name"));
            itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.InFellowship_ActiveProspects, "Express InvitedOwner", "Name");
            Assert.AreEqual("Express InvitedOwner", test.Driver.FindElementByXPath(TableIds.InFellowship_ActiveProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[0].Text);
            //Assert.AreEqual("Invited", test.Selenium.GetTable(string.Format("{0}.{1}.3", TableIds.InFellowship_ActiveProspects, itemRow + 1)));
            Assert.AreEqual("Invited", test.Driver.FindElementByXPath(TableIds.InFellowship_ActiveProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[3].Text);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

            // Login to Portal
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "QAEUNLX0C2");

            // View the prospects of the group
            test.Portal.Groups_Group_View_Prospects_WebDriver(_groupName);

            // Verify individual shows up as an active prospect that is invited and active 
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_Group_ActiveProspects, "Express InvitedOwner", "Name"));
            itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Groups_Group_ActiveProspects, "Express InvitedOwner", "Name");
            Assert.AreEqual("Express InvitedOwner", test.Driver.FindElementByXPath(TableIds.Groups_Group_ActiveProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[1].Text);
            //Assert.AreEqual("Invited", test.Selenium.GetTable(string.Format("{0}.{1}.4", TableIds.Groups_Group_ActiveProspects, itemRow)), "Prospect status was not correct.");
            Assert.AreEqual("Invited", test.Driver.FindElementByXPath(TableIds.Groups_Group_ActiveProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[4].Text);

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the statuses are correct for a prospect when they go from interested to denied by leader action.")]
        public void ProspectWorkflow_ExpressInterest_To_Denied_Status_Correct_Leader()
        {
            // Set initial conditions
            int churchId = 254;
            string firstName = "Express";
            string lastName = "Denied";
            string email = "infellowship.group.tests@gmail.com";
            base.SQL.People_DeleteProspectInfo(churchId, _groupName, firstName, lastName, email);

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            // Express interest to a group
            test.Infellowship.Groups_Prospect_ExpressInterest_WebDriver(_groupName, firstName, lastName, email);

            // Login to infellowship
            test.Infellowship.LoginWebDriver("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the group and its prospects
            test.Infellowship.Groups_Prospect_View_All_WebDriver(_groupName);

            // Verify the individual appears on the prospect list
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.InFellowship_ActiveProspects, "Express Denied", "Name"));
            int itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.InFellowship_ActiveProspects, "Express Denied", "Name");
            Assert.AreEqual("Express Denied", test.Driver.FindElementByXPath(TableIds.InFellowship_ActiveProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[0].Text);
            //Assert.AreEqual("Interested", test.Selenium.GetTable(string.Format("{0}.{1}.3", TableIds.InFellowship_ActiveProspects, itemRow + 1)));
            Assert.AreEqual("Interested", test.Driver.FindElementByXPath(TableIds.InFellowship_ActiveProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[3].Text);

            // View an individual prospect
            test.Infellowship.Groups_Prospect_View_WebDriver(_groupName, "Express Denied");

            // Decline the interested prospect 
            test.Driver.FindElementById("button_deny").Click();
            test.Driver.FindElementById("submit_deny").Click();

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

            // Login to Portal
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "QAEUNLX0C2");

            // View the prospects of the Group
            test.Portal.Groups_Group_View_Prospects_WebDriver(_groupName);

            // Verify individual shows up as an closed prospect that is denied  
            test.Driver.FindElementById(Portal.Groups.GroupsByGroupTypeConstants.GroupManagement.Link_ViewProspects_Closed_WD).Click();
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_Group_ClosedProspects, "Express Denied", "Name"));
            itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Groups_Group_ClosedProspects, "Express Denied", "Name");
            Assert.AreEqual("Express Denied", test.Driver.FindElementByXPath(TableIds.Groups_Group_ClosedProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[1].Text);
            //Assert.AreEqual("Denied", test.Selenium.GetTable(string.Format("{0}.{1}.3", TableIds.Groups_Group_ClosedProspects, itemRow)), "Prospect status was not correct.");
            Assert.AreEqual("Denied", test.Driver.FindElementByXPath(TableIds.Groups_Group_ClosedProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[3].Text);

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the statuses are correct for a prospect when they go from interested to denied by owner action.")]
        public void ProspectWorkflow_ExpressInterest_To_Denied_Status_Correct_Owner()
        {
            // Set initial conditions
            int churchId = 254;
            string firstName = "Express";
            string lastName = "DeniedOwner";
            string email = "infellowship.group.tests@gmail.com";
            base.SQL.People_DeleteProspectInfo(churchId, _groupName, firstName, lastName, email);

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            // Express interest to a group
            test.Infellowship.Groups_Prospect_ExpressInterest_WebDriver(_groupName, firstName, lastName, email);

            // Login to infellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View your Span of Care and the prospects of a specific group
            test.Infellowship.Groups_Prospect_View_All_WebDriver(_socName, _groupName);

            // Verify the individual appears on the prospect list
            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.InFellowship_ActiveProspects, "Express DeniedOwner", "Name"));
            int itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.InFellowship_ActiveProspects, "Express DeniedOwner", "Name");
            Assert.AreEqual("Express DeniedOwner", test.Driver.FindElementByXPath(TableIds.InFellowship_ActiveProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[0].Text);
            //Assert.AreEqual("Interested", test.Selenium.GetTable(string.Format("{0}.{1}.3", TableIds.InFellowship_ActiveProspects, itemRow + 1)));
            Assert.AreEqual("Interested", test.Driver.FindElementByXPath(TableIds.InFellowship_ActiveProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[3].Text);

            // View your Span of Care and a specific prospects of a specific group
            test.Infellowship.Groups_Prospect_View_WebDriver(_socName, _groupName, "Express DeniedOwner");

            // Decline the interested prospect
            test.Driver.FindElementById("button_deny").Click();
            test.Driver.FindElementById("submit_deny").Click();

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

            // Login to Portal
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "QAEUNLX0C2");

            // View the prospects of the Group
            test.Portal.Groups_Group_View_Prospects_WebDriver(_groupName);

            // Verify individual shows up as an closed prospect that is denied  
            test.Driver.FindElementById(Portal.Groups.GroupsByGroupTypeConstants.GroupManagement.Link_ViewProspects_Closed_WD).Click();

            //Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_Group_ClosedProspects, "Express DeniedOwner", "Name"));
            itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Groups_Group_ClosedProspects, "Express DeniedOwner", "Name");
            Assert.AreEqual("Express DeniedOwner", test.Driver.FindElementByXPath(TableIds.Groups_Group_ClosedProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[1].Text);
            //Assert.AreEqual("Denied", test.Selenium.GetTable(string.Format("{0}.{1}.3", TableIds.Groups_Group_ClosedProspects, itemRow)), "Prospect status was not correct.");
            Assert.AreEqual("Denied", test.Driver.FindElementByXPath(TableIds.Groups_Group_ClosedProspects).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[3].Text);

            // Logout of Portal
            test.Portal.LogoutWebDriver();
        }

        #region Captcha
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Felix Gaytan")]
        [Description("Verifies Invalid Captcha")]
        public void ProspectWorkflow_ExpressInterest_Invalid_Captcha()
        {

            //string groupName = "Stu's Crew";
            string groupName = "**work";
            string fullName = "FellowshipOne AutomatedTester01";

            // Navigate to Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("QAEUNLX0C2");

            //Create Prospect
            test.Infellowship.Groups_Prospect_ExpressInterest_WebDriver(_groupName, "FellowshipOne", "AutomatedTester01", "f1automatedtester01@gmail.com", null, Guid.NewGuid().ToString().Substring(0, 6));

        }

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Verifies Missing All Information")]
        public void ProspectWorkflow_ExpressInterest_Invalid_Information_All_Missing()
        {

            string groupName = "Stu's Crew";
            string fullName = "FellowshipOne AutomatedTester01";

            // Navigate to Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("QAEUNLX0C2");

            //Create Prospect
            test.Infellowship.Groups_Prospect_ExpressInterest_WebDriver(_groupName, null, null, null, null, Guid.NewGuid().ToString().Substring(0, 6));

        }

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Verifies Reload Captcha")]
        public void ProspectWorkflow_ExpressInterest_Reload_Captcha()
        {

            string groupName = "A Test Group";
            string fullName = "FellowshipOne AutomatedTester01";

            // Navigate to Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("dc");

            //Verify Reload of Captcha
            test.Infellowship.Groups_Prospect_ExpressInterest_Captcha_Reload_WebDriver(groupName);

        }


        #endregion Captcha

        [Test, RepeatOnFailure, Timeout(120000)]
        [Author("Stuart Platt")]
        [Description("Verifies that prospects expire after 30 days")]
        public void ProspectWorkflow_Expire_Prospects()
        {
            // Navigate to Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("dc");
            string groupName = "Stu's Crew";
            string fullName = "FellowshipOne AutomatedTester01";

            //Make individual note
            string noteText = string.Format("I would like to join this group - {0}", Guid.NewGuid().ToString().Substring(0, 5));
            //string noteText = string.Format("I would like to join this group -{0}", "8650");
            TestLog.WriteLine(noteText);

            //Create Prospect
            test.Infellowship.Groups_Prospect_ExpressInterest_WebDriver(groupName, "FellowshipOne", "AutomatedTester01", "f1automatedtester01@gmail.com", noteText, null, "75038");

            try {
                //Verify Prospect Created
                test.Portal.LoginWebDriver();

                test.Portal.Groups_Group_View_Prospects_WebDriver(groupName);
                //find table with interested prospect and name
                Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_Group_ActiveProspects, fullName, "Name"), "The individual is not found in the active prospects");
            
                //Set Prospect to expired
                string expiredDate = test.SQL.Groups_Prospect_Expire_Created_Date(15, noteText);

                //Verify Expired
                test.Driver.FindElementById("open_count").Click();

                //If table exists verify individual not present, else make sure it's empty and message is present
                if (test.GeneralMethods.IsElementPresentWebDriver(By.XPath(TableIds.Groups_Group_ActiveProspects)))
                {
                    Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_Group_ActiveProspects, fullName, "Name"), "The individual is found in the active prospects");
                }
                else
                {
                    test.GeneralMethods.VerifyTextPresentWebDriver("There are no prospects matching your search criteria");
                }

                test.Driver.FindElementById("closed_count").Click();
                int row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Groups_Group_ClosedProspects, string.Format("Today at {0}", expiredDate), "Closed", "contains");
                //Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_Group_ClosedProspects, fullName, "Name"));
                //Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_Group_ClosedProspects, "Expired", "Status"), "The individual is not set to expired");
                Assert.AreEqual(fullName, test.Driver.FindElementByXPath(TableIds.Groups_Group_ClosedProspects).FindElements(By.TagName("tr"))[row].FindElements(By.TagName("td"))[1].Text, "Individual not found");
                Assert.AreEqual("Expired", test.Driver.FindElementByXPath(TableIds.Groups_Group_ClosedProspects).FindElements(By.TagName("tr"))[row].FindElements(By.TagName("td"))[3].Text, "The individual is not set to expired");

            } finally {
                //Set Prospect to expired
                test.SQL.Groups_Prospect_Expire_Created_Date(15, noteText);
                //Logout
                test.Portal.LogoutWebDriver();
        }
            
        }

        #endregion Express Interest

        #region Communication Tasks
        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a leader can leave a comment on a prospect.")]
        public void ProspectWorkflow_Communication_Leave_Comment_Leader()
        {
            // Set initial conditions
            int churchId = 254;
            string groupName = _groupName;
            string firstName = "Comment";
            string lastName = "Prospect";
            string email = "infellowship.group.tests@gmail.com";
            base.SQL.People_DeleteProspectInfo(churchId, groupName, firstName, lastName, email);

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            // Express interest to a group
            test.Infellowship.Groups_Prospect_ExpressInterest_WebDriver(groupName, firstName, lastName, email);

            // Login to infellowship
            test.Infellowship.LoginWebDriver("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the group and its prospect
            test.Infellowship.Groups_Group_View_Prospects_WebDriver(groupName);

            // Leave a comment 
            test.Driver.FindElementByLinkText("Comment Prospect").Click();
            test.Driver.FindElementByLinkText(InFellowshipConstants.Groups.GroupsConstants.IndividualProspectPage.Link_MakeAComment_WD).Click();
            test.GeneralMethods.WaitForElementDisplayed(By.Id(InFellowshipConstants.Groups.GroupsConstants.IndividualProspectPage.TextField_Comment), 30, "Text Field Comment not displayed");
            test.Driver.FindElementById(InFellowshipConstants.Groups.GroupsConstants.IndividualProspectPage.TextField_Comment).SendKeys("This prospect has a comment now!");
            test.Driver.FindElementById(InFellowshipConstants.Groups.GroupsConstants.IndividualProspectPage.Button_SaveComment).Click();

            // View the prospect
            test.Infellowship.Groups_Group_View_Prospects_WebDriver(groupName);
            test.Driver.FindElementByLinkText("Comment Prospect").Click();

            // Verify the comment is there
            Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver("This prospect has a comment now!"));

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a leader can leave a 'Meet face to face' note on a prospect.")]
        public void ProspectWorkflow_Communication_Leave_Meeting_Note_Leader()
        {
            // Set initial conditions
            int churchId = 254;
            string groupName = _groupName;
            string firstName = "Meeting";
            string lastName = "Prospect";
            string email = "infellowship.group.tests@gmail.com";
            base.SQL.People_DeleteProspectInfo(churchId, groupName, firstName, lastName, email);

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            // Express interest to a group
            test.Infellowship.Groups_Prospect_ExpressInterest_WebDriver(groupName, firstName, lastName, email);

            // Login to infellowship
            test.Infellowship.LoginWebDriver("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the group and its prospect
            test.Infellowship.Groups_Group_View_Prospects_WebDriver(groupName);

            // Meet face to face
            test.Driver.FindElementByLinkText("Meeting Prospect").Click();
            test.Driver.FindElementByLinkText(InFellowshipConstants.Groups.GroupsConstants.IndividualProspectPage.Link_MeetFaceToFace_WD).Click();
            test.GeneralMethods.WaitForElementDisplayed(By.Id(InFellowshipConstants.Groups.GroupsConstants.IndividualProspectPage.TextField_MeetingInfo), 30, "Text Field Meeting Info not displayed");
            test.Driver.FindElementById(InFellowshipConstants.Groups.GroupsConstants.IndividualProspectPage.TextField_MeetingInfo).SendKeys("This prospect was met!");
            test.Driver.FindElementById(InFellowshipConstants.Groups.GroupsConstants.IndividualProspectPage.Button_SaveMeetin).Click();

            // View the prospect
            test.Infellowship.Groups_Group_View_Prospects_WebDriver(groupName);
            test.Driver.FindElementByLinkText("Meeting Prospect").Click();

            Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver("This prospect was met!"));

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a leader can leave a 'Phone Call' note on a prospect.")]
        public void ProspectWorkflow_Communication_Leave_Phone_Call_Note_Leader()
        {
            // Set initial conditions
            int churchId = 254;
            string groupName = _groupName;
            string firstName = "Phone";
            string lastName = "Prospect";
            string email = "infellowship.group.tests@gmail.com";
            base.SQL.People_DeleteProspectInfo(churchId, groupName, firstName, lastName, email);

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            // Express interest to a group
            test.Infellowship.Groups_Prospect_ExpressInterest_WebDriver(groupName, firstName, lastName, email);

            // Login to infellowship
            test.Infellowship.LoginWebDriver("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the group and its prospect
            test.Infellowship.Groups_Group_View_Prospects_WebDriver(groupName);

            // Make a phone call 
            test.Driver.FindElementByLinkText("Phone Prospect").Click();
            test.Driver.FindElementByLinkText(InFellowshipConstants.Groups.GroupsConstants.IndividualProspectPage.Link_MakeAPhoneCall_WD).Click();
            test.Driver.FindElementByLinkText(InFellowshipConstants.Groups.GroupsConstants.IndividualProspectPage.Link_PhoneAdd_WD).Click();
            test.Driver.FindElementById(InFellowshipConstants.Groups.GroupsConstants.IndividualProspectPage.TextField_PhoneNumber).SendKeys("214-532-7112");
            new SelectElement(test.Driver.FindElementById(InFellowshipConstants.Groups.GroupsConstants.IndividualProspectPage.DropDown_PhoneStatus)).SelectByText("Left a message");
            test.Driver.FindElementById(InFellowshipConstants.Groups.GroupsConstants.IndividualProspectPage.TextField_PhoneMessage).SendKeys("This prospect was called!");
            test.Driver.FindElementById(InFellowshipConstants.Groups.GroupsConstants.IndividualProspectPage.Button_SavePhone).Click();

            // View the prospect
            test.Infellowship.Groups_Group_View_Prospects_WebDriver(groupName);
            test.Driver.FindElementByLinkText("Phone Prospect").Click();
            Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver("This prospect was called!"));

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a leader can send an to a prospect.")]
        public void ProspectWorkflow_Communication_SendEmail_Leader()
        {
            // Set initial conditions
            int churchId = 254;
            string groupName = _groupName;
            string firstName = "Email";
            string lastName = "Prospect";
            string email = "infellowship.group.tests@gmail.com";
            base.SQL.People_DeleteProspectInfo(churchId, groupName, firstName, lastName, email);

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            // Express interest 
            test.Infellowship.Groups_Prospect_ExpressInterest_WebDriver(groupName, firstName, lastName, email);

            // Login to infellowship
            test.Infellowship.LoginWebDriver("group.leader.ft@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the group and its prospect
            test.Infellowship.Groups_Group_View_Prospects_WebDriver(groupName);

            // Send an email
            test.Driver.FindElementByLinkText("Email Prospect").Click();
            test.Driver.FindElementByLinkText(InFellowshipConstants.Groups.GroupsConstants.IndividualProspectPage.Link_SendAnEmail_WD).Click();
            test.Driver.FindElementById(InFellowshipConstants.Groups.GroupsConstants.IndividualProspectPage.TextField_Email).SendKeys("I am sending an email to a prospect");
            test.Driver.FindElementById(InFellowshipConstants.Groups.GroupsConstants.IndividualProspectPage.Button_SendEmail).Click();

            // View the prospect
            test.Infellowship.Groups_Group_View_Prospects_WebDriver(groupName);
            test.Driver.FindElementByLinkText("Email Prospect").Click();
            Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver("I am sending an email to a prospect"));

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();
        }
        #endregion Communication Tasks

    }
    
    [TestFixture, DoesNotRunInJenkins]
    public class infellowship_EventRegistration_Email_WebDriver : FixtureBaseWebDriver
        {
        // declaring fromId collection
        IList<int> _formId = new List<int>();


        public void FixtureSetUp()
        {

        }

        [FixtureTearDown]
        public void FixtureTearDown()
        {

            TestLog.WriteLine("Running Fixture Tear Down......");

            for (int i = 0; i < _formId.Count; i++)
            {
                TestLog.WriteLine("-formId = {0}", _formId[i]);
                base.SQL.Weblink_Form_Delete_Proc(15, _formId[i]);
            }


        }
        #region Confirmation message
        [Test, RepeatOnFailure, Timeout(120000)]
        [Author("Suchitra Patnam")]
        [Description("F1-4201 -Email Confirmation Message")]
        public void InfellowshipRegistration_Verify_EmailConfirmationMessage()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions
            string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
            string formName = string.Format("Test Form - Pricing{0}", guidRandom);
            double price = 0.0;
            string fund = "1 - General Fund (Contribution)";
            string startDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Date.ToShortDateString();
            string endDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(2).Date.ToShortDateString();
            string emailSender = "fellowshiponemail@activenetwork.com";
            string emailMessage = "This is Email Text";
            string emailReceipient = "Ft.autotester@gmail.com";
            string emailSubject = "Registration Confirmation";
            string[] sendEmail = { "F1AutomatedTester01@gmail.com" };
            string[] emailCC = { "Ft.autotester@gmail.com" };
            string password = "ActiveQA12";
            string passwordCC = "Romans10:9";
            string mailbox = EmailMailBox.GMAIL.INBOX;

            string[] individual = { "Automated Tester", "Qa Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }


            //Login
            test.Portal.LoginWebDriver();


            //Create New Form
            test.Portal.WebLink_FormNames_Create(formName, true, true);

            //Get form ID from database
            int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

            //adding form id to global collection to delete
            _formId.Add(formId);

            //Create Custom Email message

            test.Portal.EventRegistration_Create_CustomConfirmationMessage(emailMessage, emailSender);


            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify  Form Name 

            Assert.AreEqual(formName, test.Driver.FindElementById(GeneralInFellowship.EventRegistration.FormName).Text);


            //Clicking on 'continue' button from step2
            test.Infellowship.EventRegistration_ClickContinue();

            //Verify Step3 Summary details..
            test.Infellowship.EventRegistration_Step3_Summary(formName, individual, selectedOrder, price, sendEmail, emailCC);

            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Get confirmation mesasge
            string url = test.Driver.Url;
            string confirmcode = test.Infellowship.GetConfirmationCode(url, 15);
            TestLog.WriteLine("Confirm Code = {0}", confirmcode);


            //Verify  email was sent

            for (int i = 0; i < sendEmail.Length; i++)
            {
                Message grpMsg = test.Portal.Retrieve_Search_Weblink_Confirmation_Email(sendEmail[i], password, confirmcode);
                TestLog.WriteLine("grpMSg = {0}", grpMsg);
                test.Portal.Verify_EventRegistration_Confirmation_Email(grpMsg, emailSubject, emailSender, sendEmail[i], sendEmail, emailMessage, confirmcode, individual);

                //Delete Email from inbox
                TestLog.WriteLine("Deleting Email after verififcation");
                test.Portal.Delete_Email(grpMsg, sendEmail[i], password, mailbox);

            }


            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();
        }
        
        //Multiple REgistrants and Email confirmation

        [Test, RepeatOnFailure, Timeout(120000)]
        [Author("Suchitra Patnam")]
        [Description("F1-4224 - Multiple Registrants")]
        public void InfellowshipRegistration_Verify_EmailConfirmationMessage_MultipleIndividuals()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions
            string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
            string formName = string.Format("Test Form - Pricing{0}", guidRandom);
            double price = 0.0;
            string fund = "1 - General Fund (Contribution)";
            string startDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Date.ToShortDateString();
            string endDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(2).Date.ToShortDateString();
            string emailSender = "fellowshiponemail@activenetwork.com";
            string emailMessage = "This is Email Text";
            string emailReceipient = "Ft.autotester@gmail.com";
            string emailSubject = "Registration Confirmation";
            string[] sendEmail = { "F1AutomatedTester01@gmail.com" };
            string[] emailCC = { "Ft.autotester@gmail.com" };
            string password = "ActiveQA12";
            string passwordCC = "FT4life!";
            string mailbox = EmailMailBox.GMAIL.INBOX;

            string[] individual = { "Automated Tester", "Qa Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }


            //Login
            test.Portal.LoginWebDriver();


            //Create New Form
            test.Portal.WebLink_FormNames_Create(formName, true, true);

            //Get form ID from database
            int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

            //adding form id to global collection to delete
            _formId.Add(formId);

            //Create Custom Email message

            test.Portal.EventRegistration_Create_CustomConfirmationMessage(emailMessage, emailSender);


            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, true, true);

            //Verify  Form Name 

            Assert.AreEqual(formName, test.Driver.FindElementById(GeneralInFellowship.EventRegistration.FormName).Text);


            //Clicking on 'continue' button from step2
            test.Infellowship.EventRegistration_ClickContinue();

            //Verify Step3 Summary details..
            test.Infellowship.EventRegistration_Step3_Summary(formName, individual, selectedOrder, price, sendEmail, emailCC);

            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Get confirmation mesasge
            string url = test.Driver.Url;
            string confirmcode = test.Infellowship.GetConfirmationCode(url, 15);
            TestLog.WriteLine("Confirm Code = {0}", confirmcode);


            //Verify  email was sent

            for (int i = 0; i < sendEmail.Length; i++)
            {
                Message grpMsg = test.Portal.Retrieve_Search_Weblink_Confirmation_Email(sendEmail[i], password, confirmcode);
                TestLog.WriteLine("grpMSg = {0}", grpMsg);
                test.Portal.Verify_EventRegistration_Confirmation_Email(grpMsg, emailSubject, emailSender, sendEmail[i], sendEmail, emailMessage, confirmcode, individual);

                //Delete Email from inbox
                TestLog.WriteLine("Deleting Email after verififcation");
                test.Portal.Delete_Email(grpMsg, sendEmail[i], password, mailbox);


            }


            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();
        }
        #endregion Confirmation MEssage

        #region Payment-EmailReceipt

        //Commneting this test case until Payments are ready to be tested
        //[Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_Submission_EmailReceipt()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test From - Email Receipt";
            double price = 5.00;

            string[] individual = { "Automated Tester" };

            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Verify Form Name and Confirmation message created are displayed in confirmation page
            test.GeneralMethods.VerifyTextPresentWebDriver(formName);

            //Get confirmation mesasge
            string url = test.Driver.Url;
            string confirmcode = test.Infellowship.GetConfirmationCode(url, 15);
            TestLog.WriteLine("Confirm Code = {0}", confirmcode);


            //Login in to Portal
            test.Portal.LoginWebDriver();

            //Verify confirmation number in email receipt in Portal
            test.Portal.Verify_EventRegistration_Confirmation_Email_Receipt(confirmcode, price, individual.Length);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }

        #endregion Payment-EmailReceipt
        }
 
    [TestFixture, DoesNotRunInJenkins]
    public class infellowship_EventRegistration_Responsive_Email_WebDriver : FixtureBaseWebDriver
        {
            // declaring fromId collection
            IList<int> _formId = new List<int>();


            public void FixtureSetUp()
            {

            }

            [FixtureTearDown]
            public void FixtureTearDown()
            {

                TestLog.WriteLine("Running Fixture Tear Down......");

                for (int i = 0; i < _formId.Count; i++)
                {
                    TestLog.WriteLine("-formId = {0}", _formId[i]);
                    base.SQL.Weblink_Form_Delete_Proc(15, _formId[i]);
                }


            }

            #region Responsive-Infellowship

            [Test, RepeatOnFailure, Timeout(120000)]
            [Author("Suchitra Patnam")]
            [Description("F1-4224 - Multiple Registrants")]
            public void InfellowshipRegistration_Verify_EmailConfirmationMessage_MultipleIndividuals_Responsive_MOBILE()
            {
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


                // Set initial conditions

                string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
                string formName = string.Format("Test Form - Pricing{0}", guidRandom);
                double price = 0.0;
                string fund = "1 - General Fund (Contribution)";
                string startDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Date.ToShortDateString();
                string endDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(2).Date.ToShortDateString();
                string emailSender = "fellowshiponemail@activenetwork.com";
                string emailMessage = "This is Email Text";
                string emailReceipient = "Ft.autotester@gmail.com";
                string emailSubject = "Registration Confirmation";
                string[] sendEmail = { "F1AutomatedTester01@gmail.com" };
                string[] emailCC = { "Ft.autotester@gmail.com" };
                string password = "ActiveQA12";
                string passwordCC = "Romans10:9";
                string mailbox = EmailMailBox.GMAIL.INBOX;
                string[] individual = { "Automated Tester", "Qa Tester" };
                IList<string> selectedOrder = new List<string>();

                //Adding individuals to selected order
                for (int i = 0; i < individual.Length; i++)
                {
                    selectedOrder.Add(individual[i]);

                }


                //Login
                test.Portal.LoginWebDriver();


                //Create New Form
                test.Portal.WebLink_FormNames_Create(formName, true, true);

                //Get form ID from database
                int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

                //adding form id to global collection to delete
                _formId.Add(formId);

                //Create Custom Email message

                test.Portal.EventRegistration_Create_CustomConfirmationMessage(emailMessage, emailSender);

                //Setting Window size to mobile size
                test.Infellowship.SetBrowserSizeTo_Mobile();

                //Login to Event Registration Form
                test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

                //select individual
                test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, true, true);

                //Verify  Form Name 

                Assert.AreEqual(formName, test.Driver.FindElementById(GeneralInFellowship.EventRegistration.FormName).Text);


                //Clicking on 'continue' button from step2
                test.Infellowship.EventRegistration_ClickContinue();

                //Verify Step3 Summary details..
                test.Infellowship.EventRegistration_Step3_Summary(formName, individual, selectedOrder, price, sendEmail, emailCC);

                //Submitting details from Step3..
                test.Infellowship.EventRegistration_ClickSubmitPayment();

                //Get confirmation mesasge
                string url = test.Driver.Url;
                string confirmcode = test.Infellowship.GetConfirmationCode(url, 15);
                TestLog.WriteLine("Confirm Code = {0}", confirmcode);


                //Verify  email was sent

                for (int i = 0; i < sendEmail.Length; i++)
                {
                    Message grpMsg = test.Portal.Retrieve_Search_Weblink_Confirmation_Email(sendEmail[i], password, confirmcode);
                    TestLog.WriteLine("grpMSg = {0}", grpMsg);
                    test.Portal.Verify_EventRegistration_Confirmation_Email(grpMsg, emailSubject, emailSender, sendEmail[i], sendEmail, emailMessage, confirmcode, individual);

                    //Delete Email from inbox
                    TestLog.WriteLine("Deleting Email after verififcation");
                    test.Portal.Delete_Email(grpMsg, sendEmail[i], password, mailbox);

                }


                //Delete Email from inbox
                // test.Portal.Delete_Email(grpMsg, emailReceipient, password, mailbox);


                //Delete Form using Store proc method
                //test.SQL.Weblink_Form_Delete_Proc(15, formId);

                //Closing the browser for now since we don't have signout link for this new form Url
                test.Driver.Quit();
            }

            #endregion Responsive-Infellowship
        } 
    // End of Responsive Event Registration fixture

    [TestFixture, DoesNotRunInJenkins]
    [Category(TestCategories.Services.PaymentProcessor)]
    public class infellowship_Giving_GiveNow_WithoutAccount_Captcha_WebDriver : FixtureBaseWebDriver
    {
        #region Private Members
        public struct Contribution
        {
            public string fund;
            public string subFund;
            public string amount;
            public Contribution(string fundOrPledgeDrive, string SubFund, string Amount)
            {
                fund = fundOrPledgeDrive;
                subFund = SubFund;
                amount = Amount;
            }
        }

        #endregion Private Members

        #region Responsive

        #region Give Now Without Account

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies an individual can give now by VISA credit card without creating an account and choose to not create an account in a Responsive mode.")]
        public void GiveNow_WithoutAccount_CreditCard_Success_Visa_NoCreateAccount_RESPONSIVE_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "31.37";
            string firstName = "GW";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Set window size to simulate iPhone
            test.Infellowship.SetBrowserSizeTo_Mobile();

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies an individual can add/remove additional fund/sub funds from give now without account in Responsive mode.")]
        public void GiveNow_WithoutAccount_CreditCard_Success_MultiFundAndSubFund_RESPONSIVE_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund1 = "Auto Test Fund w Sub Fund";
            string subFund1 = "Auto Test Sub Fund";
            string fund2 = "Auto Test Fund";
            string amount1 = "311.37";
            string amount2 = "312.37";
            string firstName = "GW";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Set up contribution data
            var contributionData = new List<dynamic> {
                new Contribution { fund = fund1, subFund = subFund1, amount = amount1 },
                new Contribution { fund = fund2, subFund = null, amount = amount2 }
            };

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Set window size to simulate iPhone
            test.Infellowship.SetBrowserSizeTo_Mobile();

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, contributionData, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty, country, address1,
                string.Empty, city, state, zipCode, string.Empty, false);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies an individual can give now by personal check without creating an account and choose to not create an account in Responsive mode.")]
        public void GiveNow_WithoutAccount_PersonalCheck_Success_NoCreateAccount_RESPONSIVE_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "34.37";
            string firstName = "GW";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string phoneNumber = "8172743453";
            string routingNumber = "111000025";
            string accountNumber = "111222333";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Set window size to simulate iPhone
            test.Infellowship.SetBrowserSizeTo_Mobile();

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_PersonalCheck(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Personal_check, phoneNumber, routingNumber, accountNumber, false);

        }

        #endregion Give Now Without Account

        #endregion Responsive

        #region Give Now Without Account        

        #region Success
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies an individual can give now by VISA credit card without creating an account and choose to not create an account.")]
        public void GiveNow_WithoutAccount_CreditCard_Success_Visa_NoCreateAccount_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "1.37";
            string firstName = "GW";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false);

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies a new individual database property who is created during Giving Now without account")]
        public void GiveNow_WithoutAccount_CreditCard_NewIndividual_CreateDate_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund1 = "Auto Test Fund w Sub Fund";
            string subFund1 = "Auto Test Sub Fund";
            string amount1 = "22.33";
            string firstName = "NewIndividual";
            string lastName = "FromGiving";
            string emailAddress = "NewIndividual.FromGiving@gmail.com";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";

            // Set up contribution data
            var contributionData = new List<dynamic> 
            {
                new Contribution { fund = fund1, subFund = subFund1, amount = amount1 }
            };

            #endregion Data

            base.SQL.People_Individual_Delete(15, firstName, lastName);

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            try
            {
                // Use the Give Now link to make a contribution without creating an account
                test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund1, subFund1, amount1, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                    country, address1, string.Empty, city, state, zipCode, string.Empty, false);

                int individualId = test.SQL.People_Individuals_FetchID(churchId, firstName, lastName);
                string dateFound = test.SQL.People_Individual_FetchColumnValue(churchId, individualId, "CREATED_DATE");
                TestLog.WriteLine("Created date value found from DB is: " + dateFound);
                Assert.IsTrue(dateFound.Contains(DateTime.Now.Year.ToString()), "The created date value doesn't contain current year");
            }
            finally
            {
                base.SQL.People_Individual_Delete(15, firstName, lastName);
            }

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies an individual can give now by VISA credit card without creating an account and choose to create an account.")]
        public void GiveNow_WithoutAccount_CreditCard_Success_Visa_CreateAccount_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "1.38";
            string firstName = "GW";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, true);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies an individual can give now by Mastercard credit card without creating an account and choose to not create an account.")]
        public void GiveNow_WithoutAccount_CreditCard_Success_MasterCard_NoCreateAccount_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "2.37";
            string firstName = "GW";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Master Card";
            string cardNumber = "5555555555554444";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies an individual can give now by Mastercard credit card without creating an account and choose to create an account.")]
        public void GiveNow_WithoutAccount_CreditCard_Success_MasterCard_CreateAccount_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "2.38";
            string firstName = "GW";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Master Card";
            string cardNumber = "5555555555554444";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, true);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies an individual can give now by AMEX credit card without creating an account and choose to not create an account.")]
        public void GiveNow_WithoutAccount_CreditCard_Success_AMEX_NoCreateAccount_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "3.37";
            string firstName = "GW";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "American Express";
            string cardNumber = "378282246310005";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies an individual can give now by AMEX credit card without creating an account and choose to create an account.")]
        public void GiveNow_WithoutAccount_CreditCard_Success_AMEX_CreateAccount_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "3.38";
            string firstName = "GW";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "American Express";
            string cardNumber = "378282246310005";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, true);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies an individual can give now by Discover credit card without creating an account and choose to not create an account.")]
        public void GiveNow_WithoutAccount_CreditCard_Success_Discover_NoCreateAccount_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "4.37";
            string firstName = "GW";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Discover";
            string cardNumber = "6011111111111117";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies an individual can give now by Discover credit card without creating an account and choose to create an account.")]
        public void GiveNow_WithoutAccount_CreditCard_Success_Discover_CreateAccount_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "4.38";
            string firstName = "GW";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Discover";
            string cardNumber = "6011111111111117";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, true);

        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies an individual can give now by JCB credit card without creating an account and choose to not create an account.")]
        public void GiveNow_WithoutAccount_CreditCard_Success_JCB_NoCreateAccount_WebDriver()
        {
            // Data
            #region Data
            int churchId = 258;
            string fund = "Auto Test Fund";
            string amount = "5.37";
            string firstName = "GW";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "JCB";
            string cardNumber = "3566111111111113";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United Kingdom";
            string address1 = "63 St Leonards Road ";
            string city = "Windsor";
            string state = " Berkshire";
            string zipCode = "SL4 3BX";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("qaeunlx0c6");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies an individual can give now by JCB credit card without creating an account and choose to create an account.")]
        public void GiveNow_WithoutAccount_CreditCard_Success_JCB_CreateAccount_WebDriver()
        {
            // Data
            #region Data
            int churchId = 258;
            string fund = "Auto Test Fund";
            string amount = "5.38";
            string firstName = "GW";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "JCB";
            string cardNumber = "3566111111111113";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United Kingdom";
            string address1 = "63 St Leonards Road ";
            string city = "Windsor";
            string state = " Berkshire";
            string zipCode = "SL4 3BX";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("qaeunlx0c6");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, true);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies an individual can add/remove additional fund/sub funds from give now without account.")]
        public void GiveNow_WithoutAccount_CreditCard_Success_MultiFundAndSubFund_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund1 = "Auto Test Fund w Sub Fund";
            string subFund1 = "Auto Test Sub Fund";
            string fund2 = "Auto Test Fund";
            string amount1 = "11.37";
            string amount2 = "12.37";
            string firstName = "GW";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Set up contribution data
            var contributionData = new List<dynamic> {
                new Contribution { fund = fund1, subFund = subFund1, amount = amount1 },
                new Contribution { fund = fund2, subFund = null, amount = amount2 }
            };

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, contributionData, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty, country, address1,
                string.Empty, city, state, zipCode, string.Empty, false);

        }

        #region Validation
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies an individual can give now by personal check without creating an account and choose to not create an account.")]
        public void GiveNow_WithoutAccount_PersonalCheck_Validation_InvalidCaptcha_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "4.37";
            string firstName = "GW";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string phoneNumber = "8172743453";
            string routingNumber = "111000025";
            string accountNumber = "111222333";
            string captcha = "9Invalid9";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_PersonalCheck(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Personal_check, phoneNumber, routingNumber, accountNumber, false, captcha);

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies invalid captcha error message")]
        public void GiveNow_WithoutAccount_CreditCard_Validation_InvalidCaptcha_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "1.79";
            string firstName = "FT";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            string captcha = "Invalid";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false, captcha);
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies invalid general error message")]
        public void GiveNow_WithoutAccount_CreditCard_Validation_GeneralError_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "1200.00";
            string firstName = "FT";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            string captcha = "Invalid";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false, captcha, true);
        }


        #endregion Validation

        #region Individual Scenarios
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("F1-4373: Verifies when a brand new individual gives without an account their address is retained in portal")]
        public void GiveNow_WithoutAccount_CreditCard_Success_NewIndividual_AddressRetained()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "1.37";
            string firstName = "GW-Address";
            string lastName = "Tester";
            string emailAddress = "ft.gwaddresstester@gmail.com";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false);

            // Login to Portal
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            // Search for the individual
            test.Portal.People_FindAPerson_Individual_WebDriver(string.Format("{0} {1}", firstName, lastName), string.Empty, string.Empty, false, string.Empty, string.Empty, string.Empty, string.Empty, false);

            // Click on the individual from search
            test.Driver.FindElementByLinkText(string.Format("{0} {1}", firstName, lastName)).Click();

            // Verify address appears
            Assert.Contains(test.Driver.FindElementByXPath("//div[@class='adr']").Text, address1);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Try to clean up the individual
            test.SQL.People_MergeIndividualBulk(15, string.Format("{0} {1}", firstName, lastName), "Merge Dump");

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-4373: Verifies when an existing individual gives without an account with no address a duplicate is not created but address is retained")]
        public void GiveNow_WithoutAccount_CreditCard_Success_ExistingIndividual_NoInfellowshipAccount_NoDupe_AddressRetained()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "1.77";
            string firstName = "GW-Existing1";
            string lastName = "Tester";
            string emailAddress = "ft.gwexistingtester@gmail.com";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Try to clean up the individual
            base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", firstName, lastName), "Merge Dump");

            // Create the individual in Portal
            base.SQL.People_Individual_Create(15, firstName, lastName);

            try
            {

                base.SQL.People_CommunicationValue_Add(15, string.Format("{0} {1}", firstName, lastName), GeneralEnumerations.CommunicationTypes.Personal, emailAddress, true);

                // Launch Infellowship
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Infellowship.OpenWebDriver("DC");

                // Use the Give Now link to make a contribution without creating an account
                test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                    country, address1, string.Empty, city, state, zipCode, string.Empty, false);

                // Login to Portal
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

                // Search for the individual
                test.Portal.People_FindAPerson_Individual_WebDriver(string.Format("{0} {1}", firstName, lastName), string.Empty, string.Empty, false, string.Empty, string.Empty, string.Empty, string.Empty, false);

                // Verify only one individual returns in search results
                test.GeneralMethods.VerifyOnlyOneIndividualInSearch(firstName, lastName);

                // Click on the individual from search
                test.Driver.FindElementByLinkText(string.Format("{0} {1}", firstName, lastName)).Click();

                // Verify address appears
                Assert.Contains(test.Driver.FindElementByXPath("//div[@class='adr']").Text, address1);

                // Logout of Portal
                test.Portal.LogoutWebDriver();

            }
            finally
            {
                // Try to clean up the individual
                base.SQL.People_MergeIndividualBulk(15, string.Format("{0} {1}", firstName, lastName), "Merge Dump");
            }
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-4373: Verifies when an individual with an Infellowship account and no home address gives without an account they are not able to create a new account, a duplicate is not created, and address is retained")]
        public void GiveNow_WithoutAccount_CreditCard_Success_ExistingIndividual_WithInfellowshipAccount_NoDupe_AddressRetained()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "1.87";
            string firstName = "GW-Infellowship1";
            string lastName = "Tester";
            string emailAddress = "ft.gwinfellowshiptester@gmail.com";
            string password = "FT4life!";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Try to clean up the individual
            // base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", firstName, lastName), "Merge Dump");
            base.SQL.People_Individual_DeleteBulk(15, firstName, lastName);

            // Create the individual in Portal
            base.SQL.People_Individual_Create(15, firstName, lastName);
            try
            {
                base.SQL.People_CommunicationValue_Add(15, string.Format("{0} {1}", firstName, lastName), GeneralEnumerations.CommunicationTypes.Personal, emailAddress, true);
                base.SQL.People_CommunicationValue_Add(15, string.Format("{0} {1}", firstName, lastName), GeneralEnumerations.CommunicationTypes.InFellowship, emailAddress, true);
                base.SQL.People_InFellowshipAccount_Create(15, string.Format("{0} {1}", firstName, lastName), emailAddress, password);

                // Launch Infellowship
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Infellowship.OpenWebDriver("DC");

                // Use the Give Now link to make a contribution without creating an account
                test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                    country, address1, string.Empty, city, state, zipCode, string.Empty, false);

                // Login to Portal
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

                // Search for the individual
                test.Portal.People_FindAPerson_Individual_WebDriver(string.Format("{0} {1}", firstName, lastName), string.Empty, string.Empty, false, string.Empty, string.Empty, string.Empty, string.Empty, false);

                // Verify only one individual returns in search results
                test.GeneralMethods.VerifyOnlyOneIndividualInSearch(firstName, lastName);

                // Click on the individual from search
                test.Driver.FindElementByLinkText(string.Format("{0} {1}", firstName, lastName)).Click();

                // Verify address appears
                Assert.Contains(test.Driver.FindElementByXPath("//div[@class='adr']").Text, address1);

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            finally
            {
                // Try to clean up the individual
                // base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", firstName, lastName), "Merge Dump");
                base.SQL.People_Individual_DeleteBulk(15, firstName, lastName);
            }

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-4373: Verifies when an individual with an Infellowship account and an existing home address gives without an account they are not able to create a new account, a duplicate is not created, and address is not retained")]
        public void GiveNow_WithoutAccount_CreditCard_Success_ExistingIndividual_WithInfellowshipAccount_NoDupe_AddressNotRetained()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "2.87";
            string firstName = "GW-InfNoAddRetain1";
            string lastName = "Tester";
            string emailAddress = "ft.gwinfnoaddretaintester@gmail.com";
            string password = "FT4life!";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Try to clean up the individual
            base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", firstName, lastName), "Merge Dump");

            // Create the individual in Portal
            base.SQL.People_Individual_Create(15, firstName, lastName);
            try
            {

                base.SQL.People_CommunicationValue_Add(15, string.Format("{0} {1}", firstName, lastName), GeneralEnumerations.CommunicationTypes.Personal, emailAddress, true);
                base.SQL.People_CommunicationValue_Add(15, string.Format("{0} {1}", firstName, lastName), GeneralEnumerations.CommunicationTypes.InFellowship, emailAddress, true);
                base.SQL.People_InFellowshipAccount_Create(15, string.Format("{0} {1}", firstName, lastName), emailAddress, password);
                base.SQL.People_Addresses_Create(15, string.Format("{0} {1}", firstName, lastName), "37 Your Mom Trl.", "Fort Worth", "TX", "76120", "United States");

                // Launch Infellowship
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Infellowship.OpenWebDriver("DC");

                // Use the Give Now link to make a contribution without creating an account
                test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                    country, address1, string.Empty, city, state, zipCode, string.Empty, false);

                // Login to Portal
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

                // Search for the individual
                test.Portal.People_FindAPerson_Individual_WebDriver(string.Format("{0} {1}", firstName, lastName), string.Empty, string.Empty, false, string.Empty, string.Empty, string.Empty, string.Empty, false);

                // Verify only one individual returns in search results
                test.GeneralMethods.VerifyOnlyOneIndividualInSearch(firstName, lastName);

                // Click on the individual from search
                test.Driver.FindElementByLinkText(string.Format("{0} {1}", firstName, lastName)).Click();

                // Verify address doesn't appear
                Assert.DoesNotContain(test.Driver.FindElementByXPath("//div[@class='adr']").Text, address1);

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }
            finally
            {
                // Try to clean up the individual
                base.SQL.People_MergeIndividualBulk(15, string.Format("{0} {1}", firstName, lastName), "Merge Dump");
            }
        }

        #endregion Individual Scenarios

        #endregion Credit Card

        #endregion Give Now Without Account

    }

}