using FTTests;
using Gallio.Framework;
using MbUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ActiveUp.Net.Mail;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Common.PageEntitys;
using System.Text.RegularExpressions;

namespace Launchpad
{
    [TestFixture]
    public class Launchpad_UserSetting:FixtureBaseWebDriver
    {
        //[FixtureInitializer]


        //[FixtureTearDown]



        [Test,RepeatOnFailure]
        [Author("ivan zhang")]
        [Category(TestCategories.Services.SmokeTest)]
        [Description("try to modify user information")]
        public void UpdateUserInformation()
        {
            //login launchpad
            //login page elements 
            //Home page elements
            //User setting page elements(all), pwd, private, household list, profile etc.

        }
    }
}
