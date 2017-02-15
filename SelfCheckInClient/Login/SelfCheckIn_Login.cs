using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using OpenQA.Selenium;
using Common.PageEntitys;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Collections.Generic;
using System.Net;

namespace FTTests.SelfCheckIn
{
    [TestFixture, Parallelizable]
    class SelfCheckIn_Login : SelfCheckInBaseWebDriver
    {
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies login in sucess")]
        public void SelfCheckIn_Login_RightUserPwd()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.SelfCheckIn.LoginWebDriver();

            //Logout
            test.SelfCheckIn.LogoutWebDriver();
            
        }

        [Test(Order=1), RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies login in fails when the church has no teacher module")]
        public void SelfCheckIn_Login_FailWhenChurchUnAuth()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string module = "CheckIn SelfMobile";
            TestLog.WriteLine("Turn off church permission to module: " + module);

            string testChurchCode = "QAEUNLX0C1";
            int testChurchId = base.SQL.Ministry_Church_FetchID(test.SelfCheckIn.ChurchCode);
            test.SQL.Portal_Modules_Delete(testChurchId, module);
            try
            {
                //Login and validate log in fails
                test.SelfCheckIn.LoginWebDriver(test.SelfCheckIn.SelfCheckInUsername, test.SelfCheckIn.SelfCheckInPassword, testChurchCode, false);
            }
            finally
            {
                TestLog.WriteLine("Turn on church permission to module: " + module);
                test.SQL.Portal_Modules_Add(testChurchId, module);
            }

        }
        [Test(Order = 2), RepeatOnFailure]
        [Author("Grace zhang")]
        [Description("Verifies login in fails when Feature is set Dark")]
        public void SelfCheckIn_Login_FailWhenFeatureDark()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string module = "CheckIn SelfMobile";
            TestLog.WriteLine("Turn off church permission to module: " + module);
            string testChurchCode = "QAEUNLX0C1";
            base.SQL.Set_Feature_DarkOrLight(12, 1);
           
            try
            {
                //Login and validate log in fails
                test.SelfCheckIn.LoginWebDriver(test.SelfCheckIn.SelfCheckInUsername, test.SelfCheckIn.SelfCheckInPassword, testChurchCode, false);
            }
            finally
            {
                TestLog.WriteLine("Set Feature-Self-Check-IN Light: ");
                base.SQL.Set_Feature_DarkOrLight(12, 4);
            }

        }

        [Test(Order = 3), RepeatOnFailure]
        [Author("Grace zhang")]
        [Description("Verifies login in fails when Feature is Disable")]
        public void SelfCheckIn_Login_FailWhenDisableOnProtal()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            test.Portal.Weblink_EnterInfellowshipFeature();
            //TODO: Diasbla Self-CheckIn


            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies login in fails when the church code is wrong")]
        public void SelfCheckIn_Login_FailWhenChurchWrong()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            //Login and validate log in fails
            test.SelfCheckIn.LoginWebDriver(test.CheckIn.CheckInUsername, test.CheckIn.CheckInPassword, "DCWrong", false);

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies login in fails when user name is wrong")]
        public void SelfCheckIn_Login_FailWhenUserNameWrong()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            //Login and validate log in fails
            test.SelfCheckIn.LoginWebDriver(test.CheckIn.CheckInUsername + ".cn", test.CheckIn.CheckInPassword, test.CheckIn.ChurchCode, false);

        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("Verifies F1data index page is display sucess")]
        public void SelfCheckIn_F1Data_CheckIndex()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Load F1Data index page
            test.SelfCheckIn.OpenF1DataIndex();

            test.GeneralMethods.WaitForElementDisplayed(By.XPath("//h2"));
            test.GeneralMethods.WaitForElementDisplayed(By.XPath("//body/div/div[1]"));
            test.GeneralMethods.WaitForElementDisplayed(By.XPath("//body/div/div[2]"));
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("Verifies F1data signalR hub page is display sucess")]
        public void SelfCheckIn_F1Data_CheckSignalR()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Load F1Data SignalR page
            test.SelfCheckIn.OpenF1DataSignalR();

            test.GeneralMethods.WaitForElementDisplayed(By.XPath("//body"));
            string text = test.Driver.FindElementByXPath("//body").Text;
            Assert.IsTrue(text.Contains("ASP.NET SignalR JavaScript Library"), "SignalR page content is wrong");
            Assert.IsTrue(text.Contains("signalR"), "SignalR page content is wrong");
        }

    }
}
