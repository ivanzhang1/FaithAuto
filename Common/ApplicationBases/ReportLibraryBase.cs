using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System.Threading;

namespace FTTests {
    public class ReportLibraryBase {
        private RemoteWebDriver _driver;
        private Selenium.ISelenium _selenium;
        private string _reportLibraryUserName;
        private string _reportLibraryPassword;
        private string _reportLibraryChurchCode;
        private string _url;
        private GeneralMethods _generalMethods;

        #region Properties
        public string ReportLibraryUserName {
            set { _reportLibraryUserName = value; }
            get { return _reportLibraryUserName; }
        }

        public string ReportLibraryPassword {
            set { _reportLibraryPassword = value; }
            get { return _reportLibraryPassword; }
        }

        public string ReportLibraryChurchCode {
            set { _reportLibraryChurchCode = value; }
            get { return _reportLibraryChurchCode; }
        }

        public string URL {
            set { _url = value; }
            get { return _url; }
        }
        #endregion Properties

        public ReportLibraryBase(RemoteWebDriver driver, GeneralMethods generalMethods) {
            this._driver = driver;
            this._generalMethods = generalMethods;
        }

        public ReportLibraryBase(Selenium.ISelenium selenium, GeneralMethods generalMethods) {
            this._selenium = selenium;
            this._generalMethods = generalMethods;
        }

        #region Instance Methods
        /// <summary>
        /// This method logs into the report library application using default credentials.
        /// </summary>
        public void Login() {
            this.DoLogin(this._reportLibraryUserName, this._reportLibraryPassword, this._reportLibraryChurchCode);
        }

        /// <summary>
        /// Logs in to Report Library with specific creds.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        /// <param name="churchCode">The church code.</param>
        public void Login(string userName, string password, string churchCode) {
            this.DoLogin(userName, password, churchCode);
        }

        /// <summary>
        /// Attempts to login to report library.
        /// </summary>
        /// <param name="userName">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="churchCode">Church code.</param>
        private void DoLogin(string userName, string password, string churchCode) {


            if (!this._driver.Url.ToString().Contains(this._url))
            {
                // Open the report library login page
                this._driver.Navigate().GoToUrl(this._url);
            }
            else
            {
               TestLog.WriteLine("No need to Navigate to Login");
               System.Threading.Thread.Sleep(5000);
            }

            // Enter credentials
            this._driver.FindElementById("username").Clear();
            this._driver.FindElementById("username").SendKeys(userName);
            
            this._driver.FindElementById("password").Clear();
            this._driver.FindElementById("password").SendKeys(password);

            this._driver.FindElementById("churchcode").Clear();
            this._driver.FindElementById("churchcode").SendKeys(churchCode);

            // Attempt to login
            this._driver.FindElementById("btn_login").Submit();
            //this._driver.FindElementById("btn_login").Click();

            // Accept the Compliance Cookie if needed.
            if (churchCode.ToUpper() != "DC")
            {
                if (this._driver.FindElementsByXPath("//input[@value='Allow Cookies']").Count > 0)
                {
                    // We need to accept cookies
                    this._driver.FindElementByXPath("//input[@value='Allow Cookies']").Click();

                    // Verify the cookie is present
                    Assert.IsNotNull(this._driver.Manage().Cookies.GetCookieNamed("ComplianceCookie"), "Cookies were accepted but the compliance cookie was not present!");
                }
                else
                {
                    // Verify the compliance cookie is present
                    if (this._reportLibraryChurchCode.ToLower() == "qaeunlx0c6")
                    {
                        Assert.IsNotNull(this._driver.Manage().Cookies.GetCookieNamed("ComplianceCookie"), "There was no prompt to accept cookies but the Compliance cookie was not present!!");
                    }
                }
            }
        }

        /// <summary>
        /// This method logs out of the report library application.
        /// </summary>
        public void Logout() {
            this._driver.FindElementByLinkText("Log Out").Click();
            this._generalMethods.WaitForElement(By.Id("username"), 60);
        }
        ///<summary>
        ///Create My Reports

        public string AddReportTo_MyReports(string reportName, string reportCodeSearch, string reportCode)
        {

          // SQL.ReportLibrary_Queue_DeleteItem(15, 65211, reportName);
                        
            MyReport_Delete(reportCode);

            // Search for and select a report and add to My Reports
            this._driver.FindElementByXPath("//input[@name='q']").SendKeys(reportCodeSearch);
            this._driver.FindElementByXPath("//input[@value='Search']").Click();
            this._generalMethods.WaitForElement(this._driver, By.LinkText(reportName));

            this._driver.FindElementByLinkText(reportName).Click();
           // System.Threading.Thread.Sleep(5000);

            //wait for element btnSaveToReports...
            this._generalMethods.WaitForElement(By.Id("btnSaveToMyReports"));
            this._driver.FindElementById("btnSaveToMyReports").Click();
            //wait for element....and change id...


            //I tried using ID instead of Xpath and  it did not work so canging back to Xpath..

            //this._driver.FindElementById("id='submitQuery']").Click();
            this._driver.FindElementByXPath("//input[@id='submitQuery']").Click();

            //Verify Report added to My Reports

            this._driver.FindElementByPartialLinkText("My Reports").Click();

            this._generalMethods.VerifyTextPresentWebDriver(reportName);           

            return reportName;
        }

        public void MyReport_Delete(string reportCode)
        {

           // Delete a Report from My Reports
            string MyReportsTable = reportCode;

            int itemRow = this._generalMethods.GetTableRowNumberWebDriver(TableIds.ReportLibrary.Myreport, MyReportsTable, "Code", null, false);
            TestLog.WriteLine(itemRow);
            if (itemRow != 0)
            {
                    this._driver.FindElementById(TableIds.ReportLibrary.Myreport).FindElement(By.XPath(string.Format("//tr[{0}]/td[4]/form/a", itemRow + 1))).Click();
                    this._driver.SwitchTo().Alert().Accept();
                
            }
            
            //Assert.IsFalse(this._generalMethods.ItemExistsInTableWebDriver(TableIds.ReportLibrary.Myreport, MyReportsTable, "Code"));

            

            //Retry.WithPolling(500).WithTimeout(20000).WithFailureMessage("After Delete Report")
             //       .Until(() => this._generalMethods.IsElementPresentWebDriver(By.LinkText("Build a New Report")));
            //this._generalMethods.WaitForElement(By.LinkText("Build a New Report"));
        }
        
    
        /// <summary>
        /// Creates a label style.
        /// </summary>
        /// <param name="labelStyleName">The name of the label style.</param>
        /// <param name="availableToAllUsers">Flag determining availability for the label style.</param>
        /// <param name="active">Flag determining active/inactive status for the label style.</param>
        public void LabelStyles_Create(string labelStyleName, bool availableToAllUsers, bool active) {
            // Navigate to the label style creation page
            this._driver.FindElementByLinkText("Label Styles").Click();
            this._driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(2500));
            IWebElement newLabelStyleButton = this._driver.FindElementByXPath("//a[img[@alt='New Label Style']]");
            newLabelStyleButton.Click();

            // Create the label style
            this._driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(2500));
            IWebElement nameElement = this._driver.FindElementById("LabelFormat[FormatName]");
            nameElement.SendKeys(labelStyleName);

            new SelectElement(this._driver.FindElementById("label_templates")).SelectByText("5160 - Mailing Labels");
            SelectElement selectNameFormat = new SelectElement(this._driver.FindElementById("LabelFormat[NameFormatID]"));
            Retry.WithPolling(500).WithTimeout(5000).Until(() => selectNameFormat.SelectedOption.Text == "Joshua Randall Jr.");

            if (!availableToAllUsers) {
                this._driver.FindElementById("ShareLabelFormat").Click();
            }

            if (!active) {
                this._driver.FindElementByXPath("//input[@name='LabelFormat[IsActive]' and @value='False']").Click();
            }

            DateTime submitTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            this._driver.FindElementById("submitQuery").Click();

            // Verify the label style was created
            string uniqueIdentifier = availableToAllUsers ? string.Format("{0}\r\nAvailable to all users", labelStyleName) : labelStyleName;
            int itemRow = this._generalMethods.GetTableRowNumberWebDriver(TableIds.ReportLibrary.LabelStyles, uniqueIdentifier, "Label Style Name");

            IWebElement element = active ? this._driver.FindElementById(TableIds.ReportLibrary.LabelStyles).FindElement(By.XPath(string.Format("//tr[{0}]/td[2]/img[contains(@src, '/ReportLibrary/public/images/tick.png?')]", itemRow + 1))) : this._driver.FindElementById(TableIds.ReportLibrary.LabelStyles).FindElement(By.XPath(string.Format("//tr[{0}]/td[position()=2 and not(img[contains(@src, '/ReportLibrary/public/images/tick.png?')])]", itemRow + 1)));
            Assert.IsTrue(element.Enabled);
            Assert.Between(this._driver.FindElementById(TableIds.ReportLibrary.LabelStyles).FindElement(By.XPath(string.Format("//tr[{1}]/td[3]", TableIds.ReportLibrary.LabelStyles, itemRow + 1))).Text, string.Format("Today at {0:t}", submitTime.AddMinutes(-1)), string.Format("Today at {0:t}", submitTime.AddMinutes(1)));
        }

        /// <summary>
        /// Search For A report By Code
        /// </summary>
        /// <param name="reportCodeSearch">Report Code</param>
        /// <param name="reportName">Report Name</param>
        public void Report_Search_By_Code(string reportCodeSearch, string reportName)
        {
            this._driver.FindElementByXPath("//input[@name='q']").SendKeys(reportCodeSearch);
            this._driver.FindElementByXPath("//input[@value='Search']").Click();
            this._generalMethods.WaitForElement(this._driver, By.LinkText(reportName));
        }

        /// <summary>
        /// Go to a Report using report name link text 
        /// </summary>
        /// <param name="reportName">Report Name</param>
        public void Report_GoTo_Report(string reportName)
        {
            //this._driver.FindElementById("my_reports_tab").Click();
            this._driver.FindElementByLinkText(reportName).Click();
            this._generalMethods.WaitForElement(this._driver, By.Id("tb_CustomHeader"));
        }

        /// <summary>
        /// Save Report To My Reports [NOTICE: Report is just saved. Required specific fields are not setup]
        /// </summary>
        /// <param name="reportTitle">Save Report Title</param>
        /// <param name="saveReportName">Save Report Name</param>
        /// <param name="saveDescription">Save Report </param>
        /// <param name="saveNote">Save Report Note</param>
        /// <param name="saveTags">Save Report Tags</param>
        public void Report_Save_MyReports(string reportTitle, string saveReportName, string saveDescription = "", string saveNote = "", string saveTags = "")
        {
            //Enter Report Title
            this._driver.FindElementById("tb_CustomHeader").Clear();
            this._driver.FindElementById("tb_CustomHeader").SendKeys(reportTitle);

            //TODO Enter Report Subtitle
            //tb_SubHeader
            //this._driver.FindElementById("tb_SubHeader").Clear();
            //this._driver.FindElementById("tb_SubHeader").SendKeys(reportSubTitle);

            //Click Save Report
            //SELECT [REPORT_ID] FROM [ChmChurch].[dbo].[REPORT] WHERE REPORT_CODE = 'M1400 v2.6'
            //href="/ReportLibrary/MyReports/New.aspx?id=3203"
            this._driver.FindElementById("btnSaveToMyReports").Click();
            this._generalMethods.WaitForElement(By.Id("UserReportDefinition[ReportName]"), 30, "Waiting for My Report Page");

            //Save to My Report Page
            //Clear and Enter Name            
            this._driver.FindElementById("UserReportDefinition[ReportName]").Clear();
            this._driver.FindElementById("UserReportDefinition[ReportName]").SendKeys(saveReportName);

            //Enter Description
            this._driver.FindElementById("UserReportDefinition[Description]").SendKeys(saveDescription);

            //Enter Note
            this._driver.FindElementById("UserReportDefinition[Note]").SendKeys(saveNote);

            //Enter Tag
            this._driver.FindElementById("UserReportDefinition[ReportTags]").SendKeys(saveTags);

            //Save
            this._driver.FindElementById("submitQuery").Click();

            //Wait for tb_CustomHeader
            this._generalMethods.WaitForElement(By.Id("tb_CustomHeader"), 30, "Waiting for Saved Report");

            //Verify that Report Header value is correct
            //<div class="form_header_text">
            //<h1>Save Assignment Records 06052014</h1>
            //</div>
            Assert.AreEqual(saveReportName, this._driver.FindElement(By.XPath("//div[@class='form_header_text']")).FindElement(By.TagName("h1")).Text.Trim());

            //Verify that Report Name is saved correctly
            Assert.AreEqual(saveReportName, this._driver.FindElement(By.XPath("//input[@id='tb_CustomHeader']")).GetAttribute("value").Trim());

            //Return to Reports
            this._driver.FindElementByLinkText("Return to My Reports").Click();
            this._generalMethods.WaitForElement(By.Id("my_reports_tab"), 30, "Waiting to go to My Reports Tab");

            //Verify that Saved Reports shows up.
            Assert.IsTrue(this._driver.FindElementByLinkText(saveReportName).Displayed, string.Format("{0} does not appear in my reports", saveReportName));

        }

        /// <summary>
        ///  F1-3650: Share A Report with Individuals
        /// </summary>
        /// <param name="reportName">Report Name that will be shared</param>
        /// <param name="sharedReportName">Shared Report Name</param>
        /// <param name="sharedUserNames">Shared Users</param>
        /// <param name="sharedReportDescription">Shared Description</param>
        /// <param name="sharedReportNote">Shared Report Note</param>
        /// <param name="sharedReportTags">Shared Report Tags</param>
        public void Report_Share(string sharedReportName, string[] sharedUserNames, 
            string sharedReportDescription = "", string sharedReportNote = "", string sharedReportTags = "")
        {

            this.Report_GoTo_Report(sharedReportName);

            //Click on Link
            this._driver.FindElementByLinkText("Send a Copy").Click();

            //Wait for Input Report Name
            this._generalMethods.WaitForElement(By.Id("UserReportDefinition[ReportName]"), 30, "Waiting for Share Report Page");

            //Enter Share Report Name
            this._driver.FindElementById("UserReportDefinition[ReportName]").Clear();
            this._driver.FindElementById("UserReportDefinition[ReportName]").SendKeys(sharedReportName);

            //UserReportDefinition[Description]
            this._driver.FindElementById("UserReportDefinition[Description]").Clear();
            this._driver.FindElementById("UserReportDefinition[Description]").SendKeys(sharedReportDescription);


            //UserReportDefinition[Note]
            this._driver.FindElementById("UserReportDefinition[Note]").Clear();
            this._driver.FindElementById("UserReportDefinition[Note]").SendKeys(sharedReportNote);

            //UserReportDefinition[ReportTags]
            this._driver.FindElementById("UserReportDefinition[ReportTags]").Clear();
            this._driver.FindElementById("UserReportDefinition[ReportTags]").SendKeys(sharedReportTags);

            //Select User to Share
            foreach (string sharedUserName in sharedUserNames)
              new SelectElement(this._driver.FindElementById("selectedUsers")).SelectByText(sharedUserName);

            //Share
            this._driver.FindElement(By.XPath("//input[@id='submitQuery' and @value='Share']")).Click();

            //User will be taken back to report
            this._generalMethods.WaitForElement(By.LinkText("Return to My Reports"));
            this._driver.FindElementByLinkText("Return to My Reports").Click();

            //Wait for my_reports_tab
            this._generalMethods.WaitForElement(By.Id("my_reports_tab"), 30, "Waiting for My Shared Reports View");

        }

        /// <summary>
        /// Verify Share Report Information
        /// </summary>
        /// <param name="sharedReportName"></param>
        /// <param name="sharedIndividualName"></param>
        /// <param name="sharedDate"></param>
        public void Report_Verify_Share(string sharedReportName, string sharedIndividualName, string sharedDate)
        {

            int sharedRowFound = 0;
            IWebElement reportTbl = this._driver.FindElementById("ctl00_ctl00_MainContent_LeftColumn_grdReports");
            int rows = reportTbl.FindElements(By.TagName("tr")).Count;

            //we are starting with 1 because position 0 is header
            for (int tr = 1; tr < rows; tr++)
            {
                if (reportTbl.FindElements(By.TagName("tr"))[tr].FindElement(By.TagName("h5")).Text == sharedReportName)
                {
                    sharedRowFound = tr;
                    break;
                }
            }

            //If found  Verify else fail
            if (sharedRowFound > 0)
            {

                string rcvdFrom = reportTbl.FindElements(By.TagName("tr"))[sharedRowFound].FindElements(By.TagName("td"))[1].FindElements(By.TagName("p"))[1].Text;
                Assert.AreEqual(string.Format("Received From: {0} on {1}", sharedIndividualName, sharedDate), rcvdFrom);

            }
            else
            {
                throw new WebDriverException("Received From Information was not present");
            }

        }

        /// <summary>
        /// Delete a report within the report
        /// </summary>
        /// <param name="reportName"></param>
        public void Report_Delete(string reportName)
        {
            //Go to Report
            this.Report_GoTo_Report(reportName);

            //Delete Report
            this._driver.FindElementByLinkText("Delete this report").Click();

            //Click ok OK Message
            this._driver.SwitchTo().Alert().Accept();

            //Verify Report is not present using link Text
            this._generalMethods.VerifyElementNotPresentWebDriver(By.LinkText(reportName));
            

        }


        #endregion Instance Methods

    }
   
}
