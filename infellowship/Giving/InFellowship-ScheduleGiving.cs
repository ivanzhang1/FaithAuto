using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using log4net;


namespace FTTests.infellowship.Giving
{

    
    [TestFixture]
    [Category(TestCategories.Services.PaymentProcessor)]
    public class infellowship_Giving_ScheduledGiving : FixtureBase
    {

        [FixtureTearDown]
        public void FixtureTearDown()
        {
            TestLog.WriteLine("Running Tear Down.....");
           // log.Debug("Running TEan Down from fixture.... " );
            base.SQL.Giving_ScheduleGiving_DeleteAll(15, "ft.autotester@gmail.com");
            base.SQL.Giving_ScheduleGiving_DeleteAll(258, "ft.autotester@gmail.com");
        }

        [Test, Explicit("This test must be explicitly ran since it will populate all the scheduled contributions for Online Giving 2.0")]
        [Author("Matthew Sneeden")]
        [Description("Creates contribution schedules and runs the stored procedure to process the schedules.")]
        public void ScheduledGiving_ProcessSchedules()
        {
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

            // Capture the created date
            DateTime currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

            // Create the contribution schedules
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("A Test Fund", null, "2000", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek.ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month.ToString(), DateTime.Now.Year.ToString(), null, null, "Matthew", "Sneeden", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString(), null, true);
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("A Test Fund", null, "2402", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek.ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month.ToString(), DateTime.Now.Year.ToString(), null, null, "Matthew", "Sneeden", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString(), null, true);
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("A Test Fund", null, "2260", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek.ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month.ToString(), DateTime.Now.Year.ToString(), null, null, "Matthew", "Sneeden", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString(), null, true);
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("A Test Fund", null, "2521", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek.ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month.ToString(), DateTime.Now.Year.ToString(), null, null, "Matthew", "Sneeden", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString(), null, true);
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("A Test Fund", null, "2501", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek.ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month.ToString(), DateTime.Now.Year.ToString(), null, null, "Matthew", "Sneeden", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString(), null, true);
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("A Test Fund", null, "2204", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek.ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month.ToString(), DateTime.Now.Year.ToString(), null, null, "Matthew", "Sneeden", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString(), null, true);

            // Logout
            test.infellowship.Logout();

            // Move the StartDate and NextPaymentDate back
            base.SQL.Execute(string.Format("UPDATE ChmContribution.dbo.ScheduledGiving SET StartDate = '{0}', NextPaymentDate = '{0}' WHERE ChurchID = 15 AND IsActive = 1 AND CreatedDate > '{1}' AND CreatedByLogin = 'msneeden@fellowshiptech.com'", string.Format("{0}-{1}-{2}", currentDateTime.Year, currentDateTime.Month, currentDateTime.Day), currentDateTime.ToString("yyyy-MM-dd HH:mm:ss")));

            // Execute the stored procedure to run the schedules
            base.SQL.Execute("exec ChmContribution.Pmt.ScheduledGiving_PopulateAll");

        }

        #region Schedule Giving

        #region View Schedule
        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user cannot view a giving schedule belonging to another household.")]
        public void ScheduledGiving_ViewSchedule_DifferentHousehold()
        {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

            // Attempt to navigate to a giving schedule that does not belong to this household
            TestLog.WriteLine(string.Format("Go to ---> {0}/OnlineGiving/ScheduledGiving/{1}", test.infellowship.URL, base.SQL.Execute(string.Format("SELECT TOP 1 ScheduledGivingID FROM ChmContribution.dbo.ScheduledGiving WITH (NOLOCK) WHERE ChurchID = 15 AND IndividualID != {0} AND DeletedDate IS NULL", base.SQL.IndividualID)).Rows[0]["ScheduledGivingID"]));
            test.Selenium.Open(string.Format("{0}/OnlineGiving/ScheduledGiving/{1}", test.infellowship.URL, base.SQL.Execute(string.Format("SELECT TOP 1 ScheduledGivingID FROM ChmContribution.dbo.ScheduledGiving WITH (NOLOCK) WHERE ChurchID = 15 AND IndividualID != {0} AND DeletedDate IS NULL", base.SQL.IndividualID)).Rows[0]["ScheduledGivingID"]));

            // Verify the user is redirected to the index page for scheduled giving
            Assert.IsTrue(test.Selenium.GetLocation() == string.Format("{0}/OnlineGiving/ScheduledGiving", test.infellowship.URL.ToLower()));

            // Logout of infellowship
            test.infellowship.Logout();
        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user cannot view a giving schedule belonging to another household in another church.")]
        public void ScheduledGiving_ViewSchedule_DifferentChurch()
        {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

            // Attempt to navigate to a giving schedule that does not belong to this church
            TestLog.WriteLine(string.Format("Go to ---> {0}/OnlineGiving/ScheduledGiving/{1}", test.infellowship.URL, base.SQL.Execute(string.Format("SELECT TOP 1 ScheduledGivingID FROM ChmContribution.dbo.ScheduledGiving WITH (NOLOCK) WHERE ChurchID != 15 AND IndividualID != {0} AND DeletedDate IS NULL", base.SQL.IndividualID)).Rows[0]["ScheduledGivingID"]));
            test.Selenium.Open(string.Format("{0}/OnlineGiving/ScheduledGiving/{1}", test.infellowship.URL, base.SQL.Execute(string.Format("SELECT TOP 1 ScheduledGivingID FROM ChmContribution.dbo.ScheduledGiving WITH (NOLOCK) WHERE ChurchID != 15 AND IndividualID != {0} AND DeletedDate IS NULL", base.SQL.IndividualID)).Rows[0]["ScheduledGivingID"]));

            // Verify the user is redirected to the index page for scheduled giving
            Assert.IsTrue(test.Selenium.GetLocation() == string.Format("{0}/OnlineGiving/ScheduledGiving", test.infellowship.URL.ToLower()));

            // Logout of infellowship
            test.infellowship.Logout();
        }

        #endregion View Schedule

        #region Create

        #region Credit Cards
        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Performs an one time scheduled giving for a Visa")]
        public void ScheduledGiving_CreditCard_Visa_OneTime()
        {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, "7.98", string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Performs an one time scheduled giving for a Visa with Expiration date in current month")]
        public void ScheduledGiving_CreditCard_Visa_OneTime_Expirationdate_Current_Month()
        {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, "1.98", string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))).ToString("M" + " - " + "MMMM"), (DateTime.Now.Year).ToString(), null, true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Performs a monthly scheduled giving for a Visa")]
        public void ScheduledGiving_CreditCard_Visa_Monthly()
        {

            // Store variables
            var startDay = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Day.ToString();
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MMMM");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("Auto Test Fund", null, "7.99", startDay, startMonth, (DateTime.Now.Year + 1).ToString(), null, null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.infellowship.Logout();
        }


        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Performs a monthly scheduled giving for a Visa")]
        public void ScheduledGiving_CreditCard_Visa_Monthly_Populate()
        {

            // Store variables            
            DateTime currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            var startDay = currentDateTime.Day.ToString();
            var startMonth = currentDateTime.ToString("MMMM");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("Auto Test Fund", null, "7.99", startDay, startMonth, (DateTime.Now.Year + 1).ToString(), null, null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.infellowship.Logout();

            // Move the StartDate and NextPaymentDate back
            base.SQL.Execute(string.Format("UPDATE ChmContribution.dbo.ScheduledGiving SET StartDate = '{0}', NextPaymentDate = '{0}' WHERE ChurchID = 15 AND IsActive = 1 AND CreatedDate > '{1}' AND CreatedByLogin = 'msneeden@fellowshiptech.com'", string.Format("{0}-{1}-{2}", currentDateTime.Year, currentDateTime.Month, currentDateTime.Day), currentDateTime.ToString("yyyy-MM-dd HH:mm:ss")));

            // Execute the stored procedure to run the schedules
            base.SQL.Execute("exec ChmContribution.Pmt.ScheduledGiving_PopulateAll");

        }
            

        [Test, RepeatOnFailure, Timeout(6000)]
        [Author("Bryan Mikaelian")]
        [Description("Performs a twice monthly scheduled giving for a Visa starting on the first")]
        public void ScheduledGiving_CreditCard_Visa_TwiceMonthly_First()
        {

            // Store variables
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string startYear = now.Year.ToString();
            var startMonth = now.AddMonths(1).ToString("MMMM");
            if (startMonth == "January")
            {
                startYear = (now.Year + 1).ToString();
            }
            
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly("Auto Test Fund", null, "8.01", "1", startMonth, startYear, null, null, null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Performs a twice monthly scheduled giving for a Visa starting on the 16th")]
        public void ScheduledGiving_CreditCard_Visa_TwiceMonthly_16th()
        {

            // Store variables
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string startYear = now.Year.ToString();
            var startMonth = now.AddMonths(1).ToString("MMMM");
            if (startMonth == "January")
            {
                startYear = (now.Year + 1).ToString();
            }

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly("Auto Test Fund", null, "9.02", "16", startMonth, startYear, null, null, null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Performs a weekly scheduled giving for a Visa.")]
        public void ScheduledGiving_CreditCard_Visa_Weekly()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Weekly("Auto Test Fund", null, "10.03", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(7).ToString("MM/dd/yyyy"), null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Performs a every two week scheduled giving for a Visa.")]
        public void ScheduledGiving_CreditCard_Visa_EveryTwoWeeks()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_EveryTwoWeeks("Auto Test Fund", null, "11.13", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(7).ToString("MM/dd/yyyy"), null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.infellowship.Logout();
        }


        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a one time scheduled giving for a MasterCard")]
        public void ScheduledGiving_CreditCard_MasterCard_OneTime()
        {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, "11.04", string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Master Card", "5555555555554444", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure, Timeout(6000)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Performs a monthly scheduled giving for a MasterCard")]
        public void ScheduledGiving_CreditCard_MasterCard_Monthly()
        {
            // Store variables
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            var startDay = now.Day.ToString();
            var startMonth = now.ToString("MMMM");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("Auto Test Fund", null, "12.05", startDay, startMonth, (now.Year + 1).ToString(), null, null, "FT", "Tester", "Master Card", "5555555555554444", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a twice monthly scheduled giving for a MasterCard starting on the first")]
        public void ScheduledGiving_CreditCard_MasterCard_TwiceMonthly_First()
        {

            // Store variables
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string startYear = now.Year.ToString();
            var startMonth = now.AddMonths(1).ToString("MMMM");
            if (startMonth == "January")
            {
                startYear = (now.Year + 1).ToString();
            }

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly("Auto Test Fund", null, "13.06", "1", startMonth, startYear, null, null, null, "FT", "Tester", "Master Card", "5555555555554444", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a twice monthly scheduled giving for a MasterCard starting on the 16th")]
        public void ScheduledGiving_CreditCard_MasterCard_TwiceMonthly_16th()
        {

            // Store variables
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string startYear = now.Year.ToString();
            var startMonth = now.AddMonths(1).ToString("MMMM");
            if (startMonth == "January")
            {
                startYear = (now.Year + 1).ToString();
            }

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly("Auto Test Fund", null, "14.07", "16", startMonth, startYear, null, null, null, "FT", "Tester", "Master Card", "5555555555554444", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a weekly scheduled giving for a MasterCard.")]
        public void ScheduledGiving_CreditCard_MasterCard_Weekly()
        {

            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Weekly("Auto Test Fund", null, "15.08", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(7).ToString("MM/dd/yyyy"), null, "FT", "Tester", "Master Card", "5555555555554444", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.infellowship.Logout();

        }

        [Test, RepeatOnFailure, Timeout(6000)]
        [Author("David Martin")]
        [Description("Performs an one time scheduled giving for an American Express")]
        public void ScheduledGiving_CreditCard_AmericanExpress_OneTime()
        {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, "16.09", string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "American Express", "378282246310005", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure,Timeout(6000)]
        [Author("David Martin")]
        [Description("Performs a monthly scheduled giving for an American Express")]
        public void ScheduledGiving_CreditCard_AmericanExpress_Monthly()
        {
            // Store variables
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            var startDay = now.Day.ToString();
            var startMonth = now.ToString("MMMM");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("Auto Test Fund", null, "17.10", startDay, startMonth, (now.Year + 1).ToString(), null, null, "FT", "Tester", "American Express", "378282246310005", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a twice monthly scheduled giving for an American Express starting on the first")]
        public void ScheduledGiving_CreditCard_AmericanExpress_TwiceMonthly_First()
        {

            // Store variables
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string startYear = now.Year.ToString();
            var startMonth = now.AddMonths(1).ToString("MMMM");
            if (startMonth == "January")
            {
                startYear = (now.Year + 1).ToString();
            }

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly("Auto Test Fund", null, "18.11", "1", startMonth, startYear, null, null, null, "FT", "Tester", "American Express", "378282246310005", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure, Timeout(6000)]
        [Author("David Martin")]
        [Description("Performs a twice monthly scheduled giving for an American Express starting on the 16th")]
        public void ScheduledGiving_CreditCard_AmericanExpress_TwiceMonthly_16th()
        {

            // Store variables
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string startYear = now.Year.ToString();
            var startMonth = now.AddMonths(1).ToString("MMMM");
            if (startMonth == "January")
            {
                startYear = (now.Year + 1).ToString();
            }

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly("Auto Test Fund", null, "19.12", "16", startMonth, startYear, null, null, null, "FT", "Tester", "American Express", "378282246310005", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a weekly scheduled giving for an American Express.")]
        public void ScheduledGiving_CreditCard_AmericanExpress_Weekly()
        {

            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Weekly("Auto Test Fund", null, "20.13", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(7).ToString("MM/dd/yyyy"), null, "FT", "Tester", "American Express", "378282246310005", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.infellowship.Logout();

        }

        [Test, RepeatOnFailure, Timeout(6000)]
        [Author("David Martin")]
        [Description("Performs an one time scheduled giving for Discover")]
        public void ScheduledGiving_CreditCard_Discover_OneTime()
        {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, "21.14", string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Discover", "6011111111111117", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a monthly scheduled giving for Discover")]
        public void ScheduledGiving_CreditCard_Discover_Monthly()
        {
            // Store variables
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            var startDay = now.Day.ToString();
            var startMonth = now.ToString("MMMM");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("Auto Test Fund", null, "22.15", startDay, startMonth, (now.Year + 1).ToString(), null, null, "FT", "Tester", "Discover", "6011111111111117", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure, Timeout(600)]
        [Author("David Martin")]
        [Description("Performs a twice monthly scheduled giving for Discover starting on the first")]
        public void ScheduledGiving_CreditCard_Discover_TwiceMonthly_First()
        {
            // Store variables
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string startYear = now.Year.ToString();
            var startMonth = now.AddMonths(1).ToString("MMMM");
            if (startMonth == "January")
            {
                startYear = (now.Year + 1).ToString();
            }

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly("Auto Test Fund", null, "23.16", "1", startMonth, startYear, null, null, null, "FT", "Tester", "Discover", "6011111111111117", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure, Timeout(600)]
        [Author("David Martin")]
        [Description("Performs a twice monthly scheduled giving for Discover starting on the 16th")]
        public void ScheduledGiving_CreditCard_Discover_TwiceMonthly_16th()
        {
            // Store variables
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string startYear = now.Year.ToString();
            var startMonth = now.AddMonths(1).ToString("MMMM");
            if (startMonth == "January")
            {
                startYear = (now.Year + 1).ToString();
            }

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly("Auto Test Fund", null, "24.17", "16", startMonth, startYear, null, null, null, "FT", "Tester", "Discover", "6011111111111117", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a weekly scheduled giving for Discover.")]
        public void ScheduledGiving_CreditCard_Discover_Weekly()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Weekly("Auto Test Fund", null, "25.18", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(7).ToString("MM/dd/yyyy"), null, "FT", "Tester", "Discover", "6011111111111117", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs an one time scheduled giving for JCB")]
        public void ScheduledGiving_CreditCard_JCB_OneTime()
        {
            // Login to InFellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "QAEUNLX0C6");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, "26.19", string.Format("31/12/{0}", DateTime.Now.Year), "FT", "Tester", "JCB", "3566111111111113", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true, 258);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure, Timeout(600)]
        [Author("David Martin")]
        [Description("Performs a monthly scheduled giving for JCB")]
        public void ScheduledGiving_CreditCard_JCB_Monthly()
        {
            // Store variables
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            var startDay = now.Day.ToString();
            var startMonth = now.ToString("MMMM");

            // Login to InFellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "QAEUNLX0C6");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("Auto Test Fund", null, "27.20", startDay, startMonth, (now.Year + 1).ToString(), null, null, "FT", "Tester", "JCB", "3566111111111113", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true, 258);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a twice monthly scheduled giving for JCB starting on the first")]
        public void ScheduledGiving_CreditCard_JCB_TwiceMonthly_First()
        {
            // Store variables
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string startYear = now.Year.ToString();
            var startMonth = now.AddMonths(1).ToString("MMMM");
            if (startMonth == "January")
            {
                startYear = (now.Year + 1).ToString();
            }

            // Login to InFellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "QAEUNLX0C6");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly("Auto Test Fund", null, "28.21", "1", startMonth, startYear, null, null, null, "FT", "Tester", "JCB", "3566111111111113", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true, 258);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a twice monthly scheduled giving for JCB starting on the 16th")]
        public void ScheduledGiving_CreditCard_JCB_TwiceMonthly_16th()
        {
            // Store variables
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string startYear = now.Year.ToString();
            var startMonth = now.AddMonths(1).ToString("MMMM");
            if (startMonth == "January")
            {
                startYear = (now.Year + 1).ToString();
            }

            // Login to InFellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "QAEUNLX0C6");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly("Auto Test Fund", null, "29.22", "16", startMonth, startYear, null, null, null, "FT", "Tester", "JCB", "3566111111111113", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true, 258);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Performs a weekly scheduled giving for JCB.")]
        public void ScheduledGiving_CreditCard_JCB_Weekly()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login to InFellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "QAEUNLX0C6");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Weekly("Auto Test Fund", null, "30.23", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(7).ToString("dd/MM/yyyy"), null, "FT", "Tester", "JCB", "3566111111111113", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true, 258);

            // Logout of infellowship
            test.infellowship.Logout();
        }

        #endregion Credit Cards

        #region Personal Checks
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Performs an one time scheduled giving for a Personal Check")]
        public void ScheduledGiving_PersonalCheck_OneTime()
        {
            // Variables
            var amount = Math.Round(1.0 + new Random().NextDouble() * 100, 2).ToString();

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_OneTime("Auto Test Fund", null, amount, string.Format("12/31/{0}", DateTime.Now.Year), "8176833453", "111000025", "111222333", true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Performs a monthly scheduled giving for a Personal Check")]
        public void ScheduledGiving_PersonalCheck_Monthly()
        {
            // Store variables
            //Modify by Grace zhang
            var startDay = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddMonths(2), TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Day.ToString();
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddMonths(2), TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MMMM");
           
            var amount = Math.Round(1.0 + new Random().NextDouble() * 100, 2).ToString();

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving --- Modify by Grace zhang
            test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_Monthly("Auto Test Fund", null, amount, startDay, startMonth, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddMonths(2), TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Year.ToString(), null, null, "8176833453", "111000025", "111222333", true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Performs a twice monthly scheduled giving for a Personal Check starting on the first.")]
        public void ScheduledGiving_PersonalCheck_TwiceMonthly_First()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var amount = Math.Round(1.0 + new Random().NextDouble() * 100, 2).ToString();

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving at a twice monthly frequency, starting on the first of the month.
            test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_TwiceMonthly("Auto Test Fund", null, amount, "1", startMonth, DateTime.Now.AddMonths(1).Year.ToString(), null, null, null, "8176833453", "111000025", "111222333", true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Performs a twice monthly scheduled giving for a Personal Check starting on the 16th.")]
        public void ScheduledGiving_PersonalCheck_TwiceMonthly_16th()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var amount = Math.Round(1.0 + new Random().NextDouble() * 100, 2).ToString();

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving at a twice monthly frequency, starting on the first of the month.
            test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_TwiceMonthly("Auto Test Fund", null, amount, "16", startMonth, DateTime.Now.AddMonths(1).Year.ToString(), null, null, null, "8176833453", "111000025", "111222333", true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Performs a weekly scheduled giving for a Personal Check.")]
        public void ScheduledGiving_PersonalCheck_Weekly()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
            var amount = Math.Round(1.0 + new Random().NextDouble() * 100, 2).ToString();

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving at a weekly frequency
            test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_Weekly("Auto Test Fund", null, amount, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddMonths(1), TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddMonths(1), TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(7).ToString("MM/dd/yyyy"), null, "8176833453", "111000025", "111222333", true);

            // Logout
            test.infellowship.Logout();
        }


        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Performs an every two weeks scheduled giving for a Personal Check.")]
        public void ScheduledGiving_PersonalCheck_EveryTwoWeeks()
        {
            // Store variables
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving at a weekly frequency
            test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_EveryTwoWeeks("Auto Test Fund", null, "2.61", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(7).ToString("MM/dd/yyyy"), null, "8176833453", "111000025", "111222333", true);

            // Logout
            test.infellowship.Logout();
        }

        #endregion Personal Checks

        #endregion Create

        #region Edit

        #region Active / Inactive
        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Updates a credit card schedule to be of the inactive and active statuses.")]
        public void ScheduledGiving_Edit_Status_CreditCard()
        {
            // Variables
            var amount = "9.91";

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, amount, string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Make the schedule inactive
            test.infellowship.Giving_ScheduledGiving_Edit_Status(15, amount, false);

            // Make the schedule active
            test.infellowship.Giving_ScheduledGiving_Edit_Status(15, amount, true);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Updates a personal check schedule to be of the inactive and active statuses.")]
        public void ScheduledGiving_Edit_Status_PersonalCheck()
        {
            // Variables
            var amount = "2.95";

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_OneTime("Auto Test Fund", null, amount, string.Format("12/31/{0}", DateTime.Now.Year), "8176833453", "111000025", "111222333", true);

            // Make the schedule inactive
            test.infellowship.Giving_ScheduledGiving_Edit_Status(15, amount, false);

            // Make the schedule active
            test.infellowship.Giving_ScheduledGiving_Edit_Status(15, amount, true);

            // Logout
            test.infellowship.Logout();
        }


        #endregion Active / Inactive

        #region Where to Give
        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Updates a credit card schedule's fund, subfund and amount.")]
        public void ScheduledGiving_Edit_WhereToGive_CreditCard()
        {
            // Variables
            var amount = "7.91";

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, amount, string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Edit the where to give
            test.infellowship.Giving_ScheduledGiving_Edit_WhereToGive(amount, "1 - General Fund", null, "10.22");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Updates a credit card schedule's fund, subfund and amount but do not specify a fund.")]
        public void ScheduledGiving_Edit_WhereToGive_NoFund_CreditCard()
        {
            // Variables
            var amount = "7.61";

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, amount, string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Edit the where to give but do not specify a fund.
            test.infellowship.Giving_ScheduledGiving_Edit_WhereToGive(amount, "--", null, "4.16");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Updates a credit card schedule's fund, subfund and amount but do not specify an amount.")]
        public void ScheduledGiving_Edit_WhereToGive_NoAmount_CreditCard()
        {
            // Variables
            var amount = "3.91";

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, amount, string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            test.GeneralMethods.TakeScreenShot();

            // Edit the where to give but do not specify an amount
            test.infellowship.Giving_ScheduledGiving_Edit_WhereToGive(amount, "Auto Test Fund", null, null);

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Updates a personal check schedule's fund, subfund and amount.")]
        public void ScheduledGiving_Edit_WhereToGive_PersonalCheck()
        {
            // Variables
            var amount = "3.95";

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_OneTime("Auto Test Fund", null, amount, string.Format("12/31/{0}", DateTime.Now.Year), "8176833453", "111000025", "111222333", true);

            // Edit the where to give
            test.infellowship.Giving_ScheduledGiving_Edit_WhereToGive(amount, "1 - General Fund", null, "9.22");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Updates a personal check schedule's fund, subfund and amount but do not provide a fund.")]
        public void ScheduledGiving_Edit_WhereToGive_NoFund_PersonalCheck()
        {
            // Variables
            var amount = "3.95";

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_OneTime("Auto Test Fund", null, amount, string.Format("12/31/{0}", DateTime.Now.Year), "8176833453", "111000025", "111222333", true);

            // Edit the where to give but do not specify a fund.
            test.infellowship.Giving_ScheduledGiving_Edit_WhereToGive(amount, "--", null, "4.96");

            // Logout
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Updates a personal check schedule's fund, subfund and amount but do not provide an amount.")]
        public void ScheduledGiving_Edit_WhereToGive_NoAmount_PersonalCheck()
        {
            // Variables
            var amount = "3.35";

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_OneTime("Auto Test Fund", null, amount, string.Format("12/31/{0}", DateTime.Now.Year), "8176833453", "111000025", "111222333", true);

            // Edit the where to give but do not specify a fund.
            test.infellowship.Giving_ScheduledGiving_Edit_WhereToGive(amount, "Auto Test Fund", null, string.Empty);

            // Logout
            test.infellowship.Logout();
        }

        #endregion Where to Give

        #region Payment Information
        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can change the credit card when editing the payment information for a scheduled giving.")]
        public void ScheduledGiving_Edit_PaymentInformation_Change_CreditCard()
        {
            // Variables
            var amount = "2.29";

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, amount, string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);


            // Update the credit card
            test.infellowship.Giving_ScheduledGiving_Edit_CreditCard_PaymentInformation(amount, "FT", "Tester", "Master Card", "5555555555554444", null, null, null, null, null, null, null, null, null, null);

            // Logout
            test.infellowship.Logout();
        }


        #endregion Payment Information

        #region Frequency

        #endregion Frequency

        #endregion Edit

        #region Delete
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Deletes an one time scheduled giving for a credit card")]
        public void ScheduledGiving_Delete_CreditCard_OneTime()
        {

            // Variables
            var amount = "3.72";

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, amount, string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Delete the scheduled giving
            test.infellowship.Giving_ScheduledGiving_Delete(amount);

            // Logout
            test.infellowship.Logout();
        }
        [Test, RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("FO-3809:Giving Schedule can be deleted.")]
        public void ScheduledGiving_PersonalCheck_Create_Monthly_Delete()
        {
           
            // Store variables
            //Modify by Grace zhang
            var startDay = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddMonths(2), TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Day.ToString();
            var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddMonths(2), TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MMMM");
           
            var amount = "3.33";

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving --Modify by Grace zhang
            test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_Monthly("Auto Test Fund", null, amount, startDay, startMonth, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddMonths(2), TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Year.ToString(), null, null, "8176833453", "111000025", "111222333", true);

            // delete the scheduled.
            test.infellowship.Giving_ScheduledGiving_Delete(amount);

            // Logout
            test.infellowship.Logout();


        }
        #endregion Delete

        #region Validation
        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a fund  is required for Schedule Giving.")]
        public void ScheduledGiving_Validation_Fund_Required()
        {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Create a schedule without specifying a fund
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime(null, null, "11.11", string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that an amount is required for Schedule Giving.")]
        public void ScheduledGiving_Validation_Amount_Required()
        {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Create a schedule without specifying an amount
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, null, string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a date is required for the one time frequency.")]
        public void ScheduledGiving_Validation_OneTime_Date_Required()
        {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Create a one time schedule without providing any of the date information
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, "5.65", string.Empty, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout of infellowship
            test.infellowship.Logout();
        }


        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a date is required for the monthly frequency.")]
        public void ScheduledGiving_Validation_Monthly_Date_Required()
        {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Create a monthly schedule without providing any of the date information
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("Auto Test Fund", null, "5.65", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a date is required for the twice monthly frequency.")]
        public void ScheduledGiving_Validation_TwiceMonthly_Date_Required()
        {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Create a monthly schedule without providing any of the date information
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly("Auto Test Fund", null, "5.65", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a date is required for the weekly frequency.")]
        public void ScheduledGiving_Validation_Weekly_Date_Required()
        {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Create a weekly schedule without providing any of the date information
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Weekly("Auto Test Fund", null, "5.65", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek, string.Empty, string.Empty, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that a date is required for the twice sweekly frequency.")]
        public void ScheduledGiving_Validation_TwiceWeekly_Date_Required()
        {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Create a twice weekly schedule without providing the month 
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_EveryTwoWeeks("Auto Test Fund", null, "5.65", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek, string.Empty, string.Empty, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that all the payment information is required when creating a schedule usong a credit card.")]
        public void ScheduledGiving_Validation_PaymentInformation_Required_CreditCard()
        {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Create a schedule without providing the payment information
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, "5.65", string.Format("12/31/{0}", DateTime.Now.Year), null, null, null, null, null, null, null, true);

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that first name, last name, and credit card number cannot exceed the field lengths.")]
        public void ScheduledGiving_Validation_PaymentInformation_TextField_Lengths_CreditCard()
        {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Create a schedule with a first name, last name, and credit card number that exceed the maximum field length.
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, "5.65", string.Format("12/31/{0}", DateTime.Now.Year), "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", "Visa", "411111111111111111111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure, Timeout(1200)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that phone number, routing number, and account number cannot exceed the field lengths.")]
        public void ScheduledGiving_Validation_PaymentInformation_TextField_Lengths_PersonalCheck()
        {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Create a schedule with a phone number, routing number, and account number that exceed the maximum field length.
            test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_OneTime("Auto Test Fund", null, "5.65", string.Format("12/31/{0}", DateTime.Now.Year), "123456789012345678901234567890123456789012345678901", "123456789012345678901234567890123456789012345678901", "123456789012345678901234567890123456789012345678901", true);

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that all the payment information is required when creating a schedule using a personal check.")]
        public void ScheduledGiving_Validation_PaymentInformation_Required_PersonalCheck()
        {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Create a schedule without providing the payment information
            test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_OneTime("Auto Test Fund", null, "5.65", string.Format("12/31/{0}", DateTime.Now.Year), null, null, null, true);

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you must agree to the payment when submitting a schedule.")]
        public void ScheduledGiving_Validation_AgreeToPayment_Required()
        {
            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

            // Create a schedule without agreeing to the payment
            test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, "5.65", string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, false);

            // Logout of infellowship
            test.infellowship.Logout();
        }

        #endregion Validation

        #endregion Schedule Giving
    }


    [TestFixture]
    [Category(TestCategories.Services.PaymentProcessor)]
    public class infellowship_Giving_ScheduledGiving_WebDriver : FixtureBaseWebDriver
    {
        [FixtureTearDown]
        public void FixtureTearDown()
        {
            base.SQL.Giving_ScheduleGiving_DeleteAll(15, "ft.autotester@gmail.com");
            base.SQL.Giving_ScheduleGiving_DeleteAll(258, "ft.autotester@gmail.com");
        }

        //[Test, Explicit("This test must be explicitly ran since it will populate all the scheduled contributions for Online Giving 2.0")]
        //[Author("Matthew Sneeden")]
        //[Description("Creates contribution schedules and runs the stored procedure to process the schedules.")]
        //public void ScheduledGiving_ProcessSchedules()
        //{
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

        //    // Capture the created date
        //    DateTime currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

        //    // Create the contribution schedules
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("A Test Fund", null, "2000", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek.ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month.ToString(), DateTime.Now.Year.ToString(), null, null, "Matthew", "Sneeden", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString(), null, true);
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("A Test Fund", null, "2402", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek.ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month.ToString(), DateTime.Now.Year.ToString(), null, null, "Matthew", "Sneeden", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString(), null, true);
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("A Test Fund", null, "2260", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek.ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month.ToString(), DateTime.Now.Year.ToString(), null, null, "Matthew", "Sneeden", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString(), null, true);
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("A Test Fund", null, "2521", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek.ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month.ToString(), DateTime.Now.Year.ToString(), null, null, "Matthew", "Sneeden", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString(), null, true);
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("A Test Fund", null, "2501", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek.ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month.ToString(), DateTime.Now.Year.ToString(), null, null, "Matthew", "Sneeden", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString(), null, true);
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("A Test Fund", null, "2204", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek.ToString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month.ToString(), DateTime.Now.Year.ToString(), null, null, "Matthew", "Sneeden", "Visa", "4111111111111111", "12 - December", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).Year.ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();

        //    // Move the StartDate and NextPaymentDate back
        //    base.SQL.Execute(string.Format("UPDATE ChmContribution.dbo.ScheduledGiving SET StartDate = '{0}', NextPaymentDate = '{0}' WHERE ChurchID = 15 AND IsActive = 1 AND CreatedDate > '{1}' AND CreatedByLogin = 'msneeden@fellowshiptech.com'", string.Format("{0}-{1}-{2}", currentDateTime.Year, currentDateTime.Month, currentDateTime.Day), currentDateTime.ToString("yyyy-MM-dd HH:mm:ss")));

        //    // Execute the stored procedure to run the schedules
        //    base.SQL.Execute("exec ChmContribution.Pmt.ScheduledGiving_PopulateAll");

        //}

        #region Schedule Giving

        #region View Schedule
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user cannot view a giving schedule belonging to another household.")]
        public void ScheduledGiving_ViewSchedule_DifferentHousehold_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

            // Attempt to navigate to a giving schedule that does not belong to this household
            TestLog.WriteLine(string.Format("Go to ---> {0}/OnlineGiving/ScheduledGiving/{1}", test.Infellowship.URL, base.SQL.Execute(string.Format("SELECT TOP 1 ScheduledGivingID FROM ChmContribution.dbo.ScheduledGiving WITH (NOLOCK) WHERE ChurchID = 15 AND IndividualID != {0} AND DeletedDate IS NULL", base.SQL.IndividualID)).Rows[0]["ScheduledGivingID"]));
            test.GeneralMethods.OpenURLWebDriver(string.Format("{0}/OnlineGiving/ScheduledGiving/{1}", test.Infellowship.URL, base.SQL.Execute(string.Format("SELECT TOP 1 ScheduledGivingID FROM ChmContribution.dbo.ScheduledGiving WITH (NOLOCK) WHERE ChurchID = 15 AND IndividualID != {0} AND DeletedDate IS NULL", base.SQL.IndividualID)).Rows[0]["ScheduledGivingID"]));

            // Verify the user is redirected to the index page for scheduled giving
            Assert.IsTrue(test.GeneralMethods.GetUrl() == string.Format("{0}OnlineGiving/ScheduledGiving", test.Infellowship.URL.ToLower()));

            //commentted it by ivan.zhang, 2/22/2016
            /*if (this.F1Environment.ToString().Equals("STAGING"))
            {
                test.GeneralMethods.WaitForElement(By.Id("submitQuery"));
                test.Driver.FindElementByXPath("//a[@href='/OnlineGiving/Home']").Click();
            }
            else
            {
                test.GeneralMethods.WaitForElement(By.XPath("//a[@href='/OnlineGiving/History']"));
                // Logout of infellowship
                test.Infellowship.LogoutWebDriver();
            }*/

            test.GeneralMethods.WaitForElement(By.XPath("//span[text()='Schedule Giving']"));
            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();
            
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user cannot view a giving schedule belonging to another household in another church.")]
        public void ScheduledGiving_ViewSchedule_DifferentChurch_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

            // Attempt to navigate to a giving schedule that does not belong to this church
            TestLog.WriteLine(string.Format("Go to ---> {0}/OnlineGiving/ScheduledGiving/{1}", test.Infellowship.URL, base.SQL.Execute(string.Format("SELECT TOP 1 ScheduledGivingID FROM ChmContribution.dbo.ScheduledGiving WITH (NOLOCK) WHERE ChurchID != 15 AND IndividualID != {0} AND DeletedDate IS NULL", base.SQL.IndividualID)).Rows[0]["ScheduledGivingID"]));
            test.GeneralMethods.OpenURLWebDriver(string.Format("{0}/OnlineGiving/ScheduledGiving/{1}", test.Infellowship.URL, base.SQL.Execute(string.Format("SELECT TOP 1 ScheduledGivingID FROM ChmContribution.dbo.ScheduledGiving WITH (NOLOCK) WHERE ChurchID != 15 AND IndividualID != {0} AND DeletedDate IS NULL", base.SQL.IndividualID)).Rows[0]["ScheduledGivingID"]));

            // Verify the user is redirected to the index page for scheduled giving           
            Assert.IsTrue(test.GeneralMethods.GetUrl() == string.Format("{0}OnlineGiving/ScheduledGiving", test.Infellowship.URL.ToLower()));

            //Updaed by Jim
            /*
            if (this.F1Environment.ToString().Equals("STAGING"))
            {
                test.GeneralMethods.WaitForElement(By.Id("submitQuery"));
                test.Driver.FindElementByXPath("//a[@href='/OnlineGiving/Home']").Click();
            }
            else
            {
                test.GeneralMethods.WaitForElement(By.XPath("//a[@href='/OnlineGiving/History']"));
                // Logout of infellowship
                test.Infellowship.LogoutWebDriver();
            }*/

            test.GeneralMethods.WaitForElement(By.XPath("//a[@href='/OnlineGiving/History']"));
            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        #region View
        [Test, RepeatOnFailure, Timeout(120000)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Views a scheduled giving.")]
        public void ScheduledGiving_View_WebDriver()
        {
            // Variables
            var amount = "7.91";

            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.Infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime_WebDriver("Auto Test Fund", string.Empty, amount, string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

            // Edit the giving schedule
            test.Infellowship.Giving_ScheduledGiving_View_WebDriver(amount);

            // Verify the user is on the page
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("Back"));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("Edit this schedule"));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("Delete this schedule"));
            test.GeneralMethods.VerifyTextPresentWebDriver(string.Format("{0:c}", amount));

            // Back
            test.Driver.FindElementByLinkText("Back").Click();

            // Delete the schedule
            test.Infellowship.Giving_ScheduledGiving_Delete_WebDriver(amount);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies the content appears properly in schedule giving screen")]
        public void ScheduledGiving_UK_View_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "QAEUNLX0C6");

            // Navigate to the giving history page
            test.Driver.FindElementByXPath(Navigation.InFellowship.Your_Giving).Click();

            // Click on the Schedule giving button
            test.Driver.FindElementByLinkText("Schedule Giving").Click();

            // Click to add another row
            test.Driver.FindElementById("add_row").Click();

            // Verify user is on the Schedules page
            Assert.AreEqual(GeneralInFellowship.Giving.f, test.Driver.FindElementByXPath(string.Format("{0}/li[2]/div/div[3]/div/span", TableIds.InFellowship_ScheduleGivingTable)).Text);
            Assert.AreEqual(GeneralInFellowship.Giving.f, test.Driver.FindElementByXPath(string.Format("{0}/li[3]/div/div[3]/div/span", TableIds.InFellowship_ScheduleGivingTable)).Text);
            //Assert.AreEqual(GeneralInFellowship.Giving.f+"0.00", test.Driver.FindElementByXPath(string.Format("{0}/tfoot/tr/td[3]", TableIds.InFellowship_ScheduleGivingTable)).Text);

            test.Driver.FindElementByXPath("//a[@href='/OnlineGiving/ScheduledGiving/Cancel']").Click();

            // Logout of Infellowship
            test.Infellowship.LogoutWebDriver();

        }

        #endregion View

        #endregion View Schedule

        #region Create

        #region Credit Cards
        [Test, RepeatOnFailure, Timeout(1200)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Performs an one time scheduled giving for a Visa")]
        public void ScheduledGiving_CreditCard_Visa_OneTime_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.Infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime_WebDriver("Auto Test Fund", null, "7.98", string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

            // Logout
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure, Timeout(1200)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Performs an one time scheduled giving for a Visa, its scheduled date is today")]
        public void ScheduledGiving_CreditCard_Visa_OneTime_Of_Today_WebDriver()
        {
            //Test data
            CultureInfo culture = new CultureInfo("en-US");

            var amount = "88.88";
            var fundName = "Auto Test Fund";
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            var startDay = now.Day.ToString();
            var startMonth = now.ToString("MMMM");
            var startYear = now.Year.ToString();
            var scheduledDate = string.Format("{0}/{1}/{2}", startMonth, startDay, startYear);

            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            var formattedDate = test.GeneralMethods.ConvertDateToNeutralFormat(Convert.ToDateTime(string.Format("{0} {1} {2}", startDay, startMonth, startYear)));
            
            // Login to infellowship
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Perform a scheduled giving
            test.Infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime_Of_Today_WebDriver(fundName, null, "88.88", scheduledDate, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (now.Year + 1).ToString(), null, true);

            //Verify the one time schedule giving which scheduled date is today has been done
            test.GeneralMethods.WaitForPageIsLoaded(120);
            Assert.AreEqual(formattedDate, test.Driver.FindElementByXPath("//table[@class='grid']/tbody/tr[2]/td[4]").Text, "The date of contribution was incorrect.");
            Assert.AreEqual(string.Format("{0}\r\n{1}", string.Format(culture, "{0:c}", Convert.ToDecimal(amount)), formattedDate), test.Driver.FindElementByXPath("//table[@class='grid']/tbody/tr[2]/td[5]").Text, "The amount was incorrect.");
            Assert.IsTrue(test.Driver.FindElementByXPath("//table[@class='grid']/tbody/tr[2]/td[3]").Text.Contains(fundName), "The designation was incorrect.");
            
            // Logout
            test.Infellowship.LogoutWebDriver();

        }

        ////[Test, RepeatOnFailure]
        //[Author("Stuart Platt")]
        //[Description("Performs an one time scheduled giving for a Visa with Expiration date in current month")]
        //public void ScheduledGiving_CreditCard_Visa_OneTime_Expirationdate_Current_Month()
        //{
        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, "1.98", string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))).ToString("M" + " - " + "MMMM"), (DateTime.Now.Year).ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Performs a monthly scheduled giving for a Visa")]
        //public void ScheduledGiving_CreditCard_Visa_Monthly()
        //{

        //    // Store variables
        //    var startDay = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Day.ToString();
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(2).ToString("MMMM");

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("Auto Test Fund", null, "7.99", startDay, startMonth, DateTime.Now.Year.ToString(), null, null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();
        //}


        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Performs a monthly scheduled giving for a Visa")]
        //public void ScheduledGiving_CreditCard_Visa_Monthly_Populate()
        //{

        //    // Store variables            
        //    DateTime currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
        //    var startDay = currentDateTime.Day.ToString();
        //    var startMonth = currentDateTime.AddMonths(2).ToString("MMMM");

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("Auto Test Fund", null, "7.99", startDay, startMonth, DateTime.Now.Year.ToString(), null, null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();

        //    // Move the StartDate and NextPaymentDate back
        //    base.SQL.Execute(string.Format("UPDATE ChmContribution.dbo.ScheduledGiving SET StartDate = '{0}', NextPaymentDate = '{0}' WHERE ChurchID = 15 AND IsActive = 1 AND CreatedDate > '{1}' AND CreatedByLogin = 'msneeden@fellowshiptech.com'", string.Format("{0}-{1}-{2}", currentDateTime.Year, currentDateTime.Month, currentDateTime.Day), currentDateTime.ToString("yyyy-MM-dd HH:mm:ss")));

        //    // Execute the stored procedure to run the schedules
        //    base.SQL.Execute("exec ChmContribution.Pmt.ScheduledGiving_PopulateAll");

        //}


        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Performs a twice monthly scheduled giving for a Visa starting on the first")]
        //public void ScheduledGiving_CreditCard_Visa_TwiceMonthly_First()
        //{

        //    // Store variables
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly("Auto Test Fund", null, "8.01", "1", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Performs a twice monthly scheduled giving for a Visa starting on the 16th")]
        //public void ScheduledGiving_CreditCard_Visa_TwiceMonthly_16th()
        //{

        //    // Store variables
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly("Auto Test Fund", null, "9.02", "16", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Performs a weekly scheduled giving for a Visa.")]
        //public void ScheduledGiving_CreditCard_Visa_Weekly()
        //{
        //    // Store variables
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Weekly("Auto Test Fund", null, "10.03", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(7).ToString("MM/dd/yyyy"), null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Performs a every two week scheduled giving for a Visa.")]
        //public void ScheduledGiving_CreditCard_Visa_EveryTwoWeeks()
        //{
        //    // Store variables
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_EveryTwoWeeks("Auto Test Fund", null, "11.13", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(7).ToString("MM/dd/yyyy"), null, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();
        //}


        ////[Test, RepeatOnFailure]
        //[Author("David Martin")]
        //[Description("Performs a one time scheduled giving for a MasterCard")]
        //public void ScheduledGiving_CreditCard_MasterCard_OneTime()
        //{
        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, "11.04", string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Master Card", "5555555555554444", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("David Martin")]
        //[Description("Performs a monthly scheduled giving for a MasterCard")]
        //public void ScheduledGiving_CreditCard_MasterCard_Monthly()
        //{
        //    // Store variables
        //    var startDay = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Day.ToString();
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(2).ToString("MMMM");

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("Auto Test Fund", null, "12.05", startDay, startMonth, DateTime.Now.Year.ToString(), null, null, "FT", "Tester", "Master Card", "5555555555554444", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("David Martin")]
        //[Description("Performs a twice monthly scheduled giving for a MasterCard starting on the first")]
        //public void ScheduledGiving_CreditCard_MasterCard_TwiceMonthly_First()
        //{

        //    // Store variables
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly("Auto Test Fund", null, "13.06", "1", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "Master Card", "5555555555554444", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("David Martin")]
        //[Description("Performs a twice monthly scheduled giving for a MasterCard starting on the 16th")]
        //public void ScheduledGiving_CreditCard_MasterCard_TwiceMonthly_16th()
        //{

        //    // Store variables
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly("Auto Test Fund", null, "14.07", "16", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "Master Card", "5555555555554444", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("David Martin")]
        //[Description("Performs a weekly scheduled giving for a MasterCard.")]
        //public void ScheduledGiving_CreditCard_MasterCard_Weekly()
        //{

        //    // Store variables
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Weekly("Auto Test Fund", null, "15.08", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(7).ToString("MM/dd/yyyy"), null, "FT", "Tester", "Master Card", "5555555555554444", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();

        //}

        ////[Test, RepeatOnFailure]
        //[Author("David Martin")]
        //[Description("Performs an one time scheduled giving for an American Express")]
        //public void ScheduledGiving_CreditCard_AmericanExpress_OneTime()
        //{
        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, "16.09", string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "American Express", "378282246310005", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("David Martin")]
        //[Description("Performs a monthly scheduled giving for an American Express")]
        //public void ScheduledGiving_CreditCard_AmericanExpress_Monthly()
        //{
        //    // Store variables
        //    var startDay = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Day.ToString();
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(2).ToString("MMMM");

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("Auto Test Fund", null, "17.10", startDay, startMonth, DateTime.Now.Year.ToString(), null, null, "FT", "Tester", "American Express", "378282246310005", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("David Martin")]
        //[Description("Performs a twice monthly scheduled giving for an American Express starting on the first")]
        //public void ScheduledGiving_CreditCard_AmericanExpress_TwiceMonthly_First()
        //{

        //    // Store variables
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly("Auto Test Fund", null, "18.11", "1", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "American Express", "378282246310005", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("David Martin")]
        //[Description("Performs a twice monthly scheduled giving for an American Express starting on the 16th")]
        //public void ScheduledGiving_CreditCard_AmericanExpress_TwiceMonthly_16th()
        //{

        //    // Store variables
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly("Auto Test Fund", null, "19.12", "16", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "American Express", "378282246310005", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("David Martin")]
        //[Description("Performs a weekly scheduled giving for an American Express.")]
        //public void ScheduledGiving_CreditCard_AmericanExpress_Weekly()
        //{

        //    // Store variables
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Weekly("Auto Test Fund", null, "20.13", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(7).ToString("MM/dd/yyyy"), null, "FT", "Tester", "American Express", "378282246310005", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();

        //}

        ////[Test, RepeatOnFailure]
        //[Author("David Martin")]
        //[Description("Performs an one time scheduled giving for Discover")]
        //public void ScheduledGiving_CreditCard_Discover_OneTime()
        //{
        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, "21.14", string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Discover", "6011111111111117", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("David Martin")]
        //[Description("Performs a monthly scheduled giving for Discover")]
        //public void ScheduledGiving_CreditCard_Discover_Monthly()
        //{
        //    // Store variables
        //    var startDay = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Day.ToString();
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(2).ToString("MMMM");

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("Auto Test Fund", null, "22.15", startDay, startMonth, DateTime.Now.Year.ToString(), null, null, "FT", "Tester", "Discover", "6011111111111117", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("David Martin")]
        //[Description("Performs a twice monthly scheduled giving for Discover starting on the first")]
        //public void ScheduledGiving_CreditCard_Discover_TwiceMonthly_First()
        //{
        //    // Store variables
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly("Auto Test Fund", null, "23.16", "1", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "Discover", "6011111111111117", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("David Martin")]
        //[Description("Performs a twice monthly scheduled giving for Discover starting on the 16th")]
        //public void ScheduledGiving_CreditCard_Discover_TwiceMonthly_16th()
        //{
        //    // Store variables
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly("Auto Test Fund", null, "24.17", "16", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "Discover", "6011111111111117", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("David Martin")]
        //[Description("Performs a weekly scheduled giving for Discover.")]
        //public void ScheduledGiving_CreditCard_Discover_Weekly()
        //{
        //    // Store variables
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Weekly("Auto Test Fund", null, "25.18", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(7).ToString("MM/dd/yyyy"), null, "FT", "Tester", "Discover", "6011111111111117", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("David Martin")]
        //[Description("Performs an one time scheduled giving for JCB")]
        //public void ScheduledGiving_CreditCard_JCB_OneTime()
        //{
        //    // Login to InFellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "QAEUNLX0C6");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, "26.19", string.Format("31/12/{0}", DateTime.Now.Year), "FT", "Tester", "JCB", "3566111111111113", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true, 258);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("David Martin")]
        //[Description("Performs a monthly scheduled giving for JCB")]
        //public void ScheduledGiving_CreditCard_JCB_Monthly()
        //{
        //    // Store variables
        //    var startDay = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Day.ToString();
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(2).ToString("MMMM");

        //    // Login to InFellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "QAEUNLX0C6");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("Auto Test Fund", null, "27.20", startDay, startMonth, DateTime.Now.Year.ToString(), null, null, "FT", "Tester", "JCB", "3566111111111113", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true, 258);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("David Martin")]
        //[Description("Performs a twice monthly scheduled giving for JCB starting on the first")]
        //public void ScheduledGiving_CreditCard_JCB_TwiceMonthly_First()
        //{
        //    // Store variables
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

        //    // Login to InFellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "QAEUNLX0C6");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly("Auto Test Fund", null, "28.21", "1", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "JCB", "3566111111111113", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true, 258);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("David Martin")]
        //[Description("Performs a twice monthly scheduled giving for JCB starting on the 16th")]
        //public void ScheduledGiving_CreditCard_JCB_TwiceMonthly_16th()
        //{
        //    // Store variables
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

        //    // Login to InFellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "QAEUNLX0C6");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly("Auto Test Fund", null, "29.22", "16", startMonth, DateTime.Now.Year.ToString(), null, null, null, "FT", "Tester", "JCB", "3566111111111113", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true, 258);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("David Martin")]
        //[Description("Performs a weekly scheduled giving for JCB.")]
        //public void ScheduledGiving_CreditCard_JCB_Weekly()
        //{
        //    // Store variables
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

        //    // Login to InFellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "QAEUNLX0C6");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Weekly("Auto Test Fund", null, "30.23", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(7).ToString("dd/MM/yyyy"), null, "FT", "Tester", "JCB", "3566111111111113", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true, 258);

        //    // Logout of infellowship
        //    test.infellowship.Logout();
        //}

        #endregion Credit Cards

        #region Personal Checks
        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Performs an one time scheduled giving for a Personal Check")]
        //public void ScheduledGiving_PersonalCheck_OneTime()
        //{
        //    // Variables
        //    var amount = Math.Round(1.0 + new Random().NextDouble() * 100, 2).ToString();

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_OneTime("Auto Test Fund", null, amount, string.Format("12/31/{0}", DateTime.Now.Year), "8176833453", "111000025", "111222333", true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Performs a monthly scheduled giving for a Personal Check")]
        //public void ScheduledGiving_PersonalCheck_Monthly()
        //{
        //    // Store variables
        //    var startDay = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Day.ToString();
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(2).ToString("MMMM");
        //    var amount = Math.Round(1.0 + new Random().NextDouble() * 100, 2).ToString();

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_Monthly("Auto Test Fund", null, amount, startDay, startMonth, DateTime.Now.Year.ToString(), null, null, "8176833453", "111000025", "111222333", true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Performs a twice monthly scheduled giving for a Personal Check starting on the first.")]
        //public void ScheduledGiving_PersonalCheck_TwiceMonthly_First()
        //{
        //    // Store variables
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
        //    var amount = Math.Round(1.0 + new Random().NextDouble() * 100, 2).ToString();

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving at a twice monthly frequency, starting on the first of the month.
        //    test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_TwiceMonthly("Auto Test Fund", null, amount, "1", startMonth, DateTime.Now.Year.ToString(), null, null, null, "8176833453", "111000025", "111222333", true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Performs a twice monthly scheduled giving for a Personal Check starting on the 16th.")]
        //public void ScheduledGiving_PersonalCheck_TwiceMonthly_16th()
        //{
        //    // Store variables
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
        //    var amount = Math.Round(1.0 + new Random().NextDouble() * 100, 2).ToString();

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving at a twice monthly frequency, starting on the first of the month.
        //    test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_TwiceMonthly("Auto Test Fund", null, amount, "16", startMonth, DateTime.Now.Year.ToString(), null, null, null, "8176833453", "111000025", "111222333", true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Performs a weekly scheduled giving for a Personal Check.")]
        //public void ScheduledGiving_PersonalCheck_Weekly()
        //{
        //    // Store variables
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");
        //    var amount = Math.Round(1.0 + new Random().NextDouble() * 100, 2).ToString();

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving at a weekly frequency
        //    test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_Weekly("Auto Test Fund", null, amount, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(7).ToString("MM/dd/yyyy"), null, "8176833453", "111000025", "111222333", true);

        //    // Logout
        //    test.infellowship.Logout();
        //}


        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Performs an every two weeks scheduled giving for a Personal Check.")]
        //public void ScheduledGiving_PersonalCheck_EveryTwoWeeks()
        //{
        //    // Store variables
        //    var startMonth = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(1).ToString("MMMM");

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving at a weekly frequency
        //    test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_EveryTwoWeeks("Auto Test Fund", null, "2.61", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(7).ToString("MM/dd/yyyy"), null, "8176833453", "111000025", "111222333", true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        #endregion Personal Checks

        #endregion Create

        #region Edit

        #region Active / Inactive
        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Updates a credit card schedule to be of the inactive and active statuses.")]
        //public void ScheduledGiving_Edit_Status_CreditCard()
        //{
        //    // Variables
        //    var amount = "9.91";

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, amount, string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Make the schedule inactive
        //    test.infellowship.Giving_ScheduledGiving_Edit_Status(15, amount, false);

        //    // Make the schedule active
        //    test.infellowship.Giving_ScheduledGiving_Edit_Status(15, amount, true);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Updates a personal check schedule to be of the inactive and active statuses.")]
        //public void ScheduledGiving_Edit_Status_PersonalCheck()
        //{
        //    // Variables
        //    var amount = "2.95";

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_OneTime("Auto Test Fund", null, amount, string.Format("12/31/{0}", DateTime.Now.Year), "8176833453", "111000025", "111222333", true);

        //    // Make the schedule inactive
        //    test.infellowship.Giving_ScheduledGiving_Edit_Status(15, amount, false);

        //    // Make the schedule active
        //    test.infellowship.Giving_ScheduledGiving_Edit_Status(15, amount, true);

        //    // Logout
        //    test.infellowship.Logout();
        //}


        #endregion Active / Inactive

        #region Where to Give
        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Updates a credit card schedule's fund, subfund and amount.")]
        //public void ScheduledGiving_Edit_WhereToGive_CreditCard()
        //{
        //    // Variables
        //    var amount = "7.91";

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, amount, string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Edit the where to give
        //    test.infellowship.Giving_ScheduledGiving_Edit_WhereToGive(amount, "1 - General Fund", null, "10.22");

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Updates a credit card schedule's fund, subfund and amount but do not specify a fund.")]
        //public void ScheduledGiving_Edit_WhereToGive_NoFund_CreditCard()
        //{
        //    // Variables
        //    var amount = "7.61";

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, amount, string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Edit the where to give but do not specify a fund.
        //    test.infellowship.Giving_ScheduledGiving_Edit_WhereToGive(amount, "--", null, "4.16");

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Updates a credit card schedule's fund, subfund and amount but do not specify an amount.")]
        //public void ScheduledGiving_Edit_WhereToGive_NoAmount_CreditCard()
        //{
        //    // Variables
        //    var amount = "3.91";

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, amount, string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Edit the where to give but do not specify an amount
        //    test.infellowship.Giving_ScheduledGiving_Edit_WhereToGive(amount, "Auto Test Fund", null, null);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Updates a personal check schedule's fund, subfund and amount.")]
        //public void ScheduledGiving_Edit_WhereToGive_PersonalCheck()
        //{
        //    // Variables
        //    var amount = "3.95";

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_OneTime("Auto Test Fund", null, amount, string.Format("12/31/{0}", DateTime.Now.Year), "8176833453", "111000025", "111222333", true);

        //    // Edit the where to give
        //    test.infellowship.Giving_ScheduledGiving_Edit_WhereToGive(amount, "1 - General Fund", null, "9.22");

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Updates a personal check schedule's fund, subfund and amount but do not provide a fund.")]
        //public void ScheduledGiving_Edit_WhereToGive_NoFund_PersonalCheck()
        //{
        //    // Variables
        //    var amount = "3.95";

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_OneTime("Auto Test Fund", null, amount, string.Format("12/31/{0}", DateTime.Now.Year), "8176833453", "111000025", "111222333", true);

        //    // Edit the where to give but do not specify a fund.
        //    test.infellowship.Giving_ScheduledGiving_Edit_WhereToGive(amount, "--", null, "4.96");

        //    // Logout
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Updates a personal check schedule's fund, subfund and amount but do not provide an amount.")]
        //public void ScheduledGiving_Edit_WhereToGive_NoAmount_PersonalCheck()
        //{
        //    // Variables
        //    var amount = "3.35";

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_OneTime("Auto Test Fund", null, amount, string.Format("12/31/{0}", DateTime.Now.Year), "8176833453", "111000025", "111222333", true);

        //    // Edit the where to give but do not specify a fund.
        //    test.infellowship.Giving_ScheduledGiving_Edit_WhereToGive(amount, "Auto Test Fund", null, string.Empty);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        #endregion Where to Give

        #region Payment Information
        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Verifies you can change the credit card when editing the payment information for a scheduled giving.")]
        //public void ScheduledGiving_Edit_PaymentInformation_Change_CreditCard()
        //{
        //    // Variables
        //    var amount = "2.29";

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, amount, string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);


        //    // Update the credit card
        //    test.infellowship.Giving_ScheduledGiving_Edit_CreditCard_PaymentInformation(amount, "FT", "Tester", "Master Card", "5555555555554444", null, null, null, null, null, null, null, null, null, null);

        //    // Logout
        //    test.infellowship.Logout();
        //}


        #endregion Payment Information

        #region Frequency

        #endregion Frequency

        #endregion Edit

        #region Delete
        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Deletes an one time scheduled giving for a credit card")]
        //public void ScheduledGiving_Delete_CreditCard_OneTime()
        //{

        //    // Variables
        //    var amount = "3.72";

        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Perform a scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, amount, string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Delete the scheduled giving
        //    test.infellowship.Giving_ScheduledGiving_Delete(amount);

        //    // Logout
        //    test.infellowship.Logout();
        //}

        #endregion Delete

        #region Validation
        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Verifies that a fund  is required for Schedule Giving.")]
        //public void ScheduledGiving_Validation_Fund_Required()
        //{
        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Create a schedule without specifying a fund
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime(null, null, "11.11", string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout of infellowship
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Verifies that an amount is required for Schedule Giving.")]
        //public void ScheduledGiving_Validation_Amount_Required()
        //{
        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Create a schedule without specifying an amount
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, null, string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout of infellowship
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Verifies that a date is required for the one time frequency.")]
        //public void ScheduledGiving_Validation_OneTime_Date_Required()
        //{
        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Create a one time schedule without providing any of the date information
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, "5.65", string.Empty, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout of infellowship
        //    test.infellowship.Logout();
        //}


        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Verifies that a date is required for the monthly frequency.")]
        //public void ScheduledGiving_Validation_Monthly_Date_Required()
        //{
        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Create a monthly schedule without providing any of the date information
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Monthly("Auto Test Fund", null, "5.65", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout of infellowship
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Verifies that a date is required for the twice monthly frequency.")]
        //public void ScheduledGiving_Validation_TwiceMonthly_Date_Required()
        //{
        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Create a monthly schedule without providing any of the date information
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_TwiceMonthly("Auto Test Fund", null, "5.65", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout of infellowship
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Verifies that a date is required for the weekly frequency.")]
        //public void ScheduledGiving_Validation_Weekly_Date_Required()
        //{
        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Create a weekly schedule without providing any of the date information
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_Weekly("Auto Test Fund", null, "5.65", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek, string.Empty, string.Empty, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout of infellowship
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Verifies that a date is required for the twice sweekly frequency.")]
        //public void ScheduledGiving_Validation_TwiceWeekly_Date_Required()
        //{
        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Create a twice weekly schedule without providing the month 
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_EveryTwoWeeks("Auto Test Fund", null, "5.65", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).DayOfWeek, string.Empty, string.Empty, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout of infellowship
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Verifies that all the payment information is required when creating a schedule usong a credit card.")]
        //public void ScheduledGiving_Validation_PaymentInformation_Required_CreditCard()
        //{
        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Create a schedule without providing the payment information
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, "5.65", string.Format("12/31/{0}", DateTime.Now.Year), null, null, null, null, null, null, null, true);

        //    // Logout of infellowship
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Verifies that first name, last name, and credit card number cannot exceed the field lengths.")]
        //public void ScheduledGiving_Validation_PaymentInformation_TextField_Lengths_CreditCard()
        //{
        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Create a schedule with a first name, last name, and credit card number that exceed the maximum field length.
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, "5.65", string.Format("12/31/{0}", DateTime.Now.Year), "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", "Visa", "411111111111111111111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, true);

        //    // Logout of infellowship
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Verifies that phone number, routing number, and account number cannot exceed the field lengths.")]
        //public void ScheduledGiving_Validation_PaymentInformation_TextField_Lengths_PersonalCheck()
        //{
        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Create a schedule with a phone number, routing number, and account number that exceed the maximum field length.
        //    test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_OneTime("Auto Test Fund", null, "5.65", string.Format("12/31/{0}", DateTime.Now.Year), "123456789012345678901234567890123456789012345678901", "123456789012345678901234567890123456789012345678901", "123456789012345678901234567890123456789012345678901", true);

        //    // Logout of infellowship
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Verifies that all the payment information is required when creating a schedule using a personal check.")]
        //public void ScheduledGiving_Validation_PaymentInformation_Required_PersonalCheck()
        //{
        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Create a schedule without providing the payment information
        //    test.infellowship.Giving_ScheduledGiving_PersonalCheck_Create_OneTime("Auto Test Fund", null, "5.65", string.Format("12/31/{0}", DateTime.Now.Year), null, null, null, true);

        //    // Logout of infellowship
        //    test.infellowship.Logout();
        //}

        ////[Test, RepeatOnFailure]
        //[Author("Bryan Mikaelian")]
        //[Description("Verifies that you must agree to the payment when submitting a schedule.")]
        //public void ScheduledGiving_Validation_AgreeToPayment_Required()
        //{
        //    // Login to infellowship
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.infellowship.Login("ft.autotester@gmail.com", "FT4life!", "dc");

        //    // Create a schedule without agreeing to the payment
        //    test.infellowship.Giving_ScheduledGiving_CreditCard_Create_OneTime("Auto Test Fund", null, "5.65", string.Format("12/31/{0}", DateTime.Now.Year), "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), null, false);

        //    // Logout of infellowship
        //    test.infellowship.Logout();
        //}

        #endregion Validation

        #endregion Schedule Giving

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("F1-4085: Verifies credit card images are present")]
        public void ScheduledGiving_CreditCard_Verify_CreditCard_Images_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("felix.gaytan@activenetwork.com", "FG.Admin12", "dc");

            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

            //Go to Give Now CC
            test.Driver.FindElementByLinkText("Your Giving").Click();
            test.Driver.FindElementByLinkText("Schedule Giving").Click();

            //Set Contribution Info
            new SelectElement(test.Driver.FindElementById("fund0")).SelectByText("Auto Test Fund");
            test.Driver.FindElementById("amount").SendKeys("5.00");
            test.Driver.FindElementById("commit").Click();


            //Set Giving Schedule Once, Monthly, Twice Monthly, Every two weeks

            //Once
            #region Once
            
            test.Driver.FindElementById("frequency_0").Click();
            test.Driver.FindElementById("one_time_process_date").SendKeys(now.ToShortDateString());

            //Commit
            test.Driver.FindElementById("commit").Click();

            //Select CC
            test.Driver.FindElementById("payment_method_cc").Click();

            if (test.GeneralMethods.IsElementPresentWebDriver(By.Id(("cc_new_card_link"))))
            {
                test.Driver.FindElementById("cc_new_card_link").Click();
            }

            //Get the CC images
            List<string> ccImgOptions = test.GeneralMethods.Get_CC_Images_Attribute();

            //Verify the CC Images
            test.GeneralMethods.Verify_CC_Images();

            //Back
            test.Driver.FindElementByLinkText("« Back").Click();
            
            #endregion Once

            //Monthly
            #region Monthly            
            test.Driver.FindElementById("frequency_1").Click();
            
            new SelectElement(test.Driver.FindElementById("start_monthly_day")).SelectByText(now.Day.ToString());
            new SelectElement(test.Driver.FindElementById("start_monthly_month")).SelectByText(now.AddMonths(2).ToString("MMMM"));
            new SelectElement(test.Driver.FindElementById("start_monthly_year")).SelectByText(now.AddYears(1).Year.ToString());

            //Commit
            test.Driver.FindElementById("commit").Click();

            //Select CC
            test.Driver.FindElementById("payment_method_cc").Click();

            if (test.GeneralMethods.IsElementPresentWebDriver(By.Id(("cc_new_card_link"))))
            {
                test.Driver.FindElementById("cc_new_card_link").Click();
            }
            
            //Get the CC images
            ccImgOptions = test.GeneralMethods.Get_CC_Images_Attribute();

            //Verify the CC Images
            test.GeneralMethods.Verify_CC_Images();

            //Back
            test.Driver.FindElementByLinkText("« Back").Click();
            #endregion Monthly

            //Twice Monthly
            #region Twice Monthly
            test.Driver.FindElementById("frequency_2").Click();
            new SelectElement(test.Driver.FindElementById("start_date_1_16")).SelectByText("1");
            new SelectElement(test.Driver.FindElementById("start_month_1_16")).SelectByText(now.AddMonths(2).ToString("MMMM"));
            new SelectElement(test.Driver.FindElementById("start_year_1_16")).SelectByText(now.AddYears(1).Year.ToString());
            test.Driver.FindElementById("frequency_2").Click();


            //Commit
            test.Driver.FindElementById("commit").Click();

            //Select CC
            test.Driver.FindElementById("payment_method_cc").Click();

            if (test.GeneralMethods.IsElementPresentWebDriver(By.Id(("cc_new_card_link"))))
            {
                test.Driver.FindElementById("cc_new_card_link").Click();
            }

            //Get the CC images
            ccImgOptions = test.GeneralMethods.Get_CC_Images_Attribute();

            //Verify the CC Images
            test.GeneralMethods.Verify_CC_Images();

            //Back
            test.Driver.FindElementByLinkText("« Back").Click();
            #endregion Twice Montly

            //Weekly
            #region Weekly
            test.Driver.FindElementById("frequency_3").Click();

            new SelectElement(test.Driver.FindElementById("weekly_start_day")).SelectByText("Monday");
            test.Driver.FindElementById("weekly_start_date").SendKeys(now.ToShortDateString());

            //Commit
            test.Driver.FindElementById("commit").Click();

            //Select CC
            test.Driver.FindElementById("payment_method_cc").Click();

            if (test.GeneralMethods.IsElementPresentWebDriver(By.Id(("cc_new_card_link"))))
            {
                test.Driver.FindElementById("cc_new_card_link").Click();
            }

            //Get the CC images
            ccImgOptions = test.GeneralMethods.Get_CC_Images_Attribute();

            //Verify the CC Images
            test.GeneralMethods.Verify_CC_Images();

            //Back
            test.Driver.FindElementByLinkText("« Back").Click();
            #endregion Weekly

            //Every Two Weeks
            #region Two Weeks
            test.Driver.FindElementById("frequency_4").Click();
            new SelectElement(test.Driver.FindElementById("alt_weekly_start_day")).SelectByText("Tuesday");
            test.Driver.FindElementById("alt_weekly_start_date").SendKeys(now.ToShortDateString());

            //Commit
            test.Driver.FindElementById("commit").Click();

            //Select CC
            test.Driver.FindElementById("payment_method_cc").Click();

            if (test.GeneralMethods.IsElementPresentWebDriver(By.Id(("cc_new_card_link"))))
            {
                test.Driver.FindElementById("cc_new_card_link").Click();
            }

            //Get the CC images
            ccImgOptions = test.GeneralMethods.Get_CC_Images_Attribute();

            //Verify the CC Images
            test.GeneralMethods.Verify_CC_Images();
            #endregion Two Weeks

            //Back
            test.Driver.FindElementByLinkText("« Back").Click();
            test.Driver.FindElementByLinkText("« Back").Click();
            test.Driver.FindElementByLinkText("Cancel").Click();

            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("FO-7353: The image of the Previously used Card  is same in the Schedule Giving of the Infellowship")]
        public void ScheduledGiving_CreditCard_Verify_UsedCard_Images_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

            //Go to Give Now CC
            test.Driver.FindElementByLinkText("Your Giving").Click();
            test.Driver.FindElementByLinkText("Schedule Giving").Click();

            //Set Contribution Info
            new SelectElement(test.Driver.FindElementById("fund0")).SelectByText("Auto Test Fund");
            test.Driver.FindElementById("amount").SendKeys("5.00");
            test.Driver.FindElementById("commit").Click();


            //Set Giving Schedule Once, Monthly, Twice Monthly, Every two weeks

            //Once
            #region Once

            test.Driver.FindElementById("frequency_0").Click();
            test.Driver.FindElementById("one_time_process_date").SendKeys(now.ToShortDateString());

            //Commit
            test.Driver.FindElementById("commit").Click();

            //Select CC
            test.Driver.FindElementById("payment_method_cc").Click();

            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.visa").Text.StartsWith("Visa"), "Used visa card isn't display as expected");
            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.mastercard").Text.StartsWith("Mastercard"), "Used mastercard isn't display as expected");
            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.amex").Text.StartsWith("AMEX"), "Used AMEX card isn't display as expected");
            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.discover").Text.StartsWith("Discover"), "Used discover card isn't display as expected");

            //Back
            test.Driver.FindElementByLinkText("« Back").Click();

            #endregion Once

            //Monthly
            #region Monthly
            test.Driver.FindElementById("frequency_1").Click();

            new SelectElement(test.Driver.FindElementById("start_monthly_day")).SelectByText(now.Day.ToString());
            new SelectElement(test.Driver.FindElementById("start_monthly_month")).SelectByText(now.AddMonths(2).ToString("MMMM"));
            new SelectElement(test.Driver.FindElementById("start_monthly_year")).SelectByText(now.AddYears(1).Year.ToString());

            //Commit
            test.Driver.FindElementById("commit").Click();

            //Select CC
            test.Driver.FindElementById("payment_method_cc").Click();

            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.visa").Text.StartsWith("Visa"), "Used visa card isn't display as expected");
            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.mastercard").Text.StartsWith("Mastercard"), "Used mastercard isn't display as expected");
            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.amex").Text.StartsWith("AMEX"), "Used AMEX card isn't display as expected");
            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.discover").Text.StartsWith("Discover"), "Used discover card isn't display as expected");

            //Back
            test.Driver.FindElementByLinkText("« Back").Click();
            #endregion Monthly

            //Twice Monthly
            #region Twice Monthly
            test.Driver.FindElementById("frequency_2").Click();
            new SelectElement(test.Driver.FindElementById("start_date_1_16")).SelectByText("1");
            new SelectElement(test.Driver.FindElementById("start_month_1_16")).SelectByText(now.AddMonths(2).ToString("MMMM"));
            new SelectElement(test.Driver.FindElementById("start_year_1_16")).SelectByText(now.AddYears(1).Year.ToString());
            test.Driver.FindElementById("frequency_2").Click();


            //Commit
            test.Driver.FindElementById("commit").Click();

            //Select CC
            test.Driver.FindElementById("payment_method_cc").Click();

            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.visa").Text.StartsWith("Visa"), "Used visa card isn't display as expected");
            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.mastercard").Text.StartsWith("Mastercard"), "Used mastercard isn't display as expected");
            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.amex").Text.StartsWith("AMEX"), "Used AMEX card isn't display as expected");
            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.discover").Text.StartsWith("Discover"), "Used discover card isn't display as expected");

            //Back
            test.Driver.FindElementByLinkText("« Back").Click();
            #endregion Twice Montly

            //Weekly
            #region Weekly
            test.Driver.FindElementById("frequency_3").Click();

            new SelectElement(test.Driver.FindElementById("weekly_start_day")).SelectByText("Monday");
            test.Driver.FindElementById("weekly_start_date").SendKeys(now.ToShortDateString());

            //Commit
            test.Driver.FindElementById("commit").Click();

            //Select CC
            test.Driver.FindElementById("payment_method_cc").Click();

            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.visa").Text.StartsWith("Visa"), "Used visa card isn't display as expected");
            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.mastercard").Text.StartsWith("Mastercard"), "Used mastercard isn't display as expected");
            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.amex").Text.StartsWith("AMEX"), "Used AMEX card isn't display as expected");
            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.discover").Text.StartsWith("Discover"), "Used discover card isn't display as expected");

            //Back
            test.Driver.FindElementByLinkText("« Back").Click();
            #endregion Weekly

            //Every Two Weeks
            #region Two Weeks
            test.Driver.FindElementById("frequency_4").Click();
            new SelectElement(test.Driver.FindElementById("alt_weekly_start_day")).SelectByText("Tuesday");
            test.Driver.FindElementById("alt_weekly_start_date").SendKeys(now.ToShortDateString());

            //Commit
            test.Driver.FindElementById("commit").Click();

            //Select CC
            test.Driver.FindElementById("payment_method_cc").Click();

            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.visa").Text.StartsWith("Visa"), "Used visa card isn't display as expected");
            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.mastercard").Text.StartsWith("Mastercard"), "Used mastercard isn't display as expected");
            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.amex").Text.StartsWith("AMEX"), "Used AMEX card isn't display as expected");
            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.discover").Text.StartsWith("Discover"), "Used discover card isn't display as expected");

            #endregion Two Weeks

            //Back
            test.Driver.FindElementByLinkText("« Back").Click();
            test.Driver.FindElementByLinkText("« Back").Click();
            test.Driver.FindElementByLinkText("Cancel").Click();

            test.Infellowship.LogoutWebDriver();

        }
    
        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4169: Verifies credit card Security Tool tip information")]
        public void ScheduledGiving_CreditCard_Verify_SecurityCode_Tooltip_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.tester2@gmail.com", "FT4life!", "dc");

            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

            //Go to Give Now CC
            test.Driver.FindElementByLinkText("Your Giving").Click();
            test.Driver.FindElementByLinkText("Schedule Giving").Click();

            //Set Contribution Info
            new SelectElement(test.Driver.FindElementById("fund0")).SelectByText("Auto Test Fund");
            test.Driver.FindElementById("amount").SendKeys("5.00");
            test.Driver.FindElementById("commit").Click();

            //Set Giving Schedule Once, Monthly, Twice Monthly, Every two weeks

            //Once
            #region Once
            
            test.Driver.FindElementById("frequency_0").Click();
            test.Driver.FindElementById("one_time_process_date").SendKeys(now.ToShortDateString());

            //Commit
            test.Driver.FindElementById("commit").Click();

            //Select CC
            test.Driver.FindElementById("payment_method_cc").Click();
            test.Driver.FindElementById("cc_new_card_link").Click();

            //Verify Security code tool tip
            test.GeneralMethods.Verify_SchedGiving_SecurityCode_Tooltip();
            

            //Back
            test.Driver.FindElementByLinkText(GeneralInFellowship.Giving.Back).Click();

            #endregion Once

            //Monthly
            #region Monthly            
            test.Driver.FindElementById("frequency_1").Click();
            
            new SelectElement(test.Driver.FindElementById("start_monthly_day")).SelectByText(now.Day.ToString());
            new SelectElement(test.Driver.FindElementById("start_monthly_month")).SelectByText(now.AddMonths(2).ToString("MMMM"));
            new SelectElement(test.Driver.FindElementById("start_monthly_year")).SelectByText(now.AddYears(1).Year.ToString());

            //Commit
            test.Driver.FindElementById("commit").Click();

            //Select CC
            test.Driver.FindElementById("payment_method_cc").Click();
            test.Driver.FindElementById("cc_new_card_link").Click();

            //Verify Security code tool tip
            test.GeneralMethods.Verify_SchedGiving_SecurityCode_Tooltip();

            //Back
            test.Driver.FindElementByLinkText(GeneralInFellowship.Giving.Back).Click();
            #endregion Monthly

            //Twice Monthly
            #region Twice Monthly
            test.Driver.FindElementById("frequency_2").Click();
            new SelectElement(test.Driver.FindElementById("start_date_1_16")).SelectByText("1");
            new SelectElement(test.Driver.FindElementById("start_month_1_16")).SelectByText(now.AddMonths(2).ToString("MMMM"));
            new SelectElement(test.Driver.FindElementById("start_year_1_16")).SelectByText(now.AddYears(1).Year.ToString());
            test.Driver.FindElementById("frequency_2").Click();


            //Commit
            test.Driver.FindElementById("commit").Click();

            //Select CC
            test.Driver.FindElementById("payment_method_cc").Click();
            test.Driver.FindElementById("cc_new_card_link").Click();

            //Verify Security code tool tip
            test.GeneralMethods.Verify_SchedGiving_SecurityCode_Tooltip();

            //Back
            test.Driver.FindElementByLinkText(GeneralInFellowship.Giving.Back).Click();
            #endregion Twice Montly

            //Weekly
            #region Weekly
            test.Driver.FindElementById("frequency_3").Click();

            new SelectElement(test.Driver.FindElementById("weekly_start_day")).SelectByText("Monday");
            test.Driver.FindElementById("weekly_start_date").SendKeys(now.ToShortDateString());

            //Commit
            test.Driver.FindElementById("commit").Click();

            //Select CC
            test.Driver.FindElementById("payment_method_cc").Click();
            test.Driver.FindElementById("cc_new_card_link").Click();

            //Verify Security code tool tip
            test.GeneralMethods.Verify_SchedGiving_SecurityCode_Tooltip();
            
            //Back
            test.Driver.FindElementByLinkText(GeneralInFellowship.Giving.Back).Click();
            #endregion Weekly

            //Every Two Weeks
            #region Two Weeks
            test.Driver.FindElementById("frequency_4").Click();
            new SelectElement(test.Driver.FindElementById("alt_weekly_start_day")).SelectByText("Tuesday");
            test.Driver.FindElementById("alt_weekly_start_date").SendKeys(now.ToShortDateString());

            //Commit
            test.Driver.FindElementById("commit").Click();

            //Select CC
            test.Driver.FindElementById("payment_method_cc").Click();
            test.Driver.FindElementById("cc_new_card_link").Click();

            //Verify Security code tool tip
            test.GeneralMethods.Verify_SchedGiving_SecurityCode_Tooltip();
            

            #endregion Two Weeks

            //Back
            test.Driver.FindElementByLinkText(GeneralInFellowship.Giving.Back).Click();
            test.Driver.FindElementByLinkText(GeneralInFellowship.Giving.Back).Click();
            test.Driver.FindElementByLinkText("Cancel").Click();

            test.Infellowship.LogoutWebDriver();

        }


        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Jim Jin")]
        [Description("FO-2886 INC2287198 - Subfund does not automatically populate if Give Now has been turned off")]
        public void ScheduledGiving_Verify_Subfund_When_OnlineGiving_TurnedOff_WebDriver()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            //Test data parameters
            int church_id = 15;
            string fund_name = utility.GetUniqueName("fund");
            string sub_fund_name = utility.GetUniqueName("subfund");
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

            try
            {
                //Test data SQL preparation
                test.SQL.Giving_Funds_Create(church_id, fund_name, true, "TF-AUTO", 1, true, "Auto Testing");
                test.SQL.Giving_SubFunds_Create(church_id, fund_name, sub_fund_name, "TSF-AUTO", true, "GL-AUTO", true);

                //Login to infellowship
                test.Infellowship.LoginWebDriver("ft.tester2@gmail.com", "FT4life!", "dc");

                //Click link "Schedule Giving", and go to "Where to Give" page(step 1)
                utility.WaitAndGetElement(By.LinkText("Your Giving")).Click();
                //utility.waitForElementAppear("$(\"a[href='/OnlineGiving/ScheduledGiving/Step1']\")", 100);
                utility.WaitAndGetElement(By.LinkText("Schedule Giving")).Click();

                //Select fund
                new SelectElement(utility.WaitAndGetElement(By.Id("fund0"))).SelectByText(fund_name);

                //Assert that no duplicated subfund options
                utility.waitForElementIsEnabled("document.getElementById('sub_fund')");
                SelectElement selectSubFund = new SelectElement(utility.WaitAndGetElement(By.Id("sub_fund")));
                Assert.AreEqual(selectSubFund.Options.Count, 2);

                //Select subfund, enter amount, then commit form
                selectSubFund.SelectByText(sub_fund_name);
                utility.WaitAndGetElement(By.Id("amount")).SendKeys("100");
                utility.WaitAndGetElement(By.Id("commit")).Click();

                //Verify it will go to Giving Schedule page(step 2)
                string css_value = test.JavaScriptExecutor.ExecuteScript("return $(\"strong:contains('Giving Schedule')\").parent().parent().attr('class')").ToString();
                Assert.Contains(css_value, "uiStepWizardCurrentStep    uiStepWizardCurrentStepRemovesteps", "Current step isn't Giving Schehule");
            }
            finally
            {
                //clear test data
                test.SQL.Giving_Funds_Delete(church_id, fund_name);
                test.SQL.Giving_SubFunds_Delete(church_id, fund_name, sub_fund_name);
            }

        }
}

}