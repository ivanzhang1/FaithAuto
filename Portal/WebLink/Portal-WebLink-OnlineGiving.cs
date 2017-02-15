using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace FTTests.Portal.WebLink {

    [TestFixture]
    [Importance(Importance.Critical)]
    public class Portal_WebLink_OnlineGiving_WebDriver : FixtureBaseWebDriver
    {
        #region Confirmation Message

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Sifter #7815 Wrong Currency Symbol Showing")]
        public void WebLink_OnlineGiving_Verify_ConfirmationMessage_US_Currency()
        {

            //Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            test.GeneralMethods.Navigate_Portal(Navigation.WebLink.Online_Giving.Confirmation_Messages);

            //Verify Contribution Success
            test.Driver.FindElementByLinkText("Contribution Success").Click();
            TestLog.WriteLine("Message Contribution: " + test.Driver.FindElementById("message_contribution").Text);
            Assert.IsTrue(test.Driver.FindElementById("message_contribution").Text.Contains("$"), "$ symbol found");
            Assert.IsFalse(test.Driver.FindElementById("message_contribution").Text.Contains("£"), "£ symbol not found");

            //Verify Contribution Success
            test.Driver.FindElementByLinkText("Contribution Failure").Click();
            TestLog.WriteLine("Message Contribution: " + test.Driver.FindElementById("message_contribution").Text);
            Assert.IsTrue(test.Driver.FindElementById("message_contribution").Text.Contains("$"), "$ symbol found");
            Assert.IsFalse(test.Driver.FindElementById("message_contribution").Text.Contains("£"), "£ symbol found");

            //Logout
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Sifter #7815 Wrong Currency Symbol Showing")]
        public void WebLink_OnlineGiving_Verify_ConfirmationMessage_UK_Currency()
        {

            //Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "QAEUNLX0C6");

            test.GeneralMethods.Navigate_Portal(Navigation.WebLink.Online_Giving.Confirmation_Messages);

            //Verify Contribution Success
            test.Driver.FindElementByLinkText("Contribution Success").Click();
            TestLog.WriteLine("Message Contribution: " + test.Driver.FindElementById("message_contribution").Text);
            Assert.IsFalse(test.Driver.FindElementById("message_contribution").Text.Contains("$"), "$ symbol found");
            Assert.IsTrue(test.Driver.FindElementById("message_contribution").Text.Contains("£"), "£ symbol not found");

            //Verify Contribution Success
            test.Driver.FindElementByLinkText("Contribution Failure").Click();
            TestLog.WriteLine("Message Contribution: " + test.Driver.FindElementById("message_contribution").Text);
            Assert.IsFalse(test.Driver.FindElementById("message_contribution").Text.Contains("$"), "$ symbol found");
            Assert.IsTrue(test.Driver.FindElementById("message_contribution").Text.Contains("£"), "£ symbol found");

            //Logout
            test.Portal.LogoutWebDriver();
        }

        
        #endregion Confirmation Message
    }


    [TestFixture]
    [Importance(Importance.Critical)]
    public class Portal_WebLink_OnlineGiving : FixtureBase {
        #region Confirmation Message
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Updates a Confirmation Message.")]
        public void WebLink_OnlineGiving_ConfirmationMessage_Update() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Update a confirmation message
            test.Portal.WebLink_ConfirmationMessage_Update("contribution@failure.com", "Contribution Failure", "This is a test message for Contribution Failure");

            // Reset data
            test.Selenium.Click("link=Use Default Values");

            // Verify message was updated
            Assert.AreEqual("no-reply@infellowship.com", test.Selenium.GetValue("FromAddress"));
            Assert.AreEqual("Error Regarding your Contribution to Dynamic Church", test.Selenium.GetValue("SubjectLine"));
            Assert.AreEqual("There was an error with your recent contribution to Dynamic Church. Please review the information below.", test.Selenium.GetValue("EmailMessage"));

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Confirmation Message
    }
}