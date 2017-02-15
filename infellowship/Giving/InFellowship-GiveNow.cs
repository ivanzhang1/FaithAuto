using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Data;
using System.Drawing;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Firefox;
using System.Text.RegularExpressions;

namespace FTTests.infellowship.Giving {
    [TestFixture]
    [Category(TestCategories.Services.PaymentProcessor)]
    public class infellowship_Giving_GiveNow_WebDriver : FixtureBaseWebDriver {
        #region Private Members
        public struct Contribution {
            public string fund;
            public string subFund;
            public string amount;
            public Contribution(string fundOrPledgeDrive, string SubFund, string Amount) {
                fund = fundOrPledgeDrive;
                subFund = SubFund;
                amount = Amount;
            }
        }

        #endregion Private Members

        #region Responsive

        #region Give Now
        [Test, RepeatOnFailure, Timeout(6000)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies successful VISA credit card transaction in Responsive mode")]
        public void GiveNow_CreditCard_Success_Visa_RESPONSIVE_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.tester2@gmail.com", "FT4life!", "dc");

            // Set window size to simulate iPhone
            test.Infellowship.SetBrowserSizeTo_Mobile();

            // Process VISA credit card for Give Now
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "7.08", "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }
        #endregion Give Now

        #endregion Responsive


        #region Credit Card

        #region Card Type Success
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies successful VISA credit card transaction")]
        public void GiveNow_CreditCard_Success_Visa_WebDriver() 
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process VISA credit card for Give Now
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "2.08", "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies successful MasterCard credit card transaction")]
        public void GiveNow_CreditCard_Success_MasterCard_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process Master Card credit card for Give Now
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "3.09", "FT", "Tester", "Master Card", "5555555555554444", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure, Timeout(8000)]
        [Author("David Martin")]
        [Description("Verifies successful American Express credit card transaction")]
        public void GiveNow_CreditCard_Success_AmericanExpress_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process American Express credit card for Give Now
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "4.34", "FT", "Tester", "American Express", "378282246310005", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies successful Discover credit card transaction")]
        public void GiveNow_CreditCard_Success_Discover_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process Discover credit card for Give Now
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "4.10", "FT", "Tester", "Discover", "6011111111111117", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies successful JCB credit card transaction")]
        public void GiveNow_CreditCard_Success_JCB_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "QAEUNLX0C6");

            // Process JCB credit card for Give Now
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(258, "Auto Test Fund", null, "89.12", "FT", "Tester", "JCB", "3566111111111113", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies successful credit card transaction with Fund and Sub Fund selected")]
        public void GiveNow_CreditCard_Success_FundANDSubFund_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process credit card payment for Give Now selecting both Fund and SubFund
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund w Sub Fund", "Auto Test Sub Fund", "20.20", "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure, Timeout(1200)]
        [Author("Stuart Platt")]
        [Description("Verifies successful credit card transaction to Pledge drive with Fund and Sub Fund")]
        public void GiveNow_CreditCard_Success_ToPledge_FundANDSubFund_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");
            
            // Process credit card payment for Give Now selecting both Fund and SubFund
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Pledge Drive 2", null, "13.01", "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

            // Verify Giving to correct fund and subfund
            test.Infellowship.Giving_GivingHistory_Validate_WebDriver(15, "FT", "Tester", "Auto Test Pledge Drive 2", null, "13.01", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), 0, "Credit Card", true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies successful credit card transaction with multiple Funds/Sub Funds selected")]
        public void GiveNow_CreditCard_Success_MultipleFundsAndSubFunds_WebDriver()
        {
            // Set up contribution data
            var contributionData = new List<dynamic> {
                new Contribution { fund = "Auto Test Fund w Sub Fund", subFund = "Auto Test Sub Fund", amount = "11.00" },
                new Contribution { fund = "Auto Test Fund", subFund = null, amount = "10.00" }
            };

            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            //Process credit card payment for Give Now selecting multiple Funds and Sub Funds
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, contributionData, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        #endregion Card Type Success

        #region CC Validation

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies proper error messages for credit card payment missing all required fields")]
        public void GiveNow_CreditCard_Validation_No_Required_Fields_Selected_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process credit card payment with all fields missing
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "--", string.Empty, string.Empty, string.Empty, string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies credit card payment without fund can not be accepted")]
        public void GiveNow_CreditCard_Validation_No_Fund_Selected_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process credit card payment with no fund selected
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "--", null, "1.11", "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies credit card payment with zero dollars can not be accepted")]
        public void GiveNow_CreditCard_Validation_ZeroDollars_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process credit card payment with ZERO dollar amount
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "0.00", "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies credit card payment without First Name can not be accepted")]
        public void GiveNow_CreditCard_Validation_No_FirstName_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process credit card payment with first name missing
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "5.55", string.Empty, "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies credit card payment without Last Name can not be accepted")]
        public void GiveNow_CreditCard_Validation_No_LastName_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process credit card payment with last name missing
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "5.55", "FT", string.Empty, "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies credit card payment without credit card type can not be accepted")]
        public void GiveNow_CreditCard_Validation_No_CreditCard_Type_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process credit card payment with credit card type not selected
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "6.76", "FT", "Tester", "--", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies credit card payment without credit card number can not be accepted")]
        public void GiveNow_CreditCard_Validation_No_CreditCard_Number_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process credit card payment with credit card number not selected
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "7.77", "FT", "Tester", "Visa", string.Empty, "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies credit card payment without expiration month can not be accepted")]
        public void GiveNow_CreditCard_Validation_No_Expiration_Month_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process credit card payment with expiration month not selected
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "9.79", "FT", "Tester", "Visa", "4111111111111111", "--", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies credit card payment without expiration year can not be accepted")]
        public void GiveNow_CreditCard_Validation_No_Expiration_Year_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process credit card payment with expiration year not selected
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "8.78", "FT", "Tester", "Visa", "4111111111111111", "12 - December", "--", string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the proper error messages appear when character limitations are exceeded for credit card payment")]
        public void GiveNow_CreditCard_Validation_Character_Limits_Exceeded_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process credit card payment with all text fields exceeding their character limits
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "12.24", "123456789012345678901234567890123456789012345678901", "123456789012345678901234567890123456789012345678901", "Visa", "1234567890123456789012345678901", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, false, "United States", "12345678901234567890123456789012345678901", "12345678901234567890123456789012345678901", "1234567890123456789012345678901", "Texas", "123456789012", "123456789012345678901234567890123456789012345678901");

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies that the proper error message appears when submitting a credit card with expired date")]
        public void GiveNow_CreditCard_Validation_ExpiredDate_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Cannot do an expired credit card in January.
            if (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Month == 1)
            {
                Assert.Inconclusive("This test cannot run in January because you cannot provide an expired credit card. The previous year is not an option to select for a credit card's expiration year.  January is the first month of the year, so there is no past months this year.");

            }
            else
            {
                // Process credit card payment with expired credit card date
                test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "13.33", "FT", "Tester", "Visa", "4111111111111111", "1 - January", DateTime.Now.Year.ToString(), string.Empty, true);
            }

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies that the proper error message appears when submitting a Visa credit card payment with Invalid credit card number")]
        public void GiveNow_CreditCard_Validation_Invalid_Visa_Number_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process Visa credit card payment with Invalid credit card number
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "14.44", "FT", "Tester", "Visa", "4111111111111112", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, false);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies that the proper error message appears when submitting a MasterCard credit card payment with Invalid credit card number")]
        public void GiveNow_CreditCard_Validation_Invalid_MasterCard_Number_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process MasterCard credit card payment with Invalid credit card number
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "15.55", "FT", "Tester", "Master Card", "1555555555554444", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, false);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies that the proper error message appears when submitting an American Express credit card payment with Invalid credit card number")]
        public void GiveNow_CreditCard_Validation_Invalid_AmericanExpress_Number_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process AmericanExpress credit card payment with Invalid credit card number
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "16.69", "FT", "Tester", "American Express", "178282246310005", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, false);

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies that the proper error message appears when submitting a Discover credit card payment with Invalid credit card number")]
        public void GiveNow_CreditCard_Validation_Invalid_Discover_Number_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process Discover credit card payment with Invalid credit card number
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "17.77", "FT", "Tester", "Discover", "6811111111111117", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, false);

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies that the proper error message appears when submitting a JCB credit card payment with Invalid credit card number")]
        public void GiveNow_CreditCard_Validation_Invalid_JCB_Number_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "QAEUNLX0C6");

            // Process JCB credit card payment with Invalid credit card number
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(258, "Auto Test Fund", null, "18.88", "FT", "Tester", "JCB", "2566111111111113", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, false);

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the when editing the 'Contribution Details' or 'Payment Information' sections of a credit card payment all your information is auto prefilled")]
        public void GiveNow_CreditCard_Validation_EditContributionPayment_Prefilled_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process credit card payment to Review and Confirm and Edit either "Contribution Details" or "Payment Information" and verify all information is prefilled
            test.Infellowship.Giving_GiveNow_CreditCard_Edit_Step1_WebDriver(15, "Auto Test Fund", null, "21.21", "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, "United States", "6363 N Hwy 161", "Suite 200", "Irving", "Texas", "75038", string.Empty);
            test.Driver.FindElementByLinkText(GeneralInFellowship.Giving.Cancel).Click();

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verify Credit Card Payment Error 150")]
        public void GiveNow_CreditCard_Error_150_WebDriver()
        {
            //Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process credit card for 150 error message
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "2000", "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verify Credit Card Payment Error 201")]
        public void GiveNow_CreditCard_Error_201_WebDriver()
        {
            //Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");
            test.Infellowship.LoginWebDriver("felix.gaytan@activenetwork.com", "FG.Admin12", "dc");

            // Process credit card for 201 error message
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "2402", "Felix", "Gaytan", "Visa", "4111111111111111", "12 - December", "2020", string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verify Credit Card Payment Error 203")]
        public void GiveNow_CreditCard_Error_203_WebDriver()
        {
            //Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process credit card for 203 error message
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "2260", "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verify Credit Card Payment Error 204")]
        public void GiveNow_CreditCard_Error_204_WebDriver()
        {
            //Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process credit card for 204 error message
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "2521", "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verify Credit Card Payment Error 205")]
        public void GiveNow_CreditCard_Error_205_WebDriver()
        {
            //Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process credit card for 205 error message
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "2501", "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verify Credit Card Payment Error 208")]
        public void GiveNow_CreditCard_Error_208_WebDriver()
        {
            //Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process credit card for 208 error message
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "2606", "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verify Credit Card Payment Error 209")]
        public void GiveNow_CreditCard_Error_209_WebDriver()
        {
            //Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process credit card for 209 error message
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "2811", "FT", "Tester", "American Express", "378282246310005", "12 - December", "2020", string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure, Timeout(6000)]
        [Author("David Martin")]
        [Description("Verify Credit Card Payment Error 211")]
        public void GiveNow_CreditCard_Error_211_WebDriver()
        {
            //Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process credit card for 211 error message
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "2531", "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verify Credit Card Payment Error 233")]
        public void GiveNow_CreditCard_Error_233_WebDriver()
        {
            //Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process credit card for 233 error message
            test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "2204", "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", string.Empty, true);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        #endregion CC Validation

        #region AMS Error Code Validation
        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("Tests AMS response code 211 - Invalid card verification number.")]
        public void GiveNow_CreditCard_Error_AMS_211_WebDriver()
        {
            if (base.SQL.IsAMSEnabled(15))
            {
                //Login to infellowship
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

                // Process credit card for 233 error message
                test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Simulated Fund", null, "2", "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", string.Empty, true);

                // Logout of infellowship
                test.Infellowship.LogoutWebDriver();

            }
        }

        //Commenting this test case until DEV team added the corresponding return code to PaymentReason table
        //[Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("Tests AMS response code 930144 - Approved not Processed.CVV2 Required")]
        public void GiveNow_CreditCard_Error_AMS_930144_WebDriver()
        {
            if (base.SQL.IsAMSEnabled(15))
            {
                //Login to infellowship
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

                // Process credit card for 233 error message
                test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Simulated Fund", null, "4", "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", string.Empty, true);

                // Logout of infellowship
                test.Infellowship.LogoutWebDriver();

            }
        }

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("Tests AMS response code 230 - Declined by CyberSource")]
        public void GiveNow_CreditCard_Error_AMS_230_WebDriver()
        {
            if (base.SQL.IsAMSEnabled(15))
            {
                //Login to infellowship
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

                // Process credit card for 233 error message
                test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Simulated Fund", null, "5", "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", string.Empty, true);

                // Logout of infellowship
                test.Infellowship.LogoutWebDriver();

            }
        }

        //Commenting this test case until DEV team added the corresponding return code to PaymentReason table
        //[Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("Tests AMS response code 0 - Approved not Processed.")]
        public void GiveNow_CreditCard_Error_AMS_ZERO_WebDriver()
        {
            if (base.SQL.IsAMSEnabled(15))
            {
                //Login to infellowship
                TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
                test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

                // Process credit card for 233 error message
                test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Simulated Fund", null, "3", "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", string.Empty, true);

                // Logout of infellowship
                test.Infellowship.LogoutWebDriver();

            }
        }
        #endregion AMS Error Code Validation

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("F1-4085: Verifies credit card images are present")]
        public void GiveNow_CreditCard_Verify_CreditCard_Images_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");            

            //Go to Give Now CC
            try
            {
                test.Driver.FindElementByLinkText("Your Giving").Click();
                test.Driver.FindElementByPartialLinkText("Give Now").Click();
            }
            catch (Exception e)
            {
                //If have been logged out, let's try one more time
                if (test.GeneralMethods.IsElementPresentWebDriver(By.Id("username")))
                {
                    test.Infellowship.ReLoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");
                    test.Driver.FindElementByLinkText("Your Giving").Click();
                    test.Driver.FindElementByPartialLinkText("Give Now").Click();
                }
                else
                {
                    throw new Exception(e.Message, e);

                }

            }

            test.Driver.FindElementById("payment_method_cc").Click();

            //Click on CC
            test.Driver.FindElementById("new_card").Click();

            //Verify CC Images
            test.GeneralMethods.Verify_CC_Images();

            // Exit before logout
            test.Driver.FindElementByLinkText(GeneralInFellowship.Giving.Cancel).Click();

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("FO-7353: The image of the Previously used Card  is same in the Schedule Giving of the Infellowship")]
        public void GiveNow_CreditCard_Verify_UsedCard_Images_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            //Go to Give Now CC
            try
            {
                test.Driver.FindElementByLinkText("Your Giving").Click();
                test.Driver.FindElementByPartialLinkText("Give Now").Click();
            }
            catch (Exception e)
            {
                //If have been logged out, let's try one more time
                if (test.GeneralMethods.IsElementPresentWebDriver(By.Id("username")))
                {
                    test.Infellowship.ReLoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");
                    test.Driver.FindElementByLinkText("Your Giving").Click();
                    test.Driver.FindElementByPartialLinkText("Give Now").Click();
                }
                else
                {
                    throw new Exception(e.Message, e);

                }

            }

            test.Driver.FindElementById("payment_method_cc").Click();

            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.visa").Text.StartsWith("Visa"), "Used visa card isn't display as expected");
            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.mastercard").Text.StartsWith("Mastercard"), "Used mastercard isn't display as expected");
            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.amex").Text.StartsWith("AMEX"), "Used AMEX card isn't display as expected");
            Assert.IsTrue(test.Driver.FindElementByCssSelector(".payment.discover").Text.StartsWith("Discover"), "Used discover card isn't display as expected");

            // Exit before logout
            test.Driver.FindElementByLinkText(GeneralInFellowship.Giving.Cancel).Click();

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies security code images in tooltip are present")]
        public void GiveNow_CreditCard_Verify_CreditCard_SecurityCode_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            //Go to Give Now CC
            try
            {
                test.Driver.FindElementByLinkText("Your Giving").Click();
                test.Driver.FindElementByPartialLinkText("Give Now").Click();
            }
            catch (Exception e)
            {
                //If have been logged out, let's try one more time
                if (test.GeneralMethods.IsElementPresentWebDriver(By.Id("username")))
                {
                    test.Infellowship.ReLoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");
                    test.Driver.FindElementByLinkText("Your Giving").Click();
                    test.Driver.FindElementByPartialLinkText("Give Now").Click();
                }
                else
                {
                    throw new Exception(e.Message, e);

                }

            }

            test.Driver.FindElementById("payment_method_cc").Click();

            //Click on CC
            test.Driver.FindElementById("new_card").Click();

            //Verify Security code tool tip
            test.GeneralMethods.Verify_GivingNow_SecurityCode_Tooltip();

            // Exit before logout
            test.Driver.FindElementByLinkText(GeneralInFellowship.Giving.Cancel).Click();

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        #endregion Credit Card

        #region Personal Check

        #region Personal Check Success
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies successful Personal Check transaction")]
        public void GiveNow_PersonalCheck_Success_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process personal check payment for Give Now
            test.Infellowship.Giving_GiveNow_PersonalCheck_WebDriver(15, "FT", "Tester", "Auto Test Fund", null, "9.99", "8176833453", "111000025", "111222333");

            // Logout of InFfellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies successful personal check transaction with Fund and Sub Fund selected")]
        public void GiveNow_PersonalCheck_Success_FundANDSubFund_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process personal check payment for Give Now selecting both Fund and SubFund
            test.Infellowship.Giving_GiveNow_PersonalCheck_WebDriver(15, "FT", "Tester", "Auto Test Fund w Sub Fund", "Auto Test Sub Fund", "88.88", "8176833453", "111000025", "111222333");

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies successful personal check transaction with multiple Funds/Sub Funds selected")]
        public void GiveNow_PersonalCheck_Success_MultipleFundsAndSubFunds_WebDriver()
        {
            #region Data
            // Data
            int churchId = 15;
            string fund1 = "Auto Test Fund w Sub Fund";
            string subFund1 = "Auto Test Sub Fund";
            string fund2 = "Auto Test Fund";
            string amount1 = "11.37";
            string amount2 = "12.37";

            // Set up contribution data
            var contributionData = new List<dynamic> {
                new Contribution { fund = fund1, subFund = subFund1, amount = amount1 },
                new Contribution { fund = fund2, subFund = null, amount = amount2 }
            };

            #endregion Data

            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process personal check payment for Give Now selecting multiple Funds and Sub Funds
            test.Infellowship.Giving_GiveNow_PersonalCheck_WebDriver(churchId, "FT", "Tester", contributionData, "8176833453", "111000025", "111222333");

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

        }
        #endregion Personal check Success

        #region Personal Check Validation
        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies proper error messages for personal check payment missing all required fields")]
        public void GiveNow_PersonalCheck_Validation_No_Required_Fields_Selected_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process personal check payment with all fields missing
            test.Infellowship.Giving_GiveNow_PersonalCheck_WebDriver(15, null, null, null, null, null, null, null, null);

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the proper error messages appear when character limitations are exceeded for personal check payment")]
        public void GiveNow_PersonalCheck_Validation_Character_Limits_Exceeded_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process personal check payment with all fields exceeding thier character limits
            test.Infellowship.Giving_GiveNow_PersonalCheck_WebDriver(15, "FT", "Tester", "Auto Test Fund", null, "20.01", "123456789012345678901234567890123456789012345678901234", "1234567890", "123456789012345678901234567890123456789012345678901234");

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies when editing the 'Contribution Details' or 'Payment Information' sections of a personal check payment all your information is auto prefilled")]
        public void GiveNow_PersonalCheck_Validation_EditContributionPayment_Prefilled_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process personal check payment to Review and Confirm and Edit either "Contribution Details" or "Payment Information" and verify all information is prefilled
            test.Infellowship.Giving_GiveNow_PersonalCheck_Edit_Step1_WebDriver(15, "Auto Test Fund", null, "21.12", "8176833453", "111000025", "111222333");
            test.Driver.FindElementByLinkText(GeneralInFellowship.Giving.Cancel).Click();

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure, DoesNotRunInStaging, Timeout(1000)]
        [Author("David Martin")]
        [Description("Verifies the exception for exceeding the velocity count.")]
        public void GiveNow_PersonalCheck_Velocity_Transactions_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "QAEUNLX0C2");

            // Populate the payment information
            test.Infellowship.Giving_GiveNow_PersonalCheck_WebDriver(254, "FT", "Tester", "Auto Test Fund", null, "1102", "6302172170", "111000025", "123123123");

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure, DoesNotRunInStaging, Timeout(1000)]
        [Author("David Martin")]
        [Description("Verifies the exception for exceeding the velocity dollar amount.")]
        public void GiveNow_PersonalCheck_Velocity_DollarAmount_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "QAEUNLX0C2");

            // Populate the payment information
            test.Infellowship.Giving_GiveNow_PersonalCheck_WebDriver(254, "FT", "Tester", "Auto Test Fund", null, "1101", "6302172170", "111000025", "123123123");

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure, DoesNotRunInStaging, Timeout(1000)]
        [Author("David Martin")]
        [Description("Verifies the exception for exceeding the velocity transaction amount.")]
        public void GiveNow_PersonalCheck_Velocity_TransactionAmount_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "QAEUNLX0C2");

            // Populate the payment information
            test.Infellowship.Giving_GiveNow_PersonalCheck_WebDriver(254, "FT", "Tester", "Auto Test Fund", null, "1102", "6302172170", "111000025", "123123123");

            // Logout of infellowship
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Suchitra")]
        [Description("F1-5427 - Verifies transaction is successful when Personal check is changes to CC)")]
        public void GiveNow_PersonalCheck_To_CC_Success()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

            // Process personal check payment for Give Now
            test.Infellowship.Giving_GiveNow_PersonalCheck_Edit_Step1_WebDriver(15, "Auto Test Fund", null, "9.99", "8176833453", "111000025", "111222333");

            //Click Edit to change to Credit card to process transaction
            test.Driver.FindElementById("payment_method_cc").Click();
            
            //Process credit card information
            // Process VISA credit card for Give Now
          //  .Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, "2.08", "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);
           // test.Infellowship.Giving_GiveNow_Populate_Step1_WebDriver(15 "Credit Card", contributionData, "FT", "Tester", "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, "United States", "6363 N Hwy 161", "Suite 200", "Irving", "Texas", "75038", string.Empty, null, null, null, null, null, null);

            test.Infellowship.Giving_GiveNow_CreditCard_Process_Step1("Auto Test Fund", null, "2.00");

            // Logout of InFfellowship
            test.Infellowship.LogoutWebDriver();

        }

        #endregion Personal Check Validation

        #endregion Personal Check
    }

    [TestFixture]
    [Category(TestCategories.Services.PaymentProcessor)]
    public class infellowship_Giving_GiveNow_WithoutAccount_WebDriver : FixtureBaseWebDriver
    {
        #region Private Members
        public struct Contribution
        {
            public string fund;
            public string subFund;
            public string amount;
            public Contribution(string fundOrPledgeDrive, string SubFund, string Amount)
            {
                fund = fundOrPledgeDrive;
                subFund = SubFund;
                amount = Amount;
            }
        }

        #endregion Private Members

        #region Responsive

        #region Give Now Without Account

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies an individual can edit their fund/subfund information from the Review and Confirm screen on Give Now without account in Responsive mode.")]
        public void GiveNow_WithoutAccount_CreditCard_Edit_FundSubFundAmount_RESPONSIVE_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund1 = "Auto Test Fund w Sub Fund";
            string subFund1 = "Auto Test Sub Fund";
            string fund2 = "Auto Test Fund";
            string amount1 = "322.33";
            string amount2 = "344.55";
            string firstName = "GW";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";

            // Set up contribution data
            var contributionData = new List<dynamic> 
            {
                new Contribution { fund = fund1, subFund = subFund1, amount = amount1 }
            };

            var contributionData2 = new List<dynamic> 
            {
                new Contribution { fund = fund2, subFund = null, amount = amount2 }
            };
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Set window size to simulate iPhone
            test.Infellowship.SetBrowserSizeTo_Mobile();

            // Click on the Give Now link
            test.Driver.FindElementByLinkText(GeneralInFellowship.Giving.GIVE_NOW).Click();
            test.GeneralMethods.WaitForElement(By.Id(string.Format("{0}0", GeneralInFellowship.Giving.Fund)));

            // Fill in fund/subfund/amount information
            test.Infellowship.GivingWithoutAccount_PopulateFundSubFundAmountInformation_WebDriver(contributionData);

            // Fill in the name
            test.Infellowship.GivingWithoutAccount_PopulateFirstLastNameEmail_WebDriver(firstName, lastName, emailAddress);

            // Fill in the payment information
            test.Infellowship.GivingWithoutAccount_PopulatePaymentInformation_WebDriver(churchId, GeneralEnumerations.GiveByTypes.Credit_card, firstName, lastName, string.Empty, string.Empty, string.Empty, cardType, cardNumber, expireMonth, expireYear,
                string.Empty, country, address1, string.Empty, city, state, zipCode, string.Empty);

            // Click to Continue
            test.Driver.FindElementByXPath(GeneralInFellowship.Giving.Continue_Responsive).Click();

            // Verify information is correct
            Assert.AreEqual(string.Format("{0} > {1}", fund1, subFund1), test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[1]/td", TableIds.InFellowship_ContributionDetails_GiveNowWithoutAccount)).Text);
            Assert.AreEqual(string.Format("${0}", amount1), test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[1]/td[2]", TableIds.InFellowship_ContributionDetails_GiveNowWithoutAccount)).Text.Trim());

            // Click on the Edit link next to the Fund/Subfund section
            test.Driver.FindElementByXPath(GeneralInFellowship.Giving.Edit_ContributionDetails).Click();
            test.GeneralMethods.WaitForElement(By.Id(string.Format("{0}0", GeneralInFellowship.Giving.Fund)));

            // Change the fund/subfund/amount information
            test.Infellowship.GivingWithoutAccount_PopulateFundSubFundAmountInformation_WebDriver(contributionData2);

            // Enter Card Number again
            test.Driver.FindElementById(GeneralInFellowship.Giving.CreditCard_Number).SendKeys(cardNumber);

            // Click continue back to Review and Confirm
            test.Driver.FindElementByXPath(GeneralInFellowship.Giving.Continue_Responsive).Click();

            // Verify updates to fund/subfund/amount
            Assert.AreEqual(string.Format("{0}", fund2), test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[1]/td", TableIds.InFellowship_ContributionDetails_GiveNowWithoutAccount)).Text);
            Assert.AreEqual(string.Format("${0}", amount2), test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[1]/td[2]", TableIds.InFellowship_ContributionDetails_GiveNowWithoutAccount)).Text.Trim());

            // Close page
            test.Driver.Close();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies validation message when a user does not select a fund")]
        public void GiveNow_WithoutAccount_CreditCard_Validation_NoFields_RESPONSIVE_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "--";
            string amount = string.Empty;
            string firstName = string.Empty;
            string lastName = string.Empty;
            string emailAddress = string.Empty;
            string cardType = "--";
            string cardNumber = string.Empty;
            string expireMonth = "--";
            string expireYear = "--";
            string country = "United States";
            string address1 = string.Empty;
            string city = string.Empty;
            string state = "--";
            string zipCode = string.Empty;
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Set window size to simulate iPhone
            test.Infellowship.SetBrowserSizeTo_Mobile();

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies validation message when a user does not enter their phone number")]
        public void GiveNow_WithoutAccount_PersonalCheck_Validation_NoFields_RESPONSIVE_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "--";
            string amount = string.Empty;
            string firstName = string.Empty;
            string lastName = string.Empty;
            string emailAddress = string.Empty;
            string phoneNumber = string.Empty;
            string routingNumber = string.Empty;
            string accountNumber = string.Empty;
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Set window size to simulate iPhone
            test.Infellowship.SetBrowserSizeTo_Mobile();

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_PersonalCheck(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Personal_check, phoneNumber, routingNumber, accountNumber, false);

        }

        #endregion Give Now Without Account

        #endregion Responsive

        #region Give Now View

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("Verifies the content appears properly in give now screen")]
        public void GiveNow_UK_View_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "QAEUNLX0C6");

            // Navigate to the giving history page
            test.Driver.FindElementByXPath(Navigation.InFellowship.Your_Giving).Click();

            // Click on the Give Now button
            test.Driver.FindElementByLinkText("Give Now").Click();

            // Click to add another row
            test.Driver.FindElementById("add_row").Click();

            // Verify user is on the Schedules page
            Assert.AreEqual("£", test.Driver.FindElementByXPath(string.Format("{0}/li[2]/div/div[3]/div/span", TableIds.InFellowship_GiveNowTable)).Text);
            Assert.AreEqual("£", test.Driver.FindElementByXPath(string.Format("{0}/li[3]/div/div[3]/div/span", TableIds.InFellowship_GiveNowTable)).Text);
            Assert.AreEqual("£0.00\r\nTotal", test.Driver.FindElementByXPath(string.Format("{0}/li[4]/div/div[2]", TableIds.InFellowship_GiveNowTable)).Text);

            test.Driver.FindElementByLinkText("Cancel").Click();

            // Logout of Infellowship
            test.Infellowship.LogoutWebDriver();

        }

        #endregion

        #region Give Now Without Account

        #region Credit Card

        #region Validation

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies an individual can edit their fund/subfund information from the Review and Confirm screen on Give Now without account")]
        public void GiveNow_WithoutAccount_CreditCard_Edit_FundSubFundAmount_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund1 = "Auto Test Fund w Sub Fund";
            string subFund1 = "Auto Test Sub Fund";
            string fund2 = "Auto Test Fund";
            string amount1 = "22.33";
            string amount2 = "44.55";
            string firstName = "GW";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";

            // Set up contribution data
            var contributionData = new List<dynamic> 
            {
                new Contribution { fund = fund1, subFund = subFund1, amount = amount1 }
            };

            var contributionData2 = new List<dynamic> 
            {
                new Contribution { fund = fund2, subFund = null, amount = amount2 }
            };
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Click on the Give Now link
            test.Driver.FindElementByLinkText(GeneralInFellowship.Giving.GIVE_NOW).Click();
            test.GeneralMethods.WaitForElement(By.Id(string.Format("{0}0", GeneralInFellowship.Giving.Fund)));

            // Fill in fund/subfund/amount information
            test.Infellowship.GivingWithoutAccount_PopulateFundSubFundAmountInformation_WebDriver(contributionData);

            // Fill in the name
            test.Infellowship.GivingWithoutAccount_PopulateFirstLastNameEmail_WebDriver(firstName, lastName, emailAddress);

            // Fill in the payment information
            test.Infellowship.GivingWithoutAccount_PopulatePaymentInformation_WebDriver(churchId, GeneralEnumerations.GiveByTypes.Credit_card, firstName, lastName, string.Empty, string.Empty, string.Empty, cardType, cardNumber, expireMonth, expireYear,
                string.Empty, country, address1, string.Empty, city, state, zipCode, string.Empty);

            // Click to Continue
            if (test.GeneralMethods.IsElementVisibleWebDriver(By.XPath(GeneralInFellowship.Giving.Continue)))
            {
                test.Driver.FindElementByXPath(GeneralInFellowship.Giving.Continue).Click();
            }
            else
            {
                test.Driver.FindElementByXPath(GeneralInFellowship.Giving.Continue_Responsive).Click();
            }

            // Verify information is correct
            Assert.AreEqual(string.Format("{0} > {1}", fund1, subFund1), test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[1]/td", TableIds.InFellowship_ContributionDetails_GiveNowWithoutAccount)).Text);
            Assert.AreEqual(string.Format("${0}", amount1), test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[1]/td[2]", TableIds.InFellowship_ContributionDetails_GiveNowWithoutAccount)).Text.Trim());

            // Click on the Edit link next to the Fund/Subfund section
            test.Driver.FindElementByXPath(GeneralInFellowship.Giving.Edit_ContributionDetails).Click();
            test.GeneralMethods.WaitForElement(By.Id(string.Format("{0}0", GeneralInFellowship.Giving.Fund)));

            // Change the fund/subfund/amount information
            test.Infellowship.GivingWithoutAccount_PopulateFundSubFundAmountInformation_WebDriver(contributionData2);

            // Enter Card Number again
            test.Driver.FindElementById(GeneralInFellowship.Giving.CreditCard_Number).SendKeys(cardNumber);

            // Click continue back to Review and Confirm
            if (test.GeneralMethods.IsElementVisibleWebDriver(By.XPath(GeneralInFellowship.Giving.Continue)))
            {
                test.Driver.FindElementByXPath(GeneralInFellowship.Giving.Continue).Click();
            }
            else
            {
                test.Driver.FindElementByXPath(GeneralInFellowship.Giving.Continue_Responsive).Click();
            }

            // Verify updates to fund/subfund/amount
            Assert.AreEqual(string.Format("{0}", fund2), test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[1]/td", TableIds.InFellowship_ContributionDetails_GiveNowWithoutAccount)).Text);
            Assert.AreEqual(string.Format("${0}", amount2), test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr[1]/td[2]", TableIds.InFellowship_ContributionDetails_GiveNowWithoutAccount)).Text.Trim());

            // Close page
            test.Driver.Close();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies validation message when a user does not select a fund")]
        public void GiveNow_WithoutAccount_CreditCard_Validation_NoFund_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "--";
            string amount = "1.37";
            string firstName = "FT";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies validation message when a user does not enter their first name")]
        public void GiveNow_WithoutAccount_CreditCard_Validation_NoFirstName_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "1.37";
            string firstName = string.Empty;
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies validation message when a user does not enter their last name")]
        public void GiveNow_WithoutAccount_CreditCard_Validation_NoLastName_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "1.37";
            string firstName = "FT";
            string lastName = string.Empty;
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false);

        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies validation message when a user does not enter their email address")]
        public void GiveNow_WithoutAccount_CreditCard_Validation_NoEmailAddress_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "1.37";
            string firstName = "FT";
            string lastName = "Tester";
            string emailAddress = string.Empty;
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false);

        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Stuart Platt")]
        [Description("Verifies validation message when a user enters invalid email address")]
        public void GiveNow_WithoutAccount_CreditCard_Validation_InvalidEmailAddress_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "1.39";
            string firstName = "FT";
            string lastName = "Tester";
            string emailAddress = "invalid email";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false);
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies validation message when a user does not select a card type")]
        public void GiveNow_WithoutAccount_CreditCard_Validation_NoCardType_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "1.37";
            string firstName = "FT";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "--";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies validation message when a user does not enter their card number")]
        public void GiveNow_WithoutAccount_CreditCard_Validation_NoCardNumber_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "1.37";
            string firstName = "FT";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Visa";
            string cardNumber = string.Empty;
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies validation message when a user does not select an expiration month")]
        public void GiveNow_WithoutAccount_CreditCard_Validation_NoExpireMonth_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "1.37";
            string firstName = "FT";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "--";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies validation message when a user does not select an expiration year")]
        public void GiveNow_WithoutAccount_CreditCard_Validation_NoExpireYear_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "1.37";
            string firstName = "FT";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = "--";
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies validation message when a user does not enter their address line 1")]
        public void GiveNow_WithoutAccount_CreditCard_Validation_NoAddressLine1_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "1.37";
            string firstName = "FT";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = string.Empty;
            string city = "Dallas";
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies validation message when a user does not enter their city")]
        public void GiveNow_WithoutAccount_CreditCard_Validation_NoCity_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "1.37";
            string firstName = "FT";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = string.Empty;
            string state = "Texas";
            string zipCode = "75201";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies validation message when a user does not select a state")]
        public void GiveNow_WithoutAccount_CreditCard_Validation_NoState_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "1.37";
            string firstName = "FT";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "--";
            string zipCode = "75201";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies validation message when a user does not enter their zip code")]
        public void GiveNow_WithoutAccount_CreditCard_Validation_NoZipCode_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "1.37";
            string firstName = "FT";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string cardType = "Visa";
            string cardNumber = "4111111111111111";
            string expireMonth = "12 - December";
            string expireYear = (DateTime.Now.Year + 1).ToString();
            string country = "United States";
            string address1 = "717 N. Harwood St.";
            string city = "Dallas";
            string state = "Texas";
            string zipCode = string.Empty;
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_CreditCard(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Credit_card, cardType, cardNumber, expireMonth, expireYear, string.Empty,
                country, address1, string.Empty, city, state, zipCode, string.Empty, false);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies the credit card images for give now without account page.")]
        public void GiveNow_WithoutAccount_CreditCard_Validation_CreditCard_Images_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Click on the Give Now link for no account
            test.Driver.FindElementByLinkText(GeneralInFellowship.Giving.GIVE_NOW).Click();
            test.GeneralMethods.WaitForElement(By.Id(GeneralInFellowship.Giving.Amount));

            // Click on the Bank card bullet
            if (!test.GeneralMethods.IsElementVisibleWebDriver(By.Id(GeneralInFellowship.Giving.CreditCard_Type)))
            {
                test.Driver.FindElementById(GeneralInFellowship.Giving.CreditCard_Bullet).Click();
            }

            // Verify card images
            test.GeneralMethods.Verify_CC_Images();

            // Exit
            test.Driver.FindElementByLinkText(GeneralInFellowship.Giving.Cancel).Click();

        }
        #endregion Validation


        #endregion Credit Card

        #region Personal Check

        #region Validation
        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies validation message when a user does not enter their phone number")]
        public void GiveNow_WithoutAccount_PersonalCheck_Validation_NoPhoneNumber_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "4.37";
            string firstName = "FT";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string phoneNumber = string.Empty;
            string routingNumber = "111000025";
            string accountNumber = "111222333";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_PersonalCheck(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Personal_check, phoneNumber, routingNumber, accountNumber, false);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies validation message when a user does not enter their routing number")]
        public void GiveNow_WithoutAccount_PersonalCheck_Validation_NoRoutingNumber_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "4.37";
            string firstName = "FT";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string phoneNumber = "8172743453";
            string routingNumber = string.Empty;
            string accountNumber = "111222333";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_PersonalCheck(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Personal_check, phoneNumber, routingNumber, accountNumber, false);

        }

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Verifies validation message when a user does not enter their routing number")]
        public void GiveNow_WithoutAccount_PersonalCheck_Validation_InvalidRoutingNumber_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "4.37";
            string firstName = "FT";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string phoneNumber = "8172743453";
            string routingNumber = "10000";
            string accountNumber = "111222333";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_PersonalCheck(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Personal_check, phoneNumber, routingNumber, accountNumber, false);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies validation message when a user does not enter their account number")]
        public void GiveNow_WithoutAccount_PersonalCheck_Validation_NoAccountNumber_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "4.37";
            string firstName = "FT";
            string lastName = "Tester";
            string emailAddress = "ft.autotester@gmail.com";
            string phoneNumber = "8172743453";
            string routingNumber = "111000025";
            string accountNumber = string.Empty;
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_PersonalCheck(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Personal_check, phoneNumber, routingNumber, accountNumber, false);

        }

        [Test, RepeatOnFailure, Timeout(6000)]
        [Author("Daniel Lee")]
        [Description("Verifies a successful giving with personal check")]
        public void GiveNow_WithoutAccount_PersonalCheck_Success_WebDriver()
        {
            // Data
            #region Data
            int churchId = 15;
            string fund = "Auto Test Fund";
            string amount = "4.37";
            string firstName = "daniel";
            string lastName = "Lee";
            string emailAddress = "daniel.lee@activenetwork.com";
            string phoneNumber = "8172743453";
            string routingNumber = "111000025";
            string accountNumber = "111222333";
            #endregion Data

            // Launch Infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Infellowship.OpenWebDriver("DC");

            // Use the Give Now link to make a contribution without creating an account
            test.Infellowship.Giving_GiveNow_WithoutAccount_PersonalCheck(churchId, fund, string.Empty, amount, firstName, lastName, emailAddress, GeneralEnumerations.GiveByTypes.Personal_check, phoneNumber, routingNumber, accountNumber, false);

        }
        #endregion Validation

        #endregion Personal Check

        #endregion Give Now Without Account



    }

}
