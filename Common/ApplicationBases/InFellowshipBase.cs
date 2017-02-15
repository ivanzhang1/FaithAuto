using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Globalization;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using DeathByCaptcha;
using Selenium;
using Gallio.Framework;
using MbUnit.Framework;
using Gallio.Framework.Pattern;

using log4net;

using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using System.Collections;

namespace FTTests {
    public class InFellowshipBase {
        private RemoteWebDriver _driver;
        private ISelenium _selenium;
        private string _infellowshipEmail;
        private string _infellowshipUser;
        private string _infellowshipPassword;
        private string _infellowshipChurchCode;
        private int _infellowshipChurchID;
        private F1Environments _f1Environment;
        private GeneralMethods _generalMethods;
        private IList<string> _errorText = new List<string>();
        private JavaScript _javascript;
        private SQL _sql;
        

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Properties
        public string InFellowshipUser {
            set { _infellowshipUser = value; }
            get { return _infellowshipUser; }
        }

        public string InFellowshipEmail {
            set { _infellowshipEmail = value; }
            get { return _infellowshipEmail; }
        }

        public string InFellowshipPassword {
            set { _infellowshipPassword = value; }
            get { return _infellowshipPassword; }
        }

        public string InFellowshipChurchCode {
            set { _infellowshipChurchCode = value; }
            get { return _infellowshipChurchCode; }
        }

        public int InFellowshipChurchID
        {
            set { _infellowshipChurchID = value; }
            get { return _infellowshipChurchID; }
        }

        public string URL {
            get { return this.GetInFellowshipURL(); }
        }

        public F1Environments InFellowshipEnvironment
        {
            set { _f1Environment = value; }
            get { return _f1Environment; }
        }

        #endregion Properties

        public InFellowshipBase(ISelenium selenium, F1Environments f1Environment, GeneralMethods generalMethods, JavaScript javascript, SQL sql) {
            this._selenium = selenium;
            this._f1Environment = f1Environment;
            this._generalMethods = generalMethods;
            this._javascript = javascript;
            this._sql = sql;

        }

        public InFellowshipBase(RemoteWebDriver driver, F1Environments f1Environment, GeneralMethods generalMethods, JavaScript javascript, SQL sql) {
            this._driver = driver;
            this._f1Environment = f1Environment;
            this._generalMethods = generalMethods;
            this._javascript = javascript;
            this._sql = sql;

        }

        #region Instance Methods

        #region Culture

        public CultureInfo Culture_Settings(int churchId)
        {
            // Store culture for date settings
            CultureInfo culture = null;
            if (churchId.ToString() == "258")
            {
                culture = new CultureInfo("en-GB");
            }
            else
            {
                culture = new CultureInfo("en-US");
            }

            return culture;

        }

        #endregion Culture

        #region Open / Login / Logout
        /// <summary>
        /// Navigates to the InFellowship website.  Uses username, password, and church code specified specified in App.config.
        /// </summary>
        public void Open() {

            // Open the web page
            this._selenium.Open(this.GetInFellowshipURL());


//            TestLog.WriteLine("Allow Cookies Visible: " + this._selenium.IsVisible("//input[@value='Allow Cookies']"));

            // Accept the Compliance Cookie if needed.
                if (this._selenium.IsElementPresent("//input[@value='Allow Cookies']"))
                {
                    // We need to accept cookies
                    this._selenium.ClickAndWaitForPageToLoad("//input[@value='Allow Cookies']");

                    // Verify the cookie is present
                    Assert.IsTrue(this._selenium.GetCookie().Contains("ComplianceCookie"), "Cookies were accepted by the compliance cookie was not present!");
                }
                else
                {
                    // Verify the compliance cookie is present
                    if (this._infellowshipChurchCode.ToLower() == "qaeunlx0c6")
                    {
                        Assert.IsTrue(this._selenium.GetCookie().Contains("ComplianceCookie"), "There was no prompt to accept cookies but the Compliance cookie was not present!!");
                    }
                }            
            
        }

        public void Refresh()
        {
            TestLog.WriteLine("Refresh ....");
            this._selenium.Refresh();
        }

        /// <summary>
        /// Logs the user in to InFellowship. Specific to a church code.  Uses username and password specified in App.config.
        /// </summary>
        /// <param name="churchCode">Church code to use for InFellowship.</param>
        public void Open(string churchCode) {

            TestLog.WriteLine("Open ... " + churchCode);

            // Visit portal first to bypass the SHA2 certification. This is a workaround.
            this._selenium.Open("https://portal.qa.fellowshipone.com");

            // Open the web page
            this._infellowshipChurchCode = churchCode;
            string infellowURL = this.GetInFellowshipURL();
            TestLog.WriteLine("URL: " + infellowURL);
            this._selenium.Open(infellowURL);
            TestLog.WriteLine("Wait for Page to load ...");
            this._selenium.WaitForPageToLoad("120000");

            // Accept the Compliance Cookie if needed.
            // *[@id='submitQuery']
            if (this._selenium.IsElementPresent("//input[@value='Allow Cookies']")) {
                // We need to accept cookies
                this._selenium.ClickAndWaitForPageToLoad("//input[@value='Allow Cookies']");

                // Verify the cookie is present
                Assert.IsTrue(this._selenium.GetCookie().Contains("ComplianceCookie"), "Cookies were accepted by the compliance cookie was not present!");
            }
            else {
                // Verify the compliance cookie is present
                if (this._infellowshipChurchCode.ToLower() == "qaeunlx0c6") {
                    Assert.IsTrue(this._selenium.GetCookie().Contains("ComplianceCookie"), "There was no prompt to accept cookies but the Compliance cookie was not present!!");
                }
            }
        }

        /// <summary>
        /// Logs the user in to InFellowship. Specific to a church code.  Uses username and password specified in App.config.
        /// </summary>
        /// <param name="churchCode">Church code to use for InFellowship.</param>
        public void OpenWebDriver(string churchCode = "QAEUNLX0C2")
        {

            TestLog.WriteLine("Open ... " + churchCode);

            // Open the web page
            this._infellowshipChurchCode = churchCode;
            TestLog.WriteLine("URL: " + this.GetInFellowshipURL());
            this._driver.Navigate().GoToUrl(this.GetInFellowshipURL());
            TestLog.WriteLine("Wait for Page to load ...");
            // this._selenium.WaitForPageToLoad("120000");

            if (churchCode.ToLower() == "qaeunlx0c6")
            {
                // Accept the Compliance Cookie if needed.
                if (this._generalMethods.IsElementPresentWebDriver(By.XPath("//input[@value='Allow Cookies']")))
                {
                    // We need to accept cookies
                    this._driver.FindElementByXPath("//input[@value='Allow Cookies']").Click();

                    // Verify the cookie is present
                    Assert.IsNotNull(this._driver.Manage().Cookies.GetCookieNamed("ComplianceCookie"), "Cookies were accepted but the compliance cookie was not present!");
                }
                else
                {
                    // Verify the compliance cookie is present
                    if (churchCode.ToLower() == "qaeunlx0c6")
                    {
                        Assert.IsNotNull(this._driver.Manage().Cookies.GetCookieNamed("ComplianceCookie"), "There was no prompt to accept cookies but the Compliance cookie was not present!!");
                    }
                }
            }
        }

        public void OpenExpectingNoAccess(string url) {
            // Attempt to navigate to a url, expecting to be redirected to the 'NoAccess' page
            this._selenium.Open(url);

            // Verify user is taken to the 'NoAccess' page
            Assert.IsTrue(this._selenium.GetLocation().Contains("/GroupApp/NoAccess"));
            Assert.IsTrue(this._selenium.IsTextPresent("Access Denied"));
            Assert.IsTrue(this._selenium.IsTextPresent("Note: You do not have the proper credentials to view the requested page."));

            // Return to the home page
            this._selenium.ClickAndWaitForPageToLoad("link=BACK TO HOME");
        }

        /// <summary>
        /// Logs the user in to InFellowship. Specific to an InFellowship user and password.  Uses church code specified in App.config.
        /// </summary>
        /// <param name="infellowshipAccount">InFellowship account to log in with.</param>
        /// <param name="password">Password for the specified account.</param>
        public void Login(string infellowshipAccount, string password) {
            DoLoginInFellowship(infellowshipAccount, password, this._infellowshipChurchCode);
        }

        /// <summary>
        /// Logs the user in to InFellowship.  Specific to an InFellowship user, password, and church code.
        /// </summary>
        /// <param name="infellowshipAccount">InFellowship account to log in with.</param>
        /// <param name="password">Password for the specified account.</param>
        /// <param name="churchCode">Church code to use for InFellowship.</param>
        public void Login(string infellowshipAccount, string password, string churchCode) {
            DoLoginInFellowship(infellowshipAccount, password, churchCode);
        }

        /// <summary>
        /// Re Login to Infellowship due logout security enhancements.
        /// F1-4374 SECURITY: Forms Authentication - Sign Out
        /// </summary>
        /// <param name="username">Email Address</param>
        /// <param name="password">Password</param>
        /// <param name="churchCode">Church Code</param>
        public void ReLoginInFellowship(string username, string password, string churchCode)
        {

            if (this._selenium.IsElementPresent("username"))
            {
                TestLog.WriteLine("Re Infellowship Login: " + username + "/" + password + "/" + churchCode);

                //Open the web page
                this.Open(churchCode);

                //If we have to pass in creds, do so else most likely we are good to continue 
                if (this._selenium.IsElementPresent("username"))
                {
                    this._selenium.Type("username", username);
                    this._selenium.Type("password", password);

                    // Login, wait for the page to load
                    this._selenium.ClickAndWaitForPageToLoad("btn_login");
                }
            }
        }


        /// <summary>
        /// Logs the user in to InFellowship.  Specific to an InFellowship user, password, and church code.
        /// </summary>
        /// <param name="infellowshipAccount">InFellowship account to log in with.</param>
        /// <param name="password">Password for the specified account.</param>
        /// <param name="churchCode">Church code to use for InFellowship.</param>
        public void LoginWebDriver(string infellowshipAccount, string password, string churchCode = "DC") {
            // Set the username
            if (!string.IsNullOrEmpty(infellowshipAccount)) {
                this._infellowshipEmail = infellowshipAccount;
            }

            // Set the password
            if (!string.IsNullOrEmpty(password)) {
                this._infellowshipPassword = password;
            }

            // Set the church code
            if (!string.IsNullOrEmpty(churchCode)) {
                this._infellowshipChurchCode = churchCode;
            }

            TestLog.WriteLine("Infellowship Login: " + infellowshipAccount + "/" + password + "/" + churchCode);            

            // Open the web page
            if (string.IsNullOrEmpty(churchCode)) {
                TestLog.WriteLine("Open DC Church");
                this.OpenWebDriver("DC");
            }
            else {
                TestLog.WriteLine("Open " + churchCode + " Church");
                this.OpenWebDriver(churchCode);
            }

            //Are we in login page
            TestLog.WriteLine("Username Present: " + this._generalMethods.IsElementPresentWebDriver(By.Id("username")));
            Assert.IsTrue(this._generalMethods.IsElementPresentWebDriver(By.Id("username")));
            TestLog.WriteLine("Password Present: " + this._generalMethods.IsElementPresentWebDriver(By.Id("password")));
            Assert.IsTrue(this._generalMethods.IsElementPresentWebDriver(By.Id("password")));

            // Type the username            
            if (!string.IsNullOrEmpty(infellowshipAccount)) {
                this._driver.FindElementById("username").SendKeys(infellowshipAccount);
            }
            else {
                this._driver.FindElementById("username").SendKeys(this._infellowshipEmail);
            }

            // Type the password
            if (!string.IsNullOrEmpty(password)) {
                this._driver.FindElementById("password").SendKeys(password);
            }
            else {
                this._driver.FindElementById("password").SendKeys(this._infellowshipPassword);
            }

            // Login, wait for the page to load
            this._driver.FindElementById("btn_login").Click();
            this._generalMethods.WaitForElement(this._driver, By.LinkText("FIND A GROUP"), 30, "Login Error");

            this._infellowshipChurchCode = churchCode;
            this._infellowshipChurchID = this._sql.FetchChurchID(this._infellowshipChurchCode);

        }

        /// <summary>
        /// Re Login to Infellowship due logout security enhancement.
        /// F1-4374 SECURITY: Forms Authentication - Sign Out
        /// </summary>
        /// <param name="infellowshipAccount">Infellowship Email</param>
        /// <param name="password">Password</param>
        /// <param name="churchCode">Church Code</param>
        public void ReLoginWebDriver(string infellowshipAccount, string password, string churchCode)
        {

            TestLog.WriteLine("Re Infellowship Login: " + infellowshipAccount + "/" + password + "/" + churchCode);
            this.OpenWebDriver(churchCode);

            //Are we in login page
            if (this._generalMethods.IsElementPresentWebDriver(By.Id("username")))
            {
                this._driver.FindElementById("username").SendKeys(infellowshipAccount);
                this._driver.FindElementById("password").SendKeys(password);
                this._driver.FindElementById("btn_login").Click();
            }



        }


        /// <summary>
        /// Cause Infellowship Account to max out login attempts
        /// </summary>
        /// <param name="userEmail">user email address or phone number</param>
        /// <param name="password">password</param>
        /// <param name="churchCode">church code</param>
        public void LoginFailMaxAttemptsWebDriver(string userEmail, string password, string churchCode = "DC")
        {

            int maxLoginAttempts = 3;

            for (int x = 0; x < maxLoginAttempts; x++)
            {
                this.LoginWebDriver(userEmail, password, churchCode);

                // Verify login attempt failed
                if (x == (maxLoginAttempts - 1))
                {
                    Assert.AreEqual("Your account has been locked out and you will not be able to login until you reset your password.", this._driver.FindElementById("login_failed").Text.Trim(), "Failed Login error message invalid");
                }
                else
                {
                    Assert.AreEqual("Login attempt failed. Verify your information and try again. Your account will be locked out after 3 unsuccessful attempts.", this._driver.FindElementById("login_failed").Text.Trim(), "Failed Login error message invalid");
                }
            }


        }

        /// <summary>
        /// Login to Infellowship Event Registration Form
        /// </summary>
        /// <param name="churchId">Church Id</param>
        /// <param name="username">User Name</param>
        /// <param name="password">Password</param>
        /// <param name="formName">Form Name</param>
        public void Login_Event_Registration_Form(string formName, int churchId = 15, string username = "ft.autotester@gmail.com", string password = "FT4life!")
        {

            //Get Infellowship Registration link
            string linkText = this.Get_Infellowship_EventRegistration_Form_URL(formName, churchId);

            //Login Infellowhship Event Registration
            this._generalMethods.OpenURLWebDriver(linkText);

            //For European churches Allow cookies
            if (this._generalMethods.IsTextPresentWebDriver("Allow Cookies"))
            {
                this._driver.FindElement(By.Id("submitQuery")).Click();
            }

            //If User needs to login then login else we have already logged in
            if (this._generalMethods.IsElementPresentWebDriver(By.Id("username")))
            {
                this._driver.FindElement(By.Id("username")).SendKeys(username);
                this._driver.FindElement(By.Id("password")).SendKeys(password);
                this._driver.FindElement(By.Id("btn_login")).Click();
            }
            
            

        }

        /// <summary>
        /// This method is used to get confirmation code from Individual_Set_id table
        /// </summary>
        /// <param name="url"></param>
        /// <returns> Confirmation code</returns>

       public string GetConfirmationCode(string url, int churchId)
        {
           TestLog.WriteLine("URL : {0}", url);
           // Modified by Mady after URL format is changed in reg confirmation page
           // string[] urlString = url.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
           string[] urlString = url.Split(new string[] { "?" }, StringSplitOptions.None)[0].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
           int individual_set_id = -1;
           for (int i = urlString.Length - 1; i >= 0; i--)
           {
               try
               {
                   individual_set_id = Convert.ToInt32(urlString[i]);
                   // Add below sql call to ensure above individual id is real rather than form id
                   this._sql.Weblink_InfellowshipForm_GetConfirmationCode(churchId, individual_set_id);
                   break;
               }
               catch (System.FormatException) { }
               catch (System.IndexOutOfRangeException) { }
           }

           Assert.IsFalse(individual_set_id == -1, "Fail to find valid individual set id from url");
           TestLog.WriteLine("individual set id : {0}", individual_set_id);
           string confirmationCode = this._sql.Weblink_InfellowshipForm_GetConfirmationCode(churchId, individual_set_id);

           return confirmationCode;                       

        } 

        public void Groups_Attendance_Enter_All_Webdriver(string groupName, string scheduleDate, bool didGroupMeet)
        {
            
            // View the group's attendance page
       
            this.Groups_Attendance_View_WebDriver(groupName);

            this._driver.FindElementByLinkText("Attendance").Click();
            // Enter attendance
            //this._selenium.ClickAndWaitForPageToLoad("link=Enter attendance");
            this._driver.FindElementByPartialLinkText("Enter attendance").Click();
            


            // Variables
            var attendanceDate = scheduleDate;
            var attendanceDateNoDayEndTime = string.Empty;
            var attendanceDateNoDayNoEndTime = string.Empty;

            // If no date is specified, use the first date in the drop down
            if (string.IsNullOrEmpty(scheduleDate))
            {
                // Store the first date in the drop down
                //attendanceDate = this._selenium.GetText("//select[@id='post_attendance']//optgroup[@label='Choose an event']/option[1]");
                attendanceDate = this._driver.FindElementByXPath("//*[@id='post_attendance']/optgroup[1]/option[1]/text()").GetAttribute("value");
            }

            // Select the date
            this._driver.FindElementByXPath("//*[@id='post_attendance']/optgroup[1]/option[1]").Click();
            //this._selenium.SelectAndWaitForCondition("post_attendance", attendanceDate, this._javascript.IsElementPresent("attendance_roster"), "10000");

            // Regex the date for various purposes
            attendanceDateNoDayEndTime = Regex.Replace(attendanceDate, "(Satur|Sun|Mon|Tues|Wednes|Thurs|Fri){1}(day, )", string.Empty);
            attendanceDateNoDayNoEndTime = Regex.Replace(attendanceDateNoDayEndTime, "(\\s? - \\s?[0-9]?[0-9]:[0-9][0-9]\\s[A|P]M)", string.Empty);


            // If the group met, check all and submit.  If it didn't meet, click no and hit submit
            if (didGroupMeet)
            {
                //this._selenium.Click("group_attendance_yes");
                this._driver.FindElement(By.Id("group_attendance_yes")).Click();

                //this._selenium.Click("//input[@type='checkbox']");
               // this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
                this._driver.FindElement(By.XPath("//*[@id='attendance_roster']/table/tbody/tr[1]/th[1]/input")).Click();
                this._generalMethods.WaitForPageIsLoaded();
                // this._generalMethods.WaitForElementVisible(By.Id("btn_save"));
                // Changed by Mady Kou, update the way to locatte Save button to click on the activated one
                this._driver.FindElements(By.Id("btn_save")).FirstOrDefault(x => x.Displayed == true).Click();
                /*
                IWebElement[] elements = this._driver.FindElementsById("btn_save").ToArray();
                for (int i=0; i < elements.Length; i++)
                {
                    if(elements[i].Displayed)
                        elements[i].Click();
                }
                */
                    // Verify the attendance was posted
                    //    Assert.IsTrue(this._generalMethods.ItemExistsInTable(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayNoEndTime, "Met on", null), "The group met for a given attendance date but this data was not shown on the Attendance view page!");

                    // If there was an endtime, verifiy it is not present
                    if (attendanceDateNoDayNoEndTime != attendanceDateNoDayEndTime)
                    {
                        Assert.IsFalse(this._generalMethods.ItemExistsInTable(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayEndTime, "Met on", null), "End time was present.");
                    }

            }
            else
            {
                this._driver.FindElement(By.Id("group_attendance_no")).Click();

                string text = string.Format("group_attendance_comments The group did not meet on {0}.", scheduleDate);
                this._driver.FindElementById("content").SendKeys(text);
                this._driver.FindElementById("submitQuery").Click();

                // Verify the attendance was posted for this date under the "Did not meet" table.
                Assert.IsTrue(this._generalMethods.ItemExistsInTable(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayNoEndTime, "Did not meet", null), "The group did not meet for a given attendance date but this data was not shown on the Attendance view page!");

                // If there was an endtime, verifiy it is not present
                if (attendanceDateNoDayNoEndTime != attendanceDateNoDayEndTime)
                {
                    Assert.IsFalse(this._generalMethods.ItemExistsInTable(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayEndTime, "Did not meet", null), "End time was present.");
                }
            }

        }

        /// <summary>
        /// Logs the user out of InFellowship.
        /// </summary>
        public void Logout() {
            try
            {
                if (this._selenium.IsElementPresent("link=Sign out"))
                {
                    this._selenium.ClickAndWaitForPageToLoad("link=Sign out");
                }
                else
                {
                    log.WarnFormat("User already logged out");
                }
            }
            catch (System.Exception e)
            {
                log.ErrorFormat("Logout Error [URL: {0}] ", this._selenium.GetLocation());
                throw new System.Exception(e.ToString());
            }

            TestLog.WriteLine("Username Present: " + this._selenium.IsElementPresent("username"));
            Assert.IsTrue(this._selenium.IsElementPresent("username"));

            
            TestLog.WriteLine("Password Present: " + this._selenium.IsElementPresent("password"));
            Assert.IsTrue(this._selenium.IsElementPresent("password"));


        }

        /// <summary>
        /// Logs the user out of InFellowship.
        /// </summary>
        public void LogoutWebDriver() {

            try {
                if (this._generalMethods.IsElementPresentWebDriver(By.LinkText("Sign out")))
                {
                    this._driver.FindElementByLinkText("Sign out").Click();
                }
                else
                {
                    log.WarnFormat("User already logged out");
                }
            }
            catch (System.Exception e) {
                log.ErrorFormat("Logout Error [URL: {0}] ", this._generalMethods.GetUrl());
                throw new System.Exception(e.ToString());
            }

            TestLog.WriteLine("Username Present: " + this._generalMethods.IsElementPresentWebDriver(By.Id("username")));
            this._generalMethods.WaitForElement(By.Id("username"));
            Assert.IsTrue(this._generalMethods.IsElementPresentWebDriver(By.Id("username")));
            TestLog.WriteLine("Password Present: " + this._generalMethods.IsElementPresentWebDriver(By.Id("password")));
            Assert.IsTrue(this._generalMethods.IsElementPresentWebDriver(By.Id("password")));

        }

        #endregion Open / Login / Logout

        #region Responsive Design

        public void SetBrowserSizeTo_Mobile()
        {
            // Set window size to simulate iPhone
            this._driver.Manage().Window.Size = new Size(640, 960);

        }
        #endregion Responsive Design

        #region People

        #region Accounts
        /// <summary>
        /// Creates an InFellowship account.
        /// </summary>
        /// <param name="firstname">The first name of the account.</param>
        /// <param name="lastname">The last name of the account.</param>
        /// <param name="email">The email address of the account.</param>
        /// <param name="password">The password for the account.</param>
        public void People_Accounts_Create(string firstname, string lastname, string email, string password) {
            // Navigate to the account registration page
            this._selenium.ClickAndWaitForPageToLoad("link=Register");

            // Populate the input fields
            this._selenium.Type("firstname", firstname);
            this._selenium.Type("lastname", lastname);
            this._selenium.Type("emailaddress", email);
            this._selenium.Type("password", password);
            this._selenium.Type("confirmpassword", password);

            // Submit the form
            this._selenium.ClickAndWaitForPageToLoad("submit");
        }

        /// <summary>
        /// Creates an account, submitting all the required information.  This will also finalize the account creation process and fully create the account.
        /// </summary>
        /// <param name="churchID">The church code.</param>
        /// <param name="firstName">Your first name.</param>
        /// <param name="lastName">Your last name.</param>
        /// <param name="emailAddress">Your email address.</param>
        /// <param name="desiredPassword">Your desired password.</param>
        /// <param name="dateOfBirth">Your date of birth.</param>
        /// <param name="gender">Your gender.</param>
        /// <param name="country">Your current country.</param>
        /// <param name="addressOne">Street 1.</param>
        /// <param name="addressTwo">Street 2.</param>
        /// <param name="city">Your current city.</param>
        /// <param name="state">Your current state</param>
        /// <param name="postalCode">Your current postal code.</param>
        /// <param name="county">Your current county.</param>
        /// <param name="homePhone">Your current home phone.</param>
        /// <param name="mobilePhone">Your current mobile phone.</param>
        public void People_Accounts_Create(string churchID, string firstName, string lastName, string emailAddress, string desiredPassword, string dateOfBirth, GeneralEnumerations.Gender gender, string country, string addressOne, string addressTwo, string city, string state, string postalCode, string county, string homePhone, string mobilePhone) {
            // Fill out the required information to submit an account creation.
            this._selenium.ClickAndWaitForPageToLoad("link=Register");
            this._selenium.Type("firstname", firstName);
            this._selenium.Type("lastname", lastName);
            this._selenium.Type("emailaddress", emailAddress);
            this._selenium.Type("password", desiredPassword);
            this._selenium.Type("confirmpassword", desiredPassword);

            this._selenium.ClickAndWaitForPageToLoad("submit");

            // Query the Database to get the activation code
            string activationCode = this._sql.Groups_FetchUserActivationCode(Convert.ToInt16(churchID), emailAddress);

            TestLog.WriteLine(string.Format("Activation Code: {0}", activationCode));
            TestLog.WriteLine(string.Format("Go to {0}/UserLogin/AdditionalInformation/{1}", this.GetInFellowshipURL(), activationCode));

            // Use the activation code to open the Addtional Information page
            this._selenium.Open(string.Format("{0}/UserLogin/AdditionalInformation/{1}", this.GetInFellowshipURL(), activationCode));

            // Fill out the additional information
            this.Account_Confirm(dateOfBirth, gender, country, addressOne, addressTwo, city, state, postalCode, county, homePhone, mobilePhone);

            // Submit
            this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.CreateAccount.SaveAndCreateAccount);
        }

        /// <summary>
        /// Creates an account, submitting all the required information.  This will also finalize the account creation process and fully create the account.  Also specifies a preferred number.
        /// </summary>
        /// <param name="churchID">The church code.</param>
        /// <param name="firstName">Your first name.</param>
        /// <param name="lastName">Your last name.</param>
        /// <param name="emailAddress">Your email address.</param>
        /// <param name="desiredPassword">Your desired password.</param>
        /// <param name="dateOfBirth">Your date of birth.</param>
        /// <param name="gender">Your gender.</param>
        /// <param name="country">Your current country.</param>
        /// <param name="addressOne">Street 1.</param>
        /// <param name="addressTwo">Street 2.</param>
        /// <param name="city">Your current city.</param>
        /// <param name="state">Your current state</param>
        /// <param name="postalCode">Your current postal code.</param>
        /// <param name="county">Your current county.</param>
        /// <param name="homePhone">Your current home phone.</param>
        /// <param name="mobilePhone">Your current mobile phone.</param>
        /// <param name="preferredPhone">The phone communication type you wish to make preferred.</param>
        public void People_Accounts_Create(string churchID, string firstName, string lastName, string emailAddress, string desiredPassword, string dateOfBirth, GeneralEnumerations.Gender gender, string country, string addressOne, string addressTwo, string city, string state, string postalCode, string county, string homePhone, string mobilePhone, GeneralEnumerations.CommunicationTypes preferredPhone) {
            // Fill out the required information to submit an account creation.
            this._selenium.ClickAndWaitForPageToLoad("link=Register");
            this._selenium.Type("firstname", firstName);
            this._selenium.Type("lastname", lastName);
            this._selenium.Type("emailaddress", emailAddress);
            this._selenium.Type("password", desiredPassword);
            this._selenium.Type("confirmpassword", desiredPassword);

            this._selenium.ClickAndWaitForPageToLoad("submit");

            // Query the Database to get the activation code
            string activationCode = this._sql.Groups_FetchUserActivationCode(Convert.ToInt16(churchID), emailAddress);

            // Use the activation code to open the Addtional Information page
            this._selenium.Open(string.Format("{0}/UserLogin/AdditionalInformation/{1}", this.GetInFellowshipURL(), activationCode));

            // Fill out the additional information
            this.Account_Confirm(dateOfBirth, gender, country, addressOne, addressTwo, city, state, postalCode, county, homePhone, mobilePhone, preferredPhone);

            // Submit
            this._selenium.ClickAndWaitForPageToLoad("submit");
            //this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
        }

        /// <summary>
        /// This Method creates an infellowship account using webdriver
        /// </summary>
        /// <param name="churchID"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="emailAddress"></param>
        /// <param name="password"></param>
        /// <param name="linked">Will this user be automatically linked to an existing account?</param>
        /// <param name="dateOfBirth"></param>
        /// <param name="gender"></param>
        /// <param name="country"></param>
        /// <param name="addressOne"></param>
        /// <param name="addressTwo"></param>
        /// <param name="city"></param>
        /// <param name="state"></param>
        /// <param name="zipCode"></param>
        /// <param name="county"></param>
        /// <param name="homePhone"></param>
        /// <param name="mobilePhone"></param>
        /// <param name="prefferedPhone"></param>
        public string People_Accounts_Create_Webdriver(string churchID, string firstName, string lastName, string emailAddress, string password, bool linked, string dateOfBirth = null, string gender = null, string country = null, string addressOne = null, string addressTwo = null, string city = null, string state = null, string zipCode = null, string county = null, string homePhone = null, string mobilePhone = null, string prefferedPhone = null)
        {
            string churchCode = this._sql.FetchChurchCode(Convert.ToInt16(churchID));
            
            //Navigate to Infellowship 
            this.OpenWebDriver(churchCode);

            //Click Register
            this._driver.FindElementByLinkText("Register").Click();
            this._generalMethods.WaitForElement(By.Id(GeneralInFellowship.Account.FirstName));

            //Input fields 
            this._driver.FindElementById(GeneralInFellowship.Account.FirstName).SendKeys(firstName);
            this._driver.FindElementById(GeneralInFellowship.Account.LastName).SendKeys(lastName);
            this._driver.FindElementById(GeneralInFellowship.Account.LoginEmail).SendKeys(emailAddress);
            this._driver.FindElementById(GeneralInFellowship.Account.Password).SendKeys(password);
            this._driver.FindElementById(GeneralInFellowship.Account.ConfrimPassword).SendKeys(password);

            //Click Create Account
            this._driver.FindElementById(GeneralInFellowship.Account.CreateAccountButton).Click();
            this._generalMethods.WaitForElement(By.LinkText("re-send verification"));

            // Query the Database to get the activation code
            string activationCode = this._sql.Groups_FetchUserActivationCode(Convert.ToInt16(churchID), emailAddress);

            if (!linked)
            {
                // Use the activation code to open the Addtional Information page
                this._generalMethods.OpenURLWebDriver(string.Format("{0}/UserLogin/AdditionalInformation/{1}", this.GetInFellowshipURL(), activationCode));
                this._generalMethods.WaitForElement(By.Id(GeneralInFellowship.Account.DateOfBirth));

                //Input Date of birth
                this._driver.FindElementById(GeneralInFellowship.Account.DateOfBirth).SendKeys(dateOfBirth);

                //Gender
                if (gender == "Male")
                {
                    //Click Male Radio button
                    this._driver.FindElementByXPath(GeneralInFellowship.Account.MaleRadio).Click();
                }
                else
                {
                    //Click Female Radio button
                    this._driver.FindElementByXPath(GeneralInFellowship.Account.FemaleRadio).Click();
                }

                //Input address fields
                new SelectElement(this._driver.FindElementById(GeneralInFellowship.Account.CountryDropDown)).SelectByText(country);
                this._driver.FindElementById(GeneralInFellowship.Account.Address1).SendKeys(addressOne);
                if (addressTwo != null)
                {
                    //Input address two
                    this._driver.FindElementById(GeneralInFellowship.Account.Address2).SendKeys(addressTwo);
                }
                this._driver.FindElementById(GeneralInFellowship.Account.City).SendKeys(city);
                if (country == "United Kingdom")
                {
                    this._driver.FindElementById("province").SendKeys(state);
                }
                else
                {
                    new SelectElement(this._driver.FindElementById(GeneralInFellowship.Account.StateDropDown)).SelectByText(state);
                }

                this._driver.FindElementById(GeneralInFellowship.Account.ZipCode).SendKeys(zipCode);
                this._driver.FindElementById(GeneralInFellowship.Account.County).SendKeys(county);

                if (homePhone != null)
                {
                    // Enter home phone
                    this._driver.FindElementById(GeneralInFellowship.Account.HomePhone).SendKeys(homePhone);
                }
                if (mobilePhone != null)
                {
                    // enter mobile phone
                    this._driver.FindElementById(GeneralInFellowship.Account.MobilePhone).SendKeys(mobilePhone);
                }
                if (prefferedPhone != null)
                {
                    switch (prefferedPhone)
                    {
                        case "Home":
                            this._driver.FindElementByXPath("//a[@class='select_primary' and text()='home']").Click();
                            break;
                        case "Mobile":
                            this._driver.FindElementByXPath("//a[@class='select_primary' and text()='mobile']").Click();
                            break;
                        default:
                            break;
                    }
                }

                //Save account
                this._driver.FindElementById(GeneralInFellowshipConstants.CreateAccount.SaveAndCreateAccount).Click();
                //this._generalMethods.WaitForElement(By.LinkText("HOME"));
            }
            else
            {
                //Use activation to login to infellowship home
                this._generalMethods.OpenURLWebDriver(string.Format("{0}/UserLogin/AdditionalInformation/{1}", this.GetInFellowshipURL(), activationCode));
                //this._generalMethods.WaitForElement(By.LinkText("HOME"));
            }
            
            //Verify Success Message - Your account has been successfully activated.
            TestLog.WriteLine("Success Message: {0}", this._driver.FindElementById("success_message").Text.Trim());
            Assert.AreEqual("Close\r\nYour account has been successfully activated." , this._driver.FindElementById("success_message").Text.Trim());
            this._driver.FindElementById("success_close").Click();

            return activationCode;

        }

        public void People_Accounts_Create_Incomplete_WebDriver(string firstName, string lastName, string email, string password)
        {
            // Navigate to the account registration page
            this._driver.FindElementByLinkText("Register").Click();
            this._generalMethods.WaitForElement(By.Id("firstname"));

            // Populate the input fields
            this._driver.FindElementById("firstname").SendKeys(firstName);
            this._driver.FindElementById("lastname").SendKeys(lastName);
            this._driver.FindElementById("emailaddress").SendKeys(email);
            this._driver.FindElementById("password").SendKeys(password);
            this._driver.FindElementById("confirmpassword").SendKeys(password);

            // Submit the form
            this._driver.FindElementById("submit").Click();
            this._generalMethods.WaitForElement(By.LinkText("re-send verification"));
        }

        /// <summary>
        /// Updates your account information.
        /// </summary>
        /// <param name="updatedAccountName">The new login name.</param>
        /// <param name="updatedPassword">The new password.</param>
        /// <param name="currentPassword">The current password to confirm the update.</param>
        public void People_Accounts_Update_Account_Information(string updatedAccountName, string updatedPassword, string currentPassword) {
            // View the profile editor view page
            this.People_ProfileEditor_View();

            // Change your information
            this._selenium.ClickAndWaitForPageToLoad("link=Change Login / Password");

            // Unless we do not want to update our account name, type in the new account name
            if (!string.IsNullOrEmpty(updatedAccountName)) {
                this._selenium.Type("NewEmailAddress", updatedAccountName);
                this._selenium.Type("EmailAddressConfirm", updatedAccountName);
            }

            // Unless we do not want to update our password, type in the new password
            if (!string.IsNullOrEmpty(updatedPassword)) {
                this._selenium.Type("NewPassword", updatedPassword);
                this._selenium.Type("PasswodConfirm", updatedPassword);
            }

            // Type the current password to verify
            this._selenium.Type("CurrentPassword", currentPassword);

            // Submit
            this._selenium.ClickAndWaitForPageToLoad("btn_save");
        }

        /// <summary>
        /// This method updates an infellowship account in Webdriver
        /// </summary>
        /// <param name="loginEmail"></param>
        /// <param name="confirmLoginEmail"></param>
        /// <param name="updatedPassword"></param>
        /// <param name="updateConfirmPassword"></param>
        /// <param name="currentPassword"></param>
        public void People_Accounts_Update_Account_Information_WebDriver(string loginEmail, string confirmLoginEmail, string updatedPassword, string updateConfirmPassword, string currentPassword)
        {
            //Navigate to "Your Profile" page
            this._driver.FindElementByLinkText("Update Profile").Click();
            this._generalMethods.WaitForElement(By.LinkText("Change Login / Password"));
            //Click Change Login / Password
            this._driver.FindElementByLinkText("Change Login / Password").Click();
            this._generalMethods.WaitForElement(By.Id(GeneralInFellowship.Account.UpdateLoginEmail));

            //Input Email
            if (loginEmail != null)
            {
                this._driver.FindElementById(GeneralInFellowship.Account.UpdateLoginEmail).SendKeys(loginEmail);
                this._driver.FindElementById(GeneralInFellowship.Account.UpdateConfirmEmail).SendKeys(confirmLoginEmail);
            }
            if (updatedPassword != null)
            {
                this._driver.FindElementById(GeneralInFellowship.Account.NewPassword).SendKeys(updatedPassword);
                this._driver.FindElementById(GeneralInFellowship.Account.NewPasswordConfirm).SendKeys(updateConfirmPassword);
            }

            this._driver.FindElementById(GeneralInFellowship.Account.CurrentPassword).SendKeys(currentPassword);

            //Click Save
            this._driver.FindElementById(GeneralInFellowship.Account.UpdateSaveButton).Click();
            
        }

        /// <summary>
        /// Reset Password workflow
        /// </summary>
        /// <param name="userEmail">User Email</param>
        /// <param name="passwordReset">New Password</param>
        public void People_Accounts_Reset_Password(string userEmail, string passwordReset)
        {

            //Input recovery email
            this._driver.FindElementById("recovery_email").SendKeys(userEmail);
            this._driver.FindElementById("btnResetPassword").Click();

            //Forgot Password header
            this._generalMethods.VerifyTextPresentWebDriver("Forgot password");
            this._generalMethods.VerifyTextPresentWebDriver(userEmail);

            //Reset Password using URL
            //https://dc.qa.infellowship.com/UserLogin/NewPassword/90e03f69c1ea40b2b1478a4f36c4fcb1
            string passwordResetCode = this._sql.People_FetchUserPasswordResetCode(15, userEmail);
            this._generalMethods.OpenURLWebDriver(string.Format("{0}/UserLogin/NewPassword/{1}", this.GetInFellowshipURL(), passwordResetCode));
            this._driver.FindElementById("Password").SendKeys(passwordReset);
            this._driver.FindElementById("ConfirmPassword").SendKeys(passwordReset);
            this._driver.FindElementById("btnSave").Click();

            //Verify that we are taken to Infellowship Home Page
            this._generalMethods.TakeScreenShot_WebDriver();
            TestLog.WriteLine("Success Message: {0}", this._driver.FindElementById("success_message").Text.Trim());
            Assert.AreEqual("Close\r\nPassword reset!", this._driver.FindElementById("success_message").Text.Trim());
            this._driver.FindElementById("success_close").Click();

            //Verify that we are taken to the login page
            Assert.IsTrue(this._generalMethods.IsElementPresentWebDriver(By.Id("username")));
            Assert.IsTrue(this._generalMethods.IsElementPresentWebDriver(By.Id("password")));
            
            //this._generalMethods.VerifyElementPresentWebDriver(By.Id("tab_topper_home"));
            //this._generalMethods.VerifyElementPresentWebDriver(By.Id("tab_topper_groups"));
            //this._generalMethods.VerifyElementPresentWebDriver(By.Id("tab_topper_search"));
            //this._generalMethods.VerifyElementPresentWebDriver(By.Id("tab_topper_directory"));
            //this._generalMethods.VerifyElementPresentWebDriver(By.Id("tab_topper_giving"));

        }

        #endregion Accounts

        #region Church Directory
        /// <summary>
        /// Views an individual inside the Church Directory.
        /// </summary>
        /// <param name="individual">The name of the individual.</param>
        public void People_ChurchDirectory_View_Individual(string individual) {
            // Parse the individual for the first name and last name. Build the link
            string[] name = individual.Split(' ');
            string individualLinkInDirectory = string.Format("//a[contains(@href, '/Directory/Show') and span[1]/text()='{0}' and span[2]/text()='{1}']", name[0], name[1]);

            // Search for the individual
            this.People_ChurchDirectory_Search(individual);

            // View the individual, if it is returned in the results.
            if (this._selenium.IsElementPresent(individualLinkInDirectory)) {
                this._selenium.ClickAndWaitForPageToLoad(individualLinkInDirectory);
            }
            else {
                throw new System.Exception("Individual was not returned in the results.");
            }
        }

        /// <summary>
        /// Views a household member of an inidividual in the Church Directory.
        /// </summary>
        /// <param name="individual">The name of the individual in the directory.</param>
        /// <param name="houseHoldMember">The name of the household member you wish to view.</param>
        public void People_ChurchDirectory_View_HouseholdMember(string individual, string houseHoldMember) {
            this.People_ChurchDirectory_View_Individual(individual);

            // Figure out which individual to view.
            var numberOfHouseHoldMembers = (Int32)this._selenium.GetXpathCount("//ui[@class='list']/li");
            for (int x = 0; x < numberOfHouseHoldMembers; x++) {
                if (this._selenium.GetText(string.Format("//ui[@class='list']/li[{0}]", (x + 1).ToString())) == houseHoldMember) {
                    this._selenium.ClickAndWaitForPageToLoad(string.Format("//ui[@class='list']/li[{0}]/a", (x + 1).ToString()));
                    break;
                }
            }
        }

        /// <summary>
        /// Searches the Church Directory.
        /// </summary>
        /// <param name="searchQuery">The query you wish to search on.</param>
        public void People_ChurchDirectory_Search(string searchQuery) {
            // Figure out if we are viewing the directory.
            if (!this._selenium.IsElementPresent("search_query")) {
                try
                {
                    this.People_ChurchDirectory_View();
                }
                catch (System.Exception e)
                {
                    if (this._selenium.IsElementPresent("username"))
                    {
                        this.ReLoginInFellowship(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);
                        this.People_ChurchDirectory_View();
                    }
                    else
                    {
                        throw new System.Exception(e.Message, e);
                    } 
                
                }
            }

            // Perform the query
            this._selenium.Type("search_query", searchQuery);
            this._selenium.KeyPressAndWaitForPageToLoad("search_query", "\\13", "5000");

            // Verify the View All link is present
            this._selenium.VerifyElementPresent("link=Start over");
        }

        /// <summary>
        /// Views the church directory.
        /// </summary>
        public void People_ChurchDirectory_View() {
            // Are we already viewing the church directory index page?
            if (!this._selenium.IsElementPresent(TableIds.InFellowship_ChurchDirectory) && !this._selenium.IsElementPresent("search_query") && !this._selenium.IsElementPresent("//body[@id='directory_index']")) {
                // We were not.  Are we on the Home Page?
                if (this._selenium.IsElementPresent(GeneralInFellowshipConstants.LandingPage.Link_ChurchDirectory)) {
                    // We are. Click the Directory link.
                    this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_ChurchDirectory);
                }
                else {
                    // We were not on the home page.  Unable to determine where we are. Click the Directory Link at the top.
                    this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Directory);
                }
            }
            // We were already viewing the directory.  Do nothing.
        }

        #endregion Church Directory

        #region Privacy Settings

        /// <summary>
        /// Views your privacy settings page.
        /// </summary>
        /// <param name="selenium"></param>
        public void People_PrivacySettings_View() {
            // Are we already viewing privacy settings?
            if (!this._selenium.IsElementPresent("//table[@class='matrix']")) {
                // We were not. Is the link to view privacy settings present?
                if (this._selenium.IsElementPresent(InFellowshipConstants.People.ProfileEditorConstants.Link_PrivacySettings)) {
                    this._selenium.ClickAndWaitForPageToLoad(InFellowshipConstants.People.ProfileEditorConstants.Link_PrivacySettings); // It was. Just click the link.
                }
                // Are we on the home page?
                else if (this._selenium.IsElementPresent(GeneralInFellowshipConstants.LandingPage.Link_PrivacySettings)) {
                    // We are.  Click the link.
                    this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_PrivacySettings);
                }
                else {
                    // The link was not present and could not be located.  View the profile and click the Privacy Settings link.
                    this.People_ProfileEditor_View();
                    this._selenium.ClickAndWaitForPageToLoad(InFellowshipConstants.People.ProfileEditorConstants.Link_PrivacySettings);
                }
            }
        }

        /// <summary>
        /// Sets the privacy level for addresses.
        /// </summary>
        /// <param name="desiredLevel">The highest church role that can view this setting.</param>
        public void People_PrivacySettings_Update_AddressSetting(GeneralEnumerations.PrivacyLevels desiredLevel) {

            // View your privacy settings
            this.People_PrivacySettings_View();

            // Set the privacy setting for address
            this.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Address, desiredLevel);

            // Save
            this._selenium.ClickAndWaitForPageToLoad(InFellowshipConstants.People.PrivacySettingsConstants.Button_Submit);
        }

        /// <summary>
        /// Sets the privacy level for birthdate.
        /// </summary>
        /// <param name="desiredLevel">The highest church role that can view this setting.</param>
        public void People_PrivacySettings_Update_BirthdateSetting(GeneralEnumerations.PrivacyLevels desiredLevel) {

            // View your privacy settings
            this.People_PrivacySettings_View();

            // Set the privacy setting for birthdate
            this.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Birthdate, desiredLevel);

            // Save
            this._selenium.ClickAndWaitForPageToLoad(InFellowshipConstants.People.PrivacySettingsConstants.Button_Submit);
        }

        /// <summary>
        /// Sets the privacy level for email.
        /// </summary>
        /// <param name="desiredLevel">The highest church role that can view this setting.</param>
        public void People_PrivacySettings_Update_EmailSetting(GeneralEnumerations.PrivacyLevels desiredLevel) {

            // View your privacy settings
            this.People_PrivacySettings_View();

            // Set the privacy setting for email
            this.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Email, desiredLevel);

            // Save
            this._selenium.ClickAndWaitForPageToLoad(InFellowshipConstants.People.PrivacySettingsConstants.Button_Submit);
        }

        /// <summary>
        /// Sets the privacy level for phone.
        /// </summary>
        /// <param name="desiredLevel">The highest church role that can view this setting.</param>
        public void People_PrivacySettings_Update_PhoneSetting(GeneralEnumerations.PrivacyLevels desiredLevel) {

            // View your privacy settings
            this.People_PrivacySettings_View();

            // Set the privacy setting for phone
            this.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Phone, desiredLevel);

            // Save
            this._selenium.ClickAndWaitForPageToLoad(InFellowshipConstants.People.PrivacySettingsConstants.Button_Submit);
        }

        /// <summary>
        /// Sets the privacy level for websites.
        /// </summary>
        /// <param name="desiredLevel">The highest church role that can view this setting.</param>
        public void People_PrivacySettings_Update_WebsiteSetting(GeneralEnumerations.PrivacyLevels desiredLevel) {

            // View your privacy settings
            this.People_PrivacySettings_View();

            // Set the privacy setting for websites
            this.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Websites, desiredLevel);

            // Save
            this._selenium.ClickAndWaitForPageToLoad(InFellowshipConstants.People.PrivacySettingsConstants.Button_Submit);
        }

        /// <summary>
        /// Sets the privacy level for social media.
        /// </summary>
        /// <param name="desiredLevel">The highest church role that can view this setting.</param>
        public void People_PrivacySettings_Update_SocialSetting(GeneralEnumerations.PrivacyLevels desiredLevel) {
            // View your privacy settings
            this.People_PrivacySettings_View();

            // Set the privacy setting for social media
            this.People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes.Social, desiredLevel);

            // Save
            this._selenium.ClickAndWaitForPageToLoad(InFellowshipConstants.People.PrivacySettingsConstants.Button_Submit);
        }

        /// <summary>
        /// Specifies if you wish to be included or removed from the Church Directory.
        /// </summary>
        /// <param name="includeInDirectory">The setting that specifies if you are include or removed from the directory.</param>
        public void People_PrivacySettings_Update_OptInToDirectory(bool includeInDirectory) {
            // View Privacy Settings
            this.People_PrivacySettings_View();

            this._selenium.WaitForCondition("selenium.isVisible('include_in_church_directory');", "20000");
            // Specify if you wish to opt into the directory.
            if (includeInDirectory) {
                // We wish to opt into the directory.  Is it checked?
                if (!this._selenium.IsChecked(InFellowshipConstants.People.PrivacySettingsConstants.Checkbox_IncludeInDirectory))
                    this._selenium.Click(InFellowshipConstants.People.PrivacySettingsConstants.Checkbox_IncludeInDirectory); // It wasn't, check it.
            }
            else {
                // We want to be removed from the directory.  It is it checked
                if (this._selenium.IsChecked(InFellowshipConstants.People.PrivacySettingsConstants.Checkbox_IncludeInDirectory))
                    this._selenium.Click(InFellowshipConstants.People.PrivacySettingsConstants.Checkbox_IncludeInDirectory); // It was, uncheck it.
            }

            // Save
            this._selenium.ClickAndWaitForPageToLoad(InFellowshipConstants.People.PrivacySettingsConstants.Button_Submit);
        }

        #endregion Privacy Settings

        #region Profile Editor

        /// <summary>
        /// Views your profile.
        /// </summary>
        public void People_ProfileEditor_View() {
            // Are we already viewing your profile?
            if (!this._selenium.IsElementPresent("//body[@id='profile_index']")) {
                // We were not. View the home page and click the your profile link
                this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
                this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourProfile);
            }
            // We were.  Do nothing.
        }
        /// <summary>
        /// Views Your Profile in webdriver
        /// </summary>
        public void People_ProfileEditor_View_WebDriver()
        {
            // Are we already viewing your profile?
            if (!this._generalMethods.IsElementPresentWebDriver(By.XPath("//body[@id='profile_index']")))
            {
                // We were not. View the home page and click the your profile link
                this._driver.FindElementByLinkText("HOME").Click();
                this._generalMethods.WaitForElement(By.LinkText("Update Profile"));
                this._driver.FindElementByLinkText("Update Profile").Click();
                this._generalMethods.WaitForElement(By.XPath("//body[@id='profile_index']"));
            }
            // We were.  Do nothing.
        }

        /// <summary>
        /// Views the profile for a househlold member.
        /// </summary>
        /// <param name="householdMemberName">The name of the household.</param>
        public void People_ProfileEditor_View_HouseholdMember(string householdMemberName) {
        }

        /// <summary>
        /// Updates your personal info in InFellowship (name, goes by name, marital status, etc).
        /// </summary>
        /// <param name="updatedFirstName">The updated first name.</param>
        /// <param name="updatedLastName">The updated last name.</param>
        /// <param name="updatedGoesByName">The updated goes by name.</param>
        /// <param name="updatedHouseholdPosition">the updated household position.</param>
        /// <param name="updateDOB">The updated date of birth.</param>
        /// <param name="updatedMaritalStatus">The updated marital status.</param>
        /// <param name="updatedGender">The updated gender.</param>
        /// <param name="editFromGroupRoster">Specifiy if you wish to edit at the group roster. By default, editing from Profile Editor will occur.  This parameter is used to handle the shared page layout between Group Member Edit and Profile Editor Edit</param>
        public void People_ProfileEditor_Update_PersonalInfo(string updatedFirstName, string updatedLastName, string updatedGoesByName, GeneralEnumerations.HouseholdPositonWeb updatedHouseholdPosition, string updateDOB, GeneralEnumerations.MaritalStatus updatedMaritalStatus, GeneralEnumerations.Gender updatedGender, [Optional, DefaultParameterValue(false)] bool editFromGroupRoster) {
            // View the profile edit page, unless we are editing from the group roster
            if (!editFromGroupRoster) {
                try
                {
                    this.People_ProfileEditor_View_UpdatePage();
                }
                catch (System.Exception e)
                {
                    //If we are in login page then reLogin infellowship again
                    if (this._selenium.IsElementPresent("username"))
                    {
                        this.ReLoginInFellowship(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);
                        this.People_ProfileEditor_View_UpdatePage();
                    }
                    else
                    {
                        throw new System.Exception(e.Message, e);
                    }
                }
            }

            // Update the information
            if (!string.IsNullOrEmpty(updatedFirstName)) {
                this._selenium.Type("first_name", updatedFirstName);
            }
            if (!string.IsNullOrEmpty(updatedLastName)) {
                this._selenium.Type("last_name", updatedLastName);
            }
            if (!string.IsNullOrEmpty(updatedGoesByName)) {
                this._selenium.Type("goes_by", updatedGoesByName);
            }

            switch (updatedHouseholdPosition) {
                case GeneralEnumerations.HouseholdPositonWeb.Husband:
                    this._selenium.Select(InFellowshipConstants.People.ProfileEditorConstants.DropDown_HouseholdPositon, "Husband");
                    break;
                case GeneralEnumerations.HouseholdPositonWeb.Wife:
                    this._selenium.Select(InFellowshipConstants.People.ProfileEditorConstants.DropDown_HouseholdPositon, "Wife");
                    break;
                case GeneralEnumerations.HouseholdPositonWeb.SingleAdult:
                    this._selenium.Select(InFellowshipConstants.People.ProfileEditorConstants.DropDown_HouseholdPositon, "Single Adult");
                    break;
                case GeneralEnumerations.HouseholdPositonWeb.Child:
                    this._selenium.Select(InFellowshipConstants.People.ProfileEditorConstants.DropDown_HouseholdPositon, "Child");
                    break;
                case GeneralEnumerations.HouseholdPositonWeb.Other:
                    this._selenium.Select(InFellowshipConstants.People.ProfileEditorConstants.DropDown_HouseholdPositon, "Other (extended family)");
                    break;
                case GeneralEnumerations.HouseholdPositonWeb.FriendOfFamily:
                    this._selenium.Select(InFellowshipConstants.People.ProfileEditorConstants.DropDown_HouseholdPositon, "Friend of Family");
                    break;
                case GeneralEnumerations.HouseholdPositonWeb.NA:
                    break;
                default:
                    break;
            }

            // Null = do nothing.  Empty string = clear the input.
            if (updateDOB != null)
                this._selenium.Type(InFellowshipConstants.People.ProfileEditorConstants.DateControl_DateOfBirth, updateDOB);
            else if (updateDOB != string.Empty)
                this._selenium.Type(InFellowshipConstants.People.ProfileEditorConstants.DateControl_DateOfBirth, string.Empty);

            switch (updatedMaritalStatus) {
                case GeneralEnumerations.MaritalStatus.ChildYouth:
                    this._selenium.Select(InFellowshipConstants.People.ProfileEditorConstants.DropDown_MaritalStatus, "Child/Yth");
                    break;
                case GeneralEnumerations.MaritalStatus.Divorced:
                    this._selenium.Select(InFellowshipConstants.People.ProfileEditorConstants.DropDown_MaritalStatus, "Divorced");
                    break;
                case GeneralEnumerations.MaritalStatus.Married:
                    this._selenium.Select(InFellowshipConstants.People.ProfileEditorConstants.DropDown_MaritalStatus, "Married");
                    break;
                case GeneralEnumerations.MaritalStatus.Seperated:
                    this._selenium.Select(InFellowshipConstants.People.ProfileEditorConstants.DropDown_MaritalStatus, "Seperated");
                    break;
                case GeneralEnumerations.MaritalStatus.Single:
                    this._selenium.Select(InFellowshipConstants.People.ProfileEditorConstants.DropDown_MaritalStatus, "Single");
                    break;
                case GeneralEnumerations.MaritalStatus.Widow:
                    this._selenium.Select(InFellowshipConstants.People.ProfileEditorConstants.DropDown_MaritalStatus, "Widow");
                    break;
                case GeneralEnumerations.MaritalStatus.Widower:
                    this._selenium.Select(InFellowshipConstants.People.ProfileEditorConstants.DropDown_MaritalStatus, "Widower");
                    break;
                case GeneralEnumerations.MaritalStatus.NA:
                    break;
                default:
                    break;
            }

            switch (updatedGender) {
                case GeneralEnumerations.Gender.Male:
                    this._selenium.Select(InFellowshipConstants.People.ProfileEditorConstants.DropDown_Gender, "Male");
                    break;
                case GeneralEnumerations.Gender.Female:
                    this._selenium.Select(InFellowshipConstants.People.ProfileEditorConstants.DropDown_Gender, "Female");
                    break;
                case GeneralEnumerations.Gender.NA:
                    break;
                default:
                    break;
            }

            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
        }

        #region Phone

        /// <summary>
        /// Updates your phone information in InFellowship.
        /// </summary>
        /// <param name="phoneType">The type of phone number we wish to update.</param>
        /// <param name="updatedNumber">The updated number.</param>
        /// <param name="editFromGroupRoster">Specifiy if you wish to edit at the group roster. By default, editing from Profile Editor will occur.  This parameter is used to handle the shared page layout between Group Member Edit and Profile Editor Edit</param>
        public void People_ProfileEditor_Update_Phone(GeneralEnumerations.CommunicationTypes phoneType, string updatedNumber, [Optional, DefaultParameterValue(false)] bool editFromGroupRoster) {
            // View the profile edit page, unless we are editing from the group roster
            if (!editFromGroupRoster) {
                this.People_ProfileEditor_View_UpdatePage();
            }

            // Store the current phone number in case we are removing it.
            var currentPhoneNumber = string.Empty;

            // Remove the phone information for the specified type
            switch (phoneType) {
                case GeneralEnumerations.CommunicationTypes.Mobile:
                    currentPhoneNumber = this._selenium.GetValue("//fieldset[2]/table/tbody/tr[2]/td[2]/input[1]");
                    this._selenium.Type("//fieldset[2]/table/tbody/tr[2]/td[2]/input[1]", updatedNumber);
                    break;
                case GeneralEnumerations.CommunicationTypes.Work:
                    currentPhoneNumber = this._selenium.GetValue("//fieldset[2]/table/tbody/tr[3]/td[2]/input[1]");
                    this._selenium.Type("//fieldset[2]/table/tbody/tr[3]/td[2]/input[1]", updatedNumber);
                    break;
                case GeneralEnumerations.CommunicationTypes.Home:
                    currentPhoneNumber = this._selenium.GetValue("//fieldset[2]/table/tbody/tr[4]/td[2]/input[1]");
                    this._selenium.Type("//fieldset[2]/table/tbody/tr[4]/td[2]/input[1]", updatedNumber);
                    break;
                default:
                    throw new SeleniumException(string.Format("Phone type {0} is not supported in Profile Editor.", phoneType.ToString()));
            }

            // Save
            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify the updated number on the View Page.
            if (!string.IsNullOrEmpty(updatedNumber)) {
                // A number was provided. View page
                Assert.IsTrue(this._selenium.IsTextPresent(updatedNumber), "Added phone number was not present!");
            }
            else {
                // A number was removed. View Page
                Assert.IsFalse(this._selenium.IsTextPresent(currentPhoneNumber), "Deleted phone number was still present!");
            }

            // Update page
            this.People_ProfileEditor_View_UpdatePage();
            switch (phoneType) {
                case GeneralEnumerations.CommunicationTypes.Mobile:
                    Assert.AreEqual(updatedNumber, this._selenium.GetValue("//fieldset[2]/table/tbody/tr[2]/td[2]/input[1]"), "Phone number was not persisted on the update page!");
                    break;
                case GeneralEnumerations.CommunicationTypes.Work:
                    Assert.AreEqual(updatedNumber, this._selenium.GetValue("//fieldset[2]/table/tbody/tr[3]/td[2]/input[1]"), "Phone number was not persisted on the update page!");
                    break;
                case GeneralEnumerations.CommunicationTypes.Home:
                    Assert.AreEqual(updatedNumber, this._selenium.GetValue("//fieldset[2]/table/tbody/tr[4]/td[2]/input[1]"), "Phone number was not persisted on the update page!");
                    break;
                default:
                    throw new SeleniumException(string.Format("Phone type {0} is not supported in Profile Editor.", phoneType.ToString()));
            }


        }

        /// <summary>
        /// Updates your preferred phone number.
        /// </summary>
        /// <param name="preferedPhone">The phone type to be made preferred.</param>
        /// <param name="editFromGroupRoster">Specifiy if you wish to edit at the group roster. By default, editing from Profile Editor will occur.  This parameter is used to handle the shared page layout between Group Member Edit and Profile Editor Edit</param>
        public void People_ProfileEditor_Update_Preferred_Phone(GeneralEnumerations.CommunicationTypes preferedPhone, [Optional, DefaultParameterValue(false)] bool editFromGroupRoster) {
            // View the profile edit page, unless we are editing from the group roster
            if (!editFromGroupRoster) {
                this.People_ProfileEditor_View_UpdatePage();
            }

            switch (preferedPhone) {
                case GeneralEnumerations.CommunicationTypes.Mobile:
                    this._selenium.Click("//fieldset[2]/table/tbody/tr[2]/td[3]/input");
                    break;
                case GeneralEnumerations.CommunicationTypes.Work:
                    this._selenium.Click("//fieldset[2]/table/tbody/tr[3]/td[3]/input");
                    break;
                case GeneralEnumerations.CommunicationTypes.Home:
                    this._selenium.Click("//fieldset[2]/table/tbody/tr[4]/td[3]/input");
                    break;
                default:
                    break;
            }

            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
        }

        /// <summary>
        /// Deletes the phone information for a given type in Profile Editor.
        /// </summary>
        /// <param name="phoneType">The phone type you wish to delete. </param>
        /// <param name="editFromGroupRoster">Specifiy if you wish to edit at the group roster. By default, editing from Profile Editor will occur.  This parameter is used to handle the shared page layout between Group Member Edit and Profile Editor Edit</param>
        public void People_ProfileEditor_Delete_Phone(GeneralEnumerations.CommunicationTypes phoneType, [Optional, DefaultParameterValue(false)] bool editFromGroupRoster) {

            // Update the phone with an empty string
            this.People_ProfileEditor_Update_Phone(phoneType, string.Empty, editFromGroupRoster);
        }


        /// <summary>
        /// Enters an emergency phone on the profile editor page.
        /// </summary>
        /// <param name="phoneNumber">The phone number to be used.</param>
        /// <param name="comment">The comment.</param>
        public void People_ProfileEditor_Update_EmergencyPhone(string emergencyPhoneNumber, string comment) {
            // View the update profile page
            this.People_ProfileEditor_View_UpdatePage();

            // Store the current value
            var currentValuePhone = this._selenium.GetValue("phone_emergency");
            var currentValueComment = this._selenium.GetValue("phone_emergency_comment");

            // Enter the emergency phone
            this._selenium.Type("phone_emergency", emergencyPhoneNumber);

            // Enter a comment
            if (!string.IsNullOrEmpty(comment)) {
                this._selenium.Type("phone_emergency_comment", comment);

            }

            // Submit
            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify the updated emergency number on the View Page.
            if (!string.IsNullOrEmpty(emergencyPhoneNumber)) {
                // An emergency phone was provided
                Assert.IsTrue(this._selenium.IsTextPresent(emergencyPhoneNumber), "Added emergency number was not present!");
                if (!string.IsNullOrEmpty(comment)) {
                    Assert.IsTrue(this._selenium.IsTextPresent(comment), "Added emergency number comment was not present!");
                }
            }
            else {
                // An emergency number was removed. View Page
                Assert.IsFalse(this._selenium.IsTextPresent(currentValuePhone), "Deleted emergency number was still present!");
                Assert.IsFalse(this._selenium.IsTextPresent(currentValueComment), "Emergency number comment was still present even though the number was removed!");
            }


            // View the update page
            this.People_ProfileEditor_View_UpdatePage();

            // Verify the values
            Assert.AreEqual(emergencyPhoneNumber, this._selenium.GetValue("phone_emergency"), "Emergency phone was not saved!");
            Assert.AreEqual(comment, this._selenium.GetValue("phone_emergency_comment"), "Emergency comment was not saved!");
        }

        /// <summary>
        /// Deletes an emergy phone.
        /// </summary>
        public void People_ProfileEditor_Delete_EmergencyPhone() {
            this.People_ProfileEditor_Update_EmergencyPhone(string.Empty, string.Empty);
        }

        #endregion Phone

        #region Email
        /// <summary>
        /// Updates an email communication value.
        /// </summary>
        /// <param name="emailType">The communication email type.</param>
        /// <param name="updatedEmail">The updated email.</param>
        /// <param name="editFromGroupRoster">Specifiy if you wish to edit at the group roster. By default, editing from Profile Editor will occur.  This parameter is used to handle the shared page layout between Group Member Edit and Profile Editor Edit</param>
        public void People_ProfileEditor_Update_Email(GeneralEnumerations.CommunicationTypes emailType, string updatedEmail, [Optional, DefaultParameterValue(false)] bool editFromGroupRoster) {

            // View the profile edit page, unless we are editing from the group roster
            if (!editFromGroupRoster) {
                this.People_ProfileEditor_View_UpdatePage();
            }

            // Store the current email in case we are removing it.
            var currentEmail = string.Empty;

            switch (emailType) {
                case GeneralEnumerations.CommunicationTypes.Home:
                    currentEmail = this._selenium.GetValue("email_home");
                    this._selenium.Type("email_home", updatedEmail);
                    break;
                case GeneralEnumerations.CommunicationTypes.Personal:
                    currentEmail = this._selenium.GetValue("email_personal");
                    this._selenium.Type("email_personal", updatedEmail);
                    break;
                default:
                    throw new SeleniumException(string.Format("Email type {0} is not supported in Profile Editor.", emailType.ToString()));
            }

            // Save
            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify the updated email on the View Page.
            if (!string.IsNullOrEmpty(updatedEmail)) {
                // An email was provided. View page
                Assert.IsTrue(this._selenium.IsTextPresent(updatedEmail), "Added email was not present!");
            }
            else {
                // An email was removed. View Page
                Assert.IsFalse(this._selenium.IsTextPresent(currentEmail), "Deleted email was still present!");
            }

            // Update page
            this.People_ProfileEditor_View_UpdatePage();
            switch (emailType) {
                case GeneralEnumerations.CommunicationTypes.Home:
                    Assert.AreEqual(updatedEmail, this._selenium.GetValue("email_home"), "Email was not persisted on the update page!");
                    break;
                case GeneralEnumerations.CommunicationTypes.Personal:
                    Assert.AreEqual(updatedEmail, this._selenium.GetValue("email_personal"), "Email was not persisted on the update page!");
                    break;
                default:
                    throw new SeleniumException(string.Format("Email type {0} is not supported in Profile Editor.", emailType.ToString()));
            }
        }

        /// <summary>
        /// Updates your preferred email.
        /// </summary>
        /// <param name="preferedEmail">The phone type to be made preferred.</param>
        /// <param name="editFromGroupRoster">Specifiy if you wish to edit at the group roster. By default, editing from Profile Editor will occur.  This parameter is used to handle the shared page layout between Group Member Edit and Profile Editor Edit</param>
        public void People_ProfileEditor_Update_Preferred_Email(GeneralEnumerations.CommunicationTypes preferedEmail, [Optional, DefaultParameterValue(false)] bool editFromGroupRoster) {
            // View the profile edit page, unless we are editing from the group roster
            if (!editFromGroupRoster) {
                this.People_ProfileEditor_View_UpdatePage();
            }

            switch (preferedEmail) {
                case GeneralEnumerations.CommunicationTypes.InFellowship:
                    this._selenium.Click("//fieldset[3]/table/tbody/tr[2]/td[3]/input");
                    break;
                case GeneralEnumerations.CommunicationTypes.Personal:
                    this._selenium.Click("//fieldset[3]/table/tbody/tr[3]/td[3]/input");
                    break;
                case GeneralEnumerations.CommunicationTypes.Home:
                    this._selenium.Click("//fieldset[3]/table/tbody/tr[4]/td[3]/input");
                    break;
                default:
                    break;
            }

            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
        }


        /// <summary>
        /// Deletes the email information for a given type.
        /// </summary>
        /// <param name="emailType">The email type.</param>
        /// <param name="editFromGroupRoster">Specifiy if you wish to edit at the group roster. By default, editing from Profile Editor will occur.  This parameter is used to handle the shared page layout between Group Member Edit and Profile Editor Edit</param>
        public void People_ProfileEditor_Delete_Email(GeneralEnumerations.CommunicationTypes emailType, [Optional, DefaultParameterValue(false)] bool editFromGroupRoster) {

            // Update the email with an empty string
            this.People_ProfileEditor_Update_Email(emailType, string.Empty, editFromGroupRoster);
        }

        #endregion Email

        #region Address

        /// <summary>
        /// Updates your address in InFellowship.
        /// </summary>
        /// <param name="country">The name of the country.</param>
        /// <param name="streetOne">Address One information.</param>
        /// <param name="streetTwo">Address Two information.</param>
        /// <param name="cityName">The name of the city.</param>
        /// <param name="state">The name of the state.</param>
        /// <param name="zipCode">The zipcode of the address.</param>
        /// <param name="countyName">The name of the county.</param>
        /// <param name="editFromGroupRoster">Specifiy if you wish to edit at the group roster. By default, editing from Profile Editor will occur.  This parameter is used to handle the shared page layout between Group Member Edit and Profile Editor Edit</param>
        public void People_ProfileEditor_Update_Address(string country, string streetOne, string streetTwo, string cityName, string state, string zipCode, string countyName, [Optional, DefaultParameterValue(false)] bool editFromGroupRoster) {
            // View the profile edit page, unless we are editing from the group roster
            if (!editFromGroupRoster) {
                this.People_ProfileEditor_View_UpdatePage();
            }

            // Update the information
            if (!string.IsNullOrEmpty(country))
                this._selenium.Select(InFellowshipConstants.People.ProfileEditorConstants.DropDown_Country, country);
            if (!string.IsNullOrEmpty(streetOne))
                this._selenium.Type(InFellowshipConstants.People.ProfileEditorConstants.TextField_AddressOne, streetOne);
            if (!string.IsNullOrEmpty(streetTwo))
                this._selenium.Type(InFellowshipConstants.People.ProfileEditorConstants.TextField_AddressTwo, streetTwo);
            if (!string.IsNullOrEmpty(cityName))
                this._selenium.Type(InFellowshipConstants.People.ProfileEditorConstants.TextField_City, cityName);
            if (!string.IsNullOrEmpty(state))
                this._selenium.Select(InFellowshipConstants.People.ProfileEditorConstants.DropDown_State, state);
            if (!string.IsNullOrEmpty(zipCode))
                this._selenium.Type(InFellowshipConstants.People.ProfileEditorConstants.TextField_ZipCode, zipCode);
            if (!string.IsNullOrEmpty(countyName))
                this._selenium.Type(InFellowshipConstants.People.ProfileEditorConstants.TextField_County, countyName);

            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

        }

        #endregion Address

        #region Website
        /// <summary>
        /// Updates your website information in InFellowship.
        /// </summary>
        /// <param name="websiteInfo">The value fo the website.</param>
        /// <param name="updatedWebsiteInfo">The updated value for the website.</param>
        /// <param name="editFromGroupRoster">Specifiy if you wish to edit at the group roster. By default, editing from Profile Editor will occur.  This parameter is used to handle the shared page layout between Group Member Edit and Profile Editor Edit</param>
        public void People_ProfileEditor_Update_Website(string websiteInfo, string updatedWebsiteInfo, [Optional, DefaultParameterValue(false)] bool editFromGroupRoster) {
            // View the profile edit page, unless we are editing from the group roster
            if (!editFromGroupRoster) {
                this.People_ProfileEditor_View_UpdatePage();
            }

            // Get number of website values.
            var numberOfWebsitesAndIMs = this._selenium.GetXpathCount("//form/fieldset[5]/ul[@id='web_addresses']/li/input[1]");

            // Loop through each website item to see if it is the on you wish to update. If it is, update it with the new information
            for (int x = 1; x <= numberOfWebsitesAndIMs; x++) {
                if (this._selenium.GetValue("//form/fieldset[5]/ul[@id='web_addresses']/li[" + x + "]/input[1]") == websiteInfo)
                    this._selenium.Type("//form/fieldset[5]/ul[@id='web_addresses']/li[" + x + "]/input[1]", updatedWebsiteInfo);
            }

            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
        }


        /// <summary>
        /// Adds a new website.
        /// </summary>
        /// <param name="websiteInfo">The value fo the website.</param>
        /// <param name="editFromGroupRoster">Specifiy if you wish to edit at the group roster. By default, editing from Profile Editor will occur.  This parameter is used to handle the shared page layout between Group Member Edit and Profile Editor Edit</param>
        public void People_ProfileEditor_Update_Add_Website(string websiteInfo, [Optional, DefaultParameterValue(false)] bool editFromGroupRoster) {
            log.Debug("Update Add Website");
            // View the profile edit page, unless we are editing from the group roster
            if (!editFromGroupRoster) {
                this.People_ProfileEditor_View_UpdatePage();
            }

            // Get number of website values.  This is needed so we know what text field to type into (based on the xpath)
            var numberOfWebsitesAndIMs = this._selenium.GetXpathCount("//form/fieldset[5]/ul[@id='web_addresses']/li");
            var liElementPositionForNewValue = numberOfWebsitesAndIMs + 1;
            var TextField_NewWebsiteInfo = "//form/fieldset[5]/ul[@id='web_addresses']/li[" + liElementPositionForNewValue + "]/input[1]";

            //log.Debug("numberOfWebsitesAndIMs: " + numberOfWebsitesAndIMs);
            //log.Debug("liElementPositionForNewValue: "  + liElementPositionForNewValue);
            //log.Debug("TextField_NewWebsiteInfo: "  + TextField_NewWebsiteInfo);

            // Add a website value
                //this._selenium.Click(InFellowshipConstants.People.ProfileEditorConstants.Link_AddAnotherWebsite);//, JavaScriptMethods.IsElementPresent(TextField_NewWebsiteInfo), "5000");
                //this._selenium.Type(TextField_NewWebsiteInfo, websiteInfo);

                this._selenium.Type("//form/fieldset[5]/ul[@id='web_addresses']/li[1]/input[1]", websiteInfo);

            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
        }

        #endregion Website

        #region Social Media
        /// <summary>
        /// Updates your social media information.
        /// </summary>
        /// <param name="socialMediaType">The social media type you wish to update.</param>
        /// <param name="updatedSocialMediaInfo">The value for the social media.</param>
        /// <param name="editFromGroupRoster">Specifiy if you wish to edit at the group roster. By default, editing from Profile Editor will occur.  This parameter is used to handle the shared page layout between Group Member Edit and Profile Editor Edit</param>
        public void People_ProfileEditor_Update_SocialMedia(GeneralEnumerations.SocialMediaTypes socialMediaType, string updatedSocialMediaInfo, [Optional, DefaultParameterValue(false)] bool editFromGroupRoster) {
            // View the profile edit page, unless we are editing from the group roster
            if (!editFromGroupRoster) {
                this.People_ProfileEditor_View_UpdatePage();
            }


            // Store the current phone number in case we are removing it.
            var previousSocialMediaValue = string.Empty;

            // Update the information
            switch (socialMediaType) {
                case GeneralEnumerations.SocialMediaTypes.Facebook:
                    previousSocialMediaValue = this._selenium.GetValue("facebook");
                    this._selenium.Type("facebook", updatedSocialMediaInfo);
                    break;
                case GeneralEnumerations.SocialMediaTypes.Twitter:
                    previousSocialMediaValue = this._selenium.GetValue("twitter");
                    this._selenium.Type("twitter", updatedSocialMediaInfo);
                    break;
                case GeneralEnumerations.SocialMediaTypes.LinkedIn:
                    previousSocialMediaValue = this._selenium.GetValue("linkedin");
                    this._selenium.Type("linkedin", updatedSocialMediaInfo);
                    break;
                default:
                    throw new SeleniumException(string.Format("Social Media type {0} is not supported in Profile Editor.", socialMediaType.ToString()));
            }

            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);


            // Verify the updated social media value on the View Page.
            if (!string.IsNullOrEmpty(updatedSocialMediaInfo)) {
                // A social media value was provided. Verify the View page
                this._selenium.VerifyElementPresent(string.Format("link={0}", updatedSocialMediaInfo));
            }
            else {
                // A social media value not provided. Verify the View page does not contain the old value
                this._selenium.VerifyElementNotPresent(string.Format("link={0}", previousSocialMediaValue));
            }

            // Update page
            this.People_ProfileEditor_View_UpdatePage();
            switch (socialMediaType) {
                case GeneralEnumerations.SocialMediaTypes.Facebook:
                    Assert.AreEqual(updatedSocialMediaInfo, this._selenium.GetValue("facebook"), "Facebook value was not persisted on the update page.");
                    break;
                case GeneralEnumerations.SocialMediaTypes.Twitter:
                    Assert.AreEqual(updatedSocialMediaInfo, this._selenium.GetValue("twitter"), "Twitter value was not persisted on the update page.");
                    break;
                case GeneralEnumerations.SocialMediaTypes.LinkedIn:
                    Assert.AreEqual(updatedSocialMediaInfo, this._selenium.GetValue("linkedin"), "LinkedIn value was not persisted on the update page.");
                    break;
                default:
                    throw new SeleniumException(string.Format("Social Media type {0} is not supported in Profile Editor.", socialMediaType.ToString()));
            }
        }

        /// <summary>
        /// Deletes a social media value.
        /// </summary>
        /// <param name="socialMediaType">The social media type.</param>
        /// <param name="editFromGroupRoster">Specifiy if you wish to edit at the group roster. By default, editing from Profile Editor will occur.  This parameter is used to handle the shared page layout between Group Member Edit and Profile Editor Edit</param>
        public void People_ProfileEditor_Delete_SocialMedia(GeneralEnumerations.SocialMediaTypes socialMediaType, [Optional, DefaultParameterValue(false)] bool editFromGroupRoster) {
            this.People_ProfileEditor_Update_SocialMedia(socialMediaType, string.Empty, editFromGroupRoster);
        }


        #endregion Social Media

        /// <summary>
        /// Removes your goes by name.
        /// </summary>
        /// <param name="editFromGroupRoster">Specifiy if you wish to edit at the group roster. By default, editing from Profile Editor will occur.  This parameter is used to handle the shared page layout between Group Member Edit and Profile Editor Edit</param>
        public void People_ProfileEditor_Delete_GoesByName([Optional, DefaultParameterValue(false)] bool editFromGroupRoster) {
            // View the profile edit page, unless we are editing from the group roster
            if (!editFromGroupRoster) {
                this.People_ProfileEditor_View_UpdatePage();
            }

            // Clear the goes by name
            this._selenium.Type("goes_by", "");

            // Submit
            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
        }

        /// <summary>
        /// Deletes a website value.
        /// </summary>
        /// <param name="websiteIMName">The name of the website.</param>
        /// <param name="editFromGroupRoster">Specifiy if you wish to edit at the group roster. By default, editing from Profile Editor will occur.  This parameter is used to handle the shared page layout between Group Member Edit and Profile Editor Edit</param>
        public void People_ProfileEditor_Delete_Website(string websiteIMName, [Optional, DefaultParameterValue(false)] bool editFromGroupRoster) {
            // View the profile edit page, unless we are editing from the group roster
            if (!editFromGroupRoster) {
                this.People_ProfileEditor_View_UpdatePage();
            }

            // Get number of website values.
            var numberOfWebsitesAndIMs = this._selenium.GetXpathCount("//form/fieldset[5]/ul[@id='web_addresses']/li/input[1]");

            log.Debug("Del numberOfWebsitesAndIMs: " + numberOfWebsitesAndIMs);

            if (this._selenium.GetValue("//form/fieldset[5]/ul[@id='web_addresses']/li[1]/input[1]") == websiteIMName)
            {
                this._selenium.Focus("//form/fieldset[5]/ul[@id='web_addresses']/li[1]/input[1]");
                this._selenium.Type("//form/fieldset[5]/ul[@id='web_addresses']/li[1]/input[1]", "");
            }else{
                throw new System.Exception(websiteIMName + " web address not found.");
            }

            //We don't have multiple websites option any more just one
            // Loop through each website item to see if it is the on you wish to delete. If it is, delete it.
            //for (int x = 1; x <= numberOfWebsitesAndIMs; x++) {
            //    if (this._selenium.GetValue("//form/fieldset[5]/ul[@id='web_addresses']/li[" + x + "]/input[1]") == websiteIMName)
            //        this._selenium.Click("//form/fieldset[5]/ul[@id='web_addresses']/li[" + x + "]/a");
            //}

            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
        }


        /// <summary>
        /// Views the update profile page in InFellowship.
        /// </summary>
        public void People_ProfileEditor_View_UpdatePage() {
            // Are we already viewing the Update page?
            if (!this._selenium.IsElementPresent("//body[@id='profile_edit']")) {
                // We weren't. Is the update your profile link present?
                if (this._selenium.IsElementPresent(InFellowshipConstants.People.ProfileEditorConstants.Link_EditProfile)) {
                    // It was. Click it.
                    this._selenium.ClickAndWaitForPageToLoad(InFellowshipConstants.People.ProfileEditorConstants.Link_EditProfile);
                }
                else {
                    // We can't figure out where we are.  View your profile and click the edit your profile link.
                    this.People_ProfileEditor_View();
                    this._selenium.ClickAndWaitForPageToLoad(InFellowshipConstants.People.ProfileEditorConstants.Link_EditProfile);
                }
            }
            // We were. Don't do anything.
        }

        /// <summary>
        /// Makes an invalid email in InFellowship valid and verifies the values are updated correctly in the database.
        /// </summary>
        /// <param name="emailCommunicationType">The communication type to be made valid.</param>
        /// <param name="communicationValue">The current value of the communication type.</param>
        /// <param name="individualName">The individual name that owns the communication type.</param>
        /// <param name="churchId">The church id.  Default value is 15.</param>
        public void People_ProfileEditor_Update_Email_Make_Valid(GeneralEnumerations.CommunicationTypes emailCommunicationType, string communicationValue, string individualName, [Optional, DefaultParameterValueAttribute(15)] int churchId) {
            // View the update page
            this.People_ProfileEditor_View_UpdatePage();

            // Click "Why not" for the invalid communication type.  Store the communication value.
            switch (emailCommunicationType) {
                case GeneralEnumerations.CommunicationTypes.Home:
                    this._selenium.Click("//table[@id='email_address']/tbody/tr[4]/td[2]/small/a");
                    break;
                case GeneralEnumerations.CommunicationTypes.Personal:
                    this._selenium.Click("//table[@id='email_address']/tbody/tr[3]/td[2]/small/a");
                    break;
                case GeneralEnumerations.CommunicationTypes.InFellowship:
                    this._selenium.Click("//table[@id='email_address']/tbody/tr[2]/td[2]/small[2]/a");
                    //this._selenium.Click("//table[@id='email_address']/tbody/tr[2]/td[2]/small/a");
                    break;
                default:
                    throw new SeleniumException(string.Format("{0} is not a valid email communication type.", emailCommunicationType));
            }

            // Reactivate
            this._selenium.ClickAndWaitForPageToLoad("//input[@id='submitQuery' and @value='Reactivate address']");

            // Verify the email address is active and listed and the comment is null
            var communicationInformation = this._sql.People_GetCommunicationValueInformation(churchId, individualName, communicationValue).Rows[0];
            Assert.AreEqual(true, communicationInformation["Listed"], "Communication was made valid but the value was still unlisted.");
            Assert.AreEqual(DBNull.Value, communicationInformation["COMMUNICATION_COMMENT"], "Communication was made valid but the comment was not set to null.");
            Assert.AreEqual(DBNull.Value, communicationInformation["VALIDATION_STATUS_ID"], "Communication was made valid but the validation status id was not set to null.");
        }

        /// <summary>
        /// Updates your subscribe / unsubscribe status for emails.
        /// </summary>
        /// <param name="unsubscribe">Specify if you would like to subscribe to emails.</param>
        /// <param name="individualName">The name of the individual unsubscribing.</param>
        /// <param name="churchId">The church id.  This is optional and has a default value of 15.</param>
        public void People_ProfileEditor_Update_Email_SubscriptionStatus(bool unsubscribe, string individualName, [Optional, DefaultParameterValue(15)] int churchId) {
            // View the update page
            this.People_ProfileEditor_View_UpdatePage();

            // Do we want to subscribe or unsubscribe?
            if (unsubscribe) {
                // Unsubscribe
                if (!this._selenium.IsChecked("//input[@name='unsubscribe']")) {
                    this._selenium.Click("//input[@name='unsubscribe']");

                    // Submit
                    this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

                    // Verify
                    this._selenium.VerifyTextPresent("You have unsubscribed from church communication emails");
                    Assert.AreEqual(true, this._sql.People_GetIndividualUnsubscribedStatus(254, individualName), "Unsubscribed id was incorrect!");
                }
            }
            else {
                // Subscribe
                if (this._selenium.IsChecked("//input[@name='unsubscribe']")) {
                    this._selenium.Click("//input[@name='unsubscribe']");

                    // Submit
                    this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

                    // Verify
                    Assert.IsFalse(this._selenium.IsTextPresent("You have unsubscribed from church communication emails"), "Unsubscribed message was visible but user subscribed to emails!!");
                    Assert.AreEqual(false, this._sql.People_GetIndividualUnsubscribedStatus(254, individualName), "Unsubscribed id was incorrect!");
                }
            }
            

        }

        #endregion Profile Editor

        #endregion People

        #region Giving

        #region Give Now

        /// <summary>
        /// Performs a give now for a given credit card.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="creditCardType">The credit card type.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <param name="securityCode">The security code.</param>
        /// <param name="validCreditCard">Specifies if this credit card is valid or not.</param>
        public void Giving_GiveNow_CreditCard(int churchId, string fundOrPledgeDrive, string subFund, string amount, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, bool validCreditCard, [Optional, DefaultParameterValue("1 - January")] string validFromMonth, [Optional, DefaultParameterValue("2012")] string validFromYear) {

            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { FundOrPledgeDrive = fundOrPledgeDrive, SubFund = subFund, Amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            // Process the credit card
            this.Giving_GiveNow_CreditCard_Process(churchId, contributionData, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, validCreditCard, null, null, null, null, null, null, null, validFromMonth, validFromYear);
        }

        /// <summary>
        /// Performs a give now for a given credit card.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="creditCardType">The credit card type.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <param name="securityCode">The security code.</param>
        /// <param name="validCreditCard">Specifies if this credit card is valid or not.</param>
        public void Giving_GiveNow_CreditCard_WebDriver(int churchId, string fundOrPledgeDrive, string subFund, string amount, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, bool validCreditCard, [Optional, DefaultParameterValue("1 - January")] string validFromMonth, [Optional, DefaultParameterValue("2012")] string validFromYear) {

            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { fund = fundOrPledgeDrive, subFund = subFund, amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            // Process the credit card
            try
            {
                //Updated by Jim: Don't want to hard code the address
                this.Giving_GiveNow_CreditCard_Process_WebDriver(churchId, contributionData, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, validCreditCard, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, validFromMonth, validFromYear);
            }
            catch (System.Exception e)
            {
                //If have been logged out, let's try one more time
                if (this._generalMethods.IsElementPresentWebDriver(By.Id("username")))
                {
                    this.ReLoginWebDriver(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);                    
                    this.Giving_GiveNow_CreditCard_Process_WebDriver(churchId, contributionData, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, validCreditCard, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, validFromMonth, validFromYear);
                }
                else
                {
                    throw new System.Exception(e.Message, e);
                }
            }
        }

        /// <summary>
        /// Performs a give now for a given credit card.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="creditCardType">The credit card type.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <param name="securityCode">The security code.</param>
        /// <param name="validCreditCard">Specifies if this credit card is valid or not.</param>
        public void Giving_GiveNow_CreditCard_WithoutValidation_WebDriver(string fund, string amount, string creditCardType, string creditCardNumber, int expirationMonth, string expirationYear)
        {
            GeneralMethods utility = this._generalMethods;

            if (!utility.IsElementPresentWebDriver(By.LinkText("Sign out")))
            {
                this.ReLoginWebDriver(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);
            }

            utility.WaitAndGetElement(By.LinkText("HOME")).Click();
            utility.WaitAndGetElement(By.LinkText("Your Giving")).Click();

            utility.WaitForPageIsLoaded();
            utility.WaitAndGetElement(By.LinkText("Give Now")).Click();

            new SelectElement(utility.WaitAndGetElement(By.Id("fund0"))).SelectByText(fund);
            utility.WaitAndGetElement(By.Id("amount")).SendKeys(amount);

            utility.WaitAndGetElement(By.Id("payment_method_cc")).Click();
            utility.WaitAndGetElement(By.Id("new_card")).Click();

            new SelectElement(utility.WaitAndGetElement(By.Id("payment_type_id"))).SelectByText(creditCardType);

            utility.WaitAndGetElement(By.Id("cc_account_number")).SendKeys(creditCardNumber);

            new SelectElement(utility.WaitAndGetElement(By.Id("expiration_month"))).SelectByValue(expirationMonth.ToString());
            new SelectElement(utility.WaitAndGetElement(By.Id("expiration_year"))).SelectByText(expirationYear);

            utility.WaitAndGetElement(By.Id("submit")).Click();

            utility.WaitForPageIsLoaded();
            utility.WaitAndGetElement(By.Id("submit")).Click();
        }

        /// <summary>
        /// Performs a give now for a given credit card with multiple funds, subfunds and amounts
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="contributionData">The funds or pledge drives, subfunds, and amounts to give to.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="creditCardType">The credit card type.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <param name="securityCode">The security code.</param>
        /// <param name="validCreditCard">Specifies if this credit card is valid or not.</param>
        public void Giving_GiveNow_CreditCard(int churchId, IEnumerable<dynamic> contributionData, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, bool validCreditCard, [Optional, DefaultParameterValue("1 - January")] string validFromMonth, [Optional, DefaultParameterValue("2012")] string validFromYear) {

            // Process the credit card
            this.Giving_GiveNow_CreditCard_Process(churchId, contributionData, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, validCreditCard, null, null, null, null, null, null, null, validFromMonth, validFromYear);
        }

        /// <summary>
        /// Performs a give now for a given credit card with multiple funds, subfunds and amounts
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="contributionData">The funds or pledge drives, subfunds, and amounts to give to.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="creditCardType">The credit card type.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <param name="securityCode">The security code.</param>
        /// <param name="validCreditCard">Specifies if this credit card is valid or not.</param>
        public void Giving_GiveNow_CreditCard_WebDriver(int churchId, IEnumerable<dynamic> contributionData, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, bool validCreditCard, [Optional, DefaultParameterValue("1 - January")] string validFromMonth, [Optional, DefaultParameterValue("2012")] string validFromYear)
        {
            try
            {
                // Process the credit card
                this.Giving_GiveNow_CreditCard_Process_WebDriver(churchId, contributionData, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, validCreditCard, null, null, null, null, null, null, null, validFromMonth, validFromYear);
            }
            catch (System.Exception e)
            {
                //If have been logged out, let's try one more time
                if (this._generalMethods.IsElementPresentWebDriver(By.Id("username")))
                {
                    this.ReLoginWebDriver(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);
                    this.Giving_GiveNow_CreditCard_Process_WebDriver(churchId, contributionData, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, validCreditCard, null, null, null, null, null, null, null, validFromMonth, validFromYear);
                }
                else
                {
                    throw new System.Exception(e.Message, e);
                }

            }
        }

        /// <summary>
        /// Performs a give now for a given credit card, allowing you to provide a different billing address
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="creditCardType">The credit card type.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <param name="securityCode">The security code.</param>
        /// <param name="validCreditCard">Specifies if this credit card is valid or not.</param>
        /// <param name="country">The country</param>
        /// <param name="streetOne">The street one address.</param>
        /// <param name="streetTwo">The street two address.</param>
        /// <param name="city">The city.</param>
        /// <param name="state">The state.</param>
        /// <param name="postalCode">The postal code.</param>
        /// <param name="county">The county.</param>
        public void Giving_GiveNow_CreditCard(int churchId, string fundOrPledgeDrive, string subFund, string amount, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, bool validCreditCard, string country, string streetOne, string streetTwo, string city, string state, string postalCode, string county, [Optional, DefaultParameterValue("1 - January")] string validFromMonth, [Optional, DefaultParameterValue("2012")] string validFromYear) {
            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { FundOrPledgeDrive = fundOrPledgeDrive, SubFund = subFund, Amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            this.Giving_GiveNow_CreditCard_Process(churchId, contributionData, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, validCreditCard, country, streetOne, streetTwo, city, state, postalCode, county, validFromMonth, validFromYear);
        }

        /// <summary>
        /// Performs a give now for a given credit card, allowing you to provide a different billing address
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="creditCardType">The credit card type.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <param name="securityCode">The security code.</param>
        /// <param name="validCreditCard">Specifies if this credit card is valid or not.</param>
        /// <param name="country">The country</param>
        /// <param name="streetOne">The street one address.</param>
        /// <param name="streetTwo">The street two address.</param>
        /// <param name="city">The city.</param>
        /// <param name="state">The state.</param>
        /// <param name="postalCode">The postal code.</param>
        /// <param name="county">The county.</param>
        public void Giving_GiveNow_CreditCard_WebDriver(int churchId, string fundOrPledgeDrive, string subFund, string amount, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, bool validCreditCard, string country, string streetOne, string streetTwo, string city, string state, string postalCode, string county, [Optional, DefaultParameterValue("1 - January")] string validFromMonth, [Optional, DefaultParameterValue("2012")] string validFromYear)
        {
            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { fund = fundOrPledgeDrive, subFund = subFund, amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            this.Giving_GiveNow_CreditCard_Process_WebDriver(churchId, contributionData, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, validCreditCard, country, streetOne, streetTwo, city, state, postalCode, county, validFromMonth, validFromYear);
        }

        /// <summary>
        /// Performs a give now for a given credit card with multiple funds, subfunds and amounts, allowing you to provide a different billing address
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="contributionData">The funds or pledge drives, subfunds, and amounts to give to.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="creditCardType">The credit card type.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <param name="securityCode">The security code.</param>
        /// <param name="validCreditCard">Specifies if this credit card is valid or not.</param>
        /// <param name="country">The country</param>
        /// <param name="streetOne">The street one address.</param>
        /// <param name="streetTwo">The street two address.</param>
        /// <param name="city">The city.</param>
        /// <param name="state">The state.</param>
        /// <param name="postalCode">The postal code.</param>
        /// <param name="county">The county.</param>
        public void Giving_GiveNow_CreditCard(int churchId, IEnumerable<dynamic> contributionData, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, bool validCreditCard, string country, string streetOne, string streetTwo, string city, string state, string postalCode, string county, [Optional, DefaultParameterValue("1 - January")] string validFromMonth, [Optional, DefaultParameterValue("2012")] string validFromYear) {

            // Process the credit card
            this.Giving_GiveNow_CreditCard_Process(churchId, contributionData, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, validCreditCard, country, streetOne, streetTwo, city, state, postalCode, county, validFromMonth, validFromYear);

        }

        /// <summary>
        /// Performs a Give Now contribution without having to have an InFellowship account
        /// </summary>
        /// <param name="churchId">The church ID</param>
        /// <param name="fund">The fund</param>
        /// <param name="subFund">The subfund</param>
        /// <param name="amount">The dollar amount</param>
        /// <param name="firstName">Contributor's first name</param>
        /// <param name="lastName">Contributor's last name</param>
        /// <param name="emailAddress">Contributor's email address</param>
        /// <param name="paymentType">The payment type</param>
        /// <param name="cardType">The card type</param>
        /// <param name="cardNumber">The credit card number</param>
        /// <param name="expireMonth">The card expiration month</param>
        /// <param name="expireYear">The card expiration year</param>
        /// <param name="securityCode">The card security code</param>
        /// <param name="country">Country</param>
        /// <param name="addressLine1">Address Line 1</param>
        /// <param name="addressLine2">Address Line 2</param>
        /// <param name="city">City</param>
        /// <param name="state">State</param>
        /// <param name="zipCode">Zip Code</param>
        /// <param name="county">County</param>
        /// <param name="createAccount">Create account after processing?</param>
        /// <param name="captchaText">Captcha text to be entered</param>
        /// <param name="genericErrorMsg">Force a generic error message based on amount value passed in</param>
        public void Giving_GiveNow_WithoutAccount_CreditCard(int churchId, string fund, string subFund, string amount, string firstName, string lastName, string emailAddress, GeneralEnumerations.GiveByTypes paymentType, string cardType, string cardNumber,
            string expireMonth, string expireYear, string securityCode, string country, string addressLine1, string addressLine2, string city, string state, string zipCode, string county, bool createAccount, string captchaText = null, bool genericErrorMsg = false)
        {

            // Captcha Settings
            Captcha captcha = null;
            
            // Store culture for date settings
            CultureInfo culture = this.Culture_Settings(churchId);

            // Click on the Give Now link at the top of the screen
            this._driver.FindElementByLinkText(GeneralInFellowship.Giving.GIVE_NOW).Click();
            this._generalMethods.WaitForElement(By.Id(string.Format("{0}0", GeneralInFellowship.Giving.Fund)));

            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { fund = fund, subFund = subFund, amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            // Specify the fund / pledge drive information
            this.GivingWithoutAccount_PopulateFundSubFundAmountInformation_WebDriver(contributionData);

            // Enter the contributor's First & Last name and Email address
            this.GivingWithoutAccount_PopulateFirstLastNameEmail_WebDriver(firstName, lastName, emailAddress);

            // Enter the payment information
            #region Payment Information

            string validFromMonth = string.Empty;
            string validFromYear = string.Empty;


            if (churchId == 258)
            {
                validFromMonth = "1 - January";
                validFromYear = DateTime.Now.Year.ToString();
                
            }

            var paymentData = new
            {
                cardType = cardType,
                cardNumber = cardNumber,
                expireMonth = expireMonth,
                expireYear = expireYear,
                validFromMonth = validFromMonth,
                validFromYear = validFromYear,
                securityCode = securityCode,
                country = country,
                addressLine1 = addressLine1,
                addressLine2 = addressLine2,
                city = city,
                state = state,
                zipCode = zipCode,
                county = county
            };


            this.GivingWithoutAccount_PopulatePaymentInformation_WebDriver(churchId, paymentType, firstName, lastName, paymentData);

            #endregion Payment Information

            
            // Click to Continue
            if (this._generalMethods.IsElementVisibleWebDriver(By.XPath(GeneralInFellowship.Giving.Continue)))
            {
                this._driver.FindElementByXPath(GeneralInFellowship.Giving.Continue).Click();
            }
            else
            {
                this._driver.FindElementByXPath(GeneralInFellowship.Giving.Continue_Responsive).Click();
            }

            // If no validation messages occur, continue on to Review & Confirm
            #region Review & Confirm
            if (!this._generalMethods.IsElementPresentWebDriver(By.XPath(GeneralInFellowship.Giving.Error_Message)))
            {

                this._generalMethods.WaitForElement(By.XPath("//button[@type='submit']"));

                // Verify information looks correct
                if (churchId == 258)
                {
                    if (string.IsNullOrEmpty(subFund))
                    {
                        Assert.AreEqual(fund, this._driver.FindElementByXPath(string.Format("{0}/tbody/tr/td", TableIds.InFellowship_ContributionDetails_GiveNowWithoutAccount)).Text);
                        Assert.AreEqual(string.Format("£{0}", amount), this._driver.FindElementByXPath(string.Format("{0}/tbody/tr/td[2]", TableIds.InFellowship_ContributionDetails_GiveNowWithoutAccount)).Text.Trim());
                    }
                    else
                    {
                        Assert.AreEqual(string.Format("{0} > {1}", fund, subFund), this._driver.FindElementByXPath(string.Format("{0}/tbody/tr/td", TableIds.InFellowship_ContributionDetails_GiveNowWithoutAccount)).Text);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(subFund))
                    {
                        ;
                        Assert.AreEqual(fund, this._driver.FindElementByXPath(string.Format("{0}/tbody/tr/td", TableIds.InFellowship_ContributionDetails_GiveNowWithoutAccount)).Text);
                        Assert.AreEqual(String.Format("${0}", Convert.ToDecimal(amount).ToString("#,##0.00")), this._driver.FindElementByXPath(string.Format("{0}/tbody/tr/td[2]", TableIds.InFellowship_ContributionDetails_GiveNowWithoutAccount)).Text.Trim());
                    }
                    else
                    {
                        Assert.AreEqual(string.Format("{0} > {1}", fund, subFund), this._driver.FindElementByXPath(string.Format("{0}/tbody/tr/td", TableIds.InFellowship_ContributionDetails_GiveNowWithoutAccount)).Text);
                    }
                }

                Assert.Contains(this._driver.FindElementByXPath("//div[@class='box_notice']/p").Text, "When giving via credit or debit card, a temporary ");
                
                //If captcha present enter captcha text, also if captcha text passed in add captcha error text
                captcha = this.Enter_Giving_Now_Without_Accts_Captcha(captchaText);

                // Check whether or not to create account after processing payment
                if (!createAccount)
                {
                    // Uncheck the box to create an account
                    if (this._generalMethods.IsElementPresentWebDriver(By.XPath(string.Format("//input[@type='{0}']", GeneralInFellowship.Giving.CreateAnAccount_CheckBox))))
                    {
                        this._generalMethods.SelectCheckbox(By.XPath(string.Format("//input[@type='{0}']", GeneralInFellowship.Giving.CreateAnAccount_CheckBox)), false);
                    }

                    this.Giving_Now_Without_Accts_Captcha_Process_Payment_Submit(captcha, captchaText, genericErrorMsg);

                    // Success workflow
                    if (string.IsNullOrEmpty(captchaText) && (genericErrorMsg = false))
                    {
                        this._generalMethods.WaitForElement(By.Id(GeneralInFellowshipConstants.LoginPage.Email));

                        // Verify confirmation message appears
                        this._generalMethods.VerifyElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.Payment_Succes_Message));
                    }
                }
                else
                {
                    // Make sure the box for creating an account is checked
                    this._generalMethods.SelectCheckbox(By.XPath(string.Format("//input[@type='{0}']", GeneralInFellowship.Giving.CreateAnAccount_CheckBox)), true);

                    // Click to Process Payment
                    this.Giving_Now_Without_Accts_Captcha_Process_Payment_Submit(captcha, captchaText, genericErrorMsg);

                    // Success workflow
                    if (string.IsNullOrEmpty(captchaText) && (genericErrorMsg = false))
                    {
                        this._generalMethods.WaitForElement(By.Id(GeneralInFellowshipConstants.CreateAccount.FirstName));

                        // Verify confirmation message appears
                        this._generalMethods.VerifyElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.Payment_Succes_Message));

                        // Verify fields are prepopulated with firstname, lastname, email
                        Assert.AreEqual(firstName, this._driver.FindElementById(GeneralInFellowshipConstants.CreateAccount.FirstName).GetAttribute("value"));
                        Assert.AreEqual(lastName, this._driver.FindElementById(GeneralInFellowshipConstants.CreateAccount.LastName).GetAttribute("value"));
                        Assert.AreEqual(emailAddress, this._driver.FindElementById(GeneralInFellowshipConstants.CreateAccount.LoginEmail).GetAttribute("value"));
                    }
                }
            }
            #endregion Reveiw & Confirm
            // Validation Messages
            #region Validation Messages
            else
            {
                
                if (fund == "--")
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Give to... is required");
                    this._errorText.Add("Give to... is required");
                }
                if (string.IsNullOrEmpty(firstName))
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Contributor's first name is required");
                    //this._generalMethods.VerifyTextPresentWebDriver("First name is required");
                    this._errorText.Add("Contributor's first name is required");
                    this._errorText.Add("First name is required");

                }
                if (string.IsNullOrEmpty(lastName))
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Contributor's last name is required");
                    //this._generalMethods.VerifyTextPresentWebDriver("Last name is required");
                    this._errorText.Add("Contributor's last name is required");
                    this._errorText.Add("Last name is required");
                }
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(emailAddress);
                if (!match.Success)
                {
                    if (string.IsNullOrEmpty(emailAddress))
                    {
                        //this._generalMethods.VerifyTextPresentWebDriver("Contributor's email address is required");
                        this._errorText.Add("Contributor's email address is required");
                    }
                    else
                    {
                        this._errorText.Add("Contributor's email address is not a valid email address.");
                    }
                }
                if (cardType == "--")
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Credit card type is required");
                    this._errorText.Add("Credit card type is required");
                }
                if (string.IsNullOrEmpty(cardNumber))
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Credit card number is required");
                    this._errorText.Add("Credit card number is required");
                }
                if (expireMonth == "--")
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Please select a valid expiration month");
                    this._errorText.Add("Please select a valid expiration month");
                }
                if (expireYear == "--")
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Please select a valid expiration year");
                    this._errorText.Add("Please select a valid expiration year");
                }
                if (string.IsNullOrEmpty(addressLine1))
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Street 1 is required");
                    this._errorText.Add("Street 1 is required");
                }
                if (string.IsNullOrEmpty(city))
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("City is required");
                    this._errorText.Add("City is required");
                }
                if (string.IsNullOrEmpty(zipCode))
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Postal Code is required");
                    this._errorText.Add("Postal Code is required");
                }
                if (state == "--")
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("State is required");
                    this._errorText.Add("State is required");
                }

                this._generalMethods.VerifyErrorMessagesInfellowshipWebDriver(this._errorText);

            }
            #endregion Validation Messages
        }

        /// <summary>
        /// Performs a Give Now contribution without having to have an InFellowship account
        /// </summary>
        /// <param name="churchId">The church ID</param>
        /// <param name="fund">The fund</param>
        /// <param name="subFund">The subfund</param>
        /// <param name="amount">The dollar amount</param>
        /// <param name="firstName">Contributor's first name</param>
        /// <param name="lastName">Contributor's last name</param>
        /// <param name="emailAddress">Contributor's email address</param>
        /// <param name="paymentType">The payment type</param>
        /// <param name="cardType">The card type</param>
        /// <param name="cardNumber">The credit card number</param>
        /// <param name="expireMonth">The card expiration month</param>
        /// <param name="expireYear">The card expiration year</param>
        /// <param name="securityCode">The card security code</param>
        /// <param name="country">Country</param>
        /// <param name="addressLine1">Address Line 1</param>
        /// <param name="addressLine2">Address Line 2</param>
        /// <param name="city">City</param>
        /// <param name="state">State</param>
        /// <param name="zipCode">Zip Code</param>
        /// <param name="county">County</param>
        /// <param name="createAccount">Create account after processing?</param>
        /// <param name="captchaText">Captcha text to be entered</param>
        /// <param name="genericErrorMsg">Force a generic error message based on amount value passed in</param>
        public void Giving_GiveNow_WithoutAccount_CreditCard(int churchId, IEnumerable<dynamic> contributionData, string firstName, string lastName, string emailAddress, GeneralEnumerations.GiveByTypes paymentType, string cardType, string cardNumber,
            string expireMonth, string expireYear, string securityCode, string country, string addressLine1, string addressLine2, string city, string state, string zipCode, string county, bool createAccount, string captchaText = null, bool genericErrorMsg = false)
        {

            // Captcha Settings
            Captcha captcha = null;
            
            // Store culture for date settings
            CultureInfo culture = this.Culture_Settings(churchId);

            // Click on the Give Now link at the top of the screen
            this._driver.FindElementByLinkText(GeneralInFellowship.Giving.GIVE_NOW).Click();
            this._generalMethods.WaitForElement(By.Id(string.Format("{0}0", GeneralInFellowship.Giving.Fund)));

            //// Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            //var singleContribution = new { fund = fund, subFund = subFund, amount = amount };

            //// Even though we are just passing in one set of contribution data, we still need to store this in a list.
            //IList<dynamic> contributionData = new List<dynamic>();
            //contributionData.Add(singleContribution);

            // Specify the fund / pledge drive information
            this.GivingWithoutAccount_PopulateFundSubFundAmountInformation_WebDriver(contributionData);

            // Enter the contributor's First & Last name and Email address
            this.GivingWithoutAccount_PopulateFirstLastNameEmail_WebDriver(firstName, lastName, emailAddress);

            // Enter the payment information
            #region Payment Information

            string validFromMonth = string.Empty;
            string validFromYear = string.Empty;

            if (churchId == 258)
            {
                validFromMonth = "1 - January";
                validFromYear = DateTime.Now.Year.ToString();

            }

            var paymentData = new
            {
                cardType = cardType,
                cardNumber = cardNumber,
                expireMonth = expireMonth,
                expireYear = expireYear,
                validFromMonth = validFromMonth,
                validFromYear = validFromYear,
                securityCode = securityCode,
                country = country,
                addressLine1 = addressLine1,
                addressLine2 = addressLine2,
                city = city,
                state = state,
                zipCode = zipCode,
                county = county
            };


            this.GivingWithoutAccount_PopulatePaymentInformation_WebDriver(churchId, paymentType, firstName, lastName, paymentData);

            #endregion Payment Information

            // Click to Continue
            if (this._generalMethods.IsElementVisibleWebDriver(By.XPath(GeneralInFellowship.Giving.Continue)))
            {
                this._driver.FindElementByXPath(GeneralInFellowship.Giving.Continue).Click();
            }
            else
            {
                this._driver.FindElementByXPath(GeneralInFellowship.Giving.Continue_Responsive).Click();
            }

            // If no validation messages occur, continue on to Review & Confirm
            #region Review & Confirm
            if (!this._generalMethods.IsElementPresentWebDriver(By.XPath(GeneralInFellowship.Giving.Error_Message)))
            {

                this._generalMethods.WaitForElement(By.XPath("//button[@type='submit']"));

                // Since there are multiple funds/subfunds and amounts that can be used, loop through the collection.
                int fundCounter = 0;

                foreach (var item in contributionData)
                {
                    // Verify Fund/Subfund information
                    if (string.IsNullOrEmpty(item.subFund))
                    {
                        Assert.AreEqual(item.fund, this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td", TableIds.InFellowship_ContributionDetails_GiveNowWithoutAccount, fundCounter +1)).Text);
                    }
                    else
                    {
                        Assert.AreEqual(string.Format("{0} > {1}", item.fund, item.subFund), this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td", TableIds.InFellowship_ContributionDetails_GiveNowWithoutAccount, fundCounter + 1)).Text);
                    }

                    // Verify Amount
                    if (churchId == 258)
                    {
                        Assert.AreEqual(string.Format("£{0}", item.amount), this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.InFellowship_ContributionDetails_GiveNowWithoutAccount, fundCounter + 1)).Text.Trim());
                    }
                    else
                    {
                        Assert.AreEqual(string.Format("${0}", item.amount), this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.InFellowship_ContributionDetails_GiveNowWithoutAccount, fundCounter + 1)).Text.Trim());
                    }
                    fundCounter++;
                }

                Assert.Contains(this._driver.FindElementByXPath("//div[@class='box_notice']/p").Text, "When giving via credit or debit card, a temporary ");
                
                //If captcha present enter captcha text, also if captcha text passed in add captcha error text
                captcha = this.Enter_Giving_Now_Without_Accts_Captcha(captchaText);

                // Check whether or not to create account after processing payment
                if (!createAccount)
                {
                    // Uncheck the box to create an account
                    this._generalMethods.SelectCheckbox(By.XPath(string.Format("//input[@type='{0}']", GeneralInFellowship.Giving.CreateAnAccount_CheckBox)), false);

                    // Click to Process Payment
                    this.Giving_Now_Without_Accts_Captcha_Process_Payment_Submit(captcha, captchaText, genericErrorMsg);

                    // Success workflow
                    if (string.IsNullOrEmpty(captchaText) && (genericErrorMsg = false))
                    {
                        this._generalMethods.WaitForElement(By.Id(GeneralInFellowshipConstants.LoginPage.Email));

                        // Verify confirmation message appears
                        this._generalMethods.VerifyElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.Payment_Succes_Message));
                    }
                }
                else
                {
                    // Make sure the box for creating an account is checked
                    this._generalMethods.SelectCheckbox(By.XPath(string.Format("//input[@type='{0}']", GeneralInFellowship.Giving.CreateAnAccount_CheckBox)), true);

                    // Click to Process Payment
                    this.Giving_Now_Without_Accts_Captcha_Process_Payment_Submit(captcha, captchaText, genericErrorMsg);

                    // Success workflow
                    if (string.IsNullOrEmpty(captchaText) && (genericErrorMsg = false))
                    {
                        this._generalMethods.WaitForElement(By.Id(GeneralInFellowshipConstants.CreateAccount.FirstName));

                        // Verify confirmation message appears
                        this._generalMethods.VerifyElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.Payment_Succes_Message));

                        // Verify fields are prepopulated with firstname, lastname, email
                        Assert.AreEqual(firstName, this._driver.FindElementById(GeneralInFellowshipConstants.CreateAccount.FirstName).GetAttribute("value"));
                        Assert.AreEqual(lastName, this._driver.FindElementById(GeneralInFellowshipConstants.CreateAccount.LastName).GetAttribute("value"));
                        Assert.AreEqual(emailAddress, this._driver.FindElementById(GeneralInFellowshipConstants.CreateAccount.LoginEmail).GetAttribute("value"));
                    }
                }
            }
            #endregion Reveiw & Confirm
            // Validation Messages
            #region Validation Messages
            else
            {
                for (int i = 0; i < contributionData.Count<dynamic>(); i++)
                {
                    if (contributionData.ElementAt<dynamic>(i).fund == "--")
                    {
                        //this._generalMethods.VerifyTextPresentWebDriver("Give to... is required");
                        this._errorText.Add("Give to... is required");
                    }
                }
                if (string.IsNullOrEmpty(firstName))
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Contributor's first name is required");
                    //this._generalMethods.VerifyTextPresentWebDriver("First name is required");
                    this._errorText.Add("Contributor's first name is required");
                    this._errorText.Add("First name is required");

                }
                if (string.IsNullOrEmpty(lastName))
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Contributor's last name is required");
                    //this._generalMethods.VerifyTextPresentWebDriver("Last name is required");
                    this._errorText.Add("Contributor's last name is required");
                    this._errorText.Add("Last name is required");
                }
                if (string.IsNullOrEmpty(emailAddress))
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Contributor's email address is required");
                    this._errorText.Add("Contributor's email address is required");
                }
                if (cardType == "--")
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Credit card type is required");
                    this._errorText.Add("Credit card type is required");
                }
                if (string.IsNullOrEmpty(cardNumber))
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Credit card number is required");
                    this._errorText.Add("Credit card number is required");
                }
                if (expireMonth == "--")
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Please select a valid expiration month");
                    this._errorText.Add("Please select a valid expiration month");
                }
                if (expireYear == "--")
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Please select a valid expiration year");
                    this._errorText.Add("Please select a valid expiration year");
                }
                if (string.IsNullOrEmpty(addressLine1))
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Street 1 is required");
                    this._errorText.Add("Street 1 is required");
                }
                if (string.IsNullOrEmpty(city))
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("City is required");
                    this._errorText.Add("City is required");
                }
                if (state == "--")
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("State is required");
                    this._errorText.Add("State is required");
                }
                if (string.IsNullOrEmpty(zipCode))
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Postal Code is required");
                    this._errorText.Add("Postal Code is required");
                }

                this._generalMethods.VerifyErrorMessagesInfellowshipWebDriver(this._errorText);

            }
            #endregion Validation Messages
        }

        /// <summary>
        /// Performs a Give Now contribution without having to have an InFellowship account
        /// </summary>
        /// <param name="churchId">The church ID</param>
        /// <param name="fund">The fund</param>
        /// <param name="subFund">The subfund</param>
        /// <param name="amount">The dollar amount</param>
        /// <param name="firstName">Contributor's first name</param>
        /// <param name="lastName">Contributor's last name</param>
        /// <param name="emailAddress">Contributor's email address</param>
        /// <param name="paymentType">The payment type</param>
        /// <param name="phoneNumber">The phone number</param>
        /// <param name="routingNumber">The routing number</param>
        /// <param name="accountNumber">The account number</param>
        /// <param name="createAccount">Create account after processing?</param>
        /// <param name="captchaText">Captcha text to be entered</param>
        /// <param name="genericErrorMsg">Force a generic error message based on amount value passed in</param>
        public void Giving_GiveNow_WithoutAccount_PersonalCheck(int churchId, string fund, string subFund, string amount, string firstName, string lastName, string emailAddress, GeneralEnumerations.GiveByTypes paymentType, string phoneNumber, 
            string routingNumber, string accountNumber, bool createAccount, string captchaText = null, bool genericErrorMsg = false)
        {
            // Captcha Settings
            Captcha captcha = null;

            // Store culture for date settings
            CultureInfo culture = this.Culture_Settings(churchId);

            // Click on the Give Now link at the top of the screen
            this._driver.FindElementByLinkText(GeneralInFellowship.Giving.GIVE_NOW).Click();
            this._generalMethods.WaitForElement(By.Id(string.Format("{0}0", GeneralInFellowship.Giving.Fund)));

            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { fund = fund, subFund = subFund, amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            // Specify the fund / pledge drive information
            this.GivingWithoutAccount_PopulateFundSubFundAmountInformation_WebDriver(contributionData);

            // Enter the contributor's First & Last name and Email address
            this.GivingWithoutAccount_PopulateFirstLastNameEmail_WebDriver(firstName, lastName, emailAddress);

            // Enter the payment information
            var paymentData = new
            {
                phoneNumber = phoneNumber,
                routingNumber = routingNumber,
                accountNumber = accountNumber,
            };

            this.GivingWithoutAccount_PopulatePaymentInformation_WebDriver(churchId, paymentType, firstName, lastName, paymentData);

            // Click to Continue
            if (this._generalMethods.IsElementVisibleWebDriver(By.XPath(GeneralInFellowship.Giving.Continue)))
            {
                this._driver.FindElementByXPath(GeneralInFellowship.Giving.Continue).Click();
            }
            else
            {
                this._driver.FindElementByXPath(GeneralInFellowship.Giving.Continue_Responsive).Click();
            }

            // If no validation messages occur, continue on to Review & Confirm
            if (!this._generalMethods.IsElementPresentWebDriver(By.XPath(GeneralInFellowship.Giving.Error_Message)))
            {

                this._generalMethods.WaitForElement(By.XPath("//button[@type='submit']"));

                //FO-2942 verify ACH Auth Message     
                string msg = this._driver.FindElementById("echeck_auth_message").Text.ToString();

                String format = "MMMM dd, yyyy";

                Assert.AreEqual(msg, "By clicking the button below, I authorize Active Network, LLC to charge my bank account on " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(format, DateTimeFormatInfo.InvariantInfo) + " for the amount of $" + amount + " for " + fund + ".");

                // Verify information looks correct
                if (string.IsNullOrEmpty(subFund))
                {
                    Assert.AreEqual(fund, this._driver.FindElementByXPath(string.Format("{0}/tbody/tr/td", TableIds.InFellowship_ContributionDetails_GiveNowWithoutAccount)).Text);
                    Assert.AreEqual(string.Format("${0}", amount), this._driver.FindElementByXPath(string.Format("{0}/tbody/tr/td[2]", TableIds.InFellowship_ContributionDetails_GiveNowWithoutAccount)).Text.Trim());
                }
                else
                {
                    Assert.AreEqual(string.Format("{0} > {1}", fund, subFund), this._driver.FindElementByXPath(string.Format("{0}/tbody/tr/td", TableIds.InFellowship_ContributionDetails_GiveNowWithoutAccount)).Text);
                }

                //This is only for CC or Debit and not for Pesonal checks
                //Assert.Contains(this._driver.FindElementByXPath("//div[@class='box_notice']/p").Text, "When giving via credit or debit card, a temporary ");

                //If captcha present enter captcha text, also if captcha text passed in add captcha error text
                captcha = this.Enter_Giving_Now_Without_Accts_Captcha(captchaText);

                // Check whether or not to create account after processing payment
                if (!createAccount)
                {
                    // Uncheck the box to create an account
                    this._generalMethods.SelectCheckbox(By.XPath(string.Format("//input[@type='{0}']", GeneralInFellowship.Giving.CreateAnAccount_CheckBox)), false);

                    // Click to Process Payment
                    this.Giving_Now_Without_Accts_Captcha_Process_Payment_Submit(captcha, captchaText, genericErrorMsg);

                    // Success workflow
                    if (string.IsNullOrEmpty(captchaText) && (genericErrorMsg = false))
                    {
                        this._generalMethods.WaitForElement(By.Id(GeneralInFellowshipConstants.LoginPage.Email));

                        // Verify confirmation message appears
                        this._generalMethods.VerifyElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.Payment_Succes_Message));
                    }
                }
                else
                {
                    // Make sure the box for creating an account is checked
                    this._generalMethods.SelectCheckbox(By.XPath(string.Format("//input[@type='{0}']", GeneralInFellowship.Giving.CreateAnAccount_CheckBox)), true);

                    // Click to Process Payment
                    this.Giving_Now_Without_Accts_Captcha_Process_Payment_Submit(captcha, captchaText, genericErrorMsg);

                    // Success workflow
                    if (string.IsNullOrEmpty(captchaText) && (genericErrorMsg = false))
                    {
                        this._generalMethods.WaitForElement(By.Id(GeneralInFellowshipConstants.CreateAccount.FirstName));

                        // Verify confirmation message appears
                        this._generalMethods.VerifyElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.Payment_Succes_Message));

                        // Verify fields are prepopulated with firstname, lastname, email
                        Assert.AreEqual(firstName, this._driver.FindElementById(GeneralInFellowshipConstants.CreateAccount.FirstName).GetAttribute("value"));
                        Assert.AreEqual(lastName, this._driver.FindElementById(GeneralInFellowshipConstants.CreateAccount.LastName).GetAttribute("value"));
                        Assert.AreEqual(emailAddress, this._driver.FindElementById(GeneralInFellowshipConstants.CreateAccount.LoginEmail).GetAttribute("value"));
                    }
                }
            }

            // Validation Messages
            #region Validation Messages
            else
            {
                this._errorText.Clear();

                if (fund == "--")
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Give to... is required");
                    this._errorText.Add("Give to... is required");
                }
                if (string.IsNullOrEmpty(firstName))
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Contributor's first name is required");
                    this._errorText.Add("Contributor's first name is required");
                }
                if (string.IsNullOrEmpty(lastName))
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Contributor's last name is required");
                    this._errorText.Add("Contributor's last name is required");
                }
                if (string.IsNullOrEmpty(emailAddress))
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Contributor's email address is required");
                    this._errorText.Add("Contributor's email address is required");
                }
                if (string.IsNullOrEmpty(phoneNumber))
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Phone number is required");
                    this._errorText.Add("Phone number is required");
                }

                if (string.IsNullOrEmpty(routingNumber))
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Bank routing number is required");
                    this._errorText.Add("Bank routing number is required");
                }
                else if ((routingNumber.Length < 9) || (routingNumber.Length > 9))
                {
                    this._errorText.Add("Please provide a valid nine-digit routing number");
                }
                if (string.IsNullOrEmpty(accountNumber))
                {
                    //this._generalMethods.VerifyTextPresentWebDriver("Account number is required");
                    this._errorText.Add("Account number is required");
                }

                this._generalMethods.VerifyErrorMessagesInfellowshipWebDriver(this._errorText);
            }
            #endregion Validation Messages
        }

        /// <summary>
        /// Try multiple Process Payment submits around Giving Without Accounts using Captcha
        /// </summary>
        /// <param name="captcha">Captcha object returned by DeathByCaptcha</param>
        /// <param name="captchaText">Captcha to be used instead one retrieved from DeathByCaptcha</param>
        /// <param name="genericErrorMsg">Are we going to process payment and cause generic error message</param>
        /// <param name="captchaErrorTxt">Error Message Text</param>
        /// <returns></returns>
        private Captcha Giving_Now_Without_Accts_Captcha_Process_Payment_Submit(Captcha captcha, string captchaText, bool genericErrorMsg = false, string captchaErrorTxt = "The characters typed in the challenge didn’t match.")
        {

            this.Giving_Now_Process_Payment_Click();

            bool isCaptchaError = Is_Captcha_Error_Only(captchaText, captchaErrorTxt);

            //If we are expecting a captcha error, let's verify that we got it
            if (!String.IsNullOrEmpty(captchaText))
            {
                Assert.AreEqual(captchaErrorTxt, this._driver.FindElement(By.XPath(GeneralInFellowship.Giving.Error_Message)).FindElement(By.TagName("ul")).FindElements(By.TagName("li"))[0].Text);
            }
            else
            {

                //Try again if only Captcha Error
                if (Is_Captcha_Error_Only(captchaText, captchaErrorTxt))
                {
                    this.Send_Captcha_Report_Error(captcha);
                    captcha = this.Enter_Captcha_Text(captchaText, captchaErrorTxt);

                    this.Giving_Now_Process_Payment_Click();

                }

                if (this._generalMethods.IsElementPresentWebDriver(By.XPath(GeneralInFellowship.Giving.Error_Message)))
                {
                    if (genericErrorMsg)
                    {
                        log.DebugFormat("Error: {0}", this._driver.FindElement(By.XPath(GeneralInFellowship.Giving.Error_Message)).FindElement(By.TagName("ul")).FindElements(By.TagName("li"))[0].Text);
                        Assert.AreEqual("Authorization was not successful. Please check your information including your billing address to make sure they are accurate, or use another card or select another form of payment.", this._driver.FindElement(By.XPath(GeneralInFellowship.Giving.Error_Message)).FindElement(By.TagName("ul")).FindElements(By.TagName("li"))[0].Text);
                    }
                    else
                    {
                        //Send Captcha Report Error
                        if (Is_Captcha_Error_Only(captchaText, captchaErrorTxt))
                        {
                            this.Send_Captcha_Report_Error(captcha);
                        }

                        throw new System.Exception(string.Format("Error Processing Payment: {0}", this._driver.FindElement(By.XPath(GeneralInFellowship.Giving.Error_Message)).FindElement(By.TagName("ul")).FindElements(By.TagName("li"))[0].Text));
                    }

                }
            }

            return captcha;
        }

        /// <summary>
        /// Click on Process Payment button for non-responsive or responsive
        /// </summary>
        private void Giving_Now_Process_Payment_Click()
        {

            // Click to Process Payment
            if (this._generalMethods.IsElementVisibleWebDriver(By.XPath(GeneralInFellowship.Giving.Process_Payment)))
            {
                this._driver.FindElementByXPath(GeneralInFellowship.Giving.Process_Payment).Click();
            }
            else
            {
                this._driver.FindElementByXPath(GeneralInFellowship.Giving.Process_Payment_Responsive).Click();
            }

        }

        /// <summary>
        /// Enter Captcha if Present for Giving Without Accounts
        /// </summary>
        /// <param name="captchaText">Captcha to be used instead one retrieved from DeathByCaptcha</param>
        /// <param name="captchaErrorTxt">Error Message Text</param>
        private Captcha Enter_Giving_Now_Without_Accts_Captcha(string captchaText, string captchaErrorTxt = "The characters typed in the challenge didn’t match.")
        {

            Captcha captcha = null;
            bool isPresent = this._generalMethods.IsElementPresentWebDriver(By.Id(GeneralInFellowship.Giving.Captcha));

            if (isPresent)
            {
                TestLog.WriteLine("Captcha Found");
                captcha = this.Enter_Captcha_Text(captchaText, captchaErrorTxt);
            }
            else
            {
                TestLog.WriteLine("Captcha Not found");
            }

            return captcha;
        }

        /// <summary>
        /// Performs a give now for a personal check.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="routingNumber">The routing number.</param>
        /// <param name="accountNumber">The account number.</param>
        public void Giving_GiveNow_PersonalCheck(int churchId, string firstName, string lastName, string fundOrPledgeDrive, string subFund, string amount, string phoneNumber, string routingNumber, string accountNumber) {
            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { FundOrPledgeDrive = fundOrPledgeDrive, SubFund = subFund, Amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            this.Giving_GiveNow_PersonalCheck_Process(churchId, firstName, lastName, contributionData, phoneNumber, routingNumber, accountNumber);
        }

        /// <summary>
        /// Performs a give now for a personal check with multiple funds, subfunds and amounts
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="contributionData">The funds or pledge drives, subfunds, and amounts to give to.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="routingNumber">The routing number.</param>
        /// <param name="accountNumber">The account number.</param>
        public void Giving_GiveNow_PersonalCheck(int churchId, string firstName, string lastName, IEnumerable<dynamic> contributionData, string phoneNumber, string routingNumber, string accountNumber) {
            this.Giving_GiveNow_PersonalCheck_Process(churchId, firstName, lastName, contributionData, phoneNumber, routingNumber, accountNumber);
        }

        /// <summary>
        /// Performs a give now for a personal check.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="routingNumber">The routing number.</param>
        /// <param name="accountNumber">The account number.</param>
        public void Giving_GiveNow_PersonalCheck_WebDriver(int churchId, string firstName, string lastName, string fundOrPledgeDrive, string subFund, string amount, string phoneNumber, string routingNumber, string accountNumber)
        {
            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { fund = fundOrPledgeDrive, subFund = subFund, amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            try
            {
                this.Giving_GiveNow_PersonalCheck_Process_WebDriver(churchId, firstName, lastName, contributionData, phoneNumber, routingNumber, accountNumber);
            }
            catch(System.Exception e)
            {
                //If have been logged out, let's try one more time
                if (this._generalMethods.IsElementPresentWebDriver(By.Id("username")))
                {
                    this.ReLoginWebDriver(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);
                    this.Giving_GiveNow_PersonalCheck_Process_WebDriver(churchId, firstName, lastName, contributionData, phoneNumber, routingNumber, accountNumber);
                }
                else
                {
                    throw new System.Exception(e.Message, e);
                }
            }

        }

        /// <summary>
        /// Performs a give now for a personal check with multiple funds, subfunds and amounts
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="contributionData">The funds or pledge drives, subfunds, and amounts to give to.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="routingNumber">The routing number.</param>
        /// <param name="accountNumber">The account number.</param>
        public void Giving_GiveNow_PersonalCheck_WebDriver(int churchId, string firstName, string lastName, IEnumerable<dynamic> contributionData, string phoneNumber, string routingNumber, string accountNumber)
        {
            try
            {
                this.Giving_GiveNow_PersonalCheck_Process_WebDriver(churchId, firstName, lastName, contributionData, phoneNumber, routingNumber, accountNumber);
            }
            catch (System.Exception e)
            {
                //If have been logged out, let's try one more time
                if (this._generalMethods.IsElementPresentWebDriver(By.Id("username")))
                {
                    this.ReLoginWebDriver(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);
                    this.Giving_GiveNow_PersonalCheck_Process_WebDriver(churchId, firstName, lastName, contributionData, phoneNumber, routingNumber, accountNumber);
                }
                else
                {
                    throw new System.Exception(e.Message, e);
                }
            }
        }


        /// <summary>
        /// Performs a give now for a credit card but instead of finishing the process, edit Step 1 from Step 2.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="creditCardType">The credit card type.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <param name="securityCode">The security code.</param>
        /// <param name="country">The country</param>
        /// <param name="streetOne">The street one address.</param>
        /// <param name="streetTwo">The street two address.</param>
        /// <param name="city">The city.</param>
        /// <param name="state">The state.</param>
        /// <param name="postalCode">The postal code.</param>
        /// <param name="county">The county.</param>
        public void Giving_GiveNow_CreditCard_Edit_Step1(int churchId, string fundOrPledgeDrive, string subFund, string amount, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, string country, string streetOne, string streetTwo, string city, string state, string postalCode, string county) {

            // Store the last four digits of the credit card
            var lastFourDigits = "*" + creditCardNumber.Substring(creditCardNumber.Length - 4);

            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { FundOrPledgeDrive = fundOrPledgeDrive, SubFund = subFund, Amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            // View Give Now 
            this._selenium.ClickAndWaitForPageToLoad("link=Your Giving");
            this._selenium.ClickAndWaitForPageToLoad("link=Give Now");

            // Populate credit card information on step 1
            this.Giving_GiveNow_Populate_Step1(churchId, "Credit Card", contributionData, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, country, streetOne, streetTwo, city, state, postalCode, county, null, null, null, null, null);

            // Store the Billing address information
            var streetOneText = string.IsNullOrEmpty(streetOne) ? this._selenium.GetValue("Address1") : streetOne;
            var streetTwoText = string.IsNullOrEmpty(streetTwo) ? this._selenium.GetText("Address2") : streetTwo;
            var cityText = string.IsNullOrEmpty(city) ? this._selenium.GetValue("City") : city;
            var stateText = string.IsNullOrEmpty(state) ? this._selenium.GetSelectedValue("state") : state;
            var postCodeText = string.IsNullOrEmpty(postalCode) ? this._selenium.GetValue("PostalCode") : postalCode;
            var countryName = string.IsNullOrEmpty(country) ? this._selenium.GetSelectedLabel("Country") : country;
            var countyName = string.IsNullOrEmpty(county) ? this._selenium.GetValue("County") : county;

            // Click to Continue
            if (this._selenium.IsVisible(GeneralInFellowship.Giving.Continue))
            {
                this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowship.Giving.Continue);
            }
            else
            {
                this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowship.Giving.Continue_Responsive);
            }

            // Edit step 1
            //this._selenium.ClickAndWaitForPageToLoad("//a[@href='/OnlineGiving/GiveNow/Step1' and following-sibling::span[@class='icon_text icon_payment']]");
            this._selenium.ClickAndWaitForPageToLoad("link=Edit");

            // Verify all the data provided was persisted
            Assert.AreEqual(firstName, this._selenium.GetValue("FirstName"));
            Assert.AreEqual(lastName, this._selenium.GetValue("LastName"));
            Assert.AreEqual(creditCardType, this._selenium.GetSelectedLabel("payment_type_id"));

            // Verify the credit card is masked up to the last four digits
            if (this._sql.IsAMSEnabled(this._infellowshipChurchID))
            {
                Assert.AreEqual("", this._selenium.GetValue("account_number"), "AMS Credit Card Number should be empty");
            }
            else
            {
                Assert.IsFalse(this._selenium.GetValue("account_number").Equals(creditCardNumber), "Credit Card Number is not masked");
                // Commenting this out for now until I talk to Thomas about how he wants this treated
                //var maskedLastFourDigits = "*" + this._selenium.GetValue("account_number").Substring(this._selenium.GetValue("account_number").Length - 4);
                //Assert.AreEqual(lastFourDigits, maskedLastFourDigits);
            }

            Assert.AreEqual(expirationMonth, this._selenium.GetSelectedLabel("expiration_month"));
            Assert.AreEqual(expirationYear, this._selenium.GetSelectedLabel("expiration_year"));
            Assert.AreEqual(countryName, this._selenium.GetSelectedLabel("Country"));
            Assert.AreEqual(streetOneText, this._selenium.GetValue("Address1"));

            // Street two is optional
            if (string.IsNullOrEmpty(streetTwo)) {
                Assert.AreEqual(string.Empty, this._selenium.GetValue("Address2"));
            }

            else {
                Assert.AreEqual(streetTwoText, this._selenium.GetValue("Address2"));
            }

            Assert.AreEqual(cityText, this._selenium.GetValue("City"));
            Assert.AreEqual(stateText, this._selenium.GetSelectedLabel("state"));
            Assert.AreEqual(postCodeText, this._selenium.GetValue("PostalCode"));
            Assert.AreEqual(countyName, this._selenium.GetValue("County"));
        }

        /// <summary>
        /// Performs a give now for a credit card but instead of finishing the process, edit Step 1 from Step 2.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="creditCardType">The credit card type.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <param name="securityCode">The security code.</param>
        /// <param name="country">The country</param>
        /// <param name="streetOne">The street one address.</param>
        /// <param name="streetTwo">The street two address.</param>
        /// <param name="city">The city.</param>
        /// <param name="state">The state.</param>
        /// <param name="postalCode">The postal code.</param>
        /// <param name="county">The county.</param>
        public void Giving_GiveNow_CreditCard_Edit_Step1_WebDriver(int churchId, string fundOrPledgeDrive, string subFund, string amount, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, string country, string streetOne, string streetTwo, string city, string state, string postalCode, string county)
        {

            // Store the last four digits of the credit card
            var lastFourDigits = "*" + creditCardNumber.Substring(creditCardNumber.Length - 4);

            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { fund = fundOrPledgeDrive, subFund = subFund, amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            // View Giving History
            this._driver.FindElementByLinkText("Your Giving").Click();

            // If in Mobile view verify Schedule Giving is not visible
            var windowSize = this._driver.Manage().Window.Size;

            if (windowSize.Width == 640)
            {
                this._generalMethods.VerifyElementNotPresentWebDriver(By.LinkText("Schedule Giving"));
            }

            // Click on Give Now
            this._driver.FindElementByPartialLinkText("Give Now").Click();

            // Populate credit card information on step 1
            this.Giving_GiveNow_Populate_Step1_WebDriver(churchId, "Credit Card", contributionData, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, country, streetOne, streetTwo, city, state, postalCode, county, null, null, null, null, null);

            // Store the Billing address information
            var streetOneText = string.IsNullOrEmpty(streetOne) ? this._driver.FindElementById("Address1").GetAttribute("value") : streetOne;
            var streetTwoText = string.IsNullOrEmpty(streetTwo) ? this._driver.FindElementById("Address2").GetAttribute("value") : streetTwo;
            var cityText = string.IsNullOrEmpty(city) ? this._driver.FindElementById("City").GetAttribute("value") : city;
            var stateText = string.IsNullOrEmpty(state) ? this._driver.FindElementById("state").GetAttribute("value") : state;
            var postCodeText = string.IsNullOrEmpty(postalCode) ? this._driver.FindElementById("PostalCode").GetAttribute("value") : postalCode;
            var countryName = string.IsNullOrEmpty(country) ? this._driver.FindElementById("Country").GetAttribute("value") : country;
            var countyName = string.IsNullOrEmpty(county) ? this._driver.FindElementById("County").GetAttribute("value") : county;

            // Click to Continue
            if (this._generalMethods.IsElementVisibleWebDriver(By.XPath(GeneralInFellowship.Giving.Continue)))
            {
                this._driver.FindElementByXPath(GeneralInFellowship.Giving.Continue).Click();
            }
            else
            {
                this._driver.FindElementByXPath(GeneralInFellowship.Giving.Continue_Responsive).Click();
            }

            // Edit step 1
            //this._selenium.ClickAndWaitForPageToLoad("//a[@href='/OnlineGiving/GiveNow/Step1' and following-sibling::span[@class='icon_text icon_payment']]");
            this._driver.FindElementByLinkText("Edit").Click();

            // Click bullet for new card
            if (this._generalMethods.IsElementPresentWebDriver(By.Id("new_card")))
            {
                this._driver.FindElementById("new_card").Click();
            }

            var cardTypeNumber = this._sql.Giving_CreditCardTypeID(creditCardType);

            // Verify all the data provided was persisted
            Assert.AreEqual(firstName, this._driver.FindElementById("FirstName").GetAttribute("value"));
            Assert.AreEqual(lastName, this._driver.FindElementById("LastName").GetAttribute("value"));
            Assert.AreEqual(cardTypeNumber.ToString(), this._driver.FindElementById("payment_type_id").GetAttribute("value"));

            // Verify the credit card is masked up to the last four digits
            if (this._sql.IsAMSEnabled(this._infellowshipChurchID))
            {
                Assert.AreEqual("", this._driver.FindElementById("account_number").GetAttribute("value"), "AMS Credit Card Number should be empty");
            }
            else
            {
                Assert.IsFalse(this._driver.FindElementById("account_number").GetAttribute("value").Equals(creditCardNumber), "Credit Card Number is not masked");
                // Commenting this out for now until I talk to Thomas about how he wants this treated
                //var maskedLastFourDigits = "*" + this._selenium.GetValue("account_number").Substring(this._selenium.GetValue("account_number").Length - 4);
                //Assert.AreEqual(lastFourDigits, maskedLastFourDigits);
            }

            Assert.Contains(expirationMonth, this._driver.FindElementById("expiration_month").GetAttribute("value"));
            Assert.AreEqual(expirationYear, this._driver.FindElementById("expiration_year").GetAttribute("value"));

            var countryNameAbbr = string.Empty;

            if (countryName == "United States")
            {
                countryNameAbbr = "US";
            }
            else if (countryName == "United Kingdom")
            {
                countryNameAbbr = "UK";
            }

            Assert.AreEqual(countryNameAbbr, this._driver.FindElementById("Country").GetAttribute("value"));
            Assert.AreEqual(streetOneText, this._driver.FindElementById("Address1").GetAttribute("value"));

            // Street two is optional
            if (string.IsNullOrEmpty(streetTwo))
            {
                Assert.AreEqual(string.Empty, this._driver.FindElementById("Address2").GetAttribute("value"));
            }

            else
            {
                Assert.AreEqual(streetTwoText, this._driver.FindElementById("Address2").GetAttribute("value"));
            }

            Assert.AreEqual(cityText, this._driver.FindElementById("City").GetAttribute("value"));
            var stateAbbr = string.Empty;
            if (stateText == "Texas")
            {
                stateAbbr = "TX";
            }
            Assert.AreEqual(stateAbbr, this._driver.FindElementById("state").GetAttribute("value"));
            Assert.AreEqual(postCodeText, this._driver.FindElementById("PostalCode").GetAttribute("value"));
            Assert.AreEqual(countyName, this._driver.FindElementById("County").GetAttribute("value"));

        }


        /// <summary>
        /// Performs a give now for a personal check but instead of finishing the process, edit Step 1 from Step 2.
        /// </summary>
        /// /// <param name="churchId">The church ID.</param>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="routingNumber">The routing number.</param>
        /// <param name="accountNumber">The account number.</param>
        public void Giving_GiveNow_PersonalCheck_Edit_Step1(int churchId, string fundOrPledgeDrive, string subFund, string amount, string phoneNumber, string routingNumber, string accountNumber) {

            // Store the last four digits of the account number
            var lastFourDigits = accountNumber.Substring(accountNumber.Length - 4);

            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { FundOrPledgeDrive = fundOrPledgeDrive, SubFund = subFund, Amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            // View Give Now 
            this._selenium.ClickAndWaitForPageToLoad("link=Your Giving");
            this._selenium.ClickAndWaitForPageToLoad("link=Give Now");

            // Populate personal check information on step 1
            this.Giving_GiveNow_Populate_Step1(churchId, "Personal Check", contributionData, null, null, null, null, null, null, null, null, null, null, null, null, null, null, phoneNumber, routingNumber, accountNumber, null, null);

            // Submit 
            this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowship.Giving.Continue);

            // Edit step 1
            this._selenium.ClickAndWaitForPageToLoad("link=Edit");

            // Verify all the data provided was persisted
            Assert.AreEqual(phoneNumber, this._selenium.GetValue("phone"));
            Assert.AreEqual(routingNumber, this._selenium.GetValue("routing_number"));

            // Verify the account number for the check is masked up to the last four digits
            if (this._sql.IsAMSEnabled(this._infellowshipChurchID))
            {
                Assert.AreEqual("", this._selenium.GetValue("account_number"), "AMS Account Number should be empty.");
            }
            else
            {
                Assert.IsFalse(this._selenium.GetValue("account_number").Equals(accountNumber), "Account Number is not masked");
                // Commenting this out for now until I talk to Thomas about how he wants this treated
                //var maskedLastFourDigits = this._selenium.GetValue("//input[@id='account_number' and preceding-sibling::label[contains(text(), 'Account number *')]]").Substring(this._selenium.GetValue("//input[@id='account_number' and preceding-sibling::label[contains(text(), 'Account number *')]]").Length - 4);
                //Assert.AreEqual(lastFourDigits, maskedLastFourDigits);
            }
        }

        /// <summary>
        /// Performs a give now for a personal check but instead of finishing the process, edit Step 1 from Step 2.
        /// </summary>
        /// /// <param name="churchId">The church ID.</param>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="routingNumber">The routing number.</param>
        /// <param name="accountNumber">The account number.</param>
        public void Giving_GiveNow_PersonalCheck_Edit_Step1_WebDriver(int churchId, string fund, string subFund, string amount, string phoneNumber, string routingNumber, string accountNumber)
        {

            // Store the last four digits of the account number
            var lastFourDigits = accountNumber.Substring(accountNumber.Length - 4);

            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { fund = fund, subFund = subFund, amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            // View Giving History
            this._driver.FindElementByLinkText("Your Giving").Click();

            // If in Mobile view verify Schedule Giving is not visible
            var windowSize = this._driver.Manage().Window.Size;

            if (windowSize.Width == 640)
            {
                this._generalMethods.VerifyElementNotPresentWebDriver(By.LinkText("Schedule Giving"));
            }

            // Click on Give Now
            this._driver.FindElementByPartialLinkText("Give Now").Click();

            // Populate personal check information on step 1
            this.Giving_GiveNow_Populate_Step1_WebDriver(churchId, "Personal check", contributionData, null, null, null, null, null, null, null, null, null, null, null, null, null, null, phoneNumber, routingNumber, accountNumber, null, null);

            // Click to Continue
            if (this._generalMethods.IsElementVisibleWebDriver(By.XPath(GeneralInFellowship.Giving.Continue)))
            {
                this._driver.FindElementByXPath(GeneralInFellowship.Giving.Continue).Click();
            }
            else
            {
                this._driver.FindElementByXPath(GeneralInFellowship.Giving.Continue_Responsive).Click();
            }

            // Edit step 1
            this._driver.FindElementByLinkText("Edit").Click();

            // Verify all the data provided was persisted
            Assert.AreEqual(phoneNumber, this._driver.FindElementById("phone").GetAttribute("value"));
            Assert.AreEqual(routingNumber, this._driver.FindElementById("routing_number").GetAttribute("value"));

            // Verify the account number for the check is masked up to the last four digits
            if (this._sql.IsAMSEnabled(this._infellowshipChurchID))
            {
                Assert.AreEqual("", this._driver.FindElementById("account_number").GetAttribute("value"), "AMS Account Number should be empty.");
            }
            else
            {
                Assert.IsFalse(this._driver.FindElementById("account_number").GetAttribute("value").Equals(accountNumber), "Account Number is not masked");
                // Commenting this out for now until I talk to Thomas about how he wants this treated
                //var maskedLastFourDigits = this._selenium.GetValue("//input[@id='account_number' and preceding-sibling::label[contains(text(), 'Account number *')]]").Substring(this._selenium.GetValue("//input[@id='account_number' and preceding-sibling::label[contains(text(), 'Account number *')]]").Length - 4);
                //Assert.AreEqual(lastFourDigits, maskedLastFourDigits);
            }
        }

        /// <summary>
        /// This method is to populate credit card information GiveNow step after  clicking on Edit
        /// </summary>
        public void Giving_GiveNow_CreditCard_Process_Step1(string fund, string subFund, string amount)
        {
          
            
            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { fund = fund, subFund = subFund, amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            //populte step 1 details
            this.Giving_GiveNow_Populate_Step1_WebDriver(15 ,"Credit Card", contributionData, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, "United States", "6363 N Hwy 161", "Suite 200", "Irving", "Texas", "75038", string.Empty, null, null, null, null, null);
              
            /*
            //Add CC number to the payment screen

            new SelectElement(this._driver.FindElementById("payment_type_id")).SelectByText("Visa");
            this._driver.FindElementById("cc_account_number").SendKeys("4111111111111111");
            new SelectElement(this._driver.FindElementById("expiration_month")).SelectByText("12 - December");
            new SelectElement(this._driver.FindElementById("expiration_year")).SelectByText("2020"); */

            // Click to Continue
            if (this._generalMethods.IsElementVisibleWebDriver(By.XPath(GeneralInFellowship.Giving.Continue)))
            {
                this._driver.FindElementByXPath(GeneralInFellowship.Giving.Continue).Click();
            }
            else
            {
                this._driver.FindElementByXPath(GeneralInFellowship.Giving.Continue_Responsive).Click();
            }

            // Submit to attempt to process this payment. Store the time it takes to submit.
            if (this._generalMethods.IsElementVisibleWebDriver(By.XPath(GeneralInFellowship.Giving.Process_Payment)))
            {
                TestLog.WriteLine("Click on Process Payment - Non Responsive");
                this._driver.FindElementByXPath(GeneralInFellowship.Giving.Process_Payment).Click();
            }
            else
            {
                TestLog.WriteLine("Click on Process Payment - Responsive");
                this._driver.FindElementByXPath(GeneralInFellowship.Giving.Process_Payment_Responsive).Click();
            }

            DateTime paymentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMinutes(-5);
            // DateTime paymentTime = (this._selenium.ClickAndWaitForPageToLoad("//input[@value='Process this payment']")).AddMinutes(-5);

            if (!this._generalMethods.IsElementPresentWebDriver(By.XPath("//div[@class='error_msgs_for']")))
            {


                // If the payment is pending, verify it is pending and wait for it to be processed
                var paymentStatusID = this._sql.Giving_GetPaymentStatusID(15, Convert.ToDouble(amount), paymentTime, "Infellowship Give Now", 1);
                TestLog.WriteLine("Payment status is: " + paymentStatusID);
                if (paymentStatusID == 3 || paymentStatusID == 1)
                {

                    // Verify Giving History before the payment is processed
                    foreach (var item in contributionData)
                    {
                        // Validate the Giving History Page
                        this.Giving_GivingHistory_Validate_WebDriver(15, "FT", "Tester", item.fund, item.subFund, Convert.ToString(amount), paymentTime, 1, "Credit Card", false);
                    }

                }

                // Wait until the payment has processed
                this._sql.Giving_WaitUntilPaymentProcessed(15, Convert.ToDouble(amount), paymentTime, "Infellowship Give Now", 1);

                // View Give Now 
                this._driver.FindElementById(GeneralInFellowship.Giving.GivingHistory_View).Click();

                // Verify Giving History after the payment is processed
                foreach (var item in contributionData)
                {
                    // Validate the Giving History Page
                    this.Giving_GivingHistory_Validate_WebDriver(15, "FT", "Tester", item.fund, item.subFund, item.amount, paymentTime, 1, "Credit Card", true);
                }
            }


        }


        #endregion Give Now

        #region Scheduled Giving

        #region One Time
        /// <summary>
        /// Creates a one time schedule giving using a Credit card.
        /// </summary>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="scheduledDate">The day you wish this payment to be processed.</param>
        /// <param name="firstName">The first name on the credit card.</param>
        /// <param name="lastName">The last name on the credit card.</param>
        /// <param name="creditCardType">The credit card type.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <param name="securityCode">The security code on the credit card.</param>
        /// <param name="agreeToPayment">Specifies if you agree to schedule this payment.</param>
        /// <param name="churchId">The church id the contribution is being made in.  This is optional and only used for currency formatting.  Default value is 15.</param>
        public void Giving_ScheduledGiving_CreditCard_Create_OneTime(string fundOrPledgeDrive, string subFund, string amount, string scheduledDate, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, bool agreeToPayment, [Optional, DefaultParameterValue(15)] int churchId) {
            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { FundOrPledgeDrive = fundOrPledgeDrive, SubFund = subFund, Amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            // This is a one time schedule.  Store the date in an anonymous type
            var dates = new { OneTimeDate = scheduledDate };

            // Process the Schedule Giving for the credit card
            try
            {
                this.Giving_ScheduleGiving_Process(churchId, "Credit Card", GeneralEnumerations.GivingFrequency.Once, contributionData, dates, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, null, null, null, null, null, null, null, null, null, null, agreeToPayment);
            }
            catch (System.Exception e)
            {
                //If have been logged out, let's try one more time
                if (this._selenium.IsElementPresent("username"))
                {
                    this.ReLoginInFellowship(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);
                    this.Giving_ScheduleGiving_Process(churchId, "Credit Card", GeneralEnumerations.GivingFrequency.Once, contributionData, dates, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, null, null, null, null, null, null, null, null, null, null, agreeToPayment);
                }
                else
                {
                    throw new System.Exception(e.Message, e);
                }                
            }
        }

        /// <summary>
        /// Creates a one time schedule giving using a Credit card.
        /// </summary>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="scheduledDate">The day you wish this payment to be processed.</param>
        /// <param name="firstName">The first name on the credit card.</param>
        /// <param name="lastName">The last name on the credit card.</param>
        /// <param name="creditCardType">The credit card type.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <param name="securityCode">The security code on the credit card.</param>
        /// <param name="agreeToPayment">Specifies if you agree to schedule this payment.</param>
        /// <param name="churchId">The church id the contribution is being made in.  This is optional and only used for currency formatting.  Default value is 15.</param>
        public void Giving_ScheduledGiving_CreditCard_Create_OneTime_WebDriver(string fundOrPledgeDrive, string subFund, string amount, string scheduledDate, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, bool agreeToPayment, [Optional, DefaultParameterValue(15)] int churchId)
        {
            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { fund = fundOrPledgeDrive, subFund = subFund, amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            // This is a one time schedule.  Store the date in an anonymous type
            var dates = new { OneTimeDate = scheduledDate };

            // Process the Schedule Giving for the credit card
            try
            {
                this.Giving_ScheduleGiving_Process_WebDriver(churchId, "Credit Card", GeneralEnumerations.GivingFrequency.Once, contributionData, dates, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, null, null, null, null, null, null, null, null, null, null, agreeToPayment);
            }
            catch (System.Exception e)
            {
                //If have been logged out, let's try one more time
                if (this._generalMethods.IsElementPresentWebDriver(By.Id("username")))
                {
                    this.ReLoginWebDriver(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);
                    this.Giving_ScheduleGiving_Process_WebDriver(churchId, "Credit Card", GeneralEnumerations.GivingFrequency.Once, contributionData, dates, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, null, null, null, null, null, null, null, null, null, null, agreeToPayment);
                }
                else
                {
                    throw new System.Exception(e.Message, e);
                }
            }
        }

        /// <summary>
        /// Creates a one time schedule giving of using a Credit card, its scheduled date is today.
        /// Created by Jim
        /// </summary>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="scheduledDate">The day you wish this payment to be processed.</param>
        /// <param name="firstName">The first name on the credit card.</param>
        /// <param name="lastName">The last name on the credit card.</param>
        /// <param name="creditCardType">The credit card type.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <param name="securityCode">The security code on the credit card.</param>
        /// <param name="agreeToPayment">Specifies if you agree to schedule this payment.</param>
        /// <param name="churchId">The church id the contribution is being made in.  This is optional and only used for currency formatting.  Default value is 15.</param>
        public void Giving_ScheduledGiving_CreditCard_Create_OneTime_Of_Today_WebDriver(string fundOrPledgeDrive, string subFund, string amount, string scheduledDate, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, bool agreeToPayment, [Optional, DefaultParameterValue(15)] int churchId)
        {
            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { fund = fundOrPledgeDrive, subFund = subFund, amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            // This is a one time schedule.  Store the date in an anonymous type
            var dates = new { OneTimeDate = scheduledDate };

            // Process the Schedule Giving for the credit card
            try
            {
                this.Giving_ScheduleGiving_OneTime_Of_Today_Process_WebDriver(churchId, "Credit Card", contributionData, dates, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, null, null, null, null, null, null, null, null, null, null, agreeToPayment);
            }
            catch (System.Exception e)
            {
                //If have been logged out, let's try one more time
                if (this._generalMethods.IsElementPresentWebDriver(By.Id("username")))
                {
                    this.ReLoginWebDriver(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);
                    this.Giving_ScheduleGiving_OneTime_Of_Today_Process_WebDriver(churchId, "Credit Card", contributionData, dates, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, null, null, null, null, null, null, null, null, null, null, agreeToPayment);
                }
                else
                {
                    throw new System.Exception(e.Message, e);
                }
            }
        }

        /// <summary>
        /// Creates a one time schedule giving using a Credit card.
        /// </summary>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="scheduledDate">The day you wish this payment to be processed.</param>
        /// <param name="phoneNumber">The phone number for the contribution.</param>
        /// <param name="routingNumber">The routing number.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="agreeToPayment">Specifies if you agree to schedule this payment.</param>
        /// <param name="churchId">The church id the contribution is being made in.  This is optional and only used for currency formatting.  Default value is 15.</param>
        public void Giving_ScheduledGiving_PersonalCheck_Create_OneTime(string fundOrPledgeDrive, string subFund, string amount, string scheduledDate, string phoneNumber, string routingNumber, string accountNumber, bool agreeToPayment, [Optional, DefaultParameterValue(15)] int churchId) {
            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { FundOrPledgeDrive = fundOrPledgeDrive, SubFund = subFund, Amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            // This is a one time schedule.  Store the date in an anonymous type
            var dates = new { OneTimeDate = scheduledDate };

            // Process the Schedule Giving for the personal check
            this.Giving_ScheduleGiving_Process(churchId, "Personal Check", GeneralEnumerations.GivingFrequency.Once, contributionData, dates, null, null, null, null, null, null, null, null, null, null, null, null, null, null, phoneNumber, routingNumber, accountNumber, agreeToPayment);

        }

        #endregion One Time

        #region Monthly

        /// <summary>
        /// Creates a monthly frequency scheduled giving with a credit card
        /// </summary>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="startDay">The day the contribution starts.</param>
        /// <param name="startMonth">The month the contribution starts.</param>
        /// <param name="startYear">The year the contribution starts.</param>
        /// <param name="endMonth">The month the contribution ends.  This is optional.  Pass null if you do not wish to specify.</param>
        /// <param name="endYear">The year the contribution ends.  This is optional.  Pass null if you do not wish to specify.</param>
        /// <param name="firstName">The first name on the credit card.</param>
        /// <param name="lastName">The last name on the credit card.</param>
        /// <param name="creditCardType">The credit card type.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <param name="securityCode">The security code on the credit card.</param>
        /// <param name="agreeToPayment">Specifies if you agree to schedule this payment.</param>
        /// <param name="churchId">The church id the contribution is being made in.  This is optional and only used for currency formatting.  Default value is 15.</param>
        public void Giving_ScheduledGiving_CreditCard_Create_Monthly(string fundOrPledgeDrive, string subFund, string amount, string startDay, string startMonth, string startYear, string endMonth, string endYear, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, bool agreeToPayment, [Optional, DefaultParameterValue(15)] int churchId) {
            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { FundOrPledgeDrive = fundOrPledgeDrive, SubFund = subFund, Amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            // Store the dates in an anonymous type
            var dates = new { StartDay = startDay, StartMonth = startMonth, StartYear = startYear, EndMonth = endMonth, EndYear = endYear };

            // Process the Schedule Giving for the credit card
            this.Giving_ScheduleGiving_Process(churchId, "Credit Card", GeneralEnumerations.GivingFrequency.Monthly, contributionData, dates, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, null, null, null, null, null, null, null, null, null, null, agreeToPayment);

        }


        /// <summary>
        /// Creates a monthly frequency scheduled giving with a credit card
        /// </summary>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="startDay">The day the contribution starts.</param>
        /// <param name="startMonth">The month the contribution starts.</param>
        /// <param name="startYear">The year the contribution starts.</param>
        /// <param name="endMonth">The month the contribution ends.  This is optional.  Pass null if you do not wish to specify.</param>
        /// <param name="endYear">The year the contribution ends.  This is optional.  Pass null if you do not wish to specify.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="routingNumber">The routing number.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="churchId">The church id the contribution is being made in.  This is optional and only used for currency formatting.  Default value is 15.</param>
        public void Giving_ScheduledGiving_PersonalCheck_Create_Monthly(string fundOrPledgeDrive, string subFund, string amount, string startDay, string startMonth, string startYear, string endMonth, string endYear, string phoneNumber, string routingNumber, string accountNumber, bool agreeToPayment, [Optional, DefaultParameterValue(15)] int churchId) {
            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { FundOrPledgeDrive = fundOrPledgeDrive, SubFund = subFund, Amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            // Store the dates in an anonymous type
            var dates = new { StartDay = startDay, StartMonth = startMonth, StartYear = startYear, EndMonth = endMonth, EndYear = endYear };

            // Process the Schedule Giving for the personal check
            this.Giving_ScheduleGiving_Process(churchId, "Personal Check", GeneralEnumerations.GivingFrequency.Monthly, contributionData, dates, null, null, null, null, null, null, null, null, null, null, null, null, null, null, phoneNumber, routingNumber, accountNumber, agreeToPayment);

        }

        #endregion Monthly

        #region Twice Monthly

        /// <summary>
        /// Creates a twice monthly frequency scheduled giving with a credit card
        /// </summary>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="startDay">The day the contribution starts.</param>
        /// <param name="startMonth">The month the contribution starts.</param>
        /// <param name="startYear">The year the contribution starts.</param>
        /// <param name="endDay">The day the contribution ends.  This is optional.  Pass null if you do not wish to specify.</param>
        /// <param name="endMonth">The month the contribution ends.  This is optional.  Pass null if you do not wish to specify.</param>
        /// <param name="endYear">The year the contribution ends.  This is optional.  Pass null if you do not wish to specify.</param>
        /// <param name="firstName">The first name on the credit card.</param>
        /// <param name="lastName">The last name on the credit card.</param>
        /// <param name="creditCardType">The credit card type.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <param name="securityCode">The security code on the credit card.</param>
        /// <param name="agreeToPayment">Specifies if you agree to schedule this payment.</param>
        /// <param name="churchId">The church id the contribution is being made in.  This is optional and only used for currency formatting.  Default value is 15.</param>
        public void Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly(string fundOrPledgeDrive, string subFund, string amount, string startDay, string startMonth, string startYear, string endDay, string endMonth, string endYear, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, bool agreeToPayment, [Optional, DefaultParameterValue(15)] int churchId) {
            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { FundOrPledgeDrive = fundOrPledgeDrive, SubFund = subFund, Amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            // Store the dates in an anonymous type
            var dates = new { StartDay = startDay, StartMonth = startMonth, StartYear = startYear, EndDay = endDay, EndMonth = endMonth, EndYear = endYear };

            // Process the Schedule Giving for the credit card
            this.Giving_ScheduleGiving_Process(churchId, "Credit Card", GeneralEnumerations.GivingFrequency.TwiceMonthly, contributionData, dates, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, null, null, null, null, null, null, null, null, null, null, agreeToPayment);

        }


        /// <summary>
        /// Creates a twice monthly frequency scheduled giving with a credit card
        /// </summary>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="startDay">The day the contribution starts.</param>
        /// <param name="startMonth">The month the contribution starts.</param>
        /// <param name="startYear">The year the contribution starts.</param>
        /// <param name="endDay">The day the contribution ends.  This is optional.  Pass null if you do not wish to specify.</param>
        /// <param name="endMonth">The month the contribution ends.  This is optional.  Pass null if you do not wish to specify.</param>
        /// <param name="endYear">The year the contribution ends.  This is optional.  Pass null if you do not wish to specify.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="routingNumber">The routing number.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="churchId">The church id the contribution is being made in.  This is optional and only used for currency formatting.  Default value is 15.</param>
        public void Giving_ScheduledGiving_PersonalCheck_Create_TwiceMonthly(string fundOrPledgeDrive, string subFund, string amount, string startDay, string startMonth, string startYear, string endDay, string endMonth, string endYear, string phoneNumber, string routingNumber, string accountNumber, bool agreeToPayment, [Optional, DefaultParameterValue(15)] int churchId) {
            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { FundOrPledgeDrive = fundOrPledgeDrive, SubFund = subFund, Amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            // Store the dates in an anonymous type
            var dates = new { StartDay = startDay, StartMonth = startMonth, StartYear = startYear, EndDay = endDay, EndMonth = endMonth, EndYear = endYear };

            // Process the Schedule Giving for the personal check
            this.Giving_ScheduleGiving_Process(churchId, "Personal Check", GeneralEnumerations.GivingFrequency.TwiceMonthly, contributionData, dates, null, null, null, null, null, null, null, null, null, null, null, null, null, null, phoneNumber, routingNumber, accountNumber, agreeToPayment);

        }



        #endregion Twice Monthly

        #region Weekly


        /// <summary>
        /// Creates a weekly frequency scheduled giving with a credit card
        /// </summary>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="startDay">The day the contribution starts.</param>
        /// <param name="startDate">The date the contribution starts.</param>
        /// <param name="endDate">The date the contribution ends.  This is optional.  Pass null if you do not wish to specify.</param>
        /// <param name="firstName">The first name on the credit card.</param>
        /// <param name="lastName">The last name on the credit card.</param>
        /// <param name="creditCardType">The credit card type.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <param name="securityCode">The security code on the credit card.</param>
        /// <param name="agreeToPayment">Specifies if you agree to schedule this payment.</param>
        /// <param name="churchId">The church id the contribution is being made in.  This is optional and only used for currency formatting.  Default value is 15.</param>
        public void Giving_ScheduledGiving_CreditCard_Create_Weekly(string fundOrPledgeDrive, string subFund, string amount, DayOfWeek startDay, string startDate, string endDate, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, bool agreeToPayment, [Optional, DefaultParameterValue(15)] int churchId) {
            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { FundOrPledgeDrive = fundOrPledgeDrive, SubFund = subFund, Amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            // Store the dates in an anonymous type
            var dates = new { StartDay = startDay.ToString(), StartDate = startDate, EndDate = endDate };

            // Process the Schedule Giving for the credit card
            try
            {
                this.Giving_ScheduleGiving_Process(churchId, "Credit Card", GeneralEnumerations.GivingFrequency.Weekly, contributionData, dates, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, null, null, null, null, null, null, null, null, null, null, agreeToPayment);
            }
            catch (System.Exception e)
            {
                //If have been logged out, let's try one more time
                if (this._selenium.IsElementPresent("username"))
                {
                    this.ReLoginInFellowship(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);
                    this.Giving_ScheduleGiving_Process(churchId, "Credit Card", GeneralEnumerations.GivingFrequency.Weekly, contributionData, dates, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, null, null, null, null, null, null, null, null, null, null, agreeToPayment);
                }
                else
                {
                    throw new System.Exception(e.Message, e);
                }  
            }

        }

        /// <summary>
        /// Creates a weekly frequency scheduled giving with a credit card
        /// </summary>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="startDay">The day the contribution starts.</param>
        /// <param name="startDate">The date the contribution starts.</param>
        /// <param name="endDate">The date the contribution ends.  This is optional.  Pass null if you do not wish to specify.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="routingNumber">The routing number.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="agreeToPayment">Specifies if you agree to the payment.</param>
        /// <param name="churchId">The church id the contribution is being made in.  This is optional and only used for currency formatting.  Default value is 15.</param>     
        public void Giving_ScheduledGiving_PersonalCheck_Create_Weekly(string fundOrPledgeDrive, string subFund, string amount, DayOfWeek startDay, string startDate, string endDate, string phoneNumber, string routingNumber, string accountNumber, bool agreeToPayment, [Optional, DefaultParameterValue(15)] int churchId) {
            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { FundOrPledgeDrive = fundOrPledgeDrive, SubFund = subFund, Amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            // Store the dates in an anonymous type
            var dates = new { StartDay = startDay.ToString(), StartDate = startDate, EndDate = endDate };

            // Process the Schedule Giving for the personal check
            try
            {
                this.Giving_ScheduleGiving_Process(churchId, "Personal Check", GeneralEnumerations.GivingFrequency.Weekly, contributionData, dates, null, null, null, null, null, null, null, null, null, null, null, null, null, null, phoneNumber, routingNumber, accountNumber, agreeToPayment);
            }
            catch (System.Exception e)
            {
                //If have been logged out, let's try one more time
                if (this._selenium.IsElementPresent("username"))
                {
                    this.ReLoginInFellowship(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);
                    this.Giving_ScheduleGiving_Process(churchId, "Personal Check", GeneralEnumerations.GivingFrequency.Weekly, contributionData, dates, null, null, null, null, null, null, null, null, null, null, null, null, null, null, phoneNumber, routingNumber, accountNumber, agreeToPayment);
                }
                else
                {
                    throw new System.Exception(e.Message, e);
                }  

            }
        }

        #endregion Weekly

        #region Every Two Weeks

        /// <summary>
        /// Creates a twice weekly frequency scheduled giving with a credit card
        /// </summary>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="startDay">The day the contribution starts.</param>
        /// <param name="startDate">The date the contribution starts.</param>
        /// <param name="endDate">The date the contribution ends.  This is optional.  Pass null if you do not wish to specify.</param>
        /// <param name="firstName">The first name on the credit card.</param>
        /// <param name="lastName">The last name on the credit card.</param>
        /// <param name="creditCardType">The credit card type.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <param name="securityCode">The security code on the credit card.</param>
        /// <param name="agreeToPayment">Specifies if you agree to schedule this payment.</param>
        /// <param name="churchId">The church id the contribution is being made in.  This is optional and only used for currency formatting.  Default value is 15.</param>
        public void Giving_ScheduledGiving_CreditCard_Create_EveryTwoWeeks(string fundOrPledgeDrive, string subFund, string amount, DayOfWeek startDay, string startDate, string endDate, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, bool agreeToPayment, [Optional, DefaultParameterValue(15)] int churchId) {
            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { FundOrPledgeDrive = fundOrPledgeDrive, SubFund = subFund, Amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            // Store the dates in an anonymous type
            var dates = new { StartDay = startDay.ToString(), StartDate = startDate, EndDate = endDate };

            // Process the Schedule Giving for the credit card
            this.Giving_ScheduleGiving_Process(churchId, "Credit Card", GeneralEnumerations.GivingFrequency.EveryTwoWeeks, contributionData, dates, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, null, null, null, null, null, null, null, null, null, null, agreeToPayment);

        }

        /// <summary>
        /// Creates an every two weeks frequency scheduled giving with a credit card
        /// </summary>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="startDay">The day the contribution starts.</param>
        /// <param name="startDate">The date the contribution starts.</param>
        /// <param name="endDate">The date the contribution ends.  This is optional.  Pass null if you do not wish to specify.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="routingNumber">The routing number.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="churchId">The church id the contribution is being made in.  This is optional and only used for currency formatting.  Default value is 15.</param>
        public void Giving_ScheduledGiving_PersonalCheck_Create_EveryTwoWeeks(string fundOrPledgeDrive, string subFund, string amount, DayOfWeek startDay, string startDate, string endDate, string phoneNumber, string routingNumber, string accountNumber, bool agreeToPayment, [Optional, DefaultParameterValue(15)] int churchId) {
            // Conver the single fund/pledge drive, subfund, and amount to an anonymous type
            var singleContribution = new { FundOrPledgeDrive = fundOrPledgeDrive, SubFund = subFund, Amount = amount };

            // Even though we are just passing in one set of contribution data, we still need to store this in a list.
            IList<dynamic> contributionData = new List<dynamic>();
            contributionData.Add(singleContribution);

            // Store the dates in an anonymous type
            var dates = new { StartDay = startDay.ToString(), StartDate = startDate, EndDate = endDate };

            // Process the Schedule Giving for the personal check
            this.Giving_ScheduleGiving_Process(churchId, "Personal Check", GeneralEnumerations.GivingFrequency.EveryTwoWeeks, contributionData, dates, null, null, null, null, null, null, null, null, null, null, null, null, null, null, phoneNumber, routingNumber, accountNumber, agreeToPayment);

        }

        #endregion Every Two Weeks

        /// <summary>
        /// Views a scheduled contribution for Schedule Giving.
        /// </summary>
        /// <param name="amount">The amount of the contribution that you wish to view.</param>
        public void Giving_ScheduledGiving_View(string amount) {

            // Are we already viewing schedules or can we click on the giving schedules' tab?
            if (!this._selenium.IsElementPresent(TableIds.InFellowship_GivingSchedules) || !this._selenium.IsElementPresent("link=Schedules"))
            {
                // We aren't and cannot click the tab.  View your Giving
                // updated by demi.zhang - from "Link=Your Giving" to "link=Giving".
                this._selenium.ClickAndWaitForPageToLoad("link=Giving");
            }
            
            //If we are taken to login
            this.ReLoginInFellowship(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);

            //If taken to home landing page, click on Your Giving.
            //Probably a better way to handle this but for now coding it this way
            if (this._selenium.IsElementPresent("link=Your Giving"))
            {
                    this._selenium.ClickAndWaitForPageToLoad("link=Your Giving");
            }

            //If we are taken to login
            this.ReLoginInFellowship(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);

            // Click the Giving Schedules tab
            this._selenium.ClickAndWaitForPageToLoad("link=Schedules");

            //If we are taken to login
            this.ReLoginInFellowship(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);

            // Store the item row
            decimal itemRow = this._generalMethods.GetTableRowNumber(TableIds.InFellowship_GivingSchedules, string.Format("{0:c}", Convert.ToDecimal(amount)), "Amount", null);

            // Click on the link
            this._selenium.ClickAndWaitForPageToLoad(string.Format("{0}/tbody/tr[{1}]/td[1]/a", TableIds.InFellowship_GivingSchedules, itemRow + 1));

        }

        /// <summary>
        /// Views a scheduled contribution for Schedule Giving.
        /// </summary>
        /// <param name="amount">The amount of the contribution that you wish to view.</param>
        public void Giving_ScheduledGiving_View_WebDriver(string amount)
        {

            // Are we already viewing schedules or can we click on the giving schedules' tab?
            if (!this._generalMethods.IsElementPresentWebDriver(By.XPath(TableIds.InFellowship_GivingSchedules)) || !this._generalMethods.IsElementPresentWebDriver(By.LinkText("Schedules")))
            {
                // We aren't and cannot click the tab.  View your Giving
                this._driver.FindElementByXPath(Navigation.InFellowship.Your_Giving).Click();
            }

            // Click the Giving Schedules tab
            this._driver.FindElementByLinkText(Navigation.InFellowship.Your_Giving_GivingSchedules_Tab).Click();

            // Store the item row
            decimal itemRow = this._generalMethods.GetTableRowNumberWebDriver(TableIds.InFellowship_GivingSchedules, string.Format("{0:c}", Convert.ToDecimal(amount)), "Amount", null);

            // Click on the link
            this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[1]/a", TableIds.InFellowship_GivingSchedules, itemRow)).Click();

        }

        /// <summary>
        /// Updates a scheduled contribution's status to Active or Inactive.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="amount">The amount of the contribution you wish to edit.</param>
        /// <param name="isActive">Specifies if this contribution is active or inactive.</param>
        public void Giving_ScheduledGiving_Edit_Status(int churchId, string amount, bool makeActive) {

            // View the Schedule
            this.Giving_ScheduledGiving_View(amount);

            // Do wish to enable or disable the schedule?
            if (makeActive) {
                // We wish to activate it.  Is it already enabled?
                if (!this._selenium.IsVisible("link=Pause this schedule")) {
                    // It isn't.  Activate it.
                    this._selenium.Click("link=Resume processing");
                }

                //this._selenium.WaitForCondition("!selenium.isElementPresent('//span[@id='next_contribution' and @style='text-decoration: line-through;']')", "10000");
                // Verify the next contribution does not have a strike through effect.
                this._selenium.VerifyElementNotPresent("//span[@id='next_contribution' and @style='text-decoration: line-through;']");

                // Verify the contribution is active
                Assert.IsTrue(this._sql.Giving_ScheduledGiving_IsScheduleActive(churchId, Convert.ToDouble(amount)), "Schedule was made active but was still inactive in the DB!");
            }
            else {
                // We wish to disable the schedule. Is it already disabled?
                if (!this._selenium.IsVisible("link=Resume processing")) {
                    // It isn't. Deactivate it.
                    this._selenium.Click("link=Pause this schedule");
                }


                // Verify the next contribution has a strike through effect.
                this._selenium.VerifyElementPresent("//span[@id='next_contribution' and @style='text-decoration: line-through;']");

                System.Threading.Thread.Sleep(5000);
                // Verify the contribution is inactive
                Assert.IsFalse(this._sql.Giving_ScheduledGiving_IsScheduleActive(churchId, Convert.ToDouble(amount)), "Schedule was made inactive but was still active in the DB!");
            }

            // Back
            this._selenium.ClickAndWaitForPageToLoad("link=Back");

            //Verify the status icon is only displayed
            //F1-4279 Remove "Test" from Scheduled Giving status in InFellowship
            decimal itemRow = this._generalMethods.GetTableRowNumber(TableIds.InFellowship_GivingSchedules, string.Format("{0:c}", Convert.ToDecimal(amount)), "Amount", null);

            // Status Icon
            if (makeActive)
            {
                this._selenium.IsElementPresent(string.Format("{0}/tbody/tr[{1}]/td[5]/img[contains(@src, '/images/icon_complete.png')]", TableIds.InFellowship_GivingSchedules, itemRow + 1));
            }
            else
            {
                this._selenium.IsElementPresent(string.Format("{0}/tbody/tr[{1}]/td[5]/img[contains(@src, '/images/icon_pause.png')]", TableIds.InFellowship_GivingSchedules, itemRow + 1));
            }

            // Verify that we only have icon and not random texts
            Assert.AreEqual("", this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.InFellowship_GivingSchedules, itemRow + 1)), "Text is displayed in Status column");

        }


        /// <summary>
        /// Updates the payment information for a scheduled giving for credit cards.
        /// </summary>
        /// <param name="scheduleAmount">The schedule with this amount to be updated.</param>
        /// <param name="updatedFirstName">The updated first name.</param>
        /// <param name="updatedLastName">The updated last name.</param>
        /// <param name="updatedCreditCardType">The updated credit card type.</param>
        /// <param name="updatedCreditCardNumber">The updated credit card number.</param>
        /// <param name="updatedExpirationMonth">The updated expiration month.</param>
        /// <param name="updatedExpirationYear">The updated expiration year.</param>
        /// <param name="updatedSecurityCode">The updated security code.</param>
        /// <param name="updatedCountry">The updated country.</param>
        /// <param name="updatedStreetOne">The update street one.</param>
        /// <param name="updatedStreetTwo">The updates street two.</param>
        /// <param name="updatedCity">The updated city.</param>
        /// <param name="updatedState">The updated state.</param>
        /// <param name="updatedPostalCode">The updated postal code.</param>
        /// <param name="updatedCounty">The updated county.</param>
        public void Giving_ScheduledGiving_Edit_CreditCard_PaymentInformation(string scheduleAmount, string updatedFirstName, string updatedLastName, string updatedCreditCardType, string updatedCreditCardNumber, string updatedExpirationMonth, string updatedExpirationYear, string updatedSecurityCode, string updatedCountry, string updatedStreetOne, string updatedStreetTwo, string updatedCity, string updatedState, string updatedPostalCode, string updatedCounty) {

            // View the schedule
            this.Giving_ScheduledGiving_View(scheduleAmount);

            // Click the edit payment information link
            this._selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Payment')]");

            // Update the payment information
            this.Giving_ScheduledGiving_Populate_PaymentInformation("Credit Card", updatedFirstName, updatedLastName, updatedCreditCardType, updatedCreditCardNumber, updatedExpirationMonth, updatedExpirationYear, updatedSecurityCode, updatedCountry, updatedStreetOne, updatedStreetTwo, updatedCity, updatedState, updatedPostalCode, updatedCounty, null, null, null);

            // Submit
            this._selenium.ClickAndWaitForPageToLoad("//input[@name='commit']");

            // Verify
            if (!string.IsNullOrEmpty(updatedFirstName)) {
                this._selenium.VerifyTextPresent(updatedFirstName);
            }
            if (!string.IsNullOrEmpty(updatedLastName)) {
                this._selenium.VerifyTextPresent(updatedLastName);
            }

            if (!string.IsNullOrEmpty(updatedCreditCardType)) {
                if (updatedCreditCardType == "Master Card") {
                    updatedCreditCardType = "Mastercard";
                }
                if (updatedCreditCardType == "American Express") {
                    updatedCreditCardType = "AMEX";
                }
                this._selenium.VerifyTextPresent(updatedCreditCardType);
            }

            if (!string.IsNullOrEmpty(updatedCreditCardNumber)) {
                var lastFourDigits = updatedCreditCardNumber.Substring(updatedCreditCardNumber.Length - 4);
                this._selenium.VerifyTextPresent(lastFourDigits);
            }

            if (!string.IsNullOrEmpty(updatedExpirationMonth)) {
                Assert.IsTrue(this._selenium.GetText("//div[@class='gutter_left_half']/table[@class='grid']/tbody/tr[4]").Contains(updatedExpirationMonth), "Expiration Month was not updated!");
            }


            if (!string.IsNullOrEmpty(updatedExpirationYear)) {
                Assert.IsTrue(this._selenium.GetText("//div[@class='gutter_left_half']/table[@class='grid']/tbody/tr[4]").Contains(updatedExpirationYear), "Expiration Year was not updated!");
            }

            if (!string.IsNullOrEmpty(updatedSecurityCode)) {
                this._selenium.VerifyTextPresent(updatedSecurityCode);
            }

            if (!string.IsNullOrEmpty(updatedCountry)) {
                this._selenium.VerifyTextPresent(updatedCountry);

            }

            if (!string.IsNullOrEmpty(updatedStreetOne)) {
                this._selenium.VerifyTextPresent(updatedStreetOne);
            }

            if (!string.IsNullOrEmpty(updatedStreetTwo)) {
                this._selenium.VerifyTextPresent(updatedStreetTwo);
            }

            if (!string.IsNullOrEmpty(updatedCity)) {
                this._selenium.VerifyTextPresent(updatedCity);
            }

            if (!string.IsNullOrEmpty(updatedState)) {
                this._selenium.VerifyTextPresent(updatedState);
            }

            if (!string.IsNullOrEmpty(updatedPostalCode)) {
                this._selenium.VerifyTextPresent(updatedPostalCode);
            }

            if (!string.IsNullOrEmpty(updatedCounty)) {
                this._selenium.VerifyTextPresent(updatedCounty);
            }

            // Back
            this._selenium.ClickAndWaitForPageToLoad("link=Back");

        }


        /// <summary>
        /// Updates the payment information for a scheduled giving for personal checks.
        /// </summary>
        /// <param name="scheduleAmount">The schedule with this amount to be updated.</param>
        /// <param name="updatedPhoneNumber">The updated phone number.</param>
        /// <param name="updatedRoutingNumber">The updated routing number.</param>
        /// <param name="updatedAccountNumber">The updated account number.</param>
        public void Giving_ScheduledGiving_Edit_PersonalCheck_PaymentInformation(string scheduleAmount, string updatedPhoneNumber, string updatedRoutingNumber, string updatedAccountNumber) {

            // View the schedule
            this.Giving_ScheduledGiving_View(scheduleAmount);

            // Click the edit payment information link
            this._selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Payment')]");

            // Update the payment information
            this.Giving_ScheduledGiving_Populate_PaymentInformation("Personal Check", null, null, null, null, null, null, null, null, null, null, null, null, null, null, updatedPhoneNumber, updatedRoutingNumber, updatedAccountNumber);

            // Submit
            this._selenium.ClickAndWaitForPageToLoad("//input[@name='commit']");

            // Verify
            Assert.Inconclusive("No validation has occured.");

            // Back
            this._selenium.ClickAndWaitForPageToLoad("link=Back");

        }

        /// <summary>
        /// Updates the fund/pledge drive, subfund, and/or amount for an existing schedule.
        /// </summary>
        /// <param name="originalAmount">The original amount of the schedule you want to edit.</param>
        /// <param name="updatedFundOrPledgeDrive">The updated fund or pledge drive.</param>
        /// <param name="updatedSubFund">The updated subfund.</param>
        /// <param name="updatedAmount">The updated amount.</param>
        public void Giving_ScheduledGiving_Edit_WhereToGive(string originalAmount, string updatedFundOrPledgeDrive, string updatedSubFund, string updatedAmount) {

            //If we are in login page then reLogin infellowship again
            if (this._selenium.IsElementPresent("username"))
            {
                this.ReLoginInFellowship(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);
            }

            // View the schedule
            this.Giving_ScheduledGiving_View(originalAmount);

            // Click the edit where to give link
            this._selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Where')]");

            // Select a new fund.  We are assuming you are updating the first one.
            this._selenium.Select("fund", updatedFundOrPledgeDrive);

            // Select a new subfund unless it is null
            if (!string.IsNullOrEmpty(updatedSubFund)) {
                this._selenium.Select("sub_fund", updatedSubFund);
            }

            // Type in the new amount
            this._selenium.Type("amount", updatedAmount);

            // Submit
            this._selenium.ClickAndWaitForPageToLoad("//input[@name='commit']");

            // If there is validation, verify the messages
            if (this._selenium.IsElementPresent("//div[@class='error_msgs_for']")) {
                if (string.IsNullOrEmpty(updatedFundOrPledgeDrive)) {
                    this._selenium.VerifyTextPresent("Give to... is required");
                }
                else if (string.IsNullOrEmpty(updatedAmount)) {
                    this._selenium.VerifyTextPresent("Please enter a valid amount");
                }

                // Cancel
                this._selenium.ClickAndWaitForPageToLoad("link=Cancel");

                // Back
                this._selenium.ClickAndWaitForPageToLoad("link=Back");
            }
            else {

                // Back
                this._selenium.ClickAndWaitForPageToLoad("link=Back");

                // Verify the schedule reflects the new amount
                Assert.IsTrue(this._generalMethods.ItemExistsInTable(TableIds.InFellowship_GivingSchedules, string.Format("${0}", updatedAmount), "Amount"), "Updated amount was not reflected on the Schedules page.");
            }
        }


        /// <summary>
        /// Deletes a scheduled contribution in InFellowship.
        /// </summary>
        /// <param name="amount">The amount of the contribution to be deleted.</param>
        public void Giving_ScheduledGiving_Delete(string amount) {
            this.Giving_ScheduledGiving_View(amount);

            // Delete the schedule
            this._selenium.Click("link=Delete this schedule");
            Assert.IsTrue(Regex.IsMatch(this._selenium.GetConfirmation(), "^Are you sure you want to delete this schedule[\\s\\S]$"));
            this._selenium.WaitForPageToLoad("30000");

            // Verify the schedule was deleted
            Assert.IsTrue(this._selenium.IsTextPresent("Giving Schedule Deleted"));
            if (this._selenium.IsElementPresent(TableIds.InFellowship_GivingSchedules)) {
                Assert.IsFalse(this._generalMethods.ItemExistsInTable(TableIds.InFellowship_GivingSchedules, string.Format("{0:c}", amount), "Amount"), "Deleted schedule was still present!");
            }
            else {
                this._selenium.VerifyTextPresent("No giving schedules found");
            }
        }

        /// <summary>
        /// Deletes a scheduled contribution in InFellowship.
        /// </summary>
        /// <param name="amount">The amount of the contribution to be deleted.</param>
        public void Giving_ScheduledGiving_Delete_WebDriver(string amount)
        {
            this.Giving_ScheduledGiving_View_WebDriver(amount);

            // Delete the schedule
            this._driver.FindElementByLinkText("Delete this schedule").Click();
            this._driver.SwitchTo().Alert().Accept();
            //this._generalMethods.Popups_ConfirmationWebDriver("Yes");
            //Assert.IsTrue(Regex.IsMatch(this._selenium.GetConfirmation(), "^Are you sure you want to delete this schedule[\\s\\S]$"));
            //this._selenium.WaitForPageToLoad("30000");

            // Verify the schedule was deleted
            Assert.IsTrue(this._generalMethods.IsElementPresentWebDriver(By.Id("success_message_inner")));
            if (this._generalMethods.IsElementPresentWebDriver(By.XPath(TableIds.InFellowship_GivingSchedules)))
            {
                Assert.IsFalse(this._generalMethods.ItemExistsInTableWebDriver(TableIds.InFellowship_GivingSchedules, string.Format("{0:c}", amount), "Amount"), "Deleted schedule was still present!");
            }
            else
            {
                this._generalMethods.VerifyTextPresentWebDriver("No giving schedules found");
            }
        }

        #endregion Scheduled Giving

        #endregion Giving

        #region Groups

        #region Span of Care
        /// <summary>
        /// Views a Span of Care you own.
        /// </summary>
        /// <param name="socName">The name of the Span of Care.</param>
        public void Groups_SpanOfCare_View(string socName) {
            // See if you are on the landing page
            if (this._selenium.IsElementPresent(GeneralInFellowshipConstants.LandingPage.Link_YourGroups)) {
                this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourGroups);
                this._selenium.ClickAndWaitForPageToLoad("link=" + socName);
            }
            // See if you are on the groups dashboard
            else if (this._selenium.IsElementPresent("link=" + socName)) {
                this._selenium.ClickAndWaitForPageToLoad("link=" + socName);
            }
            // Just click Home and navigate through.
            else {
                this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
                this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourGroups);
                this._selenium.ClickAndWaitForPageToLoad("link=" + socName);
            }
        }

        /// <summary>
        /// Views a Span of Care you own.
        /// </summary>
        /// <param name="socName">The name of the Span of Care.</param>
        public void Groups_SpanOfCare_View_WebDriver(string socName) {
            // See if you are on the landing page
            if (this._generalMethods.IsElementPresentWebDriver(By.LinkText("Your Groups"))) {
                this._driver.FindElementByLinkText("Your Groups").Click();
                this._driver.FindElementByLinkText(socName).Click();
            }
            // See if you are on the groups dashboard
            else if (this._generalMethods.IsElementPresentWebDriver(By.LinkText(socName))) {
                this._driver.FindElementByLinkText(socName).Click();
            }
            // Just click Home and navigate through.
            else {
                this._driver.FindElementByLinkText("HOME").Click();
                this._driver.FindElementByLinkText("Your Groups").Click();
                this._driver.FindElementByLinkText(socName).Click();
            }
        }

        /// <summary>
        /// Views the emails all leaders page for a Span of Care you own.
        /// </summary>
        /// <param name="socName">The name of the Span of Care.</param>
        public void Groups_SpanOfCare_View_EmailLeaders(string socName) {

            // View a Span of Care
            this.Groups_SpanOfCare_View(socName);

            // Send an email to all leaders
            this._selenium.Click(InFellowshipConstants.Groups.GroupsConstants.SpanOfCareDashboard.Gear_GearMenu);
            this._selenium.ClickAndWaitForPageToLoad(InFellowshipConstants.Groups.GroupsConstants.SpanOfCareDashboard.Link_EmailAllLeaders);
        }

        /// <summary>
        /// Views the prospect data page for a Span of Care your own.
        /// </summary>
        /// <param name="socName">The name of the Span of Care.</param>
        public void Groups_SpanOfCare_View_ProspectsData(string socName) {
            // View the Span of Care
            this.Groups_SpanOfCare_View(socName);

            // Select the Prospect's link
            this._selenium.ClickAndWaitForPageToLoad(InFellowshipConstants.Groups.GroupsConstants.SpanOfCareDashboard.Link_Prospects);
        }

        /// <summary>
        /// Views the attendance tab for a span of care
        /// </summary>
        /// <param name="socName">The name of the Span of Care.</param>
        public void Groups_Groups_SpanOfCare_View_Attendance(string socName) {
            this.Groups_SpanOfCare_View(socName);
            this._selenium.ClickAndWaitForPageToLoad("link=Attendance");
        }

        /// <summary>
        /// Views the prospects of a group you own by clicking on the numerical link on the prospect data page for a Span of Care you own.
        /// </summary>
        /// <param name="socName">The name of the Span of Care.</param>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_SpanOfCare_View_Group_Prospects_From_Prospect_Data_Page(string socName, string groupName) {

            // View the prospect data of a Span of Care you own
            this.Groups_SpanOfCare_View_ProspectsData(socName);

            //
            var text = string.Format("css=tr td label:contains(\"{0}\") a", groupName);
            // Click the prospect link
            // Table does not have header text, so we need to use XPaths.
            int numberOfRows = (Int32)this._selenium.GetXpathCount(TableIds.InFellowship_SpanOfCare_Prospects_GroupList + "/tbody/tr/td[1]");

            // Figure out which link to click.
            for (int rowPosition = 1; rowPosition < numberOfRows; rowPosition++) {
                if (this._selenium.GetText(TableIds.InFellowship_SpanOfCare_Prospects_GroupList + "/tbody/tr[" + rowPosition + "]/td[1]") == groupName) {
                    if (this._selenium.IsElementPresent(TableIds.InFellowship_SpanOfCare_Prospects_GroupList + "/tbody/tr[" + rowPosition + "]/td[2]/a") == true) {
                        this._selenium.ClickAndWaitForPageToLoad(TableIds.InFellowship_SpanOfCare_Prospects_GroupList + "/tbody/tr[" + rowPosition + "]/td[2]/a");
                        break;
                    }
                }
            }
        }

        #endregion Span of Care

        #region Group Attendance
        /// <summary>
        /// Views a group's attendance
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Attendance_View(string groupName) {

            try
            {
                // View the group
                this.Groups_Group_View(groupName);
            }
            catch (System.Exception e)
            {
                //If have been logged out, let's try one more time
                if (this._selenium.IsElementPresent("username"))
                {
                    this.ReLoginInFellowship(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);
                    this.Groups_Group_View(groupName);
                }
                else
                {
                    throw new System.Exception(e.Message, e);
                }
                
            }

            // Click the Attendance Tab
            this._selenium.ClickAndWaitForPageToLoad("link=Attendance");
        }
        
        /// <summary>
        /// Views a group's attendance
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Attendance_View_WebDriver(string groupName)
        {
            try
            {
                // View the group
                this.Groups_Group_View_WebDriver(groupName);
            }
            catch (System.Exception e)
            {
                //If have been logged out, let's try one more time
                if (this._generalMethods.IsElementPresentWebDriver(By.Id("username")))
                {
                    this.ReLoginWebDriver(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);
                    this.Groups_Group_View_WebDriver(groupName);
                }
                else
                {
                    throw new System.Exception(e.Message, e);
                }
            }

            // Click the Attendance Tab
            this._driver.FindElementByLinkText("Attendance").Click();
        }
        

        /// <summary>
        /// Views a group's attendance for a group under your span of care.
        /// </summary>
        /// <param name="socName">The name of your span of care.</param>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Attendance_View(string socName, string groupName) {
            // View the group
            this.Groups_Group_View(socName, groupName);

            // Click the Attendance Tab
            this._selenium.ClickAndWaitForPageToLoad("link=Attendance");
        }

        /// <summary>
        /// Views a date for a given attendance record
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="attendanceDate">The attendance date.</param>
        public void Groups_Attendance_View_Date(string groupName, string attendanceDate) {
            // Regex out the end time and day.
            var attendanceDateNoDay = Regex.Replace(attendanceDate, "(Satur|Sun|Mon|Tues|Wednes|Thurs|Fri){1}(day, )", string.Empty);
            var attendanceDateNoEndTime = Regex.Replace(attendanceDateNoDay, "(\\s? - \\s?[0-9]?[0-9]:[0-9][0-9]\\s[A|P]M)", string.Empty);


            // If the link is present, just click it.  Otherwise view attendance and click the link
            if (this._selenium.IsElementPresent(string.Format("link={0}", attendanceDateNoEndTime))) {
                this._selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", attendanceDateNoEndTime, string.Empty));
            }

            else {
                // View the attendance for a group
                this.Groups_Attendance_View(groupName);

                // View the attendance date
                this._selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", attendanceDateNoEndTime));
            }
        }

        /// <summary>
        /// Posts attendance for a group for a given date.  Does not specify individuals but instead clicks Check all.  If no date is specified, the first date in the drop down will be selected.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="scheduleDate">The schedule date the group met that you are posting attendance for. Not specifying a date will select the first date in the drop down.</param>
        /// <param name="didGroupMeet">Did the group meet?</param>
        public void Groups_Attendance_Enter_All(string groupName, string scheduleDate, bool didGroupMeet) {
            // View the group's attendance page
            this.Groups_Attendance_View(groupName);

            // Enter attendance
            this._selenium.ClickAndWaitForPageToLoad("link=Enter attendance");

            // Variables
            var attendanceDate = scheduleDate;
            var attendanceDateNoDayEndTime = string.Empty;
            var attendanceDateNoDayNoEndTime = string.Empty;

            //Added thsi repalcement sicne the UI in infellwoship as "to" instead of "-"
            var attendanceDate1 = Regex.Replace(attendanceDate, "-", "to");
            TestLog.WriteLine("Attendance Date : {0}", attendanceDate1);
            // If no date is specified, use the first date in the drop down
            if (string.IsNullOrEmpty(scheduleDate)) {
                // Store the first date in the drop down
                attendanceDate = this._selenium.GetText("//select[@id='post_attendance']//optgroup[@label='Choose an event']/option[1]");
            }

            // Select the date
            this._selenium.SelectAndWaitForCondition("post_attendance", attendanceDate1, this._javascript.IsElementPresent("attendance_roster"), "20000");
            //this._selenium.("//select[@id='post_attendance']");
            //this._selenium.Select("post_attendance", string.Format("label={0}",attendanceDate));

            // Regex the date for various purposes
            attendanceDateNoDayEndTime = Regex.Replace(attendanceDate, "(Satur|Sun|Mon|Tues|Wednes|Thurs|Fri){1}(day, )", string.Empty);
            attendanceDateNoDayNoEndTime = Regex.Replace(attendanceDateNoDayEndTime, "(\\s? - \\s?[0-9]?[0-9]:[0-9][0-9]\\s[A|P]M)", string.Empty);
            

            // If the group met, check all and submit.  If it didn't meet, click no and hit submit
            if (didGroupMeet) {
                // this._selenium.Click("group_attendance_yes");
                // this._selenium.WaitForCondition(this._javascript.IsElementPresent("attendance_roster"), "5000");
                this._selenium.ClickAndWaitForPageToLoad("group_attendance_yes");
                this._selenium.WaitForCondition("selenium.isVisible(\"xpath=//div[@id='attendance_roster']\");", "20000");

                this._selenium.Click("//input[@type='checkbox']");
               // this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
                //SP - changed this since ID value is changed in UI for Attendance Save button....
                this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.Attendance_submitQuery);

                
                // Verify the attendance was posted
                // Changed how we verify things
                // Method uses an xpath that is not suitable since we now have a link
                // i.e. //table[@class='grid']/tbody/tr[*]/td[position()=1 and (normalize-space(string(.))='October 29, 2014 at 7:31 PM' or normalize-space(string(./*))='October 29, 2014 at 7:31 PM')] will return October Oct 29 , 2014 at 19:31:00 October 29, 2014 at 7:31 PM
                // i.e. //table[@class='grid']/tbody/tr[*]/td[position()=1 and (normalize-space(string(.))='October 29, 2014 at 7:31 PM' or normalize-space(string(./*))='October 29, 2014 at 7:31 PM')]/a will return October 29, 2014 at 7:31 PM
                Assert.IsTrue(this._generalMethods.ItemExistsInTable(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayNoEndTime, "Met on", "contains"), "The group met for a given attendance date but this data was not shown on the Attendance view page!");

                // If there was an endtime, verifiy it is not present
                if (attendanceDateNoDayNoEndTime != attendanceDateNoDayEndTime) {
                    Assert.IsFalse(this._generalMethods.ItemExistsInTable(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayEndTime, "Met on", "contains"), "End time was present.");
                }

            }
            else {
                this._selenium.Click("group_attendance_no");
                this._selenium.Type("group_attendance_comments", string.Format("The group did not meet on {0}.", scheduleDate));
               // this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
                //SP - changed this since ID value is changed in UI for Attendance Save button....
                this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.Attendance_submitQuery);

                // Verify the attendance was posted for this date under the "Did not meet" table.
                Assert.IsTrue(this._generalMethods.ItemExistsInTable(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayNoEndTime, "Met on", "contains"), "The group did not meet for a given attendance date but this data was not shown on the Attendance view page!");
                
                // If there was an endtime, verifiy it is not present
                if (attendanceDateNoDayNoEndTime != attendanceDateNoDayEndTime) {
                    Assert.IsFalse(this._generalMethods.ItemExistsInTable(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayEndTime, "Met on", "contains"), "End time was present.");
                }
            }

        }

        /// <summary>
        /// Posts attendance for a group for a given date.  Does not specify individuals but instead clicks Check all.  If no date is specified, the first date in the drop down will be selected.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="scheduleDate">The schedule date the group met that you are posting attendance for. Not specifying a date will select the first date in the drop down.</param>
        /// <param name="didGroupMeet">Did the group meet?</param>
        public void Groups_Attendance_Enter_All_WebDriver(string groupName, string scheduleDate, bool didGroupMeet)
        {
            // View the group's attendance page
            this.Groups_Attendance_View_WebDriver(groupName);

            // Enter attendance
            this._driver.FindElementByLinkText("Enter attendance").Click();

            // Variables
            var attendanceDate = scheduleDate;
            var attendanceDateNoDayEndTime = string.Empty;
            var attendanceDateNoDayNoEndTime = string.Empty;
            var attendanceDate1 = string.Empty;

            //Added thsi repalcement sicne the UI in infellwoship as "to" instead of "-"
            // If no date is specified, use the first date in the drop down
            if (string.IsNullOrEmpty(scheduleDate))
            {
                // Store the first date in the drop down
                attendanceDate1 = this._driver.FindElementByXPath("//select[@id='post_attendance']//optgroup[@label='Choose an event']/option[1]").Text;
            }
            else
            {
                attendanceDate1 = Regex.Replace(attendanceDate, "-", "to");
                TestLog.WriteLine("Attendance Date : {0}", attendanceDate1);

            }

            // Select the date
            new SelectElement(this._driver.FindElementById("post_attendance")).SelectByText(attendanceDate1);
            this._generalMethods.WaitForElement(By.Id("attendance_roster"));


            // Regex the date for various purposes
            attendanceDateNoDayEndTime = Regex.Replace(attendanceDate1, "(Satur|Sun|Mon|Tues|Wednes|Thurs|Fri){1}(day, )", string.Empty);
            attendanceDateNoDayNoEndTime = Regex.Replace(attendanceDateNoDayEndTime, "(\\s? - \\s?[0-9]?[0-9]:[0-9][0-9]\\s[A|P]M)", string.Empty);

            // If the group met, check all and submit.  If it didn't meet, click no and hit submit
            if (didGroupMeet)
            {
                this._driver.FindElementById("group_attendance_yes").Click();

                // Select individuals based on mobile view or not
                var windowSize = this._driver.Manage().Window.Size;

                if (windowSize.Width == 640)
                {
                    IWebElement table = this._driver.FindElementByXPath(TableIds.InFellowship_Group_Attendance_Roster_Responsive);
                    int rowsTbl = table.FindElements(By.TagName("tr")).Count;

                    for (int r = 1; r < rowsTbl; r++)
                    {
                        //this._driver.FindElementByXPath("//td[@class='pinch_left clickable name']").Click();
                        table.FindElements(By.TagName("tr"))[r].FindElements(By.TagName("td"))[2].Click();
                        //table.FindElements(By.TagName("tr"))[r].FindElement(By.XPath("//td[@class='pinch_left clickable name']")).Click();
                    }

                    //Screen shot to see if they all got selected
                    this._generalMethods.TakeScreenShot_WebDriver();

                }
                else
                {
                    this._driver.FindElementByXPath("//input[@type='checkbox']").Click();
                }

                int attendanceNum = this._generalMethods.GetTableRowCountWebDriver(TableIds.InFellowship_Group_Attendance_Roster_Responsive) - 1;

                // this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);                         
                var btn = this._driver.FindElementsById(GeneralButtons.Attendance_submitQuery).FirstOrDefault(x => x.Displayed == true);
                btn.Click();  

                // Verify the attendance was posted
                // Changed how we verify things
                // Method uses an xpath that is not suitable since we now have a link
                // i.e. //table[@class='grid']/tbody/tr[*]/td[position()=1 and (normalize-space(string(.))='October 29, 2014 at 7:31 PM' or normalize-space(string(./*))='October 29, 2014 at 7:31 PM')] will return October Oct 29 , 2014 at 19:31:00 October 29, 2014 at 7:31 PM
                // i.e. //table[@class='grid']/tbody/tr[*]/td[position()=1 and (normalize-space(string(.))='October 29, 2014 at 7:31 PM' or normalize-space(string(./*))='October 29, 2014 at 7:31 PM')]/a will return October 29, 2014 at 7:31 PM
                if (windowSize.Width == 640)
                {
                    Assert.IsTrue(this._generalMethods.ItemExistsInTableResponsiveWebDriver(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayNoEndTime, "Met on"), "The group met for a given attendance date but this data was not shown on the Attendance view page!");

                    // If there was an endtime, verifiy it is not present
                    if (attendanceDateNoDayNoEndTime != attendanceDateNoDayEndTime)
                    {
                        Assert.IsFalse(this._generalMethods.ItemExistsInTableResponsiveWebDriver(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayEndTime, "Met on"), "End time was present.");
                    }

                    Assert.IsTrue(this._generalMethods.ItemExistsInTableResponsiveWebDriver(TableIds.InFellowship_Group_Attendance, attendanceNum + " Present", "Present"), "Attended number is wrong");
                    Assert.IsTrue(this._generalMethods.ItemExistsInTableResponsiveWebDriver(TableIds.InFellowship_Group_Attendance, "0 Absent", "Absent"), "Absent number is wrong");
                    Assert.IsTrue(this._generalMethods.ItemExistsInTableResponsiveWebDriver(TableIds.InFellowship_Group_Attendance, "100%", "Percentage"), "Percentage number is wrong");
                }
                else
                {
                    Assert.IsTrue(this._generalMethods.ItemExistsInTableWebDriver(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayNoEndTime, "Met on", "contains"), "The group met for a given attendance date but this data was not shown on the Attendance view page!");

                    // If there was an endtime, verifiy it is not present
                    if (attendanceDateNoDayNoEndTime != attendanceDateNoDayEndTime)
                    {
                        Assert.IsFalse(this._generalMethods.ItemExistsInTableWebDriver(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayEndTime, "Met on", "contains"), "End time was present.");
                    }

                    //Assert.IsTrue(this._generalMethods.ItemExistsInTableWebDriver(TableIds.InFellowship_Group_Attendance, attendanceNum.ToString(), "Attended", "contains"), "Attended number is wrong");
                    //Assert.IsTrue(this._generalMethods.ItemExistsInTableWebDriver(TableIds.InFellowship_Group_Attendance, "0", "Absent", "contains"), "Absent number is wrong");
                    //Assert.IsTrue(this._generalMethods.ItemExistsInTableWebDriver(TableIds.InFellowship_Group_Attendance, "100%", "Percentage", "contains"), "Percentage number is wrong");
                }
            }
            else
            {
                this._driver.FindElementById("group_attendance_no").Click();
                this._driver.FindElementById("group_attendance_comments").SendKeys(string.Format("The group did not meet on {0}.", scheduleDate));
                // this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
                //SP - changed this since ID value is changed in UI for Attendance Save button....
                this._driver.FindElementById(GeneralButtons.Attendance_submitQuery).Click();

                // Verify the attendance was posted for this date under the "Did not meet" table.
                Assert.IsTrue(this._generalMethods.ItemExistsInTableWebDriver(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayNoEndTime, "Met on", "contains"), "The group did not meet for a given attendance date but this data was not shown on the Attendance view page!");

                // If there was an endtime, verifiy it is not present
                if (attendanceDateNoDayNoEndTime != attendanceDateNoDayEndTime)
                {
                    Assert.IsFalse(this._generalMethods.ItemExistsInTableWebDriver(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayEndTime, "Met on", "contains"), "End time was present.");
                }
            }
        }

        /// <summary>
        /// Posts attendances matches given name list for a group for a given date.
        /// <param name="groupName">The name of the group.</param>
        /// <param name="scheduleDate">The schedule date the group met that you are posting attendance for. Not specifying a date will select the first date in the drop down.</param>
        /// <param name="individualName">The list of individual name, should contain both first name and last name
        /// <param name="didGroupMeet">Did the group meet?</param>
        public void Groups_Attendance_Enter_Given_WebDriver(string groupName, string scheduleDate, List<string> individualName, bool didGroupMeet)
        {
            // View the group's attendance page
            this.Groups_Attendance_View_WebDriver(groupName);

            // Enter attendance
            this._driver.FindElementByLinkText("Enter attendance").Click();

            // Variables
            var attendanceDate = scheduleDate;
            var attendanceDateNoDayEndTime = string.Empty;
            var attendanceDateNoDayNoEndTime = string.Empty;
            var attendanceDate1 = string.Empty;

            //Added thsi repalcement sicne the UI in infellwoship as "to" instead of "-"
            // If no date is specified, use the first date in the drop down
            if (string.IsNullOrEmpty(scheduleDate))
            {
                // Store the first date in the drop down
                attendanceDate1 = this._driver.FindElementByXPath("//select[@id='post_attendance']//optgroup[@label='Choose an event']/option[1]").Text;
            }
            else
            {
                attendanceDate1 = Regex.Replace(attendanceDate, "-", "to");
                TestLog.WriteLine("Attendance Date : {0}", attendanceDate1);

            }

            // Select the date
            new SelectElement(this._driver.FindElementById("post_attendance")).SelectByText(attendanceDate1);
            this._generalMethods.WaitForElement(By.Id("attendance_roster"));


            // Regex the date for various purposes
            attendanceDateNoDayEndTime = Regex.Replace(attendanceDate1, "(Satur|Sun|Mon|Tues|Wednes|Thurs|Fri){1}(day, )", string.Empty);
            attendanceDateNoDayNoEndTime = Regex.Replace(attendanceDateNoDayEndTime, "(\\s? - \\s?[0-9]?[0-9]:[0-9][0-9]\\s[A|P]M)", string.Empty);

            // If the group met, check all and submit.  If it didn't meet, click no and hit submit
            if (didGroupMeet)
            {
                this._driver.FindElementById("group_attendance_yes").Click();

                // Select individuals based on mobile view or not
                var windowSize = this._driver.Manage().Window.Size;
                IWebElement table = this._driver.FindElementByXPath(TableIds.InFellowship_Group_Attendance_Roster_Responsive);
                int rowsTbl = table.FindElements(By.TagName("tr")).Count;
                int attendanceNum = 0;
                if (windowSize.Width == 640)
                {
                    for (int r = 1; r < rowsTbl; r++)
                    {
                        if (individualName.Contains(table.FindElements(By.TagName("tr"))[r].FindElements(By.TagName("td"))[2].Text))
                        {
                            table.FindElements(By.TagName("tr"))[r].FindElements(By.TagName("td"))[2].Click();
                            attendanceNum++;
                        }
                    }
                }
                else
                {
                    for (int r = 1; r < rowsTbl; r++)
                    {
                        if (individualName.Contains(Regex.Split(table.FindElements(By.TagName("tr"))[r].FindElements(By.TagName("td"))[2].Text, "\r\n")[0]))
                        {
                            this._driver.FindElementsByXPath("//input[@type='checkbox']")[r].Click();
                            attendanceNum++;
                        }
                    }
                }
                //Screen shot to see if they all got selected
                this._generalMethods.TakeScreenShot_WebDriver();

                // this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
                //SP - changed this since ID value is changed in UI for Attendance Save button....
                this._driver.FindElementById(GeneralButtons.Attendance_submitQuery).Click();


                // Verify the attendance was posted
                // Changed how we verify things
                // Method uses an xpath that is not suitable since we now have a link
                // i.e. //table[@class='grid']/tbody/tr[*]/td[position()=1 and (normalize-space(string(.))='October 29, 2014 at 7:31 PM' or normalize-space(string(./*))='October 29, 2014 at 7:31 PM')] will return October Oct 29 , 2014 at 19:31:00 October 29, 2014 at 7:31 PM
                // i.e. //table[@class='grid']/tbody/tr[*]/td[position()=1 and (normalize-space(string(.))='October 29, 2014 at 7:31 PM' or normalize-space(string(./*))='October 29, 2014 at 7:31 PM')]/a will return October 29, 2014 at 7:31 PM
                if (windowSize.Width == 640)
                {
                    Assert.IsTrue(this._generalMethods.ItemExistsInTableResponsiveWebDriver(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayNoEndTime, "Met on"), "The group met for a given attendance date but this data was not shown on the Attendance view page!");

                    // If there was an endtime, verifiy it is not present
                    if (attendanceDateNoDayNoEndTime != attendanceDateNoDayEndTime)
                    {
                        Assert.IsFalse(this._generalMethods.ItemExistsInTableResponsiveWebDriver(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayEndTime, "Met on"), "End time was present.");
                    }

                    Assert.IsTrue(this._generalMethods.ItemExistsInTableResponsiveWebDriver(TableIds.InFellowship_Group_Attendance, attendanceNum + " Present", "Present"), "Attended number is wrong");
                    Assert.IsTrue(this._generalMethods.ItemExistsInTableResponsiveWebDriver(TableIds.InFellowship_Group_Attendance, (rowsTbl-1-attendanceNum) + " Absent", "Absent"), "Absent number is wrong");
                    Assert.IsTrue(this._generalMethods.ItemExistsInTableResponsiveWebDriver(TableIds.InFellowship_Group_Attendance, Math.Round(attendanceNum * 1.0 / (rowsTbl - 1), 2).ToString("0%"), "Percentage"), "Percentage number is wrong");
                }
                else
                {
                    Assert.IsTrue(this._generalMethods.ItemExistsInTableWebDriver(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayNoEndTime, "Met on", "contains"), "The group met for a given attendance date but this data was not shown on the Attendance view page!");

                    // If there was an endtime, verifiy it is not present
                    if (attendanceDateNoDayNoEndTime != attendanceDateNoDayEndTime)
                    {
                        Assert.IsFalse(this._generalMethods.ItemExistsInTableWebDriver(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayEndTime, "Met on", "contains"), "End time was present.");
                    }

                    //Assert.IsTrue(this._generalMethods.ItemExistsInTableWebDriver(TableIds.InFellowship_Group_Attendance, attendanceNum.ToString(), "Attended", "contains"), "Attended number is wrong");
                    //Assert.IsTrue(this._generalMethods.ItemExistsInTableWebDriver(TableIds.InFellowship_Group_Attendance, rowsTbl-1-attendanceNum, "Absent", "contains"), "Absent number is wrong");
                    //Assert.IsTrue(this._generalMethods.ItemExistsInTableWebDriver(TableIds.InFellowship_Group_Attendance, Math.Round(attendanceNum * 1.0 / (rowsTbl - 1), 2).ToString("0%"), "Percentage", "contains"), "Percentage number is wrong");
                }
            }
            else
            {
                this._driver.FindElementById("group_attendance_no").Click();
                this._driver.FindElementById("group_attendance_comments").SendKeys(string.Format("The group did not meet on {0}.", scheduleDate));
                // this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
                //SP - changed this since ID value is changed in UI for Attendance Save button....
                this._driver.FindElementById(GeneralButtons.Attendance_submitQuery).Click();

                // Verify the attendance was posted for this date under the "Did not meet" table.
                Assert.IsTrue(this._generalMethods.ItemExistsInTableWebDriver(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayNoEndTime, "Met on", "contains"), "The group did not meet for a given attendance date but this data was not shown on the Attendance view page!");

                // If there was an endtime, verifiy it is not present
                if (attendanceDateNoDayNoEndTime != attendanceDateNoDayEndTime)
                {
                    Assert.IsFalse(this._generalMethods.ItemExistsInTableWebDriver(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayEndTime, "Met on", "contains"), "End time was present.");
                }
            }
        }
        

        /// <summary>
        /// Updates a given schedule date's attendance.
        /// </summary>
        /// <param name="test"></param>
        /// /// <param name="groupName">The name of the group.</param>
        /// <param name="attendanceDate">The date of the schedule.</param>
        /// <param name="didGroupMeet">Did the group meet?</param>
        public void Groups_Attendance_Update(string groupName, string attendanceDate, bool didGroupMeet) {
            this.Groups_Attendance_View_Date(groupName, attendanceDate);
            this._selenium.ClickAndWaitForPageToLoad("link=Edit attendance");

            // Store various forms of the date
            var attendanceDateNoDayEndTime = Regex.Replace(attendanceDate, "(Satur|Sun|Mon|Tues|Wednes|Thurs|Fri){1}(day, )", string.Empty);
            var attendanceDateNoDayNoEndTime = Regex.Replace(attendanceDateNoDayEndTime, "(\\s? - \\s?[0-9]?[0-9]:[0-9][0-9]\\s[A|P]M)", string.Empty);

            // If the group did meet, click yes and check all.  If it didn't meet, click no
            if (didGroupMeet) {
                this._selenium.Click("group_attendance_yes");
                this._selenium.Click("//input[@type='checkbox']");
               // this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
                //SP - changed this since ID value is changed in UI for Attendance Save button....
                this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.Attendance_submitQuery);

                // Verify the attendance was posted
                Assert.IsTrue(this._generalMethods.ItemExistsInTable(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayNoEndTime, "Met on", "contains"), "The group met for a given attendance date but this data was not shown on the Attendance view page!");

                // If there was an endtime, verifiy it is not present
                if (attendanceDateNoDayNoEndTime != attendanceDateNoDayEndTime) {
                    Assert.IsFalse(this._generalMethods.ItemExistsInTable(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayEndTime, "Met on", null), "End time was present.");
                }

            }
            else {
                this._selenium.Click("group_attendance_no");
                this._selenium.Type("group_attendance_comments", string.Format("The group did not meet on {0}.", attendanceDate));
               // this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
                //SP - changed this since ID value is changed in UI for Attendance Save button....
                this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.Attendance_submitQuery);

                // Verify the attendance was posted for this date under the "Did not meet" table.
                string dnmText = this._selenium.GetText(string.Format("{0}/tbody/tr[*]/td[2]", TableIds.InFellowship_Group_Attendance));
                Assert.AreEqual("DID NOT MEET", dnmText);
                //Assert.IsTrue(this._generalMethods.ItemExistsInTable(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayNoEndTime, "Did not meet", null), "The group did not meet for a given attendance date but this data was not shown on the Attendance view page!");
                
                // If there was an endtime, verifiy it is not present
                if (attendanceDateNoDayNoEndTime != attendanceDateNoDayEndTime) {
                    Assert.IsFalse(this._generalMethods.ItemExistsInTable(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayEndTime, "Did not meet", null), "End time was present.");
                }
            }
        }

        /// <summary>
        /// Verifies the percentage for the given day the group met.
        /// </summary>
        /// <param name="attendanceDate">The day the group met.</param>
        public void Groups_Attendance_VerifyPercentage(string attendanceDate) {
            // Store the first attendance date.  Regex out the end time and day.
            var attendanceDateNoDayEndTime = Regex.Replace(attendanceDate, "(Satur|Sun|Mon|Tues|Wednes|Thurs|Fri){1}(day, )", string.Empty);
            var attendanceDateNoDayNoEndTime = Regex.Replace(attendanceDateNoDayEndTime, "(\\s? - \\s?[0-9]?[0-9]:[0-9][0-9]\\s[A|P]M)", string.Empty);

            // Get the table row number
            var row = this._generalMethods.GetTableRowNumber(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayEndTime, "Met on", null);

            var attended = Int32.Parse(this._selenium.GetText(string.Format("//table[@class='grid']/tbody/tr[{0}]/td[3]", row + 1)));
            var didNotAttend = Int32.Parse(this._selenium.GetText(string.Format("//table[@class='grid']/tbody/tr[{0}]/td[4]", row + 1)));
            var total = attended + didNotAttend;
            var percentage = (int)(attended / total * 100);
            var stringPercent = percentage.ToString() + "%";

            // Verify the attendance was posted
            Assert.IsTrue(this._generalMethods.ItemExistsInTable(TableIds.InFellowship_Group_Attendance, attendanceDateNoDayNoEndTime, "Met on", null), "The group met for a given attendance date but this data was not shown on the Attendance view page!");


        }

        /// <summary>
        /// Enters attendance for a group, allowing you to create a new date.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="date">The date for the attendance.</param>
        /// <param name="time">The time the meeting occured.</param>
        public void Groups_Attendance_Post_Enter_All_New_Date(string groupName, string date, string time) {

            // View the attendance page
            this.Groups_Attendance_View(groupName);

            // Enter attendance
            this._selenium.ClickAndWaitForPageToLoad("link=Enter attendance");

            // Select the enter new date option
            this._selenium.SelectAndWaitForCondition("//select[@id='post_attendance']", "Add your new event...", this._javascript.IsElementPresent("pick_event_date"), "20000");
           // this._selenium.Type("pick_event_date", date);
            this._selenium.Focus("pick_event_date");
            this._selenium.Type("pick_event_date", date);
            //this._selenium.KeyPress("pick_event_date", "\\9");
            this._selenium.Focus("pick_event_time");
            this._selenium.Type("pick_event_time", time);

            // Wait for the table
            this._selenium.WaitForCondition(this._javascript.IsElementPresentSelector("table.grid"), "10000");

            System.Threading.Thread.Sleep(5000);
            this._selenium.Click("//input[@type='checkbox']");
            //this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
            //SP - changed this since ID value is changed in UI for Attendance Save button....
            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.Attendance_submitQuery);

            // Verify the new date exists.
            var attendanceDateFormal = string.Format("{0} {1}, {2} at {3}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MMMM"), 
                TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("dd"), DateTime.Now.Year, time);
            string yourMom = this._selenium.GetText(string.Format("{0}/tbody/tr[*]/td/a[@class='hidden-xs']", TableIds.InFellowship_Group_Attendance));
            //Assert.IsTrue(this._generalMethods.ItemExistsInTable(TableIds.InFellowship_Group_Attendance, attendanceDateFormal, "Met on", null));
            Assert.AreEqual(attendanceDateFormal, yourMom);
            
        }

        #endregion Group Attendance

        #region Group Management
        /// <summary>
        /// Views a group.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_View(string groupName) {
            // We are attempting to view a group. There is some logic to figure out where you are when this method is called and how we are going to view the group.
            // Are you on the landing page?
            if (this._selenium.IsElementPresent(GeneralInFellowshipConstants.LandingPage.Link_YourGroups)) {
                // We were. Click "Your Groups" and move on.
                this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourGroups);
                this._selenium.ClickAndWaitForPageToLoad("link=" + groupName);
            }
            // Is the link with the group's name present?
            else if (this._selenium.IsElementPresent("link=" + groupName)) {
                // It is.  Are we viewing that group?
                if (this._selenium.GetText("//div[@id='brand']/a[1]") != groupName)
                    // We aren't.  We are at the Group's dashboard.  Click the group name.
                    this._selenium.ClickAndWaitForPageToLoad("link=" + groupName);
            }
            else {
                // We couldn't figure out where we were. Just click home, your groups, and move on.
                this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
                this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourGroups);
                this._selenium.ClickAndWaitForPageToLoad("link=" + groupName);
            }
        }
       
        /// <summary>
        /// Views a group.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_View_WebDriver(string groupName)
        {
            // We are attempting to view a group. There is some logic to figure out where you are when this method is called and how we are going to view the group.
            // Are you on the landing page?
            if (this._generalMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")))
            {
                // We were. Click "Your Groups" and move on.
                this._driver.FindElementByLinkText("Your Groups").Click();
                //this._generalMethods.WaitForElement(this._driver, By.LinkText(groupName));
                //this._driver.FindElementByLinkText(groupName).Click();
                this._generalMethods.WaitAndGetElement(By.LinkText(groupName)).Click();
                //this._generalMethods.WaitForElement(this._driver, By.LinkText("View roster"));
            }
            // Is the link with the group's name present?
            else if (this._generalMethods.IsElementPresentWebDriver(By.LinkText(groupName)))
            {
                // It is.  Are we viewing that group?
                //if (this._driver.FindElementByXPath("//div[@id='brand']/a[1]").Text != groupName)
                    // We aren't.  We are at the Group's dashboard.  Click the group name.
                    this._driver.FindElementByLinkText("Dashboard").Click();
                    this._generalMethods.WaitForElement(this._driver, By.LinkText("View roster"));
            }
            else
            {
                // We couldn't figure out where we were. Just click home, your groups, and move on.
                this._driver.FindElementByLinkText("Home").Click();
                this._driver.FindElementByLinkText("Your Groups").Click();
                this._generalMethods.WaitForElement(this._driver, By.LinkText(groupName));
                this._driver.FindElementByLinkText(groupName).Click();
                this._generalMethods.WaitForElement(this._driver, By.LinkText("View Roster"));
            }
        }

        /// <summary>
        /// Views a group under a span of care you own.
        /// </summary>
        /// <param name="socName">The name of the Span of Care.</param>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_View(string socName, string groupName) {
            // We are attempting to view a group under a span of care. There is some logic to figure out where you are when this method is called and how we are going to view the group.
            // Are you on the landing page?
            if (this._selenium.IsElementPresent(GeneralInFellowshipConstants.LandingPage.Link_YourGroups)) {
                // We were. Click "Your Groups" and move on.
                this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourGroups);
                this._selenium.ClickAndWaitForPageToLoad("link=" + socName);
                this._selenium.ClickAndWaitForPageToLoad("link=" + groupName);
            }
            // Is the link with the group's name present?
            else if (this._selenium.IsElementPresent("link=" + groupName)) {
                // It is.  Are we viewing that group?
                if (this._selenium.GetText("//div[@id='brand']/a[1]") != groupName)
                    // We aren't.  We are at the Group's dashboard.  Click the group name
                    this._selenium.ClickAndWaitForPageToLoad("link=" + socName);
                this._selenium.ClickAndWaitForPageToLoad("link=" + groupName);
            }
            else {
                // We couldn't figure out where we were. Just click home, your groups, and move on.
                this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
                this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourGroups);
                this._selenium.ClickAndWaitForPageToLoad("link=" + socName);
                this._selenium.ClickAndWaitForPageToLoad("link=" + groupName);
            }
        }

        /// <summary>
        /// Views a group under a span of care you own.
        /// </summary>
        /// <param name="socName">The name of the Span of Care.</param>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_View_WebDriver(string socName, string groupName)
        {
            // We are attempting to view a group. There is some logic to figure out where you are when this method is called and how we are going to view the group.
            // Are you on the landing page?
            if (this._generalMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")))
            {
                // We were. Click "Your Groups" and move on.
                this._driver.FindElementByLinkText("Your Groups").Click();
                this._generalMethods.WaitForElement(this._driver, By.LinkText(socName));
                this._driver.FindElementByLinkText(socName).Click();
                this._generalMethods.WaitForElement(this._driver, By.LinkText(groupName));
                this._driver.FindElementByLinkText(groupName).Click();
                this._generalMethods.WaitForElement(this._driver, By.LinkText("View roster"));
            }
            // Is the link with the group's name present?
            else if (this._generalMethods.IsElementPresentWebDriver(By.LinkText(groupName)))
            {
                // It is.  Are we viewing that group?
                if (this._driver.FindElementByXPath("//div[@id='brand']/a[1]").Text != groupName)
                    // We aren't.  We are at the Group's dashboard.  Click the group name. 
                    //modify by grace zhang
                    //this._driver.FindElementByLinkText(socName).Click();
                    this._driver.FindElementByLinkText(groupName).Click();
                    //this._generalMethods.WaitForElement(this._driver, By.LinkText(socName));
                this._driver.FindElementByLinkText(groupName).Click();
                this._generalMethods.WaitForElement(this._driver, By.LinkText("View roster"));
            }
            else
            {
                // We couldn't figure out where we were. Just click home, your groups, and move on.
                this._driver.FindElementByLinkText("Your Groups").Click();
                this._generalMethods.WaitForElement(this._driver, By.LinkText(socName));
                this._driver.FindElementByLinkText(socName).Click();
                this._generalMethods.WaitForElement(this._driver, By.LinkText(groupName));
                this._driver.FindElementByLinkText(groupName).Click();
                this._generalMethods.WaitForElement(this._driver, By.LinkText("View roster"));
            }
        }


        /// <summary>
        /// Views the dashboard of a group.
        /// </summary>
        /// <param name="selenium"></param>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_View_Dashboard(string groupName) {
            this.Groups_Group_View(groupName);
            this._selenium.ClickAndWaitForPageToLoad("link=Dashboard");
        }

        /// <summary>
        /// Views the dashboard of a group under your span of care
        /// </summary>>
        /// <param name="socName">The name of the span of care.</param>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_View_Dashboard(string socName, string groupName) {
            if (this._selenium.IsElementPresent("link=Dashboard"))
                this._selenium.ClickAndWaitForPageToLoad("link=Dashboard");
            else {
                this.Groups_Group_View(socName, groupName);
                this._selenium.ClickAndWaitForPageToLoad("link=Dashboard");
            }
        }

        /// <summary>
        /// Views the dashboard of a group.
        /// </summary>
        /// <param name="selenium"></param>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_View_Dashboard_WebDriver(string groupName)
        {
            this.Groups_Group_View_WebDriver(groupName);
            this._driver.FindElementByLinkText("Dashboard").Click();
            this._generalMethods.WaitForElement(this._driver, By.LinkText("View roster"));
        }

        /// <summary>
        /// Views the dashboard of a group under your span of care
        /// </summary>>
        /// <param name="socName">The name of the span of care.</param>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_View_Dashboard_WebDriver(string socName, string groupName)
        {
            if (this._generalMethods.IsElementPresentWebDriver(By.LinkText("Dashboard")))
            {
                this._driver.FindElementByLinkText("Dashboard").Click();
                this._generalMethods.WaitForElement(this._driver, By.LinkText("View roster"));
            }

            else
            {
                this.Groups_Group_View_WebDriver(socName, groupName);
                this._driver.FindElementByLinkText("Dashboard").Click();
                this._generalMethods.WaitForElement(this._driver, By.LinkText("View roster"));
            }
        }

        /// <summary>
        /// Views the roster of a group.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_View_Roster(string groupName) {
            if (this._selenium.IsElementPresent("link=Roster"))
                this._selenium.ClickAndWaitForPageToLoad("link=Roster");
            else {
                try
                {
                    this.Groups_Group_View(groupName);
                }
                catch (System.Exception e)
                {
                    //If have been logged out, let's try one more time
                    if (this._selenium.IsElementPresent("//div[@id='username']"))
                    {
                        this.ReLoginInFellowship(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);
                        this.Groups_Group_View(groupName);
                    }
                    else
                    {
                        throw new System.Exception(e.Message, e);
                    }
                }
                
                this._selenium.ClickAndWaitForPageToLoad("link=Roster");
            }
        }

        /// <summary>
        /// Views the roster of a group.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_View_Roster_WebDriver(string groupName)
        {
            if (this._generalMethods.IsElementPresentWebDriver(By.LinkText("Roster")))
            {
                this._driver.FindElementByLinkText("Roster").Click();
                this._generalMethods.WaitForElement(this._driver, By.LinkText("View prospects"));
            }
            else
            {
                this.Groups_Group_View_WebDriver(groupName);
                this._generalMethods.IsElementPresentWebDriver(By.LinkText(groupName));
                this._driver.FindElementByLinkText("Roster").Click();
               // this._generalMethods.WaitForElement(this._driver, By.LinkText("View prospects"));
            }
        }

        /// <summary>
        /// Views the roster of a group under your span of care.
        /// </summary>
        /// <param name="socName">The name of the span of care.</param>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_View_Roster(string socName, string groupName) {
            if (this._selenium.IsElementPresent("link=Roster"))
                this._selenium.ClickAndWaitForPageToLoad("link=Roster");
            else {
                this.Groups_Group_View(socName, groupName);
                this._selenium.ClickAndWaitForPageToLoad("link=Roster");
            }
        }

        /// <summary>
        /// Views the roster of a group under your span of care.
        /// </summary>
        /// <param name="socName">The name of the span of care.</param>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_View_Roster_WebDriver(string socName, string groupName)
        {
            if (this._generalMethods.IsElementPresentWebDriver(By.LinkText("Roster")))
                this._driver.FindElementByLinkText("Roster").Click();
            else
            {
                this.Groups_Group_View_WebDriver(socName, groupName);
                this._driver.FindElementByLinkText("Roster").Click();
            }
        }

        /// <summary>
        /// Views the Export to CSV page.
        /// <param name="groupName">The name of the group.</param>
        /// </summary>
        public void Groups_Group_View_Roster_ExportToCSV(string groupName) {
            // Views the roster
            this.Groups_Group_View_Roster(groupName);

            // Selects the Export to CSV link
            this._selenium.ClickAndWaitForPageToLoad("link=Download CSV");
        }

        /// <summary>
        /// Views the settings of a group.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_View_Settings(string groupName) {
            this.Groups_Group_View_Dashboard(groupName);
            Assert.IsTrue(this._selenium.IsElementPresent("link=" + groupName), "Group name is not display correctly in group detail page");
            this._selenium.ClickAndWaitForPageToLoad("link=View settings");
        }

        /// <summary>
        /// Views the settings of a group under your Span of Care.
        /// </summary>
        /// <param name="socName">The name of the span of care.</param>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_View_Settings(string socName, string groupName) {
            this.Groups_Group_View_Dashboard(socName, groupName);
            this._selenium.ClickAndWaitForPageToLoad("link=View settings");
        }

        /// <summary>
        /// Views the settings of a group.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_View_Settings_WebDriver(string groupName)
        {
            this.Groups_Group_View_Dashboard_WebDriver(groupName);
            this._driver.FindElementByLinkText("View settings").Click();
            this._generalMethods.WaitForElement(this._driver, By.LinkText("Edit details"));
        }

        /// <summary>
        /// Views the settings of a group under your Span of Care.
        /// </summary>
        /// <param name="socName">The name of the span of care.</param>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_View_Settings_WebDriver(string socName, string groupName)
        {
            this.Groups_Group_View_Dashboard_WebDriver(socName, groupName);
            this._driver.FindElementByLinkText("View settings").Click();
            this._generalMethods.WaitForElement(this._driver, By.LinkText("Edit details"));
        }

        /// <summary>
        /// Views the prospects of a group.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_View_Prospects(string groupName) {
            // View a group.
            this.Groups_Group_View(groupName);

            // Click the prospect's link
            this._selenium.ClickAndWaitForPageToLoad("css=li#tab_group_prospects a");
        }

        public void Groups_Group_View_Prospects_WebDriver(string groupName)
        {
            // View a group.
            try
            {
                this.Groups_Group_View_WebDriver(groupName);
            }
            catch (System.Exception e)
            {
                //If have been logged out, let's try one more time
                if (this._generalMethods.IsElementPresentWebDriver(By.Id("username")))
                {
                    this.ReLoginWebDriver(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);
                    this.Groups_Group_View_WebDriver(groupName);
                }
                else
                {
                    throw new System.Exception(e.Message, e);
                }
            }

            // Click the prospect's link
            //this._selenium.ClickAndWaitForPageToLoad("css=li#tab_group_prospects a");
            this._driver.FindElementByPartialLinkText("Prospects").Click();
        }

        /// <summary>
        /// Views the prospects of a group under your Span of Care.
        /// </summary>
        /// <param name="socName">The name of the span of care.</param>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_View_Prospects(string socName, string groupName) {
            // View a group
            this.Groups_Group_View(socName, groupName);

            // Click the prospect's link
            this._selenium.ClickAndWaitForPageToLoad("css=li#tab_group_prospects a");
        }

        /// <summary>
        /// Views the prospects of a group under your Span of Care.
        /// </summary>
        /// <param name="socName">The name of the span of care.</param>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_View_Prospects_WebDriver(string socName, string groupName)
        {
            // View a group
            this.Groups_Group_View_WebDriver(socName, groupName);

            // Click the prospect's link
            //this._selenium.ClickAndWaitForPageToLoad("css=li#tab_group_prospects a");
            this._driver.FindElementByPartialLinkText("Prospects").Click();
        }

        public void Groups_Group_View_Individual(string groupName, string individual) {
            // Parse the individual for the first name and last name. Build the link
            string[] name = individual.Split(' ');
            string groupMemberLink = string.Format("//a[contains(@href, '/Members/') and span[1]/text()='{0}' and span[2]/text()='{1}']", name[0], name[1]);

            // View the roster of a group
            this.Groups_Group_View_Roster(groupName);

            // View the member of the group
            this._selenium.ClickAndWaitForPageToLoad(groupMemberLink);
        }

        public void Groups_Group_View_Individual_WebDriver(string groupName, string individual)
        {
            // Parse the individual for the first name and last name. Build the link
            //string[] name = individual.Split(' ');
            //string groupMemberLink = string.Format("//a[contains(@href, '/Members/') and span[1]/text()='{0}' and span[2]/text()='{1}']", name[0], name[1]);

            // View the roster of a group
            this.Groups_Group_View_Roster_WebDriver(groupName);

            // View the member of the group
            this._driver.FindElementByLinkText(individual).Click();
            this._generalMethods.WaitForElement(this._driver, By.LinkText("Remove from group"));
        }

        /// <summary>
        /// Creates a schedule that is a one time event for a group.
        /// </summary>
        /// <param name="startDate">The start date for the schedule.</param>
        /// <param name="startTime">The start time for the schedule.</param>
        /// <param name="endTime">The end time for the schedule.  This is optional.</param>
        /// <param name="notify">Notify the group about the schedule change.</param>
        public void Groups_Group_Create_Schedule_OneTimeEvent(string groupName, string startDate, string startTime, string endTime, bool notify) {
            // View the group
            this.Groups_Group_View_Settings(groupName);

            // Create a schedule
            this._selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/GroupSchedule/')]");
            this._selenium.Type("start_date", startDate);
            this._selenium.Type("start_time", startTime);

            if (!string.IsNullOrEmpty(endTime)) {
                this._selenium.Click("//input[@class='end_toggle']");
                this._selenium.Type("end_time", endTime);
            }

            // Unless you want to notify the group, uncehck the box.
            if (!notify) {
                this._selenium.Click("notify");
            }

            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify
            var date = Convert.ToDateTime(startDate);
            var time = Convert.ToDateTime(startTime);
            this._selenium.VerifyTextPresent(string.Format("Meets {0} {1}, {2} {3}", date.ToString("MMMM"), date.ToString("dd"), date.Year, time.ToShortTimeString()));
        }

        /// <summary>
        /// Creates a schedule that is a one time event for a group under your Span of Care.
        /// </summary>
        /// <param name="socName">The name of the span of care.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="startDate">The start date for a schedule.</param>
        /// <param name="startTime">The start time for the schedule.</param>
        /// <param name="endTime">The optional end time for the schedule/</param>
        /// <param name="notify">The option if the group should be notified.</param>
        public void Groups_Group_Create_Schedule_OneTimeEvent(string socName, string groupName, string startDate, string startTime, string endTime, bool notify) {
            // View the group
            this.Groups_Group_View(socName, groupName);

            // Create a one time schedule
            this.Groups_Group_Create_Schedule_OneTimeEvent(groupName, startDate, startTime, endTime, notify);
        }

        /// <summary>
        /// Creates a schedule that is a weekly event for a group.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="startDate">The start date for the schedule.</param>
        /// <param name="startTime">The start time for the schedule.</param>
        /// <param name="endTime">The optional end time for the schedule.</param>
        /// <param name="daysOfWeek">The days of the week the schedule occurs.</param>
        /// <param name="reocurrance">The reoccurance of the schedule.</param>
        /// <param name="notify">Notifies the group of the new schedule.</param>
        public void Groups_Group_Create_Schedule_Weekly(string groupName, string startDate, string startTime, string endTime, GeneralEnumerations.WeeklyScheduleDays[] daysOfWeek, string reocurrance, bool notify) {
            // View the group
            this.Groups_Group_View_Settings(groupName);

            // Create a schedule that meets weekly
            this._selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/GroupSchedule/')]");
            this._selenium.Type("start_date", startDate);
            this._selenium.Type("start_time", startTime);

            if (endTime != null) {
                this._selenium.Click("css=input.end_toggle");
                this._selenium.Type("end_time", endTime);
            }

            this._selenium.Click("recurrence_weekly");

            foreach (GeneralEnumerations.WeeklyScheduleDays weekday in daysOfWeek) {
                switch (weekday) {
                    case GeneralEnumerations.WeeklyScheduleDays.Sunday:
                        this._selenium.Click("recurrence_weekly_sunday");
                        break;
                    case GeneralEnumerations.WeeklyScheduleDays.Monday:
                        this._selenium.Click("recurrence_weekly_monday");
                        break;
                    case GeneralEnumerations.WeeklyScheduleDays.Tuesday:
                        this._selenium.Click("recurrence_weekly_tuesday");
                        break;
                    case GeneralEnumerations.WeeklyScheduleDays.Wednesday:
                        this._selenium.Click("recurrence_weekly_wednesday");
                        break;
                    case GeneralEnumerations.WeeklyScheduleDays.Thursday:
                        this._selenium.Click("recurrence_weekly_thursday");
                        break;
                    case GeneralEnumerations.WeeklyScheduleDays.Friday:
                        this._selenium.Click("recurrence_weekly_friday");
                        break;
                    case GeneralEnumerations.WeeklyScheduleDays.Saturday:
                        this._selenium.Click("recurrence_weekly_saturday");
                        break;
                }
            }

            this._selenium.Select("recurrence_every", reocurrance);

            // Unless you want to notify the group, uncehck the box.
            if (!notify) {
                this._selenium.Click("notify");
            }

            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
        }

        /// <summary>
        /// Creates a schedule that is a weekly event for a group under your span of care.
        /// </summary>
        /// <param name="socName">The name of the span of care.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="startDate">The start date for the schedule.</param>
        /// <param name="startTime">The start time for the schedule.</param>
        /// <param name="endTime">The optional end time for the schedule.</param>
        /// <param name="daysOfWeek">The days of the week the schedule occurs.</param>
        /// <param name="reocurrance">The reoccurance of the schedule.</param>
        /// <param name="notify">Notifies the group of the new schedule.</param>
        public void Groups_Group_Create_Schedule_Weekly(string socName, string groupName, string startDate, string startTime, string endTime, GeneralEnumerations.WeeklyScheduleDays[] daysOfWeek, string reocurrance, bool notify) {

            // View the group
            this.Groups_Group_View(socName, groupName);

            // Create a schedule that meets weekly
            this.Groups_Group_Create_Schedule_Weekly(groupName, startDate, startTime, endTime, daysOfWeek, reocurrance, notify);
        }

        /// <summary>
        /// Creates a physical location for a group. Allows you specify if the location is private.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="locationName">The name of the location.</param>
        /// <param name="locationDescription">The description of the location.</param>
        /// <param name="street">The street of the location.</param>
        /// <param name="city">The city of the location.</param>
        /// <param name="state">The state of the location.</param>
        /// <param name="zipCode">The zip code of the location.</param>
        /// <param name="isPrivate">Is the location private?</param>
        public void Groups_Group_Create_Location_Physical(string groupName, string locationName, string locationDescription, string street, string city, string state, string zipCode, bool isPrivate) {
            // View the group
            this.Groups_Group_View_Settings(groupName);

            // Create a location
            this._selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/GroupLocation/')]");

            // Specify the name, description, street, city, state, and zip code
            this._selenium.Type("location_name", locationName);
            this._selenium.Type("location_description", locationDescription);
            this._selenium.Type("street1", street);
            this._selenium.Type("city", city);
            this._selenium.Select("state", state);
            this._selenium.Type("postal_code", zipCode);

            // Is the location private?
            if (isPrivate) {
                this._selenium.Click("is_private");
            }

            // Store the selected state's abbreviation
            var stateAbbreviation = this._selenium.GetSelectedValue("state");

            // Submit
            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify 
            //this._selenium.VerifyTextPresent(locationName);

            if (!string.IsNullOrEmpty(locationDescription)) {
                this._selenium.VerifyTextPresent(locationDescription);
            }

            //this._selenium.VerifyTextPresent(street);
            //this._selenium.VerifyTextPresent(string.Format("{0}, {1} {2}", city, stateAbbreviation, zipCode));
        }

        /// <summary>
        /// Creates a physical location for a group under your span of care. Allows you specify if the location is private.
        /// </summary>
        /// <param name="scoName">The name of the span of care.</para
        /// <param name="groupName">The name of the group.</param>
        /// <param name="locationName">The name of the location.</param>
        /// <param name="locationDescription">The description of the location.</param>
        /// <param name="street">The street of the location.</param>
        /// <param name="city">The city of the location.</param>
        /// <param name="state">The state of the location.</param>
        /// <param name="zipCode">The zip code of the location.</param>
        /// <param name="isPrivate">Is the location private?</param>
        public void Groups_Group_Create_Location_Physical(string socName, string groupName, string locationName, string locationDescription, string street, string city, string state, string zipCode, bool isPrivate) {
            // View the group
            this.Groups_Group_View(socName, groupName);

            // Create a location
            this.Groups_Group_Create_Location_Physical(groupName, locationName, locationDescription, street, city, state, zipCode, isPrivate);
        }

        /// <summary>
        /// Creates an online location for a group.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="locationName">The name of the location.</param>
        /// <param name="locationDescription">The description of the group.</param>
        /// <param name="url">The URL for the online location.</param>
        public void Groups_Group_Create_Location_Online(string groupName, string locationName, string locationDescription, string url) {
            // View the group
            this.Groups_Group_View_Settings(groupName);

            // Create a location
            this._selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/GroupLocation/')]");

            // Specify online
            this._selenium.Click("location_online");

            // Specify the name, description, and URL
            this._selenium.Type("location_name", locationName);
            this._selenium.Type("location_description", locationDescription);
            this._selenium.Type("url", url);

            // Submit
            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify 
            this._selenium.VerifyTextPresent(locationName);

            if (!string.IsNullOrEmpty(locationDescription)) {
                this._selenium.VerifyTextPresent(locationDescription);
            }

            this._selenium.VerifyElementPresent(string.Format("link={0}", url));
        }

        /// <summary>
        /// Creates an online location for a group under your span of care.
        /// </summary>
        /// <param name="socName">The name of the span of care</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="locationName">The name of the location.</param>
        /// <param name="locationDescription">The description of the group.</param>
        /// <param name="url">The URL for the online location.</param>
        public void Groups_Group_Create_Location_Online(string socName, string groupName, string locationName, string locationDescription, string url) {
            // View the group
            this.Groups_Group_View(socName, groupName);

            // Create an online location
            this.Groups_Group_Create_Location_Online(groupName, locationName, locationDescription, url);
        }

        /// <summary>
        /// Updates the details of a group, minus custom fields and standard fields.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="updatedName">The new name of the group.</param>
        /// <param name="updatedDescription">The new description of the group.</param>
        public void Groups_Group_Update_Details(string groupName, string updatedName, string updatedDescription) {
            // View the settings of the group
            this.Groups_Group_View_Settings(groupName);

            // Edit the details
            this._selenium.ClickAndWaitForPageToLoad("link=Edit details");

            // Update the details as necessary
            if (!string.IsNullOrEmpty(updatedName)) {
                this._selenium.Type("name", updatedName);
            }

            this._selenium.Type("description", updatedDescription);

            // Submit
            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify the changes
            this._selenium.VerifyElementPresent(string.Format("link={0}", updatedName));
            this._selenium.VerifyElementNotPresent(string.Format("link={0}", groupName));

            if (!string.IsNullOrEmpty(updatedDescription)) {
                this._selenium.VerifyTextPresent(updatedDescription);
            }
        }

        /// <summary>
        /// Updates the details of a group, minus custom fields and standard fields.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="updatedName">The new name of the group.</param>
        /// <param name="updatedDescription">The new description of the group.</param>
        public void Groups_Group_Update_Details_WebDriver(string groupName, string updatedName, string updatedDescription)
        {
            // View the settings of the group
            try
            {
                this.Groups_Group_View_Settings_WebDriver(groupName);
            }
            catch (System.Exception e)
            {
                //If have been logged out, let's try one more time
                if (this._generalMethods.IsElementPresentWebDriver(By.Id("username")))
                {
                    this.ReLoginWebDriver(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);
                    this.Groups_Group_View_Settings_WebDriver(groupName);
                }
                else
                {
                    throw new System.Exception(e.Message, e);
                }
            }

            // Edit the details
            this._driver.FindElementByLinkText("Edit details").Click();
            this._generalMethods.WaitForElement(this._driver, By.LinkText("Cancel"));

            // Update the details as necessary
            if (!string.IsNullOrEmpty(updatedName))
            {
                this._driver.FindElementById("name").SendKeys(updatedName);
            }

            this._driver.FindElementById("description").SendKeys(updatedDescription);

            // Submit
            //this._driver.FindElementById(GeneralButtons.submitQuery).Click();
            var btn = this._driver.FindElementsById(GeneralButtons.submitQuery).FirstOrDefault(x => x.Displayed == true);
            btn.Click();
            this._generalMethods.WaitForElement(this._driver, By.LinkText("Edit details"));

            // Verify the changes
            this._generalMethods.VerifyElementPresentWebDriver(By.LinkText(updatedName));
            this._generalMethods.VerifyElementNotPresentWebDriver(By.LinkText(groupName));

            if (!string.IsNullOrEmpty(updatedDescription))
            {
                this._generalMethods.VerifyTextPresentWebDriver(updatedDescription);
            }
        }

        /// <summary>
        /// Updates the details of a group under your span of care, minus custom fields and standard fields.
        /// </summary>
        /// <param name="socName">The name of the span of care.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="updatedName">The new name of the group.</param>
        /// <param name="updatedDescription">The new description of the group.</param>
        public void Groups_Group_Update_Details(string socName, string groupName, string updatedName, string updatedDescription) {
            // View the settings of the group
            this.Groups_Group_View_Settings(socName, groupName);

            // Edit the details
            this.Groups_Group_Update_Details(groupName, updatedName, updatedDescription);
        }

        /// <summary>
        /// Updates the details of a group under your span of care, minus custom fields and standard fields.
        /// </summary>
        /// <param name="socName">The name of the span of care.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="updatedName">The new name of the group.</param>
        /// <param name="updatedDescription">The new description of the group.</param>
        public void Groups_Group_Update_Details_WebDriver(string socName, string groupName, string updatedName, string updatedDescription)
        {
            // View the settings of the group
            this.Groups_Group_View_Settings_WebDriver(socName, groupName);

            // Edit the details
            this.Groups_Group_Update_Details_WebDriver(groupName, updatedName, updatedDescription);
        }

        /// <summary>
        /// Updates the bulletin board for a group.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="updatedText">The message to post.</param>
        /// <param name="leaderName">The name of the person updating the bulletin board.</param>
        /// <param name="publish">Determines if the message is posted to the dashboard page.</param>
        public void Groups_Group_Update_BulletinBoard(string groupName, string updatedText, string leaderName, bool publish) {
            this.Groups_Group_View_Settings(groupName);

            // Update bulletin board
            this._selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Group/BulletinBoard/')]");

            // Type the values
            this.Groups_UpdateBulletinBoard(updatedText, leaderName, publish);

        }
        //overload this above method to support timezone by churchcode,add by grace zhang
        public void Groups_Group_Update_BulletinBoard(string groupName, string updatedText, string leaderName, bool publish,String churchcode)
        {
            this.Groups_Group_View_Settings(groupName);

            // Update bulletin board
            this._selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Group/BulletinBoard/')]");

            // Type the values
            this.Groups_UpdateBulletinBoard(updatedText, leaderName, publish, churchcode);

        }
        //Gracezhang add for FO-3435
        public void Groups_Group_Update_BulletinBoard_UsingMarkdown(string groupName, string updatedText, String expectUpdatedText, string leaderName, bool publish)
        {
            this.Groups_Group_View_Settings(groupName);

            // Update bulletin board
            this._selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Group/BulletinBoard/')]");

            // Type the values
            this.Groups_UpdateBulletinBoard_UsingMarkdown(updatedText, expectUpdatedText, leaderName, publish);

        }

        /// <summary>
        /// Updates the bulletin board for a group.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="updatedText">The message to post.</param>
        /// <param name="leaderName">The name of the person updating the bulletin board.</param>
        /// <param name="publish">Determines if the message is posted to the dashboard page.</param>
        public void Groups_Group_Update_BulletinBoard_WebDriver(string groupName, string updatedText, string leaderName, bool publish)
        {
            this.Groups_Group_View_Settings_WebDriver(groupName);

            // Update bulletin board
            this._driver.FindElementByXPath("//div[contains(@class,'hidden-xs')]//a[contains(@href,'/Group/BulletinBoard/')]").Click();
            this._generalMethods.WaitForElement(this._driver, By.LinkText("Cancel"));

            // Type the values
            this.Groups_UpdateBulletinBoard_WebDriver(updatedText, leaderName, publish);

        }

        /// <summary>
        /// Updates the bulletin board for a group under your Span of Care
        /// </summary>
        /// <param name="socOwner">The name of the Span of Care you own.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="updatedText">The message to post.</param>
        /// <param name="ownerName">The name of the person updating the bulletin board.</param>
        /// <param name="publish">Determines if the message is posted to the dashboard page.</param>
        public void Groups_Group_Update_BulletinBoard(string socName, string groupName, string updatedText, string ownerName, bool publish) {
            this.Groups_Group_View(socName, groupName);

            // Update the bulletin board
            this.Groups_Group_Update_BulletinBoard(groupName, updatedText, ownerName, publish);
        }
        //overload this above method to support timezone by churchcode,add by grace zhang
        public void Groups_Group_Update_BulletinBoard(string socName, string groupName, string updatedText, string ownerName, bool publish, String churchcode)
        {
            this.Groups_Group_View(socName, groupName);

            // Update the bulletin board
            this.Groups_Group_Update_BulletinBoard(groupName, updatedText, ownerName, publish, churchcode);
        }

        /// <summary>
        /// Updates the bulletin board for a group via the Wrench on the dashboard.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="updatedText">The message to post.</param>
        /// <param name="leaderName">The name of the person updating the bulletin board.</param>
        /// <param name="publish">Determines if the message is posted to the dashboard page.</param>
        public void Groups_Group_Update_BulletinBoard_Wrench(string groupName, string updatedText, string leaderName, bool publish) {
            // View the settings
            this.Groups_Group_View_Settings(groupName);

            // Update bulletin board via the wrench
            this._selenium.ClickAndWaitForPageToLoad("//a[text()='update' and contains(@href, '/Group/BulletinBoard/')]");

            // Type the values
            this.Groups_UpdateBulletinBoard(updatedText, leaderName, publish);
        }

        /// <summary>
        /// Updates the bulletin board for a group that is under your span of care via the Wrench on the dashboard.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="updatedText">The message to post.</param>
        /// <param name="ownerName">The name of the person updating the bulletin board.</param>
        /// <param name="publish">Determines if the message is posted to the dashboard page.</param>
        public void Groups_Group_Update_BulletinBoard_Wrench(string socName, string groupName, string updatedText, string ownerName, bool publish) {
            this.Groups_Group_View_Dashboard(socName, groupName);

            // Update the bulletin board
            this.Groups_Group_Update_BulletinBoard_Wrench(groupName, updatedText, ownerName, publish);
        }

        /// <summary>
        ///  Views the Send Email page for a group under your Span of Care.
        /// </summary>
        /// <param name="socName">The name of the span of care.</param>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_View_SendEmail(string socName, string groupName) {
            this.Groups_Group_View_Roster(socName, groupName);

            // Send an email
            this._selenium.ClickAndWaitForPageToLoad("link=Send an email");
        }

        /// <summary>
        ///  Views the Send Email page for a group under your Span of Care.
        /// </summary> 
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_View_SendEmail(string groupName) {
            this.Groups_Group_View_Roster(groupName);

            // Send an email
            this._selenium.ClickAndWaitForPageToLoad("link=Send an email");
        }

        /// <summary>
        /// Sends an email to everyone in the group.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        public void Groups_Group_SendEmail_All(string groupName, string subject, string message) {
            this.Groups_Group_View_SendEmail(groupName);

            // Type the subject and message
            this._selenium.Type("subject", subject);
            this._selenium.Type("email_body", message);

            // Submit
            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Unless there was validation, verify you are on the send email page
            if (!this._selenium.IsElementPresent("//div[@class='error_msgs_for']")) {
                this._selenium.VerifyElementPresent("all_recipients");
                this._selenium.VerifyElementPresent("choose_recipients");
                this._selenium.VerifyElementPresent("subject");
                this._selenium.VerifyElementPresent("email_body");
                this._selenium.VerifyElementPresent("link=Attach a file…");

                // Verify the screen message
                this._selenium.VerifyTextPresent("Email sent!");
            }
            else {
                this._selenium.VerifyTextPresent("Subject is required and cannot exceed 200 characters.");
            }

        }

        /// <summary>
        /// Sends an email to everyone in a group.
        /// </summary>
        /// <param name="socName">The span of care the group belongs to.</param>
        /// <param name="groupName">The group name.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="message">The message in the email.</param>
        public void Groups_Group_SendEmail_All(string socName, string groupName, string subject, string message) {
            this.Groups_Group_View(socName, groupName);

            this.Groups_Group_SendEmail_All(groupName, subject, message);
        }

        /// <summary>
        /// Sends an email to specific recipients
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="message">The message of the email.</param>
        /// <param name="recipientNames">A list of the recipients to send to.</param>
        public void Groups_Group_SendEmail(string groupName, string subject, string message, List<string> recipientNames) {
            this.Groups_Group_View_SendEmail(groupName);

            // Select the recipients
            this._selenium.Click("choose_recipients");

            // Select each recipient
            foreach (var recipient in recipientNames) {
                this._selenium.Check(string.Format("//input[@name='send_to' and normalize-space(ancestor::td/following-sibling::td/label/text())='{0}']", recipient));
            }

            // Type the subject and message
            this._selenium.Type("subject", subject);
            this._selenium.Type("email_body", message);

            // Submit
            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Unless there was validation, verify you are on the send email page
            if (!this._selenium.IsElementPresent("//div[@class='error_msgs_for']")) {
                this._selenium.VerifyElementPresent("all_recipients");
                this._selenium.VerifyElementPresent("choose_recipients");
                this._selenium.VerifyElementPresent("subject");
                this._selenium.VerifyElementPresent("email_body");
                this._selenium.VerifyElementPresent("link=Attach a file…");

                // Verify the screen message
                this._selenium.VerifyTextPresent("Email sent!");
            }
            else {
                this._selenium.VerifyTextPresent("Subject is required and cannot exceed 200 characters.");
            }

        }

        /// <summary>
        /// Sends an email to specific recipients
        /// </summary>
        /// <param name="socName">The name of the span of care the group belongs to.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="message">The message of the email.</param>
        /// <param name="recipientNames">A list of the recipients to send to.</param>
        public void Groups_Group_SendEmail(string socName, string groupName, string subject, string message, List<string> recipientNames) {
            this.Groups_Group_View(socName, groupName);

            // Send an email
            this.Groups_Group_SendEmail(groupName, subject, message, recipientNames);
        }

        /// <summary>
        /// Deletes a schedule for a group.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_Delete_Schedule(string groupName, bool notify) {
            this.Groups_Group_View_Settings(groupName);

            this._selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/GroupSchedule/')]");
            this._selenium.Click("link=Delete this schedule");

            // Check notify
            if (notify == true) {
                if (this._selenium.IsChecked("notify") == false)
                    this._selenium.Click("notify");
            }

            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify
            this._selenium.VerifyTextPresent("No schedule exists.");
        }


        /// <summary>
        /// Deletes a schedule for a group under your span of care.
        /// </summary>
        /// <param name="socName">The name of the span of care.</param>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_Group_Delete_Schedule(string socName, string groupName, bool notify) {
            this.Groups_Group_View_Settings(socName, groupName);

            this._selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/GroupSchedule/')]");
            this._selenium.Click("link=Delete this schedule");

            // Check notify
            if (notify == true) {
                if (this._selenium.IsChecked("notify") == false)
                    this._selenium.Click("notify");
            }

            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
        }

        /// <summary>
        /// Edits a group member's joined date for the group.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="memberName">The name of the group member.</param>
        /// <param name="joinedDate">The date the member joined the group.</param>
        public void Groups_Group_ViewMember_Edit_Joined_Date_WebDriver(string groupName, string memberName, DateTime joinedDate) {
            // Store group start date

          //  string date = "10/01/2013";

            string date = "01/01/2014";
            DateTime startDate = Convert.ToDateTime(date);
            
            // View an individual
            this.Groups_Group_View_Individual_WebDriver(groupName, memberName);

            // Edit their "Member since" date
            this._driver.FindElementByXPath("//a[@class='modal_link']").Click();
            this._generalMethods.WaitForElement(this._driver, By.LinkText("Cancel"));

            // Enter the joined date for the group member
            this._driver.FindElementById("member_join_date").Clear();
            this._driver.FindElementById("member_join_date").SendKeys(joinedDate.ToShortDateString());

            // Click Save
            this._driver.FindElementById("submitQuery").Click();
            this._generalMethods.WaitForElement(this._driver, By.LinkText("Remove from group"));

            // If no error occurs verify date was updated
            if (!this._generalMethods.IsElementPresentWebDriver(By.XPath("//div[@class='error_msgs_for']")))
            {
                //this._generalMethods.VerifyTextPresentWebDriver(joinedDate.ToString ());
                this._generalMethods.VerifyTextPresentWebDriver(joinedDate.ToString("MM/dd/yyyy"));
            }
            else
            {
                // Validate the error that occurred
                if (joinedDate < startDate)
                {
                    this._generalMethods.VerifyTextPresentWebDriver(string.Format("Join date must be after the group was started. Group started on {0}", date));
                }
                else if (joinedDate > TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")))
                {
                    this._generalMethods.VerifyTextPresentWebDriver("You must provide a valid date that is not in the future");
                }
                else
                {
                    throw new System.Exception("WTF? Some other error message occured");
                }
            }
        }
        /// <summary>
        /// Adds an individual to a group.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="memberFirstName">The first name of the group member.</param>
        /// <param name="memberLastName">The last name of the group member.</param>
        public void Groups_Group_Add_New_Member_WebDriver(string groupName, string memberFirstName, string memberLastName)
        {
            // View the roster of a group
            this.Groups_Group_View_Roster_WebDriver(groupName);

            if (this._generalMethods.IsElementPresentWebDriver(By.LinkText(string.Format("{0} {1}", memberFirstName, memberLastName))))
            {
                this.Groups_Group_Delete_Member_WebDriver(groupName, string.Format("{0} {1}", memberFirstName, memberLastName));
                this.Groups_Group_View_Roster_WebDriver(groupName);
            }
            // Click to add a new group member
            this._driver.FindElementByLinkText("Add or Invite someone").Click();
            this._generalMethods.WaitForElement(this._driver, By.LinkText("Cancel"));

            // Add member to group
            this._driver.FindElementById("FirstName").SendKeys(memberFirstName);
            this._driver.FindElementById("LastName").SendKeys(memberLastName);
            this._driver.FindElementById("btn_invite").Click();
            this._generalMethods.WaitForElement(this._driver, By.LinkText("Start over"));

            // Confirm and add the member
            this._generalMethods.VerifyTextPresentWebDriver(string.Format("{0} {1}", memberFirstName, memberLastName));
            //this._driver.FindElementByXPath("//table[@class='grid']").FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[2].FindElements(By.TagName("a"))[0].Click();
            this._driver.FindElementByXPath("//div[contains(@class,'col-sm-6 text_right hidden-xs')]/a[text()='Add to group']").Click();
            
            //log.Debug("CSS count: " + this._driver.FindElementsByCssSelector("#modal_wrapper").Count);
            //log.Debug("Modal Enabled: " + this._driver.FindElementByXPath("//div[@class='modal_box']").Enabled);
            var wait = new WebDriverWait(this._driver, TimeSpan.FromSeconds(2));
            var clickableElement = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("modal_wrapper")));
            this._driver.FindElementByXPath("//div[@class='modal_box']/form/span/input").Click();
            
            //this._driver.FindElementByXPath("//div[@class='modal_box']").FindElements(By.TagName("form"))[0].FindElements(By.TagName("span"))[0].FindElements(By.TagName("input"))[0].Click();
            //this._driver.SwitchTo().Window(this._driver.WindowHandles[0]);
            this._generalMethods.WaitForElement(this._driver, By.Id("btn_invite"));
            this._driver.FindElementByLinkText("Cancel").Click();
            this._generalMethods.WaitForElement(this._driver, By.LinkText(string.Format("{0} {1}", memberFirstName, memberLastName)));

        }

        /// <summary>
        /// Removes an individual from a group.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="memberName">The name of the group member.</param>
        public void Groups_Group_Delete_Member_WebDriver(string groupName, string memberName)
        {
            // View the roster of a group
            this.Groups_Group_View_Roster_WebDriver(groupName);

            // Select the individual
            this._driver.FindElementByLinkText(memberName).Click();
            this._generalMethods.WaitForElement(this._driver, By.LinkText("Remove from group"));

            // Remove member from group
            this._driver.FindElementByLinkText("Remove from group").Click();
            this._driver.SwitchTo().Alert().Accept();

            // Confirm member has been removed
            this._generalMethods.WaitForElementInexistent(this._driver, By.LinkText(memberName));

        }
        


        #endregion Group Management

        #region Prospect Workflow

        /// <summary>
        /// Invites / adds a prospect to a group.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="prospectFirstName">The first name of the prospect.</param>
        /// <param name="prospectLastName">The last name of the prospect.</param>
        /// <param name="prospectEmailAddress">The email address for the prospect.</param>
        /// <param name="prospectPhone">The phone number of of the prospect.</param>
        /// <param name="personalMessage">The personal message you wish to use.</param>
        /// <param name="addIndividual">Specifies if you wish to add this individual insteaad of inviting them.</param>
        /// <param name="useOriginal">Specifies if you wish to use the original information to invite instead of the potential matches.</param>
        public void Groups_Prospect_Invite(string groupName, string prospectFirstName, string prospectLastName, string prospectEmailAddress, string prospectPhone, string personalMessage, bool addIndividual, bool useOriginal) {

            // View the group, if issues viewing group because we are re-routed to login then login else if something else throw exception with error
            try
            {
                this.Groups_Group_View_Roster(groupName);
            }
            catch (System.Exception e)
            {
                if (this._selenium.IsElementPresent("username"))
                {
                    this.ReLoginInFellowship(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);
                    this.Groups_Group_View_Roster(groupName);

                }
                else
                {
                    throw new System.Exception(e.Message, e);
                }

            }

            // Invite someone / Add them
            this._selenium.ClickAndWaitForPageToLoad("link=Add or Invite someone");

            // Step 1
            if (!string.IsNullOrEmpty(prospectFirstName)) {
                this._selenium.Type("FirstName", prospectFirstName);
            }

            if (!string.IsNullOrEmpty(prospectLastName)) {
                this._selenium.Type("LastName", prospectLastName);
            }

            if (!string.IsNullOrEmpty(prospectEmailAddress)) {
                this._selenium.Type("Email", prospectEmailAddress);
            }

            if (!string.IsNullOrEmpty(prospectPhone)) {
                this._selenium.Type("Phone", prospectPhone);
            }

            // Submit
            this._selenium.ClickAndWaitForPageToLoad("btn_invite");


            // Unless there was validation, continue to step 2
            if (!this._selenium.IsElementPresent("//div[@class='error_msgs_for']")) {
                // Did we find potential matches?
                //if (this._selenium.IsElementPresent("//body[@id='group_invitefind']")) {    //commented out by Winnie
                if (this._selenium.IsElementPresent("//a[contains(@class,'btn-success') and text()='Add to group']")) {    //added by Winnie   

                    // We did.  Get the row number for the prospect
                    var phone = !string.IsNullOrEmpty(prospectPhone) ? prospectPhone : string.Empty;
                    var email = !string.IsNullOrEmpty(prospectEmailAddress) ? string.Format(prospectEmailAddress) : string.Empty;
                    var emailPhone = string.IsNullOrEmpty(email) && string.IsNullOrEmpty(phone) ? "–" : !string.IsNullOrEmpty(email) && string.IsNullOrEmpty(phone) ? string.Format("{0}", prospectEmailAddress) : string.Format("{0} {1}", prospectEmailAddress, prospectPhone);
                    var rowNumber = this._generalMethods.GetTableRowNumber(TableIds.InFellowship_Groups_MatchedProspects, emailPhone, "Email/Phone") + 1;

                    // Do we want to use the original info?
                    if (useOriginal) {
                        this._selenium.Click("//div[@class='box gutter bottom']/a");
                        // Populate the invite information
                        if (!string.IsNullOrEmpty(prospectEmailAddress) && string.IsNullOrEmpty(this._selenium.GetValue("Email"))) {
                            this._selenium.Type("Email", prospectEmailAddress);
                        }

                        if (!string.IsNullOrEmpty(prospectPhone) && string.IsNullOrEmpty(this._selenium.GetValue("Phone"))) {
                            this._selenium.Type("Phone", prospectPhone);
                        }

                        if (!string.IsNullOrEmpty(personalMessage)) {
                            this._selenium.Type("Message", personalMessage);
                        }

                        this._selenium.ClickAndWaitForPageToLoad("submit");
                    }
                    else {
                        // We don't want to use the original info
                        // Do we want to add the individual?
                        if (addIndividual) {
                            // We do
                            //this._selenium.Click(string.Format("//table[@class='grid']/tbody/tr[{0}]/td[3]/a", rowNumber));
                            //Modify grace zhang:element change
                            this._selenium.Click(string.Format("//table[@class='grid']/tbody/tr[{0}]/td[2]/div[1]/a[1]", rowNumber));
                            //Modify grace zhang:Click  add group
                            this._selenium.ClickAndWaitForPageToLoad("//div[@class='modal_box']/form/span/input");
                        }
                        else {
                            // We don't
                            //this._selenium.Click(string.Format("//table[@class='grid']/tbody/tr[{0}]/td[4]/a", rowNumber));  //commented out by Winnie
                            this._selenium.Click(string.Format("//table[@class='grid']/tbody/tr[{0}]/td[2]/div[1]/a[2]", rowNumber));  //added by Winnie  
                            //this._selenium.WaitForCondition("selenium.isElementPresent(\"xpath=//div[@id='modal_wrapper']\");", "15000");
                            //this._selenium.WaitForCondition("selenium.isElementPresent(\"xpath=//div[@class='modal_title gutter_bottom_none']\");", "15000");  //commented out by Winnie
                            this._selenium.WaitForCondition("selenium.isElementPresent(\"xpath=//div[@class='modal_title gutter_bottom_none hidden-xs']\");", "15000");   //added by Winnie     
                            // Populate the invite information
                            if (!string.IsNullOrEmpty(prospectEmailAddress) && string.IsNullOrEmpty(this._selenium.GetValue("Email"))) {
                                this._selenium.Type("Email", prospectEmailAddress);
                            }

                            if (!string.IsNullOrEmpty(prospectPhone) && string.IsNullOrEmpty(this._selenium.GetValue("Phone"))) {
                                this._selenium.Type("Phone", prospectPhone);
                            }

                            if (!string.IsNullOrEmpty(personalMessage)) {
                                this._selenium.Type("Message", personalMessage);
                            }

                            this._selenium.ClickAndWaitForPageToLoad("submit");
                        }
                    }
                }
                else {
                    // We didn't find any matches        
                    // Populate the invite information
                    if (!string.IsNullOrEmpty(prospectEmailAddress) && string.IsNullOrEmpty(this._selenium.GetValue("Email"))) {
                        this._selenium.Type("Email", prospectEmailAddress);
                    }

                    if (!string.IsNullOrEmpty(prospectPhone) && string.IsNullOrEmpty(this._selenium.GetValue("Phone"))) {
                        this._selenium.Type("Phone", prospectPhone);
                    }

                    if (!string.IsNullOrEmpty(personalMessage)) {
                        this._selenium.Type("Message", personalMessage);
                    }

                    this._selenium.ClickAndWaitForPageToLoad("submit");
                }

                // If there is no validation on step 2, verify the notification message if we invited them
                if (!this._selenium.IsElementPresent("//div[@class='error_msgs_for']")) {
                    if (!addIndividual) {
                        this._selenium.VerifyTextPresent("Invitation sent!");                             
                    }
                }
                else {
                    // Validation occured.  The only valdiation that can occur is that someone is already in the group.
                    this._selenium.VerifyTextPresent("This user is already a part of this group.");

                    // Start over
                    this._selenium.ClickAndWaitForPageToLoad("link=Start over");
                }
            }

            else {
                // There was validation on Step 1
                if (string.IsNullOrEmpty(prospectFirstName)) {
                    this._selenium.VerifyTextPresent("First name is required.");
                }

                if (string.IsNullOrEmpty(prospectLastName)) {
                    this._selenium.VerifyTextPresent("Last name is required.");
                }
            }

            // Cancel out of the wizard
            this._selenium.ClickAndWaitForPageToLoad("link=Cancel");

            // Verify the prospect exists if an invite occured and the table exists
            if (!addIndividual && this._selenium.IsElementPresent(TableIds.InFellowship_ActiveProspects)) {
                Assert.IsTrue(this._generalMethods.ItemExistsInTable(TableIds.InFellowship_ActiveProspects, string.Format("{0} {1}", prospectFirstName, prospectLastName), "Name"), "Invited prospect was not present!");
                decimal itemRow = this._generalMethods.GetTableRowNumber(TableIds.InFellowship_ActiveProspects, "Test Invite", "Name");
                Assert.AreEqual("Invited", this._selenium.GetTable(string.Format("{0}.{1}.3", TableIds.InFellowship_ActiveProspects, itemRow + 1)), "Prospect status was incorrect!");
            }
        }

        /// <summary>
        /// Invites / adds a prospect to a group.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="prospectFirstName">The first name of the prospect.</param>
        /// <param name="prospectLastName">The last name of the prospect.</param>
        /// <param name="prospectEmailAddress">The email address for the prospect.</param>
        /// <param name="prospectPhone">The phone number of of the prospect.</param>
        /// <param name="personalMessage">The personal message you wish to use.</param>
        /// <param name="addIndividual">Specifies if you wish to add this individual insteaad of inviting them.</param>
        /// <param name="useOriginal">Specifies if you wish to use the original information to invite instead of the potential matches.</param>
        public void Groups_Prospect_Invite_WebDriver(string groupName, string prospectFirstName, string prospectLastName, string prospectEmailAddress, string prospectPhone, string personalMessage, 
            bool addIndividual, bool useOriginal, bool newInidividual)
        {
            // View the group, if issues viewing group because we are re-routed to login then login else if something else throw exception with error
            try
            {
                this.Groups_Group_View_Roster_WebDriver(groupName);
            }
            catch (System.Exception e)
            {
                //If have been logged out, let's try one more time
                if (this._generalMethods.IsElementPresentWebDriver(By.Id("username")))
                {
                    this.ReLoginWebDriver(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);
                    this.Groups_Group_View_Roster_WebDriver(groupName);
                }
                else
                {
                    throw new System.Exception(e.Message, e);
                }
            }

            // Invite someone / Add them
            this._driver.FindElementByLinkText("Add or Invite someone").Click();
            this._generalMethods.WaitForElement(By.Id("FirstName"));

            // Step 1
            if (!string.IsNullOrEmpty(prospectFirstName))
            {
                this._driver.FindElementById("FirstName").SendKeys(prospectFirstName);
            }

            if (!string.IsNullOrEmpty(prospectLastName))
            {
                this._driver.FindElementById("LastName").SendKeys(prospectLastName);
            }

            if (!string.IsNullOrEmpty(prospectEmailAddress))
            {
                this._driver.FindElementById("Email").SendKeys(prospectEmailAddress);
            }

            if (!string.IsNullOrEmpty(prospectPhone))
            {
                this._driver.FindElementById("Phone").SendKeys(prospectPhone);
            }

            // Submit
            this._driver.FindElementById("btn_invite").Click();
            if (newInidividual)
            {
                this._generalMethods.WaitForElement(By.LinkText("Cancel"));
            }
            else
            {
                this._generalMethods.WaitForElement(By.LinkText("Start over"));
            }

            // Unless there was validation, continue to step 2
            if (!this._generalMethods.IsElementPresentWebDriver(By.XPath("//div[@class='error_msgs_for']")))
            {
                // Did we find potential matches?
                if (this._generalMethods.IsElementPresentWebDriver(By.XPath("//body[@id='group_invitefind']")))
                {

                    // We did.  Get the row number for the prospect
                    var phone = !string.IsNullOrEmpty(prospectPhone) ? prospectPhone : string.Empty;
                    var email = !string.IsNullOrEmpty(prospectEmailAddress) ? string.Format(prospectEmailAddress) : string.Empty;
                    var emailPhone = string.IsNullOrEmpty(email) && string.IsNullOrEmpty(phone) ? "–" : !string.IsNullOrEmpty(email) && string.IsNullOrEmpty(phone) ? string.Format("{0}", prospectEmailAddress) : string.Format("{0} {1}", prospectEmailAddress, prospectPhone);
                    //var rowNumber = this._generalMethods.GetTableRowNumberWebDriver(TableIds.InFellowship_Groups_MatchedProspects, emailPhone, "Email/Phone", "contains") + 1;

                    //Find Email match
                    //GetTableRowNumber method not working like we want to so hacking it
                    int rowNumber = 0;
                    IWebElement table = this._driver.FindElementByXPath(TableIds.InFellowship_Groups_MatchedProspects);
                    int rowsTbl = table.FindElements(By.TagName("tr")).Count;
                    for (int r = 1; r < rowsTbl; r++)
                    {
                        string emailPhoneTxt = table.FindElements(By.TagName("tr"))[r].FindElements(By.TagName("td"))[1].Text;
                        TestLog.WriteLine("Email/Phone: {0}", emailPhoneTxt);
                        if (emailPhoneTxt.Contains(emailPhone))
                        {
                            log.DebugFormat("Email {0} match found", emailPhoneTxt);
                            rowNumber = r+1;
                            break;
                        }
                    }

                    //Check if we found if not why continue
                    if (rowNumber < 1)
                    {
                        throw new WebDriverException(string.Format("Email/Phone [{0}] was not found", emailPhone));
                    }

                    // Do we want to use the original info?
                    if (useOriginal)
                    {
                        this._driver.FindElementByXPath("//div[@class='box gutter_bottom']/a").Click();
                        
                        // Populate the invite information
                        if (!string.IsNullOrEmpty(prospectEmailAddress) && string.IsNullOrEmpty(this._driver.FindElementById("Email").Text.ToString()))
                        {
                            this._driver.FindElementById("Email").SendKeys(prospectEmailAddress);
                        }

                        if (!string.IsNullOrEmpty(prospectPhone) && string.IsNullOrEmpty(this._driver.FindElementById("Phone").Text.ToString()))
                        {
                            this._driver.FindElementById("Phone").SendKeys(prospectPhone);
                        }

                        if (!string.IsNullOrEmpty(personalMessage))
                        {
                            this._driver.FindElementById("Message").SendKeys(personalMessage);
                        }

                        this._driver.FindElementById("submit").Click();
                    }
                    else
                    {
                        // We don't want to use the original info
                        // Do we want to add the individual?
                        if (addIndividual)
                        {
                            // We do
                            this._driver.FindElementByXPath(string.Format("//table[@class='grid']/tbody/tr[{0}]/td[3]/a", rowNumber)).Click();
                            this._driver.FindElementByXPath("//div[@class='modal_box']/form/span/input").Click();
                        }
                        else
                        {
                            // We don't
                            this._driver.FindElementByXPath(string.Format("//table[@class='grid']/tbody/tr[{0}]/td[4]/a", rowNumber)).Click();

                            // Populate the invite information
                            if (!string.IsNullOrEmpty(prospectEmailAddress) && string.IsNullOrEmpty(this._driver.FindElementById("Email").GetAttribute("value").ToString()))
                            {
                                this._driver.FindElementById("Email").Clear();                            
                                this._driver.FindElementById("Email").SendKeys(prospectEmailAddress);
                            }

                            if (!string.IsNullOrEmpty(prospectPhone) && string.IsNullOrEmpty(this._driver.FindElementById("Phone").GetAttribute("value").ToString()))
                            {
                                this._driver.FindElementById("Phone").Clear();                            
                                this._driver.FindElementById("Phone").SendKeys(prospectPhone);
                            }

                            if (!string.IsNullOrEmpty(personalMessage))
                            {
                                this._driver.FindElementById("Message").SendKeys(personalMessage);
                            }

                            this._driver.FindElementById("submit").Click();

                            if (this._generalMethods.IsElementVisibleWebDriver(By.Id("email_error")))
                            {
                                throw new WebDriverException(this._driver.FindElementById("email_error").Text);
                            }

                        }
                    }
                }
                else
                {
                    // We didn't find any matches        
                    // Populate the invite information
                    if (!string.IsNullOrEmpty(prospectEmailAddress) && string.IsNullOrEmpty(this._driver.FindElementById("Email").GetAttribute("value").ToString()))
                    {
                        if (!this._driver.FindElementById("Email").GetAttribute("value").Equals(prospectEmailAddress))
                        {
                            this._driver.FindElementById("Email").Clear();                            
                            this._driver.FindElementById("Email").SendKeys(prospectEmailAddress);
                        }
                    }

                    if (!string.IsNullOrEmpty(prospectPhone) && string.IsNullOrEmpty(this._driver.FindElementById("Phone").GetAttribute("value").ToString()))
                    {
                        this._driver.FindElementById("Phone").Clear();
                        this._driver.FindElementById("Phone").SendKeys(prospectPhone);
                    }

                    if (!string.IsNullOrEmpty(personalMessage))
                    {
                        this._driver.FindElementById("Message").SendKeys(personalMessage);
                    }

                    this._generalMethods.WaitForElementEnabled(By.XPath("//input[@value='Send Invite']"), 30, "Submit Button not Enabled");
                    this._driver.FindElement(By.XPath("//span[@class='submit_wait']"));
                    this._driver.FindElement(By.XPath("//input[@value='Send Invite']")).Click();

                    if (this._generalMethods.IsElementVisibleWebDriver(By.Id("email_error")))
                    {
                        throw new WebDriverException(this._driver.FindElementById("email_error").Text);
                    }

                }

                // If there is no validation on step 2, verify the notification message if we invited them
                if (!this._generalMethods.IsElementPresentWebDriver(By.XPath("//div[@class='error_msgs_for']")))
                {
                    if (!addIndividual)
                    {
                        //this._generalMethods.VerifyTextPresentWebDriver("Invitation sent!");
                        //Verify Message Sent Flyout
                        this._generalMethods.WaitForElementDisplayed(By.XPath("//span[@class='big']"), 30, "Did not detect the invitation sent flyout in the specified time.");
                        Assert.AreEqual("Invitation sent!", this._driver.FindElementByXPath("//span[@class='big']").Text.Trim());

                    }
                }
                else
                {
                    // Validation occured.  The only valdiation that can occur is that someone is already in the group.
                    this._generalMethods.VerifyTextPresentWebDriver("This user is already a part of this group.");

                    // Start over
                    this._driver.FindElementByLinkText("Start over").Click();
                }
            }

            else
            {
                // There was validation on Step 1
                if (string.IsNullOrEmpty(prospectFirstName))
                {
                    this._generalMethods.VerifyTextPresentWebDriver("First name is required.");
                }

                if (string.IsNullOrEmpty(prospectLastName))
                {
                    this._generalMethods.VerifyTextPresentWebDriver("Last name is required.");
                }
            }

            // Cancel out of the wizard
            this._driver.FindElementByLinkText("Cancel").Click();

            // Verify the prospect exists if an invite occured and the table exists
            if (!addIndividual && this._generalMethods.IsElementPresentWebDriver(By.XPath(TableIds.InFellowship_ActiveProspects)))
            {
                Assert.IsTrue(this._generalMethods.ItemExistsInTableWebDriver(TableIds.InFellowship_ActiveProspects, string.Format("{0} {1}", prospectFirstName, prospectLastName), "Name"), "Invited prospect was not present!");
                int itemRow = this._generalMethods.GetTableRowNumberWebDriver(TableIds.InFellowship_ActiveProspects, "Test Invite", "Name");
                IWebElement groupTable = this._driver.FindElementByXPath(TableIds.InFellowship_ActiveProspects);
                Assert.AreEqual("Invited", groupTable.FindElements(By.TagName("tr"))[itemRow+1].FindElements(By.TagName("td"))[3].ToString(), "Prospect status was incorrect!");
                //Assert.AreEqual("Invited", this._selenium.GetTable(string.Format("{0}/{1}/3", TableIds.InFellowship_ActiveProspects, itemRow + 1)), "Prospect status was incorrect!");
            }
        }

        /// <summary>
        /// Invites / adds a prospect to a group under your span of care.
        /// </summary>
        /// <param name="socName">The name of your span of care.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="prospectFirstName">The first name of the prospect.</param>
        /// <param name="prospectLastName">The last name of the prospect.</param>
        /// <param name="prospectEmailAddress">The email address for the prospect.</param>
        /// <param name="prospectPhone">The phone number of of the prospect.</param>
        /// <param name="personalMessage">The personal message you wish to use.</param>
        /// <param name="addIndividual">Specifies if you wish to add this individual insteaad of inviting them.</param>
        /// <param name="useOriginal">Specifies if you wish to use the original information to invite instead of the potential matches.</param>
        public void Groups_Prospect_Invite(string socName, string groupName, string prospectFirstName, string prospectLastName, string prospectEmailAddress, string prospectPhone, string personalMessage, bool addIndividual, bool useOriginal) {
            // View the group under your span of care.
            this.Groups_Group_View(socName, groupName);

            // Invite the prospect
            this.Groups_Prospect_Invite(groupName, prospectFirstName, prospectLastName, prospectEmailAddress, prospectPhone, personalMessage, addIndividual, useOriginal);

        }

        /// <summary>
        /// Expresses interest to a group.
        /// </summary>
        /// <param name="test">The current test object.</param>
        /// <param name="groupName">The group to express interest to.</param>
        /// <param name="prospectFirstName">The first name of the prospect.</param>
        /// <param name="prospectLastName">The last name of the prospect.</param>
        /// <param name="prospectEmailAddress">The prospect's email address.</param>
        public void Groups_Prospect_ExpressInterest(string groupName, string prospectFirstName, string prospectLastName, string prospectEmailAddress) {
            // Find a group
            this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LoginPage.Link_FindAGroupHere);
            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Express interest in a group
            decimal itemRow = this._generalMethods.GetTableRowNumber("//table[@class='grid']", groupName, "Group", null);
            this._selenium.ClickAndWaitForPageToLoad(string.Format("//table[@class='grid']/tbody/tr[{0}]/td[1]/a", itemRow + 1));
            //this._selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", groupName));

            // Supply the necessary information
            this._selenium.Type("FirstName", prospectFirstName);
            this._selenium.Type("LastName", prospectLastName);
            this._selenium.Type("EmailAddress", prospectEmailAddress);
            this._selenium.Type("Message", "I am expressing interest!");

            this._selenium.ClickAndWaitForPageToLoad("btn_save");

            //TODO
            //Check for error messages

        }
        /// <summary>
        /// This method allows a logged in user to express interest in a group
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="userFullName"></param>
        /// <param name="userEmail"></param>
        /// <param name="phoneNumber">Optional</param>
        /// <param name="messageText">Optional</param>
        public void Groups_Prospect_ExpressInterest_UserLoggedIn_WebDriver(string groupName, string userFullName, string userEmail, string phoneNumber = null, string messageText = null)
        {
            // Find a group
            this._driver.FindElementByLinkText("FIND A GROUP").Click();
            this._generalMethods.WaitForElement(By.Id("zipcode"));
            this._driver.FindElementById(GeneralButtons.submitQuery).Click();
            this._generalMethods.WaitForElement(By.XPath(TableIds.InFellowship_FindAGroup_Results));

            // Express interest in a group
            this._generalMethods.GetTableRowNumberWebDriver(TableIds.InFellowship_FindAGroup_Results, groupName, "Group");
            this._driver.FindElementByLinkText(groupName).Click();
            this._generalMethods.WaitForElement(By.Id("btn_save"));

            //Verify User information
            Assert.AreEqual(userFullName, this._driver.FindElementByXPath("//div[@class='grid_5']/form/p/strong").Text);
            Assert.AreEqual(userEmail, this._driver.FindElementByXPath("//div[@class='grid_5']/form/p/span").Text);
                ////html/body/div/div[3]/div[3]/div/div[2]/form/p/strong
                ////html/body/div/div[3]/div[3]/div/div[2]/form/p/span
            //Enter valid fields
            if (phoneNumber != null)
            {
                this._driver.FindElementById("Phone").SendKeys(phoneNumber);
            }
            if (messageText != null)
            {
                this._driver.FindElementById("Message").SendKeys(messageText);
            }
            //Submit Interest
            this._driver.FindElementById("btn_save").Click();
            
            //Verify prospect was submitted
            Assert.AreEqual("Thanks!", this._driver.FindElementByXPath("//div[@class='grid_5']/h3").Text.Trim());
            ///html/body/div/div[3]/div[3]/div/div[2]/h3
            Assert.AreEqual("Thank you for expressing interest in this group. You will be contacted by a leader of this group once your request has been processed.", this._driver.FindElementByXPath("//div[@class='grid_5']/p").Text.Trim());
            ///html/body/div/div[3]/div[3]/div/div[2]/p
        }

        /// <summary>
        /// Expresses interest to a group (Converted to Webdriver).
        /// </summary>
        /// <param name="test">The current test object.</param>
        /// <param name="groupName">The group to express interest to.</param>
        /// <param name="prospectFirstName">The first name of the prospect.</param>
        /// <param name="prospectLastName">The last name of the prospect.</param>
        /// <param name="prospectEmailAddress">The prospect's email address.</param>
        public void Groups_Prospect_ExpressInterest_WebDriver(string groupName, string prospectFirstName, string prospectLastName, string prospectEmailAddress, string messageText = null, string captchaText = null, string zipCode = null)
        {

            Captcha captcha = null;
            string captchaErrorTxt = "The characters typed in the challenge didn’t match.";

            // Find a group
            this._driver.FindElementByLinkText("FIND A GROUP").Click();
            this._generalMethods.WaitForElement(By.Id("zipcode"));

            //If user passes in zipcode for better search results, enter zipcode
            if (!string.IsNullOrEmpty(zipCode))
            {
                this._driver.FindElementById("zipcode").SendKeys(zipCode);
            }

            this._driver.FindElementById(GeneralButtons.submitQuery).Click();
            this._generalMethods.WaitForElement(By.XPath(TableIds.InFellowship_FindAGroup_Results));

            // Express interest in a group
            int row = this._generalMethods.GetTableRowNumberWebDriver(TableIds.InFellowship_FindAGroup_Results, groupName, "Group") + 1;
            this._driver.FindElementByXPath(TableIds.InFellowship_FindAGroup_Results).FindElement(By.XPath(string.Format("//tbody/tr[{0}]/td[1]/a", row))).Click();
            //this._driver.FindElementByLinkText(groupName).Click();
            this._generalMethods.WaitForElement(By.Id("VerificationCaptcha_CaptchaImage"));
            
            //NOTE: Placing in order of error messages
            //If we pass in the text then don't get it
            captcha = this.Enter_Captcha_Text(captchaText, captchaErrorTxt);

            // Supply the necessary information
            if (!string.IsNullOrEmpty(prospectFirstName))
            {
                this._driver.FindElementByName("FirstName").SendKeys(prospectFirstName);
            }
            else
            {
                _errorText.Add("First name is required");
            }

            if (!string.IsNullOrEmpty(prospectLastName))
            {
                this._driver.FindElementByName("LastName").SendKeys(prospectLastName);
            }
            else
            {
                _errorText.Add("Last name is required");
            }

            if (!string.IsNullOrEmpty(prospectEmailAddress))
            {
                this._driver.FindElementByName("EmailAddress").SendKeys(prospectEmailAddress);
            }
            else
            {
                _errorText.Add("Email address is required");
            }

            if (!string.IsNullOrEmpty(messageText))
            {
                this._driver.FindElementByName("Message").SendKeys(messageText);
            }

            //Submit
            this._driver.FindElementById("btn_save").Click();

            //Try again if only Captcha Error
            if(Is_Captcha_Error_Only(captchaText, captchaErrorTxt)){
                this.Send_Captcha_Report_Error(captcha);
                captcha = this.Enter_Captcha_Text(captchaText, captchaErrorTxt);

                //Submit
                this._driver.FindElementById("btn_save").Click();

            }

            //Verify Errors and report Captcha errors if any
            if (this._generalMethods.IsElementPresentWebDriver(By.XPath("//div[@class='error_msgs_for']")))
            {
                
                //How many errors do we have
                int errors = this._driver.FindElementByXPath("//div[@class='error_msgs_for']").FindElement(By.TagName("ul")).FindElements(By.TagName("li")).Count;
                
                //Loops through the errors and verify them
                for (int e = 0; e < errors; e++)
                {
                    log.DebugFormat("Errors: {0}", this._driver.FindElementByXPath("//div[@class='error_msgs_for']").FindElement(By.TagName("ul")).FindElements(By.TagName("li"))[e].Text);
                    string err = this._driver.FindElementByXPath("//div[@class='error_msgs_for']").FindElement(By.TagName("ul")).FindElements(By.TagName("li"))[e].Text;

                    //If we did not pass captcha text and got a captcha error let DeathByCaptcha know
                    if ((err.Equals(captchaErrorTxt)) && (string.IsNullOrEmpty(captchaText)))
                    {
                        //Send Error Report
                        this.Send_Captcha_Report_Error(captcha);
                        throw new WebDriverException("The captcha was invalid.");
                    }

                    if (_errorText.Count > 0)
                    {
                        Assert.AreEqual(_errorText[e], err, "Invalid error message");
                    }
                    else
                    {
                        throw new WebDriverException(err);
                    }
                }

                //Verify expected num of errors
                if (errors != this._errorText.Count)
                {
                    Assert.AreEqual(this._errorText.Count, errors, "Errors expected do not match");
                }

                //Singular or Plural error message wording
                if (_errorText.Count == 1)
                {
                    Assert.AreEqual(TextConstants.ErrorHeadingSingular, this._driver.FindElementByXPath("//div[@class='error_msgs_for']").FindElement(By.TagName("h2")).Text);
                }
                else
                {
                    Assert.AreEqual(TextConstants.ErrorHeadingPlural, this._driver.FindElementByXPath("//div[@class='error_msgs_for']").FindElement(By.TagName("h2")).Text);
                }

            }
            else
            {
                this._generalMethods.VerifyTextPresentWebDriver("Thank you for expressing interest in this group. You will be contacted by a leader of this group once your request has been processed.");
            }
        }

        /// <summary>
        /// Verify Captcha when reloaded
        /// </summary>
        /// <param name="groupName">Group Name</param>
        public void Groups_Prospect_ExpressInterest_Captcha_Reload_WebDriver(string groupName)
        {

            Captcha captchaOrig = null;
            Captcha captchaUpdate = null;

            // Find a group
            this._driver.FindElementByLinkText("FIND A GROUP").Click();
            this._generalMethods.WaitForElement(By.Id("zipcode"));
            this._driver.FindElementById(GeneralButtons.submitQuery).Click();
            this._generalMethods.WaitForElement(By.XPath(TableIds.InFellowship_FindAGroup_Results));

            // Express interest in a group            
            //int row = this._generalMethods.GetTableRowNumberWebDriver(TableIds.InFellowship_FindAGroup_Results, groupName, "Group") + 1;
            //this._driver.FindElementByXPath(TableIds.InFellowship_FindAGroup_Results).FindElement(By.XPath(string.Format("//tbody/tr[{0}]/td/a", row))).Click();            
            this._driver.FindElementByLinkText(groupName).Click();
            this._generalMethods.WaitForElement(By.Id("VerificationCaptcha_CaptchaImage"));

            //Get original Captcha
            captchaOrig = this.Get_Captcha_Text_WebDriver(By.Id("VerificationCaptcha_CaptchaImage"));

            //Type in Captha for Demo
            this._driver.FindElementById("CaptchaCode").SendKeys(captchaOrig.Text);

            this._generalMethods.TakeScreenShot_WebDriver();

            //Reload a new Captcha
            this._driver.FindElementById("VerificationCaptcha_ReloadIcon").Click();

            //Get updated Captcha
            captchaUpdate = this.Get_Captcha_Text_WebDriver(By.Id("VerificationCaptcha_CaptchaImage"));

            //Type in Captcha for Demo
            this._driver.FindElementById("CaptchaCode").Clear();
            this._driver.FindElementById("CaptchaCode").SendKeys(captchaUpdate.Text);

            this._generalMethods.TakeScreenShot_WebDriver();

            //Verify that they we got a different one
            Assert.AreNotEqual(captchaUpdate.Text, captchaOrig.Text, "Captcha was not updated");

        }

        /// <summary>
        /// Tells a friend about a group.
        /// </summary>
        /// <param name="prospectFirstName">The first name of the prospect.</param>
        /// <param name="prospectLastName">The last name of the prospect.</param>
        /// <param name="prospectEmailAddress">The prospect's email address.</param>
        /// <param name="message"></param>
        public void Groups_Prospect_TellAFriend(string prospectFirstName, string prospectLastName, string prospectEmailAddress, string message) {

            // Tell a friend, from the Gear 
            //this._selenium.Click("nav_gear"); Modify by Clark.Peng
            this._selenium.ClickAndWaitForPageToLoad("//div[contains(@class,'hidden-xs')]//span[contains(text(),'Quick Links')]");
            this._selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Group/TellAFriend/')]");

            // Fill out the information and send the message
            this._selenium.Type("FirstName", prospectFirstName);
            this._selenium.Type("LastName", prospectLastName);
            this._selenium.Type("Email", prospectEmailAddress);
            this._selenium.Type("Note", message);

            // Submit
            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify the message
            this._selenium.VerifyTextPresent("Saved!");
        }


        /// <summary>
        /// Views all the prospects of a group as a leader.
        /// </summary>
        /// <param name="groupName">The name of the group you wish to view the prospects of.</param>
        public void Groups_Prospect_View_All(string groupName) {
            // View the group
            this.Groups_Group_View_Roster(groupName);
            // View the prospects
            this._selenium.ClickAndWaitForPageToLoad("link=View prospects");
        }

        /// <summary>
        /// Views all the prospects of a group as a span of care owner.
        /// </summary>
        /// <param name="socName">The span of care the group falls under.</param>
        /// <param name="groupName">The name of the group you wish to view the prospects of.</param>
        public void Groups_Prospect_View_All(string socName, string groupName) {
            // View the group
            this.Groups_Group_View_Roster(socName, groupName);
            // View the prospects
            this._selenium.ClickAndWaitForPageToLoad("link=View prospects");
        }

        /// <summary>
        /// Views a specific prospect of a group as a leader.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="prospectName">The prospect you wish to view.</param>
        public void Groups_Prospect_View(string groupName, string prospectName) {
            this.Groups_Prospect_View_All(groupName);
            this._selenium.ClickAndWaitForPageToLoad("link=" + prospectName);
        }

        /// <summary>
        /// Views a specific prospect of a group as an owner.
        /// </summary>
        /// <param name="socName">The span of care the group falls under.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="prospectName">The prospect you wish to view.</param>
        public void Groups_Prospect_View(string socName, string groupName, string prospectName) {
            this.Groups_Prospect_View_All(socName, groupName);
            this._selenium.ClickAndWaitForPageToLoad("link=" + prospectName);
        }

        /// <summary>
        /// Tells a friend about a group.
        /// </summary>
        /// <param name="prospectFirstName">The first name of the prospect.</param>
        /// <param name="prospectLastName">The last name of the prospect.</param>
        /// <param name="prospectEmailAddress">The prospect's email address.</param>
        /// <param name="message"></param>
        public void Groups_Prospect_TellAFriend_WebDriver(string prospectFirstName, string prospectLastName, string prospectEmailAddress, string message)
        {

            // Tell a friend, from the Gear
            this._driver.FindElementById("nav_gear").Click();
            this._driver.FindElementByXPath("//a[contains(@href, '/Group/TellAFriend/')]").Click();

            // Fill out the information and send the message
            this._driver.FindElementById("FirstName").SendKeys(prospectFirstName);
            this._driver.FindElementById("LastName").SendKeys(prospectLastName);
            this._driver.FindElementById("Email").SendKeys(prospectEmailAddress);
            this._driver.FindElementById("Note").SendKeys(message);

            // Submit
            this._driver.FindElementById(GeneralButtons.submitQuery).Click();

            // Verify the message
            this._generalMethods.VerifyTextPresentWebDriver("Saved!");
        }


        /// <summary>
        /// Views all the prospects of a group as a leader.
        /// </summary>
        /// <param name="groupName">The name of the group you wish to view the prospects of.</param>
        public void Groups_Prospect_View_All_WebDriver(string groupName)
        {
            // View the group
            try
            {
                this.Groups_Group_View_Roster_WebDriver(groupName);
                // View the prospects
                this._driver.FindElementByLinkText("View prospects").Click();
            }
            catch (System.Exception e)
            {
                //If have been logged out, let's try one more time
                if (this._generalMethods.IsElementPresentWebDriver(By.Id("username")))
                {
                    this.ReLoginWebDriver(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);
                    this.Groups_Group_View_Roster_WebDriver(groupName);
                    // View the prospects
                    this._driver.FindElementByLinkText("View prospects").Click();
                }
                else
                {
                    throw new System.Exception(e.Message, e);
                }
            }
        }

        /// <summary>
        /// Views all the prospects of a group as a span of care owner.
        /// </summary>
        /// <param name="socName">The span of care the group falls under.</param>
        /// <param name="groupName">The name of the group you wish to view the prospects of.</param>
        public void Groups_Prospect_View_All_WebDriver(string socName, string groupName)
        {
            // View the group
            try
            {
                this.Groups_Group_View_Roster_WebDriver(socName, groupName);
            }
            catch (System.Exception e)
            {
                //If have been logged out, let's try one more time
                if (this._generalMethods.IsElementPresentWebDriver(By.Id("username")))
                {
                    this.ReLoginWebDriver(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);
                    this.Groups_Group_View_Roster_WebDriver(socName, groupName);
                }
                else
                {
                    throw new System.Exception(e.Message, e);
                }
            }

            // View the prospects
            this._driver.FindElementByLinkText("View prospects").Click();
        }

        /// <summary>
        /// Views a specific prospect of a group as a leader.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="prospectName">The prospect you wish to view.</param>
        public void Groups_Prospect_View_WebDriver(string groupName, string prospectName)
        {
            try
            {
                this.Groups_Prospect_View_All_WebDriver(groupName);
                this._driver.FindElementByLinkText(prospectName).Click();
            }
            catch (System.Exception e)
            {
                //If have been logged out, let's try one more time
                if (this._generalMethods.IsElementPresentWebDriver(By.Id("username")))
                {
                    this.ReLoginWebDriver(this._infellowshipEmail, this._infellowshipPassword, this._infellowshipChurchCode);
                    this.Groups_Prospect_View_All_WebDriver(groupName);
                    this._driver.FindElementByLinkText(prospectName).Click();
                }
                else
                {
                    throw new System.Exception(e.Message, e);
                }
            }
        }

        /// <summary>
        /// Views a specific prospect of a group as an owner.
        /// </summary>
        /// <param name="socName">The span of care the group falls under.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="prospectName">The prospect you wish to view.</param>
        public void Groups_Prospect_View_WebDriver(string socName, string groupName, string prospectName)
        {
            this.Groups_Prospect_View_All_WebDriver(socName, groupName);
            this._driver.FindElementByLinkText(prospectName).Click();
        }
        #endregion Prospect Workflow

        #region Find a Group
        /// <summary>
        /// Finds a group using no specific criteria.
        /// </summary>
        public void Groups_FindAGroup() {
            if (this._selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_Logout)) {
                if (this._selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup))
                    this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup);
                else {
                    this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
                    this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup);
                }
            }

            // If you aren't logged in, figure out if you are on the login page or inside the app
            else {
                this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LoginPage.Link_FindAGroupHere);
            }

            // Submit
            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
        }

        /// <summary>
        /// Finds a group based on the given parameters.
        /// </summary>
        /// <param name="selenium"></param>
        /// <param name="zipCode">A zip code to search the proximity around.  Note, online groups will not be returned.</param>
        /// <param name="campus">Filters on groups that only meet at a given campus.</param>
        /// <param name="categoryName">Filters on groups that only fall into this category.</param>
        /// <param name="childCare">Filters groups that only provide childcare.</param>
        public void Groups_FindAGroup_Search(string zipCode, string campus, string categoryName, bool childCare) {
            // Figure out if you are logged in.  If you are, figure out where you are.  If the Find a group link isn't present, hit home and then Find a Group.
            if (this._selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_Logout)) {
                if (this._selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup))
                    this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup);
                else {
                    this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
                    this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup);
                }
            }

            // If you aren't logged in, figure out if you are on the login page or inside the app
            else {
                this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LoginPage.Link_FindAGroupHere);
            }

            // Specify the zipcode, if provided
            this._selenium.Type(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.TextField_ZipCode, zipCode);

            // Specify the campus if provided only if the campus drop down is present.
            if (this._selenium.IsElementPresent(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Dropdown_Campus) && !string.IsNullOrEmpty(campus)) {
                this._selenium.Select(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Dropdown_Campus, campus);
            }

            // Pick the category, if specified.
            if (!string.IsNullOrEmpty(categoryName)) {
                this._selenium.Select(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Dropdown_Category, categoryName);
            }

            // If specified, filter on childcare.
            if (childCare) {
                this._selenium.Click(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Checkbox_ChildcareProvided);
            }

            // Submit
            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
        }

        /// <summary>
        /// Finds a group based on the given parameters.
        /// </summary>
        /// <param name="zipCode">A zip code to search the proximity around.  Note, online groups will not be returned.</param>
        /// <param name="campus">Filters on groups that only meet at a given campus.</param>
        /// <param name="categoryName">Filters on groups that only fall into this category.</param>
        /// <param name="childCare">Filters groups that only provide childcare.</param>
        /// <param name="weekday">Filters on groups that only meet on this day.</param>
        /// <param name="startTime">Filters on groups that only meet during this start time range.</param>
        public void Groups_FindAGroup_Search(string zipCode, string campus, string categoryName, bool childCare, GeneralEnumerations.WeeklyScheduleDays weekday, GeneralEnumerations.StartTimes startTime) {
            // Figure out if you are logged in.  If you are, figure out where you are.  If the Find a group link isn't present, hit home and then Find a Group.
            if (this._selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_Logout)) {
                if (this._selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup))
                    this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup);
                else {
                    this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
                    this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup);
                }
            }

            // If you aren't logged in, figure out if you are on the login page or inside the app
            else {
                this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LoginPage.Link_FindAGroupHere);
            }

            // Find a group here to view the advanced search options
            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Specify the zipcode, if provided
            this._selenium.Type(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.TextField_ZipCode, zipCode);

            // Specify the campus if provided only if the campus drop down is present.
            if (this._selenium.IsElementPresent(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Dropdown_Campus) && !string.IsNullOrEmpty(campus)) {
                this._selenium.Select(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Dropdown_Campus, campus);
            }

            // Pick the category, if specified.
            if (!string.IsNullOrEmpty(categoryName)) {
                this._selenium.Select(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Dropdown_Category, categoryName);
            }

            // If specified, filter on childcare.
            if (childCare) {
                this._selenium.Click(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Checkbox_ChildcareProvided);
            }

            // If specified, filter on Weekday
            if (weekday != GeneralEnumerations.WeeklyScheduleDays.Any) {
                this._selenium.Select(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Dropdown_Weekday, weekday.ToString());
            }

            // If specified, select a start time
            if (startTime != GeneralEnumerations.StartTimes.Any) {
                this._selenium.Select(InFellowshipConstants.Groups.GroupsConstants.FindAGroupPage.Dropdown_StartTime, startTime.ToString());
            }

            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
        }

        /// <summary>
        /// Views the find the group page via the gear whilst in a group.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_FindAGroup_View_Gear(string groupName) {
            // View a group
            this.Groups_Group_View(groupName);

            // Mouse over the gear
            this._selenium.MouseOver("//div[@class='nav_gear_wrapper']/span");
            

            // Click Find a Group
            this._selenium.ClickAndWaitForPageToLoad("link=Find a group");
        }

        /// <summary>
        /// Views the groups that meet at the same location
        /// </summary>
        /// <param name="test"></param>
        /// <param name="locationAddress">The address for the location (i.e. 123 Main Street).</param>
        /// <param name="locationCityStateCip">The city state and zip for the location. (i.e. Flower Mound, TX 75022).  Note, it must follow that format.</param>
        public void Groups_FindAGroup_View_GroupsAtSameLocation(string locationAddress, string locationCityStateCip)
        {

            TestLog.WriteLine("Click On Map View");
            // View the map
            this._selenium.Click(GeneralInFellowshipConstants.GeneralLinks.Link_MapView);

            Thread.Sleep(2000);

            // Build the string for the text, for the second paragraph tag, that is shown inside the bubble that shows when you click a marker in the map.  Also need to append the text View larger map.
            // Is a private location, meaning we passed in null for the address?
            string location = !string.IsNullOrEmpty(locationAddress) ? string.Concat(locationAddress, "\n", locationCityStateCip, "\nView larger map") : location = string.Concat(locationCityStateCip, "\nView larger map");

            TestLog.WriteLine("Location: " + location);
            // Get the number of map markers.  The 5th div element in this mess of div elements holds the image.
            //int numberOfMapMarkers = (Int32)this._selenium.GetXpathCount("//div[@id='map_view']/div/div/div/div[5]/img");
            // /html/body/div/div/div[3]/div/div/div[2]/div/div/div/div/div[2]/div[2]/div/div/div[5]/canvas
            // /html/body/div/div/div[3]/div/div/div[2]/div/div/div/div
            // /html/body/div/div/div[3]/div/div/div[2]/div/div/div[7]/div[2]/div/img
            // /html/body/div/div/div[3]/div/div/div[2]/div/div/div/div/div[2]/div[2]/div/div/div/canvas
            // /html/body/div/div/div[3]/div/div/div[2]/div/div/div/div/div[2]/div[2]/div/div/div[5]/canvas

            int mapBubbleBlue = (Int32)this._selenium.GetXpathCount("//div[@id='map_bubble']");
            int mapBubbleContent = (Int32)this._selenium.GetXpathCount("//div[@id='map_bubble_content']");

            int numberOfCanvas = (Int32)this._selenium.GetXpathCount("/html/body/div/div/div[3]/div/div/div[2]/div/div/div/div/div[2]/div[2]/div/div/div[5]/canvas");
            //int numberOfMapMarkers = (Int32)this._selenium.GetXpathCount("//div[@id='map_view']/div/div/div[7]/div[2]/div/img");
            int numberOfMapMarkers = (Int32)this._selenium.GetXpathCount("//div[@id='map_view']/div/div/div/div/img");
            TestLog.WriteLine("Map Bubble Blue: " + mapBubbleBlue);
            TestLog.WriteLine("Canvas Markers: " + numberOfCanvas);
            TestLog.WriteLine("Map Markers: " + numberOfMapMarkers);

            // For each of these markers, get the styles for each marker.  The seventh div element stores all the important style information.  A width of 27px means it is the marker that is flagged for multiple groups
            string[] mapMarkerStyles = new string[numberOfMapMarkers];
            for (int i = 0; i < numberOfMapMarkers; i++)
            {
                //mapMarkerStyles[i] = this._selenium.GetAttribute(string.Format("//div[@id='map_view']/div/div/div/div/img[{0}]@style", i + 1));
                //mapMarkerStyles[i] = this._selenium.GetAttribute(string.Format("//div[@id='map_view']/div/div/div/div[7]/img[{0}]@src", i + 1));
                mapMarkerStyles[i] = this._selenium.GetAttribute(string.Format("//div[@id='map_view']/div/div/div/div/img[{0}]@src", i + 1));

                TestLog.WriteLine("Map Markers Styles: " + mapMarkerStyles[i]);
            }

            // Now we need to parse out the width from this mess of a string that represents the style.  Store the widths in an array.  The width is always the first element (position 0).
            string[] mapMarkerWidths = new string[numberOfMapMarkers];

            for (int i = 0; i < numberOfMapMarkers; i++)
            {
                mapMarkerWidths[i] = mapMarkerStyles[i].Split(';').ElementAt(0);
                TestLog.WriteLine("Map Marker Widths: " + mapMarkerWidths[i]);

            }

            // Now we can figure out which marker to click. 
            // Group's at multiple locations have an image width of 27 px.  For each width, see if it is 27 px.  If it is, we have a multiple group marker.  Click it.
            //for (int j = 0; j < mapMarkerWidths.Length; j++) {
            for (int j = 0; j < mapMarkerStyles.Length; j++)
            {
                //if (mapMarkerWidths[j] == "width: 27px") {
                if (mapMarkerWidths[j].Contains("map_pin_multi.png"))
                {
                    TestLog.WriteLine("Map Pin Multi");
                    this._selenium.Click(string.Format("//div[@id='map_view']/div/div/div/div[9]/map[{0}]/area", j + 1));
                    // Figure out if the this is the right bubble pop up with our groups.
                    if (this._selenium.GetText("//div[@id='map_view']/div/div/div/div[10]/div/div/p[2]") == location)
                    {
                        // It is. Click the link to view the page that shows the multiple groups.
                        this._selenium.ClickAndWaitForPageToLoad("//div[@id='map_view']/div/div/div/div[10]/div/div/p[1]/strong/a");
                        break;
                    }
                    // It isn't. Close the bubble and search again
                    else
                    {
                        this._selenium.Click("//div[@id='map_view']/div/div/div/div[10]/div/span[@id='map_bubble_close']");
                    }
                }
            }
        }


        /// <summary>
        /// Views the groups that meet at the same location
        /// </summary>
        /// <param name="test"></param>
        /// <param name="locationAddress">The address for the location (i.e. 123 Main Street).</param>
        /// <param name="locationCityStateCip">The city state and zip for the location. (i.e. Flower Mound, TX 75022).  Note, it must follow that format.</param>
        public void Groups_FindAGroup_View_GroupsAtSameLocation_Old(string locationAddress, string locationCityStateCip) {

            TestLog.WriteLine("Click On Map View");
            // View the map
            this._selenium.Click(GeneralInFellowshipConstants.GeneralLinks.Link_MapView);

            Thread.Sleep(2000);

            // Build the string for the text, for the second paragraph tag, that is shown inside the bubble that shows when you click a marker in the map.  Also need to append the text View larger map.
            // Is a private location, meaning we passed in null for the address?
            string location = !string.IsNullOrEmpty(locationAddress) ? string.Concat(locationAddress, "\n", locationCityStateCip, "\nView larger map") : location = string.Concat(locationCityStateCip, "\nView larger map");

            TestLog.WriteLine("Location: " + location);
            // Get the number of map markers.  The 5th div element in this mess of div elements holds the image.
            //int numberOfMapMarkers = (Int32)this._selenium.GetXpathCount("//div[@id='map_view']/div/div/div/div[5]/img");
            // /html/body/div/div/div[3]/div/div/div[2]/div/div/div/div/div[2]/div[2]/div/div/div[5]/canvas
            // /html/body/div/div/div[3]/div/div/div[2]/div/div/div/div
            // /html/body/div/div/div[3]/div/div/div[2]/div/div/div[7]/div[2]/div/img
            // /html/body/div/div/div[3]/div/div/div[2]/div/div/div/div/div[2]/div[2]/div/div/div/canvas
            // /html/body/div/div/div[3]/div/div/div[2]/div/div/div/div/div[2]/div[2]/div/div/div[5]/canvas

            int mapBubbleBlue = (Int32)this._selenium.GetXpathCount("//div[@id='map_bubble']");
            int numberOfCanvas = (Int32)this._selenium.GetXpathCount("/html/body/div/div/div[3]/div/div/div[2]/div/div/div/div/div[2]/div[2]/div/div/div[5]/canvas");
            //int numberOfMapMarkers = (Int32)this._selenium.GetXpathCount("//div[@id='map_view']/div/div/div[7]/div[2]/div/img");
            int numberOfMapMarkers = (Int32)this._selenium.GetXpathCount("//div[@id='map_view']/div/div/div/div/img");
            TestLog.WriteLine("Map Bubble Blue: " + mapBubbleBlue);
            TestLog.WriteLine("Canvas Markers: " + numberOfCanvas);
            TestLog.WriteLine("Map Markers: " + numberOfMapMarkers);

            // For each of these markers, get the styles for each marker.  The seventh div element stores all the important style information.  A width of 27px means it is the marker that is flagged for multiple groups
            string[] mapMarkerStyles = new string[numberOfMapMarkers];
            for (int i = 0; i < numberOfMapMarkers; i++) {
                //mapMarkerStyles[i] = this._selenium.GetAttribute(string.Format("//div[@id='map_view']/div/div/div/div[7]/img[{0}]@style", i + 1));
                //mapMarkerStyles[i] = this._selenium.GetAttribute(string.Format("//div[@id='map_view']/div/div/div/div[7]/img[{0}]@src", i + 1));
                
                //FGJ
                this._selenium.GetAttribute(string.Format("//div[@id='map_canvas']/div/div/div/div[2]/div[2]/div/div/div[{0}]/canvas", i + 1));
                
                TestLog.WriteLine("Map Markers Styles: " + mapMarkerStyles[i]);
            }

            // Now we need to parse out the width from this mess of a string that represents the style.  Store the widths in an array.  The width is always the first element (position 0).
            string[] mapMarkerWidths = new string[numberOfMapMarkers];

            for (int i = 0; i < numberOfMapMarkers; i++) {
                mapMarkerWidths[i] = mapMarkerStyles[i].Split(';').ElementAt(0);
                TestLog.WriteLine("Map Marker Widths: " + mapMarkerWidths[i]);

            }

            // Now we can figure out which marker to click. 
            // Group's at multiple locations have an image width of 27 px.  For each width, see if it is 27 px.  If it is, we have a multiple group marker.  Click it.
            //for (int j = 0; j < mapMarkerWidths.Length; j++) {
            for (int j = 0; j < mapMarkerStyles.Length; j++) {
                //if (mapMarkerWidths[j] == "width: 27px") {
                if (mapMarkerWidths[j].Contains("map_pin_multi.png")) {
                    TestLog.WriteLine("Map Pin Multi");
                    this._selenium.Click(string.Format("//div[@id='map_view']/div/div/div/div[9]/map[{0}]/area", j + 1));
                    // Figure out if the this is the right bubble pop up with our groups.
                    if (this._selenium.GetText("//div[@id='map_view']/div/div/div/div[10]/div/div/p[2]") == location) {
                        // It is. Click the link to view the page that shows the multiple groups.
                        this._selenium.ClickAndWaitForPageToLoad("//div[@id='map_view']/div/div/div/div[10]/div/div/p[1]/strong/a");
                        break;
                    }
                    // It isn't. Close the bubble and search again
                    else {
                        this._selenium.Click("//div[@id='map_view']/div/div/div/div[10]/div/span[@id='map_bubble_close']");
                    }
                }
            }
        }

        public bool Groups_FindAGroup_GroupExistsInResults(string groupName) {
            // Is the table even present?
            if (!this._selenium.IsElementPresent(TableIds.InFellowship_FindAGroup_Results)) {
                // Nope.  Return false since the group isn't there.
                return false;
            }
            else {
                return this._generalMethods.ItemExistsInTable(TableIds.InFellowship_FindAGroup_Results, groupName, "Group");
            }
        }

        #endregion Find a Group

        #endregion Groups

        #region Events
        #endregion Events

        #region Event Registration

        /// <summary>
        /// Returns Infellowship Form ID URL.
        /// </summary>
        /// <param name="formName">Form Name</param>
        /// <param name="churchId">Church Id</param>
        /// <returns>Form URL</returns>
        public string Get_Infellowship_EventRegistration_Form_URL(string formName, int churchId)
        {
            //NOTE: Why this method is in here, I don't know. Should be in Infellowship Base.
            string fullLinkText = string.Empty;

            int formId = this._sql.Weblink_InfellowshipForm_GetFormId(churchId, formName);

            //Format Form Link test i.e. https://dc.qa.infellowship.com/Forms/
            switch (this._f1Environment)
            {
                case (F1Environments.INT):
                case (F1Environments.INT2):
                case (F1Environments.INT3):
                case (F1Environments.LV_QA):
                case (F1Environments.LV_UAT):
                case (F1Environments.STAGING):
                case (F1Environments.QA):
                    fullLinkText = string.Format("https://{0}.{1}.infellowship.com/Forms/{2}", this._sql.FetchChurchCode(churchId).ToLower(), PortalBase.GetLVEnvironment(this._f1Environment), formId);
                    break;
                default:
                    throw new SeleniumException(string.Format("Invalid Environment: {0}", this._f1Environment));
            }

            TestLog.WriteLine("Form Link: {0}", fullLinkText);

            return fullLinkText;

        }

        /// <summary>
        /// This Method selects and verifies selected individuals for Infellowship Registration forms
        /// </summary>
        /// <param name="individualName"></param>
        /// <param name="selectedOrder">The selected order should be set in the testcase and should match the household position order</param>
        /// <param name="isGuest">Is this individual a guest in the household?</param>
        /// <param name="clickContinue">Should the method click continue?</param>
        public void EventRegistration_Select_Individual(string[] individualName, IList<string> selectedOrder, bool isGuest = false, bool clickContinue = true)
        {
            if (isGuest)
            {
                this._driver.FindElementById(GeneralInFellowship.EventRegistration.SelectViewGuests).Click();
            }

            //Go through all individuals in list
            for (int ind = 0; ind < individualName.Length; ind++)
            {
                //Select individuals based on the list
                IList<IWebElement> listPeople = this._driver.FindElement(By.Id("available_people")).FindElements(By.TagName("li"));
                foreach (IWebElement person in listPeople)
                {
                    string individual = Regex.Split(person.Text, "\r\n")[0];
                    if (individual.Equals(individualName[ind]))
                    {
                        person.Click();
                    }

                }
                
            }

            //In Mobile view the selected order will not display so need to skip this validation
            var windowSize = this._driver.Manage().Window.Size;
            if (windowSize.Width != 640)
            {
                //Verify selected person is in the list. Not verifying that it's in the right order yet
              /*  IList<IWebElement> verifyListPeople = this._driver.FindElement(By.Id("selected_people")).FindElements(By.XPath("//li[@class='selected']/div/strong"));
                TestLog.WriteLine(string.Format("Count: {0}", verifyListPeople.Count));
                Assert.AreEqual(individualName.Count(), verifyListPeople.Count, "Selected People Do not match");
                int s = 0;
                TestLog.WriteLine(string.Format("selected order", selectedOrder[s]));
                foreach (IWebElement verifyPerson in verifyListPeople)
                {
                    //TestLog.WriteLine(string.Format("Verify Person: {0}", verifyPerson.Text));
                    //TestLog.WriteLine(string.Format("Verify Selected: {0}", selectedOrder[s]));
                    TestLog.WriteLine(string.Format("selected order", selectedOrder[s]));
                    Assert.AreEqual(selectedOrder[s], verifyPerson.Text, "Selected person is not in the correct order");
                    s++;
                } */

                this.EventRegistration_Individual_Selected_Order(individualName, selectedOrder);
            }


                       
            if (clickContinue)
            {

                    
                this.EventRegistration_ClickContinue();

            }
 
        }

        /// <summary>
        /// This Method selects given individual for Infellowship Registration forms
        /// </summary>
        /// <param name="individualName"></param>
        public void EventRegistration_Select_Individual(string individualName)
        {
            //Select individuals based on the list
            bool foundPerson = false;
            IList<IWebElement> listPeople = this._driver.FindElementById("available_people").FindElements(By.TagName("li"));
            foreach (IWebElement person in listPeople)
            {
                string individual = System.Text.RegularExpressions.Regex.Split(person.Text, "\r\n")[0];
                if (individual.Equals(individualName))
                {
                    foundPerson = true;
                    person.Click();
                    break;
                }

            }

            Assert.IsTrue(foundPerson, "Fail to find person with expected name");

        }

        /// <summary>
        /// This Method selects all individuals in current household for Infellowship Registration forms
        /// </summary>
        public void EventRegistration_Select_AllIndividuals()
        {
            //show all list.
            var btnExpandLink = this._driver.FindElement(By.Id("expand_link"));
            while (btnExpandLink.Displayed)
            {
                btnExpandLink.Click();
            }

            //Select individuals based on the list
            IList<IWebElement> listPeople = this._driver.FindElementById("available_people").FindElements(By.TagName("li"));
            foreach (IWebElement person in listPeople)
            {
                if (person.GetAttribute("id") != "expand_visitors" && person.GetAttribute("id") != "add_person")
                {
                    person.Click();
                }
            }

        }

        /// <summary>
        /// This method will verify selected people displaying in step1 of registration process
        /// </summary>
        /// <param name="individualName"></param>
        /// <param name="selectedOrder"></param>
        public void EventRegistration_Individual_Selected_Order(string[] individualName, IList<string> selectedOrder)
        {
            IList<IWebElement> verifyListPeople = this._driver.FindElement(By.Id("selected_people")).FindElements(By.XPath("//li[@class='selected']/div/strong"));
            TestLog.WriteLine(string.Format("Count: {0}", verifyListPeople.Count));
            Assert.AreEqual(individualName.Count(), verifyListPeople.Count, "Selected People Do not match");
            int s = 0;
            TestLog.WriteLine(string.Format("selected order", selectedOrder[s]));
            foreach (IWebElement verifyPerson in verifyListPeople)
            {
                //TestLog.WriteLine(string.Format("Verify Person: {0}", verifyPerson.Text));
                //TestLog.WriteLine(string.Format("Verify Selected: {0}", selectedOrder[s]));
                TestLog.WriteLine(string.Format("selected order", selectedOrder[s]));
                Assert.AreEqual(selectedOrder[s], verifyPerson.Text, "Selected person is not in the correct order");
                s++;
            }
        }

        /// <summary>
        /// This method will verify selected people displaying in step2 of registration process
        /// </summary>
        /// <param name="individualName"></param>
        /// <param name="selectedOrder"></param>
        public void EventRegistration_Individual_Selected_Order_step2(string[] individualName, IList<string> selectedOrder)
        {
            IList<IWebElement> verifyListPeople = this._driver.FindElement(By.Id("selected_people")).FindElements(By.XPath("//ul[@class='list']/li/div/strong"));
            TestLog.WriteLine(string.Format("Count: {0}", verifyListPeople.Count));
            Assert.AreEqual(individualName.Count(), verifyListPeople.Count, "Selected People Do not match");
            int s = 0;
            TestLog.WriteLine(string.Format("selected order", selectedOrder[s]));
            foreach (IWebElement verifyPerson in verifyListPeople)
            {
                //TestLog.WriteLine(string.Format("Verify Person: {0}", verifyPerson.Text));
                //TestLog.WriteLine(string.Format("Verify Selected: {0}", selectedOrder[s]));
                TestLog.WriteLine(string.Format("selected order", selectedOrder[s]));
                Assert.AreEqual(selectedOrder[s], verifyPerson.Text, "Selected person is not in the correct order");
                s++;
            }
        }

        /// <summary>
        /// This method will verify selected people, price for individuals and total value
        /// </summary>
        /// <param name="individualName"></param>
        /// <param name="selectedOrder"></param>
        /// <param name="formPrice"></param>
        public void EventRegistration_Individual_Selected_Order_Price(string[] individualName, IList<string> selectedOrder, double formPrice)
        {
            //IList<IWebElement> verifyListPeople = this._driver.FindElement(By.Id("selected_people")).FindElements(By.XPath("//li[@class='selected']/div/strong"));
            IList<IWebElement> verifyListPeople = this._driver.FindElement(By.Id("selected_people")).FindElements(By.XPath("//ul[@class='list']/li/div/strong"));
            IList<IWebElement> ListPeoplePrice = this._driver.FindElements(By.ClassName("price"));

            double total = 0;
            TestLog.WriteLine(string.Format("Count: {0}", verifyListPeople.Count));
            Assert.AreEqual(individualName.Count(), verifyListPeople.Count, "Selected People Do not match");
            int s = 0;
            TestLog.WriteLine(string.Format("selected order", selectedOrder[s]));

            //Add this check for the change f1-5664 - to not display zero amounts

            if (formPrice == 0.0)
            {
                Assert.AreEqual(Convert.ToString(ListPeoplePrice[s].Text), string.Empty, "Price is not displayed correctly in steps");
            }
            else
            {

                foreach (IWebElement verifyPerson in verifyListPeople)
                {

                    TestLog.WriteLine(string.Format("selected order", selectedOrder[s]));
                    Assert.AreEqual(selectedOrder[s], verifyPerson.Text, "Selected person is not in the correct order");
                    Assert.AreEqual(Convert.ToString(ListPeoplePrice[s].Text), string.Format("{0:C2}", formPrice), "Price is not displayed correctly in steps");
                    string itemPrice1 = ListPeoplePrice[s].Text;
                    var itemPrice2 = Double.Parse(itemPrice1.Substring(1));
                    total = total + itemPrice2;
                    TestLog.WriteLine("Item Price {0}", itemPrice2);
                    TestLog.WriteLine("Total : {0}", total);
                    s++;

                }



                Assert.AreEqual(this._driver.FindElementById("total_price").Text, string.Format("{0:C2}", total), "Total Price not matched in steps");
            }
        }

        /// <summary>
        /// This method will calculate total value for selected people in Step2
        /// </summary>
        /// <param name="individualName"></param>
        /// <param name="selectedOrder"></param>
        /// <param name="formPrice"></param>
        public void EventRegistration_Individual_Selected_Order_Price_Step2(string[] individualName, IList<string> selectedOrder, double formPrice)
        {
            //IList<IWebElement> verifyListPeople = this._driver.FindElement(By.Id("selected_people")).FindElements(By.XPath("//li[@class='selected']/div/strong"));
          //  IList<IWebElement> verifyListPeople = this._driver.FindElement(By.XPath("//ul[@id = 'selected_people']")).FindElements(By.XPath("//div[@class='name']/strong"));
            IList<IWebElement> verifyListPeople = this._driver.FindElement(By.Id("selected_people")).FindElements(By.XPath("//ul[@class='list']/li/div/strong"));
            IList<IWebElement> ListPeoplePrice = this._driver.FindElements(By.ClassName("price"));

            double total = 0;
            TestLog.WriteLine(string.Format("Count: {0}", verifyListPeople.Count));
            Assert.AreEqual(individualName.Count(), verifyListPeople.Count, "Selected People Do not match");
            int s = 0;
            TestLog.WriteLine(string.Format("selected order", selectedOrder[s]));

            //Add this check for the change f1-5664 - to not display zero amounts

            if (formPrice == 0.0)
            {
                Assert.AreEqual(Convert.ToString(ListPeoplePrice[s].Text), string.Empty, "Price is not displayed correctly in steps");
            }
            else
            {

                foreach (IWebElement verifyPerson in verifyListPeople)
                {

                    TestLog.WriteLine(string.Format("selected order", selectedOrder[s]));
                    Assert.AreEqual(selectedOrder[s], verifyPerson.Text, "Selected person is not in the correct order");
                    Assert.AreEqual(Convert.ToString(ListPeoplePrice[s].Text), string.Format("{0:C2}", formPrice), "Price is not displayed correctly in steps");
                    string itemPrice1 = ListPeoplePrice[s].Text;
                    var itemPrice2 = Double.Parse(itemPrice1.Substring(1));
                    total = total + itemPrice2;
                    TestLog.WriteLine("Item Price {0}", itemPrice2);
                    TestLog.WriteLine("Total : {0}", total);
                    s++;

                }



                Assert.AreEqual(this._driver.FindElementById("total_price").Text, string.Format("{0:C2}", total), "Total Price not matched in steps");
            }
        }


        public void EventRegistration_Individual_Selected_Order_Price_Upsell_Step2(string[] individualName, IList<string> selectedOrder, double formPrice)
        {
            //IList<IWebElement> verifyListPeople = this._driver.FindElement(By.Id("selected_people")).FindElements(By.XPath("//li[@class='selected']/div/strong"));
            //  IList<IWebElement> verifyListPeople = this._driver.FindElement(By.XPath("//ul[@id = 'selected_people']")).FindElements(By.XPath("//div[@class='name']/strong"));
            IList<IWebElement> verifyListPeople = this._driver.FindElement(By.Id("selected_people")).FindElements(By.XPath("//ul[@class='list']/li/div/strong"));
            IList<IWebElement> ListPeoplePrice = this._driver.FindElements(By.ClassName("price"));

            double total = 0;
            TestLog.WriteLine(string.Format("Count: {0}", verifyListPeople.Count));
            Assert.AreEqual(individualName.Count(), verifyListPeople.Count, "Selected People Do not match");
            int s = 0;
            TestLog.WriteLine(string.Format("selected order", selectedOrder[s]));

            //Add this check for the change f1-5664 - to not display zero amounts

            if (formPrice == 0.0)
            {
                Assert.AreEqual(Convert.ToString(ListPeoplePrice[s].Text), string.Empty, "Price is not displayed correctly in steps");
            }
            else
            {

                foreach (IWebElement verifyPerson in verifyListPeople)
                {

                    TestLog.WriteLine(string.Format("selected order", selectedOrder[s]));
                    Assert.AreEqual(selectedOrder[s], verifyPerson.Text, "Selected person is not in the correct order");
                    Assert.AreEqual(Convert.ToString(ListPeoplePrice[s+1].Text), string.Format("{0:C2}", formPrice), "Price is not displayed correctly in steps");
                    string itemPrice1 = ListPeoplePrice[s+1].Text;
                    var itemPrice2 = Double.Parse(itemPrice1.Substring(1));
                    total = total + itemPrice2;
                    TestLog.WriteLine("Item Price {0}", itemPrice2);
                    TestLog.WriteLine("Total : {0}", total);
                    s++;

                }

                 //add by Grace Zhang
                 String ActuralValue =this._driver.FindElementByCssSelector("[class='indent'][data-role='record']").FindElement(By.CssSelector("[class='price']")).Text.ToString();
                 Assert.AreEqual(ActuralValue, string.Format("{0:C2}", total), "Total Price not matched in steps");
            }
        }

        /// <summary>
        /// This method will verify selectd people, price and total in step3
        /// </summary>
        /// <param name="individualName"></param>
        /// <param name="selectedOrder"></param>
        /// <param name="formPrice"></param>
        public void EventRegistration_Individual_Selected_Order_Price_Step3(string[] individualName, IList<string> selectedOrder, double formPrice)
        {

            IList<IWebElement> verifyListPeople = this._driver.FindElements(By.XPath("//table[@id='order_summary']/tbody/tr[*]/td[1]"));
            IList<IWebElement> ListPeoplePrice = this._driver.FindElements(By.XPath("//table[@id='order_summary']/tbody/tr[*]/td[2]"));

            double total = 0;
            TestLog.WriteLine(string.Format("Count: {0}", verifyListPeople.Count));
            Assert.AreEqual(individualName.Count(), verifyListPeople.Count, "Selected People Do not match");
            int s = 0;
            TestLog.WriteLine(string.Format("selected order", selectedOrder[s]));

            //Add this check for the change f1-5664 - to not display zero amounts
            if (formPrice == 0.0)
            {
                Assert.AreEqual(Convert.ToString(ListPeoplePrice[s].Text), string.Empty, "Price is not displayed correctly in steps");
            }
            else
            {
                foreach (IWebElement verifyPerson in verifyListPeople)
                {
                    TestLog.WriteLine(string.Format("selected order", selectedOrder[s]));
                    Assert.AreEqual(selectedOrder[s], verifyPerson.Text, "Selected person is not in the correct order");
                    Assert.AreEqual(Convert.ToString(ListPeoplePrice[s].Text), string.Format("{0:C2}", formPrice), "Price is not displayed correctly in steps");

                    string itemPrice1 = ListPeoplePrice[s].Text;
                    var itemPrice2 = Double.Parse(itemPrice1.Substring(1));
                    total = total + itemPrice2;
                    TestLog.WriteLine("Item Price {0}", itemPrice2);
                    TestLog.WriteLine("Total : {0}", total);
                    s++;
                }

                Assert.AreEqual(this._driver.FindElementByXPath("//table[@id='order_summary']/tfoot/tr[*]/td[2]").Text, string.Format("{0:C2}", total), "Total Price not matched in steps");
            }

        }
        /// <summary>
        /// This method will verify selectd people, price and total in step3 with Upsell value
        /// </summary>
        /// <param name="individualName"></param>
        /// <param name="selectedOrder"></param>
        /// <param name="formPrice"></param>
        public void EventRegistration_Individual_Selected_Order_Price_Step3_Upsell(string[] individualName, IList<string> selectedOrder, double formPrice, string[]answerChoice, double answerChoicePrice)
        {

            /**** commenting this logic until figure out a way  xpaths works for both individuals and upsell items **********
            //IList<IWebElement> verifyListPeople = this._driver.FindElement(By.Id("selected_people")).FindElements(By.XPath("//li[@class='selected']/div/strong"));
            IList<IWebElement> verifyListPeople = this._driver.FindElements(By.XPath("//table[@id='order_summary']/tbody/tr[*[@class='group_top']]/td[1]"));

          //  IList<IWebElement> verifyListPeople = this._driver.FindElement(By.Id("order_summary")).FindElements(By.XPath("tbody/tr/tr[*]/[@class = 'group_top']/td[1]"));
           // IList<IWebElement> ListPeoplePrice = this._driver.FindElements(By.XPath("//table[@id='order_summary']/tbody/tr[*]/td[2]"));
            IList<IWebElement> listPeoplePrice = this._driver.FindElements(By.XPath("//table[@id='order_summary']/tbody/tr[*[@class='group_top']]/td[2]"));
           // IList<IWebElement> listPeoplePrice = this._driver.FindElement(By.Id("order_summary")).FindElements(By.XPath("tbody/tr/tr[*]/[@class = 'group_top']/td[2]"));



           // IList<IWebElement> verifyupsellItem = this._driver.FindElement(By.Id("order_summary")).FindElements(By.XPath("tbody/tr/tr[*][@class = 'group_bottom']/td[1]"));
           //

            IList<IWebElement> verifyupsellItem = this._driver.FindElements(By.XPath("//table[@id='order_summary']/tbody/tr[*[@class='group_bottom']]/td[1]"));
            IList<IWebElement> verifyupsellPrice = this._driver.FindElements(By.XPath("//table[@id='order_summary']/tbody/tr[*[@class='group_bottom']]/td[2]"));

            //Verifying selected individual list and Price
            double totalIndividuals = 0;
            TestLog.WriteLine(string.Format("Count ind: {0}", verifyListPeople.Count));
//            Assert.AreEqual(individualName.Count(), verifyListPeople.Count, "Selected People Do not match");
            int s = 0;           
            TestLog.WriteLine(string.Format("selected order", selectedOrder[s]));
            foreach (IWebElement verifyPerson in verifyListPeople)
            {
                
                TestLog.WriteLine(string.Format("selected order", selectedOrder[s]));
                Assert.AreEqual(selectedOrder[s], verifyPerson.Text, "Selected person is not in the correct order");
                Assert.AreEqual(Convert.ToString(listPeoplePrice[s].Text), string.Format("{0:C2}", formPrice), "Price is not displayed correctly in steps");
                string itemPrice1 = listPeoplePrice[s].Text;
                var itemPrice2 = Double.Parse(itemPrice1.Substring(1));
                totalIndividuals = totalIndividuals + itemPrice2;
                TestLog.WriteLine("Item Price {0}", itemPrice2);
                TestLog.WriteLine("Total item : {0}", totalIndividuals);
                s++;
              
            }

            //Verifying selected upsell items and price value

            double totalupsell = 0;
            TestLog.WriteLine(string.Format("Count upsell: {0}", verifyupsellItem.Count));
            //            Assert.AreEqual(individualName.Count(), verifyListPeople.Count, "Selected People Do not match");
            int u = 0;
            TestLog.WriteLine(string.Format("selected order", selectedOrder[u]));
            foreach (IWebElement verifyItem in verifyupsellItem)
            {

                TestLog.WriteLine(string.Format("selected order", selectedOrder[u]));
                string itemPrice1 = verifyupsellPrice[u].Text;
                var itemPrice2 = Double.Parse(itemPrice1.Substring(1));
                totalupsell = totalupsell + itemPrice2;
                TestLog.WriteLine("Item up sell Price {0}", itemPrice2);
                TestLog.WriteLine("Total upsell: {0}", totalupsell);
                u++;

            }

            Assert.AreEqual(this._driver.FindElementByXPath("//table[@id='order_summary']/tfoot/tr[*]/td[2]").Text, string.Format("{0:C2}", totalIndividuals + totalupsell), "Total Price not matched in steps");
        
             
            **********************************************************/

            //Different Logic from here........

            //To get all rows in table

            IList<IWebElement> tableRows = this._driver.FindElements(By.XPath("//table[@id='order_summary']/tbody/tr[*]"));

            TestLog.WriteLine(string.Format("Count rows: {0}", tableRows.Count));


            //To verify individuals and form price value in the list

            double totalIndividual = 0.0;
            int s = 1;
           
            for(int i=1;i<tableRows.Count;i++)
            {
                foreach(string person in selectedOrder)
                {
                   
                    IList<IWebElement> cells = tableRows[i].FindElements(By.TagName("td"));                
  
                 
                   
                        if (cells[0].Text == person)
                        {
                            Assert.AreEqual(Convert.ToString(cells[1].Text), string.Format("{0:C2}", formPrice), "Price is not displayed correctly in steps");
                            string itemPrice1 = cells[1].Text;
                            var itemPrice2 = Double.Parse(itemPrice1.Substring(1));
                            totalIndividual = totalIndividual + itemPrice2;
                            TestLog.WriteLine("Item Price {0}", itemPrice2);
                            TestLog.WriteLine("Total item : {0}", totalIndividual);
                            s++;
                        }
                    
                   

                }

            }

            //Verifying upsell answers and Price value in table

            double totalUpsell = 0.0;
            int u = 1;
            for(int i=1;i<tableRows.Count;i++)
            {
          
                foreach (string answer in answerChoice)
                {
                    IList<IWebElement> cells = tableRows[i].FindElements(By.TagName("td"));

                    if (cells[0].Text == answer) 
                    {
                                           
                        Assert.AreEqual(Convert.ToString(cells[1].Text), string.Format("{0:C2}", answerChoicePrice), "Price is not displayed correctly in steps");
                        string itemPrice1 = cells[1].Text;
                        
                        var itemPrice2 = Double.Parse(itemPrice1.Substring(1));
                        totalUpsell = totalUpsell + itemPrice2;
                        TestLog.WriteLine("Item Price {0}", itemPrice2);
                        TestLog.WriteLine("Total item : {0}", totalUpsell);
                        u++;
                    }

                }

            }
            //Total amount display....
            TestLog.WriteLine("Total Price: {0}", totalIndividual + totalUpsell);
            // Modified by Mady, total amound xpath is changing frequently, so replace it with data-role value rather than table id path
            // Assert.AreEqual(this._driver.FindElementByXPath("//*[@id='order_summary']/tfoot/tr[2]/td[2]/span").Text, string.Format("{0:N2}", totalIndividual + totalUpsell), "Total Price not matched in steps");
            Assert.AreEqual(this._driver.FindElementByXPath(".//*[@data-role='amount-due']/span").Text, string.Format("{0:N2}", totalIndividual + totalUpsell), "Total Price not matched in steps");
        
        }


        /// <summary>
        /// This method will verify summary details in Step 3 
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="individual"></param>
        /// <param name="selectedOrder"></param>
        /// <param name="price"></param>
        /// <param name="sendEmail"></param>
        /// <param name="emailCC"></param>
        public void EventRegistration_Step3_Summary(string formName, string[] individual, IList<string> selectedOrder, double price, string[] sendEmail, string[] emailCC)
        {
            //Verify Finalize Registration text present
            this._generalMethods.VerifyTextPresentWebDriver("Finalize Registration");

            //Verify Form Name
            Assert.AreEqual(formName, this._driver.FindElementByXPath("//table[@id='order_summary']/tbody/tr/th").Text);


            //Verifying selected individual list and price value
            this.EventRegistration_Individual_Selected_Order_Price_Step3(individual, selectedOrder, price);

            //selecting Email send and CC text boxes         

            for (int i = 0; i < sendEmail.Length; i++)
            {
                this._driver.FindElementByName("confirmation_email").Clear();
                this._driver.FindElementByName("confirmation_email").SendKeys(sendEmail[i]);
                // this._driver.FindElementByName("confirmation_email").SendKeys(",");

            }

            for (int j = 0; j < emailCC.Length; j++)
            {
                this._driver.FindElementByName("cc_email").Clear();
                this._driver.FindElementByName("cc_email").SendKeys(emailCC[j]);
              //  this._driver.FindElementByName("cc_email").SendKeys(",");
            }
           
            
        }

        /// <summary>
        /// This method will verify summary details in Step 3 with Upsell value
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="individual"></param>
        /// <param name="selectedOrder"></param>
        /// <param name="price"></param>
        /// <param name="sendEmail"></param>
        /// <param name="emailCC"></param>
        public void EventRegistration_Step3_Summary_Upsell(string formName, string[] individual, IList<string> selectedOrder, double price, string[] sendEmail, string[] emailCC, string[] answerChoice, double answerChoicePrice)
        {
            //Verify Finalize Registration text present
            this._generalMethods.VerifyTextPresentWebDriver("Finalize Registration");

            //Verify Form Name
            Assert.AreEqual(formName, this._driver.FindElementByXPath("//table[@id='order_summary']/tbody/tr/th").Text);


            //Verifying selected individual list and price value
            
            this.EventRegistration_Individual_Selected_Order_Price_Step3_Upsell(individual, selectedOrder, price, answerChoice, answerChoicePrice);

            //selecting Email send and CC text boxes         

            for (int i = 0; i < sendEmail.Length; i++)
            {
                this._driver.FindElementByName("confirmation_email").Clear();
                this._driver.FindElementByName("confirmation_email").SendKeys(sendEmail[i]);
                // this._driver.FindElementByName("confirmation_email").SendKeys(",");

            }

            for (int j = 0; j < emailCC.Length; j++)
            {
                this._driver.FindElementByName("cc_email").Clear();
                this._driver.FindElementByName("cc_email").SendKeys(emailCC[j]);
              //  this._driver.FindElementByName("cc_email").SendKeys(",");
            }
           
            
        }

        /// <summary>
        /// This method will process Credit card payment transaction information in infellowship Event registration 
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="creditCardType"></param>
        /// <param name="creditCardNumber"></param>
        /// <param name="expirationMonth"></param>
        /// <param name="expirationYear"></param>
        /// <param name="securityCode"></param>
        public void EventRegistration_CC_Process(int churchId, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, [Optional] string securityCode)
        {

            //AMS Status for the Church
            Boolean amsEnabled = this._sql.IsAMSEnabled(churchId);

            TestLog.WriteLine(string.Format("AMS Church ID {0} Enabled: {1}", churchId, amsEnabled));

            // Store the currency format
            CultureInfo culture = null;
            if (churchId == 258)
            {
                culture = new CultureInfo("en-GB");
            }
            else
            {
                culture = new CultureInfo("en-US");
            }

            //Select Payment type CC
            this._driver.FindElementById(GeneralInFellowship.EventRegistration.CC_Payment_Method).Click();

            //Credit card processing information
            //Cardholder First Name
            this._driver.FindElement(By.Id(GeneralInFellowship.EventRegistration.CC_FirstName)).SendKeys(firstName);
            //Cardholder Last Name
            this._driver.FindElement(By.Id(GeneralInFellowship.EventRegistration.CC_LastName)).SendKeys(lastName);
            //Card type
            this._driver.FindElementById(GeneralInFellowship.EventRegistration.CC_Type).SendKeys(creditCardType);

            //CC Number
            this._driver.FindElementById(GeneralInFellowship.EventRegistration.CC_Number).SendKeys(creditCardNumber);
            //Expiration month
            this._driver.FindElementById(GeneralInFellowship.EventRegistration.CC_Expiration_Month).SendKeys(expirationMonth);
            //Expiration year
            this._driver.FindElementById(GeneralInFellowship.EventRegistration.CC_Expiration_Year).SendKeys(expirationYear);


            //Billing Address information            
           
            new SelectElement(this._driver.FindElementById("Country")).SelectByText("United States");          
            this._driver.FindElementById("address1").Clear();
            this._driver.FindElementById("address1").SendKeys("6363 N State Hwy 161");         
            this._driver.FindElementById("address2").Clear();
            this._driver.FindElementById("address2").SendKeys("Suite 200");          
            this._driver.FindElementById("city").Clear();
            this._driver.FindElementById("city").SendKeys("Irving");
            this._driver.FindElementById("state").SendKeys(string.Empty);
            this._driver.FindElementById("state").SendKeys("Texas");
            this._driver.FindElementById("PostalCode").Clear();
            this._driver.FindElementById("PostalCode").SendKeys("75038");                         
                                               
        }
        /// <summary>
        /// This method process credit card details for european church
        /// </summary>
        /// <param name="churchId"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="creditCardType"></param>
        /// <param name="creditCardNumber"></param>
        /// <param name="expirationMonth"></param>
        /// <param name="expirationYear"></param>
        /// <param name="securityCode"></param>

        public void EventRegistration_CC_Process_European(int churchId, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, [Optional] string securityCode)
        {

            //AMS Status for the Church
            Boolean amsEnabled = this._sql.IsAMSEnabled(churchId);

            TestLog.WriteLine(string.Format("AMS Church ID {0} Enabled: {1}", churchId, amsEnabled));

            // Store the currency format
            CultureInfo culture = null;
            if (churchId == 258)
            {
                culture = new CultureInfo("en-GB");
            }
            else
            {
                culture = new CultureInfo("en-US");
            }

            //Select Payment type CC
            this._driver.FindElementById(GeneralInFellowship.EventRegistration.CC_Payment_Method).Click();

            //Credit card processing information
            //Cardholder First Name
            this._driver.FindElement(By.Id(GeneralInFellowship.EventRegistration.CC_FirstName)).SendKeys(firstName);
            //Cardholder Last Name
            this._driver.FindElement(By.Id(GeneralInFellowship.EventRegistration.CC_LastName)).SendKeys(lastName);
            //Card type
            this._driver.FindElementById(GeneralInFellowship.EventRegistration.CC_Type).SendKeys(creditCardType);

            //CC Number
            this._driver.FindElementById(GeneralInFellowship.EventRegistration.CC_Number).SendKeys(creditCardNumber);
            //Expiration month
            this._driver.FindElementById(GeneralInFellowship.EventRegistration.CC_Expiration_Month).SendKeys(expirationMonth);
            //Expiration year
            this._driver.FindElementById(GeneralInFellowship.EventRegistration.CC_Expiration_Year).SendKeys(expirationYear);


            //Billing Address information            

          /*  new SelectElement(this._driver.FindElementById("Country")).SelectByText("United Kingdom");
            this._driver.FindElementById("address1").Clear();
            this._driver.FindElementById("address1").SendKeys("63 St Leonards Road");
            this._driver.FindElementById("address2").Clear();
            this._driver.FindElementById("address2").SendKeys(" ");
            this._driver.FindElementById("city").Clear();
            this._driver.FindElementById("city").SendKeys("Windsor");
            this._driver.FindElementById("state").SendKeys(string.Empty);
            this._driver.FindElementById("state").SendKeys("Berkshire");
            this._driver.FindElementById("PostalCode").Clear();
            this._driver.FindElementById("PostalCode").SendKeys("SL43BX"); */

        }


        /// <summary>
        /// This method will process E-check payment transaction information in infellowship Event registration 
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="routingNumber"></param>
        /// <param name="accountNumber"></param>
        public void EventRegistration_Echeck_Process(string firstName, string lastName, string phoneNumber, string routingNumber, string accountNumber)
        {
            //Select Payment type E-check
            this._driver.FindElementById(GeneralInFellowship.EventRegistration.Echeck_Payment_Method).Click();

            //First Name
            this._driver.FindElementById(GeneralInFellowship.EventRegistration.Echeck_FirstName).SendKeys(firstName);

            //Last Name
            this._driver.FindElementById(GeneralInFellowship.EventRegistration.Echeck_LastName).SendKeys(lastName);

            //Phone Number
            this._driver.FindElementById(GeneralInFellowship.EventRegistration.Echeck_PhoneNumber).SendKeys(phoneNumber);


            //Routing Number
            this._driver.FindElementById(GeneralInFellowship.EventRegistration.BankRouting_Number_Name).SendKeys(routingNumber);

            //Account Number
            this._driver.FindElementById(GeneralInFellowship.EventRegistration.Echeck_AccountNumber).SendKeys(accountNumber);




        }

        public void EventRegistration_Add_Individual(string firstName, string lastName, string dateOfBirth, string maritalStatus, string gender, string householdType, bool createSuccess = true)
        {
            //Click Add individual button
            this._driver.FindElementByXPath(GeneralInFellowship.EventRegistration.SelectAddPersonButton).Click();
            this._generalMethods.WaitForElement(By.Id(GeneralInFellowship.EventRegistration.AddModalFirstName));

            //Fill out fields
            if (firstName != null)
            {
            this._driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalFirstName).SendKeys(firstName);
            }
            if (lastName != null)
            {
            this._driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalLastName).SendKeys(lastName);
            }
            if (dateOfBirth != null)
            {
                this._driver.FindElementByName(GeneralInFellowship.EventRegistration.AddModalDOB).Clear();
                this._driver.FindElementByName(GeneralInFellowship.EventRegistration.AddModalDOB).SendKeys(dateOfBirth);
            }
            //new feature, there is no maritalstatus element. so, comment below script. modified by ivan.zhang
            //if (maritalStatus != null)
            //{
            //    new SelectElement(this._driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalMaritalStatus)).SelectByText(maritalStatus);
            //}
            if (gender != null)
            {
                new SelectElement(this._driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalGender)).SelectByText(gender);
            }
            if (householdType != null)
            {
                new SelectElement(this._driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalHouseholdPosition)).SelectByText(householdType);
            }
            if (createSuccess)
            {
                //Click add
                this._generalMethods.WaitForElementEnabled(By.XPath(GeneralInFellowship.EventRegistration.ModalSave));
                this._driver.FindElementByXPath(GeneralInFellowship.EventRegistration.ModalSave).Click();
            }
            else
            {
                this._driver.FindElementByLinkText("Cancel");
            }
            this._generalMethods.WaitForElement(By.XPath(GeneralInFellowship.EventRegistration.SelectAddPersonButton));
        }
        /// <summary>
        /// Edit individual on the Select individual page in infellowship registration form. 
        /// </summary>
        /// <param name="individualName"></param>
        /// <param name="birthDate"></param>
        /// <param name="gender"></param>
        /// <param name="isGuest"></param>
        public void EventRegistration_Edit_Individual(string individualName, string birthDate = null, string gender = null,  bool isGuest = false)
        {
            if (isGuest)
            {
                this._driver.FindElementById(GeneralInFellowship.EventRegistration.SelectViewGuests).Click();
            }
            //Select individual to edit
            IList<IWebElement> listPeople = this._driver.FindElement(By.Id("available_people")).FindElements(By.TagName("li"));
            foreach (IWebElement person in listPeople)
            {
                string individual = Regex.Split(person.Text, "\r\n")[0];
                if (individual.Equals(individualName))
                {
                    person.Click();
                    this._generalMethods.VerifyElementDisplayedWebDriver(By.Id("edit_modal"));
                    if (birthDate != null)
                    {
                        this._driver.FindElementByName("dob").SendKeys("1");
                        this._driver.FindElementByName("dob").Clear();
                        this._driver.FindElementByName("dob").SendKeys(birthDate);
                    }
                    if (gender != null)
                    {
                        new SelectElement(this._driver.FindElementById("gender")).SelectByText(gender);
                    }
                    this._generalMethods.WaitForElementEnabled(By.XPath(GeneralInFellowship.EventRegistration.ModalSave));
                    this._driver.FindElementByXPath(GeneralInFellowship.EventRegistration.ModalSave).Click();
                }
            }

            
            
        }
        /// <summary>
        /// Got this method from Carney
        /// Courtesy of an article by D. Patrick Caldwell on Software Engineering entitled ".Net Method to Combine Hash Codes."
        /// http://dpatrickcaldwell.blogspot.com/2011/08/net-method-to-combine-hash-codes.html?m=1
        /// </summary>


        /// <summary>
        /// This method will select Answer for Question                              
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="elementId"></param>
        public void EventRegistration_TextBox_Answer(string answer)
        {

            //   TestLog.WriteLine("Id {0}", elementId);


            if (!string.IsNullOrEmpty(answer))
            {
                //  this._driver.FindElementByXPath("//*[@id='formQuestions']/div/div[2]/div/div[1]/label").SendKeys(answer);

                /* IJavaScriptExecutor executor = (IJavaScriptExecutor)this._driver;

                 string answerId = "document.getElementByXPath('//*[@id='formQuestions']/div/div[2]/div/div[1]/label').value(answer)";

                 ((IJavaScriptExecutor)executor).ExecuteScript(answerId); */

                //This code is to get form individual ID and question id and using these get Hashcode value
                string attributeValue = this._driver.FindElement(By.ClassName("form-control")).GetAttribute("data-forminfo");



              /*  TestLog.WriteLine("attributeValue {0}", attributeValue);
                string[] attributeValues = attributeValue.Split('_');
                string form_Individual_Id = attributeValues[0];

                TestLog.WriteLine("Att-1 {0}", attributeValues[0]);
                TestLog.WriteLine("Att-2 {0}", attributeValues[1]);

                int elementId = HashCodeHelper.CombineHashCodes(Convert.ToInt32(attributeValues[0]), Convert.ToInt32(attributeValues[1])); */

                int elementId = this.GetHashcodeId(attributeValue);
                

                TestLog.WriteLine(" Element Id : {0}", elementId);

                //this._driver.FindElement(By.Id(Convert.ToString(elementId))).SendKeys(answer);
                this._driver.FindElement(By.XPath("//input[@class='form-control' and @type='text']")).SendKeys(answer);

                //Click Continue
                this.EventRegistration_ClickContinue();
            }
           
        }

        /// <summary>
        /// This method will select Answer for Drop down Question                              
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="elementId"></param>
        public void EventRegistration_Dropdown_Answer_Select(string[] answerChoiceDisplay)
        {                      
            for(int i=0;i<answerChoiceDisplay.Length;i++)
            {
            if (!string.IsNullOrEmpty(answerChoiceDisplay[i]))
                {

                    //updated by ivan.zhang
                    string DropdownList_ID = this._driver.FindElement(By.ClassName("form-control")).GetAttribute("id");
                    TestLog.WriteLine(" DropdownList_ID : {0}", DropdownList_ID);
                    new SelectElement(this._driver.FindElement(By.Id(DropdownList_ID))).SelectByText(answerChoiceDisplay[i]);
                    

                    /* commented by ivan.zhang
                    string attributeValue = this._driver.FindElement(By.ClassName("form-control")).GetAttribute("data-forminfo");                                             
                    int elementId = this.GetHashcodeId(attributeValue);
                    TestLog.WriteLine(" Element Id : {0}", elementId);

                    //Select Answer Choice
                    new SelectElement(this._driver.FindElement(By.Id(Convert.ToString(elementId)))).SelectByText(answerChoiceDisplay[i]);
                    */
                
                }
            }
        }

        /// <summary>
        /// This method is used to generate Hashcode
        /// </summary>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public int GetHashcodeId(string attributeValue)
        {
            TestLog.WriteLine("attributeValue {0}", attributeValue);
            string[] attributeValues = attributeValue.Split('_');
            string form_Individual_Id = attributeValues[0];

            TestLog.WriteLine("Att-1 {0}", attributeValues[0]);
            TestLog.WriteLine("Att-2 {0}", attributeValues[1]);

            int elementId = HashCodeHelper.CombineHashCodes(Convert.ToInt32(attributeValues[0]), Convert.ToInt32(attributeValues[1]));

            return elementId;

        }

        /// <summary>
        /// This method is pass answer to shared question
        /// </summary>
        /// <param name="answer"></param>
        public void EventRegistration_Shared_TextBox_Answer(string[] answer)
        {

        
                IList<IWebElement> sharedQuestions =   this._driver.FindElementsByXPath("//input[@class='form-control shared-question']");
                int i=0;

                foreach (IWebElement question in sharedQuestions)               
                {
                    question.SendKeys(string.Format("{0}", answer[i]));
                    i++;

                   // this._driver.FindElementByClassName("form-control shared-question").SendKeys(answer);
                }
      
        }

        public void EventRegistration_Shared_Dropdown_Answer(string answer)
        {

            if (!string.IsNullOrEmpty(answer))
            {
                this._driver.FindElementByXPath("//select[@class='form-control shared-question']").SendKeys(answer); 

               // new SelectElement(this._driver.FindElementByXPath("//select[@class='form-control shared-question']")).SelectByValue(answer);
                // this._driver.FindElementByClassName("form-control shared-question").SendKeys(answer);
            }

        }

        /// <summary>
        /// This method is verify shared answer for selected individuals
        /// </summary>
        /// <param name="answer"></param>
        public void EventRegistration_Shared_Textbox_Answer_Verification(string answer)
        {
            IJavaScriptExecutor executor = (IJavaScriptExecutor)this._driver;

            if (!string.IsNullOrEmpty(answer))
            {
                IList<IWebElement> answer_individual = this._driver.FindElementsByXPath("//input[@class='form-control']");
                string[] answers = new string[answer_individual.Count];
                               
//                IList<IWebElement> shared_Header_expand = this._driver.FindElementsByClassName("uiToggle uiToggleExpanded");

                for (int i = 0; i < answer_individual.Count; i++)
                {
                    //Click on the Header to expand
                   // shared_Header_expand[i].Click();

                    TestLog.WriteLine("Veifying answer for individual {0}", i);
                   // Assert.AreEqual(answer,this._javascript.TextboxValue(answers[i], answer));

                    // this._javascript.OptionExistsInSelect(answers[i], answer);

                    //Trying to verify 
                    String script = "document.getElementByXpath('{0}',answer_individual['i']).value";
                    Assert.AreEqual(executor.ExecuteScript(script), answer);

                   
                }
            }

        }

        public Boolean EventRegistration_Shared_Textbox_Answer_Verification_Javascript()
        {
            IJavaScriptExecutor executor = (IJavaScriptExecutor)this._driver;

            var script = @"var sharedAnswers = $(':input.shared-question');
                return _.every(sharedAnswers, function(sharedAnswer) {
                var sharedAnswerValue = $(sharedAnswer).val();
                var sharedAnswerFormDefinition = $(sharedAnswer).attr('data-form-definition-id');
                console.log(sharedAnswerFormDefinition);
                var individualAnswers = $(':input[data-form-definition-id=' + sharedAnswerFormDefinition + ']');
                console.log(individualAnswers);
                return _.every(individualAnswers, function(individualAnswer) {
                return sharedAnswerValue === $(individualAnswer).val();
                });
                });";


            //to execute script
            return(Convert.ToBoolean(executor.ExecuteScript(script)));

           // return(executor.ExecuteScript(script));



        }
        /// <summary>
        /// 
        /// This method is used to click on Continue button in registration process
        /// </summary>
        /// modified it for we can select the instance or schedule to continue. ivan.zhang.
        public void EventRegistration_ClickContinue()
        {
            var windowSize = this._driver.Manage().Window.Size;
            if (windowSize.Width != 640)
            {
                IList<IWebElement> continueButton = this._driver.FindElementsById(GeneralInFellowship.EventRegistration.ContinueButton);
                bool elementFlag = false;
                IWebElement obj_instance = null;

                for (int i = 0; i < continueButton.Count; i++)
                {
                    #region select instance
                    try
                    {
                        //this._generalMethods.WaitForElement(this._driver, By.Id(GeneralInFellowship.EventRegistration.Instance));
                        obj_instance = this._driver.FindElementById(GeneralInFellowship.EventRegistration.Instance);
                        //determine the instance is displayed really, updated by ivan.zhang
                        elementFlag = obj_instance.Displayed;
                    }
                    catch (NoSuchElementException e)
                    {
                        elementFlag = false;
                    }
                    if (elementFlag)
                    {
                        obj_instance.Click();
                        obj_instance.SendKeys(Keys.Down);
                        obj_instance.SendKeys(Keys.Enter);
                    }
                    #endregion

                    if (continueButton[i].Displayed)
                        continueButton[i].Click();
                }
            }
            else
            {
                this._driver.FindElementById(GeneralInFellowship.EventRegistration.ResponsiveContinueButton).Click();
            }
        }

        /// <summary>
        /// This method is used to click on Submit Payment from Step3
        /// </summary>
        public void EventRegistration_ClickSubmitPayment()
        {

            var windowSize = this._driver.Manage().Window.Size;
            if (windowSize.Width == 640)
            {
                IList<IWebElement> submitPayment = this._driver.FindElementsById(GeneralInFellowship.EventRegistration.ResponsiveSubmitPayment);

                for (int i = 0; i < submitPayment.Count; i++)
                {
                    if (submitPayment[i].Displayed)
                        submitPayment[i].Click();

                }


            }

            else
                this._driver.FindElementById(GeneralInFellowship.EventRegistration.SubmitPayment).Click();

        }


        #endregion Event Registration

        #endregion Instance Methods

        #region Private Methods
        /// <summary>
        /// Generic Login method for InFellowship
        /// </summary>
        /// <param name="username">InFellowship User Name</param>
        /// <param name="password">InFellowship Password</param>
        /// <param name="churchCode">InFellowship Church Code</param>
        private void DoLoginInFellowship(string username, string password, string churchCode) {

            // Set the username
            if (!string.IsNullOrEmpty(username)) {
                this._infellowshipEmail = username;
            }

            // Set the password
            if (!string.IsNullOrEmpty(password)) {
                this._infellowshipPassword = password;
            }

            // Set the church code
            if (!string.IsNullOrEmpty(churchCode)) {
                this._infellowshipChurchCode = churchCode;
            }

            TestLog.WriteLine("Infellowship Login: " + username + "/" + password + "/" + churchCode);            

            // Open the web page
            if (string.IsNullOrEmpty(churchCode)) {
                TestLog.WriteLine("Open DC Church");
                this.Open("DC");
            }
            else {
                TestLog.WriteLine("Open " + churchCode + " Church");
                this.Open(churchCode);
            }


            //Are we in login page
            TestLog.WriteLine("Username Present: " + this._selenium.IsElementPresent("username"));
            Assert.IsTrue(this._selenium.IsElementPresent("username"));
            TestLog.WriteLine("Password Present: " + this._selenium.IsElementPresent("password"));
            Assert.IsTrue(this._selenium.IsElementPresent("password"));

            // Type the username            
            if (!string.IsNullOrEmpty(username)) {
                this._selenium.Type("username", username);
            }
            else {
                this._selenium.Type("username", this._infellowshipEmail);
            }

            // Type the password
            if (!string.IsNullOrEmpty(password)) {
                this._selenium.Type("password", password);
            }
            else {
                this._selenium.Type("password", this._infellowshipPassword);
            }

            // Login, wait for the page to load
            this._selenium.ClickAndWaitForPageToLoad("btn_login");

            this._infellowshipChurchCode = churchCode;
            this._infellowshipChurchID = this._sql.FetchChurchID(this._infellowshipChurchCode);

        }

        /// <summary>
        ///  Gets the InFellowship URL.  We cannot just set the URL because the church code is part of the URL and may vary.
        /// </summary>
        /// <returns></returns>
        public string GetInFellowshipURL() {
            string returnValue = string.Empty;

            switch (this._f1Environment) {
                /*case F1Environments.DEV1:
                case F1Environments.DEV2:
                case F1Environments.DEV3:
                    returnValue = string.Format("http://{0}.infellowship.{1}.dev.corp.local", this._infellowshipChurchCode, this._f1Environment);
                    break;
                case F1Environments.INTEGRATION:
                    returnValue = string.Format("http://{0}.infellowship.integration.corp.local", this._infellowshipChurchCode);
                    break;
                case F1Environments.DEV:                
                    returnValue = string.Format("http://{0}.infellowship{1}.dev.corp.local", this._infellowshipChurchCode, this._f1Environment);
                    break; */

                case F1Environments.STAGING:
                    //updated by ivan, add '/' here.
                    returnValue = string.Format("https://{0}.staging.infellowship.com/", this._infellowshipChurchCode);
                    break;
                case F1Environments.QA:
                case F1Environments.LV_QA:
                case F1Environments.LV_UAT:
                case F1Environments.INT:
                case F1Environments.INT2:
                case F1Environments.INT3:
                case F1Environments.LV_PROD:
                case F1Environments.PRODUCTION:
                    //https://dc.qa.f1infellowship.dev.activenetwork.com/UserLogin/Index?ReturnUrl=%2f
                    //returnValue = string.Format("https://{0}.{1}.f1infellowship.dev.activenetwork.com", this._infellowshipChurchCode, this._f1Environment);
                    returnValue = string.Format("https://{0}.{1}.infellowship.com/", this._infellowshipChurchCode, PortalBase.GetLVEnvironment(this._f1Environment));
                    break;
                default:
                    throw new System.Exception("Not a valid option!");
            }
            return returnValue;
        }

        #region People
        /// <summary>
        /// Fills out the additional information to complete the account creation process.
        /// </summary>
        /// <param name="dateOfBirth">Your date of birth.</param>
        /// <param name="gender">Your gender.</param>
        /// <param name="country">Your current country.</param>
        /// <param name="addressOne">Street 1.</param>
        /// <param name="addressTwo">Street 2.</param>
        /// <param name="city">Your current city.</param>
        /// <param name="state">Your current state</param>
        /// <param name="postalCode">Your current postal code.</param>
        /// <param name="county">Your current county.</param>
        /// <param name="homePhone">Your current home phone.</param>
        /// <param name="mobilePhone">Your current mobile phone.</param>
        private void Account_Confirm(string dateOfBirth, GeneralEnumerations.Gender gender, string country, string addressOne, string addressTwo, string city, string state, string postalCode, string county, string homePhone, string mobilePhone) {
            // Specify all the information
            this._selenium.Type("dob", dateOfBirth);

            switch (gender) {
                case GeneralEnumerations.Gender.Male:
                    this._selenium.Click("//input[@name='gender' and @value='Male']");
                    break;
                case GeneralEnumerations.Gender.Female:
                    this._selenium.Click("//input[@name='gender' and @value='Female']");
                    break;
                default:
                    break;
            }

            this._selenium.Type("street1", addressOne);
            this._selenium.Type("street2", addressTwo);
            this._selenium.Type("city", city);
            this._selenium.Select("state", state);
            this._selenium.Type("postal_code", postalCode);
            this._selenium.Type("county", county);

            if (!string.IsNullOrEmpty(homePhone)) {
                this._selenium.Type("home_phone", homePhone);
            }

            if (!string.IsNullOrEmpty(mobilePhone)) {
                this._selenium.Type("mobile_phone", mobilePhone);
            }
        }

        /// <summary>
        /// Fills out the additional information to complete the account creation process. This also specifies a preferred number.
        /// </summary>
        /// <param name="dateOfBirth">Your date of birth.</param>
        /// <param name="gender">Your gender.</param>
        /// <param name="country">Your current country.</param>
        /// <param name="addressOne">Street 1.</param>
        /// <param name="addressTwo">Street 2.</param>
        /// <param name="city">Your current city.</param>
        /// <param name="state">Your current state</param>
        /// <param name="postalCode">Your current postal code.</param>
        /// <param name="county">Your current county.</param>
        /// <param name="homePhone">Your current home phone.</param>
        /// <param name="mobilePhone">Your current mobile phone.</param>
        /// <param name="preferredPhone">The communication type you wish to make preferred.</param>
        private void Account_Confirm(string dateOfBirth, GeneralEnumerations.Gender gender, string country, string addressOne, string addressTwo, string city, string state, string postalCode, string county, string homePhone, string mobilePhone, GeneralEnumerations.CommunicationTypes preferredPhone) {
            // Specify all the information
            this._selenium.Type("dob", dateOfBirth);

            switch (gender) {
                case GeneralEnumerations.Gender.Male:
                    this._selenium.Click("//input[@name='gender' and @value='Male']");
                    break;
                case GeneralEnumerations.Gender.Female:
                    this._selenium.Click("//input[@name='gender' and @value='Female']");
                    break;
                default:
                    break;
            }

            this._selenium.Type("street1", addressOne);
            this._selenium.Type("street2", addressTwo);
            this._selenium.Type("city", city);
            this._selenium.Select("state", state);
            this._selenium.Type("postal_code", postalCode);
            this._selenium.Type("county", county);

            if (!string.IsNullOrEmpty(homePhone)) {
                this._selenium.Type("home_phone", homePhone);
            }

            if (!string.IsNullOrEmpty(mobilePhone)) {
                this._selenium.Type("mobile_phone", mobilePhone);
            }

            switch (preferredPhone) {
                case GeneralEnumerations.CommunicationTypes.Home:
                    this._selenium.Click("//a[@class='select_primary' and text()='home']");
                    break;
                case GeneralEnumerations.CommunicationTypes.Mobile:
                    this._selenium.Click("//a[@class='select_primary' and text()='mobile']");
                    break;
                default:
                    throw new SeleniumException("Invalid communication type for preferred phone.");
            }

        }

        /// <summary>
        /// Attemps to set the privacy setting for a value.
        /// </summary>
        /// <param name="privacySettingType">The setting you wish to set a privacy level for (Address, birthdate, etc).</param>
        /// <param name="desiredLevel">The desired privacy level.</param>
        public void People_PrivacySettings_SetPrivacyLevel(GeneralEnumerations.PrivacySettingTypes privacySettingType, GeneralEnumerations.PrivacyLevels desiredLevel) {

            // Privacy settings have 4 levels for the slider control. Each tick has a certain percentage set at the style attribute.
            // 0% is Church Staff, 33% is Leaders, 66% is members, 100% is everyone.  These four values will be used to calculate how to move the slider.

            // The target privacy percentage is what the new privacy setting will be set at.
            var targetPrivacyPercentage = 0;

            // The adjustment value is what will be used to move the slider.  It is calculated by subtracting the target value from the current value.
            double adjustmentValue;

            // First, set the target privacy percentage based on the highest role for the privacy setting.
            switch (desiredLevel) {
                case GeneralEnumerations.PrivacyLevels.ChurchStaff:
                    targetPrivacyPercentage = 0;
                    break;
                case GeneralEnumerations.PrivacyLevels.Leaders:
                    targetPrivacyPercentage = 33;
                    break;
                case GeneralEnumerations.PrivacyLevels.Members:
                    targetPrivacyPercentage = 66;
                    break;
                case GeneralEnumerations.PrivacyLevels.Everyone:
                    targetPrivacyPercentage = 100;
                    break;
                default:
                    break;
            }

            // Figure out the adjustment value
            adjustmentValue = PrivacySettings_CalculateAdjustmentValue(targetPrivacyPercentage, privacySettingType);

            // Set the slider position based on the adjustment value.  If negative is returned, we are moving backwards. Otherwise, we are moving forward.  If 0 is returned,
            // we aren't moving anywhere.
            switch (privacySettingType) {
                case GeneralEnumerations.PrivacySettingTypes.Address:
                    this._selenium.DragAndDrop(InFellowshipConstants.People.PrivacySettingsConstants.Slider_Address, string.Format("{0},0", adjustmentValue.ToString()));
                    break;
                case GeneralEnumerations.PrivacySettingTypes.Birthdate:
                    this._selenium.DragAndDrop(InFellowshipConstants.People.PrivacySettingsConstants.Slider_Birthdate, string.Format("{0},0", adjustmentValue.ToString()));
                    break;
                case GeneralEnumerations.PrivacySettingTypes.Email:
                    this._selenium.DragAndDrop(InFellowshipConstants.People.PrivacySettingsConstants.Slider_Email, string.Format("{0},0", adjustmentValue.ToString()));
                    break;
                case GeneralEnumerations.PrivacySettingTypes.Phone:
                    this._selenium.DragAndDrop(InFellowshipConstants.People.PrivacySettingsConstants.Slider_Phone, string.Format("{0},0", adjustmentValue.ToString()));
                    break;
                case GeneralEnumerations.PrivacySettingTypes.Websites:
                    this._selenium.DragAndDrop(InFellowshipConstants.People.PrivacySettingsConstants.Slider_Websites, string.Format("{0},0", adjustmentValue.ToString()));
                    break;
                case GeneralEnumerations.PrivacySettingTypes.Social:
                    this._selenium.DragAndDrop(InFellowshipConstants.People.PrivacySettingsConstants.Slider_Social, string.Format("{0},0", adjustmentValue.ToString()));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Calculates the adjustment value.
        /// </summary>
        /// <param name="targetPrivacyPercentage">The target privacy percentage.</param>
        /// <param name="privacySettingType">The privacy setting type that will gain the new privacy setting value.</param>
        /// <returns></returns>
        private double PrivacySettings_CalculateAdjustmentValue(double targetPrivacyPercentage, GeneralEnumerations.PrivacySettingTypes privacySettingType) {

            // Based on the target privacy percentage, we can calculate how much we need to adjust the slider.

            // The current percentage of the slider control based on the GetAttribute method.  This will return in the format of left: xxx%;
            string currentPrivacySettingPercentage = null;

            // Based on the the current privacy setting percentage style value, we can map this to an numerical value.
            var currentPrivacyPercentage = 0.0;

            // The adjustment value
            var adjustmentValue = 0.0;

            // The difference between the current position and the target position of the slider.
            var differenceBetweenCurrentAndTargetPercentage = 0.0;

            // A multiplier used for moving between large gaps in privacy (I.e from Church staff only to everyone).
            var multiplier = 0;

            // Based on the data getting a new privacy setting, figure out the percentage based on the style attribute.
            switch (privacySettingType) {
                case GeneralEnumerations.PrivacySettingTypes.Address:
                    currentPrivacySettingPercentage = this._selenium.GetAttribute(InFellowshipConstants.People.PrivacySettingsConstants.Slider_Address + "@style");
                    break;
                case GeneralEnumerations.PrivacySettingTypes.Birthdate:
                    currentPrivacySettingPercentage = this._selenium.GetAttribute(InFellowshipConstants.People.PrivacySettingsConstants.Slider_Birthdate + "@style");
                    break;
                case GeneralEnumerations.PrivacySettingTypes.Email:
                    currentPrivacySettingPercentage = this._selenium.GetAttribute(InFellowshipConstants.People.PrivacySettingsConstants.Slider_Email + "@style");
                    break;
                case GeneralEnumerations.PrivacySettingTypes.Phone:
                    currentPrivacySettingPercentage = this._selenium.GetAttribute(InFellowshipConstants.People.PrivacySettingsConstants.Slider_Phone + "@style");
                    break;
                case GeneralEnumerations.PrivacySettingTypes.Websites:
                    currentPrivacySettingPercentage = this._selenium.GetAttribute(InFellowshipConstants.People.PrivacySettingsConstants.Slider_Websites + "@style");
                    break;
                case GeneralEnumerations.PrivacySettingTypes.Social:
                    currentPrivacySettingPercentage = this._selenium.GetAttribute(InFellowshipConstants.People.PrivacySettingsConstants.Slider_Social + "@style");
                    break;
                default:
                    currentPrivacySettingPercentage = "left: 0%;";
                    break;
            }


            // Map this percentage to a numerical value.
            switch (currentPrivacySettingPercentage) {
                case "left: 0%;":
                    currentPrivacyPercentage = 0;
                    break;
                case "left: 33.3333%;":
                    currentPrivacyPercentage = 33;
                    break;
                case "left: 66.6667%;":
                    currentPrivacyPercentage = 66;
                    break;
                case "left: 100%;":
                    currentPrivacyPercentage = 100;
                    break;
                default:
                    break;
            }

            // Calculate the difference between current privacy value and the target privacy value.  We use absolute values to ignore negatives.
            differenceBetweenCurrentAndTargetPercentage = Math.Abs((currentPrivacyPercentage - targetPrivacyPercentage));

            // We also need to add an arbitrary amount of pixels to help adjust the silder.  Since we are multiplying the target privacy percentage, this pixel value might have to adjust. 
            // The distance between each tick is about 196 pixels.
            int arbitraryAmountOfPixels = 200;

            // Figure out the mulitplier.  Note, the values assigned are arbitrary and offer the most accurate results of moving the slider.
            // If it is 33% , that is only moving one position left or right, set the mulitplier to 1.
            if (differenceBetweenCurrentAndTargetPercentage == 33 || differenceBetweenCurrentAndTargetPercentage == 34) {
                multiplier = 1;
                // The amount of pixels we will be traveling will be 33 * 1 + 200 pixels, which equates to 233. That is enough to move the slider one tick.
            }
            // If it is 66% , that is only moving two positions left or right, set the mulitplier to 2.
            else if (differenceBetweenCurrentAndTargetPercentage == 66 || differenceBetweenCurrentAndTargetPercentage == 67) {
                multiplier = 2;
                arbitraryAmountOfPixels = 260; // When moving two places over, the amount of pixels is about 390 pixels. 2 * 66 + 200 will move the slider about 332 pixels which isn't enough. Change 200 pixels to about 260 pixels. This should bump the slider enough.
            }
            // If it is 100% , that is only all the way to the left or all the way to the right, set the mulitplier to 3..
            else if (differenceBetweenCurrentAndTargetPercentage == 100) {
                multiplier = 3;
                // When moving from one end to the other, the amount of pixels is about 588 pixels. 100 * 3 + 200 pixels will move the slider about 533 pixels, which is well over the amount it takes to move two places over (390 pixels). As a result, the slider should move enough ticks.
            }

            // Figure out if we are moving left or right.
            // If the current equals the target, we aren't moving anywhere.
            if (currentPrivacyPercentage == targetPrivacyPercentage)
                adjustmentValue = 0.0;
            // If the current is greater than the target, we are moving backwards. Multiply the target percentage and the multiplier, add 250 pixels (arbitrary)
            // and multipy it all -1.  This sets the direction backwards
            else if (currentPrivacyPercentage > targetPrivacyPercentage) {
                if (targetPrivacyPercentage == 0)
                    targetPrivacyPercentage = 100;
                adjustmentValue = (targetPrivacyPercentage * multiplier + arbitraryAmountOfPixels) * -1;
            }
            // If the current is less than the target, we are moving forward. Multiply the target percentage and the multiplier, add 200 pixels (arbitrary)
            else if (currentPrivacyPercentage < targetPrivacyPercentage) {
                if (targetPrivacyPercentage == 0)
                    targetPrivacyPercentage = 100;
                adjustmentValue = targetPrivacyPercentage * multiplier + arbitraryAmountOfPixels;
            }
            // Can't figure it out.  Throw an exception.
            else
                throw new System.Exception("Cannot calculate adjustment value for privacy setting.");

            // Return the adjustment value
            return adjustmentValue;
        }

        #endregion People

        #region Giving

        #region Give Now
        /// <summary>
        /// Steps through the Give Now wizard to process a Credit Card payment.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="contributionData">A collection of funds, subfunds, and amounts to give to.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="creditCardType">The credit card type.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <param name="securityCode">The security code.</param>
        /// <param name="validCreditCard">Specifies if this credit card is valid or not.</param>
        /// <param name="country">The country</param>
        /// <param name="streetOne">The street one address.</param>
        /// <param name="streetTwo">The street two address.</param>
        /// <param name="city">The city.</param>
        /// <param name="state">The state.</param>
        /// <param name="postalCode">The postal code.</param>
        /// <param name="county">The county.</param>
        private void Giving_GiveNow_CreditCard_Process(int churchId, IEnumerable<dynamic> contributionData, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, bool validCreditCard, string country, string streetOne, string streetTwo, string city, string state, string postalCode, string county, string validFromMonth, string validFromYear) {

            // Store the payment type id and credit card name.  This is used for queries and validation.
            var paymentTypeId = 0;
            var creditCardName = creditCardType;
            Boolean amsEnabled = false;

            switch (creditCardType) {
                case "Visa":
                    paymentTypeId = 1;
                    break;
                case "Master Card":
                    paymentTypeId = 2;
                    creditCardName = "Mastercard";
                    break;
                case "American Express":
                    paymentTypeId = 3;
                    creditCardName = "AMEX";
                    break;
                case "Discover":
                    paymentTypeId = 4;
                    break;
                case "Diners Club":
                    paymentTypeId = 5;
                    break;
                case "Carte Blanche":
                    paymentTypeId = 6;
                    break;
                case "JCB":
                    paymentTypeId = 7;
                    break;
                case "Optima":
                    paymentTypeId = 8;
                    break;
                case "EnRoute":
                    paymentTypeId = 9;
                    break;
                case "Delta":
                    paymentTypeId = 20;
                    break;
                //case "Switch":
                //    paymentTypeId = 11;
                //    break;
                //case "Solo":
                //    paymentTypeId = 13;
                //    break;
                default:
                    break;
            }

            // What is our AMS Status, default is always false
            amsEnabled = this._sql.IsAMSEnabled(churchId);

            TestLog.WriteLine(string.Format("AMS Church ID {0} Enabled: {1}", churchId, amsEnabled));

            // Store the currency format
            CultureInfo culture = null;
            if (churchId == 258) {
                culture = new CultureInfo("en-GB");
            }
            else {
                culture = new CultureInfo("en-US");
            }

            // Store the last four digits of the credit card
            var lastFourDigits = string.Empty;
            if (!string.IsNullOrEmpty(creditCardNumber)) {
                lastFourDigits = creditCardNumber.Substring(creditCardNumber.Length - 4);
            }

            // View Give Now 
            this._selenium.ClickAndWaitForPageToLoad("link=Your Giving");
            this._selenium.ClickAndWaitForPageToLoad("link=Give Now");

            #region Step 1

            // Populate step 1
            this.Giving_GiveNow_Populate_Step1(churchId, "Credit Card", contributionData, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, country, streetOne, streetTwo, city, state, postalCode, county, null, null, null, validFromMonth, validFromYear);

            // Store the Billing address information for verification purposes on Step 2
            var streetOneText = string.IsNullOrEmpty(streetOne) ? this._selenium.GetValue("Address1") : streetOne;
            var streetTwoText = string.IsNullOrEmpty(streetTwo) ? this._selenium.GetValue("Address2") : streetTwo;
            var cityText = string.IsNullOrEmpty(city) ? this._selenium.GetValue("City") : city;
            var stateText = string.IsNullOrEmpty(state) ? this._selenium.GetSelectedValue("state") : state;
            // If international, the get the province
            if (churchId == 258) {
                stateText = string.IsNullOrEmpty(city) ? this._selenium.GetValue("StProvince") : city;
            }
            var postCodeText = string.IsNullOrEmpty(postalCode) ? this._selenium.GetValue("PostalCode") : postalCode;
            var countryName = string.IsNullOrEmpty(country) ? this._selenium.GetSelectedLabel("Country") : country;


            // Get the selected month's numerical value.
            // Store the month number
            var monthNumber = string.Empty;
            if (!string.IsNullOrEmpty(expirationMonth)) {
                // If the month number is a single digit, prepend a 0.
                var selectedMonth = this._selenium.GetValue("expiration_month");
                monthNumber = selectedMonth.Length > 1 ? selectedMonth : "0" + selectedMonth;
            }

            // Store the total contribution
            var total = this._selenium.GetText("total_sum");

            #endregion Step 1

            // Submit 
            this._selenium.ClickAndWaitForPageToLoad("//button[@class='btn btn-primary btn-lg next hidden-xs']");


            // Unless Step 1 validation is present, continue to Step 2
            if (!this._selenium.IsElementPresent("//div[@class='error_msgs_for']")) {

                // Verify all data populated on step 1 is present

                // Contribution Details Section
                // Since there are multiple funds/subfunds and amounts that can be used, loop through the collection.
                for (int i = 0; i < contributionData.Count<dynamic>(); i++) {
                    // There is no subfund so the > character is not present.
                    if (string.IsNullOrEmpty(contributionData.ElementAt<dynamic>(i).subFund)) {
                        Assert.AreEqual(contributionData.ElementAt<dynamic>(i).fund, this._selenium.GetText("//table[@class='grid']/tbody/tr[" + (i + 1) + "]/td[1]"));
                    }
                    else {
                        // There is a subfund
                        Assert.AreEqual(contributionData.ElementAt<dynamic>(i).FundOrPledgeDrive + " > " + contributionData.ElementAt<dynamic>(i).subFund, this._selenium.GetText("//table[@class='grid']/tbody/tr[" + (i + 1) + "]/td[1]"));
                    }

                    Assert.AreEqual(string.Format(culture, "{0:c}", Convert.ToDecimal(contributionData.ElementAt<dynamic>(i).amount)), this._selenium.GetText("//table[@class='grid']/tbody/tr[" + (i + 1) + "]/td[2]"));
                }

                // Is the total correct?
                Assert.AreEqual("Total", this._selenium.GetText("//table[@class='grid']/tfoot/tr[1]/td[1]"));
                Assert.AreEqual(string.Format(culture, "{0:c}", Convert.ToDecimal(total)), this._selenium.GetText("//table[@class='grid']/tfoot/tr[1]/td[2]"));

                // Payment Information Section
                Assert.AreEqual("Payment Information", this._selenium.GetText("//span[@class='icon_text icon_payment']"));
                Assert.AreEqual(string.Format("{0} \n {1} ending in {2} \n Expires {3}/{4}", firstName + " " + lastName, creditCardName, lastFourDigits, monthNumber, expirationYear), this._selenium.GetText("//table[@class='grid']/tbody/tr[preceding-sibling::tr/th/span[@class='icon_text icon_payment']]/td[1]"));

                // Street two is not required and results in a different rendering if it is provided.
                if (string.IsNullOrEmpty(streetTwoText)) {
                    Assert.AreEqual(string.Format("{0} \n {1}, {2} {3} \n {4}", streetOneText, cityText, stateText, postCodeText, countryName), this._selenium.GetText("//table[@class='grid']/tbody/tr[preceding-sibling::tr/th/span[@class='icon_text icon_payment']]/td[2]"));
                }

                else {
                    Assert.AreEqual(string.Format("{0} \n {1} \n {2}, {3} {4} \n {5}", streetOneText, streetTwoText, cityText, stateText, postCodeText, countryName), this._selenium.GetText("//table[@class='grid']/tbody/tr[preceding-sibling::tr/th/span[@class='icon_text icon_payment']]/td[2]"));
                }

                // Submit to attempt to attempt to process this payment. Store the time it takes to submit.
                DateTime paymentTime = this._selenium.ClickAndWaitForPageToLoad("//button[@class='btn btn-primary btn-lg next hidden-xs']").AddMinutes(-5);

                // Unless Step 2 validation occurs, verify the payment exists on the giving history tab
                if (!this._selenium.IsElementPresent("//div[@class='error_msgs_for']")) {

                    
                    // If the payment is pending, verify it is pending and wait for it to be processed
                    var paymentStatusID = this._sql.Giving_GetPaymentStatusID(churchId, Convert.ToDouble(total), paymentTime, "Infellowship Give Now", paymentTypeId);
                    if (paymentStatusID == 3) {
 
                        // Verify Giving History before the payment is processed
                        foreach (var item in contributionData) {
                            // Validate the Giving History Page
                            this.Giving_GivingHistory_Validate(churchId, firstName, lastName, item.fund, item.subFund, item.amount, paymentTime, paymentTypeId, "Credit Card", false);
                        }

                        // Wait until the payment has processed
                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, Convert.ToDouble(total), paymentTime, "Infellowship Give Now", paymentTypeId);


                    }



                    // View Give Now 
                    this._selenium.ClickAndWaitForPageToLoad("link=Giving History");

                    // Verify Giving History after the payment is processed
                    foreach (var item in contributionData) {
                        // Validate the Giving History Page
                        this.Giving_GivingHistory_Validate(churchId, firstName, lastName, item.fund, item.subFund, item.amount, paymentTime, paymentTypeId, "Credit Card", true);
                    }
                }
                #region Step 2 Validation
                // Validation has occured on Step 2 of the Give Now wizard. See if the messages are correct along with the payment reason codes.
                else {

                    var validationText1 = "Authorization was not successful. Please check your information and try again. An error has occurred while attempting to process the request.";
                    var validationText2 = "Authorization was not successful. Please use a different card or select another form of payment.";

                    // Verify the validation messages and reason codes
                    // 150 Reason Code
                    if (contributionData.Any(contribution => contribution.Amount == "2000")) {

                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 2000, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to 233                                                                                 
                            Assert.AreEqual(233, this._sql.Giving_GetPaymentReasonCode(churchId, 2000, paymentTime, "Infellowship Give Now"), "AMS reason code was not 233.");
                            this._selenium.VerifyTextPresent(this.Get_AMS_Error_Text("233"));
                        }
                        else
                        {
                            this._selenium.VerifyTextPresent(validationText1);
                            // Verify the reason code is 150
                            Assert.AreEqual(150, this._sql.Giving_GetPaymentReasonCode(churchId, 2000, paymentTime, "Infellowship Give Now"), "Reason code was not 150.");
                        }
                    }

                    // 201 Reason Code
                    else if (contributionData.Any(contribution => contribution.Amount == "2402")) {

                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 2402, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to 233
                            Assert.AreEqual(233, this._sql.Giving_GetPaymentReasonCode(churchId, 2000, paymentTime, "Infellowship Give Now"), "AMS reason code was not 233.");
                            this._selenium.VerifyTextPresent(this.Get_AMS_Error_Text("233"));
                        }
                        else
                        {
                            // Verify the reason code is 201
                            Assert.AreEqual(201, this._sql.Giving_GetPaymentReasonCode(churchId, 2402, paymentTime, "Infellowship Give Now"), "Reason code was not 201.");
                            this._selenium.VerifyTextPresent(validationText2);
                        }
                    }

                    // 203 Reason Code
                    else if (contributionData.Any(contribution => contribution.Amount == "2260")) {
                        
                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 2260, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to 231                                                                                 
                            Assert.AreEqual(231, this._sql.Giving_GetPaymentReasonCode(churchId, 2260, paymentTime, "Infellowship Give Now"), "AMS reason code was not 231.");
                            this._selenium.VerifyTextPresent(this.Get_AMS_Error_Text("231"));
                        }
                        else
                        {
                            // Verify the reason code is 203
                            Assert.AreEqual(203, this._sql.Giving_GetPaymentReasonCode(churchId, 2260, paymentTime, "Infellowship Give Now"), "Reason code was not 203.");
                            this._selenium.VerifyTextPresent(validationText2);
                        }

                    }

                    // 204 Reason Code
                    else if (contributionData.Any(contribution => contribution.Amount == "2521")) {
                        
                        //Legacy to AMS error code mapping stays the same.

                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 2521, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to 204                                                                                 
                            Assert.AreEqual(204, this._sql.Giving_GetPaymentReasonCode(churchId, 2521, paymentTime, "Infellowship Give Now"), "AMS reason code was not 204.");
                            this._selenium.VerifyTextPresent(this.Get_AMS_Error_Text("204"));
                        }
                        else
                        {
                            // Verify the reason code is 204
                            Assert.AreEqual(204, this._sql.Giving_GetPaymentReasonCode(churchId, 2521, paymentTime, "Infellowship Give Now"), "Reason code was not 204.");
                            this._selenium.VerifyTextPresent("Authorization was not successful. Insufficient funds in the account. Please use a different card or select another form of payment.");
                        }
                    }

                    // 205 Reason Code
                    else if (contributionData.Any(contribution => contribution.Amount == "2501")) {
                       

                       
                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 2501, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to 231                                                                                 
                            Assert.AreEqual(231, this._sql.Giving_GetPaymentReasonCode(churchId, 2501, paymentTime, "Infellowship Give Now"), "AMS reason code was not 231.");
                            this._selenium.VerifyTextPresent(this.Get_AMS_Error_Text("231"));
                        }
                        else
                        {
                            // Verify the reason code is 205
                            Assert.AreEqual(205, this._sql.Giving_GetPaymentReasonCode(churchId, 2501, paymentTime, "Infellowship Give Now"), "Reason code was not 205.");
                            this._selenium.VerifyTextPresent(validationText2);
                        }
                    }

                    // 208 Reason Code
                    else if (contributionData.Any(contribution => contribution.Amount == "2606")) {
                        

                        
                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 2606, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to 231                                                                                 
                            Assert.AreEqual(231, this._sql.Giving_GetPaymentReasonCode(churchId, 2606, paymentTime, "Infellowship Give Now"), "AMS reason code was not 231.");
                            this._selenium.VerifyTextPresent(this.Get_AMS_Error_Text("231"));
                        }
                        else
                        {
                            // Verify the reason code is 208
                            Assert.AreEqual(208, this._sql.Giving_GetPaymentReasonCode(churchId, 2606, paymentTime, "Infellowship Give Now"), "Reason code was not 208.");
                            this._selenium.VerifyTextPresent(validationText2);
                        }
                    }

                    // 209 Reason Code
                    else if (contributionData.Any(contribution => contribution.Amount == "2811")) {
                        

                        
                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 2811, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to 231                                                                                 
                            Assert.AreEqual(231, this._sql.Giving_GetPaymentReasonCode(churchId, 2811, paymentTime, "Infellowship Give Now"), "AMS reason code was not 231.");
                            //*[@id="content"]/div[2]/div/div[1]/ul/li
                            //TestLog.WriteLine(this._selenium.GetText("//*[@id='content']/div[2]/div/div[1]/ul/li"));
                            this._selenium.VerifyTextPresent(this.Get_AMS_Error_Text("231"));
                        }
                        else
                        {
                            // Verify the reason code is 209
                            Assert.AreEqual(209, this._sql.Giving_GetPaymentReasonCode(churchId, 2811, paymentTime, "Infellowship Give Now"), "Reason code was not 209.");
                            this._selenium.VerifyTextPresent(validationText1);
                        }
                    }

                    // 211 Reason Code
                    else if (contributionData.Any(contribution => contribution.Amount == "2531")) {
                        

                        
                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 2531, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to 231                                                                                 
                            Assert.AreEqual(231, this._sql.Giving_GetPaymentReasonCode(churchId, 2531, paymentTime, "Infellowship Give Now"), "AMS reason code was not 231.");
                            this._selenium.VerifyTextPresent(this.Get_AMS_Error_Text("231"));
                        }
                        else
                        {
                            // Verify the reason code is 211
                            Assert.AreEqual(211, this._sql.Giving_GetPaymentReasonCode(churchId, 2531, paymentTime, "Infellowship Give Now"), "Reason code was not 211.");
                            this._selenium.VerifyTextPresent(validationText1);
                        }
                    }

                        


                    // 233 Reason Code
                    else if (contributionData.Any(contribution => contribution.Amount == "2204")) {


                        //Legacy to AMS error code mapping stays the same.
                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 2204, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to 233                                                                                 
                            Assert.AreEqual(233, this._sql.Giving_GetPaymentReasonCode(churchId, 2204, paymentTime, "Infellowship Give Now"), "AMS reason code was not 233.");
                            this._selenium.VerifyTextPresent(this.Get_AMS_Error_Text("233"));
                        }
                        else
                        {
                            // Verify the reason code is 233
                            Assert.AreEqual(233, this._sql.Giving_GetPaymentReasonCode(churchId, 2204, paymentTime, "Infellowship Give Now"), "Reason code was not 233.");
                            this._selenium.VerifyTextPresent(validationText2);
                        }
                    }

                        //Suchitra - New AMS Error codes validation

                        //Reason Code AMS - RC-211

                        // 211 Reason Code
                    else if (contributionData.Any(contribution => contribution.Amount == "2"))
                    {



                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 2, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to 211                                                                                 
                            Assert.AreEqual(211, this._sql.Giving_GetPaymentReasonCode(churchId, 2, paymentTime, "Infellowship Give Now"), "AMS reason code was not 231.");
                            this._selenium.VerifyTextPresent(this.Get_AMS_Error_Text("211"));
                        }
                        /* else
                         {
                             // Verify the reason code is 211
                             Assert.AreEqual(211, this._sql.Giving_GetPaymentReasonCode(churchId, 2531, paymentTime, "Infellowship Give Now"), "Reason code was not 211.");
                             this._selenium.VerifyTextPresent(validationText1);
                         } */
                    }
                        //Reason Code AMS - RC-230

                        // 230 Reason Code
                    else if (contributionData.Any(contribution => contribution.Amount == "5"))
                    {



                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 5, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to 230                                                                                 
                            Assert.AreEqual(230, this._sql.Giving_GetPaymentReasonCode(churchId, 5, paymentTime, "Infellowship Give Now"), "AMS reason code was not 230.");
                            this._selenium.VerifyTextPresent(this.Get_AMS_Error_Text("230"));
                        }
                    }

                        //Reason Code AMS - RC-CVV2 - 200 in table

                        // CVV2 Reason Code - 200
                    else if (contributionData.Any(contribution => contribution.Amount == "4"))
                    {



                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 4, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to CCVS_Declined since I did not find the code CCV2                                                                                
                            Assert.AreEqual(200, this._sql.Giving_GetPaymentReasonCode(churchId, 4, paymentTime, "Infellowship Give Now"), "AMS reason code was not 200.");
                            this._selenium.VerifyTextPresent(this.Get_AMS_Error_Text("200"));
                        }
                    }

                         //Reason Code AMS - RC-ZERO

                        // ZERO Reason Code
                    else if (contributionData.Any(contribution => contribution.Amount == "3"))
                    {



                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 3, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to 930202 - for now I selected this since no ZERO present in the table                                                                                 
                            Assert.AreEqual(930202, this._sql.Giving_GetPaymentReasonCode(churchId, 3, paymentTime, "Infellowship Give Now"), "AMS reason code was not ZERO.");
                            this._selenium.VerifyTextPresent(this.Get_AMS_Error_Text("930202"));
                        }
                    }


                        // Anything else
                    // The following error occurred…
                    // Authorization was not successful. Please check your information and try again.
                    //else (this._selenium.IsElementPresent("//div[@class='error_msgs_for']") & (!validated) )
                    else
                    {
                        log.Debug("Most likely got a generic error message");
                        throw new SeleniumException(string.Format("Unknown Error Message: {0}", this._selenium.GetText("//div[@class='error_msgs_for']")));
                    }

                    // Cancel out of the wizard
                    this._selenium.ClickAndWaitForPageToLoad("link=Cancel");
                }
                #endregion Step 2 Validation
            }

            #region Step 1 validation
            else {

                // Check the presence validation messages based on what was provided and what wasn't provided
                // If the fund or pledge drive is not provided, verify Give to is required.
                if (contributionData.Any(contribution => string.IsNullOrEmpty(contribution.FundOrPledgeDrive))) {
                    this._selenium.VerifyTextPresent("Give to... is required");
                }
                else {
                    // If the fund or pledge drive is provided, verify amount is provided.
                    if (contributionData.Any(contribution => string.IsNullOrEmpty(contribution.Amount))) {
                        this._selenium.VerifyTextPresent("Please enter a valid amount");
                    }

                    if (contributionData.Any(contribution => contribution.Amount == "0.00")) {
                        this._selenium.VerifyTextPresent("Please enter a valid amount");
                    }
                }

                if (creditCardType == "--") {
                    this._selenium.VerifyTextPresent("Credit card type is required");
                }

                if (string.IsNullOrEmpty(creditCardNumber)) {
                    this._selenium.VerifyTextPresent("Credit card number is required");
                }

                //else if (creditCardNumber.Length > 30) {
                //    this._selenium.VerifyTextPresent("Credit card cannot exceed 30 characters");
                //}

                if (string.IsNullOrEmpty(firstName)) {
                    this._selenium.VerifyTextPresent("First name is required");
                }

                else if (firstName.Length > 50) {
                    this._selenium.VerifyTextPresent("First name cannot exceed 50 characters");
                }

                if (string.IsNullOrEmpty(lastName)) {
                    this._selenium.VerifyTextPresent("Last name is required");
                }

                else if (lastName.Length > 50) {
                    this._selenium.VerifyTextPresent("Last name cannot exceed 50 characters");
                }

                if (expirationMonth == "--") {
                    this._selenium.VerifyTextPresent("Please select a valid expiration month");
                }

                if (expirationYear == "--") {
                    this._selenium.VerifyTextPresent("Please select a valid expiration year");
                }

                if (expirationYear == DateTime.Now.Year.ToString()) {
                    this._selenium.VerifyTextPresent("Card is expired");
                }

                if (!validCreditCard) {
                    this._selenium.VerifyTextPresent("The card number is not valid for the type of card selected");
                }

                if (streetOneText.Length > 40) {
                    this._selenium.VerifyTextPresent("Street 1 cannot exceed 40 characters");
                }

                if (streetTwoText.Length > 40) {
                    this._selenium.VerifyTextPresent("Street 2 cannot exceed 40 characters");
                }

                if (cityText.Length > 30) {
                    this._selenium.VerifyTextPresent("City cannot exceed 30 characters");
                }

                if (postCodeText.Length > 10) {
                    this._selenium.VerifyTextPresent("Postal Code cannot exceed 10 characters");
                }

                if (countryName.Length > 50) {
                    this._selenium.VerifyTextPresent("County cannot exceed 50 characters");
                }

                // Jump out of the wizard so we can continue.
                this._selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", GeneralInFellowship.Giving.Cancel));
            }
            #endregion Step 1 validation
        }

        /// <summary>
        /// Steps through the Give Now wizard to process a Credit Card payment.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="contributionData">A collection of funds, subfunds, and amounts to give to.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="creditCardType">The credit card type.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <param name="securityCode">The security code.</param>
        /// <param name="validCreditCard">Specifies if this credit card is valid or not.</param>
        /// <param name="country">The country</param>
        /// <param name="streetOne">The street one address.</param>
        /// <param name="streetTwo">The street two address.</param>
        /// <param name="city">The city.</param>
        /// <param name="state">The state.</param>
        /// <param name="postalCode">The postal code.</param>
        /// <param name="county">The county.</param>
        private void Giving_GiveNow_CreditCard_Process_WebDriver(int churchId, IEnumerable<dynamic> contributionData, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, bool validCreditCard, string country, string streetOne, string streetTwo, string city, string state, string postalCode, string county, string validFromMonth, string validFromYear) {

            // Store the payment type id and credit card name.  This is used for queries and validation.
            #region Payment Types
            var paymentTypeId = 0;
            var creditCardName = creditCardType;
            Boolean amsEnabled = false;

            switch (creditCardType) {
                case "Visa":
                    paymentTypeId = 1;
                    break;
                case "Master Card":
                    paymentTypeId = 2;
                    creditCardName = "Mastercard";
                    break;
                case "American Express":
                    paymentTypeId = 3;
                    creditCardName = "AMEX";
                    break;
                case "Discover":
                    paymentTypeId = 4;
                    break;
                case "Diners Club":
                    paymentTypeId = 5;
                    break;
                case "Carte Blanche":
                    paymentTypeId = 6;
                    break;
                case "JCB":
                    paymentTypeId = 7;
                    break;
                case "Optima":
                    paymentTypeId = 8;
                    break;
                case "EnRoute":
                    paymentTypeId = 9;
                    break;
                case "Delta":
                    paymentTypeId = 20;
                    break;
                //case "Switch":
                //    paymentTypeId = 11;
                //    break;
                //case "Solo":
                //    paymentTypeId = 13;
                //    break;
                default:
                    break;
            }
            #endregion Payment Types


            // What is our AMS Status, default is always false
            amsEnabled = this._sql.IsAMSEnabled(churchId);

            TestLog.WriteLine(string.Format("AMS Church ID {0} Enabled: {1}", churchId, amsEnabled));

            // Store the currency format
            CultureInfo culture = null;
            if (churchId == 258) {
                culture = new CultureInfo("en-GB");
            }
            else {
                culture = new CultureInfo("en-US");
            }

            // Store the last four digits of the credit card
            var lastFourDigits = string.Empty;
            if (!string.IsNullOrEmpty(creditCardNumber)) {
                lastFourDigits = creditCardNumber.Substring(creditCardNumber.Length - 4);
            }

            this._generalMethods.WaitForElement(this._driver, By.LinkText("Your Giving"));

            // View Giving History
            this._driver.FindElementByLinkText("Your Giving").Click();
            //TestLog.WriteLine("Your Giving Landing Page");
            //this._generalMethods.TakeScreenShot_WebDriver();

            // If in Mobile view verify Schedule Giving is not visible
            var windowSize = this._driver.Manage().Window.Size;

            if (windowSize.Width == 640) 
            {
                this._generalMethods.VerifyElementPresentWebDriver(By.LinkText("Schedule Giving"));
            }

            // Click on Give Now
            this._driver.FindElementByPartialLinkText("Give Now").Click();

            #region Step 1

            // Populate step 1
            this.Giving_GiveNow_Populate_Step1_WebDriver(churchId, "Credit Card", contributionData, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, country, streetOne, streetTwo, city, state, postalCode, county, null, null, null, validFromMonth, validFromYear);

            // Store the Billing address information for verification purposes on Step 2
            var streetOneText = this._driver.FindElementById("Address1").GetAttribute("value");
            var streetTwoText = this._driver.FindElementById("Address2").GetAttribute("value");
            var cityText = this._driver.FindElementById("City").GetAttribute("value");
            var stateText = new SelectElement(this._driver.FindElementById("state")).SelectedOption.GetAttribute("value");
            
            TestLog.WriteLine("Address Line 2: {0}", streetTwoText);

            // IWebElement comboBox = driver.FindElement(By.Id("superior"));
            // SelectElement selectedValue = new SelectElement(comboBox);
            // string wantedText = selectedValue.SelectedOption.Text;

            // If international, the get the province
            if (churchId == 258) {
                stateText = string.IsNullOrEmpty(city) ? this._driver.FindElementById("StProvince").GetAttribute("value") : city;
            }
            var postCodeText = this._driver.FindElementById("PostalCode").GetAttribute("value");
            var countryName = string.IsNullOrEmpty(country) ? new SelectElement(this._driver.FindElementById("Country")).SelectedOption.Text : country;


            // Get the selected month's numerical value.
            // Store the month number
            var monthNumber = string.Empty;
            if (!string.IsNullOrEmpty(expirationMonth)) {
                // If the month number is a single digit, prepend a 0.
                var selectedMonth = new SelectElement(this._driver.FindElementById("expiration_month")).SelectedOption.GetAttribute("value");
                TestLog.WriteLine("Selected Month: {0}", selectedMonth);
                monthNumber = selectedMonth.Length > 1 ? selectedMonth : "0" + selectedMonth;
            }

            // Store the total contribution
            var total = this._driver.FindElementById("total_sum").Text;

            // Click to Continue
            if (this._generalMethods.IsElementVisibleWebDriver(By.XPath(GeneralInFellowship.Giving.Continue)))
            {
                this._driver.FindElementByXPath(GeneralInFellowship.Giving.Continue).Click();
            }
            else
            {
                this._driver.FindElementByXPath(GeneralInFellowship.Giving.Continue_Responsive).Click();
            }

            #endregion Step 1

            #region Step 2
            // Unless Step 1 validation is present, continue to Step 2
            if (!this._generalMethods.IsElementPresentWebDriver(By.XPath("//div[@class='error_msgs_for']"))) {

                // Verify all data populated on step 1 is present

                // Contribution Details Section
                // Since there are multiple funds/subfunds and amounts that can be used, loop through the collection.
                for (int i = 0; i < contributionData.Count<dynamic>(); i++) {
                    // There is no subfund so the > character is not present.
                    if (string.IsNullOrEmpty(contributionData.ElementAt<dynamic>(i).subFund)) {
                        Assert.AreEqual(contributionData.ElementAt<dynamic>(i).fund, this._driver.FindElementByXPath("//table[@class='grid']/tbody/tr[" + (i + 1) + "]/td[1]").Text);
                    }
                    else {
                        // There is a subfund
                        Assert.AreEqual(contributionData.ElementAt<dynamic>(i).fund + " > " + contributionData.ElementAt<dynamic>(i).subFund, this._driver.FindElementByXPath("//table[@class='grid']/tbody/tr[" + (i + 1) + "]/td[1]").Text);
                    }

                    Assert.AreEqual(string.Format(culture, "{0:c}", Convert.ToDecimal(contributionData.ElementAt<dynamic>(i).amount)), this._driver.FindElementByXPath("//table[@class='grid']/tbody/tr[" + (i + 1) + "]/td[2]").Text);
                }

                // Is the total correct?
                Assert.AreEqual("Total", this._driver.FindElementByXPath("//table[@class='grid']/tfoot/tr[1]/td[1]").Text);
                Assert.AreEqual(string.Format(culture, "{0:c}", Convert.ToDecimal(total)), this._driver.FindElementByXPath("//table[@class='grid']/tfoot/tr[1]/td[2]").Text);

                // Payment Information Section
                Assert.AreEqual("Payment Information", this._driver.FindElementByXPath("//span[@class='icon_text icon_payment']").Text);
                Assert.AreEqual(string.Format("{0}\r\n{1} ending in {2}\r\nExpires {3}/{4}", firstName + " " + lastName, creditCardName, lastFourDigits, monthNumber, expirationYear), this._driver.FindElementByXPath("//table[@class='grid']/tbody/tr[preceding-sibling::tr/th/span[@class='icon_text icon_payment']]/td[1]").Text);

                // Street two is not required and results in a different rendering if it is provided.
                if (string.IsNullOrEmpty(streetTwoText)) {
                    Assert.AreEqual(string.Format("{0}\r\n{1}, {2} {3}\r\n{4}", streetOneText, cityText, stateText, postCodeText, countryName), this._driver.FindElementByXPath("//table[@class='grid']/tbody/tr[preceding-sibling::tr/th/span[@class='icon_text icon_payment']]/td[2]").Text);
                }

                else {
                    Assert.AreEqual(string.Format("{0}\r\n{1}\r\n{2}, {3} {4}\r\n{5}", streetOneText, streetTwoText, cityText, stateText, postCodeText, countryName), this._driver.FindElementByXPath("//table[@class='grid']/tbody/tr[preceding-sibling::tr/th/span[@class='icon_text icon_payment']]/td[2]").Text);
                }

                // Submit to attempt to process this payment. Store the time it takes to submit.
                if (this._generalMethods.IsElementVisibleWebDriver(By.XPath(GeneralInFellowship.Giving.Process_Payment)))
                {
                    TestLog.WriteLine("Click on Process Payment - Non Responsive");
                    this._driver.FindElementByXPath(GeneralInFellowship.Giving.Process_Payment).Click();
                }
                else
                {
                    TestLog.WriteLine("Click on Process Payment - Responsive");
                    this._driver.FindElementByXPath(GeneralInFellowship.Giving.Process_Payment_Responsive).Click();
                }

                DateTime paymentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMinutes(-5);
                // DateTime paymentTime = (this._selenium.ClickAndWaitForPageToLoad("//input[@value='Process this payment']")).AddMinutes(-5);

            #endregion Step 2

                // Unless Step 2 validation occurs, verify the payment exists on the giving history tab
                if (!this._generalMethods.IsElementPresentWebDriver(By.XPath("//div[@class='error_msgs_for']"))) {


                    // If the payment is pending, verify it is pending and wait for it to be processed
                    var paymentStatusID = this._sql.Giving_GetPaymentStatusID(churchId, Convert.ToDouble(total), paymentTime, "Infellowship Give Now", paymentTypeId);
                    if (paymentStatusID == 3) {

                        // Verify Giving History before the payment is processed
                        foreach (var item in contributionData) {
                            // Validate the Giving History Page
                            this.Giving_GivingHistory_Validate_WebDriver(churchId, firstName, lastName, item.fund, item.subFund, item.amount, paymentTime, paymentTypeId, "Credit Card", false);
                        }

                    }

                    // Wait until the payment has processed
                    this._sql.Giving_WaitUntilPaymentProcessed(churchId, Convert.ToDouble(total), paymentTime, "Infellowship Give Now", paymentTypeId);

                    // View Give Now 
                    this._driver.FindElementById(GeneralInFellowship.Giving.GivingHistory_View).Click();

                    // Verify Giving History after the payment is processed
                    foreach (var item in contributionData) {
                        // Validate the Giving History Page
                        this.Giving_GivingHistory_Validate_WebDriver(churchId, firstName, lastName, item.fund, item.subFund, item.amount, paymentTime, paymentTypeId, "Credit Card", true);
                    }
                }
                #region Step 2 Validation
                // Validation has occured on Step 2 of the Give Now wizard. See if the messages are correct along with the payment reason codes.
                else {

                  //  var validationText1 = "Authorization was not successful. Please check your information and try again. An error has occurred while attempting to process the request.";
                  //  var validationText2 = "Authorization was not successful. Please use a different card or select another form of payment.";

                    //SP - Message changed to the following
                    var validationText1 = "Authorization was not successful. Please check your information including your billing address to make sure they are accurate, or use another card or select another form of payment. An error has occurred while attempting to process the request.";
                    var validationText2 = "Authorization was not successful. Please check your information including your billing address to make sure they are accurate, or use another card or select another form of payment.";

                    // Verify the validation messages and reason codes
                    // 150 Reason Code
                    if (contributionData.Any(contribution => contribution.amount == "2000"))
                    {

                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 2000, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to 233                                                                                 
                            Assert.AreEqual(233, this._sql.Giving_GetPaymentReasonCode(churchId, 2000, paymentTime, "Infellowship Give Now"), "AMS reason code was not 233.");
                            this._generalMethods.VerifyTextPresentWebDriver(this.Get_AMS_Error_Text("233"));
                            
                        }
                        else
                        {
                            
                            // Verify the reason code is 150
                            Assert.AreEqual(150, this._sql.Giving_GetPaymentReasonCode(churchId, 2000, paymentTime, "Infellowship Give Now"), "Reason code was not 150.");
                            this._generalMethods.VerifyTextPresentWebDriver(validationText1);
                        }
                    }

                    // 201 Reason Code
                    else if (contributionData.Any(contribution => contribution.amount == "2402"))
                    {

                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 2402, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to 233
                            Assert.AreEqual(233, this._sql.Giving_GetPaymentReasonCode(churchId, 2402, paymentTime, "Infellowship Give Now"), "AMS reason code was not 233.");
                            this._generalMethods.VerifyTextPresentWebDriver(this.Get_AMS_Error_Text("233"));
                            
                        }
                        else
                        {
                            // Verify the reason code is 201
                            Assert.AreEqual(201, this._sql.Giving_GetPaymentReasonCode(churchId, 2402, paymentTime, "Infellowship Give Now"), "Reason code was not 201.");
                            this._generalMethods.VerifyTextPresentWebDriver(validationText2);
                        }
                    }

                    // 203 Reason Code
                    else if (contributionData.Any(contribution => contribution.amount == "2260"))
                    {

                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 2260, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to 233                                                                                 
                            Assert.AreEqual(233, this._sql.Giving_GetPaymentReasonCode(churchId, 2260, paymentTime, "Infellowship Give Now"), "AMS reason code was not 231.");
                            this._generalMethods.VerifyTextPresentWebDriver(this.Get_AMS_Error_Text("233"));
                            
                        }
                        else
                        {
                            // Verify the reason code is generic reason
                            Assert.AreEqual(150, this._sql.Giving_GetPaymentReasonCode(churchId, 2260, paymentTime, "Infellowship Give Now"), "Reason code was not 203.");
                            this._generalMethods.VerifyTextPresentWebDriver(validationText1);
                        }

                    }

                    // 204 Reason Code
                    else if (contributionData.Any(contribution => contribution.amount == "2521"))
                    {

                        //Legacy to AMS error code mapping stays the same.

                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 2521, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to 204                                                                                 
                            Assert.AreEqual(204, this._sql.Giving_GetPaymentReasonCode(churchId, 2521, paymentTime, "Infellowship Give Now"), "AMS reason code was not 204.");
                            this._generalMethods.VerifyTextPresentWebDriver(this.Get_AMS_Error_Text("204"));
                            
                        }
                        else
                        {
                            // Verify the reason code is 204
                            Assert.AreEqual(204, this._sql.Giving_GetPaymentReasonCode(churchId, 2521, paymentTime, "Infellowship Give Now"), "Reason code was not 204.");
                            this._generalMethods.VerifyTextPresentWebDriver("Authorization was not successful. Insufficient funds in the account. Please use a different card or select another form of payment.");
                        }
                    }

                    // 205 Reason Code
                    else if (contributionData.Any(contribution => contribution.amount == "2501"))
                    {

                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 2501, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to 231                                                                                 
                            Assert.AreEqual(231, this._sql.Giving_GetPaymentReasonCode(churchId, 2501, paymentTime, "Infellowship Give Now"), "AMS reason code was not 231.");
                            this._generalMethods.VerifyTextPresentWebDriver(this.Get_AMS_Error_Text("231"));
                           // Assert.AreEqual(this.Get_AMS_Error_Text("231"), this._driver.FindElement(By.XPath("//div[@class='error_msgs_for']")).Text);
                            
                        }
                        else
                        {
                            // Verify the reason code is 205
                            Assert.AreEqual(205, this._sql.Giving_GetPaymentReasonCode(churchId, 2501, paymentTime, "Infellowship Give Now"), "Reason code was not 205.");
                            this._generalMethods.VerifyTextPresentWebDriver(validationText2);
                        }
                    }

                    // 208 Reason Code
                    else if (contributionData.Any(contribution => contribution.amount == "2606"))
                    {

                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 2606, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to 231                                                                                 
                            Assert.AreEqual(231, this._sql.Giving_GetPaymentReasonCode(churchId, 2606, paymentTime, "Infellowship Give Now"), "AMS reason code was not 231.");
                            this._generalMethods.VerifyTextPresentWebDriver(this.Get_AMS_Error_Text("231"));
                            
                        }
                        else
                        {
                            // Verify the reason code is 208
                            Assert.AreEqual(208, this._sql.Giving_GetPaymentReasonCode(churchId, 2606, paymentTime, "Infellowship Give Now"), "Reason code was not 208.");
                            this._generalMethods.VerifyTextPresentWebDriver(validationText2);
                        }
                    }

                    // 209 Reason Code
                    else if (contributionData.Any(contribution => contribution.amount == "2811"))
                    {

                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 2811, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to 231                                                                                 
                            Assert.AreEqual(231, this._sql.Giving_GetPaymentReasonCode(churchId, 2811, paymentTime, "Infellowship Give Now"), "AMS reason code was not 231.");
                            this._generalMethods.VerifyTextPresentWebDriver(this.Get_AMS_Error_Text("231"));
                            
                        }
                        else
                        {
                            // Verify the reason code is 209
                            Assert.AreEqual(209, this._sql.Giving_GetPaymentReasonCode(churchId, 2811, paymentTime, "Infellowship Give Now"), "Reason code was not 209.");
                            this._generalMethods.VerifyTextPresentWebDriver(validationText1);
                        }
                    }

                    // 211 Reason Code
                    else if (contributionData.Any(contribution => contribution.amount == "2531"))
                    {

                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 2531, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to 231                                                                                 
                            Assert.AreEqual(231, this._sql.Giving_GetPaymentReasonCode(churchId, 2531, paymentTime, "Infellowship Give Now"), "AMS reason code was not 231.");
                            this._generalMethods.VerifyTextPresentWebDriver(this.Get_AMS_Error_Text("231"));
                            
                        }
                        else
                        {
                            // Verify the reason code is 211
                            Assert.AreEqual(211, this._sql.Giving_GetPaymentReasonCode(churchId, 2531, paymentTime, "Infellowship Give Now"), "Reason code was not 211.");
                            this._generalMethods.VerifyTextPresentWebDriver(validationText1);
                        }
                    }

                    // 233 Reason Code
                    else if (contributionData.Any(contribution => contribution.amount == "2204"))
                    {

                        //Legacy to AMS error code mapping stays the same.
                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 2204, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to 233                                                                                 
                            Assert.AreEqual(233, this._sql.Giving_GetPaymentReasonCode(churchId, 2204, paymentTime, "Infellowship Give Now"), "AMS reason code was not 233.");
                            this._generalMethods.VerifyTextPresentWebDriver(this.Get_AMS_Error_Text("233"));
                           
                        }
                        else
                        {
                            // Verify the reason code is 233
                            Assert.AreEqual(233, this._sql.Giving_GetPaymentReasonCode(churchId, 2204, paymentTime, "Infellowship Give Now"), "Reason code was not 233.");
                            this._generalMethods.VerifyTextPresentWebDriver(validationText2);
                        }
                    }
                    //Adding AMS Error amounts codes
                    else if (contributionData.Any(contribution => contribution.amount == "2"))
                    {

                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 2, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to Generic validation message                                                                                 
                            Assert.AreEqual(211, this._sql.Giving_GetPaymentReasonCode(churchId, 2, paymentTime, "Infellowship Give Now"), "AMS reason code was not 233.");
                            
                            this._generalMethods.VerifyTextPresentWebDriver(validationText1);
                        }
                        
                    }

                    else if (contributionData.Any(contribution => contribution.amount == "5"))
                    {
                                                
                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, 5, paymentTime, "Infellowship Give Now", paymentTypeId);

                        if (amsEnabled)
                        {
                            //AMS Mapping to Generic validation message                                                                                 
                            Assert.AreEqual(230, this._sql.Giving_GetPaymentReasonCode(churchId, 5, paymentTime, "Infellowship Give Now"), "AMS reason code was not 233.");
                            
                            this._generalMethods.VerifyTextPresentWebDriver(validationText1);
                        }
                        
                    }

                    // Anything else
                    // The following error occurred…
                    // Authorization was not successful. Please check your information and try again.
                    //else (this._selenium.IsElementPresent("//div[@class='error_msgs_for']") & (!validated) )
                    else {
                        log.Debug("Most likely got a generic error message");
                        throw new SeleniumException(string.Format("Unknown Error Message: {0}", this._driver.FindElementByXPath("//div[@class='error_msgs_for']").Text));
                    }

                    // Cancel out of the wizard
                    this._driver.FindElementByLinkText("Cancel").Click();

                }
                #endregion Step 2 Validation
            }

            #region Step 1 validation
            else {

                // Check the presence validation messages based on what was provided and what wasn't provided
                // If the fund or pledge drive is not provided, verify Give to is required.
                if (contributionData.Any(contribution => string.IsNullOrEmpty(contribution.fund))) {
                    this._generalMethods.VerifyTextPresentWebDriver("Give to... is required");
                }
                else {
                    // If the fund or pledge drive is provided, verify amount is provided.
                    if (contributionData.Any(contribution => string.IsNullOrEmpty(contribution.amount))) {
                        this._generalMethods.VerifyTextPresentWebDriver("Please enter a valid amount");
                    }

                    if (contributionData.Any(contribution => contribution.amount == "0.00")) {
                        this._generalMethods.VerifyTextPresentWebDriver("Please enter a valid amount");
                    }
                }

                if (creditCardType == "--") {
                    this._generalMethods.VerifyTextPresentWebDriver("Credit card type is required");
                }

                if (string.IsNullOrEmpty(creditCardNumber)) {
                    this._generalMethods.VerifyTextPresentWebDriver("Credit card number is required");
                }

                //else if (creditCardNumber.Length > 30) {
                //    this._generalMethods.VerifyTextPresentWebDriver("Credit card cannot exceed 30 characters");
                //}

                if (string.IsNullOrEmpty(firstName)) {
                    this._generalMethods.VerifyTextPresentWebDriver("First name is required");
                }

                else if (firstName.Length > 50) {
                    this._generalMethods.VerifyTextPresentWebDriver("First name cannot exceed 50 characters");
                }

                if (string.IsNullOrEmpty(lastName)) {
                    this._generalMethods.VerifyTextPresentWebDriver("Last name is required");
                }

                else if (lastName.Length > 50) {
                    this._generalMethods.VerifyTextPresentWebDriver("Last name cannot exceed 50 characters");
                }

                if (expirationMonth == "--") {
                    this._generalMethods.VerifyTextPresentWebDriver("Please select a valid expiration month");
                }

                if (expirationYear == "--") {
                    this._generalMethods.VerifyTextPresentWebDriver("Please select a valid expiration year");
                }

                if (expirationYear == DateTime.Now.Year.ToString()) {
                    this._generalMethods.VerifyTextPresentWebDriver("Card is expired");
                }

                if (!validCreditCard) {
                    this._generalMethods.VerifyTextPresentWebDriver("The card number is not valid for the type of card selected");
                }

                if (streetOneText.Length > 40) {
                    this._generalMethods.VerifyTextPresentWebDriver("Street 1 cannot exceed 40 characters");
                }

                if (streetTwoText.Length > 40) {
                    this._generalMethods.VerifyTextPresentWebDriver("Street 2 cannot exceed 40 characters");
                }

                if (cityText.Length > 30) {
                    this._generalMethods.VerifyTextPresentWebDriver("City cannot exceed 30 characters");
                }

                if (postCodeText.Length > 10) {
                    this._generalMethods.VerifyTextPresentWebDriver("Postal Code cannot exceed 10 characters");
                }

                if (countryName.Length > 50) {
                    this._generalMethods.VerifyTextPresentWebDriver("County cannot exceed 50 characters");
                }

                // Jump out of the wizard so we can continue.
                this._driver.FindElementByLinkText("Cancel").Click();
            }
            #endregion Step 1 validation
        }

        /// <summary>
        /// Steps through the Give Now wizard to process a Personal Check payment.
        /// </summary>
        /// <param name="churchId">The church ID.</param>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="routingNumber">The routing number.</param>
        /// <param name="accountNumber">The account number.</param>
        private void Giving_GiveNow_PersonalCheck_Process([Optional, DefaultParameterValue(15)] int churchId, string firstName, string lastName, IEnumerable<dynamic> contributionData, string phoneNumber, string routingNumber, string accountNumber) {

            // Store the payment type
            var eCheckPaymentTypeID = 19;

            // View Give Now 
            this._selenium.ClickAndWaitForPageToLoad("link=Your Giving");
            this._selenium.ClickAndWaitForPageToLoad("link=Give Now");

            #region Step 1

            // Populate step 1
            this.Giving_GiveNow_Populate_Step1(churchId, "Personal Check", contributionData, null, null, null, null, null, null, null, null, null, null, null, null, null, null, phoneNumber, routingNumber, accountNumber, null, null);

            // Store the total contribution
            var total = this._selenium.GetText("total_sum");

            #endregion Step 1

            // Click to Continue
            if (this._selenium.IsVisible(GeneralInFellowship.Giving.Continue))
            {
                this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowship.Giving.Continue);
            }
            else
            {
                this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowship.Giving.Continue_Responsive);
            }

            // Unless validation occurs on Step 1, continue to step 2.
            if (!this._selenium.IsElementPresent("//div[@class='error_msgs_for']")) {

                // Store the last four digits of the account nummber
                var lastFourDigitsAccount = accountNumber.Substring(accountNumber.Length - 4);

                // Store the last four digits of the routing nummber
                var lastFourDigitsRouting = routingNumber.Substring(routingNumber.Length - 4);

                // Verify all data populated on step 1 is present
                // Contribution Details
                // Formatting changes based on a sub fund being present
                for (int i = 0; i < contributionData.Count<dynamic>(); i++) {
                    if (string.IsNullOrEmpty(contributionData.ElementAt<dynamic>(i).SubFund)) {
                        Assert.AreEqual(contributionData.ElementAt<dynamic>(i).FundOrPledgeDrive, this._selenium.GetText("//table[@class='grid']/tbody/tr[" + (i + 1) + "]/td[1]"));
                    }
                    else {
                        Assert.AreEqual(contributionData.ElementAt<dynamic>(i).FundOrPledgeDrive + " > " + contributionData.ElementAt<dynamic>(i).SubFund, this._selenium.GetText("//table[@class='grid']/tbody/tr[" + (i + 1) + "]/td[1]"));
                    }
                    Assert.AreEqual(string.Format("{0:c}", Convert.ToDecimal(contributionData.ElementAt<dynamic>(i).Amount)), this._selenium.GetText("//table[@class='grid']/tbody/tr[" + (i + 1) + "]/td[2]"));
                }

                Assert.AreEqual("Total", this._selenium.GetText("//table[@class='grid']/tfoot/tr[1]/td[1]"));
                Assert.AreEqual(string.Format("{0:c}", Convert.ToDecimal(total)), this._selenium.GetText("//table[@class='grid']/tfoot/tr[1]/td[2]"));


                // Payment Information
                Assert.AreEqual("Payment Information", this._selenium.GetText("//span[@class='icon_text icon_payment']"));
                Assert.AreEqual(string.Format("{0} \n eCheck ending in {1} \n Routing Number ending in {2}", firstName + " " + lastName, lastFourDigitsAccount, lastFourDigitsRouting), this._selenium.GetText("//table[@class='grid']/tbody/tr[preceding-sibling::tr/th/span[@class='icon_text icon_payment']]/td[1]"));
                Assert.AreEqual(string.Format("Phone number: {0}", phoneNumber), this._selenium.GetText("//table[@class='grid']/tbody/tr[preceding-sibling::tr/th/span[@class='icon_text icon_payment']]/td[2]"));
            

                // Submit the payment.  Store the time it takes to process this payment.
                if (this._selenium.IsVisible(GeneralInFellowship.Giving.Process_Payment))
                {
                    this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowship.Giving.Process_Payment);
                }
                else
                {
                    this._selenium.ClickAndWaitForPageToLoad(GeneralInFellowship.Giving.Process_Payment_Responsive);
                }
                DateTime paymentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMinutes(-5);

                // Unless there is validation on step 2, continue to the Giving History page and verify the payment was processed
                if (!this._selenium.IsElementPresent("//div[@class='error_msgs_for']")) {

                    // If no validation was present but we provided an account that would provide velocity validation, fail the test
                    if ( (!this._sql.IsAMSEnabled(churchId)) && accountNumber == "123123123" && (contributionData.Any(contribution => contribution.Amount == "1101" || contribution.Amount == "1102"))) {
                        Assert.Fail("Validation for a velocity amount was expected but no validation occured!  The payment attempted to process normally!");
                    }
                    else {
                        // If the payment is pending, verify it is pending and wait for it to be processed
                        if (this._sql.Giving_GetPaymentStatusID(churchId, Convert.ToDouble(total), paymentTime, "Infellowship Give Now", eCheckPaymentTypeID) == 22) {

                            // Verify Giving History before the payment is processed
                            foreach (var item in contributionData) {
                                // Validate the Giving History Page
                                this.Giving_GivingHistory_Validate(churchId, firstName, lastName, item.fund, item.subFund, item.amount, paymentTime, eCheckPaymentTypeID, "eCheck", false);

                            }

                            // Wait until the payment has processed
                            this._sql.Giving_WaitUntilPaymentProcessed(churchId, Convert.ToDouble(total), paymentTime, "Infellowship Give Now", eCheckPaymentTypeID);

                        }

                        // View Give Now 
                        this._selenium.ClickAndWaitForPageToLoad("link=Giving History");

                        // Verify Giving History after the payment is processed
                        foreach (var item in contributionData) {
                            // Validate the Giving History Page
                            this.Giving_GivingHistory_Validate(churchId, firstName, lastName, item.fund, item.subFund, item.amount, paymentTime, eCheckPaymentTypeID, "eCheck", true);
                        }
                    }
                }


                #region Step 2 Validation
                else {
                    // Verify the validation messages
                    // Velocity
                    if (accountNumber == "123123123" && (contributionData.Any(contribution => contribution.Amount == "1101" || contribution.Amount == "1102"))) {
                        Assert.IsTrue(this._selenium.IsTextPresent(TextConstants.ErrorHeadingSingular));
                        this._selenium.VerifyTextPresent("We're sorry, we are unable to process your payment at this time. For more information contact our office.");
                    }
                    else if (accountNumber == "123123123" && (contributionData.Any(contribution => contribution.Amount == "2101"))) {
                            Assert.IsTrue(this._selenium.IsTextPresent(TextConstants.ErrorHeadingSingular));
                            this._selenium.VerifyTextPresent("Authorization was not successful. Please check your information and try again. An error has occurred while attempting to process the request.");
                    }
                    else if (this._selenium.IsElementPresent("//div[@class='error_msgs_for']")) {
                        Assert.Fail(string.Format("{0}", this._selenium.GetText("//div[@class='error_msgs_for']")));
                    }

                    // Cancel out of the wizard
                    this._selenium.ClickAndWaitForPageToLoad("link=Cancel");
                }
                #endregion Step 2 validation
            }

            else {
                // Check the presence validation messages based on what was provided and what wasn't provided
                // If the fund or pledge drive is not provided, verify Give to is required.
                if (contributionData.Any(contribution => string.IsNullOrEmpty(contribution.fund))) {
                    this._selenium.VerifyTextPresent("Give to... is required");
                }
                else {
                    // If the fund or pledge drive is provided, verify amount is provided.
                    if (contributionData.Any(contribution => string.IsNullOrEmpty(contribution.amount))) {
                        this._selenium.VerifyTextPresent("Please enter a valid amount");
                    }
                }

                if (string.IsNullOrEmpty(phoneNumber)) {
                    this._selenium.VerifyTextPresent("Phone number is required");
                }

                else if (phoneNumber.Length > 50) {
                    this._selenium.VerifyTextPresent("Phone number cannot exceed 50 characters");
                }

                if (string.IsNullOrEmpty(accountNumber)) {
                    this._selenium.VerifyTextPresent("Account number is required");
                }

                else if (accountNumber.Length > 30) {
                    this._selenium.VerifyTextPresent("Account number cannot exceed 30 characters");
                }

                if (string.IsNullOrEmpty(routingNumber)) {
                    this._selenium.VerifyTextPresent("Bank routing number is required");
                }

                // If the routing number is greater than 9 digits, verify the correct validation message
                else if (routingNumber.Length > 9) {
                    this._selenium.VerifyTextPresent("Please provide a valid nine-digit routing number");
                }

                // If the routing number is less than 9 digits, verify the correct validation message
                else if (routingNumber.Length < 9) {
                    this._selenium.VerifyTextPresent("Please provide a valid nine-digit routing number");
                }

                this._selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", GeneralInFellowship.Giving.Cancel));
            }
        }

        /// <summary>
        /// Steps through the Give Now wizard to process a Personal Check payment.
        /// </summary>
        /// <param name="churchId">The church ID.</param>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive.</param>
        /// <param name="subFund">The subfund.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="routingNumber">The routing number.</param>
        /// <param name="accountNumber">The account number.</param>
        private void Giving_GiveNow_PersonalCheck_Process_WebDriver([Optional, DefaultParameterValue(15)] int churchId, string firstName, string lastName, IEnumerable<dynamic> contributionData, string phoneNumber, string routingNumber, string accountNumber)
        {

            // Store the payment type
            var eCheckPaymentTypeID = 19;

            // View Giving History
            this._driver.FindElementByLinkText("Your Giving").Click();

            // If in Mobile view verify Schedule Giving is not visible
            var windowSize = this._driver.Manage().Window.Size;

            if (windowSize.Width == 640)
            {
                this._generalMethods.VerifyElementNotPresentWebDriver(By.LinkText("Schedule Giving"));
            }

            // Click on Give Now
            this._driver.FindElementByPartialLinkText("Give Now").Click();

            #region Step 1

            // Populate step 1
            this.Giving_GiveNow_Populate_Step1_WebDriver(churchId, "Personal Check", contributionData, null, null, null, null, null, null, null, null, null, null, null, null, null, null, phoneNumber, routingNumber, accountNumber, null, null);

            // Store the total contribution
            var total = this._driver.FindElementById("total_sum").Text;

            #endregion Step 1

            // Click to Continue
            if (this._generalMethods.IsElementVisibleWebDriver(By.XPath(GeneralInFellowship.Giving.Continue)))
            {
                this._driver.FindElementByXPath(GeneralInFellowship.Giving.Continue).Click();
            }
            else
            {
                this._driver.FindElementByXPath(GeneralInFellowship.Giving.Continue_Responsive).Click();
            }

            // Unless Step 1 validation is present, continue to Step 2
            if (!this._generalMethods.IsElementPresentWebDriver(By.XPath("//div[@class='error_msgs_for']")))
            {

                // Store the last four digits of the account nummber
                var lastFourDigitsAccount = accountNumber.Substring(accountNumber.Length - 4);

                // Store the last four digits of the routing nummber
                var lastFourDigitsRouting = routingNumber.Substring(routingNumber.Length - 4);

                // Verify all data populated on step 1 is present
                // Contribution Details
                // Formatting changes based on a sub fund being present
                for (int i = 0; i < contributionData.Count<dynamic>(); i++)
                {
                    if (string.IsNullOrEmpty(contributionData.ElementAt<dynamic>(i).subFund))
                    {
                        Assert.AreEqual(contributionData.ElementAt<dynamic>(i).fund, this._driver.FindElementByXPath("//table[@class='grid']/tbody/tr[" + (i + 1) + "]/td[1]").Text);
                    }
                    else
                    {
                        Assert.AreEqual(contributionData.ElementAt<dynamic>(i).fund + " > " + contributionData.ElementAt<dynamic>(i).subFund, this._driver.FindElementByXPath("//table[@class='grid']/tbody/tr[" + (i + 1) + "]/td[1]").Text);
                    }
                    Assert.AreEqual(string.Format("{0:c}", Convert.ToDecimal(contributionData.ElementAt<dynamic>(i).amount)), this._driver.FindElementByXPath("//table[@class='grid']/tbody/tr[" + (i + 1) + "]/td[2]").Text);
                }

                Assert.AreEqual("Total", this._driver.FindElementByXPath("//table[@class='grid']/tfoot/tr[1]/td[1]").Text);
                Assert.AreEqual(string.Format("{0:c}", Convert.ToDecimal(total)), this._driver.FindElementByXPath("//table[@class='grid']/tfoot/tr[1]/td[2]").Text);


                // Payment Information
                Assert.AreEqual("Payment Information", this._driver.FindElementByXPath("//span[@class='icon_text icon_payment']").Text);
                Assert.AreEqual(string.Format("{0}\r\neCheck ending in {1}\r\nRouting Number ending in {2}", firstName + " " + lastName, lastFourDigitsAccount, lastFourDigitsRouting), this._driver.FindElementByXPath("//table[@class='grid']/tbody/tr[preceding-sibling::tr/th/span[@class='icon_text icon_payment']]/td[1]").Text);
                Assert.AreEqual(string.Format("Phone number: {0}", phoneNumber), this._driver.FindElementByXPath("//table[@class='grid']/tbody/tr[preceding-sibling::tr/th/span[@class='icon_text icon_payment']]/td[2]").Text);


                // Submit to attempt to process this payment. Store the time it takes to submit.
                if (this._generalMethods.IsElementVisibleWebDriver(By.XPath(GeneralInFellowship.Giving.Process_Payment)))
                {
                    this._driver.FindElementByXPath(GeneralInFellowship.Giving.Process_Payment).Click();
                }
                else
                {
                    this._driver.FindElementByXPath(GeneralInFellowship.Giving.Process_Payment_Responsive).Click();
                }
                DateTime paymentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMinutes(-5);

                // Unless there is validation on step 2, continue to the Giving History page and verify the payment was processed
                if (!this._generalMethods.IsElementPresentWebDriver(By.XPath("//div[@class='error_msgs_for']")))
                {

                    // If no validation was present but we provided an account that would provide velocity validation, fail the test
                    if ((!this._sql.IsAMSEnabled(churchId)) && accountNumber == "123123123" && (contributionData.Any(contribution => contribution.Amount == "1101" || contribution.Amount == "1102")))
                    {
                        Assert.Fail("Validation for a velocity amount was expected but no validation occured!  The payment attempted to process normally!");
                    }
                    else
                    {
                        // If the payment is pending, verify it is pending and wait for it to be processed
                        if (this._sql.Giving_GetPaymentStatusID(churchId, Convert.ToDouble(total), paymentTime, "Infellowship Give Now", eCheckPaymentTypeID) == 22)
                        {

                            // Verify Giving History before the payment is processed
                            foreach (var item in contributionData)
                            {
                                // Validate the Giving History Page
                                this.Giving_GivingHistory_Validate_WebDriver(churchId, firstName, lastName, item.fund, item.subFund, item.amount, paymentTime, eCheckPaymentTypeID, "eCheck", false);

                            }

                        }

                        // Wait until the payment has processed
                        this._sql.Giving_WaitUntilPaymentProcessed(churchId, Convert.ToDouble(total), paymentTime, "Infellowship Give Now", eCheckPaymentTypeID);

                        // View Give Now 
                        this._driver.FindElementById(GeneralInFellowship.Giving.GivingHistory_View).Click();

                        // Verify Giving History after the payment is processed
                        foreach (var item in contributionData)
                        {
                            // Validate the Giving History Page
                            this.Giving_GivingHistory_Validate_WebDriver(churchId, firstName, lastName, item.fund, item.subFund, item.amount, paymentTime, eCheckPaymentTypeID, "eCheck", true);
                        }
                    }
                }
                #region Step 2 Validation
                else
                {
                    // Verify the validation messages
                    // Velocity
                    if (accountNumber == "123123123" && (contributionData.Any(contribution => contribution.amount == "1101" || contribution.amount == "1102")))
                    {
                        Assert.IsTrue(this._generalMethods.IsTextPresentWebDriver(TextConstants.ErrorHeadingSingular));
                        this._generalMethods.VerifyTextPresentWebDriver("We're sorry, we are unable to process your payment at this time. For more information contact our office.");
                    }
                    else if (accountNumber == "123123123" && (contributionData.Any(contribution => contribution.amount == "2101")))
                    {
                        Assert.IsTrue(this._generalMethods.IsTextPresentWebDriver(TextConstants.ErrorHeadingSingular));
                        this._generalMethods.VerifyTextPresentWebDriver("Authorization was not successful. Please check your information and try again. An error has occurred while attempting to process the request.");
                    }
                    else if (this._generalMethods.IsElementPresentWebDriver(By.XPath("//div[@class='error_msgs_for']")))
                    {
                        Assert.Fail(string.Format("{0}", this._driver.FindElementByXPath("//div[@class='error_msgs_for']").Text));
                    }

                    // Cancel out of the wizard
                    this._driver.FindElementByLinkText("Cancel").Click();
                }
                #endregion Step 2 validation
            }

            else
            {
                // Check the presence validation messages based on what was provided and what wasn't provided
                // If the fund or pledge drive is not provided, verify Give to is required.
                if (contributionData.Any(contribution => string.IsNullOrEmpty(contribution.fund)))
                {
                    this._generalMethods.VerifyTextPresentWebDriver("Give to... is required");
                }
                else
                {
                    // If the fund or pledge drive is provided, verify amount is provided.
                    if (contributionData.Any(contribution => string.IsNullOrEmpty(contribution.amount)))
                    {
                        this._generalMethods.VerifyTextPresentWebDriver("Please enter a valid amount");
                    }
                }

                if (string.IsNullOrEmpty(phoneNumber))
                {
                    this._generalMethods.VerifyTextPresentWebDriver("Phone number is required");
                }

                else if (phoneNumber.Length > 50)
                {
                    this._generalMethods.VerifyTextPresentWebDriver("Phone number cannot exceed 50 characters");
                }

                if (string.IsNullOrEmpty(accountNumber))
                {
                    this._generalMethods.VerifyTextPresentWebDriver("Account number is required");
                }

                else if (accountNumber.Length > 30)
                {
                    this._generalMethods.VerifyTextPresentWebDriver("Account number cannot exceed 30 characters");
                }

                if (string.IsNullOrEmpty(routingNumber))
                {
                    this._generalMethods.VerifyTextPresentWebDriver("Bank routing number is required");
                }

                // If the routing number is greater than 9 digits, verify the correct validation message
                else if (routingNumber.Length > 9)
                {
                    this._generalMethods.VerifyTextPresentWebDriver("Please provide a valid nine-digit routing number");
                }

                // If the routing number is less than 9 digits, verify the correct validation message
                else if (routingNumber.Length < 9)
                {
                    this._generalMethods.VerifyTextPresentWebDriver("Please provide a valid nine-digit routing number");
                }

                this._driver.FindElementByLinkText(GeneralInFellowship.Giving.Cancel).Click() ;
            }
        }

        /// <summary>
        /// Populates the fields on step 1 of the Give Now Wizard
        /// </summary>
        /// <param name="churchId">The church ID</param>
        /// <param name="type">The type of contribution (Credit card or Personal Check)</param>
        /// <param name="contributionData">A collection of funds, subfunds, and amounts to give to.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="creditCardType">The credit card type.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <param name="securityCode">The security code.</param>
        /// <param name="country">The country</param>
        /// <param name="streetOne">The street one address.</param>
        /// <param name="streetTwo">The street two address.</param>
        /// <param name="city">The city.</param>
        /// <param name="state">The state.</param>
        /// <param name="postalCode">The postal code.</param>
        /// <param name="county">The county.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="routingNumber">The routing number.</param>
        /// <param name="accountNumber">The account number.</param>
        private void Giving_GiveNow_Populate_Step1(int churchId, string type, IEnumerable<dynamic> contributionData, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, string country, string streetOne, string streetTwo, string city, string state, string postalCode, string county, string phoneNumber, string routingNumber, string accountNumber, string validFromMonth, string validFromYear) {
            if (type == "Credit Card") {

                if (churchId == 15) {
                    // Select credit card
                    this._selenium.Click("payment_method_cc");
                }

                // Specify the fund / pledge drive information
                this.Giving_PopulateFundSubFundAmountInformation(contributionData);

                // Always enter a new credit card
                if (this._selenium.IsElementPresent("new_card")) {
                    this._selenium.Click("new_card");
                }

                // Specify the personal information
                this._selenium.Type("FirstName", firstName);
                this._selenium.Type("LastName", lastName);
                this._selenium.Select("payment_type_id", creditCardType);
                this._selenium.Type("cc_account_number", creditCardNumber);

                // Non international
                if ((creditCardType == "Visa") || (creditCardType == "Master Card") || (creditCardType == "American Express") || (creditCardType == "Discover") || (creditCardType == "JCB")) {
                    this._selenium.Select("expiration_month", expirationMonth);
                    this._selenium.Select("expiration_year", expirationYear);
                    this._selenium.Type("CVC", securityCode);
                }

                // International
                //else if ((creditCardType == "Switch") || (creditCardType == "Solo")) {
                //    this._selenium.Select("expiration_month", expirationMonth);
                //    this._selenium.Select("expiration_year", expirationYear);
                //    this._selenium.Select("valid_from", validFromMonth);
                //    this._selenium.Select("valid_from_year", validFromYear);
                //    this._selenium.Type("IssueNumber", securityCode);
                //}


                // Update the billing address
                if (!string.IsNullOrEmpty(country)) {
                    this._selenium.Select("Country", country);
                }

                if (!string.IsNullOrEmpty(streetOne)) {
                    this._selenium.Type("Address1", streetOne);
                }

                if (!string.IsNullOrEmpty(streetTwo)) {
                    this._selenium.Type("Address2", streetTwo);
                }

                if (!string.IsNullOrEmpty(city)) {
                    this._selenium.Type("City", city);
                }

                if (!string.IsNullOrEmpty(state)) {
                    this._selenium.Select("state", state);
                }

                if (!string.IsNullOrEmpty(postalCode)) {
                    this._selenium.Type("PostalCode", postalCode);
                }

                if (!string.IsNullOrEmpty(county)) {
                    this._selenium.Type("County", county);
                }
            }
            else {
                if (churchId != 254)
                {
                    // Select echeck
                    this._selenium.Click("payment_method_check");
                }

                // Specify the fund / pledge drive information
                this.Giving_PopulateFundSubFundAmountInformation(contributionData);

                // Enter the check information
                if (!string.IsNullOrEmpty(phoneNumber)) {
                    this._selenium.Type("phone", phoneNumber);
                }

                if (!string.IsNullOrEmpty(routingNumber)) {
                    this._selenium.Type("routing_number", routingNumber);
                }

                if (!string.IsNullOrEmpty(accountNumber)) {
                    this._selenium.Type(GeneralInFellowship.Giving.AccountNumber, accountNumber);
                }
            }
        }

        /// <summary>
        /// Populates the fields on step 1 of the Give Now Wizard
        /// </summary>
        /// <param name="churchId">The church ID</param>
        /// <param name="type">The type of contribution (Credit card or Personal Check)</param>
        /// <param name="contributionData">A collection of funds, subfunds, and amounts to give to.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="creditCardType">The credit card type.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <param name="securityCode">The security code.</param>
        /// <param name="country">The country</param>
        /// <param name="streetOne">The street one address.</param>
        /// <param name="streetTwo">The street two address.</param>
        /// <param name="city">The city.</param>
        /// <param name="state">The state.</param>
        /// <param name="postalCode">The postal code.</param>
        /// <param name="county">The county.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="routingNumber">The routing number.</param>
        /// <param name="accountNumber">The account number.</param>
        private void Giving_GiveNow_Populate_Step1_WebDriver(int churchId, string type, IEnumerable<dynamic> contributionData, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, string country, string streetOne, string streetTwo, string city, string state, string postalCode, string county, string phoneNumber, string routingNumber, string accountNumber, string validFromMonth, string validFromYear) {

            if (type == "Credit Card")
            {
                #region Credit Card
                if (churchId == 15) {
                    // Select credit card
                    this._driver.FindElementById("payment_method_cc").Click();
                }

                // Specify the fund / pledge drive information
                this.Giving_PopulateFundSubFundAmountInformation_WebDriver(contributionData);

                // Always enter a new credit card
                if (this._generalMethods.IsElementPresentWebDriver(By.Id("new_card"))) {
                    this._driver.FindElementById("new_card").Click();
                }

                this._generalMethods.WaitForElement(this._driver, By.Id("FirstName"));

                // Specify the personal information
                this._driver.FindElementById("FirstName").Clear();
                this._driver.FindElementById("FirstName").SendKeys(firstName);
                this._driver.FindElementById("LastName").Clear();
                this._driver.FindElementById("LastName").SendKeys(lastName);
                new SelectElement(this._driver.FindElementById("payment_type_id")).SelectByText(creditCardType);
                this._driver.FindElementById("cc_account_number").SendKeys(creditCardNumber);

                // Non international
                if ((creditCardType == "Visa") || (creditCardType == "Master Card") || (creditCardType == "American Express") || (creditCardType == "Discover") || (creditCardType == "JCB")) {
                    new SelectElement(this._driver.FindElementById("expiration_month")).SelectByText(expirationMonth);
                    new SelectElement(this._driver.FindElementById("expiration_year")).SelectByText(expirationYear);
                    this._driver.FindElementById("CVC").SendKeys(securityCode);
                }

                // International
                //else if ((creditCardType == "Switch") || (creditCardType == "Solo")) {
                //    new SelectElement(this._driver.FindElementById("expiration_month")).SelectByText(expirationMonth);
                //    new SelectElement(this._driver.FindElementById("expiration_year")).SelectByText(expirationYear);
                //    new SelectElement(this._driver.FindElementById("valid_from")).SelectByText(validFromMonth);
                //    new SelectElement(this._driver.FindElementById("valid_from_year")).SelectByText(validFromYear);
                //    this._driver.FindElementById("IssueNumber").SendKeys(securityCode);
                //}


                // Update the billing address
                // Store the Billing address information for verification purposes on Step 2
                if (!string.IsNullOrEmpty(country))
                {
                    new SelectElement(this._driver.FindElementById("Country")).SelectByText(country);
                }

                if (!string.IsNullOrEmpty(streetOne))
                {
                    this._driver.FindElementById("Address1").Clear();
                    this._driver.FindElementById("Address1").SendKeys(streetOne);
                }

                if (!string.IsNullOrEmpty(streetTwo))
                {
                    this._driver.FindElementById("Address2").Clear();
                    this._driver.FindElementById("Address2").SendKeys(streetTwo);
                }

                if (!string.IsNullOrEmpty(city))
                {
                    this._driver.FindElementById("City").Clear();
                    this._driver.FindElementById("City").SendKeys(city);
                }

                if (!string.IsNullOrEmpty(state))
                {
                    new SelectElement(this._driver.FindElementById("state")).SelectByText(state);
                }

                if (!string.IsNullOrEmpty(postalCode))
                {
                    this._driver.FindElementById("PostalCode").Clear();
                    this._driver.FindElementById("PostalCode").SendKeys(postalCode);
                }

                if (!string.IsNullOrEmpty(county))
                {
                    this._driver.FindElementById("County").SendKeys(county);
                }
                #endregion Credit Card
            }
            else {
                // Select echeck
                //this._driver.FindElementById("payment_method_check").Click();

                // Specify the fund / pledge drive information
                this.Giving_PopulateFundSubFundAmountInformation_WebDriver(contributionData);

                // Enter the check information
                if (!string.IsNullOrEmpty(phoneNumber)) {
                    System.Threading.Thread.Sleep(4000);
                    this._driver.FindElementByXPath(GeneralInFellowship.Giving.PhoneNumber).Clear();
                    this._driver.FindElementByXPath(GeneralInFellowship.Giving.PhoneNumber).SendKeys(phoneNumber);
                }

                if (!string.IsNullOrEmpty(routingNumber)) {
                    System.Threading.Thread.Sleep(4000);
                    this._driver.FindElementByXPath(GeneralInFellowship.Giving.RoutingNumber).Clear();
                    this._driver.FindElementByXPath(GeneralInFellowship.Giving.RoutingNumber).SendKeys(routingNumber);
                }

                if (!string.IsNullOrEmpty(accountNumber)) {
                    System.Threading.Thread.Sleep(4000);
                    this._driver.FindElementByXPath(GeneralInFellowship.Giving.AccountNumber).Clear();
                    this._driver.FindElementByXPath(GeneralInFellowship.Giving.AccountNumber).SendKeys(accountNumber);
                }
            }


        }

        #endregion Give Now

        #region Scheduled Giving

        /// <summary>
        /// Processes a schedule giving through the Schedule Giving Wizard.  Universal for Credit Cards and Personal Checks
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="paymentType">The payment type.</param>
        /// <param name="frequency">The frequency that this schedule occurs.</param>
        /// <param name="contributionData">The contribution data (funds, subfunds, amounts) stored an anonymous type.</param>
        /// <param name="dates">The dates for giving stored in anonymous type.</param>
        /// <param name="firstName">The first name on the credit card.</param>
        /// <param name="lastName">T</param>
        /// <param name="creditCardType">The type of credit card.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month for the credit card.</param>
        /// <param name="expirationYear">The expiration year for the credit card.</param>
        /// <param name="securityCode">The security code.</param>
        /// <param name="country">The country for the billing address.</param>
        /// <param name="streetOne">Street One for the billing address.</param>
        /// <param name="streetTwo">Street Two for the billing address.</param>
        /// <param name="city">The city for the billing address.</param>
        /// <param name="state">The state for the billing address</param>
        /// <param name="postalCode">The postal code for the billing address.</param>
        /// <param name="county">The county for the billing address.</param>
        /// <param name="phoneNumber">The phone number to be used for a Personal Check.</param>
        /// <param name="routingNumber">The routing number to be used for a Personal Check.</param>
        /// <param name="accountNumber">The account number to be used for a Personal Check.</param>
        /// <param name="agree">Specifies if you agree for the payment to be processed.</param>
        private void Giving_ScheduleGiving_Process(int churchId, string paymentType, GeneralEnumerations.GivingFrequency frequency, IEnumerable<dynamic> contributionData, dynamic dates, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, string country, string streetOne, string streetTwo, string city, string state, string postalCode, string county, string phoneNumber, string routingNumber, string accountNumber, bool agree) {

            // Store the last four digits and the payment name
            var lastFourDigits = string.Empty;
            var paymentName = string.Empty;

            var parms = contributionData.FirstOrDefault();

            string amount = parms == null ? string.Empty : parms.Amount;
            string fund = parms == null ? string.Empty : parms.FundOrPledgeDrive;
            string subFund = parms == null ? string.Empty : parms.SubFund;

            if (!string.IsNullOrEmpty(creditCardNumber)) {
                lastFourDigits = creditCardNumber.Substring(creditCardNumber.Length - 4);
                paymentName = creditCardType;
            }
            else if (!string.IsNullOrEmpty(accountNumber)) {
                lastFourDigits = accountNumber.Substring(accountNumber.Length - 4);
                paymentName = "Personal Check";
            }

            // View Scheduled Giving
            this._selenium.ClickAndWaitForPageToLoad("link=Your Giving");
            this._selenium.ClickAndWaitForPageToLoad("link=Schedule Giving");

            // Step 1 0- Contribution Information
            // Specify the contribution information
            this.Giving_PopulateFundSubFundAmountInformation(contributionData);

            // Store the total contribution
            var total = this._selenium.GetText("total_sum");

            // Submit
            //this._selenium.ClickAndWaitForPageToLoad("//input[@value='Continue >>']");
            this._selenium.ClickAndWaitForPageToLoad("//button[@id='commit']");

            // Unless there is validation on step 1, continue to step 2
            if (!this._selenium.IsElementPresent("//div[@class='error_msgs_for']")) {

                // Step 2 - Frequency
                // Determine the frequency and populate the appropriate dates
                switch (frequency) {
                    case GeneralEnumerations.GivingFrequency.Once:
                        this._selenium.Click("frequency_0");
                        this._selenium.Type("one_time_process_date", dates.OneTimeDate.ToString());
                        break;
                    case GeneralEnumerations.GivingFrequency.Monthly:
                        this.Giving_ScheduledGiving_Populate_MonthlyFrequency(dates);
                        break;
                    case GeneralEnumerations.GivingFrequency.TwiceMonthly:
                        this.Giving_ScheduledGiving_Populate_TwiceMonthlyFrequency(dates);
                        break;
                    case GeneralEnumerations.GivingFrequency.Weekly:
                        this.Giving_ScheduledGiving_Populate_WeeklyFrequency(dates);
                        break;
                    case GeneralEnumerations.GivingFrequency.EveryTwoWeeks:
                        this.Giving_ScheduledGiving_Populate_EveryTwoWeeksFrequency(dates);
                        break;
                    default:
                        break;
                }

                // Submit
                this._selenium.ClickAndWaitForPageToLoad("//button[@id='commit']");

                // Unless there is validation on Step 2, continue to step 3
                if (!this._selenium.IsElementPresent("//div[@class='error_msgs_for']")) {

                    // Step 3 - Payment Information
                    this.Giving_ScheduledGiving_Populate_PaymentInformation(paymentType, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, country, streetOne, streetTwo, city, state, postalCode, county, phoneNumber, routingNumber, accountNumber);

                    // Submit
                    this._selenium.ClickAndWaitForPageToLoad("//button[@id='commit']");

                    // Unless there is validation on Step 3, verify the schedule is created and exists
                    if (!this._selenium.IsElementPresent("//div[@class='error_msgs_for']")) {

                        // Step 4 - Review
                        
                        // FO-2942 verify ACH Auth Message  
                        if(paymentType == "Personal Check")
                        {
                        string msg = this._selenium.GetText("//div[@id='echeck_auth_message']");

                        String format = "MMMM dd, yyyy";

                        switch (frequency)
                        {
                            case GeneralEnumerations.GivingFrequency.Once:
                                var culture = new CultureInfo("en-us", true);
                                var dt = DateTime.ParseExact(dates.OneTimeDate, "MM/dd/yyyy", culture);
                                var enDateStr = dt.ToString("MMMM dd, yyyy");
                                TestLog.WriteLine(msg);
                                TestLog.WriteLine("By clicking the button below, I authorize Active Network, LLC to charge my bank account on " + enDateStr + " for the amount of $" + amount + " for " + fund + subFund + " .");
                                Assert.AreEqual(msg, "By clicking the button below, I authorize Active Network, LLC to charge my bank account on " + enDateStr + " for the amount of $" + amount + " for " + fund + subFund + " .");
                                break;
                            case GeneralEnumerations.GivingFrequency.Monthly:
                                string monthlyDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddMonths(2), TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(format, DateTimeFormatInfo.InvariantInfo);                            
                                Assert.AreEqual(msg, "By clicking the button below, I authorize Active Network, LLC to charge my bank account starting on " + monthlyDate + " of each month for the amount of $" + amount + " for " + fund + subFund + " .");
                                break;
                            case GeneralEnumerations.GivingFrequency.TwiceMonthly:
                                string amount1 = String.Format("{0:N2}", Convert.ToDouble(amount));
                                string startDate = Convert.ToString(dates.StartDay);
                                if (startDate == "1")
                                {
                                    DateTime date1 = new DateTime(Convert.ToInt16(DateTime.Now.AddMonths(1).Year), Convert.ToInt16(DateTime.UtcNow.AddMonths(1).Month), 1);
                                    string monthDate = date1.ToString("MMMM dd, yyyy", DateTimeFormatInfo.InvariantInfo);
                                    Assert.AreEqual(msg, "By clicking the button below, I authorize Active Network, LLC to charge my bank account starting on " + monthDate + " and on 1st and 16th of each month for the amount of $" + amount1 + " for " + fund + subFund + " .");
                                }
                                else
                                {
                                    DateTime date1 = new DateTime(Convert.ToInt16(DateTime.Now.AddMonths(1).Year), Convert.ToInt16(DateTime.UtcNow.AddMonths(1).Month), 16);
                                    string monthDate = date1.ToString("MMMM dd, yyyy", DateTimeFormatInfo.InvariantInfo);
                                    Assert.AreEqual(msg, "By clicking the button below, I authorize Active Network, LLC to charge my bank account starting on " + monthDate + " and on 1st and 16th of each month for the amount of $" + amount1 + " for " + fund + subFund + " .");
                                }
                                
                                break;
                            case GeneralEnumerations.GivingFrequency.Weekly:
                                
                                string StartDay = Convert.ToString(dates.StartDay);
                                var culture1 = new CultureInfo("en-us", true);
                                var dt1 = DateTime.ParseExact(dates.StartDate, "MM/dd/yyyy", culture1);
                                var StartDate = dt1.ToString("MMMM dd, yyyy");
                                Assert.AreEqual(msg, "By clicking the button below, I authorize Active Network, LLC to charge my bank account starting on " + StartDate + " and on the " + StartDay + " of each week for the amount of $" + amount + " for " + fund + subFund + " .");
                                break;
                            case GeneralEnumerations.GivingFrequency.EveryTwoWeeks:
                                string StartDay2 = Convert.ToString(dates.StartDay);
                                var culture2 = new CultureInfo("en-us", true);
                                var dt2 = DateTime.ParseExact(dates.StartDate, "MM/dd/yyyy", culture2);
                                var StartDate2 = dt2.ToString("MMMM dd, yyyy");
                                Assert.AreEqual(msg, "By clicking the button below, I authorize Active Network, LLC to charge my bank account starting on " + StartDate2 + " and on the " + StartDay2 + " of every other week for the amount of $" + amount + " for " + fund + subFund + " .");
                                break;
                            default:
                                break;
                        }
                        }
                        
                        
                        if (agree) {
                            TestLog.WriteLine("Authorize Withdrawl");
                            this._selenium.Click("//input[@id='authorize_withdrawl']");
                            TestLog.WriteLine("Authorize Withdrawl: " + this._selenium.IsChecked("authorize_withdrawl"));

                            if (!this._selenium.IsChecked("authorize_withdrawl"))
                            {
                                Assert.Fail("Authorize Withdrawl Check Box not checked!!!");
                            }
                        }

                        // Submit the payment
                        this._selenium.ClickAndWaitForPageToLoad("//button[@id='commit']");

                        // Unless there is validation on Step 4, verify the schedule is created and exists
                        if (!this._selenium.IsElementPresent("//div[@class='error_msgs_for']")) {
                            // Verify the sucess message is present
                            this._selenium.VerifyTextPresent("Schedule Created Successfully");

                            // Verify the scheduled giving page
                            this.Giving_ScheduleGiving_VerifyScheduleExists(churchId, frequency, dates, total, paymentName, lastFourDigits);
                        }
                        // Validation on Step 4 occured
                        #region Step 4 Validation
                        else {
                            // Unless we agreed to the payment, verify the proper validation.
                            if (!agree) {
                                this._selenium.VerifyTextPresent("You must agree to the recurring giving conditions. Check the box that begins with \"I understand...\"");
                            }
                            else {
                                // Payment did not process for some unknown reason.                   
                                log.Debug("Payment did not process for some unknown reason.");
                                Assert.Fail(string.Format("{0}", this._selenium.GetText("//div[@class='error_msgs_for']")));                    
                                //Assert.Fail("An unexpected error occured while creating this schedule!!!");
                            }

                            // Cancel out of the wizard
                            this._selenium.ClickAndWaitForPageToLoad("link=Cancel");
                        }
                        #endregion Step 4 Validation
                    }
                    // Validation on Step 3 occured
                    #region Step 3 Validation
                    else {
                        if (paymentType == "Credit Card") {
                            if (string.IsNullOrEmpty(creditCardType)) {
                                this._selenium.VerifyTextPresent("Credit card type is required");
                            }

                            if (string.IsNullOrEmpty(creditCardNumber)) {
                                this._selenium.VerifyTextPresent("Credit card number is required");
                            }

                            else if (creditCardNumber.Length > 30) {
                                this._selenium.VerifyTextPresent("Credit card cannot exceed 30 characters");
                            }

                            if (string.IsNullOrEmpty(firstName)) {
                                this._selenium.VerifyTextPresent("First name is required");
                            }

                            else if (firstName.Length > 50) {
                                this._selenium.VerifyTextPresent("First name cannot exceed 50 characters");
                            }

                            if (string.IsNullOrEmpty(lastName)) {
                                this._selenium.VerifyTextPresent("Last name is required");
                            }

                            else if (lastName.Length > 50) {
                                this._selenium.VerifyTextPresent("Last name cannot exceed 50 characters");
                            }

                            if (string.IsNullOrEmpty(expirationMonth)) {
                                this._selenium.VerifyTextPresent("Please select a valid expiration month");
                            }

                            if (string.IsNullOrEmpty(expirationYear)) {
                                this._selenium.VerifyTextPresent("Please select a valid expiration year");
                            }
                        }
                        else {
                            if (string.IsNullOrEmpty(phoneNumber)) {
                                this._selenium.VerifyTextPresent("Phone number is required");
                            }

                            else if (phoneNumber.Length > 50) {
                                this._selenium.VerifyTextPresent("Phone number cannot exceed 50 characters");
                            }

                            if (string.IsNullOrEmpty(accountNumber)) {
                                this._selenium.VerifyTextPresent("Account number is required");
                            }

                            else if (accountNumber.Length > 30) {
                                this._selenium.VerifyTextPresent("Account number cannot exceed 30 characters");
                            }

                            if (string.IsNullOrEmpty(routingNumber)) {
                                this._selenium.VerifyTextPresent("Bank routing number is required");
                            }

                            // If the routing number is greater than 9 digits, verify the correct validation message
                            else if (routingNumber.Length > 9) {
                                this._selenium.VerifyTextPresent("Please provide a valid nine-digit routing number");
                            }

                            // If the routing number is less than 9 digits, verify the correct validation message
                            else if (routingNumber.Length < 9) {
                                this._selenium.VerifyTextPresent("Please provide a valid nine-digit routing number");
                            }
                        }

                        // Exit out of the wizard
                        TestLog.WriteLine("Exiting out of Wizard");
                        this._selenium.ClickAndWaitForPageToLoad("link=« Back");
                        this._selenium.ClickAndWaitForPageToLoad("link=« Back");
                        this._selenium.Click("link=Cancel");

                    }
                    #endregion Step 3 Validation
                }
                // Validation on Step 2 occured
                #region Step 2 Validation
                else {
                    switch (frequency) {
                        case GeneralEnumerations.GivingFrequency.Once:
                            this._selenium.VerifyTextPresent("Please enter a valid date");
                            break;
                        case GeneralEnumerations.GivingFrequency.Monthly:
                            if (string.IsNullOrEmpty(dates.StartDay)) {
                                this._selenium.VerifyTextPresent("Please select a valid Begins Day");
                            }
                            if (string.IsNullOrEmpty(dates.StartMonth)) {
                                this._selenium.VerifyTextPresent("Please select a valid Begins Month");
                            }
                            if (string.IsNullOrEmpty(dates.StartYear)) {
                                this._selenium.VerifyTextPresent("Please select a valid Begins Year");
                            }
                            break;
                        case GeneralEnumerations.GivingFrequency.TwiceMonthly:
                            if (string.IsNullOrEmpty(dates.StartDay)) {
                                this._selenium.VerifyTextPresent("Please select a valid Begins Day");
                            }
                            if (string.IsNullOrEmpty(dates.StartMonth)) {
                                this._selenium.VerifyTextPresent("Please select a valid Begins Month");
                            }
                            if (string.IsNullOrEmpty(dates.StartYear)) {
                                this._selenium.VerifyTextPresent("Please select a valid Begins Year");
                            }
                            break;
                        case GeneralEnumerations.GivingFrequency.Weekly:
                            if (string.IsNullOrEmpty(dates.StartDay)) {
                                this._selenium.VerifyTextPresent("Please select a weekday");
                            }
                            if (string.IsNullOrEmpty(dates.StartDate)) {
                                this._selenium.VerifyTextPresent("Please enter a valid date for Begins");
                            }
                            break;
                        case GeneralEnumerations.GivingFrequency.EveryTwoWeeks:
                            if (string.IsNullOrEmpty(dates.StartDay)) {
                                this._selenium.VerifyTextPresent("Please select a weekday");
                            }
                            if (string.IsNullOrEmpty(dates.StartDate)) {
                                this._selenium.VerifyTextPresent("Please enter a valid date for Begins");
                            }
                            break;
                    }

                    // Cancel out of the wizard
                    this._selenium.ClickAndWaitForPageToLoad("link=« Back");
                    this._selenium.Click("link=Cancel");
                }
                #endregion Step 2 Validation
            }

            // Validation on Step 1 occured
            #region Step 1 Validation
            else {
                if (contributionData.Any(contribution => string.IsNullOrEmpty(contribution.FundOrPledgeDrive))) {
                    this._selenium.VerifyTextPresent("Give to... is required");
                }
                else {
                    // If the fund or pledge drive is provided, verify amount is provided.
                    if (contributionData.Any(contribution => string.IsNullOrEmpty(contribution.Amount))) {
                        this._selenium.VerifyTextPresent("Please enter a valid amount");
                    }
                }

                // Cancel out of the wizard
                this._selenium.ClickAndWaitForPageToLoad("link=Cancel");
            }
            #endregion Step 1 Validation


        }

        /// <summary>
        /// Processes an one time schedule giving of today through the Schedule Giving Wizard.  Universal for Credit Cards and Personal Checks
        /// Created by Jim
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="paymentType">The payment type.</param>
        /// <param name="contributionData">The contribution data (funds, subfunds, amounts) stored an anonymous type.</param>
        /// <param name="dates">The dates for giving stored in anonymous type.</param>
        /// <param name="firstName">The first name on the credit card.</param>
        /// <param name="lastName">T</param>
        /// <param name="creditCardType">The type of credit card.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month for the credit card.</param>
        /// <param name="expirationYear">The expiration year for the credit card.</param>
        /// <param name="securityCode">The security code.</param>
        /// <param name="country">The country for the billing address.</param>
        /// <param name="streetOne">Street One for the billing address.</param>
        /// <param name="streetTwo">Street Two for the billing address.</param>
        /// <param name="city">The city for the billing address.</param>
        /// <param name="state">The state for the billing address</param>
        /// <param name="postalCode">The postal code for the billing address.</param>
        /// <param name="county">The county for the billing address.</param>
        /// <param name="phoneNumber">The phone number to be used for a Personal Check.</param>
        /// <param name="routingNumber">The routing number to be used for a Personal Check.</param>
        /// <param name="accountNumber">The account number to be used for a Personal Check.</param>
        /// <param name="agree">Specifies if you agree for the payment to be processed.</param>
        private void Giving_ScheduleGiving_OneTime_Of_Today_Process_WebDriver(int churchId, string paymentType, IEnumerable<dynamic> contributionData, dynamic dates, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, string country, string streetOne, string streetTwo, string city, string state, string postalCode, string county, string phoneNumber, string routingNumber, string accountNumber, bool agree)
        {

            // Store the last four digits and the payment name
            var lastFourDigits = string.Empty;
            var paymentName = string.Empty;

            if (!string.IsNullOrEmpty(creditCardNumber))
            {
                lastFourDigits = creditCardNumber.Substring(creditCardNumber.Length - 4);
                paymentName = creditCardType;
            }
            else if (!string.IsNullOrEmpty(accountNumber))
            {
                lastFourDigits = accountNumber.Substring(accountNumber.Length - 4);
                paymentName = "Personal Check";
            }

            // View Scheduled Giving
            this._driver.FindElementByLinkText("Your Giving").Click();
            this._driver.FindElementByLinkText("Schedule Giving").Click();

            // Step 1 0- Contribution Information
            // Specify the contribution information
            this.ScheduledGiving_PopulateFundSubFundAmountInformation_WebDriver(contributionData);

            // Store the total contribution
            var total = this._driver.FindElementById("total_sum").Text;

            // Submit
            //this._driver.FindElementByXPath("//button[@value='Continue']").Click();
            this._driver.FindElementById("commit").Click();

            // Unless there is validation on step 1, continue to step 2
            if (!this._generalMethods.IsElementPresentWebDriver(By.XPath("//div[@class='error_msgs_for']")))
            {

                // Step 2 - Frequency
                this._driver.FindElementById("frequency_0").Click();
                this._driver.FindElementById("one_time_process_date").SendKeys(dates.OneTimeDate.ToString());

                // Submit
                this._driver.FindElementById("commit").Click();

                // Unless there is validation on Step 2, continue to step 3
                if (!this._generalMethods.IsElementPresentWebDriver(By.XPath("//div[@class='error_msgs_for']")))
                {

                    // Step 3 - Payment Information
                    this.Giving_ScheduledGiving_Populate_PaymentInformation_WebDriver(paymentType, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, country, streetOne, streetTwo, city, state, postalCode, county, phoneNumber, routingNumber, accountNumber);

                    // Submit
                    this._driver.FindElementById("commit").Click();

                    // Unless there is validation on Step 3, verify the schedule is created and exists
                    if (!this._generalMethods.IsElementPresentWebDriver(By.XPath("//div[@class='error_msgs_for']")))
                    {

                        // Step 4 - Review
                        if (agree)
                        {
                            TestLog.WriteLine("Authorize Withdrawl");
                            this._driver.FindElementById("authorize_withdrawl").Click();
                            TestLog.WriteLine("Authorize Withdrawl: " + this._driver.FindElementById("authorize_withdrawl").Selected);

                            if (!this._driver.FindElementById("authorize_withdrawl").Selected)
                            {
                                Assert.Fail("Authorize Withdrawl Check Box not checked!!!");
                            }
                        }

                        // Submit the payment
                        //this._driver.FindElementByXPath("//input[@type='submit' and @value='Process this schedule']").Click();
                        this._driver.FindElementById("commit").Click();

                        // Unless there is validation on Step 4, verify the schedule is created and exists
                        if (!this._generalMethods.IsElementPresentWebDriver(By.XPath("//div[@class='error_msgs_for']")))
                        {
                            // Verify the sucess message is present
                            this._generalMethods.VerifyTextPresentWebDriver("One Time Payment Created Succesfully");
                        }
                        else
                        {
                            TestLog.WriteLine("Error occurred on step 4!");
                        }
                    }
                    else 
                    {
                        TestLog.WriteLine("Error occurred on step 3");
                    }
                }
                else
                {
                    TestLog.WriteLine("Error occurred on step 2");
                }
            }
            else
            {
                TestLog.WriteLine("Error occurred on step 1");
            }
        }

        /// <summary>
        /// Processes a schedule giving through the Schedule Giving Wizard.  Universal for Credit Cards and Personal Checks
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="paymentType">The payment type.</param>
        /// <param name="frequency">The frequency that this schedule occurs.</param>
        /// <param name="contributionData">The contribution data (funds, subfunds, amounts) stored an anonymous type.</param>
        /// <param name="dates">The dates for giving stored in anonymous type.</param>
        /// <param name="firstName">The first name on the credit card.</param>
        /// <param name="lastName">T</param>
        /// <param name="creditCardType">The type of credit card.</param>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expirationMonth">The expiration month for the credit card.</param>
        /// <param name="expirationYear">The expiration year for the credit card.</param>
        /// <param name="securityCode">The security code.</param>
        /// <param name="country">The country for the billing address.</param>
        /// <param name="streetOne">Street One for the billing address.</param>
        /// <param name="streetTwo">Street Two for the billing address.</param>
        /// <param name="city">The city for the billing address.</param>
        /// <param name="state">The state for the billing address</param>
        /// <param name="postalCode">The postal code for the billing address.</param>
        /// <param name="county">The county for the billing address.</param>
        /// <param name="phoneNumber">The phone number to be used for a Personal Check.</param>
        /// <param name="routingNumber">The routing number to be used for a Personal Check.</param>
        /// <param name="accountNumber">The account number to be used for a Personal Check.</param>
        /// <param name="agree">Specifies if you agree for the payment to be processed.</param>
        private void Giving_ScheduleGiving_Process_WebDriver(int churchId, string paymentType, GeneralEnumerations.GivingFrequency frequency, IEnumerable<dynamic> contributionData, dynamic dates, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, string country, string streetOne, string streetTwo, string city, string state, string postalCode, string county, string phoneNumber, string routingNumber, string accountNumber, bool agree)
        {

            // Store the last four digits and the payment name
            var lastFourDigits = string.Empty;
            var paymentName = string.Empty;

            if (!string.IsNullOrEmpty(creditCardNumber))
            {
                lastFourDigits = creditCardNumber.Substring(creditCardNumber.Length - 4);
                paymentName = creditCardType;
            }
            else if (!string.IsNullOrEmpty(accountNumber))
            {
                lastFourDigits = accountNumber.Substring(accountNumber.Length - 4);
                paymentName = "Personal Check";
            }

            // View Scheduled Giving
            this._driver.FindElementByLinkText("Your Giving").Click();
            this._driver.FindElementByLinkText("Schedule Giving").Click();

            // Step 1 0- Contribution Information
            // Specify the contribution information
            this.ScheduledGiving_PopulateFundSubFundAmountInformation_WebDriver(contributionData);

            // Store the total contribution
            var total = this._driver.FindElementById("total_sum").Text;

            // Submit
            //this._driver.FindElementByXPath("//button[@value='Continue']").Click();
            this._driver.FindElementById("commit").Click();
      
            // Unless there is validation on step 1, continue to step 2
            if (!this._generalMethods.IsElementPresentWebDriver(By.XPath("//div[@class='error_msgs_for']")))
            {

                // Step 2 - Frequency
                // Determine the frequency and populate the appropriate dates
                switch (frequency)
                {
                    case GeneralEnumerations.GivingFrequency.Once:
                        this._driver.FindElementById("frequency_0").Click();
                        this._driver.FindElementById("one_time_process_date").SendKeys(dates.OneTimeDate.ToString());
                        break;
                    case GeneralEnumerations.GivingFrequency.Monthly:
                        this.Giving_ScheduledGiving_Populate_MonthlyFrequency_WebDriver(dates);
                        break;
                    case GeneralEnumerations.GivingFrequency.TwiceMonthly:
                        this.Giving_ScheduledGiving_Populate_TwiceMonthlyFrequency_WebDriver(dates);
                        break;
                    case GeneralEnumerations.GivingFrequency.Weekly:
                        this.Giving_ScheduledGiving_Populate_WeeklyFrequency_WebDriver(dates);
                        break;
                    case GeneralEnumerations.GivingFrequency.EveryTwoWeeks:
                        this.Giving_ScheduledGiving_Populate_EveryTwoWeeksFrequency_WebDriver(dates);
                        break;
                    default:
                        break;
                }

                // Submit
                this._driver.FindElementById("commit").Click();

                // Unless there is validation on Step 2, continue to step 3
                if (!this._generalMethods.IsElementPresentWebDriver(By.XPath("//div[@class='error_msgs_for']")))
                {

                    // Step 3 - Payment Information
                    this.Giving_ScheduledGiving_Populate_PaymentInformation_WebDriver(paymentType, firstName, lastName, creditCardType, creditCardNumber, expirationMonth, expirationYear, securityCode, country, streetOne, streetTwo, city, state, postalCode, county, phoneNumber, routingNumber, accountNumber);

                    // Submit
                    this._driver.FindElementById("commit").Click();

                    // Unless there is validation on Step 3, verify the schedule is created and exists
                    if (!this._generalMethods.IsElementPresentWebDriver(By.XPath("//div[@class='error_msgs_for']")))
                    {

                        // Step 4 - Review
                        if (agree)
                        {
                            TestLog.WriteLine("Authorize Withdrawl");
                            this._driver.FindElementById("authorize_withdrawl").Click();
                            TestLog.WriteLine("Authorize Withdrawl: " + this._driver.FindElementById("authorize_withdrawl").Selected);

                            if (!this._driver.FindElementById("authorize_withdrawl").Selected)
                            {
                                Assert.Fail("Authorize Withdrawl Check Box not checked!!!");
                            }
                        }

                        // Submit the payment
                        //this._driver.FindElementByXPath("//input[@type='submit' and @value='Process this schedule']").Click();
                        this._driver.FindElementById("commit").Click();

                        // Unless there is validation on Step 4, verify the schedule is created and exists
                        if (!this._generalMethods.IsElementPresentWebDriver(By.XPath("//div[@class='error_msgs_for']")))
                        {
                            // Verify the sucess message is present
                            this._generalMethods.VerifyTextPresentWebDriver("Schedule Created Successfully");

                            // Verify the scheduled giving page
                            this.Giving_ScheduleGiving_VerifyScheduleExists_WebDriver(churchId, frequency, dates, total, paymentName, lastFourDigits);
                        }
                        // Validation on Step 4 occured
                        #region Step 4 Validation
                        else
                        {
                            // Unless we agreed to the payment, verify the proper validation.
                            if (!agree)
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("You must agree to the recurring giving conditions. Check the box that begins with \"I understand...\"");
                            }
                            else
                            {
                                // Payment did not process for some unknown reason.                   
                                log.Debug("Payment did not process for some unknown reason.");
                                Assert.Fail(string.Format("{0}", this._driver.FindElementByXPath("//div[@class='error_msgs_for']").Text));
                                //Assert.Fail("An unexpected error occured while creating this schedule!!!");
                            }

                            // Cancel out of the wizard
                            this._driver.FindElementByLinkText(GeneralInFellowship.Giving.Cancel).Click();
                        }
                        #endregion Step 4 Validation
                    }
                    // Validation on Step 3 occured
                    #region Step 3 Validation
                    else
                    {
                        if (paymentType == "Credit Card")
                        {
                            if (string.IsNullOrEmpty(creditCardType))
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Credit card type is required");
                            }

                            if (string.IsNullOrEmpty(creditCardNumber))
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Credit card number is required");
                            }

                            else if (creditCardNumber.Length > 30)
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Credit card cannot exceed 30 characters");
                            }

                            if (string.IsNullOrEmpty(firstName))
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("First name is required");
                            }

                            else if (firstName.Length > 50)
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("First name cannot exceed 50 characters");
                            }

                            if (string.IsNullOrEmpty(lastName))
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Last name is required");
                            }

                            else if (lastName.Length > 50)
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Last name cannot exceed 50 characters");
                            }

                            if (string.IsNullOrEmpty(expirationMonth))
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Please select a valid expiration month");
                            }

                            if (string.IsNullOrEmpty(expirationYear))
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Please select a valid expiration year");
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(phoneNumber))
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Phone number is required");
                            }

                            else if (phoneNumber.Length > 50)
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Phone number cannot exceed 50 characters");
                            }

                            if (string.IsNullOrEmpty(accountNumber))
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Account number is required");
                            }

                            else if (accountNumber.Length > 30)
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Account number cannot exceed 30 characters");
                            }

                            if (string.IsNullOrEmpty(routingNumber))
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Bank routing number is required");
                            }

                            // If the routing number is greater than 9 digits, verify the correct validation message
                            else if (routingNumber.Length > 9)
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Please provide a valid nine-digit routing number");
                            }

                            // If the routing number is less than 9 digits, verify the correct validation message
                            else if (routingNumber.Length < 9)
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Please provide a valid nine-digit routing number");
                            }
                        }

                        // Exit out of the wizard
                        this._driver.FindElementByLinkText(GeneralInFellowship.Giving.Back);
                        this._driver.FindElementByLinkText(GeneralInFellowship.Giving.Back);
                        this._driver.FindElementByLinkText(GeneralInFellowship.Giving.Back);

                    }
                    #endregion Step 3 Validation
                }
                // Validation on Step 2 occured
                #region Step 2 Validation
                else
                {
                    switch (frequency)
                    {
                        case GeneralEnumerations.GivingFrequency.Once:
                            this._generalMethods.VerifyTextPresentWebDriver("Please enter a valid date");
                            break;
                        case GeneralEnumerations.GivingFrequency.Monthly:
                            if (string.IsNullOrEmpty(dates.StartDay))
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Please select a valid Begins Day");
                            }
                            if (string.IsNullOrEmpty(dates.StartMonth))
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Please select a valid Begins Month");
                            }
                            if (string.IsNullOrEmpty(dates.StartYear))
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Please select a valid Begins Year");
                            }
                            break;
                        case GeneralEnumerations.GivingFrequency.TwiceMonthly:
                            if (string.IsNullOrEmpty(dates.StartDay))
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Please select a valid Begins Day");
                            }
                            if (string.IsNullOrEmpty(dates.StartMonth))
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Please select a valid Begins Month");
                            }
                            if (string.IsNullOrEmpty(dates.StartYear))
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Please select a valid Begins Year");
                            }
                            break;
                        case GeneralEnumerations.GivingFrequency.Weekly:
                            if (string.IsNullOrEmpty(dates.StartDay))
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Please select a weekday");
                            }
                            if (string.IsNullOrEmpty(dates.StartDate))
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Please enter a valid date for Begins");
                            }
                            break;
                        case GeneralEnumerations.GivingFrequency.EveryTwoWeeks:
                            if (string.IsNullOrEmpty(dates.StartDay))
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Please select a weekday");
                            }
                            if (string.IsNullOrEmpty(dates.StartDate))
                            {
                                this._generalMethods.VerifyTextPresentWebDriver("Please enter a valid date for Begins");
                            }
                            break;
                    }

                    // Cancel out of the wizard
                    this._driver.FindElementByLinkText(GeneralInFellowship.Giving.Back);
                    this._driver.FindElementByLinkText(GeneralInFellowship.Giving.Back);

                }
                #endregion Step 2 Validation
            }

            // Validation on Step 1 occured
            #region Step 1 Validation
            else
            {
                if (contributionData.Any(contribution => string.IsNullOrEmpty(contribution.FundOrPledgeDrive)))
                {
                    this._generalMethods.VerifyTextPresentWebDriver("Give to... is required");
                }
                else
                {
                    // If the fund or pledge drive is provided, verify amount is provided.
                    if (contributionData.Any(contribution => string.IsNullOrEmpty(contribution.Amount)))
                    {
                        this._generalMethods.VerifyTextPresentWebDriver("Please enter a valid amount");
                    }
                }

                // Cancel out of the wizard
                this._driver.FindElementByLinkText(GeneralInFellowship.Giving.Back);

            }
            #endregion Step 1 Validation

        }

        /// <summary>
        /// Populates the funds, subfunds, and amounts for a give now
        /// </summary>
        /// <param name="contributionData">A collection of funds, subfunds, and amounts to give to.</param>
        private void Giving_PopulateFundSubFundAmountInformation(IEnumerable<dynamic> contributionData) {

            // Store the current row that you can input contribution details info,
            var currentRow = 2;

            // Populate the data for each fund, sub fund, and amount
            // Select the Fund
            int fundCounter = 0;

            foreach (var item in contributionData) {
                if (!string.IsNullOrEmpty(item.FundOrPledgeDrive)) {

                    // Unless the subfund is null or empty, select the fund then select the subfund attached to the fund.
                    if (!string.IsNullOrEmpty(item.SubFund)) {
                        this._selenium.Select(string.Format("{0}{1}", GeneralInFellowship.Giving.Fund, fundCounter), item.FundOrPledgeDrive);
                        Retry.WithPolling(5000).WithTimeout(100000).WithFailureMessage("Subfund was not present")
                            .Until(() => this._selenium.IsElementPresent("//select[@id='sub_fund']/option"));

                        this._selenium.Select("sub_fund", item.SubFund);
                    }
                    else
                        this._selenium.Select(string.Format("{0}{1}", GeneralInFellowship.Giving.Fund, fundCounter), item.FundOrPledgeDrive);

                    // Type the amount
                    //Terrible hack until Scheduled Giving is updated to responsive
                    TestLog.WriteLine("F1 Env: {0}", this._f1Environment);
                    TestLog.WriteLine("URL: {0}", this._selenium.GetLocation());

                    string environment = this._f1Environment.ToString();
                    /*
                    if (environment == "LV_QA")
                    {
                        if ((this._selenium.GetLocation() == "https://dc.qa.infellowship.com/OnlineGiving/ScheduledGiving/Step1") || (this._selenium.GetLocation() == "https://qaeunlx0c6.qa.infellowship.com/OnlineGiving/ScheduledGiving/Step1"))
                        {
                            this._selenium.Type(string.Format(string.Format("{0}/tbody/tr[{1}]/td[3]/input[@id='amount']", TableIds.InFellowship_ScheduledGiving_FundSubFundAmount, currentRow)), item.Amount);
                        }
                        else
                        {
                            this._selenium.Type(string.Format(string.Format("{0}/li[{1}]/div/div[3]/div/input[@id='amount']", TableIds.InFellowship_Giving_FundSubFundAmount, currentRow)), item.Amount);
                        }
                    }
                    else
                    {
                        if (this._selenium.GetLocation() == string.Format("https://dc.{0}.infellowship.com/OnlineGiving/ScheduledGiving/Step1", environment.ToLower()) || (this._selenium.GetLocation() == (string.Format("https://qaeunlx0c6.{0}.infellowship.com/OnlineGiving/ScheduledGiving/Step1", environment.ToLower()))))
                        {
                            this._selenium.Type(string.Format(string.Format("{0}/tbody/tr[{1}]/td[3]/input[@id='amount']", TableIds.InFellowship_ScheduledGiving_FundSubFundAmount, currentRow)), item.Amount);
                        }
                        else
                        {
                            this._selenium.Type(string.Format(string.Format("{0}/li[{1}]/div/div[3]/div/input[@id='amount']", TableIds.InFellowship_Giving_FundSubFundAmount, currentRow)), item.Amount);
                        }
                    }*/

                    this._selenium.Type("//input[@id='amount']", item.Amount);

                    this._selenium.Click("add_row");
                    currentRow++;

                    Retry.WithPolling(5000).WithTimeout(20000).WithFailureMessage("Subfund was not present")
                        .Until(() => this._selenium.IsElementPresent("amount"));

                    // Yay hack. Apparently the way we do a running total is weird and the javascript doesn't update the running total when selenium interacts with the browser.
                    this._selenium.TypeKeys("amount", "0");
                }
                fundCounter++;
            }
        }

        /// <summary>
        /// Populates the funds, subfunds, and amounts for a give now
        /// </summary>
        /// <param name="contributionData">A collection of funds, subfunds, and amounts to give to.</param>
        public void Giving_PopulateFundSubFundAmountInformation_WebDriver(IEnumerable<dynamic> contributionData) {

            // Select the Fund
            int fundCounter = 0;

            // Populate the data for each fund, sub fund, and amount
            foreach (var item in contributionData)
            {
                if (!string.IsNullOrEmpty(item.fund))
                {

                    // Unless the subfund is null or empty, select the fund then select the subfund attached to the fund.
                    if (!string.IsNullOrEmpty(item.subFund))
                    {
                        new SelectElement(this._driver.FindElementById(string.Format("{0}{1}", GeneralInFellowship.Giving.Fund, fundCounter))).SelectByText(item.fund);
                        Retry.WithPolling(5000).WithTimeout(20000).WithFailureMessage("Subfund was not present")
                            .Until(() => !this._generalMethods.IsElementPresentWebDriver(By.XPath(string.Format("//select[@id='{0}'] and [@disabled='disabled']", GeneralInFellowship.Giving.SubFund))));

                        new SelectElement(this._driver.FindElementById(GeneralInFellowship.Giving.SubFund)).SelectByText(item.subFund);

                    }
                    else
                    {
                        new SelectElement(this._driver.FindElementById(string.Format("{0}{1}", GeneralInFellowship.Giving.Fund, fundCounter))).SelectByText(item.fund);


                        // Retry.WithPolling(5000).WithTimeout(20000).WithFailureMessage("Subfund was not present")
                        //     .Until(() => this._generalMethods.IsElementPresentWebDriver(By.XPath("//table[@id='giving_table']/tbody/tr[" + currentRow + "]/td[3]/input[@id='amount']")));

                        // Yay hack. Apparently the way we do a running total is weird and the javascript doesn't update the running total when selenium interacts with the browser.
                        // this._driver.FindElementByXPath("//table[@id='giving_table']/tbody/tr[" + currentRow + "]/td[3]/input[@id='amount']").SendKeys("0");
                    }

                    // Type the amount
                    this._driver.FindElementByXPath(string.Format("//ul[@id='giving_table']/li[{0}]/div/div[3]/div/input[@id='{1}']", fundCounter + 2, GeneralInFellowship.Giving.Amount)).Clear();
                    this._driver.FindElementByXPath(string.Format("//ul[@id='giving_table']/li[{0}]/div/div[3]/div/input[@id='{1}']", fundCounter + 2, GeneralInFellowship.Giving.Amount)).SendKeys(item.amount);

                    if (contributionData.Count() > 1)
                    {
                        this._driver.FindElementByLinkText("Add another").Click();
                    }

                }
                fundCounter++;
            }
        }

        /// <summary>
        /// Populates the funds, subfunds, and amounts for a give now without account
        /// </summary>
        /// <param name="contributionData">A collection of funds, subfunds, and amounts to give to.</param>
        public void GivingWithoutAccount_PopulateFundSubFundAmountInformation_WebDriver(IEnumerable<dynamic> contributionData)
        {
            // Select the Fund
            int fundCounter = 0;

            foreach (var item in contributionData)
            {
                if (!string.IsNullOrEmpty(item.fund))
                {

                    // Unless the subfund is null or empty, select the fund then select the subfund attached to the fund.
                    if (!string.IsNullOrEmpty(item.subFund))
                    {
                        new SelectElement(this._driver.FindElementById(string.Format("{0}{1}", GeneralInFellowship.Giving.Fund, fundCounter))).SelectByText(item.fund);
                        Retry.WithPolling(5000).WithTimeout(20000).WithFailureMessage("Subfund was not present")
                            .Until(() => !this._generalMethods.IsElementPresentWebDriver(By.XPath(string.Format("//select[@id='{0}'] and [@disabled='disabled']", GeneralInFellowship.Giving.SubFund))));

                        new SelectElement(this._driver.FindElementById(GeneralInFellowship.Giving.SubFund)).SelectByText(item.subFund);

                        // Type the amount
                        //this._driver.FindElementByXPath(string.Format("//ul[@id='giving_table']/li[{0}]/div/div[3]/div/input[@id='{1}']", fundCounter +2, GeneralInFellowship.Giving.Amount)).SendKeys(item.amount);
                        //this._driver.FindElementById(GeneralInFellowship.Giving.Amount).SendKeys(item.amount);

                    }
                    else
                    {
                        new SelectElement(this._driver.FindElementById(string.Format("{0}{1}", GeneralInFellowship.Giving.Fund, fundCounter))).SelectByText(item.fund);

                        // Type the amount
                        //this._driver.FindElementByXPath(string.Format("//ul[@id='giving_table']/li[{0}]/div/div[3]/div/input[@id='{1}']", fundCounter + 2, GeneralInFellowship.Giving.Amount)).SendKeys(item.amount);
                        //this._driver.FindElementById(GeneralInFellowship.Giving.Amount).SendKeys(item.amount);

                        // Never Hack!!! I hacked this since normal way was not working but turns out it could be due to potential issue due to decimal input value
                        //IWebElement element = this._driver.FindElement(By.Id(GeneralInFellowship.Giving.Amount));
                        //IJavaScriptExecutor executor = (IJavaScriptExecutor)this._driver;
                        //executor.ExecuteScript("arguments[0].focus();", element);
                        //executor.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2])", element, "value", item.amount);
                    }

                    // Type the amount
                    this._driver.FindElementByXPath(string.Format("//ul[@id='giving_table']/li[{0}]/div/div[3]/div/input[@id='{1}']", fundCounter + 2, GeneralInFellowship.Giving.Amount)).Clear();
                    this._driver.FindElementByXPath(string.Format("//ul[@id='giving_table']/li[{0}]/div/div[3]/div/input[@id='{1}']", fundCounter + 2, GeneralInFellowship.Giving.Amount)).SendKeys(item.amount);

                    if (contributionData.Count() > 1)
                    {
                        this._driver.FindElementByLinkText("Add another").Click();
                    }

                }
                fundCounter++;
            }
        }

        /// <summary>
        /// Populates the funds, subfunds, and amounts for a give now
        /// </summary>
        /// <param name="contributionData">A collection of funds, subfunds, and amounts to give to.</param>
        public void ScheduledGiving_PopulateFundSubFundAmountInformation_WebDriver(IEnumerable<dynamic> contributionData)
        {

            // Select the Fund
            int fundCounter = 0;

            // Populate the data for each fund, sub fund, and amount
            foreach (var item in contributionData)
            {
                if (!string.IsNullOrEmpty(item.fund))
                {

                    // Unless the subfund is null or empty, select the fund then select the subfund attached to the fund.
                    if (!string.IsNullOrEmpty(item.subFund))
                    {
                        new SelectElement(this._driver.FindElementById(string.Format("{0}{1}", GeneralInFellowship.Giving.Fund, fundCounter))).SelectByText(item.fund);
                        Retry.WithPolling(5000).WithTimeout(20000).WithFailureMessage("Subfund was not present")
                            .Until(() => !this._generalMethods.IsElementPresentWebDriver(By.XPath(string.Format("//select[@id='{0}'] and [@disabled='disabled']", GeneralInFellowship.Giving.SubFund))));

                        new SelectElement(this._driver.FindElementById(GeneralInFellowship.Giving.SubFund)).SelectByText(item.subFund);

                    }
                    else
                    {
                        new SelectElement(this._driver.FindElementById(string.Format("{0}{1}", GeneralInFellowship.Giving.Fund, fundCounter))).SelectByText(item.fund);


                        // Retry.WithPolling(5000).WithTimeout(20000).WithFailureMessage("Subfund was not present")
                        //     .Until(() => this._generalMethods.IsElementPresentWebDriver(By.XPath("//table[@id='giving_table']/tbody/tr[" + currentRow + "]/td[3]/input[@id='amount']")));

                        // Yay hack. Apparently the way we do a running total is weird and the javascript doesn't update the running total when selenium interacts with the browser.
                        // this._driver.FindElementByXPath("//table[@id='giving_table']/tbody/tr[" + currentRow + "]/td[3]/input[@id='amount']").SendKeys("0");
                    }

                    // Type the amount
                    //this._driver.FindElementByXPath(string.Format("//ul[@id='giving_table']/li[][{0}]/input[@id='{1}']", fundCounter + 1, GeneralInFellowship.Giving.Amount)).Clear();
                    //this._driver.FindElementByXPath(string.Format("//ul[@id='giving_table']/li[{0}]/input[@id='{1}']", fundCounter + 1, GeneralInFellowship.Giving.Amount)).SendKeys(item.amount);

                    IWebElement amontElement = this._driver.FindElementById("giving_table").FindElements(By.CssSelector("li[class=\"resp_giving_row\"]"))[fundCounter].FindElement(By.TagName("div")).FindElement(By.CssSelector("[class=\"col-sm-3 giving_amount\"]")).FindElement(By.TagName("div")).FindElement(By.Id(GeneralInFellowship.Giving.Amount));
                    amontElement.Clear();
                    amontElement.SendKeys(item.amount);
                    
                    if (contributionData.Count() > 1)
                    {
                        this._driver.FindElementByLinkText("Add another").Click();
                    }

                }
                fundCounter++;
            }
        }

        /// <summary>
        /// Populates the firstname, lastname, and email address for a give now without an account
        /// </summary>
        /// <param name="firstName">Contributor's First Name</param>
        /// <param name="lastName">Contributor's Last Name</param>
        /// <param name="emailAddress">Contributor's Email Address</param>
        public void GivingWithoutAccount_PopulateFirstLastNameEmail_WebDriver(string firstName, string lastName, string emailAddress)
        {
            // Enter contributor's First Name
            this._driver.FindElementById(GeneralInFellowship.Giving.Contributor_FirstName).SendKeys(firstName);

            // Enter contributor's Last Name
            this._driver.FindElementById(GeneralInFellowship.Giving.Contributor_LastName).SendKeys(lastName);

            // Enter contributor's Email Address
            this._driver.FindElementById(GeneralInFellowship.Giving.Contributor_Email).SendKeys(emailAddress);

        }

        /// <summary>
        /// Populates the payment fields for a give now without an account
        /// </summary>
        /// <param name="churchId">The church ID</param>
        /// <param name="paymentType">The payment type</param>
        /// <param name="firstName">Contributor's first name</param>
        /// <param name="lastName">Contributor's last name</param>
        /// <param name="paymentData">The payment data for the contribution</param>
        public void GivingWithoutAccount_PopulatePaymentInformation_WebDriver(int churchId, GeneralEnumerations.GiveByTypes paymentType, string firstName, string lastName, dynamic paymentData)
            // string phoneNumber = null, string accountNumber = null, string routingNumber = null, string cardType = null, string cardNumber = null, string expireMonth = null, string expireYear = null, string securityCode = null, string country = "United States", string addressLine1 = null, string addressLine2 = null, string city = null, string state = null, string zipCode = null, string county = null
        {
            // Select Payment Type
            switch (paymentType)
            {
                case GeneralEnumerations.GiveByTypes.Credit_card:
                    #region Credit Card
                    if (churchId == 15)
                    {
                        // Select bank Card bullet
                        this._driver.FindElementById(GeneralInFellowship.Giving.CreditCard_Bullet).Click();
                    }

                    // Enter First Name
                    this._driver.FindElementById(GeneralInFellowship.Giving.Cardholder_FirstName).SendKeys(firstName);

                    // Enter Last Name
                    this._driver.FindElementById(GeneralInFellowship.Giving.Cardholder_LastName).SendKeys(lastName);

                    // Select Card Type
                    new SelectElement(this._driver.FindElementById(GeneralInFellowship.Giving.CreditCard_Type)).SelectByText(paymentData.cardType);

                    // Enter Card Number
                    this._driver.FindElementById(GeneralInFellowship.Giving.CreditCard_Number).SendKeys(paymentData.cardNumber);

                    // Select Expiration Month
                    new SelectElement(this._driver.FindElementById(GeneralInFellowship.Giving.CreditCard_ExpireMonth)).SelectByText(paymentData.expireMonth);

                    // Select Expiration Year
                    new SelectElement(this._driver.FindElementById(GeneralInFellowship.Giving.CreditCard_ExpireYear)).SelectByText(paymentData.expireYear);

                        //if (paymentData.cardType == "Switch")
                        //{
                        //    // Enter the valid from date
                        //    new SelectElement(this._driver.FindElementById(GeneralInFellowship.Giving.CreditCard_ValidFromMonth)).SelectByText(paymentData.validFromMonth);
                        //    new SelectElement(this._driver.FindElementById(GeneralInFellowship.Giving.CreditCard_ValidFromYear)).SelectByText(paymentData.validFromYear);
                        //}
                        //else
                        //{
                    // Enter Security Code
                    this._driver.FindElementById(GeneralInFellowship.Giving.CreditCard_SecurityCode).SendKeys(paymentData.securityCode);
                        //}

                    // Select Country
                    new SelectElement(this._driver.FindElementById(GeneralInFellowship.Giving.Country)).SelectByText(paymentData.country);

                    // Enter Address Line 1
                    this._driver.FindElementById(GeneralInFellowship.Giving.AddressLine1).SendKeys(paymentData.addressLine1);

                    // Enter Address Line 2
                    this._driver.FindElementById(GeneralInFellowship.Giving.AddressLine2).SendKeys(paymentData.addressLine2);

                    // Enter City
                    this._driver.FindElementById(GeneralInFellowship.Giving.City).SendKeys(paymentData.city);

                    if (paymentData.country == "United Kingdom")
                    {
                        // Select State, Province
                        this._driver.FindElementById(GeneralInFellowship.Giving.StProvince).SendKeys(paymentData.state);
                    }
                    else
                    {
                        // Select State
                        new SelectElement(this._driver.FindElementById(GeneralInFellowship.Giving.State)).SelectByText(paymentData.state);
                    }

                    // Enter Zip Code
                    this._driver.FindElementById(GeneralInFellowship.Giving.ZipCode).SendKeys(paymentData.zipCode);

                    if (churchId == 15)
                    {
                        // Enter County
                        this._driver.FindElementById(GeneralInFellowship.Giving.County).SendKeys(paymentData.county);
                    }
                    break;
                    #endregion Credit Card
                case GeneralEnumerations.GiveByTypes.Personal_check:
                    #region Personal Check
                    //Instantiate the js executor
                    IJavaScriptExecutor executor = (IJavaScriptExecutor)this._driver;

                    // Select Personal Check
                    this._driver.FindElementById(GeneralInFellowship.Giving.PersonalCheck_Bullet).Click();

                    // Enter Phone Number
                    this._driver.FindElementByXPath(GeneralInFellowship.Giving.PhoneNumber).SendKeys(paymentData.phoneNumber);

                    // Enter Routing Number
                    //this._driver.FindElementById(GeneralInFellowship.Giving.RoutingNumber).SendKeys(paymentData.routingNumber);
                    //User JS to set value
                    IWebElement routingNumElement = this._driver.FindElement(By.XPath(GeneralInFellowship.Giving.RoutingNumber));
                    executor.ExecuteScript("arguments[0].focus();", routingNumElement);
                    executor.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2])", routingNumElement, "value", paymentData.routingNumber);

                    // Enter Account Number
                    //this._driver.FindElementById(GeneralInFellowship.Giving.AccountNumber).SendKeys(paymentData.accountNumber);
                    //User JS to set value
                    IWebElement accountNumElement = this._driver.FindElement(By.XPath(GeneralInFellowship.Giving.AccountNumber));
                    executor.ExecuteScript("arguments[0].focus();", routingNumElement);
                    executor.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2])", accountNumElement, "value", paymentData.accountNumber);
                    break;
                    #endregion Personal Check

                default:
                    break;
            }
        }

        /// <summary>
        /// Populates the payment fields for a give now without an account
        /// </summary>
        /// <param name="churchId">The church ID</param>
        /// <param name="paymentType">The payment type</param>
        /// <param name="firstName">Contributor's first name</param>
        /// <param name="lastName">Contributor's last name</param>
        /// <param name="phoneNumber">ECHECK - Phone Number</param>
        /// <param name="accountNumber">ECHECK - Account Number</param>
        /// <param name="routingNumber">ECHECK - Routing Number</param>
        /// <param name="cardType">The Card Type</param>
        /// <param name="cardNumber">The Card Number</param>
        /// <param name="expireMonth">The Expiration Month</param>
        /// <param name="expireYear">The Expiration Year</param>
        /// <param name="securityCode">The Security Code</param>
        /// <param name="country">The Country</param>
        /// <param name="addressLine1">The Address Line 1</param>
        /// <param name="addressLine2">The Address Line 2</param>
        /// <param name="city">The City</param>
        /// <param name="state">The State</param>
        /// <param name="zipCode">The ZipCode</param>
        /// <param name="county">The County</param>
        public void GivingWithoutAccount_PopulatePaymentInformation_WebDriver(int churchId, GeneralEnumerations.GiveByTypes paymentType, string firstName, string lastName, string phoneNumber = null, string accountNumber = null, string routingNumber = null,
            string cardType = null, string cardNumber = null, string expireMonth = null, string expireYear = null, string securityCode = null, string country = "United States", string addressLine1 = null, string addressLine2 = null, string city = null, 
            string state = null, string zipCode = null, string county = null)
        {
            // Select Payment Type
            switch (paymentType)
            {
                case GeneralEnumerations.GiveByTypes.Credit_card:
                    #region Credit Card
                    if (churchId == 15)
                    {
                        // Select bank Card bullet
                        this._driver.FindElementById(GeneralInFellowship.Giving.CreditCard_Bullet).Click();
                    }

                    // Enter First Name
                    this._driver.FindElementById(GeneralInFellowship.Giving.Cardholder_FirstName).SendKeys(firstName);

                    // Enter Last Name
                    this._driver.FindElementById(GeneralInFellowship.Giving.Cardholder_LastName).SendKeys(lastName);

                    // Select Card Type
                    new SelectElement(this._driver.FindElementById(GeneralInFellowship.Giving.CreditCard_Type)).SelectByText(cardType);

                    // Enter Card Number
                    this._driver.FindElementById(GeneralInFellowship.Giving.CreditCard_Number).SendKeys(cardNumber);

                    // Select Expiration Month
                    new SelectElement(this._driver.FindElementById(GeneralInFellowship.Giving.CreditCard_ExpireMonth)).SelectByText(expireMonth);

                    // Select Expiration Year
                    new SelectElement(this._driver.FindElementById(GeneralInFellowship.Giving.CreditCard_ExpireYear)).SelectByText(expireYear);

                    //if (paymentData.cardType == "Switch")
                    //{
                    //    // Enter the valid from date
                    //    new SelectElement(this._driver.FindElementById(GeneralInFellowship.Giving.CreditCard_ValidFromMonth)).SelectByText(paymentData.validFromMonth);
                    //    new SelectElement(this._driver.FindElementById(GeneralInFellowship.Giving.CreditCard_ValidFromYear)).SelectByText(paymentData.validFromYear);
                    //}
                    //else
                    //{
                    // Enter Security Code
                    this._driver.FindElementById(GeneralInFellowship.Giving.CreditCard_SecurityCode).SendKeys(securityCode);
                    //}

                    // Select Country
                    new SelectElement(this._driver.FindElementById(GeneralInFellowship.Giving.Country)).SelectByText(country);

                    // Enter Address Line 1
                    this._driver.FindElementById(GeneralInFellowship.Giving.AddressLine1).SendKeys(addressLine1);

                    // Enter Address Line 2
                    this._driver.FindElementById(GeneralInFellowship.Giving.AddressLine2).SendKeys(addressLine2);

                    // Enter City
                    this._driver.FindElementById(GeneralInFellowship.Giving.City).SendKeys(city);

                    if (country == "United Kingdom")
                    {
                        // Select State, Province
                        this._driver.FindElementById(GeneralInFellowship.Giving.StProvince).SendKeys(state);
                    }
                    else
                    {
                        // Select State
                        new SelectElement(this._driver.FindElementById(GeneralInFellowship.Giving.State)).SelectByText(state);
                    }

                    // Enter Zip Code
                    this._driver.FindElementById(GeneralInFellowship.Giving.ZipCode).SendKeys(zipCode);

                    if (churchId == 15)
                    {
                        // Enter County
                        this._driver.FindElementById(GeneralInFellowship.Giving.County).SendKeys(county);
                    }
                    break;
                    #endregion Credit Card
                case GeneralEnumerations.GiveByTypes.Personal_check:
                    #region Personal Check
                    //Instantiate the js executor
                    IJavaScriptExecutor executor = (IJavaScriptExecutor)this._driver;

                    // Select Personal Check
                    this._driver.FindElementById(GeneralInFellowship.Giving.PersonalCheck_Bullet).Click();

                    // Enter Phone Number
                    this._driver.FindElementById(GeneralInFellowship.Giving.PhoneNumber).SendKeys(phoneNumber);

                    // Enter Routing Number
                    //this._driver.FindElementById(GeneralInFellowship.Giving.RoutingNumber).SendKeys(paymentData.routingNumber);
                    //User JS to set value
                    IWebElement routingNumElement = this._driver.FindElement(By.Id(GeneralInFellowship.Giving.RoutingNumber));
                    executor.ExecuteScript("arguments[0].focus();", routingNumElement);
                    executor.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2])", routingNumElement, "value", routingNumber);

                    // Enter Account Number
                    //this._driver.FindElementById(GeneralInFellowship.Giving.AccountNumber).SendKeys(paymentData.accountNumber);
                    //User JS to set value
                    IWebElement accountNumElement = this._driver.FindElement(By.Id(GeneralInFellowship.Giving.AccountNumber));
                    executor.ExecuteScript("arguments[0].focus();", routingNumElement);
                    executor.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2])", accountNumElement, "value", accountNumber);
                    break;
                    #endregion Personal Check

                default:
                    break;
            }
        }
        
        /// <summary>
        /// Validates the giving history page for a given contribution.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="firstName">The first name of the contributor.</param>
        /// <param name="lastName">The last name of the contributor.</param>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive the contribution was given to</param>
        /// <param name="subFund">The sub fund the contribution was given to.</param>
        /// <param name="individualAmount">The amount.</param>
        /// <param name="paymentTime">The time the payment was processed.</param>
        /// <param name="paymentTypeId">The payment ID.</param>
        /// <param name="paymentType">The type of payment (Credit Card or eCheck).  This is used to figure out the text displayed on the Giving History page.</param>
        /// <param name="paymentProcessed">Specify if the payment has processed.  This determines if the Pending text is present or not</param>
        public void Giving_GivingHistory_Validate(int churchId, string firstName, string lastName, string fundOrPledgeDrive, string subFund, string individualAmount, DateTime paymentTime, int paymentTypeId, string paymentType, bool paymentProcessed) {

            // Store the text for the payment type
            var paymentTypeText = paymentType == "Credit Card" ? "Credit Card" : "eCheck";

            // Store the full name of this individual
            var fullName = string.Empty;
            if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName)) {
                fullName = this._sql.People_GetIndividualName(churchId, firstName, lastName);
            }

            // Store the culture format
            CultureInfo culture = null;
            if (churchId == 258) {
                culture = new CultureInfo("en-GB");
            }
            else {
                culture = new CultureInfo("en-US");
            }


            // Verify credit card payment success message displays
           // if (this._f1Environment == F1Environments.DEV || this._f1Environment == F1Environments.QA || this._f1Environment == F1Environments.STAGING) {
            if (this._f1Environment == F1Environments.QA || this._f1Environment == F1Environments.STAGING)
            {
    
            this._selenium.IsTextPresent("Your payment was successfully processed. Thank you for your contribution. This was successful in the test environment of Cybersource");
            }

            // Verify the current contribution exists in Giving History page
            // Get the row the contribution exists on
            TestLog.WriteLine(string.Format("Text Amount: {0}", this._selenium.GetText(string.Format("{0}/tbody/tr[2]/td[5]", TableIds.InFellowship_GivingHistory))));
            decimal itemRow = this._generalMethods.GetTableRowNumber(TableIds.InFellowship_GivingHistory, string.Format(culture, "{0:c}", Convert.ToDecimal(individualAmount)), "Amount", "contains");

            // Is the name correct?
            Assert.Contains(this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.InFellowship_GivingHistory, itemRow + 1)), fullName);
            //Assert.AreEqual(fullName, this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.InFellowship_GivingHistory, itemRow + 1)));

            // Is the fund and or subfund correct?
            if (string.IsNullOrEmpty(subFund)) {
                Assert.Contains(this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[3]", TableIds.InFellowship_GivingHistory, itemRow + 1)), string.Format("{0} \n {1}", fundOrPledgeDrive, paymentTypeText));
                //Assert.AreEqual(string.Format("{0} \n {1}", fundOrPledgeDrive, paymentTypeText), this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[3]", TableIds.InFellowship_GivingHistory, itemRow + 1)));
            }
            else {
                
               //Suchitra Notes - This line is failing in INT because the last saved fund is  not always on top or the page refresh is chnaging the giving value...
                Assert.Contains(this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[3]", TableIds.InFellowship_GivingHistory, itemRow + 1)), string.Format("{0}> {1} \n {2}", fundOrPledgeDrive, subFund, paymentTypeText));
                //Assert.AreEqual(string.Format("{0} > {1} \n {2}", fundOrPledgeDrive, subFund, paymentTypeText), this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[3]", TableIds.InFellowship_GivingHistory, itemRow + 1)));
                
                
                
            }

            // Is the payment pending?
            if (!paymentProcessed) {
                Assert.AreEqual("Pending", this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[4]", TableIds.InFellowship_GivingHistory, itemRow + 1)));
            }
            else {
                Assert.Contains(this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[4]", TableIds.InFellowship_GivingHistory, itemRow + 1)), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("dd MMM yyyy"));
                //Assert.AreEqual(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("dd MMM yyyy"), this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[4]", TableIds.InFellowship_GivingHistory, itemRow + 1)));
            }


            // Is the amount correct?
            Assert.Contains(this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.InFellowship_GivingHistory, itemRow + 1)), string.Format(culture, "{0:c}", Convert.ToDecimal(individualAmount)));
            //Assert.AreEqual(string.Format(culture, "{0:c}", Convert.ToDecimal(individualAmount)), this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.InFellowship_GivingHistory, itemRow + 1)));


        }

        /// <summary>
        /// Validates the giving history page for a given contribution.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="firstName">The first name of the contributor.</param>
        /// <param name="lastName">The last name of the contributor.</param>
        /// <param name="fundOrPledgeDrive">The fund or pledge drive the contribution was given to</param>
        /// <param name="subFund">The sub fund the contribution was given to.</param>
        /// <param name="individualAmount">The amount.</param>
        /// <param name="paymentTime">The time the payment was processed.</param>
        /// <param name="paymentTypeId">The payment ID.</param>
        /// <param name="paymentType">The type of payment (Credit Card or eCheck).  This is used to figure out the text displayed on the Giving History page.</param>
        /// <param name="paymentProcessed">Specify if the payment has processed.  This determines if the Pending text is present or not</param>
        public void Giving_GivingHistory_Validate_WebDriver(int churchId, string firstName, string lastName, string fundOrPledgeDrive, string subFund, string individualAmount, DateTime paymentTime, int paymentTypeId, string paymentType, bool paymentProcessed) {

            // Store the text for the payment type
            var paymentTypeText = paymentType == "Credit Card" ? "Credit Card" : "eCheck";

            // Store the full name of this individual
            var fullName = string.Empty;
            if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName)) {
                fullName = this._sql.People_GetIndividualName(churchId, firstName, lastName);
            }

            // Store the culture format
            CultureInfo culture = null;
            string moneySign = string.Empty;
            if (churchId == 258) {
                culture = new CultureInfo("en-GB");
                moneySign = "£";
            }
            else {
                culture = new CultureInfo("en-US");
                moneySign = "$";
            }


            // Verify credit card payment success message displays
           // if (this._f1Environment == F1Environments.DEV || this._f1Environment == F1Environments.QA || this._f1Environment == F1Environments.STAGING) {
                        if (this._f1Environment == F1Environments.QA || this._f1Environment == F1Environments.STAGING) {
    
                this._generalMethods.IsTextPresentWebDriver("Your payment was successfully processed. Thank you for your contribution. This was successful in the test environment of Cybersource");
            }

            // modify by grace zhang start
            string uniqueIdentifier = "";
            if (Convert.ToDecimal(individualAmount) / 1000 >= 1)
            {
                uniqueIdentifier = string.Format("{0}", string.Format("{0}{1},{2}", moneySign, Convert.ToInt32(Convert.ToDecimal(individualAmount) / 1000), Convert.ToDecimal(individualAmount) % 1000));
            }
            else
            {
                uniqueIdentifier = string.Format("{0}", string.Format("{0}{1}", moneySign, Convert.ToDecimal(individualAmount)));
            }
            
            TestLog.WriteLine("uniqueIdentifier=="+uniqueIdentifier);
            // Verify the current contribution exists in Giving History page
            // Get the row the contribution exists on
            //decimal itemRow = this._generalMethods.GetTableRowNumberWebDriver(TableIds.InFellowship_GivingHistory, string.Format("{0} \n {1}", string.Format(culture, "{0:c}", Convert.ToDecimal(individualAmount)), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("dd MMM yyyy")), "Amount", "contains");
            decimal itemRow = 0;
            var windowSize = this._driver.Manage().Window.Size;

            if (windowSize.Width == 640)
            {
                itemRow = this._generalMethods.GetTableRowNumberResponsiveWebDriver(TableIds.InFellowship_GivingHistory, string.Format("{0}", string.Format("{0}{1}", moneySign, Convert.ToDecimal(individualAmount)))); //string.Format("{0:0,00.00}", Convert.ToDecimal(individualAmount))
            }
            else
            {
                //itemRow = this._generalMethods.GetTableRowNumberWebDriver(TableIds.InFellowship_GivingHistory, string.Format("{0}", string.Format("{0}{1}", moneySign, string.Format("{0:0,00.00}",Convert.ToDecimal(individualAmount)))), "Amount", "contains");
                //modify by grace zhang:Modify the value of the second input parameter
                itemRow = this._generalMethods.GetTableRowNumberWebDriver(TableIds.InFellowship_GivingHistory, uniqueIdentifier, "Amount", "contains");
            }
            // modify by grace zhang end
            // Is the name correct?
            Assert.Contains(this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.InFellowship_GivingHistory, itemRow + 1)).Text, fullName);
            //Assert.AreEqual(fullName, this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.InFellowship_GivingHistory, itemRow + 1)).Text);

            // Is the fund and or subfund correct? add by grace zhang:fund will not appear on the page,so not verified. page element:Name, Giving Details, Date, Amount
            if (string.IsNullOrEmpty(subFund)) {
                //Assert.Contains(this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[3]", TableIds.InFellowship_GivingHistory, itemRow + 1)).Text, string.Format("{0}", fundOrPledgeDrive));
                //Assert.AreEqual(string.Format("{0}\r\n{1}", fundOrPledgeDrive, paymentTypeText), this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[3]", TableIds.InFellowship_GivingHistory, itemRow + 1)).Text);
            }
            else {
                //Assert.Contains(this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[3]", TableIds.InFellowship_GivingHistory, itemRow + 1)).Text, string.Format("{0}> {1}", fundOrPledgeDrive, subFund));
                //Assert.AreEqual(string.Format("{0} > {1} \n {2}", fundOrPledgeDrive, subFund, paymentTypeText), this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[3]", TableIds.InFellowship_GivingHistory, itemRow + 1)).Text);
            }

            // Is the payment pending?
            if (!paymentProcessed) {
                Assert.AreEqual("Pending", this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[4]", TableIds.InFellowship_GivingHistory, itemRow + 1)).Text);
            }
            else {

                //Set Time Zone based on church id (UK = 258, others US Central)
                DateTime tzInfo;

                if (churchId == 258)
                {
                    tzInfo = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Greenwich Standard Time"));
                }
                else
                {
                    tzInfo = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
                }

                Assert.Contains(this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[4]", TableIds.InFellowship_GivingHistory, itemRow + 1)).Text, tzInfo.ToString("dd MMM yyyy"));

                //Assert.AreEqual(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("dd MMM yyyy"), this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[4]", TableIds.InFellowship_GivingHistory, itemRow + 1)).Text);
            }


            // Is the amount correct?
            if (windowSize.Width == 640)
            {
                Assert.Contains(this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[3]/span", TableIds.InFellowship_GivingHistory, itemRow + 1)).Text, string.Format("{0}{1}", moneySign, Convert.ToDecimal(individualAmount)));
            }
            else
            {
                Assert.Contains(this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.InFellowship_GivingHistory, itemRow + 1)).Text, uniqueIdentifier);
                //Assert.Contains(this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.InFellowship_GivingHistory, itemRow + 1)).Text, string.Format(culture, "{0:c}", Convert.ToDecimal(individualAmount)));
                //Assert.AreEqual(string.Format(culture, "{0:c}", Convert.ToDecimal(individualAmount)), this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[5]", TableIds.InFellowship_GivingHistory, itemRow + 1)).Text);
            }

        }

        /// <summary>
        /// Verifies that a scheduled giving exists on the schedule giving page.
        /// </summary>
        /// <param name="churchId">The church id</param>
        /// <param name="frequency">The frequency this schedule was set to</param>
        /// <param name="dates">The dates for this scheduled giving, stored in an anonymous type.</param>
        /// <param name="total">The total for the scheduled contribution.</param>
        /// <param name="paymentName">The payment name.</param>
        /// <param name="lastFourDigits">The last four digits.</param>
        private void Giving_ScheduleGiving_VerifyScheduleExists(int churchId, GeneralEnumerations.GivingFrequency frequency, dynamic dates, string total, string paymentName, string lastFourDigits) {

            // Store the culture format
            CultureInfo culture = null;
            if (churchId == 258) {
                culture = new CultureInfo("en-GB");
            }
            else {
                culture = new CultureInfo("en-US");
            }

            // Get row the schedule is on
            decimal itemRow = this._generalMethods.GetTableRowNumber(TableIds.InFellowship_GivingSchedules, string.Format(culture, "{0:c}", Convert.ToDecimal(total)), "Amount");

            // Verify the data
            // Name and Next Contribution            
            switch (frequency) {
                case GeneralEnumerations.GivingFrequency.Once:
                    var formattedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
                    formattedDate = Convert.ToDateTime(dates.OneTimeDate, culture);
                    Assert.Contains(this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[1]", TableIds.InFellowship_GivingSchedules, itemRow + 1)), "One time");
                    //Assert.AreEqual("One time", this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[1]", TableIds.InFellowship_GivingSchedules, itemRow + 1)));
                    Assert.Contains(this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.InFellowship_GivingSchedules, itemRow + 1)), string.Format("{0:dd MMM yyyy}", formattedDate));
                    //Assert.AreEqual(string.Format("{0:dd MMM yyyy}", formattedDate), this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.InFellowship_GivingSchedules, itemRow + 1)));
                    break;
                case GeneralEnumerations.GivingFrequency.Monthly:
                    formattedDate = Convert.ToDateTime(string.Format("{0} {1} {2}", dates.StartDay, dates.StartMonth, dates.StartYear));

                    // Link 
                   // Assert.AreEqual(string.Format("{0}{1} every month", formattedDate.Day.ToString("d"), this._generalMethods.GetSuffexForDay(formattedDate)), this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[1]", TableIds.InFellowship_GivingSchedules, itemRow + 1)));

                    //SP - changing to Assert.contains to check the substring only 
                    Assert.Contains(this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[1]", TableIds.InFellowship_GivingSchedules, itemRow + 1)), "month");

                    // Next date
                    // Did we already pass the current month's scheduled date?
                    if (formattedDate.Day < TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Day)
                    {
                        // We did
                        formattedDate.AddMonths(1);
                    }

                   // Assert.AreEqual(string.Format("{0:dd MMM yyyy}", formattedDate), this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.InFellowship_GivingSchedules, itemRow + 1)));

                    break;
                case GeneralEnumerations.GivingFrequency.TwiceMonthly:
                    formattedDate = Convert.ToDateTime(string.Format("{0} {1} {2}", dates.StartDay, dates.StartMonth, dates.StartYear));

                    Assert.Contains(this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[1]", TableIds.InFellowship_GivingSchedules, itemRow + 1)), "1st and 16th every month");
                    //Assert.AreEqual("1st and 16th every month", this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[1]", TableIds.InFellowship_GivingSchedules, itemRow + 1)));

                    // Did we already pass the current month's scheduled date?
                    if (formattedDate.Day < TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Day)
                    {
                        // We did
                        formattedDate.AddMonths(1);

                        // If the provided day is 1, make it the 16th since we passed the date.  Likewise, make it one if it is 16.
                        if (formattedDate.Day == 1) {
                            formattedDate.AddDays(15);
                        }
                        else {
                            formattedDate.AddDays(-15);
                        }
                    }
                    Assert.Contains(this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.InFellowship_GivingSchedules, itemRow + 1)), string.Format("{0:dd MMM yyyy}", formattedDate));
                    //Assert.AreEqual(string.Format("{0:dd MMM yyyy}", formattedDate), this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.InFellowship_GivingSchedules, itemRow + 1)));
                    break;
                case GeneralEnumerations.GivingFrequency.Weekly:
                    formattedDate = Convert.ToDateTime(dates.StartDate, culture);
                    Assert.Contains(this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[1]", TableIds.InFellowship_GivingSchedules, itemRow + 1)), string.Format("Weekly every {0}", dates.StartDay));
                    //Assert.AreEqual(string.Format("Weekly every {0}", dates.StartDay), this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[1]", TableIds.InFellowship_GivingSchedules, itemRow + 1)));

                    // Store the current day
                    var currentDay = Convert.ToDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), culture);

                    // Store the next schedule date
                    var nextScheduleDate = formattedDate;

                    // Did the schedule start date already occur?
                    if (currentDay > formattedDate) {
                        // It did.  Keep adding 7 days until the next schedule day occurs after the current day
                        while (currentDay > formattedDate) {
                            nextScheduleDate.AddDays(7);
                        }
                    }

                    // Convert to a universal date
                    var universalDateFancy = this._generalMethods.ConvertDateToNeutralFormat(nextScheduleDate, culture);

                    Assert.Contains(this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.InFellowship_GivingSchedules, itemRow + 1)), universalDateFancy);
                    //Assert.AreEqual(universalDateFancy, this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.InFellowship_GivingSchedules, itemRow + 1)));

                    break;
                case GeneralEnumerations.GivingFrequency.EveryTwoWeeks:
                    formattedDate = Convert.ToDateTime(dates.StartDate);
                    Assert.Contains(this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[1]", TableIds.InFellowship_GivingSchedules, itemRow + 1)), string.Format("{0} every two weeks", dates.StartDay));
                    //Assert.AreEqual(string.Format("{0} every two weeks", dates.StartDay), this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[1]", TableIds.InFellowship_GivingSchedules, itemRow + 1)));

                    // Store the current day
                    currentDay = Convert.ToDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), culture);

                    // Store the next schedule date
                    nextScheduleDate = formattedDate;

                    // Did the schedule start date already occur?
                    if (currentDay > formattedDate) {
                        // It did.  Keep adding 14 days until the next schedule day occurs after the current day
                        while (currentDay > formattedDate) {
                            nextScheduleDate.AddDays(14);
                        }
                    }

                    // Convert to a universal date
                    universalDateFancy = this._generalMethods.ConvertDateToNeutralFormat(nextScheduleDate, culture);

                    Assert.Contains(this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.InFellowship_GivingSchedules, itemRow + 1)), universalDateFancy);
                    //Assert.AreEqual(universalDateFancy, this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.InFellowship_GivingSchedules, itemRow + 1)));

                    break;
                default:
                    throw new SeleniumException(string.Format("Unknown frequency {0} for Scheduled Giving.", frequency.ToString()));
            }

            // Giving Method
            if (paymentName == "Personal Check") {
                Assert.AreEqual(string.Format("Checking Account …{0}", lastFourDigits), this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[3]", TableIds.InFellowship_GivingSchedules, itemRow + 1)));
            }
            else {
                Assert.AreEqual(string.Format("{0} …{1}", paymentName, lastFourDigits), this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[3]", TableIds.InFellowship_GivingSchedules, itemRow + 1)));
            }

            // Amount
            Assert.AreEqual(string.Format(culture, "{0:c}", Convert.ToDecimal(total)), this._selenium.GetText(string.Format("{0}/tbody/tr[{1}]/td[4]", TableIds.InFellowship_GivingSchedules, itemRow + 1)));

            // Status
            this._selenium.IsElementPresent(string.Format("{0}/tbody/tr[{1}]/td[5]/img[contains(@src, '/assets/images/icon_complete.png')]", TableIds.InFellowship_GivingSchedules, itemRow + 1));
        }

        /// <summary>
        /// Verifies that a scheduled giving exists on the schedule giving page.
        /// </summary>
        /// <param name="churchId">The church id</param>
        /// <param name="frequency">The frequency this schedule was set to</param>
        /// <param name="dates">The dates for this scheduled giving, stored in an anonymous type.</param>
        /// <param name="total">The total for the scheduled contribution.</param>
        /// <param name="paymentName">The payment name.</param>
        /// <param name="lastFourDigits">The last four digits.</param>
        private void Giving_ScheduleGiving_VerifyScheduleExists_WebDriver(int churchId, GeneralEnumerations.GivingFrequency frequency, dynamic dates, string total, string paymentName, string lastFourDigits)
        {

            // Store the culture format
            CultureInfo culture = null;
            if (churchId == 258)
            {
                culture = new CultureInfo("en-GB");
            }
            else
            {
                culture = new CultureInfo("en-US");
            }

            // Get row the schedule is on
            decimal itemRow = this._generalMethods.GetTableRowNumberWebDriver(TableIds.InFellowship_GivingSchedules, string.Format(culture, "{0:c}", Convert.ToDecimal(total)), "Amount");

            // Verify the data
            // Name and Next Contribution            
            switch (frequency)
            {
                case GeneralEnumerations.GivingFrequency.Once:
                    var formattedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
                    formattedDate = Convert.ToDateTime(dates.OneTimeDate, culture);
                    Assert.AreEqual("One time", this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[1]", TableIds.InFellowship_GivingSchedules, itemRow)).Text);
                    Assert.AreEqual(string.Format("{0:dd MMM yyyy}", formattedDate), this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.InFellowship_GivingSchedules, itemRow)).Text);
                    break;
                case GeneralEnumerations.GivingFrequency.Monthly:
                    formattedDate = Convert.ToDateTime(string.Format("{0} {1} {2}", dates.StartDay, dates.StartMonth, dates.StartYear));

                    // Link                     
                    Assert.AreEqual(string.Format("{0}{1} every month", formattedDate.Day.ToString("d"), this._generalMethods.GetSuffexForDay(formattedDate)), this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[1]", TableIds.InFellowship_GivingSchedules, itemRow)).Text);

                    // Next date
                    // Did we already pass the current month's scheduled date?
                    if (formattedDate.Day < TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Day)
                    {
                        // We did
                        formattedDate.AddMonths(1);
                    }

                    Assert.AreEqual(string.Format("{0:dd MMM yyyy}", formattedDate), this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.InFellowship_GivingSchedules, itemRow)).Text);

                    break;
                case GeneralEnumerations.GivingFrequency.TwiceMonthly:
                    formattedDate = Convert.ToDateTime(string.Format("{0} {1} {2}", dates.StartDay, dates.StartMonth, dates.StartYear));

                    Assert.AreEqual("1st and 16th every month", this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[1]", TableIds.InFellowship_GivingSchedules, itemRow)).Text);

                    // Did we already pass the current month's scheduled date?
                    if (formattedDate.Day < TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Day)
                    {
                        // We did
                        formattedDate.AddMonths(1);

                        // If the provided day is 1, make it the 16th since we passed the date.  Likewise, make it one if it is 16.
                        if (formattedDate.Day == 1)
                        {
                            formattedDate.AddDays(15);
                        }
                        else
                        {
                            formattedDate.AddDays(-15);
                        }
                    }
                    Assert.AreEqual(string.Format("{0:dd MMM yyyy}", formattedDate), this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.InFellowship_GivingSchedules, itemRow)).Text);
                    break;
                case GeneralEnumerations.GivingFrequency.Weekly:
                    formattedDate = Convert.ToDateTime(dates.StartDate, culture);
                    Assert.AreEqual(string.Format("Weekly every {0}", dates.StartDay), this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[1]", TableIds.InFellowship_GivingSchedules, itemRow)).Text);

                    // Store the current day
                    var currentDay = Convert.ToDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), culture);

                    // Store the next schedule date
                    var nextScheduleDate = formattedDate;

                    // Did the schedule start date already occur?
                    if (currentDay > formattedDate)
                    {
                        // It did.  Keep adding 7 days until the next schedule day occurs after the current day
                        while (currentDay > formattedDate)
                        {
                            nextScheduleDate.AddDays(7);
                        }
                    }

                    // Convert to a universal date
                    var universalDateFancy = this._generalMethods.ConvertDateToNeutralFormat(nextScheduleDate, culture);

                    Assert.AreEqual(universalDateFancy, this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.InFellowship_GivingSchedules, itemRow)).Text);

                    break;
                case GeneralEnumerations.GivingFrequency.EveryTwoWeeks:
                    formattedDate = Convert.ToDateTime(dates.StartDate);
                    Assert.AreEqual(string.Format("{0} every two weeks", dates.StartDay), this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[1]", TableIds.InFellowship_GivingSchedules, itemRow)).Text);

                    // Store the current day
                    currentDay = Convert.ToDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), culture);

                    // Store the next schedule date
                    nextScheduleDate = formattedDate;

                    // Did the schedule start date already occur?
                    if (currentDay > formattedDate)
                    {
                        // It did.  Keep adding 14 days until the next schedule day occurs after the current day
                        while (currentDay > formattedDate)
                        {
                            nextScheduleDate.AddDays(14);
                        }
                    }

                    // Convert to a universal date
                    universalDateFancy = this._generalMethods.ConvertDateToNeutralFormat(nextScheduleDate, culture);

                    Assert.AreEqual(universalDateFancy, this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[2]", TableIds.InFellowship_GivingSchedules, itemRow)).Text);

                    break;
                default:
                    throw new SeleniumException(string.Format("Unknown frequency {0} for Scheduled Giving.", frequency.ToString()));
            }

            // Giving Method
            if (paymentName == "Personal Check")
            {
                Assert.AreEqual(string.Format("Checking Account …{0}", lastFourDigits), this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[3]", TableIds.InFellowship_GivingSchedules, itemRow)).Text);
            }
            else
            {
                Assert.AreEqual(string.Format("{0} …{1}", paymentName, lastFourDigits), this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[3]", TableIds.InFellowship_GivingSchedules, itemRow)).Text);
            }

            // Amount
            Assert.AreEqual(string.Format(culture, "{0:c}", Convert.ToDecimal(total)), this._driver.FindElementByXPath(string.Format("{0}/tbody/tr[{1}]/td[4]", TableIds.InFellowship_GivingSchedules, itemRow)).Text);

            // Status
            this._generalMethods.IsElementPresentWebDriver(By.XPath(string.Format("{0}/tbody/tr[{1}]/td[5]/img[contains(@src, '/assets/images/icon_complete.png')]", TableIds.InFellowship_GivingSchedules, itemRow)));
        }

        /// <summary>
        ///  Populates the monthly frequency option for Scheduled Giving
        /// </summary>
        /// <param name="dates">The dates for the frequency to start.  This is an anonymous type.</param>
        private void Giving_ScheduledGiving_Populate_MonthlyFrequency(dynamic dates) {
            this._selenium.Click("frequency_1");
            if (!string.IsNullOrEmpty(dates.StartDay.ToString())) {
                this._selenium.Select("start_monthly_day", dates.StartDay.ToString());
            }
            if (!string.IsNullOrEmpty(dates.StartMonth.ToString())) {
                this._selenium.Select("start_monthly_month", dates.StartMonth.ToString());
            }
            if (!string.IsNullOrEmpty(dates.StartYear.ToString())) {
                this._selenium.Select("start_monthly_year", dates.StartYear.ToString());
            }

            // If there is an end month, select the end check box and populate the data
            if (!string.IsNullOrEmpty(dates.EndMonth)) {
                this._selenium.Click("once_monthly_end");
                this._selenium.Select("end_monthly_month", dates.EndMonth.ToString());
                this._selenium.Select("end_monthly_year", dates.EndYear.ToString());
            }
        }

        /// <summary>
        ///  Populates the monthly frequency option for Scheduled Giving
        /// </summary>
        /// <param name="dates">The dates for the frequency to start.  This is an anonymous type.</param>
        private void Giving_ScheduledGiving_Populate_MonthlyFrequency_WebDriver(dynamic dates)
        {
            this._driver.FindElementById("frequency_1").Click();
            if (!string.IsNullOrEmpty(dates.StartDay.ToString()))
            {
                new SelectElement(this._driver.FindElementById("start_monthly_day")).SelectByText(dates.StartDay.ToString());
            }
            if (!string.IsNullOrEmpty(dates.StartMonth.ToString()))
            {
                new SelectElement(this._driver.FindElementById("start_monthly_month")).SelectByText(dates.StartMonth.ToString());
            }
            if (!string.IsNullOrEmpty(dates.StartYear.ToString()))
            {
                new SelectElement(this._driver.FindElementById("start_monthly_year")).SelectByText(dates.StartYear.ToString());
            }

            // If there is an end month, select the end check box and populate the data
            if (!string.IsNullOrEmpty(dates.EndMonth))
            {
                this._driver.FindElementById("once_monthly_end").Click();
                new SelectElement(this._driver.FindElementById("end_monthly_month")).SelectByText(dates.EndMonth.ToString());
                new SelectElement(this._driver.FindElementById("end_monthly_year")).SelectByText(dates.EndYear.ToString());
            }
        }

        /// <summary>
        ///  Populates the twice monthly frequency option for Scheduled Giving
        /// </summary>
        /// <param name="dates">The dates for the frequency to start.  This is an anonymous type.</param>
        private void Giving_ScheduledGiving_Populate_TwiceMonthlyFrequency(dynamic dates) {
            this._selenium.Click("frequency_2");
            if (!string.IsNullOrEmpty(dates.StartDay.ToString())) {
                this._selenium.Select("start_date_1_16", dates.StartDay.ToString());
            }
            if (!string.IsNullOrEmpty(dates.StartMonth.ToString())) {
                this._selenium.Select("start_month_1_16", dates.StartMonth.ToString());
            }
            if (!string.IsNullOrEmpty(dates.StartYear.ToString())) {
                this._selenium.Select("start_year_1_16", dates.StartYear.ToString());
            }

            // If there is an end month, select the end check box and populate the data
            if (!string.IsNullOrEmpty(dates.EndMonth)) {
                this._selenium.Click("twice_monthly_end");
                this._selenium.Select("end_date_1_16", dates.EndDay.ToString());
                this._selenium.Select("end_month_1_16", dates.EndMonth.ToString());
                this._selenium.Select("end_year_1_16", dates.EndYear.ToString());
            }
        }

        /// <summary>
        ///  Populates the twice monthly frequency option for Scheduled Giving
        /// </summary>
        /// <param name="dates">The dates for the frequency to start.  This is an anonymous type.</param>
        private void Giving_ScheduledGiving_Populate_TwiceMonthlyFrequency_WebDriver(dynamic dates)
        {
            this._driver.FindElementById("frequency_2").Click();
            if (!string.IsNullOrEmpty(dates.StartDay.ToString()))
            {
                new SelectElement(this._driver.FindElementById("start_date_1_16")).SelectByText(dates.StartDay.ToString());
            }
            if (!string.IsNullOrEmpty(dates.StartMonth.ToString()))
            {
                new SelectElement(this._driver.FindElementById("start_month_1_16")).SelectByText(dates.StartMonth.ToString());
            }
            if (!string.IsNullOrEmpty(dates.StartYear.ToString()))
            {
                new SelectElement(this._driver.FindElementById("start_year_1_16")).SelectByText(dates.StartYear.ToString());
            }

            // If there is an end month, select the end check box and populate the data
            if (!string.IsNullOrEmpty(dates.EndMonth))
            {
                this._driver.FindElementById("twice_monthly_end").Click();
                new SelectElement(this._driver.FindElementById("end_date_1_16")).SelectByText(dates.EndDay.ToString());
                new SelectElement(this._driver.FindElementById("end_month_1_16")).SelectByText(dates.EndMonth.ToString());
                new SelectElement(this._driver.FindElementById("end_year_1_16")).SelectByText(dates.EndYear.ToString());
            }
        }

        /// <summary>
        ///  Populates the weeklyfrequency option for Scheduled Giving
        /// </summary>
        /// <param name="dates">The dates for the frequency to start.  This is an anonymous type.</param>
        private void Giving_ScheduledGiving_Populate_WeeklyFrequency(dynamic dates) {
            this._selenium.Click("frequency_3");
            this._selenium.Select("weekly_start_day", dates.StartDay.ToString());
            this._selenium.Type("weekly_start_date", dates.StartDate.ToString());

            // If there is an end month, select the end check box and populate the data
            if (!string.IsNullOrEmpty(dates.EndDate)) {
                this._selenium.Click("weekly_ends");
                this._selenium.Type("weekly_end_date", dates.EndDate.ToString());
            }
        }

        /// <summary>
        ///  Populates the weeklyfrequency option for Scheduled Giving
        /// </summary>
        /// <param name="dates">The dates for the frequency to start.  This is an anonymous type.</param>
        private void Giving_ScheduledGiving_Populate_WeeklyFrequency_WebDriver(dynamic dates)
        {
            this._driver.FindElementById("frequency_3").Click();
            new SelectElement(this._driver.FindElementById("weekly_start_day")).SelectByText(dates.StartDay.ToString());
            new SelectElement(this._driver.FindElementById("weekly_start_date")).SelectByText(dates.StartDate.ToString());

            // If there is an end month, select the end check box and populate the data
            if (!string.IsNullOrEmpty(dates.EndDate))
            {
                this._driver.FindElementById("weekly_ends").Click();
                this._driver.FindElementById("weekly_end_date").SendKeys(dates.EndDate.ToString());
            }
        }


        /// <summary>
        ///  Populates the every two weeks option for Scheduled Giving
        /// </summary>
        /// <param name="dates">The dates for the frequency to start.  This is an anonymous type.</param>
        private void Giving_ScheduledGiving_Populate_EveryTwoWeeksFrequency(dynamic dates) {
            this._selenium.Click("frequency_4");
            this._selenium.Select("alt_weekly_start_day", dates.StartDay.ToString());
            this._selenium.Type("alt_weekly_start_date", dates.StartDate.ToString());

            // If there is an end month, select the end check box and populate the data
            if (!string.IsNullOrEmpty(dates.EndDate)) {
                this._selenium.Click("alt_weekly_ends");
                this._selenium.Type("alt_weekly_end_date", dates.EndDate.ToString());
            }
        }

        /// <summary>
        ///  Populates the every two weeks option for Scheduled Giving
        /// </summary>
        /// <param name="dates">The dates for the frequency to start.  This is an anonymous type.</param>
        private void Giving_ScheduledGiving_Populate_EveryTwoWeeksFrequency_WebDriver(dynamic dates)
        {
            this._driver.FindElementById("frequency_4").Click();
            new SelectElement(this._driver.FindElementById("alt_weekly_start_day")).SelectByText(dates.StartDay.ToString());
            this._driver.FindElementById("alt_weekly_start_date").SendKeys(dates.StartDate.ToString());

            // If there is an end month, select the end check box and populate the data
            if (!string.IsNullOrEmpty(dates.EndDate))
            {
                this._driver.FindElementById("alt_weekly_ends").Click();
                this._driver.FindElementById("alt_weekly_end_date").SendKeys(dates.EndDate.ToString());
            }
        }

        /// <summary>
        /// Populates the payment information on Step 3 of Scheduled Giving and the Edit Page for Scheduled Giving.
        /// </summary>
        /// <param name="paymentType">The payment type.</param>
        /// <param name="firstName">The first name of the contributor.</param>
        /// <param name="lastName">The last name of the contributor.</param>
        /// <param name="creditCardType">The credit card type, if applicable.</param>
        /// <param name="creditCardNumber">The credit card number, if applicable.</param>
        /// <param name="expirationMonth">The expiration month, if applicable.</param>
        /// <param name="expirationYear">The expiration year, if applicable.</param>
        /// <param name="securityCode">The security code, if applicable.</param>
        /// <param name="country">The country of the billing address.</param>
        /// <param name="streetOne">The street one info for the billing address.</param>
        /// <param name="streetTwo">The street two info for the billing address.</param>
        /// <param name="city">The city for the billing address.</param>
        /// <param name="state">The state for the billing address.</param>
        /// <param name="postalCode">The postal code for the billing address.</param>
        /// <param name="county">The county for the billing address.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="routingNumber">The routing number.</param>
        /// <param name="accountNumber">The account number.</param>
        private void Giving_ScheduledGiving_Populate_PaymentInformation(string paymentType, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, string country, string streetOne, string streetTwo, string city, string state, string postalCode, string county, string phoneNumber, string routingNumber, string accountNumber) {

            var lastFourDigits = string.Empty;

            if (paymentType == "Credit Card") {
                // Select the Credit Card radio button if present
                if (this._selenium.IsElementPresent("payment_method_cc")) {
                    this._selenium.Click("payment_method_cc");
                }

                // Always enter a new credit card
                if (this._selenium.IsElementPresent("link=Use a different credit card")) {
                    this._selenium.Click("link=Use a different credit card");
                }

                // Specify the personal information
                if (!string.IsNullOrEmpty(firstName)) {
                    this._selenium.Type("FirstName", firstName);
                }
                else {
                    this._selenium.Type("FirstName", string.Empty);
                }
                if (!string.IsNullOrEmpty(lastName)) {
                    this._selenium.Type("LastName", lastName);
                }
                else {
                    this._selenium.Type("LastName", string.Empty);
                }
                if (!string.IsNullOrEmpty(creditCardType)) {
                    this._selenium.Select("payment_type_id", creditCardType);
                    // Non international
                    if ((creditCardType == "Visa") || (creditCardType == "Master Card") || (creditCardType == "American Express") || (creditCardType == "Discover") || (creditCardType == "JCB")) {

                        if (!string.IsNullOrEmpty(expirationMonth)) {
                            this._selenium.Select("expiration_month", expirationMonth);
                        }

                        if (!string.IsNullOrEmpty(expirationYear)) {
                            this._selenium.Select("expiration_year", expirationYear);
                        }

                        if (!string.IsNullOrEmpty(securityCode)) {
                            this._selenium.Type("CVC", securityCode);
                        }
                    }

                    // International
                    //else if ((creditCardType == "Switch") || (creditCardType == "Solo")) {
                    //    if (!string.IsNullOrEmpty(expirationMonth)) {
                    //        this._selenium.Select("valid_from", expirationMonth);
                    //    }
                    //    if (!string.IsNullOrEmpty(expirationYear)) {
                    //        this._selenium.Select("valid_from_year", expirationYear);
                    //    }

                    //    if (!string.IsNullOrEmpty(securityCode)) {
                    //        this._selenium.Type("IssueNumber", securityCode);
                    //    }
                    //}

                }
                if (!string.IsNullOrEmpty(creditCardNumber)) {
                    this._selenium.Type("cc_account_number", creditCardNumber);
                }


                // Update the billing address
                if (!string.IsNullOrEmpty(country)) {
                    this._selenium.Select("Country", country);
                }

                if (!string.IsNullOrEmpty(streetOne)) {
                    this._selenium.Type("Address1", streetOne);
                }

                if (!string.IsNullOrEmpty(streetTwo)) {
                    this._selenium.Type("Address2", streetTwo);
                }

                if (!string.IsNullOrEmpty(city)) {
                    this._selenium.Type("City", city);
                }

                if (!string.IsNullOrEmpty(state)) {
                    this._selenium.Select("state", state);
                }

                if (!string.IsNullOrEmpty(postalCode)) {
                    this._selenium.Type("PostalCode", postalCode);
                }

                if (!string.IsNullOrEmpty(county)) {
                    this._selenium.Type("County", county);
                }

                // Store the last four digits of the credit card
                if (!string.IsNullOrEmpty(creditCardNumber)) {
                    lastFourDigits = creditCardNumber.Substring(creditCardNumber.Length - 4);
                }

            }
            else {
                // Select echeck
                this._selenium.Click("payment_method_check");

                // Store the payment type
                paymentType = "Personal Check";

                // Enter the check information
                if (!string.IsNullOrEmpty(phoneNumber)) {
                    this._selenium.Type("phone", phoneNumber);
                }

                if (!string.IsNullOrEmpty(routingNumber)) {
                    this._selenium.Type("routing_number", routingNumber);
                }

                if (!string.IsNullOrEmpty(accountNumber)) {
                    this._selenium.Type("//input[@id='account_number']", accountNumber);

                    // Store the last four digits
                    lastFourDigits = accountNumber.Substring(accountNumber.Length - 4);
                }
            }
        }

        /// <summary>
        /// Populates the payment information on Step 3 of Scheduled Giving and the Edit Page for Scheduled Giving.
        /// </summary>
        /// <param name="paymentType">The payment type.</param>
        /// <param name="firstName">The first name of the contributor.</param>
        /// <param name="lastName">The last name of the contributor.</param>
        /// <param name="creditCardType">The credit card type, if applicable.</param>
        /// <param name="creditCardNumber">The credit card number, if applicable.</param>
        /// <param name="expirationMonth">The expiration month, if applicable.</param>
        /// <param name="expirationYear">The expiration year, if applicable.</param>
        /// <param name="securityCode">The security code, if applicable.</param>
        /// <param name="country">The country of the billing address.</param>
        /// <param name="streetOne">The street one info for the billing address.</param>
        /// <param name="streetTwo">The street two info for the billing address.</param>
        /// <param name="city">The city for the billing address.</param>
        /// <param name="state">The state for the billing address.</param>
        /// <param name="postalCode">The postal code for the billing address.</param>
        /// <param name="county">The county for the billing address.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="routingNumber">The routing number.</param>
        /// <param name="accountNumber">The account number.</param>
        private void Giving_ScheduledGiving_Populate_PaymentInformation_WebDriver(string paymentType, string firstName, string lastName, string creditCardType, string creditCardNumber, string expirationMonth, string expirationYear, string securityCode, string country, string streetOne, string streetTwo, string city, string state, string postalCode, string county, string phoneNumber, string routingNumber, string accountNumber)
        {

            var lastFourDigits = string.Empty;

            if (paymentType == "Credit Card")
            {
                // Select the Credit Card radio button if present
                if (this._generalMethods.IsElementPresentWebDriver(By.Id("payment_method_cc")))
                {
                    this._driver.FindElementById("payment_method_cc").Click();
                }

                // Always enter a new credit card
                if (this._generalMethods.IsElementPresentWebDriver(By.LinkText("Use a different credit card")))
                {
                    this._driver.FindElementByLinkText("Use a different credit card").Click();
                }

                // Specify the personal information
                if (!string.IsNullOrEmpty(firstName))
                {
                    this._driver.FindElementById("FirstName").SendKeys(firstName);
                }
                else
                {
                    this._driver.FindElementById("FirstName").SendKeys(string.Empty);
                }
                if (!string.IsNullOrEmpty(lastName))
                {
                    this._driver.FindElementById("LastName").SendKeys(lastName);
                }
                else
                {
                    this._driver.FindElementById("LastName").SendKeys(string.Empty);
                }
                if (!string.IsNullOrEmpty(creditCardType))
                {
                    new SelectElement(this._driver.FindElementById("payment_type_id")).SelectByText(creditCardType);
                    // Non international
                    if ((creditCardType == "Visa") || (creditCardType == "Master Card") || (creditCardType == "American Express") || (creditCardType == "Discover") || (creditCardType == "JCB"))
                    {

                        if (!string.IsNullOrEmpty(expirationMonth))
                        {
                            new SelectElement(this._driver.FindElementById("expiration_month")).SelectByText(expirationMonth);
                        }

                        if (!string.IsNullOrEmpty(expirationYear))
                        {
                            new SelectElement(this._driver.FindElementById("expiration_year")).SelectByText(expirationYear);
                        }

                        if (!string.IsNullOrEmpty(securityCode))
                        {
                            this._driver.FindElementById("CVC").SendKeys(securityCode);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(creditCardNumber))
                {
                    this._driver.FindElementById("cc_account_number").SendKeys(creditCardNumber);
                }


                // Update the billing address
                if (!string.IsNullOrEmpty(country))
                {
                    new SelectElement(this._driver.FindElementById("Country")).SelectByText(country);
                }

                if (!string.IsNullOrEmpty(streetOne))
                {
                    this._driver.FindElementById("Address1").SendKeys(streetOne);
                }

                if (!string.IsNullOrEmpty(streetTwo))
                {
                    this._driver.FindElementById("Address2").SendKeys(streetTwo);
                }

                if (!string.IsNullOrEmpty(city))
                {
                    this._driver.FindElementById("City").SendKeys(city);
                }

                if (!string.IsNullOrEmpty(state))
                {
                    new SelectElement(this._driver.FindElementById("state")).SelectByText(state);
                }

                if (!string.IsNullOrEmpty(postalCode))
                {
                    this._driver.FindElementById("PostalCode").SendKeys(postalCode);
                }

                if (!string.IsNullOrEmpty(county))
                {
                    this._driver.FindElementById("County").SendKeys(county);
                }

                // Store the last four digits of the credit card
                if (!string.IsNullOrEmpty(creditCardNumber))
                {
                    lastFourDigits = creditCardNumber.Substring(creditCardNumber.Length - 4);
                }

            }
            else
            {
                // Select echeck
                this._driver.FindElementById("payment_method_check").Click();

                // Store the payment type
                paymentType = "Personal Check";

                // Enter the check information
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    this._driver.FindElementById("phone").SendKeys(phoneNumber);
                }

                if (!string.IsNullOrEmpty(routingNumber))
                {
                    this._driver.FindElementById("routing_number").SendKeys(routingNumber);
                }

                if (!string.IsNullOrEmpty(accountNumber))
                {
                    this._driver.FindElementByXPath("//input[@id='account_number' and preceding-sibling::label[contains(text(), 'Account number *')]]").SendKeys(accountNumber);

                    // Store the last four digits
                    lastFourDigits = accountNumber.Substring(accountNumber.Length - 4);
                }
            }
        }

        #endregion Scheduled Giving

        #endregion Giving

        #region Groups
        /// <summary>
        /// Updates the bulletin board for a group.
        /// </summary>
        /// <param name="updatedText">The text for the bulletin board post.</param>
        /// <param name="authorName">The name of the person updating the bulletin board.</param>
        /// <param name="publish">Specifies if you want to publish this update.</param>
        private void Groups_UpdateBulletinBoard(string updatedText, string authorName, bool publish) {
            this._selenium.Type("post", updatedText);

            if (publish != this._selenium.IsChecked("publish"))
            {
                this._selenium.Click("publish");
            }
            //// Do we want to publish?
            //if (publish) {
            //    // We do. Is the checkbox already checked?
            //    if (!this._selenium.IsChecked("publish")) {
            //        // It isn't. Check it
            //        this._selenium.Click("publish");
            //    }

            //}
            //else {
            //    // We don't.  Is the checkbox checked?
            //    if (this._selenium.IsChecked("publish")) {
            //        // It is. Uncheck it
            //        this._selenium.Click("publish");
            //    }
            //}

            // Submit
            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Store the timestamp
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            
            var publishString = string.Format("Published {0}, {1} {2}, {3} by {4}", today.DayOfWeek, System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(today.Month), today.Day, today.Year, authorName);        
            // Verify
            if (publish) {
                this._selenium.VerifyTextPresent(publishString);
            }

            this._selenium.VerifyTextPresent(updatedText);
        }
        //overload this above method to support timezone by churchcode,add by grace zhang
        private void Groups_UpdateBulletinBoard(string updatedText, string authorName, bool publish, String churchcode)
        {
            this._selenium.Type("post", updatedText);

            if (publish != this._selenium.IsChecked("publish"))
            {
                this._selenium.Click("publish");
            }
            //// Do we want to publish?
            //if (publish) {
            //    // We do. Is the checkbox already checked?
            //    if (!this._selenium.IsChecked("publish")) {
            //        // It isn't. Check it
            //        this._selenium.Click("publish");
            //    }

            //}
            //else {
            //    // We don't.  Is the checkbox checked?
            //    if (this._selenium.IsChecked("publish")) {
            //        // It is. Uncheck it
            //        this._selenium.Click("publish");
            //    }
            //}

            // Submit
            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Store the timestamp
            int churchId = this._sql.Ministry_Church_FetchID(churchcode);
            String timeZoneName = this._sql.Ministry_Activity_Instance_TimeZone(churchId);
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            TestLog.WriteLine("today==" + today);
            var publishString = string.Format("Published {0}, {1} {2}, {3} by {4}", today.DayOfWeek, System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(today.Month), today.Day, today.Year, authorName);
            TestLog.WriteLine("publishString==" + publishString);
            // Verify
            if (publish)
            {
                this._selenium.VerifyTextPresent(publishString);
            }

            this._selenium.VerifyTextPresent(updatedText);
        }
        //Gracezhang add for FO-3435
        private void Groups_UpdateBulletinBoard_UsingMarkdown(string updatedText, String expectUpdatedText, string authorName, bool publish)
        {
            this._selenium.Type("post", updatedText);

            if (publish != this._selenium.IsChecked("publish"))
            {
                this._selenium.Click("publish");
            }
            //// Do we want to publish?
            //if (publish) {
            //    // We do. Is the checkbox already checked?
            //    if (!this._selenium.IsChecked("publish")) {
            //        // It isn't. Check it
            //        this._selenium.Click("publish");
            //    }

            //}
            //else {
            //    // We don't.  Is the checkbox checked?
            //    if (this._selenium.IsChecked("publish")) {
            //        // It is. Uncheck it
            //        this._selenium.Click("publish");
            //    }
            //}

            // Submit
            this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Store the timestamp
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            var publishString = string.Format("Published {0}, {1} {2}, {3} by {4}", today.DayOfWeek, System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(today.Month), today.Day, today.Year, authorName);

            // Verify
            if (publish)
            {
                this._selenium.VerifyTextPresent(publishString);
            }

            this._selenium.VerifyTextPresent(expectUpdatedText);
        }

        /// <summary>
        /// Updates the bulletin board for a group.
        /// </summary>
        /// <param name="updatedText">The text for the bulletin board post.</param>
        /// <param name="authorName">The name of the person updating the bulletin board.</param>
        /// <param name="publish">Specifies if you want to publish this update.</param>
        private void Groups_UpdateBulletinBoard_WebDriver(string updatedText, string authorName, bool publish)
        {
            this._driver.FindElementById("post").SendKeys(updatedText);

            // Do we want to publish?
            if (publish)
            {
                // We do. Is the checkbox already checked?
                if (!this._driver.FindElementById("publish").Selected)
                {
                    // It isn't. Check it
                    this._driver.FindElementById("publish").Click();
                }

            }
            else
            {
                // We don't.  Is the checkbox checked?
                if (this._driver.FindElementById("publish").Selected)
                {
                    // It is. Uncheck it
                    this._driver.FindElementById("publish").Click();
                }
            }

            // Submit
            var btn = this._driver.FindElementsById(GeneralButtons.submitQuery).FirstOrDefault(x => x.Displayed == true);
            btn.Click();
            //this._driver.FindElementById(GeneralButtons.submitQuery)..Click();
            this._generalMethods.WaitForElement(this._driver, By.LinkText("Edit details"));

            // Store the timestamp
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            var publishString = string.Format("Published {0}, {1} {2}, {3} by {4}", today.DayOfWeek, System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(today.Month), today.Day, today.Year, authorName);

            // Verify
            if (publish)
            {
                this._generalMethods.VerifyTextPresentWebDriver(publishString);
            }

            this._generalMethods.VerifyTextPresentWebDriver(updatedText);
        }


        #endregion Groups

        #region AMS Specific

        #region AMS Error Text
        private string Get_AMS_Error_Text(string reasonCode)
        {

            StringBuilder amsErrorText = new StringBuilder();

            switch (reasonCode)
            {
                case "150":
                case "200":
                case "209":
                case "211":
                    amsErrorText.Append("Authorization was not successful. ");
                    amsErrorText.Append("Please check your information and try again. An error has occurred while attempting to process the request.");
                    break;
                case "201":
                case "203":
                case "205":
                case "208":
                case "233":
                    amsErrorText.Append("Authorization was not successful. ");
                    amsErrorText.Append("Please check your information including your billing address to make sure they are accurate, or use another card or select another form of payment.");
                    break;
                case "204":
                    amsErrorText.Append("Authorization was not successful. ");
                    amsErrorText.Append("Insufficient funds in the account. Please use a different card or select another form of payment.");
                    break;
                case "231":
                    //• The credit card number entered is invalid.
                    //amsErrorText.AppendFormat("There is missing or incorrect information. Please complete the form and submit again.\n{0} The credit card number entered is invalid.", HttpUtility.HtmlEncode("&#149;"));
                    //amsErrorText.Append("There is missing or incorrect information. Please complete the form and submit again.\n• The credit card number entered is invalid.");
                    amsErrorText.Append("There is missing or incorrect information. Please complete the form and submit again.\r\n• The credit card number entered is invalid.");
                    break;
                case "234":
                    amsErrorText.Append("Authorization was not successful. Please contact your Church Administrator");
                    break;
                case "Velocity_Amount":
                case "Velocity_Count":
                    amsErrorText.Append("We're sorry, we are unable to process your payment at this time. For more information contact our office.");
                    break;
                case "2009":
                    //amsErrorText.Append("Authorization was not successful. The minimum amount allowed is ").Append(string.Format("{0:c}", GetMinimumAmount() + .01m)).Append(". This minimum exists to combat fraudulent activity.");
                    break;
                default:
                    amsErrorText.Append("Authorization was not successful. ");
                    amsErrorText.Append("Please check your information and try again. An error has occurred while attempting to process the request.");
                    break;

            }

            return amsErrorText.ToString();

        }
        #endregion AMS Error Text

        #endregion AMS Specific

        #region Captcha        

        /// <summary>
        /// Get's Captchat Text via DeathByCaptcha API
        /// http://www.deathbycaptcha.com/user/login
        /// </summary>
        /// <param name="element">IWebElement where captcha is present</param>
        /// <returns></returns>
        private Captcha Get_Captcha_Text_WebDriver(By element)
        {
            Captcha response;
            SocketClient client;
            string captchaText = string.Empty;
            ITakesScreenshot webDriver = (ITakesScreenshot)this._driver;
            byte[] ss = webDriver.GetScreenshot().AsByteArray;

            using (MemoryStream msScreen = new MemoryStream(ss))
            {
                Bitmap bitmap = new Bitmap(msScreen);
                IWebElement captchaElement = this._driver.FindElement(element);
                Rectangle rcCrop = new Rectangle(captchaElement.Location, captchaElement.Size);
                Image imgCaptcha = bitmap.Clone(rcCrop, bitmap.PixelFormat);


                using (MemoryStream msCaptcha = new MemoryStream())
                {
                    imgCaptcha.Save(msCaptcha, ImageFormat.Png);

                    //Connect
                    client = new SocketClient("ft.tester", "FT4life!");
                    log.Debug("Sending request to DeathByCaptcha");
                    response = client.Decode(msCaptcha.GetBuffer(), 60);

                    if (response != null && response.Solved && response.Correct)
                    {
                        captchaText = response.Text;
                        TestLog.WriteLine(string.Format("Captcha Received: {0}", captchaText));

                    }
                    else
                    {
                        log.Error("Captcha recognition error");                       
                        throw new WebDriverException("Captcha recognition error." + response.ToString());
                    }

                }

            }

            client.Close();

            return response;

        }

        /// <summary>
        /// Send Captha Report Error
        /// </summary>
        /// <param name="response"></param>
        private void Send_Captcha_Report_Error(Captcha response)
        {
            SocketClient client = new SocketClient("ft.tester", "FT4life!");
            log.Debug("Sending error report to DeathByCaptcha");
            client.Report(response);
            client.Close();

        }

        /// <summary>
        /// Enter Captcha in input Text
        /// </summary>
        /// <param name="captchaText">Captcha Text Returned</param>
        /// <param name="captchaErrorTxt">Captcha error message expected if any</param>
        /// <returns>Captcha for use in error notification to DeathByCaptcha</returns>
        private Captcha Enter_Captcha_Text(string captchaText, string captchaErrorTxt)
        {
            Captcha captcha = null;

            this._generalMethods.WaitForElementDisplayed(By.Id("VerificationCaptcha_CaptchaImage"));

            if (string.IsNullOrEmpty(captchaText))
            {
                captcha = this.Get_Captcha_Text_WebDriver(By.Id("VerificationCaptcha_CaptchaImage"));
                this._driver.FindElementById("CaptchaCode").SendKeys(captcha.Text);
            }
            else
            {
                //Input passed in captcha for error handling
                this._driver.FindElementById("CaptchaCode").SendKeys(captchaText);
                //_errorText.Add("The captcha was invalid.");
                if (!_errorText.Contains(captchaErrorTxt))
                {
                    _errorText.Add(captchaErrorTxt);
                }
            }

            return captcha;
        }

        /// <summary>
        /// Checks if we only have an Capthca error. Used when we are trying to resent again
        /// </summary>
        /// <param name="captchaText"></param>
        /// <param name="captchaErrorTxt"></param>
        /// <returns></returns>
        private bool Is_Captcha_Error_Only(string captchaText, string captchaErrorTxt)
        {
            bool isCaptchaError = false;

            if (this._generalMethods.IsElementPresentWebDriver(By.XPath("//div[@class='error_msgs_for']")))
            {
                //How many errors do we have
                int errors = this._driver.FindElementByXPath("//div[@class='error_msgs_for']").FindElement(By.TagName("ul")).FindElements(By.TagName("li")).Count;

                //Loops through the errors and verify them
                for (int e = 0; e < errors; e++)
                {
                    log.DebugFormat("Errors: {0}", this._driver.FindElementByXPath("//div[@class='error_msgs_for']").FindElement(By.TagName("ul")).FindElements(By.TagName("li"))[e].Text);
                    string err = this._driver.FindElementByXPath("//div[@class='error_msgs_for']").FindElement(By.TagName("ul")).FindElements(By.TagName("li"))[e].Text;

                    //If we did not pass captcha text we got a captcha error
                    if ((err.Equals(captchaErrorTxt)) && (string.IsNullOrEmpty(captchaText)))
                    {
                        //If we only have one then, it's only captcha error so yes
                        if (errors == 1)
                        {
                            isCaptchaError = true;
                        }
                    }
                }
            }

            log.DebugFormat("Captcha Error Only: {0}", isCaptchaError);
            return isCaptchaError;
        }

        #endregion Captcha

        #region EventRegistration

        #endregion EventRegistration

        #endregion Private Methods

        #region V2 Giving

        public void Giving_V2_CheckOut_DeleteExistingGifts()
        {
            bool isTrue = true;

            // Go to Check Out page
            this._driver.FindElementByXPath(".//*[@id='sidebar']//*/span[text()='Checkout']").Click();

            //Check if there already have gifts in basket, Delete the existing gifts in basket          

            do
            {
                try
                {
                    this._generalMethods.WaitForElement(By.XPath(".//*[@id='items']/*//a[text()='Delete']"));
                    this._driver.FindElementByXPath(".//*[@id='items']/*//a[text()='Delete']").Click();

                    this._generalMethods.WaitForElement(By.XPath(".//*//div[text()='OK']"));
                    this._driver.FindElementByXPath(".//*//div[text()='OK']").Click();
                }
                catch (WebDriverException)
                {
                    isTrue = false;
                }


            }
            while (isTrue);
        }

        public void Giving_V2_GiveNow_AddGiftToBasket()
        {
            // select the first found Gift on V2 Give Now page
            this._driver.FindElementByXPath("html/body/*//a[1]/em/span[text()='Give Now']").Click();

            //Check if there are any available gifts to do Give Now
            Assert.IsTrue(this._generalMethods.IsElementExist(By.XPath(".//*[@id='projectBrowseid1View']/*//div/a")), "there is no gift available");

            //Click Give Now to add a gift to basket
            this._driver.FindElementByXPath(".//*[@id='projectBrowseid1View']/*//div/a").Click();

            Assert.IsTrue(this._generalMethods.IsElementExist(By.XPath(".//*[@id='v2AddItemActionButtons']/a[2][text()='Continue to Checkout']")), "Failed - cannot add gift to basket");

            this._driver.FindElementByXPath(".//*[@id='v2AddItemActionButtons']/a[2][text()='Continue to Checkout']").Click();
            System.Threading.Thread.Sleep(6000);

        }
        #endregion V2 Giving

    }
}
