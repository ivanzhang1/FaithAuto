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

namespace FTTests.SelfCheckIn
{
    [TestFixture, Parallelizable]
    class SelfCheckIn_Assignments : SelfCheckInBaseWebDriver
    {
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies Assignment Title right")]
        public void SelfCheckIn_Assignments_CheckAssignmentTitle()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();
            String assignmentTitleName = test.SelfCheckIn.GetAssignmentTitleByIndividualNameandActivity("GraceAutoChildOld Zhang", "Activity-SelfCheckIn-T3");
            Assert.AreEqual("Activity-SelfCheckIn-T3 - Roster__001", assignmentTitleName);
            //Logout
            test.SelfCheckIn.LogoutWebDriver();
        }
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies Assignment Sign right")]
        public void SelfCheckIn_Assignments_CheckAssignmentSign()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();
            String assignmentSign = test.SelfCheckIn.GetAssignmentSignByIndividualNameandActivity("GraceAutoChildOld Zhang", "Activity-SelfCheckIn-T3");
            Assert.AreEqual("V", assignmentSign);
            //Logout
            test.SelfCheckIn.LogoutWebDriver();
        }
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies Assignment Time right")]
        public void SelfCheckIn_Assignments_CheckAssignmentTime()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();
            TestLog.WriteLine("Check the assignment time display");
            String actualAssignmentTime = test.SelfCheckIn.GetAssignmentTimeByIndividualNameandActivity("GraceAutoChildOld Zhang", "Activity-SelfCheckIn-T3");
            String expectAssignmentTime = test.SelfCheckIn.GetExpectAssignmentTimeByActivityTime(churchId, activityName);
            
            Assert.AreEqual(expectAssignmentTime, actualAssignmentTime);
            //Logout
            test.SelfCheckIn.LogoutWebDriver();
        }
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies No Assignment Info right")]
        public void SelfCheckIn_Assignments_CheckNoAssignment()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();
            String actualNoAssignmentInfo = test.SelfCheckIn.GetNoAssignmentInfoByIndividualName("GraceAutoSpouse Zhang");
            String expectNoAssignmentInfo = "GraceAutoSpouse currently has no assignments, please find a church volunteer for further assistance.";
            //TODO
            Assert.AreEqual(expectNoAssignmentInfo, actualNoAssignmentInfo);
            //Logout
            test.SelfCheckIn.LogoutWebDriver();
        }
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies No Assignment Rule")]
        public void SelfCheckIn_Assignments_CheckAssignmentRule()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();
            String assignmentTitleName = test.SelfCheckIn.GetAssignmentTitleByIndividualNameandActivity("GraceAutoAssignRuleTester1 Zhang", "Activity-SelfCheckIn-T3");
            Assert.AreEqual("Activity-SelfCheckIn-T3 - Roster__003", assignmentTitleName);
            //Logout
            test.SelfCheckIn.LogoutWebDriver();
        }
       
    }
}
