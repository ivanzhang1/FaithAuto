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
using System.Threading;

namespace FTTests.CheckInTeacher
{
    [TestFixture, Parallelizable]
    class CheckInTeacher_RealTime : CheckInBaseWebDriver
    {
        [Test(Order = 1), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("Verifies Real-time sucess")]
        public void CheckInTeacher_RealTime_CheckinAndCheckout()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string individualId = Convert.ToString(this._sql.People_Individuals_FetchIDByName(churchId, "GraceAutoSpouse Zhang"));
            String[] individualNameArray = { "GraceAutoSpouse Zhang" };
            string individualTypeId = "1";//student
            
            //Login
            test.CheckIn.LoginWebDriver();
            //Open setitngs
            test.CheckIn.ClickIconMenu();
            test.CheckIn.ClickSettingsRoom(rosterToCheckIn);
            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            try
            {
                //Checkin a student in db
                String[] attendenceids = test.CheckIn.CheckInMultipleIndividuals(activityName, individualNameArray, 16729, currentTimzoneTime.ToString(), individualTypeId, rosterToCheckIn);
                //Notice front end
                test.CheckIn.RealtimeNoticeCheckin(churchId, activityName, individualId, "1", rosterToCheckIn, null, null, "0");
                //wait for signalR
                Thread.Sleep(10 * 1000);
               
                //Check status right
                String expectstatusInfo = "On Site at " + test.CheckIn.getExpectOnSiteTime(churchId, "ActivityCheckIn-T3", "GraceAutoSpouse Zhang");
                String statusInfo = test.CheckIn.GetStatusAndTimeByStudentName("GraceAutoSpouse", null, "Zhang");
                Assert.AreEqual(expectstatusInfo, statusInfo);

                //RealTime-Checkout
                test.CheckIn.RealTimeDeleteIndividualInstance(activityName, individualId, individualTypeId, rosterToCheckIn);
                Thread.Sleep(5 * 1000);

                int listNo = test.CheckIn.GetListIndexByStudentName("GraceAutoSpouse", null, "Zhang");
                Assert.AreEqual(-1, listNo);
                //Logout
                test.CheckIn.LogoutWebDriver();
            }
            finally
            {
                test.CheckIn.RealTimeDeleteIndividualInstance(activityName, individualId, individualTypeId, rosterToCheckIn);
            }
        }
        [Test(Order = 2), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies Real-time RemoverCurentAssignment")]
        public void CheckInTeacher_RealTime_RemoveCurrentAssignment()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string newTeacherEmail = "ft.autotester@gmail.com";
            string individualTypeId = "2";//teacher
            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            String individualId = Convert.ToString(base.SQL.People_Individuals_FetchIDByEmail(churchId, newTeacherEmail));
            TestLog.WriteLine("-individualId = {0}", individualId);
            try
            {
                test.CheckIn.CheckInATeacher(churchId, activityName, individualId, rosterNameArray[4], currentTimzoneTime);
                //Login
                test.CheckIn.LoginWebDriver(newTeacherEmail, "FT4life!", test.CheckIn.ChurchCode);
                test.CheckIn.ClickIconMenu();
                test.CheckIn.ClickSettingsRoom(rosterNameArray[4]);
                test.GeneralMethods.WaitForElementVisible(By.CssSelector("[class=\"no-students\"]"), 30, "Fail to find the no student content on roster page");
                Assert.AreEqual("No students have checked into this class yet. Students will appear automatically as they arrive.", test.Driver.FindElementByCssSelector("[class=\"no-students\"]").Text.Trim(), "The no student content is wrong");
                //RealTime-Checkout
                test.CheckIn.RealTimeDeleteIndividualInstance(activityName, individualId, individualTypeId, rosterNameArray[4]);
                Thread.Sleep(5 * 1000);
                //Check the no student content display on the Roster page
                test.GeneralMethods.WaitForElementVisible(By.CssSelector("[class=\"no-students\"]"), 30, "Fail to find the no longer been assigned content on roster page");
                Assert.AreEqual("You have no longer been assigned to this room. Please select another available assignment.",
                    test.Driver.FindElementByCssSelector("[class=\"no-students\"]").Text.Trim(), "The no longer been assigned content is wrong");

            }
            finally
            {
                test.CheckIn.RealTimeDeleteIndividualInstance(activityName, individualId, individualTypeId, rosterNameArray[4]);
            }

            //Logout
            test.CheckIn.LogoutWebDriver();

        }
        [Test(Order = 3), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies Real-time RemoveNoCurrentAssignment")]
        public void CheckInTeacher_RealTime_RemoveNoCurrentAssignment()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string newTeacherEmail = "graceteacher4@163.com";
            string individualTypeId = "2";//teacher
            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            String individualId = Convert.ToString(base.SQL.People_Individuals_FetchIDByEmail(churchId, newTeacherEmail));
            TestLog.WriteLine("-individualId = {0}", individualId);

            try
            { 
                //Login
                test.CheckIn.LoginWebDriver();

                //Open setitngs
                test.CheckIn.ClickIconMenu();
                test.CheckIn.ClickSettingsRoom(rosterToCheckIn);
                //Check into a new classroom for teacher
                test.CheckIn.CheckInATeacher(churchId, activityName, individualId, rosterNameArray[4], currentTimzoneTime);
                
                //Notice front end
                test.CheckIn.RealtimeNoticeCheckin(churchId, activityName, individualId, individualTypeId, rosterNameArray[4], null, null, "0");
                //wait for signalR
                Thread.Sleep(10 * 1000);

                Assert.IsTrue(test.CheckIn.IsExistSettingsRoom(rosterNameArray[4]));

                test.GeneralMethods.WaitForElement(By.CssSelector("[class=\"drawer\"]"), 30, "Fail to find students info on roster page");            
                
                //RealTime-Checkout            
                test.CheckIn.RealTimeDeleteIndividualInstance(activityName, individualId, individualTypeId, rosterNameArray[4]);
                Thread.Sleep(5 * 1000);
                test.GeneralMethods.WaitForElement(By.CssSelector("[class=\"drawer\"]"), 30, "Fail to find students info on roster page again");

                //Open setitngs again
                test.CheckIn.ClickIconMenu();
                
                //Check classroom rosterNameArray[4] disappear on settings.
                Assert.IsFalse(test.CheckIn.IsExistSettingsRoom(rosterNameArray[4]));
            }
            finally
            {
                test.CheckIn.RealTimeDeleteIndividualInstance(activityName, individualId, individualTypeId, rosterNameArray[4]);
            }

            //Logout
            test.CheckIn.LogoutWebDriver();

        }

        [Test(Order = 4), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("Verifies Real-time sucess")]
        public void CheckInTeacher_RealTime_CheckinAndCheckout_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string individualId = Convert.ToString(this._sql.People_Individuals_FetchIDByName(churchId, "GraceAutoSpouse Zhang"));
            String[] individualNameArray = { "GraceAutoSpouse Zhang" };
            string individualTypeId = "1";//student
           
            //Login
            test.CheckIn.LoginMobileWebDriver();
            //Open setitngs
            test.CheckIn.ClickIconMenu();
            test.CheckIn.ClickSettingsRoom(rosterToCheckIn);
            
            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            try
            {
                //Checkin a student in db
                String[] attendenceids = test.CheckIn.CheckInMultipleIndividuals(activityName, individualNameArray, 16729, currentTimzoneTime.ToString(), individualTypeId, rosterToCheckIn);
                //Notice front end
                test.CheckIn.RealtimeNoticeCheckin(churchId, activityName, individualId, "1", rosterToCheckIn, null, null, "0");
                //wait for signalR
                Thread.Sleep(10 * 1000);             
                //Check status right
                String expectstatusInfo = "On Site at " + test.CheckIn.getExpectOnSiteTime(churchId, "ActivityCheckIn-T3", "GraceAutoSpouse Zhang");
                String statusInfo = test.CheckIn.GetStatusAndTimeByStudentName("GraceAutoSpouse", null, "Zhang");
                Assert.AreEqual(expectstatusInfo, statusInfo);

                //RealTime-Checkout
                test.CheckIn.RealTimeDeleteIndividualInstance(activityName, individualId, individualTypeId, rosterToCheckIn);
                Thread.Sleep(5 * 1000);
                int listNo = test.CheckIn.GetListIndexByStudentName("GraceAutoSpouse", null, "Zhang");
                Assert.AreEqual(-1, listNo);
                //Logout
                test.CheckIn.LogoutWebDriver();
            }
            finally
            {
                test.CheckIn.RealTimeDeleteIndividualInstance(activityName, individualId, individualTypeId, rosterToCheckIn);
            }
            
        }
        [Test(Order = 5), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("Verifies Real-time RemoverCurentAssignment")]
        public void CheckInTeacher_RealTime_RemoveCurrentAssignment_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string newTeacherEmail = "ft.autotester@gmail.com";
            string individualTypeId = "2";//teacher
            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            String individualId = Convert.ToString(base.SQL.People_Individuals_FetchIDByEmail(churchId, newTeacherEmail));
            TestLog.WriteLine("-individualId = {0}", individualId);
            try
            {
                test.CheckIn.CheckInATeacher(churchId, activityName, individualId, rosterNameArray[4], currentTimzoneTime);
                //Login
                test.CheckIn.LoginMobileWebDriver(newTeacherEmail, "FT4life!", test.CheckIn.ChurchCode);
                test.CheckIn.ClickIconMenu();
                test.CheckIn.ClickSettingsRoom(rosterNameArray[4]);
                test.GeneralMethods.WaitForElementVisible(By.CssSelector("[class=\"no-students\"]"), 30, "Fail to find the no student content on roster page");
                Assert.AreEqual("No students have checked into this class yet. Students will appear automatically as they arrive.", test.Driver.FindElementByCssSelector("[class=\"no-students\"]").Text.Trim(), "The no student content is wrong");
                //RealTime-Checkout
                test.CheckIn.RealTimeDeleteIndividualInstance(activityName, individualId, individualTypeId, rosterNameArray[4]);
                Thread.Sleep(5 * 1000);
                //Check the no student content display on the Roster page
                test.GeneralMethods.WaitForElementVisible(By.CssSelector("[class=\"no-students\"]"), 30, "Fail to find the no longer been assigned content on roster page");
                Assert.AreEqual("You have no longer been assigned to this room. Please select another available assignment.",
                    test.Driver.FindElementByCssSelector("[class=\"no-students\"]").Text.Trim(), "The no longer been assigned content is wrong");

            }
            finally
            {
                test.CheckIn.RealTimeDeleteIndividualInstance(activityName, individualId, individualTypeId, rosterNameArray[4]);
            }

            //Logout
            test.CheckIn.LogoutWebDriver();

        }
        [Test(Order = 6), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies Real-time RemoveNoCurrentAssignment")]
        public void CheckInTeacher_RealTime_RemoveNoCurrentAssignment_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string newTeacherEmail = "graceteacher4@163.com";
            string individualTypeId = "2";//teacher
            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            String individualId = Convert.ToString(base.SQL.People_Individuals_FetchIDByEmail(churchId, newTeacherEmail));
            TestLog.WriteLine("-individualId = {0}", individualId);

            try
            {

                //Login
                test.CheckIn.LoginMobileWebDriver();

                //Open setitngs
                test.CheckIn.ClickIconMenu();
                test.CheckIn.ClickSettingsRoom(rosterToCheckIn);
                //Check into a new classroom for teacher
                test.CheckIn.CheckInATeacher(churchId, activityName, individualId, rosterNameArray[4], currentTimzoneTime);

                //Notice front end
                test.CheckIn.RealtimeNoticeCheckin(churchId, activityName, individualId, individualTypeId, rosterNameArray[4], null, null, "0");
                //wait for signalR
                Thread.Sleep(10 * 1000);

                Assert.IsTrue(test.CheckIn.IsExistSettingsRoom(rosterNameArray[4]));

                test.GeneralMethods.WaitForElement(By.CssSelector("[class=\"drawer\"]"), 30, "Fail to find students info on roster page");

                //RealTime-Checkout            
                test.CheckIn.RealTimeDeleteIndividualInstance(activityName, individualId, individualTypeId, rosterNameArray[4]);
                Thread.Sleep(5 * 1000);
                test.GeneralMethods.WaitForElement(By.CssSelector("[class=\"drawer\"]"), 30, "Fail to find students info on roster page again");

                //Open setitngs again
                test.CheckIn.ClickIconMenu();

                //Check classroom rosterNameArray[4] disappear on settings.
                Assert.IsFalse(test.CheckIn.IsExistSettingsRoom(rosterNameArray[4]));
            }
            finally
            {
                test.CheckIn.RealTimeDeleteIndividualInstance(activityName, individualId, individualTypeId, rosterNameArray[4]);
            }

            //Logout
            test.CheckIn.LogoutWebDriver();

        }
        [Test(Order = 7), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("FO-7662")]
        public void CheckInTeacher_RealTime_NoDuplicateIndividualsOnRoster_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string individualId = Convert.ToString(this._sql.People_Individuals_FetchIDByName(churchId, "GraceAutoSpouse Zhang"));
            String[] individualNameArray = { "GraceAutoSpouse Zhang" };
            string individualTypeId = "1";//student

            //Login
            test.CheckIn.LoginMobileWebDriver();
            //Open setitngs
            test.CheckIn.ClickIconMenu();
            test.CheckIn.ClickSettingsRoom(rosterToCheckIn);

            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            try
            {
                //Checkin a student in db
                String[] attendenceids = test.CheckIn.CheckInMultipleIndividuals(activityName, individualNameArray, 16729, currentTimzoneTime.ToString(), individualTypeId, rosterToCheckIn);
                //Notice front end
                test.CheckIn.RealtimeNoticeCheckin(churchId, activityName, individualId, "1", rosterToCheckIn, null, null, "0");
                //wait for signalR
                Thread.Sleep(10 * 1000);
                int actualAppearTimes = test.CheckIn.StudentAppearTimesOnRoster("GraceAutoSpouse", null, "Zhang");
                if (actualAppearTimes >= 2)
                {
                    Assert.Fail("Appear times of 'GraceAutoSpouse Zhang' on roster is more than 2 times, the bug FO-7662 still exists!");
                }               
                //RealTime-Checkout
                test.CheckIn.RealTimeDeleteIndividualInstance(activityName, individualId, individualTypeId, rosterToCheckIn);
                Thread.Sleep(5 * 1000);
               
                //Logout
                test.CheckIn.LogoutWebDriver();
            }
            finally
            {
                test.CheckIn.RealTimeDeleteIndividualInstance(activityName, individualId, individualTypeId, rosterToCheckIn);
            }

        }
    }
}
