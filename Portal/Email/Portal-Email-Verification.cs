using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using ActiveUp.Net.Mail;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace FTTests.Portal.Email.Verification {
    [TestFixture, DoesNotRunInJenkins]
    [Category(TestCategories.Services.PaymentProcessor)]
    public class Portal_AddSubmission_EventRegistration : FixtureBaseWebDriver
    {

        [FixtureSetUp]
        public void FixtureSetUp()
        {

            try
            {
                string password = "ActiveQA12";
                base.DeleteAllEmails.Delete_All_Email("F1AutomatedTester01@gmail.com", password, EmailMailBox.GMAIL.INBOX);
                base.DeleteAllEmails.Delete_All_Email("F1AutomatedTester01@gmail.com", password, EmailMailBox.GMAIL.SPAM);
                base.DeleteAllEmails.Delete_All_Email("F1AutomatedTester02@gmail.com", password, EmailMailBox.GMAIL.INBOX);
                base.DeleteAllEmails.Delete_All_Email("F1AutomatedTester02@gmail.com", password, EmailMailBox.GMAIL.SPAM);
                base.DeleteAllEmails.Delete_All_Email("ft.autotester@gmail.com", "Romans10:9", EmailMailBox.GMAIL.INBOX);
                base.DeleteAllEmails.Delete_All_Email("ft.autotester@gmail.com", "Romans10:9", EmailMailBox.GMAIL.SPAM);
                base.DeleteAllEmails.Delete_All_Email("ft.kevinspacey@gmail.com", "FT4life!", EmailMailBox.GMAIL.INBOX);
                base.DeleteAllEmails.Delete_All_Email("ft.kevinspacey@gmail.com", "FT4life!", EmailMailBox.GMAIL.SPAM);

            }
            catch (Exception e)
            {
                TestLog.WriteLine("DELETE ALL EMAIL WARN: " + e.Message);
            }

        }

        #region View Submissions

        #region Event Registration Email Confirmation

        private string Infellowship_Reigster(TestBaseWebDriver test, string[] individual, System.Collections.Generic.IList<string> selectedOrder,
            string payementType = "Visa", bool sendEmail = false, string confirmEmail = "", string ccEmail = "")
        {

            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]);

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            if (test.Driver.FindElementById("send_confirmation_email").GetAttribute("checked").Equals("false"))
            {
                test.Driver.FindElementById("send_confirmation_email").Click();
                test.GeneralMethods.WaitForElementVisible(By.Id("confirmation_email"));
            }
            
            // Two same ids are defined by dev in the same page.
            // Get element with xpath, modify by ivan.zhang
            test.Driver.FindElementByXPath(".//input[@id='confirmation_email']").Clear();
            test.Driver.FindElementByXPath(".//input[@id='confirmation_email']").SendKeys(confirmEmail);
            test.Driver.FindElementById("cc_email").SendKeys(ccEmail);

            if (!sendEmail)
            {
                test.Driver.FindElementById("send_confirmation_email").Click();
            }


            switch (payementType)
            {
                case "Visa":
                    test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", payementType, "4111111111111111", "12 - December", "2020");
                    break;
                case "MasterCard":
                    test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", payementType, "5555555555554444", "12 - December", "2020");
                    break;
                case "American Express":
                    test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", payementType, "378282246310005", "12 - December", "2020");
                    break;
                case "Discover":
                case "Switch":
                    // Process VISA credit card 
                    test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", payementType, "6011111111111117", "12 - December", "2020");
                    break;
                case "ECheck":
                    test.Infellowship.EventRegistration_Echeck_Process("FT", "Tester", "6302172170", "111000025", "1234567890");
                    break;
                case "Cash":
                    test.Driver.FindElementById(GeneralInFellowship.EventRegistration.Cash_Payment_Method).Click();
                    break;
                case "Check":
                    test.Driver.FindElementById(GeneralInFellowship.EventRegistration.Check_Payment_Method).Click();
                    test.GeneralMethods.WaitForElementVisible(By.Id("check_account_number"));
                    test.Driver.FindElementById("check_account_number").SendKeys("1234");
                    break;
            }

            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();
            test.GeneralMethods.WaitForElementVisible(By.LinkText("Home"));
            string confirmCode = test.Driver.FindElementByXPath(".//*[@id='main-content']/div[2]/div[1]/h4").Text.ToString().Replace("Confirmation: ", "");

            // Close the popup
            test.Driver.Close();
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[0]);

            return confirmCode;

        }

        [Test, RepeatOnFailure, Timeout(120000)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Felix Gaytan")]
        [Description("Verify Confirm eMail sent after sucessful Visa event registration payment.")]
        public void Portal_EventRegistration_ViewSubmissions_Verify_Visa_Email_Sent()
        {

            string confirmationEmailAddr = "F1AutomatedTester01@gmail.com";
            string password = "ActiveQA12";

            string individualName = "Matthew Sneeden";
            string formName = "A Test Form";
            string paymentMethod = "Visa";
            string creditCardNumber = "4111111111111111";
            string date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MMMM dd, yyyy");
            //TestLog.WriteLine(date);
            double amount = 2.00;

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            // Commented out by Mady, replaced with code below it. Weblink process is changed to Infellowship in Portal - Add submission
            // Generate a form submisssion
            // string[] confirmRefNum = test.Portal.Weblink_ViewSubmissions_Register(individualName, formName, paymentMethod, "2.00", "DC", false, false, creditCardNumber, true, confirmationEmailAddr);

            test.Portal.Weblink_ViewSubmissions_OpenRegisterWindow(individualName, formName);
            string confirmCode = Infellowship_Reigster(test, new string[] { individualName }, new List<string>() { individualName }, paymentMethod, true, confirmationEmailAddr);
            test.Portal.WebLink_ViewSubmissions_VerifyNewSubmission(formName, confirmCode, individualName, "DC", paymentMethod, "2.00", false);

            // Logout of portal
            test.Portal.LogoutWebDriver();

            //Retrieve Confirmation Email was sent
            Message email = test.Portal.Retrieve_Search_Email(confirmationEmailAddr, password, confirmCode.ToString());

            //Verify Info in Email
            test.Portal.Verify_Reg2_Confirmation_Email(email, "fellowshiponemail@activenetwork.com", confirmCode, "",
                                                          individualName, formName, amount, date, creditCardNumber);

        }

        [Test, RepeatOnFailure, Timeout(120000)]
        [Author("Felix Gaytan")]
        [Description("Verify Confirm and Copy eMail sent after sucessful Mastercard event registration payment.")]
        public void Portal_EventRegistration_ViewSubmissions_Verify_Mastercard_Email_CC_Sent()
        {

            string confirmationEmailAddr = "F1AutomatedTester01@gmail.com";
            string ccEmailAddr = "F1AutomatedTester02@gmail.com";
            string password = "ActiveQA12";

            string individualName = "Matthew Sneeden";
            string formName = "A Test Form";
            string paymentMethod = "MasterCard";
            string creditCardNumber = "5555555555554444";
            string date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MMMM dd, yyyy");

            double amount = 2.00;

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            // Commented out by Mady, replaced with code below it. Weblink process is changed to Infellowship in Portal - Add submission
            // Generate a form submisssion
            // string[] confirmRefNum = test.Portal.Weblink_ViewSubmissions_Register(individualName, formName, paymentMethod, "2.00", "DC", false, false, creditCardNumber, true, confirmationEmailAddr, ccEmailAddr);

            test.Portal.Weblink_ViewSubmissions_OpenRegisterWindow(individualName, formName);
            string confirmCode = Infellowship_Reigster(test, new string[] { individualName }, new List<string>() { individualName }, paymentMethod, true, confirmationEmailAddr, ccEmailAddr);
            test.Portal.WebLink_ViewSubmissions_VerifyNewSubmission(formName, confirmCode, individualName, "DC", paymentMethod, "2.00", false);

            // Logout of portal
            test.Portal.LogoutWebDriver();

            //Retrieve Confirmation email was delivered
            Message email = test.Portal.Retrieve_Search_Weblink_Confirmation_Email(confirmationEmailAddr, password, confirmCode);
            //Verify Confirmation Info in Email
            test.Portal.Verify_Weblink_Confirmation_Email(email, "fellowshiponemail@activenetwork.com", confirmCode, "", individualName,
                                                          formName, amount, date, creditCardNumber);

            //Retrieve Confirmation Email was sent
            //Verify CC Confirmation email was delivered
            Message email2 = test.Portal.Retrieve_Search_Weblink_Confirmation_Email(ccEmailAddr, password, confirmCode);

            //Verify Info in Email
            test.Portal.Verify_Weblink_Confirmation_Email(email2, "fellowshiponemail@activenetwork.com", confirmCode, "", individualName, 
                                                          formName, amount, date, creditCardNumber);
           
        }


        [Test, RepeatOnFailure, Timeout(120000)]
        [Author("Felix Gaytan")]
        [Description("Verify No Confirm and Copy eMail sent after sucessful AMEX event registration payment.")]
        public void Portal_EventRegistration_ViewSubmissions_Verify_AMEX_Email_CC_Not_Sent()
        {

            string confirmationEmailAddr = "F1AutomatedTester01@gmail.com";
            string ccEmailAddr = "F1AutomatedTester02@gmail.com";
            string password = "ActiveQA12";

            string individualName = "Matthew Sneeden";
            string formName = "A Test Form";
            string paymentMethod = "American Express";
            string creditCardNumber = "378282246310005";
            
            
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            // Commented out by Mady, replaced with code below it. Weblink process is changed to Infellowship in Portal - Add submission
            // Generate a form submisssion
            // string[] confirmRefNum = test.Portal.Weblink_ViewSubmissions_Register(individualName, formName, paymentMethod, "2.00", "DC", false, false, creditCardNumber, false, confirmationEmailAddr, ccEmailAddr);

            test.Portal.Weblink_ViewSubmissions_OpenRegisterWindow(individualName, formName);
            string confirmCode = Infellowship_Reigster(test, new string[] { individualName }, new List<string>() { individualName }, paymentMethod, false, confirmationEmailAddr, ccEmailAddr);
            test.Portal.WebLink_ViewSubmissions_VerifyNewSubmission(formName, confirmCode, individualName, "DC", paymentMethod, "2.00", false);

            // Logout of portal
            test.Portal.LogoutWebDriver();

            //Retrieve Confirmation Email was sent
            Message email = test.Portal.Retrieve_Search_Email(confirmationEmailAddr, password, confirmCode);

            //Retrieve Confirmation Email was sent
            Message email2 = test.Portal.Retrieve_Search_Email(ccEmailAddr, password, confirmCode);

            //Verify No Email was found
            Assert.IsNull(email, string.Format("Confirmation Email for Event Registration [{0}] was sent", confirmCode));
            Assert.IsNull(email2, string.Format("Confirmation CC Email for Event Registration [{0}] was sent", confirmCode));
            
        }

        [Test, RepeatOnFailure, Timeout(120000)]
        [Author("Felix Gaytan")]
        [Description("Verify Resend eMail Receipt for AMEX event registration payment.")]
        public void Portal_EventRegistration_ViewSubmissions_Verify_Resent_Email()
        {

            string mailBox = EmailMailBox.GMAIL.INBOX;
            string confirmationEmailAddr = "F1AutomatedTester01@gmail.com";
            string ccEmailAddr = "F1AutomatedTester02@gmail.com";
            string password = "ActiveQA12";

            string individualName = "Matthew Sneeden";
            string formName = "A Test Form";
            string paymentMethod = "American Express";
            string creditCardNumber = "378282246310005";
            string date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("M/d/yyyy");
            double amount = 2.00;

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            // Commented out by Mady, replaced with code below it. Weblink process is changed to Infellowship in Portal - Add submission
            // Generate a form submisssion
            // string[] confirmRefNum = test.Portal.Weblink_ViewSubmissions_Register(individualName, formName, paymentMethod, "2.00", "DC", false, false, creditCardNumber, false, confirmationEmailAddr, ccEmailAddr);

            test.Portal.Weblink_ViewSubmissions_OpenRegisterWindow(individualName, formName);
            string confirmCode = Infellowship_Reigster(test, new string[] { individualName }, new List<string>() { individualName }, paymentMethod, false, confirmationEmailAddr, ccEmailAddr);
            test.Portal.WebLink_ViewSubmissions_VerifyNewSubmission(formName, confirmCode, individualName, "DC", paymentMethod, "2.00", false);

            string referenceNumber = test.SQL.WebLink_FetchEventRegistrationReference(15, amount, paymentMethod, confirmCode);

            //Send e-mail through view submission
            test.Portal.Weblink_ViewSubmissions_ProcessEmailReceipt(confirmCode, referenceNumber, individualName, date,
                                               confirmationEmailAddr);

            // Logout of portal
            test.Portal.LogoutWebDriver();

            //Retrieve Confirmation Email was sent
            MessageCollection msgs = test.Portal.Retrieve_Email(mailBox, EmailMailBox.SearchParse.ALL, confirmationEmailAddr, password);

            //We should only have one e-mail
            int msgCount = test.Portal.Count_Weblink_Confirmation_Email(msgs, confirmCode);

            if (msgCount == 0)
            {
                //Go to SPAM
                mailBox = EmailMailBox.GMAIL.SPAM;
                msgs = test.Portal.Retrieve_Email(mailBox, EmailMailBox.SearchParse.ALL, confirmationEmailAddr, password);
                msgCount = test.Portal.Count_Weblink_Confirmation_Email(msgs, confirmCode);

            }

            Assert.AreEqual(msgCount, 1, string.Format("More than/Less Than one [{0}] Confirmation Email for Event Registration [{1}] was sent", msgs.Count, confirmCode));

            //Verify Confirmation email was delivered
            Message email = test.Portal.Search_Weblink_Confirmation_Email(msgs, confirmCode);

            //Verify Info in Email
            test.Portal.Verify_Weblink_Confirmation_Email(email, "fellowshiponemail@activenetwork.com", confirmCode, referenceNumber,
                                                          individualName, formName, amount, date, creditCardNumber);


            //Verify CC Confirmation email was not delivered
            Message email2 = test.Portal.Retrieve_Search_Email(ccEmailAddr, password, confirmCode);

            //Verify No CC Email was found
            Assert.IsNull(email2, string.Format("Confirmation CC Email for Event Registration [{0}] was sent", confirmCode));


        }

        #endregion Event Registration Email Confirmation

        #endregion View Submissions
    }

    [TestFixture, DoesNotRunInJenkins]
    public class Portal_Home_Login_Failure : FixtureBaseWebDriver
    {
        [FixtureSetUp]
        public void FixtureSetUp()
        {

            try
            {
                string password = "Romans10:9";
                base.DeleteAllEmails.Delete_All_Email("ft.autotester@gmail.com", password, EmailMailBox.GMAIL.INBOX);
                base.DeleteAllEmails.Delete_All_Email("ft.autotester@gmail.com", password, EmailMailBox.GMAIL.SPAM);

            }
            catch (Exception e)
            {
                TestLog.WriteLine("DELETE ALL EMAIL WARN: " + e.Message);
            }

        }

        #region Login Fail Email

        // This case now fails on LV_QA env since no email is triggered after account is locked, but works well on Staging
        [Test, RepeatOnFailure, Timeout(8000)]
        [Author("Stuart Platt")]
        [Description("Verifies that when a user fails to login and the account is locked that an email is recieved")]
        public void Home_User_Login_Fail_LockedAccountEmail()
        {
            string username = "lockedemail";
            string wrongPassword = "WrongPassword";
            string churchCode = "dc";
            string email = "ft.autotester@gmail.com";
            string emailPassword = "Romans10:9";
            string _fromAddr = "fellowshiponemail@activenetwork.com";
            string _replyTo = "no-reply@infellowship.com";
            string[] _recepients = new string[] { "Locked Email" };
            int churchId = 15;
            string emailBody = "Your Fellowship One Portal account has been locked due to multiple unsuccessful attempts at logging in. Please wait 15 minutes before trying again, or contact your F1 Champion to reset your password.";
            
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.SQL.Admin_Set_User_RetryCount(churchId, username, 0);

            //Lock Account
            test.Portal.LoginFailMaxAttemptsWebdriver(username, wrongPassword, churchCode);
            
            
            string emailSubject = test.Portal.getEmailSubjectTwo("Account Locked Out");
            //Verify Email recieved
            Message lockMsg = test.Portal.Retrieve_Search_Email(email, emailPassword, emailBody);
            test.Portal.Verify_Sent_Email(lockMsg, emailSubject, _fromAddr, _replyTo, _recepients, emailBody);

        }

        #endregion Login Fail Email

    }

    [TestFixture, DoesNotRunInJenkins]
    public class Portal_People_GroupEmail : FixtureBase
    {

        #region Private Members
        private string _replyTo = "F1AutomatedTester01@gmail.com";
        #endregion Private Members

        [FixtureSetUp]
        public void FixtureSetUp()
        {

            //using (OpenPop.Pop3.Pop3Client client = new OpenPop.Pop3.Pop3Client()) {
            //    client.Connect("pop.gmail.com", 995, true);
            //    client.Authenticate("F1AutomatedTester01@gmail.com", "ActiveQA");
            //    client.DeleteMessage(1);
            //    client.DeleteAllMessages();
            //    client.Disconnect();
            //}

            //DAPP01 F:\FileSharedev, FileShareqa, \EmailTemplates\Fileshare\15

            try
            {
                string password = "ActiveQA12";
                base.DeleteAllEmails.Delete_All_Email("F1AutomatedTester01@gmail.com", password, EmailMailBox.GMAIL.INBOX);
                base.DeleteAllEmails.Delete_All_Email("F1AutomatedTester01@gmail.com", password, EmailMailBox.GMAIL.SPAM);
                base.DeleteAllEmails.Delete_All_Email("F1AutomatedTester02@gmail.com", password, EmailMailBox.GMAIL.INBOX);
                base.DeleteAllEmails.Delete_All_Email("F1AutomatedTester02@gmail.com", password, EmailMailBox.GMAIL.SPAM);
                base.DeleteAllEmails.Delete_All_Email("ft.autotester@gmail.com", "Romans10:9", EmailMailBox.GMAIL.INBOX);
                base.DeleteAllEmails.Delete_All_Email("ft.autotester@gmail.com", "Romans10:9", EmailMailBox.GMAIL.SPAM);
                base.DeleteAllEmails.Delete_All_Email("ft.kevinspacey@gmail.com", "FT4life!", EmailMailBox.GMAIL.INBOX);
                base.DeleteAllEmails.Delete_All_Email("ft.kevinspacey@gmail.com", "FT4life!", EmailMailBox.GMAIL.SPAM);

            }
            catch (Exception e)
            {
                TestLog.WriteLine("DELETE ALL EMAIL WARN: " + e.Message);
            }

        }

        [Test, RepeatOnFailure(Order = 1), Timeout(8000)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Stuart Platt")]
        [Description("Sends a group email via Portal.")]
        public void People_GroupEmail_Compose_Send()
        {
            //Set conditions
            string emailAddr = "F1AutomatedTester01@gmail.com";
            string password = "ActiveQA12";
            string _fromConfirmAddr = "fellowshiponemail@activenetwork.com";
            string _infoEmail = "info@fellowshiponemail.com";
            string _fromAddr = "\"FellowshipOne AutomatedTester01\" <" + _fromConfirmAddr + ">";
            string _replyTo = emailAddr;
            string _confirmAddr = _replyTo;
            string[] _recepients = new string[] { "FellowshipOne AutomatedTester01" };

            // Login to portal

            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            test.Portal.Login("autotester01", "FT4life!", "dc");

            string emailRand = Guid.NewGuid().ToString().Substring(0, 7);
            string emailBody = string.Format("This is a test email sent from Portal {0}", emailRand);

            // Send an email to the designated recipient(s)  
            string emailTimeStamp = test.Portal.People_GroupEmail_Compose("Test email from Portal", null, _replyTo, _recepients, emailBody, false, emailAddr, false);

            // Logout of portal
            test.Portal.Logout();

            TestLog.WriteLine("Enivornment: " + this.F1Environment.ToString());

            //Verify confirmation email was sent            
            string emailSubject = test.Portal.getEmailSubject("MTA TEST LICENSE - Your Email has been sent");
            string emailSubject2 = test.Portal.getEmailSubject("Test email from Portal");
            string confirmEmailBody = test.Portal.getConfirmEmailBody("FellowshipOne", "AutomatedTester01", "FellowshipOne", "AutomatedTester01", _infoEmail, emailTimeStamp, "Test email from Portal", false);

            Message msg = test.Portal.Retrieve_Search_Email(emailAddr, password, confirmEmailBody, true, emailSubject2);
            test.Portal.Verify_Confirmation_Email(msg, "Your email has been sent", _fromConfirmAddr, _fromConfirmAddr, _recepients, confirmEmailBody, emailSubject2);

            //Verify Group email was delivered
            Message grpMsg = test.Portal.Retrieve_Search_Email(emailAddr, password, emailBody);
            test.Portal.Verify_Sent_Email(grpMsg, emailSubject2, _fromAddr, _replyTo, _recepients, emailBody);

        }

        [Test, RepeatOnFailure(Order = 2), Timeout(8000)]
        [Author("Stuart Platt")]
        [Description("Sends a group email via Portal.")]
        public void People_GroupEmail_Compose_Send_MultipleRecipients()
        {

            //Set conditions
            string name1 = "FellowshipOne AutomatedTester01";
            string name2 = "FellowshipOne AutomatedTester02";
            string emailAddr1 = "fellowshiponemail@activenetwork.com";
            string emailAddr2 = "F1AutomatedTester02@gmail.com";
            string password = "ActiveQA12";
            string _fromAddr = "\"FellowshipOne AutomatedTester01\" <" + emailAddr1 + ">";
            string[] _recepients1 = new string[] { "FellowshipOne AutomatedTester01" };
            string[] _recepients2 = new string[] { "FellowshipOne AutomatedTester02" };

            MessageCollection grpMsg_1;
            MessageCollection grpMsg_2;
            Message email_1;
            Message email_2;

            string emailRand = Guid.NewGuid().ToString().Substring(0, 7);
            string emailBody = string.Format("This is a test email sent from Portal to multiple recipients. {0}", emailRand);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("autotester01", "FT4life!", "dc");

            // Send an email to the designated recipient(s)  
            string emailTimeStamp = test.Portal.People_GroupEmail_Compose("Test email from Portal - Multiple Recipients", null, _replyTo, new string[] { name1, name2 }, emailBody, false, null, false);

            // Logout of portal
            test.Portal.Logout();

            string mailBox = EmailMailBox.GMAIL.INBOX;

            //Verify email 1 was sent
            string emailSubject = test.Portal.getEmailSubject("Test email from Portal - Multiple Recipients");

            //Verify Group email 1 was delivered
            Message grpMsg = test.Portal.Retrieve_Search_Email(emailAddr2, password, emailBody);
            test.Portal.Verify_Sent_Email(grpMsg, emailSubject, _fromAddr, _replyTo, _recepients1, emailBody);


            //Verify email 2 was sent            
            string emailSubject2 = test.Portal.getEmailSubject("Test email from Portal - Multiple Recipients");

            //Verify Group email 2 was delivered
            Message grpMsg2 = test.Portal.Retrieve_Search_Email(emailAddr2, password, emailBody);
            email_2 = test.Portal.Verify_Sent_Email(grpMsg2, emailSubject2, _fromAddr,
                _replyTo, _recepients2, emailBody);

        }

        [Test, RepeatOnFailure(Order = 3), Timeout(8000)]
        [Author("Stuart Platt")]
        [Description("Sends a group email via Portal containing HTML in the body.")]
        public void People_GroupEmail_Compose_SendHTMLInBody()
        {
            //Set conditions
            string emailAddr = "F1AutomatedTester01@gmail.com";
            string password = "ActiveQA12";
            string _replyTo = "fellowshiponemail@activenetwork.com";
            string _fromAddr = "\"FellowshipOne AutomatedTester01\" <" + _replyTo + ">";
            string[] _recepients = new string[] { "FellowshipOne AutomatedTester01" };

            string emailRand = Guid.NewGuid().ToString().Substring(0, 7);
            string emailBody = string.Format("<p>This is a test email sent from Portal<br /><strong>This text should be BOLD!!!</strong><br /><span style=\"color: red;\">This text should be RED {0}!!!</span></p>", emailRand);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("autotester01", "FT4life!", "dc");

            // Send an email to the designated recipient(s)
            string emailTimeStamp = test.Portal.People_GroupEmail_Compose("Test email from Portal containing HTML", null, emailAddr, _recepients, emailBody, false, null, false);

            // Logout of portal
            test.Portal.Logout();

            //Verify Group Email was sent

            string emailSubject = test.Portal.getEmailSubject("Test email from Portal containing HTML");

            //Verify group email was delivered
            Message grpMsg = test.Portal.Retrieve_Search_Email(emailAddr, password, emailBody);
            test.Portal.Verify_Sent_Email(grpMsg, emailSubject, _fromAddr, _replyTo, _recepients, emailBody);

        }

        [Test, RepeatOnFailure(Order = 5), Timeout(8000)]
        [Author("Stuart Platt")]
        [Description("Sends a test email via Portal with no recipient.")]
        public void People_GroupEmail_Compose_SendTestEmail_NoRecipient()
        {
            //Set conditions
            string emailAddr = "F1AutomatedTester01@gmail.com";
            string _fromAddr;
            if (base.F1Environment == F1Environments.LV_QA)
            {
                //modify by grace zhang
                _fromAddr = "\"FellowshipOne AutomatedTester01\" <" + "fellowshiponemail@activenetwork.com" + ">";
            }
            else
            {
                _fromAddr = "\"FellowshipOne AutomatedTester01\" <" + "fellowshiponemail@activenetwork.com" + ">";
            }
            string password = "ActiveQA12";
            string _replyTo = emailAddr;
            string[] _recepients = new string[] { "FellowshipOne AutomatedTester01" };

            string emailRand = Guid.NewGuid().ToString().Substring(0, 7);
            string emailBody = string.Format("This is a test, test email sent from Portal {0}", emailRand);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("autotester01", "FT4life!", "dc");

            // Send a test email with no recipients
            string emailTimeStamp = test.Portal.People_GroupEmail_Compose("Test email from Portal - Test w/o Recipient", null, _replyTo, null, emailBody, true, null, false);

            // Logout of portal
            test.Portal.Logout();

            //Verify No Test email was sent
            string emailSubject = test.Portal.getEmailSubject("Test email from Portal - Test w/o Recipient");

            //Verify Group email was delivered
            Message grpMsg = test.Portal.Retrieve_Search_Email(emailAddr, password, emailBody);
            test.Portal.Verify_Sent_Email(grpMsg, emailSubject, _fromAddr, _replyTo, _recepients, emailBody);

        }

        // This case now fails on LV_QA env since no email is triggered after account is locked, but works well on Staging
        //Jim added test attribute "DoesNotRunOutsideOfStaging"
        [Test, RepeatOnFailure, DoesNotRunOutsideOfStaging, Timeout(10000)]
        [Author("David Martin")]
        [Description("Sends an email on behalf of another portal user via a Delegate.")]
        public void People_GroupEmail_Compose_SendViaDelegate()
        {

            string emailAddr = "ft.autotester@gmail.com";
            string password = "Romans10:9";
            string confirmAddr = "fellowshiponemail@activenetwork.com";

            string emailRand = Guid.NewGuid().ToString().Substring(0, 7);
            string emailBody = string.Format("This is a test email sent from Portal - Delegate {0}", emailRand);

            // Login to portal using delegates credentials
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            TestLog.WriteLine("Add Delegate");
            test.Portal.Login("kevin.spacey", "FT4life!", "dc");

            //Delete all same delegate
            test.Portal.People_GroupEmail_Delegates_Delete_All("Tester, FT");

            // Add a delegate
            test.Portal.People_GroupEmail_Delegates_Create("Tester, FT");

            test.Portal.Logout();
            TestLog.WriteLine("Add Delegate Complete");

            TestLog.WriteLine("Send Delegate Email");
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Compose an email via a delegate
            string emailTimeStamp = test.Portal.People_GroupEmail_Compose("Test email from Portal - Delegate", "Kevin Spacey", "ft.kevinspacey@gmail.com", new string[] { "FT Tester" }, emailBody, false, null, false);
            TestLog.WriteLine("Delegate Email sent at " + emailTimeStamp);

            // Logout of portal
            test.Portal.Logout();

            //Verify confirmation email was sent
            string emailSubject = test.Portal.getEmailSubject("Test email from Portal - Delegate");
            TestLog.WriteLine("Email Verify Subject: " + emailSubject);

            Message grpMsg = test.Portal.Retrieve_Search_Email(emailAddr, password, emailBody);
            test.Portal.Verify_Sent_Email(grpMsg, emailSubject, "\"Kevin Spacey\" <" + confirmAddr + ">", confirmAddr, new string[] { "FT Tester" }, emailBody);

        }

        [Test, RepeatOnFailure(Order = 4), Timeout(8000)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Stuart Platt")]
        [Description("Sends a test email via Portal with one recipient.")]
        public void People_GroupEmail_Compose_SendTestEmail_Recipient()
        {
            //Set conditions
            string emailAddr = "F1AutomatedTester01@gmail.com";
            string _fromAddr;
            if (base.F1Environment == F1Environments.LV_QA)
            {
                //modify by grace zhang
                _fromAddr = "\"FellowshipOne AutomatedTester01\" <" + "fellowshiponemail@activenetwork.com" + ">";
            }
            else
            {
                _fromAddr = "\"FellowshipOne AutomatedTester01\" <" + "fellowshiponemail@activenetwork.com" + ">";
            }
            string password = "ActiveQA12";

            string _replyTo = emailAddr;
            string[] _recepients = new string[] { "FellowshipOne AutomatedTester01" };

            string emailRand = Guid.NewGuid().ToString().Substring(0, 7);
            string emailBody = string.Format("This is a test, test email sent from Portal {0}", emailRand);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("autotester01", "FT4life!", "dc");

            // Send a test email to the designated recipient(s)
            string emailTimeStamp = test.Portal.People_GroupEmail_Compose("Test email from Portal - Test w/Recipient", null, _replyTo, _recepients, emailBody, true, null, false);

            // Logout of portal
            test.Portal.Logout();

            //Verify Test email was sent
            string emailSubject = test.Portal.getEmailSubject("Test email from Portal - Test w/Recipient");

            //Verify Group email was delivered
            Message grpMsg = test.Portal.Retrieve_Search_Email(emailAddr, password, emailBody);
            test.Portal.Verify_Sent_Email(grpMsg, emailSubject, _fromAddr, _replyTo, _recepients, emailBody);

        }

        [Test, RepeatOnFailure(Order = 6), Timeout(8000)]
        [Author("Stuart Platt")]
        [Description("Sends a test email via Portal generating a closed contact item.")]
        public void People_GroupEmail_Compose_SendEmailContactItem()
        {
            //Set conditions
            string emailAddr = "F1AutomatedTester01@gmail.com";
            string password = "ActiveQA12";
            string _fromConfirmAddr = "fellowshiponemail@activenetwork.com";
            string _infoEmail = "info@fellowshiponemail.com";
            string _fromAddr = "\"FellowshipOne AutomatedTester01\" <" + _fromConfirmAddr + ">";
            string _replyTo = emailAddr;
            string _confirmAddr = _replyTo;
            string[] _recepients = new string[] { "FellowshipOne AutomatedTester01" };

            string emailRand = Guid.NewGuid().ToString().Substring(0, 7);
            string emailBody = string.Format("This is a test email sent from Portal {0}", emailRand);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("autotester01", "FT4life!", "dc");

            // Send a test email and generate a closed contact item
            string emailTimeStamp = test.Portal.People_GroupEmail_Compose("Test email from Portal - Closed Contact Item", null, _replyTo, _recepients, emailBody, false, _confirmAddr, true);

            // Logout of portal
            test.Portal.Logout();

            //Set Mailbox retrieve settings

            //Verify confirmation email was sent            
            string emailSubject = test.Portal.getEmailSubject("MTA TEST LICENSE - Your Email has been sent");
            string emailSubject2 = test.Portal.getEmailSubject("Test email from Portal - Closed Contact Item");
            string confirmEmailBody = test.Portal.getConfirmEmailBody("FellowshipOne", "AutomatedTester01", "FellowshipOne", "AutomatedTester01", _infoEmail, emailTimeStamp, "Test email from Portal - Closed Contact Item", true);

            Message confirmMsg = test.Portal.Retrieve_Search_Email(emailAddr, password, confirmEmailBody, true, emailSubject2);
            test.Portal.Verify_Confirmation_Email(confirmMsg, "Your email has been sent", _fromConfirmAddr, _fromConfirmAddr, _recepients, confirmEmailBody, emailSubject2);

            //Verify Group email was delivered
            Message grpMsg = test.Portal.Retrieve_Search_Email(emailAddr, password, emailBody);
            test.Portal.Verify_Sent_Email(grpMsg, emailSubject2, _fromAddr, _replyTo, _recepients, emailBody);

        }

    }
}