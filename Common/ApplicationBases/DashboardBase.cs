using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Globalization;
using System.Web;
using Selenium;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using System.Threading;

using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

using log4net;

using ActiveUp.Net.Mail;
using System.Net;
using System.IO;
using System.Configuration;
using System.Web.Script.Serialization;
using FTTests.Dashboard;
using System.Collections;


namespace FTTests
{
    #region DashboardBase
    public class DashboardBase
    {
        private RemoteWebDriver _driver;
        private string _dashboardUser;
        private string _dashboardUsername;
        private string _dashboardEmail;
        private string _dashboardPassword;
        private string _churchCode;
        private int _churchID;
        private GeneralMethods _generalMethods;
        private IList<string> _errorText = new List<string>();
        private F1Environments _f1Environment;
        private SQL _sql;

        #region Properties
        public string DashboardUser
        {
            get { return _dashboardUser; }
            set { _dashboardUser = value; }
        }

        public string DashboardEmail
        {
            get { return _dashboardEmail; }
            set { _dashboardEmail = value; }
        }

        public string DashboardUsername
        {
            get { return _dashboardUsername; }
            set { _dashboardUsername = value; }
        }

        public string DashboardPassword
        {
            get { return _dashboardPassword; }
            set { _dashboardPassword = value; }
        }

        public string ChurchCode
        {
            get { return _churchCode; }
            set { _churchCode = value; }
        }

        public int ChurchID
        {
            get { return _churchID; }
            set { _churchID = value; }
        }

        public F1Environments F1Environments
        {
            get { return _f1Environment; }
            set { _f1Environment = value; }
        }

        #endregion Properties

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Constructor method
        public DashboardBase(RemoteWebDriver driver, GeneralMethods generalMethods, F1Environments f1Environment, SQL sql)
        {
            log.Debug("Enter DashboardBase RemoteWebDriver");
            this._driver = driver;
            this._generalMethods = generalMethods;
            this._f1Environment = f1Environment;
            this._sql = sql;

            log.Debug("Exit DashboardBase RemoteWebDriver");
        }
        #endregion

        #region Culture Settings
        public CultureInfo Culture_Settings(int churchId)
        {
            // Store culture for date settings
            CultureInfo culture = null;
            if (churchId.ToString() == "258")
            {
                culture = new CultureInfo("en-GB");
            }
            else
            {
                culture = new CultureInfo("en-US");
            }

            return culture;

        }
        #endregion Culture Settings

        #region Login / Logout
        public void OpenLoginWebDriver()
        {

            TestLog.WriteLine(string.Format("Open Login Page Web Driver"));

            if (!this._driver.Url.ToString().Contains(GetDashboardURL(this._f1Environment)))
            {
                // Open the login page for dashboard
                log.Debug("Navigate to Login Page");
                this._driver.Navigate().GoToUrl(GetDashboardURL(this._f1Environment));

            }
            else
            {
                log.Debug("No need to Navigate to Login");
            }
        }

        public void LoginWebDriver()
        {
            this._generalMethods.WaitForPageIsLoaded();
            LoginWebDriver(this._dashboardUsername, this._dashboardPassword, this._churchCode);
            this._generalMethods.WaitForPageIsLoaded();
        }

        public void LoginWebDriver(string username, string password, string churchCode = "DC")
        {

            TestLog.WriteLine(string.Format("Enter Login Web Driver {0}/{1}/{2}", username, password, churchCode));

            GeneralMethods utility = this._generalMethods;

            TestLog.WriteLine("Login to: " + this._driver.Url.ToString());
            this.OpenLoginWebDriver();
            utility.WaitForPageIsLoaded();
            DashboardLoginPage loginPage = new DashboardLoginPage(this._driver, this._generalMethods);
            loginPage.login(username, password, churchCode);     
        }

        public void LogoutWebDriver()
        {

            log.Debug("Enter Logout Web Driver");

            if (this._driver.FindElementsByLinkText("RETURN").Count > 0)
            {
                this._driver.FindElementByLinkText("RETURN").Click();
            }

            if (this._driver.FindElementsByLinkText("RETURN").Count > 0)
            {
                this._driver.FindElementByLinkText("RETURN").Click();
            }

            this._driver.FindElementByLinkText("sign out").Click();

            log.Debug("Exit Logout Web Driver");

        }
        #endregion Login / Logout

        #region TimeFrame and chart
        public DateTime[] getDateRange(DateTime now, string viewType, int startDay = 0)
        {
            //DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            DateTime[] dateRange = new DateTime[2];
            switch (viewType)
            {
                case "week":
                    dateRange[0] = getStartDateOfWeek(now, startDay);
                    dateRange[1] = dateRange[0].AddDays(6);
                    break;
                case "month":
                    dateRange[0] = getStartDateOfMonth(now);
                    dateRange[1] = dateRange[0].AddMonths(1).AddDays(-1);
                    break;
                case "quarter":
                    dateRange[0] = getStartDateOfQuarter(now);
                    dateRange[1] = dateRange[0].AddMonths(3).AddDays(-1);
                    break;
                case "year":
                    dateRange[0] = new DateTime(now.Year, 1, 1);
                    dateRange[1] = dateRange[0].AddYears(1).AddDays(-1);
                    break;
                default:
                    throw new Exception("It is not a valid view type");
            }
            return dateRange;
        }

        public ArrayList getStartDatesOfChart(DateTime now, string viewType, int startDay = 0)
        {
            //DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            ArrayList dates = new ArrayList();
            switch (viewType)
            {
                case "week":
                    dates = getStartDaysOfLatestTwoYearsIsoWeeks(now, startDay);
                    break;
                case "month":
                    for (int i = 1; i <= 12; i++)
                    {
                        dates.Add(new DateTime(now.Year - 1, i, 1));
                    }
                    for (int i = 1; i <= 12; i++)
                    {
                        dates.Add(new DateTime(now.Year, i, 1));
                    } 
                    break;
                case "quarter":
                    for (int i = 1; i < 12; i = i + 3)
                    {
                        dates.Add(new DateTime(now.Year - 1, i, 1));
                    }
                    for (int i = 1; i < 12; i = i + 3)
                    {
                        dates.Add(new DateTime(now.Year, i, 1));
                    }
                    break;
                case "year":
                    for (int i = 24; i >= 0; i-- )
                    {
                        dates.Add(new DateTime(now.Year - i, 1, 1));
                    }
                    break;
                default:
                    throw new Exception("It is not a valid view type");
            }
            return dates;
        }
       
        public DateTime getStartDateOfMonth(DateTime now)
        {
            return new DateTime(now.Year, now.Month, 1);
        }

        public DateTime getStartDateOfQuarter(DateTime now)
        {        
            int month = now.Month;
            month = (month - 1) / 3 + (month - 1) / 3 + (month - 1) / 3 + 1;

            return new DateTime(now.Year, month, 1);
        }

        public ArrayList getStartDaysOfIsoWeeks(DateTime now, int startDay)
        {
            int year = now.Year;

            ArrayList startDays = new ArrayList();

            DateTime startDateOfweek = getStartDateOfWeek(now, startDay);

            while (startDateOfweek.Year == year)
            {
                startDateOfweek = startDateOfweek.AddDays(-7);
            }


            if (!isWeekInCurrentYear(year, startDateOfweek))
            {
                startDateOfweek = startDateOfweek.AddDays(7);
            }

            while (startDateOfweek.AddDays(7).Year == year)
            {
                startDays.Add(startDateOfweek);
                startDateOfweek = startDateOfweek.AddDays(7);
            }

            if (isWeekInCurrentYear(year, startDateOfweek))
            {
                startDays.Add(startDateOfweek);
            }

            return startDays;
        }

        public ArrayList getStartDaysOfLatestTwoYearsIsoWeeks(DateTime now, int startDay)
        {
            int year = now.Year;
            ArrayList startDays = new ArrayList();

            DateTime startDateOfweek = getStartDateOfWeek(now, startDay);

            while (startDateOfweek.Year == year || startDateOfweek.Year == year - 1)
            {
                startDateOfweek = startDateOfweek.AddDays(-7);
            }


            if (!isWeekInCurrentYear(year - 1, startDateOfweek))
            {
                startDateOfweek = startDateOfweek.AddDays(7);
            }

            while (startDateOfweek.AddDays(7).Year == year - 1 || startDateOfweek.AddDays(7).Year == year)
            {
                startDays.Add(startDateOfweek);
                startDateOfweek = startDateOfweek.AddDays(7);
            }

            if (isWeekInCurrentYear(year, startDateOfweek))
            {
                startDays.Add(startDateOfweek);
            }

            return startDays;
        }

        public DateTime getStartDayOfWeekByWeekNumber(int number, DateTime now, int startDay)
        {
            int index = 0;
            DateTime startDayOfWeek = new DateTime(now.Year, 12, 31);

            ArrayList weekStartDays = getStartDaysOfIsoWeeks(now, startDay);
            foreach (DateTime start in weekStartDays)
            {
                if (number == (++index))
                {
                    startDayOfWeek = start;
                    break;
                }
            }

            return startDayOfWeek;
        }
        /// <summary>
        /// Get the week number against specific start day setting
        /// This method need to improve later
        /// </summary>
        /// <param name="now">now</param>
        /// <param name="startDay">start day of every week</param>
        /// <returns></returns>
        public int getWeekNumberOfYear(DateTime now, int year, int startDay)
        {
            int number = 1;
            ArrayList startDays = getStartDaysOfIsoWeeks(now, startDay);

            foreach (DateTime start in startDays)
            {
                DateTime end = start.AddDays(6);
                if (end.Year <= year)
                {
                    if (end.Month > now.Month)
                    {
                        break;
                    }
                    else if (end.Month == now.Month && end.Day > now.Day)
                    {
                        break;
                    }
                    else
                    {
                        number++;
                    }
                }
                else
                {
                    break;
                }
            }

            return number;
        }
            
        public bool isWeekInCurrentYear(int year, DateTime startDayOfWeek)
        {
            return startDayOfWeek.AddDays(3).Year == year;
        }

        public DateTime getStartDateOfWeek(DateTime now, int startDay)
        {
            DateTime startDate;
            int current = (int)now.DayOfWeek;
            
            if (startDay <= current)
            {
                startDate = now.AddDays(startDay - current);
            }
            else
            {
                startDate = now.AddDays(startDay - current - 7);
            }

            return startDate;
        }

        public int getIntStartDay(string dayOfWeek)
        {
            int startDay = 0;
            switch(dayOfWeek)
            {
                case "sunday":
                    startDay = 0;
                    break;
                case "monday":
                    startDay = 1;
                    break;
                case "tuesday":
                    startDay = 2;
                    break;
                case "wednesday":
                    startDay = 3;
                    break;
                case "thursday":
                    startDay = 4;
                    break;
                case "friday":
                    startDay = 5;
                    break;
                case "saturday":
                    startDay = 6;
                    break;
                default:
                    throw new Exception("It is not a valid day of week");
            }
            return startDay;
        }

        public string removeZeroInTheEndOfDecimalpart(string strDecimal)
        {
            if (strDecimal.EndsWith(".00"))
            {
                strDecimal = strDecimal.Substring(0, strDecimal.Length - 3);
            }
            if (strDecimal.EndsWith("0"))
            {
                strDecimal = strDecimal.Substring(0, strDecimal.Length - 1);
            }
            return strDecimal;
        }
        #endregion

        #region External
        public void checkAccessRight(int userId, string accessRightName)
        {
            GeneralMethods utility = this._generalMethods;
            string path = string.Format("{0}//{1}", this._driver.Url.ToString().Split('/')[0], this._driver.Url.ToString().Split('/')[2]);

            if (!this._driver.Url.ToString().Contains("/admin/security/manageuserroles.aspx?userId="))
            {
                log.Debug("Navigate to manage user roles page");
                this._driver.Navigate().GoToUrl(string.Format("{0}/admin/security/manageuserroles.aspx?userId={1}", path, userId));
            }

            utility.WaitForPageIsLoaded();

            var checkBox = utility.WaitAndGetElement(By.XPath(string.Format("//span[text()='{0}']/parent::div/input", accessRightName)));
            if (!bool.Parse(this._driver.ExecuteScript("return arguments[0].checked;", checkBox).ToString()))
            {
                checkBox.Click();
            }

            utility.WaitAndGetElement(By.Id("ctl00_ctl00_MainContent_content_btnSave")).Click();
            utility.WaitForPageIsLoaded();
        }

        public void uncheckAccessRight(int userId, string accessRightName)
        {
            GeneralMethods utility = this._generalMethods;
            string path = string.Format("{0}//{1}", this._driver.Url.ToString().Split('/')[0], this._driver.Url.ToString().Split('/')[2]);
            if (!this._driver.Url.ToString().Contains("/admin/security/manageuserroles.aspx?userId="))
            {
                log.Debug("Navigate to manage user roles page");
                this._driver.Navigate().GoToUrl(string.Format("{0}/admin/security/manageuserroles.aspx?userId={1}", path, userId));
            }

            utility.WaitForPageIsLoaded();

            var checkBox = utility.WaitAndGetElement(By.XPath(string.Format("//span[text()='{0}']/parent::div/input", accessRightName)));
            if (bool.Parse(this._driver.ExecuteScript("return arguments[0].checked;", checkBox).ToString()))
            {
                checkBox.Click();
            }

            utility.WaitAndGetElement(By.Id("ctl00_ctl00_MainContent_content_btnSave")).Click();
            utility.WaitForPageIsLoaded();
        }

        public void uncheckAllRoles(int userId)
        {
            GeneralMethods utility = this._generalMethods;
            string path = string.Format("{0}//{1}", this._driver.Url.ToString().Split('/')[0], this._driver.Url.ToString().Split('/')[2]);
            if (!this._driver.Url.ToString().Contains("/admin/security/manageuserroles.aspx?userId="))
            {
                log.Debug("Navigate to manage user roles page");
                this._driver.Navigate().GoToUrl(string.Format("{0}/admin/security/manageuserroles.aspx?userId={1}", path, userId));
            }

            utility.WaitForPageIsLoaded();

            for (int i = 0; i < int.MaxValue; i++)
            {
                try
                {
                    var checkBox = this._driver.FindElement(By.XPath(string.Format("//table[@id='ctl00_ctl00_MainContent_content_dgRoles']/tbody/tr[{0}]/td/span/input", i + 1)));
                    if (bool.Parse(this._driver.ExecuteScript("return arguments[0].checked;", checkBox).ToString()))
                    {
                        checkBox.Click();
                    }
                }
                catch (NoSuchElementException e)
                {
                    break;
                    throw e;
                }
            }

            utility.WaitAndGetElement(By.Id("ctl00_ctl00_MainContent_content_btnSave")).Click();
            utility.WaitForPageIsLoaded();
        }

        public void checkAllRoles(int userId)
        {
            GeneralMethods utility = this._generalMethods;
            string path = string.Format("{0}//{1}", this._driver.Url.ToString().Split('/')[0], this._driver.Url.ToString().Split('/')[2]);
            if (!this._driver.Url.ToString().Contains("/admin/security/manageuserroles.aspx?userId="))
            {
                log.Debug("Navigate to manage user roles page");
                this._driver.Navigate().GoToUrl(string.Format("{0}/admin/security/manageuserroles.aspx?userId={1}", path, userId));
            }

            utility.WaitForPageIsLoaded();

            for (int i = 0; i < int.MaxValue; i++)
            {
                try
                {
                    var checkBox = this._driver.FindElement(By.XPath(string.Format("//table[@id='ctl00_ctl00_MainContent_content_dgRoles']/tbody/tr[{0}]/td/span/input", i + 1)));
                    if (!bool.Parse(this._driver.ExecuteScript("return arguments[0].checked;", checkBox).ToString()))
                    {
                        checkBox.Click();
                    }
                }
                catch (NoSuchElementException e)
                {
                    break;
                    throw e;
                }
            }

            utility.WaitAndGetElement(By.Id("ctl00_ctl00_MainContent_content_btnSave")).Click();
            utility.WaitForPageIsLoaded();
        }
        #endregion

        #region Giving
        public string getBigNumberStringGreaterThanMillion(double total)
        {
            if (total > 1000000)
            {
                //return string.Format("${0}M", string.Format("{0:0,00.00}", Math.Round(total / 1000000, 2)));
                return string.Format("${0}M", string.Format("{0:n}", total / 1000000));
            }
            else
            {
                return string.Format("${0}", string.Format("{0:n}", total));
            }
        }
        #endregion

        #region ScreenShot

        public void TakeScreenShot_WebDriver()
        {

            ITakesScreenshot webDriver = (ITakesScreenshot)this._driver;
            Screenshot ss = webDriver.GetScreenshot();
            string screenshot = ss.AsBase64EncodedString;
            byte[] imageBytes = Convert.FromBase64String(ss.AsBase64EncodedString);

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                // Convert byte[] to Image
                ms.Write(imageBytes, 0, imageBytes.Length);
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);

                // Embed the image to the log
                TestLog.EmbedImage(null, image);

            }

        }

        #endregion ScreenShot

        #region Static Methods
        public static string GetDashboardURL(F1Environments f1Environment)
        {

            string returnValue = string.Empty;

            switch (f1Environment)
            {
                case F1Environments.LOCAL:
                    break;
                case F1Environments.INT:
                case F1Environments.INT2:
                case F1Environments.INT3:
                case F1Environments.LV_QA:
                case F1Environments.LV_UAT:
                case F1Environments.STAGING:
                    returnValue = string.Format("https://dashboard.{0}.fellowshipone.com", GetLVEnvironment(f1Environment));
                    break;
                case F1Environments.LV_PROD:
                    returnValue = "https://dashboard.fellowshipone.com";
                    break;
                default:
                    throw new Exception("Not a valid environment!!");
            }

            TestLog.WriteLine("URL: " + returnValue);
            return returnValue;
            //return "http://wsf1qaweb10a.dev.activenetwork.com:8042";
        }

        public static string GetLVEnvironment(F1Environments f1Environment)
        {
            string returnValue = string.Empty;

            switch (f1Environment)
            {
                case F1Environments.LV_QA:
                    returnValue = "QA";
                    break;
                case F1Environments.LV_UAT:
                case F1Environments.STAGING:
                    returnValue = "STAGING";
                    break;
                case F1Environments.INT:
                    returnValue = "INT";
                    break;
                case F1Environments.INT2:
                    returnValue = "INT2";
                    break;
                case F1Environments.INT3:
                    returnValue = "INT3";
                    break;
                case F1Environments.LV_PROD:
                    returnValue = "PROD";
                    break;
                default:
                    throw new Exception(string.Format("[{0}] is Not a valid LV environment option!", f1Environment));

            }

            return returnValue.ToLower();

        }

        #endregion Static Methods
    }
    #endregion

    #region DashboardAPIBase
    public class DashboardAPIBase
    {
        private F1Environments _f1Environment;
        private ExeConfigurationFileMap _appConfigFileMap = new ExeConfigurationFileMap();
        private Configuration _configuration;

        private string _churchcode;
        private string _consumerKey;
        private string _consumerSecret;
        private string _username;
        private string _password;
        private string _email;

        public string ChurchCode {
            get 
            { 
                return this._churchcode; 
            }
            set 
            {
                this._churchcode = value;
            }
        }
        public string ConsumerKey 
        {
            get
            {
                return this._consumerKey;
            }
            set
            {
                this._consumerKey = value;
            }
        }
        public string ConsumerSecret 
        {
            get
            {
                return this._consumerSecret;
            }
            set
            {
                this._consumerSecret = value;
            }
        }
        public string Username 
        {
            get
            {
                return this._username;
            }
            set
            {
                this._username = value;
            }
        }
        public string Password 
        {
            get
            {
                return this._password;
            }
            set
            {
                this._password = value;
            }
        }
        public string Email
        {
            get
            {
                return this._email;
            }
            set
            {
                this._email = value;
            }
        }

        #region constructor methods
        public DashboardAPIBase()
        {
            // Configure and open the config file
            _appConfigFileMap.ExeConfigFilename = @"..\..\..\Common\bin\Debug\Common.dll.config";
            _configuration = ConfigurationManager.OpenMappedExeConfiguration(_appConfigFileMap, ConfigurationUserLevel.None);

            // Set Environment variables
            this._f1Environment = (F1Environments)Enum.Parse(typeof(F1Environments), _configuration.AppSettings.Settings["FTTests.Environment"].Value);
            this._churchcode = _configuration.AppSettings.Settings["FTTests.ChurchCode"].Value;

            this._consumerKey = _configuration.AppSettings.Settings["FTTests.DashboardAPIConsumerKey"].Value;
            this._consumerSecret = _configuration.AppSettings.Settings["FTTests.DashboardAPIConsumerSecret"].Value;
            this._username = _configuration.AppSettings.Settings["FTTests.DashboardAPIUsername"].Value;
            this._password = _configuration.AppSettings.Settings["FTTests.DashboardAPIPassword"].Value;
            this._email = _configuration.AppSettings.Settings["FTTests.DashboardAPIEmail"].Value;  
        }
        #endregion

        #region Public methods
        public HttpWebResponse getAccessTokenObject(string consumerKey, string consumerSecret, string username, string password, string churchcode)
        {
            string url = this.GetAPIURL() + "/api/authenticate/token";
            StringBuilder jsonBody = new StringBuilder();
            jsonBody.Append("{ ");
            jsonBody.AppendFormat("consumerKey:\"{0}\",", consumerKey);
            jsonBody.AppendFormat("consumerSecret: \"{0}\",", consumerSecret);
            jsonBody.Append("grantType:\"password\",");
            jsonBody.AppendFormat("userName:\"{0}\",", username);
            jsonBody.AppendFormat("password :\"{0}\",", password);
            jsonBody.AppendFormat("churchCode :\"{0}\" ", churchcode);
            jsonBody.Append("}");

            TestLog.WriteLine("Get access token of dashboard...");

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "Post";
            request.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(jsonBody.ToString());
                streamWriter.Flush();
                streamWriter.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //AccessToken obj = JsonToObject<AccessToken>(response);
            return response;
        }
        
        public T DoGetRequest<T>(string relatedUrl)
        {
            string token = this.getAccessToken();
            string url = String.Format("{0}{1}", this.GetAPIURL(), relatedUrl);
            TestLog.WriteLine("API Request: " + url);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            request.Method = "Get";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", token);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            T obj = this.JsonToObject<T>(response);
            return obj;
        }

        public HttpWebResponse DoGetRequest(string relatedUrl)
        {
            string token = this.getAccessToken();
            string url = String.Format("{0}{1}", this.GetAPIURL(), relatedUrl);
            TestLog.WriteLine("API Request: " + url);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            request.Method = "Get";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", token);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response;
        }

        public T DoPostRequest<T>(string relatedUrl, string jsonBody)
        {
            string token = this.getAccessToken();
            string url = String.Format("{0}{1}", this.GetAPIURL(), relatedUrl);
            TestLog.WriteLine("API Request: " + url);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            request.Method = "Post";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", token);

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(jsonBody.ToString());
                streamWriter.Flush();
                streamWriter.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            T obj = this.JsonToObject<T>(response);
            return obj;
        }

        public HttpWebResponse DoPostRequest(string relatedUrl, string jsonBody)
        {
            string token = this.getAccessToken();
            string url = String.Format("{0}{1}", this.GetAPIURL(), relatedUrl);
            TestLog.WriteLine("API Request: " + url);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            request.Method = "Post";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", token);

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(jsonBody.ToString());
                streamWriter.Flush();
                streamWriter.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response;
        }
        #endregion

        #region Prase json methods
        public T JsonToObject<T>(string strJson)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            T obj = (T)js.Deserialize(strJson, typeof(T));
            return obj;
        }

        public T JsonToObject<T>(HttpWebResponse jsonResponse)
        {
            using (var reader = new StreamReader(jsonResponse.GetResponseStream()))
            {
                var json = reader.ReadToEnd();
                return JsonToObject<T>(json);
            }
        }
        #endregion

        #region Private Methods
        private string GetAPIURL()
        {
            string returnValue = string.Empty;

            switch (this._f1Environment)
            {
                case F1Environments.LOCAL:
                    break;
                case F1Environments.INT:
                case F1Environments.INT2:
                case F1Environments.INT3:
                    returnValue = string.Format("https://f1data.{0}.fellowshipone.com", this._f1Environment.ToString());
                    break;
                case F1Environments.LV_QA:
                    returnValue = "https://f1data.qa.fellowshipone.com";
                    break;
                case F1Environments.LV_UAT:
                case F1Environments.STAGING:
                    returnValue = "https://f1data.staging.fellowshipone.com";
                    break;
                case F1Environments.LV_PROD:
                    returnValue = "https://f1data.fellowshipone.com";
                    break;
                default:
                    throw new Exception("Not a valid environment!!");
            }

            TestLog.WriteLine("URL: " + returnValue);
            return returnValue;
            //return "http://wsf1qaweb12a.dev.activenetwork.com:8016";
        }

        private string getAccessToken()
        {
            string url = this.GetAPIURL() + "/api/authenticate/token";
            StringBuilder jsonBody = new StringBuilder();
            jsonBody.Append("{ ");
            jsonBody.AppendFormat("consumerKey:\"{0}\",", this._consumerKey);
            jsonBody.AppendFormat("consumerSecret: \"{0}\",", this._consumerSecret);
            jsonBody.Append("grantType:\"password\",");
            jsonBody.AppendFormat("userName:\"{0}\",", this._username);
            jsonBody.AppendFormat("password :\"{0}\",", this._password);
            jsonBody.AppendFormat("churchCode :\"{0}\" ", this._churchcode);
            jsonBody.Append("}");

            TestLog.WriteLine("Get access token of dashboard...");

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "Post";
            request.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(jsonBody.ToString());
                streamWriter.Flush();
                streamWriter.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (HttpStatusCode.OK == response.StatusCode)
            {
                var obj = JsonToObject<AccessToken>(response);
                return string.Format("{0} {1}", obj.tokenType, obj.accessToken);
            }
            else
            {
                return null;
                throw new Exception(string.Format("HttpStatusCode is {0}:{1}", response.StatusCode, response.StatusDescription));
            }

        }
        #endregion

    }
    #endregion

    #region AccessToken Obj
    public class User
    {
        public string userId { get; set; }
        public string churchId { get; set; }
        public string churchName { get; set; }
        public string firstName { get; set; }
        public string goesBy { get; set; }
        public string lastName { get; set; }
    }

    public class AccessToken
    {
        public string tokenType { get; set; }
        public string accessToken { get; set; }
        public string expirationDate { get; set; }
        public string refreshToken { get; set; }
        public User user { get; set; }
    }
    #endregion
}
