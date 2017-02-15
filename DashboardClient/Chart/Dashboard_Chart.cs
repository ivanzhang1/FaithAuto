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

namespace FTTests.Dashboard.Chart
{
    [TestFixture]
    class Dashboard_Chart : FixtureBaseWebDriver
    {
        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify Chart can be folded and unfolded")]
        public void Dashboard_Chart_ExpandAndUnexpand()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                home.openSettingsPage();
                int widgetTotal = settings.getWidgetsTotalOnPage();
                int turnedOnWidgetTotal = settings.getTurnedOnWidgetsTotalOnPage(widgetTotal);

                if (!settings.isWidgetTurnedOn(1))
                {
                    settings.turnOnWidget(1);
                }

                settings.closeSettingsPage();

                int widgetTotalOnHomePage = home.getWidgetsTotalOnPage();

                Assert.AreEqual(turnedOnWidgetTotal, widgetTotalOnHomePage, "Total turned on widgets on settings page should equal to the number of widget on dashboard page");

                home.unfoldGivingChart();
                Assert.IsTrue(home.isGivingChartExpanded(), "Giving chart is not expanded, or this user has no permession to access giving report");

                home.foldGivingChart();
                home.selectView("quarter");
                Assert.IsFalse(home.isGivingChartExpanded(), "Giving chart is not folded");
            }
            finally
            {
                //clear test data
            }
        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify check/uncheck fund in chart will be saved")]
        public void Dashboard_Chart_Save_CheckedItems()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;
            string widgetName = "Giving";
            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                home.openSettingsPage();
                int widgetItemTotal = settings.getWidgetSubItemsOnPage(1).Count;
                settings.turnOnWidget(1);
                for(int i=0; i<widgetItemTotal; i++)
                {
                    settings.checkWidgetItem(1, i+1);
                }

                settings.closeSettingsPage();
                home.unfoldGivingChart();

                for(int i=0; i<widgetItemTotal; i++)
                {
                    Assert.IsTrue(home.isChartItemChecked(1, i + 1, widgetName), string.Format("{0}th checkbox is not checked", i + 1));
                }

                if (widgetItemTotal > 0)
                {
                    home.uncheckChartItem(1, 1, widgetName);
                }

                home.foldGivingChart();

                int userWidgetItemID = home.getUserWidgetItemID(1, 1, widgetName);
                bool chartItemIsChecked = test.SQL.Dashboard_Widget_Item_Is_Checked_InChart(userWidgetItemID);
                Assert.IsFalse(chartItemIsChecked, "unchecked chart item has been saved to DB");

                home.ClickWidgetName(2);
                home.unfoldGivingChart();

                Assert.IsFalse(home.isChartItemChecked(1, 1, widgetName), "1th checkbox should not be checked");
            }
            finally
            {
                //clear test data
            }
        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify widgets order on dashboard page should be same to the settings page")]
        public void Dashboard_Chart_WidgetsOrder()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                home.openSettingsPage();
                int widgetTotal = settings.getWidgetsTotalOnPage();
                //int turnedOnWidgetTotal = settings.getTurnedOnWidgetsTotalOnPage(widgetTotal);

                ArrayList turnedOnWidgets = settings.getTurnedOnWidgetsNamesOnPage(widgetTotal);
                settings.closeSettingsPage();

                TestLog.WriteLine(string.Format("There are {0} turned on widgets", turnedOnWidgets.Count));
                for(int i =0; i<turnedOnWidgets.Count; i++)
                {
                    string widgetName = home.getWidgetName(i+1);
                    TestLog.WriteLine(string.Format("{0}|{1}", turnedOnWidgets.ToArray()[i].ToString(), widgetName));
                    Assert.IsTrue(turnedOnWidgets.ToArray()[i].ToString().Contains(widgetName));
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
        [Description("Verify yearly giving chart data is correct")]
        public void Dashboard_Chart_Giving_Yearly()
        {
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
            int userId = test.SQL.User_FetchID(churchId, test.Dashboard.DashboardEmail, test.Dashboard.DashboardUsername);
            GeneralMethods utility = test.GeneralMethods;

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                home.openSettingsPage();
                int widgetTotal = settings.getWidgetsTotalOnPage();
                int turnedOnWidgetTotal = settings.getTurnedOnWidgetsTotalOnPage(widgetTotal);

                if (!settings.isWidgetTurnedOn(1))
                {
                    settings.turnOnWidget(1);
                }

                settings.closeSettingsPage();

                home.unfoldGivingChart();
                home.selectView("year");

                ArrayList numbers = home.getPrasedGivingChartNumber();
                IEnumerator enumerator = numbers.GetEnumerator();
                ArrayList dates = test.Dashboard.getStartDatesOfChart(now, "year");

                foreach (DateTime start in dates)
                {
                    DateTime end = start.AddYears(1).AddDays(-1);
                    double subTotal = test.SQL.Dashboard_Giving_GetPeriodSum_Chart(churchId, userId, start, end);
                    if (enumerator.MoveNext())
                    {
                        TestLog.WriteLine(string.Format("{0}|{1}", subTotal, enumerator.Current.ToString()));
                        Assert.AreEqual(subTotal, double.Parse(enumerator.Current.ToString()), "The giving total of at least one year is not correct");
                    }
                    
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
        [Description("Verify weekly giving chart data is correct")]
        public void Dashboard_Chart_Giving_Weekly()
        {
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
            int userId = test.SQL.User_FetchID(churchId, test.Dashboard.DashboardEmail, test.Dashboard.DashboardUsername);
            GeneralMethods utility = test.GeneralMethods;

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                home.openSettingsPage();
                int widgetTotal = settings.getWidgetsTotalOnPage();
                int turnedOnWidgetTotal = settings.getTurnedOnWidgetsTotalOnPage(widgetTotal);
                int startDay = home.getCurrentStartDay();

                if (!settings.isWidgetTurnedOn(1))
                {
                    settings.turnOnWidget(1);
                }

                settings.closeSettingsPage();

                home.unfoldGivingChart();
                home.selectView("week");

                ArrayList numbers = home.getPrasedGivingChartNumber();
                TestLog.WriteLine(numbers.ToArray().ToString());
                IEnumerator enumerator = numbers.GetEnumerator();
                ArrayList dates = test.Dashboard.getStartDatesOfChart(now, "week", startDay);

                foreach (DateTime start in dates)
                {
                    DateTime end = start.AddDays(6);
                    double subTotal = test.SQL.Dashboard_Giving_GetPeriodSum_Chart(churchId, userId, start, end);
                    if (enumerator.MoveNext())
                    {
                        TestLog.WriteLine(string.Format("{0}|{1}", subTotal, enumerator.Current.ToString()));
                        Assert.AreEqual(subTotal, double.Parse(enumerator.Current.ToString()), "The giving total of at least one week is not correct");
                    }

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
        [Description("Verify yearly comparison data is correct")]
        public void Dashboard_Chart_Giving_Comparison_Yearly()
        {
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
            int userId = test.SQL.User_FetchID(churchId, test.Dashboard.DashboardEmail, test.Dashboard.DashboardUsername);
            GeneralMethods utility = test.GeneralMethods;

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                home.openSettingsPage();
                int widgetTotal = settings.getWidgetsTotalOnPage();
                int turnedOnWidgetTotal = settings.getTurnedOnWidgetsTotalOnPage(widgetTotal);
  
                if (!settings.isWidgetTurnedOn(1))
                {
                    settings.turnOnWidget(1);
                }

                settings.closeSettingsPage();

                home.selectView("year");
                string comparisonText = home.getComparisonText(1);
                string icon1 = home.getComparisonIcon1(1);

                double thisYearSubTotal = test.SQL.Dashboard_Giving_GetPeriodSum_Chart(churchId, userId, new DateTime(now.Year, 1, 1), new DateTime(now.Year, 12, 31));
                double lastYearSubTotal = test.SQL.Dashboard_Giving_GetPeriodSum_Chart(churchId, userId, new DateTime(now.Year-1, 1, 1), new DateTime(now.Year-1, 12, 31));
                double last2YearSubTotal = test.SQL.Dashboard_Giving_GetPeriodSum_Chart(churchId, userId, new DateTime(now.Year - 2, 1, 1), new DateTime(now.Year - 2, 12, 31));

                string yearComparisonPercentage = string.Format("{0:n}", (thisYearSubTotal / lastYearSubTotal - 1) * 100);
                yearComparisonPercentage = test.Dashboard.removeZeroInTheEndOfDecimalpart(yearComparisonPercentage);

                Assert.IsTrue(comparisonText.Contains(string.Format("{0}% From last year", yearComparisonPercentage)), "The percentage of yearly comparison is not correct!");

                if (thisYearSubTotal > lastYearSubTotal)
                {
                    Assert.IsTrue(icon1.Contains("positive-arrow"), "The icon of year comparison is not correct!");
                }
                else if (thisYearSubTotal < lastYearSubTotal)
                {
                    Assert.IsTrue(icon1.Contains("negative-arrow"), "The icon of year comparison is not correct!");
                }
                else 
                {
                    Assert.IsTrue(icon1.Contains("flat"), "The icon of year comparison is not correct!");
                }

                home.unfoldGivingChart();
                
                string thisYearDetail = home.getThisYearDetail(1, "Giving");
                string lastYearDetail = home.getLastYearDetail(1, "Giving");
                string last2YearDetail = home.getLast2YearDetail(1, "Giving");
                Assert.IsTrue(thisYearDetail.Contains(string.Format("{0}% From year prior ${1}", yearComparisonPercentage, test.Dashboard.removeZeroInTheEndOfDecimalpart(string.Format("{0:n}", lastYearSubTotal)))), "The percentage of yearly comparison or total giving of last year is not correct!");
                Assert.IsTrue(thisYearDetail.Contains(test.Dashboard.removeZeroInTheEndOfDecimalpart(string.Format("{0:n}", thisYearSubTotal))), "The total giving of this year is not correct!");
                Assert.IsTrue(lastYearDetail.Contains(test.Dashboard.removeZeroInTheEndOfDecimalpart(string.Format("{0:n}", lastYearSubTotal))), "The total giving of last year is not correct!");
                Assert.IsTrue(last2YearDetail.Contains(test.Dashboard.removeZeroInTheEndOfDecimalpart(string.Format("{0:n}", last2YearSubTotal))), "The total giving of the year before last is not correct!");
            }
            finally
            {
                //clear test data
            }
        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify quarterly comparison data is correct")]
        public void Dashboard_Chart_Giving_Comparison_Quarterly()
        {
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
            int userId = test.SQL.User_FetchID(churchId, test.Dashboard.DashboardEmail, test.Dashboard.DashboardUsername);
            GeneralMethods utility = test.GeneralMethods;

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                home.openSettingsPage();
                int widgetTotal = settings.getWidgetsTotalOnPage();
                int turnedOnWidgetTotal = settings.getTurnedOnWidgetsTotalOnPage(widgetTotal);

                if (!settings.isWidgetTurnedOn(1))
                {
                    settings.turnOnWidget(1);
                }

                settings.closeSettingsPage();

                home.selectView("quarter");
                string comparisonText = home.getComparisonText(1);
                string icon1 = home.getComparisonIcon1(1);
                string icon2 = home.getComparisonIcon1(1);

                DateTime quarterStartDay = test.Dashboard.getStartDateOfQuarter(now);
                DateTime quarterEndDay = quarterStartDay.AddMonths(3).AddDays(-1);
                double thisYearQuarterSubTotal = test.SQL.Dashboard_Giving_GetPeriodSum_Chart(churchId, userId, quarterStartDay, quarterEndDay);
                double thisYearLastQuarterSubTotal = test.SQL.Dashboard_Giving_GetPeriodSum_Chart(churchId, userId, quarterStartDay.AddMonths(-3), quarterEndDay.AddMonths(-3));
                double lastYearQuarterSubTotal = test.SQL.Dashboard_Giving_GetPeriodSum_Chart(churchId, userId, quarterStartDay.AddYears(-1), quarterEndDay.AddYears(-1));
                double last2YearQuarterSubTotal = test.SQL.Dashboard_Giving_GetPeriodSum_Chart(churchId, userId, quarterStartDay.AddYears(-2), quarterEndDay.AddYears(-2));

                string lastQuarterComparisonPercentage = string.Format("{0:n}", (thisYearQuarterSubTotal / thisYearLastQuarterSubTotal - 1) * 100);
                string quarterOfLastYearComparisonPercentage = string.Format("{0:n}", (thisYearQuarterSubTotal / lastYearQuarterSubTotal - 1) * 100);
                lastQuarterComparisonPercentage = test.Dashboard.removeZeroInTheEndOfDecimalpart(lastQuarterComparisonPercentage);
                quarterOfLastYearComparisonPercentage = test.Dashboard.removeZeroInTheEndOfDecimalpart(quarterOfLastYearComparisonPercentage);

                Assert.IsTrue(comparisonText.Contains(string.Format("{0}% From last quarter", lastQuarterComparisonPercentage)), "The percentage of quarterly comparison is not correct!");
                Assert.IsTrue(comparisonText.Contains(string.Format("{0}% From last year", quarterOfLastYearComparisonPercentage)), "The percentage of quarterly comparison is not correct!");

                if (thisYearQuarterSubTotal > thisYearLastQuarterSubTotal)
                {
                    Assert.IsTrue(icon1.Contains("positive-arrow"), "The icon of comparison is not correct!");
                }
                else if (thisYearQuarterSubTotal < thisYearLastQuarterSubTotal)
                {
                    Assert.IsTrue(icon1.Contains("negative-arrow"), "The icon of comparison is not correct!");
                }
                else
                {
                    Assert.IsTrue(icon1.Contains("flat"), "The icon of comparison is not correct!");
                }

                if (thisYearQuarterSubTotal > lastYearQuarterSubTotal)
                {
                    Assert.IsTrue(icon2.Contains("positive-arrow"), "The icon of comparison is not correct!");
                }
                else if (thisYearQuarterSubTotal < lastYearQuarterSubTotal)
                {
                    Assert.IsTrue(icon2.Contains("negative-arrow"), "The icon of comparison is not correct!");
                }
                else
                {
                    Assert.IsTrue(icon2.Contains("flat"), "The icon of comparison is not correct!");
                }

                home.unfoldGivingChart();

                string thisYearDetail = home.getThisYearDetail(1, "Giving");
                string lastYearDetail = home.getLastYearDetail(1, "Giving");
                string last2YearDetail = home.getLast2YearDetail(1, "Giving");
                Assert.IsTrue(thisYearDetail.Contains(string.Format("{0}% From quarter prior ${1}", lastQuarterComparisonPercentage, test.Dashboard.removeZeroInTheEndOfDecimalpart(string.Format("{0:n}", thisYearLastQuarterSubTotal)))), "The percentage of quarterly comparison or total giving of last quarter is not correct!");
                Assert.IsTrue(thisYearDetail.Contains(test.Dashboard.removeZeroInTheEndOfDecimalpart(string.Format("{0:n}", thisYearQuarterSubTotal))), "The total giving of this quarter is not correct!");
                Assert.IsTrue(lastYearDetail.Contains(test.Dashboard.removeZeroInTheEndOfDecimalpart(string.Format("{0:n}", lastYearQuarterSubTotal))), "The total giving of the same quarter of last year is not correct!");
                Assert.IsTrue(last2YearDetail.Contains(test.Dashboard.removeZeroInTheEndOfDecimalpart(string.Format("{0:n}", last2YearQuarterSubTotal))));
            }
            finally
            {
                //clear test data
            }
        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify monthly comparison data is correct")]
        public void Dashboard_Chart_Giving_Comparison_Monthly()
        {
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
            int userId = test.SQL.User_FetchID(churchId, test.Dashboard.DashboardEmail, test.Dashboard.DashboardUsername);
            GeneralMethods utility = test.GeneralMethods;

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                home.openSettingsPage();
                int widgetTotal = settings.getWidgetsTotalOnPage();
                int turnedOnWidgetTotal = settings.getTurnedOnWidgetsTotalOnPage(widgetTotal);

                if (!settings.isWidgetTurnedOn(1))
                {
                    settings.turnOnWidget(1);
                }

                settings.closeSettingsPage();

                home.selectView("month");
                string comparisonText = home.getComparisonText(1);
                string icon1 = home.getComparisonIcon1(1);
                string icon2 = home.getComparisonIcon1(1);

                DateTime monthStartDay = test.Dashboard.getStartDateOfMonth(now);
                DateTime monthEndDay = monthStartDay.AddMonths(1).AddDays(-1);
                double thisYearMonthSubTotal = test.SQL.Dashboard_Giving_GetPeriodSum_Chart(churchId, userId, monthStartDay, monthEndDay);
                double thisYearLastMonthSubTotal = test.SQL.Dashboard_Giving_GetPeriodSum_Chart(churchId, userId, monthStartDay.AddMonths(-1), monthStartDay.AddDays(-1));
                double lastYearMonthSubTotal = test.SQL.Dashboard_Giving_GetPeriodSum_Chart(churchId, userId, monthStartDay.AddYears(-1), monthEndDay.AddYears(-1));
                double last2YearMonthSubTotal = test.SQL.Dashboard_Giving_GetPeriodSum_Chart(churchId, userId, monthStartDay.AddYears(-2), monthEndDay.AddYears(-2));

                string lastMonthComparisonPercentage = string.Format("{0:n}", (thisYearMonthSubTotal / thisYearLastMonthSubTotal - 1) * 100);
                string monthOfLastYearComparisonPercentage = string.Format("{0:n}", (thisYearMonthSubTotal / lastYearMonthSubTotal - 1) * 100);
                lastMonthComparisonPercentage = test.Dashboard.removeZeroInTheEndOfDecimalpart(lastMonthComparisonPercentage);
                monthOfLastYearComparisonPercentage = test.Dashboard.removeZeroInTheEndOfDecimalpart(monthOfLastYearComparisonPercentage);

                Assert.IsTrue(comparisonText.Contains(string.Format("{0}% From last month", lastMonthComparisonPercentage)), "The percentage of monthly comparison is not correct!");
                Assert.IsTrue(comparisonText.Contains(string.Format("{0}% From last year", monthOfLastYearComparisonPercentage)), "The percentage of monthly comparison is not correct!");

                if (thisYearMonthSubTotal > thisYearLastMonthSubTotal)
                {
                    Assert.IsTrue(icon1.Contains("positive-arrow"), "The icon of comparison is not correct!");
                }
                else if (thisYearMonthSubTotal < thisYearLastMonthSubTotal)
                {
                    Assert.IsTrue(icon1.Contains("negative-arrow"), "The icon of comparison is not correct!");
                }
                else
                {
                    Assert.IsTrue(icon1.Contains("flat"), "The icon of comparison is not correct!");
                }

                if (thisYearMonthSubTotal > lastYearMonthSubTotal)
                {
                    Assert.IsTrue(icon2.Contains("positive-arrow"), "The icon of comparison is not correct!");
                }
                else if (thisYearMonthSubTotal < lastYearMonthSubTotal)
                {
                    Assert.IsTrue(icon2.Contains("negative-arrow"), "The icon of comparison is not correct!");
                }
                else
                {
                    Assert.IsTrue(icon2.Contains("flat"), "The icon of comparison is not correct!");
                }

                home.unfoldGivingChart();

                string thisYearDetail = home.getThisYearDetail(1, "Giving");
                string lastYearDetail = home.getLastYearDetail(1, "Giving");
                string last2YearDetail = home.getLast2YearDetail(1, "Giving");
                Assert.IsTrue(thisYearDetail.Contains(string.Format("{0}% From month prior ${1}", lastMonthComparisonPercentage, test.Dashboard.removeZeroInTheEndOfDecimalpart(string.Format("{0:n}", thisYearLastMonthSubTotal)))), "The percentage of monthly comparison or total giving of last month is not correct!");
                Assert.IsTrue(thisYearDetail.Contains(test.Dashboard.removeZeroInTheEndOfDecimalpart(string.Format("{0:n}", thisYearMonthSubTotal))), "The total giving of this month is not correct!");
                Assert.IsTrue(lastYearDetail.Contains(test.Dashboard.removeZeroInTheEndOfDecimalpart(string.Format("{0:n}", lastYearMonthSubTotal))), "The total giving of same month of last year is not correct!");
                Assert.IsTrue(last2YearDetail.Contains(test.Dashboard.removeZeroInTheEndOfDecimalpart(string.Format("{0:n}", last2YearMonthSubTotal))));
            }
            finally
            {
                //clear test data
            }
        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify weekly comparison data is correct")]
        public void Dashboard_Chart_Giving_Comparison_Weekly()
        {
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
            int userId = test.SQL.User_FetchID(churchId, test.Dashboard.DashboardEmail, test.Dashboard.DashboardUsername);
            GeneralMethods utility = test.GeneralMethods;

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                home.openSettingsPage();
                int widgetTotal = settings.getWidgetsTotalOnPage();
                int turnedOnWidgetTotal = settings.getTurnedOnWidgetsTotalOnPage(widgetTotal);

                if (!settings.isWidgetTurnedOn(1))
                {
                    settings.turnOnWidget(1);
                }

                settings.closeSettingsPage();

                home.selectView("week");
                string comparisonText = home.getComparisonText(1);
                string icon1 = home.getComparisonIcon1(1);
                string icon2 = home.getComparisonIcon1(1);
                int startDay = home.getCurrentStartDay();

                DateTime weekStartDay = test.Dashboard.getStartDateOfWeek(now, startDay);
                DateTime weekEndDay = now;
                int theWeekNumber = test.Dashboard.getWeekNumberOfYear(now, now.Year, startDay);

                ArrayList weekStartDayOfLastYear = test.Dashboard.getStartDaysOfIsoWeeks(now.AddYears(-1), startDay);
                ArrayList weekStartDayOfLast2Year = test.Dashboard.getStartDaysOfIsoWeeks(now.AddYears(-2), startDay);

                DateTime lastYearRelatedWeekStartDay = test.Dashboard.getStartDayOfWeekByWeekNumber(theWeekNumber, now.AddYears(-1), startDay);
                DateTime last2YearRelatedWeekStartDay = test.Dashboard.getStartDayOfWeekByWeekNumber(theWeekNumber, now.AddYears(-2), startDay);

                double thisYearWeekSubTotal = test.SQL.Dashboard_Giving_GetPeriodSum_Chart(churchId, userId, weekStartDay, weekEndDay);
                double thisYearLastWeekSubTotal = test.SQL.Dashboard_Giving_GetPeriodSum_Chart(churchId, userId, weekStartDay.AddDays(-7), weekStartDay.AddDays(-1));
                double lastYearWeekSubTotal = test.SQL.Dashboard_Giving_GetPeriodSum_Chart(churchId, userId, lastYearRelatedWeekStartDay, lastYearRelatedWeekStartDay.AddDays(6));
                double last2YearWeekSubTotal = test.SQL.Dashboard_Giving_GetPeriodSum_Chart(churchId, userId, last2YearRelatedWeekStartDay, last2YearRelatedWeekStartDay.AddDays(6));

                string lastWeekComparisonPercentage = string.Format("{0:n}", (thisYearWeekSubTotal / thisYearLastWeekSubTotal - 1) * 100);
                string weekOfLastYearComparisonPercentage = string.Format("{0:n}", (thisYearWeekSubTotal / lastYearWeekSubTotal - 1) * 100);
                lastWeekComparisonPercentage = test.Dashboard.removeZeroInTheEndOfDecimalpart(lastWeekComparisonPercentage);
                weekOfLastYearComparisonPercentage = test.Dashboard.removeZeroInTheEndOfDecimalpart(weekOfLastYearComparisonPercentage);

                Assert.IsTrue(comparisonText.Contains(string.Format("{0}% From last week", lastWeekComparisonPercentage)), "The percentage of weekly comparison is not correct!");
                Assert.IsTrue(comparisonText.Contains(string.Format("{0}% From last year", weekOfLastYearComparisonPercentage)), "The percentage of weekly comparison is not correct!");

                if (thisYearWeekSubTotal > thisYearLastWeekSubTotal)
                {
                    Assert.IsTrue(icon1.Contains("positive-arrow"), "The icon of comparison is not correct!");
                }
                else if (thisYearWeekSubTotal < thisYearLastWeekSubTotal)
                {
                    Assert.IsTrue(icon1.Contains("negative-arrow"), "The icon of comparison is not correct!");
                }
                else
                {
                    Assert.IsTrue(icon1.Contains("flat"), "The icon of comparison is not correct!");
                }

                if (thisYearWeekSubTotal > lastYearWeekSubTotal)
                {
                    Assert.IsTrue(icon2.Contains("positive-arrow"), "The icon of comparison is not correct!");
                }
                else if (thisYearWeekSubTotal < lastYearWeekSubTotal)
                {
                    Assert.IsTrue(icon2.Contains("negative-arrow"), "The icon of comparison is not correct!");
                }
                else
                {
                    Assert.IsTrue(icon2.Contains("flat"), "The icon of comparison is not correct!");
                }

                home.unfoldGivingChart();

                string thisYearDetail = home.getThisYearDetail(1, "Giving");
                string lastYearDetail = home.getLastYearDetail(1, "Giving");
                string last2YearDetail = home.getLast2YearDetail(1, "Giving");
                Assert.IsTrue(thisYearDetail.Contains(string.Format("{0}% From week prior ${1}", lastWeekComparisonPercentage, test.Dashboard.removeZeroInTheEndOfDecimalpart(string.Format("{0:n}", thisYearLastWeekSubTotal)))), "The percentage of weekly comparison or total giving of last week is not correct!");
                Assert.IsTrue(thisYearDetail.Contains(test.Dashboard.removeZeroInTheEndOfDecimalpart(string.Format("{0:n}", thisYearWeekSubTotal))), "The total giving of this week is not correct!");
                Assert.IsTrue(lastYearDetail.Contains(test.Dashboard.removeZeroInTheEndOfDecimalpart(string.Format("{0:n}", lastYearWeekSubTotal))), "The total giving of same week of last year is not correct!");
                Assert.IsTrue(last2YearDetail.Contains(test.Dashboard.removeZeroInTheEndOfDecimalpart(string.Format("{0:n}", last2YearWeekSubTotal))));
            }
            finally
            {
                //clear test data
            }
        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify yearly attribute group chart data is correct")]
        public void Dashboard_Chart_AttributEGroup_Yearly()
        {
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
            int userId = test.SQL.User_FetchID(churchId, test.Dashboard.DashboardEmail, test.Dashboard.DashboardUsername);
            GeneralMethods utility = test.GeneralMethods;

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                home.openSettingsPage();
                int widgetTotal = settings.getWidgetsTotalOnPage();
                int turnedOnWidgetTotal = settings.getTurnedOnWidgetsTotalOnPage(widgetTotal);

                if (turnedOnWidgetTotal<3)
                {
                    settings.turnOnWidget(3);
                }

                string[] attributeGroup = settings.getAttributeGroupNameAndId(3);

                int turnedOnSubItems = settings.getWidgetSubItemsOnPage(3).Count;
                for (int i = 0; i < turnedOnSubItems; i++)
                {
                    settings.checkWidgetItem(3, i + 1);
                }

                settings.closeSettingsPage();

                home.ClickWidgetByName(turnedOnWidgetTotal, attributeGroup[0]);
                home.selectView("year");

                ArrayList numbers = home.getPrasedChartNumber("Attribute Group", attributeGroup[0]);
                IEnumerator enumerator = numbers.GetEnumerator();
                ArrayList dates = test.Dashboard.getStartDatesOfChart(now, "year");

                foreach (DateTime start in dates)
                {
                    DateTime end = start.AddYears(1).AddDays(-1);
                    int subTotal = test.SQL.Dashboard_AttributeGroup_GetPeriodSum_Chart(churchId, userId, int.Parse(attributeGroup[1]), start, end);
                    if (enumerator.MoveNext())
                    {
                        TestLog.WriteLine(string.Format("{0}|{1}", subTotal, enumerator.Current.ToString()));
                        Assert.AreEqual(subTotal, double.Parse(enumerator.Current.ToString()), "The attribute group total of at least one year is not correct");
                    }

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
        [Description("Verify quarterly giving chart data is correct")]
        public void Dashboard_Chart_Giving_Quarterly()
        {
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
            int userId = test.SQL.User_FetchID(churchId, test.Dashboard.DashboardEmail, test.Dashboard.DashboardUsername);
            GeneralMethods utility = test.GeneralMethods;

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                home.openSettingsPage();
                int widgetTotal = settings.getWidgetsTotalOnPage();
                int turnedOnWidgetTotal = settings.getTurnedOnWidgetsTotalOnPage(widgetTotal);

                if (!settings.isWidgetTurnedOn(1))
                {
                    settings.turnOnWidget(1);
                }

                settings.closeSettingsPage();

                home.unfoldGivingChart();
                home.selectView("quarter");

                ArrayList numbers = home.getPrasedGivingChartNumber();
                IEnumerator enumerator = numbers.GetEnumerator();
                ArrayList dates = test.Dashboard.getStartDatesOfChart(now, "quarter");

                foreach (DateTime start in dates)
                {
                    DateTime end = start.AddMonths(3).AddDays(-1);
                    double subTotal = test.SQL.Dashboard_Giving_GetPeriodSum_Chart(churchId, userId, start, end);
                    if (enumerator.MoveNext())
                    {
                        TestLog.WriteLine(string.Format("{0}|{1}", subTotal, enumerator.Current.ToString()));
                        Assert.AreEqual(subTotal, double.Parse(enumerator.Current.ToString()), "The giving total of at least one quarter is not correct");
                    }

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
        [Description("Verify quarterly attribute group chart data is correct")]
        public void Dashboard_Chart_AttributEGroup_Quarterly()
        {
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
            int userId = test.SQL.User_FetchID(churchId, test.Dashboard.DashboardEmail, test.Dashboard.DashboardUsername);
            GeneralMethods utility = test.GeneralMethods;

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                home.openSettingsPage();
                int widgetTotal = settings.getWidgetsTotalOnPage();
                int turnedOnWidgetTotal = settings.getTurnedOnWidgetsTotalOnPage(widgetTotal);

                if (turnedOnWidgetTotal < 3)
                {
                    settings.turnOnWidget(3);
                }

                string[] attributeGroup = settings.getAttributeGroupNameAndId(3);
                settings.closeSettingsPage();

                home.ClickWidgetByName(turnedOnWidgetTotal, attributeGroup[0]);
                home.selectView("quarter");

                ArrayList numbers = home.getPrasedChartNumber("Attribute Group", attributeGroup[0]);
                IEnumerator enumerator = numbers.GetEnumerator();
                ArrayList dates = test.Dashboard.getStartDatesOfChart(now, "quarter");

                foreach (DateTime start in dates)
                {
                    DateTime end = start.AddMonths(3).AddDays(-1);
                    int subTotal = test.SQL.Dashboard_AttributeGroup_GetPeriodSum_Chart(churchId, userId, int.Parse(attributeGroup[1]), start, end);
                    if (enumerator.MoveNext())
                    {
                        TestLog.WriteLine(string.Format("{0}|{1}", subTotal, enumerator.Current.ToString()));
                        Assert.AreEqual(subTotal, double.Parse(enumerator.Current.ToString()), "The attribute group total of at least one quarter is not correct");
                    }

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
        [Description("Verify monthly giving chart data is correct")]
        public void Dashboard_Chart_Giving_Monthly()
        {
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
            int userId = test.SQL.User_FetchID(churchId, test.Dashboard.DashboardEmail, test.Dashboard.DashboardUsername);
            GeneralMethods utility = test.GeneralMethods;

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                home.openSettingsPage();
                int widgetTotal = settings.getWidgetsTotalOnPage();
                int turnedOnWidgetTotal = settings.getTurnedOnWidgetsTotalOnPage(widgetTotal);

                if (!settings.isWidgetTurnedOn(1))
                {
                    settings.turnOnWidget(1);
                }

                settings.closeSettingsPage();

                home.unfoldGivingChart();
                home.selectView("month");

                ArrayList numbers = home.getPrasedGivingChartNumber();
                IEnumerator enumerator = numbers.GetEnumerator();
                ArrayList dates = test.Dashboard.getStartDatesOfChart(now, "month");

                foreach (DateTime start in dates)
                {
                    DateTime end = start.AddMonths(1).AddDays(-1);
                    double subTotal = test.SQL.Dashboard_Giving_GetPeriodSum_Chart(churchId, userId, start, end);
                    if (enumerator.MoveNext())
                    {
                        TestLog.WriteLine(string.Format("{0}|{1}", subTotal, enumerator.Current.ToString()));
                        Assert.AreEqual(subTotal, double.Parse(enumerator.Current.ToString()), "The giving total of at least one month is not correct");
                    }

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
        [Description("Verify monthly attribute group chart data is correct")]
        public void Dashboard_Chart_AttributEGroup_Monthly()
        {
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
            int userId = test.SQL.User_FetchID(churchId, test.Dashboard.DashboardEmail, test.Dashboard.DashboardUsername);
            GeneralMethods utility = test.GeneralMethods;

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                home.openSettingsPage();
                int widgetTotal = settings.getWidgetsTotalOnPage();
                int turnedOnWidgetTotal = settings.getTurnedOnWidgetsTotalOnPage(widgetTotal);

                if (turnedOnWidgetTotal < 3)
                {
                    settings.turnOnWidget(3);
                }

                string[] attributeGroup = settings.getAttributeGroupNameAndId(3);
                settings.closeSettingsPage();

                home.ClickWidgetByName(turnedOnWidgetTotal, attributeGroup[0]);
                home.selectView("month");

                ArrayList numbers = home.getPrasedChartNumber("Attribute Group", attributeGroup[0]);
                IEnumerator enumerator = numbers.GetEnumerator();
                ArrayList dates = test.Dashboard.getStartDatesOfChart(now, "month");

                foreach (DateTime start in dates)
                {
                    DateTime end = start.AddMonths(1).AddDays(-1);
                    int subTotal = test.SQL.Dashboard_AttributeGroup_GetPeriodSum_Chart(churchId, userId, int.Parse(attributeGroup[1]), start, end);
                    if (enumerator.MoveNext())
                    {
                        TestLog.WriteLine(string.Format("{0}|{1}", subTotal, enumerator.Current.ToString()));
                        Assert.AreEqual(subTotal, double.Parse(enumerator.Current.ToString()), "The attribute group total of at least one month is not correct");
                    }

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
        [Description("Verify message 'No Data or Insufficient Data' will be shown if no data")]
        public void Dashboard_Chart_AttributEGroup_NoData()
        {
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
            int userId = test.SQL.User_FetchID(churchId, test.Dashboard.DashboardEmail, test.Dashboard.DashboardUsername);
            GeneralMethods utility = test.GeneralMethods;
            string[] viewTypes = new string[] { "week", "month", "quarter", "year" };
            string expectedMessage = "No Data or Insufficient Data";

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                home.openSettingsPage();
                int widgetTotal = settings.getWidgetsTotalOnPage();
                int turnedOnWidgetTotal = settings.getTurnedOnWidgetsTotalOnPage(widgetTotal);

                if (turnedOnWidgetTotal < 3)
                {
                    settings.turnOnWidget(3);
                }

                int turnedOnSubItems = settings.getWidgetSubItemsOnPage(3).Count;
                for (int i = 0; i < turnedOnSubItems; i++)
                {
                    settings.uncheckWidgetItem(3, i + 1);
                }

                string[] attributeGroup = settings.getAttributeGroupNameAndId(3);

                settings.closeSettingsPage();

                home.ClickWidgetByName(turnedOnWidgetTotal, attributeGroup[0]);

                for (int i = 0; i < viewTypes.Length; i++)
                {
                    home.selectView(viewTypes[i]);
                    string chartText = home.getTextOfChart("Attribute Group", attributeGroup[0]);
                    Assert.IsTrue(chartText.Contains(expectedMessage), "Message 'No Data or Insufficient Data' is not shown");
                }

                //recover settings
                home.openSettingsPage();
                for (int i = 0; i < turnedOnSubItems; i++)
                {
                    settings.checkWidgetItem(3, i + 1);
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
        [Description("Verify string of X axis for chart is correct against different view type")]
        public void Dashboard_Chart_XAxis_DifferentViewType()
        {
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
            int userId = test.SQL.User_FetchID(churchId, test.Dashboard.DashboardEmail, test.Dashboard.DashboardUsername);
            GeneralMethods utility = test.GeneralMethods;
            string[] viewTypes = new string[] { "week", "month", "quarter","year" };
            string[] expectedResults = new string[] 
            {
                "",
                "[\"Jan\",\"Feb\",\"Mar\",\"Apr\",\"May\",\"Jun\",\"Jul\",\"Aug\",\"Sep\",\"Oct\",\"Nov\",\"Dec\"]",
                "[\"Q1\",\"Q2\",\"Q3\",\"Q4\"]"   
            };

            StringBuilder expectedXAxis = new StringBuilder();
            expectedXAxis.Append(string.Format("[\"{0}\"", now.AddYears(-24).Year.ToString()));
            for(int i=23; i>=0;i--)
            {
                expectedXAxis.Append(string.Format(",\"{0}\"", now.AddYears(0-i).Year.ToString()));
            }
            expectedXAxis.Append("]");

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                home.openSettingsPage();
                int widgetTotal = settings.getWidgetsTotalOnPage();
                int turnedOnWidgetTotal = settings.getTurnedOnWidgetsTotalOnPage(widgetTotal);

                if (turnedOnWidgetTotal < 3)
                {
                    settings.turnOnWidget(3);
                }

                string[] attributeGroup = settings.getAttributeGroupNameAndId(3);

                settings.closeSettingsPage();

                home.ClickWidgetByName(turnedOnWidgetTotal, attributeGroup[0]);

                for (int i = 0; i < viewTypes.Length; i++)
                {
                    home.selectView(viewTypes[i]);
                    string xAxis = home.getXAxisOfChart("Attribute Group", attributeGroup[0]);
                    if (i < 3)
                    {
                        TestLog.WriteLine(string.Format("{0}|{1}", xAxis, expectedResults[i]));
                        Assert.AreEqual(xAxis, expectedResults[i], "x-axis is not changed according to the currently selected view type");
                    }
                    else 
                    {
                        TestLog.WriteLine(string.Format("{0}|{1}", xAxis, expectedXAxis.ToString()));
                        Assert.AreEqual(xAxis, expectedXAxis.ToString(), "x-axis is not changed according to the currently selected view type");
                    }
                }

            }
            finally
            {
                //clear test data
            }
        }
    }
}

