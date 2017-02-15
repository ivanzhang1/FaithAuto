using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace FTTests.Portal.People {
    [TestFixture]
    public class Portal_People_Search_PeopleQuery : FixtureBaseWebDriver {
        private string _peopleQueryError = "Please provide a proper value in the \"Value\" field below.";

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies People Query results: Household Communication->Value->EqualTo->6302172170")]
        public void People_Search_PeopleQuery_HHComm_Value_EqualTo_6302172170() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Perform a people query
            test.Portal.People_PeopleQuery(SearchByConstants.Household_Communication, FieldConstants.Value, ComparisonConstants.Equal_To, new string[] { "6302172170" });
            test.Portal.People_VerifyPeopleSearchResults("Phone", "6302172170");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        public void People_Search_PeopleQuery_HHAddressNoValue() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Perform a people query
            test.Portal.People_PeopleQuery(SearchByConstants.Household_Address, FieldConstants.Address, ComparisonConstants.Contains, null);

            // Verify the error message(s)
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(TextConstants.ErrorHeadingSingular));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(this._peopleQueryError));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        public void People_Search_PeopleQuery_HHCommunicationNoValue() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Perform a people query
            test.Portal.People_PeopleQuery(SearchByConstants.Household_Communication, FieldConstants.Value, ComparisonConstants.Contains, null);

            // Verify the error message(s)
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(TextConstants.ErrorHeadingSingular));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(this._peopleQueryError));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        public void People_Search_PeopleQuery_INDAddressNoValue() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Perform a people query
            test.Portal.People_PeopleQuery(SearchByConstants.Individual_Address, FieldConstants.Address, ComparisonConstants.Contains, null);

            // Verify the error message(s)
            //Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(TextConstants.ErrorHeadingSingular));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(this._peopleQueryError));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        public void People_Search_PeopleQuery_INDCommunicationNoValue() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Perform a people query
            test.Portal.People_PeopleQuery(SearchByConstants.Individual_Communication, FieldConstants.Value, ComparisonConstants.Contains, null);

            // Verify the error message(s)
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(TextConstants.ErrorHeadingSingular));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(this._peopleQueryError));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        public void People_Search_PeopleQuery_INDInformationBarCodeNoValue() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Perform a people query
            test.Portal.People_PeopleQuery(SearchByConstants.Individual_Information, FieldConstants.Bar_Code, ComparisonConstants.Contains, null);

            // Verify the error message(s)
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(TextConstants.ErrorHeadingSingular));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(this._peopleQueryError));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        public void People_Search_PeopleQuery_INDInformationMemberEnvCodeNoValue() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Perform a people query
            test.Portal.People_PeopleQuery(SearchByConstants.Individual_Information, FieldConstants.Member_Env_Code, ComparisonConstants.Contains, null);

            // Verify the error message(s)
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(TextConstants.ErrorHeadingSingular));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(this._peopleQueryError));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        public void People_Search_PeopleQuery_INDInformationFirstNameNoValue() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Perform a people query
            test.Portal.People_PeopleQuery(SearchByConstants.Individual_Information, FieldConstants.First_Name, ComparisonConstants.Contains, null);

            // Verify the error message(s)
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(TextConstants.ErrorHeadingSingular));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(this._peopleQueryError));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the user can perform a people query using date of birth.")]
        public void People_Search_PeopleQuery_DateOfBirth() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Perform a people query
            test.Portal.People_PeopleQuery(SearchByConstants.Individual_Information, FieldConstants.Date_Of_Birth, ComparisonConstants.Is_Equal_To, new string [] {"09/02/1982"});

            // Verify the search results
            Assert.AreEqual("Matthew Sneeden", test.Driver.FindElementById(TableIds.Portal.People_Individuals).FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[2].FindElement(By.TagName("a")).Text);
            Assert.IsTrue(test.Driver.FindElementById(TableIds.Portal.People_Individuals).FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[3].Text.Contains("9/2/1982"), "Date of birth was not in the correct format.");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the user can perform a people query using date of birth in an international church.")]
        public void People_Search_PeopleQuery_DateOfBirth_International() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "QAEUNLX0C6");

            // Perform a people query
            test.Portal.People_PeopleQuery(SearchByConstants.Individual_Information, FieldConstants.Date_Of_Birth, ComparisonConstants.Is_Equal_To, new string[] { "02/09/1982" });

            // Verify the search results
            Assert.AreEqual("Matthew Sneeden", test.Driver.FindElementById(TableIds.Portal.People_Individuals).FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[2].FindElement(By.TagName("a")).Text);
            Assert.IsTrue(test.Driver.FindElementById(TableIds.Portal.People_Individuals).FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[3].Text.Contains("02/09/1982"), "Date of birth was not in the correct format.");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the user cannot perform a people query when the date of birth is null.")]
        public void People_Search_PeopleQuery_DateOfBirthNoValue() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Perform a people query
            test.Portal.People_PeopleQuery(SearchByConstants.Individual_Information, FieldConstants.Date_Of_Birth, ComparisonConstants.Is_Equal_To, null);

            // Verify the error message(s)
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(TextConstants.ErrorHeadingSingular));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(this._peopleQueryError));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user can generate labels via people query.")]
        public void People_Search_PeopleQuery_Export_Labels() {
            // Set initial conditions
            string report = "Export to Labels (Query Builder)";
            base.SQL.ReportLibrary_Queue_DeleteItem(15, 65211, report);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Perform a people query
            test.Portal.People_PeopleQuery(SearchByConstants.Household_Address, FieldConstants.Address, ComparisonConstants.Contains, new string[] { "9616 Armour Dr" });

            // Attempt to export to labels
            int itemRowHause = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Portal.People_PeopleQuery, "Laura Hause", "Name", "contains");
            test.Driver.FindElementById(TableIds.Portal.People_PeopleQuery).FindElements(By.TagName("tr"))[itemRowHause].FindElements(By.TagName("td"))[0].FindElement(By.TagName("input")).Click();
            
            int itemRowSneeden = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Portal.People_PeopleQuery, "Matthew Sneeden", "Name", "contains");
            test.Driver.FindElementById(TableIds.Portal.People_PeopleQuery).FindElements(By.TagName("tr"))[itemRowSneeden].FindElements(By.TagName("td"))[0].FindElement(By.TagName("input")).Click();
                        
            test.Driver.FindElementByXPath("//a[@class='grid_gear']").Click();
            test.Driver.FindElementByLinkText("Export…").Click(); // was wait for condition, "selenium.isElementPresent(\"xpath=//div[@id='form_modal_border']\");", "10000");
            test.Driver.FindElementByLinkText("Export to labels »").Click();
            //modify by grace zhang
            new SelectElement(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlLabelStyles")).SelectByIndex(1); //SelectByText("A Test Label Style");
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnExport").Click();

            // Verify the labels are created successfully
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Download Your Labels"));
            if (test.Driver.FindElementByTagName("html").Text.Contains("Your labels are being processed…")) {
                Retry.WithPolling(15000).WithTimeout(75000).Until(() => test.Driver.FindElementByTagName("html").Text.Contains("Labels processing has completed!"));
            }

            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Labels processing has completed!"));
            Assert.IsTrue(test.Driver.FindElementsByLinkText("Download your labels").Count > 0);
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Once you leave this page you will no longer be able to save this set of labels."));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("So, be sure to save them to your computer."));

            // Logout of portal
            test.Portal.LogoutWebDriver();


            // Login to Report Library
            //string url = this.F1Environment == F1Environments.QA ? string.Format("http://reportlibrary{0}.dev.corp.local/ReportLibrary/Login/Index.aspx", this.F1Environment) : "https://staging-reportlibrary.fellowshipone.com/ReportLibrary/Login/Index.aspx";
            string url = test.ReportLibrary.URL;
            TestLog.WriteLine(string.Format("Go To Report Library URL: {0}", url));
            test.Driver.Navigate().GoToUrl(url);
            test.Driver.FindElementById("username").SendKeys("msneeden");
            test.Driver.FindElementById("password").SendKeys("Pa$$w0rd");
            test.Driver.FindElementById("churchcode").SendKeys("dc");
            test.Driver.FindElementById("btn_login").Click();

            // View the report queue
            test.Driver.FindElementByLinkText("Queue").Click();

            // Verify the report exists and has completed
            int itemRowQueue = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.ReportLibrary.Queue, report, "Name");
            IWebElement table = test.Driver.FindElementById(TableIds.ReportLibrary.Queue);
            Assert.IsTrue(table.FindElements(By.TagName("tr"))[itemRowQueue].FindElements(By.TagName("td"))[0].FindElements(By.XPath("a/img[contains(@src, '/ReportLibrary/public/images/status_complete.png?') and @alt='Complete']")).Count > 0);//, "//table[@id='ctl00_MainContent_grdReports']" test.Selenium.IsElementPresent(string.Format("{0}/tbody/tr[{1}]/td[1]/a/img[contains(@src, '/ReportLibrary/public/images/status_complete.png?') and @alt='Complete']", "//table[@id='ctl00_MainContent_grdReports']", itemRowQueue + 1)));
            Assert.AreEqual(report, table.FindElements(By.TagName("tr"))[itemRowQueue].FindElements(By.TagName("td"))[1].Text);
            Assert.IsTrue(table.FindElements(By.TagName("tr"))[itemRowQueue].FindElements(By.TagName("td"))[2].FindElement(By.TagName("a")).GetAttribute("text").Equals("Download")/*  test.Selenium.IsElementPresent(string.Format("{0}/tbody/tr[{1}]/td[3]/a[text()='Download']", "//table[@id='ctl00_MainContent_grdReports']", itemRowQueue + 1))*/);
            Assert.IsTrue(table.FindElements(By.TagName("tr"))[itemRowQueue].FindElements(By.TagName("td"))[4].Text.Contains("Today at ")/*  test.Selenium.IsElementPresent(string.Format("{0}/tbody/tr[{1}]/td[position()=5 and contains(text(), 'Today at ')]", "//table[@id='ctl00_MainContent_grdReports']", itemRowQueue + 1))*/);

            // Logout of report library
            test.Driver.FindElementByLinkText("Log Out").Click();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies you can change the starting position for labels when exporting to labels")]
        public void People_Search_PeopleQuery_Export_Labels_ChangeStartPosition()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a people query
            test.Portal.People_PeopleQuery(SearchByConstants.Household_Address, FieldConstants.Address, ComparisonConstants.Contains, new string[] { "4505" });

            // Select all recipients
            test.Driver.FindElementById(GeneralPeople.PeopleQuery.Check_All).Click();

            // Click the gear icon and select to expost to labels
            test.Driver.FindElementByXPath("//a[@class='grid_gear']").Click();
            test.Driver.FindElementByLinkText("Export…").Click();
            test.Driver.FindElementByLinkText("Export to labels »").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Change starting position"));
            //modify by grace zhang
            new SelectElement(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ddlLabelStyles")).SelectByIndex(1); //SelectByText("A Test Label Style");
            test.Driver.FindElementByLinkText("Change starting position").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Close"), 30, "Label position grid did not appear");

            // Verify that the label position grid appears
            Assert.IsTrue(test.Driver.FindElementById("selectStartPosition").Displayed, "Label position grid was not visible");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies link contents in exporting dialog are display well")]
        public void People_Search_PeopleQuery_Export_LinkDisplay()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Perform a people query
            test.Portal.People_PeopleQuery(SearchByConstants.Individual_Information, FieldConstants.Status, ComparisonConstants.NULL, null);

            // Select all recipients
            test.Driver.FindElementById(GeneralPeople.PeopleQuery.Check_All).Click();

            // Click the gear icon and select to expost to labels
            test.Driver.FindElementByXPath("//a[@class='grid_gear']").Click();
            test.Driver.FindElementByLinkText("Export…").Click();
            
            // Verify content display well in exporting dialog
            Assert.IsTrue(test.Driver.FindElementByLinkText("Export to labels »").Displayed, "Export to label link was not visible");

            Assert.IsTrue(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlSelectedPeopleAction_lnkExportToMailMergeTSV").Displayed, "Export link 1 was not visible");
            Assert.IsTrue(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlSelectedPeopleAction_lnkExportToMailMergeXLS").Displayed, "Export link 2 was not visible");
            Assert.IsTrue(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlSelectedPeopleAction_lnkExportToUniqueHouseholdMailMergeTSV").Displayed, "Export link 3 was not visible");
            Assert.IsTrue(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlSelectedPeopleAction_lnkExportToUniqueHouseholdMailMergeXLS").Displayed, "Export link 4 was not visible");

            Assert.IsTrue(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlSelectedPeopleAction_lnkExportToRosterTSV").Displayed, "Export link 5 was not visible");
            Assert.IsTrue(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlSelectedPeopleAction_lnkExportToRosterXLS").Displayed, "Export link 6 was not visible");
            Assert.IsTrue(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlSelectedPeopleAction_lnkExportToUniqueHouseholdRosterTSV").Displayed, "Export link 7 was not visible");
            Assert.IsTrue(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlSelectedPeopleAction_lnkExportToUniqueHouseholdRosterXLS").Displayed, "Export link 8 was not visible");
            
            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        #region My Saved Queries
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can save a query.")]
        public void People_Search_PeopleQuery_Save_Create() {
            // Set initial conditions
            string queryName = "My Saved Query";
            base.SQL.People_PeopleQuery_Delete(254, queryName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Perform a people query
            test.Portal.People_PeopleQuery(SearchByConstants.Individual_Information, FieldConstants.Last_Name, ComparisonConstants.Contains, new string[] { "Mikaelian" });
                
            // Save the query
            test.Portal.People_PeopleQuery_CreateSaved(queryName, null);

            // View saved queries
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.People.Search.My_Saved_Queries);

            // Verify the saved query exists
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver("//table[@class='grid']", queryName, "Name", null));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a name is required to save a query.")]
        public void People_Search_PeopleQuery_Save_Create_Named_Required() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Perform a people query
            test.Portal.People_PeopleQuery(SearchByConstants.Individual_Information, FieldConstants.Last_Name, ComparisonConstants.Contains, new string[] { "Mikaelian" });

            // Save the query without a name
            test.Portal.People_PeopleQuery_CreateSaved(null, null);

            // Verify name is required.
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Name is required to save a query."));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a description cannot exceed 255 characters for a saved query.")]
        public void People_Search_PeopleQuery_Save_Create_Description_Cannot_Exceed_255_Characters() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Perform a people query
            test.Portal.People_PeopleQuery(SearchByConstants.Individual_Information, FieldConstants.Last_Name, ComparisonConstants.Contains, new string[] { "Mikaelian" });

            // Save the query a long description
            test.Portal.People_PeopleQuery_CreateSaved("Long Description", "Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description Really long description ");

            // Verify description cannot exceed 255 characters.
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Description cannot exceed 255 characters. Please provide a shorter description."));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
 
        [Test, RepeatOnFailure, Timeout(1000)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can delete a saved query.")]
        public void People_Search_PeopleQuery_Save_Delete() {
            // Set initial conditions
            string queryName = "My Saved Query - Delete";
            base.SQL.People_PeopleQuery_Delete(254, queryName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Perform a people query
            test.Portal.People_PeopleQuery(SearchByConstants.Individual_Information, FieldConstants.First_Name, ComparisonConstants.Equal_To, new string[] { "Bryan" });

            // Save the query
            test.Portal.People_PeopleQuery_CreateSaved(queryName, null);

            // Delete the saved query
            test.Portal.People_PeopleQuery_DeleteSaved(queryName);

            // Verify the query was deleted.  If the table is there, verify in the table.  Else verify we don't even see it.
            if (test.Driver.FindElementsByXPath("//table[@class='grid']").Count > 0) {
                Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver("//table[@class='grid']", queryName, "Name", null));
            }
            else {
                Assert.IsFalse(test.Driver.FindElementsByLinkText(queryName).Count > 0);
            }

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion My Saved Queries
    }
}