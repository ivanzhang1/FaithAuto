using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Data;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace FTTests.Dashboard.Login
{
    [TestFixture]
    public class Dashboard_Login : FixtureBaseWebDriver
    {
        
        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verifies default time frame")]
        public void Dashboard_Login_GetAccessToken()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;
            
            try
            {
                DashboardAPIBase api = new DashboardAPIBase();
                HttpWebResponse response = api.getAccessTokenObject(api.ConsumerKey, api.ConsumerSecret, api.Username, api.Password, api.ChurchCode);
                AccessToken token = api.JsonToObject<AccessToken>(response);
                
                TestLog.WriteLine(string.Format("{0}|{1}|{2}|{3}", token.user.firstName, token.user.lastName, token.accessToken, token.tokenType));
                Assert.AreEqual("FT", token.user.firstName);

                test.Dashboard.LoginWebDriver();
                Assert.IsTrue(utility.IsElementExist(By.Id("settings")));
            }
            finally
            {
                //clear test data
            }

        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verifies the error message of login is proper")]
        public void Dashboard_Login_ErrorMessage()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            string[,] accounts = new string[,] { { "", "", "", "Username is required"},
                                                 { "", "FT4life!", "dc", "Username is required"}, 
                                                 { "ft.tester", "", "dc", "Password is required"}, 
                                                 { "ft.tester", "FT4life!", "", "Church code is required"}, 
                                                 { "ft.tester", "FT4life!", "dc$", "Your login attempt has failed. Church is not found."},
                                                 { "ft.tester", "FT4life!*", "dc", "Your login attempt has failed. Your account will be locked out after 10 attempts." }, 
                                                 { "ft.tester&", "FT4life!", "dc", "Your login attempt has failed"},
                                                 { "ft.tester~!@#$%^&*(}>|]", "FT4life!ft.tester~!@#$%^&*(}>|]", "dcft.tester~!@#$%^&*(}>|]", "Your login attempt has failed. Church is not found"}
                                               };

            test.Dashboard.OpenLoginWebDriver();
            DashboardLoginPage loginPage = new DashboardLoginPage(test.Driver, test.GeneralMethods);

            try
            {
                for (int i = 0; i < accounts.GetLength(0); i++)
                {
                    TestLog.WriteLine(string.Format("{0}|{1}|{2}", accounts[i, 0], accounts[i, 1], accounts[i, 2]));
                    loginPage.login(accounts[i,0], accounts[i,1], accounts[i,2]);
                    TestLog.WriteLine(loginPage.getLoginErrorText());
                    Assert.IsTrue(loginPage.getLoginErrorText().Contains(accounts[i,3]));   
                }
            }
            finally
            {
                //clear failed login attempts 
                loginPage.login("ft.tester", "FT4life!", "dc");
            }

        }

    }
}
