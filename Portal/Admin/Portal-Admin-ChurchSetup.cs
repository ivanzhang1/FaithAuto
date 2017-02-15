using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace FTTests.Portal.Admin {
    [TestFixture]
    public class Portal_Admin_ChurchSetupWebDriver : FixtureBaseWebDriver {
        #region Church Contacts
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create additional church contacts making the total greater than 3.")]
        public void Admin_ChurchSetup_ChurchContacts_MoreThan3() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Navigate to admin->church contacts
            test.GeneralMethods.Navigate_Portal(Navigation.Admin.Church_Setup.Church_Contacts);

            // Delete any contacts greater than 2
            test.Driver.FindElementByXPath("//a[@href='Edit.aspx']").Click();
            foreach (var deleteItem in test.Driver.FindElementsByXPath("//a[@class='delete float_left nudge_left']")) {
                deleteItem.Click();
            }
            test.Driver.FindElementById(GeneralButtons.Save).Click();

            // Add a contact so that we end up with at least 4
            test.Driver.FindElementByXPath("//a[@href='Edit.aspx']").Click();
            // test.Driver.FindElementByXPath("//a[text()='Add contact' and normalize-space(ancestor::div/preceding-sibling::h3)='Fellowship One Champion']").Click();
            test.Driver.FindElementByXPath("//h3[normalize-space(text())='Fellowship One Champion Team']/following-sibling::div[1]/p/a[text()='Add contact']").Click();

            // new SelectElement(test.Driver.FindElementByXPath("//h3[normalize-space(text())='Fellowship One Champion']/following-sibling::div[1]/div[1]/p/select")).SelectByText("Luke Jiang");
            // new SelectElement(test.Driver.FindElementByXPath("//h3[normalize-space(text())='Fellowship One Champion']/following-sibling::div[1]/div[2]/p/select")).SelectByText("Luke Jiang");
            new SelectElement(test.Driver.FindElementByXPath("//h3[normalize-space(text())='Fellowship One Champion Team']/following-sibling::div[1]/div[3]/p/select")).SelectByText("FT Tester");
            new SelectElement(test.Driver.FindElementByXPath("//h3[normalize-space(text())='Fellowship One Champion Team']/following-sibling::div[1]/div[4]/p/select")).SelectByText("FT Tester");

            // Save the contacts
            test.Driver.FindElementById(GeneralButtons.Save).Click();

            // Verify the contacts were added
            // Assert.IsTrue(test.Driver.FindElementsByXPath("//h3[normalize-space(text())='Fellowship One Champions']/following-sibling::div[1]/table/tbody/tr[3]/td/text()[self::text()[1]='Luke Jiang']/ancestor::td").Count > 0);
            // Assert.IsTrue(test.Driver.FindElementsByXPath("//h3[normalize-space(text())='Fellowship One Champions']/following-sibling::div[1]/table/tbody/tr[4]/td/text()[self::text()[1]='Luke Jiang']/ancestor::td").Count > 0);
            Assert.IsTrue(test.Driver.FindElementsByXPath("//h3[normalize-space(text())='Fellowship One Champion Team']/following-sibling::div[1]/table/tbody/tr[3]/td/text()[self::text()[1]='FT Tester']/ancestor::td").Count > 0);
            Assert.IsTrue(test.Driver.FindElementsByXPath("//h3[normalize-space(text())='Fellowship One Champion Team']/following-sibling::div[1]/table/tbody/tr[4]/td/text()[self::text()[1]='FT Tester']/ancestor::td").Count > 0);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Attempts to add Designated Support Contacts to a Core edition church.")]
        public void Admin_ChurchSetup_ChurchContacts_DesignatedSupportContacts_Core() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C1");

            // Attempt to add Designated Support Contacts to a Core edition church
            test.Portal.Admin_ChurchContacts_Add_DesignatedSupportContacts_WebDriver("Core", 0);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Attempts to add Designated Support Contacts to a Select edition church.")]
        public void Admin_ChurchSetup_ChurchContacts_DesignatedSupportContacts_Select() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C2");

            // Attempt to add Designated Support Contacts to a Core edition church
            test.Portal.Admin_ChurchContacts_Add_DesignatedSupportContacts_WebDriver("Select", 2);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Attempts to add more than the allowed number of Designated Support Contacts to a Select edition church.")]
        public void Admin_ChurchSetup_ChurchContacts_DesignatedSupportContacts_Select_MoreThan2() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C2");

            // Attempt to add Designated Support Contacts to a Core edition church
            test.Portal.Admin_ChurchContacts_Add_DesignatedSupportContacts_WebDriver("Select", 3);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Attempts to add Designated Support Contacts to a Premiere edition church.")]
        public void Admin_ChurchSetup_ChurchContacts_DesignatedSupportContacts_Premiere() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C3");

            // Attempt to add Designated Support Contacts to a Core edition church
            test.Portal.Admin_ChurchContacts_Add_DesignatedSupportContacts_WebDriver("Premiere", 4);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Attempts to add more than the allowed number of Designated Support Contacts to a Premiere edition church.")]
        public void Admin_ChurchSetup_ChurchContacts_DesignatedSupportContacts_Premiere_MoreThan4() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C3");

            // Attempt to add Designated Support Contacts to a Core edition church
            test.Portal.Admin_ChurchContacts_Add_DesignatedSupportContacts_WebDriver("Premiere", 5);

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Attempts to add an Inactive portal user as a Designated Support Contact.")]
        public void Admin_ChurchSetup_ChurchContacts_DesignatedSupportContacts_InactiveUser() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C3");

            // Attempt to add an Inactive portal user as a Designated Support Contact
            test.Portal.Admin_ChurchContacts_Add_DesignatedSupportContacts_InactiveUser("FT Inactive");

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Attempts to add an unlinked portal user as a Designated Support Contact.")]
        public void Admin_ChurchSetup_ChurchContacts_DesignatedSupportContacts_UnlinkedUser() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C3");

            // Attempt to add an Inactive portal user as a Designated Support Contact
            test.Portal.Admin_ChurchContacts_Add_DesignatedSupportContacts_UnlinkedUser("FT Unlinked");

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        #endregion Church Contacts

        #region News & Announcements
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Updates the News & Announcements.")]
        public void Admin_ChurchSetup_NewsAnnouncements_Update() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Navigate to admin->news & announcements
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.Admin.Church_Setup.News_Announcements);

            // Update the news and announcements
            string prepareText = "Preparing to update News & Announcements";
            string updateText = "Testing update to News & Announcements!!";

            test.Driver.SwitchTo().Frame("tiny_mce_editor_ifr");
            test.Driver.SwitchTo().ActiveElement();
            ((IJavaScriptExecutor)test.Driver).ExecuteScript(string.Format("document.body.innerHTML = '{0}'", prepareText));
            test.Driver.SwitchTo().DefaultContent();
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_cmdSave").Click();

            test.Driver.SwitchTo().Frame("sandboxed_iframe");
            Assert.AreEqual(prepareText, test.Driver.FindElementByXPath("//body[@marginwidth='0']").Text);
            test.Driver.SwitchTo().DefaultContent();

            test.Driver.SwitchTo().Frame("tiny_mce_editor_ifr");
            test.Driver.SwitchTo().ActiveElement();
            ((IJavaScriptExecutor)test.Driver).ExecuteScript(string.Format("document.body.innerHTML = '{0}'", updateText));
            test.Driver.SwitchTo().DefaultContent();
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_cmdSave").Click();

            // Verify the data in the preview window
            test.Driver.SwitchTo().Frame("sandboxed_iframe");
            Assert.AreEqual(updateText, test.Driver.FindElementByXPath("//body[@marginwidth='0']").Text);
            test.Driver.SwitchTo().DefaultContent();

            // Verify news & announcements on home page
            test.Driver.FindElementByLinkText("Home").Click();
            test.Driver.SwitchTo().Frame("sandboxed_iframe");
            Assert.AreEqual(updateText, test.Driver.FindElementByXPath("//body[@marginwidth='0']").Text);
            test.Driver.SwitchTo().DefaultContent();

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion News & Announcements

        #region Campuses
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to creates a Campus with no data entered.")]
        public void Admin_ChurchSetup_Campuses_CreateNoData() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Attempt to create a campus without providing a name, check for errors
            test.Portal.Admin_Campuses_Create(null);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Creates a Campus.")]
        public void Admin_ChurchSetup_Campuses_Create() {
            // Set initial conditions
            string campusName = "Test Campus";
            base.SQL.Admin_CampusDelete(15, campusName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Create a campus
            test.Portal.Admin_Campuses_Create(campusName);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Updates a Campus.")]
        public void Admin_ChurchSetup_Campuses_Update() {
            // Set initial conditions
            string campusName = "Test Campus - Update";
            string campusNameUpdated = "Test Campus - Modified";
            base.SQL.Admin_CampusDelete(15, campusName);
            base.SQL.Admin_CampusDelete(15, campusNameUpdated);
            base.SQL.Admin_CampusCreate(15, campusName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Update a campus
            test.Portal.Admin_Campuses_Update(campusName, campusNameUpdated);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Deletes a Campus.")]
        public void Admin_ChurchSetup_Campuses_Delete() {
            // Set initial conditions
            string campusName = "Test Campus - Delete";
            base.SQL.Admin_CampusDelete(15, campusName);
            base.SQL.Admin_CampusCreate(15, campusName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Delete a campus
            test.Portal.Admin_Campuses_Delete(campusName);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Campuses
    }

    [TestFixture]
    public class Portal_Admin_ChurchSetup : FixtureBase {
        #region Buildings
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create a Building with no data provided.")]
        public void Admin_ChurchSetup_Buildings_CreateNoData() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Attempt to create a building with no data
            test.Portal.Admin_Buildings_Create(null);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Creates a Building.")]
        public void Admin_ChurchSetup_Buildings_Create() {
            // Set initial conditions
            string buildingName = "Test Building";
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            base.SQL.Admin_BuildingsDelete(15, buildingName);

            // Login to Portal
            test.Portal.Login();

            // Create a building
            test.Portal.Admin_Buildings_Create(buildingName);

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Updates a Building.")]
        public void Admin_ChurchSetup_Buildings_Update() {
            // Set initial conditions
            string buildingName = "Test Building - Update";
            string buildingNameUpdated = "Test Building - Modified";
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            base.SQL.Admin_BuildingsDelete(15, buildingName);
            base.SQL.Admin_BuildingsDelete(15, buildingNameUpdated);
            base.SQL.Admin_BuildingsCreate(15, buildingName);

            // Login to Portal
            test.Portal.Login();

            // Update a building
            test.Portal.Admin_Buildings_Update(buildingName, buildingNameUpdated);

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Deletes a Building.")]
        public void Admin_ChurchSetup_Buildings_Delete() {
            // Set initial conditions
            string buildingName = "Test Building - Delete";
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            base.SQL.Admin_BuildingsDelete(15, buildingName);
            base.SQL.Admin_BuildingsCreate(15, buildingName);

            // Login to Portal
            test.Portal.Login();

            // Delete a building
            test.Portal.Admin_Buildings_Delete(buildingName);

            // Logout of Portal
            test.Portal.Logout();
        }
        #endregion Buildings

        #region Rooms
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create a Room with no data provided.")]
        public void Admin_ChurchSetup_Rooms_CreateNoData() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Attempt to create a room with no info
            test.Portal.Admin_Rooms_Create(null, null, null, null);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Creates a Room.")]
        public void Admin_ChurchSetup_Rooms_Create() {
            // Create a building
            string buildingName = "Test Building (Room Create)";
            string roomName = "Test Room";
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            base.SQL.Admin_RoomsDelete(15, roomName);
            base.SQL.Admin_BuildingsDelete(15, buildingName);
            base.SQL.Admin_BuildingsCreate(15, buildingName);

            // Login to Portal
            test.Portal.Login();

            // Create a room
            test.Portal.Admin_Rooms_Create(buildingName, roomName, "Test Description", "TR01");

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Updates a Room.")]
        public void Admin_ChurchSetup_Rooms_Update() {
            // Set initial conditions
            string buildingName1 = "Test Building 1 (Room Update)";
            string buildingName2 = "Test Building 2 (Room Update)";
            string roomName = "Test Room - Update";
            string roomCode = "TR01";
            string roomDescription = "Test Description";
            string roomNameUpdated = "Test Room - Mod";
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            base.SQL.Admin_BuildingsDelete(15, buildingName1);
            base.SQL.Admin_BuildingsDelete(15, buildingName2);
            base.SQL.Admin_RoomsDelete(15, roomName);
            base.SQL.Admin_RoomsDelete(15, roomNameUpdated);
            base.SQL.Admin_BuildingsCreate(15, buildingName1);
            base.SQL.Admin_BuildingsCreate(15, buildingName2);
            base.SQL.Admin_RoomsCreate(15, roomCode, buildingName1, roomName, roomDescription);

            // Login to Portal
            test.Portal.Login();

            // Update a room
            test.Portal.Admin_Rooms_Update(buildingName1, roomName, roomDescription, roomCode, buildingName2, roomNameUpdated, "Test Description - Mod", "TR01M");

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Deletes a Room.")]
        public void Admin_ChurchSetup_Rooms_Delete() {
            // Set initial conditions
            string buildingName = "Test Building (Room Delete)";
            string roomName = "Test Room - Delete";
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            base.SQL.Admin_RoomsDelete(15, roomName);
            base.SQL.Admin_BuildingsDelete(15, buildingName);
            base.SQL.Admin_BuildingsCreate(15, buildingName);
            base.SQL.Admin_RoomsCreate(15, "TR01", buildingName, roomName, "Test Description");

            // Login to Portal
            test.Portal.Login();

            // Delete a room
            test.Portal.Admin_Rooms_Delete(roomName);

            // Logout of Portal
            test.Portal.Logout();
        }
        #endregion Rooms

        #region Departments
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create a Department with no data entered.")]
        public void Admin_ChurchSetup_Departments_CreateNoData() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Attempt to create a department with no 
            test.Portal.Admin_Departments_Create(null, null);

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Creates a Department.")]
        public void Admin_ChurchSetup_Departments_Create() {
            // Set initial conditions
            string departmentName = "01 Test Department";
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            base.SQL.Admin_DepartmentsDelete(15, departmentName);

            // Login to Portal
            test.Portal.Login();

            // Create a department
            test.Portal.Admin_Departments_Create(departmentName, "TD01");

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Updates a Department.")]
        public void Admin_ChurchSetup_Departments_Update() {
            // Set initial conditions
            string departmentName = "01 Test Department - Update";
            string departmentCode = "TD01U";
            string departmentNameUpdated = "01 Test Department - Modified";
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            base.SQL.Admin_DepartmentsDelete(15, departmentName);
            base.SQL.Admin_DepartmentsDelete(15, departmentNameUpdated);
            base.SQL.Admin_DepartmentsCreate(15, departmentName, departmentCode);

            // Login to Portal
            test.Portal.Login();

            // Update a department
            test.Portal.Admin_Departments_Update(departmentName, departmentCode, departmentNameUpdated, "TD01M");

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Deletes a Department.")]
        public void Admin_ChurchSetup_Departments_Delete() {
            // Set initial conditions
            string departmentName = "01 Test Department - Delete";
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            base.SQL.Admin_DepartmentsDelete(15, departmentName);
            base.SQL.Admin_DepartmentsCreate(15, departmentName, null);

            // Login to Portal
            test.Portal.Login();

            // Delete a department
            test.Portal.Admin_Departments_Delete(departmentName);

            // Logout of Portal
            test.Portal.Logout();
        }
        #endregion Departments

        #region Mailing Address
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Updates the church mailing address.")]
        public void Admin_ChurchSetup_MailingAddress_Update() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to admin->mailing address
            test.Selenium.Navigate(Navigation.Admin.Church_Setup.Mailing_Address);

            // Change the address
            test.Selenium.Select("ctl00_ctl00_MainContent_content_ctlAddress_ddlCountry_dropDownList", "United States");
            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlAddress_txtAddress1_textBox", "9616 Armour Dr");
            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlAddress_txtAddress2_textBox", "");
            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlAddress_txtCity_textBox", "Keller");
            test.Selenium.Select("ctl00_ctl00_MainContent_content_ctlAddress_ddlState_dropDownList", "Texas");
            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlAddress_txtPostalCode_textBox", "76244");
            test.Selenium.ClickAndWaitForPageToLoad("ctl00_ctl00_MainContent_content_btnSubmit");

            // Navigate to admin->mailing address
            test.Selenium.Navigate(Navigation.Admin.Church_Setup.Mailing_Address);

            // Verify the address
            Assert.AreEqual("United States", test.Selenium.GetSelectedLabel("ctl00_ctl00_MainContent_content_ctlAddress_ddlCountry_dropDownList"));
            Assert.AreEqual("9616 Armour Dr", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlAddress_txtAddress1_textBox"));
            Assert.AreEqual("", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlAddress_txtAddress2_textBox"));
            Assert.AreEqual("Keller", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlAddress_txtCity_textBox"));
            Assert.AreEqual("Texas", test.Selenium.GetSelectedLabel("ctl00_ctl00_MainContent_content_ctlAddress_ddlState_dropDownList"));
            Assert.AreEqual("76244", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_ctlAddress_txtPostalCode_textBox"));

            // Restore the address
            test.Selenium.Select("ctl00_ctl00_MainContent_content_ctlAddress_ddlCountry_dropDownList", "United States");
            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlAddress_txtAddress1_textBox", "6363 N. State Hwy 161");
            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlAddress_txtAddress2_textBox", "Suite 200");
            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlAddress_txtCity_textBox", "Irving");
            test.Selenium.Select("ctl00_ctl00_MainContent_content_ctlAddress_ddlState_dropDownList", "Texas");
            test.Selenium.Type("ctl00_ctl00_MainContent_content_ctlAddress_txtPostalCode_textBox", "75038");
            test.Selenium.ClickAndWaitForPageToLoad("ctl00_ctl00_MainContent_content_btnSubmit");

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Mailing Address

        #region TWA Census
        #endregion TWA Census

        #region Giftedness Programs
        #endregion Giftedness Programs
    }
}