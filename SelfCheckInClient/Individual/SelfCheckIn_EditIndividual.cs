using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using OpenQA.Selenium;
using Common.PageEntitys;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace FTTests.SelfCheckIn
{
    [TestFixture, Parallelizable]
    class SelfCheckIn_EditIndividual : SelfCheckInBaseWebDriver
    {

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("Verifies name title of household member display well")]
        public void SelfCheckIn_Individual_CheckMemberTitle()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string individual = "GraceAutoChildOld Zhang";

            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();

            test.SelfCheckIn.ViewIndividualDetailByName(individual);
            test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='edit-button']"));
            TestLog.WriteLine("Expected name: " + individual);
            test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='basic-info']/div/span"));
            string actualName = test.Driver.FindElementByXPath("//div[@class='basic-info']/div/span").Text;
            Assert.AreEqual(individual, actualName, "Individual name isn't display well on View page");

            test.SelfCheckIn.ClickEditButtonOnViewPage();
            test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='save-button']"));
            TestLog.WriteLine("Expected name: " + individual);
            test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='read-info']/div/span"));
            actualName = test.Driver.FindElementByXPath("//div[@class='read-info']/div/span").Text;
            Assert.AreEqual(individual, actualName, "Individual name isn't display well on Edit page");

            //Expand My profile menu
            test.SelfCheckIn.ClickIconMenu();
            //Logout
            test.SelfCheckIn.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies name title of household visitors display well")]
        public void SelfCheckIn_Individual_CheckGuestTitle()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string individual = "GraceAutoVisitor1 Zhang";

            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();

            try
            {
                test.SelfCheckIn.ViewIndividualDetailByName(individual);
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='edit-button']"));
                TestLog.WriteLine("Expected name: " + individual);
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='basic-info']/div/span"));
                string actualName = test.Driver.FindElementByXPath("//div[@class='basic-info']/div/span").Text;
                Assert.AreEqual(individual, actualName, "Individual name isn't display well on View page");

                test.SelfCheckIn.ClickEditButtonOnViewPage();
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='save-button']"));
                TestLog.WriteLine("Expected name: " + individual);
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='read-info']/div/span"));
                actualName = test.Driver.FindElementByXPath("//div[@class='read-info']/div/span").Text;
                Assert.AreEqual(individual, actualName, "Individual name isn't display well on View page");
            }
            finally
            {
                //Expand My profile menu
                test.SelfCheckIn.ClickIconMenu();
                //Logout
                test.SelfCheckIn.LogoutWebDriver();
            }

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies quick links on View page work well")]
        public void SelfCheckIn_Individual_CheckIndividualNamePreview()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string individual = "GraceAutoChildYong Zhang";

            //Login
            test.SelfCheckIn.LoginWebDriver();

            test.SelfCheckIn.WaitForAssignmentLoaded();

            TestLog.WriteLine("Open individual view page");
            test.SelfCheckIn.ViewIndividualDetailByName(individual);
            test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='edit-button']"));

            TestLog.WriteLine("Open individual edit page");
            test.SelfCheckIn.ClickEditButtonOnViewPage();
            test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='save-button']"));
            test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='read-info']/div/span"));

            test.SelfCheckIn.InputNameOnEditPage("", "", "");
            test.SelfCheckIn.InputPhoneOnEditPage("", "", "");
            Assert.IsTrue(("").Equals(test.Driver.FindElementByXPath("//div[@class='read-info']/div/span").Text.Trim()),
                "Individual name preview isn't dynamically change");

            test.SelfCheckIn.InputNameOnEditPage("Mady", "", "Kou");
            test.SelfCheckIn.InputPhoneOnEditPage("", "", "");
            Assert.IsTrue(("Mady Kou").Equals(test.Driver.FindElementByXPath("//div[@class='read-info']/div/span").Text),
                    "Individual name preview isn't dynamically change");

            test.SelfCheckIn.InputNameOnEditPage("Mady", "middle", "Kou");
            test.SelfCheckIn.InputPhoneOnEditPage("", "", "");
            Assert.IsTrue(("Mady 'middle' Kou").Equals(test.Driver.FindElementByXPath("//div[@class='read-info']/div/span").Text),
                "Individual name preview isn't dynamically change");

            //Expand My profile menu
            test.SelfCheckIn.ClickIconMenu();
            //Logout
            test.SelfCheckIn.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("Verifies quick links on View page work well")]
        public void SelfCheckIn_Individual_CheckQuickLinkToEdit()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string individual = "GraceAutoChildYong Zhang";

            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();

            TestLog.WriteLine("Open individual view page");
            test.SelfCheckIn.ViewIndividualDetailByName(individual);
            test.GeneralMethods.WaitForElement(By.XPath("//div[@class='edit-button']"));

            TestLog.WriteLine("Open individual edit page");
            test.SelfCheckIn.ClickEditButtonOnViewPage();
            test.GeneralMethods.WaitForElement(By.XPath("//div[@class='save-button']"));

            string[] oldDetail = test.SelfCheckIn.GetIndividualDetailsOnEidt();

            try
            {
                test.SelfCheckIn.InputPhoneOnEditPage("", "", "");
                test.SelfCheckIn.InputSpecialInfoOnEditPage("", true, "", true);
                Assert.IsTrue(individual.Equals(test.Driver.FindElementByXPath("//div[@class='read-info']/div/span").Text),
                    "Individual name should not change if isn't edited");
                
                test.SelfCheckIn.ClickDoneButtonOnEditPage();
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='edit-button']"));
                test.SelfCheckIn.ClickToAddMobileContact();
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='save-button']"));

                test.SelfCheckIn.ClickDoneButtonOnEditPage();
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='edit-button']"));
                test.SelfCheckIn.ClickToAddHomeContact();
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='save-button']"));

                test.SelfCheckIn.ClickDoneButtonOnEditPage();
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='edit-button']"));
                test.SelfCheckIn.ClickToAddEmergenyContact();
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='save-button']"));

                test.SelfCheckIn.ClickDoneButtonOnEditPage();
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='edit-button']"));
                test.SelfCheckIn.ClickToAdd1stInfo();
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='save-button']"));

                test.SelfCheckIn.ClickDoneButtonOnEditPage();
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='edit-button']"));
                test.SelfCheckIn.ClickToAdd2ndInfo();
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='save-button']"));
            }
            finally
            {
                SetBackIndividualDetails(oldDetail);

                //Expand My profile menu
                test.SelfCheckIn.ClickIconMenu();
                //Logout
                test.SelfCheckIn.LogoutWebDriver();
            }

        }

        [Test(Order=1), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("Verifies home phone and emegency phone work for the whole household")]
        public void SelfCheckIn_Individual_CheckHomePhoneEffectToAll()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string individual = "GraceAutoVisitor1 Zhang";
            string individual2 = "GraceAutoAssignRuleTester1 Zhang";
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();

            TestLog.WriteLine("Open individual view page");
            test.SelfCheckIn.ViewIndividualDetailByName(individual);
            test.GeneralMethods.WaitForElement(By.XPath("//div[@class='edit-button']"));

            TestLog.WriteLine("Open individual edit page");
            test.SelfCheckIn.ClickEditButtonOnViewPage();
            test.GeneralMethods.WaitForElement(By.XPath("//div[@class='save-button']"));

            string[] oldDetail = test.SelfCheckIn.GetIndividualDetailsOnEidt();

            try
            {
                test.SelfCheckIn.InputPhoneOnEditPage("12345", "23456", "453234");
                test.SelfCheckIn.InputSpecialInfoOnEditPage("test243", true, "test9873", false);

                test.SelfCheckIn.ClickDoneButtonOnEditPage();
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='edit-button']"));
                test.SelfCheckIn.ClickGoLeftButton();
                test.SelfCheckIn.WaitForAssignmentLoaded();

                test.SelfCheckIn.ViewIndividualDetailByName(individual2);
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='edit-button']"));

                TestLog.WriteLine("Open individual edit page");
                test.SelfCheckIn.ClickEditButtonOnViewPage();
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='save-button']"));

                string[] individual2Detail = test.SelfCheckIn.GetIndividualDetailsOnEidt();
                Assert.IsFalse("12345".Equals(individual2Detail[3]), "Individual 2 mobile number should not be same as old individual");
                Assert.IsTrue("23456".Equals(individual2Detail[4]), "Individual 2 home number should be same as old individual");
                Assert.IsTrue("453234".Equals(individual2Detail[5]), "Individual 2 emergency number should be same as old individual");
                Assert.IsFalse("test243".Equals(individual2Detail[6]), "Individual 2 first special instruction should not be same as old individual");
                Assert.IsFalse("test9873".Equals(individual2Detail[8]), "Individual 2 second special instruction should not be same as old individual");
            }
            finally
            {
                test.SelfCheckIn.ClickGoLeftButton();
                TestLog.WriteLine("Open individual view page");
                try
                {
                    test.SelfCheckIn.ViewIndividualDetailByName(individual);
                }
                catch (WebDriverException)
                {
                    test.SelfCheckIn.ClickGoLeftButton();
                    test.SelfCheckIn.ViewIndividualDetailByName(individual);
                }

                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='edit-button']"));

                TestLog.WriteLine("Open individual edit page");
                test.SelfCheckIn.ClickEditButtonOnViewPage();
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='save-button']"));

                SetBackIndividualDetails(oldDetail);

                //Expand My profile menu
                test.SelfCheckIn.ClickIconMenu();
                //Logout
                test.SelfCheckIn.LogoutWebDriver();
            }

        }

        [Test(Order = 2), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("Verifies modify all values of individual works well")]
        public void SelfCheckIn_Individual_CheckChangeAllDetails()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string individual = "GraceAutoOther Zhang";
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();

            TestLog.WriteLine("Open individual view page");
            test.SelfCheckIn.ViewIndividualDetailByName(individual);
            test.GeneralMethods.WaitForElement(By.XPath("//div[@class='edit-button']"));

            TestLog.WriteLine("Open individual edit page");
            test.SelfCheckIn.ClickEditButtonOnViewPage();
            test.GeneralMethods.WaitForElement(By.XPath("//div[@class='save-button']"));

            string[] oldDetail = test.SelfCheckIn.GetIndividualDetailsOnEidt();
            test.SelfCheckIn.InputNameOnEditPage("first", "goesBy", "last");
            test.SelfCheckIn.InputPhoneOnEditPage("54321", "878987654", "245676453");
            test.SelfCheckIn.InputSpecialInfoOnEditPage("test a today", false, "test a per", true);

            test.SelfCheckIn.ClickDoneButtonOnEditPage();

            try
            {
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='edit-button']"));

                TestLog.WriteLine("Open individual edit page");
                test.SelfCheckIn.ClickEditButtonOnViewPage();
                test.GeneralMethods.WaitForElement(By.XPath("//div[@class='save-button']"));

                string[] newDetail = test.SelfCheckIn.GetIndividualDetailsOnEidt();
                Assert.IsTrue("first".Equals(newDetail[0]), "First name should be same as set");
                Assert.IsTrue("goesBy".Equals(newDetail[1]), "Middle name should be same as set");
                Assert.IsTrue("last".Equals(newDetail[2]), "Last name should be same as set");
                Assert.IsTrue("54321".Equals(newDetail[3]), "Mobile phone name should be same as set");
                Assert.IsTrue("878987654".Equals(newDetail[4]), "Home phone name should be same as set");
                Assert.IsTrue("245676453".Equals(newDetail[5]), "Emergency phone name should be same as set");
                Assert.IsTrue("test a today".Equals(newDetail[6]), "Special info 1 should be same as set");
                Assert.IsTrue("test a per".Equals(newDetail[8]), "Special info 2 should be same as set");
            }
            finally
            {
                SetBackIndividualDetails(oldDetail);

                //Expand My profile menu
                test.SelfCheckIn.ClickIconMenu();
                //Logout
                test.SelfCheckIn.LogoutWebDriver();
            }

        }

        [Test(Order = 3), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("Verifies delete special info on View page")]
        public void SelfCheckIn_Individual_CheckDeleteSpecialInfo()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string individual = "GraceAutoSpouse Zhang";
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();

            TestLog.WriteLine("Open individual view page");
            test.SelfCheckIn.ViewIndividualDetailByName(individual);
            test.GeneralMethods.WaitForElement(By.XPath("//div[@class='edit-button']"));

            TestLog.WriteLine("Open individual edit page");
            test.SelfCheckIn.ClickEditButtonOnViewPage();
            test.GeneralMethods.WaitForElement(By.XPath("//div[@class='save-button']"));

            string[] oldDetail = test.SelfCheckIn.GetIndividualDetailsOnEidt();

            test.SelfCheckIn.InputSpecialInfoOnEditPage("Today", false, "Per", true);

            test.SelfCheckIn.ClickDoneButtonOnEditPage();

            try
            {
                test.GeneralMethods.WaitForElement(By.XPath("//div[@class='edit-button']"));

                test.SelfCheckIn.ClickToAdd1stInfo();
                test.SelfCheckIn.ClickToAdd2ndInfo();
                test.GeneralMethods.WaitForElement(By.XPath("//div[@class='save-button']"));

            }
            catch (Exception) 
            {
                test.SelfCheckIn.ClickToDelete1stInfo();
                test.GeneralMethods.WaitForElement(By.XPath("//div[@class='edit-button']"));
                test.SelfCheckIn.ClickToDelete2ndInfo();

                test.SelfCheckIn.ClickToAdd1stInfo();
                test.GeneralMethods.WaitForElement(By.XPath("//div[@class='save-button']"));
            }
            finally
            {
                SetBackIndividualDetails(oldDetail);

                //Expand My profile menu
                test.SelfCheckIn.ClickIconMenu();
                //Logout
                test.SelfCheckIn.LogoutWebDriver();
            }

        }

        [Test(Order = 4), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("Verifies special info type is saved correctly on Edit page")]
        public void SelfCheckIn_Individual_CheckSpecialInfoTypeSave()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string individual = "GraceAutoSpouse Zhang";
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();

            TestLog.WriteLine("Open individual view page");
            test.SelfCheckIn.ViewIndividualDetailByName(individual);
            test.GeneralMethods.WaitForElement(By.XPath("//div[@class='edit-button']"));

            TestLog.WriteLine("Open individual edit page");
            test.SelfCheckIn.ClickEditButtonOnViewPage();
            test.GeneralMethods.WaitForElement(By.XPath("//div[@class='save-button']"));

            string[] oldDetail = test.SelfCheckIn.GetIndividualDetailsOnEidt();

            test.SelfCheckIn.InputSpecialInfoOnEditPage("Today", false, "Per", true);

            test.SelfCheckIn.ClickDoneButtonOnEditPage();

            try
            {
                test.GeneralMethods.WaitForElement(By.XPath("//div[@class='edit-button']"));

                TestLog.WriteLine("Open individual edit page");
                test.SelfCheckIn.ClickEditButtonOnViewPage();
                test.GeneralMethods.WaitForElement(By.XPath("//div[@class='save-button']"));

                string[] newDetail = test.SelfCheckIn.GetIndividualDetailsOnEidt();
                Assert.IsTrue("Today".Equals(newDetail[6]), "Special info 1 should be same as set");
                Assert.IsTrue("false".Equals(newDetail[7]), "Special info 1 type should be same as set");
                Assert.IsTrue("Per".Equals(newDetail[8]), "Special info 2 should be same as set");
                Assert.IsTrue("true".Equals(newDetail[9]), "Special info 2 type should be same as set");
            }
            catch (Exception)
            {
                test.SelfCheckIn.ClickToDelete1stInfo();
                test.GeneralMethods.WaitForElement(By.XPath("//div[@class='edit-button']"));
                test.SelfCheckIn.ClickToDelete2ndInfo();

                test.SelfCheckIn.ClickToAdd1stInfo();
                test.GeneralMethods.WaitForElement(By.XPath("//div[@class='save-button']"));
            }
            finally
            {
                SetBackIndividualDetails(oldDetail);

                //Expand My profile menu
                test.SelfCheckIn.ClickIconMenu();
                //Logout
                test.SelfCheckIn.LogoutWebDriver();
            }

        }

        [Test(Order = 5), RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies it can auto format phone numbers")]
        public void SelfCheckIn_Individual_CheckPhoneAutoFormat()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string individual = "GraceAutoSpouse Zhang";
            //Login
            test.SelfCheckIn.LoginWebDriver();
            test.SelfCheckIn.WaitForAssignmentLoaded();

            TestLog.WriteLine("Open individual view page");
            test.SelfCheckIn.ViewIndividualDetailByName(individual);
            test.GeneralMethods.WaitForElement(By.XPath("//div[@class='edit-button']"));

            TestLog.WriteLine("Open individual edit page");
            test.SelfCheckIn.ClickEditButtonOnViewPage();
            test.GeneralMethods.WaitForElement(By.XPath("//div[@class='save-button']"));

            string[] oldDetail = test.SelfCheckIn.GetIndividualDetailsOnEidt();

            test.SelfCheckIn.InputPhoneOnEditPage("1112223333", "2223334444", "3334445555");

            test.SelfCheckIn.ClickDoneButtonOnEditPage();

            try
            {
                test.GeneralMethods.WaitForElement(By.XPath("//div[@class='edit-button']"));

                TestLog.WriteLine("Open individual edit page");
                test.SelfCheckIn.ClickEditButtonOnViewPage();
                test.GeneralMethods.WaitForElement(By.XPath("//div[@class='save-button']"));

                string[] newDetail = test.SelfCheckIn.GetIndividualDetailsOnEidt();
                Assert.IsTrue("(111) 222-3333".Equals(newDetail[3]), "Mobile phone name should be same as set");
                Assert.IsTrue("(222) 333-4444".Equals(newDetail[4]), "Home phone name should be same as set");
                Assert.IsTrue("(333) 444-5555".Equals(newDetail[5]), "Emergency phone name should be same as set");

            }
            catch (Exception)
            {
                test.SelfCheckIn.ClickToDelete1stInfo();
                test.GeneralMethods.WaitForElement(By.XPath("//div[@class='edit-button']"));
                test.SelfCheckIn.ClickToDelete2ndInfo();

                test.SelfCheckIn.ClickToAdd1stInfo();
                test.GeneralMethods.WaitForElement(By.XPath("//div[@class='save-button']"));
            }
            finally
            {
                SetBackIndividualDetails(oldDetail);

                //Expand My profile menu
                test.SelfCheckIn.ClickIconMenu();
                //Logout
                test.SelfCheckIn.LogoutWebDriver();
            }

        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("Verifies it can save character in phone number in UK church")]
        public void SelfCheckIn_Individual_CheckPhoneValidationUK()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string individual = "FT Tester";
            //Login
            test.SelfCheckIn.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "QAEUNLX0C6", true);
            test.SelfCheckIn.WaitForAssignmentLoaded();

            TestLog.WriteLine("Open individual view page");
            test.SelfCheckIn.ViewIndividualDetailByName(individual);
            test.GeneralMethods.WaitForElement(By.XPath("//div[@class='edit-button']"));

            TestLog.WriteLine("Open individual edit page");
            test.SelfCheckIn.ClickEditButtonOnViewPage();
            test.GeneralMethods.WaitForElement(By.XPath("//div[@class='save-button']"));

            string[] oldDetail = test.SelfCheckIn.GetIndividualDetailsOnEidt();

            test.SelfCheckIn.InputPhoneOnEditPage("abc2223333", "2223334444", "3334445555# *");

            test.SelfCheckIn.ClickDoneButtonOnEditPage();

            try
            {
                test.GeneralMethods.WaitForElement(By.XPath("//div[@class='edit-button']"));

                TestLog.WriteLine("Open individual edit page");
                test.SelfCheckIn.ClickEditButtonOnViewPage();
                test.GeneralMethods.WaitForElement(By.XPath("//div[@class='save-button']"));

                string[] newDetail = test.SelfCheckIn.GetIndividualDetailsOnEidt();
                Assert.IsTrue("abc2223333".Equals(newDetail[3]), "Mobile phone name should be same as set");
                Assert.IsTrue("2223334444".Equals(newDetail[4]), "Home phone name should be same as set");
                Assert.IsTrue("3334445555# *".Equals(newDetail[5]), "Emergency phone name should be same as set");

            }
            catch (Exception)
            {
                test.SelfCheckIn.ClickToDelete1stInfo();
                test.GeneralMethods.WaitForElement(By.XPath("//div[@class='edit-button']"));
                test.SelfCheckIn.ClickToDelete2ndInfo();

                test.SelfCheckIn.ClickToAdd1stInfo();
                test.GeneralMethods.WaitForElement(By.XPath("//div[@class='save-button']"));
            }
            finally
            {
                SetBackIndividualDetails(oldDetail);

                //Expand My profile menu
                test.SelfCheckIn.ClickIconMenu();
                //Logout
                test.SelfCheckIn.LogoutWebDriver();
            }

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies it can not save without first name")]
        public void SelfCheckIn_Individual_CheckFirstNameMissError()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string individual = "FT Tester";
            //Login
            test.SelfCheckIn.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "QAEUNLX0C6", true);
            test.SelfCheckIn.WaitForAssignmentLoaded();

            TestLog.WriteLine("Open individual view page");
            test.SelfCheckIn.ViewIndividualDetailByName(individual);
            test.GeneralMethods.WaitForElement(By.XPath("//div[@class='edit-button']"));

            TestLog.WriteLine("Open individual edit page");
            test.SelfCheckIn.ClickEditButtonOnViewPage();
            test.GeneralMethods.WaitForElement(By.XPath("//div[@class='save-button']"));

            string[] oldDetail = test.SelfCheckIn.GetIndividualDetailsOnEidt();

            test.SelfCheckIn.InputNameOnEditPage("", "middle", "last");

            test.SelfCheckIn.ClickDoneButtonOnEditPage();

            try
            {
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='edit-button']"));
                test.GeneralMethods.WaitForElementEnabled(By.XPath("//div[@class='edit-button']"));
                Assert.Fail("It should not save individual successfully without first name");
            }
            catch (Exception)
            {
                TestLog.WriteLine("Check first name error dialog");
                // Disable error dialog operation since FF30 can't display the dialog but the latest version works well
                // test.Driver.SwitchTo().Alert().Accept();
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='save-button']"));
            }

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies it can not save without last name")]
        public void SelfCheckIn_Individual_CheckLastNameMissError()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string individual = "FT Tester";
            //Login
            test.SelfCheckIn.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "QAEUNLX0C6", true);
            test.SelfCheckIn.WaitForAssignmentLoaded();

            TestLog.WriteLine("Open individual view page");
            test.SelfCheckIn.ViewIndividualDetailByName(individual);
            test.GeneralMethods.WaitForElement(By.XPath("//div[@class='edit-button']"));

            TestLog.WriteLine("Open individual edit page");
            test.SelfCheckIn.ClickEditButtonOnViewPage();
            test.GeneralMethods.WaitForElement(By.XPath("//div[@class='save-button']"));

            string[] oldDetail = test.SelfCheckIn.GetIndividualDetailsOnEidt();

            test.SelfCheckIn.InputNameOnEditPage("first", "middle", "");

            test.SelfCheckIn.ClickDoneButtonOnEditPage();

            try
            {
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='edit-button']"));
                test.GeneralMethods.WaitForElementEnabled(By.XPath("//div[@class='edit-button']"));
                Assert.Fail("It should not save individual successfully without first name");
            }
            catch (Exception)
            {
                TestLog.WriteLine("Check first name error dialog");
                // Disable error dialog operation since FF30 can't display the dialog but the latest version works well
                // test.Driver.SwitchTo().Alert().Accept();
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='save-button']"));
            }

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies it can save without middle name")]
        public void SelfCheckIn_Individual_CheckMiddleNameCanMiss()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string individual = "FT Tester";
            //Login
            test.SelfCheckIn.LoginWebDriver("ft.autotester@gmail.com", "FT4life!", "QAEUNLX0C6", true);
            test.SelfCheckIn.WaitForAssignmentLoaded();

            TestLog.WriteLine("Open individual view page");
            test.SelfCheckIn.ViewIndividualDetailByName(individual);
            test.GeneralMethods.WaitForElement(By.XPath("//div[@class='edit-button']"));

            TestLog.WriteLine("Open individual edit page");
            test.SelfCheckIn.ClickEditButtonOnViewPage();
            test.GeneralMethods.WaitForElement(By.XPath("//div[@class='save-button']"));

            string[] oldDetail = test.SelfCheckIn.GetIndividualDetailsOnEidt();

            test.SelfCheckIn.InputNameOnEditPage("first", "", "last");

            test.SelfCheckIn.ClickDoneButtonOnEditPage();

            try
            {
                test.GeneralMethods.WaitForElement(By.XPath("//div[@class='edit-button']"));

            }
            finally
            {
                TestLog.WriteLine("Open individual edit page");
                test.SelfCheckIn.ClickEditButtonOnViewPage();
                test.GeneralMethods.WaitForElement(By.XPath("//div[@class='save-button']"));
                SetBackIndividualDetails(oldDetail);

                //Expand My profile menu
                test.SelfCheckIn.ClickIconMenu();
                //Logout
                test.SelfCheckIn.LogoutWebDriver();
            }

        }

        private void SetBackIndividualDetails(string[] values)
        {
            try
            {
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='save-button']"));
            }
            catch (Exception)
            {
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='edit-button']"));
                test.SelfCheckIn.ClickEditButtonOnViewPage();
                test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='save-button']"));
            }

            test.SelfCheckIn.InputNameOnEditPage(values[0], values[1], values[2]);
            test.SelfCheckIn.InputPhoneOnEditPage(values[3], values[4], values[5]);
            test.SelfCheckIn.InputSpecialInfoOnEditPage(values[6], "true".Equals(values[7]), values[8], "true".Equals(values[9]));

            test.SelfCheckIn.ClickDoneButtonOnEditPage();
            test.GeneralMethods.WaitForElementDisplayed(By.XPath("//div[@class='edit-button']"));
        }

    }
}
