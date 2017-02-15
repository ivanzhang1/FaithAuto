using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using log4net;

using ActiveUp.Net.Mail;
using System.Linq;
using System.Xml.Linq;
using OpenQA.Selenium;

namespace FTTests.API {

    [TestFixture]
    public class PeopleAPI_Forgot_Password : FixtureBaseWebDriver
    {

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Verifies the user is taken to Forgot Password Home Page")]
        public void ForgotPassword_HomePage()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string forgotPasswordURL = string.Format("{0}{1}", test.API.GetAPIURL(), "accounts/forgotpassword");
            test.GeneralMethods.OpenURLWebDriver(forgotPasswordURL);

            //Verify Elements
            test.GeneralMethods.VerifyTextPresentWebDriver("Forgot Password");
            test.GeneralMethods.VerifyTextPresentWebDriver("Enter the email address you use to login with, and we'll send you an email containing a link to reset your password.");
            Assert.IsTrue(test.Driver.FindElementByName("email").Displayed, "Reset Password Email Input Text Not Displayed");

            // /html/body/div[2]/form/button
            Assert.IsTrue(test.Driver.FindElement(By.CssSelector("[class='btn input-large']")).Displayed, "Reset Password Button Not Displayed");
            Assert.AreEqual("Reset Password", test.Driver.FindElement(By.CssSelector("[class='btn input-large']")).Text, "Reset Password Text Not Displayed in Button");

            test.Driver.Close();


        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Reset Password Successfully")]
        public void ForgotPassword_Successs()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string forgotPasswordURL = string.Format("{0}{1}", test.API.GetAPIURL(), "accounts/forgotpassword");
            test.GeneralMethods.OpenURLWebDriver(forgotPasswordURL);

            //Setup 
            string emailAddr = "ft.automatedemail03@gmail.com";
            string password = "FTAdmin01!";
            string _fromAddr = "fellowshiponemail@activenetwork.com";
            string[] _recipients = new string[] {"Test Email"};


            //Enter Valid Account Email
            test.Driver.FindElement(By.Name("email")).SendKeys(emailAddr);

            //Reset password
            test.Driver.FindElement(By.CssSelector("[class='btn input-large']")).Click();
            string emailTimeStamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("HH:mm");

            //Verify that we sent e-mail
            test.GeneralMethods.WaitForElement(test.Driver, By.CssSelector("[class='alert alert-success']"));
            test.GeneralMethods.VerifyTextPresentWebDriver(emailAddr);

            // Retrieve Email
            string emailSubject = "Change Password Request: Dynamic Church";
            string emailBody = "A password change was requested for this email address, please navigate to the address below to create a new password.";

            Message grpMsg = test.Portal.Retrieve_Search_Email(emailAddr, password, emailBody);
            test.Portal.Verify_Sent_Email(grpMsg, emailSubject, _fromAddr, "", _recipients, emailBody);

            //Email body (link subject to changes)
            // A password change was requested for this email address, please navigate to the address below to create a new password.
            //https://dc.staging.fellowshiponeapi.com/v1/Accounts/ResetPassword/4392fab3943a4121a809b7e29f18202e
            //Thank you.

            TestLog.WriteLine("Enivornment: " + this.F1Environment.ToString());

            string resetUrl = test.Portal.Get_URL_Email_Body(grpMsg.BodyHtml.Text);

            TestLog.WriteLine("Reset URL: " + resetUrl);
            test.GeneralMethods.OpenURLWebDriver(resetUrl);

            //Are we there yet?
            test.GeneralMethods.WaitForElement(test.Driver, By.Name("password"));

            // Reset Password page
            test.Driver.FindElement(By.Name("password")).SendKeys(password);
            test.Driver.FindElement(By.Name("password-confirm")).SendKeys(password);

            //Reset password
            test.Driver.FindElement(By.CssSelector("[class='btn input-full']")).Click();

            test.GeneralMethods.WaitForElement(test.Driver, By.CssSelector("[class='alert alert-success']"));
            //test.GeneralMethods.VerifyElementDisplayedWebDriver(By.CssSelector("[class='alert alert-success']"));
            test.GeneralMethods.VerifyTextPresentWebDriver("Reset Password");
            test.GeneralMethods.VerifyTextPresentWebDriver("You have successfully updated your password!");

            test.Driver.Close();

        }

        /// ---------------- ADD TC for new login security to Portal and Infellowship based on the three time security failure. ---------------------------------------------------------------    
        
        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Reset Password Using Invalid Token")]
        public void ForgotPassword_Invalid_Token()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            //Navigate to Previously used reset links
            string invalidTokenURL = string.Empty;
            string resetToken = Guid.NewGuid().ToString().Substring(0, 32);

            TestLog.WriteLine("Enivornment: " + this.F1Environment.ToString());
            TestLog.WriteLine("Reset Token: " + resetToken);

            invalidTokenURL = string.Format("{0}{1}{2}", test.API.GetAPIURL(), "accounts/ResetPassword/", resetToken);
            
            //Navigate to expired reset password url
            TestLog.WriteLine("Invalid URL " + invalidTokenURL);
            test.GeneralMethods.OpenURLWebDriver(invalidTokenURL);

            //Did we make it? 
            test.GeneralMethods.WaitForElement(test.Driver, By.CssSelector("[class='container']"));
            test.GeneralMethods.VerifyTextPresentWebDriver("This forgot password code is invalid");
        }


        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Reset Password using Expired Token")]
        public void ForgotPassword_Expired_Token()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            
            //Navigate to Previously used reset links
            string resetPasswordURL = string.Empty;
            string resetToken = string.Empty;

            TestLog.WriteLine("Enivornment: " + this.F1Environment.ToString());

            switch (this.F1Environment.ToString())
            {

                case "QA":
                    resetToken = "b753e0387d1441dd8e15975b236f43b6";
                    break;

                case "STAGING":
                    //resetToken = "2b82dc290904444e94dab1732c92e7c7";
                    resetToken = "7F485932BA1F4FBCBD62064D338F08A7";
                    break;

                case "LV_QA":
                    resetToken = "b753e0387d1441dd8e15975b236f43b6";
                    break;

                case "LV_UAT":
                    //resetToken = "ead6aa6794ba41dbbd80ab085e0cdd83";
                    resetToken = "7F485932BA1F4FBCBD62064D338F08A7";
                    break;

                case "LV_PROD":
                    resetToken = "";
                    break;

                default:
                    throw new WebDriverException(string.Format("{0} not valid enviornment", this.F1Environment.ToString()));

            }


            resetPasswordURL = string.Format("{0}accounts/ResetPassword/{1}", test.API.GetAPIURL(), resetToken);                
            

            //Navigate to expired reset password url
            test.GeneralMethods.OpenURLWebDriver(resetPasswordURL);

            //Did we make it? 
            test.GeneralMethods.WaitForElement(test.Driver, By.CssSelector("[class='container']"));
            test.GeneralMethods.VerifyTextPresentWebDriver("Your forgot password code has expired");
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Reset Password with an Invalid email")]
        public void ForgotPassword_Invalid_Email()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string forgotPasswordURL = string.Format("{0}{1}", test.API.GetAPIURL(), "accounts/forgotpassword");
            test.GeneralMethods.OpenURLWebDriver(forgotPasswordURL);

            //Enter Invalid Account Email
            test.Driver.FindElement(By.Name("email")).SendKeys("ang.platt@gmail.com");

            //Reset password
            test.Driver.FindElement(By.CssSelector("[class='btn input-large']")).Click();

            //Verify error message displayed
            test.GeneralMethods.VerifyElementDisplayedWebDriver(By.CssSelector("[class='alert alert-error']"));
            test.GeneralMethods.VerifyTextPresentWebDriver("The email address provided could not be found");

            test.Driver.Close();

        }

    }

    [TestFixture]
    public class PeopleAPI_Create_Account : FixtureBaseWebDriver
    {

        //[Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Verifies the general help document for Create Account")]
        public void Create_Account_Help()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string forgotPasswordURL = string.Format("{0}{1}", test.API.GetAPIURL(), "accounts/forgotpassword");
            test.GeneralMethods.OpenURLWebDriver(forgotPasswordURL);

            //Verify Elements
            test.GeneralMethods.VerifyTextPresentWebDriver("Forgot Password");
            test.GeneralMethods.VerifyTextPresentWebDriver("Enter the email address you use to login with, and we'll send you an email containing a link to reset your password.");
            Assert.IsTrue(test.Driver.FindElementByName("email").Displayed, "Reset Password Email Input Text Not Displayed");

            // /html/body/div[2]/form/button
            Assert.IsTrue(test.Driver.FindElement(By.CssSelector("[class='btn input-large']")).Displayed, "Reset Password Button Not Displayed");
            Assert.AreEqual(test.Driver.FindElement(By.CssSelector("[class='btn input-large']")).Text, "Reset Password");

            test.Driver.Close();


        }



    }

    [TestFixture]
    public class PeopleAPI
    {
        #region Help

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the general help document for the People API")]
        public void General() {
            APIBase api = new APIBase();

            HttpWebResponse response = api.MakeAPIRequest("GET", "application/help", "Util/Docs.help");

            // General
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.LessThanOrEqualTo(0, response.ContentLength);

            string responseString = api.GetResponseString(response);

            TestLog.WriteLine("Response: " + responseString);
            Assert.Contains(responseString, "Fellowship One RESTful API - Docs");
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the Households help document for the People API")]
        public void Households() {
            APIBase api = new APIBase();

            // Households
            HttpWebResponse response =  api.MakeAPIRequest("GET", "application/help", "Households.help");


            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.LessThanOrEqualTo(0, response.ContentLength);

            string responseString = api.GetResponseString(response);

            TestLog.WriteLine("Response: " + responseString);
            Assert.Contains(responseString, "Fellowship One RESTful API - Households");
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the People help document for the People API")]
        public void People() {
            APIBase api = new APIBase();

            // People
            HttpWebResponse response =  api.MakeAPIRequest("GET", "application/help", "People.help");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.LessThanOrEqualTo(0, response.ContentLength);

            string responseString = api.GetResponseString(response);

            TestLog.WriteLine("Response: " + responseString);
            Assert.Contains(responseString, "Fellowship One RESTful API - People");

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the Addresses help document for the People API")]
        public void Addresses() {
            APIBase api = new APIBase();

            // Addresses
            HttpWebResponse response =  api.MakeAPIRequest("GET", "application/help", "Addresses.help");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.LessThanOrEqualTo(0, response.ContentLength);

            string responseString = api.GetResponseString(response);

            TestLog.WriteLine("Response: " + responseString);
            Assert.Contains(responseString, "Fellowship One RESTful API - Addresses");
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the AttributeGroups help document for the People API")]
        public void AttributeGroups() {
            APIBase api = new APIBase();

            // AttributeGroups
            HttpWebResponse response =  api.MakeAPIRequest("GET", "application/help", "people/AttributeGroups.help");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.LessThanOrEqualTo(0, response.ContentLength);

            string responseString = api.GetResponseString(response);

            TestLog.WriteLine("Response: " + responseString);
            Assert.Contains(responseString, "Fellowship One RESTful API - AttributeGroups");
        
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the Communications help document for the People API")]
        public void Communications() {
            APIBase api = new APIBase();

            // Communications
            HttpWebResponse response = api.MakeAPIRequest("GET", "application/help", "Communications.help");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.LessThanOrEqualTo(0, response.ContentLength);

            string responseString = api.GetResponseString(response);

            TestLog.WriteLine("Response: " + responseString);
            Assert.Contains(responseString, "Fellowship One RESTful API - Communications");

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the Denominations help document for the People API")]
        public void Denominations() {
            APIBase api = new APIBase();

            // Denominations
            HttpWebResponse response = api.MakeAPIRequest("GET", "application/help", "people/Denominations.help");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.LessThanOrEqualTo(0, response.ContentLength);

            string responseString = api.GetResponseString(response);

            TestLog.WriteLine("Response: " + responseString);
            Assert.Contains(responseString, "Fellowship One RESTful API - Denominations");
        
        }


        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the Occupations help document for the People API")]
        public void Occupations() {
            APIBase api = new APIBase();

            // Occupations
            HttpWebResponse response = api.MakeAPIRequest("GET", "application/help", "people/Occupations.help");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.LessThanOrEqualTo(0, response.ContentLength);

            string responseString = api.GetResponseString(response);

            TestLog.WriteLine("Response: " + responseString);
            Assert.Contains(responseString, "Fellowship One RESTful API - Occupations");
        
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the Schools help document for the People API")]
        public void Schools() {
            APIBase api = new APIBase();

            // Schools
            HttpWebResponse response = api.MakeAPIRequest("GET", "application/help", "people/Schools.help");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.LessThanOrEqualTo(0, response.ContentLength);

            string responseString = api.GetResponseString(response);

            TestLog.WriteLine("Response: " + responseString);
            Assert.Contains(responseString, "Fellowship One RESTful API - Schools");
        
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the Statuses help document for the People API")]
        public void Statuses() {
            APIBase api = new APIBase();

            // Statuses
            HttpWebResponse response =  api.MakeAPIRequest("GET", "application/help", "people/Statuses.help");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.LessThanOrEqualTo(0, response.ContentLength);

            string responseString = api.GetResponseString(response);

            TestLog.WriteLine("Response: " + responseString);
            Assert.Contains(responseString, "Fellowship One RESTful API - Statuses");

        }


        #endregion Help

    }

    [TestFixture]
    public class PeopleAPIFromPHP : APIBase
    {
        private string houseHoldId = "";
        private string peopleId = "";
        private string memberId = "";
        private string responseString = "";
        private string responseTemp = "";

        #region Households

        [Test(Order=1), RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verify household search api works well")]
        public void TestHouseholdSearch()
        {
            string uri = string.Format("{0}households/search.json?createdDate=2009-01-01", this.GetAPIURL());
            responseTemp = SendAPIRequest(uri, "GET", "", HttpStatusCode.OK);

            Assert.Contains(responseTemp, "\"results\":{\"@count\":");

            this.houseHoldId = GetFirstValueByStrKey(responseTemp, "@id");
            TestLog.WriteLine("Get householdId: " + houseHoldId);
            Assert.AreNotEqual("", houseHoldId);
        }

        [Test(Order = 2), RepeatOnFailure, DependsOn("TestHouseholdSearch")]
        [Author("Mady Kou")]
        [Description("Verify specific household show api works well")]
        public void TestHouseholdShow()
        {
            string uri = string.Format("{0}households/{1}.json", this.GetAPIURL(), this.houseHoldId);
            responseTemp = SendAPIRequest(uri, "GET", "", HttpStatusCode.OK);

            Assert.Contains(responseTemp, "{\"household\":");
            Assert.Contains(responseTemp, String.Format("\"@id\":\"{0}\",", this.houseHoldId));
        }

        [Test(Order = 3), RepeatOnFailure, DependsOn("TestHouseholdSearch")]
        [Author("Mady Kou")]
        [Description("Verify specific household edit api works well")]
        public void TestHouseholdEdit()
        {
            string uri = string.Format("{0}households/{1}/edit.json", this.GetAPIURL(), this.houseHoldId);
            responseString = SendAPIRequest(uri, "GET", "", HttpStatusCode.OK);

            Assert.Contains(responseString, "{\"household\":");
            Assert.Contains(responseString, String.Format("\"@id\":\"{0}\",", this.houseHoldId));
        }

        [Test(Order = 4), RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verify specific household new api works well")]
        public void TestHouseholdNew()
        {
            string uri = string.Format("{0}households/new.json", this.GetAPIURL());
            responseString = SendAPIRequest(uri, "GET", "", HttpStatusCode.OK);

            Assert.Contains(responseString, "{\"household\":");
            Assert.Contains(responseString, "\"householdName\":");
        }

        [Test(Order = 5), RepeatOnFailure, DependsOn("TestHouseholdNew")]
        [Author("Mady Kou")]
        [Description("Verify specific household create api works well")]
        public void TestHouseholdCreate()
        {
            string json = SetValueByStrKey(responseString, "householdName", "API Create Unit Test - " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("yyyy-m-d H:m:s"));
            string uri = string.Format("{0}households.json", this.GetAPIURL());

            responseTemp = SendAPIRequest(uri, "POST", json, HttpStatusCode.Created);

            Assert.Contains(responseTemp, "{\"household\":");
            Assert.Contains(responseTemp, "\"householdName\":");

            this.houseHoldId = GetValueByStrKey(responseTemp, "household", "@id");
        }

        [Test(Order = 6), RepeatOnFailure, DependsOn("TestHouseholdEdit")]
        [Author("Mady Kou")]
        [Description("Verify specific household update api works well")]
        public void TestHouseholdUpdate()
        {
            string json = SetValueByStrKey(responseString, "householdName", "API Create Unit Test - " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("yyyy-m-d H:m:s"));
            string uri = string.Format("{0}households/{1}.json", this.GetAPIURL(), "21707654");

            responseTemp = SendAPIRequest(uri, "PUT", json, HttpStatusCode.OK);
        }

        #endregion Households

        #region HouseholdMemberTypes

        [Test(Order = 7), RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verify household member type list api works well")]
        public void TestHouseholdMemberTypesList()
        {
            string uri = string.Format("{0}people/householdmembertypes.json", this.GetAPIURL());

            responseTemp = SendAPIRequest(uri, "GET", "", HttpStatusCode.OK);

            Assert.Contains(responseTemp, "\"householdMemberTypes\":{\"householdMemberType\":");

            memberId = GetFirstValueByStrKey(responseTemp, "@id");
            TestLog.WriteLine("Get householdMemberType Id: " + memberId);
            Assert.AreNotEqual("", memberId);
        }

        [Test(Order = 8), RepeatOnFailure, DependsOn("TestHouseholdMemberTypesList")]
        [Author("Mady Kou")]
        [Description("Verify household member type show api works well")]
        public void TestHouseholdMemberTypesShow()
        {
            string uri = string.Format("{0}people/householdmembertypes/{1}.json", this.GetAPIURL(), memberId);

            responseTemp = SendAPIRequest(uri, "GET", "", HttpStatusCode.OK);

            Assert.Contains(responseTemp, "{\"householdMemberType\":");
            Assert.Contains(responseTemp, String.Format("\"@id\":\"{0}\",", memberId));
        }

        #endregion  HouseholdMemberTypes

        #region People

        [Test(Order = 9), RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verify people search api works well")]
        public void TestPeopleSearch()
        {
            string uri = string.Format("{0}people/search.json?createdDate=2010-01-01", this.GetAPIURL());
            responseTemp = SendAPIRequest(uri, "GET", "", HttpStatusCode.OK);

            Assert.Contains(responseTemp, "\"results\":{\"@count\":");

            peopleId = GetFirstValueByStrKey(responseTemp, "@id");
            TestLog.WriteLine("Get people Id: " + peopleId);
            Assert.AreNotEqual("", peopleId);
        }

        [Test(Order = 10), RepeatOnFailure, DependsOn("TestPeopleSearch")]
        [Author("Mady Kou")]
        [Description("Verify people show api works well")]
        public void TestPeopleShow()
        {
            string uri = string.Format("{0}people/{1}.json", this.GetAPIURL(), peopleId);
            responseTemp = SendAPIRequest(uri, "GET", "", HttpStatusCode.OK);

            Assert.Contains(responseTemp, "{\"person\":");
            Assert.Contains(responseTemp, String.Format("\"@id\":\"{0}\",", peopleId));
        }

        [Test(Order = 11), RepeatOnFailure, DependsOn("TestPeopleSearch")]
        [Author("Mady Kou")]
        [Description("Verify people edit api works well")]
        public void TestPeopleEdit()
        {
            string uri = string.Format("{0}people/{1}/edit.json", this.GetAPIURL(), peopleId);
            responseString = SendAPIRequest(uri, "GET", "", HttpStatusCode.OK);

            Assert.Contains(responseString, "{\"person\":");
            Assert.Contains(responseString, String.Format("\"@id\":\"{0}\",", peopleId));
        }

        [Test(Order = 12), RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verify people new api works well")]
        public void TestPeopleNew()
        {
            string uri = string.Format("{0}people/new.json", this.GetAPIURL());
            responseString = SendAPIRequest(uri, "GET", "", HttpStatusCode.OK);

            Assert.Contains(responseString, "{\"person\":");
        }

        [Test(Order = 13), RepeatOnFailure, DependsOn("TestPeopleNew"), DependsOn("TestHouseholdCreate")]
        [Author("Mady Kou")]
        [Description("Verify people create api works well")]
        public void TestPeopleCreate()
        {
            string uri = string.Format("{0}people.json", this.GetAPIURL());
            string json = SetValueByStrKey(responseString, "@householdID", this.houseHoldId);
            json = SetValueByStrKey(json, "firstName", "API Unit");
            json = SetValueByStrKey(json, "lastName", "Test");
            json = SetValueByStrKey(json, "householdMemberType", "@id", "1");
            json = SetValueByStrKey(json, "status", "@id", "1");

            responseString = SendAPIRequest(uri, "POST", json, HttpStatusCode.Created);

            Assert.Contains(responseString, "{\"person\":");
        }

        [Test(Order = 14), RepeatOnFailure, DependsOn("TestPeopleCreate")]
        [Author("Mady Kou")]
        [Description("Verify people update api works well")]
        public void TestPeopleUpdate()
        {
            string uri = string.Format("{0}people/{1}.json", this.GetAPIURL(), GetValueByStrKey(responseString, "person", "@id"));
            string json = SetValueByStrKey(responseString, "person", "lastName", "Test Update");

            responseTemp = SendAPIRequest(uri, "PUT", json, HttpStatusCode.OK);

            Assert.Contains(responseTemp, "{\"person\":");
        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("FO-4973 API: People Merge Losers API (FO-4075) Not Filtering By ChurchCode")]
        public void TestPeopleMergerSearch()
        {
            string uri = string.Format("{0}people/mergers/search?lastupdateddate=2015-08-31&mode=demo", this.GetAPIURL());
            responseTemp = SendAPIRequestNoAuth(uri, "GET", HttpStatusCode.OK);

            Assert.Contains(responseTemp, "<results count=\"");
        }

        #endregion People
    }
}
