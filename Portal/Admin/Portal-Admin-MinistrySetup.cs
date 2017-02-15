using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace FTTests.Portal.Admin {
	[TestFixture]
	public class Portal_Admin_MinistrySetup : FixtureBase {
		#region Ministries
		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Attempts to create a Ministry with no data.")]
		public void Admin_MinistrySetup_Ministries_CreateNoData() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Attempt to create a ministry without the required data
            test.Portal.Admin_Ministries_Create(null);

			// Logout of portal
			test.Portal.Logout();
		}
		#endregion Ministries

		#region Head Count Attributes
		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Attempts to create a Head Count Attribute with no data.")]
		public void Admin_MinistrySetup_HeadCountAttributes_CreateNoData() {
			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Craete a head count attribute
            test.Portal.Admin_HeadCountAttributes_Create(null, true);

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Creates a Head Count Attribute.")]
		public void Admin_MinistrySetup_HeadCountAttributes_Create() {
            // Set initial conditions
            string headCountAttributeName = "Test Head Count Attribute";
            base.SQL.Admin_HeadCountAttributes_Delete(15, headCountAttributeName);

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Craete a head count attribute
            test.Portal.Admin_HeadCountAttributes_Create(headCountAttributeName, true);

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Updates a Head Count Attribute.")]
		public void Admin_MinistrySetup_HeadCountAttributes_Update() {
            // Set initial conditions
            string headCountAttributeName = "Test Head Count Attribute - U";
            string headCountAttributeNameUpdated = "Test Head Count Attribute - Updated";
            base.SQL.Admin_HeadCountAttributes_Delete(15, headCountAttributeName);
            base.SQL.Admin_HeadCountAttributes_Delete(15, headCountAttributeNameUpdated);
            base.SQL.Admin_HeadCountAttributes_Create(15, headCountAttributeName, true);

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Update a head count attribute
            test.Portal.Admin_HeadCountAttributes_Update(headCountAttributeName, true, headCountAttributeNameUpdated, false);

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Updates a Head Count Attribute.")]
		public void Admin_MinistrySetup_HeadCountAttributes_Delete() {
            // Set initial conditions
            string headCountAttributeName = "Test Head Count Attribute - Delete";
            base.SQL.Admin_HeadCountAttributes_Delete(15, headCountAttributeName);
            base.SQL.Admin_HeadCountAttributes_Create(15, headCountAttributeName, false);

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Delete a head count attribute
            test.Portal.Admin_HeadCountAttributes_Delete(headCountAttributeName);

			// Logout of Portal
			test.Portal.Logout();
		}
		#endregion Head Count Attributes

		#region Activity Types
		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Matthew Sneeden")]
		[Description("Attempts to create an Activity Type with no data.")]
		public void Admin_MinistrySetup_ActivityTypes_CreateNoData() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Create an activity type
			test.Portal.Admin_ActivityTypes_Create(null, true);

			// Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Creates an Activity Type.")]
		public void Admin_MinistrySetup_ActivityTypes_Create() {
            // Set initial conditions
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string activityTypeName = "Test Activity Type_1";
            base.SQL.Admin_ActivityTypesDelete(15, activityTypeName);

			// Login to Portal
			test.Portal.Login();

            try
            {
                // Create an activity type
                test.Portal.Admin_ActivityTypes_Create(activityTypeName, true);
            }
            finally
            {
                base.SQL.Admin_ActivityTypesDelete(15, activityTypeName);

                // Logout of Portal
                test.Portal.Logout();
            }
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Updates an Activity Type.")]
		public void Admin_MinistrySetup_ActivityTypes_Update() {
            // Set initial conditions
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string activityTypeName = "Test Activity Type - Update";
            string activityTypeNameUpdated = "Test Activity Type - Mod";
            base.SQL.Admin_ActivityTypesDelete(15, activityTypeName);
            base.SQL.Admin_ActivityTypesDelete(15, activityTypeNameUpdated);
            base.SQL.Admin_ActivityTypesCreate(15, activityTypeName, true);

			// Login to Portal
			test.Portal.Login();

            try
            {
                // Update an activity type
                test.Portal.Admin_ActivityTypes_Update(activityTypeName, true, activityTypeNameUpdated, true);
            }
            finally
            {
                base.SQL.Admin_ActivityTypesDelete(15, activityTypeName);
                base.SQL.Admin_ActivityTypesDelete(15, activityTypeNameUpdated);

                // Logout of Portal
                test.Portal.Logout();
            }
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Deletes an Activity Type.")]
		public void Admin_MinistrySetup_ActivityTypes_Delete() {
            // Set initial conditions
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            string activityTypeName = "Test Activity Type - Delete";
            base.SQL.Admin_ActivityTypesDelete(15, activityTypeName);
            base.SQL.Admin_ActivityTypesCreate(15, activityTypeName, true);

			// Login to Portal
			test.Portal.Login();

			// Delete an activity type
            test.Portal.Admin_ActivityTypes_Delete(activityTypeName);

			// Logout of Portal
			test.Portal.Logout();
		}
		#endregion Activity Types

		#region Job Attributes
		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Attempts to create a Job Attribute with no data.")]
		public void Admin_MinistrySetup_JobAttributes_CreateNoData() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Create a job attribute group without specifying a name
            test.Portal.Admin_JobAttributeGroups_Create(null);

			// Logout of portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
		[Author("Matthew Sneeden")]
		[Description("Creates a Job Attribute Group.")]
		public void Admin_MinistrySetup_JobAttributeGroups_Create() {
            // Set initial conditions
            string jobAttributeGroupName = "Test Attribute Group";
            base.SQL.Admin_JobAttributeGroups_Delete(15, jobAttributeGroupName);

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Create a job attribute group
            test.Portal.Admin_JobAttributeGroups_Create(jobAttributeGroupName);

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Deletes a Job Attribute Group.")]
		public void Admin_MinistrySetup_JobAttributeGroups_Delete() {
            // Set initial conditions
            string jobAttributeGroupName = "Test Attribute Group - Delete";
            base.SQL.Admin_JobAttributeGroups_Delete(15, jobAttributeGroupName);
            base.SQL.Admin_JobAttributeGroups_Create(15, jobAttributeGroupName);

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Delete a job attribute group
            test.Portal.Admin_JobAttributeGroups_Delete(jobAttributeGroupName);

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Creates a Job Attribute tied to a Job Attribute Group.")]
		public void Admin_MinistrySetup_JobAttributes_Create() {
            // Set initial conditions
            string jobAttributeGroupName = "Test Attribute Group - A:Create";
            string jobAttributeName = "Test Attribute";
            base.SQL.Admin_JobAttributes_Delete(15, jobAttributeGroupName, jobAttributeName);
            base.SQL.Admin_JobAttributeGroups_Delete(15, jobAttributeGroupName);
            base.SQL.Admin_JobAttributeGroups_Create(15, jobAttributeGroupName);

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Create a job attribute
			test.Portal.Admin_JobAttributes_Create(jobAttributeGroupName, jobAttributeName);

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Updates a Job Attribute tied to a Job Attribute Group.")]
		public void Admin_MinistrySetup_JobAttributes_Update() {
            // Set initial conditions
            string jobAttributeGroupName = "Test Attribute Group - A:Update";
            string jobAttributeName = "Test Attribute - Mod";
            string jobAttributeNameUpdated = "Test Attribute - Updated";
            base.SQL.Admin_JobAttributes_Delete(15, jobAttributeGroupName, jobAttributeName);
            base.SQL.Admin_JobAttributes_Delete(15, jobAttributeGroupName, jobAttributeNameUpdated);
            base.SQL.Admin_JobAttributeGroups_Delete(15, jobAttributeGroupName);
            base.SQL.Admin_JobAttributeGroups_Create(15, jobAttributeGroupName);
            base.SQL.Admin_JobAttributes_Create(15, jobAttributeGroupName, jobAttributeName, 65211);

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Update a job attribute
            test.Portal.Admin_JobAttributes_Update(jobAttributeGroupName, jobAttributeName, jobAttributeNameUpdated);

			// Logout of Portal
			test.Portal.Logout();
		}

		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Deletes a Job Attribute tied to a Job Attribute Group.")]
		public void Admin_MinistrySetup_JobAttributes_Delete() {
            // Set initial conditions
            string jobAttributeGroupName = "Test Attribute Group - A:Delete";
            string jobAttributeName = "Test Attribute - Delete";
            base.SQL.Admin_JobAttributes_Delete(15, jobAttributeGroupName, jobAttributeName);
            base.SQL.Admin_JobAttributeGroups_Delete(15, jobAttributeGroupName);
            base.SQL.Admin_JobAttributeGroups_Create(15, jobAttributeGroupName);
            base.SQL.Admin_JobAttributes_Create(15, jobAttributeGroupName, jobAttributeName, 65211);

			// Login to Portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Delete a job attribute
            test.Portal.Admin_JobAttributes_Delete(jobAttributeGroupName, jobAttributeName);

			// Logout of Portal
			test.Portal.Logout();
		}
		#endregion Job Attributes

		#region Job Information
		[Test, RepeatOnFailure]
		[Author("Matthew Sneeden")]
		[Description("Attempts to create a Job Information with no data.")]
		public void Admin_MinistrySetup_JobInformation_CreateNoData() {
			// Login to portal
			TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
			test.Portal.Login();

			// Attempt to create a job information without required data      
			test.Portal.Admin_JobInformation_Create(null, null);

			// Logout of portal
			test.Portal.Logout();
		}
		#endregion Job Information

        #region Volunteer Types
        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create a Volunteer Type with no data.")]
        public void Admin_MinistrySetup_VolunteerTypes_CreateNoData()
        {
            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Attempt to create a volunteer type with missing required fields
            test.Portal.Admin_VolunteerTypes_Create(null, true);

            // Logout of portal
            test.Portal.Logout();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Attempts to create a Volunteer Type with name data.")]
        public void Admin_MinistrySetup_VolunteerTypes_Create()
        {
            // Set initial conditions
            string volunteerTypeName = "Test Volunteer Type";
            this.SQL.Admin_VolunteerTypes_Delete(15, volunteerTypeName);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            try
            {
                // Create a volunteer type
                test.Portal.Admin_VolunteerTypes_Create(volunteerTypeName, true);
            } finally 
            {
                this.SQL.Admin_VolunteerTypes_Delete(15, volunteerTypeName);

                // Logout of portal
                test.Portal.Logout();
            }
           


            
        }
        #endregion Volunteer Types
    }

}