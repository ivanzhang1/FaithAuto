using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using ActiveUp.Net.Mail;
using OpenQA.Selenium;
using Common.PageEntitys;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Collections.Generic;

namespace FTTests.Portal.WebLink {
    [TestFixture]
    [Category(TestCategories.Services.PaymentProcessor)]
    public class Portal_WebLink_EventRegistration : FixtureBaseWebDriver
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
                base.DeleteAllEmails.Delete_All_Email("ft.kevinspacey@gmail.com", "Romans10:9", EmailMailBox.GMAIL.INBOX);
                base.DeleteAllEmails.Delete_All_Email("ft.kevinspacey@gmail.com", "Romans10:9", EmailMailBox.GMAIL.SPAM);

            }
            catch (Exception e)
            {
                TestLog.WriteLine("DELETE ALL EMAIL WARN: " + e.Message);
            }

        }

        #region Form Builder
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Creates a Form Name.")]      
        public void WebLink_EventRegistration_FormNames_Create()
        {
            // Set initial conditions
            string formName = "Y-Test Form_CreateAndDelete";            

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "DC");

            // Create a form name
            test.Portal.WebLink_FormNames_Create(formName, true);

            // Delete Form
            test.Portal.WebLink_FormNames_Delete(formName);

            // Logout of portal
            test.Portal.LogoutWebDriver();


        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Updates a Form Name.")]
        public void WebLink_EventRegistration_FormNames_Update()
        {
            // Set initial conditions
            string formName = "Test Form - PUpdate";
            string formNameUpdated = "Test Form - PUpdated";
            base.SQL.WebLink_FormNames_Delete(1, 15, formName);
            base.SQL.WebLink_FormNames_Delete(1, 15, formNameUpdated);
            base.SQL.WebLink_FormNames_Create(1, 15, formName, true);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "DC");

            // Update a form name
            test.Portal.WebLink_FormNames_Update(formName, true, formNameUpdated, true);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Deletes a Form Name.")]
        public void WebLink_EventRegistration_FormNames_Delete()
        {
            // Set initial conditions
            string formName = "Y-Test Form - Delete";
            base.SQL.WebLink_FormNames_Delete(1, 15, formName);
            base.SQL.WebLink_FormNames_Create(1, 15, formName, true);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "DC");

            // Delete a form name
            test.Portal.WebLink_FormNames_Delete(formName);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verify EU Text Warning on Questions")]
        public void Weblink_EventRegistration_ManageForms_Questions_VerifyEUText()
        {
            //Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("splatt", "Romans10:9", "DC");

            //Navigate to Event Registration Manage Forms 
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.WebLink.Event_Registration.Manage_Forms);
            TestLog.WriteLine("Navigate to Forms");
            
            //Click on a form
            test.Driver.FindElementByLinkText("*EU Text Form").Click();
            TestLog.WriteLine("Click on a form");

            //Click on the Questions and Answers link
            test.Driver.FindElementByLinkText("Questions and answers").Click();
            TestLog.WriteLine("Go to Questions and Answers");
            
            // Verify EU Text
            test.GeneralMethods.WaitForPageIsLoaded();
            //test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Preview"));
            test.GeneralMethods.VerifyTextPresentWebDriver("If these questions are being asked of citizens residing in the European Union you agree not to use the Products or Services to collect or elicit any special categories of data (as defined in the European Union Data Protection Directive, as may be amended from time to time), including, but not limited to data revealing racial or ethnic origin, political opinions, or religious or other beliefs, trade-union membership, as well as personal data concerning health or sexual life or criminal convictions.");
            TestLog.WriteLine("Verify EU Text");

            //Click on Add a new question
            test.Driver.FindElementByLinkText("Add new question").Click();
            TestLog.WriteLine("Go to Add Question Page");

            // Verify EU text present
            test.GeneralMethods.VerifyTextPresentWebDriver("If these questions are being asked of citizens residing in the European Union you agree not to use the Products or Services to collect or elicit any special categories of data (as defined in the European Union Data Protection Directive, as may be amended from time to time), including, but not limited to data revealing racial or ethnic origin, political opinions, or religious or other beliefs, trade-union membership, as well as personal data concerning health or sexual life or criminal convictions.");
            TestLog.WriteLine("Verify EU Text 2nd page");

            //Logout
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Emily zhang")]
        [Description("Verify the InFellowship Registration Only Text")]
        public void Weblink_EventRegistration_ManageForms_VerifyText_InFellowshipRegistrationOnly()
        {

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "DC");

            //Navigate to Event Registration Manage Forms 
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.WebLink.Event_Registration.Manage_Forms);
            TestLog.WriteLine("Navigate to Forms");

            //Click on a form
            test.Driver.FindElementByLinkText("*EU Text Form").Click();
            TestLog.WriteLine("Click on a form");

            //Click on the Edit Presentation link
            test.Driver.FindElementByLinkText("Edit Presentation").Click();
            TestLog.WriteLine("Go to the Edit Presentation page");

            // Verify the Edit Presentation Text
            test.GeneralMethods.WaitForPageIsLoaded();
            test.GeneralMethods.VerifyTextPresentWebDriver("Edit Presentation (InFellowship Registration only)");
            TestLog.WriteLine("Verify the Edit Presentation Text");

            //Return the form page
            test.Driver.FindElementByLinkText("Back").Click();
            TestLog.WriteLine("Return the form page ");

            //Click on the Edit Social link
            test.Driver.FindElementByLinkText("Edit Social").Click();
            TestLog.WriteLine("Go to the Edit Social page");

            // Verify the Edit Social Text
            test.GeneralMethods.WaitForPageIsLoaded();
            test.GeneralMethods.VerifyTextPresentWebDriver("Edit Social Media Message (InFellowship Registration only)");
            TestLog.WriteLine("Verify the Edit Social Text");

            //Return the form page
            test.Driver.FindElementByLinkText("Back").Click();
            TestLog.WriteLine("Return the form page ");

            //Click on the Questions and Answers link
            test.Driver.FindElementByLinkText("Questions and answers").Click();
            TestLog.WriteLine("Go to Questions and Answers page");

            //Click on Add new header
            test.Driver.FindElementByLinkText("Add new header").Click();
            TestLog.WriteLine("Go to Add new header Page");

            // Verify the share box text present
            test.GeneralMethods.VerifyTextPresentWebDriver("All questions under this header are only asked once for the entire family (InFellowship Registration only");


            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Alan Luo")]
        [Description("Verify Edit Presentation function: SaveSettings-HasFriendlyName")]
        public void Weblink_EventRegistration_ManageForms_EditPresentation_HasFriendlyName()
        {
            /*
             * FO-1740, FO-1728, FO-1741
             * Changed by F0-3516
             */

            string userName = "splatt";
            string password="Romans10:9";
            string churchCode = "DC";
            int churchId = 15;
            string formName = "EditPresentationHasFriendlyName - Test Form";

            string friendlyName = "Test-FriendlyName";
            string description = "Test-Description";

            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                //01. Create From in portal.
                //Login to portal            
                test.Portal.LoginWebDriver(userName, password, churchCode);

                test.Portal.WebLink_FormNames_Create(formName, true);
                TestLog.WriteLine("Create a form to DB, formName: {0};", formName);

                #region test in portal.
                //Save Setting 
                FormPresentationPage formPresentationPage = new FormPresentationPage(test, formName);
                formPresentationPage.SaveSettings(friendlyName, description);
                //formPresentationPage.ValidationSaveSettings(friendlyName, description);

                Assert.AreEqual(friendlyName, test.Driver.FindElement(By.Id("ctl00_ctl00_MainContent_content_lblPresentationName")).Text, "Name display failed!");
                Assert.AreEqual(description, test.Driver.FindElement(By.Id("ctl00_ctl00_MainContent_content_lblPresentationDescription")).Text, "Description display failed!");

                //Cache Data;
                string fromUrl = formPresentationPage.LinkUrl;

                //Logout
                test.Portal.LogoutWebDriver();
                #endregion test in portal.

                #region test not login
                test.Driver.Url = fromUrl;
                var lblFriendlyName = test.Driver.FindElement(By.XPath(".//*[@id='main-content']/form/div[@class='form-group']/label[@class='big' and @for='title']"));
                var lblDescription = test.Driver.FindElement(By.XPath(".//*[@id='main-content']/form/div[@class='form-group']/p[@class='wordbreak']"));

                Assert.AreEqual(friendlyName, lblFriendlyName.Text, "[infellowship-no login]: 'friendly Name' update error!");
                Assert.AreEqual(description, lblDescription.Text, "[infellowship-no login]: 'description' update error!");
                #endregion

                #region test login
                test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "DC");
                test.Driver.Url = fromUrl;

                lblFriendlyName = test.Driver.FindElement(By.XPath(".//*[@id='main-content']//*/h3[@class='form_friendly_name']"));
                lblDescription = test.Driver.FindElement(By.XPath(".//*[@id='page_header']/p"));

                Assert.AreEqual(friendlyName, lblFriendlyName.Text, "[infellowship-login]: 'friendly Name' update error!");
                Assert.AreEqual(description, lblDescription.Text, "[infellowship-login]: 'description' update error!");
                #endregion
            }
            finally
            {
                TestLog.WriteLine("Clean up the form from DB, name: " + formName);
                test.SQL.Weblink_Form_Delete_Proc(churchId, formName);
            }
        }

        [Test, RepeatOnFailure]
        [Author("Alan Luo")]
        [Description("Verify Edit Presentation function: SaveSettings-NoFriendlyName")]
        public void Weblink_EventRegistration_ManageForms_EditPresentation_NoFriendlyName()
        {
            /*
             * FO-1740, FO-1728, FO-1741
             * Changed by F0-3516
             */

            string userName = "splatt";
            string password = "Romans10:9";
            string churchCode = "DC";
            int churchId = 15;
            string formName = "EditPresentationNoFriendlyName - Test Form";

            string description = "Test-Description";

            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                //01. Create From in portal.
                //Login to portal            
                test.Portal.LoginWebDriver(userName, password, churchCode);

                test.Portal.WebLink_FormNames_Create(formName, true);
                TestLog.WriteLine("Create a form to DB, formName: {0};", formName);

                #region test in portal.
                //Save Setting 
                FormPresentationPage formPresentationPage = new FormPresentationPage(test, formName);
                formPresentationPage.SaveSettings("", description);
                //formPresentationPage.ValidationSaveSettings(formName, description);
                Assert.AreEqual(formName, test.Driver.FindElement(By.Id("ctl00_ctl00_MainContent_content_lblPresentationName")).Text, "Name display failed!");
                Assert.AreEqual(description, test.Driver.FindElement(By.Id("ctl00_ctl00_MainContent_content_lblPresentationDescription")).Text, "Description display failed!");

                //Cache Data;
                string fromUrl = formPresentationPage.LinkUrl;

                //Logout
                test.Portal.LogoutWebDriver();
                #endregion test in portal.

                #region test not login
                test.Driver.Url = fromUrl;
                var lblFriendlyName = test.Driver.FindElement(By.XPath(".//*[@id='main-content']/form/div[@class='form-group']/label[@class='big' and @for='title']"));
                var lblDescription = test.Driver.FindElement(By.XPath(".//*[@id='main-content']/form/div[@class='form-group']/p[@class='wordbreak']"));
                

                Assert.AreEqual(formName, lblFriendlyName.Text, "[infellowship-no login]: 'friendly Name' update error!");
                Assert.AreEqual(description, lblDescription.Text, "[infellowship-no login]: 'description' update error!");
                #endregion

                #region test login
                test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "DC");
                test.Driver.Url = fromUrl;

                lblFriendlyName = test.Driver.FindElement(By.XPath(".//*[@id='main-content']//*/h3[@class='form_friendly_name']"));
                lblDescription = test.Driver.FindElement(By.XPath(".//*[@id='page_header']/p"));

                Assert.AreEqual(formName, lblFriendlyName.Text, "[infellowship-login]: 'friendly Name' update error!");
                Assert.AreEqual(description, lblDescription.Text, "[infellowship-login]: 'description' update error!");
                #endregion
            }
            finally
            {
                TestLog.WriteLine("Clean up the form from DB, name: " + formName);
                test.SQL.Weblink_Form_Delete_Proc(churchId, formName);
            }
        }
        #endregion Form Builder

        #region Infellowship Registration

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies the new Infellowship registration link")]
        public void Weblink_EventRegistration_ManageForms_InfellowshipRegistration_Link()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string formName = "A Test Form";          
  
            //Login
            test.Portal.LoginWebDriver();

            //Navigate to Weblink forms
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.WebLink.Event_Registration.Manage_Forms);
            test.Driver.FindElementByLinkText("A").Click();
            test.Driver.FindElementByLinkText(formName).Click();
            test.GeneralMethods.WaitForElement(By.Id("ctl00_ctl00_MainContent_content_infellowshipUrl"));

            //Verify Infellowship Registration link
            string linkText = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, 15);
            Assert.AreEqual(linkText, test.Driver.FindElementById("ctl00_ctl00_MainContent_content_infellowshipUrl").GetAttribute("value"), "Infellowship registration link not present");

            //Logout
            test.Portal.LogoutWebDriver(); 

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies the display of confirmation message setting")]
        public void Weblink_EventRegistration_ManageForms_ConfirmationMessage_Display()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string formName = "A Test Form";

            //Login
            test.Portal.LoginWebDriver();

            //Navigate to Weblink forms
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.WebLink.Event_Registration.Manage_Forms);
            // Click the first character to locate form list first.
            test.Driver.FindElementByLinkText(formName.Substring(0, 1).ToUpper()).Click();
            // int itemRow = this._generalMethods.GetTableRowNumberWebDriver(TableIds.Portal.WebLink_ManageForms, formName, "Form Name", null);
            // this._driver.FindElementById(TableIds.Portal.WebLink_ManageForms).FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[0].FindElement(By.TagName("a")).Click();
            test.Driver.FindElementByLinkText(formName).Click();

            try
            {
                test.GeneralMethods.WaitForElement(By.Id("ctl00_ctl00_MainContent_content_tblRowConfirmationMessageFrom"));
                Assert.AreEqual("Reply-to", test.Driver.FindElementByXPath("//tr[@id='ctl00_ctl00_MainContent_content_tblRowConfirmationMessageFrom']/th").Text, "");
            } catch(WebDriverException) 
            {
                test.Portal.WebLink_Form_ConfirmationMessage_Create("fakemail@active.com", "test body");
                Assert.AreEqual("Reply-to", test.Driver.FindElementByXPath("//tr[@id='ctl00_ctl00_MainContent_content_tblRowConfirmationMessageFrom']/th").Text, "");
            }
            

            //Logout
            test.Portal.LogoutWebDriver();

        }

        //Moving this test case to Infellowship
       //[Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4206 - Verifies the Form Name in  Infellowship Registration Form")]
        public void Weblink_EventRegistration_ManageForms_InfellowshipRegistration_VerifyFormName()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string formName = "A Test Form";          
  
            //Login
            test.Portal.LoginWebDriver();

            //Navigate to Weblink forms
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.WebLink.Event_Registration.Manage_Forms);
            test.Driver.FindElementByLinkText(formName).Click();
            test.GeneralMethods.WaitForElement(By.Id("ctl00_ctl00_MainContent_content_infellowshipUrl"));


            //Get Form ID
            int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

          /*  //Get Infellowship Registration link
            string linkText = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, 15);

           //Accessing Infellowhship link to form
            test.GeneralMethods.OpenURLWebDriver(linkText);
            test.Driver.FindElement(By.Id("username")).SendKeys("ft.tester@gmail.com");
            test.Driver.FindElement(By.Id("password")).SendKeys("FT4life!"); */

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName);


            //Verifying the Form Name
            Assert.AreEqual(formName, test.Driver.FindElementById("page_header").Text);


            // Logout of portal
            test.Portal.LogoutWebDriver();


        }


        #endregion Infellowship Registration

        #region Aggregate Forms
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Creates an Aggregate Form.")]
        public void WebLink_EventRegistration_AggregateForms_Create()
        {
            // Set initial conditions:
            base.SQL.WebLink_FormNames_Delete(3, 15, "Test Aggregate Form");
            base.SQL.WebLink_FormNames_Delete(1, 15, "Test Form - AF1");
            base.SQL.WebLink_FormNames_Delete(1, 15, "Test Form - AF2");
            base.SQL.WebLink_FormNames_Create(1, 15, "Test Form - AF1", true);
            base.SQL.WebLink_FormNames_Create(1, 15, "Test Form - AF2", true);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "DC");

                // Create an aggregate form
                test.Portal.WebLink_AggregateForms_Create("Test Aggregate Form", true, new string[] { "Test Form - AF1", "Test Form - AF2" });


            }
            finally
            {
                base.SQL.WebLink_FormNames_Delete(3, 15, "Test Aggregate Form");
                base.SQL.WebLink_FormNames_Delete(1, 15, "Test Form - AF1");
                base.SQL.WebLink_FormNames_Delete(1, 15, "Test Form - AF2");

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }
            
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Deletes an Aggregate Form.")]
        public void WebLink_EventRegistration_AggregateForms_Delete()
        {
            // Set initial conditions
            string aggregateFormName = "Test Aggregate Form - Delete";
            string formName1 = "Test Form - AF1D";
            string formName2 = "Test Form - AF2D";
            base.SQL.WebLink_FormNames_Delete(3, 15, aggregateFormName);
            base.SQL.WebLink_FormNames_Delete(1, 15, formName1);
            base.SQL.WebLink_FormNames_Delete(1, 15, formName2);
            base.SQL.WebLink_FormNames_Create(3, 15, aggregateFormName, true);
            base.SQL.WebLink_FormNames_Create(1, 15, formName1, true);
            base.SQL.WebLink_FormNames_Create(1, 15, formName2, true);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "DC");

                // Delete an aggregate form
                test.Portal.WebLink_AggregateForms_Delete(aggregateFormName);
            }
            finally
            {
                base.SQL.WebLink_FormNames_Delete(1, 15, formName1);
                base.SQL.WebLink_FormNames_Delete(1, 15, formName2);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }
            
        }
        #endregion Aggregate Forms

        #region View Submissions
        #region Register

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Felix Gaytan")]
        [Description("Sifter #7824 - WARRANTY: Can't give with international address")]
        public void WebLink_EventRegistration_ViewSubmissions_Register_Check_Address_Diff_Country()
        {
            string individualName = "FT NoStProvince";
            string infellowIndividual = "Ft Nostprovince";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C6");

            // Submit a weblink form
            // test.Portal.Weblink_ViewSubmissions_Register("FT NoStProvince", "Auto Test Form", "Check", "8.19", "QAEUNLX0C6", false, true);

            test.Portal.Weblink_ViewSubmissions_OpenRegisterWindow(individualName, "Auto Test Form");
            string confirmCode = Infellowship_Reigster(test, new string[] { infellowIndividual }, new List<string>() { infellowIndividual }, "Check");
            test.Portal.WebLink_ViewSubmissions_VerifyNewSubmission("Auto Test Form", confirmCode, individualName, "QAEUNLX0C6", "Check", "8.19", false);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Sifter #7756 - Cannot create ACH schedule without a verified address")]
        public void WebLink_EventRegistration_ViewSubmissions_Register_Cash_UK_No_Province()
        {
            string individualName = "FT NoStProvince";
            string infellowIndividual = "Ft Nostprovince";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C6");

            // Submit a weblink form
            // test.Portal.Weblink_ViewSubmissions_Register("FT NoStProvince", "Auto Test Form", "Cash", "8.19", "QAEUNLX0C6", false);

            test.Portal.Weblink_ViewSubmissions_OpenRegisterWindow(individualName, "Auto Test Form");
            string confirmCode = Infellowship_Reigster(test, new string[] { infellowIndividual }, new List<string>() { infellowIndividual }, "Cash");
            test.Portal.WebLink_ViewSubmissions_VerifyNewSubmission("Auto Test Form", confirmCode, individualName, "QAEUNLX0C6", "Cash", "8.19", false);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure, Timeout(120000)]
        [Author("Felix Gaytan")]
        [Description("Sifter 7736: Performs a One Time scheduled giving for a Visa card for user with no linked e-mail address")]
        public void WebLink_EventRegistration_ViewSubmissions_Register_Visa_No_Linked_Email()
        {
            string individualName = "FT NoEmail";
            string infellowIndividual = "Ft Noemail";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            // Commented out by Mady, replaced with code below it. Weblink process is changed to Infellowship in Portal - Add submission
            // Submit a weblink form
            // test.Portal.Weblink_ViewSubmissions_Register("FT NoEmail", "A Test Form", "Visa");

            test.Portal.Weblink_ViewSubmissions_OpenRegisterWindow(individualName, "A Test Form");
            string confirmCode = Infellowship_Reigster(test, new string[] { infellowIndividual }, new List<string>() { infellowIndividual }, "Visa");
            test.Portal.WebLink_ViewSubmissions_VerifyNewSubmission("A Test Form", confirmCode, individualName, "DC");

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure, Timeout(120000)]
        [Author("Felix Gaytan")]
        [Description("Sifter 7736: Performs a One Time scheduled giving for a Visa card for user with no linked phone number")]
        public void WebLink_EventRegistration_ViewSubmissions_Register_Check_No_Linked_Address_PhoneNumber()
        {
            string individualName = "FT NoEmail";
            string infellowIndividual = "Ft Noemail";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            test.Portal.Weblink_ViewSubmissions_OpenRegisterWindow(individualName, "A Test Form");
            string confirmCode = Infellowship_Reigster(test, new string[] { infellowIndividual }, new List<string>() { infellowIndividual });
            test.Portal.WebLink_ViewSubmissions_VerifyNewSubmission("A Test Form", confirmCode, individualName, "DC");

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        private string Infellowship_Reigster(TestBaseWebDriver test, string[] individual, System.Collections.Generic.IList<string> selectedOrder,
            string payementType="Visa", bool sendEmail=false, string confirmEmail="", string ccEmail="")
        {

            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]);

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");

            if (sendEmail)
            {
                if (test.Driver.FindElementById("send_confirmation_email").GetAttribute("checked").Equals("false"))
                {
                    test.Driver.FindElementById("send_confirmation_email").Click();
                    test.GeneralMethods.WaitForElementVisible(By.Id("confirmation_email"));
                }

                test.Driver.FindElementById("confirmation_email").SendKeys(confirmEmail);
                test.Driver.FindElementById("cc_email").SendKeys(ccEmail);

            }
            else
            {
                if (test.Driver.FindElementById("send_confirmation_email").GetAttribute("checked").Equals("true"))
                {
                    test.Driver.FindElementById("send_confirmation_email").Click();
                }
            }

            switch(payementType.ToLower()) {
                case "visa":
                    test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", payementType, "4111111111111111", "12 - December", "2020");
                    break;
                case "mastercard":
                    test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", payementType, "5555555555554444", "12 - December", "2020");
                    break;
                case "american express":
                    test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", payementType, "378282246310005", "12 - December", "2020");
                    break;
                case "discover":
                case "switch":
                    // Process VISA credit card 
                    test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", payementType, "6011111111111117", "12 - December", "2020");
                    break;
                case "echeck":
                    test.Infellowship.EventRegistration_Echeck_Process("FT", "Tester", "6302172170", "111000025", "1234567890");
                    break;
                case "cash":
                    test.Driver.FindElementById(GeneralInFellowship.EventRegistration.Cash_Payment_Method).Click();
                    break;
                case "check":
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
        [Author("Matthew Sneeden")]
        [Description("Submits an weblink form submission using Visa as the payment type.")]
        public void WebLink_EventRegistration_ViewSubmissions_Register_Visa()
        {
            string individualName = "Matthew Sneeden";
            string infellowIndividual = "Matthew Sneeden";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            // Commented out by Mady, replaced with code below it. Weblink process is changed to Infellowship in Portal - Add submission
            // Submit a weblink form
            // test.Portal.Weblink_ViewSubmissions_Register("Matthew Sneeden", "A Test Form", "Visa");

            test.Portal.Weblink_ViewSubmissions_OpenRegisterWindow(individualName, "A Test Form");
            string confirmCode = Infellowship_Reigster(test, new string[] { infellowIndividual }, new List<string>() { infellowIndividual }, "Visa");
            test.Portal.WebLink_ViewSubmissions_VerifyNewSubmission("A Test Form", confirmCode, individualName, "DC");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Submits an weblink form submission using eCheck as the payment type.")]
        public void WebLink_EventRegistration_ViewSubmissions_Register_eCheck()
        {
            string individualName = "Matthew Sneeden";
            string infellowIndividual = "Matthew Sneeden";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            // Commented out by Mady, replaced with code below it. Weblink process is changed to Infellowship in Portal - Add submission
            // Submit a weblink form
            // test.Portal.Weblink_ViewSubmissions_Register("Matthew Sneeden", "A Test Form", "eCheck");

            test.Portal.Weblink_ViewSubmissions_OpenRegisterWindow(individualName, "A Test Form");
            string confirmCode = Infellowship_Reigster(test, new string[] { infellowIndividual }, new List<string>() { infellowIndividual }, "eCheck");
            test.Portal.WebLink_ViewSubmissions_VerifyNewSubmission("A Test Form", confirmCode, individualName, "DC", "eCheck");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure, Timeout(120000)]
        [Author("Matthew Sneeden")]
        [Description("Submits an weblink form submission using Check as the payment type.")]
        public void WebLink_EventRegistration_ViewSubmissions_Register_Check()
        {
            string individualName = "Matthew Sneeden";
            string infellowIndividual = "Matthew Sneeden";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            // Commented out by Mady, replaced with code below it. Weblink process is changed to Infellowship in Portal - Add submission
            // Submit a weblink form
            // test.Portal.Weblink_ViewSubmissions_Register("Matthew Sneeden", "A Test Form", "Check");

            test.Portal.Weblink_ViewSubmissions_OpenRegisterWindow(individualName, "A Test Form");
            string confirmCode = Infellowship_Reigster(test, new string[] { infellowIndividual }, new List<string>() { infellowIndividual }, "Check");
            test.Portal.WebLink_ViewSubmissions_VerifyNewSubmission("A Test Form", confirmCode, individualName, "DC", "Check");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Submits an weblink form submission using Cash as the payment type.")]
        public void WebLink_EventRegistration_ViewSubmissions_Register_Cash()
        {
            string individualName = "Matthew Sneeden";
            string infellowIndividual = "Matthew Sneeden";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            // Commented out by Mady, replaced with code below it. Weblink process is changed to Infellowship in Portal - Add submission
            // Submit a weblink form
            // test.Portal.Weblink_ViewSubmissions_Register("Matthew Sneeden", "A Test Form", "Cash");

            test.Portal.Weblink_ViewSubmissions_OpenRegisterWindow(individualName, "A Test Form");
            string confirmCode = Infellowship_Reigster(test, new string[] { infellowIndividual }, new List<string>() { infellowIndividual }, "Cash");
            test.Portal.WebLink_ViewSubmissions_VerifyNewSubmission("A Test Form", confirmCode, individualName, "DC", "Cash");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Register

        #region Refund
        [Test, RepeatOnFailure, Timeout(120000)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Refunds a Visa event registration payment.")]
        public void WebLink_EventRegistration_ViewSubmissions_Refund_Visa()
        {
            string individualName = "Matthew Sneeden";
            string infellowIndividual = "Matthew Sneeden";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            // Commented out by Mady, replaced with code below it. Weblink process is changed to Infellowship in Portal - Add submission
            // Generate a form submisssion
            // string[] confirmRefNum = test.Portal.Weblink_ViewSubmissions_Register("Matthew Sneeden", "A Test Form", "Visa");

            test.Portal.Weblink_ViewSubmissions_OpenRegisterWindow(individualName, "A Test Form");
            string confirmCode = Infellowship_Reigster(test, new string[] { infellowIndividual }, new List<string>() { infellowIndividual }, "Visa");
            test.Portal.WebLink_ViewSubmissions_VerifyNewSubmission("A Test Form", confirmCode, individualName, "DC", "Visa");

            // Refund a visa submission
            //string confirmationCode = base.SQL.WebLink_FetchEventRegistrationConfirmationCode(15, 2.00, "Visa");
            test.Portal.Weblink_ViewSubmissions_ProcessAdditionalPaymentOrRefund(confirmCode, "Visa", "Refund");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Refunds an eCheck event registration payment.")]
        public void WebLink_EventRegistration_ViewSubmissions_Refund_eCheck()
        {
            string individualName = "Matthew Sneeden";
            string infellowIndividual = "Matthew Sneeden";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            // Commented out by Mady, replaced with code below it. Weblink process is changed to Infellowship in Portal - Add submission
            // Generate a form submisssion
            // string[] confirmRefNum = test.Portal.Weblink_ViewSubmissions_Register("Matthew Sneeden", "A Test Form", "eCheck");

            test.Portal.Weblink_ViewSubmissions_OpenRegisterWindow(individualName, "A Test Form");
            string confirmCode = Infellowship_Reigster(test, new string[] { infellowIndividual }, new List<string>() { infellowIndividual }, "eCheck");
            test.Portal.WebLink_ViewSubmissions_VerifyNewSubmission("A Test Form", confirmCode, individualName, "DC", "eCheck");

            // Refund an eCheck submission
            //string confirmationCode = base.SQL.WebLink_FetchEventRegistrationConfirmationCode(15, 2.00, "eCheck");
            test.Portal.Weblink_ViewSubmissions_ProcessAdditionalPaymentOrRefund(confirmCode, "eCheck", "Refund");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure, Timeout(120000)]
        [Author("Matthew Sneeden")]
        [Description("Refunds a Cash event registration payment.")]
        public void WebLink_EventRegistration_ViewSubmissions_Refund_Cash()
        {
            string individualName = "Matthew Sneeden";
            string infellowIndividual = "Matthew Sneeden";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            // Commented out by Mady, replaced with code below it. Weblink process is changed to Infellowship in Portal - Add submission
            // Generate a form submisssion
            // string[] confirmRefNum = test.Portal.Weblink_ViewSubmissions_Register("Matthew Sneeden", "A Test Form", "Cash");

            test.Portal.Weblink_ViewSubmissions_OpenRegisterWindow(individualName, "A Test Form");
            string confirmCode = Infellowship_Reigster(test, new string[] { infellowIndividual }, new List<string>() { infellowIndividual }, "Cash");
            test.Portal.WebLink_ViewSubmissions_VerifyNewSubmission("A Test Form", confirmCode, individualName, "DC", "Cash");

            // Refund a eCheck submission
            //string confirmationCode = base.SQL.WebLink_FetchEventRegistrationConfirmationCode(15, 2.00, "Cash");
            test.Portal.Weblink_ViewSubmissions_ProcessAdditionalPaymentOrRefund(confirmCode, "Cash", "Refund");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure, Timeout(120000)]
        [Author("Matthew Sneeden")]
        [Description("Refunds a Check event registration payment.")]
        public void WebLink_EventRegistration_ViewSubmissions_Refund_Check()
        {
            string individualName = "Matthew Sneeden";
            string infellowIndividual = "Matthew Sneeden";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            // Commented out by Mady, replaced with code below it. Weblink process is changed to Infellowship in Portal - Add submission
            // Generate a form submisssion
            // string[] confirmRefNum = test.Portal.Weblink_ViewSubmissions_Register("Matthew Sneeden", "A Test Form", "Check");

            test.Portal.Weblink_ViewSubmissions_OpenRegisterWindow(individualName, "A Test Form");
            string confirmCode = Infellowship_Reigster(test, new string[] { infellowIndividual }, new List<string>() { infellowIndividual }, "Check");
            test.Portal.WebLink_ViewSubmissions_VerifyNewSubmission("A Test Form", confirmCode, individualName, "DC", "Check");

            // Refund a eCheck submission
            //string confirmationCode = base.SQL.WebLink_FetchEventRegistrationConfirmationCode(15, 2.00, "Check");
            test.Portal.Weblink_ViewSubmissions_ProcessAdditionalPaymentOrRefund(confirmCode, "Check", "Refund");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Refund

        #region Add Payment
        [Test, RepeatOnFailure, Timeout(120000)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Adds a payment to an existing Visa event registration payment.")]
        public void WebLink_EventRegistration_ViewSubmissions_AddPayment_Visa()
        {
            string individualName = "Matthew Sneeden";
            string infellowIndividual = "Matthew Sneeden";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");

            // Commented out by Mady, replaced with code below it. Weblink process is changed to Infellowship in Portal - Add submission
            // Generate a form submission
            // string[] confirmRefNumRefund = test.Portal.Weblink_ViewSubmissions_Register("Matthew Sneeden", "A Test Form", "Visa");

            test.Portal.Weblink_ViewSubmissions_OpenRegisterWindow(individualName, "A Test Form");
            string confirmCode = Infellowship_Reigster(test, new string[] { infellowIndividual }, new List<string>() { infellowIndividual }, "Visa");
            test.Portal.WebLink_ViewSubmissions_VerifyNewSubmission("A Test Form", confirmCode, individualName, "DC", "Visa");

            // Refund a visa submission
            //string[] confirmRefNumRefund = base.SQL.WebLink_FetchEventRegistrationConfirmationCode(15, 2.00, "Visa");
            test.Portal.Weblink_ViewSubmissions_ProcessAdditionalPaymentOrRefund(confirmCode, "Visa", "Refund");

            // Add a payment to a visa submission
            //string[] confirmRefNumAdd = base.SQL.WebLink_FetchEventRegistrationConfirmationCode(15, 2.00, "Visa");
            test.Portal.Weblink_ViewSubmissions_ProcessAdditionalPaymentOrRefund(confirmCode, "Visa", "Add Payment");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Add Payment

        #region Event Registration Error Text Validation



        #endregion Event Registration Error Text Validation

        // Mady Kou: Commented the case out since the Add submission function on portal is changed from Weblink to Infellowship
        // [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Felix Gaytan")]
        [Description("F1-4085: Verifies credit card images are present")]
        public void WebLink_EventRegistration_Verify_CreditCard_Images()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            //Verify CC Images
            test.Portal.Weblink_ViewSubmissions_Verify_CC_Images("Felix Gaytan", "A Test Form");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        #endregion View Submissions
    }

}