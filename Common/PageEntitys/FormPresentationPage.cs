using FTTests;
using Gallio.Framework;
using MbUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.PageEntitys
{
    /// <summary>
    /// Page entity for Form Presentation Page
    /// </summary>
    public class FormPresentationPage
    {
        protected TestBaseWebDriver _testManager;

        #region input Control
        /// <summary>
        /// input text: 'Friendly Name'
        /// </summary>
        [FindsBy(How = How.Id, Using = "ctl00_ctl00_MainContent_content_txtFriendlyName")]
        public IWebElement TxtFriendlyName { get; set; }
        
        /// <summary>
        /// input text: 'Description'
        /// </summary>
        [FindsBy(How = How.Id, Using = "ctl00_ctl00_MainContent_content_txtDesc")]
        public IWebElement TxtDescription { get; set; }

        /// <summary>
        /// image Element: 'Example Image'
        /// </summary>
        [FindsBy(How = How.Id, Using = "ctl00_ctl00_MainContent_content_imgExampleImage")]
        public IWebElement ImgExampleImage { get; set; }

        /// <summary>
        /// lable Element: 'Example Name'
        /// </summary>
        [FindsBy(How = How.Id, Using = "ctl00_ctl00_MainContent_content_lblExampleName")]
        public IWebElement LblExampleName { get; set; }

        /// <summary>
        /// lable Element: 'Example Description'
        /// </summary>
        [FindsBy(How = How.Id, Using = "ctl00_ctl00_MainContent_content_lblExampleDesc")]
        public IWebElement LblExampleDesc { get; set; }

        /// <summary>
        /// button Element: 'Save Settings'
        /// </summary>
        [FindsBy(How = How.Id, Using = "ctl00_ctl00_MainContent_content_btnSaveSettings")]
        public IWebElement BtnSaveSettings { get; set; }

        public string LinkUrl { get; set; }
        #endregion

        public FormPresentationPage(TestBaseWebDriver testManager, string fromName)
        {
            testManager.Portal.EventRegistration_Select_FormName_FromTable(fromName);
            var lnkFormPresentation = testManager.Driver.FindElement(By.Id("ctl00_ctl00_MainContent_content_lnkFormPresentation"));
            var lnkUrl = testManager.Driver.FindElement(By.Id("ctl00_ctl00_MainContent_content_infellowshipUrl"));
            this.LinkUrl = lnkUrl.GetAttribute("value");
            
            lnkFormPresentation.Click();
            testManager.GeneralMethods.WaitForElement(By.XPath(".//*[@id='main_content']//*/span[text()='Edit Presentation (InFellowship Registration only)']"));

            this._testManager = testManager;
            TestLog.WriteLine("Navigate to Manage_Forms page. fromName:'{0}'", fromName);

            PageFactory.InitElements(testManager.Driver, this);
        }
        /// <summary>
        /// Action: SaveSettings
        /// </summary>
        /// <param name="friendlyName"></param>
        /// <param name="description"></param>
        public void SaveSettings(string friendlyName, string description)
        {
            this.TxtFriendlyName.Clear();
            this.TxtDescription.Clear();

            this.TxtFriendlyName.SendKeys(friendlyName);
            this.TxtDescription.SendKeys(description);
            this.BtnSaveSettings.Click();
        }

        public void ValidationSaveSettings(string actualFormName, string actualDescription)
        {
            //Check data.
            Assert.AreEqual(actualFormName, this.LblExampleName.Text, "[Portal]: 'friendly Name' update error!");
            Assert.AreEqual(actualDescription, this.LblExampleDesc.Text, "[Portal]: 'description' update error!");
        }




    }
}
