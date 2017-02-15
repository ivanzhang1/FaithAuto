using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using OpenQA.Selenium;

namespace FTTests.Portal.Giving {
	[TestFixture]
	public class Portal_Giving_Statements : FixtureBaseWebDriver {
		#region Statement Builder
		[Test, RepeatOnFailure, Timeout(1000)]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Matthew Sneeden")]
		[Description("Generates a statement, waits for it to process, then deletes the statement from the queue.")]
		public void Giving_Statements_StatementBuilder_CreateProcessDelete() {
            // Set initial conditions
            string statementName = "Test Statement";
            base.SQL.Execute(string.Format("UPDATE ChmContribution.dbo.STM_REQUEST SET STM_STATUS_ID = 0 WHERE CHURCH_ID = 15 AND STM_STATUS_ID != 0 AND REQUEST_DESCRIPTION LIKE '{0} - %'", statementName));

			// Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

			// Generate a statement
			test.GeneralMethods.Navigate_Portal(Navigation.Portal.Giving.Statements.Statement_Builder);
			test.Driver.FindElementById("ctl00_ctl00_MainContent_content_txtNewSettings_textBox").SendKeys(statementName);

            DateTime submittedTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            test.Driver.FindElementById(GeneralButtons.Submit).Click();

			// Allow 45 seconds for the statement(s) to be generated, verify the request completed
            Retry.Repeat(30).WithPolling(1000).WithTimeout(500000).WithFailureMessage("The statement was not completed in the alloted time.")
                .DoBetween(() => test.Driver.FindElementById("ctl00_ctl00_MainContent_content_btnRefresh").Click())
                .Until(() => test.Driver.FindElementById(TableIds.Portal.Giving_StatementQueue).FindElements(By.XPath(string.Format("//tbody/tr[*]/td[1]/span[contains(text(), '{0} - ') and ancestor::tr/td[2]/span[text()='Completed'] and (ancestor::tr/td[4]/span[text()='{1}'] or ancestor::tr/td[4]/span[text()='{2}'] or ancestor::tr/td[4]/span[text()='{3}'])]", statementName, string.Format("{0:t}", submittedTime.AddMinutes(-1)), string.Format("{0:t}", submittedTime), string.Format("{0:t}", submittedTime.AddMinutes(1))))).Count > 0);

			// Delete the contribution statement
            // test.Driver.FindElementById(TableIds.Portal.Giving_StatementQueue).FindElement(By.XPath(string.Format("//tbody/tr[*]/td[7]/a[ancestor::tr/td[4]/span/text()='{0}']", string.Format("{0:t}", submittedTime)))).Click();
            test.Driver.FindElementById(TableIds.Portal.Giving_StatementQueue).FindElement(By.Id(string.Format("ctl00_ctl00_MainContent_content_dgStatements_ctl02_lnkDelete"))).Click();

			// Verify the statement was removed
			if (test.Driver.FindElementsById(TableIds.Portal.Giving_StatementQueue).Count > 0) {
                Assert.IsFalse(test.Driver.FindElementById(TableIds.Portal.Giving_StatementQueue).FindElements(By.XPath(string.Format("//tbody/tr[*]/td[7]/a[ancestor::tr/td[4]/span/text()='{0}']", string.Format("{0:t}", submittedTime)))).Count > 0);
			}
			else {
				Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("No records found"));
			}

			// Logout of portal
			test.Portal.LogoutWebDriver();
		}
		#endregion Statement Builder

		#region Queue
		#endregion Queue

		#region Custom Styles
		#endregion Custom Styles

		#region Online Statement
		#endregion Online Statement
	}
}