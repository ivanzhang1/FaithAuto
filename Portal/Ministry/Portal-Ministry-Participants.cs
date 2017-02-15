using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using System.Collections.Generic;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace FTTests.Portal.Ministry {
	[TestFixture]
	public class Portal_Ministry_Participants : FixtureBaseWebDriver {

		#region Assignments
        //[Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies Participant assignment pagew will load.")]
        public void Portal_Ministry_ParticipantAssignment()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");
            // Navigate to Participant Assignment page
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Participants.Assignments);
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("ctl00_ctl00_MainContent_content_ctl00_ctlFindPerson_lnkFindPerson"));
            // verify page elements            
            test.GeneralMethods.VerifyTextPresentWebDriver("Current ministry");
            test.GeneralMethods.VerifyTextPresentWebDriver("Participant Assignments");
            test.GeneralMethods.VerifyTextPresentWebDriver("Activity Detail");
            test.GeneralMethods.VerifyTextPresentWebDriver("Activity");

            //Logout
            test.Portal.LogoutWebDriver();
            
        }

      //  [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Tests the ability to add and remove an assigment")]
        public void Portal_Ministry_ParticipantAssignment_AddRemove()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");
            //Set conditions
            string firstName = "Assignment";
            string lastName = "Individual";
            string fullName = firstName + " " + lastName; 
            test.SQL.People_Individual_Create(15, firstName, lastName);
            //Navigate to participant assignment page 
            test.GeneralMethods.Navigate_Portal(Navigation.Ministry.Participants.Assignments);
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("ctl00_ctl00_MainContent_content_ctl00_ctlFindPerson_lnkFindPerson"));
            //Find an individual
            test.Driver.FindElementByLinkText("Find person").Click();
            test.GeneralMethods.SelectPersonFromModalWebDriver(fullName, false);
            //Select parameters for assignments
            new SelectElement(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctl00_ddlActivity_dropDownList")).SelectByText("A Test Activity");
            new SelectElement(test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctl01_ddlActivityDetail_dropDownList")).SelectByText("A Test RLC");
            //Click Add Assignment
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnSubmit").Click();
            //Verify Assignment
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id("ctl00_ctl00_MainContent_content_dgIndividualPrefs_ctl02_lblActivityName"));
            //Delete Assignment
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_dgIndividualPrefs_ctl02_lbtnDelete").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("ctl00_ctl00_MainContent_content_ctl00_ddlActivity_dropDownList"));
            // Cleanup
            test.SQL.People_MergeIndividual(15, fullName, "Merge Dump");

            //Log out 
            test.Portal.LogoutWebDriver();
            
        }


		#endregion Assignments

		#region Move Assignments
		#endregion Move Assignments
	}
}