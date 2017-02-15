using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Globalization;

namespace FTTests.Portal.People {

    [TestFixture]    
    public class Portal_People_Search_FindAPerson_Addresses : FixtureBaseWebDriver {

        [Test (Order = 1), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Felix Gaytan")]
        [Description("Clean up all secondary addresses.")]
        public void People_Search_Addresses_Cleanup_Secondary_Addresses()
        {

            // Set initial conditions
            string individualName = "Matthew Sneeden";

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // View an individual
            test.Portal.People_ViewIndividual_WebDriver(individualName);

            //Are we in Household View
            test.GeneralMethods.WaitForElement(test.Driver, By.TagName("html"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Individual Detail"), "Did Not Go To Individual Detail Page");
            //test.GeneralMethods.VerifyTextPresentWebDriver("Household View");

            //Find Secondary Counts
            int secondaryAddrCount = test.Driver.FindElementsByXPath("//a[contains(@href, '/people/Household/Address/Edit.aspx?') and following-sibling::div/strong/text()='Secondary']").Count();

            TestLog.WriteLine(string.Format("Secondary Addr Count: {0}", secondaryAddrCount));

            
            while (secondaryAddrCount > 0)
            {

                TestLog.WriteLine("Delete Secondary Address");

                // Click the edit link associated with the specified address                              
                test.Driver.FindElementByXPath("//a[contains(@href, '/people/Household/Address/Edit.aspx?') and following-sibling::div/strong/text()='Secondary']").Click();

                // Delete the address and click 'yes' at the confirmation
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Delete this address"));
                test.Driver.FindElementByLinkText("Delete this address").Click();
                Assert.IsTrue(Regex.IsMatch(test.Driver.SwitchTo().Alert().Text, "^Are you sure you want to delete this address[\\s\\S]$"));
                test.Driver.SwitchTo().Alert().Accept();

                System.Threading.Thread.Sleep(1000);

                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Add another"));
                
                secondaryAddrCount = test.Driver.FindElementsByXPath("//a[contains(@href, '/people/Household/Address/Edit.aspx?') and following-sibling::div/strong/text()='Secondary']").Count();
                TestLog.WriteLine(string.Format("Secondary Addr Count: {0}", secondaryAddrCount));

            }
            

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        [Test(Order = 2), RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the address section correctly displays a suffix.")]
        public void People_Search_FindAPerson_Suffix() {

            TestBaseWebDriver test = null;

            // Set initial conditions
            string address1 = "1704 S Milbrook Ln";
            string individualName = "Visitor Sneeden";

            try
            {
                base.SQL.People_Addresses_Delete(15, base.SQL.People_Individuals_FetchID(15, individualName), address1);
            }
            catch (Exception e)
            {

                // Login to portal
                test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                TestLog.WriteLine(e + " happened. Removing suffix");

                // View the individual
                test.Portal.People_ViewIndividual_WebDriver(individualName);

                // Remove the suffix from the individual
                test.Driver.FindElementByXPath(Portal.People.SearchConstants.Edit_individual).Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.Id("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtSuffix"), 20);
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtSuffix").Clear();
                test.Driver.FindElementById(GeneralButtons.Save).Click();

                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Back"));
                test.Driver.FindElementByLinkText("Back").Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("sign out"));

                // Logout of portal
                test.Portal.LogoutWebDriver();

                base.SQL.People_Addresses_Delete(15, base.SQL.People_Individuals_FetchID(15, individualName), address1);

            }

            // Login to portal
            if (test == null)
            {
                test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            }

            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // View an individual
            TestLog.WriteLine("View " + individualName);
            test.Portal.People_ViewIndividual_WebDriver(individualName);

            // Edit the individual to add a suffix
            test.GeneralMethods.WaitForElement(test.Driver, By.TagName("html"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Individual Detail"), "Did Not Go To Individual Detail Page");

            test.Driver.FindElementsByXPath(Portal.People.SearchConstants.Edit_individual)[0].Click();
                                       //ctl00$ctl00$MainContent$content$ctlIndividualFull$txtSuffix
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtSuffix"));
            //ctl00_ctl00_MainContent_content_ctlIndividualFull_txtSuffix
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtSuffix").Clear();
            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtSuffix").SendKeys("Jr");
            test.Driver.FindElementById(GeneralButtons.Save).Click();

            try
            {
                // Add an address to the individual
                test.Portal.People_AddAddress(string.Format("{0}{1}", individualName, ", Jr (Visitor)"), "Secondary", null, address1, null, "Arlington Heights", "Illinois", "60005", null, null);

                // Verify suffix is displayed in the sidebar
                //Assert.AreEqual(string.Format("{0}{1}", individualName, ", Jr"), test.Driver.FindElementByXPath(string.Format("//div[@class='addresses']/p/text()[contains(., '{0}')]", individualName)).Text);
                Assert.AreEqual(string.Format("{0}{1}", individualName, ", Jr"), test.Driver.FindElementByXPath(string.Format("//div[@class='addresses']/p[contains(text(), '{0}')]", individualName)).Text);

                // Verify the name for the communications
                //Assert.AreEqual(string.Format("{0}{1}", individualName, ", Jr"), test.Driver.FindElementByXPath(string.Format("//div[@class='communications']/p/text()[contains(., '{0}')]", individualName)).Text);
                Assert.AreEqual(string.Format("{0}{1}", individualName, ", Jr"), test.Driver.FindElementByXPath(string.Format("//div[@class='communications']/p[contains(text(), '{0}')]", individualName)).Text);

                // View the household
                test.Driver.FindElementByLinkText("View the household");

                // Verify the name for the communications
                //Assert.AreEqual(string.Format("{0}{1}", individualName, ", Jr"), test.Driver.FindElementByXPath(string.Format("//div[@class='communications']/p/text()[contains(., '{0}')]", individualName)).Text);
                Assert.AreEqual(string.Format("{0}{1}", individualName, ", Jr"), test.Driver.FindElementByXPath(string.Format("//div[@class='communications']/p[contains(text(), '{0}')]", individualName)).Text);

                // Verify suffix is displayed in the sidebar
                //Assert.AreEqual(string.Format("{0}{1}", individualName, ", Jr"), test.Driver.FindElementByXPath(string.Format("//div[@class='addresses']/p/text()[contains(., '{0}')]", individualName)).Text);
                Assert.AreEqual(string.Format("{0}{1}", individualName, ", Jr"), test.Driver.FindElementByXPath(string.Format("//div[@class='addresses']/p[contains(text(), '{0}')]", individualName)).Text);

            }
            finally
            {
                // View the individual
                test.Portal.People_ViewIndividual_WebDriver(individualName);


                // Remove the suffix from the individual
                test.GeneralMethods.WaitForElement(test.Driver, By.XPath(Portal.People.SearchConstants.Edit_individual), 20);
                test.Driver.FindElementByXPath(Portal.People.SearchConstants.Edit_individual).Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.Id("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtSuffix"));
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ctlIndividualFull_txtSuffix").Clear();
                test.Driver.FindElementById(GeneralButtons.Save).Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Back"));
                test.Driver.FindElementByLinkText("Back").Click();
                test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("sign out"));

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }
       

        }

        [Test(Order = 3), RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to change the primary address of a household to an individual w/out selecting a type.")]
        public void People_Search_FindAPerson_ViewIndividual_EditAddress_ChangeHHToINDNoType() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // View and individual
            test.Portal.People_ViewIndividual_WebDriver("Matthew Sneeden");

            // Edit the primary household address
            test.Driver.FindElementByXPath("//a[contains(@href, '/people/Household/Address/Edit.aspx?') and following-sibling::div/strong/text()='Primary']").Click(); //and normalize-space(ancestor::div/p/text())='Household'

            // Change from household to an individual
            new SelectElement(test.Driver.FindElementById("address_for")).SelectByText("Matthew Sneeden (Head)");

            // Attempt to save, verify screen message
            test.Driver.FindElementById("save_address").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.TagName("html"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(TextConstants.ErrorHeadingSingular));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Address type is required."));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test(Order = 4), RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies the user can delete an address via the user interface.")]
        public void People_Search_FindAPerson_ViewIndividual_EditAddress_Delete() {
            // Set initial conditions
            int churchId = 15;
            string individualName = "Matthew Sneeden";
            string address1 = "6009 Rock Ridge Dr";
            base.SQL.People_Addresses_Delete(churchId, base.SQL.People_Individuals_FetchID(churchId, individualName), address1);
            base.SQL.People_Addresses_Create(churchId, individualName, address1, "Flower Mound", "TX", "75028-3771", "US");

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // View an individual
            test.Portal.People_ViewIndividual_WebDriver(individualName);


            // Click the edit link associated with the specified address
            test.Driver.FindElementByXPath(string.Format("//div[@class='street-address' and text()='{0}']/ancestor::div/ancestor::div/preceding-sibling::a[contains(@href, '/people/Household/Address/Edit.aspx?household_id=')]", address1)).Click();

            // Delete the address and click 'yes' at the confirmation
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Delete this address"));
            test.Driver.FindElementByLinkText("Delete this address").Click();
            Assert.IsTrue(Regex.IsMatch(test.Driver.SwitchTo().Alert().Text, "^Are you sure you want to delete this address[\\s\\S]$"));
            test.Driver.SwitchTo().Alert().Accept();

            // Verify the address is not present
            test.GeneralMethods.WaitForElement(test.Driver, By.TagName("html"));
            Assert.IsFalse(test.Driver.FindElementByTagName("html").Text.Contains(address1));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        //add by grace zhang: FO-3486 involving 5 cases relate to Individual_Addressvalidation don't run on staging because of a little problem staging env
        [Test(Order = 5), RepeatOnFailure, DoesNotRunInStaging]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create an address that will result in a premise partial.")]
        public void People_Search_FindAPerson_ViewIndividual_AddAnAddress_PremisePartial() {
            // Set initial conditions
            string address1 = "401 Pebble Way";
            base.SQL.People_Addresses_Delete(15, base.SQL.IndividualID, address1);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // View an individual
            test.Portal.People_ViewIndividual_WebDriver("Matthew Sneeden");

            // Add an address
            test.Portal.People_AddAddress("Matthew Sneeden (Head)", "Secondary", null, address1, null, "Arlington", "Texas", "76006", null, null);

            // Verify user is prompted for premise partial
            test.GeneralMethods.WaitForElement(test.Driver, By.TagName("html"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("premise partial"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("According to the United States Postal Service the address you entered is either missing important information such as an Apartment or Suite or the information you provided is incorrect. Please choose one of the options below."));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Enter apartment, flat or unit number"));

            // Enter an apartment number and proceed
            test.Driver.FindElementById("partial_number").SendKeys("240");
            test.Driver.FindElementById("submitQuery").Click();

            // Verify the suggested format and proceed
            test.GeneralMethods.WaitForElement(test.Driver, By.TagName("html"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("The following address is recommended"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(address1));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Apt 240"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Arlington, TX 76006-3503"));
            test.Driver.FindElementByXPath("//div[@id='main_content']/div[1]/div[2]/div[1]/div[2]/form/p/input").Click();

            // Verify the address was created
            test.Portal.People_ViewIndividual_WebDriver("Matthew Sneeden");

            test.GeneralMethods.WaitForElement(test.Driver, By.TagName("html"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Secondary"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(address1));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Apt 240"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Arlington, TX 76006-3503"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Last Updated: Today"));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test(Order = 6), RepeatOnFailure, DoesNotRunInStaging]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create an address that will result in a multiple match.")]
        public void People_Search_FindAPerson_ViewIndividual_AddAnAddress_MultipleMatch() {
            // Set initial conditions
            string address1Resolved = "415 W National St";
            base.SQL.People_Addresses_Delete(15, base.SQL.IndividualID, address1Resolved);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // View an individual
            test.Portal.People_ViewIndividual_WebDriver("Matthew Sneeden");

            // Add an address
            string addressType = "Secondary";
            test.Portal.People_AddAddress("Matthew Sneeden (Head)", addressType, null, "415 National", null, "Vermillion", "South Dakota", "57069", null, null);

            // Verify user is prompted for multiple matches
            test.GeneralMethods.WaitForElement(test.Driver, By.TagName("html"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("multiple matches"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("According to the United States Postal Service the address you entered is missing important information such as directional or street descriptor. Please choose the correct address from the list below."));

            // Select the direction and proceed
            string xPath = test.Driver.FindElementByXPath("//table[@class='grid']/tbody/tr[3]/td[1]").Text == "415 W National St, Vermillion SD" ? "//table[@class='grid']/tbody/tr[3]/td[3]/form/a" : "//table[@class='grid']/tbody/tr[2]/td[3]/form/a";
            test.Driver.FindElementByXPath(xPath).Click();

            // Verify the address was created
            test.GeneralMethods.WaitForElement(test.Driver, By.TagName("html"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(addressType));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(address1Resolved));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Vermillion, SD 57069-1947"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Last Updated: Today"));

            // Logout of portal
            test.Portal.LogoutWebDriver();
            //// Select the address and proceed
            //test.Driver.FindElementByXPath("//div[@id='main_content']/div[1]/div[2]/div[1]/div[2]/form/p/input").Click();

            //test.GeneralMethods.WaitForElement(test.Driver, By.ClassName("addresses"));
            //Boolean isFound = false;
            //int index = 1;

            //try
            //{
            //    while (test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/div/div[1]", index)) != null)
            //    {
            //        try
            //        {
            //            if (test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/strong", index)).Text.Equals(addressType))
            //            {
            //                if (test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/div/div[1]", index)).Text.Equals(address1Resolved))
            //                {
            //                    Assert.IsTrue(test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/div/div[2]", index)).Text.Equals("Vermillion, SD 57069-1947"), "Address 2 isn't display well");
            //                    Assert.IsTrue(test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/small", index)).Text.Equals("Last Updated: Today"), "Last Updated time is wrong");
            //                    isFound = true;
            //                    break;
            //                }
            //            }
            //        }
            //        catch (OpenQA.Selenium.NoSuchElementException e) { }

            //        TestLog.WriteLine("Tested address with index: " + index);
            //        index++;
            //    }
            //}
            //catch (OpenQA.Selenium.NoSuchElementException e) { }
            //finally
            //{
            //    // Logout of portal
            //    test.Portal.LogoutWebDriver();

            //    Assert.IsTrue(isFound, "Failed to find new addresses are display well");
            //}

        }

        [Test(Order = 7), RepeatOnFailure, DoesNotRunInStaging]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create an address that will result in interaction required.")]
        public void People_Search_FindAPerson_ViewIndividual_AddAnAddress_InteractionRequired() {
            // Set initial conditions
            string address1Resolved = "5809 N 81st St";
            base.SQL.People_Addresses_Delete(15, base.SQL.IndividualID, address1Resolved);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // View an individual
            test.Portal.People_ViewIndividual_WebDriver("Matthew Sneeden");

            // Add an address
            string addressType = "Secondary";
            test.Portal.People_AddAddress("Matthew Sneeden (Head)", addressType, null, "5809 81st St", null, "Scottsdale", "Arizona", "85250", null, null);

            // Verify user is prompted for interaction
            test.GeneralMethods.WaitForElement(test.Driver, By.TagName("html"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("interaction required"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("According to the United States Postal Service the address you entered may be incorrect."));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Please choose one of the options below."));

            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(address1Resolved));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Scottsdale, AZ 85250-6207"));

            // Select the address and proceed
            test.Driver.FindElementByXPath("//div[@id='main_content']/div[1]/div[2]/div[1]/div[2]/form/p/input").Click();

            // Verify the address was created
            test.GeneralMethods.WaitForElement(test.Driver, By.TagName("html"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(addressType));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(address1Resolved));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Scottsdale, AZ 85250-6207"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Last Updated: Today"));

            // Logout of portal
            test.Portal.LogoutWebDriver();
            //test.GeneralMethods.WaitForElement(test.Driver, By.ClassName("addresses"));
            //Boolean isFound = false;
            //int index = 1;

            //try
            //{
            //    while (test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/div/div[1]", index)) != null)
            //    {
            //        try
            //        {
            //            if (test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/strong", index)).Text.Equals(addressType))
            //            {
            //                if (test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/div/div[1]", index)).Text.Equals(address1Resolved))
            //                {
            //                    Assert.IsTrue(test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/div/div[2]", index)).Text.Equals("Scottsdale, AZ 85250-6207"), "Address 2 isn't display well");
            //                    Assert.IsTrue(test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/small", index)).Text.Equals("Last Updated: Today"), "Last Updated time is wrong");
            //                    isFound = true;
            //                    break;
            //                }
            //            }
            //        }
            //        catch (OpenQA.Selenium.NoSuchElementException e) { }

            //        TestLog.WriteLine("Tested address with index: " + index);
            //        index++;
            //    }
            //}
            //catch (OpenQA.Selenium.NoSuchElementException e) { }
            //finally
            //{
            //    // Logout of portal
            //    test.Portal.LogoutWebDriver();

            //    Assert.IsTrue(isFound, "Failed to find new addresses are display well");
            //}

        }

        [Test(Order = 8), RepeatOnFailure, DoesNotRunInStaging]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create an address that will result in interaction required 2.")]
        public void People_Search_FindAPerson_ViewIndividual_AddAnAddress_InteractionRequired2() {
            // Set initial conditions
            string address1 = "11774 SW Swendon Loop";
            base.SQL.People_Addresses_Delete(15, base.SQL.IndividualID, address1);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // View an individual
            test.Portal.People_ViewIndividual_WebDriver("Matthew Sneeden");

            // Add an address
            string addressType = "Secondary";
            test.Portal.People_AddAddress("Matthew Sneeden (Head)", addressType, null, address1, null, "Eugene", "Oregon", "97223", null, null);

            // Verify user is prompted for city change
            test.GeneralMethods.WaitForElement(test.Driver, By.TagName("html"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("According to the United States Postal Service, the address you entered does not match the data on record for this location. Please either choose the suggested address, or continue with the one you originally entered."));

            Assert.IsFalse(test.Driver.FindElementByTagName("html").Text.Contains("11774 SW Swendon Loop Tigard"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(address1));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Portland, OR 97223-1603"));

            // Select the address and proceed
            test.Driver.FindElementByXPath("//div[@id='main_content']/div[1]/div[2]/div[1]/div[2]/form/p/input").Click();

            // Verify the address was created
            // Verify the address was created
            test.GeneralMethods.WaitForElement(test.Driver, By.TagName("html"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(addressType));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(address1));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Portland, OR 97223-1603"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Last Updated: Today"));

            // Logout of portal
            test.Portal.LogoutWebDriver();
            //test.GeneralMethods.WaitForElement(test.Driver, By.ClassName("addresses"));
            //Boolean isFound = false;
            //int index = 1;

            //try
            //{
            //    while (test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/div/div[1]", index)) != null)
            //    {
            //        try
            //        {
            //            if (test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/strong", index)).Text.Equals(addressType))
            //            {
            //                if (test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/div/div[1]", index)).Text.Equals(address1))
            //                {
            //                    Assert.IsTrue(test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/div/div[2]", index)).Text.Equals("Portland, OR 97223-1603"), "Address 2 isn't display well");
            //                    Assert.IsTrue(test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/small", index)).Text.Equals("Last Updated: Today"), "Last Updated time is wrong");
            //                    isFound = true;
            //                    break;
            //                }
            //            }
            //        }
            //        catch (OpenQA.Selenium.NoSuchElementException e) { }

            //        TestLog.WriteLine("Tested address with index: " + index);
            //        index++;
            //    }
            //}
            //catch (OpenQA.Selenium.NoSuchElementException e) { }
            //finally
            //{
            //    // Logout of portal
            //    test.Portal.LogoutWebDriver();

            //    Assert.IsTrue(isFound, "Failed to find new addresses are display well");
            //}

        }

        [Test(Order = 9), RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create an canadian address.")]
        public void People_Search_FindAPerson_ViewIndividual_AddAnAddress_Canada() {
            // Set initial conditions
            string address1 = "#35 250 Satok Crescent";
            base.SQL.People_Addresses_Delete(15, base.SQL.IndividualID, address1);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // View an individual
            test.Portal.People_ViewIndividual_WebDriver("Matthew Sneeden");

            // Add an address
            string addressType = "Secondary";
            test.Portal.People_AddAddress("Matthew Sneeden (Head)", addressType, "Canada", address1, null, "Milton", "Ontario", "L9T 3P4", null, null);

            // Verify user is taken directly back to the individual view
            Assert.AreEqual(PeopleHeadingText.TitleFormat("Individual Detail"), test.Driver.Title);

            // Verify the address was created
            test.GeneralMethods.WaitForElement(test.Driver, By.TagName("html"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(addressType));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(address1));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Milton, Ontario L9T 3P4"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Last Updated: Today"));

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test(Order = 10), RepeatOnFailure, DoesNotRunInStaging]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create an address using a po box.")]
        public void People_Search_FindAPerson_ViewIndividual_AddAnAddress_POBox() {
            
            // Set initial conditions
            string address1Resolved = "PO Box 50333";
            base.SQL.People_Addresses_Delete(15, base.SQL.IndividualID, address1Resolved);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // View an individual
            test.Portal.People_ViewIndividual_WebDriver("Matthew Sneeden");

            // Add an address
            string addressType = "Secondary";
            test.Portal.People_AddAddress("Matthew Sneeden (Head)", addressType, null, "P.O. Box 50333", null, "Billings", "Montana", "59105", null, null);

            // Verify user is taken directly back to the individual view
            Assert.AreEqual(PeopleHeadingText.TitleFormat("Individual Detail"), test.Driver.Title);

            // Verify the address was created
            test.GeneralMethods.WaitForElement(test.Driver, By.TagName("html"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(addressType));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(address1Resolved));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Billings, MT 59105-0333"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Last Updated: Today"));

            // Logout of portal
            test.Portal.LogoutWebDriver();
            //// Select the address and proceed
            //test.Driver.FindElementByXPath("//div[@id='main_content']/div[1]/div[2]/div[1]/div[2]/form/p/input").Click();
            ////*[@id="ma//*[@id="attendance"]
            //test.GeneralMethods.WaitForElement(test.Driver, By.ClassName("addresses"));
            //Boolean isFound = false;
            //int index = 1;

            //try
            //{
            //    while (test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/div/div[1]", index)) != null)
            //    {
            //        try
            //        {
            //            if (test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/strong", index)).Text.Equals(addressType))
            //            {
            //                if (test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/div/div[1]", index)).Text.Equals(address1Resolved))
            //                {
            //                    Assert.IsTrue(test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/div/div[2]", index)).Text.Equals("Billings, MT 59105-0333"), "Address 2 isn't display well");
            //                    Assert.IsTrue(test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/small", index)).Text.Equals("Last Updated: Today"), "Last Updated time is wrong");
            //                    isFound = true;
            //                    break;
            //                }
            //            }
            //        }
            //        catch (OpenQA.Selenium.NoSuchElementException e) { }

            //        TestLog.WriteLine("Tested address with index: " + index);
            //        index++;
            //    }
            //}
            //catch (OpenQA.Selenium.NoSuchElementException e) { }
            //finally
            //{
            //    // Logout of portal
            //    test.Portal.LogoutWebDriver();

            //    Assert.IsTrue(isFound, "Failed to find new addresses are display well");
            //}

        }

        [Test(Order = 11), RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create an address using address1 and zip code.")]
        public void People_Search_FindAPerson_ViewIndividual_AddAnAddress_Address1ZipCode() {
            // Set initial conditions
            string address1Resolved = "7604 Naples Ln";
            string address2 = "Frisco, TX 75035-2965";
            base.SQL.People_Addresses_Delete(15, base.SQL.IndividualID, address1Resolved);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

            // View an individual
            test.Portal.People_ViewIndividual_WebDriver("Matthew Sneeden");

            // Add an address
            string addressType = "Secondary";
            test.Portal.People_AddAddress("Matthew Sneeden (Head)", addressType, null, address1Resolved, address2, null, null, "75035", null, null);

            // Verify user is taken directly back to the individual view
            Assert.AreEqual(PeopleHeadingText.TitleFormat("Individual Detail"), test.Driver.Title);

            // Verify the address was created
            test.GeneralMethods.WaitForElement(test.Driver, By.TagName("html"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(addressType));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains(address1Resolved));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Frisco, TX 75035-2965"));
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Last Updated: Today"));

            // Logout of portal
            test.Portal.LogoutWebDriver();
            //test.GeneralMethods.WaitForElement(test.Driver, By.ClassName("addresses"));
            //Boolean isFound = false;
            //int index = 1;

            //try
            //{
            //    while (test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/div/div[1]", index)) != null)
            //    {
            //        try
            //        {
            //            if (test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/strong", index)).Text.Equals(addressType))
            //            {
            //                if (test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/div/div[1]", index)).Text.Equals(address1Resolved))
            //                {
            //                    Assert.IsTrue(test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/div/div[2]", index)).Text.Equals(address2), "Address 2 isn't display well");
            //                    Assert.IsTrue(test.Driver.FindElementByXPath(String.Format("//div[@class='addresses']/ul/li[{0}]/div/small", index)).Text.Equals("Last Updated: Today"), "Last Updated time is wrong");
            //                    isFound = true;
            //                    break;
            //                }
            //            }
            //        }
            //        catch (OpenQA.Selenium.NoSuchElementException e) { }

            //        TestLog.WriteLine("Tested address with index: " + index);
            //        index++;
            //    }
            //}
            //catch (OpenQA.Selenium.NoSuchElementException e) { }
            //finally
            //{
            //    // Logout of portal
            //    test.Portal.LogoutWebDriver();

            //    Assert.IsTrue(isFound, "Failed to find new addresses are display well");
            //}

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("FO-6267 Verifies contact item created time and updated time are saved correctly")]
        public void People_Search_FindAPerson_ViewIndividual_CreateContactItem()
        {
            string churchCode = "QAEUNLX0C2";

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", churchCode);

            // View an individual
            test.Portal.People_ViewIndividual_WebDriver("Madysen Freemen");

            TestLog.WriteLine("Click to add contact item");
            test.Driver.FindElementByXPath(".//div[@id='contact_items']/div/a").Click();
            test.GeneralMethods.WaitForElementDisplayed(By.Id("ddlContactForm"));

            TestLog.WriteLine("Select options to the contact item");
            new SelectElement(test.Driver.FindElementById("ddlContactForm")).SelectByIndex(4);
            test.GeneralMethods.WaitForElementEnabled(By.Id("ddlContactItem"));
            SelectElement contactOption = new SelectElement(test.Driver.FindElementById("ddlContactItem"));
            contactOption.SelectByIndex(2);
            string itemName = contactOption.SelectedOption.Text;
            TestLog.WriteLine("The new contact item is named as: " + itemName);
            int churchId = test.SQL.FetchChurchID(churchCode);
            try
            {
                TestLog.WriteLine("Click to save the contact item");
                test.Driver.FindElementById("btnSaveContactItem").Click();
                test.GeneralMethods.WaitForElementDisplayed(By.LinkText(itemName));

                System.Threading.Thread.Sleep(5000);
                string contactInstanceId = "contact_instance_item_" + test.SQL.Admin_ContactItems_FetchTheLatestContactInstanceId(churchId, itemName);
                test.GeneralMethods.WaitForElementDisplayed(By.Id(contactInstanceId));

                TestLog.WriteLine("Check create and update time of the contact item are the same");
                Assert.IsTrue(test.Driver.FindElementByXPath(String.Format(".//div[@id='{0}']/small[1]", contactInstanceId)).Text.Replace("Created", "").Equals(
                    test.Driver.FindElementByXPath(String.Format(".//div[@id='{0}']/small[2]", contactInstanceId)).Text.Replace("Updated", "")), "Contact item create time is not the same as default update time");

                TestLog.WriteLine("Click on the contact item name to edit");
                test.Driver.FindElementByXPath(String.Format(".//div[@id='{0}']/div/strong/a", contactInstanceId)).Click();

                TestLog.WriteLine("Wait for a moment to make update time greater than create time");
                System.Threading.Thread.Sleep(60000);

                TestLog.WriteLine("Click on Save to udpate");
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_Menu_contacts_txtIndividualContactNote").SendKeys("Update this contact item");
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_Menu_contacts_btnSave").Click();

                TestLog.WriteLine("View above individual agagin");
                test.Portal.People_ViewIndividual_WebDriver("Madysen Freemen");

                TestLog.WriteLine("Check create and update time of the contact item are different");
                Assert.IsFalse(test.Driver.FindElementByXPath(String.Format(".//div[@id='{0}']/small[1]", contactInstanceId)).Text.Replace("Created", "").Equals(
                    test.Driver.FindElementByXPath(String.Format(".//div[@id='{0}']/small[2]", contactInstanceId)).Text.Replace("Updated", "")), "Contact item create time are the same as default update time");

                // Assert.IsTrue(DateTime.ParseExact(test.Driver.FindElementByXPath(String.Format(".//div[@id='{0}']/small[2]", contactInstanceId)).Text.Replace("Updated", ""), "HH:mm tt", CultureInfo.InvariantCulture) >
                   // DateTime.ParseExact(test.Driver.FindElementByXPath(String.Format(".//div[@id='{0}']/small[1]", contactInstanceId)).Text.Replace("Updated", ""), "HH:mm tt", CultureInfo.InvariantCulture), "Updated time is not greater than Created time");
            }
            finally
            {
                test.SQL.Admin_ContactItems_ContactItemInstance_Delete(churchId, Convert.ToInt32(test.SQL.Admin_ContactItems_FetchTheLatestContactInstanceId(churchId, itemName)));

                // Logout of Portal
                test.Portal.LogoutWebDriver();
            }

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("FO-7291 P4 - Attribute End Date Not Appearing If Only Field Selected")]
        public void People_Search_FindAPerson_ViewIndividual_CreateAttributeItem()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string churchCode = "QAEUNLX0C2";
            int churchId = test.SQL.FetchChurchID(churchCode);

            string individualAttributeGroupName = "Attribute Group FO-7291";
            string individualAttributeName = "Individual Attribute FO-7291";
            base.SQL.Admin_IndividualAttributeGroups_Delete(churchId, individualAttributeGroupName);
            base.SQL.Admin_IndividualAttributes_Delete(churchId, individualAttributeGroupName, individualAttributeName);
            base.SQL.Admin_IndividualAttributeGroups_Create(churchId, individualAttributeGroupName, true);
            base.SQL.Admin_IndividualAttributes_Create(churchId, individualAttributeGroupName, individualAttributeName, false, false, true, false, true);

            try
            {
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", churchCode);

                // View an individual
                test.Portal.People_ViewIndividual_WebDriver("Madysen Freemen");

                TestLog.WriteLine("Click to add an attribute");
                test.Driver.FindElementByXPath(".//div[@id='ind_attributes']/div/a").Click();
                test.GeneralMethods.WaitForElementDisplayed(By.Id("ddlAttribute"));

                TestLog.WriteLine("Select options to the attribute");
                new SelectElement(test.Driver.FindElementById("ddlAttribute")).SelectByText(individualAttributeName);
                test.GeneralMethods.WaitForElementEnabled(By.Id("individual_attribute_end_date"));

                string endTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).ToString("MM/dd/yyyy");
                test.Driver.FindElementById("individual_attribute_end_date").SendKeys(endTime);

                TestLog.WriteLine("Click to save the attribute item");
                test.Driver.FindElementByCssSelector("input[value=\"Add attribute\"]").Click();
                test.GeneralMethods.WaitForElementDisplayed(By.LinkText(individualAttributeGroupName));

                TestLog.WriteLine("Click on the attribute group to expand it");
                test.Driver.FindElementByLinkText(individualAttributeGroupName).Click();

                TestLog.WriteLine("Check display of the attribute item");
                test.GeneralMethods.WaitForElementDisplayed(By.XPath(String.Format("//span[contains(text(), '{0}')]", individualAttributeName)));
                bool isLabel = test.GeneralMethods.IsElementExist(By.XPath(String.Format("//span[contains(text(), 'Ends:')]")));
                bool isDate = test.GeneralMethods.IsElementExist(By.XPath(String.Format("//div[contains(text(), '{0}')]", endTime)));

                TestLog.WriteLine("Delete the attribute");
                test.Driver.FindElementByCssSelector("a[class=\"delete float_right\"]").Click();
                test.Driver.SwitchTo().Alert().Accept();
                TestLog.WriteLine("Wait for the attribute group is gone");
                test.GeneralMethods.WaitForElementInexistent(test.Driver, By.LinkText(individualAttributeGroupName));

                Assert.IsTrue(isLabel && isDate, "Either end label or date isn't display correctly.");
            }
            finally
            {
                // Logout of Portal
                test.Portal.LogoutWebDriver();

                TestLog.WriteLine("Delete the attribute group from DB");
                base.SQL.Admin_IndividualAttributeGroups_Delete(churchId, individualAttributeGroupName);
                base.SQL.Admin_IndividualAttributes_Delete(churchId, individualAttributeGroupName, individualAttributeName);
            }

        }
    }
}