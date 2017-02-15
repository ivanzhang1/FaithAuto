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

namespace FTTests.ImportInfo
{
    [TestFixture, Parallelizable]
    class CheckInTeacher_InportInfo_Mobile : CheckInBaseWebDriver
    {
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies Security Code right")]
        public void CheckInTeacher_Login_Verify_SecurityCode_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            try
            {
                test.SQL.People_Individuals_UpdateTagCode(churchId, activityName, "Emma Kemp", "cod");
                //Login
                test.CheckIn.LoginRosterMobileWebDriver(rosterNameArray[0]);

                String securityCode = test.CheckIn.GetMobileSecurityCodeByStudentName("Emma", null, "Kemp");
                Assert.AreEqual("cod", securityCode);
            }
            finally
            {
                //restore securityCode
            }
            //Logout
            test.CheckIn.LogoutWebDriver();

        }
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies Security Birthday right")]
        public void CheckInTeacher_Login_Verify_Birthday_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            try
            {
                String expectBirthdayInfo = "Emma has a birthday on " + test.CheckIn.getExpectBirthdayMDInfo(churchId);
                test.CheckIn.UpdateBirthdayForCheckinTeacher(churchId, "Emma Kemp");
                //Login
                test.CheckIn.LoginRosterMobileWebDriver(rosterNameArray[0]);
                String birthdayInfo = test.CheckIn.getBirthdayByStudentName("Emma", null, "Kemp");
                Assert.AreEqual(expectBirthdayInfo, birthdayInfo);
            }
            finally
            {
                //restore birthday
            }
            //Logout
            test.CheckIn.LogoutWebDriver();
        }
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies Security TagComment right")]
        public void CheckInTeacher_Login_Verify_TagComment_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            try
            {
                test.SQL.People_Individuals_UpdateTagCommentByName(churchId, "Emma Kemp", "sssss");
                //Login
                test.CheckIn.LoginRosterMobileWebDriver(rosterNameArray[0]);
                String tagCommentInfo = test.CheckIn.GetTagCommentByStudentName("Emma", null, "Kemp");
                Assert.AreEqual("sssss", tagCommentInfo);
            }
            finally
            {
                //Keep the value in DB
                //test.SQL.People_Individuals_UpdateTagCommentByName(churchId, "Emma Kemp", "");
            }
            //Logout
            test.CheckIn.LogoutWebDriver();
            
        }
        [Test, RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("Verifies Security LastAttendance right")]
        public void CheckInTeacher_Login_Verify_LastAttendance_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            String expectLastAttendDateInfoInfo = test.CheckIn.getExpectLastAttendInfo(churchId);
            test.CheckIn.CreateLastAttendance(churchId, activityName, "Emma Kemp", rosterNameArray[2]);
            //Login
            test.CheckIn.LoginRosterMobileWebDriver(rosterNameArray[0]);
            String lastAttendDateInfo = test.CheckIn.GetLastAttendanceByStudentName("Emma", null, "Kemp");
            Assert.AreEqual(expectLastAttendDateInfoInfo, lastAttendDateInfo);
            //Logout
            test.CheckIn.LogoutWebDriver();
        }
    }
}
