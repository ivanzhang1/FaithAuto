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
    public class CheckInBaseWebDriver : FixtureBaseWebDriver
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected int churchId;
        protected string timeZoneName;
        protected DateTime currentTimzoneTime;
        private string currentDate;
        protected string[] rosterNameArray;
        protected string rosterToCheckIn;
        protected static string activityName = "";

        [SetUp]
        public void CheckInBaseWebDriver_SetUp()
        {

            log.Debug("Enter CheckIn Setup WebDriver");

            churchId = base.SQL.Ministry_Church_FetchID(test.CheckIn.ChurchCode);
            timeZoneName = base.SQL.Ministry_Activity_Instance_TimeZone(churchId);

            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            currentDate = Convert.ToString(currentTimzoneTime.Year) + Convert.ToString(currentTimzoneTime.Month) + Convert.ToString(currentTimzoneTime.Day);

            string rosterNamepre = "Roster_" ;
            rosterNameArray = new string[] { rosterNamepre + "_001", rosterNamepre + "_002", rosterNamepre + "_003", rosterNamepre + "_004", rosterNamepre + "_005", rosterNamepre + "_006", rosterNamepre + "_007", rosterNamepre + "_008", rosterNamepre + "_009", rosterNamepre + "_010" };
            rosterToCheckIn = rosterNameArray[0];

            // Activity resource naming
            if ("".Equals(activityName))
            {
                // activityName = "ActivityCheckIn" + currentDate + "_" + Convert.ToString(new Random().Next(1000, 9999));
                activityName = "ActivityCheckIn-T3" ;
            }

            TestLog.WriteLine("-activityName = {0}", activityName);
            
            if (base.SQL.Ministry_Activities_FetchID(churchId, activityName) != -1 && base.SQL.Ministry_Activities_FetchIDS(churchId, activityName).Length==1 && System.Math.Abs((Convert.ToDateTime(Convert.ToString(currentTimzoneTime).Split(' ')[0]).Subtract(
             Convert.ToDateTime(base.SQL.Ministry_Activities_FetchCreatedDate(churchId, activityName).Split(' ')[0]))).TotalDays) < 1)
            {
                TestLog.WriteLine("The activity with given name: " + activityName + " is already existent in DB, skip creating it.");
                return;
            }
            
            string activityScheduleName = "ScheduleForCheckIn" ;
            TestLog.WriteLine("-activityScheduleName = {0}", activityScheduleName);

            // Set under which ministry the activity wil be created
            string ministryName = "A Test Ministry"; // A Test Ministry, Bible Study

            int interalHour = 24 - currentTimzoneTime.Hour - 1;

            if (interalHour < 1)
            {
                TestLog.WriteLine("The rest of the day may not be enough for test execution, please wait!");
                Thread.Sleep(60 * 60 * 1000);
            }

            int ministryId = base.SQL.Ministry_Ministries_FetchID(churchId, ministryName);
            base.SQL.Ministry_ActivitySchedules_DeleteActivity(churchId, activityName);

            //create Activity
            base.SQL.Ministry_Activities_CreateForCheckIn(churchId, ministryId, activityName, rosterNameArray);
            //create schedule
            base.SQL.Ministry_ActivitySchedules_CreateForCheckIn(churchId, activityName, activityScheduleName, rosterNameArray,
                currentTimzoneTime.AddHours(0), currentTimzoneTime.AddHours(interalHour));

            //Teacher Check-in
            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            String individualId = Convert.ToString(base.SQL.People_Individuals_FetchIDByEmail(churchId, test.CheckIn.CheckInUsername));

            test.CheckIn.CheckInATeacher(churchId, activityName, individualId, rosterToCheckIn, currentTimzoneTime);
            test.CheckIn.CheckInATeacher(churchId, activityName, individualId, rosterNameArray[5], currentTimzoneTime);

            //Students Check-in
            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));//get new time
            string checkInTime = currentTimzoneTime.ToString();
            String[] individualNameArray = { "Christopher Kemp", "Emma Kemp", "Kayla Kemp", "Lorraine Kemp", "Chris Kemp", "Chris Balisteri", "GraceAutoDrop Zhang" };
            string individualTypeId = "1";//student 1
            TestLog.WriteLine("Test individuals--!");

            String[] attendenceids = test.CheckIn.CheckInMultipleIndividuals(activityName, individualNameArray, 16729, checkInTime, individualTypeId, rosterToCheckIn);

            for (int i = 0; i < attendenceids.Length; i++)
            {
                TestLog.WriteLine("-attendenceids = {0}", attendenceids[i]);
            }
            String[] otherIndividualNameArray = { "Mary Martin"};
            String[] otherAttendenceids = test.CheckIn.CheckInMultipleIndividuals(activityName, otherIndividualNameArray, 16729, checkInTime, individualTypeId, rosterNameArray[5]);

            for (int i = 0; i < otherAttendenceids.Length; i++)
            {
                TestLog.WriteLine("-otherAttendenceids = {0}", otherAttendenceids[i]);
            }

            log.Debug("Exit CheckIn Setup WebDriver");

        }

        [TearDown]
        public void CheckInBaseWebDriver_TearDown()
        {

            TestLog.WriteLine("Enter CheckIn TearDown WebDriver");

            // Uncomment below code if want to delete activity after case execution
            // TestLog.WriteLine("Delete the activity used in CheckIn teacher cases");
            // base.SQL.Ministry_ActivitySchedules_DeleteActivity(churchId, activityName);

            test.Driver.Quit();

            TestLog.WriteLine("Exit CheckIn TearDown WebDriver");

        }

    }

    public class CheckInTeacherBase
    {
        private RemoteWebDriver _driver;
        private ISelenium _selenium;
        private string _checkinUsername;
        private string _checkinPassword;
        private string _churchCode;
        private int _churchID;
        private GeneralMethods _generalMethods;
        private IList<string> _errorText = new List<string>();
        private JavaScript _javascript;
        private F1Environments _f1Environment;
        private SQL _sql;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Properties

        public string CheckInUsername
        {
            get { return _checkinUsername; }
            set { _checkinUsername = value; }
        }

        public string CheckInPassword
        {
            get { return _checkinPassword; }
            set { _checkinPassword = value; }
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

        #region Instance Methods

        public CheckInTeacherBase(ISelenium selenium, GeneralMethods generalMethods, JavaScript javascript, F1Environments f1Environment, SQL sql)
        {
            log.Debug("Enter CheckInBase");
            this._selenium = selenium;
            this._generalMethods = generalMethods;
            this._javascript = javascript;
            this._f1Environment = f1Environment;
            this._sql = sql;

            log.Debug("Exit CheckInBase");
        }

        public CheckInTeacherBase(RemoteWebDriver driver, GeneralMethods generalMethods, F1Environments f1Environment, SQL sql)
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

        public void LoginDirectWebDriver(Boolean needAssert = true)
        {
            LoginWebDriver(this._checkinUsername, this._checkinPassword, this._churchCode, needAssert);
        }

        public void LoginWebDriver(Boolean needAssert = true)
        {
            // Set browser size to pad.
            SetBrowserSizeTo_Pad();

            LoginWebDriver(this._checkinUsername, this._checkinPassword, this._churchCode, needAssert);
        }

        public void LoginRosterWebDriver(string rosterName, Boolean needAssert = true)
        {
            LoginWebDriver(needAssert);
            //Expand roster list
            this.ClickIconMenu();
            //select a classroom
            this.ClickSettingsRoom(rosterName);
            this.WaitForRosterDisplay();
        }

        public void LoginMobileWebDriver(Boolean needAssert = true)
        {
            // Set browser size to mobile phone.
            SetBrowserSizeTo_Mobile();

            LoginWebDriver(this._checkinUsername, this._checkinPassword, this._churchCode, needAssert);
        }

        public void LoginRosterMobileWebDriver(string rosterName, Boolean needAssert = true)
        {
            // Set browser size to mobile phone.
            SetBrowserSizeTo_Mobile();

            LoginDirectWebDriver(needAssert);
            //Expand roster list
            this.ClickIconMenu();
            //select a classroom
            this.ClickSettingsRoom(rosterName);
            this.WaitForRosterDisplay();
        }

        public void LoginMobileWebDriver(string username, string password, string churchCode, Boolean needAssert = true)
        {
            // Set browser size to mobile phone.
            SetBrowserSizeTo_Mobile();

            LoginWebDriver(username, password, churchCode, needAssert);
        }

        public void LoginWebDriver(string username, string password, string churchCode, Boolean needAssert = true)
        {

            TestLog.WriteLine(string.Format("Enter Login Web Driver {0}/{1}/{2}", username, password, churchCode));

            //Check if we are already in login page
            //This is when login/logout out of application
            if (!this._driver.Url.ToString().Contains(GetCheckInURL(this._f1Environment)))
            {
                // Open the login page for Teacher project
                log.Debug("Navigate to Login Page");
                this._driver.Navigate().GoToUrl(GetCheckInURL(this._f1Environment));

            }
            else
            {
                log.Debug("No need to Navigate to Login");
            }

            TestLog.WriteLine("Login to: " + this._driver.Url.ToString());

            // Set the username, password, and church code. NOTE:later replace CheckIn's Elements

            IWebElement ele = this._driver.FindElementByName("username");
            ele.Click();
            ele.Clear();
            ele.SendKeys(username);

            ele = this._driver.FindElementByName("password");
            ele.Click();
            ele.Clear();
            ele.SendKeys(password);

            ele = this._driver.FindElementByName("churchCode");
            ele.Click();
            ele.Clear();
            ele.SendKeys(churchCode);

            // Attempt to log in
            this._driver.FindElementByCssSelector("[type=\"submit\"]").Click();

            //We need to verify that we don't get routed to login page
            TestLog.WriteLine("URL routed to: " + this._driver.Url.ToString());

            bool logInSucceed = false;
            try
            {
                String churchName = this._sql.FetchChurchName(churchCode);
                this._generalMethods.WaitForPageIsLoaded();
                this._generalMethods.WaitForElement(By.CssSelector("[class=\"nav-bar-block\"][nav-bar=\"active\"]"));
                IWebElement titleCenterElement = this._driver.FindElementByCssSelector("[class=\"nav-bar-block\"][nav-bar=\"active\"]").FindElement(By.CssSelector("[align-title=\"center\"]")).FindElement(By.CssSelector("[class=\"title title-center header-item\"]"));
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

                    // Set the username, password, and church code. NOTE:later replace CheckIn's Elements

                    ele = this._driver.FindElementByName("username");
                    ele.Clear();
                    ele.SendKeys(username);

                    ele = this._driver.FindElementByName("password");
                    ele.Clear();
                    ele.SendKeys(password);

                    ele = this._driver.FindElementByName("churchCode");
                    ele.Clear();
                    ele.SendKeys(churchCode);

                    // Attempt to log in
                    this._driver.FindElementByCssSelector("[type=\"submit\"]").Click();
                }

            }

        }
        public void ClickIconMenu()
        {
            this._generalMethods.WaitForElement(this._driver, By.XPath("//div[@class='nav-bar-block'][@nav-bar='active']//a[@menu-toggle='right']"));
            this._driver.FindElement(By.XPath("//div[@class='nav-bar-block'][@nav-bar='active']//a[@menu-toggle='right']")).Click();

            try
            {
                this._generalMethods.WaitForElement(By.CssSelector("[style=\"transform: translate3d(-350px, 0px, 0px);\"]"));
            }
            catch (Exception e)
            {
                TestLog.WriteLine("Roster menu doesn't expanded in the first click, try again");
                //add by grace zhang:when roster menu doesn't expanded in the first click, try again
                this._driver.FindElement(By.XPath("//div[@class='nav-bar-block'][@nav-bar='active']//a[@menu-toggle='right']")).Click();
            }
        }

        public void ClickSettingsRoom(String classRoomName)
        {

            this._generalMethods.WaitForElement(this._driver, By.XPath("//ion-item[@ng-class='{classroomSelected:assignment.detailID == settings.selectedDetailID && assignment.instanceID == settings.selectedInstanceID}']"));
            int classRoomNumber = this._driver.FindElements(By.XPath("//ion-item[@ng-class='{classroomSelected:assignment.detailID == settings.selectedDetailID && assignment.instanceID == settings.selectedInstanceID}']")).Count;
            TestLog.WriteLine("Find roster section count is: " + classRoomNumber);
            classRoomNumber *= 2;
            for (int i = 0; i < classRoomNumber; i++)
            {
                string classRoomtempName = this._driver.FindElementsByXPath("//ion-item[@ng-class='{classroomSelected:assignment.detailID == settings.selectedDetailID && assignment.instanceID == settings.selectedInstanceID}']/button/span")[i].Text;
                TestLog.WriteLine("The roster name found is: " + classRoomtempName);

                if (classRoomtempName.Trim().Equals(classRoomName.Trim()))
                {
                    TestLog.WriteLine("The roster is matched, click on it");
                    this._driver.FindElementsByXPath("//ion-item[@ng-class='{classroomSelected:assignment.detailID == settings.selectedDetailID && assignment.instanceID == settings.selectedInstanceID}']/button/span")[i].Click();
                    this.WaitForRosterDisplay();
                    try
                    {
                        this._generalMethods.WaitForElement(By.CssSelector("[style=\"transform: translate3d(0px, 0px, 0px);\"]"));
                    }
                    catch (Exception e)
                    {
                        this.ClickIconMenu();
                        this.WaitForRosterDisplay();
                    }
                    return;

                }

            }

            throw new Exception("Fail to find the expected roster: " + classRoomName);

        }

        public bool IsExistSettingsRoom(String classRoomName)
        {
            bool isExist = false;
            this._generalMethods.WaitForElement(this._driver, By.XPath("//ion-item[@ng-class='{classroomSelected:assignment.detailID == settings.selectedDetailID && assignment.instanceID == settings.selectedInstanceID}']"));
            int classRoomNumber = this._driver.FindElements(By.XPath("//ion-item[@ng-class='{classroomSelected:assignment.detailID == settings.selectedDetailID && assignment.instanceID == settings.selectedInstanceID}']")).Count;
            TestLog.WriteLine("Find roster section count is: " + classRoomNumber);
            classRoomNumber *= 2;
            for (int i = 0; i < classRoomNumber; i++)
            {
                string classRoomtempName = this._driver.FindElementsByXPath("//ion-item[@ng-class='{classroomSelected:assignment.detailID == settings.selectedDetailID && assignment.instanceID == settings.selectedInstanceID}']/button/span")[i].Text;
                TestLog.WriteLine("The roster name found is: " + classRoomtempName);

                if (classRoomtempName.Trim().Equals(classRoomName.Trim()))
                {
                    isExist = true;
                }

            }
            return isExist;
        }

        public String GetTeacherName()
        {

            this._generalMethods.WaitForElement(this._driver, By.CssSelector("[class=\"account\"]"));
            String teacherName = this._driver.FindElement(By.CssSelector("[class=\"account\"]")).FindElement(By.CssSelector("[class=\"user-name ng-binding\"]")).Text.ToString();
            return teacherName;           
        }

        public void LogoutWebDriver()
        {
            //later modify by actural checkin system

            log.Debug("Enter Logout Web Driver");
            this._generalMethods.WaitForElement(this._driver, By.CssSelector("[ng-click=\"sidemenu.logout()\"]"));
            this._driver.FindElement(By.CssSelector("[ng-click=\"sidemenu.logout()\"]")).Click();

            log.Debug("Exit Login Web Driver ");

        }
        
       
        #endregion Login / Logout

        #region Name Search
        public void OpenIconSearch()
        {
            this._generalMethods.WaitForElementDisplayed(By.XPath("//div[@class='nav-bar-block'][@nav-bar='active']//a[@ng-click='sidemenu.toggleSearch()']"));
            this._driver.FindElementByXPath("//div[@class='nav-bar-block'][@nav-bar='active']//a[@ng-click='sidemenu.toggleSearch()']").Click();
            this._generalMethods.WaitForElementDisplayed(By.XPath("//div[@class='name-search']/input"));
        }
        public void CloseIconSearch()
        {
            this._generalMethods.WaitForElementDisplayed(By.XPath("//div[@class='nav-bar-block'][@nav-bar='active']//a[@ng-click='sidemenu.toggleSearch()']"));
            this._driver.FindElementByXPath("//div[@class='nav-bar-block'][@nav-bar='active']//a[@ng-click='sidemenu.toggleSearch()']").Click();
        }
        public void TypeNameAndSearch(String Name)
        {
            this._generalMethods.WaitForElementDisplayed(By.XPath("//div[@class='name-search']/input"));
            this._driver.FindElement(By.CssSelector("[class=\"name-search\"]")).FindElement(By.TagName("input")).SendKeys(Name);
            System.Threading.Thread.Sleep(3000);
        }
       
        #endregion Name Search

        #region Filter

        public void OpenIconFilter()
        {
            this._generalMethods.WaitForElementDisplayed(By.XPath("//div[@class='nav-bar-block'][@nav-bar='active']//a[@ng-click='sidemenu.toggleFilters()']"));
            this._driver.FindElementByXPath("//div[@class='nav-bar-block'][@nav-bar='active']//a[@ng-click='sidemenu.toggleFilters()']").Click();
            this._generalMethods.WaitForElementDisplayed(By.CssSelector("[class=\"filter-by\"]"));
        }
        public void CloseIconFilter()
        {
            this._generalMethods.WaitForElementDisplayed(By.XPath("//div[@class='nav-bar-block'][@nav-bar='active']//a[@ng-click='sidemenu.toggleFilters()']"));
            this._driver.FindElementByXPath("//div[@class='nav-bar-block'][@nav-bar='active']//a[@ng-click='sidemenu.toggleFilters()']").Click();
            this.WaitForRosterDisplay();
        }

        public void SetFilter(bool Notes, bool OnSite, bool In, bool Out)
        {
            IWebElement element = this._driver.FindElementByXPath("//label[@for='filter-special']/input");
            if (!element.Selected.Equals(Notes))
            {
                element.Click();
            }

            element = this._driver.FindElementByXPath("//label[@for='filter-checkedIn']/input");
            if (!element.Selected.Equals(OnSite))
            {
                element.Click();
            }

            element = this._driver.FindElementByXPath("//label[@for='filter-present']/input");
            if (!element.Selected.Equals(In))
            {
                element.Click();
            }

            element = this._driver.FindElementByXPath("//label[@for='filter-checkedOut']/input");
            if (!element.Selected.Equals(Out))
            {
                element.Click();
            }
        }
        public void SetSortBy(bool byFirstName)
        {
            if (byFirstName)
            {
                this._driver.FindElementByCssSelector("[value=\"firstName\"]").Click();
            }
            else
            {
                this._driver.FindElementByCssSelector("[value=\"lastName\"]").Click();
            }
        }

        #endregion Filter

        #region Roster Operation

        public String GetChurchName()
        {
            WaitForRosterDisplay();
            IWebElement churchnameItem = this._driver.FindElements(By.CssSelector("div[class=\"title title-center header-item\"]"))[1];
            return  churchnameItem.Text.ToString().Trim();  
        }
        //if there is no goesbyName,please input null or "";return the number of the Students list
        public int GetListIndexByStudentName(String firstName, String goesbyName, String lastName)
        {
            
            String inputName = "";
            if (goesbyName == null || goesbyName == "")
            {
                inputName = firstName.Trim() + " " + lastName.Trim();
            }
            else
            {
                inputName = firstName.Trim() + " " + goesbyName.Trim() + " " + lastName.Trim();
            }

            return GetListIndexByStudentFullName(inputName);

        }

        public int GetListIndexByStudentFullName(String fullName)
        {
            try
            {
                this._generalMethods.WaitForElement(By.CssSelector("[style=\"transform: translate3d(0px, 0px, 0px);\"]"));
            }
            catch (Exception e)
            {
                this.ClickIconMenu();
            }

            String studentName = "";
            int listNo = -1;

            this._generalMethods.WaitForElement(this._driver, By.CssSelector("[class=\"list\"]"));

            int studentsNumber = this._driver.FindElement(By.ClassName("student-list")).FindElements(By.TagName("ion-item")).Count;
            for (int i = 0; i < studentsNumber; i++)
            {
                IWebElement rosterItem = this._driver.FindElements(By.TagName("ion-item"))[i].FindElement(By.CssSelector("[class=\"roster-item\"]"));
                IWebElement rosterItemMain = rosterItem.FindElement(By.CssSelector("[class=\"roster-item-main\"]"));
                IWebElement studentP = rosterItemMain.FindElement(By.CssSelector("[class=\"student-name disable-select\"]")).FindElement(By.TagName("p"));
                studentName = studentP.Text.ToString().Trim();
                string[] nameArray = studentName.Split(' ');
                if (nameArray.Length == 3)
                {

                    nameArray[1] = nameArray[1].Substring(1, nameArray[1].Length - 2);
                }

                if (nameArray.Length == 2)
                {
                    studentName = nameArray[0].Trim() + " " + nameArray[1].Trim();
                }
                if (nameArray.Length == 3)
                {
                    studentName = nameArray[0].Trim() + " " + nameArray[1].Trim() + " " + nameArray[2].Trim();
                }
                TestLog.WriteLine("-studentName = {0}", studentName);
                if (studentName == fullName)
                {
                    listNo = i;
                    break;
                }
            }

            TestLog.WriteLine("Found roster index: " + listNo + " of individual: " + fullName);
            return listNo;
        }
        public int StudentAppearTimesOnRoster(String firstName, String goesbyName, String lastName)
        {
            int appearTimes = 0;
            String fullName = "";
            if (goesbyName == null || goesbyName == "")
            {
                fullName = firstName.Trim() + " " + lastName.Trim();
            }
            else
            {
                fullName = firstName.Trim() + " " + goesbyName.Trim() + " " + lastName.Trim();
            }

            try
            {
                this._generalMethods.WaitForElement(By.CssSelector("[style=\"transform: translate3d(0px, 0px, 0px);\"]"));
            }
            catch (Exception e)
            {
                this.ClickIconMenu();
            }

            String studentName = "";
           
            this._generalMethods.WaitForElement(this._driver, By.CssSelector("[class=\"list\"]"));

            int studentsNumber = this._driver.FindElement(By.ClassName("student-list")).FindElements(By.TagName("ion-item")).Count;
            for (int i = 0; i < studentsNumber; i++)
            {
                IWebElement rosterItem = this._driver.FindElements(By.TagName("ion-item"))[i].FindElement(By.CssSelector("[class=\"roster-item\"]"));
                IWebElement rosterItemMain = rosterItem.FindElement(By.CssSelector("[class=\"roster-item-main\"]"));
                IWebElement studentP = rosterItemMain.FindElement(By.CssSelector("[class=\"student-name disable-select\"]")).FindElement(By.TagName("p"));
                studentName = studentP.Text.ToString().Trim();
                string[] nameArray = studentName.Split(' ');
                if (nameArray.Length == 3)
                {

                    nameArray[1] = nameArray[1].Substring(1, nameArray[1].Length - 2);
                }

                if (nameArray.Length == 2)
                {
                    studentName = nameArray[0].Trim() + " " + nameArray[1].Trim();
                }
                if (nameArray.Length == 3)
                {
                    studentName = nameArray[0].Trim() + " " + nameArray[1].Trim() + " " + nameArray[2].Trim();
                }
                TestLog.WriteLine("-studentName = {0}", studentName);
                if (studentName == fullName)
                {
                    appearTimes = appearTimes + 1;
                    continue;
                }
            }

            TestLog.WriteLine("Appear times of student " + fullName +"is :"+ appearTimes);
            return appearTimes;
        }

        public void WaitForRosterDisplay()
        {
            //must wait
            Thread.Sleep(2 * 1000);
            this._generalMethods.WaitForElementDisplayed(By.CssSelector("[class=\"list\"]"));
        }

        public string GetRandomStudentNameFromRoster()
        {

            String studentName = "";
            this._generalMethods.WaitForElement(this._driver, By.CssSelector("[class=\"list\"]"));

            int studentsNumber = this._driver.FindElement(By.CssSelector("[class=\"list\"]")).FindElements(By.TagName("ion-item")).Count;
            int i = new Random().Next(0, studentsNumber);

            IWebElement rosterItem = this._driver.FindElements(By.TagName("ion-item"))[i].FindElement(By.CssSelector("[class=\"roster-item\"]"));
            IWebElement rosterItemMain = rosterItem.FindElement(By.CssSelector("[class=\"roster-item-main\"]"));
            IWebElement studentP = rosterItemMain.FindElement(By.CssSelector("[class=\"student-name disable-select\"]")).FindElement(By.TagName("p"));
            studentName = studentP.Text.ToString().Trim();

            return studentName;
        }

        public int GetTotalNumOfStudentsFromRoster()
        {
            int count = 0;
            List<IWebElement> roster = this._driver.FindElement(By.CssSelector("[class=\"list\"]")).FindElements(By.TagName("ion-item")).ToList();
            foreach(IWebElement ele in roster) 
            {
                if (ele.Displayed)
                    count++;
            }

            return count;
        }

        public int GetRandomStudentIndexFromRoster()
        {
            int studentsNumber = this.GetTotalNumOfStudentsFromRoster();
            int i = new Random().Next(0, studentsNumber);

            return i;
        }

        public String GetSecurityCodeByStudentName(String firstName, String goesbyName, String lastName)
        {
            int listNo = GetListIndexByStudentName(firstName, goesbyName, lastName);
            return GetSecurityCodeByStudentIndex(listNo);
        }
        public String GetMobileSecurityCodeByStudentName(String firstName, String goesbyName, String lastName)
        {
            int listNo = GetListIndexByStudentName(firstName, goesbyName, lastName);
            return GetMobileSecurityCodeByStudentIndex(listNo);
        }

        public String GetSecurityCodeByStudentFullName(String fullName)
        {
            int listNo = GetListIndexByStudentFullName(fullName);
            return GetSecurityCodeByStudentIndex(listNo);
        }
       
        public String GetSecurityCodeByStudentIndex(int listNo)
        {
            IWebElement rosterItem = this._driver.FindElements(By.TagName("ion-item"))[listNo].FindElement(By.CssSelector("[class=\"roster-item\"]"));
            IWebElement rosterItemMain = rosterItem.FindElement(By.CssSelector("[class=\"roster-item-main\"]"));
            IWebElement securityCodeDiv = rosterItemMain.FindElement(By.CssSelector("[class=\"security-code\"]"));
            return securityCodeDiv.Text.ToString().Trim();
        }
        public String GetMobileSecurityCodeByStudentIndex(int listNo)
        {
            IWebElement rosterItem = this._driver.FindElements(By.TagName("ion-item"))[listNo].FindElement(By.CssSelector("[class=\"roster-item\"]"));
            IWebElement rosterItemMain = rosterItem.FindElement(By.CssSelector("[class=\"roster-item-main\"]")).FindElement(By.CssSelector("[class=\"student-name disable-select\"]"));
            rosterItemMain.Click();
            IWebElement drawerDataItem = rosterItem.FindElement(By.CssSelector("[class=\"drawer\"]")).FindElement(By.ClassName("drawer-data"));
            IWebElement securityCodeDiv = drawerDataItem.FindElement(By.TagName("div"));
            return securityCodeDiv.Text.ToString().Trim();
        }
        public void CheckInByStudentName(String firstName, String goesbyName, String lastName)
        {
            int listNo = GetListIndexByStudentName(firstName, goesbyName, lastName);
            CheckInByStudentIndex(listNo);
        }
        public void CheckInByStudentFullName(String fullName)
        {
            int listNo = GetListIndexByStudentFullName(fullName);
            CheckInByStudentIndex(listNo);
        }
        public void CheckInByStudentIndex(int listNo)
        {
            IWebElement rosterItem = this._driver.FindElements(By.TagName("ion-item"))[listNo].FindElement(By.CssSelector("[class=\"roster-item\"]"));
            IWebElement rosterItemControl = rosterItem.FindElement(By.CssSelector("[class=\"roster-item-control\"]"));
            rosterItemControl.FindElement(By.CssSelector("[class=\"gooey-control\"]")).FindElement(By.CssSelector("div.check-mark.svg-button")).Click();  
        }

        public void CheckOutByStudentName(String firstName, String goesbyName, String lastName)
        {
            int listNo = GetListIndexByStudentName(firstName, goesbyName, lastName);
            CheckOutByStudentIndex(listNo);
        }
        public void CheckOutByStudentFullName(String fullName)
        {
            int listNo = GetListIndexByStudentFullName(fullName);
            CheckOutByStudentIndex(listNo);
        }
        public void CheckOutByStudentIndex(int listNo)
        {
            IWebElement rosterItem = this._driver.FindElements(By.TagName("ion-item"))[listNo].FindElement(By.CssSelector("[class=\"roster-item\"]"));
            IWebElement rosterItemControl = rosterItem.FindElement(By.CssSelector("[class=\"roster-item-control\"]"));
            rosterItemControl.FindElement(By.CssSelector("[class=\"gooey-control\"]")).FindElement(By.CssSelector("div.out.svg-button")).Click();
        }

        public void UndoByStudentName(String firstName, String goesbyName, String lastName)
        {
            int listNo = GetListIndexByStudentName(firstName, goesbyName, lastName);
            UndoByStudentIndex(listNo);
        }
        public void UndoByStudentFullName(String fullName)
        {
            int listNo = GetListIndexByStudentFullName(fullName);
            UndoByStudentIndex(listNo);
        }
        public void UndoByStudentIndex(int listNo)
        {
            IWebElement rosterItem = this._driver.FindElements(By.TagName("ion-item"))[listNo].FindElement(By.CssSelector("[class=\"roster-item\"]"));
            IWebElement rosterItemControl = rosterItem.FindElement(By.CssSelector("[class=\"roster-item-control\"]"));
            rosterItemControl.FindElement(By.CssSelector("[class=\"gooey-control\"]")).FindElement(By.CssSelector("div.map-pin.svg-button")).Click();
        }

        #endregion Roster Operation

        #region Drawer

        public void OpenInfoByStudentName(String firstName, String goesbyName, String lastName)
        {
            int listNo = GetListIndexByStudentName(firstName, goesbyName, lastName);
            IWebElement rosterItem = this._driver.FindElements(By.TagName("ion-item"))[listNo].FindElement(By.CssSelector("[class=\"roster-item\"]"));
            IWebElement rosterItemMain =  rosterItem.FindElement(By.CssSelector("[class=\"roster-item-main\"]"));
            rosterItemMain.FindElement(By.CssSelector("[class=\"student-name disable-select\"]")).Click();
        }

        public String getBirthdayByStudentName(String firstName, String goesbyName, String lastName)
        {
            int listNo = GetListIndexByStudentName(firstName, goesbyName, lastName);
            OpenInfoByStudentName(firstName, goesbyName, lastName);
            IWebElement rosterItem = this._driver.FindElements(By.TagName("ion-item"))[listNo].FindElement(By.CssSelector("[class=\"roster-item\"]"));
            IWebElement drawerItem = rosterItem.FindElement(By.CssSelector("[class=\"drawer\"]"));
            IWebElement birthdayTR = drawerItem.FindElement(By.CssSelector("div[class=\"info-section\"]")).FindElement(By.CssSelector("[ng-if=\"showBirthday\"]")).FindElement(By.TagName("p"));

            String birthday = birthdayTR.Text.ToString().Trim();
            String birthdayInfo = "";
            for (int i = 0; i < birthday.Split(Environment.NewLine.ToCharArray()).Length; i++)
            {
                TestLog.WriteLine("array = {0}", birthday.Split(Environment.NewLine.ToCharArray())[i]);
                if (birthday.Split(Environment.NewLine.ToCharArray())[i].Contains(firstName))
                {
                    birthdayInfo = birthday.Split(Environment.NewLine.ToCharArray())[i];
                    break;
                }
            }
            return birthdayInfo;

        }
        
        public String GetLastAttendanceByStudentName(String firstName, String goesbyName, String lastName)
        {         
            int listNo = GetListIndexByStudentName(firstName, goesbyName, lastName);
            OpenInfoByStudentName(firstName, goesbyName, lastName);
            IWebElement rosterItem = this._driver.FindElements(By.TagName("ion-item"))[listNo].FindElement(By.CssSelector("[class=\"roster-item\"]")).FindElement(By.CssSelector("[class=\"drawer\"]"));
            IWebElement statusTR = rosterItem.FindElement(By.CssSelector("li[class=\"data status\"]")).FindElement(By.CssSelector("div[class=\"info\"]"));
            String lastAttendanceDateInfo = statusTR.FindElement(By.CssSelector("span[class=\"last-attended\"]")).Text.ToString().Trim();
            TestLog.WriteLine("-lastAttendanceDateInfo = {0}", lastAttendanceDateInfo);
            return lastAttendanceDateInfo;
           
        }
        public String GetStatusAndTimeByStudentName(String firstName, String goesbyName, String lastName)
        {
            int listNo = GetListIndexByStudentName(firstName, goesbyName, lastName);
            TestLog.WriteLine("-listNoforStatus = {0}", listNo);
            OpenInfoByStudentName(firstName, goesbyName, lastName);
            IWebElement rosterItem = this._driver.FindElement(By.ClassName("student-list")).FindElements(By.TagName("ion-item"))[listNo].FindElement(By.CssSelector("[class=\"roster-item\"]"));
            IWebElement drawerItem = rosterItem.FindElement(By.CssSelector("[class=\"drawer\"]"));
            IWebElement statusTR = drawerItem.FindElement(By.CssSelector("[class=\"drawer-data\"]")).FindElement(By.CssSelector("[class=\"info-section\"]")).FindElement(By.CssSelector("[class=\"data status\"]"));
            IWebElement statusInfoTD = statusTR.FindElement(By.CssSelector("[class=\"info\"]"));
            String statusAndTimeInfo = statusInfoTD.FindElement(By.CssSelector("span[class=\"status-time ng-binding\"]")).Text.ToString().Trim();
            TestLog.WriteLine("-statusAndTimeInfo = {0}", statusAndTimeInfo);
            String statusTimeInfo = "";
            for (int i = 0; i < statusAndTimeInfo.Split(Environment.NewLine.ToCharArray()).Length; i++)
            {
                TestLog.WriteLine("array = {0}", statusAndTimeInfo.Split(Environment.NewLine.ToCharArray())[i]);
                if (statusAndTimeInfo.Split(Environment.NewLine.ToCharArray())[i].Contains("at"))
                {
                    statusTimeInfo = statusAndTimeInfo.Split(Environment.NewLine.ToCharArray())[i].Trim();
                    break;
                }
            }
            return statusTimeInfo;

        }
        
        public String GetTagCommentByStudentName(String firstName, String goesbyName, String lastName)
        {
            int listNo = GetListIndexByStudentName(firstName, goesbyName, lastName);
            OpenInfoByStudentName(firstName, goesbyName, lastName);
            IWebElement rosterItem = this._driver.FindElements(By.TagName("ion-item"))[listNo].FindElement(By.CssSelector("[class=\"roster-item\"]")).FindElement(By.CssSelector("[class=\"drawer\"]"));
            IWebElement tagCommentTR = rosterItem.FindElement(By.CssSelector("div[class=\"info-section important-information\"]")).FindElement(By.CssSelector("li[ng-repeat=\"info in data.importantInformation\"]"));

            String tagCommentInfo = tagCommentTR.FindElement(By.TagName("p")).Text.ToString().Trim();
            TestLog.WriteLine("-tagCommentInfo = {0}", tagCommentInfo);       
            return tagCommentInfo;
        }

        #endregion

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

        #region generateActiveCheckInJson / get response's info

        //typeId:1 Participant,2 Staff,3 Paid Worker,4 Volunteer
        public String GenerateActiveCheckInJson(String activityName, String individualId, String individualTypeId, String rosterName)
        {
            APIBase api = new APIBase();
            int churchId = this._sql.Ministry_Church_FetchID(this._churchCode);
            int activityId = this._sql.Ministry_Activities_FetchID(churchId, activityName);
            if (-1 == activityId)
            {
                Assert.Fail("Activity: " + activityName + " can't be found as expected");
            }
            String activityInstanceId = Convert.ToString(this._sql.Ministry_Activities_FetchInstanceID(churchId, activityId));
            String rosterId = Convert.ToString(this._sql.Ministry_Activities_FetchRosterIDByName(churchId, activityId, rosterName));
            String churchCode = this._churchCode;
            String enviroment = GetEnvironmentInUse(this._f1Environment);
            //String individualTypeId = getOneIndividualTypeId(activityName, individualId);
            String individualTypeName = GetIndividualTypeName(individualTypeId);

            return api.GenerateCheckInJson(activityName, Convert.ToString(activityId), individualId, activityInstanceId, rosterId, individualTypeId, individualTypeName, this._churchCode.ToLower(), enviroment);

        }
        //typeId:1 Participant,2 Staff,3 Paid Worker,4 Volunteer
        public String GenerateActiveCheckInJson(String activityName, String individualId, String individualTypeId, String rosterName,String activityInstanceId)
        {
            APIBase api = new APIBase();
            int churchId = this._sql.Ministry_Church_FetchID(this._churchCode);
            int activityId = this._sql.Ministry_Activities_FetchID(churchId, activityName);
            if (-1 == activityId)
            {
                Assert.Fail("Activity: " + activityName + " can't be found as expected");
            }
            //String activityInstanceId = Convert.ToString(this._sql.Ministry_Activities_FetchInstanceID(churchId, Convert.ToInt32(activityId)));
            String rosterId = Convert.ToString(this._sql.Ministry_Activities_FetchRosterIDByName(churchId, activityId, rosterName));
            String churchCode = this._churchCode;
            String enviroment = GetEnvironmentInUse(this._f1Environment);
            //String individualTypeId = getOneIndividualTypeId(activityName, individualId);
            String individualTypeName = GetIndividualTypeName(individualTypeId);

            return api.GenerateCheckInJson(activityName, Convert.ToString(activityId), individualId, activityInstanceId, rosterId, individualTypeId, individualTypeName, this._churchCode.ToLower(), enviroment);

        }
        //get a type of individual , relative to an activity
        public String GetIndividualTypeId(String activityName, String individualId)
        {
            int churchId = this._sql.Ministry_Church_FetchID(this._churchCode);
            String activityId = Convert.ToString(this._sql.Ministry_Activities_FetchID(churchId, activityName));
            String activityInstanceId = Convert.ToString(this._sql.Ministry_Activities_FetchInstanceID(churchId, Convert.ToInt32(activityId)));
            String individualTypeId = "";
            try
            {
                individualTypeId = Convert.ToString(this._sql.Ministry_Activities_FetchIndividualTypeId(churchId, Convert.ToInt32(individualId), Convert.ToInt32(activityInstanceId)));
            }
            catch (Exception e)
            {
                individualTypeId = "1";
                e.ToString();
            }

            return individualTypeId;

        }
        public String GetIndividualTypeName(String individualTypeId)
        {
            String individualTypeName = Convert.ToString(this._sql.Ministry_Activities_FetchIndividualTypeName(Convert.ToInt32(individualTypeId)));
            return individualTypeName;

        }

        //create
        public String GetCheckInURl()
        {
            return "https://" + this._churchCode.ToLower() + "." + GetEnvironmentInUse(this._f1Environment) + ".fellowshiponeapi.com/activities/v1/attendances/?mode=demo";

        }
        //delete
        public String GetCheckOutURl(String attendanceId)
        {
            String URL = "https://" + this._churchCode.ToLower() + "." + GetEnvironmentInUse(this._f1Environment) + ".fellowshiponeapi.com/activities/v1/attendances/" + attendanceId + "?mode=demo";
            return URL;

        }
        //RealTimeNotice
        public String GetRealTimeNoticeURl()
        {
            return "https://f1data." + GetEnvironmentInUse(this._f1Environment) + ".fellowshipone.com/api/IndividualInstance/UpdateIndividualInstance";

        }
        public string GetValueByStrKey(string orgString, string key)
        {
            //key = string.Format("\"{0}\":\"", key);
            string[] splits = orgString.Split(new string[] { key }, StringSplitOptions.None);
            return splits[1].Substring(0, splits[1].IndexOf("\","));
        }

        public string GetValueByStrKey(string orgString, string preKey, string key)
        {
            preKey = string.Format("\"{0}\"", preKey);
            string[] splits = orgString.Split(new string[] { preKey }, StringSplitOptions.None);
            string tempString = "";
            for (int i = 1; i < splits.Length; i++)
            {
                tempString = tempString + splits[i];
            }

            return GetValueByStrKey(tempString, key);
        }

        public string[] GetMultipleIndividuals(int individualNumber)
        {
            int churchId = this._sql.Ministry_Church_FetchID(this._churchCode);

            return this._sql.Ministry_Activities_FetchMultipleIndividualIDs(churchId, individualNumber);
        }

        public string[] CheckInMultipleIndividuals(String activityName, int individualNumber, int checkInMachineId, 
            String checkInTime, String individualTypeId, String rosterName, String activityInstanceId = "")
        {
            String[] individuals = GetMultipleIndividuals(individualNumber);

            return CheckInMultipleIndividualsById(activityName, individuals, checkInMachineId, checkInTime, individualTypeId, rosterName, activityInstanceId);

        }
        
        public string[] CheckInMultipleIndividuals(String activityName, String[] individualNameArray, int checkInMachineId, 
            String checkInTime, String individualTypeId, String rosterName)
        {
            int churchId = this._sql.Ministry_Church_FetchID(this._churchCode);
            string[] individuals = new string[individualNameArray.Length];
            for (int i = 0; i < individuals.Length; i++)
            {
                TestLog.WriteLine("-individualNameArray["+Convert.ToString(i)+"] = {0}", individualNameArray[i]);
                TestLog.WriteLine("-churchId = {0}", churchId);
                individuals[i] = Convert.ToString(this._sql.People_Individuals_FetchIDByName(churchId, individualNameArray[i])); 
            }

            return CheckInMultipleIndividualsById(activityName, individuals, checkInMachineId, checkInTime, individualTypeId, rosterName);

        }

        private string[] CheckInMultipleIndividualsById(String activityName, String[] individuals, int checkInMachineId,
            String checkInTime, String individualTypeId, String rosterName, String activityInstanceId = "")
        {
            String url = GetCheckInURl();
            TestLog.WriteLine("-createUrl = {0}", url);

            string[] attendanceIds = new string[individuals.Length];
            APIBase api = new APIBase();
            string json;
            string responseString;
            for (int i = 0; i < individuals.Length; i++)
            {
                TestLog.WriteLine("-individual = {0}", i);

                if ("".Equals(activityInstanceId))
                    {
                        json = GenerateActiveCheckInJson(activityName, individuals[i], individualTypeId, rosterName);
                    }
                    else
                    {
                        json = GenerateActiveCheckInJson(activityName, individuals[i], individualTypeId, rosterName, activityInstanceId);
                    }
                TestLog.WriteLine("-createJson = {0}", json);

                responseString = api.SendAPIRequestwithBodyNoAuth(url, "POST", json, HttpStatusCode.Created);
                TestLog.WriteLine("-createResponseString = {0}", responseString);
                if (responseString == "ERROR")
                {
                    attendanceIds[i] = "ERROR";
                }
                else
                {
                    Assert.Contains(responseString, "{\"id\":");
                    Assert.Contains(responseString, "\"url\":");
                    attendanceIds[i] = GetValueByStrKey(responseString, "url", "attendances/");
                    TestLog.WriteLine("-createAttendenceId = {0}", attendanceIds[i]);
                    if ("".Equals(activityInstanceId))
                    {
                        UpdateIndividual_Instance(activityName, individuals[i], checkInMachineId, checkInTime);
                    }
                    else
                    {
                        UpdateIndividual_Instance(activityName, individuals[i], checkInMachineId, checkInTime, activityInstanceId);
                    }


                }
            }

            return attendanceIds;

        }

        public string CheckInATeacher(int churchId, string activityName, string individualId, string rosterToCheckIn, DateTime checkInTime)
        {
            //Teacher Check-in
            String url = this.GetCheckInURl();
            String json = this.GenerateActiveCheckInJson(activityName, individualId, "2", rosterToCheckIn);
            TestLog.WriteLine("-createUrl = {0}", url);
            TestLog.WriteLine("-createJson = {0}", json);

            APIBase api = new APIBase();
            String responseString = api.SendAPIRequestwithBodyNoAuth(url, "POST", json, HttpStatusCode.Created);
            TestLog.WriteLine("-createResponseString = {0}", responseString);

            Assert.Contains(responseString, "{\"id\":");
            Assert.Contains(responseString, "\"url\":");
            string attendenceId = this.GetValueByStrKey(responseString, "url", "attendances/");
            TestLog.WriteLine("-createAttendenceId = {0}", attendenceId);

            this.UpdateIndividual_Instance(activityName, individualId, 16729, checkInTime.ToString());

            return attendenceId;
        }

        /**
         * after Api check in,update table:ChmActivity.dbo.INDIVIDUAL_INSTANCE
         * update ChmActivity.dbo.INDIVIDUAL_INSTANCE set CHECK_IN_MACHINE_ID = 16729  WHERE CHURCH_ID = 15 AND INDIVIDUAL_ID = 635098 AND ACTIVITY_INSTANCE_ID = 33966778
         * update ChmActivity.dbo.INDIVIDUAL_INSTANCE set CHECK_IN_TIME = '2015-07-21 11:21:00.000'  WHERE CHURCH_ID = 15 AND INDIVIDUAL_ID = 635098 AND ACTIVITY_INSTANCE_ID = 33966778
         * */
        public void UpdateIndividual_Instance(String activityName, String individualId, int checkInMachineId, String checkInTime)
        {
            APIBase api = new APIBase();
            int churchId = this._sql.Ministry_Church_FetchID(this._churchCode);
            String activityId = Convert.ToString(this._sql.Ministry_Activities_FetchID(churchId, activityName));
            String activityInstanceId = Convert.ToString(this._sql.Ministry_Activities_FetchInstanceID(churchId, Convert.ToInt32(activityId)));
            this._sql.IndividualInstance_Checkin_Update(churchId, Convert.ToInt32(activityInstanceId), Convert.ToInt32(individualId), checkInMachineId, checkInTime);

        }
        public void UpdateIndividual_Instance(String activityName, String individualId, int checkInMachineId, String checkInTime, String activityInstanceId)
        {
            APIBase api = new APIBase();
            int churchId = this._sql.Ministry_Church_FetchID(this._churchCode);
            String activityId = Convert.ToString(this._sql.Ministry_Activities_FetchID(churchId, activityName));
            this._sql.IndividualInstance_Checkin_Update(churchId, Convert.ToInt32(activityInstanceId), Convert.ToInt32(individualId), checkInMachineId, checkInTime);

        }
        public void CreateLastAttendance(int churchId, string activityName, string studentName, string rosterName)
        {

            String timeZoneName = this._sql.Ministry_Activity_Instance_TimeZone(churchId);
            DateTime  currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            int interalHour = 24 - currentTimzoneTime.Hour - 1;
            this._sql.Ministry_Activity_CreateLastAttendance(churchId, activityName, studentName, rosterName, currentTimzoneTime.AddDays(-1).AddHours(0), currentTimzoneTime.AddDays(-1).AddHours(interalHour));
           
        }
        public void DeleteLastAttendance(int churchId, string studentName)
        {
            this._sql.Ministry_Activity_DeleteLastAttendance(churchId, studentName);

        }
        public String[] getExpectBirthdayMD(int churchId)
        {
            String timeZoneName = this._sql.Ministry_Activity_Instance_TimeZone(churchId);
            DateTime currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            String[] birthdayMD = { "", "" };
            String month = "";
            if (currentTimzoneTime.Month <= 9)
            {
                month = "0" + Convert.ToString(currentTimzoneTime.Month);
            }
            if (currentTimzoneTime.Month >= 10)
            {
                month = Convert.ToString(currentTimzoneTime.Month);
            }
            String day = "";
            if (currentTimzoneTime.Day <= 9)
            {
                day = "0" + Convert.ToString(currentTimzoneTime.Day);
            }
            if (currentTimzoneTime.Day >= 10)
            {
                day = Convert.ToString(currentTimzoneTime.Day);
            }
            birthdayMD[0] = month;
            birthdayMD[1] = day;
            return birthdayMD;
        }
        public void UpdateBirthdayForCheckinTeacher(int churchId, string studentName)
        {
            String tempBirthDay = this._sql.People_Individuals_FetchBirthdayByName(churchId, studentName);
            if (tempBirthDay == null || tempBirthDay == "")
            {
                TestLog.WriteLine("birthday is null or empty");
                return;
            }
            string year = tempBirthDay.Split(' ')[0].Split('-')[0];
            
            String[] birthdayMD = getExpectBirthdayMD(churchId);
            String birthDay = year + "-" + birthdayMD[0] + "-" + birthdayMD[1] + " 00:00:00.000";

            this._sql.People_Individuals_UpdateBirthdayByName(churchId, studentName, birthDay);

        }
        
        public String getExpectBirthdayMDInfo(int churchId)
        {
            String timeZoneName = this._sql.Ministry_Activity_Instance_TimeZone(churchId);
            DateTime currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            String monthInfo = "";
            switch (currentTimzoneTime.Month) 
            {
                case 1:
                    monthInfo = "January";
                    break;
                case 2:
                    monthInfo = "February";
                    break;
                case 3:
                    monthInfo = "March";
                    break;
                case 4:
                    monthInfo = "April";
                    break;
                case 5:
                    monthInfo = "May";
                    break;
                case 6:
                    monthInfo = "June";
                    break;
                case 7:
                    monthInfo = "July";
                    break;
                case 8:
                    monthInfo = "August";
                    break;
                case 9:
                    monthInfo = "September";
                    break;
                case 10:
                    monthInfo = "October";
                    break;
                case 11:
                    monthInfo = "November";
                    break;
                case 12:
                    monthInfo = "December";
                    break;
                default :
                    break;
            }

            return monthInfo + " "+Convert.ToString(currentTimzoneTime.Day);
        }
        public String getExpectLastAttendInfo(int churchId)
        {
            String timeZoneName = this._sql.Ministry_Activity_Instance_TimeZone(churchId);
            DateTime currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            int interalHour = 24 - currentTimzoneTime.Hour - 1;
            DateTime yesterdayTimzoneTime = currentTimzoneTime.AddDays(-1);        
            String month = "";
            //if (yesterdayTimzoneTime.Month <= 9)
            //{
                month = "0" + Convert.ToString(yesterdayTimzoneTime.Month);
            //}
            //else
            //{
                month = Convert.ToString(yesterdayTimzoneTime.Month);
            //}
            String day = "";
            // if (yesterdayTimzoneTime.Day <= 9)
            // {
               // day = "0" + Convert.ToString(yesterdayTimzoneTime.Day);
            //}
            // if (yesterdayTimzoneTime.Day >= 10)
            //{
                day = Convert.ToString(yesterdayTimzoneTime.Day);
            //}
            
            return "(Last Attended " + month + "/" + day + "/" + Convert.ToString(yesterdayTimzoneTime.Year).Substring(2) + ")";
        }
        public String getExpectOnSiteTime(int churchId, string activityName, string individualName)
        {
            String checkOnsiteTime = this._sql.Individual_Instance_FetchCheckInTime(churchId, activityName, individualName);
            String time = checkOnsiteTime.Split(' ')[1];
            String[] timeArray = time.Split(':');
            String timeFlag = checkOnsiteTime.Split(' ')[2];
            return timeArray[0] + ":" + timeArray[1] + " " + timeFlag.ToLower();
        }
        public String getExpectInTime(int churchId, string activityName, string individualName)
        {
            String checkInTime = this._sql.Individual_Instance_FetchPresentTime(churchId, activityName, individualName);
            String time = checkInTime.Split(' ')[1];
            String[] timeArray = time.Split(':');
            String timeFlag = checkInTime.Split(' ')[2];
            return timeArray[0] + ":" + timeArray[1] + " " + timeFlag.ToLower();
        }
        public String getExpectOutTime(int churchId, string activityName, string individualName)
        {
            String checkOutTime = this._sql.Individual_Instance_FetchCheckOutTime(churchId, activityName, individualName);
            String time = checkOutTime.Split(' ')[1];
            String[] timeArray = time.Split(':');
            String timeFlag = checkOutTime.Split(' ')[2];
            return timeArray[0] + ":" + timeArray[1] + " " + timeFlag.ToLower();
        }

        #endregion generateActiveCheckInJson / get response's info

        #region generateRealtimeNoticeJson
        public String GenerateRealtimeNoticeJson(String individualInstanceId,String activityName, String individualId, String individualTypeId, String rosterName, String oldActivityInstanceId, String OldActivityDetailId, String OperType)
        {
            APIBase api = new APIBase();
            int churchId = this._sql.Ministry_Church_FetchID(this._churchCode);
            int activityId = this._sql.Ministry_Activities_FetchID(churchId, activityName);
            if (-1 == activityId)
            {
                Assert.Fail("Activity: " + activityName + " can't be found as expected");
            }
            String activityInstanceId = Convert.ToString(this._sql.Ministry_Activities_FetchInstanceID(churchId, activityId));
            String activityDetailId = Convert.ToString(this._sql.Ministry_Activities_FetchRosterIDByName(churchId, activityId, rosterName));
            String churchCode = this._churchCode;
            String enviroment = GetEnvironmentInUse(this._f1Environment);
                   
            return api.GenerateRealtimeNoticeJson(individualInstanceId, activityInstanceId, activityDetailId, individualId, oldActivityInstanceId, OldActivityDetailId, OperType, individualTypeId, Convert.ToString(churchId));

        }
        public String GenerateRealtimeNoticeJson(String activityName, String individualId, String individualTypeId, String rosterName, String oldActivityInstanceId, String OldActivityDetailId, String OperType)
        {
            APIBase api = new APIBase();
            int churchId = this._sql.Ministry_Church_FetchID(this._churchCode);
            int activityId = this._sql.Ministry_Activities_FetchID(churchId, activityName);
            if (-1 == activityId)
            {
                Assert.Fail("Activity: " + activityName + " can't be found as expected");
            }
            String activityInstanceId = Convert.ToString(this._sql.Ministry_Activities_FetchInstanceID(churchId, activityId));
            String activityDetailId = Convert.ToString(this._sql.Ministry_Activities_FetchRosterIDByName(churchId, activityId, rosterName));
            String churchCode = this._churchCode;
            String enviroment = GetEnvironmentInUse(this._f1Environment);
            String individualInstanceId = Convert.ToString(this._sql.Ministry_Activities_FetchIndividualInstanceID(churchId, Convert.ToInt32(activityInstanceId), Convert.ToInt32(individualId), Convert.ToInt32(activityDetailId)));
            return api.GenerateRealtimeNoticeJson(individualInstanceId, activityInstanceId, activityDetailId, individualId, oldActivityInstanceId, OldActivityDetailId, OperType, individualTypeId, Convert.ToString(churchId));

        }
        public void RealtimeNoticeCheckin(int churchId, string activityName, string individualId, String individualTypeId, string rosterToCheckIn, String oldActivityInstanceId, String OldActivityDetailId, String OperType)
        {
            //Teacher Check-in
            String url = this.GetRealTimeNoticeURl();
            String json = this.GenerateRealtimeNoticeJson(activityName, individualId, individualTypeId, rosterToCheckIn, oldActivityInstanceId, OldActivityDetailId, OperType);
            TestLog.WriteLine("-createRealTimeUrl = {0}", url);
            TestLog.WriteLine("-createRealTimeJson = {0}", json);

            APIBase api = new APIBase();
            String responseString = api.SendAPIRequestwithBodyNoAuth(url, "POST", json, HttpStatusCode.NoContent);
            TestLog.WriteLine("-createRealTimeResponseString = {0}", responseString);
            
        }
        public void RealtimeNoticeCheckin(int churchId, String individualInstanceId, string activityName, string individualId, String individualTypeId, string rosterToCheckIn, String oldActivityInstanceId, String OldActivityDetailId, String OperType)
        {
            //Teacher Check-in
            String url = this.GetRealTimeNoticeURl();
            String json = this.GenerateRealtimeNoticeJson(individualInstanceId,activityName, individualId, individualTypeId, rosterToCheckIn, oldActivityInstanceId, OldActivityDetailId, OperType);
            TestLog.WriteLine("-createRealTimeUrl = {0}", url);
            TestLog.WriteLine("-createRealTimeJson = {0}", json);

            APIBase api = new APIBase();
            String responseString = api.SendAPIRequestwithBodyNoAuth(url, "POST", json, HttpStatusCode.NoContent);
            TestLog.WriteLine("-createRealTimeResponseString = {0}", responseString);

        }
        public void RealTimeDeleteIndividualInstance(String activityName, String individualId, String individualTypeId, String rosterName)
        {
            APIBase api = new APIBase();
            int churchId = this._sql.Ministry_Church_FetchID(this._churchCode);
            int activityId = this._sql.Ministry_Activities_FetchID(churchId, activityName);
            if (-1 == activityId)
            {
                Assert.Fail("Activity: " + activityName + " can't be found as expected");
            }
            String activityInstanceId = Convert.ToString(this._sql.Ministry_Activities_FetchInstanceID(churchId, activityId));
            String activityDetailId = Convert.ToString(this._sql.Ministry_Activities_FetchRosterIDByName(churchId, activityId, rosterName));
            String individualInstanceID = Convert.ToString(this._sql.Ministry_Activities_FetchIndividualInstanceID(churchId, Convert.ToInt32(activityInstanceId), Convert.ToInt32(individualId), Convert.ToInt32(activityDetailId)));
            this._sql.Ministry_Activities_DeleteIndividualInstanceID(churchId, Convert.ToInt32(activityInstanceId), Convert.ToInt32(individualId), Convert.ToInt32(activityDetailId));
            RealtimeNoticeCheckin(churchId, individualInstanceID, activityName, individualId, individualTypeId, rosterName, activityInstanceId, activityDetailId, "2");

        }
        #endregion generateRealtimeNoticeJson

        #region Methods
        public string GetCheckInURL(F1Environments f1Environment)
        {

            string returnValue = string.Empty;

            //NOTE:later replace CheckIn's loginUrl
            switch (f1Environment)
            {
                case F1Environments.LOCAL:
                    returnValue = "http://teacher.local/login.aspx";
                    break;
                case F1Environments.QA:
                case F1Environments.LV_QA:
                case F1Environments.INT:
                case F1Environments.INT2:
                case F1Environments.INT3:
                case F1Environments.LV_UAT:
                case F1Environments.STAGING:
                    returnValue = string.Format("https://teacher.{0}.fellowshipone.com/", GetEnvironmentInUse(f1Environment));
                    break;
                case F1Environments.LV_PROD:
                case F1Environments.PRODUCTION:
                    returnValue = "https://teacher.fellowshipone.com/";
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
                    returnValue = "PROD";
                    break;
                default:
                    throw new Exception(string.Format("[{0}] is Not a valid LV environment option!", f1Environment));

            }

            return returnValue.ToLower();

        }

        #endregion Static Methods

        #region SetBrowserSizeTo common Mobile

        public void SetBrowserSizeTo_Mobile()
        {
            // Set window size to simulate iPhone 5
            this._driver.Manage().Window.Size = new Size(375, 667);

        }

        public void SetBrowserSizeTo_Pad()
        {
            // Set window size to simulate iPhone
            this._driver.Manage().Window.Size = new Size(1536, 1048);

        }

        #endregion SetBrowserSizeTo common Mobile

        public object SECONDS { get; set; }
    }
}