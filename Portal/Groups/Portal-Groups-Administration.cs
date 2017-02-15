using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Firefox;

namespace FTTests.Portal.Groups {
    [TestFixture]
    public class Portal_Groups_Administration_CustomFields : FixtureBase {

        private string _customFieldName = "General Custom Field";
        private List<string> _customFieldChoices = new List<string>() { "A", "B", "C" };

        #region FixtureSetUp
        [FixtureSetUp]
        public void FixtureSetUp() {
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.MultiSelect, _customFieldName, null, _customFieldChoices);
        }
        #endregion FixtureSetUp

        #region FixtureTearDown
        [FixtureTearDown]
        public void FixtureTearDown() {
            base.SQL.Groups_CustomField_Delete(254, _customFieldName);
        }
        #endregion FixtureTearDown

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Creates a Single Select Custom Field.")]
        public void Groups_Administration_CustomFields_Create_Single() {
            // Initial data
            var customFieldName = "Test Single Select Custom Field";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a custom field         
            test.Portal.Groups_CustomField_Create(customFieldName, null, GeneralEnumerations.CustomFieldType.SingleSelect, new string[] { "1", "2", "3" });

            // Logout of portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Updates a Single selectCustom Field's name, description, and type.")]
        public void Groups_Administration_CustomFields_Edit_Details_Single() {
            // Intiial data
            var customFieldName = "Update Custom Field - Single Select";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.SingleSelect, customFieldName, null, new List<string> { "1", "2", "3" });

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Update a Single Select custom field
            test.Portal.Groups_CustomField_Update_Field_Details(customFieldName, null, "Single select", "Test Single Select Custom Field UPDATED", "Added description", "Multi-select");

            // Logout of portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, "Test Single Select Custom Field UPDATED");
        }

        
        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Deletes a Single Select Custom Field.")]
        public void Groups_Administration_CustomFields_Delete_Single() {
            // Intiial data
            var customFieldName = "Delete Custom Field - Single Select";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.SingleSelect, customFieldName, null, new List<string> { "1", "2", "3" });

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Delete a custom field
            test.Portal.Groups_CustomField_Delete(customFieldName);

            // Logout of portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Creates a Multi Select Custom Field.")]
        public void Groups_Administration_CustomFields_Create_Multi() {
            // Initial data
            var customFieldName = "Test Multi Select Custom Field";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a custom field
            test.Portal.Groups_CustomField_Create(customFieldName, null, GeneralEnumerations.CustomFieldType.MultiSelect, new string[] { "1", "2", "3" });

            // Logout of portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian", "bmikaelian@fellowshiptech.com")]
        [Description("Updates a Multi Select Custom Field's name, description, and type.")]
        public void Groups_Administration_CustomFields_Edit_Details_Multi() {
            // Initial data
            var customFieldName = "Test Multi Select Custom Field - Update";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.MultiSelect, customFieldName, null, new List<string> { "1", "2", "3" });

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Update a multi select custom field
            test.Portal.Groups_CustomField_Update_Field_Details(customFieldName, null, "Multi-select", "Test Multi Select Custom Field UPDATED", "Added description", "Single select");

            // Logout of portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, "Test Multi Select Custom Field UPDATED");
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Deletes a Multi Select Custom Field.")]
        public void Groups_Administration_CustomFields_Delete_Multi() {
            // Initial data
            var customFieldName = "Test Multi Select Custom Field - Delete";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.MultiSelect, customFieldName, null, new List<string> { "1", "2", "3" });

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Delete a custom field
            test.Portal.Groups_CustomField_Delete(customFieldName);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Creates a Single Select Custom Field with special characters.")]
        public void Groups_Administration_CustomFields_Create_Special_Characters_Single() {
            // Initial data
            var customFieldName = "@Test & $ingle & Select \"Custom\" - Field";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a custom field
            test.Portal.Groups_CustomField_Create(customFieldName, null, GeneralEnumerations.CustomFieldType.SingleSelect, new string[] { "1", "2", "3" });

            // Logout of portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Deletes a Single Select Custom Field with special characters.")]
        public void Groups_Administration_CustomFields_Delete_Special_Characters_Single() {
            // Intiial data
            var customFieldName = "@Test & $ingle & Select \"Custom\" - Field - Delete";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.SingleSelect, customFieldName, null, new List<string> { "1", "2", "3" });


            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Delete a custom field
            test.Portal.Groups_CustomField_Delete(customFieldName);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian", "bmikaelian@fellowshiptech.com")]
        [Description("Creates a Multi Select Custom Field with special characters.")]
        public void Groups_Administration_CustomFields_Create_Special_Characters_Multi() {
            // Initial data
            var customFieldName = "@Test & $Multi & Select \"Custom\" - Field";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a custom field
            test.Portal.Groups_CustomField_Create(customFieldName, null, GeneralEnumerations.CustomFieldType.MultiSelect, new string[] { "1", "2", "3" });

            // Logout of portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Deletes a Single selectCustom Field with special characters.")]
        public void Groups_Administration_CustomFields_Delete_Special_Characters_Multi() {
            // Intiial data
            var customFieldName = "@Test & $Multi & Select \"Custom\" - Field - Delete";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.SingleSelect, customFieldName, null, new List<string> { "1", "2", "3" });

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Delete a custom field
            test.Portal.Groups_CustomField_Delete(customFieldName);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a portal user that is not a Group Type Admin cannot create a custom field.")]
        public void Groups_Administration_CustomFields_Create_Non_GTAs_Cannot_Create() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "QAEUNLX0C2");

            // Verify Custom Field link isn't present
            test.Selenium.Click("link=Groups");
            test.Selenium.VerifyElementNotPresent("link=Custom Fields");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a name is required when creating a custom field.")]
        public void Groups_Administration_CustomFields_Create_Name_Required() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->custom fields
            test.Selenium.Navigate(Navigation.Groups.Administration.Custom_Fields);

            // Add a new custom field
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Add);

            // Pick a field type, but don't specify a name
            test.Selenium.Select("field_type", "Single select");

            // Verify the validation
            test.Selenium.ClickAndWaitForPageToLoad("submit_custom_field");
            test.Selenium.VerifyTextPresent("Field label is required and cannot exceed 50 characters.");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you cannot create a custom field with a duplicate name.")]
        public void Groups_Administration_CustomFields_Create_Duplicate() {
            // Intiial data
            var customFieldName = "Existing Custom Field";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.SingleSelect, customFieldName, null, new List<string> { "1", "2", "3" });

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->custom fields
            test.Selenium.Navigate(Navigation.Groups.Administration.Custom_Fields);

            // Add a new custom field
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Add);

            // Specify a duplicate name
            test.Selenium.Type("custom_field_name", customFieldName);

            // Pick a field type
            test.Selenium.Select("field_type", "Single select");

            // Verify the validation
            test.Selenium.ClickAndWaitForPageToLoad("submit_custom_field");
            test.Selenium.VerifyTextPresent("The field label is not unique.");
            
            // Logout of portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you cannot create a custom field with a name that exceeds 50 characters.")]
        public void Groups_Administration_CustomFields_Create_Name_Exceed_50_Characters() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->custom fields
            test.Selenium.Navigate(Navigation.Groups.Administration.Custom_Fields);

            // Add a new custom field
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Add);

            // Specify a long name
            test.Selenium.Type("custom_field_name", "Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field ");

            // Pick a field type
            test.Selenium.Select("field_type", "Single select");

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad("submit_custom_field");

            // Verify text
            Assert.IsTrue(test.Selenium.IsTextPresent("Field label is required and cannot exceed 50 characters."));

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you cannot create a custom field with a description that exceeds 500 characters.")]
        public void Groups_Administration_CustomFields_Create_Description_Exceed_500_Characters() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->custom fields
            test.Selenium.Navigate(Navigation.Groups.Administration.Custom_Fields);

            // Add a new custom field
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Add);

            // Specify a name
            test.Selenium.Type("custom_field_name", "Some Custom Field");

            // Pick a field type
            test.Selenium.Select("field_type", "Single select");

            // Enter a description that exceeds 500 characters
            test.Selenium.Type("custom_field_description", "Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. ");

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad("submit_custom_field");

            // Verify text
            Assert.IsTrue(test.Selenium.IsTextPresent("Field description cannot exceed 500 characters. "), "Validation message was not present and/or incorrect.");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies if a description entered on Step 1, it is present on Step 2.")]
        public void Groups_Administration_CustomFields_Create_Description_Present_On_Step_2() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->custom fields
            test.Selenium.Navigate(Navigation.Groups.Administration.Custom_Fields);

            // Add a new custom field
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Add);

            // Specify a name
            test.Selenium.Type("custom_field_name", "Some Custom Field");

            // Pick a field type
            test.Selenium.Select("field_type", "Single select");

            // Enter a description 
            test.Selenium.Type("custom_field_description", "Some description");

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad("submit_custom_field");

            // Verify description is present on Step 2
            Assert.AreEqual("Some description", test.Selenium.GetText(GroupsAdministrationConstants.Wizard_CustomField.Step2_Text_CustomFieldDescription), "Description text was not present and/or correct");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a field type is required when creating a custom field.")]
        public void Groups_Administration_CustomFields_Create_Field_Type_Required() {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->custom fields
            test.Selenium.Navigate(Navigation.Groups.Administration.Custom_Fields);

            // Add a new custom field
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Add);

            // Specify a name
            test.Selenium.Type("custom_field_name", "Some Custom Field");

            // Don't pick a field type

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad("submit_custom_field");

            // Verify message
            Assert.IsTrue(test.Selenium.IsTextPresent("Please select a valid field type."), "Validation message is not present and / or is incorrect.");

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the option text is required when creating a custom field.")]
        public void Groups_Administration_CustomFields_Create_Option_Text_Required() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->custom fields
            test.Selenium.Navigate(Navigation.Groups.Administration.Custom_Fields);

            // Add a new custom field
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Add);

            // Specify a name
            test.Selenium.Type("custom_field_name", "Some Custom Field");

            // Pick a field type
            test.Selenium.Select("field_type", "Single select");

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad("submit_custom_field");

            // Don't specify any text for the options
            test.Selenium.ClickAndWaitForPageToLoad("submit_custom_field");

            // Verify validation message
            Assert.IsTrue(test.Selenium.IsTextPresent("Option Text is required and cannot exceed 100 characters."), "Verification message was not present and/or incorrect.");

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the option text cannot exceed 100 characters when creating a custom field.")]
        public void Groups_Administration_CustomFields_Create_Option_Text_Exceed_100_Characters() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->custom fields
            test.Selenium.Navigate(Navigation.Groups.Administration.Custom_Fields);

            // Add a new custom field
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Add);

            // Specify a name
            test.Selenium.Type("custom_field_name", "Some Custom Field");

            // Pick a field type
            test.Selenium.Select("field_type", "Single select");

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad("submit_custom_field");

            // Specify text that exceeds 100 characters for one of options
            test.Selenium.Type("new_option_field_text_0", "field name field name field name field name field name field name field name field name field name field name field name field name");

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad("submit_custom_field");

            // Verify validation message
            Assert.IsTrue(test.Selenium.IsTextPresent("Option Text is required and cannot exceed 100 characters."), "Verification message was not present and/or incorrect.");

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Edits the options of a Single selectcustom field and verifies they apply to the custom field.")]
        public void Groups_Administration_CustomFields_Edit_Choices_Single() {
            // Intiial data
            var customFieldName = "Edit Choices - Single";
            string[] updateFieldValues = new string[] { "Update 1", "Update 2", "Update 3" };
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.SingleSelect, customFieldName, null, new List<string> { "1", "2", "3" });

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Change the choices of a custom field
            test.Portal.Groups_CustomField_Update_Field_Choices(customFieldName, updateFieldValues);

            // Edit the choices again
            test.Portal.Groups_CustomField_View_Edit_Values(customFieldName);

            // Verify the update was applied
            for (int i = 0; i < updateFieldValues.Length; i++) {
                Assert.AreEqual(test.Selenium.GetValue(string.Format("//ul[@id='custom_field_select_list']/li[{0}]/input[1]", i + 1)), updateFieldValues[i]);
            }

            // Return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout of portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Edits the options of a Single selectcustom field and verifies they apply to the custom field.")]
        public void Groups_Administration_CustomFields_Edit_Choices_Multi() {
            // Intiial data
            var customFieldName = "Edit Choices - Multi";
            string[] updateFieldValues = new string[] { "Update 1", "Update 2", "Update 3" };
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.MultiSelect, customFieldName, null, new List<string> { "1", "2", "3" });

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Change the choices of a custom field
            test.Portal.Groups_CustomField_Update_Field_Choices(customFieldName, updateFieldValues);

            // Edit the choices again
            test.Portal.Groups_CustomField_View_Edit_Values(customFieldName);

            // Verify the update was applied
            for (int i = 0; i < updateFieldValues.Length; i++) {
                Assert.AreEqual(test.Selenium.GetValue(string.Format("//ul[@id='custom_field_select_list']/li[{0}]/input[1]", i + 1)), updateFieldValues[i]);
            }

            // Return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout of portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a name is required when editing a custom field.")]
        public void Groups_Administration_CustomFields_Edit_Name_Exceed_50_Characters() {
            // Setup data
            var _customFieldName1 = string.Format("New 12x General Custom Field - {0}", new Random().Next(1000).ToString()); ;
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.MultiSelect, _customFieldName1, null, _customFieldChoices);
            
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");


            // Edit a custom field's details
            test.Portal.Groups_CustomField_View_Edit_Details(_customFieldName1);

            // Specify a long name
            test.Selenium.Type("custom_field_name", "Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field Some Custom Field ");

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad("submit_custom_field");

            // Verify message
            test.Selenium.VerifyTextPresent("Field label is required and cannot exceed 50 characters.");

            // Logout of Portal
            test.Portal.Logout();

            // Clean up?
            base.SQL.Groups_CustomField_Delete(254, _customFieldName1);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a duplicate name cannot be entered when editing a custom field.")]
        public void Groups_Administration_CustomFields_Edit_Name_Duplicate() {
            // Intiial data
            var _customFieldName1 = string.Format("New x2 General Custom Field - {0}", new Random().Next(1000).ToString()); ;
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.MultiSelect, _customFieldName1, null, _customFieldChoices);
            var customFieldName = "Existing Name - Custom Field";
            string[] updateFieldValues = new string[] { "Update 1", "Update 2", "Update 3" };
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.MultiSelect, customFieldName, null, new List<string> { "1", "2", "3" });

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Edit a custom field's details
            test.Portal.Groups_CustomField_View_Edit_Details(_customFieldName1);

            // Enter a duplicate name
            test.Selenium.Type("custom_field_name", customFieldName);

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad("submit_custom_field");

            // Verify text
            test.Selenium.VerifyTextPresent("The field label is not unique.");

            // Logout of Portal
            test.Portal.Logout();

            // Clean up?
            base.SQL.Groups_CustomField_Delete(254, _customFieldName1);
            //base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a description cannot exceed 500 characters when editing a custom field.")]
        public void Groups_Administration_CustomFields_Edit_Description_Exceed_500_Characters() {
            // Set conditions
            var _customFieldName1 = string.Format("New 13x General Custom Field - {0}", new Random().Next(1000).ToString()); ;
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.MultiSelect, _customFieldName1, null, _customFieldChoices);

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Edit a custom field's details
            test.Portal.Groups_CustomField_View_Edit_Details(_customFieldName1);

            // Enter a description that exceeds 500 characters
            test.Selenium.Type("custom_field_description", "Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. Long description. ");

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad("submit_custom_field");

            // Verify text
            test.Selenium.VerifyTextPresent("Field description cannot exceed 500 characters.");

            // Logout of Portal
            test.Portal.Logout();

            // Clean up?
            base.SQL.Groups_CustomField_Delete(254, _customFieldName1);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a name is required when editing a custom field.")]
        public void Groups_Administration_CustomFields_Edit_Name_Required() {
            // Set conditions
            var _customFieldName1 = string.Format("New 14x General Custom Field - {0}", new Random().Next(1000).ToString());
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.MultiSelect, _customFieldName1, null, _customFieldChoices);
            
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Edit a custom field's details
            test.Portal.Groups_CustomField_View_Edit_Details(_customFieldName1);

			// Don't specify a name
			test.Selenium.Type("custom_field_name", "");

			// Submit
			test.Selenium.ClickAndWaitForPageToLoad("submit_custom_field");

            // Verify message
            test.Selenium.VerifyTextPresent("Field label is required and cannot exceed 50 characters.");

            // Logout of Portal
            test.Portal.Logout();

            // Clean up?
            base.SQL.Groups_CustomField_Delete(254, _customFieldName1);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a field type is required when editing a custom field.")]
        public void Groups_Administration_CustomFields_Edit_Field_Type_Required() {
            // Set conditions
            var _customFieldName1 = string.Format("New 15x General Custom Field {0}", new Random().Next(1000).ToString());
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.MultiSelect, _customFieldName1, null, _customFieldChoices);

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Edit a custom field's details
            test.Portal.Groups_CustomField_View_Edit_Details(_customFieldName1);

            // Don't pick a field type
            test.Selenium.Select("field_type", "-- Please choose --");

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad("submit_custom_field");

            // Verify message
            test.Selenium.VerifyTextPresent("Please select a valid field type.");

            // Logout of Portal
            test.Portal.Logout();

            // Clean up?
            base.SQL.Groups_CustomField_Delete(254, _customFieldName1);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the option text is required when editing a custom field.")]
        public void Groups_Administration_CustomFields_Edit_Option_Text_Required() {
            // Set conditions
            var _customFieldName1 = "QA2-QA General Custom Field";
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.MultiSelect, _customFieldName1, null, _customFieldChoices);
            base.SQL.Groups_CustomField_Create(15, GeneralEnumerations.CustomFieldType.MultiSelect, _customFieldName1, null, _customFieldChoices);
            // Variables
            string[] nullValues = new string[] { "", "" };

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");
            test.Portal.Login("ft.tester", "FT4life!", "dc");

            // Don't specify any text for the options            
            test.Portal.Groups_CustomField_Update_Field_Choices(_customFieldName1, nullValues);

            // Verify validation message
            test.Selenium.VerifyTextPresent("Option Text is required and cannot exceed 100 characters.");

            // Logout of Portal
            test.Portal.Logout();

            // Clean up?
            base.SQL.Groups_CustomField_Delete(15, _customFieldName1);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the option text cannot exceed 100 characters when editing a custom field.")]
        public void Groups_Administration_CustomFields_Edit_Option_Text_Exceed_100_Characters() {
            // Set conditions
            var _customFieldName1 = string.Format("{0} New 100 General Custom Field", new Random().Next(1000).ToString());
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.MultiSelect, _customFieldName1, null, _customFieldChoices);
            
            // Variables
            string[] longNames = new string[] { "Super long name Super long name Super long name Super long name Super long name Super long name Super long name Super long name Super long name Super long name Super long name Super long name ", "Super long name Super long name Super long name Super long name Super long name Super long name Super long name Super long name Super long name Super long name Super long name Super long name Super long name Super long name Super long name Super long name Super long name Super long name Super long name Super long name Super long name Super long name Super long name Super long name " };

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Enter really long names for the options            
            test.Portal.Groups_CustomField_Update_Field_Choices(_customFieldName1, longNames);

            // Verify validation message
            test.Selenium.VerifyTextPresent("Option Text is required and cannot exceed 100 characters.");

            // Logout of Portal
            test.Portal.Logout();

            // Clean up?
            base.SQL.Groups_CustomField_Delete(254, _customFieldName1);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a name cannot be edited when a custom field is in use.")]
        public void Groups_Administration_CustomFields_Edit_Name_Not_Possible_If_In_Use() {
            // Initial Data
            var customFieldName = "In Use - Edit Name";
            var groupTypeName = "In Use - Edit Name Group Type";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int>() { 11 });
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.SingleSelect, customFieldName, null, new List<string>() { "A" });
            base.SQL.Groups_GroupTypes_AddCustomField(254, groupTypeName, customFieldName);

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Edit a custom field's details
            test.Portal.Groups_CustomField_View_Edit_Details(customFieldName);

            // Verify the name text field control is not present
            Assert.IsFalse(test.Selenium.IsElementPresent("custom_field_name"), "Editing the name was allowed while custom field was in use!");

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the custom field type cannot be edited when a custom field is in use.")]
        public void Groups_Administration_CustomFields_Edit_Type_Not_Possible_If_In_Use() {
            // Initial Data
            var customFieldName = "In Use - Edit Type";
            var groupTypeName = "In Use - Edit Type Group Type";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int>() { 11 });
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.SingleSelect, customFieldName, null, new List<string>() { "A" });
            base.SQL.Groups_GroupTypes_AddCustomField(254, groupTypeName, customFieldName);

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Edit a custom field's details
            test.Portal.Groups_CustomField_View_Edit_Details(customFieldName);

            // Verify the custom field type drop down control is not present
            Assert.IsFalse(test.Selenium.IsVisible("field_type"), "Editing the field type was allowed!");

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the custom field description can be edited when a custom field is in use.")]
        public void Groups_Administration_CustomFields_Edit_Description_Possible_If_In_Use() {
            // Initial Data
            var customFieldName = "In Use - Edit Desc";
            var groupTypeName = "In Use - Edit Desc Group Type";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int>() { 11 });
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.SingleSelect, customFieldName, null, new List<string>() { "A" });
            base.SQL.Groups_GroupTypes_AddCustomField(254, groupTypeName, customFieldName);

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Edit a custom field's details
            test.Portal.Groups_CustomField_View_Edit_Details(customFieldName);

            // Verify the custom field description text field control is present
            test.Selenium.VerifyElementPresent("Textarea1");

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the custom field choice's names can not be edited when a custom field is in use.")]
        public void Groups_Administration_CustomFields_Edit_Choices_Name_Not_Possible_If_In_Use() {
            // Initial Data
            var customFieldName = "In Use - Edit Choice Name";
            var groupTypeName = "In Use - Edit Choice Name Group Type";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int>() { 11 });
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.SingleSelect, customFieldName, null, new List<string>() { "A" });
            base.SQL.Groups_GroupTypes_AddCustomField(254, groupTypeName, customFieldName);


            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Edit a custom field's details
            test.Portal.Groups_CustomField_View_Edit_Details(customFieldName);

            // Verify the choices text field control is not present
            Assert.IsFalse(test.Selenium.IsElementPresent("//ul[@id='custom_field_select_list']/li/input"));

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the custom field can gain additional choices a custom field is in use.")]
        public void Groups_Administration_CustomFields_Edit_Choices_Add_Possible_If_In_Use() {
            // Initial Data
            var customFieldName = "In Use - Add Choice";
            var groupTypeName = "In Use - Add Choice Name Group Type";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int>() { 11 });
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.SingleSelect, customFieldName, null, new List<string>() { "A" });
            base.SQL.Groups_GroupTypes_AddCustomField(254, groupTypeName, customFieldName);

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Edit a custom field's details
            test.Portal.Groups_CustomField_View_Edit_Details(customFieldName);

            // Verify the add choices link is present
            test.Selenium.IsElementPresent("link=Add another");

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);

        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the custom field can be deleted if a custom field is in use.")]
        public void Groups_Administration_CustomFields_Delete_Possible_If_In_Use() {
            // Initial Data
            var customFieldName = "In Use - Delete";
            var groupTypeName = "In Use - Delete Group Type";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int>() { 11 });
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.SingleSelect, customFieldName, null, new List<string>() { "A" });
            base.SQL.Groups_GroupTypes_AddCustomField(254, groupTypeName, customFieldName);

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->custom fields
            test.Selenium.Navigate(Navigation.Groups.Administration.Custom_Fields);

            decimal itemRow = test.GeneralMethods.GetTableRowNumber(TableIds.Groups_CustomFields, customFieldName, "Custom Field", null);
            Assert.IsTrue(test.Selenium.IsElementPresent(string.Format("{0}/tbody/tr[{1}]/td[4]/ul/li[3]/a", TableIds.Groups_CustomFields, itemRow + 1)), "Delete was not possible!");

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies an additional single select value can be created for a single select custom field during creation.")]
        public void Groups_Administration_CustomFields_Create_Add_Additional_Single_Select_Value() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Creates a custom field with an additional value beyond the first 3.
            test.Portal.Groups_CustomField_Create("*Extra Single Custom Field", null, GeneralEnumerations.CustomFieldType.SingleSelect, new string[] { "a", "b", "c", "d" });

            // View the custom field's choices
            test.Portal.Groups_CustomField_View_Choices("*Extra Single Custom Field");

            // Verify the extra choice is there
            test.Portal.Groups_CustomField_View_VerifyChoiceExists("d");

            // Return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Delete the custom field
            test.Portal.Groups_CustomField_Delete("*Extra Single Custom Field");

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies an additional multi select value can be created for a multi select custom field during creation.")]
        public void Groups_Administration_CustomFields_Create_Add_Additional_Multi_Select_Value() {
            // Intiial data
            var customFieldName = "Additional Multi Select Value";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Creates a custom field with an additional value beyond the first 3.
            test.Portal.Groups_CustomField_Create(customFieldName, null, GeneralEnumerations.CustomFieldType.MultiSelect, new string[] { "a", "b", "c", "d" });

            // View the custom field's choices
            test.Portal.Groups_CustomField_View_Choices(customFieldName);

            // Verify the extra choice is there
            test.Portal.Groups_CustomField_View_VerifyChoiceExists("d");

            // Return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies an additional single select value can be created for a single select custom field .")]
        public void Groups_Administration_CustomFields_Edit_Add_Additional_Single_Select_Value() {
            // Intiial data
            var customFieldName = "Additional Single Select Value";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.SingleSelect, customFieldName, null, new List<string> { "1", "2", "3" });

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Add a new custom field value
            test.Portal.Groups_CustomField_Create_Choice(customFieldName, "d");

            // View the field choices
            test.Portal.Groups_CustomField_View_Choices(customFieldName);

            // Verify new choice is there
            test.Portal.Groups_CustomField_View_VerifyChoiceExists("d");

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a single select value can be updated for a single select custom field.")]
        public void Groups_Administration_CustomFields_Edit_Update_Single_Select_Value() {
            // Intiial data
            var customFieldName = "Update Additional Single Select Value";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.SingleSelect, customFieldName, null, new List<string> { "1", "2", "3" });

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Update a new custom field value
            test.Portal.Groups_CustomField_Update_Field_Choice(customFieldName, "3", "3 UPDATED");

            // View the field choices
            test.Portal.Groups_CustomField_View_Choices(customFieldName);

            // Verify the updated choice is there
            test.Portal.Groups_CustomField_View_VerifyChoiceExists("3 UPDATED");

            // Verify the deleted choice is not there
            test.Selenium.VerifyElementNotPresent("//input[@value='3']");

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a single select value can be delete for a single select custom field.")]
        public void Groups_Administration_CustomFields_Edit_Delete_Single_Select_Value() {
            // Intiial data
            var customFieldName = "Delete Single Select Value";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.SingleSelect, customFieldName, null, new List<string> { "1", "2", "3" });

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Delete a new custom field value
            test.Portal.Groups_CustomField_Delete_Choice(customFieldName, "3");

            // View the field choices
            test.Portal.Groups_CustomField_View_Choices(customFieldName);

            // Verify the deleted choice is not there
            test.Selenium.VerifyElementNotPresent("//input[@value='3']");

            // Return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies an additional single select value can be created for a multi select custom field.")]
        public void Groups_Administration_CustomFields_Edit_Create_Additional_Multi_Select_Value() {
            // Intiial data
            var customFieldName = "Additional Multi Select Value - Add";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.MultiSelect, customFieldName, null, new List<string> { "1", "2", "3" });

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Add a new custom field value
            test.Portal.Groups_CustomField_Create_Choice(customFieldName, "4");

            // View the field choices
            test.Portal.Groups_CustomField_View_Choices(customFieldName);

            // Verify new choice is there
            test.Portal.Groups_CustomField_View_VerifyChoiceExists("4");

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies an additional single select value can be updated for a multi select custom field.")]
        public void Groups_Administration_CustomFields_Edit_Update_Multi_Select_Value() {
            // Intiial data
            var customFieldName = "Additional Multi Select Value - Update";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.MultiSelect, customFieldName, null, new List<string> { "1", "2", "3" });

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Update a new custom field value
            test.Portal.Groups_CustomField_Update_Field_Choice(customFieldName, "3", "3 UPDATED");

            // View the field choices
            test.Portal.Groups_CustomField_View_Edit_Values(customFieldName);

            // Verify the updated choice is there
            test.Portal.Groups_CustomField_View_VerifyChoiceExists("3 UPDATED");

            // Verify the deleted choice is not there
            test.Selenium.VerifyElementNotPresent("//input[@value='3']");

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies an additional single select value can be delete for a multi select custom field.")]
        public void Groups_Administration_CustomFields_Edit_Delete_Additional_Multi_Select_Value() {
            // Intiial data
            var customFieldName = "Additional Multi Select Value - Delete";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.MultiSelect, customFieldName, null, new List<string> { "1", "2", "3" });

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Delete a new custom field value
            test.Portal.Groups_CustomField_Delete_Choice(customFieldName, "3");

            // View the field choices
            test.Portal.Groups_CustomField_View_Choices(customFieldName);

            // Verify the deleted choice is not there
            test.Selenium.VerifyElementNotPresent("//input[@value='3']");

            // Return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }
    }

    [TestFixture]
    public class Portal_Groups_Administration_SearchCategories : FixtureBase {
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Creates a published Search Category.")]
        public void Groups_Administration_SearchCategories_Published_Create() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a search category
            test.Portal.Groups_SearchCategory_Create("Test Search Category", null, true, new string[] { "Gender" });

            // Logout of Portal
            test.Portal.Logout();

            // Open InFellowship
            test.infellowship.Open();

            // Find a Group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LoginPage.Link_FindAGroupHere);

            // Verify Search Category Exists in InFellowship
            Assert.IsTrue(test.Selenium.IsElementPresent("//select[@id='category']/option[text()='" + "Test Search Category" + "']"));
        }

        [Test, RepeatOnFailure]
        [DependsOn("Groups_Administration_SearchCategories_Published_Create")]
        [Author("Bryan Mikaelian")]
        [Description("Updates the details of a published Search Category.")]
        public void Groups_Administration_SearchCategories_Published_Update_Details() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Update the details of a published search category
            test.Portal.Groups_SearchCategory_Update_Details("Test Search Category", "Test UPDATED Search Category", null);

            // Navigate to Groups->Administration->Search Categories
            test.Selenium.Navigate(Navigation.Groups.Administration.Search_Categories);

            // Logout of portal
            test.Portal.Logout();

            // Open InFellowship
            test.infellowship.Open();

            // Find a Group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LoginPage.Link_FindAGroupHere);
            
            // Verify Search Category Exists in InFellowship
            Assert.IsTrue(test.Selenium.IsElementPresent("//select[@id='category']/option[text()='" + "Test UPDATED Search Category" + "']"));
        }

        [Test, RepeatOnFailure]
        [DependsOn("Groups_Administration_SearchCategories_Published_Update_Details")]
        [Author("Bryan Mikaelian")]
        [Description("Updates the criteria of a published Search Category.")]
        public void Groups_Administration_SearchCategories_Published_Update_Criteria() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Update the details of a published search category, removing the gender criteria and add the marital status and childcare criteria  
            test.Portal.Groups_SearchCategory_Update_Criteria("Test UPDATED Search Category", new GeneralEnumerations.SearchCategoryCriteria[] { GeneralEnumerations.SearchCategoryCriteria.Gender, GeneralEnumerations.SearchCategoryCriteria.MaritalStatus, GeneralEnumerations.SearchCategoryCriteria.Childcare });

            // Edit the criteria of the search category
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Link_EditSeachCategoryCriteria);

            // Verify changes (Gender should not be checked but marital status/childcare should)
            Assert.IsFalse(test.Selenium.IsChecked(TableIds.Groups_SearchCategories + "/tbody/tr[3]/td[1]/p/input"));
            Assert.IsTrue(test.Selenium.IsChecked(TableIds.Groups_SearchCategories + "/tbody/tr[4]/td[1]/p/input"));
            Assert.IsTrue(test.Selenium.IsChecked(TableIds.Groups_SearchCategories + "/tbody/tr[5]/td[1]/p/input"));

            // Return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [DependsOn("Groups_Administration_SearchCategories_Published_Update_Criteria")]
        [Author("Bryan Mikaelian")]
        [Description("Deletes a published Search Category.")]
        public void Groups_Administration_SearchCategories_Published_Delete() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Delete a published search category
            test.Portal.Groups_SearchCategory_Delete("Test UPDATED Search Category");


            // Logout of portal
            test.Portal.Logout();

            // Open InFellowship
            test.infellowship.Open();

            // Find a Group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LoginPage.Link_FindAGroupHere);
            
            // Verify Search Category Exists in InFellowship
            Assert.IsFalse(test.Selenium.IsElementPresent("//select[@id='category']/option[text()='" + "Test UPDATED Search Category" + "']"));
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Creates an unpublished Search Category.")]
        public void Groups_Administration_SearchCategories_Unpublished_Create() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Create a search category
            test.Portal.Groups_SearchCategory_Create("Test Unpublished Search Category", null, false, new string[] { "Gender" });

            // Logout of Portal
            test.Portal.Logout();

            // Open InFellowship
            test.infellowship.Open();

            // Find a Group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LoginPage.Link_FindAGroupHere);
            
            // Verify Search Category does not exist in InFellowship
            Assert.IsFalse(test.Selenium.IsElementPresent("//select[@id='category']/option[text()='" + "Test Unpublished Search Category" + "']"));
        }

        [Test, RepeatOnFailure]
        [DependsOn("Groups_Administration_SearchCategories_Unpublished_Create")]
        [Author("Bryan Mikaelian")]
        [Description("Updates the details of an unpublished Search Category.")]
        public void Groups_Administration_SearchCategories_Unpublished_Update_Details() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Update the details of an unpublished search category
            test.Portal.Groups_SearchCategory_Update_Details("Test Unpublished Search Category", "Test UPDATED Unpublished Search Category", null);

            // Logout of portal
            test.Portal.Logout();

            // Open InFellowship
            test.infellowship.Open();

            // Find a Group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LoginPage.Link_FindAGroupHere);
            
            // Verify Search Category does not exist in InFellowship
            Assert.IsFalse(test.Selenium.IsElementPresent("//select[@id='category']/option[text()='" + "Test Unpublished Search Category" + "']"));
        }

        [Test, RepeatOnFailure]
        [DependsOn("Groups_Administration_SearchCategories_Unpublished_Update_Details")]
        [Author("Bryan Mikaelian")]
        [Description("Updates the criteria of an unpublished Search Category.")]
        public void Groups_Administration_SearchCategories_Unpublished_Update_Criteria() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Update the details of a published search category, removing the gender criteria and add the marital status and childcare criteria  
            test.Portal.Groups_SearchCategory_Update_Criteria("Test UPDATED Unpublished Search Category", new GeneralEnumerations.SearchCategoryCriteria[] { GeneralEnumerations.SearchCategoryCriteria.Gender, GeneralEnumerations.SearchCategoryCriteria.MaritalStatus, GeneralEnumerations.SearchCategoryCriteria.Childcare });

            // Edit the criteria of the search category
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Link_EditSeachCategoryCriteria);

            // Verify changes (Gender should not be checked but marital status/childcare should)
            Assert.IsFalse(test.Selenium.IsChecked(TableIds.Groups_SearchCategories + "/tbody/tr[3]/td[1]/p/input"));
            Assert.IsTrue(test.Selenium.IsChecked(TableIds.Groups_SearchCategories + "/tbody/tr[4]/td[1]/p/input"));
            Assert.IsTrue(test.Selenium.IsChecked(TableIds.Groups_SearchCategories + "/tbody/tr[5]/td[1]/p/input"));

            // Return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);
            
            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [DependsOn("Groups_Administration_SearchCategories_Unpublished_Update_Criteria")]
        [Author("Bryan Mikaelian")]
        [Description("Deletes an unpublished Search Category.")]
        public void Groups_Administration_SearchCategories_Unpublished_Delete() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Delete a published search category
            test.Portal.Groups_SearchCategory_Delete("Test UPDATED Unpublished Search Category");

            // Logout of portal
            test.Portal.Logout();

            // Open InFellowship
            test.infellowship.Open();

            // Find a Group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LoginPage.Link_FindAGroupHere);

            // Verify Search Category does not exist in InFellowship
            Assert.IsFalse(test.Selenium.IsElementPresent("//select[@id='category']/option[text()='" + "Test UPDATED Unpublished Search Category" + "']"));
        }

        #region Advanced CRUD
        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a portal user that is not a Group Type Admin cannot create a search category.")]
        public void Groups_Administration_SearchCategories_Create_Non_GTAs_Cannot_Create() {
            // Login to Portal as a non group type admin
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "QAEUNLX0C2");

            // Verify Custom Field link isn't present
            test.Selenium.Click("link=Groups");
            Assert.IsFalse(test.Selenium.IsElementPresent("link=Search Categories"));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a name is required when creating a seach category.")]
        public void Groups_Administration_SearchCategories_Create_Name_Required() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Groups->Administration->Search Categories
            test.Selenium.Navigate(Navigation.Groups.Administration.Search_Categories);

            // Create a new search category
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Link_AddSearchCategory);

            // Don't specify a name
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_SearchCategories.Step1_Button_Next);

            // Verify validation
            Assert.IsTrue(test.Selenium.IsTextPresent("The category name is required."));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a name must be unique when creating a seach category.")]
        public void Groups_Administration_SearchCategories_Create_Unique_Name() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Groups->Administration->Search Categories
            test.Selenium.Navigate(Navigation.Groups.Administration.Search_Categories);

            // Create a new search category with a name that already exists
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Link_AddSearchCategory);
            test.Selenium.Type(GroupsAdministrationConstants.Wizard_SearchCategories.Step1_TextField_SearchCategoryName, "Test Category");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.Next);

            // Verify validation
            Assert.IsTrue(test.Selenium.IsTextPresent("The category name can't be duplicated. The name \"Test Category\" has existed."));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a name cannot exceed 50 characters when creating a seach category.")]
        public void Groups_Administration_SearchCategories_Create_Name_Exceed_50_Characters() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Groups->Administration->Search Categories
            test.Selenium.Navigate(Navigation.Groups.Administration.Search_Categories);

            // Create a new search category
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Link_AddSearchCategory);

            // Specify a name that exceeds 50 characters
            test.Selenium.Type(GroupsAdministrationConstants.SearchCategoryManagement.TextField_SearchCategoryName, "Long name name name name Long name name name name Long name name name name Long name name name name Long name name name name Long name name name name Long name name name name");
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_SearchCategories.Step1_Button_Next);

            // Verify validation
            Assert.IsTrue(test.Selenium.IsTextPresent("The category name cannot exceed 50 characters."));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a description cannot exceed 500 characters when creating a seach category.")]
        public void Groups_Administration_SearchCategories_Create_Description_Exceed_500_Characters() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Groups->Administration->Search Categories
            test.Selenium.Navigate(Navigation.Groups.Administration.Search_Categories);

            // Create a new search category
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Link_AddSearchCategory);

            // Specify a description that exceeds 500 characters
            test.Selenium.Type(GroupsAdministrationConstants.SearchCategoryManagement.TextField_SearchCategoryName, "Test Search Category 2");
            test.Selenium.Type(GroupsAdministrationConstants.SearchCategoryManagement.TextField_SearchCategoryDescription, "Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2");
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_SearchCategories.Step1_Button_Next);

            // Verify validation
            Assert.IsTrue(test.Selenium.IsTextPresent("The category description cannot exceed 500 characters."));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies published is not checked by default when creating a search category.")]
        public void Groups_Administration_SearchCategories_Create_Published_Not_Checked_By_Default() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Groups->Administration->Search Categories
            test.Selenium.Navigate(Navigation.Groups.Administration.Search_Categories);

            // Create a new search category
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Link_AddSearchCategory);

            // Verify published is not checked
            Assert.IsFalse(test.Selenium.IsChecked(GroupsAdministrationConstants.Wizard_SearchCategories.Step1_Checkbox_Published));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies criteria is required when creating a search category.")]
        public void Groups_Administration_SearchCategories_Create_Criteria_Required() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Groups->Administration->Search Categories
            test.Selenium.Navigate(Navigation.Groups.Administration.Search_Categories);

            // Create a new search category
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Link_AddSearchCategory);

            // Specify a name and description
            test.Selenium.Type(GroupsAdministrationConstants.Wizard_SearchCategories.Step1_TextField_SearchCategoryName, "Search Category");
            test.Selenium.Type(GroupsAdministrationConstants.Wizard_SearchCategories.Step1_TextField_SearchCategoryDescription, "Search Category");

            // Next
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_SearchCategories.Step1_Button_Next);

            // Don't specify an criteria
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_SearchCategories.Step2_Button_CreateSearchCategory);

            // Verify message
            Assert.IsTrue(test.Selenium.IsTextPresent("You must select at least one option."));

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies a name is required when editing a seach category.")]
		public void Groups_Administration_SearchCategories_Edit_Name_Required() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Groups->Administration->Search Categories
            test.Selenium.Navigate(Navigation.Groups.Administration.Search_Categories);

            // Select a search category
            test.Selenium.ClickAndWaitForPageToLoad("link=Automation Search Category");

            // Edit the details
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Link_EditSearchCategoryDetails);

            // Don't specify a name
            test.Selenium.Type(GroupsAdministrationConstants.SearchCategoryManagement.TextField_SearchCategoryName, "");
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Button_SaveChanges_EditDetails);

            // Verify validation
            Assert.IsTrue(test.Selenium.IsTextPresent("The category name is required."));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a description cannot exceed 500 characters when editing a seach category.")]
        public void Groups_Administration_SearchCategories_Edit_Description_Exceed_500_Characters() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Groups->Administration->Search Categories
            test.Selenium.Navigate(Navigation.Groups.Administration.Search_Categories);

            // Select an existing search category
            test.Selenium.ClickAndWaitForPageToLoad("link=Automation Search Category");

            // Edit the details
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Link_EditSearchCategoryDetails);

            // Specify a description that exceeds 500 characters
            test.Selenium.Type(GroupsAdministrationConstants.SearchCategoryManagement.TextField_SearchCategoryDescription, "Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2Test Search Category 2 Test Search Category 2 Test Search Category 2 Test Search Category 2");
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Button_SaveChanges_EditDetails);

            // Verify validation
            Assert.IsTrue(test.Selenium.IsTextPresent("The category description cannot exceed 500 characters."));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the filtering works properly on the Search Category index page.")]
        public void Groups_Administration_SearchCategories_View_Filter_Shows_Correct_Categories() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Groups->Administration->Search Categories
            test.Selenium.Navigate(Navigation.Groups.Administration.Search_Categories);

            // Filter on published search categories
            test.Selenium.Click(GroupsAdministrationConstants.SearchCategoryManagement.RadioButton_ShowPublishedCategories);
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Button_Filter_Categories);
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Test Category"));
            Assert.IsFalse(test.Selenium.IsElementPresent("link=Automation Search Category"));

            // Filter on unpublished search categories                        
            test.Selenium.Click(GroupsAdministrationConstants.SearchCategoryManagement.RadioButton_ShowUnpublishedCategories);
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Button_Filter_Categories);
            Assert.IsFalse(test.Selenium.IsElementPresent("link=Test Category"));
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Automation Search Category"));

            // Filter on all search categories
            test.Selenium.Click(GroupsAdministrationConstants.SearchCategoryManagement.RadioButton_ShowAllCategories);
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Button_Filter_Categories);
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Test Category"));
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Automation Search Category"));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a name cannot exceed 50 characters when edit a seach category.")]
        public void Groups_Administration_SearchCategories_Edit_Name_Exceed_50_Characters() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Groups->Administration->Search Categories
            test.Selenium.Navigate(Navigation.Groups.Administration.Search_Categories);

            // Specify an existing search category
            test.Selenium.ClickAndWaitForPageToLoad("link=Automation Search Category");

            // Edit the details
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Link_EditSearchCategoryDetails);

            // Specify a name that exceeds 50 characters
            test.Selenium.Type(GroupsAdministrationConstants.SearchCategoryManagement.TextField_SearchCategoryName, "Long name name name name Long name name name name Long name name name name Long name name name name Long name name name name Long name name name name Long name name name name");
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Button_SaveChanges_EditDetails);

            // Verify validation
            Assert.IsTrue(test.Selenium.IsTextPresent("The category name cannot exceed 50 characters."));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies criteria is required when editing a search category.")]
        public void Groups_Administration_SearchCategories_Edit_Criteria_Required() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Groups->Administration->Search Categories
            test.Selenium.Navigate(Navigation.Groups.Administration.Search_Categories);

            // View a search category
            test.Selenium.ClickAndWaitForPageToLoad("link=Automation Search Category");

            // Edit the criteria
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Link_EditSeachCategoryCriteria);

            // Don't specify an criteria
            test.Selenium.Click(TableIds.Groups_SearchCategories + "/tbody/tr[3]/td[1]/p/input");
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Button_SaveChanges_EditCriteria);

            // Verify message
            Assert.IsTrue(test.Selenium.IsTextPresent("You must select at least one option."));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies an unpublished search category can be published.")]
        public void Groups_Administration_SearchCategories_Edit_Publish() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Groups->Administration->Search Categories
            test.Portal.Groups_SearchCategory_Create("Published Search Category", null, false, new string[] { "Gender" });

            // Publish this search category
            test.Selenium.ClickAndWaitForPageToLoad("link=Published Search Category");
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Link_EditSearchCategoryDetails);
            test.Selenium.Click(GroupsAdministrationConstants.SearchCategoryManagement.Checkbox_Published);
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Button_SaveChanges_EditDetails);

            // Navigate to groups->search categories
            test.Selenium.Navigate(Navigation.Groups.Administration.Search_Categories);

            // Filter on published search categories
            test.Selenium.Click(GroupsAdministrationConstants.SearchCategoryManagement.RadioButton_ShowPublishedCategories);
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Button_Filter_Categories);
            Assert.IsTrue(test.Selenium.IsElementPresent("link=Published Search Category"));

            // Filter on unpublished search categories
            test.Selenium.Click(GroupsAdministrationConstants.SearchCategoryManagement.RadioButton_ShowUnpublishedCategories);
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.SearchCategoryManagement.Button_Filter_Categories);
            Assert.IsFalse(test.Selenium.IsElementPresent("link=Published Search Category"));

            // Logout of Portal
            test.Portal.Logout();


            // Open InFellowship

            test.infellowship.Open();
            // Find a Group
            test.Selenium.ClickAndWaitForPageToLoad(GeneralInFellowshipConstants.LoginPage.Link_FindAGroupHere);

            // Verify Search Category Exists in InFellowship
            Assert.IsTrue(test.Selenium.IsElementPresent("//select[@id='category']/option[text()='" + "Published Search Category" + "']"));

            // Log in to Portal     
            test.Portal.Login("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Delete the search category
            test.Portal.Groups_SearchCategory_Delete("Published Search Category");

            // Logout of Portal
            test.Portal.Logout();
        }
        #endregion Advanced CRUD

    }

    [TestFixture]
    public class Portal_Groups_Administration_SpanOfCare_WebDriver : FixtureBaseWebDriver {

        #region CRUD

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Creates a Span of Care.")]
        public void Groups_Administration_SpanOfCare_Create_WebDriver() {
            // Initial conditions
            var socName = "Test Span of Care";
            base.SQL.Groups_SpanOfCare_Delete(254, socName);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a Span of Care
            test.Portal.Groups_SpanOfCare_Create_WebDriver(socName, string.Empty, new string[] { "Automation Group Type" }, new GeneralEnumerations.SpanOfCareCriteria[] { GeneralEnumerations.SpanOfCareCriteria.NotSpecified }, null, null, null);

            // Add an owner
            test.Portal.Groups_SpanOfCare_Update_AddOwner_WebDriver(socName, "Bryan Mikaelian");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // Verify the Span of Care is there
            test.Driver.FindElementByLinkText("Your Groups").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText(string.Format("{0}", socName)));

            // Logout
            test.Infellowship.LogoutWebDriver();

            // Delete the SOC
            base.SQL.Groups_SpanOfCare_Delete(254, socName);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Updates the name of a Span of Care")]
        public void Groups_Administration_SpanOfCare_Edit_Name_WebDriver() {
            // Initial conditions
            var socName = "Update Span of Care";
            List<string> groupTypes = new List<string>();
            groupTypes.Add("Automation Group Type");
            base.SQL.Groups_SpanOfCare_Delete(254, socName);
            base.SQL.Groups_SpanOfCare_Delete(254, "Test UPDATED Span of Care");
            base.SQL.Groups_SpanOfCare_Create(254, socName, "Bryan Mikaelian", "SOC Owner", groupTypes);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // Updates the name of a SPOC
            test.Portal.Groups_SpanOfCare_Update_Name_Description_WebDriver(socName, "Test UPDATED Span of Care", string.Empty);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View your groups
            test.Driver.FindElementByLinkText("Your Groups").Click();

            // Verify the updated Span of Care is present in InFellowship
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("Test UPDATED Span of Care"));
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.LinkText(string.Format("{0}", socName)));

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Clean up
            base.SQL.Groups_SpanOfCare_Delete(254, "Test UPDATED Span of Care");
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Deletes a Span of Care")]
        public void Groups_Administration_SpanOfCare_Delete_WebDriver() {
            // Initial conditions
            var socName = "Delete Span of Care";
            List<string> groupTypes = new List<string>();
            groupTypes.Add("Automation Group Type");
            base.SQL.Groups_SpanOfCare_Delete(254, socName);
            base.SQL.Groups_SpanOfCare_Create(254, socName, "Bryan Mikaelian", "SOC Owner", groupTypes);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // Delete a Span of Care
            test.Portal.Groups_SpanOfCare_Delete_WebDriver(socName);

            // Verify it has been removed from the Span Of Care page
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);
            test.Driver.FindElementByLinkText("Spans of Care").Click();
            
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.LinkText(socName));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View your groups
            test.Driver.FindElementByLinkText("Your Groups").Click();

            // Verify the Span of Care is not present in InFellowship
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.LinkText(socName));

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Clean up
            base.SQL.Groups_SpanOfCare_Delete(254, socName);

        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("David Martin")]
        [Description("Verifies the user can associate more than a total of 10 Custom Field choices to a Span of Care.")]
        public void Groups_Administration_SpanOfCare_Create_MoreThan10CustomFieldChoices_WebDriver() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            string groupType = "Test Group Type - Custom Field over 10 choices";
            string spanOfCare = @"Test Span of Care - Custom Field over 10 choices";
            string customField = "Test Custom Field - over 10 Choices";

            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            try {
                // Delete the span of care
                test.Portal.Groups_SpanOfCare_Delete_WebDriver(spanOfCare);
            }
            catch (Exception e1) {
                TestLog.WriteLine(string.Format("{0} has possibly been already deleted. {1}", spanOfCare, e1.ToString()));
            }

            try {
                // Delete the group type
                test.Portal.Groups_GroupType_Delete_WebDriver(groupType);
            }
            catch (Exception e2) {
                TestLog.WriteLine(string.Format("{0} has possibly been already deleted. {1}", groupType, e2.ToString()));
            }

            try {
                // Delete the custom field
                test.Portal.Groups_CustomField_Delete_WebDriver(customField);
            }
            catch (Exception e3) {
                TestLog.WriteLine(string.Format("{0} has possibly been already deleted. {1}", customField, e3.ToString()));
            }

            // Create a custom field
            string[] customFieldChoices = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11" };
            test.Portal.Groups_CustomField_Create_WebDriver(customField, null, GeneralEnumerations.CustomFieldType.MultiSelect, customFieldChoices);

            // Create a group type

            // This group type references a custom field with more than 10 choices.
            test.Portal.Groups_GroupType_Create_WebDriver(groupType, string.Empty, true, true, new GeneralEnumerations.GroupTypeLeaderAdminRights[] { GeneralEnumerations.GroupTypeLeaderAdminRights.InviteSomeone }, true, GeneralEnumerations.GroupTypeLeaderViewRights.Limited, GeneralEnumerations.GroupTypeMemberViewRights.Limited, new string[] { test.Portal.PortalUser }, new string[] { customField });

            // Create a new span of Care
            List<Dictionary<string, List<string>>> customFieldsAndChoices = new List<Dictionary<string, List<string>>>();
            Dictionary<string, List<string>> customFieldAndChoices = new Dictionary<string, List<string>>();
            customFieldAndChoices.Add(customField, customFieldChoices.ToList<string>());
            customFieldsAndChoices.Add(customFieldAndChoices);
            string[] customFields = new string[] { customField };

            test.Portal.Groups_SpanOfCare_Create_WebDriver(spanOfCare, string.Empty, new string[] { groupType }, new GeneralEnumerations.SpanOfCareCriteria[] { GeneralEnumerations.SpanOfCareCriteria.NotSpecified }, null, null, null, customFieldsAndChoices);

            // Verify the custom field data on the confirmation page
            test.Driver.FindElementByLinkText(spanOfCare).Click();
            Assert.AreEqual(customField, test.Driver.FindElementByXPath("//a[normalize-space(following-sibling::text())='Custom Fields']/ancestor::h6/following-sibling::dl/dt").Text);
            int customFieldIndex = 1;
            for (int i = 0; i < customFieldChoices.Length; i++) {
                Assert.AreEqual(customFieldChoices[i], test.Driver.FindElementByXPath(string.Format("//a[normalize-space(following-sibling::text())='Custom Fields']/ancestor::h6/following-sibling::dl/dd[{0}]", customFieldIndex)).Text);
                customFieldIndex++;
            }

            // Delete the span of care
            test.Portal.Groups_SpanOfCare_Delete_WebDriver(spanOfCare);

            // Delete the group type
            test.Portal.Groups_GroupType_Delete_WebDriver(groupType);

            // Delete the custom field
            System.Threading.Thread.Sleep(5000);
            test.Portal.Groups_CustomField_Delete_WebDriver(customField);

            // Logout of portal
            test.Portal.LogoutWebDriver();

            // Clean up
            //test.SQL.Groups_GroupType_Delete(256, groupType);
            //test.SQL.Groups_CustomField_Delete(256, customField);

        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("David Martin")]
        [Description("Verifies the user can associate more than a total of 10 Custom Field choices to a Span of Care when editing.")]
        public void Groups_Administration_SpanOfCare_Edit_MoreThan10CustomFieldChoices_WebDriver() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("ft.tester", "FT4life!", "QAEUNLX0C4");

            string customField = "Test Custom Field - +10 Choices - Edit";
            string groupType = "Test Group Type - Custom Field - Edit";
            string spanOfCare = @"Test Span of Care - Custom Field w/+10 choices - Edit";

            // Mady added SQL operatoins to delete duplicated test data if some wasn't successfully deleted.
            int churchId = test.SQL.Ministry_Church_FetchID("QAEUNLX0C4");
            test.SQL.Groups_SpanOfCare_Delete(churchId, spanOfCare);
            test.SQL.Groups_GroupType_Delete(churchId, groupType);
            test.SQL.Groups_CustomField_Delete(churchId, customField);

            // Create a custom field
            string[] customFieldChoices = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11" };
            test.Portal.Groups_CustomField_Create_WebDriver(customField, string.Empty, GeneralEnumerations.CustomFieldType.MultiSelect, customFieldChoices);

            // Create a group type
            //"This group type references a custom field with more than 10 choices."
            test.Portal.Groups_GroupType_Create_WebDriver(groupType, string.Empty, true, true, new GeneralEnumerations.GroupTypeLeaderAdminRights[] { GeneralEnumerations.GroupTypeLeaderAdminRights.InviteSomeone }, true, GeneralEnumerations.GroupTypeLeaderViewRights.Limited, GeneralEnumerations.GroupTypeMemberViewRights.Limited, new string[] { test.Portal.PortalUser }, new string[] { customField });

            // Create a span of care
            List<Dictionary<string, List<string>>> customFieldsAndChoices = new List<Dictionary<string, List<string>>>();
            Dictionary<string, List<string>> customFieldAndChoices = new Dictionary<string, List<string>>();
            customFieldAndChoices.Add(customField, customFieldChoices.ToList<string>());
            customFieldsAndChoices.Add(customFieldAndChoices);
            string[] customFields = new string[] { customField };

            test.Portal.Groups_SpanOfCare_Create_WebDriver(spanOfCare, string.Empty, new string[] { groupType }, new GeneralEnumerations.SpanOfCareCriteria[] { GeneralEnumerations.SpanOfCareCriteria.NotSpecified }, null, null, null, null);

            try
            {
                // Edit the span of care
                //test.Driver.FindElementByLinkText(GeneralLinks.Back).Click();
                test.Driver.FindElementByLinkText(string.Format("{0}", spanOfCare)).Click();

                // Edit the custom fields
                test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step4.aspx')]").Click();
                foreach (var cFieldAndChoices in customFieldsAndChoices)
                {
                    foreach (var cField in cFieldAndChoices.Keys)
                    {
                        test.Driver.FindElementByXPath(string.Format("//input[contains(@id, 'custom_field_')]/ancestor::div/following-sibling::div/label[text()='{0}']", cField)).Click();
                        decimal checkboxCount = test.Driver.FindElementsByXPath(string.Format("//input[contains(@id, 'custom_field_')]/ancestor::div/following-sibling::div/label[text()='{0}']/ancestor::div/following-sibling::div/table/tbody/tr[child::th/input]", customField)).Count;

                        for (int i = 1; i <= checkboxCount; i++)
                        {
                            test.Driver.FindElementByXPath(string.Format("//input[contains(@id, 'custom_field_')]/ancestor::div/following-sibling::div/label[normalize-space(text()='{0}')]/ancestor::div/following-sibling::div/table/tbody/tr[{1}]/th/input", customFieldAndChoices[customField][i - 1], i)).Click();
                        }
                    }
                }

                test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
                test.Driver.FindElementById(GeneralButtons.submitQuery).Click();

                // Verify the custom field data on the confirmation page
                Assert.AreEqual(customFields[0], test.Driver.FindElementByXPath("//a[normalize-space(following-sibling::text())='Custom Fields']/ancestor::h6/following-sibling::dl/dt").Text);
                int customFieldIndex = 1;
                for (int i = 0; i < customFieldChoices.Length; i++)
                {
                    Assert.AreEqual(customFieldChoices[i], test.Driver.FindElementByXPath(string.Format("//a[normalize-space(following-sibling::text())='Custom Fields']/ancestor::h6/following-sibling::dl/dd[{0}]", customFieldIndex)).Text);
                    customFieldIndex++;
                }
            }
            finally
            {
                // Delete the span of care
                System.Threading.Thread.Sleep(5000);
                test.Portal.Groups_SpanOfCare_Delete_WebDriver(spanOfCare);

                // Delete the group type
                test.Portal.Groups_GroupType_Delete_WebDriver(groupType);

                // Delete the custom field
                System.Threading.Thread.Sleep(8000);
                test.Portal.Groups_CustomField_Delete_WebDriver(customField);

                // Logout of portal
                test.Portal.LogoutWebDriver();
            }
        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies a name must be specified when creating a Span of Care")]
        public void Groups_Administration_SpanOfCare_Create_Name_Required_WebDriver() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->span of care
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.Administration.Span_Of_Care);

            // Create a SPOC without a name 
            test.Driver.FindElementByLinkText("Add").Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("error_message"));

            // Verify error message
            Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver("Name is required"));

            // Cancel and return
            test.Driver.FindElementByLinkText("RETURN").Click();

            // Logout
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies Span of Care names cannot be larger than 100 characters.")]
        public void Groups_Administration_SpanOfCare_Create_Name_Exceed_100_Characters_WebDriver() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->span of care
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.Administration.Span_Of_Care);

            // Create a SOC with a duplicate name
            test.Driver.FindElementByLinkText("Add").Click();
            test.Driver.FindElementById("soc_name").SendKeys("Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care");

            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("error_message"));

            // Assert the error text is present
            Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver("Name cannot exceed 100 characters."));

            // Cancel and return
            test.Driver.FindElementByLinkText("RETURN").Click();

            // Logout
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies Span of Care descriptions cannot exceed 500 characters.")]
        public void Groups_Administration_SpanOfCare_Create_Description_Exceed_500_Characters_WebDriver() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->span of care
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.Administration.Span_Of_Care);

            // Create a SPOC with a duplicate name
            test.Driver.FindElementByLinkText("Add").Click();
            test.Driver.FindElementById("soc_name").SendKeys("Dinosaur Span of Care");
            test.Driver.FindElementById(GroupsAdministrationConstants.Wizard_SpanOfCare.Step1_TextField_SpanOfCareDescription).SendKeys("Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care Test Duplicate Span of Care ");

            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();

            // Assert the error text is present
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("error_message"));
            Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver("Description cannot exceed 500 characters."));

            // Cancel and return
            test.Driver.FindElementByLinkText("RETURN").Click();

            // Logout
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies Span of Care names must be unique.")]
        public void Groups_Administration_SpanOfCare_Create_Unique_Name_Required_WebDriver() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->span of care
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.Administration.Span_Of_Care);

            // Create a SPOC with a duplicate name
            test.Driver.FindElementByLinkText("Add").Click();
            test.Driver.FindElementById("soc_name").SendKeys("Automation Span of Care");

            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();

            // Assert the error text is present
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("error_message"));
            Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver("Span of care name should be unique."));

            // Cancel and return
            test.Driver.FindElementByLinkText("RETURN").Click();

            // Logout
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies a group type must be selected when creating a Span of Care.")]
        public void Groups_Administration_SpanOfCare_Create_GroupType_Required_WebDriver() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->span of care
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.Administration.Span_Of_Care);

            // Create a SPOC 
            test.Driver.FindElementByLinkText("Add").Click();
            test.Driver.FindElementById("soc_name").SendKeys("No Group Type Span of Care");

            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();

            // Attempt to go to step 3 without selecting a group type.
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("error_message"));

            // Verify error message
            Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver("Choose at least one group type."));

            // Cancel and return
            test.Driver.FindElementByLinkText("RETURN").Click();

            // Logout
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies all group types for the church are present for a group type admin when they create a Span of Care, regardless if they have rights to admin/manage/view groups of that type.")]
        public void Groups_Administration_SpanOfCare_Create_All_Group_Types_Present_WebDriver() {
            // Create a new group type
            var groupTypeName = "New Group Type SOC";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int> { 11 });

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->span of care
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.Administration.Span_Of_Care);

            // Create a Span of Care and verify the new group type that no one has rights to is present.
            test.Driver.FindElementByLinkText("Add").Click();
            test.Driver.FindElementById("soc_name").SendKeys("All Group Types Span of Care");
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Edit"));

            // Verify a group type that the group type admin has no group admin/manager/view rights to groups of that type still shows up during the creation of a span of care.
            Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver(groupTypeName), "Group Type was not present");
            test.Driver.FindElementByLinkText("RETURN").Click();

            // Logout of portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies a portal user that is not a Group Type Admin cannot create a Span of Care.")]
        public void Groups_Administration_SpanOfCare_Create_Non_GTAs_Cannot_Create_WebDriver() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("groupadmin", "BM.Admin09", "QAEUNLX0C2");

            // Verify Span of Care link isn't present
            test.Driver.FindElementByLinkText("Groups").Click();
            Assert.IsFalse(test.GeneralMethods.IsElementPresentWebDriver(By.LinkText("Span of Care")));

            // Logout
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("David Martin")]
        [Description("Verifies the list of groups updates for a Span of Care if you add/remove a group type.")]
        public void Groups_Administration_SpanOfCare_Edit_GroupTypes_WebDriver() {
            // Var initial data
            var groupType = "Add Group Type 1 SOC";
            var groupType2 = "Add Group Type 2 SOC";
            var groupType1Group = "Add Group Type 1 Group";
            var groupType2Group = "Add Group Type 2 Group";
            var spanOfCareName = "Add Group Type SOC";
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_GroupType_Delete(254, groupType2);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupType, new List<int> { 11 });
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupType2, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupType, "Bryan Mikaelian", groupType1Group, null, "11/15/2009");
            base.SQL.Groups_Group_Create(254, groupType2, "Bryan Mikaelian", groupType2Group, null, "11/15/2009");
            base.SQL.Groups_SpanOfCare_Create(254, spanOfCareName, "Bryan Mikaelian", "SOC Owner", new List<string> { groupType });

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the span of care
            test.Portal.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Verify some of the groups are present    
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType1Group, "Groups"));

            // Verify some of the groups aren't present
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType2Group, "Groups"));

            // Add Another group type
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step2.aspx')]").Click();
            test.Driver.FindElementByXPath(string.Format("//input[@name='group_type' and normalize-space(ancestor::td/following-sibling::td/label/text())='{0}']", groupType2)).Click();

            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();

            // Verify all of the groups are present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType1Group, "Groups"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType2Group, "Groups"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_GroupType_Delete(254, groupType2);
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);

        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("David Martin")]
        [Description("Verifies the list of groups updates for a Span of Care if you add/remove a campus.")]
        public void Groups_Administration_SpanOfCare_Edit_Campuses_WebDriver() {
            // Var initial data
            var groupType = "SOC Campus Group Type 1";
            var groupType2 = "SOC Campus Group Type 2";
            var groupType1Group = "Has Campus Group";
            var groupType2Group = "No Campus Group";
            var spanOfCareName = "Campus SOC";
            var campusOne = "SOC Campus";
            base.SQL.Admin_CampusDelete(254, campusOne);
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_GroupType_Delete(254, groupType2);
            base.SQL.Admin_CampusCreate(254, campusOne);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupType, new List<int> { 11 });
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupType2, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupType, "Bryan Mikaelian", groupType1Group, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, false, false, campusOne, true);
            base.SQL.Groups_Group_Create(254, groupType2, "Bryan Mikaelian", groupType2Group, null, "11/15/2009");
            base.SQL.Groups_SpanOfCare_Create(254, spanOfCareName, "Bryan Mikaelian", "SOC Owner", new List<string> { groupType, groupType2 });

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the span of care
            test.Portal.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Specify a campus
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]").Click();
            test.Driver.FindElementByXPath(string.Format("//input[@name='campus' and normalize-space(ancestor::td/following-sibling::td/label/text())='{0}']", campusOne)).Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();

            // Verify the groups that meet at the campus are present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType1Group, "Groups"));

            // Verify the groups that do not meet at the campus are present
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType2Group, "Groups"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Span of Care
            test.Infellowship.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Verify groups that meet a campus are the only ones present
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(groupType1Group));
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.LinkText(groupType2Group));

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Login to Portal
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the span of care
            test.Portal.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Unspecify a campus
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]").Click();
            test.Driver.FindElementByXPath(string.Format("//input[@name='campus' and normalize-space(ancestor::td/following-sibling::td/label/text())='{0}']", campusOne)).Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();

            // Verify the groups
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType1Group, "Groups"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType2Group, "Groups"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Span of Care
            test.Infellowship.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Verify groups 
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(groupType1Group));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(groupType2Group));

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_GroupType_Delete(254, groupType2);
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);

        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("David Martin")]
        [Description("Verifies the list of groups updates for a Span of Care if toggle between the options of Childcare")]
        public void Groups_Administration_SpanOfCare_Edit_Childcare_WebDriver() {
            // Var initial data
            var groupType = "SOC Childcare Group Type 1";
            var groupType2 = "SOC Childcare Group Type 2";
            var groupType1Group = "Childcare Group";
            var groupType2Group = "No Childcare Group";
            var spanOfCareName = "Childcare SOC";
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupType, new List<int> { 11 });
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupType2, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupType, "Bryan Mikaelian", groupType1Group, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, true);
            base.SQL.Groups_Group_Create(254, groupType2, "Bryan Mikaelian", groupType2Group, null, "11/15/2009");
            base.SQL.Groups_SpanOfCare_Create(254, spanOfCareName, "Bryan Mikaelian", "SOC Owner", new List<string> { groupType, groupType2 });

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the span of care
            test.Portal.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Specify childcare is provided
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]").Click();
            test.Driver.FindElementById("rb_childcare_yes").Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();

            // Verify the groups that have childcare are present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType1Group, "Groups"));

            // Verify the groups that do not have childcare are not present
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType2Group, "Groups"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Span of Care
            test.Infellowship.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Verify groups that have childcare are the only ones present
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", groupType1Group)));
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.LinkText(string.Format("{0}", groupType2Group)));

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Login to Portal
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the span of care
            test.Portal.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Specify no childcare
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]").Click();
            test.Driver.FindElementById("rb_childcare_no").Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();

            // Verify the groups that have childcare are not present
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType1Group, "Groups"));

            // Verify the groups that do not have childcare are present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType2Group, "Groups"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Span of Care
            test.Infellowship.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Verify groups 
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.LinkText(string.Format("{0}", groupType1Group)));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", groupType2Group)));

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Login to Portal
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the span of care
            test.Portal.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Specify childcare is N/A
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]").Click();
            test.Driver.FindElementById("rb_childcare_none").Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();

            // Verify the groups that have childcare are present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType1Group, "Groups"));

            // Verify the groups that do not have childcare are present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType2Group, "Groups"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Span of Care
            test.Infellowship.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Verify groups 
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", groupType1Group)));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", groupType2Group)));

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_GroupType_Delete(254, groupType2);
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);

        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("David Martin")]
        [Description("Verifies the list of groups updates for a Span of Care if you add/remove the coed option.")]
        public void Groups_Administration_SpanOfCare_Edit_Gender_Coed_WebDriver() {
            // Var initial data
            var groupType = "SOC Coed Group Type 1";
            var groupType2 = "SOC Coed Group Type 2";
            var groupType1Group = "Coed Group";
            var groupType2Group = "Female Group";
            var spanOfCareName = "Coed SOC";
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupType, new List<int> { 11 });
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupType2, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupType, "Bryan Mikaelian", groupType1Group, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, true);
            base.SQL.Groups_Group_Create(254, groupType2, "Bryan Mikaelian", groupType2Group, null, "11/15/2009", GeneralEnumerations.Gender.Female);
            base.SQL.Groups_SpanOfCare_Create(254, spanOfCareName, "Bryan Mikaelian", "SOC Owner", new List<string> { groupType, groupType2 });

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the span of care
            test.Portal.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Specify coed groups only
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]").Click();
            test.Driver.FindElementById("chk_gender_coed").Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();

            // Verify the groups that are coed are present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType1Group, "Groups"));

            // Verify the groups that are not coed are not present
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType2Group, "Groups"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Span of Care
            test.Infellowship.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Verify groups that are coed are the only ones present
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", groupType1Group)));
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.LinkText(string.Format("{0}", groupType2Group)));

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Login to Portal
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the span of care
            test.Portal.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Undo the coed choice
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]").Click();
            test.Driver.FindElementById("chk_gender_coed").Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();

            // Verify all the groups are present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType1Group, "Groups"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType2Group, "Groups"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Span of Care
            test.Infellowship.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Verify all the groups are present
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", groupType1Group)));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", groupType2Group)));

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_GroupType_Delete(254, groupType2);
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("David Martin")]
        [Description("Verifies the list of groups updates for a Span of Care if you add/remove the female option.")]
        public void Groups_Administration_SpanOfCare_Edit_Gender_Female_WebDriver() {
            // Var initial data
            var groupType = "SOC Female Group Type 1";
            var groupType2 = "SOC Female Group Type 2";
            var groupType1Group = "Female Group";
            var groupType2Group = "Coed Group";
            var spanOfCareName = "Female SOC";
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupType, new List<int> { 11 });
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupType2, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupType, "Bryan Mikaelian", groupType1Group, null, "11/15/2009", GeneralEnumerations.Gender.Female);
            base.SQL.Groups_Group_Create(254, groupType2, "Bryan Mikaelian", groupType2Group, null, "11/15/2009", GeneralEnumerations.Gender.Coed);
            base.SQL.Groups_SpanOfCare_Create(254, spanOfCareName, "Bryan Mikaelian", "SOC Owner", new List<string> { groupType, groupType2 });

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the span of care
            test.Portal.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Specify female groups only
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]").Click();
            test.Driver.FindElementById("chk_gender_female").Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();

            // Verify the groups that are female are present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType1Group, "Groups"));

            // Verify the groups that are not female are not present
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType2Group, "Groups"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Span of Care
            test.Infellowship.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Verify groups that are female are the only ones present
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", groupType1Group)));
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.LinkText(string.Format("{0}", groupType2Group)));

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Login to Portal
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the span of care
            test.Portal.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Undo the female choice
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]").Click();
            test.Driver.FindElementById("chk_gender_female").Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();

            // Verify all the groups are present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType1Group, "Groups"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType2Group, "Groups"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Span of Care
            test.Infellowship.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Verify all the groups are present 
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", groupType1Group)));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", groupType2Group)));

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_GroupType_Delete(254, groupType2);
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("David Martin")]
        [Description("Verifies the list of groups updates for a Span of Care if you add/remove the male option.")]
        public void Groups_Administration_SpanOfCare_Edit_Gender_Male_WebDriver() {
            // Var initial data
            var groupType = "SOC Male Group Type 1";
            var groupType2 = "SOC Male Group Type 2";
            var groupType1Group = "Male Group";
            var groupType2Group = "Coed Group";
            var spanOfCareName = "Male SOC";
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupType, new List<int> { 11 });
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupType2, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupType, "Bryan Mikaelian", groupType1Group, null, "11/15/2009", GeneralEnumerations.Gender.Male);
            base.SQL.Groups_Group_Create(254, groupType2, "Bryan Mikaelian", groupType2Group, null, "11/15/2009", GeneralEnumerations.Gender.Coed);
            base.SQL.Groups_SpanOfCare_Create(254, spanOfCareName, "Bryan Mikaelian", "SOC Owner", new List<string> { groupType, groupType2 });

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the span of care
            test.Portal.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Specify male groups only
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]").Click();
            test.Driver.FindElementById("chk_gender_male").Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();

            // Verify the groups that are male are present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType1Group, "Groups"));

            // Verify the groups that are not male are not present
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType2Group, "Groups"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Span of Care
            test.Infellowship.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Verify groups that are male are the only ones present
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", groupType1Group)));
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.LinkText(string.Format("{0}", groupType2Group)));

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Login to Portal
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the span of care
            test.Portal.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Undo the male choice
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]").Click();
            test.Driver.FindElementById("chk_gender_male").Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();

            // Verify all the groups are present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType1Group, "Groups"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType2Group, "Groups"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Span of Care
            test.Infellowship.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Verify all the groups are present 
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", groupType1Group)));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", groupType2Group)));

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_GroupType_Delete(254, groupType2);
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);

        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("David Martin")]
        [Description("Verifies the list of groups updates for a Span of Care if you add/remove the married or single option.")]
        public void Groups_Administration_SpanOfCare_Edit_MaritalStatus_MarriedOrSingle_WebDriver() {
            // Var initial data
            var groupType = "SOC MarriedSingle Group Type 1";
            var groupType2 = "SOC MarriedSingle Group Type 2";
            var groupType1Group = "Married or Single Group";
            var groupType2Group = "Married Group";
            var spanOfCareName = "Married or Single SOC";
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupType, new List<int> { 11 });
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupType2, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupType, "Bryan Mikaelian", groupType1Group, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle);
            base.SQL.Groups_Group_Create(254, groupType2, "Bryan Mikaelian", groupType2Group, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.Married);
            base.SQL.Groups_SpanOfCare_Create(254, spanOfCareName, "Bryan Mikaelian", "SOC Owner", new List<string> { groupType, groupType2 });

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the span of care
            test.Portal.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Specify married or single groups only
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]").Click();
            test.Driver.FindElementById("chk_marital_status_both").Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();

            // Verify the groups that are married or single are present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType1Group, "Groups"));

            // Verify the groups that are not married or single are not present
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType2Group, "Groups"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Span of Care
            test.Infellowship.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Verify groups that are married or single  are the only ones present
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", groupType1Group)));
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.LinkText(string.Format("{0}", groupType2Group)));

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Login to Portal
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the span of care
            test.Portal.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Undo the married or single choice
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]").Click();
            test.Driver.FindElementById("chk_marital_status_both").Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();

            // Verify all the groups are present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType1Group, "Groups"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType2Group, "Groups"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Span of Care
            test.Infellowship.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Verify all the groups are present 
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", groupType1Group)));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", groupType2Group)));

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_GroupType_Delete(254, groupType2);
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);

        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("David Martin")]
        [Description("Verifies the list of groups updates for a Span of Care if you add/remove the married option.")]
        public void Groups_Administration_SpanOfCare_Edit_MaritalStatus_Married_WebDriver() {
            // Var initial data
            var groupType = "SOC Married Group Type 1";
            var groupType2 = "SOC Married Group Type 2";
            var groupType1Group = "Married Group";
            var groupType2Group = "Married or Single Group";
            var spanOfCareName = "Married SOC";
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupType, new List<int> { 11 });
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupType2, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupType, "Bryan Mikaelian", groupType1Group, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.Married);
            base.SQL.Groups_Group_Create(254, groupType2, "Bryan Mikaelian", groupType2Group, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle);
            base.SQL.Groups_SpanOfCare_Create(254, spanOfCareName, "Bryan Mikaelian", "SOC Owner", new List<string> { groupType, groupType2 });

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the span of care
            test.Portal.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Specify married groups only
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]").Click();
            test.Driver.FindElementById("chk_marital_status_married").Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();

            // Verify the groups that are married are present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType1Group, "Groups"));

            // Verify the groups that are not married are not present
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType2Group, "Groups"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Span of Care
            test.Infellowship.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Verify groups that are married are the only ones present
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", groupType1Group)));
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.LinkText(string.Format("{0}", groupType2Group)));

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Login to Portal
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the span of care
            test.Portal.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Undo the married choice
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]").Click();
            test.Driver.FindElementById("chk_marital_status_married").Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();

            // Verify all the groups are present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType1Group, "Groups"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType2Group, "Groups"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Span of Care
            test.Infellowship.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Verify all the groups are present 
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", groupType1Group)));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", groupType2Group)));

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_GroupType_Delete(254, groupType2);
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);

        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("David Martin")]
        [Description("Verifies the list of groups updates for a Span of Care if you add/remove the single option.")]
        public void Groups_Administration_SpanOfCare_Edit_MaritalStatus_Single_WebDriver() {
            // Var initial data
            var groupType = "SOC Single Group Type 1";
            var groupType2 = "SOC Single Group Type 2";
            var groupType1Group = "Single Group";
            var groupType2Group = "Married or Single Group";
            var spanOfCareName = "Single SOC";
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupType, new List<int> { 11 });
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupType2, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupType, "Bryan Mikaelian", groupType1Group, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.Single);
            base.SQL.Groups_Group_Create(254, groupType2, "Bryan Mikaelian", groupType2Group, null, "11/15/2009", GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle);
            base.SQL.Groups_SpanOfCare_Create(254, spanOfCareName, "Bryan Mikaelian", "SOC Owner", new List<string> { groupType, groupType2 });

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the span of care
            test.Portal.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Specify single groups only
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]").Click();
            test.Driver.FindElementById("chk_marital_status_single").Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();

            // Verify the groups that are single are present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType1Group, "Groups"));

            // Verify the groups that are not single are not present
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType2Group, "Groups"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Span of Care
            test.Infellowship.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Verify groups that are single are the only ones present
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", groupType1Group)));
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.LinkText(string.Format("{0}", groupType2Group)));

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Login to Portal
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the span of care
            test.Portal.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Undo the single choice
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]").Click();
            test.Driver.FindElementById("chk_marital_status_single").Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();
            test.Driver.FindElementById(GeneralButtons.submitQuery).Click();

            // Verify all the groups are present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType1Group, "Groups"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, groupType2Group, "Groups"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Span of Care
            test.Infellowship.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Verify all the groups are present 
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", groupType1Group)));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", groupType2Group)));

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_GroupType_Delete(254, groupType2);
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);

        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("David Martin")]
        [Description("Verifies the list of groups updates for a Span of Care if you add/remove a group.")]
        public void Groups_Administration_SpanOfCare_Edit_Groups_WebDriver() {
            // Var initial data
            var groupType = "Add Group Group Type 1";
            var group1 = "Existing Group";
            var group2 = "New Group";
            var spanOfCareName = "New Group SOC";
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupType, new List<int> { 11 });
            base.SQL.Groups_Group_Create(254, groupType, "Bryan Mikaelian", group1, null, "11/15/2009");
            base.SQL.Groups_SpanOfCare_Create(254, spanOfCareName, "Bryan Mikaelian", "SOC Owner", new List<string> { groupType });

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // Create a new group
            test.Portal.Groups_Group_Create_WebDriver(groupType, group2, string.Empty, "11/15/2009", true);

            // View the span of care
            test.Portal.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Verify the new group is present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, group1, "Groups"));
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, group2, "Groups"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Span of Care
            test.Infellowship.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Verify the new group is present
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", group1)));
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", group2)));

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Delete the group
            base.SQL.Groups_Group_Delete(254, group2);

            // Login to Portal
            test.Portal.LoginWebDriver("bmikaelian", "BM.Admin09", "QAEUNLX0C2");

            // View the span of care
            test.Portal.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Verify the new group is no longer present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, group1, "Groups"));
            Assert.IsFalse(test.GeneralMethods.ItemExistsInTableWebDriver(TableIds.Groups_SpanOfCare_GroupList, group2, "Groups"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("socowner@gmail.com", "BM.Admin09", "QAEUNLX0C2");

            // View the Span of Care
            test.Infellowship.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Verify all the groups are present 
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText(string.Format("{0}", group1)));
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.LinkText(string.Format("{0}", group2)));

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);

        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("David Martin")]
        [Description("Verifies you can create a Span of Care based on a custom field.")]
        public void Groups_Administration_SpanOfCare_Create_Custom_Field_Based_WebDriver() {
            // Initial data
            List<Dictionary<string, List<string>>> customFieldsAndChoices = new List<Dictionary<string, List<string>>>();
            var groupTypeName = "Custom Field SOC GT";
            var groupName = "Custom Field SOC G";
            var customFieldName = "SOC Custom Field Add Group";

            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.MultiSelect, customFieldName, null, new List<string>() { "Yes", "No" });
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int>() { 11 });
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, today.ToShortDateString());

            List<string> customFieldChoices = new List<string>() { "Yes" };
            Dictionary<string, List<string>> customFieldAndChoices = new Dictionary<string, List<string>>();
            customFieldAndChoices.Add(customFieldName, customFieldChoices);

            // Add custom Field A to the general collection
            customFieldsAndChoices.Add(customFieldAndChoices);

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // Add a custom field with a default to the group type
            test.Portal.Groups_GroupType_View_WebDriver(groupTypeName);
            test.Driver.FindElementByLinkText("Change custom fields").Click();
            test.Portal.Groups_GroupType_Update_CustomField_WebDriver(customFieldName, "Yes");
            test.Driver.FindElementById("update_customfields").Click();

            // Create a Span of Care with a Custom Field
            System.Threading.Thread.Sleep(5000);
            test.Portal.Groups_SpanOfCare_Create_WebDriver("Custom Field Span of Care", string.Empty, new string[] { groupTypeName }, new GeneralEnumerations.SpanOfCareCriteria[] { GeneralEnumerations.SpanOfCareCriteria.NotSpecified }, null, null, null, customFieldsAndChoices);

            // Verify the correct groups are returned
            test.Driver.FindElementByLinkText("Custom Field Span of Care").Click();
            test.GeneralMethods.VerifyTextPresentWebDriver(groupName);

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_SpanOfCare_Delete(254, "Custom Field Span of Care");

        }
        #endregion CRUD

        #region Navigation
        [Test, RepeatOnFailure, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies the edit links in the Span of Care wizard go to the right pages.")]
        public void Groups_Administration_SpanOfCare_Create_Edit_Links_Go_To_Correct_Pages_WebDriver() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->span of care
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.Administration.Span_Of_Care);

            // Create a new span of Care
            test.Driver.FindElementByLinkText("Add").Click();

            // Step 1
            test.Driver.FindElementById("soc_name").SendKeys("Test Span of Care Navigation");

            // Next
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();

            // Edit Step 1 and verify you are at Step 1
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step1.aspx')]").Click();

            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("soc_name")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step1_TextField_SpanOfCareDescription)));

            // Next to Step 2
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();

            test.GeneralMethods.WaitForElement(test.Driver, By.PartialLinkText("Back"));
            var row = test.GeneralMethods.GetTableRowNumberWebDriver(TableIds.Groups_SpanOfCare_GroupTypes, "Automation Group Type", "Group Type");
            test.Driver.FindElementByXPath(string.Format("//*/tr[{0}]/td[1]/*", row + 1)).Click();

            // Next to Step 3
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();

            // Edit Step 2 and verify you are at Step 2
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step2.aspx')]").Click();

            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath(TableIds.Groups_SpanOfCare_GroupTypes)));

            // Next to Step 3
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();

            // Next to Step 4
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();

            // Edit Step 3 and verify you are at Step 3
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]").Click();
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step3_CheckBox_Coed)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step3_CheckBox_Male)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step3_CheckBox_Female)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step3_CheckBox_MarriedOrSingle)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step3_CheckBox_Married)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step3_CheckBox_Single)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step3_DropDown_MinAgeRange)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step3_DropDown_MaxAgeRange)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step3_Radio_ChildcareProvided)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step3_Radio_ChildcareNotApplicable)));

            // Next to Step 4
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();

            // Next to Step 5
            test.Driver.FindElementByXPath(GeneralButtons.Next).Click();

            // Edit Step 4 and verify you are at Step 4 
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step4.aspx')]").Click();
            Assert.IsTrue(test.Driver.FindElementByTagName("html").Text.Contains("Automation Custom Field"));
            // Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver("Automation Custom Field"));

            // Cancel and Return
            test.Driver.FindElementByLinkText("RETURN").Click();

            // Logout
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("David Martin")]
        [Description("Verifies when you click the Edit Link for Step 1 from the Owners Page, you are taken to Step 1")]
        public void Groups_Administration_SpanOfCare_View_Edit_Step_1_WebDriver() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->span of care
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.Administration.Span_Of_Care);

            // View a Span of Care
            test.Driver.FindElementByLinkText("Automation Span of Care").Click();

            // Verify you are taken to the Owners page
            Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver("Automation Span of Care"));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//a[contains(@href, '/Groups/GroupSoc/Step1.aspx')]")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//a[contains(@href, '/Groups/GroupSoc/Step2.aspx')]")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//a[contains(@href, '/Groups/GroupSoc/Step4.aspx')]")));

            // Edit Step 1 from this page
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step1.aspx')]").Click();

            // Verify you are on Step 1 of the Wizard
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("soc_name")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step1_TextField_SpanOfCareDescription)));

            // Cancel and Return
            test.Driver.FindElementByLinkText("RETURN").Click();

            // Logout
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies when you click the Edit Link for Step 2 from the Owners Page, you are taken to Step 2")]
        public void Groups_Administration_SpanOfCare_View_Edit_Step_2_WebDriver() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->span of care
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.Administration.Span_Of_Care);
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Add"));

            // View a Span of Care
            test.Driver.FindElementByLinkText("Automation Span of Care").Click();

            // Verify you are taken to the Owners page
            Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver("Automation Span of Care"));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//a[contains(@href, '/Groups/GroupSoc/Step1.aspx')]")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//a[contains(@href, '/Groups/GroupSoc/Step2.aspx')]")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//a[contains(@href, '/Groups/GroupSoc/Step4.aspx')]")));

            // Edit Step 2 from this page
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step2.aspx')]").Click();

            // Verify you are on Step 2 of the Wizard
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath(TableIds.Groups_SpanOfCare_GroupTypes)));

            // Cancel and Return
            test.Driver.FindElementByLinkText("RETURN").Click();

            // Logout
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies when you click the Edit Link for Step 3 from the Owners Page, you are taken to Step 3")]
        public void Groups_Administration_SpanOfCare_View_Edit_Step_3_WebDriver() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->span of care
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.Administration.Span_Of_Care);

            // View a Span of Care
            test.Driver.FindElementByLinkText("Automation Span of Care").Click();

            // Verify you are taken to the Owners page
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Add"));
            Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver("Automation Span of Care"));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//a[contains(@href, '/Groups/GroupSoc/Step1.aspx')]")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//a[contains(@href, '/Groups/GroupSoc/Step2.aspx')]")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//a[contains(@href, '/Groups/GroupSoc/Step4.aspx')]")));

            // Edit Step 3 from this page
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]").Click();

            // Verify you are on Step 3 of the Wizard
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step3_CheckBox_Coed)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step3_CheckBox_Male)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step3_CheckBox_Female)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step3_CheckBox_MarriedOrSingle)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step3_CheckBox_Married)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step3_CheckBox_Single)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step3_DropDown_MinAgeRange)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step3_DropDown_MaxAgeRange)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step3_Radio_ChildcareProvided)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step3_Radio_ChildcareNotProvided)));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id(GroupsAdministrationConstants.Wizard_SpanOfCare.Step3_Radio_ChildcareNotApplicable)));

            // Cancel and Return
            test.Driver.FindElementByLinkText("RETURN").Click();

            // Logout
            test.Portal.LogoutWebDriver();

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies when you click the Edit Link for Step 4 from the Owners Page, you are taken to Step 4")]
        public void Groups_Administration_SpanOfCare_View_Edit_Step_4_WebDriver() {
            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->span of care
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.Administration.Span_Of_Care);

            // View a Span of Care
            test.Driver.FindElementByLinkText("Automation Span of Care").Click();

            // Verify you are taken to the Owners page
            Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver("Automation Span of Care"));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//a[contains(@href, '/Groups/GroupSoc/Step1.aspx')]")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//a[contains(@href, '/Groups/GroupSoc/Step2.aspx')]")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//a[contains(@href, '/Groups/GroupSoc/Step3.aspx')]")));
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.XPath("//a[contains(@href, '/Groups/GroupSoc/Step4.aspx')]")));

            // Edit Step 4 from this page
            test.Driver.FindElementByXPath("//a[contains(@href, '/Groups/GroupSoc/Step4.aspx')]").Click();
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Edit"));

            // Verify you are on Step 4 of the Wizard
            Assert.IsTrue(test.GeneralMethods.IsTextPresentWebDriver("Automation Custom Field"));

            // Cancel and Return
            test.Driver.FindElementByLinkText("RETURN").Click();

            // Logout
            test.Portal.LogoutWebDriver();

        }
        #endregion Navigation

        #region Owner Management
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("David Martin")]
        [Description("Verifies you can create/remove an owner for a Span of Care.")]
        public void Groups_Administration_SpanOfCare_Edit_Owner_WebDriver() {
            // Var initial data
            var groupType = "New Owner Soc";
            var spanOfCareName = "New Owner SOC";
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupType, new List<int> { 11 });
            base.SQL.Groups_SpanOfCare_Create(254, spanOfCareName, "Bryan Mikaelian", null, new List<string> { groupType });

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->span of care
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.Administration.Span_Of_Care);

            // Add an owner
            test.Portal.Groups_SpanOfCare_Update_AddOwner_WebDriver(spanOfCareName, "Bryan Mikaelian");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // View your groups
            test.Driver.FindElementByLinkText("Your Groups").Click();

            // Verify the Span of Care is present
            test.Driver.FindElementByLinkText(string.Format("{0}", spanOfCareName)).Click();

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Login to Portal
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // Delete the owner 
            test.Portal.Groups_SpanOfCare_Update_RemoveOwner_WebDriver(spanOfCareName, "Bryan Mikaelian");

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Login to InFellowship
            test.Infellowship.LoginWebDriver("bmikaelian@fellowshiptech.com", "BM.Admin09", "QAEUNLX0C2");

            // View your groups
            test.Driver.FindElementByLinkText("Your Groups").Click();

            // Verify the Span of Care is not present
            test.GeneralMethods.VerifyElementNotPresentWebDriver(By.LinkText(string.Format("{0}", spanOfCareName)));

            // Logout of InFellowship
            test.Infellowship.LogoutWebDriver();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);

        }

        [Test, RepeatOnFailure]
        [Author("David Martin")]
        [Description("Verifies you can create an owner for a Span of Care using advanced search")]
        public void Groups_Administration_SpanOfCare_Edit_Owner_Advanced_Search_WebDriver() {
            // Var initial data
            var groupType = "New Advanced Owner";
            var spanOfCareName = "New Advanced Owner SOC";
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupType, new List<int> { 11 });
            base.SQL.Groups_SpanOfCare_Create(254, spanOfCareName, "Bryan Mikaelian", null, new List<string> { groupType });

            // Login to Portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver("gta", "BM.Admin09", "QAEUNLX0C2");

            // Navigate to groups->span of care
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.Administration.Span_Of_Care);

            // View a Span of Care
            test.Portal.Groups_SpanOfCare_View_WebDriver(spanOfCareName);

            // Add an owner, using advanced search
            test.Driver.FindElementByXPath(GroupsAdministrationConstants.SpanOfCareManagement.Link_Owners_Add).Click();
            test.Driver.FindElementByLinkText("Advanced search").Click();
            new SelectElement(test.Driver.FindElementById(GroupsAdministrationConstants.SpanOfCareManagement.Dropdown_Owners_AttributeGroup)).SelectByText("Test Attribute");
            test.GeneralMethods.WaitForElement(test.Driver, By.XPath("//form[@id='adv_search']/table/tbody/tr[2]/td[2]/select[2]/option"));
            new SelectElement(test.Driver.FindElementById(GroupsAdministrationConstants.SpanOfCareManagement.Dropdown_Owners_IndividualAttribute)).SelectByText("Test Individual Attribute");
            test.Driver.FindElementById(GroupsAdministrationConstants.SpanOfCareManagement.Button_Owners_AdvancedSearch).Click();
            test.Driver.FindElementById(GroupsAdministrationConstants.SpanOfCareManagement.Button_Owners_AddNew).Click();

            // Verify the individual is present
            test.GeneralMethods.VerifyElementPresentWebDriver(By.LinkText("Bryan Mikaelian"));

            // Logout of Portal
            test.Portal.LogoutWebDriver();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupType);
            base.SQL.Groups_SpanOfCare_Delete(254, spanOfCareName);
        }

        #endregion Owner Management
    }

    [TestFixture]
    public class Portal_Groups_Administration_GroupTypes : FixtureBase {
        private string _groupTypeName = "General Group Type";

        #region FixtureSetUp
        [FixtureSetUp]
        public void FixtureSetUp() {
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", _groupTypeName, new List<int>() { 11 });

        }
        #endregion FixtureSetUp

        #region FixtureTearDown
        [FixtureTearDown]
        public void FixtureTearDown() {
            base.SQL.Groups_GroupType_Delete(254, _groupTypeName);
        }
        #endregion FixtureTearDown

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Creates a group type.")]
        public void Groups_Administration_GroupTypes_Create() {
            // Initial values
            var groupTypeName = "New Group Type";

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a new group type
            test.Portal.Groups_GroupType_Create(groupTypeName, null, false, false, new GeneralEnumerations.GroupTypeLeaderAdminRights[] { GeneralEnumerations.GroupTypeLeaderAdminRights.None }, false, GeneralEnumerations.GroupTypeLeaderViewRights.NotSpecified, GeneralEnumerations.GroupTypeMemberViewRights.NotSpecified, new string[] { "Mikaelian, Bryan" });

            // View the group types of the church
            test.Selenium.Navigate(Navigation.Groups.Administration.Group_Types);

            // Verify Group type was created
            Assert.IsTrue(test.Selenium.IsElementPresent("link=New Group Type"));

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Updates the properties of a group type.")]
        public void Groups_Administration_GroupTypes_Edit_Properties() {
            // Initial values
            var groupTypeName = "New Group Type - Update Prop";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Delete(254, "New UPDATED Group Type");
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int>() { 11 }, false);

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Update the properties of a group type
            test.Portal.Groups_GroupType_Update_Properties(groupTypeName, "New UPDATED Group Type", "Some description", true, true);

            test.Selenium.VerifyTextPresent("New UPDATED Group Type");
            test.Selenium.VerifyTextPresent("Some description");
            test.Selenium.VerifyTextPresent("Groups of this type are public");
            test.Selenium.VerifyTextPresent("Groups of this type are searchable");

            // Edit the properties of the group type and verify settings were applied.
            test.Selenium.ClickAndWaitForPageToLoad("link=Edit properties");

            Assert.AreEqual("New UPDATED Group Type", test.Selenium.GetValue("group_type_name"), "Group type name was not updated!");
            Assert.AreEqual("Some description", test.Selenium.GetValue("group_type_description"), "Group type description was not updated!");
            Assert.IsTrue(test.Selenium.IsChecked("group_type_web_enabled"), "The group type was made public but the public checkbox was not checked!");
            Assert.IsTrue(test.Selenium.IsChecked("group_type_searchable"), "Groups were made searchable but the searchable checkbox was not checked!");

            // Return and Back
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.Back);

            // Verify the new link is present
            test.Selenium.VerifyElementPresent("link=New UPDATED Group Type");

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, "New UPDATED Group Type");
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Deletes a group type.")]
        public void Groups_Administration_GroupTypes_Delete() {
            // Initial values
            var groupTypeName = "New Group Type - Delete";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int>() { 11 });

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Deletes a Group Type
            test.Portal.Groups_GroupType_Delete(groupTypeName);

            // Verify Group type was deleted
            test.Selenium.VerifyElementNotPresent(string.Format("link={0}", groupTypeName));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a group admin of a newely created group type can actually pick this group type to create a group.")]
        public void Groups_Administration_GroupTypes_Create_Group_Type_Can_Create_Group() {
            // Initial values
            var groupTypeName = "A Brand New Group Type";
            var groupName = "Brand New Group";
            
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a group type
            test.Portal.Groups_GroupType_Create(groupTypeName, null, new string[] { "Admin, Group" });

            // Logout of Portal
            test.Portal.Logout();

            // Login as a Group Admin
            test.Portal.Login("groupadmin", "BM.Admin09", "QAEUNLX0C2");

            // Attempt to create a group under the new type
            test.Portal.Groups_Group_Create(groupTypeName, groupName, null, "11/15/2009", false);

            // Groups -> View All
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // View the groups tab
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.ViewAllTabs.GroupsTab);

            // Verify the new group is present
            test.Selenium.VerifyElementPresent(string.Format("link={0}", groupName));

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a group admin of a deleted created group type can not pick this group type to create a group.")]
        public void Groups_Administration_GroupTypes_Delete_Group_Type_Cannot_Create_Group() {
            // Initial values
            var groupTypeName = "Delete Group Type";
            var groupName = "Brand New Group";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int>() { 11 });
           
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Deletes the Group Type
            test.Portal.Groups_GroupType_Delete(groupTypeName);

            // Logout
            test.Portal.Logout();

            // Login to Portal
            test.Portal.Login("groupadmin", "BM.Admin09", "QAEUNLX0C2");

            // Groups -> View All
            test.Selenium.Navigate(Navigation.Groups.GroupsByGroupType.View_All);

            // Add a group
            test.Selenium.ClickAndWaitForPageToLoad(GroupsByGroupTypeConstants.ViewAll.Link_NewGroup);

            // Verify the group type is not present
            test.Selenium.VerifyElementNotPresent(string.Format("link={0}", groupName));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the move group option is present and the page loads if you try to delete a group type with groups")]
        public void Groups_Administration_GroupTypes_Delete_Move_Groups_Availible() {
            // Initial values
            var groupTypeName = "Move Groups Group Type";
            var groupName = "Move Group";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int>() { 11 });
            base.SQL.Groups_Group_Create(254, groupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Delete a group type that has groups
            test.Portal.Groups_GroupType_View(groupTypeName);
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/GroupType/Delete.aspx')]");

            // Verify move this group link is present
            test.Selenium.VerifyElementPresent("link=Move groups first");

            // Click move this group
            test.Selenium.ClickAndWaitForPageToLoad("link=Move groups first");

            // Verify you are on the move page
            test.Selenium.VerifyTextPresent("Move Groups");
            test.Selenium.VerifyTextPresent("You may move the following groups to another group type...");

            // Logout
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_Group_Delete(254, groupName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies the move group option is not present for a group type you are trying to delete that has no groups under it.")]
        public void Groups_Administration_GroupTypes_Delete_Move_Groups_Not_Availible() {
            // Initial values
            var groupTypeName = "A Move Group Type - No Groups";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int>() { 11 });

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Delete a group type that has no groups
            test.Portal.Groups_GroupType_View(groupTypeName);
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/GroupType/Delete.aspx')]");

            // Verify move this group link is not present
            test.Selenium.VerifyElementNotPresent("link=Move groups first");

            // Return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies a portal user that is not a Group Type Admin cannot create a Group Type.")]
        public void Groups_Administration_GroupTypes_Non_GTAs_Cannot_Create() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("groupadmin", "BM.Admin09", "QAEUNLX0C2");

            // Verify Group Type link isn't present
            test.Selenium.Click("link=Groups");
            Assert.IsFalse(test.Selenium.IsElementPresent("link=Group Types"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can move a group from one group type to another")]
        public void Groups_Administration_GroupTypes_Edit_Move_Group() {
            // Initial values
            var sourceGroupTypeName = "Source";
            var destGroupTypeName = "Destination";
            var groupName = "Move this group";
            var otherGroupName = "Not moving";
            base.SQL.Groups_GroupType_Delete(254, sourceGroupTypeName);
            base.SQL.Groups_GroupType_Delete(254, destGroupTypeName);
            base.SQL.Groups_Group_Delete(254, groupName);
            base.SQL.Groups_Group_Delete(254, otherGroupName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", sourceGroupTypeName, new List<int>() { 11 });
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", destGroupTypeName, new List<int>() { 11 });
            base.SQL.Groups_Group_Create(254, sourceGroupTypeName, "Bryan Mikaelian", groupName, null, "11/15/2009");
            base.SQL.Groups_Group_Create(254, sourceGroupTypeName, "Bryan Mikaelian", otherGroupName, null, "11/15/2009");

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Move the group from one group type to the other
            test.Portal.Groups_GroupType_Update_MoveGroup(sourceGroupTypeName, destGroupTypeName, groupName);

            // Verify the group is part of the new group type
            test.Portal.Groups_GroupType_View(destGroupTypeName);
            test.Selenium.ClickAndWaitForPageToLoad("link=Manage/Move groups");
            test.Selenium.VerifyTextPresent(groupName);

            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Verify the group is not part of the old group type
            test.Portal.Groups_GroupType_View(sourceGroupTypeName);
            test.Selenium.ClickAndWaitForPageToLoad("link=Manage/Move groups");
            Assert.IsFalse(test.Selenium.IsTextPresent(groupName), "Moved group was still present under the old group type!");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, sourceGroupTypeName);
            base.SQL.Groups_GroupType_Delete(254, destGroupTypeName);
        }

        //[Test, RepeatOnFailure, MultipleAsserts]
        //[Author("Bryan Mikaelian")]
        //[Description("Verifies you can edit the permissions of a group type.")]
        //public void Groups_Administration_GroupTypes_Edit_Permissions() {
        //    // Login to Portal
        //    TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
        //    test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

        //    // View the group type
        //    test.Portal.Groups_GroupType_View(_groupTypeName);

        //    // Edit the permissions of the group type
        //    test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.GroupTypeManagement.Link_EditPermissions);

        //    // Save Changes
        //    test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.GroupTypeManagement.Button_EditPermissions_SaveChanges);

        //    // Logout of Portal
        //    test.Portal.Logout();
        //}

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that creating a Single select custom field results in the custom field showing up on the custom field edit page.")]
        public void Groups_Administration_GroupTypes_Edit_Custom_Fields_New_Single_Present() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a custom field
            string customFieldName = "New Single Select Custom Field";
            test.Portal.Groups_CustomField_Create(customFieldName, null, GeneralEnumerations.CustomFieldType.SingleSelect, new string[] { "1", "2", "3" });

            // View the group type
            test.Portal.Groups_GroupType_View(_groupTypeName);

            // Edit the custom fields of the group type
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/GroupType/EditCustomFields.aspx')]");

            // Verify New Custom Field appears 
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_GroupTypes_CustomFields, customFieldName, "Custom Field", "contains"));

            // Return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout of Portal
            test.Portal.Logout();

            // Delete the custom field
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that creating a multi select custom field results in the custom field showing up on the custom field edit page.")]
        public void Groups_Administration_GroupTypes_Edit_Custom_Fields_New_Multi_Present() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a custom field
            string customFieldName = "New Multi Select Custom Field - GT";
            test.Portal.Groups_CustomField_Create(customFieldName, null, GeneralEnumerations.CustomFieldType.MultiSelect, new string[] { "1", "2", "3" });

            // View the group type
            test.Portal.Groups_GroupType_View(_groupTypeName);

            // Edit the custom fields of the group type
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/GroupType/EditCustomFields.aspx')]");

            // Verify New Custom Field appears 
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_GroupTypes_CustomFields, customFieldName, "Custom Field", "contains"));

            // Return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout of Portal
            test.Portal.Logout();

            // Delete the custom field
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you can add new single select custom field with a default value to a group type.")]
        public void Groups_Administration_GroupTypes_Edit_Custom_Fields_Add_New_Single_With_Default() {
            // Intiial data
            var groupTypeName = "Default Single Select";
            var customFieldName = "New Single Select CF";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int>() { 11 }, false);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.SingleSelect, customFieldName, null, new List<string>() { "Yes", "No", "Dont care" });

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the group type
            test.Portal.Groups_GroupType_View(groupTypeName);

            // Edit the custom fields of the group type
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/GroupType/EditCustomFields.aspx')]");

            // Add an existing Single Select custom field to this group type and specify a default value
            test.Portal.Groups_GroupType_Update_CustomField(customFieldName, "Yes");

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.GroupTypeManagement.Button_EditCustomFields_SaveChanges);

            // Verify it was applied to the group type
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_GroupTypes_LandingPage_CustomFields, customFieldName, "Custom Field", null), "Custom field was not applied to the group type.");

            // Edit the custom fields
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/GroupType/EditCustomFields.aspx')]");

            // Verify the default was picked
            test.Portal.Groups_GroupTypes_Update_CustomFields_Verify_Default_Checked(true, string.Format("{0} Not specified Yes No Dont care", customFieldName), "Yes");

            // Uncheck this custom field
            test.Portal.Groups_GroupTypes_Update_UnSelectCustomField(string.Format("{0} Not specified Yes No Dont care", customFieldName), null);

            // Verify the custom field doesn't exist
            Assert.IsFalse(test.Selenium.IsTextPresent(customFieldName), "Removed custom field was still applied to the group type!");

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you can add new single select custom field without a default value to a group type.")]
        public void Groups_Administration_GroupTypes_Edit_Custom_Fields_Add_New_Single_No_Default() {
            // Intiial data
            var groupTypeName = "No Default Single Select";
            var customFieldName = "New Single Select CF - No Default";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int>() { 11 }, false);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.SingleSelect, customFieldName, null, new List<string>() { "Yes", "No", "Dont care" });

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the group type
            test.Portal.Groups_GroupType_View(groupTypeName);

            // Edit the custom fields of the group type
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/GroupType/EditCustomFields.aspx')]");

            // Add an existing Single Select custom field to this group type and but do not specify a default
            test.Portal.Groups_GroupType_Update_CustomField(customFieldName, null);

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad("update_customfields");

            // Verify it was applied to the group type
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_GroupTypes_LandingPage_CustomFields, customFieldName, "Custom Field", null), "Custom field was not applied to the group type.");

            // Edit the custom fields
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/GroupType/EditCustomFields.aspx')]");

            // Verify Not specified is picked
            test.Portal.Groups_GroupTypes_Update_CustomFields_Verify_Default_Checked(true, string.Format("{0} Not specified Yes No Dont care", customFieldName), "Not specified");
            test.Portal.Groups_GroupTypes_Update_CustomFields_Verify_Default_Checked(false, string.Format("{0} Not specified Yes No Dont care", customFieldName), "Yes");
            test.Portal.Groups_GroupTypes_Update_CustomFields_Verify_Default_Checked(false, string.Format("{0} Not specified Yes No Dont care", customFieldName), "No");
            test.Portal.Groups_GroupTypes_Update_CustomFields_Verify_Default_Checked(false, string.Format("{0} Not specified Yes No Dont care", customFieldName), "Dont care");

            // Uncheck this custom field
            test.Portal.Groups_GroupTypes_Update_UnSelectCustomField(string.Format("{0} Not specified Yes No Dont care", customFieldName), null);

            // Verify the custom field doesn't exist
            Assert.IsFalse(test.Selenium.IsTextPresent(customFieldName), "Removed custom field was still applied to the group type!");

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you can add new multi select custom field without a default value to a group type.")]
        public void Groups_Administration_GroupTypes_Edit_Custom_Fields_Add_New_Multi_No_Default() {
            // Intiial data
            var groupTypeName = "No Default Multi Select";
            var customFieldName = "New Multi Select CF - No Default";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int>() { 11 }, false);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.MultiSelect, customFieldName, null, new List<string>() { "Yes", "No", "Dont care" });

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the group type
            test.Portal.Groups_GroupType_View(groupTypeName);

            // Edit the custom fields of the group type
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/GroupType/EditCustomFields.aspx')]");

            // Add an existing multi select custom field to this group type and but do not specify a default
            test.Portal.Groups_GroupType_Update_CustomField(customFieldName, null);

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad("update_customfields");

            // Verify it was applied to the group type
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_GroupTypes_LandingPage_CustomFields, customFieldName, "Custom Field", null), "Custom field was not applied to the group type.");

            // Edit the custom fields
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/GroupType/EditCustomFields.aspx')]");

            // Verify no default is picked
            test.Portal.Groups_GroupTypes_Update_CustomFields_Verify_Default_Checked(false, string.Format("{0} Yes No Dont care", customFieldName), "Yes");
            test.Portal.Groups_GroupTypes_Update_CustomFields_Verify_Default_Checked(false, string.Format("{0} Yes No Dont care", customFieldName), "No");
            test.Portal.Groups_GroupTypes_Update_CustomFields_Verify_Default_Checked(false, string.Format("{0} Yes No Dont care", customFieldName), "Dont care");

            // Uncheck this custom field
            test.Portal.Groups_GroupTypes_Update_UnSelectCustomField(string.Format("{0} Yes No Dont care", customFieldName), null);

            // Verify the custom field doesn't exist
            Assert.IsFalse(test.Selenium.IsTextPresent(customFieldName), "Removed custom field was still applied to the group type!");

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that you can add new multi select custom field with a default value to a group type.")]
        public void Groups_Administration_GroupTypes_Edit_Custom_Fields_Add_New_Multi_With_Default() {
            // Intiial data
            var groupTypeName = "Default Multi Select";
            var customFieldName = "New Multi Select CF - Default";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int>() { 11 }, false);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.MultiSelect, customFieldName, null, new List<string>() { "Yes", "No", "Dont care" });

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the group type
            test.Portal.Groups_GroupType_View(groupTypeName);

            // Edit the custom fields of the group type
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/GroupType/EditCustomFields.aspx')]");

            // Add an existing multi select custom field to this group type and specify a default
            test.Portal.Groups_GroupType_Update_CustomField(customFieldName, "Yes");

            // Submit
            test.Selenium.ClickAndWaitForPageToLoad("update_customfields");

            // Verify it was applied to the group type
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_GroupTypes_LandingPage_CustomFields, customFieldName, "Custom Field", null), "Custom field was not applied to the group type.");

            // Edit the custom fields
            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/GroupType/EditCustomFields.aspx')]");

            // Verify the default is picked
            test.Portal.Groups_GroupTypes_Update_CustomFields_Verify_Default_Checked(true, string.Format("{0} Yes No Dont care", customFieldName), "Yes");
            test.Portal.Groups_GroupTypes_Update_CustomFields_Verify_Default_Checked(false, string.Format("{0} Yes No Dont care", customFieldName), "No");
            test.Portal.Groups_GroupTypes_Update_CustomFields_Verify_Default_Checked(false, string.Format("{0} Yes No Dont care", customFieldName), "Dont care");

            // Uncheck this custom field
            test.Portal.Groups_GroupTypes_Update_UnSelectCustomField(string.Format("{0} Yes No Dont care", customFieldName), null);

            // Verify the custom field doesn't exist
            Assert.IsFalse(test.Selenium.IsTextPresent(customFieldName), "Removed custom field was still applied to the group type!");

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies if you select only person to have administrative rights to this group type, the sidebar does not show this as '1 people.'")]
        public void Groups_Administration_GroupTypes_Create_Sidebar_Link_Correct_For_One_Individual() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a new group type
            test.Selenium.Navigate(Navigation.Groups.Administration.Group_Types);
            test.Selenium.ClickAndWaitForPageToLoad("link=New group type");

            // Enter a name
            test.Selenium.Type("group_type_name", "Test Group Type A");

            // Navigate through the wizard to Step 4
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step3_Button_Next);

            // Select an individual
            decimal itemRow = test.GeneralMethods.GetTableRowNumber("//table[@class='grid']", "Gutekunst, Coly", "Portal User — must be linked to an individual record", null);
            test.Selenium.Click(string.Format("//table[@class='grid']/tbody/tr[{0}]/td[2]/input", itemRow + 1));
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step4_Button_Next);

            // Verify sidebar link is correct
            Assert.IsFalse(test.Selenium.IsElementPresent("link=1 people"));
            Assert.IsTrue(test.Selenium.IsElementPresent("link=1 person"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that all values are persisted from Step 1 if you navigate forward in the wizard, then backwards.")]
        public void Groups_Administration_GroupTypes_Create_Values_Persisted_Step_1() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a new group type
            test.Selenium.Navigate(Navigation.Groups.Administration.Group_Types);
            test.Selenium.ClickAndWaitForPageToLoad("link=New group type");

            // Enter a name and a description
            test.Selenium.Type("group_type_name", "Test Group Type A");
            test.Selenium.Type(GroupsAdministrationConstants.Wizard_GroupType.Step1_TextField_GroupTypeDescription, "I love Dinosaurs.");

            // Make the group public and searchable
            test.Selenium.Click(GroupsAdministrationConstants.Wizard_GroupType.Step1_CheckBox_GroupsPublic);
            test.Selenium.Click(GroupsAdministrationConstants.Wizard_GroupType.Step1_CheckBox_GroupsSearchable);

            // Hit Next
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Hit Back
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step3_Link_Back);

            // Verify the information on Step 1 is persisted
            Assert.AreEqual("Test Group Type A", test.Selenium.GetValue("group_type_name"));
            Assert.AreEqual("I love Dinosaurs.", test.Selenium.GetValue(GroupsAdministrationConstants.Wizard_GroupType.Step1_TextField_GroupTypeDescription));
            Assert.IsTrue(test.Selenium.IsChecked(GroupsAdministrationConstants.Wizard_GroupType.Step1_CheckBox_GroupsPublic));
            Assert.IsTrue(test.Selenium.IsChecked(GroupsAdministrationConstants.Wizard_GroupType.Step1_CheckBox_GroupsSearchable));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that all values are persisted from Step 2 if you navigate forward in the wizard, then backwards.")]
        public void Groups_Administration_GroupTypes_Create_Values_Persisted_Step_2() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a new group type
            test.Selenium.Navigate(Navigation.Groups.Administration.Group_Types);
            test.Selenium.ClickAndWaitForPageToLoad("link=New group type");

            // Enter a name and a description
            test.Selenium.Type("group_type_name", "Test Group Type A");

            // Hit Next
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Check All the rights
            test.Selenium.Click("group_type_leaders_can_email");
            test.Selenium.Click("group_type_leaders_can_invite");
            test.Selenium.Click("group_type_leaders_can_add");
            test.Selenium.Click("group_type_leaders_can_edit");
            test.Selenium.Click("group_type_leaders_can_updategroup");
            test.Selenium.Click("group_type_leaders_can_schedule");
            test.Selenium.Click("group_type_members_can_email");

            // Hit Next
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Hit Back
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step3_Link_Back);

            // Verify all values are checked
            Assert.IsTrue(test.Selenium.IsChecked("group_type_leaders_can_email"));
            Assert.IsTrue(test.Selenium.IsChecked("group_type_leaders_can_invite"));
            test.Selenium.Click("group_type_leaders_can_add");
            Assert.IsTrue(test.Selenium.IsChecked("group_type_leaders_can_edit"));
            Assert.IsTrue(test.Selenium.IsChecked("group_type_leaders_can_updategroup"));
            Assert.IsTrue(test.Selenium.IsChecked("group_type_leaders_can_schedule"));
            Assert.IsTrue(test.Selenium.IsChecked("group_type_members_can_email"));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that all values are persisted from Step 3 if you navigate forward in the wizard, then backwards.")]
        public void Groups_Administration_GroupTypes_Create_Values_Persisted_Step_3() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a new group type
            test.Selenium.Navigate(Navigation.Groups.Administration.Group_Types);
            test.Selenium.ClickAndWaitForPageToLoad("link=New group type");

            // Enter a name and a description
            test.Selenium.Type("group_type_name", "Test Group Type A");

            // Hit Next
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Hit Next
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Change the View rights
            test.Selenium.Click(GroupsAdministrationConstants.Wizard_GroupType.Step3_Radio_LeadersFullInformation);
            test.Selenium.Click(GroupsAdministrationConstants.Wizard_GroupType.Step3_Radio_MembersBasicInformation);

            // Hit Next
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step3_Button_Next);

            // Hit Back
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step5_Link_Back);

            // Verify rights were saved
            Assert.IsTrue(test.Selenium.IsChecked(GroupsAdministrationConstants.Wizard_GroupType.Step3_Radio_LeadersFullInformation));
            Assert.IsTrue(test.Selenium.IsChecked(GroupsAdministrationConstants.Wizard_GroupType.Step3_Radio_MembersBasicInformation));

            // Logout
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian")]
        [Description("Verifies that all values are persisted from Step 4 if you navigate forward in the wizard, then backwards.")]
        public void Groups_Administration_GroupTypes_Create_Values_Persisted_Step_4() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a new group type
            test.Selenium.Navigate(Navigation.Groups.Administration.Group_Types);
            test.Selenium.ClickAndWaitForPageToLoad("link=New group type");

            // Enter a name and a description
            test.Selenium.Type("group_type_name", "Test Group Type A");

            // Hit Next
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Hit Next
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Hit Nexts
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step3_Button_Next);

            // Grant permission to some user
            decimal itemRow = test.GeneralMethods.GetTableRowNumber("//table[@class='grid']", "Gutekunst, Coly", "Portal User — must be linked to an individual record", null);
            test.Selenium.Click(string.Format("//table[@class='grid']/tbody/tr[{0}]/td[2]/input", itemRow + 1));

            // Hit Next
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step4_Button_Next);

            // Hit Back
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step5_Link_Back);

            // Verify the individual is still checked
            Assert.IsTrue(test.Selenium.IsChecked(string.Format("//table[@class='grid']/tbody/tr[{0}]/td[2]/input", test.GeneralMethods.GetTableRowNumber("//table[@class='grid']", "Gutekunst, Coly", "Portal User — must be linked to an individual record", null) + 1)));

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian", "bmikaelian@fellowshiptech.com")]
        [Description("Verifies that creating a Single selectcustom field results in the custom field showing up in the group type wizard.")]
        public void Groups_Administration_GroupTypes_Create_Custom_Fields_New_Single_Present() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a custom field
            string customFieldName = "New Single select Custom Field - GT";
            test.Portal.Groups_CustomField_Create(customFieldName, null, GeneralEnumerations.CustomFieldType.SingleSelect, new string[] { "1", "2", "3" });

            // Create a group type, verify new custom field is present
            test.Selenium.Navigate(Navigation.Groups.Administration.Group_Types);
            test.Selenium.ClickAndWaitForPageToLoad("link=New group type");

            // Name the group type
            test.Selenium.Type("group_type_name", "New Group Type - CF, SS");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Move to views
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Move to permissions
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step3_Button_Next);

            // Move to custom fields
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step4_Button_Next);

            // Verify new custom field is present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_GroupTypes_CustomFields, customFieldName, "Custom Field", "contains"));

            // Return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

			// Delete the custom field
            test.Portal.Groups_CustomField_Delete(customFieldName);

			// Logout of Portal
			test.Portal.Logout();
		}

        [Test, RepeatOnFailure, MultipleAsserts]
        [Author("Bryan Mikaelian", "bmikaelian@fellowshiptech.com")]
        [Description("Verifies that an existing Single selectcustom field without a shows up in the group type wizard.")]
        public void Groups_Administration_GroupTypes_Create_Custom_Fields_Existing_Single_Present() {
            // Intiial data
            var customFieldName = "Existing Single Select";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.SingleSelect, customFieldName, null, new List<string>() { "A" });

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a group type, verify new custom field is present
            test.Selenium.Navigate(Navigation.Groups.Administration.Group_Types);
            test.Selenium.ClickAndWaitForPageToLoad("link=New group type");

            // Name the group type
            test.Selenium.Type("group_type_name", "New Group Type - CF, ES");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Move to views
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Move to permissions
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step3_Button_Next);

            // Move to custom fields
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step4_Button_Next);

            // Verify existing custom field without a default is present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_GroupTypes_CustomFields, customFieldName, "Custom Field", "contains"));

			// Logout of Portal
			test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
		}

		[Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian", "bmikaelian@fellowshiptech.com")]
		[Description("Verifies that creating a multi select custom field results in the custom field showing up in the group type wizard.")]
		public void Groups_Administration_GroupTypes_Create_Custom_Fields_New_Multi_Present() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a custom field
            string customFieldName = "New Multi select Custom Field";
            test.Portal.Groups_CustomField_Create(customFieldName, null, GeneralEnumerations.CustomFieldType.MultiSelect, new string[] { "1", "2", "3" });

            // Create a group type, verify new custom field is present
            test.Selenium.Navigate(Navigation.Groups.Administration.Group_Types);
            test.Selenium.ClickAndWaitForPageToLoad("link=New group type");

            // Name the group type
            test.Selenium.Type("group_type_name", "New Group Type - CF, NM");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Move to views
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Move to permissions
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step3_Button_Next);

            // Move to custom fields
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step4_Button_Next);

            // Verify new custom field is present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_GroupTypes_CustomFields, customFieldName, "Custom Field", "contains"));

			// Return
			test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

			// Delete the custom field
            test.Portal.Groups_CustomField_Delete(customFieldName);

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian", "bmikaelian@fellowshiptech.com")]
		[Description("Verifies that an existing multi-select custom field without a shows up in the group type wizard.")]
		public void Groups_Administration_GroupTypes_Create_Custom_Fields_Existing_Multi_Present() {
            // Intiial data
            var customFieldName = "Existing Multi Select";
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
            base.SQL.Groups_CustomField_Create(254, GeneralEnumerations.CustomFieldType.MultiSelect, customFieldName, null, new List<string>() { "A" });

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a group type, verify new custom field is present
            test.Selenium.Navigate(Navigation.Groups.Administration.Group_Types);
            test.Selenium.ClickAndWaitForPageToLoad("link=New group type");

            // Name the group type
            test.Selenium.Type("group_type_name", "New Group Type - CF, EM");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Move to views
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Move to permissions
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step3_Button_Next);

            // Move to custom fields
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step4_Button_Next);

            // Verify existing custom field without a default is present
            Assert.IsTrue(test.GeneralMethods.ItemExistsInTable(TableIds.Groups_GroupTypes_CustomFields, customFieldName, "Custom Field", "contains"));

			// Logout of Portal
			test.Portal.Logout();

            // Clean up
            base.SQL.Groups_CustomField_Delete(254, customFieldName);
		}

		[Test, RepeatOnFailure, MultipleAsserts]
		[Author("Bryan Mikaelian")]
		[Description("Verifies that if nothing is selected on Step 2, the sidebar still shows what leaders and members can do.")]
		public void Groups_Administration_GroupTypes_Create_Sidebar_Shows_Step_2_If_Nothing_Selected() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a new group type
            test.Selenium.Navigate(Navigation.Groups.Administration.Group_Types);
            test.Selenium.ClickAndWaitForPageToLoad("link=New group type");

            // Enter a name and a description
            test.Selenium.Type("group_type_name", "Test Group Type A");

            // Hit Next
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Hit Next, not specifying any rights
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify step 2 shows up on the side bar
            Assert.IsTrue(test.Selenium.IsTextPresent("Group leaders can…"));
            Assert.IsTrue(test.Selenium.IsTextPresent("Group members can…"));

            // Cancel and Return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout
            test.Portal.Logout();
        }

        #region Navigation
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you can navigate forwards and backwards through the new wizard.")]
        public void Groups_Administration_GroupTypes_Create_Navigate_Through_Wizard() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a new group type
            test.Selenium.Navigate(Navigation.Groups.Administration.Group_Types);
            test.Selenium.ClickAndWaitForPageToLoad("link=New group type");

            // Name
            test.Selenium.Type("group_type_name", "Test Group Type A");

            // Navigate through the wizard
            test.Selenium.VerifyTitle("Fellowship One :: Step 1 Group Type Wizard");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
            test.Selenium.VerifyTitle("Fellowship One :: Step 2 Group Type Wizard");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
            test.Selenium.VerifyTitle("Fellowship One :: Step 3 Group Type Wizard");
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step3_Button_Next);
            test.Selenium.VerifyTitle("Fellowship One :: Step 4 Group Type Wizard");
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step4_Button_Next);
            test.Selenium.VerifyTitle("Fellowship One :: Step 5 Group Type Wizard");

            // Navigate backwards through the wizard
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step5_Link_Back);
            test.Selenium.VerifyTitle("Fellowship One :: Step 4 Group Type Wizard");
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step4_Link_Back);
            test.Selenium.VerifyTitle("Fellowship One :: Step 3 Group Type Wizard");
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step3_Link_Back);
            test.Selenium.VerifyTitle("Fellowship One :: Step 2 Group Type Wizard");
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step2_Link_Back);
            test.Selenium.VerifyTitle("Fellowship One :: Step 1 Group Type Wizard");

            // Cancel And Return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Verify you are on the Group Type page
            test.Selenium.VerifyTitle("Fellowship One :: Group Types");

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies the edit links in the wizard go to the correct pages.")]
		public void Groups_Administration_GroupTypes_Create_Edit_Links_Go_To_Correct_Page() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a new group type
            test.Selenium.Navigate(Navigation.Groups.Administration.Group_Types);
            test.Selenium.ClickAndWaitForPageToLoad("link=New group type");

            // Name
            test.Selenium.Type("group_type_name", "Automation Group Type A");

            // Go to Step 2
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Edit Step 1
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Link_EditStep1);

            // Verify you are on step 1
            test.Selenium.VerifyTitle("Fellowship One :: Step 1 Group Type Wizard");

            // Go to Step 3
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step3_Button_Next);

            // Edit Step 2
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Link_EditStep2);

            // Verify you are on Step 2
            test.Selenium.VerifyTitle("Fellowship One :: Step 2 Group Type Wizard");

            // Go to step 4 
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step3_Button_Next);

            // Edit step 3
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Link_EditStep3Link);

            // Verify you are on Step 3
            test.Selenium.VerifyTitle("Fellowship One :: Step 3 Group Type Wizard");

            // Go to Step 5
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step3_Button_Next);
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Step4_Button_Next);

            // Edit Step 4
            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.Wizard_GroupType.Link_EditStep4);

            // Verify you are on Step 4
            test.Selenium.VerifyTitle("Fellowship One :: Step 4 Group Type Wizard");

            // Cancel And Return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Verify you are on the Group Type page
            test.Selenium.VerifyTitle("Fellowship One :: Group Types");

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies group type names cannot exceed 100 characters.")]
        public void Groups_Administration_GroupTypes_Create_Name_Exceed_100_Characters() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a new group type
            test.Selenium.Navigate(Navigation.Groups.Administration.Group_Types);
            test.Selenium.ClickAndWaitForPageToLoad("link=New group type");

            // Set the name to be 100 characters long
            test.Selenium.Type("group_type_name", "Automation Automation Automation Automation Automation Automation Automation Automation Automation Automation");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify error message
            Assert.IsTrue(test.Selenium.IsTextPresent("Group Type Name is required and cannot exceed 100 characters."));

            // Cancel and return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies group type names cannot exceed 100 characters.")]
        public void Groups_Administration_GroupTypes_Create_Description_Exceed_500_Characters() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a new group type
            test.Selenium.Navigate(Navigation.Groups.Administration.Group_Types);
            test.Selenium.ClickAndWaitForPageToLoad("link=New group type");

            // Set the name
            test.Selenium.Type("group_type_name", "Dinosaur Group Type");

            // Set the description to be 500 characters long
            test.Selenium.Type(GroupsAdministrationConstants.Wizard_GroupType.Step1_TextField_GroupTypeDescription, "Automation Automation Automation Automation Automation Automation Automation Automation Automation Automation Automation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation Automation");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify error message
            Assert.IsTrue(test.Selenium.IsTextPresent("Group Type Description cannot exceed 500 characters."));

            // Cancel and return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies group type names must be unique.")]
        public void Groups_Administration_GroupTypes_Create_Unique_Name() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a new group type
            test.Selenium.Navigate(Navigation.Groups.Administration.Group_Types);
            test.Selenium.ClickAndWaitForPageToLoad("link=New group type");

            // Create a group type with the same name as an existing group type
            test.Selenium.Type("group_type_name", _groupTypeName);
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify error message
            test.Selenium.VerifyTextPresent("Group Type Name must be unique.");

            // Cancel and return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies you are prompted about group type names being unique if you try to name an existing group type to a name already in use.")]
        public void Groups_Administration_GroupTypes_Edit_Unique_Name() {
            // Initial values
            var groupTypeName = "Existing Group Type - Edit Unique";
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
            base.SQL.Groups_GroupType_Create(254, "Bryan Mikaelian", groupTypeName, new List<int>() { 11 }, false);

            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the group type
            test.Portal.Groups_GroupType_View(_groupTypeName);

            // Edit the properties of the group type
            test.Selenium.ClickAndWaitForPageToLoad("link=Edit properties");

            // Update the name and the description to a name already in use
            test.Selenium.Type("group_type_name", groupTypeName);
            test.Selenium.ClickAndWaitForPageToLoad("update_properties");

            // Verify error message
            test.Selenium.VerifyTextPresent("Group Type Name must be unique.");

            // Cancel and return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout of Portal
            test.Portal.Logout();

            // Clean up
            base.SQL.Groups_GroupType_Delete(254, groupTypeName);
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies group type names cannot exceed 100 characters if you try to edit one to exceed 100 characters.")]
        public void Groups_Administration_GroupTypes_Edit_Name_Exceed_100_Characters() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the group type
            test.Portal.Groups_GroupType_View(_groupTypeName);

            // Edit the properties of the group type
            test.Selenium.ClickAndWaitForPageToLoad("link=Edit properties");

            // Edit the name to be 100 characters long
            test.Selenium.Type("group_type_name", "Automation Automation Automation Automation Automation Automation Automation Automation Automation Automation");
            test.Selenium.ClickAndWaitForPageToLoad("update_properties");

            // Verify error message
            test.Selenium.VerifyTextPresent("Group Type Name is required and cannot exceed 100 characters.");

            // Cancel and return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies group type descriptions cannot exceed 500 characters if you try to edit one to exceed 500 characters.")]
        public void Groups_Administration_GroupTypes_Edit_Description_Exceed_500_Characters() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // View the group type            
            test.Portal.Groups_GroupType_View(_groupTypeName);

            // Edit the properties of the group type
            test.Selenium.ClickAndWaitForPageToLoad("link=Edit properties");

            // Edit the description to be 500 characters long
            test.Selenium.Type(GroupsAdministrationConstants.Wizard_GroupType.Step1_TextField_GroupTypeDescription, "Automation Automation Automation Automation Automation Automation Automation Automation Automation Automation Automation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation AutomationAutomation Automation Automation Automation Automation Automation Automation Automation Automation Automation");
            test.Selenium.ClickAndWaitForPageToLoad("update_properties");

            // Verify error message
            Assert.IsTrue(test.Selenium.IsTextPresent("Group Type Description cannot exceed 500 characters."));

            // Cancel and return
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout of Portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Bryan Mikaelian")]
        [Description("Verifies group type name is required.")]
        public void Groups_Administration_GroupTypes_Create_Name_Required() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

            // Create a new group type
            test.Selenium.Navigate(Navigation.Groups.Administration.Group_Types);
            test.Selenium.ClickAndWaitForPageToLoad("link=New group type");

            // Create a group type with no name
            test.Selenium.ClickAndWaitForPageToLoad(GeneralButtons.submitQuery);

            // Verify error message
            Assert.IsTrue(test.Selenium.IsTextPresent("Group Type Name is required and cannot exceed 100 characters."));

			// Cancel and return
			test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Bryan Mikaelian")]
		[Description("Verifies clicking on each edit link go to the correct pages when viewing a group type added edit links.")]
		public void Groups_Administration_GroupTypes_Edit_Links_Go_To_Correct_Page() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login("gta", "BM.Admin09", "QAEUNLX0C2");

			// View the group type
			test.Portal.Groups_GroupType_View(_groupTypeName);

			// Click each link, verify you are on the page, and return back to current group type's page.
			test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.GroupTypeManagement.Link_EditAdminRights);
			test.Selenium.VerifyTitle("Fellowship One :: Edit Group Type Rights");
			test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.GroupTypeManagement.Link_EditViewRights);
            test.Selenium.VerifyTitle("Fellowship One :: Edit Group Type Views");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            test.Selenium.ClickAndWaitForPageToLoad(GroupsAdministrationConstants.GroupTypeManagement.Link_EditPermissions);
            test.Selenium.VerifyTitle("Fellowship One :: Edit Group Type Permissions");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            test.Selenium.ClickAndWaitForPageToLoad("//a[contains(@href, '/Groups/GroupType/EditCustomFields.aspx')]");
            test.Selenium.VerifyTitle("Fellowship One :: Edit Group Type Custom Fields");
            test.Selenium.ClickAndWaitForPageToLoad(GeneralLinks.RETURN);

            // Logout
            test.Portal.Logout();
        }
        #endregion Navigation

        #region Unlinked Users
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Bryan Mikaelian")]
        [Description("Verifies unlinked users do not have access to Group Type Admin roles")]
        public void Groups_Administration_Unlinked_Users() {
            // Login to Portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login("unlinkeduser", "BM.Admin09", "QAEUNLX0C2");

            // Groups -> Administration
            // Verify message
            test.Selenium.Navigate(Navigation.Groups.Administration.Group_Types);
            Assert.IsTrue(test.Selenium.IsTextPresent("Your user account is not set up to use Groups."));

            test.Selenium.Navigate(Navigation.Groups.Administration.Custom_Fields);
            Assert.IsTrue(test.Selenium.IsTextPresent("Your user account is not set up to use Groups."));

            test.Selenium.Navigate(Navigation.Groups.Administration.Search_Categories);
            Assert.IsTrue(test.Selenium.IsTextPresent("Your user account is not set up to use Groups."));

            test.Selenium.Navigate(Navigation.Groups.Administration.Span_Of_Care);
            Assert.IsTrue(test.Selenium.IsTextPresent("Your user account is not set up to use Groups."));

            // Logout
            test.Portal.Logout();
        }
        #endregion Unlinked Users

    }
}