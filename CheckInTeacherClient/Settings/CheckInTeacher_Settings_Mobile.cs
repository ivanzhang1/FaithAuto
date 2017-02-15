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

namespace FTTests.CheckInTeacher
{
    [TestFixture, Parallelizable]
    class CheckInTeacher_Settings_Mobile : CheckInBaseWebDriver
    {
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies the teacher's name")]
        public void CheckInTeacher_Settings_CheckTeacherName_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.CheckIn.LoginMobileWebDriver();
            //Click Icon-User
            test.CheckIn.ClickIconMenu();

            String acturalTeacherName = test.CheckIn.GetTeacherName();
            String churchCode = test.CheckIn.ChurchCode;
            int churchId = base.SQL.Ministry_Church_FetchID(churchCode);
            int individualId = base.SQL.People_Individuals_FetchIDByEmail(churchId, test.CheckIn.CheckInUsername);
            String expectTeacherName = base.SQL.People_GetIndividualName(churchId, individualId);
            Assert.AreEqual(expectTeacherName, acturalTeacherName);
           
            //Logout
            test.CheckIn.LogoutWebDriver();

        }
        [Test, RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("Verifies can select clasroom")]
        public void CheckInTeacher_Settings_SelectOneClassroom_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.CheckIn.LoginMobileWebDriver();
            //Click Icon-User
            test.CheckIn.ClickIconMenu();
           
            String churchCode = test.CheckIn.ChurchCode;
            String classRoomName = rosterNameArray[0];
            //select a classroom
            test.CheckIn.ClickSettingsRoom(classRoomName);
            
            //Logout
            test.CheckIn.LogoutWebDriver();
            
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("Verifies SSO icon display well on left top")]
        public void CheckInTeacher_Settings_CheckSSO_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            TestLog.WriteLine("Log in Teacher project");
            test.CheckIn.LoginMobileWebDriver();

            TestLog.WriteLine("Check SSO icon displays well");
            test.GeneralMethods.WaitForElement(By.CssSelector("[ng-controller=\"LauncherController\"]"));
            TestLog.WriteLine("Click SSO icon");
            test.Driver.FindElementByCssSelector("[class=\"nav-bar-block\"][nav-bar=\"active\"]").FindElement(By.CssSelector("[class=\"NineGrid-img\"]")).Click();

            test.GeneralMethods.WaitForElement(By.CssSelector("[class=\"NineGrid-apps-list\"]"));
            test.GeneralMethods.WaitForElement(By.CssSelector("[class=\"NineGrid-img NineGrid-img--prom\"]"));
            test.Driver.FindElementByCssSelector("[class=\"nav-bar-block\"][nav-bar=\"active\"]").FindElement(By.CssSelector("[class=\"NineGrid-img NineGrid-img--prom\"]")).Click();

            //Logout
            test.CheckIn.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies My account link works well in Setting panel")]
        public void CheckInTeacher_Settings_CheckMyAccount_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            TestLog.WriteLine("Log in Teacher project");
            test.CheckIn.LoginMobileWebDriver();
            TestLog.WriteLine("Click Icon-Menu on the right top");
            test.CheckIn.ClickIconMenu();

            TestLog.WriteLine("Wait for My account display");
            test.GeneralMethods.WaitForElementDisplayed(By.LinkText("My account"));

            TestLog.WriteLine("Click on the My Account link");
            test.Driver.FindElementByCssSelector(String.Format("[href=\"https://launchpad.{0}.fellowshipone.com/#/profile/update\"]", test.CheckIn.GetEnvironmentInUse(this._f1Environment)).ToString()).Click();

            TestLog.WriteLine("Wait for a second and check new tab opened");
            System.Threading.Thread.Sleep(1000);
            Assert.IsTrue(2 == test.Driver.WindowHandles.Count, "The new browser tab for Launchpad is not opened");
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[0]);

            //Logout
            test.CheckIn.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies Footers link works well in Setting panel")]
        public void CheckInTeacher_Settings_CheckFooters_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            TestLog.WriteLine("Log in Teacher project");
            test.CheckIn.LoginMobileWebDriver();
            TestLog.WriteLine("Click Icon-Menu on the right top");
            test.CheckIn.ClickIconMenu();

            TestLog.WriteLine("Wait for Footers display");
            churchId = test.SQL.Ministry_Church_FetchID(test.CheckIn.ChurchCode);
            timeZoneName = test.SQL.Ministry_Activity_Instance_TimeZone(churchId);

            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            String year = Convert.ToString(currentTimzoneTime.Year);
            test.GeneralMethods.WaitForElementDisplayed(By.LinkText("© " + year + " Active Network, LLC. All Rights Reserved"));

            TestLog.WriteLine("Click on the Footers link");
            test.Driver.FindElementByCssSelector("[href=\"http://www.activenetwork.com/information/terms-of-use\"]").Click();
            test.Driver.FindElementByCssSelector("[href=\"http://www.activenetwork.com/information/copyright-policy\"]").Click();
            test.Driver.FindElementByCssSelector("[href=\"http://www.activenetwork.com/information/privacy-policy\"]").Click();
            test.Driver.FindElementByCssSelector("[href=\"http://www.activenetwork.com/information/cookie-policy\"]").Click();
            test.Driver.FindElementByCssSelector("[href=\"http://www.activenetwork.com/information/security\"]").Click();
            test.Driver.FindElementByCssSelector("[href=\"http://www.fellowshipone.com\"]").Click();
            test.Driver.FindElementByCssSelector("[href=\"http://www.activenetwork.com\"]").Click();

            TestLog.WriteLine("Wait for a second and check new tabs opened");
            System.Threading.Thread.Sleep(1000);
            Assert.IsTrue(8 == test.Driver.WindowHandles.Count, "The new browser tabs for footer are not all opened");
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[0]);

            //Logout
            test.CheckIn.LogoutWebDriver();

        }

    }
}
