using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Threading.Tasks;
using MbUnit.Framework;
using Gallio.Framework;
using System.Threading;

namespace FTTests.CheckInTeacher
{
    [TestFixture]
    public class PreviousCheckIn : FixtureBaseWebDriver
    {

        TestBaseWebDriver test;
        private int churchId;
        private string timeZoneName;
        DateTime currentTimzoneTime;
        string currentDate;
        string[] rosterNameArray;
        string rosterToCheckIn;
        string activityName;
        string activityScheduleName;
        string ministryName = "A Test Ministry"; // A Test Ministry, Bible Study

        [SetUp]
        public void PreCheckIn_SetUp()
        {
            test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            churchId = base.SQL.Ministry_Church_FetchID(test.CheckIn.ChurchCode);
            timeZoneName = base.SQL.Ministry_Activity_Instance_TimeZone(churchId);

            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            currentDate = Convert.ToString(currentTimzoneTime.Year) + Convert.ToString(currentTimzoneTime.Month) + Convert.ToString(currentTimzoneTime.Day);

            // Activity resource naming
            // activityName = "ActivityCheckIn" + currentDate;
            activityName = "ActivityCheckIn-T3";
            TestLog.WriteLine("-activityName = {0}", activityName);

            activityScheduleName = "ScheduleForCheckIn";
            TestLog.WriteLine("-activityScheduleName = {0}", activityScheduleName);

            string rosterNamepre = "Roster_";
            rosterNameArray = new string[] { rosterNamepre + "_001", rosterNamepre + "_002", rosterNamepre + "_003", rosterNamepre + "_004", rosterNamepre + "_005", rosterNamepre + "_005", rosterNamepre + "_006", rosterNamepre + "_007", rosterNamepre + "_008", rosterNamepre + "_009", rosterNamepre + "_010" };
            rosterToCheckIn = rosterNameArray[0];
        }

        #region Precondition-DeleteActivity

        // [Test]
        [Author("Mady Kou")]
        [Description("Delete the activity for check in automation")]
        public void CheckInTeacher_AfterCheckIn_DeleteActivities_All()
        {
            string[] activitiesCheckIn = base.SQL.Ministry_Activities_FetchNameList(churchId, "ActivityCheckIn-T3");
            foreach (string name in activitiesCheckIn)
            {
                TestLog.WriteLine("Try to delete the activity: " + name);
                base.SQL.Ministry_ActivitySchedules_DeleteActivity(churchId, name);
                TestLog.WriteLine("Complette for deleting the activity: " + name);
            }

        }

        // [Test]
        [Author("Mady Kou")]
        [Description("Delete the activity for check in automation")]
        public void CheckInTeacher_AfterCheckIn_DeleteActivity()
        {
            base.SQL.Ministry_ActivitySchedules_DeleteActivity(churchId, activityName);
        }

        #endregion Precondition-DeleteActivity

        #region Precondition-CheckIn

        // [Test]
        [Author("Grace Zhang")]
        [Description("Precondition-CheckIn")]
        public void CheckInTeacher_PreCheckIn()
        {
            // Set under which ministry the activity wil be created
            test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            int interalHour = 24 - currentTimzoneTime.Hour - 1;

            if (interalHour < 1)
            {
                TestLog.WriteLine("The rest of the day may not be enough for test execution, please wait!");
                Thread.Sleep(60 * 60 * 1000);
            }

            int ministryId = base.SQL.Ministry_Ministries_FetchID(churchId, ministryName);

            base.SQL.Ministry_ActivitySchedules_DeleteActivity(churchId, activityName);

            //create Activity
            base.SQL.Ministry_Activities_CreateForCheckIn(churchId, ministryId, activityName, rosterNameArray);
            //create schedule
            base.SQL.Ministry_ActivitySchedules_CreateForCheckIn(churchId, activityName, activityScheduleName, rosterNameArray, 
                currentTimzoneTime.AddHours(0), currentTimzoneTime.AddHours(interalHour));

            //Teacher Check-in
            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            String individualId = Convert.ToString(base.SQL.People_Individuals_FetchIDByEmail(churchId, test.CheckIn.CheckInUsername));

            test.CheckIn.CheckInATeacher(churchId, activityName, individualId, rosterToCheckIn, currentTimzoneTime);

            //Students Check-in
            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));//get new time
            string checkInTime = currentTimzoneTime.ToString();
            String[] individualNameArray = { "Christopher Kemp", "Emma Kemp", "Kayla Kemp", "Lorraine Kemp", "Chris Kemp", "Chris Balisteri" };
            string individualTypeId = "1";
            TestLog.WriteLine("Test individuals--!");

            String[] attendenceids = test.CheckIn.CheckInMultipleIndividuals(activityName, individualNameArray, 16729, checkInTime, individualTypeId, rosterToCheckIn);

            for (int i = 0; i < attendenceids.Length; i++)
            {
                TestLog.WriteLine("-attendenceids = {0}", attendenceids[i]);
            }
        }
        #endregion Precondition-CheckIn

        // [Test]
        [Author("Mady Kou")]
        [Description("Precondition-CheckIn some participants")]
        public void CheckInTeacher_PreCheckIn_Students()
        {
            int newStudentNum = 17;

            test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));

            test.CheckIn.CheckInMultipleIndividuals(activityName, newStudentNum, 16729, currentTimzoneTime.ToString(), "1", rosterNameArray[0]);
        }

        // [Test]
        [Author("Mady Kou")]
        [Description("Precondition-CheckIn a new teacher")]
        public void CheckInTeacher_PreCheckIn_Teacher()
        {
            string newTeacherEmail = "graceteacher4@163.com";
            // string newTeacherEmail = "mady.kou@activenetwork.com";

            test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            String individualId = Convert.ToString(base.SQL.People_Individuals_FetchIDByEmail(churchId, newTeacherEmail));

            test.CheckIn.CheckInATeacher(churchId, activityName, individualId, rosterNameArray[2], currentTimzoneTime);
        }
        //[Test]
        [Author("Grace Zhang")]
        [Description("Create a last attendance record for a student")]
        public void CheckInTeacher_CreateLastAttendance()
        {
            test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.CheckIn.CreateLastAttendance(churchId, activityName, "Emma Kemp", rosterNameArray[2]);
        }
        //[Test]
        [Author("Grace Zhang")]
        [Description("Update Birthday for a student")]
        public void CheckInTeacher_UpdateBirthday()
        {
            test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.CheckIn.UpdateBirthdayForCheckinTeacher(churchId, "Emma Kemp");
        }
        //[Test]
        [Author("Grace Zhang")]
        [Description("Update Tag Comment for a student")]
        public void CheckInTeacher_UpdateTagComment()
        {
            test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.SQL.People_Individuals_UpdateTagCommentByName(churchId, "Emma Kemp","sssss");
        }
        //[Test]
        [Author("Grace Zhang")]
        [Description("Update Tag Comment for a student")]
        public void CheckInTeacher_UpdateTagCode()
        {
            test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.SQL.People_Individuals_UpdateTagCode(churchId, activityName, "Emma Kemp", "cod");
        }
        // [Test]
        [Author("Grace Zhang")]
        [Description("Update Tag Comment for a student")]
        public void CheckInTeacher_GetIndex()
        {
            test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            //Login
            test.CheckIn.LoginWebDriver();
            int index = test.CheckIn.GetListIndexByStudentName( "Emma",null, "Kemp");
            TestLog.WriteLine("-index= {0}", index);
        }
       
    }
}
