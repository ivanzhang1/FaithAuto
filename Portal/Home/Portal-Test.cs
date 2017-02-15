using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using ActiveUp.Net.Mail;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;


namespace FTTests.Portal.Test
{

    [TestFixture]
    public class Portal_Launch_Website_WebDriver : FixtureBaseWebDriver
    {
        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("")]
        public void Merge_Individual()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Login To Portal");
            test.Portal.LoginWebDriver("ft.tester", "FT4life!");

            // Run merge individual proc
            test.SQL.People_MergeIndividual(15, "Bob Jones", "Merge Dump");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("")]
        public void Infellowship_Giving()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            
            test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!");

            test.Infellowship.SetBrowserSizeTo_Mobile();

            test.Driver.FindElementByLinkText("Your Giving").Click();

            int uniqueRow = 0;
            string amount = "10.00";

            int rows = test.GeneralMethods.GetTableRowCountWebDriver(TableIds.InFellowship_GivingHistory);
            IWebElement givingTable = test.Driver.FindElementByXPath(TableIds.InFellowship_GivingHistory);
            for (int r = 1; r < rows; r++)
            {
                string peekText = givingTable.FindElements(By.TagName("tr"))[r].FindElements(By.TagName("td"))[2].Text;

                if(peekText.Contains(amount)){
                    uniqueRow = r;
                    break;
                }

                TestLog.WriteLine("Text: " + peekText);

            }

            TestLog.WriteLine("Row Found: " + uniqueRow);

            // Logout of Portal
            test.Infellowship.LogoutWebDriver();

        }

        //[Test, RepeatOnFailure, Timeout(300000)]        
        [Author("Felix Gaytan")]
        [Description("Launch Portal")]
        [Category("PortalLaunch_Test")]
        public void Portal_Launch_Website()
        {
            // Login to portal

                        TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            
                        TestLog.WriteLine("Login To Portal");
                        test.Portal.LoginWebDriver("ft.tester", "FT4life!");

                        test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);
                        test.Driver.FindElementByLinkText("Groups").Click();
                        test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ucGrpGrid_grdGroups_1").Click();

                        int rows = test.GeneralMethods.GetTableRowCountWebDriver(TableIds.Groups_ViewAll_GroupList_AllTab);

                        for (int r = 1; r < rows; r++)
                        {
                            if (test.Driver.FindElements(By.TagName("tr"))[r].FindElements(By.TagName("td"))[0].FindElement(By.TagName("a")).Text.Contains("Group-"))
                            {
                                TestLog.WriteLine("Found {0}", test.Driver.FindElements(By.TagName("tr"))[r].FindElements(By.TagName("td"))[0].FindElement(By.TagName("a")).Text);
                                test.Driver.FindElements(By.TagName("tr"))[r].FindElements(By.TagName("td"))[0].FindElement(By.TagName("a")).Click();
                                //break;
                                test.Driver.FindElementByLinkText("View group settings").Click();
                                test.Driver.FindElementByLinkText("Delete this group").Click();
                                test.Driver.SwitchTo().Alert().Accept();
                            }

                            test.Driver.FindElementByLinkText("Groups").Click();
                            test.Driver.FindElementById("ctl00_ctl00_MainContent_content_ucGrpGrid_grdGroups_1").Click();

                            r = 1;

                        }


            /*
                        // Set initial conidiitions
                        string individualName = "Felix Gaytan";
                        string groupName = "History Attendance - FGJ";
                        string groupTypeName = "Automation Tests";
                        List<string> individualsInGroup = new List<string>();
                        individualsInGroup.Add("Felix Gaytan");
                        individualsInGroup.Add("Erica Gaytan");

                        base.SQL.Groups_Group_Delete(15, groupName);
                        base.SQL.Groups_Group_Create(15, groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(-2).ToShortDateString());
                        base.SQL.Groups_Group_AddLeaderOrMember(15, groupName, individualName, "Leader", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(-2).ToShortDateString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(-2).ToShortDateString());
                        base.SQL.Groups_Group_AddLeaderOrMember(15, groupName, "Erica Gaytan", "Member", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(-2).ToShortDateString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddMonths(-2).ToShortDateString());
                        //base.SQL.Groups_Group_UpdateMemberJoinDate(15, individualName, groupName, "-", 30);
                        //base.SQL.Groups_Group_UpdateMemberJoinDate(15, "Erica Gaytan", groupName, "-", 30);
                        //var date1 = base.SQL.Groups_Group_CreateAttendanceRecord(15, groupName, individualsInGroup, true, true, "-30");
                        //var date2 = base.SQL.Groups_Group_CreateAttendanceRecord(15, groupName, individualsInGroup, false, true, "-15");
                        var date3 = base.SQL.Groups_Group_CreateAttendanceRecord(15, groupName, individualsInGroup, false, false, "-30");

                        //TestLog.WriteLine("base.SQL.Groups_Group_CreateAttendanceRecord(15, groupName, individualsInGroup, true, true, '-30'): {0}", date1);
                        //TestLog.WriteLine("base.SQL.Groups_Group_CreateAttendanceRecord(15, groupName, individualsInGroup, false, true, '-15'): {0}", date2);
                        TestLog.WriteLine("base.SQL.Groups_Group_CreateAttendanceRecord(15, groupName, individualsInGroup, false, false, '-30'): {0}", date3);


            //            test.Portal.People_ViewIndividual_WebDriver(individualName);

                        //base.SQL.Groups_Group_DeleteAttendanceSummary(15, groupName);
            //            base.SQL.Groups_Group_Delete(15, groupName);

            //            test.Portal.People_ViewIndividual_WebDriver(individualName);

                        //test.SQL.Giving_Batches_Delete_RDC(15, "", "1234567890", "111000025");

                        //TestLog.WriteLine("Search for Individual");
                        //test.Portal.People_ViewIndividual_WebDriver("Felix Gaytan");

                        //test.Portal.People_Generate_BackgroundCheck_WebDriver("Matthew Sneeden", "09/02/1982", "111111111", "Background Checks", "Felix Gaytan");
                        //test.Portal.People_Generate_BackgroundCheck_WebDriver("Matthew Sneeden", "09/02/1982", "111111112", "Background Checks", "Felix Gaytan");
                        //test.Portal.People_Generate_BackgroundCheck_WebDriver("Matthew Sneeden", "09/02/1982", "111111113", "Background Checks", "Felix Gaytan");
                        //test.Portal.People_Generate_BackgroundCheck_WebDriver("Matthew Sneeden", "09/02/1982", "111111114", "Background Checks", "Felix Gaytan");

                        //test.Portal.People_Delete_Requirements_WebDriver("Background Check", "Matthew Sneeden", "", "Today", "");
                        //test.Portal.People_Delete_Requirements_WebDriver("Background Check", "Matthew Sneeden", "", "Today", "", true);
                        //test.Portal.People_Delete_Requirements_WebDriver("Background Check", "Matthew Sneeden", "", "", "", true);

            */
                        TestLog.WriteLine("Logout");
                        test.Portal.LogoutWebDriver();
            
        }

        // NOTE: Performance Testing Setup
        #region PerfTestSetup
        /*
        

        [Test, RepeatOnFailure]
        [Author("Felix Gaytan")]
        [Description("Launch Portal")]
        [Category("PortalLaunch_Test")]
        public void Portal_Add_Groups_Leader_Member_Etc_()
        {
            // Login to portal

            //TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            //string activationCode = base.SQL.Groups_FetchUserActivationCode(Convert.ToInt16(15), "perftester@null.active.com");

            //TestLog.WriteLine("Activation Code: " + activationCode);
            int i = 0;

            base.SQL.Groups_Group_AddLeaderOrMember(15, "Performance Group", string.Format("Perf{0} TestAccount{0}", Convert.ToString(i)), "Leader", null, "1/01/2000");
            base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));

            i = 200;
            base.SQL.Groups_Group_AddLeaderOrMember(15, "Performance Group 2", string.Format("Perf{0} TestAccount{0}", Convert.ToString(i)), "Leader", null, "1/01/2000");
            base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));

            i = 300;
            base.SQL.Groups_Group_AddLeaderOrMember(15, "Performance Group 3", string.Format("Perf{0} TestAccount{0}", Convert.ToString(i)), "Leader", null, "1/01/2000");
            base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));

            i = 400;
            base.SQL.Groups_Group_AddLeaderOrMember(15, "Performance Group 4", string.Format("Perf{0} TestAccount{0}", Convert.ToString(i)), "Leader", null, "1/01/2000");
            base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));

            i = 500;
            base.SQL.Groups_Group_AddLeaderOrMember(15, "Performance Group 5", string.Format("Perf{0} TestAccount{0}", Convert.ToString(i)), "Leader", null, "1/01/2000");
            base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));

            i = 600;
            base.SQL.Groups_Group_AddLeaderOrMember(15, "Performance Group 6", string.Format("Perf{0} TestAccount{0}", Convert.ToString(i)), "Leader", null, "1/01/2000");
            base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));

            i = 700;
            base.SQL.Groups_Group_AddLeaderOrMember(15, "Performance Group 7", string.Format("Perf{0} TestAccount{0}", Convert.ToString(i)), "Leader", null, "1/01/2000");
            base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));

            i = 800;
            base.SQL.Groups_Group_AddLeaderOrMember(15, "Performance Group 8", string.Format("Perf{0} TestAccount{0}", Convert.ToString(i)), "Leader", null, "1/01/2000");
            base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));

            i = 900;
            base.SQL.Groups_Group_AddLeaderOrMember(15, "Performance Group 9", string.Format("Perf{0} TestAccount{0}", Convert.ToString(i)), "Leader", null, "1/01/2000");
            base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));

            i = 1000;
            base.SQL.Groups_Group_AddLeaderOrMember(15, "Performance Group 10", string.Format("Perf{0} TestAccount{0}", Convert.ToString(i)), "Leader", null, "1/01/2000");
            base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));

            i = 1100;
            base.SQL.Groups_Group_AddLeaderOrMember(15, "Performance Group 11", string.Format("Perf{0} TestAccount{0}", Convert.ToString(i)), "Leader", null, "1/01/2000");
            base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));


            //TestLog.WriteLine("Login To Portal");
            //test.Portal.LoginWebDriver("felixg2", "FG.Admin122", "DC");
            //TestLog.WriteLine("Search for Individual");
            //test.Portal.People_ViewIndividual_WebDriver("Felix Gaytan");
            //TestLog.WriteLine("Logout");
            //test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure, Timeout(40000)]
        [Author("Stuart Platt")]
        [Description("Verifies you can create an account in InFellowship.")]
        public void Accounts_CreateAccount_Create_WebDriver()
        {
            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 1; i < 200; i++)
            {
                
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.OpenWebDriver("DC");

                //Create Account
                test.Infellowship.People_Accounts_Create_Webdriver("15", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), email, password, false, "10/28/1945", "Male", "United States", "717 N Harwood St.", null, "Dallas", "Texas", "75201", "", "877-318-5669", null, null);

                Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
                //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));
                

                //Logout
                test.Infellowship.LogoutWebDriver();

                test.Infellowship.LoginWebDriver(email, password, "DC");

                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
                //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));

                test.Infellowship.LogoutWebDriver();
                
                base.SQL.Groups_Group_AddLeaderOrMember(15, "Performance Group", string.Format("Perf{0} TestAccount{0}", Convert.ToString(i)), "Leader", null, "01/01/2000");
                base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));

            }
            
        }

        [Test, RepeatOnFailure, Timeout(40000)]
        [Author("Stuart Platt")]
        [Description("Verifies you can create an account in InFellowship.")]
        public void Accounts_CreateAccount_Create_WebDriver_200()
        {
            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 201; i < 300; i++)
            {
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.OpenWebDriver("DC");

                //Create Account
                test.Infellowship.People_Accounts_Create_Webdriver("15", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), email, password, false, "10/28/1945", "Male", "United States", "717 N Harwood St.", null, "Dallas", "Texas", "75201", "", "877-318-5669", null, null);

                Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
                //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));


                //Logout
                test.Infellowship.LogoutWebDriver();

                test.Infellowship.LoginWebDriver(email, password, "DC");

                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
                //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));

                test.Infellowship.LogoutWebDriver();

                base.SQL.Groups_Group_AddLeaderOrMember(15, "Performance Group 2", string.Format("Perf{0} TestAccount{0}", Convert.ToString(i)), "Leader", null, "01/01/2000");
                base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));

            }

        }


        [Test, RepeatOnFailure, Timeout(40000)]
        [Author("Stuart Platt")]
        [Description("Verifies you can create an account in InFellowship.")]
        public void Accounts_CreateAccount_Create_WebDriver_300()
        {
            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 301; i < 400; i++)
            {
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.OpenWebDriver("DC");

                //Create Account
                test.Infellowship.People_Accounts_Create_Webdriver("15", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), email, password, false, "10/28/1945", "Male", "United States", "717 N Harwood St.", null, "Dallas", "Texas", "75201", "", "877-318-5669", null, null);

                Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
                //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));


                //Logout
                test.Infellowship.LogoutWebDriver();

                test.Infellowship.LoginWebDriver(email, password, "DC");

                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
                //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));

                test.Infellowship.LogoutWebDriver();

                base.SQL.Groups_Group_AddLeaderOrMember(15, "Performance Group 3", string.Format("Perf{0} TestAccount{0}", Convert.ToString(i)), "Leader", null, "01/01/2000");
                base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));

            }

        }


        [Test, RepeatOnFailure, Timeout(40000)]
        [Author("Stuart Platt")]
        [Description("Verifies you can create an account in InFellowship.")]
        public void Accounts_CreateAccount_Create_WebDriver_400()
        {
            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 401; i < 500; i++)
            {
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.OpenWebDriver("DC");

                //Create Account
                test.Infellowship.People_Accounts_Create_Webdriver("15", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), email, password, false, "10/28/1945", "Male", "United States", "717 N Harwood St.", null, "Dallas", "Texas", "75201", "", "877-318-5669", null, null);

                Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
                //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));


                //Logout
                test.Infellowship.LogoutWebDriver();

                test.Infellowship.LoginWebDriver(email, password, "DC");

                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
                //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));

                test.Infellowship.LogoutWebDriver();

                base.SQL.Groups_Group_AddLeaderOrMember(15, "Performance Group 4", string.Format("Perf{0} TestAccount{0}", Convert.ToString(i)), "Leader", null, "01/01/2000");
                base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));

            }

        }

        [Test, RepeatOnFailure, Timeout(40000)]
        [Author("Stuart Platt")]
        [Description("Verifies you can create an account in InFellowship.")]
        public void Accounts_CreateAccount_Create_WebDriver_500()
        {
            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 501; i < 600; i++)
            {
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.OpenWebDriver("DC");

                //Create Account
                test.Infellowship.People_Accounts_Create_Webdriver("15", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), email, password, false, "10/28/1945", "Male", "United States", "717 N Harwood St.", null, "Dallas", "Texas", "75201", "", "877-318-5669", null, null);

                Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
                //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));


                //Logout
                test.Infellowship.LogoutWebDriver();

                test.Infellowship.LoginWebDriver(email, password, "DC");

                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
                //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));

                test.Infellowship.LogoutWebDriver();

                base.SQL.Groups_Group_AddLeaderOrMember(15, "Performance Group 5", string.Format("Perf{0} TestAccount{0}", Convert.ToString(i)), "Leader", null, "01/01/2000");
                base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));

            }

        }

        [Test, RepeatOnFailure, Timeout(40000)]
        [Author("Stuart Platt")]
        [Description("Verifies you can create an account in InFellowship.")]
        public void Accounts_CreateAccount_Create_WebDriver_600()
        {
            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 601; i < 700; i++)
            {
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.OpenWebDriver("DC");

                //Create Account
                test.Infellowship.People_Accounts_Create_Webdriver("15", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), email, password, false, "10/28/1945", "Male", "United States", "717 N Harwood St.", null, "Dallas", "Texas", "75201", "", "877-318-5669", null, null);

                Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
                //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));


                //Logout
                test.Infellowship.LogoutWebDriver();

                test.Infellowship.LoginWebDriver(email, password, "DC");

                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
                //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));

                test.Infellowship.LogoutWebDriver();

                base.SQL.Groups_Group_AddLeaderOrMember(15, "Performance Group 6", string.Format("Perf{0} TestAccount{0}", Convert.ToString(i)), "Leader", null, "01/01/2000");
                base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));

            }

        }

        [Test, RepeatOnFailure, Timeout(40000)]
        [Author("Stuart Platt")]
        [Description("Verifies you can create an account in InFellowship.")]
        public void Accounts_CreateAccount_Create_WebDriver_700()
        {
            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 701; i < 800; i++)
            {
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.OpenWebDriver("DC");

                //Create Account
                test.Infellowship.People_Accounts_Create_Webdriver("15", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), email, password, false, "10/28/1945", "Male", "United States", "717 N Harwood St.", null, "Dallas", "Texas", "75201", "", "877-318-5669", null, null);

                Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
                //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));


                //Logout
                test.Infellowship.LogoutWebDriver();

                test.Infellowship.LoginWebDriver(email, password, "DC");

                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
                //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));

                test.Infellowship.LogoutWebDriver();

                base.SQL.Groups_Group_AddLeaderOrMember(15, "Performance Group 7", string.Format("Perf{0} TestAccount{0}", Convert.ToString(i)), "Leader", null, "01/01/2000");
                base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));

            }

        }

        [Test, RepeatOnFailure, Timeout(40000)]
        [Author("Stuart Platt")]
        [Description("Verifies you can create an account in InFellowship.")]
        public void Accounts_CreateAccount_Create_WebDriver_800()
        {
            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 801; i < 900; i++)
            {
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.OpenWebDriver("DC");

                //Create Account
                test.Infellowship.People_Accounts_Create_Webdriver("15", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), email, password, false, "10/28/1945", "Male", "United States", "717 N Harwood St.", null, "Dallas", "Texas", "75201", "", "877-318-5669", null, null);

                Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
                //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));


                //Logout
                test.Infellowship.LogoutWebDriver();

                test.Infellowship.LoginWebDriver(email, password, "DC");

                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
                //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));

                test.Infellowship.LogoutWebDriver();

                base.SQL.Groups_Group_AddLeaderOrMember(15, "Performance Group 8", string.Format("Perf{0} TestAccount{0}", Convert.ToString(i)), "Leader", null, "01/01/2000");
                base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));

            }

        }

        [Test, RepeatOnFailure, Timeout(40000)]
        [Author("Stuart Platt")]
        [Description("Verifies you can create an account in InFellowship.")]
        public void Accounts_CreateAccount_Create_WebDriver_900()
        {
            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 901; i < 1000; i++)
            {
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.OpenWebDriver("DC");

                //Create Account
                test.Infellowship.People_Accounts_Create_Webdriver("15", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), email, password, false, "10/28/1945", "Male", "United States", "717 N Harwood St.", null, "Dallas", "Texas", "75201", "", "877-318-5669", null, null);

                Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
                //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));


                //Logout
                test.Infellowship.LogoutWebDriver();

                test.Infellowship.LoginWebDriver(email, password, "DC");

                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
                //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));

                test.Infellowship.LogoutWebDriver();

                base.SQL.Groups_Group_AddLeaderOrMember(15, "Performance Group 9", string.Format("Perf{0} TestAccount{0}", Convert.ToString(i)), "Leader", null, "01/01/2000");
                base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));

            }

        }

        [Test, RepeatOnFailure, Timeout(40000)]
        [Author("Stuart Platt")]
        [Description("Verifies you can create an account in InFellowship.")]
        public void Accounts_CreateAccount_Create_WebDriver_1000()
        {
            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 1001; i < 1100; i++)
            {
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.OpenWebDriver("DC");

                //Create Account
                test.Infellowship.People_Accounts_Create_Webdriver("15", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), email, password, false, "10/28/1945", "Male", "United States", "717 N Harwood St.", null, "Dallas", "Texas", "75201", "", "877-318-5669", null, null);

                Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
                //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));


                //Logout
                test.Infellowship.LogoutWebDriver();

                test.Infellowship.LoginWebDriver(email, password, "DC");

                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
                //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));

                test.Infellowship.LogoutWebDriver();

                base.SQL.Groups_Group_AddLeaderOrMember(15, "Performance Group 10", string.Format("Perf{0} TestAccount{0}", Convert.ToString(i)), "Leader", null, "01/01/2000");
                base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));

            }

        }

        [Test, RepeatOnFailure, Timeout(40000)]
        [Author("Stuart Platt")]
        [Description("Verifies you can create an account in InFellowship.")]
        public void Accounts_CreateAccount_Create_WebDriver_1100()
        {
            // Open InFellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 1101; i < 1200; i++)
            {
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.OpenWebDriver("DC");

                //Create Account
                test.Infellowship.People_Accounts_Create_Webdriver("15", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), email, password, false, "10/28/1945", "Male", "United States", "717 N Harwood St.", null, "Dallas", "Texas", "75201", "", "877-318-5669", null, null);

                Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
                //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));


                //Logout
                test.Infellowship.LogoutWebDriver();

                test.Infellowship.LoginWebDriver(email, password, "DC");

                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Update Profile")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Privacy Settings")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Groups")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Find A Group")));
                //Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Your Giving")));
                //Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Church Directory")));

                test.Infellowship.LogoutWebDriver();

                base.SQL.Groups_Group_AddLeaderOrMember(15, "Performance Group 11", string.Format("Perf{0} TestAccount{0}", Convert.ToString(i)), "Leader", null, "01/01/2000");
                base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));

            }

        }
       
        [Test, RepeatOnFailure, Timeout(1200000)]
        [Author("David Martin")]
        [Description("Verifies successful MasterCard credit card transaction")]
        public void GiveNow_CreditCard_Success_MasterCard_WebDriver()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 75; i < 100; i++)
            {
                string amount = test.GeneralMethods.GetRandAmount();
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.LoginWebDriver(email, password, "DC");

                // Process Master Card credit card for Give Now
                test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, amount, string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), "Master Card", "5555555555554444", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

                // Logout of infellowship
                test.Infellowship.LogoutWebDriver();
            }
        }

        [Test, RepeatOnFailure, Timeout(1200000)]
        [Author("David Martin")]
        [Description("Verifies successful AMEX credit card transaction")]
        public void GiveNow_CreditCard_Success_AmericanExpress_WebDriver_100()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 181; i < 200; i++)
            {
                string amount = test.GeneralMethods.GetRandAmount();
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.LoginWebDriver(email, password, "DC");

                // Process Master Card credit card for Give Now
                test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, amount, string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), "American Express", "378282246310005", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

                // Logout of infellowship
                test.Infellowship.LogoutWebDriver();
            }
        }

        [Test, RepeatOnFailure, Timeout(1200000)]
        [Author("David Martin")]
        [Description("Verifies successful Discover Card credit card transaction")]
        public void GiveNow_CreditCard_Success_DiscoverCard_WebDriver_200()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 275; i < 300; i++)
            {
                string amount = test.GeneralMethods.GetRandAmount();
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.LoginWebDriver(email, password, "DC");

                // Process Master Card credit card for Give Now
                test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, amount, string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), "Discover", "6011111111111117", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

                // Logout of infellowship
                test.Infellowship.LogoutWebDriver();
            }
        }

        [Test, RepeatOnFailure, Timeout(1200000)]
        [Author("David Martin")]
        [Description("Verifies successful MasterCard credit card transaction")]
        public void GiveNow_CreditCard_Success_MasterCard_WebDriver_300()
        {
           // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 380; i < 400; i++)
            {
                string amount = test.GeneralMethods.GetRandAmount();
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.LoginWebDriver(email, password, "DC");

                // Process Master Card credit card for Give Now
                test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, amount, string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), "Master Card", "5555555555554444", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

                // Logout of infellowship
                test.Infellowship.LogoutWebDriver();
            }
        }

        [Test, RepeatOnFailure, Timeout(1200000)]
        [Author("David Martin")]
        [Description("Verifies successful Visa credit card transaction")]
        public void GiveNow_CreditCard_Success_Visa_WebDriver_400()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 475; i < 500; i++)
            {
                string amount = test.GeneralMethods.GetRandAmount();
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.LoginWebDriver(email, password, "DC");

                // Process Master Card credit card for Give Now
                test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, amount, string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

                // Logout of infellowship
                test.Infellowship.LogoutWebDriver();
            }
        }

        [Test, RepeatOnFailure, Timeout(1200000)]
        [Author("David Martin")]
        [Description("Verifies successful MasterCard credit card transaction")]
        public void GiveNow_CreditCard_Success_MasterCard_WebDriver_500()
        {
            
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 586; i < 600; i++)
            {
                string amount = test.GeneralMethods.GetRandAmount();
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.LoginWebDriver(email, password, "DC");

                // Process Master Card credit card for Give Now
                test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, amount, string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), "Master Card", "5555555555554444", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

                // Logout of infellowship
                test.Infellowship.LogoutWebDriver();
            }
        }

        [Test, RepeatOnFailure, Timeout(1200000)]
        [Author("David Martin")]
        [Description("Verifies successful MasterCard credit card transaction")]
        public void GiveNow_CreditCard_Success_MasterCard_WebDriver_600()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 696; i < 700; i++)
            {
                string amount = test.GeneralMethods.GetRandAmount();
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.LoginWebDriver(email, password, "DC");

                // Process Master Card credit card for Give Now
                test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, amount, string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), "Master Card", "5555555555554444", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

                // Logout of infellowship
                test.Infellowship.LogoutWebDriver();
            }
        }

        [Test, RepeatOnFailure, Timeout(1200000)]
        [Author("David Martin")]
        [Description("Verifies successful AMEX credit card transaction")]
        public void GiveNow_CreditCard_Success_AmericanExpress_WebDriver_700()
        {
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 775; i < 800; i++)
            {
                string amount = test.GeneralMethods.GetRandAmount();
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.LoginWebDriver(email, password, "DC");

                // Process Master Card credit card for Give Now
                test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, amount, string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), "American Express", "378282246310005", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

                // Logout of infellowship
                test.Infellowship.LogoutWebDriver();
            }
        }

        [Test, RepeatOnFailure, Timeout(1200000)]
        [Author("David Martin")]
        [Description("Verifies successful Discover Card credit card transaction")]
        public void GiveNow_CreditCard_Success_DiscoverCard_WebDriver_800()
        {
            
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 875; i < 900; i++)
            {
                string amount = test.GeneralMethods.GetRandAmount();
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.LoginWebDriver(email, password, "DC");

                // Process Master Card credit card for Give Now
                test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, amount, string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), "Discover", "6011111111111117", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

                // Logout of infellowship
                test.Infellowship.LogoutWebDriver();
            }
        }

        [Test, RepeatOnFailure, Timeout(1200000)]
        [Author("David Martin")]
        [Description("Verifies successful MasterCard credit card transaction")]
        public void GiveNow_CreditCard_Success_MasterCard_WebDriver_900()
        {

            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 989; i < 1000; i++)
            {
                string amount = test.GeneralMethods.GetRandAmount();
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.LoginWebDriver(email, password, "DC");

                // Process Master Card credit card for Give Now
                test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, amount, string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), "Master Card", "5555555555554444", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

                // Logout of infellowship
                test.Infellowship.LogoutWebDriver();
            }
        }

        [Test, RepeatOnFailure, Timeout(1200000)]
        [Author("David Martin")]
        [Description("Verifies successful Visa credit card transaction")]
        public void GiveNow_CreditCard_Success_Visa_WebDriver_1000()
        {
            
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 1081; i < 1100; i++)
            {
                string amount = test.GeneralMethods.GetRandAmount();
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.LoginWebDriver(email, password, "DC");

                // Process Master Card credit card for Give Now
                test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, amount, string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), "Visa", "4111111111111111", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

                // Logout of infellowship
                test.Infellowship.LogoutWebDriver();
            }
        }

        [Test, RepeatOnFailure, Timeout(1200000)]
        [Author("David Martin")]
        [Description("Verifies successful MasterCard credit card transaction")]
        public void GiveNow_CreditCard_Success_MasterCard_WebDriver_1100()
        {
            
            // Login to infellowship
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            for (int i = 1181; i < 1200; i++)
            {
                string amount = test.GeneralMethods.GetRandAmount();
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                string password = "Perf.Test14";

                TestLog.WriteLine(string.Format("Email: {0}", email));
                test.Infellowship.LoginWebDriver(email, password, "DC");

                // Process Master Card credit card for Give Now
                test.Infellowship.Giving_GiveNow_CreditCard_WebDriver(15, "Auto Test Fund", null, amount, string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)), "Master Card", "5555555555554444", "12 - December", (DateTime.Now.Year + 1).ToString(), string.Empty, true);

                // Logout of infellowship
                test.Infellowship.LogoutWebDriver();
            }
        }

        [Test, RepeatOnFailure, Timeout(80000)]
        [Author("Stuart Platt")]
        [Description("Update Individual Status")]
        public void Update_Indivdual_Status_WebDriver()
        {

            for (int i = 100; i < 1200; i++)
            {
                string email = string.Format("perf.test{0}@no-email.com", Convert.ToString(i));
                TestLog.WriteLine(string.Format("Email: {0}", email));
                base.SQL.People_Individual_Update_Status(15, "Attendee", string.Format("Perf{0}", Convert.ToString(i)), string.Format("TestAccount{0}", Convert.ToString(i)));

            }

        }

        
        */
        #endregion PerfTestSetup
        //END Performance Testing Setup

        // [Test]
        [Description("Quick way to import Scanned batches into portal")]
        public void IMPORT_SCANNED_BATCHES_SAVED()
        {
            // create batches
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string batchName1 = string.Format("{0} Test Scanned Batch - 1", today.ToShortDateString());
            string batchName2 = string.Format("{0} Test Scanned Batch - 2", today.ToShortDateString());
            string batchName3 = string.Format("{0} Test Scanned Batch - 3", today.ToShortDateString());
            string batchName4 = string.Format("{0} Test Scanned Batch - 4", today.ToShortDateString());
            string batchName5 = string.Format("{0} Test Scanned Batch - 5", today.ToShortDateString());
            string batchName6 = string.Format("{0} Test Scanned Batch - 6", today.ToShortDateString());
            string batchName7 = string.Format("{0} Test Scanned Batch - 7", today.ToShortDateString());
            string batchName8 = string.Format("{0} Test Scanned Batch - 8", today.ToShortDateString());
            string batchName9 = string.Format("{0} Test Scanned Batch - 9", today.ToShortDateString());
            string batchName10 = string.Format("{0} Test Scanned Batch - 10", today.ToShortDateString());

            base.SQL.Giving_Batches_Create_Scanned(15, batchName1, false, new System.Collections.Generic.List<double>() { 1.00, 1.01, 1.02, 1.03, 1.04 }, null,
                new System.Collections.Generic.List<string> { "111111111", "111111112", "111111113", "111111114", "111111115" },
                new System.Collections.Generic.List<string> { "111000025", "111000025", "111000025", "111000025", "111000025" }, false, "Auto Test Fund", false);

            base.SQL.Giving_Batches_Create_Scanned(15, batchName2, false, new System.Collections.Generic.List<double>() { 2.00, 2.01, 2.02, 2.03, 2.04 }, null,
                new System.Collections.Generic.List<string> { "222222222", "222222223", "222222224", "222222225", "222222226" },
                new System.Collections.Generic.List<string> { "111000025", "111000025", "111000025", "111000025", "111000025" }, false, "Auto Test Fund", false);

            base.SQL.Giving_Batches_Create_Scanned(15, batchName3, false, new System.Collections.Generic.List<double>() { 3.00, 3.01, 3.02, 3.03, 3.04 }, null,
                new System.Collections.Generic.List<string> { "333333333", "333333334", "333333335", "333333336", "333333337" },
                new System.Collections.Generic.List<string> { "111000025", "111000025", "111000025", "111000025", "111000025" }, false, "Auto Test Fund", false);

            base.SQL.Giving_Batches_Create_Scanned(15, batchName4, false, new System.Collections.Generic.List<double>() { 4.00, 4.01, 4.02, 4.03, 4.04 }, null,
                new System.Collections.Generic.List<string> { "444444444", "444444445", "444444446", "444444447", "444444448" },
                new System.Collections.Generic.List<string> { "111000025", "111000025", "111000025", "111000025", "111000025" }, false, "Auto Test Fund", false);

            base.SQL.Giving_Batches_Create_Scanned(15, batchName5, false, new System.Collections.Generic.List<double>() { 5.00, 5.01, 5.02, 5.03, 5.04 }, null,
                new System.Collections.Generic.List<string> { "555555555", "555555556", "555555557", "555555558", "555555559" },
                new System.Collections.Generic.List<string> { "111000025", "111000025", "111000025", "111000025", "111000025" }, false, "Auto Test Fund", false);

            base.SQL.Giving_Batches_Create_Scanned(15, batchName6, true, new System.Collections.Generic.List<double>() { 6.00, 6.01, 6.02, 6.03, 6.04 }, null,
                new System.Collections.Generic.List<string> { "666666666", "666666667", "666666668", "666666669", "666666661" },
                new System.Collections.Generic.List<string> { "111000025", "111000025", "111000025", "111000025", "111000025" }, false, "Auto Test Fund", false);

            base.SQL.Giving_Batches_Create_Scanned(15, batchName7, true, new System.Collections.Generic.List<double>() { 7.00, 7.01, 7.02, 7.03, 7.04 }, null,
                new System.Collections.Generic.List<string> { "777777777", "777777778", "777777779", "777777771", "777777772" },
                new System.Collections.Generic.List<string> { "111000025", "111000025", "111000025", "111000025", "111000025" }, false, "Auto Test Fund", false);

            base.SQL.Giving_Batches_Create_Scanned(15, batchName8, true, new System.Collections.Generic.List<double>() { 8.00, 8.01, 8.02, 8.03, 8.04 }, null,
                new System.Collections.Generic.List<string> { "888888888", "888888889", "888888881", "888888882", "888888883" },
                new System.Collections.Generic.List<string> { "111000025", "111000025", "111000025", "111000025", "111000025" }, false, "Auto Test Fund", false);

            base.SQL.Giving_Batches_Create_Scanned(15, batchName9, true, new System.Collections.Generic.List<double>() { 9.00, 9.01, 9.02, 9.03, 9.04 }, null,
                new System.Collections.Generic.List<string> { "999999998", "999999997", "999999996", "999999995", "999999994" },
                new System.Collections.Generic.List<string> { "111000025", "111000025", "111000025", "111000025", "111000025" }, false, "Auto Test Fund", false);

            base.SQL.Giving_Batches_Create_Scanned(15, batchName10, true, new System.Collections.Generic.List<double>() { 10.00, 10.01, 10.02, 10.03, 10.04 }, null,
                new System.Collections.Generic.List<string> { "101010101", "101010102", "101010103", "101010104", "101010105" },
                new System.Collections.Generic.List<string> { "111000025", "111000025", "111000025", "111000025", "111000025" }, false, "Auto Test Fund", false);

        }


        //[Test]
        [Author("Stuart Platt")]
        [Description("Deletes duplicate groups created by automation.")]
        public void TEST_Delete_Automation_Groups()
        {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");

            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            test.Portal.Groups_Search_NameAndStartDate_WebDriver("Group-", null, null, null);

            test.Driver.FindElementById("commit").Click();
            test.GeneralMethods.WaitForElement(By.XPath(TableIds.Groups_ViewAll_GroupList_SearchResults));

            //Find Secondary Counts
            //group_name=Group-
            //    /html/body/div/div[4]/div/div/div/div[2]/form/div[3]/table/tbody/tr[2]/td/a

            //"//table[@id='ctl00_ctl00_MainContent_content_grdGroups']/a[contains(text(), '{1}')]", itemRow + 1, gearOption)).Click();
            //int groupCount = test.Driver.FindElementsByXPath("//a[contains(text(), 'Group-')]").Count();
            int groupCount = test.GeneralMethods.GetTableRowCountWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults);
            
            TestLog.WriteLine(string.Format("Group Count: {0}", groupCount));


            for (int r = 2; r <= groupCount; r++)
            {
                IWebElement table = test.Driver.FindElementByXPath(TableIds.Groups_ViewAll_GroupList_SearchResults);

                string text = table.FindElements(By.TagName("tr"))[r].FindElements(By.TagName("td"))[1].FindElement(By.TagName("a")).Text;

                if (table.FindElements(By.TagName("tr"))[r].FindElements(By.TagName("td"))[1].FindElement(By.TagName("a")).Text.Contains("Group-"))
                {

                    table.FindElements(By.TagName("tr"))[r].FindElements(By.TagName("td"))[1].FindElement(By.TagName("a")).Click();

                    //Go to group settings
                    test.Driver.FindElementByLinkText("View group settings").Click();
                    test.GeneralMethods.WaitForElement(By.XPath(TableIds.Groups_Group_Edit_Details));

                    test.Driver.FindElementByLinkText("Delete this group").Click();
                    TestLog.WriteLine(string.Format("Confirmation Message: {0}", test.Driver.SwitchTo().Alert().Text));
                    test.Driver.SwitchTo().Alert().Accept();

                    test.Portal.Groups_Search_NameAndStartDate_WebDriver("Group-", null, null, null);
                    test.Driver.FindElementById("commit").Click();
                    test.GeneralMethods.WaitForElement(By.XPath(TableIds.Groups_ViewAll_GroupList_SearchResults));

                    groupCount = test.GeneralMethods.GetTableRowCountWebDriver(TableIds.Groups_ViewAll_GroupList_SearchResults);
                    r = 2;

                }

            }

            // Logout of portal
            test.Portal.LogoutWebDriver();

        }

        //[Test]
        [Author("Suchitra")]
        [Description("Testing Retain Attendance History")]
        public void Portal_Groups_RetainAttendance()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            
            string individualName = "Group Attendance";
            string groupTypeName = "Attendance group";
            string groupName = "Group B";
            int churchId = 254; 
            List<string> individualsInGroup = new List<string>();
            individualsInGroup.Add("Group Leader");
           // base.SQL.Groups_Group_Delete(254, groupName);
         //  base.SQL.Groups_Group_Create(254, _groupTypeName, individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());
            base.SQL.Groups_Group_Create(254, "Attendance group", individualName, groupName, null, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString());

            base.SQL.Groups_Group_AddLeaderOrMember(254, groupName, "Group Leader", "Leader");
            base.SQL.Groups_Group_DeleteAttendanceSummary(churchId, groupName);
            var attendanceDate = base.SQL.Groups_Group_CreateAttendanceRecord(254, groupName, individualsInGroup, false, false);
            
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");
            test.Portal.People_AddToGroup("AttTest", groupTypeName, groupName);
            test.Infellowship.LoginWebDriver("group.leader.ft@gmail.com", "BM.Admin09");
          //  test.Infellowship.Groups_Attendance_Enter_All(groupName, attendanceDate, true);
            test.Infellowship.Groups_Attendance_Enter_All(groupName, "5/3/2014", true);

            test.Infellowship.LogoutWebDriver();
            
            test.Portal.People_FindAPerson_Individual("AttTest", null, null, false, null, null, null, null, false);

            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//*[@id='involvement_graph']/ul/li[2]/div[1]/div[3]/div/ul/li/text()"));
            Assert.AreEqual("Group B", test.Driver.FindElementByXPath("//*[@id='involvement_graph']/ul/li[2]/div[1]/div[3]/div/ul/li/text()").Text);

            base.SQL.Groups_Group_DeleteAttendanceSummary(churchId, groupName);
            base.SQL.Groups_Group_Delete(254, groupName);


        }

        //[Test, RepeatOnFailure]
        [Author("Suchitra")]
        [Description("Report Library Share a Report")]
        public void ReportLibrary_ShareReport()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            // Login to report library
            test.ReportLibrary.Login();

            // Click on "My Reports Tab"
            TestLog.WriteLine("Click on My Reports");
            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//*[@id='my_reports_tab']/a"));
            test.Driver.FindElementByXPath("//*[@id='my_reports_tab']/a").Click();

            //select the first report in My Reports

            TestLog.WriteLine("select First report in the list");
            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//*[@id='ctl00_ctl00_MainContent_LeftColumn_grdReports']/tbody/tr[2]/td[2]/h5/a"));
            test.Driver.FindElementByXPath("//*[@id='ctl00_ctl00_MainContent_LeftColumn_grdReports']/tbody/tr[2]/td[2]/h5/a").Click();

            //Verify Share this Report Text exisits

            TestLog.WriteLine("Verify Send Copy text exists");
            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//*[@id='right_column']/ul/li[3]/a"));
            Assert.AreEqual("Send a Copy", test.Driver.FindElementByXPath("//*[@id='right_column']/ul/li[3]/a").Text);

            //Click on Share this Report

            TestLog.WriteLine("Click on Send a copy");
            test.Driver.FindElementByXPath("//*[@id='right_column']/ul/li[3]/a").Click();

            //Verify Share a Report page elements

            TestLog.WriteLine("Verify the Send a Copy Age elments");
            test.Driver.FindElementByXPath("//*[@id='left_column']/h1");
            Assert.AreEqual(test.Driver.FindElementByXPath("//*[@id='left_column']/h1").Text, "Send a copy");
            test.Driver.FindElementByXPath("//*[@id='left_column']/form/div[1]/label");

            //Verify Name field

            TestLog.WriteLine("Verify Name field");
            Assert.AreEqual(test.Driver.FindElementByXPath("//*[@id='left_column']/form/div[1]/label").Text, "Name: *");

            //Get Report Name
            TestLog.WriteLine("Get Report Name");
            string reportName = test.Driver.FindElementByXPath("//*[@id='UserReportDefinition[ReportName]']").GetAttribute("value");
            test.Driver.FindElementByXPath("//*[@id='left_column']/form/div[2]/label");

            //Verify Description Field
            TestLog.WriteLine("Verify Description field");
            Assert.AreEqual(test.Driver.FindElementByXPath("//*[@id='left_column']/form/div[2]/label").Text, "Description:");
            test.Driver.FindElementByXPath("//*[@id='left_column']/form/div[3]/label");

            //Verify Note field

            TestLog.WriteLine("Verify Note Field");
            Assert.AreEqual(test.Driver.FindElementByXPath("//*[@id='left_column']/form/div[3]/label").Text, "Note:");
            Assert.AreEqual(test.Driver.FindElementByXPath("//*[@id='divForSelectedUsers']/label").Text, "Select the Users you want to share the report with: *");

            //Select "FT Tester" 
            TestLog.WriteLine("Select FT Tester from selected Users");
            new SelectElement(test.Driver.FindElementById("selectedUsers")).SelectByText("FT Tester");


            //Submit Query
            TestLog.WriteLine("Submit Send a Copy of Report");
            test.Driver.FindElementByXPath("//*[@id='submitQuery']").Click();

            //Log out of Report Library
            test.ReportLibrary.Logout();

            //Login to Report library  as Receipient to view report 

            test.ReportLibrary.Login("ft.tester", "FT4life!", "DC");

            // Click on "My Reports Tab"

            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//*[@id='my_reports_tab']/a"));
            test.Driver.FindElementByXPath("//*[@id='my_reports_tab']/a").Click();

            //Verify that Shared Report name exists
            test.Driver.FindElementByLinkText(reportName);

            




        }



    }


    [TestFixture]
    public class Portal_Test : FixtureBase
    {


        //[Test, RepeatOnFailure, MultipleAsserts]
        public void Test_Login()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("felixg", "FG.Admin12", "DC");

            // Login to infellowship

            //test.Selenium.Open(string.Format("https://{0}.staging.infellowship.com", "dc"));
            //test.Portal.People_ViewIndividual("FellowshipOne AutomatedTester01");

            // Logout of portal
            test.Portal.Logout();

        }

        //[Test, RepeatOnFailure] 
        public void Test_Giving_Schedule_Expiration_Date_Current_Month()
        {
            DateTime dateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));


            TestLog.WriteLine("Date " + dateTime.ToString());
            TestLog.WriteLine("Date MM " + dateTime.ToString("MM"));
            TestLog.WriteLine("Date MMMM " + dateTime.ToString("MMMM"));
            TestLog.WriteLine("Date int " + dateTime.Month.ToString());
            TestLog.WriteLine("Date " + dateTime.ToString("M" + " - " + "MMMM"));


        }

        //[Test]
        public void Test_Maps()
        {

            // Login to infellowship
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Selenium.Open(string.Format("https://{0}.staging.infellowship.com", "dc"));

            // Find a group while logged out
            //TestLog.WriteLine("Find a group while logged out ZIP 75022");
            //test.infellowship.Groups_FindAGroup_Search("75022", null, null, false);

            //test.infellowship.Groups_FindAGroup_View_GroupsAtSameLocation("", "Bedford, TX 76021");

            //test.SQL.ExecuteDBProc("ChmContribution.dbo.Weblink_FetchGiving"); //Household
            //test.SQL.ExecuteDBProc("ChmContribution.dbo.Weblink_FetchGiving", "29972315"); //Individual
            //test.SQL.ExecuteDBProc("ChmContribution.dbo.Weblink_FetchGiving", "0, 29972315"); //Everyone
            //test.SQL.ExecuteDBProc("ChmContribution.dbo.Weblink_FetchGiving", "");

        }

        //[Test, RepeatOnFailure]
        public void Test_Email()
        {

            string _userName = "autotester01";
            string _userPassword = "FT4life!";
            string _userChurch = "DC";

            string _emailAddr = "F1AutomatedTester01@gmail.com";
            string _fromConfirmAddr = "info@fellowshiponemail.com";
            //"\"FellowshipOne AutomatedTester01\" <F1AutomatedTester01@gmail.com>"
            string _fromAddr = "\"FellowshipOne AutomatedTester01\" <" + _emailAddr + ">";
            string _replyTo = _emailAddr;
            string _confirmAddr = _replyTo;
            string _password = "ActiveQA12";
            string[] _recepients = new string[] { "FellowshipOne AutomatedTester01" };

            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            //Login to portal
            //test.Portal.Login(_userName, _userPassword, _userChurch);


            test.Portal.Login();

            // Send an email to the designated recipient(s)  
            //string emailTimeStamp = test.Portal.People_GroupEmail_Compose("Test email from Portal - Felix", null, _replyTo, _recepients, "This is a test email sent from Portal - Felix", false, 
            //    _confirmAddr, true);

            // Logout to portal
            test.Portal.Logout();

            test.Selenium.Close();

            //string emailTimeStamp = "4:10 PM";


            //Confirmation E-mail
            /*DateTime begTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            DateTime startTime = begTime;
            MessageCollection confirmMsg = test.Portal.Retrieve_Email("Inbox", "ALL", _emailAddr, _password);
            DateTime endTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            TestLog.WriteLine("Took Retrieve Email: " + (endTime - begTime));

            begTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string confirmEmailBody = test.Portal.getConfirmEmailBody("FellowshipOne", "AutomatedTester01", "FellowshipOne", "AutomatedTester01", _fromConfirmAddr, emailTimeStamp, "Test email from Portal - Felix", true);
            string emailSubject = test.Portal.getEmailSubject("Test email from Portal - Felix");
            
            //Message email = test.Portal.Verify_Confirmation_Email(confirmMsg, emailTimeStamp, "Your email has been sent", _fromConfirmAddr,
            //    _fromConfirmAddr, _recepients,
            //    confirmEmailBody, emailSubject);
            endTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            TestLog.WriteLine("Took Verify Email: " + (endTime - begTime));

            begTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            //test.Portal.Delete_Email(email, _emailAddr, _password, "Inbox");
            TestLog.WriteLine("Took Delete Email: " + (endTime - begTime));

            TestLog.WriteLine("Took Retrieve, Verify, and Delete Email: " + (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")) - startTime));
            //test.Portal.Test_Print_Message_ID(emailFound, _emailAddr, _password, "Inbox");

            //Group Email
            MessageCollection grpMsg = test.Portal.Retrieve_Email("[Gmail]/Spam", "UNSEEN", _emailAddr, _password);
            //"MTA TEST LICENSE - QA Test email from Portal"
           // test.Portal.Verify_Sent_Email(grpMsg, emailTimeStamp, emailSubject, _fromAddr,
           //     _replyTo, _recepients, "This is a test email sent from Portal - Felix");

            //test.Portal.getConfirmEmail("Felix", "Gaytan", "FellowshipOne", "AutomatedTester01", _fromAddr, emailTimeStamp, "Test email from Portal", false);
            
            //Verify the email was not deleivered
            //test.Portal.Verify_Retrieve_NoEmail(grpMsg, emailTimeStamp, "Test email from Portal - Test w/o Recipient", false, "Test email from Portal - Test w/o Recipient");
            */
        }

    }
}