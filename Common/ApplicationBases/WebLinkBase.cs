using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;

using log4net;

namespace FTTests {
    public class WeblinkBase {
        private RemoteWebDriver _driver;
        private string _weblinkEmail;
        private string _weblinkPassword;
        private string _encryptedChurchCode;
        private string _url;
        private string _portalUser;
        private GeneralMethods _generalMethods;
        private F1Environments _f1Environment;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Properties
        public string WeblinkEmail {
            set { _weblinkEmail = value; }
            get { return _weblinkEmail; }
        }

        public string WebLinkPassword {
            set { _weblinkPassword = value; }
            get { return _weblinkPassword; }
        }

        public string EncryptedChurchCode {
            set { _encryptedChurchCode = value; }
            get { return _encryptedChurchCode; }
        }

        public string URL {
            set { _url = value; }
            get { return _url; }
        }
        #endregion Properties

        public WeblinkBase(RemoteWebDriver driver, string portalUser, GeneralMethods generalMethods, F1Environments f1Environment)
        {
            this._driver = driver;
            this._portalUser = portalUser;
            this._generalMethods = generalMethods;
            this._f1Environment = f1Environment;
        }

        #region Instance Methods

        public void Login() {
            // Open the weblink login page
            this.OpenPage(this.GetLoginURL());

            VerifyPrivacyPolicy("login");

            // Enter credentials
            this._driver.FindElementById("txtEmail").SendKeys(this._weblinkEmail);
            this._driver.FindElementById("txtPassword").SendKeys(this._weblinkPassword);

            // Attempt to login
            this._driver.FindElementById("btnSignIn").Click();


        }

        public void Login(string encryptedChurchCode, string login, string password) {
            // Open the weblink login page
            this.OpenPage(this.GetLoginURL(encryptedChurchCode));

            VerifyPrivacyPolicy("login");

            // Enter credentials
            this._driver.FindElementById("txtEmail").SendKeys(login);
            this._driver.FindElementById("txtPassword").SendKeys(password);

            // Attempt to login
            this._driver.FindElementById("btnSignIn").Click();
        }

        public void Logout() {
            // Open the weblink logout page
            this.OpenPage(this.GetLogoutURL());
            VerifyPrivacyPolicy("Logout Page");
        }

        public void Logout(string encryptedChurchCode)
        {
            // Open the weblink logout page
            this.OpenPage(this.GetLogoutURL(encryptedChurchCode));
            VerifyPrivacyPolicy("Logout Page");
        }

        public void ViewCreateAccount() {
            this.OpenPage(this.GetCreateAccountURL());
            VerifyPrivacyPolicy("Create Account");

        }

        public void ViewEditProfile() {
            this.Login();
            this.OpenPage(this.GetEditProfileURL());
            VerifyPrivacyPolicy("Edit Profile");

        }

        public void ViewResetPassword() {
            this.Login();
            this.OpenPage(this.GetResetPasswordURL());
            VerifyPrivacyPolicy("Reset Password");

        }

        public void ViewHelp() {
            this.OpenPage(this.GetLoginHelpURL());
            VerifyPrivacyPolicy("Help");

        }

        public void ViewForgotPassword()
        {
            this.OpenPage(this.GetForgotPasswordURL());
        }


        public void ViewOnlineGiving() {
            this.Login();
            this.OpenPage(this.GetOnlineGivingURL());
            VerifyPrivacyPolicy("Online Giving");

        }

        public void ViewVolunteerApplication(string iCode) {
            string volunteerApplicationURL = string.Format("{0}&iCode=", this.GetVolunteerApplicationURL(), iCode);

            this.Login();
            this.OpenPage(volunteerApplicationURL);
            VerifyPrivacyPolicy("Volunteer Application");

        }

        public void ViewRelationshipManager(string encryptedChurchCode, string userEmail, string password)
        {
            this.OpenPage(this.GetRelationshipManagerURL(encryptedChurchCode));

            VerifyPrivacyPolicy("login");

            // Enter credentials
            this._driver.FindElementById("txtEmail").SendKeys(userEmail);
            this._driver.FindElementById("txtPassword").SendKeys(password);

            // Attempt to login
            this._driver.FindElementById("btnSignIn").Click();
        }

        /// <summary>
        /// Views the online giving page for a given church and user.
        /// </summary>
        /// <param name="encryptedChurchCode">The encrypted church code.</param>
        /// <param name="login">The user login.</param>
        /// <param name="password">The user password.</param>
        public void ViewOnlineGiving(string encryptedChurchCode, string login, string password) {
            this.Login(encryptedChurchCode, login, password);
            this.OpenPage(this.GetOnlineGivingURL(encryptedChurchCode));
            VerifyPrivacyPolicy("Online Giving");

        }

        public void ViewContactForm() {
            this.Login();
            this.OpenPage(this.GetContactFormURL());
            VerifyPrivacyPolicy("Contact Form");

        }

        /// <summary>
        /// Views the add individual URL for event registration.
        /// </summary>
        public void ViewEventRegistrationAddIndividualURL() {
            this.OpenPage(this.GetEventRegistrationAddIndividualURL());
            VerifyPrivacyPolicy("Event Registration Add Individual");

        }

        public void ViewEventRegistration()
        {
            this.OpenPage(this.GetEventRegistrationURL());
        }

        /// <summary>
        /// Creates an account in WebLink.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="email">The email address.</param>
        /// <param name="password">The password.</param>
        public void CreateAccount(string firstName, string lastName, string email, string password) {
            // Attempt to create an account
            this.OpenPage(this.GetLoginURL());

            this._driver.FindElementByLinkText("Create an account").Click();

            this._driver.FindElementById("txtFirstname_textBox").SendKeys(firstName);
            this._driver.FindElementById("txtLastname_textBox").SendKeys(lastName);
            this._driver.FindElementById("txtEmail_textBox").SendKeys(email);
            this._driver.FindElementById("txtPassword_textBox").SendKeys(password);
            this._driver.FindElementById("txtPassword2_textBox").SendKeys(password);

            VerifyPrivacyPolicy("Create Account");

            this._driver.FindElementById("btnCreate").Click();


        }

        public void FetchActivationCodeAndOpenAccountCreationPage() {
            // Get the activation code from the url from the resulting page
            Regex activationCodeRegEx = new Regex("[0-9, a-f]{32}");
            string activationCode = activationCodeRegEx.Match(this._driver.Url).Value;

            string output = string.Format("{0}&activationCode={1}", this.GetAccountCreationInfoURL(), activationCode);

            // Open the page
            this.OpenPage(output);
            VerifyPrivacyPolicy("Fetch Activation and Open Account");

        }

        /// <summary>
        /// Creates a contribution in WebLink.
        /// </summary>
        /// <param name="amount">The amount of the contribution</param>
        /// <param name="frequency">The frequency of the contribution</param>
        /// <param name="fund">The fund of the contribution</param>
        /// <param name="subFund">The sub fund of the contribution</param>
        /// <param name="pledgeDrive">The pledge drive of the contribution</param>
        /// <param name="date">The date of the contribution</param>
        /// <param name="gifts">The number of gifts if contribution is a schedule</param>
        /// <param name="method">The payment method of the contribution</param>
        /// <param name="values">The collection of values for payment information</param>
        public void OnlineGiving_EnterContribution(double amount, string frequency, string fund, string subFund, string pledgeDrive, string date, int? gifts, string method, string[] values) {
            // Populate the necessary fields
            this._driver.FindElementById("txtAmount_textBox").SendKeys(amount.ToString());

            if (!string.IsNullOrEmpty(frequency)) {
                new SelectElement(this._driver.FindElementById("ddlFrequency")).SelectByText(frequency);

                if (frequency != "One time") {
                    string waitId = frequency == "Monthly - Last day of the month" ? "dtbMonthYearStartDate" : "_ctl1_DateTextBox";
                    this._driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(1000));
                    IWebElement waitElement = this._driver.FindElementByXPath(waitId);
                }
            }

            // Designation
            string designation = string.Empty;
            if (!string.IsNullOrEmpty(fund)) {
                new SelectElement(this._driver.FindElementById("ddlFund_dropDownList")).SelectByText(fund);
                //WebDriverWait wait = new WebDriverWait(this._driver, new TimeSpan(0, 0, 3));
                //IJavaScriptExecutor javascript = this._driver as IJavaScriptExecutor;
                //wait.Until((d) => { string readyState = javascript.ExecuteScript("if (document.readyState) return document.readyState;").ToString(); return readyState.ToLower() == "complete"; });

                designation = fund;
            }

            if (!string.IsNullOrEmpty(subFund)) {
                new SelectElement(this._driver.FindElementById("ddlSubFund_dropDownList")).SelectByText(subFund);
                designation = string.Format("{0} - {1}", fund, subFund);
            }
            else if (this._driver.FindElementsById("ddlSubFund_dropDownList").Count > 0) {
                // Select the empty element
                new SelectElement(this._driver.FindElementById("ddlSubFund_dropDownList")).SelectByText("");
            }

            if (!string.IsNullOrEmpty(pledgeDrive)) {
                this._driver.FindElementById("rdbtnPledges").Click();
                new SelectElement(this._driver.FindElementById("ddlPledges_dropDownList")).SelectByText(pledgeDrive);
                designation = pledgeDrive;
            }

            if (!string.IsNullOrEmpty(date)) {
                if (frequency != "One time") {
                    string dateElement = frequency == "Monthly - Last day of the month" ? "dtbMonthYearStartDate" : "_ctl1_DateTextBox";
                    this._driver.FindElementById(dateElement).SendKeys(date);

                    if (gifts > 0) {
                        this._driver.FindElementById("rblLength_1").Click();
                        this._driver.FindElementById("txtNumberOfGifts").SendKeys(gifts.ToString());
                    }
                }
                else {
                    this._driver.FindElementById("_ctl3_DateTextBox").SendKeys(date);
                }
            }
            else {
                this._driver.FindElementById("rbImmediate").Click();
                this._driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(1000));
                IWebElement submitPaymentButton = this._driver.FindElementByXPath(string.Format("//input[@id='btnNew' and @value='Submit Payment Now']"));
            }

            if (method == "Visa" || method == "American Express") {
                new SelectElement(this._driver.FindElementById("ddlPaymentMethod_dropDownList")).SelectByText(method);
                this._driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(1000));
                IWebElement myDynamicElement = this._driver.FindElementById("txtcvcNumber_textBox");
                this._driver.FindElementById("txtHoldersName_textBox").SendKeys(values[0]);
                this._driver.FindElementById("txtCardNo_textBox").SendKeys(values[1]);
                this._driver.FindElementById("mytExpirationDate").SendKeys(values[2]);
            }
            else {
                new SelectElement(this._driver.FindElementById("ddlPaymentMethod_dropDownList")).SelectByText(method);
                this._driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(1000));
                IWebElement myDynamicElement = this._driver.FindElementById("txtBankName_textBox");
                this._driver.FindElementById("txtBankName_textBox").SendKeys(values[0]);
                this._driver.FindElementById("txtBankRoutingNumber_textBox").SendKeys(values[1]);
                this._driver.FindElementById("txtBankAccountNumber_textBox").SendKeys(values[2]);
                this._driver.FindElementById("txtReenterAccountNumber_textBox").SendKeys(values[2]);
                this._driver.FindElementById("ctlAddress_txtPhoneNumber_textBox").Clear();
                this._driver.FindElementById("ctlAddress_txtPhoneNumber_textBox").SendKeys("6302172170");
                this._driver.FindElementById("txtBankAgree_textBox").SendKeys("AGREE");
            }

            this._driver.FindElementById("ctlAddress_txtAddress1_textBox").Clear();
            this._driver.FindElementById("ctlAddress_txtAddress1_textBox").SendKeys("9616 Armour Dr");
            this._driver.FindElementById("ctlAddress_txtAddress2_textBox").Clear();
            this._driver.FindElementById("ctlAddress_txtCity_textBox").Clear();
            this._driver.FindElementById("ctlAddress_txtCity_textBox").SendKeys("Keller");
            new SelectElement(this._driver.FindElementById("ctlAddress_ddlState_dropDownList")).SelectByText("Texas");
            this._driver.FindElementById("ctlAddress_txtPostalCode_textBox").Clear();
            this._driver.FindElementById("ctlAddress_txtPostalCode_textBox").SendKeys("76244");

            // Submit the form
            this._driver.FindElementById("btnNew").Click();

            if (((method == "Visa" || method == "American Express") && values.Length > 3) || method == "eCheck" && values.Length > 4) {
                Assert.IsTrue(this._driver.FindElementsByXPath("//img[@src='/integration/image/warning.gif']").Count > 0);
                
                if (method == "eCheck") {
                    string errorTextECheck = (values[4] == "velocity_amount" || values[4] == "velocity_count") ? "We're sorry, we are unable to process your payment at this time. For more information contact our office." : "Authorization was not successful. Please check your information and try again.";

                    Assert.IsTrue(this._driver.FindElementByTagName("html").Text.Contains(errorTextECheck));
                }
                else {
                    string errorText = string.Empty;
                    switch (Convert.ToInt16(values[3])) {
                        case 150:
                        case 203:
                        case 209:
                        case 211:
                        case 999:
                            errorText = "Authorization was not successful. Please check your information including your billing address to make sure they are accurate, or use another card or select another form of payment. An error has occurred while attempting to process the request.";
                            break;
                        case 204:
                            errorText = "Authorization was not successful. Insufficient funds in the account. Please use a different card or select another form of payment.";
                            break;
                        case 231:
                            errorText = "There is missing or incorrect information. Please complete the form and submit again.\r\n• The credit card number entered is invalid.";
                            break;
                        default:
                            errorText = "Authorization was not successful. Please check your information including your billing address to make sure they are accurate, or use another card or select another form of payment. An error has occurred while attempting to process the request.";
                            break;
                    }

                    Assert.IsTrue(this._driver.FindElementByTagName("html").Text.Contains(errorText), string.Format("Expected Error Text: {0}", errorText));
                }
            }
            else if (date != null) {
                // Verify the payment information is present in the table
                int tableRow = this._generalMethods.GetTableRowNumberWebDriver(TableIds.Weblink.ContributionSchedule, amount.ToString(), "Amount");
                IWebElement table = this._driver.FindElementById(TableIds.Weblink.ContributionSchedule);
                Assert.AreEqual(designation, table.FindElements(By.TagName("tr"))[tableRow].FindElements(By.TagName("td"))[1].Text);
                Assert.AreEqual(amount.ToString("N2"), table.FindElements(By.TagName("tr"))[tableRow].FindElements(By.TagName("td"))[2].Text); //amount.ToString()
                Assert.AreEqual(frequency, table.FindElements(By.TagName("tr"))[tableRow].FindElements(By.TagName("td"))[3].Text);
                if (frequency != "Monthly - Last day of the month") {
                    Assert.AreEqual(date, table.FindElements(By.TagName("tr"))[tableRow].FindElements(By.TagName("td"))[4].Text);
                }
            }
            else {
                // Verify the payment information is present in the table
                Assert.IsTrue(this._driver.FindElementByTagName("html").Text.Contains("Your payment was successfully processed. Thank you for your contribution."));
                Assert.IsTrue(this._driver.FindElementByTagName("html").Text.Contains("Please DO NOT refresh this page or use the back button or you may be double charged."));
                
                Assert.AreEqual("FT Tester", this._driver.FindElementById("dgHistory__ctl2_lblAttributedTo").Text);
                Assert.AreEqual(designation, this._driver.FindElementById("dgHistory__ctl2_lblFund").Text);
                DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
                Assert.AreEqual(now.ToString("M/d/yyyy"), this._driver.FindElementById("dgHistory__ctl2_lblDate").Text);
                Assert.AreEqual(string.Format("{0:c}", amount), this._driver.FindElementById("dgHistory__ctl2_lblAmount").Text);

                Assert.IsTrue(this._driver.FindElementsByXPath("//img[@id='dgHistory__ctl2_imgContributionStatus' and @title='Transaction pending']").Count > 0);
            }

            log.Debug("Open " + this._url);
            this.OpenPage(this._url);
            //VerifyPrivacyPolicy("Payment Information");
        
        }

        /// <summary>
        /// Updates a contribution schedule.
        /// </summary>
        /// <param name="amount">The amount of the contribution schedule</param>
        /// <param name="amountUpdated">The updated amount for the schedule</param>
        /// <param name="method">The payment method of the schedule</param>
        public void OnlineGiving_UpdateScheduledContribution(double amount, double amountUpdated, string method) {
            // Edit a contribution schedule
            string commonXPath = "//table[@id='dgSchedule']/tbody/tr[*]";
            this._driver.FindElementByXPath(string.Format("{0}/td[1]/a[position()=1 and ancestor::tr/td[3]/span/text()='{1}']", commonXPath, amount)).Click();

            VerifyPrivacyPolicy("Online Giving Update Sched Contribution");
            // Update the amount
            this._driver.FindElementById("txtAmount_textBox").Clear();
            this._driver.FindElementById("txtAmount_textBox").SendKeys(amountUpdated.ToString());

            if (method == "eCheck") {
                this._driver.FindElementById("txtBankAgree_textBox").SendKeys("AGREE");
            }
            this._driver.FindElementById("btnUpdate").Click();

            // Verify the update was successful
            VerifyPrivacyPolicy("Update Schedule Contribution");
            Assert.IsTrue(this._driver.FindElementByTagName("html").Text.Contains("Your scheduled contribution has been successfully saved."));
            Assert.IsTrue(this._driver.FindElementsByXPath(string.Format("{0}/td[3]/span[text()='{1}']", commonXPath, amountUpdated)).Count > 0);
            Assert.IsFalse(this._driver.FindElementsByXPath(string.Format("{0}/td[3]/span[text()='{1}']", commonXPath, amount)).Count > 0);
        }

        /// <summary>
        /// Deletes a contribution schedule.
        /// </summary>
        /// <param name="amount">The amount of the contribution schedule</param>
        public void OnlineGiving_DeleteScheduledContribution(double amount) {
            // Delete a contribution schedule
            string commonXPath = "//table[@id='dgSchedule']/tbody/tr[*]";
            this._driver.FindElementByXPath(string.Format("{0}/td[1]/a[position()=2 and ancestor::tr/td[3]/span/text()='{1}']", commonXPath, amount)).Click();

            VerifyPrivacyPolicy("Delete Sched Contribution");
            // Verify the contribution schedule was removed
            Assert.IsFalse(this._driver.FindElementsByXPath(string.Format("{0}/td[3]/span[text()='{1}']", commonXPath, amount)).Count > 0);
        }

        /// <summary>
        /// Views the login page for WebLink.
        /// </summary>
        public void ViewLoginPage() {
            this.OpenPage(this.GetLoginURL());
            VerifyPrivacyPolicy("Login Page");
        }

        /// <summary>
        /// Views the login page for WebLink.
        /// </summary>
        /// <param name="encryptedChurchCode">The church code you wish to view.</param>
        public void ViewLoginPage(string encryptedChurchCode) {
            this.OpenPage(this.GetLoginURL(encryptedChurchCode));
            VerifyPrivacyPolicy("Login Page");
        }

        /// <summary>
        ///  Opens a page in Weblink.
        /// </summary>
        /// <param name="url">The URL for the page.</param>
        public void OpenPage(string url) {
            // Open the page
            log.Debug("Go To " + url);

            //this._driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));
            this._driver.Navigate().GoToUrl(url);

            // Accept the Compliance Cookie if needed.
            if (this._driver.FindElementsById("btnAcceptCookies").Count > 0) {
                // We need to accept cookies
                this._driver.FindElementById("btnAcceptCookies").Click();

                // Verify the cookie is present
                Assert.IsNotNull(this._driver.Manage().Cookies.GetCookieNamed("ComplianceCookie"), "Cookies were accepted by the compliance cookie was not present!");
            }
            else {
                // Verify the compliance cookie is present
                if (this._encryptedChurchCode.ToLower() == "kankGp5fXv8h5PGpcpmIpw==") {
                    Assert.IsNotNull(this._driver.Manage().Cookies.GetCookieNamed("ComplianceCookie"), "There was no prompt to accept cookies but the Compliance cookie was not present!!");
                }
            }
        }
        #endregion Instance Methods

        #region Private Methods
        private string GetLoginURL() {
            return string.Format("{0}/login.aspx?cCode={1}", this._url, this._encryptedChurchCode);
        }

        private string GetLoginURL(string encryptedChurchCode) {
            return string.Format("{0}/login.aspx?cCode={1}", this._url, encryptedChurchCode);
        }

        private string GetLogoutURL() {
            return string.Format("{0}/logout.aspx?cCode={1}", this._url, this._encryptedChurchCode);
        }

        private string GetLogoutURL(string encryptedChurchCode)
        {
            return string.Format("{0}/logout.aspx?cCode={1}", this._url, encryptedChurchCode);
        }

        private string GetCreateAccountURL() {
            return string.Format("{0}/newuser.aspx?cCode={1}", this._url, this._encryptedChurchCode);
        }

        private string GetResetPasswordURL() {
            return string.Format("{0}/resetpassword.aspx?cCode={1}", this._url, this._encryptedChurchCode);
        }

        private string GetEditProfileURL() {
            return string.Format("{0}/profileeditor.aspx?cCode={1}", this._url, this._encryptedChurchCode);
        }

        private string GetLoginHelpURL() {
            return string.Format("{0}/loginhelp.aspx?cCode={1}", this._url, this._encryptedChurchCode);
        }

        private string GetForgotPasswordURL()
        {
            return string.Format("{0}/conversion/forgotpassword.aspx?cCode={1}", this._url, this._encryptedChurchCode);
        }

        private string GetOnlineGivingURL() {
            return string.Format("{0}/contribution/onlinecontribution.aspx?cCode={1}", this._url, this._encryptedChurchCode);
        }

        private string GetOnlineGivingURL(string encryptedChurchCode) {
            return string.Format("{0}/contribution/onlinecontribution.aspx?cCode={1}", this._url, encryptedChurchCode);
        }

        private string GetVolunteerApplicationURL() {
            return string.Format("{0}/volunteer/volunteerapplication.aspx?cCode={1}", this._url, this._encryptedChurchCode);
        }

        private string GetContactFormURL() {
            return string.Format("{0}/contact/onlinecontact.aspx?cCode={1}", this._url, this._encryptedChurchCode);
        }

        private string GetEventRegistrationURL() {
            return string.Format("{0}/integration/integration/FormBuilder/FormBuilder.aspx?cCode={1}", this._url, this._encryptedChurchCode);
        }

        private string GetEventRegistrationAddIndividualURL() {
            return string.Format("{0}/registration/personalinfo.aspx?cCode={1}&anm=1", this._url, this._encryptedChurchCode);
        }

        private string GetAccountCreationInfoURL() {
            return string.Format("{0}/conversion/CreateInfo.aspx?cCode={1}", this._url, this._encryptedChurchCode);
        }
        private string GetRelationshipManagerURL(string encryptedChurchCode)
        {
            return string.Format("{0}/Relationship/RelationshipNotes.aspx?cCode={1}", this._url, encryptedChurchCode);
        }

        //F1-3363 Active Privacy Policy Links
        private void VerifyPrivacyPolicy(String landingpage)
        {

            if( (this._f1Environment.Equals("QA")) || (this._f1Environment.Equals("STAGING")) )
            {
                log.Debug("Verify Privacy Policy in " + landingpage); // + " URL: " + this._driver.Url);
                //<a target="_new" href="http://www.fellowshipone.com/privacy-policy/">Privacy Policy</a>

                Assert.IsTrue(this._driver.FindElement(By.LinkText("Privacy Policy")).Displayed, "No Privacy Policy Found");
                IWebElement privacyElement = this._driver.FindElementByLinkText("Privacy Policy");
                Assert.Contains(privacyElement.GetAttribute("href"), "www.fellowshipone.com/privacy-policy/", "Privacy Policy Link Not Found");
            }


        }

        #endregion Private Methods
    }
}