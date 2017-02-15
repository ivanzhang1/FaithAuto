using FTTests;
using Gallio.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.PageEntitys
{
    public class AddPersonEntity
    {
        /// <summary>
        /// get and set 'FirstName'
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// get and set 'LastName'
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// get and set 'Date Of Birth'
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// get and set 'Gender'
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// get and set 'Household Position'
        /// </summary>
        public string HouseholdMemberType { get; set; }

        /// <summary>
        /// get and set 'Phone Number'
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// get and set 'Email'
        /// </summary>        
        public string Email { get; set; }
    }

    public class RegistrationFormAddPerson
    {
        protected TestBaseWebDriver _testManager;

        #region input Control
        /// <summary>
        /// input text: 'first name'
        /// </summary>
        [FindsBy(How = How.Id, Using = "first_name")]
        public IWebElement TxtFirstName { get; set; }

        /// <summary>
        /// input text: 'last name'
        /// </summary>
        [FindsBy(How = How.Id, Using = "last_name")]
        public IWebElement TxtLastName { get; set; }

        /// <summary>
        /// input text: 'Date Of Birth'
        /// </summary>
        [FindsBy(How = How.Id, Using = "dob")]
        public IWebElement TxtDateOfBirth { get; set; }

        [FindsBy(How = How.Id, Using = "gender")]
        public IWebElement selectGender { get; set; }
        /// <summary>
        /// select Element: 'Gender'
        /// </summary>
        public SelectElement SelectGender
        {
            get
            {
                return new SelectElement(selectGender);
            }
        }


        [FindsBy(How = How.Id, Using = "household_member_type")]
        public IWebElement selectHouseholdMemberType;
        /// <summary>
        /// select Element: 'Household Position'
        /// </summary>
        public SelectElement SelectHouseholdMemberType
        {
            get
            {
                return new SelectElement(selectHouseholdMemberType);
            }
        }

        /// <summary>
        /// input text: 'Phone Number'
        /// </summary>
        [FindsBy(How = How.Id, Using = "Phone_Numbers")]
        public IWebElement TxtPhoneNumber { get; set; }

        /// <summary>
        /// input text: 'Email'
        /// </summary>
        [FindsBy(How = How.Id, Using = "Email")]
        public IWebElement TxtEmail { get; set; }

        /// <summary>
        /// button text: 'Save Changes'
        /// </summary>
        [FindsBy(How = How.XPath, Using = ".//*[@id='add_modal']//*/form/div/button[text()='Save Changes']")]
        public IWebElement BtnSaveChanges { get; set; }
        #endregion

        /// <summary>
        /// go to form page and show add window.
        /// </summary>
        /// <param name="testManager">test object</param>
        /// <param name="formName">form name</param>
        /// <param name="churchId">church id</param>
        public RegistrationFormAddPerson(TestBaseWebDriver testManager, string formName, int churchId)
        {
            //go to form page and show add window.
            TestLog.WriteLine("Navigate to Registration form step1  page. fromName:'{0}'", formName);
            testManager.GeneralMethods.OpenURLWebDriver(testManager.Infellowship.Get_Infellowship_EventRegistration_Form_URL(formName, churchId));
            testManager.Driver.FindElement(By.XPath(".//*[@id='add_person']/a")).Click();
            testManager.GeneralMethods.WaitForElementDisplayed(By.XPath(".//*[@id='add_modal']//*/h4[text()='Add Person']"));

            PageFactory.InitElements(testManager.Driver, this);
            this._testManager = testManager;
        }
        /// <summary>
        /// Action：Set Person Information
        /// </summary>
        /// <param name="entity">entity</param>
        public void SetPersonInformation(AddPersonEntity entity)
        {
            this.TxtFirstName.Clear();
            this.TxtLastName.Clear();
            this.TxtDateOfBirth.Clear();
            this.SelectGender.SelectByIndex(1);
            this.SelectHouseholdMemberType.SelectByIndex(1);
            this.TxtPhoneNumber.Clear();
            this.TxtEmail.Clear();

            //Name
            this.TxtFirstName.SendKeys(entity.FirstName);
            this.TxtLastName.SendKeys(entity.LastName);
            //DateOfBirth
            if (entity.DateOfBirth == null || entity.DateOfBirth == DateTime.MinValue)
            {
                this.TxtDateOfBirth.SendKeys("MM/DD/YYYY");
            }
            else
            {
                this.TxtDateOfBirth.SendKeys(entity.DateOfBirth.ToString("MM/dd/yyyy"));
            }
            //Gender
            if (!string.IsNullOrEmpty(entity.Gender))
            {
                this.SelectGender.SelectByText(entity.Gender);
            }
            //HouseholdMemberType
            if (!string.IsNullOrEmpty(entity.HouseholdMemberType))
            {
                this.SelectHouseholdMemberType.SelectByText(entity.HouseholdMemberType);
            }

            this.TxtPhoneNumber.SendKeys(entity.PhoneNumber);
            this.TxtEmail.SendKeys(entity.Email);

            //must have this active.
            this.TxtFirstName.SendKeys("");
        }

        /// <summary>
        /// Action：add Person Information
        /// this function will close the 'AddPerson Form'
        /// </summary>
        /// <param name="entity">entity</param>
        public void AddPersonInformation(AddPersonEntity entity)
        {
            SetPersonInformation(entity);
            if (!IsSaveChangesDisplayed)
            {
                throw new Exception("Can't add person with your input data.");
            }
            this.BtnSaveChanges.Click();
        }
        /// <summary>
        /// Check element has '*'.
        /// </summary>
        /// <param name="labalName"></param>
        /// <returns></returns>
        public bool IsRequiredLabal(string labalName)
        {
            string elementXPath = string.Format(".//*[@id='add_modal']//*/label[text()='{0}']",labalName);
            var element = this._testManager.Driver.FindElement(By.XPath(elementXPath));
            string strClass = element.GetAttribute("class");
            return strClass.Contains("required");
        }
        public bool IsSaveChangesDisplayed
        {
            get
            {
                bool isDisplayed = false;
                if (this.BtnSaveChanges != null)
                {
                    isDisplayed = this.BtnSaveChanges.Displayed && BtnSaveChanges.Enabled;
                }
                return isDisplayed;
            }
        }

    }
}
