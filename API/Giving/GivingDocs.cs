using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using System.Linq;
using System.Net;
using System.IO;
using System.Xml.Linq;
using FTTests;

namespace FTTests.API {
    [TestFixture]
    public class GivingDocs {

        //[Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("www.google.com")]
        public void Google() {
            APIBase api = new APIBase();

            // General

            //HttpWebResponse response = api.MakeWebRequest("http://dc.apiqa.dev.corp.local/giving/v1/Accounts.help");
            WebResponse response = api.MakeWebRequest("http://dc.apiqa.dev.corp.local/giving/v1/Accounts.help");

            string responseString = api.GetResponseString((HttpWebResponse)response);

            TestLog.WriteLine("Response: " + responseString);
            //Assert.AreEqual(HttpStatusCode.OK, api.MakeAPIRequest("GET", "text/html", "Util/Docs.help").StatusCode);
            //Assert.IsNotNull(api.MakeAPIRequest("GET", "text/html", "Util/Docs.help").ContentLength);
        }


        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the general help document for the Giving API")]
        public void General() {
            APIBase api = new APIBase();
            api.Realm = "giving";

            // General
            HttpWebResponse response = api.MakeAPIRequest("GET", "application/help", "Util/Docs.help");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.LessThanOrEqualTo(0, response.ContentLength);


            string responseString = api.GetResponseString(response);

            TestLog.WriteLine("Response: " + responseString);
            Assert.Contains(responseString, "Fellowship One RESTful API - Docs");

        }


        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the Accounts help document for the Giving API")]
        public void Accounts() {
            APIBase api = new APIBase();
            api.Realm = "giving";

            HttpWebResponse response = api.MakeAPIRequest("GET", "application/help", "Accounts.help");

            // Accounts
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.LessThanOrEqualTo(0, response.ContentLength);

            string responseString = api.GetResponseString(response);

            TestLog.WriteLine("Response: " + responseString);
            Assert.Contains(responseString, "Fellowship One RESTful API - Accounts");

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the Batches help document for the Giving API")]
        public void Batches() {
            APIBase api = new APIBase();
            api.Realm = "giving";

            HttpWebResponse response = api.MakeAPIRequest("GET", "application/help", "Batches.help");

            // Batches
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.LessThanOrEqualTo(0, response.ContentLength);

            string responseString = api.GetResponseString(response);

            TestLog.WriteLine("Response: " + responseString);
            Assert.Contains(responseString, "Fellowship One RESTful API - Batches");

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the ContributionReceipts help document for the Giving API")]
        public void ContributionReceipts() {
            APIBase api = new APIBase();
            api.Realm = "giving";


            HttpWebResponse response = api.MakeAPIRequest("GET", "application/help", "ContributionReceipts.help");

            // Contribution Receipts
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.LessThanOrEqualTo(0, response.ContentLength);

            string responseString = api.GetResponseString(response);

            TestLog.WriteLine("Response: " + responseString);
            Assert.Contains(responseString, "Fellowship One RESTful API - ContributionReceipts");

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the ContributionTypes help document for the Giving API")]
        public void ContributionTypes() {
            APIBase api = new APIBase();
            api.Realm = "giving";

            HttpWebResponse response = api.MakeAPIRequest("GET", "application/help", "ContributionTypes.help");

            // Contribution Types
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.LessThanOrEqualTo(0, response.ContentLength);

            string responseString = api.GetResponseString(response);

            TestLog.WriteLine("Response: " + responseString);
            Assert.Contains(responseString, "Fellowship One RESTful API - ContributionTypes");


        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the Funds help document for the Giving API")]
        public void Funds() {
            APIBase api = new APIBase();
            api.Realm = "giving";

            HttpWebResponse response = api.MakeAPIRequest("GET", "application/help", "Funds.help");

            // Funds
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.LessThanOrEqualTo(0, response.ContentLength);

            string responseString = api.GetResponseString(response);

            TestLog.WriteLine("Response: " + responseString);
            Assert.Contains(responseString, "Fellowship One RESTful API - Funds");



        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the SubFunds help document for the Giving API")]
        public void SubFunds() {
            APIBase api = new APIBase();
            api.Realm = "giving";

            // SubFunds
            HttpWebResponse response = api.MakeAPIRequest("GET", "application/help", "SubFunds.help");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.LessThanOrEqualTo(0, response.ContentLength);

            string responseString = api.GetResponseString(response);

            TestLog.WriteLine("Response: " + responseString);
            Assert.Contains(responseString, "Fellowship One RESTful API - SubFunds");

        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the PledgeDrives help document for the Giving API")]
        public void PledgeDrives() {
            APIBase api = new APIBase();
            api.Realm = "giving";

            // Pledge Drives
            HttpWebResponse response = api.MakeAPIRequest("GET", "application/help", "PledgeDrives.help");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.LessThanOrEqualTo(0, response.ContentLength);

            string responseString = api.GetResponseString(response);

            TestLog.WriteLine("Response: " + responseString);
            Assert.Contains(responseString, "Fellowship One RESTful API - PledgeDrives");


        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the RDCBatches help document for the Giving API")]
        public void RDCBatches() {
            APIBase api = new APIBase();
            api.Realm = "giving";

            // RDCBatches
            HttpWebResponse response = api.MakeAPIRequest("GET", "application/help", "RDCBatches.help");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.LessThanOrEqualTo(0, response.ContentLength);

            string responseString = api.GetResponseString(response);

            TestLog.WriteLine("Response: " + responseString);
            Assert.Contains(responseString, "Fellowship One RESTful API - RDCBatches");

        }


        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the ReferenceImages help document for the Giving API")]
        public void ReferenceImages() {
            APIBase api = new APIBase();
            api.Realm = "giving";

            // Reference Images
            HttpWebResponse response = api.MakeAPIRequest("GET", "application/help", "ReferenceImages.help");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.LessThanOrEqualTo(0, response.ContentLength);

            string responseString = api.GetResponseString(response);

            TestLog.WriteLine("Response: " + responseString);
            Assert.Contains(responseString, "Fellowship One RESTful API - ReferenceImages");


        }
    }
}
