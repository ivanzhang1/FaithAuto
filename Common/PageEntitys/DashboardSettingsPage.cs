using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace FTTests.Dashboard
{
    public class DashboardSettingsPage
    {
        private RemoteWebDriver _driver;
        private GeneralMethods _generalMethods;
        private SQL _sql;
        private IJavaScriptExecutor _javaScriptExecutor;

        #region locator
        private string i_xpath_parameterized_widgetExpander = "//div[@class='widgets ng-scope']/fieldset/div[{0}]/div/div[1]/div[1]/i[@ng-show='!metric.isExpanded']";
        private string i_xpath__parameterized_widgetUnexpander = "//div[@class='widgets ng-scope']/fieldset/div[{0}]/div/div[1]/div[1]/i[@ng-show='metric.isExpanded']";

        private string div_xpath_parameterized_widgetName = "//div[@class='widgets ng-scope']/fieldset/div[{0}]/div/div[1]/div[2]";
        private string div_xpath_parameterized_widgetItemsArea = "//div[@class='widgets ng-scope']/fieldset/div[{0}]/div/div[2]";

        private string span_xpath_parameterized_widgetSwitcher = "//div[@class='widgets ng-scope']/fieldset/div[{0}]/div/div[1]/span";
        private string label_xpath_parameterized_widgetItemName = "//div[@class='widgets ng-scope']/fieldset/div[{0}]/div/div[2]/ul/li[{1}]/label";
        private string input_xpath_parameterized_widgetItemCheckbox = "//div[@class='widgets ng-scope']/fieldset/div[{0}]/div/div[2]/ul/li[{1}]/input";
        private string button_xpath_closeAlert = "//button[@ng-click='onAlertDismissal()']";
        private string span_xpath_close = "//span[@class='icon-close']";
        private string div_id_alert = "configurators-limit-modal";
        #endregion

        public DashboardSettingsPage(RemoteWebDriver driver, GeneralMethods generalMethods, SQL sql)
        {
            this._driver = driver;
            this._generalMethods = generalMethods;
            this._sql = sql;
            this._javaScriptExecutor = (IJavaScriptExecutor)this._driver;
        }

        #region page data and operations
        public void closeSettingsPage()
        {
            this._generalMethods.WaitForPageIsLoaded();
            this._driver.FindElement(By.XPath(this.span_xpath_close)).Click();
            this._generalMethods.WaitForPageIsLoaded();
            this._generalMethods.waitForElementAppear("$(\"div.Metric-name-text.ng-binding\")");
        }

        public void expandWidget(int i)
        {
            this._generalMethods.WaitForPageIsLoaded();
            if (!isWidgetExpanded(i))
            {
                this._generalMethods.WaitAndGetElement(By.XPath(string.Format(this.i_xpath_parameterized_widgetExpander, i))).Click();
            }
        }

        public void unExpandWidget(int i)
        {
            this._generalMethods.WaitForPageIsLoaded();
            if (isWidgetExpanded(i))
            {
                this._generalMethods.WaitAndGetElement(By.XPath(string.Format(this.i_xpath__parameterized_widgetUnexpander, i))).Click();
            }
        }

        public string getWidgetName(int i)
        {
            this._generalMethods.WaitForPageIsLoaded();
            var div_widgetName = this._generalMethods.WaitAndGetElement(By.XPath(string.Format(this.div_xpath_parameterized_widgetName, i)));
            return div_widgetName.Text.Trim();
        }

        public bool isWidgetExpanded(int i)
        {
            this._generalMethods.WaitForPageIsLoaded();
            var div_widgetItemsArea = this._driver.FindElement(By.XPath(string.Format(this.div_xpath_parameterized_widgetItemsArea, i)));
            return !div_widgetItemsArea.GetAttribute("class").Contains("ng-hide");
        }

        public bool isWidgetTurnedOn(int i)
        {
            this._generalMethods.WaitForPageIsLoaded();
            var span_widgetSwitcher = this._driver.FindElement(By.XPath(string.Format(this.span_xpath_parameterized_widgetSwitcher, i)));
            return span_widgetSwitcher.GetAttribute("class").Contains("checked");
        }

        public void turnOnWidget(int i)
        {
            this._generalMethods.WaitForPageIsLoaded();
            if (!isWidgetTurnedOn(i))
            {
                this._generalMethods.WaitAndGetElement(By.XPath(string.Format(this.span_xpath_parameterized_widgetSwitcher, i))).Click();
            }
        }

        public void turnOffWidget(int i)
        {
            this._generalMethods.WaitForPageIsLoaded();
            if (isWidgetTurnedOn(i))
            {
                this._generalMethods.WaitAndGetElement(By.XPath(string.Format(this.span_xpath_parameterized_widgetSwitcher, i))).Click();
            }
        }

        public string getWidgetItemName(int widgetIndex, int itemIndex)
        {
            this._generalMethods.WaitForPageIsLoaded();
            expandWidget(widgetIndex);

            var label_widgetItemName = this._generalMethods.WaitAndGetElement(By.XPath(string.Format(this.label_xpath_parameterized_widgetItemName, widgetIndex, itemIndex)));
            return label_widgetItemName.Text.Trim();
        }

        public bool isWidgetItemInactive(int widgetIndex, int itemIndex)
        {
            this._generalMethods.WaitForPageIsLoaded();
            expandWidget(widgetIndex);

            var label_widgetItemName = this._generalMethods.WaitAndGetElement(By.XPath(string.Format(this.label_xpath_parameterized_widgetItemName, widgetIndex, itemIndex)));
            return label_widgetItemName.GetAttribute("class").Contains("inactive");
        }

        public void checkWidgetItem(int widgetIndex, int itemIndex)
        {
            this._generalMethods.WaitForPageIsLoaded();
            turnOnWidget(widgetIndex);
            expandWidget(widgetIndex);

            var input_widgetItem = this._driver.FindElement(By.XPath(string.Format(this.input_xpath_parameterized_widgetItemCheckbox, widgetIndex, itemIndex)));
            if (!isWidgetItemChecked(widgetIndex, itemIndex))
            {
                //input_widgetItem.Click();
                this._driver.FindElement(By.XPath(string.Format(this.label_xpath_parameterized_widgetItemName, widgetIndex, itemIndex))).Click();
            }
        }

        public void uncheckWidgetItem(int widgetIndex, int itemIndex)
        {
            this._generalMethods.WaitForPageIsLoaded();
            turnOnWidget(widgetIndex);
            expandWidget(widgetIndex);

            var input_widgetItem = this._driver.FindElement(By.XPath(string.Format(this.input_xpath_parameterized_widgetItemCheckbox, widgetIndex, itemIndex)));
            if (isWidgetItemChecked(widgetIndex, itemIndex))
            {
                //input_widgetItem.Click();
                this._driver.FindElement(By.XPath(string.Format(this.label_xpath_parameterized_widgetItemName, widgetIndex, itemIndex))).Click();
            }
        }

        public bool isWidgetItemChecked(int widgetIndex, int itemIndex)
        {
            this._generalMethods.WaitForPageIsLoaded();
            //turnOnWidget(widgetIndex);
            expandWidget(widgetIndex);
            
            var input_widgetItem = this._driver.FindElement(By.XPath(string.Format(this.input_xpath_parameterized_widgetItemCheckbox, widgetIndex, itemIndex)));
            return bool.Parse(this._javaScriptExecutor.ExecuteScript("return arguments[0].checked;", input_widgetItem).ToString());
        }

        public int getWidgetsTotalOnPage( )
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

        public ArrayList getWidgetTurnedOnSubItemsOnPage(int widgetIndex)
        {
            this._generalMethods.WaitForPageIsLoaded();
            expandWidget(widgetIndex);

            ArrayList items = new ArrayList();

            for (int i = 0; i < int.MaxValue; i++)
            {
                try
                {
                    var label_widgetItemName = this._driver.FindElement(By.XPath(string.Format(this.label_xpath_parameterized_widgetItemName, widgetIndex, i+1)));
                    if (isWidgetItemChecked(widgetIndex, i+1))
                    {
                        items.Add(label_widgetItemName.Text.Trim());
                    }
                }
                catch (NoSuchElementException e)
                {
                    break;
                    throw e;
                }
            }

            return items;
        }

        public ArrayList getWidgetSubItemsOnPage(int widgetIndex)
        {
            this._generalMethods.WaitForPageIsLoaded();
            expandWidget(widgetIndex);

            ArrayList items = new ArrayList();

            for (int i = 0; i < int.MaxValue; i++)
            {
                try
                {
                    var label_widgetItemName = this._driver.FindElement(By.XPath(string.Format(this.label_xpath_parameterized_widgetItemName, widgetIndex, i + 1)));
                    items.Add(label_widgetItemName.Text.Trim());
                }
                catch (NoSuchElementException e)
                {
                    break;
                    throw e;
                }
            }

            return items;
        }

        public void checkSubItemByName(int widgetIndex, string itemName)
        {
            this._generalMethods.WaitForPageIsLoaded();
            expandWidget(widgetIndex);

            for (int i = 0; i < int.MaxValue; i++)
            {
                try
                {
                    var label_widgetItemName = this._driver.FindElement(By.XPath(string.Format(this.label_xpath_parameterized_widgetItemName, widgetIndex, i + 1)));
                    if (label_widgetItemName.Text.Trim().Contains(itemName))
                    {
                        if (!isWidgetItemChecked(widgetIndex, i+1))
                        {
                            label_widgetItemName.Click();
                        }
                    }
                }
                catch (NoSuchElementException e)
                {
                    break;
                    throw e;
                }
            }
        }

        public int getTurnedOnWidgetsTotalOnPage(int totalWidgets)
        {
            this._generalMethods.WaitForPageIsLoaded();
            int counter = 0;
            for (int i = 0; i < totalWidgets; i++)
            {
                if (isWidgetTurnedOn(i+1)) 
                {
                    counter++;
                }
            }

            return counter;
        }

        public ArrayList getTurnedOnWidgetsNamesOnPage(int totalWidgets)
        {
            this._generalMethods.WaitForPageIsLoaded();

            ArrayList widgets = new ArrayList();
            for (int i = 0; i < totalWidgets; i++)
            {
                if (isWidgetTurnedOn(i + 1))
                {
                    widgets.Add(getWidgetName(i+1));
                }
            }

            return widgets;
        }

        public ArrayList getAllWidgetsNamesOnPage(int totalWidgets)
        {
            this._generalMethods.WaitForPageIsLoaded();

            ArrayList widgets = new ArrayList();
            for (int i = 0; i < totalWidgets; i++)
            {
                widgets.Add(getWidgetName(i + 1));
            }

            return widgets;
        }

        public void turnOnMultiWidgets(int totalWidgets, int n)
        {
            int index = 1;
            for (int i = 0; i < n; i++)
            { 
                while (index <= totalWidgets)
                {
                    if (!isWidgetTurnedOn(index))
                    {
                        turnOnWidget(index);
                        break;
                    }
                    else
                    {
                        index++;
                    }
                }
            }
        }

        public string turnOnOneAttributeGroup(int totalWidgets)
        {
            string widgetName = null;
            int index = 1;
            while (index <= totalWidgets)
            {
                widgetName = getWidgetName(index);
                if (!widgetName.Equals("Giving") && !widgetName.Equals("Attendance") && !isWidgetTurnedOn(index))
                {
                    turnOnWidget(index);
                    break;
                }
                else
                {
                    index++;
                }
            }

            return widgetName;
        }

        public void turnOffMultiWidgets(int totalWidgets, int n)
        {
            int index = totalWidgets;
            for (int i = 0; i < n; i++)
            {
                while (index > 0)
                {
                    if (isWidgetTurnedOn(index))
                    {
                        turnOffWidget(index);
                        break;
                    }
                    else
                    {
                        index --;
                    }
                }
            }
        }

        public bool isAlertDisplayed()
        {
            this._generalMethods.WaitForPageIsLoaded();
            string script = string.Format("var obj=document.getElementById(\"{0}\");if (!obj && typeof(obj)!=\"undefined\" && obj!=0){{return false;}} else {{return true;}}", this.div_id_alert);
            return bool.Parse(this._driver.ExecuteScript(script).ToString());
        }

        public void closeAlert()
        {
            this._generalMethods.WaitForPageIsLoaded();
            if (isAlertDisplayed())
            {
                this._generalMethods.WaitAndGetElement(By.XPath(this.button_xpath_closeAlert)).Click();
            }
        }
        #endregion

        #region sql data
        public bool hasReportRightOfGiving(int churchId, int userId)
        {
            int count = this._sql.Dashboard_Giving_GetCountOfReportRights(churchId, userId);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string[] getAttributeGroupNameAndId(int widgetIndex)
        {
            turnOnWidget(widgetIndex);
            expandWidget(widgetIndex);
            System.Threading.Thread.Sleep(3000);

            var input = this._driver.FindElement(By.XPath(string.Format(this.input_xpath_parameterized_widgetItemCheckbox, widgetIndex, 1)));
            string id = this._javaScriptExecutor.ExecuteScript("return arguments[0].getAttribute(\"id\");", input).ToString();

            int userWidgetItemId = int.Parse(id.Split('-')[3]);
            string[] attributeGroup = this._sql.Dashboard_GetAttributeGroupID(userWidgetItemId);

            return attributeGroup;
        }
        #endregion
    }
}
