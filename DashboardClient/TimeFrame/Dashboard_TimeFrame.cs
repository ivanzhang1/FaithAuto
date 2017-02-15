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

namespace FTTests.Dashboard
{
    [TestFixture]
    public class Dashboard_TimeFrame : FixtureBaseWebDriver
    {

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verifies default time frame data")]
        public void Dashboard_TimeFrame_GetDefaultTimeFrame()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

            try
            {
                DashboardAPIBase api = new DashboardAPIBase();
                HttpWebResponse response = api.getAccessTokenObject(api.ConsumerKey, api.ConsumerSecret, api.Username, api.Password, api.ChurchCode);
                AccessToken token = api.JsonToObject<AccessToken>(response);
                TimeFrame obj = api.DoGetRequest<TimeFrame>(string.Format("/api/dashboard/settings/date/{0}", token.user.userId));

                TestLog.WriteLine(string.Format("{0}|{1}|{2}|{3}|{4}", obj.timeframeId, obj.time, obj.userId, obj.window, obj.firstDay));
                TestLog.WriteLine(now.ToLongDateString() + "|" + now.ToLongTimeString());

                test.Dashboard.LoginWebDriver();
                
                DashboardHomePage dashboard = new DashboardHomePage(test.Driver, test.GeneralMethods);

                Assert.AreEqual(dashboard.whichViewTypeIsSelected(), obj.window);
                Assert.AreEqual(dashboard.whichStartDayIsSelected(), obj.firstDay);

                DateTime[] dateRange = test.Dashboard.getDateRange(now, obj.window, test.Dashboard.getIntStartDay(obj.firstDay));
                string format = "MMMM d";
                TestLog.WriteLine(dateRange[0].ToString(format));
                TestLog.WriteLine(dashboard.getCurrentSDateRangeString());
                Assert.IsTrue(dashboard.getCurrentSDateRangeString().Contains(dateRange[0].ToString(format)));
                Assert.IsTrue(dashboard.getCurrentSDateRangeString().Contains(dateRange[1].ToString(format)));
            }
            finally
            {
                //clear test data
            }

        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verifies set time frame data")]
        public void Dashboard_TimeFrame_SetTimeFrame()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

            try
            {

                DashboardAPIBase api = new DashboardAPIBase();
                int churchId = test.SQL.FetchChurchID(api.ChurchCode);
                int userId = test.SQL.User_FetchID(churchId, api.Email, api.Username);

                string jsonBody = string.Format("{{ userId: {0}, firstDay: \"sunday\", window:\"week\"}}", userId.ToString());
                TestLog.WriteLine(jsonBody);
                TimeFrame obj = api.DoPostRequest<TimeFrame>("/api/dashboard/settings/date", jsonBody);

                TestLog.WriteLine(string.Format("{0}|{1}|{2}|{3}|{4}", obj.timeframeId, obj.time, obj.userId, obj.window, obj.firstDay));
                TestLog.WriteLine(now.ToLongDateString() + "|" + now.ToLongTimeString());

                test.Dashboard.LoginWebDriver();

                DashboardHomePage dashboard = new DashboardHomePage(test.Driver, test.GeneralMethods);

                Assert.AreEqual(dashboard.whichViewTypeIsSelected(), "week");
                Assert.AreEqual(dashboard.whichStartDayIsSelected(), "sunday");

                DateTime[] dateRange = test.Dashboard.getDateRange(now, obj.window, test.Dashboard.getIntStartDay("sunday"));
                string format = "MMMM d";
                Assert.IsTrue(dashboard.getCurrentSDateRangeString().Contains(dateRange[0].ToString(format)));
                Assert.IsTrue(dashboard.getCurrentSDateRangeString().Contains(dateRange[1].ToString(format)));
            }
            finally
            {
                //clear test data
            }

        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verifies the select view is expanded by default")]
        public void Dashboard_TimeFrame_SelectView_Is_Expanded_ByDefault()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage dashboard = new DashboardHomePage(test.Driver, test.GeneralMethods);

                Assert.IsFalse(dashboard.isSelectViewFolded());
            }
            finally
            {
                //clear test data
            }
        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verifies the select view can be folded and unfolded")]
        public void Dashboard_TimeFrame_SelectView_Fold_And_Unfold()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage dashboard = new DashboardHomePage(test.Driver, test.GeneralMethods);

                dashboard.foldSelectView();
                Assert.IsTrue(dashboard.isSelectViewFolded());

                dashboard.unfoldSelectView();
                Assert.IsFalse(dashboard.isSelectViewFolded());
            }
            finally
            {
                //clear test data
            }
        }

        [Test, MultipleAsserts]
        [Author("Jim Jin")]
        [Description("Verifies each view type can be selected")] 
        public void Dashboard_TimeFrame_SelectView_Can_Be_Selected()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string[] viewTypes = new string[] { "week", "month", "quarter", "year" };

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage dashboard = new DashboardHomePage(test.Driver, test.GeneralMethods);

                foreach (string viewType in viewTypes)
                {
                    dashboard.selectView(viewType);
                    Assert.IsTrue(dashboard.isViewTypeSelected(viewType));
                }
            }
            finally
            {
                //clear test data
            }
        }

        [Test, MultipleAsserts]
        [Author("Jim Jin")]
        [Description("Verifies each startDay can be selected")]
        public void Dashboard_TimeFrame_StartDay_Can_Be_Selected()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string[] startDays = new string[] { "sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday" };

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage dashboard = new DashboardHomePage(test.Driver, test.GeneralMethods);
                dashboard.selectView("week");

                foreach (string startDay in startDays)
                {
                    dashboard.selectStartDay(startDay);
                    Assert.IsTrue(dashboard.isDayOfWeekSelected(startDay));
                }
            }
            finally
            {
                //clear test data
            }
        }

        [Test, MultipleAsserts]
        [Author("Jim Jin")]
        [Description("Verifies 'start day of week' would not show if 'week' view is not selected")]
        public void Dashboard_TimeFrame_StartDayOfWeek_NotShow()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string[] viewTypes = new string[] { "month", "quarter", "year" };

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage dashboard = new DashboardHomePage(test.Driver, test.GeneralMethods);

                foreach (string viewType in viewTypes)
                {
                    dashboard.selectView(viewType);
                    Assert.IsFalse(dashboard.isWeekViewTypeSelected());
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
        [Description("Verifies date range will be changed according to currently selected view")]
        public void Dashboard_TimeFrame_SelectView_DateRange()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string[] viewTypes = new string[] { "month", "quarter", "year" };
            string format = "MMMM d";

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage dashboard = new DashboardHomePage(test.Driver, test.GeneralMethods);

                foreach (string viewType in viewTypes)
                {
                    dashboard.selectView(viewType);
                    DateTime[] dateRange = test.Dashboard.getDateRange(now, viewType);
                    TestLog.WriteLine(string.Format("Expected: {0}|Actual: {1}", dateRange[0].ToString(format), dashboard.getCurrentSDateRangeString()));
                    Assert.IsTrue(dashboard.getCurrentSDateRangeString().Contains(dateRange[0].ToString(format)));
                    Assert.IsTrue(dashboard.getCurrentSDateRangeString().Contains(dateRange[1].ToString(format)));
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
        [Description("Verifies date range will be changed according to currently selected start day of week")]
        public void Dashboard_TimeFrame_StartDayOfWeek_DateRange()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string[] startDays = new string[] { "sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday" };
            string format = "MMMM d";

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage dashboard = new DashboardHomePage(test.Driver, test.GeneralMethods);
                dashboard.selectView("week");

                foreach (string startDay in startDays)
                {
                    dashboard.selectStartDay(startDay);
                    DateTime[] dateRange = test.Dashboard.getDateRange(now, "week", test.Dashboard.getIntStartDay(startDay));
                    Assert.IsTrue(dashboard.getCurrentSDateRangeString().Contains(dateRange[0].ToString(format)));
                    Assert.IsTrue(dashboard.getCurrentSDateRangeString().Contains(dateRange[1].ToString(format)));
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
        [Description("Verifies church name is correct on home page")]
        public void Dashboard_TimeFrame_ChurchName()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                string churchName = test.SQL.FetchChurchName(test.Dashboard.ChurchCode);
                
                test.Dashboard.LoginWebDriver();
                DashboardHomePage dashboard = new DashboardHomePage(test.Driver, test.GeneralMethods);

                Assert.AreEqual(dashboard.getChurchName(), churchName, "Church name on home page is not correct!");
            }
            finally 
            {
                //clear test data
            }
        }

    }
}
