using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Selenium;

using Gallio.Common;
using Gallio.Framework;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Common.Reflection;
using MbUnit.Framework;

using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Collections.Concurrent;

using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;



using log4net;
using log4net.Config;

using ActiveUp.Net.Mail;
using System.Net;
using System.Collections;
using System.Linq;

namespace FTTests {

    //Only Runs In Staging
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = true, Inherited = true)]
    public class DoesNotRunOutsideOfStaging : TestDecoratorAttribute {

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ExeConfigurationFileMap _appConfigFileMap = new ExeConfigurationFileMap();
        public ConfigurationFileMap AppConfigFileMap {
            get { return _appConfigFileMap; }
        }

        private Configuration _configuration;
        protected Configuration Configuration {
            get { return _configuration; }
        }

        protected override void Initialize(PatternTestInstanceState testInstanceState) {


            log.Debug("Initialize ... Load Variables");

            _appConfigFileMap.ExeConfigFilename = @"..\..\..\Common\bin\Debug\Common.dll.config";
            _configuration = ConfigurationManager.OpenMappedExeConfiguration(_appConfigFileMap, ConfigurationUserLevel.None);

            if (_configuration.AppSettings.Settings["FTTests.Environment"].Value != "STAGING")
            {
                log.Warn("This test will not run outside of the STAGING environment.");
                Assert.TerminateSilently(Gallio.Model.TestOutcome.Ignored, "This test will not run outside of the STAGING environment.");
            }

            base.Initialize(testInstanceState);

            log.Debug("Exit Initialize");

        }
    }

    //Does not Run in Staging
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = true, Inherited = true)]
    public class DoesNotRunInStaging : TestDecoratorAttribute
    {

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ExeConfigurationFileMap _appConfigFileMap = new ExeConfigurationFileMap();
        public ConfigurationFileMap AppConfigFileMap
        {
            get { return _appConfigFileMap; }
        }

        private Configuration _configuration;
        protected Configuration Configuration
        {
            get { return _configuration; }
        }

        protected override void Initialize(PatternTestInstanceState testInstanceState)
        {


            log.Debug("Initialize ...  Not in Staging ... Load Variables");
            

            _appConfigFileMap.ExeConfigFilename = @"..\..\..\Common\bin\Debug\Common.dll.config";
            _configuration = ConfigurationManager.OpenMappedExeConfiguration(_appConfigFileMap, ConfigurationUserLevel.None);

            if (_configuration.AppSettings.Settings["FTTests.Environment"].Value == "STAGING")
            {
                log.Warn("This test will not run in the STAGING environment.");
                Assert.TerminateSilently(Gallio.Model.TestOutcome.Ignored, "This test will not run on the STAGING environment.");
            }

            base.Initialize(testInstanceState);

            log.Debug("Exit Not in Staging Initialize");

        }
    }


    /// <summary>
    /// This sets test cases/fixture not to be executed in Jenkins (master/slave) servers.
    /// Any test cases using Email verification or Captcha will be candidates for this category
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = true, Inherited = true)]
    public class DoesNotRunInJenkins : TestDecoratorAttribute
    {

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ExeConfigurationFileMap _appConfigFileMap = new ExeConfigurationFileMap();
        public ConfigurationFileMap AppConfigFileMap
        {
            get { return _appConfigFileMap; }
        }

        private Configuration _configuration;
        protected Configuration Configuration
        {
            get { return _configuration; }
        }

        protected override void Initialize(PatternTestInstanceState testInstanceState)
        {


            log.Debug("Initialize ... Not run in Jenkins ... Load Variables");

            _appConfigFileMap.ExeConfigFilename = @"..\..\..\Common\bin\Debug\Common.dll.config";
            _configuration = ConfigurationManager.OpenMappedExeConfiguration(_appConfigFileMap, ConfigurationUserLevel.None);

            if (_configuration.AppSettings.Settings["FTTests.Jenkins"].Value == "true")
            {
                log.Warn("This test will not run in Jenkins. Please execute manually through Gallio GUI.");
                Assert.TerminateSilently(Gallio.Model.TestOutcome.Ignored, "This test will not run in Jenkins. Please execute manually through Gallio GUI.");
            }

            base.Initialize(testInstanceState);

            log.Debug("Exit Jenkins Initialize");

        }
    }

    /// <summary>
    /// Decorates a test method and causes it to be re-run following failure until we get a pass.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each repetition of the test method will occur within its own individually labeled
    /// test step so that it can be identified in the test report.
    /// </para>
    /// <para>
    /// The initialize, setup, teardown and dispose methods will are invoked around each
    /// repetition of the test.
    /// </para>
    /// </remarks>
    /// <seealso cref="RepeatAttribute"/>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = true, Inherited = true)]
    public class RepeatOnFailureAttribute : TestDecoratorPatternAttribute
    {
        //private readonly int _maxNumberOfAttempts;
        private int _maxNumberOfAttempts;

        /// <summary>
        /// Will re-run the test method each time we get a failure for a limited number of attempts.
        /// </summary>
        /// <example>
        /// <code><![CDATA[
        /// [Test, RepeatOnFailure]
        /// [RepeatOnFailure(3)]
        /// public void Test()
        /// {
        ///     // This test will be executed until we get a pass or have run it 3 times.
        ///     // Eg, if the first test run fails, we will run it again, and if the second attempt passes, then we will stop.
        ///     // if 3 attempts all fail, we dont try anymore
        /// }
        /// ]]></code>
        /// </example>
        /// <param name="maxNumberOfAttempts">The number of times to repeat the test while searching for a pass</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxNumberOfAttempts"/>
        /// is less than 1.</exception>
        public RepeatOnFailureAttribute() { }

        /// <inheritdoc />
        protected override void DecorateTest(IPatternScope scope, ICodeElementInfo codeElement)
        {
            scope.TestBuilder.TestInstanceActions.RunTestInstanceBodyChain.Around(delegate(PatternTestInstanceState state, Gallio.Common.Func<PatternTestInstanceState, TestOutcome> inner)
            {

                this.setMaxNumberOfAttempts();

                string TCName = Gallio.Framework.TestStep.CurrentStep.Name;
                //TestLog.WriteLine("TC Name: " + TCName);

                TestOutcome outcome = TestOutcome.Passed;
                int failureCount = 0;
                // we will try up to 'max' times to get a pass, if we do, then break out and don't run the test anymore
                for (int i = 0; i < _maxNumberOfAttempts; i++)
                {
                    string name = String.Format("{0}:  Repetition #{1}", TCName, i + 1);
                    TestContext context = TestStep.RunStep(name, delegate
                    {
                        TestOutcome innerOutcome = inner(state);
                        // if we get a fail, and we have used up the number of attempts allowed to get a pass, throw an error
                        if (innerOutcome.Status != TestStatus.Passed)
                        {
                            throw new SilentTestException(innerOutcome);
                            //SilentTestException ste = new SilentTestException(innerOutcome);                                                       
                                      
                        }
                    }, null, false, codeElement);
                    //}, TimeSpan.FromSeconds(500), false, codeElement);


                    outcome = context.Outcome;
                        
                    // escape the loop if the test has passed, otherwise increment the failure count
                    if (context.Outcome.Status == TestStatus.Passed)
                        break;

                       failureCount++;
                }

                
                TestLog.WriteLine(String.Format(
                        failureCount == _maxNumberOfAttempts
                            ? "Failed on attempt {0} out of {0}"
                            : "The test passed on attempt {1} out of {0}", _maxNumberOfAttempts, failureCount+1));
                

                return outcome;
            });
        }

        private void setMaxNumberOfAttempts()
        {

            ExeConfigurationFileMap _appConfigFileMap = new ExeConfigurationFileMap();
            Configuration _configuration;
            int repeatOnFail = 0;

            _appConfigFileMap.ExeConfigFilename = @"..\..\..\Common\bin\Debug\Common.dll.config";
            _configuration = ConfigurationManager.OpenMappedExeConfiguration(_appConfigFileMap, ConfigurationUserLevel.None);
            
            if(_configuration.AppSettings.Settings["FTTests.RepeatOnFail"].Value != null){
                repeatOnFail = Convert.ToInt32(_configuration.AppSettings.Settings["FTTests.RepeatOnFail"].Value);
                _maxNumberOfAttempts = repeatOnFail;
            }

            //TestLog.WriteLine("Repeat On Fail: " + repeatOnFail);      

        }
    }

    public class TestBaseWebDriver{

        private ScreenShotRemoteWebDriver _driver;
        private RemoteWebDriver _remoteWebDriver;
        private string _serverHost = string.Empty;
        private string _browser = string.Empty;
        private string _port = string.Empty;
        //private bool _amsEnabled = false;
        private Dictionary<int, string> _amsChurches = new Dictionary<int, string>();
        private F1Environments _f1Environment;
        private GeneralMethods _generalMethods;
        private AdHocReportingBase _adHocReporting;
        private ReportLibraryBase _reportLibrary;
        private PortalBase _portal;
        private InFellowshipBase _infellowship;
        private WeblinkBase _weblink;
        // CheckIn add by Grace Zhang
        private CheckInTeacherBase _checkIn;
        private SelfCheckInBase _selfCheckIn;
        private DashboardBase _dashboard;
        private Configuration _configuration;
        private APIBase _api;
        private SQL _sql;
        private JavaScript _javascript;
        private IJavaScriptExecutor _javaScriptExecutor;

        //FGJ log4net
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Properties

        public IJavaScriptExecutor JavaScriptExecutor
        {
            get { return (IJavaScriptExecutor)_driver; }
        }

        public ScreenShotRemoteWebDriver Driver
        {
            get { return (ScreenShotRemoteWebDriver) _driver; }
        }

        public ScreenShotRemoteWebDriver ScreenShotDriver
        {
            get { return _driver; }
        }
        
        public RemoteWebDriver RemoteWebDriver
        {
            get { return (RemoteWebDriver) _remoteWebDriver; }
        }
        
        public Configuration Configuration {
            get { return _configuration; }
        }

        public AdHocReportingBase AdHocReporting {
            get { return _adHocReporting; }
        }

        public ReportLibraryBase ReportLibrary {
            get { return _reportLibrary; }
        }

        public PortalBase Portal {
            get { return _portal; }
        }
        // CheckIn add by Grace Zhang
        public CheckInTeacherBase CheckIn
        {
            get { return _checkIn; }
        }
        public SelfCheckInBase SelfCheckIn
        {
            get { return _selfCheckIn; }
        }
        public DashboardBase Dashboard
        {
            get { return _dashboard; }
        }

        public WeblinkBase Weblink {
            get { return _weblink; }
        }

        public GeneralMethods GeneralMethods {
            get { return _generalMethods; }
        }

        public SQL SQL
        {
            get { return _sql; }
        }

        public APIBase API
        {

            get { return _api; }
        }

        public InFellowshipBase Infellowship {
            get { return _infellowship; }

        }

        public JavaScript JavaScript
        {
            get { return _javascript; }
        }

        /*public bool AMSEnabled
        {
            get { return this._amsEnabled; }
        }
        */

        /*public Dictionary<int, string> AMSChurches
        {
            get { return this._amsChurches; }
        }
        */

        #endregion Properties

        public TestBaseWebDriver(F1Environments _f1Environment, string _dbConnectionString, Configuration _configuration, SQL _sql, APIBase _api)               
        {


            log.Debug("Enter Test Base Web Driver");
            DesiredCapabilities dc;
            
            bool amsEnabled = false;
            string amsChurchValues = string.Empty;

            // Configure the config file
            this._configuration = _configuration;

            log.Debug("Set Environments");
            // Environment variables
            this._serverHost = _configuration.AppSettings.Settings["FTTests.Host"].Value;
            this._browser = _configuration.AppSettings.Settings["FTTests.Browser"].Value;
            this._port = _configuration.AppSettings.Settings["FTTests.Port"].Value;
            if (string.IsNullOrWhiteSpace(_configuration.AppSettings.Settings["FTTests.AMSEnabled"].Value))
            {
                amsEnabled = false;
            }
            else
            {
                amsEnabled = Boolean.Parse(_configuration.AppSettings.Settings["FTTests.AMSEnabled"].Value);
            }

            amsChurchValues = _configuration.AppSettings.Settings["FTTests.AMSChurches"].Value;
            
            this._f1Environment = _f1Environment;

            log.Info(string.Format("Environment: {0}", this._f1Environment));
            log.Info(string.Format("Host: {0}", this._serverHost));            
            log.Info(string.Format("Browser: {0}", this._browser));
            log.Info(string.Format("Port: {0}", this._port));
            log.Info(string.Format("AMS Enabled: {0}", amsEnabled));
            log.Info(string.Format("AMS Churches: {0}", amsChurchValues));

            //string chromeDriverLoc = (this._serverHost.Contains("localhost")) ? @"chromedriver.exe" : @"..\..\..\chromedriver.exe";
            //string ieDriverLoc = (this._serverHost.Contains("localhost")) ? @"IEDriverServer.exe" : @"..\..\..\IEDriverServer.exe";
            string chromeDriverLoc = @"..\..\..\chromedriver.exe";
            string ieDriverLoc = @"..\..\..\IEDriverServer.exe";


            // Create the Selenium object. We are using composition to establish a HAS-A relationship between TestBase and Selenium object            
            if( (this._serverHost.Equals("localhost"))  || (this._serverHost.Equals("local")) )
            {
                this._serverHost = string.Format("http://localhost:{0}/wd/hub", this._port);
            }
            else
            {
                this._serverHost = string.Format("http://{0}:{1}/wd/hub", this._serverHost, this._port);
            }


            log.Info("Server Host: " + this._serverHost);
            
            
                switch (this._browser)
                {

                    case "*firefox":
                    case "*Firefox":
                    case "Firefox":
                    case "ff":
                    case "FF":

                        dc = DesiredCapabilities.Firefox();                        
                        break;

                    case "chrome":
                    case "*chrome":
                    case "Chrome":
                    case "*googlechrome":

                        System.Environment.SetEnvironmentVariable("webdriver.chrome.driver", chromeDriverLoc);
                        dc = DesiredCapabilities.Chrome();
                        break;

                    case "internet explorer" :
                    case "*iehta":
                    case "ie":
                    case "*ie":
                    case "Internet Explorer":
                    case "*iexplore":

                        System.Environment.SetEnvironmentVariable("webdriver.ie.driver", ieDriverLoc);                
                        dc = DesiredCapabilities.InternetExplorer();
                        break;

                    default:
                        throw new Exception(this._browser + " not supported. Chose between chrome, Chrome, firefox, ff, ie, internet explorer");
                }
                dc.IsJavaScriptEnabled = true;

                try
                {
                    log.Debug("Instantiate ScreenShotRemoteWebDriver");
                    this._driver = new ScreenShotRemoteWebDriver(new Uri(this._serverHost), dc, TimeSpan.FromSeconds(300));
                    //this._driver = new ScreenShotRemoteWebDriver(new Uri(this._serverHost), dc);
                    log.Debug("ScreenShotRemoteWebDriver launched");
                }
                catch (Exception e)
                {
                    if (this._driver != null)
                    {
                        log.DebugFormat("Forced a quit. Hopefully it cleared up the browser");
                        this._driver.Quit();
                    }

                    throw new SeleniumException(string.Format("Can't instantitate WebDriver. Selenium Grid issue is most likely the culprit. \n{0} \n{1} \n{2}", 
                                                 e.Message, e.StackTrace, e.InnerException));
                }

            // Set the SQL class
            this._sql = _sql;

            // Create a new JavaScript class. By composition, we establish as HAS-A relationship between TestBase and JavaScript
            this._javascript = new JavaScript();

            // Set the API class
            this._api = new APIBase(this._driver);

            // Create the generic methods class
            this._generalMethods = new GeneralMethods(this._driver, this._f1Environment, this._javascript);

            // Adhoc Reporting
            this._adHocReporting = new AdHocReportingBase(this._driver);
            this._adHocReporting.AdHocReportingUserName = this._configuration.AppSettings.Settings["FTTests.PortalUsername"].Value;
            this._adHocReporting.AdHocReportingPassword = this._configuration.AppSettings.Settings["FTTests.PortalPassword"].Value;
            this._adHocReporting.AdHocReportingChurchCode = this._configuration.AppSettings.Settings["FTTests.ChurchCode"].Value;

            // Report Library
            this._reportLibrary = new ReportLibraryBase(this._driver, this._generalMethods);
            this._reportLibrary.ReportLibraryUserName = this._configuration.AppSettings.Settings["FTTests.PortalUsername"].Value;
            this._reportLibrary.ReportLibraryPassword = this._configuration.AppSettings.Settings["FTTests.PortalPassword"].Value;
            this._reportLibrary.ReportLibraryChurchCode = this._configuration.AppSettings.Settings["FTTests.ChurchCode"].Value;

            // Portal
            this._portal = new PortalBase(this._driver, this._generalMethods, this._f1Environment, this._sql);
            this._portal.PortalUsername = this._configuration.AppSettings.Settings["FTTests.PortalUsername"].Value;
            this._portal.PortalPassword = this._configuration.AppSettings.Settings["FTTests.PortalPassword"].Value;
            this._portal.ChurchCode = this._configuration.AppSettings.Settings["FTTests.ChurchCode"].Value;

            // InFellowship
            this._infellowship = new InFellowshipBase(this._driver, this._f1Environment, this._generalMethods, this._javascript, this._sql);
            this._infellowship.InFellowshipUser = Configuration.AppSettings.Settings["FTTests.InFellowshipUser"].Value;
            this._infellowship.InFellowshipEmail = Configuration.AppSettings.Settings["FTTests.InFellowshipEmail"].Value;
            this._infellowship.InFellowshipPassword = Configuration.AppSettings.Settings["FTTests.InFellowshipPassword"].Value;
            this._infellowship.InFellowshipChurchCode = Configuration.AppSettings.Settings["FTTests.InFellowshipChurchCode"].Value;

            // Weblink
            this._weblink = new WeblinkBase(this._driver, this._portal.PortalUser, _generalMethods, this._f1Environment);
            this._weblink.WebLinkPassword = Configuration.AppSettings.Settings["FTTests.WebLinkPassword"].Value;
            this._weblink.EncryptedChurchCode = Configuration.AppSettings.Settings["FTTests.WeblinkChurchCode"].Value;
            this._weblink.WeblinkEmail = Configuration.AppSettings.Settings["FTTests.Email"].Value;

            // CheckIn add by Grace Zhang
            this._checkIn = new CheckInTeacherBase(this._driver, this._generalMethods, this._f1Environment, this._sql);
            this._checkIn.CheckInUsername = this._configuration.AppSettings.Settings["FTTests.CheckInUsername"].Value;
            this._checkIn.CheckInPassword = this._configuration.AppSettings.Settings["FTTests.CheckInPassword"].Value;
            this._checkIn.ChurchCode = this._configuration.AppSettings.Settings["FTTests.CheckInChurchCode"].Value;

            this._selfCheckIn = new SelfCheckInBase(this._driver, this._generalMethods, this._f1Environment, this._sql);
            this._selfCheckIn.SelfCheckInUsername = this._configuration.AppSettings.Settings["FTTests.SelfCheckInUsername"].Value;
            this._selfCheckIn.SelfCheckInPassword = this._configuration.AppSettings.Settings["FTTests.SelfCheckInPassword"].Value;
            this._selfCheckIn.ChurchCode = this._configuration.AppSettings.Settings["FTTests.SelfCheckInChurchCode"].Value;
            this._selfCheckIn.SelfCheckInUser = this._configuration.AppSettings.Settings["FTTests.SelfCheckInUser"].Value;
            
            //Dashboard
            this._dashboard = new DashboardBase(this._driver, this._generalMethods, this._f1Environment, this._sql);
            this._dashboard.DashboardUsername = this._configuration.AppSettings.Settings["FTTests.DashboardUsername"].Value;
            this._dashboard.DashboardPassword = this._configuration.AppSettings.Settings["FTTests.DashboardPassword"].Value;
            this._dashboard.DashboardEmail = this._configuration.AppSettings.Settings["FTTests.DashboardEmail"].Value;
            this._dashboard.ChurchCode = this._configuration.AppSettings.Settings["FTTests.DashboardChurchCode"].Value; 

            switch (this._f1Environment) {
                case F1Environments.LOCAL:
                    break;                            

                case F1Environments.LV_QA:
                case F1Environments.LV_UAT:
                case F1Environments.INT:
                case F1Environments.INT2:
                case F1Environments.INT3:
                case F1Environments.STAGING:
                case F1Environments.LV_PROD:
                case F1Environments.PRODUCTION:
                    //TODO
                    //FOOS: https://opsmanager.qa.fellowshipone.com/opsmanager/login.aspx
                    this._weblink.URL = string.Format("https://integration.{0}.fellowshipone.com/integration", PortalBase.GetLVEnvironment(_f1Environment));
                    this._reportLibrary.URL = string.Format("https://reportlibrary.{0}.fellowshipone.com/ReportLibrary/Login/Index.aspx", PortalBase.GetLVEnvironment(_f1Environment));
                    break;
                default:
                    log.Fatal(string.Format("{0} Not a valid option!", _f1Environment));
                    this._driver.Quit();
                    throw new Exception(string.Format("{0} Not a valid option!", _f1Environment));
            }

            //log.Debug("Server Host: " + this._serverHost);
            // Maximize the window if we are running local host
            log.Info("***Maximize Window for Web Driver***");
            //this.Driver.Manage().Window.Maximize();
            this._driver.Manage().Window.Maximize();

            //Should we set page to load timeout or should we just implicitly wait for DOM to appear?
            this._driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(200)); //Set Page Load Timeout
            this._driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(30)); // Set implicit wait timeouts
            //this._driver.Manage().Timeouts().SetScriptTimeout(new TimeSpan(0, 0, 0, 30));  // Set script timeouts
            this._driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(120));  // Set script timeouts

        }

        public class ScreenShotRemoteWebDriver : RemoteWebDriver, ITakesScreenshot
        {

            public ScreenShotRemoteWebDriver(Uri RemoteAdress, ICapabilities capabilities)
                : base(RemoteAdress, capabilities)
            {
                log.Debug("ScreenShotRemote WebDriver conscructor. No Time Span");
                SessionId sessionId = base.SessionId;
                log.DebugFormat("Session ID: {0}", sessionId.ToString());

            }


            public ScreenShotRemoteWebDriver(Uri RemoteAdress, ICapabilities capabilities, TimeSpan timeSpan)
                : base(RemoteAdress, capabilities, timeSpan)
            {
                log.Debug("ScreenShotRemote WebDriver conscructor");

            }

            ~ScreenShotRemoteWebDriver()
            {
                log.Debug("ScreenShotRemote WebDriver Descructor");
                this.Quit();
            }

            /// <summary>
            /// Gets a <see cref="Screenshot"/> object representing the image of the page on the screen.
            /// </summary>
            /// <returns>A <see cref="Screenshot"/> object containing the image.</returns>
            public Screenshot GetScreenshot()
            {
                // Get the screenshot as base64.
                Response screenshotResponse = this.Execute(DriverCommand.Screenshot, null);
                string base64 = screenshotResponse.Value.ToString();

                // ... and convert it.
                return new Screenshot(base64);
            }

            public void Navigate(string p)
            {
                throw new NotImplementedException();
            }
        }

    }

    public class TestBase {
        private ISelenium _selenium;
        private string _serverHost = string.Empty;
        private string _browser = string.Empty;
        //private bool _amsEnabled = false;
        //private Dictionary<int, string> _amsChurches = new Dictionary<int, string>();
        private F1Environments _f1Environment;
        private IList<string> _errorText = new List<string>();
        private Configuration _configuration;
        private SQL _sql;
        private JavaScript _javascript;
        private PortalBase _portal;
        private InFellowshipBase _infellowship;
        private Dictionary<string, string> _webLink = new Dictionary<string,string>();
        private FOOSBase _foos;
        private ReportLibraryBase _reportLibrary;
        private string _staticFileServer = string.Empty;
        private GeneralMethods _generalMethods;
        private Int16 _port;
        private DeleteAllEmails _emails;

        //log4net
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Properties
        public ISelenium Selenium {
            get { return _selenium; }
        }

        public string StaticFileURL {
            get { return _staticFileServer; }
        }

        public Configuration Configuration {
            get { return _configuration; }
        }

        public JavaScript JavaScript {
            get { return _javascript; }
        }

        public PortalBase Portal {
            get { return _portal; }
        }

        public InFellowshipBase infellowship {
            get { return _infellowship; }
        }

        public Dictionary<string, string> Weblink {
            get { return _webLink; }
        }

        public FOOSBase FOOS {
            get { return _foos; }
        }

        public ReportLibraryBase ReportLibrary {
            get { return _reportLibrary; }
        }

        public GeneralMethods GeneralMethods {
            get { return _generalMethods; }
        }

        public SQL SQL
        {
            get { return _sql; }
        }

        public DeleteAllEmails DeleteAllEmails
        {
            get { return _emails; }
        }

        /*public bool AMSEnabled
        {
            get { return this._amsEnabled; }
        }
        */

        /*public Dictionary<int, string> AMSChurches
        {
            get { return this._amsChurches; }
        }
        */

        #endregion Properties

        public TestBase(F1Environments _f1Environment, string _dbConnectionString, Configuration _configuration, SQL _sql) {

            log.Debug("Enter Test Base");
                        
            // Configure the config file
            this._configuration = _configuration;
            bool amsEnabled = false;
            string amsChurchValues = string.Empty;

            log.Debug("Set Environments");

            // Environment variables
            this._serverHost = _configuration.AppSettings.Settings["FTTests.Host"].Value;
            this._port = Convert.ToInt16(_configuration.AppSettings.Settings["FTTests.Port"].Value);
            this._browser = _configuration.AppSettings.Settings["FTTests.Browser"].Value;
            
            if (string.IsNullOrWhiteSpace(_configuration.AppSettings.Settings["FTTests.AMSEnabled"].Value))
            {
                amsEnabled = false;
            }
            else
            {
                amsEnabled = Boolean.Parse(_configuration.AppSettings.Settings["FTTests.AMSEnabled"].Value);
            }

            amsChurchValues = _configuration.AppSettings.Settings["FTTests.AMSChurches"].Value;
            
            
            this._f1Environment = _f1Environment;

            log.Info(string.Format("Environment: {0}", this._f1Environment));
            log.Info(string.Format("Browser: {0}", this._browser));

            //Just in case it is set to localhostMulti
            //We are always going to use port 5444 for old selenium 1.0
            if (this._serverHost == ("localhost") || (this._serverHost == ("local"))) 
            {
                this._serverHost = "localhost";
                this._port = 4444;
            }

            //TODO 
            // Create the Selenium object.
            if (this._browser.Contains("chrome"))
            {
                this._browser = "*googlechrome";
                //this._browser = "*googlechrome C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
                //this._browser = @"*chrome";

                log.Info(string.Format("Browser: {0}", this._browser));
            }
            
            //if (!this._browser.Contains("firefox"))
            //{
            //    throw new SeleniumException(this._browser + " is currently not supported. Only Firefox");
            //}


            log.Info(string.Format("Host: {0}", this._serverHost));
            log.Info(string.Format("Port: {0}", this._port));
            log.Info(string.Format("Browser: {0}", this._browser));
            log.Info(string.Format("AMS Enabled: {0}", amsEnabled));
            log.Info(string.Format("AMS Churches: {0}", amsChurchValues));

            this._selenium = new DefaultSelenium(this._serverHost, this._port, this._browser, PortalBase.GetPortalURL(this._f1Environment));

            // Set the SQL class
            this._sql = _sql;

            // Create a new JavaScript class. By composition, we establish as HAS-A relationship between TestBase and JavaScript
            this._javascript = new JavaScript();

            // Create the generic methods class
            this._generalMethods = new GeneralMethods(this._selenium, this._f1Environment, this._javascript);

            //Get AMS Church Info
            //Moved this to FixtureBase
            //this._amsChurches = this.GeneralMethods.TokenizeAmsChurches(amsChurchesValue, this._sql);
            //this.GeneralMethods.EnableDisableAMSChurches(this._amsEnabled, this._sql);

            // Create and configure all the application objects. By composition, Test objects will have access to all the application objects due to a HAS-A relationship
            // Portal
            this._portal = new PortalBase(this._selenium, this._generalMethods, this._javascript, this._f1Environment, this._sql);
            this._portal.PortalUser = this._configuration.AppSettings.Settings["FTTests.PortalUser"].Value;
            this._portal.PortalUsername = this._configuration.AppSettings.Settings["FTTests.PortalUsername"].Value;
            this._portal.PortalPassword = this._configuration.AppSettings.Settings["FTTests.PortalPassword"].Value;
            this._portal.ChurchCode = this._configuration.AppSettings.Settings["FTTests.ChurchCode"].Value;

            // InFellowship
            this._infellowship = new InFellowshipBase(this._selenium, this._f1Environment, this._generalMethods , this._javascript, this._sql);
            this._infellowship.InFellowshipUser = Configuration.AppSettings.Settings["FTTests.InFellowshipUser"].Value;
            this._infellowship.InFellowshipEmail = Configuration.AppSettings.Settings["FTTests.InFellowshipEmail"].Value;
            this._infellowship.InFellowshipPassword = Configuration.AppSettings.Settings["FTTests.InFellowshipPassword"].Value;
            this._infellowship.InFellowshipChurchCode = Configuration.AppSettings.Settings["FTTests.InFellowshipChurchCode"].Value;

            // WebLink
            //this._webLink = new WeblinkBase(this._selenium, this._portal.PortalUser, _generalMethods);
            //this._webLink.WebLinkPassword = Configuration.AppSettings.Settings["FTTests.WebLinkPassword"].Value;
            //this._webLink.EncryptedChurchCode = Configuration.AppSettings.Settings["FTTests.WeblinkChurchCode"].Value;
            //this._webLink.WeblinkEmail = Configuration.AppSettings.Settings["FTTests.Email"].Value;

            // FOOS
            this._foos = new FOOSBase(this._selenium);

            // Report Library
            this._reportLibrary = new ReportLibraryBase(this._selenium, _generalMethods);
            this._reportLibrary.ReportLibraryUserName = this._configuration.AppSettings.Settings["FTTests.PortalUsername"].Value;
            this._reportLibrary.ReportLibraryPassword = this._configuration.AppSettings.Settings["FTTests.PortalPassword"].Value;
            this._reportLibrary.ReportLibraryChurchCode = this._configuration.AppSettings.Settings["FTTests.ChurchCode"].Value;

            // Set up all the urls
            switch (this._f1Environment) {
                case F1Environments.LOCAL:
                    break;

                case F1Environments.LV_QA:
                case F1Environments.LV_UAT:
                case F1Environments.INT:
                case F1Environments.INT2:
                case F1Environments.INT3:
                case F1Environments.LV_PROD:
                case F1Environments.STAGING:
                    this._foos.URL = string.Format("https://opsmanager.{0}.fellowshipone.com/opsmanager/login.aspx", PortalBase.GetLVEnvironment(_f1Environment));
                    this._reportLibrary.URL = string.Format("https://reportlibrary.{0}.fellowshipone.com/ReportLibrary/Login/Index.aspx", PortalBase.GetLVEnvironment(_f1Environment));                    
                    this._webLink.Add("URL", string.Format("https://integration.{0}.fellowshipone.com/integration", PortalBase.GetLVEnvironment(_f1Environment)));
                    //this._staticFileServer = "https://staging-static.fellowshipone.com/portal/images/";
                    //this._staticFileServer = "http://contentqa.dev.corp.local/portal/images/";
                    break;
                default:
                    log.Fatal("URL Setup error. Not a valid option!");
                    throw new Exception("URL Setup error. Not a valid option!");
            }

            _emails = new DeleteAllEmails();

            // Start the selenium server.
            log.Info("Start Selenium Server");
            this._selenium.Start();

            //NOTE: THIS is causing lot's of Internal Server Errors (500). Need to investigate
            //Wait for Server (browser to launch) to start?
            //Thread.Sleep(1000);
            this._selenium.SetTimeout("60000");
            
            // Maximize the window if we are running local host
            this._selenium.WindowMaximize();
        }

        #region Instance Methods

        #endregion Instance Methods
    }

    public class DeleteAllEmails
    {

        //log4net
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DeleteAllEmails()
        {}

        public void Delete_All_Email(string emailAddr, string password, string mailFolder)
        {

            DateTime beginTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));            
            log.Debug("Enter Delete All Email ");

            Imap4Client client = new Imap4Client();

            //Only support GMAIL
            //TODO add other e-mail services
            Assert.AreEqual(true, emailAddr.ToUpper().Contains("@GMAIL.COM"), "Only gmail is supported");
            client.ConnectSsl("imap.gmail.com", 993);

            //Verify connected
            Assert.AreEqual(true, client.IsConnected, "Not connected to Email client");
            log.Debug("Login to Address: " + emailAddr);
            client.Login(emailAddr, password);

                //Go to Mail Folder
                log.Debug(string.Format("Get mail from folder: {0}", mailFolder));
                Mailbox mails = client.SelectMailbox(mailFolder);
                log.Debug("Messages: " + mails.MessageCount);

                if (mails.MessageCount > 1)
                {

                    int[] ids = mails.Search(EmailMailBox.SearchParse.ALL);
                    log.Debug("ID: " + ids.Length);
                    if (ids.Length > 0)
                    {
                        //Trash it!
                        Message msg = null;
                        for (var i = 1; i < ids.Length; i++)
                        {
                            msg = mails.Fetch.MessageObject(ids[i]);
                            //mails.DeleteMessage(ids[1], true);
                            client.Command("capability");
                            client.Command("copy " + ids[i].ToString() + " [Gmail]/Trash");

                        }
                    }

                }

            DeleteGmailTrash(client);

            client.Disconnect();
            
            DateTime endTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            
            log.Debug("[EXIT] Time To Delete Email: " + (endTime - beginTime));

        }

        private void DeleteGmailTrash(Imap4Client client)
        {

            log.Debug("Enter Empty Trash");
            Mailbox mailbox = client.SelectMailbox(EmailMailBox.GMAIL.TRASH);
            FlagCollection flags = new FlagCollection();
            flags.Add("Deleted");
            for (int i = 1; i <= mailbox.MessageCount; i++) mailbox.AddFlagsSilent(i, flags);
            mailbox.SourceClient.Expunge();

            log.Debug("Exit Empty Trash");

        }

    }

    public class GeneralMethods {
        private ISelenium _selenium;
        private F1Environments _f1Environment;
        private JavaScript _javascript;
        private RemoteWebDriver _driver;
        //private Dictionary<int, string> _amsChurches = new Dictionary<int, string>();
        //private Dictionary<int, string> _nonAmsChurches = new Dictionary<int, string>();

        //log4net
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public F1Environments F1Environment
        {
            get { return _f1Environment; }
        }

        /*public Dictionary<int, string> AMSChurches
        {
            get { return this._amsChurches; }
        }
        */
        /*
        public Dictionary<int, string> NonAMSChurches
        {
            get { return this._nonAmsChurches; }
        }
        */

        public GeneralMethods(ISelenium selenium, F1Environments f1Environment, JavaScript javascript) {
            log.Debug("Enter General Methods Javascript");
            this._selenium = selenium;
            this._f1Environment = f1Environment;
            this._javascript = javascript;
            log.Debug("Exit General Methods Javascript");
        }

        public GeneralMethods(RemoteWebDriver driver, F1Environments f1Environment, JavaScript javascript) {
            log.Debug("Enter General Methods RemoteWebDriver");
            this._driver = driver;
            this._f1Environment = f1Environment;
            this._javascript = javascript;
            log.Debug("Exit General Methods RemoteWebDriver");            
        }

        #region Instance Methods

        /// <summary>
        /// Navigate Back
        /// </summary>
        public void Navigate_Back()
        {
            this._driver.Navigate().Back();            
        }

        public void Navigate_Portal(string menuPath) {

                log.Debug("Enter Navigate Portal: " + menuPath);

                string[] menuPathSplit = menuPath.Split('›');

                this._driver.FindElementByLinkText(menuPathSplit[0].Trim()).Click();
                this._driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(1000));
                IWebElement menuItem = this._driver.FindElement(By.XPath(string.Format("//div[@style='visibility: visible;']/dl/dt[contains(text(), '{0}')]/following-sibling::dd/a[contains(text(), '{1}')]", menuPathSplit[1].Trim(), menuPathSplit[2].Trim())));
                log.Debug("Menu Item Path: " + string.Format("//div[@style='visibility: visible;']/dl/dt[contains(text(), '{0}')]/following-sibling::dd/a[contains(text(), '{1}')]", menuPathSplit[1].Trim(), menuPathSplit[2].Trim()));
                menuItem.Click();

                if (menuPathSplit.Length > 3)
                {
                    this._driver.FindElementByLinkText(menuPathSplit[3].Trim()).Click();
                }

                log.Debug("Exit Navigate Portal: " + menuPath);

        }

        /// <summary>
        /// Fetches the row number of the item provided as it exists in an HTML table.
        /// </summary>
        /// <param name="test">The current test object.</param>
        /// <param name="tableId">The table id that the item exists in.</param>
        /// <param name="uniqueIdentifier">The unique identifier used to identify the row.</param>
        /// <param name="columnName">The column name of the item in question.</param>
        /// <param name="function">An optional parameter used to specify an xpath function to be used in the query.</param>
        /// <returns>A decimal representing the row of the table the item exists in.</returns>
        public decimal GetTableRowNumber(string tableId, string uniqueIdentifier, string columnName, [Optional] string function) {

//            log.Debug("Enter Get Table Row Number: ");

            // Verify the table is present
            this._selenium.VerifyElementPresent(tableId);

            // Find the column number of the column name
            decimal columnNumber = (this._selenium.IsElementPresent(string.Format("{0}/*/tr[*]/td[normalize-space(string(.))='{1}']", tableId, columnName)) ? this._selenium.GetElementIndex(string.Format("{0}/*/tr[*]/td[normalize-space(string(.))='{1}']", tableId, columnName)) : this._selenium.GetElementIndex(string.Format("{0}/*/tr[*]/th[normalize-space(string(.))='{1}']", tableId, columnName))) + 1;

            // Special case for InFellowship group roster
            if (tableId == TableIds.InFellowship_Groups_Roster || tableId == TableIds.People_GroupsWidget) {
                columnNumber++;
            }

            // Search for all items active or inactive, if the option exists
            if (this._selenium.IsElementPresent("ctl00_ctl00_MainContent_content_ddlActive_dropDownList")) {
                this._selenium.Select("ctl00_ctl00_MainContent_content_ddlActive_dropDownList", "All");
                this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.Search);
            }

            // Configure the common xpath
            string surroundingChar = uniqueIdentifier.Contains("'") ? '"'.ToString() : "'";
            string commonXPath = function != null ? string.Format("{0}/tbody/tr[*]/td[position()={1} and ({2}(normalize-space(string(.)), {3}{4}{3}) or {2}(normalize-space(string(./*)), {3}{4}{3}))]", tableId, columnNumber, function, surroundingChar, uniqueIdentifier) : string.Format("{0}/tbody/tr[*]/td[position()={1} and (normalize-space(string(.))={2}{3}{2} or normalize-space(string(./*))={2}{3}{2})]", tableId, columnNumber, surroundingChar, uniqueIdentifier);

            // If there are multiple pages...
            if (this._selenium.IsElementPresent("//div[@class='grid_controls']/ul/li[1]/a[text()='1']")) {
                for (int pageIndex = 1; pageIndex < this._selenium.GetXpathCount("//div[@class='grid_controls']/ul/li"); pageIndex++) {
                    this._selenium.ClickAndWaitForPageToLoad(string.Format("//div[@class='grid_controls']/ul/li[*]/a[text()='{0}']", pageIndex));

                    if (this._selenium.IsElementPresent(commonXPath)) {
//                        log.Debug("Exit Get Table Row Number");
                        return this._selenium.GetElementIndex(string.Format("{0}/ancestor::tr", commonXPath));
                    }
                }
            }
            else {
                if (this._selenium.IsElementPresent(commonXPath)) {
//                    log.Debug("Exit Get Table Row Number");
                    return this._selenium.GetElementIndex(string.Format("{0}/parent::tr", commonXPath));
                }
            }

            // If we get this far, we did not find the element
            log.Fatal("Did not find the unique identifier: " + uniqueIdentifier);
            throw new SeleniumException("Did not find the unique identifier: " + uniqueIdentifier);
        }

        /// <summary>
        /// Fetches the row number of the item provided as it exists in an HTML table.
        /// </summary>
        /// <param name="test">The current test object.</param>
        /// <param name="tableId">The table id that the item exists in.</param>
        /// <param name="uniqueIdentifier">The unique identifier used to identify the row.</param>
        /// <param name="columnName">The column name of the item in question.</param>
        /// <param name="function">An optional parameter used to specify an xpath function to be used in the query.</param>
        /// <returns>A decimal representing the row of the table the item exists in.</returns>
        public int GetTableRowNumberWebDriver(string tableId, string uniqueIdentifier, string columnName, [Optional] string function, Boolean throwException = true)
        {

//            log.Debug("Enter Get Table Row Number Web Driver");
            
            // Find the column number of the column name
            IWebElement table = tableId.Contains("//") ? this._driver.FindElementByXPath(tableId) : this._driver.FindElementById(tableId);

            System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> columns = table.FindElements(By.TagName("th")).Count > 0 ? table.FindElements(By.TagName("th")) : table.FindElements(By.TagName("td"));
            IWebElement column = columns[0];
            foreach (var col in columns) {
                if (col.Text == columnName) {
                    column = col;
                    break;
                }
            }

            //string tagName = table.FindElements(By.XPath(string.Format("//tr[*]/td[normalize-space(string(.))='{0}']", columnName))).Count > 0 ? "td" : "th";
            
            //IWebElement column = table.FindElement(By.XPath(string.Format("//tr[*]/{0}[normalize-space(string(.))='{1}']", tagName, columnName)));
            IWebElement headerRow = column.FindElement(By.XPath(".."));
            int rowNumber = table.FindElements(By.TagName("tr")).IndexOf(headerRow);
            //int rowNumber = table.FindElements(By.TagName("tr")).IndexOf(table.FindElement(By.XPath(string.Format("//tr[*]/{0}[normalize-space(string(.))='{1}']/ancestor::tr", tagName, columnName))));
            //int columnNumber = table.FindElements(By.TagName("tr"))[rowNumber].FindElements(By.TagName(tagName)).IndexOf(table.FindElements(By.TagName("tr"))[rowNumber].FindElement(By.XPath((string.Format("//{0}[normalize-space(string(.))='{1}']", tagName, columnName)))));

            string tagName = column.TagName;
            int columnNumber = headerRow.FindElements(By.TagName(tagName)).IndexOf(column);

            // Special case for InFellowship group roster
            if (tableId == TableIds.InFellowship_Groups_Roster) {
                columnNumber++;
            }

            //modify by grace zhang:This is a method for multi module utilities.Add exception protection, if the element can not find it can continue
            try
            {
                if (this._driver.FindElementsById("ctl00_ctl00_MainContent_content_ddlActive_dropDownList").Count > 0)
                {
                    new SelectElement(this._driver.FindElementById("ctl00_ctl00_MainContent_content_ddlActive_dropDownList")).SelectByText("All");
                    this._driver.FindElementById(GeneralButtons.Search).Click();
                    table = tableId.Contains("//") ? this._driver.FindElementByXPath(tableId) : this._driver.FindElementById(tableId);
                }
            }
            catch (Exception e)
            {
            }

            // Configure the common xpath
            string surroundingChar = uniqueIdentifier.Contains("'") ? '"'.ToString() : "'";
            string commonXPath = function != null ? string.Format("//td[position()={0} and ({1}(normalize-space(string(.)), {2}{3}{2}) or {1}(normalize-space(string(./*)), {2}{3}{2}))]/ancestor::tr", columnNumber, function, surroundingChar, uniqueIdentifier) : string.Format("//td[position()={0} and (normalize-space(string(.))={1}{2}{1} or normalize-space(string(./*))={1}{2}{1})]/ancestor::tr", columnNumber, surroundingChar, uniqueIdentifier);

            // If there are multiple pages...
            try
            {
                if (this._driver.FindElementsByXPath("//div[@class='grid_controls']/ul/li[1]/a[text()='1']").Count > 0) {
                    for (int pageIndex = 1; pageIndex < this._driver.FindElementsByXPath("//div[@class='grid_controls']/ul/li").Count; pageIndex++) {
                        this._driver.FindElementByXPath(string.Format("//div[@class='grid_controls']/ul/li[*]/a[text()='{0}']", pageIndex)).Click();

                        table = tableId.Contains("//") ? this._driver.FindElementByXPath(tableId) : this._driver.FindElementById(tableId);
                        var tableRowsMult = table.FindElements(By.TagName("tr"));
                        foreach (var rowMult in tableRowsMult) {
                            if (rowMult.FindElements(By.TagName("td")).Count > 0 && rowMult.FindElements(By.TagName("td")).Count >= columnNumber) {
                                string columnText = rowMult.FindElements(By.TagName("td"))[columnNumber].Text;
                                //log.Debug("********** TEXT: " + columnText);
                                if (columnText == uniqueIdentifier) {
    //                                log.Debug("Exit Get Table Row Number Web Driver");
                                    return tableRowsMult.IndexOf(rowMult);
                                }
                                else if (function == "contains") {
                                    if (columnText.Contains(uniqueIdentifier)) {
    //                                    log.Debug("Exit Get Table Row Number Web Driver");
                                        return tableRowsMult.IndexOf(rowMult);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
            //else {
            var tableRows = table.FindElements(By.TagName("tr"));
            foreach (var row in tableRows) {
                var count = row.FindElements(By.TagName("td")).Count;

                if (row.FindElements(By.TagName("td")).Count > 0 && row.FindElements(By.TagName("td")).Count >= columnNumber) {
                    string columnText = row.FindElements(By.TagName("td"))[columnNumber].Text;
                    //log.Debug("********** TEXT: " + columnText);
                    if (columnText == uniqueIdentifier) {
//                            log.Debug("Exit Get Table Row Number Web Driver");
                        return tableRows.IndexOf(row);
                    }
                    else if (function == "contains") {
                        if (columnText.Contains(uniqueIdentifier)) {
//                                log.Debug("Exit Get Table Row Number Web Driver");
                            return tableRows.IndexOf(row);
                        }
                    }
                }
            }
            //}

            // If we get this far, we did not find the element
            if (throwException)
            {
                log.Fatal("Did not find the unique identifier: " + uniqueIdentifier);
                throw new WebDriverException("Did not find the unique identifier: " + uniqueIdentifier);

            }
            else
            {

                return 0;
            }
        }

        /// <summary>
        /// Gets the table row number for a table without column headers
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="uniqueIdentifier"></param>
        /// <returns></returns>
        public int GetTableRowNumberResponsiveWebDriver(string tableId, string uniqueIdentifier)
        {
            int uniqueRow = 0;

            int rows = this.GetTableRowCountWebDriver(tableId);
            IWebElement givingTable = this._driver.FindElementByXPath(tableId);
            for (int r = 1; r < rows; r++)
            {
                string peekText = "";

                try
                {
                    peekText = givingTable.FindElements(By.TagName("tr"))[r].FindElements(By.TagName("td"))[2].Text;
                }
                catch (System.ArgumentOutOfRangeException e)
                {
                    continue;
                }

                if (peekText.Contains(uniqueIdentifier))
                {
                    uniqueRow = r;
                    break;
                }

                TestLog.WriteLine("Text: " + peekText);

            }

            return uniqueRow;
        }

        /// <summary>
        /// Gets the number of rows in a table.
        /// </summary>
        /// <param name="tableId">The table id.</param>
        /// <returns>An integer representing the number of rows in a table.</returns>
        public int GetTableRowCount(string tableId) {

//            log.Debug("Enter Get Table Row Count");
            if (tableId.Contains("//")) {
//                log.Debug("Exit Get Table Row Count");
                return (int)this._selenium.GetXpathCount(tableId + "/tbody/tr");
            }
            else {
//                log.Debug("Exit Get Table Row Count");
                return Convert.ToInt16(this._selenium.GetEval(string.Format("this.browserbot.getCurrentWindow().document.getElementById('{0}').rows.length", tableId)));
            }
        }

        /// <summary>
        /// Gets the number of rows in a table.
        /// </summary>
        /// <param name="tableId">The table id.</param>
        /// <returns>An integer representing the number of rows in a table.</returns>
        public int GetTableRowCountWebDriver(string tableId)
        {

            //            log.Debug("Enter Get Table Row Count");
            if (tableId.Contains("//"))
            {
                //                log.Debug("Exit Get Table Row Count");
                //return (int) this._selenium.GetXpathCount(tableId + "/tbody/tr");
                return this._driver.FindElements(By.XPath(tableId + "/tbody/tr")).Count;
            }
            else
            {
                log.Debug("ELSE Exit Get Table Row Count");
                //return Convert.ToInt16(this._selenium.GetEval(string.Format("this.browserbot.getCurrentWindow().document.getElementById('{0}').rows.length", tableId)));
                return Convert.ToInt16(JavaScriptMethods.GetNumberOfTableRows(tableId));
            }
        }
        /// <summary>
        /// Gets the number of columns in a table for a given row.
        /// </summary>
        /// <param name="tableId">The table id.</param>
        /// <param name="tableRow">The table row number.</param>
        /// <returns>An integer representing the number of columns in a table for a given row.</returns>
        public int GetTableColumnCount(string tableId, int tableRow) {
//            log.Debug("Enter Get Table Column Count");
            if (tableId.Contains("//")) {
                if (this._selenium.IsElementPresent(tableId + "/tbody/tr[" + (tableRow + 1) + "]/td")) {
//                    log.Debug("Exit Get Table Column Count");
                    return (int)this._selenium.GetXpathCount(tableId + "/tbody/tr[" + (tableRow + 1) + "]/td");
                }
                else if (this._selenium.IsElementPresent(tableId + "/tbody/tr[" + (tableRow + 1) + "]/th")) {
//                    log.Debug("Exit Get Table Column Count");
                    return (int)this._selenium.GetXpathCount(tableId + "/tbody/tr[" + (tableRow + 1) + "]/th");
                }
                else if (this._selenium.IsElementPresent(tableId + "/thead/tr[" + (tableRow + 1) + "]/th")) {
//                    log.Debug("Exit Get Table Column Count");
                    return (int)this._selenium.GetXpathCount(tableId + "/thead/tr[" + (tableRow + 1) + "]/th");
                }
                else if (this._selenium.IsElementPresent("//form[@id='Form1']")) {
//                    log.Debug("Exit Get Table Column Count");
                    return (int)this._selenium.GetXpathCount("//form[@id='Form1']/table/thead/tr/th");
                }
                else {
                    log.Fatal("Did not find the columns or column headers!!");
                    throw new Exception("Did not find the columns or column headers!!");
                }
            }
            else {
                return Convert.ToInt16(this._selenium.GetEval(string.Format("this.browserbot.getCurrentWindow().document.getElementById('{0}').rows[{1}].cells.length", tableId, tableRow)));
            }
        }

        /// <summary>
        /// Checks and see if an item exists in a table.
        /// </summary>
        /// <param name="tableId">The table id.</param>
        /// <param name="uniqueIdentifier">The item you are looking for inside a table..</param>
        /// <param name="columnName">The column name that this item is under.</param>
        /// <param name="function">An optional parameter that can be used to pass in a specific xpath function.</param>
        /// <returns>A boolean that represents if the item exists or not in a table.</returns>
        public bool ItemExistsInTable(string tableId, string uniqueIdentifier, string columnName, [Optional] string function) {
            // Verify table is present
            this._selenium.VerifyElementPresent(tableId);

            // Find the column number of the column name
            //decimal columnNumber = this._selenium.IsElementPresent(string.Format("{0}/tbody/tr[*]/td[normalize-space(string(.))='{1}']", tableId, columnName)) ? this._selenium.GetElementIndex(string.Format("{0}/tbody/tr[*]/td[normalize-space(string(.))='{1}']", tableId, columnName)) + 1 : this._selenium.GetElementIndex(string.Format("{0}/tbody/tr[*]/th[normalize-space(string(.))='{1}']", tableId, columnName)) + 1;
            decimal columnNumber = this._selenium.GetElementIndex(string.Format("{0}/*[self::tbody or self::thead]/tr[*]/*[self::td or self::th][normalize-space(string(.))='{1}']", tableId, columnName)) + 1;
            
            // If we are showing the avatar next to the indentifier and we are using colspan=2, update the position by one.
            if (this._selenium.IsElementPresent("//tbody/tr/td/a/img") && this._selenium.IsElementPresent("//th[@colspan=2]")) {
                columnNumber++;
            }

            string surroundingChar = uniqueIdentifier.Contains("'") ? '"'.ToString() : "'";
            //string commonXPath = string.Format("{0}/tbody/tr[*]/td[position()={1} and (normalize-space(string(.))={2}{3}{2} or normalize-space(string(./*))={2}{3}{2})]", tableId, columnNumber, surroundingChar, uniqueIdentifier);
            string commonXPath = function != null ? string.Format("{0}/tbody/tr[*]/td[position()={1} and ({2}(normalize-space(string(.)), {3}{4}{3}) or {2}(normalize-space(string(./*)), {3}{4}{3}))]", tableId, columnNumber, function, surroundingChar, uniqueIdentifier) : string.Format("{0}/tbody/tr[*]/td[position()={1} and (normalize-space(string(.))={2}{3}{2} or normalize-space(string(./*))={2}{3}{2})]", tableId, columnNumber, surroundingChar, uniqueIdentifier);

            // Search for all items active or inactive
            if (this._selenium.IsElementPresent("ctl00_ctl00_MainContent_content_ddlActive_dropDownList")) {
                this._selenium.Select("ctl00_ctl00_MainContent_content_ddlActive_dropDownList", "All");
                this._selenium.ClickAndWaitForPageToLoad(GeneralButtons.Search);
            }

            // If there are multiple pages...
            if (this._selenium.IsElementPresent("//div[@class='grid_controls']/ul[@class='grid_pagination']") && this._selenium.IsVisible("//div[@class='grid_controls']/ul[@class='grid_pagination']") && (this._selenium.GetXpathCount("//div[@class='grid_controls']/ul[@class='grid_pagination']/li") > 1)) {
                for (int pageIndex = 1; pageIndex <= this._selenium.GetXpathCount("//div[@class='grid_controls']/ul[@class='grid_pagination']/li"); pageIndex++) {
                    this._selenium.ClickAndWaitForPageToLoad(string.Format("//div[@class='grid_controls']/ul[@class='grid_pagination']/li[{0}]/a", pageIndex));

                    if (this._selenium.IsElementPresent(commonXPath)) {
                        return this._selenium.IsElementPresent(commonXPath);
                    }
                }
            }
            else {
                return this._selenium.IsElementPresent(commonXPath);
            }

            // If we get this far, we did not find the element
            return false;
        }

        /// <summary>
        /// Checks and see if an item exists in a table.
        /// </summary>
        /// <param name="tableId">The table id.</param>
        /// <param name="uniqueIdentifier">The item you are looking for inside a table..</param>
        /// <param name="columnName">The column name that this item is under.</param>
        /// <param name="function">An optional parameter that can be used to pass in a specific xpath function.</param>
        /// <returns>A boolean that represents if the item exists or not in a table.</returns>
        public bool ItemExistsInTableWebDriver(string tableId, string uniqueIdentifier, string columnName, [Optional] string function) {
            // Find the column number of the column name
            IWebElement table = tableId.Contains("//") ? this._driver.FindElementByXPath(tableId) : this._driver.FindElementById(tableId);

            string tagName = table.FindElements(By.XPath(string.Format("//tr[*]/td[normalize-space(string(.))='{0}']", columnName))).Count > 0 ? "td" : "th";
            TestLog.WriteLine("Tag Name: " + tagName);
            // /html/body/div/div[5]/div/div/div/div[2]/form/table/tbody/tr/th

            int rowNumber = table.FindElements(By.TagName("tr")).IndexOf(table.FindElement(By.XPath(string.Format("//{0}[normalize-space(string(.))='{1}']/ancestor::tr", tagName, columnName))));
            TestLog.WriteLine("Row Number: " + rowNumber);

            // /html/body/div/div[5]/div/div/div/div[2]/form/table/tbody/tr[2]
            int columnNumber = table.FindElements(By.TagName("tr"))[rowNumber].FindElements(By.TagName(tagName)).IndexOf(table.FindElements(By.TagName("tr"))[rowNumber].FindElement(By.XPath((string.Format("//{0}[normalize-space(string(.))='{1}']", tagName, columnName)))));            
            TestLog.WriteLine("Column Number: " + columnNumber);

            // If we are showing the avatar next to the indentifier and we are using colspan=2, update the position by one.
            if (table.FindElements(By.XPath("tbody/tr/td/a/img")).Count > 0 && table.FindElements(By.XPath("tbody/tr[*]/th[@colspan=2]")).Count > 0) {
                columnNumber++;
            }
            //if (this._driver.FindElementById(tableId).FindElements(By.XPath("//tbody/tr/td/a/img")).Count > 0 && this._driver.FindElementById(tableId).FindElements(By.XPath("//th[@colspan=2]")).Count > 0) {
            //    columnNumber++;
            //}

            string surroundingChar = uniqueIdentifier.Contains("'") ? '"'.ToString() : "'";
            string commonXPath = function != null ? string.Format("{0}/tbody/tr[*]/td[position()={1} and ({2}(normalize-space(string(.)), {3}{4}{3}) or {2}(normalize-space(string(./*)), {3}{4}{3}))]", tableId, columnNumber, function, surroundingChar, uniqueIdentifier) : string.Format("{0}/tbody/tr[*]/td[position()={1} and (normalize-space(string(.))={2}{3}{2} or normalize-space(string(./*))={2}{3}{2})]", tableId, columnNumber, surroundingChar, uniqueIdentifier);

            // Search for all items active or inactive
            if (this._driver.FindElementsById("ctl00_ctl00_MainContent_content_ddlActive_dropDownList").Count > 0) {
                new SelectElement(this._driver.FindElementById("ctl00_ctl00_MainContent_content_ddlActive_dropDownList")).SelectByText("All");
                this._driver.FindElementById(GeneralButtons.Search).Click();
                table = tableId.Contains("//") ? this._driver.FindElementByXPath(tableId) : this._driver.FindElementById(tableId);
            }

            // If there are multiple pages...
            IWebElement pageControl = this._driver.FindElementsByXPath("//div[@class='grid_controls']/ul[@class='grid_pagination']").Count > 0 ? this._driver.FindElementByXPath("//div[@class='grid_controls']/ul[@class='grid_pagination']") : null;

            if (pageControl != null && pageControl.Displayed && pageControl.FindElements(By.TagName("li")).Count > 1) {
                for (int pageIndex = 1; pageIndex <= this._driver.FindElementsByXPath("//div[@class='grid_controls']/ul[@class='grid_pagination']/li").Count; pageIndex++) {
                    this._driver.FindElementByXPath(string.Format("//div[@class='grid_controls']/ul[@class='grid_pagination']/li[{0}]/a", pageIndex)).Click();

                    table = tableId.Contains("//") ? this._driver.FindElementByXPath(tableId) : this._driver.FindElementById(tableId);
                    var tableRowsMult = table.FindElements(By.TagName("tr"));
                    foreach (var rowMult in tableRowsMult) {
                        if (rowMult.FindElements(By.TagName("td")).Count > 0 && rowMult.FindElements(By.TagName("td")).Count >= columnNumber) {
                            string columnText = rowMult.FindElements(By.TagName("td"))[columnNumber].Text;
                            if (columnText.Trim() == uniqueIdentifier) {
                                TestLog.WriteLine(string.Format("{0} was found", uniqueIdentifier));
                                return true;
                            }
                        }
                    }
                }
            }
            else {
                var tableRows = table.FindElements(By.TagName("tr"));
                foreach (var row in tableRows) {
                    if (row.FindElements(By.TagName("td")).Count > 0 && row.FindElements(By.TagName("td")).Count >= columnNumber) {
                        string columnText = row.FindElements(By.TagName("td"))[columnNumber].Text;
                        if (columnText.Trim() == uniqueIdentifier) {
                            TestLog.WriteLine(string.Format("{0} was found", uniqueIdentifier));
                            return true;
                        }
                    }
                }
            }

            // If we get this far, we did not find the element
            TestLog.WriteLine(string.Format("{0} was NOT found", uniqueIdentifier));
            return false;
        }

        /// <summary>
        /// Checks and see if an item exists in a table.
        /// </summary>
        /// <param name="tableId">The table id.</param>
        /// <param name="identifierVal">The item you are looking for inside a table..</param>
        /// <param name="identifierCol">The column name that this item is under.</param>
        /// <returns>A boolean that represents if the item exists or not in a table.</returns>
        public bool ItemExistsInTableResponsiveWebDriver(string tableId, string identifierVal, string identifierCol)
        {
            if (identifierCol.Equals("Met on"))
            {
                identifierVal = Convert.ToDateTime(identifierVal.Replace("at", "")).ToString("MMM dd").ToUpper();
            }

            int rows = this.GetTableRowCountWebDriver(tableId);
            IWebElement table = this._driver.FindElementByXPath(tableId);
            for (int r = 1; r < rows; r++)
            {
                string[] textArray = Regex.Split(table.FindElements(By.TagName("tr"))[r].Text, "\r\n");
                string peekText = "";
                switch (identifierCol)
                {
                    case "Percentage":
                        {
                            peekText = textArray[6];
                            break;
                        }
                    case "Met on":
                        {
                            peekText = String.Format("{0} {1}", textArray[0], textArray[1]);
                            break;
                        }
                    case "Present":
                        {
                            peekText = String.Format("{0} {1}", textArray[2], textArray[3]);
                            break;
                        }
                    case "Absent":
                        {
                            peekText = String.Format("{0} {1}", textArray[4], textArray[5]);
                            break;
                        }
                    default:
                        throw new InvalidSelectorException("Given column name: " + identifierCol + " is out of range");
                }

                log.Debug("Text: " + peekText);

                if (peekText.Equals(identifierVal))
                {
                    return true;
                }
            }

            // If we get this far, we did not find the element
            TestLog.WriteLine(string.Format("{0} was NOT found", identifierVal));
            return false;

        }

        /// <summary>
        /// Verifies only one individual returns in people search
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        public void VerifyOnlyOneIndividualInSearch(string firstName, string lastName)
        {
            int count = 0;

            int rows = this.GetTableRowCountWebDriver(TableIds.People_Individuals);
            IWebElement table = this._driver.FindElementByXPath(TableIds.People_Individuals);
            for (int r = 1; r < rows; r++)
            {
                string peekText = Regex.Split(table.FindElements(By.TagName("tr"))[r].FindElements(By.TagName("td"))[1].Text, "\r\n")[0];


                if (peekText.Equals(string.Format("{0} {1}", firstName, lastName)))
                {
                    count++;
                }

            }
            Assert.AreEqual(1, count, "There was more than one duplicate individual");
        }

        /// <summary>
        /// Verifies all the data in a table for a given row.
        /// </summary>
        /// <param name="tableId">The table id.</param>
        /// <param name="itemRow">The item row.</param>
        /// <param name="tableValues">The table values you wish to verify.</param>
        public void VerifyTableDataWebDriver(string tableId, int itemRow, Dictionary<int, string> tableValues) {
            IWebElement table = tableId.Contains("//") ? this._driver.FindElementByXPath(tableId) : this._driver.FindElementById(tableId);
            foreach (KeyValuePair<int, string> tableValue in tableValues) {
                Assert.AreEqual(tableValue.Value, table.FindElements(By.TagName("tr"))[itemRow].FindElements(By.TagName("td"))[tableValue.Key].Text);
            }
        }

        /// <summary>
        /// Selects a gear option from a gear within an HTML table.
        /// </summary>
        /// <param name="test">The current test object</param>
        /// <param name="itemRow">The row of the gear to be interacted with (raw value from GetTableRowNumber method)</param>
        /// <param name="gearOption">The gear option to be selected</param>
        public void SelectOptionFromGear(int itemRow, string gearOption, bool waitForPageToLoad = true) {
            this._selenium.Click(string.Format("//table[*]/tbody/tr[{0}]/td[*]/a[@class='gear_trigger']", itemRow + 1));
            this._selenium.Click(string.Format("//table[*]/tbody/tr[{0}]/td[*]/ul/li[*]/a/span[text()='{1}']|//table[*]/tbody/tr[{0}]/td[*]/ul/li[*]/a[text()='{1}']", itemRow + 1, gearOption));

            if (waitForPageToLoad) {
                this._selenium.WaitForPageToLoad("60000");
            }
        }

        /// <summary>
        /// Selects a gear option from a gear within an HTML table.
        /// </summary>
        /// <param name="itemRow">The row of the gear to be interacted with (raw value from GetTableRowNumber method)</param>
        /// <param name="gearOption">The gear option to be selected</param>
        public void SelectOptionFromGearWebDriver(int itemRow, string gearOption) {            
           this._driver.FindElementByXPath(string.Format("//table[*]/tbody/tr[{0}]/td[*]/a[@class='gear_trigger']|//table[*]/tbody/tr[{0}]/td[*]/div[@data-menu-type='gear']/a[text()='Options']", itemRow + 1)).Click();
           this._driver.FindElementByXPath(string.Format("//table[*]/tbody/tr[{0}]/td[*]/ul/li[*]/a/span[text()='{1}']|//table[*]/tbody/tr[{0}]/td[*]/ul/li[*]/a[text()='{1}']|//table/tbody/tr[{0}]/td[*]/div/ul/li[*]/a[contains(text(), '{1}')]", itemRow + 1, gearOption)).Click();
           
       
        }

        /// <summary>
        /// Verifies correct functionality of a given date control
        /// </summary>
        /// <param name="dateControlID">The ID of the date control in question</param>
        public void VerifyDateControl(string dateControlID) {

            // Clear data
            this._selenium.Focus(dateControlID);
            this._selenium.Type(dateControlID, "");
 

            // t
            this._selenium.KeyDown(dateControlID, "\\84");
            this._selenium.KeyUp(dateControlID, "\\84");
            Assert.AreEqual(string.Format("{0:MM/dd/yyyy}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))), this._selenium.GetValue(dateControlID));

            // mmddyy
            this.DoVerifyDateControl(dateControlID, "120574", "12/05/1974");

            this.DoVerifyDateControl(dateControlID, "021510", "02/15/2010");

            // mm.dd.yy
            this.DoVerifyDateControl(dateControlID, "12.05.74", "12/05/1974");

            this.DoVerifyDateControl(dateControlID, "02.15.10", "02/15/2010");

            // mm-dd-yy
            this.DoVerifyDateControl(dateControlID, "12-05-74", "12/05/1974");

            this.DoVerifyDateControl(dateControlID, "02-15-10", "02/15/2010");

            // mmddyyyy
            this.DoVerifyDateControl(dateControlID, "12051974", "12/05/1974");

            this.DoVerifyDateControl(dateControlID, "02152010", "02/15/2010");

            // mm/dd/yy
            this.DoVerifyDateControl(dateControlID, "12/05/74", "12/05/1974");

            this.DoVerifyDateControl(dateControlID, "02/15/10", "02/15/2010");

            // 120520
            this.DoVerifyDateControl(dateControlID, "120520", "12/05/1920");

            // Verify autocomplete is off
            Assert.IsTrue(this._selenium.IsElementPresent(string.Format("//input[@id='{0}' and @autocomplete='off']", dateControlID)));
        }

        /// <summary>
        /// Verifies correct functionality of a given date control
        /// </summary>
        /// <param name="dateControlID">The ID of the date control in question</param>
        public void VerifyDateControlWebDriver(string dateControlID) {
            // Clear data
            IWebElement dateControl = this._driver.FindElementById(dateControlID);
            dateControl.Clear();

            // t
            this.DoVerifyDateControlWebDriver(dateControlID, "t", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));

            // mmddyy
            this.DoVerifyDateControlWebDriver(dateControlID, "120574", "12/05/1974");

            this.DoVerifyDateControlWebDriver(dateControlID, "021510", "02/15/2010");

            // mm.dd.yy
            this.DoVerifyDateControlWebDriver(dateControlID, "12.05.74", "12/05/1974");

            this.DoVerifyDateControlWebDriver(dateControlID, "02.15.10", "02/15/2010");

            // mm-dd-yy
            this.DoVerifyDateControlWebDriver(dateControlID, "12-05-74", "12/05/1974");

            this.DoVerifyDateControlWebDriver(dateControlID, "02-15-10", "02/15/2010");

            // mmddyyyy
            this.DoVerifyDateControlWebDriver(dateControlID, "12051974", "12/05/1974");

            this.DoVerifyDateControlWebDriver(dateControlID, "02152010", "02/15/2010");

            // mm/dd/yy
            this.DoVerifyDateControlWebDriver(dateControlID, "12/05/74", "12/05/1974");

            this.DoVerifyDateControlWebDriver(dateControlID, "02/15/10", "02/15/2010");

            // 120520
            this.DoVerifyDateControlWebDriver(dateControlID, "120520", "12/05/1920");

            // Verify autocomplete is off
            Assert.IsTrue(this._driver.FindElementsByXPath(string.Format("//input[@id='{0}' and @autocomplete='off']", dateControlID)).Count > 0);
        }

        /// <summary>
        /// Searches for and selects an individual from the find person popup.
        /// </summary>
        /// <param name="test">The current test object</param>
        /// <param name="individual">The individual to be searched and selected</param>
        public void SelectIndividualFromFindPersonPopup(string individual) {
            this.DoSelectIndividualFromFindPersonPopup(null, individual);
        }

        /// <summary>
        /// Searches for and selects an individual from the find person popup. Allows user to specify non-common
        /// element for the find person link.
        /// </summary>
        /// <param name="test">The current test object</param>
        /// <param name="elementID">The ID of the find person link</param>
        /// <param name="individual">The individual to be searched and selected</param>
        public void SelectIndividualFromFindPersonPopup(string elementID, string individual) {
            this.DoSelectIndividualFromFindPersonPopup(elementID, individual);
        }

        /// <summary>
        /// Goes to URL using Web Driver
        /// </summary>
        /// <param name="url"></param>
        public void OpenURLWebDriver(string url)
        {

            //Attempts to open a URL
            TestLog.WriteLine("Go to " + url);
            this._driver.Navigate().GoToUrl(url);


        }

        public string GetUrl() {

            return this._driver.Url.ToString();
        }

        /// <summary>
        /// Opens a given url, expecting a HTTP 403 error.
        /// </summary>
        /// <param name="url">The url</param>
        public void OpenURLExpecting403(string url) {

            TestLog.WriteLine("Go to {0}", url);
            // Attempt to open the url
            string exceptionText = string.Empty;

            //try {
            this._selenium.Open(url);
            //}
            //catch (SeleniumException selEx) {
            //exceptionText = selEx.Message;
            //}

          //  if (this._f1Environment == F1Environments.LOCAL || this._f1Environment == F1Environments.DEV || this._f1Environment == F1Environments.QA || this._f1Environment == F1Environments.INTEGRATION) {
                //Assert.IsTrue(exceptionText.Contains("Response_Code = 403 Error_Message = Forbidden"));
            if(this._f1Environment == F1Environments.INT){
                this._selenium.VerifyTextPresent("Server Error in '/' Application.");
                this._selenium.VerifyTextPresent("User is not authorized to view the page:");
                this._selenium.VerifyTextPresent(url);
            }
            else {
                Assert.AreEqual("403 Forbidden", this._selenium.GetText("//span[@class='float_left']"));
                this._selenium.VerifyTextPresent("You are not authorized to view the requested page");
            }
        }

        /// <summary>
        /// Opens a given url, expecting a HTTP 403 error.
        /// </summary>
        /// <param name="url">The url</param>
        public void OpenURLExpecting403_WebDriver(string url)
        {
            // Attempt to open the url
            string exceptionText = string.Empty;

            //try {
            TestLog.WriteLine("Go to " + url);
            this._driver.Navigate().GoToUrl(url);
            //}
            //catch (SeleniumException selEx) {
            //exceptionText = selEx.Message;
            //}

            //if (this._f1Environment == F1Environments.LOCAL || this._f1Environment == F1Environments.DEV || this._f1Environment == F1Environments.QA || this._f1Environment == F1Environments.INTEGRATION)
            if (this._f1Environment == F1Environments.STAGING || this._f1Environment == F1Environments.LV_PROD || this._f1Environment == F1Environments.LV_UAT || this._f1Environment == F1Environments.LV_QA)
            {
                Assert.AreEqual("403 Forbidden", this._driver.FindElementByClassName("float_left").Text, "We did not go to 403 Error Page");
                this.VerifyTextPresentWebDriver("You are not authorized to view the requested page");
            }
            else
            {
                //Assert.IsTrue(exceptionText.Contains("Response_Code = 403 Error_Message = Forbidden"));
                TestLog.WriteLine("Checking Non Staging 403 Error:  " + this._driver.Url);
                this.VerifyTextPresentWebDriver("Server Error in '/' Application.");
                this.VerifyTextPresentWebDriver("User is not authorized to view the page:");
                //For some reasone new LV environments gives us a port number. Splitting this.


                Uri myuri = new Uri(url);

                string theURI = string.Empty;
                string theURL = string.Empty;

                for (int x = 0; x < myuri.Segments.Length; x++)
                {
                    if (x == 0)
                    {
                        theURL = myuri.Segments[x];
                    }
                    else
                    {

                        theURI += myuri.Segments[x];
                    }

                }

                TestLog.WriteLine(myuri.GetLeftPart(System.UriPartial.Authority));
                TestLog.WriteLine(myuri.Host);
                TestLog.WriteLine(theURL);
                TestLog.WriteLine(theURI);


                this.VerifyTextPresentWebDriver(myuri.Host);
                this.VerifyTextPresentWebDriver(theURI);

                //http://portal.qa.fellowshipone.com(:8000)?/Payment/Batch/Edit.aspxf

            }
            
        }

        /// <summary>
        /// Opens a given url, expecting a HTTP 500 error.
        /// </summary>
        /// <param name="url">The url</param>
        public void OpenURLExpecting500(string url) {
            // Attempt to open the url
            this._selenium.Open(url);

            // Verify the response
            if (this._f1Environment == F1Environments.STAGING || this._f1Environment == F1Environments.LV_PROD)
            {
                Assert.AreEqual("500 Error", this._selenium.GetText("//span[@class='float_left']"));
                this._selenium.VerifyTextPresent("Well, that wasn’t supposed to happen…");
            }
            else {
                //Assert.IsTrue(exceptionText.Contains("Response_Code = 500 Error_Message = Internal Server Error"));
                Assert.IsTrue(this._selenium.IsTextPresent("Server Error in '/' Application."));
                Assert.IsTrue(this._selenium.IsTextPresent("Object reference not set to an instance of an object."));
            }
        }

        /// <summary>
        /// Opens a given url, expecting a HTTP 500 error.
        /// </summary>
        /// <param name="url">The url</param>
        public void OpenURLExpecting500WebDriver(string url)
        {
            // Attempt to open the url
            this.OpenURLWebDriver(url);
            

            // Verify the response
            if (this._f1Environment == F1Environments.STAGING || this._f1Environment == F1Environments.LV_PROD || this._f1Environment == F1Environments.LV_UAT || this._f1Environment == F1Environments.LV_QA)
            {
                Assert.AreEqual("500 Error", this._driver.FindElementByXPath("//span[@class='float_left']").Text);
                this.VerifyTextPresentWebDriver("Well, that wasn’t supposed to happen…");
            }
            else
            {
                //Assert.IsTrue(exceptionText.Contains("Response_Code = 500 Error_Message = Internal Server Error"));
                Assert.IsTrue(this.IsTextPresentWebDriver("Server Error in '/' Application."));
                Assert.IsTrue(this.IsTextPresentWebDriver("Object reference not set to an instance of an object."));
            }
        }

        public void Popups_Confirmation(string yesNo) {
            
            try{
                this._selenium.WaitForPopUp("psuedoModal", "30000");
                this._selenium.SelectWindow("name=psuedoModal");

            }
            catch (Exception e)
            {
                this._selenium.WaitForPopUp("frmQuestionPopUp", "30000");
                this._selenium.SelectWindow("name=frmQuestionPopUp");

            }

            switch (yesNo) {
                case "Yes":
                    this._selenium.KeyPress("btnYes", "\\9");
                    this._selenium.KeyPress("btnYes", "\\13");
                    break;
                case "No":
                    this._selenium.KeyPress("btnNo", "\\9");
                    this._selenium.KeyPress("btnNo", "\\9");
                    this._selenium.KeyPress("btnNo", "\\13");
                    break;
                default:
                    throw new SeleniumException("Not a valid option!!");
            }
            //this._selenium.DeselectPopUp();
            this._selenium.SelectWindow("");

            for (int i = 0; i < 50; i++) {
                try {
                    this._selenium.SelectWindow("null");
                    this._selenium.Focus("//div[@id='nav']");

                    if (this._selenium.GetEval("this._selenium.browserbot.getCurrentWindow().document.activeElement.id.match('nav')") == "true") {
                        break;
                    }
                }
                catch (SeleniumException) {
                }
            }
        }

        public void Popups_ConfirmationWebDriver(string yesNo) {
            this._driver.SwitchTo().Window("psuedoModal");

            switch (yesNo) {
                case "Yes":
                    this._driver.FindElementById("btnYes").Click();
                    break;
                case "No":
                    this._driver.FindElementById("btnNo").Click();
                    break;
                default:
                    throw new WebDriverException("Not a valid option!!");
            }

            this._driver.SwitchTo().Window(this._driver.WindowHandles[0]);
        }

        /// <summary>
        /// Verifies the screen message errors displayed to the user.
        /// </summary>
        public void VerifyErrorMessages(IList<string> _errorText) {
            if (_errorText.Count > 0) {
                if (_errorText.Count == 1) {
                    this._selenium.VerifyTextPresent(TextConstants.ErrorHeadingSingular);
                }
                else {
                    this._selenium.VerifyTextPresent(TextConstants.ErrorHeadingPlural);
                }

                foreach (string error in _errorText) {
                    this._selenium.VerifyTextPresent(error);
                }

                // Verify the number of errors present
                Assert.AreEqual(_errorText.Count, this._selenium.GetXpathCount("//dl[@id='error_message']/dd"));
            }
        }

        /// <summary>
        /// Verifies the screen message errors displayed to the user.
        /// </summary>
        public void VerifyErrorMessagesWebDriver(IList<string> _errorText) {

            string errText = string.Empty;

            // Verify the number of errors present
            foreach (string eText in _errorText)
            {
                errText = string.Format(" {0} \n", eText);
            }

            Assert.AreEqual(_errorText.Count, this._driver.FindElementsByXPath("//dl[@id='error_message']/dd").Count, string.Format("Expecting following error(s): {0}\n", errText));

            if (_errorText.Count > 0) {
                if (_errorText.Count == 1) {
                    Assert.IsTrue(this._driver.FindElementByTagName("html").Text.Contains(TextConstants.ErrorHeadingSingular));
                }
                else {
                    Assert.IsTrue(this._driver.FindElementByTagName("html").Text.Contains(TextConstants.ErrorHeadingPlural));
                }

                foreach (string error in _errorText) {
                    Assert.IsTrue(this._driver.FindElementByTagName("html").Text.Contains(error));
                }

            }
        }


        /// <summary>
        /// Verifies the screen message errors displayed to the user.
        /// </summary>
        public void VerifyErrorMessagesInfellowshipWebDriver(IList<string> _errorText)
        {

            string errText = string.Empty;

            // Verify the number of errors present
            foreach (string eText in _errorText)
            {
                errText = string.Format(" {0} \n", eText);
            }

            Assert.AreEqual(_errorText.Count, this._driver.FindElementsByXPath("//div[@class='error_msgs_for']/ul/li").Count, string.Format("Expecting following error(s): {0}\n", _errorText));
            
            if (_errorText.Count > 0)
            {
                if (_errorText.Count == 1)
                {
                    Assert.IsTrue(this._driver.FindElementByXPath("//div[@class='error_msgs_for']/h2").Text.Contains(TextConstants.ErrorHeadingSingular));
                }
                else
                {
                    Assert.IsTrue(this._driver.FindElementByXPath("//div[@class='error_msgs_for']/h2").Text.Contains(TextConstants.ErrorHeadingPlural));
                }

                //_errorText_errorText.Reverse();
                foreach (string error in _errorText)
                {
                    //Assert.IsTrue(this._driver.FindElementByTagName("li").Text.Contains(error));
                    Assert.IsTrue(this._driver.FindElementByTagName("html").Text.Contains(error), string.Format("Expected Error: {0} ", error));
                }

            }
        }

        /// <summary>
        ///  Verifies that Text Is Present Web Driver
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void VerifyTextPresentWebDriver(string value)
        {
            //return this._driver.PageSource.Contains(value);
            //Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(value));
            Boolean foundText = false;

            if(this._driver.PageSource.Contains(value) || this._driver.FindElementByTagName("html").Text.Contains(value))
            {
                foundText = true;

            }

            Assert.IsTrue(foundText, string.Format("Text [{0}] was not found.", value));

        }

        /// <summary>
        ///  Verifies that Text Is NOT Present Web Driver
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void VerifyTextNotPresentWebDriver(string value)
        {
            //return this._driver.PageSource.Contains(value);
            Boolean foundText = false;

            if (this._driver.PageSource.Contains(value) || this._driver.FindElementByTagName("html").Text.Contains(value))
            {
                foundText = true;

            }
            
            Assert.IsFalse(foundText, string.Format("Text [{0}] was found.", value));

        }

        /// <summary>
        /// Focus on an element
        /// </summary>
        /// <param name="byElement"></param>
        public void FocusElementWebDriver(By byElement)
        {

            this._driver.FindElement(byElement).SendKeys(Keys.Space);
        }

        /// <summary>
        /// Checks To see if Element Pesent using WebDriver
        /// </summary>
        /// <param name="byElement">Element</param>
        /// <returns>True or False</returns>
        public Boolean IsElementPresentWebDriver(By byElement)
        {
            Boolean present = false;

            try
            {
                this._driver.FindElement(byElement);
                present = true;
            }
            catch (NoSuchElementException e)
            {
                log.Debug(e.Message);
                present = false;
            }

            return present;
        }

        /// <summary>
        /// Checks To see if Element Visible using WebDriver
        /// </summary>
        /// <param name="byElement">Element</param>
        /// <returns>True or False</returns>
        public Boolean IsElementVisibleWebDriver(By byElement)
        {
            Boolean present = false;

            try
            {
                present = this._driver.FindElement(byElement).Displayed;
            }
            catch (NoSuchElementException e)
            {
                log.Warn(e.Message);
                present = false;
            }

            TestLog.WriteLine("Element {0} is visible: {1}", byElement, present);

            return present;
        }

        /// <summary>
        /// Checks to see if Text is present in Present using WebDriver
        /// </summary>
        /// <param name="value">Text to locate</param>
        /// <returns>True or False</returns>
        public Boolean IsTextPresentWebDriver(string value)
        {
            return this._driver.PageSource.Contains(value);
        }

        /// <summary>
        /// Verifies if Element Pesent using WebDriver
        /// </summary>
        /// <param name="byElement">Element</param>
        /// <returns>True or False</returns>
        public void VerifyElementPresentWebDriver(By byElement)
        {
            Assert.IsTrue(this.IsElementPresentWebDriver(byElement), string.Format("Element {0} not present", byElement.ToString()));
        }

        /// <summary>
        /// Verifies if Element Not Pesent using WebDriver
        /// </summary>
        /// <param name="byElement">Element</param>
        /// <returns>True or False</returns>
        public void VerifyElementNotPresentWebDriver(By byElement)
        {
            Assert.IsFalse(this.IsElementPresentWebDriver(byElement), string.Format("Element {0} is present", byElement.ToString()));
        }

        /// <summary>
        /// Verifies Element Is Displayed
        /// </summary>
        /// <param name="byElement"></param>
        public void VerifyElementDisplayedWebDriver(By byElement)
        {

            Assert.IsTrue(this._driver.FindElement(byElement).Displayed, string.Format("Element {0} not displayed", byElement.ToString()));

        }

        /// <summary>
        /// Verifies Element Not Displayed
        /// </summary>
        /// <param name="byElement">By Element (i.e. By.LinkText("All"))</param>
        public void VerifyElementNotDisplayedWebDriver(By byElement)
        {

            Assert.IsFalse(this._driver.FindElement(byElement).Displayed, string.Format("Element {0} is displayed", byElement.ToString()));

        }

        /// <summary>
        /// Checks to see if Error Messages are found and then throws Error Messages Found
        /// </summary>
        public void CheckAndThrowErrorMessagesException()
        {

            string errorText = string.Empty;
        
            if (this.IsElementVisibleWebDriver(By.Id("error_message")))
            {
                int errorCount = this._driver.FindElementsByXPath("//*[@id='error_message']/dd").Count;
                TestLog.WriteLine("Error XPath count:" + errorCount);
                for (int e = 0; e < errorCount; e++)
                {
                    TestLog.WriteLine("Error Text: " + this._driver.FindElementByXPath(string.Format("//*[@id='error_message']/dd[{0}]", e + 1)).Text);
                    errorText += string.Format(" {0}", (this._driver.FindElementByXPath(string.Format("//*[@id='error_message']/dd[{0}]", e + 1)).Text));

                }

                throw new WebDriverException("Error Messages Found. " + errorText);
            }

        }


        #region Hacks

        public DateTime AddHourSeGrid(DateTime now, string hostname, double hourValue)
        {
            if (hostname == "segrid.active.tan")
            {
                TestLog.WriteLine("BEFORE: {0}", now.ToShortTimeString());
                now = now.Add(TimeSpan.FromHours(hourValue));
                TestLog.WriteLine("AFTER: {0}", now.ToShortTimeString());
            }

            return now;
        }

        #endregion Hacks

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


        public void TakeScreenShot()
        {

            try
            {
                // Convert Base64 String to byte[]
                log.Info("... taking screen shot");

                byte[] imageBytes = Convert.FromBase64String(this._selenium.CaptureEntirePageScreenshotToString(null));

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(imageBytes, 0, imageBytes.Length))
                {
                    // Convert byte[] to Image
                    ms.Write(imageBytes, 0, imageBytes.Length);
                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);

                    // Embed the image to the log
                    TestLog.EmbedImage(null, image);
                }
            }
            catch (Exception e)
            {
                log.Fatal("Tear Down Error", e);
                TestLog.WriteLine(e.Message);
            }

        }
        #endregion ScreenShot

        public List<string> Get_CC_Images_Attribute()
        {
            //Terrible hack until Scheduled Giving is updated to responsive
            TestLog.WriteLine("F1 Env: {0}", this._f1Environment);

            string environment = this._f1Environment.ToString();

            if (environment == "LV_QA")
            {
                if ((this._driver.Url == "https://dc.qa.infellowship.com/OnlineGiving/ScheduledGiving/Step3") || (this._driver.Url == "https://qaeunlx0c6.qa.infellowship.com/OnlineGiving/ScheduledGiving/Step3")
                    || (this._driver.Url.Contains("https://integration.qa.fellowshipone.com/integration/FormBuilder/")))
                {
                    List<string> ccImgOptionAttributes = new List<string>();
                    IList<IWebElement> ccImgOptions = this._driver.FindElementById("bank_card_images").FindElements(By.TagName("img"));
                    foreach (IWebElement ccImgOption in ccImgOptions)
                    {
                        string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/credit_cards/", "?" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        //string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/credit_cards/" }, StringSplitOptions.RemoveEmptyEntries)[1];

                        //string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/assets/images/credit_cards/" }, StringSplitOptions.RemoveEmptyEntries)[1];

                        log.Debug(imgTxt);
                        ccImgOptionAttributes.Add(imgTxt);
                    }

                    return ccImgOptionAttributes;
                }
                else
                {
                    List<string> ccImgOptionAttributes = new List<string>();
                    IList<IWebElement> ccImgOptions = this._driver.FindElementById("cc_icons").FindElements(By.TagName("img"));
                    foreach (IWebElement ccImgOption in ccImgOptions)
                    {
                        string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/credit_cards/", "?" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        //string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/credit_cards/" }, StringSplitOptions.RemoveEmptyEntries)[1];

                        //string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/assets/images/credit_cards/" }, StringSplitOptions.RemoveEmptyEntries)[1];

                        log.Debug(imgTxt);
                        ccImgOptionAttributes.Add(imgTxt);
                    }

                    return ccImgOptionAttributes;
                }
            }
            else
            {

                if ((this._driver.Url == string.Format("https://dc.{0}.infellowship.com/OnlineGiving/ScheduledGiving/Step3", environment.ToLower())) || (this._driver.Url == string.Format("https://qaeunlx0c6.{0}.infellowship.com/OnlineGiving/ScheduledGiving/Step3", environment.ToLower()))
                    || (this._driver.Url.Contains(string.Format("https://integration.{0}.fellowshipone.com/integration/FormBuilder/", environment.ToLower()))))

                {
                    List<string> ccImgOptionAttributes = new List<string>();
                    IList<IWebElement> ccImgOptions = this._driver.FindElementById("bank_card_images").FindElements(By.TagName("img"));
                    foreach (IWebElement ccImgOption in ccImgOptions)
                    {
                        string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/credit_cards/", "?" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        //string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/credit_cards/" }, StringSplitOptions.RemoveEmptyEntries)[1];

                        //string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/assets/images/credit_cards/" }, StringSplitOptions.RemoveEmptyEntries)[1];

                        log.Debug(imgTxt);
                        ccImgOptionAttributes.Add(imgTxt);
                    }

                    return ccImgOptionAttributes;
                }
                else
                {
                    List<string> ccImgOptionAttributes = new List<string>();
                    IList<IWebElement> ccImgOptions = this._driver.FindElementById("cc_icons").FindElements(By.TagName("img"));
                    foreach (IWebElement ccImgOption in ccImgOptions)
                    {
                        string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/credit_cards/", "?" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        //string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/credit_cards/" }, StringSplitOptions.RemoveEmptyEntries)[1];

                        //string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/assets/images/credit_cards/" }, StringSplitOptions.RemoveEmptyEntries)[1];

                        log.Debug(imgTxt);
                        ccImgOptionAttributes.Add(imgTxt);
                    }

                    return ccImgOptionAttributes;
                }
            }
        }

        public void Verify_CC_Images_Display()
        {
            //Terrible hack until Scheduled Giving is updated to responsive
            TestLog.WriteLine("F1 Env: {0}", this._f1Environment);

            string environment = this._f1Environment.ToString();

            if (environment == "LV_QA")
            {
                if ((this._driver.Url == "https://dc.qa.infellowship.com/OnlineGiving/ScheduledGiving/Step3") || (this._driver.Url == "https://qaeunlx0c6.qa.infellowship.com/OnlineGiving/ScheduledGiving/Step3")
                    || (this._driver.Url.Contains("https://integration.qa.fellowshipone.com/integration/FormBuilder/")))
                {
                    IList<IWebElement> ccImgOptions = this._driver.FindElementById("bank_card_images").FindElements(By.TagName("img"));

                    foreach (IWebElement ccImgOption in ccImgOptions)
                    {
                        Object result = ((IJavaScriptExecutor)this._driver).ExecuteScript(
                                        "return arguments[0].complete && " +
                                        "typeof arguments[0].naturalWidth != \"undefined\" && " +
                                        "arguments[0].naturalWidth > 0", ccImgOption);

                        Boolean loaded = false;
                        if (result.GetType().IsAssignableFrom(loaded.GetType()))
                        {
                            loaded = (Boolean)result;
                        }
                        string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/credit_cards/", "?", ".png" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        Assert.IsTrue(loaded, string.Format("Credit Card image for [{0}] is not visible", imgTxt));
                    }
                }
                else
                {
                    IList<IWebElement> ccImgOptions = this._driver.FindElementById("cc_icons").FindElements(By.TagName("img"));

                    foreach (IWebElement ccImgOption in ccImgOptions)
                    {
                        Object result = ((IJavaScriptExecutor)this._driver).ExecuteScript(
                                        "return arguments[0].complete && " +
                                        "typeof arguments[0].naturalWidth != \"undefined\" && " +
                                        "arguments[0].naturalWidth > 0", ccImgOption);

                        Boolean loaded = false;
                        if (result.GetType().IsAssignableFrom(loaded.GetType()))
                        {
                            loaded = (Boolean)result;
                        }
                        string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/credit_cards/", "?", ".png" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        Assert.IsTrue(loaded, string.Format("Credit Card image for [{0}] is not visible", imgTxt));
                    }
                }
            }
            else
            {

                if ((this._driver.Url == string.Format("https://dc.{0}.infellowship.com/OnlineGiving/ScheduledGiving/Step3", environment.ToLower())) || (this._driver.Url == string.Format("https://qaeunlx0c6.{0}.infellowship.com/OnlineGiving/ScheduledGiving/Step3", environment.ToLower())) 
                    || (this._driver.Url.Contains(string.Format("https://integration.{0}.fellowshipone.com/integration/FormBuilder/", environment.ToLower()))))
                {
                    IList<IWebElement> ccImgOptions = this._driver.FindElementById("bank_card_images").FindElements(By.TagName("img"));

                    foreach (IWebElement ccImgOption in ccImgOptions)
                    {
                        Object result = ((IJavaScriptExecutor)this._driver).ExecuteScript(
                                        "return arguments[0].complete && " +
                                        "typeof arguments[0].naturalWidth != \"undefined\" && " +
                                        "arguments[0].naturalWidth > 0", ccImgOption);

                        Boolean loaded = false;
                        if (result.GetType().IsAssignableFrom(loaded.GetType()))
                        {
                            loaded = (Boolean)result;
                        }
                        string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/credit_cards/", "?", ".png" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        Assert.IsTrue(loaded, string.Format("Credit Card image for [{0}] is not visible", imgTxt));
                    }
                }
                else
                {
                    IList<IWebElement> ccImgOptions = this._driver.FindElementById("cc_icons").FindElements(By.TagName("img"));

                    foreach (IWebElement ccImgOption in ccImgOptions)
                    {
                        Object result = ((IJavaScriptExecutor)this._driver).ExecuteScript(
                                        "return arguments[0].complete && " +
                                        "typeof arguments[0].naturalWidth != \"undefined\" && " +
                                        "arguments[0].naturalWidth > 0", ccImgOption);

                        Boolean loaded = false;
                        if (result.GetType().IsAssignableFrom(loaded.GetType()))
                        {
                            loaded = (Boolean)result;
                        }
                        string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/credit_cards/", "?", ".png" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        Assert.IsTrue(loaded, string.Format("Credit Card image for [{0}] is not visible", imgTxt));
                    }
                }
            }
                //IList<IWebElement> ccImgOptions = this._driver.FindElementById("bank_card_images").FindElements(By.TagName("img"));

                //Verify Images via an API
                //APIBase api = new APIBase();
                /*foreach (IWebElement ccImgOption in ccImgOptions)
                {
                    //Passing image Url and validatating the httlp response
                    string imageUrl = ccImgOption.GetAttribute("src");
                    TestLog.WriteLine("image URL {0}", imageUrl);
                    WebResponse response = api.MakeWebRequest(imageUrl);
                    TestLog.WriteLine("Reponse : {0}", ((HttpWebResponse)response).StatusCode);
                    string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/credit_cards/", "?" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    Assert.AreEqual(HttpStatusCode.OK, ((HttpWebResponse)response).StatusCode, string.Format("Credit card image [{0}] not loaded correctly", imgTxt));
                    //response.Close();
                }
                */

                //Verify Images via java script instead of API calls
            //    foreach (IWebElement ccImgOption in ccImgOptions)
            //    {
            //        Object result = ((IJavaScriptExecutor)this._driver).ExecuteScript(
            //                        "return arguments[0].complete && " +
            //                        "typeof arguments[0].naturalWidth != \"undefined\" && " +
            //                        "arguments[0].naturalWidth > 0", ccImgOption);

            //        Boolean loaded = false;
            //        if (result.GetType().IsAssignableFrom(loaded.GetType()))
            //        {
            //            loaded = (Boolean)result;
            //        }
            //        string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/credit_cards/", "?", ".png" }, StringSplitOptions.RemoveEmptyEntries)[1];
            //        Assert.IsTrue(loaded, string.Format("Credit Card image for [{0}] is not visible", imgTxt));
            //    }
            //}
            //else
            //{
            //    IList<IWebElement> ccImgOptions = this._driver.FindElementById("cc_icons").FindElements(By.TagName("img"));

                //Verify Images via an API
                //APIBase api = new APIBase();
                /*foreach (IWebElement ccImgOption in ccImgOptions)
                {
                    //Passing image Url and validatating the httlp response
                    string imageUrl = ccImgOption.GetAttribute("src");
                    TestLog.WriteLine("image URL {0}", imageUrl);
                    WebResponse response = api.MakeWebRequest(imageUrl);
                    TestLog.WriteLine("Reponse : {0}", ((HttpWebResponse)response).StatusCode);
                    string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/credit_cards/", "?" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    Assert.AreEqual(HttpStatusCode.OK, ((HttpWebResponse)response).StatusCode, string.Format("Credit card image [{0}] not loaded correctly", imgTxt));
                    //response.Close();
                }
                */

                //Verify Images via java script instead of API calls
            //    foreach (IWebElement ccImgOption in ccImgOptions)
            //    {
            //        Object result = ((IJavaScriptExecutor)this._driver).ExecuteScript(
            //                        "return arguments[0].complete && " +
            //                        "typeof arguments[0].naturalWidth != \"undefined\" && " +
            //                        "arguments[0].naturalWidth > 0", ccImgOption);

            //        Boolean loaded = false;
            //        if (result.GetType().IsAssignableFrom(loaded.GetType()))
            //        {
            //            loaded = (Boolean)result;
            //        }
            //        string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/credit_cards/", "?", ".png" }, StringSplitOptions.RemoveEmptyEntries)[1];
            //        Assert.IsTrue(loaded, string.Format("Credit Card image for [{0}] is not visible", imgTxt));
            //    }
            //}
        }

        public void Verify_CC_Images(string elementId = "payment_type_id")
        {
            //Verifying Credit Card image display
            this.Verify_CC_Images_Display();

            //Get the CC images
            List<string> ccImgOptions = this.Get_CC_Images_Attribute();

            //Get CC Options            
            IList<IWebElement> ccOptions = new SelectElement(this._driver.FindElementById(elementId)).Options;

            //Verify that we contain the images that are in the list
            //NOTE: Not all options have images. Just checking on what we have
            foreach (IWebElement ccOption in ccOptions)
            {
                

                //Assert.Contains(ccNames, ccOption.Text, "Credit Card Option mistmatch");
                this.Verify_CC_Images_And_Options(ccOption.Text, ccImgOptions);

                //HttpRequest image to get response of 200
                //TODO
                
            }

        }

        public void Verify_CC_Images_And_Options(string ccName, List<string> ccImages)
        {
            log.DebugFormat("CC Name [{0}]", ccName);

            switch (ccName)
            {

                case CreditCard.Names.AMEX:
                    Assert.Contains(ccImages, CreditCard.Images.AMEX, string.Format("{0} image was not displayed", CreditCard.Names.AMEX));                    
                    break;
                case CreditCard.Names.Carte_Blanche:
                    Assert.Contains(ccImages, CreditCard.Images.Carte_Blanche, string.Format("{0} image was not displayed", CreditCard.Names.Carte_Blanche));
                    break;
                case CreditCard.Names.Carte_Bleue:
                    Assert.Contains(ccImages, CreditCard.Images.Carte_Bleue, string.Format("{0} image was not displayed", CreditCard.Names.Carte_Bleue));
                    break;
                case CreditCard.Names.Carta_Si:
                    Assert.Contains(ccImages, CreditCard.Images.Carta_Si, string.Format("{0} image was not displayed", CreditCard.Names.Carta_Si));
                    break;
                case CreditCard.Names.Dankort:
                    Assert.Contains(ccImages, CreditCard.Images.Dankort, string.Format("{0} image was not displayed", CreditCard.Names.Dankort));
                    break;
                case CreditCard.Names.Diners_Club:
                    Assert.Contains(ccImages, CreditCard.Images.Diners_Club, string.Format("{0} image was not displayed", CreditCard.Names.Diners_Club));
                    break;
                case CreditCard.Names.Discover:
                    Assert.Contains(ccImages, CreditCard.Images.Discover, string.Format("{0} image was not displayed", CreditCard.Names.Discover));
                    break;
                case CreditCard.Names.JCB:
                    Assert.Contains(ccImages, CreditCard.Images.JCB, string.Format("{0} image was not displayed", CreditCard.Names.JCB));
                    break;
                case CreditCard.Names.Laser:
                    Assert.Contains(ccImages, CreditCard.Images.Laser, string.Format("{0} image was not displayed", CreditCard.Names.Laser));
                    break;
                case CreditCard.Names.Maestro:
                    Assert.Contains(ccImages, CreditCard.Images.Maestro, string.Format("{0} image was not displayed", CreditCard.Names.Maestro));
                    break;
                case CreditCard.Names.Master_Card:
                case CreditCard.Names.MasterCard:
                    Assert.Contains(ccImages, CreditCard.Images.Master_Card, string.Format("{0} image was not displayed", CreditCard.Names.Master_Card));
                    break;
                case CreditCard.Names.Solo:
                    Assert.Contains(ccImages, CreditCard.Images.Solo, string.Format("{0} image was not displayed", CreditCard.Names.Solo));
                    break;
                case CreditCard.Names.Switch:
                    Assert.Contains(ccImages, CreditCard.Images.Switch, string.Format("{0} image was not displayed", CreditCard.Names.Switch));
                    break;
                case CreditCard.Names.Switch_Maestro:
                    Assert.Contains(ccImages, CreditCard.Images.Switch_Maestro, string.Format("{0} image was not displayed", CreditCard.Names.Switch_Maestro)); ;
                    break;
                case CreditCard.Names.Visa:
                     Assert.Contains(ccImages, CreditCard.Images.Visa, string.Format("{0} image was not displayed", CreditCard.Names.Visa));
                    break;
                case CreditCard.Names.Visa_Electron:
                    Assert.Contains(ccImages, CreditCard.Images.Visa_Electron, string.Format("{0} image was not displayed", CreditCard.Names.Visa_Electron));
                    break;
                case CreditCard.Names.EnRoute:
                case CreditCard.Names.JAL:
                case CreditCard.Names.Bill_Me_Later:
                case CreditCard.Names.Delta:
                case CreditCard.Names.None:
                case CreditCard.Names.Optima:
                case CreditCard.Names.Cash:
                case CreditCard.Names.Empty:
                case CreditCard.Names.eCheck:
                case CreditCard.Names.Check:
                    log.DebugFormat("No image for {0}", ccName);
                    //if(ccImages.Contains
                    break;

                default:
                    throw new WebDriverException(string.Format("Invalid CC: {0}", ccName));
            }

        }

        public void Verify_SchedGiving_SecurityCode_Tooltip()
        {
            Actions builder = new Actions(this._driver);
            IWebElement toolTipElement = this._driver.FindElementByXPath("//span[@id='cc_tooltip']");            
            builder.MoveToElement(toolTipElement).ClickAndHold().Build().Perform();

            //Verify images in tool tip
            IList<IWebElement> ccImgOptions = this._driver.FindElementById("security-code-help").FindElements(By.TagName("img"));

            foreach (IWebElement ccImgOption in ccImgOptions)
            {
                Object result = ((IJavaScriptExecutor)this._driver).ExecuteScript(
                                "return arguments[0].complete && " +
                                "typeof arguments[0].naturalWidth != \"undefined\" && " +
                                "arguments[0].naturalWidth > 0", ccImgOption);

                Boolean loaded = false;
                if (result.GetType().IsAssignableFrom(loaded.GetType()))
                {
                    loaded = (Boolean)result;
                }
                string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/credit_cards/", "?", ".png" }, StringSplitOptions.RemoveEmptyEntries)[1];
                Assert.IsTrue(loaded, string.Format("Credit Card image for [{0}] is not visible", imgTxt));
            }

            // commneted these lines since Text is not recognizing from tool tip window.
            //Verify text in tool tip
            //*[@id="security-code-help"]/div[1]
            //*[@id="security-code-help"]/div[1]/text()
            //log.DebugFormat("First DIV {0}", this._driver.FindElementByXPath("//div[@id='security-code-help']/div[1]").FindElement(By.TagName("br")).Text.Trim());
            //log.DebugFormat("First DIV {0}", this._driver.FindElementByXPath("//div[@id='security-code-help']/div[1]").ToString().Trim());
            //Assert.AreEqual("On American Express and Diners Club cards, the code is a 4-digit number just above and to the right of the card number", this._driver.FindElementByXPath("//div[@id='security-code-help']/div[1]").GetAttribute("innerHtml").Trim());
            //Assert.AreEqual("On American Express and Diners Club cards, the code is a 4-digit number just above and to the right of the card number", (this._driver.FindElementByXPath("//*[@id='security-code-help']/p[1]").GetAttribute("innerHtml").Trim()));
            //Assert.AreEqual("On Visa, Mastercard, and other credit cards, the code is a 3-digit number located on the back of the card, in the signature block.", this._driver.FindElementByXPath("//*[@id='security-code-help']/p[2]").GetAttribute("innerHtml").Trim());

            //Assert.AreEqual("On American Express and Diners Club cards, the code is a 4-digit number just above and to the right of the card number", (this._driver.FindElementById("security-code-help/p[1]").GetAttribute("innerHtml").Trim()));
            //Assert.AreEqual("On Visa, Mastercard, and other credit cards, the code is a 3-digit number located on the back of the card, in the signature block.", this._driver.FindElementById("security-code-help']/p[2]").GetAttribute("innerHtml").Trim());

            //Release ClickAndHold for next iteration
            builder.MoveToElement(toolTipElement).Release().Perform();

        }

        public void Verify_GivingNow_SecurityCode_Tooltip()
        {
            Actions builder = new Actions(this._driver);
            IWebElement toolTipElement = this._driver.FindElementByXPath("//div[@class='form-group cc_security_code']/div[2]/a");
            builder.MoveToElement(toolTipElement).ClickAndHold().Build().Perform();

            Assert.IsTrue("What is a security code?".Equals(toolTipElement.GetAttribute("data-title")), "The tooltip tile is wrong");

            //Verify images in tool tip
            IWebElement ccImgOption = this._driver.FindElementById("sample-card");


            Object result = ((IJavaScriptExecutor)this._driver).ExecuteScript(
                                "return arguments[0].complete && " +
                                "typeof arguments[0].naturalWidth != \"undefined\" && " +
                                "arguments[0].naturalWidth > 0", ccImgOption);

            Boolean loaded = false;
            if (result.GetType().IsAssignableFrom(loaded.GetType()))
            {
                    loaded = (Boolean)result;
            }
            string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/images/", "?", ".png" }, StringSplitOptions.RemoveEmptyEntries)[1];
            Assert.IsTrue(loaded, string.Format("Security code  image for [{0}] is not visible", imgTxt));

            //Release ClickAndHold for next iteration
            builder.MoveToElement(toolTipElement).Release().Perform();

        }

        public void Verify_Reg2_SecurityCode_Tooltip()
        {
            Actions builder = new Actions(this._driver);
            IWebElement toolTipElement = this._driver.FindElementByXPath("//div[@id='cc_details']/fieldset/div[5]/div[2]/a");
            builder.MoveToElement(toolTipElement).ClickAndHold().Build().Perform();

            Assert.IsTrue("What is a security code?".Equals(toolTipElement.GetAttribute("data-title")), "The tooltip tile is wrong");

            //Verify images in tool tip
            //IWebElement ccImgOption = toolTipElement.FindElement(By.TagName("img"));
            IWebElement ccImgOption = this._driver.FindElement(By.XPath("//img[@class='float_right']"));

            Object result = ((IJavaScriptExecutor)this._driver).ExecuteScript(
                                "return arguments[0].complete && " +
                                "typeof arguments[0].naturalWidth != \"undefined\" && " +
                                "arguments[0].naturalWidth > 0", ccImgOption);

            Boolean loaded = false;
            if (result.GetType().IsAssignableFrom(loaded.GetType()))
            {
                loaded = (Boolean)result;
            }
            string imgTxt = ccImgOption.GetAttribute("src").Split(new string[] { "/images/", "?", ".png" }, StringSplitOptions.RemoveEmptyEntries)[1];
            Assert.IsTrue(loaded, string.Format("Security code  image for [{0}] is not visible", imgTxt));

            //Release ClickAndHold for next iteration
            builder.MoveToElement(toolTipElement).Release().Perform();

        }

        public void Click_Button(By byElement)
        {
            Actions builder = new Actions(this._driver);
            IWebElement btnElement = this._driver.FindElement(byElement);
            builder.MoveToElement(btnElement).Perform();
            builder.ContextClick(btnElement).Perform();

        }


        /// <summary>
        /// Wait For Page is loaded
        /// </summary>
        /// <param name="timeOut">Timeout Wait</param>
        public void WaitForPageIsLoaded(double timeOut = 30)
        {
            IWait<IWebDriver> wait = new WebDriverWait(this._driver, TimeSpan.FromSeconds(timeOut));
            wait.Until(js_driver => ((IJavaScriptExecutor)js_driver).ExecuteScript("return document.readyState").Equals("complete"));
        }


        /// <summary>
        /// Wait for element is enabled
        /// </summary>
        /// <param name ="js_locator">for example, document.getElementById('student_id')</param>
        /// <param name="timeOut">Timeout Wait</param>
        public void waitForElementIsEnabled(string js_locator, double timeOut = 120)
        {
            string script = "return " + js_locator + ".disabled;";
            IWait<IWebDriver> wait = new WebDriverWait(this._driver, TimeSpan.FromSeconds(timeOut));
            wait.Until(js_driver => ((IJavaScriptExecutor)js_driver).ExecuteScript(script).Equals(false));
        }
 

        /// <summary>
        /// Wait for element appear
        /// </summary>
        /// <param name ="jquery_locator">for example, $("div#id")</param>
        /// <param name="timeOut">Timeout Wait</param>
        public void waitForElementAppear(string jquery_locator, double timeOut = 30)
        {
            string script = "return " + jquery_locator + ".length > 0;";
            IWait<IWebDriver> wait = new WebDriverWait(this._driver, TimeSpan.FromSeconds(timeOut));
            wait.Until(js_driver => ((IJavaScriptExecutor)js_driver).ExecuteScript(script));
        }

        /// <summary>
        /// Wait for element appear
        /// </summary>
        /// <param name ="jquery_locator">for example, $("div#id")</param>
        /// <param name="timeOut">Timeout Wait</param>
        public bool isElementAppear(string jquery_locator, double timeOut = 30)
        {
            string script = "return " + jquery_locator + ".length > 0;";

            IWait<IWebDriver> wait = new WebDriverWait(this._driver, TimeSpan.FromSeconds(timeOut));
            wait.Until(js_driver => ((IJavaScriptExecutor)js_driver).ExecuteScript(script));

            return bool.Parse(((IJavaScriptExecutor)this._driver).ExecuteScript(script).ToString());
        }

        /// <summary>
        /// Get page load time
        /// </summary>
        /// <param></param>
        public int getPageLoadTime()
        {
            this.WaitForPageIsLoaded(100);

            IJavaScriptExecutor js_driver = (IJavaScriptExecutor)this._driver;
            string plt = js_driver.ExecuteScript("var t = performance.timing; return (t.loadEventEnd - t.navigationStart);").ToString();

            return int.Parse(plt);
        }

        /// <summary>
        /// Wait For Element in certain amount of time
        /// </summary>
        /// <param name="byElement">Element to locate (i.e. By.XPath("//div[@id='modal_select_individual']") )</param>
        /// <param name="timeOut">Timeout Wait</param>
        /// <param name="errorMessage">Error message to be displayed</param>
        public void WaitForElement(By byElement, double timeOut = 30, string errorMessage = "")
        {
            this.WaitForElement(this._driver, byElement, timeOut, errorMessage);
        }

        /// <summary>
        /// Wait For Element in certain amount of time
        /// </summary>
        /// <param name="driver">WebDriver instantiated</param>
        /// <param name="byElement">Element to locate (i.e. By.XPath("//div[@id='modal_select_individual']") )</param>
        /// <param name="timeOut">Timeout</param>
        public void WaitForElement(IWebDriver driver, By byElement, double timeOut = 30, string errorMessage = "")
        {

            IWebElement myDynamicElement = null;
            WebDriverWait wait = null;

            //Thread.Sleep(TimeSpan.FromSeconds(timeOut / 2));
            //wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOut));

            try
            {
                log.Debug("Wait for " + byElement.ToString() + " in " + timeOut + " seconds");
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOut));
                myDynamicElement = wait.Until<IWebElement>((d) =>
                {
                    return d.FindElement(byElement);
                });
            }
            catch (Exception e)
            {
                throw new WebDriverException(string.Format("Wait for {0} Error. {1} {2}", byElement, errorMessage, e.Message));
            }

            //Assert.IsTrue(myDynamicElement.Displayed, byElement.ToString() + " not displayed");

        }

        /// <summary>
        /// Wait For Element in certain amount of time
        /// </summary>
        /// <param name="driver">WebDriver instantiated</param>
        /// <param name="byElement">Element to locate (i.e. By.XPath("//div[@id='modal_select_individual']") )</param>
        /// <param name="timeOut">Timeout</param>
        public void WaitForElementInexistent(IWebDriver driver, By byElement, double timeOut = 30, string errorMessage = "")
        {

            bool myDynamicElement = false;
            WebDriverWait wait = null;

            try
            {
                log.Debug("Wait for " + byElement.ToString() + " inexistent in " + timeOut + " seconds");
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOut));
                myDynamicElement = wait.Until<bool>((d) =>
                {
                    return !this.IsElementPresentWebDriver(byElement);
                });
            }
            catch (Exception e)
            {
                throw new WebDriverException(string.Format("Wait for {0} inexistent, Error. {1} {2}", byElement, errorMessage, e.Message));
            }

        }

        /// <summary>
        /// Wait For Element in certain amount of time to be enabled
        /// </summary>
        /// <param name="driver">WebDriver instantiated</param>
        /// <param name="byElement">Element to locate (i.e. By.XPath("//div[@id='modal_select_individual']") )</param>
        /// <param name="timeOut">Timeout</param>
        public void WaitForElementEnabled(By byElement, double timeOut = 30, string errorMessage = "")
        {

            bool myDynamicElement = false;
            WebDriverWait wait = null;
            IWebDriver driver = this._driver;

            try
            {
                log.Debug("Wait for Enabled" + byElement.ToString() + " in " + timeOut + " seconds");
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOut));
                myDynamicElement = wait.Until<bool>((d) =>
                {
                    return d.FindElement(byElement).Enabled;
                });
            }
            catch (Exception e)
            {
                throw new WebDriverException(string.Format("Wait for {0} Enabled Error. {1} {2}", byElement, errorMessage, e.Message));
            }

            Assert.IsTrue(myDynamicElement, "{0} not enabled in {1} seconds.", byElement.ToString(), timeOut);

        }

        /// <summary>
        /// Wait For Element in certain amount of time to be displayed
        /// </summary>
        /// <param name="driver">WebDriver instantiated</param>
        /// <param name="byElement">Element to locate (i.e. By.XPath("//div[@id='modal_select_individual']") )</param>
        /// <param name="timeOut">Timeout</param>
        public void WaitForElementDisplayed(By byElement, double timeOut = 30, string errorMessage = "")
        {

            bool myDynamicElement = false;
            WebDriverWait wait = null;
            IWebDriver driver = this._driver;

            try
            {
                log.Debug("Wait for Displayed" + byElement.ToString() + " in " + timeOut + " seconds");
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOut));
                myDynamicElement = wait.Until<bool>((d) =>
                {
                    return d.FindElement(byElement).Displayed;
                });
            }
            catch (Exception e)
            {
                throw new WebDriverException(string.Format("Wait for {0} Displayed Error. {1} {2}", byElement, errorMessage, e.Message));
            }

            Assert.IsTrue(myDynamicElement, "{0} not displayed in {1} seconds.", byElement.ToString(), timeOut);

        }

        /// <summary>
        /// Wait For Element in certain amount of time to be visible
        /// </summary>
        /// <param name="driver">WebDriver instantiated</param>
        /// <param name="byElement">Element to locate (i.e. By.XPath("//div[@id='modal_select_individual']") )</param>
        /// <param name="timeOut">Timeout</param>
        public void WaitForElementVisible(By byElement, double timeOut = 30, string errorMessage = "")
        {
            IWebDriver driver = this._driver;

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOut));
            wait.Until(ExpectedConditions.ElementExists(byElement));
            wait.Until(ExpectedConditions.ElementIsVisible(byElement));

        }

        /// <summary>
        /// Wait For Element in certain amount of time to not be displayed
        /// </summary>
        /// <param name="driver">WebDriver instantiated</param>
        /// <param name="byElement">Element to locate (i.e. By.XPath("//div[@id='modal_select_individual']") )</param>
        /// <param name="timeOut">Timeout</param>
        public void WaitForElementNotDisplayed(By byElement, double timeOut = 30, string errorMessage = "")
        {

            bool myDynamicElement = false;
            WebDriverWait wait = null;
            IWebDriver driver = this._driver;

            try
            {
                log.Debug("Wait for Not Displayed" + byElement.ToString() + " in " + timeOut + " seconds");
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOut));
                myDynamicElement = wait.Until<bool>((d) =>
                {
                    return !d.FindElement(byElement).Displayed;
                });
            }
            catch (Exception e)
            {
                throw new WebDriverException(string.Format("Wait for {0} Not Displayed Error. {1} {2}", byElement, errorMessage, e.Message));
            }

            

        }

        /// <summary>
        /// Wait and get a web Element
        /// </summary>
        /// <param name="driver">WebDriver instantiated</param>
        /// <param name="byElement">Element to locate (i.e. By.XPath("//div[@id='modal_select_individual']") )</param>
        /// <param name="timeOut">Timeout</param>
        public IWebElement WaitAndGetElement(By byElement, double timeOut = 30)
        {
            IWebDriver driver = this._driver;

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOut));
            wait.Until(ExpectedConditions.ElementIsVisible(byElement));

            try
            {
                return this._driver.FindElement(byElement);
            }
            catch (NoSuchElementException e)
            {
                log.Debug(e.Message);
                return null;
            }

        }

        /// <summary>
        /// Get Randon Amount Value (i.e. 6.99) used for Giving or Contribution or Event Registration
        /// </summary>
        /// <returns></returns>
        public string GetRandAmount()
        {

            Random rnd = new Random();
            string dollar = Convert.ToString(rnd.Next(1, 1000));
            string cents = Convert.ToString(rnd.Next(10, 99));
            string amount = string.Format("{0}.{1}", dollar, cents);

            return amount;
        }

        public string GetRandPhoneNumber()
        {
            Random rnd = new Random();
            string areaCode = Convert.ToString(rnd.Next(100, 999));
            string prefix = Convert.ToString(rnd.Next(100, 999));
            string number = Convert.ToString(rnd.Next(1000, 9999));
            string phoneNumber = string.Format("{0}-{1}-{2}", areaCode, prefix, number);

            return phoneNumber;

        }

        /// <summary>
        /// Generate an unique name
        /// </summary>
        /// <param name="prefix">The prefix of this unique name</param>
        /// <returns>A unique string with given prefix (e.g. Test20150607140454138)</returns>
        public string GetUniqueName(string prefix)
        {
            DateTime now = DateTime.Now;
            return string.Format("{0}{1}{2}", prefix, now.ToString("yyyyMMddHHmmss"), now.Millisecond.ToString());
        }

        /// <summary>
        /// Converts a given date to the format used in Fellowship One.
        /// </summary>
        /// <param name="date">The date time.</param>
        /// <returns>A string in the format of dd MMM yyyy (e.g. 31 Oct 2001)</returns>
        public string ConvertDateToNeutralFormat(DateTime date) {
            return date.ToString("dd MMM yyyy");
        }


        /// <summary>
        /// Converts a given date to the format used in Fellowship One.
        /// </summary>
        /// <param name="date">The date time.</param>
        /// <param name="format">A formatting provider.</param>
        /// <returns>A string in the format of dd MMM yyyy (e.g. 31 Oct 2001)</returns>
        public string ConvertDateToNeutralFormat(DateTime date, IFormatProvider format) {
            return date.ToString("dd MMM yyyy", format);
        }

        /// <summary>
        /// Returns the suffex for the day of a DateTime object.
        /// </summary>
        /// <param name="currentDateTime">A date time object.</param>
        /// <returns></returns>
        public string GetSuffexForDay(DateTime currentDateTime) {
            return (currentDateTime.Day == 1 || currentDateTime.Day == 21 || currentDateTime.Day == 31) ? "st" :
                (currentDateTime.Day == 2 || currentDateTime.Day == 22) ? "nd" :
                (currentDateTime.Day == 3 || currentDateTime.Day == 23) ? "rd" : "th";
        }

        public void SelectPersonWebDriver_Search(string name, bool organization = false)
        {
            this._driver.FindElementById("searchFor").SendKeys(name);

            if (organization)
            {
                this._driver.FindElementByXPath("//input[@id='rdoSearchType' and @value='organization']").Click();
            }

            this._driver.FindElementById("search_submit_button").Click();                       
        }

        public void SelectPersonWebDriver(string name, bool organization = false) {
            this._driver.FindElementById("searchFor").SendKeys(name);

            if (organization) {
                this._driver.FindElementByXPath("//input[@id='rdoSearchType' and @value='organization']").Click();
            }

            this._driver.FindElementById("search_submit_button").Click();

            this._driver.FindElementByXPath(string.Format("//div[@id='search_results_inner']/table/tbody/tr[*]/td[position()=2 and contains(normalize-space(string(.)), '{0}')]/ancestor::tr/td[1]/button", name)).Click();
        }

        /// <summary>
        /// Searches and selects a person from the modal people search window.  This modal only exists on MVC 3 pages.
        /// </summary>
        /// <param name="individualOrOrganizationName">The individual name to search for.</param>
        /// <param name="organizationName">The organization name to search for.</param>
        /// <param name="address">The address to search for.  This is an optional search value and will not be used in the search by default.</param>
        /// <param name="communicationValue">The communication value to search for.  This is an optional search value and will not be used in the search by default.</param>
        public void SelectPersonFromModal(string individualOrOrganizationName, bool isOrganization, [Optional, DefaultParameterValue(null)] string address, [Optional, DefaultParameterValue(null)] string communicationValue) {
            // Search for organization
            if (isOrganization) {
                this._selenium.Click("//input[@id='rdoSearchType' and @value='organization']");
                this._selenium.Type("searchFor", individualOrOrganizationName);
            }
            // Search for individual
            else {
                this._selenium.Type("searchFor", individualOrOrganizationName);
            }

            if (!string.IsNullOrEmpty(address)) {
                this._selenium.Type("txtAddress", address);
            }

            if (!string.IsNullOrEmpty(address)) {
                this._selenium.Type("txtCommunication", communicationValue);
            }

            // Submit
            this._selenium.Click("search_submit_button");

            // Wait for the results
            Retry.WithPolling(500).WithTimeout(30000).WithFailureMessage("Search results were not returned in alotted time.")
                .Until(() => this._selenium.IsElementPresent("//div[@id='search_results_inner']"));

            // Click select
            this._selenium.ClickAndWaitForPageToLoad("//div[@id='search_results_inner']/table/tbody/tr[2]/td[1]/button");
        }

        public void SelectPersonFromModalWebDriver(string individualOrOrganizationName, bool isOrganization, [Optional, DefaultParameterValue(null)] string address, [Optional, DefaultParameterValue(null)] string communicationValue)
        {
            // Search for organization
            if (isOrganization)
            {
                //this._driver.FindElementByCssSelector("//input[@id='rdoSearchType' and @value='organization']").Click();
                this._driver.FindElementByXPath("//input[@id='rdoSearchType' and @value='organization']").Click();
                this._driver.FindElementById("searchFor").SendKeys(individualOrOrganizationName);
                this._driver.FindElementById("chkIncludeInactive").Click();
            }
            // Search for individual
            else
            {
                this._driver.FindElementById("searchFor").SendKeys(individualOrOrganizationName);
            }

            if (!string.IsNullOrEmpty(address))
            {
                this._driver.FindElementById("txtAddress").SendKeys(address);
            }

            if (!string.IsNullOrEmpty(address))
            {
                this._driver.FindElementById("txtCommunication").SendKeys(communicationValue);
            }

            // Submit
            this._driver.FindElementById("search_submit_button").Click();

            // Wait for the results
            //Retry.WithPolling(500).WithTimeout(30000).WithFailureMessage("Search results were not returned in alotted time.")
            //    .Until(() => this._driver.FindElementByCssSelector("//div[@id='search_results_inner']").Displayed);           
            //Retry.WithPolling(500).WithTimeout(30000).Until(() => this._driver.FindElementByCssSelector("//div[@id='search_results_inner']").Displayed);

            //this.WaitForElement(this._driver, By.XPath(string.Format("//div[@id='search_results_inner']/table/tbody/tr[*]/td[position()=2 and contains(normalize-space(string(.)), '{0}')]/ancestor::tr/td[1]/button", individualOrOrganizationName)), 60);
            this.WaitForElement(this._driver, By.XPath(string.Format("//b[text()='{0}']/parent::td/preceding::td[1]/button", individualOrOrganizationName)), 60);
            // Click select
            //this._driver.FindElementByCssSelector("//div[@id='search_results_inner']/table/tbody/tr[2]/td[1]/button").Click();
            //this._driver.FindElementByXPath(string.Format("//div[@id='search_results_inner']/table/tbody/tr[*]/td[position()=2 and contains(normalize-space(string(.)), '{0}')]/ancestor::tr/td[1]/button", individualOrOrganizationName)).Click();
            this._driver.FindElementByXPath(string.Format("//b[text()='{0}']/parent::td/preceding::td[1]/button", individualOrOrganizationName)).Click();
        }
        
        /// <summary>
        /// Selects/Deselects a checkbox on the screen
        /// </summary>
        /// <param name="element">The path of the checkbox element</param>
        /// <param name="yesNo">Do you want it check?</param>
        public void SelectCheckbox(By element, bool yesNo)
        {
            if (yesNo)
            {
                if (!this._driver.FindElement(element).Selected)
                {
                    this._driver.FindElement(element).Click();
                }
            }
            else
            {
                if (this._driver.FindElement(element).Selected)
                {
                    this._driver.FindElement(element).Click();
                }
            }
        }

        /// <summary>
        /// Returns data text from a given table
        /// </summary>
        /// <param name="tableElement">The table element</param>
        /// <param name="row">The row the data is located</param>
        /// <param name="column">The column</param>
        /// <param name="columnType">The column type (default=tr)</param>
        /// <returns>Text</returns>
        public string GetDataFromTable(By tableElement, int row, int column, string columnType="td")
        {
            IWebElement table = this._driver.FindElement(tableElement);
            return table.FindElements(By.TagName("tr"))[row].FindElements(By.TagName(columnType))[column].Text;        
            
        }
        #endregion Instance Methods

        #region Private Methods
        private void DoVerifyDateControl(string dateControlID, string rawDate, string expectedFormattedDate) {
            string focus = "//input[@id='submitQuery' or @id='ctl00_content_btnSave' or @id='btn_submit']"; //submitQuery

            this._selenium.Focus(dateControlID);
            this._selenium.Type(dateControlID, "");
            this._selenium.Focus(dateControlID);
            this._selenium.TypeKeys(dateControlID, rawDate);
            this._selenium.Focus(focus);
            this._selenium.WaitForCondition(string.Format("selenium.browserbot.getCurrentWindow().document.getElementById('{0}').value.match('{1}')", dateControlID, "[0-9]{1,2}/[0-9]{1,2}/[0-9]{4}"), "10000");
            Assert.AreEqual(expectedFormattedDate, this._selenium.GetValue(dateControlID));
        }

        private void DoVerifyDateControlWebDriver(string dateControlID, string rawDate, string expectedFormattedDate) {
            IWebElement dateControl = this._driver.FindElementById(dateControlID);

            dateControl.Clear();
            dateControl.SendKeys(rawDate);
            this._driver.FindElementByXPath("//input[@id='submitQuery' or @id='ctl00_content_btnSave' or @id='btn_submit']").SendKeys("1");
            Assert.AreEqual(expectedFormattedDate, dateControl.GetAttribute("value"));
        }


        /// <summary>
        /// Performs the seach and selection of an individual from the find person popup.
        /// </summary>
        /// <param name="test">The current test object</param>
        /// <param name="elementID">The ID of the find person link</param>
        /// <param name="individual">The individual to be searched and selected</param>
        private void DoSelectIndividualFromFindPersonPopup(string elementID, string individual) {
            string pageTitle = this._selenium.GetTitle();
            string pageLocation = this._selenium.GetLocation();

            if (!string.IsNullOrEmpty(elementID)) {
                //this._selenium.ClickAndSelectPopUp(elementID, "psuedoModal", "10000");
                this._selenium.Click(elementID);
                this._selenium.WaitForPopUp("psuedoModal", "30000");
                this._selenium.SelectPopUp("psuedoModal");
            }
            else {
                //this._selenium.ClickAndSelectPopUp("//a[@id='ctl00_ctl00_MainContent_content_ctlFindPerson_lnkFindPerson' or @id='ctl00_ctl00_MainContent_content_Addeditstaff1_ctlFindPerson_lnkFindPerson']", "psuedoModal", "10000");
                this._selenium.Click("//a[@id='ctl00_ctl00_MainContent_content_ctlFindPerson_lnkFindPerson' or @id='ctl00_ctl00_MainContent_content_Addeditstaff1_ctlFindPerson_lnkFindPerson']");
                this._selenium.WaitForPopUp("psuedoModal", "30000");
                this._selenium.SelectPopUp("psuedoModal");
            }

            string indQuery = individual.Split(' ').Length > 2 ? string.Format("{0} {1}", individual.Split(' ')[0], individual.Split(' ')[2]) : individual;
            this._selenium.Type("ctl00_content_txtName_textBox", indQuery);
            //this._selenium.KeyPressAndWaitForCondition("ctl00_content_btnSearchPeople", "\\13", this._javascript.IsElementPresent("ctl00_content_dgSearchResults"), "15000");
            this._selenium.KeyPressAndWaitForCondition("ctl00_content_btnSearchPeople", "\\13", string.Format("selenium.isElementPresent(\"xpath=//table[@id='ctl00_content_dgSearchResults']/tbody/tr[*]/td[2]/strong[text()='{0}']\");", indQuery), "15000");
            this._selenium.KeyPress("ctl00_content_dgSearchResults_ctl02_lnkSelect", "\\13");
            //this._selenium.Close();

            //this._selenium.DeselectPopUp();
            //this._selenium.SelectWindow(null);
            this._selenium.SelectWindow(pageTitle);

            try {
                this._selenium.WaitForPageToLoad("60000");
            }
            catch (SeleniumException) {
                foreach (var window in this._selenium.GetAllWindowNames()) {
                    TestLog.WriteLine(window);
                }
                this._selenium.OpenWindow(pageLocation, "selenium_main_app_window");
                throw;
            }

        }

        /// <summary>
        /// Performs the seach and selection of an individual from the find person popup.
        /// </summary>
        /// <param name="test">The current test object</param>
        /// <param name="elementID">The ID of the find person link</param>
        /// <param name="individual">The individual to be searched and selected</param>
        public void SelectIndividualFromFindPersonPopupWebDriver(string individual, [Optional] string controlId) {
            string target = !string.IsNullOrEmpty(controlId) ? controlId : "//a[@id='ctl00_ctl00_MainContent_content_ctlFindPerson_lnkFindPerson' or @id='ctl00_ctl00_MainContent_content_Addeditstaff1_ctlFindPerson_lnkFindPerson']";

            this._driver.FindElementByXPath(target).Click();
            this._driver.SwitchTo().Window("psuedoModal");
            
            string indQuery = individual.Split(' ').Length > 2 ? string.Format("{0} {1}", individual.Split(' ')[0], individual.Split(' ')[2]) : individual;
            this._driver.FindElementById("ctl00_content_txtName_textBox").SendKeys(indQuery);
            this._driver.FindElementById("ctl00_content_btnSearchPeople").Click();
            // Sleep 3 seconds before select individual searched out.
            System.Threading.Thread.Sleep(3000);
            this._driver.FindElementById("ctl00_content_dgSearchResults_ctl02_lnkSelect").Click();
            this._driver.SwitchTo().Window(this._driver.WindowHandles[0]);
        }
        #endregion Private Methods

        public void VerifyElementPresentWebDriver(System.Func<string, By> func, string p) {
            throw new NotImplementedException();
        }

        
        #region AMS Setup
        //Moved to SQL Class
        //public Dictionary<int, string> TokenizeAmsChurches(string amsChurchesValue, SQL sql)
        //{
        //    //Instantiate variables so they at least be empty
        //    DataTable amdDT = new DataTable();
        //    string[] amsChurches = null;
        //    Dictionary<int, string> amsChurchesDict = new Dictionary<int, string>();
        //    Dictionary<int, string> nonAmsChurches = new Dictionary<int, string>();

        //    //Set List of Test Churches
        //    List<string> amsTestChurches = new List<string>();
        //    amsTestChurches.Add("DC");
        //    amsTestChurches.Add("QAEUNLX0C1"); 
        //    amsTestChurches.Add("QAEUNLX0C2");
        //    amsTestChurches.Add("QAEUNLX0C3");
        //    amsTestChurches.Add("QAEUNLX0C4");
        //    amsTestChurches.Add("QAEUNLX0C6");
        //    amsTestChurches.Add("DCDMDFWTX");

        //    StringBuilder query = new StringBuilder("SELECT [CHURCH_ID], [CHURCH_NAME], [CHURCH_CODE] FROM [ChmChurch].[dbo].[CHURCH] WITH (NOLOCK)");
        //    amdDT = sql.Execute(query.ToString());

        //    log.Debug(string.Format("AMS Churches: {0}", amsChurchesValue));

        //    //Remove spaces before tokenize amsChurches
        //    amsChurchesValue.Trim().Replace(" ", "");
        //    amsChurches = amsChurchesValue.Split(',');

        //    //Sift through passed in ams church values and get the churchIDs and add them to the AMS church list
        //    foreach (string amsChurch in amsChurches)
        //    {
        //        string church = amsChurch.Trim();
        //        if (!string.IsNullOrWhiteSpace(church))
        //        {
        //            amsChurchesDict.Add(sql.FetchChurchID(church), church);
        //            log.Debug(string.Format("AMS Church Code: {0}", church));
        //        }
        //    }

        //    /*foreach (string amsChurch in amsChurches) 
        //    {                
        //        //log.Info(string.Format("AMS Church: {0}", amsChurch.Trim()));
        //        foreach (DataRow dr in amdDT.Rows)
        //        {
        //            //log.Debug(string.Format("Church Code: {0}", dr["CHURCH_CODE"].ToString()));
        //            //Add to AMS list
        //            if (dr["CHURCH_CODE"].ToString().Equals(amsChurch.Trim()))
        //            {
        //                if (!amsChurchesDict.ContainsKey(Convert.ToInt32(dr["CHURCH_ID"])))
        //                {
        //                    log.Debug(string.Format("AMS Church Code: {0} [{1}] ", amsChurch.Trim(), dr["CHURCH_ID"].ToString()));
        //                    amsChurchesDict.Add(Convert.ToInt32(dr["CHURCH_ID"]), dr["CHURCH_CODE"].ToString());
        //                }
        //            }                    
        //        }
        //    }
        //    */

        //    //Add non AMS churches to the nonAmsChurch list
        //    //This will only be our Test Churches
        //    //DC, QAEUNLX0C1, QAEUNLX0C2, QAEUNLX0C3, QAEUNLX0C4, QAEUNLX0C6, DCDMDFWTX
        //    foreach (string amsTestChurch in amsTestChurches)
        //    {
        //        if (!amsChurchesDict.ContainsValue(amsTestChurch))
        //        {
        //            log.Debug(string.Format("*** Non AMS Church Code: {0} ", amsTestChurch));
        //            nonAmsChurches.Add(sql.FetchChurchID(amsTestChurch), amsTestChurch);
        //        }
        //    }

        //    /*foreach (string amsChurch in amsChurches){

        //        foreach(DataRow dr in amdDT.Rows)
        //        {
        //            if (!dr["CHURCH_CODE"].ToString().Equals(amsChurch.Trim()))
        //            {
        //                if(!nonAmsChurches.ContainsKey(Convert.ToInt32(dr["CHURCH_ID"])) && (!amsChurchesDict.ContainsKey(Convert.ToInt32(dr["CHURCH_ID"]))))
        //                {
        //                    //log.Debug(string.Format("*** Non AMS Church Code: {0} [{1}] ", dr["CHURCH_CODE"].ToString(), dr["CHURCH_ID"].ToString()));
        //                    nonAmsChurches.Add(Convert.ToInt32(dr["CHURCH_ID"]), dr["CHURCH_CODE"].ToString());                            
        //                }

        //            }
        //        }
        //    }*/
             

            
        //    //Set it to access it later
        //    this._amsChurches = amsChurchesDict;
        //    this._nonAmsChurches = nonAmsChurches;

        //    //Print what we got
        //    foreach (KeyValuePair<int, string> amsChurch in this._amsChurches)
        //    {
        //        log.Debug(string.Format("AMS Church Code: {0} [{1}] ", amsChurch.Key, amsChurch.Value));
        //    }

        //    foreach (KeyValuePair<int, string> nonAmsChurch in this._nonAmsChurches)
        //    {
        //        log.Debug(string.Format("Non AMS Church Code: {0} [{1}] ", nonAmsChurch.Key, nonAmsChurch.Value));
        //    }

        //    //Do we really want to return it?
        //    return _amsChurches;
        //}

        //public void EnableDisableAMSChurches(bool amsEnabled, SQL sql)
        //{
        //    //Init to flag to disabled (1)
        //    int enabledDisabled = 1;
        //    List<string> amsTableEntries = null;

        //    log.InfoFormat("AMS Enabled: {0}", amsEnabled);

        //    //Get Global AMS used for checking through if church is enabled or not
        //    //sql.AMS_Enable_Status_Add();
        //    amsTableEntries = sql.QueryAMSTable();

        //    //If specified to enable AMS churches but there are not then kill everything else code will remove all churches
        //    if ((amsEnabled) && (this._amsChurches.Count == 0)) { throw new Exception(string.Format("AMS was set to {0} and there were no churches specified", amsEnabled)); }

        //    //Set flag to enabled (0)
        //    if (amsEnabled) { enabledDisabled = 0; };

        //    log.InfoFormat("AMS Enabled Flag: {0}", enabledDisabled);
        
        //    //Let's make sure we have ams populated data to set enableDisable flag
        //    if (this._amsChurches.Count > 0)
        //    {

        //        foreach (KeyValuePair<int, string> churchID in _amsChurches)
        //        {
        //            //Is ChurchId already in table                    
        //            //rows = sql.Query_DelegateChurchToAms(churchID).Rows.Count;
        //            //log.DebugFormat("Rows found in DelegateChurchToAms: {0}", rows);
        //            amsTableEntries = sql.QueryAMSTable();

        //            //Is it in DelegateChurchToAms table
        //            //Enabled/Disable it
        //            if (amsTableEntries.Contains(Convert.ToString(churchID.Key)))
        //            {
        //                if (!sql.AMS_Church_Status(churchID.Key) == amsEnabled)
        //                {
        //                    sql.Update_DelegateChurchToAms(churchID, amsEnabled, enabledDisabled);
        //                }
        //                else
        //                {
        //                    log.DebugFormat("AMS status for Church ID [{0}] already [{1}]", churchID.Value, amsEnabled);
        //                }
        //            }
        //            else
        //            {
        //                sql.Insert_DelegateChurchToAms(churchID, amsEnabled, enabledDisabled);
        //            }

        //        }
        //    }

        //    //Remove all other non AMS churches from DelegateChurchToAms table
        //    amsTableEntries = sql.QueryAMSTable();
        //    foreach (KeyValuePair<int, string> churchID in _nonAmsChurches)
        //    {
        //        foreach (string amsTableEntry in amsTableEntries)
        //        {
        //            if (Convert.ToString(churchID.Key) == amsTableEntry)
        //            {
        //                sql.Delete_DelegateChurchToAms(churchID.Key);
        //            }
        //        }                
        //    }

        //    //Set Global AMS used for checking through if church is enabled or not
        //    sql.AMS_Enable_Status_Add();

        //}        
        
        #endregion AMS Seutp

        /// <summary>
        /// Is Element Exist.
        /// </summary>
        /// <param name="by">query condition</param>
        /// <returns>element is exist.</returns>
        public bool IsElementExist(By by)
        {
            bool isExist = true;
            try
            {
                this.WaitForElement(by);
            }
            catch
            {
                isExist = false;
            }
            return isExist;
        }

    }

    [TestFixture]
    public class FixtureBaseWebDriver{

        private ConcurrentDictionary<string, TestBaseWebDriver> _testContainer = new ConcurrentDictionary<string, TestBaseWebDriver>();
        protected F1Environments _f1Environment;
        private ExeConfigurationFileMap _appConfigFileMap = new ExeConfigurationFileMap();
        protected Configuration _configuration;
        protected SQL _sql;
        protected APIBase _api;
        protected string _dbConnectionString;
        private DeleteAllEmails _emails;
        private Dictionary<int, string> _amsChurches = new Dictionary<int, string>();
        private bool _amsEnabled = false;
        private bool _amsMigrated = false;

        protected TestBaseWebDriver test;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Properties
        public ConcurrentDictionary<string, TestBaseWebDriver> TestContainer {
            get { return _testContainer; }
        }

        public string DBConnectionString {
            get { return _dbConnectionString; }
        }

        public SQL SQL {
            get { return _sql; }
        }

        public F1Environments F1Environment {
            get { return _f1Environment; }
        }

        public APIBase API
        {

            get { return _api; }
        }

        public DeleteAllEmails DeleteAllEmails
        {
            get { return _emails; }
        }

        public Dictionary<int, string> AMSChurches
        {
            get { return this._amsChurches; }
        }

        public bool AMSEnabled
        {
            get { return this._amsEnabled; }
        }
                
        #endregion Properties

        public FixtureBaseWebDriver()
        {

            _appConfigFileMap.ExeConfigFilename = @"..\..\..\Common\bin\Debug\Common.dll.config";

            //FGJ
            log.Debug("Enter FixtureBaseWebDriver");

            // Configure and open the config file
            log.Debug("Load Variables");
            _configuration = ConfigurationManager.OpenMappedExeConfiguration(_appConfigFileMap, ConfigurationUserLevel.None);

            
            log.Debug("Set Environment Variables");
            // Set Environment variables
            this._f1Environment = (F1Environments)Enum.Parse(typeof(F1Environments), _configuration.AppSettings.Settings["FTTests.Environment"].Value);
            if (string.IsNullOrWhiteSpace(_configuration.AppSettings.Settings["FTTests.AMSEnabled"].Value))
            {
                this._amsEnabled = false;
            }
            else
            {
                this._amsEnabled = Boolean.Parse(_configuration.AppSettings.Settings["FTTests.AMSEnabled"].Value.ToLower());
            }

            if (string.IsNullOrWhiteSpace(_configuration.AppSettings.Settings["FTTests.AMSMigrated"].Value))
            {
                this._amsMigrated = false;
            }
            else
            {
                this._amsMigrated = Boolean.Parse(_configuration.AppSettings.Settings["FTTests.AMSMigrated"].Value.ToLower());
            }

            string amsChurchesValue = _configuration.AppSettings.Settings["FTTests.AMSChurches"].Value;

            log.Info("Enironment: " + this._f1Environment);
            log.Info("   Browser: " + _configuration.AppSettings.Settings["FTTests.Browser"].Value);
            log.Info(string.Format("AMS Enabled: {0}", this._amsEnabled));
            log.Info(string.Format("AMS Migrated: {0}", this._amsMigrated));
            log.Info(string.Format("AMS Churches: {0}", amsChurchesValue));

            // Set up the DB Connection String
            switch (this._f1Environment) {
                case F1Environments.LOCAL:
                    this._dbConnectionString = @"data source=localhost;initial catalog=ChmContribution;password=Fris121co;persist security info=True;user id=chm_system;packet size=4096";
                    break;

               /* case F1Environments.DEV:
                    this._dbConnectionString = @"data source=DEVDB.DEV.CORP.LOCAL,65316;initial catalog=ChmContribution;password=Fris121co;persist security info=True;user id=chm_system;packet size=4096";
                    break;
                case F1Environments.DEV1:
                    this._dbConnectionString = @"data source=DSQL10.DEV.CORP.LOCAL,65316;initial catalog=ChmContribution;password=Fris121co;persist security info=True;user id=chm_system;packet size=4096";
                    break;
                case F1Environments.DEV2:
                    this._dbConnectionString = @"data source=DSQL20.DEV.CORP.LOCAL,65316;initial catalog=ChmContribution;password=Fris121co;persist security info=True;user id=chm_system;packet size=4096";
                    break;
                case F1Environments.DEV3:
                    this._dbConnectionString = @"data source=DSQL30.DEV.CORP.LOCAL,65316;initial catalog=ChmContribution;password=Fris121co;persist security info=True;user id=chm_system;packet size=4096";
                    break;
                case F1Environments.QA:
                    this._dbConnectionString = @"data source=QADB.DEV.CORP.LOCAL,65317;initial catalog=ChmContribution;password=Fris121co;persist security info=True;user id=chm_system;packet size=4096";
                    break;
                //case F1Environments.STAGING:
                //    this._dbConnectionString = @"data source=ftstage.staging.dallas-dc.ft.com,65316;initial catalog=ChmPeople;password=staging;persist security info=True;user id=chm_system;packet size=4096";
                //    break; */

                case F1Environments.LV_PROD:
                case F1Environments.PRODUCTION:
                    break;
                case F1Environments.LV_QA:
                case F1Environments.LV_UAT:
                case F1Environments.STAGING:
                case F1Environments.INT:
                case F1Environments.INT2:
                case F1Environments.INT3:
                    this._dbConnectionString = string.Format("data source=transdb.{0}.fellowshipone.com;initial catalog=ChmContribution;persist security info=True;integrated security=True;packet size=4096", PortalBase.GetLVEnvironment(this._f1Environment));                    
                    break;
                default:
                    throw new SeleniumException("Please select a valid environment!!!");
            }

            // Create a new SQL class
            if (this._f1Environment != F1Environments.PRODUCTION && this._f1Environment != F1Environments.LV_PROD)
            {
                log.Debug("Instantiate SQL DB " + this._dbConnectionString);
                this._sql = new SQL(this._dbConnectionString);

                //Get AMS Church Info
                try
                {
                    this._amsChurches = this._sql.TokenizeAmsChurches(amsChurchesValue, this._sql);
                    this._sql.EnableDisableAMSChurches(this._amsEnabled, this._amsMigrated, this._sql);
                }
                catch (Exception e)
                {
                    log.ErrorFormat("Error setting up AMS. {0}", e.StackTrace);
                    throw new WebDriverException(string.Format("Error setting up AMS. {0} {1}", e.Message, e.StackTrace));
                }
            }

            this._emails = new DeleteAllEmails();
        }

        [SetUp]
        public void SetUp()
        {

            //Adding log4net config setup in here
            //Log4net info is found in App.config --> Common.dll.config
            //XmlConfigurator.Configure(new System.IO.FileInfo(_appConfigFileMap.ExeConfigFilename));
            //log4net.Config.BasicConfigurator.Configure();
            //log4net.Config.XmlConfigurator.Configure();

            log.Debug("Enter Setup WebDriver");
            

            _api = new APIBase();

            // Create a new test object to represent the current test that just called SetUp
            test = new TestBaseWebDriver(this._f1Environment, this._dbConnectionString, this._configuration, this._sql, this._api);

            // Add the test object to the container
            log.DebugFormat("Add {0} to testContainer", Gallio.Framework.TestContext.CurrentContext.Test.Name);
            this._testContainer.TryAdd(Gallio.Framework.TestContext.CurrentContext.Test.Name, test);

            log.Debug("Exit Setup WebDriver");

        }

        [TearDown]
        public void TearDown() {

            log.Debug("Enter TearDown WebDriver");
            log.DebugFormat("Test Case [{0}]: ", Gallio.Framework.TestContext.CurrentContext.Test.Name);

            // If the test fails, take a screen shot and store it to the log
            if (Gallio.Framework.TestContext.CurrentContext.Outcome == Gallio.Model.TestOutcome.Failed)
            {
                

                log.Error("Test Case Failed ... Taking screen shot");

                //ScreenShotRemoteWebDriver webDriver = (ITakesScreenshot)this._testContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name].ScreenShotDriver;
                //log.Debug("URL: " + webDriver.Url);
                //log.Debug("Cap: " + webDriver.Capabilities.BrowserName);
                
                try
                {

                    ITakesScreenshot webDriver = (ITakesScreenshot)this._testContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name].Driver;
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
                catch (Exception e)
                {
                    log.Fatal("Can't take screen shot: " + e.Message + "\n" + e.StackTrace);
                    
                }

            }

            try
            {
                if (this._testContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name].Driver != null)
                {
                    try
                    {
                        // Close browser and stop the test server
                        this._testContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name].Driver.Quit();

                    }
                    catch (Exception e)
                    {
                        TestLog.WriteLine(e.Message);
                        TestLog.WriteLine(e.StackTrace.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                TestLog.WriteLine(e.StackTrace.ToString());
            }
            finally
            {
                TestBaseWebDriver containerValue;
                this._testContainer.TryRemove(Gallio.Framework.TestContext.CurrentContext.Test.Name, out containerValue);

            }

            log.Debug("Exit TearDown WebDriver");

        }

    }

    [TestFixture]
    public class FixtureBase {
        private ConcurrentDictionary<string, TestBase> _testContainer = new ConcurrentDictionary<string, TestBase>();
        private F1Environments _f1Environment;
        private IList<string> _errorText = new List<string>();
        private ExeConfigurationFileMap _appConfigFileMap = new ExeConfigurationFileMap();
        private Configuration _configuration;
        private SQL _sql;
        private string _dbConnectionString;
        private DeleteAllEmails _emails;
        private string _amsChurchValues;
        private bool _amsEnabled = false;
        private Dictionary<int, string> _amsChurches = new Dictionary<int, string>();
        private bool _amsMigrated = false;

        //FGJ log4net
        //private static readonly ILog log = LogManager.GetLogger(typeof(FixtureBase));
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        #region Properties
        public ConcurrentDictionary<string, TestBase> TestContainer {
            get { return _testContainer; }
        }

        public string DBConnectionString {
            get { return _dbConnectionString; }
        }

        public SQL SQL {
            get { return _sql; }
        }

        public F1Environments F1Environment {
            get { return _f1Environment; }
        }

        public DeleteAllEmails DeleteAllEmails
        {
            get { return _emails; }
        }

        public bool AMSEnabled
        {
            get { return this._amsEnabled; }
        }

        public Dictionary<int, string> AMSChurches
        {
            get { return this._amsChurches; }
        }

        public string AMSChurcheValues
        {
            get { return this._amsChurchValues; }
        }
        #endregion Properties

        public FixtureBase() {

            _appConfigFileMap.ExeConfigFilename = @"..\..\..\Common\bin\Debug\Common.dll.config";

            log.Debug("Enter Fixture Base");

            // Configure and open the config file
            log.Debug("Load Variables");            
            _configuration = ConfigurationManager.OpenMappedExeConfiguration(_appConfigFileMap, ConfigurationUserLevel.None);


            log.Debug("Set Environments");

            // Set Environment variables
            this._f1Environment = (F1Environments)Enum.Parse(typeof(F1Environments), _configuration.AppSettings.Settings["FTTests.Environment"].Value);
            if (string.IsNullOrWhiteSpace(_configuration.AppSettings.Settings["FTTests.AMSEnabled"].Value))
            {
                this._amsEnabled = false;
            }
            else
            {
                this._amsEnabled = Boolean.Parse(_configuration.AppSettings.Settings["FTTests.AMSEnabled"].Value.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(_configuration.AppSettings.Settings["FTTests.AMSMigrated"].Value))
            {
                this._amsMigrated = Boolean.Parse(_configuration.AppSettings.Settings["FTTests.AMSMigrated"].Value.ToLower());
            }

            _amsChurchValues = _configuration.AppSettings.Settings["FTTests.AMSChurches"].Value;
            
            log.Info(string.Format("Enironment: {0}", this._f1Environment));
            log.Info(string.Format("Browser: {0}", _configuration.AppSettings.Settings["FTTests.Browser"].Value));
            log.Info(string.Format("AMS Enabled: {0}", this._amsEnabled));
            log.Info(string.Format("AMS Migrated: {0}", this._amsMigrated));
            log.Info(string.Format("AMS Churches: {0}", _amsChurchValues));

            // Set up the DB Connection String
            switch (this._f1Environment) {
                case F1Environments.LOCAL:
                    this._dbConnectionString = @"data source=localhost;initial catalog=ChmContribution;password=Fris121co;persist security info=True;user id=chm_system;packet size=4096";
                    break;
               /* case F1Environments.DEV:
                    this._dbConnectionString = @"data source=DEVDB.DEV.CORP.LOCAL,65316;initial catalog=ChmContribution;password=Fris121co;persist security info=True;user id=chm_system;packet size=4096";
                    break;
                case F1Environments.DEV1:
                    this._dbConnectionString = @"data source=DSQL10.DEV.CORP.LOCAL,65316;initial catalog=ChmContribution;password=Fris121co;persist security info=True;user id=chm_system;packet size=4096";
                    break;
                case F1Environments.DEV2:
                    this._dbConnectionString = @"data source=DSQL20.DEV.CORP.LOCAL,65316;initial catalog=ChmContribution;password=Fris121co;persist security info=True;user id=chm_system;packet size=4096";
                    break;
                case F1Environments.DEV3:
                    this._dbConnectionString = @"data source=DSQL30.DEV.CORP.LOCAL,65316;initial catalog=ChmContribution;password=Fris121co;persist security info=True;user id=chm_system;packet size=4096";
                    break;
                case F1Environments.QA:
                    this._dbConnectionString = @"data source=QADB.DEV.CORP.LOCAL,65317;initial catalog=ChmContribution;password=Fris121co;persist security info=True;user id=chm_system;packet size=4096";
                    break;
                case F1Environments.STAGING:
                    //this._dbConnectionString = @"data source=ftstage.staging.dallas-dc.ft.com,65316;initial catalog=ChmPeople;password=staging;persist security info=True;user id=chm_system;packet size=4096";
                    //this._dbConnectionString = string.Format("data source=transdb.{0}.fellowshipone.com;initial catalog=ChmContribution;persist security info=True;integrated security=True;packet size=4096", PortalBase.GetLVEnvironment(this._f1Environment));
                    //break; */

                case F1Environments.LV_QA:
                case F1Environments.LV_UAT:
                case F1Environments.STAGING:
                case F1Environments.INT:
                case F1Environments.INT2:
                case F1Environments.INT3:
                case F1Environments.LV_PROD:
                    //this._dbConnectionString = @"data source=transdb;initial catalog=ChmChurch;persist security info=True;integrated security=True;packet size=4096";
                    //this._dbConnectionString = @"data source=transdb.qa.fellowshipone.com;initial catalog=ChmContribution;persist security info=True;integrated security=True;packet size=4096";
                    //this._dbConnectionString = @"data source=wsf1qadb10vs.dev.activenetwork.com;initial catalog=ChmContribution;persist security info=True;integrated security=True;packet size=4096";
                    this._dbConnectionString = string.Format("data source=transdb.{0}.fellowshipone.com;initial catalog=ChmContribution;persist security info=True;integrated security=True;packet size=4096", PortalBase.GetLVEnvironment(this._f1Environment));
                    break;
                default:
                    log.Error("Please select a valid environment!!!");
                    throw new SeleniumException("Please select a valid environment!!!");
            }

            // Create a new SQL class            
            log.Debug("Instantiate SQL DB " + this._dbConnectionString);
            this._sql = new SQL(this._dbConnectionString);

            //Get AMS Church Info
            this._amsChurches = this._sql.TokenizeAmsChurches(_amsChurchValues, this._sql);
            this._sql.EnableDisableAMSChurches(this._amsEnabled, this._amsMigrated, this._sql);

            this._emails = new DeleteAllEmails();

        }

        [SetUp]
        public void SetUp() {

            //TODO BIG TIME!!!
            //FGJ
            //Adding log4net config setup in here
            //I'll get creative later. Let's just test it out on a real environment. For now it has been set in AseemblyFixture.
            //log4net.Config.BasicConfigurator.Configure();

            //Getting log4net:ERROR Could not create Appender [AutomationPatternFileAppender] of type [log4net.Appender.RollingPatternFileAppender]. Reported error follows
            //Investigating more later.
            //String log4NetFileName = @"..\..\..\Common\bin\Debug\Common.dll.config";
            //XmlConfigurator.Configure(new System.IO.FileInfo(log4NetFileName));
            //log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(log4NetFileName));

            //Log4net info is found in App.config --> Common.dll.config
            //XmlConfigurator.Configure(new System.IO.FileInfo(_appConfigFileMap.ExeConfigFilename));

            //log4net.Config.XmlConfigurator.Configure();

            log.Debug("Enter Setup");
            // Create a new test object to represent the current test that just called SetUp
            TestBase test = null;

            //try
            //{

                test = new TestBase(this._f1Environment, this._dbConnectionString, this._configuration, this._sql);
                // Add the test object to the container
                this._testContainer.TryAdd(Gallio.Framework.TestContext.CurrentContext.Test.Name, test);

            /*}            
            catch (SeleniumException se)
            {

                TestLog.WriteException(se, "Error in Selenium Setup");
                log.Fatal("Error Test", se);
                throw new Exception(se.StackTrace);
            }
            catch (Exception e)
            {
                TestLog.WriteException(e, "Error in Setup");
                log.Fatal("Error Test", e);
                throw new Exception(e.StackTrace);
            }
            */

        }

        [TearDown]
        public void TearDown() {

            log.Debug("Enter TearDown");

            TestLog.WriteLine("Test Name: " + Gallio.Framework.TestStep.CurrentStep.Name);

            // If the test fails, take a screen shot and store it to the log
            if (Gallio.Framework.TestContext.CurrentContext.Outcome == Gallio.Model.TestOutcome.Failed) {

                try
                {
                    // Convert Base64 String to byte[]
                    log.Info("Test Failed ... taking screen shot");

                    byte[] imageBytes = Convert.FromBase64String(this._testContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name].Selenium.CaptureEntirePageScreenshotToString(null));

                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream(imageBytes, 0, imageBytes.Length))
                    {
                        // Convert byte[] to Image
                        ms.Write(imageBytes, 0, imageBytes.Length);
                        System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);

                        // Embed the image to the log
                        TestLog.EmbedImage(null, image);
                    }
                }
                catch (Exception e)
                {
                    log.Fatal("Tear Down Error", e);
                    TestLog.WriteLine(e.Message);
                }
            }

            if (this._testContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name].Selenium != null) {
                try {
                    // Close browser and stop the test server
                    log.Debug("Close Browser");
                    this._testContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name].Selenium.Stop();
                    TestBase containerValue;
                    this._testContainer.TryRemove(Gallio.Framework.TestContext.CurrentContext.Test.Name, out containerValue);
                }
                catch (Exception e) {
                    log.Error("Error closing browser " + e.Message);
                    TestLog.WriteLine(e.Message);
                }
            }
        }
    }
    #region Hashcode 

    public class HashCodeHelper
    {


        public static int CombineHashCodes(params object[] args)
        {
            return CombineHashCodes(EqualityComparer<object>.Default, args);
        }


        public static int CombineHashCodes(IEqualityComparer comparer, params object[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            if (args.Length == 0)
            {
                throw new ArgumentException("args");
            }

            var hashcode = 0;

            unchecked
            {
                // Commented the old id calculation logic, and replaced it with the new logic provided by Rock on FO-4113
                // hashcode = args.Aggregate(hashcode, (current, arg) => (current << 5) - current ^ comparer.GetHashCode(arg));
                hashcode = args.Aggregate(hashcode, (current, arg) => ((current << 5) + current) ^ comparer.GetHashCode(arg));
            }

            return hashcode;
        }


    }





    #endregion Hashcode

    #region Old Stuff
    public abstract class Setup {
        private string _serverHost;
        private string _browser;
        private F1Environments _f1Environment;

        private int _individualId;
        public int IndividualId {
            get { return _individualId = Convert.ToInt32(ExecuteDBQuery("SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = 15 AND FIRST_NAME = 'Matthew' AND Last_Name = 'Sneeden'").Rows[0]["INDIVIDUAL_ID"]); }
        }

        private ConcurrentDictionary<string, TestBaseOld> _testContainer = new ConcurrentDictionary<string, TestBaseOld>();
        protected ConcurrentDictionary<string, TestBaseOld> TestContainer {
            get { return _testContainer; }
        }

        private ExeConfigurationFileMap _appConfigFileMap = new ExeConfigurationFileMap();
        public ConfigurationFileMap AppConfigFileMap {
            get { return _appConfigFileMap; }
        }

        private Configuration _configuration;
        protected Configuration Configuration {
            get { return _configuration; }
        }

        public Setup() {
            // Configure and open the config file
            _appConfigFileMap.ExeConfigFilename = @"..\..\..\Common\bin\Debug\Common.dll.config";
            _configuration = ConfigurationManager.OpenMappedExeConfiguration(_appConfigFileMap, ConfigurationUserLevel.None);

            // Set Selenium and environment variables
            this._serverHost = _configuration.AppSettings.Settings["FTTests.Host"].Value;
            this._browser = _configuration.AppSettings.Settings["FTTests.Browser"].Value;
            this._f1Environment = (F1Environments)Enum.Parse(typeof(F1Environments), _configuration.AppSettings.Settings["FTTests.Environment"].Value);
        }

        protected string ServerHost {
            get { return _serverHost; }
        }

        protected string Browser {
            get { return _browser; }
        }

        public F1Environments F1Environment {
            get { return _f1Environment; }
        }

        protected string GetDBConnectionString {
            get {
                string returnValue = string.Empty;

                switch (_f1Environment) {
                    case F1Environments.LOCAL:
                    
                    /*case F1Environments.DEV:
                        returnValue = @"data source=DEVDB.DEV.CORP.LOCAL,65316;initial catalog=ChmContribution;password=Fris121co;persist security info=True;user id=chm_system;packet size=4096";
                        break;
                    case F1Environments.DEV1:
                        returnValue = @"data source=DSQL10.DEV.CORP.LOCAL,65316;initial catalog=ChmContribution;password=Fris121co;persist security info=True;user id=chm_system;packet size=4096";
                        break;
                    case F1Environments.DEV2:
                        returnValue = @"data source=DSQL20.DEV.CORP.LOCAL,65316;initial catalog=ChmContribution;password=Fris121co;persist security info=True;user id=chm_system;packet size=4096";
                        break;
                    case F1Environments.DEV3:
                        returnValue = @"data source=DSQL30.DEV.CORP.LOCAL,65316;initial catalog=ChmContribution;password=Fris121co;persist security info=True;user id=chm_system;packet size=4096";
                        break; */

                    case F1Environments.QA:
                        returnValue = @"data source=QADB.DEV.CORP.LOCAL,65317;initial catalog=ChmContribution;password=Fris121co;persist security info=True;user id=chm_system;packet size=4096";
                        break;
                    case F1Environments.STAGING:
                        returnValue = @"data source=ftstage.staging.dallas-dc.ft.com,65316;initial catalog=ChmPeople;password=staging;persist security info=True;user id=chm_system;packet size=4096";
                        break;
                    default:
                        throw new SeleniumException("Please select a valid environment!!!");
                }
                return returnValue;
            }
        }


        protected DataTable ExecuteDBQuery(string query) {
            DataTable results;
            using (SqlConnection dbConnection = new SqlConnection(GetDBConnectionString)) {
                dbConnection.Open();

                using (SqlDataAdapter dbAdapter = new SqlDataAdapter(query, dbConnection)) {
                    results = new DataTable();
                    dbAdapter.Fill(results);
                }
                dbConnection.Close();
            }
            return results;
        }
    }

    [TestFixture]
    [Obsolete("Use FixtureBase")]
    public class FixtureBaseOld : Setup {
        [SetUp]
        public void SetUp() {
            TestBaseOld testBase = new TestBaseOld();
            testBase.PortalUser = Configuration.AppSettings.Settings["FTTests.PortalUser"].Value;
            testBase.PortalUsername = Configuration.AppSettings.Settings["FTTests.PortalUsername"].Value;
            testBase.PortalPassword = Configuration.AppSettings.Settings["FTTests.PortalPassword"].Value;
            testBase.ChurchCode = Configuration.AppSettings.Settings["FTTests.ChurchCode"].Value;
            
            testBase.InFellowshipEmail = Configuration.AppSettings.Settings["FTTests.InFellowshipEmail"].Value;
            testBase.InFellowshipPassword = Configuration.AppSettings.Settings["FTTests.InFellowshipPassword"].Value;
            testBase.InFellowshipChurchCode = Configuration.AppSettings.Settings["FTTests.InFellowshipChurchCode"].Value;
            testBase.InFellowshipUser = Configuration.AppSettings.Settings["FTTests.InFellowshipUser"].Value;

            testBase.WebLinkPassword = Configuration.AppSettings.Settings["FTTests.WebLinkPassword"].Value;
            testBase.WebLinkChurchCode = Configuration.AppSettings.Settings["FTTests.WeblinkChurchCode"].Value;

            testBase.Email = Configuration.AppSettings.Settings["FTTests.Email"].Value;

            testBase.F1Environment = base.F1Environment;
            testBase.Selenium = new DefaultSelenium(ServerHost, 4444, Browser, testBase.GetPortalUrl());
            testBase.Selenium.Start();

            // Maximize the window if running the test locally
            if (base.ServerHost == "localhost") {
                testBase.Selenium.WindowMaximize();
            }

            // Add the test object to the container
            base.TestContainer.TryAdd(Gallio.Framework.TestContext.CurrentContext.Test.Name, testBase);
        }

        [TearDown]
        public void TearDown() {
            // Set the end test time
            base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name].TestEndTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

            // If the test fails, take a screen shot and store it to the log
            if (Gallio.Framework.TestContext.CurrentContext.Outcome == Gallio.Model.TestOutcome.Failed) {
                // Convert Base64 String to byte[]
                byte[] imageBytes = Convert.FromBase64String(base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name].Selenium.CaptureEntirePageScreenshotToString(null));
                System.IO.MemoryStream ms = new System.IO.MemoryStream(imageBytes, 0, imageBytes.Length);

                // Convert byte[] to Image
                ms.Write(imageBytes, 0, imageBytes.Length);
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);

                // Embed the image to the log
                TestLog.EmbedImage(null, image);
            }

            if (base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name].Selenium != null) {
                try {
                    // Close browser
                    base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name].Selenium.Stop();
                    TestBaseOld containerValue;
                    base.TestContainer.TryRemove(Gallio.Framework.TestContext.CurrentContext.Test.Name, out containerValue);
                }
                catch { }
            }
        }
    }

    public class TestBaseOld {
        protected ISelenium _selenium;
        protected F1Environments _f1Environment;
        protected DateTime _testStartTime;
        protected DateTime _testEndTime;
        protected IList<string> _errorText = new List<string>();
        protected string _dbConnectionString;
        protected string _serverHost;

        private string _portalUser;
        private string _portalUsername;
        private string _portalPassword;
        private string _churchCode;

        private string _infellowshipEmail;
        private string _infellowshipPassword;
        private string _infellowshipChurchCode;
        private string _infellowshipUser;

        private string _webLinkPassword;
        private string _webLinkChurchCode;

        private string _adhocReportingUserName;
        private string _adhocReportingPassword;
        private string _adhocReportingChurchCode;

        private string _email;

        public ISelenium Selenium {
            get { return _selenium; }
            set { _selenium = value; }
        }

        public F1Environments F1Environment {
            get { return _f1Environment; }
            set { _f1Environment = value; }
        }

        public DateTime TestStartTime {
            get { return _testStartTime; }
            set { _testStartTime = value; }
        }

        public DateTime TestEndTime {
            get { return _testEndTime; }
            set { _testEndTime = value; }
        }

        public string DBConnectionString {
            get { return _dbConnectionString; }
            set { _dbConnectionString = value; }
        }

        public IList<string> ErrorText {
            get { return _errorText; }
        }

        public string PortalUser {
            get { return _portalUser; }
            set { _portalUser = value; }
        }

        public string PortalUsername {
            get { return _portalUsername; }
            set { _portalUsername = value; }
        }

        public string PortalPassword {
            set { _portalPassword = value; }
        }

        public string ChurchCode {
            set { _churchCode = value; }
        }

        public string InFellowshipChurchCode {
            set { _infellowshipChurchCode = value; }
        }

        public string InFellowshipEmail {
            set { _infellowshipEmail = value; }
        }

        public string InFellowshipPassword {
            set { _infellowshipPassword = value; }
        }

        public string InFellowshipUser {
            get { return _infellowshipUser; }
            set { _infellowshipUser = value; }
        }

        public string WebLinkPassword {
            set { _webLinkPassword = value; }
        }

        public string WebLinkChurchCode {
            get { return _webLinkChurchCode; }
            set { _webLinkChurchCode = value; }
        }

        public string Email {
            get { return _email; }
            set { _email = value; }
        }

        public string AdhocReportingUserName {
            set { _adhocReportingUserName = value; }
        }

        public string AdhocReportingPassword {
            set { _adhocReportingPassword = value; }
        }

        public string AdhocReportingChurchCode {
            set { _adhocReportingChurchCode = value; }
        }

        #region FOOS
        /// <summary>
        /// Logs into FOOS.
        /// </summary>
        public void LoginFOOS() {
            // Open the foos login page
            this._selenium.Open(GetFOOSUrl());

            // Login to foos
            this._selenium.Type("txtUserName_textBox", "tcoulson"); //pm
            this._selenium.Type("txtPassword:textBox", "Tara.Coulson1");    //God!1st
            this._selenium.ClickAndWaitForPageToLoad("btnLogin");
        }

        /// <summary>
        /// Logs out of FOOS.
        /// </summary>
        public void LogoutFOOS() {
            this._selenium.ClickAndWaitForPageToLoad("link=Sign Out");
        }

        /// <summary>
        /// Fetches the url for FOOS depending on the environment.
        /// </summary>
        /// <returns>The FOOS url of the current environment</returns>
        public string GetFOOSUrl() {
            string returnValue = string.Empty;

            switch (this._f1Environment) {
                /*case F1Environments.DEV1:
                case F1Environments.DEV2:
                case F1Environments.DEV3:
                    returnValue = string.Format("http://manage.{0}.dev.corp.local/opsmanager/login.aspx", this._f1Environment.ToString().ToLower());
                    break; 
                case F1Environments.INTEGRATION:
                    returnValue = string.Format("http://manage.{0}.integration.corp.local/opsmanager/login.aspx", this._f1Environment);
                    break;
              case F1Environments.DEV: */ 

                case F1Environments.QA:
                    returnValue = string.Format("http://manage{0}.dev.corp.local/opsmanager/login.aspx", this._f1Environment);
                    break;
                case F1Environments.STAGING:
                    returnValue = "https://manage.staging.fellowshipone.com/opsmanager/login.aspx";
                    break;
                default:
                    throw new Exception("Not a valid option!");
            }
            return returnValue;
        }
        #endregion FOOS

        #region Portal
        /// <summary>
        /// Logs into portal using credentials from app.config
        /// </summary>
        public void LoginPortal() {
            // Log in
            DoLogin(null, null, null);
        }

        /// <summary>
        /// Logs into portal with the specified church code
        /// </summary>
        /// <param name="churchCode">Church code</param>
        public void LoginPortal(string churchCode) {
            DoLogin(null, null, churchCode);
        }

        /// <summary>
        /// Logs into portal using non-default username, password
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        public void LoginPortal(string username, string password) {
            DoLogin(username, password, null);
        }

        /// <summary>
        /// Logs into portal using non-default username, password, and church code
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="churchCode">Church code</param>
        public void LoginPortal(string username, string password, string churchCode) {
            DoLogin(username, password, churchCode);
        }

        /// <summary>
        /// Logs out of portal
        /// </summary>
        public void LogoutPortal() {
            // If user is on a minimal page, click the RETURN link
            if (this._selenium.IsElementPresent(GeneralLinks.RETURN)) {
                this._selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);
            }

            // And again...
            if (this._selenium.IsElementPresent(GeneralLinks.RETURN)) {
                this._selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);
            }

            // Check if a pop-out is open
            if (this._selenium.IsElementPresent(GeneralLinks.Close)) {
                //this._selenium.ClickAndWaitForCondition(GeneralLinks.Close, "var x = selenium.browserbot.getCurrentWindow().document.getElementById('success_close'); x.match('false');", "10000");
                this._selenium.Click(GeneralLinks.Close);
                System.Threading.Thread.Sleep(750);
            }

            // Log out of Fellowship One
            this._selenium.Click("link=sign out");
        }

        private void DoLogin(string username, string password, string CC) {
            // Open the web page
            this._selenium.Open(this.GetPortalUrl());

            // Set the username
            if (!string.IsNullOrEmpty(username)) {
                this._selenium.Type("ctl00_content_userNameText", username);
            }
            else {
                this._selenium.Type("ctl00_content_userNameText", _portalUsername);
            }

            // Set the password
            if (!string.IsNullOrEmpty(password)) {
                this._selenium.Type("ctl00_content_passwordText", password);
            }
            else {
                this._selenium.Type("ctl00_content_passwordText", _portalPassword);
            }

            // Set the church code
            if (!string.IsNullOrEmpty(CC)) {
                this._selenium.Type("ctl00_content_churchCodeText", CC);
            }
            else {
                this._selenium.Type("ctl00_content_churchCodeText", _churchCode);
            }

            // Login, wait for the page to load
            if (CC == "AdHoc") {
                this._selenium.Click("ctl00_content_btnLogin");
                this._selenium.WaitForPageToLoad("120000"); 
            }
            else {
                this._selenium.ClickAndWaitForPageToLoad("ctl00_content_btnLogin");
            }
            
            // Set the portal user if necessary
            if (username != "msneeden") {
                this._portalUser = this._selenium.GetText("//a[@href='/UserAccount/Index.aspx']");
            }
        }

        /// <summary>
        /// Fetches the url for Portal depending on the environment
        /// </summary>
        /// <returns>The Portal url of the current environment</returns>
        public string GetPortalUrl() {
            string returnValue = string.Empty;

            switch (this._f1Environment) {
                case F1Environments.LOCAL:
                    returnValue = "http://portal.local/login.aspx";
                    break;
                /*case F1Environments.DEV1:
                case F1Environments.DEV2:
                case F1Environments.DEV3:
                    returnValue = string.Format("http://portal.{0}.dev.corp.local/login.aspx", this._f1Environment.ToString().ToLower());
                    break; 
                case F1Environments.INTEGRATION:
                    returnValue = "http://portal.integration.corp.local/login.aspx";
                    break;
                case F1Environments.DEV: */

                case F1Environments.QA:
                    returnValue = string.Format("http://portal{0}.dev.corp.local/login.aspx", this._f1Environment);
                    break;
                case F1Environments.STAGING:
                    returnValue = "https://staging-www.fellowshipone.com/login.aspx";
                    break;
                default:
                    throw new Exception("Not a valid option!");
            }
            return returnValue;
        }

        public static string GetStaticFileServer(F1Environments f1Environment) {
            string returnValue = string.Empty;

            switch (f1Environment) {
               /* case F1Environments.DEV1:
                case F1Environments.DEV2:
                case F1Environments.DEV3:
                    returnValue = "/assets/images/";
                    break; */
                case F1Environments.QA:
                    returnValue = "http://contentqa.dev.corp.local/portal/images/";
                    break;
                case F1Environments.STAGING:
                    returnValue = "https://staging-static.fellowshipone.com/portal/images/";
                    break;
                default:
                    throw new Exception("Not a valid option!");
            }
            return returnValue;
        }
        #endregion Portal

        #region Report Library
        public void LoginReportLibrary() {
            // Open the report library login page
            this._selenium.Open(GetReportLibraryUrl());

            // Enter credentials
            this._selenium.Type("username", _portalUsername);
            this._selenium.Type("password", _portalPassword);
            this._selenium.Type("churchcode", _churchCode);

            // Attempt to login
            this._selenium.ClickAndWaitForPageToLoad("btn_login");
        }

        public void LogoutReportLibrary() {
            this._selenium.ClickAndWaitForPageToLoad("link=Log Out");
        }

        public string GetReportLibraryUrl() {
            string returnValue = string.Empty;

            switch (_f1Environment) {
               /* case F1Environments.DEV1:
                case F1Environments.DEV2:
                case F1Environments.DEV3:
                    returnValue = string.Format("http://reportlibrary.{0}.dev.corp.local/ReportLibrary/Login/Index.aspx", _f1Environment);
                    break;
                case F1Environments.INTEGRATION:
                    returnValue = "http://reportlibrary.integration.corp.local/ReportLibrary/Login/Index.aspx";
                    break;
                case F1Environments.DEV: */

                case F1Environments.QA:
                    returnValue = string.Format("http://reportlibrary{0}.dev.corp.local/ReportLibrary/Login/Index.aspx", _f1Environment);
                    break;
                    //case F1Environments.STAGING:
                    //returnValue = string.Format("https://{0}.{1}.infellowship.com/UserLogin", this._infellowshipChurchCode, PortalBase.GetLVEnvironment(this._f1Environment));
                    //returnValue = "https://staging-reportlibrary.fellowshipone.com/ReportLibrary/Login/Index.aspx";
                    //break;
                case F1Environments.LV_QA:
                case F1Environments.LV_UAT:
                case F1Environments.STAGING:
                case F1Environments.INT:
                case F1Environments.INT2:
                case F1Environments.INT3:
                case F1Environments.LV_PROD:
                    //https://dc.qa.f1infellowship.dev.activenetwork.com/UserLogin/Index?ReturnUrl=%2f
                    //returnValue = string.Format("https://{0}.{1}.f1infellowship.dev.activenetwork.com", this._infellowshipChurchCode, this._f1Environment);
                    returnValue = string.Format("https://{0}.{1}.infellowship.com/UserLogin", this._infellowshipChurchCode, PortalBase.GetLVEnvironment(this._f1Environment));
                    break;
                default:
                    throw new Exception("Not a valid option!");
            }
            return returnValue;
        }
        #endregion Report Library

        #region InFellowship
        /// <summary>
        /// Navigates to the InFellowship website.  Uses username, password, and church code specified specified in App.config.
        /// </summary>
        public void OpenInFellowship() {
            // Open the web page
            this.Selenium.Open(GetInFellowshipURL());
        }

        /// <summary>
        /// Logs the user in to InFellowship. Specific to a church code.  Uses username and password specified in App.config.
        /// </summary>
        /// <param name="churchCode">Church code to use for InFellowship.</param>
        public void OpenInFellowship(string churchCode) {
            // Open the web page
            this._infellowshipChurchCode = churchCode;
            this.Selenium.Open(GetInFellowshipURL());
        }

        /// <summary>
        /// Logs into infellowship using default credentials.
        /// </summary>
        public void LoginInFellowship() {
            DoLoginInFellowship(null, null, null);
        }

        /// <summary>
        /// Logs the user in to InFellowship. Specific to an InFellowship user and password.  Uses church code specified in App.config.
        /// </summary>
        /// <param name="infellowshipAccount">InFellowship account to log in with.</param>
        /// <param name="password">Password for the specified account.</param>
        public void LoginInFellowship(string infellowshipAccount, string password) {
            DoLoginInFellowship(infellowshipAccount, password, _infellowshipChurchCode);
        }

        /// <summary>
        /// Logs the user in to InFellowship.  Specific to an InFellowship user, password, and church code.
        /// </summary>
        /// <param name="infellowshipAccount">InFellowship account to log in with.</param>
        /// <param name="password">Password for the specified account.</param>
        /// <param name="churchCode">Church code to use for InFellowship.</param>
        public void LoginInFellowship(string infellowshipAccount, string password, string churchCode) {
            DoLoginInFellowship(infellowshipAccount, password, churchCode);
        }

        /// <summary>
        /// Logs the user out of InFellowship.
        /// </summary>
        public void LogoutInFellowship() {
            this._selenium.ClickAndWaitForPageToLoad("link=Sign out");
        }

        private void DoLoginInFellowship(string username, string password, string churchCode) {
            // Set the church code
            if (!string.IsNullOrEmpty(churchCode)) {
                this._infellowshipChurchCode = churchCode;
            }

            // Open the web page
            this._selenium.Open(this.GetInFellowshipURL());

            // Set the username
            if (!string.IsNullOrEmpty(username)) {
                this._selenium.Type("username", username);
            }
            else {
                this._selenium.Type("username", _infellowshipEmail);
            }

            // Set the password
            if (!string.IsNullOrEmpty(password)) {
                this._selenium.Type("password", password);
            }
            else {
                this._selenium.Type("password", _infellowshipPassword);
            }

            // Login, wait for the page to load
            this._selenium.ClickAndWaitForPageToLoad("btn_login");
        }

        public string GetInFellowshipURL() {
            string returnValue = string.Empty;

            switch (this._f1Environment) {
              /*  case F1Environments.DEV1:
                case F1Environments.DEV2:
                case F1Environments.DEV3:
                    returnValue = string.Format("http://{0}.infellowship.{1}.dev.corp.local", this._infellowshipChurchCode, this._f1Environment);
                    break;
                case F1Environments.INTEGRATION:
                    returnValue = string.Format("http://{0}.infellowship.integration.corp.local", this._infellowshipChurchCode);
                    break;
                case F1Environments.DEV:*/

                case F1Environments.QA:
                    returnValue = string.Format("http://{0}.infellowship{1}.dev.corp.local", this._infellowshipChurchCode, this._f1Environment);
                    break;
                case F1Environments.STAGING:
                    //returnValue = string.Format("https://{0}.staging.infellowship.com", this._infellowshipChurchCode);
                    //break;
                case F1Environments.LV_QA:
                case F1Environments.LV_UAT:
                case F1Environments.INT:
                case F1Environments.INT2:
                case F1Environments.INT3:
                case F1Environments.LV_PROD:
                    //https://dc.qa.f1infellowship.dev.activenetwork.com/UserLogin/Index?ReturnUrl=%2f
                    //returnValue = string.Format("https://{0}.{1}.f1infellowship.dev.activenetwork.com", this._infellowshipChurchCode, this._f1Environment);
                    returnValue = string.Format("https://{0}.{1}.infellowship.com/UserLogin", this._infellowshipChurchCode, PortalBase.GetLVEnvironment(this._f1Environment));
                    break;
                default:
                    throw new Exception("Not a valid option!");
            }
            return returnValue;
        }

        public void InFellowship_OpenURLExpectingNoAccess(string url) {
            // Attempt to navigate to a url, expecting to be redirected to the 'NoAccess' page
            this._selenium.Open(url);

            // Verify user is taken to the 'NoAccess' page
            Assert.IsTrue(this._selenium.GetLocation().Contains("/GroupApp/NoAccess"));
            Assert.IsTrue(this._selenium.IsTextPresent("Access Denied"));
            Assert.IsTrue(this._selenium.IsTextPresent("Note: You do not have the proper credentials to view the requested page."));

            // Return to the home page
            this._selenium.ClickAndWaitForPageToLoad("link=BACK TO HOME");
        }
        #endregion InFellowship

        #region WebLink
        public void LoginWebLink() {
            // Open the weblink login page
            this._selenium.Open(GetWeblinkLoginURL());

            // Enter credentials
            this._selenium.Type("txtEmail", this._email);
            this._selenium.Type("txtPassword", this._webLinkPassword);

            // Attempt to login
            this._selenium.ClickAndWaitForPageToLoad("btnSignIn");
        }

        public void LoginWebLink(string encryptedChurchCode, string login, string password) {
            // Open the weblink login page
            this._selenium.Open(GetWeblinkLoginURL(encryptedChurchCode));

            // Enter credentials
            this._selenium.Type("txtEmail", login);
            this._selenium.Type("txtPassword", password);

            // Attempt to login
            this._selenium.ClickAndWaitForPageToLoad("btnSignIn");
        }

        public void LogoutWebLink() {
            // Open the weblink logout page
            this._selenium.Open(GetWeblinkLogoutURL());
        }

        public void EditProfile() {
            this.LoginWebLink();
            this._selenium.Open(GetWeblinkEditProfileURL());
        }

        public void ResetPassword() {
            this.LoginWebLink();
            this._selenium.Open(GetWeblinkResetPasswordURL());
        }

        public void LoginHelp() {
            this._selenium.Open(GetWeblinkLoginHelpURL());
        }

        public void ViewOnlineGiving() {
            this.LoginWebLink();
            this._selenium.Open(GetWeblinkOnlineGivingURL());
        }

        /// <summary>
        /// Views the online giving page for a given church and user.
        /// </summary>
        /// <param name="encryptedChurchCode">The encrypted church code.</param>
        /// <param name="login">The user login.</param>
        /// <param name="password">The user password.</param>
        public void ViewOnlineGiving(string encryptedChurchCode, string login, string password) {
            this.LoginWebLink(encryptedChurchCode, login, password);
            this._selenium.Open(GetWeblinkOnlineGivingURL(encryptedChurchCode));
        }

        public void ViewVolunteerApplication(string iCode) {
            string volunteerApplicationURL = string.Format("{0}&iCode=", GetWeblinkVolunteerApplicationURL(), iCode);

            this.LoginWebLink();
            this._selenium.Open(volunteerApplicationURL);
        }

        public void ViewContactForm() {
            this.LoginWebLink();
            this._selenium.Open(GetWeblinkContactFormURL());
        }

        private string GetWebLinkBaseURL() {
            string returnValue = string.Empty;

            switch (_f1Environment) {
                case F1Environments.LOCAL:
                    throw new Exception("Not a valid option!");
                /*case F1Environments.DEV1:
                case F1Environments.DEV2:
                case F1Environments.DEV3:
                    returnValue = string.Format("http://integration.{0}.dev.corp.local/integration", _f1Environment);
                    break;
                case F1Environments.INTEGRATION:
                    returnValue = "http://integration.integration.corp.local/integration";
                    break;
                case F1Environments.DEV: */

                case F1Environments.QA:
                    returnValue = string.Format("http://integration{0}.dev.corp.local/integration", _f1Environment);
                    break;
                case F1Environments.STAGING:
                    returnValue = "https://staging-integration.fellowshipone.com/integration";
                    break;
                default:
                    throw new Exception("Not a valid option!");
            }
            return returnValue;
        }

        public string GetWeblinkLoginURL() {
            return string.Format("{0}/login.aspx?cCode={1}", GetWebLinkBaseURL(), _webLinkChurchCode);
        }

        public string GetWeblinkLoginURL(string encryptedChurchCode) {
            return string.Format("{0}/login.aspx?cCode={1}", GetWebLinkBaseURL(), encryptedChurchCode);
        }

        protected string GetWeblinkLogoutURL() {
            return string.Format("{0}/logout.aspx?cCode={1}", GetWebLinkBaseURL(), _webLinkChurchCode);
        }

        public string GetWeblinkCreateAccountURL() {
            return string.Format("{0}/newuser.aspx?cCode={1}", GetWebLinkBaseURL(), _webLinkChurchCode);
        }

        protected string GetWeblinkResetPasswordURL() {
            return string.Format("{0}/resetpassword.aspx?cCode={1}", GetWebLinkBaseURL(), _webLinkChurchCode);
        }

        protected string GetWeblinkEditProfileURL() {
            return string.Format("{0}/profileeditor.aspx?cCode={1}", GetWebLinkBaseURL(), _webLinkChurchCode);
        }

        protected string GetWeblinkLoginHelpURL() {
            return string.Format("{0}/loginhelp.aspx?cCode={1}", GetWebLinkBaseURL(), _webLinkChurchCode);
        }

        public string GetWeblinkOnlineGivingURL() {
            return string.Format("{0}/contribution/onlinecontribution.aspx?cCode={1}", GetWebLinkBaseURL(), _webLinkChurchCode);
        }

        public string GetWeblinkOnlineGivingURL(string encryptedChurchCode) {
            return string.Format("{0}/contribution/onlinecontribution.aspx?cCode={1}", GetWebLinkBaseURL(), encryptedChurchCode);
        }

        protected string GetWeblinkVolunteerApplicationURL() {
            return string.Format("{0}/volunteer/volunteerapplication.aspx?cCode={1}", GetWebLinkBaseURL(), _webLinkChurchCode);
        }

        protected string GetWeblinkContactFormURL() {
            return string.Format("{0}/contact/onlinecontact.aspx?cCode={1}", GetWebLinkBaseURL(), _webLinkChurchCode);
        }

        protected string GetWeblinkEventRegistrationURL() {
            return string.Format("{0}/integration/integration/FormBuilder/FormBuilder.aspx?cCode={1}", GetWebLinkBaseURL(), _webLinkChurchCode);
        }

        public string GetWebLinkEventRegistrationAddIndividualURL() {
            return string.Format("{0}/registration/personalinfo.aspx?cCode={1}&anm=1", GetWebLinkBaseURL(), _webLinkChurchCode);
        }

        public string GetWebLinkAccountCreationInfoURL() {
            return string.Format("{0}/conversion/CreateInfo.aspx?cCode={1}", GetWebLinkBaseURL(), _webLinkChurchCode);
        }
        #endregion WebLink

        #region AdhocReporting
        /// <summary>
        /// Logs in to Reporting Analytics.
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="churchCode">Church code.</param>
        /// </summary>
        public void LoginAdhocReporting([Optional] string username, [Optional] string password, [Optional] string churchCode) {
            // Set the specific church code
            if (!string.IsNullOrEmpty(churchCode)) {
                this._adhocReportingChurchCode = churchCode;
            }

            // Open the AdhocReporting Login page
            this._selenium.Open(this.GetAdhocReportingURL());

            // Set the specific username
            if (!string.IsNullOrEmpty(username)) {
                this._selenium.Type("username", username);
            }
            else {
                this._selenium.Type("username", _adhocReportingUserName);
            }

            // Set the specific password
            if (!string.IsNullOrEmpty(password)) {
                this._selenium.Type("password", password);
            }
            else {
                this._selenium.Type("password", _adhocReportingPassword);
            }

            //Set the specific church code
            if (!string.IsNullOrEmpty(churchCode)) {
                this._selenium.Type("churchcode", churchCode);
            }
            else {
                this._selenium.Type("churchcode", _adhocReportingChurchCode);
            }

            // Login
            this._selenium.ClickAndWaitForPageToLoad("btn_login");
        }

        /// <summary>
        /// Fetches the url for Adhoc Reporting based on the current environment.
        /// </summary>
        /// <returns>The url for Adhoc Reporting.</returns>
        public string GetAdhocReportingURL() {
            string returnURL = string.Empty;
            switch (this._f1Environment) {
               /* case F1Environments.DEV:
                    returnURL = "http://adhoc.dev.corp.local/";
                    break; */

                case F1Environments.QA:
                    returnURL = "http://adhocqa.dev.corp.local/";
                    break;
                case F1Environments.STAGING:
                    returnURL = "https://analytics.staging.fellowshipone.com/UserLogin/Index";
                    break;
                default:
                    throw new SeleniumException("Please select a valid environment!!!");
            }
            return returnURL;
        }

        /// <summary>
        /// Logs out of the Adhoc Reporting application.
        /// </summary>
        public void LogoutAdhocReporting() {
            this._selenium.ClickAndWaitForPageToLoad("link=sign out");
        }
        #endregion AdhocReporting

        #region Helpers
        /// <summary>
        /// Verifies the screen message errors displayed to the user.
        /// </summary>
        public void VerifyErrorMessages() {
            if (this._errorText.Count > 0) {
                if (this._errorText.Count == 1) {
                    this._selenium.VerifyTextPresent(TextConstants.ErrorHeadingSingular);
                }
                else {
                    this._selenium.VerifyTextPresent(TextConstants.ErrorHeadingPlural);
                }

                foreach (string error in this._errorText) {
                    this._selenium.VerifyTextPresent(error);
                }

                // Verify the number of errors present
                Assert.AreEqual(this._errorText.Count, this._selenium.GetXpathCount("//dl[@id='error_message']/dd"));
            }
        }
        #endregion Helpers
    }

    [Obsolete("Use test.GeneralMethods")]
    public class BaseTestMethods : Setup {
        /// <summary>
        /// Fetches the row number of the item provided as it exists in an HTML table.
        /// </summary>
        /// <param name="test">The current test object.</param>
        /// <param name="tableId">The table id that the item exists in.</param>
        /// <param name="uniqueIdentifier">The unique identifier used to identify the row.</param>
        /// <param name="columnName">The column name of the item in question.</param>
        /// <param name="function">An optional parameter used to specify an xpath function to be used in the query.</param>
        /// <returns>A decimal representing the row of the table the item exists in.</returns>
        public static decimal GetTableRowNumberNew(TestBaseOld test, string tableId, string uniqueIdentifier, string columnName, [Optional] string function) {
            // Verify the table is present
            test.Selenium.VerifyElementPresent(tableId);
            
            // Find the column number of the column name
            decimal columnNumber = (test.Selenium.IsElementPresent(string.Format("{0}/*/tr[*]/td[normalize-space(string(.))='{1}']", tableId, columnName)) ? test.Selenium.GetElementIndex(string.Format("{0}/*/tr[*]/td[normalize-space(string(.))='{1}']", tableId, columnName)) : test.Selenium.GetElementIndex(string.Format("{0}/*/tr[*]/th[normalize-space(string(.))='{1}']", tableId, columnName))) + 1;
            
            // Special case for InFellowship group roster
            if (tableId == TableIds.InFellowship_Groups_Roster) {
                columnNumber++;
            }

            // Search for all items active or inactive, if the option exists
            if (test.Selenium.IsElementPresent("ctl00_ctl00_MainContent_content_ddlActive_dropDownList")) {
                test.Selenium.Select("ctl00_ctl00_MainContent_content_ddlActive_dropDownList", "All");
                test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.Search);
            }

            // Configure the common xpath
            string surroundingChar = uniqueIdentifier.Contains("'") ? '"'.ToString() : "'";
            string commonXPath = function != null ? string.Format("{0}/tbody/tr[*]/td[position()={1} and ({2}(normalize-space(string(.)), {3}{4}{3}) or {2}(normalize-space(string(./*)), {3}{4}{3}))]", tableId, columnNumber, function, surroundingChar, uniqueIdentifier) : string.Format("{0}/tbody/tr[*]/td[position()={1} and (normalize-space(string(.))={2}{3}{2} or normalize-space(string(./*))={2}{3}{2})]", tableId, columnNumber, surroundingChar, uniqueIdentifier);

            // If there are multiple pages...
            if (test.Selenium.IsElementPresent("//div[@class='grid_controls']/ul/li[1]/a[text()='1']")) {
                for (int pageIndex = 1; pageIndex < test.Selenium.GetXpathCount("//div[@class='grid_controls']/ul/li"); pageIndex++) {
                    test.Selenium.ClickAndWaitForPageToLoad(string.Format("//div[@class='grid_controls']/ul/li[*]/a[text()='{0}']", pageIndex));

                    if (test.Selenium.IsElementPresent(commonXPath)) {
                        return test.Selenium.GetElementIndex(string.Format("{0}/ancestor::tr", commonXPath));
                    }
                }
            }
            else {
                if (test.Selenium.IsElementPresent(commonXPath)) {
                    return test.Selenium.GetElementIndex(string.Format("{0}/ancestor::tr", commonXPath));
                }
            }

            // If we get this far, we did not find the element
            throw new SeleniumException("Did not find the unique identifier!!");
        }

        public static int GetTableRowCount(ISelenium selenium, string tableId) {
            if (tableId.Contains("//")) {
                return (int)selenium.GetXpathCount(tableId + "/tbody/tr");
            }
            else {
                return Convert.ToInt16(selenium.GetEval(string.Format("this.browserbot.getCurrentWindow().document.getElementById('{0}').rows.length", tableId)));
            }
        }

        public static int GetTableColumnCount(ISelenium selenium, string tableId, int tableRow) {
            if (tableId.Contains("//")) {
                if (selenium.IsElementPresent(tableId + "/tbody/tr[" + (tableRow + 1) + "]/td")) {
                    return (int)selenium.GetXpathCount(tableId + "/tbody/tr[" + (tableRow + 1) + "]/td");
                }
                else if (selenium.IsElementPresent(tableId + "/tbody/tr[" + (tableRow + 1) + "]/th")) {
                    return (int)selenium.GetXpathCount(tableId + "/tbody/tr[" + (tableRow + 1) + "]/th");
                }
                else if (selenium.IsElementPresent(tableId + "/thead/tr[" + (tableRow + 1) + "]/th")) {
                    return (int)selenium.GetXpathCount(tableId + "/thead/tr[" + (tableRow + 1) + "]/th");
                }
                else if (selenium.IsElementPresent("//form[@id='Form1']")) {
                    return (int)selenium.GetXpathCount("//form[@id='Form1']/table/thead/tr/th");
                }
                else {
                    throw new Exception("Did not find the columns or column headers!!");
                }
            }
            else {
                return Convert.ToInt16(selenium.GetEval(string.Format("this.browserbot.getCurrentWindow().document.getElementById('{0}').rows[{1}].cells.length", tableId, tableRow)));
            }
        }

        public static bool ItemExistsInTableNew(TestBaseOld test, string tableId, string uniqueIdentifier, string columnName, [Optional] string function) {
            // Verify table is present
            test.Selenium.VerifyElementPresent(tableId);

            // Find the column number of the column name
            //decimal columnNumber = test.Selenium.IsElementPresent(string.Format("{0}/tbody/tr[*]/td[normalize-space(string(.))='{1}']", tableId, columnName)) ? test.Selenium.GetElementIndex(string.Format("{0}/tbody/tr[*]/td[normalize-space(string(.))='{1}']", tableId, columnName)) + 1 : test.Selenium.GetElementIndex(string.Format("{0}/tbody/tr[*]/th[normalize-space(string(.))='{1}']", tableId, columnName)) + 1;
            decimal columnNumber = test.Selenium.GetElementIndex(string.Format("{0}/*[self::tbody or self::thead]/tr[*]/*[self::td or self::th][normalize-space(string(.))='{1}']", tableId, columnName)) + 1;

            string surroundingChar = uniqueIdentifier.Contains("'") ? '"'.ToString() : "'";
            //string commonXPath = string.Format("{0}/tbody/tr[*]/td[position()={1} and (normalize-space(string(.))={2}{3}{2} or normalize-space(string(./*))={2}{3}{2})]", tableId, columnNumber, surroundingChar, uniqueIdentifier);
            string commonXPath = function != null ? string.Format("{0}/tbody/tr[*]/td[position()={1} and ({2}(normalize-space(string(.)), {3}{4}{3}) or {2}(normalize-space(string(./*)), {3}{4}{3}))]", tableId, columnNumber, function, surroundingChar, uniqueIdentifier) : string.Format("{0}/tbody/tr[*]/td[position()={1} and (normalize-space(string(.))={2}{3}{2} or normalize-space(string(./*))={2}{3}{2})]", tableId, columnNumber, surroundingChar, uniqueIdentifier);

            // Search for all items active or inactive
            if (test.Selenium.IsElementPresent("ctl00_ctl00_MainContent_content_ddlActive_dropDownList")) {
                test.Selenium.Select("ctl00_ctl00_MainContent_content_ddlActive_dropDownList", "All");
                test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.Search);
            }

            // If there are multiple pages...
            if (test.Selenium.IsElementPresent("//div[@class='grid_controls']/ul[@class='grid_pagination']") && test.Selenium.IsVisible("//div[@class='grid_controls']/ul[@class='grid_pagination']") && (test.Selenium.GetXpathCount("//div[@class='grid_controls']/ul[@class='grid_pagination']/li") > 1)) {
                for (int pageIndex = 1; pageIndex <= test.Selenium.GetXpathCount("//div[@class='grid_controls']/ul[@class='grid_pagination']/li"); pageIndex++) {
                    test.Selenium.ClickAndWaitForPageToLoad(string.Format("//div[@class='grid_controls']/ul[@class='grid_pagination']/li[{0}]/a", pageIndex));

                    if (test.Selenium.IsElementPresent(commonXPath)) {
                        return test.Selenium.IsElementPresent(commonXPath);
                    }
                }
            }
            else {
                return test.Selenium.IsElementPresent(commonXPath);
            }

            // If we get this far, we did not find the element
            return false;
        }

        /// <summary>
        /// Selects a gear option from a gear within an HTML table.
        /// </summary>
        /// <param name="test">The current test object</param>
        /// <param name="itemRow">The row of the gear to be interacted with (raw value from GetTableRowNumber method)</param>
        /// <param name="gearOption">The gear option to be selected</param>
        public static void SelectOptionFromGear(TestBaseOld test, int itemRow, string gearOption, bool waitForPageToLoad = true) {
            test.Selenium.Click(string.Format("//table[*]/tbody/tr[{0}]/td[*]/a[@class='gear_trigger']", itemRow + 1));
            test.Selenium.Click(string.Format("//table[*]/tbody/tr[{0}]/td[*]/ul/li[*]/a/span[text()='{1}']|//table[*]/tbody/tr[{0}]/td[*]/ul/li[*]/a[text()='{1}']", itemRow + 1, gearOption));

            if (waitForPageToLoad) {
                test.Selenium.WaitForPageToLoad("60000");
            }
        }

        /// <summary>
        /// Verifies correct functionality of a given date control
        /// </summary>
        /// <param name="dateControlID">The ID of the date control in question</param>
        public static void VerifyDateControl(TestBaseOld test, string dateControlID) {
            // Clear data
            test.Selenium.Type(dateControlID, "");

            // t
            test.Selenium.KeyDown(dateControlID, "\\84");
            test.Selenium.KeyUp(dateControlID, "\\84");
            Assert.AreEqual(string.Format("{0:MM/dd/yyyy}", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))), test.Selenium.GetValue(dateControlID));

            // mmddyy
            DoVerifyDateControl(test, dateControlID, "120574", "12/05/1974");

            DoVerifyDateControl(test, dateControlID, "021510", "02/15/2010");

            // mm.dd.yy
            DoVerifyDateControl(test, dateControlID, "12.05.74", "12/05/1974");

            DoVerifyDateControl(test, dateControlID, "02.15.10", "02/15/2010");

            // mm-dd-yy
            DoVerifyDateControl(test, dateControlID, "12-05-74", "12/05/1974");

            DoVerifyDateControl(test, dateControlID, "02-15-10", "02/15/2010");

            // mmddyyyy
            DoVerifyDateControl(test, dateControlID, "12051974", "12/05/1974");

            DoVerifyDateControl(test, dateControlID, "02152010", "02/15/2010");

            // mm/dd/yy
            DoVerifyDateControl(test, dateControlID, "12/05/74", "12/05/1974");

            DoVerifyDateControl(test, dateControlID, "02/15/10", "02/15/2010");

            // 120520
            DoVerifyDateControl(test, dateControlID, "120520", "12/05/1920");

            // Verify autocomplete is off
            Assert.IsTrue(test.Selenium.IsElementPresent(string.Format("//input[@id='{0}' and @autocomplete='off']", dateControlID)));
        }

        private static void DoVerifyDateControl(TestBaseOld test, string dateControlID, string rawDate, string expectedFormattedDate) {
            string focus = "//input[@id='submitQuery' or @id='ctl00_content_btnSave' or @id='btn_submit']"; //submitQuery

            test.Selenium.Focus(dateControlID);
            test.Selenium.Type(dateControlID, "");
            test.Selenium.TypeKeys(dateControlID, rawDate);
            test.Selenium.Focus(focus);
            test.Selenium.WaitForCondition(string.Format("selenium.browserbot.getCurrentWindow().document.getElementById('{0}').value.match('{1}')", dateControlID, "[0-9]{1,2}/[0-9]{1,2}/[0-9]{4}"), "10000");
            Assert.AreEqual(expectedFormattedDate, test.Selenium.GetValue(dateControlID));
        }

        /// <summary>
        /// Searches for and selects an individual from the find person popup.
        /// </summary>
        /// <param name="test">The current test object</param>
        /// <param name="individual">The individual to be searched and selected</param>
        public static void SelectIndividualFromFindPersonPopup(TestBaseOld test, string individual) {
            DoSelectIndividualFromFindPersonPopup(test, null, individual);
        }

        /// <summary>
        /// Searches for and selects an individual from the find person popup. Allows user to specify non-common
        /// element for the find person link.
        /// </summary>
        /// <param name="test">The current test object</param>
        /// <param name="elementID">The ID of the find person link</param>
        /// <param name="individual">The individual to be searched and selected</param>
        public static void SelectIndividualFromFindPersonPopup(TestBaseOld test, string elementID, string individual) {
            DoSelectIndividualFromFindPersonPopup(test, elementID, individual);
        }

        /// <summary>
        /// Performs the seach and selection of an individual from the find person popup.
        /// </summary>
        /// <param name="test">The current test object</param>
        /// <param name="elementID">The ID of the find person link</param>
        /// <param name="individual">The individual to be searched and selected</param>
        private static void DoSelectIndividualFromFindPersonPopup(TestBaseOld test, string elementID, string individual) {
            string pageTitle = test.Selenium.GetTitle();
            string pageLocation = test.Selenium.GetLocation();

            if (!string.IsNullOrEmpty(elementID)) {
                //test.Selenium.ClickAndSelectPopUp(elementID, "psuedoModal", "10000");
                test.Selenium.Click(elementID);
                test.Selenium.WaitForPopUp("psuedoModal", "30000");
                test.Selenium.SelectPopUp("psuedoModal");
            }
            else {
                //test.Selenium.ClickAndSelectPopUp("//a[@id='ctl00_ctl00_MainContent_content_ctlFindPerson_lnkFindPerson' or @id='ctl00_ctl00_MainContent_content_Addeditstaff1_ctlFindPerson_lnkFindPerson']", "psuedoModal", "10000");
                test.Selenium.Click("//a[@id='ctl00_ctl00_MainContent_content_ctlFindPerson_lnkFindPerson' or @id='ctl00_ctl00_MainContent_content_Addeditstaff1_ctlFindPerson_lnkFindPerson']");
                test.Selenium.WaitForPopUp("psuedoModal", "30000");
                test.Selenium.SelectPopUp("psuedoModal");
            }

            test.Selenium.Type("ctl00_content_txtName_textBox", individual);
            test.Selenium.KeyPressAndWaitForCondition("ctl00_content_btnSearchPeople", "\\13", JavaScriptMethods.IsElementPresent("ctl00_content_dgSearchResults"), "15000");
            
            
            test.Selenium.KeyPress("ctl00_content_dgSearchResults_ctl02_lnkSelect", "\\13");
            //test.Selenium.Close();

            //test.Selenium.DeselectPopUp();
            //test.Selenium.SelectWindow(null);
            test.Selenium.SelectWindow(pageTitle);

            try {
                test.Selenium.WaitForPageToLoad("60000");
            }
            catch (SeleniumException) {
                test.Selenium.OpenWindow(pageLocation, "selenium_main_app_window");
                foreach (var window in test.Selenium.GetAllWindowNames()) {
                    TestLog.WriteLine(window);
                }
                throw;
            }
            
        }

        public static void OpenURLExpecting403(TestBaseOld test, string url) {
            // Attempt to open the url
            string exceptionText = string.Empty;

            //try {
                test.Selenium.Open(url);
            //}
            //catch (SeleniumException selEx) {
                //exceptionText = selEx.Message;
            //}

           // if (test.F1Environment == F1Environments.LOCAL || test.F1Environment == F1Environments.DEV || test.F1Environment == F1Environments.QA || test.F1Environment == F1Environments.INTEGRATION) {
               
                if (test.F1Environment == F1Environments.LOCAL  || test.F1Environment == F1Environments.QA) 
                {
                //Assert.IsTrue(exceptionText.Contains("Response_Code = 403 Error_Message = Forbidden"));
                test.Selenium.VerifyTextPresent("Server Error in '/' Application.");
                test.Selenium.VerifyTextPresent("User is not authorized to view the page:");
                test.Selenium.VerifyTextPresent(url);
            }
            else {
                Assert.AreEqual("403 Forbidden", test.Selenium.GetText("//span[@class='float_left']"));
                test.Selenium.VerifyTextPresent("You are not authorized to view the requested page");
            }
        }

        public static void OpenURLExpecting500(TestBaseOld test, string url) {
            // Attempt to open the url
            test.Selenium.Open(url);

            // Verify the response
            if (test.F1Environment == F1Environments.STAGING) {
                Assert.AreEqual("500 Error", test.Selenium.GetText("//span[@class='float_left']"));
                test.Selenium.VerifyTextPresent("Well, that wasn’t supposed to happen…");
            }
            else {
                //Assert.IsTrue(exceptionText.Contains("Response_Code = 500 Error_Message = Internal Server Error"));
                Assert.IsTrue(test.Selenium.IsTextPresent("Server Error in '/' Application."));
                Assert.IsTrue(test.Selenium.IsTextPresent("Object reference not set to an instance of an object."));
            }
        }

        public static void Popups_Confirmation(ISelenium selenium, string yesNo) {
            selenium.WaitForPopUp("psuedoModal", "30000");
            selenium.SelectWindow("name=psuedoModal");

            switch (yesNo) {
                case "Yes":
                    selenium.KeyPress("btnYes", "\\9");
                    selenium.KeyPress("btnYes", "\\13");
                    break;
                case "No":
                    selenium.KeyPress("btnNo", "\\9");
                    selenium.KeyPress("btnNo", "\\9");
                    selenium.KeyPress("btnNo", "\\13");
                    break;
                default:
                    throw new SeleniumException("Not a valid option!!");
            }
            //selenium.DeselectPopUp();
            selenium.SelectWindow("");

            for (int i = 0; i < 50; i++) {
                try {
                    selenium.SelectWindow("null");
                    selenium.Focus("//div[@id='nav']");

                    if (selenium.GetEval("selenium.browserbot.getCurrentWindow().document.activeElement.id.match('nav')") == "true") {
                        break;
                    }
                }
                catch (SeleniumException) {
                }
            }
        }

        public static void ChangeMinistry(TestBaseOld test, string ministry) {
            // Navigate to ministry->activities
            test.Selenium.Navigate(Navigation.Ministry.ActivityRoom_Setup.Activities);

            test.Selenium.Click("link=Change");

            if (test.Selenium.GetSelectedLabel("ctl00_ctl00_MainContent_ddlMinistryTemplateSelection") != ministry) {
                test.Selenium.SelectAndWaitForPageToLoad("ctl00_ctl00_MainContent_ddlMinistryTemplateSelection", ministry);
            }
        }

        public static void VerifyTableData(TestBaseOld test, string tableId, decimal itemRow, Dictionary<int, string> tableValues) {
            foreach (KeyValuePair<int, string> tableValue in tableValues) {
                Assert.AreEqual(tableValue.Value, test.Selenium.GetTable(string.Format("{0}.{1}.{2}", tableId, itemRow, tableValue.Key)));
            }
        }
    }

    /// <summary>
    /// This class is used for executing queries against the database.
    /// </summary>
    [Obsolete("Use base.SQL")]
    public class SQLMethods : Setup {
        #region Admin
        /// <summary>
        /// This method creates a building.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="buildingName">The building name.</param>
        [Obsolete]
        public void Admin_BuildingsCreate(int churchId, string buildingName) {
            this.ExecuteDBQuery(string.Format("INSERT INTO ChmChurch.dbo.BUILDING (CHURCH_ID, BUILDING_NAME) VALUES({0}, '{1}')", churchId, buildingName));
        }

        /// <summary>
        /// This method deletes a building.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="buildingName">The name of the building to be deleted.</param>
        [Obsolete]
        public void Admin_BuildingsDelete(int churchId, string buildingName) {
            this.ExecuteDBQuery(string.Format("DELETE FROM ChmChurch.dbo.BUILDING WHERE CHURCH_ID = {0} AND BUILDING_NAME = '{1}'", churchId, buildingName));
        }

        /// <summary>
        /// This method creates a campus.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="campusName">The name of the campus.</param>
        [Obsolete]
        public void Admin_CampusCreate(int churchId, string campusName) {
            this.ExecuteDBQuery(string.Format("INSERT INTO ChmPeople.dbo.ChurchCampus (ChurchID, CampusName) VALUES ({0}, '{1}')", churchId, campusName));
        }

        /// <summary>
        /// This method deletes a campus.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="campusName">The name of the campus.</param>
        [Obsolete]
        public void Admin_CampusDelete(int churchId, string campusName) {
            this.ExecuteDBQuery(string.Format("DELETE FROM ChmPeople.dbo.ChurchCampus WHERE ChurchID = {0} AND CampusName = '{1}'", churchId, campusName));
        }

        /// <summary>
        /// This method creates a department.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="departmentName">The name of the department.</param>
        /// <param name="departmentCode">The code of the department.</param>
        [Obsolete]
        public void Admin_DepartmentsCreate(int churchId, string departmentName, string departmentCode) {
            if (departmentCode == string.Empty) {
                base.ExecuteDBQuery(string.Format("INSERT INTO ChmChurch.dbo.DEPT (CHURCH_ID, DEPT_NAME) VALUES ({0}, '{1}')", churchId, departmentName));
            }
            else {
                base.ExecuteDBQuery(string.Format("INSERT INTO ChmChurch.dbo.DEPT (CHURCH_ID, DEPT_NAME, DEPT_CODE) VALUES ({0}, '{1}', '{2}')", churchId, departmentName, departmentCode));
            }
        }

        /// <summary>
        /// This method deletes a department.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="departmentName">The name of the department.</param>
        [Obsolete]
        public void Admin_DepartmentsDelete(int churchId, string departmentName) {
            base.ExecuteDBQuery(string.Format("DELETE FROM ChmChurch.dbo.DEPT WHERE CHURCH_ID = {0} AND DEPT_NAME = '{1}'", churchId, departmentName));
        }

        /// <summary>
        /// This method creates a room.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="roomCode">The room code.</param>
        /// <param name="buildingName">The name of the building the room should be tied to.</param>
        /// <param name="roomName">The name of the room.</param>
        /// <param name="roomDescription">The description of the room.</param>
        [Obsolete]
        public void Admin_RoomsCreate(int churchId, string roomCode, string buildingName, string roomName, string roomDescription) {
            StringBuilder query = new StringBuilder("DECLARE @buildingID INT ");
            query.AppendFormat("SET @buildingID = {0} ", Convert.ToInt32(base.ExecuteDBQuery(string.Format("SELECT TOP 1 BUILDING_ID FROM ChmChurch.dbo.BUILDING WHERE CHURCH_ID = {0} AND BUILDING_NAME = '{1}'", churchId, buildingName)).Rows[0]["BUILDING_ID"]));
            query.AppendFormat("INSERT INTO ChmChurch.dbo.ROOM (CHURCH_ID, BUILDING_ID, ROOM_CODE, ROOM_DESC, ROOM_NAME) VALUES({0}, @buildingID, '{1}', '{2}', '{3}')", churchId, roomCode, roomDescription, roomName);
            base.ExecuteDBQuery(query.ToString());
        }

        /// <summary>
        /// This method deletes a room.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="roomName">The room name.</param>
        [Obsolete]
        public void Admin_RoomsDelete(int churchId, string roomName) {
            base.ExecuteDBQuery(string.Format("DELETE FROM ChmChurch.dbo.ROOM WHERE CHURCH_ID = {0} AND ROOM_NAME = '{1}'", churchId, roomName));
        }

        /// <summary>
        /// This method creates a contact form name.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="formName">The name of the contact form.</param>
        /// <param name="isActive">Flag designating active status for the form.</param>
        [Obsolete]
        public void Admin_FormNamesCreate(int churchId, string formName, bool isActive) {
            base.ExecuteDBQuery(string.Format("INSERT INTO ChmChurch.dbo.CONTACT_TYPE (CHURCH_ID, CONTACT_TYPE_NAME, IS_ACTIVE) VALUES({0}, '{1}', {2})", churchId, formName, Convert.ToInt16(isActive)));
        }

        /// <summary>
        /// This method deletes a contact form name.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="formName">The name of the contact form.</param>
        [Obsolete]
        public void Admin_FormNamesDelete(int churchId, string formName) {
            base.ExecuteDBQuery(string.Format("DELETE FROM ChmChurch.dbo.CONTACT_TYPE WHERE CHURCH_ID = {0} AND CONTACT_TYPE_NAME = '{1}'", churchId, formName));
        }

        /// <summary>
        /// This method creates a school.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="schoolName">The name of the school.</param>
        /// <param name="schoolTypeId">The type for the school.</param>
        [Obsolete]
        public void Admin_Schools_Create(int churchId, string schoolName, int schoolTypeId) {
            base.ExecuteDBQuery(string.Format("INSERT INTO ChmPeople.dbo.SCHOOL (CHURCH_ID, SCHOOL_NAME, SCHOOL_TYPE_ID) VALUES({0}, '{1}', {2})", churchId, schoolName, schoolTypeId));
        }

        /// <summary>
        /// This method deletes a school.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="schoolName">The name of the school.</param>
        [Obsolete]
        public void Admin_Schools_Delete(int churchId, string schoolName) {
            base.ExecuteDBQuery(string.Format("DELETE FROM ChmPeople.dbo.SCHOOL WHERE CHURCH_ID = {0} AND SCHOOL_NAME = '{1}'", churchId, schoolName));
        }

        /// <summary>
        /// This method deletes all security roles for a user based the specified security group id.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="login">The user login.</param>
        /// <param name="securityGroupId">The security type id.</param>
        public void Admin_Users_DeleteSecurityRolesBySecurityGroup(int churchId, string login, int securityGroupId) {
            StringBuilder query = new StringBuilder("DECLARE @roleID INT ");
            query.AppendFormat("SET @roleID = {0} ", this.FetchUserSecurityRole(churchId, login));

            query.Append("DELETE r_s_t FROM ChmChurch.dbo.ROLE_SECURITY_TYPE r_s_t WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmChurch.dbo.SECURITY_TYPE s_t WITH (NOLOCK) ");
            query.Append("ON r_s_t.SECURITY_TYPE_ID = s_t.SECURITY_TYPE_ID ");
            query.AppendFormat("WHERE ROLE_ID = @roleID AND SECURITY_GROUP_ID = {0}", securityGroupId);
            this.ExecuteDBQuery(query.ToString());
        }

        /// <summary>
        /// This method creates a user security type for the given login.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="login">The user login.</param>
        /// <param name="securityGroupId">The security group id.</param>
        /// <param name="securityTypeName">The security type name.</param>
        public void Admin_Users_CreateSecurityRoles(int churchId, string login, int securityGroupId, string[] securityTypeNames) {
            StringBuilder query = new StringBuilder("DECLARE @churchID INT, @roleID INT ");
            query.AppendFormat("SET @churchID = {0} ", churchId);
            query.AppendFormat("SET @roleID = {0} ", this.FetchUserSecurityRole(churchId, login));

            foreach (string securityTypeName in securityTypeNames) {
                StringBuilder insertQuery = new StringBuilder();
                insertQuery.Append(query);
                insertQuery.Append("INSERT INTO ChmChurch.dbo.ROLE_SECURITY_TYPE (ROLE_ID, SECURITY_TYPE_ID) ");
                insertQuery.AppendFormat("SELECT @roleID, SECURITY_TYPE_ID FROM ChmChurch.dbo.SECURITY_TYPE WHERE SECURITY_GROUP_ID = {0} AND CHURCH_ID IN (0, @churchID) AND SECURITY_TYPE_NAME = '{1}'", securityGroupId, securityTypeName);
                this.ExecuteDBQuery(insertQuery.ToString());
            }
        }

        /// <summary>
        /// Fetches the role id for a given church id and login.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="login">The user login.</param>
        /// <returns>The role id for the user.</returns>
        private int FetchUserSecurityRole(int churchId, string login) {
            StringBuilder query = new StringBuilder("SELECT u_r.ROLE_ID FROM ChmChurch.dbo.USERS usr WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmChurch.dbo.USER_ROLE u_r WITH (NOLOCK) ");
            query.Append("ON usr.USER_ID = u_r.USER_ID ");
            query.AppendFormat("WHERE usr.CHURCH_ID = {0} AND LOGIN = '{1}' AND ENABLED = 1", churchId, login);
            return Convert.ToInt32(this.ExecuteDBQuery(query.ToString()).Rows[0]["ROLE_ID"]);

        }
        #endregion Admin

        #region Giving
        /// <summary>
        /// Polls the database checking for a range of reason codes for a payment based on the payment type.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="paymentAmount">The amount of the payment.</param>
        /// <param name="dateTimeFrom">The starting date range.</param>
        /// <param name="clientApplication">The application that generated the payment.</param>
        /// <param name="paymentType">The type of payment.</param>
        /// <returns>The payment status.</returns>
        public int Giving_WaitUntilPaymentProcessed(int churchId, double paymentAmount, DateTime dateTimeFrom, string clientApplication, int paymentType) {
            // Find the payment
            StringBuilder query = new StringBuilder();
            query.Append("SELECT TOP 1 pay.PaymentID FROM ChmContribution.dbo.Payment pay WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmContribution.dbo.PaymentInformation payinf WITH (NOLOCK) ");
            query.Append("ON pay.PaymentInformationID = payinf.PaymentInformationID ");
            query.AppendFormat("WHERE pay.ChurchID = {0} AND pay.Amount = {1} AND pay.CreatedDate > '{2}' AND pay.ClientApplication = '{3}' AND payinf.PaymentTypeID = {4} ", churchId, paymentAmount, dateTimeFrom.ToString("yyyy-MM-dd HH:mm:ss"), clientApplication, paymentType);
            query.Append("ORDER BY pay.CreatedDate DESC");

            int paymentId = (int)this.ExecuteDBQuery(query.ToString()).Rows[0]["PaymentID"];
            int paymentStatusId = int.MinValue;
            List<int> expectedPaymentStatusIds = (paymentType == 998 || paymentType == 999) ? new List<int> { 14, 15 } : new List<int> { 5, 7, 8 }; // If non-transactional, wait for status of 14 or 15, if transactional, wait for status of 5, 7, or 8

            // Wait until the payment reason code matches one of the expected types
            Retry.WithPolling(5000).WithTimeout(360000).WithFailureMessage("The payment did not process in the allotted time.")
                .DoBetween(() => paymentStatusId = Convert.ToInt16(this.ExecuteDBQuery(string.Format("SELECT PaymentStatusID FROM ChmContribution.dbo.Payment WHERE ChurchID = {0} AND PaymentID = {1}", churchId, paymentId)).Rows[0]["PaymentStatusID"]))
                .Until(() => expectedPaymentStatusIds.Contains(paymentStatusId));

            // Return the payment status id
            return paymentStatusId;
        }

        /// <summary>
        /// This method deletes a batch.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="batchName">The name of the batch.</param>
        /// <param name="batchTypeId">The type of the batch.</param>
        public void Giving_DeleteBatch(int churchId, string batchName, int batchTypeId) {
            this.ExecuteDBQuery(string.Format("DELETE FROM ChmContribution.dbo.BATCH WHERE CHURCH_ID = {0} AND BATCH_NAME = '{1}' AND BatchTypeID = {2}", churchId, batchName.Replace("'", "''"), batchTypeId));
        }

        /// <summary>
        /// This method deletes the non-transactional contribution receipts.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualId">The individual id the receipts are tied to.</param>
        /// <param name="householdId">The household id the receipts are tied to.</param>
        public void DeleteNonTransactionalContributionReceipts(int churchId, int? individualId, int? householdId) {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("DELETE FROM ChmContribution.dbo.CONTRIBUTION_RECEIPT WHERE CHURCH_ID = {0} ", churchId);

            if (individualId.HasValue) {
                query.AppendFormat("AND INDIVIDUAL_ID = {0} ", individualId);
            }

            if (householdId.HasValue) {
                query.AppendFormat("AND HOUSEHOLD_ID = {0} ", householdId);
            }

            query.Append("AND ACCOUNT_REFERENCE != 'Online'");
        }

        /// <summary>
        /// This method creates a credit card batch.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="batchName">The name of the batch.</param>
        /// <param name="batchAmount">The amount of the batch.</param>
        /// <param name="receivedDate">The received date for the batch.</param>
        public void CreateCCBatch(int churchId, string batchName, double batchAmount, DateTime receivedDate) {
            StringBuilder query = new StringBuilder("INSERT INTO ChmContribution.dbo.BATCH (CHURCH_ID, BATCH_NAME, BATCH_DATE, BATCH_AMOUNT, CREATED_DATE, BatchStatusID, ReceivedDate, BatchTypeID) ");
            query.AppendFormat("VALUES ({0}, '{1}', CURRENT_TIMESTAMP, {2}, CURRENT_TIMESTAMP, 1, '{3}', 2)", churchId, batchName, batchAmount, receivedDate.ToString("yyyy-MM-dd"));
            this.ExecuteDBQuery(query.ToString());
        }

        /// <summary>
        /// This method deletes schedule giving information.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="login">The login the giving is tied to.</param>
        public void DeleteScheduledGivingByChurchAndLogin(int churchId, string login) {
            StringBuilder query = new StringBuilder();
            this.ExecuteDBQuery(query.ToString());
        }

        /// <summary>
        /// This method creates an RDC Batch with one item equaling the amount of the batch.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="batchName">The name for the batch.</param>
        /// <param name="batchAmount">The name of the batch.</param>
        public void CreateRDCBatch(int churchId, string batchName, string batchAmount){
            StringBuilder query = new StringBuilder("DECLARE @batchName VARCHAR(50) ");
            query.Append("DECLARE @amount MONEY ");
            query.Append("DECLARE @batchId INT, @churchID INT, @ppMerchantAccountID INT, @locationID INT, @referenceImageID INT ");
            query.AppendFormat("SET @amount = {0} ", batchAmount);
            query.AppendFormat("SET @churchID = {0} ", churchId);
            query.AppendFormat("SET @batchName = '{0}' ", batchName);
            query.Append("SET @ppMerchantAccountID = (SELECT TOP 1 PP_MERCHANT_ACCOUNT_ID FROM ChmContribution.dbo.PP_MERCHANT_ACCOUNT WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND PP_MERCHANT_ACCOUNT_CODE = 'ftone') ");
            query.Append("SET @locationID = (SELECT TOP 1 Location_ID FROM ChmContribution.dbo.PP_ProfitStars_Merchant_Location WITH (NOLOCK) WHERE Church_ID = @churchID) ");
            query.Append("SET @referenceImageID = (SELECT TOP 1 reference_image_id FROM ChmContribution.dbo.REFERENCE_IMAGE WITH (NOLOCK) WHERE church_Id = @churchID) ");
            query.Append("INSERT INTO ChmContribution.dbo.BATCH (CHURCH_ID, BATCH_NAME, BATCH_DATE, BATCH_AMOUNT, BatchStatusID, BatchTypeID) ");
            query.Append("VALUES(@churchID, @batchName, CURRENT_TIMESTAMP, @amount, 4, 3) ");
            query.Append("SET @batchId = (SELECT SCOPE_IDENTITY()) ");
            query.Append("INSERT INTO ChmContribution.Pmt.RDCBatch (ChurchID, BatchID, BatchName, BatchCreatedDate, ItemCount, BatchAmount, PPMerchantAccountID, LocationID) ");
            query.Append("VALUES(@churchID, @batchId, @batchName, CURRENT_TIMESTAMP, 1, @amount, @ppMerchantAccountID, @locationID) ");
            query.Append("INSERT INTO ChmContribution.Pmt.RDCBatchItem (ChurchID, BatchID, ItemCreatedDate, AccountNumber, RoutingNumber, Amount, ReferenceImageID, ReferenceNumber, PPMerchantAccountID, LocationID, CreatedDate, CheckNumber) ");
            query.Append("VALUES(@churchID, @batchId, CURRENT_TIMESTAMP, '1234567890', '111000025', @amount, @referenceImageID, 'T:65H8W0X7w', @ppMerchantAccountID, @locationID, CURRENT_TIMESTAMP, 1099) ");
            this.ExecuteDBQuery(query.ToString());
        }

        /// <summary>
        /// This method calculates the total amount of contributions made on behalf an individual or household.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="householdID">The household id.</param>
        /// <param name="individualID">The individual id.</param>
        /// <param name="receivedDateFrom">The starting date to calculate the total from.</param>
        /// <returns>The total amount of contributions for the given starting date.</returns>
        public double GetContributionReceiptTotalForIndividualOrHousehold(int churchId, int? householdID, int? individualID, string receivedDateFrom) {
            StringBuilder query = new StringBuilder("SELECT SUM(AMOUNT) FROM ChmContribution.dbo.CONTRIBUTION_RECEIPT cr WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmContribution.dbo.FUND f WITH (NOLOCK) ");
            query.Append("ON f.FUND_ID = cr.FUND_ID ");

            if (individualID > 0) {
                query.AppendFormat("WHERE f.FUND_TYPE = 1 AND cr.CHURCH_ID = {0} AND HOUSEHOLD_ID = {1} AND INDIVIDUAL_ID = {2} AND RECEIVED_DATE > '{3}'", churchId, householdID, individualID, receivedDateFrom);
            }
            else if (individualID == int.MinValue) {
                query.AppendFormat("WHERE f.FUND_TYPE = 1 AND cr.CHURCH_ID = {0} AND HOUSEHOLD_ID = {1} AND RECEIVED_DATE > '{2}'", churchId, householdID, receivedDateFrom);
            }
            else {
                query.AppendFormat("WHERE f.FUND_TYPE = 1 AND cr.CHURCH_ID = {0} AND HOUSEHOLD_ID = {1} AND INDIVIDUAL_ID IS NULL AND RECEIVED_DATE > '{2}'", churchId, householdID, receivedDateFrom);
            }

            return Convert.ToDouble(this.ExecuteDBQuery(query.ToString()).Rows[0][0]);
        }

        /// <summary>
        /// This method fetches the most recent provider reply response code for the specified payment amount.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="paymentAmount">The amount of the payment.</param>
        /// <param name="dateTimeFrom">The starting date for the created date.</param>
        /// <param name="clientApplication">The client application that generated the payment.</param>
        /// <returns>The reason code for the payment.</returns>
        public int GetPaymentReasonCode(int churchId, double paymentAmount, DateTime dateTimeFrom, string clientApplication) {
            StringBuilder query = new StringBuilder("SELECT TOP 1 pay_rea.ReasonCode FROM ChmContribution.dbo.PaymentReason pay_rea WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmContribution.dbo.ProviderRequest pro_req WITH (NOLOCK) ");
            query.Append("ON pro_req.PaymentReasonID = pay_rea.PaymentReasonID ");
            query.Append("INNER JOIN ChmContribution.dbo.Payment pay WITH (NOLOCK) ");
            query.Append("ON pay.PaymentID = pro_req.PaymentID ");
            query.AppendFormat("WHERE pay.ChurchID = {0} AND pay.Amount = {1} AND pay.CreatedDate > '{2}' AND pay.ClientApplication = '{3}' ORDER BY pay.PaymentID DESC", churchId, paymentAmount, dateTimeFrom.ToString("yyyy-MM-dd HH:mm:ss"), clientApplication);

            return Convert.ToInt16(this.ExecuteDBQuery(query.ToString()).Rows[0]["ReasonCode"]);
        }

        /// <summary>
        /// This method fetches all of the fund names in the church tied to the specified merchant account.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="merchantAccountCode">The merchant account code.</param>
        /// <returns>Data table containing the names of the funds belonging to the merchant account, sorted in ascending order.</returns>
        public DataTable FetchFundNamesTiedToMerchantAccount(int churchId, string merchantAccountCode) {
            StringBuilder query = new StringBuilder("SELECT fund.FUND_NAME FROM ChmContribution.dbo.FUND fund WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmContribution.dbo.PP_ACCOUNT_REFERENCE acc_ref WITH (NOLOCK) ");
            query.Append("ON fund.PP_ACCOUNT_REFERENCE_ID = acc_ref.PP_ACCOUNT_REFERENCE_ID ");
            query.Append("INNER JOIN ChmContribution.dbo.PP_MERCHANT_ACCOUNT mer_acc WITH (NOLOCK) ");
            query.Append("ON acc_ref.PP_MERCHANT_ACCOUNT_ID = mer_acc.PP_MERCHANT_ACCOUNT_ID ");
            query.AppendFormat("WHERE fund.CHURCH_ID = {0} AND fund.IS_ACTIVE = 1 AND fund.IS_WEB_ACTIVE = 1 AND fund.FUND_TYPE IN (1, 2) AND mer_acc.PP_MERCHANT_ACCOUNT_CODE = '{1}' ", churchId, merchantAccountCode);
            query.Append("ORDER BY fund.FUND_NAME ASC");
            return this.ExecuteDBQuery(query.ToString());
        }

        /// <summary>
        /// This method returns all of the fund names in the specified church.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="isActive">Flag deisgnating active funds.</param>
        /// <param name="isWebActive">Flag designating web active funds.</param>
        /// <param name="hasAccountReferenceIdSet">Flag designating funds that have an account reference.</param>
        /// <returns>DataTable containing the names of the funds belonging to the church.</returns>
        public DataTable FetchFundNamesByChurch(int churchId, bool isActive, bool? isWebActive, bool? hasAccountReferenceIdSet) {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT FUND_NAME FROM ChmContribution.dbo.FUND WITH (NOLOCK) WHERE CHURCH_ID = {0} AND IS_ACTIVE = {1} ", churchId, Convert.ToInt16(isActive));
            if (isWebActive.HasValue) {
                if ((bool)isWebActive) {
                    query.AppendFormat("AND IS_WEB_ACTIVE = {0} ", Convert.ToInt16(isWebActive));
                }
            }

            if (hasAccountReferenceIdSet.HasValue) {
                if ((bool)hasAccountReferenceIdSet) {
                    query.AppendFormat("AND PP_ACCOUNT_REFERENCE_ID IS NOT NULL ");
                }
            }
            query.Append(" ORDER BY FUND_NAME ASC");
            return this.ExecuteDBQuery(query.ToString());
        }

        /// <summary>
        /// This method creates a fund.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="fundName">The name of the fund.</param>
        /// <param name="active">The active flag.</param>
        /// <param name="fundType">The type of the fund.</param>
        /// <param name="webActive">The web active flag.</param>
        /// <param name="accountReferenceDescription">The reference of the account reference the fund is to be tied to.</param>
        public void Giving_FundsCreate(int churchId, string fundName, bool active, string glAccount, int fundType, bool webActive, string accountReferenceDescription) {
            StringBuilder query = new StringBuilder("INSERT INTO ChmContribution.dbo.FUND (CHURCH_ID, FUND_NAME, IS_ACTIVE, GL_ACCOUNT, FUND_TYPE, IS_WEB_ACTIVE, PP_ACCOUNT_REFERENCE_ID) ");
            query.AppendFormat("SELECT {0}, '{1}', {2}, '{3}', {4}, {5}, PP_ACCOUNT_REFERENCE_ID FROM ChmContribution.dbo.PP_ACCOUNT_REFERENCE WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACCOUNT_DESC = '{6}'", churchId, fundName, Convert.ToInt16(active), glAccount, fundType, Convert.ToInt16(webActive), accountReferenceDescription);
            this.ExecuteDBQuery(query.ToString());
        }

        /// <summary>
        /// This method deletes a fund.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="fundName">The name of the fund.</param>
        public void Giving_FundsDelete(int churchId, string fundName) {
            this.ExecuteDBQuery(string.Format("DELETE FROM ChmContribution.dbo.FUND WHERE CHURCH_ID = {0} AND FUND_NAME = '{1}'", churchId, fundName));
        }
        #endregion Giving

        #region Groups
        /// <summary>
        /// Creates a group.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupTypeName">The name of the group type the group will belong to.</param>
        /// <param name="individualName">The individual name who will be creating the group.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="groupDescription">The description of the group.</param>
        /// <param name="startDate">The start date for the group.</param>
        public void Groups_CreateGroup(int churchId, string groupTypeName, string individualName, string groupName, string groupDescription, string startDate) {
            StringBuilder query = new StringBuilder("DECLARE @Group_Type_ID INT ");
            query.AppendFormat("SET @Group_Type_ID = (SELECT TOP 1 Group_Type_ID FROM ChmPeople.dbo.Group_Type WITH (NOLOCK) WHERE Church_ID = {0} AND Group_Type_Name = '{1}' AND Deleted_Date IS NULL) ", churchId, groupTypeName);
            query.Append("DECLARE @Individual_ID INT ");           
            query.AppendFormat("SET @Individual_ID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND INDIVIDUAL_NAME = '{1}') ", churchId, individualName);            
            query.Append("INSERT INTO ChmPeople.dbo.Groups (Church_ID, Group_Type_ID, Group_Name, Description, Is_Open, Is_Public, Start_Date, Created_By_Individual_ID) ");
            query.AppendFormat("VALUES ({0}, @Group_Type_ID, '{1}', '{2}', 1, 1, '{3}', @Individual_ID)", churchId, groupName, groupDescription, startDate);
            this.ExecuteDBQuery(query.ToString());
        }

        /// <summary>
        /// Adds a leader or member to a group.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupName">The group the leader or member will belong to.</param>
        /// <param name="individualName">The name of the individual.</param>
        /// <param name="leaderOrMember">Leader or member.</param>
        public void Groups_AddLeaderOrMemberToGroup(int churchId, string groupName, string individualName, string leaderOrMember) {
            // Figure out if we are adding a leader or a member
            var groupMemberSystemTypeValue = 0;

            if (leaderOrMember == "Leader") {
                groupMemberSystemTypeValue = 1;
            }
            else {
                groupMemberSystemTypeValue = 2;
            }

            StringBuilder query = new StringBuilder("DECLARE @churchID INT, @groupID INT, @individualID INT, @groupMemberTypeID INT ");
            query.AppendFormat("SET @churchID = {0} ", churchId);
            query.AppendFormat("SET @individualID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND INDIVIDUAL_NAME = '{0}') ", individualName);
            query.Append(string.Format("SET @groupMemberTypeID = (SELECT Group_Member_Type_ID FROM ChmPeople.dbo.Group_Member_Type WITH (NOLOCK) WHERE Church_ID = @churchID AND Group_Member_System_Type_ID = {0}) ", groupMemberSystemTypeValue));
            query.AppendFormat("SET @groupID = (SELECT TOP 1 Group_ID FROM ChmPeople.dbo.Groups WITH (NOLOCK) WHERE Church_ID = @churchID AND Group_Name = '{0}' AND Deleted_Date IS NULL) ", groupName);
            query.Append("INSERT INTO ChmPeople.dbo.Group_Member (Group_ID, Church_ID, Individual_ID, Group_Member_Type_ID, Created_By_Individual_ID) ");
            query.Append("VALUES (@groupID, @churchID, @individualID, @groupMemberTypeID, @individualID)");
            this.ExecuteDBQuery(query.ToString());
        }

        /// <summary>
        /// Updates a person in the group to a leader/member.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="individualName">The name of the individual in the group.</param>
        /// <param name="leaderOrMember">The new role for the individual</param>
        public void Groups_UpdateLeaderOrMemberRole(int churchId, string groupName, string individualName, string leaderOrMember) {
            // Figure out if we are adding a leader or a member
            var groupMemberSystemTypeValue = 0;

            if (leaderOrMember == "Leader") {
                groupMemberSystemTypeValue = 1;
            }
            else {
                groupMemberSystemTypeValue = 2;
            }

            StringBuilder query = new StringBuilder("DECLARE @churchID INT, @groupID INT, @individualID INT, @groupMemberTypeID INT ");
            query.AppendFormat("SET @churchID = {0} ", churchId);
            query.AppendFormat("SET @individualID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND INDIVIDUAL_NAME = '{0}') ", individualName);
            query.Append(string.Format("SET @groupMemberTypeID = (SELECT Group_Member_Type_ID FROM ChmPeople.dbo.Group_Member_Type WITH (NOLOCK) WHERE Church_ID = @churchID AND Group_Member_System_Type_ID = {0}) ", groupMemberSystemTypeValue));
            query.AppendFormat("SET @groupID = (SELECT TOP 1 Group_ID FROM ChmPeople.dbo.Groups WITH (NOLOCK) WHERE Church_ID = @churchID AND Group_Name = '{0}' AND Deleted_Date IS NULL) ", groupName);
            query.Append("UPDATE ChmPeople.dbo.Group_Member ");
            query.Append("SET Group_Member_Type_ID = @groupMemberTypeID ");
            query.Append("WHERE Group_ID = @groupID and Church_ID = @churchID and Individual_ID = @individualID");
            this.ExecuteDBQuery(query.ToString());
        }

        /// <summary>
        /// Creates a group type, sets an individual as an admin, and allows admins to post attendance.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualName">The name of the admin.</param>
        /// <param name="groupTypeName">The name of the group type.</param>
        [Obsolete("This method is now obsolete.  Use Groups_CreateGroupType")]
        public void Groups_CreateGroupTypeWithAdminAndAttendancePermissions(int churchId, string individualName, string groupTypeName) {
            StringBuilder query = new StringBuilder("DECLARE @churchID INT, @individualID INT, @groupTypeID INT, @groupMemberTypeID INT ");
            query.AppendFormat("SET @churchID = {0} ", churchId);
            query.AppendFormat("SET @individualID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND INDIVIDUAL_NAME = '{0}') ", individualName);
            query.Append("SET @groupMemberTypeID = (SELECT Group_Member_Type_ID FROM ChmPeople.dbo.Group_Member_Type WITH (NOLOCK) WHERE Church_ID = @churchID AND Group_Member_System_Type_ID = 1) ");
            query.Append("INSERT INTO ChmPeople.dbo.Group_Type (Church_ID, Group_Type_Name, Description, Created_By_Individual_ID, Is_Web_Enabled, Is_Searchable, GroupSystemTypeID) ");
            query.AppendFormat("VALUES (@churchID, '{0}', null, @individualID, 1, 1, 3) ", groupTypeName);
            query.Append("SET @groupTypeID = (SELECT SCOPE_IDENTITY()) ");
            query.Append("INSERT INTO ChmPeople.dbo.Group_Admin (Church_ID, Group_Type_ID, Individual_ID, Created_By_Individual_ID, Group_Admin_System_Type_ID) ");
            query.Append("VALUES (@churchID, @groupTypeID, @individualID, @individualID, 1) ");
            query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
            query.Append("VALUES (@churchID, @groupTypeID, @groupMemberTypeID, 10, @individualID)");
            this.ExecuteDBQuery(query.ToString());
        }

        /// <summary>
        /// Creates a group type, sets an individual as an admin, and allows admins to do all security rights to the group
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualName">The name of the admin.</param>
        /// <param name="groupTypeName">The name of the group type.</param>
        [Obsolete("This method is now obsolete.  Use Groups_CreateGroupType")]
        public void Groups_CreateGroupTypeWithAdminAndAllPermissions(int churchId, string individualName, string groupTypeName) {
            StringBuilder query = new StringBuilder("DECLARE @churchID INT, @individualID INT, @groupTypeID INT, @groupMemberTypeID INT ");
            query.AppendFormat("SET @churchID = {0} ", churchId);
            query.AppendFormat("SET @individualID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND INDIVIDUAL_NAME = '{0}') ", individualName);
            query.Append("SET @groupMemberTypeID = (SELECT Group_Member_Type_ID FROM ChmPeople.dbo.Group_Member_Type WITH (NOLOCK) WHERE Church_ID = @churchID AND Group_Member_System_Type_ID = 1) ");
            query.Append("INSERT INTO ChmPeople.dbo.Group_Type (Church_ID, Group_Type_Name, Description, Created_By_Individual_ID, Is_Web_Enabled, Is_Searchable, GroupSystemTypeID) ");
            query.AppendFormat("VALUES (@churchID, '{0}', null, @individualID, 1, 1, 3) ", groupTypeName);
            query.Append("SET @groupTypeID = (SELECT SCOPE_IDENTITY()) ");
            query.Append("INSERT INTO ChmPeople.dbo.Group_Admin (Church_ID, Group_Type_ID, Individual_ID, Created_By_Individual_ID, Group_Admin_System_Type_ID) ");
            query.Append("VALUES (@churchID, @groupTypeID, @individualID, @individualID, 1) ");
            query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
            query.Append("VALUES (@churchID, @groupTypeID, @groupMemberTypeID, 1, @individualID)");
            query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
            query.Append("VALUES (@churchID, @groupTypeID, @groupMemberTypeID, 2, @individualID)");
            query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
            query.Append("VALUES (@churchID, @groupTypeID, @groupMemberTypeID, 3, @individualID)");
            query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
            query.Append("VALUES (@churchID, @groupTypeID, @groupMemberTypeID, 4, @individualID)");
            query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
            query.Append("VALUES (@churchID, @groupTypeID, @groupMemberTypeID, 5, @individualID)");
            query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
            query.Append("VALUES (@churchID, @groupTypeID, @groupMemberTypeID, 7, @individualID)");
            query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
            query.Append("VALUES (@churchID, @groupTypeID, @groupMemberTypeID, 9, @individualID) ");
            query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
            query.Append("VALUES (@churchID, @groupTypeID, @groupMemberTypeID, 10, @individualID)");
            this.ExecuteDBQuery(query.ToString());
        }

        /// <summary>
        /// Creates a group type, sets an individual as an admin, and applies the appropriate permissions.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualName">The name of the individual.</param>
        /// <param name="groupTypename">The name of the group type.</param>
        /// <param name="leaderAdminRights">An array of rights to be configured.</param>
        //public void Groups_CreateGroupType(int churchId, string individualName, string groupTypeName, Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights[] leaderAdminRights) {
        public void Groups_CreateGroupType(int churchId, string individualName, string groupTypeName, List<int> leaderAdminRights) {
            // Get each right passed in and add it to an array of permissions.  Each element represents the database value for that security right.
            //foreach (Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights leaderMemberRights in leaderAdminRights) {
            //    switch (leaderMemberRights) {
            //        case FTTests.Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights.None:
            //            break;
            //        case FTTests.Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights.EmailGroup:
            //            permisions.Add(3);
            //            break;
            //        case FTTests.Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights.AdministerGroup:
            //            permisions.Add(1);
            //            break;
            //        case FTTests.Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights.EditRecords:
            //            permisions.Add(2);
            //            break;
            //        case FTTests.Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights.EditDetails:
            //            permisions.Add(9);
            //            break;
            //        case FTTests.Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights.ChangeScheduleLocation:
            //            permisions.Add(4);
            //            permisions.Add(5);
            //            break;
            //        case FTTests.Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights.TakeAttendance:
            //            permisions.Add(10);
            //            break;
            //        default:
            //            break;
            //    }
            //}

            // Add the leader view right
            //permisions.Add(7);
            leaderAdminRights.Add(7);

            // Write the query
            StringBuilder query = new StringBuilder("DECLARE @churchID INT, @individualID INT, @groupTypeID INT, @groupMemberTypeID INT ");
            query.AppendFormat("SET @churchID = {0} ", churchId);
            query.AppendFormat("SET @individualID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND INDIVIDUAL_NAME = '{0}') ", individualName);
            query.Append("SET @groupMemberTypeID = (SELECT Group_Member_Type_ID FROM ChmPeople.dbo.Group_Member_Type WITH (NOLOCK) WHERE Church_ID = @churchID AND Group_Member_System_Type_ID = 1) ");
            query.Append("INSERT INTO ChmPeople.dbo.Group_Type (Church_ID, Group_Type_Name, Description, Created_By_Individual_ID, Is_Web_Enabled, Is_Searchable, GroupSystemTypeID) ");
            query.AppendFormat("VALUES (@churchID, '{0}', null, @individualID, 1, 1, 3) ", groupTypeName);
            query.Append("SET @groupTypeID = (SELECT SCOPE_IDENTITY()) ");
            query.Append("INSERT INTO ChmPeople.dbo.Group_Admin (Church_ID, Group_Type_ID, Individual_ID, Created_By_Individual_ID, Group_Admin_System_Type_ID) ");
            query.Append("VALUES (@churchID, @groupTypeID, @individualID, @individualID, 1) ");

            // Append the insert statements for each security right.
            //for (int i = 0; i < permisions.Count; i++) {
            for (int i = 0; i < leaderAdminRights.Count; i++) {
                query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
                //query.Append(string.Format("VALUES (@churchID, @groupTypeID, @groupMemberTypeID, {0}, @individualID)", permisions[i]));
                query.Append(string.Format("VALUES (@churchID, @groupTypeID, @groupMemberTypeID, {0}, @individualID)", leaderAdminRights[i]));
            }

            // ExecuteDBQuery the query
            this.ExecuteDBQuery(query.ToString());
        }

        /// <summary>
        /// Creates an attendance summary record for a group
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="totalCount">The total number of people in the group.</param>
        public void Groups_CreateAttendanceSummary(int churchId, string groupName, string totalCount) {
            StringBuilder query = new StringBuilder("DECLARE @churchID INT, @groupID INT ");
            query.AppendFormat("SET @churchID = {0} ", churchId);
            query.Append(string.Format("SET @groupID = (SELECT TOP 1 Group_ID FROM ChmPeople.dbo.Groups WITH (NOLOCK) WHERE Church_ID = @churchID AND group_name = '{0}' and deleted_date is null) ", groupName));
            query.Append("insert into ChmPeople.dbo.AttendanceSummary(ChurchID, AttendanceContextTypeID, AttendanceContextValueID, StartDateTime, EndDateTime, PresentCount, AbsentCount, TotalCount) ");
            query.Append(string.Format("values (@churchID, 1, @groupID, GETDATE() - 1, GETDATE() + 1, 0, 0, {0})", totalCount));
            this.ExecuteDBQuery(query.ToString());
        }

        /// <summary>
        /// Deletes all Attendance Summaries for a group.
        /// </summary>
        /// <param name="churchId">The church id./param>
        /// <param name="groupName">The name of the group.</param>
        public void Groups_DeleteAttendanceSummary(int churchId, string groupName) {
            StringBuilder query = new StringBuilder("DECLARE @churchID INT, @groupID INT ");
            query.AppendFormat("SET @churchID = {0} ", churchId);
            query.Append(string.Format("SET @groupID = (SELECT TOP 1 Group_ID FROM ChmPeople.dbo.Groups WITH (NOLOCK) WHERE Church_ID = @churchID AND group_name = '{0}' and deleted_date is null) ", groupName));
            query.Append("delete from ChmPeople.dbo.Attendancesummary WHERE AttendanceContextValueId = @groupid");
            this.ExecuteDBQuery(query.ToString());
        }

        /// <summary>
        /// Deletes a group type in the database along with all groups tied to it.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupTypeName">The group type name.</param>
        public void Groups_DeleteGroupType(int churchId, string groupTypeName) {
            DataTable groupTypes = this.ExecuteDBQuery(string.Format("SELECT Group_Type_ID FROM ChmPeople.dbo.Group_Type WITH (NOLOCK) WHERE Church_ID = {0} AND Group_Type_Name = '{1}' AND Deleted_Date IS NULL", churchId, groupTypeName));
            if (groupTypes.Rows.Count > 0) {
                this.ExecuteDBQuery(string.Format("UPDATE ChmPeople.dbo.Groups SET Deleted_Date = CURRENT_TIMESTAMP, Deleted_By_Individual_ID = {0} WHERE Church_ID = {1} AND Group_Type_ID = '{2}' AND Deleted_Date IS NULL", base.IndividualId, churchId, groupTypes.Rows[0]["Group_Type_ID"]));
                this.ExecuteDBQuery(string.Format("UPDATE ChmPeople.dbo.Group_Type SET Deleted_Date = CURRENT_TIMESTAMP, Deleted_By_Individual_ID = {0} WHERE Church_ID = {1} AND Group_Type_Name = '{2}' AND Deleted_Date IS NULL", base.IndividualId, churchId, groupTypeName));
            }
        }

        /// <summary>
        /// Deletes a group from the database.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupName">The group name.</param>
        public void Groups_DeleteGroup(int churchId, string groupName) {
            this.ExecuteDBQuery(string.Format("UPDATE ChmPeople.dbo.Groups SET Deleted_Date = CURRENT_TIMESTAMP, Deleted_By_Individual_ID = {0} WHERE Church_ID = {1} AND Group_Name = '{2}' AND Deleted_Date IS NULL", base.IndividualId, churchId, groupName));
        }

        /// <summary>
        /// Generates all the attendance schedules for groups.
        /// </summary>
        public void Groups_GenerateAttendanceDates() {
            this.ExecuteDBQuery(string.Format("EXEC {0}", "ChmPeople.dbo.AttendanceSummary_BatchCreate"));
        }
        #endregion Groups

        #region People
        /// <summary>
        /// Deletes an address.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualId">The individual id.</param>
        /// <param name="address1">The first line of the address.</param>
        [Obsolete]
        public void People_DeleteAddress(int churchId, int individualId, string address1) {
            base.ExecuteDBQuery(string.Format("DELETE FROM ChmPeople.dbo.HOUSEHOLD_ADDRESS WHERE CHURCH_ID = {0} AND INDIVIDUAL_ID = {1} AND ADDRESS_1 = '{2}'", churchId, individualId, address1));
        }

        /// <summary>
        /// Fetches the individual id.
        /// </summary>
        /// <param name="churchId">The church id the individual belongs to.</param>
        /// <param name="individualName">The name of the individual.</param>
        [Obsolete]
        public int People_GetIndividualId(int churchId, string individualName) {
            return Convert.ToInt32(this.ExecuteDBQuery(string.Format("SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND INDIVIDUAL_NAME = '{1}'", churchId, individualName)).Rows[0]["INDIVIDUAL_ID"]);
        }

        /// <summary>
        /// Deletes a prospect from a group.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupName">The name of the group the prospect is tied to.</param>
        /// <param name="firstName">The prospect's first name.</param>
        /// <param name="lastName">The prospect's last name.</param>
        /// <param name="email">The prospect's email.</param>
        public void DeleteProspectInfo(int churchId, string groupName, string firstName, string lastName, string email) {
            StringBuilder script = new StringBuilder("DECLARE @churchID INT, @groupName VARCHAR(50), @groupID INT, @firstName VARCHAR(25), @lastName VARCHAR(25), @email VARCHAR(50) ");
            script.AppendFormat("SET @churchID = {0} ", churchId);
            script.AppendFormat("SET @groupName = '{0}' ", groupName);
            script.Append("SET @groupID = (SELECT TOP 1 Group_ID FROM ChmPeople.dbo.Groups WITH (NOLOCK) WHERE Church_ID = @churchID AND Group_Name = @groupName AND Deleted_Date IS NULL) ");
            script.AppendFormat("SET @firstName = '{0}' ", firstName);
            script.AppendFormat("SET @lastName = '{0}' ", lastName);
            script.AppendFormat("SET @email = '{0}' ", email);
            script.Append("CREATE TABLE #TaskIDs (ID INT IDENTITY(1,1), task_ID INT) ");
            script.Append("INSERT INTO #TaskIDs (task_ID) SELECT TaskID FROM ChmPeople.dbo.Task WITH (NOLOCK) WHERE ChurchID = @churchID AND CommunicationTargetID IN (SELECT CommunicationTargetID FROM ChmPeople.dbo.CommunicationTarget WITH (NOLOCK) WHERE ChurchID = @churchID AND GroupID = @groupID AND FirstName = @firstName AND LastName = @lastName AND Email = @email) ");
            script.Append("DECLARE @startid INT, @endid INT, @taskID INT ");
            script.Append("SET @startid = (SELECT MIN(ID) FROM #TaskIDs) ");
            script.Append("SET @endid = (SELECT MAX(ID) FROM #TaskIDs) ");
            script.Append("WHILE (@startid <= @endid) ");
            script.Append("BEGIN ");
            script.Append("SELECT @taskID = task_ID FROM #TaskIDs WHERE ID = @startid ");
            script.Append("DELETE FROM ChmPeople.dbo.TaskIndividual WHERE ChurchID = @churchID AND TaskID = @taskID ");
            script.Append("DELETE FROM ChmPeople.dbo.TaskEvent WHERE ChurchID = @churchID AND TaskID = @taskID ");
            script.Append("DELETE FROM ChmPeople.dbo.UserLoginActionCode WHERE ChurchID = @churchID AND ContextTypeDataID = @taskID ");
            script.Append("DELETE FROM ChmPeople.dbo.Task WHERE ChurchID = @churchID AND TaskID = @taskID ");
            script.Append("DELETE FROM ChmPeople.dbo.CommunicationTarget WHERE ChurchID = @churchID AND CommunicationTargetID = (SELECT CommunicationTargetID FROM ChmPeople.dbo.Task WITH (NOLOCK) WHERE ChurchID = @churchID AND TaskID = @taskID) ");
            script.Append("SET @startid = @startid + 1 ");
            script.Append("END ");
            script.Append("DROP TABLE #TaskIDs");

            this.ExecuteDBQuery(script.ToString());
        }

        /// <summary>
        /// Deletes a template.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="templateName">The name of the template.</param>
        [Obsolete]
        public void People_DeleteTemplate(int churchId, string templateName) {
            this.ExecuteDBQuery(string.Format("DELETE FROM ChmEmail.dbo.EMAIL_TEMPLATE WHERE CHURCH_ID = {0} AND TEMPLATE_NAME = '{1}'", churchId, templateName));
        }


        /// <summary>
        /// This method creates an email delegate.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="userLogin">The login of the user.</param>
        /// <param name="delegateLogin">The login of the delgate to be created.</param>
        [Obsolete]
        public void People_CreateDelegate(int churchId, string userLogin, string delegateLogin) {
            StringBuilder query = new StringBuilder("DECLARE @churchID INT, @userID INT, @delegateForUserID INT ");
            query.AppendFormat("SET @churchID = {0} ", churchId);
            query.AppendFormat("SET @userID = (SELECT TOP 1 USER_ID FROM ChmChurch.dbo.USERS WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND LOGIN = '{0}') ", delegateLogin);
            query.AppendFormat("SET @delegateForUserID = (SELECT TOP 1 USER_ID FROM ChmChurch.dbo.USERS WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND LOGIN = '{0}') ", userLogin);
            query.Append("INSERT INTO ChmEmail.dbo.EMAIL_USER_PROXY (CHURCH_ID, USER_ID, PROXY_FOR_USER_ID, LAST_UPDATED_DATE) ");
            query.Append("VALUES (@churchID, @userID, @delegateForUserID, CURRENT_TIMESTAMP)");
            this.ExecuteDBQuery(query.ToString());
        }

        /// <summary>
        /// This method deletes an email delegate.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="userLogin">The login of the user.</param>
        /// <param name="delegateLogin">The login of the delegate to be deleted.</param>
        [Obsolete]
        public void People_DeleteDelegate(int churchId, string userLogin, string delegateLogin) {
            StringBuilder query = new StringBuilder("DECLARE @churchID INT, @userID INT, @delegateForUserID INT ");
            query.AppendFormat("SET @churchID = {0} ", churchId);
            query.AppendFormat("SET @userID = (SELECT TOP 1 USER_ID FROM ChmChurch.dbo.USERS WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND LOGIN = '{0}') ", delegateLogin);
            query.AppendFormat("SET @delegateForUserID = (SELECT TOP 1 USER_ID FROM ChmChurch.dbo.USERS WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND LOGIN = '{0}') ", userLogin);
            query.Append("DELETE FROM ChmEmail.dbo.EMAIL_USER_PROXY WHERE CHURCH_ID = @churchID AND USER_ID = @userID AND PROXY_FOR_USER_ID = @delegateForUserID");
            this.ExecuteDBQuery(query.ToString());
        }

        /// <summary>
        /// This method returns the most recent activation code for the specified user.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="email">The email of the user.</param>
        /// <returns>The activation code for the user.</returns>
        public string FetchUserActivationCode(int churchId, string email) {
            return this.ExecuteDBQuery(string.Format("SELECT * FROM ChmPeople.dbo.UserLoginActivation WITH (NOLOCK) WHERE ChurchID = {0} AND Email = '{1}' ORDER BY CreatedDate DESC", churchId, email)).Rows[0]["ActivationCode"].ToString();
        }

        /// <summary>
        /// This method fetches the individual and household ids for the given email address.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="email">The email address of the individual.</param>
        /// <returns>DataTable containing the individual and household ids.</returns>
        public DataTable GetIndividualHouseholdIdFromEmail(int churchId, string email) {
            StringBuilder query = new StringBuilder("SELECT H.HOUSEHOLD_ID, IN_H.INDIVIDUAL_ID FROM ChmPeople.dbo.HOUSEHOLD H WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmPeople.dbo.INDIVIDUAL_HOUSEHOLD IN_H WITH (NOLOCK) ");
            query.Append("ON H.HOUSEHOLD_ID = IN_H.HOUSEHOLD_ID ");
            query.Append("INNER JOIN ChmPeople.dbo.UserLogin UL WITH (NOLOCK) ");
            query.Append("ON UL.IndividualID = IN_H.INDIVIDUAL_ID ");
            query.AppendFormat("WHERE UL.ChurchID = {0} AND UL.Email = '{1}' AND UL.DeletedDate IS NULL", churchId, email);

            //StringBuilder filter = new StringBuilder();
            //if (email.Length == 1) {
            //    //filter.AppendFormat("WHERE UL.ChurchID = {0} AND UL.Email = '{1}' AND UL.DeletedDate IS NULL)", churchID, emails[0]);
            //    filter.AppendFormat("WHERE UL.ChurchID = {0} AND UL.Email = '{1}' AND UL.DeletedDate IS NULL", churchId, email[0]);
            //}
            //else {
            //    filter.AppendFormat("WHERE UL.ChurchID = {0} AND UL.Email IN (", churchId);

            //    for (int i = 0; i < email.Length; i++) {
            //        filter.AppendFormat("'{0}'", email[i]);
            //        if (i != email.Length - 1) {
            //            filter.Append(", ");
            //        }
            //        else {
            //            //filter.Append(") AND UL.DeletedDate IS NULL)");
            //            filter.Append(") AND UL.DeletedDate IS NULL");
            //        }
            //    }
            //}

            //query.Append(filter.ToString());

            //StringBuilder householdsQuery = new StringBuilder("SELECT H.HOUSEHOLD_ID, IN_H.INDIVIDUAL_ID,UL.Email FROM ChmPeople.dbo.HOUSEHOLD H WITH (NOLOCK) ");
            //householdsQuery.Append("INNER JOIN ChmPeople.dbo.INDIVIDUAL_HOUSEHOLD IN_H WITH (NOLOCK) ON H.HOUSEHOLD_ID = IN_H.HOUSEHOLD_ID ");
            //householdsQuery.Append("INNER JOIN ChmPeople.dbo.UserLogin UL WITH (NOLOCK) ON UL.IndividualID = IN_H.INDIVIDUAL_ID ");
            //householdsQuery.Append("WHERE UL.ChurchID = 15 AND UL.Email in ");
            //householdsQuery.Append("('msneeden@fellowshiptech.com','ljiang@fellowshiptech.com','rubyzhou2018@yahoo.com','sunnyjiang2018@yahoo.com','kerryyo2018@yahoo.com','rosemarry2018@yahoo.com') order by UL.Email ASC");
            //hhResults = this.ExecuteDBQueryDBQuery(householdsQuery.ToString());

            return this.ExecuteDBQuery(query.ToString());
        }

        /// <summary>
        /// This method returns the names of all of the individuals in the specified household.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="householdId">The household id.</param>
        /// <returns>DataTable containing the individual names belonging to the household.</returns>
        public DataTable FetchIndividualsInHouseholdByHouseholdID(int churchId, int householdId) {
            StringBuilder query = new StringBuilder("SELECT INDIVIDUAL_NAME FROM ChmPeople.dbo.INDIVIDUAL_HOUSEHOLD ih WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmPeople.dbo.INDIVIDUAL ind WITH (NOLOCK) ");
            query.Append("ON ih.INDIVIDUAL_ID = ind.INDIVIDUAL_ID ");
            query.AppendFormat("WHERE ind.CHURCH_ID = {0} AND ih.HOUSEHOLD_ID = {1}", churchId, householdId);

            return this.ExecuteDBQuery(query.ToString());
        }

        /// <summary>
        /// This method calls the MergeIndividual stored procedure.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualFrom">The name of the duplicate individual.</param>
        /// <param name="individualTo">The name of the master individual.</param>
        [Obsolete]
        public void MergeIndividual(int churchId, string individualFrom, string individualTo) {
            try {

                int fromINDId = Convert.ToInt32(this.ExecuteDBQuery(string.Format("SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND INDIVIDUAL_NAME = '{1}'", churchId, individualFrom)).Rows[0][0]);
                int toINDId = Convert.ToInt32(this.ExecuteDBQuery(string.Format("SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND INDIVIDUAL_NAME = '{1}'", churchId, individualTo)).Rows[0][0]);
                int fromHHId = Convert.ToInt32(this.ExecuteDBQuery(string.Format("SELECT TOP 1 HOUSEHOLD_ID FROM ChmPeople.dbo.INDIVIDUAL_HOUSEHOLD WITH (NOLOCK) WHERE CHURCH_ID = {0} AND INDIVIDUAL_ID = {1}", churchId, fromINDId)).Rows[0][0]);
                int toHHId = Convert.ToInt32(this.ExecuteDBQuery(string.Format("SELECT TOP 1 HOUSEHOLD_ID FROM ChmPeople.dbo.INDIVIDUAL_HOUSEHOLD WITH (NOLOCK) WHERE CHURCH_ID = {0} AND INDIVIDUAL_ID = {1}", churchId, toINDId)).Rows[0][0]);


                using (SqlConnection conn = new SqlConnection(base.GetDBConnectionString)) {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("ChmPeople.dbo.MergeIndividual", conn)) {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MovingToHouseholdID", toHHId);
                        cmd.Parameters.AddWithValue("@MovingFromHouseholdID", fromHHId);
                        cmd.Parameters.AddWithValue("@MovingToIndividualID", toINDId);
                        cmd.Parameters.AddWithValue("@MovingFromIndividualID", fromINDId);
                        cmd.Parameters.AddWithValue("@ChurchID", churchId);
                        cmd.ExecuteReader();
                    }
                    conn.Close();
                }
            }
            catch (Exception ex){
                if (ex is SqlException || ex is IndexOutOfRangeException) { }
                else { throw; }
            }
        }
        #endregion People

        #region Ministry
        /// <summary>
        /// This method deletes an activity schedule.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityScheduleName">The name of the activity schedule.</param>
        [Obsolete]
        public void DeleteActivitySchedule(int churchId, string activityScheduleName) {
            this.ExecuteDBQuery(string.Format("DELETE FROM ChmActivity.dbo.ACTIVITY_TIME WHERE CHURCH_ID = {0} AND ACTIVITY_TIME_NAME = '{1}'", churchId, activityScheduleName));
        }

        /// <summary>
        /// This method creates a staffing schedule.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityName">The activity name this schedule will be tied to.</param>
        /// <param name="staffingScheduleName">The name of the staffing schedule.</param>
        [Obsolete]
        public void CreateStaffingSchedule(int churchId, string activityName, string staffingScheduleName) {
            StringBuilder query = new StringBuilder("DECLARE @activityID INT ");
            query.AppendFormat("SET @activityID = (SELECT TOP 1 ACTIVITY_ID FROM ChmActivity.dbo.ACTIVITY WHERE ACTIVITY_NAME = '{0}' AND CHURCH_ID = {1} AND IS_ACTIVE = 1) ", activityName, churchId);
            query.Append("INSERT INTO ChmActivity.dbo.STAFFING_SCHEDULE (CHURCH_ID, ACTIVITY_ID, STAFFING_SCHEDULE_NAME) ");
            query.AppendFormat("VALUES({0}, @activityID, '{1}')", churchId, staffingScheduleName);

            this.ExecuteDBQuery(query.ToString());
        }

        /// <summary>
        /// This method deletes a staffing schedule.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityName">The activity name the schedule is tied to.</param>
        /// <param name="staffingScheduleName">The name of the staffing schedule.</param>
        [Obsolete]
        public void DeleteStaffingSchedule(int churchId, string activityName, string staffingScheduleName) {
            StringBuilder query = new StringBuilder("DECLARE @activityID INT ");
            query.AppendFormat("SET @activityID = (SELECT TOP 1 ACTIVITY_ID FROM ChmActivity.dbo.ACTIVITY WHERE ACTIVITY_NAME = '{0}' AND CHURCH_ID = {1} AND IS_ACTIVE = 1) ", activityName, churchId);
            query.AppendFormat("DELETE FROM ChmActivity.dbo.STAFFING_SCHEDULE WHERE CHURCH_ID = {0} AND ACTIVITY_ID = @activityID AND STAFFING_SCHEDULE_NAME = '{1}'", churchId, staffingScheduleName);

            this.ExecuteDBQuery(query.ToString());
        }
        #endregion Ministry

        #region WebLink
        /// <summary>
        /// This method fetches the event registration confirmation code for an event registration.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="amount">The amount of the payment.</param>
        /// <param name="paymentMethod">The method of payment.</param>
        /// <returns>The confirmation code for the event registration.</returns>
        public string FetchEventRegistrationConfirmationCode(int churchId, double amount, string paymentMethod) {
            StringBuilder query = new StringBuilder("SELECT TOP 1 CONFIRMATION_CODE FROM ChmActivity.dbo.FORM_INDIVIDUAL_SET formIndSet ");
            query.Append("INNER JOIN ChmActivity.dbo.PAY_TRANSACTION payTrans ");
            query.Append("ON formIndSet.PAY_ORDER_SET_ID = payTrans.PAY_ORDER_SET_ID ");
            query.AppendFormat("WHERE PayTrans.CHURCH_ID = {0} AND payTrans.AMOUNT = {1} AND payTrans.PAYMENT_DESCRIPTION LIKE '{2}%' ORDER BY payTrans.CREATED_DATE DESC", churchId, amount, paymentMethod);
            return this.ExecuteDBQuery(query.ToString()).Rows[0]["CONFIRMATION_CODE"].ToString();
        }

        /// <summary>
        /// This method creates a form name.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="formName">The name of the form.</param>
        /// <param name="isActive">Flag designating active status of the form.</param>
        public void WebLink_FormNamesCreate(int formTypeId, int churchId, string formName, bool isActive) {
            StringBuilder query = new StringBuilder("INSERT INTO ChmActivity.dbo.FORM (FORM_TYPE_ID, CHURCH_ID, FORM_NAME, IS_ACTIVE) ");
            query.AppendFormat("VALUES({0}, {1}, '{2}', {3})", formTypeId, churchId, formName, Convert.ToInt16(isActive));

            this.ExecuteDBQuery(query.ToString());
        }

        /// <summary>
        /// This method deletes a form name.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="formName">The name of the form.</param>
        public void WebLink_FormNamesDelete(int churchId, string formName) {
            this.ExecuteDBQuery(string.Format("DELETE FROM ChmActivity.dbo.FORM WHERE CHURCH_ID = {0} AND FORM_NAME = '{1}'", churchId, formName));
        }
        #endregion WebLink

        #region Reports
        /// <summary>
        /// Deletes a label style.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="labelStyleName">The name of the label style.</param>
        public void Reports_DeleteLabelStyle(int churchId, string labelStyleName) {
            //int mle_format_id = (int)base.ExecuteDBQuery(string.Format("SELECT TOP 1 MLE_FORMAT_ID FROM ChmChurch.dbo.MLE_FORMAT WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FORMAT_NAME = '{1}'", churchId, labelStyleName)).Rows[0]["MLE_FORMAT_ID"];

            StringBuilder query = new StringBuilder("DELETE lFormat FROM ChmChurch.dbo.MLE_LABEL_FORMAT lFormat ");
            query.Append("INNER JOIN ChmChurch.dbo.MLE_FORMAT format ");
            query.Append("ON lFormat.MLE_FORMAT_ID = format.MLE_FORMAT_ID ");
            query.AppendFormat("WHERE format.CHURCH_ID = {0} AND format.FORMAT_NAME = '{1}' ", churchId, labelStyleName);
            //query.AppendFormat("DELETE FROM ChmChurch.dbo.MLE_LABEL_FORMAT WHERE MLE_FORMAT_ID = {0} ", mle_format_id);
            query.AppendFormat("DELETE FROM ChmChurch.dbo.MLE_FORMAT WHERE CHURCH_ID = {0} AND FORMAT_NAME = '{1}'", churchId, labelStyleName);
            base.ExecuteDBQuery(query.ToString());
        }
        #endregion Reports
    }
    #endregion Old Stuff
}