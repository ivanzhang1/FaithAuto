using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using Selenium;

namespace FTTests.Report_Library {
    [TestFixture]
    public class ReportLibrary : FixtureBaseWebDriver
    {
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the Queue page in Report Library.")]
        public void Queue()
        {
            // Login to report library
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.ReportLibrary.Login();

            // View the queue
            test.Driver.FindElementByLinkText("Queue").Click();

            // Verify page loaded
            Assert.AreEqual("Queue", test.Driver.FindElementByXPath("//div[@id='title']/h1").Text);

            // Logout of report library
            test.ReportLibrary.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the Tags page in Report Library.")]
        public void Tags()
        {
            // Login to report library
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.ReportLibrary.Login();

            // View the tags page
            test.Driver.FindElementByLinkText("Tags").Click();

            // Verify page loaded
            Assert.AreEqual("Manage Your Tags", test.Driver.FindElementByXPath("//h1[@class='page_header']").Text);

            // Logout of report library
            test.ReportLibrary.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the Label Styles page in Report Library.")]
        public void LabelStyles()
        {
            // Login to report library
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.ReportLibrary.Login();

            // View the label styles page
            test.Driver.FindElementByLinkText("Label Styles").Click();

            // Verify page loaded
            Assert.AreEqual("Manage Your Label Styles", test.Driver.FindElementByXPath("//h1[@class='page_header']").Text);

            // Logout of report library
            test.ReportLibrary.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the Queue page in Report Library in an international church.")]
        public void Queue_International()
        {
            // Login to report library
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.ReportLibrary.Login("msneeden", "Pa$$w0rd", "QAEUNLX0C6");

            // View the queue
            test.Driver.FindElementByLinkText("Queue").Click();

            // Verify page loaded
            Assert.AreEqual("Queue", test.Driver.FindElementByXPath("//div[@id='title']/h1").Text);

            // Logout of report library
            test.ReportLibrary.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the Tags page in Report Library for an international church.")]
        public void Tags_International()
        {
            // Login to report library
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.ReportLibrary.Login("msneeden", "Pa$$w0rd", "QAEUNLX0C6");

            // View the tags page
            test.Driver.FindElementByLinkText("Tags").Click();

            // Verify page loaded
            Assert.AreEqual("Manage Your Tags", test.Driver.FindElementByXPath("//h1[@class='page_header']").Text);

            // Logout of report library
            test.ReportLibrary.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the Label Styles page in Report Library for an international church.")]
        public void LabelStyles_International()
        {
            // Login to report library
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.ReportLibrary.Login("msneeden", "Pa$$w0rd", "QAEUNLX0C6");

            // View the label styles page
            test.Driver.FindElementByLinkText("Label Styles").Click();

            // Verify page loaded
            Assert.AreEqual("Manage Your Label Styles", test.Driver.FindElementByXPath("//h1[@class='page_header']").Text);

            // Logout of report library
            test.ReportLibrary.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Generates a report resulting in a temporary group.")]
        public void Report_P1100_TemporaryGroup()
        {
            // Set initial conditions
            string reportName = "Individuals with Birthday in X Month (Temporary Group)";
            string reportCode = "P1100";
            base.SQL.ReportLibrary_Queue_DeleteItem(15, 65211, reportName);
            base.SQL.Groups_Group_Delete(15, reportName);

            // Login to report library
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.ReportLibrary.Login();

            // Search for and select a report
            test.Driver.FindElementByXPath("//input[@name='q']").SendKeys("P1100");
            test.Driver.FindElementByXPath("//input[@value='Search']").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText(reportName));
            test.Driver.FindElementByLinkText(reportName).Click();

            // Populate the report data
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("dd_BirthMonth"));
            new SelectElement(test.Driver.FindElementById("dd_BirthMonth")).SelectByText("Sep");
            new SelectElement(test.Driver.FindElementById("dd_BirthMonthEnd")).SelectByText("Sep");

            test.Driver.FindElementByLinkText("Additional Filters").Click();

            test.Driver.FindElementById("rb_UseBirthDay_add").Click();
            test.Driver.FindElementById("tb_BirthDay_add").SendKeys("2");
            test.Driver.FindElementById("tb_BirthDayEnd_add").SendKeys("2");

            test.Driver.FindElementById("tb_StartAge_add").SendKeys("29");
            test.Driver.FindElementById("tb_EndAge_add").SendKeys("31");
            test.Driver.FindElementByXPath("//input[@id='submitQuery']").Click();

            // Verify the report exists and has completed
            int itemRowQueue = 0;

            Retry.WithPolling(3000).WithTimeout(200000).DoBetween(() => { test.Driver.FindElementByLinkText("Refresh for latest results").Click(); itemRowQueue = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.ReportLibrary.Queue, reportName, "Name"); }).Until(() => test.Driver.FindElementById(TableIds.ReportLibrary.Queue).FindElements(By.XPath(string.Format("//tr[{0}]/td[1]/img[@alt='Complete']", itemRowQueue + 1))).Count > 0);
            Assert.IsTrue(test.Driver.FindElementById(TableIds.ReportLibrary.Queue).FindElement(By.XPath(string.Format("//tr[{0}]/td[1]/img[contains(@src, '/ReportLibrary/public/images/status_complete.png?') and @alt='Complete']", itemRowQueue + 1))).Displayed);
            Assert.AreEqual(reportName, test.Driver.FindElementById(TableIds.ReportLibrary.Queue).FindElements(By.TagName("tr"))[itemRowQueue].FindElements(By.TagName("td"))[1].Text);
            Assert.IsTrue(test.Driver.FindElementById(TableIds.ReportLibrary.Queue).FindElement(By.XPath(string.Format("//tr[{0}]/td[position()=3 and normalize-space(text())='Temporary Group']", itemRowQueue + 1))).Displayed);
            Assert.AreEqual(reportCode, test.Driver.FindElementById(TableIds.ReportLibrary.Queue).FindElements(By.TagName("tr"))[itemRowQueue].FindElements(By.TagName("td"))[3].Text);
            Assert.IsTrue(test.Driver.FindElementById(TableIds.ReportLibrary.Queue).FindElement(By.XPath(string.Format("//tr[{0}]/td[position()=5 and contains(text(), 'Today at ')]", itemRowQueue + 1))).Displayed);

            // Logout of report library
            test.ReportLibrary.Logout();


            // Login to portal
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // Verify the temporary group was created
            test.Driver.FindElementByLinkText("Groups").Click();
            test.Driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(1000));
            IWebElement menuItem = test.Driver.FindElement(By.XPath(string.Format("//div[@style='visibility: visible;']/dl/dt[contains(text(), '{0}')]/following-sibling::dd/a[contains(text(), '{1}')]", "Groups by Group Type", "View All")));

            menuItem.Click();
            test.Driver.FindElementByLinkText("Temporary").Click();
            int itemRowTempGroupTable = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Portal.Groups_ViewAll_GroupList_TemporaryTab, string.Format("{0}\r\nCreated from Report Library via Individuals wit...", reportName), "Temporary Group");
            Assert.AreEqual(string.Format("{0}\r\nCreated from Report Library via Individuals wit...", reportName), test.Driver.FindElementById(TableIds.Portal.Groups_ViewAll_GroupList_TemporaryTab).FindElement(By.XPath(string.Format("//tr[{0}]/td[1]", itemRowTempGroupTable + 1))).Text);
            Assert.IsTrue(test.Driver.FindElementById(TableIds.Portal.Groups_ViewAll_GroupList_TemporaryTab).FindElement(By.XPath(string.Format("//tr[{0}]/td[2]", itemRowTempGroupTable + 1))).Text.Contains(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(3).AddHours(6).ToShortDateString()));   // ToString("M/d/yyyy h:mm:ss tt ")

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Generates a report resulting in Avery 5160 lables.")]
        public void Report_P1100L_Avery5160()
        {
            // Set initial conditions
            string reportName = "Members with Birthday in X Month(Avery 5160)";
            string reportCode = "P1100L";

            // Login to report library
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.ReportLibrary.Login();

            // Search for and select a report
            TestLog.WriteLine("Search for P1100");
            test.Driver.FindElementByXPath("//input[@name='q']").SendKeys("P1100");
            //test.Driver.FindElementByLinkText("Search").Click();
            test.Driver.FindElementByXPath("//input[@value='Search']").Click();

            TestLog.WriteLine("Click on " + reportName);

            IWebElement reportElement = test.Driver.FindElementByLinkText(reportName);
            TestLog.WriteLine("URL Text: " + reportElement.Text.ToString());
            try
            {
                reportElement.Click();
            }
            catch (Exception e)
            {
                TestLog.WriteLine("Try Again!!!");
                reportElement.Click();
            }

            //test.Driver.FindElementByXPath(string.Format("//a[contains(text(), '{0}')]", reportName)).Click();

            // Populate the report data
            new SelectElement(test.Driver.FindElementById("dd_BirthMonth")).SelectByText("Sep");
            new SelectElement(test.Driver.FindElementById("dd_BirthMonthEnd")).SelectByText("Sep");

            test.Driver.FindElementByLinkText("Additional Filters").Click();

            test.Driver.FindElementById("tb_StartAge_add").SendKeys("29");
            test.Driver.FindElementById("tb_EndAge_add").Clear();
            test.Driver.FindElementById("tb_EndAge_add").SendKeys("31");
            test.Driver.FindElementByXPath("//input[@id='submitQuery']").Click();

            // Verify the report exists and has completed
            int itemRowQueue = 0;
            Retry.WithPolling(2500).WithTimeout(300000).DoBetween(() => { test.Driver.FindElementByLinkText("Refresh for latest results").Click(); test.Driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(5000)); IWebElement table = test.Driver.FindElementById(TableIds.ReportLibrary.Queue); itemRowQueue = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.ReportLibrary.Queue, reportName, "Name", "contains"); }).Until(() => test.Driver.FindElementById(TableIds.ReportLibrary.Queue).FindElements(By.XPath(string.Format("tbody/tr[{0}]/td[1]/a/img[@alt='Complete']", itemRowQueue + 1))).Count > 0);
            Assert.IsTrue(test.Driver.FindElementById(TableIds.ReportLibrary.Queue).FindElement(By.XPath(string.Format("//tr[{0}]/td[1]/a/img[contains(@src, '/ReportLibrary/public/images/status_complete.png?') and @alt='Complete']", itemRowQueue + 1))).Displayed);
            Assert.IsTrue(test.Driver.FindElementById(TableIds.ReportLibrary.Queue).FindElements(By.TagName("tr"))[itemRowQueue].FindElements(By.TagName("td"))[1].Text.Contains(reportName));
            Assert.IsTrue(test.Driver.FindElementById(TableIds.ReportLibrary.Queue).FindElement(By.XPath(string.Format("//tr[{0}]/td[3]/a[text()='Download']", itemRowQueue + 1))).Displayed);
            Assert.IsTrue(test.Driver.FindElementById(TableIds.ReportLibrary.Queue).FindElements(By.TagName("tr"))[itemRowQueue].FindElements(By.TagName("td"))[3].Text.Contains(reportCode));
            Assert.IsTrue(test.Driver.FindElementById(TableIds.ReportLibrary.Queue).FindElement(By.XPath(string.Format("//tr[{0}]/td[position()=5 and contains(text(), 'Today at ')]", itemRowQueue + 1))).Displayed);

            // Logout of report library
            test.ReportLibrary.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Generates a report exporting individuals to labels.")]
        //public void Report_P1070E_IndividualsBySchoolAndStatus() {
        public void Report_P9400_IndividualsBySchoolAndStatus()
        {
            // Set initial conditions
            //string reportName = "Individuals by School and Status";
            //string reportCode = "P1070E";
            string reportName = "*Core People Records (v3.1)";
            string reportCodeSearch = "P9400";
            string reportCode = "P9400 v3.1";
            base.SQL.ReportLibrary_Queue_DeleteItem(15, 65211, reportName);

            // Login to report library
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.ReportLibrary.Login();

            // Search for and select a report
            test.Driver.FindElementByXPath("//input[@name='q']").SendKeys(reportCodeSearch);
            test.Driver.FindElementByXPath("//input[@value='Search']").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText(reportName));

            test.Driver.FindElementByLinkText(reportName).Click();

            // Populate the report data
            /* OLD DATA REPORT
            new SelectElement(test.Driver.FindElementById("lb_StatusGroup")).SelectByText("Member");
            new SelectElement(test.Driver.FindElementById("lb_Status")).SelectByText("Member - Member");

            new SelectElement(test.Driver.FindElementById("output_type")).SelectByText("Export Individuals to Labels...");
            new SelectElement(test.Driver.FindElementById("label_format")).SelectByText("A Test Label Style");
            */

            test.Driver.FindElementByXPath("//input[@id='submitQuery']").Click();

            int itemRowQueue = 0;

            Retry.WithPolling(3000).WithTimeout(250000).DoBetween(() => { test.Driver.FindElementByLinkText("Refresh for latest results").Click(); itemRowQueue = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.ReportLibrary.Queue, reportName, "Name"); }).Until(() => test.Driver.FindElementById(TableIds.ReportLibrary.Queue).FindElements(By.XPath(string.Format("//tr[{0}]/td[1]/a/img[@alt='Complete']", itemRowQueue + 1))).Count > 0);
            Assert.IsTrue(test.Driver.FindElementById(TableIds.ReportLibrary.Queue).FindElement(By.XPath(string.Format("//tr[{0}]/td[1]/a/img[contains(@src, '/ReportLibrary/public/images/status_complete.png?') and @alt='Complete']", itemRowQueue + 1))).Displayed);
            Assert.AreEqual(reportName, test.Driver.FindElementById(TableIds.ReportLibrary.Queue).FindElements(By.TagName("tr"))[itemRowQueue].FindElements(By.TagName("td"))[1].Text);
            Assert.IsTrue(test.Driver.FindElementById(TableIds.ReportLibrary.Queue).FindElement(By.XPath(string.Format("//tr[{0}]/td[3]/a[text()='Download']", itemRowQueue + 1))).Displayed);
            Assert.AreEqual(reportCode, test.Driver.FindElementById(TableIds.ReportLibrary.Queue).FindElements(By.TagName("tr"))[itemRowQueue].FindElements(By.TagName("td"))[3].Text);
            Assert.IsTrue(test.Driver.FindElementById(TableIds.ReportLibrary.Queue).FindElement(By.XPath(string.Format("//tr[{0}]/td[position()=5 and contains(text(), 'Today at ')]", itemRowQueue + 1))).Displayed);

            // Logout of report library
            test.ReportLibrary.Logout();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Felix Gaytan")]
        [Description("F1-3650: HV/LC: Share A Report (SB): Save a Report")]
        public void Report_Save_Delete_Report()
        {
            // Set initial conditions
            string reportName = "*Core Assignment Records (v2.6)";
            string reportCodeSearch = "M1400";
            string reportCode = "M1400 v2.6";
            string guidRand = Guid.NewGuid().ToString().Substring(0, 5);
            string reportTitle = string.Format("Saved Assignment Records {0}", guidRand);
            string savedReportName = string.Format("Saved Assignment Records {0}", guidRand);
            string savedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MMM d, yyyy");

            // Login to report library
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.ReportLibrary.Login();

            // Search for and select a report
            test.ReportLibrary.Report_Search_By_Code(reportCodeSearch, reportName);

            // Go To Report
            test.ReportLibrary.Report_GoTo_Report(reportName);

            // Save Report
            test.ReportLibrary.Report_Save_MyReports(reportTitle, savedReportName);

            //Delete Report
            test.ReportLibrary.Report_Delete(savedReportName);

            //Logout
            test.ReportLibrary.Logout();

        }




        #region Label Styles
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Creates an active label style in Report Library.")]
        public void LabelStyles_Create_Active()
        {
            // Set initial conditions
            string labelStyleName = "Test Label Style - Active";
            base.SQL.Reports_DeleteLabelStyle(15, labelStyleName);

            // Login to report library
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.ReportLibrary.Login();

            // Create a label style
            test.ReportLibrary.LabelStyles_Create(labelStyleName, true, true);

            // Logout of report library
            test.ReportLibrary.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Creates an inactive label style in Report Library.")]
        public void LabelStyles_Create_Inactive()
        {
            // Set initial conditions
            string labelStyleName = "Test Label Style - Inactive";
            base.SQL.Reports_DeleteLabelStyle(15, labelStyleName);

            // Login to report library
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.ReportLibrary.Login();

            // Create a label style
            test.ReportLibrary.LabelStyles_Create(labelStyleName, true, false);

            // Logout of Report Library
            test.ReportLibrary.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Creates a non-shared label style in Report Library.")]
        public void LabelStyles_Create_NotShared()
        {
            // Set initial conditions
            string labelStyleName = "Test Label Style - Not Shared";
            base.SQL.Reports_DeleteLabelStyle(15, labelStyleName);

            // Login to report library
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.ReportLibrary.Login();

            // Create a label style
            test.ReportLibrary.LabelStyles_Create(labelStyleName, false, true);

            // Logout of Report Library
            test.ReportLibrary.Logout();
        }


        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Deletes a label style in Report Library.")]
        public void LabelStyles_Delete()
        {
            // Set initial conditions
            string labelStyleName = "Test Label Style - Delete";
            base.SQL.Reports_DeleteLabelStyle(15, labelStyleName);
            base.SQL.Reports_DeleteLabelStyle(15, "Test Label Style - Placeholder");

            // Login to report library
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.ReportLibrary.Login();

            // Create two label styles (need more than one to delete via the UI)
            test.ReportLibrary.LabelStyles_Create("Test Label Style - Placeholder", true, true);
            test.ReportLibrary.LabelStyles_Create(labelStyleName, true, true);

            // Delete a label style
            string labelStyleNameTable = string.Format("{0}\r\nAvailable to all users", labelStyleName);
            int itemRow = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.ReportLibrary.LabelStyles, labelStyleNameTable, "Label Style Name");
            test.Driver.FindElementById(TableIds.ReportLibrary.LabelStyles).FindElement(By.XPath(string.Format("//tr[{0}]/td[4]/form/a", itemRow + 1))).Click();
            test.Driver.SwitchTo().Alert().Accept();
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.ReportLibrary.LabelStyles, labelStyleNameTable, "Label Style Name"));
        }

        #endregion Label Styles

        #region Share Report
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Felix Gaytan")]
        [Description("F1-3650: HV/LC: Share A Report (SB): Save and Share a Report")]
        public void Report_Save_Share_Delete_Report()
        {
            // Set initial conditions
            string reportName = "*Core Assignment Records (v2.6)";
            string reportCodeSearch = "M1400";
            string reportCode = "M1400 v2.6";
            string guidRand = Guid.NewGuid().ToString().Substring(0, 5);
            string reportTitle = string.Format("Shared Assignment Records {0}", guidRand);
            string sharedReportName = reportTitle;
            string sharedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MMM d, yyyy");

            // Login to report library
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            // Login
            test.ReportLibrary.Login();

            // Search for and select a report
            test.ReportLibrary.Report_Search_By_Code(reportCodeSearch, reportName);

            // Go To Report
            test.ReportLibrary.Report_GoTo_Report(reportName);

            // Save Report
            test.ReportLibrary.Report_Save_MyReports(reportTitle, sharedReportName);

            //Share a Report
            test.ReportLibrary.Report_Share(sharedReportName, new string[] { "Felix Gaytan" });

            //Delete Report
            test.ReportLibrary.Report_Delete(sharedReportName);

            //Logout
            test.ReportLibrary.Logout();

            //Login to user report was shared
            test.ReportLibrary.Login("felixg", "FG.Admin12", "DC");

            //Go to my_reports_tab
            test.GeneralMethods.WaitForElement(By.Id("my_reports_tab"));
            test.Driver.FindElementById("my_reports_tab").Click();

            //Verify that shared report is present
            //Assert.IsTrue(test.Driver.FindElementByLinkText(sharedReportName).Displayed, string.Format("Report [{0}] was not shared", sharedReportName));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(sharedReportName));

            //Verify Received From: FT Tester on Jun 4, 2014
            test.ReportLibrary.Report_Verify_Share(sharedReportName, "Matthew Sneeden", sharedDate);

            //Delete Report
            test.ReportLibrary.Report_Delete(sharedReportName);

            // Logout of report library
            test.ReportLibrary.Logout();
        }


        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("FO-961: REP - Accessing Saved Reports is Throwing an Error")]
        public void Report_View_Shared_Report_InActive_User()
        {
            // Set initial conditions
            string reportName = "*Core Assignment Records (v2.6)";
            string reportCodeSearch = "M1400";
            string reportCode = "M1400 v2.6";
            string guidRand = Guid.NewGuid().ToString().Substring(0, 5);
            string reportTitle = string.Format("Shared Assignment Records {0}", guidRand);
            string sharedReportName = reportTitle;
            string sharedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MMM d, yyyy");

            // Login to report library
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            //Make sure user is active
            test.SQL.Admin_Set_User_ActiveInActive(15, "QATest", 1);

            // Login portal user that will be set to inactive 
            test.ReportLibrary.Login("QATest", "FT4life!", "DC");

            // Search for and select a report
            test.ReportLibrary.Report_Search_By_Code(reportCodeSearch, reportName);

            // Go To Report
            test.ReportLibrary.Report_GoTo_Report(reportName);

            // Save Report
            test.ReportLibrary.Report_Save_MyReports(reportTitle, sharedReportName);

            //Share a Report
            test.ReportLibrary.Report_Share(sharedReportName, new string[] { "Felix Gaytan" });

            //Delete Report
            test.ReportLibrary.Report_Delete(sharedReportName);

            //Logout
            test.ReportLibrary.Logout();

            //Set portal user to inactive
            test.SQL.Admin_Set_User_ActiveInActive(15, "QATest", 0);

            //Login to user report was shared
            test.ReportLibrary.Login("felixg", "FG.Admin12", "DC");

            //Go to my_reports_tab
            test.GeneralMethods.WaitForElement(By.Id("my_reports_tab"));
            test.Driver.FindElementById("my_reports_tab").Click();

            //Verify that shared report is present
            //Assert.IsTrue(test.Driver.FindElementByLinkText(sharedReportName).Displayed, string.Format("Report [{0}] was not shared", sharedReportName));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(sharedReportName));

            //Verify Received From: FT Tester on Jun 4, 2014
            test.ReportLibrary.Report_Verify_Share(sharedReportName, "QA Test", sharedDate);

            //Delete Report
            test.ReportLibrary.Report_Delete(sharedReportName);

            // Logout of report library
            test.ReportLibrary.Logout();

            //Make sure user is active
            test.SQL.Admin_Set_User_ActiveInActive(15, "QATest", 0);

        }

        /// <summary>
        /// Report Library Share a Report
        /// </summary>
  
        [Test, RepeatOnFailure]
        [Author("Suchitra")]
        [Description("F1-3650 HV/LC: Share A Report")]
        public void Report_ShareReport_SingleUser()
        {

            // Set initial conditions
            string reportName = "*Core Assignment Records (v2.6)";
            string reportCodeSearch = "M1400";
            string reportCode = "M1400 v2.6";
            string guidRand = Guid.NewGuid().ToString().Substring(0, 5);
            string reportTitle = string.Format("Shared Assignment Records {0}", guidRand);
            string sharedReportName = reportTitle;
            string sharedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MMM d, yyyy");                     

            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            //Getting Portal User First and Last Name 
            string userNameToDisplay = test.SQL.FetchUserName(15, test.ReportLibrary.ReportLibraryUserName);
            TestLog.WriteLine("User Name: {0}", userNameToDisplay);

            // Login to report library
            test.ReportLibrary.Login();
            

            // Search for and select a report
            test.ReportLibrary.Report_Search_By_Code(reportCodeSearch, reportName);

            // Go To Report
            test.ReportLibrary.Report_GoTo_Report(reportName);


            // Save Report
            test.ReportLibrary.Report_Save_MyReports(reportTitle, sharedReportName);

            //Share a Report
            test.ReportLibrary.Report_Share(sharedReportName, new string[] { "FT Tester" });

            //Delete Report
            test.ReportLibrary.Report_Delete(sharedReportName);

            //Logout
            test.ReportLibrary.Logout();

           
            //Login to Report library  as Receipient to view report 
            
            test.ReportLibrary.Login("ft.tester", "FT4life!", "DC");

            //Go to my_reports_tab
            test.GeneralMethods.WaitForElement(By.Id("my_reports_tab"));
            test.Driver.FindElementById("my_reports_tab").Click();


            //Verify that shared report is present
            //Assert.IsTrue(test.Driver.FindElementByLinkText(sharedReportName).Displayed, string.Format("Report [{0}] was not shared", sharedReportName));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(sharedReportName));


            //Verify Received From: FT Tester on Jun 4, 2014
            test.ReportLibrary.Report_Verify_Share(sharedReportName, userNameToDisplay, sharedDate);
            //test.ReportLibrary.Report_Verify_Share(sharedReportName, "Matthew Sneeden", sharedDate);


            //Delete Report
            test.ReportLibrary.Report_Delete(sharedReportName);

            // Logout of report library
            test.ReportLibrary.Logout();

        }

        /// <summary>
        /// Share Report to Multiple Users
        /// </summary>
        /// <returns></returns>

        [Test, RepeatOnFailure]
        [Author("Suchitra")]
        [Description("F1-3650 HV/LC: Share A Report to Multiple Users")]
        public void Report_ShareReport_MultiUsers()
        {

            // Set initial conditions
            // Set initial conditions
            string reportName = "*Core Assignment Records (v2.6)";
            string reportCodeSearch = "M1400";
            string reportCode = "M1400 v2.6";
            string guidRand = Guid.NewGuid().ToString().Substring(0, 5);
            string reportTitle = string.Format("Shared Assignment Records {0}", guidRand);
            string sharedReportName = reportTitle;
            string sharedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MMM d, yyyy");


            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            // Login to report library
            test.ReportLibrary.Login();


            //Getting Portal User First and Last Name 
            string userNameToDisplay = test.SQL.FetchUserName(15, test.ReportLibrary.ReportLibraryUserName);
            TestLog.WriteLine("User Name: {0}", userNameToDisplay);

            // Search for and select a report
            test.ReportLibrary.Report_Search_By_Code(reportCodeSearch, reportName);

            // Go To Report
            test.ReportLibrary.Report_GoTo_Report(reportName);


            // Save Report
            test.ReportLibrary.Report_Save_MyReports(reportTitle, sharedReportName);

            //Share a Report
            test.ReportLibrary.Report_Share(sharedReportName, new string[] { "FT Tester", "Suchitra Patnam" });

            //Delete Report
            test.ReportLibrary.Report_Delete(sharedReportName);

            //Logout
            test.ReportLibrary.Logout();


            //Login to Report library  as Receipient to view report 

            test.ReportLibrary.Login("ft.tester", "FT4life!", "DC");

            //Go to my_reports_tab
            test.GeneralMethods.WaitForElement(By.Id("my_reports_tab"));
            test.Driver.FindElementById("my_reports_tab").Click();


            //Verify that shared report is present
            //Assert.IsTrue(test.Driver.FindElementByLinkText(sharedReportName).Displayed, string.Format("Report [{0}] was not shared", sharedReportName));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(sharedReportName));


            //Verify Received From: FT Tester on Jun 4, 2014
            test.ReportLibrary.Report_Verify_Share(sharedReportName, userNameToDisplay, sharedDate);
            //test.ReportLibrary.Report_Verify_Share(sharedReportName, "Matthew Sneeden", sharedDate);


            //Delete Report
            test.ReportLibrary.Report_Delete(sharedReportName);

            // Logout of report library
            test.ReportLibrary.Logout();



        }
               
        #endregion Share Report

        #region Interactive Reporting

        [Test, RepeatOnFailure, DoesNotRunInStaging]
        [Author("Demi Zhang")]
        [Description("FO-3817 Present Interactive Reporting link in Portal UI")]
        public void Report_InteractiveReporting_PreviewUser()
        {
            //Login Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            // Verify the "Interactive Reporting" is exist            
            Assert.IsTrue(test.GeneralMethods.IsElementExist(By.XPath("*//*/div/ul/*/a[@class='menu_new_window'][text()='Interactive Reports']")));

            var handler = test.Driver.WindowHandles[0];

            // Click Interactive Reporting
            test.Driver.FindElementByXPath("*//*/div/ul/*/a[@class='menu_new_window'][text()='Interactive Reports']").Click();

            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]);

            //Verify the Jasper Reporting page is loaded and user can view the Public Folder, folder Public Folder/ActiveFaith/Giving Reports is being shown.
            System.Threading.Thread.Sleep(5000);

            //Assert.IsTrue(test.GeneralMethods.IsElementExist(By.Id("anonymous_element_2")));
            Assert.IsTrue(test.GeneralMethods.IsElementExist(By.XPath("*//div//ul/li/ul/li/p[text()='Public Folder']")));

            test.Driver.FindElement(By.XPath("*//*/div//ul/li/ul/li/p[text()='Public Folder']/b")).Click();
            Assert.IsTrue(test.GeneralMethods.IsElementExist(By.XPath("*//*/div/*/ul/li/ul/li/ul/li/p[text()='ActiveFaith']")));

            test.Driver.FindElement(By.XPath("*//*//div/*//ul/li/ul/li/ul/li/p[text()='ActiveFaith']/b")).Click();
            Assert.IsTrue(test.GeneralMethods.IsElementExist(By.XPath("*//*//div/*//ul/li/ul/li/ul/li/ul/li/p[text()='Giving Reports']")));

            // verify the user shown Jasper is the same as the one logged in Portal
            Assert.AreEqual("fo_ft.tester", test.Driver.FindElementById("userID").Text);

            //logout and close Japser Reporting
            test.Driver.FindElementById("main_logOut_link").Click();
            test.Driver.Close();

            // Switch to portal
            test.Driver.SwitchTo().Window(handler);   
           
            // logout of portal
            test.Portal.LogoutWebDriver();
            
        }


        [Test, RepeatOnFailure, DoesNotRunInStaging]
        [Author("Demi Zhang")]
        [Description("FO-3817 Present Interactive Reporting link in Portal UI")]
        public void Report_InteractiveReporting_NotPreviewUser()
        {
            //Login Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("jaspertest", "Jasper!test1", "dc");

            // Verify the "Interactive Reporting" is not displayed
            //test.GeneralMethods.WaitForElementNotDisplayed(By.XPath("*//*/div/ul/*/a[@class='menu_new_window'][text()='Interactive Reports']"));

            bool isJasperExist = test.GeneralMethods.IsElementExist(By.XPath("*//*/div/ul/*/a[@class='menu_new_window'][text()='Interactive Reports']"));
            Assert.IsFalse(isJasperExist);
            
            // logout of portal
            test.Portal.LogoutWebDriver();
            
        }

        #endregion Interactive Reporting
    }
}
