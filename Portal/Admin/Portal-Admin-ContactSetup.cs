using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace FTTests.Portal.Admin {
	[TestFixture]
	public class Portal_Admin_ContactSetup : FixtureBase {
		#region Form Names
		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Matthew Sneeden")]
		[Description("Attempts to create a form name without the required data.")]
		public void Admin_ContactSetup_FormNames_CreateNoData() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Attempt to create a form name without required data
            test.Portal.Admin_FormNames_Create(null, null);

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Creates an inactive form name.")]
		public void Admin_ContactSetup_FormNames_CreateInactive() {
            // Set initial conditions
            string formName = "Test Form Name - Inactive";
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            base.SQL.Admin_FormNamesDelete(15, formName);

			// Login to Portal    
			test.Portal.Login();

			// Create an inactive form name
            test.Portal.Admin_FormNames_Create(formName, false);

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Creates a form name.")]
		public void Admin_ContactSetup_FormNames_Create() {
            // Set initial conditions
            string formName = "Test Form Name";
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            base.SQL.Admin_FormNamesDelete(15, formName);

			// Login to Portal
			test.Portal.Login();

			// Create a form name
            test.Portal.Admin_FormNames_Create(formName, true);

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Updates a Form Name.")]
		public void Admin_ContactSetup_FormNames_Update() {
            // Set initial conditions
            string formName = "Test Form Name - Update";
            string formNameUpdated = "Test Form Name - Mod";
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            base.SQL.Admin_FormNamesDelete(15, formName);
            base.SQL.Admin_FormNamesDelete(15, formNameUpdated);
            base.SQL.Admin_FormNamesCreate(15, formName, true);

			// Login to Portal
			test.Portal.Login();

			// Update a form name
            test.Portal.Admin_FormNames_Update(formName, true, formNameUpdated, false);

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Deletes a Form Name.")]
		public void Admin_ContactSetup_FormNames_Delete() {
            // Set initial conditions
            string formName = "Test Form Name - Delete";
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            base.SQL.Admin_FormNamesDelete(15, formName);
            base.SQL.Admin_FormNamesCreate(15, formName, true);

			// Login to Portal
			test.Portal.Login();

			// Delete a form name
            test.Portal.Admin_FormNames_Delete(formName);

			// Logout of Portal
			test.Portal.Logout();
		}
		#endregion Form Names

		#region Manage Items
		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Creates a Contact Item.")]
		public void Admin_ContactSetup_ManageItems_Create() {
            // Set initial conditions
            string contactItemName = "Test Contact Item";
            base.SQL.Admin_ContactItems_Delete(15, contactItemName);

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Create a contact item
			test.Portal.Admin_ManageItems_Create(contactItemName, "Interest", "A Test Ministry", "", true, true, true);

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Matthew Sneeden")]
		[Description("Updates a Contact Item.")]
		public void Admin_ContactSetup_ManageItems_Update() {
            // Set initial conditions
            string contactItemName = "Test Contact Item - Update";
            string contactItemNameUpdated = "Test Contact Item - Mod";
            string ministryName = "A Test Ministry";
            base.SQL.Admin_ContactItems_Delete(15, contactItemName);
            base.SQL.Admin_ContactItems_Delete(15, contactItemNameUpdated);
            base.SQL.Admin_ContactItems_Create(15, contactItemName, 1, ministryName, null, true, true, true);

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Update a contact item
            test.Portal.Admin_ManageItems_Update(contactItemName, "Interest", ministryName, "", true, true, true,
				contactItemNameUpdated, "Request", ministryName, "Matthew Sneeden", false, false, false);

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Deletes a Contact Item.")]
		public void Admin_ContactSetup_ManageItems_Delete() {
            // Set initial conditions
            string contactItemName = "Test Contact Item - Delete";
            base.SQL.Admin_ContactItems_Delete(15, contactItemName);
            base.SQL.Admin_ContactItems_Create(15, contactItemName, 2, "A Test Ministry", 65211, false, false, false);

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Delete a contact item
            test.Portal.Admin_ManageItems_Delete(contactItemName);

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Searches existing Contact Items by Ministry.")]
		public void Admin_ContactSetup_ManageItems_SearchMinistry() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Navigate to admin->manage items
			test.Selenium.Navigate(Navigation.Admin.Contact_Setup.Manage_Items);

			bool processedFlag = false;

			// Search by ministry
			for (int ministryIndex = 0; ministryIndex < test.Selenium.GetXpathCount("//select[@id='ctl00_ctl00_MainContent_content_ddlbMinistryFilter_dropDownList']/option"); ministryIndex++) {
				string ministry = test.Selenium.GetText(string.Format("//select[@id='ctl00_ctl00_MainContent_content_ddlbMinistryFilter_dropDownList']/option[{0}]", ministryIndex + 1));

				if (!string.IsNullOrEmpty(ministry)) {
					test.Selenium.Select("ctl00_ctl00_MainContent_content_ddlbMinistryFilter_dropDownList", ministry);
					test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.Search);

					// Verify search results
					if (!test.Selenium.IsTextPresent("No records found")) {
						// If there are multiple pages...
						if (test.Selenium.IsElementPresent("//div[@class='grid_controls']/ul")) {
							for (int pageIndex = 1; pageIndex < test.Selenium.GetXpathCount("//div[@class='grid_controls']/ul/li"); pageIndex++) {
								test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", pageIndex));
                                for (int row = 1; row < test.GeneralMethods.GetTableRowCount(TableIds.Admin_ManageItems); row++) {
                                    Assert.AreEqual(ministry, test.Selenium.GetTable(string.Format("{0}.{1}.3", TableIds.Admin_ManageItems, row)));
                                }
							}
							processedFlag = true;
						}
						else {
                            for (int row = 1; row < test.GeneralMethods.GetTableRowCount(TableIds.Admin_ManageItems); row++) {
                                Assert.AreEqual(ministry, test.Selenium.GetTable(string.Format("{0}.{1}.3", TableIds.Admin_ManageItems, row)));
                            }
							processedFlag = true;
						}
					}
				}
                if (processedFlag) {
                    break;
                }
			}

			// Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Searches existing inactive Contact Items.")]
		public void Admin_ContactSetup_ManageItems_SearchInactive() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Navigate to admin->manage items
			test.Selenium.Navigate(Navigation.Admin.Contact_Setup.Manage_Items);

			// Search by inactive
			test.Selenium.Select("ctl00_ctl00_MainContent_content_ddlActive_dropDownList", "Inactive");
			test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.Search);

			// If there are multiple pages...
			if (test.Selenium.IsElementPresent("//div[@class='grid_controls']/ul")) {
				for (int pageIndex = 1; pageIndex < test.Selenium.GetXpathCount("//div[@class='grid_controls']/ul/li"); pageIndex++) {
					test.Selenium.ClickAndWaitForPageToLoad(string.Format("link={0}", pageIndex));
                    for (int row = 1; row < test.GeneralMethods.GetTableRowCount(TableIds.Admin_ManageItems); row++) {
                        Assert.IsFalse(test.Selenium.IsElementPresent(string.Format("{0}/tbody/tr[{1}]/td[8]/img", TableIds.Admin_ManageItems, row + 1)));
                    }
				}
			}
			else {
				for (int row = 1; row < test.GeneralMethods.GetTableRowCount(TableIds.Admin_ManageItems); row++) {
                    Assert.IsFalse(test.Selenium.IsElementPresent(string.Format("{0}/tbody/tr[{1}]/td[8]/img", TableIds.Admin_ManageItems, row + 1)));
				}
			}

			// Logout of portal
			test.Portal.Logout();
		}
		#endregion Manage Items

		#region Build Forms
		#endregion Build Forms

		#region Contact Dispositions
		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Creates a Contact Disposition")]
		public void Admin_ContactSetup_ContactDispositions_Create() {
            // Set initial conditions
            string contactDispositionName = "Test Contact Disposition";
            base.SQL.Admin_ContactDispositionsDelete(15, contactDispositionName);

			// Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Create a contact disposition
            test.Portal.Admin_ContactDispositions_Create(contactDispositionName, true, false, true);

			// Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Matthew Sneeden")]
		[Description("Updates a Contact Disposition")]
		public void Admin_ContactSetup_ContactDispositions_Update() {
            // Set initial conditions
            string contactDispositionName = "Test Contact Disposition - Update";
            string contactDispositionNameUpdated = "Test Contact Disposition - Mod";
            base.SQL.Admin_ContactDispositionsDelete(15, contactDispositionName);
            base.SQL.Admin_ContactDispositionsDelete(15, contactDispositionNameUpdated);
            base.SQL.Admin_ContactDispositionsCreate(15, contactDispositionName, true, true, true);

			// Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Update a contact disposition
            test.Portal.Admin_ContactDispositions_Update(contactDispositionName, true, false, true, contactDispositionNameUpdated, true, true, false);

			// Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Deletes a Contact Disposition")]
		public void Admin_ContactSetup_ContactDispositions_Delete() {
            // Set initial conditions
            string contactDispositionName = "Test Contact Disposition - Delete";
            base.SQL.Admin_ContactDispositionsDelete(15, contactDispositionName);
            base.SQL.Admin_ContactDispositionsCreate(15, contactDispositionName, false, false, true);

			// Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Delete a contact disposition
			test.Portal.Admin_ContactDispositions_Delete(contactDispositionName);

			// Logout of portal
			test.Portal.Logout();
		}
		#endregion Contact Dispositions
    }
}