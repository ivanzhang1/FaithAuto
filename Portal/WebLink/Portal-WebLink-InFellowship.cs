using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FTTests.Portal.WebLink {
    [TestFixture]
    public class Portal_Weblink_InFellowship_WebDriver : FixtureBaseWebDriver
    {
        #region Features

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("F1-4248: Verifies a user can enable the option to give without an account and the link will appear in InFellowship")]
        public void WebLink_InFellowship_Features_EnableGiveWithoutAccount()
        {
            // Login portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!");

            // Navigate to Features page
            test.GeneralMethods.Navigate_Portal(Navigation.WebLink.InFellowship.Features);

            // Click on the Online Giving section
            test.Driver.FindElementByXPath(GeneralWebLink.InFellowship.Features.Online_Giving).Click();

            // Enable give without account
            test.GeneralMethods.SelectCheckbox(By.Id(GeneralWebLink.InFellowship.Features.OnlineGiving.EnableGiveWithoutAccount), true); 

            // Save changes
            test.Driver.FindElementById(GeneralWebLink.InFellowship.Features.OnlineGiving.Save_Changes).Click();

            // Logout of portal
            test.Portal.LogoutWebDriver();

            // Verify link appears in InFellowship

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("F1-4248: Verifies a user can disable the option to give without an account and the link will not appear in InFellowship")]
        public void WebLink_InFellowship_Features_DisableGiveWithoutAccount()
        {
            // Login portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!");

            // Navigate to Features page
            test.GeneralMethods.Navigate_Portal(Navigation.WebLink.InFellowship.Features);

            // Click on the Online Giving section
            test.Driver.FindElementByXPath(GeneralWebLink.InFellowship.Features.Online_Giving).Click();

            // Disable give without account
            test.GeneralMethods.SelectCheckbox(By.Id(GeneralWebLink.InFellowship.Features.OnlineGiving.EnableGiveWithoutAccount), false);

            // Save changes
            test.Driver.FindElementById(GeneralWebLink.InFellowship.Features.OnlineGiving.Save_Changes).Click();

            // Verify link on the page doesn't work


            // Logout of portal
            test.Portal.LogoutWebDriver();

            // Verify link does not appear in InFellowship

        }
        #endregion Features

        #region Branding
        //[Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Illustrates that the custom images can be used properly")]
        public void Weblink_InFellowship_Branding_CustomImage1()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login portal
            test.Portal.LoginWebDriver();

            //Navigate to the Branding page 
            test.GeneralMethods.Navigate_Portal(Navigation.WebLink.InFellowship.Branding);

            //CLick on the Custom Content Tab
            test.Driver.FindElementByLinkText("Custom content").Click();
            test.GeneralMethods.WaitForElement(By.Id("inf_custom_content"));

            //Set all fields to default
            test.Driver.FindElementByXPath("//input[@name='inf_branding_radio_grp_1'][@value='1']").Click();
            test.Driver.FindElementByXPath("//input[@name='inf_branding_radio_grp_3'][@value='1']").Click();
            test.Driver.FindElementByXPath("//input[@name='inf_branding_radio_grp_2'][@value='1']").Click();
            test.Driver.FindElementByXPath("//input[@name='inf_branding_radio_grp_4'][@value='1']").Click();
            test.Driver.FindElementById("submitQuery").Click();

            //Page loads?

            //Upload images
            ////*[@id="ctl00_ctl00_MainContent_content_inf_upload_image"]
            ///html/body/div[2]/div[4]/div/div/div/div[2]/form/div/table/tbody/tr[2]/td/input
        }

        #endregion Branding

        #region Privacy Policy
        //[Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Updates the InFellowship Privacy Policy for the church.")]
        public void WebLink_InFellowship_Privacy_Policy_Update_WebDriver()
        {
            // Variables
            var privacyPolicyText = "This is our privacy policy.";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to weblink->privacy policy
            test.GeneralMethods.Navigate_Portal(Navigation.WebLink.InFellowship.Privacy);

            // Overwrite the current value
            test.Driver.FindElementById("privacy_policy_text").SendKeys(privacyPolicyText);
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();
            test.GeneralMethods.VerifyTextPresentWebDriver(privacyPolicyText);

            // Logout of portal
            test.Portal.LogoutWebDriver();

            // Open InFellowship
            test.Infellowship.OpenWebDriver("QAEUNLX0C2");

            //Store the current window handle
            //string BaseWindow = test.Driver.CurrentWindowHandle;

            // View the privacy policy
            test.Driver.FindElementByLinkText("Church Privacy Policy").Click();

            //ReadOnlyCollection<string> handles = test.Driver.WindowHandles;

            ////Should only have 2 windows
            //Assert.AreEqual(2, handles.Count, "More Than One Window Was Launched");

            //foreach (string handle in handles)
            //{

            //    if (handle != BaseWindow)
            //    {
            //        // Verify the text is there
            //        test.GeneralMethods.VerifyTextPresentWebDriver("Your Privacy Rights");
            //    }
            //}
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]);

            // Verify the text is there
            test.GeneralMethods.VerifyTextPresentWebDriver("Your Privacy Rights");

            // Back
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[0]);

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            //Store the current window handle
            //string BaseWindow = test.Driver.CurrentWindowHandle;

            // View the privacy policy
            test.Driver.FindElementByLinkText("Church Privacy Policy").Click();

            //ReadOnlyCollection<string> handles = test.Driver.WindowHandles;

            //Should only have 2 windows
            //Assert.AreEqual(2, handles.Count, "More Than One Window Was Launched");

            //foreach (string handle in handles)
            //{

            //    if (handle != BaseWindow)
            //    {
            //        // Verify the text is there
            //        test.GeneralMethods.VerifyTextPresentWebDriver("Your Privacy Rights");
            //    }
            //}
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]);

            // Verify the text is there
            test.GeneralMethods.VerifyTextPresentWebDriver("Your Privacy Rights");

            // Back
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]).Close();
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[0]);

            //// View the privacy policy
            //test.Driver.FindElementByLinkText("Church Privacy Policy").Click();
            //test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]);

            //// Verify the text is there
            //test.GeneralMethods.VerifyTextPresentWebDriver(privacyPolicyText);

            //// Back
            //test.Driver.Close();
            ////test.Driver.SwitchTo().Window(test.Driver.WindowHandles[0]);

            // Logout
            test.Infellowship.LogoutWebDriver();

            // Login to Portal
            test.Portal.LoginWebDriver("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to weblink->privacy policy
            test.GeneralMethods.Navigate_Portal(Navigation.WebLink.InFellowship.Privacy);

            // Revert the change
            test.Driver.FindElementById("privacy_policy_text").SendKeys("");
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();

            // Verify the new value is not present
            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(privacyPolicyText), "Privacy Policy was not removed!");

            // Verify the default is there
            Assert.AreEqual("We know that you care how information about you is used and shared. We hope the following statements will help you understand how we will collect, use and protect the information you provide to us on our site. We will not use or share your information with anyone except as described in this Privacy Policy.", test.Driver.FindElementByXPath("//div[@class='box_notice']/p[1]").Text);
            Assert.AreEqual("We may collect any information that you provide to us, such as names, e-mail addresses, etc. We also use cookies to store and sometimes track information about our users.", test.Driver.FindElementByXPath("//div[@class='box_notice']/p[2]").Text);
            Assert.AreEqual("We use the personal information that you submit to operate, maintain and provide you the features and functionality of this website and support the mission of the church.", test.Driver.FindElementByXPath("//div[@class='box_notice']/p[3]").Text);
            Assert.AreEqual("We will never share, rent or sell your personal information without your permission, unless required to do so by law or by subpoena. We take appropriate security measures to protect against unauthorized access to or unauthorized alteration, disclosure or destruction of data.", test.Driver.FindElementByXPath("//div[@class='box_notice']/p[4]").Text);
            Assert.AreEqual("We process personal information only for the purposes for which it was collected and in accordance with this Privacy Policy. We take reasonable steps to ensure that the personal information we process is accurate, complete, and current, but we depend on our users to update or correct their personal information whenever necessary.", test.Driver.FindElementByXPath("//div[@class='box_notice']/p[5]").Text);


            // Logout of portal
            test.Portal.LogoutWebDriver();

            // Open InFellowship
            test.Infellowship.OpenWebDriver("QAEUNLX0C2");

            // View the privacy policy
            test.Driver.FindElementByLinkText("Church Privacy Policy").Click();
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]);

            // Verify the text is not there
            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(privacyPolicyText), "Privacy Policy was not removed!");

            // Verify the default is there
            Assert.AreEqual("We know that you care how information about you is used and shared. We hope the following statements will help you understand how we will collect, use and protect the information you provide to us on our site. We will not use or share your information with anyone except as described in this Privacy Policy.", test.Driver.FindElementByXPath("//div[@class='grid_16']/p[1]").Text);
            Assert.AreEqual("We may collect any information that you provide to us, such as names, e-mail addresses, etc. We also use cookies to store and sometimes track information about our users.", test.Driver.FindElementByXPath("//div[@class='grid_16']/p[2]").Text);
            Assert.AreEqual("We use the personal information that you submit to operate, maintain and provide you the features and functionality of this website and support the mission of the church.", test.Driver.FindElementByXPath("//div[@class='grid_16']/p[3]").Text);
            Assert.AreEqual("We will never share, rent or sell your personal information without your permission, unless required to do so by law or by subpoena. We take appropriate security measures to protect against unauthorized access to or unauthorized alteration, disclosure or destruction of data.", test.Driver.FindElementByXPath("//div[@class='grid_16']/p[4]").Text);
            Assert.AreEqual("We process personal information only for the purposes for which it was collected and in accordance with this Privacy Policy. We take reasonable steps to ensure that the personal information we process is accurate, complete, and current, but we depend on our users to update or correct their personal information whenever necessary.", test.Driver.FindElementByXPath("//div[@class='grid_16']/p[5]").Text);

            // Back
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[0]);

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // View the privacy policy
            test.Driver.FindElementByLinkText("Church Privacy Policy").Click();
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]);

            // Verify the text is not there
            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(privacyPolicyText), "Privacy Policy was not removed!");

            // Verify the default is there
            Assert.AreEqual("We know that you care how information about you is used and shared. We hope the following statements will help you understand how we will collect, use and protect the information you provide to us on our site. We will not use or share your information with anyone except as described in this Privacy Policy.", test.Driver.FindElementByXPath("//div[@class='grid_16']/p[1]").Text);
            Assert.AreEqual("We may collect any information that you provide to us, such as names, e-mail addresses, etc. We also use cookies to store and sometimes track information about our users.", test.Driver.FindElementByXPath("//div[@class='grid_16']/p[2]").Text);
            Assert.AreEqual("We use the personal information that you submit to operate, maintain and provide you the features and functionality of this website and support the mission of the church.", test.Driver.FindElementByXPath("//div[@class='grid_16']/p[3]").Text);
            Assert.AreEqual("We will never share, rent or sell your personal information without your permission, unless required to do so by law or by subpoena. We take appropriate security measures to protect against unauthorized access to or unauthorized alteration, disclosure or destruction of data.", test.Driver.FindElementByXPath("//div[@class='grid_16']/p[4]").Text);
            Assert.AreEqual("We process personal information only for the purposes for which it was collected and in accordance with this Privacy Policy. We take reasonable steps to ensure that the personal information we process is accurate, complete, and current, but we depend on our users to update or correct their personal information whenever necessary.", test.Driver.FindElementByXPath("//div[@class='grid_16']/p[5]").Text);

            // Back
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[0]);

            // Logout
            test.Infellowship.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Updates the InFellowship Privacy Policy for the church using special characters.")]
        public void WebLink_InFellowship_Privacy_Policy_Update_Special_Characters()
        {
            // Variables
            var privacyPolicyText = "<b><%=hell0%></b>";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to weblink->privacy policy
            test.GeneralMethods.Navigate_Portal(Navigation.WebLink.InFellowship.Privacy);

            // Overwrite the current value
            test.Driver.FindElementById("privacy_policy_text").SendKeys(privacyPolicyText);
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();
            test.GeneralMethods.VerifyTextPresentWebDriver(privacyPolicyText);

            // Logout of portal
            test.Portal.LogoutWebDriver();

            // Open InFellowship
            test.Infellowship.OpenWebDriver("dc");

            // View the privacy policy
            test.Driver.FindElementByLinkText("Church Privacy Policy").Click();
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]);

            // Verify the text is there
            test.GeneralMethods.VerifyTextPresentWebDriver(privacyPolicyText);

            // Back
            test.Driver.Close();
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[0]);

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

            // View the privacy policy
            test.Driver.FindElementByLinkText("Church Privacy Policy").Click();
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]);

            // Verify the text is there
            test.GeneralMethods.VerifyTextPresentWebDriver(privacyPolicyText);

            // Back
            test.Driver.Close();
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[0]);

            // Logout
            test.Infellowship.LogoutWebDriver();

            // Login to Portal
            test.Portal.LoginWebDriver();

            // Navigate to weblink->privacy policy
            test.GeneralMethods.Navigate_Portal(Navigation.WebLink.InFellowship.Privacy);

            // Revert the change
            test.Driver.FindElementById("privacy_policy_text").SendKeys("");
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();

            // Verify the new value is not present
            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(privacyPolicyText), "Privacy Policy was not removed!");

            // Verify the default is there
            Assert.AreEqual("We know that you care how information about you is used and shared. We hope the following statements will help you understand how we will collect, use and protect the information you provide to us on our site. We will not use or share your information with anyone except as described in this Privacy Policy.", test.Driver.FindElementByXPath("//div[@class='box_notice']/p[1]").Text);
            Assert.AreEqual("We may collect any information that you provide to us, such as names, e-mail addresses, etc. We also use cookies to store and sometimes track information about our users.", test.Driver.FindElementByXPath("//div[@class='box_notice']/p[2]").Text);
            Assert.AreEqual("We use the personal information that you submit to operate, maintain and provide you the features and functionality of this website and support the mission of the church.", test.Driver.FindElementByXPath("//div[@class='box_notice']/p[3]").Text);
            Assert.AreEqual("We will never share, rent or sell your personal information without your permission, unless required to do so by law or by subpoena. We take appropriate security measures to protect against unauthorized access to or unauthorized alteration, disclosure or destruction of data.", test.Driver.FindElementByXPath("//div[@class='box_notice']/p[4]").Text);
            Assert.AreEqual("We process personal information only for the purposes for which it was collected and in accordance with this Privacy Policy. We take reasonable steps to ensure that the personal information we process is accurate, complete, and current, but we depend on our users to update or correct their personal information whenever necessary.", test.Driver.FindElementByXPath("//div[@class='box_notice']/p[5]").Text);

            // Logout of portal
            test.Portal.LogoutWebDriver();

            // Open InFellowship
            test.Infellowship.OpenWebDriver("dc");

            // View the privacy policy
            test.Driver.FindElementByLinkText("Church Privacy Policy").Click();
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]);

            // Verify the text is not there
            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(privacyPolicyText), "Privacy Policy was not removed!");

            // Verify the default is there
            Assert.AreEqual("We know that you care how information about you is used and shared. We hope the following statements will help you understand how we will collect, use and protect the information you provide to us on our site. We will not use or share your information with anyone except as described in this Privacy Policy.", test.Driver.FindElementByXPath("//div[@class='grid_16']/p[1]").Text);
            Assert.AreEqual("We may collect any information that you provide to us, such as names, e-mail addresses, etc. We also use cookies to store and sometimes track information about our users.", test.Driver.FindElementByXPath("//div[@class='grid_16']/p[2]").Text);
            Assert.AreEqual("We use the personal information that you submit to operate, maintain and provide you the features and functionality of this website and support the mission of the church.", test.Driver.FindElementByXPath("//div[@class='grid_16']/p[3]").Text);
            Assert.AreEqual("We will never share, rent or sell your personal information without your permission, unless required to do so by law or by subpoena. We take appropriate security measures to protect against unauthorized access to or unauthorized alteration, disclosure or destruction of data.", test.Driver.FindElementByXPath("//div[@class='grid_16']/p[4]").Text);
            Assert.AreEqual("We process personal information only for the purposes for which it was collected and in accordance with this Privacy Policy. We take reasonable steps to ensure that the personal information we process is accurate, complete, and current, but we depend on our users to update or correct their personal information whenever necessary.", test.Driver.FindElementByXPath("//div[@class='grid_16']/p[5]").Text);

            // Back
            test.Driver.Close();
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[0]);

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

            // View the privacy policy
            test.Driver.FindElementByLinkText("Church Privacy Policy").Click();
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[1]);

            // Verify the text is not there
            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(privacyPolicyText), "Privacy Policy was not removed!");

            // Verify the default is there
            Assert.AreEqual("We know that you care how information about you is used and shared. We hope the following statements will help you understand how we will collect, use and protect the information you provide to us on our site. We will not use or share your information with anyone except as described in this Privacy Policy.", test.Driver.FindElementByXPath("//div[@class='grid_16']/p[1]").Text);
            Assert.AreEqual("We may collect any information that you provide to us, such as names, e-mail addresses, etc. We also use cookies to store and sometimes track information about our users.", test.Driver.FindElementByXPath("//div[@class='grid_16']/p[2]").Text);
            Assert.AreEqual("We use the personal information that you submit to operate, maintain and provide you the features and functionality of this website and support the mission of the church.", test.Driver.FindElementByXPath("//div[@class='grid_16']/p[3]").Text);
            Assert.AreEqual("We will never share, rent or sell your personal information without your permission, unless required to do so by law or by subpoena. We take appropriate security measures to protect against unauthorized access to or unauthorized alteration, disclosure or destruction of data.", test.Driver.FindElementByXPath("//div[@class='grid_16']/p[4]").Text);
            Assert.AreEqual("We process personal information only for the purposes for which it was collected and in accordance with this Privacy Policy. We take reasonable steps to ensure that the personal information we process is accurate, complete, and current, but we depend on our users to update or correct their personal information whenever necessary.", test.Driver.FindElementByXPath("//div[@class='grid_16']/p[5]").Text);

            // Back
            test.Driver.Close();
            test.Driver.SwitchTo().Window(test.Driver.WindowHandles[0]);

            // Logout
            test.Infellowship.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the privacy policy cannot exceed 6000 characters")]
        public void WebLink_InFellowship_Privacy_Policy_Update_Cannot_Exceed_6000_Characters()
        {
            // Variables
            StringBuilder privacyPolicyText = new StringBuilder("This is my really long privacy policy that is long");
            // Build an 6000 character long string
            for (int i = 0; i < 500; i++)
            {
                privacyPolicyText.Append(" This is my really long string");
            }

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("cgutekunst", "CG.Admin09", "QAEUNLX0C1");

            // Navigate to weblink->privacy policy
            test.GeneralMethods.Navigate_Portal(Navigation.WebLink.InFellowship.Privacy);

            // Overwrite the current value
            test.Driver.FindElementById("privacy_policy_text").SendKeys(privacyPolicyText.ToString());
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();

            test.GeneralMethods.VerifyTextPresentWebDriver("Privacy Policy cannot exceed 6000 characters.");

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }
        #endregion Privacy Policy

    }

    [TestFixture]
    [Importance(Importance.Critical)]
    public class Portal_WebLink_InFellowship : FixtureBase
    {
        #region Branding

        #region Color Theme
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that selecting each radio button for Color theme changes the preview")]
        public void WebLink_InFellowship_Branding_Color_Theme_Preview_Updates_Correctly()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("weblinkuser", "BM.Admin09", "QAEUNLX0C2");

            // View the Branding Page
            test.Selenium.Navigate(Navigation.WebLink.InFellowship.Branding);

            // Change the color theme to each value and verify the preview changes correctly
            test.Selenium.Click("radio_theme_blue_light");
            test.Selenium.VerifyElementPresent("css=div.theme_blue_light");

            test.Selenium.Click("radio_theme_blue_dark");
            test.Selenium.VerifyElementPresent("css=div.theme_blue_dark");

            test.Selenium.Click("radio_theme_green_light");
            test.Selenium.VerifyElementPresent("css=div.theme_green_light");

            test.Selenium.Click("radio_theme_green_dark");
            test.Selenium.VerifyElementPresent("css=div.theme_green_dark");

            test.Selenium.Click("radio_theme_red_light");
            test.Selenium.VerifyElementPresent("css=div.theme_red_light");

            test.Selenium.Click("radio_theme_red_dark");
            test.Selenium.VerifyElementPresent("css=div.theme_red_dark");

            test.Selenium.Click("radio_theme_brown_light");
            test.Selenium.VerifyElementPresent("css=div.theme_brown_light");

            test.Selenium.Click("radio_theme_brown_dark");
            test.Selenium.VerifyElementPresent("css=div.theme_brown_dark");

            test.Selenium.Click("radio_theme_gray");
            test.Selenium.VerifyElementPresent("css=div.theme_gray");

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies picking a branding color updates InFellowship")]
        public void WebLink_InFellowship_Branding_Color_Theme_Applies_To_InFellowship()
        {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("weblinkuser", "BM.Admin09", "QAEUNLX0C2");

            // Update the Branding
            test.Portal.WebLink_Branding_Update_ColorTheme(GeneralEnumerations.InFellowshipBrandingColors.blue_light);

            // Logout of Portal
            test.Portal.Logout();


            // Verify in InFellowship
            test.infellowship.Open();
            test.Selenium.VerifyElementPresent("css=body.theme_blue_light");


            // Login to Portal
            test.Portal.Login("weblinkuser", "BM.Admin09", "QAEUNLX0C2");

            // Update the Branding
            test.Portal.WebLink_Branding_Update_ColorTheme(GeneralEnumerations.InFellowshipBrandingColors.blue_dark);

            // Logout of Portal
            test.Portal.Logout();


            // Verify in InFellowship
            test.infellowship.Open();
            test.Selenium.VerifyElementPresent("css=body.theme_blue_dark");


            // Login to Portal
            test.Portal.Login("weblinkuser", "BM.Admin09", "QAEUNLX0C2");

            // Update the Branding
            test.Portal.WebLink_Branding_Update_ColorTheme(GeneralEnumerations.InFellowshipBrandingColors.green_light);

            // Logout of Portal
            test.Portal.Logout();


            // Verify in InFellowship
            test.infellowship.Open();
            test.Selenium.VerifyElementPresent("css=body.theme_green_light");


            // Login to Portal
            test.Portal.Login("weblinkuser", "BM.Admin09", "QAEUNLX0C2");

            // Update the Branding
            test.Portal.WebLink_Branding_Update_ColorTheme(GeneralEnumerations.InFellowshipBrandingColors.green_dark);

            // Logout of Portal
            test.Portal.Logout();


            // Verify in InFellowship
            test.infellowship.Open();
            test.Selenium.VerifyElementPresent("css=body.theme_green_dark");


            // Login to Portal
            test.Portal.Login("weblinkuser", "BM.Admin09", "QAEUNLX0C2");

            // Update the Branding
            test.Portal.WebLink_Branding_Update_ColorTheme(GeneralEnumerations.InFellowshipBrandingColors.red_light);

            // Logout of Portal
            test.Portal.Logout();


            // Verify in InFellowship
            test.infellowship.Open();

            // Login to Portal
            test.Portal.Login("weblinkuser", "BM.Admin09", "QAEUNLX0C2");

            // Update the Branding
            test.Portal.WebLink_Branding_Update_ColorTheme(GeneralEnumerations.InFellowshipBrandingColors.red_dark);

            // Logout of Portal
            test.Portal.Logout();


            // Verify in InFellowship
            test.infellowship.Open();
            test.Selenium.VerifyElementPresent("css=body.theme_red_dark");


            // Login to Portal
            test.Portal.Login("weblinkuser", "BM.Admin09", "QAEUNLX0C2");

            // Update the Branding
            test.Portal.WebLink_Branding_Update_ColorTheme(GeneralEnumerations.InFellowshipBrandingColors.brown_light);

            // Logout of Portal
            test.Portal.Logout();


            // Verify in InFellowship
            test.infellowship.Open();
            test.Selenium.VerifyElementPresent("css=body.theme_brown_light");

            // Login to Portal
            test.Portal.Login("weblinkuser", "BM.Admin09", "QAEUNLX0C2");

            // Update the Branding
            test.Portal.WebLink_Branding_Update_ColorTheme(GeneralEnumerations.InFellowshipBrandingColors.brown_dark);

            // Logout of Portal
            test.Portal.Logout();


            // Verify in InFellowship
            test.infellowship.Open();
            test.Selenium.VerifyElementPresent("css=body.theme_brown_dark");


            // Login to Portal
            test.Portal.Login("weblinkuser", "BM.Admin09", "QAEUNLX0C2");

            // Update the Branding
            test.Portal.WebLink_Branding_Update_ColorTheme(GeneralEnumerations.InFellowshipBrandingColors.gray);

            // Logout of Portal
            test.Portal.Logout();


            // Verify in InFellowship
            test.infellowship.Open();
            test.Selenium.VerifyElementPresent("css=body.theme_gray");
        }
        #endregion Color Theme

        #region Custom Content
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the second section of custom content has proper validation.")]
        public void WebLink_InFellowship_Branding_Custom_Content_Section_Two_Miscellaneous_Cannot_Exceed_100_Characters()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View the branding page, specifically custom content
            test.Portal.WebLink_Branding_View_Custom_Content();

            // Enter an amount of characeters greater than 100 for section 2
            test.Selenium.ClickAndWaitForCondition("css=tbody > tr:last-child > td > input[name=inf_branding_radio_grp_3]", JavaScriptMethods.IsElementPresent("inf_church_address"), "5000");
            test.Selenium.Type("inf_church_address", "Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters. Over 100 characters.");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify validation message
            Assert.IsTrue(test.Selenium.IsTextPresent("Miscellaneous cannot exceed 100 characters."));

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the third section of custom content has proper validation.")]
        public void WebLink_InFellowship_Branding_Custom_Content_Section_Three_Miscellaneou_Cannot_Exceed_6000_Characters()
        {
            string bigText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi in consequat dui. Morbi aliquet massa ipsum, vel pulvinar dolor congue vel. Curabitur aliquet sem lectus, id posuere orci mollis ac. Mauris eget erat massa. Sed id neque iaculis, ultricies nibh egestas, feugiat orci. Cras sit amet tempus est, eu scelerisque neque. Mauris placerat molestie dignissim. Phasellus sollicitudin eros turpis.Donec mauris risus, porta ut suscipit quis, pharetra in velit. Mauris vel elementum ipsum. Suspendisse vel nulla vel lectus accumsan fringilla. Vestibulum quis est sit amet nibh ullamcorper dapibus ut vel lectus. Aenean ligula neque, bibendum a odio ut, condimentum sollicitudin odio. Pellentesque volutpat tincidunt nulla, hendrerit rutrum nunc congue ut. In ultricies fermentum risus et sagittis. In et ullamcorper lorem, ac gravida lorem. Donec at euismod felis.Phasellus porta eleifend dapibus. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam sit amet orci id massa mattis ultricies. Ut lobortis velit a aliquet viverra. Morbi ut nulla non tortor euismod eleifend sed quis enim. Vestibulum malesuada a risus eget blandit. Morbi id elit in velit tempus consectetur vitae a justo. Aliquam pharetra risus libero, sit amet condimentum elit dapibus vel. In ullamcorper diam semper, auctor diam varius, vehicula augue. Donec massa libero, vulputate ac metus ut, rutrum dictum nibh. Integer euismod tempus velit quis consequat. Vivamus eleifend dui vel nisi ultricies, vitae faucibus leo adipiscing. Phasellus interdum odio nisi, quis hendrerit arcu pharetra ac.Aenean mattis lorem turpis, vitae euismod orci gravida eu. Quisque et interdum ligula. Ut viverra nunc eros, vel imperdiet metus viverra nec. Praesent adipiscing et lorem non condimentum. Sed nec massa lacinia felis ultricies molestie. Vestibulum et tortor suscipit, vulputate est eget, feugiat purus. Pellentesque eget hendrerit mauris. Phasellus euismod augue est, non sodales sem vehicula a. Morbi magna libero, adipiscing a convallis quis, sollicitudin nec ipsum. Fusce vestibulum a nibh et ultrices. Sed ligula ligula, cursus vitae feugiat eu, semper in ante. Curabitur nec elit tincidunt, blandit tellus ac, imperdiet est. Nam auctor eleifend turpis, quis tempor felis sodales id. Etiam in lobortis velit.Nullam egestas urna suscipit porta accumsan. Sed ornare interdum nisi, at laoreet dui iaculis vitae. Nullam eu sapien congue, egestas metus vitae, imperdiet tellus. Nulla quis lectus cursus, auctor sem in, eleifend erat. Ut gravida arcu at bibendum pulvinar. Donec orci sem, mollis eu iaculis egestas, dictum sit amet massa. In neque neque, semper at arcu eget, consectetur volutpat lacus. Phasellus dapibus arcu mauris, sed interdum est ornare tincidunt. Nullam sed orci quis libero dignissim ullamcorper nec nec libero. Etiam vehicula interdum nisl non elementum. Mauris non posuere dolor. Etiam a pharetra mauris, a luctus elit. Donec a leo ut enim eleifend malesuada ac ac enim.Mauris quis viverra quam, sed viverra ipsum. In placerat sodales imperdiet. Phasellus posuere ipsum mi, in bibendum mauris scelerisque luctus. Aliquam sed consectetur tellus, in ornare ante. Sed vitae commodo ligula, et varius velit. Ut fermentum facilisis quam id imperdiet. Integer porta lorem ac lacus euismod, quis vestibulum nisi porta. Praesent egestas dui sem, quis vulputate nulla lacinia vitae. Nunc egestas blandit mauris non dictum. Nunc scelerisque, dolor vitae auctor hendrerit, lectus risus volutpat dolor, eget rhoncus purus tortor ut libero. Etiam a consectetur ligula. Praesent ultricies tortor ut risus iaculis gravida. Mauris tortor lectus, hendrerit quis gravida mollis, ullamcorper tristique tellus. Suspendisse potenti. Sed ullamcorper massa porta arcu cursus elementum.Vivamus ut auctor mauris. Morbi ultrices, purus vel condimentum tempor, elit purus elementum lorem, sit amet eleifend est leo id diam. Vivamus quis sem molestie, tincidunt eros id, condimentum lorem. Nullam non tincidunt diam, eget aliquam augue. Morbi quis dictum augue. Donec in pharetra ante. Pellentesque nec tellus eget tortor aliquam luctus vitae id nunc. Aenean nec lectus sed eros feugiat vestibulum. Nullam ullamcorper ultrices enim, et ornare leo iaculis sed. Nam mattis pellentesque viverra. Ut sagittis turpis non quam congue, a venenatis ipsum tristique. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae;Etiam aliquet risus quis diam ultricies, nec vulputate dui pharetra. Donec et laoreet lorem. Nulla sollicitudin ultricies arcu eget egestas. Donec sollicitudin egestas dapibus. Pellentesque pellentesque sed ante et faucibus. Donec ac gravida ligula. Quisque convallis orci quis dictum vulputate. Sed varius dapibus urna. Vivamus iaculis egestas ornare. Donec sagittis, sem ut fringilla tincidunt, ipsum lectus rutrum lectus, id porttitor libero nisl non orci. Integer vehicula placerat nulla quis gravida. In dapibus lorem massa, eget facilisis tortor dictum eget. Vestibulum vel nisi lectus. Vivamus malesuada ipsum in arcu venenatis, vitae varius sem fermentum.Sed eget placerat massa. Vivamus tincidunt erat ac augue faucibus dapibus. Sed quis elit dictum, euismod ipsum ac, pulvinar tortor. Pellentesque feugiat ipsum libero, sed porttitor nibh consequat condimentum. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Morbi iaculis lectus ut ipsum facilisis aliquet. Pellentesque lorem tellus, imperdiet pellentesque ipsum a, volutpat convallis lectus. Integer ultricies mattis ultrices. Integer tortor quam, semper tempor egestas vel, congue in est. Donec ut feugiat mi. Nam mattis neque erat, malesuada tempor lectus iaculis ut. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Donec vel nulla placerat, ultricies odio ac, porta dui. Curabitur porta non sapien ut viverra.Nunc rhoncus suscipit enim et condimentum. Duis ac feugiat odio. Quisque vitae justo id nisi tempor congue a at urna. Vivamus sed.Nunc rhoncus suscipit enim et condimentum. Duis ac feugiat odio. Quisque vitae justo id nisi tempor congue a at urna. Vivamus sed.  ";
                
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View the branding page, specifically custom content
            test.Portal.WebLink_Branding_View_Custom_Content();

            // Enter an amount of characeters greater than 6000 for section 3
            test.Selenium.ClickAndWaitForCondition("css=tbody > tr:last-child > td > input[name=inf_branding_radio_grp_2]", JavaScriptMethods.IsElementPresent("inf_footer_misc"), "5000");
            test.Selenium.Type("inf_footer_misc", bigText);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify validation message
            Assert.IsTrue(test.Selenium.IsTextPresent("Miscellaneous cannot exceed 6000 characters."));

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the second section of custom content is shown in InFellowship when text is provided")]
        public void WebLink_InFellowship_Branding_Custom_Content_Section_Two_Miscellaneous_Visible_In_InFellowship()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Update section two's miscellaneous' text.
            test.Portal.WebLink_Branding_Update_SectionTwo_Text("Section Two Text for InFellowship");

            // Logout of portal
            test.Portal.Logout();


            // Open InFellowship and Verify text is present
            test.infellowship.Open("dc");

            // Login Page
            test.Selenium.VerifyTextPresent("Section Two Text for InFellowship");

            // Find a Group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup);

            // Verify on the find a group page
            test.Selenium.VerifyTextPresent("Section Two Text for InFellowship");

            // Login to Infellowship
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd");

            // Verify on the landing page
            test.Selenium.VerifyTextPresent("Section Two Text for InFellowship");

            // View Your Groups
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourGroups);

            // Verify on the group's dashboard
            //Commenting this out until this is fixed for Responsive page
            //test.Selenium.VerifyTextPresent("Section Two Text for InFellowship");

            // Find a group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup);

            // Verify on the find a group page
            test.Selenium.VerifyTextPresent("Section Two Text for InFellowship");

            // Logout of infellowship
            test.infellowship.Logout();


            // Revert the change
            // Login to portal
            test.Portal.Login();

            // Update section two to just show the default.
            test.Portal.WebLink_Branding_Update_SectionTwo_SetToDefault();

            // Logout of portal
            test.Portal.Logout();


            // Open InFellowship and Verify text is present
            test.infellowship.Open("dc");

            // Login Page
            Assert.IsFalse(test.Selenium.IsTextPresent("Section Two Text for InFellowship"));

            // Find a Group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup);

            // Verify on the find a group page
            Assert.IsFalse(test.Selenium.IsTextPresent("Section Two Text for InFellowship"));

            // Login
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd");

            // Verify on the landing page
            Assert.IsFalse(test.Selenium.IsTextPresent("Section Two Text for InFellowship"));

            // View Your Groups
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourGroups);

            // Verify on the group's dashboard
            Assert.IsFalse(test.Selenium.IsTextPresent("Section Two Text for InFellowship"));

            // Find a group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup);

            // Verify on the find a group page
            Assert.IsFalse(test.Selenium.IsTextPresent("Section Two Text for InFellowship"));

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the third section of custom content is shown in InFellowship when text is provided")]
        public void WebLink_InFellowship_Branding_Custom_Content_Section_Three_Miscellaneous_Visible_In_InFellowship()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Update section three's miscellaneous' text.
            test.Portal.WebLink_Branding_Update_SectionThree_Text("Section Three Text for InFellowship");

            // Logout of portal
            test.Portal.Logout();


            // Open InFellowship and Verify text is present
            test.infellowship.Open("dc");

            // Login Page
            test.Selenium.VerifyTextPresent("Section Three Text for InFellowship");

            // Find a Group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup);

            // Verify on the find a group page
            test.Selenium.VerifyTextPresent("Section Three Text for InFellowship");

            // Login
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd");

            // Verify on the landing page
            test.Selenium.VerifyTextPresent("Section Three Text for InFellowship");

            // View Your Groups
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourGroups);

            // Verify on the group's dashboard
            //Commenting this out until this is fixed with Responsive page
            //test.Selenium.VerifyTextPresent("Section Three Text for InFellowship");

            // Find a group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup);

            // Verify on the find a group page
            test.Selenium.VerifyTextPresent("Section Three Text for InFellowship");

            // Logout
            test.infellowship.Logout();


            // Revert the change
            // Login to Portal
            test.Portal.Login();

            // Update section three to just show the default.
            test.Portal.WebLink_Branding_Update_SectionThree_SetToDefault();

            // Logout
            test.Portal.Logout();


            // Open InFellowship and Verify text is present
            test.infellowship.Open("dc");

            // Login Page
            Assert.IsFalse(test.Selenium.IsTextPresent("Section Three Text for InFellowship"));

            // Find a Group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup);

            // Verify on the find a group page
            Assert.IsFalse(test.Selenium.IsTextPresent("Section Three Text for InFellowship"));

            // Login
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd");

            // Verify on the landing page
            Assert.IsFalse(test.Selenium.IsTextPresent("Section Three Text for InFellowship"));

            // View Your Groups
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourGroups);

            // Verify on the group's dashboard
            Assert.IsFalse(test.Selenium.IsTextPresent("Section Three Text for InFellowship"));

            // Find a group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup);

            // Verify on the find a group page
            Assert.IsFalse(test.Selenium.IsTextPresent("Section Three Text for InFellowship"));

            // Logout of infellowship
            test.infellowship.Logout();
        }
        #endregion Custom Content

        #endregion Branding

        #region Church Directory
        [Test, RepeatOnFailure(Order = 1), MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies turning off the ability to view household members in the directory hides that section in InFellowship.")]
        public void Weblink_InFellowship_Features_Disable_ChurchDirectory_Household_Members()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.InFellowshipChurchCode = "QAEUNLX0C1";
            test.Portal.Login("msneeden", "Pa$$w0rd", "QAEUNLX0C1");

            // Disable Household members in the church directory
            test.Portal.WebLink_Toggle_InFellowship_Feature("Church Directory", true, true, new Dictionary<string, bool>() { { "Display household members when viewing a person", false } });

            // Logout of portal
            test.Portal.Logout();


            // Login to infellowship
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "QAEUNLX0C1");

            // Verify household members are not present.
            test.Selenium.ClickAndWaitForPageToLoad("link=Church Directory");
            test.infellowship.People_ChurchDirectory_View_Individual("Matthew Sneeden");
            test.Selenium.VerifyElementNotPresent("//div[@class='grid_5']/h6");  // Household text header
            test.Selenium.VerifyElementNotPresent("//div[@class='grid_5']/ul");  // Household member's list

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure(Order = 2), MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies turning on the ability to view household members in the directory shows that section in InFellowship.")]
        public void Weblink_InFellowship_Features_Enable_ChurchDirectory_Household_Members()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.InFellowshipChurchCode = "QAEUNLX0C1";
            test.Portal.Login("msneeden", "Pa$$w0rd", "QAEUNLX0C1");

            // Enable household members in the church directory
            test.Portal.WebLink_Toggle_InFellowship_Feature("Church Directory", true, true, new Dictionary<string, bool>() { { "Display household members when viewing a person", true } });

            // Logout of portal
            test.Portal.Logout();


            // Login to infellowship
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "QAEUNLX0C1");

            // Verify household members are present.
            test.Selenium.ClickAndWaitForPageToLoad("link=Church Directory");
            test.infellowship.People_ChurchDirectory_View_Individual("Matthew Sneeden");
            test.Selenium.VerifyElementPresent("//div[@class='grid_5']/h6");  // Household text header
            test.Selenium.VerifyElementPresent("//div[@class='grid_5']/ul");  // Household member's list

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure(Order = 3), MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies turning off Church Directory disables Church Directory in InFellowship.")]
        public void Weblink_InFellowship_Features_Disable_ChurchDirectory()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("msneeden", "Pa$$w0rd", "QAEUNLX0C1");

            // Disable church directory
            test.Portal.WebLink_Toggle_InFellowship_Feature("Church Directory", false);

            // Verify the links page updates
            test.Selenium.ClickAndWaitForPageToLoad("link=Links");
            test.Selenium.VerifyElementNotPresent("//tbody/tr[7]/td[2]/span/a");

            // Logout of portal
            test.Portal.Logout();


            // Login to infellowship
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "QAEUNLX0C1");

            // Verify the church directory is not visible.
            test.Selenium.VerifyElementNotPresent(GeneralInFellowshipConstants.GeneralLinks.Link_Directory);
            test.Selenium.VerifyElementNotPresent(GeneralInFellowshipConstants.LandingPage.Link_ChurchDirectory);

            // Check privacy settings to verify you cannot opt in to the directory.
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourProfile);
            test.Selenium.ClickAndWaitForPageToLoad(InFellowshipConstants.People.ProfileEditorConstants.Link_PrivacySettings);

            test.Selenium.VerifyElementNotPresent(InFellowshipConstants.People.PrivacySettingsConstants.Checkbox_IncludeInDirectory);
            Assert.IsFalse(test.Selenium.IsTextPresent("Include me in the church directory – This includes your name, city, state, zip, and info marked Everyone."));

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure(Order = 4), MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies turning on Church Directory enables Church Directory in InFellowship.")]
        public void Weblink_InFellowship_Features_Enable_ChurchDirectory()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.InFellowshipChurchCode = "QAEUNLX0C1";
            test.Portal.Login("msneeden", "Pa$$w0rd", "QAEUNLX0C1");

            // Enable church directory
            test.Portal.WebLink_Toggle_InFellowship_Feature("Church Directory", true, true, new Dictionary<string, bool>() { { "Display household members when viewing a person", false } });

            // Verify the Links page updates
            test.Selenium.ClickAndWaitForPageToLoad("link=Links");
            Assert.IsTrue(test.Selenium.IsElementPresent("//table/tbody/tr[*]/td[position()=1 and normalize-space(text())='Church directory']/following-sibling::td/span/a"));

            // Logout of portal
            test.Portal.Logout();


            // Login to infellowship
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "QAEUNLX0C1");

            // Verify the church directory is visible.
            test.Selenium.VerifyElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_Directory);
            test.Selenium.VerifyElementPresent(GeneralInFellowshipConstants.LandingPage.Link_ChurchDirectory);

            // Check privacy settings to verify you can opt in to the directory.
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourProfile);
            test.Selenium.ClickAndWaitForPageToLoad(InFellowshipConstants.People.ProfileEditorConstants.Link_PrivacySettings);

            test.Selenium.VerifyElementPresent(InFellowshipConstants.People.PrivacySettingsConstants.Checkbox_IncludeInDirectory);
            Assert.IsTrue(test.Selenium.IsTextPresent("Include me in the church directory – This includes your name, city, state, and info marked Everyone."));

            // Logout of InFellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies Inactive statuses under the status group are not present.")]
        public void WebLink_InFellowship_Features_ChurchDirectory_Inactive_Status_Not_Present()
        {
            // Set initial conditions
            var memberStatusName = "Automation Inactive Member";
            var attendeeStatusName = "Automation Inactive Attendee";
            base.SQL.Admin_Status_Delete(15, memberStatusName);
            base.SQL.Admin_Status_Delete(15, attendeeStatusName);
            base.SQL.Admin_Status_Create(15, "Member", memberStatusName, false);
            base.SQL.Admin_Status_Create(15, "Attendee", attendeeStatusName, false);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            try
            {
                test.Portal.Login();

                // View the church directory feature page
                test.Selenium.Navigate(Navigation.WebLink.InFellowship.Features);

                // Verify the inactive statuses are not there
                Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.WebLink_InFellowship_ChurchDirectory_MemberStatusTable, memberStatusName, "Member"), "Inactive status was present!");
                Assert.IsFalse(test.GeneralMethods.ItemExistsInTable(TableIds.WebLink_InFellowship_ChurchDirectory_AttendeeStatusTable, attendeeStatusName, "Attendee"), "Inactive status was present!");
            }
            finally
            {
                // Delete the statuses
                base.SQL.Admin_Status_Delete(15, memberStatusName);
                base.SQL.Admin_Status_Delete(15, attendeeStatusName);

                // Logout of portal
                test.Portal.Logout();
            }
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies new statuses under the status group are present.")]
        public void WebLink_InFellowship_Features_ChurchDirectory_New_Statuses_Present()
        {
            // Set initial conditions
            var memberStatusName = "Automation New Member";
            var attendeeStatusName = "Automation New Attendee";
            base.SQL.Admin_Status_Create(15, "Member", memberStatusName, true);
            base.SQL.Admin_Status_Create(15, "Attendee", attendeeStatusName, true);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // View the church directory feature page
            test.Selenium.Navigate(Navigation.WebLink.InFellowship.Features);

            // Verify the new statuses are there
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.WebLink_InFellowship_ChurchDirectory_MemberStatusTable, memberStatusName, "Member"), "New member status was not present!");
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.WebLink_InFellowship_ChurchDirectory_AttendeeStatusTable, attendeeStatusName, "Attendee"), "New attendee status was not present!");

            // Delete the statuses
            base.SQL.Admin_Status_Delete(15, memberStatusName);
            base.SQL.Admin_Status_Delete(15, attendeeStatusName);

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Church Directory

        #region Groups
        [Test, RepeatOnFailure(Order = 1), MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies turning off Group Search disables Find a Group.")]
        public void Weblink_InFellowship_Features_Disable_Group_Search()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("QAEUNLX0C1");

            // Disable group search
            test.Portal.WebLink_Toggle_InFellowship_Feature("Groups", true, true, new Dictionary<string, bool>() { { "Enable group search for InFellowship", false } });

            // Verify the Link's page updates
            test.Selenium.ClickAndWaitForPageToLoad("link=Links");
            Assert.IsFalse(test.Selenium.IsElementPresent("//tbody/tr[5]/td[2]/span/a"));
            Assert.IsFalse(test.Selenium.IsElementPresent("//tbody/tr[6]/td[2]/span/a"));
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr[5]/td[2]/span"), string.Format("{0}groupsearch/show", test.infellowship.URL.Replace("QAEUNLX0C2", "QAEUNLX0C1").ToLower()));
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr[6]/td[2]/span"), string.Format("{0}groupsearch/index", test.infellowship.URL.Replace("QAEUNLX0C2", "QAEUNLX0C1").ToLower()));
           
            // Logout of portal
            test.Portal.Logout();


            // Open inpellowship
            test.infellowship.Open("QAEUNLX0C1");

            // Verify Find a Group is not possible
            Assert.IsFalse(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup));

            // Login to infellowship
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "QAEUNLX0C1");

            // Verify Find a Group is not possible
            Assert.IsFalse(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup));
            Assert.IsFalse(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.LandingPage.Link_FindAGroup));

            // View a group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourGroups);
            test.Selenium.ClickAndWaitForPageToLoad("link=Automation One");

            // Verify the Find a Group gear menu option is not present
            Assert.IsFalse(test.Selenium.IsElementPresent("//span[@class='nav_gear_menu_find']"));

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure(Order = 2), MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies turning on Group Search enables Find a Group.")]
        public void Weblink_InFellowship_Features_Enable_Group_Search()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("QAEUNLX0C1");

            // Enable group search
            test.Portal.WebLink_Toggle_InFellowship_Feature("Groups", true, true, new Dictionary<string, bool>() { { "Enable group search for InFellowship", true } });

            // Verify the Link's page updates
            test.Selenium.ClickAndWaitForPageToLoad("link=Links");
            //There is '/' at the end of the url
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr[5]/td[2]/span/a"), string.Format("{0}groupsearch/show", test.infellowship.URL.Replace("QAEUNLX0C2", "QAEUNLX0C1").ToLower()));
            Assert.AreEqual(test.Selenium.GetText("//tbody/tr[6]/td[2]/span/a"), string.Format("{0}groupsearch/index", test.infellowship.URL.Replace("QAEUNLX0C2", "QAEUNLX0C1").ToLower()));

            // Logout of portal
            test.Portal.Logout();


            // Open infellowship
            test.infellowship.Open("QAEUNLX0C1");

            // Verify Find a Group is possible
            Assert.IsTrue(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup));

            // Login to infellowship
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "QAEUNLX0C1");

            // Verify Find a Group is possible
            Assert.IsTrue(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup));
            Assert.IsTrue(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.LandingPage.Link_FindAGroup));

            // View a group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourGroups);
            test.Selenium.ClickAndWaitForPageToLoad("link=Automation One");

            // Verify the Find a Group gear menu option is present
            Assert.IsTrue(test.Selenium.IsElementPresent("//span[@class='nav_gear_menu_find']"));

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure(Order = 3), MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies turning off Groups disables Groups in InFellowship.")]
        public void Weblink_InFellowship_Features_Disable_Groups()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("QAEUNLX0C1");

            // Disable groups
            test.Portal.WebLink_Toggle_InFellowship_Feature("Groups", false);

            // Logout of portal
            test.Portal.Logout();


            // Login to portal
            test.Portal.Login("QAEUNLX0C1");

            // Verify Portal->Groups was unchanged.
            Assert.AreEqual("Groups by Group Type", test.Selenium.GetText("//div[@id='nav_sub_13']/dl[1]/dt"));
            Assert.AreEqual("View All", test.Selenium.GetText("//div[@id='nav_sub_13']/dl[1]/dd[1]/a"));

            Assert.AreEqual("Administration", test.Selenium.GetText("//div[@id='nav_sub_13']/dl[2]/dt"));
            Assert.AreEqual("Group Types", test.Selenium.GetText("//div[@id='nav_sub_13']/dl[2]/dd[1]/a"));
            Assert.AreEqual("Custom Fields", test.Selenium.GetText("//div[@id='nav_sub_13']/dl[2]/dd[2]/a"));
            Assert.AreEqual("Search Categories", test.Selenium.GetText("//div[@id='nav_sub_13']/dl[2]/dd[3]/a"));

            // Logout of portal
            test.Portal.Logout();


            // Open infellowship
            test.infellowship.Open("QAEUNLX0C1");

            // Verify Find a Group is not possible
            Assert.IsFalse(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup));

            // Login to infellowship
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "QAEUNLX0C1");

            // Verify Find a Group is not possible
            Assert.IsFalse(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup));
            Assert.IsFalse(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.LandingPage.Link_FindAGroup));

            // Verify Groups cannot be viewed.
            Assert.IsFalse(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.LandingPage.Link_YourGroups));

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure(Order = 4), MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies turning on Groups enables Groups in InFellowship.")]
        public void Weblink_InFellowship_Features_Enable_Groups()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("msneeden", "Pa$$w0rd", "QAEUNLX0C1");

            // Enable groups
            test.Portal.WebLink_Toggle_InFellowship_Feature("Groups", true, true, new Dictionary<string, bool>() { { "Enable group search", false } });

            // Logout of portal
            test.Portal.Logout();


            // Open infellowship
            test.infellowship.Open("QAEUNLX0C1");

            // Verify Find a Group is not possible since we did not turn it on.
            Assert.IsFalse(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup));

            // Login to infellowship
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "QAEUNLX0C1");

            // Verify Find a Group is not possible since we did not turn it on.
            Assert.IsFalse(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.GeneralLinks.Link_FindAGroup));
            Assert.IsFalse(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.LandingPage.Link_FindAGroup));

            // Verify Groups can be viewed.
            Assert.IsTrue(test.Selenium.IsElementPresent(GeneralInFellowshipConstants.LandingPage.Link_YourGroups));

            // Logout of infellowship
            test.infellowship.Logout();
        }

        //[Test, RepeatOnFailure(Order=3), MultipleAsserts]
        //[Author("Bryan Mikaelian")]
        //[Description("Verifies turning Groups on and off has no effect on Portal.")]
        //public void Weblink_InFellowship_Features_Enable_Disable_Portal_Unchanged() {
        //    // Login to Portal
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.Portal.Login("weblinkuser", "BM.Admin09", "qaeunlx0c2");

        //    // Disable groups
        //    InFellowshipMethods.Configuration_Update_Groups(test, false, false);

        //    // Logout of Portal
        //    test.Portal.Logout();

        //    // Login as a GTA
        //    test.Portal.Login("gta", "BM.Admin09", "qaeunlx0c2");

        //    // Verify Portal->Groups was unchanged.
        //    Assert.AreEqual("Groups by Group Type", test.Selenium.GetText("//div[@id='nav_sub_13']/dl[1]/dt"));
        //    Assert.AreEqual("View All", test.Selenium.GetText("//div[@id='nav_sub_13']/dl[1]/dd[1]/a"));

        //    Assert.AreEqual("Administration", test.Selenium.GetText("//div[@id='nav_sub_13']/dl[2]/dt"));
        //    Assert.AreEqual("Group Types", test.Selenium.GetText("//div[@id='nav_sub_13']/dl[2]/dd[1]/a"));
        //    Assert.AreEqual("Custom Fields", test.Selenium.GetText("//div[@id='nav_sub_13']/dl[2]/dd[2]/a"));
        //    Assert.AreEqual("Search Categories", test.Selenium.GetText("//div[@id='nav_sub_13']/dl[2]/dd[3]/a"));

        //    // Logout of Portal
        //    test.Portal.Logout();

        //    // Login to Portal
        //    test.Portal.Login("weblinkuser", "BM.Admin09", "qaeunlx0c2");

        //    // Enable groups
        //    InFellowshipMethods.Configuration_Update_Groups(test, true, false);

        //    // Logout of Portal
        //    test.Portal.Logout();

        //    // Login as a GTA
        //    test.Portal.Login("gta", "BM.Admin09", "qaeunlx0c2");

        //    // Verify Portal->Groups was unchanged.
        //    Assert.AreEqual("Groups by Group Type", test.Selenium.GetText("//div[@id='nav_sub_13']/dl[1]/dt"));
        //    Assert.AreEqual("View All", test.Selenium.GetText("//div[@id='nav_sub_13']/dl[1]/dd[1]/a"));

        //    Assert.AreEqual("Administration", test.Selenium.GetText("//div[@id='nav_sub_13']/dl[2]/dt"));
        //    Assert.AreEqual("Group Types", test.Selenium.GetText("//div[@id='nav_sub_13']/dl[2]/dd[1]/a"));
        //    Assert.AreEqual("Custom Fields", test.Selenium.GetText("//div[@id='nav_sub_13']/dl[2]/dd[2]/a"));
        //    Assert.AreEqual("Search Categories", test.Selenium.GetText("//div[@id='nav_sub_13']/dl[2]/dd[3]/a"));

        //    // Logout of Portal
        //    test.Portal.Logout();
        //}
        #endregion Groups

        #region Profile Editor
        [Test, RepeatOnFailure(Order = 1), MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies turning off Profile Editor disables Profile Editor.")]
        public void Weblink_InFellowship_Features_Disable_Profile_Editor()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.InFellowshipChurchCode = "QAEUNLX0C1";
            string inFellowshipURL = test.infellowship.URL.ToLower();
            test.Portal.Login("msneeden", "Pa$$w0rd", "QAEUNLX0C1");

            // Disable profile editor
            test.Portal.WebLink_Toggle_InFellowship_Feature("Profile Editor", false);

            // Verify the infellowship links
            test.Selenium.ClickAndWaitForPageToLoad("link=Links");
            Assert.IsFalse(test.Selenium.IsElementPresent("//table/tbody/tr[*]/td[position()=1 and normalize-space(text())='Profile editor']/following-sibling::td/span/a"));
            Assert.IsTrue(test.Selenium.IsElementPresent("//table/tbody/tr[*]/td[position()=1 and normalize-space(text())='Privacy settings']/following-sibling::td/span/a"));
            //There is '/' at the end of the url
            Assert.AreEqual(string.Format("{0}people/profile", inFellowshipURL), test.Selenium.GetText("//tbody/tr[3]/td[2]/span"));
            Assert.AreEqual(string.Format("{0}people/privacy/edit", inFellowshipURL), test.Selenium.GetText("//tbody/tr[4]/td[2]/span"));

            // Logout of portal
            test.Portal.Logout();


            // Login to infellowship
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd");

            // View your profile
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourProfile);

            // Verify the update profile links are not present
            // Profile View page
            test.Selenium.VerifyElementNotPresent(InFellowshipConstants.People.ProfileEditorConstants.Link_EditProfile);

            // Update email page
            test.Selenium.ClickAndWaitForPageToLoad("link=Change Login / Password");
            test.Selenium.VerifyElementNotPresent(InFellowshipConstants.People.ProfileEditorConstants.Link_EditProfile);

            // Logout of infellowship
            test.infellowship.Logout();
        }

        [Test, RepeatOnFailure(Order = 2), MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies turning on Profile Editor enable Profile Editor.")]
        public void Weblink_InFellowship_Features_Enable_Profile_Editor()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.infellowship.InFellowshipChurchCode = "QAEUNLX0C1";
            var inFellowshipURL = test.infellowship.URL.ToLower();
            test.Portal.Login("msneeden", "Pa$$w0rd", "QAEUNLX0C1");

            // Enable profile editor
            test.Portal.WebLink_Toggle_InFellowship_Feature("Profile Editor", true, false);

            // Verify the Link's page updates
            test.Selenium.ClickAndWaitForPageToLoad("link=Links");
            Assert.IsTrue(test.Selenium.IsElementPresent("//table/tbody/tr[*]/td[position()=1 and normalize-space(text())='Profile editor']/following-sibling::td/span/a"));
            Assert.IsTrue(test.Selenium.IsElementPresent("//table/tbody/tr[*]/td[position()=1 and normalize-space(text())='Privacy settings']/following-sibling::td/span/a"));
            Assert.AreEqual(string.Format("{0}people/profile", inFellowshipURL), test.Selenium.GetText("//tbody/tr[3]/td[2]/span"));
            Assert.AreEqual(string.Format("{0}people/privacy/edit", inFellowshipURL), test.Selenium.GetText("//tbody/tr[4]/td[2]/span"));
            
            // Logout of portal
            test.Portal.Logout();


            // Login to infellowship
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd");

            // View your profile
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.GeneralLinks.Link_Home);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LandingPage.Link_YourProfile);

            // Verify the update profile links are present
            // Profile View page
            test.Selenium.VerifyElementPresent(InFellowshipConstants.People.ProfileEditorConstants.Link_EditProfile);

            // Update email page
            test.Selenium.ClickAndWaitForPageToLoad("link=Change Login / Password");
            test.Selenium.VerifyElementPresent(InFellowshipConstants.People.ProfileEditorConstants.Link_EditProfile);

            // Logout of infellowship
            test.infellowship.Logout();
        }
        #endregion Profile Editor

        #region Privacy Policy
        //[Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Updates the InFellowship Privacy Policy for the church.")]
        public void WebLink_InFellowship_Privacy_Policy_Update()
        {
            // Variables
            var privacyPolicyText = "This is our privacy policy.";

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to weblink->privacy policy
            test.Selenium.Navigate(Navigation.WebLink.InFellowship.Privacy);

            // Overwrite the current value
            test.Selenium.Type("privacy_policy_text", privacyPolicyText);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
            test.Selenium.VerifyTextPresent(privacyPolicyText);

            // Logout of portal
            test.Portal.Logout();

            // Open InFellowship
            test.infellowship.Open("QAEUNLX0C2");

            // View the privacy policy
            test.Selenium.ClickAndWaitForPageToLoad("link=Church Privacy Policy");

            // Verify the text is there
            test.Selenium.VerifyTextPresent("Your Privacy Rights");

            // Back
            test.Selenium.GoBack();
            test.Selenium.WaitForPageToLoad("5000");

            // Login to InFellowship
            test.infellowship.Login("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // View the privacy policy
            test.Selenium.ClickAndWaitForPageToLoad("link=Church Privacy Policy");

            // Verify the text is there
            test.Selenium.VerifyTextPresent(privacyPolicyText);

            // Back
            test.Selenium.GoBack();
            test.Selenium.WaitForPageToLoad("5000");

            // Logout
            test.infellowship.Logout();

            // Login to Portal
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to weblink->privacy policy
            test.Selenium.Navigate(Navigation.WebLink.InFellowship.Privacy);

            // Revert the change
            test.Selenium.Type("privacy_policy_text", "");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify the new value is not present
            Assert.IsFalse(test.Selenium.IsTextPresent(privacyPolicyText), "Privacy Policy was not removed!");

            // Verify the default is there
            Assert.AreEqual("We know that you care how information about you is used and shared. We hope the following statements will help you understand how we will collect, use and protect the information you provide to us on our site. We will not use or share your information with anyone except as described in this Privacy Policy.", test.Selenium.GetText("//div[@class='box_notice']/p[1]"));
            Assert.AreEqual("We may collect any information that you provide to us, such as names, e-mail addresses, etc. We also use cookies to store and sometimes track information about our users.", test.Selenium.GetText("//div[@class='box_notice']/p[2]"));
            Assert.AreEqual("We use the personal information that you submit to operate, maintain and provide you the features and functionality of this website and support the mission of the church.", test.Selenium.GetText("//div[@class='box_notice']/p[3]"));
            Assert.AreEqual("We will never share, rent or sell your personal information without your permission, unless required to do so by law or by subpoena. We take appropriate security measures to protect against unauthorized access to or unauthorized alteration, disclosure or destruction of data.", test.Selenium.GetText("//div[@class='box_notice']/p[4]"));
            Assert.AreEqual("We process personal information only for the purposes for which it was collected and in accordance with this Privacy Policy. We take reasonable steps to ensure that the personal information we process is accurate, complete, and current, but we depend on our users to update or correct their personal information whenever necessary.", test.Selenium.GetText("//div[@class='box_notice']/p[5]"));


            // Logout of portal
            test.Portal.Logout();

            // Open InFellowship
            test.infellowship.Open("QAEUNLX0C2");

            // View the privacy policy
            test.Selenium.ClickAndWaitForPageToLoad("link=Church Privacy Policy");
            test.Selenium.SelectWindow(" 	 QA Enterprise Unlimited #2 - In Fellowship  ");

            // Verify the text is not there
            Assert.IsFalse(test.Selenium.IsTextPresent(privacyPolicyText), "Privacy Policy was not removed!");

            // Verify the default is there
            Assert.AreEqual("We know that you care how information about you is used and shared. We hope the following statements will help you understand how we will collect, use and protect the information you provide to us on our site. We will not use or share your information with anyone except as described in this Privacy Policy.", test.Selenium.GetText("//div[@class='grid_16']/p[1]"));
            Assert.AreEqual("We may collect any information that you provide to us, such as names, e-mail addresses, etc. We also use cookies to store and sometimes track information about our users.", test.Selenium.GetText("//div[@class='grid_16']/p[2]"));
            Assert.AreEqual("We use the personal information that you submit to operate, maintain and provide you the features and functionality of this website and support the mission of the church.", test.Selenium.GetText("//div[@class='grid_16']/p[3]"));
            Assert.AreEqual("We will never share, rent or sell your personal information without your permission, unless required to do so by law or by subpoena. We take appropriate security measures to protect against unauthorized access to or unauthorized alteration, disclosure or destruction of data.", test.Selenium.GetText("//div[@class='grid_16']/p[4]"));
            Assert.AreEqual("We process personal information only for the purposes for which it was collected and in accordance with this Privacy Policy. We take reasonable steps to ensure that the personal information we process is accurate, complete, and current, but we depend on our users to update or correct their personal information whenever necessary.", test.Selenium.GetText("//div[@class='grid_16']/p[5]"));

            // Back
            test.Selenium.Close();
            test.Selenium.SelectWindow(null);
            // test.Selenium.GoBack();
            // test.Selenium.WaitForPageToLoad("5000");

            // Login to InFellowship
            test.infellowship.Login("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // View the privacy policy
            test.Selenium.ClickAndWaitForPageToLoad("link=Church Privacy Policy");
            test.Selenium.SelectWindow(" 	 QA Enterprise Unlimited #2 - In Fellowship  ");

            // Verify the text is not there
            Assert.IsFalse(test.Selenium.IsTextPresent(privacyPolicyText), "Privacy Policy was not removed!");

            // Verify the default is there
            Assert.AreEqual("We know that you care how information about you is used and shared. We hope the following statements will help you understand how we will collect, use and protect the information you provide to us on our site. We will not use or share your information with anyone except as described in this Privacy Policy.", test.Selenium.GetText("//div[@class='grid_16']/p[1]"));
            Assert.AreEqual("We may collect any information that you provide to us, such as names, e-mail addresses, etc. We also use cookies to store and sometimes track information about our users.", test.Selenium.GetText("//div[@class='grid_16']/p[2]"));
            Assert.AreEqual("We use the personal information that you submit to operate, maintain and provide you the features and functionality of this website and support the mission of the church.", test.Selenium.GetText("//div[@class='grid_16']/p[3]"));
            Assert.AreEqual("We will never share, rent or sell your personal information without your permission, unless required to do so by law or by subpoena. We take appropriate security measures to protect against unauthorized access to or unauthorized alteration, disclosure or destruction of data.", test.Selenium.GetText("//div[@class='grid_16']/p[4]"));
            Assert.AreEqual("We process personal information only for the purposes for which it was collected and in accordance with this Privacy Policy. We take reasonable steps to ensure that the personal information we process is accurate, complete, and current, but we depend on our users to update or correct their personal information whenever necessary.", test.Selenium.GetText("//div[@class='grid_16']/p[5]"));

            // Back
            test.Selenium.Close();
            test.Selenium.SelectWindow(null);
            // test.Selenium.GoBack();
            // test.Selenium.WaitForPageToLoad("5000");

            // Logout
            test.infellowship.Logout();
        }

        //[Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Updates the InFellowship Privacy Policy for the church using special characters.")]
        public void WebLink_InFellowship_Privacy_Policy_Update_Special_Characters()
        {
            // Variables
            var privacyPolicyText = "<b><%=hell0%></b>";

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Navigate to weblink->privacy policy
            test.Selenium.Navigate(Navigation.WebLink.InFellowship.Privacy);

            // Overwrite the current value
            test.Selenium.Type("privacy_policy_text", privacyPolicyText);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
            test.Selenium.VerifyTextPresent(privacyPolicyText);

            // Logout of portal
            test.Portal.Logout();

            // Open InFellowship
            test.infellowship.Open("dc");

            // View the privacy policy
            test.Selenium.ClickAndWaitForPageToLoad("link=Church Privacy Policy");

            // Verify the text is there
            test.Selenium.VerifyTextPresent(privacyPolicyText);

            // Back
            test.Selenium.GoBack();
            test.Selenium.WaitForPageToLoad("5000");

            // Login to InFellowship
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

            // View the privacy policy
            test.Selenium.ClickAndWaitForPageToLoad("link=Church Privacy Policy");

            // Verify the text is there
            test.Selenium.VerifyTextPresent(privacyPolicyText);

            // Back
            test.Selenium.GoBack();
            test.Selenium.WaitForPageToLoad("5000");

            // Logout
            test.infellowship.Logout();

            // Login to Portal
            test.Portal.Login();

            // Navigate to weblink->privacy policy
            test.Selenium.Navigate(Navigation.WebLink.InFellowship.Privacy);

            // Revert the change
            test.Selenium.Type("privacy_policy_text", "");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify the new value is not present
            Assert.IsFalse(test.Selenium.IsTextPresent(privacyPolicyText), "Privacy Policy was not removed!");

            // Verify the default is there
            Assert.AreEqual("We know that you care how information about you is used and shared. We hope the following statements will help you understand how we will collect, use and protect the information you provide to us on our site. We will not use or share your information with anyone except as described in this Privacy Policy.", test.Selenium.GetText("//div[@class='box_notice']/p[1]"));
            Assert.AreEqual("We may collect any information that you provide to us, such as names, e-mail addresses, etc. We also use cookies to store and sometimes track information about our users.", test.Selenium.GetText("//div[@class='box_notice']/p[2]"));
            Assert.AreEqual("We use the personal information that you submit to operate, maintain and provide you the features and functionality of this website and support the mission of the church.", test.Selenium.GetText("//div[@class='box_notice']/p[3]"));
            Assert.AreEqual("We will never share, rent or sell your personal information without your permission, unless required to do so by law or by subpoena. We take appropriate security measures to protect against unauthorized access to or unauthorized alteration, disclosure or destruction of data.", test.Selenium.GetText("//div[@class='box_notice']/p[4]"));
            Assert.AreEqual("We process personal information only for the purposes for which it was collected and in accordance with this Privacy Policy. We take reasonable steps to ensure that the personal information we process is accurate, complete, and current, but we depend on our users to update or correct their personal information whenever necessary.", test.Selenium.GetText("//div[@class='box_notice']/p[5]"));

            // Logout of portal
            test.Portal.Logout();

            // Open InFellowship
            test.infellowship.Open("dc");

            // View the privacy policy
            test.Selenium.ClickAndWaitForPageToLoad("link=Church Privacy Policy");

            // Verify the text is not there
            Assert.IsFalse(test.Selenium.IsTextPresent(privacyPolicyText), "Privacy Policy was not removed!");

            // Verify the default is there
            Assert.AreEqual("We know that you care how information about you is used and shared. We hope the following statements will help you understand how we will collect, use and protect the information you provide to us on our site. We will not use or share your information with anyone except as described in this Privacy Policy.", test.Selenium.GetText("//div[@class='grid_16']/p[1]"));
            Assert.AreEqual("We may collect any information that you provide to us, such as names, e-mail addresses, etc. We also use cookies to store and sometimes track information about our users.", test.Selenium.GetText("//div[@class='grid_16']/p[2]"));
            Assert.AreEqual("We use the personal information that you submit to operate, maintain and provide you the features and functionality of this website and support the mission of the church.", test.Selenium.GetText("//div[@class='grid_16']/p[3]"));
            Assert.AreEqual("We will never share, rent or sell your personal information without your permission, unless required to do so by law or by subpoena. We take appropriate security measures to protect against unauthorized access to or unauthorized alteration, disclosure or destruction of data.", test.Selenium.GetText("//div[@class='grid_16']/p[4]"));
            Assert.AreEqual("We process personal information only for the purposes for which it was collected and in accordance with this Privacy Policy. We take reasonable steps to ensure that the personal information we process is accurate, complete, and current, but we depend on our users to update or correct their personal information whenever necessary.", test.Selenium.GetText("//div[@class='grid_16']/p[5]"));

            // Back
            test.Selenium.GoBack();
            test.Selenium.WaitForPageToLoad("5000");

            // Login to InFellowship
            test.infellowship.Login("msneeden@fellowshiptech.com", "Pa$$w0rd", "dc");

            // View the privacy policy
            test.Selenium.ClickAndWaitForPageToLoad("link=Church Privacy Policy");

            // Verify the text is not there
            Assert.IsFalse(test.Selenium.IsTextPresent(privacyPolicyText), "Privacy Policy was not removed!");

            // Verify the default is there
            Assert.AreEqual("We know that you care how information about you is used and shared. We hope the following statements will help you understand how we will collect, use and protect the information you provide to us on our site. We will not use or share your information with anyone except as described in this Privacy Policy.", test.Selenium.GetText("//div[@class='grid_16']/p[1]"));
            Assert.AreEqual("We may collect any information that you provide to us, such as names, e-mail addresses, etc. We also use cookies to store and sometimes track information about our users.", test.Selenium.GetText("//div[@class='grid_16']/p[2]"));
            Assert.AreEqual("We use the personal information that you submit to operate, maintain and provide you the features and functionality of this website and support the mission of the church.", test.Selenium.GetText("//div[@class='grid_16']/p[3]"));
            Assert.AreEqual("We will never share, rent or sell your personal information without your permission, unless required to do so by law or by subpoena. We take appropriate security measures to protect against unauthorized access to or unauthorized alteration, disclosure or destruction of data.", test.Selenium.GetText("//div[@class='grid_16']/p[4]"));
            Assert.AreEqual("We process personal information only for the purposes for which it was collected and in accordance with this Privacy Policy. We take reasonable steps to ensure that the personal information we process is accurate, complete, and current, but we depend on our users to update or correct their personal information whenever necessary.", test.Selenium.GetText("//div[@class='grid_16']/p[5]"));

            // Back
            test.Selenium.GoBack();
            test.Selenium.WaitForPageToLoad("5000");

            // Logout
            test.infellowship.Logout();
        }

        //[Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the privacy policy cannot exceed 6000 characters")]
        public void WebLink_InFellowship_Privacy_Policy_Update_Cannot_Exceed_6000_Characters()
        {
            // Variables
            StringBuilder privacyPolicyText = new StringBuilder("This is my really long privacy policy that is long");
            // Build an 6000 character long string
            for (int i = 0; i < 500; i++)
            {
                privacyPolicyText.Append(" This is my really long string");
            }

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("cgutekunst", "CG.Admin09", "QAEUNLX0C1");

            // Navigate to weblink->privacy policy
            test.Selenium.Navigate(Navigation.WebLink.InFellowship.Privacy);

            // Overwrite the current value
            test.Selenium.Type("privacy_policy_text", privacyPolicyText.ToString());
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            test.Selenium.VerifyTextPresent("Privacy Policy cannot exceed 6000 characters.");

            // Logout of portal
            test.Portal.Logout();
        }
        #endregion Privacy Policy

        #region Contact
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
            [Author("Matthew Sneeden")]
            [Description("Updates the InFellowship contact for the church.")]
            public void WebLink_InFellowship_Contact_Update()
            {
                // Login to portal
                TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.Login();

                // Navigate to weblink->contact
                test.Selenium.Navigate(Navigation.WebLink.InFellowship.Contact);

                // Overwrite the current value
                test.Selenium.Type("contact_us_email", "invalid@email.com");
                test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
                Assert.AreEqual("invalid@email.com", test.Selenium.GetValue("contact_us_email"));

                // Store the new value
                test.Selenium.Type("contact_us_email", test.infellowship.InFellowshipEmail);
                test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

                // Navigate to weblink->contact
                test.Selenium.Navigate(Navigation.WebLink.InFellowship.Contact);

                // Verify the new value
                Assert.AreEqual(test.infellowship.InFellowshipEmail, test.Selenium.GetValue("contact_us_email"));

                // Logout of portal
                test.Portal.Logout();
            }
        #endregion Contact
    }
}