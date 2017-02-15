using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;
using System.IO;
using System.Net;
using System.Xml;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Threading;

namespace ChangeRequest {
    class Program {
        // possible args: duration, threads, target url, postdata, etc.
        static void Main(string[] args) {
            Console.WriteLine("QA Load Testing App");
            Console.Write("Enter the test xml file name: ");
            string xmlFile = Console.ReadLine();

            // Attempt to open the xml file specified by the user
            XmlDocument test = new XmlDocument();
            try {
                test.Load(string.Format("../../../{0}", xmlFile));
            }
            catch (FileNotFoundException e) {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                Environment.Exit(2);
            }

            // Extract the configuration and test details
            Enums.Applications app = (Enums.Applications)Enum.Parse(typeof(Enums.Applications), test.SelectSingleNode("//configuration/application").InnerText.ToUpper());
            Enums.Environments environment = (Enums.Environments)Enum.Parse(typeof(Enums.Environments), test.SelectSingleNode("//configuration/environment").InnerText.ToUpper());
            string churchCode = test.SelectSingleNode("//configuration/churchCode").InnerText;
            int maxThreads = Convert.ToInt16(test.SelectSingleNode("//configuration/threads").InnerText);
            int rampUp = Convert.ToInt16(test.SelectSingleNode("//configuration/rampUp").InnerText);

            int duration = int.MinValue;
            bool isTimedRun = int.TryParse(test.SelectSingleNode("//configuration/duration").InnerText, out duration);
            
            string URL = test.SelectSingleNode("//configuration/targetURL").InnerText;
            string requestType = test.SelectSingleNode("//configuration/requestType").InnerText;
            XmlNode credentials = test.SelectSingleNode("//credentials");
            XmlNodeList pData = test.SelectNodes("//postData/dataItem");

            // Configure the test engine
            ThreadPool.SetMaxThreads(maxThreads, maxThreads);
            ThreadPool.SetMinThreads(maxThreads, maxThreads);

            Actions action = new Actions();
            DateTime stopTime = DateTime.Now.AddSeconds(duration);

            // If the request is a post generate the data, if not, set to null
            string targetReqData = requestType == "POST" ? Helpers.GenerateTargetRequestData(pData) : null;

            if (isTimedRun) {
                while (DateTime.Now < stopTime) {
                    int workerThreads;
                    int portThreads;

                    ThreadPool.GetAvailableThreads(out workerThreads, out portThreads);

                    if (workerThreads > 0) {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(action.MakeRequests), new TestInfo() { application = app, loginURL = Helpers.FormatLoginURL(app, churchCode, environment), loginRequestData = Helpers.GenerateLoginRequestData(app, credentials, churchCode), targetURL = URL, targetRequestData = targetReqData });
                        Thread.Sleep(250);
                    }
                    else {
                        var timeLeft = (stopTime - DateTime.Now);
                        Console.WriteLine("Queue is full... time left {0:0}:{1:00}", timeLeft.Minutes, timeLeft.Seconds);
                        Thread.Sleep(1000);
                    }
                }
            }
            else {
                for (int i = 0; i < maxThreads; i++) {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(action.MakeRequests), new TestInfo() { application = app, loginURL = Helpers.FormatLoginURL(app, churchCode, environment), loginRequestData = Helpers.GenerateLoginRequestData(app, credentials, churchCode), targetURL = URL, targetRequestData = targetReqData });
                    Thread.Sleep(250);
                }
            }

            // Allow the existing requests to burnoff before exiting the application
            int threadsLeft = 1;
            int portThreadsLeft = 1;
            DateTime burnoff = DateTime.Now.AddSeconds(180);

            Console.WriteLine("Finished making requests, entering the burnoff period.");
            while (DateTime.Now < burnoff && threadsLeft < maxThreads) {
                ThreadPool.GetAvailableThreads(out threadsLeft, out portThreadsLeft);

                Console.WriteLine("Waiting for {0} thread(s) to finish...", maxThreads - threadsLeft);

                Thread.Sleep(1000);
            }

            Console.WriteLine("Test run completed. Press any key to close the application.");
            Console.ReadKey();
        }
    }


    public class TestInfo {
        public Enums.Applications application { get; set; }
        public string loginURL { get; set; }
        public string loginRequestData { get; set; }
        public string targetURL { get; set; }
        public string targetRequestData { get; set; }
        public string requestType { get; set; }
    }

    public class Actions {
        private int totalTransactions = 0;

        public void MakeRequests(Object stateInfo) {
            var requestInfo = stateInfo as TestInfo;

            totalTransactions++;
            int currentTransaction = totalTransactions;
            Console.WriteLine("Begin Transaction # {0}", currentTransaction);

            // If the target application is portal, make a request to the home page to get the viewstate
            CookieCollection loginPageResponseCookies = new CookieCollection();
            string cleanedViewState = string.Empty;
            if (requestInfo.application == Enums.Applications.PORTAL || requestInfo.application == Enums.Applications.REPORTLIBRARY) {
                HttpWebRequest loginPageRequest = (HttpWebRequest)WebRequest.Create(requestInfo.loginURL);
                loginPageRequest.CookieContainer = new CookieContainer();

                StringBuilder sb = new StringBuilder();

                // Scrape the screen to get the viewstate
                HttpWebResponse loginPageResponse = (HttpWebResponse)loginPageRequest.GetResponse();
                loginPageResponseCookies = loginPageResponse.Cookies;
                using (StreamReader stream = new StreamReader(loginPageResponse.GetResponseStream())) {
                    string strLine;
                    while ((strLine = stream.ReadLine()) != null) {
                        // Ignore blank lines

                        if (strLine.Length > 0)
                            sb.Append(strLine);
                    }

                    stream.Close();
                }

                string rawData = sb.ToString();

                Regex filters = new Regex("id=\"__VIEWSTATE\" value=\"/[a-z, A-Z, 0-9, =, /, +]*\" />");
                string rawViewState = filters.Match(rawData).Value;
                cleanedViewState = rawViewState.Replace("id=\"__VIEWSTATE\" value=\"", " ");
                cleanedViewState = cleanedViewState.Replace("\" />", "").TrimStart(' ');
            }

            // Generate a web request to login to the application
            HttpWebRequest loginRequest = (HttpWebRequest)WebRequest.Create(requestInfo.loginURL);

            byte[] byteArrayLogin = requestInfo.application == Enums.Applications.PORTAL ? Encoding.UTF8.GetBytes(string.Format("__EVENTTARGET=&__EVENTARGUMENT=&__VIEWSTATE={0}&{1}", HttpUtility.UrlEncode(cleanedViewState), requestInfo.loginRequestData)) : Encoding.UTF8.GetBytes(requestInfo.loginRequestData);
            loginRequest.ContentType = "application/x-www-form-urlencoded";
            loginRequest.Method = "POST";
            loginRequest.ContentLength = byteArrayLogin.Length;
            loginRequest.CookieContainer = new CookieContainer();

            if (requestInfo.application == Enums.Applications.REPORTLIBRARY) {
                loginRequest.CookieContainer.Add(loginPageResponseCookies[".F1RL"]);
            }

            loginRequest.AllowAutoRedirect = false;

            using (Stream postDataStream = loginRequest.GetRequestStream()) {
                postDataStream.Write(byteArrayLogin, 0, byteArrayLogin.Length);
            }

            // Get the response
            HttpWebResponse loginResponse = (HttpWebResponse)loginRequest.GetResponse();
            
            Cookie aspNet = new Cookie();

            // If the application is portal, make a request to get the ASP.NET_SessionId cookie
            if (requestInfo.application == Enums.Applications.PORTAL) {
                HttpWebRequest initSessionRequest = (HttpWebRequest)WebRequest.Create(requestInfo.loginURL.Replace("/login.aspx", string.Format("/InitSession.aspx?id={0}", loginResponse.Cookies["userID"].Value)));
                initSessionRequest.CookieContainer = Helpers.RetrieveCookiesFromResponse(requestInfo.application, loginResponse);
                initSessionRequest.AllowAutoRedirect = false;
                HttpWebResponse initSessionResponse = (HttpWebResponse)initSessionRequest.GetResponse();

                // Need to get the cookies from this response, add the switch cookie, then move on...
                aspNet = initSessionResponse.Cookies["ASP.NET_SessionId"];
            }


            // Generate a web request for the target url, set the cookies based on the response from the login request
            HttpWebRequest targetPost = (HttpWebRequest)WebRequest.Create(requestInfo.targetURL);
            targetPost.CookieContainer = Helpers.RetrieveCookiesFromResponse(requestInfo.application, loginResponse);

            if (requestInfo.application == Enums.Applications.PORTAL) {
                targetPost.CookieContainer.Add(aspNet);
            }

            if (requestInfo.targetRequestData != null) {
                byte[] byteArrayPostAttendance = Encoding.UTF8.GetBytes(requestInfo.targetRequestData);
                targetPost.ContentType = "application/x-www-form-urlencoded";
                targetPost.Method = "POST";
                targetPost.ContentLength = byteArrayPostAttendance.Length;

                using (Stream postDataStreamNew = targetPost.GetRequestStream()) {
                    postDataStreamNew.Write(byteArrayPostAttendance, 0, byteArrayPostAttendance.Length);
                }
            }

            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            // Get the response, stop the timer
            targetPost.GetResponse();
            timer.Stop();

            Console.WriteLine("End Transaction # {0} of {1} - {2} took {3}ms", currentTransaction, totalTransactions, requestInfo.targetURL, timer.ElapsedMilliseconds);
        }
    }

    public class Helpers {
        /// <summary>
        /// Formats the login url based on the specified application, church code and environment.
        /// </summary>
        /// <param name="application">The application under test.</param>
        /// <param name="churchCode">The church code targeted.</param>
        /// <param name="environment">The environment targeted.</param>
        /// <returns>A string url representing the entry point for the test.</returns>
        public static string FormatLoginURL(Enums.Applications application, string churchCode, Enums.Environments environment) {
            string targetURL = string.Empty;

            if (application == Enums.Applications.INFELLOWSHIP) {
                switch (environment) {
                    case Enums.Environments.DEV1:
                    case Enums.Environments.DEV2:
                    case Enums.Environments.DEV3:
                        targetURL = string.Format("http://{0}.infellowship.{1}.dev.corp.local/UserLogin/Attempt", churchCode, environment);
                        break;
                    case Enums.Environments.INTEGRATION:
                        targetURL = string.Format("http://{0}.infellowship.integration.corp.local/UserLogin/Attempt", churchCode);
                        break;
                    case Enums.Environments.DEV:
                    case Enums.Environments.QA:
                        targetURL = string.Format("http://{0}.infellowship{1}.dev.corp.local/UserLogin/Attempt", churchCode, environment);
                        break;
                    case Enums.Environments.STAGING:
                        targetURL = string.Format("https://{0}.staging.infellowship.com/UserLogin/Attempt", churchCode);
                        break;
                    default:
                        throw new Exception("Not a valid option!");
                }
            }
            else if (application == Enums.Applications.PORTAL) {
                switch (environment) {
                    case Enums.Environments.LOCAL:
                        targetURL = "http://portal.local/login.aspx/";
                        break;
                    case Enums.Environments.DEV1:
                    case Enums.Environments.DEV2:
                    case Enums.Environments.DEV3:
                        targetURL = string.Format("http://portal.{0}.dev.corp.local/login.aspx/", environment.ToString().ToLower());
                        break;
                    case Enums.Environments.INTEGRATION:
                        targetURL = "http://portal.integration.corp.local/login.aspx/";
                        break;
                    case Enums.Environments.DEV:
                    case Enums.Environments.QA:
                        targetURL = string.Format("http://portal{0}.dev.corp.local/login.aspx", environment);
                        break;
                    case Enums.Environments.STAGING:
                        targetURL = "https://staging-www.fellowshipone.com/login.aspx";
                        break;
                    default:
                        throw new Exception("Not a valid option!");
                }
            }
            else if (application == Enums.Applications.REPORTLIBRARY) {
                switch (environment) {
                    case Enums.Environments.LOCAL:
                        targetURL = "http://portal.local/login.aspx/";
                        break;
                    case Enums.Environments.DEV1:
                    case Enums.Environments.DEV2:
                    case Enums.Environments.DEV3:
                        targetURL = string.Format("http://portal.{0}.dev.corp.local/login.aspx/", environment.ToString().ToLower());
                        break;
                    case Enums.Environments.INTEGRATION:
                        targetURL = "http://portal.integration.corp.local/login.aspx/";
                        break;
                    case Enums.Environments.DEV:
                    case Enums.Environments.QA:
                        targetURL = string.Format("http://reportlibrary{0}.dev.corp.local/ReportLibrary/Login/Index.aspx", environment);
                        break;
                    case Enums.Environments.STAGING:
                        targetURL = "https://staging-reportlibrary.fellowshipone.com/ReportLibrary/Login/Index.aspx";
                        break;
                    default:
                        throw new Exception("Not a valid option!");
                }
            }
            else {
                throw new Exception("Application not available!");
            }
            return targetURL;
        }

        /// <summary>
        /// Stores the necessary cookies from a web response to be used in future requests.
        /// </summary>
        /// <param name="application">The application under test.</param>
        /// <param name="response">The web response to extract the cookies from.</param>
        /// <returns>A CookieContainer containing the needed cookies for subsequent requests.</returns>
        public static CookieContainer RetrieveCookiesFromResponse(Enums.Applications application, HttpWebResponse response) {
            CookieContainer cookies = new CookieContainer();

            if (application == Enums.Applications.PORTAL) {
                cookies.Add(response.Cookies["switch"]);
                cookies.Add(response.Cookies["uid"]);
                cookies.Add(response.Cookies["cid"]);
                cookies.Add(response.Cookies["userID"]);
                cookies.Add(response.Cookies["CIDpe"]);
            }
            else if (application == Enums.Applications.INFELLOWSHIP) {
                cookies.Add(response.Cookies["ASP.NET_SessionId"]);
                cookies.Add(response.Cookies[".ASPXAUTH"]);
            }
            else {  // Assumes else will cover report library
                cookies.Add(response.Cookies["ASP.NET_SessionId"]);
                cookies.Add(response.Cookies[".F1RL"]);
                cookies.Add(response.Cookies["storedusername"]);
                cookies.Add(response.Cookies["storedchurchcode"]);
                cookies.Add(response.Cookies["culture"]);
            }
            return cookies;
        }

        /// <summary>
        /// Generates the POST request data necessary for login based on the specified application.
        /// </summary>
        /// <param name="application">The target application.</param>
        /// <param name="creds">An XmlNode representing the credentials for the target application.</param>
        /// <param name="churchCode">The church code for the target.</param>
        /// <returns>A string representing the POST data necessary for login.</returns>
        public static string GenerateLoginRequestData(Enums.Applications application, XmlNode creds, string churchCode) {
            string postData = string.Empty;
            switch (application) {
                case Enums.Applications.PORTAL:
                    //postData = string.Format("__EVENTTARGET=&__EVENTARGUMENT=&__VIEWSTATE=%2FwEPDwULLTE4MTUzNjAzMTIPZBYCZg9kFgICAQ9kFgJmD2QWAgIHDw9kFgIeB29uY2xpY2sFL3Nob3dXYWl0QmVmb3JlU3VibWl0KCdjdGwwMF9jb250ZW50X2J0bkxvZ2luJyk7ZGQJWigDm%2Fbh8Gdphi7byq07OzFU%2BQ%3D%3D&ctl00%24content%24userNameText={0}&ctl00%24content%24passwordText={1}&ctl00%24content%24churchCodeText={2}&ctl00%24content%24btnLogin=Sign+in", creds.SelectSingleNode("//credentials/username").InnerText, creds.SelectSingleNode("//credentials/password").InnerText, churchCode);
                    postData = string.Format("ctl00%24content%24userNameText={0}&ctl00%24content%24passwordText={1}&ctl00%24content%24churchCodeText={2}&ctl00%24content%24btnLogin=Sign+in", creds.SelectSingleNode("//credentials/username").InnerText, creds.SelectSingleNode("//credentials/password").InnerText, churchCode);
                    break;
                case Enums.Applications.INFELLOWSHIP:
                    postData = string.Format("username={0}&password={1}&rememberme=false&btn_login=Sign+in", creds.SelectSingleNode("//credentials/username").InnerText, creds.SelectSingleNode("//credentials/password").InnerText);
                    break;
                case Enums.Applications.REPORTLIBRARY:
                    postData = string.Format("__Action=Attempt&username={0}&password={1}&churchcode={2}&btn_login=Login", creds.SelectSingleNode("//credentials/username").InnerText, creds.SelectSingleNode("//credentials/password").InnerText, churchCode);
                    break;
                default:
                    break;
            }
            return postData;
        }

        /// <summary>
        /// Generates the request data for the target url.
        /// </summary>
        /// <param name="postData">An XmlNodeList representing the data required to POST to the target url.</param>
        /// <returns>A string representing the POST data.</returns>
        public static string GenerateTargetRequestData(XmlNodeList postData) {
            StringBuilder postDataString = new StringBuilder();

            foreach (XmlNode item in postData) {
                postDataString.AppendFormat("{0}={1}&", item["name"].InnerText, item["value"].InnerText);
            }
            return postDataString.ToString().TrimEnd('&');
        }
    }

    public class Enums {
        public enum Environments {
            LOCAL,
            DEV,
            DEV1,
            DEV2,
            DEV3,
            INTEGRATION,
            QA,
            STAGING,
        }

        public enum Applications {
            PORTAL,
            INFELLOWSHIP,
            REPORTLIBRARY,
        }
    }
}