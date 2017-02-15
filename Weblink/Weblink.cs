using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using System.Collections.ObjectModel;
using System.Data;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace FTTests.Weblink {
    [TestFixture]
    [Category(TestCategories.Services.PaymentProcessor)]
    public class Weblink : FixtureBaseWebDriver {
        #region Private Members
        string _fund = "Auto Test Fund";
        string _pledgeDrive = "Auto Test Pledge Drive";
        private DataTable _funds;
        #endregion Private Members

        [FixtureSetUp]
        public void FixtureSetUp() {
            _funds = SQL.Giving_Funds_FetchNames(256, true, true, null);
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verify Privacy Policy")]
        public void Weblink_PrivacyPolicy()
        {
            //Login to Weblink
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.Login();

            //Verify Privacy Policy
            Assert.IsTrue(test.Driver.FindElement(By.LinkText("Your Privacy Rights")).Displayed, "No Privacy Policy Found");
            IWebElement privacyElement = test.Driver.FindElementByLinkText("Your Privacy Rights");
            Assert.Contains(privacyElement.GetAttribute("href"), "http://www.activenetwork.com/information/privacy-policy.htm", "Privacy Policy Link Not Found");
            privacyElement.Click();

            TestLog.WriteLine("Privacy Policy Found");
            //Assert.AreEqual(test.Driver.Title, "Your Privacy Rights - Active Network", "Did Not Go To Privacy Policy Page");
            Assert.AreEqual(test.Driver.Title, "Your Privacy Rights", "Did Not Go To Privacy Policy Page");
            test.GeneralMethods.VerifyTextPresentWebDriver("Your Privacy Rights");

            // Close the Weblink Session
            test.Driver.Close();
           
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can log into Weblink.")]
        public void Weblink_Login() {
            // Log in to Weblink
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.Login();

            // Verify text      
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("You have successfully logged in."));

            // Close the Weblink Session
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the logout page loads for Webllink.")]
        public void Weblink_Logout() {
            // Logout from Weblink
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.Logout();

            // Verify text      
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("You have successfully logged out."));

            // Close the Weblink Session
            test.Driver.Close();
        }

        #region Create Account
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the create account page loads for Weblink.")]
        public void Weblink_CreateAccount() {
            // Create an account in Weblink
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.ViewCreateAccount();

            // Verify you are on the page    
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Create Account"), "Create Account Link Missing");
            Assert.IsTrue(test.Driver.FindElementsById("btnCreate").Count > 0, "Create Account Button Missing");

            // Close the Weblink Session
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can navigate to the Create Account page from the Login Page.")]
        public void Weblink_CreateAccount_From_Login() {
            // Login to Weblink
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.ViewLoginPage();

            // Navigate to the account creation page
            test.Driver.FindElementByLinkText("Create an account").Click();

            // Verify you are on the page
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Create Account"));
            Assert.IsTrue(test.Driver.FindElementsById("btnCreate").Count > 0);

            // Close the Weblink Session
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create a user in Weblink with an age < 13 years old.")]
        public void Weblink_CreateAccount_Unmatched_Under13() {
            // View the account creation page in weblink
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Driver.Navigate().GoToUrl(test.Weblink.URL);

            // Attempt to create an account
            test.Weblink.CreateAccount("Underage", "User", "underageWeblink@gmail.com", "Pa$$w0rd");

            // Get the activation code from the url from the resulting page and open the page
            test.Weblink.FetchActivationCodeAndOpenAccountCreationPage();

            // Attempt to complete the account creation
            test.Driver.FindElementById("txtDob_textBox").SendKeys(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(-13).AddDays(1).ToShortDateString());  //string.Format("{0:MM/dd/yyyy}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(-13).AddDays(1))
            new SelectElement(test.Driver.FindElementById("ddlGender_dropDownList")).SelectByText("Male");
            test.Driver.FindElementById("ctlAddress_txtAddress1_textBox").SendKeys("9616 Armour Dr");
            test.Driver.FindElementById("ctlAddress_txtCity_textBox").SendKeys("Keller");
            new SelectElement(test.Driver.FindElementById("ctlAddress_ddlState_dropDownList")).SelectByText("Texas");
            test.Driver.FindElementById("ctlAddress_txtPostalCode_textBox").SendKeys("76244");
            test.Driver.FindElementById("btnCreate").Click();

            // Verify the user is prompted regarding the age
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Minimum age requirement not met."));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("You must be at least 13 years of age to join Weblink. If you feel you've reached this page by accident, please contact your church administrator."));

            // Close the Weblink Session
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create a user in Weblink with no date of birth when matching.")]
        public void Weblink_CreateAccount_Matched_NoDOB() {
            // View the account creation page in weblink
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Driver.Navigate().GoToUrl(test.Weblink.URL);

            // Attempt to create an account
            test.Weblink.CreateAccount("Underage", "UserNoDOB", "underageUserNoDOB_plus@gmail.com", "Pa$$w0rd");

            // Get the activation code from the url from the resulting page and open the page
            test.Weblink.FetchActivationCodeAndOpenAccountCreationPage();

            //test.Driver.FindElement(By.Id(""))
            // Finish the account creation
            //Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Please enter your Date of Birth to ensure you meet the minimum age requirement."));
            //We just need a little more info
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("We just need a little more info"));

            test.Driver.FindElementById("txtDob_textBox").SendKeys(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(-13).AddDays(1).ToShortDateString());
            new SelectElement(test.Driver.FindElementById("ddlGender_dropDownList")).SelectByText("Male");
            new SelectElement(test.Driver.FindElementById("ctlAddress_ddlCountry_dropDownList")).SelectByText("China");
            test.GeneralMethods.WaitForPageIsLoaded();
            test.Driver.FindElementById("ctlAddress_txtAddress1_textBox").SendKeys("401 ZiWeiTianYuan D");
            test.Driver.FindElementById("ctlAddress_txtCity_textBox").SendKeys("Xi An");
            test.Driver.FindElementById("ctlAddress_txtPostalCode_textBox").SendKeys("23415");            
            
            test.Driver.FindElementById("btnCreate").Click();

            test.GeneralMethods.WaitForPageIsLoaded();
            // Verify the user is prompted regarding the age restriction
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("You must be at least 13 years of age to join Weblink. If you feel you've reached this page by accident, please contact your church administrator"));

            // Close the Weblink Session
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create a user in Weblink with an age < 13 years old when matching.")]
        public void Weblink_CreateAccount_Matched_Under13() {
            // View the account creation page in weblink
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Driver.Navigate().GoToUrl(test.Weblink.URL);

            // Attempt to create an account
            test.Weblink.CreateAccount("Underage", "User", "underageUser_plus@gmail.com", "Pa$$w0rd");

            // Get the activation code from the url from the resulting page and open the page
            test.Weblink.FetchActivationCodeAndOpenAccountCreationPage();

            test.Driver.FindElementById("txtDob_textBox").SendKeys(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(-13).AddDays(1).ToShortDateString());
            new SelectElement(test.Driver.FindElementById("ddlGender_dropDownList")).SelectByText("Male");
            new SelectElement(test.Driver.FindElementById("ctlAddress_ddlCountry_dropDownList")).SelectByText("China");
            test.GeneralMethods.WaitForPageIsLoaded();
            test.Driver.FindElementById("ctlAddress_txtAddress1_textBox").SendKeys("401 ZiWeiTianYuan D");
            test.Driver.FindElementById("ctlAddress_txtCity_textBox").SendKeys("Xi An");
            test.Driver.FindElementById("ctlAddress_txtPostalCode_textBox").SendKeys("23415");
            test.Driver.FindElementById("btnCreate").Click();

            test.GeneralMethods.WaitForPageIsLoaded();
            // Verify the user is prompted regarding the age restriction
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("You must be at least 13 years of age to join Weblink. If you feel you've reached this page by accident, please contact your church administrator"));

            // Close the Weblink Session
            test.Driver.Close();
        }
        #endregion Create Account

        //[Test, RepeatOnFailure]
        // Based on the F1-3377 story changes this test is no longer valid.
        [Author("Bryan Mikaelian")]
        [Description("Verifies the reset password page loads for Weblink.")]
        public void Weblink_Reset_Password() {
            // Edit Profile
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.ViewResetPassword();

            // Verify you are on the page
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Reset Your Password"));

            // Close the Weblink Session
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the profile editor page.")]
        public void Weblink_Edit_Profile() {
            // Edit Profile
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.ViewEditProfile();

            // Verify you are on the page
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Edit Account"));

            // Verify the marital status selections
            IWebElement dropDown = test.Driver.FindElementById("ddlMaritalStatus_dropDownList");
            Assert.AreEqual(2, dropDown.FindElements(By.TagName("option")).Count);
            //Assert.AreEqual(2, test.Selenium.GetXpathCount("//select[@id='ddlMaritalStatus_dropDownList']/option"));
            Assert.AreEqual("Married", dropDown.FindElements(By.TagName("option"))[0].Text);
            Assert.AreEqual("Single", dropDown.FindElements(By.TagName("option"))[1].Text);

            // Close the Weblink Session
            test.Driver.Close();
        }

        //[Test, RepeatOnFailure]
        // Based on the work done in the F1-3377 story this test is no longer valid.
        [Author("Bryan Mikaelian")]
        [Description("Verifies the login help page loads for Weblink.")]
        public void Weblink_Login_Help() {
            // Login Help
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.ViewHelp();

            // Verify you are on the page
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Login Problems"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("If you forgot your password, please confirm your identity below and enter your User ID. Follow the instructions on the next screen and we will provide you with a new password. If you have forgotten your User ID, please confirm your identity below and enter the email address you registered your account under."));

            // Close the Weblink Session
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies the forgot password page loads for Weblink.")]
        public void Weblink_Login_Forgot_Password()
        {
            // Forgot Password
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.ViewForgotPassword();

            // Verify you are on the page
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Forgot password"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Enter the email address you use to login with and we’ll send you an email containing a link to reset your password."));

            // Close the Weblink Session
            test.Driver.Close();
        }

        #region Online Giving
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the online giving page loads for Weblink.")]
        public void Weblink_OnlineGiving() {
            // Online giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");

            // Verify you are on the page
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Schedule New Contributions"));

            // Verify the available funds are active and web enabled
            System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> options = test.Driver.FindElementById("ddlFund_dropDownList").FindElements(By.TagName("option"));

            for (int i = 0; i < _funds.Rows.Count; i++) {
                Assert.AreEqual(_funds.Rows[i]["FUND_NAME"], options[i + 1].Text);
            }

            //Assert.AreEqual(_funds.Rows.Count, test.Selenium.GetXpathCount("//select[@id='ddlFund_dropDownList']/option") - 1);
            //for (int i = 0; i < _funds.Rows.Count; i++) {
            //    Assert.AreEqual(_funds.Rows[i]["FUND_NAME"], test.Selenium.GetText(string.Format("//select[@id='ddlFund_dropDownList']/option[{0}]", i + 2)));
            //}

            // Close Weblink
            test.Driver.Close();
        }

        #region Immediate
        // Commented out by Mady due to bug FO-4642 that we don't support Giving v1.0 any more.
        //[Test, RepeatOnFailure(Order = 1)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Creates a credit card online contribution designated to a fund.")]
        public void Weblink_OnlineGiving_Contribution_Immediate_CreditCard_Fund() {
            // Online Giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.ViewOnlineGiving("WmD0wg7EO16s29KFjwhGUg==", "ft.autotester@gmail.com", "FT4life!");

            // Create a credit card online contribution
            test.Weblink.OnlineGiving_EnterContribution(101, null, _fund, null, null, null, null, "Visa", new string[] { "FT Tester", "4111111111111111", string.Format("{0}/{1}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year) });

            // Close the Weblink Session
            test.Driver.Close();
        }

        // Commented out by Mady due to bug FO-4642 that we don't support Giving v1.0 any more.
        //[Test, RepeatOnFailure(Order = 2)]
        [Author("Matthew Sneeden")]
        [Description("Creates a credit card online contribution designated to a pledge drive.")]
        public void Weblink_OnlineGiving_Contribution_Immediate_CreditCard_PledgeDrive() {
            // Online giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.ViewOnlineGiving("WmD0wg7EO16s29KFjwhGUg==", "ft.autotester@gmail.com", "FT4life!");

            // Create a credit card online contribution
            test.Weblink.OnlineGiving_EnterContribution(201, null, _fund, null, _pledgeDrive, null, null, "Visa", new string[] { "FT Tester", "4111111111111111", string.Format("{0}/{1}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year) });

            // Close weblink
            test.Driver.Close();
        }

        // Commented out by Mady due to bug FO-4642 that we don't support Giving v1.0 any more.
        // [Test, RepeatOnFailure(Order = 3)]
        [Author("Matthew Sneeden")]
        [Description("Creates an eCheck online contribution.")]
        public void Weblink_OnlineGiving_Contribution_Immediate_eCheck() {
            // Online giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            //test.Weblink.ViewOnlineGiving("WmD0wg7EO16s29KFjwhGUg==", "ft.autotester@gmail.com", "FT4life!");

            // Create a credit card online contribution
            test.Weblink.OnlineGiving_EnterContribution(1301, null, _fund, null, null, null, null, "eCheck", new string[] { "Bank of America", "111000025", "111222333", "111222333" });

            // Close weblink
            test.Driver.Close();
        }

        // Commented out by Mady due to bug FO-4642 that we don't support Giving v1.0 any more.
        // [Test, RepeatOnFailure(Order = 4)]
        [Author("Matthew Sneeden")]
        [Description("Creates an eCheck online contribution.")]
        public void Weblink_OnlineGiving_Contribution_Immediate_eCheck_PledgeDrive() {
            // Online giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            //test.Weblink.ViewOnlineGiving("WmD0wg7EO16s29KFjwhGUg==", "ft.autotester@gmail.com", "FT4life!");

            // Create an eCheck contribution
            test.Weblink.OnlineGiving_EnterContribution(401, null, null, null, _pledgeDrive, null, null, "eCheck", new string[] { "Bank of America", "111000025", "111222333", "111222333" });

            // Close Weblink
            test.Driver.Close();
        }

        #region Cybersource Error Codes
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies a 150 response code from cybersource.")]
        public void Weblink_OnlineGiving_Contribution_Immediate_CreditCard_150() {
            // Online giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.ViewOnlineGiving("WmD0wg7EO16s29KFjwhGUg==", "ft.autotester@gmail.com", "FT4life!");

            // Create a credit card online contribution
            test.Weblink.OnlineGiving_EnterContribution(2000, null, _fund, null, null, null, null, "Visa", new string[] { "FT Tester", "4111111111111111", "12/2020", "150" });

            // Close weblink
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies a 201 response code from cybersource.")]
        public void Weblink_OnlineGiving_Contribution_Immediate_CreditCard_201() {
            // Online giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.ViewOnlineGiving("WmD0wg7EO16s29KFjwhGUg==", "ft.autotester@gmail.com", "FT4life!");

            // Create a credit card online contribution
            test.Weblink.OnlineGiving_EnterContribution(2402.00, null, _fund, null, null, null, null, "Visa", new string[] { "FT Tester", "4111111111111111", "12/2020", "201" });

            // Close weblink
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies a 203 response code from cybersource.")]
        public void Weblink_OnlineGiving_Contribution_Immediate_CreditCard_203() {
            // Online giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.ViewOnlineGiving("WmD0wg7EO16s29KFjwhGUg==", "ft.autotester@gmail.com", "FT4life!");

            // Create a credit card online contribution
            test.Weblink.OnlineGiving_EnterContribution(2260, null, _fund, null, null, null, null, "Visa", new string[] { "FT Tester", "4111111111111111", "12/2020", "203" });

            // Close weblink
            test.Driver.Close();
        }

        // Commented out for bug FO-4642, we no longer support Giving 1.0 payment.
        // [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies a 204 response code from cybersource.")]
        public void Weblink_OnlineGiving_Contribution_Immediate_CreditCard_204() {
            // Online giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.ViewOnlineGiving("WmD0wg7EO16s29KFjwhGUg==", "ft.autotester@gmail.com", "FT4life!");

            // Create a credit card online contribution
            test.Weblink.OnlineGiving_EnterContribution(2521, null, _fund, null, null, null, null, "Visa", new string[] { "FT Tester", "4111111111111111", "12/2020", "204" });

            // Close weblink
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies a 205 response code from cybersource.")]
        public void Weblink_OnlineGiving_Contribution_Immediate_CreditCard_205() {
            // Online giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.ViewOnlineGiving("WmD0wg7EO16s29KFjwhGUg==", "ft.autotester@gmail.com", "FT4life!");

            // Create a credit card online contribution
            test.Weblink.OnlineGiving_EnterContribution(2501, null, _fund, null, null, null, null, "Visa", new string[] { "FT Tester", "4111111111111111", "12/2020", "205" });

            // Close weblink
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies a 208 response code from cybersource.")]
        public void Weblink_OnlineGiving_Contribution_Immediate_CreditCard_208() {
            // Online giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.ViewOnlineGiving("WmD0wg7EO16s29KFjwhGUg==", "ft.autotester@gmail.com", "FT4life!");

            // Create a credit card online contribution
            test.Weblink.OnlineGiving_EnterContribution(2606, null, _fund, null, null, null, null, "Visa", new string[] { "FT Tester", "4111111111111111", "12/2020", "208" });

            // Close weblink
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies a 209 response code from cybersource.")]
        public void Weblink_OnlineGiving_Contribution_Immediate_CreditCard_209() {
            // Online giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.ViewOnlineGiving("WmD0wg7EO16s29KFjwhGUg==", "ft.autotester@gmail.com", "FT4life!");

            // Create a credit card online contribution
            test.Weblink.OnlineGiving_EnterContribution(2811, null, _fund, null, null, null, null, "American Express", new string[] { "FT Tester", "378282246310005", "12/2020", "209" });

            // Close weblink
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies a 211 response code from cybersource.")]
        public void Weblink_OnlineGiving_Contribution_Immediate_CreditCard_211() {
            // Online giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.ViewOnlineGiving("WmD0wg7EO16s29KFjwhGUg==", "ft.autotester@gmail.com", "FT4life!");

            // Create a credit card online contribution
            test.Weblink.OnlineGiving_EnterContribution(2531, null, _fund, null, null, null, null, "Visa", new string[] { "FT Tester", "4111111111111111", "12/2020", "211" });

            // Close weblink
            test.Driver.Close();
        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies a 230 response code from cybersource.")]
        public void Weblink_OnlineGiving_Contribution_Immediate_CreditCard_230() {
            // Online giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.ViewOnlineGiving("WmD0wg7EO16s29KFjwhGUg==", "ft.autotester@gmail.com", "FT4life!");

            // Create a credit card online contribution
            test.Weblink.OnlineGiving_EnterContribution(1000, null, _fund, null, null, null, null, "Visa", new string[] { "FT Tester", "4111111111111111", "12/2020", "230" });

            // Close weblink
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies a 231 response code from cybersource.")]
        public void Weblink_OnlineGiving_Contribution_Immediate_CreditCard_231() {
            // Online giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.ViewOnlineGiving("WmD0wg7EO16s29KFjwhGUg==", "ft.autotester@gmail.com", "FT4life!");

            // Create a credit card online contribution
            test.Weblink.OnlineGiving_EnterContribution(1000, null, _fund, null, null, null, null, "Visa", new string[] { "FT Tester", "4111111111111112", "12/2020", "231" });

            // Close weblink
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies a 233 response code from cybersource.")]
        public void Weblink_OnlineGiving_Contribution_Immediate_CreditCard_233() {
            // Online giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.ViewOnlineGiving("WmD0wg7EO16s29KFjwhGUg==", "ft.autotester@gmail.com", "FT4life!");

            // Create a credit card online contribution
            test.Weblink.OnlineGiving_EnterContribution(2204, null, _fund, null, null, null, null, "Visa", new string[] { "FT Tester", "4111111111111111", "12/2020", "233" });

            // Close weblink
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies a 234 response code from cybersource.")]
        public void Weblink_OnlineGiving_Contribution_Immediate_CreditCard_234() {
            // Online giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.ViewOnlineGiving("WmD0wg7EO16s29KFjwhGUg==", "ft.autotester@gmail.com", "FT4life!");

            // Create a credit card online contribution
            test.Weblink.OnlineGiving_EnterContribution(2231, null, _fund, null, null, null, null, "Visa", new string[] { "FT Tester", "4111111111111111", "12/2020", "234" });

            // Close weblink
            test.Driver.Close();
        }
        #endregion Cybersource Error Codes

        #region ProfitStars Velocity Exceptions
        // Commented out by Mady due to bug FO-4642 that we don't support Giving v1.0 any more.
        // [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Verifies the exception for exceeding the velocity amount.")]
        public void Weblink_OnlineGiving_Contribution_Immediate_eCheck_Velocity_Amount() {
            // Login to Weblink->Online Giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");

            // Create an eCheck contribution that exceeds the velocity amount
            test.Weblink.OnlineGiving_EnterContribution(1101.00, "One time", "Auto Test Fund", null, null, null, null, "eCheck", new string[] { "Bank of America", "111000025", "123123123", "123123123", "velocity_amount" });

            // Close Weblink
            test.Driver.Close();
        }

        // Commented out by Mady due to bug FO-4642 that we don't support Giving v1.0 any more.
        // [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the exception for exceeding the velocity count.")]
        public void Weblink_OnlineGiving_Contribution_Immediate_eCheck_Velocity_Count() {
            // Login to Weblink->Online Giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");

            // Create an eCheck contribution that exceeds the velocity count
            test.Weblink.OnlineGiving_EnterContribution(1102.00, "One time", "Auto Test Fund", null, null, null, null, "eCheck", new string[] { "Bank of America", "111000025", "123123123", "123123123", "velocity_count" });

            // Close Weblink
            test.Driver.Close();
        }
        #endregion ProfitStars Velocity Exceptions

        #region Authorization Reversals
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies an authorization reversal is generated.")]
        public void Weblink_OnlineGiving_Contribution_Immediate_eCheck_AuthorizationReversal() {
            // Login to Weblink->Online Giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            //"kmtT7SIE6PXAocYwed5fKA==", "bmikaelian@fellowshiptech.com", "BM.Admin09"

            // Create an eCheck contribution that exceeds the velocity amount
            test.Weblink.OnlineGiving_EnterContribution(10.00, "One time", "Building Fund", "Sports Building Addition", null, null, null, "eCheck", new string[] { "Bank of America", "111000025", "999999999", "999999999", "authorization_reversal" });

            // Close Weblink
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies an authorization reversal is generated.")]
        public void Weblink_OnlineGiving_Contribution_Immediate_CreditCard_AuthorizationReversal() {
            // Login to Weblink->Online Giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");

            // Create a credit card online contribution
            test.Weblink.OnlineGiving_EnterContribution(100, null, _fund, null, null, null, null, "Visa", new string[] { "Error McGee", "4111111111111111", "12/2015", "999" });

            // Close weblink
            test.Driver.Close();
        }
        #endregion Authorization Reversals
        #endregion Immediate

        #region Scheduled
        // Commented out by Mady due to bug FO-4642 that we don't support Giving v1.0 any more.
        // [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Creates a scheduled contribution with a credit card.")]
        public void Weblink_OnlineGiving_Scheduled_Create_CreditCard() {
            // Set initial conditions
            double amount = 1.01;
            // base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(256, base.SQL.People_Individuals_FetchID(256, "FT Tester"), amount, 4);
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(99005, base.SQL.People_Individuals_FetchID(99005, "FT Tester"), amount, 4);

            // View online giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.ViewOnlineGiving("WmD0wg7EO16s29KFjwhGUg==", "ft.autotester@gmail.com", "FT4life!");

            // Create a scheduled contribution
            test.Weblink.OnlineGiving_EnterContribution(amount, "One time", "Auto Test Fund", null, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1.0).ToShortDateString(), null, "Visa", new string[] { "FT Tester", "4111111111111111", string.Format("{0}/{1}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year) });

            // Close weblink
            test.Driver.Close();
        }

        // Commented out by Mady due to bug FO-4642 that we don't support Giving v1.0 any more.
        // [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Updates a scheduled contribution with a credit card.")]
        public void Weblink_OnlineGiving_Scheduled_Update_CreditCard() {
            // Set initial conditions
            double amount = 1.02;
            double amountUpdated = 1.03;
            // base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(256, base.SQL.People_Individuals_FetchID(256, "FT Tester"), amount, 4);
            // base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(256, base.SQL.People_Individuals_FetchID(256, "FT Tester"), amountUpdated, 4);
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(99005, base.SQL.People_Individuals_FetchID(99005, "FT Tester"), amount, 4);
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(99005, base.SQL.People_Individuals_FetchID(99005, "FT Tester"), amountUpdated, 4);

            // View online giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.ViewOnlineGiving("WmD0wg7EO16s29KFjwhGUg==", "ft.autotester@gmail.com", "FT4life!");

            // Create a scheduled contribution
            test.Weblink.OnlineGiving_EnterContribution(amount, "One time", "Auto Test Fund", null, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1.0).ToShortDateString(), null, "Visa", new string[] { "FT Tester", "4111111111111111", string.Format("{0}/{1}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year) });

            // Update a scheduled contribution
            //test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.ViewOnlineGiving("WmD0wg7EO16s29KFjwhGUg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.OnlineGiving_UpdateScheduledContribution(amount, amountUpdated, "Visa");

            // Close weblink
            test.Driver.Close();
        }

        // Commented out by Mady due to bug FO-4642 that we don't support Giving v1.0 any more.
        // [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Deletes a scheduled contribution with a credit card.")]
        public void Weblink_OnlineGiving_Scheduled_Delete_CreditCard() {
            // Set initial conditions
            double amount = 1.04;
            // base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(256, base.SQL.People_Individuals_FetchID(256, "FT Tester"), amount, 4);
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(99005, base.SQL.People_Individuals_FetchID(99005, "FT Tester"), amount, 4);
            
            // View online giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.ViewOnlineGiving("WmD0wg7EO16s29KFjwhGUg==", "ft.autotester@gmail.com", "FT4life!");

            // Create a scheduled contribution
            test.Weblink.OnlineGiving_EnterContribution(amount, "One time", "Auto Test Fund", null, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1.0).ToShortDateString(), null, "Visa", new string[] { "FT Tester", "4111111111111111", string.Format("{0}/{1}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year) });

            // Delete a scheduled contribution
            //test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.ViewOnlineGiving("WmD0wg7EO16s29KFjwhGUg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.OnlineGiving_DeleteScheduledContribution(amount);

            // Close weblink
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Creates a scheduled contribution with an echeck.")]
        public void Weblink_OnlineGiving_Scheduled_Create_eCheck() {
            // Set initial conditions
            double amount = 2.01;
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(256, base.SQL.People_Individuals_FetchID(256, "FT Tester"), amount, 4);

            // View online giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");

            // Create a scheduled contribution
            test.Weblink.OnlineGiving_EnterContribution(amount, "One time", "Auto Test Fund", null, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1.0).ToShortDateString(), null, "eCheck", new string[] { "Bank of America", "111000025", "1234567890", "1234567890" });

            // Close weblink
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Creates a scheduled contribution with an echeck.")]
        public void Weblink_OnlineGiving_Scheduled_Update_eCheck() {
            // Set initial conditions
            double amount = 2.02;
            double amountUpdated = 2.03;
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(256, base.SQL.People_Individuals_FetchID(256, "FT Tester"), amount, 4);
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(256, base.SQL.People_Individuals_FetchID(256, "FT Tester"), amountUpdated, 4);

            // View online giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");

            // Create a scheduled contribution
            test.Weblink.OnlineGiving_EnterContribution(amount, "One time", "Auto Test Fund", null, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1.0).ToShortDateString(), null, "eCheck", new string[] { "Bank of America", "111000025", "1234567890", "1234567890" });

            // Update a scheduled contribution
            test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.OnlineGiving_UpdateScheduledContribution(amount, amountUpdated, "eCheck");

            // Close weblink
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Creates a scheduled contribution with an echeck.")]
        public void Weblink_OnlineGiving_Scheduled_Delete_eCheck() {
            // Set initial conditions
            double amount = 2.04;
            base.SQL.Giving_ScheduledContributions_Delete_PortalORWeblink(256, base.SQL.People_Individuals_FetchID(256, "FT Tester"), amount, 4);

            // View online giving
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");

            // Create a scheduled contribution
            test.Weblink.OnlineGiving_EnterContribution(amount, "One time", "Auto Test Fund", null, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1.0).ToShortDateString(), null, "eCheck", new string[] { "Bank of America", "111000025", "1234567890", "1234567890" });

            // Delete a scheduled contribution
            test.Weblink.ViewOnlineGiving("P+A1VRVfzQ3zi6cqHjvGMg==", "ft.autotester@gmail.com", "FT4life!");
            test.Weblink.OnlineGiving_DeleteScheduledContribution(amount);

            // Close weblink
            test.Driver.Close();
        }
        #endregion Scheduled
        #endregion Online Giving

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Views and verifies the contact form page.")]
        public void Weblink_Contact_Form() {
            // View the contact form page
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.ViewContactForm();

            // Verify you are on the contact form page
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Please select an item of interest and submit any questions or comments you might have."));

            // Verify the marital status selections
            IWebElement dropDown = test.Driver.FindElementById("ddlMaritalStatus_dropDownList");
            Assert.AreEqual(2, dropDown.FindElements(By.TagName("option")).Count);
            Assert.AreEqual("Married", dropDown.FindElements(By.TagName("option"))[0].Text);
            Assert.AreEqual("Single", dropDown.FindElements(By.TagName("option"))[1].Text);

            // Close weblink
            test.Driver.Close();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views and verifies the add individual page tied to event registration.")]
        public void Weblink_EventRegistration_AddIndividual() {
            // Login to Weblink
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.Login();

            // View the add individual page for event registration
            test.Weblink.ViewEventRegistrationAddIndividualURL();

            // Verify the marital status selections
            IWebElement dropDown = test.Driver.FindElementById("ddlMaritalStatus");
            Assert.AreEqual(3, dropDown.FindElements(By.TagName("option")).Count);
            Assert.AreEqual("", dropDown.FindElements(By.TagName("option"))[0].Text);
            Assert.AreEqual("Married", dropDown.FindElements(By.TagName("option"))[1].Text);
            Assert.AreEqual("Single", dropDown.FindElements(By.TagName("option"))[2].Text);

            // Close weblink
            test.Driver.Close();
        }

        //[Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("F1-4085: Verifies credit card images are present")]
        public void WebLink_EventRegistration_Verify_CreditCard_Images()
        {
            // Login to Weblink
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Weblink.Login();

            // View the add individual page for event registration
            test.Weblink.ViewEventRegistration();

            // Verify the marital status selections
            //IWebElement dropDown = test.Driver.FindElementById("ddlMaritalStatus");
            //Assert.AreEqual(3, dropDown.FindElements(By.TagName("option")).Count);
            //Assert.AreEqual("", dropDown.FindElements(By.TagName("option"))[0].Text);
            //Assert.AreEqual("Married", dropDown.FindElements(By.TagName("option"))[1].Text);
            //Assert.AreEqual("Single", dropDown.FindElements(By.TagName("option"))[2].Text);

            // Close weblink
            test.Driver.Close();
        }

    }
}