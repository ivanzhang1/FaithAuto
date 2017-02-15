using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace FTTests.Dashboard
{
    public class DashboardLoginPage
    {
        private RemoteWebDriver _driver;
        private GeneralMethods _generalMethods;

        #region locator
        private string input_id_username = "username";
        private string input_id_password = "password";
        private string input_id_churchCode = "church-code";

        private string button_xpath_signIn = "//button[contains(text(), 'Sign in')]";

        private string div_xpath_errorText = "//div[@ng-show='errorText']";
        #endregion

        public DashboardLoginPage(RemoteWebDriver driver, GeneralMethods generalMethods)
        {
            this._driver = driver;
            this._generalMethods = generalMethods;
        }

        public void login(string username, string password, string churchCode)
        {
            var input_username = this._driver.FindElement(By.Id(this.input_id_username));
            input_username.Clear();
            input_username.SendKeys(username);

            var input_password = this._driver.FindElement(By.Id(this.input_id_password));
            input_password.Clear();
            input_password.SendKeys(password);

            var input_churCode = this._driver.FindElement(By.Id(this.input_id_churchCode));
            input_churCode.Clear();
            input_churCode.SendKeys(churchCode);

            var button_signIn = this._driver.FindElement(By.XPath(this.button_xpath_signIn));
            button_signIn.Click();
            this._generalMethods.WaitForPageIsLoaded();
        }

        public string getLoginErrorText()
        {
            System.Threading.Thread.Sleep(3000);
            var div_errorText = this._generalMethods.WaitAndGetElement(By.XPath(this.div_xpath_errorText));
            return div_errorText.Text.Trim();
        }
    }
}
