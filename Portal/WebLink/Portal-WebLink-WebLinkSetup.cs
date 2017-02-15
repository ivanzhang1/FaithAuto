using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace FTTests.Portal.WebLink {
	[TestFixture]
    [Importance(Importance.Critical)]
	public class Portal_WebLink_WebLinkSetup : FixtureBase {
		#region Church Information
		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Matthew Sneeden", "msneeden@fellowshiptech.com")]
		[Description("Attempts to update Church Information with required fields blank.")]
		public void WebLink_WebLinkSetup_ChurchInformation_UpdateNoData() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Navigate to weblink->church information
			test.Selenium.Navigate(Navigation.WebLink.WebLink_Setup.Church_Information);

			// Attempt to save with required fields blank
			test.Selenium.Type("ctl00_ctl00_MainContent_content_txtWebsiteAddress", "");
			test.Selenium.Type("ctl00_ctl00_MainContent_content_txtWebmasterEmail", "");
			test.Selenium.ClickAndWaitForPageToLoad("ctl00_ctl00_MainContent_content_btnUpdate");

			// Verify error(s) displayed to the user
			test.Selenium.VerifyTextPresent(new string[] { TextConstants.ErrorHeadingPlural, "Website address is required.", "Webmaster email is required." });

			// Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden", "msneeden@fellowshiptech.com")]
		[Description("Attempts to update Church Information.")]
		public void WebLink_WebLinkSetup_ChurchInformation_Update() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Navigate to weblink->church information
			test.Selenium.Navigate(Navigation.WebLink.WebLink_Setup.Church_Information);

			// Attempt to update the information
			test.Selenium.Type("ctl00_ctl00_MainContent_content_txtWebsiteAddress", "www.testing.com");
			test.Selenium.Type("ctl00_ctl00_MainContent_content_txtWebmasterEmail", "test@testing.com");
			test.Selenium.ClickAndWaitForPageToLoad("ctl00_ctl00_MainContent_content_btnUpdate");

			// Verify the updated information
			test.Selenium.Navigate(Navigation.WebLink.WebLink_Setup.Church_Information);

			Assert.AreEqual("www.testing.com", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_txtWebsiteAddress"));
			Assert.AreEqual("test@testing.com", test.Selenium.GetValue("ctl00_ctl00_MainContent_content_txtWebmasterEmail"));

			// Reset the data
			test.Selenium.Type("ctl00_ctl00_MainContent_content_txtWebsiteAddress", "www.dynamic-church.com");
			test.Selenium.Type("ctl00_ctl00_MainContent_content_txtWebmasterEmail", "webminister@fellowshiptech.com");
			test.Selenium.ClickAndWaitForPageToLoad("ctl00_ctl00_MainContent_content_btnUpdate");

			// Logout of portal
			test.Portal.Logout();
		}
		#endregion Church Information

		#region Skin Manager
		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Matthew Sneeden", "msneeden@fellowshiptech.com")]
		[Description("Attempts to update a Skin with none of the required fields.")]
		public void WebLink_WebLinkSetup_SkinManager_UpdateNoData() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Navigate to weblink->skin manager
			test.Selenium.Navigate(Navigation.WebLink.WebLink_Setup.Skin_Manager);

			// Edit an existing skin
			test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Edit);

			// Attempt to save with required fields blank
			test.Selenium.Type("ctl00_ctl00_MainContent_content_txtAlias", "");
			test.Selenium.ClickAndWaitForPageToLoad("ctl00_ctl00_MainContent_content_btnUpdate");

			// Verify error(s) displayed to the user
			test.Selenium.VerifyTextPresent(new string[] { TextConstants.ErrorHeadingSingular, "Module name is required." });

			// Logout of portal
			test.Portal.Logout();
		}
		#endregion Skin Manager

		#region Integration Codes
		#endregion Integration Codes
	}
}