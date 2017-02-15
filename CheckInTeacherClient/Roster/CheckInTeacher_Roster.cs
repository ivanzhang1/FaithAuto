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
    class CheckInTeacher_Roster : CheckInBaseWebDriver
    {
        [Test(Order = 1), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("Verifies roster page display when no student is checked in")]
        public void CheckInTeacher_Roster_DisplayWhenNoStudent()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string teacherEmail = "ft.autotester@gmail.com";
            string teacherPassword = "FT4life!";

            // Check in a teacher
            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            TestLog.WriteLine("Check in a teacher individual: " + teacherEmail);
            string individualId = Convert.ToString(test.SQL.People_Individuals_FetchIDByEmail(churchId, teacherEmail));

            test.CheckIn.CheckInATeacher(churchId, activityName, individualId, rosterNameArray[2], currentTimzoneTime);

            try
            {
                //Login and validate log in successful
                test.CheckIn.LoginWebDriver(teacherEmail, teacherPassword, test.CheckIn.ChurchCode);

                test.CheckIn.ClickIconMenu();
                test.CheckIn.ClickSettingsRoom(rosterNameArray[2]);

                //Check the no student content display on the Roster page
                test.GeneralMethods.WaitForElementVisible(By.CssSelector("[class=\"no-students\"]"), 30, "Fail to find the no student content on roster page");
                Assert.AreEqual("No students have checked into this class yet. Students will appear automatically as they arrive.", 
                    test.Driver.FindElementByCssSelector("[class=\"no-students\"]").Text.Trim(), "The no student content is wrong");
                
            }
            finally
            {
                test.SQL.Ministry_ActivitySchedules_DeleteIndividualCheckedIn(churchId, activityName, Convert.ToInt32(individualId));
            }
            
        }

        [Test(Order = 2), RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies roster page display when switch to a class room with no student checked in")]
        public void CheckInTeacher_Roster_DisplaySwitchClassRoomNoStudent()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string teacherEmail = "ft.autotester@gmail.com";
            string teacherPassword = "FT4life!";

            // Check in a teacher to Class 3
            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            TestLog.WriteLine("Check in a teacher individual: " + teacherEmail + " to class 3");
            string individualId = Convert.ToString(test.SQL.People_Individuals_FetchIDByEmail(churchId, teacherEmail));

            test.CheckIn.CheckInATeacher(churchId, activityName, individualId, rosterNameArray[2], currentTimzoneTime);

            // Check in the teacher to Class 2
            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            TestLog.WriteLine("Check in the teacher individual: " + teacherEmail + " to class 2");

            test.CheckIn.CheckInATeacher(churchId, activityName, individualId, rosterNameArray[1], currentTimzoneTime);

            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            String[] individualNameArray = { "Chris Kemp" };

            TestLog.WriteLine("Check in one student individual to class 2");
            test.CheckIn.CheckInMultipleIndividuals(activityName, individualNameArray, 16729, currentTimzoneTime.ToString(), "1", rosterNameArray[1]);

            try
            {
                //Login and validate log in successful
                test.CheckIn.LoginWebDriver(teacherEmail, teacherPassword, test.CheckIn.ChurchCode);

                test.CheckIn.ClickIconMenu();
                test.CheckIn.ClickSettingsRoom(rosterNameArray[2]);

                //Check the no student content display on the Roster page
                test.GeneralMethods.WaitForElementVisible(By.CssSelector("[class=\"no-students\"]"), 30, "Fail to find the no student content on roster page");
                Assert.AreEqual("No students have checked into this class yet. Students will appear automatically as they arrive.",
                    test.Driver.FindElementByCssSelector("[class=\"no-students\"]").Text.Trim(), "The no student content is wrong");

            }
            finally
            {
                test.SQL.Ministry_ActivitySchedules_DeleteIndividualCheckedIn(churchId, activityName, Convert.ToInt32(individualId));
                test.SQL.Ministry_ActivitySchedules_DeleteIndividualCheckedIn(churchId, activityName, individualNameArray[0]);
            }

        }

        [Test(Order = 3), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("Verifies Status-On Site Initial state")]
        public void CheckInTeacher_Roster_OnSiteInitialstate()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.CheckIn.LoginRosterWebDriver(rosterNameArray[5]);

            String expectstatusInfo = "On Site at " + test.CheckIn.getExpectOnSiteTime(churchId, "ActivityCheckIn-T3", "Mary Martin");
            String statusInfo = test.CheckIn.GetStatusAndTimeByStudentName("Mary", null, "Martin"); 
            Assert.AreEqual(expectstatusInfo, statusInfo);
            //Logout
            test.CheckIn.LogoutWebDriver();
        }
        [Test(Order = 4), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("Verifies Status-In ")]
        public void CheckInTeacher_Roster_CheckIn()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.CheckIn.LoginRosterWebDriver(rosterNameArray[5]);

            test.CheckIn.UndoByStudentName("Mary", null, "Martin");
            Thread.Sleep(5 * 1000);
            test.CheckIn.CheckInByStudentName("Mary", null, "Martin");
            Thread.Sleep(5 * 1000);
            String expectstatusInfo = "In at " + test.CheckIn.getExpectInTime(churchId, "ActivityCheckIn-T3", "Mary Martin");
            String statusInfo = test.CheckIn.GetStatusAndTimeByStudentName("Mary", null, "Martin");
            Assert.AreEqual(expectstatusInfo, statusInfo);
            //Logout
            test.CheckIn.LogoutWebDriver();
        }
        [Test(Order = 5), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("Verifies Status-Out ")]
        public void CheckInTeacher_Roster_CheckOut()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.CheckIn.LoginRosterWebDriver(rosterNameArray[5]);

            test.CheckIn.UndoByStudentName("Mary", null, "Martin");
            Thread.Sleep(5 * 1000);
            test.CheckIn.CheckOutByStudentName("Mary", null, "Martin");
            Thread.Sleep(5 * 1000);
            String expectstatusInfo = "Out at " + test.CheckIn.getExpectOutTime(churchId, "ActivityCheckIn-T3", "Mary Martin");
            String statusInfo = test.CheckIn.GetStatusAndTimeByStudentName("Mary", null, "Martin");
            Assert.AreEqual(expectstatusInfo, statusInfo);
            //Logout
            test.CheckIn.LogoutWebDriver();
        }
        [Test(Order = 6), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("Verifies Status-out to In ")]
        public void CheckInTeacher_Roster_CheckOutTocheckIn()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.CheckIn.LoginRosterWebDriver(rosterNameArray[5]);

            test.CheckIn.UndoByStudentName("Mary", null, "Martin");
            Thread.Sleep(5 * 1000);
            test.CheckIn.CheckInByStudentName("Mary", null, "Martin");
            Thread.Sleep(5 * 1000);
            String expectstatusInfo = "In at " + test.CheckIn.getExpectInTime(churchId, "ActivityCheckIn-T3", "Mary Martin");
            Thread.Sleep(60 * 1000);
            test.CheckIn.CheckOutByStudentName("Mary", null, "Martin");
            Thread.Sleep(60 * 1000);
            test.CheckIn.CheckInByStudentName("Mary", null, "Martin");
            String statusInfo = test.CheckIn.GetStatusAndTimeByStudentName("Mary", null, "Martin");
            Assert.AreEqual(expectstatusInfo, statusInfo);
            //Logout
            test.CheckIn.LogoutWebDriver();
        }
        [Test(Order = 7), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("Verifies Status-undo ")]
        public void CheckInTeacher_Roster_Undo()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.CheckIn.LoginRosterWebDriver(rosterNameArray[5]);

            String expectstatusInfo = "On Site at " + test.CheckIn.getExpectOnSiteTime(churchId, "ActivityCheckIn-T3", "Mary Martin");
            test.CheckIn.UndoByStudentName("Mary", null, "Martin");
            Thread.Sleep(5 * 1000);
            test.CheckIn.CheckInByStudentName("Mary", null, "Martin");        
            Thread.Sleep(5 * 1000);
            test.CheckIn.CheckOutByStudentName("Mary", null, "Martin");
            Thread.Sleep(5 * 1000);
            test.CheckIn.UndoByStudentName("Mary", null, "Martin");
            String statusInfo = test.CheckIn.GetStatusAndTimeByStudentName("Mary", null, "Martin");
            Assert.AreEqual(expectstatusInfo, statusInfo);
            //Logout
            test.CheckIn.LogoutWebDriver();
        }
        [Test(Order = 8), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("Verifies ChurchName ")]
        public void CheckInTeacher_Roster_ChurchName()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            String expectChurchName = test.SQL.Ministry_Church_FetchName(churchId);
            //Login
            test.CheckIn.LoginWebDriver();      
            String churchName = test.CheckIn.GetChurchName();
            Assert.AreEqual(expectChurchName, churchName);
            //Logout
            test.CheckIn.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("Verifies participant new icon display right")]
        public void CheckInTeacher_Login_Verify_ParticipantNew()
        {
            String[] students = { "Mady Kou" };
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.CheckIn.DeleteLastAttendance(churchId, students[0]);
            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));//get new time
            string checkInTime = currentTimzoneTime.ToString();
            test.CheckIn.CheckInMultipleIndividuals(activityName, students, 16729, checkInTime, "1", rosterToCheckIn);
            //Login
            test.CheckIn.LoginRosterWebDriver(rosterToCheckIn);
            int index = test.CheckIn.GetListIndexByStudentFullName(students[0]);
            // Since the index is retrieved by css finder, it starts with 0. But I will use it in XPath below then I have to plus 1 to it.
            index += 1;
            IWebElement ele = test.Driver.FindElement(By.XPath(String.Format("//ion-item[@class='no-pad item'][{0}]/div/div[2]/div/div", index)));
            Assert.IsTrue(ele.GetAttribute("class").Equals("new"), "New icon isn't display on the student correctly");
            //Logout
            test.CheckIn.LogoutWebDriver();
        }


        [Test(Order = 9), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("Verifies roster page display when no student is checked in")]
        public void CheckInTeacher_Roster_DisplayWhenNoStudent_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string teacherEmail = "ft.autotester@gmail.com";
            string teacherPassword = "FT4life!";

            // Check in a teacher
            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            TestLog.WriteLine("Check in a teacher individual: " + teacherEmail);
            string individualId = Convert.ToString(test.SQL.People_Individuals_FetchIDByEmail(churchId, teacherEmail));

            test.CheckIn.CheckInATeacher(churchId, activityName, individualId, rosterNameArray[2], currentTimzoneTime);

            try
            {
                //Login and validate log in successful
                test.CheckIn.LoginMobileWebDriver(teacherEmail, teacherPassword, test.CheckIn.ChurchCode);

                test.CheckIn.ClickIconMenu();
                test.CheckIn.ClickSettingsRoom(rosterNameArray[2]);

                //Check the no student content display on the Roster page
                test.GeneralMethods.WaitForElementVisible(By.CssSelector("[class=\"no-students\"]"), 30, "Fail to find the no student content on roster page");
                Assert.AreEqual("No students have checked into this class yet. Students will appear automatically as they arrive.",
                    test.Driver.FindElementByCssSelector("[class=\"no-students\"]").Text.Trim(), "The no student content is wrong");

            }
            finally
            {
                test.SQL.Ministry_ActivitySchedules_DeleteIndividualCheckedIn(churchId, activityName, Convert.ToInt32(individualId));
            }

        }

        [Test(Order = 10), RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies roster page display when switch to a class room with no student checked in")]
        public void CheckInTeacher_Roster_DisplaySwitchClassRoomNoStudent_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string teacherEmail = "ft.autotester@gmail.com";
            string teacherPassword = "FT4life!";

            // Check in a teacher to Class 3
            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            TestLog.WriteLine("Check in a teacher individual: " + teacherEmail + " to class 3");
            string individualId = Convert.ToString(test.SQL.People_Individuals_FetchIDByEmail(churchId, teacherEmail));

            test.CheckIn.CheckInATeacher(churchId, activityName, individualId, rosterNameArray[2], currentTimzoneTime);

            // Check in the teacher to Class 2
            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            TestLog.WriteLine("Check in the teacher individual: " + teacherEmail + " to class 2");

            test.CheckIn.CheckInATeacher(churchId, activityName, individualId, rosterNameArray[1], currentTimzoneTime);

            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            String[] individualNameArray = { "Chris Kemp" };

            TestLog.WriteLine("Check in one student individual to class 2");
            test.CheckIn.CheckInMultipleIndividuals(activityName, individualNameArray, 16729, currentTimzoneTime.ToString(), "1", rosterNameArray[1]);

            try
            {
                //Login and validate log in successful
                test.CheckIn.LoginMobileWebDriver(teacherEmail, teacherPassword, test.CheckIn.ChurchCode);

                test.CheckIn.ClickIconMenu();
                test.CheckIn.ClickSettingsRoom(rosterNameArray[2]);

                //Check the no student content display on the Roster page
                test.GeneralMethods.WaitForElementVisible(By.CssSelector("[class=\"no-students\"]"), 30, "Fail to find the no student content on roster page");
                Assert.AreEqual("No students have checked into this class yet. Students will appear automatically as they arrive.",
                    test.Driver.FindElementByCssSelector("[class=\"no-students\"]").Text.Trim(), "The no student content is wrong");

            }
            finally
            {
                test.SQL.Ministry_ActivitySchedules_DeleteIndividualCheckedIn(churchId, activityName, Convert.ToInt32(individualId));
                test.SQL.Ministry_ActivitySchedules_DeleteIndividualCheckedIn(churchId, activityName, individualNameArray[0]);
            }

        }

        [Test(Order = 11), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("Verifies Status-On Site Initial state")]
        public void CheckInTeacher_Roster_OnSiteInitialstate_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.CheckIn.LoginRosterMobileWebDriver(rosterNameArray[5]);

            String expectstatusInfo = "On Site at " + test.CheckIn.getExpectOnSiteTime(churchId, "ActivityCheckIn-T3", "Mary Martin");
            String statusInfo = test.CheckIn.GetStatusAndTimeByStudentName("Mary", null, "Martin");
            Assert.AreEqual(expectstatusInfo, statusInfo);
            //Logout
            test.CheckIn.LogoutWebDriver();
        }
        [Test(Order = 12), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("Verifies Status-In ")]
        public void CheckInTeacher_Roster_CheckIn_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.CheckIn.LoginRosterMobileWebDriver(rosterNameArray[5]);

            test.CheckIn.UndoByStudentName("Mary", null, "Martin");
            Thread.Sleep(5 * 1000);
            test.CheckIn.CheckInByStudentName("Mary", null, "Martin");
            Thread.Sleep(5 * 1000);
            String expectstatusInfo = "In at " + test.CheckIn.getExpectInTime(churchId, "ActivityCheckIn-T3", "Mary Martin");
            String statusInfo = test.CheckIn.GetStatusAndTimeByStudentName("Mary", null, "Martin");
            Assert.AreEqual(expectstatusInfo, statusInfo);
            //Logout
            test.CheckIn.LogoutWebDriver();
        }
        [Test(Order = 13), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("Verifies Status-Out ")]
        public void CheckInTeacher_Roster_CheckOut_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.CheckIn.LoginRosterMobileWebDriver(rosterNameArray[5]);

            test.CheckIn.UndoByStudentName("Mary", null, "Martin");
            Thread.Sleep(5 * 1000);
            test.CheckIn.CheckOutByStudentName("Mary", null, "Martin");
            Thread.Sleep(5 * 1000);
            String expectstatusInfo = "Out at " + test.CheckIn.getExpectOutTime(churchId, "ActivityCheckIn-T3", "Mary Martin");
            String statusInfo = test.CheckIn.GetStatusAndTimeByStudentName("Mary", null, "Martin");
            Assert.AreEqual(expectstatusInfo, statusInfo);
            //Logout
            test.CheckIn.LogoutWebDriver();
        }
        [Test(Order = 14), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("Verifies Status-out to In ")]
        public void CheckInTeacher_Roster_CheckOutTocheckIn_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.CheckIn.LoginRosterMobileWebDriver(rosterNameArray[5]);

            test.CheckIn.UndoByStudentName("Mary", null, "Martin");
            Thread.Sleep(5 * 1000);
            test.CheckIn.CheckInByStudentName("Mary", null, "Martin");
            Thread.Sleep(5 * 1000);
            String expectstatusInfo = "In at " + test.CheckIn.getExpectInTime(churchId, "ActivityCheckIn-T3", "Mary Martin");
            Thread.Sleep(60 * 1000);
            test.CheckIn.CheckOutByStudentName("Mary", null, "Martin");
            Thread.Sleep(60 * 1000);
            test.CheckIn.CheckInByStudentName("Mary", null, "Martin");
            String statusInfo = test.CheckIn.GetStatusAndTimeByStudentName("Mary", null, "Martin");
            Assert.AreEqual(expectstatusInfo, statusInfo);
            //Logout
            test.CheckIn.LogoutWebDriver();
        }
        [Test(Order = 15), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("Verifies Status-undo ")]
        public void CheckInTeacher_Roster_Undo_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.CheckIn.LoginRosterMobileWebDriver(rosterNameArray[5]);

            String expectstatusInfo = "On Site at " + test.CheckIn.getExpectOnSiteTime(churchId, "ActivityCheckIn-T3", "Mary Martin");
            test.CheckIn.UndoByStudentName("Mary", null, "Martin");
            Thread.Sleep(5 * 1000);
            test.CheckIn.CheckInByStudentName("Mary", null, "Martin");
            Thread.Sleep(5 * 1000);
            test.CheckIn.CheckOutByStudentName("Mary", null, "Martin");
            Thread.Sleep(5 * 1000);
            test.CheckIn.UndoByStudentName("Mary", null, "Martin");
            String statusInfo = test.CheckIn.GetStatusAndTimeByStudentName("Mary", null, "Martin");
            Assert.AreEqual(expectstatusInfo, statusInfo);
            //Logout
            test.CheckIn.LogoutWebDriver();
        }
        [Test, RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("Verifies ChurchName ")]
        public void CheckInTeacher_Roster_ChurchName_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            String expectChurchName = test.SQL.Ministry_Church_FetchName(churchId);
            //Login
            test.CheckIn.LoginMobileWebDriver();
            String churchName = test.CheckIn.GetChurchName();
            Assert.AreEqual(expectChurchName, churchName);
            //Logout
            test.CheckIn.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("Verifies participant new icon display right")]
        public void CheckInTeacher_Login_Verify_ParticipantNew_Mobile()
        {
            String[] students = { "Mady Kou" };
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.CheckIn.DeleteLastAttendance(churchId, students[0]);
            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));//get new time
            string checkInTime = currentTimzoneTime.ToString();
            test.CheckIn.CheckInMultipleIndividuals(activityName, students, 16729, checkInTime, "1", rosterToCheckIn);
            //Login
            test.CheckIn.LoginRosterMobileWebDriver(rosterToCheckIn);
            int index = test.CheckIn.GetListIndexByStudentFullName(students[0]);
            // Since the index is retrieved by css finder, it starts with 0. But I will use it in XPath below then I have to plus 1 to it.
            index += 1;
            IWebElement ele = test.Driver.FindElement(By.XPath(String.Format("//ion-item[@class='no-pad item'][{0}]/div/div[2]/div/div", index)));
            Assert.IsTrue(ele.GetAttribute("class").Equals("new"), "New icon isn't display on the student correctly");
            //Logout
            test.CheckIn.LogoutWebDriver();
        }
        
    }
}
