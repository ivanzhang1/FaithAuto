using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using System.IO;
using System.Configuration;
using System.Net;
using System.Xml.Linq;
using System.Web;

using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Firefox;

namespace FTTests {
    public class APIBase {
        private F1Environments _f1Environment;
        private ExeConfigurationFileMap _appConfigFileMap = new ExeConfigurationFileMap();
        private Configuration _configuration;
        private RemoteWebDriver _driver;
        private string _churchcode;
        private string _realm = string.Empty;

        private string _consumerKey;
        private string _consumerSecret;
        private string _username;
        private string _password;
        private string _accessToken;
        private string _accessTokenSecret;

        public string Realm {
            set { _realm = value; }
            get { return _realm; }
        }

        //public RemoteWebDriver Driver
        //{
        //    get { return _driver; }
        //}

        public APIBase() {
            // Configure and open the config file
            _appConfigFileMap.ExeConfigFilename = @"..\..\..\Common\bin\Debug\Common.dll.config";
            _configuration = ConfigurationManager.OpenMappedExeConfiguration(_appConfigFileMap, ConfigurationUserLevel.None);

            // Set Environment variables
            this._f1Environment = (F1Environments)Enum.Parse(typeof(F1Environments), _configuration.AppSettings.Settings["FTTests.Environment"].Value);
            this._churchcode = _configuration.AppSettings.Settings["FTTests.ChurchCode"].Value;

            this._consumerKey = _configuration.AppSettings.Settings["FTTests.APIConsumerKey"].Value;
            this._consumerSecret = _configuration.AppSettings.Settings["FTTests.APIConsumerSecret"].Value;
            this._username = _configuration.AppSettings.Settings["FTTests.APIUsername"].Value;
            this._password = _configuration.AppSettings.Settings["FTTests.APIPassword"].Value;
        }


        public APIBase(RemoteWebDriver driver)
        {
            // Configure and open the config file
            _appConfigFileMap.ExeConfigFilename = @"..\..\..\Common\bin\Debug\Common.dll.config";
            _configuration = ConfigurationManager.OpenMappedExeConfiguration(_appConfigFileMap, ConfigurationUserLevel.None);

            // Set Environment variables
            this._f1Environment = (F1Environments)Enum.Parse(typeof(F1Environments), _configuration.AppSettings.Settings["FTTests.Environment"].Value);
            this._churchcode = _configuration.AppSettings.Settings["FTTests.ChurchCode"].Value;

            this._driver = driver;

        }

        public XDocument MakeAPIRequest(string method, string resourceURL) {
            var response = this.MakeAPIRequest(method, "application/xml", resourceURL);
            XDocument xDoc = new XDocument();
            System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream());
            if (response.ContentLength > 0) {
                xDoc = XDocument.Load(sr);
            }

            response.Close();
            return xDoc;
        }


        public WebResponse MakeWebRequest(string resourceURL)
        {

            TestLog.WriteLine("Web Request: " + resourceURL);            
            WebRequest request = (HttpWebRequest)HttpWebRequest.Create(resourceURL);
            
            //Get response no matter what
            try
            {
                return request.GetResponse();
            }
            catch (WebException e)
            {
                return e.Response;
            }


            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                                  
            return response;

        }


        public HttpWebResponse MakeAPIRequest(string method, string contentType, string resourceURL) {

            string url = String.Format("{0}{1}", this.GetAPIURL(), resourceURL);
            TestLog.WriteLine("API Request: " + url);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            request.Method = method;
            request.ContentType = contentType;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //response.Close();
            return response;
        }

        private HttpWebResponse MakeAPIRequestOAuth(string method, string oAuthHeader, string url, string jsonBody = "")
        {
            TestLog.WriteLine("Build Request URL: " + url);
            TestLog.WriteLine("Build Request OAuthHeader: " + oAuthHeader);
            TestLog.WriteLine("Build Request method: " + method);
            TestLog.WriteLine("Build Request json body: " + jsonBody);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            request.Method = method;
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", oAuthHeader);

            if (!String.IsNullOrEmpty(jsonBody))
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(jsonBody);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //response.Close();
            return response;
        }

        public string GetAPIURL(string churchId) {
            string returnValue = string.Empty;

            // If the realm is not empty, append a slash
            if (!string.IsNullOrEmpty(this._realm)) {
                this._realm = this._realm + "/";
            }

            switch (this._f1Environment) {
                case F1Environments.LOCAL:
                    break;
               /* case F1Environments.DEV:
                    returnValue = string.Format("http://{0}.api.dev.corp.local/{1}{2}/", this._churchcode, this._realm, "v1");
                    break;
                case F1Environments.QA:
                    returnValue = string.Format("http://{0}.apiqa.dev.corp.local/{1}{2}/", this._churchcode, this._realm, "v1");
                    break;
                case F1Environments.DEV1:
                case F1Environments.DEV2:
                case F1Environments.DEV3:
                    break;
                case F1Environments.INTEGRATION:
                    break;
                case F1Environments.STAGING:
                    returnValue = string.Format("https://{0}.staging.fellowshiponeapi.com/{1}{2}/", this._churchcode, this._realm, "v1");
                    break; */

                case F1Environments.LV_QA:
                case F1Environments.LV_UAT:
                case F1Environments.STAGING:
                case F1Environments.INT:
                case F1Environments.INT2:
                case F1Environments.INT3:
                case F1Environments.LV_PROD:
                    //v1/people or people/v1 or peoplev1
                    returnValue = string.Format("https://{0}.{1}.fellowshiponeapi.com/{2}{3}/", churchId, PortalBase.GetLVEnvironment(this._f1Environment), this._realm, "v1");
                    break;
                default:
                    throw new Exception("Not a valid environment!!");
            }

            TestLog.WriteLine("URL: " + returnValue);
            return returnValue;
        }

        public string GetAPIURL()
        {
            return GetAPIURL(this._churchcode);
        }

        public String GetResponseString(HttpWebResponse response)
        {
                    // we will read data via the response stream
            Stream resStream = response.GetResponseStream();

            string tempString = null;
            int count = 0;
            // used to build entire input
            StringBuilder sb = new StringBuilder();

            // used on each read operation
            byte[] buf = new byte[8192];
            do
            {
                // fill the buffer with data
                count = resStream.Read(buf, 0, buf.Length);

                // make sure we read some data
                if (count != 0)
                {
                    // translate from bytes to ASCII text
                    tempString = Encoding.ASCII.GetString(buf, 0, count);

                    // continue building the string
                    sb.Append(tempString);
                }
            }
            while (count > 0); // any more data to read?

            return sb.ToString();
       }

        private string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private string GenerateOAuthHeader(string uri, bool needRealm, string method)
        {
            if (needRealm) 
            { 
                Login2ndParty();
            }

            OAuthBase oAuth = new OAuthBase();
            string nonce = oAuth.GenerateNonce();
            string timeStamp = oAuth.GenerateTimeStamp();
            string normalizedUrl;
            string normalizedRequestParameters;
            string sig;
            StringBuilder authHeader = new StringBuilder("OAuth ");

            if(!needRealm) {
                sig = oAuth.GenerateSignature(new Uri(uri), this._consumerKey, this._consumerSecret,
                    string.Empty, string.Empty, method,
                    timeStamp, nonce, OAuthBase.SignatureTypes.HMACSHA1,
                    out normalizedUrl, out normalizedRequestParameters);

                authHeader.Append("oauth_version=\"1.0\", ");
                authHeader.Append("oauth_signature_method=\"HMAC-SHA1\", ");
                authHeader.Append("oauth_nonce=\"" + nonce + "\", ");
                authHeader.Append("oauth_timestamp=\"" + timeStamp + "\", ");
                authHeader.Append("oauth_consumer_key=\"" + this._consumerKey + "\", ");
                authHeader.Append("oauth_signature=\"" + HttpUtility.UrlEncode(sig) + "\"");
            } else {
                string realm = uri;
                if(uri.Contains("?")) {
                    realm = uri.Substring(0, uri.IndexOf("?"));
                }
                sig = oAuth.GenerateSignature(new Uri(uri), this._consumerKey, this._consumerSecret,
                    _accessToken, _accessTokenSecret, method,
                    timeStamp, nonce, OAuthBase.SignatureTypes.HMACSHA1,
                    out normalizedUrl, out normalizedRequestParameters);

                authHeader.Append("realm=\"" + HttpUtility.UrlEncode(realm) + "\", ");
                authHeader.Append("oauth_version=\"1.0\", ");
                authHeader.Append("oauth_signature_method=\"HMAC-SHA1\", ");
                authHeader.Append("oauth_nonce=\"" + nonce + "\", ");
                authHeader.Append("oauth_timestamp=\"" + timeStamp + "\", ");
                authHeader.Append("oauth_consumer_key=\"" + this._consumerKey + "\", ");
                authHeader.Append("oauth_token=\"" + this._accessToken + "\", ");
                authHeader.Append("oauth_signature=\"" + HttpUtility.UrlEncode(sig) + "\"");
            }

            return authHeader.ToString();

        }

        private void SetToken(string httpResponse)
        {
            string[] response = httpResponse.Split('&');
            if(response.Length < 2) {
                throw new Exception("Fail to find token value from given response: " + httpResponse);
            }
            this._accessToken = response[0].Replace("oauth_token=", "");
            this._accessTokenSecret = response[1].Replace("oauth_token_secret=", "");
        }

        private void Login2ndParty()
        {
            string logInURL = string.Format("{0}PortalUser/AccessToken?ec={1}", this.GetAPIURL(), Base64Encode(this._username + " " + this._password));
            TestLog.WriteLine("Generate token request URL: " + logInURL);

            string tokenResponse = (GetResponseString(MakeAPIRequestOAuth("GET", GenerateOAuthHeader(logInURL, false, "GET"), logInURL)));
            TestLog.WriteLine("Get token from server: " + tokenResponse);
            SetToken(tokenResponse);
        }

        public string SetValueByStrKey(string orgString, string key, string valueToSet)
        {
            key = string.Format("\"{0}\"", key);
            string[] splits = orgString.Split(new string[] {key}, StringSplitOptions.None);
            string tempString = splits[0] + key + ":\"" + valueToSet + "\"" + splits[1].Substring(splits[1].IndexOf(","));

            for (int i = 2; i < splits.Length; i++)
            {
                tempString = tempString + key + splits[i];
            }

            return tempString;
        }

        public string SetValueByStrKey(string orgString, string preKey, string key, string valueToSet)
        {
            preKey = string.Format("\"{0}\"", preKey);
            string[] splits = orgString.Split(new string[] { preKey }, StringSplitOptions.None);
            string tempString = "";
            for (int i = 1; i < splits.Length; i++)
            {
                tempString = tempString + splits[i];
            }

            return splits[0] + preKey + SetValueByStrKey(tempString, key, valueToSet);
        }

        public string GetValueByStrKey(string orgString, string key)
        {
            key = string.Format("\"{0}\":\"", key);
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

        public string GetFirstValueByStrKey(string responseString, string key)
        {
            return responseString.Substring(responseString.IndexOf("\"" + key + "\":\"")).Split('\"')[3].Replace("\"", "").Trim();
        }

        public string SendAPIRequest(string uri, string httpMethod, string json, HttpStatusCode expectCode)
        {
            HttpWebResponse response = MakeAPIRequestOAuth(httpMethod, GenerateOAuthHeader(uri, true, httpMethod), uri, json);
            Assert.AreEqual(expectCode, response.StatusCode);
            Assert.LessThanOrEqualTo(0, response.ContentLength);

            string responseTemp = this.GetResponseString(response);
            TestLog.WriteLine("Response: " + responseTemp);

            return responseTemp;
        }

        public string SendAPIRequestwithBodyNoAuth(string url, string httpMethod, string json, HttpStatusCode expectCode)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = httpMethod;
            request.ContentType = "application/json";  
           
            byte[] btBodys = Encoding.UTF8.GetBytes(json);
            request.ContentLength = btBodys.Length;
            request.GetRequestStream().Write(btBodys, 0, btBodys.Length);

            TestLog.WriteLine("-request = {0}", request);
            string responseTemp = string.Empty;
            try
            {
                HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
                Assert.AreEqual(expectCode, httpWebResponse.StatusCode);
                if (expectCode.Equals(HttpStatusCode.NoContent))
                {
                    Assert.LessThanOrEqualTo(-1, httpWebResponse.ContentLength);
                }
                else
                {
                    Assert.LessThanOrEqualTo(0, httpWebResponse.ContentLength);
                }
                long resContLen = httpWebResponse.ContentLength;
                TestLog.WriteLine("-resContLen = {0}", resContLen);
                responseTemp = this.GetResponseString(httpWebResponse);
                return responseTemp;
            }
            catch (Exception e)
            {
                responseTemp = "ERROR";
                e.ToString();
            }
            
            return responseTemp;
            
            /*
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();
            httpWebResponse.Close();
            streamReader.Close();
            request.Abort();
            httpWebResponse.Close();
            return responseContent;
             * */
        }
        public string SendAPIRequestNoAuth(string url, string httpMethod, HttpStatusCode expectCode)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = httpMethod;
            TestLog.WriteLine("-request = {0}", request);

            HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();

            Assert.AreEqual(expectCode, httpWebResponse.StatusCode);
            string responseTemp = this.GetResponseString(httpWebResponse);
            return responseTemp;
            /*
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();
            httpWebResponse.Close();
            streamReader.Close();
            request.Abort();
            httpWebResponse.Close();
            return responseContent;
             * */
        }

        public string GenerateCheckInJson(String activityName, String activeId, String individualId, String instanceId, String rosterId, String individualTypeId, String individualTypeName, String churchCode, String enviroment)
        {
            String uri = "https://" + churchCode + "." + enviroment + ".fellowshiponeapi.com/activities/v1/attendances/85489467?mode=demo";
            String idUri = "\"url\": \"\",";
            String personidAppend = "\"id\": " + individualId + ",";
            String personUriAppend = " \"uri\": \"https://" + churchCode + "." + enviroment + ".fellowshiponeapi.com/v1/people/" + individualId + "\"";
            String activeidAppend = " \"id\": " + activeId + ",";
            String activenameAppend = " \"name\": \"" + activityName + "\",";
            String activeUriAppend = " \"uri\": \"https://" + churchCode + "." + enviroment + ".fellowshiponeapi.com/activities/v1/activities/" + activeId + "\"";
            String instanceidAppend = " \"id\": " + instanceId + ",";
            String instanceUriAppend = "\"uri\": \"https://" + churchCode + "." + enviroment + ".fellowshiponeapi.com/activities/v1/instances/" + instanceId + "\"";
            String rosteridAppend = "\"id\": " + rosterId + ",";
            String rosterUriAppend = "\"uri\": \"https://" + churchCode + "." + enviroment + ".fellowshiponeapi.com/activities/v1/rosters/" + rosterId + "\"";
            String typenidAppend = ("\"id\": " + individualTypeId + ",");
            String typenameAppend = "\"name\": \"" + individualTypeName + "\",";
            String typeUriAppend = " \"uri\": \"https://" + churchCode + "." + enviroment + ".fellowshiponeapi.com/activities/v1/types/" + individualTypeId + "\"";

            StringBuilder contentTypeBufer = new StringBuilder("");
            contentTypeBufer.Append("{");
            contentTypeBufer.Append("\"id\": null,");
            contentTypeBufer.Append(idUri);
            contentTypeBufer.Append("\"person\": {");
            contentTypeBufer.Append(personidAppend);
            contentTypeBufer.Append(personUriAppend);
            contentTypeBufer.Append(" },");
            contentTypeBufer.Append(" \"activity\": {");
            contentTypeBufer.Append(activenameAppend);
            contentTypeBufer.Append(activeidAppend);
            contentTypeBufer.Append(activeUriAppend);
            contentTypeBufer.Append("},");
            contentTypeBufer.Append("\"instance\": {");
            contentTypeBufer.Append(instanceidAppend);
            contentTypeBufer.Append(instanceUriAppend);
            contentTypeBufer.Append("},");
            contentTypeBufer.Append("\"roster\": {");
            contentTypeBufer.Append(rosteridAppend);
            contentTypeBufer.Append(rosterUriAppend);
            contentTypeBufer.Append("},");
            contentTypeBufer.Append("\"type\": {");
            contentTypeBufer.Append(typenameAppend);
            contentTypeBufer.Append(typenidAppend);
            contentTypeBufer.Append(typeUriAppend);
            contentTypeBufer.Append("},");
            contentTypeBufer.Append("\"checkin\": null,");
            contentTypeBufer.Append("\"checkout\": null");
            contentTypeBufer.Append("}");

            string json = contentTypeBufer.ToString();
            return json;

        }
        //grace zhang:add for RealtimeNoticeJson
        public string GenerateRealtimeNoticeJson(String individualInstanceId, String activityInstanceId, String activityDetailId, String individualId, 
            String oldActivityInstanceId, String oldActivityDetailId, String OperType, String individualType, String churchId)
        {
            if (oldActivityInstanceId == null)
            {
                oldActivityInstanceId ="null";
            }
            if (oldActivityDetailId == null)
            {
                oldActivityDetailId = "null";
            }
          
          
            StringBuilder contentTypeBufer = new StringBuilder("");
            contentTypeBufer.Append("[{");
            contentTypeBufer.Append("\"IndividualInstanceId\": " + individualInstanceId+",");
            contentTypeBufer.Append("\"ActivityInstanceId\": " + activityInstanceId + ",");
            contentTypeBufer.Append("\"ActivityDetailId\": " + activityDetailId + ",");
            contentTypeBufer.Append("\"IndividualId\": " + individualId + ",");
            contentTypeBufer.Append("\"OldActivityInstanceId\": " + oldActivityInstanceId + ",");
            contentTypeBufer.Append("\"OldActivityDetailId\": " + oldActivityDetailId + ",");
            contentTypeBufer.Append("\"OperType\": " + OperType + ",");
            contentTypeBufer.Append("\"IndividualType\": " + individualType + ",");
            contentTypeBufer.Append("\"ChurchId\": " + churchId);           
            contentTypeBufer.Append("}]");

            string json = contentTypeBufer.ToString();
            TestLog.WriteLine("-RealTimeJson = {0}", json);
            return json;

        }
        
        public string url { get; set; }
    }
}