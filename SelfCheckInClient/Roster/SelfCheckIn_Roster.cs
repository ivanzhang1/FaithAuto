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
    class SelfCheckIn_Roster : SelfCheckInBaseWebDriver
    {
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies title name right on roster page")]
        public void SelfCheckIn_Roster_CheckHouseholdName()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();
            String actualTitleName = test.SelfCheckIn.GetRosterHouseholdName();
            Assert.AreEqual("GraceAutoHousehold and GraceAutoSpouse Zhang", actualTitleName);
            //Logout
            test.SelfCheckIn.LogoutWebDriver();
        }
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies individuals name and list on roster page by rule")]
        public void SelfCheckIn_Roster_CheckIndividualsNameAndOrderAndGoesbyname()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();
            String[] actualIndividualNames = test.SelfCheckIn.GetRosterIndividuals();
            String[] expectIndividualNames = { "GraceAutoHousehold Zhang", "GraceAutoSpouse Zhang", "GraceAutoChildOld Zhang", "GraceAutoChildYong Zhang", "GraceAutoOther Zhang", "GraceAutoAssignRuleTester2 Zhang", "GraceAutoVisitor1 Zhang", "GraceAutoVisitor2 'GoesbyName' Zhang", "GraceAutoAssignRuleTester1 Zhang" };
            Assert.AreEqual(expectIndividualNames, actualIndividualNames);
            //Logout
            test.SelfCheckIn.LogoutWebDriver();

        }
        [Test, RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("Verifies Guset Sign right")]
        public void SelfCheckIn_Roster_CheckGuestSign()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();
            String guestSign = test.SelfCheckIn.GetGuestSignByIndividualName("GraceAutoVisitor1 Zhang");
            Assert.AreEqual("G", guestSign);
            //Logout
            test.SelfCheckIn.LogoutWebDriver();
        }
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Grace Zhang")]
        [Description("Verifies whether to have linkToDetail button")]
        public void SelfCheckIn_Roster_ChecklinkToDetail()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();
            bool toDetailflag = test.SelfCheckIn.IslinkToDetailByIndividualName("GraceAutoHousehold Zhang");
            Assert.IsTrue(toDetailflag);
            //Logout
            test.SelfCheckIn.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("FO-7815 Custom Statuses do not show in Self-Mobile ")]
        public void SelfCheckIn_Roster_CheckIndividualsDisplayWithCustomStatus()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.SelfCheckIn.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "QAEUNLX0C2");
            test.SelfCheckIn.WaitForAssignmentLoaded();
            String[] actualIndividualNames = test.SelfCheckIn.GetRosterIndividuals();
            String[] expectIndividualNames = { "FT Tester", "Sub FT" };
            // Check the individual "Sub FT" which has customer status can display well
            Assert.AreEqual(expectIndividualNames, actualIndividualNames);
            //Logout
            test.SelfCheckIn.LogoutWebDriver();

        }

    }
}
