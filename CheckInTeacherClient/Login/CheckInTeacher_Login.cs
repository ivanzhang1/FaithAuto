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
    class CheckInTeacher_Login : CheckInBaseWebDriver
    {
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies login in sucess")]
        public void CheckInTeacher_Login_RightUserPwd()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.CheckIn.LoginWebDriver();
            //Click Icon-User
            test.CheckIn.ClickIconMenu();
            //Logout
            test.CheckIn.LogoutWebDriver();
            
        }

        //[Test(Order=1), RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies login in fails when the church has no teacher module")]
        public void CheckInTeacher_Login_FailWhenChurchUnAuth()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string module = "CheckIn Teacher";
            TestLog.WriteLine("Turn off church permission to module: " + module);
            test.SQL.Portal_Modules_Delete(churchId, module);
            try
            {
                //Login and validate log in fails
                test.CheckIn.LoginWebDriver(false);
            }
            finally
            {
                TestLog.WriteLine("Turn on church permission to module: " + module);
                test.SQL.Portal_Modules_Add(churchId, module);
            }

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies login in fails when the church code is wrong")]
        public void CheckInTeacher_Login_FailWhenChurchWrong()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            //Login and validate log in fails
            test.CheckIn.LoginWebDriver(test.CheckIn.CheckInUsername, test.CheckIn.CheckInPassword, "DCWrong", false);

        }

        // [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies login in fails when user password is wrong")]
        public void CheckInTeacher_Login_FailWhenPasswordWrong()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            //Login and validate log in fails
            test.CheckIn.LoginWebDriver(test.CheckIn.CheckInUsername, test.CheckIn.CheckInPassword + "bad", test.CheckIn.ChurchCode, false);

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies login in fails when user name is wrong")]
        public void CheckInTeacher_Login_FailWhenUserNameWrong()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            //Login and validate log in fails
            test.CheckIn.LoginWebDriver(test.CheckIn.CheckInUsername + ".cn", test.CheckIn.CheckInPassword, test.CheckIn.ChurchCode, false);

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies login in fails when user is not checked in")]
        public void CheckInTeacher_Login_FailWhenUserUncheckedIn()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            //Login and validate log in fails
            test.CheckIn.LoginWebDriver("graceteacher1@163.com", test.CheckIn.CheckInPassword, test.CheckIn.ChurchCode, false);

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies login in fails when user is checked in as a student")]
        public void CheckInTeacher_Login_FailWhenUserCheckedInAsStudent()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string studentlName = "FT Tester";

            // Check in a student
            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            string checkInTime = currentTimzoneTime.ToString();
            String[] individualNameArray = { studentlName };
            string individualTypeId = "1";//student 1
            TestLog.WriteLine("Check in a student individual: " + individualNameArray[0]);

            test.CheckIn.CheckInMultipleIndividuals(activityName, individualNameArray, 13245, checkInTime, individualTypeId, rosterNameArray[2]);

            try
            {
                //Login and validate log in fails
                test.CheckIn.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", test.CheckIn.ChurchCode, false);
            }
            finally
            {
                test.SQL.Ministry_ActivitySchedules_DeleteIndividualCheckedIn(churchId, activityName, studentlName);
            }


        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies login in sucess if password of the teacher account has space")]
        public void CheckInTeacher_Login_SpaceInUserPwd()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string newTeacherEmail = "group.spaceinpassword@gmail.com";

            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            String individualId = Convert.ToString(base.SQL.People_Individuals_FetchIDByEmail(churchId, newTeacherEmail));

            test.CheckIn.CheckInATeacher(churchId, activityName, individualId, rosterToCheckIn, currentTimzoneTime);

            try
            {
                //Login and validate log in succeeds
                test.CheckIn.LoginWebDriver(newTeacherEmail, "BM.Admin 09", test.CheckIn.ChurchCode);
                test.CheckIn.LogoutWebDriver();
            }
            finally
            {
                test.SQL.Ministry_ActivitySchedules_DeleteIndividualCheckedIn(churchId, activityName, Convert.ToInt32(individualId));
            }

        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("Verifies F1data index page is display sucess")]
        public void CheckInTeacher_F1Data_CheckIndex()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Load F1Data index page
            test.CheckIn.OpenF1DataIndex();

            test.GeneralMethods.WaitForElementDisplayed(By.XPath("//h2"));
            test.GeneralMethods.WaitForElementDisplayed(By.XPath("//body/div/div[1]"));
            test.GeneralMethods.WaitForElementDisplayed(By.XPath("//body/div/div[2]"));
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("Verifies F1data signalR hub page is display sucess")]
        public void CheckInTeacher_F1Data_CheckSignalR()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Load F1Data SignalR page
            test.CheckIn.OpenF1DataSignalR();

            test.GeneralMethods.WaitForElementDisplayed(By.XPath("//body"));
            string text = test.Driver.FindElementByXPath("//body").Text;
            Assert.IsTrue(text.Contains("ASP.NET SignalR JavaScript Library"), "SignalR page content is wrong");
            Assert.IsTrue(text.Contains("signalR"), "SignalR page content is wrong");
        }
    }
}
