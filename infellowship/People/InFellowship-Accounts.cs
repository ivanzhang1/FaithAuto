using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Data;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Selenium;
using OpenQA.Selenium;

namespace FTTests.infellowship.People
{
    public struct AccountConstants
    {
        public struct CreateAccountPage
        {
            public const string TextField_FirstName = "firstname";
            public const string TextField_LastName = "lastname";
            public const string TextField_Email = "emailaddress";
            public const string TextField_Password = "password";
            public const string TextField_ConfirmPassword = "confirmpassword";
            public const string Button_CreateAccount = "submit";
            public const string Link_Cancel = "Cancel";

            public const string Label_YourInformation = " Your Information ";
            public const string Label_FirstName = "First Name";
            public const string Label_LastName = "Last Name";
            public const string Label_AccountInformation = " Account Information ";
            public const string Label_Email = "Email";
            public const string Label_Password = "Password";
            public const string Label_ConfirmPassword = "Confirm password";
        }

        public struct AdditionalInformationPage
        {
            public const string DateControl_DateOfBirth = "dob";

            public const string RadioButton_Gender_Male = "//input[@name='gender' and @value='Male']";
            public const string RadioButton_Gender_Female = "//input[@name='gender' and @value='Female']";

            public const string DropDown_State = "state";
            public const string DropDown_Country = "country_code";

            public const string TextField_StreetOne = "street1";
            public const string TextField_StreetTwo = "street2";
            public const string TextField_City = "city";
            public const string TextField_ZipCode = "postal_code";
            public const string TextField_County = "county";
            public const string TextField_HomePhone = "home_phone";
            public const string TextField_MobilePhone = "mobile_phone";
        }
    }

 

    [TestFixture]
    public class infellowship_Login_PenTest_WebDriver : FixtureBaseWebDriver
    {
        #region Penetration Tests

        [Test, RepeatOnFailure]
        [Author("Suchitra")]
        [Description("F1-4160 Infellowship Login page Penetration test for password Autocomplete Off ")]
        public void Login_Password_Autocomplete_Off()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            test.GeneralMethods.OpenURLWebDriver(test.Infellowship.GetInFellowshipURL());
            TestLog.WriteLine("AutoComplete : {0}", test.Driver.FindElement(By.Id("password")).GetAttribute("autocomplete"));

            Assert.AreEqual("off", test.Driver.FindElement(By.Id("password")).GetAttribute("autocomplete"));

        }

        [Test, RepeatOnFailure]
        [Author("Suchitra")]
        [Description("F1-4160 Infellowship Login page Penetration test for forgot password  Request Verification")]
        public void Login_ForgotPassword_RequestVerification()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            test.GeneralMethods.OpenURLWebDriver(test.Infellowship.GetInFellowshipURL());

            test.Driver.FindElementByLinkText("forgot?").Click();
            test.GeneralMethods.WaitForElementDisplayed(By.Id("btnResetPassword"));
            test.Driver.FindElementByName("__RequestVerificationToken");


        }

        //    [Test, RepeatOnFailure]
        [Author("Suchitra")]
        [Description("F1-4160 Infellowship URL Redirection ")]
        public void Login_URL_Redirection()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            //Navigating to URL and verifying the elements in the page
            test.GeneralMethods.OpenURLWebDriver(test.Infellowship.GetInFellowshipURL());
            test.Driver.FindElement(By.Id("username"));
            test.Driver.FindElement(By.Id("password"));
            
            //Adding additional text to the end of URL and verifying the URL is redirected
            TestLog.WriteLine(string.Format("URL to click :{0}/badlink",test.Infellowship.GetInFellowshipURL()));
            test.GeneralMethods.OpenURLWebDriver(string.Format("{0}/badlink",test.Infellowship.GetInFellowshipURL()));
            
            test.Driver.FindElement(By.Id("username"));
            test.Driver.FindElement(By.Id("password"));
        }
        #endregion Penetration Tests

    }

    [TestFixture]
    public class infellowship_Login_Accounts_WebDriver : FixtureBaseWebDriver
    {

        #region Login
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies validation if someone attempts to log in with an invalid log in.")]
        public void Accounts_Login_Invalid_Login_WebDriver()
        {
            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("random@fakeemail.com", "12345", "dc");

            // Verify the validation message
            Assert.AreEqual("Login attempt failed. Verify your information and try again. Your account will be locked out after 3 unsuccessful attempts.", test.Driver.FindElementById("login_failed").Text.Trim(), "Failed Login error message invalid");

        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Felix Gaytan")]
        [Description("F1-5727: Limit failed log in attempts for infellowship. Verifies Email Login Max Attempts account lock.")]
        public void Accounts_Login_Max_Email_Login_Attempts()
        {
            // Setup Data
            string guidRand = Guid.NewGuid().ToString().Substring(0, 5);
            string individualFirstName = "LockIndividual";
            string individualLastName = string.Format("Fellowship{0}", guidRand);
            string userEmail = string.Format("FT.Lock{0}@gmail.com", guidRand);

            // Clean up
            base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", individualFirstName, individualLastName), "Merge Dump");
            base.SQL.People_Individual_Create(15, individualFirstName, individualLastName);
            base.SQL.People_CommunicationValue_Add(15, string.Format("{0} {1}", individualFirstName, individualLastName), GeneralEnumerations.CommunicationTypes.Mobile, "505-555-4578", true);
            base.SQL.People_InFellowshipAccount_Create(15, string.Format("{0} {1}", individualFirstName, individualLastName), userEmail, "FT4life!");

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            TestLog.WriteLine("User Email: {0}", userEmail);

            //Max out login attempts
            test.Infellowship.LoginFailMaxAttemptsWebDriver(userEmail, "FTFourLife!", "dc");
           
            //Reset Password
            test.Infellowship.People_Accounts_Reset_Password(userEmail, "FT4life!");
            
            //Login and Logout to verify password is valid
            test.Infellowship.LoginWebDriver(userEmail, "FT4life!", "dc");
            test.Infellowship.LogoutWebDriver();


            // Clean up
            base.SQL.People_MergeIndividualBulk(15, string.Format("{0} {1}", individualFirstName, individualLastName), "Merge Dump");

        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Felix Gaytan")]
        [Description("F1-5727: Limit failed log in attempts for infellowship. Verifies Email Login Max Attempts resets account lock.")]
        public void Accounts_Login_Min_Max_Email_Login_Attempts()
        {
            // Setup Data
            string guidRand = Guid.NewGuid().ToString().Substring(0, 5);
            string individualFirstName = "LockIndividual";
            string individualLastName = string.Format("Fellowship{0}", guidRand);
            string userEmail = string.Format("FT.Lock{0}@gmail.com", guidRand);

            // Clean up
            base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", individualFirstName, individualLastName), "Merge Dump");
            base.SQL.People_Individual_Create(15, individualFirstName, individualLastName);
            base.SQL.People_CommunicationValue_Add(15, string.Format("{0} {1}", individualFirstName, individualLastName), GeneralEnumerations.CommunicationTypes.Mobile, "505-555-4578", true);
            base.SQL.People_InFellowshipAccount_Create(15, string.Format("{0} {1}", individualFirstName, individualLastName), userEmail, "FT4life!");

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            TestLog.WriteLine("User Email: {0}", userEmail);

            //Min login attempts then successful
            for (int x = 0; x < 2; x++)
            {
                //Login twice
                test.Infellowship.LoginWebDriver(userEmail, "John3:16", "dc");

                // Verify login attempt failed
                Assert.AreEqual("Login attempt failed. Verify your information and try again. Your account will be locked out after 3 unsuccessful attempts.", test.Driver.FindElementById("login_failed").Text.Trim(), "Failed Login error message invalid");

            }

            //Login successful and should reset attempts
            test.Infellowship.LoginWebDriver(userEmail, "FT4life!", "dc");
            test.Infellowship.LogoutWebDriver();

            //Max out login attempts
            test.Infellowship.LoginFailMaxAttemptsWebDriver(userEmail, "FTFourLife!", "dc");

            //Reset Password
            test.Infellowship.People_Accounts_Reset_Password(userEmail, "FT4life!");

            //Login and Logout to verify password is valid
            test.Infellowship.LoginWebDriver(userEmail, "FT4life!", "dc");
            test.Infellowship.LogoutWebDriver();


            // Clean up
            base.SQL.People_MergeIndividualBulk(15, string.Format("{0} {1}", individualFirstName, individualLastName), "Merge Dump");

        }

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("F1-5727: Limit failed log in attempts for infellowship. Verifies Email Login Max Attempts account lock. User hits cancel and tries to login again.")]
        public void Accounts_Login_Max_Email_Login_Attempts_Cancel()
        {
            // Setup Data
            string guidRand = Guid.NewGuid().ToString().Substring(0, 5);
            string individualFirstName = "LockIndividual";
            string individualLastName = string.Format("Fellowship{0}", guidRand);
            string userEmail = string.Format("FT.Lock{0}@gmail.com", guidRand);

            // Clean up
            base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", individualFirstName, individualLastName), "Merge Dump");
            base.SQL.People_Individual_Create(15, individualFirstName, individualLastName);
            base.SQL.People_CommunicationValue_Add(15, string.Format("{0} {1}", individualFirstName, individualLastName), GeneralEnumerations.CommunicationTypes.Mobile, "505-555-4578", true);
            base.SQL.People_InFellowshipAccount_Create(15, string.Format("{0} {1}", individualFirstName, individualLastName), userEmail, "FT4life!");

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            TestLog.WriteLine("User Email: {0}", userEmail);

            //Max out login attempts
            test.Infellowship.LoginFailMaxAttemptsWebDriver(userEmail, "FTFourLife!", "dc");

            //Cancel Reset
            test.Driver.FindElementByLinkText("Cancel").Click();

            //Verify you are taken to login page and then try to Login with valid password
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("username")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("password")));
            test.Infellowship.LoginWebDriver(userEmail, "FT4life!", "dc");

            //Reset Password
            test.Infellowship.People_Accounts_Reset_Password(userEmail, "FT4life!");

            //Login and Logout to verify password is valid
            test.Infellowship.LoginWebDriver(userEmail, "FT4life!", "dc");
            test.Infellowship.LogoutWebDriver();

            // Clean up
            base.SQL.People_MergeIndividualBulk(15, string.Format("{0} {1}", individualFirstName, individualLastName), "Merge Dump");

        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Felix Gaytan")]
        [Description("F1-5727: Limit failed log in attempts for infellowship. Verifies Phone Login Max Attempts account lock.")]
        public void Accounts_Login_Max_Phone_Login_Attempts()
        {

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            // Setup Data
            string guidRand = Guid.NewGuid().ToString().Substring(0, 5);
            string individualFirstName = "LockIndividual";
            string individualLastName = string.Format("Fellowship{0}", guidRand);
            string userEmail = string.Format("FT.Lock{0}@gmail.com", guidRand);
            string userMobile =  test.GeneralMethods.GetRandPhoneNumber();

            // Clean up
            base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", individualFirstName, individualLastName), "Merge Dump");
            base.SQL.People_Individual_Create(15, individualFirstName, individualLastName);
            base.SQL.People_CommunicationValue_Add(15, string.Format("{0} {1}", individualFirstName, individualLastName), GeneralEnumerations.CommunicationTypes.Mobile, userMobile, true);
            base.SQL.People_InFellowshipAccount_Create(15, string.Format("{0} {1}", individualFirstName, individualLastName), userEmail, "FT4life!");

            try
            {

                //Max out login attempts
                test.Infellowship.LoginFailMaxAttemptsWebDriver(userMobile, "FTFourLife!", "dc");

                //Reset Password
                test.Infellowship.People_Accounts_Reset_Password(userEmail, "FT4life!");

                //Login and Logout to verify password is valid
                test.Infellowship.LoginWebDriver(userMobile, "FT4life!", "dc");
                test.Infellowship.LogoutWebDriver();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                // Clean up
                base.SQL.People_MergeIndividualBulk(15, string.Format("{0} {1}", individualFirstName, individualLastName), "Merge Dump");
            }

        }

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("F1-5727: Limit failed log in attempts for infellowship. Verifies Phone Login Max Attempts resets account lock.")]
        public void Accounts_Login_Min_Max_Phone_Login_Attempts()
        {

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            // Setup Data
            string guidRand = Guid.NewGuid().ToString().Substring(0, 5);
            string individualFirstName = "LockIndividual";
            string individualLastName = string.Format("Fellowship{0}", guidRand);
            string userEmail = string.Format("FT.Lock{0}@gmail.com", guidRand);
            string userMobile = test.GeneralMethods.GetRandPhoneNumber();

            // Clean up
            base.SQL.People_MergeIndividual(15, string.Format("{0} {1}", individualFirstName, individualLastName), "Merge Dump");
            base.SQL.People_Individual_Create(15, individualFirstName, individualLastName);
            base.SQL.People_CommunicationValue_Add(15, string.Format("{0} {1}", individualFirstName, individualLastName), GeneralEnumerations.CommunicationTypes.Mobile, userMobile, true);
            base.SQL.People_InFellowshipAccount_Create(15, string.Format("{0} {1}", individualFirstName, individualLastName), userEmail, "FT4life!");

            try
            {
                //Min login attempts then successful
                for (int x = 0; x < 2; x++)
                {
                    //Login twice
                    test.Infellowship.LoginWebDriver(userMobile, "John3:16", "dc");

                    // Verify login attempt failed
                    Assert.AreEqual("Login attempt failed. Verify your information and try again. Your account will be locked out after 3 unsuccessful attempts.", test.Driver.FindElementById("login_failed").Text.Trim(), "Failed Login error message invalid");
                    
                }

                //Login successful and should reset attempts
                test.Infellowship.LoginWebDriver(userMobile, "FT4life!", "dc");
                test.Infellowship.LogoutWebDriver();

                //Max out login attempts
                test.Infellowship.LoginFailMaxAttemptsWebDriver(userMobile, "FTCuatroLife!", "dc");

                //Reset Password
                test.Infellowship.People_Accounts_Reset_Password(userEmail, "FT4life!");

                //Login and Logout to verify password is valid
                test.Infellowship.LoginWebDriver(userMobile, "FT4life!", "dc");
                test.Infellowship.LogoutWebDriver();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                // Clean up
                base.SQL.People_MergeIndividualBulk(15, string.Format("{0} {1}", individualFirstName, individualLastName), "Merge Dump");
            }

        }

        #region Mobile Number
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies an individual can login using their Mobile number.")]
        public void Accounts_Login_MobileNumber_NoDashes()
        {
            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("9728675309", "FT4life!", "dc");

            // Verify login attempt succeeded and user is taken to Home screen
            Assert.AreEqual("Welcome FT", test.Driver.FindElementByXPath("//div[@id='main-content']/h2").Text);
            Assert.AreEqual("Update Profile", test.Driver.FindElementByXPath("//a[@href='/People/profile']").Text);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies an individual can login using their Mobile number.")]
        public void Accounts_Login_MobileNumber_WithDashes()
        {
            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("972-867-5309", "FT4life!", "dc");

            // Verify login attempt succeeded and user is taken to Home screen
            Assert.AreEqual("Welcome FT", test.Driver.FindElementByXPath("//div[@id='main-content']/h2").Text);
            Assert.AreEqual("Update Profile", test.Driver.FindElementByXPath("//a[@href='/People/profile']").Text);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies an individual cannot login with spaces in their Mobile number.")]
        public void Accounts_Login_MobileNumber_WithSpaces()
        {
            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("972 867 5309", "FT4life!", "dc");

            // Verify login attempt failed
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id("login_failed"));
            test.GeneralMethods.VerifyTextPresentWebDriver("Login attempt failed.");
            test.GeneralMethods.VerifyTextPresentWebDriver("Verify your information and try again.");

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies an individual cannot login if another individual shares the same Mobile number")]
        public void Accounts_Login_MobileNumber_MultipleIndividuals_SameNumber()
        {
            // Setup Data
            string individualFirstName = "MobileIndividual";
            string individual1LastName = "One";
            string individual2LastName = "Two";
            base.SQL.People_Individual_Create(15, individualFirstName, individual1LastName);
            base.SQL.People_Individual_Create(15, individualFirstName, individual2LastName);
            base.SQL.People_CommunicationValue_Add(15, string.Format("{0} {1}", individualFirstName, individual1LastName), GeneralEnumerations.CommunicationTypes.Mobile, "817-555-4578", true);
            base.SQL.People_CommunicationValue_Add(15, string.Format("{0} {1}", individualFirstName, individual2LastName), GeneralEnumerations.CommunicationTypes.Mobile, "817-555-4578", true);
            base.SQL.People_InFellowshipAccount_Create(15, string.Format("{0} {1}", individualFirstName, individual1LastName), "FT.MobileIndividualOne@gmail.com", "FT4life!");
            base.SQL.People_InFellowshipAccount_Create(15, string.Format("{0} {1}", individualFirstName, individual2LastName), "FT.MobileIndividualTwo@gmail.com", "FT4life!");

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("817-555-4578", "FT4life!", "dc");

            // Verify login attempt failed
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id("login_failed"));
            test.GeneralMethods.VerifyTextPresentWebDriver("Login attempt failed.");
            test.GeneralMethods.VerifyTextPresentWebDriver("Verify your information and try again.");

            // Close Infellowship
            test.Driver.Close();

            // Clean up
            base.SQL.People_MergeIndividualBulk(15, string.Format("{0} {1}", individualFirstName, individual1LastName), "Merge Dump");
            base.SQL.People_MergeIndividualBulk(15, string.Format("{0} {1}", individualFirstName, individual2LastName), "Merge Dump");

        }
        #endregion Mobile Number

        #endregion Login

        #region Accounts

        #region Create

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Stuart Platt")]
        [Category(TestCategories.Services.SmokeTest)]
        [Description("Verifies the create account page loads for InFellowship.")]
        public void Accounts_Create_Account_WebDriver()
        {
            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            // Navigate to create account
            test.Driver.FindElementByLinkText("Register").Click();
            test.GeneralMethods.WaitForElement(By.Id("firstname"));

            // Verify elements are on the page
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("firstname")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("lastname")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("emailaddress")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("password")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("confirmpassword")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("submit")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Cancel")));

            Assert.AreEqual("Your Information", test.Driver.FindElementByXPath("//div[@id='main-content']/form/div[1]/h3").Text, "");
            Assert.AreEqual("First Name (required)", test.Driver.FindElementByXPath("//label[@for='firstname']").Text, "");
            Assert.AreEqual("Last Name(required)", test.Driver.FindElementByXPath("//label[@for='lastname']").Text, "");
            Assert.AreEqual("Account Information", test.Driver.FindElementByXPath("//div[@id='main-content']/form/div[2]/h3").Text, "");
            Assert.AreEqual("Login Email(required)", test.Driver.FindElementByXPath("//label[@for='emailaddress']").Text, "");
            Assert.AreEqual("Password(required)", test.Driver.FindElementByXPath("//label[@for='password']").Text, "");
            Assert.AreEqual("Confirm password", test.Driver.FindElementByXPath("//label[@for='confirmpassword']").Text, "");
            
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Stuart Platt")]
        [Description("Verifies the create account page loads for InFellowship through the Sign Up link")]
        public void Accounts_Create_Account_SignUp_WebDriver()
        {
            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            // Navigate to create account
            test.Driver.FindElementByLinkText("Sign Up").Click();
            test.GeneralMethods.WaitForElement(By.Id("firstname"));

            // Verify elements are on the page
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("firstname")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("lastname")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("emailaddress")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("password")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("confirmpassword")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("submit")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Cancel")));

            Assert.AreEqual("Your Information", test.Driver.FindElementByXPath("//div[@id='main-content']/form/div[1]/h3").Text, "Text not displayed");
            Assert.AreEqual("First Name (required)", test.Driver.FindElementByXPath("//label[@for='firstname']").Text, "Text not displayed");
            Assert.AreEqual("Last Name(required)", test.Driver.FindElementByXPath("//label[@for='lastname']").Text, "Text not displayed");
            Assert.AreEqual("Account Information", test.Driver.FindElementByXPath("//div[@id='main-content']/form/div[2]/h3").Text, "Text not displayed");
            Assert.AreEqual("Login Email(required)", test.Driver.FindElementByXPath("//label[@for='emailaddress']").Text, "Text not displayed");
            Assert.AreEqual("Password(required)", test.Driver.FindElementByXPath("//label[@for='password']").Text, "Text not displayed");
            Assert.AreEqual("Confirm password", test.Driver.FindElementByXPath("//label[@for='confirmpassword']").Text, "Text not displayed");
            
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies the required fields on the Create Account Page.")]
        public void Accounts_Create_Account_Required_Fields_WebDriver()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            test.Driver.FindElementByLinkText("Register").Click();
            test.GeneralMethods.WaitForElement(By.Id("firstname"));

            test.Driver.FindElementById("submit").Click();
            test.GeneralMethods.WaitForElement(By.XPath("//div[@class='error_msgs_for']/h2"));

            Assert.AreEqual("First name is required.", test.Driver.FindElementByXPath("//div[@class='error_msgs_for']/ul/li[1]").Text);
            Assert.AreEqual("Last name is required.", test.Driver.FindElementByXPath("//div[@class='error_msgs_for']/ul/li[2]").Text);
            Assert.AreEqual("Login email was not valid.", test.Driver.FindElementByXPath("//div[@class='error_msgs_for']/ul/li[3]").Text);
            Assert.AreEqual("Password is required", test.Driver.FindElementByXPath("//div[@class='error_msgs_for']/ul/li[4]").Text);
            
        }

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Verifies that when an account is created that activation code expires within 24 hours.")]
        public void Accounts_CreateAccount_VerifyExpiredActivationCode_WebDriver()
        {
            string email = "VerifyExpiredActivation@gmail.com";
            base.SQL.People_InFellowshipAccount_Delete(15, email);
            
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            // Open InFellowship
            test.Infellowship.OpenWebDriver("DC");

            // Create a new account            
            test.Infellowship.People_Accounts_Create_Incomplete_WebDriver("VerifyExpired", "Activation", email, "Pa$$w0rd");

            //Verify When activation code expires
            DataTable dt = base.SQL.Execute(string.Format("SELECT * FROM ChmPeople.dbo.UserLoginActivation WITH (NOLOCK) WHERE ChurchID = {0} AND Email = '{1}' ORDER BY CreatedDate DESC", 15, email));
            DateTime createDate = Convert.ToDateTime(dt.Rows[0]["CreatedDate"].ToString());
            Assert.Between(Convert.ToDateTime(dt.Rows[0]["DateExpires"]), createDate.AddDays(1).AddSeconds(-1), createDate.AddDays(1).AddSeconds(1), "Activation Date Expire Date is not within a day");

            // Close the session
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Attempts to create an account with an expired activation code.")]
        public void Accounts_CreateAccount_ExpiredActivationCode_WebDriver()
        {
            string email = "expiredActivation@gmail.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            // Create a new account
            test.Infellowship.People_Accounts_Create_Incomplete_WebDriver("Expired", "Activation", email, "Pa$$w0rd");

            // Expire the activation code
            string activationCode = base.SQL.Groups_FetchUserActivationCode(254, email);
            base.SQL.Execute(string.Format("UPDATE ChmPeople.dbo.UserLoginActivation SET DateExpires = DATEADD(MINUTE, -5, CURRENT_TIMESTAMP) WHERE ChurchID = 254 AND ActivationCode = '{0}'", activationCode));

            // Attempt to complete the account creation
            test.GeneralMethods.OpenURLWebDriver(string.Format("{0}/UserLogin/Activate/{1}", test.Infellowship.URL, activationCode));

            // Verify the user is informed that the activation code has expired
            Assert.AreEqual("This activation code is invalid or your account has already been activated.", test.Driver.FindElementByXPath("//div[@class='grid_11']").Text, "Warning not displayed");

            // Close the session
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Attempts to create an account with a used activation code.")]
        public void Accounts_CreateAccount_UsedActivationCode_WebDriver()
        {

            string email = "usedActivation254@gmail.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            // Create a new account            
            test.Infellowship.People_Accounts_Create_Incomplete_WebDriver("Used", "Activation", email, "Pa$$w0rd");

            // Expire the activation code
            string activationCode = base.SQL.Groups_FetchUserActivationCode(254, email);
            base.SQL.Execute(string.Format("UPDATE ChmPeople.dbo.UserLoginActivation SET DateUsed = CURRENT_TIMESTAMP, DateExpires = CURRENT_TIMESTAMP WHERE ChurchID = 254 AND ActivationCode = '{0}'", activationCode));

            // Attempt to complete the account creation
            test.GeneralMethods.OpenURLWebDriver(string.Format("{0}/UserLogin/Activate/{1}", test.Infellowship.URL, activationCode));

            // Verify the user is informed that the activation code has expired
            Assert.AreEqual("This activation code is invalid or your account has already been activated.", test.Driver.FindElementByXPath("//div[@class='grid_11']").Text, "Warning not displayed correctly");

            // Close the session
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies the user cannot create a new InFellowship account if the user is less than 13 years old.")]
        public void Accounts_CreateAccount_LessThan13YearsOld_WebDriver()
        {
            string email = "underageInFellowship@gmail.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();
            
            // Create a new account
            test.Infellowship.People_Accounts_Create_Incomplete_WebDriver("Underage", "InFellowship", email, "Pa$$w0rd");

            // Attempt to complete the account creation
            string activationCode = base.SQL.Groups_FetchUserActivationCode(254, email);
            TestLog.WriteLine("Activation URL: " + string.Format("{0}/UserLogin/Activate/{1}", test.Infellowship.URL, activationCode));

            test.GeneralMethods.OpenURLWebDriver(string.Format("{0}/UserLogin/Activate/{1}", test.Infellowship.URL, activationCode));
            test.Driver.FindElementById("dob").SendKeys(string.Format("{0:MM/dd/yyyy}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(-13).AddDays(1)));
            test.Driver.FindElementById("street1").SendKeys("9616 Armour Dr");
            test.Driver.FindElementById("city").SendKeys("Keller");
            test.Driver.FindElementById("state").SendKeys("Texas");
            test.Driver.FindElementById("postal_code").SendKeys("76244");
            test.Driver.FindElementById("submit").Click();

            // Verify user is prompted regarding the age restriction
            Assert.AreEqual("Minimum age requirement not met.", test.Driver.FindElementByXPath("//h1[@class='gutter_bottom_none']").Text);
            Assert.Contains(test.Driver.FindElementByXPath("//div[@class='under_age_warning_msg']/p").Text, "You must be at least 13 years of age to join InFellowship.");
            Assert.Contains(test.Driver.FindElementByXPath("//div[@class='under_age_warning_msg']/p").Text, "If you feel you've reached this page by accident, please contact your church administrator.");
            
            // Verify that you can't use the activation code again
            test.GeneralMethods.OpenURLWebDriver(string.Format("{0}/UserLogin/Activate/{1}", test.Infellowship.URL, activationCode));
            Assert.AreEqual("This activation code is invalid or your account has already been activated.", test.Driver.FindElementByXPath("//div[@class='grid_11']").Text, "Warning not displayed");


            // Close InFellowship
            test.Driver.Close();
        }

        // [Test, RepeatOnFailure, Timeout(15000)]
        [Author("Mady Kou")]
        [Description("Create Infellowship account with parameterized email")]
        public void Accounts_CreateAccount_PerformanceTest_InfellowshipCreate_WebDriver()
        {
            string churchCode = "dc";
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            try
            {
                for (int index = 627; index < 2801; index++)
                {
                    string email = "performanceteststudent" + index + "@gmail.com";

                    // Open InFellowship
                    test.Infellowship.OpenWebDriver(churchCode);
                    TestLog.WriteLine("Creating account: " + email);
                    // Create a new account
                    test.Infellowship.People_Accounts_Create_Incomplete_WebDriver("firstName" + index, "lastName" + index, email, "Active@123");
                    TestLog.WriteLine("Created account: " + email);
                }
            }
            finally
            {
                // Close InFellowship
                test.Driver.Close();
            }

        }

        // [Test, RepeatOnFailure, Timeout(15000)]
        [Author("Mady Kou")]
        [Description("Create Infellowship account with parameterized email")]
        public void Accounts_CreateAccount_PerformanceTest_InfellowshipActivate_WebDriver()
        {
            string churchCode = "dc";

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string activationCode = @"28781b848df043c9af7075ef58040b9e,5ed92cd424ac49978e2c95d06705cc81,81d874cf901447fdb04d096e48bb04bd,7713a18b7c3b4251966052b0164fc413,ded6b409e8a0482ea728a4e41062a1bf,94173ddf188b4721b1ad0eafd58c3bdf,9c4df399c36243818a0a0ce1bc39b0a0,33274d756b4844f387b863799d776057,c199213276254b12b9a361280815ce44,153713a469904ecd95ce694a16ad1d47,b30672fe7dee492684e39e01be2ad1fb,6d500cae0a0e4c159408dd9cfa559d9c,32be1a4271fa4a329da4dbd9c782e481,b4eeebffa2134234baa94a6086fa7f56,62d7163bd39b4eb1b98ba7c985c7c95e,f80853e6cd1c46f6a2faa94b04ce0410,0da6e3f931de4b9b96bf16d692fe3e46,0e4782f1d9b84bc295f5633693e15f22,6e2f7aeeaf294ba8b5b1e20cb36f3e4c,95c3b4f43d3e441badf46eb2b11e7b51,b93800d3f6da4275aafa9bf56b8e4f01,74efa2cfd6474d17b3ab903898a45b94,be9c4e1b4ade4e3498c2fb5db392aa3e,24cd82ed000a49f098a09ddbc9a68661,05d2c089e0c448eda2cdb02e94682bec,8804bf79e2da46dcbfcc3ddf9afcec3d,5fd84c23498a4cc4abfbf35ee569a473,b83f0fd93e8549879805dcab5e2e52b8,06bf60facb16490cbbc24c598be9c808,98f81c27465e418ab11753a0e1c25f4e,bede4ff0c97b4adc9ae4469333765d2e,231ce6c5bbd749889b17d32f89f27af3,3d901ada1f604ffd80ed3acf2b7a6176,3bad48169e9b43d99eef94fdf92464ad,ecb53212023647979ef7c5eee01a7159,9d01de66e7314be4b189e04fa4004be1,b81dd1a61ed647b299b3df4e8c1305a0,30e77cc2257e4033aca6c598ad2f71b2,cd4ff7849b3d4e0badad2a554c552c84,855756cf9dc447bd8b05ba11181bff5e,79d5164896f241abb6682654155fc614,1f3b453391fe4f3e989730fed7e9f265,f53e58d788b74609a943b8f74525d567,4c4f730676084430bca0c0eee38f1d59,815635234d77438090505a911b012ba7,b3cf8b462df4402f9cb1ab340933de96,abf4134295cb41968ab6e86aac1812e1,f4a19f5b3d6945388e128e3b95e88b2c,070ff4cb8b8047bea52d84417a4d3558,f7772c3eaa1b46b38d3a4399f76d098c,d5b6feae13c64b78b689accee35f08fa,440fbfc26e5d45ac8554e3a1ca4ebd44,bd286f64337343508e75da14248012b8,9011de43816246a194fdfe1d677548ff,69853e4751c1485a9d6cfe584c629b63,a9b8d6c26a1f4e98ab8aadf37267e3cb,ac515af7084e431cbaf94d953da3b6ea,deb352bc4529466890a45d094680e50b,05413eafd2c94b9d8a5a4bee1beb9b99,556428e78bb545a0a48e02843efded2d,b3260b829e784c51a869e13e2d1bf745,0abc9dda131e403395195dc86ce0e015,145716a59579404d9e54d8e938a080a1,bb35e55b17f3436a9e3343527bcdd982,0e1bebb3abfb4967b882ed2124d197bd,9ceb221bc4a94870a07a7fe43950bd12,8a99ab88dc824269a53d5165ddc7669e,591c80fd23a549e7a4204fd36a4f0aaa,dac63002124f496787714ffb31d37367,062a369252864bf884451013d602ba21,35a471fcc13e457ca21716b1c8552d28,bc78e1cb49e74b38a78747fef2319a8a,15e8f800286a410090b5cd816dc31497,50b8508c97294c06a2871ed799006ece,baa745b50f444d33915338c7a1e22923,761bccc3419d4e53947d36845a6d94e1,0df000bb4c4b44a0bb08cdc401a0faf3,984fc048b276431ab294980ff6de731a,63cac00f1552468e96df5f3750b072ce,ee73eb7472c244319e4d0f2eb5a0b398,5d684d25ad884bfaa83ff425a5b2ec3a,e7932ba748b44966affb2abe475c84a1,101d2246f6ef41eaae1f042cbe0c8ef8,a38fec5f90694b358f073a2bbd438cd0,33f4b2e7c34f4f3b87691591fde4db38,2e56f90f5a9f4a34bebd291b075fb52a,15bc39a8a80743a8a73bee87b3cd7d63,597eac16dd52464cbc55dccadb3f6e9a,7a4f19407cb64b79a244c5db9d207e59,13d48ed895a84ec0beddbf914b6a320d,0c767dab0f164ef39ad54c4bbf63c9ab,4ade3626c84843a49657a0471ab30888,ef520a9f14ea4a84928061559bc82ca2,bab99b9a6ec143c1afcda19b9305df59,1cbda7f627034cf1bf3cbb1e8da42ca6,89445c00762549d9bee0410b7f10b2b6,c915dd787118489c995dbfa018194d11,deb9d48e980c40c5aed423c542477074,70485a6f01aa421898d64483a414bfb3,7d66f4257b4d4d79ac7044e95b8927ba,6e2584ee49294049ba4908750e6e236d,4d6adab17921436791437fbc390003c1,6b0fb8e035424e409f7b0d61a06cdb33,d4a30457237d45eb8bc758f707bd11d6,b833b6b484434750aa359df203a47d12,0c93aa674a5f489680e794b7801d2036,3fd6e2adfa704d4494905e0da8a9ad92,5933b14e005e44428b005b9ebac3c251,0ea65f6cae83444bb2079a7f6bc2c91d,e1ff79ee802f4e7d8f3860b5061e9285,e5a276a0cffe45f5ac53b8683218c1cd,9b45f17d98404f6ca0d77d6c3b1c0379,68504d7689d844bfadcfedc1f15f1efb,afdaec9d5c6743a1a8c9086643365b15,3fc35d2916e14da8a2acddfb6292e034,d17b2e12a02b40239a8b2ee3e787c477,b0ef5caa8dc14d95b891857bc9471327,2d10993fff5e4e0caaf39b67d74a427d,b8cf2fb707694bfea86a88d036a1a9da,d4f39b941933423eaaddf6e88ae918bf,eb2cb67c253646d4aabd40c903e778a1,4acafde2e080410e88e4556aa9214d01,7cbef539ef8e45319d86cd434cc771c5,2302fb1373e44fcfaf1c7ffeae98b38c,ac55a849472c4b8ea499ca54c3d9c9db,74a44bbeef9141dea842be9ca80eabc3,2975e3ac20174f48914046255bd2b486,097cb571951742d2b921df5da4b16fb5,9a2b3df19047433f9a40f7d715057432,99bb2eb5b649446e8104df19a08067c0,ac898f32931b4d788c9f696baf10e141,3088a17e1ee24c938da8620974b607ef,2da0ffcce8104b969a23b9d5b9c1c864,27bf8664ecb04f29b5094f1b4deaa354,9b959839575f40f9aa6f0bc95bb0db0e,63201cae77a747618a542f47f57a9f22,6ad590524d684441b91cd36de7e4a4d4,102d654f30754cc2ae1eaa06a9024e53,d452297446d24ca1a94bb23c50c510d8,11fa5538b9e64f0a899f186d55d71251,811d6dbc74c04b7ba61bc8cc0ee6e891,95802a7b67274be6baab2905e5214703,899628bb5cd84f729bb29181f247dd68,b9e7fe62d0734ee6bf01c330fb17e128,4e43a274314c40c49dcd4be3d81e1c0b,43a80686e14940f4bb2b2bb221ba7301,2c47a9cd07f5465f9319ae431566f5f7,7f4fc19aeaf944dba8da94606afccc02,846bb569466148cf8966b947c2056f42,2e3594d30ec74284b5a7bf2201da51e9,5ae2e37046c34b5a960c45bed5ee8b42,93307ff0fa9943e9a4bb3afa6e4356cf,f6b5fdccd34d40ceaf0949fbafc05b0c,39828782ba5949dc87452b02c83a7278,cbd3bc26c3b240ddb0d9e4c6605646cb,c8b35614842247679d7a87ed6e998a59,6a4f573b508f447884f7b5e699141099,272cf282e76b46e0adcb44f0d0132970,2f801600b6a342688bd1b009ba1a86e9,da7af2bd809c47b6892dbd9fc6e4ba41,60039c09e4604e3595dd86725d3a4305,2f8aece5b3f249ac8e1e9066a2ac5534,1ba23a7d3233444cbe3c52433ffee607,9c589608c9c545db9662e7733a953827,cc81ad8343604462a715e744f5e5615e,1a81dfab9a8b414b8389b81ccc676f28,9d0cc30a54c349f0a8291b9748d19981,b60ede1d80a04bfca844fb0fa413c888,5a5ef90556e84a828126d73e91227828,310f7f09beb2472cafca9d1e56a45c16,85f5c915d1b24e5a9b1c0bd0ec20f377,50c6999b6c71402cb087866de8b91d32,1893f450b71c42f7b81939438b5676c4,dfedc50b7c7b48cc9528ba30f5ce1214,bd95c9c977c740ba93aa839787f31eab,7a5950d1b35841348a71595d068d5420,afdc459e2a49453e9e17a2c46d18f01b,defb5895ad2e47d79f8c9e23e516b601,3163ffa01cf349fba2026e8070c97f20,c4a8d773431541baa1e2b212c70e744e,e3795af11c0c47ed8f88b89fd4b22adf,137ba55eaf564eb28a683b493463bf4a,30e2499f59094a2aa6a16df6c0146f38,dd619b288c1c4ce3b88013985f5288c5,70af904921db4d16b678c2147708f913,3bb37397c7204917b461838320e0e5d7,5fda32d75408402aafce0cc9e25c7657,787db590a3244c6ca3c693a28f462846,7e107e117a0e47d2ab95ebf2b4188d32,56432732950c4c2282cbb3aa8736d01d,2c4d332169e149599df9a095fa1bd1b7,d6ebc9a41c544ac08817b7874b401d6f,fb5cee12bb9945bbb4ca989992ca5f8d,7ec5c1cbe1914bb29f4208642d3bbfd5,2d44512bf32d41c58a65319fc6323874,c33855ba58b7498ea0169d1be152e925,5070e1531abf406db1ce8e1165e1eb90,b30dc5f720ae48db80d386a1b12fec72,994b8fc88e624cc3a52e1c9c0bfe83e0,8fc2befc69f143b7baadf37af6f2d3ed,6260794113c843f686390b9e684e8716,752c108b633044fba3e24435a09363bc,b56755eb495942aab8b64e2bf0431778,73b0ae34337b4ac183cbb77bf1fc9383,b2830c06b47a461fb2c0e29ef5dc218f,3358a0f5ce02472cb24eb1678fa0ea41,1ddf137b3b0b484285127bc35eadc570,73d0b6523ec74f0e92d8550a072ee54d,a23179527e7e49819fd31c13382924f7,5767ce76bcc548919755470f8491b1d3,764e7fae88c94eb5917ddfd7e8baface,9707397daf2d434dbb91eca543e73fc8,442833da67ed4ec290c9bd8d50753442,037feb21288842f8b50151c475259a90,8082da80213e430c8fa19994f23da8b1,847d578837724b0a9796148431dc490e,a1292ec8f2014564a6e5811e72ebe69e,9c5419fbf19c425cbe93716e02b0ff92,901eaced227242efa9c94f961391f97b,39f682d19f8d4e9f81458128efddb556,a92ee778830c49fc9129b6026792f74d,7d2eb12d3afa4489a9bfd5f3cab62980,bf2a24815a66417ca7961d47320fe6e5,
42b8f4c0581048a5a0726efbb3306d38,6025a4ec458d464b80db45105e0a124b,9b2b121285a7425b8b0c0779c90df2af,fbe81d8de1204bdfa018a85d5703cf0c,130770b177404c1aa74ea87ee7ff45cc,740a5fd9b08f49b8b46d9d6db37ce2a6,dbe3f154083f4e668c3fda492d68aea1,c5e48e31b04642aaa8a05c8f4a3d8d1e,1df89e5119564118bae5a6ce5e49ce9d,7303a4ec9a354b3a9697f145e32338c0,2dc856674bcb41daa5ee532dcc5780ea,b43a4d8025b64a499a0f7b0422ce7ea9,b46ef0543ec14791a17b118ad61d3a96,3e190aecd59f4ab3a3a0330a90088959,59a79d7c0d604fba8667fb052e648c7f,9f44cbb022af4310a8a967e74c906fc8,e47e8b2300db48fa88aa35c3bee12eb9,95f147694d4c445b948d612235a01471,447557b028a54734b8df4fcc527332ec,ab0102f9742443aebf610684819ba28b,09c9add8ce3c456a9c2a71c8c972c95d,edd3fe18cd6148bda57cc34bba009f98,2c3733b96ad645b6a297917f5f23d1ed,70c2a7953c2b4b84a35a1afd38367ff1,acfd8eeeb7b04927ad7098f1bc52ef28,86d79b04b401408a901a6b113fc520df,c4d7767e271a4a26b5992206dd3958bc,7be99ec2667d46a69d50b36a97b51dc2,b08d20ea34b54f95a01caa67924a5c22,1f17f65a95e9425db01f528d5e199e58,b08f702c916f424d94555dd1b9fd9263,b79f0c9906724541a88f210bb32fceaa,bcf0c854c11f40249a8724cd4fca4d69,2bc12df7321c4deaa43a5f402dbc2a93,f398026e2b3f4a1087ce1e3f517ed327,672627d225a94809a45ea087dbc07362,c0d8c49094f24d92a918ff1b9298da36,6c05c11d4fbf45b0bbae7aaf607312bd,9ba0ad7ead0f44468ce3f647604b5686,89a38c4adb684d50a9ecdc4ff9b91efd,c10c4ae59cb74d699cf0dcf0b72718cd,6d0a6e4acbc14617b4fad4ef61aec57c,3d722081b6c14f6ca71068e3e3dfa390,da95a01ee7994dcb9e9c2ee85bff8b64,497479146c7e413fa33759eadca611d1,7391b7ca02624273b24c83da4b5fdf98,638f7281472f43e0bde8449e83c0a677,21df37ff1e004cba9f515f2d95728557,2e447870d6504872ae56b5c471d0872b,60a6f0f0233c40d8994de8fe9e791a4f,e2eab4179c3c4655b5a0b7d4d35870fc,0987aa939f494a9eb7a51831a71c057d,18860c07dc5c461893f8cc6fb73b86d5,add318afbb1f4cbb97d135984393d16a,5b12e575003e46b280cd57b79c239d80,16f889b7446041f9b2bb473d7c3a0daf,da63afded5c24fbd9049e57fcb1b0183,261f506599304a0483ffbd9e78a304d1,9b7d4ccafa6d456c9a8a5b6339c276eb,e6125d9df3b247f5be030b2cea008aea,e10f4fc78508487e943dc5c60f7fbd43,17a201642b9a4b0d97b66008b396a1dd,11eb0cb0434e413aa0d8e89499ba22f6,d1184679bf464b51ab293c449d972a33,6b997539fae34ce2a7842068ee82ce46,51390d72f2094264860722bd0cc5b08c,65beb496c06a4e14bddd11fba82e999e,b327baa21bd44a0a93bc708db8daaee2,f8596448d1fb49c09a2b9082517e330e,ae70f1acaa384415888f496eb766a3f8,330715e256c94008a777445d74d8d25a,0374912f6d2b447f9aa0d41ab162e5fb,759c409558aa43a89c1dfb587eb34fad,cce06095dbbb45439840c53fb8ebaf3a,f7efd2fa9bf1466d8ae724336ff43a2e,4bb94dc7b11949e08fe83f88fe14061f,5d365ca46ae94ce28f4c8660ab89760e,d22075b9de1b4c4ab9113e77ccf8caa4,25171108af1a4e0db1ada25823687c39,76dd85d74634452b8a0a08c256519d91,a3e5b219e11541219cc9979d28b2913d,c7a37c7d19e34537bf071ebf9bc17cb1,8dd286e920d848b4adf768f5bbd9e846,72229c82943547ffbc6ea99e813407a2,84e9f8cf94b448dfba6fedf129ade1a7,ff3131e676a043cd8016f27800a6ee45,c0a870e4fbf94473abb50935c349e47e,1873e05f5f7f4379938784c48c17ca97,49c89734b9a64a5395d1a6ac2af3e496,c83cf537367c4f02978301afb673c58e,dff6a684d8794f839b8ae7de6044198c,9f3838a656d5438d8259361c7e5914ea,b6787ec30eb5489b8379e168c0c3c494,decafa1bb5154065acd5230229abd785,7feb5f7e59b7452fb79927e9a95fe27d,b13f3993baa34c8890a740dcd2fceb92,e00b0c5b9bac4b0bad9ab82fae470fa4,18dcdf8d1e1945eaabb97abb5c30c558,28ba3ce8209d4795a7b7a98836a1bd18,6f326e3e5fdf479ca398ccedb4e4d51b,537d44807bc04bfea63981b977e2db38,c6a5ea2b65b34a6b9d677fec90373ad1,1ff3f83f4fbe4fc494ee59bbe4427ade,c3cf70b097f6493ca051b8a59fcfe413,7a28ce3ce5a44525bbddf3a53d524851,e772203e85a241f8a5025df8721fc64b,f43b27454f1d4e3eb7bbab2057a16152,70f9a8d95bf64cbb90c1a7f80cb261d2,8ba1323b1b264e31aa826128c53145ae,d6e43a7499994219804e0b7ca0321fe6,8c44387f4e3849e0b18b6bcf24634d61,6e7db979a17941819e7a3cccee555690,9739ebf848124cedb63c6dbbfc6a9e23,d93cc9df88a44a688eb6613758efecf3,2bc58864e3df4d14a75af8f65555c3bc,1348fd5b86ed45919d918dde8e679890,f0d590b6b7644509894f8cb2c5c205c2,0afb680ebd7d41e295868d44618e673e,b58660f00d6546908aaf088809854085,49cdca6b66c24f2abdc678dc36ac09de,6962936a04c54478b928f935663f615f,507edb58e2224a59bc76419fcc5193fe,f5e5aa906eb04476ba2f2780d9235974,f9189c0be9da48c9afe90bbb5f1375c0,cbd44d69a7184c9faf34d578d66b5bdc,ea25c6993f694f0ebf04c4aab0a3220c,91c45d9382c940f09954d4fed95134b0,3f01f0e08ae54dafa247edea1a91d2a1,4805fb13631f42afa6d0141466ed5f7b,291ddd0e30cd4fc8a3000607daed7c22,085bccccfd054b69a46ef085d3778949,c8a75a7faeaf4b7aae9865af236bfd04,1df13fc8d91649bdadc311ff80458b27,712751ff015840869faac9426ced30a9,6f3f962287e746c59a935dcb3acf3cef,505a6eff69eb4135abcee11360b009e2,957fe851cf7f4dc3afc40bcd099a4552,a34f710c241047629aad995441cd5d65,ff502760bc944df0918f261bf299273f,bc8fd8a57f5c47518e34ccb4e1be596b,dfecb894414b4ab4b36b9fef6d2c5b0d,dd3dae8d031c449880847a848c7da056,2c57b6b726ab4898b34d328943e4b5e2,5211bb4596c54daf838bc60570cacb5b,1a03e0556004485291961106c0eb92af,29e416b2ed6649d6b1aebc8b464aa683,af15de19f60a4f4fa186c9f48b5227ff,db637a7dcfc945a98bad975c4ead8253,0a1b5d1dbf3e436d9f1ccc9d087051aa,244374df3e9b45d68b62e34f375e6502,fea161b0e9dc4035bf19a30b3fa3f771,0c613b387a5d4ee3a458cf9747095429,80368e476ede49c0803e8d450a698eb3,e871eb430c1442e3a41108bb011a6794,4b50a78c5d6b495cb0784d45d23a24d0,db114239e57c460c90f0dcdca6f5be4e,2d9b48ea811c4d86821994fe445f1825,65c3d5fba8e64566bc4e886c199e2ae7,256b96715a1e4a6a95fa615d28c110fb,982dbc657c704c8295109f9b3cd47151,84fa68a5154142ee95dbffe2f0d79b78,bcb0b89cb94c4d39a9c671848bf4b56c,1b3ae8c162894ec4933abb3be9a13070,014e3ed4c2444b2c9905eda25054082a,935f746594e24bc2856f181dfffe0f96,d27a34f29d814c4ebc970eb01fdb9249,99a78490277942bf9dc69f7ad3a59ccc,90c94010f3f74583adcbf5f2acbb88e1,a2c2a6f221944070ba7144fc097c718f,71a308cc021544c6a54cdfc5a2538254,76713469f7a24ba9936d4f541c0b769c,7d5bd234871b47dbb041dcb0e9069c21,4662e497d1e544ab8be71f50503bd49e,5631a74100434be1bd8068970f8b2dbe,9d6243c5efa741ec96686befb31b6523,babcf0f2b3e946aaaaf61162c1d38081,6b8ddeb715c14bdda2eb915c9b2a77d7,dae8162b93f94f909031cb3eb7a0d47b,1b3777387060442ab8832fad34c98db4,0ac0dde9f8354915acd3f2375621a595,0103660cb88d403386b95a0d0aa660a1,bcdf877bd6e74d59ba2db5a49be1fff3,50b6a669784048aeaf34dab5ea0a2d05,c8e1d2a44d1e408490124c9b356ce8cf,fcb18c7effc64d78a1e3466daefaca49,55546a61e47d4d7ebb2393346228613e,0dc29858a82e499db433f9a16034ffdb,21057d3c8ab041f69534f7ed3451cf9e,ed39becd77f64492b5f787788eb9a6b8,5aad4b580f574f0b8b675ca95ddf3de0,7b6148886d83492fb82b560bee5b6bd4,13fa17d15f174fe0adc2ec0c55feb4fd,eddd989e94434a1783036ed15945f6fa,1b37be8f3c00451486381a4f65e5aab5,10d98fa1096a478183b90a2200bd1753,49f7a3bcf0f44004b3133dd93c4f20e0,9b534eff97744e2db6eb6f699fb12b81,5f7610120b4c40b79c5f038449722ae7,ae16a965174d4cdb94b546828a6efb8a,a825232300b64a7b943839be8ebaeb38,a7d472133220464f8315fbfa81ec8427,560194b72b0b4d66ae0f7b10b9487aef,663696d94f04437ca0c0a51d2477fd21,bc65359d60144a7dac17339cf3e2d658,1bc3eb54bae346a080e5578039e32524,3ee0981be8a34671ad0602e6f7828578,6c17624ef1274d938b7fdf308cf8f67f,ead878045cac4839b4f78e7532536e8d,3c6679129a4549f49deffb367cbff3af,1588f54c29d14792a980265b1f8b324b,061441e5202f43228a0395dd7bf7d23f,eb9b665f05474e5985fa1aedf7a9463b,befae02b21d74395a356d67ee2c2b1fa,9144dde8c643486fb4cfa610b3ca5bc1,cb7108852ae7416eb0f6b43842dbb438,57bde77f22db4fc0929b444b4e53df31,e355a4f607ac4c6ab6796d7f9d5b43f8,d253eb8b9c824dd0b2295fed37ba0e69,27fc8a3ee1044069928bac51d3ee75f9,c79da1ca70984b25ba7fbcb16b434803,a869862b8c3b4cb69772c0cb0aa0f946,df054c0b6c8846519d7888eaf117d30a,8330b38005574dec814f8a78ac1656c3,96ce19882f414cd5b85e14d86ddf31b3,e1ab73fa6e5c447393c755815ba3acb7,34a713f9a8e84359b5af16b47f054752,20a7738d3ab14ae896568c81edb4d701,fe5c33020f6f4589b059112c17d61236,473aaf6e1afd4826af8f8f01a94023ed,190fc2b379724b7ea5b9671536d43db4,13a3c17ece614d438bfa899a029b8af8,1294229d481042a28bc35ffb6759e735,dd9e7c23e9e149a0800c4ae312f4ac6d,594f262d220c429680e63ad079ea1f11,85884c9503874626b87ce00603276fdb,53c047a11046411cb1a1755700ada826,af9d8051eeeb4f26a628b4b01fc8b5ca,e7a5a73767864241a356c69c9519d89f,a0e083b8c2094fb7a45659772d1526a3,b83125976c744d7ca3cfe90d2d701f03,0578973b3262497181c65f4783163442,479420ea9eaa459cb52e3f0eed33a694,172b8b84c3d84593b0df20eadd217978,d367164f1f6e4fd18c6ea918dcab384f,ce674f64f20c42e2b5fa96c8835bc04c,a7e68a169ba94e7d8d99c0558c95a94b,38a74cc5f7ea4836ac9d0c970aeaad90,594ad8db78f0461b836febcc5e661aa2,97138ab07d8a49ebaee471fdcf41c3c1,960d377c73094221be0fa8bb0fb502d0,ea7c1594ddff407b967297f49887012d,11925105f4d34b169e01b03d1aa00337,7c0cb5fa16404d7098b5246e2469b1a3,5ee694bae66842c8b0ffff70f50bf95c,17e4d2a9a926461285113975d4e6c12b,89afff9cdc774bba872141bf2f5de453,ebb6c79bdfb8461bb127f6cd1d5b02d7,421aea0550384c01971d1f5f93d18add,c29711ec96a241ae93cdaa4e7fda9c55,838ac3eedb174bcc91b829eca19904e0,2b7cce67bb27438fa346f3525edc2309,a3091f95f30d4e3b9d07387c3e21fed2,dcc3a857b6ee45a3991a0a8677a116ba,294bc30d36404f3fb7fbf4d31d980d9a,1b793107e9ae4759a8bbc8c5dc47cb8e,c290238f86544f48a19196d299c4d75b,436fa2dcc4cd4d30bd0a28b87a69c23e,d885ddc63e4e4e2fbf640417a7d8ae94,46458e7686bb4c2a9947a6ea7e4ad570,bfb8321796844f97937a4639a2528b29,cfb10a89173e44cfbc570ed061e13ba7,a3a5faccc09b4294bc83c0ceda1ac0f7,abe057b412894f188177873c5b014b09,c2e6a520d06c4535b256bdf9d33ecb58,d6f106ee2174458cbd241083fffdf5d1,1af60c86f2d2436595c309617bbb2b0b,5328b00a959f4d1395c93a3398c10cf7,2c20f01a15ee445bbc2118f96eff98ad,f5f9bdcf2b934866bbddbcf70e2de512,9a83083cd4da4b7090985e8cea32bc59,91c3c9e4dc644537b759f671c3abd970,15794d331a6f4bcd9d7221448a05ba4b,a480be32dac944878eaa388509569bf8,11c154c70abb416b991aff87f929daee,5a8698481b314b3283b7b129353bd153,eed0b69e6d6f469cbba02d4767ef0a5d,29f71d1359234cd9abc318ed15e8938d,b15711fb071e4f419d7f4ab50063bb3e,62220ce450b945a4a33d17707c982fc6,3bbeaa305be6432a8d8e0b3af644a9a2,abc6923c200e454c973c3d543eb40346,ca88470d31c547fbbea2b277c4578637,6f90823bcff342a8873c06920f22f038,5b6638bf08d44ec794c51e978e8d8a70,925daea3773041299aa88f00ebc9574d,570d70924ecb45959127db33cd0f36d2,c096029e56694cebbc9662bfd3b60f03,bd912caefe7a482a8b63d87330d0b610,65ba5dcc34914d19b46aa5ed84ebbf68,bbfadac3d6ab4c03be4c258bd961e8ac,45a82fe2885045c4a61d6411f965014d,29b9181c2a694603b7ac4c28e36d7105,b0fdd03ea7a147d28bdf7b0ddb7579b1,148735f9b6b947dbb4d842bfba96d7b4,b3f6465fb7ff4e788bd1263db27e2c55,1c3aae728b6844f19cdd72cbe89a06af,2ad94072292b4060bd12fcd5827e4d95,cbab97e07a06442ebaddb44c75a3ab25,c3c08b69b5b742b78bd67f14ec2fa8aa,167be337f5794e0992cbcee8c1c6ea5b,119f2ff1ba314521a4e1dd160126c2e8,4fa20c9b8a604854851c40751af000a6,fea72e7e0a28441ba156f322ffc8d4ec,719b69830edb4d8ba10157e56d185e93,5c2a6b4c453347768089d2eae6fba08b,1978a4b8b87647c59f634c744b78d848,1c2287b696b24578972239a0fa266efd,fd49f321221d4f1b87197c2a016c7a42,06d489ce4c8442099f7f8740888c46d1,880149e84ebe455f89df5a783eb6c66f,b603115d8f6046c691c6619126568e3f,05396cde56a14659890085550fbd6bad,756a73300817422b93aaa6198f9e3e48,cd7c0958db4a41a0928f87109b9f5430,b0496ac12a7e451281d314ffeec265b2,a2e29288c236438294faa3e23dea9bc5,016b5c69797943aca89331a39a9e34c7,e8177afc8bb74e2db97c3f111626ff9a,c53d9f84e0054bf88ade6b0424054079,54be0fc37ed24d61816351605249995e,7fae8139506a47198bcb4ff73719bee6,b04b3c78b7a1415eaebc11383d34d8fc,5b4d063cd1554d9eaa278e29b04cb342,86b37277c4724418b88a4bd7851caf7f,209287be43014797a1f413c287e0eec9,a2e5b39bf3f24a3b9248669b75366fc8,3c1bd2a6a3c74e55917dab3bfc4780af,c190e1bdf60048ca891a949c4cbe8c30,245033792d404734a92455d04d9fe8c9,7e530e5f66f44c649cee96158194c652,f3103cc397b34e4291b6c5fef2e32f4e,6fb4379bdd16452b81cea99daa0884a9,ef08621961ad46b8b9dd52faeaecaa21,0b29f31f39b34507bfded413d0a64fac,8b2756af8df84894805d39f6c7f52b0d,ad0e42a881504020aa6512e4ba5f31e8,2e11aac7e55e48f8a5639ec91e35da56,9ebdbf6176b54aa6bb454b2629bd692a,31d7031c8791430ca8bf0b2e666e0a97,ec690afc318a46db85ba7bc1c41e535e,8159292cca464e6884b8fee11d2bada4,c3d850fcdacf45dc8f4e42db36f2d051,f893abd616374fd98478bb74cfc774f7,5f516a97f8d04e3d9a44d1154d7df531,980d4cb661024d4ab248616376ce29d7,48b521da1e994adeb06bb033a3512d77,a8eb9d54b48f4f3cb2fcca64d877154b,5a68726f95fd4e84b2273936bce5186d,d3ea5fe97fe44b9ea3fac6ecb394e8f9,7d750fa19fdf4437b70ca80b0fda6152,49639c68842a4284b560b860f7a67396,391ad3346c494f16ba6911f27f6bee34,8ce10bd9ab41409f99609d237f1c4887,b16a55532bd04194841003ee4b9cffec,6ee1ab823ba24c3d9f1b405bac8e99bf,d1860520a4ba41c7b4f0a3659e9797eb,8a80df0da8e24c8fa2f248c69095bcb8,4288c3f3d3a74346b31bb8a6556142d4";
            string[] code = activationCode.Split(',');
            try
            {
                for (int index = 0; index < code.Length; index++)
                {
                    test.Infellowship.OpenWebDriver(churchCode);
                    // Attempt to complete the account creation

                    TestLog.WriteLine("Activation URL: " + string.Format("{0}/UserLogin/Activate/{1}", test.Infellowship.URL, code[index]));

                    test.GeneralMethods.OpenURLWebDriver(string.Format("{0}/UserLogin/Activate/{1}", test.Infellowship.URL, code[index]));
                    test.Driver.FindElementById("dob").SendKeys(string.Format("{0:MM/dd/yyyy}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(-20).AddDays(1)));
                    test.Driver.FindElementById("street1").SendKeys("9616 Armour Dr");
                    test.Driver.FindElementById("city").SendKeys("Keller");
                    test.Driver.FindElementById("state").SendKeys("Texas");
                    test.Driver.FindElementById("postal_code").SendKeys("76244");
                    test.Driver.FindElementById("submit").Click();
                    TestLog.WriteLine("Activated the code: " + code[index]);
                }
            }
            finally
            {
                // Close InFellowship
                test.Driver.Close();
            }

        }

        // Case may fail on LV_QA by it shows the account is activated when first time to open the activation url.
        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies the user cannot create a new InFellowship account if the user is less than 13 years old - by matching to an established individual.")]
        public void Accounts_CreateAccount_LessThan13YearsOld_Matched_NoDOB_WebDriver()
        {
            int churchId = 254;
            string email = "underageUserNoDOB@gmail.com";
            base.SQL.People_InFellowshipAccount_Delete(churchId, email);

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            // Create a new account            
            test.Infellowship.People_Accounts_Create_Incomplete_WebDriver("Underage", "UserNoDOB", email, "Pa$$w0rd");

            try
            {
                System.Threading.Thread.Sleep(12000);
                // Attempt to complete the account creation by matching to an existing household
                string activationCode = base.SQL.Groups_FetchUserActivationCode(churchId, email);
                test.GeneralMethods.OpenURLWebDriver(string.Format("{0}/UserLogin/Activate/{1}", test.Infellowship.URL, activationCode));
                test.Driver.FindElementById("dob").SendKeys(string.Format("{0:MM/dd/yyyy}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(-13).AddDays(1)));
                test.Driver.FindElementById("submit").Click();

                // Verify user is prompted regarding the age restriction
                Assert.AreEqual("Minimum age requirement not met.", test.Driver.FindElementByXPath("//h1[@class='gutter_bottom_none']").Text);
                Assert.Contains(test.Driver.FindElementByXPath("//div[@class='under_age_warning_msg']/p").Text, "You must be at least 13 years of age to join InFellowship.");
                Assert.Contains(test.Driver.FindElementByXPath("//div[@class='under_age_warning_msg']/p").Text, "If you feel you've reached this page by accident, please contact your church administrator.");

                // Verify that you can't use the activation code again
                test.GeneralMethods.OpenURLWebDriver(string.Format("{0}/UserLogin/Activate/{1}", test.Infellowship.URL, activationCode));
                Assert.AreEqual("This activation code is invalid or your account has already been activated.", test.Driver.FindElementByXPath("//div[@class='grid_11']").Text, "Warning not displayed");

                // Close InFellowship
                test.Driver.Close();
            }
            finally
            {
                base.SQL.People_InFellowshipAccount_Delete(254, email);
            }
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies the user cannot create a new InFellowship account if the user is less than 13 years old - by matching to an established individual w/a DOB under 13.")]
        public void Accounts_CreateAccount_LessThan13YearsOld_Matched_DOBUnder13_WebDriver()
        {

            string email = "underageUser@gmail.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();
            
            // Create a new account            
            test.Infellowship.People_Accounts_Create_Incomplete_WebDriver("Underage", "User", email, "Pa$$w0rd");

            // Attempt to complete the account creation by matching to an existing individual
            string activationCode = base.SQL.Groups_FetchUserActivationCode(254, email);
            test.GeneralMethods.OpenURLWebDriver(string.Format("{0}/UserLogin/Activate/{1}", test.Infellowship.URL, activationCode));
            test.GeneralMethods.WaitForElement(By.ClassName("under_age_warning_msg"));

            // Verify user is prompted regarding the age restriction
            Assert.AreEqual("Minimum age requirement not met.", test.Driver.FindElementByXPath("//h1[@class='gutter_bottom_none']").Text);
            Assert.Contains(test.Driver.FindElementByXPath("//div[@class='under_age_warning_msg']/p").Text, "You must be at least 13 years of age to join InFellowship.");
            Assert.Contains(test.Driver.FindElementByXPath("//div[@class='under_age_warning_msg']/p").Text, "If you feel you've reached this page by accident, please contact your church administrator.");

            // Verify that you can't use the activation code again
            test.GeneralMethods.OpenURLWebDriver(string.Format("{0}/UserLogin/Activate/{1}", test.Infellowship.URL, activationCode));
            Assert.AreEqual("This activation code is invalid or your account has already been activated.", test.Driver.FindElementByXPath("//div[@class='grid_11']").Text, "Warning not displayed");


        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Stuart Platt")]
        [Description("Verifies the required fields when finishing up the creation of a new InFellowship Account")]
        public void Accounts_CreateAccount_AdditionalInformation_Required_Fields_WebDriver()
        {
            
            string email = "new.createdaccount@gmail.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            // Create an account
            test.Infellowship.People_Accounts_Create_Incomplete_WebDriver("New", "CreatedAccount", email, "BM.Admin09");
            test.GeneralMethods.OpenURLWebDriver(string.Format("{0}/UserLogin/Activate/{1}", test.Infellowship.URL, base.SQL.Groups_FetchUserActivationCode(254, email)));
            test.GeneralMethods.WaitForElement(By.Id("dob"));

            // Submit the form without the fields filled out. 
            test.Driver.FindElementById("submit").Click();

            // Verify Required Fields
            Assert.AreEqual("Date of birth is required.", test.Driver.FindElementByXPath("//div[@class='error_msgs_for']/ul/li[1]").Text);
            Assert.AreEqual("Address street 1 is required", test.Driver.FindElementByXPath("//div[@class='error_msgs_for']/ul/li[2]").Text);
            Assert.AreEqual("State is required.", test.Driver.FindElementByXPath("//div[@class='error_msgs_for']/ul/li[3]").Text);
            Assert.AreEqual("Postal Code is required.", test.Driver.FindElementByXPath("//div[@class='error_msgs_for']/ul/li[4]").Text);
            Assert.AreEqual("City is required.", test.Driver.FindElementByXPath("//div[@class='error_msgs_for']/ul/li[5]").Text);

            base.SQL.People_InFellowshipAccount_Delete(254, email);
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Stuart Platt")]
        [Description("Verifies the required fields when finishing up the creation of a new InFellowship Account for an international church.")]
        public void Accounts_CreateAccount_AdditionalInformation_Required_Fields_International_WebDriver()
        {
            
            string email = "international.createdaccount258@gmail.com";
            base.SQL.People_InFellowshipAccount_Delete(258, email);
            
            // Open infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("QAEUNLX0C6");

            // Create an account
            test.Infellowship.People_Accounts_Create_Incomplete_WebDriver("International", "CreatedAccount", email, "BM.Admin09");

            test.GeneralMethods.OpenURLWebDriver(string.Format("{0}/UserLogin/Activate/{1}", test.Infellowship.URL, base.SQL.Groups_FetchUserActivationCode(258, email)));
            test.GeneralMethods.WaitForElement(By.Id("dob"));

            // Submit the form without the fields filled out. 
            test.Driver.FindElementById("submit").Click();

            // Verify Required Fields
            Assert.AreEqual("Date of birth is required.", test.Driver.FindElementByXPath("//div[@class='error_msgs_for']/ul/li[1]").Text);
            Assert.AreEqual("Address street 1 is required", test.Driver.FindElementByXPath("//div[@class='error_msgs_for']/ul/li[2]").Text);
            Assert.AreEqual("Postal Code is required.", test.Driver.FindElementByXPath("//div[@class='error_msgs_for']/ul/li[3]").Text);
            Assert.AreEqual("City is required.", test.Driver.FindElementByXPath("//div[@class='error_msgs_for']/ul/li[4]").Text);

            // Verify province and state are not required
            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver("Province is required."));
            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver("State is required."));

            base.SQL.People_InFellowshipAccount_Delete(258, email);
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Stuart Platt")]
        [Description("Verifies you can create an account in InFellowship.")]
        public void Accounts_CreateAccount_Create_WebDriver()
        {
            
            string email = "stu.platt.t.est@gmail.com";
            string password = "Romans10:9";

            base.SQL.People_InFellowshipAccount_Delete(254, email);

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            //Create Account
            string activationCode = test.Infellowship.People_Accounts_Create_Webdriver("254", "Created", "Account", email, password, false, "10/28/1945", "Male", "United States", "717 N Harwood St.", null, "Dallas", "Texas", "75201", "", "877-318-5669", null, null);

            //Login to Created Account
            test.Infellowship.LoginWebDriver(email, password, base.SQL.FetchChurchCode(254));

            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));

            //Logout
            test.Infellowship.LogoutWebDriver();

            test.Infellowship.LoginWebDriver(email, password, "QAEUNLX0C2");

            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));

            test.Infellowship.LogoutWebDriver();

            //Verify that we can't use the code again via SavedAdditionaInformation URL
            //https://<curchCode>.<env>.infellowship.com/UserLogin/SaveAdditionalInformation/<activationCode>
            test.GeneralMethods.OpenURLWebDriver(string.Format("{0}/UserLogin/SaveAdditionalInformation/{1}", test.Infellowship.URL, activationCode));

            try
            {
                Assert.AreEqual("This activation code is invalid or your account has already been activated.", test.Driver.FindElementByXPath("//div[@class='grid_11']").Text, "Warning not displayed");
            }
            catch (OpenQA.Selenium.NoSuchElementException)
            {
                try
                {
                    Assert.AreEqual("The activation code is invalid.", test.Driver.FindElementByXPath("//div[@id='main-content']/div/p").Text, "Warning not displayed");
                }
                catch (OpenQA.Selenium.NoSuchElementException)
                {
                    Assert.AreEqual("The page you requested could not be found.", test.Driver.FindElementByXPath("//div[@class='box_notice big']/p").Text.Trim(), "Warning not displayed");
                }
            }


            base.SQL.People_InFellowshipAccount_Delete(254, email);
           
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Stuart Platt")]
        [Description("Verifies you can create an account in InFellowship in an international church.")]
        public void Accounts_CreateAccount_International_Create_WebDriver()
        {
            
            string email = "stuplat.t.t.est@gmail.com";
            string password = "Romans10:9";

            base.SQL.People_InFellowshipAccount_Delete(258, email);

            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            //Create Account
            test.Infellowship.People_Accounts_Create_Webdriver("258", "Brittish", "Account", email, password, false, "28/10/1945", "Male", "United Kingdom", "20 Tib Lane", null, "Manchester", "Lancashire", "M2 4JA", "", "877-318-5669", null, null);

            //Login to Created Account
            test.Infellowship.LoginWebDriver(email, password, base.SQL.FetchChurchCode(258));

            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));

            //Logout
            test.Infellowship.LogoutWebDriver();

            test.Infellowship.LoginWebDriver(email, password, "QAEUNLX0C6");

            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));

            test.Infellowship.LogoutWebDriver();

            base.SQL.People_InFellowshipAccount_Delete(258, email);
           
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies you can create an account in InFellowship and specify the home number as preferred.")]
        public void Accounts_CreateAccount_Create_Home_Number_Preferred_WebDriver()
        {
            // Store the email address
            string email = "home.preffered@email.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);

            // Phone Numbers
            var homePhone = "214-444-9999";
            var mobilePhone = "214-444-2222";

            // Open infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            // Create an account
            test.Infellowship.People_Accounts_Create_Webdriver("254", "HomeNumber", "Preffered", email, "Romans10:9", false, "1/31/1979", "Male", "United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", "", homePhone, mobilePhone, "Home");

            //Login to Created Account
            test.Infellowship.LoginWebDriver(email, "Romans10:9", base.SQL.FetchChurchCode(254));

            // Verify the home number is preferred.
            test.Infellowship.People_ProfileEditor_View_WebDriver();
            Assert.AreEqual("Preferred Number", test.Driver.FindElementByXPath("//div[@id='phone']/table/tbody/tr[3]/td[2]/img").GetAttribute("alt").ToString(), "The home phone is not selected as preferred");
            
            // Logout
            test.Infellowship.LogoutWebDriver();

            base.SQL.People_InFellowshipAccount_Delete(254, email);
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies you can create an account in InFellowship and specify the mobile number as preferred.")]
        public void Accounts_CreateAccount_Create_Mobile_Number_Preferred_WebDriver()
        {
            // Store the email address
            string email = "mobile.preffered@email.com";
            base.SQL.People_InFellowshipAccount_Delete(254, email);

            // Phone Numbers
            var homePhone = "214-444-9999";
            var mobilePhone = "214-444-2222";

            // Open infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver();

            // Create an account
            test.Infellowship.People_Accounts_Create_Webdriver("254", "MobileNumber", "Preffered", email, "Romans10:9", false, "1/31/1979", "Male", "United States", "2812 Meadow Wood Drive", null, "Flower Mound", "Texas", "75022", "", homePhone, mobilePhone, "Mobile");

            //Login to Created Account
            test.Infellowship.LoginWebDriver(email, "Romans10:9", base.SQL.FetchChurchCode(254));

            // Verify the home number is preferred.
            test.Infellowship.People_ProfileEditor_View_WebDriver();
            Assert.AreEqual("Preferred Number", test.Driver.FindElementByXPath("//div[@id='phone']/table/tbody/tr[1]/td[2]/img").GetAttribute("alt").ToString(), "The Mobile phone is not selected as preferred");
            
            // Logout
            test.Infellowship.LogoutWebDriver();

            base.SQL.People_InFellowshipAccount_Delete(254, email);
        }

        #endregion Create

        #region Update

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Stuart Platt")]
        [Description("Updates Infellowship account in Webdriver")]
        public void Accounts_Update_AccountInformation_WebDriver()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string loginEmail = "stuplatttest@gmail.com";
            string password = "Romans10:9";
            string updatedEmail = "stu.platt.test@gmail.com";
            string updatedPassword = "John3:16";
            string churchCode = "QAEUNLX0C2";

            base.SQL.People_InFellowshipAccount_Delete(254, loginEmail);
            base.SQL.People_InFellowshipAccount_Delete(254, updatedEmail);

            //Create Linked account 
            test.Infellowship.People_Accounts_Create_Webdriver("254", "Joe", "Cool", loginEmail, password, false, "10/28/1970", "Male", "United States", "478 Harding Ln", null, "Lavon", "Texas", "75166", "Collin", null, "979-220-9561", "Mobile");


            //Login to account
            //Login to Created Account
            test.Infellowship.LoginWebDriver(loginEmail, "Romans10:9", base.SQL.FetchChurchCode(254));

            //Update Account 
            test.Infellowship.People_Accounts_Update_Account_Information_WebDriver(updatedEmail, updatedEmail, updatedPassword, updatedPassword, password);
            test.GeneralMethods.WaitForElement(By.LinkText("Update Profile"));

            //Logout
            test.Infellowship.LogoutWebDriver();

            //Login with new creds
            test.Infellowship.LoginWebDriver(updatedEmail, updatedPassword, churchCode);
            test.GeneralMethods.WaitForElement(By.LinkText("Update Profile"));

            //Logout
            test.Infellowship.LogoutWebDriver();

            //Cleanup 
            base.SQL.People_InFellowshipAccount_Delete(254, updatedEmail);
            base.SQL.People_InFellowshipAccount_Delete(254, loginEmail);

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that Email addresses must match when updating account")]
        public void Accounts_Update_AccountInformation_EmailsDontMatch()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string loginEmail = "stuplatt.test@gmail.com";
            string password = "Romans10:9";
            string updatedEmail = "st.uplatttest@gmail.com";
            string updatedEmail2 = "stuplatttest7@gmail.com";

            //Login account 
            test.Infellowship.LoginWebDriver(loginEmail, password, "QAEUNLX0C2");

            //Update Account 
            test.Infellowship.People_Accounts_Update_Account_Information_WebDriver(updatedEmail, updatedEmail2, null, null, password);
            Assert.AreEqual("The new email address doesn't match the confirmation email address.", test.Driver.FindElementByXPath("//div[@class='error_msgs_for']/ul/li").Text, "The Error message is not present or is incorrect.");
            
            //Logout
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies that the passwords must match when updating account")]
        public void Accounts_Update_AccountInformation_PasswordsDontMatch()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string loginEmail = "st.uplattte.st@gmail.com";
            string password = "Romans10:9";
            string updatedPassword = "P@$$w0rd";
            string updatedPassword2 = "L3tM3!n";

            //Login account 
            test.Infellowship.LoginWebDriver(loginEmail, password, "QAEUNLX0C2");

            //Update Account 
            test.Infellowship.People_Accounts_Update_Account_Information_WebDriver(null, null, updatedPassword, updatedPassword2, password);
            Assert.AreEqual("Confirm password value is different from password.", test.Driver.FindElementByXPath("//div[@class='error_msgs_for']/ul/li").Text, "The Error message is not present or is incorrect.");
            
            //Logout
            test.Infellowship.LogoutWebDriver();
        }

        #endregion Update

        #endregion Accounts
    }
}