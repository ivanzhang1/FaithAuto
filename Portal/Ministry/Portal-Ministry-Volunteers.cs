using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace FTTests.Portal.Ministry {
    [TestFixture, Ignore]
    public class Portal_Ministry_VolunteersWebDriver : FixtureBaseWebDriver {
        private string _ministryName = "A Test Ministry";
        private string _activityName = "A Test Activity";

        #region Staffing Assignment
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Verifies you can create a staffing assignment.")]
        public void Ministry_Volunteers_StaffingAssignment_Create() {
            // Set initial conditions
            string staffingScheduleName = "Test Schedule - Assignment:C";
            base.SQL.Ministry_StaffingAssignments_Delete(15, base.SQL.IndividualID, _activityName, staffingScheduleName);
            base.SQL.Ministry_StaffingSchedules_Delete(15, _activityName, staffingScheduleName);
            base.SQL.Ministry_StaffingSchedules_Create(15, _activityName, staffingScheduleName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            try
            {
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                // Create a staffing assignment
                test.Portal.Ministry_StaffingAssignment_Create(_ministryName, "Matthew Sneeden", _activityName, "Staff", true, null, null, null, null, staffingScheduleName, null);
            }
            finally
            {
                // Logout of portal
                test.Portal.LogoutWebDriver();
                base.SQL.Ministry_StaffingAssignments_Delete(15, base.SQL.IndividualID, _activityName, staffingScheduleName);
                base.SQL.Ministry_StaffingSchedules_Delete(15, _activityName, staffingScheduleName);
            }
        }

        //comment by ivan.zhang since we had removed all features related with legacy. 09/24/2015
        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Verifies you can delete a staffing assignment.")]
        public void Ministry_Volunteers_StaffingAssignment_Delete() {
            // Set initial conditions
            string staffingScheduleName = "Test Schedule - Assignment:D";
            base.SQL.Ministry_StaffingAssignments_Delete(15, base.SQL.IndividualID, _activityName, staffingScheduleName);
            base.SQL.Ministry_StaffingSchedules_Delete(15, _activityName, staffingScheduleName);
            base.SQL.Ministry_StaffingSchedules_Create(15, _activityName, staffingScheduleName);
            base.SQL.Ministry_StaffingAssignments_Create(15, base.SQL.IndividualID, 2, _activityName, staffingScheduleName);

            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            try
            {
                test.Portal.LoginWebDriver("msneeden", "Pa$$w0rd", "dc");

                // Delete a staffing assignment
                test.Portal.Ministry_StaffingAssignment_Delete(_ministryName, "Matthew Sneeden", true, _activityName, "Staff", "n/a", staffingScheduleName, "n/a", "n/a", "n/a", "n/a");
            }
            finally
            {
                // Logout of portal
                test.Portal.LogoutWebDriver();
                base.SQL.Ministry_StaffingAssignments_Delete(15, base.SQL.IndividualID, _activityName, staffingScheduleName);
                base.SQL.Ministry_StaffingSchedules_Delete(15, _activityName, staffingScheduleName);
            }
        }
        #endregion Staffing Assignment
    }

    [TestFixture]
    public class Portal_Ministry_Volunteers : FixtureBase {
        private string _ministryName = "A Test Ministry";
        private string _activityName = "A Test Activity";

        #region Jobs
        #endregion Jobs

        #region Schedules
        // Schedules of Volunteers hyperlink was removed by FO-4800. 09/24/2015. commented by ivan.zhang
        //[Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Creates a Schedule.")]
        public void Ministry_Volunteers_Schedules_Create() {
            // Set initial conditions
            string scheduleName = "Test Schedule";
            base.SQL.Ministry_StaffingSchedules_Delete(15, _activityName, scheduleName);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.Login();

            // Create a schedule
            test.Portal.Ministry_Schedules_Create(scheduleName);

            // Logout of portal
            test.Portal.Logout();
        }

        // Schedules of Volunteers hyperlink was removed by FO-4800. 09/24/2015. commented by ivan.zhang
        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Updates a Schedule.")]
        public void Ministry_Volunteers_Schedules_Update() {
            // Set initial conditions
            string scheduleName = "Test Schedule - Update";
            string scheduleNameUpdated = "Test Schedule - Updated";
            base.SQL.Ministry_StaffingSchedules_Delete(15, _activityName, scheduleName);
            base.SQL.Ministry_StaffingSchedules_Delete(15, _activityName, scheduleNameUpdated);
            base.SQL.Ministry_StaffingSchedules_Create(15, _activityName, scheduleName);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            try
            {
                test.Portal.Login();

                // Update a schedule
                test.Portal.Ministry_Schedules_Update(scheduleName, scheduleNameUpdated);

                // Logout of portal
                test.Portal.Logout();
            }
            finally
            {
                base.SQL.Ministry_StaffingSchedules_Delete(15, _activityName, scheduleName);
                base.SQL.Ministry_StaffingSchedules_Delete(15, _activityName, scheduleNameUpdated);
            }
        }

        // Schedules of Volunteers hyperlink was removed by FO-4800. 09/24/2015. commented by ivan.zhang
        //[Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Deletes a Schedule.")]
        public void Ministry_Volunteers_Schedules_Delete() {
            // Set initial conditions
            string scheduleName = "Test Schedule - Delete";
            base.SQL.Ministry_StaffingSchedules_Delete(15, _activityName, scheduleName);
            base.SQL.Ministry_StaffingSchedules_Create(15, _activityName, scheduleName);

            // Login to portal
            TestBase test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            try
            {
                test.Portal.Login();

                // Delete a schedule
                test.Portal.Ministry_Schedules_Delete(scheduleName);

                // Logout of portal
                test.Portal.Logout();
            }
            finally
            {
                base.SQL.Ministry_StaffingSchedules_Delete(15, _activityName, scheduleName);
            }
        }
        #endregion Schedules

        #region VSO Funnel Report
        #endregion VSO Funnel Report
    }
}