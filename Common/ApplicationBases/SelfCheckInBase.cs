using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Globalization;
using System.Web;
using System.Net;
using Selenium;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using System.Threading;
using System.Drawing;

using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

using log4net;

using ActiveUp.Net.Mail;


namespace FTTests
{

    [TestFixture]
    public class SelfCheckInBaseWebDriver : FixtureBaseWebDriver
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected int churchId;
        protected string timeZoneName;
        protected DateTime currentTimzoneTime;
        private string currentDate;
        protected string[] rosterNameArray;
        protected string rosterToCheckIn;
        protected static string activityName = "";
        public static DateTime activityStartime;

        [SetUp]
        public void SelfCheckInBaseWebDriver_SetUp()
        {

            log.Debug("Enter SelfCheckIn Setup WebDriver");

            churchId = base.SQL.Ministry_Church_FetchID(test.SelfCheckIn.ChurchCode);
            timeZoneName = base.SQL.Ministry_Activity_Instance_TimeZone(churchId);

            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            currentDate = Convert.ToString(currentTimzoneTime.Year) + Convert.ToString(currentTimzoneTime.Month) + Convert.ToString(currentTimzoneTime.Day);

            string rosterNamepre = "Roster_";
            rosterNameArray = new string[] { rosterNamepre + "_001", rosterNamepre + "_002", rosterNamepre + "_003", rosterNamepre + "_full", rosterNamepre + "_close" };
            rosterToCheckIn = rosterNameArray[0];

            // Activity resource naming
            if ("".Equals(activityName))
            {
                activityName = "Activity-SelfCheckIn-T3";
            }

            if (base.SQL.Ministry_Activities_FetchID(churchId, activityName) != -1 && base.SQL.Ministry_Activities_FetchIDS(churchId, activityName).Length == 1 && 
                System.Math.Abs((Convert.ToDateTime(Convert.ToString(currentTimzoneTime).Split(' ')[0]).Subtract(
                Convert.ToDateTime(base.SQL.Ministry_Activities_FetchCreatedDate(churchId, activityName).Split(' ')[0]))).TotalDays) < 1)
            {
                string[] createdDateArray = base.SQL.Ministry_Activities_FetchCreatedDate(churchId, activityName).Split(' ');
                string[] currentTimeArray = Convert.ToString(currentTimzoneTime).Split(' ');
                int currentHour = 0;
                int createdDateHour = 0;
                int currentMinute = 0;
                int createdMinute = 0;
                if (createdDateArray[2].Equals("AM"))
                {
                    createdDateHour = Convert.ToInt32(createdDateArray[1].Split(':')[0]);
                }
                else
                {
                    createdDateHour = Convert.ToInt32(createdDateArray[1].Split(':')[0]) + 12;
                }
                if (currentTimeArray[2].Equals("AM"))
                {
                    currentHour = Convert.ToInt32(currentTimeArray[1].Split(':')[0]);
                }
                else
                {
                    currentHour = Convert.ToInt32(currentTimeArray[1].Split(':')[0]) + 12;
                }
                TestLog.WriteLine("-createdDateHour = {0}", createdDateHour);
                TestLog.WriteLine("-currentHour = {0}", currentHour);
                TestLog.WriteLine("-createdDateArray = {0}", createdDateArray);
                TestLog.WriteLine("-currentTimeArray = {0}", currentTimeArray);

                currentMinute = Convert.ToInt32(currentTimeArray[1].Split(':')[1]);
                createdMinute = Convert.ToInt32(createdDateArray[1].Split(':')[1]);

                if ((currentHour * 60 + currentMinute) - (createdDateHour * 60 + createdMinute) > -60)
                {                  
                    //TestLog.WriteLine("The existent activity is out of time, will create a new one.");
                }
                else
                {
                    TestLog.WriteLine("The activity with given name: " + activityName + " is already existent in DB and can be used, skip creating it.");
                    return;
                }
                
            }

            string activityScheduleName = "ScheduleForSelfCheckIn";
            TestLog.WriteLine("-activityScheduleName = {0}", activityScheduleName);

            // Set under which ministry the activity wil be created
            string ministryName = "A Test Ministry"; // A Test Ministry, Bible Study

            int timeToSchedule = 4;
            int interalHour = 24 - currentTimzoneTime.Hour - timeToSchedule;

            if (interalHour < 1)
            {
                interalHour = 24 - currentTimzoneTime.Hour - 1;
                if (interalHour < 1)
                {
                    TestLog.WriteLine("The rest of the day may not be enough for create activity for assignments in 4 houres and test execution, please wait!");
                    Assert.Fail("The rest of the day may not be enough for create activity for assignments in 4 houres and test execution, will stop tests now. and please trigger the test after US day passes.");
                }
                else
                {
                    timeToSchedule = 1;
                }
            }

            int ministryId = base.SQL.Ministry_Ministries_FetchID(churchId, ministryName);
            TestLog.WriteLine("Delete all activities with the test name");
            base.SQL.Ministry_ActivitySchedules_DeleteActivitiesForSelfChekIn(churchId, activityName);

            //create Activity
            TestLog.WriteLine("Create an activity which will happen in " + timeToSchedule + " hours");
            activityStartime = currentTimzoneTime.AddHours(timeToSchedule);
            base.SQL.Ministry_Activities_CreateForSelfCheckIn(churchId, ministryId, activityName, activityStartime, 0, rosterNameArray);
            //create schedule
            TestLog.WriteLine("Create a schedule for the activity");
            base.SQL.Ministry_ActivitySchedules_CreateForCheckIn(churchId, activityName, activityScheduleName, rosterNameArray,
                activityStartime, activityStartime.AddHours(1));

            //Volunteer/Staff Assignment
            TestLog.WriteLine("Create staff assignments to the activity");
            String[] individualVolNameArray = { "GraceAutoHousehold Zhang", "GraceAutoChildOld Zhang" };
            base.SQL.VolStaffAssignment_CreateForSelfCheckIn(churchId, activityName, activityScheduleName, rosterNameArray[0], individualVolNameArray, 2, currentTimzoneTime.AddHours(0));

            //Participant Assignment
            TestLog.WriteLine("Create participant assignments to the activity");
            String[] individualPartiNameArray = { "GraceAutoChildYong Zhang", "GraceAutoOther Zhang", "GraceAutoDrop Zhang", "GraceAutoVisitor1 Zhang" };
            base.SQL.PartiAssignment_CreateForSelfCheckIn(churchId, activityName, activityScheduleName, rosterNameArray[0], individualPartiNameArray, currentTimzoneTime.AddHours(0));

            TestLog.WriteLine("Create participant assignments to the activity with all three types of effection type");
            String[] individualPartiAssignRuleArray = { "GraceAutoAssignRuleTester1 Zhang" };
            base.SQL.PartiAssignment_CreateForSelfCheckInAssignRule(churchId, activityName, activityScheduleName, rosterNameArray, individualPartiAssignRuleArray, currentTimzoneTime.AddHours(0));

        }

        [TearDown]
        public void SelfCheckInBaseWebDriver_TearDown()
        {

            TestLog.WriteLine("Enter SelfCheckIn TearDown WebDriver");

            // Uncomment below code if want to delete activity after case execution
            // TestLog.WriteLine("Delete the activity used in Self CheckIn cases");
            // base.SQL.Ministry_ActivitySchedules_DeleteActivityForSelfChekIn(churchId, activityName);

            test.Driver.Quit();

            TestLog.WriteLine("Exit SelfCheckIn TearDown WebDriver");

        }

    }

    public class SelfCheckInBase
    {
        private RemoteWebDriver _driver;
        private ISelenium _selenium;
        private string _selfCheckInUsername;
        private string _selfCheckInPassword;
        private string _churchCode;
        private string _selfCheckInUser;
        private int _churchID;
        private GeneralMethods _generalMethods;
        private IList<string> _errorText = new List<string>();
        private JavaScript _javascript;
        private F1Environments _f1Environment;
        private SQL _sql;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Properties

        public string SelfCheckInUsername
        {
            get { return _selfCheckInUsername; }
            set { _selfCheckInUsername = value; }
        }

        public string SelfCheckInPassword
        {
            get { return _selfCheckInPassword; }
            set { _selfCheckInPassword = value; }
        }

        public string ChurchCode
        {
            get { return _churchCode; }
            set { _churchCode = value; }
        }

        public string SelfCheckInUser
        {
            get { return _selfCheckInUser; }
            set { _selfCheckInUser = value; }
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

        #region Instance Methods

        public SelfCheckInBase(ISelenium selenium, GeneralMethods generalMethods, JavaScript javascript, F1Environments f1Environment, SQL sql)
        {
            log.Debug("Enter CheckInBase");
            this._selenium = selenium;
            this._generalMethods = generalMethods;
            this._javascript = javascript;
            this._f1Environment = f1Environment;
            this._sql = sql;

            log.Debug("Exit CheckInBase");
        }

        public SelfCheckInBase(RemoteWebDriver driver, GeneralMethods generalMethods, F1Environments f1Environment, SQL sql)
        {
            log.Debug("Enter CheckInBase RemoteWebDriver");
            this._driver = driver;
            //this._screenShotDriver = driver;
            this._generalMethods = generalMethods;
            this._f1Environment = f1Environment;
            this._sql = sql;

            log.Debug("Exit CheckInBase RemoteWebDriver");
        }

        #endregion Instance Methods

        #region Culture Settings

        public CultureInfo Culture_Settings(int churchId)
        {
            // Store culture for date settings,later modify
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
        // Logs into checkin using credentials from app.config

        public void LoginWebDriver(Boolean needAssert = true)
        {
            LoginWebDriver(this._selfCheckInUsername, this._selfCheckInPassword, this._churchCode, needAssert);
        }

        public void LoginWebDriver(string username, string password, string churchCode, Boolean needAssert = true)
        {

            TestLog.WriteLine(string.Format("Enter Login Web Driver {0}/{1}/{2}", username, password, churchCode));

            //Check if we are already in login page
            //This is when login/logout out of application
            if (!this._driver.Url.ToString().Contains(GetSelfCheckInURL(this._f1Environment, churchCode)))
            {
                // Open the login page for Self Checkin project
                log.Debug("Navigate to Login Page");
                this._driver.Navigate().GoToUrl(GetSelfCheckInURL(this._f1Environment, churchCode));

            }
            else
            {
                log.Debug("No need to Navigate to Login");
            }

            TestLog.WriteLine("Login to: " + this._driver.Url.ToString());

            // Set window size to simulate iPhone
            SetBrowserSizeTo_Mobile();

            IWebElement ele = this._driver.FindElementByName("username");
            ele.Click();
            ele.Clear();
            ele.SendKeys(username);

            ele = this._driver.FindElementByName("password");
            ele.Click();
            ele.Clear();
            ele.SendKeys(password);

            // Attempt to log in
            this._driver.FindElementByCssSelector("[type=\"submit\"]").Click();

            //We need to verify that we don't get routed to login page
            TestLog.WriteLine("URL routed to: " + this._driver.Url.ToString());

            bool logInSucceed = false;
            try
            {
                Thread.Sleep(3 * 1000);
                String churchName = this._sql.FetchChurchName(churchCode);               
                IWebElement titleCenterElement = this._driver.FindElementsByCssSelector("div[class=\"title title-center header-item\"]")[0];                                                                                                                         
                String titleName = titleCenterElement.Text.ToString();
                Assert.AreEqual(churchName, titleName);

                logInSucceed = true;
                Assert.IsTrue(logInSucceed == needAssert, "The real log in response is diff to expecation");
            }
            catch (Exception e)
            {
                if (!needAssert)
                {
                    TestLog.WriteLine("It fails to log in, this is what expected.");
                }
                else
                {
                    TestLog.WriteLine("Login failed first time, try again to it: " + this._driver.Url.ToString());

                    // Set the username and password.

                    ele = this._driver.FindElementByName("username");
                    ele.Clear();
                    ele.SendKeys(username);

                    ele = this._driver.FindElementByName("password");
                    ele.Clear();
                    ele.SendKeys(password);

                    // Attempt to log in
                    this._driver.FindElementByCssSelector("[type=\"submit\"]").Click();
                }

            }

            TestLog.WriteLine("Method LoginWebDriver ran completed");

        }

        public void WaitForAssignmentLoaded()
        {
            TestLog.WriteLine("Start to wait for assignment page loads well");

            try
            {
                TestLog.WriteLine("Waiting for individual name display");
                this._generalMethods.WaitForElement(By.XPath("//h3[@class='name-info']"));

                TestLog.WriteLine("Waiting for indivudal assignments panel display");
                this._generalMethods.WaitForElement(By.XPath("//div[@class='assignments-wrapper']"));

                TestLog.WriteLine("Waiting for Individual details icon display");
                this._generalMethods.WaitForElement(By.CssSelector("[class=\"action\"]"));

                TestLog.WriteLine("Waiting for assignment details display");
                this._generalMethods.WaitForElement(By.XPath("//ion-item[@class='no-pad assignment-item item']"));
            }
            catch (WebDriverException e)
            {
                TestLog.WriteLine("Warning: Some elements on roster may not display well." + e.Message);
            }

            TestLog.WriteLine("Wait for assignment page loading completed");
        }

        public void ClickIconMenu()
        {
            TestLog.WriteLine("Click Menu icon to expand my profile");
            this._generalMethods.WaitForElement(this._driver, By.CssSelector("[class=\"button-clear icon-user fa fa-bars\"]"));
            IWebElement rightButtonsElement = this._driver.FindElementByCssSelector("div[class=\"nav-bar-block\"][nav-bar=\"active\"]").FindElement(By.TagName("ion-header-bar")).FindElement(By.CssSelector("div[class=\"buttons buttons-right header-item\"]")).FindElement(By.CssSelector("span[class=\"right-buttons\"]"));
            rightButtonsElement.FindElement(By.CssSelector("a[class=\"button-clear icon-user fa fa-bars\"]")).Click();

            try
            {
                this._generalMethods.WaitForElement(By.CssSelector("[style=\"transform: translate3d(0%, 0px, 0px);\"]"));
            }
            catch (Exception e)
            {
                TestLog.WriteLine("Roster menu doesn't expanded in the first click, try again");
                this._driver.FindElement(By.XPath("//div[@class='nav-bar-block'][@nav-bar='active']//a[@menu-toggle='right']")).Click();
            }
        }
        
        public void LogoutWebDriver()
        {
            TestLog.WriteLine("Click Log out button");
            try
            {
                this._generalMethods.WaitForElement(this._driver, By.CssSelector("[ng-click=\"user.signOut()\"]"));
            }
            catch (Exception)
            {
                ClickIconMenu();
                this._generalMethods.WaitForElement(this._driver, By.CssSelector("[ng-click=\"user.signOut()\"]"));
            }
            this._driver.FindElement(By.CssSelector("[ng-click=\"user.signOut()\"]")).Click();

            this._generalMethods.WaitForElement(this._driver, By.Name("username"));

            log.Debug("Exit Log out Web Driver ");
        }

        #endregion Login / Logout

        #region Roster

        public String GetRosterHouseholdName()
        {
            this._generalMethods.WaitForElementDisplayed(By.XPath("//div[@class='scroll']"));
            String titleName =this._driver.FindElementByXPath("//div[@class='scroll']//span").Text.ToString();
            return titleName;
        }

        public String[] GetRosterIndividuals()
        {
            this._generalMethods.WaitForElementDisplayed(By.XPath("//div[@class='scroll']"));
            IWebElement[] itemListElement = this._driver.FindElements(By.XPath("//ion-item[@ng-repeat='person in individuals | IndividualsFilter']")).ToArray<IWebElement>();
            int count = itemListElement.Length;
            String[] individualsArray = new String[count];
            IWebElement itemSpan;

            for (int i = 0; i < count; i++)
            {
                try
                {
                    itemSpan = itemListElement[i].FindElement(By.CssSelector("div[class=\"name-wrapper\"]"));

                    ((IJavaScriptExecutor)this._driver).ExecuteScript("arguments[0].scrollIntoView();", itemSpan);
                    individualsArray[i] = itemSpan.Text.ToString().Replace("G\r\n", "");
                    TestLog.WriteLine("Getting individual: " + individualsArray[i] + ";");
                }
                catch (WebDriverException e)
                {
                    TestLog.WriteLine("Fail to get individual name on index " + i);
                }
            }          
            
            return individualsArray;
        }
        public int GetIndividualIndexByName(String individualName)
        {
            TestLog.WriteLine("Finding index of individual: " + individualName + ";");
            this._generalMethods.WaitForElementDisplayed(By.XPath("//div[@class='scroll']"));
            
            IWebElement[] roster = this._driver.FindElements(By.XPath("//ion-item[@ng-repeat='person in individuals | IndividualsFilter']")).ToArray<IWebElement>();
            int count = roster.Length;
            String pageIndividualName = "";
            int index = -1;
            IWebElement itemSpan;

            for (int i = 0; i < count; i++)
            {
                try
                {
                    itemSpan = roster[i].FindElement(By.CssSelector("div[class=\"name-wrapper\"]"));
                    ((IJavaScriptExecutor)this._driver).ExecuteScript("arguments[0].scrollIntoView();", itemSpan);
                    pageIndividualName = itemSpan.Text.ToString();
                    pageIndividualName = pageIndividualName.Replace("G\r\n", "");
                    TestLog.WriteLine("Checking individual: " + pageIndividualName + ";");
                    if (pageIndividualName.Equals(individualName))
                    {

                        TestLog.WriteLine("Found individual: " + pageIndividualName + ";");
                        TestLog.WriteLine("Found index: " + i + ";");
                        return i;
                    }
                }
                catch (WebDriverException e)
                {
                    TestLog.WriteLine("Fail to get individual name on index " + i);
                }
            }
            
            TestLog.WriteLine("index=" + -1);
            return -1;
        }

        public void ViewIndividualDetailByName(String individualName)
        {
            int index = GetIndividualIndexByName(individualName);
            if (index == -1)
            {
                TestLog.WriteLine("Fail to find expected individual in roster: " + individualName);
                Assert.Fail();
            }
            
            TestLog.WriteLine("The individual expected is at the index: " + ++index);

            this._driver.FindElementByXPath(String.Format("//ion-item[{0}]//i[@class='fa fa-angle-right']", index)).Click();
        }

        public void ClickGoLeftButton()
        {
            string xpath = "//i[@class='fa fa-angle-left']";

            this._generalMethods.WaitForElement(By.XPath(xpath));
            IWebElement[] ele = this._driver.FindElementsByXPath(xpath).ToArray();
            TestLog.WriteLine("Find Go back button count: " + ele.Length);

            for (int index = 0; index < ele.Length; index++)
            {
                TestLog.WriteLine("Try Go back button with index: " + index);
                try
                {
                    ele[index].Click();
                    break;
                 }
                 catch (WebDriverException) 
                {
                    TestLog.WriteLine("Warning: fail to found or click on Go back link. Try to click another element");
                }

            }

        }

        public void ClickEditButtonOnViewPage()
        {
            string xpath = "//div[@class='edit-button']";
            this._generalMethods.WaitForElement(this._driver, By.XPath(xpath));
            this._driver.FindElementByXPath(xpath).Click();

        }

        public void ClickDoneButtonOnEditPage()
        {
            string xpath = "//div[@class='save-button']";
            this._generalMethods.WaitForElement(this._driver, By.XPath(xpath));
            this._driver.FindElementByXPath(xpath).Click();

        }

        public void ClickToAddMobileContact()
        {
            string xpath = "//div[@class='detail-info']/ul[1]/li[1]/div";
            this._generalMethods.WaitForElementDisplayed(By.XPath(xpath));
            IWebElement ele = this._driver.FindElementByXPath(xpath);
            Assert.IsTrue("Add mobile contact...".Equals(ele.Text), "Add mobile contact link string is wrong");
            ele.Click();
        }

        public void ClickToAddHomeContact()
        {
            string xpath = "//div[@class='detail-info']/ul[1]/li[2]/div";
            this._generalMethods.WaitForElementDisplayed(By.XPath(xpath));
            IWebElement ele = this._driver.FindElementByXPath(xpath);
            Assert.IsTrue("Add home contact...".Equals(ele.Text), "Add home contact link string is wrong");
            ele.Click();
        }

        public void ClickToAddEmergenyContact()
        {
            string xpath = "//div[@class='detail-info']/ul[1]/li[3]/div";
            this._generalMethods.WaitForElementDisplayed(By.XPath(xpath));
            IWebElement ele = this._driver.FindElementByXPath(xpath);
            Assert.IsTrue("Add emergency contact...".Equals(ele.Text), "Add emergency contact link string is wrong");
            ele.Click();
        }

        public void ClickToDelete1stInfo()
        {
            string xpath = "//div[@class='detail-info']/ul[2]/li[1]/div/div";
            this._generalMethods.WaitForElementDisplayed(By.XPath(xpath));
            this._driver.FindElementByXPath(xpath).Click();
        }

        public void ClickToDelete2ndInfo()
        {
            string xpath = "//div[@class='detail-info']/ul[2]/li[2]/div/div";
            this._generalMethods.WaitForElementDisplayed(By.XPath(xpath));
            this._driver.FindElementByXPath(xpath).Click();
        }

        public void ClickToAdd1stInfo()
        {
            string xpath = "//div[@class='detail-info']/ul[2]/li[1]/div";
            this._generalMethods.WaitForElementDisplayed(By.XPath(xpath));
            this._driver.FindElementByXPath(xpath).Click();
        }

        public void ClickToAdd2ndInfo()
        {
            string xpath = "//div[@class='detail-info']/ul[2]/li[2]/div";
            this._generalMethods.WaitForElementDisplayed(By.XPath(xpath));
            this._driver.FindElementByXPath(xpath).Click();
        }

        public void InputNameOnEditPage(string firstName, string goesBy, string lastName)
        {
            IWebElement ele;

            if (null != firstName)
            {
                ele = this._driver.FindElementByCssSelector("input[ng-model=\"profile.firstName\"]");
                ele.Clear();
                ele.SendKeys(firstName);
            }

            if (null != goesBy)
            {
                ele = this._driver.FindElementByCssSelector("input[ng-model=\"profile.goesBy\"]");
                ele.Clear();
                ele.SendKeys(goesBy);
            }

            if (null != lastName)
            {
                ele = this._driver.FindElementByCssSelector("input[ng-model=\"profile.lastName\"]");
                ele.Clear();
                ele.SendKeys(lastName);
            }
        }

        public void InputPhoneOnEditPage(string mobileP, string homeP, string emergencyP)
        {
            IWebElement ele;

            if (null != mobileP)
            {
                ele = this._driver.FindElementByCssSelector("input[ng-model=\"profile.individualMobilePhone\"]");
                ele.Clear();
                ele.SendKeys(mobileP);
            }

            if (null != homeP)
            {
                ele = this._driver.FindElementByCssSelector("input[ng-model=\"profile.householdPhone\"]");
                ele.Clear();
                ele.SendKeys(homeP);
            }

            if (null != emergencyP)
            {
                ele = this._driver.FindElementByCssSelector("input[ng-model=\"profile.householdEmergencyPhone\"]");
                ele.Clear();
                ele.SendKeys(emergencyP);
            }

        }

        public void InputSpecialInfoOnEditPage(string special1, bool isPer1, string special2, bool isPer2)
        {
            IWebElement ele;

            if (null != special1)
            {
                ele = this._driver.FindElementByCssSelector("textarea[ng-model=\"profile.instruction1\"]");
                ele.Clear();
                ele.SendKeys(special1);

                if (isPer1)
                {
                    ele = this._driver.FindElementByCssSelector("label[for=\"instruction1-1\"]");
                }
                else
                {
                    ele = this._driver.FindElementByCssSelector("label[for=\"instruction1-2\"]");
                }
                ele.Click();
            }

            if (null != special2)
            {
                ele = this._driver.FindElementByCssSelector("textarea[ng-model=\"profile.instruction2\"]");
                ele.Clear();
                ele.SendKeys(special2);

                if (isPer2)
                {
                    ele = this._driver.FindElementByCssSelector("label[for=\"instruction2-1\"]");
                }
                else
                {
                    ele = this._driver.FindElementByCssSelector("label[for=\"instruction2-2\"]");
                }
                ele.Click();
            }
        }

        public string[] GetIndividualDetailsOnEidt()
        {
            this._generalMethods.WaitForElementDisplayed(By.XPath("//div[@class='read-info']/div/span"));
            this._generalMethods.WaitForElementDisplayed(By.XPath(".//div[@class='sub-form'][1]/input[1]"));

            string[] details = new string[10];
            IWebElement ele;
            int index = 0;

            // Get name
            ele = this._driver.FindElementByXPath(".//div[@class='sub-form'][1]/input[1]");
            TestLog.WriteLine("Individual value of index: " + index + ", is " + ele.GetAttribute("value"));
            Assert.IsFalse("".Equals(ele.GetAttribute("value").Trim()), "First name of individual should not be retrieved as blank.");
            details[index++] = ele.GetAttribute("value");
            ele = this._driver.FindElementByXPath(".//div[@class='sub-form'][1]/input[2]");
            TestLog.WriteLine("Individual value of index: " + index + ", is " + ele.GetAttribute("value"));
            details[index++] = ele.GetAttribute("value");
            ele = this._driver.FindElementByXPath(".//div[@class='sub-form'][1]/input[3]");
            TestLog.WriteLine("Individual value of index: " + index + ", is " + ele.GetAttribute("value"));
            details[index++] = ele.GetAttribute("value");

            // Get phone
            ele = this._driver.FindElementByXPath(".//div[@class='sub-form'][2]/input[1]");
            TestLog.WriteLine("Individual value of index: " + index + ", is " + ele.GetAttribute("value"));
            details[index++] = ele.GetAttribute("value");
            ele = this._driver.FindElementByXPath(".//div[@class='sub-form'][2]/input[2]");
            TestLog.WriteLine("Individual value of index: " + index + ", is " + ele.GetAttribute("value"));
            details[index++] = ele.GetAttribute("value");
            ele = this._driver.FindElementByXPath(".//div[@class='sub-form'][2]/input[3]");
            TestLog.WriteLine("Individual value of index: " + index + ", is " + ele.GetAttribute("value"));
            details[index++] = ele.GetAttribute("value");

            // Get special info
            ele = this._driver.FindElementByXPath("//textarea[1]");
            TestLog.WriteLine("Individual value of index: " + index + ", is " + ele.GetAttribute("value"));
            details[index++] = ele.GetAttribute("value");
            ele = this._driver.FindElementByXPath(".//label[@for='instruction1-1']");
            if ("radio-label selected".Equals(ele.GetAttribute("class")))
                details[index] = "true";
            else
                details[index] = "false";
            index++;
            ele = this._driver.FindElementByXPath("//textarea[2]");
            TestLog.WriteLine("Individual value of index: " + index + ", is " + ele.GetAttribute("value"));
            details[index++] = ele.GetAttribute("value");
            ele = this._driver.FindElementByXPath(".//label[@for='instruction2-1']");
            if ("radio-label selected".Equals(ele.GetAttribute("class")))
                details[index] = "true";
            else
                details[index] = "false";

            return details;
        }

        public String GetGuestSignByIndividualName(String individualName)
        {
            String guestAssign = "";
            int index = GetIndividualIndexByName(individualName);
            IWebElement itemListElement = this._driver.FindElementByXPath("//div[@class='scroll']").FindElement(By.CssSelector("ion-list[class =\"disable-user-behavior\"]")).FindElement(By.ClassName("list"));
            try
            {
                IWebElement guestElement = itemListElement.FindElements(By.CssSelector("ion-item[ng-repeat=\"person in individuals | IndividualsFilter\"]"))[index].FindElement(By.ClassName("roster-item")).FindElement(By.ClassName("name-info")).FindElement(By.ClassName("guest"));
                guestAssign = guestElement.Text.ToString();
            }
            catch (WebDriverException e)
            {
                guestAssign = "";
            }
            return guestAssign;
        }
        public bool IslinkToDetailByIndividualName(String individualName)
        {
            bool flag = false;
            int index = GetIndividualIndexByName(individualName);
            IWebElement itemListElement = this._driver.FindElementByXPath("//div[@class='scroll']").FindElement(By.CssSelector("ion-list[class =\"disable-user-behavior\"]")).FindElement(By.ClassName("list"));
            try
            {
                IWebElement linkDetailElement = itemListElement.FindElements(By.CssSelector("ion-item[ng-repeat=\"person in individuals | IndividualsFilter\"]"))[index].FindElement(By.ClassName("roster-item")).FindElement(By.ClassName("name-info")).FindElement(By.ClassName("action")).FindElement(By.CssSelector("[ng-click=\"linkToDetail()\"]"));
                flag = true;
            }
            catch (WebDriverException e)
            {
                flag = false;
            }
            return flag;
        }

        #endregion Roster

        #region Assignments
        
        public int GetAssignmentIndexByName(String individualName, String activityName)
        {
            TestLog.WriteLine("Finding assignment of individual: " + individualName + ": " + activityName);
            int index = GetIndividualIndexByName(individualName);
            IWebElement[] itemListElement = this._driver.FindElements(By.XPath("//ion-item[@ng-repeat='person in individuals | IndividualsFilter']")).ToArray<IWebElement>();
            IWebElement assignmentsListElement = itemListElement[index].FindElement(By.ClassName("roster-item")).FindElement(By.ClassName("assignments-wrapper")).FindElement(By.ClassName("list"));
            int assignmentsCount = assignmentsListElement.FindElements(By.TagName("ion-item")).Count;
            int assignmentIndex = -1;

            for (int i = 0; i < assignmentsCount; i++)
            {

                try
                {
                    IWebElement itemSpan1 = itemListElement[index].FindElements(By.TagName("ion-item"))[i].FindElement(By.ClassName("assignment-title")).FindElements(By.TagName("span"))[0];
                    String pageActivityName = itemSpan1.Text.ToString();
                    TestLog.WriteLine("Checking assignment: " + pageActivityName);
                    if (pageActivityName.Equals(activityName + " - "))
                    {
                        TestLog.WriteLine("Found assignment: " + pageActivityName);
                        assignmentIndex = i;
                        break;
                    }
                }
                catch (WebDriverException e)
                {
                    TestLog.WriteLine("Fail to find individual on index of " + i);
                }
            }
            
            TestLog.WriteLine("assignmentIndex=" + assignmentIndex);
            return assignmentIndex;
        }

        public String GetAssignmentTitleByIndividualNameandActivity(String individualName, String activityName)
        {           
            int index = GetIndividualIndexByName(individualName);
            IWebElement itemListElement = this._driver.FindElementByXPath("//div[@class='scroll']").FindElement(By.CssSelector("ion-list[class =\"disable-user-behavior\"]")).FindElement(By.ClassName("list"));
            IWebElement assignmentsListElement = itemListElement.FindElements(By.CssSelector("ion-item[ng-repeat=\"person in individuals | IndividualsFilter\"]"))[index].FindElement(By.ClassName("roster-item")).FindElement(By.ClassName("assignments-wrapper")).FindElement(By.ClassName("list"));
            int assignmentIndex = GetAssignmentIndexByName(individualName, activityName);
            String pageAssignActivityName = assignmentsListElement.FindElements(By.TagName("ion-item"))[assignmentIndex].FindElement(By.ClassName("assignment-info")).FindElement(By.ClassName("assignment-title")).FindElements(By.TagName("span"))[0].Text.ToString();
            String pageAssignActivityRoster = assignmentsListElement.FindElements(By.TagName("ion-item"))[assignmentIndex].FindElement(By.ClassName("assignment-info")).FindElement(By.ClassName("assignment-title")).FindElements(By.TagName("span"))[1].Text.ToString();
            String pageAssignmentTitle = pageAssignActivityName + pageAssignActivityRoster;
            return pageAssignmentTitle;
        }
        public String GetAssignmentSignByIndividualNameandActivity(String individualName, String activityName)
        {
            String assignmentSign = "";
            int index = GetIndividualIndexByName(individualName);
            IWebElement itemListElement = this._driver.FindElementByXPath("//div[@class='scroll']").FindElement(By.CssSelector("ion-list[class =\"disable-user-behavior\"]")).FindElement(By.ClassName("list"));
            IWebElement assignmentsListElement = itemListElement.FindElements(By.CssSelector("ion-item[ng-repeat=\"person in individuals | IndividualsFilter\"]"))[index].FindElement(By.ClassName("roster-item")).FindElement(By.ClassName("assignments-wrapper")).FindElement(By.ClassName("list"));
            int assignmentIndex = GetAssignmentIndexByName(individualName, activityName);
            try
            {
                IWebElement volunteerSignElement = assignmentsListElement.FindElements(By.TagName("ion-item"))[assignmentIndex].FindElement(By.ClassName("assignment-info")).FindElement(By.ClassName("assignment-title")).FindElement(By.ClassName("volunteer"));
                assignmentSign = volunteerSignElement.Text.ToString();
            }
            catch (WebDriverException e)
            {
                assignmentSign = "";
            }
            return assignmentSign;
        }
        public String GetAssignmentTimeByIndividualNameandActivity(String individualName, String activityName)
        {
            String time = "";
            int index = GetIndividualIndexByName(individualName);
            IWebElement itemListElement = this._driver.FindElementByXPath("//div[@class='scroll']").FindElement(By.CssSelector("ion-list[class =\"disable-user-behavior\"]")).FindElement(By.ClassName("list"));
            IWebElement assignmentsListElement = itemListElement.FindElements(By.CssSelector("ion-item[ng-repeat=\"person in individuals | IndividualsFilter\"]"))[index].FindElement(By.ClassName("roster-item")).FindElement(By.ClassName("assignments-wrapper")).FindElement(By.ClassName("list"));
            int assignmentIndex = GetAssignmentIndexByName(individualName, activityName);
            try
            {
                IWebElement assignmentTimeElement = assignmentsListElement.FindElements(By.TagName("ion-item"))[assignmentIndex].FindElement(By.ClassName("assignment-info")).FindElement(By.CssSelector("p[class=\"time ng-binding\"]"));
                time = assignmentTimeElement.Text.ToString();
            }
            catch (WebDriverException e)
            {
                time = "";
            }
            return time;
        }
        public String GetExpectAssignmentTimeByActivityTime(int churchId, String activityName)
        {
            
            DateTime tempDate = Convert.ToDateTime(this._sql.Ministry_Activities_FetchCreatedDate(churchId, activityName));
            TestLog.WriteLine("-tempDate = {0}", tempDate);

            string[] createdDateArray = this._sql.Ministry_Activities_FetchCreatedDate(churchId, activityName).Split(' ');
            string weekX = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day).DayOfWeek.ToString();
            TestLog.WriteLine("-weekX = {0}", weekX);

            string[] startTimeArray = this._sql.Ministry_Activities_FetchCreatedDate(churchId, activityName).Split(' ');
            int startHour = Convert.ToInt32(startTimeArray[1].Split(':')[0]);

            String assignPM = startTimeArray[2];

            //if (startHour > 12)
            //{
            //    assignPM = "PM";
            //    startHour = startHour - 12;
            //}
           //else
            //{
            //    assignPM = "AM";
            //}
            String expectAssignmentTime = weekX + " " + Convert.ToString(startHour) + ":" + startTimeArray[1].Split(':')[1] + " " + assignPM;
            TestLog.WriteLine("-expectAssignmentTime = {0}", expectAssignmentTime);
            return expectAssignmentTime;
        }
        public String GetNoAssignmentInfoByIndividualName(String individualName)
        {
            String noAssignmentInfo = "";
            int index = GetIndividualIndexByName(individualName);
            
            try
            {
                IWebElement noAssignmentElement = this._driver.FindElements(By.XPath("//ion-item[@ng-repeat='person in individuals | IndividualsFilter']"))[index].FindElement(By.CssSelector("[ng-if=\"!assignments.length\"]"));
                noAssignmentInfo = noAssignmentElement.Text.ToString();
            }
            catch (WebDriverException)
            {
            }
            return noAssignmentInfo;
        }
        
        #endregion Assignments

        #region Checkin

        public bool IsHaveNext()
        {
            bool flag = false;
            try
            {
                this._generalMethods.WaitForElement(By.CssSelector("button[class=\"button next-button\"][ng-click=\"checkin()\"]"));
                IWebElement nextElement = this._driver.FindElement(By.CssSelector("button[class=\"button next-button\"][ng-click=\"checkin()\"]"));
                flag = true;
            }
            catch (WebDriverException e)
            {
                flag = false;
                TestLog.WriteLine("No Next to click!Please slect a assignment for check in!");
            }
            return flag;
        }
        public void ClickNextAndWaitForLoaded()
        {
            if (IsHaveNext())
            {
                this._generalMethods.WaitForElement(By.CssSelector("button[class=\"button next-button\"][ng-click=\"checkin()\"]"));
                IWebElement nextElement = this._driver.FindElement(By.CssSelector("button[class=\"button next-button\"][ng-click=\"checkin()\"]"));
                nextElement.Click();
            }
            else
            {
                TestLog.WriteLine("No Next to click!Please slect a assignment for check in!");
            }

            this._generalMethods.WaitForElementNotDisplayed(By.CssSelector("button[class=\"button next-button\"][ng-click=\"checkin()\"]"));
            this._generalMethods.WaitForElementEnabled(By.CssSelector("button[class=\"button next-button\"][ng-click=\"next()\"]"));
            this._generalMethods.WaitForElementVisible(By.CssSelector("button[class=\"button next-button\"][ng-click=\"next()\"]"));
            this._generalMethods.WaitForElementDisplayed(By.CssSelector("button[class=\"button next-button\"][ng-click=\"next()\"]"));
        }
        

        public bool IsCanCheckInByIndividualNameandActivity(String individualName, String activityName)
        {
            bool flag = true;
            int index = GetIndividualIndexByName(individualName);

            IWebElement assignmentsListElement = this._driver.FindElements(By.XPath("//ion-item[@ng-repeat='person in individuals | IndividualsFilter']"))[index].FindElement(By.ClassName("assignments-wrapper")).FindElement(By.ClassName("list"));
            int assignmentIndex = GetAssignmentIndexByName(individualName, activityName);
            try
            {
                IWebElement inputCheckBoxElement = assignmentsListElement.FindElements(By.TagName("ion-item"))[assignmentIndex].FindElement(By.ClassName("checkbox-wrapper")).FindElement(By.CssSelector("input[type=\"checkbox\"]"));
                if (inputCheckBoxElement.Selected.Equals(false))
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }

            }
            catch (WebDriverException e)
            {
                flag = false;
            }
            return flag;
        }
        public void CheckOnAssignment(String individualName, String activityName)
        {
           
            int index = GetIndividualIndexByName(individualName);

            IWebElement assignmentsListElement = this._driver.FindElements(By.XPath("//ion-item[@ng-repeat='person in individuals | IndividualsFilter']"))[index].FindElement(By.ClassName("assignments-wrapper")).FindElement(By.ClassName("list"));
            int assignmentIndex = GetAssignmentIndexByName(individualName, activityName);

            if (IsCanCheckInByIndividualNameandActivity(individualName, activityName))
            {
                assignmentsListElement.FindElements(By.TagName("ion-item"))[assignmentIndex].FindElement(By.ClassName("details")).Click();
            }
      
        }
        
        #endregion Checkin

        #region Item Tag

        public String GetItemTagTitle()
        {
            this._generalMethods.WaitForElementDisplayed(By.XPath("//div[@class='itemtag-container pane']"));
            String titleName = this._driver.FindElementByXPath("//div[@class='itemtag-container pane']").FindElement(By.CssSelector("[class=\"main-content scroll-content ionic-scroll\"]")).FindElement(By.ClassName("scroll")).FindElement(By.ClassName("title")).Text.ToString();
            return titleName;
        }

        public int GetCheckedIndividualIndexByName(String individualName)
        {
            TestLog.WriteLine("Finding checked individual: " + individualName);
            IWebElement[] itemListElement = this._driver.FindElements(By.XPath("//ion-item[@ng-repeat='person in checkedIndividuals | IndividualsFilter']")).ToArray<IWebElement>();
            int count = itemListElement.Length;
            String pageIndividualName = "";

            IWebElement itemNameP;

            for (int i = 0; i < count; i++)
            {
                try
                {
                    itemNameP = itemListElement[i].FindElement(By.CssSelector("p[class=\"name ng-binding\"]"));
                    ((IJavaScriptExecutor)this._driver).ExecuteScript("arguments[0].scrollIntoView();", itemNameP);
                    pageIndividualName = itemNameP.Text.ToString();
                    TestLog.WriteLine("Checking individual: " + pageIndividualName + ";");
                    if (pageIndividualName.Equals(individualName))
                    {
                        TestLog.WriteLine("Found individual: " + pageIndividualName + ";");
                        return i;
                    }
                }
                catch (WebDriverException e)
                {
                    TestLog.WriteLine("Fail to find checked individual on index: " + i);
                }
            }

            
            TestLog.WriteLine("index=" + -1);
            return -1;
        }
        public void AddItemTagNumberByIndividualName(String individualName, int count)
        {
            int index = GetCheckedIndividualIndexByName(individualName);
            IWebElement itemListElement = this._driver.FindElementByXPath("//div[@class='itemtag-container pane']").FindElement(By.CssSelector("[class=\"main-content scroll-content ionic-scroll\"]")).FindElement(By.ClassName("scroll")).FindElement(By.CssSelector("ion-list[class =\"disable-user-behavior\"]")).FindElement(By.ClassName("list"));
            try
            {
                IWebElement incElement = itemListElement.FindElements(By.CssSelector("ion-item[ng-repeat=\"person in checkedIndividuals | IndividualsFilter\"]"))[index].FindElement(By.ClassName("tag-item")).FindElement(By.ClassName("spinner-wrapper")).FindElement(By.CssSelector("div[ng-click=\"inc()\"]"));
                IWebElement decElement = itemListElement.FindElements(By.CssSelector("ion-item[ng-repeat=\"person in checkedIndividuals | IndividualsFilter\"]"))[index].FindElement(By.ClassName("tag-item")).FindElement(By.ClassName("spinner-wrapper")).FindElement(By.CssSelector("div[ng-click=\"dec()\"]"));
                for (int i = 0; i < 3; i++)
                {
                    decElement.Click();
                }
                for (int i = 0; i < count; i++)
                {
                    incElement.Click();
                }

            }
            catch (WebDriverException e)
            {

            }
        }
        public String GetItemTagNumberByIndividualName(String individualName)
        {
            String itemNubmer = "";
            int index = GetCheckedIndividualIndexByName(individualName);
            IWebElement itemListElement = this._driver.FindElementByXPath("//div[@class='itemtag-container pane']").FindElement(By.CssSelector("[class=\"main-content scroll-content ionic-scroll\"]")).FindElement(By.ClassName("scroll")).FindElement(By.CssSelector("ion-list[class =\"disable-user-behavior\"]")).FindElement(By.ClassName("list"));
            try
            {
                IWebElement itemValueElement = itemListElement.FindElements(By.CssSelector("ion-item[ng-repeat=\"person in checkedIndividuals | IndividualsFilter\"]"))[index].FindElement(By.ClassName("tag-item")).FindElement(By.ClassName("spinner-wrapper")).FindElement(By.CssSelector("div[class=\"val ng-binding\"]"));
                itemNubmer = itemValueElement.Text.ToString();

            }
            catch (WebDriverException e)
            {
                itemNubmer = "";
            }
            return itemNubmer;
        }
        public void ClickFinishAndWaitForLoaded()
        {
            if (IsHaveNext())
            {
                IWebElement nextElement = this._driver.FindElement(By.CssSelector("button[class=\"button next-button\"][ng-click=\"next()\"]"));
                nextElement.Click();
            }
            else
            {
                TestLog.WriteLine("No Finish to click on item tag page!");
            }

            this._generalMethods.WaitForElementNotDisplayed(By.CssSelector("button[class=\"button next-button\"][ng-click=\"next()\"]"));
            this._generalMethods.WaitForElementEnabled(By.CssSelector("button[class=\"button next-button\"][ng-click=\"linkToAssignments()\"]"));
            this._generalMethods.WaitForElementVisible(By.CssSelector("button[class=\"button next-button\"][ng-click=\"linkToAssignments()\"]"));
            this._generalMethods.WaitForElementDisplayed(By.CssSelector("button[class=\"button next-button\"][ng-click=\"linkToAssignments()\"]"));
        }

        public void ClickDone()
        {
            if (IsHaveNext())
            {
                IWebElement nextElement = this._driver.FindElement(By.CssSelector("button[class=\"button next-button\"][ng-click=\"linkToAssignments()\"]"));
                nextElement.Click();
            }
            else
            {
                TestLog.WriteLine("No Finish to click on item tag page!");
            }

            this._generalMethods.WaitForElementNotDisplayed(By.CssSelector("button[class=\"button next-button\"][ng-click=\"linkToAssignments()\"]"));
            this.WaitForAssignmentLoaded();
        }
        
        #endregion Item Tag

        #region Confirmation Page
        public String GetConfirmationTitle()
        {
            this._generalMethods.WaitForElementDisplayed(By.XPath("//div[@class='confirmation-view pane']"));
            String titleName = this._driver.FindElementByXPath("//div[@class='confirmation-view pane']").FindElement(By.CssSelector("[class=\"main-content scroll-content ionic-scroll\"]")).FindElement(By.ClassName("scroll")).FindElement(By.ClassName("title")).Text.ToString();
            return titleName;
        }

        public int GetConfirmedIndividualIndexByName(String individualName)
        {
            TestLog.WriteLine("Finding individual on Confirmed page: " + individualName);
            IWebElement[] ele = this._driver.FindElements(By.XPath("//div[@class='confirmation-view pane']//ion-item[@ng-repeat='person in checkedIndividuals | IndividualsFilter']")).ToArray<IWebElement>();
            int count = ele.Length;
            String pageIndividualName = "";

            IWebElement itemNamespan;

            for (int i = 0; i < count; i++)
            {
                try
                {
                    itemNamespan = ele[i].FindElement(By.CssSelector("div[class=\"name ng-binding\"]"));
                    ((IJavaScriptExecutor)this._driver).ExecuteScript("arguments[0].scrollIntoView();", itemNamespan);
                    pageIndividualName = itemNamespan.Text.ToString();
                    TestLog.WriteLine("Checking individual: " + pageIndividualName + ";");
                    if (pageIndividualName.Equals(individualName))
                    {

                        TestLog.WriteLine("Found individual: " + individualName);
                        return i;
                    }
                }
                catch (WebDriverException e)
                {
                    TestLog.WriteLine("Fail to get individual name on index " + i);
                }
            }

            TestLog.WriteLine("index=" + -1);
            return -1;
        }
        public int GetConfirmedAssignmentIndexByName(String individualName, String activityName)
        {
            int index = GetConfirmedIndividualIndexByName(individualName);
            IWebElement[] itemListElement = this._driver.FindElementsByXPath("//div[@class='confirmation-view pane']//ion-item[@ng-repeat='person in checkedIndividuals | IndividualsFilter']").ToArray<IWebElement>();
            IWebElement assignmentsListElement = itemListElement[index].FindElement(By.CssSelector("div[class=\"roster-item view-mode\"]")).FindElement(By.ClassName("assignments-wrapper")).FindElement(By.ClassName("list"));           
            
            int assignmentsCount = assignmentsListElement.FindElements(By.TagName("ion-item")).Count;
            int assignmentIndex = -1;
            if (assignmentsCount > 0)
            {
                for (int i = 0; i < assignmentsCount; i++)
                {

                    try
                    {
                        IWebElement itemSpan1 = itemListElement[index].FindElement(By.CssSelector("div[class=\"roster-item view-mode\"]")).FindElement(By.ClassName("assignments-wrapper")).FindElement(By.ClassName("list")).FindElements(By.TagName("ion-item"))[i].FindElement(By.ClassName("assignment-title")).FindElements(By.TagName("span"))[0];
                        String pageActivityName = itemSpan1.Text.ToString();
                        if (pageActivityName.Equals(activityName + " - "))
                        {
                            assignmentIndex = i;
                            break;
                        }
                    }
                    catch (WebDriverException e)
                    {
                        TestLog.WriteLine("Fail to get individual name on index " + i);
                    }
                }
            }
            TestLog.WriteLine("assignmentIndex=" + assignmentIndex);
            return assignmentIndex;
        }

        public String GetConfirmedItemTagInfoByIndividualName(String individualName)
        {
            String itemTagValue = "";
            int index = GetConfirmedIndividualIndexByName(individualName);
            TestLog.WriteLine("confirmIndex=" + index);
            IWebElement itemListElement = this._driver.FindElementByXPath("//div[@class='confirmation-view pane']").FindElement(By.CssSelector("[class=\"main-content scroll-content ionic-scroll\"]")).FindElement(By.ClassName("scroll")).FindElement(By.CssSelector("ion-list[class =\"disable-user-behavior\"]")).FindElement(By.ClassName("list"));

            try
            {
                IWebElement itemValueSpanElement = itemListElement.FindElements(By.CssSelector("ion-item[ng-repeat=\"person in checkedIndividuals | IndividualsFilter\"]"))[index].FindElement(By.CssSelector("div[class=\"roster-item view-mode\"]")).FindElement(By.ClassName("name-info")).FindElement(By.ClassName("action")).FindElement(By.TagName("span"));
               
                itemTagValue = itemValueSpanElement.Text.ToString();

            }
            catch (WebDriverException e)
            {
                itemTagValue = "";
            }
            return itemTagValue;
        }
        public String GetConfirmedGuestSignByIndividualName(String individualName)
        {
            String guestAssign = "";
            int index = GetConfirmedIndividualIndexByName(individualName);
            IWebElement itemListElement = this._driver.FindElementByXPath("//div[@class='confirmation-view pane']").FindElement(By.CssSelector("[class=\"main-content scroll-content ionic-scroll\"]")).FindElement(By.ClassName("scroll")).FindElement(By.CssSelector("ion-list[class =\"disable-user-behavior\"]")).FindElement(By.ClassName("list"));
            
            try
            {
                IWebElement guestElement = itemListElement.FindElements(By.CssSelector("ion-item[ng-repeat=\"person in checkedIndividuals | IndividualsFilter\"]"))[index].FindElement(By.CssSelector("div[class=\"roster-item view-mode\"]")).FindElement(By.ClassName("name-info")).FindElement(By.ClassName("guest"));
                
                guestAssign = guestElement.Text.ToString();
            }
            catch (WebDriverException e)
            {
                guestAssign = "";
            }
            return guestAssign;
        }

        public String GetConfirmedAssignmentTitleByIndividualNameandActivity(String individualName, String activityName)
        {
            int index = GetConfirmedIndividualIndexByName(individualName);
            TestLog.WriteLine("index of individual" + individualName + ": " + index);
            IWebElement itemListElement = this._driver.FindElementByXPath("//div[@class='confirmation-view pane']").FindElement(By.CssSelector("[class=\"main-content scroll-content ionic-scroll\"]")).FindElement(By.ClassName("scroll")).FindElement(By.CssSelector("ion-list[class =\"disable-user-behavior\"]")).FindElement(By.ClassName("list"));
            IWebElement assignmentsListElement = itemListElement.FindElements(By.CssSelector("ion-item[ng-repeat=\"person in checkedIndividuals | IndividualsFilter\"]"))[index].FindElement(By.CssSelector("div[class=\"roster-item view-mode\"]")).FindElement(By.ClassName("assignments-wrapper")).FindElement(By.ClassName("list"));

            int assignmentIndex = GetConfirmedAssignmentIndexByName(individualName, activityName);
            String pageAssignActivityName = assignmentsListElement.FindElements(By.TagName("ion-item"))[assignmentIndex].FindElement(By.ClassName("assignment-info")).FindElement(By.ClassName("assignment-title")).FindElements(By.TagName("span"))[0].Text.ToString();
            String pageAssignActivityRoster = assignmentsListElement.FindElements(By.TagName("ion-item"))[assignmentIndex].FindElement(By.ClassName("assignment-info")).FindElement(By.ClassName("assignment-title")).FindElements(By.TagName("span"))[1].Text.ToString();
            String pageAssignmentTitle = pageAssignActivityName + pageAssignActivityRoster;
            return pageAssignmentTitle;
        }
        public String GetConfirmedAssignmentSignByIndividualNameandActivity(String individualName, String activityName)
        {
            String assignmentSign = "";
            int index = GetConfirmedIndividualIndexByName(individualName);
            IWebElement itemListElement = this._driver.FindElementByXPath("//div[@class='confirmation-view pane']").FindElement(By.CssSelector("[class=\"main-content scroll-content ionic-scroll\"]")).FindElement(By.ClassName("scroll")).FindElement(By.CssSelector("ion-list[class =\"disable-user-behavior\"]")).FindElement(By.ClassName("list"));
            IWebElement assignmentsListElement = itemListElement.FindElements(By.CssSelector("ion-item[ng-repeat=\"person in checkedIndividuals | IndividualsFilter\"]"))[index].FindElement(By.CssSelector("div[class=\"roster-item view-mode\"]")).FindElement(By.ClassName("assignments-wrapper")).FindElement(By.ClassName("list"));
            
            int assignmentIndex = GetConfirmedAssignmentIndexByName(individualName, activityName);
            try
            {
                IWebElement volunteerSignElement = assignmentsListElement.FindElements(By.TagName("ion-item"))[assignmentIndex].FindElement(By.ClassName("assignment-info")).FindElement(By.ClassName("assignment-title")).FindElement(By.ClassName("volunteer"));
                assignmentSign = volunteerSignElement.Text.ToString();
            }
            catch (WebDriverException e)
            {
                assignmentSign = "";
            }
            return assignmentSign;
        }
        public String GetConfirmedAssignmentTimeByIndividualNameandActivity(String individualName, String activityName)
        {
            String time = "";
            int index = GetConfirmedIndividualIndexByName(individualName);
            IWebElement itemListElement = this._driver.FindElementByXPath("//div[@class='confirmation-view pane']").FindElement(By.CssSelector("[class=\"main-content scroll-content ionic-scroll\"]")).FindElement(By.ClassName("scroll")).FindElement(By.CssSelector("ion-list[class =\"disable-user-behavior\"]")).FindElement(By.ClassName("list"));
            IWebElement assignmentsListElement = itemListElement.FindElements(By.CssSelector("ion-item[ng-repeat=\"person in checkedIndividuals | IndividualsFilter\"]"))[index].FindElement(By.CssSelector("div[class=\"roster-item view-mode\"]")).FindElement(By.ClassName("assignments-wrapper")).FindElement(By.ClassName("list"));
            
            int assignmentIndex = GetConfirmedAssignmentIndexByName(individualName, activityName);
            try
            {
                IWebElement assignmentTimeElement = assignmentsListElement.FindElements(By.TagName("ion-item"))[assignmentIndex].FindElement(By.ClassName("assignment-info")).FindElement(By.CssSelector("p[class=\"time ng-binding\"]"));
                time = assignmentTimeElement.Text.ToString();
            }
            catch (WebDriverException e)
            {
                time = "";
            }
            return time;
        }
        
        #endregion QR Code

        #region SetBrowserSizeTo common Mobile

        public void SetBrowserSizeTo_Mobile()
        {
            // Set window size to simulate iPhone 5
            this._driver.Manage().Window.Size = new Size(375, 667);

        }

        #endregion SetBrowserSizeTo common Mobile

        #region Static Methods
        public string GetSelfCheckInURL(F1Environments f1Environment, String churchCode)
        {

            string returnValue = string.Empty;

            //NOTE:later replace CheckIn's loginUrl
            switch (f1Environment)
            {
                case F1Environments.LOCAL:
                    returnValue = "http://mycheckin.local/login.aspx";
                    break;
                case F1Environments.QA:
                case F1Environments.LV_QA:
                case F1Environments.INT:
                case F1Environments.INT2:
                case F1Environments.INT3:
                case F1Environments.LV_UAT:
                case F1Environments.STAGING:
                    returnValue = string.Format("https://mycheckin.{0}.fellowshipone.com/#/{1}", GetEnvironmentInUse(f1Environment), churchCode.ToLower());
                    break;
                case F1Environments.LV_PROD:
                case F1Environments.PRODUCTION:
                    returnValue = string.Format("https://mycheckin.fellowshipone.com/#/{1}", churchCode.ToLower());
                    break;
                default:
                    throw new Exception(string.Format("[{0}] is Not a valid environment option!", f1Environment));
            }

            return returnValue;
        }

        public string GetF1DataURL(F1Environments f1Environment)
        {

            string returnValue = string.Empty;

            //NOTE:later replace F1data's loginUrl
            switch (f1Environment)
            {
                case F1Environments.LOCAL:
                    returnValue = "http://f1data.local/login.aspx";
                    break;
                case F1Environments.QA:
                case F1Environments.LV_QA:
                case F1Environments.INT:
                case F1Environments.INT2:
                case F1Environments.INT3:
                case F1Environments.LV_UAT:
                case F1Environments.STAGING:
                    returnValue = string.Format("https://f1data.{0}.fellowshipone.com/", GetEnvironmentInUse(f1Environment));
                    break;
                case F1Environments.LV_PROD:
                case F1Environments.PRODUCTION:
                    returnValue = "https://f1data.fellowshipone.com/";
                    break;
                default:
                    throw new Exception(string.Format("[{0}] is Not a valid environment option!", f1Environment));
            }

            return returnValue;
        }

        public string GetEnvironmentInUse(F1Environments f1Environment)
        {
            string returnValue = string.Empty;

            switch (f1Environment)
            {
                case F1Environments.QA:
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
                case F1Environments.PRODUCTION:
                    returnValue = "PROD";
                    break;
                default:
                    throw new Exception(string.Format("[{0}] is Not a valid LV environment option!", f1Environment));

            }

            return returnValue.ToLower();

        }

        #endregion Static Methods

        public object SECONDS { get; set; }

        #region Open F1Data

        public void OpenF1DataIndex()
        {
            log.Debug("Navigate to F1Data index Page");
            this._driver.Navigate().GoToUrl(GetF1DataURL(this._f1Environment) + "index.html");

        }

        public void OpenF1DataSignalR()
        {
            log.Debug("Navigate to F1Data SignalR Page");
            this._driver.Navigate().GoToUrl(GetF1DataURL(this._f1Environment) + "signalr/hubs");

        }

        #endregion
    }
}