using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace FTTests.Portal.WebLink {
    public struct WebLinkEnumerations {
        public enum InFellowshipBrandingColors {
            blue_light,
            blue_dark,
            green_light,
            green_dark,
            red_light,
            red_dark,
            brown_light,
            brown_dark,
            gray
        }
    }

	[TestFixture]
    [Importance(Importance.Critical)]
	public class Portal_WebLink : FixtureBase {
		[Test, RepeatOnFailure, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Verifies the links present under the weblink menu.")]
		public void WebLink() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Verify the links present
			Assert.AreEqual("Event Registration", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[1]/dt"));
			Assert.AreEqual("Manage Forms", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[1]/dd[1]/a"));
			Assert.AreEqual("Aggregate Forms", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[1]/dd[2]/a"));
			Assert.AreEqual("View Submissions", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[1]/dd[3]/a"));

			Assert.AreEqual("Volunteer Application", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[2]/dt"));
			Assert.AreEqual("Manage Forms", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[2]/dd[1]/a"));

			Assert.AreEqual("Online Giving", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[3]/dt"));
			Assert.AreEqual("Confirmation Messages", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[3]/dd[1]/a"));

			Assert.AreEqual("WebLink Groups", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[4]/dt"));
			Assert.AreEqual("Questions", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[4]/dd[1]/a"));
            Assert.AreEqual("Associate Questions", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[4]/dd[2]/a"));
            Assert.AreEqual("Search Properties", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[4]/dd[3]/a"));

            Assert.AreEqual("Small Group Manager", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[5]/dt"));
            Assert.AreEqual("Questions", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[5]/dd[1]/a"));
            Assert.AreEqual("Associate Questions", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[5]/dd[2]/a"));

			Assert.AreEqual("WebLink Setup", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[6]/dt"));
			Assert.AreEqual("Church Information", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[6]/dd[1]/a"));
			Assert.AreEqual("Skin Manager", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[6]/dd[2]/a"));
			Assert.AreEqual("Integration Codes", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[6]/dd[3]/a"));

            Assert.AreEqual("InFellowship", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[7]/dt"));
            Assert.AreEqual("Features", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[7]/dd[1]/a"));
            Assert.AreEqual("Branding", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[7]/dd[2]/a"));
            Assert.AreEqual("Links", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[7]/dd[3]/a"));
            Assert.AreEqual("Contact", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[7]/dd[4]/a"));
            Assert.AreEqual("Privacy", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[7]/dd[5]/a"));

            // Logout of portal
            test.Portal.Logout();
        }
        
        #region Event Registration
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Views the manage forms page under weblink.")]
        public void WebLink_EventRegistration_ManageForms() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to weblink->manage forms
			test.Selenium.Navigate(Navigation.Portal.WebLink.Event_Registration.Manage_Forms);

			// Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Manage Event Registration Forms");
            test.Selenium.VerifyTextPresent("Manage Event Registration Forms");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the aggregate forms page under weblink.")]
        public void WebLink_EventRegistration_AggregateForms() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Naivgate to weblink->aggregate forms
			test.Selenium.Navigate(Navigation.Portal.WebLink.Event_Registration.Aggregate_Forms);

			// Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Aggregate Forms");
            test.Selenium.VerifyTextPresent("Aggregate Forms");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the view submissions page under weblink.")]
        public void WebLink_EventRegistration_ViewSubmissions() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to weblink->view submissions
			test.Selenium.Navigate(Navigation.Portal.WebLink.Event_Registration.View_Submissions);

			// Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: View Submissions");
            test.Selenium.VerifyTextPresent("View Submissions");

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Event Registration
        
        #region Volunteer Application
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Views the manage forms page under weblink.")]
        public void WebLink_VolunteerApplication_ManageForms() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Select weblink->manage forms
            test.Selenium.Navigate(Navigation.WebLink.Volunteer_Application.Manage_Forms);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Manage Volunteer Forms");
            test.Selenium.VerifyTextPresent("Manage Volunteer Forms");

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Volunteer Application
        
        #region Online Giving
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Views the confirmation messages page under weblink.")]
        public void WebLink_OnlineGiving_ConfirmationMessages() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to weblink->confirmation message
			test.Selenium.Navigate(Navigation.WebLink.Online_Giving.Confirmation_Messages);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Contribution Success Message");
            test.Selenium.VerifyTextPresent("Contribution Success Message");


            // View the contribution failure message page
            test.Selenium.ClickAndWaitForPageToLoad("link=Contribution Failure");

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Contribution Failure Message");
            test.Selenium.VerifyTextPresent("Contribution Failure Message");

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Online Giving

		#region Small Group Manager
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Views the questions page under weblink.")]
        public void WebLink_SmallGroupManager_Questions() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to weblink->questions
			test.Selenium.Navigate(Navigation.WebLink.Small_Group_Manager.Questions);

			// Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Questions");
            test.Selenium.VerifyTextPresent("Questions");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Views the associate questions page under weblink.")]
        public void WebLink_SmallGroupManager_AssociateQuestions() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to weblink->associate questions
			test.Selenium.Navigate(Navigation.WebLink.Small_Group_Manager.Associate_Questions);

			// Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Associate Questions");
            test.Selenium.VerifyTextPresent("Associate Questions");

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Small Group Manager
        
        #region WebLink Setup
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Views the church information page under weblink.")]
        public void WebLink_WebLinkSetup_ChurchInformation() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to weblink->church information
			test.Selenium.Navigate(Navigation.WebLink.WebLink_Setup.Church_Information);

			// Verify title, text
			test.Selenium.VerifyTitle("Fellowship One :: Church Information");
			test.Selenium.VerifyTextPresent("Church Information");

			// Verify breadcrumb
            Assert.AreEqual(Navigation.WebLink.WebLink_Setup.Church_Information, test.Selenium.GetText("//p[@id='breadcrumb']"));

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the skin manager page under weblink.")]
        public void WebLink_WebLinkSetup_SkinManager() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Navigate to weblink->skin manager
			test.Selenium.Navigate(Navigation.WebLink.WebLink_Setup.Skin_Manager);

			// Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Skin Manager");
            test.Selenium.VerifyTextPresent("Skin Manager");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Views the integration codes page under weblink.")]
        public void WebLink_WebLinkSetup_IntegrationCodes() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

			// Select weblink->integration codes
			test.Selenium.Navigate(Navigation.WebLink.WebLink_Setup.Integration_Codes);

			// Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: Integration Codes");
            test.Selenium.VerifyTextPresent("Integration Codes");

			// Logout of portal
			test.Portal.Logout();
        }
        #endregion WebLink Setup

        #region InFellowship
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the branding page under Weblink.")]
        public void WebLink_InFellowship_Branding() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to weblink->branding
            test.Selenium.Navigate(Navigation.WebLink.InFellowship.Branding);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: InFellowship Branding");
            test.Selenium.VerifyTextPresent("InFellowship Branding");

            // ** Color theme
            // Click link to view the color theme
            test.Selenium.ClickAndWaitForPageToLoad("link=Color theme");

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: InFellowship Branding");
            test.Selenium.VerifyTextPresent("InFellowship Branding");

            // ** Header logo
            // Click link to view the header logo
            test.Selenium.ClickAndWaitForPageToLoad("link=Header logo");

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: InFellowship Branding");
            test.Selenium.VerifyTextPresent("InFellowship Branding");

            // ** Custom content
            // Click link to view custom content
            test.Selenium.ClickAndWaitForPageToLoad("link=Custom content");

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: InFellowship Branding");
            test.Selenium.VerifyTextPresent("InFellowship Branding");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Navigates to the Features page under WebLink.")]
        public void WebLink_InFellowship_Features() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to weblink->features
            test.Selenium.Navigate(Navigation.WebLink.InFellowship.Features);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: InFellowship Features");
            test.Selenium.VerifyTextPresent("InFellowship Features");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Navigates to the links page under WebLink.")]
        public void WebLink_InFellowship_Links() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to weblink->links
            test.Selenium.Navigate(Navigation.WebLink.InFellowship.Links);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: InFellowship Links");
            test.Selenium.VerifyTextPresent("InFellowship Links");

            // Verify the links are correct
            test.infellowship.InFellowshipChurchCode = "dc";
            var inFellowshipURL = test.infellowship.URL.ToLower();
            TestLog.Write("inFellowshipURL :" + inFellowshipURL);
            Assert.AreEqual(inFellowshipURL, test.Selenium.GetText("//tbody/tr[2]/td[2]/span/a") + "/");
            //There is '/' at the end of the url
            Assert.AreEqual(string.Format("{0}people/profile", inFellowshipURL), test.Selenium.GetText("//tbody/tr[3]/td[2]/span/a"));
            Assert.AreEqual(string.Format("{0}people/privacy/edit", inFellowshipURL), test.Selenium.GetText("//tbody/tr[4]/td[2]/span/a"));
            Assert.AreEqual(string.Format("{0}groupsearch/show", inFellowshipURL), test.Selenium.GetText("//tbody/tr[5]/td[2]/span/a"));
            Assert.AreEqual(string.Format("{0}groupsearch/index", inFellowshipURL), test.Selenium.GetText("//tbody/tr[6]/td[2]/span/a"));

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the contact page under WebLink.")]
        public void WebLink_InFellowship_Contact() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to weblink->contact
            test.Selenium.Navigate(Navigation.WebLink.InFellowship.Contact);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: InFellowship Contact Email");
            test.Selenium.VerifyTextPresent("InFellowship Contact Email");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan MIkaelian")]
        [Description("Navigates to the privacy policy page under WebLink.")]
        public void WebLink_InFellowship_Privacy() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to weblink->contact
            test.Selenium.Navigate(Navigation.WebLink.InFellowship.Privacy);

            // Verify title, text
            test.Selenium.VerifyTitle("Fellowship One :: InFellowship Privacy Policy");
            test.Selenium.VerifyTextPresent("InFellowship Privacy Policy");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a Group Type Admin cannot change any settings for InFellowship.")]
        public void WebLink_InFellowship_GTA_Denied() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Verify a group type admin cannot anything for InFellowship, under Weblink.
            test.Selenium.VerifyElementNotPresent("link=WebLink");

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a Portal user with the WebLink right can change all the settings for InFellowship.")]
        public void WebLink_InFellowship_WebLinkUser_Allowed() {
            // Login to portal as a user with WebLink rights.
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("weblinkuser", "BM.Admin09", "QAEUNLX0C2");

            // Verify the InFellowship links are all present.       
            Assert.AreEqual("InFellowship", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[7]/dt"));
            Assert.AreEqual("Features", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[7]/dd[1]/a"));
            Assert.AreEqual("Branding", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[7]/dd[2]/a"));
            Assert.AreEqual("Links", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[7]/dd[3]/a"));
            Assert.AreEqual("Contact", test.Selenium.GetText("//div[@id='nav_sub_7']/dl[7]/dd[4]/a"));
            
            // Logout of portal
            test.Portal.Logout();

        }


        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Navigates to the Online Giving 2.0 Conversion page under WebLink.")]
        public void WebLink_InFellowship_Conversion() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("ft.tester", "FT4life!", "QAEUNLX0C4");

            if (test.Configuration.AppSettings.Settings["FTTests.Environment"].Value.ToString() == "LV_QA")
            {


                // Navigate to weblink->features
                test.Selenium.Navigate(Navigation.WebLink.InFellowship.Conversion);

                // Verify title, text
                test.Selenium.VerifyTitle("Fellowship One :: Online Giving Conversion");
                test.Selenium.VerifyTextPresent("Online Giving Conversion");

            }

            // Logout of portal
            test.Portal.Logout();
        }

        #endregion InFellowship
    }
}