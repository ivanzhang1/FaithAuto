using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Data;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Net;
using System.IO;
using System.Collections;

namespace FTTests.Dashboard.Giving
{
    [TestFixture]
    public class Dashboard_Giving : FixtureBaseWebDriver 
    {
        [Test, MultipleAsserts]
        [Author("Jim Jin")]
        [Description("Verifies new giving under contribution fund will be included by dashboard")]
        public void Dashboard_Giving_NewGiving_Of_ContributionFund()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            //Test data parameters
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
            int userId = test.SQL.User_FetchID(churchId, test.Dashboard.DashboardEmail, test.Dashboard.DashboardUsername);
            string fund_name_1 = utility.GetUniqueName("fund");
            string card_number = "4111111111111111";
            string card_type = "Visa";

            try
            {  
                test.SQL.Giving_Funds_Create(churchId, fund_name_1, true, null, 1, true, "Auto Testing");

                test.Dashboard.LoginWebDriver();
                test.GeneralMethods.WaitForPageIsLoaded();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);
                home.openSettingsPage();
                settings.turnOnWidget(1);
                settings.closeSettingsPage();
                home.selectView("year");

                //string bigNumber = home.getWidgetBigNumber(1);
                //double subTotalOnPage = double.Parse(bigNumber.Replace("$", "").Replace(",", ""));

                //Do an online giving
                test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!");
                test.Infellowship.Giving_GiveNow_CreditCard_WithoutValidation_WebDriver(fund_name_1, "10000", card_type, card_number, 12, (DateTime.Now.Year + 5).ToString());
                test.Infellowship.LogoutWebDriver();

                test.Dashboard.OpenLoginWebDriver();
                home.openSettingsPage();
                settings.checkSubItemByName(1, fund_name_1);
                settings.closeSettingsPage();

                home.selectView("year");

                string bigNumberNew = home.getWidgetBigNumber(1);
                //double subTotalOnPageNew = double.Parse(bigNumberNew.Replace("$", "").Replace(",", ""));

                DateTime[] dateRange = test.Dashboard.getDateRange(now,"year");
                double subTotalInDb = test.SQL.Dashboard_Giving_GetPeriodSum(churchId, userId, dateRange[0], dateRange[1]);

                //verify the big number on page is correct
                //TestLog.WriteLine(string.Format("{0}|{1}|{2}", subTotalOnPage, subTotalInDb, subTotalOnPageNew));
                //Assert.IsTrue(subTotalInDb>=subTotalOnPage + 10000);
                //Assert.AreEqual(subTotalInDb, subTotalOnPageNew);
                Assert.AreEqual(bigNumberNew, test.Dashboard.getBigNumberStringGreaterThanMillion(subTotalInDb));
            }
            finally
            {
                //clear test data
                test.SQL.Giving_Funds_Delete(churchId, fund_name_1);
            }
        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify the giving area will be removed from dashboard page if turned off giving widget")]
        public void Dashboard_Home_Giving_TurnOff()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            try
            {
                test.Dashboard.LoginWebDriver();
                test.GeneralMethods.WaitForPageIsLoaded();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                home.openSettingsPage();
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                settings.turnOffWidget(1);
                settings.closeSettingsPage();

                //verify the giving widget is removed
                Assert.IsFalse(home.getWidgetName(1).ToLower().Contains("giving"));

                home.openSettingsPage();
                settings.turnOnWidget(1);
                settings.closeSettingsPage();

                //verify the giving widget is back
                Assert.IsTrue(home.getWidgetName(1).ToLower().Contains("giving"));
            }
            finally
            {
                //clear test data
            }
        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify the giving big number on dashboard page is correct by default")]
        public void Dashboard_Home_Giving_BigNumber_ByDefault()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

            try
            {
                int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
                int userId = test.SQL.User_FetchID(churchId, test.Dashboard.DashboardEmail, test.Dashboard.DashboardUsername);
                test.SQL.Dashboard_ClearData(churchId, test.Dashboard.DashboardUsername);

                test.Dashboard.LoginWebDriver();
                test.GeneralMethods.WaitForPageIsLoaded();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);
                home.openSettingsPage();
                settings.turnOnWidget(1);
                settings.closeSettingsPage();

                string bigNumber = home.getWidgetBigNumber(1);
                double subTotalOnPage = double.Parse(bigNumber.Replace("$", "").Replace(",", ""));

                DateTime[] dateRange = test.Dashboard.getDateRange(now, "week", 1);
                double subTotalInDb = test.SQL.Dashboard_Giving_GetPeriodSum(churchId, userId, dateRange[0], dateRange[1]);
                
                //verify the default big number on page is correct
                TestLog.WriteLine(string.Format("{0}|{1}", subTotalOnPage, subTotalInDb));
                Assert.AreEqual(subTotalOnPage, subTotalInDb);
            }
            finally
            {
                //clear test data
            }
        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify the giving big number on dashboard page is correct against different start day of week")]
        public void Dashboard_Home_Giving_BigNumber_DifferentStartDay()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string[] startDays = new string[] { "sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday" };
            
            try
            {
                int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
                int userId = test.SQL.User_FetchID(churchId, test.Dashboard.DashboardEmail, test.Dashboard.DashboardUsername);

                test.Dashboard.LoginWebDriver();
                test.GeneralMethods.WaitForPageIsLoaded();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);
                home.openSettingsPage();
                settings.turnOnWidget(1);
                settings.closeSettingsPage();

                home.selectView("week");

                foreach (string startDay in startDays)
                {
                    home.selectStartDay(startDay);

                    string bigNumber = home.getWidgetBigNumber(1);
                    double subTotalOnPage = double.Parse(bigNumber.Replace("$", "").Replace(",", ""));

                    DateTime[] dateRange = test.Dashboard.getDateRange(now, "week", test.Dashboard.getIntStartDay(startDay));
                    double subTotalInDb = test.SQL.Dashboard_Giving_GetPeriodSum(churchId, userId, dateRange[0], dateRange[1]);

                    //verify the big number on page is correct
                    TestLog.WriteLine(string.Format("{0}|{1}", subTotalOnPage, subTotalInDb));
                    Assert.AreEqual(subTotalOnPage, subTotalInDb);
                }
            }
            finally
            {
                //clear test data
            }
        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify the giving big number on dashboard page is correct against different view type")]
        public void Dashboard_Home_Giving_BigNumber_DifferentViewType()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string[] viewTypes = new string[] { "month", "quarter", "year" };

            try
            {
                int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
                int userId = test.SQL.User_FetchID(churchId, test.Dashboard.DashboardEmail, test.Dashboard.DashboardUsername);

                test.Dashboard.LoginWebDriver();
                test.GeneralMethods.WaitForPageIsLoaded();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);
                home.openSettingsPage();
                settings.turnOnWidget(1);
                settings.closeSettingsPage();

                foreach (string viewType in viewTypes)
                {
                    home.selectView(viewType);

                    string bigNumber = home.getWidgetBigNumber(1);
                    double subTotalOnPage = double.Parse(bigNumber.Replace("$", "").Replace(",", ""));

                    DateTime[] dateRange = test.Dashboard.getDateRange(now,viewType);
                    double subTotalInDb = test.SQL.Dashboard_Giving_GetPeriodSum(churchId, userId, dateRange[0], dateRange[1]);

                    //verify the big number on page is correct
                    TestLog.WriteLine(string.Format("{0}|{1}", subTotalOnPage, subTotalInDb));
                    Assert.AreEqual(subTotalOnPage, subTotalInDb);
                }
            }
            finally
            {
                //clear test data
            }
        }

        [Test, MultipleAsserts, Timeout(10000)]
        [Author("Jim Jin")]
        [Description("Verify the giving big number on dashboard page is correct even turn off all funds")]
        public void Dashboard_Home_Giving_BigNumber_TurnOffAllFunds()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string[] viewTypes = new string[] { "week", "month", "quarter", "year" };

            try
            {
                int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
                int userId = test.SQL.User_FetchID(churchId, test.Dashboard.DashboardEmail, test.Dashboard.DashboardUsername);

                test.Dashboard.LoginWebDriver();
                test.GeneralMethods.WaitForPageIsLoaded();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                home.openSettingsPage();

                int totalItems = settings.getWidgetSubItemsOnPage(1).Count;

                for (int i = 1; i <= totalItems; i++)
                {
                    settings.uncheckWidgetItem(1, i);
                }

                settings.closeSettingsPage();

                foreach (string viewType in viewTypes)
                {
                    home.selectView(viewType);

                    string bigNumber = home.getWidgetBigNumber(1);
                    double subTotalOnPage = double.Parse(bigNumber.Replace("$", "").Replace(",", ""));

                    DateTime[] dateRange = test.Dashboard.getDateRange(now, viewType, 1);
                    double subTotalInDb = test.SQL.Dashboard_Giving_GetPeriodSum(churchId, userId, dateRange[0], dateRange[1]);

                    //verify the big number on page is correct
                    TestLog.WriteLine(string.Format("{0}|{1}", subTotalOnPage, subTotalInDb));
                    Assert.AreEqual(subTotalOnPage, subTotalInDb);
                    Assert.AreEqual(subTotalOnPage, 0);
                }

                home.openSettingsPage();

                for (int i = 1; i <= totalItems; i++)
                {
                    settings.checkWidgetItem(1, i);
                }
            }
            finally
            {
                //clear test data
                test.SQL.Dashboard_ClearData(test.SQL.FetchChurchID(test.Dashboard.ChurchCode), test.Dashboard.DashboardUsername);
            }
        }

    }
}
