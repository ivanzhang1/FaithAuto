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
namespace FTTests.infellowship.Registrations
{
    [TestFixture]
    class infellowship_EventRegistration_WebDriver : FixtureBaseWebDriver
    {
       // declaring fromId collection
        IList<int>  _formId= new List<int>();
             
        

        public void FixtureSetUp()
        {
           
          }

        [FixtureTearDown]
        public void FixtureTearDown(){

            TestLog.WriteLine("Running Fixture Tear Down......");

            for (int i = 0; i < _formId.Count; i++)
            {
                TestLog.WriteLine("-formId = {0}", _formId[i]);                               
                base.SQL.Weblink_Form_Delete_Proc(15, _formId[i]);
            }

                       
        }

        #region EventRegistration

          
        
        /// <summary>
        /// Infellowship Event Registration test cases
        /// </summary>
        #region EventRegistration Form Name Verification

       
        [Test, RepeatOnFailure, Timeout(6000)]
        [Author("Suchitra Patnam")]
        [Description("F1-4206 - Verifies the Form Name in  Infellowship Registration Form")]
        public void InfellowshipRegistration_VerifyFormName()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            // Set initial conditions
            string formName = "Our Test Form Create";

            //Login
            test.Portal.LoginWebDriver();

            //Delete Form using Store proc method
            test.SQL.Weblink_Form_Delete_Proc(15, formName);
            
            //Create new form            
            test.Portal.WebLink_FormNames_Create(formName, true, true);

            try
            {
                //Get Infellowship Registration link and Verify
                string linkText = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, 15);
                // Assert.AreEqual(linkText, test.Driver.FindElementById("ctl00_ctl00_MainContent_content_infellowshipUrl").GetAttribute("value"), "Infellowship registration link not present");

                //Logout
                test.Portal.LogoutWebDriver();

                //Login to Infellowship Weblink Form
                test.Infellowship.Login_Event_Registration_Form(formName);

                //Verifying the Form Name
                test.GeneralMethods.WaitForElementDisplayed(By.Id(GeneralInFellowship.EventRegistration.FormName));
                Assert.AreEqual(formName, test.Driver.FindElementById(GeneralInFellowship.EventRegistration.FormName).Text);

                //Logout from Infellowship
                //test.Infellowship.LogoutWebDriver();

                //Closing the browser for now since we don't have signout link for this new form Url
                test.Driver.Close();
            }
            finally
            {
                //Delete Form using Stored proc method
                test.SQL.Weblink_Form_Delete_Proc(15, formName);
            }
                         
        }
    

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4206 - Update Form Name in  Infellowship Registration Form")]
        public void InfellowshipRegistration_UpdateFormName()
        {
            
            // Set initial conditions
            string formName = "Best Form - Update";
            string formNameUpdated = "Best Form Updated";

            //Delete and Create Form using Store proc method
            base.SQL.WebLink_FormNames_Create(1, 15, formName, true);

            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            try
            {
                //Login
                test.Portal.LoginWebDriver();

                //Get Form Link to verify
                string linkText = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, 15);

                //Update Form Name
                test.Portal.WebLink_FormNames_Update(formName, true, formNameUpdated, true);

                // finding form name to click by looping through the table
                // test.Portal.EventRegistration_Select_FormName_FromTable(formNameUpdated);

                //Logout Portal
                test.Portal.LogoutWebDriver();


                //Get Form Link to verify
                string linkTextUpdated = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formNameUpdated, 15);

                //Verify we did not change links
                Assert.AreEqual(linkText, linkTextUpdated, "Form Links do not match after update");

                //Login to Event Registration Form
                test.Infellowship.Login_Event_Registration_Form(formNameUpdated);

                //Verifying the Form Name
                test.GeneralMethods.WaitForElementDisplayed(By.Id(GeneralInFellowship.EventRegistration.FormName));
                Assert.AreEqual(formNameUpdated, test.Driver.FindElementById(GeneralInFellowship.EventRegistration.FormName).Text);

            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
            finally
            {
                //Delete and Create Form using Store proc method
                test.SQL.Weblink_Form_Delete_Proc(15, formName);
                test.SQL.Weblink_Form_Delete_Proc(15, formNameUpdated);

                //Closing the browser for now since we don't have signout link for this new form Url
                test.Driver.Close();

            }



        }

        #endregion Form Name Verification

        #region Form Questions

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4206 - Verifies the Header/Question and Answer as required drop down value Infellowship Registration Form")]
        public void InfellowshipRegistration_Verify_QuestionAndAnswer_Dropdown()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions
            string formName = "Test Form - QADropdown";
            string guidRand = Guid.NewGuid().ToString().Substring(0, 5);
            string header = string.Format("Header-{0}", guidRand);
            string question = "Please Enter City Name you live";
            string [] answer = {"city1", "city2", "city3"};
            string[] individual = { "Ft Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login
            test.Portal.LoginWebDriver();

            //Get form ID from database
            int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);


            //Delete Form using Store proc method
            test.SQL.Weblink_Form_Delete_Proc(15, formId);                

            //Create Form
            test.Portal.WebLink_FormNames_Create(formName, true, true);

            //Create Header, question and Answer as drop down selection 

            test.Portal.EventRegistration_Create_QuestionAndAnswer_Dropdown(header, question, answer);

            //Navigate Back to Form page
            test.Driver.FindElementById("tab_back").Click();
            test.Driver.FindElementById("tab_back").Click();

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName);

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Validating  Header Name in infellowship
            test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
            test.GeneralMethods.VerifyTextPresentWebDriver(header);

            //Validating Question in infellowship
            test.GeneralMethods.VerifyTextPresentWebDriver(question);

            //Validating Question required
            //TestLog.WriteLine(" question Text {0}", test.Driver.FindElementByXPath("//div[label[text[]='Please Enter City Name you live']]/div[@class='quetion']"));

            //Get drop down values            
            IList<IWebElement> ddOptions = new SelectElement(test.Driver.FindElementByClassName("form-control")).Options;

            Boolean elementFound = true;
            //validating answer choices
            foreach (IWebElement Option in ddOptions)
            {
                for (int i = 0; i < answer.Length; i++)
                {
                    if (Option.Text.Equals(answer[i]))

                        elementFound = true;

                    Assert.IsTrue(elementFound, "Element not found in drop down list");
                }

            }


            //Logout from Infellowship
            //test.Infellowship.LogoutWebDriver();

            //Clsoing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();
           

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("FO-2900 - Quetion&Answer items will be always display in the step3 page evenif user didn't select it.")]
        public void InfellowshipRegistration_Verify_QuestionAndAnswer_DisplayOnStep3()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            // Set initial conditions
            string formName = "Test Form - QAStep3";
            string guidRand = Guid.NewGuid().ToString().Substring(0, 5); 
            string header = string.Format("Header-{0}", guidRand);
            string question = "Please choose your pet";
            string[] answer = { "Cat", "Dog", "Lizard" };
            string[] answerPrice = { "5.11", "6.11", "8.11" };
            string[] individual = { "Ft Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);
            }

            //Get form ID from database
            int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

            //Delete Form using Store proc method
            //modify by Grace Zhang:delete operation in finally
            //test.SQL.Weblink_Form_Delete_Proc(15, formId);
           
            //Login
            test.Portal.LoginWebDriver();

            //Create Form
            test.Portal.WebLink_FormNames_Create(formName, true, true);

            try
            {
                //Create Header, question and Answer as drop down selection 
                test.Portal.EventRegistration_Create_QuestionAndAnswer_Dropdown_Upsell(header, question, answer, answerPrice, false);

                //Login to Event Registration Form
                test.Infellowship.Login_Event_Registration_Form(formName);

                //select individual
                test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

                //Validating  Header Name in infellowship
                test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
                test.GeneralMethods.VerifyTextPresentWebDriver(header);

                //Validating Question in infellowship
                test.GeneralMethods.VerifyTextPresentWebDriver(question);

                SelectElement questionDrop = new SelectElement(test.Driver.FindElementByClassName("form-control"));
                //Get drop down values            
                IList<IWebElement> ddOptions = questionDrop.Options;

                Boolean elementFound = true;
                //validating answer choices
                foreach (IWebElement Option in ddOptions)
                {
                    for (int i = 0; i < answer.Length; i++)
                    {
                        if (Option.Text.Equals(String.Format("{0} (${1})", answer[i], answerPrice[i])))
                            elementFound = true;

                        Assert.IsTrue(elementFound, "Element not found in drop down list");
                    }

                }

                // Select answer as Dog
                questionDrop.SelectByText(String.Format("{0} (${1})", answer[1], answerPrice[1]));
                // Click Continue to open step 3
                test.Infellowship.EventRegistration_ClickContinue();
                test.GeneralMethods.WaitForElementVisible(By.Id("order_summary"));
                Assert.IsTrue(answer[1].Equals(test.Driver.FindElementByXPath("//table[@id='order_summary']/tbody/tr[3]/td[1]").Text.ToString()), "Selected answer doesn't display");
                Assert.IsTrue(("$" + answerPrice[1]).Equals(test.Driver.FindElementByXPath("//table[@id='order_summary']/tbody/tr[3]/td[2]").Text.ToString()), "Selected answer doesn't display");

                test.Driver.FindElementByXPath(String.Format("//a[@href='/Forms/{0}/Questions']", test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName))).Click();
                test.GeneralMethods.WaitForElementVisible(By.ClassName("form-control"));
                // Choose empty as question answer
                questionDrop = new SelectElement(test.Driver.FindElementByClassName("form-control"));
                questionDrop.SelectByIndex(0);
                // Click continue to open Step 3 again
                test.Infellowship.EventRegistration_ClickContinue();
                test.GeneralMethods.WaitForElementVisible(By.Id("order_summary"));
                try
                {
                    Assert.IsFalse(answer[1].Equals(test.Driver.FindElementByXPath("//table[@id='order_summary']/tbody/tr[3]/td[1]").Text.ToString()), "Wrong selected answer should not display");
                    Assert.IsFalse(("$" + answerPrice[1]).Equals(test.Driver.FindElementByXPath("//table[@id='order_summary']/tbody/tr[3]/td[2]").Text.ToString()), "Wrong selected answer should not display");
                }
                catch (OpenQA.Selenium.NoSuchElementException e)
                {
                    TestLog.WriteLine("Old answer selection doesn't display after choose the empty option, it's right");
                }
            }
            finally
            {
                //Delete Form using Store proc method              
                test.SQL.Weblink_Form_Delete_Proc(15, formId);
               
                //Clsoing the browser for now since we don't have signout link for this new form Url
                test.Driver.Quit();
            }
   
        }


        #endregion Form Questions

        #region Form Schedule Association

        [Test, RepeatOnFailure, Timeout(6000)]
        [Author("Mady Kou")]
        [Description("F1-4204 - To Verify Activity Single schedule in Infellowship Registration Form")]
        public void InfellowshipRegistration_Verify_Single_Schedule()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            // Set initial conditions
            string formName = "OurTestForm_SingleSch";
            string ministry = "A Test Ministry";
            string activity = "A Test Activity";
            string breakoutGroup = "A Test Breakout";
            string activityGroup = "Test Activity RLC Group";
            string[] scheduleActivityName = {"Infellowship Activity Single"};
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string start_Date = Convert.ToString(now.AddDays(2).Day);
            string end_Date = Convert.ToString(now.AddDays(3).Day);
            string[] individual = { "Ft Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Get form ID from database
            int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

            //Delete Form using Store proc method
            test.SQL.Weblink_Form_Delete_Proc(15, formId);
            test.SQL.Ministry_ActivitySchedules_BulkDelete(15, scheduleActivityName);

            //Login
            test.Portal.LoginWebDriver();

            try
            {
                //Testing with new method.....SP
                for (int i = 0; i < scheduleActivityName.Length; i++)
                {
                    test.Portal.Ministry_Activities_Schedules_Add_Daily(ministry, activity, scheduleActivityName[i], TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(2).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(3).ToShortTimeString(), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(1).ToShortDateString(), "1");
                }

                //Create new Form
                test.Portal.WebLink_FormNames_Create(formName, true, true);

                //Create Single Schedule
                test.Portal.EventRegistration_Create_Schedule(ministry, activity, breakoutGroup, activityGroup, scheduleActivityName);

                //save changes
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnDone").Click();

                //Login to Event Registration Form
                test.Infellowship.Login_Event_Registration_Form(formName);

                //select individual
                test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

                //Verify Single schedule
                for (int i = 0; i < scheduleActivityName.Length; i++)
                {
                    //Assert.Contains(test.Driver.FindElement(By.XPath("//div[@class='col-sm-8']/span")).Text, scheduleActivityName[i]);
                    Assert.Contains(test.Driver.FindElement(By.XPath("//div[@class='col-sm-8']")).Text, scheduleActivityName[i]);
                }
            }
            finally
            {
                formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

                //Delete Form using Store proc method
                test.SQL.Weblink_Form_Delete_Proc(15, formId);
                test.SQL.Ministry_ActivitySchedules_BulkDelete(15, scheduleActivityName);

                //clsoing browser
                test.Driver.Quit();
            }  

        }

        // Case invalid after Activity(Leagcy) is removed
        // [Test, RepeatOnFailure, Timeout(1500)]
        [Author("Mady Kou")]
        [Description("FO-2899 - The Instance should be required in the registration step2 page.")]
        public void InfellowshipRegistration_Verify_Multi_Schedule()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            // Set initial conditions
            string formName = "Test Form - MultiSchedule";
            string ministry = "A Test Ministry";
            string activity = "A Test Activity";
            string breakoutGroup = "A Test Breakout";
            string activityGroup = "Test Activity RLC Group";
            string[] scheduleActivityName = { "Infellowship form Multi-1", "Infellowship form Multi-2" };
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string start_Date = now.AddDays(2).ToString("M/d/yyyy");
            string end_Date = now.AddDays(3).ToString("M/d/yyyy");

            string[] individual = { "Ft Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login
            test.Portal.LoginWebDriver();
            
            //Get form ID from database
            int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

            //Delete Form using Store proc method
            test.SQL.Weblink_Form_Delete_Proc(15, formId);
            test.SQL.Ministry_ActivitySchedules_BulkDelete(15, scheduleActivityName);

            try
            {
                //Creating Activity Schedules
                test.Portal.Ministry_ActivitySchedules_Create_FutureDate_Webdriver(activity, scheduleActivityName, start_Date, end_Date);

                //Create New Form
                test.Portal.WebLink_FormNames_Create(formName, true, true);

                //Create Single Schedule
                test.Portal.EventRegistration_Create_Schedule(ministry, activity, breakoutGroup, activityGroup, scheduleActivityName);

                //save changes
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnDone").Click();

                //Login to Event Registration Form
                test.Infellowship.Login_Event_Registration_Form(formName);

                //select individual
                test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);


                //Verify Multi schedule
                SelectElement dropdown = new SelectElement(test.GeneralMethods.WaitAndGetElement(By.Id("selectedScheduleOrInstanceId")));
                IList<IWebElement> ddOptions = dropdown.Options;

                Boolean elementFound = true;

                //validating answer choices
                foreach (IWebElement Option in ddOptions)
                {
                    if (Option.Text.Equals(""))
                    {
                        continue;
                    }

                    elementFound = false;
                    for (int i = 0; i < scheduleActivityName.Length; i++)
                    {
                        if (Option.Text.Equals(String.Format("{0} ({1} - {2})", scheduleActivityName[i], start_Date, end_Date)))
                        {
                            elementFound = true;
                            break;
                        }

                    }

                    Assert.IsTrue(elementFound, "Element not found in drop down list");

                }

                dropdown = new SelectElement(test.Driver.FindElementById("selectedScheduleOrInstanceId"));
                dropdown.SelectByText("");
                Thread.Sleep(3000);
                Assert.IsTrue("".Equals(dropdown.SelectedOption.Text), "There is wrong otion selected in dropdown");

                test.Infellowship.EventRegistration_ClickContinue();

                //verify it will go to step 3 page
                IJavaScriptExecutor js_driver = (IJavaScriptExecutor)test.Driver;
                string css_value = js_driver.ExecuteScript("return $(\"strong:contains('Step 3')\").parent().attr('class')").ToString();
                Assert.Contains(css_value, "CurrentStep", "Current step isn't step 3");
                //test.GeneralMethods.WaitForElementVisible(By.XPath(".//div[@id='main-content']/div[1]/div/ul/li"));

            }
            finally
            {
                formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

                //Delete Form using Store proc method
                test.SQL.Weblink_Form_Delete_Proc(15, formId);
                test.SQL.Ministry_ActivitySchedules_BulkDelete(15, scheduleActivityName);

                //Closing browser
                test.Driver.Quit();
            }
            


        }

        [Test, RepeatOnFailure, Timeout(6000)]
        [Author("Mady Kou")]
        [Description("F1-4204 - To Verify Activity Single schedule date Instance selection in Infellowship Registration Form")]
        public void InfellowshipRegistration_Verify_SingleDate_Schedule()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            // Set initial conditions
            string formName = "OurTestForm_SingleDateSch";
            string ministry = "A Test Ministry";
            string activity = "A Test Activity";
            string breakoutGroup = "A Test Breakout";
            string activityGroup = "Test Activity RLC Group";
            string [] scheduleName  = {"Infellowship Form STime"};
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string start_Time = now.AddHours(2).ToString("h:mm tt");
            string end_Time = now.AddHours(3).ToString("h:mm tt");
            string start_Date = Convert.ToString(now.Date.AddDays(1).ToShortDateString());
            string[] activitySchedule = {string.Format("{0} {1}", start_Date, start_Time)};
            string[] individual = { "Ft Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }              

            //Get form ID from database
            int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

            //Delete Form using Store proc method
            test.SQL.Weblink_Form_Delete_Proc(15, formId);
            test.SQL.Ministry_ActivitySchedules_BulkDelete(15, scheduleName);

            //Login
            test.Portal.LoginWebDriver();

            try
            {
                //Add schedule one time selection
                for (int i = 0; i < scheduleName.Length; i++)
                {
                    test.Portal.Ministry_Activities_Schedules_Add_OneTime(ministry, activity, scheduleName[i], start_Time, end_Time, start_Date);
                }

                //Clicking on Home link to get back to Portal home
                test.Driver.FindElementByLinkText("Home").Click();
           
                //Create New Form
                test.Portal.WebLink_FormNames_Create(formName, true, true);

                //Create Single Schedule
                test.Portal.EventRegistration_Create_Date_Schedule(ministry, activity, breakoutGroup, activityGroup, scheduleName, activitySchedule);

                //save changes
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnDone").Click();

                //Login to Event Registration Form
                test.Infellowship.Login_Event_Registration_Form(formName);

                //select individual
                test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

                //Verify Single schedule
                for (int i = 0; i < activitySchedule.Length; i++)
                {
                    Assert.Contains(test.Driver.FindElement(By.XPath("//div[@class='col-sm-8']/span")).Text, activitySchedule[i]);
                }
            }
            finally
            {
                formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

                //Delete Form using Store proc method
                test.SQL.Weblink_Form_Delete_Proc(15, formId);
                test.SQL.Ministry_ActivitySchedules_BulkDelete(15, scheduleName);

                //closing browser
                test.Driver.Quit();
            }


        }


        [Test, RepeatOnFailure, Timeout(6000)]
        [Author("Mady Kou")]
        [Description("FO-2899 - The Instance should be required in the registration step2 page.")]
        public void InfellowshipRegistration_Verify_MultiDate_Schedule()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            // Set initial conditions
            string formName = "OurTestForm_MultiDateSch";
            string ministry = "A Test Ministry";
            string activity = "A Test Activity";
            string breakoutGroup = "A Test Breakout";
            string activityGroup = "Test Activity RLC Group";
            string[] scheduleName = { "Infellowship Form MultiTime", "Infellowship Form MultiTime1" };
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string[] start_Time = { now.AddHours(2).ToString("h:mm tt"), now.AddHours(4).ToString("h:mm tt") };
            string[] end_Time = { now.AddHours(3).ToString("h:mm tt"), now.AddHours(5).ToString("h:mm tt") };

            string start_Date = Convert.ToString(now.Date.AddDays(1).ToShortDateString());
            string[] scheduleActivity = { string.Format("{0} {1}", start_Date, start_Time[0]), string.Format("{0} {1}", start_Date, start_Time[1]) };
            string[] individual = { "Ft Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Get form ID from database
            int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

            //Deelte Form using Store proc method
            test.SQL.Weblink_Form_Delete_Proc(15, formId);
            test.SQL.Ministry_ActivitySchedules_BulkDelete(15, scheduleName);

            //Login
            test.Portal.LoginWebDriver();


            try
            {
                //Add schedule one time selection
                for (int i = 0; i < scheduleName.Length; i++)
                {
                    test.Portal.Ministry_Activities_Schedules_Add_OneTime(ministry, activity, scheduleName[i], start_Time[i], end_Time[i], start_Date);
                }

                //Clicking on Home link to get back to Portal home
                test.Driver.FindElementByLinkText("Home").Click();

                //Create New Form
                test.Portal.WebLink_FormNames_Create(formName, true, true);

                //Create Multi Schedule                                    
                test.Portal.EventRegistration_Create_Date_Schedule(ministry, activity, breakoutGroup, activityGroup, scheduleName, scheduleActivity);

                //save changes
                test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnDone").Click();


                //Get Infellowship Registration link
                string linkText = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, 15);

                test.Infellowship.Login_Event_Registration_Form(formName);

                //select individual
                test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

                //Verify Multi schedule
                test.GeneralMethods.WaitForElementVisible(By.Id("selectedScheduleOrInstanceId"));
                SelectElement dropdown = new SelectElement(test.Driver.FindElementById("selectedScheduleOrInstanceId"));
                IList<IWebElement> ddOptions = dropdown.Options;

                Boolean elementFound = true;
                //validating schedule drop down
                foreach (IWebElement Option in ddOptions)
                {
                    if (Option.Text.Equals(""))
                    {
                        continue;
                    }
                    elementFound = false;
                    for (int i = 0; i < scheduleActivity.Length; i++)
                    {
                        if (Option.Text.Equals(scheduleActivity[i]))
                        {
                            elementFound = true;
                            break;
                        }

                    }

                    Assert.IsTrue(elementFound, "Element not found in drop down list");
                }

                dropdown = new SelectElement(test.Driver.FindElementById("selectedScheduleOrInstanceId"));
                dropdown.SelectByText("");
                Thread.Sleep(3000);
                Assert.IsTrue("".Equals(dropdown.SelectedOption.Text), "There is wrong otion selected in dropdown");
                
                test.Infellowship.EventRegistration_ClickContinue();
                // Assert.IsTrue(test.GeneralMethods.IsElementVisibleWebDriver(By.XPath("//div[@class='error_msgs_for']")), "Fail to find error message if no instance selected");

                //Verify current page is step 3 page
                IJavaScriptExecutor js_driver = (IJavaScriptExecutor)test.Driver;
                string css_value = js_driver.ExecuteScript("return $(\"strong:contains('Step 3')\").parent().attr('class')").ToString();
                Assert.Contains(css_value, "CurrentStep", "Current step isn't step 3");
                //test.GeneralMethods.WaitForElementVisible(By.XPath(".//div[@id='main-content']/div[1]/div/ul/li"));
            }
            finally
            {
                formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

                //Deelte Form using Store proc method
                test.SQL.Weblink_Form_Delete_Proc(15, formId);
                test.SQL.Ministry_ActivitySchedules_BulkDelete(15, scheduleName);

                //Closing Browser
                test.Driver.Quit();
            }
            
        }

        

        #endregion Form Schedule Association

        #region Inactive Form

        [Test, RepeatOnFailure, Timeout(6000)]
        [Author("Ivan Zhang")]
        [Description("FO-2616 Verify registration close is displayed when we register an inactive form with inactive status.")]
        public void InfellowshipRegistration_InActiveStatusForm()
        {
            #region define variable
            int Form_type_ID = 2;
            string Form_Name = "Ivan_InactiveForm";
            int Form_church_ID = 15;
            string expectedKeyWord = "Registration Closed";
            #endregion

            //DateTime tempDate = DateTime.Now;
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            #region status is inactive
            //create a form with inactive status
            test.SQL.Weblink_Form_Delete_Proc(Form_church_ID,Form_Name);
            test.SQL.WebLink_FormNames_Create(1, Form_church_ID, Form_Name, false);
            
            //Login Infellowhship Event Registration
            string linkText = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(Form_Name, Form_church_ID);
            test.GeneralMethods.OpenURLWebDriver(linkText);
            TestLog.WriteLine(string.Format("Registration URL {0} is opened.", linkText));

            string realKeyWord = test.Driver.FindElementById("errorMes").Text;
            string realFormName = test.Driver.FindElementByXPath(".//div[@id='page_header']/div/h3").Text;
            Assert.AreEqual(expectedKeyWord, realKeyWord);
            Assert.AreEqual(Form_Name, realFormName);
            #endregion status is inactive

            //clean the inserted test data
            test.SQL.WebLink_FormNames_Delete(Form_type_ID, Form_church_ID, Form_Name);

        }

        [Test, RepeatOnFailure]
        [Author("Ivan Zhang")]
        [Description("FO-2616 Verify registration close is displayed when we register the number of individuals is bigger than max submission.")]
        public void InfellowshipRegistration_ExceedMaxSubmissionForm()
        {
            #region define variables
            int Form_ID;
            int Form_church_ID=15;
            string Form_Name = "Ivan_MaxSubmissionForm";
            string[] individuals = { "fo-2616 testaccount1", "fo-2616 testaccount2" };//Updated by Jim
            string expectedKeyWord = "Registration Closed";
            IList<string> selectedOrder = new List<string>();
            #endregion end define
            
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //updated by ivan.zhang
            try { 
                #region exceed the max submission.
                //create a form with inactive status
                test.SQL.Weblink_Form_Delete_Proc(Form_church_ID, Form_Name);
                test.SQL.WebLink_FormNames_Create(1, Form_church_ID, Form_Name, true);

                int individuleId = test.SQL.People_Individuals_FetchID_ByEmail(Form_church_ID, "ivan.zhang@active.com");
                int householdId = test.SQL.People_Households_FetchID(Form_church_ID, individuleId);
                //Updated by Jim
                test.SQL.People_Individual_Create_In_Household(Form_church_ID, individuals[0].Split(' ')[0], individuals[0].Split(' ')[1], householdId.ToString(), 1);
                test.SQL.People_Individual_Create_In_Household(Form_church_ID, individuals[1].Split(' ')[0], individuals[1].Split(' ')[1], householdId.ToString(), 1);

                //update max submission to 1
                Form_ID = test.SQL.Weblink_InfellowshipForm_GetFormId(Form_church_ID, Form_Name);
                test.SQL.Update_ExistingForm(null, null, 1, Form_ID);
                //Login Infellowhship Event Registration
                test.Infellowship.Login_Event_Registration_Form(Form_Name, Form_church_ID, "ivan.zhang@active.com", "123123");
                TestLog.WriteLine("Registration URL is opened.");

                //select the number of individual bigger than max submission
                //Select individuals based on the list
                IList<IWebElement> listPeople = test.Driver.FindElement(By.Id("available_people")).FindElements(By.TagName("li"));
                foreach (IWebElement person in listPeople)
                {
                    string individual = Regex.Split(person.Text, "\r\n")[0].ToString().ToLower();
                    if (individual.Equals(individuals[0]) || individual.Equals(individuals[1]))
                    {
                        person.Click();
                    }
                }

                //click the continue button to go to next page check whether the registration closed is displayed.
                test.GeneralMethods.WaitForElementDisplayed(By.XPath(".//*[@id='continue'][@class='btn btn-primary btn-lg next pull-right btn-group-vertical']"), 30, "The continue button is not enable.");
                test.Driver.FindElementByXPath(".//*[@id='continue'][@class='btn btn-primary btn-lg next pull-right btn-group-vertical']").Click();

                //Assert the result
                test.GeneralMethods.WaitForElementVisible(By.Id("errorMes"), 30, "The step2 page is not opened when user clicked continue btn.");
                string realKeyWord = test.Driver.FindElementById("errorMes").Text;
                string realFormName = test.Driver.FindElementByXPath(".//div[@id='page_header']/div/h3").Text;
                Assert.AreEqual(expectedKeyWord, realKeyWord, "No registration close prompt");
                Assert.AreEqual(Form_Name, realFormName, "form name is not consistent with each other.");
                #endregion exceed the max submission
            }
            finally { 
                //clean the insert test data
                test.SQL.People_Individual_DeleteBulk(Form_church_ID, individuals[0].Split(' ')[0], individuals[0].Split(' ')[1]);//Updated by Jim
                test.SQL.People_Individual_DeleteBulk(Form_church_ID, individuals[1].Split(' ')[0], individuals[1].Split(' ')[1]);//Updated by Jim
                test.SQL.Weblink_Form_Delete_Proc(Form_church_ID, Form_Name);
            }
        }


        [Test, RepeatOnFailure,Timeout(6000)]
        [Author("Ivan Zhang")]
        [Description("FO-2616 Verify registration close is displayed when we register a out of date range form.")]
        public void InfellowshipRegistration_OutOfDateRangeForm()
        {
            #region define variables
            int Form_ID;
            int Form_church_ID = 15;
            string Form_Name = "Ivan_OutOfDateRangeForm";
            string expectedKeyWord = "Registration Closed";
            #endregion end define

            DateTime tempDate = DateTime.Now;
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            #region out of date range
            //create a form
            test.SQL.Weblink_Form_Delete_Proc(Form_church_ID, Form_Name);
            test.SQL.WebLink_FormNames_Create(1, Form_church_ID, Form_Name, true);

            //update date, set the end date smaller than current date
            Form_ID = test.SQL.Weblink_InfellowshipForm_GetFormId(Form_church_ID, Form_Name);
            test.SQL.Update_ExistingForm(tempDate.AddMonths(-2).Date.ToString("yyyy-MM-dd hh:mm:ss.fff"), tempDate.AddMonths(-2).Date.ToString("yyyy-MM-dd hh:mm:ss.fff"), 0, Form_ID);

            //Login Infellowhship Event Registration
            string linkText = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(Form_Name, Form_church_ID);
            test.GeneralMethods.OpenURLWebDriver(linkText);
            TestLog.WriteLine(string.Format("Registration URL {0} is opened.", linkText));

            string realKeyWord = test.Driver.FindElementById("errorMes").Text;
            string realFormName = test.Driver.FindElementByXPath(".//div[@id='page_header']/div/h3").Text;
            Assert.AreEqual(expectedKeyWord, realKeyWord, "No registration close prompt");
            Assert.AreEqual(Form_Name, realFormName, "form name is not consistent with each other.");
            #endregion out of date range

            //clean the insert data
            test.SQL.Weblink_Form_Delete_Proc(Form_church_ID, Form_Name);

        }


        #endregion Inactive Form

    } //End of first event registration fixture


    [TestFixture]
    class infellowship_EventRegistration_TextBoxValidations_WebDriver : FixtureBaseWebDriver
    {
        // declaring fromId collection
        IList<int> _formId = new List<int>();

        
        public void FixtureSetUp()
        {

         }

        [FixtureTearDown]
        public void FixtureTearDown()
        {

            TestLog.WriteLine("Running Fixture Tear Down......");

            for (int i = 0; i < _formId.Count; i++)
            {
                TestLog.WriteLine("-formId = {0}", _formId[i]);
                base.SQL.Weblink_Form_Delete_Proc(15, _formId[i]);
            }


        }
        #region Submit Form

        #region TextBox-No validation 
        //commenting this test case until figured out why hashcode is not working...
        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
        public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxNovalidation_Required_NoError()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions

            string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
            string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
            string header = string.Format("Header-{0}", guidRandom);
            string question = "Please Enter City Name you live";
            string answer = "city1";
            string validationType = "no validation";
            string[] individual = { "Ft Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login
            test.Portal.LoginWebDriver();
                       

            //Create New Form
            test.Portal.WebLink_FormNames_Create(formName,true, true);

            

            //Create Header, question and Answer as Textbox 

            test.Portal.EventRegistration_Create_Question_Textbox(header, question);

            //Selecting Answer as required for question
            test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

            //Drop down seelct
            //   this._driver.FindElementById("ctl00_ctl00_MainContent_content_rdbtnDropDown").Click();

            //Selecting validation type for text box from drop down

            test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

            //Get form ID from database
            int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

            //adding form id to global collection to delete
            _formId.Add(formId);

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName);

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);



            //Validating  Header Name in infellowship
            test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
            test.GeneralMethods.VerifyTextPresentWebDriver(header);

            //Validating Question in infellowship
            test.GeneralMethods.VerifyTextPresentWebDriver(question);

           // Entering Text to Text box

            TestLog.WriteLine("Form Id {0}", formId);
            int elementItemId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

          //  TestLog.WriteLine("Id {0}", elementItemId);

            //This code is to get form individual ID and question id and using these get Hashcode value
            string attributeValue = test.Driver.FindElement(By.ClassName("form-control")).GetAttribute("data-forminfo");

           TestLog.WriteLine("attributeValue {0}", attributeValue);
            string[] attributeValues = attributeValue.Split('_');
            string form_Individual_Id = attributeValues[0];

            TestLog.WriteLine("Att-1 {0}", attributeValues[0]);
            TestLog.WriteLine("Att-2 {0}", attributeValues[1]);

            int elementId = HashCodeHelper.CombineHashCodes(Convert.ToInt32(attributeValues[0]),Convert.ToInt32(attributeValues[1]));

            TestLog.WriteLine(" Element Id : {0}", elementId);



            //Entering Text to Text box
            test.Infellowship.EventRegistration_TextBox_Answer(answer);

            //Validate that no error messages present since the answer is already passed to textbox

            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


            //Delete Form using Store proc method
           // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Logout from Infellowship
            //test.Infellowship.LogoutWebDriver();

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();


        }  
       

        [Test, RepeatOnFailure(Order = 1)]
        [Author("Suchitra Patnam")]
        [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
        public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxNoValidation_Required_WithError()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            // Set initial conditions

            string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
            string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
            string header = string.Format("Header-{0}", guidRandom);
            string question = "Please Enter City Name you live";
            string answer = " ";
            string validationType = "no validation";
            string[] individual = { "Ft Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login
            test.Portal.LoginWebDriver();
            test.Portal.WebLink_FormNames_Create(formName, true, true);

                    
            //Create New Form
            //Create Header, question and Answer as Textbox 

            test.Portal.EventRegistration_Create_Question_Textbox(header, question);

            //Selecting Answer as required for question
            test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

            //Selecting validation type for text box from drop down

            test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

            //Get form ID from database
            int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

            //adding form id to global collection to delete
            _formId.Add(formId);

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName);

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Validating  Header Name in infellowship
            test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
            test.GeneralMethods.VerifyTextPresentWebDriver(header);

            //Validating Question in infellowship
            test.GeneralMethods.VerifyTextPresentWebDriver(question);

            //Entering Text to Text box

            TestLog.WriteLine("Form Id {0}", formId);
            int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

            //Entering Text to Text box
            test.Infellowship.EventRegistration_TextBox_Answer(answer);

            //Validate that  error messages present since the answer is not passed to textbox

            Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


            //Delete Form using Store proc method
           // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Logout from Infellowship
            //test.Infellowship.LogoutWebDriver();

            //Closing the browser for now since we don't have signout link for this new form Url
          //  test.Driver.Close();
            test.Driver.Quit();


        }
        
         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxNoValidation_NotRequired_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Please Enter City Name you live";
             string answer = "city1";
             string validationType = "no validation";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }

             //Login
             test.Portal.LoginWebDriver();           

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);
 
             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //No selection of Answer as required

             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);
             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             //Entering Text to Text box

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             TestLog.WriteLine("Id {0}", elementId);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Validate that no error messages present since the answer is already passed to textbox

            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         #endregion TextBox - No Validation

        #region TextBox-Alpha-Validations

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxAlpha_Required_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Please Enter City Name you live";
             string answer = "Dallas";
             string validationType = "Alpha";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }

             //Login
             test.Portal.LoginWebDriver();

             TestLog.WriteLine("Form Name: {0}", formName);

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

            //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

            
             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

            
             TestLog.WriteLine("Formid = {0}", formId);
            
             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);
             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

            

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Validate that no error messages present since the answer is already passed to textbox

            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }


         [Test, RepeatOnFailure(Order = 1)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxAlpha_Required_WithAlphaData()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Please Enter City Name you live";
             string answer = "ABcd";
             string validationType = "Alpha";
             string[] individual = {"Ft Tester"};
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);
                 
             }

             //Login
             test.Portal.LoginWebDriver();
          
             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

           //  test.SQL.WebLink_FormNames_Create(1, 15, formName, true);

             //finding formname from table
            // test.Portal.EventRegistration_Select_FormName_FromTable(formName);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             TestLog.WriteLine("Formid = {0}", formId);
        
             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

            

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             TestLog.WriteLine("Id {0}", elementId);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that  error messages present since the answer is not passed to textbox


             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
            //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }
         [Test, RepeatOnFailure(Order = 1)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxAlpha_Required_WithNonAlphaData()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Please Enter City Name you live";
             //validation rule changed in FO-1888, Alpha validation aloow space and special characters
             string answer = "12 345$$ ABC";
             string validationType = "Alpha";
             string[] individual = {"Ft Tester"};
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }

             //Login
             test.Portal.LoginWebDriver();
                        
             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

            // test.SQL.WebLink_FormNames_Create(1, 15, formName, true);

             //finding formname from table
          //   test.Portal.EventRegistration_Select_FormName_FromTable(formName);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);      

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Validate that  error messages present since the answer invalid format
           
             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Alpha)));

             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }
                       

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxAlpha_NotRequired_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

             // Set initial conditions
             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Please Enter City Name you live";
             string answer = "Dallas";
             string validationType = "Alpha";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }

             //Login
             test.Portal.LoginWebDriver();
            
             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

            //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Not Selecting Answer as required for question
           

             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);


             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);
             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

            
             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             TestLog.WriteLine("Id {0}", elementId);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Validate that no error messages present since the answer is not required

             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         #endregion TextBox-Alpha

        #region TextBox-Alphanumeric-Validations
         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxAlphanumeric_Required_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Please Enter City Name you live";
             string answer = "Dallas123";
             string validationType = "Alphanumeric";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }

             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

            
             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();


             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

            
             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that no error messages present since the answer is already passed to textbox

             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         [Test, RepeatOnFailure(Order = 1)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxAlphanumeric_Required_WithError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Please Enter City Name you live";
             string answer = "";
             string validationType = "Alphanumeric";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);
             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             TestLog.WriteLine("Id {0}", elementId);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that  error messages present since the answer is not passed to textbox


             Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure(Order = 1)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxAlphanumeric_Required_WithNonAlphanumericData()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Please Enter City Name you live";
             string answer = "#######";
             string validationType = "Alphanumeric";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that  error messages present since the answer invalid format
         
             Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Alphanumeric)));

             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxAlphanumeric_NotRequired_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Please Enter City Name you live";
             string answer = "Dallas";
             string validationType = "Alphanumeric";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

            
             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Not Selecting Answer as required for question
            

             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Validate that no error messages present since the answer is already passed to textbox

            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         #endregion

        #region TextBox-CurrencyPlus-Validations
         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxCurrency_Required_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Please Enter Currency value";
             string answer = "12.00";
             string validationType = "Currency (+)";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();


             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that no error messages present since the answer is already passed to textbox

            Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         [Test, RepeatOnFailure(Order = 2)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxCurrency_Required_WithError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Please Enter Currency value";
             string answer = " ";
             string validationType = "Currency (+)";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }


             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

            
             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             TestLog.WriteLine("Id {0}", elementId);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that  error messages present since the answer is not passed to textbox


             Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure(Order = 2)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxCurrency_Required_WithNonCurrencyData()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Please Enter Currency value";
             string answer = "abc";
             string validationType = "Currency (+)";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

                     
             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);


             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that  error messages present since the answer invalid format
            
             Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.CurrencyPlus)));

             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxCurrency_NotRequired_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Please Enter Currency value";
             string answer = "Dallas";
             string validationType = "Currency (+)";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }


             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Not Selecting Answer as required for question
            
             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             TestLog.WriteLine("Id {0}", elementId);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Validate that no error messages present since the answer is already passed to textbox

             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         #endregion CurrencyPlus

        #region TextBox-CurrencyMinus-Validations

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxCurrencyWithMinus_Required_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Please Enter Currency value";
             string answer = "-12.00";
             string validationType = "Currency(-, +)";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

            //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();


             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);


             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

           
             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Validate that no error messages present since the answer is already passed to textbox

             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         [Test, RepeatOnFailure(Order = 2)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxCurrencyWithMinus_Required_WithError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Please Enter Currency value";
             string answer = " ";
             string validationType = "Currency(-, +)";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

            
             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             TestLog.WriteLine("Id {0}", elementId);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that  error messages present since the answer is not passed to textbox


             Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure(Order = 2)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxCurrencyWithMinus_Required_WithNonCurrencyData()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Please Enter Currency value";
             string answer = "abc";
             string validationType = "Currency(-, +)";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Validate that  error messages present since the answer invalid format


           
             Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.CurrencyMinus)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxCurrencyWithMinus_NotRequired_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Please Enter Currency value";
             string answer = "Dallas";
             string validationType = "Currency(-, +)";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

            //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Not Selecting Answer as required for question
            

             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that no error messages present since the answer is already passed to textbox

             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));;


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }
         #endregion TextBox-CurrencyMinus

        #region TextBox-DateTime-Validations

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxDateTime_Required_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter DOB";
             string answer = "12/12/1990";
             string validationType = "Date Time";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

         
             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();


             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);


             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

           
             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Validate that no error messages present since the answer is already passed to textbox

             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         [Test, RepeatOnFailure(Order = 2)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxDateTime_Required_WithError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter DOB";
             string answer = "12.00";
             string validationType = "Date Time";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);


             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             TestLog.WriteLine("Id {0}", elementId);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Validate that  error messages present since the answer is not passed to textbox


            Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure(Order = 3)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxDateTime_Required_WithNonDateData()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter DOB";
             string answer = "abc";
             string validationType = "Date Time";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

            //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

           
             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Validate that  error messages present since the answer invalid format          
             Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.DateTime)));


             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxDateTime_NotRequired_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter DOB";
             string answer = "Dallas";
             string validationType = "Date Time";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }


             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Not Selecting Answer as required for question
            
             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

            
             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that no error messages present since the answer is already passed to textbox

             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         #endregion TextBox-DateTime

        #region TextBox-Email-Validations
         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxEmail_Required_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Email";
             string answer = "abc@abc.com";
             string validationType = "Email";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();


             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Validate that no error messages present since the answer is already passed to textbox

             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         [Test, RepeatOnFailure(Order = 3)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxEmail_Required_WithError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Email";
             string answer = "12.00";
             string validationType = "Email";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }


             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

            
             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             TestLog.WriteLine("Id {0}", elementId);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that  error messages present since the answer is not passed to textbox


             Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));

             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure(Order = 3)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxEmail_Required_WithNonEmailinfo()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Email";
             string answer = "abc";
             string validationType = "Email";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);


             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

          
             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that  error messages present since the answer invalid format
            
             Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Email)));


             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxEmail_NotRequired_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Email";
             string answer = "Dallas";
             string validationType = "Email";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

            //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Not Selecting Answer as required for question
            
             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);


             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

            
             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Validate that no error messages present since the answer is already passed to textbox

             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         #endregion TextBox-Email

        #region TextBox-IntergerPlus-Validations
         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxIntegerPlus_Required_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Age";
             string answer = "12";
             string validationType = "Integer (+)";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();
            
             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);
            
             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();


             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);
            
             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Validate that no error messages present since the answer is already passed to textbox

             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         [Test, RepeatOnFailure(Order = 3)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxIntegerPlus_Required_WithError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Age";
             string answer = "abc";
             string validationType = "Integer (+)";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             TestLog.WriteLine("Id {0}", elementId);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that  error messages present since the answer is not passed to textbox


             Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure(Order = 3)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxIntegerPlus_Required_WithNonIntergerData()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Age";
             string answer = "abc";
             string validationType = "Integer (+)";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

            //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();


             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that  error messages present since the answer invalid format            
             Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.IntegerPlus)));



             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxIntegerPlus_NotRequired_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Age";
             string answer = "Dallas";
             string validationType = "Integer (+)";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             // Not Selecting Answer as required for question
            

             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that no error messages present since the answer is already passed to textbox

             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         #endregion TextBox-IntegerPlus-Validations

        #region TextBox-IntergerMinus-Validations
         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxIntegerMinus_Required_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Age difference";
             string answer = "-12";
             string validationType = "Integer (-, 0, +)";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }


             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();


             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

           
             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Validate that no error messages present since the answer is already passed to textbox

             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));

             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         [Test, RepeatOnFailure(Order = 4)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxIntegerMinus_Required_WithError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Age";
             string answer = "abc";
             string validationType = "Integer (-, 0, +)";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();


             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Validate that  error messages present since the answer is not passed to textbox


            Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure(Order = 4)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxIntegerMinus_Required_WithNonIntergerData()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Age";
             string answer = "abc";
             string validationType = "Integer (-, 0, +)";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);


             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that  error messages present since the answer invalid format                       
             Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.IntegerMinus)));

             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxIntegerMinus_NotRequired_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Age";
             string answer = "Dallas";
             string validationType = "Integer (-, 0, +)";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

           
             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Not Selecting Answer as required for question
           

             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that no error messages present since the answer is already passed to textbox

             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         #endregion TextBox-IntegerMinus-Validations

        #region TextBox-IntegerZero-Validations
         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxIntegerZero_Required_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Age difference";
             string answer = "00";
             string validationType = "Integer (0, +)";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();


             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Validate that no error messages present since the answer is already passed to textbox

             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         [Test, RepeatOnFailure(Order = 4)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxIntegerZero_Required_WithError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Age";
             string answer = "abc";
             string validationType = "Integer (0, +)";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }


             //Login
             test.Portal.LoginWebDriver();
            
             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

           
             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             TestLog.WriteLine("Id {0}", elementId);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that  error messages present since the answer is not passed to textbox


             Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure(Order = 4)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxIntegerZero_Required_WithNonIntergerData()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Age";
             string answer = "abc";
             string validationType = "Integer (0, +)";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }


             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

          
             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

         
             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Validate that  error messages present since the answer invalid format


           
             Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.IntergerZero)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxIntegerZero_NotRequired_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Age";
             string answer = "Dallas";
             string validationType = "Integer (0, +)";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();
            
             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

            //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Not Selecting Answer as required for question
            
             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

          
             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             TestLog.WriteLine("Id {0}", elementId);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Validate that no error messages present since the answer is already passed to textbox

             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         #endregion TextBox-IntegerZero

        #region TextBox-MultiEmail-Validations
         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxMultiEmail_Required_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Email";
             string answer = "abc@abc.com,xyz@xyz.com";
             string validationType = "Multi-email separated by comma or simicolon";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

            //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();


             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);
             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);


             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);


             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that no error messages present since the answer is already passed to textbox

             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         [Test, RepeatOnFailure(Order = 5)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxMultiEmail_Required_WithError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Email";
             string answer = "12.00";
             string validationType = "Multi-email separated by comma or simicolon";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

            
             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

           
             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             TestLog.WriteLine("Id {0}", elementId);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Validate that  error messages present since the answer is not passed to textbox


             Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure(Order = 5)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxMultiEmail_Required_WithNonEmailinfo()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Email";
             string answer = "abc";
             string validationType = "Multi-email separated by comma or simicolon";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);
                        
             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

            
             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that  error messages present since the answer invalid format

                   
             Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.MultiEmail)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxMultiEmail_NotRequired_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Email";
             string answer = "Dallas";
             string validationType = "Multi-email separated by comma or simicolon";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             // test.Driver.FindElementById("ctl00_ctl00_MainContent_content_chkAnswerRequired").Click();


             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

          
             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             TestLog.WriteLine("Id {0}", elementId);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that no error messages present since the answer is already passed to textbox

             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         #endregion TextBox-MultiEmail
        
        #region TextBox-Phone-Validations
         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxPhone_Required_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Phone number";
             string answer = "972-972-9000";
             string validationType = "Phone";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }


             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();


             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

           
             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that no error messages present since the answer is already passed to textbox

             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         [Test, RepeatOnFailure(Order = 5)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxPhone_Required_WithError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Phone number";
             string answer = "12.00";
             string validationType = "Phone";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();



             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             TestLog.WriteLine("Id {0}", elementId);
             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that  error messages present since the answer is not passed to textbox


             Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure(Order = 5)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxPhone_Required_WithNonPhoneinfo()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Phone number";
             string answer = "999999999";
             string validationType = "Phone";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that  error messages present since the answer invalid format
            
             Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Phone)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxPhone_NotRequired_NoError()    
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Phone number";
             string answer = "6666666";
             string validationType = "Phone";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }

             //updated by ivan, 2016/1/7
             try { 
                 //Login
                 test.Portal.LoginWebDriver();

                 //Create New Form
                 test.Portal.WebLink_FormNames_Create(formName, true, true);

                 //Get form ID from database
                 int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

                 //Create Header, question and Answer as Textbox 
                 test.Portal.EventRegistration_Create_Question_Textbox(header, question);

                 //Selecting Answer as required for question
                 // test.Driver.FindElementById("ctl00_ctl00_MainContent_content_chkAnswerRequired").Click();

                 //Selecting validation type for text box from drop down
                 test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

                 //Login to Event Registration Form
                 test.Infellowship.Login_Event_Registration_Form(formName);

                 //select individual
                 test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

                 //Validating  Header Name in infellowship
                 test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
                 test.GeneralMethods.VerifyTextPresentWebDriver(header);

                 //Validating Question in infellowship
                 test.GeneralMethods.VerifyTextPresentWebDriver(question);
                     
                 TestLog.WriteLine("Form Id {0}", formId);
                 int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

                 TestLog.WriteLine("Id {0}", elementId);

                 //Entering Text to Text box
                 test.Infellowship.EventRegistration_TextBox_Answer(answer);

                 //Validate that no error messages present since the answer is already passed to textbox
                 Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));
             }
             finally {
                 //Get form ID from database
                 int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

                 //adding form id to global collection to delete
                 _formId.Add(formId);

                 //Delete Form using Store proc method
                 test.SQL.Weblink_Form_Delete_Proc(15, formId);

                 //Logout from Infellowship
                 //test.Infellowship.LogoutWebDriver();

                 //Closing the browser for now since we don't have signout link for this new form Url
                 test.Driver.Quit();
             }

         }
         #endregion TextBox-Phone

        #region TextBox-PhoneACH-Validations

         [Test, RepeatOnFailure, Timeout(1500)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxPhoneACH_Required_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("xTestForm-Textbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Phone number";
             string answer = "972-972-9000";
             string validationType = "Phone ACH";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }


             //Login
             test.Portal.LoginWebDriver();
             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);
                     
             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that no error messages present since the answer is already passed to textbox

             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         [Test, RepeatOnFailure(Order = 5), Timeout(1500)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxPhoneACH_Required_WithError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("xTestForm-Textbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Phone number";
             string answer = "##";
             string validationType = "Phone ACH";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();
            
             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             TestLog.WriteLine("Id {0}", elementId);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that  error messages present since the answer is not passed to textbox


             Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure(Order = 5), Timeout(1500)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxPhoneACH_Required_WithNonPhoneinfo()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("xTestForm-Textbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Phone number";
             string answer = "#########";
             string validationType = "Phone ACH";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();
             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);


             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that  error messages present since the answer invalid format

             Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.PhoneACH)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxPhoneACH_NotRequired_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - QATextbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter Phone number";
             string answer = "6666666";
             string validationType = "Phone ACH";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);


             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             // test.Driver.FindElementById("ctl00_ctl00_MainContent_content_chkAnswerRequired").Click();


             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             TestLog.WriteLine("Id {0}", elementId);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that no error messages present since the answer is already passed to textbox

             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }
         #endregion TextBox-PhoneACH
        
        #region TextBox-URL-Validations

         [Test, RepeatOnFailure,Timeout(1500)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxURL_Required_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("xTestForm-Textbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter URL";
             string answer = "www.yahoo.com";
             string validationType = "URL";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();


             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);
                        
             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Validate that no error messages present since the answer is already passed to textbox

             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         [Test, RepeatOnFailure(Order = 6), Timeout(1200)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxURL_Required_WithError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("xTestForm-Textbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter URL";
             string answer = "AAAA";
             string validationType = "URL";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();
             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down
             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Name : {0}", formName);
             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             TestLog.WriteLine("Id {0}", elementId);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that  error messages present since the answer is not passed to textbox


             Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure(Order = 7), Timeout(1200)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxURL_Required_WithNonURLinfo()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("xTestForm-Textbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter URL";
             string answer = "#########";
             string validationType = "URL";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }
             
             //Login
             test.Portal.LoginWebDriver();


             test.GeneralMethods.Navigate_Portal(Navigation.Portal.WebLink.Event_Registration.Manage_Forms);

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true);

           
             // finding form name to click by looping through the table
             test.Portal.EventRegistration_Select_FormName_FromTable(formName);

            //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             //test.Driver.FindElementById("ctl00_ctl00_MainContent_content_chkAnswerRequired").Click();
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);


             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);


             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);


             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Validate that  error messages present since the answer invalid format
                  
             Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.URL)));


             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }

         [Test, RepeatOnFailure, Timeout(1500)]
         [Author("Suchitra Patnam")]
         [Description("F1-4207 - Verifies the Header/Question and Answer as required Textbox value Infellowship Registration Form")]
         public void InfellowshipRegistration_Verify_QuestionAndAnswer_TextboxURL_NotRequired_NoError()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("xTestForm-Textbox{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Enter URL";
             string answer = "6666666";
             string validationType = "URL";
             string[] individual = { "Ft Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }


             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             // test.Driver.FindElementById("ctl00_ctl00_MainContent_content_chkAnswerRequired").Click();


             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName);

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Validating  Header Name in infellowship
             test.GeneralMethods.WaitForElementDisplayed(By.Id("brand"));
             test.GeneralMethods.VerifyTextPresentWebDriver(header);

             //Validating Question in infellowship
             test.GeneralMethods.VerifyTextPresentWebDriver(question);

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             TestLog.WriteLine("Id {0}", elementId);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Validate that no error messages present since the answer is already passed to textbox

             Assert.IsFalse(test.GeneralMethods.IsTextPresentWebDriver(string.Format("{0}", GeneralInFellowship.EventRegistration.TextValidationErrorMessage.Question_Required)));


             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         #endregion TextBox-URL

         #endregion Submit Form
    } //End of Text box validations fixture


    [TestFixture]
    class infellowship_EventRegistration_Confirmations_WebDriver : FixtureBaseWebDriver
    {
        // declaring fromId collection
        IList<int> _formId = new List<int>();
               

        public void FixtureSetUp()
        {          
            
        }

        [FixtureTearDown]
        public void FixtureTearDown()
        {

            TestLog.WriteLine("Running Fixture Tear Down......");

            for (int i = 0; i < _formId.Count; i++)
            {
                TestLog.WriteLine("-formId = {0}", _formId[i]);
                base.SQL.Weblink_Form_Delete_Proc(15, _formId[i]);
            }


        }
        #region confirmation message

    //     [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4202 - Confirmation Message")]
         public void InfellowshipRegistration_Verify_ConfirmationMessage_ScheduleName()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions
            string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - ConfirmationMessage{0}",guidRandom);
             string ministry = "A Test Ministry";
             string activity = "A Test Activity";
             string breakoutGroup = "A Test Breakout";
             string activityGroup = "Test Activity RLC Group";
             string[]scheduleActivityName = {"Test Schedule"};            
             string[] individual = { "Automated Tester", "Qa Tester" };
             string[] sendEmail = { "F1AutomatedTester01@gmail.com" };
             string[] emailCC = { "Ft.autotester@gmail.com" };
             double price = 0.0;
             IList<string> selectedOrder = new List<string>();
             


             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();


             //Get form ID from database
            // int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             TestLog.WriteLine("Confirm Form = {0}", formName);                             

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             
             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Create Single Schedule

             test.Portal.EventRegistration_Create_Schedule(ministry, activity, breakoutGroup, activityGroup, scheduleActivityName);

            
             //save changes
             test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnDone").Click();

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Verify  Form Name and schedule

             Assert.AreEqual(formName, test.Driver.FindElementById(GeneralInFellowship.EventRegistration.FormName).Text);

             for (int i = 0; i < scheduleActivityName.Length; i++)
             {
                 Assert.Contains(test.Driver.FindElement(By.XPath("//div[@class='col-sm-8']/span")).Text, scheduleActivityName[i]);
             }

             //Clicking on 'continue' button from step2
             test.Infellowship.EventRegistration_ClickContinue();

             //Verify Step3 Summary details..
             test.Infellowship.EventRegistration_Step3_Summary(formName, individual, selectedOrder, price, sendEmail, emailCC);

             //Submitting details from Step3..
             test.Infellowship.EventRegistration_ClickSubmitPayment();
            
             //Verify Form Name and Schedule name selected are displayed in confirmation page
             test.GeneralMethods.VerifyTextPresentWebDriver(formName);
             for (int i = 0; i < scheduleActivityName.Length; i++)
             {
                 test.GeneralMethods.VerifyTextPresentWebDriver(scheduleActivityName[i]);
             }

             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();
         }


         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4202 - Confirmation Message")]
         public void InfellowshipRegistration_Verify_ConfirmationMessage_WithQuestions()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions
             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - ConfirmationMessage{0}", guidRandom);
            
             string[] scheduleActivityName = { "Test Activity Schedule" };
             string emailAddress = "Ft.autotester@gmail.com";
             string emailMessage = "This is Email Text";
             string[] individual = { "Automated Tester"};
             string[] sendEmail = { "F1AutomatedTester01@gmail.com" };
             string[] emailCC = { "Ft.autotester@gmail.com" };
             double price = 0.0;
             IList<string> selectedOrder = new List<string>();
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Please Enter City Name you live";
             string answer = "city1";
             string validationType = "no validation";


             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();
                     

             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);
             //adding form id to global collection to delete
             _formId.Add(formId);

             //******To Test flow with question*******
             //Create Header, question and Answer as Textbox 

             test.Portal.EventRegistration_Create_Question_Textbox(header, question);

             //Selecting Answer as required for question
             test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

             //Drop down seelct
             //   this._driver.FindElementById("ctl00_ctl00_MainContent_content_rdbtnDropDown").Click();

             //Selecting validation type for text box from drop down

             test.Portal.EventRegistration_Select_TextBox_ValidationType(validationType);

             //****************

             //Create Custom Email message

//             test.Portal.EventRegistration_Create_CustomConfirmationMessage(emailMessage, emailAddress);


             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Verify  Form Name 

             Assert.AreEqual(formName, test.Driver.FindElementById(GeneralInFellowship.EventRegistration.FormName).Text);

            
             //Clicking on 'continue' button from step2
             test.Infellowship.EventRegistration_ClickContinue();

             //Entering Text to Text box

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);


             //Verify Step3 Summary details..
             test.Infellowship.EventRegistration_Step3_Summary(formName, individual, selectedOrder, price, sendEmail, emailCC);

             //Submitting details from Step3..
             test.Infellowship.EventRegistration_ClickSubmitPayment();

             //Verify Form Name and Confirmation message created are displayed in confirmation page
             test.GeneralMethods.VerifyTextPresentWebDriver(formName);
//             test.GeneralMethods.VerifyTextPresentWebDriver(emailMessage);
            
             //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

       }
        

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4202 - Confirmation Message")]
         public void InfellowshipRegistration_Verify_ConfirmationMessage()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions
             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - ConfirmationMessage{0}", guidRandom);

             string[] scheduleActivityName = { "Test Activity Schedule" };
             string emailAddress = "Ft.autotester@gmail.com";
             string emailMessage = "This is Email Text";
             string[] individual = { "Automated Tester" };
             string[] sendEmail = { "F1AutomatedTester01@gmail.com" };
             string[] emailCC = { "Ft.autotester@gmail.com" };
             double price = 0.0;
             IList<string> selectedOrder = new List<string>();
            


             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }

             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Create Custome Email message

             test.Portal.EventRegistration_Create_CustomConfirmationMessage(emailMessage, emailAddress);


             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Verify  Form Name 

             Assert.AreEqual(formName, test.Driver.FindElementById(GeneralInFellowship.EventRegistration.FormName).Text);


             //Clicking on 'continue' button from step2
             test.Infellowship.EventRegistration_ClickContinue();

             
             //Verify Step3 Summary details..
             test.Infellowship.EventRegistration_Step3_Summary(formName, individual, selectedOrder, price, sendEmail, emailCC);

             //Submitting details from Step3..
             test.Infellowship.EventRegistration_ClickSubmitPayment();

             //Verify Form Name and Confirmation message created are displayed in confirmation page
             test.GeneralMethods.VerifyTextPresentWebDriver(formName);
             test.GeneralMethods.VerifyTextPresentWebDriver(emailMessage);

             //Delete Form using Store proc method
             // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }


         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-5664 - Link to back page from Step 3 ")]
         public void InfellowshipRegistration_Verify_Step3BackButton()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions
             string formName = "A Test Form - Step3 Back link";
             string[] individual = { "Automated Tester" };             
             string answer = "city1";
             IList<string> selectedOrder = new List<string>();


             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }



             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.EventRegistration_Select_FormName_FromTable(formName);
                                                  

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Verify  Form Name 

             Assert.AreEqual(formName, test.Driver.FindElementById(GeneralInFellowship.EventRegistration.FormName).Text);

                        
             //click on back to Step1 and comeback to verify the previously selected users are still selected

             test.Driver.FindElementByLinkText("Return to Step 1").Click();

             //Click continue to come back to step2 
             
             test.Infellowship.EventRegistration_ClickContinue();       
             
              //Entering Text to Text box
             test.Infellowship.EventRegistration_TextBox_Answer(answer);

             //Click back link to goto step 2 from step 3
             test.Driver.FindElementByLinkText("Return to Step 2").Click();

            //Verify that the answer value is persist in page
             test.GeneralMethods.VerifyTextPresentWebDriver(answer);

             //Click continue 

             test.Infellowship.EventRegistration_ClickContinue();

             
             //Submitting details from Step3..
             test.Infellowship.EventRegistration_ClickSubmitPayment();

             //Verify Form Name 
             test.GeneralMethods.VerifyTextPresentWebDriver(formName);
             
             //Delete Form using Store proc method
             // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }
        
        

        #endregion confirmation message

    } // End of Confirmations fixture..


    [TestFixture]
    class infellowship_EventRegistration_MultipleUserSelection_WebDriver : FixtureBaseWebDriver
    {
        // declaring fromId collection
        IList<int> _formId = new List<int>();       

        public void FixtureSetUp()
        {                    
            
        }

        [FixtureTearDown]
        public void FixtureTearDown()
        {

            TestLog.WriteLine("Running Fixture Tear Down......");

            for (int i = 0; i < _formId.Count; i++)
            {
                TestLog.WriteLine("-formId = {0}", _formId[i]);
                base.SQL.Weblink_Form_Delete_Proc(15, _formId[i]);
            }


        }
        #region Multiple individual selection

         

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4402 - Verifies the individual selection in step2 of Event Registration")]
         public void InfellowshipRegistration_Verify_Individual_Selected_Order_Step2()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - SelectOrder{0}", guidRandom);

             string[] individual = { "Automated Tester", "Qa Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);
             }

             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);      

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Verify individual selection in step2
             test.Infellowship.EventRegistration_Individual_Selected_Order_step2(individual, selectedOrder);

             //Click Continue
             test.Infellowship.EventRegistration_ClickContinue();
                          
             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();


         }  
       
        #endregion Multiple Individuals selection 

    } // End of Multiple Use selection fixture

    

    [TestFixture]
    class infellowship_EventRegistration_SharedQuestions_WebDriver : FixtureBaseWebDriver
    {
        // declaring fromId collection
        IList<int> _formId = new List<int>();


        //    string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
        //    string formName = string.Format("Test Form - QATextbox{0}", guidRandom);

        public void FixtureSetUp()
        {

            //     SQL.WebLink_FormNames_Create(1, 15, formName, true);

            //Get form ID from database
            //    int formId = SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

            //    return formId;


        }

        [FixtureTearDown]
        public void FixtureTearDown()
        {

            TestLog.WriteLine("Running Fixture Tear Down......");

            for (int i = 0; i < _formId.Count; i++)
            {
                TestLog.WriteLine("-formId = {0}", _formId[i]);
                base.SQL.Weblink_Form_Delete_Proc(15, _formId[i]);
            }


        }
         #region Shared Questions
         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4229 - Shared question check box")]
         public void InfellowshipRegistration_Verify_SharedQuestionCheckbox()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

             // Set initial conditions
             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - Shared Question{0}", guidRandom);

             //updated by ivan, 2016/1/7
             try { 
                 //Login
                 test.Portal.LoginWebDriver();

                 //Create New Form
                 test.Portal.WebLink_FormNames_Create(formName, true, true);

                 //Create new Header
                 test.Driver.FindElementByLinkText("Questions and answers").Click();
                 test.Driver.FindElementByLinkText("Add new header").Click();

                 //Verify Shared check box exisits for Header
                 test.GeneralMethods.VerifyElementPresentWebDriver(By.Id(Navigation.Portal.WebLink.Event_Registration.ManageForms.Shared_Question_Checkbox));
            }
             finally {
                 //Get form ID from database
                 int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

                 //adding form id to global collection to delete
                 _formId.Add(formId);

                 //Delete Form using Store proc method
                 test.SQL.Weblink_Form_Delete_Proc(15, formId);

                 //Closing the browser for now since we don't have signout link for this new form Url
                 test.Driver.Quit();
             }
         }


         [Test, RepeatOnFailure, Timeout(1200)]
         [Author("Suchitra Patnam")]
         [Description("F1-4228 - Multiple Registrants shared Text questions")]
         public void InfellowshipRegistration_Verify_Shared_Answers_MultipleIndividuals_TextBox()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions
             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - Shared Question{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Please Enter City Name you live";
             string[] answer = {"city1"};
             string[] individual = { "Automated Tester", "Qa Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }

             //updated by ivan, 2016/1/7
             try { 
                 //Login
                 test.Portal.LoginWebDriver();

                 //Create New Form
                 test.Portal.WebLink_FormNames_Create(formName, true, true);

                 //Create Header, question and Answer as Textbox 

                 test.Portal.EventRegistration_Create_SharedHeader_Question_Textbox(header, question);

                 //Selecting Answer as required for question
                 test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Answer_Required).Click();

                 // Adding/submitting question
                 test.Driver.FindElementById(Navigation.Portal.WebLink.Event_Registration.ManageForms.Save_Question).Click();
             
                 //Login to Event Registration Form
                 test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

                 //select individual
                 test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, true, true);

                 //Verify  Form Name 

                 Assert.AreEqual(formName, test.Driver.FindElementById(GeneralInFellowship.EventRegistration.FormName).Text);


                 //Clicking on 'continue' button
                 test.Infellowship.EventRegistration_ClickContinue();

                 //Validating Question in infellowship
                 test.GeneralMethods.VerifyTextPresentWebDriver(question);

                 //Passing answer to Shared question
                 test.Infellowship.EventRegistration_Shared_TextBox_Answer(answer);

                 //Verifying shared questions and answers
               //  test.Infellowship.EventRegistration_Shared_Textbox_Answer_Verification(answer);


                 //Testing the java script to  verify answer

                 Assert.IsTrue(test.Infellowship.EventRegistration_Shared_Textbox_Answer_Verification_Javascript(), "Individual Answers not matched");
             
                 /*     //Verify that individuals names are present in confirmation page
                 for (int j = 0; j < individual.Length; j++)
                 {
                     test.GeneralMethods.VerifyTextPresentWebDriver(individual[j]);

                 }

                 //Verify Form Name and Confirmation message created are displayed in confirmation page
                 test.GeneralMethods.VerifyTextPresentWebDriver(formName);
             
                 string url = test.Driver.Url;

                 string confirmcode = test.Infellowship.GetConfirmationCode(url);
                 TestLog.WriteLine("Confirm Code = {0}", confirmcode); */
             }
             finally {
                 //Get form ID from database
                 int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);
                 //adding form id to global collection to delete
                 _formId.Add(formId);
                 //Delete Form using Store proc method
                 test.SQL.Weblink_Form_Delete_Proc(15, formId);

                 //Closing the browser for now since we don't have signout link for this new form Url
                 test.Driver.Quit();
             }
         }

         [Test, RepeatOnFailure, Timeout(1200)]
         [Author("Suchitra Patnam")]
         [Description("F1-4228 - Multiple Registrants shared drop down questions")]
         public void InfellowshipRegistration_Verify_Shared_Answers_MultipleIndividuals_Dropdown()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions
             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - Shared Question{0}", guidRandom);
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Please Enter City Name you live";
             string [] answer = {"city1", "city2", "city3"};
             string answerChoice = "city2";
             string[] individual = { "Automated Tester", "Qa Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }

             //updated by ivan, 2016/1/7
             try { 
                 //Login
                 test.Portal.LoginWebDriver();

                 //Create New Form
                 test.Portal.WebLink_FormNames_Create(formName, true, true);

                 //Create Header, question and Answer as drop down 
                 test.Portal.EventRegistration_Create_QuestionAndAnswer_Dropdown(header, question, answer);

                 //Navigate Back to Form page
                 test.Driver.FindElementById("tab_back").Click();
                 test.Driver.FindElementById("tab_back").Click();

                 //Login to Event Registration Form
                 test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

                 //select individual
                 test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, true, true);

                 //Verify  Form Name 
                 Assert.AreEqual(formName, test.Driver.FindElementById(GeneralInFellowship.EventRegistration.FormName).Text);

                 //Clicking on 'continue' button
                 test.Infellowship.EventRegistration_ClickContinue();

                 //Validating Question in infellowship
                 test.GeneralMethods.VerifyTextPresentWebDriver(question);

                 //Passing answer to Shared question
                 test.Infellowship.EventRegistration_Shared_Dropdown_Answer(answerChoice);

                 //Verifying shared questions and answers
                 //  test.Infellowship.EventRegistration_Shared_Textbox_Answer_Verification(answer);

                 //Testing the java script to  verify answer

                 Assert.IsTrue(test.Infellowship.EventRegistration_Shared_Textbox_Answer_Verification_Javascript(), "Individual Answers not matched" );

             

                 /*     //Verify that individuals names are present in confirmation page
                 for (int j = 0; j < individual.Length; j++)
                 {
                     test.GeneralMethods.VerifyTextPresentWebDriver(individual[j]);

                 }

                 //Verify Form Name and Confirmation message created are displayed in confirmation page
                 test.GeneralMethods.VerifyTextPresentWebDriver(formName);
             
                 string url = test.Driver.Url;

                 string confirmcode = test.Infellowship.GetConfirmationCode(url);
                 TestLog.WriteLine("Confirm Code = {0}", confirmcode); */


             }
             finally { 
                 //Delete Form using Store proc method
                  //Get form ID from database
                  int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);
                  //adding form id to global collection to delete
                  _formId.Add(formId);
                  test.SQL.Weblink_Form_Delete_Proc(15, formId);

                  //Closing the browser for now since we don't have signout link for this new form Url
                  test.Driver.Quit();
             }
         }

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("Multiple Headers and multiple Registrants shared Text questions")]
         public void InfellowshipRegistration_Verify_Shared_Answers_MultipleIndividuals_TextBox_MultipleHeaders()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions
            
             string formName = "A Test Form - Multiple Shared Headers";
             string[] answers = { "Ans1", "Ans2" };
            
             string[] individual = { "Automated Tester", "Qa Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }


             //Login
             test.Portal.LoginWebDriver();
                         

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, true, true);

             //Verify  Form Name 

             Assert.AreEqual(formName, test.Driver.FindElementById(GeneralInFellowship.EventRegistration.FormName).Text);


             //Clicking on 'continue' button
             test.Infellowship.EventRegistration_ClickContinue();

             //Validating Question in infellowship
            // test.GeneralMethods.VerifyTextPresentWebDriver(question);

             //Passing answer to Shared question
             test.Infellowship.EventRegistration_Shared_TextBox_Answer(answers);

             //Verifying shared questions and answers
             //  test.Infellowship.EventRegistration_Shared_Textbox_Answer_Verification(answer);


             //Testing the java script to  verify answer

             Assert.IsTrue(test.Infellowship.EventRegistration_Shared_Textbox_Answer_Verification_Javascript(), "Individual Answers not matched");

             /*     //Verify that individuals names are present in confirmation page
             for (int j = 0; j < individual.Length; j++)
             {
                 test.GeneralMethods.VerifyTextPresentWebDriver(individual[j]);

             }

             //Verify Form Name and Confirmation message created are displayed in confirmation page
             test.GeneralMethods.VerifyTextPresentWebDriver(formName);
             
             string url = test.Driver.Url;

             string confirmcode = test.Infellowship.GetConfirmationCode(url);
             TestLog.WriteLine("Confirm Code = {0}", confirmcode); */



             //Delete Form using Store proc method
             //test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();
         }

         


         #endregion Shared Questions
    } // End of Shared Questions fixture

    [TestFixture]
    class infellowship_EventRegistration_Pricing_WebDriver : FixtureBaseWebDriver
    {
        // declaring fromId collection
        IList<int> _formId = new List<int>();


        //    string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
        //    string formName = string.Format("Test Form - QATextbox{0}", guidRandom);

        public void FixtureSetUp()
        {

            //     SQL.WebLink_FormNames_Create(1, 15, formName, true);

            //Get form ID from database
            //    int formId = SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

            //    return formId;


        }

        [FixtureTearDown]
        public void FixtureTearDown()
        {

            TestLog.WriteLine("Running Fixture Tear Down......");

            for (int i = 0; i < _formId.Count; i++)
            {
                TestLog.WriteLine("-formId = {0}", _formId[i]);
                base.SQL.Weblink_Form_Delete_Proc(15, _formId[i]);
            }


        }
        #region Form Pricing

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4238 - Reg 2: Epic 4 - Implement Step 1  Registration Cost")]
         public void InfellowshipRegistration_Verify_Form_Pricing_Steps_FutureDates()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - Pricing{0}", guidRandom);
             double price = 5.00;
             string fund = "1 - General Fund (Contribution)";
             string startDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Date.ToShortDateString();
             string endDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(2).Date.ToShortDateString();

             string[] individual = { "Automated Tester", "Qa Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }

             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Account/Fund Association for form
             test.Portal.EventRegistration_Create_Fund_Form(fund);

             //Create Price date range for form
             test.Portal.EventRegistration_Create_Price_Form(price, startDate, endDate);

             //Navigate Back to Form page
             test.Driver.FindElementById("tab_back").Click();


             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Verify individual price in step2
             test.Infellowship.EventRegistration_Individual_Selected_Order_Price(individual, selectedOrder, price);

             //Click Continue
             test.Infellowship.EventRegistration_ClickContinue();

             //Verify individual price in step2 - commenting these until figureout a way to split the logic of select order from individual selection method........
            // test.Infellowship.EventRegistration_Individual_Selected_Order_Price(individual, selectedOrder, price);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4238 - Reg 2: Epic 4 - Implement Step 1  Registration Cost")]
         public void InfellowshipRegistration_Verify_Form_Pricing_Steps_PastDates()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - Pricing{0}", guidRandom);
             double price = 5.00;
             string fund = "1 - General Fund (Contribution)";
             string startDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(-2).Date.ToShortDateString();
             string endDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(-1).Date.ToShortDateString();

             string[] individual = { "Automated Tester", "Qa Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }

             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Account/Fund Association for form
             test.Portal.EventRegistration_Create_Fund_Form(fund);

             //Create Price date range for form
             test.Portal.EventRegistration_Create_Price_Form(price, startDate, endDate);

             //Navigate Back to Form page
             test.Driver.FindElementById("tab_back").Click();


             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Verify individual price in step1
             //passing 0 amount since for past dates it should display zero amount
             test.Infellowship.EventRegistration_Individual_Selected_Order_Price(individual, selectedOrder, 0.0);

             //Click Continue
             test.Infellowship.EventRegistration_ClickContinue();

             //Verify individual price in step2
             //passing 0 amount since for past dates it should display zero amount
            // test.Infellowship.EventRegistration_Individual_Selected_Order_Price(individual, selectedOrder, 0.0);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4238 - Reg 2: Epic 4 - Implement Step 1  Registration Cost")]
         public void InfellowshipRegistration_Verify_Form_Pricing_Steps_WithdecimalAmount()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - Pricing{0}", guidRandom);
             double price = 5.22;
             string fund = "1 - General Fund (Contribution)";
             string startDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Date.ToShortDateString();
             string endDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(2).Date.ToShortDateString();

             string[] individual = { "Automated Tester", "Qa Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }

             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Account/Fund Association for form
             test.Portal.EventRegistration_Create_Fund_Form(fund);

             //Create Price date range for form
             test.Portal.EventRegistration_Create_Price_Form(price, startDate, endDate);

             //Navigate Back to Form page
             test.Driver.FindElementById("tab_back").Click();


             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Verify individual price in step1
             test.Infellowship.EventRegistration_Individual_Selected_Order_Price(individual, selectedOrder, price);

             //Click Continue
             test.Infellowship.EventRegistration_ClickContinue();

             //Verify individual price in step2
             //test.Infellowship.EventRegistration_Individual_Selected_Order_Price(individual, selectedOrder, price);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4238 - Reg 2: Epic 4 - Implement Step 1  Registration Cost")]
         public void InfellowshipRegistration_Verify_Form_Pricing_Step1_NoFormCost()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions


             string formName = "A Test Form - Non Payment";
             double price = 0.0;

             string[] individual = { "Automated Tester", "Qa Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }

             
             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Verify individual price in step1
             test.Infellowship.EventRegistration_Individual_Selected_Order_Price(individual, selectedOrder, price);

             //Click Continue
             test.Infellowship.EventRegistration_ClickContinue();

             //Verify individual price in step2
             //test.Infellowship.EventRegistration_Individual_Selected_Order_Price(individual, selectedOrder, price);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4238 - Reg 2: Epic 4 - Implement Step 1  Registration Cost")]
         public void InfellowshipRegistration_Verify_Form_Pricing_Step2_WithdecimalAmount()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - Pricing{0}", guidRandom);
             double price = 5.22;
             string fund = "1 - General Fund (Contribution)";
             string startDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Date.ToShortDateString();
             string endDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(2).Date.ToShortDateString();

             string[] individual = { "Automated Tester", "Qa Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }

             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Account/Fund Association for form
             test.Portal.EventRegistration_Create_Fund_Form(fund);

             //Create Price date range for form
             test.Portal.EventRegistration_Create_Price_Form(price, startDate, endDate);

             //Navigate Back to Form page
             test.Driver.FindElementById("tab_back").Click();


             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Verify individual price in step1
             test.Infellowship.EventRegistration_Individual_Selected_Order_Price(individual, selectedOrder, price);

             //Click Continue
             //test.Infellowship.EventRegistration_ClickContinue();

             //Verify individual Price in step2
             
             test.Infellowship.EventRegistration_Individual_Selected_Order_Price(individual, selectedOrder, price);

             //Verify individual price in step2
             //test.Infellowship.EventRegistration_Individual_Selected_Order_Price(individual, selectedOrder, price);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }
         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4238 - Reg 2: Epic 4 - Implement Step 1  Registration Cost")]
         public void InfellowshipRegistration_Verify_Form_Pricing_Step2_NoFormCost()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions


             string formName = "A Test Form - Non Payment";
             double price = 0.0;

             string[] individual = { "Automated Tester", "Qa Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }


             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Verify individual price in step1
             test.Infellowship.EventRegistration_Individual_Selected_Order_Price(individual, selectedOrder, price);

             
             //Verify individual price in step2
             test.Infellowship.EventRegistration_Individual_Selected_Order_Price_Step2(individual, selectedOrder, price);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4237 - Reg 2: Epic 4 - Implement Step 2 Upsell page")]
         public void InfellowshipRegistration_Verify_Step2_Upsell_Price()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions
             
             string formName = "A Test Form - Upsell Only";

             string[] individual = { "Automated Tester"};
             double price = 5.11;
             IList<string> selectedOrder = new List<string>();
             string header = "Header--1";
             string question = "Please Select T-shirt size";
             string[] answer = { "Small", "Medium", "Large" };
             string[] answerPrice = { "5.11", "6.11", "8.11" };
             string validationType = "no validation";
             string[] answerChoice = { "Small" };
             double answerChoicePrice = 5.11;
             string[] answerChoiceDisplay = { "Small ($5.11)" };
             
             

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }


             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);                          

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Verify  Form Name 

             Assert.AreEqual(formName, test.Driver.FindElementById(GeneralInFellowship.EventRegistration.FormName).Text);

                         

             //Entering Text to Text box

            
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_Dropdown_Answer_Select(answerChoiceDisplay);

             //Verify individula selection and price and total values
             test.Infellowship.EventRegistration_Individual_Selected_Order_Price_Upsell_Step2(individual, selectedOrder, price);

             //Click continue to step3...
             test.Infellowship.EventRegistration_ClickContinue();
                          

             //Submitting details from Step3..
             test.Infellowship.EventRegistration_ClickSubmitPayment();

             //Verify Form Name and Confirmation message created are displayed in confirmation page
             test.GeneralMethods.VerifyTextPresentWebDriver(formName);
             

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }


        #endregion Form Pricing

        #region Step3

       //  [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4236 - Reg 2: Epic 4 - Implement Step 3 page")]
         public void InfellowshipRegistration_Verify_Form_Step3_Summary()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


             // Set initial conditions

             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - Pricing{0}", guidRandom);
             double price = 5.11;
             string fund = "1 - General Fund (Contribution)";
             string startDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Date.ToShortDateString();
             string endDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(2).Date.ToShortDateString();
             string emailSender = "fellowshiponemail@activenetwork.com";
             string emailMessage = "This is Email Text";
             string emailReceipient = "Ft.autotester@gmail.com";
             string emailSubject = "Registration Confirmation";
             string [] sendEmail = {"F1AutomatedTester01@gmail.com"};
             string [] emailCC = {"Ft.autotester@gmail.com"};
             string password = "ActiveQA12";
             string passwordCC = "Romans10:9";
             string mailbox = EmailMailBox.GMAIL.INBOX;
             string[] individual = { "Automated Tester", "Qa Tester" };
             IList<string> selectedOrder = new List<string>();

             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }

             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

             //adding form id to global collection to delete
             _formId.Add(formId);

             //Create Account/Fund Association for form
             test.Portal.EventRegistration_Create_Fund_Form(fund);

             //Create Price date range for form
             test.Portal.EventRegistration_Create_Price_Form(price, startDate, endDate);

             //Navigate Back to Form page
             test.Driver.FindElementById("tab_back").Click();                                   

             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

             //select individuals
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             
             //Verify individual price in step2
             test.Infellowship.EventRegistration_Individual_Selected_Order_Price(individual, selectedOrder, price);

             //Click Continue
             test.Infellowship.EventRegistration_ClickContinue();

             //Verify Step3 Summary details..
             test.Infellowship.EventRegistration_Step3_Summary(formName, individual, selectedOrder, price, sendEmail, emailCC);

             
             //Submitting details from Step3..
             test.Infellowship.EventRegistration_ClickSubmitPayment();


             //Get confirmation mesasge
             string url = test.Driver.Url;
             string confirmcode = test.Infellowship.GetConfirmationCode(url, 15);
             TestLog.WriteLine("Confirm Code = {0}", confirmcode);


             //Verify  email was sent

             for (int i = 0; i < sendEmail.Length; i++)
             {
                 Message grpMsg = test.Portal.Retrieve_Search_Weblink_Confirmation_Email(sendEmail[i], password, confirmcode);
                 TestLog.WriteLine("grpMSg = {0}", grpMsg);
                 test.Portal.Verify_EventRegistration_Confirmation_Email(grpMsg, emailSubject, emailSender, sendEmail[i], sendEmail, emailMessage, confirmcode, individual);
                 
                 //Delete Email from inbox
                 TestLog.WriteLine("Deleting Email after verififcation");
                 test.Portal.Delete_Email(grpMsg, sendEmail[i], password, mailbox);
             
             }
             
             //CC email verification
             for (int j = 0; j < sendEmail.Length; j++)
             {
                 Message grpMsg1 = test.Portal.Retrieve_Search_Weblink_Confirmation_Email(emailCC[1], passwordCC, confirmcode);
                 TestLog.WriteLine("grpMSg = {0}", grpMsg1);
                 test.Portal.Verify_EventRegistration_Confirmation_Email(grpMsg1, emailSubject, emailSender, emailCC[j], sendEmail, emailMessage, confirmcode, individual);
                 
                 //Delete Email from inbox
                 TestLog.WriteLine("Deleting Email after verififcation");
                 test.Portal.Delete_Email(grpMsg1, emailCC[j], passwordCC, mailbox);
             }
             
            

             //Verify individual price in step2 - commenting these until figureout a way to split the logic of select order from individual selection method........
             // test.Infellowship.EventRegistration_Individual_Selected_Order_Price(individual, selectedOrder, price);

             //Logout from Infellowship
             //test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }


         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4236 - Reg 2: Epic 4 - Implement Step 3 page")]
         public void InfellowshipRegistration_Verify_Form_Step3_TimeoutExpired()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
             string userName = "stuplatttest2390@gmail.com";
             string password = "Romans10:9";
            // string userName = "ft.autotester@gmail.com";
            // string password = "FT4life!";
             string formName = "TimeOut Test Form";
             IList<string> selectedOrder = new List<string>();
             string individualName = "Stuart Platt";
             selectedOrder.Add(individualName);

             //login to infellowship 
             test.Infellowship.LoginWebDriver(userName, password);

             //Get form link
             string formURL = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, 15);

             //Navigate to URL
             test.GeneralMethods.OpenURLWebDriver(formURL);
             test.GeneralMethods.WaitForElement(By.Id(GeneralInFellowship.EventRegistration.FormName));

             //Select individual
             test.Infellowship.EventRegistration_Select_Individual(new string[] { "Stuart Platt" }, selectedOrder);

             test.GeneralMethods.WaitForElement(By.Id("continue"));

             //navigate to step3....Click Continue
             test.Infellowship.EventRegistration_ClickContinue();

             //Verify Warning Text Present
             IWebElement warning = test.Driver.FindElement(By.Id("warning_modal"));
             string warningText = warning.GetAttribute("innerHTML").Trim();
             string cleanedText = warningText.Replace("\t", "").Replace("\r", "").Replace("\n", "");
             TestLog.WriteLine("This is the cleaned text -- {0}", cleanedText);
             Assert.Contains(cleanedText, "<h4 class=\"modal-title\">Time Expiring</h4>");
             Assert.Contains(cleanedText, "<big>Due to inactivity you will lose your reserved registration(s) in approximately:</big>");
             Assert.Contains(cleanedText, "<button type=\"button\" class=\"btn btn-primary\" data-role=\"extend\" id=\"ResetFormTimer\">I'm still here</button>");
             Assert.Contains(cleanedText, "<button type=\"button\" class=\"btn btn-default\" data-dismiss=\"modal\" id=\"DeleteFormIndividualSet\">Give them away!</button>");

             // Click Church logo to return to home page, element identification is different on environments.
             if (base.F1Environment == F1Environments.STAGING)
             {
                 //test.GeneralMethods.WaitForElement(By.XPath("//div[@id='brand']/a"));
                 //test.Driver.FindElementByXPath("//div[@id='brand']/a").Click();
                 //modify by Grace Zhang
                 test.GeneralMethods.WaitForElement(By.XPath("//div[@id='brand']//a"));
                 test.Driver.FindElementByXPath("//div[@id='brand']//a").Click();

             }
             else
             {
                 test.GeneralMethods.WaitForElement(By.XPath("//div[@id='brand']//a"));
                 test.Driver.FindElementByXPath("//div[@id='brand']//a").Click();
             }
            
             test.GeneralMethods.WaitForElement(By.LinkText("Sign out"));
             //Logout 
             test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();
         }

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4236 - Reg 2: Epic 4 - Implement Step 3 page")]
         public void InfellowshipRegistration_Verify_Form_Step3_TimeoutWarning()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
             string userName = "stuplatttest2390@gmail.com";
             string password = "Romans10:9";
             string formName = "TimeOut Test Form";
             IList<string> selectedOrder = new List<string>();
             string individualName = "Stuart Platt";
             selectedOrder.Add(individualName);

             //login to infellowship 
             test.Infellowship.LoginWebDriver(userName, password);

             //Get form link
             string formURL = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, 15);

             //Navigate to URL
             test.GeneralMethods.OpenURLWebDriver(formURL);
             test.GeneralMethods.WaitForElement(By.Id(GeneralInFellowship.EventRegistration.FormName));

             //Select individual
             test.Infellowship.EventRegistration_Select_Individual(new string[] { "Stuart Platt" }, selectedOrder);

             test.GeneralMethods.WaitForElement(By.Id("continue"));

             //navigate to step3....Click Continue
             test.Infellowship.EventRegistration_ClickContinue();

             //Verify Expired Text Present
             IWebElement warning = test.Driver.FindElement(By.Id("expired_modal"));
             string warningText = warning.GetAttribute("innerHTML").Trim();
             string cleanedText = warningText.Replace("\t", "").Replace("\r", "").Replace("\n", "");
             Assert.Contains(cleanedText, "<h4 class=\"modal-title\">Time Expired</h4>");
             Assert.Contains(cleanedText, "<big>Your time has expired and your registrations have been released.</big>");
             Assert.Contains(cleanedText, "<a id=\"GoToForm\" class=\"btn btn-default\">Restart the registration process</a>");

             //CLick Logo
             // test.Driver.FindElementByClassName("brand_replacement").Click();

             if (base.F1Environment == F1Environments.STAGING)
             {
                 //test.GeneralMethods.WaitForElement(By.XPath("//div[@id='brand']/a"));
                 //test.Driver.FindElementByXPath("//div[@id='brand']/a").Click();
                 //modify by Grace Zhang
                 test.GeneralMethods.WaitForElement(By.XPath("//div[@id='brand']//a"));
                 test.Driver.FindElementByXPath("//div[@id='brand']//a").Click();

             }
             else
             {
                 test.GeneralMethods.WaitForElement(By.XPath("//div[@id='brand']//a"));
                 test.Driver.FindElementByXPath("//div[@id='brand']//a").Click();
             }

             test.GeneralMethods.WaitForElement(By.LinkText("Sign out"));
             //Logout 
             test.Infellowship.LogoutWebDriver();

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

        #endregion Step3

        #region Upsell

         [Test, RepeatOnFailure]
         [Author("Suchitra Patnam")]
         [Description("F1-4237 - Reg 2: Epic 4 - Implement Step 2 Upsell page")]
         public void InfellowshipRegistration_Verify_Upsell_Price()
         {
             TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

             // Set initial conditions
             string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
             string formName = string.Format("Test Form - ConfirmationMessage{0}", guidRandom);

             string[] scheduleActivityName = { "Test Activity Schedule" };
             string emailAddress = "Ft.autotester@gmail.com";
             string emailMessage = "This is Email Text";
             string[] individual = { "Automated Tester" };
             string[] sendEmail = { "F1AutomatedTester01@gmail.com" };
             string[] emailCC = { "Ft.autotester@gmail.com" };
             double price = 5.11;
             IList<string> selectedOrder = new List<string>();
             string header = string.Format("Header-{0}", guidRandom);
             string question = "Please Select T-shirt size";
             string[] answer = {"Small", "Medium", "Large"};
             string[] answerPrice = { "5.11", "6.11", "8.11" };
             string validationType = "no validation";
             string[] answerChoice = { "Small" };
             double answerChoicePrice = 5.11;
             string[] answerChoiceDisplay = { "Small ($5.11)" };
             string fund = "1 - General Fund (Contribution)";
             string startDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Date.ToShortDateString();
             string endDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(2).Date.ToShortDateString();


             //Adding individuals to selected order
             for (int i = 0; i < individual.Length; i++)
             {
                 selectedOrder.Add(individual[i]);

             }

             //Login
             test.Portal.LoginWebDriver();


             //Create New Form
             test.Portal.WebLink_FormNames_Create(formName, true, true);

             //Create Account/Fund Association for form
             test.Portal.EventRegistration_Create_Fund_Form(fund);

             //Create Price date range for form
             test.Portal.EventRegistration_Create_Price_Form(price, startDate, endDate);

             //Navigate Back to Form page
             test.Driver.FindElementById("tab_back").Click();

             //Get form ID from database
             int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);
             //adding form id to global collection to delete
             _formId.Add(formId);
             
             //Create Header, question and Answer as Drop down

             test.Portal.EventRegistration_Create_QuestionAndAnswer_Dropdown_Upsell(header, question, answer, answerPrice);

             //Create Custome Email message

             //  test.Portal.EventRegistration_Create_CustomConfirmationMessage(emailMessage, emailAddress);


             //Login to Event Registration Form
             test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

             //select individual
             test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

             //Verify  Form Name 

             Assert.AreEqual(formName, test.Driver.FindElementById(GeneralInFellowship.EventRegistration.FormName).Text);


             //Clicking on 'continue' button from step2
             test.Infellowship.EventRegistration_ClickContinue();

             //Entering Text to Text box

             TestLog.WriteLine("Form Id {0}", formId);
             int elementId = test.SQL.Weblink_InfellowshipForm_GetFormItemId(15, formId, header, question);

             //Entering Text to Text box
             test.Infellowship.EventRegistration_Dropdown_Answer_Select(answerChoiceDisplay);

             test.Infellowship.EventRegistration_ClickContinue();

             //Verify Step3 Summary details..
             test.Infellowship.EventRegistration_Step3_Summary_Upsell(formName, individual, selectedOrder, price, sendEmail, emailCC, answerChoice, answerChoicePrice);

             //Submitting details from Step3..
             test.Infellowship.EventRegistration_ClickSubmitPayment();

             //Verify Form Name and Confirmation message created are displayed in confirmation page
             test.GeneralMethods.VerifyTextPresentWebDriver(formName);
             //             test.GeneralMethods.VerifyTextPresentWebDriver(emailMessage);

             //Delete Form using Store proc method
             // test.SQL.Weblink_Form_Delete_Proc(15, formId);

             //Closing the browser for now since we don't have signout link for this new form Url
             test.Driver.Quit();

         }

        #endregion Upsell

       
        
    } // End of Pricing fixture

    
    //Testcase clean up
    //SP - Making not to run since we already have the delete logic in every tear down and we can use these test cases to clean up the forms not deleted using tear down
    [TestFixture, DoesNotRunOutsideOfStaging, DoesNotRunInStaging]
    //[TestFixture]
    class infellowship_EventRegistration_Form_Cleanup_WebDriver : FixtureBaseWebDriver
    {
        #region Clean up Test Cases

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description(" Form Name clean up in Portal ")]
        public void InfellowshipRegistration_CleanUp_FormNames_TextBox()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // declaring fromId collection
            IList<int> formIDList = new List<int>(200);



            //login to Portal
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");
            // Navigate to weblink->manage forms
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.WebLink.Event_Registration.Manage_Forms);

            //To select form

            test.Driver.FindElementByLinkText("T").Click();
            IWebElement table = test.Driver.FindElementById(TableIds.Portal.WebLink_ManageForms);

            IList<string> testFormName = new List<string>();


            //  IList<IWebElement> formNameList = table.FindElements(By.TagName("tr"))[*].FindElements(By.TagName("td"))[0]);

            // If there are multiple pages...
            if (test.Driver.FindElementsByXPath("//div[@class='grid_controls']/ul/li[1]/a[text()='1']").Count > 0)
            {
                for (int pageIndex = 1; pageIndex < test.Driver.FindElementsByXPath("//div[@class='grid_controls']/ul/li").Count; pageIndex++)
                {
                    test.Driver.FindElementByXPath(string.Format("//div[@class='grid_controls']/ul/li[*]/a[text()='{0}']", pageIndex)).Click();

                    IList<IWebElement> formNameList = test.Driver.FindElements(By.XPath("//table[@id='ctl00_ctl00_MainContent_content_dgForms']/tbody/tr/td/a"));
                    //Getting Test form Name to delete

                    for (int j = 0; j < formNameList.Count; j++)
                    {
                        if ((formNameList[j].Text).StartsWith("Test Form - QATextbox"))
                        {

                            testFormName.Add(formNameList[j].Text);

                        }
                    }

                }
            }

            //Adding form ids
            for (int k = 0; k < testFormName.Count; k++)
            {
                TestLog.WriteLine("Form Name to delete = {0}", testFormName[k]);
                int fID = test.SQL.Weblink_InfellowshipForm_GetFormId(15, testFormName[k]);

                TestLog.WriteLine("Form ID = {0}", fID);

                formIDList.Add(fID);
            }


            for (int i = 0; i < formIDList.Count; i++)
            {
                TestLog.WriteLine("Form ID = {0}", formIDList[i]);
                base.SQL.Weblink_Form_Delete_Proc(15, formIDList[i]);
            }

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description(" Form Name clean up in Portal ")]
        public void InfellowshipRegistration_CleanUp_FormNames_Question()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // declaring fromId collection
            IList<int> formIDList = new List<int>(200);



            //login to Portal
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");
            // Navigate to weblink->manage forms
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.WebLink.Event_Registration.Manage_Forms);

            //To select form

            test.Driver.FindElementByLinkText("T").Click();
            IWebElement table = test.Driver.FindElementById(TableIds.Portal.WebLink_ManageForms);

            IList<string> testFormName = new List<string>();


            //  IList<IWebElement> formNameList = table.FindElements(By.TagName("tr"))[*].FindElements(By.TagName("td"))[0]);

            // If there are multiple pages...
            if (test.Driver.FindElementsByXPath("//div[@class='grid_controls']/ul/li[1]/a[text()='1']").Count > 0)
            {
                for (int pageIndex = 1; pageIndex < test.Driver.FindElementsByXPath("//div[@class='grid_controls']/ul/li").Count; pageIndex++)
                {
                    test.Driver.FindElementByXPath(string.Format("//div[@class='grid_controls']/ul/li[*]/a[text()='{0}']", pageIndex)).Click();

                    IList<IWebElement> formNameList = test.Driver.FindElements(By.XPath("//table[@id='ctl00_ctl00_MainContent_content_dgForms']/tbody/tr/td/a"));
                    //Getting Test form Name to delete

                    for (int j = 0; j < formNameList.Count; j++)
                    {
                        if ((formNameList[j].Text).StartsWith("Test Form - Shared Question"))
                        {

                            testFormName.Add(formNameList[j].Text);

                        }
                    }

                }
            }

            //Adding form ids
            for (int k = 0; k < testFormName.Count; k++)
            {
                TestLog.WriteLine("Form Name to delete = {0}", testFormName[k]);
                int fID = test.SQL.Weblink_InfellowshipForm_GetFormId(15, testFormName[k]);

                TestLog.WriteLine("Form ID = {0}", fID);

                formIDList.Add(fID);
            }


            for (int i = 0; i < formIDList.Count; i++)
            {
                TestLog.WriteLine("Form ID = {0}", formIDList[i]);
                base.SQL.Weblink_Form_Delete_Proc(15, formIDList[i]);
            }

        }

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description(" Form Name clean up in Portal ")]
        public void InfellowshipRegistration_CleanUp_FormNames_Pricing()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // declaring fromId collection
            IList<int> formIDList = new List<int>(200);



            //login to Portal
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");
            // Navigate to weblink->manage forms
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.WebLink.Event_Registration.Manage_Forms);

            //To select form

            test.Driver.FindElementByLinkText("T").Click();
            IWebElement table = test.Driver.FindElementById(TableIds.Portal.WebLink_ManageForms);

            IList<string> testFormName = new List<string>();


            //  IList<IWebElement> formNameList = table.FindElements(By.TagName("tr"))[*].FindElements(By.TagName("td"))[0]);

            // If there are multiple pages...
            if (test.Driver.FindElementsByXPath("//div[@class='grid_controls']/ul/li[1]/a[text()='1']").Count > 0)
            {
                for (int pageIndex = 1; pageIndex < test.Driver.FindElementsByXPath("//div[@class='grid_controls']/ul/li").Count; pageIndex++)
                {
                    test.Driver.FindElementByXPath(string.Format("//div[@class='grid_controls']/ul/li[*]/a[text()='{0}']", pageIndex)).Click();

                    IList<IWebElement> formNameList = test.Driver.FindElements(By.XPath("//table[@id='ctl00_ctl00_MainContent_content_dgForms']/tbody/tr/td/a"));
                    //Getting Test form Name to delete

                    for (int j = 0; j < formNameList.Count; j++)
                    {
                        if ((formNameList[j].Text).StartsWith("Test Form - Pricing"))
                        {

                            testFormName.Add(formNameList[j].Text);

                        }
                    }

                }
            }

            //Adding form ids
            for (int k = 0; k < testFormName.Count; k++)
            {
                TestLog.WriteLine("Form Name to delete = {0}", testFormName[k]);
                int fID = test.SQL.Weblink_InfellowshipForm_GetFormId(15, testFormName[k]);

                TestLog.WriteLine("Form ID = {0}", fID);

                formIDList.Add(fID);
            }


            for (int i = 0; i < formIDList.Count; i++)
            {
                TestLog.WriteLine("Form ID = {0}", formIDList[i]);
                base.SQL.Weblink_Form_Delete_Proc(15, formIDList[i]);
            }

        }


        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description(" Form Name clean up in Portal ")]
        public void InfellowshipRegistration_CleanUp_FormNames_SelectOrder()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // declaring fromId collection
            IList<int> formIDList = new List<int>(200);



            //login to Portal
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");
            // Navigate to weblink->manage forms
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.WebLink.Event_Registration.Manage_Forms);

            //To select form

            test.Driver.FindElementByLinkText("T").Click();
            IWebElement table = test.Driver.FindElementById(TableIds.Portal.WebLink_ManageForms);

            IList<string> testFormName = new List<string>();


            //  IList<IWebElement> formNameList = table.FindElements(By.TagName("tr"))[*].FindElements(By.TagName("td"))[0]);

            // If there are multiple pages...
            if (test.Driver.FindElementsByXPath("//div[@class='grid_controls']/ul/li[1]/a[text()='1']").Count > 0)
            {
                for (int pageIndex = 1; pageIndex < test.Driver.FindElementsByXPath("//div[@class='grid_controls']/ul/li").Count; pageIndex++)
                {
                    test.Driver.FindElementByXPath(string.Format("//div[@class='grid_controls']/ul/li[*]/a[text()='{0}']", pageIndex)).Click();

                    IList<IWebElement> formNameList = test.Driver.FindElements(By.XPath("//table[@id='ctl00_ctl00_MainContent_content_dgForms']/tbody/tr/td/a"));
                    //Getting Test form Name to delete

                    for (int j = 0; j < formNameList.Count; j++)
                    {
                        if ((formNameList[j].Text).StartsWith("Test Form - SelectOrder"))
                        {
                            testFormName.Add(formNameList[j].Text);
                        }
                    }

                }
            }

            //Adding form ids
            for (int k = 0; k < testFormName.Count; k++)
            {
                TestLog.WriteLine("Form Name to delete = {0}", testFormName[k]);
                int fID = test.SQL.Weblink_InfellowshipForm_GetFormId(15, testFormName[k]);

                TestLog.WriteLine("Form ID = {0}", fID);

                formIDList.Add(fID);
            }


            for (int i = 0; i < formIDList.Count; i++)
            {
                TestLog.WriteLine("Form ID = {0}", formIDList[i]);
                base.SQL.Weblink_Form_Delete_Proc(15, formIDList[i]);
            }

        }

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description(" Form Name clean up in Portal ")]
        public void InfellowshipRegistration_CleanUp_FormNames_ConfirmationMessage()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // declaring fromId collection
            IList<int> formIDList = new List<int>(200);

            //login to Portal
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "DC");
            // Navigate to weblink->manage forms
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.WebLink.Event_Registration.Manage_Forms);

            //To select form

            test.Driver.FindElementByLinkText("T").Click();
            IWebElement table = test.Driver.FindElementById(TableIds.Portal.WebLink_ManageForms);

            IList<string> testFormName = new List<string>();


            //  IList<IWebElement> formNameList = table.FindElements(By.TagName("tr"))[*].FindElements(By.TagName("td"))[0]);

            // If there are multiple pages...
            if (test.Driver.FindElementsByXPath("//div[@class='grid_controls']/ul/li[1]/a[text()='1']").Count > 0)
            {
                for (int pageIndex = 1; pageIndex < test.Driver.FindElementsByXPath("//div[@class='grid_controls']/ul/li").Count; pageIndex++)
                {
                    test.Driver.FindElementByXPath(string.Format("//div[@class='grid_controls']/ul/li[*]/a[text()='{0}']", pageIndex)).Click();

                    IList<IWebElement> formNameList = test.Driver.FindElements(By.XPath("//table[@id='ctl00_ctl00_MainContent_content_dgForms']/tbody/tr/td/a"));
                    //Getting Test form Name to delete

                    for (int j = 0; j < formNameList.Count; j++)
                    {
                        if ((formNameList[j].Text).StartsWith("Test Form - ConfirmationMessage"))
                        {
                            testFormName.Add(formNameList[j].Text);
                        }
                    }

                }
            }

            //Adding form ids
            for (int k = 0; k < testFormName.Count; k++)
            {
                TestLog.WriteLine("Form Name to delete = {0}", testFormName[k]);
                int fID = test.SQL.Weblink_InfellowshipForm_GetFormId(15, testFormName[k]);

                TestLog.WriteLine("Form ID = {0}", fID);

                formIDList.Add(fID);
            }


            for (int i = 0; i < formIDList.Count; i++)
            {
                TestLog.WriteLine("Form ID = {0}", formIDList[i]);
                base.SQL.Weblink_Form_Delete_Proc(15, formIDList[i]);
            }

        }

        #endregion Clean up
    }    //end of clean up fixture

    //Payment Test cases

    //SP - Making these tests not run since Payments are not ready to be tested
    [TestFixture]
    [Category(TestCategories.Services.PaymentProcessor)]
    class infellowship_EventRegistration_Payments_WebDriver : FixtureBaseWebDriver
    {

        public void FixtureSetUp()
        {
        }

        [FixtureTearDown]
        public void FixtureTearDown()
        {

        }

        #region Payments
        #region CC Successful transactions

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Success_Visa()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Form Cost Only";
            double price = 5.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process VISA credit card 
            try
            {
                test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", null);
            }
            catch (System.Exception e)
            {
                //If have been logged out, let's try one more time
                if (test.GeneralMethods.IsElementPresentWebDriver(By.Id("username")))
                {
                    test.Infellowship.ReLoginWebDriver("F1AutomatedTester01@gmail.com", "FT4life!", "15");
                }

            }

            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Success messge verification.......

            //Get confirmation mesasge
            string url = test.Driver.Url;
            string confirmcode = test.Infellowship.GetConfirmationCode(url, 15);
            TestLog.WriteLine("Confirm Code = {0}", confirmcode);

            //Verify confirmation code in confirmation screen
            test.GeneralMethods.VerifyTextPresentWebDriver(confirmcode);


            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();


        }

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Success_MasterCard()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Form Cost Only";
            double price = 5.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process Master card 
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Master Card", "5555555555554444", "12 - December", "2020", null);


            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Success messge verification.......

            //Get confirmation mesasge
            string url = test.Driver.Url;
            string confirmcode = test.Infellowship.GetConfirmationCode(url, 15);
            TestLog.WriteLine("Confirm Code = {0}", confirmcode);

            //Verify confirmation code in confirmation screen
            test.GeneralMethods.VerifyTextPresentWebDriver(confirmcode);


            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();
        }


        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Success_AmericanExpress()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Form Cost Only";
            double price = 5.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process Credit Card details
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "American Express", "378282246310005", "12 - December", "2020", null);


            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Success messge verification.......

            //Get confirmation mesasge
            string url = test.Driver.Url;
            string confirmcode = test.Infellowship.GetConfirmationCode(url, 15);
            TestLog.WriteLine("Confirm Code = {0}", confirmcode);

            //Verify confirmation code in confirmation screen
            test.GeneralMethods.VerifyTextPresentWebDriver(confirmcode);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Success_Discover()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Form Cost Only";
            double price = 5.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process Credit Card details
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Discover", "6011111111111117", "12 - December", "2020", null);


            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Success messge verification.......

            //Get confirmation mesasge
            string url = test.Driver.Url;
            string confirmcode = test.Infellowship.GetConfirmationCode(url, 15);
            TestLog.WriteLine("Confirm Code = {0}", confirmcode);

            //Verify confirmation code in confirmation screen
            test.GeneralMethods.VerifyTextPresentWebDriver(confirmcode);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }


        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Success_JCB()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            int churchId = 258;

            // Store the currency format
            System.Globalization.CultureInfo culture = null;
            if (churchId == 258)
            {
                culture = new CultureInfo("en-GB");
            }
            else
            {
                culture = new CultureInfo("en-US");
            }


            // Set initial conditions


            string formName = "A Test Form - Form Cost Only";
            double price = 5.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 258, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
           // Assert.AreEqual(string.Format("{0:C}", price), test.Driver.FindElementById("total_price").Text);
            Assert.AreEqual(price.ToString("C", culture), test.Driver.FindElementById("total_price").Text);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process Credit Card details
            test.Infellowship.EventRegistration_CC_Process_European(258, "FT", "Tester", "JCB", "3566111111111113", "12 - December", "2020", null);


            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Success messge verification.......

            //Get confirmation mesasge
            string url = test.Driver.Url;
            string confirmcode = test.Infellowship.GetConfirmationCode(url, 258);
            TestLog.WriteLine("Confirm Code = {0}", confirmcode);

            //Verify confirmation code in confirmation screen
            test.GeneralMethods.VerifyTextPresentWebDriver(confirmcode);

            //test.GeneralMethods.VerifyTextPresentWebDriver(formName);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }
        #endregion CC Successful transactions

        #region Saved Credit card transactions
        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Success_Saved_CreditCard()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            // Set initial conditions


            string formName = "A Test Form - Form Cost Only";
            double price = 5.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process VISA credit card 
            //test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", null);

            //Commenting this to figureout a way to  select specified card.....
            //Selecting existing Visa saved payment card for processing..
           // test.Driver.FindElementByClassName("payment visa").Click();

            //Since by default the first card on the list is selected the payments will process using that information

            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Success messge verification.......

            //Get confirmation mesasge
            string url = test.Driver.Url;
            string confirmcode = test.Infellowship.GetConfirmationCode(url, 15);
            TestLog.WriteLine("Confirm Code = {0}", confirmcode);

            //Verify confirmation code in confirmation screen
            test.GeneralMethods.VerifyTextPresentWebDriver(confirmcode);


            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();


        }
        #endregion

        #region CC Error validations

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Missing_FirstName()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Form Cost Only";
            double price = 5.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process Credit Card details
            test.Infellowship.EventRegistration_CC_Process(15, string.Empty, "Tester", "Discover", "6011111111111117", "12 - December", "2020", null);




            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Error message .......
            test.GeneralMethods.VerifyTextPresentWebDriver("First Name is required");

            //Verify Form Name and Confirmation message created are displayed in confirmation page
            // test.GeneralMethods.VerifyTextPresentWebDriver(formName);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();


        }

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Missing_LastName()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Form Cost Only";
            double price = 5.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process Credit Card details
            test.Infellowship.EventRegistration_CC_Process(15, "FT", string.Empty, "Discover", "6011111111111117", "12 - December", "2020", null);




            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Error message .......
            test.GeneralMethods.VerifyTextPresentWebDriver("Last Name is required");

            //Verify Form Name and Confirmation message created are displayed in confirmation page
            // test.GeneralMethods.VerifyTextPresentWebDriver(formName);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();


        }

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Missing_CardType()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Form Cost Only";
            double price = 5.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process Credit Card details
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", string.Empty, "6011111111111117", "12 - December", "2020", null);

            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Error message .......
            test.GeneralMethods.VerifyTextPresentWebDriver("Card Type is required");

            //Verify Form Name and Confirmation message created are displayed in confirmation page
            // test.GeneralMethods.VerifyTextPresentWebDriver(formName);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();


        }

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Missing_CardNumber()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Form Cost Only";
            double price = 5.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process Credit Card details
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Discover", string.Empty, "12 - December", "2020", null);




            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Error message .......
            test.GeneralMethods.VerifyTextPresentWebDriver("Card Number is required");

            //Verify Form Name and Confirmation message created are displayed in confirmation page
            // test.GeneralMethods.VerifyTextPresentWebDriver(formName);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Missing_ExpirationMonth()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Form Cost Only";
            double price = 5.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process Credit Card details
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Discover", "6011111111111117", string.Empty, "2020", null);




            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Error message .......
            test.GeneralMethods.VerifyTextPresentWebDriver("Expiration Month is required");

            //Verify Form Name and Confirmation message created are displayed in confirmation page
            // test.GeneralMethods.VerifyTextPresentWebDriver(formName);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();


        }

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Missing_ExpirationYear()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Form Cost Only";
            double price = 5.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process Credit Card details
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Discover", "6011111111111117", "12 - December", string.Empty, null);

            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Error message .......
            test.GeneralMethods.VerifyTextPresentWebDriver("Expiration Year is required");

            //Verify Form Name and Confirmation message created are displayed in confirmation page
            // test.GeneralMethods.VerifyTextPresentWebDriver(formName);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }


        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Visa_InvalidCardNumber()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Form Cost Only";
            double price = 5.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process Credit Card details
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Visa", "4111111111111112", "12 - December", "2020", null);




            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Error message .......
            if (base.SQL.IsAMSEnabled(15))
            {
                test.GeneralMethods.VerifyTextPresentWebDriver("The card number is not valid for the type of card selected");
            }

            else
                test.GeneralMethods.VerifyTextPresentWebDriver("The card number is not valid for the type of card selected");

            //Verify Form Name and Confirmation message created are displayed in confirmation page
            // test.GeneralMethods.VerifyTextPresentWebDriver(formName);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();


        }


        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Master_InvalidCardNumber()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Form Cost Only";
            double price = 5.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process Credit Card details
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Master Card", "1555555555554444", "12 - December", "2020", null);




            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Error message .......
            //test.GeneralMethods.VerifyTextPresentWebDriver("The card number is not valid for the type of card selected");
            test.GeneralMethods.VerifyTextPresentWebDriver("Please enter a valid card number");

            //Verify Form Name and Confirmation message created are displayed in confirmation page
            // test.GeneralMethods.VerifyTextPresentWebDriver(formName);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();


        }

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_AmericanExpress_InvalidCardNumber()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Form Cost Only";
            double price = 5.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process Credit Card details
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "American Express", "178282246310005", "12 - December", "2020", null);




            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Error message .......
            test.GeneralMethods.VerifyTextPresentWebDriver("Please enter a valid card number");

            //Verify Form Name and Confirmation message created are displayed in confirmation page
            // test.GeneralMethods.VerifyTextPresentWebDriver(formName);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();


        }

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Discover_InvalidCardNumber()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions

            string formName = "A Test Form - Form Cost Only";
            double price = 5.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();

            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");

            // Process Credit Card details
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Discover", "6811111111111117", "12 - December", "2020", null);

            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Error message .......
            test.GeneralMethods.VerifyTextPresentWebDriver("Please enter a valid card number");

            //Verify Form Name and Confirmation message created are displayed in confirmation page
            // test.GeneralMethods.VerifyTextPresentWebDriver(formName);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_JCB_InvalidCardNumber()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            int churchId = 258;

            // Store the currency format
            System.Globalization.CultureInfo culture = null;
            if (churchId == 258)
            {
                culture = new CultureInfo("en-GB");
            }
            else
            {
                culture = new CultureInfo("en-US");
            }


            // Set initial conditions

            string formName = "A Test Form - Form Cost Only";
            double price = 5.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 258, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            //Assert.AreEqual(string.Format("{0:C}", price), test.Driver.FindElementById("total_price").Text);
            Assert.AreEqual(price.ToString("C", culture), test.Driver.FindElementById("total_price").Text);


            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();

            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");

            // Process Credit Card details
            test.Infellowship.EventRegistration_CC_Process_European(256, "FT", "Tester", "JCB", "2566111111111113", "12 - December", "2020", null);

            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Error message .......
            test.GeneralMethods.VerifyTextPresentWebDriver("Please enter a valid card number");

            //Verify Form Name and Confirmation message created are displayed in confirmation page
            // test.GeneralMethods.VerifyTextPresentWebDriver(formName);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }


        #endregion CC Error validations

        #region CC Error codes

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Error_150()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Error 150";
            double price = 2000.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            // Assert.AreEqual(test.Driver.FindElementById("total_price").Text, string.Format("{0:C}", price), "Total Price value did not match");
            Assert.AreEqual(price.ToString("$0,000.00"), test.Driver.FindElementById("total_price").Text, "Total Price value did not match");


            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process VISA credit card 
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", null);


            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Error Message 

            if (this.SQL.IsAMSEnabled(15))
            {
                test.GeneralMethods.VerifyTextPresentWebDriver("Authorization was not successful. Please check your information including your billing address to make sure they are accurate, or use another card or select another form of payment");
            }
            else
                test.GeneralMethods.VerifyTextPresentWebDriver("Authorization was not successful. Please check your information including your billing address to make sure they are accurate, or use another card or select another form of payment.");


            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Error_201()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Error 201";
            double price = 2402.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(price.ToString("$0,000.00"), test.Driver.FindElementById("total_price").Text, "Total Price value did not match");

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process VISA credit card 
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", null);


            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Error Message 

            if (this.SQL.IsAMSEnabled(15))
            {
                test.GeneralMethods.VerifyTextPresentWebDriver("Authorization was not successful. Please check your information including your billing address to make sure they are accurate, or use another card or select another form of payment");
            }
            else
                test.GeneralMethods.VerifyTextPresentWebDriver("Authorization was not successful. Please check your information including your billing address to make sure they are accurate, or use another card or select another form of payment");


            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }


        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Error_203()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Error 203";
            double price = 2260.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(price.ToString("$0,000.00"), test.Driver.FindElementById("total_price").Text, "Total Price value did not match");

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process VISA credit card 
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", null);


            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Error Message 

            if (this.SQL.IsAMSEnabled(15))
            {
                test.GeneralMethods.VerifyTextPresentWebDriver("Authorization was not successful. Please check your information including your billing address to make sure they are accurate, or use another card or select another form of payment");
            }
            else
                test.GeneralMethods.VerifyTextPresentWebDriver("Authorization was not successful. Please check your information including your billing address to make sure they are accurate, or use another card or select another form of payment");


            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }


        [Test, RepeatOnFailure, Timeout(1000)]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Error_204()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Error 204";
            double price = 2521.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(price.ToString("$0,000.00"), test.Driver.FindElementById("total_price").Text, "Total Price value did not match");

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process VISA credit card 
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", null);


            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Error Message 

            if (this.SQL.IsAMSEnabled(15))
            {
                test.GeneralMethods.VerifyTextPresentWebDriver("Authorization was not successful.  Insufficient funds in the account. Please use a different card or select another form of payment.");
            }
            else
                test.GeneralMethods.VerifyTextPresentWebDriver("Authorization was not successful.  Insufficient funds in the account. Please use a different card or select another form of payment.");


            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }


        [Test, RepeatOnFailure, Timeout(1000)]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Error_205()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Error 205";
            double price = 2501.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(price.ToString("$0,000.00"), test.Driver.FindElementById("total_price").Text, "Total Price value did not match");

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process VISA credit card 
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", null);


            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Error Message 

            if (this.SQL.IsAMSEnabled(15))
            {
                test.GeneralMethods.VerifyTextPresentWebDriver("There is missing or incorrect information. Please complete the form and submit again.\r\n• The credit card number entered is invalid.");
            }
            else
                test.GeneralMethods.VerifyTextPresentWebDriver("Authorization was not successful. Please check your information including your billing address to make sure they are accurate, or use another card or select another form of payment");


            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }

        [Test, RepeatOnFailure, Timeout(1000)]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Error_208()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Error 208";
            double price = 2606.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(price.ToString("$0,000.00"), test.Driver.FindElementById("total_price").Text, "Total Price value did not match");

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process VISA credit card 
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", null);


            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Error Message 

            if (this.SQL.IsAMSEnabled(15))
            {
                test.GeneralMethods.VerifyTextPresentWebDriver("There is missing or incorrect information. Please complete the form and submit again.\r\n• The credit card number entered is invalid.");
            }
            else
                test.GeneralMethods.VerifyTextPresentWebDriver("Authorization was not successful. Please check your information including your billing address to make sure they are accurate, or use another card or select another form of payment");


            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }

        [Test, RepeatOnFailure, Timeout(1000)]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Error_209()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Error 209";
            double price = 2811.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(price.ToString("$0,000.00"), test.Driver.FindElementById("total_price").Text, "Total Price value did not match");

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process VISA credit card 
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", null);


            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Error Message 

            if (this.SQL.IsAMSEnabled(15))
            {
                test.GeneralMethods.VerifyTextPresentWebDriver("There is missing or incorrect information. Please complete the form and submit again.\r\n• The credit card number entered is invalid.");
            }
            else
                test.GeneralMethods.VerifyTextPresentWebDriver("Authorization was not successful. Please check your information including your billing address to make sure they are accurate, or use another card or select another form of payment.");


            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }

        [Test, RepeatOnFailure, Timeout(1000)]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Error_211()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Error 211";
            double price = 2531.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(price.ToString("$0,000.00"), test.Driver.FindElementById("total_price").Text, "Total Price value did not match");

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process VISA credit card 
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", null);


            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Error Message 

            if (this.SQL.IsAMSEnabled(15))
            {
                test.GeneralMethods.VerifyTextPresentWebDriver("There is missing or incorrect information. Please complete the form and submit again.\r\n• The credit card number entered is invalid.");
            }
            else
                test.GeneralMethods.VerifyTextPresentWebDriver("Authorization was not successful. Please check your information including your billing address to make sure they are accurate, or use another card or select another form of payment.");


            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }


        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_Error_233()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Error 233";
            double price = 2204.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(price.ToString("$0,000.00"), test.Driver.FindElementById("total_price").Text, "Total Price value did not match");

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process VISA credit card 
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", null);


            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Error Message 

            if (this.SQL.IsAMSEnabled(15))
            {
                test.GeneralMethods.VerifyTextPresentWebDriver("Authorization was not successful. Please check your information including your billing address to make sure they are accurate, or use another card or select another form of payment");
            }
            else
                test.GeneralMethods.VerifyTextPresentWebDriver("Authorization was not successful. Please check your information including your billing address to make sure they are accurate, or use another card or select another form of payment");


            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }

        #endregion CC Error validations

        #region CC AMS Error validations

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_AMS_Error_211()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - AMS 211";
            double price = 2.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            if (this.SQL.IsAMSEnabled(15))
            {
                //Login to Event Registration Form
                test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

                //select individual
                test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

                //Verify Total price in step2
                Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

                //Click Continue
                test.Infellowship.EventRegistration_ClickContinue();


                //Verify that we are in Step3
                test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


                // Process VISA credit card 
                test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", null);


                //Submitting details from Step3..
                test.Infellowship.EventRegistration_ClickSubmitPayment();

                //Error Message 


                //test.GeneralMethods.VerifyTextPresentWebDriver("Authorization was not successful.Please use a different card or select another form of payment.");

                test.GeneralMethods.VerifyTextPresentWebDriver("Authorization was not successful. Please check your information including your billing address to make sure they are accurate, or use another card or select another form of payment.");
            }



            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_CreditCard_AMS_Error_230()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            // Set initial conditions


            string formName = "A Test Form - AMS 230";
            double price = 5.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            if (this.SQL.IsAMSEnabled(15))
            {
                //Login to Event Registration Form
                test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

                //select individual
                test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

                //Verify Total price in step2
                Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

                //Click Continue
                test.Infellowship.EventRegistration_ClickContinue();


                //Verify that we are in Step3
                test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


                // Process VISA credit card 
                test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", null);


                //Submitting details from Step3..
                test.Infellowship.EventRegistration_ClickSubmitPayment();

                //Error Message 


                test.GeneralMethods.VerifyTextPresentWebDriver("Authorization was not successful. Please check your information including your billing address to make sure they are accurate, or use another card or select another form of payment.");
            }



            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }

        #endregion CC AMS Error validations

        #region E-Check successful transactions

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_Echeck_Success()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions

            string formName = "A Test Form - Form Cost Only";
            double price = 5.00;


            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process E-check
            test.Infellowship.EventRegistration_Echeck_Process("FT", "Tester", "972-972-9999", "111000025", "111222333");


            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Success messge verification.......

            //Get confirmation mesasge
            string url = test.Driver.Url;
            string confirmcode = test.Infellowship.GetConfirmationCode(url, 15);
            TestLog.WriteLine("Confirm Code = {0}", confirmcode);

            //Verify confirmation code in confirmation screen
            test.GeneralMethods.VerifyTextPresentWebDriver(confirmcode);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }

        [Test, RepeatOnFailure]
        [Author("Daniel Lee")]
        [Description("FO-2942 - ACH payment type for F1")]
        public void InfellowshipRegistration_Echeck_Verify_Message()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions

            string formName = "A Test Form - Form Cost Only";
            double price = 5.00;


            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");

            test.Driver.FindElementById("payment_method_echeck").Click();

            //Verify ACH message is correct
            String format = "MMMM dd, yyyy";
            string amount = String.Format("{0:N2}", Convert.ToDouble(price));
            string actualMsg = test.Driver.FindElementById("echeck_auth_message").Text.ToString();
            string expectMsg = "By clicking the button below, I authorize Active Network, LLC to charge my bank account on " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString(format, DateTimeFormatInfo.InvariantInfo) + " for the amount of $" + amount + " for " + formName + ".";
            Assert.AreEqual(expectMsg, actualMsg);

            test.Driver.Quit();

        }

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_Echeck_Success_WithFundAndSubFund()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Form Cost Only - SubFund";
            double price = 5.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process E-check
            test.Infellowship.EventRegistration_Echeck_Process("FT", "Tester", "972-972-9999", "111000025", "111222333");


            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Success messge verification.......

            //Get confirmation mesasge
            string url = test.Driver.Url;
            string confirmcode = test.Infellowship.GetConfirmationCode(url, 15);
            TestLog.WriteLine("Confirm Code = {0}", confirmcode);

            //Verify confirmation code in confirmation screen
            test.GeneralMethods.VerifyTextPresentWebDriver(confirmcode);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_Echeck_Success_SimulatedFund()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions

            if (this.AMSEnabled)
            {
                string formName = "A Test Form - Form Cost Only-SimulatedFund";
                double price = 10.00;

                string[] individual = { "Automated Tester" };
                IList<string> selectedOrder = new List<string>();

                //Adding individuals to selected order
                for (int i = 0; i < individual.Length; i++)
                {
                    selectedOrder.Add(individual[i]);

                }

                //Login to Event Registration Form
                test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

                //select individual
                test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

                //Verify Total price in step2
                Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

                //Click Continue
                test.Infellowship.EventRegistration_ClickContinue();


                //Verify that we are in Step3
                test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


                // Process E-check
                test.Infellowship.EventRegistration_Echeck_Process("FT", "Tester", "972-972-9999", "111000025", "111222333");


                //Submitting details from Step3..
                test.Infellowship.EventRegistration_ClickSubmitPayment();

                //Success messge verification.......

                //Get confirmation mesasge
                string url = test.Driver.Url;
                string confirmcode = test.Infellowship.GetConfirmationCode(url, 15);
                TestLog.WriteLine("Confirm Code = {0}", confirmcode);

                //Verify confirmation code in confirmation screen
                test.GeneralMethods.VerifyTextPresentWebDriver(confirmcode);

                //Delete Form using Store proc method
                // test.SQL.Weblink_Form_Delete_Proc(15, formId);

                //Closing the browser for now since we don't have signout link for this new form Url
                test.Driver.Quit();
            }

        }
        #endregion E-Check successful transactions

        #region E-Check Error validations

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_Echeck_Missing_RequiredFields()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Form Cost Only";
            double price = 5.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process E-check
            test.Infellowship.EventRegistration_Echeck_Process(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);



            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Verify Error message

            test.GeneralMethods.VerifyTextPresentWebDriver("First Name is required");
            test.GeneralMethods.VerifyTextPresentWebDriver("Last Name is required");
            test.GeneralMethods.VerifyTextPresentWebDriver("Bank Routing Number is required");
            test.GeneralMethods.VerifyTextPresentWebDriver("Account Number is required");

            //Assert.AreEqual("First Name is required", test.Driver.FindElementByXPath("//*[@id='echeck_details']/fieldset/div/div/label").Text, "Error not found");
            //Assert.AreEqual("Last Name is required", test.Driver.FindElementByXPath("//*[@id='echeck_details']/fieldset/div/div/div/label").Text, "Error not found");
            //Assert.AreEqual("Bank Routing Number is required", test.Driver.FindElementByXPath("//*[@id='echeck_details']/fieldset/div/div/div/label").Text, "Error not found");
            //Assert.AreEqual("Account Number is required", test.Driver.FindElementByXPath("//*[@id='echeck_details']/fieldset/div/div/div/div/label").Text, "Error not found");             


            //Verify Form Name and Confirmation message created are displayed in confirmation page
            test.GeneralMethods.VerifyTextPresentWebDriver(formName);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }


        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_Echeck_Exceeding_Character_limits()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Form Cost Only";
            double price = 5.00;

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Verify Total price in step2
            Assert.AreEqual(string.Format("{0:C2}", price), test.Driver.FindElementById("total_price").Text);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");


            // Process E-check
            test.Infellowship.EventRegistration_Echeck_Process("FT", "Tester", "972-972-9999", "1234567890", "123456789012345678901234567890123456789012345678901234");

            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Verify Error message

            //Verify Form Name and Confirmation message created are displayed in confirmation page
            test.GeneralMethods.VerifyTextPresentWebDriver(formName);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }

        #endregion E-Check Error valdations

        #region Submissions


        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_Submission_EmailReceipt_MultipleRegistrants()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Email Receipt";
            double price = 10.00;

            string[] individual = { "Automated Tester", "Qa Tester" };

            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");

            // Process VISA credit card 
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", null);

            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Verify Form Name and Confirmation message created are displayed in confirmation page
            test.GeneralMethods.VerifyTextPresentWebDriver(formName);

            //Get confirmation mesasge
            string url = test.Driver.Url;
            string confirmcode = test.Infellowship.GetConfirmationCode(url, 15);
            TestLog.WriteLine("Confirm Code = {0}", confirmcode);


            //Login in to Portal
            test.Portal.LoginWebDriver();

            //Verify confirmation number in email receipt in Portal
            test.Portal.Verify_EventRegistration_Confirmation_Email_Receipt(confirmcode, price, individual.Length);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_Submission_Refund_CreditCard()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Email Receipt";
            double price = 5.00;

            string[] individual = { "Automated Tester" };

            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");

            // Process VISA credit card 
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", null);


            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Verify Form Name and Confirmation message created are displayed in confirmation page
            test.GeneralMethods.VerifyTextPresentWebDriver(formName);

            //Get confirmation mesasge
            string url = test.Driver.Url;
            string confirmcode = test.Infellowship.GetConfirmationCode(url, 15);
            TestLog.WriteLine("Confirm Code = {0}", confirmcode);


            //Login in to Portal
            test.Portal.LoginWebDriver();

            //Verify confirmation number in email receipt in Portal
            test.Portal.Verify_EventRegistration_Submission_Refund(confirmcode, price);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_Submission_Refund_ECheck()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Email Receipt";
            double price = 5.00;

            string[] individual = { "Automated Tester" };

            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");

            // Process E-check
            test.Infellowship.EventRegistration_Echeck_Process("FT", "Tester", "972-972-9999", "111000025", "111222333");


            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Verify Form Name and Confirmation message created are displayed in confirmation page
            test.GeneralMethods.VerifyTextPresentWebDriver(formName);

            //Get confirmation mesasge
            string url = test.Driver.Url;
            string confirmcode = test.Infellowship.GetConfirmationCode(url, 15);
            TestLog.WriteLine("Confirm Code = {0}", confirmcode);


            //Login in to Portal
            test.Portal.LoginWebDriver();

            //Verify confirmation number in email receipt in Portal
            test.Portal.Verify_EventRegistration_Submission_Refund(confirmcode, price);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }

        //commenting until figure out a way to click and hold works for schedule selection... 
        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4235 - Reg 2: Epic 4 - Support Payments on Step 3")]
        public void InfellowshipRegistration_Submission_WithActivitySelection()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];


            // Set initial conditions


            string formName = "A Test Form - Activity Schedule";
            double price = 2.00;

            string[] individual = { "Automated Tester" };

            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }


            //Login
            test.Portal.LoginWebDriver();


            //Create New Form
            test.Portal.EventRegistration_Select_FormName_FromTable(formName);
                       
                                             
            //Select Current  date for scedule
            test.Portal.EventRegistration_Select_Schedule_CurrentDate(formName);

            //Logout from Portal
            test.Portal.LogoutWebDriver();

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");

            // Process VISA credit card 
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", null);

            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Verify Form Name and Confirmation message created are displayed in confirmation page
            test.GeneralMethods.VerifyTextPresentWebDriver(formName);

            //Get confirmation mesasge
            string url = test.Driver.Url;
            string confirmcode = test.Infellowship.GetConfirmationCode(url, 15);
            TestLog.WriteLine("Confirm Code = {0}", confirmcode);


            //Login in to Portal to verfify submission
            test.Portal.LoginWebDriver("ft.tester","FT4life!", "DC");

            //Verify confirmation number in email receipt in Portal
            test.Portal.Verify_EventRegistration_Submission_Refund(confirmcode, price);

            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }

        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("F1-4237 - Reg 2: Epic 4 - Implement Step 2 Upsell page")]
        public void InfellowshipRegistration_Submission_MultipleIndividual_Upsell_Price()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string formName = "A Test Form - Submission Upsell";
            double price = 16.22;

            string[] individual = { "Automated Tester", "Qa Tester" };


            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");

            // Process VISA credit card 
            test.Infellowship.EventRegistration_CC_Process(15, "FT", "Tester", "Visa", "4111111111111111", "12 - December", "2020", null);

            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Verify Form Name and Confirmation message created are displayed in confirmation page
            test.GeneralMethods.VerifyTextPresentWebDriver(formName);

            //Get confirmation mesasge
            string url = test.Driver.Url;
            string confirmcode = test.Infellowship.GetConfirmationCode(url, 15);
            TestLog.WriteLine("Confirm Code = {0}", confirmcode);


            //Login in to Portal
            test.Portal.LoginWebDriver();

            //Verify confirmation number in email receipt in Portal
            test.Portal.Verify_EventRegistration_Confirmation_Email_Receipt(confirmcode, price, individual.Length);


            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }

        //This test case is to submit a zero dollar form with a user with no payment history
        [Test, RepeatOnFailure]
        [Author("Suchitra Patnam")]
        [Description("Reg 2: Epic 4 - Submitting non payment Form with a user no payment information")]
        public void InfellowshipRegistration_Submission_WithNonPaymentInfo()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string formName = "A Test Form - Non Payment";
            double price = 0.0;

            string[] individual = { "Automated Tester"};


            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester02@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Click Continue
            test.Infellowship.EventRegistration_ClickContinue();


            //Verify that we are in Step3
            test.GeneralMethods.VerifyTextPresentWebDriver("Finalize Registration");

            //Submitting details from Step3..
            test.Infellowship.EventRegistration_ClickSubmitPayment();

            //Verify Form Name and Confirmation message created are displayed in confirmation page
            test.GeneralMethods.VerifyTextPresentWebDriver(formName);

            //Get confirmation mesasge
            string url = test.Driver.Url;
            string confirmcode = test.Infellowship.GetConfirmationCode(url, 15);
            TestLog.WriteLine("Confirm Code = {0}", confirmcode);


            //Verify confirmation code in confirmation screen
            test.GeneralMethods.VerifyTextPresentWebDriver(confirmcode);


            //Delete Form using Store proc method
            // test.SQL.Weblink_Form_Delete_Proc(15, formId);

            //Closing the browser for now since we don't have signout link for this new form Url
            test.Driver.Quit();

        }

    }
        #endregion Submissions
    

#endregion Payments
    
    [TestFixture]
    class infellowship_EventRegistration_FormRestrictions_WebDriver : FixtureBaseWebDriver
    {
        // declaring fromId collection
        IList<int> _formId = new List<int>();             

        public void FixtureSetUp()
        {      
            
        }

        [FixtureTearDown]
        public void FixtureTearDown()
        {


        }
        #region Form Restrictions
        //[Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies Infellowship Registration is full")]
        public void Infellowship_Registration_MaxSubmissions()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string formName = "Submission Restriction Form -1";
            string username = "stuplatttest2390@gmail.com";
            string password = "Romans10:9";
            string secondUser = "stuplatttest@gmail.com";
            IList<string> selectedOrder = new List<string>();
            string individualName = "Stuart Platt";

            selectedOrder.Add(individualName);


            //commenting this line since it is throwing error
            test.SQL.Weblink_ExpireFormSubmissions_Proc(1);
            test.SQL.WebLink_FormNames_Delete(1, 15, formName);

            //Login portal
            test.Portal.LoginWebDriver();

            //Create Weblink form with restrictions
            test.Portal.WebLink_FormNames_Create_WithRestrictions(formName, true, "1", null, null, null, null, null, null);

            //Get form link
            string formLink = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, 15);

            //Logout portal
            test.Portal.LogoutWebDriver();

            //Login infellowship
            test.Infellowship.LoginWebDriver(username, password);

            //Navigate to form
            test.GeneralMethods.OpenURLWebDriver(formLink);
            test.GeneralMethods.WaitForElement(By.Id(GeneralInFellowship.EventRegistration.FormName));

            test.Infellowship.EventRegistration_Select_Individual(new string[] { individualName }, selectedOrder);
            test.GeneralMethods.WaitForElement(By.Id("continue"));

            //Go Home
            test.Driver.FindElementByLinkText("Dynamic Church").Click();
            test.GeneralMethods.WaitForElement(By.LinkText("HOME"));

            //Logout User 1
            test.Infellowship.LogoutWebDriver();

            //Login infellowship
            test.Infellowship.LoginWebDriver(secondUser, password);

            //navigate to form
            test.GeneralMethods.OpenURLWebDriver(formLink);

            //Verify the user is unable to complete the form
            Assert.AreEqual(formName, test.Driver.FindElementById(GeneralInFellowship.EventRegistration.FormName).Text, "The form name does not match");
            TestLog.WriteLine("This is the header text - {0}", test.Driver.FindElementById("page_header").Text);
            TestLog.WriteLine("This is at the jumbo - {0}", test.Driver.FindElementByClassName("jumbo").Text);
            Assert.Contains(test.Driver.FindElementByClassName("jumbo").Text, "Registration Unavailable");
            Assert.Contains(test.Driver.FindElementByClassName("jumbo").Text, "We apologize, but this form is currently unavailable. Please contact us directly for further assistance.");
            Assert.Contains(test.Driver.FindElementByClassName("jumbo").Text, "Not enough available slots... capacity is currently 1/1.");
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("Return to Homepage"));

            test.Driver.FindElementByLinkText("Return to Homepage").Click();
            test.GeneralMethods.WaitForElement(By.LinkText("Update Profile"));

            //Logout infellowship
            test.Infellowship.LogoutWebDriver();

            //Delete Form
            // test.SQL.Weblink_ExpireFormSubmissions_Proc(1);
            test.SQL.WebLink_FormNames_Delete(1, 15, formName);

        }


        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies the Active/InActive restriction for infellowship forms")]
        public void Infellowship_Registration_InactiveForm()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string formName = "Inactive Restriction Form";
            string userName = "stuplatttest2390@gmail.com";
            string password = "Romans10:9";

            test.SQL.WebLink_FormNames_Delete(1, 15, formName);

            //Login Portal
            test.Portal.LoginWebDriver();

            //Create Form
            test.Portal.WebLink_FormNames_Create(formName, false);
            
            //Get form link
            string formLink = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, 15);

            //Logout Portal
            test.Portal.LogoutWebDriver();

            //Login Infellowship
            test.Infellowship.LoginWebDriver(userName, password);

            //Navigate to form 
            test.GeneralMethods.OpenURLWebDriver(formLink);
            test.GeneralMethods.WaitForElement(By.Id(GeneralInFellowship.EventRegistration.FormName));

            //Verify the user is unable to complete the form
            Assert.AreEqual(formName, test.Driver.FindElementById(GeneralInFellowship.EventRegistration.FormName).Text, "The form name does not match");
            TestLog.WriteLine("This is the header text - {0}", test.Driver.FindElementById("page_header").Text);
            /*
            // Commented out by Mady and replace it with code below to check the page is display as expected
            TestLog.WriteLine("This is at the jumbo - {0}", test.Driver.FindElementByClassName("jumbo").Text);
            Assert.Contains(test.Driver.FindElementByClassName("jumbo").Text, "Registration Unavailable");
            Assert.Contains(test.Driver.FindElementByClassName("jumbo").Text, "We apologize, but this form is currently unavailable. Please contact us directly for further assistance.");
            Assert.Contains(test.Driver.FindElementByClassName("jumbo").Text, "Form is inactive.");
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("Return to Homepage"));

            test.Driver.FindElementByLinkText("Return to Homepage").Click();
            */

            Assert.AreEqual("Registration Closed", test.Driver.FindElementByClassName("errorMes").Text, "The error message is wrong");
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("HOME"));

            test.Driver.FindElementByLinkText("HOME").Click();
            test.GeneralMethods.WaitForElement(By.LinkText("Update Profile"));

            //Logout Infellowship
            test.Infellowship.LogoutWebDriver();

            //Delete Form
            test.SQL.WebLink_FormNames_Delete(1, 15, formName);
        }

        [Test, RepeatOnFailure]
        [Author("Michelle Zheng")]
        [Description("FO-1731- Verifies the Home button after successful registration")]
        public void Infellowship_Registration_HomeButton()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string formName = "Spring TT test02";
            int churchId = 15;
            string userName = "ft.autotester@gmail.com";
            string password = "FT4life!";
            string[] individual = { "Ft Tester" };
            string ministry = "A Test Ministry";
            string activity = "A Test Activity";
            string breakoutGroup = null; 
            string activityGroup = null;  
            //string[] scheduleActivityName = { "Test Schedule" };
            

            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            try
            { //Login
              test.Portal.LoginWebDriver();

              TestLog.WriteLine("Confirm Form = {0}", formName);

              //Create New Form
              test.Portal.WebLink_FormNames_Create(formName, true, true);

              //Create activity association
               test.Portal.Activity_Association_Settings(ministry, activity, breakoutGroup, activityGroup);
              //Create Single Schedule
              //test.Portal.EventRegistration_Create_Schedule(ministry, activity, breakoutGroup, activityGroup, scheduleActivityName);

              //save changes
              //test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnDone").Click();

              //Login to Event Registration Form
              test.Infellowship.Login_Event_Registration_Form(formName, churchId, userName, password);

              //select individual
              test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

              //Clicking on 'continue' button from step2
              test.Infellowship.EventRegistration_ClickContinue();

              //Submitting details from Step3..
              test.Infellowship.EventRegistration_ClickSubmitPayment();


              //Validating Home button in infellowship
              test.GeneralMethods.VerifyTextPresentWebDriver("Home");

              //Click home
              test.Driver.FindElement(By.LinkText("Home")).Click();

              Assert.IsTrue(test.Driver.FindElement(By.LinkText("Update Profile")).Displayed, "Home page is not displayed");
            }

            finally
            {
                //Get form ID from database
                int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

                TestLog.WriteLine("+formId = {0}", formId);
                //Clear data
                TestLog.WriteLine("-formId = {0}", formId);
                base.SQL.Weblink_Form_Delete_Proc(15, formId);

                test.Driver.Quit();
            }
            
        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("FO-2901 - Verifies the security code tooltip display well in Reg 2 flow")]
        public void Infellowship_Registration_SecurityCode_Display()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string formName = "A Test Form - Form Cost Only";

            string[] individual = { "Automated Tester" };
            IList<string> selectedOrder = new List<string>();

            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++)
            {
                selectedOrder.Add(individual[i]);

            }

            //Login to Event Registration Form
            test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");

            //select individual
            test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);

            //Clicking on 'continue' button from step2
            test.Infellowship.EventRegistration_ClickContinue();


            test.Driver.FindElementById("payment_method_cc").Click();

            //Verify Security code tool tip
            test.GeneralMethods.Verify_Reg2_SecurityCode_Tooltip();

            test.Driver.Quit();

        }

        [Test, RepeatOnFailure]
        [Author("Charles Lei")]
        [Description("FO-1729- Verifies the payment option is 'Pay with eCheck'")]
        public void Infellowship_Registration_PayWitheCheck() {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            // Set initial conditions
            string guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
            string formName = string.Format("Test Form - Pricing{0}", guidRandom);
            double price = 2.00;
            string fund = "1 - General Fund (Contribution)";
            string startDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Date.ToShortDateString();
            string endDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(2).Date.ToShortDateString();

            string[] individual = { "Automated Tester", "Qa Tester" };
            IList<string> selectedOrder = new List<string>();
            //Adding individuals to selected order
            for (int i = 0; i < individual.Length; i++) {
                selectedOrder.Add(individual[i]);
            }
            //Login
            test.Portal.LoginWebDriver();
            test.Portal.WebLink_FormNames_Create(formName, true, true);
            try {
                //Create Account/Fund Association for form
                test.Portal.EventRegistration_Create_Fund_Form(fund);
                //Create Price date range for form
                test.Portal.EventRegistration_Create_Price_Form(price, startDate, endDate);
                test.Portal.LogoutWebDriver();
                //Login to Event Registration Form
                test.Infellowship.Login_Event_Registration_Form(formName, 15, "F1AutomatedTester01@gmail.com", "FT4life!");
                //select individual
                test.Infellowship.EventRegistration_Select_Individual(individual, selectedOrder, false, true);
                //Click Continue(Step2)
                test.Infellowship.EventRegistration_ClickContinue();
                //Verify 
                IWebElement element = test.Driver.FindElementById(GeneralInFellowship.EventRegistration.Echeck_Payment_Method).FindElement(By.XPath(@".."));
                string actualValue = element.Text.Trim();
                Assert.AreEqual("Pay with eCheck", actualValue);

            }
            finally {
                //Get form ID from database
                int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);
                TestLog.WriteLine("+formId = {0}", formId);
                ////Clear data
                TestLog.WriteLine("-formId = {0}", formId);
                base.SQL.Weblink_Form_Delete_Proc(15, formId);
                test.Driver.Close();
            }
        }

        [Test, RepeatOnFailure]
        [Author("Alan Luo")]
        [Description("FO-1726: Verifies share button where registration summary page.")]
        public void Infellowhip_Registration_SummaryPage_ShareSocialNetwork()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string individual_Name = "Ft Tester";
            string formName = "ShareSocialNetwork Test Form";
            string firstName = "First";
            string lastName = "Last";
            string middleName = "Middle";
            int churchId = 15;

            //init data.
            TestLog.WriteLine("Create a form to DB, formName: {0};", formName);
            test.Portal.LoginWebDriver("splatt", "Romans10:9", "DC");
            test.Portal.WebLink_FormNames_Create(formName, true);
            test.Portal.LogoutWebDriver();

            TestLog.WriteLine("Create a individual from DB, name: " + firstName + " " + middleName + " " + lastName);
            int individualId = this.SQL.People_Individual_Only_Create(churchId, firstName, lastName, middleName);
            int houseHoldId = this.SQL.People_Households_FetchID(churchId, this.SQL.People_Individuals_FetchID_ByEmail(churchId, "ft.autotester@gmail.com"));
            TestLog.WriteLine(String.Format("Add individual {0} to given household {1}", individualId, houseHoldId));
            this.SQL.People_Individual_Join_Household(churchId, houseHoldId, individualId);

            try
            {
                //test.Infellowship.LoginWebDriver(userName, password);
                test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

                //Navigate to Form
                test.GeneralMethods.OpenURLWebDriver(test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, churchId));

                test.Infellowship.EventRegistration_Select_Individual(firstName + " " + middleName.Substring(0, 1) + " " + lastName);

                //continue step 1
                test.Driver.FindElements(By.Id("continue")).First(x => x.Displayed == true).Click();
                //continue step 2
                test.Driver.FindElements(By.Id("continue")).First(x => x.Displayed == true).Click();
                //submit From 
                test.Driver.FindElements(By.Id("submitPayment")).First(x => x.Displayed == true).Click();

                TestLog.Write("Test sunmmy page link.");
                string tempPath = ".//*[@id='main-content']//*/a[@title='{0}']";
                Assert.IsTrue(test.GeneralMethods.IsElementVisibleWebDriver(By.XPath(string.Format(tempPath, "Facebook"))), "'Facebook' link not Visible!");
                Assert.IsTrue(test.GeneralMethods.IsElementVisibleWebDriver(By.XPath(string.Format(tempPath, "Tweet"))), "'Tweet' link not Visible!");
                Assert.IsTrue(test.GeneralMethods.IsElementVisibleWebDriver(By.XPath(string.Format(tempPath, "Google+"))), "'Google+' link not Visible!");

                CheckShareSocialNetworkOpenWindow("Facebook", "https://www.facebook.com", test, tempPath);
                CheckShareSocialNetworkOpenWindow("Tweet", "https://twitter.com", test, tempPath);
                CheckShareSocialNetworkOpenWindow("Google+", "https://accounts.google.com", test, tempPath);

            }
            finally
            {
                TestLog.WriteLine(String.Format("Clean up to remove individual {0} from household {1}", individualId, houseHoldId));
                this.SQL.People_DeleteIndividualFromHousehold(churchId, individualId, houseHoldId);
                TestLog.WriteLine("Clean up to remove individual from DB, id: " + individualId);
                this.SQL.People_DeleteIndividual(churchId, individualId);
                TestLog.WriteLine("Clean up the form from DB, name: " + formName);
                test.SQL.Weblink_Form_Delete_Proc(churchId, formName);
            }

        }


        /// <summary>
        /// Check Share Social Network Open Window
        /// </summary>
        /// <param name="type">Facebook,twitter,</param>
        /// <param name="link">link</param>
        /// <param name="test">test entity</param>
        /// <param name="tempPath">XPath template</param>
        public void CheckShareSocialNetworkOpenWindow(string type, string link, TestBaseWebDriver test, string tempPath)
        {
            TestLog.Write("Test open window '{0}'", type);
            test.Driver.FindElement(By.XPath(string.Format(tempPath, type))).Click();
            var currWinhandle = test.Driver.CurrentWindowHandle;
            var popwinHandle = test.Driver.WindowHandles.First(x => x != currWinhandle);

            test.Driver.SwitchTo().Window(popwinHandle);
            WebDriverWait wait = new WebDriverWait(test.Driver, TimeSpan.FromSeconds(30));
            wait.Until(delegate(IWebDriver dir)
            {
                return !dir.Url.StartsWith("https://www.addthis.com/bookmark.php");
            });

            Assert.IsTrue(test.Driver.Url.StartsWith(link), string.Format("'{0}' share page cannot open!", type));
            test.Driver.ExecuteScript("window.close();");
            test.Driver.SwitchTo().Window(currWinhandle);
        }

        [Test, RepeatOnFailure, Timeout(2000)]
        [Author("Charles Lei")]
        [Description("FO-2464-Case1.1 NO cutoff date|Due date is vaild")]
        public void Infellowship_Registration_Case11() {
            var test = TestContainer[TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            dynamic testCaseData = EventRegistration_DownpaymentAndPromoCode_Verification_SetTestCase(test, 94, null, true);
            try {
                EventRegistration_DownpaymentAndPromoCode_Verification(test, testCaseData);
            }
            finally {
                SQL.Weblink_Form_Delete_Payment_Requirement(15, testCaseData.FormName);
                SQL.Weblink_Form_Delete_Proc(15, testCaseData.FormName);
                test.Driver.Close();
            }
        }
        [Test, RepeatOnFailure, Timeout(2000)]
        [Author("Charles Lei")]
        [Description("FO-2464-case1.2 NO cutoff date|Due date is invaild")]
        public void Infellowship_Registration_Case12() {
            var test = TestContainer[TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            dynamic testCaseData = EventRegistration_DownpaymentAndPromoCode_Verification_SetTestCase(test, 95, null, false);
            try {
                EventRegistration_DownpaymentAndPromoCode_Verification(test, testCaseData);
            }
            finally {
                SQL.Weblink_Form_Delete_Payment_Requirement(15, testCaseData.FormName);
                SQL.Weblink_Form_Delete_Proc(15, testCaseData.FormName);
                test.Driver.Close();
            }
        }
        [Test, RepeatOnFailure, Timeout(2000)]
        [Author("Charles Lei")]
        [Description("FO-2464-case1.3 With vaild cutoff date&&Due date is vaild")]
        public void Infellowship_Registration_Case13() {
            var test = TestContainer[TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            dynamic testCaseData = EventRegistration_DownpaymentAndPromoCode_Verification_SetTestCase(test, 96, true, true);
            try {
                EventRegistration_DownpaymentAndPromoCode_Verification(test, testCaseData);

            }
            catch (Exception e) {
                TestLog.WriteLine(e.StackTrace);
            }
            finally {
                SQL.Weblink_Form_Delete_Payment_Requirement(15,testCaseData.FormName);
                SQL.Weblink_Form_Delete_Proc(15, testCaseData.FormName);
                test.Driver.Close();
            }
        }
        [Test, RepeatOnFailure, Timeout(2000)]
        [Author("Charles Lei")]
        [Description("FO-2464-case1.4 With vaild cutoff date&&Due date is invaild")]
        public void Infellowship_Registration_Case14() {
            var test = TestContainer[TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            dynamic testCaseData = EventRegistration_DownpaymentAndPromoCode_Verification_SetTestCase(test, 97, true, false);
            try {
                EventRegistration_DownpaymentAndPromoCode_Verification(test, testCaseData);
            }
            finally {
                SQL.Weblink_Form_Delete_Payment_Requirement(15, testCaseData.FormName);
                SQL.Weblink_Form_Delete_Proc(15, testCaseData.FormName);
                test.Driver.Close();
            }
        }
        [Test, RepeatOnFailure, Timeout(2000)]
        [Author("Charles Lei")]
        [Description("FO-2464-case1.5 With invaild cutoff date&&Due date is vaild")]
        public void Infellowship_Registration_Case15() {
            var test = TestContainer[TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            dynamic testCaseData = EventRegistration_DownpaymentAndPromoCode_Verification_SetTestCase(test, 98, false, true);
            try {
                EventRegistration_DownpaymentAndPromoCode_Verification(test, testCaseData);
            }
            finally {
                SQL.Weblink_Form_Delete_Payment_Requirement(15, testCaseData.FormName);
                SQL.Weblink_Form_Delete_Proc(15, testCaseData.FormName);
                test.Driver.Close();
            }
        }
        [Test, RepeatOnFailure, Timeout(2000)]
        [Author("Charles Lei")]
        [Description("FO-2464-case1.6 With invaild cutoff date&&Due date is invaild")]
        public void Infellowship_Registration_Case16() {
            var test = TestContainer[TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            dynamic testCaseData = EventRegistration_DownpaymentAndPromoCode_Verification_SetTestCase(test, 99, false, false);
            try {
                EventRegistration_DownpaymentAndPromoCode_Verification(test, testCaseData);
            }
            finally {
                SQL.Weblink_Form_Delete_Payment_Requirement(15, testCaseData.FormName);
                SQL.Weblink_Form_Delete_Proc(15, testCaseData.FormName);
                test.Driver.Close();
            }
        }
        [Test, RepeatOnFailure, Timeout(2000)]
        [Author("Charles Lei")]
        [Description("FO-2464-case2 DownPaymentAmount is equal to price.")]
        public void Infellowship_Registration_Case2() {
            var test = TestContainer[TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            dynamic testCaseData = EventRegistration_DownpaymentAndPromoCode_Verification_SetTestCase(test, 100, null, true);
            try {
                EventRegistration_DownpaymentAndPromoCode_Verification(test, testCaseData);
            }
            finally {
                SQL.Weblink_Form_Delete_Payment_Requirement(15, testCaseData.FormName);
                SQL.Weblink_Form_Delete_Proc(15, testCaseData.FormName);
                test.Driver.Close();
            }
        }
        [Test, RepeatOnFailure, Timeout(2000)]
        [Author("Charles Lei")]
        [Description("FO-2464-case3 DownPaymentAmount is greater than price.")]
        public void Infellowship_Registration_Case3() {
            var test = TestContainer[TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            dynamic testCaseData = EventRegistration_DownpaymentAndPromoCode_Verification_SetTestCase(test, 101, null, true);
            try {
                EventRegistration_DownpaymentAndPromoCode_Verification(test, testCaseData);
            }
            finally {
                SQL.Weblink_Form_Delete_Payment_Requirement(15,testCaseData.FormName);
                SQL.Weblink_Form_Delete_Proc(15, testCaseData.FormName);
                test.Driver.Close();
            }
        }

        private static object EventRegistration_DownpaymentAndPromoCode_Verification_SetTestCase(TestBaseWebDriver test,double downPaymentAmount, bool? isCutoffDateValid, bool? isBalanceDueDateValid)
        {
            //step1:create a test Form,name:random;
            var guidRandom = Guid.NewGuid().ToString().Substring(0, 5);
            var formName = string.Format("FO2464-Test{0}", guidRandom);
            TestLog.WriteLine(string.Format("step1:create a test Form:{0}", formName));

            //Navigate to weblink->manage forms
            TestLog.WriteLine("Navigate to weblink->manage forms");
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.WebLink.Event_Registration.Manage_Forms);
            test.Portal.EventRegistration_Create_NewForm(formName, true);

            //step2:Associate a fund;
            const string fundName = "1 - General Fund (Contribution)";
            test.Portal.EventRegistration_Create_Fund_Form(fundName);
            TestLog.WriteLine(string.Format("step2:Associate a fund:{0}", fundName));

            const int price = 100;
            var startDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).Date.ToShortDateString();
            var endDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(2).Date.ToShortDateString();
            test.Portal.EventRegistration_Create_Price_Form(price, startDate, endDate);
            TestLog.WriteLine(string.Format("step3:Create Date Range Pricing:{0}", price));

            test.Driver.FindElementByLinkText("Back").Click();
            TestLog.WriteLine("back to form summary");

            //step4:Add Promotion codes:3 test cases
            string[] promotionCodeNames = { "GreaterThanPrice", "LowerThanPrice", "EqualWithPrice", "PercentageNormal" };
            double[] amount = { 120.22, 92.11, 100, 11};
            var promotionCodes = new List<PromocodeEntity>();
            for (var i = 0; i < 4; i++) {
                var promotionCode = new PromocodeEntity {
                    PromocodeName = promotionCodeNames[i],
                    Promocode = Guid.NewGuid().ToString().Substring(0, 8)
                };
                if (i < 3) {
                    promotionCode.IsAmount = true;
                    promotionCode.Amount = amount[i];
                }
                else {
                    promotionCode.IsAmount = false;
                    promotionCode.Percentage = (uint)amount[i];
                }
                promotionCode.StartDate = startDate;
                promotionCode.EndDate = endDate;
                promotionCodes.Add(promotionCode);
            }
            test.Portal.EventRegistration_Create_PromotionCodes_Form(promotionCodes);
           
            //step3:Date Range Pricing
            string[] individual = { "Automated Tester", "Qa Tester" };
            var dateNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            var validDate = dateNow.AddDays(2).Date.ToShortDateString();
            var invalidDate = dateNow.AddDays(-2).Date.ToShortDateString();
            string cutoffDate;
            string balanceDueDate;
            if (!isCutoffDateValid.HasValue)
            {
                cutoffDate = string.Empty;
            }
            else
            {
                cutoffDate = isCutoffDateValid == true ? validDate : invalidDate;
            }
            if (!isBalanceDueDateValid.HasValue)
            {
                balanceDueDate = string.Empty;
            }
            else
            {
                balanceDueDate = isBalanceDueDateValid == true ? validDate : invalidDate;
            }
            test.Portal.EventRegistration_Create_PaymentSetting_Form(downPaymentAmount, cutoffDate, balanceDueDate);
           
            return  new {
                FormName = formName,
                Individual = individual,
                FundName = fundName,
                Price = price,
                StartDate = startDate,
                EndDate = endDate,
                PromotionCodes = promotionCodes,
                DownPaymentAmount = downPaymentAmount,
                CutoffDate = cutoffDate,
                BalanceDueDate = balanceDueDate,

            };
        }

        private static void EventRegistration_DownpaymentAndPromoCode_Verification(TestBaseWebDriver test,
            dynamic testCaseCommonData, int churchId = 15, string username = "F1AutomatedTester01@gmail.com",
            string password = "FT4life!")
        {
            double downPaymentAmount = testCaseCommonData.DownPaymentAmount;
            string cutoffDate = testCaseCommonData.CutoffDate;
            string balanceDueDate = testCaseCommonData.BalanceDueDate;
            string formName = testCaseCommonData.FormName;
            string[] individual = testCaseCommonData.Individual;
            double price = testCaseCommonData.Price;
            List<PromocodeEntity> promotionCodes = testCaseCommonData.PromotionCodes;

            //Step1:Go to Infellowship-step3
            test.Infellowship.Login_Event_Registration_Form(formName, churchId, username, password);
            test.Infellowship.EventRegistration_Select_Individual(individual, individual.ToList(), false, true);
            test.Infellowship.EventRegistration_ClickContinue();

            //Step2:Verify
            //2.1 Verify Payment options is hide or visible
            var isPaymentOptionTextExist =
                test.GeneralMethods.IsElementVisibleWebDriver(
                    By.XPath(@"//*[@id=""main-content""]/div[1]/fieldset[1]/div[1]/h3"));
            var isPaymentOptionFullExist =
                test.GeneralMethods.IsElementVisibleWebDriver(
                    By.XPath(@"//*[@id=""main-content""]/div[1]/fieldset[1]/div[1]/div[1]"));
            var isPaymentOptionPartialExist =
                test.GeneralMethods.IsElementVisibleWebDriver(
                    By.XPath(@"//*[@id=""main-content""]/div[1]/fieldset[1]/div[1]/div[2]"));

            if (DateTime.Parse(balanceDueDate) < DateTime.Now.Date
                || !string.IsNullOrWhiteSpace(cutoffDate) && DateTime.Parse(cutoffDate) < DateTime.Now.Date)
            {
                //hide
                Assert.AreEqual(false, isPaymentOptionTextExist, "PaymentOptionText should not be Visible!");
                Assert.AreEqual(false, isPaymentOptionFullExist, "PaymentOptionFull should not be Visible!");
                Assert.AreEqual(false, isPaymentOptionPartialExist, "PaymentOptionPartial should not be Visible!");
            }
            else
            {
                //visible
                Assert.AreEqual(true, isPaymentOptionTextExist, "Can not find PaymentOptionText!");
                Assert.AreEqual(true, isPaymentOptionFullExist, "Can not find PaymentOptionFull!");
                Assert.AreEqual(true, isPaymentOptionPartialExist, "Can not find PaymentOptionPartial!");
            }

            //2.2 Verify promo code link is exist
            var promoCodeLink = test.Driver.FindElementById("promo_toggle");
            var isPromoCodeLinkExist = promoCodeLink.Displayed;
            Assert.AreEqual(true, isPromoCodeLinkExist);

            //2.3 Verify when click apply button, discount code row is hide or visible
            //click and show promo code textbox
            promoCodeLink.Click();
            if (promotionCodes != null)
                foreach (var p in promotionCodes)
                {

                    var elemPromCode = test.Driver.FindElementById("promo_code");
                    elemPromCode.Clear();
                    elemPromCode.SendKeys(p.Promocode);
                    test.Driver.FindElementById("apply_promo_code").Click();
                    Thread.Sleep(3000);

                    //Verify Discount Code is visiable
                    test.GeneralMethods.WaitForElementDisplayed(By.ClassName("promo"));
                    var isPromoExist = test.GeneralMethods.IsElementVisibleWebDriver(By.ClassName("promo"));
                    Assert.AreEqual(true, isPromoExist, "Discount Code row should be visiable!");

                    //Verify Discount Code Name
                    var elemDiscountName = test.Driver.FindElementByXPath(".//*[@class='promo']/td[1]/span");
                    Assert.AreEqual(p.PromocodeName, elemDiscountName.Text, "elemDiscountName is wrong!");

                    //Verify Discount Code amount
                    var elemDiscountAmount =
                        test.Driver.FindElementByXPath(".//*[@class='promo']/td[2]/span");
                    double expectAmount;
                    var count = 0;
                    if (individual != null)
                        count += individual.Count();
                    if (p.IsAmount)
                    {
                        expectAmount = Math.Min(p.Amount, price)*count;
                    }
                    else
                    {
                        expectAmount = price*Math.Min(100, p.Percentage)*count/100;
                    }
                    Assert.AreEqual(expectAmount.ToString("f2"), elemDiscountAmount.Text,"DiscountAmount is wrong!");

                    //Verify Total Amount Due
                    var elemTotalAmount =
                        test.Driver.FindElementByXPath(".//*[@data-role='amount-due']/span");
                    var expectTotalAmount = price*count - expectAmount;
                    Assert.AreEqual(expectTotalAmount.ToString("f2"), elemTotalAmount.Text,"TotalAmount is wrong");
                    
                    //Verify Payment Method is display or not
                    var elemPaymentOption =
                        test.Driver.FindElementByXPath(".//*[@id='main-content']/div[1]/fieldset[2]/div[1]");
                    if (Math.Abs(expectTotalAmount) < 0.01)
                    {
                        //Verify Payment Option is hide                       
                        Assert.AreEqual(false, elemPaymentOption.Displayed,"Payment Option should be visible!");
                    }
                    else
                    {
                        //Verify Payment Option is visible  
                        Assert.AreEqual(true, elemPaymentOption.Displayed);

                        if (isPaymentOptionFullExist)
                        {
                            var elemPaymentFullAmount =
                                test.Driver.FindElementByXPath(
                                    @"//*[@id=""main-content""]/div[1]/fieldset[1]/div[1]/div[1]/div/div/label/strong");
                            Assert.Contains(elemPaymentFullAmount.Text, expectTotalAmount.ToString("f2"),"Total Amount is wrong!");
                        }
                        if (!isPaymentOptionPartialExist) continue;
                        //Verify downpayment partial textbox value
                        //var elemPaymentPartialAmount = test.Driver.FindElementById(@"down_payment");
                        //Assert.AreEqual((downPaymentAmount*count).ToString("f2"), elemPaymentPartialAmount.Text,"Payment partial amount is wrong");

                        //verify downpayment tips minmum amount and remainning amount.
                        var elemMinimumAmount =
                            test.Driver.FindElementByXPath(
                                @"//*[@id=""main-content""]/div[1]/fieldset[1]/div[1]/div[2]/div/div/p/strong[1]");
                        var elemRemainAmount = test.Driver.FindElementByXPath(@"//*[@id=""main-content""]/div[1]/fieldset[1]/div[1]/div[2]/div/div/p[2]/strong[2]/span");


                        Assert.Contains(elemMinimumAmount.Text, (Math.Min(price,downPaymentAmount)*count).ToString("f2"),"Minimum Amount is wrong");
                        var remainAmount = expectTotalAmount - downPaymentAmount*count;
                        Assert.Contains(elemRemainAmount.Text,
                            remainAmount >= 0.01 ? remainAmount.ToString("f2") : string.Empty, "Remain Amount is Wrong");

                        if (remainAmount >= 0.01) {
                            var elemDueDate =
                                test.Driver.FindElementByXPath(
                                    @"//*[@id=""main-content""]/div[1]/fieldset[1]/div[1]/div[2]/div/div/p");
                            Assert.Contains(elemDueDate.Text, balanceDueDate);
                        }
                        //Verify downpayment radio is enable or not
                        var paymentOptionPartial = test.Driver.FindElementById(@"payment_option_partial");
                        var enable = remainAmount >= 0.01 ? "Enable" : "Disable";
                        Assert.AreEqual(remainAmount >= 0.01, paymentOptionPartial.Enabled,
                            string.Format("Payment Option should be {0}", enable));
                    }
                }
            //2.4 Verify total amount after 2.3
            //2.5 Verify tip message show or not when the promo code is invailid
            //2.6 Verify downpayment radio is enable or not
            //2.7 Verify downpayment textbox value is between min and max
            //2.8 Verify downpayment tip message min and due to date is right or not
            //2.9 Verify Confirmation message contains downpayment row and discount code row
            //.10 Verify Confirmation message total amount.
            //.11 Verify Email.

            // Process E-check
            //test.Infellowship.EventRegistration_Echeck_Process("FT", "Tester", "972-972-9999", "111000025", "111222333");
            //Submitting details from
            //test.Infellowship.EventRegistration_ClickSubmitPayment();
            //Step3:Return to Portal
        }

        #endregion Form Restrictions

        #region Form TimeOut
        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Stuart Platt")]
        [Description("Verifies the Timeout Warning message for infellowship forms")]
        public void Infellowship_Registration_TimeOutWarning()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string userName = "stuplatttest2390@gmail.com";
            string password = "Romans10:9";
            string formName = "TimeOut Test Form";
            IList<string> selectedOrder = new List<string>();
            string individualName = "Stuart Platt";
            selectedOrder.Add(individualName);

            //login to infellowship 
            test.Infellowship.LoginWebDriver(userName, password);

            //Get form link
            string formURL = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, 15);

            //Navigate to URL
            test.GeneralMethods.OpenURLWebDriver(formURL);
            test.GeneralMethods.WaitForElement(By.Id(GeneralInFellowship.EventRegistration.FormName));

            //Select individual
            test.Infellowship.EventRegistration_Select_Individual(new string[] { "Stuart Platt" }, selectedOrder);

            test.GeneralMethods.WaitForElement(By.Id("continue"));

            //Verify Warning Text Present
            IWebElement warning = test.Driver.FindElement(By.Id("warning_modal"));
            string warningText = warning.GetAttribute("innerHTML").Trim();
            string cleanedText = warningText.Replace("\t", "").Replace("\r", "").Replace("\n", "");
            TestLog.WriteLine("This is the cleaned text -- {0}", cleanedText);
            Assert.Contains(cleanedText, "<h4 class=\"modal-title\">Time Expiring</h4>");
            Assert.Contains(cleanedText, "<big>Due to inactivity you will lose your reserved registration(s) in approximately:</big>");
            Assert.Contains(cleanedText, "<button type=\"button\" class=\"btn btn-primary\" data-role=\"extend\" id=\"ResetFormTimer\">I'm still here</button>");
            Assert.Contains(cleanedText, "<button type=\"button\" class=\"btn btn-default\" data-dismiss=\"modal\" id=\"DeleteFormIndividualSet\">Give them away!</button>");

            //CLick Logo
            // test.Driver.FindElementByClassName("brand_replacement").Click();

            if (base.F1Environment == F1Environments.STAGING)
            {
                //test.GeneralMethods.WaitForElement(By.XPath("//div[@id='brand']/a"));
                //test.Driver.FindElementByXPath("//div[@id='brand']/a").Click();
                //modify Grace Zhang
                test.GeneralMethods.WaitForElement(By.XPath("//div[@id='brand']//a"));
                test.Driver.FindElementByXPath("//div[@id='brand']//a").Click();
            }
            else
            {
                test.GeneralMethods.WaitForElement(By.XPath("//div[@id='brand']//a"));
                test.Driver.FindElementByXPath("//div[@id='brand']//a").Click();
            }

            test.GeneralMethods.WaitForElement(By.LinkText("Sign out"));
            //Logout 
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies the Timeout message for infellowship forms")]
        public void Infellowship_Registration_TimeOutExpired()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string userName = "stuplatttest2390@gmail.com";
            string password = "Romans10:9";
            string formName = "TimeOut Test Form";
            IList<string> selectedOrder = new List<string>();
            string individualName = "Stuart Platt";
            selectedOrder.Add(individualName);

            //login to infellowship 
            test.Infellowship.LoginWebDriver(userName, password);

            //Get form link
            string formURL = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, 15);

            //Navigate to URL
            test.GeneralMethods.OpenURLWebDriver(formURL);
            test.GeneralMethods.WaitForElement(By.Id(GeneralInFellowship.EventRegistration.FormName));

            //Select individual
            test.Infellowship.EventRegistration_Select_Individual(new string[] { "Stuart Platt" }, selectedOrder);

            test.GeneralMethods.WaitForElement(By.Id("continue"));

            //Verify Expired Text Present
            IWebElement warning = test.Driver.FindElement(By.Id("expired_modal"));
            string warningText = warning.GetAttribute("innerHTML").Trim();
            string cleanedText = warningText.Replace("\t", "").Replace("\r", "").Replace("\n", "");
            Assert.Contains(cleanedText, "<h4 class=\"modal-title\">Time Expired</h4>");
            Assert.Contains(cleanedText, "<big>Your time has expired and your registrations have been released.</big>");
            Assert.Contains(cleanedText, "<a id=\"GoToForm\" class=\"btn btn-default\">Restart the registration process</a>");

            //CLick Logo
            //test.Driver.FindElementByClassName("brand_replacement").Click();
            /*
            if (base.F1Environment == F1Environments.STAGING)
            {
                test.GeneralMethods.WaitForElement(By.XPath("//div[@id='brand']/a"));
                test.Driver.FindElementByXPath("//div[@id='brand']/a").Click();
            }
            else
            {
                test.GeneralMethods.WaitForElement(By.XPath("//div[@id='brand']/div/div/a"));
                test.Driver.FindElementByXPath("//div[@id='brand']/div/div/a").Click();
            }*/
            //Updated by Jim Jin: Element locator on QA and staging are same now
            test.GeneralMethods.WaitForElement(By.XPath("//div[@id='brand']/div/div/a"));
            test.Driver.FindElementByXPath("//div[@id='brand']/div/div/a").Click();
            
            test.GeneralMethods.WaitForElement(By.LinkText("Sign out"));

            //Logout Infellowship
            test.Infellowship.LogoutWebDriver();


        }

        #endregion Form TimeOut

    }

    [TestFixture]
    class infellowship_EventRegistration_SelectPage_WebDriver : FixtureBaseWebDriver
    {
        
        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verify Elements on the Select page")]
        public void Infellowship_Registration_Verify_SelectPage()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string userName = "stuplatttest2390@gmail.com";
            string password = "Romans10:9";
            string formName = "Stus Test Form";
            string fullName = "Stuart Platt";
            IList<string> selectedOrder = new List<string>();
            selectedOrder.Add(fullName);

            //Login
            test.Infellowship.LoginWebDriver(userName, password);

            //Navigate to Form
            string formURL = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, 15);
            test.GeneralMethods.OpenURLWebDriver(formURL);

            //Verify Church Logo
            test.GeneralMethods.VerifyElementDisplayedWebDriver(By.LinkText(GeneralInFellowship.EventRegistration.ChurchLogo));

            //Verify step crumbs
            // Modified by Mady, changed the classname value based on the new UI schema
            // string currentStepText = test.Driver.FindElementByClassName("uiStepWizardCurrentStep").Text;
            string currentStepText = test.Driver.FindElementByClassName("uiStepWizardCurrentStepRemovesteps").Text;
            TestLog.WriteLine("This is the Step Crumb Text : {0}", currentStepText);

            //Verfiy Form Name
            test.GeneralMethods.VerifyElementDisplayedWebDriver(By.Id(GeneralInFellowship.EventRegistration.FormName));

            //Verify individuals
            test.Infellowship.EventRegistration_Select_Individual(new string[] { fullName }, selectedOrder, false, false);

            //Verify Guest Button
            test.GeneralMethods.VerifyElementDisplayedWebDriver(By.Id(GeneralInFellowship.EventRegistration.SelectViewGuests));

            //Verify Add individual button
            test.GeneralMethods.VerifyElementDisplayedWebDriver(By.XPath(GeneralInFellowship.EventRegistration.SelectAddPersonButton));

            //Verfiy continue button
            test.GeneralMethods.VerifyElementPresentWebDriver(By.Id(GeneralInFellowship.EventRegistration.ContinueButton));

            //Click logo to go home 
            test.Driver.FindElementByLinkText(GeneralInFellowship.EventRegistration.ChurchLogo).Click();
            test.GeneralMethods.WaitForElement(By.LinkText("HOME"));

            //Logout
            test.Infellowship.LogoutWebDriver();

        }
        
        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies Add function on Select page for household member without an image")]
        public void Infellowship_Registration_Select_Individual_Add_HouseholdMember_without_Image()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string userName = "stuplatttest2390@gmail.com";
            string password = "Romans10:9";
            string formName = "Stus Test Form";
            string firstName = "New";
            string lastName = "Platt";
            string fullName = firstName + " " + lastName;
            string dob = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(-5).ToString("MM/dd/yyyy");
            IList<string> selectedOrder = new List<string>();
            selectedOrder.Add(fullName);

            //Cleanup individual
            base.SQL.People_MergeIndividual(15, "New Platt", "Merge Platt");

            //Login
            test.Infellowship.LoginWebDriver(userName, password);

            //Navigate to Form
            string formURL = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, 15);
            test.GeneralMethods.OpenURLWebDriver(formURL);

            //Add indvidual
            test.Infellowship.EventRegistration_Add_Individual(firstName, lastName, dob, "Child/Yth", "Male", "Child in Family");

            //Verify Add
            //test.Infellowship.EventRegistration_Select_Individual(new string[] { fullName }, selectedOrder, false, false);

            //Logout infellowship
            test.Driver.FindElementByLinkText(GeneralInFellowship.EventRegistration.ChurchLogo).Click();
            test.GeneralMethods.WaitForElement(By.LinkText("HOME"));
            test.Infellowship.LogoutWebDriver();

            //Cleanup newly added individual
            base.SQL.People_MergeIndividual(15, "New Platt", "Merge Platt");


        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Stuart Platt")]
        [Description("Verifies that the Save button is inactive until all required fields are valid")]
        public void Infellowship_Registration_Select_Inidivdual_Add_Individual_InvalidDOB()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string userName = "stuplatttest2390@gmail.com";
            string password = "Romans10:9";
            string formName = "Stus Test Form";
            
            //Login
            test.Infellowship.LoginWebDriver(userName, password);

            //Navigate to Form
            string formURL = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, 15);
            test.GeneralMethods.OpenURLWebDriver(formURL);

            test.Driver.FindElementByXPath(GeneralInFellowship.EventRegistration.SelectAddPersonButton).Click();
            test.GeneralMethods.WaitForElement(By.Id(GeneralInFellowship.EventRegistration.AddModalFirstName));
            
            //Verify Save Button is Disabled
            Assert.IsFalse(test.Driver.FindElementByXPath(GeneralInFellowship.EventRegistration.ModalSave).Enabled);

            // Enter Name
            test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalFirstName).SendKeys("Test");
            test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalLastName).SendKeys("Testing");

            //Verify Save Button is Disabled
            Assert.IsFalse(test.Driver.FindElementByXPath(GeneralInFellowship.EventRegistration.ModalSave).Enabled);

            //Select other options
            //new SelectElement(test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalMaritalStatus)).SelectByText("Divorced");
            new SelectElement(test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalGender)).SelectByText("Male");
            new SelectElement(test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalHouseholdPosition)).SelectByText("Single Adult");

            //Verify Save Button is Enabled
            Assert.IsTrue(test.Driver.FindElementByXPath(GeneralInFellowship.EventRegistration.ModalSave).Enabled);

            //Enter Invalid Date of Birth
            test.Driver.FindElementByName(GeneralInFellowship.EventRegistration.AddModalDOB).SendKeys("14/24/3000");

            //Verify Save Button is Disabled
            Assert.IsFalse(test.Driver.FindElementByXPath(GeneralInFellowship.EventRegistration.ModalSave).Enabled);

            //Enter Invalid Date of Birth
            test.Driver.FindElementByName(GeneralInFellowship.EventRegistration.AddModalDOB).Clear();
            test.Driver.FindElementByName(GeneralInFellowship.EventRegistration.AddModalDOB).SendKeys("Today");

            //Verify Save Button is Disabled
            Assert.IsFalse(test.Driver.FindElementByXPath(GeneralInFellowship.EventRegistration.ModalSave).Enabled);

            //Enter Valid Date of Birth
            test.Driver.FindElementByName(GeneralInFellowship.EventRegistration.AddModalDOB).Clear();
            test.Driver.FindElementByName(GeneralInFellowship.EventRegistration.AddModalDOB).SendKeys("12/25/2000");

            //Verify Save Button is Enabled
            test.GeneralMethods.WaitForElementEnabled(By.XPath(GeneralInFellowship.EventRegistration.ModalSave));
            Assert.IsTrue(test.Driver.FindElementByXPath(GeneralInFellowship.EventRegistration.ModalSave).Enabled);

            //Click Cancel
            test.Driver.FindElementByLinkText("Cancel").Click();
            test.GeneralMethods.WaitForElement(By.XPath(GeneralInFellowship.EventRegistration.SelectAddPersonButton));

            //Logout infellowship
            test.Driver.FindElementByLinkText(GeneralInFellowship.EventRegistration.ChurchLogo).Click();
            test.GeneralMethods.WaitForElement(By.LinkText("HOME"));
            test.Infellowship.LogoutWebDriver();

        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Stuart Platt")]
        [Description("Verifies that the DOB calendar for the Add individual Modal is formatted to the UK")]
        public void Infellowship_Registration_Select_Inidivdual_Add_Individual_UK_DOB()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string userName = "ft.autotester@gmail.com";
            string password = "FT4life!";
            string formName = "A Test Form";

            //Login
            test.Infellowship.LoginWebDriver(userName, password, "QAEUNLX0C6");

            //Navigate to Form
            string formURL = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, 258);
            test.GeneralMethods.OpenURLWebDriver(formURL);

            test.Driver.FindElementByXPath(GeneralInFellowship.EventRegistration.SelectAddPersonButton).Click();
            test.GeneralMethods.WaitForElement(By.Id(GeneralInFellowship.EventRegistration.AddModalFirstName));

            //Verify Save Button is Disabled
            Assert.IsFalse(test.Driver.FindElementByXPath(GeneralInFellowship.EventRegistration.ModalSave).Enabled);

            // Enter Name
            test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalFirstName).SendKeys("UKTest");
            test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalLastName).SendKeys("Testing");

            //Select other options
            //new SelectElement(test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalMaritalStatus)).SelectByText("Divorced");
            new SelectElement(test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalGender)).SelectByText("Male");
            new SelectElement(test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalHouseholdPosition)).SelectByText("Single Adult");

            //Verify Save Button is Enabled
            Assert.IsTrue(test.Driver.FindElementByXPath(GeneralInFellowship.EventRegistration.ModalSave).Enabled);

            //Enter Invalid Date of Birth
            test.Driver.FindElementByName(GeneralInFellowship.EventRegistration.AddModalDOB).SendKeys("12/24/2000");

            //Verify Save Button is Disabled
            Assert.IsFalse(test.Driver.FindElementByXPath(GeneralInFellowship.EventRegistration.ModalSave).Enabled);

            //Enter Valid Date of Birth
            test.Driver.FindElementByName(GeneralInFellowship.EventRegistration.AddModalDOB).Clear();
            test.Driver.FindElementByName(GeneralInFellowship.EventRegistration.AddModalDOB).SendKeys("25/12/2000");

            //Verify Save Button is Enabled
            test.GeneralMethods.WaitForElementEnabled(By.XPath(GeneralInFellowship.EventRegistration.ModalSave));
            Assert.IsTrue(test.Driver.FindElementByXPath(GeneralInFellowship.EventRegistration.ModalSave).Enabled);

            //Click Cancel
            test.Driver.FindElementByLinkText("Cancel").Click();
            test.GeneralMethods.WaitForElement(By.XPath(GeneralInFellowship.EventRegistration.SelectAddPersonButton));

            //Logout infellowship
            test.Driver.FindElementByLinkText("QA Enterprise Unlimited #6").Click();
            test.GeneralMethods.WaitForElement(By.LinkText("HOME"));
            test.Infellowship.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies the add function on the select individual page to household guest without image")]
        public void Infellowship_Registration_Select_Individual_Add_HouseHold_Guest()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string userName = "stuplatttest2390@gmail.com";
            string password = "Romans10:9";
            string formName = "Stus Test Form";
            string firstName = "New";
            string lastName = "Guest";
            string fullName = firstName + " " + lastName;
            string dob = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(-5).ToString("MM/dd/yyyy");
            IList<string> selectedOrder = new List<string>();
            selectedOrder.Add(fullName);

            //Cleanup newly added individual
            base.SQL.People_MergeIndividual(15, "New Guest", "Merge Platt");

            //Login
            test.Infellowship.LoginWebDriver(userName, password);

            //Navigate to Form
            string formURL = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, 15);
            test.GeneralMethods.OpenURLWebDriver(formURL);

            //Add individual
            test.Infellowship.EventRegistration_Add_Individual(firstName, lastName, dob, "Single", "Female", "Friend of Family");
            
            //Verify Add
            //test.Infellowship.EventRegistration_Select_Individual(new string[] { fullName }, selectedOrder, true, false);

            //Logout infellowship
            test.Driver.FindElementByLinkText(GeneralInFellowship.EventRegistration.ChurchLogo).Click();
            test.GeneralMethods.WaitForElement(By.LinkText("HOME"));
            test.Infellowship.LogoutWebDriver();

            //Cleanup newly added individual
            base.SQL.People_MergeIndividual(15, "New Guest", "Merge Platt");

        }

        ///Add person code to upload an image to profile 
        ///  String script = "document.getElementById('fileName').value='" + "C:\\\\temp\\\\file.txt" + "';";
        /// ((IJavascriptExecutor)driver).executeScript(script);

        [Test, RepeatOnFailure]
        [Author("Stuart Platt")]
        [Description("Verifies the edit function of the Select individual page")]
        public void Infellowhip_Registration_Select_Individual_Edit()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string userName = "stuplatttest2390@gmail.com";
            string password = "Romans10:9";
            string formName = "Edit Test Form";
            string fullName = "Lindsley Platt";
            string birthDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(-5).ToString("MM/dd/yyyy");
            string gender = "Female";

            test.SQL.WebLink_FormNames_Delete(1, 15, formName);

            //login Portal
            test.Portal.LoginWebDriver();

            //Navigate to Manage form
            test.GeneralMethods.Navigate_Portal(Navigation.Portal.WebLink.Event_Registration.Manage_Forms);
            test.GeneralMethods.WaitForElement(By.Id("ctl00_ctl00_MainContent_content_ddlActiveFilter_dropDownList"));

            //Click Add
            test.Driver.FindElementByLinkText("Add").Click();
            test.GeneralMethods.WaitForElement(By.Id("ctl00_ctl00_MainContent_content_txtFormName_textBox"));

            //Create test form
            test.Portal.EventRegistration_Create_FormRestrictions(formName, null, null, null, "Male", "0", "15", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"));

            //Search for individual
            test.Portal.People_ViewIndividual_WebDriver(fullName);

            // Edit individual
            test.Portal.People_Edit_Individual_Info(fullName, null, "-2147483648", null, "");

            //Logout Portal
            test.Portal.LogoutWebDriver();

            //Login
            test.Infellowship.LoginWebDriver(userName, password);

            //Navigate to Form
            string formURL = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, 15);
            test.GeneralMethods.OpenURLWebDriver(formURL);

            //Select individual who is needing edit
            test.Infellowship.EventRegistration_Edit_Individual(fullName, birthDate, gender);

            //Logout
            test.Driver.FindElementByLinkText(GeneralInFellowship.EventRegistration.ChurchLogo).Click();
            test.GeneralMethods.WaitForElement(By.LinkText("HOME"));
            test.Infellowship.LogoutWebDriver();

            test.SQL.WebLink_FormNames_Delete(1, 15, formName);

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies individual name display of Select individual page if has middle name")]
        public void Infellowhip_Registration_Select_Individual_MiddleNameCheck()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string userName = "stuplatttest2390@gmail.com";
            string password = "Romans10:9";
            string individual_Name = "Stuart Platt";
            string formName = "Individual Name Test Form";
            string firstName = "First";
            string lastName = "Last";
            string middleName = "Middle";
            int churchId = 15;

            TestLog.WriteLine("Create a form from DB, name: " + formName);
            test.SQL.WebLink_FormNames_Create(2, churchId, formName, true);
            TestLog.WriteLine("Create a individual from DB, name: " + firstName + " " + middleName + " " + lastName);
            int individualId = this.SQL.People_Individual_Only_Create(churchId, firstName, lastName, middleName);
            int houseHoldId = this.SQL.People_Households_FetchID(churchId, this.SQL.People_Individuals_FetchID(churchId, individual_Name));
            TestLog.WriteLine(String.Format("Add individual {0} to given household {1}", individualId, houseHoldId));
            this.SQL.People_Individual_Join_Household(churchId, houseHoldId, individualId);
            try
            {
                test.Infellowship.LoginWebDriver(userName, password);

                //Navigate to Form
                test.GeneralMethods.OpenURLWebDriver(test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, churchId));

                test.Infellowship.EventRegistration_Select_Individual(firstName + " " + middleName.Substring(0, 1) + " " + lastName);
            }
            finally
            {
                TestLog.WriteLine(String.Format("Clean up to remove individual {0} from household {1}", individualId, houseHoldId));
                this.SQL.People_DeleteIndividualFromHousehold(churchId, individualId, houseHoldId);
                TestLog.WriteLine("Clean up to remove individual from DB, id: " + individualId);
                this.SQL.People_DeleteIndividual(churchId, individualId);
                TestLog.WriteLine("Clean up the form from DB, name: " + formName);
                test.SQL.Weblink_Form_Delete_Proc(churchId, formName);
            }

        }
        
        [Test, RepeatOnFailure]
        [Author("Michelle Zheng")]
        [Description("Verifies continue button when there are too many registrants")]
        public void Infellowhip_Registration_ContinueButtonCheck()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string userName = "stuplatttest2390@gmail.com";
            string password = "Romans10:9";
            string individual_Name = "Stuart Platt";
            string formName = "Individual Name Test Form";
            string firstName = "First";
            string lastName = "Last";
            string middleName = "Middle";
            int churchId = 15;
            int peopleSize = 10;

            #region init data.

            //Step1: add information to 'ChmActivity.dbo.FORM'.
            test.SQL.WebLink_FormNames_Create(2, churchId, formName, true);
            TestLog.WriteLine("Create a form to DB, formName: {0};", formName);

            //Step2: add individual information to 'ChmPeople.dbo.Individual'.
            int individualeId = this.SQL.People_Individual_Only_Create(churchId, firstName, lastName, middleName);
            int houseHoldId = this.SQL.People_Households_FetchID(churchId, this.SQL.People_Individuals_FetchID(churchId, individual_Name));
            this.SQL.People_Individual_Join_Household(churchId, houseHoldId, individualeId);

            //create people list.
            IList<int> peoples = new List<int>(peopleSize);
            for (int i = 0; i < peopleSize; i++)
            {
                int id = this.SQL.People_Individual_Only_Create(churchId,
                   firstName + i.ToString(),
                   lastName + i.ToString(),
                   middleName + i.ToString());
                this.SQL.People_Individual_Join_Household(churchId, houseHoldId, id);
                peoples.Add(id);

            }
            #endregion

            try
            {
                test.Infellowship.LoginWebDriver(userName, password);

                //Navigate to Form
                test.GeneralMethods.OpenURLWebDriver(test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, churchId));

                test.Infellowship.EventRegistration_Select_AllIndividuals();

                test.Driver.ExecuteScript("$(document).scrollTop(100000);");
                var panelSidebar = Convert.ToDecimal(test.Driver.ExecuteScript("return $('#sidebar')[0].getBoundingClientRect().bottom;"));
                var contentSidebar = Convert.ToDecimal(test.Driver.ExecuteScript("return $('#sidebar>div')[0].getBoundingClientRect().bottom;"));

                TestLog.WriteLine("panelSidebar:{0}, contentSidebar:{1} ", panelSidebar, contentSidebar);
                Assert.IsTrue(panelSidebar > contentSidebar, "Failed to find continue button in correct location");
            }
            finally
            {
                #region clear data
                foreach (int id in peoples)
                {
                    this.SQL.People_DeleteIndividualFromHousehold(churchId, id, houseHoldId);
                    this.SQL.People_DeleteIndividual(churchId, id);
                }
                this.SQL.People_DeleteIndividualFromHousehold(churchId, individualeId, houseHoldId);
                this.SQL.People_DeleteIndividual(churchId, individualeId);
                test.SQL.Weblink_Form_Delete_Proc(churchId, formName);
                #endregion
            }

        }


        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies individual name display of Select individual page if has goes_by name")]
        public void Infellowhip_Registration_Select_Individual_GoesbyNameCheck()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string userName = "stuplatttest2390@gmail.com";
            string password = "Romans10:9";
            string individual_Name = "Stuart Platt";
            string formName = "Individual Name Test Form";
            string firstName = "First";
            string lastName = "Last";
            string goesbyName = "Goesby";
            int churchId = 15;

            TestLog.WriteLine("Create a form from DB, name: " + formName);
            test.SQL.WebLink_FormNames_Create(2, churchId, formName, true);
            TestLog.WriteLine("Create a individual from DB, name: " + firstName + " " + lastName + ", goes by: " + goesbyName);
            int individualId = this.SQL.People_Individual_Only_Create(churchId, firstName, lastName, "", goesbyName);
            int houseHoldId = this.SQL.People_Households_FetchID(churchId, this.SQL.People_Individuals_FetchID(churchId, individual_Name));
            TestLog.WriteLine(String.Format("Add individual {0} to given household {1}", individualId, houseHoldId));
            this.SQL.People_Individual_Join_Household(churchId, houseHoldId, individualId);
            try
            {
                test.Infellowship.LoginWebDriver(userName, password);

                //Navigate to Form
                test.GeneralMethods.OpenURLWebDriver(test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, churchId));

                test.Infellowship.EventRegistration_Select_Individual(goesbyName + " " + lastName);
            }
            finally
            {
                TestLog.WriteLine(String.Format("Clean up to remove individual {0} from household {1}", individualId, houseHoldId));
                this.SQL.People_DeleteIndividualFromHousehold(churchId, individualId, houseHoldId);
                TestLog.WriteLine("Clean up to remove individual from DB, id: " + individualId);
                this.SQL.People_DeleteIndividual(churchId, individualId);
                TestLog.WriteLine("Clean up the form from DB, name: " + formName);
                test.SQL.Weblink_Form_Delete_Proc(churchId, formName);
            }

        }

        [Test, RepeatOnFailure]
        [Author("Alan Luo")]
        [Description("Verifies individual age display when age < 1")]
        public void Infellowhip_Registration_Individual_AgeText()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string userName = "stuplatttest2390@gmail.com";
            string password = "Romans10:9";
            string individual_Name = "Stuart Platt";
            string formName = "Individual Age Test Form";
            string firstName = "First";
            string lastName = "Last";
            int churchId = 15;

            #region init data.

            //Step1: add information to 'ChmActivity.dbo.FORM'.
            test.SQL.WebLink_FormNames_Create(2, churchId, formName, true);
            TestLog.WriteLine("Create a form to DB, formName: {0};", formName);

            //Step2: add individual information to 'ChmPeople.dbo.Individual'.
            int individualeId = test.SQL.People_Individual_Only_Create(churchId, firstName, lastName);
            int houseHoldId = test.SQL.People_Households_FetchID(churchId, this.SQL.People_Individuals_FetchID(churchId, individual_Name));
            this.SQL.People_Individual_Join_Household(churchId, houseHoldId, individualeId);

            //create people list.
            var peoples = new List<int>();
            var birthdays = new List<DateTime>() { 
                DateTime.Now.AddMonths(-1), // < 1 year old
                DateTime.Now.AddYears(-2).AddMonths(1), // = 1 year old
                DateTime.Now.AddYears(-3).AddMonths(1) // > 1 year old
            };

            for (int i = 0; i < 3; i++)
            {
                //this delete sentence is redundant. so, comment it. updated by ivan.zhang  2016/1/8
                //this.SQL.People_DeleteIndividual(churchId, firstName + "_Age" + i.ToString(), lastName + "_Age" + i.ToString());
                int id = this.SQL.People_Individual_Only_Create_Param(churchId,
                   firstName + "_Age" + i.ToString(),
                   lastName + "_Age" + i.ToString(),
                   "", ""
                   , birthdays[i].ToString("MM/dd/yyyy"));
                this.SQL.People_Individual_Join_Household(churchId, houseHoldId, id);
                peoples.Add(id);

            }
            #endregion

            try
            {
                test.Infellowship.LoginWebDriver(userName, password);

                //Navigate to Form
                test.GeneralMethods.OpenURLWebDriver(test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, churchId));
                //show all list.
                var btnExpandLink = test.Driver.FindElement(By.Id("expand_link"));
                while (btnExpandLink.Displayed)
                {
                    btnExpandLink.Click();
                }
                TestLog.WriteLine("Show all list.");

                string ageTemp = ".//*[@id='available_people']//*[text()='{0}']/following-sibling::div/span[@class='mute']";
                var age01 = test.Driver.FindElement(By.XPath(string.Format(ageTemp, "First_age0  Last_age0")));
                var age02 = test.Driver.FindElement(By.XPath(string.Format(ageTemp, "First_age1  Last_age1")));
                var age03 = test.Driver.FindElement(By.XPath(string.Format(ageTemp, "First_age2  Last_age2")));
                
                TestLog.WriteLine("Begin check data.");
                Assert.AreEqual("- under 1", age01.Text.Trim(), "'under 1' display error!");
                Assert.AreEqual("- 1 yr old", age02.Text.Trim(), "'- 1 yr old' display error!");
                Assert.AreEqual("- 2 yrs old", age03.Text.Trim(), "'- 2 yrs old' display error!");
            }
            finally
            {
                #region clear data
                foreach (int id in peoples)
                {
                    this.SQL.People_DeleteIndividualFromHousehold(churchId, id, houseHoldId);
                    this.SQL.People_DeleteIndividual(churchId, id);
                }
                this.SQL.People_DeleteIndividualFromHousehold(churchId, individualeId, houseHoldId);
                this.SQL.People_DeleteIndividual(churchId, individualeId);
                test.SQL.Weblink_Form_Delete_Proc(churchId, formName);
                #endregion
            }

        }

        [Test, RepeatOnFailure]
        [Author("Michelle Zheng"), Timeout(1200)]
        [Description("FO-4126-Verifies deceased individual not display")]
        public void Infellowhip_Registration_Individual_Deceased_NotDisplay()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string userName = "stuplatttest2390@gmail.com";
            string password = "Romans10:9";
            string individual_Name = "Stuart Platt";
            string formName = "Individual deceased Test Form";
            string firstName = "Deceased";
            string lastName = "Last";
            string fullName = "Deceased Last";
            int displayDeceased = 0;
            int churchId = 15;

            #region init data.
            //Add information to 'ChmActivity.dbo.FORM'.
            test.SQL.WebLink_FormNames_Create(2, churchId, formName, true);
            TestLog.WriteLine("Create a form to DB, formName: {0};", formName);

            //Add individual information to 'ChmPeople.dbo.Individual'.
            int individualeId = test.SQL.People_Individual_Only_Create(churchId, firstName, lastName);
            int houseHoldId = test.SQL.People_Households_FetchID(churchId, this.SQL.People_Individuals_FetchID(churchId, individual_Name));
            this.SQL.People_Individual_Join_Household(churchId, houseHoldId, individualeId);

            //Login Portal
            test.Portal.LoginWebDriver();
            //Search for individual
            test.Portal.People_ViewIndividual_WebDriver(fullName);
            // Edit individual status
            test.Portal.People_Edit_Individual_Status(fullName,"Deceased",null,null,null);
            //Logout Portal
            test.Portal.LogoutWebDriver();


            #endregion
            try
            {
                test.Infellowship.LoginWebDriver(userName, password);

                //Navigate to Form
                test.GeneralMethods.OpenURLWebDriver(test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, churchId));
                //show all list.
                var btnExpandLink = test.Driver.FindElement(By.Id("expand_link"));
                while (btnExpandLink.Displayed)
                {
                    btnExpandLink.Click();
                }
                TestLog.WriteLine("Show all list.");

                //Go through the people list and check if deceased individual displays
                IList<IWebElement> listPeople = test.Driver.FindElement(By.Id("available_people")).FindElements(By.TagName("li"));
                foreach (IWebElement person in listPeople)
                {
                    string individual = Regex.Split(person.Text, "\r\n")[0];
                    if (individual.Equals(fullName))
                    {
                        displayDeceased = displayDeceased+1;
                    }
                }
                Assert.AreEqual(0, displayDeceased);
              
            }

            finally
            {
                #region clear data
                this.SQL.People_DeleteIndividualFromHousehold(churchId, individualeId, houseHoldId);
                this.SQL.People_DeleteIndividual(churchId, individualeId);
                test.SQL.Weblink_Form_Delete_Proc(churchId, formName);
                #endregion
            }


        }
        #endregion EventRegistration
    }


    class infellowship_EventRegistration_FormStep_WebDriver : FixtureBaseWebDriver
    {
        #region Add Person Form
        [Test, RepeatOnFailure]
        [Author("Alan Luo")]
        [Description("FO-1730: Fields when Adding New Person via form.(Verifies 'SaveChanges' button status).")]
        public void Infellowhip_Registration_Step1AddPerson_CheckSaveChangesButton()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string formName = "Registration-AddPerson Test Form";
            int churchId = 15;
            TestLog.WriteLine("Create a form in portal, formName: {0};", formName);
            test.Portal.LoginWebDriver("splatt", "Romans10:9", "DC");
            test.Portal.WebLink_FormNames_Create(formName, true);
            test.Portal.LogoutWebDriver();

            try
            {
                test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

                //Create page manager entity.
                RegistrationFormAddPerson pageMgr = new RegistrationFormAddPerson(test, formName, 15);

                #region 
                #region *Name
                //@.Check field '*Name'
                TestLog.WriteLine("[AddPerson]:Check field '*First Name'");
                pageMgr.SetPersonInformation(new AddPersonEntity()
                {
                    FirstName = string.Empty,
                    LastName = "LastName",
                    DateOfBirth = DateTime.MinValue,
                    Email = "test@123.com",
                    PhoneNumber = "18600000000",
                    Gender = "Male",
                    HouseholdMemberType = "Husband"
                });
                Assert.IsFalse(pageMgr.IsSaveChangesDisplayed, "'*First Name' is Required!");

                //@.Check field '*Name'
                TestLog.WriteLine("[AddPerson]:Check field '*Last Name'");
                pageMgr.SetPersonInformation(new AddPersonEntity()
                {
                    FirstName = "First Name",
                    LastName = string.Empty,
                    DateOfBirth = DateTime.MinValue,
                    Email = "test@123.com",
                    PhoneNumber = "18600000000",
                    Gender = "Male",
                    HouseholdMemberType = "Husband"
                });
                Assert.IsFalse(pageMgr.IsSaveChangesDisplayed, "'*Last Name' is Required!");
                #endregion Name

                #region *Household Position
                //@.Check field '*Household Position'
                TestLog.WriteLine("[AddPerson]:Check field '*Household Position'");
                pageMgr.SetPersonInformation(new AddPersonEntity()
                {
                    FirstName = "FirstName",
                    LastName = "LastName",
                    DateOfBirth = DateTime.MinValue,
                    Email = "test@123.com",
                    PhoneNumber = "18600000000",
                    Gender = string.Empty,
                    HouseholdMemberType = string.Empty
                });
                Assert.IsFalse(pageMgr.IsSaveChangesDisplayed, "'*Household Position' is Required!");
                #endregion *Household Position

                #endregion

                #region Check field 'phone number or email' when DateOfBirth <18 yrs
                //@.Check field 'phone number or email' when DateOfBirth <18 yrs
                TestLog.WriteLine("[AddPerson]:Check field 'phone number or email' when DateOfBirth <18 yrs");
                pageMgr.SetPersonInformation(new AddPersonEntity()
                {
                    FirstName = "FirstName",
                    LastName = "LastName",
                    DateOfBirth = DateTime.Now.AddYears(-1),
                    Email = string.Empty,
                    PhoneNumber = string.Empty,
                    Gender = "Male",
                    HouseholdMemberType = "Husband"
                });
                Assert.IsTrue(pageMgr.IsSaveChangesDisplayed, "'phone number or email' when DateOfBirth <18 yrs is Optional!");
                #endregion Check field 'phone number or email' when DateOfBirth <18 yrs

                #region Check field 'phone number or email' when DateOfBirth >18 yrs
                //@.Check field 'phone number or email' when DateOfBirth >18 yrs
                TestLog.WriteLine("[AddPerson]:Check field 'phone number or email' when DateOfBirth >18 yrs");
                //01. no Email , no Phone Number
                pageMgr.SetPersonInformation(new AddPersonEntity()
                {
                    FirstName = "FirstName",
                    LastName = "LastName",
                    DateOfBirth = DateTime.Now.AddYears(-19),
                    Email = string.Empty,
                    PhoneNumber = string.Empty,
                    Gender = "Male",
                    HouseholdMemberType = "Husband"
                });
                Assert.IsFalse(pageMgr.IsSaveChangesDisplayed, "'phone number or email' when DateOfBirth > 18 yrs is Required!");
                Assert.IsTrue(pageMgr.IsRequiredLabal("Phone Number"), "'Phone Number' must add '*',because it's Required!");
                Assert.IsTrue(pageMgr.IsRequiredLabal("Email"), "'Email' must add '*',because it's Required!");

                //02. no Email , has Phone Number(Incorrect)
                pageMgr.SetPersonInformation(new AddPersonEntity()
                {
                    FirstName = "FirstName",
                    LastName = "LastName",
                    DateOfBirth = DateTime.Now.AddYears(-19),
                    PhoneNumber = "123456789",
                    Email = string.Empty,
                    Gender = "Male",
                    HouseholdMemberType = "Husband"
                });
                Assert.IsFalse(pageMgr.IsSaveChangesDisplayed, "'phone number' length must >= 10!");

                //03. no Email , has Phone Number
                pageMgr.SetPersonInformation(new AddPersonEntity()
                {
                    FirstName = "FirstName",
                    LastName = "LastName",
                    DateOfBirth = DateTime.Now.AddYears(-19),
                    PhoneNumber = "1234567890",
                    Email = string.Empty,
                    Gender = "Male",
                    HouseholdMemberType = "Husband"
                });
                Assert.IsTrue(pageMgr.IsSaveChangesDisplayed, "'phone number' length must >= 10!");
                Assert.IsTrue(pageMgr.IsRequiredLabal("Phone Number"), "'Phone Number' must add '*',because it's Required!");
                Assert.IsFalse(pageMgr.IsRequiredLabal("Email"), "'Email' is empty please removed '*'");

                //04. has Email(Incorrect) , no Phone Number
                pageMgr.SetPersonInformation(new AddPersonEntity()
                {
                    FirstName = "FirstName",
                    LastName = "LastName",
                    DateOfBirth = DateTime.Now.AddYears(-19),
                    PhoneNumber = string.Empty,
                    Email = "123.com",
                    Gender = "Male",
                    HouseholdMemberType = "Husband"
                });
                Assert.IsFalse(pageMgr.IsSaveChangesDisplayed, "'Email' formart must like 'ABC@123.com'!");

                //05. has Email , no Phone Number
                pageMgr.SetPersonInformation(new AddPersonEntity()
                {
                    FirstName = "FirstName",
                    LastName = "LastName",
                    DateOfBirth = DateTime.Now.AddYears(-19),
                    PhoneNumber = string.Empty,
                    Email = "ABC@123.com",
                    Gender = "Male",
                    HouseholdMemberType = "Husband"
                });
                Assert.IsTrue(pageMgr.IsSaveChangesDisplayed, "'Email' formart must like 'ABC@123.com'!");
                Assert.IsFalse(pageMgr.IsRequiredLabal("Phone Number"), "'Phone Number' is empty please removed '*'");
                Assert.IsTrue(pageMgr.IsRequiredLabal("Email"), "'Email' must add '*',because it's Required!");

                //06. has Email , has Phone Number
                pageMgr.SetPersonInformation(new AddPersonEntity()
                {
                    FirstName = "FirstName",
                    LastName = "LastName",
                    DateOfBirth = DateTime.Now.AddYears(-19),
                    PhoneNumber = "1234567890",
                    Email = "ABC@123.com",
                    Gender = "Male",
                    HouseholdMemberType = "Husband"
                });
                Assert.IsTrue(pageMgr.IsSaveChangesDisplayed, "'phone number or email' when DateOfBirth > 18 yrs is Required!");
                Assert.IsTrue(pageMgr.IsRequiredLabal("Phone Number"), "'Phone Number' must add '*',because it's Required!");

                //07. has Email , has Phone Number(Incorrect)
                pageMgr.SetPersonInformation(new AddPersonEntity()
                {
                    FirstName = "FirstName",
                    LastName = "LastName",
                    DateOfBirth = DateTime.Now.AddYears(-19),
                    PhoneNumber = "123456789",
                    Email = "ABC@123.com",
                    Gender = "Male",
                    HouseholdMemberType = "Husband"
                });
                Assert.IsFalse(pageMgr.IsSaveChangesDisplayed, "'phone number or email' when DateOfBirth > 18 yrs is Required!");

                //08. has Email(Incorrect) , has Phone Number
                pageMgr.SetPersonInformation(new AddPersonEntity()
                {
                    FirstName = "FirstName",
                    LastName = "LastName",
                    DateOfBirth = DateTime.Now.AddYears(-19),
                    PhoneNumber = "1234567890",
                    Email = "123.com",
                    Gender = "Male",
                    HouseholdMemberType = "Husband"
                });
                Assert.IsFalse(pageMgr.IsSaveChangesDisplayed, "'phone number or email' when DateOfBirth > 18 yrs is Required!");
                #endregion
                
            }
            finally
            {
                TestLog.WriteLine("Clean up the form from DB, name: " + formName);
                test.SQL.Weblink_Form_Delete_Proc(churchId, formName);
            }

        }

        [Test, RepeatOnFailure]
        [Author("Alan Luo")]
        [Description("FO-1730: Fields when Adding New Person via form.(Verifies the result which click 'SaveChanges' button).")]
        public void Infellowhip_Registration_Step1AddPerson_CheckResult()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string formName = "Registration-AddPerson result Test Form";

            int churchId = 15;
            string addPerson_FirstName = "FNAddPerson";
            string addPerson_LastName = "LNAddPerson";
            string addPerson_Name = string.Format("{0} {1}", addPerson_FirstName, addPerson_LastName);

            //init data.
            TestLog.WriteLine("Create a form in portal, formName: {0};", formName);
            test.Portal.LoginWebDriver("splatt", "Romans10:9", "DC");
            test.Portal.WebLink_FormNames_Create(formName, true);
            test.Portal.LogoutWebDriver();

            TestLog.WriteLine("Clear individual information in DB, Clear Name: {0};", addPerson_Name);
            test.SQL.People_MergeIndividual(churchId, addPerson_Name, "Merge Dump");
            try
            {
                test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

                //Create page manager entity.
                RegistrationFormAddPerson pageMgr = new RegistrationFormAddPerson(test, formName, 15);

                //@.Check field 'phone number or email' when DateOfBirth <18 yrs
                TestLog.WriteLine("[AddPerson]:add person");

                //this function will close the 'AddPerson Form'
                pageMgr.AddPersonInformation(new AddPersonEntity()
                {
                    FirstName = addPerson_FirstName,
                    LastName = addPerson_LastName,
                    DateOfBirth = DateTime.Now.AddYears(-19),
                    Email = "ABC@123.com",
                    PhoneNumber = "1234567890",
                    Gender = "Male",
                    HouseholdMemberType = "Husband"
                });

                #region check data
                By by = By.XPath(".//*[@id='available_people']//*/strong[text()='Fnaddperson  Lnaddperson']");
                test.GeneralMethods.WaitForElement(by);
                Assert.IsNotNull(test.Driver.FindElement(by));
                #endregion

            }
            finally
            {
                TestLog.WriteLine("Clear individual information in DB, Clear Name: {0};", addPerson_Name);
                test.SQL.People_MergeIndividual(churchId, addPerson_Name, "Merge Dump");

                TestLog.WriteLine("Clean up the form from DB, name: " + formName);
                test.SQL.Weblink_Form_Delete_Proc(churchId, formName);
            }

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("FO-6836: INC2324352 - No Error Message When Entering Incorrect DOB in Inf Reg")]
        public void Infellowhip_Registration_Step1AddPerson_CheckInvalidDOBError()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string formName = "Registration-AddPerson Invalid DOB";
            int churchId = 15;
            test.SQL.Weblink_Form_Delete_Proc(churchId, formName);
            TestLog.WriteLine("Create a form in SQL Server, formName: {0};", formName);
            base.SQL.WebLink_FormNames_Create(1, churchId, formName, true);

            try
            {
                test.Infellowship.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "dc");

                //Create page manager entity.
                RegistrationFormAddPerson pageMgr = new RegistrationFormAddPerson(test, formName, churchId);

                #region Check field 'Invalid DateOfBirth format'
                TestLog.WriteLine("[AddPerson]:Check field 'Email' error");
                pageMgr.SetPersonInformation(new AddPersonEntity()
                {
                    FirstName = "FirstName",
                    LastName = "LastName",
                    DateOfBirth = DateTime.Now.AddYears(-1),
                    PhoneNumber = string.Empty,
                    Gender = "Male",
                    Email = String.Empty,
                    HouseholdMemberType = "Husband"
                });
                Assert.IsTrue(pageMgr.IsSaveChangesDisplayed, "Save button should show but not");

                TestLog.WriteLine("Move mouse focus from Last name to DOB picker");
                test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalLastName).Click();

                TestLog.WriteLine("Type invalid birthday value");
                test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalDOB).Clear();
                test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalDOB).SendKeys(DateTime.Now.AddYears(+1).ToString("MM/dd/yyyy"));
                TestLog.WriteLine("Move mouse focus out of birthday picker");
                test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalLastName).Click();

                Assert.IsFalse(pageMgr.IsSaveChangesDisplayed, "Save button should not be enable");
                test.GeneralMethods.WaitForElementDisplayed(By.Id("dobValidationMes"));
                Assert.IsTrue("Date of Birth was not valid.".Equals(test.Driver.FindElementById("dobValidationMes").Text), "Birthday error message is incorrect");

                TestLog.WriteLine("Type invalid birthday format");
                test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalDOB).Clear();
                test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalDOB).SendKeys("12121980");
                TestLog.WriteLine("Move mouse focus out of birthday picker");
                test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalLastName).Click();

                Assert.IsFalse(pageMgr.IsSaveChangesDisplayed, "Save button should not be enable");
                test.GeneralMethods.WaitForElementDisplayed(By.Id("dobValidationMes"));
                Assert.IsTrue("Date of Birth was not valid.".Equals(test.Driver.FindElementById("dobValidationMes").Text), "Birthday error message is incorrect");

                TestLog.WriteLine("Type correct birthday");
                test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalDOB).Clear();
                test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalDOB).SendKeys(DateTime.Now.AddYears(-9).ToString("MM/dd/yyyy"));
                TestLog.WriteLine("Move mouse focus out of birthday picker");
                test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalLastName).Click();

                Assert.IsTrue(pageMgr.IsSaveChangesDisplayed, "Save button should be enable");
                test.GeneralMethods.WaitForElementNotDisplayed(By.Id("dobValidationMes"));
                #endregion Check field 'Invalid DateOfBirth format'

            }
            finally
            {
                TestLog.WriteLine("Clean up the form from DB, name: " + formName);
                test.SQL.Weblink_Form_Delete_Proc(churchId, formName);
            }

        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Daniel Lee")]
        [Description("FO-1736 Reg2: Optimize > Auto-expand guests when going back to register page")]
        public void Infellowship_Registration_Auto_Expand_Guests()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string userName = "ft.autotester@gmail.com";
            string password = "FT4life!";
            string formName = "A Test Form";
            string firstName = "Friend";
            string lastName = "Guest";
            string fullName = firstName + " " + lastName;
            IList<string> selectedOrder = new List<string>();
            selectedOrder.Add(fullName);

            //Login
            test.Infellowship.LoginWebDriver(userName, password, "DC");

            //Navigate to Form
            string formURL = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, 15);
            test.GeneralMethods.OpenURLWebDriver(formURL);
            try
            {
                //Add a guest
                test.Driver.FindElementByXPath(GeneralInFellowship.EventRegistration.SelectAddPersonButton).Click();
                test.GeneralMethods.WaitForElement(By.Id(GeneralInFellowship.EventRegistration.AddModalFirstName));

                //Verify Save Button is Disabled
                Assert.IsFalse(test.Driver.FindElementByXPath(GeneralInFellowship.EventRegistration.ModalSave).Enabled);

                // Enter Name
                test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalFirstName).SendKeys(firstName);
                test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalLastName).SendKeys(lastName);

                new SelectElement(test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalGender)).SelectByText("Male");
                new SelectElement(test.Driver.FindElementById(GeneralInFellowship.EventRegistration.AddModalHouseholdPosition)).SelectByText("Friend of Family");

                //Enter Valid Date of Birth
                test.Driver.FindElementByName(GeneralInFellowship.EventRegistration.AddModalDOB).Clear();
                test.Driver.FindElementByName(GeneralInFellowship.EventRegistration.AddModalDOB).SendKeys("06/12/2000");

                //Click Save
                test.GeneralMethods.WaitForElementEnabled(By.XPath(GeneralInFellowship.EventRegistration.ModalSave));
                test.Driver.FindElementByXPath(GeneralInFellowship.EventRegistration.ModalSave).Click();


                test.GeneralMethods.WaitForElement(By.XPath(GeneralInFellowship.EventRegistration.SelectAddPersonButton));

                test.Infellowship.EventRegistration_Select_Individual(new string[] { fullName }, selectedOrder, true, true);
                //Return To step 1
                test.GeneralMethods.OpenURLWebDriver(formURL);

                test.GeneralMethods.WaitForElement(By.XPath(GeneralInFellowship.EventRegistration.SelectAddPersonButton));
                Assert.IsFalse(test.Driver.FindElementById("expand_link").Displayed);
                Assert.IsTrue(test.Driver.FindElementById("collapse_link").Displayed);
            }
            finally
            {
                this.SQL.People_Individual_Delete(15, firstName, lastName);
            }

            //Logout infellowship
            test.Driver.FindElementByLinkText(GeneralInFellowship.EventRegistration.ChurchLogo).Click();
            test.GeneralMethods.WaitForElement(By.LinkText("HOME"));
            test.Infellowship.LogoutWebDriver();
        }
        #endregion

        #region invalid individual
        [Test, RepeatOnFailure, Timeout(600)]
        [Author("Jim Jin")]
        [Description("FO-3350: Anti-forgery token Error should not be displayed when encounter an invalid individual")]
        public void Infellowship_Registration_InvalidIndividual()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            string formName = utility.GetUniqueName("Form");
            string firstName = "Auto";
            string lastName = utility.GetUniqueName("T");
            string individualName = "Stuart Platt";                       
            string householdId;
            int individual_id;

            individual_id = test.SQL.People_Individuals_FetchID(15, individualName);
            householdId = test.SQL.People_Households_FetchID(15, individual_id).ToString();
            
            try
            {
                //create an individual in specific household
                test.SQL.People_Individual_Create_In_Household(15, firstName, lastName, householdId, 1);

                //create a registration form with restrictions
                test.Portal.LoginWebDriver();
                test.Portal.WebLink_FormNames_Create_WithRestrictions(formName, true, "99999", DateTime.Now.AddMonths(-1).ToString("M/d/yyyy"), DateTime.Now.AddMonths(1).ToString("M/d/yyyy"), "Male", "5", "50", DateTime.Now.ToString("M/d/yyyy"));
                test.Portal.LogoutWebDriver();

                //login to infellowship 
                test.Infellowship.LoginWebDriver("stuplatttest2390@gmail.com", "Romans10:9");

                //Get form link
                string formURL = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, 15);

                //Navigate to URL
                test.GeneralMethods.OpenURLWebDriver(formURL);

                //Select valid individual
                utility.WaitAndGetElement(By.XPath(string.Format("//img[contains(@alt,'{0}')]", individualName.Split()[0]))).Click();

                //Click an invalid individual
                utility.WaitAndGetElement(By.XPath(string.Format("//img[contains(@alt,'{0}')]", lastName))).Click();

                string currentWindow = test.Driver.CurrentWindowHandle;
                test.Driver.SwitchTo().Window(test.Driver.WindowHandles.Last());
                utility.WaitAndGetElement(By.XPath("//a[contains(text(),'Cancel')]")).Click();
                test.Driver.SwitchTo().Window(currentWindow);

                //Continue to step 2
                utility.WaitAndGetElement(By.XPath("//button[@id='continue' and @class='btn btn-primary btn-lg next pull-right btn-group-vertical']")).Click();

                //Assertion current step is step 2
                utility.WaitForPageIsLoaded();
                IJavaScriptExecutor js_driver = (IJavaScriptExecutor)test.Driver;
                string css_value = js_driver.ExecuteScript("return $(\"strong:contains('Step 2')\").parent().attr('class')").ToString();
                Assert.Contains(css_value, "CurrentStep", "Current step isn't step 2");

                //Logout Infellowship
                //test.Driver.Close();
            }
            finally
            {
                TestLog.WriteLine("Clean up the test data");
                test.SQL.People_Individual_Delete(15, firstName, lastName);
                test.SQL.Weblink_Form_Delete_Proc(15, formName);
            }


        }
        #endregion

        #region Down payment option
        [Test, RepeatOnFailure, MultipleAsserts, Timeout(600)]
        [Author("Jim Jin")]
        [Description("FO-2903 The payment option won't be displayed in the reg1")]
        public void Infellowship_Registration_DownPaymentOption_valid_BalanceDueDate()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            //Parameters
            int formType = 1;
            string individualName = "Stuart Platt";
            string formName = utility.GetUniqueName("Form");
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

            try
            {
                //create a registration form with downPayment
                test.Portal.LoginWebDriver();
                test.Portal.WebLink_FormNames_Create_With_DownPayment(formName, "1 - General Fund (Contribution)", 10000, now.AddDays(-30), now.AddDays(20), 100, now.AddDays(-60));
                test.Portal.LogoutWebDriver();

                //get form id
                int formId = test.SQL.Weblink_InfellowshipForm_GetFormId(15, formName);

                //login to infellowship 
                test.Infellowship.LoginWebDriver("stuplatttest2390@gmail.com", "Romans10:9");

                //Navigate to form URL
                string formURL = test.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, 15);
                test.GeneralMethods.OpenURLWebDriver(formURL);

                //Select an individual and go to step2 and step3
                utility.WaitAndGetElement(By.XPath(string.Format("//img[contains(@alt,'{0}')]", individualName.Split()[0]))).Click();
                utility.WaitAndGetElement(By.XPath("//button[@id='continue' and @class='btn btn-primary btn-lg next pull-right btn-group-vertical']")).Click();
                utility.WaitAndGetElement(By.XPath("//button[@id='continue' and @class='btn btn-primary btn-lg next pull-right']")).Click();
                utility.WaitForPageIsLoaded();

                //Verify "payment options" will not be shown if balance due date is earlier than today
                Assert.IsFalse(test.Driver.PageSource.Contains("Payment Options"));

                //Set balance due date to a date later than today
                test.Driver.Manage().Cookies.DeleteAllCookies();
                test.Portal.LoginWebDriver("ft.tester", "FT4life!", "dc");
                test.Portal.WebLink_FormId_PaymentSetting_New_Or_Edit(formType, formId, 100, now.AddDays(1));
                test.Portal.LogoutWebDriver();

                //login to infellowship
                test.Infellowship.LoginWebDriver("stuplatttest2390@gmail.com", "Romans10:9");
                test.GeneralMethods.OpenURLWebDriver(formURL);

                //Select an individual, then go to step2 and step3
                utility.WaitAndGetElement(By.XPath(string.Format("//img[contains(@alt,'{0}')]", individualName.Split()[0]))).Click();
                utility.WaitAndGetElement(By.XPath("//button[@id='continue' and @class='btn btn-primary btn-lg next pull-right btn-group-vertical']")).Click();
                utility.WaitAndGetElement(By.XPath("//button[@id='continue' and @class='btn btn-primary btn-lg next pull-right']")).Click();
                utility.WaitForPageIsLoaded();

                //Verify "payment options" will be shown if balance due date is later than today
                Assert.Contains(test.Driver.PageSource, "Payment Options");
                test.Driver.Manage().Cookies.DeleteAllCookies();
            }
            finally
            {
                //clear test data
                TestLog.WriteLine("Clean up the test data");
                test.SQL.Weblink_Form_Delete_Payment_Requirement(15, formName);
                test.SQL.Weblink_Form_Delete_Proc(15, formName);
            }


        }
        #endregion
    }
 } // End of Form Restrictions fixture...
