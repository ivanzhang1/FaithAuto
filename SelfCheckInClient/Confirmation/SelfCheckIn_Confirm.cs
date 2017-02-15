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
    class SelfCheckIn_Confirm : SelfCheckInBaseWebDriver
    {

        [Test(Order = 1), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies Assignment Title right")]
        public void SelfCheckIn_Confirm_CheckTitle()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Delete the person checkin record 
            test.SQL.Ministry_ActivitySchedules_DeleteIndividualSelfCheckedIn(churchId, activityName, "GraceAutoChildOld Zhang");
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();
            //Step1:Check a assignment of a individual
            test.SelfCheckIn.CheckOnAssignment("GraceAutoChildOld Zhang", SelfCheckInBaseWebDriver.activityName);
            //Step2:Click Next to jump to item-tag page
            test.SelfCheckIn.ClickNextAndWaitForLoaded();
            //Step3:Click Finish to jump to Confirm page
            test.SelfCheckIn.ClickFinishAndWaitForLoaded();

            String confirmTitle = test.SelfCheckIn.GetConfirmationTitle();
            Assert.AreEqual("Bring your QR code to a print station near you to get your security tags.", confirmTitle);
            //Logout
            test.SelfCheckIn.LogoutWebDriver();
        }
        [Test(Order = 2), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies Confirm Title right")]
        public void SelfCheckIn_Confirm_CheckAssignmentTitle()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Delete the person checkin record 
            test.SQL.Ministry_ActivitySchedules_DeleteIndividualSelfCheckedIn(churchId, activityName, "GraceAutoChildOld Zhang");
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();
            //Step1:Check a assignment of a individual
            test.SelfCheckIn.CheckOnAssignment("GraceAutoChildOld Zhang", SelfCheckInBaseWebDriver.activityName);
            //Step2:Click Next to jump to item-tag page
            test.SelfCheckIn.ClickNextAndWaitForLoaded();
            //Step3:Click Finish to jump to Confirm page
            test.SelfCheckIn.ClickFinishAndWaitForLoaded();

            String confirmAssignmentTitleName = test.SelfCheckIn.GetConfirmedAssignmentTitleByIndividualNameandActivity("GraceAutoChildOld Zhang", "Activity-SelfCheckIn-T3");
            Assert.AreEqual("Activity-SelfCheckIn-T3 - Roster__001", confirmAssignmentTitleName);
            //Logout
            test.SelfCheckIn.LogoutWebDriver();
        }
        [Test(Order = 3), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies Confirm Assignment Sign right")]
        public void SelfCheckIn_Confirm_CheckAssignmentSign()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Delete the person checkin record 
            test.SQL.Ministry_ActivitySchedules_DeleteIndividualSelfCheckedIn(churchId, activityName, "GraceAutoChildOld Zhang");
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();
            //Step1:Check a assignment of a individual
            test.SelfCheckIn.CheckOnAssignment("GraceAutoChildOld Zhang", SelfCheckInBaseWebDriver.activityName);
            //Step2:Click Next to jump to item-tag page
            test.SelfCheckIn.ClickNextAndWaitForLoaded();
            //Step3:Click Finish to jump to Confirm page
            test.SelfCheckIn.ClickFinishAndWaitForLoaded();

            String confirmAssignmentSign = test.SelfCheckIn.GetConfirmedAssignmentSignByIndividualNameandActivity("GraceAutoChildOld Zhang", "Activity-SelfCheckIn-T3");
            Assert.AreEqual("V", confirmAssignmentSign);
            //Logout
            test.SelfCheckIn.LogoutWebDriver();
        }
        [Test(Order = 4), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies Confirm Assignment Time right")]
        public void SelfCheckIn_Confirm_CheckAssignmentTime()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Delete the person checkin record 
            test.SQL.Ministry_ActivitySchedules_DeleteIndividualSelfCheckedIn(churchId, activityName, "GraceAutoChildOld Zhang");
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();
            //Step1:Check a assignment of a individual
            test.SelfCheckIn.CheckOnAssignment("GraceAutoChildOld Zhang", SelfCheckInBaseWebDriver.activityName);
            //Step2:Click Next to jump to item-tag page
            test.SelfCheckIn.ClickNextAndWaitForLoaded();
            //Step3:Click Finish to jump to Confirm page
            test.SelfCheckIn.ClickFinishAndWaitForLoaded();

            String actualConfirmAssignmentTime = test.SelfCheckIn.GetConfirmedAssignmentTimeByIndividualNameandActivity("GraceAutoChildOld Zhang", "Activity-SelfCheckIn-T3");
            String expectConfirmAssignmentTime = test.SelfCheckIn.GetExpectAssignmentTimeByActivityTime(churchId, activityName);

            Assert.AreEqual(expectConfirmAssignmentTime, actualConfirmAssignmentTime);
            //Logout
            test.SelfCheckIn.LogoutWebDriver();
        }
        [Test(Order = 5), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies Confirm Guset Sign right")]
        public void SelfCheckIn_Confirm_CheckGuestSign()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Delete the person checkin record 
            test.SQL.Ministry_ActivitySchedules_DeleteIndividualSelfCheckedIn(churchId, activityName, "GraceAutoVisitor1 Zhang");
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();
            //Step1:Check a assignment of a individual
            test.SelfCheckIn.CheckOnAssignment("GraceAutoVisitor1 Zhang", SelfCheckInBaseWebDriver.activityName);
            //Step2:Click Next to jump to item-tag page
            test.SelfCheckIn.ClickNextAndWaitForLoaded();
            //Step3:Click Finish to jump to Confirm page
            test.SelfCheckIn.ClickFinishAndWaitForLoaded();

            String guestSign = test.SelfCheckIn.GetConfirmedGuestSignByIndividualName("GraceAutoVisitor1 Zhang");
            Assert.AreEqual("G", guestSign);
            //Logout
            test.SelfCheckIn.LogoutWebDriver();
        }
        [Test(Order = 6), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies Confirm Item Tag Info right")]
        public void SelfCheckIn_Confirm_CheckItemTag()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Delete the person checkin record 
            test.SQL.Ministry_ActivitySchedules_DeleteIndividualSelfCheckedIn(churchId, activityName, "GraceAutoChildOld Zhang");
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();
            //Step1:Check a assignment of a individual
            test.SelfCheckIn.CheckOnAssignment("GraceAutoChildOld Zhang", SelfCheckInBaseWebDriver.activityName);
            //Step2:Click Next to jump to item-tag page
            test.SelfCheckIn.ClickNextAndWaitForLoaded();
            //Step3:Set Item-tag Number
            int itemTagNumber = 3;
            test.SelfCheckIn.AddItemTagNumberByIndividualName("GraceAutoChildOld Zhang", itemTagNumber);            
            //Step4:Click Finish to jump to Confirm page
            test.SelfCheckIn.ClickFinishAndWaitForLoaded();

            String itemTagInfo = test.SelfCheckIn.GetConfirmedItemTagInfoByIndividualName("GraceAutoChildOld Zhang");
            Assert.AreEqual("3 Item Tags", itemTagInfo);
            //Logout
            test.SelfCheckIn.LogoutWebDriver();
        }

    }
}
