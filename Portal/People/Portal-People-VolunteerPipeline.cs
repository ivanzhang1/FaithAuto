using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using System.Xml.Linq;
using System.Xml;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace FTTests.Portal.People
{


    [TestFixture]
    [Importance(Importance.Critical)]
    public class Portal_People_VolunteerPipeline_WebDriver : FixtureBaseWebDriver
    {
        #region Background Checks

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Felix Gaytan")]
        [Description("Submits a background check by different user")]
        public void People_VolunteerPipeline_BackgroundChecks_Submit_Diff_User()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string social;
            test.Portal.LoginWebDriver("felixg", "FG.Admin12");

            // Submit a background check
            test.Portal.People_Generate_BackgroundCheck_WebDriver("Matthew Sneeden", "09/02/1982", "111111111", "Background Check", test.Portal.PortalUsername);

            try
            {
                //Verify SSN Encryption in Database(UNIQUE_ID)
                int individualID = test.SQL.People_Individuals_FetchID(15, "Matthew Sneeden");
                TestLog.WriteLine("Individual ID {0}", individualID);
                string uniqueID = test.SQL.People_Individuals_UniqueID(15, individualID);
                TestLog.WriteLine("Encrypted value : {0}", uniqueID);
                Assert.AreNotEqual(uniqueID, "111111111", "SSN is not Encrypted");

                //Commenting these lines until we figureout a way to run background check service automatically
                /*
                //EXtracting and validating SSN value in Xml Request
                string xmlString = test.SQL.People_Individuals_xmlRequest(individualID);
            

                //Parsing the xml string to Xmldocument
           
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(xmlString); 

                XmlNodeList xnList = xml.SelectNodes("/Element[@*]/SSN");
                foreach (XmlNode xn in xnList)
                {
                    XmlNode ssn = xn.SelectSingleNode("SSN");
                    if (ssn != null)
                    {
                        string ssn1 = ssn["SSN1"].InnerText;
                        string ssn2 = ssn["SSN2"].InnerText;
                        string ssn3 = ssn["SSN3"].InnerText;
                        social = string.Concat( string.Concat(ssn1, ssn2), ssn3);

                        //Validating Social from xml with WebForm application Social
                        Assert.AreEqual("111111111", social);

                    }

                }
                */
            }
            finally
            {
                test.Portal.People_Verify_Delete_Requirement_WebDriver("Background Check", "Matthew Sneeden", test.Portal.PortalUsername, "Today", "Pending", true, true);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }

        }


        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Verifies the proper error appears when an unlinked portal user tries to submit a background check.")]
        public void People_VolunteerPipeline_BackgroundChecks_Submit_Unlinked()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.unlinkedtest", "FT4life!", "dc");

            // Try to submit a background check
            test.Portal.People_Generate_BackgroundCheck_WebDriver("David Martin", "07/02/1978", "111111111", "Background Check", test.Portal.PortalUsername, false);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Ivan Zhang")]
        [Description("Verifies the proper error appears when an invalid value input to DOB")]
        public void People_VolunteerPipeline_BackgroundChecks_DOBValidation()
        {
            string ExpectedErrorMsg = "Date of Birth is invalid.";
            string RealErrormsg = "";
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("****Try to login the portal *****");
            test.Portal.LoginWebDriver("izhang2", "123qwe!@#QWE", "DC");


            // catach the error msg
            try
            {
                test.Portal.People_Generate_BackgroundCheck_WebDriver("Ivan Zhang", "1111111111", "111111111", "Background Check", test.Portal.PortalUsername, true);
            }
            catch (WebDriverException e)
            {
                RealErrormsg=e.Message.Replace("Error Messages Found. ", "");
            }

            //determine whether it is the expected error msg.
            TestLog.WriteLine("***Try to determine whether it is expected error msg");
            Assert.AreEqual(RealErrormsg.Trim(),ExpectedErrorMsg);
            
            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        #endregion Background Checks

        #region Submit Application
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Views the find person popup on the submit application page.")]
        public void People_DataIntegrity_SubmitApplication_FindPerson()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to people->submit application

            test.GeneralMethods.Navigate_Portal(Navigation.People.Volunteer_Pipeline.Submit_Application);

            // View the find person popup
            test.Driver.FindElementByLinkText("Find person").Click();
            test.Driver.SwitchTo().Window("psuedoModal");

            // Close the popup
            test.Driver.FindElementByLinkText("Cancel").Click();
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[0]);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        #endregion Submit Application

        #region Application Workflow
        //[Test, RepeatOnFailure, Timeout(6000)]
        [Author("Jim Jin")]
        [Description("Verify the page load time of verify requirement page would not exceed 10 seconds")]
        public void People_VolunteerPipeline_VerifyRequirement_PLT()
        {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            #region parameters
            //string applicationName = "App2015061013385442";
            //string opportunityName = "Opp2015061013385442";
            string applicationName = utility.GetUniqueName("App");
            string opportunityName = utility.GetUniqueName("Opp");
            string requirementName = utility.GetUniqueName("Req");
            string individualName = "";

            // Prepare the communication data
            List<List<string>> comm = new List<List<string>>();
            comm.Add(new List<string>());
            comm[0].Add("Phone");
            comm[0].Add("Home");
            comm[0].Add("8479520411");
            comm.Add(new List<string>());
            comm[1].Add("Email");
            comm[1].Add("Email");
            comm[1].Add("jzf2050@qp1.org");

            List<string> individules = new List<string>();

            PortalBase.AddressData address = new PortalBase.AddressData { Country = "Canada", Address_1 = "#35 250 Satok Crescent", City = "Milton", Province = "Ontario", Postal_code = "L9T 3P4" };
            #endregion

            try
            {
                //Login to portal
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

                #region create a volunteer application form
                test.Portal.WebLink_volunteerApplicationForm_Create(applicationName, opportunityName, "Children - hyphen", "FT Tester", requirementName);
                #endregion

                #region create requirements against specific application
                for (int i = 0; i < 100; i++)
                {
                    string firstName = "Auto";
                    string lastName = utility.GetUniqueName("T");
                    individualName = string.Format("{0} {1}", firstName, lastName);
                    individules.Add(individualName);
                    //Will update it later to generate fake data in database, instead of by front-end
                    test.Portal.People_AddHousehold(firstName, lastName, HouseholdPositionConstants.Head, "Member", comm, address, DateTime.Now.ToString("M/d/yyyy"));
                    test.Portal.People_Edit_Individual_Info(individualName, null, "Male", "Married", "12/23/1960");
                    test.Portal.People_volunteerPipeline_submitApplication(individualName, applicationName, opportunityName);
                    test.Portal.People_volunteerPipeline_reviewApplication_addNotes(individualName, applicationName, opportunityName);
                    test.Portal.People_volunteerPipeline_reviewApplication(individualName, applicationName, opportunityName);
                    test.Portal.People_volunteerPipeline_ministryReview_addNotes(individualName, "Children - hyphen", applicationName, opportunityName);
                    test.Portal.People_volunteerPipeline_ministryReview(individualName, "Children - hyphen", applicationName, opportunityName);

                    test.Portal.People_volunteerPipeline_backgroundCheck_add(individualName, applicationName, opportunityName, "111111111", "Background Check");
                    test.Portal.People_volunteerPipeline_verifyRequirements_addNotes(individualName, applicationName, opportunityName);
                }
                #endregion

                utility.Navigate_Portal(Navigation.People.Volunteer_Pipeline.Verify_Requirements);
                new SelectElement(test.Driver.FindElement(By.Id("ctl00_ctl00_MainContent_content_WorkVolunteerListCtrl1_ddVolunteerApplicationId"))).SelectByText(applicationName);

                //assert page load time
                Assert.GreaterThan(10000, utility.getPageLoadTime(), "Page load time should not exceed 10 seconds");

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }
            finally
            {
                test.SQL.WebLink_VolunteerApplicationForm_Delete(15, applicationName);
                foreach (string individule in individules)
                {
                    test.SQL.People_Individual_Delete(15, individule.Split(' ')[0], individule.Split(' ')[1]);
                }
            }
        }

        #endregion Submit Application
    }

    [TestFixture]
    [Importance(Importance.Critical)]
    public class Portal_People_VolunteerPipeline : FixtureBase
    {
        #region Submit Application
        #endregion Submit Application

        #region Review Applications
        #endregion Review Applications

        #region Ministry Review
        #endregion Ministry Review

        #region Verify Requirements
        #endregion Verify Requirements

        #region Background Checks

        #endregion Background Checks

        #region Approved Volunteers
        #endregion Approved Volunteers

        #region Rejected Volunteers
        #endregion Rejected Volunteers
    }
}