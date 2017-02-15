using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using System.Xml;
using System.Net;
using System.Web;

using System.Linq;

using System.Xml.Linq;

namespace FTTests.DataExchange {
    [TestFixture]
    public class DataExchange : FixtureBaseWebDriver {
        private string _churchCodeValue = "alyq5rXA9igtXilwXAT+3Q==";
        private string _userValue = "msneeden";
        private string _passwordValue = "Pa$$w0rd";
        private string _individualCode = string.Empty;
        private string _householdCode = string.Empty;

        private string _soapHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\"><soap12:Body><Request xmlns=\"http://www.fellowshipone.com/\"><sXML>";
        private string _soapHeaderEnd = "</sXML></Request></soap12:Body></soap12:Envelope>";

        private enum Methods {
            GetIndividual,
            GetHousehold,
            GetIndividualCollection,
            GetHouseholdCollection,
            UpdateIndividual,
            UpdateHousehold,
            UpdateIndividualCollection,
            UpdateHouseholdCollection,
            IsConverted
        }

        /// <summary>
        /// Constructs the authorization header for the request.
        /// </summary>
        /// <param name="methodValue">The method for the request</param>
        /// <returns>String representation of the constructed authorization header</returns>
        private string BuildAuthHeader(Methods methodValue) {
            StringBuilder authHeader = new StringBuilder();

            var version = "2.0";
            var methodGroup = "People";

            if (methodValue == Methods.IsConverted) {
                version = "1.0";
                methodGroup = string.Empty;
            }

            authHeader.AppendFormat("<tns:dataRequest xmlns:tns=\"{0}\">", methodValue);
            authHeader.Append("<authenticateHeader>");
            authHeader.AppendFormat("<churchCode>{0}</churchCode>", _churchCodeValue);
            authHeader.AppendFormat("<user>{0}</user>", _userValue);
            authHeader.AppendFormat("<password>{0}</password>", _passwordValue);
            authHeader.AppendFormat("<method>{0}</method>", methodValue);
            authHeader.AppendFormat("<version>{0}</version>", version);
            if (methodValue != Methods.IsConverted) {
                authHeader.Append("<methodGroup>People</methodGroup>");
            }        
            authHeader.Append("</authenticateHeader>");

            // If UpdateIndividual or UpdateHousehold, use snychronous
            if (methodValue == Methods.UpdateIndividual || methodValue == Methods.UpdateHousehold) {
                authHeader.Append("<configuration>");
                authHeader.Append("<requestProcessType>Synchronous</requestProcessType>");
                authHeader.Append("</configuration>");
            }

            return authHeader.ToString();
        }

        /// <summary>
        /// Makes a request against data exchange.
        /// </summary>
        /// <param name="method">The method for the request</param>
        /// <param name="sXML">The parameters of the request</param>
        /// <returns>An XElement representing the data portion of the response from data exchange</returns>
        private XElement MakeDERequest(Methods method, StringBuilder sXML) {
            StringBuilder request = new StringBuilder();
            request.Append(_soapHeader);
            request.Append(HttpUtility.HtmlEncode(BuildAuthHeader(method)));
            request.Append(HttpUtility.HtmlEncode(sXML));
            request.Append(HttpUtility.HtmlEncode("</tns:dataRequest>"));
            request.Append(_soapHeaderEnd);

            byte[] reqBytes = System.Text.UTF8Encoding.UTF8.GetBytes(request.ToString());

            string returnValue = string.Empty;
            switch (base.F1Environment) {
                case F1Environments.LOCAL:
                    break;
                //case F1Environments.DEV:
                case F1Environments.QA:
                    returnValue = string.Format("http://services2{0}.dev.corp.local/DataExchange/v1/DataRequest.asmx", base.F1Environment);
                    break;
                    /*
                case F1Environments.DEV1:
                case F1Environments.DEV2:
                case F1Environments.DEV3:
                    break;
                case F1Environments.INTEGRATION:
                    break; */
                case F1Environments.STAGING:
                    returnValue = "https://staging-services.fellowshipone.com/DataExchange/v1/DataRequest.asmx?op=Request";
                    break;
                default:
                    throw new Selenium.SeleniumException("Not a valid environment!!");
            }

            WebRequest webRequest = WebRequest.Create(returnValue);
            webRequest.Method = "POST";
            webRequest.ContentType = "text/xml";
            webRequest.ContentLength = reqBytes.Length;
            System.IO.Stream reqStream = webRequest.GetRequestStream();
            reqStream.Write(reqBytes, 0, reqBytes.Length);

            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

            XDocument xDoc = new XDocument();
            System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream());
            if (response.ContentLength > 0) {
                xDoc = XDocument.Load(sr);
            }

            string descendants = string.Empty;
            switch (method) {
                case Methods.GetIndividual:
                case Methods.GetHousehold:
                case Methods.GetIndividualCollection:
                case Methods.GetHouseholdCollection:
                case Methods.IsConverted:
                    descendants = "data";
                    break;
                case Methods.UpdateIndividual:
                case Methods.UpdateHousehold:
                    descendants = "dataResults";
                    break;
                case Methods.UpdateIndividualCollection:
                case Methods.UpdateHouseholdCollection:
                    descendants = "message";
                    break;
                default:
                    throw new Exception("Not a valid method!!");
            }
            return xDoc.Descendants(descendants).FirstOrDefault();
        }

        [FixtureSetUp]
        public void FixtureSetUp() {
            // Build the request data
            StringBuilder sXML = new StringBuilder();
            sXML.Append("<parameters>");
            sXML.Append("<weblink>");
            sXML.Append("<userId>msneeden@fellowshiptech.com</userId>");
            sXML.Append("<password>Pa$$w0rd</password>");
            sXML.Append("</weblink>");
            sXML.Append("</parameters>");

            // Make the request
            XElement responseData = MakeDERequest(Methods.GetIndividual, sXML);

            // Retrieve and store the individualCode for use in tests
            _individualCode = responseData.Descendants("individual").FirstOrDefault().Attribute("individualCode").Value;
            _householdCode = responseData.Descendants("individual").FirstOrDefault().Attribute("householdCode").Value;
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the number of households returned when querying on a household communication value.")]
        public void GetHouseholdCollection_Communication()
        {
            // Get the comparison data from the database
            TestLog.WriteLine("Get expected count");
            string communicationValue = "6302172170";
            int expectedCount = Convert.ToInt16(base.SQL.Execute(string.Format("SELECT COUNT(*) FROM ChmPeople.dbo.HOUSEHOLD_COMMUNICATION WITH (NOLOCK) WHERE CHURCH_ID = 15 AND COMMUNICATION_TYPE_ID = 1 AND COMMUNICATION_VALUE = '{0}'", communicationValue)).Rows[0][0].ToString());

            // Build the request data
            TestLog.WriteLine("Build Request Data");
            StringBuilder sXML = new StringBuilder();
            sXML.Append("<parameters>");
            sXML.Append("<communication>");
            sXML.Append("<communicationType>Home Phone</communicationType>");
            sXML.AppendFormat("<communicationValue>{0}</communicationValue>", communicationValue);
            sXML.Append("<lastUpdatedDate></lastUpdatedDate>");
            sXML.Append("</communication>");
            sXML.Append("</parameters>");

            // Make the request
            TestLog.WriteLine("SendAndWait Response ...");
            XElement responseData = MakeDERequest(Methods.GetHouseholdCollection, sXML);


            // Verify data points
            TestLog.WriteLine("Verify Response ... [" + expectedCount + "]");            
            try{
                Int16 actualCount = Int16.MinValue;
                Int16.TryParse(responseData.Attribute("exportedCount").Value, out actualCount);
                Assert.AreEqual(expectedCount, actualCount);
            }catch(NullReferenceException)
            {
                TestLog.WriteLine("Null Response Data");
                Assert.AreEqual(expectedCount, 0);
            }

        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the number of households returned when querying on a household communication value.")]
        public void GetHouseholdCollection_HouseholdName() {
            // Get the comparison data from the database
            int expectedCount = Convert.ToInt16(base.SQL.Execute("SELECT COUNT(*) FROM ChmPeople.dbo.HOUSEHOLD WITH (NOLOCK) WHERE CHURCH_ID = 15 AND HOUSEHOLD_NAME = 'Matthew Sneeden and Laura Hause'").Rows[0][0].ToString());

            // Build the request data
            StringBuilder sXML = new StringBuilder();
            sXML.Append("<parameters>");
            sXML.Append("<householdName>Matthew Sneeden and Laura Hause</householdName>");
            sXML.Append("</parameters>");

            // Make the request
            XElement responseData = MakeDERequest(Methods.GetHouseholdCollection, sXML);

            // Verify data points
            Int16 actualCount = Int16.MinValue;
            Int16.TryParse(responseData.Attribute("exportedCount").Value, out actualCount);
            Assert.AreEqual(expectedCount, actualCount);
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the GetIndividual method.")]
        public void GetIndividual() {
            // Build the request data
            StringBuilder sXML = new StringBuilder();
            sXML.Append("<parameters>");
            sXML.Append("<weblink>");
            sXML.Append("<userId>msneeden@fellowshiptech.com</userId>");
            sXML.Append("<password>Pa$$w0rd</password>");
            sXML.Append("</weblink>");
            sXML.Append("</parameters>");

            // Make the request
            XElement responseData = MakeDERequest(Methods.GetIndividual, sXML);

            // Verify data points
            Assert.AreEqual("Matthew", responseData.Element("individual").Element("firstName").Value);
            Assert.AreEqual("Sneeden", responseData.Element("individual").Element("lastName").Value);
        }

        #region Update Individual
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user can update the marital status with a valid type.")]
        public void UpdateIndividual_MaritalStatus_Valid() {
            // Build the request data
            StringBuilder sXML = new StringBuilder();
            sXML.Append("<parameters>");
            sXML.Append("<weblink>");
            sXML.Append("<userId>msneeden@fellowshiptech.com</userId>");
            sXML.Append("<password>Pa$$w0rd</password>");
            sXML.Append("</weblink>");
            sXML.Append("</parameters>");

            // Make a request to get an individual
            XElement responseData = MakeDERequest(Methods.GetIndividual, sXML);

            // Update the marital status
            responseData.Descendants("individual").FirstOrDefault().SetAttributeValue("entityState", "Modified");
            responseData.Descendants("maritalStatus").FirstOrDefault().SetValue("Single");

            // Make the request
            StringBuilder sXML2 = new StringBuilder(responseData.ToString());
            XElement secondResponseData = MakeDERequest(Methods.UpdateIndividual, sXML2);
            Assert.AreEqual("1009::PI::The individual was found - update process will continue", secondResponseData.Value);

            // Verify the marital status
            responseData = MakeDERequest(Methods.GetIndividual, sXML);
            Assert.AreEqual("Single", responseData.Descendants("maritalStatus").FirstOrDefault().Value);
        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user can update the marital status with a null value.")]
        public void UpdateIndividual_MaritalStatus_Null() {
            // Build the request data
            StringBuilder sXML = new StringBuilder();
            sXML.Append("<parameters>");
            sXML.Append("<weblink>");
            sXML.Append("<userId>msneeden@fellowshiptech.com</userId>");
            sXML.Append("<password>Pa$$w0rd</password>");
            sXML.Append("</weblink>");
            sXML.Append("</parameters>");

            // Make a request to get an individual
            XElement responseData = MakeDERequest(Methods.GetIndividual, sXML);

            // Update the marital status
            responseData.Descendants("individual").FirstOrDefault().SetAttributeValue("entityState", "Modified");
            responseData.Descendants("maritalStatus").FirstOrDefault().SetValue(string.Empty);

            // Make the request
            StringBuilder sXML2 = new StringBuilder(responseData.ToString());
            XElement secondResponseData = MakeDERequest(Methods.UpdateIndividual, sXML2);
            Assert.AreEqual("1009::PI::The individual was found - update process will continue", secondResponseData.Value);

            // Verify the marital status
            responseData = MakeDERequest(Methods.GetIndividual, sXML);
            Assert.AreEqual(string.Empty, responseData.Descendants("maritalStatus").FirstOrDefault().Value);
        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user cannot update the marital status with an invalid value.")]
        public void UpdateIndividual_MaritalStatus_Invalid() {
            // Build the request data
            StringBuilder sXML = new StringBuilder();
            sXML.Append("<parameters>");
            sXML.Append("<weblink>");
            sXML.Append("<userId>msneeden@fellowshiptech.com</userId>");
            sXML.Append("<password>Pa$$w0rd</password>");
            sXML.Append("</weblink>");
            sXML.Append("</parameters>");

            // Make a request to get an individual
            XElement responseData = MakeDERequest(Methods.GetIndividual, sXML);

            // Update the marital status
            responseData.Descendants("individual").FirstOrDefault().SetAttributeValue("entityState", "Modified");
            responseData.Descendants("maritalStatus").FirstOrDefault().SetValue("Divorced");

            // Make the request
            StringBuilder sXML2 = new StringBuilder(responseData.ToString());
            XElement secondResponseData = MakeDERequest(Methods.UpdateIndividual, sXML2);
            Assert.AreEqual("False", secondResponseData.Descendants("Success").FirstOrDefault().Value);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the user can update an individual code, synchronously, with a new value.")]
        public void UpdateIndividual_IndividualCode_Valid() {
            // Build the request data
            StringBuilder sXML = new StringBuilder();
            sXML.Append("<parameters>");
            sXML.Append("<weblink>");
            sXML.Append("<userId>msneeden@fellowshiptech.com</userId>");
            sXML.Append("<password>Pa$$w0rd</password>");
            sXML.Append("</weblink>");
            sXML.Append("</parameters>");

            // Make a request to get an individual
            XElement responseData = MakeDERequest(Methods.GetIndividual, sXML);

            // Update the individual code
            responseData.Descendants("individual").FirstOrDefault().SetAttributeValue("entityState", "Modified");
            responseData.Descendants("individual").FirstOrDefault().SetAttributeValue("individualSourceCode", "1a2b3c4d");

            // Make the request
            StringBuilder sXML2 = new StringBuilder(responseData.ToString());
            XElement secondResponseData = MakeDERequest(Methods.UpdateIndividual, sXML2);
            Assert.IsTrue(secondResponseData.Value.Contains("1009::PI::The individual was found - update process will continue"));

            // Verify the marital status
            responseData = MakeDERequest(Methods.GetIndividual, sXML);
            Assert.AreEqual("1a2b3c4d", responseData.Descendants("individual").FirstOrDefault().Attribute("individualSourceCode").Value);
        }

        #endregion Update Individual

        #region Update Individual Collection
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user can update the marital status with a valid value.")]
            public void UpdateIndividualCollection_MaritalStatus_Valid() {
            // Build the request data
            StringBuilder sXML = new StringBuilder();
            sXML.Append("<parameters>");
            sXML.AppendFormat("<individualCode>{0}</individualCode>", _individualCode);
            sXML.Append("</parameters>");

            // Make a request to get an individual
            XElement responseData = MakeDERequest(Methods.GetIndividualCollection, sXML);

            // Update the marital status
            responseData.Descendants("individual").FirstOrDefault().SetAttributeValue("entityState", "Modified");
            responseData.Descendants("maritalStatus").FirstOrDefault().SetValue("Single");
            responseData.Element("individuals").Parent.RemoveAttributes();

            // Make the request
            StringBuilder sXML2 = new StringBuilder(responseData.ToString());
            XElement secondResponseData = MakeDERequest(Methods.UpdateIndividualCollection, sXML2);
            Assert.AreEqual("True", secondResponseData.Descendants("Success").FirstOrDefault().Value);

            // Verify the marital status
            responseData = MakeDERequest(Methods.GetIndividual, sXML);
            Assert.AreEqual("Single", responseData.Descendants("maritalStatus").FirstOrDefault().Value);
        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user cannot update the marital status with an invalid value.")]
        public void UpdateIndividualCollection_MaritalStatus_Invalid() {
            // Build the request data
            StringBuilder sXML = new StringBuilder();
            sXML.Append("<parameters>");
            sXML.AppendFormat("<individualCode>{0}</individualCode>", _individualCode);
            sXML.Append("</parameters>");

            // Make a request to get an individual
            XElement responseData = MakeDERequest(Methods.GetIndividualCollection, sXML);

            // Update the marital status
            responseData.Descendants("individual").FirstOrDefault().SetAttributeValue("entityState", "Modified");
            responseData.Descendants("maritalStatus").FirstOrDefault().SetValue("Divorced");
            responseData.Element("individuals").Parent.RemoveAttributes();

            // Make the request
            StringBuilder sXML2 = new StringBuilder(responseData.ToString());
            XElement secondResponseData = MakeDERequest(Methods.UpdateIndividualCollection, sXML2);
            Assert.AreEqual("False", secondResponseData.Descendants("Success").FirstOrDefault().Value);
        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user can update the marital status with a null value.")]
        public void UpdateIndividualCollection_MaritalStatus_Null() {
            // Build the request data
            StringBuilder sXML = new StringBuilder();
            sXML.Append("<parameters>");
            sXML.AppendFormat("<individualCode>{0}</individualCode>", _individualCode);
            sXML.Append("</parameters>");

            // Make a request to get an individual
            XElement responseData = MakeDERequest(Methods.GetIndividualCollection, sXML);

            // Update the marital status
            responseData.Descendants("individual").FirstOrDefault().SetAttributeValue("entityState", "Modified");
            responseData.Descendants("maritalStatus").FirstOrDefault().SetValue(string.Empty);
            responseData.Element("individuals").Parent.RemoveAttributes();

            // Make the request
            StringBuilder sXML2 = new StringBuilder(responseData.ToString());
            XElement secondResponseData = MakeDERequest(Methods.UpdateIndividualCollection, sXML2);
            Assert.AreEqual("True", secondResponseData.Descendants("Success").FirstOrDefault().Value);

            // Verify the marital status
            responseData = MakeDERequest(Methods.GetIndividual, sXML);
            Assert.AreEqual(string.Empty, responseData.Descendants("maritalStatus").FirstOrDefault().Value);
        }


        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the user can update the status comment with a valid value.")]
        public void UpdateIndividualCollection_StatusComment_Valid() {
            // Build the request data
            StringBuilder sXML = new StringBuilder();
            sXML.Append("<parameters>");
            sXML.AppendFormat("<individualCode>{0}</individualCode>", _individualCode);
            sXML.Append("</parameters>");

            // Make a request to get an individual
            XElement responseData = MakeDERequest(Methods.GetIndividualCollection, sXML);

            // Update the status comment
            var guid = Guid.NewGuid().ToString();
            responseData.Descendants("individual").FirstOrDefault().SetAttributeValue("entityState", "Modified");
            responseData.Descendants("statusComment").FirstOrDefault().SetValue(guid);
            responseData.Element("individuals").Parent.RemoveAttributes();

            // Make the request
            StringBuilder sXML2 = new StringBuilder(responseData.ToString());
            XElement secondResponseData = MakeDERequest(Methods.UpdateIndividualCollection, sXML2);
            Assert.AreEqual("True", secondResponseData.Descendants("Success").FirstOrDefault().Value);


            // Verify the comment.  Asynch request.
            Retry.WithPolling(100).WithTimeout(30000).WithFailureMessage("Async request was never processed!!")
                .Until(() => guid == MakeDERequest(Methods.GetIndividual, sXML).Descendants("statusComment").FirstOrDefault().Value);
  
            responseData = MakeDERequest(Methods.GetIndividual, sXML);
            Assert.AreEqual(guid, MakeDERequest(Methods.GetIndividual, sXML).Descendants("statusComment").FirstOrDefault().Value);
        }

        #endregion Update Individual Collection

        #region Update Household
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user can update the marital status with a valid type.")]
        public void UpdateHousehold_MaritalStatus_Valid() {
            // Build the request data
            StringBuilder sXML = new StringBuilder();
            sXML.Append("<parameters>");
            sXML.AppendFormat("<householdCode>{0}</householdCode>", _householdCode);
            sXML.Append("</parameters>");

            // Make the request to get the household
            XElement responseData = MakeDERequest(Methods.GetHousehold, sXML);

            // Update the marital status
            responseData.Element("household").SetAttributeValue("entityState", "Modified");
            responseData.Element("household").Element("individuals").Elements("individual").Attributes("individualCode").Single(individualCode => individualCode.Value == _individualCode).Parent.SetAttributeValue("entityState", "Modified");
            responseData.Element("household").Element("individuals").Elements("individual").Attributes("individualCode").Single(individualCode => individualCode.Value == _individualCode).Parent.Descendants("maritalStatus").FirstOrDefault().SetValue("Single");

            // Make the request
            StringBuilder sXML2 = new StringBuilder(responseData.ToString());
            XElement secondResponseData = MakeDERequest(Methods.UpdateHousehold, sXML2);
            Assert.AreEqual("1009::PI::The individual was found - update process will continue", secondResponseData.Value);

            // Verify the marital status
            responseData = MakeDERequest(Methods.GetHousehold, sXML);
            Assert.AreEqual("Single", responseData.Element("household").Element("individuals").Elements("individual").Attributes("individualCode").Single(individualCode => individualCode.Value == _individualCode).Parent.Descendants("maritalStatus").FirstOrDefault().Value);
        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user cannot update the marital status with an invalid type.")]
        public void UpdateHousehold_MaritalStatus_Invalid() {
            // Build the request data
            StringBuilder sXML = new StringBuilder();
            sXML.Append("<parameters>");
            sXML.AppendFormat("<householdCode>{0}</householdCode>", _householdCode);
            sXML.Append("</parameters>");

            // Make the request to get the household
            XElement responseData = MakeDERequest(Methods.GetHousehold, sXML);

            // Update the marital status
            responseData.Element("household").SetAttributeValue("entityState", "Modified");
            responseData.Element("household").Element("individuals").Elements("individual").Attributes("individualCode").Single(individualCode => individualCode.Value == _individualCode).Parent.SetAttributeValue("entityState", "Modified");
            responseData.Element("household").Element("individuals").Elements("individual").Attributes("individualCode").Single(individualCode => individualCode.Value == _individualCode).Parent.Descendants("maritalStatus").FirstOrDefault().SetValue("Divorced");

            // Make the request
            StringBuilder sXML2 = new StringBuilder(responseData.ToString());
            XElement secondResponseData = MakeDERequest(Methods.UpdateHousehold, sXML2);
            Assert.AreEqual("False", secondResponseData.Descendants("Success").FirstOrDefault().Value);
        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user can update the marital status with a null value.")]
        public void UpdateHousehold_MaritalStatus_Null() {
            // Build the request data
            StringBuilder sXML = new StringBuilder();
            sXML.Append("<parameters>");
            sXML.AppendFormat("<householdCode>{0}</householdCode>", _householdCode);
            sXML.Append("</parameters>");

            // Make the request to get the household
            XElement responseData = MakeDERequest(Methods.GetHousehold, sXML);

            // Update the marital status
            responseData.Element("household").SetAttributeValue("entityState", "Modified");
            responseData.Element("household").Element("individuals").Elements("individual").Attributes("individualCode").Single(individualCode => individualCode.Value == _individualCode).Parent.SetAttributeValue("entityState", "Modified");
            responseData.Element("household").Element("individuals").Elements("individual").Attributes("individualCode").Single(individualCode => individualCode.Value == _individualCode).Parent.Descendants("maritalStatus").FirstOrDefault().SetValue(string.Empty);

            // Make the request
            StringBuilder sXML2 = new StringBuilder(responseData.ToString());
            XElement secondResponseData = MakeDERequest(Methods.UpdateHousehold, sXML2);
            Assert.AreEqual("1009::PI::The individual was found - update process will continue", secondResponseData.Value);

            // Verify the marital status
            responseData = MakeDERequest(Methods.GetHousehold, sXML);
            Assert.AreEqual(string.Empty, responseData.Element("household").Element("individuals").Elements("individual").Attributes("individualCode").Single(individualCode => individualCode.Value == _individualCode).Parent.Descendants("maritalStatus").FirstOrDefault().Value);
        }
        #endregion Update Household

        #region Update Household Collection
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user can update the marital status with a valid type.")]
        public void UpdateHouseholdCollection_MaritalStatus_Valid() {
            // Build the request data
            StringBuilder sXML = new StringBuilder();
            sXML.Append("<parameters>");
            sXML.AppendFormat("<householdCode>{0}</householdCode>", _householdCode);
            sXML.Append("</parameters>");

            // Make the request to get the household
            XElement responseData = MakeDERequest(Methods.GetHouseholdCollection, sXML);

            // Update the marital status
            responseData.Element("households").Elements("household").Attributes("householdCode").Single(householdCode => householdCode.Value == _householdCode).Parent.SetAttributeValue("entityState", "Modified");
            responseData.Element("households").Elements("household").Attributes("householdCode").Single(householdCode => householdCode.Value == _householdCode).Parent.Element("individuals").Elements("individual").Attributes("individualCode").Single(individualCode => individualCode.Value == _individualCode).Parent.SetAttributeValue("entityState", "Modified");
            responseData.Element("households").Elements("household").Attributes("householdCode").Single(householdCode => householdCode.Value == _householdCode).Parent.Element("individuals").Elements("individual").Attributes("individualCode").Single(individualCode => individualCode.Value == _individualCode).Parent.Descendants("maritalStatus").FirstOrDefault().SetValue("Single");
            responseData.Element("households").Parent.RemoveAttributes();

            // Make the request
            StringBuilder sXML2 = new StringBuilder(responseData.ToString());
            XElement secondResponseData = MakeDERequest(Methods.UpdateHouseholdCollection, sXML2);
            Assert.AreEqual("True", secondResponseData.Descendants("Success").FirstOrDefault().Value);

            // Verify the marital status
            responseData = MakeDERequest(Methods.GetHousehold, sXML);
            Assert.AreEqual("Single", responseData.Element("household").Element("individuals").Elements("individual").Attributes("individualCode").Single(individualCode => individualCode.Value == _individualCode).Parent.Descendants("maritalStatus").FirstOrDefault().Value);
        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user cannot update the marital status with an invalid type.")]
        public void UpdateHouseholdCollection_MaritalStatus_Invalid() {
            // Build the request data
            StringBuilder sXML = new StringBuilder();
            sXML.Append("<parameters>");
            sXML.AppendFormat("<householdCode>{0}</householdCode>", _householdCode);
            sXML.Append("</parameters>");

            // Make the request to get the household
            XElement responseData = MakeDERequest(Methods.GetHouseholdCollection, sXML);

            // Update the marital status
            responseData.Element("households").Elements("household").Attributes("householdCode").Single(householdCode => householdCode.Value == _householdCode).Parent.SetAttributeValue("entityState", "Modified");
            responseData.Element("households").Elements("household").Attributes("householdCode").Single(householdCode => householdCode.Value == _householdCode).Parent.Element("individuals").Elements("individual").Attributes("individualCode").Single(individualCode => individualCode.Value == _individualCode).Parent.SetAttributeValue("entityState", "Modified");
            responseData.Element("households").Elements("household").Attributes("householdCode").Single(householdCode => householdCode.Value == _householdCode).Parent.Element("individuals").Elements("individual").Attributes("individualCode").Single(individualCode => individualCode.Value == _individualCode).Parent.Descendants("maritalStatus").FirstOrDefault().SetValue("Divorced");
            responseData.Element("households").Parent.RemoveAttributes();

            // Make the request
            StringBuilder sXML2 = new StringBuilder(responseData.ToString());
            XElement secondResponseData = MakeDERequest(Methods.UpdateHouseholdCollection, sXML2);
            Assert.AreEqual("False", secondResponseData.Descendants("Success").FirstOrDefault().Value);
        }

        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user can update the marital status with a null value.")]
        public void UpdateHouseholdCollection_MaritalStatus_Null() {
            // Build the request data
            StringBuilder sXML = new StringBuilder();
            sXML.Append("<parameters>");
            sXML.AppendFormat("<householdCode>{0}</householdCode>", _householdCode);
            sXML.Append("</parameters>");

            // Make the request to get the household
            XElement responseData = MakeDERequest(Methods.GetHouseholdCollection, sXML);

            // Update the marital status
            responseData.Element("households").Elements("household").Attributes("householdCode").Single(householdCode => householdCode.Value == _householdCode).Parent.SetAttributeValue("entityState", "Modified");
            responseData.Element("households").Elements("household").Attributes("householdCode").Single(householdCode => householdCode.Value == _householdCode).Parent.Element("individuals").Elements("individual").Attributes("individualCode").Single(individualCode => individualCode.Value == _individualCode).Parent.SetAttributeValue("entityState", "Modified");
            responseData.Element("households").Elements("household").Attributes("householdCode").Single(householdCode => householdCode.Value == _householdCode).Parent.Element("individuals").Elements("individual").Attributes("individualCode").Single(individualCode => individualCode.Value == _individualCode).Parent.Descendants("maritalStatus").FirstOrDefault().SetValue(string.Empty);
            responseData.Element("households").Parent.RemoveAttributes();

            // Make the request
            StringBuilder sXML2 = new StringBuilder(responseData.ToString());
            XElement secondResponseData = MakeDERequest(Methods.UpdateHouseholdCollection, sXML2);
            Assert.AreEqual("True", secondResponseData.Descendants("Success").FirstOrDefault().Value);

            // Verify the marital status
            responseData = MakeDERequest(Methods.GetHousehold, sXML);
            Assert.AreEqual(string.Empty, responseData.Element("household").Element("individuals").Elements("individual").Attributes("individualCode").Single(individualCode => individualCode.Value == _individualCode).Parent.Descendants("maritalStatus").FirstOrDefault().Value);
        }

        #endregion Update Household Collection

        #region IsConverted
        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the IsConverted method.")]
        public void IsConverted() {
            // Build the request data
            StringBuilder sXML = new StringBuilder();
            sXML.Append("<parameters>");
            sXML.AppendFormat("<individualCode>{0}</individualCode>", _individualCode);
            sXML.Append("</parameters>");

            // Make the request
            XElement responseData = MakeDERequest(Methods.IsConverted, sXML);

            // Verify data points
            Assert.IsTrue(bool.Parse(responseData.Element("isConverted").Value));
        }
        #endregion IsConverted

    }
}