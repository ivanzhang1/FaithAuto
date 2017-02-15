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
    class CheckInTeacher_Login_Mobile : CheckInBaseWebDriver
    {
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies login in sucess")]
        public void CheckInTeacher_Login_RightUserPwd_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.CheckIn.LoginMobileWebDriver();
            //Click Icon-User
            test.CheckIn.ClickIconMenu();
            //Logout
            test.CheckIn.LogoutWebDriver();
            
        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies login in fails when the church code is wrong")]
        public void CheckInTeacher_Login_FailWhenChurchWrong_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            //Login and validate log in fails
            test.CheckIn.LoginMobileWebDriver(test.CheckIn.CheckInUsername, test.CheckIn.CheckInPassword, "DCWrong", false);

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies login in fails when user name is wrong")]
        public void CheckInTeacher_Login_FailWhenUserNameWrong_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            //Login and validate log in fails
            test.CheckIn.LoginMobileWebDriver(test.CheckIn.CheckInUsername + ".cn", test.CheckIn.CheckInPassword, test.CheckIn.ChurchCode, false);

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies login in fails when user is not checked in")]
        public void CheckInTeacher_Login_FailWhenUserUncheckedIn_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            //Login and validate log in fails
            test.CheckIn.LoginMobileWebDriver("graceteacher1@163.com", test.CheckIn.CheckInPassword, test.CheckIn.ChurchCode, false);

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies login in fails when user is checked in as a student")]
        public void CheckInTeacher_Login_FailWhenUserCheckedInAsStudent_Mobile()
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
                test.CheckIn.LoginMobileWebDriver("ft.autotester@gmail.com", "FT4life!", test.CheckIn.ChurchCode, false);
            }
            finally
            {
                test.SQL.Ministry_ActivitySchedules_DeleteIndividualCheckedIn(churchId, activityName, studentlName);
            }


        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies login in sucess if password of the teacher account has space")]
        public void CheckInTeacher_Login_SpaceInUserPwd_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string newTeacherEmail = "group.spaceinpassword@gmail.com";

            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            String individualId = Convert.ToString(base.SQL.People_Individuals_FetchIDByEmail(churchId, newTeacherEmail));

            test.CheckIn.CheckInATeacher(churchId, activityName, individualId, rosterToCheckIn, currentTimzoneTime);

            try
            {
                //Login and validate log in succeeds
                test.CheckIn.LoginMobileWebDriver(newTeacherEmail, "BM.Admin 09", test.CheckIn.ChurchCode);
                test.CheckIn.LogoutWebDriver();
            }
            finally
            {
                test.SQL.Ministry_ActivitySchedules_DeleteIndividualCheckedIn(churchId, activityName, Convert.ToInt32(individualId));
            }

        }

    }
}
