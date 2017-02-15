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
    class SelfCheckIn_Checkin : SelfCheckInBaseWebDriver
    {
        [Test(Order = 1), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies No Next button when no one checkin")]
        public void SelfCheckIn_CheckIn_CheckNoNextbuttonWhenNoSelectAnyOne()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Delete all Self-Checked-in-record 
            test.SQL.Ministry_ActivitySchedules_DeleteAllSelfCheckedIn(churchId, activityName);
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();
            bool noNextflag = test.SelfCheckIn.IsHaveNext();
            Assert.IsFalse(noNextflag);
            //Logout
            test.SelfCheckIn.LogoutWebDriver();
        }
        [Test(Order = 2), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies Can Checkin a Volunteer Assignment")]
        public void SelfCheckIn_CheckIn_CheckCheckinOneAssignmentVolunteer()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Delete the person checkin record 
            test.SQL.Ministry_ActivitySchedules_DeleteIndividualSelfCheckedIn(churchId, activityName, "GraceAutoHousehold Zhang");
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();
            //Step1:Check a assignment of a individual
            test.SelfCheckIn.CheckOnAssignment("GraceAutoHousehold Zhang", SelfCheckInBaseWebDriver.activityName);
            //Step2:Click Next to complete Checkin
            test.SelfCheckIn.ClickNextAndWaitForLoaded();
            //Step3:Check Checkin record in table ChmActivity.dbo.INDIVIDUAL_INSTANCE,simply
            Thread.Sleep(3000);
            bool isSuccChekedin = test.SQL.Ministry_ActivitySchedules_IsSuccSelfCheckedIn(churchId, activityName, "GraceAutoHousehold Zhang");
            Assert.IsTrue(isSuccChekedin);
            //Logout
            test.SelfCheckIn.LogoutWebDriver();

            string teacherEmail = "graceautohousehold@163.com";
            string teacherPassword = "Active@123";
            string individualId = Convert.ToString(test.SQL.People_Individuals_FetchIDByEmail(churchId, teacherEmail));
            try
            {
                //Login and validate log in successful
                test.CheckIn.LoginWebDriver(teacherEmail, teacherPassword, test.SelfCheckIn.ChurchCode);

                test.CheckIn.ClickIconMenu();
                test.CheckIn.ClickSettingsRoom(rosterNameArray[0]);

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
        [Test(Order = 3), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies Can Checkin a Participant Assignment")]
        public void SelfCheckIn_CheckIn_CheckCheckinOneAssignmentParticipant()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Delete the person checkin record 
            test.SQL.Ministry_ActivitySchedules_DeleteIndividualSelfCheckedIn(churchId, activityName, "GraceAutoHousehold Zhang");
            test.SQL.Ministry_ActivitySchedules_DeleteIndividualSelfCheckedIn(churchId, activityName, "GraceAutoChildYong Zhang");
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();
            //Step1:Check a assignment of a teacher
            test.SelfCheckIn.CheckOnAssignment("GraceAutoHousehold Zhang", SelfCheckInBaseWebDriver.activityName);
            //Step2:Click Next to complete Checkin
            test.SelfCheckIn.ClickNextAndWaitForLoaded();
            //Step3:Check Checkin record in table ChmActivity.dbo.INDIVIDUAL_INSTANCE,simply
            Thread.Sleep(3000);
            bool isSuccChekedin = test.SQL.Ministry_ActivitySchedules_IsSuccSelfCheckedIn(churchId, activityName, "GraceAutoHousehold Zhang");
            Assert.IsTrue(isSuccChekedin);
            //Logout
            test.SelfCheckIn.LogoutWebDriver();

            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();
            //Step1:Check a assignment of a individual
            test.SelfCheckIn.CheckOnAssignment("GraceAutoChildYong Zhang", SelfCheckInBaseWebDriver.activityName);
            //Step2:Click Next to complete Checkin
            test.SelfCheckIn.ClickNextAndWaitForLoaded();
            //Step3:Check Checkin record in table ChmActivity.dbo.INDIVIDUAL_INSTANCE,simply
            Thread.Sleep(3000);
             isSuccChekedin = test.SQL.Ministry_ActivitySchedules_IsSuccSelfCheckedIn(churchId, activityName, "GraceAutoChildYong Zhang");
            Assert.IsTrue(isSuccChekedin);
            //Logout
            test.SelfCheckIn.LogoutWebDriver();

            string teacherEmail = "graceautohousehold@163.com";
            string teacherPassword = "Active@123";
            string individualId = Convert.ToString(test.SQL.People_Individuals_FetchIDByEmail(churchId, teacherEmail));
            try
            {
                //Login and validate log in successful
                test.CheckIn.LoginWebDriver(teacherEmail, teacherPassword, test.SelfCheckIn.ChurchCode);

                test.CheckIn.ClickIconMenu();
                test.CheckIn.ClickSettingsRoom(rosterNameArray[0]);
                String expectstatusInfo = "On Site at " + test.CheckIn.getExpectOnSiteTime(churchId, activityName, "GraceAutoChildYong Zhang");
                String statusInfo = test.CheckIn.GetStatusAndTimeByStudentName("GraceAutoChildYong", null, "Zhang");
                Assert.AreEqual(expectstatusInfo, statusInfo);
                //Logout
                test.CheckIn.LogoutWebDriver();

            }
            finally
            {
                //Delete the student checkin record 
                test.SQL.Ministry_ActivitySchedules_DeleteIndividualSelfCheckedIn(churchId, activityName, "GraceAutoHousehold Zhang");
                test.SQL.Ministry_ActivitySchedules_DeleteIndividualSelfCheckedIn(churchId, activityName, "GraceAutoChildYong Zhang");
            }

        }  
        [Test(Order = 4), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("Verifies Item Tag Title")]
        public void SelfCheckIn_ItemTag_CheckTitle()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];           
            //Delete the person checkin record 
            test.SQL.Ministry_ActivitySchedules_DeleteIndividualSelfCheckedIn(churchId, activityName, "GraceAutoAssignRuleTester1 Zhang");
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();
            //Step1:Check a assignment of a individual
            test.SelfCheckIn.CheckOnAssignment("GraceAutoAssignRuleTester1 Zhang", SelfCheckInBaseWebDriver.activityName);
            //Step2:Click Next to complete Checkin and jump to item-tag page
            test.SelfCheckIn.ClickNextAndWaitForLoaded();           
            String actualTitleName = test.SelfCheckIn.GetItemTagTitle();
            Assert.AreEqual("Please select number of item tags to print for the following individual(s):", actualTitleName);
            //Logout
            test.SelfCheckIn.LogoutWebDriver();
        }

        [Test(Order = 5), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("Verifies Item Tag Number")]
        public void SelfCheckIn_ItemTag_CheckNumber()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Delete the person checkin record 
            test.SQL.Ministry_ActivitySchedules_DeleteIndividualSelfCheckedIn(churchId, activityName, "GraceAutoAssignRuleTester1 Zhang");
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();
            //Step1:Check a assignment of a individual
            test.SelfCheckIn.CheckOnAssignment("GraceAutoAssignRuleTester1 Zhang", SelfCheckInBaseWebDriver.activityName);
            //Step2:Click Next to complete Checkin and jump to item-tag page
            test.SelfCheckIn.ClickNextAndWaitForLoaded();
            //Step3:Set Item-tag Number
            int itemTagNumber = 3;
            test.SelfCheckIn.AddItemTagNumberByIndividualName("GraceAutoAssignRuleTester1 Zhang",itemTagNumber);
            String actualItemTagNumbber = test.SelfCheckIn.GetItemTagNumberByIndividualName("GraceAutoAssignRuleTester1 Zhang");
            Assert.AreEqual(Convert.ToString(itemTagNumber), actualItemTagNumbber);
            //Logout
            test.SelfCheckIn.LogoutWebDriver();
        } 
    }
}
