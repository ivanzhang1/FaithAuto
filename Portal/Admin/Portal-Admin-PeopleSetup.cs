using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace FTTests.Portal.Admin {
	[TestFixture]
	public class Portal_Admin_PeopleSetup : FixtureBase {
		#region Status
		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Matthew Sneeden")]
		[Description("Attempts to create a status without one or more required fields.")]
		public void Admin_PeopleSetup_Status_CreateNoData() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Attempt to create a status without one or more required fields
            test.Portal.Admin_Status_Create(null, null, false, true);

			// Logout of portal
			test.Portal.Logout();
		}
		#endregion Status

		#region Individual Attributes
		#region Individual Attribute Groups
		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Matthew Sneeden")]
		[Description("Creates an Individual Attribute Group.")]
		public void Admin_PeopleSetup_IndividualAttributeGroups_Create() {
            // Set initial conditions
            string individualAttributeGroupName = "Test Attribute Group";
            base.SQL.Admin_IndividualAttributeGroups_Delete(15, individualAttributeGroupName);

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Create an individual attribute group
            test.Portal.Admin_IndividualAttributeGroups_Create(individualAttributeGroupName, true);

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Updates an Individual Attribute Group.")]
		public void Admin_PeopleSetup_IndividualAttributeGroups_Update() {
            // Set initial conditions
            string individualAttributeGroupName = "Test Attribute Group - U";
            string individualAttributeGroupNameUpdated = "Test Attribute Group - Updated";
            base.SQL.Admin_IndividualAttributeGroups_Delete(15, individualAttributeGroupName);
            base.SQL.Admin_IndividualAttributeGroups_Delete(15, individualAttributeGroupNameUpdated);
            base.SQL.Admin_IndividualAttributeGroups_Create(15, individualAttributeGroupName, true);

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Update an individual attribute group
            test.Portal.Admin_IndividualAttributeGroups_Update(individualAttributeGroupName, true, individualAttributeGroupNameUpdated, true);

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Deletes an Individual Attribute Group.")]
		public void Admin_PeopleSetup_IndividualAttributeGroups_Delete() {
            // Set initial conditions
            string individualAttributeGroupName = "Test Attribute Group - Delete";
            base.SQL.Admin_IndividualAttributeGroups_Delete(15, individualAttributeGroupName);
            base.SQL.Admin_IndividualAttributeGroups_Create(15, individualAttributeGroupName, true);

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Delete an individual attribute group
            test.Portal.Admin_IndividualAttributeGroups_Delete(individualAttributeGroupName);

			// Logout of Portal
			test.Portal.Logout();
		}
		#endregion Individual Attribute Groups

		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Matthew Sneeden")]
		[Description("Creates an Individual Attribute.")]
		public void Admin_PeopleSetup_IndividualAttributes_Create() {
            // Set initial conditions
            string individualAttributeName = "Test Individual Attribute";
            string individualAttributeGroupName = "Test Attribute Group - A:Create";
            base.SQL.Admin_IndividualAttributes_Delete(15, individualAttributeGroupName, individualAttributeName);
            base.SQL.Admin_IndividualAttributeGroups_Delete(15, individualAttributeGroupName);
            base.SQL.Admin_IndividualAttributeGroups_Create(15, individualAttributeGroupName, true);

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();
            
			// Create an individual attribute
			test.Portal.Admin_IndividualAttributes_Create(individualAttributeGroupName, individualAttributeName, true, true, true, true, true);

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Updates an Individual Attribute.")]
		public void Admin_PeopleSetup_IndividualAttributes_Update() {
            // Set initial conditions
            string individualAttributeGroupName = "Test Attribute Group - A:Update";
            string individualAttributeName = "Test Individual Attribute";
            string individualAttributeNameUpdated = "Test Individual Attribute - Updated";
            base.SQL.Admin_IndividualAttributes_Delete(15, individualAttributeGroupName, individualAttributeName);
            base.SQL.Admin_IndividualAttributes_Delete(15, individualAttributeGroupName, individualAttributeNameUpdated);
            base.SQL.Admin_IndividualAttributeGroups_Delete(15, individualAttributeGroupName);
            base.SQL.Admin_IndividualAttributeGroups_Create(15, individualAttributeGroupName, true);
            base.SQL.Admin_IndividualAttributes_Create(15, individualAttributeGroupName, individualAttributeName, true, true, true, true, true);

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Update an individual attribute
            test.Portal.Admin_IndividualAttributes_Update(individualAttributeGroupName, individualAttributeName, true, true, true, true, true, individualAttributeNameUpdated, false, false, false, false, true);

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Deletes an Individual Attribute.")]
		public void Admin_PeopleSetup_IndividualAttributes_Delete() {
            // Set initial conditions
            string individualAttributeGroupName = "Test Attribute Group - A:Delete";
            string individualAttributeName = "Test Individual Attribute - Delete";
            base.SQL.Admin_IndividualAttributeGroups_Delete(15, individualAttributeGroupName);
            base.SQL.Admin_IndividualAttributes_Delete(15, individualAttributeGroupName, individualAttributeName);
            base.SQL.Admin_IndividualAttributeGroups_Create(15, individualAttributeGroupName, true);
            base.SQL.Admin_IndividualAttributes_Create(15, individualAttributeGroupName, individualAttributeName, false, false, false, false, true);

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Delete an individual attribute
            test.Portal.Admin_IndividualAttributes_Delete(individualAttributeGroupName, individualAttributeName);

			// Logout of Portal
			test.Portal.Logout();
		}
		#endregion Individual Attributes

		#region Individual Note Types
		#endregion Individual Note Types

		#region Individual Requirements
		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Attempts to create an individual requirement without providing any of the required data.")]
		public void Admin_PeopleSetup_IndividualRequirements_CreateNoData() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Attempt to create a requirement with no data
            test.Portal.Admin_IndividualRequirements_Create(null);

			// Logout of portal
			test.Portal.Logout();
		}
		#endregion Individual Requirements

		#region Relationship Types
		#endregion Relationship Types

		#region Schools
		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Creates a School.")]
		public void Admin_PeopleSetup_Schools_Create() {
            // Set initial conditions
            string schoolName = "Test School";
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            base.SQL.Admin_Schools_Delete(15, schoolName);

			// Login to Portal
			test.Portal.Login();

			// Create a school
            test.Portal.Admin_Schools_Create("University", schoolName);

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Matthew Sneeden")]
		[Description("Updates a School.")]
		public void Admin_PeopleSetup_Schools_Update() {
            // Set initial conditions
            string schoolName = "Test School - Update";
            string schoolNameUpdated = "Test School - Mod";
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            base.SQL.Admin_Schools_Delete(15, schoolName);
            base.SQL.Admin_Schools_Delete(15, schoolNameUpdated);
            base.SQL.Admin_Schools_Create(15, schoolName, 102);

			// Login to Portal
			test.Portal.Login();

			// Update a school
			test.Portal.Admin_Schools_Update("University", schoolName, "Home School", schoolNameUpdated);

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Deletes a School.")]
		public void Admin_PeopleSetup_Schools_Delete() {
            // Set initial conditions
            string schoolName = "Test School - Delete";
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            base.SQL.Admin_Schools_Delete(15, schoolName);
            base.SQL.Admin_Schools_Create(15, schoolName, 102);

			// Login to Portal
			test.Portal.Login();

			// Delete a school
			test.Portal.Admin_Schools_Delete(schoolName);

			// Logout of Portal
			test.Portal.Logout();
		}
		#endregion Schools
    }
}