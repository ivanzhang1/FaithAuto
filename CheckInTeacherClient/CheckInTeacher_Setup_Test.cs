using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Threading.Tasks;
using MbUnit.Framework;
using Gallio.Framework;

namespace FTTests.CheckInTeacher
{

    // [TestFixture]
    public class CheckIn_SignIn : FixtureBaseWebDriver
    {
        private string attendenceId = "";

        int churchId = 15;//15:DC 256:QAEUNLX0C4  
        private string ministryName = "Bible Study";//"Bible Study";"Youth"
        private string activityName = "ActivityForCheckInTest0813";//ActivityForCheckInTest0730C ActivityForCheckInTest0730
        private static String rosterNamepre = "RosterCheckInTest0813";
        private string[] rosterNameArray = { rosterNamepre + "001", rosterNamepre + "002", rosterNamepre + "003", rosterNamepre + "004", rosterNamepre + "005", rosterNamepre + "006", rosterNamepre + "007", rosterNamepre + "008", rosterNamepre + "009", rosterNamepre + "010", rosterNamepre + "011", rosterNamepre + "012", rosterNamepre + "013" };
        private string activityScheduleName = "ScheduleForCheckInTest0813";

        private String individualId = "30047331";//30047327  30047329 30047330  30047331 30048834  30049458-gracechurch  'gracezz1 30050229' 'gracezz2 30050240' 'gracezz3 30050248'
        private String checkInTime = "2015-08-14 02:37:23.000";
        private String individualTypeId = "2";//student 1,teacher 2 3 4 101

        private string[] rosterNameAddArray = { rosterNamepre + "a001", rosterNamepre + "a002" };

        // [Test, RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("test create Activity and Schedule for Check-In-Teacher")]
        public void CheckInTeacher_SignIn_TestDeleteActivityForCheckin()
        {
            base.SQL.Ministry_ActivitySchedules_DeleteActivity(churchId, activityName);
        }

        // [Test, RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("test create Activity and Schedule for Check-In-Teacher")]
        public void CheckInTeacher_SignIn_TestCreateActivityForCheckin()
        {

            int ministryId = base.SQL.Ministry_Ministries_FetchID(churchId, ministryName);
            base.SQL.Ministry_Activities_Delete_Activity(churchId, activityName);
            //create Activity
            base.SQL.Ministry_Activities_CreateForCheckIn(churchId, ministryId, activityName, rosterNameArray);
            //create schedule
            base.SQL.Ministry_ActivitySchedules_CreateForCheckIn(churchId, activityName, activityScheduleName, rosterNameArray, TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(0), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(23));

        }

        // [Test, RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("test create roster  for Check-In-Teacher")]
        public void CheckInTeacher_SignIn_TestCreateRosterForCheckIn()
        {
            int ministryId = base.SQL.Ministry_Ministries_FetchID(churchId, ministryName);
            //create Roster
            base.SQL.Ministry_Rosters_CreateForCheckIn(churchId, ministryId, activityName, rosterNameArray);
            //create schedule
            for (int i = 0; i < rosterNameAddArray.Length; i++)
            {
                base.SQL.Ministry_Active_Detail_CheckIn(churchId, activityName, rosterNameAddArray[i], TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(0));
            }

        }

        // [Test(Order = 1), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("test Creates an attendance record")]
        public void CheckInTeacher_SignIn_CheckIn_Teacher()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];

            String churchCode = test.CheckIn.ChurchCode;
            int churchId = base.SQL.Ministry_Church_FetchID(churchCode);
            String timeZoneName = base.SQL.Ministry_Activity_Instance_TimeZone(churchId);
            DateTime currentTimzoneTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));
            String curentDate = Convert.ToString(currentTimzoneTime.Year) + Convert.ToString(currentTimzoneTime.Month) + Convert.ToString(currentTimzoneTime.Day);
            int interalHour = 24 - currentTimzoneTime.Hour - 1;
            if (interalHour < 1)
            {
                TestLog.WriteLine("Time is not enough!");
            }
            string ministryName = "Bible Study";
            string activityName = "ActivityForCheckIn" + curentDate;
            TestLog.WriteLine("-activityName = {0}", activityName);
            String rosterNamepre = "RosterCheckIn" + curentDate;
            string[] rosterNameArray = { rosterNamepre + "001", rosterNamepre + "002", rosterNamepre + "003", rosterNamepre + "004", rosterNamepre + "005" };
            string activityScheduleName = "ScheduleForCheckIn" + curentDate;
            TestLog.WriteLine("-activityScheduleName = {0}", activityScheduleName);

            TestLog.WriteLine("Test Creates an attendance record  ---!");
            String url = test.CheckIn.GetCheckInURl();
            String json = test.CheckIn.GenerateActiveCheckInJson(activityName, individualId, individualTypeId, rosterNameArray[0]);// grace1:30047327   grace2:30047329  grace3: 30047330 grace4:30047331 grace5:30048834
            TestLog.WriteLine("-createUrl = {0}", url);
            TestLog.WriteLine("-createJson = {0}", json);

            APIBase api = new APIBase();
            String responseString = api.SendAPIRequestwithBodyNoAuth(url, "POST", json, HttpStatusCode.Created);
            TestLog.WriteLine("-createResponseString = {0}", responseString);

            Assert.Contains(responseString, "{\"id\":");
            Assert.Contains(responseString, "\"url\":");
            attendenceId = test.CheckIn.GetValueByStrKey(responseString, "url", "attendances/");
            TestLog.WriteLine("-createAttendenceId = {0}", attendenceId);
            test.CheckIn.UpdateIndividual_Instance(activityName, individualId, 16729, checkInTime);
        }

        // [Test(Order = 5), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("test individuals")]
        public void CheckInTeacher_SignIn_CheckIn_Students()
        {
            individualTypeId = "1";
            TestBaseWebDriver Test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Test individuals--!");

            Test.CheckIn.CheckInMultipleIndividuals(activityName, 20, 16729, checkInTime, individualTypeId, rosterNameArray[0]);

        }

        // [Test(Order = 1), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("test Creates an attendance record")]
        public void CheckInTeacher_SignIn_CheckIn_TeacherForSchedule()
        {
            String activityName = "GraceBibleStudy";
            String activityInstanceId = "33968158";
            string[] rosterNameArray = { "roster1", "roster2", "roster3", "roster4", "roster072001", "roster072002", "roster072003", "roster072004", "roster072005", };
            // string activityScheduleName = "scheduleeveryday";

            String individualId = "30047327";//30047327  30047329 30047330  30047331 30048834  30049458-gracechurch
            String checkInTime = "2015-08-13 00:50:00.000";
            String individualTypeId = "2";//student 1,teacher 2 3 4 101

            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Test Creates an attendance record  ---!");
            String url = test.CheckIn.GetCheckInURl();
            String json = test.CheckIn.GenerateActiveCheckInJson(activityName, individualId, individualTypeId, rosterNameArray[0], activityInstanceId);// grace1:30047327   grace2:30047329  grace3: 30047330 grace4:30047331 grace5:30048834
            TestLog.WriteLine("-createUrl = {0}", url);
            TestLog.WriteLine("-createJson = {0}", json);

            APIBase api = new APIBase();
            String responseString = api.SendAPIRequestwithBodyNoAuth(url, "POST", json, HttpStatusCode.Created);
            TestLog.WriteLine("-createResponseString = {0}", responseString);

            Assert.Contains(responseString, "{\"id\":");
            Assert.Contains(responseString, "\"url\":");
            attendenceId = test.CheckIn.GetValueByStrKey(responseString, "url", "attendances/");
            TestLog.WriteLine("-createAttendenceId = {0}", attendenceId);
            test.CheckIn.UpdateIndividual_Instance(activityName, individualId, 16729, checkInTime, activityInstanceId);
        }

        // [Test(Order = 5), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("test individuals")]
        public void CheckInTeacher_SignIn_CheckIn_StudentsForSchedule()
        {
            String activityName = "GraceBibleStudy";
            String activityInstanceId = "33968158";//33968143(0729) 33968144(0730)
            string[] rosterNameArray = { "roster1", "roster2", "roster3", "roster4", "roster072001", "roster072002", "roster072003", "roster072004", "roster072005", };
            // string activityScheduleName = "scheduleeveryday";

            String checkInTime = "2015-08-13 00:50:00.000";
            String individualTypeId = "1";//student 1,teacher 2 3 4 101

            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Test individuals--!");

            String[] attendenceids = test.CheckIn.CheckInMultipleIndividuals(activityName, 10, 16729, checkInTime, individualTypeId, rosterNameArray[0], activityInstanceId);

            for (int i = 0; i < attendenceids.Length; i++)
            {
                TestLog.WriteLine("-attendenceids = {0}", attendenceids[i]);
            }


        }

        // [Test(Order = 2), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("test Delete an attendance record")]
        public void CheckInTeacher_SignIn_TestDeleteOneCheckIn()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Test Delete an attendance record  ---!");
            attendenceId = "85490540";
            String url = test.CheckIn.GetCheckOutURl(attendenceId);//85490313 85490316 85490315 85490316
            TestLog.WriteLine("-deleteUrl = {0}", url);

            APIBase api = new APIBase();
            String responseString = api.SendAPIRequestNoAuth(url, "DELETE", HttpStatusCode.NoContent);
            TestLog.WriteLine("-deleteResponseString = {0}", responseString);

        }

        // [Test, RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("test")]
        public void CheckInTeacher_SignIn_TestJsom()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Test Json  ---!");
            String url = test.CheckIn.GetCheckInURl();
            String json = "";
            //String json = test.CheckIn.generateActiveCheckInJson(activityName, individualId, individualTypeId);
            TestLog.WriteLine("-url = {0}", url);
            TestLog.WriteLine("-json = {0}", json);

            //Data
            int individualId = base.SQL.People_Individuals_FetchID(churchId, "Graceteacher1 Zhang");
            int individual2Id = base.SQL.People_Individuals_FetchID(churchId, "Graceteacher2 Zhang");
            int individual3Id = base.SQL.People_Individuals_FetchID(churchId, "Graceteacher3 Zhang");
            int individual4Id = base.SQL.People_Individuals_FetchID(churchId, "Graceteacher4 Zhang");
            int individual5Id = base.SQL.People_Individuals_FetchID(churchId, "Graceteacher5 Zhang");
            TestLog.WriteLine("-individualId = {0}", individualId);
            TestLog.WriteLine("-individual2Id = {0}", individual2Id);
            TestLog.WriteLine("-individual3Id = {0}", individual3Id);
            TestLog.WriteLine("-individual4Id = {0}", individual4Id);
            TestLog.WriteLine("-individual5Id = {0}", individual5Id);

        }

        // [Test(Order = 3), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("test Delete an attendance record")]
        public void CheckInTeacher_SignIn_TestIndividualTypeId()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Test   individualTypeId---!");
            // String url = test.CheckIn.generateActiveCheckInURl();

            String individualTypeId = test.CheckIn.GetIndividualTypeId("GraceBibleStudy", "30045861");
            //select INDIVIDUAL_TYPE_NAME from ChmActivity.dbo.INDIVIDUAL_TYPE  where INDIVIDUAL_TYPE_ID =1
            //getIndividualTypeName(String individualTypeId)

            TestLog.WriteLine("-individualTypeId = {0}", individualTypeId);

            String individualTypeName1 = test.CheckIn.GetIndividualTypeName("1");
            String individualTypeName2 = test.CheckIn.GetIndividualTypeName("2");
            String individualTypeName3 = test.CheckIn.GetIndividualTypeName("3");
            String individualTypeName4 = test.CheckIn.GetIndividualTypeName("4");
            TestLog.WriteLine("-individualTypeName1 = {0}", individualTypeName1);
            TestLog.WriteLine("-individualTypeName2 = {0}", individualTypeName2);
            TestLog.WriteLine("-individualTypeName3 = {0}", individualTypeName3);
            TestLog.WriteLine("-individualTypeName4 = {0}", individualTypeName4);

        }

        // [Test(Order = 4), RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("test individuals")]
        public void CheckInTeacher_SignIn_TestIndividuals()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            TestLog.WriteLine("Test individuals--!");

            String[] individualsId = test.CheckIn.GetMultipleIndividuals(10);
            for (int i = 0; i < individualsId.Length; i++)
            {
                TestLog.WriteLine("-individualsId = {0}", individualsId[i]);
            }

        }

        [Test, RepeatOnFailure]
        [Author("Grace Zhang")]
        [Description("test create Activity and Schedule for Check-In-Teacher")]
        public void CheckInTeacher_SignIn_Test()
        {
            DateTime.ParseExact("8/21/2015 3:16:08 AM", "M/dd/yyyy h:mm:ss tt", 
                System.Globalization.CultureInfo.InvariantCulture);
        }
        //select INDIVIDUAL_TYPE_ID from chmactivity.dbo.individual_instance  where INDIVIDUAL_ID = 30045861  and ACTIVITY_INSTANCE_ID =33951892 and  CHURCH_ID =15 
    }

}
