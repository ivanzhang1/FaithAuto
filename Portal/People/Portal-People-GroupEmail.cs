using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using ActiveUp.Net.Mail;
using System.Security.Cryptography;
using System.IO;

namespace FTTests.Portal.People {
    [TestFixture]
    [Importance(Importance.Critical)]
    public class Portal_People_GroupEmail : FixtureBase {
        #region Private Members
        private string _replyTo = "F1AutomatedTester01@gmail.com";
        #endregion Private Members

        #region Compose
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user can unsubscribe from an email.")]
        public void People_GroupEmail_Compose_Send_Unsubscribe() {
            // Set initial conditions
            int churchId = 15;
            string individualName = "Visitor Sneeden";
            int individualId = base.SQL.People_Individuals_FetchID(churchId, individualName);
            string email = "F1AutomatedTester03@gmail.com";
            base.SQL.Execute("UPDATE ChmPeople.dbo.Individual SET UnsubscribeAllChurchEmail = 0 WHERE CHURCH_ID = {0} AND INDIVIDUAL_ID = {1}", churchId, individualId);

            try
            {
                // Login to portal
                TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.Login();

                // Send an email to the designated recipient(s)  
                test.Portal.People_GroupEmail_Compose("Test email from Portal - Unsubscribe", null, _replyTo, new string[] { individualName }, "Testing unsubscribe functionality", false, "F1AutomatedTester01@gmail.com", false);

                // Logout of portal
                test.Portal.Logout();

                // Navigate to the unsubscribe page
                TestLog.WriteLine(test.Weblink["URL"]);
                string url = string.Format("{0}/email/unsubscribe.aspx?ccode=alyq5rXA9igtXilwXAT+3Q==&erid={1}", test.Weblink["URL"], this.encrypt(base.SQL.Execute(string.Format("SELECT TOP 1 EMAIL_RECIPIENT_ID FROM ChmEmail.dbo.EMAIL_RECIPIENT WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ADDRESS = '{1}' AND INDIVIDUAL_ID = {2} ORDER BY CREATED_DATE DESC", churchId, email, individualId)).Rows[0]["EMAIL_RECIPIENT_ID"].ToString()));
                TestLog.WriteLine(url);
                test.Selenium.Open(url);

                //Verify Updated Unsubscribed F1-3945
                test.Selenium.VerifyTextPresent("Confirmation");
                test.Selenium.VerifyTextPresent("Please be aware by clicking \"Unsubscribe\" below this will prevent you from receiving any email sent from the church or organization to any group of people for any reason.");
                test.Selenium.VerifyTextPresent("Unfortunately, there is no automatic method of allowing some emails through when you unsubscribe. In rare cases, your church may have implemented a manual process of allowing this.");

                //Unsubscribe
                test.Selenium.ClickAndWaitForPageToLoad("btnUnsubscribe");

                //Verify re-subscribe
                test.Selenium.VerifyTextPresent(string.Format("The email address {0} has been successfully unsubscribed.", email));
                test.Selenium.VerifyTextPresent("Didn't mean to unsubscribe? No problem, you can");
                test.Selenium.VerifyElementPresent("link=re-subscribe");

                // Verify the individual is unsubscribed
                Assert.AreEqual(true, base.SQL.People_GetIndividualUnsubscribedStatus(churchId, individualName), "Individual unsubscribed from emails but the DB was not updated correctly!");

                // Re-Subscribe
                test.Selenium.ClickAndWaitForPageToLoad("link=re-subscribe");

                // Verify Success
                test.Selenium.VerifyTextPresent(string.Format("You have successfully re-subscribed {0}", email));
            }
            finally
            {
                // Resubscribe the individual
                base.SQL.Execute("UPDATE ChmPeople.dbo.Individual SET UnsubscribeAllChurchEmail = 0 WHERE CHURCH_ID = {0} AND INDIVIDUAL_ID = {1}", churchId, individualId);
            }
            
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies the user that has unsubscribed from an email shows as unsuscribed.")]
        public void People_GroupEmail_Compose_Verify_Unsubscribe()
        {
            // Set initial conditions
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            //Login to Portal
            test.Portal.Login();

            //Navigate to Recipients page
            test.Selenium.Navigate(Navigation.People.GroupEmail.Compose);
            test.Selenium.Type("ctl00_ctl00_MainContent_content_txtSubject", "Test email");
            test.Selenium.ClickAndWaitForPageToLoad("link=Add recipients");
            
            //Search for individual
            test.Selenium.Type("txtSearch", "Unsubscribe");
            test.Selenium.Click("btnSearch");
            test.Selenium.Click("btnAddIndividual");

            // Verify the individual is unsubscribed on the recipients page
            test.Selenium.VerifyTextPresent("Recipient is Unsubscribed from receiving group emails");
            
            //Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Sifter #7787, Verifies Email can be send to adress with more than 50 chars")]
        public void People_GroupEmail_Compose_EmailAddressMorethan50Chars()
        {
            // Set initial conditions
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            //Set conditions
            string emailAddr = "F1AutomatedTester01@gmail.com";
            string password = "ActiveQA12";
            string _fromConfirmAddr = "info@fellowshiponemail.com";
            string _fromAddr = "\"FellowshipOne AutomatedTester01\" <" + emailAddr + ">";
            string _replyTo = emailAddr;
            string _confirmAddr = _replyTo;
            string[] _recepients = new string[] { "Email Long" };

            // Login to portal
            test.Portal.Login("autotester01", "FT4life!", "dc");

            // Send an email to the designated recipient(s)  
           test.Portal.People_GroupEmail_Compose("Test email from Portal - >50 chars", null, _replyTo, _recepients, "This is a test email sent from Portal", false, emailAddr, false);

            //Verify Sent Email page loads
            test.Selenium.VerifyTextPresent("Test email from Portal - >50 chars"); 

            // Logout of portal
            test.Portal.Logout();

        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to views an email preview without entering a subject.")]
        public void People_GroupEmail_Compose_PreviewEmailNoData() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->compose
            test.Selenium.Navigate(Navigation.People.GroupEmail.Compose);

            // Attempt to view a preview without entering a subject
            test.Selenium.ClickAndWaitForPageToLoad("link=Preview email");

            // Verify error(s) displayed to the user
            test.Selenium.VerifyTextPresent(new string[] { TextConstants.ErrorHeadingSingular, "Subject is required and cannot exceed 200 characters." });

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views an email preview.")]
        public void People_GroupEmail_Compose_PreviewEmail() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("autotester01", "FT4life!", "dc");

            // Navigate to people->compose
            test.Selenium.Navigate(Navigation.People.GroupEmail.Compose);

            // Enter a subject
            test.Selenium.Type("ctl00_ctl00_MainContent_content_txtSubject", "Test Email");

            // View the preview
            test.Selenium.ClickAndWaitForPageToLoad("link=Preview email");

            // Verify the button text
            Assert.AreEqual("Email composition", test.Selenium.GetText("//span[@class='preview_button_middle']"));

            // Verify details
            Assert.AreEqual("FellowshipOne AutomatedTester01 <F1AutomatedTester01@gmail.com>", test.Selenium.GetText("//div[@id='preview_window']/table/tbody/tr[1]/td"));

            // Return
            test.Selenium.ClickAndWaitForPageToLoad("//span[@class='preview_button_middle']");

            // Logout of portal
            test.Portal.Logout();
        }

        #endregion Compose

        #region Drafts
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Attempts to save a Draft after only providing a subject.")]
        public void People_GroupEmail_Drafts_CreateOnlySubject() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Create a draft with a subject only
            test.Portal.People_GroupEmail_Drafts_Create("Test Draft - Subject Only", null, test.Configuration.AppSettings.Settings["FTTests.Host"].Value.ToString());

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to save a Draft with an email body.")]
        public void People_GroupEmail_Drafts_CreateWithBody() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Create a new draft
            test.Portal.People_GroupEmail_Drafts_Create("Test Draft - With Body", "This is a test draft", test.Configuration.AppSettings.Settings["FTTests.Host"].Value.ToString());

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to save a Draft when created via People->Compose with a body.")]
        public void People_GroupEmail_Drafts_CreateViaComposeWithBody() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->compose
            test.Selenium.Navigate(Navigation.People.GroupEmail.Compose);

            // Populate the fields with the supplied data
            test.Selenium.Type("ctl00_ctl00_MainContent_content_txtSubject", "Test Draft - Via Compose");

            // Enter the body of the email
            test.Selenium.SelectFrame("rich_text_editor_ifr");
            test.Selenium.Focus(TestConstants.HtmlEditor);
            test.Selenium.TypeKeys(TestConstants.HtmlEditor, "This is a test draft.");
            test.Selenium.SelectWindow("");

            // Save the draft
            DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Selenium.Click("link=Save draft");
            string currentTimeFormatted = string.Empty;

            currentTime = test.GeneralMethods.AddHourSeGrid(currentTime, test.Configuration.AppSettings.Settings["FTTests.Host"].Value.ToString(), 0);

            //Modified Selnium Grid to match Central Time
            currentTimeFormatted = string.Format("Today at {0:t}", currentTime.AddMinutes(-1));
            

            Retry.WithPolling(500).WithTimeout(10000).WithFailureMessage(string.Format("The draft was not saved in the allotted time. Between {0:t} and {1}", currentTime, currentTimeFormatted))
                .Until(() => ((test.Selenium.GetText("saved_timestamp") == (currentTimeFormatted = string.Format("Today at {0:t}", currentTime))) || (test.Selenium.GetText("saved_timestamp") == (currentTimeFormatted))));

            // Verify the timestamp
            test.Selenium.VerifyTextPresent(currentTimeFormatted);

            currentTime = test.GeneralMethods.AddHourSeGrid(currentTime, test.Configuration.AppSettings.Settings["FTTests.Host"].Value.ToString(), 0);


            // Verify the draft was created
            test.Selenium.Navigate(Navigation.People.GroupEmail.Drafts);
            decimal tableRow = test.GeneralMethods.GetTableRowNumber(TableIds.People_Drafts, "Test Draft - Via Compose", "Subject", null);
            Assert.AreEqual("Test Draft - Via Compose", test.Selenium.GetTable(string.Format("{0}.{1}.0", TableIds.People_Drafts, tableRow)));
            Assert.Between(test.Selenium.GetTable(string.Format("{0}.{1}.2", TableIds.People_Drafts, tableRow)), string.Format("Today at {0:t}", currentTime.AddMinutes(-2)), string.Format("Today at {0:t}", currentTime.AddMinutes(2)));

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to save a Draft when created via People->Compose without a body.")]
        public void People_GroupEmail_Drafts_CreateViaComposeNoBody() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to people->compose
            test.Selenium.Navigate(Navigation.People.GroupEmail.Compose);

            // Populate the fields with the supplied data
            test.Selenium.Type("ctl00_ctl00_MainContent_content_txtSubject", "Test Draft - Via Compose, No Body");

            // Save the draft
            DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Selenium.Click("link=Save draft");
            string currentTimeFormatted = string.Empty;

            TestLog.WriteLine("HOST: {0}", test.Configuration.AppSettings.Settings["FTTests.Host"].Value);

            currentTime = test.GeneralMethods.AddHourSeGrid(currentTime, test.Configuration.AppSettings.Settings["FTTests.Host"].Value.ToString(), 0);

            currentTimeFormatted = string.Format("Today at {0:t}", currentTime.AddMinutes(-1));


            Retry.WithPolling(500).WithTimeout(10000).WithFailureMessage(string.Format("The draft was not saved in the allotted time. Between {0:t} and {1}", currentTime, currentTimeFormatted))
                .Until(() => ((test.Selenium.GetText("saved_timestamp") == (currentTimeFormatted = string.Format("Today at {0:t}", currentTime))) || (test.Selenium.GetText("saved_timestamp") == (currentTimeFormatted))));

            test.GeneralMethods.TakeScreenShot();

            currentTime = test.GeneralMethods.AddHourSeGrid(currentTime, test.Configuration.AppSettings.Settings["FTTests.Host"].Value.ToString(), 0);

            // Verify the timestamp
            test.Selenium.VerifyTextPresent(currentTimeFormatted);

            // Verify the draft was created
            test.Selenium.Navigate(Navigation.People.GroupEmail.Drafts);
            decimal tableRow = test.GeneralMethods.GetTableRowNumber(TableIds.People_Drafts, "Test Draft - Via Compose, No Body", "Subject", null);
            Assert.AreEqual("Test Draft - Via Compose, No Body", test.Selenium.GetTable(string.Format("{0}.{1}.0", TableIds.People_Drafts, tableRow)));
            Assert.Between(test.Selenium.GetTable(string.Format("{0}.{1}.2", TableIds.People_Drafts, tableRow)), string.Format("Today at {0:t}", currentTime.AddMinutes(-1)), string.Format("Today at {0:t}", currentTime.AddMinutes(1)));

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Drafts

        #region Sent Items
        #endregion Sent Items

        #region Templates
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Creates a shared Template.")]
        public void People_GroupEmail_Templates_CreateShared() {
            // Set initial conditions
            string templateName = "Test Template - Shared";
            base.SQL.People_DeleteTemplate(15, templateName);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Create a shared email template
            //string[] mergeFields = { "link=First Name", "link=Last Name", "link=Household Name", "link=Goes By/First Name", "link=Prefix" };
            string[] mergeFields = { "//a[@data-value='{FirstName}']", "//a[@data-value='{LastName}']", "//a[@data-value='{HouseholdName}']", "//a[@data-value='{GoesByName}']", "//a[@data-value='{Prefix}']" };
            test.Portal.People_GroupEmail_Templates_Create(templateName, true, "This is a shared test template", mergeFields);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Creates a Template.")]
        public void People_GroupEmail_Templates_Create() {
            // Set initial conditions
            string templateName = "Test Template";
            base.SQL.People_DeleteTemplate(15, templateName);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Create an email template
            string[] mergeFields = { "//a[@data-value='{FirstName}']", "//a[@data-value='{LastName}']", "//a[@data-value='{HouseholdName}']", "//a[@data-value='{GoesByName}']", "//a[@data-value='{Prefix}']" };
            test.Portal.People_GroupEmail_Templates_Create(templateName, false, "This is a test template", mergeFields);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Updates a Template.")]
        public void People_GroupEmail_Templates_Update() {
            // Set initial conditions
            string templateName = "Test Template - Update";
            string templateNameUpdated = "Test Template - Updated";
            base.SQL.People_DeleteTemplate(15, templateName);
            base.SQL.People_DeleteTemplate(15, templateNameUpdated);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Create an email template
            string[] mergeFields = { "//a[@data-value='{FirstName}']", "//a[@data-value='{LastName}']", "//a[@data-value='{HouseholdName}']", "//a[@data-value='{GoesByName}']", "//a[@data-value='{Prefix}']" };
            test.Portal.People_GroupEmail_Templates_Create(templateName, false, "This is a test template", mergeFields);

            // Update an email template
            string[] mergeFieldsUpdate = { "//a[@data-value='{Prefix}']", "//a[@data-value='{GoesByName}']", "//a[@data-value='{HouseholdName}']", "//a[@data-value='{LastName}']", "//a[@data-value='{FirstName}']" };
            test.Portal.People_GroupEmail_Templates_Update(templateName, false, "{FirstName}{LastName}{HouseholdName}{GoesByName}{Prefix}\nThis is a test template", templateNameUpdated, true, "This is an updated email template", mergeFieldsUpdate);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Deletes a Template.")]
        public void People_GroupEmail_Templates_Delete() {
            // Set initial conditions
            string templateName = "Test Template - Delete";
            base.SQL.People_DeleteTemplate(15, templateName);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Create an email template
            string[] mergeFields = { "//a[@data-value='{FirstName}']", "//a[@data-value='{LastName}']", "//a[@data-value='{HouseholdName}']", "//a[@data-value='{GoesByName}']", "//a[@data-value='{Prefix}']" };
            test.Portal.People_GroupEmail_Templates_Create(templateName, false, "This is a test template - Delete", mergeFields);

            // Delete an email template
            test.Portal.People_GroupEmail_Templates_Delete(templateName);

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Templates

        #region Delegates
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Creates a Delegate.")]
        public void People_GroupEmail_Delegates_Create() {
            // Set initial conditions
            base.SQL.People_Delegates_Delete(15, "msneeden", "cgutekunst");

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            //Delete all same delegate
            test.Portal.People_GroupEmail_Delegates_Delete_All("Gutekunst, Coly");

            // Add a delegate
            test.Portal.People_GroupEmail_Delegates_Create("Gutekunst, Coly");

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Deletes a Delegate.")]
        public void People_GroupEmail_Delegates_Delete() {
            // Set initial conditions
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            base.SQL.People_Delegates_Create(15, "msneeden", "ft.tester");

            // Login to Portal
            test.Portal.Login();

            //Delete all same delegate
            test.Portal.People_GroupEmail_Delegates_Delete_All("Tester, FT");

            // Add a delegate
            test.Portal.People_GroupEmail_Delegates_Create("Tester, FT");

            // Delete a delegate
            test.Portal.People_GroupEmail_Delegates_Delete("Tester, FT");

            // Logout of Portal
            test.Portal.Logout();
        }
        #endregion Delegates


        #region Private Methods
        private string encrypt(string value) {
            // first we need to translate the string into an array of bytes
            byte[] abPlainText = System.Text.Encoding.UTF8.GetBytes(value);

            // create a symmetric algorithm object
            Rijndael alg = Rijndael.Create();
            alg.Key = Encoding.ASCII.GetBytes("K2Uh7T3lhnB9j1l$");
            alg.IV = Encoding.ASCII.GetBytes("VkRisp&nksTsko%4");

            // create a memory stream to hold the encrypted bytes
            MemoryStream ms = new MemoryStream();

            // create and setup a crypto stream
            CryptoStream cs;

            // we need base64 encoded text as a result
            // chain two crypto stream together - encrypting and base64 encoding
            CryptoStream cs_base64 = new CryptoStream(ms, new ToBase64Transform(), CryptoStreamMode.Write);
            cs = new CryptoStream(cs_base64, alg.CreateEncryptor(), CryptoStreamMode.Write);

            // perform the encryption
            cs.Write(abPlainText, 0, abPlainText.Length);

            // we need to call Close() or FlushFinalBlock() to indicate that
            // we have written all the data in and it is safe to apply padding
            cs.Close();

            // get the encrypted bytes
            byte[] abCipherText = ms.ToArray();
            String sCipherText = System.Text.Encoding.UTF8.GetString(abCipherText);
            return sCipherText;
        }
        #endregion Private Methods
    }
}