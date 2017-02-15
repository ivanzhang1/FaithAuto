using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace FTTests.Dashboard
{
    public class DashboardHomePage
    {
        private RemoteWebDriver _driver;
        private GeneralMethods _generalMethods;
        private IJavaScriptExecutor _javaScriptExecutor;
        private string[] _viewType;
        private string[] _startDay;

        #region locator
        private string img_id_settings = "settings";

        private string input_id_viewTypeParameterized = "view-type-{0}";
        private string input_id_startDayParameterized = "first-day-{0}";
        private string div_id_giving_banner = "metric-result-detail-Giving";

        private string label_xpath_startDaySunday = "//label[@for='first-day-sunday']";
        private string label_xpath_startDayMonday = "//label[@for='first-day-monday']";
        private string label_xpath_startDayTuesday = "//label[@for='first-day-tuesday']";
        private string label_xpath_startDayWednesday = "//label[@for='first-day-wednesday']";
        private string label_xpath_startDayThursday = "//label[@for='first-day-thursday']";
        private string label_xpath_startDayFriday = "//label[@for='first-day-friday']";
        private string label_xpath_startDaySaturday = "//label[@for='first-day-saturday']";
        private string label_xpath_startDayParameterized = "//label[@for='first-day-{0}']";

        private string label_xpath_viewTypeWeek = "//label[@for='view-type-week']";
        private string label_xpath_viewTypeMonth = "//label[@for='view-type-month']";
        private string label_xpath_viewTypeQuarter = "//label[@for='view-type-quarter']";
        private string label_xpath_viewTypeYear = "//label[@for='view-type-year']";
        private string label_xpath_viewTypeParameterized = "//label[@for='view-type-{0}']";

        private string span_xpath_dateRange = "//span[@class='DateRange-date']";
        private string span_xpath_churchName = "//span[@class='DateRange-church ng-binding']";

        private string i_xpath_headerExpanded = "//i[@ng-click='headerExpanded = false;']";
        private string i_xpath_headerUnExpanded = "//i[@ng-click='headerExpanded = true;']";

        private string div_xpath_selectView = "//div[@ng-show='headerExpanded']";
        //private string div_xpath_selectView_hiden = "//div[@class='row Range ng-hide']";
        private string div_xpath_startDay = "//div[@ng-show=\"settings.window === 'week'\"]";
        private string div_xpath_parameterized_chart = "//div[@class='Metric-items __{0}_dark dark--is{1}']";
        private string div_xpath_parameterized_widgetName = "//div[@ng-controller='DashboardController']/div[@ng-repeat='metric in metrics'][{0}]/div/div[1]/div/div";
        public string div_xpath_parameterized_widgetTotal = "//div[@ng-controller='DashboardController']/div[@ng-repeat='metric in metrics'][{0}]/div[1]/div[2]";

        private string div_xpath_parameterized_comparison = "//div[@ng-controller='DashboardController']/div[@ng-repeat='metric in metrics'][{0}]/div[1]";
        private string div_xpath_parameterized_comparison1 = "//div[@ng-controller='DashboardController']/div[@ng-repeat='metric in metrics'][{0}]/div[1]/div[3]";
        private string div_xpath_parameterized_comparison2 = "//div[@ng-controller='DashboardController']/div[@ng-repeat='metric in metrics'][{0}]/div[1]/div[4]";

        private string span_xpath_parameterized_comparisonPercentage1 = "//div[@ng-controller='DashboardController']/div[@ng-repeat='metric in metrics'][{0}]/div[1]/div[3]/span";
        private string span_xpath_parameterized_comparisonPercentage2 = "//div[@ng-controller='DashboardController']/div[@ng-repeat='metric in metrics'][{0}]/div[1]/div[4]/span";
        private string img_xpath_parameterized_comparisonIcon1 = "//div[@ng-controller='DashboardController']/div[@ng-repeat='metric in metrics'][{0}]/div[1]/div[3]/img";
        private string img_xpath_parameterized_comparisonIcon2 = "//div[@ng-controller='DashboardController']/div[@ng-repeat='metric in metrics'][{0}]/div[1]/div[4]/img";

        private string div_xpath_parameterized_thisYearDetail = "//div[@class='col-xs-6 col-sm-7 metric-detail-value-now Metric-name--is{0} __{1}']";
        private string div_xpath_parameterized_lastYearDetail = "//div[@class='col-xs-3 col-sm-3 metric-detail-value metric-detail-value--last __{0}_dark dark--is{1}']";
        private string div_xpath_parameterized_last2YearDetail = "//div[@class='col-xs-3 col-sm-2 metric-detail-value metric-detail-value--last2last __{0}_darker darker--is{1}']";
        private string label_xpath_parameterized_chartItem = "//div[@class='Metric-items __{0}_dark dark--is{1}']/span[{2}]/label";
        private string input_xpath_parameterized_chartItem = "//div[@class='Metric-items __{0}_dark dark--is{1}']/span[{2}]/input";

        private string metricChart_xpath_givingChart = "//div[@type='Giving' and @text='Giving']";
        private string metricChart_xpath_parameterized_Chart = "//div[@type='{0}' and @text='{1}']";
        #endregion

        public DashboardHomePage(RemoteWebDriver driver, GeneralMethods generalMethods)
        {
            this._driver = driver;
            this._generalMethods = generalMethods;
            this._javaScriptExecutor = (IJavaScriptExecutor)this._driver;
            this._viewType = new string[] { "week", "month", "quarter", "year" };
            this._startDay = new string[] { "sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday" };
        }

        /// <summary>
        /// Get the comparison text
        /// </summary>
        public string getComparisonText(int index)
        {
            var divComparison = this._driver.FindElement(By.XPath(string.Format(this.div_xpath_parameterized_comparison, index)));
            return divComparison.Text;
        }

        /// <summary>
        /// Get this year detail on bar
        /// </summary>
        public string getThisYearDetail(int index, string widgetName)
        {
            var detail = this._driver.FindElement(By.XPath(string.Format(this.div_xpath_parameterized_thisYearDetail, widgetName,index-1)));
            return detail.Text;
        }

        /// <summary>
        /// Get last year detail on bar
        /// </summary>
        public string getLastYearDetail(int index, string widgetName)
        {
            var detail = this._driver.FindElement(By.XPath(string.Format(this.div_xpath_parameterized_lastYearDetail, index-1, widgetName)));
            return detail.Text;
        }

        /// <summary>
        /// Get the year before last year detail on bar
        /// </summary>
        public string getLast2YearDetail(int index, string widgetName)
        {
            var detail = this._driver.FindElement(By.XPath(string.Format(this.div_xpath_parameterized_last2YearDetail, index-1, widgetName)));
            return detail.Text;
        }

        /// <summary>
        /// Get the comparison icon 1
        /// </summary>
        public string getComparisonIcon1(int index)
        {
            string src = "";
            try
            {
                var detail = this._driver.FindElement(By.XPath(string.Format(this.img_xpath_parameterized_comparisonIcon1, index)));
                src = detail.GetAttribute("ng-src");
            }
            catch(NoSuchElementException e)
            {
                src = "flat";
            }
                return src;
        }

        /// <summary>
        /// Get the comparison icon 2
        /// </summary>
        public string getComparisonIcon2(int index)
        {
            var detail = this._driver.FindElement(By.XPath(string.Format(this.img_xpath_parameterized_comparisonIcon2, index)));
            return detail.GetAttribute("ng-src");
        }

        /// <summary>
        /// Get the widget name by index
        /// </summary>
        public string getWidgetName(int index)
        {
            this._generalMethods.WaitForPageIsLoaded();
            return this._driver.FindElement(By.XPath(string.Format(this.div_xpath_parameterized_widgetName, index))).Text.Trim();
        }

        /// <summary>
        /// Get the widget total on home page
        /// </summary>
        public int getWidgetsTotalOnPage()
        {
            this._generalMethods.WaitForPageIsLoaded();
            int counter = 0;
            for (int i = 0; i < int.MaxValue; i++)
            {
                try
                {
                    var div_widget = this._driver.FindElement(By.XPath(string.Format(this.div_xpath_parameterized_widgetName, i + 1)));
                }
                catch (NoSuchElementException e)
                {
                    break;
                    throw e;
                }

                counter++;
            }

            return counter;
        }

        /// <summary>
        /// Expand chart by index
        /// </summary>
        public void ClickWidgetName(int index)
        {
            this._generalMethods.WaitForPageIsLoaded();
            this._driver.FindElement(By.XPath(string.Format(this.div_xpath_parameterized_widgetName, index))).Click();
        }

        /// <summary>
        /// Expand chart by index
        /// </summary>
        public void ClickWidgetByName(int totalWidgets, string widgetName)
        {
            this._generalMethods.WaitForPageIsLoaded();
            for (int i = 1; i < totalWidgets; i++)
            {
                var element = this._driver.FindElement(By.XPath(string.Format(this.div_xpath_parameterized_widgetName, i)));
                if (element.Text.Trim().Equals(widgetName))
                {
                    element.Click();
                    break;
                }
            }
        }

        /// <summary>
        /// unfold giving chart
        /// </summary>
        public void unfoldGivingChart()
        {
            this._generalMethods.WaitForPageIsLoaded();
            if (!isGivingChartExpanded())
            {
                this.ClickWidgetName(1);
            }

            System.Threading.Thread.Sleep(5000);
        }

        /// <summary>
        /// fold giving chart
        /// </summary>
        public void foldGivingChart()
        {
            this._generalMethods.WaitForPageIsLoaded();
            if (isGivingChartExpanded())
            {
                this._driver.FindElement(By.Id(this.div_id_giving_banner)).Click();
            }
        }

        /// <summary>
        /// If the giving chart is expanded
        /// </summary>
        public bool isGivingChartExpanded()
        {
            return this._generalMethods.isElementAppear("$(\"div.Metric-items.__0_dark.dark--isGiving\")");
        }

        /// <summary>
        /// If the chart is expanded
        /// </summary>
        public bool isChartExpanded(int chartIndex, string widgetName)
        {
            string script = string.Format("$(\"div.Metric-items.__{0}_dark.dark--is{1}\")", chartIndex - 1, widgetName);
            return this._generalMethods.isElementAppear(script);
        }

        public bool isChartItemChecked(int chartIndex, int itemIndex, string widgetName)
        {
            if (!isChartExpanded(chartIndex, widgetName))
            {
                ClickWidgetName(chartIndex);
            }
            string xpath = string.Format(this.input_xpath_parameterized_chartItem, chartIndex - 1, widgetName, itemIndex);
            var input = this._driver.FindElement(By.XPath(xpath));
            return bool.Parse(this._javaScriptExecutor.ExecuteScript("return arguments[0].checked;", input).ToString());
        }

        public int getUserWidgetItemID(int chartIndex, int itemIndex, string widgetName)
        {
            if (!isChartExpanded(chartIndex, widgetName))
            {
                ClickWidgetName(chartIndex);
            }
            string xpath = string.Format(this.input_xpath_parameterized_chartItem, chartIndex - 1, widgetName, itemIndex);
            var input = this._driver.FindElement(By.XPath(xpath));
            string itemId = this._javaScriptExecutor.ExecuteScript("return arguments[0].getAttribute(\"id\");", input).ToString().Trim();

            return int.Parse(itemId.Split('-')[1]);
        }

        public void checkChartItem(int chartIndex, int itemIndex, string widgetName)
        {
            if (!isChartExpanded(chartIndex, widgetName))
            {
                ClickWidgetName(chartIndex);
            }

            string xpath = string.Format(this.label_xpath_parameterized_chartItem, chartIndex - 1, widgetName, itemIndex);
            var label = this._driver.FindElement(By.XPath(xpath));

            if (!isChartItemChecked(chartIndex, itemIndex, widgetName))
            {
                label.Click();
                System.Threading.Thread.Sleep(5000);
            }
        }

        public void uncheckChartItem(int chartIndex, int itemIndex, string widgetName)
        {
            if (!isChartExpanded(chartIndex, widgetName))
            {
                ClickWidgetName(chartIndex);
            }

            string xpath = string.Format(this.label_xpath_parameterized_chartItem, chartIndex - 1, widgetName, itemIndex);
            var label = this._driver.FindElement(By.XPath(xpath));

            if (isChartItemChecked(chartIndex, itemIndex, widgetName))
            {
                label.Click();
                System.Threading.Thread.Sleep(5000);
            }
        }


        /// <summary>
        /// Get the widget total by index
        /// </summary>
        public string getWidgetBigNumber(int index)
        {
            this._generalMethods.WaitForPageIsLoaded();
            return this._driver.FindElement(By.XPath(string.Format(this.div_xpath_parameterized_widgetTotal, index))).Text.Trim();
        }

        /// <summary>
        /// Fold select view
        /// </summary>
        public void foldSelectView()
        {
            if (!isSelectViewFolded())
            {
                this._generalMethods.WaitAndGetElement(By.XPath(this.i_xpath_headerExpanded)).Click();
                this._generalMethods.WaitForPageIsLoaded();
            }
        }

        /// <summary>
        /// Expand select view
        /// </summary>
        public void unfoldSelectView()
        {
            if (isSelectViewFolded())
            {
                this._generalMethods.WaitAndGetElement(By.XPath(this.i_xpath_headerUnExpanded)).Click();
                this._generalMethods.WaitForPageIsLoaded();
            }
        }

        /// <summary>
        /// If the select view is expanded?
        /// </summary>
        public bool isSelectViewFolded()
        {
            this._generalMethods.WaitForPageIsLoaded();
            var div_selectView = this._driver.FindElement(By.XPath(this.div_xpath_selectView));
            return div_selectView.GetAttribute("class").Contains("ng-hide");
        }

        /// <summary>
        /// tO Open settings page
        /// </summary>
        public void openSettingsPage()
        {
            this._generalMethods.WaitForPageIsLoaded();
            this._generalMethods.waitForElementAppear("$(\"div.Metric-name-wrapper\")");
            this._generalMethods.WaitAndGetElement(By.Id(this.img_id_settings)).Click();
            this._generalMethods.WaitForPageIsLoaded();
        }

        /// <summary>
        /// Select specific view type
        /// </summary>
        public void selectView(string viewType)
        {
            if (isSelectViewFolded())
            {
                this.unfoldSelectView();
            }

            this._generalMethods.WaitAndGetElement(By.XPath(string.Format(this.label_xpath_viewTypeParameterized, viewType.ToLower()))).Click();
            this._generalMethods.WaitForPageIsLoaded();
            System.Threading.Thread.Sleep(5000);
        }

        /// <summary>
        /// If week view is selected?
        /// </summary>
        public bool isWeekViewTypeSelected()
        {
            this._generalMethods.WaitForPageIsLoaded();
            var div_startDay = this._driver.FindElement(By.XPath(this.div_xpath_startDay));
            return !div_startDay.GetAttribute("class").Contains("ng-hide");
        }

        /// <summary>
        /// Select specific start day of week
        /// </summary>
        public void selectStartDay(string startDay)
        {
            if (isWeekViewTypeSelected())
            {
                this._generalMethods.WaitAndGetElement(By.XPath(string.Format(this.label_xpath_startDayParameterized, startDay.ToLower()))).Click();
                this._generalMethods.WaitForPageIsLoaded();
            }

            System.Threading.Thread.Sleep(5000);
        }

        /// <summary>
        /// Get date range string
        /// </summary>
        public string getCurrentSDateRangeString()
        {
            var span_dateRange = this._generalMethods.WaitAndGetElement(By.XPath(this.span_xpath_dateRange));
            return span_dateRange.Text;
        }

        /// <summary>
        /// If sepecific view is selected?
        /// </summary>
        public bool isViewTypeSelected(string viewType)
        {
            if (isSelectViewFolded())
            {
                this.unfoldSelectView();
            }
            this._generalMethods.WaitForPageIsLoaded();
            string id = string.Format(this.input_id_viewTypeParameterized, viewType.Trim());
            var input_viewType = this._driver.FindElement(By.Id(id));
            return bool.Parse(this._javaScriptExecutor.ExecuteScript("return arguments[0].checked;", input_viewType).ToString());
        }

        /// <summary>
        /// Which view type is currently selected?
        /// </summary>
        public string whichViewTypeIsSelected()
        {
            if (isSelectViewFolded())
            {
                this.unfoldSelectView();
            }

            string selectedViewType = "";

            this._generalMethods.WaitForPageIsLoaded();

            for (int i = 0; i < this._viewType.Length; i++)
            {
                string id = string.Format(this.input_id_viewTypeParameterized, this._viewType[i].Trim());
                var input_viewType = this._driver.FindElement(By.Id(id));
                if (isViewTypeSelected(this._viewType[i].Trim()))
                {
                    selectedViewType = this._viewType[i];
                    break;
                }
            }
            return selectedViewType;
        }

        /// <summary>
        /// If sepecific start day of week is currently selected?
        /// </summary>
        public bool isDayOfWeekSelected(string startDay)
        {
            if (isSelectViewFolded())
            {
                this.unfoldSelectView();
            }

            if (isViewTypeSelected("week"))
            {
                this.selectView("week");
            }

            this._generalMethods.WaitForPageIsLoaded();
            string id = string.Format(this.input_id_startDayParameterized, startDay);
            var input_startDay = this._driver.FindElement(By.Id(id));
            return bool.Parse(this._javaScriptExecutor.ExecuteScript("return arguments[0].checked;", input_startDay).ToString());

        }

        /// <summary>
        /// Which start day of week is currently selected?
        /// </summary>
        public string whichStartDayIsSelected()
        {
            if (isSelectViewFolded())
            {
                this.unfoldSelectView();
            }

            if (isViewTypeSelected("week"))
            {
                this.selectView("week");
            }

            string selectedStartDay = "";

            this._generalMethods.WaitForPageIsLoaded();

            for (int i = 0; i < this._startDay.Length; i++)
            {
                string id = string.Format(this.input_id_startDayParameterized, this._startDay[i].Trim());
                var input_startDay = this._driver.FindElement(By.Id(id));
                if (isDayOfWeekSelected(this._startDay[i].Trim()))
                {
                    selectedStartDay = this._startDay[i];
                    break;
                }
            }
            return selectedStartDay;
        }

        /// <summary>
        /// Which start day of week is currently selected?
        /// </summary>
        public int getCurrentStartDay()
        {
            if (isSelectViewFolded())
            {
                this.unfoldSelectView();
            }

            if (isViewTypeSelected("week"))
            {
                this.selectView("week");
            }

            int selectedStartDay = -1;

            this._generalMethods.WaitForPageIsLoaded();

            for (int i = 0; i < this._startDay.Length; i++)
            {
                string id = string.Format(this.input_id_startDayParameterized, this._startDay[i].Trim());
                var input_startDay = this._driver.FindElement(By.Id(id));
                if (isDayOfWeekSelected(this._startDay[i].Trim()))
                {
                    selectedStartDay = i;
                    break;
                }
            }
            return selectedStartDay;
        }

        /// <summary>
        /// Get church name on home page
        /// </summary>
        public string getChurchName()
        {
            return this._generalMethods.WaitAndGetElement(By.XPath(this.span_xpath_churchName)).Text.Trim();
        }

        public ArrayList getPrasedGivingChartNumber()
        {
            int index = 0;
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            ArrayList prasedNumbers = new ArrayList();
            var element = this._driver.FindElement(By.XPath(this.metricChart_xpath_givingChart));
            string[] rawNumbers = element.GetAttribute("columns").Split(',');

            while (index<rawNumbers.Length)
            {
                double num;
                string temp = rawNumbers[index++].Replace("[", "").Replace("]", "").Trim();
                //temp = temp.Replace(string.Format("\"{0}\"", now.Year - 1), "").Replace(string.Format("\"{0}\"", now.Year), "");

                if (double.TryParse(temp, out num))
                {
                    prasedNumbers.Add(num);
                }
            }

            return prasedNumbers;
        }

        public ArrayList getPrasedChartNumber(string type, string text)
        {
            int index = 0;
            ArrayList prasedNumbers = new ArrayList();
            var element = this._driver.FindElement(By.XPath(string.Format(this.metricChart_xpath_parameterized_Chart, type, text)));
            string[] rawNumbers = element.GetAttribute("columns").Split(',');

            while (index < rawNumbers.Length)
            {
                double num;
                string temp = rawNumbers[index++].Replace("[", "").Replace("]", "").Trim();

                if (double.TryParse(temp, out num))
                {
                    prasedNumbers.Add(num);
                }
            }

            return prasedNumbers;
        }

        public string getXAxisOfChart(string type, string text)
        {
            var element = this._driver.FindElement(By.XPath(string.Format(this.metricChart_xpath_parameterized_Chart, type, text)));
            return element.GetAttribute("x");
        }

        public string getTextOfChart(string type, string text)
        {
            var element = this._driver.FindElement(By.XPath(string.Format(this.metricChart_xpath_parameterized_Chart, type, text)));
            return element.Text.Trim();
        }
    }
}
